using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.MonitoringViewer;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.MonitoringViewer;

namespace ShellFiler.Command.MonitoringViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 次の検索結果を表示します。
    //   書式 　 M_SearchForwardNext()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class M_SearchForwardNextCommand : MonitoringViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public M_SearchForwardNextCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            SearchNext(IncrementalSearchOperation.MoveDown, MonitoringViewer);
            return null;
        }

        //=========================================================================================
        // 機　能：次の項目を検索する
        // 引　数：[in]operation    検索の操作
        // 　　　　[in]viewer       MonitoringViewerインターフェイス
        // 戻り値：実行結果
        //=========================================================================================
        public static void SearchNext(IncrementalSearchOperation operation, IMonitoringViewer viewer) {
            if (!viewer.Available) {
                return;
            }
            MatrixData data = viewer.MatrixData;
            if (data.SearchKeyword == null) {
                return;
            }
            bool hit = viewer.MonitoringViewerForm.IncrementalSearchLogic.SearchString(data.SearchKeyword, operation);
            MonitoringViewerStatusBar statusBar = viewer.MonitoringViewerForm.StatusBar;
            if (!hit) {
                string searchStr = data.SearchKeyword.Keyword;
                string message = string.Format(Resources.DlgMonitorSearch_NotHitCanNotMove, searchStr);
                statusBar.ShowErrorMessage(message, FileOperationStatus.LogLevel.Info, IconImageListID.MonitoringViewer_Search);
            } else {
                statusBar.RefreshStatusBar();
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.M_SearchForwardNextCommand;
            }
        }
    }
}
