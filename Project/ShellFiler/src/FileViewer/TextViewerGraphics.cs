using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using System.Runtime.InteropServices;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：テキストビューアの描画用グラフィックス
    //=========================================================================================
    public class TextViewerGraphics {
        // グラフィック（null:未初期化）
        private Graphics m_graphics;

        // 描画対象のコントロール（null:グラフィック指定）
        private Control m_control;

        // 行番号領域の幅
        private float m_cxLineNo;

        // テキストビューア 行番号のブラシ
        private Brush m_textViewerLineNoTextBrush = null;

        // テキストビューア 行番号の背景色のブラシ
        private Brush m_textViewerLineNoBackBrush = null;

        // テキストビューア 行番号境界のペン
        private Pen m_textViewerLineNoSeparatorPen = null;

        // テキストビューア 検索カーソルのペン
        private Pen m_searchCursorPen = null;

        // テキストビューア 制御関連のペン
        private Pen m_textViewerControlPen = null;

        // テキストビューア 制御関連のブラシ
        private Brush m_textViewerControlBrush = null;

        // テキストビューア 領域外のブラシ
        private Brush m_textViewerOutOfAreaBackPen;

        // テキストビューア 領域外分離線の色
        private Pen m_textViewerOutOfAreaSeparatorPen;

        // テキストビューア テキストのブラシ
        private Brush m_textViewerTextBrush = null;

        // テキストビューア 選択中テキストのブラシ
        private Brush m_textViewerSelectTextBrush = null;

        // テキストビューア 選択中テキストのブラシ２
        private Brush m_textViewerSelectTextBrush2 = null;

        // テキストビューア 選択中背景のブラシ
        private Brush m_textViewerSelectBackBrush = null;

        // テキストビューア 選択中背景のブラシ２
        private Brush m_textViewerSelectBackBrush2 = null;

        // テキストビューア 検索ヒット背景のブラシ
        private Brush m_textViewerSearchHitBackBrush = null;
        
        // テキストビューア 検索ヒットテキストのブラシ
        private Brush m_textViewerSearchHitTextBrush = null;

        // テキストビューア 自動検索ヒットテキストのブラシ
        private Brush m_textViewerSearchAutoTextBrush = null;

        // テキストビューア 描画用のフォント
        private Font m_textFont = null;
        
        // 描画時のフォーマッタ
        private StringFormat m_stringFormat = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics  グラフィックス
        // 　　　　[in]cxLineNo  行番号領域の幅
        // 戻り値：なし
        //=========================================================================================
        public TextViewerGraphics(Graphics graphics, float cxLineNo) {
            m_control = null;
            m_graphics = graphics;
            m_cxLineNo = cxLineNo;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（コントロール指定で必要時に初期化）
        // 引　数：[in]control    描画対象のコントロール
        // 　　　　[in]cxLineNo  行番号領域の幅
        // 戻り値：なし
        //=========================================================================================
        public TextViewerGraphics(Control control, float cxLineNo) {
            m_control = control;
            m_graphics = null;
            m_cxLineNo = cxLineNo;
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
            if (m_textViewerLineNoTextBrush != null) {
                m_textViewerLineNoTextBrush.Dispose();
                m_textViewerLineNoTextBrush = null;
            }
            if (m_textViewerLineNoBackBrush != null) {
                m_textViewerLineNoBackBrush.Dispose();
                m_textViewerLineNoBackBrush = null;
            }
            if (m_textViewerLineNoSeparatorPen != null) {
                m_textViewerLineNoSeparatorPen.Dispose();
                m_textViewerLineNoSeparatorPen = null;
            }
            if (m_textViewerControlPen != null) {
                m_textViewerControlPen.Dispose();
                m_textViewerControlPen = null;
            }
            if (m_searchCursorPen != null) {
                m_searchCursorPen.Dispose();
                m_searchCursorPen = null;
            }
            if (m_textViewerControlBrush != null) {
                m_textViewerControlBrush.Dispose();
                m_textViewerControlBrush = null;
            }
            if (m_textViewerOutOfAreaBackPen != null) {
                m_textViewerOutOfAreaBackPen.Dispose();
                m_textViewerOutOfAreaBackPen = null;
            }
            if (m_textViewerOutOfAreaSeparatorPen != null) {
                m_textViewerOutOfAreaSeparatorPen.Dispose();
                m_textViewerOutOfAreaSeparatorPen = null;
            }
            if (m_textViewerTextBrush != null) {
                m_textViewerTextBrush.Dispose();
                m_textViewerTextBrush = null;
            }
            if (m_textViewerSelectTextBrush != null) {
                m_textViewerSelectTextBrush.Dispose();
                m_textViewerSelectTextBrush = null;
            }
            if (m_textViewerSelectTextBrush2 != null) {
                m_textViewerSelectTextBrush2.Dispose();
                m_textViewerSelectTextBrush2 = null;
            }
            if (m_textViewerSelectBackBrush != null) {
                m_textViewerSelectBackBrush.Dispose();
                m_textViewerSelectBackBrush = null;
            }
            if (m_textViewerSelectBackBrush2 != null) {
                m_textViewerSelectBackBrush2.Dispose();
                m_textViewerSelectBackBrush2 = null;
            }
            if (m_textViewerSearchHitBackBrush != null) {
                m_textViewerSearchHitBackBrush.Dispose();
                m_textViewerSearchHitBackBrush = null;
            }
            if (m_textViewerSearchHitTextBrush != null) {
                m_textViewerSearchHitTextBrush.Dispose();
                m_textViewerSearchHitTextBrush = null;
            }
            if (m_textViewerSearchAutoTextBrush != null) {
                m_textViewerSearchAutoTextBrush.Dispose();
                m_textViewerSearchAutoTextBrush = null;
            }
            if (m_textFont != null) {
                m_textFont.Dispose();
                m_textFont = null;
            }
            if (m_stringFormat != null) {
                m_stringFormat.Dispose();
                m_stringFormat = null;
            }
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
        // プロパティ：テキストビューア 行番号のブラシ
        //=========================================================================================
        public Brush TextViewerLineNoTextBrush {
            get {
                if (m_textViewerLineNoTextBrush == null) {
                    m_textViewerLineNoTextBrush = new SolidBrush(Configuration.Current.TextViewerLineNoTextColor);
                }
                return m_textViewerLineNoTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 行番号の背景色のブラシ
        //=========================================================================================
        public Brush TextViewerLineNoBackBrush {
            get {
                if (m_textViewerLineNoBackBrush == null) {
                    if (Configuration.Current.TextViewerLineNoBackColor1.Equals(Configuration.Current.TextViewerLineNoBackColor2)) {
                        m_textViewerLineNoBackBrush = new SolidBrush(Configuration.Current.TextViewerLineNoBackColor1);
                    } else {
                        RectangleF rc = new RectangleF(0, 0, Math.Max(1, m_cxLineNo), 10);
                        m_textViewerLineNoBackBrush = new LinearGradientBrush(rc, Configuration.Current.TextViewerLineNoBackColor1, Configuration.Current.TextViewerLineNoBackColor2, LinearGradientMode.Horizontal);
                    }
                }
                return m_textViewerLineNoBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 行番号境界のペン
        //=========================================================================================
        public Pen TextViewerLineNoSeparatorPen {
            get {
                if (m_textViewerLineNoSeparatorPen == null) {
                    m_textViewerLineNoSeparatorPen = new Pen(Configuration.Current.TextViewerLineNoSeparatorColor);
                }
                return m_textViewerLineNoSeparatorPen;
            }
        }
        
        //=========================================================================================
        // プロパティ：テキストビューア 領域外のブラシ
        //=========================================================================================
        public Brush TextViewerOutOfAreaBackBrush {
            get {
                if (m_textViewerOutOfAreaBackPen == null) {
                    m_textViewerOutOfAreaBackPen = new SolidBrush(Configuration.Current.TextViewerOutOfAreaBackColor);
                }
                return m_textViewerOutOfAreaBackPen;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 領域外分離線の色
        //=========================================================================================
        public Pen TextViewerOutOfAreaSeparatorPen {
            get {
                if (m_textViewerOutOfAreaSeparatorPen == null) {
                    m_textViewerOutOfAreaSeparatorPen = new Pen(Configuration.Current.TextViewerOutOfAreaSeparatorColor);
                }
                return m_textViewerOutOfAreaSeparatorPen;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 検索カーソルのペン
        //=========================================================================================
        public Pen SearchCursorPen {
            get {
                if (m_searchCursorPen == null) {
                    m_searchCursorPen = new Pen(Configuration.Current.TextViewerSearchCursorColor);
                }
                return m_searchCursorPen;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア テキストのブラシ
        //=========================================================================================
        public Brush TextViewerTextBrush {
            get {
                if (m_textViewerTextBrush == null) {
                    m_textViewerTextBrush = new SolidBrush(Configuration.Current.TextViewerTextColor);
                }
                return m_textViewerTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 選択中のテキストのブラシ
        //=========================================================================================
        public Brush TextViewerSelectTextBrush {
            get {
                if (m_textViewerSelectTextBrush == null) {
                    m_textViewerSelectTextBrush = new SolidBrush(Configuration.Current.TextViewerSelectTextColor);
                }
                return m_textViewerSelectTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 選択中のテキストのブラシ２
        //=========================================================================================
        public Brush TextViewerSelectTextBrush2 {
            get {
                if (m_textViewerSelectTextBrush2 == null) {
                    m_textViewerSelectTextBrush2 = new SolidBrush(Configuration.Current.TextViewerSelectTextColor2);
                }
                return m_textViewerSelectTextBrush2;
            }
        }
                
        //=========================================================================================
        // プロパティ：テキストビューア 選択中背景のブラシ
        //=========================================================================================
        public Brush TextViewerSelectBackBrush {
            get {
                if (m_textViewerSelectBackBrush == null) {
                    m_textViewerSelectBackBrush = new SolidBrush(Configuration.Current.TextViewerSelectBackColor);
                }
                return m_textViewerSelectBackBrush;
            }
        }
        
        //=========================================================================================
        // プロパティ：テキストビューア 選択中背景のブラシ２
        //=========================================================================================
        public Brush TextViewerSelectBackBrush2 {
            get {
                if (m_textViewerSelectBackBrush2 == null) {
                    m_textViewerSelectBackBrush2 = new SolidBrush(Configuration.Current.TextViewerSelectBackColor2);
                }
                return m_textViewerSelectBackBrush2;
            }
        }
        
        //=========================================================================================
        // プロパティ：テキストビューア 検索ヒット背景のブラシ
        //=========================================================================================
        public Brush TextViewerSearchHitBackBrush {
            get {
                if (m_textViewerSearchHitBackBrush == null) {
                    m_textViewerSearchHitBackBrush = new SolidBrush(Configuration.Current.TextViewerSearchHitBackColor);
                }
                return m_textViewerSearchHitBackBrush;
            }
        }
        
        //=========================================================================================
        // プロパティ：テキストビューア 検索ヒットテキストのブラシ
        //=========================================================================================
        public Brush TextViewerSearchHitTextBrush {
            get {
                if (m_textViewerSearchHitTextBrush == null) {
                    m_textViewerSearchHitTextBrush = new SolidBrush(Configuration.Current.TextViewerSearchHitTextColor);
                }
                return m_textViewerSearchHitTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 自動検索ヒットテキストのブラシ
        //=========================================================================================
        public Brush TextViewerSearchAutoTextBrush {
            get {
                if (m_textViewerSearchAutoTextBrush == null) {
                    m_textViewerSearchAutoTextBrush = new SolidBrush(Configuration.Current.TextViewerSearchAutoTextColor);
                }
                return m_textViewerSearchAutoTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 制御関連のペン
        //=========================================================================================
        public Pen TextViewerControlPen {
            get {
                if (m_textViewerControlPen == null) {
                    m_textViewerControlPen = new Pen(Configuration.Current.TextViewerControlColor);
                }
                return m_textViewerControlPen;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 制御関連のブラシ
        //=========================================================================================
        public Brush TextViewerControlBrush {
            get {
                if (m_textViewerControlBrush == null) {
                    m_textViewerControlBrush = new SolidBrush(Configuration.Current.TextViewerControlColor);
                }
                return m_textViewerControlBrush;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 描画用のフォント
        //=========================================================================================
        public Font TextFont {
            get {
                if (m_textFont == null) {
                    m_textFont = new Font(Configuration.Current.TextFontName, Configuration.Current.TextFontSize);
                }
                return m_textFont;
            }
        }

        //=========================================================================================
        // プロパティ：文字列のフォーマッタ
        //=========================================================================================
        public StringFormat StringFormat {
            get {
                if (m_stringFormat == null) {
                    m_stringFormat = new StringFormat();
                }
                return m_stringFormat;
            }
        }
    }
}
