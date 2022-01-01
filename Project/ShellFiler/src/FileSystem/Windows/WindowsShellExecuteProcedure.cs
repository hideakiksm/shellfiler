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
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：コマンドの実行を行うProcedure
    //=========================================================================================
    class WindowsShellExecuteProcedure {
        // プロセス終了の1回分の待ち時間[ms]
        private const int EXIT_WAIT_TIME = 200;

        // プロセスが閉じられたときtrue
        private bool m_disposed = false;

        // ストリームから受け取った後の文字列（標準出力／標準エラー出力混在）
        private List<LogLineInfo> m_listReceive = new List<LogLineInfo>();

        // m_listReceiveに新しい文字列を追加したときにシグナル状態となるイベント
        private AutoResetEvent m_eventReceived = new AutoResetEvent(false);

        // プロセス監視でStdOutのストリームが終了したときtrue
        private bool m_stdOutEnd = false;

        // プロセス監視でStdErrorのストリームが終了したときtrue
        private bool m_stdErrorEnd = false;

        // プロセスを終了したときにシグナル状態となるイベント（非同期読み込みでStdOut/StdErrorの両方がクローズしたときシグナル）
        private ManualResetEvent m_eventProcessEnd = new ManualResetEvent(false);

        // プロセスの出力を文字列化する際のエンコーディング
        private Encoding m_encoding = Encoding.GetEncoding(932);
        
        // StdOutの読み込み中バッファ
        private byte[] m_stdOutBuffer = new byte[0];

        // StdErrorの読み込み中バッファ
        private byte[] m_stdErrorBuffer = new byte[0];

        // 取得したログデータの書き込み先
        private IRetrieveFileDataTarget m_dataTarget;

        //=========================================================================================
        // 機　能：指定されたコマンドを実行する
        // 引　数：[in]cache      キャッシュ情報
        // 　　　　[in]dirName    カレントディレクトリ名
        // 　　　　[in]command    コマンドライン
        // 　　　　[in]relayLog   標準出力の結果をログ出力するときtrue
        // 　　　　[in]dataTarget 取得データの格納先
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Execute(FileOperationRequestContext cache, string dirName, string command, bool relayLog, IRetrieveFileDataTarget dataTarget) {
            if (relayLog) {
                return ExecuteWithLog(cache, dirName, command, dataTarget);
            } else {
                return ExecuteIsolate(dirName, command);
            }
        }

        //=========================================================================================
        // 機　能：ログ出力ありで指定されたコマンドを実行する
        // 引　数：[in]cache      キャッシュ情報
        // 　　　　[in]dirName    カレントディレクトリ名
        // 　　　　[in]command    コマンドライン
        // 　　　　[in]dataTarget 取得データの格納先
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus ExecuteWithLog(FileOperationRequestContext cache, string dirName, string command, IRetrieveFileDataTarget dataTarget) {
            // 起動の準備
            m_dataTarget = dataTarget;
            string program;
            string argument;
            GenericFileStringUtils.SplitCommandLine(command, out program, out argument);    
            string programFile = GenericFileStringUtils.GetFileName(program);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = program;
            psi.WorkingDirectory = dirName;
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.Arguments = argument;

            // プロセスを起動
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.MinValue;
            Process process;
            try {
                process = OSUtils.StartProcess(psi, dirName);
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }
            Program.LogWindow.RegistLogLineHelper(string.Format(Resources.ShellExec_ProcessStart1, programFile));

            // 出力を中継する準備
            byte[] outputBuffer = new byte[1024];
            byte[] errorBuffer = new byte[1024];

            AsyncArgument argStdOut = new AsyncArgument();
            argStdOut.Process = process;
            argStdOut.Stream = process.StandardOutput.BaseStream;
            argStdOut.Buffer = outputBuffer;
            argStdOut.IsStdOut = true;
            argStdOut.Stream.BeginRead(outputBuffer, 0, outputBuffer.Length, StandardOutputCallback, argStdOut);

            AsyncArgument argStdError = new AsyncArgument();
            argStdError.Process = process;
            argStdError.Stream = process.StandardError.BaseStream;
            argStdError.Buffer = errorBuffer;
            argStdError.IsStdOut = false;
            argStdError.Stream.BeginRead(errorBuffer, 0, errorBuffer.Length, StandardOutputCallback, argStdError);

            // 出力を中継
            while (true) {
                WaitHandle[] waitEventList = { m_eventReceived, m_eventProcessEnd, cache.CancelEvent };
                int index = WaitHandle.WaitAny(waitEventList);
                if (m_eventProcessEnd.WaitOne(0, false)) {
                    endTime = DateTime.Now;
                    index = 1;
                }
                m_dataTarget.FireEvent(false);

                // プロセス終了またはキャンセル
                if (index != 0) {
                    break;
                }
            }

            // 終了処理
            m_dataTarget.AddCompleted(RetrieveDataLoadStatus.CompletedAll, null);
            if (endTime != DateTime.MinValue) {
                int exitCode = process.ExitCode;
                double totalProcessor = process.TotalProcessorTime.TotalMilliseconds / 1000.0;
                double userProcessor = process.UserProcessorTime.TotalMilliseconds / 1000.0;
                double execTime = (endTime - startTime).TotalMilliseconds / 1000.0;
                string exitInfo1 = string.Format(Resources.ShellExec_ProcessEnd1, programFile);
                string exitInfo2 = string.Format(Resources.ShellExec_ProcessEnd2, exitCode, execTime, totalProcessor, userProcessor);
                Program.LogWindow.RegistLogLineHelper(exitInfo1);
                Program.LogWindow.RegistLogLineHelper(exitInfo2);
            } else {
                Program.LogWindow.RegistLogLineHelper(Resources.ShellExec_ProcessCancel);
            }
            m_disposed = true;
            process.Dispose();

            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：非同期読み込みのコールバック
        // 引　数：[in]result  非同期読み込みの結果情報
        // 戻り値：なし
        // メ　モ：非同期読み込みスレッドで実行される
        //=========================================================================================
        private void StandardOutputCallback(IAsyncResult result) {
            try {
                if (m_disposed) {
                    return;
                }
                AsyncArgument arg = (AsyncArgument)(result.AsyncState);
                int count = arg.Stream.EndRead(result);
                if (!arg.Process.HasExited) {
                    OnReceive(arg.Buffer, count, arg.IsStdOut, false);
                    arg.Stream.BeginRead(arg.Buffer, 0, arg.Buffer.Length, StandardOutputCallback, arg);
                } else {
                    OnReceive(arg.Buffer, count, arg.IsStdOut, true);
                    lock (this) {
                        if (arg.IsStdOut) {
                            m_stdOutEnd = true;
                        } else {
                            m_stdErrorEnd = true;
                        }
                        if (m_stdOutEnd && m_stdErrorEnd) {
                            m_eventProcessEnd.Set();
                        }
                    }
                }
            } catch (ObjectDisposedException) {
                // キャンセル時：発生しても無視
            }
        }
        
        //=========================================================================================
        // 機　能：プロセスの出力を受け取ったときの処理を行う
        // 引　数：[in]buffer   受け取った文字列のバッファ
        // 　　　　[in]count    bufferで有効な部分の長さ
        // 　　　　[in]isStdOut 標準出力に書き込むときtrue/標準エラー出力に書き込むときfalse
        // 　　　　[in]final    最後の受信のときtrue
        // 戻り値：なし
        // メ　モ：非同期読み込みスレッドで実行される
        //=========================================================================================
        private void OnReceive(byte[] buffer, int count, bool isStdOut, bool final) {
            if (isStdOut) {
                m_stdOutBuffer = ParseBuffer(m_stdOutBuffer, buffer, count, final);
            } else {
                m_stdErrorBuffer = ParseBuffer(m_stdErrorBuffer, buffer, count, final);
            }
        }

        //=========================================================================================
        // 機　能：プロセスの出力を行単位に分解して記録する（標準出力/標準エラー出力の共通化）
        // 引　数：[in]prevBuffer  直前までの解析結果の残り
        // 　　　　[in]chunkBuffer 受け取った文字列のバッファ
        // 　　　　[in]count       bufferで有効な部分の長さ
        // 　　　　[in]final       最後の受信のときtrue
        // 戻り値：次回にprevBufferとして渡す、解析中のバッファ
        //=========================================================================================
        private byte[] ParseBuffer(byte[] prevBuffer, byte[] chunkBuffer, int chunkCount, bool final) {
            byte[] newBuffer = new byte[prevBuffer.Length + chunkCount];
            Array.Copy(prevBuffer, 0, newBuffer, 0, prevBuffer.Length);
            Array.Copy(chunkBuffer, 0, newBuffer, prevBuffer.Length, chunkCount);
            int start = 0;
            int index = 0;
            while (index < newBuffer.Length) {
                if (newBuffer[index] == '\n') {
                    // 改行：直前の\r\r\nを改行1つとして解析
                    int end = index - 1;
                    if (index >= 2 && newBuffer[index-2] == '\r' && newBuffer[index-1] == '\r') {
                        end = index - 2;
                    } else if (index >= 1 && newBuffer[index-1] == '\r') {
                        end = index - 1;
                    }

                    // 1行分を文字列化してm_listReceiveに格納
                    StoreData(newBuffer, start, end - start + 1);
                    start = index + 1;
                }
                index++;
            }
            // chunkBufferにもう改行がない
            if (final) {
                // 最後の場合は残りを行に格納
                if (start != index) {
                    StoreData(newBuffer, start, newBuffer.Length - start);
                    prevBuffer = new byte[0];
                }
            } else {
                // 途中の場合は次回の解析に回す
                prevBuffer = new byte[newBuffer.Length - start];
                Array.Copy(newBuffer, start, prevBuffer, 0, prevBuffer.Length);
            }
            return prevBuffer;
        }

        //=========================================================================================
        // 機　能：取得したデータを格納する
        // 引　数：[in]buffer  取得したデータの格納バッファ
        // 　　　　[in]start   データの先頭位置
        // 　　　　[in]length  データの長さ
        // 戻り値：なし
        //=========================================================================================
        private void StoreData(byte[] buffer, int start, int length) {
            byte[] lineBuffer = new byte[length + 1];
            Array.Copy(buffer, start, lineBuffer, 0, length);
            lineBuffer[length] = (byte)'\n';
            m_dataTarget.AddData(lineBuffer, 0, lineBuffer.Length);
            m_eventReceived.Set();
        }
        
        //=========================================================================================
        // 機　能：ログ出力なしで指定されたコマンドを実行する
        // 引　数：[in]dirName  カレントディレクトリ名
        // 　　　　[in]command  コマンドライン
        // 戻り値：ステータス
        //=========================================================================================
        private FileOperationStatus ExecuteIsolate(string dirName, string command) {
            // 起動の準備
            string program;
            string argument;
            GenericFileStringUtils.SplitCommandLine(command, out program, out argument);    
            string programFile = GenericFileStringUtils.GetFileName(program);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = program;
            psi.WorkingDirectory = dirName;
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.Arguments = argument;

            // プロセスを起動
            try {
                Process process = OSUtils.StartProcess(psi, dirName);
                Program.LogWindow.RegistLogLineHelper(string.Format(Resources.ShellExec_ProcessStart2, programFile));
                process.Dispose();
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // クラス：StdOut/StdErrorの非同期読み込みに使うパラメータ
        //=========================================================================================
        private class AsyncArgument {
            // 実行中のプロセス
            public Process Process;

            // StdOut/StdErrorのストリーム
            public Stream Stream;
            
            // 非同期読み込みに使用するバッファ
            public byte[] Buffer;

            // 標準出力を扱うときtrue/標準エラー出力を扱うときfalse
            public bool IsStdOut;
        }

        //=========================================================================================
        // クラス：ログ出力にする1行分の情報
        //=========================================================================================
        private class LogLineInfo {
            // ログの出力色
            public LogColor LogColor;

            // 出力するメッセージ
            public string Message;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]logColor  ログの出力色
            // 　　　　[in]message   出力するメッセージ
            // 戻り値：なし
            //=========================================================================================
            public LogLineInfo(LogColor logColor, string message) {
                LogColor = logColor;
                Message = message;
            }
        }
     }
}
