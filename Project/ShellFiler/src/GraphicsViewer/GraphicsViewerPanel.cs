using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.GraphicsViewer;
using ShellFiler.Command.GraphicsViewer.View;
using ShellFiler.GraphicsViewer.Filter;
using ShellFiler.Util;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.Locale;

namespace ShellFiler.GraphicsViewer {

    //=========================================================================================
    // クラス：グラフィックビューアの親となるパネル
    //=========================================================================================
    public partial class GraphicsViewerPanel : Panel, IUICommandTarget, ITwoStrokeKeyForm {
        // ドラッグ中のマウス位置のnull状態
        private static readonly Point MOUSE_POS_NULL = new Point(int.MaxValue, int.MaxValue);

        // グラフィックビューアのフォーム
        private GraphicsViewerForm m_form;

        // ステータスバー
        private GraphicsViewerStatusBar m_statusBar;

        // イメージの表示コンポーネント（Dispose後はnull）
        private ImageView m_imageView;

        // 使用するフィルター
        private GraphicsViewerFilterSetting m_filterSetting;

        // スライドショーの画像一覧
        private ISlideShowList m_slideShowList;

        // 読み込み対象のファイルシステム（クリップボードビューアのときnull）
        private IFileSystem m_targetFileSystem;

        // 読み込み対象のファイル一覧のコンテキスト情報
        private IFileListContext m_targetFileListContext;

        // 読み込み対象のパス（クリップボードビューアのときnull）
        private string m_targetBasePath;

        // グラフィックビューアの表示色
        private GraphicsViewerColor m_graphicViewerColor;

        // ドラッグ関連
        // ドラッグ中の加速に使用するタイマー（ドラッグ前、または、惰性終了後はnull）
        private System.Windows.Forms.Timer m_dragAccelerationTimer = null;

        // ドラッグに対する加速度
        private DraggingMouseAccelaration m_dragAccelaration = null;

        // ドラッグ中の移動量の履歴（ドラッグ中でないときはnull）
        private List<DraggingMousePosition> m_dragMousePosHistory = null;

        // ドラッグ中の前回マウス位置（ドラッグ中でないときはMOUSE_POS_NULL）
        private Point m_dragLastMousePos;

