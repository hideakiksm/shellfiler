using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ダンプビューアでのクリップボード文字列の整形クラス
    //=========================================================================================
    public class DumpClipboardFormatter {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DumpClipboardFormatter() {
        }

        //=========================================================================================
        // 機　能：クリップボードの内容を整形する
        // 引　数：[in]data        バッファの内容
        // 　　　　[in]start       開始アドレス
        // 　　　　[in]end         終了アドレス（次のバイト位置）
        // 　　　　[in]setting     整形の方法
        // 　　　　[in]screenWidth 画面の1行あたりのバイト数
        // 　　　　[in]encoding    エンコードの種類
        // 戻り値：整形した文字列
        //=========================================================================================
        public string Format(byte[] data, int start, int end, DumpClipboardSetting setting, int screenWidth, EncodingType encoding) {
            switch (setting.Mode) {
                case DumpMode.Text:
                    return FormatText(data, start, end, setting);
                case DumpMode.Dump:
                    return FormatDump(data, start, end, setting);
                case DumpMode.Screen:
                    return FormatDumpScreen(data, start, end, setting, screenWidth, encoding);
                case DumpMode.View:
                    return FormatDumpView(data, start, end, screenWidth, encoding);
                case DumpMode.Base64:
                    return FormatBase64(data, start, end, setting);
                default:
                    return "";
            }
        }

        //=========================================================================================
        // 機　能：クリップボードの内容をテキスト形式で整形する
        // 引　数：[in]data    バッファの内容
        // 　　　　[in]start   開始アドレス
        // 　　　　[in]end     終了アドレス（次のバイト位置）
        // 　　　　[in]setting 整形の方法
        // 戻り値：整形した文字列
        //=========================================================================================
        public string FormatText(byte[] data, int start, int end, DumpClipboardSetting setting) {
            return setting.Encoding.GetString(data, start, end - start);
        }

        //=========================================================================================
        // 機　能：クリップボードの内容をダンプ形式で整形する
        // 引　数：[in]data    バッファの内容
        // 　　　　[in]start   開始アドレス
        // 　　　　[in]end     終了アドレス（次のバイト位置）
        // 　　　　[in]setting 整形の方法
        // 戻り値：整形した文字列
        //=========================================================================================
        public string FormatDump(byte[] data, int start, int end, DumpClipboardSetting setting) {
            char paddingChar;
            if (setting.DumpZeroPadding) {
                paddingChar = '0';
            } else {
                paddingChar = ' ';
            }

            int byteCount = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = start; i < end; i++) {
                byteCount++;
                sb.Append(setting.DumpPrefixString);
                string strByte = setting.DumpRadix.ConvertByte(data[i]);
                for (int j = strByte.Length; j < setting.DumpWidth; j++) {
                    sb.Append(paddingChar);
                }
                sb.Append(strByte);
                sb.Append(setting.DumpPostfixString);
                if (i == end - 1) {
                    ;
                } else if (setting.DumpLineWidth > 0 && byteCount % setting.DumpLineWidth == 0) {
                    sb.Append("\r\n");
                } else {
                    sb.Append(setting.DumpSeparator);
                }
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：クリップボードの内容を画面と同じテキスト表記で整形する
        // 引　数：[in]data        バッファの内容
        // 　　　　[in]start       開始アドレス
        // 　　　　[in]end         終了アドレス（次のバイト位置）
        // 　　　　[in]setting     整形の方法
        // 　　　　[in]screenWidth 画面の1行あたりのバイト数
        // 　　　　[in]encoding    エンコードの種類
        // 戻り値：整形した文字列
        //=========================================================================================
        private string FormatDumpScreen(byte[] data, int start, int end, DumpClipboardSetting setting, int screenWidth, EncodingType encoding) {
            if (start / screenWidth == end / screenWidth) {
                // 1行
                string strText;
                List<int> charToByte, byteToChar;
                DumpTextFormatter converter = new DumpTextFormatter();
                converter.Convert(encoding, data, start, end - start, out strText, out charToByte, out byteToChar);
                return strText;
            } else {
                // 複数行
                StringBuilder sb = new StringBuilder();
                string strText;
                List<int> charToByte, byteToChar;

                // 1行目
                DumpTextFormatter converter = new DumpTextFormatter();
                converter.Convert(encoding, data, start, screenWidth - start % screenWidth, out strText, out charToByte, out byteToChar);
                sb.Append(strText).Append("\r\n");
                // 中間行
                int midStartLine = start / screenWidth + 1;
                int midEndLine = end / screenWidth - 1;
                for (int i = midStartLine; i <= midEndLine; i++) {
                    converter.Convert(encoding, data, i * screenWidth, screenWidth, out strText, out charToByte, out byteToChar);
                    sb.Append(strText).Append("\r\n");
                }
                // 最終行
                if (end % screenWidth > 0) {
                    converter.Convert(encoding, data, end / screenWidth * screenWidth, end % screenWidth, out strText, out charToByte, out byteToChar);
                    sb.Append(strText).Append("\r\n");
                }
                return sb.ToString();
            }
        }

        //=========================================================================================
        // 機　能：クリップボードの内容をビュー形式で整形する
        // 引　数：[in]data     バッファの内容
        // 　　　　[in]start    開始アドレス
        // 　　　　[in]end      終了アドレス（次のバイト位置）
        // 　　　　[in]screenWidth 画面の1行あたりのバイト数
        // 　　　　[in]encoding    エンコードの種類
        // 戻り値：整形した文字列
        //=========================================================================================
        public string FormatDumpView(byte[] data, int start, int end, int screenWidth, EncodingType encoding) {
            StringBuilder sb = new StringBuilder();
            DumpHexFormatter dumpFormatter = new DumpHexFormatter();
            int lineStart = start / screenWidth;
            int lineEnd = (end - 1) / screenWidth;
            for (int line = lineStart; line <= lineEnd; line++) {
                int startPos = Math.Max(line * screenWidth, start);
                int endPos = Math.Min(line * screenWidth + screenWidth - 1, end - 1);

                // アドレス
                string strAddress = (line * screenWidth).ToString("X8");

                // ダンプ
                string strDump;
                List<int> dummyPos;
                dumpFormatter.CreateDumpHexStr(data, startPos, endPos - startPos + 1, screenWidth, out strDump, out dummyPos);

                // テキスト
                DumpTextFormatter textFormatter = new DumpTextFormatter();
                string strText;
                List<int> dummyCharToByte, dummyByteToChar;
                textFormatter.Convert(encoding, data, startPos, endPos - startPos + 1, out strText, out dummyCharToByte, out dummyByteToChar);

                // 整形
                if (line == lineStart) {
                    int column = start % screenWidth;
                    strDump = StringUtils.Repeat(" ", column * 3 + column / 8 * 2) + strDump;
                    strText = StringUtils.Repeat(" ", column) + strText;
                }
                if (line == lineEnd) {
                    int column = (end - 1) % screenWidth;
                    strDump = strDump + StringUtils.Repeat(" ",  (screenWidth - column - 1) * 3 + ((screenWidth - column + 1) / 8 * 2));
                }

                sb.Append(strAddress).Append(" : ").Append(strDump).Append("  |  ").Append(strText).Append("\r\n");
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：クリップボードの内容をBASE64形式で整形する
        // 引　数：[in]data    バッファの内容
        // 　　　　[in]start   開始アドレス
        // 　　　　[in]end     終了アドレス（次のバイト位置）
        // 　　　　[in]setting 整形の方法
        // 戻り値：整形した文字列
        //=========================================================================================
        public string FormatBase64(byte[] data, int start, int end, DumpClipboardSetting setting) {
            string base64 = Convert.ToBase64String(data, start, end - start);
            if (setting.Base64FoldingWidth > 0) {
                StringBuilder sb = new StringBuilder();
                int index = 0;
                while (index < base64.Length) {
                    if (base64.Length - index <= setting.Base64FoldingWidth) {
                        sb.Append(base64.Substring(index));
                    } else {
                        sb.Append(base64.Substring(index, setting.Base64FoldingWidth));
                    }
                    sb.Append("\r\n");
                    index += setting.Base64FoldingWidth;
                }
                base64 = sb.ToString();
            }
            return base64;
        }
    }
}
