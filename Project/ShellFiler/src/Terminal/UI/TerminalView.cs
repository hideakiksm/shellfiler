using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Properties;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.Terminal.UI {

    //=========================================================================================
    // クラス：ターミナルのビュー
    //=========================================================================================
    public partial class TerminalView : ScrollableControl, ILogViewContainer, IConsoleScreenEvent, ITerminalWindow {
        // WM_PAINT受信時にマウスのドラッグを継続する
        public const int UM_CHECK_MOUSE_DRAG_CONTINUE = Win32API.WM_USER + 1;

        // キー入力の統合先（親フォーム）
        private IKeyEventIntegrator m_keyIntegrator;

        // コマンドの送信先
        private IUICommandTarget m_commandTarget;

        // ログパネルの実装
        private LogViewImpl m_logViewImpl;

        // ステータスバー
        private TerminalStatusBar m_statusBar;

        // ターミナルのコンソール画面(所有はTerminalPanel)
        private ConsoleScreen m_consoleScreen;

        // 反対のビュー（反対のビューがないときnull）
        private TerminalView m_oppositeView;

        // コンソール上のカレット（入力をサポートしないときnull）
        private Win32Caret m_caret = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]keyIntegrator キー入力の統合先
        // 　　　　[in]commandTarget コマンドの送信先
        // 　　　　[in]console       コンソール画面
        // 　　　　[in]userServer    このターミナルの接続先user@server
        // 　　　　[in]statusBar     ステータスバー
        // 　　　　[in]supportInput  入力をサポートするときtrue
        // 戻り値：なし
        //=========================================================================================
        public TerminalView(IKeyEventIntegrator keyIntegrator, IUICommandTarget commandTarget, ConsoleScreen console, string userServer, TerminalStatusBar statusBar, bool supportInput) {
            InitializeComponent();
            m_keyIntegrator = keyIntegrator;
            m_commandTarget = commandTarget;
            m_logViewImpl = new LogViewImpl(this, LogViewImpl.LogMode.TerminalWindow, UM_CHECK_MOUSE_DRAG_CONTINUE, true);

            m_statusBar = statusBar;
            m_consoleScreen = console;
            m_consoleScreen.Event.AddEventHandler(this);
            m_logViewImpl.Initialize(console.LogBuffer);
            if (supportInput) {
                string fontName = Configuration.Current.LogWindowFixedFontName;
                m_caret = new Win32Caret(this, ObjectUtils.SizeFToSize(m_logViewImpl.FontSize), true, fontName);
                m_logViewImpl.Caret = m_caret;
            }

            // ダブルバッファリング有効
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        //=========================================================================================
        // 機　能：反対のビューを設定する（入力ビューに対するバックログビュー）
        // 引　数：[in]opposite   反対のビュー（反対のビューがないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void InitializeOppositeView(TerminalView opposite) {
            m_oppositeView = opposite;
        }

        //=========================================================================================
        // 機　能：ビューが閉じられるときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseView() {
            m_consoleScreen.Event.DeleteEventHandler(this);
        }

        //=========================================================================================
        // 機　能：フォーカスを得た場合または失った場合の処理を行う
        // 引　数：[in]gotFocus  フォーカスを得たときtrue、失ったときfalse
        // 戻り値：なし
        //=========================================================================================
        public void OnFocusChange(bool gotFocus) {
            if (m_caret != null) {
                if (gotFocus) {
                    m_caret.OnGotFocus();
                } else {
                    m_caret.OnLostFocus();
                }
            }
        }

        //=========================================================================================
        // 機　能：ウィンドウプロシージャ
        // 引　数：[in]message  ウィンドウメッセージ
        // 戻り値：なし
        //=========================================================================================
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message message) {
            switch (message.Msg) {
                case Win32API.WM_VSCROLL:
                    m_logViewImpl.OnVScroll(message);
                    return;
                case UM_CHECK_MOUSE_DRAG_CONTINUE:
                    m_logViewImpl.OnDragContinue();
                    return;
            }
            base.WndProc(ref message);
        }
        
        //=========================================================================================
        // 機　能：マウスの選択が開始されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseSelectStart() {
            if (m_oppositeView != null) {
                m_oppositeView.ClearSelection();
            }
        }

        //=========================================================================================
        // 機　能：マウスの選択が変更されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseSelectionMove() {
            SetCaretPosition();
        }

        //=========================================================================================
        // 機　能：マウスの選択が終了したときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseSelectEnd() {
            if (m_oppositeView != null) {
                m_oppositeView.Invalidate();
            }
        }

        //=========================================================================================
        // 機　能：マウスホイールが操作されたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void LogPanel_MouseWheel(object sender, MouseEventArgs evt) {
            // 親からのメッセージを中継する
            m_logViewImpl.LogPanel_MouseWheel(sender, evt);
        }

        //=========================================================================================
        // 機　能：マウスの選択が変更されたときの処理を行う
        // 引　数：[in]type    通知の種類
        // 　　　　[in]param   通知に対するパラメータ
        // 戻り値：なし
        //=========================================================================================
        public void OnStatusChanged(LogViewContainerStatusType type, object param) {
            m_statusBar.StatusSelectionChanged(type, param);
        }

        //=========================================================================================
        // 機　能：選択範囲の情報を補正する
        // 引　数：なし
        // 戻り値：新しい選択範囲（選択がないときnull）
        //=========================================================================================
        public LogViewSelectionRange ModifySelection() {
            if (m_oppositeView.m_logViewImpl.SelectionRange != null) {
                return m_oppositeView.m_logViewImpl.SelectionRange;
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：接続処理が完了したときの処理を行う
        // 引　数：[in]sender    　 対象の画面
        // 　　　　[in]status       ステータス
        // 　　　　[in]errorDetail  詳細エラー情報
        // 戻り値：なし
        //=========================================================================================
        public void OnConnect(ConsoleScreen sender, FileOperationStatus status, string errorDetail) {
        }

        //=========================================================================================
        // 機　能：データが追加されたときの処理を行う
        // 引　数：[in]sender      対象の画面
        // 　　　　[in]changeLog   変化内容
        // 戻り値：なし
        //=========================================================================================
        public void OnAddData(ConsoleScreen sender, LogLineChangeLog changeLog) {
            // 変更を描画
            CaretRedraw caretRedraw = new CaretRedraw(m_caret);
            try {
                List<LogLineTerminal> updated = changeLog.UpdatedList;
                List<LogLineTerminal> added = changeLog.AddedList;
                bool refreshAll = changeLog.RefreshAll;
                if (updated.Count + added.Count > m_consoleScreen.CharSize.Height / 3) {
                    refreshAll = true;
                }
                if (!refreshAll) {
                    for (int i = 0; i < updated.Count; i++) {
                        m_logViewImpl.RedrawLogLine(updated[i]);
                    }
                } else {
                    this.Invalidate();
                }
                bool firstDrawDone = false;
                for (int i = 0; i < added.Count; i++) {
                    firstDrawDone |= m_logViewImpl.OnRegistLogLine(added[i], refreshAll);
                }

                if (firstDrawDone) {
                    CaretRedraw redraw = new CaretRedraw(m_caret);
                    try {
                        Graphics g = this.CreateGraphics();
                        Brush brush = new HatchBrush(HatchStyle.Percent25, Color.White, Color.Transparent);
                        g.FillRectangle(brush, this.ClientRectangle);
                        brush.Dispose();
                        g.Dispose();
                    } finally {
                        redraw.Resume();
                    }
                }

                // カレットを移動
                SetCaretPosition();
            } finally {
                caretRedraw.Resume();
            }
        }
        
        //=========================================================================================
        // 機　能：SSHの接続が閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnSSHClose() {
            Invalidate();
        }
        
        //=========================================================================================
        // 機　能：カレットを現在の表示位置に合わせて設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetCaretPosition() {
            if (m_caret != null) {
                Point cursor = m_consoleScreen.GetCaretPosition(m_logViewImpl.TopLineId);
                LogGraphics g = new LogGraphics(this, LogViewImpl.LogMode.TerminalWindow);
                int caretX;
                try {
                    // 文字の位置を計測
                    string str = StringUtils.Repeat("M", cursor.X);
                    int[] range = new int[] { str.Length };
                    float[] pos = TextRendererUtils.MeasureStringRegion(g.Graphics, g.LogWindowFixedFont, str, range);
                    caretX = (int)pos[0];
                } finally {
                    g.Dispose();
                }
                cursor.Y = Math.Max(-3, Math.Min(10000, cursor.Y));
                SizeF fontSize = m_logViewImpl.FontSize;
                Point cursorPos = new Point(caretX, (int)(cursor.Y * fontSize.Height));
                m_caret.CaretPosition = cursorPos;
            }
        }

        //=========================================================================================
        // 機　能：スクロールを実行する
        // 引　数：[in]dy  スクロール量
        // 戻り値：なし
        //=========================================================================================
        public void ScrollLine(int dy) {
            m_logViewImpl.ScrollLine(dy);
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
        private void TerminalView_Paint(object sender, PaintEventArgs evt) {
            CaretRedraw caretRedraw = new CaretRedraw(m_caret);
            try {
                m_logViewImpl.OnPaint(evt);
            } finally {
                caretRedraw.Resume();
            }
        }

        //=========================================================================================
        // 機　能：ビジュアルベルを表示する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ShowVisualBell(bool revert) {
            CaretRedraw caretRedraw = new CaretRedraw(m_caret);
            try {
                Graphics grp = this.CreateGraphics();
                try {
                    LogGraphics.BackColorMode backMode;
                    if (revert) {
                        backMode = LogGraphics.BackColorMode.VisualBell;
                    } else if (m_consoleScreen.CurrentState.ChannelClosed) {
                        backMode = LogGraphics.BackColorMode.Closed;
                    } else {
                        backMode = LogGraphics.BackColorMode.Normal;
                    }

                    LogGraphics g = new LogGraphics(grp, LogViewImpl.LogMode.TerminalWindow, backMode);
                    try {
                        m_logViewImpl.OnPaintDirect(g, -1, -1);
                    } finally {
                        g.Dispose();
                    }
                } finally {
                    grp.Dispose();
                }
            } finally {
                caretRedraw.Resume();
            }
        }

        //=========================================================================================
        // 機　能：選択を解除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearSelection() {
            m_logViewImpl.ClearSelection();
        }

        //=========================================================================================
        // 機　能：選択された範囲のテキストを返す
        // 引　数：[in]getAll     全範囲を取得するときtrue、選択範囲を取得するときfalse
        // 　　　　[in]crlf       改行コード
        // 　　　　[out]text      選択された範囲のテキストを返す変数（選択中ではないときnull）
        // 　　　　[out]createAll すべて選択できたときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        public void GetSelectedText(bool getAll, string crlf, out string text, out bool createAll) {
            if (getAll) {
                m_logViewImpl.GetAllText(crlf, out text);
                createAll = true;
            } else {
                m_logViewImpl.GetSelectedText(crlf, out text, out createAll);
            }
        }

        //=========================================================================================
        // 機　能：テキストをすべて選択する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SelectAllText() {
            m_logViewImpl.SelectAllText();
        }

        //=========================================================================================
        // 機　能：バックログをクリアする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearBackLog() {
            m_logViewImpl.ClearLog();
        }

        //=========================================================================================
        // 機　能：Windowsからのキー入力を受け取ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TerminalView_KeyDown(object sender, KeyEventArgs evt) {
            bool handled = m_keyIntegrator.OnKeyDown(new KeyCommand(evt));
            if (handled) {
                evt.Handled = true;
                evt.SuppressKeyPress = true;
                this.Focus();
            }
        }

        //=========================================================================================
        // 機　能：Windowsからのキー入力を受け取ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TerminalView_KeyPress(object sender, KeyPressEventArgs evt) {
            m_keyIntegrator.OnKeyChar(evt.KeyChar);
        }

        //=========================================================================================
        // 機　能：Windowsからのキー入力を受け取ったときの処理を行う
        // 引　数：[in]key  キーコマンド
        // 戻り値：なし
        //=========================================================================================
        private void TerminalView_KeyUp(object sender, KeyEventArgs evt) {
            m_keyIntegrator.OnKeyUp(new KeyCommand(evt));
        }

        //=========================================================================================
        // 機　能：Windowsからのキー入力を受け取ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TerminalView_MouseClick(object sender, MouseEventArgs evt) {
            if ((evt.Button & MouseButtons.Right) != MouseButtons.Right) {
                return;
            }
            // 右クリック
            // コンテキストメニューを表示
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.TerminalMenu, m_commandTarget);
            menuImpl.AddItemsFromSetting(cms, cms.Items, Program.Document.MenuSetting.TerminalContext, Program.Document.KeySetting.TerminalKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            Point ptMouse = this.PointToClient(Cursor.Position);
            this.ContextMenuStrip = cms;
            this.ContextMenuStrip.Show(this, ptMouse);
            this.ContextMenuStrip = null;
        }

        //=========================================================================================
        // プロパティ：親となるパネルのWindowsコンポーネント
        //=========================================================================================
        public ScrollableControl View {
            get {
                return this;
            }
        }

        //=========================================================================================
        // プロパティ：ログ1行文の高さ
        //=========================================================================================
        public int LogLineHeight {
            get {
                return m_logViewImpl.LogLineHeight;
            }
        }

        //=========================================================================================
        // プロパティ：画面表示中の先頭行の行ID
        //=========================================================================================
        public long TopLineId {
            get {
                return m_logViewImpl.TopLineId;
            }
        }

        //=========================================================================================
        // プロパティ：画面の大きさ（半角単位）
        //=========================================================================================
        public Size ScreenCharSize {
            get {
                Rectangle rcClient = this.ClientRectangle;
                SizeF fontSize = m_logViewImpl.FontSize;
                Size size = new Size((int)(rcClient.Width / fontSize.Width), (int)(rcClient.Height / fontSize.Height));
                return size;
            }
        }
        
        //=========================================================================================
        // プロパティ：選択範囲をドラッグ中のときtrue
        //=========================================================================================
        public bool IsMouseDrag {
            get {
                return m_logViewImpl.IsMouseDrag;
            }
        }

        //=========================================================================================
        // プロパティ：選択中の範囲（選択がないときnull）
        //=========================================================================================
        public LogViewSelectionRange SelectionRange {
            get {
                return m_logViewImpl.SelectionRange;
            }
            set {
                m_logViewImpl.SelectionRange = value;
            }
        }

        //=========================================================================================
        // プロパティ：背景色の表示モード
        //=========================================================================================
        public LogGraphics.BackColorMode BackColorMode {
            get {
                if (m_consoleScreen.CurrentState.ChannelClosed) {
                    return LogGraphics.BackColorMode.Closed;
                } else {
                    return LogGraphics.BackColorMode.Normal;
                }
            }
        }

        private void TerminalView_Resize(object sender, EventArgs evt) {
            SetCaretPosition();
        }
    }
}
