using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ShellFiler.Api;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：Win32のCopyFileExのラップ
    //=========================================================================================
    public class Win32CopyFileEx {
        private const uint COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x00000008; 
        private const uint COPY_FILE_FAIL_IF_EXISTS              = 0x00000001; 
        private const uint COPY_FILE_OPEN_SOURCE_FOR_WRITE       = 0x0000004; 
        private const uint COPY_FILE_RESTARTABLE                 = 0x00000002; 
        
        private const uint  MOVEFILE_REPLACE_EXISTING       = 0x00000001;
        private const uint  MOVEFILE_COPY_ALLOWED           = 0x00000002;
        private const uint  MOVEFILE_DELAY_UNTIL_REBOOT     = 0x00000004;
        private const uint  MOVEFILE_WRITE_THROUGH          = 0x00000008;
        private const uint  MOVEFILE_CREATE_HARDLINK        = 0x00000010;
        private const uint  MOVEFILE_FAIL_IF_NOT_TRACKABLE  = 0x00000020;

        private const uint CALLBACK_CHUNK_FINISHED = 0; 
        private const uint CALLBACK_STREAM_SWITCH = 1; 
        private const uint PROGRESS_CONTINUE = 0; 
        private const uint PROGRESS_CANCEL = 1; 
        private const uint PROGRESS_STOP = 2; 
        private const uint PROGRESS_QUIET = 3; 

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] 
        private static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref Int32 pbCancel, uint dwCopyFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] 
        private static extern bool MoveFileWithProgress(string lpExistingFileName, string lpNewFileName, CopyProgressRoutine lpProgressRoutine, IntPtr lpData, uint dwCopyFlags);

        // 進捗状況のイベント通知先のメソッド型
        private delegate uint CopyProgressRoutine(long totalSize, long transfered, long streamSize, long streamTransferred, uint streamNumber, uint reason, IntPtr hSrcFile, IntPtr hDestFile, IntPtr lpData);
        
        // 進捗状況を通知するイベント
        public FileProgressEventHandler CopyProgress; 

        //=========================================================================================
        // 機　能：コピーの進捗状況を通知する
        // 引　数：[in]totalSize   合計サイズ
        // 　　　　[in]transfered  転送済みサイズ
        // 　　　　その他、Win32APIのとおり
        // 戻り値：ステータス
        //=========================================================================================
        protected uint CopyProgressCallback(long totalSize, long transfered, long streamSize, long streamTransferred, uint streamNumber, uint reason, IntPtr hSrcFile, IntPtr hDestFile, IntPtr lpData) {
            switch (reason) { 
                case CALLBACK_CHUNK_FINISHED: 
                    FileProgressEventArgs evt = new FileProgressEventArgs(totalSize, transfered);
                    if (CopyProgress != null) {
                        CopyProgress.SetProgress(this, evt);
                    }
                    return evt.Cancel ? PROGRESS_CANCEL : PROGRESS_CONTINUE;
                case CALLBACK_STREAM_SWITCH:
                    return PROGRESS_CONTINUE;
                default:
                    return PROGRESS_CONTINUE;
            }
        }

        //=========================================================================================
        // 機　能：ファイルをコピーする
        // 引　数：[in]srcFile     転送元ファイル名のフルパス
        // 　　　　[in]destFile    転送先ファイル名のフルパス
        // 　　　　[in]overwrite   強制的に上書きするときtrue
        // 戻り値：Win32のエラーコード
        //=========================================================================================
        public int CopyFile(string srcFile, string destFile, bool overwrite) {
            uint flag;
            if (overwrite) {
                flag = 0;
            } else {
                flag = COPY_FILE_FAIL_IF_EXISTS;
            }
            Int32 cancel = 0;
            bool success = CopyFileEx(srcFile, destFile, new CopyProgressRoutine(this.CopyProgressCallback), IntPtr.Zero, ref cancel, flag);
            if (success) {
                return 0;
            } else {
                return Marshal.GetLastWin32Error(); 
            }
        }

        //=========================================================================================
        // 機　能：ファイルを移動する
        // 引　数：[in]srcFile     転送元ファイル名のフルパス
        // 　　　　[in]destFile    転送先ファイル名のフルパス
        // 　　　　[in]overwrite   強制的に上書きするときtrue
        // 戻り値：Win32のエラーコード
        //=========================================================================================
        public int MoveFile(string srcFile, string destFile, bool overwrite, bool allowCopy) {
            uint flag = 0;
            if (overwrite) {
                flag |= MOVEFILE_REPLACE_EXISTING;
            }
            if (allowCopy) {
                flag |= MOVEFILE_COPY_ALLOWED;
            }
            bool success = MoveFileWithProgress(srcFile, destFile, new CopyProgressRoutine(this.CopyProgressCallback), IntPtr.Zero, flag);
            if (success) {
                return 0;
            } else {
                return Marshal.GetLastWin32Error(); 
            }
        }
    }
}
