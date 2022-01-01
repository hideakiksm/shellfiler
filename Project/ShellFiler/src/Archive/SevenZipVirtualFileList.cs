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
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Log;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zにより仮想フォルダ内のファイルの一覧取得を行うクラス
    //=========================================================================================
    public class SevenZipVirtualFileList : IArchiveVirtualFileList {
        // アーカイブファイル名（tarのときは作業ディレクトリのアーカイブになる）
        private string m_archiveFileName;

        // パスワードの提供クラス
        private readonly ArchivePasswordSource m_passwordSource;

        // 現在のアーカイブに使用するパスワード文字列（null:使用しない）
        private string m_passwordString = null;

        // 現在のパスワードに対応する表示名（パスワードが自動入力でないときはnull）
        private string m_passwordDisplayName = null;

        // 7z.dllのライブラリ（null:開いていない）
        private SevenZipFormat m_sevenZipLib = null;

        // アーカイブインタフェース（null:開いていない）
        private IInArchive m_archive = null;
        
        // アーカイブの読み込み用ストリーム（null:開いていない）
        private InStreamWrapper m_archiveStream = null;

        // 作業フォルダ
        private readonly string m_temporaryPath;

        // tarアーカイブの展開用（null:開いていない）
        private SevenZipTarOuterExtract m_tarExtract = null;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dllPath         DLLのパス名
        // 　　　　[in]arcName         アーカイブファイル名
        // 　　　　[in]temporaryPath   作業フォルダ
        // 　　　　[in]passwordSource  パスワードの提供クラス
        // 戻り値：なし
        //=========================================================================================
        public SevenZipVirtualFileList(string dllPath, string arcName, string temporaryPath, ArchivePasswordSource passwordSource) {
            m_archiveFileName = arcName;
            m_passwordSource = passwordSource;
            m_temporaryPath = temporaryPath;
            m_sevenZipLib = new SevenZipFormat(dllPath);
        }

        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_tarExtract != null) {
                m_tarExtract.Dispose();
                m_tarExtract = null;
            }
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
        // 引　数：[in]displayPath       作業対象としてログに表示するパス名
        // 　　　　[out]internalArcName  内部アーカイブのファイル名を返す変数（null以外のときはFile.Delete()が必要）
        // 　　　　[in]logFileList       ログ出力インターフェース
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Open(string displayPath, out string internalArcName, ITaskLogger logger) {
            internalArcName = null;

            // ファイルの種別を判定
            bool tarArchive;
            KnownSevenZipFormat type;
            bool supportType = SevenZipUtils.GetFormatTypeFromExtension(m_archiveFileName, out type, out tarArchive);
            if (!supportType) {
                return FileOperationStatus.ArcUnknown;
            }

            // tar.gzのときは内部を展開する
            if (tarArchive) {
                logger.LogFileOperationStart(FileOperationType.OpenArc, displayPath, false);
                string tarTemp = Program.Document.TemporaryManager.GetTemporaryFileInFolder(m_temporaryPath, "File") +".tar";
                m_tarExtract = new SevenZipTarOuterExtract(m_sevenZipLib, m_archiveFileName, tarTemp);
                FileOperationStatus status = m_tarExtract.PrepareTarArchive(type, logger.Progress);
                logger.LogFileOperationEnd(status);
                if (!status.Succeeded) {
                    return status;
                }
                m_archiveFileName = m_tarExtract.DestFilePath;
                m_tarExtract.DetachInnerFile();
                internalArcName = m_archiveFileName;
                type = KnownSevenZipFormat.Tar;
            }

            m_archive = m_sevenZipLib.CreateInArchive(SevenZipUtils.GetClassIdFromKnownFormat(type));
            if (m_archive == null) {
                return FileOperationStatus.ErrorOpen;
            }

            // アーカイブを開く
            m_passwordString = null;
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



            String a = "";
uint count = m_archive.GetNumberOfItems();
for (uint i = 0; i < count; i++)
{
    PropVariant varName = new PropVariant();
    uint ii = i;
    m_archive.GetProperty(ii, ItemPropId.kpidPath, ref varName);
    varName.Clear();
    string name = (string)varName.GetObject();
    varName.Clear();
    PropVariant varIsFolder = new PropVariant();
    m_archive.GetProperty(i, ItemPropId.kpidIsFolder, ref varIsFolder);
    bool isFolder = (bool)varIsFolder.GetObject();
    varIsFolder.Clear();

    a += i + ": " + name + "," + isFolder + "\r\n";
}






            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：アーカイブ内のファイルとフォルダの数を返す
        // 引　数：なし
        // 戻り値：ファイルとフォルダの数
        //=========================================================================================
        public int GetFileCount() {
            return (int)(m_archive.GetNumberOfItems());
        }

        //=========================================================================================
        // 機　能：ファイル情報を返す
        // 引　数：[in]index  取得するインデックス
        // 戻り値：ファイル情報
        //=========================================================================================
        public IArchiveContentsFileInfo GetFileInfo(int index) {
            return SevenZipContentsFileInfo.NewInstance(m_archive, index, m_passwordString);
        }

        //=========================================================================================
        // 機　能：ファイルを展開する
        // 引　数：[in]index         取得するインデックス
        // 　　　　[in]destFilePath  対象ファイルのパス
        // 　　　　[in]progress      進捗情報表示用のインターフェース
        // 戻り値：ファイル情報
        //=========================================================================================
        public FileOperationStatus Extract(int index, string destFilePath, FileProgressEventHandler progress) {
            FileOperationStatus status;

            // パスワードのテスト
            SevenZipPasswordTestImpl passwordTest = new SevenZipPasswordTestImpl();
            if (passwordTest.IsEncrypted(m_archive)) {
                m_archive.Close();
                m_archiveStream.Dispose();
                m_archiveStream = null;
                status = passwordTest.OpenWithPasswordTest(m_archive, ref m_archiveStream, m_archiveFileName, m_passwordSource, Program.MainWindow);
                if (!status.Succeeded) {
                    return status;
                }
                m_passwordString = passwordTest.PasswordString;
                m_passwordDisplayName = passwordTest.PasswordDisplayName;
            } else {
                m_passwordString = null;
                m_passwordDisplayName = null;
            }
            
            // ファイル情報を取得
            SevenZipContentsFileInfo sevenFileInfo = (SevenZipContentsFileInfo)GetFileInfo(index);

            // 展開を実行
            uint fileNumber = (uint)(sevenFileInfo.Index);
            sevenFileInfo.SetTargetInfo(progress, destFilePath);
            m_archive.Extract(new uint[] { fileNumber }, 1, 0, sevenFileInfo);
            status = sevenFileInfo.ResultStatus;
            return status;
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
