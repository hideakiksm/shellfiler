using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Nomad.Archive.SevenZip;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zによりファイルの圧縮を行うクラス
    //=========================================================================================
    public class SevenZipCreate : IArchiveCreate {
        // 作成するアーカイブの設定
        private ArchiveParameter m_archiveParameter;

        // アーカイブファイル名（tar.gzとtar.bz2のときはtarファイル名、CloseArchiveで最終アーカイブに置き換わる）
        private string m_archiveFileNameInner;

        // アーカイブファイル名（tar.gzとtar.bz2のときはgz/bz2ファイル名、ない場合はnull）
        private string m_archiveFileNameOuter = null;

        // 7z.dllのライブラリ（null:開いていない）
        private SevenZipFormat m_sevenZipLib = null;

        // アーカイブインタフェース（null:開いていない）
        private IOutArchive m_archive = null;

        // アーカイブの書き込み用ストリーム（null:開いていない）
        private OutStreamWrapper m_archiveStream = null;

        // 1回目の圧縮形式
        private KnownSevenZipFormat m_format1;

        // 2回目の圧縮形式
        private KnownSevenZipFormat m_format2;

        // 圧縮時のコールバック
        private SevenZipArchiveUpdateCallback m_updateCallback;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dllPath           DLLのパス名
        // 　　　　[in]archiveParameter  作成するアーカイブの設定
        // 　　　　[in]arcFileName       作成するアーカイブファイル名
        // 戻り値：作成用インターフェース
        //=========================================================================================
        public SevenZipCreate(string dllPath, ArchiveParameter archiveParameter, string arcFileName) {
            m_archiveParameter = archiveParameter;
            m_archiveFileNameInner = arcFileName;
            m_sevenZipLib = new SevenZipFormat(dllPath);
        }

        //=========================================================================================
        // 機　能：アーカイブを閉じる
        // 引　数：[in]status  直前までの処理の結果
        // 戻り値：ステータス（失敗時はアーカイブ使用不可）
        //=========================================================================================
        public FileOperationStatus CloseArchive(FileOperationStatus status) {
            // インターフェースを閉じる
            if (m_archiveStream != null) {
                m_archiveStream.Dispose();
                m_archiveStream = null;
            }
            if (m_archive != null) {
                Marshal.ReleaseComObject(m_archive);
                m_archive = null;
            }

            // 成功時、tar.gz/tar.bz2の外側の圧縮を行う
            if (status.Succeeded && m_format1 != m_format2) {
                m_archiveFileNameOuter = Program.Document.TemporaryManager.GetTemporaryFile();
                SevenZipTarOuterCreate outer = new SevenZipTarOuterCreate(m_sevenZipLib, m_archiveParameter, m_archiveFileNameInner, m_archiveFileNameOuter, m_format2);
                status = outer.Convert(m_updateCallback.ProgressEventHandler, m_updateCallback.LogLineDetail);
                outer.CloseArchive();
            }

            // ファイルサイズを反映
            long arcFileSize = 0;
            if (status.Succeeded) {
                try {
                    FileInfo fileInfo = new FileInfo(ArchiveFileNameFinal);
                    arcFileSize = fileInfo.Length;
                } catch (Exception) {
                    status = FileOperationStatus.Fail;
                }
            }
            if (m_updateCallback != null) {
                m_updateCallback.Completed(status.Succeeded, arcFileSize);
                m_updateCallback = null;
            }

            // テンポラリを削除
            if (!status.Succeeded) {
                // 失敗：外部と内部の両方を削除
                try {
                    if (File.Exists(m_archiveFileNameInner)) {
                        File.Delete(m_archiveFileNameInner);
                    }
                } catch (Exception) {
                }
                try {
                    if (m_archiveFileNameOuter != null && File.Exists(m_archiveFileNameOuter)) {
                        File.Delete(m_archiveFileNameOuter);
                    }
                } catch (Exception) {
                }
            } else {
                // 成功：内部と外部の両方があるときは、内部を削除
                if (m_archiveFileNameOuter != null) {
                    try {
                        if (File.Exists(m_archiveFileNameInner)) {
                            File.Delete(m_archiveFileNameInner);
                        }
                    } catch (Exception) {
                    }
                }
            }

            // ライブラリを開放
            if (m_sevenZipLib != null) {
                m_sevenZipLib.Dispose();
                m_sevenZipLib = null;
            }
            return status;
        }
        
        //=========================================================================================
        // 機　能：アーカイブファイルを開く
        // 引　数：なし
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus CreateNew() {
            // ファイルの種別を判定
            SevenZipUtils.GetFormatTypeFromArchiveType(m_archiveParameter.ArchiveType, out m_format1, out m_format2);
            m_archive = m_sevenZipLib.CreateOutArchive(SevenZipUtils.GetClassIdFromKnownFormat(m_format1));
            if (m_archive == null) {
                return FileOperationStatus.ErrorOpen;
            }

            // オプションを設定する
            SevenZipCreateSetOption.SetArchiveOption(m_archive, m_archiveParameter);

            // アーカイブを開く
            try {
                m_archiveStream = new OutStreamWrapper(File.Create(m_archiveFileNameInner));
            } catch (Exception) {
                return FileOperationStatus.ErrorOpen;
            }

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：ファイルを更新する
        // 引　数：[in]task      ログ出力先のタスク
        // 　　　　[in]baseDir   圧縮対象のファイルとフォルダのベースディレクトリ
        // 　　　　[in]allList   対象ファイル一覧
        // 　　　　[in]progress  進捗情報表示用のインターフェース
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus UpdateFiles(IBackgroundTask task, string baseDir, List<ArchiveFileDirectoryInfo> allList, FileProgressEventHandler progress) {
            bool zeroFolderSize = (m_format1 == KnownSevenZipFormat.Tar);
            m_updateCallback = new SevenZipArchiveUpdateCallback(m_archiveParameter, task, baseDir, allList, zeroFolderSize, progress);
            try {
                m_archive.UpdateItems(m_archiveStream, allList.Count, m_updateCallback);
            } catch (COMException) {
                if (m_updateCallback.Status == FileOperationStatus.Canceled) {
                    return FileOperationStatus.Canceled;
                } else {
                    return FileOperationStatus.Fail;
                }
            }
            return m_updateCallback.Status;
        }

        //=========================================================================================
        // プロパティ：最終のアーカイブファイル名
        //=========================================================================================
        public string ArchiveFileNameFinal {
            get {
                if (m_archiveFileNameOuter != null) {
                    return m_archiveFileNameOuter;
                } else {
                    return m_archiveFileNameInner;
                }
            }
        }
    }
}
