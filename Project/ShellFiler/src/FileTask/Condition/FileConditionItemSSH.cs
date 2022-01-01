using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：SSHのファイル転送条件項目
    //=========================================================================================
    public class FileConditionItemSSH : FileConditionItem {
        // 対象
        private FileConditionTarget m_fileConditionTarget;

        // 表示名
        private string m_displayName;

        // ファイル名：条件
        private FileNameType m_fileNameType;

        // ファイル名：ファイル名（指定しないときnull）
        private string m_fileName = null;

        // 更新日時の転送条件
        private DateTimeCondition m_updateTimeCondition;

        // アクセス日時の転送条件
        private DateTimeCondition m_accessTimeCondition;

        // サイズの転送条件
        private FileSizeCondition m_fileSizeCondition;

        // オーナー読み込み可能属性の条件（指定しないときnull）
        private BooleanFlag m_ownerRead;

        // オーナー書き込み属性の条件（指定しないときnull）
        private BooleanFlag m_ownerWrite;

        // オーナー実行属性の条件（指定しないときnull）
        private BooleanFlag m_ownerExecute;

        // グループ読み込み可能属性の条件（指定しないときnull）
        private BooleanFlag m_groupRead;

        // グループ書き込み属性の条件（指定しないときnull）
        private BooleanFlag m_groupWrite;

        // グループ実行属性の条件（指定しないときnull）
        private BooleanFlag m_groupExecute;

        // 他人読み込み可能属性の条件（指定しないときnull）
        private BooleanFlag m_otherRead;

        // 他人書き込み属性の条件（指定しないときnull）
        private BooleanFlag m_otherWrite;

        // 他人実行属性の条件（指定しないときnull）
        private BooleanFlag m_otherExecute;

        // シンボリックリンク属性の条件（指定しないときnull）
        private BooleanFlag m_symbolicLink;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileConditionItemSSH() {
            ResetCondition();
        }

        //=========================================================================================
        // 機　能：設定をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override void ResetCondition() {
            m_fileConditionTarget = FileConditionTarget.FileAndFolder;
            m_displayName = "";
            m_fileNameType = FileNameType.None;
            m_fileName = null;
            m_updateTimeCondition = new DateTimeCondition();
            m_accessTimeCondition = new DateTimeCondition();
            m_fileSizeCondition = new FileSizeCondition();
            m_ownerRead = null;
            m_ownerWrite = null;
            m_ownerExecute = null;
            m_groupRead = null;
            m_groupWrite = null;
            m_groupExecute = null;
            m_otherRead = null;
            m_otherWrite = null;
            m_otherExecute = null;
            m_symbolicLink = null;
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
            m_accessTimeCondition.CleanupField();
            m_fileSizeCondition.CleanupField();
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public override object Clone() {
            FileConditionItemSSH clone = new FileConditionItemSSH();
            clone.m_displayName = m_displayName;
            clone.m_fileConditionTarget = m_fileConditionTarget;
            clone.m_fileNameType = m_fileNameType;
            clone.m_fileName = m_fileName;
            clone.m_updateTimeCondition = (DateTimeCondition)(m_updateTimeCondition.Clone());
            clone.m_accessTimeCondition = (DateTimeCondition)(m_accessTimeCondition.Clone());
            clone.m_fileSizeCondition = (FileSizeCondition)(m_fileSizeCondition.Clone());
            clone.m_ownerRead = BooleanFlag.CreateClone(m_ownerRead);
            clone.m_ownerWrite = BooleanFlag.CreateClone(m_ownerWrite);
            clone.m_ownerExecute = BooleanFlag.CreateClone(m_ownerExecute);
            clone.m_groupRead = BooleanFlag.CreateClone(m_groupRead);
            clone.m_groupWrite = BooleanFlag.CreateClone(m_groupWrite);
            clone.m_groupExecute = BooleanFlag.CreateClone(m_groupExecute);
            clone.m_otherRead = BooleanFlag.CreateClone(m_otherRead);
            clone.m_otherWrite = BooleanFlag.CreateClone(m_otherWrite);
            clone.m_otherExecute = BooleanFlag.CreateClone(m_otherExecute);
            clone.m_symbolicLink = BooleanFlag.CreateClone(m_symbolicLink);

            return clone;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]other  比較対象
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public override bool EqualsConfigObject(FileConditionItem other) {
            FileConditionItemSSH obj = (FileConditionItemSSH)other;
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
            if (!DateTimeCondition.EqualsConfig(m_accessTimeCondition, obj.m_accessTimeCondition)) {
                return false;
            }
            if (!FileSizeCondition.EqualsConfig(m_fileSizeCondition, obj.m_fileSizeCondition)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_ownerRead, obj.m_ownerRead)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_ownerWrite, obj.m_ownerWrite)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_ownerExecute, obj.m_ownerExecute)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_groupRead, obj.m_groupRead)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_groupWrite, obj.m_groupWrite)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_groupExecute, obj.m_groupExecute)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_otherRead, obj.m_otherRead)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_otherWrite, obj.m_otherWrite)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_otherExecute, obj.m_otherExecute)) {
                return false;
            }
            if (!BooleanFlag.Equals(m_symbolicLink, obj.m_symbolicLink)) {
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
            if (m_fileNameType != FileNameType.None && m_fileName != "") {      // 一時条件のデフォルトは""
                return false;
            }
            if (m_updateTimeCondition.TimeType != DateTimeType.None) {
                return false;
            }
            if (m_accessTimeCondition.TimeType != DateTimeType.None) {
                return false;
            }
            if (m_fileSizeCondition.SizeType != FileSizeType.None) {
                return false;
            }
            if (m_ownerRead != null) {
                return false;
            }
            if (m_ownerWrite != null) {
                return false;
            }
            if (m_ownerExecute != null) {
                return false;
            }
            if (m_groupRead != null) {
                return false;
            }
            if (m_groupWrite != null) {
                return false;
            }
            if (m_groupExecute != null) {
                return false;
            }
            if (m_otherRead != null) {
                return false;
            }
            if (m_otherWrite != null) {
                return false;
            }
            if (m_otherExecute != null) {
                return false;
            }
            if (m_symbolicLink != null) {
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
        public static FileConditionItemSSH GetDefault() {
            return new FileConditionItemSSH();
        }
        
        //=========================================================================================
        // 機　能：実際のファイルと、このオブジェクトの条件を比較する
        // 引　数：[in]condition   転送条件（キャッシュ済みRegexの管理用）
        // 　　　　[in]file        比較対象のファイル
        // 戻り値：条件に一致するファイルのときtrue
        //=========================================================================================
        public bool CompareFile(CompareCondition condition, SFTPFile file) {
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

            // オーナー読み込み可能属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_ownerRead, file.AttributeOwnerRead);
            if (!match) {
                return false;
            }

            // オーナー書き込み属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_ownerWrite, file.AttributeOwnerWrite);
            if (!match) {
                return false;
            }

            // オーナー実行属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_ownerExecute, file.AttributeOwnerExecute);
            if (!match) {
                return false;
            }

            // グループ読み込み可能属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_groupRead, file.AttributeGroupRead);
            if (!match) {
                return false;
            }

            // グループ書き込み属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_groupWrite, file.AttributeGroupWrite);
            if (!match) {
                return false;
            }

            // グループ実行属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_groupExecute, file.AttributeGroupExecute);
            if (!match) {
                return false;
            }

            // 他人読み込み可能属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_otherRead, file.AttributeOtherRead);
            if (!match) {
                return false;
            }

            // 他人書き込み属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_otherWrite, file.AttributeOtherWrite);
            if (!match) {
                return false;
            }

            // 他人実行属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_otherExecute, file.AttributeOtherExecute);
            if (!match) {
                return false;
            }

            // シンボリックリンク属性の条件
            match = TargetConditionComparetor.CompareFileAttribute(m_symbolicLink, file.Attribute.IsSymbolicLink);
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
            return new string[0];
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, List<FileConditionItemSSH> obj) {
            bool success;

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.FileCondition_ItemSSHList, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileCondition_ItemSSHList) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_ItemSSH) {
                    FileConditionItemSSH sshItem;
                    success = LoadSettingItem(loader, SettingTag.FileCondition_ItemSSH, out sshItem);
                    if (!success) {
                        return false;
                    }
                    success = sshItem.Validate();
                    if (!success) {
                        loader.SetWarning(Resources.FileCondition_LoaderSSHFail);
                    } else {
                        obj.Add(sshItem);
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
        public static bool LoadSettingItem(SettingLoader loader, SettingTag endTag, out FileConditionItemSSH obj) {
            bool success;
            obj = new FileConditionItemSSH();

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
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeOwnerRead) {
                    obj.m_ownerRead = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeOwnerWrite) {
                    obj.m_ownerWrite = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeOwnerExecute) {
                    obj.m_ownerExecute = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeGroupRead) {
                    obj.m_groupRead = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeGroupWrite) {
                    obj.m_groupWrite = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeGroupExecute) {
                    obj.m_groupExecute = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeOtherRead) {
                    obj.m_otherRead = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeOtherWrite) {
                    obj.m_otherWrite = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeOtherExecute) {
                    obj.m_otherExecute = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_ItemAttributeSymbolicLink) {
                    obj.m_symbolicLink = new BooleanFlag(loader.BoolValue);
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
        public static bool SaveSetting(SettingSaver saver, List<FileConditionItemSSH> obj) {
            bool success;
            saver.StartObject(SettingTag.FileCondition_ItemSSHList);

            foreach (FileConditionItemSSH item in obj) {
                saver.StartObject(SettingTag.FileCondition_ItemSSH);
                success = SaveSettingItem(saver, item);
                if (!success) {
                    return false;
                }
                saver.EndObject(SettingTag.FileCondition_ItemSSH);
            }
            
            saver.EndObject(SettingTag.FileCondition_ItemSSHList);
            
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに項目1件を保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSettingItem(SettingSaver saver, FileConditionItemSSH obj) {
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


            if (obj.m_ownerRead != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeOwnerRead,    obj.m_ownerRead.Value);
            }
            if (obj.m_ownerWrite != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeOwnerWrite,   obj.m_ownerWrite.Value);
            }
            if (obj.m_ownerExecute != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeOwnerExecute, obj.m_ownerExecute.Value);
            }
            if (obj.m_groupRead != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeGroupRead,    obj.m_groupRead.Value);
            }
            if (obj.m_groupWrite != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeGroupWrite,   obj.m_groupWrite.Value);
            }
            if (obj.m_groupExecute != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeGroupExecute, obj.m_groupExecute.Value);
            }
            if (obj.m_otherRead != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeOtherRead,    obj.m_otherRead.Value);
            }
            if (obj.m_otherWrite != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeOtherWrite,   obj.m_otherWrite.Value);
            }
            if (obj.m_otherExecute != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeOtherExecute, obj.m_otherExecute.Value);
            }
            if (obj.m_symbolicLink != null) {
                saver.AddBool(SettingTag.FileCondition_ItemAttributeSymbolicLink, obj.m_symbolicLink.Value);
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
        // プロパティ：オーナー読み込み可能属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag OwnerRead {
            get {
                return m_ownerRead;
            }
            set {
                m_ownerRead = value;
            }
        }

        //=========================================================================================
        // プロパティ：オーナー書き込み属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag OwnerWrite {
            get {
                return m_ownerWrite;
            }
            set {
                m_ownerWrite = value;
            }
        }

        //=========================================================================================
        // プロパティ：オーナー実行属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag OwnerExecute {
            get {
                return m_ownerExecute;
            }
            set {
                m_ownerExecute = value;
            }
        }

        //=========================================================================================
        // プロパティ：グループ読み込み可能属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag GroupRead {
            get {
                return m_groupRead;
            }
            set {
                m_groupRead = value;
            }
        }

        //=========================================================================================
        // プロパティ：グループ書き込み属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag GroupWrite {
            get {
                return m_groupWrite;
            }
            set {
                m_groupWrite = value;
            }
        }

        //=========================================================================================
        // プロパティ：グループ実行属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag GroupExecute {
            get {
                return m_groupExecute;
            }
            set {
                m_groupExecute = value;
            }
        }

        //=========================================================================================
        // プロパティ：他人読み込み可能属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag OtherRead {
            get {
                return m_otherRead;
            }
            set {
                m_otherRead = value;
            }
        }

        //=========================================================================================
        // プロパティ：他人書き込み属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag OtherWrite {
            get {
                return m_otherWrite;
            }
            set {
                m_otherWrite = value;
            }
        }

        //=========================================================================================
        // プロパティ：他人実行属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag OtherExecute {
            get {
                return m_otherExecute;
            }
            set {
                m_otherExecute = value;
            }
        }

        //=========================================================================================
        // プロパティ：シンボリックリンク属性の条件（指定しないときnull）
        //=========================================================================================
        public BooleanFlag SymbolicLink {
            get {
                return m_symbolicLink;
            }
            set {
                m_symbolicLink = value;
            }
        }
    }
}
