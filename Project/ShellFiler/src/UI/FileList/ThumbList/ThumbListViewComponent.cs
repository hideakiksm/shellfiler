using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI.FileList.Crawler;

namespace ShellFiler.UI.FileList.ThumbList {

    //=========================================================================================
    // クラス：サムネイルによるファイル一覧コンポーネント
    //=========================================================================================
    public class ThumbListViewComponent : IFileListViewComponent {
        // 親クラス
        private FileListView m_parent;

        // 行のレンダリングクラス
        private ThumbListRenderer m_fileLineRenderer;

        // 画面表示中の先頭行
        private int m_topLine;

        // カーソル行の画面内での横方向インデックス
        private int m_cursorScreenX;

        // カーソル行の画面内での縦方向インデックス
        private int m_cursorScreenY;

        // 画面に入る画像の横方向の最大数
        private int m_completeColumnSize;

        // 画面に入る画像の縦方向の最大数（実際は+1行まで表示）
        private int m_completeLineSize;

        // エクスプローラ風選択の基準となるファイルのインデックス（-1:準備なし）
        private int m_explorerSelectStartIndex = -1;

        // コンテキストメニュー（表示中ではないときnull）
        ExplorerMenuImpl m_contextMenuImpl = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ThumbListViewComponent() {
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]fileListView 親となるファイル一覧ビュー
        // 　　　　[in]viewState    ビューの初期状態
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(FileListView fileListView, IFileListViewState viewState) {
            m_parent = fileListView;

            ThumbListViewState thumbViewState = (ThumbListViewState)viewState;
            FileListViewMode viewMode = thumbViewState.FileListViewMode;
            m_fileLineRenderer = new ThumbListRenderer(fileListView, viewMode);

            OnRefreshDirectory(new ChangeDirectoryParam.Initial(m_parent.FileList.DisplayDirectoryName));
        }
        
        //=========================================================================================
        // 機　能：ビューの後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisposeView() {
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
                m_cursorScreenX = 0;
                m_cursorScreenY = 0;
            } else {
                // 特定ファイルを先頭に表示
                SetCursorPositionWithFileName(chdirParam.CursorFile, chdirParam.StayCursorPosition);
            }

            // UIの状態をリフレッシュする
            RefreshUI();
            SetVerticalScrollbar();

