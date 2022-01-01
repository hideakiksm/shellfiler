using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.MonitoringViewer {

    //=========================================================================================
    // クラス：Linuxのコマンド実行結果を保持するクラス
    //=========================================================================================
    public class LinuxCommandResult : IRetrieveFileDataTarget {
        // ロード中のタスクのID
        private BackgroundTaskID m_loadingTaskId;

        // 読み込み状況のステータス
        private RetrieveDataLoadStatus m_loadingStatus = RetrieveDataLoadStatus.Loading;
        
        // m_loadingStatus=Failedのときエラー情報の文字列、それ以外はnull
        private string m_errorInfo = null;
        
        // 実行結果を通知するdelegate
        private OnCommandCompletedDelegate m_completedDelegate;

        // 実行結果を通知するdelegate
        public delegate void OnCommandCompletedDelegate(string errorInfo);

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]completed  実行結果を通知するdelegate
        // 戻り値：なし
        //=========================================================================================
        public LinuxCommandResult(OnCommandCompletedDelegate completed) {
            m_completedDelegate = completed;
        }
        
        //=========================================================================================
        // 機　能：新しいデータを追加する
        // 引　数：[in]buffer  データの入ったバッファ
        // 　　　　[in]offset  buffer中のオフセット
        // 　　　　[in]length  データの長さ
        // 戻り値：なし
        // メ　モ：読み込みスレッドまたはその他外部の作業スレッドからの呼び出しを想定
        //=========================================================================================
        public void AddData(byte[] buffer, int offset, int length) {
        }

        //=========================================================================================
        // 機　能：データの追加が終わったことを通知する
        // 引　数：[in]status    読み込み状況のステータス
        // 　　　　[in]errorInfo status=Failedのときエラー情報の文字列、それ以外はnull
        // 戻り値：なし
        // メ　モ：読み込みスレッドまたはその他外部の作業スレッドからの呼び出しを想定
        //=========================================================================================
        public void AddCompleted(RetrieveDataLoadStatus status, string errorInfo) {
            m_loadingStatus = status;
            if (m_errorInfo == null && errorInfo != null) {
                m_errorInfo = errorInfo;
            }
        }
       
        //=========================================================================================
        // 機　能：受信したときのイベントを発行する
        // 引　数：[in]final   最後のイベント通知のときtrue
        // 戻り値：なし
        // メ　モ：読み込みスレッドからの呼び出しを想定
        //=========================================================================================
        public void FireEvent(bool final) {
            if (!final) {
                return;
            }
            BaseThread.InvokeProcedureByMainThread(new OnCommandCompletedDelegate(m_completedDelegate), m_errorInfo);
        }

        //=========================================================================================
        // プロパティ：ロード中のタスクのID
        //=========================================================================================
        public BackgroundTaskID LoadingTaskId {
            get {
                return m_loadingTaskId;
            }
            set {
                m_loadingTaskId = value;
            }
        }
    }
}
