using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;

namespace ShellFiler.Virtual {

    //=========================================================================================
    // クラス：テンポラリ領域の監視クラス
    //=========================================================================================
    public class TemporaryWatcher : BaseThread {
        // 監視対象
        private TemporaryManager m_temporaryManager;

        // 監視対象のプロセス
        private List<ProcessSpacePair> m_watchProcess = new List<ProcessSpacePair>();

        // m_watchAddProcessにデータの追加があったときシグナル状態になるイベント
        private AutoResetEvent m_watchProcessAddEvent = new AutoResetEvent(false);

        // ファイルの更新監視
        FileSystemWatcher m_fileSystemWatcher;
        
        // 終了イベント
        private ManualResetEvent m_endEvent = new ManualResetEvent(false);

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]manager  監視対象
        // 戻り値：なし
        //=========================================================================================
        public TemporaryWatcher(TemporaryManager manager) : base("TemporaryWatcher", 1) {
            m_temporaryManager = manager;

            // ファイル更新監視の初期化
            m_fileSystemWatcher = new FileSystemWatcher();
            m_fileSystemWatcher.Path = manager.AllVirtualFolderRoot;
            m_fileSystemWatcher.IncludeSubdirectories = true;
            m_fileSystemWatcher.NotifyFilter = (NotifyFilters.LastWrite);
            m_fileSystemWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_Changed);
            m_fileSystemWatcher.EnableRaisingEvents = true;
        }

        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            m_fileSystemWatcher.Dispose();
            m_fileSystemWatcher = null;
            m_endEvent.Set();
            JoinThread();
        }

        //=========================================================================================
        // 機　能：監視を開始する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void StartWatch() {
            StartThread();
        }

        //=========================================================================================
        // 機　能：監視対象のプロセスを追加する
        // 引　数：[in]process          登録するプロセス
        // 　　　　[in]processWaitList  プロセスの終了を待っている作業領域のリスト
        // 戻り値：なし
        // メ　モ：ファイル操作スレッドで実行する
        //=========================================================================================
        public void AddProcess(Process process, List<LocalExecuteTemporarySpace> processWaitList) {
            lock (this) {
                ProcessSpacePair pair = new ProcessSpacePair(process, processWaitList);
                m_watchProcess.Add(pair);
                m_watchProcessAddEvent.Set();
            }
        }

        //=========================================================================================
        // 機　能：スレッドの入り口
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：バックグラウンドスレッドで実行する
        //=========================================================================================
        protected override void ThreadEntry() {
            while (true) {
                // プロセスの終了を待つ
                List<WaitHandle> waitEventList = new List<WaitHandle>();
                waitEventList.Add(m_endEvent);
                waitEventList.Add(m_watchProcessAddEvent);
                lock (this) {
                    foreach (ProcessSpacePair process in m_watchProcess) {
                        ManualResetEvent processEvent = new ManualResetEvent(false);
                        processEvent.SafeWaitHandle = new SafeWaitHandle(process.Process.Handle, false);
                        waitEventList.Add(processEvent);
                    }
                }
//                waitEventList.Dispose();
                int index = WaitHandle.WaitAny(waitEventList.ToArray());
                if (index == 0) {
                    return;                 // 終了イベント
                } else if (index == 1) {
                    continue;               // リセットイベント
                }

                ProcessSpacePair targetProcess;
                lock (this) {
                    int processIndex = index - 2;
                    targetProcess = m_watchProcess[processIndex];
                    m_watchProcess.RemoveAt(processIndex);
                }
                m_temporaryManager.OnProcessEnd(targetProcess.ProcessSpaceList);
                targetProcess.Process.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：監視対象のファイルに更新があったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        // メ　モ：FileSystemWatcher内部の監視スレッドで実行する
        //=========================================================================================
        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs evt) {
            m_temporaryManager.OnFileUpdate(evt.FullPath);
        }

        //=========================================================================================
        // クラス：監視対象のプロセスと一時領域のリストの対応
        //=========================================================================================
        private class ProcessSpacePair {
            // 監視対象のプロセス
            public Process Process;

            // プロセスの終了を待っている一時領域のリスト
            public List<LocalExecuteTemporarySpace> ProcessSpaceList;

            //=========================================================================================
            // 機　能：監視対象のファイルに更新があったときの処理を行う
            // 引　数：[in]process          監視対象のプロセス
            // 　　　　[in]processWaitList  プロセスの終了を待っている一時領域のリスト
            // 戻り値：なし
            //=========================================================================================
            public ProcessSpacePair(Process process, List<LocalExecuteTemporarySpace> processWaitList) {
                Process = process;
                ProcessSpaceList = processWaitList;
            }
        }
    }
}
