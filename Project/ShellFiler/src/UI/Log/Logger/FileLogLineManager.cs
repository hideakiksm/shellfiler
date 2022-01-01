using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Management;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Log;

namespace ShellFiler.UI.Log.Logger {

    //=========================================================================================
    // クラス：ファイル操作のログ出力の管理クラス
    //=========================================================================================
    public class FileLogLineManager {
        // ログ出力開始後、進捗表示を開始する時間[ms]
        private const int MIN_PROGRESS_TIME = 300;

        // ログ出力時に残り時間を表示するまでの経過時間[ms]
        private const int MIN_PROGRESS_CALC_REMAINING_TIME = 10 * 1000;

        // 現在処理中のファイルに対するログ内容
        private LogLineFile m_currentLogLineFile;

        // 現在処理中のファイルに対するログ内容の作成時刻
        private DateTime m_currentLogLineFileCreateTime = DateTime.MinValue;

        // ログ出力した件数
        private int m_logCount = 0;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileLogLineManager() {
        }

        //=========================================================================================
        // 機　能：ファイル操作開始時のログを出力する
        // 引　数：[in]operationType  ファイル操作の種類
        // 　　　：[in]filePath       ファイルパス
        // 戻り値：なし
        //=========================================================================================
        public void LogFileOperationStart(FileOperationType operationType, string filePath) {
            m_logCount++;
            m_currentLogLineFile = new LogLineFile(operationType, filePath);
            m_currentLogLineFileCreateTime = DateTime.Now;
            Program.LogWindow.RegistLogLine(m_currentLogLineFile);
        }

        //=========================================================================================
        // 機　能：ファイル操作終了時のログを出力する
        // 引　数：[in]status     出力するステータス
        // 戻り値：渡されたstatus
        //=========================================================================================
        public FileOperationStatus LogFileOperationEnd(FileOperationStatus status) {
            LogFileOperationStateChange(status);
            return status;
        }

        //=========================================================================================
        // 機　能：ファイル操作終了時のログのみを出力する
        // 引　数：[in]status  出力するステータス
        // 戻り値：なし
        //=========================================================================================
        public void LogFileOperationStateChange(FileOperationStatus status) {
            if (m_currentLogLineFile != null) {
                m_currentLogLineFile.Status = status;
                Program.LogWindow.RedrawLogLine(m_currentLogLineFile);
            }
        }

        //=========================================================================================
        // 機　能：ファイル転送の状態を通知する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void ProgressEventHandler(object sender, FileProgressEventArgs evt) {
            if (m_currentLogLineFile != null) {
                double timeSpendMillis = (DateTime.Now - m_currentLogLineFileCreateTime).TotalMilliseconds;
                if (timeSpendMillis > MIN_PROGRESS_TIME) {
                    m_currentLogLineFile.Status = FileOperationStatus.Processing;
                    if (evt.TotalBytesTransferred == 0) {
                        m_currentLogLineFile.Progress = 100;
                    } else {
                        m_currentLogLineFile.Progress = (int)(Math.Min(100, evt.TotalBytesTransferred * 100 / evt.TotalFileSize));
                    }
                    if (timeSpendMillis < MIN_PROGRESS_CALC_REMAINING_TIME || evt.TotalBytesTransferred == 0) {
                        m_currentLogLineFile.RemainingTime = -1;
                    } else {
                        m_currentLogLineFile.RemainingTime = (int)(timeSpendMillis * ((double)(evt.TotalFileSize - evt.TotalBytesTransferred)) / (double)evt.TotalBytesTransferred / 1000.0);
                    }
                    Program.LogWindow.RedrawLogLine(m_currentLogLineFile);
                }
            }
        }

        //=========================================================================================
        // プロパティ：ログ出力した件数
        //=========================================================================================
        public int LogCount {
            get {
                return m_logCount;
            }
        }

        //=========================================================================================
        // プロパティ：ログ操作の種類
        //=========================================================================================
        public FileOperationType OperationType {
            get {
                return m_currentLogLineFile.OperationType;
            }
        }

        //=========================================================================================
        // プロパティ：進捗表示用のハンドラ
        //=========================================================================================
        public FileProgressEventHandler Progress {
            get {
                return new FileProgressEventHandler(ProgressEventHandler);
            }
        }
    }
}
