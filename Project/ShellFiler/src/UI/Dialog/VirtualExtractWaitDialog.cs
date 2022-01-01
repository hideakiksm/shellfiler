using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Locale;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.Virtual;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：仮想フォルダの展開中待機ダイアログ
    //=========================================================================================
    public partial class VirtualExtractWaitDialog : Form, IBackgroundWaitNotify {
        // 処理内容を保存した引数
        private VirtualExtractArg m_argument;
        
        // 作業スレッドからのコールバックインターフェイス
        private BackgroundWaitCallback m_callback;

        // 作業スレッド
        private ExtractThread m_extractThread;
        
        // 実行結果のステータス（nullのとき作業スレッドでの実行結果が定まっていない）
        private FileOperationStatus m_status;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]arg   展開処理のリクエスト／レスポンスパラメータ
        // 戻り値：なし
        //=========================================================================================
        public VirtualExtractWaitDialog(VirtualExtractArg arg) {
            InitializeComponent();
            m_argument = arg;
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = 100;
            this.progressBar.Value = 0;
            FileOperationRequestContext context = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.Virtual, FileSystemID.None, m_argument.FileListContext, null, null);
            context.ParentDialog = this;
            m_callback = new BackgroundWaitCallback(this, context);
            context.VirtualRequestContext.VirtualExtractDialogCallback = m_callback;
        }

        //=========================================================================================
        // 機　能：リクエスト処理に応じた展開処理を行う
        // 引　数：[in]arg   展開処理のリクエスト／レスポンスパラメータ
        // 戻り値：なし
        //=========================================================================================
        public static FileOperationStatus Extract(VirtualExtractArg arg) {
            VirtualExtractWaitDialog dialog = new VirtualExtractWaitDialog(arg);
            dialog.ShowDialog(Program.MainWindow);
            if (dialog.m_status != null) {
                return dialog.m_status;
            } else {
                return FileOperationStatus.Canceled;
            }
        }
        
        //=========================================================================================
        // 機　能：ダイアログが初期化されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void VirtualExtractWaitDialog_Load(object sender, EventArgs evt) {
            m_extractThread = new ExtractThread(m_callback, m_argument);
            m_extractThread.StartThread();
        }
        
        //=========================================================================================
        // 機　能：キャンセルボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            m_callback.Cancel(CancelReason.User);
            this.buttonCancel.Enabled = false;
        }

        //=========================================================================================
        // 機　能：進捗率を更新する
        // 引　数：[in]ratio     進捗率（0～100）
        // 　　　　[in]filePath  展開中のファイルパス
        // 戻り値：なし
        // メ　モ：delegateによりUIスレッドで実行する
        //=========================================================================================
        public void UpdateProgress(int ratio, string filePath) {
            BaseThread.InvokeProcedureByMainThread(new UpdateProgressDelegate(UpdateProgressUI), ratio, filePath);
        }
        delegate void UpdateProgressDelegate(int ratio, string filePath);
        private void UpdateProgressUI(int ratio, string filePath) {
            ratio = Math.Max(0, Math.Min(100, ratio));
            this.textBoxCurrent.Text = filePath;
            this.labelRatio.Text = ratio + "%";
            this.progressBar.Value = ratio;
        }

        //=========================================================================================
        // 機　能：展開処理が終了したときの処理を行う
        // 引　数：[in]status  展開処理のステータス
        // 戻り値：なし
        // メ　モ：delegateによりUIスレッドで実行する
        //=========================================================================================
        public void ExtractCompleted(FileOperationStatus status) {
            BaseThread.InvokeProcedureByMainThread(new ExtractCompletedDelegate(ExtractCompletedUI), status);
        }
        delegate void ExtractCompletedDelegate(FileOperationStatus status);
        private void ExtractCompletedUI(FileOperationStatus status) {
            m_status = status;
            this.textBoxCurrent.Text = "";
            Close();
        }
    }
}
