using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileTask;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.Management;
using ShellFiler.GraphicsViewer.Filter;
using ShellFiler.Archive;
using ShellFiler.UI.FileList;
using ShellFiler.UI.FileList.Crawler;
using ShellFiler.UI.FileList.DefaultList;
using ShellFiler.FileViewer;
using ShellFiler.GraphicsViewer;
using ShellFiler.Virtual;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：ShellFilerのドキュメント関連全般の管理クラス
    //=========================================================================================
    class SfDocument {
        // ユーザーのライセンス情報
        private UserLicenseInfo m_userLicenseInfo = null;

        // 開いているタブページの一覧
        private TabPageList m_tabPageList = null;

        // 現在開いているタブページ
        private TabPageInfo m_currentTabPage = null;

        // ユーザーの設定（個別ファイルとして保存）
        private UserSetting m_userSetting = null;

        // ユーザーの一般設定（一般設定としてまとめて保存）
        private UserGeneralSetting m_userGeneralSetting = null;

        // メニューのカスタマイズ情報
        private MenuSetting m_menuSetting = null;

        // ツールバーのカスタマイズ情報
        private ToolbarSetting m_toolbarSetting = null;

        // キーのカスタマイズ情報
        private KeySetting m_keySetting = null;

        // アドレスバーの情報
        private AddressBarSetting m_addressBarSetting = null;
        
        // SSHのユーザ認証情報の設定
        private SSHUserAuthenticateSetting m_sshUserAuthenticateSetting = null;

        // SSHサーバーの仕様設定
        private OSSpecSetting m_osSpecSetting = null;

        // フォルダ履歴
        private FolderHistoryWhole m_folderHistoryWhole = null;

        // ファイルビューアの比較バッファ
        private FileViewerSelectionCompareBuffer m_fileViewerSelectionCompareBuffer = null;

        // スライドショーでのマーク結果（null:マーク情報がない）
        private SlideShowMarkResult m_slideShowMarkResult = null;

        // コマンドファクトリ
        private CommandFactory m_commandFactory = null;

        // ファイルシステムIDからコマンド名辞書へのマップ
        private Dictionary<FileSystemID, CommandNameDictionary> m_commandNameDictionary = null;

        // ファイルシステムファクトリ
        private FileSystemFactory m_fileSystemFactory = null;

        // 圧縮／展開のファクトリ
        private ArchiveFactory m_archiveFactory = null;

        // バックグラウンドタスク
        private BackgroundTaskManager m_backgroundTaskList = null;

        // テンポラリファイルの管理クラス
        private TemporaryManager m_temporaryManager = null;

        // アイコンの管理クラス
        private FileIconManager m_fileIconManager = null;

        // ファイルバックグラウンドクローラ
        private FileCrawlThread m_fileCrawlThread = null;

        // 表示中のUIファイル一覧の変更監視クラス
        private UIFileListWatcher m_uiFileListWatcher = null;

        // バックグラウンドでリクエストを処理するためのクラス
        private UIRequestBackgroundThread m_uiRequestBackgroundThread;

        // グラフィックビューアのフィルタースレッド
        private FilterThread m_graphicsViewerFilterThread = null;

        // ファイルフィルターの管理クラス
        private FileFilterManager m_fileFilterManager;

        // ファイルの転送条件
        private FileConditionSetting m_fileConditionSetting = null;

        // フォルダサイズの取得結果（保持していないときはnull）
        private RetrieveFolderSizeResult m_retrieveFolderSizeResult = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SfDocument() {
            Configuration.Initialize();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
            m_tabPageList = new TabPageList();
            m_userSetting = new UserSetting();
            m_userGeneralSetting = new UserGeneralSetting();
            m_menuSetting = new MenuSetting();
            m_toolbarSetting = new ToolbarSetting();
            m_keySetting = new KeySetting();
            m_addressBarSetting = new AddressBarSetting();
            m_sshUserAuthenticateSetting = new SSHUserAuthenticateSetting();
            m_osSpecSetting = new OSSpecSetting();
            m_folderHistoryWhole = new FolderHistoryWhole();
            m_fileViewerSelectionCompareBuffer = new FileViewerSelectionCompareBuffer();
            m_commandFactory = new CommandFactory();
            m_archiveFactory = new ArchiveFactory();
            m_commandNameDictionary = new Dictionary<FileSystemID, CommandNameDictionary>();
            m_fileSystemFactory = new FileSystemFactory();
            m_backgroundTaskList = new BackgroundTaskManager();
            m_temporaryManager = new TemporaryManager();
            m_fileIconManager = new FileIconManager();
            m_fileCrawlThread = new FileCrawlThread();
            m_uiFileListWatcher = new UIFileListWatcher();
            m_uiRequestBackgroundThread = new UIRequestBackgroundThread();
            m_fileFilterManager = new FileFilterManager();
            m_fileConditionSetting = new FileConditionSetting();

            SettingWarningList warningList = new SettingWarningList();
            m_keySetting.Initialize(warningList);
            m_toolbarSetting.Initialize();
            m_menuSetting.Initialize(warningList);
            m_userSetting.Initialize();
            m_userGeneralSetting.Initialize();
            m_folderHistoryWhole.Initialize();

#if !FREE_VERSION
            warningList.DisplayWarningInfo();
            m_keySetting.CheckKeySettingMerge();
#endif

            m_fileIconManager.ConnectEvent(m_tabPageList);
        }

        //=========================================================================================
        // 機　能：ファイル一覧を読み込む
        // 引　数：なし
        // 戻り値：読み込んだタブページの情報
        //=========================================================================================
        public TabPageInfo LoadFileList() {
            // 初期ディレクトリを決定
            string leftDir = Configuration.Current.InitialDirectoryLeft;
            if (leftDir == "") {
                leftDir = m_userSetting.InitialSetting.LeftInitialFolder;
                if (leftDir == "") {
                    leftDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
            }
            string rightDir = Configuration.Current.InitialDirectoryRight;
            if (rightDir == "") {
                rightDir = m_userSetting.InitialSetting.RightIntialFolder;
                if (rightDir == "") {
                    rightDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
            }

            // 初期化
            FileListSortMode leftSortMode;
            if (Configuration.Current.DefaultFileListSortModeLeft == null) {
                leftSortMode = (FileListSortMode)(m_userSetting.InitialSetting.FileListSortModeLeft.Clone());
            } else {
                leftSortMode = (FileListSortMode)(Configuration.Current.DefaultFileListSortModeLeft.Clone());
            }
            FileListSortMode rightSortMode;
            if (Configuration.Current.DefaultFileListSortModeRight == null) {
                rightSortMode = (FileListSortMode)(m_userSetting.InitialSetting.FileListSortModeRight.Clone());
            } else {
                rightSortMode = (FileListSortMode)(Configuration.Current.DefaultFileListSortModeRight.Clone());
            }
            FileListViewMode leftViewMode;
            if (Configuration.Current.DefaultViewModeLeft == null) {
                leftViewMode = (FileListViewMode)(m_userSetting.InitialSetting.FileListViewModeLeft.Clone());
            } else {
                leftViewMode = (FileListViewMode)(Configuration.Current.DefaultViewModeLeft.Clone());
            }
            FileListViewMode rightViewMode;
            if (Configuration.Current.DefaultViewModeRight == null) {
                rightViewMode = (FileListViewMode)(m_userSetting.InitialSetting.FileListViewModeRight.Clone());
            } else {
                rightViewMode = (FileListViewMode)(Configuration.Current.DefaultViewModeRight.Clone());
            }
            
            UIFileList leftFileList = new UIFileList(true, new PathHistory(), leftSortMode, leftViewMode);
            UIFileList rightFileList = new UIFileList(false, new PathHistory(), rightSortMode, rightViewMode);
            leftFileList.OppositeFileList = rightFileList;
            rightFileList.OppositeFileList = leftFileList;
            
            leftFileList.LoadInitialFile(leftDir);
            rightFileList.LoadInitialFile(rightDir);
            IFileListViewState leftViewState = FileListViewUtils.GetDefaultFileListViewState(leftViewMode);
            IFileListViewState rightViewState = FileListViewUtils.GetDefaultFileListViewState(rightViewMode);
            TabPageInfo tabPageInfo = new TabPageInfo(leftFileList, rightFileList, leftViewState, rightViewState, true);
            return tabPageInfo;
        }

        //=========================================================================================
        // 機　能：終了処理を行う
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：メインウィンドウのクローズ処理中に呼ばれる
        //=========================================================================================
        public void Dispose() {
            m_userSetting.SaveSetting();
            m_userGeneralSetting.SaveSetting();
            m_folderHistoryWhole.SaveSetting();

            if (m_uiRequestBackgroundThread != null) {
                m_uiRequestBackgroundThread.JoinThread();
                m_uiRequestBackgroundThread = null;
            }
            if (m_fileSystemFactory != null) {
                m_fileSystemFactory.Dispose();
            }
            if (m_fileCrawlThread != null) {
                m_fileCrawlThread.Dispose();
            }
            if (m_temporaryManager != null) {
                m_temporaryManager.Dispose();
            }
            if (m_graphicsViewerFilterThread != null) {
                m_graphicsViewerFilterThread.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：指定されたファイルシステムに対するコマンド名辞書を返す
        // 引　数：[in]fileSystemId  ファイルシステムのID
        // 戻り値：コマンド名辞書
        //=========================================================================================
        public CommandNameDictionary GetCommandNameDictionary(FileSystemID fileSystemId) {
            if (m_commandNameDictionary.ContainsKey(fileSystemId)) {
                return m_commandNameDictionary[fileSystemId];
            } else {
                CommandNameDictionary commandDic = new CommandNameDictionary(fileSystemId);
                m_commandNameDictionary.Add(fileSystemId, commandDic);
                return commandDic;
            }
        }

        //=========================================================================================
        // プロパティ：ユーザーのライセンス情報
        //=========================================================================================
        public UserLicenseInfo UserLicenseInfo {
            get {
                return m_userLicenseInfo;
            }
            set {
                m_userLicenseInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：開いているタブページの一覧
        //=========================================================================================
        public TabPageList TabPageList {
            get {
                return m_tabPageList;
            }
        }

        //=========================================================================================
        // プロパティ：現在開いているタブページ
        //=========================================================================================
        public TabPageInfo CurrentTabPage {
            get {
                return m_currentTabPage;
            }
            set {
                m_currentTabPage = value;
                m_uiFileListWatcher.OnUIFileListChanged(m_currentTabPage.LeftFileList, m_currentTabPage.RightFileList, false);
            }
        }

        //=========================================================================================
        // プロパティ：ユーザーの設定（個別ファイルとして保存）
        //=========================================================================================
        public UserSetting UserSetting {
            get {
                return m_userSetting;
            }
        }

        //=========================================================================================
        // プロパティ：ユーザーの一般設定（一般設定としてまとめて保存）
        //=========================================================================================
        public UserGeneralSetting UserGeneralSetting {
            get {
                return m_userGeneralSetting;
            }
        }

        //=========================================================================================
        // プロパティ：メニューのカスタマイズ情報
        //=========================================================================================
        public MenuSetting MenuSetting {
            get {
                return m_menuSetting;
            }
            set {
                m_menuSetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：ツールバーのカスタマイズ情報
        //=========================================================================================
        public ToolbarSetting ToolbarSetting {
            get {
                return m_toolbarSetting;
            }
        }
        
        //=========================================================================================
        // プロパティ：キーのカスタマイズ情報
        //=========================================================================================
        public KeySetting KeySetting {
            get {
                return m_keySetting;
            }
            set {
                m_keySetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：アドレスバーの情報
        //=========================================================================================
        public AddressBarSetting AddressBarSetting {
            get {
                return m_addressBarSetting;
            }
        }

        //=========================================================================================
        // プロパティ：キー／マウス入力のコマンドファクトリ
        //=========================================================================================
        public CommandFactory CommandFactory {
            get {
                return m_commandFactory;
            }
        }

        //=========================================================================================
        // プロパティ：SSHのユーザ認証情報の設定
        //=========================================================================================
        public SSHUserAuthenticateSetting SSHUserAuthenticateSetting {
            get {
                return m_sshUserAuthenticateSetting;
            }
        }

        //=========================================================================================
        // プロパティ：SSHのユーザ認証情報の設定
        //=========================================================================================
        public OSSpecSetting OSSpecSetting {
            get {
                return m_osSpecSetting;
            }
        }

        //=========================================================================================
        // プロパティ：フォルダ履歴
        //=========================================================================================
        public FolderHistoryWhole FolderHistoryWhole {
            get {
                return m_folderHistoryWhole;
            }
            set {
                m_folderHistoryWhole = value;
            }
        }

        //=========================================================================================
        // ファイルビューアの比較バッファ
        //=========================================================================================
        public FileViewerSelectionCompareBuffer FileViewerSelectionCompareBuffer {
            get {
                return m_fileViewerSelectionCompareBuffer;
            }
        }

        //=========================================================================================
        // プロパティ：スライドショーでのマーク結果（null:マーク情報がない）
        //=========================================================================================
        public SlideShowMarkResult SlideShowMarkResult {
            get {
                return m_slideShowMarkResult;
            }
            set {
                m_slideShowMarkResult = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルシステムのファクトリ
        //=========================================================================================
        public FileSystemFactory FileSystemFactory {
            get {
                return m_fileSystemFactory;
            }
        }

        //=========================================================================================
        // プロパティ：圧縮／展開のファクトリ
        //=========================================================================================
        public ArchiveFactory ArchiveFactory {
            get {
                return m_archiveFactory;
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスク
        //=========================================================================================
        public BackgroundTaskManager BackgroundTaskManager {
            get {
                return m_backgroundTaskList;
            }
        }

        //=========================================================================================
        // プロパティ：テンポラリファイルの管理クラス
        //=========================================================================================
        public TemporaryManager TemporaryManager {
            get {
                return m_temporaryManager;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンの管理クラス
        //=========================================================================================
        public FileIconManager FileIconManager {
            get {
                return m_fileIconManager;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルバックグラウンドクローラ
        //=========================================================================================
        public FileCrawlThread FileCrawlThread {
            get {
                return m_fileCrawlThread;
            }
        }
        
        //=========================================================================================
        // プロパティ：バックグラウンドでリクエストを処理するためのクラス
        //=========================================================================================
        public UIRequestBackgroundThread UIRequestBackgroundThread {
            get {
                return m_uiRequestBackgroundThread;
            }
        }

        //=========================================================================================
        // プロパティ：表示中のUIファイル一覧の変更監視クラス
        //=========================================================================================
        public UIFileListWatcher UIFileListWatcher {
            get {
                return m_uiFileListWatcher;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアのフィルタースレッド
        //=========================================================================================
        public FilterThread GraphicsViewerFilterThread {
            get {
                if (m_graphicsViewerFilterThread == null) {
                    m_graphicsViewerFilterThread = new FilterThread(OSUtils.GetCpuCoreCount());
                }
                return m_graphicsViewerFilterThread;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルフィルターの管理クラス
        //=========================================================================================
        public FileFilterManager FileFilterManager {
            get {
                return m_fileFilterManager;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルの転送条件
        //=========================================================================================
        public FileConditionSetting FileConditionSetting {
            get {
                return m_fileConditionSetting;
            }
            set {
                m_fileConditionSetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：フォルダサイズの取得結果（保持していないときはnull）
        //=========================================================================================
        public RetrieveFolderSizeResult RetrieveFolderSizeResult {
            get {
                return m_retrieveFolderSizeResult;
            }
            set {
                m_retrieveFolderSizeResult = value;
            }
        }
    }
}