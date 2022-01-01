using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.Virtual;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.TerminalSession.CommandEmulator;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：SSHシェルでのユーティリティクラス
    //=========================================================================================
    class ShellUtils {

        //=========================================================================================
        // 機　能：指定されたファイルを削除する
        // 引　数：[in]controler   SSH接続のコントローラ
        // 　　　　[in]targetFile  削除対象のファイル
        // 戻り値：ステータスコード
        //=========================================================================================
        public static FileOperationStatus DeleteFile(ShellProcedureControler controler, string destLocalPath) {
            FileOperationStatus status;
            ShellCommandDictionary dictionary = controler.Connection.ShellCommandDictionary;
            string strCommand = dictionary.GetCommandDeleteFile(destLocalPath);
            List<OSSpecLineExpect> expect = dictionary.ExpectDeleteFile;
            string prompt = dictionary.ValueDeleteFilePrompt;
            string answer = dictionary.ValueDeleteFileAnswer;
            ShellCommandEmulator emulator = controler.TerminalChannel.ShellCommandEmulator;
            ShellEngineDelete engine = new ShellEngineDelete(emulator, controler.Connection, strCommand, expect, prompt, answer);
            status = emulator.Execute(controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }
            return FileOperationStatus.Success;
        }
    }
}
