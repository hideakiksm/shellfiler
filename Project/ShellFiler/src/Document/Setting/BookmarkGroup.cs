using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ブックマークのグループ情報
    //=========================================================================================
    public class BookmarkGroup : ICloneable {
        // ブックマーク内のグループ数の最大値
        public const int MAX_GROUP_COUNT = 10;

        // グループ内のフォルダ数の最大値
        public const int MAX_FOLDER_COUNT = 20;

        // グループ名
        private string m_groupName;

        // 登録ディレクトリの情報
        private List<BookmarkItem> m_bookmarkItemList = new List<BookmarkItem>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public BookmarkGroup() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            BookmarkGroup group = new BookmarkGroup();
            for (int i = 0; i < m_bookmarkItemList.Count; i++) {
                BookmarkItem item = (BookmarkItem)(m_bookmarkItemList[i].Clone());
                group.m_bookmarkItemList.Add(item);
            }
            group.m_groupName = m_groupName;
            return group;
        }

        //=========================================================================================
        // 機　能：登録ディレクトリに項目を追加する
        // 引　数：[in]item  追加する項目
        // 戻り値：なし
        //=========================================================================================
        public void AddDirectory(BookmarkItem item) {
            m_bookmarkItemList.Add(item);
        }

        //=========================================================================================
        // 機　能：登録ディレクトリに項目を挿入する
        // 引　数：[in]index  挿入する位置
        // 　　　　[in]item   追加する項目
        // 戻り値：なし
        //=========================================================================================
        public void InsertDirectory(int index, BookmarkItem item) {
            m_bookmarkItemList.Insert(index, item);
        }

        //=========================================================================================
        // 機　能：登録ディレクトリの項目を削除する
        // 引　数：[in]item  削除する項目
        // 戻り値：なし
        //=========================================================================================
        public void DeleteItem(BookmarkItem item) {
            m_bookmarkItemList.Remove(item);
        }

        //=========================================================================================
        // 機　能：フォルダの定義位置を入れ替える
        // 引　数：[in]index1  定義位置1
        // 　　　　[in]index2  定義位置2
        // 戻り値：なし
        //=========================================================================================
        public void SwapItem(int index1, int index2) {
            BookmarkItem item = m_bookmarkItemList[index1];
            m_bookmarkItemList.RemoveAt(index1);
            m_bookmarkItemList.Insert(index2, item);
        }

        //=========================================================================================
        // プロパティ：グループ名
        //=========================================================================================
        public string GroupName {
            get {
                return m_groupName;
            }
            set {
                m_groupName = value;
            }
        }

        //=========================================================================================
        // プロパティ：登録されている項目のリスト
        //=========================================================================================
        public List<BookmarkItem> ItemList {
            get {
                return m_bookmarkItemList;
            }
        }
    }
}
