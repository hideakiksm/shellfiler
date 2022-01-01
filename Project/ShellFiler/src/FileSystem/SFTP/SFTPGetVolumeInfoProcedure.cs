using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：ボリューム情報を取得するプロシージャ
    //=========================================================================================
    class SFTPGetVolumeInfoProcedure : SFTPProcedure {
        // SSH内部処理のコントローラ
        private SFTPProcedureControler m_controler;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]connection 接続
        // 　　　　[in]context    コンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public SFTPGetVolumeInfoProcedure(SSHConnection connection, FileOperationRequestContext context) {
            m_controler = new SFTPProcedureControler(connection, context);
        }

        //=========================================================================================
        // 機　能：コマンドを実行する
        // 引　数：[in]directory   情報を取得するディレクトリ
        // 　　　　[out]volumeInfo ボリューム情報を返す変数への参照
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus Execute(string directory, out VolumeInfo volumeInfo) {
            volumeInfo = null;
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

            // dfコマンドを実行
            try {
                string command = m_controler.Connection.ShellCommandDictionary.GetCommandGetVolumeInfo(path);
                byte[] resultBuffer = null;
                ChannelCommandExec exec = new ChannelCommandExec(m_controler.Connection);
                status = exec.Execute(m_controler.Context.SFTPRequestContext, command, out resultBuffer);
                if (status != FileOperationStatus.Success) {
                    return status;
                }
                string result = m_controler.Connection.ByteToString(resultBuffer);
                status = CreateVolumeInfo(result, out volumeInfo);
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
        // 機　能：dfコマンドの実行結果からボリューム情報を作成する
        // 引　数：[in]result       dfコマンドの結果
        // 　　　　[out]volumeInfo  作成したボリューム情報
        // 戻り値：ステータスコード
        //=========================================================================================
        public FileOperationStatus CreateVolumeInfo(string result, out VolumeInfo volumeInfo) {
            string dfLine = StringUtils.GetLastLine(result, true);
            string[] dfColumnList = StringUtils.SeparateBySpace(dfLine);
            // Filesystem、1K-ブロック、使用、使用可、使用%、マウント位置
            long dummy;
            int percent;
            if (dfColumnList.Length >= 6 && long.TryParse(dfColumnList[2], out dummy) && long.TryParse(dfColumnList[3], out dummy) && IsPercent(dfColumnList[4], out percent)) {
                volumeInfo = new VolumeInfo();
                volumeInfo.UsedDiskSize = long.Parse(dfColumnList[2]) * 1024;
                volumeInfo.FreeSize = long.Parse(dfColumnList[3]) * 1024;
                volumeInfo.FreeRatio = 100 - percent;
                volumeInfo.ClusterSize = 1;
                volumeInfo.VolumeLabel = "";
                volumeInfo.DriveEtcInfo = result;
            } else {
                volumeInfo = null;
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：dfコマンドの使用量%形式かどうかを確認する。
        // 引　数：[in]str       文字列表現
        // 　　　　[out]percent  使用量%の値
        // 戻り値：ステータスコード
        //=========================================================================================
        public bool IsPercent(string result, out int percent) {
            percent = -1;
            if (result.Length < 2) {
                return false;
            }
            int value;
            if (result.EndsWith("%") && int.TryParse(result.Substring(0, result.Length - 1), out value)) {
                if (0 <= value && value <= 100) {
                    percent = value;
                    return true;
                }
            }

            return false;
        }
    }
}
