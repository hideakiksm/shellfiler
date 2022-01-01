using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.UI.StateList {

    //=========================================================================================
    // クラス：ヘッダの描画色の管理テーブル
    //=========================================================================================
    public class HeaderColorTable {
        // サンプル描画バッファの幅
        const int CX_BUFFER = 100;

        // サンプル描画バッファの高さ
        const int CY_BUFFER = 20;

        // 通常境界線の色
        private Color m_normalLineColor;

        // 通常背景色（上半分の上）
        private Color m_normalBackColorTop1;

        // 通常背景色（上半分の下）
        private Color m_normalBackColorTop2;

        // 通常背景色（下半分の上）
        private Color m_normalBackColorBottom1;

        // 通常背景色（下半分の下）
        private Color m_normalBackColorBottom2;

        // ポイント中境界線の色
        private Color m_hotLineColor;

        // ポイント中背景色（上半分の上）
        private Color m_hotBackColorTop1;

        // ポイント中背景色（上半分の下）
        private Color m_hotBackColorTop2;

        // ポイント中背景色（下半分の上）
        private Color m_hotackColorBottom1;

        // ポイント中背景色（下半分の下）
        private Color m_hotBackColorBottom2;

        // 選択中境界線の色
        private Color m_selectedLineColor;

        // 選択中背景色（上半分の上）
        private Color m_selectedBackColorTop1;

        // 選択中背景色（上半分の下）
        private Color m_selectedBackColorTop2;

        // 選択中背景色（下半分の上）
        private Color m_selectedBackColorBottom1;

        // 選択中背景色（下半分の下）
        private Color m_selectedBackColorBottom2;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]graphics  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public HeaderColorTable(Graphics g) {
            DoubleBuffer buffer = new DoubleBuffer(g, 100, 20);
            try {
                try {
                    TabRenderer.DrawTabItem(buffer.DrawingGraphics, new Rectangle(0, 0, CX_BUFFER, CY_BUFFER), TabItemState.Normal);
                    GetColorFromImage(buffer.BitmapBuffer, out m_normalLineColor, out m_normalBackColorTop1, out m_normalBackColorTop2, out m_normalBackColorBottom1, out m_normalBackColorBottom2);
                    TabRenderer.DrawTabItem(buffer.DrawingGraphics, new Rectangle(0, 0, CX_BUFFER, CY_BUFFER), TabItemState.Hot);
                    GetColorFromImage(buffer.BitmapBuffer, out m_selectedLineColor, out m_selectedBackColorTop1, out m_selectedBackColorTop2, out m_selectedBackColorBottom1, out m_selectedBackColorBottom2);

                    // 通常色：少しWindowに近く
                    Color windowColor = SystemColors.Window;
                    m_normalLineColor = GraphicsUtils.BrendColor(m_normalLineColor, windowColor);
                    m_normalBackColorTop1 = GraphicsUtils.BrendColor(m_normalBackColorTop1, m_normalBackColorTop1, windowColor);
                    m_normalBackColorTop2 = GraphicsUtils.BrendColor(m_normalBackColorTop2, m_normalBackColorTop2, windowColor);
                    m_normalBackColorBottom1 = GraphicsUtils.BrendColor(m_normalBackColorBottom1, m_normalBackColorBottom1, windowColor);
                    m_normalBackColorBottom2 = GraphicsUtils.BrendColor(m_normalBackColorBottom2, m_normalBackColorBottom2, windowColor);
                } catch (Exception) {
                    // システムによっては取得不可
                    m_normalLineColor = SystemColors.ControlDarkDark;
                    m_normalBackColorTop1 = SystemColors.Control;
                    m_normalBackColorTop2 = SystemColors.Control;
                    m_normalBackColorBottom1 = SystemColors.Control;
                    m_normalBackColorBottom2 = SystemColors.Control;
                    m_selectedLineColor = SystemColors.ControlDarkDark;
                    m_selectedBackColorTop1 = SystemColors.Control;
                    m_selectedBackColorTop2 = SystemColors.Control;
                    m_selectedBackColorBottom1 = SystemColors.Control;
                    m_selectedBackColorBottom2 = SystemColors.Control;
                }

                // ポイント中：通常色と選択中の中間
                m_hotLineColor = GraphicsUtils.BrendColor(m_normalLineColor, m_selectedLineColor);
                m_hotBackColorTop1 = GraphicsUtils.BrendColor(m_normalBackColorTop1, m_selectedBackColorTop1);
                m_hotBackColorTop2 = GraphicsUtils.BrendColor(m_normalBackColorTop2, m_selectedBackColorTop2);
                m_hotackColorBottom1 = GraphicsUtils.BrendColor(m_normalBackColorBottom1, m_selectedBackColorBottom1);
                m_hotBackColorBottom2 = GraphicsUtils.BrendColor(m_normalBackColorBottom2, m_selectedBackColorBottom2);
            } finally {
                buffer.FlushNoUse();
            }
        }

        //=========================================================================================
        // 機　能：色を取得する
        // 引　数：[in]bitmap  色の取得元
        // 　　　　[out]colorBorder  境界線の色を返す変数
        // 　　　　[out]colorTop1    背景色の上半分のグラデーション上の色を返す変数
        // 　　　　[out]colorTop2    背景色の上半分のグラデーション下の色を返す変数
        // 　　　　[out]colorBottom1 背景色の下半分のグラデーション上の色を返す変数
        // 　　　　[out]colorBottom2 背景色の下半分のグラデーション下の色を返す変数
        // 戻り値：なし
        //=========================================================================================
        private void GetColorFromImage(Bitmap bitmap, out Color colorBorder, out Color colorTop1, out Color colorTop2, out Color colorBottom1, out Color colorBottom2) {
            int targetX = CX_BUFFER - 10;
            colorBorder = bitmap.GetPixel(targetX, 0);
            colorTop1 = bitmap.GetPixel(targetX, 2);
            colorTop2 = colorTop1;
            colorBottom2 = bitmap.GetPixel(targetX, CY_BUFFER - 2);
            colorBottom1 = colorBottom2;

            // グラデーションの中間色を探す
            Color prev = colorTop1;
            for (int i = 2; i < CY_BUFFER - 2; i++) {
                Color current = bitmap.GetPixel(targetX, i);
                if (Math.Abs(prev.R - current.R) > 10 || Math.Abs(prev.G - current.G) > 10 || Math.Abs(prev.B - current.B) > 10) {
                    colorTop2 = prev;
                    colorBottom1 = current;
                    break;
                }
                prev = current;
            }
        }

        //=========================================================================================
        // プロパティ：通常境界線の色
        //=========================================================================================
        public Color NormalLineColor {
            get {
                return m_normalLineColor;
            }
        }

        //=========================================================================================
        // プロパティ：通常背景色（上半分の上）
        //=========================================================================================
        public Color NormalBackColorTop1 {
            get {
                return m_normalBackColorTop1;
            }
        }

        //=========================================================================================
        // プロパティ：通常背景色（上半分の下）
        //=========================================================================================
        public Color NormalBackColorTop2 {
            get {
                return m_normalBackColorTop2;
            }
        }
        
        //=========================================================================================
        // プロパティ：通常背景色（下半分の上）
        //=========================================================================================
        public Color NormalBackColorBottom1 {
            get {
                return m_normalBackColorBottom1;
            }
        }

        //=========================================================================================
        // プロパティ：通常背景色（下半分の下）
        //=========================================================================================
        public Color NormalBackColorBottom2 {
            get {
                return m_normalBackColorBottom2;
            }
        }

        //=========================================================================================
        // プロパティ：ポイント中境界線の色
        //=========================================================================================
        public Color HotLineColor {
            get {
                return m_hotLineColor;
            }
        }

        //=========================================================================================
        // プロパティ：ポイント中背景色（上半分の上）
        //=========================================================================================
        public Color HotBackColorTop1 {
            get {
                return m_hotBackColorTop1;
            }
        }

        //=========================================================================================
        // プロパティ：ポイント中背景色（上半分の下）
        //=========================================================================================
        public Color HotBackColorTop2 {
            get {
                return m_hotBackColorTop2;
            }
        }
        
        //=========================================================================================
        // プロパティ：ポイント中背景色（下半分の上）
        //=========================================================================================
        public Color HotBackColorBottom1 {
            get {
                return m_hotackColorBottom1;
            }
        }

        //=========================================================================================
        // プロパティ：ポイント中背景色（下半分の下）
        //=========================================================================================
        public Color HotBackColorBottom2 {
            get {
                return m_hotBackColorBottom2;
            }
        }

        //=========================================================================================
        // プロパティ：選択中境界線の色
        //=========================================================================================
        public Color SelectedLineColor {
            get {
                return m_selectedLineColor;
            }
        }

        //=========================================================================================
        // プロパティ：選択中背景色（上半分の上）
        //=========================================================================================
        public Color SelectedBackColorTop1 {
            get {
                return m_selectedBackColorTop1;
            }
        }

        //=========================================================================================
        // プロパティ：選択中背景色（上半分の下）
        //=========================================================================================
        public Color SelectedBackColorTop2 {
            get {
                return m_selectedBackColorTop2;
            }
        }
        
        //=========================================================================================
        // プロパティ：選択中背景色（下半分の上）
        //=========================================================================================
        public Color SelectedBackColorBottom1 {
            get {
                return m_selectedBackColorBottom1;
            }
        }

        //=========================================================================================
        // プロパティ：選択中背景色（下半分の下）
        //=========================================================================================
        public Color SelectedBackColorBottom2 {
            get {
                return m_selectedBackColorBottom2;
            }
        }
    }
}
