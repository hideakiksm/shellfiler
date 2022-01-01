using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Windows;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.FileTask.Condition;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileViewer;
using ShellFiler.Properties;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイル一覧のフィルター（ファイル一覧での比較用）
    //=========================================================================================
    public class FileListFilterMode : ICloneable {
        // ダイアログでの入力値
        private FileConditionDialogInfo m_dialogInfo = new FileConditionDialogInfo();

        // 使用するフィルター
        private List<FileConditionItem> m_conditionList = new List<FileConditionItem>();

        // 条件に一致するファイルを表示するときtrue
        private bool m_isPositive = true;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListFilterMode() {
        }
    
        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileListFilterMode obj = new FileListFilterMode();
            obj.m_dialogInfo = (FileConditionDialogInfo)(m_dialogInfo.Clone());
            foreach (FileConditionItem item in m_conditionList) {
                obj.m_conditionList.Add((FileConditionItem)(item.Clone()));
            }
            obj.m_isPositive = m_isPositive;
            return obj;
        }

        //=========================================================================================
        // 機　能：条件が指定されたファイルシステムに対して有効かどうかを調べる
        // 引　数：[in]fileSystem   チェックするファイルシステム
        // 戻り値：対象のときtrue
        //=========================================================================================
        public bool IsSupportFileSystem(FileSystemID fileSystem) {
            foreach (FileConditionItem item in m_conditionList) {
                if (item is FileConditionItemDefined) {
                    ;
                } else if (FileSystemID.IsWindows(fileSystem) && item is FileConditionItemWindows) {
                    ;
                } else if (FileSystemID.IsSSH(fileSystem) && item is FileConditionItemSSH) {
                    ;
                } else {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：アドレスバーの表示用文字列を取得する
        // 引　数：なし
        // 戻り値：アドレスバーの表示用文字列
        //=========================================================================================
        public string GetDisplayString() {
            if (m_isPositive && m_conditionList.Count == 1 && m_conditionList[0] is FileConditionItemDefined) {
                FileConditionItemDefined item = (FileConditionItemDefined)m_conditionList[0];
                if (item.FileConditionTarget == FileConditionTarget.FileOnly &&
                    item.FileNameType == FileNameType.RegularExpression &&
                    item.UpdateTimeCondition.TimeType == DateTimeType.None &&
                    item.DisplayName == "") {
                    // ワイルドカード指定の場合
                    string wildEscape = WildCardConverter.ConvertRegexStringToWildCard(item.FileName);
                    if (wildEscape != null) {
                        return wildEscape;
                    }
                }
            }
            return "<filter>";
        }

        //=========================================================================================
        // プロパティ：ダイアログでの入力値
        //=========================================================================================
        public FileConditionDialogInfo DialogInfo {
            get {
                return m_dialogInfo;
            }
            set {
                m_dialogInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：使用するフィルター
        //=========================================================================================
        public List<FileConditionItem> ConditionList {
            get {
                return m_conditionList;
            }
            set {
                m_conditionList = value;
            }
        }

        //=========================================================================================
        // プロパティ：条件に一致するファイルを表示するときtrue
        //=========================================================================================
        public bool IsPositive {
            get {
                return m_isPositive;
            }
            set {
                m_isPositive = value;
            }
        }
    }
}
