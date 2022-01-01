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
    // クラス：ファイル一覧の条件についてのダイアログでの入力値
    //=========================================================================================
    public class FileConditionDialogInfo : ICloneable {
        // 定義済み条件指定のときtrue、入力されたワイルドカードのときfalse
        private bool m_conditionMode = false;

        // 指定された定義済み条件（ワイルドカード指定のとき空のリスト）
        private List<string> m_selectedConditionList = new List<string>();

        // ワイルドカード指定（定義済み条件指定のとき""）
        private string m_wildCard = "";
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileConditionDialogInfo() {
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]conditionMode  定義済み条件指定のときtrue、入力されたワイルドカードのときfalse
        // 　　　　[in]conditionList  指定された定義済み条件（ワイルドカード指定のとき空のリスト）
        // 　　　　[in]wildCard       ワイルドカード指定（定義済み条件指定のとき""）
        // 　　　　[in]positiveMode   条件に一致したものを表示するときtrue、条件に一致しなかったものを表示するときfalse
        // 戻り値：なし
        //=========================================================================================
        public FileConditionDialogInfo(bool conditionMode, List<string> conditionList, string wildCard, bool positiveMode) {
            m_conditionMode = conditionMode;
            m_selectedConditionList = conditionList;
            m_wildCard = wildCard;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader     読み込み用クラス
        // 　　　　[out]obj       読み込み対象のオブジェクトを返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out FileConditionDialogInfo obj) {
            bool success;
            obj = new FileConditionDialogInfo();
            SettingTag tagName;
            SettingTagType tagType;
            success = loader.ExpectTag(SettingTag.FileListFilterDialogInfo, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            while (true) {
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileListFilterDialogInfo) {
                    break;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileListFilterDialogInfo_ConditionMode) {
                    obj.m_conditionMode = loader.BoolValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileListFilterDialogInfo_ConditionList) {
                    while (true) {
                        success = loader.GetTag(out tagName, out tagType);
                        if (!success) {
                            return false;
                        }
                        if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileListFilterDialogInfo_ConditionList) {
                            break;
                        } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileListFilterDialogInfo_ConditionListName) {
                            obj.m_selectedConditionList.Add(loader.StringValue);
                        }
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileListFilterDialogInfo_WildCard) {
                    obj.m_wildCard = loader.StringValue;
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
        public static bool SaveSetting(SettingSaver saver, FileConditionDialogInfo obj) {
            saver.StartObject(SettingTag.FileListFilterDialogInfo);
            saver.AddBool(SettingTag.FileListFilterDialogInfo_ConditionMode, obj.m_conditionMode);

            saver.StartObject(SettingTag.FileListFilterDialogInfo_ConditionList);
            foreach (string name in obj.m_selectedConditionList) {
                saver.AddString(SettingTag.FileListFilterDialogInfo_ConditionListName, name);
            }
            saver.EndObject(SettingTag.FileListFilterDialogInfo_ConditionList);

            saver.AddString(SettingTag.FileListFilterDialogInfo_WildCard, obj.m_wildCard);
            saver.EndObject(SettingTag.FileListFilterDialogInfo);

            return true;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileConditionDialogInfo obj1, FileConditionDialogInfo obj2) {
            if (obj1.m_conditionMode != obj2.m_conditionMode) {
                return false;
            }
            if (obj1.m_selectedConditionList.Count != obj2.m_selectedConditionList.Count) {
                return false;
            }
            for (int i = 0; i < obj1.m_selectedConditionList.Count; i++) {
                if (obj1.m_selectedConditionList[i] != obj2.m_selectedConditionList[i]) {
                    return false;
                }
            }
            if (obj1.m_wildCard != obj2.m_wildCard) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileConditionDialogInfo clone = new FileConditionDialogInfo();
            clone.m_conditionMode = m_conditionMode;
            foreach (string name in m_selectedConditionList) {
                clone.m_selectedConditionList.Add(name);
            }
            clone.m_wildCard = m_wildCard;

            return clone;
        }

        //=========================================================================================
        // プロパティ：定義済み条件指定のときtrue、入力されたワイルドカードのときfalse
        //=========================================================================================
        public bool ConditionMode {
            get {
                return m_conditionMode;
            }
            set {
                m_conditionMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：指定された定義済み条件（ワイルドカード指定のとき空のリスト）
        //=========================================================================================
        public List<string> SelectedConditionList {
            get {
                return m_selectedConditionList;
            }
            set {
                m_selectedConditionList = value;
            }
        }

        //=========================================================================================
        // プロパティ：ワイルドカード指定（定義済み条件指定のとき""）
        //=========================================================================================
        public string WildCard {
            get {
                return m_wildCard;
            }
            set {
                m_wildCard = value;
            }
        }
    }
}
