using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：日時情報
    //=========================================================================================
    public class DateTimeInfo {
        // 現在の値
        private DateTime m_dateTime;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]initial   初期値
        // 戻り値：なし
        //=========================================================================================
        public DateTimeInfo(DateTime initial) {
            m_dateTime = initial;
        }
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dateInfo  日付情報

        // 戻り値：なし
        //=========================================================================================
        public DateTimeInfo(DateInfo dateInfo, TimeInfo timeInfo) {
            m_dateTime = new DateTime(dateInfo.Year, dateInfo.Month, dateInfo.Day, timeInfo.Hour, timeInfo.Minute, timeInfo.Second);
        }

        //=========================================================================================
        // 機　能：日付部分を設定する
        // 引　数：[in]date  設定する日付（nullのとき処理しない）
        // 戻り値：なし
        //=========================================================================================
        public void SetDate(DateInfo date) {
            if (date == null) {
                return;
            }
            long ticksTime = m_dateTime.Ticks - new DateTime(m_dateTime.Year, m_dateTime.Month, m_dateTime.Day).Ticks;
            DateTime targetDate = new DateTime(date.Year, date.Month, date.Day);
            long ticksTargetDateTime = targetDate.Ticks + ticksTime;
            m_dateTime = new DateTime(ticksTargetDateTime);
        }

        //=========================================================================================
        // 機　能：時刻部分を設定する
        // 引　数：[in]time  設定する時刻（nullのとき処理しない）
        // 戻り値：なし
        //=========================================================================================
        public void SetTime(TimeInfo time) {
            if (time == null) {
                return;
            }
            m_dateTime = new DateTime(m_dateTime.Year, m_dateTime.Month, m_dateTime.Day, time.Hour, time.Minute, time.Second);
        }

        //=========================================================================================
        // 機　能：DateTimeの値を返す
        // 引　数：なし
        // 戻り値：DateTimeの値
        //=========================================================================================
        public DateTime ToDateTime() {
            return m_dateTime;
        }
    }
}
