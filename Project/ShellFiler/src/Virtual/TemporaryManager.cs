using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Virtual {

    //=========================================================================================
    // クラス：テンポラリファイルの管理クラス
    // テンポラリディレクトリの構成
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<Pid1>                     他のプロセス（構成は下記同様）
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<Pid2>                     他のプロセス（構成は下記同様）
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<curr>                     現在のプロセス用 m_tempDirectoryRoot
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<curr>\TempFile            一時ファイル用 m_tempDirectoryForFile
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<curr>\TempExec\Temp1      一時ディレクトリ用1
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<curr>\TempExec\Temp2      一時ディレクトリ用2
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<curr>\Virtual             仮想ディレクトリ用 AllVirtualRootDirectory
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<curr>\Virtual\Temp1       仮想ディレクトリ空間1（外側のアーカイブ単位）
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<curr>\Virtual\Temp1\Arc   仮想ディレクトリ内のアーカイブ
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<curr>\Virtual\Temp1\Exec  仮想ディレクトリの実行領域
    // C:\Users\<user>\AppData\Local\ShellFiler\SFTemp\<curr>\Virtual\Temp2       仮想ディレクトリ空間2（外側のアーカイブ単位）
    //=========================================================================================
    public class TemporaryManager {
        // プロセス終了を無視する最小秒数
        public const int MIN_PROCESS_END_WATCH_SECONDS = 2;

        // ルートのテンポラリフォルダ名
        public const string TEMP_FOLDER_ROOT = "SFTemp\\";
        
        // 仮想ディレクトリの管理クラス
        private VirtualManager m_virtualManager;

        // 一時領域の監視クラス
        private TemporaryWatcher m_temporaryWatcher;

        // 一時ファイル名の払い出しに使用されるID
        private int m_nextFileld = 0;

        // 一時ディレクトリ名の払い出しに使用されるID
        private int m_nextDirectoryld = 0;

        // ローカル実行ディレクトリの払い出しに使用されるID
        private int m_nextLocalExecld = 0;

        // 現在のプロセスのテンポラリディレクトリルート（最後はセパレータ）
        private string m_tempDirectoryRoot;

        // 汎用の一時ファイル用ディレクトリ（最後はセパレータ）
        private string m_tempDirectoryForFile;

        // ローカル実行ディレクトリのテンポラリ空間
        private List<LocalTemporaryDirectory> m_localTemporaryDirectory = new List<LocalTemporaryDirectory>();
        
        // ローカル実行ディレクトリのテンポラリ空間
        private List<LocalExecuteTemporarySpace> m_localExecuteSpace = new List<LocalExecuteTemporarySpace>();

        // 作業領域に変化が生じたときの通知用delegate
        public delegate void TemporaryChangedEventHandler(object sender, TemporaryChangedEventArgs evt); 

        // 作業領域に変化が生じたときに通知するイベント
        public event TemporaryChangedEventHandler TemporaryChanged; 

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TemporaryManager() {
            string tempDirApp = Path.Combine(DirectoryManager.GetTemporaryDirectory(Configuration.TemporaryDirectory), TEMP_FOLDER_ROOT);
            string tempDirCurrent = tempDirApp + Process.GetCurrentProcess().Id + "\\";
            m_tempDirectoryRoot = tempDirCurrent;
            m_tempDirectoryForFile = tempDirCurrent + "TempFile\\";

            // 一時ディレクトリを作成
            string dirName = null;
            try {
                dirName = m_tempDirectoryForFile;
                if (!Directory.Exists(dirName)) {
                    Directory.CreateDirectory(dirName);
                }
                dirName = AllVirtualFolderRoot;
                if (!Directory.Exists(dirName)) {
                    Directory.CreateDirectory(dirName);
                }
            } catch (Exception e) {
                throw new SfException(SfException.WorkDirectoryCreate, dirName, e.Message);
            }

            m_virtualManager = new VirtualManager(this);
            m_temporaryWatcher = new TemporaryWatcher(this);
            m_temporaryWatcher.StartWatch();
        }
        
        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            m_virtualManager.Dispose();
            m_temporaryWatcher.Dispose();
            foreach (LocalExecuteTemporarySpace space in m_localExecuteSpace) {
                space.Dispose();
            }
            m_localExecuteSpace.Clear();
        }

        //=========================================================================================
        // 機　能：不要なディレクトリを削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DeleteGarbage() {
            bool success;

            // 一時ディレクトリそのものがない場合は作成して終了
            string tempDirApp = Path.Combine(DirectoryManager.GetTemporaryDirectory(Configuration.TemporaryDirectory), TEMP_FOLDER_ROOT);
            if (!Directory.Exists(tempDirApp)) {
                Directory.CreateDirectory(m_tempDirectoryForFile);
                return;
            }

            // 実行中の他のプロセスIDを取得
            HashSet<string> runningPath = new HashSet<string>();            // 使用中のテンポラリ一覧
            List<Process> allProcess = OSUtils.GetAllShellFilerProcess();
            foreach (Process process in allProcess) {
                string processTempDir = (tempDirApp + process.Id + "\\").ToLower();
                runningPath.Add(processTempDir);
            }

            // ゴミをピックアップ
            List<string> garbageList = new List<string>();
            List<string> garbageEmptyList = new List<string>();
            string[] directoryList = Directory.GetDirectories(tempDirApp);
            foreach (string directory in directoryList) {
                // 実行中のものは除外
                string compDir = GenericFileStringUtils.CompleteDirectoryName(directory, "\\");
                if (runningPath.Contains(compDir.ToLower())) {
                    continue;
                }
                // ファイルの合計サイズを計算
                bool existFile;
                success = FileUtils.IsExistNotEmpty(directory, out existFile);
                if (!success) {
                    existFile = true;
                }
                if (existFile) {
                    garbageList.Add(directory);
                } else {
                    garbageEmptyList.Add(directory);
                }
            }

            // 空のフォルダを削除
            DeleteTemporaryDirectory(garbageEmptyList);

            if (garbageList.Count > 0) {
                // 一時ディレクトリのゴミを発見
                DeleteTemporaryDialog dialog = new DeleteTemporaryDialog(garbageList);
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK) {
                    DeleteTemporaryDirectory(garbageList);
                }
            }

            // 一時ディレクトリを作成
            if (!Directory.Exists(m_tempDirectoryForFile)) {
                Directory.CreateDirectory(m_tempDirectoryForFile);
            }
        }

        //=========================================================================================
        // 機　能：テンポラリディレクトリを削除する
        // 引　数：[in]tempList  削除するディレクトリのリスト
        // 戻り値：なし
        //=========================================================================================
        private void DeleteTemporaryDirectory(List<string> tempList) {
            foreach (string tempDir in tempList) {
                FileUtils.DeleteDirectory(tempDir);
            }
        }

        //=========================================================================================
        // 機　能：今回使用したテンポラリディレクトリを削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DeleteCurrentTemporary() {
            FileUtils.DeleteDirectory(m_tempDirectoryRoot);
        }

        //=========================================================================================
        // 機　能：テンポラリファイルのファイル名を返す
        // 引　数：なし
        // 戻り値：テンポラリファイルのファイル名
        //=========================================================================================
        public string GetTemporaryFile() {
            int nextId = Interlocked.Add(ref m_nextFileld, 1);
            return m_tempDirectoryForFile + "file" + nextId;
        }

        //=========================================================================================
        // 機　能：テンポラリファイルが作成されるフォルダ名を返す
        // 引　数：なし
        // 戻り値：テンポラリフォルダ名
        //=========================================================================================
        public string GetTemporaryFileFolder() {
            return m_tempDirectoryForFile;
        }

        //=========================================================================================
        // 機　能：指定フォルダ内にテンポラリファイルのファイル名を返す
        // 引　数：[in]folder   作業フォルダ
        // 　　　　[in]fileBody ファイル名主部（"File"のとき"File1"などになって戻る）
        // 戻り値：テンポラリファイルのファイル名
        //=========================================================================================
        public string GetTemporaryFileInFolder(string folder, string fileBody) {
            int nextId = Interlocked.Add(ref m_nextFileld, 1);
            if (!folder.EndsWith("\\")) {
                folder += "\\";
            }
            return folder + fileBody + nextId;
        }

        //=========================================================================================
        // 機　能：ローカル実行用のテンポラリ空間を作成する
        // 引　数：なし
        // 戻り値：テンポラリディレクトリ
        //=========================================================================================
        public LocalTemporaryDirectory CreateTemporaryDirectory() {
            lock (this) {
                m_nextDirectoryld++;
                string virtualPath = m_tempDirectoryRoot + "TempDir\\Temp" + m_nextDirectoryld + "\\";
                return new LocalTemporaryDirectory(m_nextDirectoryld, virtualPath);
            }
        }

        //=========================================================================================
        // 機　能：ローカル実行用のテンポラリ空間を作成する
        // 引　数：[in]programPath  実行するプログラム名
        // 　　　　[in]nameInfo     表示名の情報
        // 　　　　[in]remoteDir    リモートディレクトリ
        // 　　　　[in]remoteSysId  リモートのファイルシステムのID
        // 　　　　[in]fileListCtx  ファイル一覧のコンテキスト情報
        // 戻り値：テンポラリ空間（呼び出し元でDispose()またはAttachLocalExecuteSpace()を実行）
        //=========================================================================================
        public LocalExecuteTemporarySpace CreateLocalExecuteSpace(string programPath, TemporarySpaceDisplayName nameInfo, string remoteDir, FileSystemID remoteSysId, IFileListContext fileListCtx) {
            lock (this) {
                m_nextLocalExecld++;
                string virtualPath = m_tempDirectoryRoot + "TempExec\\Temp" + m_nextLocalExecld + "\\";
                return new LocalExecuteTemporarySpace(m_nextLocalExecld, virtualPath, remoteDir, remoteSysId, fileListCtx, programPath, nameInfo);
            }
        }

        //=========================================================================================
        // 機　能：ローカル実行用のテンポラリ空間を管理に追加する
        // 引　数：[in]space1  追加するテンポラリ空間１
        // 　　　　[in]space2  追加するテンポラリ空間２
        // 　　　　[in]process 監視対象のプロセス
        // 戻り値：なし
        //=========================================================================================
        public void AttachLocalExecuteSpace(LocalExecuteTemporarySpace space1, LocalExecuteTemporarySpace space2, Process process) {
            if (space1 == null && space2 == null) {
                return;
            }

            List<LocalExecuteTemporarySpace> processWaitList = new List<LocalExecuteTemporarySpace>();
            lock (this) {
                if (space1 != null) {
                    m_localExecuteSpace.Add(space1);
                    processWaitList.Add(space1);
                }
                if (space2 != null) {
                    m_localExecuteSpace.Add(space2);
                    processWaitList.Add(space2);
                }
            }
            m_temporaryWatcher.AddProcess(process, processWaitList);
            if (space1 != null) {
                TemporaryChanged(this, new TemporaryChangedEventArgs(TemporaryChangedEventType.LocalExecuteAdded, space1));
            }
            if (space2 != null) {
                TemporaryChanged(this, new TemporaryChangedEventArgs(TemporaryChangedEventType.LocalExecuteAdded, space2));
            }
        }

        //=========================================================================================
        // 機　能：ローカル実行用のテンポラリ空間を削除する
        // 引　数：[in]space  削除するテンポラリ空間
        // 戻り値：なし
        //=========================================================================================
        public void DeleteLocalExecuteSpace(LocalExecuteTemporarySpace space) {
            lock (this) {
                space.Dispose();
                m_localExecuteSpace.Remove(space);
            }
            TemporaryChanged(this, new TemporaryChangedEventArgs(TemporaryChangedEventType.LocalExecuteDeleted, space));
        }

        //=========================================================================================
        // 機　能：プロセスが終了したときの処理を行う
        // 引　数：[in]processWaitList  プロセスの終了を待っている一時領域のリスト
        // 戻り値：なし
        // メ　モ：監視スレッドから呼び出される
        //=========================================================================================
        public void OnProcessEnd(List<LocalExecuteTemporarySpace> processWaitList) {
            lock (this) {
                foreach (LocalExecuteTemporarySpace space in processWaitList) {
                    space.OnProcessEnd();
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルが更新されたときの処理を行う
        // 引　数：[in]filePath  更新されたファイルのファイルパス
        // 戻り値：なし
        // メ　モ：FileSystemWatcher内部の監視スレッドで実行する
        //=========================================================================================
        public void OnFileUpdate(string filePath) {
            lock (this) {
                filePath = filePath.ToLower();
                LocalExecuteTemporarySpace targetSpace = null;
                foreach (LocalExecuteTemporarySpace space in m_localExecuteSpace) {
                    string spacePath = space.VirtualDirectory.ToLower();
                    if (filePath.StartsWith(spacePath)) {
                        targetSpace = space;
                        break;
                    }
                }
                if (targetSpace != null) {
                    targetSpace.OnFileUpdate(filePath);
                }
            }
        }

        //=========================================================================================
        // 機　能：Dirty状態になっている先頭のLocalExecuteTemporarySpaceを取得する
        // 引　数：なし
        // 戻り値：Dirty状態のLocalExecuteTemporarySpace（Dirty状態がないときnull）
        //=========================================================================================
        public LocalExecuteTemporarySpace GetFirstDirtyLocalExecuteSpace() {
            lock (this) {
                foreach (LocalExecuteTemporarySpace space in m_localExecuteSpace) {
                    if (space.Dirty) {
                        space.Dirty = false;
                        return space;
                    }
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：すべての未編集のLocalExecuteTemporarySpaceを削除する
        // 引　数：なし
        // 戻り値：全件削除したときtrue、使用中のものが残ったときfalse
        //=========================================================================================
        public bool DeleteAllLocalExecuteTemporarySpace() {
            bool deleteAll = true;
            lock (this) {
                List<LocalExecuteTemporarySpace> deleteList = new List<LocalExecuteTemporarySpace>();
                deleteList.AddRange(m_localExecuteSpace);
                foreach (LocalExecuteTemporarySpace target in deleteList) {
                    List<LocalFileInfo> modifiedList;
                    target.CheckModified(out modifiedList);
                    if (modifiedList.Count == 0) {
                        Program.Document.TemporaryManager.DeleteLocalExecuteSpace(target);
                    } else {
                        deleteAll = false;
                    }
                }
            }
            return deleteAll;
        }

        //=========================================================================================
        // プロパティ：現在のプロセスの全仮装フォルダのルート（最後はセパレータ）
        //=========================================================================================
        public string AllVirtualFolderRoot {
            get {
                return m_tempDirectoryRoot + "Virtual\\";
            }
        }

        //=========================================================================================
        // プロパティ：仮想ディレクトリの管理クラス
        //=========================================================================================
        public VirtualManager VirtualManager {
            get {
                return m_virtualManager;
            }
        }

        //=========================================================================================
        // クラス：作業領域の状態変化のイベント種別
        //=========================================================================================
        public enum TemporaryChangedEventType {
            // ローカル実行のテンポラリが追加された
            LocalExecuteAdded,
            
            // ローカル実行のテンポラリが削除された
            LocalExecuteDeleted,
            
            // 仮想ディレクトリのテンポラリが追加された
            VirtualDirectoryAdded,

            // 仮想ディレクトリのテンポラリが削除された
            VirtualDirectoryDeleted,
        }

        //=========================================================================================
        // クラス：作業領域の状態変化のイベント引数
        //=========================================================================================
        public class TemporaryChangedEventArgs {
            // イベントの種類
            private TemporaryChangedEventType m_eventType;
            
            // ローカル実行用のテンポラリ（仮想ディレクトリのときnull）
            private LocalExecuteTemporarySpace m_localExecute;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]eventType   イベントの種類
            // 　　　　[in]localExec   ローカル実行用のテンポラリ（仮想ディレクトリのときnull）
            // 戻り値：なし
            //=========================================================================================
            public TemporaryChangedEventArgs(TemporaryChangedEventType eventType, LocalExecuteTemporarySpace localExec) {
                m_eventType = eventType;
                m_localExecute = localExec;
            }

            //=========================================================================================
            // プロパティ：イベントの種類
            //=========================================================================================
            public TemporaryChangedEventType EventType {
                get {
                    return m_eventType;
                }
            }

            //=========================================================================================
            // プロパティ：ローカル実行用のテンポラリ（仮想ディレクトリのときnull）
            //=========================================================================================
            public LocalExecuteTemporarySpace LocalExecuteTemporarySpace {
                get {
                    return m_localExecute;
                }
            }
        }
    }
}