            // 実行中のクロールを中止
            Program.Document.FileCrawlThread.StopFileCrawl(fileList);
            Program.Document.FileIconManager.ClearWindowIcon(m_parent.FileList.UIFileListId);

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
                m_cursorScreenX = 0;
                m_cursorScreenY = 0;
                return;
            }
            // 指定されたファイル名を検索
            int fileCount = fileList.Files.Count;
            int lineCount = (fileCount - 1) / m_completeColumnSize + 1;
            int targetIndex = fileList.GetFileIndex(cursorFileName);

            if (targetIndex == -1 && !stayCursor) {
                // 見つからない場合は先頭を表示
                targetIndex = 0;
                m_topLine = 0;
                m_cursorScreenX = 0;
                m_cursorScreenY = 0;
            } else if (lineCount < m_completeLineSize) {
                // 1画面に収まる場合
                if (targetIndex != -1) {
                    m_topLine = 0;
                    m_cursorScreenX = targetIndex % m_completeColumnSize;
                    m_cursorScreenY = targetIndex / m_completeColumnSize;
                } else {
                    m_topLine = 0;
                    m_cursorScreenY = Math.Min(m_cursorScreenY, lineCount - 1);
                    if (m_topLine + m_cursorScreenY == lineCount - 1) {
                        m_cursorScreenX = Math.Min(m_cursorScreenX, (fileCount - 1) % m_completeColumnSize);
                    }
                }
            } else {
                if (targetIndex == -1) {
                    // できるだけ動かさない場合
                    if ((m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX >= fileCount) {
                        if (lineCount <= m_completeLineSize) {
                            m_topLine = 0;
                            m_cursorScreenY = Math.Min(m_cursorScreenY, lineCount - 1);
                        } else {
                            m_topLine = Math.Max(0, lineCount - m_cursorScreenY - 1);
                        }
                        if (m_topLine + m_cursorScreenY == lineCount - 1) {
                            m_cursorScreenX = Math.Min(m_cursorScreenX, (fileCount - 1) % m_completeColumnSize);
                        }
                    }
                } else {
                    if (m_topLine * m_completeColumnSize <= targetIndex && targetIndex < (m_topLine + m_completeLineSize) * m_completeColumnSize) {
                        // 画面上にある場合
                        m_cursorScreenX = targetIndex % m_completeColumnSize;
                        m_cursorScreenY = targetIndex / m_completeColumnSize - m_topLine;
                    } else {
                        // 画面外の場合
                        m_cursorScreenX = targetIndex % m_completeColumnSize;
                        m_cursorScreenY = Math.Min(lineCount - 1, m_completeLineSize / 2);
                        m_topLine = targetIndex / m_completeColumnSize - m_cursorScreenY;
                    }
                }

                // 修正
                if (m_topLine < 0) {
                    m_cursorScreenY = 0;
                    m_topLine = targetIndex / m_completeColumnSize;
                    m_cursorScreenX = targetIndex % m_completeColumnSize;
                } else if (lineCount - m_completeLineSize < m_topLine) {
                    int move = m_topLine - (lineCount - m_completeLineSize);
                    m_topLine -= move;
                    m_cursorScreenY += move;
                    if (m_topLine + m_cursorScreenY == lineCount - 1) {
                        m_cursorScreenX = Math.Min(m_cursorScreenX, (fileCount - 1) % m_completeColumnSize);
                    }
                }
            }
        }
       
        //=========================================================================================
        // 機　能：UIのビューの状態を読み込む
        // 引　数：[in]viewState   ビューの状態の読み込み元
        // 戻り値：なし
        //=========================================================================================
        public void LoadComponentViewState(IFileListViewState viewState) {
            ThumbListViewState thumbViewState = (ThumbListViewState)viewState;
            m_topLine = thumbViewState.TopLine;
            m_cursorScreenX = thumbViewState.CursorScreenX;
            m_cursorScreenY = thumbViewState.CursorScreenY;
            m_parent.FileList.FileListViewMode = (FileListViewMode)(thumbViewState.FileListViewMode.Clone());
            int index = (m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX;
            if (index >= m_parent.FileList.Files.Count || m_cursorScreenX >= m_completeColumnSize || m_cursorScreenY >= m_completeLineSize) {
                m_topLine = 0;
                m_cursorScreenX = 0;
                m_cursorScreenY = 0;
            }

            // UIをリフレッシュ
            RefreshUI();
        }

        //=========================================================================================
        // 機　能：デフォルトのビュー状態を返す
        // 引　数：[in]viewMode  サムネイルビューのモード
        // 戻り値：デフォルトのビュー状態
        //=========================================================================================
        public static IFileListViewState GetDefaultViewState(FileListViewMode viewMode) {
            return new ThumbListViewState(viewMode, 0, 0, 0);
        }

        //=========================================================================================
        // 機　能：UIのビューの状態を保存する
        // 引　数：なし
        // 戻り値：ビューの状態
        //=========================================================================================
        public IFileListViewState SaveComponentViewState() {
            FileListViewMode mode = (FileListViewMode)(m_parent.FileList.FileListViewMode.Clone());
            ThumbListViewState viewState = new ThumbListViewState(mode, m_topLine, m_cursorScreenX, m_cursorScreenY);
            return viewState;
        }
        
        //=========================================================================================
        // 機　能：ビューモードを変更する
        // 引　数：[in]viewMode   ビューモード
        // 戻り値：なし
        //=========================================================================================
        public void RefreshViewMode(FileListViewMode viewMode) {
            m_fileLineRenderer = new ThumbListRenderer(m_parent, viewMode);
            OnSizeChange();
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：サイズ変更時の処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnSizeChange() {
            // サイズを計算
            int oldCursorPos = CursorLineNo;
            int cyHorzScrollbar = SystemInformation.HorizontalScrollBarHeight;
            int cxVerticalScrollbar = SystemInformation.VerticalScrollBarWidth;

            Size itemSize = m_fileLineRenderer.FileItemSizeDpiModified;
            m_completeColumnSize = Math.Max(1, (m_parent.Width - cxVerticalScrollbar) / (itemSize.Width + ThumbListRenderer.MARGIN_ITEM));
            m_completeLineSize = Math.Max(1, (m_parent.Height - cyHorzScrollbar) / (itemSize.Height + ThumbListRenderer.MARGIN_ITEM));

            // カーソルの位置を調整
            int fileCount = m_parent.FileList.Files.Count;
            if (fileCount == 0) {
                m_cursorScreenX = 0;
                m_cursorScreenY = 0;
                m_topLine = 0;
            } else {
                int targetLine = oldCursorPos / m_completeColumnSize;
                m_cursorScreenX = oldCursorPos % m_completeColumnSize;
                m_cursorScreenY = targetLine - m_topLine;
                int totalLineCount = Math.Max(1, (fileCount - 1) / m_completeColumnSize + 1);
                if (m_completeLineSize > totalLineCount) {
                    // １画面に収まる
                    m_topLine = 0;
                    m_cursorScreenY = oldCursorPos / m_completeColumnSize;
                } else if (m_cursorScreenY < 0) {
                    // 上にはみ出した
                    m_cursorScreenY = 0;
                    m_topLine = Math.Min(totalLineCount - m_completeLineSize, targetLine);
                } else if (m_cursorScreenY >= m_completeLineSize) {
                    // 下にはみ出した
                    m_topLine = Math.Max(0, Math.Min(totalLineCount - m_completeLineSize, targetLine - Math.Max(0, m_completeLineSize - 1)));
                    m_cursorScreenY = targetLine - m_topLine;
                } else if (m_topLine > totalLineCount - m_completeLineSize) {
                    // 行数が少なくなって許容値を超えた
                    m_topLine = totalLineCount - m_completeLineSize;
                    m_cursorScreenY = targetLine - m_topLine;
                }
            }

            // スクロールバーを調整
            SetHorizontalScrollbar();
            SetVerticalScrollbar();
        }

        //=========================================================================================
        // 機　能：水平スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetHorizontalScrollbar() {
            // スクロールバーを表示しない
            m_parent.HorizontalScroll.Visible = false;
        }

        //=========================================================================================
        // 機　能：垂直スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void SetVerticalScrollbar() {
            if (m_completeColumnSize == 0) {
                // 初期化前：スクロールバーを表示しない
                if (m_parent.VerticalScroll.Visible != false) {
                    m_parent.VerticalScroll.Visible = false;
                }
                return;
            }

            int lines = (m_parent.FileList.Files.Count - 1) / m_completeColumnSize + 1;
            if (lines <= m_completeLineSize || m_completeColumnSize == 0) {
                // スクロールバーを表示しない
                if (m_parent.VerticalScroll.Visible != false) {
                    m_parent.VerticalScroll.Visible = false;
                }
            } else {
                // スクロールバーを表示する
                if (m_parent.VerticalScroll.Enabled != true) {
                    m_parent.VerticalScroll.Enabled = true;
                }
                m_parent.VerticalScroll.Visible = false;
                m_parent.VerticalScroll.Visible = true;
                m_parent.VerticalScroll.Minimum = 0;
                m_parent.VerticalScroll.Maximum = lines - 1;
                m_parent.VerticalScroll.Value = m_topLine;
                m_parent.VerticalScroll.SmallChange = 1;
                m_parent.VerticalScroll.LargeChange = m_completeLineSize;
            }
        }

        //=========================================================================================
        // 機　能：ファイルのスプリットの位置が更新されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnSplitChanged() {
        }

        //=========================================================================================
        // 機　能：画面を描画する
        // 引　数：[in]gcs    グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public void OnPaint(Graphics gcs) {
            FileListGraphics g = new FileListGraphics(gcs, 0, m_fileLineRenderer.FileItemSizeDpiModified.Height);
            try {
                // ファイル一覧を描画
                g.Graphics.FillRectangle(g.FileListBackBrush, ScrollRectangle);
                if (m_completeColumnSize == 0) {
                    // 表示できないとき
                    ;
                } else if (m_parent.FileList.Files.Count == 0) {
                    // ファイルがないとき
                    DrawNoFile(g);
                } else {
                    // ファイルがあるとき
                    int extendLine = SystemInformation.HorizontalScrollBarHeight / m_fileLineRenderer.ImageRawSize.Height + 1;
                    int cursorFileScreenX = CursorLineNo % m_completeColumnSize;
                    int cursorFileScreenY = CursorLineNo / m_completeColumnSize - m_topLine;
                    for (int y = 0; y < m_completeLineSize + extendLine; y++) {
                        for (int x = 0; x < m_completeColumnSize; x++) {
                            bool withCursor = (x == cursorFileScreenX && y == cursorFileScreenY);
                            DrawFileListItem(g, x, y, withCursor, false);
                        }
                    }
                }
            } finally {
                g.Dispose();
            }

            // サムネイルの読み込みをリクエスト
            RequestThumbnailLoading();
        }

        //=========================================================================================
        // 機　能：サムネイルの読み込みリクエストを行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void RequestThumbnailLoading() {
            ThumbnailLoadManager loadTarget = new ThumbnailLoadManager(m_fileLineRenderer.ImageRawSize);
            FileCrawlerCreateThumbnailRequestParam param = loadTarget.CreateLoadRequest(m_parent.FileList, m_topLine, m_cursorScreenX, m_cursorScreenY, m_completeColumnSize, m_completeLineSize);
            if (param != null) {
                Program.Document.FileCrawlThread.RequestNewFileCrawl(m_parent.FileList, CrawlType.Thumbnail, param);
            }
        }

        //=========================================================================================
        // 機　能：ファイルがないときのメッセージを描画する
        // 引　数：[in]g   描画に使用するグラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawNoFile(FileListGraphics g) {
            // メッセージを描画
            string message;
            if (m_parent.FileList.LoadingStatus != FileListLoadingStatus.Loading) {
                message = Resources.FileListNoFiles;
            } else {
                message = Resources.FileListNowLoading;
            }
            Font font = g.FileListFont;
            Brush fontBrush = m_fileLineRenderer.GetTextDrawBrush(g, null, IsActiveDraw);
            SizeF messageSize = g.Graphics.MeasureString(message, font);
            int xPos = (int)((ScrollRectangle.Width - messageSize.Width) / 2);
            int yPos = (ScrollRectangle.Height - font.Height) / 2;
            g.Graphics.DrawString(message, font, fontBrush, xPos, yPos);

            // カーソルを描画
            if (m_parent.HasCursor) {
                Pen pen;
                if (Program.MainWindow.IsFileListViewActive) {
                    pen = g.FileListCursorPen;
                } else {
                    pen = g.FileListCursorDisablePen;
                }
                int cursorYPos = yPos + font.Height;
                g.Graphics.DrawLine(pen, new Point(0, cursorYPos), new Point(ScrollRectangle.Width, cursorYPos));
            }
        }

        //=========================================================================================
        // 機　能：画面を描画する
        // 引　数：[in]g          描画に使用するグラフィックス
        // 　　　　[in]scrColumn  描画する画面上の桁
        // 　　　　[in]scrLine    描画する画面上の行
        // 　　　　[in]withCursor カーソルを描画するときtrue
        // 　　　　[in]dblBuffer  ダブルバッファリングを行うときtrue
        // 戻り値：なし
        //=========================================================================================
        private void DrawFileListItem(FileListGraphics g, int scrColumn, int scrLine, bool withCursor, bool dblBuffer) {
            if (scrLine >= m_completeLineSize + 1) {
                return;
            }
            int idxTarget = (m_topLine + scrLine) * m_completeColumnSize + scrColumn;
            if (m_parent.FileList.Files.Count <= idxTarget) {
                return;
            }
            if (!m_parent.HasCursor) {
                withCursor = false;
            }

            Point pos = GetItemScreenPosition(g, scrColumn, scrLine);
            if (dblBuffer) {
                Bitmap bmpBuffer = new Bitmap(m_fileLineRenderer.FileItemSizeDpiModified.Width, m_fileLineRenderer.FileItemSizeDpiModified.Height);
                Graphics gBmp = Graphics.FromImage(bmpBuffer);
                FileListGraphics gDraw = new FileListGraphics(gBmp, 0, m_fileLineRenderer.FileItemSizeDpiModified.Height);
                try {
                    m_fileLineRenderer.DrawItem(gDraw, 0, 0, IsActiveDraw, m_parent.FileList.Files[idxTarget], withCursor);
                } finally {
                    gDraw.Dispose();
                    gBmp.Dispose();
                    g.Graphics.DrawImage(bmpBuffer, pos.X, pos.Y);
                    bmpBuffer.Dispose();
                }
            } else {
                m_fileLineRenderer.DrawItem(g, pos.X, pos.Y, IsActiveDraw, m_parent.FileList.Files[idxTarget], withCursor);
            }
        }

        //=========================================================================================
        // 機　能：項目の画面上での位置を返す
        // 引　数：[in]g          描画に使用するグラフィックス
        // 　　　　[in]scrColumn  描画する画面上の桁
        // 　　　　[in]scrLine    描画する画面上の行
        // 戻り値：項目の画面上での位置
        //=========================================================================================
        private Point GetItemScreenPosition(FileListGraphics g, int scrColumn, int scrLine) {
            // X方向を判定
            int xPos;
            if (m_completeColumnSize == 1) {
                xPos = 0;
            } else {
                int xSpace = (m_parent.ClientRectangle.Width - (m_fileLineRenderer.FileItemSizeDpiModified.Width + g.X(ThumbListRenderer.MARGIN_ITEM)) * m_completeColumnSize) / m_completeColumnSize;
                xPos = (m_fileLineRenderer.FileItemSizeDpiModified.Width + g.X(ThumbListRenderer.MARGIN_ITEM) + xSpace) * scrColumn;
            }

            // Y方向を判定
            int yPos = (m_fileLineRenderer.FileItemSizeDpiModified.Height + g.Y(ThumbListRenderer.MARGIN_ITEM)) * scrLine;
            
            return new Point(xPos, yPos);
        }

        //=========================================================================================
        // 機　能：１行の高さを返す
        // 引　数：[in]g          描画に使用するグラフィックス
        // 戻り値：１行の高さ
        //=========================================================================================
        private int GetLineHeight(FileListGraphics g) {
            int height = m_fileLineRenderer.FileItemSizeDpiModified.Height + g.Y(ThumbListRenderer.MARGIN_ITEM);
            return height;
        }

        //=========================================================================================
        // 機　能：画面上の位置から画面上の項目のインデックスを返す
        // 引　数：[in]scrPos     調べる画面上の位置
        // 　　　　[out]index     調べたインデックスを返す変数（画面上のインデックス、件数は無視）
        // 　　　　[out]hit       領域内にヒットしたときtrueを返す変数
        // 　　　　[out]centerHit 領域の中心にヒットしたときtrueを返す変数
        // 戻り値：項目の画面上での位置
        //=========================================================================================
        private void GetScreenItemIndexFromPosition(Point scrPos, out Point index, out bool hit, out bool centerHit) {
            using (HighDpiGraphics g = new HighDpiGraphics(m_parent)) {
                hit = true;
                centerHit = true;

                // X方向を判定
                int xIndex;
                int xItemStart;
                int cxItem = m_fileLineRenderer.FileItemSizeDpiModified.Width;
                int cxImage = g.X(m_fileLineRenderer.ImageRawSize.Width);
                if (m_completeColumnSize == 1) {
                    xIndex = 0;
                    xItemStart = 0;
                } else {
                    int xSpace = (m_parent.ClientRectangle.Width - (cxItem + g.X(ThumbListRenderer.MARGIN_ITEM)) * m_completeColumnSize) / m_completeColumnSize;
                    xIndex = scrPos.X / (cxItem + g.X(ThumbListRenderer.MARGIN_ITEM) + xSpace);
                    xItemStart = (cxItem + g.X(ThumbListRenderer.MARGIN_ITEM) + xSpace) * xIndex;
                    if (xIndex >= m_completeColumnSize) {
                        xIndex = m_completeColumnSize - 1;
                        hit = false;
                        centerHit = false;
                    }
                }
                if (scrPos.X < xItemStart || scrPos.X > xItemStart + cxItem) {
                    hit = false;
                }
                if (scrPos.X < xItemStart + cxImage / 3 || scrPos.X > xItemStart + cxImage * 2 / 3) {
                    centerHit = false;
                }

                // Y方向を判定
                int cyItem = m_fileLineRenderer.FileItemSizeDpiModified.Height;
                int cyImage = g.Y(m_fileLineRenderer.ImageRawSize.Height);
                int yIndex = scrPos.Y / (cyItem + g.Y(ThumbListRenderer.MARGIN_ITEM));
                if (scrPos.Y < 0) {
                    yIndex--;
                }
                int yItemStart = (cyItem + g.Y(ThumbListRenderer.MARGIN_ITEM)) * yIndex;
                if (scrPos.Y < yItemStart || scrPos.Y > yItemStart + cyItem) {
                    hit = false;
                }
                if (scrPos.Y < yItemStart + cyImage / 3 || scrPos.Y > yItemStart + cyImage * 2 / 3) {
                    centerHit = false;
                }

                // 結果を返す
                index = new Point(xIndex, yIndex);
            }
        }

        //=========================================================================================
        // 機　能：項目の画面上での大きさを返す
        // 引　数：[in]columns  項目の横方向の個数
        // 　　　　[in]lines    項目の縦方向の個数
        // 戻り値：項目の画面上での大きさ
        //=========================================================================================
        private Size GetScreenItemRegion(int columns, int lines) {
            Size sizeItem = m_fileLineRenderer.FileItemSizeDpiModified;
            int cx = (sizeItem.Width + ThumbListRenderer.MARGIN_ITEM) * columns;
            int cy = (sizeItem.Height + ThumbListRenderer.MARGIN_ITEM) * lines;
            Size size = new Size(cx, cy);
            return size;
        }

        //=========================================================================================
        // 機　能：アイコンを描画する
        // 引　数：[in]index  描画するファイルのインデックス
        // 戻り値：なし
        //=========================================================================================
        public void DrawFileIcon(int index) {
            FileListGraphics g = CreateFileListGraphics();
            try {
                int itemX = index % m_completeColumnSize;
                int itemY = index / m_completeColumnSize;
                Point pos = GetItemScreenPosition(g, itemX, itemY - m_topLine);
                Bitmap bmpBuffer = new Bitmap(m_fileLineRenderer.FileItemSizeDpiModified.Width, m_fileLineRenderer.FileItemSizeDpiModified.Height);
                Graphics gBmp = Graphics.FromImage(bmpBuffer);
                FileListGraphics gDraw = new FileListGraphics(gBmp, 0, m_fileLineRenderer.FileItemSizeDpiModified.Height);
                try {
                    bool withCursor = (m_parent.HasCursor && m_cursorScreenX == itemX && m_cursorScreenY + m_topLine == itemY);
                    m_fileLineRenderer.DrawItem(gDraw, 0, 0, IsActiveDraw, m_parent.FileList.Files[index], withCursor);
                } finally {
                    gDraw.Dispose();
                    gBmp.Dispose();
                    g.Graphics.DrawImage(bmpBuffer, pos.X, pos.Y);
                    bmpBuffer.Dispose();
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
            int lineCount = (fileList.Files.Count - 1) / m_completeColumnSize + 1;
            if (newPosition > lineCount - m_completeLineSize) {
                newPosition = lineCount - m_completeLineSize;
            }
            if (lineCount <= m_completeLineSize) {
                return;
            }

            FileListGraphics g = CreateFileListGraphics();
            try {
                // 移動に応じてスクロール
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                int dy = newPosition - m_topLine;
                if (dy > 0) {
                    // 下に移動
                    m_cursorScreenY = Math.Max(0, m_cursorScreenY - dy);
                    int scrollPixels = GetScreenItemRegion(0, dy).Height;
                    Win32API.Win32ScrollWindow(m_parent.Handle, 0, -scrollPixels, ScrollRectangle, ScrollRectangle);
                    m_topLine = newPosition;
                } else if (dy < 0) {
                    // 上に移動
                    m_cursorScreenY = Math.Min(m_completeLineSize - 1, m_cursorScreenY - dy);
                    int scrollPixels = GetScreenItemRegion(0, -dy).Height;
                    Win32API.Win32ScrollWindow(m_parent.Handle, 0, scrollPixels, ScrollRectangle, ScrollRectangle);
                    m_topLine = newPosition;
                }
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
            } finally {
                g.Dispose();
            }

            // スクロールバーを設定
            m_parent.VerticalScroll.Value = m_topLine;
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
        }

        //=========================================================================================
        // 機　能：マウスホイールイベントを処理する
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
        // 機　能：機能を実行する
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
            Point scrIndex;
            bool hit;
            bool centerHit;
            GetScreenItemIndexFromPosition(cursorPos, out scrIndex, out hit, out centerHit);
            int line = (scrIndex.Y + m_topLine) * m_completeColumnSize + scrIndex.X;
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
                m_explorerSelectStartIndex = -1;
            } else if (markCmd.MarkAction == MouseMarkAction.MarkFirstOnly) {
                // はじめのファイルだけをマーク
                FileListGraphics g = CreateFileListGraphics();
                try {
                    if (0 <= line && line <= fileList.Files.Count - 1) {
                        if ((m_cursorScreenY + m_topLine) * m_completeColumnSize + m_cursorScreenX != line) {
                            MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                            MarkOneFile(g, line, MarkOperation.Mark, false);
                        } else if (fileList.Files[line].Marked) {
                            MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                        } else {
                            MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                            MarkOneFile(g, line, MarkOperation.Mark, false);
                        }
                    }
                } finally {
                    g.Dispose();
                }
                markCmd.MarkAction = MouseMarkAction.None;
                m_explorerSelectStartIndex = line;
            } else if (markCmd.MarkAction == MouseMarkAction.ExplorerMark) {
                // エクスプローラ風にマーク
                FileListGraphics g = CreateFileListGraphics();
                try {
                    if (m_explorerSelectStartIndex == -1) {
                        // 初回選択時
                        if (0 <= line && line <= fileList.Files.Count - 1) {
                            MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                            MarkOneFile(g, line, MarkOperation.Mark, false);
                        }
                        m_explorerSelectStartIndex = line;
                    } else {
                        // 前回選択との間をマーク
                        int start = Math.Min(m_explorerSelectStartIndex, line);
                        int end = Math.Max(m_explorerSelectStartIndex, line);
                        MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                        for (int i = start; i <= end; i++) {
                            MarkOneFile(g, i, MarkOperation.Mark, false);
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
            int fileCount = m_parent.FileList.Files.Count;
            int lineCount = (fileCount - 1) / m_completeColumnSize + 1;

            // 画面上のカーソル位置に移動
            Point cursorPos = m_parent.PointToClient(Cursor.Position);
            Point scrIndex;
            bool hit;
            bool centerHit;
            GetScreenItemIndexFromPosition(cursorPos, out scrIndex, out hit, out centerHit);
            int newIndex = Math.Min(fileCount - 1, Math.Max(0, (scrIndex.Y + m_topLine) * m_completeColumnSize + scrIndex.X));
            int newCursorX = newIndex % m_completeColumnSize;
            int newCursorY = newIndex / m_completeColumnSize - m_topLine;
            if (newCursorX != scrIndex.X) {
                if ((newCursorY - 1 + m_topLine) * m_completeColumnSize + scrIndex.X > 0) {
                    newCursorX = scrIndex.X;
                    newCursorY--;
                } else if ((newCursorY + 1 + m_topLine) * m_completeColumnSize + scrIndex.X < fileCount) {
                    newCursorX = scrIndex.X;
                    newCursorY++;
                }
            }

            FileListGraphics g = CreateFileListGraphics();
            try {
                if (scrIndex.Y < 0) {
                    // 画面外上
                    if (m_cursorScreenY == 0) {
                        // 上にスクロール
                        if (m_topLine > 0) {
                            DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                            int scrollLine = Math.Min(Math.Min(-scrIndex.Y, Configuration.Current.FileListDragMaxSpeed), m_topLine);
                            Win32API.Win32ScrollWindow(m_parent.Handle, 0, scrollLine * GetLineHeight(g), ScrollRectangle, ScrollRectangle);
                            m_cursorScreenX = newCursorX;
                            m_cursorScreenY = 0;
                            m_topLine -= scrollLine;
                            DragMark(g, markCmd);
                            DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
                            m_parent.VerticalScroll.Value = m_topLine;
                            System.Threading.Thread.Sleep(30);
                        }
                    } else {
                        // カーソルを一番上へ
                        DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                        m_cursorScreenX = newCursorX;
                        m_cursorScreenY = 0;
                        DragMark(g, markCmd);
                        DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
                    }
                } else if (scrIndex.Y >= m_completeLineSize) {
                    // 画面外下
                    if (m_cursorScreenY == m_completeLineSize - 1) {
                        // 下にスクロール
                        if (m_topLine < fileCount - m_completeLineSize) {
                            int scrollLine = Math.Min(Math.Min(scrIndex.Y - m_completeLineSize + 1, Configuration.Current.FileListDragMaxSpeed), lineCount - m_topLine - m_completeLineSize);
                            DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                            Win32API.Win32ScrollWindow(m_parent.Handle, 0, -scrollLine * GetLineHeight(g), ScrollRectangle, ScrollRectangle);
                            m_cursorScreenX = newCursorX;
                            m_cursorScreenY = m_completeLineSize - 1;
                            m_topLine += scrollLine;
                            DragMark(g, markCmd);
                            DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
                            m_parent.VerticalScroll.Value = m_topLine;
                            System.Threading.Thread.Sleep(30);
                        }
                    } else {
                        // カーソルを一番下へ
                        DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                        m_cursorScreenX = newCursorX;
                        m_cursorScreenY = Math.Min(lineCount - m_topLine - 1, Math.Max(0, m_completeLineSize - 1));
                        DragMark(g, markCmd);
                        DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
                    }
                } else {
                    // 画面内
			        if (m_cursorScreenX != newCursorX || m_cursorScreenY != newCursorY) {
                        DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                        m_cursorScreenX = newCursorX;
                        m_cursorScreenY = newCursorY;
                        DragMark(g, markCmd);
                        DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
			        } else if (markCmd.MarkOperation != MarkOperation.None) {
                        DragMark(g, markCmd);
                    }
                }
            } finally {
                g.Dispose();
            }
            markCmd.LastMouseFilePos = (m_cursorScreenY + m_topLine) * m_completeColumnSize + m_cursorScreenX;

            // UIをリフレッシュ
            int newMarkCount = m_parent.FileList.MarkedDirectoryCount + m_parent.FileList.MarkedFileCount;
            long newMarkSize = m_parent.FileList.MarkedFileSize;
            if (oldMarkCount != newMarkCount || oldMarkSize != newMarkSize) {
                m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                Program.MainWindow.RefreshUIStatus();
            }
        }

        //=========================================================================================
        // 機　能：ドラッグ中のマーク処理を行う
        // 引　数：[in]g        マーク処理の描画に使用するグラフィックス
        // 　　　　[in]markCmd  マーク処理中のコマンド
        // 戻り値：なし
        //=========================================================================================
        private void DragMark(FileListGraphics g, AbstractMouseMarkCommand markCmd) {
            int index1 = (m_cursorScreenY + m_topLine) * m_completeColumnSize + m_cursorScreenX;
            int index2 = markCmd.LastMouseFilePos;
            int startIndex = Math.Min(index1, index2);
            int endIndex = Math.Max(index1, index2);
            for (int i = startIndex; i <= endIndex; i++) {
                MarkOneFile(g, i, markCmd.MarkOperation, false);
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
        // 機　能：カーソルの位置をマウスの直下に移動する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void MoveCursorBelowMouse() {
            int fileCount = m_parent.FileList.Files.Count;
            if (fileCount == 0) {
                return;
            }
            Point mousePos = m_parent.PointToClient(Cursor.Position);
            Point scrIndex;
            bool hit;
            bool centerHit;
            GetScreenItemIndexFromPosition(mousePos, out scrIndex, out hit, out centerHit);
            int line = (scrIndex.Y + m_topLine) * m_completeColumnSize + scrIndex.X;
            if (!hit || line < 0 || line > fileCount) {
                return;
            }
            int scrX = line % m_completeColumnSize;
            int scrY = line / m_completeColumnSize - m_topLine;
            
            line = Math.Max(0, Math.Min(fileCount - 1 - m_topLine, line));
	        if (m_cursorScreenX != scrX || m_cursorScreenY != scrY) {
                FileListGraphics g = CreateFileListGraphics();
                try {
                    DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                    m_cursorScreenX = scrX;
                    m_cursorScreenY = scrY;
                    DrawCursor(g, scrX, scrY, true);
                } finally {
                    g.Dispose();
                }
            }
        }

        //=========================================================================================
        // 機　能：ドラッグ開始のマウス位置かどうかを調べる
        // 引　数：[in]mouseX  マウスのX位置
        // 　　　　[in]mouseY  マウスのY位置
        // 戻り値：ドラッグ開始位置のときtrue
        //=========================================================================================
        public bool CheckDragStartPosition(int mouseX, int mouseY) {
            Point mousePos = new Point(mouseX, mouseY);
            Point scrIndex;
            bool hit;
            bool centerHit;
            GetScreenItemIndexFromPosition(mousePos, out scrIndex, out hit, out centerHit);
            if (hit && centerHit) {
                int index = (scrIndex.Y + m_topLine) * m_completeColumnSize + scrIndex.X;
                if (index < m_parent.FileList.Files.Count) {
                    return true;
                } else {
                    return false;
                }
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
            Point mousePos = new Point(mouseX, mouseY);
            Point scrIndex;
            bool hit;
            bool centerHit;
            GetScreenItemIndexFromPosition(mousePos, out scrIndex, out hit, out centerHit);
            if (hit) {
                int index = (scrIndex.Y + m_topLine) * m_completeColumnSize + scrIndex.X;
                FileListGraphics g = CreateFileListGraphics();
                try {
                    MarkOneFile(g, index, MarkOperation.Mark, true);
                } finally {
                    g.Dispose();
                }
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
        // 機　能：グラフィックスを作成する
        // 引　数：なし
        // 戻り値：グラフィックス
        //=========================================================================================
        private FileListGraphics CreateFileListGraphics() {
            return new FileListGraphics(m_parent, 0, m_fileLineRenderer.FileItemSizeDpiModified.Height);
        }

        //=========================================================================================
        // 機　能：カーソルを描画する
        // 引　数：[in]g           描画に使用するグラフィックス
        // 　　　　[in]screenX     画面のX座標
        // 　　　　[in]screenY     画面のY座標
        // 　　　　[in]withCursor  カーソル付きで描画するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void DrawCursor(FileListGraphics g, int screenX, int screenY, bool withCursor) {
            DrawFileListItem(g, screenX, screenY, withCursor, true);
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

            int newCursorIndex;
            FileListGraphics g = CreateFileListGraphics();
            try {
                // マーク状態を変更
                int oldCursorIndex = (m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX;
                int markCursorIndex = -1;
                if (withMark == MarkOperation.Clear) {
                    if (m_explorerSelectStartIndex != -1) {
                        MarkAllFile(MarkAllFileMode.ClearAll, true, null);
                        m_explorerSelectStartIndex = -1;
                    }
                } else if (withMark == MarkOperation.Mark) {
                    if (m_explorerSelectStartIndex == -1) {
                        m_explorerSelectStartIndex = oldCursorIndex;
                        MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                        MarkOneFile(g, oldCursorIndex, MarkOperation.Mark, false);
                    }
                    markCursorIndex = oldCursorIndex;
                }

                // 移動する
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                if (m_topLine == 0) {
                    // 最後の数行
                    m_cursorScreenY = Math.Max(0, m_cursorScreenY - lines);
                } else if (m_cursorScreenY - lines < 0) {
                    // スクロールも伴う
                    int newTopLine = Math.Max(0, m_topLine - (lines - m_cursorScreenY));
                    m_cursorScreenY = 0;
                    if (m_topLine != newTopLine) {
                        int scrollPixels = GetScreenItemRegion(0, m_topLine - newTopLine).Height;
                        m_topLine = newTopLine;
                        Win32API.Win32ScrollWindow(m_parent.Handle, 0, scrollPixels, ScrollRectangle, ScrollRectangle);
                    }
                } else {
                    // 画面内
                    m_cursorScreenY -= lines;
                }
                newCursorIndex = (m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX;

                // マークする
                if (markCursorIndex != -1 && newCursorIndex != oldCursorIndex) {
                    if (m_explorerSelectStartIndex < oldCursorIndex) {
                        if (newCursorIndex <= m_explorerSelectStartIndex) {
                            for (int i = newCursorIndex; i <= m_explorerSelectStartIndex; i++) {
                                MarkOneFile(g, i, MarkOperation.Mark, false);
                            }
                            for (int i = m_explorerSelectStartIndex + 1; i <= oldCursorIndex; i++) {
                                MarkOneFile(g, i, MarkOperation.Clear, false);
                            }
                        } else {
                            for (int i = newCursorIndex + 1; i <= oldCursorIndex; i++) {
                                MarkOneFile(g, i, MarkOperation.Clear, false);
                            }
                        }
                    } else {
                        for (int i = newCursorIndex; i <= oldCursorIndex; i++) {
                            MarkOneFile(g, i, MarkOperation.Mark, false);
                        }
                    }
                    // UIをリフレッシュ
                    m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                    Program.MainWindow.RefreshUIStatus();
                }

                // カーソルを描画
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
            } finally {
                g.Dispose();
            }

            // スクロールバーを再設定
            SetVerticalScrollbar();

            return newCursorIndex;
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

            int newCursorIndex;
            FileListGraphics g = CreateFileListGraphics();
            try {
                // マーク状態を変更
                int oldCursorIndex = (m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX;
                int markCursorIndex = -1;
                if (withMark == MarkOperation.Clear) {
                    if (m_explorerSelectStartIndex != -1) {
                        MarkAllFile(MarkAllFileMode.ClearAll, true, null);
                        m_explorerSelectStartIndex = -1;
                    }
                } else if (withMark == MarkOperation.Mark) {
                    if (m_explorerSelectStartIndex == -1) {
                        m_explorerSelectStartIndex = oldCursorIndex;
                        MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                        MarkOneFile(g, oldCursorIndex, MarkOperation.Mark, false);
                    }
                    markCursorIndex = oldCursorIndex;
                }

                // 移動する
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                int finalTopLine = Math.Max(0, (fileCount - 1) / m_completeColumnSize + 1 - m_completeLineSize);
                if (m_topLine >= finalTopLine) {
                    // 最後の数行または常にスクロールなし
                    m_cursorScreenY = Math.Min((fileCount - 1) / m_completeColumnSize - finalTopLine, m_cursorScreenY + lines);
                } else if (m_cursorScreenY + lines >= m_completeLineSize) {
                    // スクロールも伴う
                    int newTopLine = Math.Min(finalTopLine, m_topLine + lines - (m_completeLineSize - 1 - m_cursorScreenY));
                    m_cursorScreenY = m_completeLineSize - 1;
                    if (m_topLine != newTopLine) {
                        int scrollPixels = GetScreenItemRegion(0, newTopLine - m_topLine).Height;
                        m_topLine = newTopLine;
                        Win32API.Win32ScrollWindow(m_parent.Handle, 0, -scrollPixels, ScrollRectangle, ScrollRectangle);
                    }
                } else {
                    // 画面内
                    m_cursorScreenY += lines;
                }
                newCursorIndex = (m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX;
                if (newCursorIndex >= fileCount) {
                    m_cursorScreenX = (fileCount - 1) % m_completeColumnSize;
                    m_cursorScreenY = (fileCount - 1) / m_completeColumnSize - m_topLine;
                    newCursorIndex = fileCount - 1;
                }

                // マークする
                if (markCursorIndex != -1 && newCursorIndex != oldCursorIndex) {
                    if (oldCursorIndex < m_explorerSelectStartIndex) {
                        if (m_explorerSelectStartIndex <= newCursorIndex) {
                            for (int i = oldCursorIndex; i <= m_explorerSelectStartIndex - 1; i++) {
                                MarkOneFile(g, i, MarkOperation.Clear, false);
                            }
                            for (int i = m_explorerSelectStartIndex; i <= newCursorIndex; i++) {
                                MarkOneFile(g, i, MarkOperation.Mark, false);
                            }
                        } else {
                            for (int i = oldCursorIndex; i <= newCursorIndex - 1; i++) {
                                MarkOneFile(g, i, MarkOperation.Clear, false);
                            }
                        }
                    } else {
                        for (int i = oldCursorIndex; i <= newCursorIndex; i++) {
                            MarkOneFile(g, i, MarkOperation.Mark, false);
                        }
                    }

                    // UIをリフレッシュ
                    m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                    Program.MainWindow.RefreshUIStatus();
                }

                // カーソルを描画
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
            } finally {
                g.Dispose();
            }

            // スクロールバーを再設定
            SetVerticalScrollbar();

            return newCursorIndex;
        }
        
        //=========================================================================================
        // 機　能：カーソルを左に移動する
        // 引　数：[in]toggleWin  ウィンドウの切り替えを行うときtrue
        // 　　　　[in]withMark   マーク付きで移動するかどうか（Revert以外が有効）
        // 戻り値：新しいカーソルの位置（ウィンドウ切り替えの操作を行ったとき-1）
        //=========================================================================================
        public int CursorLeft(bool toggleWin, MarkOperation withMark) {
            // 左ウィンドウへ
            int fileCount = m_parent.FileList.Files.Count;
            if (toggleWin || fileCount == 0 || m_cursorScreenX == 0) {
                FileListViewUtils.CursorLeft(m_parent);
                return -1;
            }

            // カーソルを移動
            int newCursorIndex;
            FileListGraphics g = CreateFileListGraphics();
            try {
                // マーク状態を変更
                int oldCursorIndex = (m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX;
                int markCursorIndex = -1;
                if (withMark == MarkOperation.Clear) {
                    if (m_explorerSelectStartIndex != -1) {
                        MarkAllFile(MarkAllFileMode.ClearAll, true, null);
                        m_explorerSelectStartIndex = -1;
                    }
                } else if (withMark == MarkOperation.Mark) {
                    if (m_explorerSelectStartIndex == -1) {
                        m_explorerSelectStartIndex = oldCursorIndex;
                        MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                        MarkOneFile(g, oldCursorIndex, MarkOperation.Mark, false);
                    }
                    markCursorIndex = oldCursorIndex;
                }

                // 移動する
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                m_cursorScreenX--;
                newCursorIndex = (m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX;

                // マークする
                if (markCursorIndex != -1 && newCursorIndex != oldCursorIndex) {
                    if (newCursorIndex < m_explorerSelectStartIndex) {
                        MarkOneFile(g, newCursorIndex, MarkOperation.Mark, false);
                    } else {
                        MarkOneFile(g, oldCursorIndex, MarkOperation.Clear, false);
                    }

                    // UIをリフレッシュ
                    m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                    Program.MainWindow.RefreshUIStatus();
                }

                // カーソルを描画
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
            } finally {
                g.Dispose();
            }

            // スクロールバーを再設定
            SetVerticalScrollbar();

            return newCursorIndex;
        }
        
        //=========================================================================================
        // 機　能：カーソルを右に移動する
        // 引　数：[in]toggleWin  ウィンドウの切り替えを行うときtrue
        // 　　　　[in]withMark   マーク付きで移動するかどうか（Revert以外が有効）
        // 戻り値：新しいカーソルの位置（ウィンドウ切り替えの操作を行ったとき-1）
        //=========================================================================================
        public int CursorRight(bool toggleWin, MarkOperation withMark) {
            // 右ウィンドウへ
            int fileCount = m_parent.FileList.Files.Count;
            int maxLine = (fileCount - 1) / m_completeColumnSize;
            int maxColumn = (fileCount - 1) % m_completeColumnSize;
            if (toggleWin || fileCount == 0 ||
                    (m_topLine + m_cursorScreenY != maxLine && m_cursorScreenX == m_completeColumnSize - 1) ||
                    (m_topLine + m_cursorScreenY == maxLine && m_cursorScreenX == maxColumn)) {
                FileListViewUtils.CursorRight(m_parent);
                return -1;
            }

            // カーソルを移動
            int newCursorIndex;
            FileListGraphics g = CreateFileListGraphics();
            try {
                // マーク状態を変更
                int oldCursorIndex = (m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX;
                int markCursorIndex = -1;
                if (withMark == MarkOperation.Clear) {
                    if (m_explorerSelectStartIndex != -1) {
                        MarkAllFile(MarkAllFileMode.ClearAll, true, null);
                        m_explorerSelectStartIndex = -1;
                    }
                } else if (withMark == MarkOperation.Mark) {
                    if (m_explorerSelectStartIndex == -1) {
                        m_explorerSelectStartIndex = oldCursorIndex;
                        MarkAllFile(MarkAllFileMode.ClearAll, false, null);
                        MarkOneFile(g, oldCursorIndex, MarkOperation.Mark, false);
                    }
                    markCursorIndex = oldCursorIndex;
                }

                // 移動する
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                m_cursorScreenX++;
                newCursorIndex = (m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX;

                // マークする
                if (markCursorIndex != -1 && newCursorIndex != oldCursorIndex) {
                    if (oldCursorIndex < m_explorerSelectStartIndex) {
                        MarkOneFile(g, oldCursorIndex, MarkOperation.Clear, false);
                    } else {
                        MarkOneFile(g, newCursorIndex, MarkOperation.Mark, false);
                    }

                    // UIをリフレッシュ
                    m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                    Program.MainWindow.RefreshUIStatus();
                }

                // カーソルを描画
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
            } finally {
                g.Dispose();
            }

            // スクロールバーを再設定
            SetVerticalScrollbar();

            return newCursorIndex;
        }

        //=========================================================================================
        // 機　能：カーソルを次のファイルに移動する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CursorNext() {
            FileListGraphics g = CreateFileListGraphics();
            try {
                CursorNextImpl(g);
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：カーソルを次のファイルに移動する（実装）
        // 引　数：[in]g   移動に使用するグラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void CursorNextImpl(FileListGraphics g) {
            int fileCount = m_parent.FileList.Files.Count;
            if (fileCount == 0 || CursorLineNo == fileCount - 1) {
                return;
            }
            if (m_cursorScreenX == m_completeColumnSize - 1) {
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                m_cursorScreenX = 0;
                CursorDown(1, MarkOperation.None);
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
            } else {
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);
                m_cursorScreenX++;
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
            }
            // スクロールバーを再設定
            SetVerticalScrollbar();
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
                marked = MarkOneFile(g, CursorLineNo, MarkOperation.Revert, true);
                if (stay) {
                    CursorNextImpl(g);
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
                        MarkOneFile(g, fileIndex, markOpr, false);
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
        // 機　能：指定された位置のファイルまたはディレクトリをマークする
        // 引　数：[in]g          グラフィックス
        // 　　　　[in]fileIndex  マーク状態を変えるファイルのインデックス
        // 　　　　[in]markOpr    マークに対する操作
        // 　　　　[in]updateUI   UIの更新も行うときtrue
        // 戻り値：新しいマークの状態
        //=========================================================================================
        private bool MarkOneFile(FileListGraphics g, int fileIndex, MarkOperation markOpr, bool updateUI) {
            // ファイルが存在するか確認
            UIFileList fileList = m_parent.FileList;
            if (fileIndex < 0 || fileIndex >= fileList.Files.Count) {
                return false;
            }
            // 親ディレクトリはマークしない
            UIFile file = fileList.Files[fileIndex];
            string fileName = file.FileName;
            if (fileName == "..") {
                return false;
            }
            // マークを実行
            bool prevMark = fileList.Files[fileIndex].Marked;
            if (markOpr == MarkOperation.Mark) {
                fileList.SetMarked(fileIndex, true);
            } else if (markOpr == MarkOperation.Clear) {
                fileList.SetMarked(fileIndex, false);
            } else if (markOpr == MarkOperation.Revert) {
                fileList.SetMarked(fileIndex, !fileList.Files[fileIndex].Marked);
            } else if (markOpr == MarkOperation.None) {
                return fileList.Files[fileIndex].Marked;
            }

            // UIに反映
            if (prevMark != fileList.Files[fileIndex].Marked) {
                int screenX = fileIndex % m_completeColumnSize;
                int screenY = fileIndex / m_completeColumnSize - m_topLine;
                if (m_cursorScreenX == screenX && m_cursorScreenY == screenY) {
                    DrawCursor(g, screenX, screenY, true);
                } else {
                    DrawFileListItem(g, screenX, screenY, false, true);
                }
            }

            if (updateUI) {
                // ステータスバーをリフレッシュ
                m_parent.ParentPanel.StatusBar.RefreshMarkInfo();
                Program.MainWindow.RefreshUIStatus();
            }

            return fileList.Files[fileIndex].Marked;
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
                    MarkOneFile(g, CursorLineNo, MarkOperation.Revert, true);
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
            int newIndexX = index % m_completeColumnSize;
            int newIndexY = index / m_completeColumnSize;
            if (newIndexX == m_cursorScreenX && newIndexY == m_topLine + m_cursorScreenY) {
                return;
            }
            if (index < 0 || index >= m_parent.FileList.Files.Count) {
                return;
            }

            // 位置を移動
            FileListGraphics g = CreateFileListGraphics();
            try {
                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, false);

                // 新しい位置を計算
                int oldTopLine = m_topLine;
                int center = m_completeLineSize / 2;
	            if (newIndexY < center) {
		            m_topLine = 0;
                    m_cursorScreenX = newIndexX;
		            m_cursorScreenY = newIndexY;
	            } else if(newIndexY > m_parent.FileList.Files.Count - 1 - center) {
		            m_topLine = m_parent.FileList.Files.Count - m_completeLineSize;
                    m_cursorScreenX = newIndexX;
		            m_cursorScreenY = newIndexY - m_topLine;
	            } else {
                    m_cursorScreenX = newIndexX;
		            m_cursorScreenY = center;
		            m_topLine = newIndexY - m_cursorScreenY;
	            }
	            if (m_topLine < 0) {
		            m_topLine = 0;
		            m_cursorScreenY = newIndexY;
	            }

                // 移動
                if (oldTopLine != m_topLine) {
                    int scrollY = (oldTopLine - m_topLine) * GetLineHeight(g);
                    if (scrollY < m_parent.ClientRectangle.Height) {
                        Win32API.Win32ScrollWindow(m_parent.Handle, 0, scrollY, ScrollRectangle, ScrollRectangle);
                        m_parent.Update();
                    } else {
                        m_parent.Invalidate();
                        m_parent.Update();
                    }
                }

                DrawCursor(g, m_cursorScreenX, m_cursorScreenY, true);
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
            const int X_POS_MENU_ITEM = -10;
            const int X_POS_MENU_TOP = 50;
            Point ptMenu = Point.Empty;
            switch (menuPos) {
                case ContextMenuPosition.OnFile: {
                    using (FileListGraphics g = CreateFileListGraphics()) {
                        Point pt = GetItemScreenPosition(g, m_cursorScreenX, m_cursorScreenY);
                        int yPos = pt.Y + m_fileLineRenderer.ImageRawSize.Height / 2;
                        ptMenu = m_parent.PointToScreen(new Point(pt.X + m_fileLineRenderer.ImageRawSize.Width + X_POS_MENU_ITEM, yPos));
                    }
                    break;
                }
                case ContextMenuPosition.FileListTop: {
                    ptMenu = m_parent.PointToScreen(new Point(X_POS_MENU_TOP, 0));
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
        // 機　能：状態一覧パネルのアクティブ状態を設定する
        // 引　数：[in]isActive  状態一覧パネルがアクティブになったときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnActivateStateListPanel(bool isActive) {
            // ヘッダコントロール用なのでDefault一覧以外では何もしない
        }
        
        //=========================================================================================
        // プロパティ：アクティブ状態で描画するときtrue
        //=========================================================================================
        private bool IsActiveDraw {
            get {
                if (m_parent.HasCursor && Program.MainWindow.IsFileListViewActive) {
                    return true;
                } else {
                    return false;
                }
            }
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
        // プロパティ：カーソル行（先頭行=0、1,2,3,…Files.Length-1）
        //=========================================================================================
        public int CursorLineNo {
            get {
                return (m_topLine + m_cursorScreenY) * m_completeColumnSize + m_cursorScreenX;
            }
        }

        //=========================================================================================
        // プロパティ：水平スクロールバーの位置
        //=========================================================================================
        public int HorzScrollPosition {
            get {
                return 0;
            }
        }

        //=========================================================================================
        // プロパティ：スクロール範囲の長方形
        //=========================================================================================
        public Rectangle ScrollRectangle {
            get {
                return m_parent.ClientRectangle;
            }
        }
    }
}
