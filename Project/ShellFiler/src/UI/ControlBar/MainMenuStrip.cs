using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI.Dialog.KeyOption;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：メインメニューのUI
    //=========================================================================================
    public partial class MainMenuStrip : MenuStrip {
        // メニューの実装
        private MenuImpl m_menuImpl;

        // コマンドの送信元の識別用
        private UICommandSender m_commandSender;

        // コマンドの送信先
        private IUICommandTarget m_commandTarget;

        // コマンドの利用シーン
        private CommandUsingSceneType m_commandScene;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MainMenuStrip() {
            InitializeComponent();
        }
 
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]sender        コマンドの送信元の識別用
        // 　　　　[in]commandTarget コマンドの送信先
        // 　　　　[in]commandScene  コマンドの利用シーン
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(UICommandSender sender, IUICommandTarget commandTarget, CommandUsingSceneType commandScene) {
            m_commandSender = sender;
            m_commandTarget = commandTarget;
            m_commandScene = commandScene;

            this.SuspendLayout();

            this.ImageList = UIIconManager.IconImageList;
            if (commandScene == CommandUsingSceneType.FileList) {
                Program.MainWindow.CursorLRChanged += new MainWindowForm.CursorLRChangedHandler(MainWindowForm_CursorLRChanged);
            }
            KeyItemSettingList keySetting = Program.Document.KeySetting.GetKeyList(commandScene);
            List<MenuItemSetting> menuList = Program.Document.MenuSetting.CreateMenuCustomizedList(commandScene);
            m_menuImpl = new MenuImpl(sender, commandTarget);
            m_menuImpl.AddItemsFromSetting(this, this.Items, menuList, keySetting, true, null);
            m_menuImpl.UpdateItemName();

            this.ResumeLayout(true);
            this.PerformLayout();
        }
 
        //=========================================================================================
        // 機　能：メニュー項目をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetItems() {
            List<MenuItemSetting> menuList = Program.Document.MenuSetting.CreateMenuCustomizedList(m_commandScene);
            KeyItemSettingList keySetting = Program.Document.KeySetting.GetKeyList(m_commandScene);
            m_menuImpl.ClearItemsFromSetting(this, this.Items);
            m_menuImpl.AddItemsFromSetting(this, this.Items, menuList, keySetting, true, null);
            m_menuImpl.UpdateItemName();
        }

        //=========================================================================================
        // 機　能：メニューのステータス状態を更新する
        // 引　数：[in]context   メニューの更新状態のコンテキスト
        // 戻り値：なし
        //=========================================================================================
        public void RefreshMenuStatus(UIItemRefreshContext context) {
            m_menuImpl.RefreshToolbarStatus(context);
        }

        //=========================================================================================
        // 機　能：メニューの項目名を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshItemName() {
            m_menuImpl.UpdateItemName();
        }

        //=========================================================================================
        // 機　能：メインウィンドウのカーソルの左右に変化が生じたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainWindowForm_CursorLRChanged(object sender, EventArgs evt) {
            m_menuImpl.OnCursorLRChanged(Program.Document.CurrentTabPage.IsCursorLeft);
        }
    }
}
