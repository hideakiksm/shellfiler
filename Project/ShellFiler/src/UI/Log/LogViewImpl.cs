using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Util;

namespace ShellFiler.UI.Log {

    //=========================================================================================
    // クラス：ログウィンドウのUI
    //=========================================================================================
    public class LogViewImpl {
        // 描画時、画面外であっても余計に描画する行数
        private readonly int EXTRA_DRAW_LINE = 2;

        // 動作モード
        private LogMode m_logMode;

        // カレット（使用しないときnull、ON/OFFだけを制御）
        private Win32Caret m_caret;

        // 親となるパネル
        private ILogViewContainer m_parentContainer;

        // 親となるパネルのWindowsコンポーネント
        private ScrollableControl m_targetControl;

        // マウスのドラッグが継続中のとき、次のスクロールを起こすためのユーザーメッセージ
        private int m_umDragContinue;

        // 初期化されたときtrue
        private bool m_initialized = false;

        // ログウィンドウで表示する対象のログ出力バッファ
        private LogBuffer m_targetLogBuffer;

        // 初回のpaint実行時のときtrue
        private bool m_firstPaint = true;

        // 画面表示中の先頭行の行ID
        private long m_topLine;

        // 画面に完全に表示できる行数（実際は+1行まで表示、-1:未初期化）
        private int m_completeLineSize = -1;

        // 半角フォントの大きさ[ピクセル]
        private SizeF m_fontSize;

        // マウスドラッグによる選択（選択中ではないときnull）
        private MouseDragSelection m_mouseDrag = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent            親となるパネル
        // 　　　　[in]logMode           利用するログウィンドウのモード
        // 　　　　[in]dragContinue      マウスのドラッグが継続中のとき、次のスクロールを起こすためのユーザーメッセージ
        // 　　　　[in]customMouseWheel  MouseWheelイベントを外部でカスタマイズするときtrue
        // 戻り値：なし
        //=========================================================================================
        public LogViewImpl(ILogViewContainer parent, LogMode logMode, int dragContinue, bool customMouseWheel) {
            m_parentContainer = parent;
            m_targetControl = parent.View;
            m_logMode = logMode;
            m_umDragContinue = dragContinue;

            // イベントを接続
            m_targetControl.Resize += new EventHandler(this.LogPanel_Resize);
            m_targetControl.MouseLeave += new EventHandler(this.LogPanel_MouseLeave);
            m_targetControl.MouseMove += new MouseEventHandler(this.LogPanel_MouseMove);
            m_targetControl.MouseDown += new MouseEventHandler(this.LogPanel_MouseDown);
            m_targetControl.MouseUp += new MouseEventHandler(this.LogPanel_MouseUp);
            if (!customMouseWheel) {
                m_targetControl.MouseWheel += new MouseEventHandler(this.LogPanel_MouseWheel);
            }
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]logBuffer  表示対象のログバッファ
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(LogBuffer logBuffer) {
            LogGraphics g = new LogGraphics(Program.MainWindow, m_logMode);
            try {
                Font font;
                if (m_logMode == LogMode.LogWindow) {
                    font = g.LogWindowFont;
                } else {
                    font = g.LogWindowFixedFont;
                }
                m_fontSize = new SizeF(GraphicsUtils.MeasureString(g.Graphics, font, "MMMMMMMMMM") / 10.0f, font.Height);
            } finally {
                g.Dispose();
            }

            m_targetLogBuffer = logBuffer;
            m_topLine = 0;

            m_initialized = true;
        }

        //=========================================================================================
        // 機　能：垂直スクロールイベントを処理する
        // 引　数：[in]message  ウィンドウメッセージ
        // 戻り値：なし
        //=========================================================================================
        public void OnVScroll(Message message) {
            if (!m_initialized) {
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
            OnVScroll(evt);
        }

        //=========================================================================================
        // 機　能：スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        private void OnVScroll(ScrollEventArgs evt) {
            if (!m_initialized) {
                return;
            }
            // スクロールを処理
            int newPosition = (int)(m_topLine - m_targetLogBuffer.FirstAvailableLineId);
            switch (evt.Type) {
                case ScrollEventType.SmallDecrement:
                    newPosition--;
                    break;
                case ScrollEventType.LargeDecrement:
                    newPosition -= m_completeLineSize;
                    break;
                case ScrollEventType.SmallIncrement:
                    newPosition++;
                    break;
                case ScrollEventType.LargeIncrement:
                    newPosition += m_completeLineSize;
                    break;
                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:
                case ScrollEventType.EndScroll:
                    newPosition = evt.NewValue;
                    break;
            }

            SetVScrollPosition(newPosition);
        }

