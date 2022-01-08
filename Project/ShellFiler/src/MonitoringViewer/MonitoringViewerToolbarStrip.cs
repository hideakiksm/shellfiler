﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Dialog.KeyOption;

namespace ShellFiler.MonitoringViewer {

    //=========================================================================================
    // クラス：ファイルビューアのツールバー
    //=========================================================================================
    public partial class MonitoringViewerToolbarStrip : ToolStrip {
        // ツールバーの実装
        private ToolBarImpl m_toolbarImpl;

        // 2ストロークボタン
        private TwoStrokeButton m_twoStrokeButton;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MonitoringViewerToolbarStrip() {
            InitializeComponent();
            m_twoStrokeButton = new TwoStrokeButton(this, CommandUsingSceneType.MonitoringViewer);
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]viewer コマンドの送信先
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(IMonitoringViewer viewer) {
            m_twoStrokeButton.Initialize();

            ToolBarImpl.IsValidToolBarItemDelegate isValid = delegate(ToolbarItemSetting item) {
                if (item is MonitoringViewerToolbarItemSetting) {
                    MonitoringViewerToolbarItemSetting monitoringItem = (MonitoringViewerToolbarItemSetting)item;
                    if (viewer.Mode == monitoringItem.AvailableMode) {
                        return true;
                    } else {
                        return false;
                    }
                }
                return true;
            };
            m_toolbarImpl = new ToolBarImpl(this, UICommandSender.MonitoringViewerToolBar, viewer.MonitoringViewerForm);
            m_toolbarImpl.AddButtonsFromSetting(Program.Document.ToolbarSetting.MonitoringViewerToolbarItemList, Program.Document.KeySetting.MonitoringViewerKeyItemList, isValid);
        }

        //=========================================================================================
        // 機　能：フォームのサイズが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileViewerToolbarStrip_SizeChanged(object sender, EventArgs evt) {
            const int MARGIN = 2;
            int cxTwo = UIIconManager.TwoStrokeKeyNormal.Width;
            int cyTwo = UIIconManager.TwoStrokeKeyNormal.Height;

            Rectangle rcTwoStroke = new Rectangle(ClientRectangle.Right - cxTwo - MARGIN, (this.Height - cyTwo) / 2, cxTwo, cyTwo);
            m_twoStrokeButton.OnSizeChanged(rcTwoStroke);
        }

        //=========================================================================================
        // 機　能：描画イベント受信時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileViewerToolbarStrip_Paint(object sender, PaintEventArgs evt) {
            m_twoStrokeButton.DrawButton(evt.Graphics);
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