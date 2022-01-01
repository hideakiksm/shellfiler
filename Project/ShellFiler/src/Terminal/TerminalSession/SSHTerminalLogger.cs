using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Locale;
using ShellFiler.Util;
using ShellFiler.Properties;

namespace ShellFiler.Terminal.TerminalSession {

    //=========================================================================================
    // クラス：SSHのターミナル受信結果をログ出力するクラス
    //=========================================================================================
    public class SSHTerminalLogger {
        // ログローテーションのためのロックオブジェクト
        private static Object s_rotationLock = new Object();

        //=========================================================================================
        // 機　能：バッファの内容をログに出力する
        // 引　数：[in]parent    所有しているチャネル
        // 　　　　[in]buffer    出力内容のバッファ
        // 　　　　[in]offset    バッファ内の開始オフセット
        // 　　　　[in]length    バッファ内の長さ
        // 戻り値：なし
        //=========================================================================================
        public static void WriteLog(TerminalShellChannel parent, byte[] buffer, int offset, int length) {
            if (Configuration.Current.TerminalLogType == TerminalLogType.None) {
                return;
            }
            
            // ログファイル名を決定
            string logFolder = DirectoryManager.TerminalLogOutputFolder;
            string logFileName;
            int pid = Process.GetCurrentProcess().Id;
            if (Configuration.Current.TerminalLogType == TerminalLogType.EachSession) {
                string dateTime = DateTimeFormatter.DateTimeToNoSeparate(parent.ChannelStartTime);
                logFileName = string.Format("ssh_{0}_{1:5}_{2}.log", pid, parent.ID.GetAsInt(), dateTime);
            } else {
                logFileName = string.Format("ssh_{0}.log", pid);
            }

            // ログ出力
            lock (s_rotationLock) {
                try {
                    FileStream stream = new FileStream(logFolder + logFileName, FileMode.OpenOrCreate);
                    try {
                        stream.Seek(0, SeekOrigin.End);
                        stream.Write(buffer, offset, length);
                    } finally {
                        stream.Close();
                    }
                } catch (Exception) {
                    return;
                }

                // ローテーション
                try {
                    FileInfo fi = new FileInfo(logFolder + logFileName);
                    long fileSize = fi.Length;
                    if (fileSize > (long)Configuration.Current.TerminalLogMaxSize * 1024L) {
                        RotateLog(logFolder, logFileName);
                    }
                } catch (Exception) {
                }
            }
        }

        //=========================================================================================
        // 機　能：ログファイルのローテーションを行う
        // 引　数：[in]logFolder    ログフォルダ（最後は'\'）
        // 　　　：[in]logFileName  現在のログファイル名
        // 戻り値：なし
        //=========================================================================================
        private static void RotateLog(string logFolder, string logFileName) {
            // *.logを取得
            FileInfo[] fileList;
            while (true) {
                DirectoryInfo logFolderInfo = new DirectoryInfo(logFolder);
                fileList =  logFolderInfo.GetFiles("*.log");
                if (fileList.Length < Configuration.Current.TerminalLogFileCount) {
                    break;
                }

                // 最古のファイルを削除
                FileInfo oldestFile = fileList[0];
                for (int i = 1; i < fileList.Length; i++) {
                    if (oldestFile.LastWriteTime > fileList[i].LastWriteTime) {
                        oldestFile = fileList[i];
                    }
                }
                try {
                    File.Delete(logFolder + oldestFile.Name);
                } catch (Exception) {
                }
            }

            // 新しいファイル名を決定
            string logFileNameBody = logFileName.Substring(0, logFileName.Length - 4) + "_";  // ssh_123.log → ssh_123_
            int lastRotationNumber = 1;
            for (int i = 0; i < fileList.Length; i++) {
                if (fileList[i].Name.StartsWith(logFileNameBody)) {
                    int rotationNumber;         // ssh_123_2.logの2を取得
                    string fileNumberPart = fileList[i].Name.Substring(logFileNameBody.Length);
                    if (fileNumberPart.EndsWith(".log")) {
                        fileNumberPart = fileNumberPart.Substring(0, fileNumberPart.Length - 4);
                    }
                    if (int.TryParse(fileNumberPart, out rotationNumber)) {
                        if (rotationNumber > lastRotationNumber) {
                            lastRotationNumber = rotationNumber;
                        }
                    }
                }
            }

            // リネーム
            try {
                string newLogFileName = logFileNameBody + (lastRotationNumber + 1) + ".log";
                File.Move(logFolder + logFileName, logFolder + newLogFileName);
            } catch (Exception) {
            }
        }
    }
}
