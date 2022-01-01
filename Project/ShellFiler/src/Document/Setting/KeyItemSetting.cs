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
    // クラス：キーのカスタマイズ情報のキー１つ分の情報
    //=========================================================================================
    public class KeyItemSetting : ICloneable {
        // コマンド実行に使用するキー
        private KeyState m_keyState;
        
        // 実行されるコマンドのクラス名
        private ActionCommandMoniker m_actionCommandMoniker;
        
        // 表示名（null:デフォルト）
        private string m_displayName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]keyState  コマンド実行に使用するキー
        // 　　　　[in]moniker   実行されるコマンドのモニカ
        // 　　　　[in]dispName  表示名（null:デフォルト）
        // 戻り値：なし
        //=========================================================================================
        public KeyItemSetting(KeyState keyState, ActionCommandMoniker moniker, string dispName) {
            m_keyState = keyState;
            m_actionCommandMoniker = moniker;
            m_displayName = dispName;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（シリアライズ/Clone専用）
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public KeyItemSetting() {
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader     読み込み用クラス
        // 　　　　[in]obj        読み込み対象のオブジェクト
        // 　　　　[in]keyList    読み込み済みのキー一覧
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out KeyItemSetting obj, KeyItemSettingList keyList) {
            bool success;
            obj = null;
            int keyCode = -1;
            TwoStrokeType twoStrokeType = TwoStrokeType.None;
            bool keyShift = false;
            bool keyCtrl = false;
            bool keyAlt = false;
            string commandDispName = null;
            ActionCommandMoniker moniker = null;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.KeySetting_KeyItemSetting) {
                    break;

                // キー
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.KeySetting_KeyCode) {
                    keyCode = loader.IntValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.KeySetting_KeyIsShift) {
                    keyShift = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.KeySetting_KeyIsControl) {
                    keyCtrl = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.KeySetting_KeyIsAlt) {
                    keyAlt = loader.BoolValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.KeySetting_KeyTwoStrokeType) {
                    string keyType = loader.StringValue;
                    if (keyType == "Key1") {
                        twoStrokeType = TwoStrokeType.Key1;
                    } else if (keyType == "Key2") {
                        twoStrokeType = TwoStrokeType.Key2;
                    } else if (keyType == "Key2") {
                        twoStrokeType = TwoStrokeType.Key3;
                    } else if (keyType == "Key2") {
                        twoStrokeType = TwoStrokeType.Key4;
                    }

                // コマンド
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.KeySetting_CommandDisplayName) {
                    commandDispName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.KeySetting_CommandClassFullName) {
                    string className;
                    success = LoadActionCommandMoniker(tagType, tagName, SettingTag.KeySetting_KeyItemSetting, loader, out moniker, out className);
                    if (!success || moniker == null) {
                        // 読み込み失敗時はコマンド不正として回復する
                        string dispKey;
                        if (keyCode == -1) {
                            dispKey = "?";
                        } else {
                            dispKey = new KeyState((Keys)keyCode, keyShift, keyCtrl, keyAlt).GetDisplayName(keyList);
                        }
                        loader.SetWarning(Resources.SettingLoader_LoadKeySettingKeyCommandFailed, dispKey);
                        return true;
                    }
                    break;
                }
            }

            // キーを復元
            if (keyCode == -1) {
                return false;
            }
            obj = new KeyItemSetting();
            if (twoStrokeType == TwoStrokeType.None) {
                obj.m_keyState = new KeyState((Keys)keyCode, keyShift, keyCtrl, keyAlt);
            } else {
                obj.m_keyState = new KeyState((Keys)keyCode, twoStrokeType);
            }

            // モニカを復元
            if (moniker == null) {
                return false;
            }
            obj.m_actionCommandMoniker = moniker;
            obj.m_displayName = commandDispName;

            return true;
        }


        //=========================================================================================
        // 機　能：ファイルからモニカを読み込む
        // 引　数：[in]tagType       読み込み済みのタグの種類（まだ読み込んでいないときnull）
        // 　　　　[in]tagName       読み込み済みのタグの名前（まだ読み込んでいないときnull）
        // 　　　　[in]endTagName    終了タグとみなすタグの名前
        // 　　　　[in]loader        読み込み用クラス
        // 　　　　[out]obj          読み込み対象のオブジェクトを返す変数（失敗したときnull）
        // 　　　　[out]monikerClass コマンドのクラス名を返す変数（読み込めなかったときnull）
        // 戻り値：読み込みに成功したときtrue
        // メ　モ：読み込み成功時はobj!=null+true、読み込み失敗時はobj==null+false、空読み込み時はobj==null+true
        //=========================================================================================
        public static bool LoadActionCommandMoniker(SettingTagType tagType, SettingTag tagName, SettingTag endTagName, SettingLoader loader, out ActionCommandMoniker obj, out string monikerClass) {
            bool success;
            obj = null;
            monikerClass = null;

            // 空の定義
            if (tagType == SettingTagType.EndObject && tagName == endTagName) {
                return true;
            }

            string monikerOption = null;
            List<object> paramList = new List<object>();
            while (true) {
                if (tagType == SettingTagType.EndObject && tagName == endTagName) {
                    break;

                // コマンド
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.KeySetting_CommandClassFullName) {
                    monikerClass = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.KeySetting_CommandOption) {
                    monikerOption = loader.StringValue;

                // パラメータ
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_CommandParameterList) {
                    ;
                } else if (tagType == SettingTagType.EndObject && tagName == SettingTag.KeySetting_CommandParameterList) {
                    ;
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.KeySetting_CommandParameter) {
                    paramList.Add(loader.IntValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.KeySetting_CommandParameter) {
                    paramList.Add(loader.StringValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.KeySetting_CommandParameter) {
                    paramList.Add(loader.BoolValue);
                }

                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
            }

            // モニカを復元
            if (monikerClass == null || monikerOption == null) {
                return false;
            }
            monikerClass = MonikerVersionConverter.ConvertMonikerClassName(monikerClass);
            Type type = Type.GetType(monikerClass);
            if (type == null) {
                return false;
            }
            ActionCommand command;
            try {
                command = (ActionCommand)(type.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
            } catch (Exception) {
                return false;
            }
            if (command.UIResource.FirstVersion > loader.FileVersion) {
                return false;
            }
            ActionCommandOption option = ActionCommandOption.None;
            if (monikerOption == "MoveNext") {
                option |= ActionCommandOption.MoveNext;
            }
            obj = new ActionCommandMoniker(option, type, paramList.ToArray());

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, KeyItemSetting obj) {
            bool success;

            // キー
            saver.AddInt(SettingTag.KeySetting_KeyCode, (int)(obj.m_keyState.Key));
            switch (obj.m_keyState.TwoStrokeKey) {
                case TwoStrokeType.None:
                    saver.AddBool(SettingTag.KeySetting_KeyIsShift, obj.m_keyState.IsShift);
                    saver.AddBool(SettingTag.KeySetting_KeyIsControl, obj.m_keyState.IsControl);
                    saver.AddBool(SettingTag.KeySetting_KeyIsAlt, obj.m_keyState.IsAlt);
                    break;
                case TwoStrokeType.Key1:
                    saver.AddString(SettingTag.KeySetting_KeyTwoStrokeType, "Key1");
                    break;
                case TwoStrokeType.Key2:
                    saver.AddString(SettingTag.KeySetting_KeyTwoStrokeType, "Key2");
                    break;
                case TwoStrokeType.Key3:
                    saver.AddString(SettingTag.KeySetting_KeyTwoStrokeType, "Key3");
                    break;
                case TwoStrokeType.Key4:
                    saver.AddString(SettingTag.KeySetting_KeyTwoStrokeType, "Key4");
                    break;
            }

            // モニカ
            saver.AddString(SettingTag.KeySetting_CommandDisplayName, obj.DisplayName);
            success = SaveActionCommandMoniker(saver, obj.m_actionCommandMoniker);
            if (!success) {
                return false;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：モニカをファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveActionCommandMoniker(SettingSaver saver, ActionCommandMoniker obj) {
            saver.AddString(SettingTag.KeySetting_CommandClassFullName, obj.CommandType.FullName);
            string option = "";
            if (obj.Option == ActionCommandOption.None) {
                option = "";
            } else if (obj.Option == ActionCommandOption.MoveNext) {
                option = "MoveNext";
            } else {
                Program.Abort("{0}に不明な属性{1}があります。", obj.CommandType.FullName, obj.Option);
            }
            saver.AddString(SettingTag.KeySetting_CommandOption, option);

            // パラメータ
            saver.StartObject(SettingTag.KeySetting_CommandParameterList);
            foreach (object param in obj.Parameter) {
                if (param is int) {
                    saver.AddInt(SettingTag.KeySetting_CommandParameter, (int)param);
                } else if (param is string) {
                    saver.AddString(SettingTag.KeySetting_CommandParameter, (string)param);
                } else if (param is bool) {
                    saver.AddBool(SettingTag.KeySetting_CommandParameter, (bool)param);
                } else {
                    Program.Abort("{0}に不明な型{1}があります。", obj.CommandType.FullName, param.GetType().FullName);
                }
            }
            saver.EndObject(SettingTag.KeySetting_CommandParameterList);

            return true;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(KeyItemSetting obj1, KeyItemSetting obj2) {
            if (!obj1.m_keyState.Equals(obj2.m_keyState)) {
                return false;
            }
            if (!obj1.m_actionCommandMoniker.Equals(obj2.m_actionCommandMoniker)) {
                return false;
            }
            if (obj1.m_displayName != obj2.m_displayName) {
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
            KeyItemSetting clone = new KeyItemSetting();
            clone.m_keyState = (KeyState)(m_keyState.Clone());
            clone.m_actionCommandMoniker = (ActionCommandMoniker)(m_actionCommandMoniker.Clone());
            clone.m_displayName = m_displayName;

            return clone;
        }

        //=========================================================================================
        // プロパティ：コマンド実行に使用するキー
        //=========================================================================================
        public KeyState KeyState {
            get {
                return m_keyState;
            }
        }

        //=========================================================================================
        // プロパティ：実行されるコマンドのクラス名
        //=========================================================================================
        public ActionCommandMoniker ActionCommandMoniker {
            get {
                return m_actionCommandMoniker;
            }
        }

        //=========================================================================================
        // プロパティ：表示名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
        }
    }
}
