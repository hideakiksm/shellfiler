using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Util;
using ShellFiler.UI.FileList;

namespace ShellFiler.Command.FileList.ChangeDir {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 対象パスを指定文字列{0}を含むパスに切り替えます。"jp:us:cn"を指定した場合、C:\\jp\\data\\jp\\res→C:\\jp\\data\\us\\res→C:\\jp\\data\\cn\\res→C:\\jp\\data\\jp\\resのように最後に見つかった部分を切り替えます。
    //   書式 　 ChdirSpecifiedPart(string path)
    //   引数  　path:変更対象のフォルダ名の一部を':'区切りで指定した文字列
    // 　　　　　path-default:jp:us:cn
    // 　　　　　path-range:
    //   戻り値　bool:フォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 2.3.0
    //=========================================================================================
    class ChdirSpecifiedPartCommand : FileListActionCommand {
        // 変更先ディレクトリのリスト（':'区切り）
        private string m_dirPartList;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ChdirSpecifiedPartCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        // メ　モ：[0]:変更対象のフォルダ名の一部を':'区切りで指定した文字列
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_dirPartList = (string)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return ChdirSpecifiedPartCommand.ChangeDirectory(FileListViewTarget, m_dirPartList, true);
        }

        //=========================================================================================
        // 機　能：ディレクトリを変更する
        // 引　数：[in]dirListPart  変更対象のフォルダ名の一部を':'区切りで指定した文字列
        //         [in]order        順序通りのときtrue、逆順のときfalse
        // 戻り値：変更できたときtrue
        //=========================================================================================
        public static bool ChangeDirectory(FileListView targetView, string dirListPart, bool order) {
            string[] splitPart = dirListPart.Split(':');
            string dirName = targetView.FileList.DisplayDirectoryName;
            int depthMax = 0;                   // 見つかった位置の\上
            int depthMaxSplitIndex = -1;
            for (int i = 0; i < splitPart.Length; i++) {
                int indexStartEn = dirName.IndexOf("\\" + splitPart[i] + "\\");
                int indexStartSl = dirName.IndexOf("/" + splitPart[i] + "/");
                if (indexStartEn == -1 && indexStartSl == -1) {
                    continue;
                } else {
                    int indexStart = Math.Max(indexStartEn, indexStartSl);
                    if (indexStart > depthMax) {
                        depthMax = indexStart + 1;
                        depthMaxSplitIndex = i;
                    }
                }
            }
            if (depthMaxSplitIndex == -1) {
                return false;
            }
            string oldStr = splitPart[depthMaxSplitIndex];
            string newStr;
            if (order) {
                newStr = splitPart[(depthMaxSplitIndex + 1) % splitPart.Length];
            } else {
                newStr = splitPart[(depthMaxSplitIndex + splitPart.Length - 1) % splitPart.Length];
            }
            string newDir = dirName.Substring(0, depthMax) + newStr + dirName.Substring(depthMax + oldStr.Length);

            return ChdirCommand.ChangeDirectory(targetView, new ChangeDirectoryParam.Direct(newDir));
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ChdirSpecifiedPartCommand;
            }
        }
    }
}
