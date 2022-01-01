using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.GraphicsViewer.Filter;

namespace ShellFiler.UI.Dialog.GraphicsViewer {
    
    //=========================================================================================
    // クラス：フィルターUI用のフィルター項目
    //=========================================================================================
    public class SelectFilterDialogItem {
        // フィルター
        private IFilterComponent m_filter;

        // フィルターのメタ情報
        private FilterMetaInfo m_filterMetaInfo;

        // このフィルターを使用するときtrue
        private bool m_useFilter;
        
        // 入力中のパラメータ（使用しないときは過去の入力値またはデフォルト）
        private object[] m_filterParameter;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]filter          フィルター
        // 　　　　[in]useFilter       そのフィルターを使用するときtrue
        // 　　　　[in]filterParameter 入力中のパラメータ
        // 戻り値：なし
        //=========================================================================================
        public SelectFilterDialogItem(IFilterComponent filter, bool useFilter, object[] filterParameter) {
            m_filter = filter;
            m_filterMetaInfo = filter.MetaInfo;
            m_useFilter = useFilter;
            m_filterParameter = filterParameter;
        }

        //=========================================================================================
        // プロパティ：フィルター
        //=========================================================================================
        public IFilterComponent Filter {
            get {
                return m_filter;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターのメタ情報
        //=========================================================================================
        public FilterMetaInfo FilterMetaInfo {
            get {
                return m_filterMetaInfo;
            }
        }

        //=========================================================================================
        // プロパティ：このフィルターを使用するときtrue
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
        // プロパティ：入力中のパラメータ（使用しないときは過去の入力値またはデフォルト）
        //=========================================================================================
        public object[] FilterParameter {
            get {
                return m_filterParameter;
            }
        }
    }
}
