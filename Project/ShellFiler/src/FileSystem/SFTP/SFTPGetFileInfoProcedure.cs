using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.Properties;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ファイル情報を取得するプロシージャ
    //=========================================================================================
    class SFTPGetFileInfoProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPGetFileInfoProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]filePath   ファイルパス
        // 　　　　[out]fileInfo  ファイルの情報（失敗したときはnull）
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(string filePath, out IFile fileInfo) {
            fileInfo = null;
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            string directory = filePath;
            string fileName = GenericFileStringUtils.GetFileName(directory, '/');

            // フルパスディレクトリを分解
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(directory, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // ファイル情報を取得
            try {
                ChannelSftp channel = m_controler.Context.SFTPRequestContext.GetChannelSftp(m_controler.Connection);
                SftpATTRS attr = channel.stat(path);
                fileInfo = new SFTPFile(fileName, attr, null);
            } catch (SftpException) {
                fileInfo = null;
                // シンボリックリンクのリンク切れを確認
                string linkTarget;
                bool exist, isDirectory;
                string basePath = GenericFileStringUtils.GetDirectoryName(path, '/');
                string fileBody = GenericFileStringUtils.GetFileName(path, '/');
                status = SFTPSymbolicLinkResolver.GetLinkTarget(m_controler, basePath, fileBody, out linkTarget, out exist, out isDirectory);
                if (status.Succeeded && !exist) {
                    return FileOperationStatus.FailLinkTarget;
                }
                // ファイルがないときは、SuccessかつfileInfo = null
                return FileOperationStatus.Success;
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            return FileOperationStatus.Success;
        }
    }
}
