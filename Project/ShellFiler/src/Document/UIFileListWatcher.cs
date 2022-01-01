using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileTask;
using ShellFiler.Archive;
using ShellFiler.UI.FileList;
using ShellFiler.Virtual;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：表示中のUIファイル一覧の変更監視クラス
    //=========================================================================================
    class UIFileListWatcher {
        // 更新通知のフィルター
        private const NotifyFilters NOTIFY_FILTER = NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
        
        // 自動更新検出用のタイマーの間隔[ms]
        public const int AUTO_UPDATE_TIMER_INTERVAL = 500;

        // 自動更新検出用のタイマー
        private Timer m_timer;

        // 左ウィンドウの監視用ワーカー
        private WatchWorker m_leftWatchWorker;

        // 右ウィンドウの監視用ワーカー
        private WatchWorker m_rightWatchWorker;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public UIFileListWatcher() {
            m_leftWatchWorker = new WatchWorker(this, true);
            m_rightWatchWorker = new WatchWorker(this, false);
        }

        //=========================================================================================
        // 機　能：ファイル一覧が変更されたときの処理を行う
        // 引　数：[in]leftFileList  左ウィンドウのファイル一覧（前回設定を維持するときnull）
        // 　　　　[in]rightFileList 右ウィンドウのファイル一覧（前回設定を維持するときnull）
        // 　　　　[in]onReload      再読込時の通知のときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnUIFileListChanged(UIFileList leftFileList, UIFileList rightFileList, bool onReload) {
            // タイマーを初期化
            if (m_timer == null) {
                m_timer = new Timer();
                m_timer.Interval = AUTO_UPDATE_TIMER_INTERVAL;
                m_timer.Tick += new EventHandler(Timer_Tick);
                m_timer.Start();
            }

            if (leftFileList != null) {
                m_leftWatchWorker.Update(leftFileList, onReload);
            }
            if (rightFileList != null) {
                m_rightWatchWorker.Update(rightFileList, onReload);
            }
        }

        //=========================================================================================
        // 機　能：タイマーの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void Timer_Tick(object sender, EventArgs evt) {
            m_leftWatchWorker.OnTimer();
            m_rightWatchWorker.OnTimer();
        }

        //=========================================================================================
        // クラス：監視機能のワーカークラス
        //=========================================================================================
        private class WatchWorker {
            // 所有オブジェクト
            private UIFileListWatcher m_parent;

            // 左ウィンドウの監視用のときtrue
            private bool m_isLeft;

            // 監視中のディレクトリ（小文字化したもの、Windows以外はnull）
            private string m_targetDirectory = null;

            // ファイルの更新監視（監視中以外はnull）
            private FileSystemWatcher m_fileSystemWatcher = null;

            // ウィンドウ更新までの時間（AUTO_UPDATE_TIMER_INTERVALms間隔、0:更新通知済み）
            private int m_autoUpdateTimer = 0;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent   親オブジェクト
            // 　　　　[in]isLeft   左ウィンドウの監視用のときtrue
            // 戻り値：なし
            //=========================================================================================
            public WatchWorker(UIFileListWatcher parent, bool isLeft) {
                m_parent = parent;
                m_isLeft = isLeft;
            }

            //=========================================================================================
            // 機　能：ファイル一覧が変更されたときの処理を行う
            // 引　数：[in]fileList  左ウィンドウのファイル一覧（前回設定を維持するときnull）
            // 　　　　[in]onReload  再読込時の通知のときtrue
            // 戻り値：なし
            //=========================================================================================
            public void Update(UIFileList fileList, bool onReload) {
                // 新しいディレクトリを取得
                string targetDirectory = null;
                if (FileSystemID.IsWindows(fileList.FileSystem.FileSystemId)) {
                    targetDirectory = fileList.DisplayDirectoryName.ToLower();
                }

                // 更新がない場合は終わり
                if (targetDirectory == m_targetDirectory) {
                    if (onReload) {
                        // 再読込のときはイベントが発生していても処理済みとする
                        lock (this) {
                            m_autoUpdateTimer = 0;
                        }
                    }
                    return;
                }

                // 監視の更新
                bool update = false;
                if (m_targetDirectory != targetDirectory) {
                    if (m_fileSystemWatcher != null) {
                        m_fileSystemWatcher.Dispose();
                        m_fileSystemWatcher = null;
                    }
                    if (targetDirectory != null) {
                        m_targetDirectory = targetDirectory;
                        m_fileSystemWatcher = new FileSystemWatcher();
                        m_fileSystemWatcher.Path = targetDirectory;
                        m_fileSystemWatcher.IncludeSubdirectories = false;
                        m_fileSystemWatcher.NotifyFilter = NOTIFY_FILTER;
                        m_fileSystemWatcher.EnableRaisingEvents = false;
                        m_fileSystemWatcher.Created += new FileSystemEventHandler(FileSystemWatcher_Changed);
                        m_fileSystemWatcher.Deleted += new FileSystemEventHandler(FileSystemWatcher_Changed);
                        m_fileSystemWatcher.Renamed += new RenamedEventHandler(FileSystemWatcher_Renamed);
                        m_fileSystemWatcher.Changed += new FileSystemEventHandler(FileSystemWatcher_Changed);
                        m_targetDirectory = targetDirectory;
                        update = true;
                    }
                }

                // 処理開始
                lock (this) {
                    m_autoUpdateTimer = 0;
                }

                if (update) {
                    m_fileSystemWatcher.EnableRaisingEvents = true;
                }
            }

            //=========================================================================================
            // 機　能：ファイルシステムで変更があったときの処理を行う
            // 引　数：[in]sender   イベントの送信元（内部呼び出しのときnull）
            // 　　　　[in]evt      送信イベント（内部呼び出しのときnull）
            // 戻り値：なし
            //=========================================================================================
            private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs evt) {
                lock (this) {
                    if (Configuration.Current.AutoDirectoryUpdateWait == 0) {
                        m_autoUpdateTimer = 0;
                    } else {
                        m_autoUpdateTimer = (Configuration.Current.AutoDirectoryUpdateWait + AUTO_UPDATE_TIMER_INTERVAL - 1) / AUTO_UPDATE_TIMER_INTERVAL;
                    }
                }
            }

            //=========================================================================================
            // 機　能：ファイルシステムでファイル名の変更があったときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            void FileSystemWatcher_Renamed(object sender, RenamedEventArgs evt) {
                FileSystemWatcher_Changed(null, null);
            }

            //=========================================================================================
            // 機　能：タイマーの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnTimer() {
                bool fire = false;
                lock (this) {
                    if (m_autoUpdateTimer == 0) {
                        ;
                    } else if (m_autoUpdateTimer == 1) {
                        fire = true;
                        m_autoUpdateTimer = 0;
                    } else {
                        m_autoUpdateTimer--;
                    }
                }
                if (fire) {
                    if (m_isLeft) {
                        RefreshUITarget.ReloadDirectory(Program.MainWindow.LeftFileListView, false, null);
                    } else {
                        RefreshUITarget.ReloadDirectory(Program.MainWindow.RightFileListView, false, null);
                    }
                }
            }
        }
    }
}
