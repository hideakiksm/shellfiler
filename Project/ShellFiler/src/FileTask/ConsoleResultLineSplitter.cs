using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;
using ShellFiler.Locale;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：コンソールの実行結果を行単位に分離するクラス
    //=========================================================================================
    class ConsoleResultLineSplitter {
        // エンコード方式
        private Encoding m_encoding;

        // 直前のデータバッファ
        private byte[] m_prevDataBuffer = new byte[0];

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]encoding   エンコード方式
        // 戻り値：なし
        //=========================================================================================
        public ConsoleResultLineSplitter(Encoding encoding) {
            m_encoding = encoding;
        }

        //=========================================================================================
        // 機　能：新しいデータを追加する
        // 引　数：[in]buffer  データの入ったバッファ
        // 　　　　[in]offset  buffer中のオフセット
        // 　　　　[in]length  データの長さ
        // 戻り値：文字列に変換した結果
        //=========================================================================================
        public List<string> AddData(byte[] buffer, int offset, int length) {
            List<string> result = new List<string>();
            MemoryStream stream = new MemoryStream();
            stream.Write(m_prevDataBuffer, 0, m_prevDataBuffer.Length);
            int lastOffset = offset + length;
            for (int i = offset; i < lastOffset; i++) {
                if (buffer[i] == CharCode.BY_CR) {
                    ;
                } else if (buffer[i] == CharCode.BY_LF) {
                    stream.Close();
                    byte[] lineBytes = stream.ToArray();
                    string line = m_encoding.GetString(lineBytes);
                    result.Add(line);
                    stream = new MemoryStream();
                } else {
                    stream.WriteByte(buffer[i]);
                }
            }
            stream.Close();
            m_prevDataBuffer = stream.ToArray();
            return result;
        }

        //=========================================================================================
        // 機　能：最後の１行のデータを取得する
        // 引　数：なし
        // 戻り値：最後の１行のデータ（不要なときnull）
        //=========================================================================================
        public string GetLastLine() {
            if (m_prevDataBuffer.Length == 0) {
                return null;
            } else {
                string line = m_encoding.GetString(m_prevDataBuffer);
                return line;
            }
        }
    }
}
