using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI;
using ShellFiler.FileTask;

namespace ShellFiler.FileTask.Management {
    
    //=========================================================================================
    // クラス：ファイル操作の表示名のエントリ
    //=========================================================================================
    public class SuspendFlag {
        // 実行状態になっているときシグナルになるイベント
        private ManualResetEvent m_runEvent;

        // suspend状態のときtrue
        private bool m_suspend;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]suspend  Suspend状態にするときtrue
        // 戻り値：なし
        //=========================================================================================
        public SuspendFlag(bool suspend) {
            m_suspend = suspend;
            m_runEvent = new ManualResetEvent(!suspend);
        }

        //=========================================================================================
        // 機　能：Suspend状態のとき待機する
        // 引　数：[in]cancelEvent   キャンセルイベント
        // 戻り値：Resumeしたときtrue、キャンセルしたときfalse
        //=========================================================================================
        public bool WaitWhileSuspend(ManualResetEvent canelEvent) {
            if (m_suspend) {
                // イベントを待つ
                WaitHandle[] waitEventList = { canelEvent, m_runEvent };
                int index = WaitHandle.WaitAny(waitEventList);
                if (index == 0) {                                   // キャンセルイベント
                    return false;
                } else {                                            // Resumeイベント
                    return true;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：Suspend状態を設定する
        // 引　数：[in]suspend  Suspend状態にするときtrue
        // 戻り値：なし
        //=========================================================================================
        public void SetSuspend(bool suspend) {
            m_suspend = suspend;
            if (suspend) {
                m_runEvent.Reset();
            } else {
                m_runEvent.Set();
            }
        }
    }
}
