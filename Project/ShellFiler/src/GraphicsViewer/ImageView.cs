using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading;
using System.Reflection;
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
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.Locale;

namespace ShellFiler.GraphicsViewer {

    //=========================================================================================
    // クラス：イメージの描画を行うビュー
    //=========================================================================================
    public class ImageView {
        // 表示対象のパネル
        private GraphicsViewerPanel m_parent;
        
        // フルスクリーン中のタイマー付き機能の実装（フルスクリーン表示中でないときnull）
        private FullScreenTimerImpl m_fullScreenTimerImpl = null;

        // 現在表示されている画像（表示中ではない場合null）
        private ImageInfo m_currentImage = null;

        // 画像を高速に描画するためのテクスチャブラシ（画像を表示中でない場合null）
        private TextureBrush m_textureBrush = null;

        // 使用するフィルター
        private GraphicsViewerFilterSetting m_filterSetting;

        // 画像に適用するフィルタ
        private ImageFilter m_imageFilter = new ImageFilter();

        // 高速描画状態で描画するときtrue、高画質状態で描画するときfalse
        private bool m_highSpeedDraw = false;

        // スクロールバーを表示するときtrue;
        private bool m_useScrollbar = true;

        // スクロールの位置（画像の左上、スクロール不要のときは-1）
        private Point m_scrollPos;

        // クライアント領域の大きさ
        private Size m_clientRectangleSize;

        // 画像の大きさ（画像非表示のときは0,0）
        private Size m_imageSize;

        // 拡大率
        private float m_zoomRatio = 1.0f;

        // 自動で拡大するモード
        private GraphicsViewerAutoZoomMode m_autoZoomMode = Configuration.Current.GraphicsViewerAutoZoomMode;

        // 自動で拡大するモードが設定された直後のときtrue
        private bool m_autoZoomModeSet = false;

        // 画像の拡大アルゴリズム
        private InterpolationMode m_interpolationMode = InterpolationMode.HighQualityBilinear;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent         表示対象のパネル
        // 　　　　[in]filterSetting  使用するフィルター
        // 戻り値：なし
        //=========================================================================================
        public ImageView(GraphicsViewerPanel parent, GraphicsViewerFilterSetting filterSetting) {
            m_parent = parent;
            m_filterSetting = filterSetting;
            m_scrollPos = new Point(-1, -1);
            m_clientRectangleSize = m_parent.ClientSize;
            m_imageSize = new Size(0, 0);
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]parent  親となるパネル
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
            m_parent.Paint += new PaintEventHandler(PicturePanel_Paint);
            m_parent.Resize += new EventHandler(PicturePanel_Resize);
            m_parent.BackColor = Color.Transparent;
            Application.Idle += new EventHandler(Application_Idle);
        }

        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisposeComponent() {
            Application.Idle -= new EventHandler(Application_Idle);
            if (m_fullScreenTimerImpl != null) {
                m_fullScreenTimerImpl.Dispose();
                m_fullScreenTimerImpl = null;
            }
            if (m_textureBrush != null) {
                m_textureBrush.Dispose();
                m_textureBrush = null;
            }
        }

