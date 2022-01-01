using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.GraphicsViewer;
using ShellFiler.Virtual;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.TerminalSession.CommandEmulator;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：ファイルをサーバにアップロードするプロシージャ
    //=========================================================================================
    class ShellTransferUploadProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        // コンテキスト情報
        private FileOperationRequestContext m_context;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellTransferUploadProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
            m_context = context;
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]srcPhysicalPath  転送元ファイル名のフルパス
        // 　　　　[in]destFilePath     転送先ファイル名のフルパス
        // 　　　　[in]progress         進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(string srcPhysicalPath, string destFilePath, FileProgressEventHandler progress) {
            FileOperationStatus status = m_controler.Initialize(destFilePath, false, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            destFilePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, destFilePath);
            SSHProtocolType protocol;
            string user, server, destLocalPath;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(destFilePath, out protocol, out user, out server, out portNo, out destLocalPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }
            
            // アップロード処理を実行
            // リトライ回数の設定を実行回数に変換
            int retryCount = m_controler.Connection.ShellCommandDictionary.ValueUploadDownloadRetryCount;
            while (true) {
                status = ExecuteUpload(srcPhysicalPath, destFilePath, destLocalPath, progress);
                if (status != FileOperationStatus.ShellDiffCrc) {
                    return status;
                }
                retryCount--;
                if (retryCount < 0) {
                    return status;
                }
            }
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]srcPhysicalPath  転送元ファイル名のフルパス
        // 　　　　[in]destFilePath     転送先ファイル名のフルパス
        // 　　　　[in]destLocalPath    転送先ファイルのローカルファイルシステム内でのパス
        // 　　　　[in]progress         進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus ExecuteUpload(string srcPhysicalPath, string destFilePath, string destLocalPath, FileProgressEventHandler progress) {
            FileOperationStatus status;
            // 読み込みファイルを開く
            Stream srcFileStream = null, srcBufferedStream = null;
            try {
                try {
                    srcFileStream = new FileStream(srcPhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    srcBufferedStream = new BufferedStream(srcFileStream);
                } catch (Exception) {
                    return FileOperationStatus.ErrorOpen;
                }

                // アップロード処理を実行
                ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
                ShellEngineUpload engineUpload = new ShellEngineUpload(emulator, m_controler.Connection, srcPhysicalPath, destLocalPath, srcBufferedStream, progress);
                status = emulator.Execute(m_controler.Context, engineUpload);
                if (!status.Succeeded) {
                    return status;
                }

                // チェックサムを取得
                if (m_controler.Connection.ShellCommandDictionary.ValueUploadDownloadUseCheckCksum) {
                    ShellEngineComputeChecksum engineCksum = new ShellEngineComputeChecksum(emulator, m_controler.Connection, destLocalPath);
                    status = emulator.Execute(m_controler.Context, engineCksum);
                    if (!status.Succeeded) {
                        return status;
                    }
                    if (engineUpload.CksumCrc.CurrentCrc != engineCksum.CksumCrc32 || engineUpload.CksumCrc.TotalSize != engineCksum.FileSize) {
                        return FileOperationStatus.ShellDiffCrc;
                    }
                }
            } finally {
                if (srcBufferedStream != null) {
                    try {
                        srcBufferedStream.Close();
                    } catch (Exception) {
                    }
                }
                if (srcFileStream != null) {
                    try {
                        srcFileStream.Close();
                    } catch (Exception) {
                    }
                }
            }
            return FileOperationStatus.Success;
        }
    }
}