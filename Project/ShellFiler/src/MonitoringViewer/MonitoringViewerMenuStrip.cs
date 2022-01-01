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

namespace ShellFiler.MonitoringViewer {

    //=========================================================================================
    // クラス：モニタリングビューアメニューのUI
    //=========================================================================================
    public partial class MonitoringViewerMenuStrip : MenuStrip {
        // メニューの実装
        private MenuImpl m_menuImpl;

        // テキストファイルビューア
        private IMonitoringViewer m_monitoringFileViewer;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MonitoringViewerMenuStrip() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]viewer モニタリングビューア
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(IMonitoringViewer viewer) {
            this.SuspendLayout();
            m_monitoringFileViewer = viewer;

            MenuImpl.IsValidMenuItemDelegate isValid = delegate(MenuItemSetting item) {
                if (item is MonitoringViewerMenuItemSetting) {
                    MonitoringViewerMenuItemSetting monitoringItem = (MonitoringViewerMenuItemSetting)item;
                    if (viewer.Mode == monitoringItem.AvailableMode) {
                        return true;
                    } else {
                        return false;
                    }
                }
                return true;
            };
            m_menuImpl = new MenuImpl(UICommandSender.MonitoringViewerMenu, viewer.MonitoringViewerForm);
            m_menuImpl.AddItemsFromSetting(this, this.Items, Program.Document.MenuSetting.MonitoringViewerMenuItemList, Program.Document.KeySetting.MonitoringViewerKeyItemList, true, isValid);

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
