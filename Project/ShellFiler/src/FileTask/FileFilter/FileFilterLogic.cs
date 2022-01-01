using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.FileFilter;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileTask.FileFilter {

    //=========================================================================================
    // クラス：ファイルフィルター関連の実装クラス
    //=========================================================================================
    class FileFilterLogic {

        //=========================================================================================
        // 機　能：ファイルフィルターを適用する
        // 引　数：[in]orgSrcFilePath  元の転送元ファイルパス（ファイル名の判別に使用）
        // 　　　　[in]srcFilePath     Windows上の転送元ファイルパス
        // 　　　　[in]destFilePath    Windows上の転送先ファイルパス
        // 　　　　[in]fileFilter      転送時に使用するフィルター（使用しないときはnull）
        // 　　　　[in]cancelEvent     キャンセル時にシグナル状態になるイベント
        // 戻り値：ステータスコード
        //=========================================================================================
        public static FileOperationStatus ApplyFileFilter(string orgSrcFilePath, string srcFilePath, string destFilePath, FileFilterTransferSetting fileFilter, WaitHandle cancelEvent) {
            FileOperationStatus status;

            // 転送元を読み込む
            byte[] srcData;
            try {
                srcData = File.ReadAllBytes(srcFilePath);
            } catch (OutOfMemoryException) {
                return FileOperationStatus.ErrorOutOfMemory;
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }

            // フィルターを適用
            byte[] destData;
            try {
                FileFilterManager filterManager = Program.Document.FileFilterManager;
                status = filterManager.ConvertWithTransferSetting(orgSrcFilePath, srcData, out destData, fileFilter, cancelEvent);
                srcData = null;
                if (status != FileOperationStatus.Success) {
                    return status;      // Skipの場合はここで終わる
                }
            } catch (OutOfMemoryException) {
                return FileOperationStatus.ErrorOutOfMemory;
            }

            // 転送先に書き込む
            try {
                File.WriteAllBytes(destFilePath, destData);
            } catch (OutOfMemoryException) {
                return FileOperationStatus.ErrorOutOfMemory;
            } catch (Exception) {
                return FileOperationStatus.Fail;
            }

            return FileOperationStatus.Success;
        }
    }
}
