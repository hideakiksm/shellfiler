using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Text;
using ShellFiler.Document;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイル転送用のファイルフィルターの設定
    //=========================================================================================
    public class FileFilterListTransfer : ICloneable {
        // この設定を使用するときtrue
        private bool m_useFilter = true;

        // このフィルターの名前
        private string m_filterName = "";

        // 使用する拡張子のマスク
        private string m_targetFileMask = "";

        // フィルター項目のリスト
        private List<FileFilterItem> m_filterList = new List<FileFilterItem>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterListTransfer() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileFilterListTransfer obj = new FileFilterListTransfer();
            obj.m_useFilter = m_useFilter;
            obj.m_filterName = m_filterName;
            obj.m_targetFileMask = m_targetFileMask;
            for (int i = 0; i < m_filterList.Count; i++) {
                obj.m_filterList.Add((FileFilterItem)m_filterList[i]);
            }
            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileFilterListTransfer obj1, FileFilterListTransfer obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_useFilter != obj2.m_useFilter) {
                return false;
            }
            if (obj1.m_filterName != obj2.m_filterName) {
                return false;
            }
            if (obj1.m_targetFileMask != obj2.m_targetFileMask) {
                return false;
            }
            if (obj1.m_filterList.Count != obj2.m_filterList.Count) {
                return false;
            }
            for (int i = 0; i < obj1.m_filterList.Count; i++) {
                if (!FileFilterItem.EqualsConfig(obj1.m_filterList[i], obj2.m_filterList[i])) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト（フィルターのどれかにエラーがあった場合はnull）
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out FileFilterListTransfer obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.FileFilter_FilterList, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            bool hasError = false;
            obj = new FileFilterListTransfer();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileFilter_FilterList) {
                    break;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileFilter_UseFilter) {
                    obj.m_useFilter = loader.BoolValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_FilterName) {
                    obj.m_filterName = loader.StringValue;
                    if (obj.m_filterName == "") {
                        hasError = true;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_TargetFileMask) {
                    obj.m_targetFileMask = loader.StringValue;
                    if (obj.m_targetFileMask == "") {
                        hasError = true;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileFilter_FilterItemList) {
                    List<FileFilterItem> list = new List<FileFilterItem>();
                    success = LoadSettingFilterList(loader, list);
                    if (!success) {
                        return false;
                    }
                    for (int i = 0; i < list.Count; i++) {      // フィルターのどれかにエラーがあったときはhasError=trueでobj=null応答
                        if (list[i] == null) {
                            hasError = true;
                        }
                    }
                    obj.m_filterList = list;
                    success = loader.ExpectTag(SettingTag.FileFilter_FilterItemList, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }
            if (hasError || obj.m_targetFileMask == null || obj.m_targetFileMask == "") {
                obj = null;
            }

            return true;
        }
        
        //=========================================================================================
        // 機　能：フィルターのリスト項目を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]list    読み込んだ結果を返すリスト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSettingFilterList(SettingLoader loader, List<FileFilterItem> list) {
            while (true) {
                bool success;
                bool fit;
                success = loader.PeekTag(SettingTag.FileFilter_FilterItemList, SettingTagType.EndObject, out fit);
                if (!success) {
                    return success;
                }
                if (fit) {
                    return true;
                }

                FileFilterItem item;
                success = FileFilterItem.LoadSetting(loader, out item);
                if (!success) {
                    return false;
                }
                list.Add(item);
            }
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, FileFilterListTransfer obj) {
            if (obj == null) {
                return true;
            }

            saver.StartObject(SettingTag.FileFilter_FilterList);
            saver.AddBool(SettingTag.FileFilter_UseFilter, obj.m_useFilter);
            saver.AddString(SettingTag.FileFilter_FilterName, obj.m_filterName);
            saver.AddString(SettingTag.FileFilter_TargetFileMask, obj.m_targetFileMask);
            saver.StartObject(SettingTag.FileFilter_FilterItemList);
            foreach (FileFilterItem item in obj.m_filterList) {
                FileFilterItem.SaveSetting(saver, item);
            }
            saver.EndObject(SettingTag.FileFilter_FilterItemList);
            saver.EndObject(SettingTag.FileFilter_FilterList);

            return true;
        }

        //=========================================================================================
        // プロパティ：この設定を使用するときtrue
        //=========================================================================================
        public bool UseFilter {
            get {
                return m_useFilter;
            }
            set {
                m_useFilter = value;
            }
        }

        //=========================================================================================
        // プロパティ：このフィルターの名前
        //=========================================================================================
        public string FilterName {
            get {
                return m_filterName;
            }
            set {
                m_filterName = value;
            }
        }

        //=========================================================================================
        // プロパティ：使用する拡張子のマスク
        //=========================================================================================
        public string TargetFileMask {
            get {
                return m_targetFileMask;
            }
            set {
                m_targetFileMask = value;
            }
        }

        //=========================================================================================
        // プロパティ：フィルター項目のリスト
        //=========================================================================================
        public List<FileFilterItem> FilterList {
            get {
                return m_filterList;
            }
            set {
                m_filterList = value;
            }
        }
    }
}
