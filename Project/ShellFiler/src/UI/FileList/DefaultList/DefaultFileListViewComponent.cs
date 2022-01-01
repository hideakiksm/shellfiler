using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI.FileList.Crawler;
using ShellFiler.FileTask.Condition;

namespace ShellFiler.UI.FileList.DefaultList {

    //=========================================================================================
    // クラス：ファイル一覧のUI制御部分本体
    //=========================================================================================
    class DefaultFileListViewComponent : IFileListViewComponent {
        // 水平スクロールバーの移動量（小）
        const int HORZ_SCROLL_SMALL_CHANGE = 1;

        // 水平スクロールバーの移動量（大）
        const int HORZ_SCROLL_LARGE_CHANGE = 64;

        // 親クラス
        private FileListView m_parent;

        // ヘッダの描画クラス
        private FilePanelHeader m_filePanelHeader;

        // 行のレンダリングクラス
        private FileLineRenderer m_fileLineRenderer;

        // ヘッダの高さ
        private int m_cyHeader;

        // 水平スクロールバーの位置
        private int m_horzScrollPosition;

        // 画面表示中の先頭行のファイル全体でのインデックス
        private int m_topLine;

        // カーソル行の画面内でのインデックス
        private int m_cursorScreenLine;

        // 画面に完全に表示できる行数（実際は+1行まで表示）
        private int m_completeLineSize;
        
        // 直前のウィンドウ幅
        private int m_prevWindowWidth;

        // エクスプローラ風選択の基準となる行番号（-1:準備なし）
        private int m_explorerSelectStartLine = -1;

        // コンテキストメニュー（表示中ではないときnull）
        ExplorerMenuImpl m_contextMenuImpl = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DefaultFileListViewComponent() {
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]fileListView 親ウィンドウ
        // 　　　　[in]viewState    ビューの初期状態
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(FileListView fileListView, IFileListViewState viewState) {
            m_parent = fileListView;

            m_filePanelHeader = new FilePanelHeader(m_parent);
            m_filePanelHeader.HeaderSortChanged += new FilePanelHeader.HeaderSortChangedEventHandler(FilePanelHeader_HeaderSortChanged);
            m_cyHeader = m_filePanelHeader.CyHeader;
            m_fileLineRenderer = new FileLineRenderer(m_parent, m_filePanelHeader);

            DefaultFileListViewState defaultViewState = (DefaultFileListViewState)viewState;
            m_topLine = defaultViewState.TopLine;
            m_cursorScreenLine = defaultViewState.CursorScreenLine;

            OnRefreshDirectory(new ChangeDirectoryParam.Initial(m_parent.FileList.DisplayDirectoryName));
        }

        //=========================================================================================
        // 機　能：ビューの後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisposeView() {
            m_filePanelHeader.DisposeControl();
        }

        //=========================================================================================
        // 機　能：デフォルトのビュー状態を返す
        // 引　数：なし
        // 戻り値：デフォルトのビュー状態
        //=========================================================================================
        public static IFileListViewState GetDefaultViewState() {
            return new DefaultFileListViewState(0, 0);
        }

        //=========================================================================================
        // 機　能：UIのビューの状態を読み込む
        // 引　数：[in]viewState   ビューの状態の読み込み元
        // 戻り値：なし
        //=========================================================================================
        public void LoadComponentViewState(IFileListViewState viewState) {
            DefaultFileListViewState defaultViewState = (DefaultFileListViewState)viewState;
            m_topLine = defaultViewState.TopLine;
            m_cursorScreenLine = defaultViewState.CursorScreenLine;
            if (m_topLine + m_cursorScreenLine >= m_parent.FileList.Files.Count || m_cursorScreenLine >= m_completeLineSize) {
                m_topLine = 0;
                m_cursorScreenLine = 0;
            }

            // UIをリフレッシュ
            RefreshUI();
        }

        //=========================================================================================
        // 機　能：UIのビューの状態を保存する
        // 引　数：なし
        // 戻り値：ビューの状態
        //=========================================================================================
        public IFileListViewState SaveComponentViewState() {
            DefaultFileListViewState viewState = new DefaultFileListViewState(m_topLine, m_cursorScreenLine);
            return viewState;
        }
                
        //=========================================================================================
        // 機　能：ビューモードを変更する
        // 引　数：[in]viewMode   ビューモード
        // 戻り値：なし
        //=========================================================================================
        public void RefreshViewMode(FileListViewMode viewMode) {
        }

