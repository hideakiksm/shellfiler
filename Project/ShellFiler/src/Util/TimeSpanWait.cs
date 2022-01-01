using System;
using System.Collections.Generic;
using System.Threading;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：前回実行からの時間に応じたスリープを実現するクラス
    //=========================================================================================
    public class TimeSpanWait {
        // 直前の実行時刻
        private DateTime m_lastTime = DateTime.Now;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TimeSpanWait() {
        }

        //=========================================================================================
        // 機　能：前回実行から一定の時間スリープする
        // 引　数：[in]span  待機時間[ms]
        // 戻り値：なし
        //=========================================================================================
        public void Sleep(int span) {
            DateTime current = DateTime.Now;
            int spentTime = (m_lastTime - current).Milliseconds;
            if (span > spentTime) {
                Thread.Sleep(span - spentTime);
            }
            m_lastTime = DateTime.Now;
        }
    }
}
