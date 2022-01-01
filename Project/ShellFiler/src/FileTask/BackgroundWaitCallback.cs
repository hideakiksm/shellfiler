using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Util;
using ShellFiler.UI.Dialog;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：バックグラウンドタスク実行中のコールバック
    //=========================================================================================
    public class BackgroundWaitCallback {
        // 展開状況の更新間隔[ms]
        private const int PROGRESS_UPDATE_SPAN = 200;

        // イベントの通知先
        private IBackgroundWaitNotify m_notifySink;

        // リクエストコンテキスト
        private FileOperationRequestContext m_requestContext;

        // 直前に進捗表示を行った時刻
        private DateTime m_lastNotifyProgress = DateTime.Now;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]notifySink  イベントの通知先
        // 　　　　[in]context     リクエストコンテキスト
        // 戻り値：なし
        //=========================================================================================
        public BackgroundWaitCallback(IBackgroundWaitNotify notifySink, FileOperationRequestContext context) {
            m_notifySink = notifySink;
            m_requestContext = context;
        }

        //=========================================================================================
        // 機　能：進捗表示を行う
        // 引　数：[in]current    現在処理しているファイル数
        // 　　　　[in]all        すべてのファイル数
        // 　　　　[in]filePath   処理中のファイルパス
        // 戻り値：なし
        //=========================================================================================
        public void NotifyProgress(int current, int all, string filePath) {
            DateTime now = DateTime.Now;
            if ((now - m_lastNotifyProgress).TotalMilliseconds > PROGRESS_UPDATE_SPAN) {
                int ratio = current * 100 / all;
                m_notifySink.UpdateProgress(ratio, filePath);
                m_lastNotifyProgress = now;
            }
        }

        //=========================================================================================
        // 機　能：処理が完了したときの処理を行う
        // 引　数：[in]status   最終的な処理ステータス
        // 戻り値：なし
        //=========================================================================================
        public void ExtractCompleted(FileOperationStatus status) {
            if (status.Succeeded) {
                m_notifySink.UpdateProgress(100, "");
            }
            m_notifySink.ExtractCompleted(status);
        }

        //=========================================================================================
        // 機　能：キャンセルする
        // 引　数：[in]reason   キャンセルする理由
        // 戻り値：なし
        //=========================================================================================
        public void Cancel(CancelReason reason) {
            m_requestContext.SetCancel(reason);
        }

        //=========================================================================================
        // プロパティ：リクエストコンテキスト
        //=========================================================================================
        public FileOperationRequestContext RequestContext {
            get {
                return m_requestContext;
            }
        }
    }
}
