using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.Util;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.DataObject;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.FileViewer;
using ShellFiler.UI.ControlBar;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：テキストビューア／ダンプビューアの親となるパネル
    //=========================================================================================
    public partial class TextFileViewer : Panel, IUICommandTarget, ITwoStrokeKeyForm {
        // WM_PAINT受信時にマウスのドラッグを継続する
        public const int UM_CHECK_MOUSE_DRAG_CONTINUE = Win32API.WM_USER + 1;

        // デフォルトのタブ幅
        private const int DEFAULT_TAB_WIDTH = 8;


        // テキストビューアのフォーム
        private FileViewerForm m_form;

        // ステータスバー
        private FileViewerStatusBar m_statusBar;

        // レーダーバー
        private RadarBar m_radarBar;

        // 現在のビューアの実装（初期化されていないときnull）
        private IViewerComponent m_currentViewerComponent = null;

        // テキストビューアの実装
        private TextViewerComponent m_textViewerComponent;

        // ダンプビューアの実装
        private DumpViewerComponent m_dumpViewerComponent;

        // テキスト検索ダイアログ（開いていないときnull）
        private ViewerTextSearchDialog m_textSearchDialog = null;

        // ダンプ検索ダイアログ（開いていないときnull）
        private ViewerDumpSearchDialog m_dumpSearchDialog = null;

        // テキストの行管理情報
        private TextBufferLineInfoList m_lineInfo;

        // 初回のpaint実行時のときtrue
        private bool m_firstPaint = true;

        // ビジュアルベルを表示するときtrue
        private bool m_requireVisualBell = false;

        // 半角１文字分の大きさの期待値
        private SizeF m_fontSize;
        
        // マウスをドラッグ中のときtrue
        private bool m_mouseDrag = false;

        // テキスト検索のエンジン
        private TextSearchEngine m_searchEngine;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]form      テキストビューアのフォーム
        // 　　　　[in]statusBar ステータスバー
        // 　　　　[in]radarBar  レーダーバー
        // 戻り値：なし
        //=========================================================================================
        public TextFileViewer(FileViewerForm form, FileViewerStatusBar statusBar, RadarBar radarBar) {
            InitializeComponent();
            this.visualBellTimer.Stop();
            m_form = form;
            m_form.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.TextFileViewer_MouseWheel);
            m_statusBar = statusBar;
            m_radarBar = radarBar;

            // 無効状態のスクロールバーを設定
            this.VerticalScroll.Visible = true;
            this.VerticalScroll.Enabled = false;

            // フォントサイズを取得
            TextViewerGraphics g = new TextViewerGraphics(this, 0);
            try {
                m_fontSize = new SizeF(GraphicsUtils.MeasureString(g.Graphics, g.TextFont, "MMMMMMMMMM") / 10.0f, g.TextFont.Height);
            } finally {
                g.Dispose();
            }

            // ダブルバッファリング有効
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // その他コンポーネントを初期化
            TextViewerLineBreakSetting lineBreak;
            if (Configuration.Current.TextViewerLineBreakDefault == null) {
                lineBreak = (TextViewerLineBreakSetting)(Program.Document.UserGeneralSetting.TextViewerLineBreak).Clone();
            } else {
                lineBreak = (TextViewerLineBreakSetting)(Configuration.Current.TextViewerLineBreakDefault).Clone();
            }
            int tabWidth;
            if (m_form.CurrentFile.DefaultTab == -1) {
                tabWidth = GetTabWidth(m_form.CurrentFile.FilePath);
            } else {
                tabWidth = m_form.CurrentFile.DefaultTab;
            }
            float screenWidth = Screen.FromControl(this.FileViewerForm).Bounds.Width;
            m_lineInfo = new TextBufferLineInfoList(m_form.CurrentFile, this, m_fontSize, screenWidth, lineBreak, tabWidth, Configuration.Current.TextViewerIsDisplayLineNumber, Configuration.Current.TextViewerMaxLineCount);
            m_searchEngine = new TextSearchEngine(this, m_statusBar, m_lineInfo);
            m_textViewerComponent = new TextViewerComponent(this, m_fontSize);
            m_dumpViewerComponent = new DumpViewerComponent(this, m_fontSize);
        }

        //=========================================================================================
        // 機　能：タブ幅を決定する
        // 引　数：[in]fileName  ファイル名
        // 戻り値：タブ幅
        //=========================================================================================
        private int GetTabWidth(string fileName) {
            string ext = GenericFileStringUtils.GetExtensionLast(fileName).ToLower();
            string[] extTab4List = Configuration.Current.TextViewerTab4Extension.Split(' ', ',', ';', ':');
            foreach (string extTab4 in extTab4List) {
                string extTab4Low = extTab4.ToLower();
                if (ext == extTab4Low) {
                    return 4;
                }
            }
            return DEFAULT_TAB_WIDTH;
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
            m_searchEngine.Dispose();       // 非nullのため、破棄後にnull設定しない
            this.visualBellTimer.Stop();
            this.visualBellTimer.Dispose();
        }

        //=========================================================================================
        // 機　能：ウィンドウプロシージャ
        // 引　数：[in]message  ウィンドウメッセージ
        // 戻り値：なし
        //=========================================================================================
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
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
            }
            base.WndProc(ref message);
        }

        //=========================================================================================
        // 機　能：垂直スクロールイベントを処理する
        // 引　数：[in]message  ウィンドウメッセージ
        // 戻り値：なし
        //=========================================================================================
        private void OnVScroll(Message message) {
            if (m_currentViewerComponent == null) {
                return;
            }
            Win32API.SCROLLINFO scInfo = new Win32API.SCROLLINFO();
            scInfo.fMask = (int)(Win32API.SIF_TRACKPOS);
            Win32API.Win32GetScrollInfo(this.Handle, Win32API.SB_VERT, ref scInfo);
            int newValue  = scInfo.nTrackPos;

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
            m_currentViewerComponent.OnVScroll(evt);
        }
        
        //=========================================================================================
        // 機　能：水平スクロールイベントを処理する
        // 引　数：[in]message  ウィンドウメッセージ
        // 戻り値：なし
        //=========================================================================================
        private void OnHScroll(Message message) {
            if (m_currentViewerComponent == null) {
                return;
            }
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
            m_currentViewerComponent.OnHScroll(evt);
        }

        //=========================================================================================
        // 機　能：ファイルの状態が変わったの処理を行う
        // 引　数：[in]chunkLoaded    新しいチャンクがロードされた
        // 　　　　[in]statusChanged  ステータスが変わった
        // 戻り値：なし
        //=========================================================================================
        public void OnFileStatusChanged(bool chunkLoaded, bool statusChanged) {
            bool invalidate = false;

            float oldWidth = m_lineInfo.MaxTextPixelWidth;
            int minLine = m_lineInfo.LogicalLineCount;
            int minByte = m_lineInfo.NextParseStartPosition;
            byte[] readBuffer;
            int maxByte;
            m_lineInfo.TargetFile.GetBuffer(out readBuffer, out maxByte);

            // 新しいチャンク部分を解析
            bool initialized = m_lineInfo.ParseChunk();
            if (m_currentViewerComponent == null && initialized) {
                if (m_lineInfo.IsBinary) {
                    m_currentViewerComponent = m_dumpViewerComponent;
                } else {
                    m_currentViewerComponent = m_textViewerComponent;
                }
            }

            // ステータス変化の場合
            if (statusChanged) {
                if (m_lineInfo.TargetFile.Status == RetrieveDataLoadStatus.Failed) {
                    this.BackColor = Configuration.Current.TextViewerErrorBackColor;
                    ShowStatusbarMessage(m_lineInfo.TargetFile.ErrorInfo, FileOperationStatus.LogLevel.Error, IconImageListID.None);
                } else if (m_lineInfo.TargetFile.Status == RetrieveDataLoadStatus.CompletedPart) {
                    ShowStatusbarMessage(Resources.FileViewer_ReadAPart, FileOperationStatus.LogLevel.Info, IconImageListID.None);
                }
                invalidate = true;
            }

            // 表示領域の変化を検出
            int maxLine = m_lineInfo.LogicalLineCount;
            if (m_currentViewerComponent != null && m_currentViewerComponent.IsDisplay(minLine, maxLine, minByte, maxByte)) {
                invalidate = true;
            }
            if (oldWidth != m_lineInfo.MaxTextPixelWidth) {
                invalidate = true;
            }

            // UIを更新
            if (invalidate) {
                Invalidate();
            }
            UpdateInformation();
            m_statusBar.RefreshLineNo();
            if (statusChanged) {
                m_statusBar.RefreshStatusBar();
            }
            if (initialized) {
                m_statusBar.RefreshTextInfo();
            }
        }

        //=========================================================================================
        // 機　能：ビューアのモードを切り替える
        // 引　数：[in]textMode  テキストモードにするときtrue、ダンプモードにするときfalse
        // 戻り値：なし
        //=========================================================================================
        public void ChangeViewMode(bool textMode) {
            // 検索ダイアログを閉じる
            if (!textMode && m_textSearchDialog != null) {
                m_textSearchDialog.Close();
                m_textSearchDialog = null;
            }
            if (textMode && m_dumpSearchDialog != null) {
                m_dumpSearchDialog.Close();
                m_dumpSearchDialog = null;
            }

            // モードを切り替える
            if (textMode && m_currentViewerComponent == m_dumpViewerComponent) {
                // ダンプビューア→テキストビューア
                m_currentViewerComponent = m_textViewerComponent;
                int address = m_dumpViewerComponent.Address;
                int topLine = m_lineInfo.GetLineNumberFromAddress(address);
                int topPhysicalLine = m_lineInfo.GetLineInfo(topLine).PhysicalLineNo;
                m_textViewerComponent.MoveSpecifiedPhysicalLineNoOnTop(topPhysicalLine);
            } else if (!textMode && m_currentViewerComponent == m_textViewerComponent) {
                // テキストビューア→ダンプビューア
                m_currentViewerComponent = m_dumpViewerComponent;
                int line = m_textViewerComponent.VisibleTopLine - 1;
                int address = m_lineInfo.GetLineInfo(line).BufferIndex;
                m_dumpViewerComponent.MoveSpecifiedAddressOnTop(address);
            } else {
                return;
            }

            // モード変更時の処理
            m_currentViewerComponent.OnSizeChange(ClientRectangle);
            m_currentViewerComponent.CancelSelect();
            SearchEngine.ClearSearchResult();
            m_radarBar.NotifySearchEnd();
            m_statusBar.SetSearchHitCount(0, 0);
            Invalidate();
        }

        //=========================================================================================
        // 機　能：ステータスバーにメッセージを表示する
        // 引　数：[in]message  メッセージ
        // 　　　　[in]level    メッセージのレベル
        // 　　　　[in]icon     使用するアイコン
        // 戻り値：なし
        //=========================================================================================
        public void ShowStatusbarMessage(string message, FileOperationStatus.LogLevel level, IconImageListID icon) {
            ReuqestVisualBell();
            m_statusBar.ShowErrorMessage(message, level, icon);
        }

        //=========================================================================================
        // 機　能：UIを更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void UpdateInformation() {
            BaseThread.InvokeProcedureByMainThread(new UpdateInformationDelegate(UpdateInformationUI));
        }
        private delegate void UpdateInformationDelegate();
        private void UpdateInformationUI() {
            if (m_currentViewerComponent != null) {
                m_currentViewerComponent.OnSizeChange(this.ClientRectangle);
            }
        }

        //=========================================================================================
        // 機　能：描画イベント受信時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TextFileViewer_Paint(object sender, PaintEventArgs evt) {
            if (m_currentViewerComponent == null) {
                return;
            }

            // 初回の描画時にサイズ変更イベントを発生させる
            if (m_firstPaint) {
                m_firstPaint = false;
                m_currentViewerComponent.OnSizeChange(this.ClientRectangle);
            }

            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, ClientRectangle.Width, ClientRectangle.Height);
            TextViewerGraphics g = new TextViewerGraphics(doubleBuffer.DrawingGraphics, ObjectUtils.Ceil(m_currentViewerComponent.LineNoAreaWidth));
            try {
                Brush backBrush = new SolidBrush(this.BackColor);
                g.Graphics.FillRectangle(backBrush, ClientRectangle);
                backBrush.Dispose();
                m_currentViewerComponent.OnPaint(g, false, -1, -1);
            } finally {
                g.Dispose();
            }
            doubleBuffer.FlushScreen(0, 0);

            // マウスのドラッグが継続状態かどうかを確認
            if (m_mouseDrag) {
                Win32API.Win32PostMessage(Handle, UM_CHECK_MOUSE_DRAG_CONTINUE, 0, 0);
            }
        }

        //=========================================================================================
        // 機　能：マウスホイールイベントを処理する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TextFileViewer_MouseWheel(object sender, MouseEventArgs evt) {
            if (m_currentViewerComponent == null) {
                return;
            }
            m_currentViewerComponent.OnMouseWheel(evt);
        }

        //=========================================================================================
        // 機　能：ウィンドウサイズ変更時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TextFileViewer_Resize(object sender, EventArgs evt) {
            if (m_currentViewerComponent != null) {
                m_currentViewerComponent.OnSizeChange(ClientRectangle);
            }
            this.Invalidate();
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]evt   送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnKeyDown(KeyCommand evt) {
            if (m_currentViewerComponent == null) {
                return;
            }

            // キーに対応するコマンドを取得して実行
            Keys key = evt.KeyCode;
            FileViewerActionCommand command = Program.Document.CommandFactory.CreateFileViewerCommandFromKeyInput(evt, this);
            if (command != null) {
                command.Execute();
            }
        }

        //=========================================================================================
        // 機　能：UIでのコマンドが発生したことを通知する
        // 引　数：[in]sender  イベント発生原因の送信元の種別
        // 　　　　[in]item    発生したイベントの項目
        // 戻り値：なし
        //=========================================================================================
        public void OnUICommand(UICommandSender sender, UICommandItem item) {
            FileViewerActionCommand command = Program.Document.CommandFactory.CreateFileViewerCommandFromUICommand(item, this);
            if (command != null) {
                command.Execute();
            }
        }

        //=========================================================================================
        // 機　能：ビジュアルベルをリクエストする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ReuqestVisualBell() {
            BaseThread.InvokeProcedureByMainThread(new RequestVisualBellDelegate(RequestVisualBellUI));
        }
        private delegate void RequestVisualBellDelegate();
        private void RequestVisualBellUI() {
            this.visualBellTimer.Start();
            this.visualBellTimer.Enabled = true;
        }

        //=========================================================================================
        // 機　能：ビジュアルベルのタイマーの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void visualBellTimer_Tick(object sender, EventArgs evt) {
            m_requireVisualBell = true;
            this.visualBellTimer.Stop();

            // ビジュアルベル
            if (m_requireVisualBell) {
                m_requireVisualBell = false;
                ShowVisualBell();
            }
        }

        //=========================================================================================
        // 機　能：ビジュアルベルを表示する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ShowVisualBell() {
            if (m_currentViewerComponent == null) {
                return;
            }
            TextViewerGraphics gPanel = new TextViewerGraphics(this, m_currentViewerComponent.LineNoAreaWidth);
            try {
                Bitmap bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                try {
                    // 画面全体のコピーを作成
                    Graphics gBmp = Graphics.FromImage(bmp);
                    try {
                        TextViewerGraphics gBmp2 = new TextViewerGraphics(gBmp, m_currentViewerComponent.LineNoAreaWidth);
                        try {
                            Brush backBrush = new SolidBrush(this.BackColor);
                            try {
                                gBmp2.Graphics.FillRectangle(backBrush, ClientRectangle);
                            } finally {
                                backBrush.Dispose();
                            }
                            if (m_currentViewerComponent != null) {
                                m_currentViewerComponent.OnPaint(gBmp2, false, -1, -1);
                            }
                        } finally {
                            gBmp2.Dispose();
                        }
                    } finally {
                        gBmp.Dispose();
                    }

                    // 表示の縮尺（左下に流れるように）
                    RectangleF[] size = new RectangleF[4];
                    size[0] = new RectangleF(0.0f, 0.00f, 1.00f, 1.0f);
                    size[1] = new RectangleF(0.0f, 0.15f, 0.95f, 1.0f);
                    size[2] = new RectangleF(0.0f, 0.45f, 0.85f, 1.0f);
                    size[3] = new RectangleF(0.0f, 0.80f, 0.70f, 1.0f);
                    int[] sleepTime = { 15, 10, 8, 3};

                    // ビジュアルベルを表示
                    for (int i = 0; i < size.Length; i++) {
                        RectangleF rect = new RectangleF(ClientRectangle.Width * size[i].Left, ClientRectangle.Height * size[i].Top, ClientRectangle.Width * size[i].Width, ClientRectangle.Height * size[i].Height);
                        gPanel.Graphics.FillRectangle(Brushes.Black, rect);
                        Thread.Sleep(15);
                        gPanel.Graphics.DrawImage(bmp, 0, 0);
                        Thread.Sleep(sleepTime[i]);
                    }
                } finally {
                    bmp.Dispose();
                }
            } finally {
                gPanel.Dispose();
            }
            Invalidate();
        }

        //=========================================================================================
        // 機　能：マウスのボタンがダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TextFileViewer_MouseDoubleClick(object sender, MouseEventArgs evt) {
            if (m_currentViewerComponent == null) {
                return;
            }
            m_currentViewerComponent.OnMouseDoubleClick(evt);
        }

        //=========================================================================================
        // 機　能：マウスのボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TextFileViewer_MouseDown(object sender, MouseEventArgs evt) {
            if (m_currentViewerComponent == null) {
                return;
            }
            // 直前のドラッグ中に別のボタンが押されたときは解放
            if (m_mouseDrag) {
                Capture = false;
                m_mouseDrag = true;
                m_currentViewerComponent.CancelSelect();
                return;
            }
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left) {
                Capture = true;
                m_mouseDrag = true;
                m_currentViewerComponent.OnMouseDown(evt);
            }
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TextFileViewer_MouseMove(object sender, MouseEventArgs evt) {
            if (m_currentViewerComponent == null) {
                return;
            }
            if (!m_mouseDrag) {
                return;
            }
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left) {
                m_currentViewerComponent.OnMouseMove(evt.X, evt.Y);
            }
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TextFileViewer_MouseUp(object sender, MouseEventArgs evt) {
            if (m_currentViewerComponent == null) {
                return;
            }
            Capture = false;
            m_mouseDrag = false;
            if ((evt.Button & MouseButtons.Left) == MouseButtons.Left) {
                m_currentViewerComponent.OnMouseUp(evt);
            } else if ((evt.Button & MouseButtons.Right) == MouseButtons.Right) {
                m_currentViewerComponent.OnContextMenu(evt);
            }
        }

        //=========================================================================================
        // 機　能：マウスがドラッグ中の処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void OnCheckMouseDragContinue() {
            if (!m_mouseDrag) {
                return;
            }
            if (Win32API.Win32GetAsyncKeyState(Keys.LButton)) {
                Point cursorPos = PointToClient(Cursor.Position);
                m_currentViewerComponent.OnMouseMove(cursorPos.X, cursorPos.Y);
            }
        }

        //=========================================================================================
        // 機　能：検索が終了したことを通知する
        // 引　数：[in]isText  テキストビューアの検索が終了したときtrue
        // 戻り値：なし
        //=========================================================================================
        public void NotifySearchEnd(bool isText) {
            Invalidate();
            m_radarBar.NotifySearchEnd();
            if (isText) {
                m_statusBar.SetSearchHitCount(m_lineInfo.SearchHitCount, m_lineInfo.AutoSearchHitCount);
            } else {
                DumpSearchHitStateList hitState = m_searchEngine.DumpSearchHitStateList;
                m_statusBar.SetSearchHitCount(hitState.SearchHitCount, hitState.AutoHitCount);
            }
        }

        //=========================================================================================
        // 機　能：EOFを描画する
        // 引　数：[in]g     グラフィックス
        // 　　　　[in]xPos  表示X位置
        // 　　　　[in]yPos  表示Y位置
        // 　　　　[in]crlf  EOFの種類
        // 戻り値：なし
        //=========================================================================================
        public void DrawEofMark(TextViewerGraphics g, float xPos, float yPos, LineBreakChar crlf) {
            string eofMark;
            if (crlf == LineBreakChar.Eof) {
                eofMark = "EOF";
            } else {
                eofMark = "...";
            }
            float cx = GraphicsUtils.MeasureString(g.Graphics, g.TextFont, eofMark);

            Rectangle rc = new Rectangle((int)xPos, (int)yPos, (int)(cx - 1), g.TextFont.Height - 1);
            Color col2 = GraphicsUtils.BrendColor(SystemColors.Window, SystemColors.Window, Configuration.Current.TextViewerControlColor);
            Brush backBrush = new LinearGradientBrush(rc, SystemColors.Window, col2, LinearGradientMode.Vertical);
            Font font = new Font(g.TextFont.FontFamily, Configuration.Current.TextFontSize - 1.0f, FontStyle.Regular);
            try {
                g.Graphics.FillRectangle(backBrush, rc);
                g.Graphics.DrawRectangle(g.TextViewerControlPen, rc);
                g.Graphics.DrawString(eofMark, font, g.TextViewerControlBrush, new PointF(xPos + 1, yPos + 1));
            } finally {
                backBrush.Dispose();
                font.Dispose();
            }
        }
        
        //=========================================================================================
        // 機　能：ステータスバーのTABラベルがクリックされたときの処理を行う
        // 引　数：[in]label  クリックされたラベル
        // 戻り値：なし
        //=========================================================================================
        public void OnLabelTab(ToolStripStatusLabel label) {
            // 項目を追加
            List<MenuItemSetting> menu = new List<MenuItemSetting>();
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTabCommand), 2), '2', "TAB 2桁"));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTabCommand), 4), '4', "TAB 4桁"));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTabCommand), 8), '8', "TAB 8桁"));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTabCommand), 16), '1', "TAB 16桁"));

            // コンテキストメニューを表示
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.FileViewerMenu, this);
            menuImpl.AddItemsFromSetting(cms, cms.Items, menu, Program.Document.KeySetting.FileViewerKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            FileViewerMenuStrip.ModifyMenuItem(cms.Items, TextBufferLineInfo);
            ContextMenuStrip = cms;
            ContextMenuStrip.Show(this, new Point(label.Bounds.Left, Height - cms.Height - 20));
            ContextMenuStrip = null;
        }

        //=========================================================================================
        // 機　能：ステータスバーのエンコーディングラベルがクリックされたときの処理を行う
        // 引　数：[in]label  クリックされたラベル
        // 戻り値：なし
        //=========================================================================================
        public void OnLabelEncoding(ToolStripStatusLabel label) {
            // 項目を追加
            List<MenuItemSetting> menu = new List<MenuItemSetting>();
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ChangeTextModeCommand)), 'T', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ChangeDumpModeCommand)), 'D', null));
            menu.Add(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "SJIS"), 'S', "ShiftJIS"));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "UTF-8"), 'U', "UTF-8"));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "EUC-JP"), 'E', "EUC"));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "JIS"), 'J', "JIS"));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "UNICODE"), 'C', "UNICODE"));

            // コンテキストメニューを表示
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.FileViewerMenu, this);
            menuImpl.AddItemsFromSetting(cms, cms.Items, menu, Program.Document.KeySetting.FileViewerKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            FileViewerMenuStrip.ModifyMenuItem(cms.Items, TextBufferLineInfo);
            ContextMenuStrip = cms;
            ContextMenuStrip.Show(this, new Point(label.Bounds.Left, Height - cms.Height - 20));
            ContextMenuStrip = null;
        }

        //=========================================================================================
        // 機　能：ステータスバーの行番号ラベルがクリックされたときの処理を行う
        // 引　数：[in]label  クリックされたラベル
        // 戻り値：なし
        //=========================================================================================
        public void OnLabelLineNo(ToolStripStatusLabel label) {
            // 項目を追加
            List<MenuItemSetting> menu = new List<MenuItemSetting>();
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpTopLineCommand)), 'T', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpBottomLineCommand)), 'B', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), -1), 'J', null));

            // コンテキストメニューを表示
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.FileViewerMenu, this);
            menuImpl.AddItemsFromSetting(cms, cms.Items, menu, Program.Document.KeySetting.FileViewerKeyItemList, false, null);
            ContextMenuStrip = cms;
            ContextMenuStrip.Show(this, new Point(label.Bounds.Left, Height - cms.Height - 20));
            ContextMenuStrip = null;
        }

        //=========================================================================================
        // 機　能：検索ダイアログを表示する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ShowSearchDialog() {
            if (TextViewerComponent is TextViewerComponent) {
                if (m_textSearchDialog == null) {
                    TextViewerComponent viewer = (TextViewerComponent)(TextViewerComponent);
                    TextSearchCondition condition = viewer.SearchCondition;
                    m_textSearchDialog = new ViewerTextSearchDialog(this, condition);
                    m_textSearchDialog.Show(Parent);
                } else {
                    m_textSearchDialog.Focus();
                }
            } else if (TextViewerComponent is DumpViewerComponent) {
                if (m_dumpSearchDialog == null) {
                    DumpViewerComponent viewer = (DumpViewerComponent)(TextViewerComponent);
                    DumpSearchCondition condition = viewer.SearchCondition;
                    m_dumpSearchDialog = new ViewerDumpSearchDialog(this, condition);
                    m_dumpSearchDialog.Show(Parent);
                } else {
                    m_dumpSearchDialog.Focus();
                }
            }
        }
        
        //=========================================================================================
        // 機　能：ビューアの検索ダイアログが閉じられたときの処理を行う
        // 引　数：[in]textMode  テキストモードで閉じるときtrue、ダンプモードで閉じるときfalse
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseViewerSearchDialog(bool textMode) {
            // 検索ダイアログを閉じる
            if (textMode && m_textSearchDialog != null) {
                m_textSearchDialog = null;
            }
            if (!textMode && m_dumpSearchDialog != null) {
                m_dumpSearchDialog = null;
            }
        }

        //=========================================================================================
        // 機　能：テキストを保存する
        // 引　数：[in]fileName    ファイル名
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public bool SaveTextDataAs(string fileName) {
            byte[] readBuffer;
            int readSize;
            m_lineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);
            byte[] bytes = new byte[readSize];
            Array.Copy(readBuffer, bytes, bytes.Length);
            try {
                File.WriteAllBytes(fileName, bytes);
            } catch (Exception e) {
                InfoBox.Warning(this.FileViewerForm, Resources.Msg_FileViewerSaveFailed, e.Message);
                return false;
            }
            m_lineInfo.TargetFile.SaveRequired = false;
            return true;
        }

        //=========================================================================================
        // 機　能：2ストロークキーの状態が変わったことを通知する
        // 引　数：[in]newState  新しい状態
        // 戻り値：なし
        //=========================================================================================
        public void TwoStrokeKeyStateChanged(TwoStrokeType newState) {
            m_form.Toolbar.TwoStrokeKeyStateChanged(newState);
        }

        //=========================================================================================
        // プロパティ：テキストバッファの行情報
        //=========================================================================================
        public TextBufferLineInfoList TextBufferLineInfo {
            get {
                return m_lineInfo;
            }
        }

        //=========================================================================================
        // プロパティ：ビューア部分の実装
        //=========================================================================================
        public IViewerComponent TextViewerComponent {
            get {
                return m_currentViewerComponent;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューアの行番号表示領域の幅
        //=========================================================================================
        public float TextViewerLineNoAreaWidth {
            get {
                return m_textViewerComponent.LineNoAreaWidth;
            }
        }

        //=========================================================================================
        // プロパティ：ビューアのフォーム
        //=========================================================================================
        public FileViewerForm FileViewerForm {
            get {
                return m_form;
            }
        }

        //=========================================================================================
        // プロパティ：ステータスバー
        //=========================================================================================
        public FileViewerStatusBar FileViewerStatusBar {
            get {
                return m_statusBar;
            }
        }

        //=========================================================================================
        // プロパティ：検索エンジン
        //=========================================================================================
        public TextSearchEngine SearchEngine {
            get {
                return m_searchEngine;
            }
        }
    }
}
