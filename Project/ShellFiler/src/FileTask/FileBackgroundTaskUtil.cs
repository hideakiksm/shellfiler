using ShellFiler.Api;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.FileSystem;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイル操作のバックグラウンドタスク用のユーティリティ
    //=========================================================================================
    public class FileBackgroundTaskUtil {
        
        //=========================================================================================
        // 機　能：ディレクトリの転送関係をチェックする
        // 引　数：[in]fileSystem ファイルシステム
        // 　　　　[in]srcPath    転送元パス名(C:\SRCBASE\MARK)
        // 　　　　[in]destPath   転送先パス名(D:\DESTBASE\MARK)
        // 戻り値：コピーしてよいときtrue
        //=========================================================================================
        public static bool AllowTransfer(IFileSystem fileSystem, string srcPath, string destPath) {
            string src = fileSystem.GetFullPath(srcPath).ToLower();
            string dest = fileSystem.GetFullPath(destPath).ToLower();
            if (src.Length > dest.Length) {
                return true;
            }
            string[] srcList = GenericFileStringUtils.SplitSubDirectoryList(src);
            string[] destList = GenericFileStringUtils.SplitSubDirectoryList(dest);
            if (srcList.Length > destList.Length) {
                return true;
            }
            for (int i = 0; i < srcList.Length; i++) {
                if (srcList[i] != destList[i]) {
                    return true;
                }
            }
            return false;
        }

        //=========================================================================================
        // 機　能：対象パスにディレクトリを作成する
        // 引　数：[in]dirPath   作成するディレクトリのパス
        // 戻り値：ステータス
        //=========================================================================================
        public static FileOperationStatus DestCreateDirectory(FileOperationRequestContext context, IFileSystem fileSystem, string dirPath) {
            string basePath = fileSystem.GetDirectoryName(dirPath);
            string newDir = fileSystem.GetFileName(dirPath);

            FileOperationStatus status = fileSystem.CreateDirectory(context, basePath, newDir, false);
            return status;
        }
    }
}
