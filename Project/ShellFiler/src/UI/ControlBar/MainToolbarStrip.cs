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
using ShellFiler.Document.Setting;
using ShellFiler.UI.Dialog.KeyOption;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：メインとなるツールバー
    //=========================================================================================
    public partial class MainToolbarStrip : ToolStrip {
        // ツールバーの実装
        private ToolBarImpl m_toolbarImpl;

        // タスクマネージャボタン（使用しないときnull）
        private MainToolbarTaskMenuButton m_taskManagerButton;

        // 2ストロークボタン
        private TwoStrokeButton m_twoStrokeButton;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MainToolbarStrip() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]sender        コマンドの送信元の識別用
        // 　　　　[in]commandTarget コマンドの送信先
        // 　　　　[in]commandScene  コマンドの利用シーン
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(UICommandSender sender, IUICommandTarget commandTarget, CommandUsingSceneType commandScene) {
            m_twoStrokeButton = new TwoStrokeButton(this, commandScene);
            m_twoStrokeButton.Initialize();
            if (commandScene == CommandUsingSceneType.FileList) {
                m_taskManagerButton = new MainToolbarTaskMenuButton(this);
                m_taskManagerButton.Initialize();
            }
            
            List<ToolbarItemSetting> toolBarItems = Program.Document.ToolbarSetting.GetItemList(commandScene);
            KeyItemSettingList keySetting = Program.Document.KeySetting.GetKeyList(commandScene);
            m_toolbarImpl = new ToolBarImpl(this, sender, commandTarget);
            m_toolbarImpl.AddButtonsFromSetting(toolBarItems, keySetting, null);

            if (commandScene == CommandUsingSceneType.FileList) {
                Program.MainWindow.CursorLRChanged += new MainWindowForm.CursorLRChangedHandler(MainWindowForm_CursorLRChanged);
            }
            MainToolbarStrip_SizeChanged(null, null);
        }

        //=========================================================================================
        // 機　能：フォームのサイズが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainToolbarStrip_SizeChanged(object sender, EventArgs evt) {
            if (m_twoStrokeButton == null) {
                return;
            }
            const int MARGIN = 2;
            int cxTask = UIIconManager.CxBgManageAnimation;
            int cyTask = UIIconManager.CyBgManageAnimation;
            int cxTwo = UIIconManager.TwoStrokeKeyNormal.Width;
            int cyTwo = UIIconManager.TwoStrokeKeyNormal.Height;

            if (m_taskManagerButton != null) {
                Rectangle rcTaskManager = new Rectangle(ClientRectangle.Right - cxTask - MARGIN, 0, cxTask, cyTask);
                Rectangle rcTwoStroke = new Rectangle(rcTaskManager.Left - cxTwo - MARGIN, (this.Height - cyTwo) / 2, cxTwo, cyTwo);

                m_taskManagerButton.OnSizeChanged(rcTaskManager);
                m_twoStrokeButton.OnSizeChanged(rcTwoStroke);
            } else {
                Rectangle rcTwoStroke = new Rectangle(ClientRectangle.Right - cxTwo - MARGIN, (this.Height - cyTwo) / 2, cxTwo, cyTwo);
                m_twoStrokeButton.OnSizeChanged(rcTwoStroke);
            }
        }

        //=========================================================================================
        // 機　能：描画イベント受信時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainToolbarStrip_Paint(object sender, PaintEventArgs evt) {
            if (m_taskManagerButton != null) {
                m_taskManagerButton.DrawButton(evt.Graphics);
            }
            m_twoStrokeButton.DrawButton(evt.Graphics);
        }
        
        //=========================================================================================
        // 機　能：ツールバーのステータス状態を更新する
        // 引　数：[in]context   ツールバーの更新状態のコンテキスト
        // 戻り値：なし
        //=========================================================================================
        public void RefreshToolbarStatus(UIItemRefreshContext context) {
            m_toolbarImpl.RefreshToolbarStatus(context);
        }

        //=========================================================================================
        // 機　能：ツールバーのドライブ一覧を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshToolbarDriveList() {
            m_toolbarImpl.RefreshToolbarDriveList();
        }

        //=========================================================================================
        // 機　能：マウスボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainToolbarStrip_MouseDown(object sender, MouseEventArgs evt) {
            if (m_taskManagerButton != null) {
                this.Capture = true;
                m_taskManagerButton.OnMouseDown(evt);
            }
        }

        //=========================================================================================
        // 機　能：マウスボタンが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainToolbarStrip_MouseUp(object sender, MouseEventArgs evt) {
            if (m_taskManagerButton != null) {
                this.Capture = false;
                m_taskManagerButton.OnMouseUp(evt);
            }
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント（マウスが領域を離れたときnull）
        // 戻り値：なし
        //=========================================================================================
        private void MainToolbarStrip_MouseMove(object sender, MouseEventArgs evt) {
            if (m_taskManagerButton != null) {
                m_taskManagerButton.OnMouseMove(evt, this.Capture);
            }
        }

        //=========================================================================================
        // 機　能：ボタンが領域を離れたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainToolbarStrip_MouseLeave(object sender, EventArgs evt) {
            if (m_taskManagerButton != null) {
                m_taskManagerButton.OnMouseLeave(evt);
            }
        }

        //=========================================================================================
        // 機　能：メインウィンドウのカーソルの左右に変化が生じたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainWindowForm_CursorLRChanged(object sender, EventArgs evt) {
            m_toolbarImpl.OnCursorLRChanged(Program.Document.CurrentTabPage.IsCursorLeft);
        }

        //=========================================================================================
        // 機　能：2ストロークキーの状態が変わったことを通知する
        // 引　数：[in]newState  新しい状態
        // 戻り値：なし
        //=========================================================================================
        public void TwoStrokeKeyStateChanged(TwoStrokeType newState) {
            m_twoStrokeButton.TwoStrokeKeyStateChanged(newState);
        }
    }
}
