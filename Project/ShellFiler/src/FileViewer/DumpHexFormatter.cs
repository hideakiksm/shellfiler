using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ダンプの16進ダンプを整形するクラス
    //=========================================================================================
    public class DumpHexFormatter {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DumpHexFormatter() {
        }

        //=========================================================================================
        // 機　能：16進数ダンプの文字列を作成する
        // 引　数：[in]buffer        ダンプ対象のバッファ
        // 　　　　[in]start         バッファ中の変換開始位置
        // 　　　　[in]length        バッファ中の変換長
        // 　　　　[in]lineBytes     1行中のバイト数
        // 　　　　[out]dumpStr      変換結果の文字列を返す変数
        // 　　　　[out]dumpPosition 変換文字列中の各バイト位置を返す変数
        // 戻り値：なし
        // メ　モ：buffer={1,2,3,4,5}、start=1, length=3のとき、dumpStr="02 03 04"、position={0,2, 3,5, 7,8}
        //=========================================================================================
        public void CreateDumpHexStr(byte[] buffer, int start, int length, int lineBytes, out string dumpStr, out List<int> dumpPosition) {
            dumpPosition = new List<int>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++) {
                dumpPosition.Add(sb.Length);
                byte data = buffer[start + i];
                sb.Append(data.ToString("X2"));
                dumpPosition.Add(sb.Length);
                if (i == lineBytes - 1 || i == length - 1) {
                    ;
                } else if ((start + i) % 8 == 7) {
                    sb.Append(" - ");
                } else {
                    sb.Append(' ');
                }
            }
            dumpStr = sb.ToString();
        }
    }
}
