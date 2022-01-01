using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Nomad.Archive.SevenZip;
using Microsoft.COM;
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
    // クラス：7zにより仮想フォルダ内のファイルの展開を行うクラス
    //=========================================================================================
    public class SevenZipVirtualExtract : IArchiveVirtualExtract {
        // アーカイブファイル名（tarのときは作業ディレクトリのアーカイブになる）
        private string m_archiveFileName;

        // パスワードの提供クラス
        private ArchivePasswordSource m_passwordSource;

        // パスワード入力ダイアログの親となるダイアログ
        private Form m_parentDialog;

        // 使用したパスワード（パスワード入力済みでないときはnull）
        private string m_passwordString = null;

        // 現在のパスワードに対応する表示名（パスワードが自動入力でないときはnull）
        private string m_passwordDisplayName = null;

        // 7z.dllのライブラリ（null:開いていない）
        private SevenZipFormat m_sevenZipLib = null;

        // アーカイブインタフェース（null:開いていない）
        private IInArchive m_archive = null;
        
        // アーカイブの読み込み用ストリーム（null:開いていない）
        private InStreamWrapper m_archiveStream = null;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dllPath         DLLのパス名
        // 　　　　[in]arcName         アーカイブファイル名
        // 　　　　[in]passwordSource  パスワードの提供クラス
        // 　　　　[in]parentDialog    パスワード入力ダイアログの親となるダイアログ
        // 戻り値：なし
        //=========================================================================================
        public SevenZipVirtualExtract(string dllPath, string arcName, ArchivePasswordSource passwordSource, Form parentDialog) {
            m_archiveFileName = arcName;
            m_passwordSource = passwordSource;
            m_parentDialog = parentDialog;
            m_sevenZipLib = new SevenZipFormat(dllPath);
        }

        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_archiveStream != null) {
                m_archiveStream.Dispose();
                m_archiveStream = null;
            }
            if (m_archive != null) {
                Marshal.ReleaseComObject(m_archive);
                m_archive = null;
            }
            if (m_sevenZipLib != null) {
                m_sevenZipLib.Dispose();
                m_sevenZipLib = null;
            }
        }
        
        //=========================================================================================
        // 機　能：アーカイブファイルを開く
        // 引　数：なし
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Open() {
            FileOperationStatus status;

            // ファイルの種別を判定
            bool tarArchive;
            KnownSevenZipFormat type;
            bool supportType = SevenZipUtils.GetFormatTypeFromExtension(m_archiveFileName, out type, out tarArchive);
            if (!supportType) {
                return FileOperationStatus.ArcUnknown;
            }
            m_archive = m_sevenZipLib.CreateInArchive(SevenZipUtils.GetClassIdFromKnownFormat(type));
            if (m_archive == null) {
                return FileOperationStatus.ErrorOpen;
            }

            // アーカイブを開く
            try {
                m_archiveStream = new InStreamWrapper(File.OpenRead(m_archiveFileName));
                SevenZipArchiveOpenCallback openCallback = SevenZipArchiveOpenCallback.NewInstance(null);
                ulong checkPos = 128 * 1024;
                if (m_archive.Open(m_archiveStream, ref checkPos, openCallback) != 0) {
                    return FileOperationStatus.ErrorOpen;
                }
            } catch (Exception) {
                return FileOperationStatus.ErrorOpen;
            }

            // パスワードのテスト
            SevenZipPasswordTestImpl passwordTest = new SevenZipPasswordTestImpl();
            if (passwordTest.IsEncrypted(m_archive)) {
                m_archive.Close();
                m_archiveStream.Dispose();
                m_archiveStream = null;
                status = passwordTest.OpenWithPasswordTest(m_archive, ref m_archiveStream, m_archiveFileName, m_passwordSource, m_parentDialog);
                if (!status.Succeeded) {
                    return status;
                }
                m_passwordString = passwordTest.PasswordString;
                m_passwordDisplayName = passwordTest.PasswordDisplayName;
            } else {
                m_passwordString = null;
                m_passwordDisplayName = null;
            }

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：アーカイブ内のファイル一覧をマスター情報に取り込む
        // 引　数：[out]srcMaster   取り込んだ結果を返すマスター情報
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus GetAllContentsForCopySrc(out VirtualFileCopySrcMaster srcMaster) {
            srcMaster = new VirtualFileCopySrcMaster();
            int fileCount = (int)(m_archive.GetNumberOfItems());
            for (int i = 0; i < fileCount; i++) {
                string filePath = SevenZipContentsFileInfo.GetFilePath(m_archive, i);
                bool isDir = SevenZipContentsFileInfo.GetIsFolder(m_archive, i);
                srcMaster.AddLocalPath(filePath, isDir, i);
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：指定されたファイル群を作業領域に展開する
        // 引　数：[in]fileNameList  展開するファイル
        // 　　　　[in]workingRoot   仮想フォルダ作業ディレクトリのルート
        // 　　　　[in]logger        ログ出力インターフェイス
        // 　　　　[in]waitCallback  展開待ちダイアログのコールバック（ダイアログを使っていないときnull）
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus ExtractTemporary(List<VirtualArchiveFileMapping> fileNameList, string workingRoot, ITaskLogger logger, BackgroundWaitCallback waitCallback) {
            FileOperationStatus status;
            VirtualTemporaryExtractMaster master = new VirtualTemporaryExtractMaster(fileNameList);
            int fileCount = (int)(m_archive.GetNumberOfItems());
            for (int i = 0; i < fileCount; i++) {
                string filePath = SevenZipContentsFileInfo.GetFilePath(m_archive, i);
                bool exist = master.CheckContains(filePath);
                if (!exist) {
                    continue;
                }
                string destFilePath = workingRoot + filePath;

                // 進捗を通知
                if (waitCallback != null) {
                    waitCallback.NotifyProgress(master.ExtractCount, master.AllFileCount, filePath);
                }

                // 展開開始
                logger.LogFileOperationStart(FileOperationType.ExtractFile, GenericFileStringUtils.GetFileName(destFilePath), false);
                if (waitCallback != null && waitCallback.RequestContext.CancelReason != CancelReason.None) {
                    status = FileOperationStatus.Canceled;
                } else if (File.Exists(destFilePath)) {
                    status = FileOperationStatus.Skip;
                } else {
                    // ファイル情報を取得
                    SevenZipContentsFileInfo sevenFileInfo = SevenZipContentsFileInfo.NewInstance(m_archive, i, m_passwordString);

                    // 展開を実行
                    uint fileNumber = (uint)(sevenFileInfo.Index);
                    sevenFileInfo.SetTargetInfo(logger.Progress, destFilePath);
                    m_archive.Extract(new uint[] { fileNumber }, 1, 0, sevenFileInfo);
                    status = sevenFileInfo.ResultStatus;
                    if (status.Succeeded && File.Exists(destFilePath)) {
                        FileUtils.SetFileReadonly(destFilePath);
                    }
                }

                logger.LogFileOperationEnd(status);
                if (!status.Succeeded) {
                    FileUtils.ForceDeleteFile(destFilePath);
                    return status;
                }
            }
            return FileOperationStatus.Success;
        }
        
        //=========================================================================================
        // 機　能：指定されたファイル群を展開する
        // 引　数：[in]fileNameList  アーカイブ内のファイル情報を取り込んだ結果
        // 　　　　[in]arcPath       転送元アーカイブ内パス
        // 　　　　[in]destFilePath  転送先ファイルのパス
        // 　　　　[in]attrSet       属性の転送モードのときtrue
        // 　　　　[in]progress      進捗状態を通知するdelegate
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus ExtractCopySrc(VirtualFileCopySrcMaster fileNameList, string arcPath, string destFilePath, bool attrSet, FileProgressEventHandler progress) {
            FileOperationStatus status;

            // ファイル情報を取得
            int index = fileNameList.CheckContainsFile(arcPath);
            if (index == -1) {
                return FileOperationStatus.FileNotFound;
            }
            SevenZipContentsFileInfo sevenFileInfo = SevenZipContentsFileInfo.NewInstance(m_archive, index, m_passwordString);

            // 展開を実行
            uint fileNumber = (uint)(sevenFileInfo.Index);
            sevenFileInfo.SetTargetInfo(progress, destFilePath);
            m_archive.Extract(new uint[] { fileNumber }, 1, 0, sevenFileInfo);
            status = sevenFileInfo.ResultStatus;
            if (!status.Succeeded) {
                try {
                    File.Delete(destFilePath);
                } catch (Exception) {
                }
            } else if (attrSet) {
                try {
                    File.SetCreationTime(destFilePath, sevenFileInfo.LastWriteTime);
                    File.SetLastAccessTime(destFilePath, sevenFileInfo.LastWriteTime);
                } catch (Exception) {
                    return FileOperationStatus.ErrorSetTime;
                }
            }
            return status;
        }

        //=========================================================================================
        // 機　能：アーカイブ内ファイルの情報を返す
        // 引　数：[in]index         アーカイブ内のファイルのインデックス
        // 　　　　[out]filePath     アーカイブ内ファイルパスを返す変数
        // 　　　　[out]updateTime   更新日時を返す変数
        // 　　　　[out]fileSize     ファイルサイズを返す変数
        // 　　　　[out]isDir        フォルダのときtrueを返す変数
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus GetFileInfo(int index, out string filePath, out DateTime updateTime, out long fileSize, out bool isDir) {
            SevenZipContentsFileInfo sevenFileInfo = SevenZipContentsFileInfo.NewInstance(m_archive, index, m_passwordString);
            filePath = sevenFileInfo.FilePath;
            updateTime = sevenFileInfo.LastWriteTime;
            fileSize = sevenFileInfo.FileSize;
            isDir = sevenFileInfo.IsDirectory;
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // プロパティ：アーカイブファイル名（tarのときは作業ディレクトリのアーカイブになる）
        //=========================================================================================
        public string ArchiveFileName {
            get {
                return m_archiveFileName;
            }
        }

        //=========================================================================================
        // プロパティ：現在のパスワードに対応する表示名（パスワードが自動入力でないときはnull）
        //=========================================================================================
        public string UsedPasswordDisplayName {
            get {
                return m_passwordDisplayName;
            }
        }

        //=========================================================================================
        // プロパティ：使用したパスワード（パスワード入力済みでないときはnull）
        //=========================================================================================
        public string UsedPassword {
            get {
                return m_passwordString;
            }
        }
    }
}
