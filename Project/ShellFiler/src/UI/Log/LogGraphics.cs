using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Terminal.TerminalSession;

namespace ShellFiler.UI.Log {

    //=========================================================================================
    // クラス：ログウィンドウの描画用グラフィックス
    //=========================================================================================
    public class LogGraphics {
        // グラフィック（null:未初期化）
        private Graphics m_graphics;

        // 描画対象のコントロール（null:グラフィック指定）
        private Control m_control;
        
        // 背景色の表示モード
        private BackColorMode m_backColorMode;

        // フォントの大きさ
        private float m_fontSize;

        // ログウィンドウ テキスト描画用のブラシ
        private Brush m_logWindowTextBrush = null;

        // ログウィンドウ リンクテキスト描画用のブラシ
        private Brush m_logWindowLinkTextBrush = null;

        // ログウィンドウ エラー描画用のブラシ
        private Brush m_logLogErrorTextBrush = null;

        // ログウィンドウ 標準エラー描画用のブラシ
        private Brush m_logLogStdErrorTextBrush = null;

        // ログウィンドウ 選択中のテキスト描画用のブラシ
        private Brush m_logWindowSelectTextBrush = null;

        // ログウィンドウ 背景描画用のブラシ
        private Brush m_logWindowBackBrush = null;

        // ログウィンドウ 選択中の背景描画用のブラシ
        private Brush m_logWindowSelectBackBrush = null;

        // ログウィンドウ 残り時間描画用のフォントブラシ（明）
        private Brush m_logWindowRemainingTimeTextBrush1 = null;

        // ログウィンドウ 残り時間描画用のフォントブラシ（明）
        private Brush m_logWindowRemainingTimeTextBrush2 = null;

        // ログウィンドウ 破線描画用のペン
        private Pen m_logWindowLeaderPen = null;

        // ログウィンドウ 選択中の破線描画用のペン
        private Pen m_logWindowLeaderSelectPen = null;

        // ログウィンドウ 進捗表示領域のペン
        private Pen m_logWindowProgressPen = null;

        // ログウィンドウ 描画用のフォント
        private Font m_logWindowFont = null;

        // ログウィンドウ 等幅描画用のフォント
        private Font m_logWindowFixedFont = null;

        // ログウィンドウ 等幅描画用アンダーライン付きのフォント
        private Font m_logWindowFixedUnderlineFont = null;
        
        // ログウィンドウ リンク描画用のフォント
        private Font m_logWindowLinkFont = null;

        // ログウィンドウ 描画用のボールドフォント
        private Font m_logWindowBoldFont = null;

        // ログウィンドウ 残り時間描画用のフォント
        private Font m_logWindowRemainingTimeFont = null;

        // 描画時のフォーマッタ（省略記号付き）
        private StringFormat m_stringFormatEllipsis = null;

        // 描画時のフォーマッタ（通常）
        private StringFormat m_stringFormatNormal = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics    グラフィックス
        // 　　　　[in]logMode     表示モード
        // 　　　　[in]backColor   背景色の表示モード
        // 戻り値：なし
        //=========================================================================================
        public LogGraphics(Graphics graphics, LogViewImpl.LogMode logMode, BackColorMode backColor) {
            m_control = null;
            m_graphics = graphics;
            m_backColorMode = backColor;
            if (logMode == LogViewImpl.LogMode.LogWindow) {
                m_fontSize = Configuration.Current.LogWindowFontSize;
            } else {
                m_fontSize = Configuration.Current.LogWindowTerminalFontSize;
            }
        }

