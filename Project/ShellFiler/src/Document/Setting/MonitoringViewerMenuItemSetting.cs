using System;
using System.Collections.Generic;
using ShellFiler.Api;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileViewer;
using ShellFiler.Properties;
using ShellFiler.MonitoringViewer;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：メニューのカスタマイズ情報（モニタリングビューア）
    //=========================================================================================
    class MonitoringViewerMenuItemSetting : MenuItemSetting {
        // 利用可能になるモード
        private MonitoringViewerMode m_availableMode;

        //=========================================================================================
        // 機　能：コンストラクタ（ポップアップ）
        // 引　数：[in]itemName   メニュー項目の表記
        // 　　　　[in]shortcut   ショートカットキー
        // 　　　　[in]mode       利用可能になるモード
        // 戻り値：なし
        //=========================================================================================
        public MonitoringViewerMenuItemSetting(string itemName, char shortcut, MonitoringViewerMode mode) : base(itemName, shortcut) {
            m_availableMode = mode;
        }

        //=========================================================================================
        // プロパティ：利用可能になるモード
        //=========================================================================================
        public MonitoringViewerMode AvailableMode {
            get {
                return m_availableMode;
            }
        }
    }
}