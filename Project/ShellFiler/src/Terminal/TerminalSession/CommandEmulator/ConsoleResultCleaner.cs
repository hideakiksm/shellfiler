using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Shell;
using ShellFiler.UI;
using ShellFiler.Util;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：コンソールの実行結果をクリーンナップする機能
    // メ　モ：bashのときは戻りのLFがCR LFとなって転送されるため、CR LF→LFと戻す
    //=========================================================================================
    public class ConsoleResultCleaner {
        // 標準出力の改行コード整形が必要なときtrue
        private bool m_crlfConvert;

        // 前回の最後にCRを読み込んだ状態のときtrue
        private bool m_prevReadCr = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]crlfConvert  標準出力の改行コード整形が必要なときtrue
        // 戻り値：なし
        //=========================================================================================
        public ConsoleResultCleaner(bool crlfConvert) {
            m_crlfConvert = crlfConvert;
        }
        
        //=========================================================================================
        // 機　能：受信データを元のデータの状態に戻す
        // 引　数：[in]buffer    受信したデータのバッファ
        // 　　　　[in]offset    解析の開始オフセット
        // 　　　　[in]length    解析する長さ
        // 　　　　[in]isFinal   データの終わりのときtrue
        // 戻り値：整形したデータ
        //=========================================================================================
        public byte[] CleanReceivedData(byte[] buffer, int offset, int length, bool isFinal) {
            // そのまま返すとき
            if (!m_crlfConvert) {
                return ArrayUtils.CreateCleanedBuffer<byte>(buffer, offset, length);
            }

            // CR LF→LFの変換を行うとき
            MemoryStream stream = new MemoryStream();
            int last = offset + length;
            for (int index = offset; index < last; index++) {
                if (buffer[index] == CharCode.BY_CR) {
                    // CR → 出力は次回へ
                    if (m_prevReadCr) {
                        // CR CR
                        stream.WriteByte(CharCode.BY_CR);
                    }
                    m_prevReadCr = true;
                } else if (buffer[index] == CharCode.BY_LF) {
                    // CR LF、または、XX LF → LFのみ、CRはクリア
                    stream.WriteByte(buffer[index]);
                    m_prevReadCr = false;
                } else {
                    if (m_prevReadCr) {
                        // CR XX
                        stream.WriteByte(CharCode.BY_CR);
                        stream.WriteByte(buffer[index]);
                    } else {
                        // YY XX
                        stream.WriteByte(buffer[index]);
                    }
                    m_prevReadCr = false;
                }
            }
            if (m_prevReadCr && isFinal) {
                stream.WriteByte(CharCode.BY_CR);
                m_prevReadCr = false;
            }
            stream.Close();
            return stream.ToArray();
        }
    }
}
