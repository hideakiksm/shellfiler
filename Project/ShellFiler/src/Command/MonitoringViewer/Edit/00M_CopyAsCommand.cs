using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.MonitoringViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.MonitoringViewer;

namespace ShellFiler.Command.MonitoringViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 全体をクリップボードにコピーします。
    //   書式 　 M_CopyAllAs()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class M_CopyAllAsCommand : MonitoringViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public M_CopyAllAsCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (!MonitoringViewer.Available) {
                return null;
            }
            MatrixData matrixData = MonitoringViewer.MatrixData;
            Encoding sshEncoding = MonitoringViewer.RetryInfo.Encoding;
            MonitoringResultCopyAsDialog dialog = new MonitoringResultCopyAsDialog(matrixData, sshEncoding);
            dialog.ShowDialog(MonitoringViewer.MonitoringViewerForm);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.M_CopyAllAsCommand;
            }
        }
    }
}
