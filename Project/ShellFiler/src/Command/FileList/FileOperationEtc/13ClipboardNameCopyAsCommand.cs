using System.Collections.Generic;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.FileOperationEtc {

    //=========================================================================================
    // クラス：コマンドを実行する
    // マークされたファイルやフォルダのファイル名を形式を指定してクリップボードにコピーします。
    //   書式 　 ClipboardNameCopyAs()
    //   引数  　なし
    //   戻り値　bool:コピーに成功したときtrue、コピーできなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ClipboardNameCopyAsCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ClipboardNameCopyAsCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // 開始条件の確認
            if (FileListViewTarget.FileList.MarkFiles.Count == 0) {
                return false;
            }

            // 用意
            List<UIFile> markFileList = FileListViewTarget.FileList.MarkFiles;
            List<string> fileNameList = new List<string>();
            foreach (UIFile file in markFileList) {
                fileNameList.Add(file.FileName);
            }

            // 形式を指定
            ClipboardCopyNameAsSetting setting;
            if (Configuration.Current.ClipboardCopyNameAsSettingDefault == null) {
                setting = (ClipboardCopyNameAsSetting)(Program.Document.UserGeneralSetting.ClipboardCopyNameAsSetting).Clone();
            } else {
                setting = (ClipboardCopyNameAsSetting)(Configuration.Current.ClipboardCopyNameAsSettingDefault).Clone();
            }
            ClipboardCopyNameAsDialog dialog = new ClipboardCopyNameAsDialog(setting, FileListViewTarget.FileList.DisplayDirectoryName, fileNameList);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return false;
            }
            Program.Document.UserGeneralSetting.ClipboardCopyNameAsSetting = (ClipboardCopyNameAsSetting)(dialog.Setting.Clone());

            // クリップボードにコピー
            string text = dialog.ClipboardText;
            Clipboard.SetDataObject(text, true);

            ClipboardCopyCommand.FlashMarkFiles(FileListViewTarget, markFileList);

            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ClipboardNameCopyAsCommand;
            }
        }
    }
}
