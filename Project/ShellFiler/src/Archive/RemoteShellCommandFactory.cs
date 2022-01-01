using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：リモートシェルの書庫操作コマンドの生成クラス
    //=========================================================================================
    public class RemoteShellCommandFactory {

        //=========================================================================================
        // 機　能：コマンドを作成する
        // 引　数：[in]parameter     圧縮のパラメータ
        // 　　　　[in]srcDispPath   転送元の表示用パス名
        // 　　　　[in]destDispPath  転送先の表示用パス名
        // 　　　　[in]commandDic    コマンドの作成クラス
        // 　　　　[in]fileList      格納するファイルの一覧
        // 戻り値：作成したコマンド
        //=========================================================================================
        public static string CreateCommand(ArchiveParameter parameter, string srcDispPath, string destDispPath, ShellCommandDictionary commandDic, List<UIFile> fileList) {
            SSHProtocolType srcProtocol, destProtocol;
            string srcUser, srcServer, srcPath;
            string destUser, destServer, destPath;
            int srcPort, destPort;
            SSHUtils.SeparateUserServer(srcDispPath, out srcProtocol, out srcUser, out srcServer, out srcPort, out srcPath);
            SSHUtils.SeparateUserServer(destDispPath, out destProtocol, out destUser, out destServer, out destPort, out destPath);
            
            string arcPath = destPath + parameter.FileName;
            List<string> fileNameList = new List<string>();
            foreach (UIFile file in fileList) {
                fileNameList.Add(file.FileName);
            }

            string fullCommand = commandDic.CompleteArchiveCommand(parameter.RemoteShellOption.CommandLine, srcPath, arcPath, fileNameList);
            return fullCommand;
        }
    }
}
