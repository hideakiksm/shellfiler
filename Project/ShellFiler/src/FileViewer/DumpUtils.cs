using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Text;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ダンプ用のユーティリティクラス
    //=========================================================================================
    public class DumpUtils {
        
        //=========================================================================================
        // 機　能：ダンプ文字列をバイト列に変換する
        // 引　数：[in]str  ダンプ文字列
        // 戻り値：バイト列
        //=========================================================================================
        public static byte[] StringToBytes(string str) {
            MemoryStream stream = new MemoryStream();
            char[] chList = str.ToCharArray();
            int digitLength = 0;                        // 数値部分の連続長
            int value = 0;                              // 現在の値
            for (int i = 0; i < chList.Length; i++) {
                char ch = chList[i];
                if (digitLength >= 2) {
                    stream.WriteByte((byte)value);
                    digitLength = 0;
                }
                int digit = 0;                          // 読み込んだ数値1桁の値
                if ('0' <= ch && ch <= '9') {
                    digit = ch - '0';
                } else if ('a' <= ch && ch <= 'f') {
                    digit = ch - 'a' + 10;
                } else if ('A' <= ch && ch <= 'F') {
                    digit = ch - 'A' + 10;
                } else {
                    if (digitLength > 0) {
                        stream.WriteByte((byte)value);
                    }
                    digitLength = 0;
                    continue;
                }
                if (digitLength == 0) {
                    value = digit;
                } else {
                    value = value * 16 + digit;
                }
                digitLength++;
            }
            if (digitLength >= 2) {
                stream.WriteByte((byte)value);
                digitLength = 0;
            }
            stream.Close();
            return stream.ToArray();
        }

        //=========================================================================================
        // 機　能：バイト列をダンプ文字列に変換する
        // 引　数：[in]bytes  バイト列
        // 戻り値：ダンプ文字列
        //=========================================================================================
        public static string BytesToString(byte[] bytes) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) {
                sb.Append(bytes[i].ToString("X2"));
                if (i != bytes.Length - 1) {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }
    }
}
