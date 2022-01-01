using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Locale;
using ShellFiler.Util;
using ShellFiler.UI.Log;
using ShellFiler.Terminal.TerminalSession;

namespace ShellFiler.Terminal.UI {

    //=========================================================================================
    // クラス：コンソールでの1行の内容
    //=========================================================================================
    public class LogLineTerminal : LogLine {
        // メッセージの内容（未登録状態のときnull：選択状態にもならない）
        private string m_message;

        // メッセージに対応する色（ConsoleDecorationの値、m_message.Lengthと同じ長さ、未登録状態のときnull）
        private short[] m_decoration;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public LogLineTerminal() {
            m_message = "";
            m_decoration = new short[0];
        }

        //=========================================================================================
        // 機　能：ログの行内容を描画する
        // 引　数：[in]g           描画に使用するグラフィックス
        // 　　　　[in]scrLine     画面上の描画行
        // 　　　　[in]logPanel    ログパネル
        // 　　　　[in]lineContext 各行を描画するときの情報
        // 戻り値：なし
        //=========================================================================================
        public override void DrawLogLine(LogGraphics g, int scrLine, ILogViewContainer logPanel, DrawingLogLineContext lineContext) {
            TerminalLineRenderer renderer = new TerminalLineRenderer(m_message, m_decoration);
            renderer.DrawLogLine(g, scrLine, logPanel, lineContext);
        }
        
        //=========================================================================================
        // 機　能：ターミナルのメッセージを設定する
        // 引　数：[in]buffer      出力する内容
        // 　　　　[in]locationX   開始X座標（半角文字単位）
        // 　　　　[in]decoration  出力する内容に設定する装飾情報
        // 　　　　[in]eraseCurr   現在の行内容を削除するときtrue、必要部分だけ上書きするときfalse
        // 戻り値：なし
        //=========================================================================================
        public void SetMessage(string buffer, int locationX, short decoration, bool eraseCurr) {
            if (m_message == null) {
                m_message = "";
                m_decoration = new short[0];
            }
            int bufferWidth = CharWidthUtils.StringHalfCharLength(null, buffer, ConsoleScreen.CHAR_WIDTH_UNKNOWN);
            int currentWidth = CharWidthUtils.StringHalfCharLength(null, m_message, ConsoleScreen.CHAR_WIDTH_UNKNOWN);
            if (currentWidth <= locationX) {
                // 今までのメッセージの後ろに連結
                string newMessage = m_message + new string(' ', locationX - currentWidth) + buffer;
                short[] newDecoration = new short[locationX + bufferWidth];
                Array.Copy(m_decoration, 0, newDecoration, 0, currentWidth);
                for (int i = currentWidth; i < locationX; i++) {
                    newDecoration[i] = 0;
                }
                for (int i = 0; i < bufferWidth; i++) {
                    newDecoration[locationX + i] = decoration;
                }
                m_message = newMessage;
                m_decoration = newDecoration;
            } else if (eraseCurr || locationX + bufferWidth >= currentWidth) {
                // 今までのメッセージすべてを上書き
                string prev = CharWidthUtils.SubstringHalfChar(null, m_message, ConsoleScreen.CHAR_WIDTH_UNKNOWN, 0, locationX, ' ', ' ');
                string newMessage = prev + buffer;
                short[] newDecoration = new short[locationX + bufferWidth];
                Array.Copy(m_decoration, 0, newDecoration, 0, locationX);
                for (int i = 0; i < bufferWidth; i++) {
                    newDecoration[locationX + i] = decoration;
                }
                m_message = newMessage;
                m_decoration = newDecoration;
            } else {
                // 今までのメッセージの途中に上書き
                string prev1 = CharWidthUtils.SubstringHalfChar(null, m_message, ConsoleScreen.CHAR_WIDTH_UNKNOWN, 0, locationX, ' ', ' ');
                string prev2 = CharWidthUtils.SubstringHalfChar(null, m_message, ConsoleScreen.CHAR_WIDTH_UNKNOWN, locationX + bufferWidth, currentWidth - (locationX + bufferWidth), ' ', ' ');
                string newMessage = prev1 + buffer + prev2;
                short[] newDecoration = ArrayUtils.CloneArray<short>(m_decoration);
                for (int i = 0; i < bufferWidth; i++) {
                    newDecoration[locationX + i] = decoration;
                }
                m_message = newMessage;
                m_decoration = newDecoration;
            }
#if DEBUG
            int lenMsg = CharWidthUtils.StringHalfCharLength(null, m_message, ConsoleScreen.CHAR_WIDTH_UNKNOWN);
            int lenDeco = m_decoration.Length;
            if (lenMsg != lenDeco) {
                Program.Abort("文字列長と装飾の関係が不正です。");
            }
#endif
        }

        //=========================================================================================
        // 機　能：コピーする
        // 引　数：[in]src   転送元
        // 戻り値：なし
        //=========================================================================================
        public void CopyFrom(LogLineTerminal src) {
            if (src.m_decoration == null) {
                m_decoration = null;
                m_message = null;
            } else {
                m_decoration = ArrayUtils.CloneArray<short>(src.m_decoration);
                m_message = src.m_message;
            }
        }

