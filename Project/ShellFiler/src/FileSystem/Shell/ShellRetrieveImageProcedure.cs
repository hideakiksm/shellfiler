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
    // クラス：画像情報を取得するプロシージャ
    //=========================================================================================
    class ShellRetrieveImageProcedure : ShellProcedure {
        // SSH内部処理のコントローラ
        private ShellProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public ShellRetrieveImageProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new ShellProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：ファイルを取得する
        // 引　数：[in]filePath  読み込み対象のファイルパス
        // 　　　　[in]maxSize   読み込む最大サイズ
        // 　　　　[out]image    読み込んだ画像を返す変数
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(string filePath, long maxSize, out BufferedImage image) {
            image = null;
            FileOperationStatus status = m_controler.Initialize(filePath, true, ShellProcedureControler.InitializeMode.GenericOperation);
            if (!status.Succeeded) {
                return status;
            }

            filePath = SSHUtils.CompleteShellDirectory(m_controler.TerminalChannel, filePath);
            SSHProtocolType protocol;
            string user, server, path;
            int portNo;
            bool success = SSHUtils.SeparateUserServer(filePath, out protocol, out user, out server, out portNo, out path);
            if (!success) {
                throw new SfException(SfException.SSHCanNotParsePath);
            }

            // headコマンドを実行
            ShellCommandDictionary dictionary = m_controler.Connection.ShellCommandDictionary;
            string strCommand = dictionary.GetCommandGetFileHead(path, (int)Math.Min(maxSize + 1, int.MaxValue));
            List<OSSpecLineExpect> errorExpect = dictionary.ExpectGetFileHead;
            ShellCommandEmulator emulator = m_controler.TerminalChannel.ShellCommandEmulator;
            ShellEngineRetrieveData engine = new ShellEngineRetrieveData(emulator, m_controler.Connection, strCommand, errorExpect);
            status = emulator.Execute(m_controler.Context, engine);
            if (!status.Succeeded) {
                return status;
            }
            byte[] buffer = engine.ResultBytes;
            if (buffer.Length > maxSize) {
                return FileOperationStatus.ErrorTooLarge;
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