        //=========================================================================================
        // 機　能：スクロールを実行する
        // 引　数：[in]dy  スクロール量
        // 戻り値：なし
        //=========================================================================================
        public void ScrollLine(int dy) {
            if (!m_initialized) {
                return;
            }

            // スクロールを処理
            int newPosition = (int)(m_topLine - m_targetLogBuffer.FirstAvailableLineId);
            newPosition += dy;
            SetVScrollPosition(newPosition);
            m_parentContainer.OnMouseSelectionMove();
        }

        //=========================================================================================
        // 機　能：スクロール処理を行う
        // 引　数：[in]newPosition  新しい先頭からの相対位置
        // 戻り値：なし
        //=========================================================================================
        private void SetVScrollPosition(int newPosition) {
            // 新しい位置を検証
            if (newPosition > m_targetLogBuffer.AvailableLogCount - m_completeLineSize) {
                newPosition = (int)(m_targetLogBuffer.AvailableLogCount - m_completeLineSize);
            }
            if (newPosition < 0) {
                newPosition = 0;
            }

            // 移動に応じてスクロール
            int dy = (int)(newPosition + m_targetLogBuffer.FirstAvailableLineId - m_topLine);
            int logHeight = this.LogLineHeight;
            if (dy > 0) {
                // 下に移動
                Graphics g = m_targetControl.CreateGraphics();
                Win32API.Win32ScrollWindow(m_targetControl.Handle, 0, -dy * logHeight, m_targetControl.ClientRectangle, m_targetControl.ClientRectangle);
                m_topLine = newPosition + m_targetLogBuffer.FirstAvailableLineId;
            } else if (dy < 0) {
                // 上に移動
                Graphics g = m_targetControl.CreateGraphics();
                Win32API.Win32ScrollWindow(m_targetControl.Handle, 0, -dy * logHeight, m_targetControl.ClientRectangle, m_targetControl.ClientRectangle);
                m_topLine = newPosition + m_targetLogBuffer.FirstAvailableLineId;
            }

            // スクロールバーを設定
            m_targetControl.VerticalScroll.Value = (int)(m_topLine - m_targetLogBuffer.FirstAvailableLineId);

            m_parentContainer.OnMouseSelectionMove();
        }

