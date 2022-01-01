using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using Microsoft.Win32.SafeHandles;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.FileTask.FileFilter {

    //=========================================================================================
    // クラス：外部プロセスを使用するカスタムフィルター
    //=========================================================================================
    public class FileFilterCustom : IFileFilterComponent {
        // プロパティ
        private const string PROP_COMMAND_LINE = "CommandLine";     // コマンドライン文字列
        private const string PROP_INPUT = "Input";                  // 入力方法
        private const string PROP_INPUT_FILE = "InputFile";         // 入力方法：ファイル
        private const string PROP_INPUT_STDIN = "InputStdIn";       // 入力方法：標準入力
        private const string PROP_OUTPUT = "Output";                // 出力方法
        private const string PROP_OUTPUT_FILE = "OutputFile";       // 出力方法：ファイル
        private const string PROP_OUTPUT_STDOUT = "OutputStdOut";   // 出力方法：標準出力
        private const string PROP_ON_STDERR = "OnStdErr";           // 標準エラー出力検出時
        private const string PROP_ON_STDERR_STOP = "Stop";          // 標準エラー出力検出時：停止
        private const string PROP_ON_STDERR_SKIP = "Skip";          // 標準エラー出力検出時：スキップ
        private const string PROP_ON_STDERR_IGNORE = "Ignore";      // 標準エラー出力検出時：無視

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterCustom() {
        }

        //=========================================================================================
        // 機　能：現在の設定に対する表示用のパラメータを取得する
        // 引　数：[in]single  パラメータ情報を1行で作成するときtrue
        // 　　　　[in]param   パラメータ
        // 戻り値：表示用のパラメータ
        //=========================================================================================
        public string[] GetDisplayParameter(bool single, Dictionary<string, object> param) {
            // コマンドライン
            string dispCommand;
            string strCommand = (string)param[PROP_COMMAND_LINE];
            dispCommand = GenericFileStringUtils.GetFileName(GenericFileStringUtils.GetCommandFilePath(strCommand));
            
            // 入出力
            string dispInOut;
            string strInput = (string)param[PROP_INPUT];
            string strOutput = (string)param[PROP_OUTPUT];
            if (strInput == PROP_INPUT_FILE && strOutput == PROP_OUTPUT_FILE) {
                dispInOut = Resources.FileFilter_CustomIOFileFile;
            } else if (strInput == PROP_INPUT_FILE && strOutput == PROP_OUTPUT_STDOUT) {
                dispInOut = Resources.FileFilter_CustomIOFileStdOut;
            } else if (strInput == PROP_INPUT_STDIN && strOutput == PROP_OUTPUT_FILE) {
                dispInOut = Resources.FileFilter_CustomIOStdInFile;
            } else {
                dispInOut = Resources.FileFilter_CustomIOStdInStdOut;
            }

            // エラー検出時
            string dispError;
            string stdError = (string)param[PROP_ON_STDERR];
            if (stdError == PROP_ON_STDERR_STOP) {
                dispError = Resources.FileFilter_CustomErrorStop;
            } else if (stdError == PROP_ON_STDERR_SKIP) {
                dispError = Resources.FileFilter_CustomErrorSkip;
            } else {
                dispError = Resources.FileFilter_CustomErrorIgnore;
            }
            if (single) {
                return new string[] { dispCommand };
            } else {
                return new string[] { dispCommand, dispInOut, dispError };
            }
        }
        
        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public FileFilterItem CreateFileFilterItem() {
            FileFilterItem item = new FileFilterItem();
            item.FileFilterClassPath = this.GetType().FullName;
            item.PropertyList.Add(PROP_COMMAND_LINE, "command /in:$I /out:$O");
            item.PropertyList.Add(PROP_INPUT, PROP_INPUT_FILE);
            item.PropertyList.Add(PROP_OUTPUT, PROP_OUTPUT_FILE);
            item.PropertyList.Add(PROP_ON_STDERR, PROP_ON_STDERR_STOP);
            return item;
        }
        
        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public List<SettingUIItem> GetSettingUI() {
            List<SettingUIItem> itemList = new List<SettingUIItem>();
            itemList.Add(new SettingUIItem.TextBox(Resources.FileFilter_CustomUI01Text, PROP_COMMAND_LINE));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_CustomUI02Text, 1));
            itemList.Add(new SettingUIItem.Space());
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_CustomUI03Combo, PROP_INPUT,
                                        new string[] { Resources.FileFilter_CustomUI03ComboFile, Resources.FileFilter_CustomUI03ComboStdIn },
                                        new string[] { PROP_INPUT_FILE, PROP_INPUT_STDIN }));
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_CustomUI04Combo, PROP_OUTPUT,
                                        new string[] { Resources.FileFilter_CustomUI04ComboFile, Resources.FileFilter_CustomUI04ComboStdIn },
                                        new string[] { PROP_OUTPUT_FILE, PROP_OUTPUT_STDOUT }));
            itemList.Add(new SettingUIItem.Space());
            itemList.Add(new SettingUIItem.Combobox(Resources.FileFilter_CustomUI05Combo, PROP_ON_STDERR,
                                        new string[] { Resources.FileFilter_CustomUI05ComboStop, Resources.FileFilter_CustomUI05ComboSkip, Resources.FileFilter_CustomUI05ComboIgnore },
                                        new string[] { PROP_ON_STDERR_STOP, PROP_ON_STDERR_SKIP, PROP_ON_STDERR_IGNORE }));
            return itemList;
        }

        //=========================================================================================
        // 機　能：パラメータが正しいかどうかを確認する
        // 引　数：[in]param   確認するパラメータ
        // 戻り値：エラーメッセージ（エラーがないときnull）
        //=========================================================================================
        public string CheckParameter(Dictionary<string, object> param) {
            // コマンドライン
            if (!param.ContainsKey(PROP_COMMAND_LINE) || !(param[PROP_COMMAND_LINE] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }

            // 入力方法
            if (!param.ContainsKey(PROP_INPUT) || !(param[PROP_INPUT] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string input = (string)(param[PROP_INPUT]);
            if (input != PROP_INPUT_FILE && input != PROP_INPUT_STDIN) {
                return Resources.FileFilter_MsgSerializeError;
            }
            
            // 出力方法
            if (!param.ContainsKey(PROP_OUTPUT) || !(param[PROP_OUTPUT] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string output = (string)(param[PROP_OUTPUT]);
            if (output != PROP_OUTPUT_FILE && output != PROP_OUTPUT_STDOUT) {
                return Resources.FileFilter_MsgSerializeError;
            }

            // 標準エラー出力検出時
            if (!param.ContainsKey(PROP_ON_STDERR) || !(param[PROP_ON_STDERR] is string)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            string stderr = (string)(param[PROP_ON_STDERR]);
            if (stderr != PROP_ON_STDERR_STOP && stderr != PROP_ON_STDERR_SKIP && stderr != PROP_ON_STDERR_IGNORE) {
                return Resources.FileFilter_MsgSerializeError;
            }

            // コマンド引数のチェック
            string commandTemplate = (string)param[PROP_COMMAND_LINE];
            string program;
            string argumentTemplate;
            GenericFileStringUtils.SplitCommandLine(commandTemplate, out program, out argumentTemplate);

            string inputFile = (input == PROP_INPUT_FILE) ? "a" : null;
            string outputFile = (output == PROP_OUTPUT_FILE) ? "a" : null;
            CommandArgumentConverter converter = new CommandArgumentConverter(inputFile, outputFile);
            string argument;
            bool success = converter.ParseCommandLine(argumentTemplate, out argument);
            if (!success) {
                return converter.Error;
            }

            return null;
        }

        //=========================================================================================
        // 機　能：変換を実行する
        // 引　数：[in]orgFileName 元のファイルパス（クリップボードのときnull）
        // 　　　　[in]src         変換元のバイト列
        // 　　　　[out]dest       変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 　　　　[in]param       変換に使用するパラメータ
        // 　　　　[in]cancelEvent キャンセル時にシグナル状態になるイベント
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Convert(string orgFileName, byte[] src, out byte[] dest, Dictionary<string, object> param, WaitHandle cancelEvent) {
            FileOperationStatus status;

            // パラメータを取得
            string commandLine = (string)param[PROP_COMMAND_LINE];
            bool isInputFile = (((string)param[PROP_INPUT]) == PROP_INPUT_FILE);
            bool isOutputFile = (((string)param[PROP_OUTPUT]) == PROP_OUTPUT_FILE);
            StdErrMode stdErrMode = GetStdErrMode((string)param[PROP_ON_STDERR]);

            Converter converter = new Converter(orgFileName, commandLine, isInputFile, isOutputFile, stdErrMode);
            try {
                status = converter.Execute(src, out dest, cancelEvent);
            } finally {
                converter.Dispose();
            }
            return status;
        }

        //=========================================================================================
        // 機　能：文字列からStdErrの処理方法を返す
        // 引　数：[in]strMode   StdErrの処理方法の文字列表現
        // 戻り値：StdErrの処理方法
        //=========================================================================================
        private StdErrMode GetStdErrMode(string strMode) {
            StdErrMode mode;
            if (strMode == PROP_ON_STDERR_STOP) {
                mode = StdErrMode.Stop;
            } else if (strMode == PROP_ON_STDERR_SKIP) {
                mode = StdErrMode.Skip;
            } else {
                mode = StdErrMode.Ignore;
            }
            return mode;
        }

        //=========================================================================================
        // プロパティ：フィルターの表示名
        //=========================================================================================
        public string FilterName {
            get {
                return Resources.FileFilter_CustomConvertName;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの説明文
        //=========================================================================================
        public string FilterExplain {
            get {
                return Resources.FileFilter_CustomConvertExp;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドの引数のテンプレートを実際の引数に変換するクラス
        //=========================================================================================
        private class CommandArgumentConverter {
            // 入力ファイル名（使用しないときnull）
            private string m_inputFile;

            // 出力ファイル名（使用しないときnull）
            private string m_outputFile;

            // 入力ファイルを使用したときtrue
            private bool m_useInputFile = false;

            // 出力ファイルを使用したときtrue
            private bool m_useOutputFile = false;

            // エラーメッセージ（エラーがないときnull）
            private string m_error = null;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]inputFile   入力ファイル名（使用しないときnull）
            // 　　　　[in]outputFile  出力ファイル名（使用しないときnull）
            // 戻り値：解析に成功したときtrue
            //=========================================================================================
            public CommandArgumentConverter(string inputFile, string outputFile) {
                m_inputFile = GenericFileStringUtils.CompleteQuoteFileName(inputFile);
                m_outputFile = GenericFileStringUtils.CompleteQuoteFileName(outputFile);
            }

            //=========================================================================================
            // 機　能：解析を実行する
            // 引　数：[in]argument  ユーザーが指定したマクロ付きの引数
            // 　　　　[out]result   テンプレートの$内を置換した内容を返す変数
            // 戻り値：解析に成功したときtrue（falseのときErrorプロパティが有効）
            //=========================================================================================
            public bool ParseCommandLine(string argument, out string result) {
                result = null;
                bool success;
                int index = 0;
                StringBuilder sbResult = new StringBuilder();
                while (index < argument.Length) {
                    char ch = argument[index];
                    if (ch == '$') {
                        // $in/$out/$$を解析
                        index++;
                        success = ConvertToken(argument, ref index, sbResult);
                        if (!success) {
                            return false;
                        }
                    } else {
                        // 通常の文字列部分
                        sbResult.Append(ch);
                        index++;
                    }
                }

                // 最終確認
                if (m_inputFile != null && !m_useInputFile) {
                    m_error = Resources.FileFilter_CustomArgErrorUnuseIn;
                    return false;
                } else if (m_outputFile != null && !m_useOutputFile) {
                    m_error = Resources.FileFilter_CustomArgErrorUnuseOut;
                    return false;
                }
                result = sbResult.ToString();
                return true;
            }

            //=========================================================================================
            // 機　能：「$」直後からのトークンの文字列を変換する
            // 引　数：[in]argument    ユーザーが指定したマクロ付きの引数
            // 　　　　[in,out]index   解析位置（$の直後、新しい位置を返す）
            // 　　　　[in]sbResult    変換結果を追加する文字列
            // 戻り値：解析に成功したときtrue
            //=========================================================================================
            private bool ConvertToken(string argument, ref int index, StringBuilder sbResult) {
                if (index < argument.Length && argument[index] == 'I') {
                    if (m_useInputFile) {
                        m_error = Resources.FileFilter_CustomArgErrorMultiIn;
                        return false;
                    } else if (m_inputFile != null) {
                        sbResult.Append(m_inputFile);
                        index++;
                        m_useInputFile = true;
                        return true;
                    } else {
                        m_error = Resources.FileFilter_CustomArgErrorInput;
                        return false;
                    }
                } else if (index < argument.Length && argument[index] == 'O') {
                    if (m_useOutputFile) {
                        m_error = Resources.FileFilter_CustomArgErrorMultiOut;
                        return false;
                    } else if (m_outputFile != null) {
                        sbResult.Append(m_outputFile);
                        index++;
                        m_useOutputFile = true;
                        return true;
                    } else {
                        m_error = Resources.FileFilter_CustomArgErrorOutput;
                        return false;
                    }
                } else if (index < argument.Length && argument[index] == '$') {
                    sbResult.Append('$');
                    index++;
                    return true;
                } else {
                    m_error = Resources.FileFilter_CustomArgErrorUnknown;
                    return false;
                }
            }

            //=========================================================================================
            // プロパティ：エラーメッセージ（エラーがないときnull）
            //=========================================================================================
            public string Error {
                get {
                    return m_error;
                }
            }
        }

        //=========================================================================================
        // クラス：フィルターでの変換処理
        //=========================================================================================
        private class Converter {
            // 非同期入出力のバッファサイズ
            public const int ASYNC_BUFFER_CHUNK_LENGTH = 4096;

            // コマンドラインのテンプレート
            private string m_commandTemplate;

            // 入力用の一時ファイル名（null:標準入力を使用）
            private string m_inputTempPath;

            // 出力用の一時ファイル名（null:標準出力を使用）
            private string m_outputTempPath;

            // StdErrの扱い
            private StdErrMode m_stdErrMode;

            // プロセスが閉じられたときtrue
            private bool m_disposed = false;

            // 起動したプロセス（null:起動していない）
            private Process m_process = null;

            // StdOut出力結果のストリーム（null:使用しない）
            private MemoryStream m_stdOutStream = null;

            // StdErr出力結果のストリーム（null:使用しない）
            private MemoryStream m_stdErrStream = null;
            
            // StdInのストリームが終了したときtrue（StdInを使用しないときtrue）
            private bool m_stdInEnd = false;

            // StdOutのストリームが終了したときtrue（StdOutを使用しないときtrue）
            private bool m_stdOutEnd = false;

            // StdErrのストリームが終了したときtrue（StdErrを使用しないときtrue）
            private bool m_stdErrEnd = false;

            // 非同期ストリームでのエラー（エラーがないときnull）
            private FileOperationStatus m_asyncStreamError = null;

            // プロセスを終了したときにシグナル状態となるイベント（非同期読み込みでStdOut/StdErrorの両方がクローズしたときシグナル）
            private ManualResetEvent m_eventProcessEnd = new ManualResetEvent(false);

            //=========================================================================================
            // 機　能：「$」直後からのトークンの文字列を変換する
            // 引　数：[in]orgFileName   元のファイルパス（クリップボードのときnull）
            // 　　　　[in]command       コマンドラインのテンプレート
            // 　　　　[in]isInputFile   入力をファイルから行うときtrue
            // 　　　　[in]isOutputFile  出力をファイルで行うときtrue
            // 　　　　[in]stdErrMode    StdErrの扱い
            // 戻り値：なし
            //=========================================================================================
            public Converter(string orgFileName, string command, bool isInputFile, bool isOutputFile, StdErrMode stdErrMode) {
                m_commandTemplate = command;
                string ext = GenericFileStringUtils.GetExtensionLast(GenericFileStringUtils.GetFileName(orgFileName));
                if (isInputFile) {
                    m_inputTempPath = Program.Document.TemporaryManager.GetTemporaryFile() + "." + ext;
                } else {
                    m_inputTempPath = null;
                }
                if (isOutputFile) {
                    m_outputTempPath = Program.Document.TemporaryManager.GetTemporaryFile() + "." + ext;
                } else {
                    m_outputTempPath = null;
                }
                m_stdErrMode = stdErrMode;
            }

            //=========================================================================================
            // 機　能：後処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Dispose() {
                // 使用したテンポラリを削除
                if (m_inputTempPath != null) {
                    try {
                        File.Delete(m_inputTempPath);
                    } catch (Exception) {
                    }
                    m_inputTempPath = null;
                }
                if (m_outputTempPath != null) {
                    try {
                        File.Delete(m_outputTempPath);
                    } catch (Exception) {
                    }
                    m_outputTempPath = null;
                }

                // プロセス
                if (m_process != null) {
                    m_disposed = true;
                    m_process.Dispose();
                    m_process = null;
                }
            }

            //=========================================================================================
            // 機　能：変換処理を実行する
            // 引　数：[in]src         変換元のバイト列
            // 　　　　[out]dest       変換先のバイト列を返す変数（変換元と同一になる可能性あり）
            // 　　　　[in]cancelEvent キャンセル時にシグナル状態になるイベント
            // 戻り値：ステータス
            //=========================================================================================
            public FileOperationStatus Execute(byte[] src, out byte[] dest, WaitHandle cancelEvent) {
                dest = null;

                // 引数を作成
                string program;
                string argumentTemplate;
                GenericFileStringUtils.SplitCommandLine(m_commandTemplate, out program, out argumentTemplate);
                string workingPath = GenericFileStringUtils.GetDirectoryName(program);
                string argument;
                CommandArgumentConverter argConverter = new CommandArgumentConverter(m_inputTempPath, m_outputTempPath);
                bool success = argConverter.ParseCommandLine(argumentTemplate, out argument);
                if (!success) {
                    // チェック済みのためエラーにならないはず
                    return FileOperationStatus.Fail;
                }

                // 入力ファイルを用意
                if (m_inputTempPath != null) {
                    try {
                        File.WriteAllBytes(m_inputTempPath, src);
                    } catch (Exception) {
                        return FileOperationStatus.FailWriteTemp;
                    }
                }

                // プロセスを起動
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = program;
                psi.WorkingDirectory = workingPath;
                psi.RedirectStandardInput = (m_inputTempPath == null);
                psi.RedirectStandardOutput = (m_outputTempPath == null);
                psi.RedirectStandardError = (m_stdErrMode != StdErrMode.Ignore);
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.Arguments = argument;

                // プロセスを起動
                try {
                    m_process = OSUtils.StartProcess(psi, workingPath);
                } catch (Exception) {
                    return FileOperationStatus.FailProcessStart;
                }

                // 出力を中継する準備
                if (m_inputTempPath == null) {
                    AsyncInputArgument argStdIn = new AsyncInputArgument();
                    argStdIn.ProcessStream = m_process.StandardInput.BaseStream;
                    argStdIn.Buffer = src;
                    argStdIn.Start = 0;
                    int length = Math.Min(src.Length, ASYNC_BUFFER_CHUNK_LENGTH);
                    argStdIn.ProcessStream.BeginWrite(argStdIn.Buffer, 0, length, StandardInputCallback, argStdIn);
                    argStdIn.Start += length;
                } else {
                    m_stdInEnd = true;
                }

                if (m_outputTempPath == null) {
                    byte[] stdOutBuffer = new byte[ASYNC_BUFFER_CHUNK_LENGTH];
                    m_stdOutStream = new MemoryStream();
                    AsyncOutputArgument argStdOut = new AsyncOutputArgument();
                    argStdOut.ProcessStream = m_process.StandardOutput.BaseStream;
                    argStdOut.ResultStream = m_stdOutStream;
                    argStdOut.Buffer = stdOutBuffer;
                    argStdOut.IsStdOut = true;
                    argStdOut.ProcessStream.BeginRead(stdOutBuffer, 0, stdOutBuffer.Length, StandardOutputCallback, argStdOut);
                } else {
                    m_stdOutEnd = true;
                }

                if (m_stdErrMode != StdErrMode.Ignore) {
                    byte[] stdErrBuffer = new byte[ASYNC_BUFFER_CHUNK_LENGTH];
                    m_stdErrStream = new MemoryStream();
                    AsyncOutputArgument argStdErr = new AsyncOutputArgument();
                    argStdErr.ProcessStream = m_process.StandardError.BaseStream;
                    argStdErr.ResultStream = m_stdErrStream;
                    argStdErr.Buffer = stdErrBuffer;
                    argStdErr.IsStdOut = false;
                    argStdErr.ProcessStream.BeginRead(stdErrBuffer, 0, stdErrBuffer.Length, StandardOutputCallback, argStdErr);
                } else {
                    m_stdErrEnd = true;
                }

                // 終了待ち
                WaitHandle[] waitEventList;
                if (m_stdInEnd && m_stdOutEnd && m_stdErrEnd) {
                    ManualResetEvent processEvent = new ManualResetEvent(false);        // Dispose不要
                    processEvent.SafeWaitHandle = new SafeWaitHandle(m_process.Handle, false);
                    waitEventList = new WaitHandle[] { processEvent, cancelEvent };
                } else {
                    waitEventList = new WaitHandle[] { m_eventProcessEnd, cancelEvent };
                }
                int index = WaitHandle.WaitAny(waitEventList);
                if (index != 0) {
                    return FileOperationStatus.Canceled;
                }
                if (m_asyncStreamError != null) {
                    m_stdOutStream = null;
                    m_stdErrStream = null;
                    return m_asyncStreamError;
                }

                // StdErrのチェック
                if (m_stdErrStream != null) {
                    m_stdErrStream.Close();
                    if (m_stdErrStream.ToArray().Length > 0) {
                        // エラー発生
                        if (m_stdErrMode == StdErrMode.Skip) {
                            dest = src;
                            return FileOperationStatus.Success;
                        } else if (m_stdErrMode == StdErrMode.Stop) {
                            return FileOperationStatus.FailProcessError;
                        }
                    }
                }

                // 結果の読み込み
                if (m_outputTempPath != null) {
                    try {
                        dest = File.ReadAllBytes(m_outputTempPath);
                    } catch (Exception) {
                        return FileOperationStatus.FailReadTemp;
                    }
                } else {
                    m_stdOutStream.Close();
                    dest = m_stdOutStream.ToArray();
                }

                return FileOperationStatus.Success;
            }

            //=========================================================================================
            // 機　能：非同期書き込みのコールバック
            // 引　数：[in]result  非同期読み込みの結果情報
            // 戻り値：なし
            // メ　モ：非同期読み込みスレッドで実行される
            //=========================================================================================
            private void StandardInputCallback(IAsyncResult result) {
                try {
                    if (m_disposed) {
                        return;
                    }
                    AsyncInputArgument arg = (AsyncInputArgument)(result.AsyncState);
                    arg.ProcessStream.EndWrite(result);
                    if (!m_process.HasExited) {
                        int length = Math.Min(arg.Buffer.Length - arg.Start, ASYNC_BUFFER_CHUNK_LENGTH);
                        if (length == 0) {
                            arg.ProcessStream.Close();
                            m_stdInEnd = true;
                            if (m_stdInEnd && m_stdOutEnd && m_stdErrEnd) {
                                m_eventProcessEnd.Set();
                            }
                        } else {
                            arg.ProcessStream.BeginWrite(arg.Buffer, arg.Start, length, StandardInputCallback, arg);
                            arg.Start += length;
                        }
                    }
                } catch (OutOfMemoryException) {
                    m_asyncStreamError = FileOperationStatus.ErrorOutOfMemory;
                    m_eventProcessEnd.Set();
                } catch (ObjectDisposedException) {
                    // キャンセル時：発生しても無視
                }
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
                    AsyncOutputArgument arg = (AsyncOutputArgument)(result.AsyncState);
                    int count = arg.ProcessStream.EndRead(result);
                    if (count > 0) {
                        arg.ResultStream.Write(arg.Buffer, 0, count);
                        arg.ProcessStream.BeginRead(arg.Buffer, 0, arg.Buffer.Length, StandardOutputCallback, arg);
                    } else {
                        if (arg.IsStdOut) {
                            m_stdOutEnd = true;
                        } else {
                            m_stdErrEnd = true;
                        }
                        if (m_stdInEnd && m_stdOutEnd && m_stdErrEnd) {
                            m_eventProcessEnd.Set();
                        }
                    }
                } catch (OutOfMemoryException) {
                    m_asyncStreamError = FileOperationStatus.ErrorOutOfMemory;
                    m_eventProcessEnd.Set();
                } catch (ObjectDisposedException) {
                    // キャンセル時：発生しても無視
                }
            }
        }

        //=========================================================================================
        // クラス：StdInの非同期書き込みに使うパラメータ
        //=========================================================================================
        private class AsyncInputArgument {
            // プロセスに入力するStdInのストリーム
            public Stream ProcessStream;
            
            // 入力データのバッファ（全体）
            public byte[] Buffer;

            // 直前までの入力開始位置
            public int Start;
        }

        //=========================================================================================
        // クラス：StdOut/StdErrの非同期読み込みに使うパラメータ
        //=========================================================================================
        private class AsyncOutputArgument {
            // プロセスから出力されたStdOut/StdErrorのストリーム
            public Stream ProcessStream;
            
            // 結果を出力するストリーム
            public MemoryStream ResultStream;

            // 非同期読み込みに使用するバッファ
            public byte[] Buffer;

            // 標準出力を扱うときtrue/標準エラー出力を扱うときfalse
            public bool IsStdOut;
        }

        //=========================================================================================
        // 列挙子：StdErrの処理方法
        //=========================================================================================
        private enum StdErrMode {
            Stop,                   // 停止
            Skip,                   // このフィルターをスキップ
            Ignore,                 // StdErrを無視
        }
    }
}
