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
    // クラス：クリップボード用のファイルフィルターの設定
    //=========================================================================================
    public class FileFilterListClipboard : ICloneable {
        // クイック設定の名前（クイック設定ではないときnull）
        private string m_quickSettingName = null;

        // フィルター項目のリスト
        private List<FileFilterItem> m_filterList = new List<FileFilterItem>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterListClipboard() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileFilterListClipboard obj = new FileFilterListClipboard();
            obj.m_quickSettingName = m_quickSettingName;
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
        public static bool EqualsConfig(FileFilterListClipboard obj1, FileFilterListClipboard obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_quickSettingName != obj2.m_quickSettingName) {
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
        public static bool LoadSetting(SettingLoader loader, out FileFilterListClipboard obj) {
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
            obj = new FileFilterListClipboard();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileFilter_FilterList) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_QuickName) {
                    obj.m_quickSettingName = loader.StringValue;
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
            if (hasError) {
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
        public static bool SaveSetting(SettingSaver saver, FileFilterListClipboard obj) {
            if (obj == null) {
                return true;
            }


            saver.StartObject(SettingTag.FileFilter_FilterList);
            saver.AddString(SettingTag.FileFilter_QuickName, obj.m_quickSettingName);
            saver.StartObject(SettingTag.FileFilter_FilterItemList);
            foreach (FileFilterItem item in obj.m_filterList) {
                FileFilterItem.SaveSetting(saver, item);
            }
            saver.EndObject(SettingTag.FileFilter_FilterItemList);
            saver.EndObject(SettingTag.FileFilter_FilterList);

            return true;
        }

        //=========================================================================================
        // プロパティ：クイック設定の名前（クイック設定ではないときnull）
        //=========================================================================================
        public string QuickSettingName {
            get {
                return m_quickSettingName;
            }
            set {
                m_quickSettingName = value;
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
