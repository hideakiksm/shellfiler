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

namespace ShellFiler.GraphicsViewer {

    //=========================================================================================
    // クラス：グラフィックビューアの描画用グラフィックス
    //=========================================================================================
    public class GraphicsViewerGraphics {
        // グラフィック
        private Graphics m_graphics;

        // 描画対象のコントロール
        private Control m_control;
        
        // グラフィックビューアの表示色
        private GraphicsViewerColor m_graphicViewerColor;

        // グラフィックビューア テキスト表示のブラシ
        private Brush m_graphicsViewerTextBrush = null;

        // グラフィックビューア テキスト表示影のブラシ
        private Brush m_graphicsViewerTextShadowBrush = null;

        // グラフィックビューア 読み込み中テキスト表示のブラシ
        private Brush m_graphicsLoadingViewerTextBrush = null;

        // グラフィックビューア 読み込み中テキスト表示影のブラシ
        private Brush m_graphicsLoadingViewerTextShadowBrush = null;

        // グラフィックビューア メッセージ描画用のフォント
        private Font m_messageFont = null;
        
        // グラフィックビューア ファイル名描画用のフォント
        private Font m_fileNameFont = null;

        // 描画時のフォーマッタ
        private StringFormat m_stringFormat = null;

        // 右寄せ描画時のフォーマッタ
        private StringFormat m_stringFormatRight = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]control   描画対象のコントロール
        // 　　　　[in]graphics  グラフィックス
        //         [in]color     グラフィックビューアの色
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerGraphics(Control control, Graphics graphics, GraphicsViewerColor color) {
            m_control = control;
            m_graphics = graphics;
            m_graphicViewerColor = color;
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_graphicsViewerTextBrush != null) {
                m_graphicsViewerTextBrush.Dispose();
                m_graphicsViewerTextBrush = null;
            }
            if (m_graphicsViewerTextShadowBrush != null) {
                m_graphicsViewerTextShadowBrush.Dispose();
                m_graphicsViewerTextShadowBrush = null;
            }
            if (m_graphicsLoadingViewerTextBrush != null) {
                m_graphicsLoadingViewerTextBrush.Dispose();
                m_graphicsLoadingViewerTextBrush = null;
            }
            if (m_graphicsLoadingViewerTextShadowBrush != null) {
                m_graphicsLoadingViewerTextShadowBrush.Dispose();
                m_graphicsLoadingViewerTextShadowBrush = null;
            }
            if (m_messageFont != null) {
                m_messageFont.Dispose();
                m_messageFont = null;
            }
            if (m_fileNameFont != null) {
                m_fileNameFont.Dispose();
                m_fileNameFont = null;
            }
            if (m_stringFormat != null) {
                m_stringFormat.Dispose();
                m_stringFormat = null;
            }
            if (m_stringFormatRight != null) {
                m_stringFormatRight.Dispose();
                m_stringFormatRight = null;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィック
        //=========================================================================================
        public Graphics Graphics {
            get {
                return m_graphics;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア テキスト表示のブラシ
        //=========================================================================================
        public Brush GraphicsViewerTextBrush {
            get {
                if (m_graphicsViewerTextBrush == null) {
                    m_graphicsViewerTextBrush = new SolidBrush(m_graphicViewerColor.GraphicsViewerTextColor);
                }
                return m_graphicsViewerTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア テキスト表示影のブラシ
        //=========================================================================================
        public Brush GraphicsViewerTextShadowBrush {
            get {
                if (m_graphicsViewerTextShadowBrush == null) {
                    m_graphicsViewerTextShadowBrush = new SolidBrush(m_graphicViewerColor.GraphicsViewerTextShadowColor);
                }
                return m_graphicsViewerTextShadowBrush;
            }
        }
        //=========================================================================================
        // プロパティ：グラフィックビューア 読み込み中テキスト表示のブラシ
        //=========================================================================================
        public Brush GraphicsLoadingViewerTextBrush {
            get {
                if (m_graphicsLoadingViewerTextBrush == null) {
                    m_graphicsLoadingViewerTextBrush = new SolidBrush(m_graphicViewerColor.GraphicsViewerLoadingTextColor);
                }
                return m_graphicsLoadingViewerTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア 読み込み中テキスト表示影のブラシ
        //=========================================================================================
        public Brush GraphicsLoadingViewerTextShadowBrush {
            get {
                if (m_graphicsLoadingViewerTextShadowBrush == null) {
                    m_graphicsLoadingViewerTextShadowBrush = new SolidBrush(m_graphicViewerColor.GraphicsViewerLoadingTextShadowColor);
                }
                return m_graphicsLoadingViewerTextShadowBrush;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア メッセージ描画用のフォント
        //=========================================================================================
        public Font GraphicsViewerMessageFont {
            get {
                if (m_messageFont == null) {
                    m_messageFont = new Font(m_control.Font.FontFamily, Configuration.Current.GraphicsViewerMessageFontSize, FontStyle.Regular);
                }
                return m_messageFont;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア ファイル名描画用のフォント
        //=========================================================================================
        public Font GraphicsViewerFileNameFont {
            get {
                if (m_fileNameFont == null) {
                    m_fileNameFont = new Font(m_control.Font.FontFamily, Configuration.Current.GraphicsViewerFileNameFontSize, FontStyle.Regular);
                }
                return m_fileNameFont;
            }
        }

        //=========================================================================================
        // プロパティ：描画時のフォーマッタ
        //=========================================================================================
        public StringFormat StringFormat {
            get {
                if (m_stringFormat == null) {
                    m_stringFormat = new StringFormat();
                    m_stringFormat.Alignment = StringAlignment.Center;
                }
                return m_stringFormat;
            }
        }

        //=========================================================================================
        // プロパティ：右寄せ描画時のフォーマッタ
        //=========================================================================================
        public StringFormat StringFormatRight {
            get {
                if (m_stringFormatRight == null) {
                    m_stringFormatRight = new StringFormat();
                    m_stringFormatRight.Alignment = StringAlignment.Far;
                }
                return m_stringFormatRight;
            }
        }
    }
}
