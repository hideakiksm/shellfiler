using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI.ControlBar;
using ShellFiler.Locale;
using ShellFiler.FileTask;

namespace ShellFiler.UI {

    //=========================================================================================
    // クラス：タブの制御クラス
    //=========================================================================================
    public class TabControlImpl {
        // 所有ウィンドウ
        private MainWindowForm m_parent;

        // ファイル一覧のスプリットウィンドウ
        private SplitContainer m_splitContainerFile;

        // タブのUI
        private TabControlEx m_tabControlMain;
        
        // タブページのUI一覧
        private List<TabPage> m_tabUIList = new List<TabPage>(); 

        // 最後に閉じたタブの情報（null:有効な情報がない）
        private TabPageInfo m_lastCloseTabInfo;

        // UI更新中のため、フォーカスの設定処理をスキップする必要があるときtrue
        private bool m_skipFocusGotEvent = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent          所有ウィンドウ
        // 　　　　[in]tabControlMain  タブのUI
        // 　　　　[in]splitFile       ファイル一覧のスプリットウィンドウ
        // 戻り値：なし
        //=========================================================================================
        public TabControlImpl(MainWindowForm parent, TabControlEx tabControlMain, SplitContainer splitFile) {
            m_parent = parent;
            m_tabControlMain = tabControlMain;
            m_splitContainerFile = splitFile;
            m_tabControlMain.SelectedIndexChanged += new EventHandler(TabControlMain_SelectedIndexChanged);
            m_tabControlMain.GotFocus += new EventHandler(TabControlMain_GotFocus);
            m_tabControlMain.TabCloseButtonClick += new EventHandler(TabControlMain_TabCloseButtonClick);
            m_tabControlMain.MouseClick += new MouseEventHandler(TabControlMain_MouseClick);
        }

        //=========================================================================================
        // 機　能：フォーカスを得たときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TabControlMain_GotFocus(object sender, EventArgs evt) {
            if (m_skipFocusGotEvent) {
                return;
            }
            TabPageInfo tabInfo = (TabPageInfo)m_tabControlMain.SelectedTab.Tag;
            SetFocusFileList(tabInfo);
        }

        //=========================================================================================
        // 機　能：フォーカスをファイル一覧に戻す
        // 引　数：[in]tabInfo  戻す先のタブ情報
        // 戻り値：なし
        //=========================================================================================
        private void SetFocusFileList(TabPageInfo tabInfo) {
            if (tabInfo.IsCursorLeft) {
                m_parent.ActiveControl = m_parent.LeftFileListView;
            } else {
                m_parent.ActiveControl = m_parent.RightFileListView;
            }
        }
        
