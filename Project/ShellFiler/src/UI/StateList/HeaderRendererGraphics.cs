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

namespace ShellFiler.UI.StateList {

    //=========================================================================================
    // クラス：状態一覧パネルの描画用グラフィックス
    //=========================================================================================
    public class HeaderRendererGraphics {
        // グラフィック（null:未初期化）
        private Graphics m_graphics;

        // 描画対象のコントロール（null:グラフィック指定）
        private Control m_control;

        // グラデーション上半分の高さ
        private int m_cyTop;

        // グラデーション下半分の高さ
        private int m_cyBottom;

        // ヘッダの色定義
        private HeaderColorTable m_colorTable;

        // 通常境界線の色
        private Pen m_normalLinePen = null;

        // 通常背景のブラシ（上半分）
        private Brush m_normalBackTopBrush = null;

        // 通常背景のブラシ（下半分）
        private Brush m_normalBackBottomBrush = null;

        // ポイント中境界線の色
        private Pen m_hotLinePen = null;

        // ポイント中背景のブラシ（上半分）
        private Brush m_hotBackTopBrush = null;

        // ポイント中背景のブラシ（下半分）
        private Brush m_hotBackBottomBrush = null;

        // 選択中境界線の色
        private Pen m_selectedLinePen = null;

        // 選択中背景のブラシ（上半分）
        private Brush m_selectedBackTopBrush = null;

        // 選択中背景のブラシ（下半分）
        private Brush m_selectedBackBottomBrush = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics  グラフィックス
        // 　　　　[in]cyTop     グラデーション上半分の高さ
        // 　　　　[in]cyBottom  グラデーション下半分の高さ
        // 戻り値：なし
        //=========================================================================================
        public HeaderRendererGraphics(Graphics graphics, int cyTop, int cyBottom) {
            m_control = null;
            m_graphics = graphics;
            m_cyTop = cyTop;
            m_cyBottom = cyBottom;
            m_colorTable = new HeaderColorTable(m_graphics);
        }

