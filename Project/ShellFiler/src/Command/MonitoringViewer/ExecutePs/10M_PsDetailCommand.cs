using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command.MonitoringViewer.Edit;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.MonitoringViewer;
using ShellFiler.MonitoringViewer.ProcessMonitor;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.MonitoringViewer;

namespace ShellFiler.Command.MonitoringViewer.ExecutePs {

    //=========================================================================================
    // クラス：コマンドを実行する
    // psの結果から、選択中のプロセスの詳細を表示します。
    //   書式 　 M_PsDetail()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class M_PsDetailCommand : MonitoringViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public M_PsDetailCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 処理開始条件をチェック
            if (!MonitoringViewer.Available) {
                return null;
            }
            if (MonitoringViewer.Mode != MonitoringViewerMode.PS) {
                return null;
            }

            // 表示
            PsDetailDialog dialog = new PsDetailDialog(MonitoringViewer.MatrixData, MonitoringViewer.MatrixDataView.CursorLine);
            dialog.ShowDialog(MonitoringViewer.MonitoringViewerForm);

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.M_PsDetailCommand;
            }
        }
    }
}
