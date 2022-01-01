using System;
using System.Collections.Generic;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // クラス：オプションダイアログの構成
    //=========================================================================================
    public class OptionStructure {
        // 項目一覧
        private List<OptionStructureItem> m_pageList = new List<OptionStructureItem>();

        // はじめに登録された項目
        private OptionStructureItem m_firstItem = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public OptionStructure() {
        }

        //=========================================================================================
        // 機　能：ページの構成情報を追加する
        // 引　数：[in]depth        階層の深さ（0:ルート）
        // 　　　　[in]id           項目のID
        // 　　　　[in]displayName  項目名
        // 　　　　[in]uiImplType   ページのUIを実装するクラス
        // 戻り値：なし
        //=========================================================================================
        public void AddOptionPage(int depth, string id, string displayName, Type uiImplType) {
            OptionStructureItem item = new OptionStructureItem(depth, id, displayName, uiImplType);
            m_pageList.Add(item);
            if (m_firstItem == null) {
                m_firstItem = item;
            }
        }

        //=========================================================================================
        // 機　能：指定されたIDに対応する項目を返す
        // 引　数：[in]id  項目のID
        // 戻り値：IDに対応する項目
        //=========================================================================================
        public OptionStructureItem GetPageItemFromId(string id) {
            foreach (OptionStructureItem item in m_pageList) {
                if (item.Id == id) {
                    return item;
                }
            }
            return m_firstItem;
        }

        //=========================================================================================
        // 機　能：指定された実装クラスのTypeに対応する項目を返す
        // 引　数：[in]type  項目のType
        // 戻り値：IDに対応する項目
        //=========================================================================================
        public OptionStructureItem GetPageItemFromType(Type type) {
            foreach (OptionStructureItem item in m_pageList) {
                if (item.UIImplType == type) {
                    return item;
                }
            }
            return m_firstItem;
        }

        //=========================================================================================
        // プロパティ：項目一覧
        //=========================================================================================
        public List<OptionStructureItem> PageList {
            get {
                return m_pageList;
            }
        }
    }
}
