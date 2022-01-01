using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Util;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.Locale;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ブックマークのカスタマイズ情報
    //=========================================================================================
    public class BookmarkSetting : ICloneable {
        // グループごとのディレクトリ情報
        private List<BookmarkGroup> m_bookmarkGroupList = new List<BookmarkGroup>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public BookmarkSetting() {
        }

        //=========================================================================================
        // 機　能：デフォルト値を設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetDefaultSetting() {
            m_bookmarkGroupList.Clear();
            BookmarkGroup group1 = CreateDefaultBookmarkGroup();
            m_bookmarkGroupList.Add(group1);
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            BookmarkSetting setting = new BookmarkSetting();
            for (int i = 0; i < m_bookmarkGroupList.Count; i++) {
                BookmarkGroup group = (BookmarkGroup)(m_bookmarkGroupList[i].Clone());
                setting.m_bookmarkGroupList.Add(group);
            }
            return setting;
        }

        //=========================================================================================
        // 機　能：データを読み込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void LoadData() {
#if !FREE_VERSION
            string fileName = DirectoryManager.BookmarkSetting;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = LoadSettingInternal(loader);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSetting, fileName, loader.ErrorDetail);
                SetDefaultSetting();
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadSettingInternal(SettingLoader loader) {
            // ファイルがないときはデフォルト
            if (!File.Exists(loader.FileName)) {
                SetDefaultSetting();
                return true;
            }

            // ファイルから読み込む
            bool success;
            success = loader.LoadSetting(false);
            if (!success) {
                return false;
            }

            // タグを読み込む
            m_bookmarkGroupList.Clear();
            success = loader.ExpectTag(SettingTag.Bookmark_Bookmark, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.Bookmark_Bookmark) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Bookmark_BookmarkGroup) {
                    BookmarkGroup group;
                    success = LoadBookmarkGroup(loader, out group);
                    if (!success) {
                        return false;
                    }
                    m_bookmarkGroupList.Add(group);
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：グループ以下を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[out]group  読み込んだグループを返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadBookmarkGroup(SettingLoader loader, out BookmarkGroup group) {
            bool success;
            group = new BookmarkGroup();
            group.GroupName = null;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.Bookmark_BookmarkGroup) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Bookmark_GroupName) {
                    group.GroupName = loader.StringValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Bookmark_BookmarkItem) {
                    BookmarkItem item;
                    success = LoadBookmarkItem(loader, out item);
                    if (!success) {
                        return false;
                    }
                    group.AddDirectory(item);
                }
            }
            if (group.GroupName == null) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ディレクトリ項目を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[out]item   読み込んだ項目を返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadBookmarkItem(SettingLoader loader, out BookmarkItem item) {
            bool success;
            item = null;
            string displayName = null;
            string directory = null;
            char shortcut = '\x00';
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.Bookmark_BookmarkItem) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Bookmark_ItemDisplayName) {
                    displayName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Bookmark_ItemDirectory) {
                    directory = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Bookmark_ItemShortCut) {
                    if (loader.StringValue.Length == 0) {
                        return false;
                    }
                    shortcut = loader.StringValue[0];
                }
            }
            if (displayName == null || directory == null || shortcut == '\x00') {
                return false;
            }

            // ディレクトリ名の移行
            if (loader.FileVersion < Ver.V1_4_0) {
                // Ver.1.3までで作られたSSHのディレクトリ名をsftp:...に移行
                if (directory.IndexOf('@') != -1 && directory.IndexOf('/') != -1 && directory.IndexOf('\\') == -1) {
                    if (directory.StartsWith("sftp:") || directory.StartsWith("ssh:")) {
                        ;
                    } else {
                        directory = "sftp:" + directory;
                    }
                }
            }

            item = new BookmarkItem(shortcut, displayName, directory);
            return true;
        }

        //=========================================================================================
        // 機　能：データを書き込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SaveData() {
