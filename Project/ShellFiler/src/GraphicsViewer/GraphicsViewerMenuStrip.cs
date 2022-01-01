using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.GraphicsViewer {

    //=========================================================================================
    // クラス：グラフィックビューアメニューのUI
    //=========================================================================================
    public partial class GraphicsViewerMenuStrip : MenuStrip {
        // メニューの実装
        private MenuImpl m_menuImpl;

        // グラフィックビューア
        private GraphicsViewerPanel m_graphicsViewer;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerMenuStrip() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]viewer グラフィックビューア
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(GraphicsViewerPanel viewer) {
            this.SuspendLayout();
            m_graphicsViewer = viewer;

            m_menuImpl = new MenuImpl(UICommandSender.GraphicsViewerMenu, viewer);
            m_menuImpl.AddItemsFromSetting(this, this.Items, Program.Document.MenuSetting.GraphicsViewerMenuItemList, Program.Document.KeySetting.GraphicsViewerKeyItemList, true, null);

            this.ResumeLayout(true);
            this.PerformLayout();
        }
                
        //=========================================================================================
        // 機　能：メニューのステータス状態を更新する
        // 引　数：[in]context   メニューの更新状態のコンテキスト
        // 戻り値：なし
        //=========================================================================================
        public void RefreshMenuStatus(UIItemRefreshContext context) {
            m_menuImpl.RefreshToolbarStatus(context);
        }
    }
}