        //=========================================================================================
        // 機　能：コンストラクタ（コントロール指定で必要時に初期化）
        // 引　数：[in]control    描画対象のコントロール
        // 戻り値：なし
        //=========================================================================================
        public LogGraphics(Control control, LogViewImpl.LogMode logMode) {
            m_control = control;
            m_graphics = null;
            if (logMode == LogViewImpl.LogMode.LogWindow) {
                m_fontSize = Configuration.Current.LogWindowFontSize;
            } else {
                m_fontSize = Configuration.Current.LogWindowTerminalFontSize;
            }
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_control != null && m_graphics != null) {
                m_graphics.Dispose();
            }
            if (m_logWindowTextBrush != null) {
                m_logWindowTextBrush.Dispose();
                m_logWindowTextBrush = null;
            }
            if (m_logWindowLinkTextBrush != null) {
                m_logWindowLinkTextBrush.Dispose();
                m_logWindowLinkTextBrush = null;
            }
            if (m_logLogErrorTextBrush != null) {
                m_logLogErrorTextBrush.Dispose();
                m_logLogErrorTextBrush = null;
            }
            if (m_logLogStdErrorTextBrush != null) {
                m_logLogStdErrorTextBrush.Dispose();
                m_logLogStdErrorTextBrush = null;
            }
            if (m_logWindowSelectTextBrush != null) {
                m_logWindowSelectTextBrush.Dispose();
                m_logWindowSelectTextBrush = null;
            }
            if (m_logWindowBackBrush != null) {
                m_logWindowBackBrush.Dispose();
                m_logWindowBackBrush = null;
            }
            if (m_logWindowSelectBackBrush != null) {
                m_logWindowSelectBackBrush.Dispose();
                m_logWindowSelectBackBrush = null;
            }
            if (m_logWindowRemainingTimeTextBrush1 != null) {
                m_logWindowRemainingTimeTextBrush1.Dispose();
                m_logWindowRemainingTimeTextBrush1 = null;
            }
            if (m_logWindowRemainingTimeTextBrush2 != null) {
                m_logWindowRemainingTimeTextBrush2.Dispose();
                m_logWindowRemainingTimeTextBrush2 = null;
            }
            if (m_logWindowLeaderPen != null) {
                m_logWindowLeaderPen.Dispose();
                m_logWindowLeaderPen = null;
            }
            if (m_logWindowLeaderSelectPen != null) {
                m_logWindowLeaderSelectPen.Dispose();
                m_logWindowLeaderSelectPen = null;
            }
            if (m_logWindowProgressPen != null) {
                m_logWindowProgressPen.Dispose();
                m_logWindowProgressPen = null;
            }
            if (m_logWindowFont != null) {
                m_logWindowFont.Dispose();
                m_logWindowFont = null;
            }
            if (m_logWindowFixedFont != null) {
                m_logWindowFixedFont.Dispose();
                m_logWindowFixedFont = null;
            }
            if (m_logWindowFixedUnderlineFont != null) {
                m_logWindowFixedUnderlineFont.Dispose();
                m_logWindowFixedUnderlineFont = null;
            }
            if (m_logWindowBoldFont != null) {
                m_logWindowBoldFont.Dispose();
                m_logWindowBoldFont = null;
            }
            if (m_logWindowLinkFont != null) {
                m_logWindowLinkFont.Dispose();
                m_logWindowLinkFont = null;
            }
            if (m_logWindowRemainingTimeFont != null) {
                m_logWindowRemainingTimeFont.Dispose();
                m_logWindowRemainingTimeFont = null;
            }
            if (m_stringFormatEllipsis != null) {
                m_stringFormatEllipsis.Dispose();
                m_stringFormatEllipsis = null;
            }
            if (m_stringFormatNormal != null) {
                m_stringFormatNormal.Dispose();
                m_stringFormatNormal = null;
            }
        }

        //=========================================================================================
        // 機　能：装飾機能付きのフォントを返す
        // 引　数：[in]decoration  装飾情報
        // 戻り値：フォント
        //=========================================================================================
        public Font GetConsoleDecorationFont(short decoration) {
            if ((decoration & ConsoleDecoration.UNDER_LINE) != 0) {
                if (m_logWindowFixedUnderlineFont != null) {
                    return m_logWindowFixedUnderlineFont;
                } else {
                    m_logWindowFixedUnderlineFont = new Font(Configuration.Current.LogWindowFixedFontName, m_fontSize, FontStyle.Underline);
                    return m_logWindowFixedUnderlineFont;
                }
            } else {
                return LogWindowFixedFont;
            }
        }

