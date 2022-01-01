using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.FileSystem;
using ShellFiler.UI;
using ShellFiler.Util;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：全画面表示中のタイマー制御付加機能の実装クラス
    // ・一定時間後にマウスカーソルを消す
    // ・一定時間後に画像情報を消す
    //=========================================================================================
    public class FullScreenTimerImpl {
        // タイマーの呼び出し間隔
        private const int TIMER_INTERVAL = 500;

        // マウスが動いたと見なす座標のしきい値
        private const int MOUSE_MOVE_THRETHOLD = 8;

        // 親となるフォーム
        private GraphicsViewerForm m_parentForm;

        // 一定時間を計測するタイマー
        private System.Windows.Forms.Timer m_fullScreenWaitTimer = null;

        // カーソルが非表示状態になっているときtrue
        private bool m_isHideCursor = false;

        // 情報が非表示状態になっているときtrue
        private bool m_isHideInfo = false;

        // 前回マウス移動時のマウス位置
        private Point m_lastMousePosition = new Point(0, 0);

        // タイマーのカウント値[ms]
        private int m_timerCount = 0;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FullScreenTimerImpl(GraphicsViewerForm form) {
            m_parentForm = form;
            if (Configuration.Current.GraphicsViewerFullScreenAutoHideCursor || Configuration.Current.GraphicsViewerFullScreenAutoHideInfo) {
                m_fullScreenWaitTimer = new Timer();
                m_fullScreenWaitTimer.Interval = TIMER_INTERVAL;
                m_fullScreenWaitTimer.Start();
                m_fullScreenWaitTimer.Tick += new EventHandler(FullScreenWaitTimer_Tick);
                m_parentForm.GraphicsViewerPanel.MouseMove += new MouseEventHandler(GraphicsViewerPanel_MouseMove);
                m_parentForm.GraphicsViewerPanel.MouseEnter += new EventHandler(GraphicsViewerPanel_MouseEnter);
                m_parentForm.GraphicsViewerPanel.MouseLeave += new EventHandler(GraphicsViewerPanel_MouseLeave);
                m_isHideCursor = false;
                m_isHideInfo = false;
                m_lastMousePosition = Cursor.Position;
            }
        }

        //=========================================================================================
        // 機　能：タイマーの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_fullScreenWaitTimer != null) {
                ResetHideState();
                m_fullScreenWaitTimer.Stop();
                m_fullScreenWaitTimer.Tick -= new EventHandler(FullScreenWaitTimer_Tick);
                m_fullScreenWaitTimer.Dispose();
                m_fullScreenWaitTimer = null;
                m_parentForm.GraphicsViewerPanel.MouseMove -= new MouseEventHandler(GraphicsViewerPanel_MouseMove);
                m_parentForm.GraphicsViewerPanel.MouseEnter -= new EventHandler(GraphicsViewerPanel_MouseEnter);
                m_parentForm.GraphicsViewerPanel.MouseLeave -= new EventHandler(GraphicsViewerPanel_MouseLeave);
                m_parentForm.Cursor = Cursors.Default;
            }
        }

        //=========================================================================================
        // 機　能：マウスがウィンドウが外れたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerPanel_MouseLeave(object sender, EventArgs evt) {
            ResetHideState();
            if (m_fullScreenWaitTimer != null) {
                m_fullScreenWaitTimer.Stop();
            }
        }

        //=========================================================================================
        // 機　能：マウスがウィンドウに入ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerPanel_MouseEnter(object sender, EventArgs evt) {
            if (m_fullScreenWaitTimer != null) {
                m_fullScreenWaitTimer.Start();
            }
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void GraphicsViewerPanel_MouseMove(object sender, MouseEventArgs evt) {
            Point currentPosition = Cursor.Position;
            if (Math.Abs(currentPosition.X - m_lastMousePosition.X) > MOUSE_MOVE_THRETHOLD ||
                Math.Abs(currentPosition.Y - m_lastMousePosition.Y) > MOUSE_MOVE_THRETHOLD) {
                ResetHideState();
            }
            m_lastMousePosition = currentPosition;
        }

        //=========================================================================================
        // 機　能：マウスカーソルや情報を隠す状態を元に戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ResetHideState() {
            if (m_isHideCursor) {
                m_parentForm.Cursor = Cursors.Default;
                m_isHideCursor = false;
            }
            if (m_isHideInfo) {
                m_parentForm.GraphicsViewerPanel.Invalidate();
                m_isHideInfo = false;
            }
            m_timerCount = 0;
        }

        //=========================================================================================
        // 機　能：タイマーの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FullScreenWaitTimer_Tick(object sender, EventArgs evt) {
            if (m_isHideCursor && m_isHideInfo) {
                return;
            }
            m_timerCount += TIMER_INTERVAL;
            if (m_timerCount > Configuration.Current.GraphicsViewerFullScreenHideTimer) {
                if (Configuration.Current.GraphicsViewerFullScreenAutoHideCursor) {
                    m_isHideCursor = true;
                    m_parentForm.Cursor = UIIconManager.NullCursor;
                }
                if (Configuration.Current.GraphicsViewerFullScreenAutoHideInfo) {
                    m_isHideInfo = true;
                    m_parentForm.GraphicsViewerPanel.Invalidate();
                }
            }
        }

        //=========================================================================================
        // 機　能：画像が変更されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnSlideChange() {
            m_isHideInfo = false;
            m_timerCount = 0;
        }

        //=========================================================================================
        // 機　能：画像情報を表示する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DrawInformation(GraphicsViewerGraphics g, ImageInfo currentImage) {
            if (m_isHideInfo || Configuration.Current.GraphicsViewerFullScreenHideInfoAlways) {
                return;
            }

            StatusBarFormatter formatter = new StatusBarFormatter(m_parentForm.GraphicsViewerPanel, currentImage);
            List<string> arrInfo = new List<string>();
            if (formatter.FileName != "") {
                arrInfo.Add(formatter.FileName);
            }
            if (m_parentForm.SlideShowMode) {
                arrInfo.Add(formatter.SlideShowPosition);
            }
            if (currentImage.MarkState != 0) {
                arrInfo.Add(formatter.MarkState);
            }
            GraphicsViewerFilterSetting filterSetting = m_parentForm.GraphicsViewerPanel.FilterSetting;
            if (filterSetting.FilterList.Count > 0 && filterSetting.UseFilter) {
                arrInfo.Add(formatter.FilterText);
            }
            arrInfo.Add(formatter.ZoomRatio + " " + formatter.SizeInfo);

            const int CX_MARGIN = 16;
            const int CY_MARGIN = 16;
            int cyFont = g.GraphicsViewerFileNameFont.Height;
            int yPos = CY_MARGIN;
            for (int i = 0; i < arrInfo.Count; i++) {
                for (int x = 0; x < 3; x++) {
                    for (int y = 0; y < 3; y++) {
                        if (x == 1 && y == 1) {
                            continue;
                        }
                        Rectangle rc = new Rectangle(0 + x, yPos + y, m_parentForm.GraphicsViewerPanel.Width - CX_MARGIN, cyFont);
                        g.Graphics.DrawString(arrInfo[i], g.GraphicsViewerFileNameFont, g.GraphicsViewerTextShadowBrush, rc, g.StringFormatRight);
                    }
                }
                Rectangle rcCenter = new Rectangle(0 + 1, yPos + 1, m_parentForm.GraphicsViewerPanel.Width - CX_MARGIN, cyFont);
                g.Graphics.DrawString(arrInfo[i], g.GraphicsViewerFileNameFont, g.GraphicsViewerTextBrush, rcCenter, g.StringFormatRight);
                yPos += cyFont;
            }
        }
    }
}
