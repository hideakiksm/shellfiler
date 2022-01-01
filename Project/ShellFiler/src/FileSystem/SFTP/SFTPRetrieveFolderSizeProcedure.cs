using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileViewer;
using ShellFiler.Util;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：フォルダサイズの取得を実行するプロシージャ
    //=========================================================================================
    class SFTPRetrieveFolderSizeProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        // 取得条件
        private RetrieveFolderSizeCondition m_condition;

        // 取得結果を返す変数
        private RetrieveFolderSizeResult m_result;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 　　　　[in]condition  取得条件
        // 　　　　[in]result     取得結果を返す変数
        // 戻り値：なし
        //=========================================================================================
        public SFTPRetrieveFolderSizeProcedure(SSHConnection connection, FileOperationRequestContext context, RetrieveFolderSizeCondition condition, RetrieveFolderSizeResult result) {
            m_controler = new SFTPProcedureControler(connection, context);
            m_condition = condition;
            m_result = result;
        }

        //=========================================================================================
        // 機　能：指定したフォルダ以下のファイルサイズ合計を取得する
        // 引　数：[in]directory   対象ディレクトリのルート
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        public FileOperationStatus Execute(string directory) {
            FileOperationStatus status = m_controler.Initialize();
            if (!status.Succeeded) {
                return status;
            }

            string filePath = SSHUtils.CompleteSFTPDirectory(m_controler.Connection, directory);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // duコマンドを実行
            try {
                string command = m_controler.Connection.ShellCommandDictionary.GetDuCommand(path);
                byte[] resultBuffer = null;
                ChannelCommandExec exec = new ChannelCommandExec(m_controler.Connection);
                status = exec.Execute(m_controler.Context.SFTPRequestContext, command, out resultBuffer);
                if (status != FileOperationStatus.Success) {
                    return status;
                }
                string result = m_controler.Connection.ByteToString(resultBuffer);
                status = ParseDuResult(result, path);
                if (status != FileOperationStatus.Success) {
                    return status;
                }
            } catch (Exception e) {
                return m_controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
            if (m_controler.IsCanceled) {
                return FileOperationStatus.Canceled;
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：duコマンドの結果を解析する
        // 引　数：[in]duResult   duコマンドの実行結果
        // 　　　　[in]basePath   対象ディレクトリのルート
        // 戻り値：ステータス（成功のときSuccess）
        //=========================================================================================
        private FileOperationStatus ParseDuResult(string duResult, string basePath) {
            basePath = GenericFileStringUtils.CompleteDirectoryName(GenericFileStringUtils.GetDirectoryName(basePath, '/'), "/");
            string[] lines = StringUtils.SeparateLine(duResult);
            for (int i = 0; i < lines.Length; i++) {
                if (lines[i].Trim().Length == 0) {
                    continue;
                }
                string[] columns = StringUtils.SeparateBySpace(lines[i].Replace('\t', ' '), 2);
                if (columns.Length != 2) {
                    return FileOperationStatus.FailFormat;
                }
                string subPath = columns[1];
                subPath = GenericFileStringUtils.CompleteDirectoryName(subPath, "/");
                if (!subPath.StartsWith(basePath)) {
                    return FileOperationStatus.FailFormat;
                }
                subPath = subPath.Substring(basePath.Length);
                if (subPath.Length > 0) {
                    subPath = subPath.Substring(0, subPath.Length - 1);
                }
                int depth = StringUtils.CountCharOf(subPath, '/') + 1;
                long folderSize;
                if (!long.TryParse(columns[0], out folderSize)) {
                    return FileOperationStatus.FailFormat;
                }
                m_result.AddResult(subPath, depth, folderSize);
            }
            return FileOperationStatus.Success;
        }
    }
}