        //=========================================================================================
        // 機　能：表示対象の画像情報を設定する
        // 引　数：[in]image        表示対象の画像情報
        // 　　　　[in]keepFilter   フィルターを保持するときtrue
        // 　　　　[in]changeImage  画像が変更されたときtrue
        // 戻り値：なし
        //=========================================================================================
        public void SetImage(ImageInfo imageInfo, bool keepFilter, bool changeImage) {
            m_currentImage = imageInfo;
            m_autoZoomModeSet = false;

            if (changeImage) {
                if (m_fullScreenTimerImpl != null) {
                    m_fullScreenTimerImpl.OnSlideChange();
                }
            }

            // 画像がある場合は高速描画用のテクスチャブラシを初期化
            if (m_textureBrush != null) {
                m_textureBrush.Dispose();
                m_textureBrush = null;
            }

            // 画像を設定
            if (imageInfo.OriginalImage != null) {
                BufferedImage filtered = null;
                try {
                    filtered = new BufferedImage();
                    filtered.Image = new Bitmap(imageInfo.OriginalImage.Image);
                    m_imageFilter.InitializeFilter(m_filterSetting, keepFilter, changeImage);
                    m_imageFilter.ApplyFilter((Bitmap)(filtered.Image));
                } catch (OutOfMemoryException) {
                    filtered = null;                // フィルターなしでリカバリ
                    GC.Collect();
                } catch (TargetInvocationException) {
                    filtered = null;
                    GC.Collect();
                } catch (ArgumentException) {
                    filtered = null;
                    GC.Collect();
                }
                imageInfo.SetFilteredImage(filtered);

                // 高速描画用のテクスチャブラシ
                try {
                    m_textureBrush = new TextureBrush(m_currentImage.Image.Image);
                    m_textureBrush.WrapMode = WrapMode.Clamp;
                } catch (OutOfMemoryException) {
                    if (m_textureBrush != null) {   // 高速描画なしでリカバリ
                        m_textureBrush.Dispose();
                        m_textureBrush = null;
                    }
                    GC.Collect();
                }
                m_parent.BackColor = Color.Transparent;

                // 画像のサイズを補正
                if (!keepFilter) {
                    ModifyImageSizeWithAutoZoom(true);
                }
            } else {
                m_parent.BackColor = Configuration.Current.GraphicsViewerBackColor;
                m_imageSize = new Size(0, 0);
            }
            UpdateScrollbar();
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：画像の拡大率を自動ズームの値で補正する
        // 引　数：[in]reset   ズームモードをリセットするときtrue
        // 戻り値：なし
        //=========================================================================================
        public void ModifyImageSizeWithAutoZoom(bool reset) {
            if (reset) {
                if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomOff) {
                    m_autoZoomMode = GraphicsViewerAutoZoomMode.AutoZoom;
                } else if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomWideOff) {
                    m_autoZoomMode = GraphicsViewerAutoZoomMode.AutoZoomWide;
                }
            }

