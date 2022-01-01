using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Api;
using ShellFiler.Command;
using ShellFiler.Util;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：アイコンの管理クラス
    //=========================================================================================
    public class FileIconManager {
        // デフォルトアイコンのプール
        private Dictionary<FileListViewIconSize, FileIconPool> m_defaultIconPool = new Dictionary<FileListViewIconSize, FileIconPool>();

        // 各ファイル一覧に対する読み込み済みアイコンのプール
        private Dictionary<UIFileListId, FileIconPool> m_iconPool = new Dictionary<UIFileListId, FileIconPool>();

        // 各ファイル一覧に対する読み込み済みサムネイルのプール
        private Dictionary<UIFileListId, FileIconPool> m_thumbnailPool = new Dictionary<UIFileListId, FileIconPool>();

        // ファイル一覧に対するアイコンプールにアクセスするためのロック
        private object m_lockIconPool = new object();

        // デフォルトのフォルダアイコンのID
        private FileIconID m_idFolderIcon;

        // デフォルトの実行ファイルアイコンのID
        private FileIconID m_idExeIcon;
        
        // デフォルトの通常ファイルアイコンのID
        private FileIconID m_idFileIcon;
        
        // アイコンを描画するためのdelegate
        // iconはnull可（見つからなかったとき）
        // 描画できたときtrueを返す
        public delegate bool DrawIconDelegate(FileIcon icon);

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileIconManager() {
            LoadDefaultIcon();
        }

        //=========================================================================================
        // 機　能：デフォルトアイコンを読み込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void LoadDefaultIcon() {
            m_idFolderIcon = FileIconID.NextId(FileIconID.FileIconType.DefaultIcon, FileListViewIconSize.IconSizeNull, UIFileListId.NullId);
            m_idExeIcon = FileIconID.NextId(FileIconID.FileIconType.DefaultIcon, FileListViewIconSize.IconSizeNull, UIFileListId.NullId);
            m_idFileIcon = FileIconID.NextId(FileIconID.FileIconType.DefaultIcon, FileListViewIconSize.IconSizeNull, UIFileListId.NullId);
            
            FileListViewIconSize[] allSize = FileListViewIconSize.AllSize;
            for (int i = 0; i < allSize.Length; i++) {
                m_defaultIconPool.Add(allSize[i], new FileIconPool(false));
            }

            // ダミーで一度読み込んでおく
            Win32IconUtils.GetFileIconBitmap(Win32IconUtils.SampleFolderPath, allSize[0].IconSize, Win32IconUtils.ICONMODE_WITH_OVERRAY);

            // フォルダ
            Bitmap bmpFolderIconPrev = null;
            for (int i = 0; i < allSize.Length; i++) {
                Bitmap bmpFolderIcon = Win32IconUtils.GetFileIconBitmap(Win32IconUtils.SampleFolderPath, allSize[i].IconSize, Win32IconUtils.ICONMODE_NORMAL);
                if (bmpFolderIcon != null) {
                    bmpFolderIconPrev = bmpFolderIcon;
                } else {
                    bmpFolderIcon = bmpFolderIconPrev;
                }
                m_defaultIconPool[allSize[i]].AddIcon(new FileIcon(m_idFolderIcon, bmpFolderIcon));         // Bitmapはアタッチ
            }

            // 実行ファイル
            Bitmap bmpExeIconPrev = null;
            for (int i = 0; i < allSize.Length; i++) {
                Bitmap bmpExeIcon = Win32IconUtils.GetFileIconBitmap("a.exe", allSize[i].IconSize, Win32IconUtils.ICONMODE_NORMAL | Win32IconUtils.ICONMODE_EXTENSION_ONLY);
                if (bmpExeIcon != null) {
                    bmpExeIconPrev = bmpExeIcon;
                } else {
                    bmpExeIcon = bmpExeIconPrev;
                }
                m_defaultIconPool[allSize[i]].AddIcon(new FileIcon(m_idExeIcon, bmpExeIcon));               // Bitmapはアタッチ
            }

            // 通常ファイル
            Bitmap bmpFileIconPrev = null;
            for (int i = 0; i < allSize.Length; i++) {
                Bitmap bmpFileIcon = Win32IconUtils.GetFileIconBitmap("$file", allSize[i].IconSize, Win32IconUtils.ICONMODE_NORMAL | Win32IconUtils.ICONMODE_EXTENSION_ONLY);
                if (bmpFileIcon != null) {
                    bmpFileIconPrev = bmpFileIcon;
                } else {
                    bmpFileIcon = bmpFileIconPrev;
                }
                m_defaultIconPool[allSize[i]].AddIcon(new FileIcon(m_idFileIcon, bmpFileIcon));             // Bitmapはアタッチ
            }
Bitmap bmp = Win32IconUtils.GetFileIconBitmap(@"E:\docfix\kiidocs\ja\samples.mkd", allSize[0].IconSize, Win32IconUtils.ICONMODE_WITH_OVERRAY);

        }
        
        //=========================================================================================
        // 機　能：イベントを接続する
        // 引　数：[in]tabPageList  タブ情報一覧
        // 戻り値：なし
        //=========================================================================================
        public void ConnectEvent(TabPageList tabPageList) {
            tabPageList.TabPageChanged += new TabPageList.TabPageChangedEventHandler(TabPageList_TabPageChanged);
        }
 
        //=========================================================================================
        // 機　能：タブページ一覧の状態に変化が発生したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        // メ　モ：UIスレッドから呼ばれる
        //=========================================================================================
        private void TabPageList_TabPageChanged(object sender, TabPageList.TabPageChangedEventArgs evt) {
 	        if (evt.Added) {
                // タブページが追加された
                TabPageInfo tabPage = evt.TabPageInfo;
                lock (m_lockIconPool) {
                    m_iconPool.Add(tabPage.LeftFileList.UIFileListId, new FileIconPool(true));
                    m_iconPool.Add(tabPage.RightFileList.UIFileListId, new FileIconPool(true));
                    m_thumbnailPool.Add(tabPage.LeftFileList.UIFileListId, new FileIconPool(true));
                    m_thumbnailPool.Add(tabPage.RightFileList.UIFileListId, new FileIconPool(true));
                }
            } else {
                // タブページが削除された
                lock (m_lockIconPool) {
                    UIFileList leftFileList = evt.TabPageInfo.LeftFileList;
                    FileIconPool poolLeft = GetFileIconPool(leftFileList.UIFileListId);
                    if (poolLeft != null) {
                        poolLeft.ClearAll();
                        m_iconPool.Remove(leftFileList.UIFileListId);
                    }
                    FileIconPool poolLeft2 = GetFileThumbnailPool(leftFileList.UIFileListId);
                    if (poolLeft2 != null) {
                        poolLeft2.ClearAll();
                        m_thumbnailPool.Remove(leftFileList.UIFileListId);
                    }
                    UIFileList rightFileList = evt.TabPageInfo.RightFileList;
                    FileIconPool poolRight = GetFileIconPool(rightFileList.UIFileListId);
                    if (poolRight != null) {
                        poolRight.ClearAll();
                        m_iconPool.Remove(rightFileList.UIFileListId);
                    }
                    FileIconPool poolRight2 = GetFileThumbnailPool(rightFileList.UIFileListId);
                    if (poolRight2 != null) {
                        poolRight2.ClearAll();
                        m_thumbnailPool.Remove(rightFileList.UIFileListId);
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイル一覧に対するアイコンプールを取得する
        // 引　数：[in]fileListId  ファイル一覧のID
        // 戻り値：アイコンプール
        //=========================================================================================
        private FileIconPool GetFileIconPool(UIFileListId fileListId) {
            lock (m_lockIconPool) {
                if (m_iconPool.ContainsKey(fileListId)) {
                    return m_iconPool[fileListId];
                } else {
                    return null;
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイル一覧に対するサムネイルプールを取得する
        // 引　数：[in]fileListId  ファイル一覧のID
        // 戻り値：アイコンプール
        //=========================================================================================
        private FileIconPool GetFileThumbnailPool(UIFileListId fileListId) {
            lock (m_lockIconPool) {
                if (m_thumbnailPool.ContainsKey(fileListId)) {
                    return m_thumbnailPool[fileListId];
                } else {
                    return null;
                }
            }
        }

        //=========================================================================================
        // 機　能：アイコンを取得する
        // 引　数：[in]iconId        アイコンのID
        // 　　　　[in]defaultIconId デフォルトのアイコンID（iconIdがデフォルトアイコン、または取得しないときNullId）
        // 　　　　[in]iconSize      取得するアイコンのサイズ
        // 戻り値：アイコン（取得できなかったときnull）
        // メ　モ：UIスレッドから呼ばれる
        //=========================================================================================
        public FileIcon GetFileIcon(FileIconID iconId, FileIconID defaultIconId, FileListViewIconSize iconSize) {
            FileIcon icon = null;
            switch (iconId.IconType) {
                case FileIconID.FileIconType.DefaultIcon:
                    icon = m_defaultIconPool[iconSize].GetFileIcon(iconId);
                    break;
                case FileIconID.FileIconType.FileListIcon: {
                    if (iconId.FileListViewIconSize == iconSize) {
                        FileIconPool pool = GetFileIconPool(iconId.FileListId);
                        if (pool != null) {
                            icon = pool.GetFileIcon(iconId);
                        }
                    }
                    break;
                }
            }
            if (icon == null && defaultIconId != FileIconID.NullId) {
                switch (defaultIconId.IconType) {
                    case FileIconID.FileIconType.DefaultIcon:
                        icon = m_defaultIconPool[iconSize].GetFileIcon(defaultIconId);
                        break;
                    case FileIconID.FileIconType.FileListIcon: {
                        FileIconPool pool = GetFileIconPool(defaultIconId.FileListId);
                        if (pool != null) {
                            icon = pool.GetFileIcon(defaultIconId);
                        }
                        break;
                    }
                }
            }
            return icon;
        }
        
        //=========================================================================================
        // 機　能：ロック配下でアイコンを描画する
        // 引　数：[in]iconId        アイコンのID
        // 　　　　[in]defaultIconId デフォルトのアイコンID
        // 　　　　[in]drawDelegate  描画処理
        // 戻り値：描画できたときtrue
        // メ　モ：UIスレッドから呼ばれる
        //=========================================================================================
        public bool DrawFileIcon(FileIconID iconId, FileIconID defaultIconId, FileListViewIconSize iconSize, DrawIconDelegate drawDelegate) {
            bool drawed;
            lock (m_lockIconPool) {
                FileIcon icon = GetFileIcon(iconId, defaultIconId, iconSize);
                drawed = drawDelegate(icon);
            }
            return drawed;
        }
        
        //=========================================================================================
        // 機　能：アイコンを取得する
        // 引　数：[in]iconId        アイコンのID
        // 　　　　[in]drawDelegate  描画処理
        // 戻り値：描画できたときtrue
        // メ　モ：UIスレッドから呼ばれる
        //=========================================================================================
        public bool DrawFileThumbnail(FileIconID iconId, FileIconManager.DrawIconDelegate drawDelegate) {
            bool drawed;
            lock (m_lockIconPool) {
                FileIcon icon = null;
                if (iconId != FileIconID.NullId) {
                    FileIconPool pool = GetFileThumbnailPool(iconId.FileListId);
                    if (pool != null) {
                        icon = pool.GetFileIcon(iconId);
                    }
                }
                drawed = drawDelegate(icon);
            }
            return drawed;
        }

        //=========================================================================================
        // 機　能：アイコンを登録する
        // 引　数：[in]fileListId  扱うファイル一覧のID
        // 　　　　[in]iconSize    アイコンのサイズ
        // 　　　　[in]bmp         アイコンのビットマップ（画像はアタッチする）
        // 戻り値：登録したアイコンのID（指定されたファイル一覧が処理できないときFileIconId.NullId）
        // メ　モ：ファイル一覧スレッドから呼ばれる
        //=========================================================================================
        public FileIconID AddIconBitmap(UIFileListId fileListId, FileListViewIconSize iconSize, Bitmap bmp) {
            FileIconPool pool = GetFileIconPool(fileListId);
            if (pool != null) {
                FileIconID iconId = FileIconID.NextId(FileIconID.FileIconType.FileListIcon, iconSize, fileListId);
//System.Diagnostics.Debug.WriteLine("add:" + iconId.IdValue);

                FileIcon fileIcon = new FileIcon(iconId, bmp);
                return pool.AddIcon(fileIcon);
            } else {
                return FileIconID.NullId;
            }
        }

        //=========================================================================================
        // 機　能：指定ウィンドウのアイコンをすべて削除する
        // 引　数：[in]fileListId  扱うファイル一覧のID
        // 戻り値：なし
        // メ　モ：ファイル一覧スレッドから呼ばれる
        //=========================================================================================
        public void ClearWindowIcon(UIFileListId fileListId) {
            lock (m_lockIconPool) {
                FileIconPool pool = GetFileIconPool(fileListId);
                if (pool != null) {
                    pool.ClearAll();
                }
                FileIconPool pool2 = GetFileThumbnailPool(fileListId);
                if (pool2 != null) {
                    pool2.ClearAll();
                }
            }
        }

        //=========================================================================================
        // 機　能：デフォルトのアイコンを取得する
        // 引　数：[in]file  ファイル情報
        // 戻り値：アイコンID
        //=========================================================================================
        public FileIconID GetDefaultIconId(UIFile file) {
            if (file.Attribute.IsDirectory) {
                return m_idFolderIcon;
            } else if (file.Extension.ToLower().Equals("exe") || file.Attribute.IsExecutable) {
                return m_idExeIcon;
            } else {
                return m_idFileIcon;
            }
        }
        
        //=========================================================================================
        // 機　能：使用するサムネイルファイルを設定する
        // 引　数：[in]fileList     ファイル一覧
        // 　　　　[in]targetFiles  使用するファイルの一覧
        // 戻り値：なし
        // メ　モ：UIスレッドから呼ばれる
        //=========================================================================================
        public void SetUseThumbnailFiles(UIFileList fileList, List<UIFileWithIndex> targetFiles) {
        }

        //=========================================================================================
        // プロパティ：デフォルトのフォルダアイコンのID
        //=========================================================================================
        public FileIconID DefaultFolderIconId {
            get {
                return m_idFolderIcon;
            }
        }

        //=========================================================================================
        // プロパティ：デフォルトの実行ファイルアイコン
        //=========================================================================================
        public FileIconID DefaultExeIconId {
            get {
                return m_idExeIcon;
            }
        }

        //=========================================================================================
        // プロパティ：デフォルトの通常ファイルアイコン
        //=========================================================================================
        public FileIconID DefaultFileIconId {
            get {
                return m_idFileIcon;
            }
        }
    }
}
