using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.MonitoringViewer;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.MonitoringViewer {

    //=========================================================================================
    // クラス：モニタリングビューアの文字列検索ダイアログ
    //=========================================================================================
    public partial class MonitoringSearchDialog : Form {
        // 検索の実行用インターフェイス
        private IncrementalSearchLogic m_parentLogic;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parentLogic   検索の実行用インターフェイス
        // 戻り値：なし
        //=========================================================================================
        public MonitoringSearchDialog(IncrementalSearchLogic parentLogic) {
            InitializeComponent();
            m_parentLogic = parentLogic;

            Point pt = GetDialogPos(true, this, m_parentLogic.MonitoringViewerForm);
            this.Left = pt.X;
            this.Top = pt.Y;

            this.ActiveControl = this.textBoxSearchStr;
            this.checkBoxCompareHead.Checked = false;
            MatrixData data = parentLogic.MonitoringViewer.MatrixData;
            if (data.SearchKeyword != null) {
                this.textBoxSearchStr.Text = data.SearchKeyword.Keyword;
            }
        }

        //=========================================================================================
        // 機　能：ダイアログの表示位置を返す
        // 引　数：[in]bottom   ダイアログをウィンドウの下に表示するときtrue、上に表示するときfalse
        // 　　　　[in]dialog   表示するダイアログ
        // 　　　　[in]parent   親ウィンドウ 
        // 戻り値：ダイアログ表示位置の左上の位置
        //=========================================================================================
        public static Point GetDialogPos(bool bottom, Form dialog, Form parent) {
            const int CX_MARGIN = 32;
            const int CY_MARGIN1 = 56;
            const int CY_MARGIN2 = 64;
            Point pt;
            if (bottom) {
                pt = new Point(parent.Right - dialog.Width - CX_MARGIN, parent.Bottom - dialog.Height - CY_MARGIN1);
            } else {
                pt = new Point(parent.Right - dialog.Width - CX_MARGIN, parent.Top + CY_MARGIN2);
            }
            return pt;
        }

        //=========================================================================================
        // 機　能：ダイアログが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MonitoringSearchDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            m_parentLogic.OnCloseDialog();
        }

        //=========================================================================================
        // 機　能：検索文字列が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxSearchStr_TextChanged(object sender, EventArgs evt) {
            Search(IncrementalSearchOperation.FromTop, Resources.DlgMonitorSearch_NotHit);
        }

        //=========================================================================================
        // 機　能：各コントロールにフォーカスが設定されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SubControl_Enter(object sender, EventArgs evt) {
            // 常に検索文字列にフォーカスを設定
            this.ActiveControl = this.textBoxSearchStr;
        }

        //=========================================================================================
        // 機　能：キー入力時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxSearchStr_KeyDown(object sender, KeyEventArgs evt) {
            if (evt.Alt && evt.KeyCode == Keys.T) {
                this.checkBoxCompareHead.Checked = !this.checkBoxCompareHead.Checked;
                evt.SuppressKeyPress = true;
            } else if (evt.KeyCode == Keys.Up) {
                buttonUp_Click(this, null);
                evt.SuppressKeyPress = true;
            } else if (evt.KeyCode == Keys.Down) {
                buttonDown_Click(this, null);
                evt.SuppressKeyPress = true;
            } else if (evt.KeyCode == Keys.Cancel) {
                Close();
                evt.SuppressKeyPress = true;
            }
        }

        //=========================================================================================
        // 機　能：カーソル上移動時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonUp_Click(object sender, EventArgs evt) {
            Search(IncrementalSearchOperation.MoveUp, Resources.DlgMonitorSearch_NotHitCanNotMove);
        }

        //=========================================================================================
        // 機　能：カーソル下移動時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs evt) {
            Search(IncrementalSearchOperation.MoveDown, Resources.DlgMonitorSearch_NotHitCanNotMove);
        }

        //=========================================================================================
        // 機　能：検索処理を実行する
        // 引　数：[in]operation     カーソル位置に対する操作
        // 　　　　[in]errorMessage  見つからなかったときに表示するエラーメッセージ
        // 戻り値：なし
        //=========================================================================================
        private void Search(IncrementalSearchOperation operation, string errorMessage) {
            // 検索を実行
            string searchStr = this.textBoxSearchStr.Text;
            MonitoringSearchCondition condition;
            if (searchStr == "") {
                condition = null;
            } else {
                condition = new MonitoringSearchCondition(searchStr, this.checkBoxCompareHead.Checked);
            }
            bool hit = m_parentLogic.SearchString(condition, operation);

            // ステータスバーに反映
            MonitoringViewerStatusBar statusBar = m_parentLogic.MonitoringViewerForm.StatusBar;
            if (condition == null) {
                // クリア
                statusBar.RefreshStatusBar();
            } else if (!hit) {
                string message = string.Format(errorMessage, searchStr);
                statusBar.ShowErrorMessage(message, FileOperationStatus.LogLevel.Info, IconImageListID.MonitoringViewer_Search);
                statusBar.RefreshStatusBar();
            } else {
                statusBar.RefreshStatusBar();
            }
        }

        //=========================================================================================
        // 機　能：キャンセルボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            Close();
        }
    }
}
