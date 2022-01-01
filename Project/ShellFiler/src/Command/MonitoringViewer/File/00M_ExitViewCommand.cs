using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Command.MonitoringViewer.File {

    //=========================================================================================
    // クラス：コマンドを実行する
    // モニタリングビューアを終了します。
    //   書式 　 M_ExitView()
    //   引数  　なし
    //   戻り値　ウィンドウを閉じたときtrue、最大化を戻したときfalse
    //   対応Ver 1.3.0
    //=========================================================================================
    class M_ExitViewCommand : MonitoringViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public M_ExitViewCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (MonitoringViewer.MonitoringViewerForm.TopMost) {
                MonitoringViewer.MonitoringViewerForm.FullScreen(false, false);
                return false;
            } else {
                MonitoringViewer.MonitoringViewerForm.Close();
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.M_ExitViewCommand;
            }
        }
    }
}
