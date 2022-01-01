using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Edit;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：利用シーンごとのキーのカスタマイズ情報
    // 　　　　ファイル一覧、ファイルビューア、グラフィックビューアは別のインスタンスとして保持する。
    //=========================================================================================
    public class KeyItemSettingList : ICloneable {
        // キー定義のリスト
        private List<KeyItemSetting> m_listKeySetting = new List<KeyItemSetting>();

        // キーから設定へのmap
        private Dictionary<KeyState, KeyItemSetting> m_mapKeyToSetting = new Dictionary<KeyState, KeyItemSetting>();

        // キーからその設定されている定義の数へのmap（AとShift+Aが定義されているなら、A→2）
        private Dictionary<Keys, int> m_mapKeyToSettingCount = new Dictionary<Keys,int>();

        // キーにより実行するActionCommandのActionCommandMonikerから設定へのmap
        private Dictionary<ActionCommandMoniker, List<KeyItemSetting>> m_mapCommandToSettingList = new Dictionary<ActionCommandMoniker, List<KeyItemSetting>>();

        // キーにより実行するActionCommandのフルクラス名から設定へのmap
        private Dictionary<string, List<KeyItemSetting>> m_mapCommandClassToSettingList = new Dictionary<string, List<KeyItemSetting>>();

        // 2ストロークキーの表示名
        private Dictionary<TwoStrokeType, KeyState> m_twoStrokeDisplayName = new Dictionary<TwoStrokeType, KeyState>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public KeyItemSettingList() {
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(KeyItemSettingList obj1, KeyItemSettingList obj2) {
            // キー設定
            if (obj1.m_listKeySetting.Count != obj2.m_listKeySetting.Count) {
                return false;
            }
            foreach (KeyState keyState in obj1.m_mapKeyToSetting.Keys) {
                if (!obj2.m_mapKeyToSetting.ContainsKey(keyState)) {
                    return false;
                }
                KeyItemSetting key1 = obj1.m_mapKeyToSetting[keyState];
                KeyItemSetting key2 = obj2.m_mapKeyToSetting[keyState];
                if (!KeyItemSetting.EqualsConfig(key1, key2)) {
                    return false;
                }
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader     読み込み用クラス
        // 　　　　[in]obj        読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, KeyItemSettingList obj) {
            bool success;
            success = loader.ExpectTag(SettingTag.KeySetting_KeyItemSettingList, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            obj.ClearAll();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.KeySetting_KeyItemSettingList) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_KeyItemSetting) {
                    KeyItemSetting keyItem;
                    success = KeyItemSetting.LoadSetting(loader, out keyItem, obj);
                    if (success && keyItem != null) {
                        obj.AddSetting(keyItem);
                    }
                    // Endタグは内部で読み込み済み
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
        public static bool SaveSetting(SettingSaver saver, KeyItemSettingList obj) {
            bool success;

            saver.StartObject(SettingTag.KeySetting_KeyItemSettingList);

            foreach (KeyItemSetting keyItem in obj.m_listKeySetting) {
                saver.StartObject(SettingTag.KeySetting_KeyItemSetting);
                success = KeyItemSetting.SaveSetting(saver, keyItem);
                if (!success) {
                    return false;
                }
                saver.EndObject(SettingTag.KeySetting_KeyItemSetting);
            }

            saver.EndObject(SettingTag.KeySetting_KeyItemSettingList);

            return true;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            KeyItemSettingList clone = new KeyItemSettingList();
            foreach (KeyState keyState in m_mapKeyToSetting.Keys) {
                KeyItemSetting setting = (KeyItemSetting)(m_mapKeyToSetting[keyState].Clone());
                clone.AddSetting(setting);
            }

            return clone;
        }

        //=========================================================================================
        // 機　能：すべての設定を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearAll() {
            m_listKeySetting.Clear();
            m_mapKeyToSetting.Clear();
            m_mapKeyToSettingCount.Clear();
            m_mapCommandToSettingList.Clear();
            m_twoStrokeDisplayName.Clear();
        }

        //=========================================================================================
        // 機　能：設定を追加する
        // 引　数：[in]setting  追加する設定
        // 戻り値：設定を追加できたときtrue、同じキー設定があるときfalse
        //=========================================================================================
        public bool AddSetting(KeyItemSetting setting) {
            KeyState keyState = setting.KeyState;
            ActionCommandMoniker moniker = setting.ActionCommandMoniker;

            // 同じキーがある場合は登録回避
            if (m_mapKeyToSetting.ContainsKey(keyState)) {
                return false;
            }

            // 2ストロークキーの表示名を登録
            TwoStrokeType twoStroke = KeyState.GetTwoStrokeKeyType(setting.ActionCommandMoniker);
            if (twoStroke != TwoStrokeType.None) {
                if (!m_twoStrokeDisplayName.ContainsKey(twoStroke)) {
                    m_twoStrokeDisplayName.Add(twoStroke, setting.KeyState);
                }
            }

            // 登録
            m_listKeySetting.Add(setting);

            m_mapKeyToSetting.Add(keyState, setting);
            if (m_mapCommandToSettingList.ContainsKey(moniker)) {
                List<KeyItemSetting> settingList = m_mapCommandToSettingList[moniker];
                settingList.Add(setting);
            } else {
                List<KeyItemSetting> settingList = new List<KeyItemSetting>();
                settingList.Add(setting);
                m_mapCommandToSettingList.Add(moniker, settingList);
            }
            if (m_mapCommandClassToSettingList.ContainsKey(moniker.CommandType.FullName)) {
                List<KeyItemSetting> settingList = m_mapCommandClassToSettingList[moniker.CommandType.FullName];
                settingList.Add(setting);
            } else {
                List<KeyItemSetting> settingList = new List<KeyItemSetting>();
                settingList.Add(setting);
                m_mapCommandClassToSettingList.Add(moniker.CommandType.FullName, settingList);
            }

            if (m_mapKeyToSettingCount.ContainsKey(keyState.Key)) {
                int count = m_mapKeyToSettingCount[keyState.Key];
                count++;
                m_mapKeyToSettingCount[keyState.Key] = count;
            } else {
                m_mapKeyToSettingCount.Add(keyState.Key, 1);
            }
            
            return true;
        }

        //=========================================================================================
        // 機　能：設定を削除する
        // 引　数：[in]keyState  削除するキー
        // 戻り値：なし
        //=========================================================================================
        public void DeleteSetting(KeyState keyState) {
            KeyItemSetting item = m_mapKeyToSetting[keyState];
            
            // 2ストロークキーの表示名の登録を削除
            TwoStrokeType twoStroke = KeyState.GetTwoStrokeKeyType(item.ActionCommandMoniker);
            if (twoStroke != TwoStrokeType.None) {
                if (m_twoStrokeDisplayName.ContainsKey(twoStroke)) {
                    m_twoStrokeDisplayName.Remove(twoStroke);
                }
            }

            // インデックスを削除
            m_mapKeyToSetting.Remove(keyState);
            int count = m_mapKeyToSettingCount[keyState.Key];
            if (count == 1) {
                m_mapKeyToSettingCount.Remove(keyState.Key);
            } else {
                m_mapKeyToSettingCount[keyState.Key]--;
            }

            List<KeyItemSetting> listKeyCommand = m_mapCommandToSettingList[item.ActionCommandMoniker];
            if (listKeyCommand.Count == 1) {
                m_mapCommandToSettingList.Remove(item.ActionCommandMoniker);
            } else {
                listKeyCommand.Remove(item);
            }

            List<KeyItemSetting> listKeyClass = m_mapCommandClassToSettingList[item.ActionCommandMoniker.CommandType.FullName];
            if (listKeyClass.Count == 1) {
                m_mapCommandClassToSettingList.Remove(item.ActionCommandMoniker.CommandType.FullName);
            } else {
                listKeyClass.Remove(item);
            }

            m_listKeySetting.Remove(item);
        }

        //=========================================================================================
        // 機　能：指定された2ストロークキーに対して、実際に割り当てられているキーを返す
        // 引　数：[in]twoStroke  2ストロークキーの種類
        // 戻り値：キー
        //=========================================================================================
        public KeyState GetTwoStrokeKey(TwoStrokeType twoStroke) {
            KeyState name = null;
            if (m_twoStrokeDisplayName.ContainsKey(twoStroke)) {
                name = m_twoStrokeDisplayName[twoStroke];
            }
            return name;
        }

        //=========================================================================================
        // 機　能：キーから設定を取得する
        // 引　数：[in]key  調べるキー
        // 戻り値：キーに対応する設定（設定がないときnull）
        //=========================================================================================
        public KeyItemSetting GetSettingFromKey(KeyState key) {
            if (m_mapKeyToSetting.ContainsKey(key)) {
                return m_mapKeyToSetting[key];
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：キーコードから設定されている定義の数を取得する
        // 引　数：[in]key  調べるキーのコード
        // 戻り値：設定されている定義の数
        //=========================================================================================
        public int GetSettingCountFromKeyCode(Keys key) {
            if (m_mapKeyToSettingCount.ContainsKey(key)) {
                return m_mapKeyToSettingCount[key];
            } else {
                return 0;
            }
        }

        //=========================================================================================
        // 機　能：コマンドのクラスのモニカから設定を取得する
        // 引　数：[in]moniker  キーにより実行するActionCommandのActionCommandMoniker
        // 戻り値：monikerに対応する設定のリスト（設定がないときnull）
        //=========================================================================================
        public List<KeyItemSetting> GetSettingFromCommand(ActionCommandMoniker moniker) {
            if (m_mapCommandToSettingList.ContainsKey(moniker)) {
                return m_mapCommandToSettingList[moniker];
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：コマンドのクラス名から設定を取得する
        // 引　数：[in]className  クラス名（FullName）
        // 戻り値：monikerに対応する設定のリスト（設定がないときnull）
        //=========================================================================================
        public List<KeyItemSetting> GetSettingFromCommandClass(string className) {
            if (m_mapCommandClassToSettingList.ContainsKey(className)) {
                return m_mapCommandClassToSettingList[className];
            } else {
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：キー定義のリスト
        //=========================================================================================
        public List<KeyItemSetting> AllKeySettingList {
            get {
                return m_listKeySetting;
            }
        }
    }
}
