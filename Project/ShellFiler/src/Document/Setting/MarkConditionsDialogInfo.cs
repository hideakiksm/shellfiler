using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Windows;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileViewer;
using ShellFiler.Properties;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：条件付マークの指定（ダイアログでの入力値）
    //=========================================================================================
    public class MarkConditionsDialogInfo : ICloneable {
        // 条件に対するダイアログでの入力値
        private FileConditionDialogInfo m_conditionDialogInfo = new FileConditionDialogInfo();

        // マークの実行方法（XxxxAll）
        private MarkAllFileMode m_markMode = MarkAllFileMode.RevertAll;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MarkConditionsDialogInfo() {
        }
        
        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            MarkConditionsDialogInfo clone = new MarkConditionsDialogInfo();
            clone.m_conditionDialogInfo = (FileConditionDialogInfo)(m_conditionDialogInfo.Clone());;
            clone.m_markMode = m_markMode;

            return clone;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(MarkConditionsDialogInfo obj1, MarkConditionsDialogInfo obj2) {
            if (!FileConditionDialogInfo.EqualsConfig(obj1.m_conditionDialogInfo, obj2.m_conditionDialogInfo)) {
                return false;
            }
            if (obj1.m_markMode != obj2.m_markMode) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader     読み込み用クラス
        // 　　　　[out]obj       読み込み対象のオブジェクトを返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out MarkConditionsDialogInfo obj) {
            bool success;
            obj = new MarkConditionsDialogInfo();
            SettingTag tagName;
            SettingTagType tagType;
            success = loader.ExpectTag(SettingTag.MarkConditionsDialogInfo, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            while (true) {
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.MarkConditionsDialogInfo) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.MarkConditionsDialogInfo_DialogInfo) {
                    success = FileConditionDialogInfo.LoadSetting(loader, out obj.m_conditionDialogInfo);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.MarkConditionsDialogInfo_DialogInfo, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.MarkConditionsDialogInfo_MarkMode) {
                    MarkAllFileMode markMode = MarkAllFileMode.FromString(loader.StringValue);
                    if (markMode == MarkAllFileMode.RevertAll || markMode == MarkAllFileMode.SelectAll || markMode == MarkAllFileMode.ClearAll) {
                        obj.m_markMode = markMode;
                    } else {
                        obj.m_markMode = MarkAllFileMode.RevertAll;
                    }
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
        public static bool SaveSetting(SettingSaver saver, MarkConditionsDialogInfo obj) {
            bool success;
            saver.StartObject(SettingTag.MarkConditionsDialogInfo);

            saver.StartObject(SettingTag.MarkConditionsDialogInfo_DialogInfo);
            success = FileConditionDialogInfo.SaveSetting(saver, obj.m_conditionDialogInfo);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.MarkConditionsDialogInfo_DialogInfo);

            saver.AddString(SettingTag.MarkConditionsDialogInfo_MarkMode, obj.m_markMode.StringName);
            saver.EndObject(SettingTag.MarkConditionsDialogInfo);

            return true;
        }

        //=========================================================================================
        // プロパティ：条件に対するダイアログでの入力値
        //=========================================================================================
        public FileConditionDialogInfo ConditionDialogInfo {
            get {
                return m_conditionDialogInfo;
            }
            set {
                m_conditionDialogInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：マークの実行方法
        //=========================================================================================
        public MarkAllFileMode MarkMode {
            get {
                return m_markMode;
            }
            set {
                m_markMode = value;
            }
        }
    }
}
