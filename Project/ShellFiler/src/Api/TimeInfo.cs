using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：時刻情報
    //=========================================================================================
    public class TimeInfo : ICloneable {
        // 時（HH）
        private int m_hour;
        
        // 分（MM）
        private int m_minute;
        
        // 秒（DD）
        private int m_second;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TimeInfo() {
        }
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]hour    時（HH）
        // 　　　　[in]minute  分（MM）
        // 　　　　[in]second  秒（DD）
        // 戻り値：なし
        //=========================================================================================
        public TimeInfo(int hour, int minute, int second) {
            m_hour = hour;
            m_minute = minute;
            m_second = second;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            TimeInfo clone = new TimeInfo();
            clone.m_hour = m_hour;
            clone.m_minute = m_minute;
            clone.m_second = m_second;

            return clone;
        }

        //=========================================================================================
        // 機　能：文字列にする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override string ToString() {
            return string.Format("{0:00}:{1:00}:{2:00}", m_hour, m_minute, m_second);
        }
        
        //=========================================================================================
        // 機　能：数値表現HHMMSSにする
        // 引　数：なし
        // 戻り値：数値表現HHMMSS
        //=========================================================================================
        public int ToIntValue() {
            return m_hour * 10000 + m_minute * 100 + m_second;
        }

        //=========================================================================================
        // 機　能：オブジェクトが同じ値かどうかを比較する
        // 引　数：[in]obj1   オブジェクト１
        // 　　　　[in]obj2   オブジェクト２
        // 戻り値：値が同じときtrue
        //=========================================================================================
        public static bool EqualsValue(TimeInfo obj1, TimeInfo obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_hour != obj2.m_hour) {
                return false;
            } else if (obj1.m_minute != obj2.m_minute) {
                return false;
            } else if (obj1.m_second != obj2.m_second) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：時刻情報を解析する
        // 引　数：[in]timeValue  時刻の文字列表現
        // 戻り値：時刻情報（解析できなかったときnull）
        //=========================================================================================
        public static TimeInfo ParseTimeInfo(string timeValue) {
            if (!Regex.IsMatch(timeValue, "[0-9][0-9]:[0-9][0-9]:[0-9][0-9]")) {
                return null;
            }
            int hh = int.Parse(timeValue.Substring(0, 2));
            int mm = int.Parse(timeValue.Substring(3, 2));
            int ss = int.Parse(timeValue.Substring(6, 2));
            if (hh >= 24) {
                return null;
            }
            if (mm >= 60) {
                return null;
            }
            if (ss >= 60) {
                return null;
            }
            TimeInfo timeInfo = new TimeInfo(hh, mm, ss);
            return timeInfo;
        }

        //=========================================================================================
        // プロパティ：HHMMSS表現
        //=========================================================================================
        public int HHMMSS {
            get {
                return m_hour * 10000 + m_minute * 100 + m_second;
            }
        }

        //=========================================================================================
        // プロパティ：時（HH）
        //=========================================================================================
        public int Hour {
            get {
                return m_hour;
            }
            set {
                m_hour = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：分（MM）
        //=========================================================================================
        public int Minute {
            get {
                return m_minute;
            }
            set {
                m_minute = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：秒（DD）
        //=========================================================================================
        public int Second {
            get {
                return m_second;
            }
            set {
                m_second = value;
            }
        }
    }
}
