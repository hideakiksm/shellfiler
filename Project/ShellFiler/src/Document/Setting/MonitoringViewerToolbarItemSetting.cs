using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.Command.FileList.Window;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.Command.GraphicsViewer;
using ShellFiler.Command.GraphicsViewer.Edit;
using ShellFiler.Command.GraphicsViewer.File;
using ShellFiler.Command.GraphicsViewer.Filter;
using ShellFiler.Command.GraphicsViewer.View;
using ShellFiler.Command.MonitoringViewer;
using ShellFiler.Command.MonitoringViewer.File;
using ShellFiler.Command.MonitoringViewer.Edit;
using ShellFiler.Command.MonitoringViewer.ExecutePs;
using ShellFiler.MonitoringViewer;

namespace ShellFiler.Document.Setting {
    
    //=========================================================================================
    // クラス：ツールバーのカスタマイズ情報（モニタリングビューア）
    //=========================================================================================
    class MonitoringViewerToolbarItemSetting : ToolbarItemSetting {
        // 利用可能になるモード
        private MonitoringViewerMode m_availableMode;

        //=========================================================================================
        // 機　能：コンストラクタ（ボタン）
        // 引　数：[in]moniker       実行するコマンド
        // 　　　　[in]condition     項目を有効にする条件（カスタマイズしないときnull）
        // 　　　　[in]toolHintText  ツールヒントのテキスト（カスタマイズしないときnull）
        // 　　　　[in]mode          利用可能になるモード
        // 戻り値：なし
        //=========================================================================================
        public MonitoringViewerToolbarItemSetting(ActionCommandMoniker moniker, UIEnableCondition condition, string toolHintText, MonitoringViewerMode mode) : base(moniker, condition, toolHintText) {
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
