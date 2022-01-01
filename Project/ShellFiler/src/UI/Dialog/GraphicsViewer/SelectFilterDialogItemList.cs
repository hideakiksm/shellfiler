using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.GraphicsViewer.Filter;

namespace ShellFiler.UI.Dialog.GraphicsViewer {
    
    //=========================================================================================
    // クラス：フィルター一覧UI入力に特化したフィルター一覧情報
    // 　　　　コンストラクタの設定を深いコピーでthisに取り込む。
    //=========================================================================================
    public class SelectFilterDialogItemList {
        // 選択されているフィルターの一覧
        private List<SelectFilterDialogItem> m_filterListOn = new List<SelectFilterDialogItem>();
        
        // 選択されていないフィルターの一覧
        private List<SelectFilterDialogItem> m_filterListOff = new List<SelectFilterDialogItem>();
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting  現在の設定
        // 戻り値：なし
        //=========================================================================================
        public SelectFilterDialogItemList(GraphicsViewerFilterSetting setting) {
            List<Type> filterTypeList = new List<Type>();
            filterTypeList.AddRange(ImageFilter.FilterTypeList);

            // 設定にあるフィルターを作成
            for (int i = 0; i < setting.FilterList.Count; i++) {
                GraphicsViewerFilterItem settingItem = setting.FilterList[i];

                filterTypeList.Remove(settingItem.FilterClass);
                IFilterComponent filter = (IFilterComponent)(settingItem.FilterClass.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                FilterMetaInfo metaInfo = filter.MetaInfo;
                object[] filterParam = new object[metaInfo.ParameterList.Length];
                for (int j = 0; j < filterParam.Length; j++) {
                    filterParam[j] = settingItem.FilterParameter[j];
                }

                SelectFilterDialogItem uiItem = new SelectFilterDialogItem(filter, true, filterParam);
                m_filterListOn.Add(uiItem);
            }

            // 設定にないフィルターを作成
            for (int i = 0; i < filterTypeList.Count; i++) {
                IFilterComponent filter = (IFilterComponent)(filterTypeList[i].InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                FilterMetaInfo metaInfo = filter.MetaInfo;
                object[] defaultParam = new object[metaInfo.ParameterList.Length];
                for (int j = 0; j < defaultParam.Length; j++) {
                    defaultParam[j] = metaInfo.ParameterList[j].DefaultValue;
                }

                SelectFilterDialogItem uiItem = new SelectFilterDialogItem(filter, false, defaultParam);
                m_filterListOff.Add(uiItem);
            }
        }

        //=========================================================================================
        // 機　能：指定された項目のONとOFFを切り替える
        // 引　数：[in]uiItem  フィルター情報
        // 戻り値：なし
        //=========================================================================================
        public void SwapFilterItemOnOff(SelectFilterDialogItem uiItem) {
            if (uiItem.UseFilter) {
                // OFF→ON
                m_filterListOff.Remove(uiItem);
                m_filterListOn.Add(uiItem);
            } else {
                // ON→OFF
                m_filterListOn.Remove(uiItem);
                m_filterListOff.Add(uiItem);
            }
        }

        //=========================================================================================
        // 機　能：指定されたフィルターの、ON状態一覧中でのインデックスを返す
        // 引　数：[in]uiItem  フィルター情報
        // 戻り値：一覧中でのインデックス（見つからなかったときは-1）
        //=========================================================================================
        public int GetFilterIndexOn(SelectFilterDialogItem uiItem) {
            for (int i = 0; i < m_filterListOn.Count; i++) {
                if (m_filterListOn[i] == uiItem) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：指定されたフィルターの、OFF状態一覧中でのインデックスを返す
        // 引　数：[in]uiItem  フィルター情報
        // 戻り値：一覧中でのインデックス（見つからなかったときは-1）
        //=========================================================================================
        public int GetFilterIndexOff(SelectFilterDialogItem uiItem) {
            for (int i = 0; i < m_filterListOff.Count; i++) {
                if (m_filterListOff[i] == uiItem) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // プロパティ：選択されているフィルターの一覧
        //=========================================================================================
        public List<SelectFilterDialogItem> FilterItemListOn {
            get {
                return m_filterListOn;
            }
        }

        //=========================================================================================
        // プロパティ：選択されていないフィルターの一覧
        //=========================================================================================
        public List<SelectFilterDialogItem> FilterItemListOff {
            get {
                return m_filterListOff;
            }
        }
    }
}
