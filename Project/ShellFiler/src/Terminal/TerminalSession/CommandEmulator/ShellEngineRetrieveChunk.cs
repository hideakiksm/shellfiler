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
    // クラス：ファイル名一覧を取得するエミュレータエンジン
    //=========================================================================================
    public class ShellEngineRetrieveChunk : IShellEmulatorEngine {
        // 所有オブジェクト
        private ShellCommandEmulator m_shellCommandEmulator;

        // リクエストのコンテキスト情報
        private SSHConnection m_connection;

        // バイト列取得の共通処理
        private ResultBytesParser m_resultChunkParser;

        // 出力結果のクリーンナップ処理
        private ConsoleResultCleaner m_consoleResultCleaner;

        // 読み込み対象のファイル
        private AccessibleFile m_accessibleFile;

        // 読み込み対象のファイルのサーバー内パス
        private string m_localPath;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent     所有オブジェクト
        // 　　　　[in]connection SSHの接続
        // 　　　　[in]file       読み込み対象のファイル
        // 　　　　[in]localPath  読み込み対象のファイルのサーバー内パス
        // 戻り値：なし
        //=========================================================================================
        public ShellEngineRetrieveChunk(ShellCommandEmulator parent, SSHConnection connection, AccessibleFile file, string localPath) {
            m_shellCommandEmulator = parent;
            m_connection = connection;
            m_accessibleFile = file;
            m_localPath = localPath;

            ShellCommandDictionary dictionary = m_connection.ShellCommandDictionary;
            m_resultChunkParser = new ResultBytesParser(m_connection, this, m_shellCommandEmulator, m_connection.ShellCommandDictionary.ExpectGetFileHead);
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
            string strCommand = dictionary.GetCommandGetFileHead(m_localPath, m_accessibleFile.MaxFileSize + 1);
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
            error = m_resultChunkParser.OnReceive(buffer, out receivedData, out completed, out remain);
            if (error.IsFailed) {
                m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.Failed, error.ErrorMessage + ":" + error.ErrorLine);
                return error;
            }

            if (receivedData.Length > 0) {
                byte[] cleaned = m_consoleResultCleaner.CleanReceivedData(buffer, receivedData.Offset, receivedData.Length, completed);
                m_accessibleFile.AddData(cleaned, 0, cleaned.Length);
            }

            if (completed) {
                m_accessibleFile.AddCompleted(RetrieveDataLoadStatus.CompletedAll, null);
                m_shellCommandEmulator.RemoveEngine(FileOperationStatus.Success);
            }
            FireEvent(completed);
            return ShellEngineError.Success();
        }

        //=========================================================================================
        // 機　能：受信時のイベントを発行する
        // 引　数：[in]completed  処理が完了したときtrue
        // 戻り値：なし
        //=========================================================================================
        public void FireEvent(bool completed) {
            Program.MainWindow.BeginInvoke(new FireEventDelegate(FireEventUI), completed);
        }
        private delegate void FireEventDelegate(bool completed);
        private void FireEventUI(bool completed) {
            try {
                m_accessibleFile.FireEvent(completed);
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionUI, "FireEventUI");
            }
        }
    }
}
