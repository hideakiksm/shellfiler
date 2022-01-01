using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Log;
using ShellFiler.Virtual;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：仮想フォルダのファイルの展開を行うインターフェース
    //=========================================================================================
    public interface IArchiveVirtualExtract {

        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        void Dispose();
        
        //=========================================================================================
        // 機　能：アーカイブファイルを開く
        // 引　数：なし
        // 戻り値：ステータス
        //=========================================================================================
        FileOperationStatus Open();

        //=========================================================================================
        // 機　能：アーカイブ内のファイル一覧をマスター情報に取り込む
        // 引　数：[out]srcMaster   取り込んだ結果を返すマスター情報
        // 戻り値：ステータス
        //=========================================================================================
        FileOperationStatus GetAllContentsForCopySrc(out VirtualFileCopySrcMaster srcMaster);

        //=========================================================================================
        // 機　能：指定されたファイル群を作業領域に展開する
        // 引　数：[in]fileNameList  展開するファイル
        // 　　　　[in]workingRoot   仮想フォルダ作業ディレクトリのルート
        // 　　　　[in]logger        ログ出力インターフェイス
        // 　　　　[in]waitCallback  展開待ちダイアログのコールバック（ダイアログを使っていないときnull）
        // 戻り値：ステータス
        //=========================================================================================
        FileOperationStatus ExtractTemporary(List<VirtualArchiveFileMapping> fileNameList, string workingRoot, ITaskLogger logger, BackgroundWaitCallback waitCallback);

        //=========================================================================================
        // 機　能：指定されたファイル群を展開する
        // 引　数：[in]fileNameList  アーカイブ内のファイル情報を取り込んだ結果
        // 　　　　[in]arcPath       転送元アーカイブ内パス
        // 　　　　[in]destFilePath  転送先ファイルのパス
        // 　　　　[in]attrSet       属性の転送モードのときtrue
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 戻り値：ステータス
        //=========================================================================================
        FileOperationStatus ExtractCopySrc(VirtualFileCopySrcMaster fileNameList, string arcPath, string destFilePath, bool attrSet, FileProgressEventHandler progress);

        //=========================================================================================
        // 機　能：アーカイブ内ファイルの情報を返す
        // 引　数：[in]index         アーカイブ内のファイルのインデックス
        // 　　　　[out]filePath     アーカイブ内ファイルパスを返す変数
        // 　　　　[out]updateTime   更新日時を返す変数
        // 　　　　[out]fileSize     ファイルサイズを返す変数
        // 　　　　[out]isDir        フォルダのときtrueを返す変数
        // 戻り値：ステータス
        //=========================================================================================
        FileOperationStatus GetFileInfo(int index, out string filePath, out DateTime updateTime, out long fileSize, out bool isDir);

        //=========================================================================================
        // プロパティ：アーカイブファイル名
        //=========================================================================================
        string ArchiveFileName {
            get;
        }

        //=========================================================================================
        // プロパティ：現在のパスワードに対応する表示名（パスワードが自動入力でないときはnull）
        //=========================================================================================
        string UsedPasswordDisplayName {
            get;
        }

        //=========================================================================================
        // プロパティ：使用したパスワード（パスワード入力済みでないときはnull）
        //=========================================================================================
        string UsedPassword {
            get;
        }
    }
}
