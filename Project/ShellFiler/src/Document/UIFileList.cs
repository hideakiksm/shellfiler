using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.Util;
using ShellFiler.Virtual;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：ファイル一覧のUIに表示するファイル情報
    //=========================================================================================
    public class UIFileList {
        // マークの順番（起動後、1,2,3…とカウントアップ、リセットされない、0はマークなしの意図）
        private static int m_markOrder = 1;
        
        // 次に使用する読み込み完了時の世代
        private static int s_nextLoadedAge = 1;

        // 反対パス
        private UIFileList m_oppositeFileList;

        // 一覧のID
        private UIFileListId m_uiFileListId;

        // ファイルシステム
        private IFileSystem m_fileSystem;

        // ファイル一覧のコンテキスト情報
        private IFileListContext m_fileListContext;

        // ディレクトリ名
        private string m_directoryName;

        // パスヒストリ
        private PathHistory m_pathHistory;

        // ロード中のファイル一覧
        private IFileList m_fileListLoading;

        // UI情報付きのファイル一覧
        private List<UIFile> m_uiFileList = new List<UIFile>();

        // ソート方法
        private FileListSortMode m_sortMode;

        // ファイル一覧のフィルター（フィルターを使用していないときnull）
        private FileListFilterMode m_fileListFilterMode = null;

        // ファイル一覧のビューモード
        private FileListViewMode m_fileListViewMode;

        // ファイルリストが左ウィンドウで表示されるときtrue
        private bool m_leftWindow;

        // ロード中の状態
        private FileListLoadingStatus m_loadingStatus;

        // 読み込み完了時の世代（一覧の読み込みが終わったとき+1）
        private int m_loadedAge = 0;

        // ボリューム情報
        private VolumeInfo m_volumeInfo;

        // マークされたディレクトリの数
        private int m_markedDirectoryCount = 0;

        // マークされたファイルの数
        private int m_markedFileCount = 0;

        // マークされたファイルの数
        private long m_markedFileSize = 0;

        // ファイル名主部の最大表示幅(-1:不明)
        private int m_cxViewFileBody = -1;

        // 拡張子の最大表示幅（-1:不明）
        private int m_cxViewExtension = -1;

        // ファイル一覧のフィルターによりスキップされたファイルが存在するときtrue
        private bool m_existFilteringSkippedFile = true;

        // 読み込み状態に変化が生じたときの通知用delegate
        public delegate void LoadStateChangedEventHandler(object sender, LoadStateChangedEventArgs evt); 

        // 読み込み状態に変化が生じたときに通知するイベント
        public event LoadStateChangedEventHandler LoadStateChanged; 

        // Clone()に対応

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private UIFileList() {
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]leftWindow  左ウィンドウで表示されるときtrue
        // 　　　　[in]pathHistory パスヒストリの項目リスト
        // 　　　　[in]sortMode    ソート方法のモード
        // 　　　　[in]viewMode    ビューの表示方法
        // 戻り値：なし
        //=========================================================================================
        public UIFileList(bool leftWindow, PathHistory pathHistory, FileListSortMode sortMode, FileListViewMode viewMode) {
            m_uiFileListId = UIFileListId.NextId();
            m_pathHistory = pathHistory;
            m_sortMode = sortMode;
            m_fileListViewMode = viewMode;
            m_loadingStatus = FileListLoadingStatus.Failed;
            m_fileListContext = new DummyFileListContext();
            m_leftWindow = leftWindow;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dir   一覧を作成するWindowsのディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public void LoadInitialFile(string dir) {
            ChangeDirectoryStatus chdirStatus = ChangeDirectory(new ChangeDirectoryParam.Direct(dir));
            if (chdirStatus == ChangeDirectoryStatus.Success) {
                return;
            }
            string desktopDir = InitialFolder;
            chdirStatus = ChangeDirectory(new ChangeDirectoryParam.Direct(desktopDir));
            if (chdirStatus == ChangeDirectoryStatus.Success) {
                return;
            }
            Program.Abort("Desktopフォルダの一覧を取得できないため、初期化できませんでした。");
        }

        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            LoadStateChanged = null;
            if (m_fileListContext != null && m_fileListContext is VirtualFileListContext) {
                VirtualFolderInfo virtualInfo = ((VirtualFileListContext)m_fileListContext).VirtualFolderInfo;
                Program.Document.TemporaryManager.VirtualManager.EndUsingVirtualFolder(virtualInfo);
            }
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public UIFileList CloneForTabCopy() {
            UIFileList fileList = new UIFileList();
            fileList.m_uiFileListId = UIFileListId.NextId();
            fileList.m_fileSystem = m_fileSystem;
            fileList.m_fileListContext = m_fileListContext.CloneForTabCopy();
            fileList.m_directoryName = m_directoryName;
            fileList.m_pathHistory = (PathHistory)m_pathHistory.Clone();
            fileList.m_fileListLoading = null;
            fileList.m_uiFileList = new List<UIFile>();
            foreach (UIFile file in m_uiFileList) {
                fileList.m_uiFileList.Add((UIFile)(file.Clone()));
            }
            fileList.m_sortMode = (FileListSortMode)m_sortMode.Clone();
            fileList.m_fileListViewMode = (FileListViewMode)m_fileListViewMode.Clone();
            fileList.m_leftWindow = m_leftWindow;
            fileList.m_loadingStatus = FileListLoadingStatus.Completed;
            fileList.m_volumeInfo = (VolumeInfo)m_volumeInfo;
            fileList.m_fileListContext = m_fileListContext;
            fileList.m_markedDirectoryCount = m_markedDirectoryCount;
            fileList.m_markedFileCount = m_markedFileCount;
            fileList.m_markedFileSize = m_markedFileSize;
            fileList.m_cxViewFileBody = m_cxViewFileBody;
            fileList.m_cxViewExtension = m_cxViewExtension;
            fileList.m_existFilteringSkippedFile = m_existFilteringSkippedFile;
            fileList.LoadStateChanged = LoadStateChanged;
            return fileList;
        }

        //=========================================================================================
        // 機　能：ディレクトリを変更する
        // 引　数：[in]chdirMode ディレクトリ変更のモード
        // 戻り値：ディレクトリを変更のステータス
        //=========================================================================================
        public ChangeDirectoryStatus ChangeDirectory(ChangeDirectoryParam chdirMode) {
            // 一覧を取得
            m_loadingStatus = FileListLoadingStatus.Failed;
            IFileList fileList;
            if (m_fileSystem == null) {
                // 初回起動時
                fileList = Program.Document.FileSystemFactory.CreateFileList(null, chdirMode.TargetDirectory, m_leftWindow);
            } else {
                // 継続時は相対/絶対の指定が可能
                fileList = Program.Document.FileSystemFactory.CreateFileList(this, chdirMode.TargetDirectory, m_leftWindow);
            }
            if (fileList == null) {
                return ChangeDirectoryStatus.Failed;
            }

            // 読み込み開始（失敗した場合はそのまま）
            ChangeDirectoryStatus chdirStatus = fileList.GetFileList(chdirMode);
            if (chdirStatus != ChangeDirectoryStatus.Success && chdirStatus != ChangeDirectoryStatus.Loading) {
                return chdirStatus;
            }

            // ロード中の場合は一時保持
            if (chdirStatus == ChangeDirectoryStatus.Loading) {
                LoadStateChanged(this, new LoadStateChangedEventArgs(ChangeDirectoryStatus.Loading));
                m_fileListLoading = fileList;
                m_loadingStatus = FileListLoadingStatus.Loading;
                fileList.StartLoading(chdirMode);
                return ChangeDirectoryStatus.Loading;
            }

            // 成功した場合は差し替え
            StoreFileInfo(fileList, chdirMode);
            m_fileListLoading = null;
            m_loadingStatus = FileListLoadingStatus.Completed;
            return ChangeDirectoryStatus.Success;
        }

        //=========================================================================================
        // 機　能：バックグラウンドで読み込んだファイル一覧を差し替える
        // 引　数：[in]status             一覧取得のステータス
        // 　　　　[in]fileList           ファイル一覧（失敗のときはnull）
        // 　　　　[in]nextVirtualFolder  次に使用する仮想フォルダ情報（エラーのとき処理途中の情報、仮想dir以外はnull）
        // 　　　　[in]chdirParam         ディレクトリ変更のパラメータ
        // 戻り値：差し替えに成功したときtrue
        //=========================================================================================
        public bool SwapLoadedFileList(FileOperationStatus status, IFileList loadedFileList, VirtualFolderInfo nextVirtualFolder, ChangeDirectoryParam chdirParam) {
            // 途中で差し替わっている場合は破棄
            if (m_fileListLoading == null || loadedFileList == null || m_fileListLoading.LoadingGeneration != loadedFileList.LoadingGeneration) {
                if (nextVirtualFolder != null) {
                    Program.Document.TemporaryManager.VirtualManager.EndUsingVirtualFolder(nextVirtualFolder);
                }
                LoadStateChanged(this, new LoadStateChangedEventArgs(ChangeDirectoryStatus.Failed));
                m_loadingStatus = FileListLoadingStatus.Completed;
                return false;
            }

            if (status.Succeeded) {
                // 読み込み中のものが終わった場合は差し替え
                StoreFileInfo(loadedFileList, chdirParam);
                m_fileListLoading = null;
                m_loadingStatus = FileListLoadingStatus.Completed;
                m_loadedAge = s_nextLoadedAge++;
                LoadStateChanged(this, new LoadStateChangedEventArgs(ChangeDirectoryStatus.Success));
            } else {
                m_loadingStatus = FileListLoadingStatus.Completed;
                m_fileListLoading = null;
                m_loadedAge = s_nextLoadedAge++;
                LoadStateChanged(this, new LoadStateChangedEventArgs(ChangeDirectoryStatus.Failed));
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイル情報を取得してメンバに保存する
        // 引　数：[in]fileList   取り込み元のファイル一覧
        // 　　　　[in]chdirParam ディレクトリ変更のパラメータ
        // 戻り値：なし
        //=========================================================================================
        private void StoreFileInfo(IFileList fileList, ChangeDirectoryParam chdirParam) {
            // マーク状態の一覧を取得
            HashSet<string> markedFileName = new HashSet<string>();
            if (chdirParam.InheritMarkState) {
                foreach (UIFile uiFile in m_uiFileList) {
                    if (uiFile.Marked) {
                        markedFileName.Add(uiFile.FileName);
                    }
                }
            }

            // 新しいファイルシステムに移行
            m_fileSystem = fileList.FileSystem;
            if (fileList.FileListContext is VirtualFileListContext) {
                VirtualFolderInfo virtualInfo = ((VirtualFileListContext)(fileList.FileListContext)).VirtualFolderInfo;
                Program.Document.TemporaryManager.VirtualManager.BeginUsingVirtualFolder(virtualInfo, VirtualFolderTemporaryDirectory.UsingType.FileListUsing);
            }
            if (m_fileListContext != null && m_fileListContext is VirtualFileListContext) {
                VirtualFolderInfo virtualInfo = ((VirtualFileListContext)m_fileListContext).VirtualFolderInfo;
                Program.Document.TemporaryManager.VirtualManager.EndUsingVirtualFolder(virtualInfo);
            }
            m_fileListContext = fileList.FileListContext;

            // ファイル一覧のフィルターを更新
            if (chdirParam.FileListFilterSpecify) {
                // ファイルフィルター指定時
                m_fileListFilterMode = chdirParam.FileListFilterMode;
            } else {
                // ファイルフィルター継続使用時
                if (m_fileListFilterMode != null && !m_fileListFilterMode.IsSupportFileSystem(fileList.FileSystem.FileSystemId)) {
                    InfoBox.Information(Program.MainWindow, Resources.Msg_FileListFilterOtherFileSystem);
                    m_fileListFilterMode = null;
                }
            }
            CompareCondition condition = null;
            bool isPositive = true;
            if (m_fileListFilterMode != null) {
                condition = new CompareCondition(m_fileListFilterMode.ConditionList);
                isPositive = m_fileListFilterMode.IsPositive;
            }

            // ファイル一覧を更新
            FileIconManager iconManager = Program.Document.FileIconManager;
            m_fileSystem = fileList.FileSystem;
            m_directoryName = fileList.DirectoryName;
            m_existFilteringSkippedFile = false;
            m_uiFileList.Clear();
            foreach (IFile file in fileList.Files) {
                UIFile uiFile = new UIFile(file);
                uiFile.DefaultFileIconId = iconManager.GetDefaultIconId(uiFile);
                uiFile.FileIconId = uiFile.DefaultFileIconId;
                if (uiFile.Attribute.IsDirectory || uiFile.IsTargetFileWithCondition(condition, isPositive)) {
                    m_uiFileList.Add(uiFile);
                } else {
                    m_existFilteringSkippedFile = true;
                }
            }
            SetFolderSize();

            // カーソルファイルの指定がない場合は履歴に基づいてレジューム
            if (chdirParam.CursorFile == null && Configuration.Current.ResumeFolderCursorFile) {
                string dir = fileList.DirectoryName;
                bool ignoreCase = FileSystemID.IgnoreCaseFolderPath(fileList.FileSystem.FileSystemId);
                PathHistory currentPathHistory = m_pathHistory;
                PathHistory oppositePathHistory = m_oppositeFileList.m_pathHistory;
                PathHistoryItem item = PathHistory.GetLatestHistoryItem(currentPathHistory, oppositePathHistory, dir, ignoreCase, false);
                if (item == null) {
                    item = Program.Document.FolderHistoryWhole.GetHistoryItem(dir, ignoreCase);
                }
                if (item != null) {
                    chdirParam.CursorFile = item.FileName;
                }
            }

            // パスヒストリをリフレッシュ
            if (chdirParam is ChangeDirectoryParam.Direct || chdirParam is ChangeDirectoryParam.Initial || chdirParam is ChangeDirectoryParam.ChdirToParent) {
                PathHistory.AddItem(DisplayDirectoryName, "", fileList.FileSystem.FileSystemId);
            } else if (chdirParam is ChangeDirectoryParam.PathHistoryPrev) {
                PathHistory.CurrentIndex--;
            } else if (chdirParam is ChangeDirectoryParam.PathHistoryNext) {
                PathHistory.CurrentIndex++;
            }

            // ソートを実行
            SortFileList();

            // 状態を更新
            m_volumeInfo = fileList.VolumeInfo;
            m_markedDirectoryCount = 0;
            m_markedFileCount = 0;
            m_markedFileSize = 0;
            m_cxViewFileBody = -1;
            m_cxViewExtension = -1;

            // マーク状態を復活
            if (chdirParam.InheritMarkState) {
                for (int i = 0; i < m_uiFileList.Count; i++) {
                    UIFile uiFile = m_uiFileList[i];
                    if (markedFileName.Contains(uiFile.FileName)) {
                        SetMarked(i, true);
                    }
                }
            } else if(chdirParam.MarkFileNameList != null) {
                HashSet<string> markFileList = chdirParam.MarkFileNameList;
                for (int i = 0; i < m_uiFileList.Count; i++) {
                    UIFile uiFile = m_uiFileList[i];
                    if (markFileList.Contains(uiFile.FileName)) {
                        SetMarked(i, true);
                    }
                }
            }

            // 表示モードを更新
            m_fileListViewMode = ModifyFileListViewMode(chdirParam, m_directoryName);

            // 自動更新に登録
            if (m_leftWindow) {
                Program.Document.UIFileListWatcher.OnUIFileListChanged(this, null, true);
            } else {
                Program.Document.UIFileListWatcher.OnUIFileListChanged(null, this, true);
            }
        }

        //=========================================================================================
        // 機　能：ファイル一覧のビューモードを切り替える
        // 引　数：[in]chdirParam  フォルダ再読込のリクエスト
        // 　　　　[in]newDirName  新しいディレクトリ名
        // 戻り値：新しいビューモード
        //=========================================================================================
        private FileListViewMode ModifyFileListViewMode(ChangeDirectoryParam chdirParam, string newDirName) {
            // モードを切り替えない場合
            if (chdirParam is ChangeDirectoryParam.Initial || chdirParam is ChangeDirectoryParam.Refresh || chdirParam is ChangeDirectoryParam.UiOnly) {
                return m_fileListViewMode;
            }
            
            // フォルダ別設定に切り替える場合
            List<FileListViewModeAutoSetting.ModeEntry> settingList = Configuration.Current.FileListViewModeAutoSetting.FolderSetting;
            if (FileSystemID.IgnoreCaseFolderPath(m_fileSystem.FileSystemId)) {
                for (int i = 0; i < settingList.Count; i++) {
                    if (settingList[i].FolderName.Equals(newDirName, StringComparison.CurrentCultureIgnoreCase)) {
                        return (FileListViewMode)(settingList[i].ViewMode.Clone());
                    }
                }
            } else {
                for (int i = 0; i < settingList.Count; i++) {
                    if (settingList[i].FolderName == newDirName) {
                        return (FileListViewMode)(settingList[i].ViewMode.Clone());
                    }
                }
            }

            // 設定がない場合、汎用の設定
            if (Configuration.Current.FileListViewChangeMode == null) {
                return m_fileListViewMode;
            } else {
                return (FileListViewMode)(Configuration.Current.FileListViewChangeMode.Clone());
            }
        }

        //=========================================================================================
        // 機　能：指定されたファイルの一覧中のインデックスを返す
        // 引　数：[in]fileName  探すファイル名（nullのときは常に-1を返す）
        // 戻り値：ファイルのインデックス（見つからなかったとき-1）
        //=========================================================================================
        public int GetFileIndex(string fileName) {
            if (fileName == null) {
                return -1;
            }
            int fileCount = m_uiFileList.Count;
            int targetIndex = -1;
            if (FileSystemID.IgnoreCaseFolderPath(m_fileSystem.FileSystemId)) {
                for (int i = 0; i < fileCount; i++) {
                    if (m_uiFileList[i].FileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)) {
                        targetIndex = i;
                        break;
                    }
                }
            } else {
                for (int i = 0; i < fileCount; i++) {
                    if (m_uiFileList[i].FileName == fileName) {
                        targetIndex = i;
                        break;
                    }
                }
            }
            return targetIndex;
        }

        //=========================================================================================
        // 機　能：取得済みのフォルダサイズを設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetFolderSize() {
            RetrieveFolderSizeResult folderSizeResult = Program.Document.RetrieveFolderSizeResult;
            if (folderSizeResult == null) {
                return;
            }
            string subPath;         // 最後に「\」あり
            int depth;              // 実行階層のとき0
            bool isTarget = folderSizeResult.IsTargetFolder(m_fileSystem.FileSystemId, DisplayDirectoryName, out subPath, out depth);
            if (!isTarget) {
                return;
            }
            int childDepth = depth + 1;
            for (int i = 0; i < m_uiFileList.Count; i++) {
                UIFile uiFile = m_uiFileList[i];
                if (!uiFile.Attribute.IsDirectory) {
                    continue;
                }
                uiFile.FileSize = folderSizeResult.LookupFolderSize(subPath + uiFile.FileName, childDepth);
            }
        }

        //=========================================================================================
        // 機　能：タブページが切り替えられたときの処理を行う
        // 引　数：[in]tabPageInfo   新しくアクティブになったタブ
        // 戻り値：なし
        //=========================================================================================
        public void OnTabChanged(TabPageInfo tabPageInfo) {
            if (m_loadingStatus == FileListLoadingStatus.Loading) {
                m_fileListLoading = null;
                LoadStateChanged(this, new LoadStateChangedEventArgs(ChangeDirectoryStatus.Failed));
            }
        }

        //=========================================================================================
        // 機　能：ファイル一覧を並べ替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SortFileList() {
            UIFileSorter sorter = new UIFileSorter(m_sortMode);
            sorter.ExecSort(m_uiFileList);
        }

        //=========================================================================================
        // 機　能：ファイル一覧を並べ替える
        // 引　数：[in]index   マーク状態を変えるファイルの配列インデックス
        // 　　　　[in]marked  新しくマークするときtrue
        // 戻り値：なし
        //=========================================================================================
        public void SetMarked(int index, bool marked) {
            // マーク状態を変更
            UIFile file = m_uiFileList[index];
            if (file.Marked == marked) {
                return;
            }
            if (file.FileName == "..") {
                return;
            }
            if (marked) {
                file.SetInternalMarked(m_markOrder++);
            } else {
                file.SetInternalMarked(0);
            }

            // マークファイルを差分で計算
            if (file.Attribute.IsDirectory) {
                if (marked) {
                    m_markedDirectoryCount++;
                    m_markedFileSize += file.FileSize;
                } else {
                    m_markedDirectoryCount--;
                    m_markedFileSize -= file.FileSize;
                }
            } else {
                if (marked) {
                    m_markedFileCount++;
                    m_markedFileSize += file.FileSize;
                } else {
                    m_markedFileCount--;
                    m_markedFileSize -= file.FileSize;
                }
            }
        }

        //=========================================================================================
        // 機　能：画面上での最大表示幅を計算する
        // 引　数：[in]g  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public void CalcViewWidth(Graphics g, Font fileListFont) {
            if (m_cxViewFileBody != -1 || !Configuration.Current.FileListSeparateExt) {
                return;
            }
            int maxCxViewFileBody = 0;
            int maxCxViewExtension = 0;
            foreach (UIFile file in m_uiFileList) {
                int extensionPos = file.DisplayExtensionPos;
                string fileName = file.FileName;
                if (extensionPos != -1) {
                    string fileBody = fileName.Substring(0, extensionPos - 1);
                    string ext = fileName.Substring(extensionPos - 1);
                    int cxFileBody = (int)Math.Ceiling(g.MeasureString(fileBody, fileListFont).Width);
                    maxCxViewFileBody = Math.Max(maxCxViewFileBody, cxFileBody);
                    int cxExt = (int)Math.Ceiling(g.MeasureString(ext, fileListFont).Width);
                    maxCxViewExtension = Math.Max(maxCxViewExtension, cxExt);
                } else {
                    int cxFileName = (int)Math.Ceiling(g.MeasureString(fileName, fileListFont).Width);
                    maxCxViewFileBody = Math.Max(maxCxViewFileBody, cxFileName);
                }
            }
            m_cxViewFileBody = maxCxViewFileBody;
            m_cxViewExtension = maxCxViewExtension;
        }

        //=========================================================================================
        // 機　能：ファイル一覧をフルパスのファイル名一覧に変換する
        // 引　数：[in]fileList    作成対象のファイル一覧
        // 　　　　[in]uiFiles     一覧を作成するファイル
        // 　　　　[in]virtualTemp 仮想フォルダの場合、テンポラリのファイル名を返すときtrue
        // 戻り値：マークされたファイルの一覧（マーク順）
        //=========================================================================================
        public static string[] GetFullPathFileNameList(UIFileList fileList, List<UIFile> uiFiles, bool virtualTemp) {
            // 一覧を作成
            string[] fileNameList = new string[uiFiles.Count];
            for (int i = 0; i < uiFiles.Count; i++) {
                UIFile file = uiFiles[i];
                string fullPath = fileList.DisplayDirectoryName + file.FileName;
                fileNameList[i] = fullPath;
            }

            // 仮想フォルダの作業領域に変換
            if (virtualTemp && FileSystemID.IsVirtual(fileList.FileSystem.FileSystemId)) {
                List<string> listFileNameList = ArrayUtils.ArrayToList<string>(fileNameList);
                listFileNameList = fileList.m_fileListContext.GetExecuteLocalPathList(listFileNameList);
                fileNameList = ArrayUtils.ListToArray<string>(listFileNameList);
            }
            return fileNameList;
        }

        //=========================================================================================
        // プロパティ：はじめに表示されるフォルダ
        //=========================================================================================
        public static string InitialFolder {
            get {
                return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
        }

        //=========================================================================================
        // プロパティ：一覧のID
        //=========================================================================================
        public UIFileListId UIFileListId {
            get {
                return m_uiFileListId;
            }
        }

        //=========================================================================================
        // プロパティ：反対パス
        //=========================================================================================
        public UIFileList OppositeFileList {
            get {
                return m_oppositeFileList;
            }
            set {
                m_oppositeFileList = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルシステム
        //=========================================================================================
        public IFileSystem FileSystem {
            get {
                return m_fileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルリストが左ウィンドウで表示されるときtrue
        //=========================================================================================
        public bool IsLeftWindow {
            get {
                return m_leftWindow;
            }
        }

        //=========================================================================================
        // プロパティ：パスヒストリ
        //=========================================================================================
        public PathHistory PathHistory {
            get {
                return m_pathHistory;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧
        //=========================================================================================
        public List<UIFile> Files {
            get {
                return m_uiFileList;
            }
        }

        //=========================================================================================
        // プロパティ：マークされたファイルの一覧
        //=========================================================================================
        public List<UIFile> MarkFiles {
            get {
                UIMarkFileSorter sorter = new UIMarkFileSorter();
                return sorter.ExecSort(this.Files);
            }
        }

        //=========================================================================================
        // プロパティ：マークされたファイルの一覧（ディレクトリ以外）
        //=========================================================================================
        public List<UIFile> MarkFilesExceptFolder {
            get {
                List<UIFile> markFileList = MarkFiles;
                List<UIFile> result = new List<UIFile>();
                for (int i = 0; i < markFileList.Count; i++) {
                    if (!markFileList[i].Attribute.IsDirectory) {
                        result.Add(markFileList[i]);
                    }
                }
                return result;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルシステムに依存した抽象的なディレクトリ名（最後は必ずセパレータ）
        //=========================================================================================
        public string DisplayDirectoryName {
            get {
                return m_directoryName;
            }
        }

        //=========================================================================================
        // プロパティ：読み込み状態
        //=========================================================================================
        public FileListLoadingStatus LoadingStatus {
            get {
                return m_loadingStatus;
            }
        }

        //=========================================================================================
        // プロパティ：読み込み完了時の世代（一覧の読み込みが終わったとき+1）
        //=========================================================================================
        public int LoadedAge {
            get {
                return m_loadedAge;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルシステムごとのコンテキスト情報（一覧が決定するまではnull）
        //=========================================================================================
        public IFileListContext FileListContext {
            get {
                return m_fileListContext;
            }
        }

        //=========================================================================================
        // プロパティ：ボリューム情報（ロードできていないときnull）
        //=========================================================================================
        public VolumeInfo VolumeInfo {
            get {
                return m_volumeInfo;
            }
        }

        //=========================================================================================
        // プロパティ：マークされたディレクトリの数
        //=========================================================================================
        public int MarkedDirectoryCount {
            get {
                return m_markedDirectoryCount;
            }
        }

        //=========================================================================================
        // プロパティ：マークされたファイルの数
        //=========================================================================================
        public int MarkedFileCount {
            get {
                return m_markedFileCount;
            }
        }

        //=========================================================================================
        // プロパティ：マークされたファイルサイズ
        //=========================================================================================
        public long MarkedFileSize {
            get {
                return m_markedFileSize;
            }
        }

        //=========================================================================================
        // プロパティ：マークされたファイルサイズ（事前にCalcViewWidth()を呼び出しておくこと）
        //=========================================================================================
        public int CxViewFileBody {
            get {
                return m_cxViewFileBody;
            }
        }
        
        //=========================================================================================
        // プロパティ：拡張子の最大表示幅（事前にCalcViewWidth()を呼び出しておくこと）
        //=========================================================================================
        public int CxViewExtension {
            get {
                return m_cxViewExtension;
            }
        }

        //=========================================================================================
        // プロパティ：ソート方法
        //=========================================================================================
        public FileListSortMode SortMode {
            get {
                return m_sortMode;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のビューモード
        //=========================================================================================
        public FileListViewMode FileListViewMode {
            get {
                return m_fileListViewMode;
            }
            set {
                m_fileListViewMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のフィルター（フィルターを使用していないときnull）
        //=========================================================================================
        public FileListFilterMode FileListFilterMode {
            get {
                return m_fileListFilterMode;
            }
            set {
                m_fileListFilterMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のフィルターによりスキップされたファイルが存在するときtrue
        //=========================================================================================
        public bool ExistFilteringSkippedFile {
            get {
                return m_existFilteringSkippedFile;
            }
            set {
                m_existFilteringSkippedFile = value;
            }
        }

        //=========================================================================================
        // クラス：ロード状態変更イベントの引数
        //=========================================================================================
        public class LoadStateChangedEventArgs {
            // イベントの種類
            private ChangeDirectoryStatus m_eventType;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]eventType  イベントの種類
            // 戻り値：なし
            //=========================================================================================
            public LoadStateChangedEventArgs(ChangeDirectoryStatus eventType) {
                m_eventType = eventType;
            }

            //=========================================================================================
            // プロパティ：イベントの種類
            //=========================================================================================
            public ChangeDirectoryStatus EventType {
                get {
                    return m_eventType;
                }
            }
        }

    }
}