        //=========================================================================================
        // 機　能：コンストラクタ（コントロール指定で必要時に初期化）
        // 引　数：[in]control   描画対象のコントロール
        // 　　　　[in]cyTop     グラデーション上半分の高さ
        // 　　　　[in]cyBottom  グラデーション下半分の高さ
        // 戻り値：なし
        //=========================================================================================
        public HeaderRendererGraphics(Control control, int cyTop, int cyBottom) {
            m_control = control;
            m_graphics = null;
            m_cyTop = cyTop;
            m_cyBottom = cyBottom;
            m_colorTable = new HeaderColorTable(m_graphics);
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
            if (m_normalLinePen != null) {
                m_normalLinePen.Dispose();
                m_normalLinePen = null;
            }
            if (m_normalBackTopBrush != null) {
                m_normalBackTopBrush.Dispose();
                m_normalBackTopBrush = null;
            }
            if (m_normalBackBottomBrush != null) {
                m_normalBackBottomBrush.Dispose();
                m_normalBackBottomBrush = null;
            }
            if (m_hotLinePen != null) {
                m_hotLinePen.Dispose();
                m_hotLinePen = null;
            }
            if (m_hotBackTopBrush != null) {
                m_hotBackTopBrush.Dispose();
                m_hotBackTopBrush = null;
            }
            if (m_hotBackBottomBrush != null) {
                m_hotBackBottomBrush.Dispose();
                m_hotBackBottomBrush = null;
            }
            if (m_selectedLinePen != null) {
                m_selectedLinePen.Dispose();
                m_selectedLinePen = null;
            }
            if (m_selectedBackTopBrush != null) {
                m_selectedBackTopBrush.Dispose();
                m_selectedBackTopBrush = null;
            }
            if (m_selectedBackBottomBrush != null) {
                m_selectedBackBottomBrush.Dispose();
                m_selectedBackBottomBrush = null;
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
        // プロパティ：通常境界線のペン
        //=========================================================================================
        public Pen NormalLinePen {
            get {
                if (m_normalLinePen == null) {
                    m_normalLinePen = new Pen(m_colorTable.NormalLineColor);
                }
                return m_normalLinePen;
            }
        }

        //=========================================================================================
        // プロパティ：通常背景のブラシ（上半分）
        //=========================================================================================
        public Brush NormalBackTopBrush {
            get {
                if (m_normalBackTopBrush == null) {
                    RectangleF rc = new RectangleF(0, 0, 10, m_cyTop);
                    m_normalBackTopBrush = new LinearGradientBrush(rc, m_colorTable.NormalBackColorTop1, m_colorTable.NormalBackColorTop2, LinearGradientMode.Vertical);
                }
                return m_normalBackTopBrush;
            }
        }

        //=========================================================================================
        // プロパティ：通常背景のブラシ（下半分）
        //=========================================================================================
        public Brush NormalBackBottomBrush {
            get {
                if (m_normalBackBottomBrush == null) {
                    RectangleF rc = new RectangleF(0, 0, 10, m_cyBottom);
                    m_normalBackBottomBrush = new LinearGradientBrush(rc, m_colorTable.NormalBackColorBottom1, m_colorTable.NormalBackColorBottom2, LinearGradientMode.Vertical);
                }
                return m_normalBackBottomBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ポイント中境界線のペン
        //=========================================================================================
        public Pen HotLinePen {
            get {
                if (m_hotLinePen == null) {
                    m_hotLinePen = new Pen(m_colorTable.HotLineColor);
                }
                return m_hotLinePen;
            }
        }

        //=========================================================================================
        // プロパティ：ポイント中背景のブラシ（上半分）
        //=========================================================================================
        public Brush HotBackTopBrush {
            get {
                if (m_hotBackTopBrush == null) {
                    RectangleF rc = new RectangleF(0, 0, 10, m_cyTop);
                    m_hotBackTopBrush = new LinearGradientBrush(rc, m_colorTable.HotBackColorTop1, m_colorTable.HotBackColorTop2, LinearGradientMode.Vertical);
                }
                return m_hotBackTopBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ポイント中背景のブラシ（下半分）
        //=========================================================================================
        public Brush HotBackBottomBrush {
            get {
                if (m_hotBackBottomBrush == null) {
                    RectangleF rc = new RectangleF(0, 0, 10, m_cyBottom);
                    m_hotBackBottomBrush = new LinearGradientBrush(rc, m_colorTable.HotBackColorBottom1, m_colorTable.HotBackColorBottom2, LinearGradientMode.Vertical);
                }
                return m_hotBackBottomBrush;
            }
        }

        //=========================================================================================
        // プロパティ：選択中境界線のペン
        //=========================================================================================
        public Pen SelectedLinePen {
            get {
                if (m_selectedLinePen == null) {
                    m_selectedLinePen = new Pen(m_colorTable.SelectedLineColor);
                }
                return m_selectedLinePen;
            }
        }

        //=========================================================================================
        // プロパティ：選択中背景のブラシ（上半分）
        //=========================================================================================
        public Brush SelectedBackTopBrush {
            get {
                if (m_selectedBackTopBrush == null) {
                    RectangleF rc = new RectangleF(0, 0, 10, m_cyTop);
                    m_selectedBackTopBrush = new LinearGradientBrush(rc, m_colorTable.SelectedBackColorTop1, m_colorTable.SelectedBackColorTop2, LinearGradientMode.Vertical);
                }
                return m_selectedBackTopBrush;
            }
        }

        //=========================================================================================
        // プロパティ：選択中背景のブラシ（下半分）
        //=========================================================================================
        public Brush SelectedBackBottomBrush {
            get {
                if (m_selectedBackBottomBrush == null) {
                    RectangleF rc = new RectangleF(0, 0, 10, m_cyBottom);
                    m_selectedBackBottomBrush = new LinearGradientBrush(rc, m_colorTable.SelectedBackColorBottom1, m_colorTable.SelectedBackColorBottom2, LinearGradientMode.Vertical);
                }
                return m_selectedBackBottomBrush;
            }
        }
    }
}
