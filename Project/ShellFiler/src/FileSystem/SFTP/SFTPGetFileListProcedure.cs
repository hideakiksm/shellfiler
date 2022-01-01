using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ファイル一覧を取得するプロシージャ
    //=========================================================================================
    class SFTPGetFileListProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPGetFileListProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]directory   一覧を作成するディレクトリ
        // 　　　　[out]fileList   ファイル一覧を取得する変数への参照
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Execute(string directory, out List<IFile> fileList) {
            fileList = null;
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            directory = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, directory);
            status = ChannelLsHelper.ExecLs(m_controler, directory, out fileList);
            if (m_controler.IsCanceled) {
                return FileOperationStatus.Canceled;
            }
            return status;
        }
    }
}
