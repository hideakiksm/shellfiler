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
using Tamir.SharpSsh.jsch;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：ダウンロード処理を実行するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineDownload : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // バイト列取得の共通処理
        private ResultBytesParser m_resultBytesParser;

        // 出力結果のクリーンナップ処理
        private ConsoleResultCleaner m_consoleResultCleaner;

        // 転送元ファイル名のフルパス（サーバー内ローカルパス）
        private string m_srcLocalPath;

        // 転送先ファイル名のフルパス（Windowsパス）
        private string m_destPhysicalPath;

        // 転送先のストリーム（処理終了後は外部で閉じる）
        private Stream m_destStream;

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
        // 　　　　[in]srcLocalPath     転送元ファイル名のフルパス（サーバー内ローカルパス）
        // 　　　　[in]destPhysicalPath 転送先ファイル名のフルパス（Windowsパス）
        // 　　　　[in]destStream       転送元のストリーム（処理終了後は外部で閉じる）
        // 　　　　[in]progress         進捗状態を通知するdelegate
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineDownload(ShellCommandEmulator parent, SSHConnection connection, string srcLocalPath, string destPhysicalPath, IFile srcFileInfo, Stream destStream, FileProgressEventHandler progress) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_srcLocalPath = srcLocalPath;
            m_destPhysicalPath = destPhysicalPath;
            m_destStream = destStream;
            m_cksumCrc = new CksumCRC32();
            m_progress = progress;
            m_totalFileSize = srcFileInfo.FileSize;
            m_totalTransferred = 0;

            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            List<OSSpecLineExpect> errorExpect = dictionary.ExpectDownload;
            m_resultBytesParser = new ResultBytesParser(m_connection, this, m_shellCommandEmulator, errorExpect);
            m_consoleResultCleaner = new ConsoleResultCleaner(dictionary.ResultCrLfConvert);
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
            string strCommand = dictionary.GetCommandDownload(m_srcLocalPath);
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
            ShellEngineError error;
            BufferRange receivedData;
            bool completed;
            error = m_resultBytesParser.OnReceive(buffer, out receivedData, out completed, out remain);
            if (error.IsFailed) {
                return error;
            }

            if (receivedData.Length > 0) {
                byte[] cleaned = m_consoleResultCleaner.CleanReceivedData(buffer, receivedData.Offset, receivedData.Length, completed);
                try {
                    m_destStream.Write(cleaned, 0, cleaned.Length);
                    m_cksumCrc.AddDataChunk(cleaned, 0, cleaned.Length);
                    m_totalTransferred += cleaned.Length;
                    m_progress.SetProgress(this, new FileProgressEventArgs(m_totalFileSize, m_totalTransferred));
                } catch (Exception) {
                    return ShellEngineError.Fail(FileOperationStatus.Fail, null, null);
                }
            }

            if (completed) {
                m_cksumCrc.EndData();
                m_shellCommandEmulator.RemoveEngine(FileOperationStatus.Success);
            }
            return ShellEngineError.Success();
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
