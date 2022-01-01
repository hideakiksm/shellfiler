using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.GraphicsViewer;
using ShellFiler.Util;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：画像情報を取得するプロシージャ
    //=========================================================================================
    class RetrieveImageProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public RetrieveImageProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：ファイルを取得する
        // 引　数：[in]dirName   読み込み対象のファイルパス
        // 　　　　[in]maxSize   読み込む最大サイズ
        // 　　　　[out]image    読み込んだ画像を返す変数
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(string dirName, long maxSize, out BufferedImage image) {
            image = null;
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            string filePath = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, dirName);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // サイズを確認
            ChannelSftp sftpChannel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
            try {
                SftpATTRS attrs = sftpChannel.stat(path);
                if (attrs.getSize() > maxSize) {
                    return FileOperationStatus.ErrorTooLarge;
                }
            } catch (SftpException) {
                return FileOperationStatus.FileNotFound;
            }

            // コマンドを実行
            byte[] buffer;
            string command = m_controler.Connection.ShellCommandDictionary.GetCommandGetFileHead(path, (int)(Math.Max(maxSize + 1, int.MaxValue)));
            try {
                ChannelCommandExec exec = new ChannelCommandExec(m_controler.Connection);
                status = exec.Execute(m_controler.Context.SFTPRequestContext, command, out buffer);
                if (status != FileOperationStatus.Success) {
                    return status;
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            if (m_controler.IsCanceled) {
                return FileOperationStatus.Canceled;
            }

            // 画像を読み込む
            ImageLoader loader = new ImageLoader();
            int colorBits;
            status = loader.LoadImage(buffer, out image, out colorBits);
            if (!status.Succeeded) {
                return status;
            }

            return FileOperationStatus.Success;

        }
    }
}
