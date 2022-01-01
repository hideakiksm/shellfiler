using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;

namespace ShellFiler.Virtual {
    //=========================================================================================
    // クラス：ローカル実行用のテンポラリ空間
    //=========================================================================================
    public class LocalExecuteTemporarySpace {
        // 作業領域の使用開始時刻
        private DateTime m_startTime;

        // 仮想ディレクトリのID
        private int m_virtualId;
        
        // 仮想ディレクトリ
        private string m_virtualDir;

        // リモートディレクトリ
        private string m_remoteDir;
        
        // リモートのファイルシステムのID
        private FileSystemID m_remoteFileSystemId;

        // ファイル一覧のコンテキスト情報
        private IFileListContext m_fileListContext;

        // ローカル実行するプログラム名のフルパス
        private string m_programNamePath;

        // 表示名の情報
        private TemporarySpaceDisplayName m_displayNameInfo;
        
        // ローカルにダウンロードしたファイル
        private List<LocalFileInfo> m_localFileList = new List<LocalFileInfo>();
        
        // 状態に変化があったときtrue
        private bool m_dirty = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]id           仮想ディレクトリのID
        // 　　　　[in]virtualDir   仮想ディレクトリ
        // 　　　　[in]remoteDir    リモートディレクトリ
        // 　　　　[in]remoteSysId  リモートのファイルシステムのID
        // 　　　　[in]programPath  ローカル実行するプログラム名のフルパス
        // 　　　　[in]dispName     表示名の情報
        // 戻り値：なし
        //=========================================================================================
        public LocalExecuteTemporarySpace(int id, string virtualDir, string remoteDir, FileSystemID remoteSysId, IFileListContext fileListCtx, string programPath, TemporarySpaceDisplayName dispName) {
            m_startTime = DateTime.Now;
            m_virtualId = id;
            m_virtualDir = virtualDir;
            m_remoteDir = remoteDir;
            m_remoteFileSystemId = remoteSysId;
            m_fileListContext = fileListCtx;
            m_programNamePath = programPath;
            m_displayNameInfo = dispName;
            try {
                Directory.CreateDirectory(m_virtualDir);
            } catch (Exception e) {
                throw new SfException(SfException.WorkDirectoryCreate, m_virtualDir, e.Message);
            }
        }

        //=========================================================================================
        // 機　能：テンポラリ空間を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            try {
                Directory.Delete(m_virtualDir, true);
            } catch (Exception) {
            }
        }

        //=========================================================================================
        // 機　能：ローカルにダウンロードしたファイルの情報を登録する
        // 引　数：[in]fileInfoList  追加するファイルの情報の一覧
        // 戻り値：なし
        //=========================================================================================
        public void AddLocalFileList(List<LocalFileInfo> fileInfoList) {
            m_localFileList.AddRange(fileInfoList);
            // ダウンロードの最終完了時刻を作業領域の使用開始時刻と見なす
            m_startTime = DateTime.Now;
        }

        //=========================================================================================
        // 機　能：プロセスが終了したときの処理を行う
        // 引　数：[in]process  終了したプロセス
        // 戻り値：なし
        // メ　モ：監視スレッドから呼び出される
        //=========================================================================================
        public void OnProcessEnd() {
            m_dirty = true;
        }
        
        //=========================================================================================
        // 機　能：ファイルが更新されたときの処理を行う
        // 引　数：[in]filePath  更新されたファイルのファイルパス
        // 戻り値：なし
        // メ　モ：FileSystemWatcher内部の監視スレッドで実行する
        //=========================================================================================
        public void OnFileUpdate(string filePath) {
            m_dirty = true;
        }

        //=========================================================================================
        // 機　能：構成ファイルが更新されているかどうか調べる
        // 引　数：[out]modifiedList  更新されたファイルの一覧
        // 戻り値：すべてのファイルを確認できたときtrue
        //=========================================================================================
        public bool CheckModified(out List<LocalFileInfo> modifiedList) {
            bool success = true;
            modifiedList = new List<LocalFileInfo>();
            foreach (LocalFileInfo fileInfo in m_localFileList) {
                try {
                    DateTime writeTime = File.GetLastWriteTime(fileInfo.FilePath);
                    if (writeTime > fileInfo.LastWriteTime) {
                        modifiedList.Add(fileInfo);
                    }
                } catch (Exception) {
                    success = false;
                }
            }
            return success;
        }

        //=========================================================================================
        // プロパティ：作業領域の使用開始時刻
        //=========================================================================================
        public DateTime StartTime {
            get {
                return m_startTime;
            }
        }

        //=========================================================================================
        // プロパティ：ローカル実行するプログラム名（短い名前）
        //=========================================================================================
        public string ProgramNameHintShort {
            get {
                string path = StringUtils.RemoveStringQuote(m_programNamePath.Trim());
                string file = GenericFileStringUtils.GetFileName(path);
                return file;
            }
        }

        //=========================================================================================
        // プロパティ：ローカル実行するプログラム名（長い名前）
        //=========================================================================================
        public string ProgramNameHintLong {
            get {
                string path = StringUtils.RemoveStringQuote(m_programNamePath.Trim());
                return path;
            }
        }

        //=========================================================================================
        // プロパティ：表示名の情報
        //=========================================================================================
        public TemporarySpaceDisplayName DisplayNameInfo {
            get {
                return m_displayNameInfo;
            }
        }

        //=========================================================================================
        // プロパティ：仮想ディレクトリ
        //=========================================================================================
        public string VirtualDirectory {
            get {
                return m_virtualDir;
            }
        }

        //=========================================================================================
        // プロパティ：リモートディレクトリ
        //=========================================================================================
        public string RemoteDirectory {
            get {
                return m_remoteDir;
            }
        }

        //=========================================================================================
        // プロパティ：リモートのファイルシステムのID
        //=========================================================================================
        public FileSystemID RemoteFileSystemId {
            get {
                return m_remoteFileSystemId;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のコンテキスト情報
        //=========================================================================================
        public IFileListContext FileListContext {
            get {
                return m_fileListContext;
            }
        }

        //=========================================================================================
        // プロパティ：ローカルにダウンロードしたファイル一覧
        //=========================================================================================
        public List<LocalFileInfo> LocalFileList {
            get {
                return m_localFileList;
            }
        }

        //=========================================================================================
        // プロパティ：状態に変化があったときtrue
        // メモ：ユーザーインターフェーススレッドから呼ばれ、変化があったときはUIに反映させる
        //=========================================================================================
        public bool Dirty {
            get {
                return m_dirty;
            }
            set {
                m_dirty = value;
            }
        }
    }
}
