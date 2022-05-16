using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileTask.Management;
using ShellFiler.Properties;
using ShellFiler.Document;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：メインツールバー内のタスクマネージャボタン
    //=========================================================================================
    class MainToolbarTaskMenuButton {
        // タスクマネージャボタンのデフォルトリソース
        private const int DEFAULT_TASK_MANAGER_INDEX = 0;

        // 親ツールバー
        private MainToolbarStrip m_parent;
        
        // タスクマネージャアニメーションリソースのインデックス
        private int m_currentTaskManagerIndex = DEFAULT_TASK_MANAGER_INDEX;

        // タスクマネージャボタンの領域
        private Rectangle m_rcTaskManagerButton;

        // タスクマネージャのボタン枠の状態
        private TaskManagerButtonState m_taskManagerButtonState = TaskManagerButtonState.None;

        // アニメーション用タイマー
        private Timer m_timerAnimation;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親ツールバー
        // 戻り値：なし
        //=========================================================================================
        public MainToolbarTaskMenuButton(MainToolbarStrip parent) {
            m_parent = parent;
            m_timerAnimation = new Timer();
            m_timerAnimation.Interval = 100;
            m_timerAnimation.Tick += new EventHandler(this.TimerAnimation_Tick);
            m_timerAnimation.Stop();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
            Program.Document.BackgroundTaskManager.TaskChanged += new BackgroundTaskManager.TaskChangedEventHandler(this.TaskManager_StateChanged);
        }

        //=========================================================================================
        // 機　能：フォームのサイズが変更されたときの処理を行う
        // 引　数：[in]rcButton   ツールバー内でのボタンの領域
        // 戻り値：なし
        //=========================================================================================
        public void OnSizeChanged(Rectangle rcButton) {
            m_rcTaskManagerButton = rcButton;
        }

        //=========================================================================================
        // 機　能：タスクマネージャの状態が変わった時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TaskManager_StateChanged(object sender, BackgroundTaskManager.TaskChangedEventArgs evt) {
            int taskCount = Program.Document.BackgroundTaskManager.Count;
            if (taskCount == 0 && m_timerAnimation.Enabled) {
                m_timerAnimation.Stop();
                m_currentTaskManagerIndex = DEFAULT_TASK_MANAGER_INDEX;
                m_parent.Invalidate();
                m_parent.Update();
            } else if (taskCount > 0 && !m_timerAnimation.Enabled) {
                m_timerAnimation.Start();
                m_timerAnimation.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：アニメーションタイマーのイベント受信時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TimerAnimation_Tick(object sender, EventArgs e) {
            using (Graphics g = m_parent.CreateGraphics()) {
                m_currentTaskManagerIndex = (m_currentTaskManagerIndex + 1) % (UIIconManager.ImageListBGManager.Length - 1) + 1;
                DrawButton(g);
            }
        }

        //=========================================================================================
        // 機　能：描画イベント受信時の処理を行う
        // 引　数：[in]g  グラフィックス（nullの場合は内部で取得）
        // 戻り値：なし
        //=========================================================================================
        public void DrawButton(Graphics g) {
            if (m_rcTaskManagerButton == null) {
                return;     // デザイナーでの表示
            }

            int cx = m_rcTaskManagerButton.Width - 1;
            int cy = Math.Min(m_parent.Height - 1, m_rcTaskManagerButton.Height - 1);
            using (Bitmap bmp = new Bitmap(cx, cy)) {
                using (Graphics gBmp = Graphics.FromImage(bmp)) {
                    ToolStripManager.Renderer.DrawToolStripBackground(new ToolStripRenderEventArgs(gBmp, m_parent));
                    switch (m_taskManagerButtonState) {
                        case TaskManagerButtonState.None:               // 通常状態
                            gBmp.DrawImage(UIIconManager.ImageListBGManager[m_currentTaskManagerIndex], 0, 0);
                            break;
                        case TaskManagerButtonState.DrawingFrame:       // 枠を描画中
                            gBmp.DrawImage(UIIconManager.ImageListBGManager[m_currentTaskManagerIndex], 0, 0);
                            gBmp.DrawLine(SystemPens.ControlLightLight, 0, 0, cx - 1, 0);
                            gBmp.DrawLine(SystemPens.ControlLightLight, 0, 0, 0, cy - 1);
                            gBmp.DrawLine(SystemPens.ControlDark, 0, cy - 1, cx - 1, cy - 1);
                            gBmp.DrawLine(SystemPens.ControlDark, cx - 1, 0, cx - 1, cy - 1);
                            break;
                        case TaskManagerButtonState.ButtonDown:        // ボタンを押している状態
                            gBmp.DrawImage(UIIconManager.ImageListBGManager[m_currentTaskManagerIndex], 1, 1);
                            gBmp.DrawLine(SystemPens.ControlDark, 0, 0, cx - 1, 0);
                            gBmp.DrawLine(SystemPens.ControlDark, 0, 0, 0, cy - 1);
                            gBmp.DrawLine(SystemPens.ControlLightLight, 0, cy - 1, cx - 1, cy - 1);
                            gBmp.DrawLine(SystemPens.ControlLightLight, cx - 1, 0, cx - 1, cy - 1);
                            break;
                    }
                }
                g.DrawImage(bmp, m_rcTaskManagerButton.Left, m_rcTaskManagerButton.Top);
            }
        }
        
        //=========================================================================================
        // 機　能：マウスボタンが押されたときの処理を行う
        // 引　数：[in]evt  マウスイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseDown(MouseEventArgs evt) {
            OnMouseMove(evt, true);
        }

        //=========================================================================================
        // 機　能：マウスボタンが離されたときの処理を行う
        // 引　数：[in]evt  マウスイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseUp(MouseEventArgs evt) {
            OnMouseMove(evt, false);
            if (m_rcTaskManagerButton.Contains(evt.X, evt.Y)) {
                m_taskManagerButtonState = TaskManagerButtonState.None;
                m_parent.Invalidate();
                m_parent.Update();
                Program.MainWindow.OnUICommand(UICommandSender.MainToolbar, UICommandItem.TaskManager);
            }
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]evt        マウスイベント（情報がないときnull）
        // 　　　　[in]isCapture  マウスがキャプチャ状態にあるときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseMove(MouseEventArgs evt, bool isCapture) {
            TaskManagerButtonState oldState = m_taskManagerButtonState;
            if (evt != null && m_rcTaskManagerButton.Contains(evt.X, evt.Y)) {
                if (isCapture) {
                    // ボタンが押されている状態でタスクバーボタンの上
                    m_taskManagerButtonState = TaskManagerButtonState.ButtonDown;
                } else {
                    // ボタンが押されていない状態でタスクバーボタンの上
                    m_taskManagerButtonState = TaskManagerButtonState.DrawingFrame;
                }
            } else {
                if (isCapture) {
                    // ボタンが押されている状態でタスクバーボタン外
                    m_taskManagerButtonState = TaskManagerButtonState.None;
                } else {
                    // ボタンが押されていない状態でタスクバーボタン外
                    m_taskManagerButtonState = TaskManagerButtonState.None;
                }
            }
            if (oldState != m_taskManagerButtonState) {
                m_parent.Invalidate();
                m_parent.Update();
            }
        }

        //=========================================================================================
        // 機　能：ボタンが領域を離れたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseLeave(EventArgs evt) {
            OnMouseMove(null, false);
        }

        //=========================================================================================
        // 列挙子：タスクマネージャボタンの状態
        //=========================================================================================
        private enum TaskManagerButtonState {
            // 通常状態
            None,
            // 枠を描画中
            DrawingFrame,
            // ボタンを押している状態
            ButtonDown,
        }
    }
}
