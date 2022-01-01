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

namespace ShellFiler.UI.Log {

    //=========================================================================================
    // クラス：自動実行タスクに対するログを出力するインターフェイス
    //=========================================================================================
    public interface ITaskLogger {

        //=========================================================================================
        // 機　能：ファイル操作開始時のログを出力する
        // 引　数：[in]operationType  ファイル操作の種類
        // 　　　　[in]filePath       ファイルパス
        // 　　　：[in]isDir          ファイルのときtrue、ディレクトリのときfalse
        // 戻り値：なし
        //=========================================================================================
        void LogFileOperationStart(FileOperationType operationType, string filePath, bool isDir);

        //=========================================================================================
        // 機　能：ファイル操作終了時のログを出力する
        // 引　数：[in]status     出力するステータス
        // 戻り値：渡されたstatus
        //=========================================================================================
        FileOperationStatus LogFileOperationEnd(FileOperationStatus status);

        //=========================================================================================
        // 機　能：ファイル転送の状態を通知する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void ProgressEventHandler(object sender, FileProgressEventArgs evt);

        //=========================================================================================
        // プロパティ：ログ出力した件数
        //=========================================================================================
        int LogCount {
            get;
        }

        //=========================================================================================
        // プロパティ：進捗表示用のハンドラ
        //=========================================================================================
        FileProgressEventHandler Progress {
            get;
        }
    }
}
