using System;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Condition;
using ShellFiler.Util;

namespace ShellFiler.Command.FileList.Internal {

    //=========================================================================================
    // クラス：コマンドを実行する
    // アドレスバーから指定されたフォルダ{0}、フィルター{1}に変更します。
    //   書式 　 AddressBarChdir(string dir, string filter)
    //   引数　　dir:変更先のフォルダ（絶対パス/相対パス/SSHのHOME相対パス）
    // 　　　　　filter:フィルター設定
    //   戻り値　bool:フォルダが変更できたとき/変更が開始できたときはtrue、変更できないときfalseを返します。
    //   対応Ver 1.1.0
    //=========================================================================================
    class AddressBarChdirCommand : FileListActionCommand {
        // 変更先ディレクトリ
        private string m_directory;

        // 変更するフィルター（既存の設定を使うときnull）
        private string m_filter;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AddressBarChdirCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        // メ　モ：[0]:変更するディレクトリ（相対、絶対）
        // 　　　　[1]:変更するフィルター（既存の設定を使うときnull）
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_directory = (string)param[0];
            m_filter = (string)param[1];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (m_filter == null) {
                // フィルター指定継続
                return ChdirCommand.ChangeDirectory(FileListViewTarget, new ChangeDirectoryParam.Direct(m_directory));
            } else {
                // フィルター指定あり
                FileConditionItemDefined filterItem = FileConditionItemDefined.CreateFromWildCard(m_filter, FileConditionTarget.FileOnly);
                if (filterItem == null) {
                    InfoBox.Warning(Program.MainWindow, Resources.DlgCond_BadWildCard);
                    return false;
                }
                FileListFilterMode filter;
                if (FileListViewTarget.FileList.FileListFilterMode != null) {
                    filter = (FileListFilterMode)(FileListViewTarget.FileList.FileListFilterMode.Clone());
                } else {
                    filter = new FileListFilterMode();
                }

                filter.DialogInfo.ConditionMode = false;
                filter.DialogInfo.WildCard = m_filter;
                filter.ConditionList.Clear();
                filter.ConditionList.Add(filterItem);
                return ChdirCommand.ChangeDirectory(FileListViewTarget, new ChangeDirectoryParam.Direct(m_directory, filter));
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ChdirCommand;
            }
        }
    }
}
