using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Management;
using ShellFiler.Properties;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;
using ShellFiler.Virtual;

namespace ShellFiler.UI.StateList {

    //=========================================================================================
    // クラス：タスク関連のノードの処理を実装するクラス
    //=========================================================================================
    class TreeViewRenderer {
        // 所有パネル
        private StateListPanel m_parent;

        //=========================================================================================
        // 機　能：コントロールパネル
        // 引　数：[in]panel  所有パネル
        // 戻り値：なし
        //=========================================================================================
        public TreeViewRenderer(StateListPanel parent) {
            m_parent = parent;
        }

        //=========================================================================================
        // 機　能：見出し項目を描画する
        // 引　数：[in]parent   所有パネル
        // 　　　　[in]evt      オーナードローイベント
        // 戻り値：なし
        //=========================================================================================
        public void DrawTitleItem(DrawTreeNodeEventArgs evt) {           
            DoubleBuffer buffer = new DoubleBuffer(evt.Graphics, Math.Max(m_parent.ClientRectangle.Width, m_parent.TreeView.PreferredSize.Width), evt.Bounds.Height);
            try {
                // 準備
                Font font = m_parent.TreeView.Font;
                int xItem = 0;
                int yItem = 0;
                int cxItem = (int)(GraphicsUtils.MeasureString(evt.Graphics, font, evt.Node.Text) + 1);
                int cyItem = evt.Bounds.Height;
                int yString = yItem + (evt.Bounds.Height - font.Height) / 2;
                Rectangle rcItem = new Rectangle(xItem, yItem, cxItem, cyItem);
                Graphics g = buffer.DrawingGraphics;

                // 背景を描画
                HeaderRenderer.HeaderState state;
                if (!Program.MainWindow.IsFileListViewActive && (evt.State & TreeNodeStates.Focused) == TreeNodeStates.Focused) {
                    state = HeaderRenderer.HeaderState.Select;
                } else if (!Program.MainWindow.IsFileListViewActive) {
                    state = HeaderRenderer.HeaderState.Hot;
                } else {
                    state = HeaderRenderer.HeaderState.Normal;
                }
                HeaderRenderer renderer = new HeaderRenderer(evt.Graphics);
                renderer.DrawHeader(g, buffer.DrawingRectangle, state);

                // 項目を描画
                g.DrawString(evt.Node.Text, font, SystemBrushes.ControlText, new Point(xItem, yString));
                if ((evt.State & TreeNodeStates.Focused) == TreeNodeStates.Focused) {
                    Rectangle rcBack = FormUtils.ShrinkRectangle(rcItem, 1);
                    ControlPaint.DrawFocusRectangle(g, rcItem);
                }
            } finally {
                buffer.FlushScreen(evt.Bounds.X, evt.Bounds.Y);
            }
        }

        //=========================================================================================
        // 機　能：項目なしの項目を描画する
        // 引　数：[in]evt   オーナードローイベント
        // 戻り値：なし
        //=========================================================================================
        public void DrawDisableItem(DrawTreeNodeEventArgs evt) {
            DoubleBuffer buffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
            try {
                // 準備
                Font font = m_parent.TreeView.Font;
                int xItem = 20;
                int yItem = 0;
                int cxItem = (int)(GraphicsUtils.MeasureString(evt.Graphics, font, evt.Node.Text) + 1);
                int cyItem = evt.Bounds.Height;
                int yString = yItem + (evt.Bounds.Height - font.Height) / 2;
                Rectangle rcItem = new Rectangle(xItem, yItem, cxItem, cyItem);

                // 描画
                Graphics g = buffer.DrawingGraphics;
                g.FillRectangle(SystemBrushes.Window, buffer.DrawingRectangle);
                if ((evt.State & TreeNodeStates.Focused) == TreeNodeStates.Focused) {
                    Rectangle rcBack = FormUtils.ShrinkRectangle(rcItem, 1);
                    g.FillRectangle(SystemBrushes.MenuHighlight, rcBack);
                    ControlPaint.DrawFocusRectangle(g, rcItem);
                    g.DrawString(evt.Node.Text, font, SystemBrushes.HighlightText, new Point(xItem, yString));
                } else if (!Program.MainWindow.IsFileListViewActive) {
                    g.FillRectangle(SystemBrushes.Window, rcItem);
                    g.DrawString(evt.Node.Text, font, SystemBrushes.ControlText, new Point(xItem, yString));
                } else {
                    g.FillRectangle(SystemBrushes.Window, rcItem);
                    g.DrawString(evt.Node.Text, font, SystemBrushes.GrayText, new Point(xItem, yString));
                }
            } finally {
                buffer.FlushScreen(evt.Bounds.X, evt.Bounds.Y);
            }
        }

        //=========================================================================================
        // 機　能：タスク項目を描画する
        // 引　数：[in]graphics  グラフィックス
        // 　　　　[in]origin    描画開始座標
        // 　　　　[in]node      描画対象のツリーノード
        // 　　　　[in]isFocus   フォーカスを持っているときtrue
        // 戻り値：なし
        //=========================================================================================
        public void DrawTaskItem(Graphics graphics, Point origin, TreeNode node, bool isFocus) {
            Rectangle bounds = node.Bounds;
            int cxBounds = m_parent.TreeView.Width - bounds.X;
            if (cxBounds <= 0) {
                return;
            }
            DoubleBuffer buffer = new DoubleBuffer(graphics, cxBounds, node.Bounds.Height);
            try {
                // 準備
                Font font = m_parent.TreeView.Font;
                FileTaskDisplayInfo nodeDisplayInfo = (FileTaskDisplayInfo)node.Tag;
                string text, hint;
                TaskNodeImpl.CreateTaskNodeText(nodeDisplayInfo, out text, out hint);
                const int CX_ICON_MARGIN = 4;
                int xItem = 20;
                int xItemText = xItem + UIIconManager.CxDefaultIcon + CX_ICON_MARGIN;
                int yItem = 0;
                int cxItem = (int)(GraphicsUtils.MeasureString(graphics, font, text) + 1);
                int cyItem = bounds.Height;
                int yString = yItem + (bounds.Height - font.Height) / 2;
                int yIcon = yItem + (bounds.Height - UIIconManager.CyDefaultIcon) / 2;
                Rectangle rcItemText = new Rectangle(xItemText, yItem, cxItem, cyItem);

                // 背景を描画
                Brush textBrush;
                Graphics g = buffer.DrawingGraphics;
                g.FillRectangle(SystemBrushes.Window, buffer.DrawingRectangle);
                if (isFocus) {
                    Rectangle rcBack = FormUtils.ShrinkRectangle(rcItemText, 1);
                    g.FillRectangle(SystemBrushes.MenuHighlight, rcBack);
                    ControlPaint.DrawFocusRectangle(g, rcItemText);
                    textBrush = SystemBrushes.HighlightText;
                } else {
                    g.FillRectangle(SystemBrushes.Window, rcItemText);
                    textBrush = SystemBrushes.ControlText;
                }

                // テキストを描画
                Image image = UIIconManager.IconImageList.Images[(int)nodeDisplayInfo.BackgroundTaskType.DialogIconId];
                g.DrawImage(image, xItem, yIcon, 16, 16);
                g.DrawString(text, font, textBrush, new Point(xItemText, yString));
                image.Dispose();
            } finally {
                buffer.FlushScreen(origin.X, origin.Y);
            }
        }
    }
}
