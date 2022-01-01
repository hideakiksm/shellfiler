using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.Internal;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Properties;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.FileList.DefaultList;
using ShellFiler.UI.FileList.ThumbList;
using ShellFiler.Util;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：ファイル一覧を実装するためのWindowsコントロール
    //         内部の制御は差し替えて使用することを前提とし、IFileListViewComponentで実装する。
    //=========================================================================================
    public partial class FileListView : UserControl {
        // WM_PAINT受信時にマウスのドラッグを継続する
        public const int UM_CHECK_MOUSE_DRAG_CONTINUE = Win32API.WM_USER + 1;

        // メインウィンドウ
        private MainWindowForm m_mainWindowForm;

        // 反対パスのファイルリストビュー
        private FileListView m_oppositeFileListView;

        // 親となるパネル
        private FilePanel m_parent;

        // ファイル一覧
        private UIFileList m_fileList;

        // このウィンドウが左側のときtrue
        private bool m_isLeft;

        // ビュー
        private IFileListViewComponent m_fileListViewComponent;

        // 初回のpaint実行時のときtrue
        private bool m_firstPaint = true;

        // マウスイベントとしてDown/Upなど一連の動作を実行中のコマンド（null:動作中でない）
        private AbstractMouseActionCommand m_mouseActionCommand;

        // ドラッグ中に使用するタイマー
        private System.Windows.Forms.Timer m_dragDropTimer = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListView() {
            InitializeComponent();
            m_fileListViewComponent = null;

            // ダブルバッファリング有効
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]mainWindowForm メインウィンドウ
        // 　　　　[in]opposite       反対パスのファイルリストビュー
        // 　　　　[in]isLeft         このウィンドウが左側のときtrue
        // 　　　　[in]fileList       ファイル一覧
        // 　　　　[in]viewState      表示の初期状態
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(MainWindowForm mainWindowForm, FileListView opposite, FilePanel parent, bool isLeft, UIFileList fileList, IFileListViewState viewState) {
            m_mainWindowForm = mainWindowForm;
            m_parent = parent;
            m_oppositeFileListView = opposite;
            m_fileList = fileList;
            m_isLeft = isLeft;
            if (viewState is DefaultFileListViewState) {
                m_fileListViewComponent = new DefaultFileListViewComponent();
                m_fileListViewComponent.Initialize(this, viewState);
            } else if (viewState is ThumbListViewState) {
                m_fileListViewComponent = new ThumbListViewComponent();
                m_fileListViewComponent.Initialize(this, viewState);
            } else {
                Program.Abort("初期化するビューの種類が不明です。viewState:" + viewState.ToString());
            }
        }

        //=========================================================================================
        // 機　能：コンテンツをタブ情報から読み込んでUIに反映する
        // 引　数：[in]tabPageInfo   復元させるタブの状態
        // 戻り値：なし
        //=========================================================================================
        public void LoadTabContents(TabPageInfo tabPageInfo) {
            m_fileList = tabPageInfo.GetFileList(m_isLeft);
            m_fileList.OnTabChanged(tabPageInfo);
            IFileListViewState viewState = tabPageInfo.GetFileListViewState(m_isLeft);
            if (viewState is DefaultFileListViewState) {
                if (m_fileListViewComponent is DefaultFileListViewComponent) {
                    ;
                } else {
                    m_fileList.FileListViewMode = FileListViewMode.DefaultFileList();
                }
            } else {
                if (m_fileListViewComponent is ThumbListViewComponent) {
                    ;
                } else {
                    FileListViewMode viewMode = ((ThumbListViewState)viewState).FileListViewMode;
                    m_fileList.FileListViewMode = (FileListViewMode)(viewMode.Clone());
                }
            }
            RefreshViewComponentByViewMode();
            m_fileListViewComponent.LoadComponentViewState(viewState);
            RefreshViewMode(m_fileList.FileListViewMode);
        }

        //=========================================================================================
        // 機　能：コンテンツをUIからタブ情報に保存する
        // 引　数：[in]tabPageInfo   UI状態の保存先
        // 戻り値：なし
        //=========================================================================================
        public void SaveTabContents(TabPageInfo tabPageInfo) {
            m_fileList = tabPageInfo.GetFileList(m_isLeft);
            IFileListViewState viewState = m_fileListViewComponent.SaveComponentViewState();
            tabPageInfo.SetFileListViewState(m_isLeft, viewState);
        }

        //=========================================================================================
        // 機　能：ビューの表示モードを更新する
        // 引　数：[in]viewMode   新しい表示モード
        // 戻り値：なし
        //=========================================================================================
        public void RefreshViewMode(FileListViewMode viewMode) {
            // カーソル位置を保存
            string cursorFile = null;
            int index = m_fileListViewComponent.CursorLineNo;
            if (index < FileList.Files.Count) {
                cursorFile = FileList.Files[index].FileName;
            }

            // ファイル一覧を更新
            m_fileList.FileListViewMode = viewMode;
            RefreshViewComponentByViewMode();
            m_fileListViewComponent.OnRefreshDirectory(new ChangeDirectoryParam.UiOnly(cursorFile));
        }

        //=========================================================================================
        // 機　能：ビューの表示モードに基づいてビューを更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshViewComponentByViewMode() {
            FileListViewMode viewMode = m_fileList.FileListViewMode;
            if (!viewMode.ThumbnailModeSwitch) {
                // 詳細モードにする
                if (m_fileListViewComponent is DefaultFileListViewComponent) {
                    m_fileListViewComponent.RefreshViewMode(viewMode);
                } else {
                    m_fileListViewComponent.DisposeView();
                    m_fileListViewComponent = new DefaultFileListViewComponent();
                    m_fileListViewComponent.Initialize(this, DefaultFileListViewComponent.GetDefaultViewState());
                    m_fileListViewComponent.RefreshViewMode(viewMode);
                    this.Invalidate();
                }
            } else {
                // サムネイルモードにする
                if (m_fileListViewComponent is ThumbListViewComponent) {
                    m_fileListViewComponent.RefreshViewMode(viewMode);
                } else {
                    m_fileListViewComponent.DisposeView();
                    m_fileListViewComponent = new ThumbListViewComponent();
                    m_fileListViewComponent.Initialize(this, ThumbListViewComponent.GetDefaultViewState(viewMode));
                    m_fileListViewComponent.RefreshViewMode(viewMode);
                    this.Invalidate();
                }
            }
        }

        //=========================================================================================
        // 機　能：ウィンドウプロシージャ
        // 引　数：[in]message  ウィンドウメッセージ
        // 戻り値：なし
        //=========================================================================================
        [SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message message) {
            switch (message.Msg) {
                case Win32API.WM_VSCROLL:
                    OnVScroll(message);
                    return;
                case Win32API.WM_HSCROLL:
                    OnHScroll(message);
                    return;
                case UM_CHECK_MOUSE_DRAG_CONTINUE:
                    OnCheckMouseDragContinue();
                    return;
                case Win32API.WM_INITMENUPOPUP:
                case Win32API.WM_DRAWITEM:
                case Win32API.WM_MEASUREITEM:
                    if (m_fileListViewComponent != null) {
                        m_fileListViewComponent.HandleExplorerMenuMessage(message.Msg, message.WParam, message.LParam);
                    }
                    break;
            }
            base.WndProc(ref message);
        }
        
        //=========================================================================================
        // 機　能：入力処理を行ってもよいキーかどうかを判断する
        // 引　数：[in]keyData  キー
        // 戻り値：keyDataの処理をアプリケーション側で行う予定のときtrue
        //=========================================================================================
        protected override bool IsInputKey(Keys keyData) {
            return true;
        }

        //=========================================================================================
        // 機　能：描画イベント受信時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void fileListView_Paint(object sender, PaintEventArgs e) {
            if (m_fileListViewComponent == null) {
                return;
            }

            // 初回の描画時にサイズ変更イベントを発生させる
            if (m_firstPaint) {
                m_firstPaint = false;
                m_fileListViewComponent.OnSizeChange();
            }

            // 画面を描画
            m_fileListViewComponent.OnPaint(e.Graphics);

            // マウスのドラッグが継続状態かどうかを確認
            if (m_mouseActionCommand != null) {
                Win32API.Win32PostMessage(Handle, UM_CHECK_MOUSE_DRAG_CONTINUE, 0, 0);
            }
        }

        //=========================================================================================
        // 機　能：ビジュアルベルを表示する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ShowVisualBell() {
            if (m_fileListViewComponent == null) {
                return;
            }

            Graphics gPanel = this.CreateGraphics();
            try {
                DoubleBuffer buffer = new DoubleBuffer(this, this.Width, this.Height);
                try {
                    // 画面全体のコピーを作成
                    Brush backBrush = new SolidBrush(this.BackColor);
                    try {
                        buffer.DrawingGraphics.FillRectangle(backBrush, ClientRectangle);
                    } finally {
                        backBrush.Dispose();
                    }
                    m_fileListViewComponent.OnPaint(buffer.DrawingGraphics);

                    // 表示の縮尺（左下に流れるように）
                    RectangleF[] size = new RectangleF[4];
                    size[0] = new RectangleF(0.0f, 0.00f, 1.00f, 1.0f);
                    size[1] = new RectangleF(0.0f, 0.15f, 0.95f, 1.0f);
                    size[2] = new RectangleF(0.0f, 0.45f, 0.85f, 1.0f);
                    size[3] = new RectangleF(0.0f, 0.80f, 0.70f, 1.0f);
                    int[] sleepTime = { 15, 10, 8, 3};

                    // ビジュアルベルを表示
                    for (int i = 0; i < size.Length; i++) {
                        RectangleF rect = new RectangleF(this.Width * size[i].Left, this.Height * size[i].Top, this.Width * size[i].Width, this.Height * size[i].Height);
                        gPanel.FillRectangle(Brushes.Black, rect);
                        Thread.Sleep(15);
                        gPanel.DrawImage(buffer.BitmapBuffer, 0, 0);
                        Thread.Sleep(sleepTime[i]);
                    }
                } finally {
                    buffer.FlushNoUse();
                }
            } finally {
                gPanel.Dispose();
            }
            Invalidate();
        }

        //=========================================================================================
        // 機　能：ウィンドウサイズ変更時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListView_Resize(object sender, EventArgs evt) {
            this.Resize -= new EventHandler(FileListView_Resize);
            try {
                if (m_fileListViewComponent != null) {
                    m_fileListViewComponent.OnSizeChange();
                }
                this.Invalidate();
            } finally {
                this.Resize += new EventHandler(FileListView_Resize);
            }
        }

        //=========================================================================================
        // 機　能：UIでのコマンドが発生したことを通知する
        // 引　数：[in]sender  イベント発生原因の送信元の種別
        // 　　　　[in]item    発生したイベントの項目
        // 戻り値：なし
        //=========================================================================================
        public void OnUICommand(UICommandSender sender, UICommandItem item) {
            if (m_fileListViewComponent == null) {
                return;
            }

            // 項目に対応するコマンドを取得して実行
            FileListActionCommand command = Program.Document.CommandFactory.CreateFromUICommand(sender, item, this);
            if (command != null) {
                FileListCommandRuntime runtime = new FileListCommandRuntime(command);
                runtime.Execute();
            }
        }

        //=========================================================================================
        // 機　能：アドレスバーでのコマンドが発生したことを通知する
        // 引　数：[in]inputDir  入力されたディレクトリ名（フィルター設定のときnull）
        // 戻り値：なし
        //=========================================================================================
        public void OnAddressBarCommand(string inputDir) {
            if (m_fileListViewComponent == null) {
                return;
            }

            // 項目に対応するコマンドを取得して実行
            FileListActionCommand command = Program.Document.CommandFactory.CreateFromAddressBarCommand(inputDir, this);
            if (command != null) {
                FileListCommandRuntime runtime = new FileListCommandRuntime(command);
                runtime.Execute();
            }
        }

        //=========================================================================================
        // 機　能：ファイル一覧取得が完了した通知をバックグラウンドタスクから受け取ったときの処理を行う
        // 引　数：[in]status             一覧取得のステータス
        // 　　　　[in]fileList           ファイル一覧（失敗のときはnull）
        // 　　　　[in]nextVirtualFolder  次に使用する仮想フォルダ情報（エラーのとき処理途中の情報、仮想dir以外はnull）
        // 　　　　[in]chdirMode          ディレクトリ変更のパラメータ
        // 戻り値：なし
        //=========================================================================================
        public void OnNotifyUIFileListTaskEnd(FileOperationStatus status, IFileList fileList, VirtualFolderInfo nextVirtualFolder, ChangeDirectoryParam chdirParam) {
            if (status.Succeeded) {
                FileListViewMode viewModePrev = (FileListViewMode)(m_fileList.FileListViewMode.Clone());
                bool swapSuccess = m_fileList.SwapLoadedFileList(status, fileList, nextVirtualFolder, chdirParam);
                if (swapSuccess) {
                    if (!FileListViewMode.EqualsConfig(viewModePrev, m_fileList.FileListViewMode)) {
                        RefreshViewComponentByViewMode();
                    }
                    m_fileListViewComponent.OnRefreshDirectory(chdirParam);
                }
            } else {
                m_fileList.SwapLoadedFileList(status, fileList, nextVirtualFolder, chdirParam);
                if (status == FileOperationStatus.FailedChangeUser) {
                    InfoBox.Warning(Program.MainWindow, Resources.Msg_ShellChangeUserError);
                } else if (status == FileOperationStatus.FailedChangeUser) {
                    InfoBox.Warning(Program.MainWindow, Resources.Msg_ShellExitError);
                } else {
                    ChdirCommand.ShowChangeDirErrorMessgage(this, chdirParam, m_fileList.PathHistory);
                }
            }
            // ステータスバーを更新
            if (m_fileList.FileSystem.FileSystemId == m_parent.Opposite.FileListView.FileList.FileSystem.FileSystemId) {
                // Shellでのユーザー切り替えの結果反映のため
                m_parent.Opposite.StatusBar.RefreshMarkInfo();
            }
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]evt   送信イベント
        // 戻り値：キー入力したときtrue
        //=========================================================================================
        public bool OnKeyDown(KeyCommand evt) {
            if (m_fileListViewComponent == null) {
                return false;
            }

            // キーに対応するコマンドを取得して実行
            Keys key = evt.KeyCode;
            FileListActionCommand command = Program.Document.CommandFactory.CreateFromKeyInput(evt, this);
            if (command != null) {
                FileListCommandRuntime runtime = new FileListCommandRuntime(command);
                runtime.Execute();
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：垂直スクロールイベントを処理する
        // 引　数：[in]message  ウィンドウメッセージ
        // 戻り値：なし
        //=========================================================================================
        private void OnVScroll(Message message) {
            int newValue = Win32API.HiWord((int)(message.WParam));
            ScrollEventType type;
            switch (Win32API.LoWord((int)(message.WParam))) {
                case Win32API.SB_LINEUP:
                    type = ScrollEventType.SmallDecrement;
                    break;
                case Win32API.SB_PAGEUP:
                    type = ScrollEventType.LargeDecrement;
                    break;
                case Win32API.SB_LINEDOWN:
                    type = ScrollEventType.SmallIncrement;
                    break;
                case Win32API.SB_PAGEDOWN:
                    type = ScrollEventType.LargeIncrement;
                    break;
                case Win32API.SB_THUMBPOSITION:
                    type = ScrollEventType.ThumbPosition;
                    break;
                case Win32API.SB_THUMBTRACK:
                    type = ScrollEventType.ThumbTrack;
                    break;
                default:
                    return;
            }
            ScrollEventArgs evt = new ScrollEventArgs(type, newValue);
            m_fileListViewComponent.OnVScroll(evt);
        }

        //=========================================================================================
        // 機　能：水平スクロールイベントを処理する
        // 引　数：[in]message  ウィンドウメッセージ
        // 戻り値：なし
        //=========================================================================================
        private void OnHScroll(Message message) {
            int newValue = Win32API.HiWord((int)(message.WParam));
            ScrollEventType type;
            switch (Win32API.LoWord((int)(message.WParam))) {
                case Win32API.SB_LINEUP:
                    type = ScrollEventType.SmallDecrement;
                    break;
                case Win32API.SB_PAGEUP:
                    type = ScrollEventType.LargeDecrement;
                    break;
                case Win32API.SB_LINEDOWN:
                    type = ScrollEventType.SmallIncrement;
                    break;
                case Win32API.SB_PAGEDOWN:
                    type = ScrollEventType.LargeIncrement;
                    break;
                case Win32API.SB_THUMBPOSITION:
                    type = ScrollEventType.ThumbPosition;
                    break;
                case Win32API.SB_THUMBTRACK:
                    type = ScrollEventType.ThumbTrack;
                    break;
                default:
                    return;
            }
            ScrollEventArgs evt = new ScrollEventArgs(type, newValue);
            m_fileListViewComponent.OnHScroll(evt);
        }

        //=========================================================================================
        // 機　能：マウスホイールイベントを処理する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListView_MouseWheel(object sender, MouseEventArgs evt) {
            m_fileListViewComponent.OnMouseWheel(evt);
        }

        //=========================================================================================
        // 機　能：マウスのボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListView_MouseDown(object sender, MouseEventArgs evt) {
            if (m_fileListViewComponent == null) {
                return;
            }

            // 直前のマウスコマンドの途中に別のボタンが押されたときは解放
            if (m_mouseActionCommand != null) {
                Capture = false;
                m_mouseActionCommand = null;
            }

            // マウスに対応するコマンドを取得して実行
            FileListActionCommand command = Program.Document.CommandFactory.CreateFromMouseInput(evt, this, false);
            if (command != null) {
                FileListCommandRuntime runtime = new FileListCommandRuntime(command);
                runtime.Execute();
            }

            // マウスイベントなら一連の動作を登録
            if (command is AbstractMouseActionCommand) {
                m_mouseActionCommand = (AbstractMouseActionCommand)command;
                Capture = true;
            }
        }

        //=========================================================================================
        // 機　能：マウスのボタンがダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListView_MouseDoubleClick(object sender, MouseEventArgs evt) {
            if (m_fileListViewComponent == null) {
                return;
            }

            // マウスに対応するコマンドを取得して実行
            FileListActionCommand command = Program.Document.CommandFactory.CreateFromMouseInput(evt, this, true);
            if (command != null) {
                FileListCommandRuntime runtime = new FileListCommandRuntime(command);
                runtime.Execute();
            }
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListView_MouseMove(object sender, MouseEventArgs evt) {
            if (m_mouseActionCommand != null) {
                m_mouseActionCommand.OnMouseMove();
            }
            if (m_fileListViewComponent != null) {
                bool dragPos = m_fileListViewComponent.CheckDragStartPosition(evt.X, evt.Y);
                if (dragPos) {
                    if (this.Cursor == Cursors.Default) {
                        this.Cursor = UIIconManager.HandCursor;
                    }
                } else {
                    if (this.Cursor != Cursors.Default) {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：マウスがドラッグ中の処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void OnCheckMouseDragContinue() {
            if (m_mouseActionCommand != null) {
                m_mouseActionCommand.OnMouseMove();
            }
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListView_MouseUp(object sender, MouseEventArgs evt) {
            if (m_mouseActionCommand != null) {
                m_mouseActionCommand.OnMouseUp();
                Capture = false;
                m_mouseActionCommand = null;
            }
        }

        //=========================================================================================
        // 機　能：外部へのファイルのドラッグ＆ドロップを行う
        // 引　数：[in]fileList  ドロップ対象のファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public void BeginDragDrop(string[] fileList) {
            // ウィンドウを隠すためのタイマーを初期化
            bool hideDragDrop = Configuration.Current.HideWindowDragDrop;
            m_dragDropTimer = new System.Windows.Forms.Timer();
            if (hideDragDrop) {
                m_dragDropTimer.Tick += new EventHandler(DragDropTimer_Tick);
                m_dragDropTimer.Interval = 100;
                m_dragDropTimer.Start();
            }

            // ドラッグ開始
            DataObject data = new DataObject(DataFormats.FileDrop, fileList);
            this.DoDragDrop(data, DragDropEffects.All);
            
            // タイマーを戻す
            if (hideDragDrop) {
                m_dragDropTimer.Stop();
                Program.MainWindow.Show();
            }
            m_dragDropTimer.Dispose();
            m_dragDropTimer = null;
        }

        //=========================================================================================
        // 機　能：外部へのドラッグ＆ドロップ中のタイマーの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void DragDropTimer_Tick(object sender, EventArgs evt) {
            if (Configuration.Current.HideWindowDragDrop) {
                Point pos = Cursor.Position;
                MainWindowForm mainWnd = Program.MainWindow;
                if (pos.X < mainWnd.Left || pos.X > mainWnd.Right || pos.Y < mainWnd.Top || pos.Y > mainWnd.Bottom) {
                    mainWnd.Hide();
                }
            }
        }

        //=========================================================================================
        // 機　能：外部からのドロップを受け付けてよいかどうかを確認する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListView_DragEnter(object sender, DragEventArgs evt) {
            // 自分へのドロップは受け付けない
            if (m_dragDropTimer != null) {
                evt.Effect = DragDropEffects.None;
                return;
            }

            // ファイル以外は受け付けない
            if (evt.Data.GetDataPresent(DataFormats.FileDrop)) {
                evt.Effect = DragDropEffects.Copy;
            } else {
                evt.Effect = DragDropEffects.None;
            }
        }

        //=========================================================================================
        // 機　能：ドラッグ＆ドロップが発生したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListView_DragDrop(object sender, DragEventArgs evt) {
            if (m_oppositeFileListView.m_dragDropTimer != null) {
                // 反対パスからのドロップ
                DragDropFromOpposite();
            } else {
                // 他のアプリケーションからのドロップ
                string[] fileNameList = (string[])evt.Data.GetData(DataFormats.FileDrop, false);
                DragDropFromOtherApp(fileNameList);
            }
        }

        //=========================================================================================
        // 機　能：反対パスからのドラッグ＆ドロップが発生したときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void DragDropFromOpposite() {
            // 仮想フォルダでは実行不可
            bool canStart = BackgroundTaskCommandUtil.CheckAllowedNonVirtualFolder(Program.MainWindow, this.FileList.FileSystem.FileSystemId, true);
            if (!canStart) {
                return;
            }

            // ダイアログで入力
            DropActionInternalDialog dialog = new DropActionInternalDialog();
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }

            // 選択した結果を実行
            FileListActionCommand command = null;
            switch (dialog.ResultAction) {
                case DropActionInternalDialog.ResultActionType.ChangeDir:
                    command = Program.Document.CommandFactory.CreateFromMoniker(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirOppositeToTargetCommand)), m_oppositeFileListView);
                    break;
                case DropActionInternalDialog.ResultActionType.Copy:
                    command = Program.Document.CommandFactory.CreateFromMoniker(new ActionCommandMoniker(ActionCommandOption.None, typeof(CopyCommand)), m_oppositeFileListView);
                    break;
                case DropActionInternalDialog.ResultActionType.Move:
                    command = Program.Document.CommandFactory.CreateFromMoniker(new ActionCommandMoniker(ActionCommandOption.None, typeof(MoveCommand)), m_oppositeFileListView);
                    break;
                case DropActionInternalDialog.ResultActionType.Shortcut:
                    command = Program.Document.CommandFactory.CreateFromMoniker(new ActionCommandMoniker(ActionCommandOption.None, typeof(CreateShortcutCommand)), m_oppositeFileListView);
                    break;
            }
            FileListCommandRuntime runtime = new FileListCommandRuntime(command);
            runtime.Execute();
        }

        //=========================================================================================
        // 機　能：外部からのドラッグ＆ドロップが発生したときの処理を行う
        // 引　数：[in]fileNameList   ドロップされたファイル一覧
        // 戻り値：なし
        //=========================================================================================
        private void DragDropFromOtherApp(string[] fileNameList) {
            // 有効な一覧を確認
            List<SimpleFileDirectoryPath> dropFileList = DropActionExternalDialog.CreateDropFileList(fileNameList);
            if (dropFileList.Count == 0) {
                InfoBox.Warning(Program.MainWindow, Resources.DlgDropAction_NoAvailableFile);
                return;
            } else if (dropFileList.Count != fileNameList.Length) {
                InfoBox.Information(Program.MainWindow, Resources.DlgDropAction_SomeFileNotAvailable);
            }

            // ダイアログで入力
            bool isVirtual = FileSystemID.IsVirtual(FileList.FileSystem.FileSystemId);
            DropActionExternalDialog dialog = new DropActionExternalDialog(dropFileList, !isVirtual);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }

            // 選択した処理を実行
            FileListActionCommand command = null;
            switch (dialog.ResultAction) {
                case DropActionExternalDialog.ResultActionType.ChangeDir:
                    ChdirCommand.ChangeDirectory(this, dialog.ChangeDirectoryParam);
                    break;
                case DropActionExternalDialog.ResultActionType.Copy:
                    command = Program.Document.CommandFactory.CreateFromMoniker(new ActionCommandMoniker(ActionCommandOption.None, typeof(CopyDirectInternalCommand), dropFileList, FileSystemID.Windows), m_oppositeFileListView);
                    break;
                case DropActionExternalDialog.ResultActionType.Move:
                    command = Program.Document.CommandFactory.CreateFromMoniker(new ActionCommandMoniker(ActionCommandOption.None, typeof(MoveDirectInternalCommand), dropFileList, FileSystemID.Windows), m_oppositeFileListView);
                    break;
                case DropActionExternalDialog.ResultActionType.Shortcut:
                    command = Program.Document.CommandFactory.CreateFromMoniker(new ActionCommandMoniker(ActionCommandOption.None, typeof(CreateShortcutDirectInternalCommand), dropFileList, FileSystemID.Windows), m_oppositeFileListView);
                    break;
            }
            if (command != null) {
                FileListCommandRuntime runtime = new FileListCommandRuntime(command);
                runtime.Execute();
            }
        }

        //=========================================================================================
        // 機　能：コンテキストメニューを表示する
        // 引　数：[in]fileName    対象のファイル名
        // 　　　　[in]menuPos     メニューを表示する位置
        // 戻り値：なし
        //=========================================================================================
        public void ExplorerMenu(string fileName, ContextMenuPosition menuPos) {
            // キー入力イベントで処理するとイベントの競合でBeep音が鳴るため、
            // メッセージキュー経由で実行
            m_parent.BeginInvoke(new ExplorerMenuDelegate(ExplorerMenuUI), fileName, menuPos);
        }
        private delegate void ExplorerMenuDelegate(string fileName, ContextMenuPosition menuPos);
        private void ExplorerMenuUI(string fileName, ContextMenuPosition menuPos) {
            m_fileListViewComponent.ContextMenuImpl(fileName, menuPos);
        }

        //=========================================================================================
        // 機　能：カーソルの左右を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ToggleCursorLeftRight() {
            m_mainWindowForm.ToggleCursorLeftRight();
        }
        
        //=========================================================================================
        // 機　能：アドレスバーにフォーカスを設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetFocusAddressbar() {
            m_parent.AddressBar.AddressBarDropDown.TextBoxInput.Focus();
        }

        //=========================================================================================
        // 機　能：状態一覧パネルのアクティブ状態を設定する
        // 引　数：[in]isActive  状態一覧パネルがアクティブになったときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnActivateStateListPanel(bool isActive) {
            this.Invalidate();
            m_fileListViewComponent.OnActivateStateListPanel(isActive);
        }

        //=========================================================================================
        // プロパティ：ファイルリストビューコンポーネント
        //=========================================================================================
        public IFileListViewComponent FileListViewComponent {
            get {
                return m_fileListViewComponent;
            }
        }

        //=========================================================================================
        // プロパティ：反対パスのファイルリストビュー
        //=========================================================================================
        public FileListView OppositeFileListView {
            get {
                return m_oppositeFileListView;
            }
        }

        //=========================================================================================
        // プロパティ：メインウィンドウ
        //=========================================================================================
        public MainWindowForm MainWindow {
            get {
                return m_mainWindowForm;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧
        //=========================================================================================
        public UIFileList FileList {
            get {
                return m_fileList;
            }
        }

        //=========================================================================================
        // プロパティ：このウィンドウが左側のときtrue
        //=========================================================================================
        public bool IsLeft {
            get {
                return m_isLeft;
            }
        }

        //=========================================================================================
        // プロパティ：このウィンドウがカーソルを持っているときtrue
        //=========================================================================================
        public bool HasCursor {
            get {
                return (Program.Document.CurrentTabPage.IsCursorLeft == m_isLeft);
            }
        }

        //=========================================================================================
        // プロパティ：親となるパネル
        //=========================================================================================
        public FilePanel ParentPanel {
            get {
                return m_parent;
            }
        }

        //=========================================================================================
        // プロパティ：水平スクロールバーの位置
        //=========================================================================================
        public int HorzScrollPosition {
            get {
                return m_fileListViewComponent.HorzScrollPosition;
            }
        }
    }
}
