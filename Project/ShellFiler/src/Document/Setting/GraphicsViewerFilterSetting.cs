using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileViewer;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：グラフィックビューアのフィルター設定
    //=========================================================================================
    public class GraphicsViewerFilterSetting : ICloneable {
        // フィルター設定
        private List<GraphicsViewerFilterItem> m_filterList = new List<GraphicsViewerFilterItem>();
        
        // フィルターを使用するときtrue
        private bool m_useFilter = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerFilterSetting() {
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(GraphicsViewerFilterSetting obj1, GraphicsViewerFilterSetting obj2) {
            if (obj1.m_useFilter != obj2.m_useFilter) {
                return false;
            }
            if (obj1.m_filterList.Count != obj2.m_filterList.Count) {
                return false;
            }
            for (int i = 0; i < obj1.m_filterList.Count; i++) {
                if (!GraphicsViewerFilterItem.EqualsConfig(obj1.m_filterList[i], obj2.m_filterList[i])) {
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
            GraphicsViewerFilterSetting clone = new GraphicsViewerFilterSetting();
            clone.m_useFilter = m_useFilter;
            for (int i = 0; i < m_filterList.Count; i++) {
                clone.m_filterList.Add((GraphicsViewerFilterItem)(m_filterList[i].Clone()));
            }

            return clone;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public void CopyFrom(GraphicsViewerFilterSetting src) {
            this.m_useFilter = src.m_useFilter;
            this.m_filterList.Clear();
            for (int i = 0; i < src.m_filterList.Count; i++) {
                this.m_filterList.Add((GraphicsViewerFilterItem)(src.m_filterList[i].Clone()));
            }
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out GraphicsViewerFilterSetting obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.GraphicsViewerFilter_GraphicsViewerFilter, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new GraphicsViewerFilterSetting();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.GraphicsViewerFilter_GraphicsViewerFilter) {
                    break;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.GraphicsViewerFilter_UseFilter) {
                    obj.m_useFilter = loader.BoolValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.GraphicsViewerFilter_FilterList) {
                    while (true) {
                        GraphicsViewerFilterItem item;
                        success = GraphicsViewerFilterItem.LoadSetting(loader, out item);
                        if (!success) {
                            return false;
                        }
                        if (item != null) {
                            obj.m_filterList.Add(item);
                        }
                        success = loader.GetTag(out tagName, out tagType);
                        if (!success) {
                            return false;
                        }
                        if (tagType == SettingTagType.EndObject && tagName == SettingTag.GraphicsViewerFilter_FilterList) {
                            break;
                        }
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
        public static bool SaveSetting(SettingSaver saver, GraphicsViewerFilterSetting obj) {
            if (obj == null) {
                return true;
            }

            bool success;
            saver.StartObject(SettingTag.GraphicsViewerFilter_GraphicsViewerFilter);
            saver.AddBool(SettingTag.GraphicsViewerFilter_UseFilter, obj.m_useFilter);
            saver.StartObject(SettingTag.GraphicsViewerFilter_FilterList);
            foreach (GraphicsViewerFilterItem item in obj.m_filterList) {
                success = GraphicsViewerFilterItem.SaveSetting(saver, item);
                if (!success) {
                    return false;
                }
            }
            saver.EndObject(SettingTag.GraphicsViewerFilter_FilterList);
            saver.EndObject(SettingTag.GraphicsViewerFilter_GraphicsViewerFilter);

            return true;
        }

        //=========================================================================================
        // 機　能：フィルター設定をリセットする
        // 引　数：[in]useFilter   フィルターを使用するときtrue
        // 戻り値：なし
        //=========================================================================================
        public void ResetFilter(bool useFilter) {
            this.m_useFilter = useFilter;
            this.m_filterList.Clear();
        }

        //=========================================================================================
        // プロパティ：フィルター設定
        //=========================================================================================
        public  List<GraphicsViewerFilterItem> FilterList {
            get {
                return m_filterList;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターを使用するときtrue
        //=========================================================================================
        public bool UseFilter {
            get {
                return m_useFilter;
            }
            set {
                m_useFilter = value;
            }
        }
    }
}
