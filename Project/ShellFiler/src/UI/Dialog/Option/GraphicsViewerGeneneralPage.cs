using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.GraphicsViewer;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイルビューア＞全般 の設定ページ
    //=========================================================================================
    public partial class GraphicsViewerGeneralPage : UserControl, IOptionDialogPage {
        // 最大化時の情報消去までの時間をトラックバーのつまみに変換するための定数
        private const int TRACK_TO_FULL_SCREEN_HIDE_TIMER = 100;
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        // ドラッグ中のブレーキのきき具合
        private static int[] s_breakingValue = {
            10, 30, 60, 130, 260, 510, 1000, 2000
        };

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerGeneralPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;
            this.numericMaxSize.Minimum = Configuration.MIN_GRAPHICS_VIEWER_MAX_FILE_SIZE / 1024 / 1024;
            this.numericMaxSize.Maximum = Configuration.MAX_GRAPHICS_VIEWER_MAX_FILE_SIZE / 1024 / 1024;
            this.labelMaxSize.Text = string.Format(this.labelMaxSize.Text, this.numericMaxSize.Minimum, this.numericMaxSize.Maximum);

            this.trackBarBraking.Minimum = 0;
            this.trackBarBraking.Maximum = s_breakingValue.Length - 1;
            this.trackBarFullHide.Minimum = Configuration.MIN_GRAPHICS_VIEWER_FULL_SCREEN_HIDE_TIMER / TRACK_TO_FULL_SCREEN_HIDE_TIMER;
            this.trackBarFullHide.Maximum = Configuration.MAX_GRAPHICS_VIEWER_FULL_SCREEN_HIDE_TIMER / TRACK_TO_FULL_SCREEN_HIDE_TIMER;
            this.trackBarFullHide.TickFrequency = 10;

            // コンフィグ値をUIに反映
            SetInitialValue(Configuration.Current);
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            if (this.checkBoxFullAlwaysHide.Checked) {
                this.checkBoxFullInfo.Checked = true;
                this.checkBoxFullInfo.Enabled = false;
            } else {
                this.checkBoxFullInfo.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：UIに初期値を設定する
        // 引　数：[in]config  取得対象のコンフィグ
        // 戻り値：なし
        //=========================================================================================
        private void SetInitialValue(Configuration config) {
            this.numericMaxSize.Value = config.GraphicsViewerMaxFileSize / 1024 / 1024;
            if (config.GraphicsViewerDragInertia == DragInertiaType.NotUse) {
                this.radioButtonNotUse.Checked = true;
            } else if (config.GraphicsViewerDragInertia == DragInertiaType.LocalOnly) {
                this.radioButtonLocal.Checked = true;
            } else {
                this.radioButtonUse.Checked = true;
            }
            this.trackBarBraking.Value = ConfigBreakingToTrackValue(config.GraphicsViewerDragBreaking);
            if (config.GraphicsViewerFilterMode == GraphicsViewerFilterMode.CurrentImageOnly) {
                this.radioButtonFilterImage.Checked = true;
            } else if (config.GraphicsViewerFilterMode == GraphicsViewerFilterMode.CurrentWindowImages) {
                this.radioButtonFilterWindow.Checked = true;
            } else {
                this.radioButtonFilterAll.Checked = true;
            }
            this.trackBarFullHide.Value = config.GraphicsViewerFullScreenHideTimer / TRACK_TO_FULL_SCREEN_HIDE_TIMER;
            this.checkBoxFullCursor.Checked = config.GraphicsViewerFullScreenAutoHideCursor;
            this.checkBoxFullInfo.Checked = config.GraphicsViewerFullScreenAutoHideInfo;
            this.checkBoxFullAlwaysHide.Checked = config.GraphicsViewerFullScreenHideInfoAlways;
        }

        //=========================================================================================
        // 機　能：コンフィグの値をトラックバーのつまみの位置に変換する
        // 引　数：[in]breaking  コンフィグの値
        // 戻り値：つまみの位置
        //=========================================================================================
        private int ConfigBreakingToTrackValue(int breaking) {
            for (int i = 0; i < s_breakingValue[i]; i++) {
                if (s_breakingValue[i] >= breaking) {
                    return i;
                }
            }
            return s_breakingValue.Length - 1;
        }

        //=========================================================================================
        // 機　能：常に情報を隠すかどうかのチェックボックスが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxFullAlwaysHide_CheckedChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // 最大サイズ
            int maxSize = (int)(this.numericMaxSize.Value) * 1024 * 1024;
            success = Configuration.CheckTextViewerMaxFileSize(ref maxSize, m_parent);
            if (!success) {
                return false;
            }

            // ドラッグ中の慣性
            DragInertiaType dragInertia;
            if (this.radioButtonNotUse.Checked) {
                dragInertia = DragInertiaType.NotUse;
            } else if (this.radioButtonLocal.Checked) {
                dragInertia = DragInertiaType.LocalOnly;
            } else {
                dragInertia = DragInertiaType.Always;
            }

            // ドラッグ中のブレーキのきき具合
            int braking = s_breakingValue[Math.Min(s_breakingValue.Length - 1, this.trackBarBraking.Value)];
            success = Configuration.CheckGraphicsViewerDragBreaking(ref braking, m_parent);
            if (!success) {
                return false;
            }

            // フィルター
            GraphicsViewerFilterMode filterMode;
            if (this.radioButtonFilterImage.Checked) {
                filterMode = GraphicsViewerFilterMode.CurrentImageOnly;
            } else if (this.radioButtonFilterWindow.Checked) {
                filterMode = GraphicsViewerFilterMode.CurrentWindowImages;
            } else {
                filterMode = GraphicsViewerFilterMode.AllImages;
            }

            // 最大化
            int hideTime = this.trackBarFullHide.Value * TRACK_TO_FULL_SCREEN_HIDE_TIMER;
            success = Configuration.CheckGraphicsViewerFullScreenHideTimer(ref hideTime, m_parent);
            if (!success) {
                return false;
            }
            bool fullCursor = this.checkBoxFullCursor.Checked;
            bool fullInfo = this.checkBoxFullInfo.Checked;
            bool fullAlwaysHide = this.checkBoxFullAlwaysHide.Checked;

            Configuration.Current.GraphicsViewerMaxFileSize = maxSize;
            Configuration.Current.GraphicsViewerDragInertia = dragInertia;
            Configuration.Current.GraphicsViewerDragBreaking = braking;
            Configuration.Current.GraphicsViewerFilterMode = filterMode;
            Configuration.Current.GraphicsViewerFullScreenHideTimer = hideTime;
            Configuration.Current.GraphicsViewerFullScreenAutoHideCursor = fullCursor;
            Configuration.Current.GraphicsViewerFullScreenAutoHideInfo = fullInfo;
            Configuration.Current.GraphicsViewerFullScreenHideInfoAlways = fullAlwaysHide;

            return true;
        }

        //=========================================================================================
        // 機　能：ページ内の設定をデフォルトに戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetDefault() {
            Configuration org = new Configuration();
            SetInitialValue(org);
        }
    }
}