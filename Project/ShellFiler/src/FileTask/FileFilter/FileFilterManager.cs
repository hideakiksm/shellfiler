using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.FileTask.FileFilter {

    //=========================================================================================
    // クラス：ファイルフィルターの管理クラス
    //=========================================================================================
    public class FileFilterManager {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterManager() {
        }

        //=========================================================================================
        // 機　能：フィルター一覧を返す
        // 引　数：なし
        // 戻り値：フィルター一覧
        //=========================================================================================
        public List<IFileFilterComponent> GetFileFilterList() {
            List<IFileFilterComponent> result = new List<IFileFilterComponent>();
            result.Add(new FileFilterCharsetConvert());
            result.Add(new FileFilterConvertCrLf());
            result.Add(new FileFilterConvertTabSpace());
            result.Add(new FileFilterTrimLastSpace());
            result.Add(new FileFilterDeleteMultiCrLf());
            result.Add(new FileFilterBase64Encode());
            result.Add(new FileFilterBase64Decode());
            result.Add(new FileFilterHttpHeaderBody());
            result.Add(new FileFilterShellFilerDump());
            result.Add(new FileFilterCustom());
            return result;
        }

        //=========================================================================================
        // 機　能：転送用フィルターの一覧から指定されたファイルに該当するものを返す
        // 引　数：[in]fileFilter   転送用フィルターの一覧
        // 　　　　[in]filePath     使用するファイルのパス
        // 戻り値：使用するフィルター（該当するものがなかったときnull）
        //=========================================================================================
        public List<FileFilterItem> SelectTransferFilter(FileFilterTransferSetting fileFilter, string filePath) {
            string fileName = GenericFileStringUtils.GetFileName(filePath).ToLower();
            int transferCount = fileFilter.TransferList.Count;
            for (int i = 0; i < transferCount; i++) {
                FileFilterListTransfer transferType = fileFilter.TransferList[i];
                if (!transferType.UseFilter) {
                    continue;
                }
                string[] targetMaskList = transferType.TargetFileMask.ToLower().Split(';');
                for (int j = 0; j < targetMaskList.Length; j++) {
                    Regex reg = StringUtils.ConvertWildcardToRegex(targetMaskList[j]);
                    bool match = reg.IsMatch(fileName);
                    if (match) {
                        return transferType.FilterList;
                    }
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：転送用ファイルフィルターの設定を使ってファイル内容を変換する
        // 引　数：[in]srcFilePath  変換するファイルのパス（ファイル名の判別に使用、内容はsrcData）
        // 　　　　[in]srcData      変換元のバイト列
        // 　　　　[out]destData    変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 　　　　[in]fileFilter   転送用ファイルフィルターの設定
        // 　　　　[in]cancelEvent キャンセル時にシグナル状態になるイベント
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus ConvertWithTransferSetting(string srcFilePath, byte[] srcData, out byte[] destData, FileFilterTransferSetting fileFilter, WaitHandle cancelEvent) {
            FileOperationStatus status;
            destData = null;
            List<FileFilterItem> filterList = SelectTransferFilter(fileFilter, srcFilePath);
            if (filterList == null) {
                if (fileFilter.OtherMode == FileFilterListTransferOtherMode.SkipTransfer) {
                    return FileOperationStatus.SkippedFilter;
                }
                destData = srcData;
            } else {
                status = Convert(srcFilePath, srcData, out destData, filterList, cancelEvent);
                srcData = null;
                if (!status.Succeeded) {
                    return status;
                }
            }
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // 機　能：変換を実行する
        // 引　数：[in]orgFileName 元のファイルパス（クリップボードのときnull）
        // 　　　　[in]src         変換元のバイト列
        // 　　　　[out]dest       変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 　　　　[in]filterList  変換に使用するフィルターの一覧
        // 　　　　[in]cancelEvent キャンセル時にシグナル状態になるイベント
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Convert(string orgFileName, byte[] src, out byte[] dest, List<FileFilterItem> filterList, WaitHandle cancelEvent) {
            // 変換処理を実行
            byte[] destTemp = src;
            for (int i = 0; i < filterList.Count; i++) {
                FileFilterItem filterItem = filterList[i];
                Type componentType = Type.GetType(filterItem.FileFilterClassPath);
                IFileFilterComponent component = (IFileFilterComponent)(componentType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                FileOperationStatus status = component.Convert(orgFileName, src, out destTemp, filterItem.PropertyList, cancelEvent);
                if (!status.Succeeded) {
                    dest = null;
                    return status;
                }
                src = destTemp;
            }

            dest = destTemp;
            return FileOperationStatus.Success;
        }
    }
}
