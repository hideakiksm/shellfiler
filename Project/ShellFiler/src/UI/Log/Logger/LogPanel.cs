using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.UI.ControlBar;
using ShellFiler.Util;

namespace ShellFiler.UI.Log.Logger {

    //=========================================================================================
    // クラス：ログウィンドウのUI
    //=========================================================================================
    public partial class LogPanel : UserControl, ILogViewContainer {
        // WM_PAINT受信時にマウスのドラッグを継続する
        public const int UM_CHECK_MOUSE_DRAG_CONTINUE = Win32API.WM_USER + 1;

        // 混合表示用ログ出力バッファ
        private LogBuffer m_mixedLogBuffer;

        // ファイル操作用ログ出力バッファ
        private LogBuffer m_fileLogBuffer;
        
        // ログパネルの実装
        private LogViewImpl m_logViewImpl;

        // 操作種別文字列の最大表示幅
        private static int s_cxOperationString;

        // ステータス文字列の最大表示幅
        private static int s_cxStatusString;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public LogPanel() {
            InitializeComponent();

            // ダブルバッファリング有効
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
            m_mixedLogBuffer = new LogBuffer(Configuration.Current.LogLineMaxCount);
            m_fileLogBuffer = new LogBuffer(Configuration.Current.LogLineMaxCount);
            m_logViewImpl = new LogViewImpl(this, LogViewImpl.LogMode.LogWindow, UM_CHECK_MOUSE_DRAG_CONTINUE, false);
            m_logViewImpl.Initialize(m_mixedLogBuffer);

            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(Program.MainWindow.MainWindowForm_PreviewKeyDown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(Program.MainWindow.MainWindowForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(Program.MainWindow.MainWindowForm_KeyUp);

            float logFontSize = Configuration.Current.LogWindowFontSize;
            s_cxOperationString = (int) MainWindowForm.Xf(110.0f * logFontSize / 9.0f);
            s_cxStatusString = (int)MainWindowForm.Xf(149 * logFontSize / 9.0f);

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
        // 機　能：ログを記録する
        // 引　数：[in]message  登録するログ文字列（改行に対応）
        // 戻り値：なし
        // メ　モ：呼び出し先RegistLogLineでdelegateによりUIスレッドで実行する
        //=========================================================================================
        public void RegistLogLineHelper(string message) {
            string[] strLineList = message.Split('\n');
            foreach (string strLine in strLineList) {
                RegistLogLine(new LogLineSimple("{0}", strLine));
            }
        }

        //=========================================================================================
        // 機　能：ログを記録する
        // 引　数：[in]logLine  登録するログ情報
        // 戻り値：なし
        // メ　モ：delegateによりUIスレッドで実行する
        //=========================================================================================
        public void RegistLogLine(LogLine logLine) {
            BaseThread.InvokeProcedureByMainThread(new RegistLogLineDelegate(RegistLogLineUI), logLine);
        }
        delegate void RegistLogLineDelegate(LogLine logLine);
        private void RegistLogLineUI(LogLine logLine) {
            try {
                m_fileLogBuffer.AddLogLine(logLine);
                m_mixedLogBuffer.AddLogLine(logLine);
                m_logViewImpl.OnRegistLogLine(logLine, false);
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionUI, "RegistLogLineUI");
            }
        }

        //=========================================================================================
        // 機　能：ログの内容をすべて削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearLog() {
            m_mixedLogBuffer.ClearAll();
            m_fileLogBuffer.ClearAll();
            m_logViewImpl.ClearLog();
        }
        
        //=========================================================================================
        // 機　能：マウスの選択が開始されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseSelectStart() {
        }

        //=========================================================================================
        // 機　能：マウスの選択が変更されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseSelectionMove() {
        }

