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

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ファイルビューアメニューのUI
    //=========================================================================================
    public partial class FileViewerMenuStrip : MenuStrip {
        // メニューの実装
        private MenuImpl m_menuImpl;

        // テキストファイルビューア
        private TextFileViewer m_textFileViewer;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileViewerMenuStrip() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]mode   ファイルビューアのモード
        // 　　　　[in]viewer テキストファイルビューア
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(FileViewerForm.ViewerMode mode, TextFileViewer viewer) {
            this.SuspendLayout();
            m_textFileViewer = viewer;

            m_menuImpl = new MenuImpl(UICommandSender.FileViewerMenu, viewer);
            m_menuImpl.MenuItemDropDownEvent += new MenuItemDropDownEventHandler(MenuImpl_MenuItemDropDownEvent);
            m_menuImpl.AddItemsFromSetting(this, this.Items, Program.Document.MenuSetting.FileViewerMenuItemList, Program.Document.KeySetting.FileViewerKeyItemList, true, null);

            this.ResumeLayout(true);
            this.PerformLayout();
        }

        //=========================================================================================
        // 機　能：ドロップダウンメニューが開かれたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void MenuImpl_MenuItemDropDownEvent(object sender, EventArgs evt) {
            ToolStripDropDownItem menuItem = (ToolStripDropDownItem)sender;
            ModifyMenuItem(menuItem.DropDownItems, m_textFileViewer.TextBufferLineInfo);
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
        // 機　能：メニュー項目の内容を調整する
        // 引　数：[in]mode   ファイルビューアのモード
        // 　　　　[in]target コマンドの送信先
        // 戻り値：なし
        //=========================================================================================
        public static void ModifyMenuItem(ToolStripItemCollection itemList, TextBufferLineInfoList lineInfoList) {
            foreach (ToolStripItem item in itemList) {
                if (item is ToolStripMenuItem) {
                    ToolStripMenuItem menuItem = (ToolStripMenuItem)item;
                    MenuImpl.MenuItemTag tag = (MenuImpl.MenuItemTag)(menuItem.Tag);
                    ActionCommandMoniker moniker = tag.ItemSetting.ActionCommandMoniker;
                    if (moniker.CommandType.FullName == typeof(V_SetTabCommand).FullName) {
                        // TAB幅の直接指定
                        int tabWidth = (int)(moniker.Parameter[0]);
                        if (tabWidth == lineInfoList.TabWidth) {
                            menuItem.Checked = true;
                        } else {
                            menuItem.Checked = false;
                        }
                    } else if (moniker.CommandType.FullName == typeof(V_SetTextEncodingCommand).FullName) {
                        // エンコーディングの直接指定
                        string codeParam = (string)(moniker.Parameter[0]);
                        EncodingType encoding = V_SetTextEncodingCommand.CodeParameterToEncoding(codeParam);
                        if (encoding == lineInfoList.TextEncodingType) {
                            menuItem.Checked = true;
                        } else {
                            menuItem.Checked = false;
                        }
                    }
                }
            }
        }
    }
}
