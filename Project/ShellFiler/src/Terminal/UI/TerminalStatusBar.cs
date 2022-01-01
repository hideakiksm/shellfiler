using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.UI;
using ShellFiler.UI.Log;

namespace ShellFiler.Terminal.UI {

    //=========================================================================================
    // クラス：ターミナルでのステータスバー
    //=========================================================================================
    public partial class TerminalStatusBar : StatusStrip {

        // 左側の領域
        private ToolStripStatusLabel toolLabelLeft;

        // 右側の領域
        private ToolStripStatusLabel toolLabelRight;

        // エラーメッセージ表示の実装
        private StatusBarErrorMessageImpl m_errorMessageImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TerminalStatusBar() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]terminalPanel   情報を表示するパネル
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(TerminalPanel terminalPanel) {
            this.ImageList = UIIconManager.IconImageList;

            this.Padding = new Padding(this.Padding.Left, this.Padding.Top, this.Padding.Left, this.Padding.Bottom); 
            this.SizingGrip = false;
            
            // 左側の領域
            this.toolLabelLeft = new ToolStripStatusLabel();
            this.toolLabelLeft.Name = "toolLabelRight";
            this.toolLabelLeft.Size = new System.Drawing.Size(118, 19);
            this.toolLabelLeft.Margin = new Padding(0, 3, 0, 0);
            this.toolLabelLeft.Spring = true;
            this.toolLabelLeft.Text = "";
            this.toolLabelLeft.TextAlign = ContentAlignment.MiddleLeft;
            this.toolLabelLeft.ImageAlign = ContentAlignment.MiddleLeft;
            this.toolLabelLeft.Image = null;

            // 右側の領域
            this.toolLabelRight = new ToolStripStatusLabel();
            this.toolLabelRight.AutoToolTip = true;
            this.toolLabelRight.Margin = new Padding(0, 3, 0, 0);
            this.toolLabelRight.Name = "toolLabelRight";
            this.toolLabelRight.Size = new Size(118, 19);
            this.toolLabelRight.Text = "";
            this.toolLabelRight.TextAlign = ContentAlignment.MiddleRight;
            
            this.Items.AddRange(new ToolStripItem[] {this.toolLabelLeft, this.toolLabelRight});

            m_errorMessageImpl = new StatusBarErrorMessageImpl(this, this.components, this.toolLabelLeft, null);
        }

        //=========================================================================================
        // 機　能：エラーメッセージを更新する
        // 引　数：[in]message   エラーメッセージ
        // 　　　　[in]level     エラーのレベル
        // 　　　　[in]icon      使用するアイコン
        // 　　　　[in]wait      表示時間
        // 戻り値：なし
        // メ　モ：UIスレッドから呼び出す
        //=========================================================================================
        public void ShowErrorMessage(string message, FileOperationStatus.LogLevel level, IconImageListID icon, StatusBarErrorMessageImpl.DisplayTime wait) {
            m_errorMessageImpl.ShowErrorMessageUI(message, level, icon, wait);
        }

        //=========================================================================================
        // 機　能：マウスの選択が変更されたときの処理を行う
        // 引　数：[in]type    通知の種類
        // 　　　　[in]param   通知に対するパラメータ
        // 戻り値：なし
        //=========================================================================================
        public void StatusSelectionChanged(LogViewContainerStatusType type, object param) {
            switch (type) {
                case LogViewContainerStatusType.StartSelection:
                    break;
                case LogViewContainerStatusType.SelectionChange:
                    LogViewSelectionRange selection = (LogViewSelectionRange)param;
                    SetStatusSelection(selection);
                    break;
                case LogViewContainerStatusType.CancelSelection:
                    this.toolLabelRight.Text = "";
                    break;
            }
        }

        //=========================================================================================
        // 機　能：選択範囲の情報をステータスバーに反映する
        // 引　数：[in]selection  選択範囲の情報
        // 戻り値：なし
        //=========================================================================================
        private void SetStatusSelection(LogViewSelectionRange selection) {
            string label;
            if (selection.StartLine == selection.EndLine) {
                label = string.Format(Resources.TerminalStatusSelectColumn,
                                      selection.StartLine, selection.StartColumn + 1, selection.EndLine, selection.EndColumn + 1,
                                      selection.EndColumn - selection.StartColumn + 1);
            } else {
                label = string.Format(Resources.TerminalStatusSelectLine,
                                      selection.StartLine, selection.StartColumn + 1, selection.EndLine, selection.EndColumn + 1,
                                      selection.EndLine - selection.StartLine + 1);
            }
            this.toolLabelRight.Text = label;
        }
    }
}
