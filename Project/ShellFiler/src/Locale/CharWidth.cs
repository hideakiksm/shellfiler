using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using ShellFiler.Util;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：文字の幅（半角/全角）を判断するクラス
    //=========================================================================================
    class CharWidth {
        // ビットアクセス用
        // スレッドセーフにするため、2ビット単位でアクセス
        // 全角：全角マスクが1、半角：半角マスクが1、どちらでもない：両方が1、未処理：両方が0
        // (s_arrFullWidth[index / 4] & BIT_MASK[index % 4])でアクセス
        private static readonly byte[] BIT_MASK = {
            0x03, 0x0c, 0x30, (byte)0xc0,
        };
        // BIT_MASKで取得後、全角であることを確認するマスク
        private static readonly byte[] FULL_MASK = {
            0x01, 0x04, 0x10, 0x40,
        };
        // BIT_MASKで取得後、半角であることを確認するマスク
        private static readonly byte[] HALF_MASK = {
            0x02, 0x08, 0x20, 0x80,
        };
        
        // 全角/半角の判断用
        private static byte[] s_charType = null;

        // 半角の文字幅[ピクセル]
        private static int s_halfCharWidth = -1;

        // 全角の文字幅[ピクセル]
        private static int s_fullCharWidth = -1;

        // 半角全角の判断に使用するフォント
        private static Font s_font = null;

        // 初期化が完了したときtrue
        private static bool s_initialized = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CharWidth() {
            if (!s_initialized) {
                s_charType = new byte[65536 * 2];
                Array.Clear(s_charType, 0, s_charType.Length);
                s_halfCharWidth = -1;
                s_fullCharWidth = -1;
                InitializeBySjis();
                s_font = new Font("MS GOTHIC", 10.0f, FontStyle.Regular);
                s_initialized = true;
            }
        }

        //=========================================================================================
        // 機　能：SJISの文字範囲から半角／全角を判断する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void InitializeBySjis() {
            Encoding sjisEncoding = Encoding.GetEncoding(932);
            byte[] byBuf = new byte[2];
            char[] chBuf;
            for (int i = 0; i <= 0xff; i++) {
                if ((0x81 <= i && i <= 0x9f) || (0xe0 <= i)) {
                    for (int j = 0x40; j <= 0xfc; j++) {
                        if (j <= 0x7e || 0x80 <= j) {
                            // 全角文字
                            byBuf[0] = (byte)i;
                            byBuf[1] = (byte)j;
                            chBuf = sjisEncoding.GetChars(byBuf, 0, 2);
                            int pos = (int)(chBuf[0]);
                            s_charType[pos / 4] |= FULL_MASK[pos % 4];
                        }
                    }
                } else if (0x20 <= i && i < 0x7f) {
                    // 半角文字（0x20未満以外、DEL以外）
                    byBuf[0] = (byte)i;
                    chBuf = sjisEncoding.GetChars(byBuf, 0, 1);
                    int pos = (int)(chBuf[0]);
                    s_charType[pos / 4] |= HALF_MASK[pos % 4];
                }
            }

            // 制御コードを無効化
            for (int i = 0; i < 20; i++) {
                s_charType[i / 4] |= (byte)(HALF_MASK[i % 4] | FULL_MASK[i % 4]);
            }
            s_charType[0x7f / 4] |= (byte)(HALF_MASK[0x7f % 4] | FULL_MASK[0x7f % 4]);
        }

        //=========================================================================================
        // 機　能：文字の種類を判断する
        // 引　数：[in]ch   判断する文字
        // 戻り値：文字の種類（初期化されていないときCharType.UnInitializeを返すことがある）
        //=========================================================================================
        public CharType GetCharType(char ch) {
            int index = (int)ch;
            byte flag = (byte)(s_charType[index / 4] & BIT_MASK[index % 4]);
            bool full = ((flag & FULL_MASK[index % 4]) != 0);
            bool half = ((flag & HALF_MASK[index % 4]) != 0);
            if (full && half) {
                return CharType.Unknown;
            } else if (full) {
                return CharType.FullWidth;
            } else if (half) {
                return CharType.HalfWidth;
            } else {
                return CharType.UnInitialize;
            }
        }
   
        //=========================================================================================
        // 機　能：文字の種類を判断する
        // 引　数：[in]g    描画に使用するグラフィックス（未初期化状態で返すときnull）
        // 　　　　[in]ch   判断する文字
        // 戻り値：文字の種類
        //=========================================================================================
        public CharType GetCharType(Graphics g, char ch) {
            int index = (int)ch;
            byte flag = (byte)(s_charType[index / 4] & BIT_MASK[index % 4]);
            bool full = ((flag & FULL_MASK[index % 4]) != 0);
            bool half = ((flag & HALF_MASK[index % 4]) != 0);
            if (full && half) {
                return CharType.Unknown;
            } else if (full) {
                return CharType.FullWidth;
            } else if (half) {
                return CharType.HalfWidth;
            } else {
                if (g == null) {
                    return CharType.UnInitialize;
                }
                // 半角と全角の文字幅を描画して測定
                if (s_halfCharWidth == -1) {
                    s_halfCharWidth = ObjectUtils.Ceil(GraphicsUtils.MeasureString(g, s_font, "MMMMMMMMMM") / 10.0f);
                    s_fullCharWidth = ObjectUtils.Ceil(GraphicsUtils.MeasureString(g, s_font, "ああああああああああ") / 10.0f);
                }

                // 描画テストして決定
                const int SAMPLE_CHAR_TIMES = 10;               // 10文字分描画してチェック
                char[] buf = new char[SAMPLE_CHAR_TIMES];
                for (int j = 0; j < buf.Length; j++) {
                    buf[j] = (char)ch;
                }
                string str = new String(buf);
                int widthTest = ObjectUtils.Ceil(GraphicsUtils.MeasureString(g, s_font, str)) / SAMPLE_CHAR_TIMES;
                
                // 描画したときの幅から判断
                if (Math.Abs(s_halfCharWidth - widthTest) <= 1) {           // 半角と判断
                    s_charType[index / 4] |= HALF_MASK[index % 4];
                    return CharType.HalfWidth;
                } else if (Math.Abs(s_fullCharWidth - widthTest) <= 1) {    // 全角と判断
                    s_charType[index / 4] |= FULL_MASK[index % 4];
                    return CharType.FullWidth;
                } else {                                                    // どちらでもないと判断
                    s_charType[index / 4] |= (byte)(HALF_MASK[index % 4] | FULL_MASK[index % 4]);
                    return CharType.Unknown;
                }
            }
        }

        //=========================================================================================
        // 列挙子：文字の種類
        //=========================================================================================
        public enum CharType {
            HalfWidth,              // 半角文字
            FullWidth,              // 全角文字
            Unknown,                // 不明
            UnInitialize,           // 未計測
        }
    }
}
