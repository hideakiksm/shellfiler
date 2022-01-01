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

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：オーナードローのリストボックス描画用グラフィックス
    //=========================================================================================
    public class OwnerDrawListBoxGraphics {
        // グラフィック（null:未初期化）
        private Graphics m_graphics;

        // 描画対象のコントロール（null:グラフィック指定）
        private Control m_control;

        // グラデーションでの行の開始位置
        private int m_lineStart;

        // グラデーションでの行の高さ
        private int m_lineHeight;

        // 背景の枠描画用のペン
        private Pen m_borderPen = null;

        // グレーアウト背景の枠描画用のペン
        private Pen m_grayBorderPen = null;

        // 選択中背景描画用のブラシ
        private Brush m_markBackBrush = null;

        // グレーアウト選択中背景色のブラシ
        private Brush m_markGrayBackBrush = null;

        // グレーアウト背景色のブラシ
        private Brush m_grayBackBrush = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics   グラフィックス
        // 　　　　[in]lineStart  グラデーションでの行の開始位置
        // 　　　　[in]lineHeight グラデーションでの行の高さ
        // 戻り値：なし
        //=========================================================================================
        public OwnerDrawListBoxGraphics(Graphics graphics, int lineStart, int lineHeight) {
            m_control = null;
            m_graphics = graphics;
            m_lineStart = lineStart;
            m_lineHeight = lineHeight;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（コントロール指定で必要時に初期化）
        // 引　数：[in]control    描画対象のコントロール
        // 　　　　[in]lineHeight 一覧の行の高さ
        // 戻り値：なし
        //=========================================================================================
        public OwnerDrawListBoxGraphics(Control control, int lineHeight) {
            m_control = control;
            m_graphics = null;
            m_lineHeight = lineHeight;
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
            if (m_borderPen != null) {
                m_borderPen.Dispose();
                m_borderPen = null;
            }
            if (m_grayBorderPen != null) {
                m_grayBorderPen.Dispose();
                m_grayBorderPen = null;
            }
            if (m_markBackBrush != null) {
                m_markBackBrush.Dispose();
                m_markBackBrush = null;
            }
            if (m_markGrayBackBrush != null) {
                m_markGrayBackBrush.Dispose();
                m_markGrayBackBrush = null;
            }
            if (m_grayBackBrush != null) {
                m_grayBackBrush.Dispose();
                m_grayBackBrush = null;
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
        // プロパティ：背景の枠描画用のペン
        //=========================================================================================
        public Pen BorderPen {
            get {
                if (m_borderPen == null) {
                    m_borderPen = new Pen(Configuration.Current.FileListMarkBackBorderColor);
                }
                return m_borderPen;
            }
        }

        //=========================================================================================
        // プロパティ：グレーアウト背景の枠描画用のペン
        //=========================================================================================
        public Pen GrayBorderPen {
            get {
                if (m_grayBorderPen == null) {
                    m_grayBorderPen = new Pen(Configuration.Current.FileListMarkGrayBackBorderColor);
                }
                return m_grayBorderPen;
            }
        }

        //=========================================================================================
        // プロパティ：選択中背景描画用のブラシ
        //=========================================================================================
        public Brush MarkBackBrush {
            get {
                if (m_markBackBrush == null) {
                    if (Configuration.Current.FileListMarkBackColor1.Equals(Configuration.Current.FileListMarkBackColor2)) {
                        m_markBackBrush = new SolidBrush(Configuration.Current.FileListMarkBackColor1);
                    } else {
                        Rectangle rc = new Rectangle(0, m_lineStart, 10, m_lineHeight);
                        m_markBackBrush = new LinearGradientBrush(rc, Configuration.Current.FileListMarkBackColor1, Configuration.Current.FileListMarkBackColor2, LinearGradientMode.Vertical);
                    }
                }
                return m_markBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：グレーアウト選択中背景色のブラシ
        //=========================================================================================
        public Brush MarkGrayBackBrush {
            get {
                if (m_markGrayBackBrush == null) {
                    if (Configuration.Current.FileListMarkGrayBackColor1.Equals(Configuration.Current.FileListMarkGrayBackColor2)) {
                        m_markGrayBackBrush = new SolidBrush(Configuration.Current.FileListMarkGrayBackColor1);
                    } else {
                        Rectangle rc = new Rectangle(0, m_lineStart, 10, m_lineHeight);
                        m_markGrayBackBrush = new LinearGradientBrush(rc, Configuration.Current.FileListMarkGrayBackColor1, Configuration.Current.FileListMarkGrayBackColor2, LinearGradientMode.Vertical);
                    } 
                }
                return m_markGrayBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：グレーアウト背景色のブラシ
        //=========================================================================================
        public Brush GrayBackBrush {
            get {
                if (m_grayBackBrush == null) {
                    Color col1 = Configuration.Current.FileListMarkGrayBackColor1;
                    Color col2 = Configuration.Current.FileListMarkGrayBackColor2;
                    int r = ((int)(col1.R) + (int)(col2.R) + 255 * 3) / 5;
                    int g = ((int)(col1.G) + (int)(col2.G) + 255 * 3) / 5;
                    int b = ((int)(col1.B) + (int)(col2.B) + 255 * 3) / 5;
                    m_grayBackBrush = new SolidBrush(Color.FromArgb(r, g, b));
                }
                return m_grayBackBrush;
            }
        }
    }
}
