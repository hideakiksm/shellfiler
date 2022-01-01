using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.Provider;

namespace ShellFiler.Api {

    //=========================================================================================
    // インターフェース：ファイル操作中のコンテキスト情報
    //=========================================================================================
    public class FileOperationRequestContext {
        // SFTPのコンテキスト情報（SFTP以外はnull）
        private SFTPRequestContext m_sftpRequestContext = null;
        
        // シェルのコンテキスト情報
        private ShellRequestContext m_shellRequestContext = null;

        // 仮想フォルダのコンテキスト情報（仮想フォルダ以外はnull）
        private VirtualRequestContext m_virtualRequestContext = null;

        // バックグラウンドタスクID
        private BackgroundTaskID m_backgroundTaskId;

        // 転送元のファイル一覧のコンテキスト情報（null:まだ一覧が確定していない）
        private IFileListContext m_srcFileListContext = null;

        // 転送先のファイル一覧のコンテキスト情報（null:まだ一覧が確定していない）
        private IFileListContext m_destFileListContext = null;

        // キャンセルされたときシグナル状態になるイベント
        private ManualResetEvent m_cancelEvent;

        // キャンセルされた理由（キャンセルされていないときnull）
        public CancelReason m_cancelReason;

        // ダイアログの親になるフォーム
        public Form m_parentDialog;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]taskId          バックグラウンドタスクのID
        // 　　　　[in]srcFileSystem   転送元ファイルシステムのID
        // 　　　　[in]destFileSystem  転送先ファイルシステムのID（null：転送先の概念がない）
        // 　　　　[in]srcFileListCtx  転送元のファイル一覧のコンテキスト情報（null:まだ一覧が確定していない）
        // 　　　　[in]destFileListCtx 転送先のファイル一覧のコンテキスト情報（null:まだ一覧が確定していない）
        // 　　　　[in]cancelEvent     外部指定のキャンセルイベント（外部指定せず、内部で新規に作成するときnull）
        // 戻り値：なし
        //=========================================================================================
        public FileOperationRequestContext(BackgroundTaskID taskId, FileSystemID srcFileSystem, FileSystemID destFileSystem, IFileListContext srcFileListCtx, IFileListContext destFileListCtx, ManualResetEvent cancelEvent) {
            m_backgroundTaskId = taskId;
            if (srcFileSystem == FileSystemID.SFTP || destFileSystem == FileSystemID.SFTP) {
                m_sftpRequestContext = new SFTPRequestContext();
            }
            if (srcFileSystem == FileSystemID.SSHShell || destFileSystem == FileSystemID.SSHShell) {
                m_shellRequestContext = new ShellRequestContext();
            }
            if (FileSystemID.IsVirtual(srcFileSystem) || FileSystemID.IsVirtual(destFileSystem)) {
                m_virtualRequestContext = new VirtualRequestContext();
            }
            m_srcFileListContext = srcFileListCtx;
            m_destFileListContext = destFileListCtx;
            m_cancelReason = CancelReason.None;
            if (cancelEvent == null) {
                m_cancelEvent = new ManualResetEvent(false);
            } else {
                m_cancelEvent = cancelEvent;
            }
            m_parentDialog = Program.MainWindow;
        }

        //=========================================================================================
        // 機　能：コンテキスト情報を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_sftpRequestContext != null) {
                m_sftpRequestContext.Dispose();
                m_sftpRequestContext = null;
            }
            if (m_virtualRequestContext != null) {
                m_virtualRequestContext.Dispose();
                m_virtualRequestContext = null;
            }
            if (m_srcFileListContext != null) {
                if (m_srcFileListContext is VirtualFileListContext) {
                    VirtualFolderInfo virtualInfo = ((VirtualFileListContext)m_srcFileListContext).VirtualFolderInfo;
                    Program.Document.TemporaryManager.VirtualManager.EndUsingVirtualFolder(virtualInfo);
                }
                m_srcFileListContext = null;
            }
            if (m_destFileListContext != null) {
                if (m_srcFileListContext is VirtualFileListContext) {
                    VirtualFolderInfo virtualInfo = ((VirtualFileListContext)m_destFileListContext).VirtualFolderInfo;
                    Program.Document.TemporaryManager.VirtualManager.EndUsingVirtualFolder(virtualInfo);
                }
                m_destFileListContext = null;
            }
        }

        //=========================================================================================
        // 機　能：FileProviderを再設定する
        // 引　数：[in]srcProvider  転送元ファイルの情報
        // 　　　　[in]destProvider 転送先ファイルの情報
        // 戻り値：なし
        //=========================================================================================
        public void ResetFileProvider(IFileProviderSrc srcProvider, IFileProviderDest destProvider) {
        }

        //=========================================================================================
        // 機　能：ファイル転送のタスクをキャンセルする
        // 引　数：[in]reason  キャンセルした理由
        // 戻り値：なし
        //=========================================================================================
        public void SetCancel(CancelReason reason) {
            m_cancelEvent.Set();
            m_cancelReason = reason;
            if (m_sftpRequestContext != null) {
                m_sftpRequestContext.Dispose();
            }
        }

        //=========================================================================================
        // プロパティ：SFTPのコンテキスト情報
        //=========================================================================================
        public SFTPRequestContext SFTPRequestContext {
            get {
                return m_sftpRequestContext;
            }
        }

        //=========================================================================================
        // プロパティ：シェルのコンテキスト情報
        //=========================================================================================
        public ShellRequestContext ShellRequestContext {
            get {
                return m_shellRequestContext;
            }
        }

        //=========================================================================================
        // プロパティ：仮想フォルダのコンテキスト情報（仮想フォルダ以外はnull）
        //=========================================================================================
        public VirtualRequestContext VirtualRequestContext {
            get {
                return m_virtualRequestContext;
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドタスクID
        //=========================================================================================
        public BackgroundTaskID BackgroundTaskId {
            get {
                return m_backgroundTaskId;
            }
        }

        //=========================================================================================
        // プロパティ：転送元のファイル一覧のコンテキスト情報（null:まだ一覧が確定していない）
        //=========================================================================================
        public IFileListContext SrcFileListContext {
            get {
                return m_srcFileListContext;
            }
        }

        //=========================================================================================
        // プロパティ：転送先のファイル一覧のコンテキスト情報（null:まだ一覧が確定していない）
        //=========================================================================================
        public IFileListContext DestFileListContext {
            get {
                return m_destFileListContext;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル操作タスクのときtrue
        //=========================================================================================
        public bool IsFileOperationTask {
            get {
                return (m_backgroundTaskId != BackgroundTaskID.NullId);
            }
        }

        //=========================================================================================
        // プロパティ：キャンセルされたときシグナル状態になるイベント
        //=========================================================================================
        public EventWaitHandle CancelEvent {
            get {
                return m_cancelEvent;
            }
        }

        //=========================================================================================
        // プロパティ：キャンセルされた理由（キャンセルされていないときnull）
        //=========================================================================================
        public CancelReason CancelReason {
            get {
                return m_cancelReason;
            }
        }

        //=========================================================================================
        // プロパティ：処理を中止するときtrue
        //=========================================================================================
        public bool IsCancel {
            get {
                if (m_cancelReason != CancelReason.None) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        //=========================================================================================
        // プロパティ：ダイアログの親になるフォーム
        //=========================================================================================
        public Form ParentDialog {
            get {
                return m_parentDialog;
            }
            set {
                m_parentDialog = value;
            }
        }
    }
}
