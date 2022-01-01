using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.FileViewer;
using ShellFiler.FileTask;
using ShellFiler.GraphicsViewer;
using ShellFiler.Locale;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.Document.Setting;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：コンフィグ設定
    //=========================================================================================
    public class Configuration : ICloneable {
        // 現在使用中の設定
        private static Configuration s_current = null;

        // Windowsのデフォルトファイル名
        private static string s_windowsDefaultFontName;

        // 最後に開いたオプション設定のページ（null:情報なし）
        private static string s_optionSettingPageLast = null;

        // 作業ディレクトリ（アプリケーションのルート、最後はセパレータ、自動設定時は""）
        private static string s_temporaryDirectory = "";

        // 生成できる待機可能バックグラウンドタスクの最大数
        private static int s_maxBackgroundTaskWaitableCount = 4;

        // 生成できる待機不可能バックグラウンドタスクの最大数
        private static int s_maxBackgroundTaskLimitedCount = 10;

        // コマンドヒストリの最大記憶件数
        private static int s_commandHistoryMaxCount = 30;

        // パスヒストリの最大記憶件数
        private static int s_pathHistoryMaxCount = 30;

        // パスヒストリの最大記憶件数（全体）
        private static int s_pathHistoryWholeMaxCount = 100;

        // ファイルビューア検索履歴の最大記憶件数
        private static int s_viewerSearchHistoryMaxCount = 30;

        // ログウィンドウで記憶する最大行数
        private static int s_logLineMaxCount = 1000;

        // ファイルの書き込み日時
        private DateTime m_lastFileWriteTime = DateTime.MinValue;

        // 無料版のときtrue
        private bool m_freewareVersion = true;

        //*****************************************************************************************
        // インストール情報＞全般
        //*****************************************************************************************
        // 作業ディレクトリ（自動設定時は""、次回起動後に有効）
        private string m_temporaryDirectoryDefault = "";

        // エディタのコマンドライン（{0}にファイル名、.txtの関連付け使用時は""）
        private string m_textEditorCommandLine = "";

        // SSH用エディタのコマンドライン（{0}にファイル名、通常ファイルと共通の場合は""）
        private string m_textEditorCommandLineSSH = "";

        // 行番号指定のエディタのコマンドライン（{0}にファイル名、{1}に行番号、TextEditorCommandLine使用時は""）
        private string m_textEditorCommandLineWithLineNumber = "";

        // SSH用行番号指定のエディタのコマンドライン（{0}にファイル名、{1}に行番号、通常ファイルと共通の場合は""）
        private string m_textEditorCommandLineWithLineNumberSSH = "";

        // 差分表示ツールのコマンドライン（{0}にファイル名、未定義のとき""）
        private string m_diffCommandLine = "";
        
        // ディレクト利用差分表示ツールのコマンドライン（{0}にファイル名、未定義のとき""）
        private string m_diffDirectoryCommandLine = "";

        //*****************************************************************************************
        // ファイル一覧＞全般
        //*****************************************************************************************
        // ファイル更新後、自動反映するまでの時間[ms]（0:チェックしない）
        private int m_autoDirectoryUpdateWait = 2000;

        // タブ切り替え時にファイル一覧を自動更新するときtrue
        private bool m_refreshFileListTabChange = true;

        // SSHでもタブ切り替え時にファイル一覧を自動更新するときtrue
        private bool m_refreshFileListTabChangeSSH = true;

        //*****************************************************************************************
        // ファイル一覧＞起動時の状態
        //*****************************************************************************************
        // 左ウィンドウの初期ディレクトリ（"":前回値を使う）
        private string m_initialDirectoryLeft = "";

        // 右ウィンドウの初期ディレクトリ（"":前回値を使う）
        private string m_initialDirectoryRight = "";

        // メインウィンドウのデフォルトサイズ（Empty:前回値を使う）
        private Rectangle m_mainWindowRectDefault = Rectangle.Empty;

        // スプラッシュウィンドウでの待ち時間[ms]
        private int m_splashWindowWait = 1000;

        //*****************************************************************************************
        // ファイル一覧＞起動時の一覧
        //*****************************************************************************************
        // 左ウィンドウのデフォルトソート方法（null:前回値を使う）
        private FileListSortMode m_defaultFileListSortModeLeft = null;

        // 右ウィンドウのデフォルトソート方法（null:前回値を使う）
        private FileListSortMode m_defaultFileListSortModeRight = null;

        //*****************************************************************************************
        // ファイル一覧＞フォルダサイズの取得
        //*****************************************************************************************
        // フォルダサイズの取得方法のデフォルト（null:前回値を使う）
        private RetrieveFolderSizeCondition m_retrieveFolderSizeCondition = null;

        // プロパティ：フォルダサイズの取得後、保持する階層
        private int m_retrieveFolderSizeKeepLowerDepth = 3;

        // プロパティ：フォルダサイズの取得後、保持するフォルダ数
        private int m_retrieveFolderSizeKeepLowerCount = 3000;

        //*****************************************************************************************
        // ファイル一覧＞動作
        //*****************************************************************************************
        // ファイル一覧でのスクロールマージンの行数
        private int m_listViewScrollMarginLineCount = 2;

        // マウスホイールが回転したときの最大移動行数
        private int m_mouseWheelMaxLines = 10;

        // ファイル一覧でドラッグ中に最大速度で移動できる行数
        private int m_fileListDragMaxSpeed = 5;

        // ファイル一覧で最後の拡張子を離して表示するときtrue
        private bool m_fileListSeparateExt = false;

        // 逆向き←→で親フォルダに戻るときtrue
        private bool m_chdirParentOtherSideMove = false;

        // ドラッグ中にウィンドウ外に移動した場合にウィンドウを隠すときtrue
        private bool m_hideWindowDragDrop = true;

        // フォルダ変更時にカーソル位置のレジュームを行うときtrue
        private bool m_resumeFolderCursorFile = true;
        
        //*****************************************************************************************
        // ファイル一覧＞起動時の表示モード
        //*****************************************************************************************
        // ファイル一覧左画面の表示モード（前回の状態に従うときnull）
        private FileListViewMode m_defaultViewModeLeft = null;

        // ファイル一覧右画面の表示モード（前回の状態に従うときnull）
        private FileListViewMode m_defaultViewModeRight = null;

        //*****************************************************************************************
        // ファイル一覧＞表示モード
        //*****************************************************************************************
        // フォルダ切り替え時のモード（直前の状態に従うときnull）
        private FileListViewMode m_fileListViewChangeMode = null;

        // フォルダごとの自動切り替えの設定
        private FileListViewModeAutoSetting m_fileListViewModeAutoSetting = new FileListViewModeAutoSetting();

        //*****************************************************************************************
        // ファイル操作＞全般
        //*****************************************************************************************
        // 生成できる待機可能バックグラウンドタスクの最大数のコンフィグ値（次回起動後に有効）
        private int m_maxBackgroundTaskWaitableCountDefault = 4;

        // 生成できる待機不可能バックグラウンドタスクの最大数のコンフィグ値（次回起動後に有効）
        private int m_maxBackgroundTaskLimitedCountDefault = 10;

        //*****************************************************************************************
        // ファイル操作＞転送と削除
        //*****************************************************************************************
        // 同名ファイルのオプションのデフォルト（null:前回値を使用する）
        private SameFileOption m_sameFileOptionDefault = null;

        // ディレクトリ削除のオプションのデフォルト（null:前回値を使用する）
        private DeleteFileOption m_deleteFileOptionDefault = new DeleteFileOption();

        //*****************************************************************************************
        // ファイル操作＞属性のコピー
        //*****************************************************************************************
        // 属性のコピーの方法
        private AttributeSetMode m_transferAttributeSetMode = new AttributeSetMode();

        //*****************************************************************************************
        // ファイル操作＞マークなし操作
        //*****************************************************************************************
        // マークなしコピーを許可するときtrue
        private bool m_marklessCopy = false;

        // マークなし移動を許可するときtrue
        private bool m_marklessMove = false;

        // マークなし削除を許可するときtrue
        private bool m_marklessDelete = false;

        // マークなしショートカットの作成を許可するときtrue
        private bool m_marklessShortcut = false;

        // マークなしファイル属性の一括編集を許可するときtrue
        private bool m_marklessAttribute = false;

        // マークなし圧縮を許可するときtrue
        private bool m_marklessPack = true;

        // マークなし展開を許可するときtrue
        private bool m_marklessUnpack = true;

        // マークなし編集を許可するときtrue
        private bool m_marklessEdit = true;

        // マークなし編集を許可するときtrue
        private bool m_marklessFodlerSize = true;

        //*****************************************************************************************
        // ファイル操作＞クリップボード
        //*****************************************************************************************
        // ファイル名を指定してコピーの方法のデフォルト（null:前回値を使用する）
        private ClipboardCopyNameAsSetting m_clipboardCopyNameAsSettingDefault = new ClipboardCopyNameAsSetting();

        //*****************************************************************************************
        // ファイル操作＞一覧の比較
        //*****************************************************************************************
        // ファイル一覧の比較方法のデフォルト（null:前回値を使用する）
        private FileCompareSetting m_fileCompareSettingDefault = new FileCompareSetting();

        //*****************************************************************************************
        // ファイル操作＞圧縮
        //*****************************************************************************************
        // 圧縮オプションのデフォルト（null:前回値を使用する）
        private ArchiveSetting m_archiveSettingDefault = new ArchiveSetting();

        //*****************************************************************************************
        // ファイル操作＞展開
        //*****************************************************************************************
        // 展開先パスのモード
        private ExtractPathMode m_archiveExtractPathMode = ExtractPathMode.NewDirectoryIsSameExist;
        
        //*****************************************************************************************
        // ファイル操作＞各種操作
        //*****************************************************************************************
        // インクリメンタルサーチで文字列の先頭から検索するときtrueのデフォルト（null:前回値を使う）
        private BooleanFlag m_incrementalSearchFromHeadDefault = new BooleanFlag(true);

        // 新規ディレクトリに自動的にカレントを移動してよいときtrue（null:前回値を使う）
        private BooleanFlag m_makeDirectoryMoveCurrentDefault = new BooleanFlag(true);

        // 新規ディレクトリ名(Windows)
        private string m_makeDirectoryNewWindowsName = Resources.DlgMakeDir_DefaultDirNameWindows;

        // 新規ディレクトリ名(SSH)
        private string m_makeDirectoryNewSSHName = Resources.DlgMakeDir_DefaultDirNameSSH;

        // Windowsコマンドの実行で標準出力の結果をログに中継する方法のデフォルト（null:前回値を使う）
        private ShellExecuteRelayMode m_shellExecuteReplayModeWindowsDefault = ShellExecuteRelayMode.None;

        // SSHコマンドの実行で標準出力の結果をログに中継する方法のデフォルト（null:前回値を使う）
        private ShellExecuteRelayMode m_shellExecuteReplayModeSSHDefault = ShellExecuteRelayMode.RelayLogWindow;

        // ミラーコピーを除外するファイル（「:」区切り）
        private string m_mirrorCopyExceptFiles = "*.bak:*.$$$:*.idb:*.pch:*.pdb:*.obj:*.ilk:*.res:*.tlb:*.sbr:*.bsc:*.ilk";

        // ファイル結合でのデフォルトファイル名の種類
        private CombineDefaultFileType m_combineDefaultFileType = CombineDefaultFileType.Specified;

        // ファイル結合でのデフォルトファイル名
        private string m_combineDefaultFileName = Resources.DlgCombine_DefaultFileName;

        //*****************************************************************************************
        // SSH＞全般
        //*****************************************************************************************
        // SSHショートカットの作成方法のデフォルト（null:前回値を使う）
        private ShortcutType m_sshShortcutTypeDefault = ShortcutType.SymbolicLink;

        // デフォルトの接続方式
        private SSHProtocolType m_sshFileSystemDefault = SSHProtocolType.SFTP;

        //*****************************************************************************************
        // SSH＞ターミナル
        //*****************************************************************************************
        // SSHターミナルのバックログ行数
        private int m_terminalLogLineCount = 1000;

        // シェル起動でSSHフォルダからはSSHターミナルを起動するときtrue
        private bool m_terminalShellCommandSSH = false;

        // ターミナルを閉じたときの動作
        private TerminalCloseConfirmMode m_terminalCloseConfirmMode = TerminalCloseConfirmMode.ShellClose;

        //*****************************************************************************************
        // SSH＞ターミナルログ
        //*****************************************************************************************
        // ターミナルログの出力方法
        private TerminalLogType m_terminalLogType = TerminalLogType.None;

        // ターミナルログの1ファイルあたりの最大サイズ[KB]
        private int m_terminalLogMaxSize = 10 * 1024;

        // ターミナルログの最大ファイル数
        private int m_terminalLogFileCount = 50;

        // ターミナルログの出力フォルダ（nullのときデフォルト）
        private string m_terminalLogOutputFolder = null;

        //*****************************************************************************************
        // プライバシー＞全般
        //*****************************************************************************************
        // コマンドヒストリの最大記憶件数のデフォルト（次回起動時に有効）
        private int m_commandHistoryMaxCountDefault = 30;

        // コマンドヒストリをディスクに保存するときtrue
        private bool m_commandHistorySaveDisk = true;

        // パスヒストリの最大記憶件数のデフォルト（次回起動時に有効）
        private int m_pathHistoryMaxCountDefault = 30;

        // パスヒストリ（全体）の最大記憶件数のデフォルト（次回起動時に有効）
        private int m_pathHistoryWholeMaxCountDefault = 100;

        // パスヒストリ（全体）の履歴をディスクに保存するときtrue
        private bool m_pathHistoryWholeSaveDisk = true;

        // ファイルビューア検索履歴の最大記憶件数のデフォルト（次回起動時に有効）
        private int m_viewerSearchHistoryMaxCountDefault = 30;

        // ファイルビューア検索履歴をディスクに保存するときtrue
        private bool m_viewerSearchHistorySaveDisk = true;

        //*****************************************************************************************
        // テキストビューア＞全般
        //*****************************************************************************************
        // テキストビューア 最大読み込みサイズ
        private int m_textViewerMaxFileSize = 10 * 1024 * 1024;

        // テキストビューアで表示する最大行数
        private int m_textViewerMaxLineCount = 99999;

        // テキストの比較用バッファを差分表示ツール起動後に削除するときtrueのデフォルト（null:前回値を使う）
        private BooleanFlag m_textViewerClearCompareBufferDefault = new BooleanFlag(true);

        //*****************************************************************************************
        // テキストビューア＞表示
        //*****************************************************************************************
        // テキストビューアで行番号を表示するときtrue
        private bool m_textViewerIsDisplayLineNumber = true;

        // テキストビューアで制御文字を表示するときtrue
        private bool m_textViewerIsDisplayCtrlChar = true;

        //*****************************************************************************************
        // テキストビューア＞折返しとタブ
        //*****************************************************************************************
        // テキストビューアでの折り返し設定のデフォルト（null:前回値を使う）
        private TextViewerLineBreakSetting m_textViewerLineBreakDefault = null;

        // テキストビューアでタブ幅4とみなすファイルの拡張子
        private string m_textViewerTab4Extension = "c cpp h java cs";

        //*****************************************************************************************
        // テキストビューア＞検索オプション
        //*****************************************************************************************
        // テキストビューアでの検索オプションのデフォルト（null:前回値を使う）
        private TextSearchOption m_textSearchOptionDefault = new TextSearchOption();

        //*****************************************************************************************
        // テキストビューア＞クリップボード
        //*****************************************************************************************
        // テキストビューアでのクリップボードコピー形式のデフォルト（null:前回値を使う）
        private TextClipboardSetting m_textClipboardSettingDefault = null;

        // ダンプビューアでのクリップボードコピー形式のデフォルト（null:前回値を使う）
        private DumpClipboardSetting m_dumpClipboardSettingDefault = null;

        //*****************************************************************************************
        // グラフィックビューア＞全般
        //*****************************************************************************************
        // 読み込むファイルの最大サイズ
        private int m_graphicsViewerMaxFileSize = 100 * 1024 * 1024;

        // ドラッグ中に慣性を使用するかどうか
        private DragInertiaType m_graphicsViewerDragInertia = DragInertiaType.LocalOnly;

        // ドラッグ中に慣性のブレーキのきき具合
        private int m_graphicsViewerDragBreaking = 80;

        // フィルターの使用方法
        private GraphicsViewerFilterMode m_graphicsViewerFilterMode = GraphicsViewerFilterMode.CurrentWindowImages;

        // グラフィックビューアで全画面表示のときに非表示にするまでの時間[ms]
        private int m_graphicsViewerFullScreenHideTimer = 3000;

        // グラフィックビューアの全画面表示で、自動的にマウスカーソルを消すときtrue
        private bool m_graphicsViewerFullScreenAutoHideCursor = true;

        // グラフィックビューアの全画面表示で、自動的にファイル情報を消すときtrue
        private bool m_graphicsViewerFullScreenAutoHideInfo = true;

        // グラフィックビューアの全画面表示で、常にファイル情報を消すときtrue
        private bool m_graphicsViewerFullScreenHideInfoAlways = false;

        //*****************************************************************************************
        // グラフィックビューア＞拡大表示
        //*****************************************************************************************
        // グラフィックビューアの画像の拡大方法
        private GraphicsViewerAutoZoomMode m_graphicsViewerAutoZoomMode = GraphicsViewerAutoZoomMode.AutoZoom;

        // 画面より画像が小さいときも画面サイズに合わせるときtrue
        private bool m_graphicsViewerZoomInLarger = false;

        //*****************************************************************************************
        // ファンクションキー＞全般
        //*****************************************************************************************
        // ファンクションキーの区切り数（4, 5, 0:なし）
        private int m_functionBarSplitCount = 4;

        // オーバーレイアイコンでキー名を表示するときtrue
        private bool m_functionBarUseOverrayIcon = false;

        //*****************************************************************************************
        // ログ＞全般
        //*****************************************************************************************
        // ログウィンドウで記憶する最大行数（次回起動時に有効）
        private int m_logLineMaxCountDefault = 1000;

        //*****************************************************************************************
        // 色とフォント＞ファイル一覧
        //*****************************************************************************************
        // ファイル一覧 背景色
        private Color m_fileListBackColor = Color.FromArgb(255, 255, 255);

        // ファイル一覧 通常のファイルの文字色
        private Color m_fileListFileTextColor = Color.FromArgb(0, 0, 0);

        // ファイル一覧 読み込み専用ファイルの文字色
        private Color m_fileListReadOnlyColor = Color.FromArgb(0, 150, 0);

        // ファイル一覧 隠しファイルの文字色
        private Color m_fileListHiddenColor = Color.FromArgb(0, 50, 255);

        // ファイル一覧 システムファイルの文字色
        private Color m_fileListSystemColor = Color.FromArgb(192, 0, 192);

        // ファイル一覧 アーカイブファイルの文字色
        private Color m_fileListArchiveColor = Color.FromArgb(0, 0, 0);

        // ファイル一覧 シンボリックリンクファイルの文字色
        private Color m_fileListSymlinkColor = Color.FromArgb(128, 128, 0);

        // ファイル一覧 ファイル名グレーアウトの文字色
        private Color m_fileListGrayColor = Color.FromArgb(128, 128, 128);

        // ファイル一覧 マーク中の文字色（Color.Emptyのときは通常色）
        private Color m_fileListMarkColor = Color.Empty;        // Color.FromArgb(255, 255, 255);

        // ファイル一覧 マーク中の背景色1
        private Color m_fileListMarkBackColor1 = Color.FromArgb(237, 243, 255);

        // ファイル一覧 マーク中の背景色2
        private Color m_fileListMarkBackColor2 = Color.FromArgb(192, 210, 255);

        // ファイル一覧 マーク中の背景枠の描画色
        private Color m_fileListMarkBackBorderColor = Color.FromArgb(135, 161, 188);

        // ファイル一覧 ファイル名グレーアウトの文字色
        private Color m_fileListGrayBackColor = Color.FromArgb(255, 255, 255);

        // ファイル一覧 マーク中グレーアウトの文字色
        private Color m_fileListMarkGrayColor = Color.FromArgb(128, 128, 128);      // Color.FromArgb(255, 255, 255);

        // ファイル一覧 マーク中のグレーアウト背景色1
        private Color m_fileListMarkGrayBackColor1 = Color.FromArgb(250, 250, 250);

        // ファイル一覧 マーク中のグレーアウト背景色2
        private Color m_fileListMarkGrayBackColor2 = Color.FromArgb(200, 200, 200);

        // ファイル一覧 マーク中のグレーアウト枠の描画色
        private Color m_fileListMarkGrayBackBorderColor = Color.FromArgb(180, 180, 180);

        // ファイル一覧 カーソルの色
        private Color m_fileListCursorColor = Color.FromArgb(192, 0, 192);

        // ファイル一覧 無効状態でのカーソルの色
        private Color m_fileListCursorDisableColor = Color.FromArgb(128, 128, 128);

        // ファイル一覧 SSHスーパーユーザーのステータスバー背景色
        private Color m_fileListStatusBarSuperUserColor = Color.FromArgb(192, 192, 64);

        // ファイル一覧 サムネイルの枠の色1
        private Color m_fileListThumbnailFrameColor1 = Color.FromArgb(240, 240, 240);

        // ファイル一覧 サムネイルの枠の色2
        private Color m_fileListThumbnailFrameColor2 = Color.FromArgb(192, 192, 192);

        // ダイアログ中でエラーが発生したときのステータス背景色
        private Color m_dialogErrorBackColor = Color.FromArgb(128, 0, 0);

        // ダイアログ中でエラーが発生したときのステータス文字色
        private Color m_dialogErrorTextColor = Color.FromArgb(255, 255, 255);

        // ダイアログ中で警告が発生したときのステータス背景色
        private Color m_dialogWarningBackColor = Color.FromArgb(255, 255, 128);

        // ダイアログ中で警告が発生したときのステータス文字色
        private Color m_dialogWarningTextColor = Color.FromArgb(0, 0, 0);

        //*****************************************************************************************
        // 色とフォント＞テキストビューア
        //*****************************************************************************************
        // テキストビューア エラー時の背景色
        private Color m_textViewerErrorBackColor = Color.FromArgb(240, 240, 240);

        // テキストビューア エラー時のステータスバー背景色
        private Color m_textViewerErrorStatusBackColor = Color.FromArgb(128, 0, 0);

        // テキストビューア エラー時のステータスバー文字色
        private Color m_textViewerErrorStatusTextColor = Color.FromArgb(255, 255, 255);

        // テキストビューア 情報表示のステータスバー背景色
        private Color m_textViewerInfoStatusBackColor = Color.FromArgb(255, 255, 128);

        // テキストビューア 情報表示のステータスバー文字色
        private Color m_textViewerInfoStatusTextColor = Color.FromArgb(0, 0, 0);

        // テキストビューア 行番号の色
        private Color m_textViewerLineNoTextColor = Color.FromArgb(0, 0, 255);

        // テキストビューア 行番号の背景色（左）
        private Color m_textViewerLineNoBackColor1 = Color.FromArgb(240, 245, 255);

        // テキストビューア 行番号の背景色（右）
        private Color m_textViewerLineNoBackColor2 = Color.FromArgb(226, 238, 249);

        // テキストビューア 行番号境界の色
        private Color m_textViewerLineNoSeparatorColor = Color.FromArgb(64, 64, 192);

        // テキストビューア 領域外の色
        private Color m_textViewerOutOfAreaBackColor = Color.FromArgb(245, 245, 245);

        // テキストビューア 領域外分離線の色
        private Color m_textViewerOutOfAreaSeparatorColor = Color.FromArgb(220, 220, 220);

        // テキストビューア 検索中カーソルの色
        private Color m_textViewerSearchCursorColor = Color.FromArgb(192, 50, 192);

        // テキストビューア 制御関連の色
        private Color m_textViewerControlColor = Color.FromArgb(192, 50, 192);

        // テキストビューア テキストの色
        private Color m_textViewerTextColor = Color.FromArgb(0, 0, 0);

        // テキストビューア 選択中テキストの色
        private Color m_textViewerSelectTextColor = Color.FromArgb(255, 255, 255);

        // テキストビューア 選択中テキストの色（ダンプの参考範囲）
        private Color m_textViewerSelectTextColor2 = Color.FromArgb(0, 0, 0);

        // テキストビューア 選択中背景の色
        private Color m_textViewerSelectBackColor = Color.FromArgb(0, 0, 128);

        // テキストビューア 選択中背景の色２
        private Color m_textViewerSelectBackColor2 = Color.FromArgb(144, 144, 220);

        // テキストビューア 検索ヒット背景の色
        private Color m_textViewerSearchHitBackColor = Color.FromArgb(255, 0, 0);

        // テキストビューア 検索ヒットテキストの色
        private Color m_textViewerSearchHitTextColor = Color.FromArgb(255, 255, 255);

        // テキストビューア 自動検索ヒットテキストの色
        private Color m_textViewerSearchAutoTextColor = Color.FromArgb(0, 192, 64);

        // レーダーバーの背景色１
        private Color m_radarBarBackColor1 = Color.FromArgb(144, 144, 144);

        // レーダーバーの背景色２
        private Color m_radarBarBackColor2 = Color.FromArgb(192, 192, 192);

        //*****************************************************************************************
        // 色とフォント＞グラフィックビューア
        //*****************************************************************************************
        // グラフィックビューア 背景色
        private Color m_graphicsViewerBackColor = Color.FromArgb(0, 0, 0);

        // グラフィックビューア テキスト表示の色
        private Color m_graphicsViewerTextColor = Color.FromArgb(255, 255, 255);

        // グラフィックビューア テキスト表示影のブラシ
        private Color m_graphicsViewerTextShadowColor = Color.FromArgb(64, 64, 64);

        // グラフィックビューア 読み込み中テキスト表示の色
        private Color m_graphicsViewerLoadingTextColor = Color.FromArgb(32, 64, 192);

        // グラフィックビューア 読み込み中テキスト表示影のブラシ
        private Color m_graphicsViewerLoadingTextShadowColor = Color.FromArgb(8, 16, 48);

        //*****************************************************************************************
        // 色とフォント＞ログ
        //*****************************************************************************************
        // プロパティ：ログウィンドウテキスト色
        private Color m_logWindowTextColor = Color.FromArgb(0, 0, 0);

        // プロパティ：ログウィンドウリンクテキスト色
        private Color m_logWindowLinkTextColor = Color.FromArgb(0, 0, 255);

        // プロパティ：ログウィンドウエラー色
        private Color m_logErrorTextColor = Color.FromArgb(255, 0, 0);

        // プロパティ：ログウィンドウ標準エラー色
        private Color m_logStdErrorTextColor = Color.FromArgb(0, 0, 255);

        // プロパティ：ログウィンドウ選択中のテキスト色
        private Color m_logWindowSelectTextColor = Color.FromArgb(255, 255, 255);

        // プロパティ：ログウィンドウ背景色
        private Color m_logWindowBackColor = Color.FromArgb(255, 255, 255);

        // プロパティ：ログウィンドウ選択中の背景色
        private Color m_logWindowSelectBackColor = Color.FromArgb(0, 0, 128);

        // プロパティ：ログウィンドウ背景色（ビジュアルベル用）
        private Color m_logWindowBackBellColor = Color.FromArgb(220, 240, 255);

        // プロパティ：ログウィンドウ背景色（セッション切断済み用）
        private Color m_logWindowBackClosedColor = Color.FromArgb(220, 220, 220);

        // プロパティ：ログウィンドウ進捗表示バーの色１
        private Color m_logWindowProgressColor1 = Color.FromArgb(0, 255, 0);

        // プロパティ：ログウィンドウ進捗表示バーの色２
        private Color m_logWindowProgressColor2 = Color.FromArgb(255, 255, 255);

        // プロパティ：ログウィンドウ進捗表示バーの色３
        private Color m_logWindowProgressColor3 = Color.FromArgb(48, 48, 48);

        // プロパティ：ログウィンドウ進捗表示バーの色４
        private Color m_logWindowProgressColor4 = Color.FromArgb(92, 128, 92);

        // プロパティ：ログウィンドウ 残り時間描画用のフォントの色（明）
        private Color m_logWindowRemainingTimeTextColor1 = Color.FromArgb(255, 255, 255);

        // プロパティ：ログウィンドウ 残り時間描画用のフォントの色（暗）
        private Color m_logWindowRemainingTimeTextColor2 = Color.FromArgb(0, 64, 0);

        //*****************************************************************************************
        // 色とフォント＞フォント
        //*****************************************************************************************
        // ファイル一覧表示用のフォント名（"":OSによる自動設定）
        private string m_listViewFontName = "";

        // ファイル一覧表示用のフォントサイズ
        private float m_listViewFontSize = 9.0f;

        // デフォルトファイル一覧 文字の高さ
        private int m_defaultFileListViewHeight = 18;

        // サムネイルファイル一覧表示用のフォント名（"":OSによる自動設定）
        private string m_thumbFileListViewFontName = "";

        // サムネイルファイル一覧表示用のフォントサイズ
        private float m_thumbFileListViewFontSize = 9.0f;

        // サムネイルファイル一覧表示用のフォントサイズ（小）
        private float m_thumbFileListViewSmallFontSize = 7.0f;

        // テキストビューア 描画用のフォント名
        private string m_textFontName = "ＭＳ ゴシック";

        // テキストビューア 描画用のフォントの大きさ
        private float m_textFontSize = 10.0f;

        // テキストビューアでの行の高さ
        private int m_textFileViewerLineHeight = 16;

        // ファンクションバー 描画用のフォント名
        private string m_functionBarFontName = "ＭＳ Ｐゴシック";

        // ファンクションバー 描画用のフォントの大きさ
        private float m_functionBarFontSize = 8.0f;

        // ログウィンドウ表示用のフォント名
        private string m_logWindowFontName = "";

        // ログウィンドウ表示用のフォント名
        private string m_logWindowFixedFontName = "ＭＳ ゴシック";

        // ログウィンドウ表示用のフォントサイズ
        private float m_logWindowFontSize = 9.0f;

        // プロパティ：ログウィンドウ表示用のフォントサイズ
        private float m_logWindowTerminalFontSize = 11.0f;

        //=========================================================================================
        // プロパティ：現在の設定
        //=========================================================================================
        public static Configuration Current {
            get {
                return s_current;
            }
            set {
                s_current = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：最後に開いたオプション設定のページ（null:情報なし）
        //=========================================================================================
        public static string OptionSettingPageLast {
            get {
                return s_optionSettingPageLast;
            }
            set {
                s_optionSettingPageLast = value;
            }
        }
                
        //=========================================================================================
        // プロパティ：作業ディレクトリ（アプリケーションのルート、最後はセパレータ、自動設定時は""）
        //=========================================================================================
        public static string TemporaryDirectory {
            get {
                return s_temporaryDirectory;
            }
        }

        //=========================================================================================
        // プロパティ：生成できる待機可能バックグラウンドタスクの最大数
        //=========================================================================================
        public static int MaxBackgroundTaskWaitableCount {
            get {
                return s_maxBackgroundTaskWaitableCount;
            }
        }

        //=========================================================================================
        // プロパティ：生成できる待機不可能バックグラウンドタスクの最大数
        //=========================================================================================
        public static int MaxBackgroundTaskLimitedCount {
            get {
                return s_maxBackgroundTaskLimitedCount;
            }
        }
        
        //=========================================================================================
        // プロパティ：コマンドヒストリの最大記憶件数
        //=========================================================================================
        public static int CommandHistoryMaxCount {
            get {
                return s_commandHistoryMaxCount;
            }
        }

        //=========================================================================================
        // プロパティ：パスヒストリの最大記憶件数
        //=========================================================================================
        public static int PathHistoryMaxCount {
            get {
                return s_pathHistoryMaxCount;
            }
        }

        //=========================================================================================
        // プロパティ：パスヒストリの最大記憶件数（全体）
        //=========================================================================================
        public static int PathHistoryWholeMaxCount {
            get {
                return s_pathHistoryWholeMaxCount;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルビューア検索履歴の最大記憶件数
        //=========================================================================================
        public static int ViewerSearchHistoryMaxCount {
            get {
                return s_viewerSearchHistoryMaxCount;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウで記憶する最大行数
        //=========================================================================================
        public int LogLineMaxCount {
            get {
                return s_logLineMaxCount;
            }
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static void Initialize() {
            s_current = new Configuration();
            s_current.LoadSetting();
            s_windowsDefaultFontName = UILocale.WindowsDefaultFontName;

            s_temporaryDirectory = s_current.m_temporaryDirectoryDefault;
            s_maxBackgroundTaskWaitableCount = s_current.m_maxBackgroundTaskWaitableCountDefault;
            s_maxBackgroundTaskLimitedCount = s_current.m_maxBackgroundTaskLimitedCountDefault;
            s_commandHistoryMaxCount = s_current.m_commandHistoryMaxCountDefault;
            s_viewerSearchHistoryMaxCount = s_current.m_viewerSearchHistoryMaxCountDefault;
            s_pathHistoryMaxCount = s_current.m_pathHistoryMaxCountDefault;
            s_pathHistoryWholeMaxCount = s_current.m_pathHistoryWholeMaxCountDefault;
            s_logLineMaxCount = s_current.m_logLineMaxCountDefault;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public Configuration() {
        }

        //=========================================================================================
        // 機　能：設定の読み込みを行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void LoadSetting() {
#if !FREE_VERSION
            m_lastFileWriteTime = DateTime.MinValue;
            string fileName = DirectoryManager.ConfigurationSetting;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = LoadSettingInternal(loader, this);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSetting, fileName, loader.ErrorDetail);
            }
#endif
        }
        
        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadSettingInternal(SettingLoader loader, Configuration obj) {
            // ここに来たら有料版
            m_freewareVersion = false;

            // ファイルがないときはそのまま
            if (!File.Exists(loader.FileName)) {
                return true;
            }

            // ファイルから読み込む
            bool success;
            success = loader.LoadSetting(false);
            if (!success) {
                return false;
            }

            Configuration defValue = new Configuration();

            // タグを読み込む
            string strValue;
            int intValue;
            Rectangle rectValue;
            success = loader.ExpectTag(SettingTag.Config_Configuration, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.Config_Configuration) {
                    break;

                // インストール情報＞全般
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_TemporaryDirectoryDefault) {
                    strValue = loader.StringValue;
                    if (CheckTemporaryDirectoryDefault(ref strValue, null)) {
                        obj.m_temporaryDirectoryDefault = strValue;
                    } else {
                        obj.m_temporaryDirectoryDefault = defValue.m_temporaryDirectoryDefault;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_TextEditorCommandLine) {
                    strValue = loader.StringValue;
                    if (CheckTextEditorCommandLine(ref strValue, null)) {
                        obj.m_textEditorCommandLine = strValue;
                    } else {
                        obj.m_textEditorCommandLine = defValue.m_textEditorCommandLine;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_TextEditorCommandLineSSH) {
                    strValue = loader.StringValue;
                    if (CheckTextEditorCommandLine(ref strValue, null)) {
                        obj.m_textEditorCommandLineSSH = strValue;
                    } else {
                        obj.m_textEditorCommandLineSSH = defValue.m_textEditorCommandLineSSH;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_TextEditorCommandLineWithLineNumber) {
                    strValue = loader.StringValue;
                    if (CheckTextEditorCommandLineWithLineNumber(ref strValue, null)) {
                        obj.m_textEditorCommandLineWithLineNumber = strValue;
                    } else {
                        obj.m_textEditorCommandLineWithLineNumber = defValue.m_textEditorCommandLineWithLineNumber;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_TextEditorCommandLineWithLineNumberSSH) {
                    strValue = loader.StringValue;
                    if (CheckTextEditorCommandLineWithLineNumber(ref strValue, null)) {
                        obj.m_textEditorCommandLineWithLineNumberSSH = strValue;
                    } else {
                        obj.m_textEditorCommandLineWithLineNumberSSH = defValue.m_textEditorCommandLineWithLineNumber;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_DiffCommandLine) {
                    strValue = loader.StringValue;
                    if (CheckDiffCommandLine(ref strValue, null)) {
                        obj.m_diffCommandLine = strValue;
                    } else {
                        obj.m_diffCommandLine = defValue.m_diffCommandLine;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_DiffDirectoryCommandLine) {
                    strValue = loader.StringValue;
                    if (CheckDiffDirectoryCommandLine(ref strValue, null)) {
                        obj.m_diffDirectoryCommandLine = strValue;
                    } else {
                        obj.m_diffDirectoryCommandLine = defValue.m_diffDirectoryCommandLine;
                    }

                // ファイル一覧＞全般
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_AutoDirectoryUpdateWait) {
                    intValue = loader.IntValue;
                    if (CheckAutoDirectoryUpdateWait(ref intValue, null)) {
                        obj.m_autoDirectoryUpdateWait = intValue;
                    } else {
                        obj.m_autoDirectoryUpdateWait = defValue.m_autoDirectoryUpdateWait;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_RefreshFileListTabChange) {
                    obj.m_refreshFileListTabChange = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_RefreshFileListTabChangeSSH) {
                    obj.m_refreshFileListTabChangeSSH = loader.BoolValue;

                // ファイル一覧＞起動時の状態
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_InitialDirectoryLeft) {
                    strValue = loader.StringValue;
                    if (CheckInitialDirectoryLeft(ref strValue, null)) {
                        obj.m_initialDirectoryLeft = strValue;
                    } else {
                        obj.m_initialDirectoryLeft = defValue.m_initialDirectoryLeft;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_InitialDirectoryRight) {
                    strValue = loader.StringValue;
                    if (CheckInitialDirectoryRight(ref strValue, null)) {
                        obj.m_initialDirectoryRight = strValue;
                    } else {
                        obj.m_initialDirectoryRight = defValue.m_initialDirectoryRight;
                    }
                } else if (tagType == SettingTagType.RectangleValue && tagName == SettingTag.Config_MainWindowRectDefault) {
                    rectValue = loader.RectangleValue;
                    if (CheckMainWindowRectDefault(ref rectValue, null)) {
                        obj.m_mainWindowRectDefault = rectValue;
                    } else {
                        obj.m_mainWindowRectDefault = defValue.m_mainWindowRectDefault;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_SplashWindowWait) {
                    obj.m_splashWindowWait = loader.IntValue;
                    ModifySplashWindowWait(ref obj.m_splashWindowWait);

                // ファイル一覧＞起動時の一覧
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_DefaultFileListSortModeLeft) {
                    success = FileListSortMode.LoadSetting(loader, out obj.m_defaultFileListSortModeLeft);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_DefaultFileListSortModeLeft, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_DefaultFileListSortModeRight) {
                    success = FileListSortMode.LoadSetting(loader, out obj.m_defaultFileListSortModeRight);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_DefaultFileListSortModeRight, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }

                // ファイル一覧＞フォルダサイズの取得
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_RetrieveFolderSizeCondition) {
                    success = RetrieveFolderSizeCondition.LoadSetting(loader, out obj.m_retrieveFolderSizeCondition);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_RetrieveFolderSizeCondition, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_RetrieveFolderSizeKeepLowerDepth) {
                    obj.m_retrieveFolderSizeKeepLowerDepth = loader.IntValue;
                    ModifyRetrieveFolderSizeKeepLowerDepth(ref obj.m_retrieveFolderSizeKeepLowerDepth);
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_RetrieveFolderSizeKeepLowerCount) {
                    obj.m_retrieveFolderSizeKeepLowerCount = loader.IntValue;
                    ModifyRetrieveFolderSizeKeepLowerCount(ref obj.m_retrieveFolderSizeKeepLowerCount);

                // ファイル一覧＞動作
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_ListViewScrollMarginLineCount) {
                    intValue = loader.IntValue;
                    if (CheckListViewScrollMarginLineCount(ref intValue, null)) {
                        obj.m_listViewScrollMarginLineCount = intValue;
                    } else {
                        obj.m_listViewScrollMarginLineCount = defValue.m_listViewScrollMarginLineCount;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_MouseWheelMaxLines) {
                    obj.m_mouseWheelMaxLines = loader.IntValue;
                    ModifyMouseWheelMaxLines(ref obj.m_mouseWheelMaxLines);
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_FileListDragMaxSpeed) {
                    obj.m_fileListDragMaxSpeed = loader.IntValue;
                    ModifyFileListDragMaxSpeed(ref obj.m_fileListDragMaxSpeed);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_FileListSeparateExt) {
                    obj.m_fileListSeparateExt = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_ChdirParentOtherSideMove) {
                    obj.m_chdirParentOtherSideMove = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_HideWindowDragDrop) {
                    obj.m_hideWindowDragDrop = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_ResumeFolderCursorFile) {
                    obj.m_resumeFolderCursorFile = loader.BoolValue;

                // ファイル一覧＞起動時の表示モード
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_DefaultViewModeLeft) {
                    success = FileListViewMode.LoadSetting(loader, out obj.m_defaultViewModeLeft);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_DefaultViewModeLeft, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_DefaultViewModeRight) {
                    success = FileListViewMode.LoadSetting(loader, out obj.m_defaultViewModeRight);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_DefaultViewModeRight, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }

                // ファイル一覧＞表示モード
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_FileListViewChangeMode) {
                    success = FileListViewMode.LoadSetting(loader, out obj.m_fileListViewChangeMode);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_FileListViewChangeMode, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_FileListViewModeAutoSetting) {
                    success = FileListViewModeAutoSetting.LoadSetting(loader, out obj.m_fileListViewModeAutoSetting);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_FileListViewModeAutoSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }

                // ファイル操作＞全般
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_MaxBackgroundTaskCountDefault) {
                    intValue = loader.IntValue;
                    if (CheckMaxBackgroundTaskWaitableCountDefault(ref intValue, null)) {
                        obj.m_maxBackgroundTaskWaitableCountDefault = intValue;
                    } else {
                        obj.m_maxBackgroundTaskWaitableCountDefault = defValue.m_maxBackgroundTaskWaitableCountDefault;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_MaxBackgroundTaskLimitedCountDefault) {
                    intValue = loader.IntValue;
                    if (CheckMaxBackgroundTaskLimitedCountDefault(ref intValue, null)) {
                        obj.m_maxBackgroundTaskLimitedCountDefault = intValue;
                    } else {
                        obj.m_maxBackgroundTaskLimitedCountDefault = defValue.m_maxBackgroundTaskLimitedCountDefault;
                    }

                // ファイル操作＞転送と削除
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_SameFileOptionDefault) {
                    success = SameFileOption.LoadSetting(loader, out obj.m_sameFileOptionDefault);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_SameFileOptionDefault, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_DeleteFileOptionDefault) {
                    success = DeleteFileOption.LoadSetting(loader, out obj.m_deleteFileOptionDefault);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_DeleteFileOptionDefault, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }

                // ファイル操作＞属性のコピー
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_TransferAttributeSetMode) {
                    success = AttributeSetMode.LoadSetting(loader, out obj.m_transferAttributeSetMode);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_TransferAttributeSetMode, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }

                // ファイル操作＞マークなし操作
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_MarklessCopy) {
                    obj.m_marklessCopy = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_MarklessMove) {
                    obj.m_marklessMove = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_MarklessDelete) {
                    obj.m_marklessDelete = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_MarklessShortcut) {
                    obj.m_marklessShortcut = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_MarklessAttribute) {
                    obj.m_marklessAttribute = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_MarklessPack) {
                    obj.m_marklessPack = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_MarklessUnpack) {
                    obj.m_marklessUnpack = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_MarklessEdit) {
                    obj.m_marklessEdit = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_MarklessFolderSize) {
                    obj.m_marklessFodlerSize = loader.BoolValue;

                // ファイル操作＞クリップボード
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_ClipboardCopyNameAsSettingDefault) {
                    success = ClipboardCopyNameAsSetting.LoadSetting(loader, out obj.m_clipboardCopyNameAsSettingDefault);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_ClipboardCopyNameAsSettingDefault, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }

                // ファイル操作＞一覧の比較
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_FileCompareSettingDefault) {
                    success = FileCompareSetting.LoadSetting(loader, out obj.m_fileCompareSettingDefault);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_FileCompareSettingDefault, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }

                // ファイル操作＞圧縮
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_ArchiveSettingDefault) {
                    success = ArchiveSetting.LoadSetting(loader, out obj.m_archiveSettingDefault);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_ArchiveSettingDefault, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }

                // ファイル操作＞展開
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_ArchiveExtractPathMode) {
                    strValue = loader.StringValue;
                    obj.m_archiveExtractPathMode = ExtractPathMode.FromSerializedData(strValue);

                // ファイル操作＞各種操作
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_IncrementalSearchFromHeadDefault) {
                    obj.m_incrementalSearchFromHeadDefault = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_MakeDirectoryMoveCurrentDefault) {
                    obj.m_makeDirectoryMoveCurrentDefault = new BooleanFlag(loader.BoolValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_MakeDirectoryNewWindowsName) {
                    strValue = loader.StringValue;
                    if (CheckMakeDirectoryNewWindowsName(ref strValue, null)) {
                        obj.m_makeDirectoryNewWindowsName = strValue;
                    } else {
                        obj.m_makeDirectoryNewWindowsName = defValue.m_makeDirectoryNewWindowsName;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_MakeDirectoryNewSSHName) {
                    strValue = loader.StringValue;
                    if (CheckMakeDirectoryNewSSHName(ref strValue, null)) {
                        obj.m_makeDirectoryNewSSHName = strValue;
                    } else {
                        obj.m_makeDirectoryNewSSHName = defValue.m_makeDirectoryNewSSHName;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_ShellExecuteReplayModeWindowsDefault) {
                    obj.m_shellExecuteReplayModeWindowsDefault = ShellExecuteRelayMode.FromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_ShellExecuteReplayModeSSHDefault) {
                    obj.m_shellExecuteReplayModeSSHDefault = ShellExecuteRelayMode.FromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_MirrorCopyExceptFiles) {
                    obj.m_mirrorCopyExceptFiles = loader.StringValue;

                // SSH＞全般
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_SshShortcutTypeDefault) {
                    obj.m_sshShortcutTypeDefault = ShortcutType.FromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_SshFileSystemDefault) {
                    obj.m_sshFileSystemDefault = SSHProtocolType.FromSerializedData(loader.StringValue);

                // SSH＞ターミナル
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_TerminalLogLineCount) {
                    intValue = loader.IntValue;
                    if (CheckTerminalLogLineCount(ref intValue, null)) {
                        obj.m_terminalLogLineCount = intValue;
                    } else {
                        obj.m_terminalLogLineCount = defValue.m_terminalLogLineCount;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_TerminalShellCommandSSH) {
                    obj.m_terminalShellCommandSSH = loader.BoolValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_TerminalCloseConfirmMode) {
                    obj.m_terminalCloseConfirmMode = TerminalCloseConfirmMode.FromSerializedData(loader.StringValue);

                // SSH＞ターミナルログ
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_TerminalLogType) {
                    obj.m_terminalLogType = TerminalLogType.FromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_TerminalLogMaxSize) {
                    intValue = loader.IntValue;
                    if (CheckTerminalLogMaxSize(ref intValue, null)) {
                        obj.m_terminalLogMaxSize = intValue;
                    } else {
                        obj.m_terminalLogMaxSize = defValue.m_terminalLogMaxSize;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_TerminalLogFileCount) {
                    intValue = loader.IntValue;
                    if (CheckTerminalLogFileCount(ref intValue, null)) {
                        obj.m_terminalLogFileCount = intValue;
                    } else {
                        obj.m_terminalLogFileCount = defValue.m_terminalLogFileCount;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_TerminalLogOutputFolder) {
                    obj.m_terminalLogOutputFolder = loader.StringValue;

                // プライバシー＞全般
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_CommandHistoryMaxCountDefault) {
                    intValue = loader.IntValue;
                    if (CheckCommandHistoryMaxCountDefault(ref intValue, null)) {
                        obj.m_commandHistoryMaxCountDefault = intValue;
                    } else {
                        obj.m_commandHistoryMaxCountDefault = defValue.m_commandHistoryMaxCountDefault;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_CommandHistorySaveDisk) {
                    obj.m_commandHistorySaveDisk = loader.BoolValue;
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_PathHistoryMaxCountDefault) {
                    intValue = loader.IntValue;
                    if (CheckPathHistoryMaxCountDefault(ref intValue, null)) {
                        obj.m_pathHistoryMaxCountDefault = intValue;
                    } else {
                        obj.m_pathHistoryMaxCountDefault = defValue.m_pathHistoryMaxCountDefault;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_PathHistoryWholeSaveDisk) {
                    obj.m_pathHistoryWholeSaveDisk = loader.BoolValue;
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_ViewerSearchHistoryMaxCountDefault) {
                    intValue = loader.IntValue;
                    if (CheckViewerSearchHistoryMaxCountDefault(ref intValue, null)) {
                        obj.m_viewerSearchHistoryMaxCountDefault = intValue;
                    } else {
                        obj.m_viewerSearchHistoryMaxCountDefault = defValue.m_viewerSearchHistoryMaxCountDefault;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_PathHistoryWholeMaxCountDefault) {
                    intValue = loader.IntValue;
                    if (CheckPathHistoryMaxCountDefault(ref intValue, null)) {
                        obj.m_pathHistoryWholeMaxCountDefault = intValue;
                    } else {
                        obj.m_pathHistoryWholeMaxCountDefault = defValue.m_pathHistoryWholeMaxCountDefault;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_ViewerSearchHistorySaveDisk) {
                    obj.m_viewerSearchHistorySaveDisk = loader.BoolValue;

                // テキストビューア＞全般
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_TextViewerMaxFileSize) {
                    intValue = loader.IntValue;
                    if (CheckTextViewerMaxFileSize(ref intValue, null)) {
                        obj.m_textViewerMaxFileSize = intValue;
                    } else {
                        obj.m_textViewerMaxFileSize = defValue.m_textViewerMaxFileSize;
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_TextViewerMaxLineCount) {
                    intValue = loader.IntValue;
                    if (CheckTextViewerMaxLineCount(ref intValue, null)) {
                        obj.m_textViewerMaxLineCount = intValue;
                    } else {
                        obj.m_textViewerMaxLineCount = defValue.m_textViewerMaxLineCount;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_TextViewerClearCompareBufferDefault) {
                    obj.m_textViewerClearCompareBufferDefault = new BooleanFlag(loader.BoolValue);

                // テキストビューア＞表示
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_TextViewerIsDisplayLineNumber) {
                    obj.m_textViewerIsDisplayLineNumber = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_TextViewerIsDisplayCtrlChar) {
                    obj.m_textViewerIsDisplayCtrlChar = loader.BoolValue;

                // テキストビューア＞折返しとタブ
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_TextViewerLineBreakDefault) {
                    success = TextViewerLineBreakSetting.LoadSetting(loader, out obj.m_textViewerLineBreakDefault);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_TextViewerLineBreakDefault, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_TextViewerTab4Extension) {
                    strValue = loader.StringValue;
                    if (CheckTextViewerTab4Extension(ref strValue, null)) {
                        obj.m_textViewerTab4Extension = strValue;
                    } else {
                        obj.m_textViewerTab4Extension = defValue.m_textViewerTab4Extension;
                    }

                // テキストビューア＞検索オプション
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_TextSearchOptionDefault) {
                    success = TextSearchOption.LoadSetting(loader, out obj.m_textSearchOptionDefault);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_TextSearchOptionDefault, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }

                // テキストビューア＞クリップボード
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_TextClipboardSettingDefault) {
                    success = TextClipboardSetting.LoadSetting(loader, out obj.m_textClipboardSettingDefault);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_TextClipboardSettingDefault, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Config_DumpClipboardSettingDefault) {
                    success = DumpClipboardSetting.LoadSetting(loader, out obj.m_dumpClipboardSettingDefault);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Config_DumpClipboardSettingDefault, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }

                // グラフィックビューア＞全般
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_GraphicsViewerMaxFileSize) {
                    intValue = loader.IntValue;
                    if (CheckGraphicsViewerMaxFileSize(ref intValue, null)) {
                        obj.m_graphicsViewerMaxFileSize = intValue;
                    } else {
                        obj.m_graphicsViewerMaxFileSize = defValue.m_graphicsViewerMaxFileSize;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_GraphicsViewerDragInertia) {
                    strValue = loader.StringValue;
                    obj.m_graphicsViewerDragInertia = DragInertiaType.FromSerializedData(strValue);
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_GraphicsViewerDragBreaking) {
                    intValue = loader.IntValue;
                    if (CheckGraphicsViewerDragBreaking(ref intValue, null)) {
                        obj.m_graphicsViewerDragBreaking = intValue;
                    } else {
                        obj.m_graphicsViewerDragBreaking = defValue.m_graphicsViewerDragBreaking;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_GraphicsViewerFilterMode) {
                    strValue = loader.StringValue;
                    obj.m_graphicsViewerFilterMode = GraphicsViewerFilterMode.FromSerializedData(strValue);
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_GraphicsViewerFullScreenHideTimer) {
                    intValue = loader.IntValue;
                    if (CheckGraphicsViewerFullScreenHideTimer(ref intValue, null)) {
                        obj.m_graphicsViewerFullScreenHideTimer = intValue;
                    } else {
                        obj.m_graphicsViewerFullScreenHideTimer = defValue.m_graphicsViewerFullScreenHideTimer;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_GraphicsViewerFullScreenAutoHideCursor) {
                    obj.m_graphicsViewerFullScreenAutoHideCursor = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_GraphicsViewerFullScreenAutoHideInfo) {
                    obj.m_graphicsViewerFullScreenAutoHideInfo = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_GraphicsViewerFullScreenHideInfoAlways) {
                    obj.m_graphicsViewerFullScreenHideInfoAlways = loader.BoolValue;

                // グラフィックビューア＞拡大表示
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_GraphicsViewerAutoZoomMode) {
                    obj.m_graphicsViewerAutoZoomMode = GraphicsViewerAutoZoomMode.FromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_GraphicsViewerZoomInLarger) {
                    obj.m_graphicsViewerZoomInLarger = loader.BoolValue;
                    
                // ファンクションキー＞全般
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_FunctionBarSplitCount) {
                    intValue = loader.IntValue;
                    if (CheckFunctionBarSplitCount(ref intValue, null)) {
                        obj.m_functionBarSplitCount = intValue;
                    } else {
                        obj.m_functionBarSplitCount = defValue.m_functionBarSplitCount;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.Config_FunctionBarUseOverrayIcon) {
                    obj.m_functionBarUseOverrayIcon = loader.BoolValue;

                // ログ＞全般
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_LogLineMaxCountDefault) {
                    intValue = loader.IntValue;
                    if (CheckLogLineMaxCountDefault(ref intValue, null)) {
                        obj.m_logLineMaxCountDefault = intValue;
                    } else {
                        obj.m_logLineMaxCountDefault = defValue.m_logLineMaxCountDefault;
                    }

                // 色とフォント＞ファイル一覧
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListBackColor) {
                    obj.m_fileListBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListBackColor) {
                    obj.m_fileListBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListFileTextColor) {
                    obj.m_fileListFileTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListReadOnlyColor) {
                    obj.m_fileListReadOnlyColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListHiddenColor) {
                    obj.m_fileListHiddenColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListSystemColor) {
                    obj.m_fileListSystemColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListArchiveColor) {
                    obj.m_fileListArchiveColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListSymlinkColor) {
                    obj.m_fileListSymlinkColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListGrayColor) {
                    obj.m_fileListGrayColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListMarkColor) {
                    obj.m_fileListMarkColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListMarkBackColor1) {
                    obj.m_fileListMarkBackColor1 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListMarkBackColor2) {
                    obj.m_fileListMarkBackColor2 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListMarkBackBorderColor) {
                    obj.m_fileListMarkBackBorderColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListGrayBackColor) {
                    obj.m_fileListGrayBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListMarkGrayColor) {
                    obj.m_fileListMarkGrayColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListMarkGrayBackColor1) {
                    obj.m_fileListMarkGrayBackColor1 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListMarkGrayBackColor2) {
                    obj.m_fileListMarkGrayBackColor2 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListMarkGrayBackBorderColor) {
                    obj.m_fileListMarkGrayBackBorderColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListCursorColor) {
                    obj.m_fileListCursorColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListCursorDisableColor) {
                    obj.m_fileListCursorDisableColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListStatusBarSuperUserColor) {
                    obj.m_fileListStatusBarSuperUserColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListThumbnailFrameColor1) {
                    obj.m_fileListThumbnailFrameColor1 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_FileListThumbnailFrameColor2) {
                    obj.m_fileListThumbnailFrameColor2 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_DialogErrorBackColor) {
                    obj.m_dialogErrorBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_DialogErrorTextColor) {
                    obj.m_dialogErrorTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_DialogWarningBackColor) {
                    obj.m_dialogWarningBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_DialogWarningTextColor) {
                    obj.m_dialogWarningTextColor = loader.ColorValue;

                // 色とフォント＞テキストビューア
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerErrorBackColor) {
                    obj.m_textViewerErrorBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerErrorStatusBackColor) {
                    obj.m_textViewerErrorStatusBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerErrorStatusTextColor) {
                    obj.m_textViewerErrorStatusTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerInfoStatusBackColor) {
                    obj.m_textViewerInfoStatusBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerInfoStatusTextColor) {
                    obj.m_textViewerInfoStatusTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerLineNoTextColor) {
                    obj.m_textViewerLineNoTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerLineNoBackColor1) {
                    obj.m_textViewerLineNoBackColor1 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerLineNoBackColor2) {
                    obj.m_textViewerLineNoBackColor2 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerLineNoSeparatorColor) {
                    obj.m_textViewerLineNoSeparatorColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerOutOfAreaBackColor) {
                    obj.m_textViewerOutOfAreaBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerOutOfAreaSeparatorColor) {
                    obj.m_textViewerOutOfAreaSeparatorColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerSearchCursorColor) {
                    obj.m_textViewerSearchCursorColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerControlColor) {
                    obj.m_textViewerControlColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerTextColor) {
                    obj.m_textViewerTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerSelectTextColor) {
                    obj.m_textViewerSelectTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerSelectTextColor2) {
                    obj.m_textViewerSelectTextColor2 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerSelectBackColor) {
                    obj.m_textViewerSelectBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerSelectBackColor2) {
                    obj.m_textViewerSelectBackColor2 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerSearchHitBackColor) {
                    obj.m_textViewerSearchHitBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerSearchHitTextColor) {
                    obj.m_textViewerSearchHitTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_TextViewerSearchAutoTextColor) {
                    obj.m_textViewerSearchAutoTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_RadarBarBackColor1) {
                    obj.m_radarBarBackColor1 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_RadarBarBackColor2) {
                    obj.m_radarBarBackColor2 = loader.ColorValue;

                // 色とフォント＞グラフィックビューア
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_GraphicsViewerBackColor) {
                    obj.m_graphicsViewerBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_GraphicsViewerTextColor) {
                    obj.m_graphicsViewerTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_GraphicsViewerTextShadowColor) {
                    obj.m_graphicsViewerTextShadowColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_GraphicsViewerLoadingTextColor) {
                    obj.m_graphicsViewerLoadingTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_GraphicsViewerLoadingTextShadowColor) {
                    obj.m_graphicsViewerLoadingTextShadowColor = loader.ColorValue;

                // 色とフォント＞ログ
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowTextColor) {
                    obj.m_logWindowTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowLinkTextColor) {
                    obj.m_logWindowLinkTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogErrorTextColor) {
                    obj.m_logErrorTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogStdErrorTextColor) {
                    obj.m_logStdErrorTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowSelectTextColor) {
                    obj.m_logWindowSelectTextColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowBackColor) {
                    obj.m_logWindowBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowSelectBackColor) {
                    obj.m_logWindowSelectBackColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowBackBellColor) {
                    obj.m_logWindowBackBellColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowBackClosedColor) {
                    obj.m_logWindowBackClosedColor = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowProgressColor1) {
                    obj.m_logWindowProgressColor1 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowProgressColor2) {
                    obj.m_logWindowProgressColor2 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowProgressColor3) {
                    obj.m_logWindowProgressColor3 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowProgressColor4) {
                    obj.m_logWindowProgressColor4 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowRemainingTimeTextColor1) {
                    obj.m_logWindowRemainingTimeTextColor1 = loader.ColorValue;
                } else if (tagType == SettingTagType.ColorValue && tagName == SettingTag.Config_LogWindowRemainingTimeTextColor2) {
                    obj.m_logWindowRemainingTimeTextColor2 = loader.ColorValue;

                // 色とフォント＞フォント
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_ListViewFontName) {
                    obj.m_listViewFontName = loader.StringValue;
                } else if (tagType == SettingTagType.FloatValue && tagName == SettingTag.Config_ListViewFontSize) {
                    obj.m_listViewFontSize = loader.FloatValue;
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_DefaultFileListViewHeight) {
                    obj.m_defaultFileListViewHeight = loader.IntValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_ThumbFileListViewFontName) {
                    obj.m_thumbFileListViewFontName = loader.StringValue;
                } else if (tagType == SettingTagType.FloatValue && tagName == SettingTag.Config_ThumbFileListViewFontSize) {
                    obj.m_thumbFileListViewFontSize = loader.FloatValue;
                } else if (tagType == SettingTagType.FloatValue && tagName == SettingTag.Config_ThumbFileListViewSmallFontSize) {
                    obj.m_thumbFileListViewSmallFontSize = loader.FloatValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_TextFontName) {
                    obj.m_textFontName = loader.StringValue;
                } else if (tagType == SettingTagType.FloatValue && tagName == SettingTag.Config_TextFontSize) {
                    obj.m_textFontSize = loader.FloatValue;
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.Config_TextFileViewerLineHeight) {
                    obj.m_textFileViewerLineHeight = loader.IntValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_FunctionBarFontName) {
                    obj.m_functionBarFontName = loader.StringValue;
                } else if (tagType == SettingTagType.FloatValue && tagName == SettingTag.Config_FunctionBarFontSize) {
                    obj.m_functionBarFontSize = loader.FloatValue;
                } else if (tagType == SettingTagType.FloatValue && tagName == SettingTag.Config_FunctionBarFontSize) {
                    obj.m_functionBarFontSize = loader.FloatValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_LogWindowFontName) {
                    obj.m_logWindowFontName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Config_LogWindowFixedFontName) {
                    obj.m_logWindowFixedFontName = loader.StringValue;
                } else if (tagType == SettingTagType.FloatValue && tagName == SettingTag.Config_LogWindowFontSize) {
                    obj.m_logWindowFontSize = loader.FloatValue;
                } else if (tagType == SettingTagType.FloatValue && tagName == SettingTag.Config_LogWindowTerminalFontSize) {
                    obj.m_logWindowTerminalFontSize = loader.FloatValue;
                }
            }
            m_lastFileWriteTime = loader.LastFileWriteTime;
            return true;
        }

        //=========================================================================================
        // 機　能：設定の保存を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SaveSetting() {
#if !FREE_VERSION
            string fileName = DirectoryManager.ConfigurationSetting;
            SettingSaver saver = new SettingSaver(fileName);
            bool success = SaveSettingInternal(saver, this);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotSaveSetting, fileName);
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存対象のオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private static bool SaveSettingInternal(SettingSaver saver, Configuration obj) {
            bool success;

            saver.StartObject(SettingTag.Config_Configuration);

            // インストール情報＞全般
            saver.AddString(SettingTag.Config_TemporaryDirectoryDefault, obj.m_temporaryDirectoryDefault);
            saver.AddString(SettingTag.Config_TextEditorCommandLine, obj.m_textEditorCommandLine);
            saver.AddString(SettingTag.Config_TextEditorCommandLineSSH, obj.m_textEditorCommandLineSSH);
            saver.AddString(SettingTag.Config_TextEditorCommandLineWithLineNumber, obj.m_textEditorCommandLineWithLineNumber);
            saver.AddString(SettingTag.Config_TextEditorCommandLineWithLineNumberSSH, obj.m_textEditorCommandLineWithLineNumberSSH);
            saver.AddString(SettingTag.Config_DiffCommandLine, obj.m_diffCommandLine);
            saver.AddString(SettingTag.Config_DiffDirectoryCommandLine, obj.m_diffDirectoryCommandLine);

            // ファイル一覧＞全般
            saver.AddInt(SettingTag.Config_AutoDirectoryUpdateWait, obj.m_autoDirectoryUpdateWait);
            saver.AddBool(SettingTag.Config_RefreshFileListTabChange, obj.m_refreshFileListTabChange);
            saver.AddBool(SettingTag.Config_RefreshFileListTabChangeSSH, obj.m_refreshFileListTabChangeSSH);

            // ファイル一覧＞起動時の状態
            saver.AddString(SettingTag.Config_InitialDirectoryLeft, obj.m_initialDirectoryLeft);
            saver.AddString(SettingTag.Config_InitialDirectoryRight, obj.m_initialDirectoryRight);
            saver.AddRectangle(SettingTag.Config_MainWindowRectDefault, obj.m_mainWindowRectDefault);
            saver.AddInt(SettingTag.Config_SplashWindowWait, obj.m_splashWindowWait);

            // ファイル一覧＞起動時の一覧
            saver.StartObject(SettingTag.Config_DefaultFileListSortModeLeft);
            success = FileListSortMode.SaveSetting(saver, obj.m_defaultFileListSortModeLeft);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_DefaultFileListSortModeLeft);
            saver.StartObject(SettingTag.Config_DefaultFileListSortModeRight);
            success = FileListSortMode.SaveSetting(saver, obj.m_defaultFileListSortModeRight);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_DefaultFileListSortModeRight);

            // ファイル一覧＞フォルダサイズの取得
            saver.StartObject(SettingTag.Config_RetrieveFolderSizeCondition);
            success = RetrieveFolderSizeCondition.SaveSetting(saver, obj.m_retrieveFolderSizeCondition);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_RetrieveFolderSizeCondition);
            saver.AddInt(SettingTag.Config_RetrieveFolderSizeKeepLowerDepth, obj.m_retrieveFolderSizeKeepLowerDepth);
            saver.AddInt(SettingTag.Config_RetrieveFolderSizeKeepLowerCount, obj.m_retrieveFolderSizeKeepLowerCount);

            // ファイル一覧＞動作
            saver.AddInt(SettingTag.Config_ListViewScrollMarginLineCount, obj.m_listViewScrollMarginLineCount);
            saver.AddInt(SettingTag.Config_MouseWheelMaxLines, obj.m_mouseWheelMaxLines);
            saver.AddInt(SettingTag.Config_FileListDragMaxSpeed, obj.m_fileListDragMaxSpeed);
            saver.AddBool(SettingTag.Config_FileListSeparateExt, obj.m_fileListSeparateExt);
            saver.AddBool(SettingTag.Config_ChdirParentOtherSideMove, obj.m_chdirParentOtherSideMove);
            saver.AddBool(SettingTag.Config_HideWindowDragDrop, obj.m_hideWindowDragDrop);
            saver.AddBool(SettingTag.Config_ResumeFolderCursorFile, obj.m_resumeFolderCursorFile);

            // ファイル一覧＞起動時の表示モード
            saver.StartObject(SettingTag.Config_DefaultViewModeLeft);
            success = FileListViewMode.SaveSetting(saver, obj.m_defaultViewModeLeft);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_DefaultViewModeLeft);

            saver.StartObject(SettingTag.Config_DefaultViewModeRight);
            success = FileListViewMode.SaveSetting(saver, obj.m_defaultViewModeRight);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_DefaultViewModeRight);

            // ファイル一覧＞表示モード
            saver.StartObject(SettingTag.Config_FileListViewChangeMode);
            success = FileListViewMode.SaveSetting(saver, obj.m_fileListViewChangeMode);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_FileListViewChangeMode);

            saver.StartObject(SettingTag.Config_FileListViewModeAutoSetting);
            success = FileListViewModeAutoSetting.SaveSetting(saver, obj.m_fileListViewModeAutoSetting);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_FileListViewModeAutoSetting);

            // ファイル操作＞全般
            saver.AddInt(SettingTag.Config_MaxBackgroundTaskCountDefault, obj.m_maxBackgroundTaskWaitableCountDefault);
            saver.AddInt(SettingTag.Config_MaxBackgroundTaskLimitedCountDefault, obj.m_maxBackgroundTaskLimitedCountDefault);

            // ファイル操作＞転送と削除
            saver.StartObject(SettingTag.Config_SameFileOptionDefault);
            success = SameFileOption.SaveSetting(saver, obj.m_sameFileOptionDefault);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_SameFileOptionDefault);
            saver.StartObject(SettingTag.Config_DeleteFileOptionDefault);
            success = DeleteFileOption.SaveSetting(saver, obj.m_deleteFileOptionDefault);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_DeleteFileOptionDefault);

            // ファイル操作＞属性のコピー
            saver.StartObject(SettingTag.Config_TransferAttributeSetMode);
            success = AttributeSetMode.SaveSetting(saver, obj.m_transferAttributeSetMode);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_TransferAttributeSetMode);

            // ファイル操作＞マークなし操作
            saver.AddBool(SettingTag.Config_MarklessCopy, obj.m_marklessCopy);
            saver.AddBool(SettingTag.Config_MarklessMove, obj.m_marklessMove);
            saver.AddBool(SettingTag.Config_MarklessDelete, obj.m_marklessDelete);
            saver.AddBool(SettingTag.Config_MarklessShortcut, obj.m_marklessShortcut);
            saver.AddBool(SettingTag.Config_MarklessAttribute, obj.m_marklessAttribute);
            saver.AddBool(SettingTag.Config_MarklessPack, obj.m_marklessPack);
            saver.AddBool(SettingTag.Config_MarklessUnpack, obj.m_marklessUnpack);
            saver.AddBool(SettingTag.Config_MarklessEdit, obj.m_marklessEdit);
            saver.AddBool(SettingTag.Config_MarklessFolderSize, obj.m_marklessFodlerSize);

            // ファイル操作＞クリップボード
            saver.StartObject(SettingTag.Config_ClipboardCopyNameAsSettingDefault);
            success = ClipboardCopyNameAsSetting.SaveSetting(saver, obj.m_clipboardCopyNameAsSettingDefault);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_ClipboardCopyNameAsSettingDefault);

            // ファイル操作＞一覧の比較
            saver.StartObject(SettingTag.Config_FileCompareSettingDefault);
            success = FileCompareSetting.SaveSetting(saver, obj.m_fileCompareSettingDefault);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_FileCompareSettingDefault);

            // ファイル操作＞圧縮
            saver.StartObject(SettingTag.Config_ArchiveSettingDefault);
            success = ArchiveSetting.SaveSetting(saver, obj.m_archiveSettingDefault);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_ArchiveSettingDefault);

            // ファイル操作＞展開
            saver.AddString(SettingTag.Config_ArchiveExtractPathMode, ExtractPathMode.ToSerializedData(obj.m_archiveExtractPathMode));

            // ファイル操作＞各種操作
            saver.AddBooleanFlag(SettingTag.Config_IncrementalSearchFromHeadDefault, obj.m_incrementalSearchFromHeadDefault);
            saver.AddBooleanFlag(SettingTag.Config_MakeDirectoryMoveCurrentDefault, obj.m_makeDirectoryMoveCurrentDefault);
            saver.AddString(SettingTag.Config_MakeDirectoryNewWindowsName, obj.m_makeDirectoryNewWindowsName);
            saver.AddString(SettingTag.Config_MakeDirectoryNewSSHName, obj.m_makeDirectoryNewSSHName);
            saver.AddString(SettingTag.Config_ShellExecuteReplayModeWindowsDefault, ShellExecuteRelayMode.ToSerializedData(obj.m_shellExecuteReplayModeWindowsDefault));
            saver.AddString(SettingTag.Config_ShellExecuteReplayModeSSHDefault, ShellExecuteRelayMode.ToSerializedData(obj.m_shellExecuteReplayModeSSHDefault));
            saver.AddString(SettingTag.Config_MirrorCopyExceptFiles, obj.m_mirrorCopyExceptFiles);

            // SSH＞全般
            saver.AddString(SettingTag.Config_SshShortcutTypeDefault, ShortcutType.ToSerializedData(obj.m_sshShortcutTypeDefault));
            saver.AddString(SettingTag.Config_SshFileSystemDefault, SSHProtocolType.ToSerializedData(obj.m_sshFileSystemDefault));

            // SSH＞ターミナル
            saver.AddInt(SettingTag.Config_TerminalLogLineCount, obj.m_terminalLogLineCount);
            saver.AddBool(SettingTag.Config_TerminalShellCommandSSH, obj.m_terminalShellCommandSSH);
            saver.AddString(SettingTag.Config_TerminalCloseConfirmMode, TerminalCloseConfirmMode.ToSerializedData(obj.m_terminalCloseConfirmMode));

            // SSH＞ターミナルログ
            saver.AddString(SettingTag.Config_TerminalLogType, TerminalLogType.ToSerializedData(obj.m_terminalLogType));
            saver.AddInt(SettingTag.Config_TerminalLogMaxSize, obj.m_terminalLogMaxSize);
            saver.AddInt(SettingTag.Config_TerminalLogFileCount, obj.m_terminalLogFileCount);
            saver.AddString(SettingTag.Config_TerminalLogOutputFolder, obj.m_terminalLogOutputFolder);

            // プライバシー＞全般
            saver.AddInt(SettingTag.Config_CommandHistoryMaxCountDefault, obj.m_commandHistoryMaxCountDefault);
            saver.AddBool(SettingTag.Config_CommandHistorySaveDisk, obj.m_commandHistorySaveDisk);
            saver.AddInt(SettingTag.Config_PathHistoryMaxCountDefault, obj.m_pathHistoryMaxCountDefault);
            saver.AddInt(SettingTag.Config_PathHistoryWholeMaxCountDefault, obj.m_pathHistoryWholeMaxCountDefault);
            saver.AddBool(SettingTag.Config_PathHistoryWholeSaveDisk, obj.m_pathHistoryWholeSaveDisk);
            saver.AddInt(SettingTag.Config_ViewerSearchHistoryMaxCountDefault, obj.m_viewerSearchHistoryMaxCountDefault);
            saver.AddBool(SettingTag.Config_ViewerSearchHistorySaveDisk, obj.m_viewerSearchHistorySaveDisk);

            // テキストビューア＞全般
            saver.AddInt(SettingTag.Config_TextViewerMaxFileSize, obj.m_textViewerMaxFileSize);
            saver.AddInt(SettingTag.Config_TextViewerMaxLineCount, obj.m_textViewerMaxLineCount);
            saver.AddBooleanFlag(SettingTag.Config_TextViewerClearCompareBufferDefault, obj.m_textViewerClearCompareBufferDefault);

            // テキストビューア＞表示
            saver.AddBool(SettingTag.Config_TextViewerIsDisplayLineNumber, obj.m_textViewerIsDisplayLineNumber);
            saver.AddBool(SettingTag.Config_TextViewerIsDisplayCtrlChar, obj.m_textViewerIsDisplayCtrlChar);

            // テキストビューア＞折返しとタブ
            saver.StartObject(SettingTag.Config_TextViewerLineBreakDefault);
            success = TextViewerLineBreakSetting.SaveSetting(saver, obj.m_textViewerLineBreakDefault);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_TextViewerLineBreakDefault);
            saver.AddString(SettingTag.Config_TextViewerTab4Extension, obj.m_textViewerTab4Extension);

            // テキストビューア＞検索オプション
            saver.StartObject(SettingTag.Config_TextSearchOptionDefault);
            success = TextSearchOption.SaveSetting(saver, obj.m_textSearchOptionDefault);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_TextSearchOptionDefault);

            // テキストビューア＞クリップボード
            saver.StartObject(SettingTag.Config_TextClipboardSettingDefault);
            success = TextClipboardSetting.SaveSetting(saver, obj.m_textClipboardSettingDefault);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_TextClipboardSettingDefault);
            saver.StartObject(SettingTag.Config_DumpClipboardSettingDefault);
            success = DumpClipboardSetting.SaveSetting(saver, obj.m_dumpClipboardSettingDefault);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Config_DumpClipboardSettingDefault);

            // グラフィックビューア＞全般
            saver.AddInt(SettingTag.Config_GraphicsViewerMaxFileSize, obj.m_graphicsViewerMaxFileSize);
            saver.AddString(SettingTag.Config_GraphicsViewerDragInertia, DragInertiaType.ToSerializedData(obj.m_graphicsViewerDragInertia));
            saver.AddInt(SettingTag.Config_GraphicsViewerDragBreaking, obj.m_graphicsViewerDragBreaking);
            saver.AddString(SettingTag.Config_GraphicsViewerFilterMode, GraphicsViewerFilterMode.ToSerializedData(obj.m_graphicsViewerFilterMode));
            saver.AddInt(SettingTag.Config_GraphicsViewerFullScreenHideTimer, obj.m_graphicsViewerFullScreenHideTimer);
            saver.AddBool(SettingTag.Config_GraphicsViewerFullScreenAutoHideCursor, obj.m_graphicsViewerFullScreenAutoHideCursor);
            saver.AddBool(SettingTag.Config_GraphicsViewerFullScreenAutoHideInfo, obj.m_graphicsViewerFullScreenAutoHideInfo);
            saver.AddBool(SettingTag.Config_GraphicsViewerFullScreenHideInfoAlways, obj.m_graphicsViewerFullScreenHideInfoAlways);

            // グラフィックビューア＞拡大表示
            saver.AddString(SettingTag.Config_GraphicsViewerAutoZoomMode, GraphicsViewerAutoZoomMode.ToSerializedData(obj.m_graphicsViewerAutoZoomMode));
            saver.AddBool(SettingTag.Config_GraphicsViewerZoomInLarger, obj.m_graphicsViewerZoomInLarger);

            // ファンクションキー＞全般
            saver.AddInt(SettingTag.Config_FunctionBarSplitCount, obj.m_functionBarSplitCount);
            saver.AddBool(SettingTag.Config_FunctionBarUseOverrayIcon, obj.m_functionBarUseOverrayIcon);

            // ログ＞全般
            saver.AddInt(SettingTag.Config_LogLineMaxCountDefault, obj.m_logLineMaxCountDefault);

            // 色とフォント＞ファイル一覧
            saver.AddColor(SettingTag.Config_FileListBackColor, obj.m_fileListBackColor);
            saver.AddColor(SettingTag.Config_FileListFileTextColor, obj.m_fileListFileTextColor);
            saver.AddColor(SettingTag.Config_FileListReadOnlyColor, obj.m_fileListReadOnlyColor);
            saver.AddColor(SettingTag.Config_FileListHiddenColor, obj.m_fileListHiddenColor);
            saver.AddColor(SettingTag.Config_FileListSystemColor, obj.m_fileListSystemColor);
            saver.AddColor(SettingTag.Config_FileListArchiveColor, obj.m_fileListArchiveColor);
            saver.AddColor(SettingTag.Config_FileListSymlinkColor, obj.m_fileListSymlinkColor);
            saver.AddColor(SettingTag.Config_FileListGrayColor, obj.m_fileListGrayColor);
            saver.AddColor(SettingTag.Config_FileListMarkColor, obj.m_fileListMarkColor);
            saver.AddColor(SettingTag.Config_FileListMarkBackColor1, obj.m_fileListMarkBackColor1);
            saver.AddColor(SettingTag.Config_FileListMarkBackColor2, obj.m_fileListMarkBackColor2);
            saver.AddColor(SettingTag.Config_FileListMarkBackBorderColor, obj.m_fileListMarkBackBorderColor);
            saver.AddColor(SettingTag.Config_FileListGrayBackColor, obj.m_fileListGrayBackColor);
            saver.AddColor(SettingTag.Config_FileListMarkGrayColor, obj.m_fileListMarkGrayColor);
            saver.AddColor(SettingTag.Config_FileListMarkGrayBackColor1, obj.m_fileListMarkGrayBackColor1);
            saver.AddColor(SettingTag.Config_FileListMarkGrayBackColor2, obj.m_fileListMarkGrayBackColor2);
            saver.AddColor(SettingTag.Config_FileListMarkGrayBackBorderColor, obj.m_fileListMarkGrayBackBorderColor);
            saver.AddColor(SettingTag.Config_FileListCursorColor, obj.m_fileListCursorColor);
            saver.AddColor(SettingTag.Config_FileListCursorDisableColor, obj.m_fileListCursorDisableColor);
            saver.AddColor(SettingTag.Config_FileListStatusBarSuperUserColor, obj.m_fileListStatusBarSuperUserColor);
            saver.AddColor(SettingTag.Config_FileListThumbnailFrameColor1, obj.m_fileListThumbnailFrameColor1);
            saver.AddColor(SettingTag.Config_FileListThumbnailFrameColor2, obj.m_fileListThumbnailFrameColor2);
            saver.AddColor(SettingTag.Config_DialogErrorBackColor, obj.m_dialogErrorBackColor);
            saver.AddColor(SettingTag.Config_DialogErrorTextColor, obj.m_dialogErrorTextColor);
            saver.AddColor(SettingTag.Config_DialogWarningBackColor, obj.m_dialogWarningBackColor);
            saver.AddColor(SettingTag.Config_DialogWarningTextColor, obj.m_dialogWarningTextColor);

            // 色とフォント＞テキストビューア
            saver.AddColor(SettingTag.Config_TextViewerErrorBackColor, obj.m_textViewerErrorBackColor);
            saver.AddColor(SettingTag.Config_TextViewerErrorStatusBackColor, obj.m_textViewerErrorStatusBackColor);
            saver.AddColor(SettingTag.Config_TextViewerErrorStatusTextColor, obj.m_textViewerErrorStatusTextColor);
            saver.AddColor(SettingTag.Config_TextViewerInfoStatusBackColor, obj.m_textViewerInfoStatusBackColor);
            saver.AddColor(SettingTag.Config_TextViewerInfoStatusTextColor, obj.m_textViewerInfoStatusTextColor);
            saver.AddColor(SettingTag.Config_TextViewerLineNoTextColor, obj.m_textViewerLineNoTextColor);
            saver.AddColor(SettingTag.Config_TextViewerLineNoBackColor1, obj.m_textViewerLineNoBackColor1);
            saver.AddColor(SettingTag.Config_TextViewerLineNoBackColor2, obj.m_textViewerLineNoBackColor2);
            saver.AddColor(SettingTag.Config_TextViewerLineNoSeparatorColor, obj.m_textViewerLineNoSeparatorColor);
            saver.AddColor(SettingTag.Config_TextViewerOutOfAreaBackColor, obj.m_textViewerOutOfAreaBackColor);
            saver.AddColor(SettingTag.Config_TextViewerOutOfAreaSeparatorColor, obj.m_textViewerOutOfAreaSeparatorColor);
            saver.AddColor(SettingTag.Config_TextViewerSearchCursorColor, obj.m_textViewerSearchCursorColor);
            saver.AddColor(SettingTag.Config_TextViewerControlColor, obj.m_textViewerControlColor);
            saver.AddColor(SettingTag.Config_TextViewerTextColor, obj.m_textViewerTextColor);
            saver.AddColor(SettingTag.Config_TextViewerSelectTextColor, obj.m_textViewerSelectTextColor);
            saver.AddColor(SettingTag.Config_TextViewerSelectTextColor2, obj.m_textViewerSelectTextColor2);
            saver.AddColor(SettingTag.Config_TextViewerSelectBackColor, obj.m_textViewerSelectBackColor);
            saver.AddColor(SettingTag.Config_TextViewerSelectBackColor2, obj.m_textViewerSelectBackColor2);
            saver.AddColor(SettingTag.Config_TextViewerSearchHitBackColor, obj.m_textViewerSearchHitBackColor);
            saver.AddColor(SettingTag.Config_TextViewerSearchHitTextColor, obj.m_textViewerSearchHitTextColor);
            saver.AddColor(SettingTag.Config_TextViewerSearchAutoTextColor, obj.m_textViewerSearchAutoTextColor);
            saver.AddColor(SettingTag.Config_RadarBarBackColor1, obj.m_radarBarBackColor1);
            saver.AddColor(SettingTag.Config_RadarBarBackColor2, obj.m_radarBarBackColor2);

            // 色とフォント＞グラフィックビューア
            saver.AddColor(SettingTag.Config_GraphicsViewerBackColor, obj.m_graphicsViewerBackColor);
            saver.AddColor(SettingTag.Config_GraphicsViewerTextColor, obj.m_graphicsViewerTextColor);
            saver.AddColor(SettingTag.Config_GraphicsViewerTextShadowColor, obj.m_graphicsViewerTextShadowColor);
            saver.AddColor(SettingTag.Config_GraphicsViewerLoadingTextColor, obj.m_graphicsViewerLoadingTextColor);
            saver.AddColor(SettingTag.Config_GraphicsViewerLoadingTextShadowColor, obj.m_graphicsViewerLoadingTextShadowColor);

            // 色とフォント＞ログ
            saver.AddColor(SettingTag.Config_LogWindowTextColor, obj.m_logWindowTextColor);
            saver.AddColor(SettingTag.Config_LogWindowLinkTextColor, obj.m_logWindowLinkTextColor);
            saver.AddColor(SettingTag.Config_LogErrorTextColor, obj.m_logErrorTextColor);
            saver.AddColor(SettingTag.Config_LogStdErrorTextColor, obj.m_logStdErrorTextColor);
            saver.AddColor(SettingTag.Config_LogWindowSelectTextColor, obj.m_logWindowSelectTextColor);
            saver.AddColor(SettingTag.Config_LogWindowBackColor, obj.m_logWindowBackColor);
            saver.AddColor(SettingTag.Config_LogWindowSelectBackColor, obj.m_logWindowSelectBackColor);
            saver.AddColor(SettingTag.Config_LogWindowBackBellColor, obj.m_logWindowBackBellColor);
            saver.AddColor(SettingTag.Config_LogWindowBackClosedColor, obj.m_logWindowBackClosedColor);
            saver.AddColor(SettingTag.Config_LogWindowProgressColor1, obj.m_logWindowProgressColor1);
            saver.AddColor(SettingTag.Config_LogWindowProgressColor2, obj.m_logWindowProgressColor2);
            saver.AddColor(SettingTag.Config_LogWindowProgressColor3, obj.m_logWindowProgressColor3);
            saver.AddColor(SettingTag.Config_LogWindowProgressColor4, obj.m_logWindowProgressColor4);
            saver.AddColor(SettingTag.Config_LogWindowRemainingTimeTextColor1, obj.m_logWindowRemainingTimeTextColor1);
            saver.AddColor(SettingTag.Config_LogWindowRemainingTimeTextColor2, obj.m_logWindowRemainingTimeTextColor2);
            
            // 色とフォント＞フォント
            saver.AddString(SettingTag.Config_ListViewFontName, obj.m_listViewFontName);
            saver.AddFloat(SettingTag.Config_ListViewFontSize, obj.m_listViewFontSize);
            saver.AddInt(SettingTag.Config_DefaultFileListViewHeight, obj.m_defaultFileListViewHeight);
            saver.AddString(SettingTag.Config_ThumbFileListViewFontName, obj.m_thumbFileListViewFontName);
            saver.AddFloat(SettingTag.Config_ThumbFileListViewFontSize, obj.m_thumbFileListViewFontSize);
            saver.AddFloat(SettingTag.Config_ThumbFileListViewSmallFontSize, obj.m_thumbFileListViewSmallFontSize);
            saver.AddString(SettingTag.Config_TextFontName, obj.m_textFontName);
            saver.AddFloat(SettingTag.Config_TextFontSize, obj.m_textFontSize);
            saver.AddInt(SettingTag.Config_TextFileViewerLineHeight, obj.m_textFileViewerLineHeight);
            saver.AddString(SettingTag.Config_FunctionBarFontName, obj.m_functionBarFontName);
            saver.AddFloat(SettingTag.Config_FunctionBarFontSize, obj.m_functionBarFontSize);
            saver.AddString(SettingTag.Config_LogWindowFontName, obj.m_logWindowFontName);
            saver.AddString(SettingTag.Config_LogWindowFixedFontName, obj.m_logWindowFixedFontName);
            saver.AddFloat(SettingTag.Config_LogWindowFontSize, obj.m_logWindowFontSize);
            saver.AddFloat(SettingTag.Config_LogWindowTerminalFontSize, obj.m_logWindowTerminalFontSize);

            saver.EndObject(SettingTag.Config_Configuration);

            success = saver.SaveSetting(false);           
            obj.m_lastFileWriteTime = saver.LastFileWriteTime;
            return success;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(Configuration obj1, Configuration obj2) {
            // インストール情報＞全般
            if (obj1.m_temporaryDirectoryDefault != obj2.m_temporaryDirectoryDefault) {
                return false;
            }
            if (obj1.m_textEditorCommandLine != obj2.m_textEditorCommandLine) {
                return false;
            }
            if (obj1.m_textEditorCommandLineSSH != obj2.m_textEditorCommandLineSSH) {
                return false;
            }
            if (obj1.m_textEditorCommandLineWithLineNumber != obj2.m_textEditorCommandLineWithLineNumber) {
                return false;
            }
            if (obj1.m_textEditorCommandLineWithLineNumberSSH != obj2.m_textEditorCommandLineWithLineNumberSSH) {
                return false;
            }
            if (obj1.m_diffCommandLine != obj2.m_diffCommandLine) {
                return false;
            }
            if (obj1.m_diffDirectoryCommandLine != obj2.m_diffDirectoryCommandLine) {
                return false;
            }

            // ファイル一覧＞全般
            if (obj1.m_autoDirectoryUpdateWait != obj2.m_autoDirectoryUpdateWait) {
                return false;
            }
            if (obj1.m_refreshFileListTabChange != obj2.m_refreshFileListTabChange) {
                return false;
            }
            if (obj1.m_refreshFileListTabChangeSSH != obj2.m_refreshFileListTabChangeSSH) {
                return false;
            }

            // ファイル一覧＞起動時の状態
            if (obj1.m_initialDirectoryLeft != obj2.m_initialDirectoryLeft) {
                return false;
            }
            if (obj1.m_initialDirectoryRight != obj2.m_initialDirectoryRight) {
                return false;
            }
            if (obj1.m_mainWindowRectDefault != obj2.m_mainWindowRectDefault) {
                return false;
            }
            if (obj1.m_splashWindowWait != obj2.m_splashWindowWait) {
                return false;
            }

            // ファイル一覧＞起動時の一覧
            if (!FileListSortMode.EqualsConfig(obj1.m_defaultFileListSortModeLeft, obj2.m_defaultFileListSortModeLeft)) {
                return false;
            }
            if (!FileListSortMode.EqualsConfig(obj1.m_defaultFileListSortModeRight, obj2.m_defaultFileListSortModeRight)) {
                return false;
            }

            // ファイル一覧＞フォルダサイズの取得
            if (!RetrieveFolderSizeCondition.EqualsConfig(obj1.m_retrieveFolderSizeCondition, obj2.m_retrieveFolderSizeCondition)) {
                return false;
            }
            if (obj1.m_retrieveFolderSizeKeepLowerDepth != obj2.m_retrieveFolderSizeKeepLowerDepth) {
                return false;
            }
            if (obj1.m_retrieveFolderSizeKeepLowerCount != obj2.m_retrieveFolderSizeKeepLowerCount) {
                return false;
            }

            // ファイル一覧＞動作
            if (obj1.m_listViewScrollMarginLineCount != obj2.m_listViewScrollMarginLineCount) {
                return false;
            }
            if (obj1.m_mouseWheelMaxLines != obj2.m_mouseWheelMaxLines) {
                return false;
            }
            if (obj1.m_fileListDragMaxSpeed != obj2.m_fileListDragMaxSpeed) {
                return false;
            }
            if (obj1.m_fileListSeparateExt != obj2.m_fileListSeparateExt) {
                return false;
            }
            if (obj1.m_chdirParentOtherSideMove != obj2.m_chdirParentOtherSideMove) {
                return false;
            }
            if (obj1.m_hideWindowDragDrop != obj2.m_hideWindowDragDrop) {
                return false;
            }
            if (obj1.m_resumeFolderCursorFile != obj2.m_resumeFolderCursorFile) {
                return false;
            }

            // ファイル一覧＞起動時の表示モード
            if (!FileListViewMode.EqualsConfig(obj1.m_defaultViewModeLeft, obj2.m_defaultViewModeLeft)) {
                return false;
            }
            if (!FileListViewMode.EqualsConfig(obj1.m_defaultViewModeRight, obj2.m_defaultViewModeRight)) {
                return false;
            }

            // ファイル一覧＞表示モード
            if (!FileListViewMode.EqualsConfig(obj1.m_fileListViewChangeMode, obj2.m_fileListViewChangeMode)) {
                return false;
            }
            if (!FileListViewModeAutoSetting.EqualsConfig(obj1.m_fileListViewModeAutoSetting, obj2.m_fileListViewModeAutoSetting)) {
                return false;
            }

            // ファイル操作＞全般
            if (obj1.m_maxBackgroundTaskWaitableCountDefault != obj2.m_maxBackgroundTaskWaitableCountDefault) {
                return false;
            }
            if (obj1.m_maxBackgroundTaskLimitedCountDefault != obj2.m_maxBackgroundTaskLimitedCountDefault) {
                return false;
            }

            // ファイル操作＞転送と削除
            if (!SameFileOption.EqualsConfig(obj1.m_sameFileOptionDefault, obj2.m_sameFileOptionDefault)) {
                return false;
            }
            if (!DeleteFileOption.EqualsConfig(obj1.m_deleteFileOptionDefault, obj2.m_deleteFileOptionDefault)) {
                return false;
            }

            // ファイル操作＞属性のコピー
            if (!AttributeSetMode.EqualsConfig(obj1.m_transferAttributeSetMode, obj2.m_transferAttributeSetMode)) {
                return false;
            }

            // ファイル操作＞マークなし操作
            if (obj1.m_marklessCopy != obj2.m_marklessCopy) {
                return false;
            }
            if (obj1.m_marklessMove != obj2.m_marklessMove) {
                return false;
            }
            if (obj1.m_marklessDelete != obj2.m_marklessDelete) {
                return false;
            }
            if (obj1.m_marklessShortcut != obj2.m_marklessShortcut) {
                return false;
            }
            if (obj1.m_marklessAttribute != obj2.m_marklessAttribute) {
                return false;
            }
            if (obj1.m_marklessPack != obj2.m_marklessPack) {
                return false;
            }
            if (obj1.m_marklessUnpack != obj2.m_marklessUnpack) {
                return false;
            }
            if (obj1.m_marklessEdit != obj2.m_marklessEdit) {
                return false;
            }
            if (obj1.m_marklessFodlerSize != obj2.m_marklessFodlerSize) {
                return false;
            }

            // ファイル操作＞クリップボード
            if (!ClipboardCopyNameAsSetting.EqualsConfig(obj1.m_clipboardCopyNameAsSettingDefault, obj2.m_clipboardCopyNameAsSettingDefault)) {
                return false;
            }

            // ファイル操作＞一覧の比較
            if (!FileCompareSetting.EqualsConfig(obj1.m_fileCompareSettingDefault, obj2.m_fileCompareSettingDefault)) {
                return false;
            }

            // ファイル操作＞圧縮
            if (!ArchiveSetting.EqualsConfig(obj1.m_archiveSettingDefault, obj2.m_archiveSettingDefault)) {
                return false;
            }

            // ファイル操作＞展開
            if (obj1.m_archiveExtractPathMode != obj2.m_archiveExtractPathMode) {
                return false;
            }

            // ファイル操作＞各種操作
            if (!BooleanFlag.Equals(obj1.m_incrementalSearchFromHeadDefault, obj2.m_incrementalSearchFromHeadDefault)) {
                return false;
            }
            if (!BooleanFlag.Equals(obj1.m_makeDirectoryMoveCurrentDefault, obj2.m_makeDirectoryMoveCurrentDefault)) {
                return false;
            }
            if (obj1.m_makeDirectoryNewWindowsName != obj2.m_makeDirectoryNewWindowsName) {
                return false;
            }
            if (obj1.m_makeDirectoryNewSSHName != obj2.m_makeDirectoryNewSSHName) {
                return false;
            }
            if (obj1.m_shellExecuteReplayModeWindowsDefault != obj2.m_shellExecuteReplayModeWindowsDefault) {
                return false;
            }
            if (obj1.m_shellExecuteReplayModeSSHDefault != obj2.m_shellExecuteReplayModeSSHDefault) {
                return false;
            }
            if (obj1.m_mirrorCopyExceptFiles != obj2.m_mirrorCopyExceptFiles) {
                return false;
            }

            // SSH＞全般
            if (obj1.m_sshShortcutTypeDefault != obj2.m_sshShortcutTypeDefault) {
                return false;
            }
            if (obj1.m_sshFileSystemDefault != obj2.m_sshFileSystemDefault) {
                return false;
            }

            // SSH＞ターミナル
            if (obj1.m_terminalLogLineCount != obj2.m_terminalLogLineCount) {
                return false;
            }
            if (obj1.m_terminalShellCommandSSH != obj2.m_terminalShellCommandSSH) {
                return false;
            }
            if (obj1.m_terminalCloseConfirmMode != obj2.m_terminalCloseConfirmMode) {
                return false;
            }

            // SSH＞ターミナルログ
            if (obj1.m_terminalLogType != obj2.m_terminalLogType) {
                return false;
            }
            if (obj1.m_terminalLogMaxSize != obj2.m_terminalLogMaxSize) {
                return false;
            }
            if (obj1.m_terminalLogFileCount != obj2.m_terminalLogFileCount) {
                return false;
            }
            if (obj1.m_terminalLogOutputFolder != obj2.m_terminalLogOutputFolder) {
                return false;
            }

            // プライバシー＞全般
            if (obj1.m_commandHistoryMaxCountDefault != obj2.m_commandHistoryMaxCountDefault) {
                return false;
            }
            if (obj1.m_commandHistorySaveDisk != obj2.m_commandHistorySaveDisk) {
                return false;
            }
            if (obj1.m_pathHistoryMaxCountDefault != obj2.m_pathHistoryMaxCountDefault) {
                return false;
            }
            if (obj1.m_pathHistoryWholeMaxCountDefault != obj2.m_pathHistoryWholeMaxCountDefault) {
                return false;
            }
            if (obj1.m_pathHistoryWholeSaveDisk != obj2.m_pathHistoryWholeSaveDisk) {
                return false;
            }
            if (obj1.m_viewerSearchHistoryMaxCountDefault != obj2.m_viewerSearchHistoryMaxCountDefault) {
                return false;
            }
            if (obj1.m_viewerSearchHistorySaveDisk != obj2.m_viewerSearchHistorySaveDisk) {
                return false;
            }

            // テキストビューア＞全般
            if (obj1.m_textViewerMaxFileSize != obj2.m_textViewerMaxFileSize) {
                return false;
            }
            if (obj1.m_textViewerMaxLineCount != obj2.m_textViewerMaxLineCount) {
                return false;
            }
            if (!BooleanFlag.Equals(obj1.m_textViewerClearCompareBufferDefault, obj2.m_textViewerClearCompareBufferDefault)) {
                return false;
            }

            // テキストビューア＞表示
            if (obj1.m_textViewerIsDisplayLineNumber != obj2.m_textViewerIsDisplayLineNumber) {
                return false;
            }
            if (obj1.m_textViewerIsDisplayCtrlChar != obj2.m_textViewerIsDisplayCtrlChar) {
                return false;
            }

            // テキストビューア＞折返しとタブ
            if (!TextViewerLineBreakSetting.EqualsConfig(obj1.m_textViewerLineBreakDefault, obj2.m_textViewerLineBreakDefault)) {
                return false;
            }
            if (obj1.m_textViewerTab4Extension != obj2.m_textViewerTab4Extension) {
                return false;
            }

            // テキストビューア＞検索オプション
            if (!TextSearchOption.EqualsConfig(obj1.m_textSearchOptionDefault, obj2.m_textSearchOptionDefault)) {
                return false;
            }

            // テキストビューア＞クリップボード
            if (!TextClipboardSetting.EqualsConfig(obj1.m_textClipboardSettingDefault, obj2.m_textClipboardSettingDefault)) {
                return false;
            }
            if (!DumpClipboardSetting.EqualsConfig(obj1.m_dumpClipboardSettingDefault, obj2.m_dumpClipboardSettingDefault)) {
                return false;
            }

            // グラフィックビューア＞全般
            if (obj1.m_graphicsViewerMaxFileSize != obj2.m_graphicsViewerMaxFileSize) {
                return false;
            }
            if (obj1.m_graphicsViewerDragInertia != obj2.m_graphicsViewerDragInertia) {
                return false;
            }
            if (obj1.m_graphicsViewerDragBreaking != obj2.m_graphicsViewerDragBreaking) {
                return false;
            }
            if (obj1.m_graphicsViewerFilterMode != obj2.m_graphicsViewerFilterMode) {
                return false;
            }
            if (obj1.m_graphicsViewerFullScreenHideTimer != obj2.m_graphicsViewerFullScreenHideTimer) {
                return false;
            }
            if (obj1.m_graphicsViewerFullScreenAutoHideCursor != obj2.m_graphicsViewerFullScreenAutoHideCursor) {
                return false;
            }
            if (obj1.m_graphicsViewerFullScreenAutoHideInfo != obj2.m_graphicsViewerFullScreenAutoHideInfo) {
                return false;
            }
            if (obj1.m_graphicsViewerFullScreenHideInfoAlways != obj2.m_graphicsViewerFullScreenHideInfoAlways) {
                return false;
            }

            // グラフィックビューア＞拡大表示
            if (obj1.m_graphicsViewerAutoZoomMode != obj2.m_graphicsViewerAutoZoomMode) {
                return false;
            }
            if (obj1.m_graphicsViewerZoomInLarger != obj2.m_graphicsViewerZoomInLarger) {
                return false;
            }

            // ファンクションキー＞全般
            if (obj1.m_functionBarSplitCount != obj2.m_functionBarSplitCount) {
                return false;
            }
            if (obj1.m_functionBarUseOverrayIcon != obj2.m_functionBarUseOverrayIcon) {
                return false;
            }

            // ログ＞全般
            if (obj1.m_logLineMaxCountDefault != obj2.m_logLineMaxCountDefault) {
                return false;
            }

            // 色とフォント＞ファイル一覧
            if (obj1.m_fileListBackColor != obj2.m_fileListBackColor) {
                return false;
            }
            if (obj1.m_fileListFileTextColor != obj2.m_fileListFileTextColor) {
                return false;
            }
            if (obj1.m_fileListReadOnlyColor != obj2.m_fileListReadOnlyColor) {
                return false;
            }
            if (obj1.m_fileListHiddenColor != obj2.m_fileListHiddenColor) {
                return false;
            }
            if (obj1.m_fileListSystemColor != obj2.m_fileListSystemColor) {
                return false;
            }
            if (obj1.m_fileListArchiveColor != obj2.m_fileListArchiveColor) {
                return false;
            }
            if (obj1.m_fileListSymlinkColor != obj2.m_fileListSymlinkColor) {
                return false;
            }
            if (obj1.m_fileListGrayColor != obj2.m_fileListGrayColor) {
                return false;
            }
            if (obj1.m_fileListMarkColor != obj2.m_fileListMarkColor) {
                return false;
            }
            if (obj1.m_fileListMarkBackColor1 != obj2.m_fileListMarkBackColor1) {
                return false;
            }
            if (obj1.m_fileListMarkBackColor2 != obj2.m_fileListMarkBackColor2) {
                return false;
            }
            if (obj1.m_fileListMarkBackBorderColor != obj2.m_fileListMarkBackBorderColor) {
                return false;
            }
            if (obj1.m_fileListGrayBackColor != obj2.m_fileListGrayBackColor) {
                return false;
            }
            if (obj1.m_fileListMarkGrayColor != obj2.m_fileListMarkGrayColor) {
                return false;
            }
            if (obj1.m_fileListMarkGrayBackColor1 != obj2.m_fileListMarkGrayBackColor1) {
                return false;
            }
            if (obj1.m_fileListMarkGrayBackColor2 != obj2.m_fileListMarkGrayBackColor2) {
                return false;
            }
            if (obj1.m_fileListStatusBarSuperUserColor != obj2.m_fileListStatusBarSuperUserColor) {
                return false;
            }
            if (obj1.m_fileListThumbnailFrameColor1 != obj2.m_fileListThumbnailFrameColor1) {
                return false;
            }
            if (obj1.m_fileListThumbnailFrameColor2 != obj2.m_fileListThumbnailFrameColor2) {
                return false;
            }
            if (obj1.m_fileListCursorColor != obj2.m_fileListCursorColor) {
                return false;
            }
            if (obj1.m_fileListCursorDisableColor != obj2.m_fileListCursorDisableColor) {
                return false;
            }
            if (obj1.m_fileListMarkGrayBackBorderColor != obj2.m_fileListMarkGrayBackBorderColor) {
                return false;
            }
            if (obj1.m_dialogErrorBackColor != obj2.m_dialogErrorBackColor) {
                return false;
            }
            if (obj1.m_dialogErrorTextColor != obj2.m_dialogErrorTextColor) {
                return false;
            }
            if (obj1.m_dialogWarningBackColor != obj2.m_dialogWarningBackColor) {
                return false;
            }
            if (obj1.m_dialogWarningTextColor != obj2.m_dialogWarningTextColor) {
                return false;
            }

            // 色とフォント＞テキストビューア
            if (obj1.m_textViewerErrorBackColor != obj2.m_textViewerErrorBackColor) {
                return false;
            }
            if (obj1.m_textViewerErrorStatusBackColor != obj2.m_textViewerErrorStatusBackColor) {
                return false;
            }
            if (obj1.m_textViewerErrorStatusTextColor != obj2.m_textViewerErrorStatusTextColor) {
                return false;
            }
            if (obj1.m_textViewerInfoStatusBackColor != obj2.m_textViewerInfoStatusBackColor) {
                return false;
            }
            if (obj1.m_textViewerInfoStatusTextColor != obj2.m_textViewerInfoStatusTextColor) {
                return false;
            }
            if (obj1.m_textViewerLineNoTextColor != obj2.m_textViewerLineNoTextColor) {
                return false;
            }
            if (obj1.m_textViewerLineNoBackColor1 != obj2.m_textViewerLineNoBackColor1) {
                return false;
            }
            if (obj1.m_textViewerLineNoBackColor2 != obj2.m_textViewerLineNoBackColor2) {
                return false;
            }
            if (obj1.m_textViewerLineNoSeparatorColor != obj2.m_textViewerLineNoSeparatorColor) {
                return false;
            }
            if (obj1.m_textViewerOutOfAreaBackColor != obj2.m_textViewerOutOfAreaBackColor) {
                return false;
            }
            if (obj1.m_textViewerOutOfAreaSeparatorColor != obj2.m_textViewerOutOfAreaSeparatorColor) {
                return false;
            }
            if (obj1.m_textViewerSearchCursorColor != obj2.m_textViewerSearchCursorColor) {
                return false;
            }
            if (obj1.m_textViewerControlColor != obj2.m_textViewerControlColor) {
                return false;
            }
            if (obj1.m_textViewerTextColor != obj2.m_textViewerTextColor) {
                return false;
            }
            if (obj1.m_textViewerSelectTextColor != obj2.m_textViewerSelectTextColor) {
                return false;
            }
            if (obj1.m_textViewerSelectTextColor2 != obj2.m_textViewerSelectTextColor2) {
                return false;
            }
            if (obj1.m_textViewerSelectBackColor != obj2.m_textViewerSelectBackColor) {
                return false;
            }
            if (obj1.m_textViewerSelectBackColor2 != obj2.m_textViewerSelectBackColor2) {
                return false;
            }
            if (obj1.m_textViewerSearchHitBackColor != obj2.m_textViewerSearchHitBackColor) {
                return false;
            }
            if (obj1.m_textViewerSearchHitTextColor != obj2.m_textViewerSearchHitTextColor) {
                return false;
            }
            if (obj1.m_textViewerSearchAutoTextColor != obj2.m_textViewerSearchAutoTextColor) {
                return false;
            }
            if (obj1.m_radarBarBackColor1 != obj2.m_radarBarBackColor1) {
                return false;
            }
            if (obj1.m_radarBarBackColor2 != obj2.m_radarBarBackColor2) {
                return false;
            }

            // 色とフォント＞グラフィックビューア
            if (obj1.m_graphicsViewerBackColor != obj2.m_graphicsViewerBackColor) {
                return false;
            }
            if (obj1.m_graphicsViewerTextColor != obj2.m_graphicsViewerTextColor) {
                return false;
            }
            if (obj1.m_graphicsViewerTextShadowColor != obj2.m_graphicsViewerTextShadowColor) {
                return false;
            }
            if (obj1.m_graphicsViewerLoadingTextColor != obj2.m_graphicsViewerLoadingTextColor) {
                return false;
            }
            if (obj1.m_graphicsViewerLoadingTextShadowColor != obj2.m_graphicsViewerLoadingTextShadowColor) {
                return false;
            }

            // 色とフォント＞ログ
            if (obj1.m_logWindowTextColor != obj2.m_logWindowTextColor) {
                return false;
            }
            if (obj1.m_logWindowLinkTextColor != obj2.m_logWindowLinkTextColor) {
                return false;
            }
            if (obj1.m_logErrorTextColor != obj2.m_logErrorTextColor) {
                return false;
            }
            if (obj1.m_logStdErrorTextColor != obj2.m_logStdErrorTextColor) {
                return false;
            }
            if (obj1.m_logWindowSelectTextColor != obj2.m_logWindowSelectTextColor) {
                return false;
            }
            if (obj1.m_logWindowBackColor != obj2.m_logWindowBackColor) {
                return false;
            }
            if (obj1.m_logWindowSelectBackColor != obj2.m_logWindowSelectBackColor) {
                return false;
            }
            if (obj1.m_logWindowBackBellColor != obj2.m_logWindowBackBellColor) {
                return false;
            }
            if (obj1.m_logWindowBackClosedColor != obj2.m_logWindowBackClosedColor) {
                return false;
            }
            if (obj1.m_logWindowProgressColor1 != obj2.m_logWindowProgressColor1) {
                return false;
            }
            if (obj1.m_logWindowProgressColor2 != obj2.m_logWindowProgressColor2) {
                return false;
            }
            if (obj1.m_logWindowProgressColor3 != obj2.m_logWindowProgressColor3) {
                return false;
            }
            if (obj1.m_logWindowProgressColor4 != obj2.m_logWindowProgressColor4) {
                return false;
            }
            if (obj1.m_logWindowRemainingTimeTextColor1 != obj2.m_logWindowRemainingTimeTextColor1) {
                return false;
            }
            if (obj1.m_logWindowRemainingTimeTextColor2 != obj2.m_logWindowRemainingTimeTextColor2) {
                return false;
            }

            // 色とフォント＞フォント
            if (obj1.m_listViewFontName != obj2.m_listViewFontName) {
                return false;
            }
            if (obj1.m_listViewFontSize != obj2.m_listViewFontSize) {
                return false;
            }
            if (obj1.m_defaultFileListViewHeight != obj2.m_defaultFileListViewHeight) {
                return false;
            }
            if (obj1.m_thumbFileListViewFontName != obj2.m_thumbFileListViewFontName) {
                return false;
            }
            if (obj1.m_thumbFileListViewFontSize != obj2.m_thumbFileListViewFontSize) {
                return false;
            }
            if (obj1.m_thumbFileListViewSmallFontSize != obj2.m_thumbFileListViewSmallFontSize) {
                return false;
            }
            if (obj1.m_textFontName != obj2.m_textFontName) {
                return false;
            }
            if (obj1.m_textFontSize != obj2.m_textFontSize) {
                return false;
            }
            if (obj1.m_textFileViewerLineHeight != obj2.m_textFileViewerLineHeight) {
                return false;
            }
            if (obj1.m_functionBarFontName != obj2.m_functionBarFontName) {
                return false;
            }
            if (obj1.m_functionBarFontSize != obj2.m_functionBarFontSize) {
                return false;
            }
            if (obj1.m_logWindowFontName != obj2.m_logWindowFontName) {
                return false;
            }
            if (obj1.m_logWindowFixedFontName != obj2.m_logWindowFixedFontName) {
                return false;
            }
            if (obj1.m_logWindowFontSize != obj2.m_logWindowFontSize) {
                return false;
            }
            if (obj1.m_logWindowTerminalFontSize != obj2.m_logWindowTerminalFontSize) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            Configuration clone = new Configuration();
            // インストール情報＞全般
            clone.m_temporaryDirectoryDefault = m_temporaryDirectoryDefault;
            clone.m_textEditorCommandLine = m_textEditorCommandLine;
            clone.m_textEditorCommandLineSSH = m_textEditorCommandLineSSH;
            clone.m_textEditorCommandLineWithLineNumber = m_textEditorCommandLineWithLineNumber;
            clone.m_textEditorCommandLineWithLineNumberSSH = m_textEditorCommandLineWithLineNumberSSH;
            clone.m_diffCommandLine = m_diffCommandLine;
            clone.m_diffDirectoryCommandLine = m_diffDirectoryCommandLine;

            // ファイル一覧＞全般
            clone.m_autoDirectoryUpdateWait = m_autoDirectoryUpdateWait;
            clone.m_refreshFileListTabChange = m_refreshFileListTabChange;
            clone.m_refreshFileListTabChangeSSH = m_refreshFileListTabChangeSSH;

            // ファイル一覧＞起動時の状態
            clone.m_initialDirectoryLeft = m_initialDirectoryLeft;
            clone.m_initialDirectoryRight = m_initialDirectoryRight;
            if (m_defaultFileListSortModeLeft != null) {
                clone.m_defaultFileListSortModeLeft = (FileListSortMode)(m_defaultFileListSortModeLeft.Clone());
            } else {
                clone.m_defaultFileListSortModeLeft = null;
            }
            if (m_defaultFileListSortModeRight != null) {
                clone.m_defaultFileListSortModeRight = (FileListSortMode)(m_defaultFileListSortModeRight.Clone());
            } else {
                clone.m_defaultFileListSortModeRight = null;
            }
            clone.m_mainWindowRectDefault = m_mainWindowRectDefault;
            clone.m_splashWindowWait = m_splashWindowWait;

            // ファイル一覧＞フォルダサイズの取得
            if (m_retrieveFolderSizeCondition != null) {
                clone.m_retrieveFolderSizeCondition = (RetrieveFolderSizeCondition)(m_retrieveFolderSizeCondition.Clone());
            } else {
                clone.m_retrieveFolderSizeCondition = null;
            }
            clone.m_retrieveFolderSizeKeepLowerDepth = m_retrieveFolderSizeKeepLowerDepth;
            clone.m_retrieveFolderSizeKeepLowerCount = m_retrieveFolderSizeKeepLowerCount;

            // ファイル一覧＞動作
            clone.m_listViewScrollMarginLineCount = m_listViewScrollMarginLineCount;
            clone.m_mouseWheelMaxLines = m_mouseWheelMaxLines;
            clone.m_fileListDragMaxSpeed = m_fileListDragMaxSpeed;
            clone.m_fileListSeparateExt = m_fileListSeparateExt;
            clone.m_chdirParentOtherSideMove = m_chdirParentOtherSideMove;
            clone.m_hideWindowDragDrop = m_hideWindowDragDrop;
            clone.m_resumeFolderCursorFile = m_resumeFolderCursorFile;

            // ファイル一覧＞サムネイル
            if (m_defaultViewModeLeft != null) {
                clone.m_defaultViewModeLeft = (FileListViewMode)(m_defaultViewModeLeft.Clone());
            } else {
                clone.m_defaultViewModeLeft = null;
            }
            if (m_defaultViewModeRight != null) {
                clone.m_defaultViewModeRight = (FileListViewMode)(m_defaultViewModeRight.Clone());
            } else {
                clone.m_defaultViewModeRight = null;
            }

            // ファイル一覧＞表示モード
            if (m_fileListViewChangeMode != null) {
                clone.m_fileListViewChangeMode = (FileListViewMode)(m_fileListViewChangeMode.Clone());
            } else {
                clone.m_fileListViewChangeMode = null;
            }
            if (m_fileListViewModeAutoSetting != null) {
                clone.m_fileListViewModeAutoSetting = (FileListViewModeAutoSetting)(m_fileListViewModeAutoSetting.Clone());
            } else {
                clone.m_fileListViewModeAutoSetting = null;
            }

            // ファイル操作＞全般
            clone.m_maxBackgroundTaskWaitableCountDefault = m_maxBackgroundTaskWaitableCountDefault;
            clone.m_maxBackgroundTaskLimitedCountDefault = m_maxBackgroundTaskLimitedCountDefault;

            // ファイル操作＞転送と削除
            if (m_sameFileOptionDefault != null) {
                clone.m_sameFileOptionDefault = (SameFileOption)(m_sameFileOptionDefault.Clone());
            } else {
                clone.m_sameFileOptionDefault = null;
            }
            if (m_deleteFileOptionDefault != null) {
                clone.m_deleteFileOptionDefault = (DeleteFileOption)(m_deleteFileOptionDefault.Clone());
            } else {
                clone.m_deleteFileOptionDefault = null;
            }

            // ファイル操作＞属性のコピー
            clone.m_transferAttributeSetMode = (AttributeSetMode)(m_transferAttributeSetMode.Clone());

            // ファイル操作＞マークなし操作
            clone.m_marklessCopy = m_marklessCopy;
            clone.m_marklessMove = m_marklessMove;
            clone.m_marklessDelete = m_marklessDelete;
            clone.m_marklessShortcut = m_marklessShortcut;
            clone.m_marklessAttribute = m_marklessAttribute;
            clone.m_marklessPack = m_marklessPack;
            clone.m_marklessUnpack = m_marklessUnpack;
            clone.m_marklessEdit = m_marklessEdit;
            clone.m_marklessFodlerSize = m_marklessFodlerSize;

            // ファイル操作＞クリップボード
            if (m_clipboardCopyNameAsSettingDefault != null) {
                clone.m_clipboardCopyNameAsSettingDefault = (ClipboardCopyNameAsSetting)(m_clipboardCopyNameAsSettingDefault.Clone());
            } else {
                clone.m_clipboardCopyNameAsSettingDefault = null;
            }

            // ファイル操作＞一覧の比較
            if (m_fileCompareSettingDefault != null) {
                clone.m_fileCompareSettingDefault = (FileCompareSetting)(m_fileCompareSettingDefault.Clone());
            } else {
                clone.m_fileCompareSettingDefault = null;
            }

            // ファイル操作＞圧縮
            if (m_archiveSettingDefault != null) {
                clone.m_archiveSettingDefault = (ArchiveSetting)(m_archiveSettingDefault.Clone());
            } else {
                m_archiveSettingDefault = null;
            }

            // ファイル操作＞展開
            clone.m_archiveExtractPathMode = m_archiveExtractPathMode;

            // ファイル操作＞各種操作
            if (m_incrementalSearchFromHeadDefault != null) {
                clone.m_incrementalSearchFromHeadDefault = (BooleanFlag)(m_incrementalSearchFromHeadDefault.Clone());
            } else {
                clone.m_incrementalSearchFromHeadDefault = null;
            }
            if (m_makeDirectoryMoveCurrentDefault != null) {
                clone.m_makeDirectoryMoveCurrentDefault = (BooleanFlag)(m_makeDirectoryMoveCurrentDefault.Clone());
            } else {
                clone.m_makeDirectoryMoveCurrentDefault = null;
            }
            clone.m_makeDirectoryNewWindowsName = m_makeDirectoryNewWindowsName;
            clone.m_makeDirectoryNewSSHName = m_makeDirectoryNewSSHName;
            if (m_shellExecuteReplayModeWindowsDefault != null) {
                clone.m_shellExecuteReplayModeWindowsDefault = m_shellExecuteReplayModeWindowsDefault;
            } else {
                clone.m_shellExecuteReplayModeWindowsDefault = null;
            }
            if (m_shellExecuteReplayModeSSHDefault != null) {
                clone.m_shellExecuteReplayModeSSHDefault = m_shellExecuteReplayModeSSHDefault;
            } else {
                clone.m_shellExecuteReplayModeSSHDefault = null;
            }
            clone.m_mirrorCopyExceptFiles = m_mirrorCopyExceptFiles;

            // SSH＞全般
            clone.m_sshShortcutTypeDefault = m_sshShortcutTypeDefault;
            clone.m_sshFileSystemDefault = m_sshFileSystemDefault;

            // SSH＞ターミナル
            clone.m_terminalLogLineCount = m_terminalLogLineCount;
            clone.m_terminalShellCommandSSH = m_terminalShellCommandSSH;
            clone.m_terminalCloseConfirmMode = m_terminalCloseConfirmMode;

            // SSH＞ターミナルログ
            clone.m_terminalLogType = m_terminalLogType;
            clone.m_terminalLogMaxSize = m_terminalLogMaxSize;
            clone.m_terminalLogFileCount = m_terminalLogFileCount;
            clone.m_terminalLogOutputFolder = m_terminalLogOutputFolder;

            // プライバシー＞全般
            clone.m_commandHistoryMaxCountDefault = m_commandHistoryMaxCountDefault;
            clone.m_commandHistorySaveDisk = m_commandHistorySaveDisk;
            clone.m_pathHistoryMaxCountDefault = m_pathHistoryMaxCountDefault;
            clone.m_pathHistoryWholeMaxCountDefault = m_pathHistoryWholeMaxCountDefault;

            // テキストビューア＞全般
            clone.m_textViewerMaxFileSize = m_textViewerMaxFileSize;
            clone.m_textViewerMaxLineCount = m_textViewerMaxLineCount;
            clone.m_textViewerClearCompareBufferDefault = m_textViewerClearCompareBufferDefault;

            // テキストビューア＞表示
            clone.m_textViewerIsDisplayLineNumber = m_textViewerIsDisplayLineNumber;
            clone.m_textViewerIsDisplayCtrlChar = m_textViewerIsDisplayCtrlChar;

            // テキストビューア＞折返しとタブ
            if (m_textViewerLineBreakDefault != null) {
                clone.m_textViewerLineBreakDefault = (TextViewerLineBreakSetting)(m_textViewerLineBreakDefault.Clone());
            } else {
                clone.m_textViewerLineBreakDefault = null;
            }
            clone.m_textViewerTab4Extension = m_textViewerTab4Extension;

            // テキストビューア＞検索オプション
            if (m_textSearchOptionDefault != null) {
                clone.m_textSearchOptionDefault = (TextSearchOption)(m_textSearchOptionDefault.Clone());
            } else {
                clone.m_textSearchOptionDefault = null;
            }

            // テキストビューア＞クリップボード
            if (m_textClipboardSettingDefault != null) {
                clone.m_textClipboardSettingDefault = (TextClipboardSetting)(m_textClipboardSettingDefault.Clone());
            } else {
                clone.m_textClipboardSettingDefault = null;
            }
            if (m_dumpClipboardSettingDefault != null) {
                clone.m_dumpClipboardSettingDefault = (DumpClipboardSetting)(m_dumpClipboardSettingDefault.Clone());
            } else {
                clone.m_dumpClipboardSettingDefault = null;
            }

            // グラフィックビューア＞全般
            clone.m_graphicsViewerMaxFileSize = m_graphicsViewerMaxFileSize;
            clone.m_graphicsViewerDragInertia = m_graphicsViewerDragInertia;
            clone.m_graphicsViewerDragBreaking = m_graphicsViewerDragBreaking;
            clone.m_graphicsViewerFilterMode = m_graphicsViewerFilterMode;
            clone.m_graphicsViewerFullScreenHideTimer = m_graphicsViewerFullScreenHideTimer;
            clone.m_graphicsViewerFullScreenAutoHideCursor = m_graphicsViewerFullScreenAutoHideCursor;
            clone.m_graphicsViewerFullScreenAutoHideInfo = m_graphicsViewerFullScreenAutoHideInfo;
            clone.m_graphicsViewerFullScreenHideInfoAlways = m_graphicsViewerFullScreenHideInfoAlways;

            // グラフィックビューア＞拡大表示
            clone.m_graphicsViewerAutoZoomMode = m_graphicsViewerAutoZoomMode;
            clone.m_graphicsViewerZoomInLarger = m_graphicsViewerZoomInLarger;

            // ファンクションキー＞全般
            clone.m_functionBarSplitCount = m_functionBarSplitCount;
            clone.m_functionBarUseOverrayIcon = m_functionBarUseOverrayIcon;

            // ログ＞全般
            clone.m_logLineMaxCountDefault = m_logLineMaxCountDefault;

            // 色とフォント＞ファイル一覧
            clone.m_fileListBackColor = m_fileListBackColor;
            clone.m_fileListFileTextColor = m_fileListFileTextColor;
            clone.m_fileListReadOnlyColor = m_fileListReadOnlyColor;
            clone.m_fileListHiddenColor = m_fileListHiddenColor;
            clone.m_fileListSystemColor = m_fileListSystemColor;
            clone.m_fileListArchiveColor = m_fileListArchiveColor;
            clone.m_fileListSymlinkColor = m_fileListSymlinkColor;
            clone.m_fileListGrayColor = m_fileListGrayColor;
            clone.m_fileListMarkColor = m_fileListMarkColor;
            clone.m_fileListMarkBackColor1 = m_fileListMarkBackColor1;
            clone.m_fileListMarkBackColor2 = m_fileListMarkBackColor2;
            clone.m_fileListMarkBackBorderColor = m_fileListMarkBackBorderColor;
            clone.m_fileListGrayBackColor = m_fileListGrayBackColor;
            clone.m_fileListMarkGrayColor = m_fileListMarkGrayColor;
            clone.m_fileListMarkGrayBackColor1 = m_fileListMarkGrayBackColor1;
            clone.m_fileListMarkGrayBackColor2 = m_fileListMarkGrayBackColor2;
            clone.m_fileListMarkGrayBackBorderColor = m_fileListMarkGrayBackBorderColor;
            clone.m_fileListCursorColor = m_fileListCursorColor;
            clone.m_fileListCursorDisableColor = m_fileListCursorDisableColor;
            clone.m_fileListStatusBarSuperUserColor = m_fileListStatusBarSuperUserColor;
            clone.m_fileListThumbnailFrameColor1 = m_fileListThumbnailFrameColor1;
            clone.m_fileListThumbnailFrameColor2 = m_fileListThumbnailFrameColor2;
            clone.m_dialogErrorBackColor = m_dialogErrorBackColor;
            clone.m_dialogErrorTextColor = m_dialogErrorTextColor;
            clone.m_dialogWarningBackColor = m_dialogWarningBackColor;
            clone.m_dialogWarningTextColor = m_dialogWarningTextColor;

            // 色とフォント＞グラフィックビューア
            clone.m_graphicsViewerBackColor = m_graphicsViewerBackColor;
            clone.m_graphicsViewerTextColor = m_graphicsViewerTextColor;
            clone.m_graphicsViewerTextShadowColor = m_graphicsViewerTextShadowColor;
            clone.m_graphicsViewerLoadingTextColor = m_graphicsViewerLoadingTextColor;
            clone.m_graphicsViewerLoadingTextShadowColor = m_graphicsViewerLoadingTextShadowColor;

            // 色とフォント＞テキストビューア
            clone.m_textViewerErrorBackColor = m_textViewerErrorBackColor;
            clone.m_textViewerErrorStatusBackColor = m_textViewerErrorStatusBackColor;
            clone.m_textViewerErrorStatusTextColor = m_textViewerErrorStatusTextColor;
            clone.m_textViewerInfoStatusBackColor = m_textViewerInfoStatusBackColor;
            clone.m_textViewerInfoStatusTextColor = m_textViewerInfoStatusTextColor;
            clone.m_textViewerLineNoTextColor = m_textViewerLineNoTextColor;
            clone.m_textViewerLineNoBackColor1 = m_textViewerLineNoBackColor1;
            clone.m_textViewerLineNoBackColor2 = m_textViewerLineNoBackColor2;
            clone.m_textViewerLineNoSeparatorColor = m_textViewerLineNoSeparatorColor;
            clone.m_textViewerOutOfAreaBackColor = m_textViewerOutOfAreaBackColor;
            clone.m_textViewerOutOfAreaSeparatorColor = m_textViewerOutOfAreaSeparatorColor;
            clone.m_textViewerSearchCursorColor = m_textViewerSearchCursorColor;
            clone.m_textViewerControlColor = m_textViewerControlColor;
            clone.m_textViewerTextColor = m_textViewerTextColor;
            clone.m_textViewerSelectTextColor = m_textViewerSelectTextColor;
            clone.m_textViewerSelectTextColor2 = m_textViewerSelectTextColor2;
            clone.m_textViewerSelectBackColor = m_textViewerSelectBackColor;
            clone.m_textViewerSelectBackColor2 = m_textViewerSelectBackColor2;
            clone.m_textViewerSearchHitBackColor = m_textViewerSearchHitBackColor;
            clone.m_textViewerSearchHitTextColor = m_textViewerSearchHitTextColor;
            clone.m_textViewerSearchAutoTextColor = m_textViewerSearchAutoTextColor;
            clone.m_radarBarBackColor1 = m_radarBarBackColor1;
            clone.m_radarBarBackColor2 = m_radarBarBackColor2;

            // 色とフォント＞ログ
            clone.m_logWindowTextColor = m_logWindowTextColor;
            clone.m_logWindowLinkTextColor = m_logWindowLinkTextColor;
            clone.m_logErrorTextColor = m_logErrorTextColor;
            clone.m_logStdErrorTextColor = m_logStdErrorTextColor;
            clone.m_logWindowSelectTextColor = m_logWindowSelectTextColor;
            clone.m_logWindowBackColor = m_logWindowBackColor;
            clone.m_logWindowSelectBackColor = m_logWindowSelectBackColor;
            clone.m_logWindowBackBellColor = m_logWindowBackBellColor;
            clone.m_logWindowBackClosedColor = m_logWindowBackClosedColor;
            clone.m_logWindowProgressColor1 = m_logWindowProgressColor1;
            clone.m_logWindowProgressColor2 = m_logWindowProgressColor2;
            clone.m_logWindowProgressColor3 = m_logWindowProgressColor3;
            clone.m_logWindowProgressColor4 = m_logWindowProgressColor4;
            clone.m_logWindowRemainingTimeTextColor1 = m_logWindowRemainingTimeTextColor1;
            clone.m_logWindowRemainingTimeTextColor2 = m_logWindowRemainingTimeTextColor2;

            // 色とフォント＞フォント
            clone.m_listViewFontName = m_listViewFontName;
            clone.m_listViewFontSize = m_listViewFontSize;
            clone.m_defaultFileListViewHeight = m_defaultFileListViewHeight;
            clone.m_thumbFileListViewFontName = m_thumbFileListViewFontName;
            clone.m_thumbFileListViewFontSize = m_thumbFileListViewFontSize;
            clone.m_thumbFileListViewSmallFontSize = m_thumbFileListViewSmallFontSize;
            clone.m_textFontName = m_textFontName;
            clone.m_textFontSize = m_textFontSize;
            clone.m_textFileViewerLineHeight = m_textFileViewerLineHeight;
            clone.m_functionBarFontName = m_functionBarFontName;
            clone.m_functionBarFontSize = m_functionBarFontSize;
            clone.m_logWindowFontName = m_logWindowFontName;
            clone.m_logWindowFixedFontName = m_logWindowFixedFontName;
            clone.m_logWindowFontSize = m_logWindowFontSize;
            clone.m_logWindowTerminalFontSize = m_logWindowTerminalFontSize;

            return clone;
        }

        //=========================================================================================
        // 機　能：フォント名を補完する
        // 引　数：[in]fontName  フォント名
        // 戻り値：フォント名
        //=========================================================================================
        private string CompleteFontName(string fontName) {
            if (fontName == "") {
                return s_windowsDefaultFontName;
            } else {
                return fontName;
            }
        }
        
        //=========================================================================================
        // 機　能：コンフィグ中にエラーが見つかったときの処理を行う
        // 引　数：[in]uiParent  メッセージボックスの親になるフォーム（null:メッセージを表示しない）
        // 　　　　[in]message   メッセージ
        // 　　　　[in]arg       メッセージに対する引数
        // 戻り値：常にfalse
        //=========================================================================================
        private static bool ConfigCheckError(Form uiParent, string message, params object[] arg) {
            if (uiParent != null) {
                InfoBox.Warning(uiParent, message, arg);
            }
            return false;
        }

        //=========================================================================================
        // プロパティ：読み込み済みファイルの書き込み日時
        //=========================================================================================
        public DateTime LastFileWriteTime {
            get {
                return m_lastFileWriteTime;
            }
            set {
                m_lastFileWriteTime = value;
            }
        }

        //=========================================================================================
        // プロパティ：書き込み済みファイルの書き込み日時
        //=========================================================================================
        public static DateTime SavedConfigLastWriteTime {
            get {
                DateTime lastWrite = DateTime.MinValue;
#if !FREE_VERSION
                try {
                    string fileName = DirectoryManager.ConfigurationSetting;
                    if (File.Exists(fileName)) {
                        FileInfo file = new FileInfo(fileName);
                        lastWrite = file.LastWriteTime;
                    }
                } catch (Exception) {
                    // MinValueのまま
                }
#endif
                return lastWrite;
            }
        }

        //=========================================================================================
        // プロパティ：無料版のときtrue
        //=========================================================================================
        public bool FreewareVersion {
            get {
                return m_freewareVersion;
            }
        }

//*****************************************************************************************
// インストール情報＞全般
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：作業ディレクトリ（自動設定時は""、次回起動後に有効）
        //=========================================================================================
        public string TemporaryDirectoryDefault {
            get {
                return m_temporaryDirectoryDefault;
            }
            set {
                m_temporaryDirectoryDefault = value;
            }
        }

        public static bool CheckTemporaryDirectoryDefault(ref string val, Form uiParent) {
            if (val == null) {
                return false;
            } else if (val == "") {
                return true;
            } else {
                val = GenericFileStringUtils.CompleteDirectoryName(val, "\\");
                bool exist = Directory.Exists(val);
                if (!exist) {
                    return ConfigCheckError(uiParent, Resources.Option_TemporaryDirectoryDefaultPath);
                }
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：エディタのコマンドライン（{0}にファイル名、.txtの関連付け使用時は""）
        //=========================================================================================
        public string TextEditorCommandLine {
            get {
                return m_textEditorCommandLine;
            }
            set {
                m_textEditorCommandLine = value;
            }
        }

        //=========================================================================================
        // プロパティ：SSH用エディタのコマンドライン（{0}にファイル名、通常ファイルと共通の場合は""）
        //=========================================================================================
        public string TextEditorCommandLineSSH {
            get {
                return m_textEditorCommandLineSSH;
            }
            set {
                m_textEditorCommandLineSSH = value;
            }
        }

        public static bool CheckTextEditorCommandLine(ref string val, Form uiParent) {
            if (val == null) {
                return false;
            } else if (val == "") {
                return true;
            } else {
                // {0}が1つだけか？
                int[] idxParam = StringUtils.SearchAllPosition(val, "{0}");
                int[] idxParL = StringUtils.SearchAllPosition(val, "{");
                int[] idxParR = StringUtils.SearchAllPosition(val, "}");
                if (idxParam.Length != 1 || idxParL.Length != 1 || idxParR.Length != 1) {
                    return ConfigCheckError(uiParent, Resources.Option_TextEditorCommandLineParam);
                }
                // A-Zと\が、{0}の前にあるか？
                int idxAlpha = StringUtils.IndexOfRangeIgnoreCase(val, 'A', 'Z');
                int idxPath = val.IndexOf('\\');
                if (idxAlpha == -1 || idxAlpha > idxParam[0] || idxPath > idxParam[0]) {
                    return ConfigCheckError(uiParent, Resources.Option_TextEditorCommandLineExe);
                }
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：行番号指定のエディタのコマンドライン（{0}にファイル名、{1}に行番号、TextEditorCommandLine使用時は""）
        //=========================================================================================
        public string TextEditorCommandLineWithLineNumber {
            get {
                return m_textEditorCommandLineWithLineNumber;
            }
            set {
                m_textEditorCommandLineWithLineNumber = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：SSH用行番号指定のエディタのコマンドライン（{0}にファイル名、{1}に行番号、通常ファイルと共通の場合は""）
        //=========================================================================================
        public string TextEditorCommandLineWithLineNumberSSH {
            get {
                return m_textEditorCommandLineWithLineNumberSSH;
            }
            set {
                m_textEditorCommandLineWithLineNumberSSH = value;
            }
        }

        public static bool CheckTextEditorCommandLineWithLineNumber(ref string val, Form uiParent) {
            if (val == null) {
                return false;
            } else if (val == "") {
                return true;
            } else {
                // {0}が1つだけか？
                int[] idxParam = StringUtils.SearchAllPosition(val, "{0}");
                int[] idxParam2 = StringUtils.SearchAllPosition(val, "{1}");
                int[] idxParL = StringUtils.SearchAllPosition(val, "{");
                int[] idxParR = StringUtils.SearchAllPosition(val, "}");
                if (idxParam.Length != 1 || idxParam2.Length != 1 || idxParL.Length != 2 || idxParR.Length != 2) {
                    return ConfigCheckError(uiParent, Resources.Option_TextEditorCommandLineParamWithLineNumber);
                }
                // A-Zと\が、{0}の前にあるか？
                int idxAlpha = StringUtils.IndexOfRangeIgnoreCase(val, 'A', 'Z');
                int idxPath = val.IndexOf('\\');
                if (idxAlpha == -1 || idxAlpha > idxParam[0] || idxPath > idxParam[0]) {
                    return ConfigCheckError(uiParent, Resources.Option_TextEditorCommandLineExeWithLineNumber);
                }
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：差分表示ツールのコマンドライン（{0}にファイル名、未定義のとき""）
        //=========================================================================================
        public string DiffCommandLine {
            get {
                return m_diffCommandLine;
            }
            set {
                m_diffCommandLine = value;
            }
        }

        public static bool CheckDiffCommandLine(ref string val, Form uiParent) {
            if (val == null) {
                return false;
            } else if (val == "") {
                return true;
            } else {
                // {0}が1つだけか？
                int[] idxParam = StringUtils.SearchAllPosition(val, "{0}");
                int[] idxParL = StringUtils.SearchAllPosition(val, "{");
                int[] idxParR = StringUtils.SearchAllPosition(val, "}");
                if (idxParam.Length != 1 || idxParL.Length != 1 || idxParR.Length != 1) {
                    return ConfigCheckError(uiParent, Resources.Option_DiffCommandLineParam);
                }
                // A-Zと\が、{0}の前にあるか？
                int idxAlpha = StringUtils.IndexOfRangeIgnoreCase(val, 'A', 'Z');
                int idxPath = val.IndexOf('\\');
                if (idxAlpha == -1 || idxAlpha > idxParam[0] || idxPath > idxParam[0]) {
                    return ConfigCheckError(uiParent, Resources.Option_DiffCommandLineExe);
                }
                return true;
            }
        }

        
        //=========================================================================================
        // プロパティ：ディレクトリ用差分表示ツールのコマンドライン（{0}にファイル名、未定義のとき""）
        //=========================================================================================
        public string DiffDirectoryCommandLine {
            get {
                return m_diffDirectoryCommandLine;
            }
            set {
                m_diffDirectoryCommandLine = value;
            }
        }

        public static bool CheckDiffDirectoryCommandLine(ref string val, Form uiParent) {
            if (val == null) {
                return false;
            } else if (val == "") {
                return true;
            } else {
                // {0}が1つだけか？
                int[] idxParam = StringUtils.SearchAllPosition(val, "{0}");
                int[] idxParL = StringUtils.SearchAllPosition(val, "{");
                int[] idxParR = StringUtils.SearchAllPosition(val, "}");
                if (idxParam.Length != 1 || idxParL.Length != 1 || idxParR.Length != 1) {
                    return ConfigCheckError(uiParent, Resources.Option_DiffCommandLineParam);
                }
                // A-Zと\が、{0}の前にあるか？
                int idxAlpha = StringUtils.IndexOfRangeIgnoreCase(val, 'A', 'Z');
                int idxPath = val.IndexOf('\\');
                if (idxAlpha == -1 || idxAlpha > idxParam[0] || idxPath > idxParam[0]) {
                    return ConfigCheckError(uiParent, Resources.Option_DiffCommandLineExe);
                }
                return true;
            }
        }


//*****************************************************************************************
// ファイル一覧＞全般
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：ファイル更新後、自動反映するまでの時間[ms]（0:チェックしない）
        //=========================================================================================
        public int AutoDirectoryUpdateWait {
            get {
                return m_autoDirectoryUpdateWait;
            }
            set {
                m_autoDirectoryUpdateWait = value;
            }
        }
        public const int MIN_CHECK_AUTO_DIRECTORY_UPDATE_WAIT = 0;
        public const int MAX_CHECK_AUTO_DIRECTORY_UPDATE_WAIT = 5000;

        public static bool CheckAutoDirectoryUpdateWait(ref int val, Form uiParent) {
            if (val < MIN_CHECK_AUTO_DIRECTORY_UPDATE_WAIT || val > MAX_CHECK_AUTO_DIRECTORY_UPDATE_WAIT) {
                return ConfigCheckError(uiParent, Resources.Option_AutoDirectoryUpdateWaitValue, MIN_CHECK_AUTO_DIRECTORY_UPDATE_WAIT, MAX_CHECK_AUTO_DIRECTORY_UPDATE_WAIT);
            }
            val = (val + UIFileListWatcher.AUTO_UPDATE_TIMER_INTERVAL - 1) / UIFileListWatcher.AUTO_UPDATE_TIMER_INTERVAL * UIFileListWatcher.AUTO_UPDATE_TIMER_INTERVAL;
            return true;
        }

        //=========================================================================================
        // プロパティ：タブ切り替え時にファイル一覧を自動更新するときtrue
        //=========================================================================================
        public bool RefreshFileListTabChange {
            get {
                return m_refreshFileListTabChange;
            }
            set {
                m_refreshFileListTabChange = value;
            }
        }

        //=========================================================================================
        // プロパティ：SSHでもタブ切り替え時にファイル一覧を自動更新するときtrue
        //=========================================================================================
        public bool RefreshFileListTabChangeSSH {
            get {
                return m_refreshFileListTabChangeSSH;
            }
            set {
                m_refreshFileListTabChangeSSH = value;
            }
        }

//*****************************************************************************************
// ファイル一覧＞起動時の状態
//*****************************************************************************************

        //=========================================================================================
        // 左ウィンドウの初期ディレクトリ（"":前回値を使う）
        //=========================================================================================
        public string InitialDirectoryLeft {
            get {
                return m_initialDirectoryLeft;
            }
            set {
                m_initialDirectoryLeft = value;
            }
        }

        public static bool CheckInitialDirectoryLeft(ref string val, Form uiParent) {
            if (val == null) {
                return false;
            } else if (val == "") {
                return true;
            } else {
                val = GenericFileStringUtils.CompleteDirectoryName(val, "\\");
                bool exist = Directory.Exists(val);
                if (!exist) {
                    return ConfigCheckError(uiParent, Resources.Option_InitialDirectoryLeftPath);
                }
                return true;
            }
        }

        //=========================================================================================
        // 右ウィンドウの初期ディレクトリ（"":前回値を使う）
        //=========================================================================================
        public string InitialDirectoryRight {
            get {
                return m_initialDirectoryRight;
            }
            set {
                m_initialDirectoryRight = value;
            }
        }

        public static bool CheckInitialDirectoryRight(ref string val, Form uiParent) {
            if (val == null) {
                return false;
            } else if (val == "") {
                return true;
            } else {
                val = GenericFileStringUtils.CompleteDirectoryName(val, "\\");
                bool exist = Directory.Exists(val);
                if (!exist) {
                    return ConfigCheckError(uiParent, Resources.Option_InitialDirectoryRightPath);
                }
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：メインウィンドウのデフォルトサイズ（Empty:前回値を使う）
        //=========================================================================================
        public Rectangle MainWindowRectDefault {
            get {
                return m_mainWindowRectDefault;
            }
            set {
                m_mainWindowRectDefault = value;
            }
        }

        public static bool CheckMainWindowRectDefault(ref Rectangle val, Form uiParent) {
            if (val == Rectangle.Empty) {
                return true;
            } else {
                Rectangle rcScreen = FormUtils.GetAllScreenRectangle();
                if (val.Left >= rcScreen.Right || val.Top >= rcScreen.Bottom) {
                    return ConfigCheckError(uiParent, Resources.Option_MainWindowRectDefaultLocation);
                }
                if (val.Width >= rcScreen.Width || val.Height >= rcScreen.Height) {
                    return ConfigCheckError(uiParent, Resources.Option_MainWindowRectDefaultSize);
                }
                if (val.Width < MainWindowForm.FILE_WINDOW_MIN_CX || val.Height < MainWindowForm.FILE_WINDOW_MIN_CY) {
                    return ConfigCheckError(uiParent, Resources.Option_MainWindowRectDefaultMinSize, MainWindowForm.FILE_WINDOW_MIN_CX, MainWindowForm.FILE_WINDOW_MIN_CY);
                }
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：スプラッシュウィンドウでの待ち時間[ms]
        //=========================================================================================
        public int SplashWindowWait {
            get {
                return m_splashWindowWait;
            }
            set {
                m_splashWindowWait = value;
            }
        }

        public const int MIN_SPLASH_WINDOW_WAIT = 0;
        public const int MAX_SPLASH_WINDOW_WAIT = 5000;

        public static void ModifySplashWindowWait(ref int val) {
            if (val < MIN_SPLASH_WINDOW_WAIT) {
                val = MIN_SPLASH_WINDOW_WAIT;
            } else if (val > MAX_SPLASH_WINDOW_WAIT) {
                val = MAX_SPLASH_WINDOW_WAIT;
            }
        }

//*****************************************************************************************
// ファイル一覧＞起動時の一覧
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：左ウィンドウのデフォルトソート方法（null:前回値を使う）
        //=========================================================================================
        public FileListSortMode DefaultFileListSortModeLeft {
            get {
                return m_defaultFileListSortModeLeft;
            }
            set {
                m_defaultFileListSortModeLeft = value;
            }
        }

        //=========================================================================================
        // プロパティ：右ウィンドウのデフォルトソート方法（null:前回値を使う）
        //=========================================================================================
        public FileListSortMode DefaultFileListSortModeRight {
            get {
                return m_defaultFileListSortModeRight;
            }
            set {
                m_defaultFileListSortModeRight = value;
            }
        }

//*****************************************************************************************
// ファイル一覧＞フォルダサイズの取得
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：フォルダサイズの取得方法のデフォルト（null:前回値を使う）
        //=========================================================================================
        public RetrieveFolderSizeCondition RetrieveFolderSizeCondition {
            get {
                return m_retrieveFolderSizeCondition;
            }
            set {
                m_retrieveFolderSizeCondition = value;
            }
        }
        
        public const int MIN_RETRIEVE_FOLDER_SIZE_UNIT = 1;
        public const int MAX_RETRIEVE_FOLDER_SIZE_UNIT = 65536;

        public static void ModifyRetrieveFolderSizeUnit(ref int val) {
            if (val < MIN_RETRIEVE_FOLDER_SIZE_UNIT) {
                val = MIN_RETRIEVE_FOLDER_SIZE_UNIT;
            } else if (val > MAX_RETRIEVE_FOLDER_SIZE_UNIT) {
                val = MAX_RETRIEVE_FOLDER_SIZE_UNIT;
            }
        }

        //=========================================================================================
        // プロパティ：フォルダサイズの取得後、保持する階層
        //=========================================================================================
        public int RetrieveFolderSizeKeepLowerDepth {
            get {
                return m_retrieveFolderSizeKeepLowerDepth;
            }
            set {
                m_retrieveFolderSizeKeepLowerDepth = value;
            }
        }

        public const int MIN_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_DEPTH = 1;
        public const int MAX_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_DEPTH = 10;

        public static void ModifyRetrieveFolderSizeKeepLowerDepth(ref int val) {
            if (val < MIN_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_DEPTH) {
                val = MIN_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_DEPTH;
            } else if (val > MAX_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_DEPTH) {
                val = MAX_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_DEPTH;
            }
        }

        //=========================================================================================
        // プロパティ：フォルダサイズの取得後、保持するフォルダ数
        //=========================================================================================
        public int RetrieveFolderSizeKeepLowerCount {
            get {
                return m_retrieveFolderSizeKeepLowerCount;
            }
            set {
                m_retrieveFolderSizeKeepLowerCount = value;
            }
        }

        public const int MIN_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_COUNT = 1;
        public const int MAX_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_COUNT = 10000;

        public static void ModifyRetrieveFolderSizeKeepLowerCount(ref int val) {
            if (val < MIN_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_COUNT) {
                val = MIN_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_COUNT;
            } else if (val > MAX_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_COUNT) {
                val = MAX_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_COUNT;
            }
        }

//*****************************************************************************************
// ファイル一覧＞動作
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：ファイル一覧でのスクロールマージンの行数
        //=========================================================================================
        public int ListViewScrollMarginLineCount {
            get {
                return m_listViewScrollMarginLineCount;
            }
            set {
                m_listViewScrollMarginLineCount = value;
            }
        }

        public const int MIN_LIST_VIEW_SCROLL_MARGIN_LINE_COUNT = 0;
        public const int MAX_LIST_VIEW_SCROLL_MARGIN_LINE_COUNT = 5;

        public static bool CheckListViewScrollMarginLineCount(ref int val, Form uiParent) {
            if (val < MIN_LIST_VIEW_SCROLL_MARGIN_LINE_COUNT || val > MAX_LIST_VIEW_SCROLL_MARGIN_LINE_COUNT) {
                return ConfigCheckError(uiParent, Resources.Option_ListViewScrollMarginLineCountValue, MIN_LIST_VIEW_SCROLL_MARGIN_LINE_COUNT, MAX_LIST_VIEW_SCROLL_MARGIN_LINE_COUNT);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：マウスホイールが回転したときの最大移動行数
        //=========================================================================================
        public int MouseWheelMaxLines {
            get {
                return m_mouseWheelMaxLines;
            }
            set {
                m_mouseWheelMaxLines = value;
            }
        }

        public const int MIN_MOUSE_WHEEL_MAX_LINES = 1;
        public const int MAX_MOUSE_WHEEL_MAX_LINES = 50;

        public static void ModifyMouseWheelMaxLines(ref int val) {
            if (val < 1) {
                val = 1;
            } else if (val > 50) {
                val = 50;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧でドラッグ中に最大速度で移動できる行数
        //=========================================================================================
        public int FileListDragMaxSpeed {
            get {
                return m_fileListDragMaxSpeed;
            }
            set {
                m_fileListDragMaxSpeed = value;
            }
        }

        public const int MIN_FILE_LIST_DRAG_MAX_SPEED = 1;
        public const int MAX_FILE_LIST_DRAG_MAX_SPEED = 50;

        public static void ModifyFileListDragMaxSpeed(ref int val) {
            if (val < 1) {
                val = 1;
            } else if (val > 50) {
                val = 50;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧で最後の拡張子を離して表示するときtrue
        //=========================================================================================
        public bool FileListSeparateExt {
            get {
                return m_fileListSeparateExt;
            }
            set {
                m_fileListSeparateExt = value;
            }
        }

        //=========================================================================================
        // プロパティ：逆向き←→で親フォルダに戻るときtrue
        //=========================================================================================
        public bool ChdirParentOtherSideMove {
            get {
                return m_chdirParentOtherSideMove;
            }
            set {
                m_chdirParentOtherSideMove = value;
            }
        }

        //=========================================================================================
        // プロパティ：ドラッグ中にウィンドウ外に移動した場合にウィンドウを隠すときtrue
        //=========================================================================================
        public bool HideWindowDragDrop {
            get {
                return m_hideWindowDragDrop;
            }
            set {
                m_hideWindowDragDrop = value;
            }
        }

        //=========================================================================================
        // プロパティ：フォルダ変更時にカーソル位置のレジュームを行うときtrue
        //=========================================================================================
        public bool ResumeFolderCursorFile {
            get {
                return m_resumeFolderCursorFile;
            }
            set {
                m_resumeFolderCursorFile = value;
            }
        }

//*****************************************************************************************
// ファイル一覧＞起動時の表示モード
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：ファイル一覧左画面の表示モード（前回の状態に従うときnull）
        //=========================================================================================
        public FileListViewMode DefaultViewModeLeft {
            get {
                return m_defaultViewModeLeft;
            }
            set {
                m_defaultViewModeLeft = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイル一覧右画面の表示モード（前回の状態に従うときnull）
        //=========================================================================================
        public FileListViewMode DefaultViewModeRight {
            get {
                return m_defaultViewModeRight;
            }
            set {
                m_defaultViewModeRight = value;
            }
        }
        
//*****************************************************************************************
// ファイル一覧＞表示モード
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：フォルダ切り替え時のモード（直前の状態に従うときnull）
        //=========================================================================================
        public FileListViewMode FileListViewChangeMode {
            get {
                return m_fileListViewChangeMode;
            }
            set {
                m_fileListViewChangeMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：フォルダごとの自動切り替えの設定
        //=========================================================================================
        public FileListViewModeAutoSetting FileListViewModeAutoSetting {
            get {
                return m_fileListViewModeAutoSetting;
            }
            set {
                m_fileListViewModeAutoSetting = value;
            }
        }

//*****************************************************************************************
// ファイル操作＞全般
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：生成できる待機可能バックグラウンドタスクの最大数のコンフィグ値（次回起動後に有効）
        //=========================================================================================
        public int MaxBackgroundTaskWaitableCountDefault {
            get {
                return m_maxBackgroundTaskWaitableCountDefault;
            }
            set {
                m_maxBackgroundTaskWaitableCountDefault = value;
            }
        }

        public const int MIN_MAX_BACKGROUND_TASK_WAITABLE_COUNT = 1;
        public const int MAX_MAX_BACKGROUND_TASK_WAITABLE_COUNT = 9;

        public static bool CheckMaxBackgroundTaskWaitableCountDefault(ref int val, Form uiParent) {
            if (val < MIN_MAX_BACKGROUND_TASK_WAITABLE_COUNT || val > MAX_MAX_BACKGROUND_TASK_WAITABLE_COUNT) {
                return ConfigCheckError(uiParent, Resources.Option_MaxBackgroundTaskWaitableCountDefaultValue, MIN_MAX_BACKGROUND_TASK_WAITABLE_COUNT, MAX_MAX_BACKGROUND_TASK_WAITABLE_COUNT);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：生成できる待機不可能バックグラウンドタスクの最大数のコンフィグ値（次回起動後に有効）
        //=========================================================================================
        public int MaxBackgroundTaskLimitedCountDefault {
            get {
                return m_maxBackgroundTaskLimitedCountDefault;
            }
            set {
                m_maxBackgroundTaskLimitedCountDefault = value;
            }
        }

        public const int MIN_MAX_BACKGROUND_TASK_LIMITED_COUNT = 1;
        public const int MAX_MAX_BACKGROUND_TASK_LIMITED_COUNT = 10;

        public static bool CheckMaxBackgroundTaskLimitedCountDefault(ref int val, Form uiParent) {
            if (val < MIN_MAX_BACKGROUND_TASK_LIMITED_COUNT || val > MAX_MAX_BACKGROUND_TASK_LIMITED_COUNT) {
                return ConfigCheckError(uiParent, Resources.Option_MaxBackgroundTaskLimitedCountDefaultValue, MIN_MAX_BACKGROUND_TASK_LIMITED_COUNT, MAX_MAX_BACKGROUND_TASK_LIMITED_COUNT);
            }
            return true;
        }

//*****************************************************************************************
// ファイル操作＞転送と削除
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：同名ファイルのオプションのデフォルト（null:前回値を使用する）
        //=========================================================================================
        public SameFileOption SameFileOptionDefault {
            get {
                return m_sameFileOptionDefault;
            }
            set {
                m_sameFileOptionDefault = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリ削除のオプションのデフォルト（null:前回値を使用する）
        //=========================================================================================
        public DeleteFileOption DeleteFileOptionDefault {
            get {
                return m_deleteFileOptionDefault;
            }
            set {
                m_deleteFileOptionDefault = value;
            }
        }

//*****************************************************************************************
// ファイル操作＞属性のコピー
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：属性のコピーの方法
        //=========================================================================================
        public AttributeSetMode TransferAttributeSetMode {
            get {
                return m_transferAttributeSetMode;
            }
            set {
                m_transferAttributeSetMode = value;
            }
        }

//*****************************************************************************************
// ファイル操作＞マークなし操作
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：マークなしコピーを許可するときtrue
        //=========================================================================================
        public bool MarklessCopy {
            get {
                return m_marklessCopy;
            }
            set {
                m_marklessCopy = value;
            }
        }

        //=========================================================================================
        // プロパティ：マークなし移動を許可するときtrue
        //=========================================================================================
        public bool MarklessMove {
            get {
                return m_marklessMove;
            }
            set {
                m_marklessMove = value;
            }
        }

        //=========================================================================================
        // プロパティ：マークなし削除を許可するときtrue
        //=========================================================================================
        public bool MarklessDelete {
            get {
                return m_marklessDelete;
            }
            set {
                m_marklessDelete = value;
            }
        }

        //=========================================================================================
        // プロパティ：マークなしショートカットの作成を許可するときtrue
        //=========================================================================================
        public bool MarklessShortcut {
            get {
                return m_marklessShortcut;
            }
            set {
                m_marklessShortcut = value;
            }
        }

        //=========================================================================================
        // プロパティ：マークなしファイル属性の一括編集を許可するときtrue
        //=========================================================================================
        public bool MarklessAttribute {
            get {
                return m_marklessAttribute;
            }
            set {
                m_marklessAttribute = value;
            }
        }

        //=========================================================================================
        // プロパティ：マークなし圧縮を許可するときtrue
        //=========================================================================================
        public bool MarklessPack {
            get {
                return m_marklessPack;
            }
            set {
                m_marklessPack = value;
            }
        }

        //=========================================================================================
        // プロパティ：マークなし展開を許可するときtrue
        //=========================================================================================
        public bool MarklessUnpack {
            get {
                return m_marklessUnpack;
            }
            set {
                m_marklessUnpack = value;
            }
        }

        //=========================================================================================
        // プロパティ：マークなし編集を許可するときtrue
        //=========================================================================================
        public bool MarklessEdit {
            get {
                return m_marklessEdit;
            }
            set {
                m_marklessEdit = value;
            }
        }

        //=========================================================================================
        // プロパティ：マークなしでのフォルダサイズの表示を許可するときtrue
        //=========================================================================================
        public bool MarklessFodlerSize {
            get {
                return m_marklessFodlerSize;
            }
            set {
                m_marklessFodlerSize = value;
            }
        }

//*****************************************************************************************
// ファイル操作＞クリップボード
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：ファイル名を指定してコピーの方法のデフォルト（null:前回値を使用する）
        //=========================================================================================
        public ClipboardCopyNameAsSetting ClipboardCopyNameAsSettingDefault {
            get {
                return m_clipboardCopyNameAsSettingDefault;
            }
            set {
                m_clipboardCopyNameAsSettingDefault = value;
            }
        }

//*****************************************************************************************
// ファイル操作＞一覧の比較
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：ファイル一覧の比較方法のデフォルト（null:前回値を使用する）
        //=========================================================================================
        public FileCompareSetting FileCompareSettingDefault {
            get {
                return m_fileCompareSettingDefault;
            }
            set {
                m_fileCompareSettingDefault = value;
            }
        }

//*****************************************************************************************
// ファイル操作＞圧縮
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：圧縮オプションのデフォルト（null:前回値を使用する）
        //=========================================================================================
        public ArchiveSetting ArchiveSettingDefault {
            get {
                return m_archiveSettingDefault;
            }
            set {
                m_archiveSettingDefault = value;
            }
        }

//*****************************************************************************************
// ファイル操作＞展開
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：展開先パスのモード
        //=========================================================================================
        public ExtractPathMode ArchiveExtractPathMode {
            get {
                return m_archiveExtractPathMode;
            }
            set {
                m_archiveExtractPathMode = value;
            }
        }
        
//*****************************************************************************************
// ファイル操作＞各種操作
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：インクリメンタルサーチで文字列の先頭から検索するときtrueのデフォルト（null:前回値を使う）
        //=========================================================================================
        public BooleanFlag IncrementalSearchFromHeadDefault {
            get {
                return m_incrementalSearchFromHeadDefault;
            }
            set {
                m_incrementalSearchFromHeadDefault = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：新規ディレクトリに自動的にカレントを移動してよいときtrue（null:前回値を使う）
        //=========================================================================================
        public BooleanFlag MakeDirectoryMoveCurrentDefault {
            get {
                return m_makeDirectoryMoveCurrentDefault;
            }
            set {
                m_makeDirectoryMoveCurrentDefault = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：新規ディレクトリ名(Windows)
        //=========================================================================================
        public string MakeDirectoryNewWindowsName {
            get {
                return m_makeDirectoryNewWindowsName;
            }
            set {
                m_makeDirectoryNewWindowsName = value;
            }
        }

        public static bool CheckMakeDirectoryNewWindowsName(ref string val, Form uiParent) {
            if (val.Length == 0) {
                return ConfigCheckError(uiParent, Resources.Option_MakeDirectoryNewWindowsNameNone);
            }
            return true;
        }
        
        //=========================================================================================
        // プロパティ：新規ディレクトリ名(SSH)
        //=========================================================================================
        public string MakeDirectoryNewSSHName {
            get {
                return m_makeDirectoryNewSSHName;
            }
            set {
                m_makeDirectoryNewSSHName = value;
            }
        }

        public static bool CheckMakeDirectoryNewSSHName(ref string val, Form uiParent) {
            if (val.Length == 0) {
                return ConfigCheckError(uiParent, Resources.Option_MakeDirectoryNewSSHNameNone);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：Windowsコマンドの実行で標準出力の結果をログに中継する方法のデフォルト（null:前回値を使う）
        //=========================================================================================
        public ShellExecuteRelayMode ShellExecuteRelayModeWindowsDefault {
            get {
                return m_shellExecuteReplayModeWindowsDefault;
            }
            set {
                m_shellExecuteReplayModeWindowsDefault = value;
            }
        }

        //=========================================================================================
        // プロパティ：SSHコマンドの実行で標準出力の結果をログに中継する方法のデフォルト（null:前回値を使う）
        //=========================================================================================
        public ShellExecuteRelayMode ShellExecuteRelayModeSSHDefault {
            get {
                return m_shellExecuteReplayModeSSHDefault;
            }
            set {
                m_shellExecuteReplayModeSSHDefault = value;
            }
        }

        //=========================================================================================
        // プロパティ：ミラーコピーを除外するファイル（「:」区切り）
        //=========================================================================================
        public string MirrorCopyExceptFiles {
            get {
                return m_mirrorCopyExceptFiles;
            }
            set {
                m_mirrorCopyExceptFiles = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル結合でのデフォルトファイル名の種類
        //=========================================================================================
        public CombineDefaultFileType CombineDefaultFileType {
            get {
                return m_combineDefaultFileType;
            }
            set {
                m_combineDefaultFileType = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル結合でのデフォルトファイル名
        //=========================================================================================
        public string CombineDefaultFileName {
            get {
                return m_combineDefaultFileName;
            }
            set {
                m_combineDefaultFileName = value;
            }
        }

//*****************************************************************************************
// SSH＞全般
//*****************************************************************************************
        
        //=========================================================================================
        // プロパティ：SSHショートカットの作成方法（null:前回値を使う）
        //=========================================================================================
        public ShortcutType SSHShortcutTypeDefault {
            get {
                return m_sshShortcutTypeDefault;
            }
            set {
                m_sshShortcutTypeDefault = value;
            }
        }

        //=========================================================================================
        // プロパティ：デフォルトの接続方式（null:前回値を使う）
        //=========================================================================================
        public SSHProtocolType SshFileSystemDefault {
            get {
                return m_sshFileSystemDefault;
            }
            set {
                m_sshFileSystemDefault = value;
            }
        }
        
//*****************************************************************************************
// SSH＞ターミナル
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：SSHターミナルのバックログ行数
        //=========================================================================================
        public int TerminalLogLineCount {
            get {
                return m_terminalLogLineCount;
            }
            set {
                m_terminalLogLineCount = value;
            }
        }
        
        public const int MIN_TERMINAL_LOG_LINE_COUNT = 100;
        public const int MAX_TERMINAL_LOG_LINE_COUNT = 10000;

        public static bool CheckTerminalLogLineCount(ref int val, Form uiParent) {
            if (val < MIN_TERMINAL_LOG_LINE_COUNT || val > MAX_TERMINAL_LOG_LINE_COUNT) {
                return ConfigCheckError(uiParent, Resources.Option_TerminalLogLineCount, MIN_TERMINAL_LOG_LINE_COUNT, MAX_TERMINAL_LOG_LINE_COUNT);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：シェル起動でSSHフォルダからはSSHターミナルを起動するときtrue
        //=========================================================================================
        public bool TerminalShellCommandSSH {
            get {
                return m_terminalShellCommandSSH;
            }
            set {
                m_terminalShellCommandSSH = value;
            }
        }

        //=========================================================================================
        // プロパティ：ターミナルを閉じたときの動作
        //=========================================================================================
        public TerminalCloseConfirmMode TerminalCloseConfirmMode {
            get {
                return m_terminalCloseConfirmMode;
            }
            set {
                m_terminalCloseConfirmMode = value;
            }
        }

//*****************************************************************************************
// SSH＞ターミナルログ
//*****************************************************************************************
        
        //=========================================================================================
        // プロパティ：ターミナルログの出力方法
        //=========================================================================================
        public TerminalLogType TerminalLogType {
            get {
                return m_terminalLogType;
            }
            set {
                m_terminalLogType = value;
            }
        }

        //=========================================================================================
        // プロパティ：ターミナルログの1ファイルあたりの最大サイズ[KB]
        //=========================================================================================
        public int TerminalLogMaxSize {
            get {
                return m_terminalLogMaxSize;
            }
            set {
                m_terminalLogMaxSize = value;
            }
        }
                
        public const int MIN_TERMINAL_LOG_MAX_SIZE = 100;
        public const int MAX_TERMINAL_LOG_MAX_SIZE = 100 * 1024;

        public static bool CheckTerminalLogMaxSize(ref int val, Form uiParent) {
            if (val < MIN_TERMINAL_LOG_MAX_SIZE || val > MAX_TERMINAL_LOG_MAX_SIZE) {
                return ConfigCheckError(uiParent, Resources.Option_TerminalLogMaxSize, MIN_TERMINAL_LOG_MAX_SIZE, MAX_TERMINAL_LOG_MAX_SIZE);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：ターミナルログの最大ファイル数
        //=========================================================================================
        public int TerminalLogFileCount {
            get {
                return m_terminalLogFileCount;
            }
            set {
                m_terminalLogFileCount = value;
            }
        }

        public const int MIN_TERMINAL_LOG_FILE_COUNT = 5;
        public const int MAX_TERMINAL_LOG_FILE_COUNT = 1000;

        public static bool CheckTerminalLogFileCount(ref int val, Form uiParent) {
            if (val < MIN_TERMINAL_LOG_FILE_COUNT || val > MAX_TERMINAL_LOG_FILE_COUNT) {
                return ConfigCheckError(uiParent, Resources.Option_TerminalLogFileCount, MIN_TERMINAL_LOG_FILE_COUNT, MAX_TERMINAL_LOG_FILE_COUNT);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：ターミナルログの出力フォルダ（nullのときデフォルト、最後は'\'）
        //=========================================================================================
        public string TerminalLogOutputFolder {
            get {
                return m_terminalLogOutputFolder;
            }
            set {
                m_terminalLogOutputFolder = value;
                if (m_terminalLogOutputFolder != null) {
                    m_terminalLogOutputFolder = GenericFileStringUtils.CompleteDirectoryName(m_terminalLogOutputFolder, "\\");
                }
            }
        }

//*****************************************************************************************
// プライバシー＞全般
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：コマンドヒストリの最大記憶件数（次回起動時に有効）
        //=========================================================================================
        public int CommandHistoryMaxCountDefault {
            get {
                return m_commandHistoryMaxCountDefault;
            }
            set {
                m_commandHistoryMaxCountDefault = value;
            }
        }

        public const int MIN_COMMAND_HISTORY_MAX_COUNT = 0;
        public const int MAX_COMMAND_HISTORY_MAX_COUNT = 500;
        
        public static bool CheckCommandHistoryMaxCountDefault(ref int val, Form uiParent) {
            if (val < MIN_COMMAND_HISTORY_MAX_COUNT || val > MAX_COMMAND_HISTORY_MAX_COUNT) {
                return ConfigCheckError(uiParent, Resources.Option_CommandHistoryMaxCountValue, MIN_COMMAND_HISTORY_MAX_COUNT, MAX_COMMAND_HISTORY_MAX_COUNT);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドヒストリをディスクに保存するときtrue
        //=========================================================================================
        public bool CommandHistorySaveDisk {
            get {
                return m_commandHistorySaveDisk;
            }
            set {
                m_commandHistorySaveDisk = value;
            }
        }

        //=========================================================================================
        // プロパティ：パスヒストリの最大記憶件数（次回起動時に有効）
        //=========================================================================================
        public int PathHistoryMaxCountDefault {
            get {
                return m_pathHistoryMaxCountDefault;
            }
            set {
                m_pathHistoryMaxCountDefault = value;
            }
        }

        //=========================================================================================
        // プロパティ：パスヒストリ（全体）の最大記憶件数（次回起動時に有効）
        //=========================================================================================
        public int PathHistoryWholeMaxCountDefault {
            get {
                return m_pathHistoryWholeMaxCountDefault;
            }
            set {
                m_pathHistoryWholeMaxCountDefault = value;
            }
        }

        public const int MIN_PATH_HISTORY_MAX_COUNT = 0;
        public const int MAX_PATH_HISTORY_MAX_COUNT = 500;
        
        public static bool CheckPathHistoryMaxCountDefault(ref int val, Form uiParent) {
            if (val < MIN_PATH_HISTORY_MAX_COUNT || val > MAX_PATH_HISTORY_MAX_COUNT) {
                return ConfigCheckError(uiParent, Resources.Option_PathHistoryMaxCountValue, MIN_PATH_HISTORY_MAX_COUNT, MAX_PATH_HISTORY_MAX_COUNT);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：パスヒストリ（全体）の履歴をディスクに保存するときtrue
        //=========================================================================================
        public bool PathHistoryWholeSaveDisk {
            get {
                return m_pathHistoryWholeSaveDisk;
            }
            set {
                m_pathHistoryWholeSaveDisk = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルビューア検索履歴の最大記憶件数のデフォルト（次回起動時に有効）
        //=========================================================================================
        public int ViewerSearchHistoryMaxCountDefault {
            get {
                return m_viewerSearchHistoryMaxCountDefault;
            }
            set {
                m_viewerSearchHistoryMaxCountDefault = value;
            }
        }

        public const int MIN_VIEWER_SEARCH_HISTORY_MAX_COUNT = 0;
        public const int MAX_VIEWER_SEARCH_HISTORY_MAX_COUNT = 50;
        
        public static bool CheckViewerSearchHistoryMaxCountDefault(ref int val, Form uiParent) {
            if (val < MIN_VIEWER_SEARCH_HISTORY_MAX_COUNT || val > MAX_VIEWER_SEARCH_HISTORY_MAX_COUNT) {
                return ConfigCheckError(uiParent, Resources.Option_ViewerSearchHistoryMaxCountValue, MIN_VIEWER_SEARCH_HISTORY_MAX_COUNT, MAX_VIEWER_SEARCH_HISTORY_MAX_COUNT);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：ファイルビューア検索履歴をディスクに保存するときtrue
        //=========================================================================================
        public bool ViewerSearchHistorySaveDisk {
            get {
                return m_viewerSearchHistorySaveDisk;
            }
            set {
                m_viewerSearchHistorySaveDisk = value;
            }
        }

//*****************************************************************************************
// テキストビューア＞全般
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：テキストビューア 最大読み込みサイズ
        //=========================================================================================
        public int TextViewerMaxFileSize {
            get {
                return m_textViewerMaxFileSize;
            }
            set {
                m_textViewerMaxFileSize = value;
            }
        }

        public const int MIN_TEXT_VIEWER_MAX_FILE_SIZE = 1 * 1024 * 1024;
        public const int MAX_TEXT_VIEWER_MAX_FILE_SIZE = 1024 * 1024 * 1024;

        public static bool CheckTextViewerMaxFileSize(ref int val, Form uiParent) {
            if (val < MIN_TEXT_VIEWER_MAX_FILE_SIZE || val > MAX_TEXT_VIEWER_MAX_FILE_SIZE) {
                return ConfigCheckError(uiParent, Resources.Option_TextViewerMaxFileSizeValue, MIN_TEXT_VIEWER_MAX_FILE_SIZE, MAX_TEXT_VIEWER_MAX_FILE_SIZE);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：テキストビューアで表示する最大行数
        //=========================================================================================
        public int TextViewerMaxLineCount {
            get {
                return m_textViewerMaxLineCount;
            }
            set {
                m_textViewerMaxLineCount = value;
            }
        }

        public const int MIN_TEXT_VIEWER_MAX_LINE_COUNT = 1000;
        public const int MAX_TEXT_VIEWER_MAX_LINE_COUNT = 999999;

        public static bool CheckTextViewerMaxLineCount(ref int val, Form uiParent) {
            if (val < MIN_TEXT_VIEWER_MAX_LINE_COUNT || val > MAX_TEXT_VIEWER_MAX_LINE_COUNT) {
                return ConfigCheckError(uiParent, Resources.Option_TextViewerMaxFileCountValue, MIN_TEXT_VIEWER_MAX_LINE_COUNT, MAX_TEXT_VIEWER_MAX_LINE_COUNT);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：テキストの比較用バッファを差分表示ツール起動後に削除するときtrueのデフォルト（null:前回値を使う）
        //=========================================================================================
        public BooleanFlag TextViewerClearCompareBufferDefault {
            get {
                return m_textViewerClearCompareBufferDefault;
            }
            set {
                m_textViewerClearCompareBufferDefault = value;
            }
        }

//*****************************************************************************************
// テキストビューア＞表示
//*****************************************************************************************
        
        //=========================================================================================
        // プロパティ：テキストビューアで行番号を表示するときtrue
        //=========================================================================================
        public bool TextViewerIsDisplayLineNumber {
            get {
                return m_textViewerIsDisplayLineNumber;
            }
            set {
                m_textViewerIsDisplayLineNumber = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューアで制御文字を表示するときtrue
        //=========================================================================================
        public bool TextViewerIsDisplayCtrlChar {
            get {
                return m_textViewerIsDisplayCtrlChar;
            }
            set {
                m_textViewerIsDisplayCtrlChar = value;
            }
        }

//*****************************************************************************************
// テキストビューア＞折返しとタブ
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：テキストビューアでの折り返し設定のデフォルト（null:前回値を使う）
        //=========================================================================================
        public TextViewerLineBreakSetting TextViewerLineBreakDefault {
            get {
                return m_textViewerLineBreakDefault;
            }
            set {
                m_textViewerLineBreakDefault = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューアでタブ幅4とみなすファイルの拡張子
        //=========================================================================================
        public string TextViewerTab4Extension {
            get {
                return m_textViewerTab4Extension;
            }
            set {
                m_textViewerTab4Extension = value;
            }
        }

        public static bool CheckTextViewerTab4Extension(ref string val, Form uiParent) {
            if (val == null) {
                return false;
            }
            return true;
        }

//*****************************************************************************************
// テキストビューア＞検索オプション
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：テキストビューアでの検索オプションのデフォルト（null:前回値を使う）
        //=========================================================================================
        public TextSearchOption TextViewerSearchOptionDefault {
            get {
                return m_textSearchOptionDefault;
            }
            set {
                m_textSearchOptionDefault = value;
            }
        }

//*****************************************************************************************
// テキストビューア＞クリップボード
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：テキストビューアでのクリップボードコピー形式のデフォルト（null:前回値を使う）
        //=========================================================================================
        public TextClipboardSetting TextClipboardSettingDefault {
            get {
                return m_textClipboardSettingDefault;
            }
            set {
                m_textClipboardSettingDefault = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダンプビューアでのクリップボードコピー形式のデフォルト（null:前回値を使う）
        //=========================================================================================
        public DumpClipboardSetting DumpClipboardSettingDefault {
            get {
                return m_dumpClipboardSettingDefault;
            }
            set {
                m_dumpClipboardSettingDefault = value;
            }
        }

//*****************************************************************************************
// グラフィックビューア＞全般
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：読み込むファイルの最大サイズ
        //=========================================================================================
        public int GraphicsViewerMaxFileSize {
            get {
                return m_graphicsViewerMaxFileSize;
            }
            set {
                m_graphicsViewerMaxFileSize = value;
            }
        }

        public const int MIN_GRAPHICS_VIEWER_MAX_FILE_SIZE = 1 * 1024 * 1024;
        public const int MAX_GRAPHICS_VIEWER_MAX_FILE_SIZE = 1024 * 1024 * 1024;

        public static bool CheckGraphicsViewerMaxFileSize(ref int val, Form uiParent) {
            if (val < MIN_GRAPHICS_VIEWER_MAX_FILE_SIZE || val > MAX_GRAPHICS_VIEWER_MAX_FILE_SIZE) {
                return ConfigCheckError(uiParent, Resources.Option_GraphicsViewerMaxFileSizeValue, MIN_GRAPHICS_VIEWER_MAX_FILE_SIZE / 1024 / 1024, MAX_GRAPHICS_VIEWER_MAX_FILE_SIZE / 1024 / 1024);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：ドラッグ中に慣性を使用するかどうか
        //=========================================================================================
        public DragInertiaType GraphicsViewerDragInertia {
            get {
                return m_graphicsViewerDragInertia;
            }
            set {
                m_graphicsViewerDragInertia = value;
            }
        }

        //=========================================================================================
        // プロパティ：ドラッグ中に慣性のブレーキのきき具合
        //=========================================================================================
        public int GraphicsViewerDragBreaking {
            get {
                return m_graphicsViewerDragBreaking;
            }
            set {
                m_graphicsViewerDragBreaking = value;
            }
        }

        public const int MIN_GRAPHICS_VIEWER_DRAG_BREAKING = 10;
        public const int MAX_GRAPHICS_VIEWER_DRAG_BREAKING = 2000;

        public static bool CheckGraphicsViewerDragBreaking(ref int val, Form uiParent) {
            if (val < MIN_GRAPHICS_VIEWER_DRAG_BREAKING || val > MAX_GRAPHICS_VIEWER_DRAG_BREAKING) {
                return ConfigCheckError(uiParent, Resources.Option_GraphicsViewerDragBreaking, MIN_GRAPHICS_VIEWER_DRAG_BREAKING, MAX_GRAPHICS_VIEWER_DRAG_BREAKING);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアでのフィルターの適用方法
        //=========================================================================================
        public GraphicsViewerFilterMode GraphicsViewerFilterMode {
            get {
                return m_graphicsViewerFilterMode;
            }
            set {
                m_graphicsViewerFilterMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアで全画面表示のときに非表示にするまでの時間[ms]
        //=========================================================================================
        public int GraphicsViewerFullScreenHideTimer {
            get {
                return m_graphicsViewerFullScreenHideTimer;
            }
            set {
                m_graphicsViewerFullScreenHideTimer = value;
            }
        }

        public const int MIN_GRAPHICS_VIEWER_FULL_SCREEN_HIDE_TIMER = 200;
        public const int MAX_GRAPHICS_VIEWER_FULL_SCREEN_HIDE_TIMER = 10000;

        public static bool CheckGraphicsViewerFullScreenHideTimer(ref int val, Form uiParent) {
            if (val < MIN_GRAPHICS_VIEWER_FULL_SCREEN_HIDE_TIMER || val > MAX_GRAPHICS_VIEWER_FULL_SCREEN_HIDE_TIMER) {
                return ConfigCheckError(uiParent, Resources.Option_GraphicsViewerFullScreenHideTimer, MIN_GRAPHICS_VIEWER_DRAG_BREAKING, MAX_GRAPHICS_VIEWER_DRAG_BREAKING);
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアの全画面表示で、自動的にマウスカーソルを消すときtrue
        //=========================================================================================
        public bool GraphicsViewerFullScreenAutoHideCursor {
            get {
                return m_graphicsViewerFullScreenAutoHideCursor;
            }
            set {
                m_graphicsViewerFullScreenAutoHideCursor = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアの全画面表示で、自動的にファイル情報を消すときtrue
        //=========================================================================================
        public bool GraphicsViewerFullScreenAutoHideInfo {
            get {
                return m_graphicsViewerFullScreenAutoHideInfo;
            }
            set {
                m_graphicsViewerFullScreenAutoHideInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアの全画面表示で、常にファイル情報を消すときtrue
        //=========================================================================================
        public bool GraphicsViewerFullScreenHideInfoAlways {
            get {
                return m_graphicsViewerFullScreenHideInfoAlways;
            }
            set {
                m_graphicsViewerFullScreenHideInfoAlways = value;
            }
        }

//*****************************************************************************************
// グラフィックビューア＞拡大表示
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：グラフィックビューアの画像の拡大方法
        //=========================================================================================
        public GraphicsViewerAutoZoomMode GraphicsViewerAutoZoomMode {
            get {
                return m_graphicsViewerAutoZoomMode;
            }
            set {
                m_graphicsViewerAutoZoomMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：画面より画像が小さいときも画面サイズに合わせるときtrue
        //=========================================================================================
        public bool GraphicsViewerZoomInLarger {
            get {
                return m_graphicsViewerZoomInLarger;
            }
            set {
                m_graphicsViewerZoomInLarger = value;
            }
        }

//*****************************************************************************************
// ファンクションキー＞全般
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：ファンクションキーの区切り数（4, 5, 0:なし）
        //=========================================================================================
        public int FunctionBarSplitCount {
            get {
                return m_functionBarSplitCount;
            }
            set {
                m_functionBarSplitCount = value;
            }
        }
        
        public static bool CheckFunctionBarSplitCount(ref int val, Form uiParent) {
            if (val == 0 || val == 4 || val == 5) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：オーバーレイアイコンでキー名を表示するときtrue
        //=========================================================================================
        public bool FunctionBarUseOverrayIcon {
            get {
                return m_functionBarUseOverrayIcon;
            }
            set {
                m_functionBarUseOverrayIcon = value;
            }
        }

//*****************************************************************************************
// ログ＞全般
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：ログウィンドウで記憶する最大行数（次回起動時に有効）
        //=========================================================================================
        public int LogLineMaxCountDefault {
            get {
                return m_logLineMaxCountDefault;
            }
            set {
                m_logLineMaxCountDefault = value;
            }
        }

        public const int MIN_LOG_LINE_MAX_COUNT = 1000;
        public const int MAX_LOG_LINE_MAX_COUNT = 100000;

        public static bool CheckLogLineMaxCountDefault(ref int val, Form uiParent) {
            if (val < MIN_LOG_LINE_MAX_COUNT || val > MAX_LOG_LINE_MAX_COUNT) {
                return ConfigCheckError(uiParent, Resources.Option_LogLineMaxCountDefaultValue, MIN_LOG_LINE_MAX_COUNT, MAX_LOG_LINE_MAX_COUNT);
            }
            return true;
        }

//*****************************************************************************************
// 色とフォント＞ファイル一覧
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：ファイル一覧 背景色
        //=========================================================================================
        public Color FileListBackColor {
            get {
                return m_fileListBackColor;
            }
            set {
                m_fileListBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 通常のファイルの文字色
        //=========================================================================================
        public Color FileListFileTextColor {
            get {
                return m_fileListFileTextColor;
            }
            set {
                m_fileListFileTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 読み込み専用ファイルの文字色
        //=========================================================================================
        public Color FileListReadOnlyColor {
            get {
                return m_fileListReadOnlyColor;
            }
            set {
                m_fileListReadOnlyColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 隠しファイルの文字色
        //=========================================================================================
        public Color FileListHiddenColor {
            get {
                return m_fileListHiddenColor;
            }
            set {
                m_fileListHiddenColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 システムファイルの文字色
        //=========================================================================================
        public Color FileListSystemColor {
            get {
                return m_fileListSystemColor;
            }
            set {
                m_fileListSystemColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 アーカイブファイルの文字色
        //=========================================================================================
        public Color FileListArchiveColor {
            get {
                return m_fileListArchiveColor;
            }
            set {
                m_fileListArchiveColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 シンボリックリンクファイルの文字色
        //=========================================================================================
        public Color FileListSymlinkColor {
            get {
                return m_fileListSymlinkColor;
            }
            set {
                m_fileListSymlinkColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 ファイル名グレーアウトの文字色
        //=========================================================================================
        public Color FileListGrayColor {
            get {
                return m_fileListGrayColor;
            }
            set {
                m_fileListGrayColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中の文字色（Color.Emptyのときは通常色）
        //=========================================================================================
        public Color FileListMarkColor {
            get {
                return m_fileListMarkColor;
            }
            set {
                m_fileListMarkColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中の背景色1
        //=========================================================================================
        public Color FileListMarkBackColor1 {
            get {
                return m_fileListMarkBackColor1;
            }
            set {
                m_fileListMarkBackColor1 = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中の背景色2
        //=========================================================================================
        public Color FileListMarkBackColor2 {
            get {
                return m_fileListMarkBackColor2;
            }
            set {
                m_fileListMarkBackColor2 = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中の背景枠の描画色
        //=========================================================================================
        public Color FileListMarkBackBorderColor {
            get {
                return m_fileListMarkBackBorderColor;
            }
            set {
                m_fileListMarkBackBorderColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 ファイル名グレーアウトの文字色
        //=========================================================================================
        public Color FileListGrayBackColor {
            get {
                return m_fileListGrayBackColor;
            }
            set {
                m_fileListGrayBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中グレーアウトの文字色
        //=========================================================================================
        public Color FileListMarkGrayColor {
            get {
                return m_fileListMarkGrayColor;
            }
            set {
                m_fileListMarkGrayColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中のグレーアウト背景色1
        //=========================================================================================
        public Color FileListMarkGrayBackColor1 {
            get {
                return m_fileListMarkGrayBackColor1;
            }
            set {
                m_fileListMarkGrayBackColor1 = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中のグレーアウト背景色2
        //=========================================================================================
        public Color FileListMarkGrayBackColor2 {
            get {
                return m_fileListMarkGrayBackColor2;
            }
            set {
                m_fileListMarkGrayBackColor2 = value;
            }
        }
                
        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中のグレーアウト枠の描画色
        //=========================================================================================
        public Color FileListMarkGrayBackBorderColor {
            get {
                return m_fileListMarkGrayBackBorderColor;
            }
            set {
                m_fileListMarkGrayBackBorderColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 カーソルの色
        //=========================================================================================
        public Color FileListCursorColor {
            get {
                return m_fileListCursorColor;
            }
            set {
                m_fileListCursorColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 無効カーソルの色
        //=========================================================================================
        public Color FileListCursorDisableColor {
            get {
                return m_fileListCursorDisableColor;
            }
            set {
                m_fileListCursorDisableColor = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイル一覧 SSHスーパーユーザーのステータスバー背景色
        //=========================================================================================
        public Color FileListStatusBarSuperUserColor {
            get {
                return m_fileListStatusBarSuperUserColor;
            }
            set {
                m_fileListStatusBarSuperUserColor = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイル一覧 サムネイルの枠の色1
        //=========================================================================================
        public Color FileListThumbnailFrameColor1 {
            get {
                return m_fileListThumbnailFrameColor1;
            }
            set {
                m_fileListThumbnailFrameColor1 = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイル一覧 サムネイルの枠の色2
        //=========================================================================================
        public Color FileListThumbnailFrameColor2 {
            get {
                return m_fileListThumbnailFrameColor2;
            }
            set {
                m_fileListThumbnailFrameColor2 = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダイアログ中でエラーが発生したときのステータス背景色
        //=========================================================================================
        public Color DialogErrorBackColor {
            get {
                return m_dialogErrorBackColor;
            }
            set {
                m_dialogErrorBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダイアログ中でエラーが発生したときのステータス文字色
        //=========================================================================================
        public Color DialogErrorTextColor {
            get {
                return m_dialogErrorTextColor;
            }
            set {
                m_dialogErrorTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダイアログ中で警告が発生したときのステータス背景色
        //=========================================================================================
        public Color DialogWarningBackColor {
            get {
                return m_dialogWarningBackColor;
            }
            set {
                m_dialogWarningBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダイアログ中で警告が発生したときのステータス文字色
        //=========================================================================================
        public Color DialogWarningTextColor {
            get {
                return m_dialogWarningTextColor;
            }
            set {
                m_dialogWarningTextColor = value;
            }
        }

//*****************************************************************************************
// 色とフォント＞テキストビューア
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：テキストビューア エラー時の背景色
        //=========================================================================================
        public Color TextViewerErrorBackColor {
            get {
                return m_textViewerErrorBackColor;
            }
            set {
                m_textViewerErrorBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア エラー時のステータスバー背景色
        //=========================================================================================
        public Color TextViewerErrorStatusBackColor {
            get {
                return m_textViewerErrorStatusBackColor;
            }
            set {
                m_textViewerErrorStatusBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア エラー時のステータスバー文字色
        //=========================================================================================
        public Color TextViewerErrorStatusTextColor {
            get {
                return m_textViewerErrorStatusTextColor;
            }
            set {
                m_textViewerErrorStatusTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 情報表示のステータスバー背景色
        //=========================================================================================
        public Color TextViewerInfoStatusBackColor {
            get {
                return m_textViewerInfoStatusBackColor;
            }
            set {
                m_textViewerInfoStatusBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 情報表示のステータスバー文字色
        //=========================================================================================
        public Color TextViewerInfoStatusTextColor {
            get {
                return m_textViewerInfoStatusTextColor;
            }
            set {
                m_textViewerInfoStatusTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 行番号の色
        //=========================================================================================
        public Color TextViewerLineNoTextColor {
            get {
                return m_textViewerLineNoTextColor;
            }
            set {
                m_textViewerLineNoTextColor = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：テキストビューア 行番号の背景色（左）
        //=========================================================================================
        public Color TextViewerLineNoBackColor1 {
            get {
                return m_textViewerLineNoBackColor1;
            }
            set {
                m_textViewerLineNoBackColor1 = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 行番号の背景色（右）
        //=========================================================================================
        public Color TextViewerLineNoBackColor2 {
            get {
                return m_textViewerLineNoBackColor2;
            }
            set {
                m_textViewerLineNoBackColor2 = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 行番号境界の色
        //=========================================================================================
        public Color TextViewerLineNoSeparatorColor {
            get {
                return m_textViewerLineNoSeparatorColor;
            }
            set {
                m_textViewerLineNoSeparatorColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 領域外の色
        //=========================================================================================
        public Color TextViewerOutOfAreaBackColor {
            get {
                return m_textViewerOutOfAreaBackColor;
            }
            set {
                m_textViewerOutOfAreaBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 領域外分離線の色
        //=========================================================================================
        public Color TextViewerOutOfAreaSeparatorColor {
            get {
                return m_textViewerOutOfAreaSeparatorColor;
            }
            set {
                m_textViewerOutOfAreaSeparatorColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 検索中カーソルの色
        //=========================================================================================
        public Color TextViewerSearchCursorColor {
            get {
                return m_textViewerSearchCursorColor;
            }
            set {
                m_textViewerSearchCursorColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 制御関連の色
        //=========================================================================================
        public Color TextViewerControlColor {
            get {
                return m_textViewerControlColor;
            }
            set {
                m_textViewerControlColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア テキストの色
        //=========================================================================================
        public Color TextViewerTextColor {
            get {
                return m_textViewerTextColor;
            }
            set {
                m_textViewerTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 選択中テキストの色
        //=========================================================================================
        public Color TextViewerSelectTextColor {
            get {
                return m_textViewerSelectTextColor;
            }
            set {
                m_textViewerSelectTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 選択中テキストの色２
        //=========================================================================================
        public Color TextViewerSelectTextColor2 {
            get {
                return m_textViewerSelectTextColor2;
            }
            set {
                m_textViewerSelectTextColor2 = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 選択中背景の色
        //=========================================================================================
        public Color TextViewerSelectBackColor {
            get {
                return m_textViewerSelectBackColor;
            }
            set {
                m_textViewerSelectBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 選択中背景の色（ダンプの参考範囲）
        //=========================================================================================
        public Color TextViewerSelectBackColor2 {
            get {
                return m_textViewerSelectBackColor2;
            }
            set {
                m_textViewerSelectBackColor2 = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 検索ヒット背景の色
        //=========================================================================================
        public Color TextViewerSearchHitBackColor {
            get {
                return m_textViewerSearchHitBackColor;
            }
            set {
                m_textViewerSearchHitBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 検索ヒットテキストの色
        //=========================================================================================
        public Color TextViewerSearchHitTextColor {
            get {
                return m_textViewerSearchHitTextColor;
            }
            set {
                m_textViewerSearchHitTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 自動検索ヒットテキストの色
        //=========================================================================================
        public Color TextViewerSearchAutoTextColor {
            get {
                return m_textViewerSearchAutoTextColor;
            }
            set {
                m_textViewerSearchAutoTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：レーダーバーの背景色１
        //=========================================================================================
        public Color RadarBarBackColor1 {
            get {
                return m_radarBarBackColor1;
            }
            set {
                m_radarBarBackColor1 = value;
            }
        }

        //=========================================================================================
        // プロパティ：レーダーバーの背景色２
        //=========================================================================================
        public Color RadarBarBackColor2 {
            get {
                return m_radarBarBackColor2;
            }
            set {
                m_radarBarBackColor2 = value;
            }
        }

//*****************************************************************************************
// 色とフォント＞グラフィックビューア
//*****************************************************************************************
        
        //=========================================================================================
        // プロパティ：グラフィックビューア 背景色
        //=========================================================================================
        public Color GraphicsViewerBackColor {
            get {
                return m_graphicsViewerBackColor;
            }
            set {
                m_graphicsViewerBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア テキスト表示の色
        //=========================================================================================
        public Color GraphicsViewerTextColor {
            get {
                return m_graphicsViewerTextColor;
            }
            set {
                m_graphicsViewerTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア テキスト表示影のブラシ
        //=========================================================================================
        public Color GraphicsViewerTextShadowColor {
            get {
                return m_graphicsViewerTextShadowColor;
            }
            set {
                m_graphicsViewerTextShadowColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア 読み込み中テキスト表示の色
        //=========================================================================================
        public Color GraphicsViewerLoadingTextColor {
            get {
                return m_graphicsViewerLoadingTextColor;
            }
            set {
                m_graphicsViewerLoadingTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア 読み込み中テキスト表示影のブラシ
        //=========================================================================================
        public Color GraphicsViewerLoadingTextShadowColor {
            get {
                return m_graphicsViewerLoadingTextShadowColor;
            }
            set {
                m_graphicsViewerLoadingTextShadowColor = value;
            }
        }

//*****************************************************************************************
// 色とフォント＞ログ
//*****************************************************************************************

        //=========================================================================================
        // プロパティ：ログウィンドウテキスト色
        //=========================================================================================
        public Color LogWindowTextColor {
            get {
                return m_logWindowTextColor;
            }
            set {
                m_logWindowTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウリンクテキスト色
        //=========================================================================================
        public Color LogWindowLinkTextColor {
            get {
                return m_logWindowLinkTextColor;
            }
            set {
                m_logWindowLinkTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウエラー色
        //=========================================================================================
        public Color LogErrorTextColor {
            get {
                return m_logErrorTextColor;
            }
            set {
                m_logErrorTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ標準エラー色
        //=========================================================================================
        public Color LogStdErrorTextColor {
            get {
                return m_logStdErrorTextColor;
            }
            set {
                m_logStdErrorTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ選択中のテキスト色
        //=========================================================================================
        public Color LogWindowSelectTextColor {
            get {
                return m_logWindowSelectTextColor;
            }
            set {
                m_logWindowSelectTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ背景色
        //=========================================================================================
        public Color LogWindowBackColor {
            get {
                return m_logWindowBackColor;
            }
            set {
                m_logWindowBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ選択中の背景色
        //=========================================================================================
        public Color LogWindowSelectBackColor {
            get {
                return m_logWindowSelectBackColor;
            }
            set {
                m_logWindowSelectBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ背景色（ビジュアルベル用）
        //=========================================================================================
        public Color LogWindowBackBellColor {
            get {
                return m_logWindowBackBellColor;
            }
            set {
                m_logWindowBackBellColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ背景色（セッション切断済み用）
        //=========================================================================================
        public Color LogWindowBackClosedColor {
            get {
                return m_logWindowBackClosedColor;
            }
            set {
                m_logWindowBackClosedColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ進捗表示バーの色１
        //=========================================================================================
        public Color LogWindowProgressColor1 {
            get {
                return m_logWindowProgressColor1;
            }
            set {
                m_logWindowProgressColor1 = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ進捗表示バーの色２
        //=========================================================================================
        public Color LogWindowProgressColor2 {
            get {
                return m_logWindowProgressColor2;
            }
            set {
                m_logWindowProgressColor2 = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ進捗表示バーの色３
        //=========================================================================================
        public Color LogWindowProgressColor3 {
            get {
                return m_logWindowProgressColor3;
            }
            set {
                m_logWindowProgressColor3 = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ進捗表示バーの色４
        //=========================================================================================
        public Color LogWindowProgressColor4 {
            get {
                return m_logWindowProgressColor4;
            }
            set {
                m_logWindowProgressColor4 = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 残り時間描画用のフォントの色（明）
        //=========================================================================================
        public Color LogWindowRemainingTimeTextColor1 {
            get {
                return m_logWindowRemainingTimeTextColor1;
            }
            set {
                m_logWindowRemainingTimeTextColor1 = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ 残り時間描画用のフォントの色（暗）
        //=========================================================================================
        public Color LogWindowRemainingTimeTextColor2 {
            get {
                return m_logWindowRemainingTimeTextColor2;
            }
            set {
                m_logWindowRemainingTimeTextColor2 = value;
            }
        }

//*****************************************************************************************
// 色とフォント＞フォント
//*****************************************************************************************
        
        public const float MIN_FONT_SIZE = 3;
        public const float MAX_FONT_SIZE = 30;

        public static bool CheckFontSize(ref float val, Form uiParent) {
            if (val <= 0 && val >= Single.MaxValue) {
                return ConfigCheckError(uiParent, Resources.Option_FontSizeValue, MIN_LOG_LINE_MAX_COUNT, MAX_LOG_LINE_MAX_COUNT);
            } else {
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧表示用のフォント名
        //=========================================================================================
        public string ListViewFontName {
            get {
                return CompleteFontName(m_listViewFontName);
            }
            set {
                m_listViewFontName = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧表示用のフォントサイズ
        //=========================================================================================
        public float ListViewFontSize {
            get {
                return m_listViewFontSize;
            }
            set {
                m_listViewFontSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：デフォルトファイル一覧 文字の高さ
        //=========================================================================================
        public int DefaultFileListViewHeight {
            get {
                return m_defaultFileListViewHeight;
            }
            set {
                m_defaultFileListViewHeight = value;
            }
        }

        public const int MIN_DEFAULT_FILE_LIST_VIEW_HEIGHT = 17;
        public const int MAX_DEFAULT_FILE_LIST_VIEW_HEIGHT = 128;

        public static bool CheckDefaultFileListViewHeight(ref int val, Form uiParent) {
            if (val <= MIN_DEFAULT_FILE_LIST_VIEW_HEIGHT && val >= MAX_DEFAULT_FILE_LIST_VIEW_HEIGHT) {
                return ConfigCheckError(uiParent, Resources.Option_DefaultFileListViewHeightValue, MIN_DEFAULT_FILE_LIST_VIEW_HEIGHT, MAX_DEFAULT_FILE_LIST_VIEW_HEIGHT);
            } else {
                return true;
            }
        }
        
        //=========================================================================================
        // プロパティ：サムネイルファイル一覧表示用のフォント名
        //=========================================================================================
        public string ThumbFileListViewFontName {
            get {
                return CompleteFontName(m_thumbFileListViewFontName);
            }
            set {
                m_thumbFileListViewFontName = value;
            }
        }

        //=========================================================================================
        // プロパティ：サムネイルファイル一覧表示用のフォントサイズ
        //=========================================================================================
        public float ThumbFileListViewFontSize {
            get {
                return m_thumbFileListViewFontSize;
            }
            set {
                m_thumbFileListViewFontSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：サムネイルファイル一覧表示用のフォントサイズ（小）
        //=========================================================================================
        public float ThumbFileListViewSmallFontSize {
            get {
                return m_thumbFileListViewSmallFontSize;
            }
            set {
                m_thumbFileListViewSmallFontSize = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：テキストビューア 描画用のフォント名
        //=========================================================================================
        public string TextFontName {
            get {
                return CompleteFontName(m_textFontName);
            }
            set {
                m_textFontName = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューア 描画用のフォントの大きさ
        //=========================================================================================
        public float TextFontSize {
            get {
                return m_textFontSize;
            }
            set {
                m_textFontSize = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：テキストビューアでの行の高さ
        //=========================================================================================
        public int TextFileViewerLineHeight {
            get {
                return m_textFileViewerLineHeight;
            }
            set {
                m_textFileViewerLineHeight = value;
            }
        }

        public const int MIN_TEXT_FILE_VIEWER_LINE_HEIGHT = 8;
        public const int MAX_TEXT_FILE_VIEWER_LINE_HEIGHT = 128;

        public static bool CheckTextFileViewerLineHeight(ref int val, Form uiParent) {
            if (val <= MIN_TEXT_FILE_VIEWER_LINE_HEIGHT && val >= MAX_TEXT_FILE_VIEWER_LINE_HEIGHT) {
                return ConfigCheckError(uiParent, Resources.Option_TextFileViewerLineHeightValue, MIN_TEXT_FILE_VIEWER_LINE_HEIGHT, MAX_TEXT_FILE_VIEWER_LINE_HEIGHT);
            } else {
                return true;
            }
        }


        //=========================================================================================
        // プロパティ：グラフィックビューア メッセージ描画用のフォント
        //=========================================================================================
        public float GraphicsViewerMessageFontSize {
            get {
                return 12;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア ファイル名描画用のフォント
        //=========================================================================================
        public float GraphicsViewerFileNameFontSize {
            get {
                return 10;
            }
        }

        //=========================================================================================
        // プロパティ：ファンクションバー 描画用のフォント名
        //=========================================================================================
        public string FunctionBarFontName {
            get {
                return CompleteFontName(m_functionBarFontName);
            }
            set {
                m_functionBarFontName = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファンクションバー 描画用のフォントの大きさ
        //=========================================================================================
        public float FunctionBarFontSize {
            get {
                return m_functionBarFontSize;
            }
            set {
                m_functionBarFontSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ表示用のフォント名
        //=========================================================================================
        public string LogWindowFontName {
            get {
                return CompleteFontName(m_logWindowFontName);
            }
            set {
                m_logWindowFontName = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ表示用のフォント名
        //=========================================================================================
        public string LogWindowFixedFontName {
            get {
                return m_logWindowFixedFontName;
            }
            set {
                m_logWindowFixedFontName = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ表示用のフォントサイズ
        //=========================================================================================
        public float LogWindowFontSize {
            get {
                return m_logWindowFontSize;
            }
            set {
                m_logWindowFontSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ表示用のフォントサイズ
        //=========================================================================================
        public float LogWindowTerminalFontSize {
            get {
                return m_logWindowTerminalFontSize;
            }
            set {
                m_logWindowTerminalFontSize = value;
            }
        }
    }
}