        //=========================================================================================
        // 機　能：装飾機能付きのブラシを返す
        // 引　数：[in]decoration  装飾情報（色情報のデフォルトは実際の色に解決済み）
        // 戻り値：ブラシ
        //=========================================================================================
        public Brush GetConsoleDecorationBrush(short decoration) {
            int color = decoration & ConsoleDecoration.COLOR_MASK;
            Brush brush = null;
            if (color != 0) {
                switch (color) {
                    case ConsoleDecoration.COLOR_DEF_FORE:
                        brush = LogWindowTextBrush;
                        break;
                    case ConsoleDecoration.COLOR_DEF_BACK:
                        brush = LogWindowBackBrush;
                        break;
                    case ConsoleDecoration.COLOR_BLACK:
                        brush = Brushes.Black;
                        break;
                    case ConsoleDecoration.COLOR_RED:
                        brush = Brushes.Red;
                        break;
                    case ConsoleDecoration.COLOR_BLUE:
                        brush = Brushes.Blue;
                        break;
                    case ConsoleDecoration.COLOR_MAGENTA:
                        brush = Brushes.Magenta;
                        break;
                    case ConsoleDecoration.COLOR_GREEN:
                        brush = Brushes.Green;
                        break;
                    case ConsoleDecoration.COLOR_CYAN:
                        brush = Brushes.Cyan;
                        break;
                    case ConsoleDecoration.COLOR_YELLOW:
                        brush = Brushes.Yellow;
                        break;
                    case ConsoleDecoration.COLOR_WHITE:
                        brush = Brushes.White;
                        break;
                    default:
                        Program.Abort("色情報の制御が不正です:{0}", color);
                        break;
                }
            } else {
                color = (decoration & ConsoleDecoration.BACK_MASK) >> ConsoleDecoration.BACK_SHIFT;
                switch (color) {
                    case ConsoleDecoration.COLOR_DEF_FORE:
                        brush = LogWindowTextBrush;
                        break;
                    case ConsoleDecoration.COLOR_DEF_BACK:
                        brush = LogWindowBackBrush;
                        break;
                    case ConsoleDecoration.COLOR_BLACK:
                        brush = Brushes.Black;
                        break;
                    case ConsoleDecoration.COLOR_RED:
                        brush = Brushes.DarkRed;
                        break;
                    case ConsoleDecoration.COLOR_BLUE:
                        brush = Brushes.DarkBlue;
                        break;
                    case ConsoleDecoration.COLOR_MAGENTA:
                        brush = Brushes.DarkMagenta;
                        break;
                    case ConsoleDecoration.COLOR_GREEN:
                        brush = Brushes.DarkGreen;
                        break;
                    case ConsoleDecoration.COLOR_CYAN:
                        brush = Brushes.DarkCyan;
                        break;
                    case ConsoleDecoration.COLOR_YELLOW:
                        brush = Brushes.Gold;
                        break;
                    case ConsoleDecoration.COLOR_WHITE:
                        brush = Brushes.White;
                        break;
                    default:
                        Program.Abort("色情報の制御が不正です:{0}", color);
                        break;
                }
            }
            return brush;
        }

        //=========================================================================================
        // プロパティ：グラフィックス
        //=========================================================================================
        public Graphics Graphics {
            get {
                if (m_graphics == null) {
                    m_graphics = m_control.CreateGraphics();
                }
                return m_graphics;
            }
        }
        
