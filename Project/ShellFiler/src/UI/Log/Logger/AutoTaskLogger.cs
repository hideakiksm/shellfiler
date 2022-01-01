using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document.Setting;
using ShellFiler.Util;
using ShellFiler.FileTask.DataObject;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.UI.Log.Logger {

    //=========================================================================================
    // クラス：自動実行タスクに対する必要最小限のログ出力を実行するクラス
    //=========================================================================================
    public class AutoTaskLogger : ITaskLogger {
        // ログ出力の実装
        private FileLogLineManager m_logManager = new FileLogLineManager();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AutoTaskLogger() {
        }

        //=========================================================================================
        // 機　能：ファイル操作開始時のログを出力する
        // 引　数：[in]operationType  ファイル操作の種類
        // 　　　　[in]filePath       ファイルパス
        // 　　　：[in]isDir          ファイルのときtrue、ディレクトリのときfalse
        // 戻り値：なし
        //=========================================================================================
        public void LogFileOperationStart(FileOperationType operationType, string filePath, bool isDir) {
            m_logManager.LogFileOperationStart(operationType, filePath);
        }

        //=========================================================================================
        // 機　能：ファイル操作終了時のログを出力する
        // 引　数：[in]status     出力するステータス
        // 戻り値：渡されたstatus
        //=========================================================================================
        public FileOperationStatus LogFileOperationEnd(FileOperationStatus status) {
            return m_logManager.LogFileOperationEnd(status);
        }

        //=========================================================================================
        // 機　能：ファイル転送の状態を通知する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void ProgressEventHandler(object sender, FileProgressEventArgs evt) {
            m_logManager.ProgressEventHandler(sender, evt);
        }

        //=========================================================================================
        // プロパティ：ログ出力した件数
        //=========================================================================================
        public int LogCount {
            get {
                return m_logManager.LogCount;
            }
        }

        //=========================================================================================
        // プロパティ：進捗表示用のハンドラ
        //=========================================================================================
        public FileProgressEventHandler Progress {
            get {
                return m_logManager.Progress;
            }
        }
    }
}
