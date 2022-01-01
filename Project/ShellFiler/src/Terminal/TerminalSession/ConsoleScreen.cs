using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Locale;
using ShellFiler.Properties;
using ShellFiler.Terminal.UI;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession {

    //=========================================================================================
    // クラス：コンソールの仮想スクリーン
    //=========================================================================================
    public class ConsoleScreen {
        // 不明な文字の幅
        public const int CHAR_WIDTH_UNKNOWN = 1;            // Parser.CH_REPLACE_CTRLの幅

        // 最小画面サイズ（半角文字単位）
        public const int MIN_CX_CONSOLE_SIZE = 30;
        public const int MIN_CY_CONSOLE_SIZE = 5;

        // 文字コードの定義
        public const char CH_ESC = (char)0x1b;
        public const char CH_BELL = (char)7;
        public const char CH_TAB = (char)9;
        public const char CH_BS = (char)8;
        public const char CH_CR = '\r';
        public const char CH_LF = '\n';
        public const char CH_DEL = (char)0x7f;

        // このコンソールに接続したユーザー名@サーバー名
        private string m_userServer;

        // このコンソールの表示名
        private string m_displayName;

        // このコンソールの開始時刻
        private DateTime m_startTime;

        // shellのチャネル（接続するまでと接続解除後はnull）
        private TerminalShellChannel m_shellChannel;

        // 画面を構成する行（LogLineTerminalを格納）
        private LogBuffer m_logBuffer;

        // 現在のビューに対する半角文字単位での画面の大きさ
        private Size m_screenCharSize;

        // 現在のビューに対するピクセル単位での画面の大きさ
        private Size m_screenPixelSize;

        // Xカーソル位置（半角単位、左端は0）
        private int m_locationCursorX = 0;

        // 仮想画面の先頭行になっているログ行の行ID
        private long m_topCursorLineId;

        // 仮想画面の現在の行になっているログ行の行ID
        private long m_currentCursorLineId;

        // 現在の文字の修飾
        private short m_currentDecoration = ConsoleDecoration.NORMAL;

        // コンソールの状態
        private ConsoleState m_consoleState = new ConsoleState();

        // 文字列のエンコード方式
        private Encoding m_encoding;

        // 直前の解析で残ったバイト列（残っていないとき長さ0の配列）
        private byte[] m_prevBuffer = new byte[0];

        // 仮想画面の変化イベントの通知先
        private ConsoleScreenEventDispatcher m_eventDispatcher;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]userServer    このコンソールに接続したユーザー名@サーバー名
        //         [in]displayName   表示名
        // 　　　　[in]startName     セッション開始時刻
        // 戻り値：なし
        //=========================================================================================
        public ConsoleScreen(string userServer, string displayName, DateTime startTime) {
            m_userServer = userServer;
            m_displayName = displayName;
            m_startTime = startTime;

            m_eventDispatcher = new ConsoleScreenEventDispatcher(this);
            int lineCount = Configuration.Current.TerminalLogLineCount;
            m_logBuffer = new LogBuffer(lineCount);
            m_logBuffer.AddLogLine(new LogLineTerminal());
            m_currentCursorLineId = m_logBuffer.FirstAvailableLineId;
            m_topCursorLineId = m_currentCursorLineId;
        }

        //=========================================================================================
        // 機　能：接続が完了したことを通知する
        // 引　数：[in]status       接続のステータス
        // 　　　　[in]errorDetail  エラーが発生したとき、その詳細情報（エラーではないときnull）
        // 戻り値：送信できたときtrue、すでに閉じられているときfalse
        // メ　モ：通信スレッドで実行される
        //=========================================================================================
        public void NotifyConnect(FileOperationStatus status, string errorDetail) {
            BaseThread.InvokeProcedureByMainThread(new NotifyConnectDelegate(NotifyConnectUI), status, errorDetail);
        }
        private delegate void NotifyConnectDelegate(FileOperationStatus status, string errorDetail);
        private void NotifyConnectUI(FileOperationStatus status, string errorDetail) {
            try {
                if (status.Succeeded) {
                    Encoding encoding = m_shellChannel.ParentConnection.AuthenticateSetting.Encoding;
                    m_encoding = Encoding.GetEncoding(encoding.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ReplacementFallback);
                }
                if (m_shellChannel != null) {
                    // 接続完了時は通知
                    m_shellChannel.NotifyConnect(status, errorDetail);
                }
                Event.NotifyConnect(status, errorDetail);
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionUI, "NotifyConnectUI");
            }
        }

        //=========================================================================================
        // 機　能：コンソールサイズを設定する
        // 引　数：[in]charSize   画面の半角文字単位での大きさ
        // 　　　　[in]pixelSize  画面のピクセル単位での大きさ
        // 戻り値：なし
        //=========================================================================================
        public void SetConsoleSize(Size charSize, Size pixelSize) {
            int cx = Math.Max(MIN_CX_CONSOLE_SIZE, charSize.Width);
            int cy = Math.Max(MIN_CY_CONSOLE_SIZE, charSize.Height);
            m_screenCharSize = new Size(cx, cy);
            m_screenPixelSize = pixelSize;
        }

        //=========================================================================================
        // 機　能：SSHからのデータ到着時の処理を行う
        // 引　数：[in]buffer    受信したデータのバッファ
        // 　　　　[in]offset    受信したデータの開始オフセット
        // 　　　　[in]length    受信したデータの長さ
        // 戻り値：なし
        // メ　モ：UIスレッドに切り替えた後で呼ばれる
        //=========================================================================================
        public void SSHOnDataReceived(byte[] buffer, int offset, int length) {
            if (length == 0) {
                return;
            }
            // 前回分とまとめる
            buffer = ArrayUtils.AppendBuffer(m_prevBuffer, 0, m_prevBuffer.Length, buffer, offset, length);
            offset = 0;
            length = buffer.Length;
            if (m_encoding == null) {
                // 普通はないはずだが、スレッドで初期化完了の前に通信が発生したときは次回に解析する
                m_prevBuffer = buffer;
                return;
            }

            // 解析
            string str = StringUtils.ConvertUncheckedByteToString(m_encoding, buffer, ref offset, ref length);
            if (length == 0) {
                m_prevBuffer = new byte[0];
            } else {
                m_prevBuffer = ArrayUtils.CreateCleanedBuffer(buffer, offset, length);
            }
            Parser parser = new Parser(this, str);
            try {
                String strRemain = parser.Parse();
                if (strRemain.Length > 0) {
                    // エスケープシーケンスなどの残りは次回に解析
                    byte[] remain = m_encoding.GetBytes(strRemain);
                    m_prevBuffer = ArrayUtils.AppendBuffer(remain, 0, remain.Length, m_prevBuffer, 0, m_prevBuffer.Length);
                }

                // ビューに通知
                LogLineChangeLog changeLog = parser.ChangeLog;
                m_eventDispatcher.AddData(changeLog);
            } finally {
                parser.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：SSHクローズのときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：UIスレッドに切り替えた後で呼ばれる
        //=========================================================================================
        public void SSHOnClose() {
            m_consoleState.ChannelClosed = true;
            m_eventDispatcher.NotifyClose();
        }

        //=========================================================================================
        // 機　能：カレットの位置を取得する
        // 引　数：[in]topLineId   先頭行と見なす行の行ID
        // 戻り値：カレットの位置（Y座標は先頭行と見なす行からの相対位置）
        //=========================================================================================
        public Point GetCaretPosition(long topLineId) {
            int x = m_locationCursorX;
            int y = (int)(m_currentCursorLineId - topLineId);
            return new Point(x, y);
        }

        //=========================================================================================
        // 機　能：バックログをクリアする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearBackLog() {
            m_logBuffer.ClearAll();
            m_locationCursorX = 0;
            m_logBuffer.AddLogLine(new LogLineTerminal());
            m_currentCursorLineId = m_logBuffer.FirstAvailableLineId;
            m_topCursorLineId = m_currentCursorLineId;
        }

        //=========================================================================================
        // プロパティ：このコンソールに接続したユーザー名@サーバー名
        //=========================================================================================
        public string UserServer {
            get {
                return m_userServer;
            }
            set {
                m_userServer = value;
            }
        }

        //=========================================================================================
        // プロパティ：このコンソールの表示名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
            set {
                m_displayName = value;
            }
        }

        //=========================================================================================
        // プロパティ：このコンソールの開始時刻
        //=========================================================================================
        public DateTime StartTime {
            get {
                return m_startTime;
            }
        }

        //=========================================================================================
        // プロパティ：shellのチャネル（接続するまでと接続解除後はnull、使用するときはTerminalShellChannelでの同期化が必要）
        //=========================================================================================
        public TerminalShellChannel ShellChannel {
            get {
                return m_shellChannel;
            }
            set {
                m_shellChannel = value;
            }
        }

        //=========================================================================================
        // プロパティ：画面を構成する行（LogLineTerminalを格納）
        //=========================================================================================
        public LogBuffer LogBuffer {
            get {
                return m_logBuffer;
            }
        }

        //=========================================================================================
        // プロパティ：現在のビューに対する半角文字単位での画面の大きさ
        //=========================================================================================
        public Size CharSize {
            get {
                return m_screenCharSize;
            }
        }

        //=========================================================================================
        // プロパティ：現在のビューに対するピクセル単位での画面の大きさ
        //=========================================================================================
        public Size PixelSize {
            get {
                return m_screenPixelSize;
            }
        }

        //=========================================================================================
        // プロパティ：Xカーソル位置（半角単位、左端は0）
        //=========================================================================================
        public int CursorXCharWidth {
            get {
                return m_locationCursorX;
            }
        }

        //=========================================================================================
        // プロパティ：コンソールの状態
        //=========================================================================================
        public ConsoleState CurrentState {
            get {
                return m_consoleState;
            }
        }

        //=========================================================================================
        // プロパティ：文字列のエンコード方式
        //=========================================================================================
        public Encoding Encoding {
            get {
                return m_encoding;
            }
        }

        //=========================================================================================
        // プロパティ：キー入力のモード
        //=========================================================================================
        public TerminalModeType TerminalMode {
            get {
                return m_consoleState.TerminalMode;
            }
        }

        //=========================================================================================
        // プロパティ：仮想画面の変化イベントの通知先
        //=========================================================================================
        public ConsoleScreenEventDispatcher Event {
            get {
                return m_eventDispatcher;
            }
        }

        //=========================================================================================
        // クラス：コンソールの状態
        //=========================================================================================
        public class ConsoleState {
            // キー入力のモード
            public TerminalModeType TerminalMode = TerminalModeType.Normal;

            // 直前の位置と属性を保存した結果
            public StorePosition StorePosition = new StorePosition(1, 1, ConsoleDecoration.NORMAL);

            // スクロール開始行（0のとき解放）
            public int ScrollStartLine = -1;

            // スクロール終了行（-1のとき解放）
            public int ScrollEndLine = -1;

            // チャネルがクローズされた状態のときtrue
            public bool ChannelClosed = false;
        }

        //=========================================================================================
        // クラス：受信文字列の意味解析クラス
        //=========================================================================================
        private class Parser {
            // 変換できない文字を表記するときの文字
            private const char CH_REPLACE_CTRL = '.';

            // 親オブジェクト
            private ConsoleScreen m_parent;

            // 解析対象の文字列
            private string m_receivedString;

            // 現在カーソルがある行の行オブジェクト
            private LogLineTerminal m_currentLogLine;
            
            // 文字を計測するためのグラフィックス（処理中はキャッシュし、最後に破棄、未使用のときnull）
            private Graphics m_graphics = null;

            // 文字幅の計測クラス
            private CharWidth m_width = new CharWidth();

            // 文字列の一時バッファ
            private ParserTempBuffer m_parserTempBuffer;

            // 1回の解析処理で変化した行の情報
            private LogLineChangeLog m_changeLog = new LogLineChangeLog();

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent   親オブジェクト
            // 　　　　[in]strData  解析対象の文字列
            // 戻り値：なし
            //=========================================================================================
            public Parser(ConsoleScreen parent, string strData) {
                m_parent = parent;
                m_receivedString = strData;
            }

            //=========================================================================================
            // 機　能：インスタンスを破棄する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Dispose() {
                if (m_graphics != null) {
                    m_graphics.Dispose();
                    m_graphics = null;
                }
            }

            //=========================================================================================
            // 機　能：解析処理を実行する
            // 引　数：なし
            // 戻り値：解析後に残った文字列
            //=========================================================================================
            public string Parse() {
                m_currentLogLine = GetCurrentLogBuffer(m_parent.m_currentCursorLineId);
                m_parserTempBuffer = new ParserTempBuffer(m_parent.m_locationCursorX, m_parent.m_currentDecoration);
                int index = 0;
                StringBuilder sbBuffer = new StringBuilder();
                while (index < m_receivedString.Length) {
                    char ch = m_receivedString[index];
                    if (ch == CH_ESC) {
                        FlashBuffer();
                        bool resolved = ResolveEscape(ref index);
                        if (!resolved) {
                            break;
                        }
                        InitializeBuffer();
                    } else if (ch == CH_CR) {
                        FlashBuffer();
                        m_parent.m_locationCursorX = 0;
                        index++;
                        InitializeBuffer();
                    } else if (ch == CH_LF) {
                        FlashBuffer();
                        NewLine();
                        index++;
                        InitializeBuffer();
                    } else if (ch == CH_TAB) {
                        AppendTab();
                        index++;
                    } else if (ch == CH_BS) {
                        FlashBuffer();
                        EscapeCursorLeft(1);
                        InitializeBuffer();
                        index++;
                    } else if (ch == CH_BELL) {
                        m_changeLog.RequestBeep = true;
                        index++;
                    } else if (ch < 0x20 || ch == CH_DEL) {
                        index++;
                    } else {
                        AppendChar(ch);
                        index++;
                    }
                }
                FlashBuffer();
                
                // 解析できなかったものは次回へ
                string strRemain;
                if (index < m_receivedString.Length) {
                    strRemain = m_receivedString.Substring(index);
                } else {
                    strRemain = "";
                }
                return strRemain;
            }

            //=========================================================================================
            // 機　能：文字の種類を取得する
            // 引　数：[in]ch   調べる文字
            // 戻り値：文字の種類
            //=========================================================================================
            private CharWidth.CharType GetCharType(char ch) {
                CharWidth.CharType charType = m_width.GetCharType(ch);
                if (charType != CharWidth.CharType.UnInitialize) {
                    return charType;
                }

                // SJIS以外はグラフィックスで描画して確認
                // 対象となるビューがない可能性があるため、メインウィンドウを使う
                m_graphics = Program.MainWindow.CreateGraphics();
                charType = m_width.GetCharType(m_graphics, ch);
                return charType;
            }

            //=========================================================================================
            // 機　能：新しい行に切り替える
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void NewLine() {
                m_parent.m_locationCursorX = 0;
                EscapeCursorDown(1);
            }

            //=========================================================================================
            // 機　能：一時バッファの内容をログ行に転送する
            // 引　数：なし
            // 戻り値：なし
            // メ　モ：この後、InitializeBuffer()が必要
            //=========================================================================================
            private void FlashBuffer() {
                string strBuffer = m_parserTempBuffer.Buffer.ToString();
                if (strBuffer == "") {
                    return;
                }
                m_currentLogLine.SetMessage(strBuffer, m_parserTempBuffer.BufferStartLocationX, m_parserTempBuffer.BufferStartDecoration, false);
                m_changeLog.AddUpdatedItem(m_currentLogLine);
            }

            //=========================================================================================
            // 機　能：新しい一時バッファを作成する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void InitializeBuffer() {
                m_parserTempBuffer = new ParserTempBuffer(m_parent.m_locationCursorX, m_parent.m_currentDecoration);
            }

            //=========================================================================================
            // 機　能：指定された行IDを元に、ログバッファの登録と取得を行い、現在行を返す
            // 引　数：[in]reqLineId   必要な行ID
            // 戻り値：行IDに対する行オブジェクト
            //=========================================================================================
            private LogLineTerminal GetCurrentLogBuffer(long reqLineId) {
                LogBuffer logBuffer = m_parent.m_logBuffer;
                LogLineTerminal logLine = null;
                long existLastId = logBuffer.FirstAvailableLineId + logBuffer.AvailableLogCount - 1;
                if (reqLineId < logBuffer.FirstAvailableLineId) {
                    // すでに消えてしまった行
                    // ダミーを作成（ここに来ないように画面サイズとバッファサイズを制御）
                    logLine = new LogLineTerminal();
                } else if (reqLineId <= existLastId) {
                    // バックログに存在
                    logLine = (LogLineTerminal)(logBuffer.LineIdToLogLine(reqLineId));
                } else {
                    // 先送り
                    for (long i = existLastId; i < reqLineId; i++) {
                        logLine = new LogLineTerminal();
                        logBuffer.AddLogLine(logLine);
                        m_changeLog.AddAddedItem(logLine);
                    }
                    if (reqLineId - m_parent.m_topCursorLineId >= m_parent.m_screenCharSize.Height) {
                        long availableFirst = m_parent.m_logBuffer.FirstAvailableLineId;
                        m_parent.m_topCursorLineId = Math.Max(availableFirst, reqLineId - m_parent.m_screenCharSize.Height + 1);
                    }
                }
                return logLine;
            }

            //=========================================================================================
            // 機　能：文字を一時バッファに追加する
            // 引　数：[in]ch  追加する文字
            // 戻り値：なし
            //=========================================================================================
            private void AppendChar(char ch) {
                CharWidth.CharType charType = GetCharType(ch);
                int chWidth = 1;
                if (charType == CharWidth.CharType.Unknown || charType == CharWidth.CharType.UnInitialize) {
                    ch = CH_REPLACE_CTRL;
                    chWidth = ConsoleScreen.CHAR_WIDTH_UNKNOWN;
                } else if (charType == CharWidth.CharType.FullWidth) {
                    chWidth = 2;
                }
                if (m_parent.m_locationCursorX + chWidth > m_parent.m_screenCharSize.Width) {
                    // この文字で改行になる
                    FlashBuffer();
                    NewLine();
                    InitializeBuffer();
                }
                m_parserTempBuffer.Buffer.Append(ch);
                m_parent.m_locationCursorX += chWidth;
            }

            //=========================================================================================
            // 機　能：タブ文字を一時バッファに追加する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void AppendTab() {
                int space = 4 - (m_parent.m_locationCursorX % 4);
                if (m_parent.m_locationCursorX + space >= m_parent.m_screenCharSize.Width) {
                    for (int i = 0; i < space; i++) {
                        AppendChar(' ');
                    }
                } else {
                    m_parserTempBuffer.Buffer.Append(new string(' ', space));
                    m_parent.m_locationCursorX += space;
                }
            }

            //=========================================================================================
            // 機　能：エスケープシーケンスを解決する
            // 引　数：[in,out]index   バッファ中の対象文字のインデックス（ESC上、新しい位置をセットする）
            // 戻り値：エスケープシーケンスが解決できたときtrue、途中で終わったときfalse
            //=========================================================================================
            private bool ResolveEscape(ref int index) {
                EscapeType escapeType;
                List<int> addInfo;
                EscapeSequenceParser parser = new EscapeSequenceParser(m_receivedString);
                bool resolved = parser.Parse(ref index, out escapeType, out addInfo);
                if (!resolved) {
                    return false;
                }

                // 成功
                switch (escapeType) {
                    case EscapeType.Nop:                    // 何もしない
                        break;
                    case EscapeType.SetColor:               // 色をつける
                        EscapeSetColor(addInfo);
                        break;
                    case EscapeType.MoveCursor:             // カーソルをY行目のX桁目に移動する
                        EscapeMoveCursor(addInfo[1], addInfo[0]);
                        break;
                    case EscapeType.DeleteBelow:            // カーソル行から下へpn行削除する
                        EscapeDeleteBelow(addInfo[0]);
                        break;
                    case EscapeType.DeleteUpper:            // カーソル行から上へpn行削除する
                        EscapeDeleteUpper(addInfo[0]);
                        break;
                    case EscapeType.CursorHome:             // カーソルを左上に移動する
                        EscapeMoveCursor(1, 1);
                        break;
                    case EscapeType.CursorDown:             // カーソルをカラム位置はそのままに１行下に移動する。(カーソルが最下行にある場合は１行スクロールする)
                        EscapeCursorDown(1);
                        break;
                    case EscapeType.CursorUp:               // カーソルをカラム位置はそのままに１行上に移動する。(カーソルが最上行にある場合は機種依存)
                        EscapeCursorUp(1);
                        break;
                    case EscapeType.CursorLeft:             // カーソルを１つ左に移動する
                        EscapeCursorLeft(1);
                        break;
                    case EscapeType.CursorRight:            // カーソルを１つ右に移動する
                        EscapeCursorRight(1);
                        break;
                    case EscapeType. CursorUpWith:          // カーソルをY行上へ移動する
                        EscapeCursorUp(addInfo[0]);
                        break;
                    case EscapeType.CursorDownWith:         // カーソルをY行下へ移動する
                        EscapeCursorDown(addInfo[0]);
                        break;
                    case EscapeType.CursorRightWith:        // カーソルをX桁右へ移動する。行の右端より先には移動しない。
                        EscapeCursorRight(addInfo[0]);
                        break;
                    case EscapeType.CursorLeftWith:         // カーソルをX桁左へ移動する。行の左端より先には移動しない。
                        EscapeCursorLeft(addInfo[0]);
                        break;
                    case EscapeType.CursorNextLeft:         // カーソルを１行下の一番左に移動する。(カーソルが最下行にある場合は１行スクロールする)
                        EscapeCursorNextLeft();
                        break;
                    case EscapeType.RegistPosition:         // 位置記憶
                        EscapeRegistPosition();
                        break;
                    case EscapeType.RestorePosition:        // 位置移動
                        EscapeRestorePosition();
                        break;
                    case EscapeType.InsertLine:             // カーソル行に1行追加する
                        EscapeInsertLine(1);
                        break;
                    case EscapeType.InsertLineWith:         // カーソル行にX行追加する
                        EscapeInsertLine(addInfo[0]);
                        break;
                    case EscapeType.ClearAllNext:           // カーソル位置から最終行の右端まで削除する
                        EscapeClearAllNext();
                        break;
                    case EscapeType.ClearAllPrev:           // 先頭行の左端からカーソル位置まで削除する
                        EscapeClearAllPrev();
                        break;
                    case EscapeType.ClearScreen:            // 画面全体を消去し、カーソルを左上に移動する
                        EscapeClearScreen();
                        break;
                    case EscapeType.ClearLineNext:          // カーソル位置から同一行の右端まで削除する
                        EscapeClearLineNext();
                        break;
                    case EscapeType.ClearLinePrev:          // 同一行の左端からカーソル位置まで削除する
                        EscapeClearLinePrev();
                        break;
                    case EscapeType.ClearLine:              // カーソルのある行全て削除する
                        EscapeClearLine();
                        break;
                    case EscapeType.TerminalModeApplication:// ターミナルモードをApplicationへ
                        EscapeTerminalModeApplication();
                        break;
                    case EscapeType.TerminalModeNormal:     // ターミナルモードをNormalへ
                        EscapeTerminalModeNormal();
                        break;
                    case EscapeType.SetScrollRange:         // スクロール範囲をS行目からE行目までに設定する
                        EscapeSetScrollRange(addInfo[0], addInfo[1]);
                        break;
                }

                return true;
            }

            //=========================================================================================
            // 機　能：色をセットする
            // 引　数：[in]colorSpecify  エスケープシーケンス上での色指定
            // 戻り値：なし
            //=========================================================================================
            private void EscapeSetColor(List<int> colorSpecify) {
                short decoration = m_parent.m_currentDecoration;
                for (int i = 0; i < colorSpecify.Count; i++) {
                    int color = colorSpecify[i];
                    switch (color) {
                        case 0:                 // 戻す
                            decoration = ConsoleDecoration.NORMAL;
                            break;
                        case 1:                 // 強調
                            decoration |= ConsoleDecoration.BOLD;
                            break;
                        case 2:                 // 垂線
                            decoration |= ConsoleDecoration.VERTIVAL;
                            break;
                        case 4:                 // 下線
                            decoration |= ConsoleDecoration.UNDER_LINE;
                            break;
                        case 5:                 // 点滅
                            decoration |= ConsoleDecoration.BLINK;
                            break;
                        case 7:                 // リバース
                            decoration |= ConsoleDecoration.REVERSE;
                            break;
                        case 8:                 // 非表示
                            decoration |= ConsoleDecoration.HIDE;
                            break;
                        case 16:                // 非表示
                            decoration |= ConsoleDecoration.HIDE;
                            break;
                        case 17:                // 赤1
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_RED);
                            break;
                        case 18:                // 青1
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_BLUE);
                            break;
                        case 19:                // 紫1
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_MAGENTA);
                            break;
                        case 20:                // 緑1
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_GREEN);
                            break;
                        case 21:                // 黄1
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_YELLOW);
                            break;
                        case 22:                // 水1
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_CYAN);
                            break;
                        case 23:                // 白1
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_WHITE);
                            break;
                        case 30:                // 黒
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_BLACK);
                            break;
                        case 31:                // 赤2
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_RED);
                            break;
                        case 32:                // 緑2
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_GREEN);
                            break;
                        case 33:                // 黄2
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_YELLOW);
                            break;
                        case 34:                // 青2
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_BLUE);
                            break;
                        case 35:                // 紫2
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_MAGENTA);
                            break;
                        case 36:                // 水2
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_CYAN);
                            break;
                        case 37:                // 白2
                            decoration = (short)((decoration & ConsoleDecoration.COLOR_NOT) | ConsoleDecoration.COLOR_WHITE);
                            break;
                        case 40:                // 黒地
                            decoration = (short)((decoration & ConsoleDecoration.BACK_NOT) | (ConsoleDecoration.BACK_BLACK << ConsoleDecoration.BACK_SHIFT));
                            break;
                        case 41:                // 赤地
                            decoration = (short)((decoration & ConsoleDecoration.BACK_NOT) | (ConsoleDecoration.BACK_RED << ConsoleDecoration.BACK_SHIFT));
                            break;
                        case 42:                // 緑地
                            decoration = (short)((decoration & ConsoleDecoration.BACK_NOT) | (ConsoleDecoration.BACK_GREEN << ConsoleDecoration.BACK_SHIFT));
                            break;
                        case 43:                // 黄地
                            decoration = (short)((decoration & ConsoleDecoration.BACK_NOT) | (ConsoleDecoration.BACK_YELLOW << ConsoleDecoration.BACK_SHIFT));
                            break;
                        case 44:                // 青地
                            decoration = (short)((decoration & ConsoleDecoration.BACK_NOT) | (ConsoleDecoration.BACK_BLUE << ConsoleDecoration.BACK_SHIFT));
                            break;
                        case 45:                // 紫地
                            decoration = (short)((decoration & ConsoleDecoration.BACK_NOT) | (ConsoleDecoration.BACK_MAGENTA << ConsoleDecoration.BACK_SHIFT));
                            break;
                        case 46:                // 水地
                            decoration = (short)((decoration & ConsoleDecoration.BACK_NOT) | (ConsoleDecoration.BACK_CYAN << ConsoleDecoration.BACK_SHIFT));
                            break;
                        case 47:                // 白地
                            decoration = (short)((decoration & ConsoleDecoration.BACK_NOT) | (ConsoleDecoration.BACK_WHITE << ConsoleDecoration.BACK_SHIFT));
                            break;
                    }
                }
                m_parent.m_currentDecoration = decoration;
            }

            //=========================================================================================
            // 機　能：カーソルを指定位置に移動する
            // 引　数：[in]x   X位置（1～）
            // 　　　　[in]y   Y位置（1～）
            // 戻り値：なし
            //=========================================================================================
            private void EscapeMoveCursor(int x, int y) {
                x = Math.Max(0, Math.Min(x - 1, m_parent.m_screenCharSize.Width - 1));
                y = Math.Max(0, Math.Min(y - 1, m_parent.m_screenCharSize.Height - 1));
                m_parent.m_locationCursorX = x;
                m_parent.m_currentCursorLineId = m_parent.m_topCursorLineId + y;
                m_currentLogLine = GetCurrentLogBuffer(m_parent.m_currentCursorLineId);
            }

            //=========================================================================================
            // 機　能：カーソル行から下へ指定行だけ削除する
            // 引　数：[in]line  削除する行数
            // 戻り値：なし
            //=========================================================================================
            private void EscapeDeleteBelow(int line) {
                if (line <= 0) {
                    return;
                }
                line = Math.Min(line, m_parent.m_screenCharSize.Height);
                long startLineId = m_parent.m_currentCursorLineId;
                long lastLineId = m_parent.m_logBuffer.NextLogId;
                DeleteLineHelper(startLineId, lastLineId, line);
            }

            //=========================================================================================
            // 機　能：カーソル行から上へ指定行だけ削除する
            // 引　数：[in]line  削除する行数
            // 戻り値：なし
            //=========================================================================================
            private void EscapeDeleteUpper(int line) {
                if (line <= 0) {
                    return;
                }
                line = Math.Min(line, m_parent.m_screenCharSize.Height);
                LogBuffer logBuffer = m_parent.m_logBuffer;
                long startLineId = m_parent.m_currentCursorLineId - line;
                long firstAvailableLine = logBuffer.FirstAvailableLineId;
                if (startLineId < firstAvailableLine) {
                    line -= (int)(firstAvailableLine - startLineId);
                    startLineId = firstAvailableLine;
                }
                long lastLineId = m_parent.m_logBuffer.NextLogId;
                DeleteLineHelper(startLineId, lastLineId, line);
                m_parent.m_currentCursorLineId -= line;
                m_currentLogLine = GetCurrentLogBuffer(m_parent.m_currentCursorLineId);
            }

            //=========================================================================================
            // 機　能：カーソルをカラム位置はそのままに下に移動する
            // 引　数：[in]line  移動する行数
            // 戻り値：なし
            // メ　モ：カーソルが最下行にある場合は１行スクロールする
            //=========================================================================================
            private void EscapeCursorDown(int line) {
                line = Math.Min(line, m_parent.m_screenCharSize.Height);
                if (m_parent.m_consoleState.ScrollStartLine == -1 && m_parent.m_consoleState.ScrollEndLine == -1) {
                    // 通常通りのスクロール
                    m_parent.m_currentCursorLineId += line;
                    m_currentLogLine = GetCurrentLogBuffer(m_parent.m_currentCursorLineId);
                } else {
                    // スクロールをシミュレーション
                    int startY;
                    if (m_parent.m_consoleState.ScrollStartLine == -1) {
                        startY = 0;
                    } else {
                        startY = m_parent.m_consoleState.ScrollStartLine;
                    }
                    int endY;
                    if (m_parent.m_consoleState.ScrollEndLine == -1) {
                        endY = m_parent.m_screenCharSize.Height - 1;
                    } else {
                        endY = Math.Min(m_parent.m_consoleState.ScrollEndLine, m_parent.m_screenCharSize.Height - 1);
                    }
                    int cursorY = (int)(m_parent.m_currentCursorLineId - m_parent.m_topCursorLineId);
                    if (cursorY > endY) {
                        // すでにスクロール範囲から出ているときはスクロールだけ実行
                        ScrollLineHelper(startY, endY, -line);
                    } else if (cursorY + line > endY) {
                        // カーソルをendYに移動して残りをスクロール
                        m_parent.m_currentCursorLineId = m_parent.m_topCursorLineId + endY;
                        ScrollLineHelper(startY, endY, -(line - (endY - cursorY)));
                    } else {
                        // スクロールなしでカーソルのみを移動
                        m_parent.m_currentCursorLineId += line;
                    }
                    m_currentLogLine = GetCurrentLogBuffer(m_parent.m_currentCursorLineId);
                }       
            }

            //=========================================================================================
            // 機　能：カーソルをカラム位置はそのままに上に移動する
            // 引　数：[in]line  移動する行数
            // 戻り値：なし
            // メ　モ：カーソルが最上行にある場合は機種依存（そのままとどまる）
            //=========================================================================================
            private void EscapeCursorUp(int line) {
                m_parent.m_currentCursorLineId = Math.Max(m_parent.m_currentCursorLineId - line, m_parent.m_topCursorLineId);
                m_currentLogLine = GetCurrentLogBuffer(m_parent.m_currentCursorLineId);

                int startY;
                if (m_parent.m_consoleState.ScrollStartLine == -1) {
                    startY = 0;
                } else {
                    startY = m_parent.m_consoleState.ScrollStartLine;
                }
                int endY;
                if (m_parent.m_consoleState.ScrollEndLine == -1) {
                    endY = m_parent.m_screenCharSize.Height - 1;
                } else {
                    endY = Math.Min(m_parent.m_consoleState.ScrollEndLine, m_parent.m_screenCharSize.Height - 1);
                }
                int cursorY = (int)(m_parent.m_currentCursorLineId - m_parent.m_topCursorLineId);
                if (cursorY < startY) {
                    // すでにスクロール範囲から出ているときはスクロールだけ実行
                    ScrollLineHelper(startY, endY, line);
                } else if (cursorY - line < startY) {
                    // カーソルをstartYに移動して残りをスクロール
                    m_parent.m_currentCursorLineId = m_parent.m_topCursorLineId + startY;
                    ScrollLineHelper(startY, endY, (line - (startY - cursorY)));
                } else {
                    // スクロールなしでカーソルのみを移動
                }
                m_currentLogLine = GetCurrentLogBuffer(m_parent.m_currentCursorLineId);
            }

            //=========================================================================================
            // 機　能：カーソルを左に移動する
            // 引　数：[in]column  移動する桁数
            // 戻り値：なし
            //=========================================================================================
            private void EscapeCursorLeft(int column) {
                m_parent.m_locationCursorX = Math.Max(0, m_parent.m_locationCursorX - column);
            }

            //=========================================================================================
            // 機　能：カーソルを右に移動する
            // 引　数：[in]column  移動する桁数
            // 戻り値：なし
            //=========================================================================================
            private void EscapeCursorRight(int column) {
                m_parent.m_locationCursorX = Math.Min(m_parent.m_screenCharSize.Width - 1, m_parent.m_locationCursorX + column);
            }

            //=========================================================================================
            // 機　能：カーソルを１行下の一番左に移動する
            // 引　数：なし
            // 戻り値：なし
            // メ　モ：カーソルが最下行にある場合は１行スクロールする
            //=========================================================================================
            private void EscapeCursorNextLeft() {
                m_parent.m_currentCursorLineId++;
                m_currentLogLine = GetCurrentLogBuffer(m_parent.m_currentCursorLineId);
                m_parent.m_locationCursorX = 0;
            }

            //=========================================================================================
            // 機　能：位置を記憶する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EscapeRegistPosition() {
                int xPos = m_parent.m_locationCursorX;
                int yPos = (int)(m_parent.m_currentCursorLineId - m_parent.m_topCursorLineId);
                short decoration = m_parent.m_currentDecoration;
                m_parent.m_consoleState.StorePosition = new StorePosition(xPos, yPos, decoration);
            }

            //=========================================================================================
            // 機　能：記憶した位置に戻る
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EscapeRestorePosition() {
                StorePosition store = m_parent.m_consoleState.StorePosition;
                m_parent.m_locationCursorX = store.LocationX;
                m_parent.m_currentCursorLineId = m_parent.m_currentCursorLineId + store.LocationY;
                m_parent.m_currentDecoration = store.Decoration;
            }

            //=========================================================================================
            // 機　能：カーソル行に行を追加する
            // 引　数：[in]line  追加する行数
            // 戻り値：なし
            //=========================================================================================
            private void EscapeInsertLine(int line) {
                if (line <= 0) {
                    return;
                }
                line = Math.Min(line, m_parent.m_screenCharSize.Height);
                InsertLineHelper(m_parent.m_currentCursorLineId, line);
                m_currentLogLine = GetCurrentLogBuffer(m_parent.m_currentCursorLineId);
            }

            //=========================================================================================
            // 機　能：カーソル位置から最終行の右端まで削除する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EscapeClearAllNext() {
                // 現在行を削除
                m_currentLogLine.SetMessage("", m_parent.m_locationCursorX, ConsoleDecoration.NORMAL, true);
                m_changeLog.AddUpdatedItem(m_currentLogLine);
                
                // 最終行まで削除
                long nextLineId = m_parent.m_currentCursorLineId + 1;
                long lastLineId = m_parent.m_logBuffer.NextLogId;
                ClearLineHelper(nextLineId, lastLineId);
            }

            //=========================================================================================
            // 機　能：先頭行の左端からカーソル位置まで削除する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EscapeClearAllPrev() {
                // 現在行を削除
                if (m_parent.m_locationCursorX > 0) {
                    string space = StringUtils.Repeat(" ", m_parent.m_locationCursorX);
                    m_currentLogLine.SetMessage(space, 0, ConsoleDecoration.NORMAL, false);
                    m_changeLog.AddUpdatedItem(m_currentLogLine);
                }

                // 先頭行から削除
                long firstLineId = m_parent.m_topCursorLineId;
                long currentLineId = m_parent.m_currentCursorLineId;
                ClearLineHelper(firstLineId, currentLineId);
            }

            //=========================================================================================
            // 機　能：画面全体を消去し、カーソルを左上に移動する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EscapeClearScreen() {
                // 内容を消去
                long firstLineId = m_parent.m_topCursorLineId;
                long lastLineId = m_parent.m_logBuffer.NextLogId;
                ClearLineHelper(firstLineId, lastLineId);

                // カーソルを移動
                m_parent.m_currentCursorLineId = firstLineId;
                m_parent.m_locationCursorX = 0;
                m_currentLogLine = GetCurrentLogBuffer(m_parent.m_currentCursorLineId);
            }

            //=========================================================================================
            // 機　能：カーソル位置から同一行の右端まで削除する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EscapeClearLineNext() {
                m_currentLogLine.SetMessage("", m_parent.m_locationCursorX, ConsoleDecoration.NORMAL, true);
                m_changeLog.AddUpdatedItem(m_currentLogLine);
            }

            //=========================================================================================
            // 機　能：同一行の左端からカーソル位置まで削除する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EscapeClearLinePrev() {
                string space = StringUtils.Repeat(" ", m_parent.m_locationCursorX);
                m_currentLogLine.SetMessage(space, 0, ConsoleDecoration.NORMAL, false);
                m_changeLog.AddUpdatedItem(m_currentLogLine);
            }

            //=========================================================================================
            // 機　能：カーソルのある行全てを削除する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EscapeClearLine() {
                m_currentLogLine.SetMessage("", 0, ConsoleDecoration.NORMAL, true);
                m_changeLog.AddUpdatedItem(m_currentLogLine);
            }

            //=========================================================================================
            // 機　能：スクロール範囲をstart行目からend行目までに設定する
            // 引　数：[in]start  スクロール開始行
            // 　　　　[in]end    スクロール終了行
            // 戻り値：なし
            //=========================================================================================
            private void EscapeSetScrollRange(int start, int end) {
                if (start > end) {
                    int temp = start;
                    start = end;
                    end = temp;
                }
                if (start == 1) {
                    m_parent.m_consoleState.ScrollStartLine = -1;
                } else {
                    m_parent.m_consoleState.ScrollStartLine = start - 1;
                }
                if (end >= m_parent.m_screenCharSize.Height) {
                    m_parent.m_consoleState.ScrollEndLine = -1;
                } else {
                    m_parent.m_consoleState.ScrollEndLine = end - 1;
                }
            }

            //=========================================================================================
            // 機　能：ターミナルモードをApplicationへ設定する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EscapeTerminalModeApplication() {
                m_parent.m_consoleState.TerminalMode = TerminalModeType.Application;
            }

            //=========================================================================================
            // 機　能：ターミナルモードをNormalへ設定する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EscapeTerminalModeNormal() {
                m_parent.m_consoleState.TerminalMode = TerminalModeType.Normal;
            }

            //=========================================================================================
            // 機　能：指定された範囲の行を削除する
            // 引　数：[in]startLineId   開始行の行ID
            // 　　　　[in]lastLineId    終了行の行ID（次の行を指定）
            // 　　　　[in]line          削除する行数
            // 戻り値：なし
            //=========================================================================================
            private void DeleteLineHelper(long startLineId, long lastLineId, int line) {
                LogBuffer logBuffer = m_parent.m_logBuffer;
                for (long i = startLineId; i < lastLineId - line; i++) {
                    LogLineTerminal lineTo = (LogLineTerminal)(logBuffer.LineIdToLogLine(i));
                    LogLineTerminal lineFrom = (LogLineTerminal)(logBuffer.LineIdToLogLine(i + line));
                    if (lineTo == null || lineFrom == null) {
                        Program.Abort("ログのバックバッファの状態が異常です。from:{0}, to:{1}, start:{2}, last:{3}", (lineTo == null), (lineFrom == null), startLineId, lastLineId);
                    }
                    lineTo.CopyFrom(lineFrom);
                    m_changeLog.AddUpdatedItem(lineFrom);
                }
                for (long i = Math.Max(startLineId, lastLineId - line - 1); i < lastLineId; i++) {
                    LogLineTerminal lineClear = (LogLineTerminal)(logBuffer.LineIdToLogLine(i));
                    if (lineClear == null) {
                        Program.Abort("ログのバックバッファの状態が異常です。clear:{0}, start:{1}, last:{2}", i, startLineId, lastLineId);
                    }
                    lineClear.ClearLogLine();
                    m_changeLog.AddUpdatedItem(lineClear);
                }
                m_changeLog.RefreshAll = true;
            }

            //=========================================================================================
            // 機　能：指定された位置に行を追加する
            // 引　数：[in]startLineId   開始行の行ID
            // 　　　　[in]line          追加する行数
            // 戻り値：なし
            //=========================================================================================
            private void InsertLineHelper(long startLineId, int line) {
                LogBuffer logBuffer = m_parent.m_logBuffer;
                long lastLineId = logBuffer.NextLogId;
                for (long i = lastLineId - 1 - line; i >= startLineId; i--) {
                    LogLineTerminal lineTo = (LogLineTerminal)(logBuffer.LineIdToLogLine(i + line));
                    LogLineTerminal lineFrom = (LogLineTerminal)(logBuffer.LineIdToLogLine(i));
                    if (lineTo == null || lineFrom == null) {
                        Program.Abort("ログのバックバッファの状態が異常です。from:{0}, to:{1}, start:{2}, last:{3}", (lineTo == null), (lineFrom == null), startLineId, lastLineId);
                    }
                    lineTo.CopyFrom(lineFrom);
                    m_changeLog.AddUpdatedItem(lineFrom);
                }
                for (long i = startLineId; i < startLineId + line; i++) {
                    LogLineTerminal lineClear = (LogLineTerminal)(logBuffer.LineIdToLogLine(i));
                    if (lineClear == null) {
                        Program.Abort("ログのバックバッファの状態が異常です。clear:{0}, start:{1}, last:{2}", i, startLineId, lastLineId);
                    }
                    lineClear.ClearLogLine();
                    m_changeLog.AddUpdatedItem(lineClear);
                }
                m_changeLog.RefreshAll = true;
            }

            //=========================================================================================
            // 機　能：指定された範囲の行を定義はそのままに、クリア状態にする
            // 引　数：[in]startId  開始行の行ID
            // 　　　　[in]endId    終了行の行ID（次の行を指定）
            // 戻り値：なし
            //=========================================================================================
            private void ClearLineHelper(long startId, long endId) {
                LogBuffer logBuffer = m_parent.m_logBuffer;
                for (long i = startId; i < endId; i++) {
                    LogLineTerminal lineClear = (LogLineTerminal)(logBuffer.LineIdToLogLine(i));
                    if (lineClear == null) {
                        Program.Abort("ログのバックバッファの状態が異常です。clear:{0}, start:{1}, end:{2}", i, startId, endId);
                    }
                    lineClear.ClearLogLine();
                    m_changeLog.AddUpdatedItem(lineClear);
                }
            }

            //=========================================================================================
            // 機　能：指定された範囲の行をスクロールさせる
            // 引　数：[in]start  開始行の画面上の位置（0～）
            // 　　　　[in]end    終了行の画面上の位置（0～）
            // 　　　　[in]line   スクロール行数
            // 戻り値：なし
            //=========================================================================================
            private void ScrollLineHelper(int start, int end, int line) {
                LogBuffer logBuffer = m_parent.m_logBuffer;
                long lastLineId = logBuffer.NextLogId;
                long startLineId = Math.Min(lastLineId - 1, m_parent.m_topCursorLineId + start);
                long endLineId = Math.Min(lastLineId - 1, m_parent.m_topCursorLineId + end);
                if (line == 0) {
                    return;
                } else if (line > 0) {
                    // 下に移動
                    for (long i = endLineId - line; i >= startLineId; i--) {
                        LogLineTerminal lineTo = (LogLineTerminal)(logBuffer.LineIdToLogLine(i + line));
                        LogLineTerminal lineFrom = (LogLineTerminal)(logBuffer.LineIdToLogLine(i));
                        if (lineTo == null || lineFrom == null) {
                            Program.Abort("ログのバックバッファの状態が異常です。from:{0}, to:{1}, start:{2}, last:{3}", (lineTo == null), (lineFrom == null), startLineId, lastLineId);
                        }
                        lineTo.CopyFrom(lineFrom);
                        m_changeLog.AddUpdatedItem(lineTo);
                    }
                    for (long i = startLineId; i < startLineId + line; i++) {
                        LogLineTerminal lineClear = (LogLineTerminal)(logBuffer.LineIdToLogLine(i));
                        lineClear.SetMessage("", 0, ConsoleDecoration.NORMAL, true);
                        m_changeLog.AddUpdatedItem(lineClear);
                    }
                } else {
                    // 上に移動
                    line = -line;
                    for (long i = startLineId; i <= endLineId - line; i++) {
                        LogLineTerminal lineTo = (LogLineTerminal)(logBuffer.LineIdToLogLine(i));
                        LogLineTerminal lineFrom = (LogLineTerminal)(logBuffer.LineIdToLogLine(i + line));
                        if (lineTo == null || lineFrom == null) {
                            Program.Abort("ログのバックバッファの状態が異常です。from:{0}, to:{1}, start:{2}, last:{3}", (lineTo == null), (lineFrom == null), startLineId, lastLineId);
                        }
                        lineTo.CopyFrom(lineFrom);
                        m_changeLog.AddUpdatedItem(lineTo);
                    }
                    for (long i = endLineId - line + 1; i <= endLineId; i++) {
                        LogLineTerminal lineClear = (LogLineTerminal)(logBuffer.LineIdToLogLine(i));
                        lineClear.SetMessage("", 0, ConsoleDecoration.NORMAL, true);
                        m_changeLog.AddUpdatedItem(lineClear);
                    }
                }
            }

            //=========================================================================================
            // プロパティ：1回の解析処理で変化した行の情報
            //=========================================================================================
            public LogLineChangeLog ChangeLog {
                get {
                    return m_changeLog;
                }
            }
        }

        //=========================================================================================
        // プロパティ：行情報の記憶/復元を実行するための行情報
        //=========================================================================================
        public class StorePosition {
            // カーソルのX座標（1～）
            public int LocationX;

            // カーソルのY座標（1～）
            public int LocationY;

            // 装飾
            public short Decoration;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]locationX   カーソルのX座標（1～）
            // 　　　　[in]locationY   カーソルのY座標（1～）
            // 　　　　[in]decoration  装飾
            // 戻り値：なし
            //=========================================================================================
            public StorePosition(int locationX, int locationY, short decoration) {
                this.LocationX = locationX;
                this.LocationY = locationY;
                this.Decoration = decoration;
            }
        }

        //=========================================================================================
        // プロパティ：現在解析中の行の一時情報
        //=========================================================================================
        private struct ParserTempBuffer {
            // 行の文字列
            public StringBuilder Buffer;

            // バッファの開始位置の半角単位X座標
            public int BufferStartLocationX;
            
            // この文字列全体に適用する装飾
            public short BufferStartDecoration;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]startLocation    バッファの開始位置の半角単位X座標
            // 　　　　[in]startDecoration  この文字列全体に適用する装飾
            // 戻り値：なし
            //=========================================================================================
            public ParserTempBuffer(int startLocation, short startDecoration) {
                Buffer = new StringBuilder();
                BufferStartLocationX = startLocation;
                BufferStartDecoration = startDecoration;
            }
        }
    }
}
