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
using ShellFiler.UI;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.MonitoringViewer {

    //=========================================================================================
    // クラス：機能実行中ダイアログ
    //=========================================================================================
    public partial class MonitoringCommandWaitDialog : Form {
        // 実行中のタスクのID
        private BackgroundTaskID m_loadingTaskId;

        // 実行が完了したときtrue（スレッドバグ防止：先にCompletedしたときは初期化で終了）
        private bool m_completed = false;

        // コマンドの実行で発生したエラー（エラーが発生していないときnull）
        private string m_errorInfo = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]iconId    実行中の機能のアイコンID
        // 　　　　[in]iconDisp  実行中の機能の表示名
        // 戻り値：なし
        //=========================================================================================
        public MonitoringCommandWaitDialog(IconImageListID iconId, string commandDisp) {
            InitializeComponent();
            this.pictureBox.Image = UIIconManager.IconImageList.Images[(int)iconId];
            this.labelCommandName.Text = commandDisp;
        }

        //=========================================================================================
        // 機　能：ダイアログが読み込まれたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MonitoringCommandWaitDialog_Load(object sender, EventArgs evt) {
            if (m_completed) {
                OnCommandCompleted(m_errorInfo);
            }
        }

        //=========================================================================================
        // 機　能：中止ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            this.buttonCancel.Enabled = false;
            Program.Document.BackgroundTaskManager.CancelBackgroundTask(m_loadingTaskId, false);
        }

        //=========================================================================================
        // 機　能：コマンドの実行が完了したときの処理を行う
        // 引　数：[in]errorInfo  処理中に発生したエラーメッセージ（エラーが発生していないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void OnCommandCompleted(string errorInfo) {
            m_completed = true;
            m_errorInfo = errorInfo;
            this.buttonCancel.Enabled = false;
            if (errorInfo == null) {
                DialogResult = DialogResult.OK;
                Close();
            } else {
                InfoBox.Warning(this, "{0}", errorInfo);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        //=========================================================================================
        // プロパティ：実行中のタスクのID
        //=========================================================================================
        public BackgroundTaskID LoadingTaskId {
            set {
                m_loadingTaskId = value;
            }
        }
    }
}
