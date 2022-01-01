using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.UI.ControlBar;

namespace ShellFiler.Terminal.UI {

    //=========================================================================================
    // クラス：ツールバーの本体
    //=========================================================================================
    public partial class TerminalToolBarStrip : ToolStrip {
        // 対応するパネル
        private TerminalPanel m_terminalPanel;

        // ツールバーの実装
        private ToolBarImpl m_toolbarImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TerminalToolBarStrip() {
            InitializeComponent();
        }
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]panel    対応するパネル
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(TerminalPanel panel) {
            m_terminalPanel = panel;
            
            m_toolbarImpl = new ToolBarImpl(this, UICommandSender.TerminalToolBar, m_terminalPanel);
            List<ToolbarItemSetting> itemSettingList = Program.Document.ToolbarSetting.TerminalToolbarItemList;
            KeyItemSettingList keySetting = Program.Document.KeySetting.TerminalKeyItemList;
            m_toolbarImpl.AddButtonsFromSetting(itemSettingList, keySetting, null);
        }

        //=========================================================================================
        // 機　能：ツールバーのステータス状態を更新する
        // 引　数：[in]context   ツールバーの更新状態のコンテキスト
        // 戻り値：なし
        //=========================================================================================
        public void RefreshToolbarStatus(UIItemRefreshContext context) {
            if (m_toolbarImpl == null) {
                return;
            }
            m_toolbarImpl.RefreshToolbarStatus(context);
        }
    }
}
