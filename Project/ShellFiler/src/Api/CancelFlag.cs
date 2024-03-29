﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：キャンセル状態を保持する
    //=========================================================================================
    public class CancelFlag {
        // キャンセルした理由
        private CancelReason m_cancelReason = CancelReason.None;

        // キャンセルイベント
        private ManualResetEvent m_cancelEvent = new ManualResetEvent(false);

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CancelFlag() {
        }

        //=========================================================================================
        // 機　能：キャンセル状態にする
        // 引　数：[in]reason  キャンセルする理由
        // 戻り値：なし
        //=========================================================================================
        public void SetCancel(CancelReason reason) {
            if (m_cancelReason == CancelReason.None) {
                m_cancelReason = reason;
            }
            m_cancelEvent.Set();
        }

        //=========================================================================================
        // プロパティ：キャンセル状態のときtrue
        //=========================================================================================
        public bool IsCancel {
            get {
                return (m_cancelReason != CancelReason.None);
            }
        }
        
        //=========================================================================================
        // プロパティ：キャンセルした理由
        //=========================================================================================
        public CancelReason CancelReason {
            get {
                return m_cancelReason;
            }
        }

        //=========================================================================================
        // プロパティ：キャンセルしたときシグナル状態になるイベント
        //=========================================================================================
        public ManualResetEvent Event {
            get {
                return m_cancelEvent;
            }
        }
    }
}