        //=========================================================================================
        // 機　能：バッファをクリアする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearLogLine() {
            m_decoration = null;
            m_message = null;
        }

        //=========================================================================================
        // 機　能：クリック位置の桁情報を返す
        // 引　数：[in]logPanel   ログパネル
        // 　　　　[in]g          桁位置計測用のグラフィックス
        // 　　　　[in]cursorPos  マウスカーソルの座標
        // 　　　　[out]column    カラム位置を返す変数
        // 　　　　[out]onChar    文字の上の位置にいるときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        public override void GetMouseHitColumn(ILogViewContainer logPanel, LogGraphics g, Point cursorPos, out int column, out bool onChar) {
            float xPos = cursorPos.X;
            if (m_message == null || m_message.Length == 0 || xPos < 0) {
                column = 0;
                onChar = false;
                return;
            }
            string message = m_message;
            if (message.IndexOf((char)0xfffd) != -1) {       // 不明な文字は誤動作するため「.」に置換
                message = message.Replace((char)0xfffd, '.');
            }

            int strIndexStart = 0;
            int strIndexEnd = message.Length;
            while (true) {
                // 分割方法を決定
                // 文字列中の調査領域を最大10分割
                const int DIV_STRING = 10;               // 1回での領域調査の分割数
                CharacterRange[] rangeList = new CharacterRange[Math.Min(DIV_STRING, strIndexEnd - strIndexStart)];
                int rangeStep = (strIndexEnd - strIndexStart) / rangeList.Length;
                for (int i = 0; i < rangeList.Length; i++) {
                    rangeList[i] = new CharacterRange(strIndexStart + i * rangeStep, rangeStep);
                }
                rangeList[rangeList.Length - 1] = new CharacterRange(rangeList[rangeList.Length - 1].First, strIndexEnd - rangeList[rangeList.Length - 1].First);

                // 領域を計測
                StringFormat sf = new StringFormat();
                sf.SetMeasurableCharacterRanges(rangeList);
                Region[] charRegion = g.Graphics.MeasureCharacterRanges(message + ".", g.LogWindowFixedFont, new RectangleF(0, 0, GraphicsUtils.INFINITY_WIDTH, GraphicsUtils.INFINITY_WIDTH), sf);
                sf.Dispose();

                // 位置を比較
                if (xPos < charRegion[0].GetBounds(g.Graphics).Left) {
                    int index = rangeList[0].First;
                    column = CharWidthUtils.StringHalfCharLength(g.Graphics, message.Substring(0, index), ConsoleScreen.CHAR_WIDTH_UNKNOWN);
                    onChar = false;
                    return;
                }
                bool found = false;
                for (int i = 0; i < charRegion.Length; i++) {
                    if (xPos < charRegion[i].GetBounds(g.Graphics).Right) {
                        if (rangeList[i].Length == 1) {
                            int index = rangeList[i].First;
                            column = CharWidthUtils.StringHalfCharLength(g.Graphics, message.Substring(0, index), ConsoleScreen.CHAR_WIDTH_UNKNOWN);
                            onChar = true;
                            return;
                        } else {
                            found = true;
                            strIndexStart = rangeList[i].First;
                            strIndexEnd = strIndexStart + rangeList[i].Length;
                            break;
                        }
                    }
                }
                if (!found) {
                    if (strIndexEnd == message.Length && xPos >= charRegion[charRegion.Length - 1].GetBounds(g.Graphics).Right) {
                        column = m_decoration.Length;
                        onChar = false;
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：選択中の行の内容を文字列で返す
        // 引　数：[in]startColumn   選択開始カラム（全選択のとき0）
        // 　　　　[in]endColumn     選択終了カラム（全選択のときint.MaxValue）
        // 戻り値：選択されている文字列（削除済みの行のときnull）
        //=========================================================================================
        public override string GetSelectedLine(int startColumn, int endColumn) {
            if (m_message == null) {
                return null;
            } else if (startColumn == 0 && endColumn >= m_decoration.Length) {
                return m_message;
            } else {
                endColumn = Math.Min(endColumn, m_decoration.Length);
                string selectedMessage = CharWidthUtils.SubstringHalfChar(null, m_message, ConsoleScreen.CHAR_WIDTH_UNKNOWN, startColumn, endColumn - startColumn, ' ', ' ');
                return selectedMessage;
            }
        }

        //=========================================================================================
        // 機　能：文字列化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override string ToString() {
            return m_message;
        }

        //=========================================================================================
        // プロパティ：メッセージ（未登録状態のときnull：選択状態にもならない）
        //=========================================================================================
        public string Message {
            get {
                return m_message;
            }
            set {
                m_message = value;
            }
        }

        //=========================================================================================
        // プロパティ：全選択されたときの最大カラム位置
        //=========================================================================================
        public override int SelectAllMaxColumn {
            get {
                if (m_decoration == null) {
                    return 0;
                } else {
                    return m_decoration.Length;
                }
            }
        }
    }
}
