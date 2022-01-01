using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.Document;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.FileViewer;
using ShellFiler.Util;
using ShellFiler.FileTask;
using ShellFiler.FileSystem;
using ShellFiler.UI.FileList;
using ShellFiler.UI.StateList;
using ShellFiler.GraphicsViewer;
using ShellFiler.Properties;

namespace ShellFiler.UI {

    //=========================================================================================
    // クラス：メインウィンドウのUI
    //=========================================================================================
    public partial class MainWindowForm : Form, IUICommandTarget, IKeyEventIntegrator, ITwoStrokeKeyForm {
        // ウィンドウ境界の最小値
        public const int FILE_LIST_SPLIT_BORDER_MIN = 32;

        // ウィンドウサイズの最小幅
        public const int FILE_WINDOW_MIN_CX = 200;

        // ウィンドウサイズの最小高
        public const int FILE_WINDOW_MIN_CY = 100;

        // 終了処理中のときtrue
        private bool m_terminateProcess = false;

        // タブの制御クラス
        private TabControlImpl m_tabControlImpl;

        // スライドショーマーク結果一覧のダイアログ（null:ダイアログを開いていない）
        private SlideShowMarkResultDialog m_slideShowMarkResultDialog = null;

        // 表示中のエラー情報ダイアログ
        private List<FileOperationErrorListDialog> m_errorDialogList = new List<FileOperationErrorListDialog>();

        // フォーカスの位置
        private ActiveControlType m_activeControl = ActiveControlType.FileListView;

        // カーソルの左右に変化が生じたときの通知用delegate
        public delegate void CursorLRChangedHandler(object sender, EventArgs evt); 

        // カーソルの左右に変化が生じたときに通知するイベント
        public event CursorLRChangedHandler CursorLRChanged;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MainWindowForm() {
            InitializeComponent();
            this.Icon = Resources.ShellFilerMain;
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
            Program.Document.UserSetting.InitialSetting.InitializeMainWindow(this);
            m_tabControlImpl = new TabControlImpl(this, this.tabControlMain, this.splitContainerFile);

            this.stateListPanel.Initialize();
            TabPageInfo tabPageInfo = Program.Document.LoadFileList();
            Program.Document.CurrentTabPage = tabPageInfo;
            m_tabControlImpl.AddTabPageUI(tabPageInfo);

            this.functionBar.Initialize(this, UICommandSender.MainFunctionBar, this, Program.Document.KeySetting.FileListKeyItemList, true);
            this.menuStripMain.Initialize(UICommandSender.MainMenubar, Program.MainWindow, CommandUsingSceneType.FileList);
            this.toolStripMain.Initialize(UICommandSender.MainToolbar, Program.MainWindow, CommandUsingSceneType.FileList);
            this.panelLeft.Initialize(this, tabPageInfo, true, panelRight);
            this.panelRight.Initialize(this, tabPageInfo, false, panelLeft);
            this.logPanel.Initialize();
            m_tabControlImpl.SetCurrentTabPageTitle();
            SetWindowTitle();
            RefreshUIStatus();
            SetFileListBorderRatio(50);
            ToggleLogWindowSize();
            Application.Idle += new EventHandler(Application_Idle);
        }

