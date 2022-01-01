using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using ShellFiler.Document;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイルフィルター１段分の設定
    //=========================================================================================
    public class FileFilterItem : ICloneable {
        // 実行するフルクラス名
        private string m_fileFilterClassPath = null;
        
        // 設定情報（初期状態のときnull）
        private Dictionary<string, object> m_propertyList = new Dictionary<string, object>();
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterItem() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileFilterItem obj = new FileFilterItem();
            obj.m_fileFilterClassPath = m_fileFilterClassPath;
            foreach (string key in m_propertyList.Keys) {
                obj.m_propertyList.Add(key, m_propertyList[key]);
            }
            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileFilterItem obj1, FileFilterItem obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_fileFilterClassPath != obj2.m_fileFilterClassPath) {
                return false;
            }
            if (obj1.m_propertyList.Count != obj2.m_propertyList.Count) {
                return false;
            }
            foreach (string key in obj1.m_propertyList.Keys) {
                if (!obj2.m_propertyList.ContainsKey(key)) {
                    return false;
                }
                object obj1Data = obj1.m_propertyList[key];
                object obj2Data = obj2.m_propertyList[key];
                if (obj1Data.GetType() != obj2Data.GetType()) {
                    return false;
                }
                if (obj1Data != obj2Data) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト（エラーがあったときはnull）
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out FileFilterItem obj) {
            obj = null;
            bool success;
            success = loader.ExpectTag(SettingTag.FileFilter_FilterItem, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            string classPath = null;
            Dictionary<string, object> param = null;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileFilter_FilterItem) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_ClassPath) {
                    classPath = loader.StringValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileFilter_ParamList) {
                    success = LoadParamList(loader, out param);
                    if (!success) {
                        return false;
                    }
                }
            }
            if (classPath == null || param == null) {
                return true;
            }

            // コンポーネントを作成して検証
            IFileFilterComponent component;
            try {
                Type componentType = Type.GetType(classPath);
                component = (IFileFilterComponent)(componentType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
            } catch (Exception) {
                return true;            // obj=nullのまま返る
            }
            string errorMessage = component.CheckParameter(param);
            if (errorMessage != null) {
                return false;
            }

            // OK
            obj = new FileFilterItem();
            obj.m_fileFilterClassPath = classPath;
            obj.m_propertyList = param;

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルからパラメータを読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]param   読み込み対象のオブジェクト（エラーがあったときはnull）
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadParamList(SettingLoader loader, out Dictionary<string, object> param) {
            param = null;
            List<string> paramName = new List<string>();
            List<object> paramValue = new List<object>();
            bool success;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileFilter_ParamList) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_ParamName) {
                    paramName.Add(loader.StringValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_ParamValue) {
                    paramValue.Add(loader.StringValue);
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.FileFilter_ParamValue) {
                    paramValue.Add(loader.IntValue);
                } else if (tagType == SettingTagType.FloatValue && tagName == SettingTag.FileFilter_ParamValue) {
                    paramValue.Add(loader.FloatValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileFilter_ParamValue) {
                    paramValue.Add(loader.BoolValue);
                }
            }
            if (paramName.Count != paramValue.Count) {
                return true;
            }
            param = new Dictionary<string, object>();
            for (int i = 0; i < paramName.Count; i++) {
                if (param.ContainsKey(paramName[i])) {
                    param = null;
                    return true;
                }
                param[paramName[i]] = paramValue[i];
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, FileFilterItem obj) {
            saver.StartObject(SettingTag.FileFilter_FilterItem);
            if (obj != null) {
                saver.AddString(SettingTag.FileFilter_ClassPath, obj.m_fileFilterClassPath);
                saver.StartObject(SettingTag.FileFilter_ParamList);
                foreach (string key in obj.m_propertyList.Keys) {
                    saver.AddString(SettingTag.FileFilter_ParamName, key);
                    object value = obj.m_propertyList[key];
                    if (value is string) {
                        saver.AddString(SettingTag.FileFilter_ParamValue, (string)value);
                    } else if (value is int) {
                        saver.AddInt(SettingTag.FileFilter_ParamValue, (int)value);
                    } else if (value is float) {
                        saver.AddFloat(SettingTag.FileFilter_ParamValue, (float)value);
                    } else if (value is bool) {
                        saver.AddBool(SettingTag.FileFilter_ParamValue, (bool)value);
                    }
                }
                saver.EndObject(SettingTag.FileFilter_ParamList);
            }
            saver.EndObject(SettingTag.FileFilter_FilterItem);

            return true;
        }

        //=========================================================================================
        // プロパティ：実行するフルクラス名
        //=========================================================================================
        public string FileFilterClassPath {
            get {
                return m_fileFilterClassPath;
            }
            set {
                m_fileFilterClassPath = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：設定情報（初期状態のときnull）
        //=========================================================================================
        public  Dictionary<string, object> PropertyList {
            get {
                return m_propertyList;
            }
            set {
                m_propertyList = value;
            }
        }
    }
}
