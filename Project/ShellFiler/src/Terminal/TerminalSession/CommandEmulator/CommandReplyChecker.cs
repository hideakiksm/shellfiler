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
    // クラス：コマンド応答をチェックするクラス
    //=========================================================================================
    public class CommandReplyChecker {
        // コマンドエコーの確認結果：不一致
        public const int ECHO_NOT_MATCH = -1;

        // コマンドエコーの確認結果：バッファが不足
        public const int ECHO_SHOTAGE = -2;

        //=========================================================================================
        // 機　能：byte配列に対して0x0aを区切りとして行の位置を解析する
        // 引　数：[in]data   元のデータ
        // 戻り値：行の位置
        // メ　モ： 0   1   2   3   4   5   6
        // 　　　　{32, 0d, 0a, 33, 34, 0d, 0a}
        // 　　　　→{0, 1}, {3, 2}, {7, 0}
        //=========================================================================================
        public static BufferRange[] ParseLinePosiiton(byte[] data, string crlf) {
            List<BufferRange> result = new List<BufferRange>();
            if (crlf == CharCode.STR_CRLF) {
                int index = 0;
                int lineStart = 0;
                while (index < data.Length) {
                    if (data[index] == CharCode.BY_CR && index + 1 < data.Length && data[index + 1] == CharCode.BY_LF) {
                        result.Add(new BufferRange(lineStart, index - lineStart));
                        lineStart = index + 2;
                        index += 2;
                    } else {
                        index++;
                    }
                }
                result.Add(new BufferRange(lineStart, index - lineStart));
            } else if (crlf == CharCode.STR_LF) {
                int index = 0;
                int lineStart = 0;
                while (index < data.Length) {
                    if (data[index] == CharCode.BY_LF) {
                        result.Add(new BufferRange(lineStart, index - lineStart));
                        lineStart = index + 1;
                    }
                    index++;
                }
                result.Add(new BufferRange(lineStart, index - lineStart));
            } else {
                Program.Abort("改行コードの種別が想定外です。");
            }
            return result.ToArray();
        }

        //=========================================================================================
        // 機　能：コマンド入力のエコーを確認する
        // 引　数：[in]buffer      受信したデータのバッファ
        // 　　　　[in]offset      解析の開始オフセット
        // 　　　　[in]length      解析する長さ
        // 　　　　[in]commandEcho 期待するバイト列
        // 戻り値：コマンドのエコーだったときはbufferの次の位置のオフセット、エコー以外のときは-1、バッファが不足しているとき-2
        // メ　モ：bashのときはコンソールの右端で20 0Dが挿入されるため、これを無視した状態で評価する
        //=========================================================================================
        public static int CheckCommandEcho(byte[] buffer, int offset, int length, byte[] commandEcho) {
            if (length == 0) {
                return ECHO_SHOTAGE;
            }
            
            int bufferLast = offset + length;
            int lastIndex = length + offset - commandEcho.Length + 1;
            for (int i = offset; i < lastIndex; i++) {
                bool hit = true;
                int indexBuffer = i;
                for (int j = 0; j < commandEcho.Length; j++) {
                    if (buffer[indexBuffer] == 0x20 && indexBuffer + 1 < bufferLast && buffer[indexBuffer + 1] == 0x0d) {
                        // 20 0Dを無視
                        indexBuffer += 2;
                    }
                    if (indexBuffer >= bufferLast) {
                        return ECHO_SHOTAGE;
                    }
                    if (buffer[indexBuffer] != commandEcho[j]) {
                        hit = false;
                        break;
                    }
                    indexBuffer++;
                }
                if (hit) {
                    return indexBuffer;
                }
            }
            return ECHO_NOT_MATCH;
        }

        //=========================================================================================
        // 機　能：プロンプト文字列を確認する
        // 引　数：[in]encoding      文字列のエンコード方式
        // 　　　　[in]buffer        受信したデータのバッファ
        // 　　　　[in]offset        解析の開始オフセット
        // 　　　　[in]length        解析する長さ
        // 　　　　[in]prompt        プロンプトの期待（ユーザー名@サーバー名のタブの次の位置から）
        // 　　　　[in]prevLf        プロンプトの直前に改行を期待するときtrue
        // 　　　　[out]userServer   ユーザー名@サーバー名を返す変数
        // 　　　　[out]promptLength プロンプトの長さを返す変数（trueのときだけ有効）
        // 　　　　[out]nextIndex    次の解析開始位置を返す変数（常に有効、未受信のときoffsetと同じ値）
        // 戻り値：プロンプトが見つかったときtrue
        //=========================================================================================
        public static bool CheckPrompt(Encoding encoding, byte[] buffer, int offset, int length, byte[] prompt, bool prevLf, out string userServer, out int promptLength, out int nextIndex) {
            PromptChecker checker = new PromptChecker(encoding, buffer, offset, length, prompt, prevLf);
            bool found = checker.CheckPrompt(out userServer, out promptLength, out nextIndex);
            return found;
        }

        //=========================================================================================
        // クラス：プロンプトの確認用クラス
        //=========================================================================================
        private class PromptChecker {
            // 文字列のエンコード方式
            private Encoding m_encoding;

            // 受信したデータのバッファ
            private byte[] m_buffer;

            // 解析の開始オフセット
            private int m_startOffset;

            // 解析の終了位置の次のオフセット
            private int m_lastIndex;

            // プロンプトの期待（ユーザー名@サーバー名のタブの次の位置から）
            private byte[] m_prompt;

            // プロンプトの直前に改行を期待するときtrue
            private bool m_prevLf;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]encoding      文字列のエンコード方式
            // 　　　　[in]buffer        受信したデータのバッファ
            // 　　　　[in]offset        解析の開始オフセット
            // 　　　　[in]length        解析する長さ
            // 　　　　[in]prompt        プロンプトの期待（ユーザー名@サーバー名のタブの次の位置から）
            // 　　　　[in]prevLf        プロンプトの直前に改行を期待するときtrue
            // 戻り値：なし
            //=========================================================================================
            public PromptChecker(Encoding encoding, byte[] buffer, int offset, int length, byte[] prompt, bool prevLf) {
                m_encoding = encoding;
                m_buffer = buffer;
                m_startOffset = offset;
                m_lastIndex = offset + length;
                m_prompt = prompt;
                m_prevLf = prevLf;
            }

            //=========================================================================================
            // 機　能：プロンプト文字列を検索する
            // 引　数：[out]userServer   ユーザー名@サーバー名を返す変数
            // 　　　　[out]promptLength プロンプトの長さを返す変数（trueのときだけ有効）
            // 　　　　[out]nextIndex    次の解析開始位置を返す変数（常に有効、未受信のときoffsetと同じ値）
            // 戻り値：プロンプトが見つかったときtrue
            //=========================================================================================
            public bool CheckPrompt(out string userServer, out int promptLength, out int nextIndex) {
                promptLength = 0;
                int i = m_startOffset;
                while (i < m_lastIndex) {
                    bool found = CheckPromptOnce(i, out userServer, out nextIndex);
                    if (found) {
                        if (m_prevLf) {
                            promptLength = nextIndex - i - 1;
                        } else {
                            promptLength = nextIndex - i;
                        }
                        return true;
                    }
                    if (nextIndex == -1) {
                        nextIndex = i;
                        return false;
                    }
                    i = nextIndex;
                }
                userServer = null;
                nextIndex = i;
                return false;
            }

            //=========================================================================================
            // 機　能：指定した位置以降がプロンプト文字列かどうかを確認する
            // 引　数：[in]startIndex    比較開始インデックス
            // 　　　　[out]userServer   ユーザー名@サーバー名を返す変数
            // 　　　　[out]promptLength プロンプトの長さを返す変数（trueのときだけ有効）
            // 　　　　[out]nextIndex    次の解析開始位置を返す変数（未受信のとき-1）
            // 戻り値：プロンプトが見つかったときtrue
            //=========================================================================================
            public bool CheckPromptOnce(int startIndex, out string userServer, out int nextIndex) {
                int index = startIndex;
                userServer = null;

                // 直前のLF
                if (m_prevLf) {
                    if (index >= m_lastIndex) {
                        nextIndex = -1;
                        return false;
                    }
                    if (m_buffer[index] != CharCode.BY_LF) {
                        nextIndex = index + 1;
                        return false;
                    }
                    index++;
                }

                // タブ
                if (index >= m_lastIndex) {
                    nextIndex = -1;
                    return false;
                }
                if (m_buffer[index] != CharCode.BY_TAB) {
                    nextIndex = index + 1;
                    return false;
                }
                index++;

                // ユーザ名@サーバ名 タブ
                int userServerStart = index;
                while (true) {
                    if (index >= m_lastIndex) {
                        nextIndex = -1;
                        return false;
                    }
                    byte ch = m_buffer[index];
                    if ('a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || '0' <= ch && ch <= '9' ||
                            ch == '@' || ch == '.' || ch == '_' || ch == ':') {
                        index++;
                    } else if (ch == m_prompt[0]) {
                        break;
                    } else {
                        nextIndex = index + 1;
                        return false;
                    }
                }                              // indexはm_prompt[0]の位置
                int userServerLength = index - userServerStart;

                // プロンプト
                for (int i = 0; i < m_prompt.Length; i++) {
                    if (index + i >= m_lastIndex) {
                        nextIndex = -1;
                        return false;
                    }
                    if (m_prompt[i] != m_buffer[index + i]) {
                        nextIndex = index + i + 1;
                        return false;
                    }
                }
                userServer = m_encoding.GetString(m_buffer, userServerStart, userServerLength);
                nextIndex = index + m_prompt.Length;
                return true;
            }
        }
    }
}