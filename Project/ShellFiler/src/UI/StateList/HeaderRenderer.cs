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

namespace ShellFiler.UI.StateList {

    //=========================================================================================
    // クラス：ヘッダの描画クラス
    //=========================================================================================
    public class HeaderRenderer {
        private static HeaderColorTable s_colorTable = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]g    グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public HeaderRenderer(Graphics g) {
            if (s_colorTable == null) {
                s_colorTable = new HeaderColorTable(g);
            }
        }

        //=========================================================================================
        // 機　能：ヘッダを描画する
        // 引　数：[in]grp       グラフィックス
        // 　　　　[in]rcRegion  ヘッダの領域
        // 　　　　[in]state     ヘッダの状態
        // 戻り値：なし
        //=========================================================================================
        public void DrawHeader(Graphics grp, Rectangle rcRegion, HeaderState state) {
            Rectangle rcTop = new Rectangle(0, 0, rcRegion.Width, rcRegion.Height / 2);
            Rectangle rcBottom = new Rectangle(0, rcTop.Height, rcRegion.Width, rcRegion.Height - rcTop.Height - 1);

            HeaderRendererGraphics g = new HeaderRendererGraphics(grp, rcTop.Height, rcBottom.Height);
            try {
                if (state == HeaderState.Normal) {
                    g.Graphics.FillRectangle(g.NormalBackTopBrush, rcTop);
                    g.Graphics.FillRectangle(g.NormalBackBottomBrush, rcBottom);
                    Pen pen = g.NormalLinePen;
                    g.Graphics.DrawLine(pen, rcRegion.Left, rcTop.Top, rcRegion.Right, rcTop.Top);
                    g.Graphics.DrawLine(pen, rcRegion.Left, rcBottom.Bottom, rcRegion.Right, rcBottom.Bottom);
                } else if (state == HeaderState.Hot) {
                    g.Graphics.FillRectangle(g.HotBackTopBrush, rcTop);
                    g.Graphics.FillRectangle(g.HotBackBottomBrush, rcBottom);
                    Pen pen = g.HotLinePen;
                    g.Graphics.DrawLine(pen, rcRegion.Left, rcTop.Top, rcRegion.Right, rcTop.Top);
                    g.Graphics.DrawLine(pen, rcRegion.Left, rcBottom.Bottom, rcRegion.Right, rcBottom.Bottom);
                } else {
                    g.Graphics.FillRectangle(g.SelectedBackTopBrush, rcTop);
                    g.Graphics.FillRectangle(g.SelectedBackBottomBrush, rcBottom);
                    Pen pen = g.SelectedLinePen;
                    g.Graphics.DrawLine(pen, rcRegion.Left, rcTop.Top, rcRegion.Right, rcTop.Top);
                    g.Graphics.DrawLine(pen, rcRegion.Left, rcBottom.Bottom, rcRegion.Right, rcBottom.Bottom);
                }
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 列挙子：ヘッダの状態
        //=========================================================================================
        public enum HeaderState {
            Normal,                     // 通常
            Hot,                        // ポイント中
            Select,                     // 選択中
            NormalHeader,               // ヘッダの描画用 通常
            HotHeader,                  // ヘッダの描画用 ポイント中
            SelectHeader,               // ヘッダの描画用 選択中
        }
    }
}
