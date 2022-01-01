using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：DateTimeを文字列化する
    //=========================================================================================
    class DateTimeFormatter {
        // 日付のカスタムフォーマット
        public const string RENAME_TIME_CUSTOM_FORMAT = "yyyy/MM/dd  HH:mm:ss";

        // 日付のカスタムフォーマット
        public const string RENAME_DATE_CUSTOM_FORMAT = "yyyy/MM/dd";

        //=========================================================================================
        // 機　能：デフォルトファイル一覧の形式（省略形式）で日付を文字列化する
        // 引　数：[in]time  変換する日付
        // 戻り値：日付文字列
        //=========================================================================================
        public static string DateTimeToDefaultFileList(DateTime time) {
            string strTime;
            if (time == DateTime.MinValue) {
                strTime = "--/--/-- --:--";
            } else {
                strTime = time.ToString("yy/MM/dd HH:mm");
            }
            return strTime;
        }

        //=========================================================================================
        // 機　能：情報出力用の形式（長い文字列）で日付を文字列化する
        // 引　数：[in]time  変換する日付
        // 戻り値：日付文字列
        //=========================================================================================
        public static string DateTimeToInformation(DateTime time) {
            string strTime;
            if (time == DateTime.MinValue) {
                strTime = "----/--/-- --:--:--";
            } else {
                strTime = time.ToString("yyyy/MM/dd HH:mm:ss");
            }
            return strTime;
        }

        //=========================================================================================
        // 機　能：情報出力用の形式（区切りなしの長い形式）で日付を文字列化する
        // 引　数：[in]time  変換する日付
        // 戻り値：日付文字列
        //=========================================================================================
        public static string DateTimeToNoSeparate(DateTime time) {
            string strTime;
            if (time == DateTime.MinValue) {
                strTime = "00000000_000000";
            } else {
                strTime = time.ToString("yyyyMMdd_HHmmss");
            }
            return strTime;
        }

        //=========================================================================================
        // 機　能：情報出力用の形式で時刻HHMMSSを文字列化する
        // 引　数：[in]time  変換する時刻（HHMMSS形式）
        // 戻り値：日付文字列
        //=========================================================================================
        public static string HHMMSSToInformation(int time) {
            int hh = time / 10000;
            int mm = (time / 100) % 100;
            int ss = time % 100;
            string strTime = string.Format("{0:00}:{1:00}:{2:00}", hh, mm, ss);
            return strTime;
        }

        //=========================================================================================
        // 機　能：ファイル名用の形式（長い文字列）で日付を文字列化する
        // 引　数：[in]time  変換する日付
        // 戻り値：日付文字列
        //=========================================================================================
        public static string DateTimeToInformationForFile(DateTime time) {
            string strTime = time.ToString("yyyyMMdd_HHmmss");
            return strTime;
        }
    }
}
