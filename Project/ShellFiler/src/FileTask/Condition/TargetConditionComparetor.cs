using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.UI.Log;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：ファイルの実行条件の比較クラス
    //=========================================================================================
    public class TargetConditionComparetor {

        //=========================================================================================
        // 機　能：指定されたファイルがファイル操作の対象かどうかを返す
        // 引　数：[in]transCond   転送条件
        // 　　　　[in]file        比較対象のファイル
        // 　　　　[in]isPositive  正論理で比較するときtrue、負論理で比較するときfalse
        // 戻り値：条件に一致するときtrue
        //=========================================================================================
        public static bool IsTarget(CompareCondition transCond, IFile file, bool isPositive) {
            foreach (FileConditionItem condItem in transCond.ConditionList) {
                bool match = false;
                if (condItem is FileConditionItemDefined) {
                    FileConditionItemDefined typedItem = (FileConditionItemDefined)condItem;
                    match = typedItem.CompareFile(transCond, file);
                } else if (condItem is FileConditionItemWindows && file is WindowsFile) {
                    FileConditionItemWindows typedItem = (FileConditionItemWindows)condItem;
                    WindowsFile typedFile = (WindowsFile)file;
                    match = typedItem.CompareFile(transCond, typedFile);
                } else if (condItem is FileConditionItemSSH && file is SFTPFile) {
                    FileConditionItemSSH typedItem = (FileConditionItemSSH)condItem;
                    SFTPFile typedFile = (SFTPFile)file;
                    match = typedItem.CompareFile(transCond, typedFile);
                } else {
                    Program.Abort("ファイル転送条件の型が一致しません。\ncondition={0}, file={1}", condItem.GetType(), file.GetType());
                }
                if (!isPositive) {
                    match = !match;
                }
                if (match) {
                    return true;
                }
            }
            return false;
        }

        //=========================================================================================
        // 機　能：ファイルまたはフォルダがファイル操作の対象かどうかを返す
        // 引　数：[in]transCond   転送条件
        // 　　　　[in]isDirectory 比較対象がディレクトリのときtrue、ファイルのときfalse
        // 戻り値：条件に一致するときtrue
        //=========================================================================================
        public static bool CompareFileType(FileConditionTarget transCond, bool isDirectory) {
            if (transCond == FileConditionTarget.FileOnly) {
                if (isDirectory) {
                    return false;
                }
            } else if (transCond == FileConditionTarget.FolderOnly) {
                if (!isDirectory) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイル名の条件が正しく動作するかどうかを返す
        // 引　数：[in]nameType    ファイル名の条件種別
        // 　　　　[in]nameCond    ファイル名の条件文字列（正規表現/ワイルドカード）
        // 戻り値：条件が正しく動作するときtrue
        //=========================================================================================
        public static bool ValidateFileName(FileNameType nameType, string nameCond) {
            try {
                if (nameType == FileNameType.RegularExpression) {
                    Regex regex = new Regex(nameCond);
                    regex.IsMatch("abc");
                } else if (nameType == FileNameType.WildCard) {
                    string regStr = WildCardConverter.ConvertWildCardToRegexString(nameCond);
                    if (regStr == null) {
                        return false;
                    }
                    Regex regex = new Regex(regStr);
                    regex.IsMatch("abc");
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：指定されたファイル名のファイルまたはフォルダがファイル操作の対象かどうかを返す
        // 引　数：[in]transCond   転送条件
        // 　　　　[in]nameType    ファイル名の条件種別
        // 　　　　[in]nameCond    ファイル名の条件文字列（正規表現/ワイルドカード）
        // 　　　　[in]file        対象のファイル
        // 戻り値：条件に一致するときtrue
        //=========================================================================================
        public static bool CompareFileName(CompareCondition transCond, FileNameType nameType, string nameCond, IFile file) {
            if (nameType != FileNameType.None) {
                bool ignoreCase = FileSystemID.IgnoreCaseFolderPathFromIFileType(file.GetType());
                string regStr = nameCond;
                string targetFileName = file.FileName;
                if (ignoreCase) {
                    regStr = nameCond.ToLower();
                    targetFileName = targetFileName.ToLower();
                }

                Regex regex;
                if (nameType == FileNameType.RegularExpression) {
                    regex = transCond.GetCachedRegularExpression(regStr, false);
                } else {
                    regex = transCond.GetCachedRegularExpression(regStr, true);
                }
                bool match = regex.IsMatch(targetFileName);
                if (!match) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：指定された属性のファイルまたはフォルダがファイル操作の対象かどうかを返す
        // 引　数：[in]condAttr   条件での属性指定（指定しないときnull）
        // 　　　　[in]fileAttr   ファイルの属性
        // 戻り値：条件に一致するときtrue
        //=========================================================================================
        public static bool CompareFileAttribute(BooleanFlag condAttr, bool fileAttr) {
            if (condAttr == null) {
                return true;
            } else if (condAttr.Value == fileAttr) {
                return true;
            } else {
                return false;
            }
        }
    }
}
