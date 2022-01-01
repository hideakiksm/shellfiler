using System;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;
using ShellFiler.Util;

namespace ShellFiler.Command.FileList.ChangeDir {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 指定されたフォルダ{0}に変更します。
    //   書式 　 Chdir(string dir)
    //   引数　　dir:変更先のフォルダ（絶対パス/相対パス/SSHのHOME相対パス）
    // 　　　　　dir-default:..
    // 　　　　　dir-range:
    //   戻り値　bool:フォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ChdirCommand : FileListActionCommand {
        // 変更先ディレクトリ
        private string m_directory;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ChdirCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        // メ　モ：[0]:変更するディレクトリ（相対、絶対）
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_directory = (string)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return ChangeDirectory(FileListViewTarget, new ChangeDirectoryParam.Direct(m_directory));
        }

        //=========================================================================================
        // 機　能：ディレクトリを変更する
        // 引　数：[in]command    変更対象のビュー
        // 　　　　[in]chdirMode  ディレクトリ変更の方法
        // 戻り値：変更に成功したときtrue
        //=========================================================================================
        public static bool ChangeDirectory(FileListView target, ChangeDirectoryParam chdirParam) {
            string dir = chdirParam.TargetDirectory;
            // ドライブ／サーバーの切り替え
            if (dir.EndsWith(":")) {
                dir = GetDriveDefault(target, chdirParam);
                chdirParam.TargetDirectory = dir;
            }
            if (!(chdirParam is ChangeDirectoryParam.Refresh) && dir == null) {
                InfoBox.Information(Program.MainWindow, Resources.Msg_FailedChangeDir);
                return false;
            }

            // パスヒストリを更新
            int fileCount = target.FileList.Files.Count;
            int cursorLineNo = target.FileListViewComponent.CursorLineNo;
            string cursorFileName = "";
            if (fileCount > 0 && cursorLineNo < fileCount) {
                cursorFileName = target.FileList.Files[cursorLineNo].FileName;
                target.FileList.PathHistory.UpdateCurrentDirectory(cursorFileName);
            }
            Program.Document.FolderHistoryWhole.AddItem(target.FileList.DisplayDirectoryName, cursorFileName, target.FileList.FileSystem.FileSystemId);

            // ディレクトリを移動
            ChangeDirectoryStatus success = target.FileList.ChangeDirectory(chdirParam);
            if (success == ChangeDirectoryStatus.Failed) {
                ShowChangeDirErrorMessgage(target, chdirParam, target.FileList.PathHistory);
                return false;
            } else if (success == ChangeDirectoryStatus.Success) {
                target.RefreshViewComponentByViewMode();
                target.FileListViewComponent.OnRefreshDirectory(chdirParam);
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ドライブのデフォルトフォルダを返す
        // 引　数：[in]command    変更対象のビュー
        // 　　　　[in]chdirMode  ディレクトリ変更の方法
        // 戻り値：ドライブのデフォルトフォルダ
        //=========================================================================================
        private static string GetDriveDefault(FileListView target, ChangeDirectoryParam chdirParam) {
            string dir = chdirParam.TargetDirectory;
            string targetDir = target.FileList.DisplayDirectoryName;
            string targetRoot = targetDir.Substring(0, Math.Min(targetDir.Length, dir.Length));
            string oppositeDir = target.OppositeFileListView.FileList.DisplayDirectoryName;
            string oppositeRoot = oppositeDir.Substring(0, Math.Min(oppositeDir.Length, dir.Length));
            if (dir.ToUpper() == targetRoot.ToUpper()) {
                // 対象パスと同じにする
                dir = targetDir;
            } else if (dir.ToUpper() == oppositeRoot.ToUpper()) {
                // 反対パスと同じにする
                dir = oppositeDir;
            } else {
                // パスヒストリの最新から探す
                PathHistory pathHistroy1 = target.FileList.PathHistory;
                PathHistory pathHistroy2 = target.OppositeFileListView.FileList.PathHistory;
                PathHistoryItem item = PathHistory.GetLatestHistoryItem(pathHistroy1, pathHistroy2, dir, true, true);
                if (item == null) {
                    // さらに全体パスヒストリを探す
                    FolderHistoryWhole wholeHistory = Program.Document.FolderHistoryWhole;
                    item = wholeHistory.GetHistoryItem(dir, FileSystemID.IgnoreCaseFolderPath(target.FileList.FileSystem.FileSystemId));
                }
                if (item != null) {
                    dir = item.Directory + "\\";
                } else {
                    // 見つからないときはディレクトリ名を補完してみる
                    dir = Program.Document.FileSystemFactory.CompleteRootDir(dir);
                }
            }
            return dir;
        }

        //=========================================================================================
        // 機　能：ディレクトリの変更に失敗したときのメッセージを表示する
        // 引　数：[in]fileListTarget  ファイル一覧
        // 　　　　[in]chdirParam      ディレクトリ変更のパラメータ
        // 　　　　[in]history         パスヒストリ
        // 戻り値：なし
        //=========================================================================================
        public static void ShowChangeDirErrorMessgage(FileListView fileListTarget, ChangeDirectoryParam chdirParam, PathHistory history) {
            ChdirErrorDialog dialog = new ChdirErrorDialog(chdirParam, history);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return;
            }
            ChangeDirectory(fileListTarget, dialog.NextChangeDirParam);
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