            int cxImage = m_currentImage.Image.Image.Width;
            int cyImage = m_currentImage.Image.Image.Height;
            int cxClient = m_parent.Width;
            int cyClient = m_parent.Height;
            float newZoom = float.NaN;
            if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AlwaysOriginal ||
                    m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomOff ||
                    m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomWideOff) {
            } else if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoom ||
                    m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomAlways) {
                newZoom = Math.Min(((float)cxClient) / ((float)cxImage), ((float)cyClient) / ((float)cyImage));
                if (!Configuration.Current.GraphicsViewerZoomInLarger && newZoom > 1.0f) {
                    newZoom = 1.0f;
                    if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoom) {
                        m_autoZoomMode = GraphicsViewerAutoZoomMode.AutoZoomOff;
                    }
                }
            } else if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomWide ||
                    m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomWideAlways) {
                if ((float)cxClient / (float)cyClient > (float)cxImage / (float)cyImage) {      // 横いっぱい：縦スクロール必要
                    float cxScrollBar = (float)SystemInformation.VerticalScrollBarWidth;
                    newZoom = ((float)(cxClient - cxScrollBar)) / ((float)cxImage);
                } else {                                                                        // 縦いっぱい：横スクロール必要
                    float cyScrollBar = (float)SystemInformation.HorizontalScrollBarHeight;
                    newZoom = ((float)(cyClient - cyScrollBar)) / ((float)cyImage);
                }
                if (!Configuration.Current.GraphicsViewerZoomInLarger && newZoom > 1.0f) {
                    newZoom = 1.0f;
                    if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomWide) {
                        m_autoZoomMode = GraphicsViewerAutoZoomMode.AutoZoomWideOff;
                    }
                }
            }
            if (!float.IsNaN(newZoom)) {
                m_zoomRatio = newZoom;
            }
            m_imageSize = new Size((int)(cxImage * m_zoomRatio), (int)(cyImage * m_zoomRatio));
            if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoom || m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomWide) {
                m_autoZoomModeSet = true;
            } else {
                m_autoZoomModeSet = false;
            }
        }

        //=========================================================================================
        // 機　能：マウスクリック時に画像の拡大率を自動で補正する
        // 引　数：[in]mouseX  クリックしたときのマウスのX座標
        // 　　　　[in]mouseY  クリックしたときのマウスのY座標
        // 戻り値：なし
        //=========================================================================================
        public void AutoZoomModeOnClick(int mouseX, int mouseY) {
            if (m_currentImage == null || m_currentImage.Image == null) {
                return;
            }
            int cxImage = m_currentImage.Image.Image.Width;
            int cyImage = m_currentImage.Image.Image.Height;
            int cxClient = m_parent.Width;
            int cyClient = m_parent.Height;
            float oldZoomRatio = m_zoomRatio; 

            // モードを変更
            bool useDefault = (!Configuration.Current.GraphicsViewerZoomInLarger && (cxImage < cxClient) && (cyImage < cyClient));
            if (useDefault ||
                    m_autoZoomMode == GraphicsViewerAutoZoomMode.AlwaysOriginal ||
                    m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomAlways ||
                    m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomWideAlways) {
                return;
            } else if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomOff) {
                m_autoZoomMode = GraphicsViewerAutoZoomMode.AutoZoom;
            } else if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomWideOff) {
                m_autoZoomMode = GraphicsViewerAutoZoomMode.AutoZoomWide;
            } else if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoom) {
                m_autoZoomMode = GraphicsViewerAutoZoomMode.AutoZoomOff;
                m_zoomRatio = 1.0f;
            } else if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomWide) {
                m_autoZoomMode = GraphicsViewerAutoZoomMode.AutoZoomWideOff;
                m_zoomRatio = 1.0f;
            }

            // マウスの位置を中央に
            if (m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomOff || m_autoZoomMode == GraphicsViewerAutoZoomMode.AutoZoomWideOff) {
                int newX, newY;
                if (m_scrollPos.X == -1) {
                    newX = (int)((mouseX - (m_clientRectangleSize.Width - m_imageSize.Width) / 2) / oldZoomRatio);
                } else {
                    newX = (int)((mouseX + m_scrollPos.X) / oldZoomRatio);
                }
                if (m_scrollPos.Y == -1) {
                    newY = (int)((mouseY - (m_clientRectangleSize.Height - m_imageSize.Height) / 2) / oldZoomRatio);
                } else {
                    newY = (int)((mouseY + m_scrollPos.Y) / oldZoomRatio);
                }
                newX = (int)(newX - (m_clientRectangleSize.Width) * m_zoomRatio / 2);       // クリック位置を中央に
                newY = (int)(newY - (m_clientRectangleSize.Height) * m_zoomRatio / 2);
                m_scrollPos = new Point(newX, newY);
            }

            // 表示に反映
            ModifyImageSizeWithAutoZoom(false);
            UpdateScrollbar();
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：画像の拡大率を設定する
        // 引　数：[in]zoom  拡大率（zoom倍）
        // 戻り値：なし
        //=========================================================================================
        public void SetZoomRatio(float zoom) {
            m_autoZoomModeSet = false;
            ZoomLevel zoomLevel = new ZoomLevel();
            m_zoomRatio = Math.Min(zoomLevel.MaxZoomValue, Math.Max(zoomLevel.MinZoomKey, zoom));
            m_highSpeedDraw = true;
            m_imageSize = new Size((int)(m_currentImage.Image.Image.Width * m_zoomRatio), (int)(m_currentImage.Image.Image.Height * m_zoomRatio));
            UpdateScrollbar();
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：スクロールバーを使用するかどうかを設定する
        // 引　数：[in]use  スクロールバーを使用するときtrue
        // 戻り値：なし
        //=========================================================================================
        public void SetShowScrollBar(bool use) {
            m_useScrollbar = use;
            UpdateScrollbar();
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：アプリケーションがアイドル状態になったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void Application_Idle(object sender, EventArgs evt) {
            // アイドル状態で高速描画のままの場合、高画質で再描画
            if (m_highSpeedDraw) {
                m_highSpeedDraw = false;
                m_parent.Invalidate();
            }
        }

        //=========================================================================================
        // 機　能：パネルのサイズが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void PicturePanel_Resize(object sender, EventArgs evt) {
            UpdateScrollbar();
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：パネルが再描画されるときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void PicturePanel_Paint(object sender, PaintEventArgs evt) {
            if (m_currentImage == null || m_currentImage.State == ImageState.Null) {
                // 処理前
            } else if (m_currentImage.Image == null) {
                // エラー
                GraphicsViewerGraphics grp = new GraphicsViewerGraphics(m_parent, evt.Graphics, m_parent.GraphicsViewerColor);
                try {
                    DrawErrorInfo(grp);
                } finally {
                    grp.Dispose();
                }
            } else {
                // 画像あり
                if (m_highSpeedDraw && m_textureBrush != null) {
                    // スクロール中などの高速描画
                    DrawHighspeed(evt.Graphics);
                } else {
                    // 固定後の高品質描画
                    DrawHighQuority(evt.Graphics);
                }
                if (m_fullScreenTimerImpl != null) {
                    GraphicsViewerGraphics grp = new GraphicsViewerGraphics(m_parent, evt.Graphics, m_parent.GraphicsViewerColor);
                    try {
                        m_fullScreenTimerImpl.DrawInformation(grp, m_currentImage);
                    } finally {
                        grp.Dispose();
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：エラー情報を描画する
        // 引　数：[in]grp  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawErrorInfo(GraphicsViewerGraphics grp) {
            Graphics g = grp.Graphics;

            int cxClient = m_parent.ClientSize.Width;
            int centerY = m_parent.ClientSize.Height / 2;
            int fontSize = grp.GraphicsViewerMessageFont.Height;
            Rectangle rectMessage1 = new Rectangle(1, centerY - fontSize + 1, cxClient - 1, fontSize);
            Rectangle rectMessage2 = new Rectangle(0, centerY - fontSize, cxClient - 1, fontSize);
            Rectangle rectFileName1 = new Rectangle(1, centerY + fontSize + 1, cxClient - 1, fontSize);
            Rectangle rectFileName2 = new Rectangle(0, centerY + fontSize, cxClient - 1, fontSize);
            
            if (m_currentImage.State == ImageState.Loading) {
                // 読み込み中
                g.DrawString(Resources.GraphicsViewer_MessageLoading, grp.GraphicsViewerMessageFont, grp.GraphicsLoadingViewerTextShadowBrush, rectMessage1, grp.StringFormat);
                g.DrawString(Resources.GraphicsViewer_MessageLoading, grp.GraphicsViewerMessageFont, grp.GraphicsLoadingViewerTextBrush, rectMessage2, grp.StringFormat);
                g.DrawString(m_currentImage.FilePath, grp.GraphicsViewerFileNameFont, grp.GraphicsLoadingViewerTextShadowBrush, rectFileName1, grp.StringFormat);
                g.DrawString(m_currentImage.FilePath, grp.GraphicsViewerFileNameFont, grp.GraphicsLoadingViewerTextBrush, rectFileName2, grp.StringFormat);
            } else {
                // エラー
                g.DrawString(m_currentImage.ErrorMessage, grp.GraphicsViewerMessageFont, grp.GraphicsViewerTextShadowBrush, rectMessage1, grp.StringFormat);
                g.DrawString(m_currentImage.ErrorMessage, grp.GraphicsViewerMessageFont, grp.GraphicsViewerTextBrush, rectMessage2, grp.StringFormat);
                g.DrawString(m_currentImage.FilePath, grp.GraphicsViewerFileNameFont, grp.GraphicsViewerTextShadowBrush, rectFileName1, grp.StringFormat);
                g.DrawString(m_currentImage.FilePath, grp.GraphicsViewerFileNameFont, grp.GraphicsViewerTextBrush, rectFileName2, grp.StringFormat);
            }
        }

        //=========================================================================================
        // 機　能：パネルを高速モードで再描画する
        // 引　数：[in]g  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawHighspeed(Graphics g) {
            int xStart;
            if (m_scrollPos.X >= 0) {
                xStart = m_scrollPos.X;
            } else {
                xStart = -(m_clientRectangleSize.Width - m_imageSize.Width) / 2;
            }
            int yStart;
            if (m_scrollPos.Y >= 0) {
                yStart = m_scrollPos.Y;
            } else {
                yStart = -(m_clientRectangleSize.Height - m_imageSize.Height) / 2;
            }
            if (m_textureBrush == null) {       // メモリ不足のとき
                return;
            }
            m_textureBrush.ResetTransform();
            m_textureBrush.TranslateTransform(-xStart, -yStart);
            m_textureBrush.ScaleTransform(m_zoomRatio, m_zoomRatio);
            g.FillRectangle(m_textureBrush, m_parent.ClientRectangle);
        }

        //=========================================================================================
        // 機　能：パネルを高品質モードで再描画する
        // 引　数：[in]g  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawHighQuority(Graphics g) {
            int xStart;
            if (m_scrollPos.X >= 0) {
                xStart = m_scrollPos.X;
            } else {
                xStart = -(m_clientRectangleSize.Width - m_imageSize.Width) / 2;
            }
            int yStart;
            if (m_scrollPos.Y >= 0) {
                yStart = m_scrollPos.Y;
            } else {
                yStart = -(m_clientRectangleSize.Height - m_imageSize.Height) / 2;
            }
            RectangleF srcRect = new RectangleF(xStart / m_zoomRatio, yStart / m_zoomRatio, m_parent.Width / m_zoomRatio, m_parent.Height / m_zoomRatio);
            RectangleF destRect = new RectangleF(0, 0, m_parent.Width, m_parent.Height);
            g.InterpolationMode  = m_interpolationMode;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.ResetTransform();
            try {
                g.DrawImage(m_currentImage.Image.Image, destRect, srcRect, GraphicsUnit.Pixel);
            } catch (OutOfMemoryException) {
                GC.Collect();
                DrawHighspeed(g);
            }
        }

        //=========================================================================================
        // 機　能：スクロールバーの状態を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void UpdateScrollbar() {
            if (m_currentImage == null || m_currentImage.Image == null) {
                if (m_parent.HorizontalScroll.Visible != false) {
                    m_parent.HorizontalScroll.Visible = false;
                }
                if (m_parent.VerticalScroll.Visible != false) {
                    m_parent.VerticalScroll.Visible = false;
                }
                return;
            }
            ScrollNeed xScroll, yScroll;
            GetClientSize(m_parent, m_useScrollbar, m_imageSize, out m_clientRectangleSize, out xScroll, out yScroll);

            // スクロール位置を調整
            ModifyScrollPos();

            // 水平スクロールバーのUIを更新
            if (xScroll == ScrollNeed.Unnecessary || m_clientRectangleSize.Width <= 0) {
                if (m_parent.HorizontalScroll.Visible != false) {
                    m_parent.HorizontalScroll.Visible = false;
                }
            } else {
                if (m_parent.HorizontalScroll.Enabled != true) {
                    m_parent.HorizontalScroll.Enabled = true;
                }
                if (m_parent.HorizontalScroll.Visible != true) {
                    m_parent.HorizontalScroll.Visible = true;
                }
                m_parent.HorizontalScroll.SmallChange = 1;
                m_parent.HorizontalScroll.LargeChange = m_clientRectangleSize.Width;
                m_parent.HorizontalScroll.Minimum = 0;
                m_parent.HorizontalScroll.Maximum = m_imageSize.Width;
                m_parent.HorizontalScroll.Value = m_scrollPos.X;
            }

            // 垂直スクロールバーのUIを更新
            if (yScroll == ScrollNeed.Unnecessary || m_clientRectangleSize.Height <= 0) {
                if (m_parent.VerticalScroll.Visible != false) {
                    m_parent.VerticalScroll.Visible = false;
                }
            } else {
                if (m_parent.VerticalScroll.Enabled != true) {
                    m_parent.VerticalScroll.Enabled = true;
                }
                if (m_parent.VerticalScroll.Visible != true) {
                    m_parent.VerticalScroll.Visible = true;
                }
                m_parent.VerticalScroll.SmallChange = 1;
                m_parent.VerticalScroll.LargeChange = m_clientRectangleSize.Height;
                m_parent.VerticalScroll.Minimum = 0;
                m_parent.VerticalScroll.Maximum = m_imageSize.Height;
                m_parent.VerticalScroll.Value = m_scrollPos.Y;
            }
        }

        //=========================================================================================
        // 機　能：画像に対するクライアント領域のサイズを返す
        // 引　数：[in]panel           対象となるパネル
        // 　　　　[in]useScrollBar    スクロールバーを使用するときtrue
        // 　　　　[in]imageSize       画像サイズ
        // 　　　　[out]resultClient   クライアント領域の大きさを返す変数
        // 　　　　[out]resultXScroll  X方向のスクロールバーの種類を返す変数
        // 　　　　[out]resultYScroll  Y方向のスクロールバーの種類を返す変数
        // 戻り値：なし
        //=========================================================================================
        public static void GetClientSize(GraphicsViewerPanel panel, bool useScrollBar, Size imageSize, out Size resultClient, out ScrollNeed resultXScroll, out ScrollNeed resultYScroll) {
            int cxImage = imageSize.Width;
            int cyImage = imageSize.Height;
            int cxScrollBar = useScrollBar ? SystemInformation.VerticalScrollBarWidth : 0;
            int cyScrollBar = useScrollBar ? SystemInformation.HorizontalScrollBarHeight : 0;
            int cxWindow = panel.ClientRectangle.Width + (panel.VerticalScroll.Visible ? cxScrollBar : 0);
            int cyWindow = panel.ClientRectangle.Height + (panel.HorizontalScroll.Visible ? cyScrollBar : 0);

            // スクロールバーの表示/非表示を決定
            ScrollNeed xScroll;
            if (!useScrollBar) {
                xScroll = ScrollNeed.Unnecessary;
            } else if (cxImage <= cxWindow - cxScrollBar) {
                xScroll = ScrollNeed.Unnecessary;
            } else if (cxImage > cxWindow) {
                xScroll = ScrollNeed.Need;
            } else {
                xScroll = ScrollNeed.Unknown;
            }
            ScrollNeed yScroll;
            if (!useScrollBar) {
                yScroll = ScrollNeed.Unnecessary;
            } else if (cyImage <= cyWindow - cyScrollBar) {
                yScroll = ScrollNeed.Unnecessary;
            } else if (cyImage > cyWindow) {
                yScroll = ScrollNeed.Need;
            } else {
                yScroll = ScrollNeed.Unknown;
            }

            // 事前の状態に基づいて最終状態を決定
            //     X方向 Y方向 → X決定 Y決定
            //     あり  あり  → あり  あり
            //     あり  なし  → あり  なし
            //     あり  不明  → あり  あり
            //     なし  あり  → なし  あり
            //     なし  なし  → なし  なし
            //     なし  不明  → なし  なし
            //     不明  あり  → あり  あり
            //     不明  なし  → なし  なし
            //     不明  不明  → なし  なし
            if (xScroll != ScrollNeed.Unknown && yScroll != ScrollNeed.Unknown) {
                ;
            } else if (xScroll == ScrollNeed.Unknown && yScroll == ScrollNeed.Unknown) {
                xScroll = ScrollNeed.Unnecessary;
                yScroll = ScrollNeed.Unnecessary;
            } else if (xScroll == ScrollNeed.Unknown) {
                xScroll = yScroll;
            } else {
                yScroll = xScroll;
            }

            // クライアント領域の大きさを計算
            int cxClient, cyClient;
            if (yScroll == ScrollNeed.Need) {
                cxClient = cxWindow - cxScrollBar;
            } else {
                cxClient = cxWindow;
            }
            if (xScroll == ScrollNeed.Need) {
                cyClient = cyWindow - cyScrollBar;
            } else {
                cyClient = cyWindow;
            }
            resultClient = new Size(cxClient, cyClient);
            resultXScroll = xScroll;
            resultYScroll = yScroll;
        }

        //=========================================================================================
        // 機　能：スクロール位置が画面外にはみ出さないように補正する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ModifyScrollPos() {
            int xPos;
            if (m_imageSize.Width <= m_clientRectangleSize.Width) {
                xPos = -1;
            } else {
                xPos = Math.Min(m_imageSize.Width - m_clientRectangleSize.Width + 1, Math.Max(0, m_scrollPos.X));
            }

            int yPos;
            if (m_imageSize.Height <= m_clientRectangleSize.Height) {
                yPos = -1;
            } else {
                yPos = Math.Min(m_imageSize.Height - m_clientRectangleSize.Height + 1, Math.Max(0, m_scrollPos.Y));
            }

            m_scrollPos = new Point(xPos, yPos);
        }

        //=========================================================================================
        // 機　能：水平スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnHScroll(ScrollEventArgs evt) {
            int newPosition =  m_parent.HorizontalScroll.Value;
            switch(evt.Type) {
                case ScrollEventType.SmallDecrement:
                    newPosition--;
                    break;
                case ScrollEventType.LargeDecrement:
                    newPosition -= m_parent.Width;
                    break;
                case ScrollEventType.SmallIncrement:
                    newPosition++;
                    break;
                case ScrollEventType.LargeIncrement:
                    newPosition += m_parent.Width;
                    break;
                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:
                case ScrollEventType.EndScroll:
                    newPosition = evt.NewValue;
                    break;
            }
            m_scrollPos = new Point(newPosition, m_scrollPos.Y);
            ModifyScrollPos();
            m_parent.HorizontalScroll.Value = m_scrollPos.X;
            m_highSpeedDraw = true;
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：垂直スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnVScroll(ScrollEventArgs evt) {
            int newPosition =  m_parent.VerticalScroll.Value;
            switch(evt.Type) {
                case ScrollEventType.SmallDecrement:
                    newPosition--;
                    break;
                case ScrollEventType.LargeDecrement:
                    newPosition -= m_parent.Width;
                    break;
                case ScrollEventType.SmallIncrement:
                    newPosition++;
                    break;
                case ScrollEventType.LargeIncrement:
                    newPosition += m_parent.Width;
                    break;
                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:
                case ScrollEventType.EndScroll:
                    newPosition = evt.NewValue;
                    break;
            }
            m_scrollPos = new Point(m_scrollPos.X, newPosition);
            ModifyScrollPos();
            m_parent.VerticalScroll.Value = m_scrollPos.Y;
            m_highSpeedDraw = true;
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：スクロールする
        // 引　数：[in]dxPixel  左に移動するピクセル数
        // 　　　　[in]dyPixel  下に移動するピクセル数
        // 戻り値：なし
        //=========================================================================================
        public void ScrollView(int dxPixel, int dyPixel) {
            // 新しい位置を計算
            Point oldScrollPos = m_scrollPos;
            m_scrollPos = new Point(m_scrollPos.X + dxPixel, m_scrollPos.Y + dyPixel);
            ModifyScrollPos();

            // 位置が変わっていればUIを更新
            if (oldScrollPos.X != m_scrollPos.X || oldScrollPos.Y != m_scrollPos.Y) {
                if (m_scrollPos.X >= 0 && m_useScrollbar) {
                    m_parent.HorizontalScroll.Value = m_scrollPos.X;
                }
                if (m_scrollPos.Y >= 0 && m_useScrollbar) {
                    m_parent.VerticalScroll.Value = m_scrollPos.Y;
                }
                m_highSpeedDraw = true;
                m_parent.Invalidate();
            }
        }

        //=========================================================================================
        // 機　能：中心にスクロールする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ScrollCenter() {
            // 新しい位置を計算
            Point oldScrollPos = m_scrollPos;
            int xPos;
            if (m_imageSize.Width > m_clientRectangleSize.Width) {
                xPos = (m_imageSize.Width - m_clientRectangleSize.Width) / 2;
            } else {
                xPos = -1;
            }
            int yPos;
            if (m_imageSize.Height > m_clientRectangleSize.Height) {
                yPos = (m_imageSize.Height - m_clientRectangleSize.Height) / 2;
            } else {
                yPos = -1;
            }
            m_scrollPos = new Point(xPos, yPos);
            ModifyScrollPos();

            // 位置が変わっていればUIを更新
            if (oldScrollPos.X != m_scrollPos.X || oldScrollPos.Y != m_scrollPos.Y) {
                if (m_scrollPos.X >= 0 && m_useScrollbar) {
                    m_parent.HorizontalScroll.Value = m_scrollPos.X;
                }
                if (m_scrollPos.Y >= 0 && m_useScrollbar) {
                    m_parent.VerticalScroll.Value = m_scrollPos.Y;
                }
                m_highSpeedDraw = true;
                m_parent.Invalidate();
            }
        }

        //=========================================================================================
        // 機　能：グラフィックビューアが全画面最大化されたときの処理を行う
        // 引　数：[in]maximized   最大化するときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnFullScreenChanged(bool maximized) {
            if (maximized && m_fullScreenTimerImpl == null) {
                // OFF→ON
                m_fullScreenTimerImpl = new FullScreenTimerImpl(m_parent.GraphicsViewerForm);
            } else if (!maximized && m_fullScreenTimerImpl != null) {
                // ON→OFF
                m_fullScreenTimerImpl.Dispose();
                m_fullScreenTimerImpl = null;
            }
        }

        //=========================================================================================
        // プロパティ：現在表示中のイメージ（表示中ではない場合null）
        //=========================================================================================
        public ImageInfo CurrentImage {
            get {
                return m_currentImage;
            }
        }

        //=========================================================================================
        // プロパティ：画像の拡大率
        //=========================================================================================
        public float ZoomRatio {
            get {
                return m_zoomRatio;
            }
        }

        //=========================================================================================
        // プロパティ：イメージの幅（拡大率のピクセル換算結果、画像表示中でないときは0）
        //=========================================================================================
        public int ImageWidth {
            get {
                return m_imageSize.Width;
            }
        }

        //=========================================================================================
        // プロパティ：イメージの高さ（拡大率のピクセル換算結果、画像表示中でないときは0）
        //=========================================================================================
        public int ImageHeight {
            get {
                return m_imageSize.Height;
            }
        }

        //=========================================================================================
        // プロパティ：画像が表示可能な状態のときtrue
        //=========================================================================================
        public bool ImageAvailable {
            get {
                if (m_currentImage == null || m_currentImage.Image == null) {
                    return false;
                } else {
                    return true;
                }
            }
        }

        //=========================================================================================
        // プロパティ：画像の拡大アルゴリズム
        //=========================================================================================
        public InterpolationMode InterpolationMode {
            get {
                return m_interpolationMode;
            }
            set {
                m_interpolationMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：スクロールバーを表示するときtrue
        //=========================================================================================
        public bool ShowScrollBar {
            get {
                return m_useScrollbar;
            }
            set {
                m_useScrollbar = value;
            }
        }

        //=========================================================================================
        // プロパティ：自動で拡大するモードが設定された直後のときtrue
        //=========================================================================================
        public bool AutoZoomModeSet {
            get {
                return m_autoZoomModeSet;
            }
        }

        //=========================================================================================
        // プロパティ：スクロールバーのＸ位置（非表示のとき0）
        //=========================================================================================
        public int ScrollX {
            get {
                if (m_parent.HorizontalScroll.Visible) {
                    return m_parent.HorizontalScroll.Value;
                } else {
                    return 0;
                }
            }
        }

        //=========================================================================================
        // プロパティ：スクロールバーのＹ位置（非表示のとき0）
        //=========================================================================================
        public int ScrollY {
            get {
                if (m_parent.VerticalScroll.Visible) {
                    return m_parent.VerticalScroll.Value;
                } else {
                    return 0;
                }
            }
        }


        //=========================================================================================
        // 列挙子：スクロールバーが必要かどうかの値
        //=========================================================================================
        public enum ScrollNeed {
            Unnecessary,                // スクロールバーが不要
            Need,                       // スクロールバーが必要
            Unknown,                    // 必要性は不明
        }
    }
}
