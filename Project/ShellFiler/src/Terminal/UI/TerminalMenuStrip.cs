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
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.Terminal.UI {

    //=========================================================================================
    // クラス：ターミナルのメニューのUI
    //=========================================================================================
    public partial class TerminalMenuStrip : MenuStrip {
        // メニューの実装
        private MenuImpl m_menuImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TerminalMenuStrip() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]panel  ターミナルのパネル
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(TerminalPanel panel) {
            this.SuspendLayout();

            m_menuImpl = new MenuImpl(UICommandSender.TerminalMenu, panel);
            m_menuImpl.AddItemsFromSetting(this, this.Items, Program.Document.MenuSetting.TerminalMenuItem, Program.Document.KeySetting.TerminalKeyItemList, true, null);

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