        // ドラッグ中にマウスが移動した量
        private long m_totalMoveDistance = 0;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerPanel() {
            InitializeComponent();
            if (Configuration.Current.GraphicsViewerFilterMode == GraphicsViewerFilterMode.AllImages) {
                m_filterSetting = Program.Document.UserGeneralSetting.GraphicsViewerImageFilter;
            } else {
                m_filterSetting = new GraphicsViewerFilterSetting();
            }
            m_imageView = new ImageView(this, m_filterSetting);

            // ダブルバッファリング有効
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]form      グラフィックビューアのフォーム
        // 　　　　[in]statusBar ステータスバー
        // 　　　　[in]gvParam   グラフィックビューアの起動パラメータ
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(GraphicsViewerForm form, GraphicsViewerStatusBar statusBar, GraphicsViewerParameter gvParam) {
            m_form = form;
            m_statusBar = statusBar;
            if (gvParam.GraphicsViewerMode == GraphicsViewerMode.ClipboardViewer) {
                m_slideShowList = new SlideShowClipboard(gvParam);
            } else {
                m_slideShowList = new SlideShowFolderList(gvParam);
            }
            m_imageView.Initialize();
            m_imageView.SetImage(m_slideShowList.CurrentImage, false, false);
            form.MouseWheel += new MouseEventHandler(GraphicsViewerPanel_MouseWheel);
            ResetColor();

            if (gvParam.GraphicsViewerMode == GraphicsViewerMode.GraphicsViewer) {
                m_targetFileSystem = gvParam.ForGraphicsViewer.FileSystem;
                m_targetFileListContext = gvParam.ForGraphicsViewer.FileListContext;
                m_targetBasePath = gvParam.ForGraphicsViewer.BasePath;
                GraphicsViewerImageLoadRequest request = new GraphicsViewerImageLoadRequest(m_targetFileSystem, m_targetFileListContext, m_targetBasePath, m_slideShowList.CurrentImage, true, m_form);
                Program.Document.BackgroundTaskManager.GraphicsViewerTaskManager.RequestLoadImage(request);
            } else if (gvParam.GraphicsViewerMode == GraphicsViewerMode.SlideShow) {
                m_targetFileSystem = gvParam.ForGraphicsViewer.FileSystem;
                m_targetFileListContext = gvParam.ForGraphicsViewer.FileListContext;
                m_targetBasePath = gvParam.ForGraphicsViewer.BasePath;
                GraphicsViewerImageLoadRequest request = new GraphicsViewerImageLoadRequest(m_targetFileSystem, m_targetFileListContext, m_targetBasePath, m_slideShowList.CurrentImage, true, m_form);
                Program.Document.BackgroundTaskManager.GraphicsViewerTaskManager.RequestLoadImage(request);
                RequestSlideShowNext(true);
            } else {
                m_targetFileSystem = null;
                m_targetFileListContext = null;
                m_targetBasePath = null;
            }
        }

        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisposeComponent() {
            m_imageView.DisposeComponent();
            m_imageView = null;
            m_slideShowList.DisposeAllImage();
            if (m_dragAccelerationTimer != null) {
                m_dragAccelerationTimer.Stop();
                m_dragAccelerationTimer = null;
            }
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
            }
            base.WndProc(ref message);
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
            if (m_imageView != null) {
                m_imageView.OnHScroll(evt);
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
            if (m_imageView != null) {
                m_imageView.OnVScroll(evt);
            }
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]evt   送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnKeyDown(KeyCommand evt) {
            Keys key = evt.KeyCode;
            GraphicsViewerActionCommand command = Program.Document.CommandFactory.CreateGraphicsViewerCommandFromKeyInput(evt, this);
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
            GraphicsViewerActionCommand command = Program.Document.CommandFactory.CreateGraphicsViewerCommandFromUICommand(item, this);
            if (command != null) {
                command.Execute();
            }
        }

        //=========================================================================================
        // 機　能：ファイルの読み込みの結果を通知する
        // 引　数：[in]imageInfo  読み込んだ画像の情報
        // 戻り値：なし
        //=========================================================================================
        public void NotifyFileLoad(ImageInfo imageInfo) {
            m_slideShowList.NotifyFileLoad(imageInfo);
            if (imageInfo.FilePath == m_imageView.CurrentImage.FilePath) {
                m_imageView.SetImage(m_slideShowList.CurrentImage, false, true);
                m_statusBar.RefreshStatusBar();
            }
        }

        //=========================================================================================
        // 機　能：スクロールする
        // 引　数：[in]dxPixel  左に移動するピクセル数
        // 　　　　[in]dyPixel  下に移動するピクセル数
        // 戻り値：なし
        //=========================================================================================
        public void ScrollView(int dxPixel, int dyPixel) {
            if (ImageAvailable) {
                m_imageView.ScrollView(dxPixel, dyPixel);
            }
        }

        //=========================================================================================
        // 機　能：中心にスクロールする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ScrollCenter() {
            if (ImageAvailable) {
                m_imageView.ScrollCenter();
            }
        }

