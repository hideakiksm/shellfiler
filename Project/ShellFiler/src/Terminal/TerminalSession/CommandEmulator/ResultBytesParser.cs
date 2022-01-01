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
    // クラス：結果バイト列の解析エンジン
    //=========================================================================================
    public class ResultBytesParser {
        // 使用中のSSH接続
        private SSHConnection m_connection;

        // 使用中の解析エンジン
        private IShellEmulatorEngine m_emulatorEngine;

        // コマンドのエミュレータ
        private ShellCommandEmulator m_shellCommandEmulator;

        // エラー行の解析ルール
        private List<OSSpecLineExpect> m_errorRule;

        // 応答行の先頭にあるコマンドのエコーを受信したときtrue
        private bool m_skippedCommandEcho = false;

        // エラー行の可能性がある行を解析したときtrue
        private bool m_parsedErrorLine = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection  使用中のSSH接続
        // 　　　　[in]engine      使用中の解析エンジン
        // 　　　　[in]emlator     コマンドのエミュレータ
        // 　　　　[in]errorRule   エラー行の解析ルール
        // 戻り値：なし
        //=========================================================================================
        public ResultBytesParser(SSHConnection connection, IShellEmulatorEngine engine, ShellCommandEmulator emulator, List<OSSpecLineExpect> errorRule) {
            m_connection = connection;
            m_emulatorEngine = engine;
            m_shellCommandEmulator = emulator;
            m_errorRule = errorRule;
        }

        //=========================================================================================
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]buffer        受信したデータのバッファ
        // 　　　　[out]result       結果のバッファ領域を返す変数
        // 　　　　[out]completed    解析が完了したときtrue
        // 　　　　[out]remainBytes  受信データを仮想画面に転送する内容（エラーのときnull）
        // 戻り値：エラーコード
        //=========================================================================================
        public ShellEngineError OnReceive(byte[] buffer, out BufferRange result, out bool completed, out byte[] remainBytes) {
            // 行に分解
            // 最後が改行だと、split.Offsetがbufferの領域を超える点に注意
            BufferRange[] split = CommandReplyChecker.ParseLinePosiiton(buffer, m_shellCommandEmulator.ReturnCharReceive);
            int startLine = 0;
            for (int i = 0; i < split.Length - 1; i++) {
                if (!m_skippedCommandEcho) {
                    // コマンドのエコーをスキップ
                    // リクエスト文字列と同じものが先頭行で受信しているはず
                    string strCommand = m_emulatorEngine.GetRequest();
                    byte[] commandEcho = m_connection.AuthenticateSetting.Encoding.GetBytes(strCommand);
                    int echoNext = CommandReplyChecker.CheckCommandEcho(buffer, split[i].Offset, split[i].Length, commandEcho);
                    if (echoNext == CommandReplyChecker.ECHO_NOT_MATCH) {
                        // 先頭がコマンドエコーではない
                        result = BufferRange.Empty;
                        completed = false;
                        remainBytes = new byte[0];
                        return ShellEngineError.Fail(FileOperationStatus.Fail, Resources.ShellError_CommandEcho, m_connection.AuthenticateSetting.Encoding.GetString(buffer, split[0].Offset, split[0].Length));
                    } else if (echoNext == CommandReplyChecker.ECHO_SHOTAGE) {
                        result = new BufferRange(0, 0);
                        completed = false;
                        remainBytes = ArrayUtils.CreateCleanedBuffer<byte>(buffer, 0, buffer.Length);
                        return ShellEngineError.Success();
                    }
                    m_skippedCommandEcho = true;
                    startLine = i + 1;
                } else if (!m_parsedErrorLine) {
                    // エラーの可能性をチェック
                    string strLine = ResultLineParser.BytesToString(buffer, split[i].Offset, split[i].Length, m_connection.AuthenticateSetting.Encoding);
                    List<ResultLineParser.ParsedToken> errorLineResult;
                    int parsePos = 0;
                    OSSpecLineType resultLineType;
                    int hitRule;
                    ResultLineParser.ParseEachLine(strLine, m_errorRule, ref parsePos, out hitRule, out errorLineResult, out resultLineType);
                    if (hitRule != -1) {
                        result = BufferRange.Empty;
                        completed = false;
                        remainBytes = new byte[0];
                        return ShellEngineError.Fail(FileOperationStatus.Fail, Resources.ShellError_ErrorLine, strLine);
                    }
                    m_parsedErrorLine = true;       // 解析済み、この行はデータ行
                } else {
                    break;
                }
            }

            // まだコマンドエコーを受信し切れていないかチェック
            if (!m_skippedCommandEcho) {
                result = new BufferRange(0, 0);
                completed = false;
                remainBytes = ArrayUtils.CreateCleanedBuffer<byte>(buffer, 0, buffer.Length);
                return ShellEngineError.Success();
            }

            // 最終行の解析
            Encoding encoding = m_connection.AuthenticateSetting.Encoding;
            byte[] prompt = encoding.GetBytes(m_shellCommandEmulator.PromptString);
            int lastLine = split.Length - 1;
            int promptLength, nextCheckIndex;
            string userServer;
            bool found = CommandReplyChecker.CheckPrompt(encoding, buffer, split[lastLine].Offset, split[lastLine].Length, prompt, false, out userServer, out promptLength, out nextCheckIndex);
            if (found) {
                // 最終行でプロンプトを発見したら終了
                result = new BufferRange(split[startLine].Offset, nextCheckIndex - promptLength - split[startLine].Offset);
                completed = true;
                remainBytes = new byte[0];
                m_shellCommandEmulator.ShellChannel.ActiveUserServer = userServer;
            } else if (nextCheckIndex == split[lastLine].Offset) {
                // 最終行がプロンプトより短い場合はまだ受信し切れていない
                result = new BufferRange(split[startLine].Offset, split[lastLine].Offset - split[startLine].Offset);
                completed = false;
                remainBytes = ArrayUtils.CreateCleanedBuffer<byte>(buffer, split[lastLine].Offset, buffer.Length - split[lastLine].Offset);
            } else {
                // 最終行からプロンプト分を残して確定
                result = new BufferRange(split[startLine].Offset, nextCheckIndex - split[startLine].Offset);
                completed = false;
                remainBytes = ArrayUtils.CreateCleanedBuffer<byte>(buffer, nextCheckIndex, split[lastLine].Length - (nextCheckIndex - split[lastLine].Offset));
            }
            return ShellEngineError.Success();
        }
    }
}
