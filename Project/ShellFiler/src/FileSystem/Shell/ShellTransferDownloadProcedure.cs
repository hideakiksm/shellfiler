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
using ShellFiler.Document;
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
    // クラス：ダウンロードを実行するプロシージャ
    //=========================================================================================
    class ShellTransferDownloadProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellTransferDownloadProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：ファイルを取得する
        // 引　数：[in]srcLogicalPath    転送元ファイルのファイルパス
        // 　　　　[in]destPhysicalPath  転送先ファイルとしてWindows上にダウンロードするときの物理パス
        // 　　　　[in]srcFileInfo       転送元のファイル情報（取得していないときnull）
        // 　　　　[in]progress          進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(string srcLogicalPath, string destPhysicalPath, IFile srcFileInfo, FileProgressEventHandler progress) {
            FileOperationStatus status = m_controler.Initialize(srcLogicalPath, true, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            srcLogicalPath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, srcLogicalPath);
            SSHProtocolType protocol;
            string user, server, srcLocalPath;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(srcLogicalPath, out protocol, out user, out server, out portNo, out srcLocalPath);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // ファイルの属性を取得
            if (srcFileInfo == null) {
                ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
                ShellEngineGetFileInfo engine = new ShellEngineGetFileInfo(emulator, m_controler.Connection, srcLocalPath);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
                if (engine.ResultFileInfo == null) {
                    return FileOperationStatus.FileNotFound;
                }
                srcFileInfo = engine.ResultFileInfo;
            }

            // ダウンロード処理を実行
            // リトライ回数の設定を実行回数に変換
            int retryCount = m_controler.Connection.ShellCommandDictionary.ValueUploadDownloadRetryCount;
            while (true) {
                status = ExecuteDownload(srcLogicalPath, srcLocalPath, destPhysicalPath, srcFileInfo, progress);
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
        // 機　能：ファイル取得処理を実際に実行する
        // 引　数：[in]srcLogicalPath    転送元ファイルのファイルパス
        // 　　　　[in]srcLocalPath      転送元ファイルのローカルファイルシステム内でのパス
        // 　　　　[in]destPhysicalPath  転送先ファイルとしてWindows上にダウンロードするときの物理パス
        // 　　　　[in]srcFileInfo       転送元のファイル情報（取得していないときnull）
        // 　　　　[in]progress          進捗状態を通知するdelegate
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus ExecuteDownload(string srcLogicalPath, string srcLocalPath, string destPhysicalPath, IFile srcFileInfo, FileProgressEventHandler progress) {
            FileOperationStatus status;

            // ファイルの属性を取得
            if (srcFileInfo == null) {
                ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
                ShellEngineGetFileInfo engine = new ShellEngineGetFileInfo(emulator, m_controler.Connection, srcLocalPath);
                status = emulator.Execute(m_controler.Context, engine);
                if (!status.Succeeded) {
                    return status;
                }
                if (engine.ResultFileInfo == null) {
                    return FileOperationStatus.FileNotFound;
                }
                srcFileInfo = engine.ResultFileInfo;
            }

            // 書き込みファイルを開く
            bool successWrite = false;
            Stream destFileStream = null, destBufferedStream = null;
            try {
                // 書き込み先を用意
                try {
                    destFileStream = new FileStream(destPhysicalPath, FileMode.Create, FileAccess.Write, FileShare.Write);
                    destBufferedStream = new BufferedStream(destFileStream);
                } catch (Exception) {
                    return FileOperationStatus.ErrorOpen;
                }

                ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;

                // ダウンロード処理を実行
                ShellEngineDownload engineDownload = new ShellEngineDownload(emulator, m_controler.Connection, srcLocalPath, destPhysicalPath, srcFileInfo, destBufferedStream, progress);
                status = emulator.Execute(m_controler.Context, engineDownload);
                if (!status.Succeeded) {
                    return status;
                }

                // チェックサムを取得
                if (m_controler.Connection.ShellCommandDictionary.ValueUploadDownloadUseCheckCksum) {
                    ShellEngineComputeChecksum engineCksum = new ShellEngineComputeChecksum(emulator, m_controler.Connection, srcLocalPath);
                    status = emulator.Execute(m_controler.Context, engineCksum);
                    if (!status.Succeeded) {
                        return status;
                    }
                    if (engineDownload.CksumCrc.CurrentCrc != engineCksum.CksumCrc32 || engineDownload.CksumCrc.TotalSize != engineCksum.FileSize) {
                        return FileOperationStatus.ShellDiffCrc;
                    }
                }

                successWrite = true;
            } finally {
                if (destBufferedStream != null) {
                    try {
                        destBufferedStream.Close();
                    } catch (Exception) {
                    }
                }
                if (destFileStream != null) {
                    try {
                        destFileStream.Close();
                    } catch (Exception) {
                    }
                }
                if (!successWrite) {
                    try {
                        File.Delete(destPhysicalPath);
                    } catch (Exception) {
                    }
                }
            }

            return FileOperationStatus.Success;
        }
    }
}