        //=========================================================================================
        // プロパティ：ログウィンドウ テキスト描画用のブラシ
        //=========================================================================================
        public Brush LogWindowTextBrush {
            get {
                if (m_logWindowTextBrush == null) {
                    m_logWindowTextBrush = new SolidBrush(Configuration.Current.LogWindowTextColor);
                }
                return m_logWindowTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ リンクテキスト描画用のブラシ
        //=========================================================================================
        public Brush LogWindowLinkTextBrush {
            get {
                if (m_logWindowLinkTextBrush == null) {
                    m_logWindowLinkTextBrush = new SolidBrush(Configuration.Current.LogWindowLinkTextColor);
                }
                return m_logWindowLinkTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ エラーテキスト描画用のブラシ
        //=========================================================================================
        public Brush LogErrorTextBrush {
            get {
                if (m_logLogErrorTextBrush == null) {
                    m_logLogErrorTextBrush = new SolidBrush(Configuration.Current.LogErrorTextColor);
                }
                return m_logLogErrorTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 標準エラーテキスト描画用のブラシ
        //=========================================================================================
        public Brush LogStdErrorTextBrush {
            get {
                if (m_logLogStdErrorTextBrush == null) {
                    m_logLogStdErrorTextBrush = new SolidBrush(Configuration.Current.LogStdErrorTextColor);
                }
                return m_logLogStdErrorTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 選択中のテキスト描画用のブラシ
        //=========================================================================================
        public Brush LogWindowSelectTextBrush {
            get {
                if (m_logWindowSelectTextBrush == null) {
                    m_logWindowSelectTextBrush = new SolidBrush(Configuration.Current.LogWindowSelectTextColor);
                }
                return m_logWindowSelectTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 背景描画用のブラシ
        //=========================================================================================
        public Brush LogWindowBackBrush {
            get {
                if (m_logWindowBackBrush == null) {
                    switch (m_backColorMode) {
                        case BackColorMode.Normal:
                            m_logWindowBackBrush = new SolidBrush(Configuration.Current.LogWindowBackColor);
                            break;
                        case BackColorMode.VisualBell:
                            m_logWindowBackBrush = new SolidBrush(Configuration.Current.LogWindowBackBellColor);
                            break;
                        case BackColorMode.Closed:
                            m_logWindowBackBrush = new SolidBrush(Configuration.Current.LogWindowBackClosedColor);
                            break;
                    }
                }
                return m_logWindowBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 選択中の背景描画用のブラシ
        //=========================================================================================
        public Brush LogWindowSelectBackBrush {
            get {
                if (m_logWindowSelectBackBrush == null) {
                    m_logWindowSelectBackBrush = new SolidBrush(Configuration.Current.LogWindowSelectBackColor);
                }
                return m_logWindowSelectBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 残り時間描画用のフォントブラシ（明）
        //=========================================================================================
        public Brush LogWindowRemainingTimeTextBrush1 {
            get {
                if (m_logWindowRemainingTimeTextBrush1 == null) {
                    m_logWindowRemainingTimeTextBrush1= new SolidBrush(Configuration.Current.LogWindowRemainingTimeTextColor1);
                }
                return m_logWindowRemainingTimeTextBrush1;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 残り時間描画用のフォントブラシ（暗）
        //=========================================================================================
        public Brush LogWindowRemainingTimeTextBrush2 {
            get {
                if (m_logWindowRemainingTimeTextBrush2 == null) {
                    m_logWindowRemainingTimeTextBrush2 = new SolidBrush(Configuration.Current.LogWindowRemainingTimeTextColor2);
                }
                return m_logWindowRemainingTimeTextBrush2;
            }
        }        

        //=========================================================================================
        // プロパティ：ログウィンドウ 破線描画用のペン
        //=========================================================================================
        public Pen LogWindowLeaderPen {
            get {
                if (m_logWindowLeaderPen == null) {
                    m_logWindowLeaderPen = new Pen(Configuration.Current.LogWindowTextColor);
                    m_logWindowLeaderPen.DashStyle = DashStyle.Dot;
                }
                return m_logWindowLeaderPen;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 選択中の破線描画用のペン
        //=========================================================================================
        public Pen LogWindowLeaderSelectPen {
            get {
                if (m_logWindowLeaderSelectPen == null) {
                    m_logWindowLeaderSelectPen = new Pen(Configuration.Current.LogWindowSelectTextColor);
                    m_logWindowLeaderSelectPen.DashStyle = DashStyle.Dot;
                }
                return m_logWindowLeaderSelectPen;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 進捗表示領域のペン
        //=========================================================================================
        public Pen LogWindowProgressPen {
            get {
                if (m_logWindowProgressPen == null) {
                    m_logWindowProgressPen = new Pen(Configuration.Current.LogWindowTextColor);
                }
                return m_logWindowProgressPen;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 描画用のフォント
        //=========================================================================================
        public Font LogWindowFont {
            get {
                if (m_logWindowFont == null) {
                    m_logWindowFont = new Font(Configuration.Current.LogWindowFontName, m_fontSize);
                }
                return m_logWindowFont;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 等幅描画用のフォント
        //=========================================================================================
        public Font LogWindowFixedFont {
            get {
                if (m_logWindowFixedFont == null) {
                    m_logWindowFixedFont = new Font(Configuration.Current.LogWindowFixedFontName, m_fontSize);
                }
                return m_logWindowFixedFont;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ リンク描画用のフォント
        //=========================================================================================
        public Font LogWindowLinkFont {
            get {
                if (m_logWindowLinkFont == null) {
                    m_logWindowLinkFont = new Font(Configuration.Current.LogWindowFontName, m_fontSize, FontStyle.Bold | FontStyle.Underline);
                }
                return m_logWindowLinkFont;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 描画用のボールドフォント
        //=========================================================================================
        public Font LogWindowBoldFont {
            get {
                if (m_logWindowBoldFont == null) {
                    m_logWindowBoldFont = new Font(Configuration.Current.LogWindowFontName, m_fontSize, FontStyle.Bold);
                }
                return m_logWindowBoldFont;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 残り時間描画用のフォント
        //=========================================================================================
        public Font LogWindowRemainingTimeFont {
            get {
                if (m_logWindowRemainingTimeFont == null) {
                    m_logWindowRemainingTimeFont = new Font(Configuration.Current.LogWindowFontName, m_fontSize * 0.8f);
                }
                return m_logWindowRemainingTimeFont;
            }
        }
        
        //=========================================================================================
        // プロパティ：省略記号付きのフォーマッタ
        //=========================================================================================
        public StringFormat StringFormatEllipsis {
            get {
                if (m_stringFormatEllipsis == null) {
                    m_stringFormatEllipsis = new StringFormat();
                    m_stringFormatEllipsis.Trimming = StringTrimming.EllipsisCharacter;
                }
                return m_stringFormatEllipsis;
            }
        }

        //=========================================================================================
        // プロパティ：通常のフォーマッタ
        //=========================================================================================
        public StringFormat StringFormatNormal {
            get {
                if (m_stringFormatNormal == null) {
                    m_stringFormatNormal = new StringFormat();
                }
                return m_stringFormatNormal;
            }
        }

        //=========================================================================================
        // 列挙子：背景色の表示モード
        //=========================================================================================
        public enum BackColorMode {
            Normal,                 // 通常
            VisualBell,             // ビジュアルベルの表示
            Closed,                 // セッション切断済み
        }
    }
}