        //=========================================================================================
        // 機　能：UIの項目をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetUIItems() {
            this.menuStripMain.ResetItems();
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void MainWindowForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs evt) {
            evt.IsInputKey = true;
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void MainWindowForm_KeyDown(object sender, KeyEventArgs evt) {
            evt.Handled = true;
            OnKeyDown(new KeyCommand(evt));
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void MainWindowForm_KeyUp(object sender, KeyEventArgs evt) {
            OnKeyUp(new KeyCommand(evt));
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]key  キーコマンド
        // 戻り値：キー入力したときtrue
        //=========================================================================================
        public bool OnKeyDown(KeyCommand key) {
            bool handled;
            if (Program.Document.CurrentTabPage.IsCursorLeft) {
                handled = this.panelLeft.FileListView.OnKeyDown(key);
            } else {
                handled = this.panelRight.FileListView.OnKeyDown(key);
            }
            this.functionBar.OnKeyChange(key);
            return handled;
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う（OnKeyDown処理後）
        // 引　数：[in]key  入力した文字
        // 戻り値：キー入力したときtrue
        //=========================================================================================
        public bool OnKeyChar(char key) {
            return false;
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]key  キーコマンド
        // 戻り値：なし
        //=========================================================================================
        public void OnKeyUp(KeyCommand key) {
            this.functionBar.OnKeyChange(key);
        }

        //=========================================================================================
        // 機　能：状態一覧パネルのアクティブ状態を設定する
        // 引　数：[in]isActive  状態一覧パネルがアクティブになったときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnActivateStateListPanel(bool isActive) {
            if (isActive) {
                m_activeControl = ActiveControlType.StateListPanel;
            } else {
                m_activeControl = ActiveControlType.FileListView;
            }
            this.panelLeft.FileListView.OnActivateStateListPanel(isActive);
            this.panelRight.FileListView.OnActivateStateListPanel(isActive);
        }

        //=========================================================================================
        // 機　能：ウィンドウが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainWindowForm_FormClosing(object sender, FormClosingEventArgs evt) {
            // バックグラウンドタスクが実行中のときは閉じられない
            int taskCount = Program.Document.BackgroundTaskManager.Count;
            if (taskCount > 0) {
                InfoBox.Warning(this, Resources.Msg_BackgroundTask, taskCount);
                evt.Cancel = true;
                return;
            }

            // 終了処理
            if (m_slideShowMarkResultDialog != null) {
                m_slideShowMarkResultDialog.Close();
                m_slideShowMarkResultDialog.Dispose();
                m_slideShowMarkResultDialog = null;
            }
            Program.Document.Dispose();
        } 

        //=========================================================================================
        // 機　能：ウィンドウが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainWindowForm_FormClosed(object sender, FormClosedEventArgs evt) {
            m_terminateProcess = true;
        }