        //=========================================================================================
        // 機　能：マウスの選択が終了したときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseSelectEnd() {
        }
        
        //=========================================================================================
        // 機　能：マウスの選択が変更されたときの処理を行う
        // 引　数：[in]type    通知の種類
        // 　　　　[in]param   通知に対するパラメータ
        // 戻り値：なし
        //=========================================================================================
        public void OnStatusChanged(LogViewContainerStatusType type, object param) {
        }

        //=========================================================================================
        // 機　能：再描画イベントが発行されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogPanel_Paint(object sender, PaintEventArgs evt) {
            m_logViewImpl.OnPaint(evt);
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogPanel_MouseUp(object sender, MouseEventArgs evt) {
            if ((evt.Button & MouseButtons.Right) != MouseButtons.Right) {
                return;
            }
            // 右クリック
            // コンテキストメニューを表示
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.MainMenubar, Program.MainWindow);
            menuImpl.AddItemsFromSetting(cms, cms.Items, Program.Document.MenuSetting.LogMenu, Program.Document.KeySetting.FileListKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            Point ptMouse = this.PointToClient(Cursor.Position);
            this.ContextMenuStrip = cms;
            this.ContextMenuStrip.Show(this, ptMouse);
            this.ContextMenuStrip = null;
        }

        //=========================================================================================
        // 機　能：選択範囲の情報を補正する
        // 引　数：なし
        // 戻り値：新しい選択範囲（選択がないときnull）
        //=========================================================================================
        public LogViewSelectionRange ModifySelection() {
            return null;
        }

        //=========================================================================================
        // 機　能：ログ内容を再描画する
        // 引　数：[in]logLine  登録するログ情報
        // 戻り値：なし
        // メ　モ：delegateによりUIスレッドで実行する
        //=========================================================================================
        public void RedrawLogLine(LogLine logLine) {
            BaseThread.InvokeProcedureByMainThread(new RedrawLogLineDelegate(RedrawLogLineUI), logLine);
        }
        delegate void RedrawLogLineDelegate(LogLine logLine);
        private void RedrawLogLineUI(LogLine logLine) {
            try {
                m_logViewImpl.RedrawLogLine(logLine);
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionUI, "RedrawLogLineUI");
            }
        }

        //=========================================================================================
        // 機　能：最新のログ行を表示する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ShowLatestLogLine() {
            m_logViewImpl.ScrollLine(int.MaxValue / 2);
        }

        //=========================================================================================
        // 機　能：テキストをすべて選択する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SelectAllLogLine() {
            m_logViewImpl.SelectAllText();
            this.Invalidate();
        }

        //=========================================================================================
        // 機　能：選択された範囲のテキストを返す
        // 引　数：[out]text      選択された範囲のテキストを返す変数（選択中ではないときnull）
        // 　　　　[out]createAll すべて選択できたときtrueを返す変数
        // 戻り値：テキストを返したときtrue、選択されていないときfalse
        //=========================================================================================
        public bool GetSelectedText(out string text, out bool createAll) {
            if (m_logViewImpl.SelectionRange != null) {
                m_logViewImpl.GetSelectedText("\r\n", out text, out createAll);
                return true;
            } else {
                text = null;
                createAll = false;
                return false;
            }
        }

        //=========================================================================================
        // 機　能：選択範囲を点滅させる
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void FlashSelection() {
            if (m_logViewImpl.SelectionRange == null) {
                return;
            }
            LogViewSelectionRange selection = m_logViewImpl.SelectionRange;
            m_logViewImpl.SelectionRange = null;
            this.Invalidate();
            this.Update();
            Thread.Sleep(100);
            m_logViewImpl.SelectionRange = selection;
            this.Invalidate();
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
        // プロパティ：操作種別文字列の最大表示幅
        //=========================================================================================
        public static int CxOperationString {
            get {
                return s_cxOperationString;
            }
        }

        //=========================================================================================
        // プロパティ：ステータス文字列の最大表示幅
        //=========================================================================================
        public static int CxStatusString {
            get {
                return s_cxStatusString;
            }
        }

        private void debugTest() {
            /*
            long logId = (new LogLineSimple("")).LogId + 1;
            LogBuffer buf1 = new LogBuffer(3);
            buf1.AddLogLine(new LogLineSimple("a"));
            Debug.Assert(buf1.AvailableLogCount == 1);
            Debug.Assert(buf1.FirstAvailableId == 0);
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(0))).Message == "a");
            Debug.Assert(buf1.LogIdToLineId(logId + 0) == 0);

            buf1.AddLogLine(new LogLineSimple("b"));
            Debug.Assert(buf1.AvailableLogCount == 2);
            Debug.Assert(buf1.FirstAvailableId == 0);
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(0))).Message == "a");
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(1))).Message == "b");
            Debug.Assert(buf1.LogIdToLineId(logId + 0) == 0);
            Debug.Assert(buf1.LogIdToLineId(logId + 1) == 1);

            buf1.AddLogLine(new LogLineSimple("c"));
            Debug.Assert(buf1.AvailableLogCount == 3);
            Debug.Assert(buf1.FirstAvailableId == 0);
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(0))).Message == "a");
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(1))).Message == "b");
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(2))).Message == "c");
            Debug.Assert(buf1.LogIdToLineId(logId + 0) == 0);
            Debug.Assert(buf1.LogIdToLineId(logId + 1) == 1);
            Debug.Assert(buf1.LogIdToLineId(logId + 2) == 2);

            buf1.AddLogLine(new LogLineSimple("d"));
            Debug.Assert(buf1.AvailableLogCount == 3);
            Debug.Assert(buf1.FirstAvailableId == 1);
            Debug.Assert(buf1.LineIdToLogLine(0) == null);
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(1))).Message == "b");
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(2))).Message == "c");
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(3))).Message == "d");
            Debug.Assert(buf1.LogIdToLineId(logId + 0) == -1);
            Debug.Assert(buf1.LogIdToLineId(logId + 1) == 1);
            Debug.Assert(buf1.LogIdToLineId(logId + 2) == 2);
            Debug.Assert(buf1.LogIdToLineId(logId + 3) == 3);

            buf1.AddLogLine(new LogLineSimple("e"));
            Debug.Assert(buf1.AvailableLogCount == 3);
            Debug.Assert(buf1.FirstAvailableId == 2);
            Debug.Assert(buf1.LineIdToLogLine(0) == null);
            Debug.Assert(buf1.LineIdToLogLine(1) == null);
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(2))).Message == "c");
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(3))).Message == "d");
            Debug.Assert(((LogLineSimple)(buf1.LineIdToLogLine(4))).Message == "e");
            Debug.Assert(buf1.LogIdToLineId(logId + 0) == -1);
            Debug.Assert(buf1.LogIdToLineId(logId + 1) == -1);
            Debug.Assert(buf1.LogIdToLineId(logId + 2) == 2);
            Debug.Assert(buf1.LogIdToLineId(logId + 3) == 3);
            Debug.Assert(buf1.LogIdToLineId(logId + 4) == 4);
            */
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
        // プロパティ：背景色の表示モード
        //=========================================================================================
        public LogGraphics.BackColorMode BackColorMode {
            get {
                return LogGraphics.BackColorMode.Normal;
            }
        }
    }
}
