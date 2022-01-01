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

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：2ストロークキーの状態表示ボタンの描画用グラフィックス
    //=========================================================================================
    public class TwoStrokeButtonGraphics {
        // グラフィック
        private Graphics m_graphics;

        // ハイライト部分描画用のブラシ
        private Brush m_highlightBrush = null;

        // 文字部分描画用のブラシ
        private Brush m_fontBrush = null;

        // 標準サイズの文字描画用のフォント
        private Font m_normalFont = null;

        // 縮小サイズの文字描画用のフォント
        private Font m_smallFont = null;

        // 描画時のフォーマッタ
        private StringFormat m_stringFormat = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public TwoStrokeButtonGraphics(Graphics graphics) {
            m_graphics = graphics;
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_highlightBrush != null) {
                m_highlightBrush.Dispose();
                m_highlightBrush = null;
            }
            if (m_fontBrush != null) {
                m_fontBrush.Dispose();
                m_fontBrush = null;
            }
            if (m_normalFont != null) {
                m_normalFont.Dispose();
                m_normalFont = null;
            }
            if (m_normalFont != null) {
                m_normalFont.Dispose();
                m_normalFont = null;
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
                return m_graphics;
            }
        }

        //=========================================================================================
        // プロパティ：ハイライト部分描画用のブラシ
        //=========================================================================================
        public Brush HighlightBrush {
            get {
                if (m_highlightBrush == null) {
                    m_highlightBrush = new SolidBrush(Color.FromArgb(255, 255, 255));
                }
                return m_highlightBrush;
            }
        }

        //=========================================================================================
        // プロパティ：文字部分描画用のブラシ
        //=========================================================================================
        public Brush FontBrush {
            get {
                if (m_fontBrush == null) {
                    m_fontBrush = new SolidBrush(Color.FromArgb(53, 73, 99));
                }
                return m_fontBrush;
            }
        }

        //=========================================================================================
        // プロパティ：標準サイズの文字描画用のフォント
        //=========================================================================================
        public Font NormalFont {
            get {
                if (m_normalFont == null) {
                    m_normalFont = new Font(UILocale.WindowsUIFontName, 10, FontStyle.Regular, GraphicsUnit.Pixel);
                }
                return m_normalFont;
            }
        }

        //=========================================================================================
        // プロパティ：縮小サイズの文字描画用のフォント
        //=========================================================================================
        public Font SmallFont {
            get {
                if (m_smallFont == null) {
                    m_smallFont = new Font(UILocale.WindowsUIFontName, 8, FontStyle.Regular, GraphicsUnit.Pixel);
                }
                return m_smallFont;
            }
        }
        
        //=========================================================================================
        // プロパティ：描画時のフォーマッタ
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
