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

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：メニューの描画用グラフィックス
    //=========================================================================================
    public class MenuGraphics {
        // グラフィック
        private Graphics m_graphics;

        // カラーテーブル
        private ProfessionalColorTable m_colorTable = new ProfessionalColorTable();

        // メニューの左側領域のブラシ
        private Brush m_leftAreaBrush = null;

        // メニューの左側領域のブラシを初期化したときのグラデーションの領域
        private Rectangle m_prevLeftAreaRect;

        // メニューの背景のブラシ
        private Brush m_menuBackBrush = null;

        // メニューの最上部のブラシ
        private Brush m_menuTopBrush = null;

        // 境界線のペン
        private Pen m_borderPen = null;

        // セパレータの上部のペン
        private Pen m_separatorLightPen = null;

        // セパレータの下部のペン
        private Pen m_separatorDarkPen = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public MenuGraphics(Graphics graphics) {
            m_graphics = graphics;
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_leftAreaBrush != null) {
                m_leftAreaBrush.Dispose();
                m_leftAreaBrush = null;
            }
            if (m_menuBackBrush != null) {
                m_menuBackBrush.Dispose();
                m_menuBackBrush = null;
            }
            if (m_menuTopBrush != null) {
                m_menuTopBrush.Dispose();
                m_menuTopBrush = null;
            }
            if (m_borderPen != null) {
                m_borderPen.Dispose();
                m_borderPen = null;
            }
            if (m_separatorLightPen != null) {
                m_separatorLightPen.Dispose();
                m_separatorLightPen = null;
            }
            if (m_separatorDarkPen != null) {
                m_separatorDarkPen.Dispose();
                m_separatorDarkPen = null;
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
        // 機　能：メニューの左側領域のブラシを取得する
        // 引　数：[in]rc  グラデーションを表示する領域
        // 戻り値：ブラシ
        //=========================================================================================
        public Brush LeftAreaBrush(Rectangle rc) {
            if (m_prevLeftAreaRect != rc && m_leftAreaBrush != null) {
                m_leftAreaBrush.Dispose();
                m_leftAreaBrush = null;
            }
            if (m_leftAreaBrush == null) {
                m_leftAreaBrush = new LinearGradientBrush(rc, m_colorTable.MenuStripGradientEnd, m_colorTable.MenuStripGradientBegin, LinearGradientMode.Horizontal);
                m_prevLeftAreaRect = rc;
            }
            return m_leftAreaBrush;
        }

        //=========================================================================================
        // プロパティ：メニューの背景のブラシ
        //=========================================================================================
        public Brush MenuBackBrush {
            get {
                if (m_menuBackBrush == null) {
                    m_menuBackBrush = new SolidBrush(m_colorTable.ToolStripDropDownBackground);
                }
                return m_menuBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：メニューの最上部のブラシ
        //=========================================================================================
        public Brush MenuTopBrush {
            get {
                if (m_menuTopBrush == null) {
                    RectangleF rc = new RectangleF(0, 1, 10, MenuSamplePanel.CY_TOP_AREA);
                    m_menuTopBrush = new LinearGradientBrush(rc, m_colorTable.MenuItemPressedGradientBegin, m_colorTable.MenuItemPressedGradientEnd, LinearGradientMode.Vertical);
                }
                return m_menuTopBrush;
            }
        }

        //=========================================================================================
        // プロパティ：境界線のペン
        //=========================================================================================
        public Pen BorderPen {
            get {
                if (m_borderPen == null) {
                    m_borderPen = new Pen(m_colorTable.MenuBorder);
                }
                return m_borderPen;
            }
        }

        //=========================================================================================
        // プロパティ：セパレータの上部のペン
        //=========================================================================================
        public Pen SeparatorLightPen {
            get {
                if (m_separatorLightPen == null) {
                    m_separatorLightPen = new Pen(m_colorTable.SeparatorLight);
                }
                return m_separatorLightPen;
            }
        }

        //=========================================================================================
        // プロパティ：セパレータの下部のペン
        //=========================================================================================
        public Pen SeparatorDarkPen {
            get {
                if (m_separatorDarkPen == null) {
                    m_separatorDarkPen = new Pen(m_colorTable.SeparatorDark);
                }
                return m_separatorDarkPen;
            }
        }
    }
}
