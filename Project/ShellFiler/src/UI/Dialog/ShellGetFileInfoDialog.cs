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
    // クラス：SSHシェルファイルシステムでファイル情報を取得するダイアログ
    //=========================================================================================
    public partial class ShellGetFileInfoDialog : Form, IBackgroundWaitNotify {
        // 作業スレッドからのコールバックインターフェイス
        private BackgroundWaitCallback m_callback;

        // 取得対象のパス
        private string m_targetFilePath;

        // 転送元のファイルシステムのID
        private FileSystemID m_fileSystemId;

        // 作業スレッド
        private GetFileInfoThread m_getFileInfoThread;
        
        // 実行結果のステータス（nullのとき作業スレッドでの実行結果が定まっていない）
        private FileOperationStatus m_status;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]path            取得対象のパス
        // 　　　　[in]srcFilePath     転送元のファイルシステムのID
        // 　　　　[in]srcFileListCtx  ファイル一覧のコンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        private ShellGetFileInfoDialog(string path, FileSystemID srcFileSystem, IFileListContext srcFileListCtx) {
            InitializeComponent();
            m_targetFilePath = path;
            m_fileSystemId = srcFileSystem;
            FileOperationRequestContext context = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, srcFileSystem, FileSystemID.None, srcFileListCtx, null, null);
            context.ParentDialog = this;
            m_callback = new BackgroundWaitCallback(this, context);
        }

        //=========================================================================================
        // 機　能：リクエスト処理に応じた展開処理を行う
        // 引　数：[in]path            取得対象のパス
        // 　　　　[in]srcFilePath     転送元のファイルシステムのID
        // 　　　　[in]srcFileListCtx  ファイル一覧のコンテキスト情報
        // 　　　　[in]result          取得した属性を返す変数
        // 戻り値：なし
        //=========================================================================================
        public static FileOperationStatus GetFileInfo(string path, FileSystemID srcFileSystem, IFileListContext srcFileListCtx, out IFile result) {
            result = null;
            ShellGetFileInfoDialog dialog = new ShellGetFileInfoDialog(path, srcFileSystem, srcFileListCtx);
            dialog.ShowDialog(Program.MainWindow);
            if (dialog.m_status != null) {
                result = dialog.m_getFileInfoThread.ResultFileInfo;
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
        private void ShellGetFileInfoDialog_Load(object sender, EventArgs evt) {
            m_getFileInfoThread = new GetFileInfoThread(m_callback, m_targetFilePath, m_fileSystemId);
            m_getFileInfoThread.StartThread();
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
            if (m_getFileInfoThread.ResultFileInfo == null) {
                status = FileOperationStatus.FileNotFound;
            }
            m_status = status;
            if (!status.Succeeded) {
                InfoBox.Warning(this, Resources.DlgGetFileInfo_Error, status.Message);
            }
            Close();
        }
    }
}