        //=========================================================================================
        // 機　能：ディレクトリ一覧をリフレッシュしたときの処理を行う
        // 引　数：[in]chdirParam  ディレクトリ変更のパラメータ
        // 戻り値：なし
        //=========================================================================================
        public void OnRefreshDirectory(ChangeDirectoryParam chdirParam) {
            UIFileList fileList = m_parent.FileList;

            // ファイル一覧中の位置をリフレッシュ
            if (chdirParam.CursorFile == null) {
                m_topLine = 0;
                m_cursorScreenLine = 0;
            } else {
                // 特定ファイルを先頭に表示
                SetCursorPositionWithFileName(chdirParam.CursorFile, chdirParam.StayCursorPosition);
            }
            SetVerticalScrollbar();

            // UIの状態をリフレッシュする
            RefreshUI();

            // 新規ファイルのクロールを開始
            Program.Document.FileCrawlThread.RequestNewFileCrawl(fileList, CrawlType.Icon, null);

            // 再描画
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：UIの状態をリフレッシュする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void RefreshUI() {
            UIFileList fileList = m_parent.FileList;

            // ヘッダをリフレッシュ
            bool refreshed = m_filePanelHeader.RefreshFilePanelHeader();
            if (refreshed) {
                m_fileLineRenderer.Refresh();
            }

            // アドレスバーをリフレッシュ
            string separator = fileList.FileSystem.GetPathSeparator(fileList.FileListContext);
            m_parent.ParentPanel.AddressBar.SetDirectoryName(fileList.DisplayDirectoryName, separator, fileList.FileListFilterMode);

            // ステータスバーをリフレッシュ
            m_parent.ParentPanel.StatusBar.RefreshStatusBar();
            Program.MainWindow.RefreshUIStatus();
        }

        //=========================================================================================
        // 機　能：カーソル位置を指定されたファイル名に変更する
        // 引　数：[in]cursorFileName   カーソルを合わせるファイル名
        // 　　　　[in]stayCursor       ファイル名がなくても、できるだけカーソル位置を動かさないときtrue
        // 戻り値：なし
        //=========================================================================================
        private void SetCursorPositionWithFileName(string cursorFileName, bool stayCursor) {
            UIFileList fileList = m_parent.FileList;
            if (m_completeLineSize == 0 || fileList.Files.Count == 0) {
                m_topLine = 0;
                m_cursorScreenLine = 0;
                return;
            }
            // 指定されたファイル名を検索
            int fileCount = fileList.Files.Count;
            int targetIndex = fileList.GetFileIndex(cursorFileName);

            if (targetIndex == -1 && !stayCursor) {
                // 見つからない場合は先頭を表示
                targetIndex = 0;
                m_topLine = 0;
                m_cursorScreenLine = 0;
            } else if (fileCount < m_completeLineSize) {
                // 1画面に収まる場合
                if (targetIndex != -1) {
                    m_topLine = 0;
                    m_cursorScreenLine = targetIndex;
                } else {
                    m_topLine = 0;
                    m_cursorScreenLine = Math.Min(m_cursorScreenLine, fileCount - 1);
                }
DebugCheckConsistency();
            } else {
                if (targetIndex == -1) {
                    // できるだけ動かさない場合
                    if (m_topLine + m_cursorScreenLine >= fileCount) {
                        if (fileCount <= m_completeLineSize) {
                            m_topLine = 0;
                            m_cursorScreenLine = Math.Min(m_cursorScreenLine, fileCount - 1);
                        } else {
                            m_topLine = fileCount - 1 - m_cursorScreenLine;
                        }
                    }
DebugCheckConsistency();
                } else {
                    if (m_topLine <= targetIndex && targetIndex < m_topLine + m_completeLineSize) {
                        // 画面上にある場合
                        m_cursorScreenLine = targetIndex - m_topLine;
                    } else {
                        // 画面外の場合
                        m_cursorScreenLine = Math.Min(fileCount - 1, m_completeLineSize / 2);
                        m_topLine = targetIndex - m_cursorScreenLine;
                    }
                }

                // 修正
                if (m_topLine < 0) {
                    m_cursorScreenLine = m_cursorScreenLine + m_topLine;
                    m_topLine = 0;
DebugCheckConsistency();
                } else if (fileCount - m_completeLineSize < m_topLine) {
                    int move = m_topLine - (fileCount - m_completeLineSize);
                    m_topLine -= move;
                    m_cursorScreenLine += move;
DebugCheckConsistency();
                }
            }
        }

        //=========================================================================================
        // 機　能：サイズ変更時の処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnSizeChange() {
            // 水平方向の処理
            bool resetHeaderPos = false;
            if (m_prevWindowWidth != m_parent.Width) {
                resetHeaderPos = true;
                m_prevWindowWidth = m_parent.Width;
                m_horzScrollPosition = 0;
            } else if (m_parent.Width < m_filePanelHeader.RequiredWidth) {
                m_horzScrollPosition = Math.Min(m_horzScrollPosition, m_filePanelHeader.RequiredWidth - m_parent.Width);
            } else {
                m_horzScrollPosition = 0;
            }

            // 垂直方向の処理
            int cyHorzScrollbar = 0;
            if (m_parent.Width < m_filePanelHeader.RequiredWidth) {
                cyHorzScrollbar = SystemInformation.HorizontalScrollBarHeight;
            }
            int cyMargin = m_fileLineRenderer.LineHeight;
            m_completeLineSize = (m_parent.Height - m_fileLineRenderer.LineHeight - cyHorzScrollbar - cyMargin) / m_fileLineRenderer.LineHeight;

            // カーソルの位置を調整
            int fileCount = m_parent.FileList.Files.Count;
            if (m_completeLineSize >= fileCount) {
                m_cursorScreenLine = m_topLine + m_cursorScreenLine;
                m_topLine = 0;
                DebugCheckConsistency();
            } else if (m_cursorScreenLine > m_completeLineSize) {
                int cursor = m_topLine + m_cursorScreenLine;
                m_cursorScreenLine = Math.Max(0, m_completeLineSize - 1);
                m_topLine = Math.Max(0, cursor - m_cursorScreenLine);
                m_cursorScreenLine = cursor - m_topLine;
                DebugCheckConsistency();
            } else if (fileCount - m_completeLineSize < m_topLine) {
                int cursor = m_topLine + m_cursorScreenLine;
                m_topLine = fileCount - m_completeLineSize;
                m_cursorScreenLine = cursor - m_topLine;
                DebugCheckConsistency();
            }

            m_filePanelHeader.OnSizeChange(resetHeaderPos);
            SetHorizontalScrollbar();
            SetVerticalScrollbar();
        }

