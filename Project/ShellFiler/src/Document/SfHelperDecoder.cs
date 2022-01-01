using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Util;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：SfHelperからの暗号を解除するクラス
    //=========================================================================================
    public class SfHelperDecoder {
        // デコードする値
        private int m_value;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]value  デコードする値
        // 戻り値：なし
        //=========================================================================================
        public SfHelperDecoder(int value) {
            m_value = value;
        }

        //=========================================================================================
        // 機　能：パスワードソルトをデコードする
        // 引　数：なし
        // 戻り値：パスワードソルト
        //=========================================================================================
        public int DecodePasswordSalt() {
            int[] decodeKey = {
                19,  5, 10, 18, 31,  2, 23, 12,  8, 16,  9, 26, 17,  7, 15, 21

            };
            int result = 0;
            for (int i = 0; i < decodeKey.Length; i++) {
                result = result << 1;
                result |= (m_value >> decodeKey[15 - i]) & 0x1;
            }
            return result;
        }

        //=========================================================================================
        // 機　能：日付を返す
        // 引　数：なし
        // 戻り値：日付（10進数でYYYYMMDD）
        //=========================================================================================
        public int DecodeDate() {
            int result = (((m_value >> 28) & 0x1) <<  0) |
                         (((m_value >>  6) & 0x1) <<  1) |
                         (((m_value >> 27) & 0x1) <<  2) |
                         (((m_value >> 24) & 0x1) <<  3) |
                         (((m_value >> 22) & 0x1) <<  4) |
                         (((m_value >> 29) & 0x1) <<  5) |
                         (((m_value >> 11) & 0x1) <<  6) |
                         (((m_value >>  4) & 0x1) <<  7) |
                         (((m_value >> 30) & 0x1) <<  8) |
                         (((m_value >>  1) & 0x1) <<  9) |
                         (((m_value >> 25) & 0x1) << 10) |
                         (((m_value >> 14) & 0x1) << 11) |
                         (((m_value >>  0) & 0x1) << 12) |
                         (((m_value >> 13) & 0x1) << 13) |
                         (((m_value >> 20) & 0x1) << 14) |
                         (((m_value >>  3) & 0x1) << 15);
            result = result ^ 0x1234;
        	// date = (systime.wYear - 2000) * 400 + (systime.wMonth - 1) * 32 + systime.wDay;
            int yyyy = result / 400 + 2000;
            int mm = (result % 400) / 32 + 1;
            int dd = result % 400 % 32;
            int yyyymmdd = yyyy * 10000 + mm * 100 + dd;
            return yyyymmdd;
        }

        //=========================================================================================
        // 機　能：パスワードソルトを返す
        // 引　数：なし
        // 戻り値：パスワードソルト
        //=========================================================================================
        public static int GetPasswordSalt() {
            int saltSrc = SfHelper.GetPasswordSaltDate();
            SfHelperDecoder decoder = new SfHelperDecoder(saltSrc);
            int salt = decoder.DecodePasswordSalt();
            return salt;
        }

        //=========================================================================================
        // 機　能：日付を返す
        // 引　数：なし
        // 戻り値：日付（10進数でYYYYMMDD）
        //=========================================================================================
        public static int GetDate() {
            int saltSrc = SfHelper.GetPasswordSaltDate();
            SfHelperDecoder decoder = new SfHelperDecoder(saltSrc);
            int date = decoder.DecodeDate();
            return date;
        }
    }
}
