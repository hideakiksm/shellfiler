using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Util;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：フォルダサイズの取得を行うProcedure
    //=========================================================================================
    class RetrieveFolderSizeProcedure {
        // コンテキスト情報
        private FileOperationRequestContext m_context;
 
        // 取得条件
        private RetrieveFolderSizeCondition m_condition;

        // 取得結果を返す変数
        private RetrieveFolderSizeResult m_result;

        // 進捗状態を通知するdelegate
        private FileProgressEventHandler m_progress;

        // ベースフォルダの文字列長
        private int m_basePathLength;

        // ファイルサイズの丸めの単位
        private long m_fileSizeUnit;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]context     コンテキスト情報
        // 　　　　[in]condition   取得条件
        // 　　　　[in]result      取得結果を返す変数
        // 　　　　[in]progress    進捗状態を通知するdelegate
        // 戻り値：なし
        //=========================================================================================
        public RetrieveFolderSizeProcedure(FileOperationRequestContext context, RetrieveFolderSizeCondition condition, RetrieveFolderSizeResult result, FileProgressEventHandler progress) {
            m_context = context;
            m_condition = condition;
            m_result = result;
            m_progress = progress;
            m_basePathLength = m_result.FolderBase.Length;
            m_fileSizeUnit = m_condition.FolderSizeUnit;
        }

        //=========================================================================================
        // 機　能：指定したフォルダ以下のファイルサイズ合計を取得する
        // 引　数：[in]directory   対象ディレクトリのルート
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(string directory) {
            if (!FileUtils.DirectoryExists(directory)) {
                return FileOperationStatus.Fail;
            }
            try {
                ClawlFolder(directory, 1, 0.0, 1.0);
                if (m_context.CancelReason != CancelReason.None) {
                    return FileOperationStatus.Canceled;
                }
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：再帰によりファイルサイズ合計を取得する
        // 引　数：[in]directory   対象ディレクトリのルート
        // 　　　　[in]depth       ディレクトリの深さ（1～）
        // 　　　　[in]prgBase     進捗率の処理済みの割合
        // 　　　　[in]prgWhole    このメソッド内の処理をすべて終えたときの進捗率の上昇分
        // 戻り値：ファイルサイズ合計（キャンセルされたとき-1）
        //=========================================================================================
        private long ClawlFolder(string directory, int depth, double prgBase, double prgWhole) {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            // ファイルの一覧
            long folderSize = 0;
            FileSystemInfo[] infoList = directoryInfo.GetFileSystemInfos();
            int folderCount = infoList.Length;
            for (int i = 0; i < infoList.Length; i++) {
                if (infoList[i] is FileInfo) {
                    folderCount--;
                    long fileSize = ((FileInfo)(infoList[i])).Length;
                    infoList[i] = null;
                    fileSize = (fileSize + m_fileSizeUnit - 1) / m_fileSizeUnit * m_fileSizeUnit;
                    folderSize += fileSize;
                }
            }

            // フォルダの一覧
            int folderProgress = 0;
            for (int i = 0; i < infoList.Length; i++) {
                if (infoList[i] != null) {
                    if (infoList[i].Name == "." || infoList[i].Name == "..") {
                        continue;
                    }
                    double currentProgress = prgBase + prgWhole * (double)folderProgress / (double)folderCount;
                    long size = ClawlFolder(directory + "\\" + infoList[i].Name, depth + 1, currentProgress, prgWhole / (double)folderCount);
                    if (size == -1) {
                        return -1;
                    }
                    folderSize += size;
                    folderProgress++;
                    currentProgress = prgBase + prgWhole * (double)folderProgress / (double)folderCount;
                    infoList[i] = null;
                    m_progress.SetProgress(this, new FileProgressEventArgs(10000, (long)(currentProgress * 10000)));
                    if (m_context.CancelReason != CancelReason.None) {
                        return -1;
                    }
                }
            }
            string subPath = directory.Substring(m_basePathLength);
            m_result.AddResult(subPath, depth, folderSize);
            return folderSize;
        }
    }
}
