using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Util;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：開いているタブページの一覧
    //=========================================================================================
    public class TabPageList {
        // タブの最大数
        public const int MAX_TAB_PAGE_COUNT = 10;

        // 開いているタブページの一覧
        private List<TabPageInfo> m_tabPageList = new List<TabPageInfo>();

        // タスクに変化が生じたときの通知用delegate
        public delegate void TabPageChangedEventHandler(object sender, TabPageChangedEventArgs evt); 

        // タブページに変化が生じたときに通知するイベント
        public event TabPageChangedEventHandler TabPageChanged; 

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TabPageList() {
        }

        //=========================================================================================
        // 機　能：タブページを追加する
        // 引　数：[in]tabPageInfo  追加するタブページの情報
        // 戻り値：なし
        //=========================================================================================
        public void Add(TabPageInfo tabPageInfo) {
            m_tabPageList.Add(tabPageInfo);

            if (TabPageChanged != null) {
                TabPageChanged(this, new TabPageChangedEventArgs(true, tabPageInfo));
            }
        }

        //=========================================================================================
        // 機　能：タブページを削除する
        // 引　数：[in]tabPageInfo  削除するタブページの情報
        // 戻り値：なし
        //=========================================================================================
        public void Delete(TabPageInfo tabPageInfo) {
            m_tabPageList.Remove(tabPageInfo);

            if (TabPageChanged != null) {
                TabPageChanged(this, new TabPageChangedEventArgs(false, tabPageInfo));
            }
        }

        //=========================================================================================
        // 機　能：ロード中のファイル一覧があるかどうかを調べる
        // 引　数：なし
        // 戻り値：ロード中のファイル一覧があるときtrue
        //=========================================================================================
        public bool CheckLoadingFileList() {
            foreach (TabPageInfo tabPage in m_tabPageList) {
                UIFileList leftFileList = tabPage.LeftFileList;
                UIFileList rightFileList = tabPage.RightFileList;
                if (leftFileList.LoadingStatus == FileListLoadingStatus.Loading || rightFileList.LoadingStatus == FileListLoadingStatus.Loading) {
                    return true;
                }
            }
            return false;
        }

        //=========================================================================================
        // プロパティ：タブページの数
        //=========================================================================================
        public int Count {
            get {
                return m_tabPageList.Count;
            }
        }

        //=========================================================================================
        // プロパティ：開いているタブページの一覧
        //=========================================================================================
        public List<TabPageInfo> AllList {
            get {
                return m_tabPageList;
            }
        }

        //=========================================================================================
        // クラス：タブページ状態変化のイベント引数
        //=========================================================================================
        public class TabPageChangedEventArgs {
            // タブページが追加されたときtrue、削除されたときfalse
            private bool m_added;
            
            // 追加または削除されたページ
            private TabPageInfo m_tabPageInfo;
            
            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]added    タスクが追加されたときtrue
            // 　　　　[in]tabPage  追加または削除されたページ
            // 戻り値：なし
            //=========================================================================================
            public TabPageChangedEventArgs(bool added, TabPageInfo tabPage) {
                m_added = added;
                m_tabPageInfo = tabPage;
            }

            //=========================================================================================
            // プロパティ：タブページが追加されたときtrue、削除されたときfalse
            //=========================================================================================
            public bool Added {
                get {
                    return m_added;
                }
            }

            //=========================================================================================
            // プロパティ：追加または削除されたページ
            //=========================================================================================
            public TabPageInfo TabPageInfo {
                get {
                    return m_tabPageInfo;
                }
            }
        }
    }
}
