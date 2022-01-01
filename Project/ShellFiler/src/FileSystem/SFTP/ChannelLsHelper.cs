using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：SFTPのLSを実行するヘルパー
    //=========================================================================================
    class ChannelLsHelper {

        //=========================================================================================
        // 機　能：lsコマンドを実行する
        // 引　数：[in]controler   SSH接続のコントローラ
        // 　　　　[in]directory   取得対象のディレクトリ
        // 　　　　[out]fileList   ファイル一覧を返す変数への参照
        // 戻り値：ステータス
        //=========================================================================================
        public static FileOperationStatus ExecLs(SFTPProcedureControler controler, string directory, out List<IFile> fileList) {
            FileOperationStatus status;

            // フルパスディレクトリを分解
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(directory, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }
            if (path != "/" && path.EndsWith("/")) {
                path = path.Substring(0, path.Length - 1);
            }

            // 一覧を取得
            fileList = new List<IFile>();
            try {
                ChannelSftp channel = controler.Context.SFTPRequestContext.GetChannelSftp(controler.Connection);
                ArrayList sftpList = channel.ls(path);
                if (sftpList == null) {
                    return FileOperationStatus.Success;
                }
                int index = 1;
                foreach (object objLsEntry in sftpList) {
                    if (!(objLsEntry is ChannelSftp.LsEntry)) {
                        continue;
                    }
                    // ファイル情報を取得
                    ChannelSftp.LsEntry lsEntry = (ChannelSftp.LsEntry)objLsEntry;
                    SFTPFile file = convertLsEntryToIFile(lsEntry);
                    if (file == null) {
                        continue;
                    }
                    if (file.FileName == "..") {
                        file.DefaultOrder = 0;
                        fileList.Insert(0, file);
                    } else {
                        file.DefaultOrder = index++;
                        fileList.Add(file);
                    }
                }

                status = SFTPSymbolicLinkResolver.SetSymbolicLinkFileList(controler, path, fileList);
                if (!status.Succeeded) {
                    return status;
                }

                return FileOperationStatus.Success;
            } catch (SftpException) {
                return FileOperationStatus.Fail;
            } catch (Exception e) {
                return controler.Connection.OnException(e, Resources.Log_SSHExecFail);
            }
        }
        
        //=========================================================================================
        // 機　能：lsの実行結果の1ファイルを内部形式に変換する
        // 引　数：[in]lsEntry  SFTPのls実行結果の1ファイル
        // 戻り値：内部形式でのファイル(null:生成しないファイル)
        //=========================================================================================
        public static SFTPFile convertLsEntryToIFile(ChannelSftp.LsEntry lsEntry) {
            string fileName = lsEntry.getFilename().ToString();
            if (fileName == ".") {
                return null;
            }

            SFTPFile file = new SFTPFile(fileName, lsEntry.getAttrs(), lsEntry.getLongname());
            return file;
        }
    }
}
