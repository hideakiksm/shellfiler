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
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Shell;
using ShellFiler.UI;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：アップロード処理を実行するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineUpload : IShellEmulatorEngine {
        // ヒアドキュメントのEOFマーカー
        private const string EOF_MARKER = "ShellFilerEOF";

        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // 行内解析の共通処理
        private ResultLineParser m_resultLineParser;

        // 転送元ファイル名のフルパス（Windowsパス）
        private string m_srcPhysicalPath;

        // 転送先ファイル名のフルパス（サーバー内ローカルパス）
        private string m_destFilePath;

        // 転送元のストリーム（処理終了後は外部で閉じる）
        private Stream m_srcStream;

        // CRCの計算用
        private CksumCRC32 m_cksumCrc;

        // 進捗状態を通知するdelegate
        private FileProgressEventHandler m_progress;

        // 全ファイルサイズ
        private long m_totalFileSize;

        // 全転送済みサイズ
        private long m_totalTransferred;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent           所有オブジェクト
        // 　　　　[in]connection       SSHの接続
        // 　　　　[in]srcPhysicalPath  転送元ファイル名のフルパス（Windowsパス）
        // 　　　　[in]destFilePath     転送先ファイル名のフルパス（サーバー内ローカルパス）
        // 　　　　[in]srcStream        転送元のストリーム（処理終了後は外部で閉じる）
        // 　　　　[in]progress         進捗状態を通知するdelegate
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineUpload(ShellCommandEmulator parent, SSHConnection connection, string srcPhysicalPath, string destFilePath, Stream srcStream, FileProgressEventHandler progress) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_srcPhysicalPath = srcPhysicalPath;
            m_destFilePath = destFilePath;
            m_srcStream = srcStream;
            m_cksumCrc = new CksumCRC32();
            m_progress = progress;

            m_resultLineParser = new ResultLineParser(m_connection, this, m_shellCommandEmulator, m_connection.ShellCommandDictionary.ExpectUpload);
            m_totalFileSize = m_srcStream.Length;
            m_totalTransferred = 0;
        }

        //=========================================================================================
        // 機　能：エンジンの処理を閉じる
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンド処理スレッドで実行される
        //=========================================================================================
        public void CloseEngine() {
        }

        //=========================================================================================
        // 機　能：コマンドラインに送信する文字列を取得する
        // 引　数：なし
        // 戻り値：送信するコマンドライン
        // メ　モ：バックグラウンド処理スレッドで実行される
        //=========================================================================================
        public string GetRequest() {
            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            string strCommand = dictionary.GetCommandUpload(EOF_MARKER, m_destFilePath);
            return strCommand;
        }

        //=========================================================================================
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]buffer   受信したデータのバッファ
        //         [in]remain   受信データを仮想画面に転送する内容（エラーのときnull）
        // 戻り値：エラーコード
        // メ　モ：ストリームスレッドで実行される
        //=========================================================================================
        public ShellEngineError OnReceive(byte[] buffer, out byte[] remain) {
            // 受信バイト列を解析
            ShellEngineError error;
            List<ResultLineParser.ParsedLine> parsedLine;
            bool completed;
            error = m_resultLineParser.OnReceive(buffer, out parsedLine, out completed, out remain);
            if (error.IsFailed) {
                return error;
            }

            // 期待値（エラー）を解析
            for (int i = 0; i < parsedLine.Count; i++) {
                ResultLineParser.ParsedLine line = parsedLine[i];
                bool success = ParseResult(line.LineString, line.ParsedToken, line.LineType);
                if (!success) {
                    return ShellEngineError.Fail(FileOperationStatus.Fail, Resources.ShellError_LineParse_OSSPEC, line.LineString);
                }
            }

            // ヒアドキュメントのプロンプトの応答
            if (remain.Length > 0) {
                bool found = CheckPrompt(remain);
                if (found) {
                    remain = new byte[0];
                    FileOperationStatus status = SendChunk();
                    if (!status.Succeeded) {
                        return ShellEngineError.Fail(status, null, null);
                    }
                }
            }

            if (completed) {
                m_cksumCrc.EndData();
                m_shellCommandEmulator.RemoveEngine(FileOperationStatus.Success);
            }
            return ShellEngineError.Success();
        }

        //=========================================================================================
        // 機　能：SSHからのデータが受信できたときの処理を行う
        // 引　数：[in]line            解析対象の行
        // 　　　　[in]parseResult     解析の結果抽出したデータ
        // 　　　　[in]resultLineType  結果行の種類
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParseResult(string line, List<ResultLineParser.ParsedToken> parseResult, OSSpecLineType resultLineType) {
            // 無意味な行なら次へ
            if ((resultLineType & OSSpecLineType.Comment) != 0) {
                return true;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ヒアドキュメントのプロンプトの応答を処理する
        // 引　数：[in]buffer   受信データ（行の先頭から）
        // 戻り値：ヒアドキュメントのプロンプトを検出したときtrue
        //=========================================================================================
        private bool CheckPrompt(byte[] buffer) {
            if (!m_resultLineParser.SkippedCommandEcho) {
                return false;
            }
            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            string strCommand = dictionary.ValueUploadHearDocument;
            byte[] expectedPrompt = m_connection.AuthenticateSetting.Encoding.GetBytes(strCommand);
            if (!ArrayUtils.CompareByteArray(expectedPrompt, 0, expectedPrompt.Length, buffer, 0, buffer.Length)) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：チャンクを送信する
        // 引　数：なし
        // 戻り値：ステータス（送信元の読み込み）
        //=========================================================================================
        private FileOperationStatus SendChunk() {
            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            ShellUploadEncoding uploadEncoding = dictionary.ValueUploadEncoding;
            int chunkSize = dictionary.ValueUploadChunkSize;

            // 転送元から読み込む
            byte[] rawChunk = new byte[chunkSize];
            int readSize;
            try {
                readSize = m_srcStream.Read(rawChunk, 0, chunkSize);
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }
            m_cksumCrc.AddDataChunk(rawChunk, 0, readSize);
            m_totalTransferred += readSize;
            m_progress.SetProgress(this, new FileProgressEventArgs(m_totalFileSize, m_totalTransferred));

            byte[] uploadData = null;
            if (readSize == 0) {
                // すべて読み込んだ
                byte[] eofMarker = m_connection.AuthenticateSetting.Encoding.GetBytes(EOF_MARKER);
                uploadData = new byte[eofMarker.Length + 1];
                Array.Copy(eofMarker, 0, uploadData, 0, eofMarker.Length);
                uploadData[uploadData.Length - 1] = (byte)'\n';
            } else {
                // 読み込んだチャンクを送信形式に変換
                switch (uploadEncoding) {
                    case ShellUploadEncoding.HexStream:
                        uploadData = ConvertDataChunkHex(rawChunk, 0, readSize);
                        break;
                    default:
                        Program.Abort("エンコード方式が不正です。encoding={0}", uploadEncoding);
                        break;
                }
            }
            m_shellCommandEmulator.WindowsToSshSendData(uploadData, 0, uploadData.Length);
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：16進数の並びとして送信データを用意する
        // 引　数：[in]buffer   送信データ
        // 　　　　[in]start    送信データの開始位置
        // 　　　　[in]length   送信データの長さ
        // 戻り値：用意した送信データ
        //=========================================================================================
        private byte[] ConvertDataChunkHex(byte[] buffer, int start, int length) {
            byte[] result = new byte[length * 2 + 1];
            int index = 0;
            for (int i = 0; i < length; i++) {
                int high = ((int)(buffer[i + start]) >> 4) & 0xf;
                int low = buffer[i + start] & 0xf;
                if (high >= 10) {
                    result[index] = (byte)('A' + high - 10);
                } else {
                    result[index] = (byte)('0' + high);
                }
                index++;
                if (low >= 10) {
                    result[index] = (byte)('A' + low - 10);
                } else {
                    result[index] = (byte)('0' + low);
                }
                index++;
            }
            result[index] = (byte)'\r';
            return result;
        }

        //=========================================================================================
        // プロパティ：計算済みのCRC
        //=========================================================================================
        public CksumCRC32 CksumCrc {
            get {
                return m_cksumCrc;
            }
        }
    }
}
