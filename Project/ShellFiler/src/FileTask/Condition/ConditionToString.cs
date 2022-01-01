using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：ファイル条件を文字列化するクラス
    //=========================================================================================
    public class ConditionToString {

        //=========================================================================================
        // 機　能：対象種別の文字列を作成する
        // 引　数：[in]target  対象種別
        // 戻り値：対象種別の文字列
        //=========================================================================================
        public static string CreateTargetCondition(FileConditionTarget target) {
            string result = "";
            if (target == FileConditionTarget.FileOnly) {
                result = Resources.FileCondition_DisplayTargetFileOnly;
            } else if (target == FileConditionTarget.FolderOnly) {
                result = Resources.DlgTransferCond_CondTargetFolderOnly;
            } else if (target == FileConditionTarget.FileAndFolder) {
                result = Resources.FileCondition_DisplayTargetFileAndFolder;
            } else {
                Program.Abort("targetが想定外の値です。");
            }
            return result;
        }

        //=========================================================================================
        // 機　能：ファイル名の文字列を作成する
        // 引　数：[in]nameType  ファイル名の指定種別
        // 　　　　[in]fileName  ファイル名の条件
        // 戻り値：ファイル名の文字列
        //=========================================================================================
        public static string CreateFileNameCondition(FileNameType nameType, string fileName) {
            string result = "";
            if (nameType == FileNameType.RegularExpression) {
                result = string.Format(Resources.FileCondition_DisplayFileNameRegular, fileName);
            } else if (nameType == FileNameType.WildCard) {
                result = string.Format(Resources.FileCondition_DisplayFileNameWildCard, fileName);
            } else {
                Program.Abort("nameTypeが想定外の値です。");
            }
            return result;
        }

        //=========================================================================================
        // 機　能：日時情報の文字列を作成する
        // 引　数：[in]condition  日時情報
        // 　　　　[in]dispName   項目の表示名
        // 戻り値：日時情報の文字列
        //=========================================================================================
        public static string CreateTimeCondition(DateTimeCondition condition, string dispName) {
            // 準備
            string start = "";
            if (condition.DateStart != DateTime.MinValue) {
                start = DateTimeFormatter.DateTimeToInformation(condition.DateStart);
            } else if (condition.RelativeStart != -1 && condition.TimeStart != null) {
                start = string.Format(Resources.FileCondition_DisplayFileRelative, condition.RelativeStart, condition.TimeStart.ToString());
            } else if (condition.TimeStart != null) {
                start = condition.TimeStart.ToString();
            }
            string startOpr = "";
            if (condition.IncludeStart) {
                startOpr = Resources.FileCondition_DisplayFileLE;
            } else {
                startOpr = Resources.FileCondition_DisplayFileLT;
            }
            string end = "";
            if (condition.DateEnd != DateTime.MinValue) {
                end = DateTimeFormatter.DateTimeToInformation(condition.DateEnd);
            } else if (condition.RelativeEnd != -1 && condition.TimeEnd != null) {
                end = string.Format(Resources.FileCondition_DisplayFileRelative, condition.RelativeEnd, condition.TimeEnd.ToString());
            } else if (condition.TimeEnd != null) {
                end = condition.TimeEnd.ToString();
            }
            string endOpr = "";
            if (condition.IncludeEnd) {
                endOpr = Resources.FileCondition_DisplayFileLE;
            } else {
                endOpr = Resources.FileCondition_DisplayFileLT;
            }

            // 変換
            DateTimeType type = condition.TimeType;
            string result = "";
            if (type == DateTimeType.DateXxxStart || type == DateTimeType.RelativeXxxStart || type == DateTimeType.TimeXxxStart) {                              // 更新＜START
                result = dispName + startOpr + start;
            } else if (type == DateTimeType.DateEndXxx || type == DateTimeType.RelativeEndXxx || type == DateTimeType.TimeEndXxx) {                             // END＜更新
                result = end + endOpr + dispName;
            } else if (type == DateTimeType.DateStartXxxEnd || type == DateTimeType.RelativeStartXxxEnd || type == DateTimeType.TimeStartXxxEnd) {              // START＜更新＜END
                result = start + startOpr + dispName + endOpr + end;
            } else if (type == DateTimeType.DateXxxStartEndXxx || type == DateTimeType.RelativeXxxStartEndXxx || type == DateTimeType.TimeXxxStartEndXxx) {     // 更新＜START & END＜更新
                result = dispName + startOpr + start + Resources.FileCondition_DisplayFileNameAnd2 + end + endOpr + dispName;
            } else if (type == DateTimeType.DateXxx || type == DateTimeType.RelativeXxx || type == DateTimeType.TimeXxx) {                                      // 更新=START
                result = dispName + Resources.FileCondition_DisplayFileEQ + start;
            }
            return result;
        }

        //=========================================================================================
        // 機　能：サイズ情報の文字列を作成して返す
        // 引　数：[in]condition  サイズ情報
        // 戻り値：サイズ情報の文字列
        //=========================================================================================
        public static string CreateSizeCondition(FileSizeCondition condition) {
            // 準備
            string minOpr = "";
            if (condition.IncludeMin) {
                minOpr = Resources.FileCondition_DisplayFileLE;
            } else {
                minOpr = Resources.FileCondition_DisplayFileLT;
            }

            string maxOpr = "";
            if (condition.IncludeMax) {
                maxOpr = Resources.FileCondition_DisplayFileLE;
            } else {
                maxOpr = Resources.FileCondition_DisplayFileLT;
            }
            string minSize = "";
            if (condition.MinSize >= 0) {
                minSize = string.Format("{0:#,0}", condition.MinSize);
            }
            string maxSize = "";
            if (condition.MaxSize >= 0) {
                maxSize = string.Format("{0:#,0}", condition.MaxSize);
            }

            string size = Resources.FileCondition_DisplayFileNameSize;

            // 変換
            string result = "";
            if (condition.SizeType == FileSizeType.XxxSize) {                           // サイズ＜MAX
                result = size + maxOpr + minSize;
            } else if (condition.SizeType == FileSizeType.SizeXxx) {                    // MIN＜サイズ
                result = maxSize + maxOpr + size;
            } else if (condition.SizeType == FileSizeType.SizeXxxSize) {                // MIN＜サイズ＜MAX
                result = minSize + minOpr + size + maxOpr + maxSize;
            } else if (condition.SizeType == FileSizeType.XxxSizeXxx) {                 // サイズ＜MIN&MAX＜サイズ
                result = size + minOpr + minSize + Resources.FileCondition_DisplayFileNameAnd2 + maxSize + maxOpr + size;
            }
            return result;
        }

        //=========================================================================================
        // 機　能：属性情報の文字列を作成して返す
        // 引　数：[in]flag       属性情報
        // 　　　　[in]dispName   項目の表示名
        // 戻り値：属性情報の文字列
        //=========================================================================================
        public static string CreateAttribute(BooleanFlag flag, String attrName) {
            string result = "";
            if (flag.Value) {
                result = string.Format(Resources.FileCondition_DisplayAttrOn, attrName);
            } else {
                result = string.Format(Resources.FileCondition_DisplayAttrOff, attrName);
            }
            return result;
        }
    }
}
