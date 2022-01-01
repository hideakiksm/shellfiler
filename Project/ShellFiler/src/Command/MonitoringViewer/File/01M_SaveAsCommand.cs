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

namespace ShellFiler.Command.MonitoringViewer.File {

    //=========================================================================================
    // クラス：コマンドを実行する
    // モニタリングビューアの表示内容をファイルに保存します。
    //   書式 　 M_SaveAs()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class M_SaveAsCommand : MonitoringViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public M_SaveAsCommand() {
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
            MonitoringResultSaveAsDialog dialog = new MonitoringResultSaveAsDialog();
            DialogResult result = dialog.ShowDialog(MonitoringViewer.MonitoringViewerForm);
            if (result != DialogResult.OK) {
                return null;
            }
            MatrixDataSaver saver = new MatrixDataSaver(MonitoringViewer.MatrixData);
            bool saveSuccess = saver.SaveFile(dialog.SaveFileName, dialog.SaveFormat);
            if (!saveSuccess) {
                InfoBox.Warning(MonitoringViewer.MonitoringViewerForm, "{0}", saver.ErrorMessage);
            }
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.M_SaveAsCommand;
            }
        }
    }
}
