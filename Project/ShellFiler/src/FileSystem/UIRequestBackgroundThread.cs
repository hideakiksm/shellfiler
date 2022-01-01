using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.UI.Log;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.Util;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace ShellFiler.FileSystem {

    //=========================================================================================
    // クラス：各種のリクエストをバックグラウンドで処理するためのクラス
    //=========================================================================================
    public class UIRequestBackgroundThread : BaseThread {
        // リクエストのキュー（Lastが新しいリクエスト、Firstから処理）
        private LinkedList<RequestQueueEntry> m_requestQueue = new LinkedList<RequestQueueEntry>();

        // キューにリクエストが入ったことを表すイベント
        private AutoResetEvent m_requestEvent = new AutoResetEvent(false);

        // 終了イベント
        private ManualResetEvent m_endEvent = new ManualResetEvent(false);

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public UIRequestBackgroundThread() : base("UIRequestBackgroundThread", 1) {
            StartThread();
        }
        
        //=========================================================================================
        // 機　能：スレッドを終了する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        protected override void OnThreadJoined() {
            m_endEvent.Set();
        }

        //=========================================================================================
        // 機　能：バックグラウンド処理のリクエストを行う
        // 引　数：[in]arg  リクエスト/レスポンス
        // 　　　：[in]path 処理対象のルートパスを認識できるパス
        // 戻り値：なし
        //=========================================================================================
        public void Request(AbstractProcedureArg arg, string path) {
            // リクエストを登録
            RequestQueueEntry entry = new RequestQueueEntry(arg, path);
            lock (this) {
                m_requestQueue.AddLast(entry);
                m_requestEvent.Set();
            }

            // リクエスト処理の終了を待機
            if (!arg.IsAsyncRequest) {
                // 同期呼び出し（通常はここを通らないように設計する）
                WaitHandle[] waitEventList = { m_endEvent, entry.Event };
                int index = WaitHandle.WaitAny(waitEventList);
                if (index == 0) {
                    // 終了イベント
                    arg.Status = FileOperationStatus.Canceled;
                }
                if (entry.Exception != null) {
                    throw entry.Exception;
                }
            } else {
                // 非同期呼び出し
                arg.Status = FileOperationStatus.Success;
            }
        }

        //=========================================================================================
        // 機　能：スレッドの入り口
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected override void ThreadEntry() {
            ExecuteTask();
        }

        //=========================================================================================
        // 機　能：タスクを実行する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ExecuteTask() {
            // サーバに接続
            while (true) {
                // イベントを待つ
                if (m_requestQueue.Count == 0) {
                    WaitHandle[] waitEventList = { m_endEvent, m_requestEvent };
                    int index = WaitHandle.WaitAny(waitEventList);
                    if (index == 0) {
                        return;               // 終了イベント
                    }
                }

                // キューからリクエストを取得
                RequestQueueEntry entry;
                lock (this) {
                    if (m_requestQueue.Count == 0) {
                        continue;
                    }
                    ArrangeRequestQueue();
                    entry = m_requestQueue.First.Value;
                    entry.Exception = null;
                    m_requestQueue.RemoveFirst();
                }

                // Nopは処理しない
                if (entry.ProcedureArg is NopArg) {
                    continue;
                }

                AbstractBackgroundProcedure procedure = AbstractBackgroundProcedure.CreateProcedure(entry.ProcedureArg);
                try {
                    // リクエストを処理
                    if (procedure == null) {
                        // NOP
                        ;
                    } else if (procedure is AbstractSFTPBackgroundProcedure) {
                        // SFTP用プロシージャ
                        RequestForSFTPConnection((AbstractSFTPBackgroundProcedure)procedure, entry);
                    } else if (procedure is AbstractShellBackgroundProcedure) {
                        // SSHシェル用プロシージャ
                        RequestForShellConnection((AbstractShellBackgroundProcedure)procedure, entry);
                    } else {
                        // その他のプロシージャ
                        procedure.Execute(entry.ProcedureArg);
                        if (!entry.ProcedureArg.Status.Succeeded) {
                            procedure.ExecuteOnException(entry.ProcedureArg, null);
                        }
                    }
                } catch (Exception e) {
                    if (entry.ProcedureArg.IsAsyncRequest) {
                        procedure.ExecuteOnException(entry.ProcedureArg, e);
                    } else {
                        entry.Exception = e;
                    }
                }

                // 終了を通知
                entry.Event.Set();
            }
        }

        //=========================================================================================
        // 機　能：SFTPのリクエストを処理する
        // 引　数：[in]procedure   処理対象のSFTP用プロシージャ
        // 　　　　[in,out]entry   処理対象となったリクエストキューのエントリ
        // 戻り値：なし
        //=========================================================================================
        private void RequestForSFTPConnection(AbstractSFTPBackgroundProcedure procedure, RequestQueueEntry entry) {
            FileOperationStatus status;
            SSHConnection connection;
            SSHConnectionManager connectionManager = Program.Document.FileSystemFactory.SSHConnectionManager;
            status = connectionManager.GetSSHConnection(entry.ProcedureArg.RequestContext, entry.Path, out connection);
            if (status != FileOperationStatus.Success) {
                procedure.ExecuteOnException(entry.ProcedureArg, null);
            } else {
                try {
                    procedure.SSHConnection = connection;
                    entry.ProcedureArg.Status = procedure.Execute(entry.ProcedureArg);
                    if (!entry.ProcedureArg.Status.Succeeded) {
                        procedure.ExecuteOnException(entry.ProcedureArg, null);
                    }
                } finally {
                    connection.CompleteRequest();
                }
            }
            entry.ProcedureArg.RequestContext.Dispose();
        }

        //=========================================================================================
        // 機　能：SSHシェルのリクエストを処理する
        // 引　数：[in]procedure   処理対象のSFTP用プロシージャ
        // 　　　　[in,out]entry   処理対象となったリクエストキューのエントリ
        // 戻り値：なし
        //=========================================================================================
        private void RequestForShellConnection(AbstractShellBackgroundProcedure procedure, RequestQueueEntry entry) {
            FileOperationStatus status;
            SSHConnection connection;
            SSHConnectionManager connectionManager = Program.Document.FileSystemFactory.SSHConnectionManager;
            status = connectionManager.GetSSHConnection(entry.ProcedureArg.RequestContext, entry.Path, out connection);
            if (status != FileOperationStatus.Success) {
                procedure.ExecuteOnException(entry.ProcedureArg, null);
            } else {
                try {
                    procedure.SSHConnection = connection;
                    entry.ProcedureArg.Status = procedure.Execute(entry.ProcedureArg);
                    if (!entry.ProcedureArg.Status.Succeeded) {
                        procedure.ExecuteOnException(entry.ProcedureArg, null);
                    }
                } finally {
                    connection.CompleteRequest();
                }
            }
            entry.ProcedureArg.RequestContext.Dispose();
        }

        //=========================================================================================
        // 機　能：リクエストキューを整理する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ArrangeRequestQueue() {
            // GetUIFileListは2重実行しない
            bool isLeftExist = false;
            bool isRightExist = false;
            LinkedListNode<RequestQueueEntry> requestNode = m_requestQueue.Last;
            while (requestNode.Next != null) {
                AbstractProcedureArg request = requestNode.Value.ProcedureArg;
                if (request is SFTPGetUIFileListArg) {
                    if (((SFTPGetUIFileListArg)request).FileList.IsLeftWindow) {
                        if (isLeftExist) {
                            requestNode.Value = new RequestQueueEntry(new NopArg(null), null);
                        }
                        isLeftExist = true;
                    } else {
                        if (isRightExist) {
                            requestNode.Value = new RequestQueueEntry(new NopArg(null), null);
                        }
                        isRightExist = true;
                    }
                }
                requestNode = requestNode.Previous;
            }
        }

        //=========================================================================================
        // クラス：リクエストキューのエントリ
        //=========================================================================================
        private class RequestQueueEntry {
            // リクエスト
            private AbstractProcedureArg m_arg;

            // 処理対象のルートパスを認識できるパス
            private string m_path;
            
            // 処理中に発生した例外
            private Exception m_exception = null;

            // 処理完了イベント
            private ManualResetEvent m_completeEvent;

            //=========================================================================================
            // 機　能：リクエストキューを整理する
            // 引　数：[in]arg  リクエスト/レスポンス
            // 　　　　[in]path 処理対象のルートパスを認識できるパス
            // 戻り値：なし
            //=========================================================================================
            public RequestQueueEntry(AbstractProcedureArg arg, string path) {
                m_arg = arg;
                m_path = path;
                m_completeEvent = new ManualResetEvent(false);
            }

            //=========================================================================================
            // プロパティ：リクエスト/レスポンス
            //=========================================================================================
            public AbstractProcedureArg ProcedureArg {
                get {
                    return m_arg;
                }
            }

            //=========================================================================================
            // プロパティ：処理対象のルートパスを認識できるパス
            //=========================================================================================
            public string Path {
                get {
                    return m_path;
                }
            }

            //=========================================================================================
            // プロパティ：処理中に発生した例外（正常終了したときnull）
            //=========================================================================================
            public Exception Exception {
                get {
                    return m_exception;
                }
                set {
                    m_exception = value;
                }
            }

            //=========================================================================================
            // プロパティ：処理完了イベント
            //=========================================================================================
            public ManualResetEvent Event {
                get {
                    return m_completeEvent;
                }
            }
        }
    }
}
