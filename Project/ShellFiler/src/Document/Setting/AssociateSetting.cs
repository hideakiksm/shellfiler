using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Archive;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.Open;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.Command.FileList.Window;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.GraphicsViewer.View;
using ShellFiler.Properties;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：関連付けのカスタマイズ情報
    //=========================================================================================
    public class AssociateSetting : ICloneable {
        // 関連づけ設定の最大件数
        private const int ASSOCIATE_SETTING_COUNT = 8;

        // 関連付け定義の一覧（常にASSOCIATE_SETTING_COUNT個の要素、設定がない要素はnull）
        private List<AssociateKeySetting> m_assocSettingList;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AssociateSetting() {
            ClearAll();
        }

        //=========================================================================================
        // 機　能：すべての設定を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearAll() {
            m_assocSettingList = new List<AssociateKeySetting>();
            for (int i = 0; i < ASSOCIATE_SETTING_COUNT; i++) {
                AssociateKeySetting setting = new AssociateKeySetting();
                KeySetting.InitializeAssociateDefault(setting, i, false);
                m_assocSettingList.Add(setting);
            }
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(AssociateSetting obj1, AssociateSetting obj2) {
            if (obj1.m_assocSettingList.Count != obj2.m_assocSettingList.Count) {
                return false;
            }
            for (int i = 0; i < obj1.m_assocSettingList.Count; i++) {
                if (!AssociateKeySetting.EqualsConfig(obj1.m_assocSettingList[i], obj2.m_assocSettingList[i])) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            AssociateSetting clone = new AssociateSetting();
            for (int i = 0; i < m_assocSettingList.Count; i++) {
                clone.m_assocSettingList[i] = (AssociateKeySetting)(m_assocSettingList[i].Clone());
            }
            return clone;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader     読み込み用クラス
        // 　　　　[in]obj        読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, AssociateSetting obj) {
            obj.ClearAll();
            bool success;
            success = loader.ExpectTag(SettingTag.KeySetting_AssocSettingList, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.KeySetting_AssocSettingList) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_AssocSettingListItem) {
                    success = loader.ExpectTag(SettingTag.KeySetting_AssocSettingListIndex, SettingTagType.IntValue);
                    if (!success) {
                        return false;
                    }
                    int index = loader.IntValue;
                    if (index < 0 || index > ASSOCIATE_SETTING_COUNT) {
                        return false;
                    }
                    obj.m_assocSettingList[index].ClearSetting();
                    KeySetting.InitializeAssociateDefault(obj.m_assocSettingList[index], index, false);
                    success = AssociateKeySetting.LoadSetting(loader, obj.m_assocSettingList[index]);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.KeySetting_AssocSettingListItem, SettingTagType.EndObject);
                    if (!success) {
                        return false;
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
        public static bool SaveSetting(SettingSaver saver, AssociateSetting obj) {
            bool success;
            saver.StartObject(SettingTag.KeySetting_AssocSettingList);
            for (int i = 0; i < obj.m_assocSettingList.Count; i++) {
                saver.StartObject(SettingTag.KeySetting_AssocSettingListItem);
                saver.AddInt(SettingTag.KeySetting_AssocSettingListIndex, i);
                success = AssociateKeySetting.SaveSetting(saver, obj.m_assocSettingList[i]);
                if (!success) {
                    return false;
                }
                saver.EndObject(SettingTag.KeySetting_AssocSettingListItem);
            }
            saver.EndObject(SettingTag.KeySetting_AssocSettingList);
            return true;
        }

        //=========================================================================================
        // プロパティ：関連付け定義の一覧（常にASSOCIATE_SETTING_COUNT個の要素、設定がない要素はnull）
        //=========================================================================================
        public List<AssociateKeySetting> AssocSettingList {
            get {
                return m_assocSettingList;
            }
        }
    }
}
