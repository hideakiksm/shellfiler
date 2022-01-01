using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Windows;
using ShellFiler.Locale;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.Util;
using ShellFiler.Virtual;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：ファイルのコピーのため、アーカイブを展開する処理を行う
    //=========================================================================================
    class VirtualExtractFileProcedure {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public VirtualExtractFileProcedure() {
        }

        //=========================================================================================
        // 機　能：処理を実行する
        // 引　数：[in]context       コンテキスト情報
        // 　　　　[in]srcFilePath   転送元ファイル名のフルパス
        // 　　　　[in]destFilePath  転送先ファイル名のフルパス
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext context, string srcFilePath, string destFilePath, FileProgressEventHandler progress) {
            FileOperationStatus status;

            // 仮想フォルダを用意
            IFileListContext fileListContext = context.SrcFileListContext;
            VirtualFolderInfo virtualInfo = null;
            if (fileListContext is VirtualFileListContext) {
                virtualInfo = ((VirtualFileListContext)fileListContext).VirtualFolderInfo;
            }
            if (virtualInfo == null) {
                Program.Abort("仮想フォルダ情報がセットされていません。");
            }

            // まだの場合はアーカイブを開く
            IArchiveVirtualExtract archiveExtract;
            VirtualFileCopySrcMaster fileCopySrcMaster;
            context.VirtualRequestContext.GetVirtualFileCopySrcContext(out archiveExtract, out fileCopySrcMaster);
            if (archiveExtract == null) {
                status = VirtualProcedureUtils.OpenArchiveForFileCopy(context, virtualInfo, out archiveExtract, out fileCopySrcMaster);
                if (!status.Succeeded) {
                    return status;
                }
                context.VirtualRequestContext.SetVirtualFileCopySrcContext(archiveExtract, fileCopySrcMaster);
            }

            // 展開を実行
            VirtualFolderArchiveInfo virtualArchive = virtualInfo.MostInnerArchive;
            string arcPath = ExtractRuntime.GetArchiveFileInfo(srcFilePath, virtualArchive);
            status = archiveExtract.ExtractCopySrc(fileCopySrcMaster, arcPath, destFilePath, false, progress);
            if (!status.Succeeded) {
                return status;
            }
            return status;
        }
    }
}
