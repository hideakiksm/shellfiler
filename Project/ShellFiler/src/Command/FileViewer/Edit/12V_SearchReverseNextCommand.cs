using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 前回の検索条件を使って、後方方向に検索します。
    //   書式 　 V_SearchReverseNext()
    //   引数  　なし
    //   戻り値　検索を開始したときtrue
    //   対応Ver 0.0.1
    //=========================================================================================
    class V_SearchReverseNextCommand : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_SearchReverseNextCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (!Abailable) {
                return false;
            }
            if (TextFileViewer.TextViewerComponent is TextViewerComponent) {
                TextViewerComponent viewer = (TextViewerComponent)(TextFileViewer.TextViewerComponent);
                TextSearchCondition condition = viewer.SearchCondition;
                if (condition == null) {
                    return false;
                }
                viewer.SearchText(null, false, -1);
            } else if (TextFileViewer.TextViewerComponent is DumpViewerComponent) {
                DumpViewerComponent viewer = (DumpViewerComponent)(TextFileViewer.TextViewerComponent);
                DumpSearchCondition condition = viewer.SearchCondition;
                if (condition == null) {
                    return false;
                }
                viewer.SearchBytes(null, false, -1);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_SearchReverseNextCommand;
            }
        }
    }
}
