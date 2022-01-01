using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.Window {

    //=========================================================================================
    // クラス：コマンドを実行する
    // すべてのビューアウィンドウを閉じます。
    //   書式 　 CloseAllViewer()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class CloseAllViewerCommand : FileListActionCommand {
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CloseAllViewerCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            List<Form> viewerList = new List<Form>();
            viewerList.AddRange(Program.WindowManager.FileViewerList);
            foreach (Form form in viewerList) {
                form.Close();
            }
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.CloseAllViewerCommand;
            }
        }
    }
}
