using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.DataObject;
using ShellFiler.UI.Log;

namespace ShellFiler.Api {

    //=========================================================================================
    // インターフェース：ファイルシステム間のファイル操作を行うAPI群
    //=========================================================================================
    public interface IFileSystemToFileSystem {

        //=========================================================================================
        // 機　能：ファイルをコピーする
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]srcFileInfoAttr 属性コピーを行うとき、srcFilePathの属性（まだ取得できていないときnull）
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]overwrite       上書きするときtrue
        // 　　　　[in]attrMode        属性をコピーするかどうかの設定（属性をコピーしないときnull）
        // 　　　　[in]fileFilter      転送時に使用するフィルター（使用しないときはnull）
        // 　　　　[in]progress        進捗状態を通知するdelegate
        // 戻り値：ステータス
        //=========================================================================================
        FileOperationStatus CopyFile(FileOperationRequestContext context, string srcFilePath, IFile srcFileInfoAttr, string destFilePath, bool overwrite, AttributeSetMode attrMode, FileFilterTransferSetting fileFilter, FileProgressEventHandler progress);

        //=========================================================================================
        // 機　能：ファイルとディレクトリを移動する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]srcFileInfoAttr 属性コピーを行うとき、srcFilePathの属性（属性をコピーしないときnull）
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]overwrite       上書きするときtrue
        // 　　　　[in]attrMode        属性をコピーするかどうかの設定（属性をコピーしないときnull）
        // 　　　　[in]progress        進捗状態を通知するdelegate
        // 戻り値：ステータス
        //=========================================================================================
        FileOperationStatus MoveFileDirectory(FileOperationRequestContext context, string srcFilePath, IFile srcFileInfoAttr, string destFilePath, bool overwrite, AttributeSetMode attrMode, FileProgressEventHandler progress);

        //=========================================================================================
        // 機　能：ディレクトリの直接コピー／移動をサポートするかどうかを確認する
        // 引　数：[in]srcDirPath   転送元ディレクトリ名のフルパス
        // 　　　　[in]destDirPath  転送先ディレクトリ名のフルパス
        // 　　　　[in]isCopy       コピーのときtrue、移動のときfalse
        // 戻り値：直接の移動ができるときtrue（trueになっても失敗することはある）
        //=========================================================================================
        bool CanMoveDirectoryDirect(string srcDirPath, string destDirPath, bool isCopy);

        //=========================================================================================
        // 機　能：ディレクトリの直接コピーを行う
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]srcDirPath    転送元ディレクトリ名のフルパス
        // 　　　　[in]destDirPath   転送先ディレクトリ名のフルパス
        // 　　　　[in]attrMode      属性をコピーするかどうかの設定（属性をコピーしないときnull）
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 戻り値：ステータス（CopyRetryのとき再試行が必要）
        //=========================================================================================
        FileOperationStatus CopyDirectoryDirect(FileOperationRequestContext context, string srcPath, string destPath, AttributeSetMode attrMode, FileProgressEventHandler progress);

        //=========================================================================================
        // 機　能：ショートカットを作成する
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]overwrite     上書きするときtrue
        // 　　　　[in]type          ショートカットの種類
        // 戻り値：エラーコード
        //=========================================================================================
        FileOperationStatus CreateShortcut(FileOperationRequestContext context, string srcFilePath, string destFilePath, bool overwrite, ShortcutType type);

        //=========================================================================================
        // 機　能：ファイル属性をコピーする
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]isDir         ディレクトリを転送するときtrue
        // 　　　　[in]srcFilePath   転送先のフルパス
        // 　　　　[in]srcFileInfo   転送元のファイル情報（まだ取得できていないときnull）
        // 　　　　[in]destFilePath  転送先のフルパス
        // 　　　　[in]attrMode      設定する属性
        // 戻り値：エラーコード
        //=========================================================================================
        FileOperationStatus CopyFileInfo(FileOperationRequestContext context, bool isDir, string srcFilePath, IFile srcFileInfo, string destFilePath, AttributeSetMode attrMode);

        //=========================================================================================
        // 機　能：ファイルを結合する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destFilePath    転送先ファイル名のフルパス
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        FileOperationStatus CombineFile(FileOperationRequestContext context, List<string> srcFilePath, string destFilePath, ITaskLogger taskLogger);

        //=========================================================================================
        // 機　能：ファイルを分割する
        // 引　数：[in]context         コンテキスト情報
        // 　　　　[in]srcFilePath     転送元ファイル名のフルパス
        // 　　　　[in]destFolderPath  転送先フォルダ名のフルパス（最後は「\」）
        // 　　　　[in]numberingInfo   ファイルの連番の命名規則
        // 　　　　[in]splitInfo       ファイルの分割方法
        // 　　　　[in]taskLogger      ログ出力クラス
        // 戻り値：ステータス
        //=========================================================================================
        FileOperationStatus SplitFile(FileOperationRequestContext context, string srcFilePath, string destFolderPath, RenameNumberingInfo numberingInfo, SplitFileInfo splitInfo, ITaskLogger taskLogger);
    }
}
