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

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zによりファイルの展開を行うクラス
    //=========================================================================================
    public class SevenZipExtract : IArchiveExtract {
        // アーカイブファイル名（tarのときは作業ディレクトリのアーカイブになる）
        private string m_archiveFileName;

        // パスワードの提供クラス
        private ArchivePasswordSource m_passwordSource;

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

        // tarアーカイブの展開用（null:開いていない）
        private SevenZipTarOuterExtract m_tarExtract = null;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dllPath         DLLのパス名
        // 　　　　[in]arcName         アーカイブファイル名
        // 　　　　[in]passwordSource  パスワードの提供クラス
        // 戻り値：なし
        //=========================================================================================
        public SevenZipExtract(string dllPath, string arcName, ArchivePasswordSource passwordSource) {
            m_archiveFileName = arcName;
            m_passwordSource = passwordSource;
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
        // 引　数：[in]progress  進捗情報表示用のインターフェース
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Open(FileProgressEventHandler progress) {
            // ファイルの種別を判定
            bool tarArchive;
            KnownSevenZipFormat type;
            bool supportType = SevenZipUtils.GetFormatTypeFromExtension(m_archiveFileName, out type, out tarArchive);
            if (!supportType) {
                return FileOperationStatus.ArcUnknown;
            }

            if (tarArchive) {
                m_tarExtract = new SevenZipTarOuterExtract(m_sevenZipLib, m_archiveFileName, null);
                FileOperationStatus status = m_tarExtract.PrepareTarArchive(type, progress);
                if (!status.Succeeded) {
                    return status;
                }
                m_archiveFileName = m_tarExtract.DestFilePath;
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

            // パスワード付きなら一度閉じてテスト
            SevenZipPasswordTestImpl passwordTest = new SevenZipPasswordTestImpl();
            if (passwordTest.IsEncrypted(m_archive)) {
                m_archive.Close();
                m_archiveStream.Dispose();
                m_archiveStream = null;
                FileOperationStatus status = passwordTest.OpenWithPasswordTest(m_archive, ref m_archiveStream, m_archiveFileName, m_passwordSource, Program.MainWindow);
                if (status.Succeeded) {
                    m_passwordString = passwordTest.PasswordString;
                    m_passwordDisplayName = passwordTest.PasswordDisplayName;
                }
                return status;
            } else {
                m_passwordString = null;
                m_passwordDisplayName = null;
                return FileOperationStatus.Success;
            }
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
        // 機　能：アーカイブ内のルートディレクトリがユニークな場合にルートディレクトリ名を返す
        // 引　数：なし
        // 戻り値：ルートディレクトリ名（一意ではない場合null）
        //=========================================================================================
        public string GetRootFileNameIfUnique() {
            string rootFolder = null;
            int count = (int)(m_archive.GetNumberOfItems());
            for (int i = 0; i < count; i++) {
                // ファイル名
                string filePath = SevenZipContentsFileInfo.GetFilePath(m_archive, i);
                if (filePath == null) {
                    return null;
                }
                string[] pathList = filePath.Split('\\');
                string folder = pathList[0];
                if (i == 0) {
                    rootFolder = folder;
                } else {
                    if (rootFolder != folder) {
                        return null;
                    }
                }
            }
            return rootFolder;
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
        // 引　数：[in]fileInfo  展開するファイルの情報
        // 　　　　[in]destPath  対象ディレクトリのパス（最後はセパレータ）
        // 　　　　[in]overwrite 上書きする場合true
        // 　　　　[in]progress  進捗情報表示用のインターフェース
        // 戻り値：ファイル情報
        //=========================================================================================
        public FileOperationStatus Extract(IArchiveContentsFileInfo fileInfo, string destPath, bool overwrite, FileProgressEventHandler progress) {
            SevenZipContentsFileInfo sevenFileInfo = (SevenZipContentsFileInfo)fileInfo;
            string destFilePath = destPath + sevenFileInfo.FilePath;
            
            // 転送先の存在をチェック
            if (!overwrite) {
                bool isExistDir = Directory.Exists(destFilePath);
                bool isExistFile = File.Exists(destFilePath);
                if (fileInfo.IsDirectory) {
                    if (isExistDir) {
                        return FileOperationStatus.Success;
                    } else if (isExistFile) {
                        return FileOperationStatus.Fail;
                    }
                } else {
                    if (isExistDir || isExistFile) {
                        return FileOperationStatus.AlreadyExists;
                    }
                }
            }

            // 展開を実行
            uint fileNumber = (uint)(sevenFileInfo.Index);
            sevenFileInfo.SetTargetInfo(progress, destFilePath);
            m_archive.Extract(new uint[] { fileNumber }, 1, 0, sevenFileInfo);
            FileOperationStatus status = sevenFileInfo.ResultStatus;
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
    }
}
