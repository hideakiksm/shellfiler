using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.COM;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using Nomad.Archive.SevenZip;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zのアーカイブ内の構成ファイル1件分の情報
    //=========================================================================================
    public class SevenZipContentsFileInfo : IArchiveContentsFileInfo, IProgress, IArchiveExtractCallback {
        // アーカイブ内構成ファイルの0から始まるインデックス
        private readonly int m_fileIndex;
        
        // アーカイブ内構成ファイルとしての名前
        private string m_filePath;
        
        // 転送先ファイルのフルパス
        private string m_destFilePath;

        // ディレクトリのときtrue
        private readonly bool m_isFolder;

        // 更新時刻
        private readonly DateTime m_lastWriteTime;

        // ファイルサイズ（取得できないとき-1）
        private readonly long m_fileSize;

        // キャンセルされたときtrue
        private bool m_canceled = false;

        // ファイルアクセス用のストリーム
        private OutStreamWrapper m_fileStream;

        // 処理結果のステータス
        private FileOperationStatus m_fileOperationStatus = FileOperationStatus.Fail;

        // 展開処理の進捗状況の通知先
        private FileProgressEventHandler m_progress;
        
        // 処理対象の合計サイズ
        private long m_totalFileSize = -1;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]archive     オープン済みのアーカイブ
        // 　　　　[in]fileIndex   アーカイブ内構成ファイルの0から始まるインデックス
        // 　　　　[in]password  パスワード（使用しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public static SevenZipContentsFileInfo NewInstance(IInArchive archive, int fileIndex, string password) {
            if (password == null) {
                return new SevenZipContentsFileInfo(archive, fileIndex);
            } else {
                return new SevenZipContentsFileInfo.WithPassword(archive, fileIndex, password);
            }
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]archive     オープン済みのアーカイブ
        // 　　　　[in]fileIndex   アーカイブ内構成ファイルの0から始まるインデックス
        // 戻り値：なし
        //=========================================================================================
        private SevenZipContentsFileInfo(IInArchive archive, int fileIndex) {
            m_fileIndex = fileIndex;

            // ファイル名
            m_filePath = GetFilePath(archive, fileIndex);

            // フォルダのときtrue
            m_isFolder = GetIsFolder(archive, fileIndex);

            // 更新時刻
            PropVariant varUpdateTime = new PropVariant();
            archive.GetProperty((uint)m_fileIndex, ItemPropId.kpidLastWriteTime, ref varUpdateTime);
            if (varUpdateTime.GetObject() != null) {
                m_lastWriteTime = (DateTime)(varUpdateTime.GetObject());
            } else {
                m_lastWriteTime = DateTime.Now;
            }
            varUpdateTime.Clear();

            // ファイルサイズ
            PropVariant varFileSize = new PropVariant();
            archive.GetProperty((uint)m_fileIndex, ItemPropId.kpidSize, ref varFileSize);
            object objSize = varFileSize.GetObject();
            if (objSize != null) {
#pragma warning disable IDE0038 // パターン マッチングを使用します
                if (objSize is long) {
                    m_fileSize = (long)objSize;
                } else if (objSize is ulong) {
                    m_fileSize = (long)((ulong)objSize);
                } else if (objSize is int) {
                    m_fileSize = (int)objSize;
                } else if (objSize is uint) {
                    m_fileSize = (uint)objSize;
                } else {
                    Program.Abort("アーカイブ中のファイルサイズが想定外の型{0}です。", objSize.GetType().ToString());
                }
#pragma warning restore IDE0038 // パターン マッチングを使用します
            } else {
                m_fileSize = -1;
            }
            varFileSize.Clear();
        }

        //=========================================================================================
        // 機　能：アーカイブから構成ファイルのファイルパスを取得する
        // 引　数：[in]archive     オープン済みのアーカイブ
        // 　　　　[in]fileIndex   アーカイブ内構成ファイルの0から始まるインデックス
        // 戻り値：構成ファイルのファイルパス
        //=========================================================================================
        public static string GetFilePath(IInArchive archive, int fileIndex) {
            PropVariant varFilePath = new PropVariant();
            archive.GetProperty((uint)fileIndex, ItemPropId.kpidPath, ref varFilePath);
            string filePath = (string)varFilePath.GetObject();
            if (filePath != null) {
                if (filePath.StartsWith("/") || filePath.StartsWith("\\")) {
                    filePath = filePath.Substring(1);
                }
                filePath.Replace('/', '\\');
            } else {
                PropVariant varFileName = new PropVariant();
                archive.GetProperty((uint)fileIndex, ItemPropId.kpidName, ref varFileName);
                filePath = (string)varFileName.GetObject();
                varFileName.Clear();
            }
            varFilePath.Clear();
            return filePath;
        }

        //=========================================================================================
        // 機　能：アーカイブから構成ファイルがファイルかフォルダかを返す
        // 引　数：[in]archive     オープン済みのアーカイブ
        // 　　　　[in]fileIndex   アーカイブ内構成ファイルの0から始まるインデックス
        // 戻り値：構成パスがフォルダのときtrue
        //=========================================================================================
        public static bool GetIsFolder(IInArchive archive, int fileIndex) {
            bool isFolder;
            PropVariant varIsFolder = new PropVariant();
            archive.GetProperty((uint)fileIndex, ItemPropId.kpidIsFolder, ref varIsFolder);
            if (varIsFolder.GetObject() != null) {
                isFolder = (bool)varIsFolder.GetObject();
            } else {
                isFolder = false;
            }
            varIsFolder.Clear();
            return isFolder;
        }

        //=========================================================================================
        // 機　能：ファイル単位の後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_fileStream != null) {
                m_fileStream.Dispose();
                m_fileStream = null;
            }
            if (m_fileOperationStatus == FileOperationStatus.Canceled && !m_isFolder) {
                // キャンセルのときは結果通知されずにファイルが残るため
                try {
                    File.Delete(m_destFilePath);
                } catch (Exception) {
                }
            }
        }

        //=========================================================================================
        // 機　能：展開先の情報を設定する
        // 引　数：[in]progress      展開処理の進捗状況の通知先
        // 　　　　[in]destFilePath  転送先ファイルのフルパス
        // 戻り値：なし
        //=========================================================================================
        public void SetTargetInfo(FileProgressEventHandler progress, string destFilePath) {
            m_progress = progress;
            m_destFilePath = destFilePath;
        }

        //=========================================================================================
        // 機　能：合計サイズを設定する
        // 引　数：[in]total  合計サイズ
        // 戻り値：COMエラーコード
        //=========================================================================================
        public int SetTotal(ulong total) {
            m_totalFileSize = (long)(total);
            return 0;
        }

        //=========================================================================================
        // 機　能：処理済みサイズを設定する
        // 引　数：[in,out]completeValue  処理済みサイズ
        // 戻り値：COMエラーコード
        //=========================================================================================
        public int SetCompleted(ref ulong completeValue) {
            if (m_totalFileSize != -1) {
                FileProgressEventArgs evt = new FileProgressEventArgs(m_totalFileSize, (long)completeValue);
                m_progress.SetProgress(this, evt);
                if (evt.Cancel) {
                    m_canceled = true;
                    m_fileOperationStatus = FileOperationStatus.Canceled;
                    return Win32API.E_FAIL;
                }
            }
            return 0;
        }

        //=========================================================================================
        // 機　能：ストリームを取得する
        // 引　数：[in]index          取得するファイル位置
        // 　　　　[out]outStream     ストリームを返す変数
        // 　　　　[in]askExtractMode モード
        // 戻り値：COMエラーコード
        //=========================================================================================
        public int GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode) {
            if ((index != m_fileIndex) || (askExtractMode != AskMode.kExtract)) {
                outStream = null;
                return 0;
            }

            try {
                if (m_isFolder) {
                    // フォルダのとき
                    Directory.CreateDirectory(m_destFilePath);
                    Directory.SetLastWriteTime(m_destFilePath, m_lastWriteTime);
                    m_fileOperationStatus = FileOperationStatus.Success;
                    outStream = null;
                } else {
                    // ファイルのとき
                    string fileDir = Path.GetDirectoryName(m_destFilePath);
                    if (!string.IsNullOrEmpty(fileDir)) {
                        Directory.CreateDirectory(fileDir);
                    }
                    m_fileStream = new OutStreamWrapper(File.Create(m_destFilePath));
                    outStream = m_fileStream;
                    m_fileOperationStatus = FileOperationStatus.Success;
                }
            } catch (Exception) {
                outStream = null;
                m_fileOperationStatus = FileOperationStatus.Fail;
                return Win32API.E_FAIL;
            }
            return 0;
        }

        //=========================================================================================
        // 機　能：ファイル操作を準備する
        // 引　数：[in]askExtractMode モード
        // 戻り値：なし
        //=========================================================================================
        public void PrepareOperation(AskMode askExtractMode) {
        }

        //=========================================================================================
        // 機　能：結果を通知する
        // 引　数：[in]result  結果
        // 戻り値：なし
        //=========================================================================================
        public void SetOperationResult(OperationResult result) {
            try {
                if (m_fileStream != null) {
                    m_fileStream.Dispose();
                    m_fileStream = null;
                }
                if (result == OperationResult.kOK) {
                    if (!m_isFolder) {
                        File.SetLastWriteTime(m_destFilePath, m_lastWriteTime);
                    }
                } else {
                    if (!m_isFolder) {
                        File.Delete(m_destFilePath);
                    }
                }
            } catch (Exception) {
                m_fileOperationStatus = FileOperationStatus.Fail;
            }
            if (m_canceled) {
                m_fileOperationStatus = FileOperationStatus.Canceled;
            } else if (result == OperationResult.kOK) {
                m_fileOperationStatus = FileOperationStatus.Success;
            } else if (result == OperationResult.kCRCError) {
                m_fileOperationStatus = FileOperationStatus.ArcCrcError;
            } else if (result == OperationResult.kDataError) {
                m_fileOperationStatus = FileOperationStatus.ArcDataError;
            } else {
                m_fileOperationStatus = FileOperationStatus.Fail;
            }
        }

        //=========================================================================================
        // プロパティ：アーカイブ内のインデックス
        //=========================================================================================
        public int Index {
            get {
                return m_fileIndex;
            }
        }

        //=========================================================================================
        // プロパティ：アーカイブ内のファイル名（ファイル名が決められないときnull）
        //=========================================================================================
        public string FileName {
            get {
                if (m_filePath == null) {
                    return null;
                } else {
                    return GenericFileStringUtils.GetFileName(m_filePath);
                }
            }
            set {
                if (m_filePath == null) {
                    m_filePath = value;
                } else {
                    if (m_filePath.Contains("\\")) {
                        m_filePath = GenericFileStringUtils.GetDirectoryName(m_filePath) + value;
                    } else {
                        m_filePath = value;
                    }
                }
            }
        }
        
        //=========================================================================================
        // プロパティ：アーカイブ内のフルパスファイル名
        //=========================================================================================
        public string FilePath {
            get {
                return m_filePath;
            }
        }
        
        //=========================================================================================
        // プロパティ：ディレクトリのときtrue
        //=========================================================================================
        public bool IsDirectory {
            get {
                return m_isFolder;
            }
        }

        //=========================================================================================
        // プロパティ：最終更新日時（取得できないときDateTime.MinValue）
        //=========================================================================================
        public DateTime LastWriteTime {
            get {
                return m_lastWriteTime;
            }
        }        

        //=========================================================================================
        // プロパティ：ファイルサイズ（取得できないとき-1）
        //=========================================================================================
        public long FileSize {
            get {
                return m_fileSize;
            }
        }

        //=========================================================================================
        // プロパティ：処理結果のステータス
        //=========================================================================================
        public FileOperationStatus ResultStatus {
            get {
                return m_fileOperationStatus;
            }
        }

        //=========================================================================================
        // クラス：パスワード付きの実装
        //=========================================================================================
        private class WithPassword : SevenZipContentsFileInfo, ICryptoGetTextPassword {
            // パスワード（使用しないときnull）
            private readonly string m_password;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]archive     オープン済みのアーカイブ
            // 　　　　[in]fileIndex   アーカイブ内構成ファイルの0から始まるインデックス
            // 　　　　[in]password  パスワード（使用しないときnull）
            // 戻り値：なし
            //=========================================================================================
            public WithPassword(IInArchive archive, int fileIndex, string password) : base(archive, fileIndex) {
                m_password = password;
            }

            //=========================================================================================
            // 機　能：パスワードを返す
            // 引　数：[in]password   パスワードを返す変数
            // 戻り値：ステータスコード
            //=========================================================================================
            public int CryptoGetTextPassword(out string password) {
                password = m_password;
                return 0;
            }
        }
    }
}
