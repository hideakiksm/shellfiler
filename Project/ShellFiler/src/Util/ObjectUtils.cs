using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Security.Cryptography;
using ShellFiler.Api;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：オブジェクト型のライブラリ
    //=========================================================================================
    public class ObjectUtils {

        //=========================================================================================
        // 機　能：指定された数値を切り上げる
        // 引　数：[in]a  数値
        // 戻り値：切り上げた数値
        //=========================================================================================
        public static int Ceil(float a) {
            return (int)(Math.Ceiling(a));
        }

        //=========================================================================================
        // 機　能：日時情報の時刻部分を指定された数値に丸める
        // 引　数：[in]date   日付情報
        // 　　　　[in]time   丸める時刻（HHMMSS）
        // 戻り値：丸めた日付
        //=========================================================================================
        public static DateTime SetDateHHMMSS(DateTime date, int time) {
            int hh = time / 10000;
            int mm = (time / 100) % 100;
            int ss = time % 100;

            date = date.AddHours(-date.Hour + hh);
            date = date.AddMinutes(-date.Minute + mm);
            date = date.AddSeconds(-date.Second + ss);
            date = date.AddMilliseconds(-date.Millisecond);

            return date;
        }

        //=========================================================================================
        // 機　能：日時情報の時刻部分を00:00:00にクリアする
        // 引　数：[in]date   日付情報
        // 戻り値：クリアした日付
        //=========================================================================================
        public static DateTime ClearTimePartOfDateTime(DateTime date) {
            date = date.AddHours(-date.Hour);
            date = date.AddMinutes(-date.Minute);
            date = date.AddSeconds(-date.Second);
            date = date.AddMilliseconds(-date.Millisecond);

            return date;
        }

        //=========================================================================================
        // 機　能：色が同じかどうかを返す
        // 引　数：[in]a   色情報1
        // 　　　　[in]b   色情報2
        // 戻り値：色が同じときtrue
        //=========================================================================================
        public static bool EqualsColor(Color a, Color b) {
            if (a.R == b.R && a.G == b.G && a.B == b.B) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：SizeFをSizeに変換する
        // 引　数：[in]size  サイズ
        // 戻り値：変換結果
        //=========================================================================================
        public static Size SizeFToSize(SizeF size) {
            return new Size((int)(Math.Ceiling(size.Width)), (int)(Math.Ceiling(size.Height)));
        }

        //=========================================================================================
        // 機　能：byte配列のハッシュをlongで返す
        // 引　数：[in]data  ハッシュを求めるデータ
        // 戻り値：ハッシュ値
        //=========================================================================================
        public static long GetMD5HashLong(byte[] data) {
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(data);
            long value = 0;
            for (int i = 0; i < hash.Length; i++) {
                value ^= hash[i] << (8 * (i % 8));
            }
            return value;
        }
    }
}
