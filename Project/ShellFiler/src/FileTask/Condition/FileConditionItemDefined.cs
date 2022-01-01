using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：定義済みのファイル転送条件項目（WindowsにもSSHにも適用可能）
    //=========================================================================================
    public class FileConditionItemDefined : FileConditionItem {
        // 設定名
        private string m_displayName = "";

        // 対象
        private FileConditionTarget m_fileConditionTarget = FileConditionTarget.FileAndFolder;

        // ファイル名：条件
        private FileNameType m_fileNameType = FileNameType.None;

        // ファイル名：ファイル名（指定しないときnull）
        private string m_fileName = null;

        // 更新日時の転送条件
        private DateTimeCondition m_updateTimeCondition = new DateTimeCondition();
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileConditionItemDefined() {
        }

        //=========================================================================================
        // 機　能：設定をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override void ResetCondition() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public override object Clone() {
            FileConditionItemDefined clone = new FileConditionItemDefined();
            clone.m_displayName = m_displayName;
            clone.m_fileConditionTarget = m_fileConditionTarget;
            clone.m_fileNameType = m_fileNameType;
            clone.m_fileName = m_fileName;
            clone.m_updateTimeCondition = (DateTimeCondition)(m_updateTimeCondition.Clone());

            return clone;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]other  比較対象
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public override bool EqualsConfigObject(FileConditionItem other) {
            FileConditionItemDefined obj = (FileConditionItemDefined)other;
            if (m_displayName != obj.m_displayName) {
                return false;
            }
            if (m_fileConditionTarget != obj.m_fileConditionTarget) {
                return false;
            }
            if (m_fileNameType != obj.m_fileNameType) {
                return false;
            }
            if (m_fileName != obj.m_fileName) {
                return false;
            }
            if (!DateTimeCondition.EqualsConfig(m_updateTimeCondition, obj.m_updateTimeCondition)) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：設定が空の状態かどうかを調べる
        // 引　数：なし
        // 戻り値：設定が空の状態のときtrue
        //=========================================================================================
        public override bool IsEmptyCondition() {
            return false;
        }

        //=========================================================================================
        // 機　能：実際のファイルと、このオブジェクトの条件を比較する
        // 引　数：[in]condition   転送条件（キャッシュ済みRegexの管理用）
        // 　　　　[in]file        比較対象のファイル
        // 戻り値：条件に一致するファイルのときtrue
        //=========================================================================================
        public bool CompareFile(CompareCondition condition, IFile file) {
            bool match;

            // 対象
            match = TargetConditionComparetor.CompareFileType(m_fileConditionTarget, file.Attribute.IsDirectory);
            if (!match) {
                return false;
            }

            // ファイル名：条件
            match = TargetConditionComparetor.CompareFileName(condition, m_fileNameType, m_fileName, file);
            if (!match) {
                return false;
            }

            // 更新日時の転送条件
            match = m_updateTimeCondition.CompareFile(file.ModifiedDate);
            if (!match) {
                return false;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：表示文字列を返す
        // 引　数：なし
        // 戻り値：表示文字列のリスト
        //=========================================================================================
        public override string[] ToDisplayString() {
            List<string> result = new List<string>();

            // 対象
            result.Add(ConditionToString.CreateTargetCondition(m_fileConditionTarget));

            // ファイル名
            if (m_fileNameType != FileNameType.None) {
                string name = ConditionToString.CreateFileNameCondition(m_fileNameType, m_fileName);
                result.Add(name);
            }

            // 更新日時
            if (m_updateTimeCondition.TimeType != DateTimeType.None) {
                string strTime = ConditionToString.CreateTimeCondition(m_updateTimeCondition, Resources.FileCondition_DisplayFileNameUpdateTime);
                result.Add(strTime);
            }

            return result.ToArray();
        }

        //=========================================================================================
        // 機　能：ワイルドカード文字列から項目を作成する
        // 引　数：[in]wildCard   ワイルドカード文字列
        // 　　　　[in]fileFolder ファイル／フォルダによる対象区分 
        // 戻り値：作成した項目（ワイルドカード文字列が不適当なときnull）
        //=========================================================================================
        public static FileConditionItemDefined CreateFromWildCard(string wildCard, FileConditionTarget fileFolder) {
            string regStr = WildCardConverter.ConvertWildCardToRegexString(wildCard);
            if (regStr == null) {
                return null;
            }
            FileConditionItemDefined item = new FileConditionItemDefined();
            item.DisplayName = "";
            item.FileConditionTarget = fileFolder;
            item.FileNameType = FileNameType.RegularExpression;
            item.FileName = regStr;
            return item;
        }

        //=========================================================================================
        // 機　能：定義済み項目かどうかを返す
        // 引　数：なし
        // 戻り値：定義済み項目のときtrue
        //=========================================================================================
        public override bool IsDefined() {
            return true;
        }

        //=========================================================================================
        // プロパティ：設定名
        //=========================================================================================
        public override string DisplayName {
            get {
                return m_displayName;
            }
            set {
                m_displayName = value;
            }
        }

        //=========================================================================================
        // プロパティ：対象
        //=========================================================================================
        public override FileConditionTarget FileConditionTarget {
            get {
                return m_fileConditionTarget;
            }
            set {
                m_fileConditionTarget = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル名：条件
        //=========================================================================================
        public override FileNameType FileNameType {
            get {
                return m_fileNameType;
            }
            set {
                m_fileNameType = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル名：ファイル名（指定しないときnull）
        //=========================================================================================
        public override string FileName {
            get {
                return m_fileName;
            }
            set {
                m_fileName = value;
            }
        } 

        //=========================================================================================
        // プロパティ：更新日時の転送条件
        //=========================================================================================
        public override DateTimeCondition UpdateTimeCondition {
            get {
                return m_updateTimeCondition;
            }
            set {
                m_updateTimeCondition = value;
            }
        }
    }
}
