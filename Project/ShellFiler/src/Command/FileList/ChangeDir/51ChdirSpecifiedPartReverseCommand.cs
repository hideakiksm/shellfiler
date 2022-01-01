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
    // 対象パスを指定文字列{0}を含むパスに逆順で切り替えます。"jp:us:cn"を指定した場合、C:\\jp\\data\\jp\\res→C:\\jp\\data\\cn\\res→C:\\jp\\data\\us\\res→C:\\jp\\data\\jp\\resのように最後に見つかった部分を切り替えます。
    //   書式 　 ChdirSpecifiedPartReverse(string path)
    //   引数  　path:変更対象のフォルダ名の一部を':'区切りで指定した文字列
    // 　　　　　path-default:jp:us:cn
    // 　　　　　path-range:
    //   戻り値　bool:フォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 2.3.0
    //=========================================================================================
    class ChdirSpecifiedPartReverseCommand : FileListActionCommand {
        // 変更先ディレクトリのリスト（':'区切り）
        private string m_dirPartList;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ChdirSpecifiedPartReverseCommand() {
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
            return ChdirSpecifiedPartCommand.ChangeDirectory(FileListViewTarget, m_dirPartList, false);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ChdirSpecifiedPartReverseCommand;
            }
        }
    }
}
