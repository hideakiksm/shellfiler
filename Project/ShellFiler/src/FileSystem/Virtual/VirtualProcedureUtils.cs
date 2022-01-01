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
    // クラス：仮想フォルダ用のユーティリティ
    //=========================================================================================
    class VirtualProcedureUtils {

        //=========================================================================================
        // 機　能：ファイル転送用にアーカイブを開く
        // 引　数：[in]context             コンテキスト情報
        // 　　　　[in]virtualFolder       仮想フォルダの情報
        // 　　　　[out]archiveExtract     開いたアーカイブを返す変数
        // 　　　　[out]fileCopySrcMaster  アーカイブ内のファイル一覧情報を返す変数
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public static FileOperationStatus OpenArchiveForFileCopy(FileOperationRequestContext context, VirtualFolderInfo virtualFolder, out IArchiveVirtualExtract archiveExtract, out VirtualFileCopySrcMaster fileCopySrcMaster) {
            FileOperationStatus status;
            archiveExtract = null;
            fileCopySrcMaster = null;

            VirtualFolderArchiveInfo virtualArchive = virtualFolder.MostInnerArchive;
            string virtualExecRoot = Program.Document.TemporaryManager.VirtualManager.CreateVirtualExecuteFolder(virtualFolder);
            if (virtualExecRoot == null) {
                return FileOperationStatus.VirtualMkDir;
            }

            // アーカイブを開く
            string password = virtualArchive.Password;
            string realArcFile = virtualArchive.RealArchiveFile;
            string clipboardString = OSUtils.GetClipboardString();
            Form parentForm = context.ParentDialog;
            ArchiveAutoPasswordSetting autoPasswordSetting = Program.Document.UserSetting.ArchiveAutoPasswordSetting;
            ArchivePasswordSource passwordSource = new ArchivePasswordSource(autoPasswordSetting, realArcFile, clipboardString, password);
            Form parentDialog = context.ParentDialog;
            archiveExtract = Program.Document.ArchiveFactory.CreateVirtualExtract(realArcFile, passwordSource, parentForm);
            status = archiveExtract.Open();
            if (status.Failed) {
                archiveExtract.Dispose();
                archiveExtract = null;
                return FileOperationStatus.VirtualArcOpen;
            }
            virtualArchive.Password = archiveExtract.UsedPassword;

            // ファイル一覧情報を作成
            status = archiveExtract.GetAllContentsForCopySrc(out fileCopySrcMaster);
            if (!status.Succeeded) {
                archiveExtract.Dispose();
                archiveExtract = null;
                return status;
            }

            return FileOperationStatus.Success;
        }
    }
}
