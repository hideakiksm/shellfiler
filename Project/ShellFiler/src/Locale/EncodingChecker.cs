using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ShellFiler.Api;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：テキストの文字コードを判別するクラス
    //=========================================================================================
    public class EncodingChecker {
        // 解析対象のバッファ
        private byte[] m_buffer;
        
        // バッファ中の有効なサイズ
        private int m_bufferSize;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]buffer   解析対象のバッファ
        // 　　　　[in]size     バッファ中の有効なサイズ
        // 戻り値：なし
        //=========================================================================================
        public EncodingChecker(byte[] buffer, int bufferSize) {
            m_buffer = buffer;
            m_bufferSize = bufferSize;
        }

        //=========================================================================================
        // 機　能：バッファ内の文字コードを解析する
        // 引　数：なし
        // 戻り値：文字コードの判定結果（UNKNOWNを含まない）
        //=========================================================================================
        public EncodingType CheckEncodingType() {
            // 各行でその文字コードと判定された回数
            Dictionary<EncodingType, int> encodeCount = new Dictionary<EncodingType, int>();
            foreach (EncodingType type in EncodingType.AllValue) {
                encodeCount.Add(type, 0);
            }

            // バッファ内を行単位に判断
            int start = 0;
            int index = 0;
            while (index < m_bufferSize) {
                if (m_buffer[index] == 0x0d || m_buffer[index] == 0x0a) {
                    if (index != start) {
                        EncodingType lineType = GetCode(m_buffer, start, index - start);
                        encodeCount[lineType] = encodeCount[lineType] + 1;
                    }
                    index++;
                    start = index;
                } else {
                    index++;
                }
            }
            if (index != start) {
                EncodingType lineType = GetCode(m_buffer, start, index - start);
                encodeCount[lineType] = encodeCount[lineType] + 1;
            }

            // ２行以上バイナリに判定さえたらバイナリ
            if (encodeCount[EncodingType.BINARY] >= 2) {
                return EncodingType.BINARY;
            }

            // 回数が最も多いものを判定
            int max = -1;
            EncodingType resultType = EncodingType.SJIS;
            foreach (EncodingType type in encodeCount.Keys) {
                if (type == EncodingType.UNKNOWN || encodeCount[type] == 0) {
                    continue;
                }
                if (max < encodeCount[type]) {
                    max = encodeCount[type];
                    resultType = type;
                }
            }

            return resultType;
        }

        //=========================================================================================
        // 機　能：1行分の文字コードの種類を判別する
        // 引　数：[in]buffer  解析対象のバッファ
        // 　　　　[in]start   開始位置
        // 　　　　[in]length  解析する長さ
        // 戻り値：文字コードの判定結果（UNKNOWNを含む）
        // メ　モ：オリジナルコードはhttp://dobon.net/vb/dotnet/string/detectcode.html
        //=========================================================================================
        public static EncodingType GetCode(byte[] buffer, int start, int length) {
            const byte bEscape = 0x1B;
            const byte bAt = 0x40;
            const byte bDollar = 0x24;
            const byte bAnd = 0x26;
            const byte bOpen = 0x28;    //'('
            const byte bB = 0x42;
            const byte bD = 0x44;
            const byte bJ = 0x4A;
            const byte bI = 0x49;

            byte b1, b2, b3, b4;
            int end = start + length;           // 終了の次の位置

            bool isBinary = false;
            bool isUnicode = false;
            for (int i = start; i < end; i++) {
                b1 = buffer[i];
                if (b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF) {
                    // 'binary'
                    isBinary = true;
                    if (b1 == 0x00 && i < end - 1) {
                        if (buffer[i + 1] == 0x00) {
                            return EncodingType.BINARY;
                        } else if (buffer[i + 1] <= 0x7F) {
                            // smells like raw unicode
                            isUnicode = true;
                        }
                    }
                }
            }
            if (isUnicode) {
                return EncodingType.UNICODE;
            }
            if (isBinary) {
                return EncodingType.BINARY;
            }

            for (int i = start; i < end - 2; i++) {
                b1 = buffer[i];
                b2 = buffer[i + 1];
                b3 = buffer[i + 2];

                if (b1 == bEscape) {
                    if (b2 == bDollar && b3 == bAt) {
                        // JIS_0208 1978
                        return EncodingType.JIS;
                    } else if (b2 == bDollar && b3 == bB) {
                        // JIS_0208 1983
                        return EncodingType.JIS;
                    } else if (b2 == bOpen && (b3 == bB || b3 == bJ)) {
                        // JIS_ASC
                        return EncodingType.JIS;
                    } else if (b2 == bOpen && b3 == bI) {
                        // JIS_KANA
                        return EncodingType.JIS;
                    }
                    if (i < end - 3) {
                        b4 = buffer[i + 3];
                        if (b2 == bDollar && b3 == bOpen && b4 == bD) {
                            // JIS_0212
                            return EncodingType.JIS;
                        } else if (i < end - 5 && b2 == bAnd && b3 == bAt && b4 == bEscape && buffer[i + 4] == bDollar && buffer[i + 5] == bB) {
                            // JIS_0208 1990
                            return EncodingType.JIS;
                        }
                    }
                }
            }

            // should be euc|sjis|utf8
            // use of (?:) by Hiroki Ohzaki <ohzaki@iod.ricoh.co.jp>
            int sjis = 0;
            int euc = 0;
            int utf8 = 0;
            for (int i = start; i < end - 1; i++) {
                b1 = buffer[i];
                b2 = buffer[i + 1];
                if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) && ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC))) {
                    // SJIS_C
                    sjis += 2;
                    i++;
                }
            }
            for (int i = start; i < end - 1; i++) {
                b1 = buffer[i];
                b2 = buffer[i + 1];
                if (((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) || (b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF))) {
                    // EUC_C
                    // EUC_KANA
                    euc += 2;
                    i++;
                } else if (i < end - 2) {
                    b3 = buffer[i + 2];
                    if (b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) && (0xA1 <= b3 && b3 <= 0xFE)) {
                        // EUC_0212
                        euc += 3;
                        i += 2;
                    }
                }
            }
            for (int i = start; i < end - 1; i++) {
                b1 = buffer[i];
                b2 = buffer[i + 1];
                if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF)) {
                    // UTF8
                    utf8 += 2;
                    i++;
                } else if (i < end - 2) {
                    b3 = buffer[i + 2];
                    if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) && (0x80 <= b3 && b3 <= 0xBF)) {
                        // UTF8
                        utf8 += 3;
                        i += 2;
                    }
                }
            }
            // M. Takahashi's suggestion
            // utf8 += utf8 / 2;

            if (euc > sjis && euc > utf8) {
                // EUC
                return EncodingType.EUC;
            } else if (sjis > euc && sjis > utf8) {
                // SJIS
                return EncodingType.SJIS;
            } else if (utf8 > euc && utf8 > sjis) {
                // UTF8
                return EncodingType.UTF8;
            }

            return EncodingType.UNKNOWN;
        }
    }
}