        //=========================================================================================
        // 機　能：ファイルのスプリットの位置が更新されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnSplitChanged() {
        }

        //=========================================================================================
        // 機　能：水平スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetHorizontalScrollbar() {
            if (m_parent.ClientRectangle.Width >= m_filePanelHeader.RequiredWidth) {
                // スクロールバーを表示しない
                if (m_parent.HorizontalScroll.Visible != false) {
                    m_parent.HorizontalScroll.Visible = false;
                }
            } else {
                // スクロールバーを表示する
                if (m_parent.HorizontalScroll.Enabled != true) {
                    m_parent.HorizontalScroll.Enabled = true;
                }
                if (m_parent.HorizontalScroll.Visible != true) {
                    m_parent.HorizontalScroll.Visible = true;
                }
                m_parent.HorizontalScroll.SmallChange = 1;
                m_parent.HorizontalScroll.LargeChange = m_parent.ClientRectangle.Width;
                m_parent.HorizontalScroll.Minimum = 0;
                m_parent.HorizontalScroll.Maximum = m_filePanelHeader.RequiredWidth - 1;
                m_parent.HorizontalScroll.Value = m_horzScrollPosition;
            }
        }

        //=========================================================================================
        // 機　能：垂直スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void SetVerticalScrollbar() {
            if (m_parent.FileList.Files.Count <= m_completeLineSize || m_completeLineSize < 1) {
                // スクロールバーを表示しない
                if (m_parent.VerticalScroll.Visible != false) {
                    m_parent.VerticalScroll.Visible = false;
                }
            } else {
                // スクロールバーを表示する
                if (m_parent.VerticalScroll.Enabled != true) {
                    m_parent.VerticalScroll.Enabled = true;
                }
                if (m_parent.VerticalScroll.Visible != true) {
                    m_parent.VerticalScroll.Visible = true;
                }
                m_parent.VerticalScroll.SmallChange = 1;
                m_parent.VerticalScroll.LargeChange = m_completeLineSize;
                m_parent.VerticalScroll.Minimum = 0;
                m_parent.VerticalScroll.Maximum = m_parent.FileList.Files.Count - 1;
                m_parent.VerticalScroll.Value = m_topLine;
            }
        }
        
        //=========================================================================================
        // 機　能：グラフィックスを作成する
        // 引　数：なし
        // 戻り値：グラフィックス
        //=========================================================================================
        private FileListGraphics CreateFileListGraphics() {
            return new FileListGraphics(m_parent, m_filePanelHeader.CyHeader, m_fileLineRenderer.LineHeight);
        }
 
        //=========================================================================================
        // 機　能：画面を描画する
        // 引　数：[in]gcs    グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public void OnPaint(Graphics gcs) {
            FileListGraphics g = new FileListGraphics(gcs, m_filePanelHeader.CyHeader, m_fileLineRenderer.LineHeight);
            try {
                Rectangle rcHeader = m_parent.ClientRectangle;
                rcHeader.Height = m_filePanelHeader.CyHeader;
                Rectangle rcList = m_parent.ClientRectangle;
                rcList.Y = rcHeader.Bottom + 1;

                // ファイル一覧を描画
                g.Graphics.FillRectangle(g.FileListBackBrush, ScrollRectangle);
                if (m_completeLineSize == 0) {
                    // 表示できないとき
                    ;
                } else if (m_parent.FileList.Files.Count == 0) {
                    // ファイルがないとき
                    DrawCursorLine(g, 0, true);
                } else {
                    // ファイルがあるとき
                    int extendLine = SystemInformation.HorizontalScrollBarHeight / m_fileLineRenderer.LineHeight + 3;
                    for (int i = 0; i < m_completeLineSize + extendLine; i++) {
                        if (i == CursorLineNo - m_topLine) {
                            // カーソル行
                            DrawCursorLine(g, i, true);
                        } else {
                            // 通常行
                            DrawFileListLine(g, i, false, false);
                        }
                    }
                }
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：ファイルがないときのメッセージを描画する
        // 引　数：[in]g   描画に使用するグラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawNoFile(FileListGraphics g) {
            string message;
            if (m_parent.FileList.LoadingStatus != FileListLoadingStatus.Loading) {
                message = Resources.FileListNoFiles;
            } else {
                message = Resources.FileListNowLoading;
            }
            Font font = g.FileListFont;
            Brush fontBrush = m_fileLineRenderer.GetTextDrawBrush(g, null);
            SizeF messageSize = g.Graphics.MeasureString(message, font);
            int xPos = (int)((ScrollRectangle.Width - messageSize.Width) / 2);
            int yPos = m_cyHeader + (ScrollRectangle.Height - m_fileLineRenderer.LineHeight) / 2;
            g.Graphics.DrawString(message, font, fontBrush, xPos, yPos);
        }

        //=========================================================================================
        // 機　能：ファイルリストの1行分をカーソルなしで描画する
        // 引　数：[in]g         描画に使用するグラフィックス
        // 　　　　[in]scrLine   画面上の描画行
        // 　　　　[in]fillBack  マークがない場合にも背景を塗りつぶすときtrue
        // 　　　　[in]dblBuffer ダブルバッファリングを行うときtrue
        // 戻り値：なし
        //=========================================================================================
        private void DrawFileListLine(FileListGraphics g, int scrLine, bool fillBack, bool dblBuffer) {
            int idxTarget = scrLine + m_topLine;
            if (scrLine < 0 || idxTarget >= m_parent.FileList.Files.Count) {
                return;
            }

            int yPos = m_cyHeader + scrLine * m_fileLineRenderer.LineHeight;
            if (dblBuffer) {
                Bitmap bmpBuffer = new Bitmap(m_parent.ClientRectangle.Width, m_fileLineRenderer.LineHeight + 1);
                Graphics gBmp = Graphics.FromImage(bmpBuffer);
                FileListGraphics gDraw = new FileListGraphics(gBmp, 0, m_fileLineRenderer.LineHeight);
                try {
                    m_fileLineRenderer.DrawLine(gDraw, 0, idxTarget, true);
                } finally {
                    gDraw.Dispose();
                    gBmp.Dispose();
                    g.Graphics.DrawImage(bmpBuffer, 0, yPos);
                    bmpBuffer.Dispose();
                }
            } else {
                m_fileLineRenderer.DrawLine(g, yPos, idxTarget, fillBack);
            }
        }

        //=========================================================================================
        // 機　能：カーソルを描画する
        // 引　数：[in]g          描画に使用するグラフィックス
        // 　　　　[in]scrLine    描画する画面上の行
        // 　　　　[in]withCursor カーソルを描画するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void DrawCursorLine(FileListGraphics g, int scrLine, bool withCursor) {
            DebugCheckConsistency();
            if (scrLine >= m_completeLineSize + 1) {
                return;
            }
            if (m_parent.FileList.Files.Count == 0) {
                // ファイルなし
                // カーソルを描画
                if (withCursor && m_parent.HasCursor) {
                    int yPos = m_cyHeader + (ScrollRectangle.Height - m_fileLineRenderer.LineHeight) / 2 + m_fileLineRenderer.LineHeight - 1;
                    FileLineRenderer.DrawCursorLine(g, yPos, m_parent.ClientRectangle.Width);
                }

                // 画面内容を描画
                DrawNoFile(g);
            } else {
                // ファイルあり
                if (m_parent.FileList.Files.Count <= m_topLine + scrLine) {
                    return;
                }

                // 画面内容を描画
                DrawFileListLine(g, scrLine, true, true);

                // カーソルを描画
                if (withCursor && m_parent.HasCursor) {
                    int yPos = m_cyHeader + scrLine * m_fileLineRenderer.LineHeight + m_fileLineRenderer.LineHeight - 1;
                    FileLineRenderer.DrawCursorLine(g, yPos, m_parent.ClientRectangle.Width);
                }
            }
        }

        //=========================================================================================
        // 機　能：アイコン読み込み時にアイコンだけを描画する
        // 引　数：[in]index  描画するファイルのインデックス
        // 戻り値：なし
        //=========================================================================================
        public void DrawFileIcon(int index) {
            int scrLine = index - m_topLine;
            int extendLine = SystemInformation.HorizontalScrollBarHeight / m_fileLineRenderer.LineHeight + 1;
            if (scrLine < 0 || scrLine > m_completeLineSize + extendLine) {
                return;
            }
            FileListGraphics g = CreateFileListGraphics();
            try {
                int yPos = m_cyHeader + scrLine * m_fileLineRenderer.LineHeight;
                Bitmap bmpBuffer = new Bitmap(IconSize.Small16.CxIconSize, m_fileLineRenderer.LineHeight);
                Graphics gBmp = Graphics.FromImage(bmpBuffer);
                FileListGraphics gDraw = new FileListGraphics(gBmp, 0, m_fileLineRenderer.LineHeight);
                try {
                    m_fileLineRenderer.FillBack(gDraw, index, 0, 0, IconSize.Small16.CxIconSize, m_fileLineRenderer.LineHeight);
                    m_fileLineRenderer.DrawFileIcon(gDraw, 0, 0, index);
                    // カーソルを描画
                    if (m_parent.HasCursor && scrLine == m_cursorScreenLine) {
                        int yPosCursor = m_fileLineRenderer.LineHeight - 1;
                        gDraw.Graphics.DrawLine(g.FileListCursorPen, new Point(0, yPosCursor), new Point(bmpBuffer.Width, yPosCursor));
                    }
                } finally {
                    gDraw.Dispose();
                    gBmp.Dispose();
                    Rectangle srcRect = new Rectangle(0, FileLineRenderer.CY_ICON_ADJUST, IconSize.Small16.CxIconSize, IconSize.Small16.CyIconSize);
                    int xPos = m_filePanelHeader.GetItemXPos(m_filePanelHeader.FileListHeaderItemList[0].ItemID) + FileLineRenderer.MARGIN_ICON_LEFT;
                    g.Graphics.DrawImage(bmpBuffer, xPos, yPos + FileLineRenderer.CY_ICON_ADJUST, srcRect, GraphicsUnit.Pixel);
                    bmpBuffer.Dispose();
                }
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：水平スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnHScroll(ScrollEventArgs evt) {
            // こちらにカーソルを移動
            if (!m_parent.HasCursor) {
                m_parent.ToggleCursorLeftRight();
            }

            // スクロールを処理
            int newPosition = m_horzScrollPosition;
            switch(evt.Type) {
                case ScrollEventType.SmallDecrement:
                    newPosition -= HORZ_SCROLL_SMALL_CHANGE;
                    break;
                case ScrollEventType.LargeDecrement:
                    newPosition -= HORZ_SCROLL_LARGE_CHANGE;
                    break;
                case ScrollEventType.SmallIncrement:
                    newPosition += HORZ_SCROLL_SMALL_CHANGE;
                    break;
                case ScrollEventType.LargeIncrement:
                    newPosition += HORZ_SCROLL_LARGE_CHANGE;
                    break;
                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:
                case ScrollEventType.EndScroll:
                    newPosition = evt.NewValue;
                    break;
            }

            // 新しい位置を検証
            if (newPosition > m_filePanelHeader.RequiredWidth - m_parent.ClientRectangle.Width) {
                newPosition = m_filePanelHeader.RequiredWidth - m_parent.ClientRectangle.Width;
            }
            if (newPosition < 0) {
                newPosition = 0;
            }

            FileListGraphics g = CreateFileListGraphics();
            try {
                // 移動に応じてスクロール
                if (newPosition != m_horzScrollPosition) {
                    m_horzScrollPosition = newPosition;
                    m_parent.Invalidate();
                    m_filePanelHeader.OnHScroll();
                    m_parent.HorizontalScroll.Value = m_horzScrollPosition;
                }
            } finally {
                g.Dispose();
            }
        }


        //=========================================================================================
        // 機　能：垂直スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnVScroll(ScrollEventArgs evt) {
            // こちらにカーソルを移動
            if (!m_parent.HasCursor) {
                m_parent.ToggleCursorLeftRight();
            }

            // スクロールを処理
            int newPosition = m_topLine;
            switch(evt.Type) {
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

            // 新しい位置を検証
            if (newPosition < 0) {
                newPosition = 0;
            }
            UIFileList fileList = m_parent.FileList;
            if (newPosition > fileList.Files.Count - m_completeLineSize) {
                newPosition = fileList.Files.Count - m_completeLineSize;
            }

            FileListGraphics g = CreateFileListGraphics();
            try {
                // 移動に応じてスクロール
                int dy = newPosition - m_topLine;
                int scrollMargin = Configuration.Current.ListViewScrollMarginLineCount;
                if (dy > 0) {
                    // 下に移動
                    DrawCursorLine(g, m_cursorScreenLine, false);
                    if (m_cursorScreenLine >= scrollMargin) {
                        m_cursorScreenLine = Math.Max(scrollMargin, m_cursorScreenLine - dy);
                    }
                    DebugCheckConsistency();
                    Win32API.Win32ScrollWindow(m_parent.Handle, 0, -dy * m_fileLineRenderer.LineHeight, ScrollRectangle, ScrollRectangle);
                    m_topLine = newPosition;
                    DrawCursorLine(g, m_cursorScreenLine, true);
                } else if (dy < 0) {
                    // 上に移動
                    DrawCursorLine(g, m_cursorScreenLine, false);
                    if (m_cursorScreenLine <= m_completeLineSize - scrollMargin - 1) {
                        m_cursorScreenLine -= dy;
                        if (m_cursorScreenLine > m_completeLineSize - scrollMargin - 1) {
                            m_cursorScreenLine = m_completeLineSize - scrollMargin - 1;
                        }
                        DebugCheckConsistency();
                    }
                    Win32API.Win32ScrollWindow(m_parent.Handle, 0, -dy * m_fileLineRenderer.LineHeight, ScrollRectangle, ScrollRectangle);
                    m_topLine = newPosition;
                    DrawCursorLine(g, m_cursorScreenLine, true);
                }
            } finally {
                g.Dispose();
            }

            // スクロールバーを設定
            m_parent.VerticalScroll.Value = m_topLine;
        }

        //=========================================================================================
        // 機　能：マウスホイールを処理する
        // 引　数：[in]evt  マウスイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseWheel(MouseEventArgs evt) {
            if (evt.Delta < 0) {
                int moveLine = Math.Min(Configuration.Current.MouseWheelMaxLines, Math.Max(1, (-evt.Delta) / 120));
                CursorDown(moveLine, MarkOperation.None);
            } else if (evt.Delta > 0) {
                int moveLine = Math.Min(Configuration.Current.MouseWheelMaxLines, Math.Max(1, evt.Delta / 120));
                CursorUp(moveLine, MarkOperation.None);
            }
        }

        //=========================================================================================
        // 機　能：マウスのボタンが押されたときの処理を行う
        // 引　数：[in]cmd      マウス操作中のコマンド
        // 戻り値：実行結果
        //=========================================================================================
        public void OnMouseDown(AbstractMouseActionCommand cmd) {
            // こちらにカーソルを移動
            if (!m_parent.HasCursor) {
                m_parent.ToggleCursorLeftRight();
            }

            // 選択状態の連続反転ならモードを決定
            if (!(cmd is AbstractMouseMarkCommand)) {
                MoveCursorBelowMouse();
                return;
            }

            int oldMarkCount = m_parent.FileList.MarkedDirectoryCount + m_parent.FileList.MarkedFileCount;
            long oldMarkSize = m_parent.FileList.MarkedFileSize;

            AbstractMouseMarkCommand markCmd = (AbstractMouseMarkCommand)cmd;
            UIFileList fileList = m_parent.FileList;
            Point cursorPos = m_parent.PointToClient(Cursor.Position);
            int line = (cursorPos.Y - m_cyHeader) / m_fileLineRenderer.LineHeight + m_topLine;
            if (markCmd.MarkAction == MouseMarkAction.RevertSelect) {
                // はじめのファイルを基準に反転
                if (0 <= line && line <= fileList.Files.Count - 1) {
                    if (fileList.Files[line].Marked) {
                        markCmd.MarkAction = MouseMarkAction.ClearSelect;
                    } else {
                        markCmd.MarkAction = MouseMarkAction.MarkSelect;
                    }
                } else {
                    markCmd.MarkAction = MouseMarkAction.None;
                }
                m_explorerSelectStartLine = -1;
            } else if (markCmd.MarkAction == MouseMarkAction.MarkFirstOnly) {
                // はじめのファイルだけをマーク
                FileListGraphics g = CreateFileListGraphics();
                try {
                    if (0 <= line && line <= fileList.Files.Count - 1) {
                        if (m_cursorScreenLine + m_topLine != line) {
                            MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                            MarkOneFile(g, m_topLine, line - m_topLine, MarkOperation.Mark, false);
                        } else if (fileList.Files[line].Marked) {
                            MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                        } else {
                            MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                            MarkOneFile(g, m_topLine, line - m_topLine, MarkOperation.Mark, false);
                        }
                    }
                } finally {
                    g.Dispose();
                }
                markCmd.MarkAction = MouseMarkAction.None;
                m_explorerSelectStartLine = line;
            } else if (markCmd.MarkAction == MouseMarkAction.ExplorerMark) {
                // エクスプローラ風にマーク
                FileListGraphics g = CreateFileListGraphics();
                try {
                    if (m_explorerSelectStartLine == -1) {
                        // 初回選択時
                        if (0 <= line && line <= fileList.Files.Count - 1) {
                            MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                            MarkOneFile(g, m_topLine, line - m_topLine, MarkOperation.Mark, false);
                        }
                        m_explorerSelectStartLine = line;
                    } else {
                        // 前回選択との間をマーク
                        int start = Math.Min(m_explorerSelectStartLine, line);
                        int end = Math.Max(m_explorerSelectStartLine, line);
                        MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                        for (int i = start; i <= end; i++) {
                            MarkOneFile(g, m_topLine, i - m_topLine, MarkOperation.Mark, false);
                        }
                    }
                } finally {
                    g.Dispose();
                }
                markCmd.MarkAction = MouseMarkAction.None;
            }
            markCmd.LastMouseFilePos = Math.Max(0, Math.Min(fileList.Files.Count - 1, line));

            // UIをリフレッシュ
            int newMarkCount = m_parent.FileList.MarkedDirectoryCount + m_parent.FileList.MarkedFileCount;
            long newMarkSize = m_parent.FileList.MarkedFileSize;
            if (oldMarkCount != newMarkCount || oldMarkSize != newMarkSize) {
                m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                Program.MainWindow.RefreshUIStatus();
            }

            // 画面上のカーソル位置に移動
            OnMouseMove(markCmd);
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]cmd      マウス操作中のコマンド
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseMove(AbstractMouseActionCommand cmd) {
            if (!(cmd is AbstractMouseMarkCommand)) {
                MoveCursorBelowMouse();
                return;
            }
            AbstractMouseMarkCommand markCmd = (AbstractMouseMarkCommand)cmd;

            int oldMarkCount = m_parent.FileList.MarkedDirectoryCount + m_parent.FileList.MarkedFileCount;
            long oldMarkSize = m_parent.FileList.MarkedFileSize;

            // 画面上のカーソル位置に移動
            Point cursorPos = m_parent.PointToClient(Cursor.Position);
            int line = (cursorPos.Y - m_cyHeader) / m_fileLineRenderer.LineHeight;
            if (cursorPos.Y < m_cyHeader) {
                line--;
            }
            FileListGraphics g = CreateFileListGraphics();
            try {
                if (line < 0) {
                    // 画面外上
                    if (m_cursorScreenLine == 0) {
                        if (m_topLine > 0) {
                            // 上にスクロール
                            DrawCursorLine(g, m_cursorScreenLine, false);
                            int scrollLine = Math.Min(Math.Min(-line, Configuration.Current.FileListDragMaxSpeed), m_topLine);
                            Win32API.Win32ScrollWindow(m_parent.Handle, 0, scrollLine * m_fileLineRenderer.LineHeight, ScrollRectangle, ScrollRectangle);
                            m_cursorScreenLine = 0;
                            m_topLine -= scrollLine;
                            for (int i = m_topLine + m_cursorScreenLine; i <= markCmd.LastMouseFilePos; i++) {
                                MarkOneFile(g, m_topLine, i - m_topLine, markCmd.MarkOperation, false);
                            }
                            DrawCursorLine(g, m_cursorScreenLine, true);
                            m_parent.VerticalScroll.Value = m_topLine;
                            System.Threading.Thread.Sleep(30);
                        }
                    } else {
                        // カーソルを一番上へ
                        if (m_cursorScreenLine >= 0) {
                            DrawCursorLine(g, m_cursorScreenLine, false);
                            m_cursorScreenLine = 0;
                            for (int i = m_topLine + m_cursorScreenLine; i <= markCmd.LastMouseFilePos; i++) {
                                MarkOneFile(g, m_topLine, i - m_topLine, markCmd.MarkOperation, false);
                            }
                            DrawCursorLine(g, m_cursorScreenLine, true);
                        }
                    }
                } else if (line > m_completeLineSize - 1) {
                    // 画面外下
                    int fileCount = m_parent.FileList.Files.Count;
                    if (m_cursorScreenLine == m_completeLineSize - 1) {
                        if (m_topLine < fileCount - m_completeLineSize) {
                            // 下にスクロール
                            int scrollLine = Math.Min(Math.Min(line - m_completeLineSize + 1, Configuration.Current.FileListDragMaxSpeed), fileCount - m_topLine - m_completeLineSize);
                            DrawCursorLine(g, m_cursorScreenLine, false);
                            Win32API.Win32ScrollWindow(m_parent.Handle, 0, -scrollLine * m_fileLineRenderer.LineHeight, ScrollRectangle, ScrollRectangle);
                            m_cursorScreenLine = m_completeLineSize - 1;
                            m_topLine += scrollLine;
                            for (int i = markCmd.LastMouseFilePos; i <= m_topLine + m_cursorScreenLine; i++) {
                                MarkOneFile(g, m_topLine, i - m_topLine, markCmd.MarkOperation, false);
                            }
                            DrawCursorLine(g, m_cursorScreenLine, true);
                            m_parent.VerticalScroll.Value = m_topLine;
                            System.Threading.Thread.Sleep(30);
                            DebugCheckConsistency();
                        }
                    } else {
                        if (fileCount > m_completeLineSize) {
                            DrawCursorLine(g, m_cursorScreenLine, false);
                            m_cursorScreenLine = Math.Min(fileCount - m_topLine - 1, Math.Max(0, m_completeLineSize - 1));
                            for (int i = markCmd.LastMouseFilePos; i <= m_topLine + m_cursorScreenLine; i++) {
                                MarkOneFile(g, m_topLine, i - m_topLine, markCmd.MarkOperation, false);
                            }
                            DrawCursorLine(g, m_cursorScreenLine, true);
                            DebugCheckConsistency();
                        }
                    }
                } else {
                    // 画面内
                    int fileCount = m_parent.FileList.Files.Count;
                    if (fileCount - m_topLine <= line) {
                        line = Math.Max(0, fileCount - m_topLine - 1);
			        }
			        if(m_cursorScreenLine != line) {
                        DrawCursorLine(g, m_cursorScreenLine, false);
                        m_cursorScreenLine = line;
                        int start = Math.Min(markCmd.LastMouseFilePos, m_topLine + m_cursorScreenLine);
                        int end = Math.Max(markCmd.LastMouseFilePos, m_topLine + m_cursorScreenLine);
                        for (int i = start; i <= end; i++) {
                            MarkOneFile(g, m_topLine, i - m_topLine, markCmd.MarkOperation, false);
                        }
                        DrawCursorLine(g, m_cursorScreenLine, true);
                        DebugCheckConsistency();
			        } else if (markCmd.MarkOperation != MarkOperation.None) {
                        int start = Math.Min(markCmd.LastMouseFilePos, m_topLine + m_cursorScreenLine);
                        int end = Math.Max(markCmd.LastMouseFilePos, m_topLine + m_cursorScreenLine);
                        for (int i = start; i <= end; i++) {
                            MarkOneFile(g, m_topLine, i - m_topLine, markCmd.MarkOperation, false);
                        }
                    }
                }
            } finally {
                g.Dispose();
            }
            markCmd.LastMouseFilePos = Math.Max(0, Math.Min(m_parent.FileList.Files.Count - 1, m_topLine + m_cursorScreenLine));

            // UIをリフレッシュ
            int newMarkCount = m_parent.FileList.MarkedDirectoryCount + m_parent.FileList.MarkedFileCount;
            long newMarkSize = m_parent.FileList.MarkedFileSize;
            if (oldMarkCount != newMarkCount || oldMarkSize != newMarkSize) {
                m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                Program.MainWindow.RefreshUIStatus();
            }
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：[in]cmd      マウス操作中のコマンド
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseUp(AbstractMouseActionCommand cmd) {
            if (!(cmd is AbstractMouseMarkCommand)) {
                MoveCursorBelowMouse();
                return;
            }
        }
        
        //=========================================================================================
        // 機　能：ドラッグ開始のマウス位置かどうかを調べる
        // 引　数：[in]mouseX  マウスのX位置
        // 　　　　[in]mouseY  マウスのY位置
        // 戻り値：ドラッグ開始位置のときtrue
        //=========================================================================================
        public bool CheckDragStartPosition(int mouseX, int mouseY) {
            mouseX += m_horzScrollPosition;
            int fileCount = m_parent.FileList.Files.Count;
            int line = (mouseY - m_cyHeader) / m_fileLineRenderer.LineHeight + m_topLine;
            if (line < fileCount && mouseX < FileLineRenderer.IconRegionRight) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：ドラッグ＆ドロップを開始する
        // 引　数：[in]mouseX  マウスのX位置
        // 　　　　[in]mouseY  マウスのY位置
        // 戻り値：なし
        //=========================================================================================
        public void BeginDragDrop(int mouseX, int mouseY) {
            int scrLine = (mouseY - m_cyHeader) / m_fileLineRenderer.LineHeight;
            FileListGraphics g = CreateFileListGraphics();
            try {
                MarkOneFile(g, m_topLine, scrLine, MarkOperation.Mark, true);
            } finally {
                g.Dispose();
            }
            List<UIFile> markedList = m_parent.FileList.MarkFiles;
            if (markedList.Count == 0) {
                return;
            }
            PrepareVirtualFolderCommand.ReadyForVirtualFolder(m_parent);
            string[] markedFileNameList = UIFileList.GetFullPathFileNameList(m_parent.FileList, markedList, true);
            m_parent.BeginDragDrop(markedFileNameList);
        }

        //=========================================================================================
        // 機　能：カーソルの位置をマウスの直下に移動する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void MoveCursorBelowMouse() {
            int fileCount = m_parent.FileList.Files.Count;
            if (fileCount == 0) {
                return;
            }
            Point cursorPos = m_parent.PointToClient(Cursor.Position);
            int line = (cursorPos.Y - m_cyHeader) / m_fileLineRenderer.LineHeight;
            line = Math.Max(0, Math.Min(fileCount - 1 - m_topLine, line));
	        if (m_cursorScreenLine != line) {
                FileListGraphics g = CreateFileListGraphics();
                try {
                    DrawCursorLine(g, m_cursorScreenLine, false);
                    m_cursorScreenLine = line;
                    DrawCursorLine(g, m_cursorScreenLine, true);
                } finally {
                    g.Dispose();
                }
            }
            DebugCheckConsistency();
        }

        //=========================================================================================
        // 機　能：カーソルを上に移動する
        // 引　数：[in]lines    移動する行数
        // 　　　　[in]withMark マーク付きで移動するかどうか（Revert以外が有効）
        // 戻り値：新しいカーソルの位置
        //=========================================================================================
        public int CursorUp(int lines, MarkOperation withMark) {
            int fileCount = m_parent.FileList.Files.Count;
            if (fileCount == 0) {
                return CursorLineNo;
            }

            FileListGraphics g = CreateFileListGraphics();
            try {
                // マーク状態を変更
                int oldCursorPos = m_topLine + m_cursorScreenLine;
                int markCursorPos = -1;
                if (withMark == MarkOperation.Clear) {
                    if (m_explorerSelectStartLine != -1) {
                        MarkAllFile(MarkAllFileMode.ClearAll, true, null);
                        m_explorerSelectStartLine = -1;
                    }
                } else if (withMark == MarkOperation.Mark) {
                    if (m_explorerSelectStartLine == -1) {
                        m_explorerSelectStartLine = m_cursorScreenLine + m_topLine;
                        MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                        MarkOneFile(g, m_topLine, m_cursorScreenLine, MarkOperation.Mark, false);
                    }
                    markCursorPos = m_cursorScreenLine + m_topLine;
                }

                // 移動する
                DrawCursorLine(g, m_cursorScreenLine, false);
                int scrollMargin = Configuration.Current.ListViewScrollMarginLineCount;
                if (m_completeLineSize <= scrollMargin) {
                    int cursorLine = m_topLine + m_cursorScreenLine - lines;
                    cursorLine = Math.Max(cursorLine, 0);
                    m_topLine = cursorLine;
                    m_cursorScreenLine = 0;
                    m_parent.Invalidate();
                } else {
                    if (m_cursorScreenLine - lines <= scrollMargin - 1) {
                        // スクロールする場合
                        if (CursorLineNo - lines <= scrollMargin - 1) {
                            // 最後の数行
                            if (m_topLine == 0) {
                                // 画面内の移動のみ
                                m_cursorScreenLine = Math.Max(0, m_cursorScreenLine - lines);
                                DebugCheckConsistency();
                            } else {
                                // スクロールも伴う
                                Win32API.Win32ScrollWindow(m_parent.Handle, 0, m_topLine * m_fileLineRenderer.LineHeight, ScrollRectangle, ScrollRectangle);
                                m_cursorScreenLine = 0;
                                m_topLine = 0;
                            }
                        } else {
                            // マージン内
                            int newLine = m_topLine + m_cursorScreenLine - lines;
                            int scrollY = lines - Math.Max(0, (m_cursorScreenLine - scrollMargin));
                            if (m_cursorScreenLine - scrollMargin < 0) {
                                scrollY++;
                            }
                            m_topLine -= scrollY;
                            m_cursorScreenLine = newLine - m_topLine;
                            Win32API.Win32ScrollWindow(m_parent.Handle, 0, scrollY * m_fileLineRenderer.LineHeight, ScrollRectangle, ScrollRectangle);
                            DebugCheckConsistency();
                        }
                    } else {
                        // スクロールしない場合
                        m_cursorScreenLine -= lines;
                        DebugCheckConsistency();
                    }
                }
                int newCursorPos = m_cursorScreenLine + m_topLine;

                // マークする
                if (markCursorPos != -1 && newCursorPos != oldCursorPos) {
                    if (m_explorerSelectStartLine < oldCursorPos) {
                        if (newCursorPos <= m_explorerSelectStartLine) {
                            for (int i = newCursorPos; i <= m_explorerSelectStartLine; i++) {
                                MarkOneFile(g, m_topLine, i - m_topLine, MarkOperation.Mark, false);
                            }
                            for (int i = m_explorerSelectStartLine + 1; i <= oldCursorPos; i++) {
                                MarkOneFile(g, m_topLine, i - m_topLine, MarkOperation.Clear, false);
                            }
                        } else {
                            for (int i = newCursorPos + 1; i <= oldCursorPos; i++) {
                                MarkOneFile(g, m_topLine, i - m_topLine, MarkOperation.Clear, false);
                            }
                        }
                    } else {
                        for (int i = newCursorPos; i <= oldCursorPos; i++) {
                            MarkOneFile(g, m_topLine, i - m_topLine, MarkOperation.Mark, false);
                        }
                    }
                    // UIをリフレッシュ
                    m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                    Program.MainWindow.RefreshUIStatus();
                }

                // カーソルを描画
                DrawCursorLine(g, m_cursorScreenLine, true);
            } finally {
                g.Dispose();
            }

            // スクロールバーを再設定
            SetVerticalScrollbar();

            return CursorLineNo;
        }

        //=========================================================================================
        // 機　能：カーソルを下に移動する
        // 引　数：[in]lines    移動する行数
        // 　　　　[in]withMark マーク付きで移動するかどうか（Revert以外が有効）
        // 戻り値：新しいカーソルの位置
        //=========================================================================================
        public int CursorDown(int lines, MarkOperation withMark) {
            int fileCount = m_parent.FileList.Files.Count;
            if (fileCount == 0) {
                return CursorLineNo;
            }

            FileListGraphics g = CreateFileListGraphics();
            try {
                // マーク状態を変更
                int oldCursorPos = m_topLine + m_cursorScreenLine;
                int markCursorPos = -1;
                if (withMark == MarkOperation.Clear) {
                    if (m_explorerSelectStartLine != -1) {
                        MarkAllFile(MarkAllFileMode.ClearAll, true, null);
                        m_explorerSelectStartLine = -1;
                    }
                } else if (withMark == MarkOperation.Mark) {
                    if (m_explorerSelectStartLine == -1) {
                        m_explorerSelectStartLine = m_cursorScreenLine + m_topLine;
                        MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                        MarkOneFile(g, m_topLine, m_cursorScreenLine, MarkOperation.Mark, false);
                    }
                    markCursorPos = m_cursorScreenLine + m_topLine;
                }

                // 移動する
                DrawCursorLine(g, m_cursorScreenLine, false);
                int scrollMargin = Configuration.Current.ListViewScrollMarginLineCount;
                if (m_completeLineSize <= scrollMargin) {
                    int cursorLine = m_topLine + m_cursorScreenLine + lines;
                    cursorLine = Math.Min(cursorLine, fileCount - 1);
                    m_topLine = cursorLine;
                    m_cursorScreenLine = 0;
                    m_parent.Invalidate();
                } else if (m_cursorScreenLine + lines >= m_completeLineSize - scrollMargin) {
                    // スクロールする場合
                    if (CursorLineNo + lines >= fileCount - scrollMargin) {
                        // 最後の数行
                        if (m_topLine == fileCount - Math.Min(m_completeLineSize, fileCount)) {
                            // 移動のみ
                            m_cursorScreenLine = Math.Min(m_cursorScreenLine + lines, m_completeLineSize - 1);
                            if (CursorLineNo > fileCount - 1) {
                                m_cursorScreenLine = fileCount - 1 - m_topLine;
                                DebugCheckConsistency();
                            }
                        } else {
                            // スクロールも伴う
                            int scrollY = (fileCount - m_topLine - m_completeLineSize) * m_fileLineRenderer.LineHeight;
                            Win32API.Win32ScrollWindow(m_parent.Handle, 0, -scrollY, ScrollRectangle, ScrollRectangle);
                            m_cursorScreenLine = m_completeLineSize - 1;
                            m_topLine = fileCount - m_completeLineSize;
                            DebugCheckConsistency();
                        }
                    } else {
                        // マージン内
                        int newLine = m_topLine + m_cursorScreenLine + lines;
                        int scrollY = lines - Math.Max(0, (m_completeLineSize - scrollMargin - m_cursorScreenLine)) + 1;
                        m_topLine += scrollY;
                        m_cursorScreenLine = newLine - m_topLine;
                        DebugCheckConsistency();
                        Win32API.Win32ScrollWindow(m_parent.Handle, 0, -scrollY * m_fileLineRenderer.LineHeight, ScrollRectangle, ScrollRectangle);
                    }
                } else {
                    // スクロールしない
                    m_cursorScreenLine += lines;
                    if (CursorLineNo > fileCount - 1) {
                        m_cursorScreenLine = fileCount - 1 - m_topLine;
                    }
                    DebugCheckConsistency();
                }
                int newCursorPos = m_cursorScreenLine + m_topLine;

                // マーク処理
                if (markCursorPos != -1 && newCursorPos != oldCursorPos) {
                    if (oldCursorPos < m_explorerSelectStartLine) {
                        if (m_explorerSelectStartLine <= newCursorPos) {
                            for (int i = oldCursorPos; i <= m_explorerSelectStartLine - 1; i++) {
                                MarkOneFile(g, m_topLine, i - m_topLine, MarkOperation.Clear, false);
                            }
                            for (int i = m_explorerSelectStartLine; i <= newCursorPos; i++) {
                                MarkOneFile(g, m_topLine, i - m_topLine, MarkOperation.Mark, false);
                            }
                        } else {
                            for (int i = oldCursorPos; i <= newCursorPos - 1; i++) {
                                MarkOneFile(g, m_topLine, i - m_topLine, MarkOperation.Clear, false);
                            }
                        }
                    } else {
                        for (int i = oldCursorPos; i <= newCursorPos; i++) {
                            MarkOneFile(g, m_topLine, i - m_topLine, MarkOperation.Mark, false);
                        }
                    }

                    // UIをリフレッシュ
                    m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                    Program.MainWindow.RefreshUIStatus();
                }

                // カーソルを描画
                DrawCursorLine(g, m_cursorScreenLine, true);
            } finally {
                g.Dispose();
            }

            // スクロールバーを再設定
            SetVerticalScrollbar();

            return CursorLineNo;
        }
                
        //=========================================================================================
        // 機　能：カーソルを左に移動する
        // 引　数：[in]toggleWin  ウィンドウの切り替えを行うときtrue
        // 　　　　[in]withMark   マーク付きで移動するかどうか（Revert以外が有効）
        // 戻り値：新しいカーソルの位置（ウィンドウ切り替えの操作を行ったとき-1）
        //=========================================================================================
        public int CursorLeft(bool toggleWin, MarkOperation withMark) {
            FileListViewUtils.CursorLeft(m_parent);
            return -1;
        }

        //=========================================================================================
        // 機　能：カーソルを右に移動する
        // 引　数：[in]toggleWin  ウィンドウの切り替えを行うときtrue
        // 　　　　[in]withMark   マーク付きで移動するかどうか（Revert以外が有効）
        // 戻り値：新しいカーソルの位置（ウィンドウ切り替えの操作を行ったとき-1）
        //=========================================================================================
        public int CursorRight(bool toggleWin, MarkOperation withMark) {
            FileListViewUtils.CursorRight(m_parent);
            return -1;
        }

        //=========================================================================================
        // 機　能：カーソルを次のファイルに移動する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CursorNext() {
            CursorDown(1, MarkOperation.None);
        }

        //=========================================================================================
        // 機　能：ファイルまたはディレクトリをマークする
        // 引　数：[in]stay  カーソル位置を保持するときtrue
        // 戻り値：新しいマークの状態
        //=========================================================================================
        public bool Mark(bool stay) {
            bool marked;
            FileListGraphics g = CreateFileListGraphics();
            try {
                marked = MarkOneFile(g, m_topLine, m_cursorScreenLine, MarkOperation.Revert, true);
                if (stay) {
                    CursorDown(1, MarkOperation.None);
                }
                int fileCount = m_parent.FileList.Files.Count;
                if (fileCount - 1 == m_topLine + m_cursorScreenLine) {
                    DrawCursorLine(g, m_cursorScreenLine, true);
                }
            } finally {
                g.Dispose();
            }
            return marked;
        }

        //=========================================================================================
        // 機　能：指定された名前のファイルをマークする
        // 引　数：[in]targetFileList ファイル名の一覧
        // 　　　　[in]markOpr        マークに対する操作
        // 戻り値：すべてのファイルをマークできたときtrue
        //=========================================================================================
        public bool MarkSpecifiedFile(List<string> targetFileList, MarkOperation markOpr) {
            // ファイル名から位置へのインデックスを作成
            Dictionary<string, int> fileNameToIndex = new Dictionary<string, int>();
            List<UIFile> fileList = m_parent.FileList.Files;
            for (int i = 0; i < fileList.Count; i++) {
                fileNameToIndex.Add(fileList[i].FileName, i);
            }

            bool successAll = true;
            FileListGraphics g = CreateFileListGraphics();
            try {
                for (int i = 0; i < targetFileList.Count; i++) {
                    if (fileNameToIndex.ContainsKey(targetFileList[i])) {
                        int fileIndex = fileNameToIndex[targetFileList[i]];
                        int scrLine = fileIndex - m_topLine;                // マイナスになることがあるが、そのまま実行
                        MarkOneFile(g, m_topLine, scrLine, markOpr, false);
                    } else {
                        successAll = false;
                    }
                }
            } finally {
                g.Dispose();
            }

            // ステータスバーをリフレッシュ
            m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
            Program.MainWindow.RefreshUIStatus();

            return successAll;
        }

        //=========================================================================================
        // 機　能：指定された位置のファイルまたはディレクトリをマークする
        // 引　数：[in]g          グラフィックス
        // 　　　　[in]topLine    マークするファイルの画面の先頭行
        // 　　　　[in]scrLine    マークするファイルの画面中の相対行
        // 　　　　[in]markOpr    マークに対する操作
        // 　　　　[in]updateUI   UIの更新も行うときtrue
        // 戻り値：新しいマークの状態
        //=========================================================================================
        private bool MarkOneFile(FileListGraphics g, int topLine, int scrLine, MarkOperation markOpr, bool updateUI) {
            // ファイルが存在するか確認
            UIFileList fileList = m_parent.FileList;
            int line = topLine + scrLine;
            if (line < 0 || line >= fileList.Files.Count) {
                return false;
            }
            // 親ディレクトリはマークしない
            UIFile file = fileList.Files[line];
            string fileName = file.FileName;
            if (fileName == "..") {
                return false;
            }
            // マークを実行
            bool prevMark = fileList.Files[line].Marked;
            if (markOpr == MarkOperation.Mark) {
                fileList.SetMarked(line, true);
            } else if (markOpr == MarkOperation.Clear) {
                fileList.SetMarked(line, false);
            } else if (markOpr == MarkOperation.Revert) {
                fileList.SetMarked(line, !fileList.Files[line].Marked);
            } else if (markOpr == MarkOperation.None) {
                return fileList.Files[line].Marked;
            }

            // UIに反映
            if (prevMark != fileList.Files[line].Marked) {
                if (m_cursorScreenLine == scrLine) {
                    DrawCursorLine(g, scrLine, true);
                } else {
                    DrawFileListLine(g, scrLine, true, true);
                }
            }

            if (updateUI) {
                // ステータスバーをリフレッシュ
                m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                Program.MainWindow.RefreshUIStatus();
            }

            return fileList.Files[line].Marked;
        }

        //=========================================================================================
        // 機　能：すべてのオブジェクトのマーク状態を変更する
        // 引　数：[in]markMode   マークの方法
        // 　　　　[in]updateUI   UIの更新も行うときtrue
        // 　　　　[in]condition  マークする条件（条件がないときnull） 
        // 戻り値：マーク状態を変更したオブジェクトの数
        //=========================================================================================
        public int MarkAllFile(MarkAllFileMode markMode, bool updateUI, CompareCondition condition) {
            int count = FileListViewUtils.MarkAllFile(m_parent, markMode, updateUI, condition);
            return count;
        }

        //=========================================================================================
        // 機　能：ファイルをインクリメンタルサーチする
        // 引　数：[in]searchText  検索文字列
        // 　　　　[in]fromHead    ファイル名の先頭から比較するときtrue
        // 　　　　[in]operation   ファイル操作の種類
        // 戻り値：検索にヒットしたときtrue、見つからないとき/これ以上操作できないときfalse
        //=========================================================================================
        public bool IncrementalSearch(string searchText, bool fromHead, IncrementalSearchOperation operation) {
            IncrementalSearchImpl.MarkDelegate markDelegate = delegate() {
                FileListGraphics g = CreateFileListGraphics();
                try {
                    MarkOneFile(g, m_topLine, m_cursorScreenLine, MarkOperation.Revert, true);
                } finally {
                    g.Dispose();
                }
            };

            IncrementalSearchImpl impl = new IncrementalSearchImpl(m_parent.FileList, this, markDelegate);
            bool hit = impl.IncrementalSearch(searchText, fromHead, operation);
            return hit;
        }

        //=========================================================================================
        // 機　能：ファイル一覧中の指定された行にカーソルを移動する
        // 引　数：[in]index  移動先ファイルのインデックス
        // 戻り値：なし
        //=========================================================================================
        public void MoveCursorLine(int index) {
            // 移動なしで済む場合
            if (index == m_topLine + m_cursorScreenLine) {
                return;
            }
            if (index < 0 || index >= m_parent.FileList.Files.Count) {
                return;
            }

            // 位置を移動
            FileListGraphics g = CreateFileListGraphics();
            try {
                DrawCursorLine(g, m_cursorScreenLine, false);

                // 新しい位置を計算
                int oldTopLine = m_topLine;
            	int center = m_completeLineSize / 2;
	            if (index < center) {
		            m_topLine = 0;
		            m_cursorScreenLine = index;
	            } else if(index > m_parent.FileList.Files.Count - 1 - center) {
		            m_topLine = m_parent.FileList.Files.Count - m_completeLineSize;
		            m_cursorScreenLine = index - m_topLine;
	            } else {
		            m_cursorScreenLine = center;
		            m_topLine = index - m_cursorScreenLine;
	            }
	            if (m_topLine < 0) {
		            m_topLine = 0;
		            m_cursorScreenLine = index;
	            }

                // 移動
                if (oldTopLine != m_topLine) {
                    int scrollY = (oldTopLine - m_topLine) * m_fileLineRenderer.LineHeight;
                    Win32API.Win32ScrollWindow(m_parent.Handle, 0, scrollY, ScrollRectangle, ScrollRectangle);
                    m_parent.Update();
                }

                DrawCursorLine(g, m_cursorScreenLine, true);
            } finally {
                g.Dispose();
            }

            // スクロールバーを再設定
            SetVerticalScrollbar();
        }

        //=========================================================================================
        // 機　能：コンテキストメニューを表示する
        // 引　数：[in]fileName    対象のファイル名
        // 　　　　[in]menuPos     メニューを表示する位置
        // 戻り値：なし
        //=========================================================================================
        public void ContextMenuImpl(string fileName, ContextMenuPosition menuPos) {
            // 表示位置を決定
            const int X_POS_MENU = 50;
            Point ptMenu = Point.Empty;
            switch (menuPos) {
                case ContextMenuPosition.OnFile: {
                    int yPos = (m_cursorScreenLine + 1) * m_fileLineRenderer.LineHeight + m_cyHeader;
                    ptMenu = m_parent.PointToScreen(new Point(X_POS_MENU, yPos));
                    break;
                }
                case ContextMenuPosition.FileListTop: {
                    ptMenu = m_parent.PointToScreen(new Point(X_POS_MENU, 0));
                    break;
                }
                case ContextMenuPosition.OnMouse: {
                    ptMenu = ExplorerMenuImpl.GetPointOnMouse(m_parent);
                    break;
                }
            }

            // メニューを作成して表示
            m_contextMenuImpl = new ExplorerMenuImpl(m_parent);
            m_contextMenuImpl.ShowMenu(fileName, ptMenu);
            m_contextMenuImpl = null;
        }

        //=========================================================================================
        // 機　能：エクスプローラメニュー表示中のとき、メッセージを中継する
        // 引　数：[in]message    ウィンドウメッセージ
        // 　　　　[in]wParam     ウィンドウメッセージのパラメータ
        // 　　　　[in]lParam     ウィンドウメッセージのパラメータ
        // 戻り値：なし
        //=========================================================================================
        public void HandleExplorerMenuMessage(int message, IntPtr wParam, IntPtr lParam) {
            if (m_contextMenuImpl != null) {
                m_contextMenuImpl.HandleExplorerMenuMessage(message, wParam, lParam);
            }
        }

        //=========================================================================================
        // 機　能：ヘッダのソート状態が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FilePanelHeader_HeaderSortChanged(object sender, FilePanelHeader.HeaderSortChangedEventArgs evt) {
            Program.MainWindow.OnUICommand(UICommandSender.FileListHeader, new UICommandItem.HeaderSortCommand(evt.SortMode, evt.SortDirection));
        }

        //=========================================================================================
        // 機　能：状態一覧パネルのアクティブ状態を設定する
        // 引　数：[in]isActive  状態一覧パネルがアクティブになったときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnActivateStateListPanel(bool isActive) {
            m_filePanelHeader.OnActivateStateListPanel(isActive);
        }

        //=========================================================================================
        // プロパティ：画面に完全に表示できる行数
        //=========================================================================================
        public int CompleteScreenLineSize {
            get {
                return m_completeLineSize;
            }
        }

        //=========================================================================================
        // プロパティ：カーソル行
        //=========================================================================================
        public int CursorLineNo {
            get {
                return m_topLine + m_cursorScreenLine;
            }
        }

        //=========================================================================================
        // プロパティ：水平スクロールバーの位置
        //=========================================================================================
        public int HorzScrollPosition {
            get {
                return m_horzScrollPosition;
            }
        }

        //=========================================================================================
        // プロパティ：スクロール範囲の長方形
        //=========================================================================================
        public Rectangle ScrollRectangle {
            get {
                Rectangle rect = m_parent.ClientRectangle;
                return new Rectangle(rect.X, rect.Y + m_cyHeader, rect.Width, rect.Height - m_cyHeader);
            }
        }
        private void DebugCheckConsistency() {
            int count = m_parent.FileList.Files.Count;
            if (m_cursorScreenLine < 0 || m_topLine + m_cursorScreenLine >= count && count > 0) {
                Program.Abort("カーソルの制御状態が異常です。m_topLine={0}, m_cursorScreenLine={1}, fileCount={2}", m_topLine, m_cursorScreenLine, count);
            }
        }
    }
}