#if !FREE_VERSION
            string fileName = DirectoryManager.BookmarkSetting;
            SettingSaver saver = new SettingSaver(fileName);
            bool success = SaveSettingInternal(saver);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotSaveSetting, fileName);
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private bool SaveSettingInternal(SettingSaver saver) {
            saver.StartObject(SettingTag.Bookmark_Bookmark);
            foreach (BookmarkGroup group in m_bookmarkGroupList) {
                saver.StartObject(SettingTag.Bookmark_BookmarkGroup);
                saver.AddString(SettingTag.Bookmark_GroupName, group.GroupName);
                foreach (BookmarkItem item in group.ItemList) {
                    saver.StartObject(SettingTag.Bookmark_BookmarkItem);
                    saver.AddString(SettingTag.Bookmark_ItemDisplayName, item.DisplayName);
                    saver.AddString(SettingTag.Bookmark_ItemDirectory, item.Directory);
                    saver.AddString(SettingTag.Bookmark_ItemShortCut, item.ShortCut.ToString());
                    saver.EndObject(SettingTag.Bookmark_BookmarkItem);
                }
                saver.EndObject(SettingTag.Bookmark_BookmarkGroup);
            }
            saver.EndObject(SettingTag.Bookmark_Bookmark);
            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // 機　能：グループを削除する
        // 引　数：[in]group  削除するグループ
        // 戻り値：なし
        //=========================================================================================
        public void DeleteGroup(BookmarkGroup group) {
            m_bookmarkGroupList.Remove(group);
        }

        //=========================================================================================
        // 機　能：グループの定義位置を入れ替える
        // 引　数：[in]index1  定義位置1
        // 　　　　[in]index2  定義位置2
        // 戻り値：なし
        //=========================================================================================
        public void SwapGroup(int index1, int index2) {
            BookmarkGroup group = m_bookmarkGroupList[index1];
            m_bookmarkGroupList.RemoveAt(index1);
            m_bookmarkGroupList.Insert(index2, group);
        }
        
        //=========================================================================================
        // 機　能：グループを追加する
        // 引　数：[in]group   追加するグループ
        // 戻り値：なし
        //=========================================================================================
        public void AddGroup(BookmarkGroup group) {
            m_bookmarkGroupList.Add(group);
        }
        
        //=========================================================================================
        // 機　能：グループを挿入する
        // 引　数：[in]index   挿入する位置
        // 　　　　[in]group   追加するグループ
        // 戻り値：なし
        //=========================================================================================
        public void InsertGroup(int index, BookmarkGroup group) {
            m_bookmarkGroupList.Insert(index, group);
        }

        //=========================================================================================
        // 機　能：デフォルトのブックマーク定義のグループを返す
        // 引　数：なし
        // 戻り値：デフォルト定義
        //=========================================================================================
        public static BookmarkGroup CreateDefaultBookmarkGroup() {
            string exeFolder = GenericFileStringUtils.GetDirectoryName(GenericFileStringUtils.RemoveLastDirectorySeparator(Program.InstallPath));
            BookmarkGroup group = new BookmarkGroup();
            group.GroupName = Resources.DefaultBookmark_Group;
            group.AddDirectory(new BookmarkItem('T', Resources.DefaultBookmark_ItemDesktop,    Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
            group.AddDirectory(new BookmarkItem('M', Resources.DefaultBookmark_ItemMyDocument, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
            group.AddDirectory(new BookmarkItem('D', Resources.DefaultBookmark_ItemDownload,   KnownUrl.WindowsDownloadFolder));
            group.AddDirectory(new BookmarkItem('S', Resources.DefaultBookmark_StartMenu,      Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu))));
            group.AddDirectory(new BookmarkItem('W', Resources.DefaultBookmark_Windows,        Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System))));
            group.AddDirectory(new BookmarkItem('R', Resources.DefaultBookmark_ProgramFiles,   Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Programs))));
            group.AddDirectory(new BookmarkItem('P', Resources.DefaultBookmark_Application,    GenericFileStringUtils.GetDirectoryName(GenericFileStringUtils.RemoveLastDirectorySeparator(exeFolder))));
            group.AddDirectory(new BookmarkItem('-', Resources.DefaultBookmark_ShellFiler,     exeFolder));
            return group;
        }

        //=========================================================================================
        // プロパティ：ブックマーク情報のリスト
        //=========================================================================================
        public List<BookmarkGroup> BookmarkGroupList {
            get {
                return m_bookmarkGroupList;
            }
        }
    }
}
