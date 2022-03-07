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

namespace ShellFiler.UI {

    //=========================================================================================
    // クラス：スプラッシュウィンドウの描画用グラフィックス
    //=========================================================================================
    public class SplashWindowGraphics : HighDpiGraphics {
        // グラフィック（null:未初期化）
        private Graphics m_graphics;

        // 各種描画用のフォント
        private Font m_etcFont = null;

        // ライセンス描画用のフォント
        private Font m_licenseFont = null;

        // 周辺ライブラリ描画用のフォント
        private Font m_libraryFont = null;

        // ライセンス用のブラシ
        private Brush m_licenseBrush = null;

        // 描画時のフォーマッタ
        private StringFormat m_stringFormat = null;

        // センタリング時のフォーマッタ
        private StringFormat m_stringFormatCentering = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public SplashWindowGraphics(Graphics graphics) : base(graphics) {
            m_graphics = graphics;
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public new void Dispose() {
            base.Dispose();
            if (m_etcFont != null) {
                m_etcFont.Dispose();
                m_etcFont = null;
            }
            if (m_libraryFont != null) {
                m_libraryFont.Dispose();
                m_libraryFont = null;
            }
            if (m_licenseFont != null) {
                m_licenseFont.Dispose();
                m_licenseFont = null;
            }
            if (m_licenseBrush != null) {
                m_licenseBrush.Dispose();
                m_licenseBrush = null;
            }
            if (m_stringFormat != null) {
                m_stringFormat.Dispose();
                m_stringFormat = null;
            }
            if (m_stringFormatCentering != null) {
                m_stringFormatCentering.Dispose();
                m_stringFormatCentering = null;
            }
        }

        //=========================================================================================
        // プロパティ：各種描画用のフォント
        //=========================================================================================
        public Font EtcFont {
            get {
                if (m_etcFont == null) {
                    m_etcFont = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Italic);
                }
                return m_etcFont;
            }
        }

        //=========================================================================================
        // プロパティ：ライセンス用のフォント
        //=========================================================================================
        public Font LicenseFont {
            get {
                if (m_licenseFont == null) {
                    m_licenseFont = new Font("Yu Gothic UI", 9.0f);
                }
                return m_licenseFont;
            }
        }
        
        //=========================================================================================
        // プロパティ：周辺ライブラリ描画用のフォント
        //=========================================================================================
        public Font LibraryFont {
            get {
                if (m_libraryFont == null) {
                    m_libraryFont = new Font("MS UI Gothic", 8.0f);
                }
                return m_libraryFont;
            }
        }

        //=========================================================================================
        // プロパティ：ライセンス用のブラシ
        //=========================================================================================
        public Brush LicenseBrush {
            get {
                if (m_licenseBrush == null) {
                    m_licenseBrush = new SolidBrush(Color.FromArgb(64, 96, 220));
                }
                return m_licenseBrush;
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

        //=========================================================================================
        // プロパティ：センタリング時のフォーマッタ
        //=========================================================================================
        public StringFormat StringFormatCentering {
            get {
                if (m_stringFormatCentering == null) {
                    m_stringFormatCentering = new StringFormat();
                    m_stringFormatCentering.Alignment = StringAlignment.Center;
                }
                return m_stringFormatCentering;
            }
        }
    }
}