        //=========================================================================================
        // 機　能：マウスがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void TabControlMain_MouseClick(object sender, MouseEventArgs evt) {
            if ((evt.Button & MouseButtons.Right) == MouseButtons.Right) {
                // 右クリック
                // コンテキストメニューを表示
                ContextMenuStrip cms = new ContextMenuStrip();
                MenuImpl menuImpl = new MenuImpl(UICommandSender.FileViewerMenu, m_parent);
                menuImpl.AddItemsFromSetting(cms, cms.Items, Program.Document.MenuSetting.TabMenu, Program.Document.KeySetting.FileListKeyItemList, false, null);
                menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
                Point ptMouse = m_parent.PointToClient(Cursor.Position);
                m_parent.ContextMenuStrip = cms;
                m_parent.ContextMenuStrip.Show(m_parent, ptMouse);
                m_parent.ContextMenuStrip = null;
            } else if ((evt.Button & MouseButtons.Middle) == MouseButtons.Middle) {
                // 中クリック
                // 閉じる
                if (Program.Document.TabPageList.Count == 1) {
                    return;
                }
                Point mousePos = new Point(evt.X, evt.Y);
                for (int i = 0; i < m_tabControlMain.TabPages.Count; i++) {
                    Rectangle rect = m_tabControlMain.GetTabRect(i);
                    if (rect.Contains(mousePos)) {
                        TabPageInfo tabInfo = (TabPageInfo)(m_tabControlMain.TabPages[i].Tag);
                        DeleteTab(tabInfo);
                        break;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：タブが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void TabControlMain_SelectedIndexChanged(object sender, EventArgs evt) {
            // UIを差し替える
            TabPage newTabPage = m_tabControlMain.SelectedTab;
            if (newTabPage == GetTabPageFromTabPageInfo(Program.Document.CurrentTabPage)) {
                return;
            }
            m_skipFocusGotEvent = true;
            try {
                m_tabControlMain.SuspendLayout();
                m_splitContainerFile.SuspendLayout();
                newTabPage.Controls.Add(m_splitContainerFile);
                
                if (Program.Document.CurrentTabPage != null) {
                    TabPageInfo oldInfo = Program.Document.CurrentTabPage;
                    TabPage oldTabPage = GetTabPageFromTabPageInfo(oldInfo);
                    if (oldTabPage != null) {           // タブが削除されたときはnull
                        oldTabPage.Controls.Clear();
                        m_parent.LeftFileListView.SaveTabContents(oldInfo);
                        m_parent.RightFileListView.SaveTabContents(oldInfo);
                    }
                }

                m_tabControlMain.ResumeLayout();
                m_splitContainerFile.ResumeLayout();
            } finally {
                m_skipFocusGotEvent = false;
            }

            // 切り替え後の処理
            TabPageInfo newInfo = (TabPageInfo)newTabPage.Tag;
            Program.Document.CurrentTabPage = newInfo;
            m_parent.LeftFileListView.LoadTabContents(newInfo);
            m_parent.RightFileListView.LoadTabContents(newInfo);
            m_parent.RefreshUIStatus();

            // 一覧を自動リフレッシュ
            if (FileSystemID.IsWindows(m_parent.LeftFileListView.FileList.FileSystem.FileSystemId) || FileSystemID.IsVirtual(m_parent.LeftFileListView.FileList.FileSystem.FileSystemId)) {
                if (Configuration.Current.RefreshFileListTabChange) {
                    RefreshUITarget.ReloadDirectory(m_parent.LeftFileListView, false, null);
                }
            } else if (FileSystemID.IsSSH(m_parent.LeftFileListView.FileList.FileSystem.FileSystemId)) {
                if (Configuration.Current.RefreshFileListTabChangeSSH) {
                    RefreshUITarget.ReloadDirectory(m_parent.LeftFileListView, false, null);
                }
            } else {
                FileSystemID.NotSupportError(m_parent.LeftFileListView.FileList.FileSystem.FileSystemId);
            }
            if (FileSystemID.IsWindows(m_parent.RightFileListView.FileList.FileSystem.FileSystemId) || FileSystemID.IsVirtual(m_parent.RightFileListView.FileList.FileSystem.FileSystemId)) {
                if (Configuration.Current.RefreshFileListTabChange) {
                    RefreshUITarget.ReloadDirectory(m_parent.RightFileListView, false, null);
                }
            } else if (FileSystemID.IsSSH(m_parent.RightFileListView.FileList.FileSystem.FileSystemId)) {
                if (Configuration.Current.RefreshFileListTabChangeSSH) {
                    RefreshUITarget.ReloadDirectory(m_parent.RightFileListView, false, null);
                }
            } else {
                FileSystemID.NotSupportError(m_parent.LeftFileListView.FileList.FileSystem.FileSystemId);
            }

            // フォーカスを一覧に戻す
            SetFocusFileList(newInfo);
        }

        //=========================================================================================
        // 機　能：タブページの情報からそれを扱うUIを返す
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：タブUI(該当するものがない場合はnull)
        //=========================================================================================
        void TabControlMain_TabCloseButtonClick(object sender, EventArgs evt) {
            if (Program.Document.TabPageList.Count == 1) {
                return;
            }
            TabPageInfo tabInfo = (TabPageInfo)(((TabPage)sender).Tag);
            DeleteTab(tabInfo);
        }

        //=========================================================================================
        // 機　能：タブページの情報からそれを扱うUIを返す
        // 引　数：[in]tabInfo  タブページの情報
        // 戻り値：タブUI(該当するものがない場合はnull)
        //=========================================================================================
        private TabPage GetTabPageFromTabPageInfo(TabPageInfo tabInfo) {
            foreach (TabPage tabPage in m_tabControlMain.TabPages) {
                if (tabPage.Tag == tabInfo) {
                    return tabPage;
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：現在表示中のタブを複製する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DuplicateCurrentTab() {
            TabPageInfo currentTab = Program.Document.CurrentTabPage;
            TabPageInfo newTab = (TabPageInfo)currentTab.Clone();
            AddTabPageUI(newTab);
            // tabControlMain_SelectedIndexChangedで切り替え処理を実行
        }

        //=========================================================================================
        // 機　能：最後に閉じたタブを開く
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ReopenTab() {
            AddTabPageUI(m_lastCloseTabInfo);
            m_lastCloseTabInfo = null;
        }

        //=========================================================================================
        // 機　能：指定されたタブを閉じる
        // 引　数：[in]delTargetInfo  閉じるタブの情報
        // 戻り値：なし
        //=========================================================================================
        public void DeleteTab(TabPageInfo delTargetInfo) {
            // 削除対象がカレントなら未保存の情報を保存
            if (delTargetInfo == Program.Document.CurrentTabPage) {
                m_parent.LeftFileListView.SaveTabContents(delTargetInfo);
                m_parent.RightFileListView.SaveTabContents(delTargetInfo);
            }
            DisposeTabPage(m_lastCloseTabInfo);
            m_lastCloseTabInfo = delTargetInfo;

            // はじめにUIから削除し、tabControlMain_SelectedIndexChangedで切り替え処理を実行
            m_skipFocusGotEvent = true;
            try {
                foreach (TabPage tabPage in m_tabControlMain.TabPages) {
                    if (tabPage.Tag == delTargetInfo) {
                        m_tabControlMain.TabPages.Remove(tabPage);
                        break;
                    }
                }
            } finally {
                m_skipFocusGotEvent = false;
            }

            // 管理から削除
            Program.Document.FileCrawlThread.OnDisposeUIFileList(delTargetInfo.LeftFileList);
            Program.Document.FileCrawlThread.OnDisposeUIFileList(delTargetInfo.RightFileList);
            Program.Document.TabPageList.Delete(delTargetInfo);
        }

        //=========================================================================================
        // 機　能：タブページを破棄する
        // 引　数：[in]delTargetInfo  削除対象のページ（ないときはnull）
        // 戻り値：なし
        // メ　モ：最後に閉じたタブを再度開く機能があるため、最後に閉じたタブがさらにほかのタブ
        // 　　　　によって削除されるときに処理する。
        //=========================================================================================
        private void DisposeTabPage(TabPageInfo delTargetInfo) {
            if (delTargetInfo == null) {
                return;
            }
            delTargetInfo.Dispose();
        }

        //=========================================================================================
        // 機　能：現在のタブ以外をすべて閉じる
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DeleteOtherTab() {
            // 最後に閉じたタブはクリア
            DisposeTabPage(m_lastCloseTabInfo);
            m_lastCloseTabInfo = null;

            // タブを閉じる
            for (int i = m_tabControlMain.TabCount - 1; i >= 0; i--) {
                TabPage delPage = m_tabControlMain.TabPages[i];
                TabPageInfo delTargetInfo = (TabPageInfo)(delPage.Tag);
                if (delTargetInfo == Program.Document.CurrentTabPage) {
                    continue;
                }
                m_tabControlMain.TabPages.RemoveAt(i);

                // 管理から削除
                Program.Document.FileCrawlThread.OnDisposeUIFileList(delTargetInfo.LeftFileList);
                Program.Document.FileCrawlThread.OnDisposeUIFileList(delTargetInfo.RightFileList);
                Program.Document.TabPageList.Delete(delTargetInfo);
                DisposeTabPage(delTargetInfo);
            }
        }

        //=========================================================================================
        // 機　能：左側のタブをすべて閉じる
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DeleteLeftTab() {
            // 最後に閉じたタブはクリア
            DisposeTabPage(m_lastCloseTabInfo);
            m_lastCloseTabInfo = null;

            // 現在のタブのインデックスを取得
            int currentIndex = GetCurrentTabIndex();

            // タブを閉じる
            for (int i = currentIndex - 1; i >= 0; i--) {
                TabPage delPage = m_tabControlMain.TabPages[i];
                TabPageInfo delTargetInfo = (TabPageInfo)(delPage.Tag);
                m_tabControlMain.TabPages.RemoveAt(i);

                // 管理から削除
                Program.Document.FileCrawlThread.OnDisposeUIFileList(delTargetInfo.LeftFileList);
                Program.Document.FileCrawlThread.OnDisposeUIFileList(delTargetInfo.RightFileList);
                Program.Document.TabPageList.Delete(delTargetInfo);
                DisposeTabPage(delTargetInfo);
            }
        }

        //=========================================================================================
        // 機　能：右側のタブをすべて閉じる
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DeleteRightTab() {
            // 最後に閉じたタブはクリア
            DisposeTabPage(m_lastCloseTabInfo);
            m_lastCloseTabInfo = null;

            // 現在のタブのインデックスを取得
            int currentIndex = GetCurrentTabIndex();

            // タブを閉じる
            for (int i = m_tabControlMain.TabCount - 1; i >= currentIndex + 1; i--) {
                TabPage delPage = m_tabControlMain.TabPages[i];
                TabPageInfo delTargetInfo = (TabPageInfo)(delPage.Tag);
                m_tabControlMain.TabPages.RemoveAt(i);

                // 管理から削除
                Program.Document.FileCrawlThread.OnDisposeUIFileList(delTargetInfo.LeftFileList);
                Program.Document.FileCrawlThread.OnDisposeUIFileList(delTargetInfo.RightFileList);
                Program.Document.TabPageList.Delete(delTargetInfo);
                DisposeTabPage(delTargetInfo);
            }
        }

        //=========================================================================================
        // 機　能：指定されたインデックスのタブを選択する
        // 引　数：[in]index  選択するインデックス
        // 戻り値：なし
        //=========================================================================================
        public void SelectTabDirect(int index) {
            m_tabControlMain.SelectedTab = m_tabControlMain.TabPages[index];
        }

        //=========================================================================================
        // 機　能：現在のタブに対するインデックスを返す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public int GetCurrentTabIndex() {
            int currentIndex = -1;
            for (int i = 0; i < m_tabControlMain.TabCount; i++) {
                TabPage page = m_tabControlMain.TabPages[i];
                TabPageInfo pageInfo = (TabPageInfo)(page.Tag);
                if (pageInfo == Program.Document.CurrentTabPage) {
                    currentIndex = i;
                    break;
                }
            }
            if (currentIndex == -1) {
                Program.Abort("タブの管理情報が想定外です。");
            }
            return currentIndex;
        }

        //=========================================================================================
        // 機　能：タブページの追加をUIに反映する
        // 引　数：[in]tabPageInfo  追加するタブページの情報
        // 戻り値：なし
        //=========================================================================================
        public void AddTabPageUI(TabPageInfo tabPageInfo) {
            m_skipFocusGotEvent = true;
            try {
                // UIを作成
                TabPage tabPage = new TabPage();
                tabPage.Controls.Add(m_splitContainerFile);
                tabPage.TabIndex = 0;
                tabPage.Tag = tabPageInfo;

                // UIを登録
                tabPage.SuspendLayout();
                m_tabControlMain.Controls.Add(tabPage);
                tabPage.ResumeLayout(false);  

                m_tabUIList.Add(tabPage);
                m_tabControlMain.SelectedTab = tabPage;
            } finally {
                m_skipFocusGotEvent = false;
            }
          
            // フォーカスを一覧に戻す
            SetFocusFileList(tabPageInfo);

            // 情報を更新
            Program.Document.TabPageList.Add(tabPageInfo);
            SetCurrentTabPageTitle();
        }

        //=========================================================================================
        // 機　能：タブページのタイトルを設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetCurrentTabPageTitle() {
            TabPageInfo tabPage = Program.Document.CurrentTabPage;
            string left = tabPage.LeftFileList.DisplayDirectoryName;
            left = GenericFileStringUtils.GetFileName(GenericFileStringUtils.RemoveLastDirectorySeparator(left));
            string right = tabPage.RightFileList.DisplayDirectoryName;
            right = GenericFileStringUtils.GetFileName(GenericFileStringUtils.RemoveLastDirectorySeparator(right));

            // パス名を短くする
            const int LENGTH_MAX = 20;
            const int LENGTH_SHORT = 8;
            const int LENGTH_MIDDLE = 10;
            if (left.Length + right.Length > LENGTH_MAX) {
                if (left.Length < LENGTH_SHORT) {
                    right = StringUtils.MakeOmittedString(right, LENGTH_MAX - LENGTH_SHORT);
                } else if (right.Length < LENGTH_SHORT) {
                    left = StringUtils.MakeOmittedString(left, LENGTH_MAX - LENGTH_SHORT);
                } else {
                    left = StringUtils.MakeOmittedString(left, LENGTH_MIDDLE);
                    right = StringUtils.MakeOmittedString(right, LENGTH_MIDDLE);
                }
            }

            // タブに設定
            string title = string.Format(Resources.WindowTabTitle, left, right);
            title += "    ";                            // 閉じるボタン用
            if (m_tabControlMain.SelectedTab != null) {
                m_tabControlMain.SelectedTab.Text = title;
            }
        }

        //=========================================================================================
        // プロパティ：タブの再オープンの情報があるときtrue
        //=========================================================================================
        public bool ExistReopenInfo {
            get {
                return (m_lastCloseTabInfo != null);
            }
        }
    }
}
