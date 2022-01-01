using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：Windowsのファイル転送条件項目
    //=========================================================================================
    public class FileConditionItemWindows : FileConditionItem {
        // 設定名
        private string m_displayName;

        // 対象
        private FileConditionTarget m_fileConditionTarget;

        // ファイル名：条件
        private FileNameType m_fileNameType;

        // ファイル名：ファイル名（指定しないときnull）
        private string m_fileName;

        // 更新日時の転送条件
        private DateTimeCondition m_updateTimeCondition;

        // 作成日時の転送条件
        private DateTimeCondition m_createTimeCondition;

        // アクセス日時の転送条件
        private DateTimeCondition m_accessTimeCondition;

        // サイズの転送条件
        private FileSizeCondition m_fileSizeCondition;

        // 読み込み専用属性の条件（指定しないときnull）
        private BooleanFlag m_attrReadOnly;

        // 隠し属性の条件（指定しないときnull）
        private BooleanFlag m_attrHidden;

        // アーカイブ属性の条件（指定しないときnull）
        private BooleanFlag m_attrArchive;

        // システム属性の条件（指定しないときnull）
        private BooleanFlag m_attrSystem;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileConditionItemWindows() {
            ResetCondition();
        }

        //=========================================================================================
        // 機　能：設定をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override void ResetCondition() {
            m_displayName = "";
            m_fileConditionTarget = FileConditionTarget.FileAndFolder;
            m_fileNameType = FileNameType.None;
            m_fileName = null;
            m_updateTimeCondition = new DateTimeCondition();
            m_createTimeCondition = new DateTimeCondition();
            m_accessTimeCondition = new DateTimeCondition();
            m_fileSizeCondition = new FileSizeCondition();
            m_attrReadOnly = null;
            m_attrHidden = null;
            m_attrArchive = null;
            m_attrSystem = null;
        }

        //=========================================================================================
        // 機　能：UI設定用のデフォルト値を設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetUIDefaultValue() {
            if (m_fileNameType == FileNameType.None) {
                m_fileName = "";
            }
            m_updateTimeCondition.SetUIDefaultValue();
            m_createTimeCondition.SetUIDefaultValue();
            m_accessTimeCondition.SetUIDefaultValue();
            m_fileSizeCondition.SetUIDefaultValue();
        }
        
        //=========================================================================================
        // 機　能：UI設定用の値から不要部分を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CleanupField() {
            if (m_fileNameType == FileNameType.None) {
                m_fileName = null;
            }
            m_updateTimeCondition.CleanupField();
            m_createTimeCondition.CleanupField();
            m_accessTimeCondition.CleanupField();
            m_fileSizeCondition.CleanupField();
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public override object Clone() {
            FileConditionItemWindows clone = new FileConditionItemWindows();
            clone.m_displayName = m_displayName;
            clone.m_fileConditionTarget = m_fileConditionTarget;
            clone.m_fileNameType = m_fileNameType;
            clone.m_fileName = m_fileName;
            clone.m_updateTimeCondition = (DateTimeCondition)(m_updateTimeCondition.Clone());
            clone.m_createTimeCondition = (DateTimeCondition)(m_createTimeCondition.Clone());
            clone.m_accessTimeCondition = (DateTimeCondition)(m_accessTimeCondition.Clone());
            clone.m_fileSizeCondition = (FileSizeCondition)(m_fileSizeCondition.Clone());
            clone.m_attrReadOnly = BooleanFlag.CreateClone(m_attrReadOnly);
            clone.m_attrHidden = BooleanFlag.CreateClone(m_attrHidden);
            clone.m_attrArchive = BooleanFlag.CreateClone(m_attrArchive);
            clone.m_attrSystem = BooleanFlag.CreateClone(m_attrSystem);

            return clone;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]other  比較対象
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public override bool EqualsConfigObject(FileConditionItem other) {
            FileConditionItemWindows obj = (FileConditionItemWindows)other;
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
            if (!DateTimeCondition.EqualsConfig(m_createTimeCondition, obj.m_createTimeCondition)) {
                return false;
            }
            if (!DateTimeCondition.EqualsConfig(m_accessTimeCondition, obj.m_accessTimeCondition)) {
                return false;
            }
            if (!FileSizeCondition.EqualsConfig(m_fileSizeCondition, obj.m_fileSizeCondition)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_attrReadOnly, obj.m_attrReadOnly)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_attrHidden, obj.m_attrHidden)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_attrArchive, obj.m_attrArchive)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_attrSystem, obj.m_attrSystem)) {
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
            if (m_createTimeCondition.TimeType != DateTimeType.None) {
                string strTime = ConditionToString.CreateTimeCondition(m_createTimeCondition, Resources.FileCondition_DisplayFileNameCreateTime);
                result.Add(strTime);
            }
            if (m_accessTimeCondition.TimeType != DateTimeType.None) {
                string strTime = ConditionToString.CreateTimeCondition(m_accessTimeCondition, Resources.FileCondition_DisplayFileNameAccessTime);
                result.Add(strTime);
            }

            // ファイルサイズ
            if (m_fileSizeCondition.SizeType != FileSizeType.None) {
                string strSize = ConditionToString.CreateSizeCondition(m_fileSizeCondition);
                result.Add(strSize);
            }

            // 属性
            if (m_attrReadOnly != null) {
                string strAttr = ConditionToString.CreateAttribute(m_attrReadOnly, Resources.FileCondition_DisplayFileNameReadonly);
                result.Add(strAttr);
            }
            if (m_attrHidden != null) {
                string strAttr = ConditionToString.CreateAttribute(m_attrHidden, Resources.FileCondition_DisplayFileNameHidden);
                result.Add(strAttr);
            }
            if (m_attrArchive != null) {
                string strAttr = ConditionToString.CreateAttribute(m_attrArchive, Resources.FileCondition_DisplayFileNameArchive);
                result.Add(strAttr);
            }
            if (m_attrSystem != null) {
                string strAttr = ConditionToString.CreateAttribute(m_attrSystem, Resources.FileCondition_DisplayFileNameSystem);
                result.Add(strAttr);
            }

            return result.ToArray();
        }

        //=========================================================================================
        // 機　能：設定が空の状態かどうかを調べる
        // 引　数：なし
        // 戻り値：設定が空の状態のときtrue
        //=========================================================================================
        public override bool IsEmptyCondition() {
            if (m_fileNameType != FileNameType.None && m_fileName != "") {      // 一時条件のデフォルトは""
                return false;
            }
            if (m_updateTimeCondition.TimeType != DateTimeType.None) {
                return false;
            }
            if (m_createTimeCondition.TimeType != DateTimeType.None) {
                return false;
            }
            if (m_accessTimeCondition.TimeType != DateTimeType.None) {
                return false;
            }
            if (m_fileSizeCondition.SizeType != FileSizeType.None) {
                return false;
            }
            if (m_attrReadOnly != null) {
                return false;
            }
            if (m_attrHidden != null) {
                return false;
            }
            if (m_attrArchive != null) {
                return false;
            }
            if (m_attrSystem != null) {
                return false;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：内容に矛盾がないかどうかを検証する
        // 引　数：なし
        // 戻り値：内容が正しいときtrue
        //=========================================================================================
        private bool Validate() {
            if (m_displayName == null) {
                return false;
            }
            if (m_fileConditionTarget == null) {
                return false;
            }
            if (m_fileNameType == null) {
                return false;
            }
            if (m_fileNameType != FileNameType.None) {
                if (m_fileName == null || m_fileName == "" || !TargetConditionComparetor.ValidateFileName(m_fileNameType, m_fileName)) {
                    return false;
                }
            } else {
                if (m_fileName != null) {
                    return false;
                }
            }
            if (m_updateTimeCondition == null || m_updateTimeCondition.Validate() == false) {
                return false;
            }
            if (m_createTimeCondition == null || m_createTimeCondition.Validate() == false) {
                return false;
            }
            if (m_accessTimeCondition == null || m_accessTimeCondition.Validate() == false) {
                return false;
            }
            if (m_fileSizeCondition == null || m_fileSizeCondition.Validate() == false) {
                return false;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：デフォルトの設定を返す
        // 引　数：なし
        // 戻り値：デフォルトの設定
        //=========================================================================================
        public static FileConditionItemWindows GetDefault() {
            return new FileConditionItemWindows();
        }

        //=========================================================================================
        // 機　能：実際のファイルと、このオブジェクトの条件を比較する
        // 引　数：[in]condition   転送条件（キャッシュ済みRegexの管理用）
        // 　　　　[in]file        比較対象のファイル
        // 戻り値：条件に一致するファイルのときtrue
        //=========================================================================================
        public bool CompareFile(CompareCondition condition, WindowsFile file) {
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

            // 作成日時の転送条件
            match = m_createTimeCondition.CompareFile(file.CreationDate);
            if (!match) {
                return false;
            }

            // アクセス日時の転送条件
            match = m_accessTimeCondition.CompareFile(file.AccessDate);
            if (!match) {
                return false;
            }
        
            // サイズの転送条件
            match = m_fileSizeCondition.CompareFile(file.FileSize);
            if (!match) {
                return false;
            }

            // 読み込み専用属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_attrReadOnly, file.Attribute.IsReadonly);
            if (!match) {
                return false;
            }

            // 隠し属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_attrHidden, file.Attribute.IsHidden);
            if (!match) {
                return false;
            }

            // アーカイブ属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_attrArchive, file.Attribute.IsArchive);
            if (!match) {
                return false;
            }

            // システム属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_attrSystem, file.Attribute.IsSystem);
            if (!match) {
                return false;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, List<FileConditionItemWindows> obj) {
            bool success;

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.FileCondition_ItemWindowsList, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileCondition_ItemWindowsList) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_ItemWindows) {
                    FileConditionItemWindows winItem;
                    success = LoadSettingItem(loader, SettingTag.FileCondition_ItemWindows, out winItem);
                    if (!success) {
                        return false;
                    }
                    success = winItem.Validate();
                    if (!success) {
                        loader.SetWarning(Resources.FileCondition_LoaderWindowsFail);
                    } else {
                        obj.Add(winItem);
                    }
                }
            }
            return true;
        }
        
        //=========================================================================================
        // 機　能：ファイルから項目1件を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]endTag  期待される終了タグ
        // 　　　　[out]obj    読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSettingItem(SettingLoader loader, SettingTag endTag, out FileConditionItemWindows obj) {
            bool success;
            obj = new FileConditionItemWindows();

            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == endTag) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileCondition_ItemDisplayName) {
                    obj.m_displayName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileCondition_ItemTarget) {
                    obj.m_fileConditionTarget = FileConditionTarget.FromString(loader.StringValue);     // NGのときnullとしてValidate()
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileCondition_ItemFileNameType) {
                    obj.m_fileNameType = FileNameType.FromString(loader.StringValue);                   // NGのときnullとしてValidate()
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileCondition_ItemFileName) {
                    obj.m_fileName = loader.StringValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_ItemUpdateTime) {
                    DateTimeCondition condition;
                    success = DateTimeCondition.LoadSetting(loader, SettingTag.FileCondition_ItemUpdateTime, out condition);
                    if (!success) {
                        return false;
                    }
                    obj.m_updateTimeCondition = condition;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_ItemCreateTime) {
                    DateTimeCondition condition;
                    success = DateTimeCondition.LoadSetting(loader, SettingTag.FileCondition_ItemCreateTime, out condition);
                    if (!success) {
                        return false;
                    }
                    obj.m_createTimeCondition = condition;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_ItemAccessTime) {
                    DateTimeCondition condition;
                    success = DateTimeCondition.LoadSetting(loader, SettingTag.FileCondition_ItemAccessTime, out condition);
                    if (!success) {
                        return false;
                    }
                    obj.m_accessTimeCondition = condition;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_ItemFileSize) {
                    FileSizeCondition condition;
                    success = FileSizeCondition.LoadSetting(loader, SettingTag.FileCondition_ItemFileSize, out condition);
                    if (!success) {
                        return false;
                    }
                    obj.m_fileSizeCondition = condition;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeReadOnly) {
                    obj.m_attrReadOnly = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeHidden) {
                    obj.m_attrHidden = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeArchive) {
                    obj.m_attrArchive = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeSystem) {
                    obj.m_attrSystem = new BooleanFlag(loader.BoolValue);
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, List<FileConditionItemWindows> obj) {
            bool success;
            saver.StartObject(SettingTag.FileCondition_ItemWindowsList);

            foreach (FileConditionItemWindows item in obj) {
                saver.StartObject(SettingTag.FileCondition_ItemWindows);
                success = SaveSettingItem(saver, item);
                if (!success) {
                    return false;
                }
                saver.EndObject(SettingTag.FileCondition_ItemWindows);
            }
            
            saver.EndObject(SettingTag.FileCondition_ItemWindowsList);
            
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに項目1件を保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSettingItem(SettingSaver saver, FileConditionItemWindows obj) {
            bool success;
            saver.AddString(SettingTag.FileCondition_ItemDisplayName, obj.m_displayName);
            saver.AddString(SettingTag.FileCondition_ItemTarget, obj.m_fileConditionTarget.StringName);
            saver.AddString(SettingTag.FileCondition_ItemFileNameType, obj.m_fileNameType.StringName);
            if (obj.m_fileName != null) {
                saver.AddString(SettingTag.FileCondition_ItemFileName, obj.m_fileName);
            }

            saver.StartObject(SettingTag.FileCondition_ItemUpdateTime);
            success = DateTimeCondition.SaveSetting(saver, obj.m_updateTimeCondition);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.FileCondition_ItemUpdateTime);

            saver.StartObject(SettingTag.FileCondition_ItemCreateTime);
            success = DateTimeCondition.SaveSetting(saver, obj.m_createTimeCondition);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.FileCondition_ItemCreateTime);

            saver.StartObject(SettingTag.FileCondition_ItemAccessTime);
            success = DateTimeCondition.SaveSetting(saver, obj.m_accessTimeCondition);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.FileCondition_ItemAccessTime);

            saver.StartObject(SettingTag.FileCondition_ItemFileSize);
            success = FileSizeCondition.SaveSetting(saver, obj.m_fileSizeCondition);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.FileCondition_ItemFileSize);


            if (obj.m_attrReadOnly != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeReadOnly, obj.m_attrReadOnly.Value);
            }
            if (obj.m_attrHidden != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeHidden, obj.m_attrHidden.Value);
            }
            if (obj.m_attrArchive != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeArchive, obj.m_attrArchive.Value);
            }
            if (obj.m_attrSystem != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeSystem, obj.m_attrSystem.Value);
            }

            return true;
        }

        //=========================================================================================
        // 機　能：定義済み項目かどうかを返す
        // 引　数：なし
        // 戻り値：定義済み項目のときtrue
        //=========================================================================================
        public override bool IsDefined() {
            return false;
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

        //=========================================================================================
        // プロパティ：作成日時の転送条件
        //=========================================================================================
        public DateTimeCondition CreateTimeCondition {
            get {
                return m_createTimeCondition;
            }
            set {
                m_createTimeCondition = value;
            }
        }

        //=========================================================================================
        // プロパティ：アクセス日時の転送条件
        //=========================================================================================
        public DateTimeCondition AccessTimeCondition {
            get {
                return m_accessTimeCondition;
            }
            set {
                m_accessTimeCondition = value;
            }
        }

        //=========================================================================================
        // プロパティ：サイズの転送条件
        //=========================================================================================
        public FileSizeCondition FileSizeCondition {
            get {
                return m_fileSizeCondition;
            }
            set {
                m_fileSizeCondition = value;
            }
        }

        //=========================================================================================
        // プロパティ：読み込み専用属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag AttrReadOnly {
            get {
                return m_attrReadOnly;
            }
            set {
                m_attrReadOnly = value;
            }
        }

        //=========================================================================================
        // プロパティ：隠し属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag AttrHidden {
            get {
                return m_attrHidden;
            }
            set {
                m_attrHidden = value;
            }
        }

        //=========================================================================================
        // プロパティ：アーカイブ属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag AttrArchive {
            get {
                return m_attrArchive;
            }
            set {
                m_attrArchive = value;
            }
        }

        //=========================================================================================
        // プロパティ：システム属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag AttrSystem {
            get {
                return m_attrSystem;
            }
            set {
                m_attrSystem = value;
            }
        }
    }
}
