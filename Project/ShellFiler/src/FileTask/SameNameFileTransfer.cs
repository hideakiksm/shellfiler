using System;
using System.Text.RegularExpressions;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：コピーや移動で発見された同名ファイルを扱うためのクラス
    //=========================================================================================
    public class SameNameFileTransfer {
        // 実行中のバックグラウンドタスク
        private IBackgroundTask m_task;

        // リクエストコンテキスト
        private FileOperationRequestContext m_context;

        // ダウンロードの転送元ファイル一覧
        private IFileProviderSrc m_fileSrc;

        // ダウンロードの転送先ファイル一覧
        private IFileProviderDest m_fileDest;

        // 転送のモード
        private TransferMode m_transferMode;

        // 同名ファイルを発見したときの動作
        private SameFileOperation m_sameFileOperation;

        // ファイル転送しないときのステータス
        private FileOperationStatus m_noTransferStatus;

        // ファイル転送を行うためのdelegate
        private TransferDelegate m_transferDelegate;

        // ファイル転送を行うためのdelegate
        public delegate FileOperationStatus TransferDelegate(FileOperationRequestContext cache, string srcFilePath, string destFilePath, bool overwrite);

        // ファイルの存在確認を行うためのdelegate
        public delegate bool CheckFileExistsDelegate(string fileName, out bool exist);

        //=========================================================================================
        // 機　能：対象パスにディレクトリを作成する
        // 引　数：[in]task     実行中のバックグラウンドタスク
        // 　　　　[in]mode     転送のモード
        // 　　　　[in]sameOpr  同名ファイルを発見したときの動作
        // 　　　　[in]transfer ファイル転送を行うためのdelegate
        // 戻り値：なし
        //=========================================================================================
        public SameNameFileTransfer(IBackgroundTask task, FileOperationRequestContext context, IFileProviderSrc fileSrc, IFileProviderDest fileDest, TransferMode mode, SameFileOperation sameOpr, TransferDelegate transfer) {
            m_task = task;
            m_context = context;
            m_fileSrc = fileSrc;
            m_fileDest = fileDest;
            m_transferMode = mode;
            m_sameFileOperation = sameOpr;
            m_transferDelegate = transfer;
            switch (mode) {
                case TransferMode.CopySameFile:
                    m_noTransferStatus = FileOperationStatus.NoCopy;
                    break;
                case TransferMode.MoveSameFile:
                    m_noTransferStatus = FileOperationStatus.NoMove;
                    break;
                case TransferMode.LinkSameFile:
                    m_noTransferStatus = FileOperationStatus.NoShortcut;
                    break;
                case TransferMode.ExtractSameFile:
                    m_noTransferStatus = FileOperationStatus.NoExtract;
                    break;
            }
        }

        //=========================================================================================
        // 機　能：ファイルをコピーまたは移動する
        // 引　数：[in]cache         キャッシュ情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]overwrite     上書きするときtrue
        // 戻り値：エラーコード
        //=========================================================================================
        private FileOperationStatus TransferFile(FileOperationRequestContext cache, string srcFilePath, string destFilePath, bool overwrite) {
            return m_transferDelegate(cache, srcFilePath, destFilePath, overwrite);
        }

        //=========================================================================================
        // 機　能：同名のファイルが見つかったときの処理を行う
        // 引　数：[in]fileDetail  同名ファイルの詳細情報
        // 戻り値：転送してよいときtrue
        //=========================================================================================
        public FileOperationStatus TransferSameFile(SameNameTargetFileDetail fileDetail) {
            string srcFilePath = fileDetail.SrcFilePath;
            string destFilePath = fileDetail.DestFilePath;
            string srcFileName = m_fileSrc.SrcFileSystem.GetFileName(srcFilePath);
            string destPath = m_fileDest.DestFileSystem.GetDirectoryName(destFilePath);
            string destFileName = m_fileDest.DestFileSystem.GetFileName(destFilePath);
            if (!m_sameFileOperation.AllApply || m_sameFileOperation.SameFileMode == SameFileOperation.SameFileTransferMode.RenameNew) {
                // 前回、すべてのファイルに適用しないとき/名前指定のとき入力
                m_sameFileOperation.NewName = destFileName;
                SameFileOperation operation = OnSameFileFound(m_sameFileOperation, fileDetail);
                if (operation == null) {
                    return FileOperationStatus.Canceled;
                }
                m_sameFileOperation = operation;
            }

            // 転送処理
            FileOperationStatus status = FileOperationStatus.Fail;
            switch (m_sameFileOperation.SameFileMode) {
                case SameFileOperation.SameFileTransferMode.ForceOverwrite:                   // 強制的に上書き
                    status = TransferFile(m_context, srcFilePath, destFilePath, true);
                    break;
                case SameFileOperation.SameFileTransferMode.OverwriteIfNewer:                 // 自分が新しければ上書き
                    status = TransferIfNewer(srcFilePath, destPath, fileDetail);
                    break;
                case SameFileOperation.SameFileTransferMode.RenameNew:                        // 名前を変更して転送
                    status = TransferRename(srcFilePath, destPath, m_sameFileOperation.NewName);
                    break;
                case SameFileOperation.SameFileTransferMode.NotOverwrite:                     // 転送しない
                    status = m_noTransferStatus;
                    break;
                case SameFileOperation.SameFileTransferMode.AutoRename:                       // ファイル名を自動変更して転送
                    status = TransferAutoRename(srcFilePath, destFileName, destPath, m_sameFileOperation.AutoUpdateMode);
                    break;
                case SameFileOperation.SameFileTransferMode.FullAutoTransfer:                 // 状況判断でファイル名を自動的に変更して転送
                    status = TransferFullAuto(srcFilePath, destFileName, destPath, m_sameFileOperation.AutoUpdateMode, fileDetail);
                    break;
                default:
                    Program.Abort("同名ファイルの処理方法が未知です。");
                    break;
            }
            return status;
        }

        //=========================================================================================
        // 機　能：自分が新しければ上書きして転送
        // 引　数：[in]srcFilePath  転送元パス名(C:\SRCBASE\MARKDIR\FILE.txt)
        // 　　　　[in]destPath     転送先パス名(D:\DESTBASE\MARKDIR)
        // 　　　　[in]fileDetail   同名ファイルの詳細情報
        // 戻り値：なし
        //=========================================================================================
        private FileOperationStatus TransferIfNewer(string srcFilePath, string destPath, SameNameTargetFileDetail fileDetail) {
            string srcFileName = m_fileSrc.SrcFileSystem.GetFileName(srcFilePath);
            string destFilePath = m_fileDest.DestFileSystem.CombineFilePath(destPath, srcFileName);

            // 新しければ上書きで転送
            FileOperationStatus status;
            int compDate = BackgroundTaskCommandUtil.CompareFileDate(fileDetail.SrcFileSystemId, fileDetail.DestFileSystemId, fileDetail.SrcLastWriteTime, fileDetail.DestLastWriteTime);
            if (compDate > 0) {
                status = TransferFile(m_context, srcFilePath, destFilePath, true);
            } else {
                status = m_noTransferStatus;
            }
            return status;
        }

        //=========================================================================================
        // 機　能：名前を変更して上書きして転送
        // 引　数：[in]srcPath     転送元パス名(C:\SRCBASE\MARKDIR\FILE.txt)
        // 　　　　[in]destPath    転送先パス名(D:\DESTBASE\MARKDIR)
        // 　　　　[in]newFileName ファイル名（NEWFILE.txt）
        // 戻り値：なし
        //=========================================================================================
        private FileOperationStatus TransferRename(string srcFilePath, string destPath, string newFileName) {
            string destFilePath = m_fileDest.DestFileSystem.CombineFilePath(destPath, newFileName);
            FileOperationStatus status = TransferFile(m_context, srcFilePath, destFilePath, false);
            return status;
        }

        //=========================================================================================
        // 機　能：ファイル名を自動変更して転送
        // 引　数：[in]srcFilePath 転送元パス名(C:\SRCBASE\MARKDIR\FILE.txt)
        // 　　　　[in]fileName    転送しようとしたファイル名（FILE.txt）
        // 　　　　[in]destPath    転送先パス名(D:\DESTBASE\MARKDIR)
        // 　　　　[in]updateMode  自動更新の方法
        // 戻り値：なし
        // メ　モ：圧縮等でテンポラリからの転送を行う場合、転送先基準のファイル名にするためfileNameを渡す
        //=========================================================================================
        private FileOperationStatus TransferAutoRename(string srcFilePath, string fileName, string destPath, SameFileOperation.SameFileAutoUpdateMode updateMode) {
            string destFilePath = CreateUniqueFileName(fileName, destPath, updateMode);
            if (destFilePath == null) {
                return FileOperationStatus.ErrorTooComplex;
            }
            FileOperationStatus status = TransferFile(m_context, srcFilePath, destFilePath, false);
            return status;
        }

        //=========================================================================================
        // 機　能：状況判断でファイル名を自動的に変更して転送
        // 引　数：[in]srcFilePath  転送元パス名(C:\SRCBASE\MARKDIR\FILE.txt)
        // 　　　　[in]fileName     転送しようとしたファイル名（FILE.txt）
        // 　　　　[in]destPath     転送先パス名(D:\DESTBASE\MARKDIR)
        // 　　　　[in]updateMode   自動更新の方法
        // 　　　　[in]fileDetail   同名ファイルの詳細情報
        // 戻り値：なし
        // メ　モ：圧縮等でテンポラリからの転送を行う場合、転送先基準のファイル名にするためfileNameを渡す
        //=========================================================================================
        private FileOperationStatus TransferFullAuto(string srcFilePath, string fileName, string destPath, SameFileOperation.SameFileAutoUpdateMode updateMode, SameNameTargetFileDetail fileDetail) {
            // ファイルサイズが同じときは転送しない
            if (fileDetail.SrcFileSize == fileDetail.DestFileSize) {
                return m_noTransferStatus;
            }

            // ファイルサイズが異なるときは自動リネーム
            return TransferAutoRename(srcFilePath, fileName, destPath, updateMode);
        }

        //=========================================================================================
        // 機　能：指定されたパス内で一意となるファイル名を作成する
        // 引　数：[in]fileName    ファイル名
        // 　　　　[in]destPath    ファイル名の一意性を保証するパス
        // 　　　　[in]updateMode  自動更新の方法
        // 戻り値：一意となるファイルのフルパス名（作成できないときはnull）
        //=========================================================================================
        private string CreateUniqueFileName(string fileName, string destPath, SameFileOperation.SameFileAutoUpdateMode updateMode) {
            // チェック用のdelegateを定義
            CheckFileExistsDelegate checkExist = delegate(string file, out bool exist) {
                string fileTest = m_fileDest.DestFileSystem.CombineFilePath(destPath, file);
                FileOperationStatus status = m_fileDest.DestFileSystem.CheckFileExist(m_context, fileTest, false, null, out exist);
                if (status == FileOperationStatus.Success) {
                    return true;
                } else {
                    return false;
                }
            };

            // 一意のファイル名を作成
            string uniqueFile = CreateUniqueFileName(fileName, updateMode, false, checkExist);
            if (uniqueFile == null) {
                return null;
            }
            string uniquePath = m_fileDest.DestFileSystem.CombineFilePath(destPath, uniqueFile);
            return uniquePath;
        }

        //=========================================================================================
        // 機　能：指定されたdelegateを使ってパス内で一意となるファイル名を作成する
        // 引　数：[in]fileName    ファイル名
        // 　　　　[in]updateMode  自動更新の方法
        // 　　　　[in]selfCheck   指定ファイル自身を有効とするときtrue
        // 　　　　[in]checkExist  重複確認用のdelegate
        // 戻り値：作成したファイル名（null:作成できなかった）
        //=========================================================================================
        public static string CreateUniqueFileName(string fileName, SameFileOperation.SameFileAutoUpdateMode updateMode, bool selfCheck, CheckFileExistsDelegate checkExist) {
            // ファイル名をファイル名主部と拡張子に分離
            string fileBody = null;
            string fileExt = null;
            int idxExt = fileName.LastIndexOf(".");
            if (idxExt == -1) {
                fileBody = fileName;
                fileExt = "";
            } else {
                fileBody = fileName.Substring(0, idxExt);
                fileExt = fileName.Substring(idxExt + 1);
            }

            // 指定ファイル自身をチェック
            if (selfCheck) {
                bool exist;
                if (checkExist(fileName, out exist)) {
                    if (!exist) {
                        return fileName;
                    }
                }
            }

            // 新しいファイル名を作成
            string newFileName = null;
            switch (updateMode) {
                case SameFileOperation.SameFileAutoUpdateMode.AddUnderBar:
                    newFileName = CreteFileUnderbar(checkExist, fileBody, fileExt);
                    break;
                case SameFileOperation.SameFileAutoUpdateMode.AddUnderBarNumber:
                    newFileName = CreteFileParentheses(checkExist, fileBody, fileExt, "_", "");
                    break;
                case SameFileOperation.SameFileAutoUpdateMode.AddParentheses:
                    newFileName = CreteFileParentheses(checkExist, fileBody, fileExt, "(", ")");
                    break;
                case SameFileOperation.SameFileAutoUpdateMode.AddBracket:
                    newFileName = CreteFileParentheses(checkExist, fileBody, fileExt, "[", "]");
                    break;
            }
            return newFileName;
        }

        //=========================================================================================
        // 機　能：転送先の同名ファイル（_付き）を作成してテストする
        // 引　数：[in]checkExist 重複確認用のdelegate
        // 　　　　[in]fileBody   ファイル名本体（FILENAME01_）
        // 　　　　[in]fileExt    ファイル名の拡張子（txt）
        // 戻り値：作成したファイル名（null:作成できなかった）
        //=========================================================================================
        private static string CreteFileUnderbar(CheckFileExistsDelegate checkExist, string fileBody, string fileExt) {
            // ファイル名主部の基本部分を作成
            if (fileBody.Length == 0) {
                return null;
            }
            string fileBodyBase = "";
            for (int i = fileBody.Length - 1; i >= 0; i--) {
                if (fileBody[i] != '_') {
                    fileBodyBase = fileBody.Substring(0, i + 1);
                    break;
                }
            }

            // アンダーバー付きのファイル名をテスト
            for (int i = 1; i <= 99; i++) {
                string fileName = fileBodyBase + StringUtils.Repeat("_", i) + "." + fileExt;
                bool exist;
                bool success = checkExist(fileName, out exist);
                if (!success) {
                    return null;
                } else if (!exist) {
                    return fileName;
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：転送先の同名ファイル（(n)付き）を作成してテストする
        // 引　数：[in]checkExist 重複確認用のdelegate
        // 　　　　[in]fileBody   ファイル名本体（FILENAME01_）
        // 　　　　[in]fileExt    ファイル名の拡張子（txt）
        // 　　　　[in]parStart   開き括弧の文字
        // 　　　　[in]parEnd     閉じ括弧の文字
        // 戻り値：作成したフルパスファイル名（null:作成できなかった）
        //=========================================================================================
        private static string CreteFileParentheses(CheckFileExistsDelegate checkExist, string fileBody, string fileExt, string parStart, string parEnd) {
            // ファイル名主部の基本部分を作成
            if (fileBody.Length == 0) {
                return null;
            }
            string pattern;
            if (parEnd == "") {
                pattern = parStart + "[0-9]+$";
            } else {
                pattern = "\\" + parStart + "[0-9]+\\" + parEnd + "$";
            }
            string fileBodyBase = Regex.Replace(fileBody, pattern, "");

            // 括弧付きのファイル名をテスト
            for (int i = 2; i <= 99; i++) {
                string fileName = fileBodyBase + parStart + i.ToString() + parEnd + "." + fileExt;
                bool exist;
                bool success = checkExist(fileName, out exist);
                if (!success) {
                    return null;
                } else if (!exist) {
                    return fileName;
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：同名のファイルが見つかったときの処理を行う
        // 引　数：[in]operation   同名ファイル発見時の動作のデフォルト
        // 　　　　[in]fileDetail  同名ファイルの詳細情報
        // 戻り値：ダイアログで入力された同名ファイルの操作（null:キャンセル）
        //=========================================================================================
        public SameFileOperation OnSameFileFound(SameFileOperation operation, SameNameTargetFileDetail fileDetail) {
            object result;
            bool success = BaseThread.InvokeFunctionByMainThread(new OnSameFileFoundDelegate(OnSameFileFoundUI), out result, m_task, operation, fileDetail, m_transferMode);
            if (!success) {
                return null;
            }
            return (SameFileOperation)result;
        }
        private delegate SameFileOperation OnSameFileFoundDelegate(AbstractFileBackgroundTask task, SameFileOperation operation, SameNameTargetFileDetail fileDetail, TransferMode mode);
        private static SameFileOperation OnSameFileFoundUI(AbstractFileBackgroundTask task, SameFileOperation operation, SameNameTargetFileDetail fileDetail, TransferMode mode) {
            // 同名ファイルダイアログを表示
            SameNameFileDialog dialog = new SameNameFileDialog();
            dialog.Result = operation;
            dialog.InitializeDialog(task.RequestContext, fileDetail, mode);
            dialog.ShowDialog(Program.MainWindow);
            if (dialog.Result != null) {
                // 入力値を直前の値としてConfigurationに保存
                if (FileSystemID.IsWindows(operation.DestFileSystemId)) {
                    Program.Document.UserGeneralSetting.SameFileOption.AutoUpdateModeWindows = dialog.Result.AutoUpdateMode;
                } else if (FileSystemID.IsSSH(operation.DestFileSystemId)) {
                    Program.Document.UserGeneralSetting.SameFileOption.AutoUpdateModeSSH = dialog.Result.AutoUpdateMode;
                } else {
                    FileSystemID.NotSupportError(operation.DestFileSystemId);
                }
                Program.Document.UserGeneralSetting.SameFileOption.SameFileMode = dialog.Result.SameFileMode;
            }
            return dialog.Result;
        }

        //=========================================================================================
        // プロパティ：同名ファイルの扱い
        //=========================================================================================
        public SameFileOperation SameFileOperation {
            get {
                return m_sameFileOperation;
            }
        }

        //=========================================================================================
        // 列挙子：転送のモード
        //=========================================================================================
        public enum TransferMode {
            CopySameFile,           // コピー中の同名ファイル
            MoveSameFile,           // 移動中の同名ファイル
            LinkSameFile,           // リンク作成中の同名ファイル
            ExtractSameFile,        // 展開中の同名ファイル
        }
    }
}