        //=========================================================================================
        // 機　能：マウスホイールが回転したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerPanel_MouseWheel(object sender, MouseEventArgs evt) {
            if (!ImageAvailable) {
                return;
            }
            ChangeZoomRatioByZoomKeyDelta(evt.Delta / 5, new ClassPoint(evt.X - this.Left, evt.Y - this.Top));
        }

        //=========================================================================================
        // 機　能：拡大率を変更する
        // 引　数：[in]zoomKeyDelta  拡大率のキー（拡大するときはプラス方向）
        // 　　　　[in]mousePos      マウスの位置（使用しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void ChangeZoomRatioByZoomKeyDelta(int zoomKeyDelta, ClassPoint mousePos) {
            if (!ImageAvailable || zoomKeyDelta == 0) {
                return;
            }
            // 拡大率を変更
            ZoomLevel level = new ZoomLevel();
            float currentZoom = m_imageView.ZoomRatio;
            int zoomKey = level.ZoomRatioToZoomKey(currentZoom, Math.Sign(zoomKeyDelta));
            zoomKey += zoomKeyDelta;
            float newZoom = level.GetZoomRatio(zoomKey);
            ChangeZoomRatioDirect(newZoom, mousePos);
        }

        //=========================================================================================
        // 機　能：拡大率を直接指定する
        // 引　数：[in]newZoom  新しい拡大率
        // 　　　　[in]mousePos マウスの位置（使用しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void ChangeZoomRatioDirect(float newZoom, ClassPoint mousePos) {
            if (!ImageAvailable) {
                return;
            }
            float currentZoom = m_imageView.ZoomRatio;
            float oldWidth = m_imageView.ImageWidth;
            float oldHeight = m_imageView.ImageHeight;
            int oldScrollX = m_imageView.ScrollX;
            int oldScrollY = m_imageView.ScrollY;
            m_imageView.SetZoomRatio(newZoom);
            int newScrollX = m_imageView.ScrollX;
            int newScrollY = m_imageView.ScrollY;
            float newWidth = m_imageView.ImageWidth;
            float newHeight = m_imageView.ImageHeight;
            
            // スクロール位置を調整
            int scrollDx, scrollDy;
            if (mousePos == null) {
                // マウス位置なし
                scrollDx = (int)((newWidth - oldWidth) / 2.0);
                scrollDy = (int)((newHeight - oldHeight) / 2.0);
            } else {
                // マウス位置あり
                Size newImageSize = new Size((int)newWidth, (int)newHeight);
                Size newClientSize;
                ImageView.ScrollNeed xScroll, yScroll;
                ImageView.GetClientSize(this, m_imageView.ShowScrollBar, newImageSize, out newClientSize, out xScroll, out yScroll);
                if (newClientSize.Width >= newWidth || mousePos.X < 0 || mousePos.X > newClientSize.Width) {
                    // スクロールなし
                    scrollDx = (int)((newWidth - oldWidth) / 2.0);
                } else {
                    // スクロールあり
                    scrollDx = (int)((newWidth - oldWidth) * (oldScrollX + (float)mousePos.X) / (float)oldWidth);
                }
                if (newClientSize.Height >= newHeight || mousePos.Y < 0 || mousePos.Y > newClientSize.Height) {
                    // スクロールなし
                    scrollDy = (int)((newHeight - oldHeight) / 2.0);
                } else {
                    // スクロールあり
                    scrollDy = (int)((newHeight - oldHeight) * (oldScrollY + (float)mousePos.Y) / (float)oldHeight);
                }
            }

            // 画面を更新
            ScrollView(scrollDx - (newScrollX - oldScrollX), scrollDy - (newScrollY - oldScrollY));
            m_statusBar.RefreshStatusBar();
        }

        //=========================================================================================
        // 機　能：画像を画面サイズに合わせて拡大する
        // 引　数：[in]zoomMode  拡大モード
        // 戻り値：なし
        //=========================================================================================
        public void FitImageToScreen(GraphicsViewerAutoZoomMode zoomMode) {
            if (!ImageAvailable) {
                return;
            }
            int cxImage = m_imageView.CurrentImage.Image.Image.Width;
            int cyImage = m_imageView.CurrentImage.Image.Image.Height;
            int cxClient = this.Width;
            int cyClient = this.Height;
            float newZoom;
            if (zoomMode == GraphicsViewerAutoZoomMode.AutoZoom) {
                newZoom = Math.Min(((float)cxClient) / ((float)cxImage), ((float)cyClient) / ((float)cyImage));
                ChangeZoomRatioDirect(newZoom, null);
            } else if (zoomMode == GraphicsViewerAutoZoomMode.AutoZoomWide) {
                if ((float)cxClient / (float)cyClient > (float)cxImage / (float)cyImage) {      // 横いっぱい：縦スクロール必要
                    float cxScrollBar = (float)SystemInformation.VerticalScrollBarWidth;
                    newZoom = ((float)(cxClient - cxScrollBar)) / ((float)cxImage);
                } else {                                                                        // 縦いっぱい：横スクロール必要
                    float cyScrollBar = (float)SystemInformation.HorizontalScrollBarHeight;
                    newZoom = ((float)(cyClient - cyScrollBar)) / ((float)cyImage);
                }
                ChangeZoomRatioDirect(newZoom, null);
            }
        }

        //=========================================================================================
        // 機　能：ズーム時の補完方法を指定する
        // 引　数：[in]mode  補完方法
        // 戻り値：なし
        //=========================================================================================
        public void SetInterpolationMode(InterpolationMode mode) {
            if (!ImageAvailable) {
                return;
            }
            m_imageView.InterpolationMode = mode;
            this.Invalidate();
        }

        //=========================================================================================
        // 機　能：マウスがダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerPanel_MouseDoubleClick(object sender, MouseEventArgs evt) {
            GraphicsViewerActionCommand command = Program.Document.CommandFactory.CreateGraphicsViewerCommandFromMouseInput(evt, this);
            if (command != null) {
                command.Execute();
            }
        }

        //=========================================================================================
        // 機　能：マウスのボタンが領域から出たときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerPanel_MouseLeave(object sender, EventArgs evt) {
            m_dragLastMousePos = MOUSE_POS_NULL;
            m_dragMousePosHistory = null;
        }

        //=========================================================================================
        // 機　能：マウスのボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerPanel_MouseDown(object sender, MouseEventArgs evt) {
            if (!ImageAvailable) {
                return;
            }
            if ((evt.Button & MouseButtons.Left) != MouseButtons.Left) {
                return;
            }
            m_dragLastMousePos = evt.Location;
            m_dragMousePosHistory = new List<DraggingMousePosition>();
            m_totalMoveDistance = 0;

            if (m_dragAccelerationTimer != null) {
                m_dragAccelerationTimer.Stop();
                m_dragAccelerationTimer = null;
            }

            this.Cursor = Cursors.SizeAll;
            this.Capture = true;

            // 慣性を使用するときはタイマーを設定
            bool localSession = !(SystemInformation.TerminalServerSession);
            DragInertiaType type = Configuration.Current.GraphicsViewerDragInertia;
            if (type == DragInertiaType.Always || type == DragInertiaType.LocalOnly && localSession) {
                m_dragAccelerationTimer = new System.Windows.Forms.Timer();
                m_dragAccelerationTimer.Interval = 30;
                m_dragAccelerationTimer.Tick += new EventHandler(DragAccelerationTimer_Tick);
                m_dragAccelerationTimer.Start();
            }
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerPanel_MouseUp(object sender, MouseEventArgs evt) {
            if (m_dragMousePosHistory != null) {
                DateTime current = DateTime.Now;
                int wholeTime = -1;
                double accelX = 0;         // 最終的に加速に使用する速度[pixel/millisec]
                double accelY = 0;         // 最終的に加速に使用する速度[pixel/millisec]
                for (int i = 0; i < m_dragMousePosHistory.Count - 1; i++) {
                    int spent = (current - m_dragMousePosHistory[i].Time).Milliseconds;
                    if (spent > 1000) {
                        continue;
                    }
                    if (wholeTime == -1) {
                        wholeTime = spent;
                    }
                    
                    // 移動距離[pixel]
                    int lengthX = m_dragMousePosHistory[i + 1].X - m_dragMousePosHistory[i].X;
                    int lengthY = m_dragMousePosHistory[i + 1].Y - m_dragMousePosHistory[i].Y;

                    // 測定時間1区間での速度[pixel/millisec]
                    double spanTime = (double)((m_dragMousePosHistory[i + 1].Time - m_dragMousePosHistory[i].Time).Milliseconds + 1);
                    double vectorX = (double)(m_dragMousePosHistory[i + 1].X - m_dragMousePosHistory[i].X) / spanTime;
                    double vectorY = (double)(m_dragMousePosHistory[i + 1].Y - m_dragMousePosHistory[i].Y) / spanTime;

                    // 加重平均付きでの加速度
                    double effect = (1000.0 - (double)spent) / 1000.0;
                    accelX += vectorX * (spanTime / wholeTime) * effect;
                    accelY += vectorY * (spanTime / wholeTime) * effect;
                }
                m_dragAccelaration = new DraggingMouseAccelaration(-accelX, -accelY, 0, 0, DateTime.Now);
            }
            m_dragLastMousePos = MOUSE_POS_NULL;
            m_dragMousePosHistory = null;

            this.Cursor = Cursors.Arrow;
            this.Capture = false;
            
            if ((evt.Button & MouseButtons.Left) == MouseButtons.Left && m_imageView != null && m_totalMoveDistance >= 0 && m_totalMoveDistance < 5) {
                m_imageView.AutoZoomModeOnClick(evt.X, evt.Y);
                m_statusBar.RefreshStatusBar();
            }
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerPanel_MouseMove(object sender, MouseEventArgs evt) {
            if (!ImageAvailable) {
                return;
            }
            if ((evt.Button & MouseButtons.Left) != MouseButtons.Left) {
                return;
            }
            if (m_dragLastMousePos == MOUSE_POS_NULL) {
                return;
            }
            int dx = m_dragLastMousePos.X - evt.Location.X;
            int dy = m_dragLastMousePos.Y - evt.Location.Y;
            m_totalMoveDistance += dx + dy;
            ScrollView(dx, dy);
            m_dragLastMousePos = evt.Location;

            if (m_dragAccelerationTimer != null) {
                DragAccelerationTimer_Tick(null, null);
            }
        }

        //=========================================================================================
        // 機　能：慣性スクロールタイマーの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void DragAccelerationTimer_Tick(object sender, EventArgs evt) {
            if (m_dragMousePosHistory != null) {
                // ドラッグ中は加速
                DraggingMousePosition mousePos = new DraggingMousePosition(m_dragLastMousePos.X, m_dragLastMousePos.Y, DateTime.Now);
                m_dragMousePosHistory.Add(mousePos);
                if (m_dragMousePosHistory.Count > 20) {
                    m_dragMousePosHistory.RemoveAt(0);
                }
            } else {
                // ドラッグ終了中は惰性で移動
                DateTime currentTime = DateTime.Now;
                int spanTime = (currentTime - m_dragAccelaration.Time).Milliseconds;
                double newXPos = m_dragAccelaration.XPos + m_dragAccelaration.XVector * spanTime;
                double newYPos = m_dragAccelaration.YPos + m_dragAccelaration.YVector * spanTime;
                int dxPos = (int)(newXPos - m_dragAccelaration.XPos);
                int dyPos = (int)(newYPos - m_dragAccelaration.YPos);
                ScrollView(dxPos, dyPos);
                double accel = (double)(Configuration.Current.GraphicsViewerDragBreaking) / 3000.0;
                double newAccelX, newAccelY;
                if (m_dragAccelaration.XVector > 0) {
                    newAccelX = Math.Max(0, m_dragAccelaration.XVector - accel);
                } else {
                    newAccelX = Math.Min(0, m_dragAccelaration.XVector + accel);
                }
                if (m_dragAccelaration.YVector > 0) {
                    newAccelY = Math.Max(0, m_dragAccelaration.YVector - accel);
                } else {
                    newAccelY = Math.Min(0, m_dragAccelaration.YVector + accel);
                }
                if (newAccelX == 0 && newAccelY == 0) {
                    // 加速終了
                    if (m_dragAccelerationTimer != null) {
                        m_dragAccelerationTimer.Stop();
                        m_dragAccelerationTimer = null;
                    }
                } else {
                    m_dragAccelaration = new DraggingMouseAccelaration(newAccelX, newAccelY, newXPos, newYPos, currentTime);
                }
            }
        }

        //=========================================================================================
        // 機　能：イメージをクリップボードにコピーする
        // 引　数：なし
        // 戻り値：コピーできたときtrue
        //=========================================================================================
        public bool CopyImage() {
            if (!ImageAvailable) {
                return false;
            }
            Clipboard.SetImage(m_imageView.CurrentImage.Image.Image);
            return true;
        }

        //=========================================================================================
        // 機　能：現在表示中の画像の状態を更新する
        // 引　数：[in]resetFilter   フィルター設定を強制的に取り込むときtrue
        // 戻り値：なし
        //=========================================================================================
        public void ResetCurrentImageUI(bool resetFilter) {
            m_imageView.SetImage(m_imageView.CurrentImage, resetFilter, false);
            m_statusBar.RefreshStatusBar();
        }

        //=========================================================================================
        // 機　能：スライドショーで次の画像の切り替えをリクエストする
        // 引　数：[in]forward  順方向に切り替えるときtrue
        // 戻り値：最終画像からさらに進めようとしたときtrue
        //=========================================================================================
        public bool RequestSlideShowNext(bool forward) {
            // 最終画像かどうかを確認
            m_slideShowList.SetSlideShowMode();
            if (m_slideShowList.CurrentIndex == 0 && !forward) {
                return true;
            } else if (m_slideShowList.CurrentIndex == m_slideShowList.AllImageCount - 1 && forward) {
                return true;
            }

            // 次の画像を取得
            ImageInfo currentImage;
            List<ImageInfo> nextLoadImageList;
            bool successChange = m_slideShowList.GetNextSlide(forward, out currentImage, out nextLoadImageList);
            if (successChange) {
                m_imageView.SetImage(currentImage, false, true);
                m_statusBar.RefreshStatusBar();
                m_form.RefreshTitle();
            }

            // バックグラウンドの読み込みを要求
            foreach (ImageInfo nextImage in nextLoadImageList) {
                GraphicsViewerImageLoadRequest request = new GraphicsViewerImageLoadRequest(m_targetFileSystem, m_targetFileListContext, m_targetBasePath, nextImage, forward, m_form);
                Program.Document.BackgroundTaskManager.GraphicsViewerTaskManager.RequestLoadImage(request);
            }

            return false;
        }
        
        //=========================================================================================
        // 機　能：ファイルのマークを切り替える
        // 引　数：[in]markNo   セットするマーク番号（0:クリア）
        // 戻り値：なし
        //=========================================================================================
        public void SetMark(int markNo) {
            if (m_imageView.CurrentImage != null) {
                m_imageView.CurrentImage.MarkState = markNo;
            }
            m_statusBar.RefreshStatusBar();
        }
        
        //=========================================================================================
        // 機　能：スライドショーのマーク画像一覧の情報を返す
        // 引　数：なし
        // 戻り値：マーク一覧の情報（画像が1件もマークされていないときnull）
        //=========================================================================================
        public SlideShowMarkResult GetMarkResult() {
            // クリップボードビューアのとき
            if (m_slideShowList.AllImages == null) {
                return null;
            }

            // スライドショーのとき
            SlideShowMarkResult markResult = new SlideShowMarkResult();
            foreach (ImageInfo imageInfo in m_slideShowList.AllImages) {
                if (imageInfo.FilePath != "") {
                    markResult.AddImage(imageInfo.FilePath, imageInfo.MarkState);
                }
            }
            if (markResult.Available) {
                return markResult;
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：画像を保存する
        // 引　数：[in]fileName    ファイル名
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public bool SaveImageDataAs(string fileName) {
            Image image = this.CurrentImage.Image.Image;
            if (image == null) {
                InfoBox.Warning(this.GraphicsViewerForm, Resources.Msg_GraphicsViewerSaveNotSupported);
                return false;
            }

            // フォーマットを決定
            ImageFormat format;
            string ext = GenericFileStringUtils.GetExtensionLast(fileName).ToLower();
            if (ext == "bmp") {
                format = ImageFormat.Bmp;
            } else if (ext == "png") {
                format = ImageFormat.Png;
            } else if (ext == "jpg" || ext == "jpeg") {
                format = ImageFormat.Jpeg;
            } else {
                InfoBox.Warning(this.GraphicsViewerForm, Resources.Msg_GraphicsViewerSaveUnknownFormat, ext);
                return false;
            }

            // 変換
            byte[] bytes;
            try {
                MemoryStream stream = new MemoryStream();
                try {
                    image.Save(stream, format);
                } finally {
                    stream.Close();
                }
                bytes = stream.ToArray();
            } catch (OutOfMemoryException) {
                InfoBox.Warning(this.GraphicsViewerForm, Resources.Msg_GraphicsViewerSaveOutOfMemory);
                return false;
            }

            // 保存
            try {
                File.WriteAllBytes(fileName, bytes);
            } catch (Exception e) {
                InfoBox.Warning(this.GraphicsViewerForm, Resources.Msg_GraphicsViewerSaveFailed, e.Message);
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：グラフィックビューアが全画面最大化されたときの処理を行う
        // 引　数：[in]maximized   最大化するときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnFullScreenChanged(bool maximized) {
            m_imageView.OnFullScreenChanged(maximized);
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
        // 機　能：色をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetColor() {
            m_graphicViewerColor = new GraphicsViewerColor();
            m_graphicViewerColor.GraphicsViewerBackColor = Configuration.Current.GraphicsViewerBackColor;
            m_graphicViewerColor.GraphicsViewerTextColor = Configuration.Current.GraphicsViewerTextColor;
            m_graphicViewerColor.GraphicsViewerTextShadowColor = Configuration.Current.GraphicsViewerTextShadowColor;
            m_graphicViewerColor.GraphicsViewerLoadingTextColor = Configuration.Current.GraphicsViewerLoadingTextColor;
            m_graphicViewerColor.GraphicsViewerLoadingTextShadowColor = Configuration.Current.GraphicsViewerLoadingTextShadowColor;

            this.BackColor = Configuration.Current.GraphicsViewerBackColor;
            m_form.BackColor = Configuration.Current.GraphicsViewerBackColor;
        }

        //=========================================================================================
        // プロパティ：背景色
        //=========================================================================================
        public Color BackgroundColor {
            get {
                return this.BackColor;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアのフォーム
        //=========================================================================================
        public GraphicsViewerForm GraphicsViewerForm {
            get {
                return m_form;
            }
        }

        //=========================================================================================
        // プロパティ：現在表示中のイメージ（エラー状態のときnull）
        //=========================================================================================
        public ImageInfo CurrentImage {
            get {
                if (m_imageView == null) {
                    return null;
                } else {
                    return m_imageView.CurrentImage;
                }
            }
        }
        
        //=========================================================================================
        // プロパティ：スライドショーの画像一覧
        //=========================================================================================
        public ISlideShowList SlideShowList {
            get {
                return m_slideShowList;
            }
        }
        //=========================================================================================
        // プロパティ：使用するフィルター
        //=========================================================================================
        public GraphicsViewerFilterSetting FilterSetting {
            get {
                return m_filterSetting;
            }
        }

        //=========================================================================================
        // プロパティ：現在表示中の画像の拡大率
        //=========================================================================================
        public float ZoomRatio {
            get {
                return m_imageView.ZoomRatio;
            }
        }

        //=========================================================================================
        // プロパティ：自動で拡大するモードが設定された直後のときtrue
        //=========================================================================================
        public bool AutoZoomModeSet {
            get {
                return m_imageView.AutoZoomModeSet;
            }
        }

        //=========================================================================================
        // プロパティ：画像が表示可能な状態のときtrue
        //=========================================================================================
        public bool ImageAvailable {
            get {
                if (m_imageView == null || !m_imageView.ImageAvailable) {
                    return false;
                } else {
                    return true;
                }
            }
        }

        //=========================================================================================
        // プロパティ：スクロールバーを表示するときtrue
        //=========================================================================================
        public bool ShowScrollBar {
            get {
                return m_imageView.ShowScrollBar;
            }
            set {
                m_imageView.SetShowScrollBar(value);
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアの表示色
        //=========================================================================================
        public GraphicsViewerColor GraphicsViewerColor {
            get {
                return m_graphicViewerColor;
            }
        }
    }
}