        //=========================================================================================
        // 機　能：ドラッグ中にスクロールの継続処理の要求イベントが発生したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        // メ　モ：WM_PAINTで連鎖的に発生
        //=========================================================================================
        public void OnDragContinue() {
            if (m_mouseDrag != null) {
                if (Win32API.Win32GetAsyncKeyState(Keys.LButton)) {
                    Point cursorPos = m_targetControl.PointToClient(Cursor.Position);
                    m_mouseDrag.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 0, cursorPos.X, cursorPos.Y, 0));
                }
            }
        }

        //=========================================================================================
        // 機　能：描画イベント受信時の処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnPaint(PaintEventArgs evt) {
            try {
                if (!m_initialized) {
                    return;
                }

                // 初回の描画時にサイズ変更イベントを発生させる
                if (m_firstPaint) {
                    m_firstPaint = false;
                    LogPanel_Resize(null, null);
                }

                // 画面を描画
                CaretRedraw redraw = new CaretRedraw(m_caret);
                try {
                    DoubleBuffer db = new DoubleBuffer(evt.Graphics, m_targetControl.ClientRectangle.Width, m_targetControl.ClientRectangle.Height);
                    try {
                        LogGraphics g = new LogGraphics(db.DrawingGraphics, m_logMode, m_parentContainer.BackColorMode);
                        try {
                            OnPaintDirect(g, -1, -1);
                        } finally {
                            g.Dispose();
                        }
                    } finally {
                        db.FlushScreen(0, 0);
                    }
                } finally {
                   redraw.Resume();
                }

                // マウスのドラッグが継続状態かどうかを確認
                if (m_mouseDrag != null && m_mouseDrag.DragContinue) {
                    Win32API.Win32PostMessage(m_targetControl.Handle, m_umDragContinue, 0, 0);
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw e;
            }
        }

        //=========================================================================================
        // 機　能：画面を描画する
        // 引　数：[in]g             グラフィックス
        // 　　　　[in]dispStartLine 画面上の開始行（-1で全体を描画）
        // 　　　　[in]dispEndLine   画面上の終了行
        // 戻り値：なし
        //=========================================================================================
        public void OnPaintDirect(LogGraphics g, int dispStartLine, int dispEndLine) {
            if (!m_initialized) {
                return;
            }
            if (g.LogWindowBackBrush == null) {
                return;             // VisualStudioのデザイナー
            }
            CaretRedraw redraw = new CaretRedraw(m_caret);
            try {
                if (dispStartLine == -1) {
                    dispStartLine = 0;
                    dispEndLine = m_completeLineSize + EXTRA_DRAW_LINE;
                }

                LogViewSelectionRange selection = null;
                if (m_mouseDrag != null) {
                    selection = m_mouseDrag.SelectionRange;
                } else {
                    selection = m_parentContainer.ModifySelection();
                }

                g.Graphics.FillRectangle(g.LogWindowBackBrush, m_targetControl.ClientRectangle);
                for (int i = dispStartLine; i <= dispEndLine; i++) {
                    long lineId = m_topLine + i;
                    LogLine line = m_targetLogBuffer.LineIdToLogLine(lineId);
                    if (line == null) {
                        continue;
                    }
                    DrawingLogLineContext lineContext = GetLineContext(lineId, selection);
                    line.DrawLogLine(g, i, m_parentContainer, lineContext);
                }
            } finally {
                redraw.Resume();
            }
        }

        //=========================================================================================
        // 機　能：選択範囲の情報を行の描画コンテキストに変換する
        // 引　数：[in]lineId     描画する行ID
        // 　　　　[in]selection  選択情報（選択されていないときnull）
        // 戻り値：なし
        //=========================================================================================
        private DrawingLogLineContext GetLineContext(long lineId, LogViewSelectionRange selection) {
            DrawingLogLineContext lineContext;
            if (selection == null) {
                lineContext = new DrawingLogLineContext(-1, -1);
            } else if (lineId < selection.StartLine || selection.EndLine < lineId) {
                lineContext = new DrawingLogLineContext(-1, -1);
            } else if (lineId == selection.StartLine && lineId == selection.EndLine) {
                lineContext = new DrawingLogLineContext(selection.StartColumn, selection.EndColumn);
            } else if (lineId == selection.StartLine) {
                lineContext = new DrawingLogLineContext(selection.StartColumn, int.MaxValue);
            } else if (lineId == selection.EndLine) {
                lineContext = new DrawingLogLineContext(0, selection.EndColumn);
            } else {
                lineContext = new DrawingLogLineContext(0, int.MaxValue);
            }
            return lineContext;
        }

        //=========================================================================================
        // 機　能：サイズ変更時の処理を行う
        // 引　数：[in]sender   イベントの送信元(null:内部発生イベント)
        // 　　　　[in]evt      送信イベント(null:内部発生イベント)
        // 戻り値：なし
        //=========================================================================================
        private void LogPanel_Resize(object sender, EventArgs evt) {
            if (!m_initialized) {
                return;
            }

            // 前回最終位置を表示していたか？
            bool prevLastPosition = false;
            long lastLineId = m_targetLogBuffer.FirstAvailableLineId + m_targetLogBuffer.AvailableLogCount - 1;
            prevLastPosition = (lastLineId - m_topLine + 1 == m_completeLineSize);

            // 垂直方向の処理
            int cyHorzScrollbar = 0;
            if (m_targetControl.HorizontalScroll.Visible) {
                cyHorzScrollbar = SystemInformation.HorizontalScrollBarHeight;
            }
            int logHeight = this.LogLineHeight;
            m_completeLineSize = (m_targetControl.ClientRectangle.Height - cyHorzScrollbar) / logHeight;

            // 表示位置の調整
            if (prevLastPosition) {
                m_topLine = lastLineId - m_completeLineSize + 1;
                m_topLine = Math.Max(m_topLine, m_targetLogBuffer.FirstAvailableLineId);
            }

            // 先頭行は有効？
            LogLine topLine = m_targetLogBuffer.LineIdToLogLine(m_topLine);
            if (topLine == null) {
                m_topLine = lastLineId - m_completeLineSize + 1;
                m_topLine = Math.Max(m_topLine, m_targetLogBuffer.FirstAvailableLineId);
            }

            SetVerticalScrollbar();
            m_targetControl.Invalidate();
        }

        //=========================================================================================
        // 機　能：垂直スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void SetVerticalScrollbar() {
            if (m_targetLogBuffer.AvailableLogCount <= m_completeLineSize) {
                // スクロールバーを表示しない
                m_targetControl.VerticalScroll.Visible = true;
                m_targetControl.VerticalScroll.Enabled = false;
            } else {
                // スクロールバーを表示する
                long firstId = m_targetLogBuffer.FirstAvailableLineId;
                m_targetControl.VerticalScroll.SmallChange = 1;
                m_targetControl.VerticalScroll.LargeChange = m_completeLineSize;
                m_targetControl.VerticalScroll.Minimum = 0;
                m_targetControl.VerticalScroll.Maximum = m_targetLogBuffer.AvailableLogCount - 1;
                m_targetControl.VerticalScroll.Value = (int)(m_topLine - firstId);
                m_targetControl.VerticalScroll.Visible = true;
                m_targetControl.VerticalScroll.Enabled = true;
            }
        }
        
        //=========================================================================================
        // 機　能：ログを記録する
        // 引　数：[in]logLine   登録するログ情報
        // 　　　　[in]fastDraw  高速描画を行うときtrue
        // 戻り値：高速描画を行ったときtrue
        // メ　モ：delegateによりUIスレッドで実行する
        //=========================================================================================
        public bool OnRegistLogLine(LogLine logLine, bool fastDraw) {
            bool fastDrawDone = false;
            CaretRedraw redraw = new CaretRedraw(m_caret);
            try {
                int logHeight = this.LogLineHeight;
                int lineCountOrg = m_targetLogBuffer.AvailableLogCount;
                long targetLineId = m_targetLogBuffer.LogIdToLineId(logLine.LogId);
                if (targetLineId == -1) {
                    // すでに消えた行
                    ;
                } else {
                    if (m_topLine <= targetLineId && targetLineId <= m_topLine + m_completeLineSize + EXTRA_DRAW_LINE) {
                        if (targetLineId - m_topLine == m_completeLineSize) {
                            // スクロール
                            m_topLine++;
                            if (!fastDraw) {
                                Win32API.Win32ScrollWindow(m_targetControl.Handle, 0, -logHeight, m_targetControl.ClientRectangle, m_targetControl.ClientRectangle);
                            } else {
                                fastDrawDone = true;
                            }
                        }

                        // 1行分描画
                        DoubleBuffer doubleBuffer = new DoubleBuffer(m_targetControl, m_targetControl.ClientRectangle.Width, logHeight);
                        LogGraphics g = new LogGraphics(doubleBuffer.DrawingGraphics, m_logMode, m_parentContainer.BackColorMode);
                        g.Graphics.FillRectangle(g.LogWindowBackBrush, doubleBuffer.DrawingRectangle);
                        DrawingLogLineContext lineContext = GetLineContext(targetLineId, null);
                        logLine.DrawLogLine(g, 0, m_parentContainer, lineContext);
                        int yPos = ((int)(targetLineId - m_topLine)) * logHeight;
                        doubleBuffer.FlushScreen(0, yPos);
                    } else if (lineCountOrg == m_targetLogBuffer.AvailableLogCount) {
                        // 画面から消えている
                        if (m_topLine < m_targetLogBuffer.FirstAvailableLineId) {
                            // スクロール
                            int scroll = (int)(m_targetLogBuffer.FirstAvailableLineId - m_topLine);
                            m_topLine += scroll;
                            if (!fastDraw) {
                                Win32API.Win32ScrollWindow(m_targetControl.Handle, 0, -logHeight * scroll, m_targetControl.ClientRectangle, m_targetControl.ClientRectangle);
                            } else {
                                fastDrawDone = true;
                            }
                        }
                    }
                }
                if (m_topLine < m_targetLogBuffer.FirstAvailableLineId) {
                    m_topLine = m_targetLogBuffer.FirstAvailableLineId;
                    m_targetControl.Invalidate();
                }
                SetVerticalScrollbar();
            } finally {
                redraw.Resume();
            }
            return fastDrawDone;
        }

        //=========================================================================================
        // 機　能：ログの内容をすべて削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearLog() {
            ClearSelection();
            m_topLine = m_targetLogBuffer.FirstAvailableLineId;
            m_targetControl.Invalidate();
            LogPanel_Resize(null, null);
        }
        
        //=========================================================================================
        // 機　能：選択を解除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearSelection() {
            if (m_mouseDrag != null) {
                m_mouseDrag.Dispose();
                m_mouseDrag = null;
            }
            m_targetControl.Invalidate();
            m_parentContainer.OnStatusChanged(LogViewContainerStatusType.CancelSelection, null);
        }

        //=========================================================================================
        // 機　能：選択された範囲のテキストを返す
        // 引　数：[in]crlf       改行コード
        // 　　　　[out]text      選択された範囲のテキストを返す変数（選択中ではないときnull）
        // 　　　　[out]createAll すべて選択できたときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        public void GetSelectedText(string crlf, out string text, out bool createAll) {
            StringBuilder sb = null;
            if (m_mouseDrag == null) {
                text = null;
                createAll = false;
                return;
            }
            long startLineId = m_mouseDrag.SelectionRange.StartLine;
            long endLineId = m_mouseDrag.SelectionRange.EndLine;
            for (long i = startLineId; i <= endLineId; i++) {
                LogLine log = m_targetLogBuffer.LineIdToLogLine(i);
                if (log == null) {
                    if (sb != null) {
                        Program.Abort("LogLineが有効になった後、再びnullになりました。");
                        text = null;
                        createAll = false;
                        return;
                    }
                } else {
                    if (sb == null) {
                        sb = new StringBuilder();
                    }
                    int startColumn = 0;
                    int endColumn = int.MaxValue;
                    if (i == startLineId) {
                        startColumn = m_mouseDrag.SelectionRange.StartColumn;
                    }
                    if (i == endLineId) {
                        endColumn = m_mouseDrag.SelectionRange.EndColumn;
                    }
                    string lineText = log.GetSelectedLine(startColumn, endColumn);
                    if (lineText != null) {
                        sb.Append(lineText);
                        if (i != endLineId) {
                            sb.Append(crlf);
                        }
                    }
                }
            }
            if (sb == null) {
                text = "";
                createAll = false;
            } else {
                text = sb.ToString();
                createAll = true;
            }
        }

        //=========================================================================================
        // 機　能：すべてのテキストを返す
        // 引　数：[in]crlf   改行コード
        // 　　　　[out]text  選択された範囲のテキストを返す変数（選択中ではないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void GetAllText(string crlf, out string text) {
            StringBuilder sb = new StringBuilder();
            long startLineId = m_targetLogBuffer.FirstAvailableLineId;
            long endLineId = m_targetLogBuffer.NextLogId - 1;
            for (long i = startLineId; i <= endLineId; i++) {
                LogLine log = m_targetLogBuffer.LineIdToLogLine(i);
                if (log == null) {
                    Program.Abort("LogLineの状態が想定外です。");
                    text = null;
                    return;
                }
                string lineText = log.GetSelectedLine(0, int.MaxValue);
                if (lineText != null) {
                    sb.Append(lineText);
                    if (i != endLineId) {
                        sb.Append(crlf);
                    }
                }
            }
            text = sb.ToString();
        }
 
        //=========================================================================================
        // 機　能：ログ内容を再描画する
        // 引　数：[in]logLine  登録するログ情報
        // 戻り値：なし
        // メ　モ：delegateによりUIスレッドで実行する
        //=========================================================================================
        public void RedrawLogLine(LogLine logLine) {
            long targetLineId = m_targetLogBuffer.LogIdToLineId(logLine.LogId);
            if (targetLineId == -1) {
                return;
            }
            CaretRedraw redraw = new CaretRedraw(m_caret);
            try {
                if (m_topLine <= targetLineId && targetLineId <= m_topLine + m_completeLineSize + EXTRA_DRAW_LINE) {
                    int logHeight = this.LogLineHeight;
                    DoubleBuffer doubleBuffer = new DoubleBuffer(m_targetControl, m_targetControl.ClientRectangle.Width, logHeight);
                    LogGraphics g = new LogGraphics(doubleBuffer.DrawingGraphics, m_logMode, m_parentContainer.BackColorMode);
                    g.Graphics.FillRectangle(g.LogWindowBackBrush, doubleBuffer.DrawingRectangle);
                    DrawingLogLineContext lineContext = GetLineContext(targetLineId, SelectionRange);
                    logLine.DrawLogLine(g, 0, m_parentContainer, lineContext);
                    int yPos = ((int)(targetLineId - m_topLine)) * logHeight;
                    doubleBuffer.FlushScreen(0, yPos);
                }
            } finally {
                redraw.Resume();
            }
        }

        //=========================================================================================
        // 機　能：テキストをすべて選択する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SelectAllText() {
            if (m_mouseDrag != null) {
                m_mouseDrag.Dispose();
                m_mouseDrag = null;
            }

            // 最終行の右端を取得
            LogLine lastLine = m_targetLogBuffer.LineIdToLogLine(m_targetLogBuffer.NextLogId - 1);
            int endColumn = lastLine.SelectAllMaxColumn;

            // 選択状態にする
            LogViewSelectionRange selection = new LogViewSelectionRange();
            selection.StartLine = m_targetLogBuffer.FirstAvailableLineId;
            selection.StartColumn = 0;
            selection.EndLine = lastLine.LogId;
            selection.EndColumn = endColumn;
            m_mouseDrag = new MouseDragSelection(this, selection);
            m_parentContainer.OnStatusChanged(LogViewContainerStatusType.SelectionChange, selection);
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogPanel_MouseMove(object sender, MouseEventArgs evt) {
            if (!m_initialized) {
                return;
            }
            int logHeight = this.LogLineHeight;
            int scrLineNo = evt.Y / logHeight;
            long lineId = m_topLine + scrLineNo;
            LogLine line = m_targetLogBuffer.LineIdToLogLine(lineId);
            Cursor newCursor = Cursors.Arrow;
            if (line != null) {
                bool hit = line.HitTest(evt.Location);
                if (hit) {
                    newCursor = Cursors.Hand;
                }
            }
            if (newCursor != Cursor.Current) {
                Cursor.Current = newCursor;
            }
            if (m_mouseDrag != null) {
                m_mouseDrag.OnMouseMove(evt);
                m_parentContainer.OnMouseSelectionMove();
            }
        }

        //=========================================================================================
        // 機　能：マウスがフォームから外れたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogPanel_MouseLeave(object sender, EventArgs evt) {
            if (!m_initialized) {
                return;
            }
            Cursor.Current = Cursors.Arrow;
        }

        //=========================================================================================
        // 機　能：マウスのボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogPanel_MouseDown(object sender, MouseEventArgs evt) {
            if (!m_initialized) {
                return;
            }

            if ((evt.Button & MouseButtons.Left) == MouseButtons.Left) {
                m_parentContainer.OnMouseSelectStart();
                m_mouseDrag = new MouseDragSelection(this, evt);
            }
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogPanel_MouseUp(object sender, MouseEventArgs evt) {
            if (!m_initialized) {
                return;
            }
            if ((evt.Button & MouseButtons.Left) != MouseButtons.Left) {
                return;
            }
            int scrLineNo = evt.Y / this.LogLineHeight;
            long lineId = m_topLine + scrLineNo;
            LogLine line = m_targetLogBuffer.LineIdToLogLine(lineId);
            if (line != null) {
                line.OnClick(evt.Location);
            }
            if (m_mouseDrag != null) {
                bool success = m_mouseDrag.OnMouseUp(evt);
                if (!success) {
                    m_mouseDrag = null;
                    m_targetControl.Invalidate();
                }
                m_parentContainer.OnMouseSelectEnd();
            }
        }
        
        //=========================================================================================
        // 機　能：マウスホイールが操作されたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void LogPanel_MouseWheel(object sender, MouseEventArgs evt) {
            if (evt.Delta < 0) {
                int moveLine = Math.Max(1, (-evt.Delta) / 120);
                if (moveLine >= 2) {
                    moveLine *= 2;
                }
                ScrollLine(moveLine);
            } else if (evt.Delta > 0) {
                int moveLine = Math.Max(1, evt.Delta / 120);
                if (moveLine >= 2) {
                    moveLine *= 2;
                }
                ScrollLine(-moveLine);
            }
        }

        //=========================================================================================
        // プロパティ：カレット（使用しないときnull、ON/OFFだけを制御）
        //=========================================================================================
        public Win32Caret Caret {
            set {
                m_caret = value;
            }
        }

        //=========================================================================================
        // プロパティ：ログの行の高さ[ピクセル]
        //=========================================================================================
        public int LogLineHeight {
            get {
                return (int)(m_fontSize.Height);
            }
        }

        //=========================================================================================
        // プロパティ：半角フォントの大きさ[ピクセル]
        //=========================================================================================
        public SizeF FontSize {
            get {
                return m_fontSize;
            }
        }

        //=========================================================================================
        // プロパティ：画面表示中の先頭行の行ID
        //=========================================================================================
        public long TopLineId {
            get {
                return m_topLine;
            }
        }
        
        //=========================================================================================
        // プロパティ：選択範囲をドラッグ中のときtrue
        //=========================================================================================
        public bool IsMouseDrag {
            get {
                if (m_mouseDrag == null) {
                    return false;
                }
                return m_mouseDrag.DragContinue;
            }
        }

        //=========================================================================================
        // プロパティ：選択中の範囲（選択がないときnull）
        //=========================================================================================
        public LogViewSelectionRange SelectionRange {
            get {
                if (m_mouseDrag == null) {
                    return null;
                } else {
                    return m_mouseDrag.SelectionRange;
                }
            }
            set {
                if (value == null) {
                    m_mouseDrag = null;
                } else {
                    m_mouseDrag = new MouseDragSelection(this, value);
                }
            }
        }

        //=========================================================================================
        // プロパティ：ログ出力のモード
        //=========================================================================================
        public enum LogMode {
            LogWindow,                  // ログウィンドウで動作
            TerminalWindow,             // ターミナルウィンドウで動作
        }

        //=========================================================================================
        // クラス：マウスドラッグによる選択の実装
        //=========================================================================================
        private class MouseDragSelection {
            // 所有クラス
            private LogViewImpl m_parent;

            // 選択中の範囲
            private LogViewSelectionRange m_selectionRange;

            // マウスのボタンが離された後のときtrue
            private bool m_mouseUp = false;

            //=========================================================================================
            // 機　能：コンストラクタ（マウスのボタンが押されたとき）
            // 引　数：[in]parent   所有クラス
            // 　　　　[in]evt      マウスのボタンが押されたときの送信イベント
            // 戻り値：なし
            //=========================================================================================
            public MouseDragSelection(LogViewImpl parent, MouseEventArgs evt) {
                m_parent = parent;

                // 位置を計算
                long lineId;
                int column;
                GetLineColumnFromMousePos(evt, out lineId, out column);

                // 新しい選択状態
                m_selectionRange = new LogViewSelectionRange();
                m_selectionRange.StartLine = lineId;
                m_selectionRange.StartColumn = column;
                m_selectionRange.EndLine = lineId;
                m_selectionRange.EndColumn = column;
                m_selectionRange.FirstClickLine = lineId;
                m_selectionRange.FirstClickColumn = column;
                m_selectionRange.PrevLine = lineId;
                m_selectionRange.PrevColumn = column;
                m_parent.m_targetControl.Invalidate();
                m_parent.m_targetControl.Capture = true;
                m_parent.m_parentContainer.OnStatusChanged(LogViewContainerStatusType.StartSelection, null);
            }

            //=========================================================================================
            // 機　能：コンストラクタ（選択状態を強制的に作り出すとき）
            // 引　数：[in]parent     所有クラス
            // 　　　　[in]selection  選択状態
            // 戻り値：なし
            //=========================================================================================
            public MouseDragSelection(LogViewImpl parent, LogViewSelectionRange selection) {
                m_parent = parent;
                m_selectionRange = selection;
                m_mouseUp = true;
            }

            //=========================================================================================
            // 機　能：選択状態を破棄する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Dispose() {
                if (!m_mouseUp) {
                    m_parent.m_targetControl.Capture = false;
                }
            }

            //=========================================================================================
            // 機　能：マウスのボタンが離されたされたときの処理を行う
            // 引　数：[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public bool OnMouseUp(MouseEventArgs evt) {
                if (m_parent.m_targetControl.IsDisposed) {
                    return false;
                }
                m_parent.m_targetControl.Capture = false;
                m_mouseUp = true;

                // 位置を計算
                long lineId;
                int column;
                GetLineColumnFromMousePos(evt, out lineId, out column);

                if (lineId == m_selectionRange.FirstClickLine && column == m_selectionRange.FirstClickColumn) {
                    m_parent.m_targetControl.Invalidate();
                    m_parent.m_parentContainer.OnStatusChanged(LogViewContainerStatusType.CancelSelection, null);
                    return false;
                }
                return true;
            }

            //=========================================================================================
            // 機　能：マウスが動いたときの処理を行う
            // 引　数：[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void OnMouseMove(MouseEventArgs evt) {
                if (m_mouseUp) {
                    return;
                }

                // 位置を計算
                long lineId;
                int column;
                GetLineColumnFromMousePos(evt, out lineId, out column);
                
                // 場所の更新
                if (lineId < m_selectionRange.FirstClickLine || lineId == m_selectionRange.FirstClickLine && column <= m_selectionRange.FirstClickColumn) {
                    m_selectionRange.StartLine = lineId;
                    m_selectionRange.StartColumn = column;
                    m_selectionRange.EndLine = m_selectionRange.FirstClickLine;
                    m_selectionRange.EndColumn = m_selectionRange.FirstClickColumn;
                } else {
                    m_selectionRange.StartLine = m_selectionRange.FirstClickLine;
                    m_selectionRange.StartColumn = m_selectionRange.FirstClickColumn;
                    m_selectionRange.EndLine = lineId;
                    m_selectionRange.EndColumn = column;
                }

                // 再描画
                if (m_selectionRange.CheckFirstSelected()) {
                    m_parent.m_targetControl.Invalidate();
                    m_parent.m_targetControl.Update();
                } else {
                    CaretRedraw redraw = new CaretRedraw(m_parent.m_caret);
                    try {
                        ScrollableControl control = m_parent.m_targetControl;
                        int cyLineHeight = m_parent.LogLineHeight;
                        int origin = (int)(Math.Min(m_selectionRange.PrevLine, lineId) - m_parent.m_topLine);
                        int height = (int)(Math.Abs(m_selectionRange.PrevLine - lineId));
                        DoubleBuffer db = new DoubleBuffer(control, control.ClientRectangle.Width, (height + 1) * cyLineHeight);
                        try {
                            LogGraphics g = new LogGraphics(db.DrawingGraphics, m_parent.m_logMode, m_parent.m_parentContainer.BackColorMode);
                            try {
                                g.Graphics.FillRectangle(SystemBrushes.Window, db.DrawingRectangle);
                                g.Graphics.TranslateTransform(0, -origin * cyLineHeight);
                                m_parent.OnPaintDirect(g, origin, origin + height);
                            } finally {
                                g.Dispose();
                            }
                        } finally {
                            db.FlushScreen(0, origin * cyLineHeight);
                        }
                    } finally {
                        redraw.Resume();
                    }
                }

                // スクロール
                MouseScroll(lineId);

                // ステータスバーを更新
                if (lineId != m_selectionRange.PrevLine || column != m_selectionRange.PrevColumn) {
                    m_parent.m_parentContainer.OnStatusChanged(LogViewContainerStatusType.SelectionChange, m_selectionRange);
                }

                // 現在位置を記憶
                m_selectionRange.PrevLine = lineId;
                m_selectionRange.PrevColumn = column;
            }

            //=========================================================================================
            // 機　能：マウスの位置から行とカラムの位置を返す
            // 引　数：[in]evt      マウスイベントの詳細情報
            // 　　　　[out]lineId  マウスカーソルの位置のログ行の行IDを返す変数
            // 　　　　[out]column  マウスカーソルの位置のカラム情報を返す変数
            // 戻り値：なし
            //=========================================================================================
            private void GetLineColumnFromMousePos(MouseEventArgs evt, out long lineId, out int column) {
                int scrLineNo = evt.Y / m_parent.LogLineHeight;
                lineId = m_parent.m_topLine + scrLineNo;
                lineId = Math.Min(m_parent.m_targetLogBuffer.NextLogId - 1, lineId);
                LogLine line = m_parent.m_targetLogBuffer.LineIdToLogLine(lineId);
                if (line == null) {
                    column = 0;
                } else {
                    LogGraphics g = new LogGraphics(m_parent.m_targetControl, m_parent.m_logMode);
                    try {
                        bool onCharDummy;
                        line.GetMouseHitColumn(m_parent.m_parentContainer, g, evt.Location, out column, out onCharDummy);
                    } finally {
                        g.Dispose();
                    }
                }
            }

            //=========================================================================================
            // 機　能：マウスによるスクロールを行う
            // 引　数：[in]topId  先頭に表示するログ行の行ID
            // 戻り値：なし
            //=========================================================================================
            public void MouseScroll(long topId) {
                ScrollableControl control = m_parent.m_targetControl;
                long newPosition = m_parent.m_topLine;
                if (topId < m_parent.m_topLine) {
                    newPosition = topId;
                } else if (topId > m_parent.m_topLine + m_parent.m_completeLineSize - 1) {
                    newPosition = topId - m_parent.m_completeLineSize + 1;
                }

                int dy;
                int sleepTime;
                ViewerScrollImpl.MouseOffsetToScrollLine((int)(newPosition - m_parent.m_topLine), out dy, out sleepTime);
                LogBuffer logBuffer = m_parent.m_targetLogBuffer;
                newPosition = Math.Max(logBuffer.FirstAvailableLineId, Math.Min(m_parent.m_topLine + dy, logBuffer.NextLogId - m_parent.m_completeLineSize));
                dy = (int)(newPosition - m_parent.m_topLine);
                if (dy != 0) {
                    CaretRedraw redraw = new CaretRedraw(m_parent.m_caret);
                    try {
                        Win32API.Win32ScrollWindow(control.Handle, 0, -dy * m_parent.LogLineHeight, control.ClientRectangle, control.ClientRectangle);
                        m_parent.m_topLine = m_parent.m_topLine + dy;
                        m_parent.SetVScrollPosition((int)(m_parent.m_topLine - logBuffer.FirstAvailableLineId));

                        if (sleepTime > 0) {
                            control.Update();
                            Thread.Sleep(sleepTime);
                        }
                    } finally {
                        redraw.Resume();
                    }
                }
            }

            //=========================================================================================
            // プロパティ：選択中の範囲
            //=========================================================================================
            public LogViewSelectionRange SelectionRange {
                get {
                    return m_selectionRange;
                }
            }

            //=========================================================================================
            // プロパティ：ドラッグが継続中のときtrue
            //=========================================================================================
            public bool DragContinue {
                get {
                    return !m_mouseUp;
                }
            }
        }
    }
}