        //=========================================================================================
        // 機　能：ウィンドウがアクティブになったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainWindowForm_Activated(object sender, EventArgs evt) {
            Keys modKey = Control.ModifierKeys;
            bool shift = ((modKey & Keys.Shift) == Keys.Shift);
            bool control = ((modKey & Keys.Control) == Keys.Control);
            bool alt = ((modKey & Keys.Alt) == Keys.Alt);
            KeyCommand key = new KeyCommand(Keys.None, shift, control, alt);
            this.functionBar.OnKeyChange(key);
        }

        //=========================================================================================
        // 機　能：ファイルウィンドウのスプリットのサイズが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void splitContainerFile_SizeChanged(object sender, EventArgs evt) {
            if (this.LeftFileListView.FileListViewComponent != null) {
                this.LeftFileListView.FileListViewComponent.OnSplitChanged();
                this.RightFileListView.FileListViewComponent.OnSplitChanged();
            }
        }

        //=========================================================================================
        // 機　能：ファイルウィンドウのスプリットの分割位置が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void splitContainerFile_SplitterMoved(object sender, SplitterEventArgs evt) {
            splitContainerFile_SizeChanged(null, null);
        }

        //=========================================================================================
        // 機　能：スプリットの境界線でマウスが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SplitContainer_MouseUp(object sender, MouseEventArgs evt) {
            bool cursorLeft = Program.Document.CurrentTabPage.IsCursorLeft;
            if (cursorLeft) {
                this.panelLeft.FileListView.Focus();
            } else {
                this.panelRight.FileListView.Focus();
            }
        }

        //=========================================================================================
        // 機　能：カーソルの左右を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ToggleCursorLeftRight() {
            // データを設定
            bool cursorLeft = Program.Document.CurrentTabPage.IsCursorLeft;
            cursorLeft = !cursorLeft;
            Program.Document.CurrentTabPage.IsCursorLeft = cursorLeft;
            
            // フォーカスを設定
            this.panelLeft.InvalidateView();
            this.panelRight.InvalidateView();
            if (cursorLeft) {
                this.panelLeft.FileListView.Focus();
            } else {
                this.panelRight.FileListView.Focus();
            }

            // UIに反映
            RefreshUIStatus();
            CursorLRChanged(this, new EventArgs());
            SetWindowTitle();
        }

        //=========================================================================================
        // 機　能：特殊キーかどうかを判断する
        // 引　数：[in]keyData 判断するキー
        // 戻り値：通常のキーとするときtrue
        //=========================================================================================
        protected override bool IsInputKey(Keys keyData) {
            return true;
        }

        //=========================================================================================
        // 機　能：ドライブ一覧を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshDriveList() {
            this.toolStripMain.RefreshToolbarDriveList();
        }

        //=========================================================================================
        // 機　能：UIの状態を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshUIStatus() {
            UIItemRefreshContext context = new UIItemRefreshContext();

            this.menuStripMain.RefreshMenuStatus(context);
            this.toolStripMain.RefreshToolbarStatus(context);
            this.panelLeft.AddressBar.RefreshToolbarStatus(context);
            this.panelRight.AddressBar.RefreshToolbarStatus(context);
            this.functionBar.RefreshButtonStatus(context);
            m_tabControlImpl.SetCurrentTabPageTitle();
            SetWindowTitle();
        }

        //=========================================================================================
        // 機　能：ウィンドウタイトルを設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetWindowTitle() {
            string path;
            TabPageInfo tabPage = Program.Document.CurrentTabPage;
            if (Program.Document.CurrentTabPage.IsCursorLeft) {
                path = tabPage.LeftFileList.DisplayDirectoryName;
            } else {
                path = tabPage.RightFileList.DisplayDirectoryName;
            }
            path = GenericFileStringUtils.RemoveLastDirectorySeparator(path);
            string title = string.Format(Resources.WindowTitleMain, path);
            this.Text = title;
        }

        //=========================================================================================
        // 機　能：UIでのコマンドが発生したことを通知する
        // 引　数：[in]sender  イベント発生原因の送信元の種別
        // 　　　　[in]item    発生したイベントの項目
        // 戻り値：なし
        //=========================================================================================
        public void OnUICommand(UICommandSender sender, UICommandItem item) {
            if (Program.Document.CurrentTabPage.IsCursorLeft) {
                this.panelLeft.FileListView.OnUICommand(sender, item);
            } else {
                this.panelRight.FileListView.OnUICommand(sender, item);
            }
        }

        //=========================================================================================
        // 機　能：UIでのコマンドが発生したことを通知する
        // 引　数：[in]inputDir  入力されたディレクトリ名（フィルター設定のときnull）
        // 戻り値：なし
        //=========================================================================================
        public void OnAddressBarCommand(string inputDir) {
            if (Program.Document.CurrentTabPage.IsCursorLeft) {
                this.panelLeft.FileListView.OnAddressBarCommand(inputDir);
            } else {
                this.panelRight.FileListView.OnAddressBarCommand(inputDir);
            }
        }

        //=========================================================================================
        // 機　能：ウィンドウ境界を移動する
        // 引　数：[in]pixel  移動するピクセル数（右側がプラス）
        // 戻り値：新しい境界
        //=========================================================================================
        public int MoveFileListBorder(int pixel) {
            int current = this.splitContainerFile.SplitterDistance;
            current = Math.Max(FILE_LIST_SPLIT_BORDER_MIN, Math.Min(current + pixel, this.splitContainerFile.Width - FILE_LIST_SPLIT_BORDER_MIN));
            this.splitContainerFile.SplitterDistance = current;
            return current;
        }
        
        //=========================================================================================
        // 機　能：ウィンドウ境界の割合を設定する
        // 引　数：[in]ratio  移動する割合（0～100）
        // 戻り値：新しい境界
        //=========================================================================================
        public int SetFileListBorderRatio(int ratio) {
            int width = this.splitContainerFile.Width - FILE_LIST_SPLIT_BORDER_MIN * 2;
            int distance = width * ratio / 100 + FILE_LIST_SPLIT_BORDER_MIN;
            this.splitContainerFile.SplitterDistance = distance;
            return distance;
        }

        //=========================================================================================
        // 機　能：ログウィンドウのサイズを切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ToggleLogWindowSize() {
            int normalHeight = (this.Height - this.menuStripMain.Height - this.toolStripMain.Height - this.functionBar.Height) * 2 / 3;
            if (this.splitContainerMain.SplitterDistance != normalHeight) {
                this.splitContainerMain.SplitterDistance = normalHeight;
            } else {
                this.splitContainerMain.SplitterDistance = 150;
            }
        }

        //=========================================================================================
        // 機　能：エラー情報ダイアログを開く
        // 引　数：[in]errorInfo  ダイアログに表示するエラー情報
        // 戻り値：なし
        //=========================================================================================
        public void ShowFileErrorDialog(FileErrorInfo errorInfo) {
            FileListView fileListViewTarget, fileListViewOpposite;
            if (Program.Document.CurrentTabPage.IsCursorLeft) {
                fileListViewTarget = LeftFileListView;
                fileListViewOpposite = RightFileListView;
            } else {
                fileListViewTarget = RightFileListView;
                fileListViewOpposite = LeftFileListView;
            }
            
            FileOperationErrorListDialog errorDialog = new FileOperationErrorListDialog(errorInfo, fileListViewTarget, fileListViewOpposite);
            m_errorDialogList.Add(errorDialog);
            errorDialog.Show(this);
        }

        //=========================================================================================
        // 機　能：エラー情報ダイアログが閉じられたときの処理を行う
        // 引　数：[in]errorInfo  エラー情報ダイアログ
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseFileErrorDialog(FileOperationErrorListDialog errorDialog) {
            m_errorDialogList.Remove(errorDialog);
        }

        //=========================================================================================
        // 機　能：アプリケーションがアイドル状態に入ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void Application_Idle(object sender, EventArgs evt) {
            Application.Idle -= new EventHandler(Application_Idle);
        }

        //=========================================================================================
        // 機　能：スライドショーの結果を表示する
        // 引　数：[in]retry   新しい状態で再描画するときtrue
        // 戻り値：なし
        //=========================================================================================
        public void ShowSlideShowResult(bool retry, SlideShowMarkResult markResult) {
            // 表示のみのときに表示中なら何もしない
            if (!retry && m_slideShowMarkResultDialog != null) {
                return;
            }

            // 表示中のダイアログをクローズ
            if (m_slideShowMarkResultDialog != null) {
                m_slideShowMarkResultDialog.ForceClose = true;
                m_slideShowMarkResultDialog.Close();
                m_slideShowMarkResultDialog = null;
            }

            // 結果ダイアログを表示
            m_slideShowMarkResultDialog = new SlideShowMarkResultDialog(markResult);
            if (Program.Document.CurrentTabPage.IsCursorLeft) {
                FileListViewCommandUtils.MoveDialogOuterArea(this.LeftFileListView, m_slideShowMarkResultDialog);
            } else {
                FileListViewCommandUtils.MoveDialogOuterArea(this.RightFileListView, m_slideShowMarkResultDialog);
            }
            m_slideShowMarkResultDialog.Show(this);
            Program.Document.SlideShowMarkResult = markResult;
        }

        //=========================================================================================
        // 機　能：スライドショーの結果ダイアログが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseSlideShowResult() {
            m_slideShowMarkResultDialog = null;
        }

        //=========================================================================================
        // 機　能：2ストロークキーの状態が変わったことを通知する
        // 引　数：[in]newState  新しい状態
        // 戻り値：なし
        //=========================================================================================
        public void TwoStrokeKeyStateChanged(TwoStrokeType newState) {
            this.toolStripMain.TwoStrokeKeyStateChanged(newState);
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ
        //=========================================================================================
        public LogPanel LogWindow {
            get {
                return this.logPanel;
            }
        }

        //=========================================================================================
        // プロパティ：左側のファイル一覧
        //=========================================================================================
        public FileListView LeftFileListView {
            get {
                return this.panelLeft.FileListView;
            }
        }

        //=========================================================================================
        // プロパティ：右側のファイル一覧
        //=========================================================================================
        public FileListView RightFileListView {
            get {
                return this.panelRight.FileListView;
            }
        }

        //=========================================================================================
        // プロパティ：メニュー
        //=========================================================================================
        public MainMenuStrip MainMenu {
            get {
                return this.menuStripMain;
            }
        }

        //=========================================================================================
        // プロパティ：ツールバー
        //=========================================================================================
        public MainToolbarStrip MainToolbar {
            get {
                return this.toolStripMain;
            }
        }

        //=========================================================================================
        // プロパティ：ファンクションバー
        //=========================================================================================
        public FunctionBar FunctionBar {
            get {
                return this.functionBar;
            }
        }

        //=========================================================================================
        // プロパティ：状態一覧パネル
        //=========================================================================================
        public StateListPanel StateListPanel {
            get {
                return this.stateListPanel;
            }
        }

        //=========================================================================================
        // プロパティ：タブの制御クラス
        //=========================================================================================
        public TabControlImpl TabControlImpl {
            get {
                return m_tabControlImpl;
            }
        }

        //=========================================================================================
        // プロパティ：終了処理中のときtrue
        //=========================================================================================
        public bool TerminateProcess {
            get {
                return m_terminateProcess;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧がアクティブになっているときtrue
        //=========================================================================================
        public bool IsFileListViewActive {
            get {
                return m_activeControl == ActiveControlType.FileListView;
            }
        }

        //=========================================================================================
        // プロパティ：アクティブになっているウィンドウの種類
        //=========================================================================================
        public enum ActiveControlType {
            FileListView,               // ファイル一覧またはその周辺（ウィンドウ境界など）
            StateListPanel,             // 状態一覧パネル
        }
    }
}
