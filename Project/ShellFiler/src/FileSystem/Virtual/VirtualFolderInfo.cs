using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Locale;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.Util;
using ShellFiler.Virtual;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：仮想ディレクトリのフォルダ管理情報
    //=========================================================================================
    public class VirtualFolderInfo {
        // 一番外側のアーカイブにアクセスできるファイルシステム
        private IFileSystem m_baseFileSystem;

        // 仮想フォルダの作業ディレクトリ
        private VirtualFolderTemporaryDirectory m_temporaryInfo;

        // 階層化された仮想フォルダの情報（添字の小さい方が外側のアーカイブ）
        private List<VirtualFolderArchiveInfo> m_virtualFolderItemList = new List<VirtualFolderArchiveInfo>();

        // 実行用作業ディレクトリのID（更新やタブ複製があったときに+1される）
        private int m_virtualExecuteId;

        // このインスタンスがライフサイクル管理をしている実行用仮想フォルダの作業ディレクトリ（null:管理していない）
        private string m_tempDirectoryForExecute = null;

        // 元のファイル一覧のコンテキスト情報
        private IFileListContext m_baseFileListContext;

        // Clone()に対応

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]baseFileSystem   一番外側のアーカイブにアクセスできるファイルシステム
        // 　　　　[in]tempDir          仮想フォルダの作業ディレクトリ
        // 　　　　[in]baseFileListCtx  元のファイル一覧のコンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public VirtualFolderInfo(IFileSystem baseFileSystem, VirtualFolderTemporaryDirectory tempDir, IFileListContext baseFileListCtx) {
            m_baseFileSystem = baseFileSystem;
            m_temporaryInfo = tempDir;
            m_baseFileListContext = baseFileListCtx;
            m_virtualExecuteId = Program.Document.TemporaryManager.VirtualManager.GetNextVirtualExecuteId();
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private VirtualFolderInfo() {
        }

        //=========================================================================================
        // 機　能：更新処理の開始時にクローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public VirtualFolderInfo CloneForUpdate() {
            VirtualFolderInfo clone = new VirtualFolderInfo();
            clone.m_baseFileSystem = m_baseFileSystem;
            clone.m_temporaryInfo = m_temporaryInfo;
            foreach (VirtualFolderArchiveInfo archiveInfo in m_virtualFolderItemList) {
                clone.m_virtualFolderItemList.Add(archiveInfo.CloneArchiveInfo());
            }
            clone.m_virtualExecuteId = Program.Document.TemporaryManager.VirtualManager.GetNextVirtualExecuteId();
            clone.m_tempDirectoryForExecute = null;
            return clone;
        }

        //=========================================================================================
        // 機　能：タブの複製時にクローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public VirtualFolderInfo CloneForTabCopy() {
            VirtualFolderInfo clone = new VirtualFolderInfo();
            clone.m_baseFileSystem = m_baseFileSystem;
            clone.m_temporaryInfo = m_temporaryInfo;
            foreach (VirtualFolderArchiveInfo archiveInfo in m_virtualFolderItemList) {
                clone.m_virtualFolderItemList.Add(archiveInfo.CloneArchiveInfo());
            }
            clone.m_virtualExecuteId = Program.Document.TemporaryManager.VirtualManager.GetNextVirtualExecuteId();
            clone.m_tempDirectoryForExecute = null;
            return clone;
        }

        //=========================================================================================
        // 機　能：バックグラウンドタスク実行用にクローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public VirtualFolderInfo CloneForBackgroundTask() {
            VirtualFolderInfo clone = new VirtualFolderInfo();
            clone.m_baseFileSystem = m_baseFileSystem;
            clone.m_temporaryInfo = m_temporaryInfo;
            foreach (VirtualFolderArchiveInfo archiveInfo in m_virtualFolderItemList) {
                clone.m_virtualFolderItemList.Add(archiveInfo.CloneArchiveInfo());
            }
            clone.m_virtualExecuteId = m_virtualExecuteId;
            clone.m_tempDirectoryForExecute = m_tempDirectoryForExecute;
            return clone;
        }

        //=========================================================================================
        // 機　能：仮想ディレクトリの情報を追加する
        // 引　数：[in]archive  追加するアーカイブ情報
        // 戻り値：なし
        //=========================================================================================
        public void AddVirtualArchive(VirtualFolderArchiveInfo archive) {
            m_virtualFolderItemList.Add(archive);
        }

        //=========================================================================================
        // 機　能：指定されたディレクトリを処理するための仮想フォルダのアーカイブ項目を返す
        // 引　数：[in]dir  調べるディレクトリ名
        // 戻り値：処理できるときtrue
        //=========================================================================================
        public VirtualFolderArchiveInfo GetVirtualFolderItem(string dir) {
            for (int i = m_virtualFolderItemList.Count - 1; i >= 0; i--) {
                if (m_virtualFolderItemList[i].IsIncludedAbsolutePath(dir)) {
                    return m_virtualFolderItemList[i];
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：指定されたアーカイブ位置より深い階層にあるアーカイブの情報を削除する
        // 引　数：[in]archiveForUse  次に使い続ける仮想フォルダ
        // 戻り値：なし
        //=========================================================================================
        public void DeleteSubVirtualArchive(VirtualFolderArchiveInfo archiveForUse) {
            for (int i = m_virtualFolderItemList.Count - 1; i >= 0; i--) {
                if (m_virtualFolderItemList[i] == archiveForUse) {
                    break;
                }
                m_virtualFolderItemList.RemoveAt(i);
            }
        }

        //=========================================================================================
        // 機　能：実行用の一時ディレクトリをアタッチする
        // 引　数：[in]dir  実行用の一時ディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public void AttachTempDirectoryForExecute(string dir) {
            if (m_tempDirectoryForExecute != null && dir.ToLower() != m_tempDirectoryForExecute.ToLower()) {
                Program.Abort("仮想フォルダの一時ディレクトリ管理に問題があります。\nexist={0}, new={1}", m_tempDirectoryForExecute, dir);
            }
            m_tempDirectoryForExecute = dir;
        }

        //=========================================================================================
        // 機　能：ライフサイクルが管理状態にある一時ファイルのリストを返す
        // 引　数：なし
        // 戻り値：一時ファイルのリスト（最後が「\」のものはディレクトリ、それ以外はファイル）
        //=========================================================================================
        public List<string> GetManagedTemporaryList() {
            List<string> result = new List<string>();
            if (m_tempDirectoryForExecute != null) {
                result.Add(m_tempDirectoryForExecute);
            }
            return result;
        }

        //=========================================================================================
        // 機　能：実行ファイルのパス名を仮想フォルダ内作業フォルダのパス名に変換する
        // 引　数：[in]dispFile  実行ファイルの表示用パス名
        // 戻り値：仮想フォルダ内作業フォルダのパス名
        //=========================================================================================
        public string GetExecuteLocalPath(string dispFile) {
            string rootDispLower = MostInnerArchive.DisplayPathArchive.ToLower();
            string dispFileLower = dispFile.ToLower();
            if (!dispFileLower.StartsWith(rootDispLower)) {
                return null;
            }
            if (m_tempDirectoryForExecute == null) {
                Program.Abort("仮想フォルダ作業領域の実行状態が異常です。");
            }
            string result = m_tempDirectoryForExecute + dispFile.Substring(rootDispLower.Length);
            return result;
        }

        //=========================================================================================
        // 機　能：実行ファイルのパス名を仮想フォルダ内作業フォルダのパス名に変換する
        // 引　数：[in]pathList    元のファイルパスのリスト
        // 戻り値：仮想フォルダ内作業フォルダのパス名のリスト
        //=========================================================================================
        public List<string> GetExecuteLocalPathList(List<string> pathList) {
            List<string> result = new List<string>();
            foreach (string path in pathList) {
                string localPath = GetExecuteLocalPath(path);
                result.Add(localPath);
            }
            return result;
        }

        //=========================================================================================
        // プロパティ：一番外側のアーカイブにアクセスできるファイルシステム
        //=========================================================================================
        public IFileSystem BaseFileSystem {
            get {
                return m_baseFileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：仮想フォルダの作業ディレクトリ
        //=========================================================================================
        public VirtualFolderTemporaryDirectory TemporaryInfo {
            get {
                return m_temporaryInfo;
            }
        }

        //=========================================================================================
        // プロパティ：一番内側のアーカイブ
        //=========================================================================================
        public VirtualFolderArchiveInfo MostInnerArchive {
            get {
                return m_virtualFolderItemList[m_virtualFolderItemList.Count - 1];
            }
        }

        //=========================================================================================
        // プロパティ：階層化された仮想フォルダの情報（添字の小さい方が外側のアーカイブ）
        //=========================================================================================
        public List<VirtualFolderArchiveInfo> VirtualFolderItemList {
            get {
                return m_virtualFolderItemList;
            }
        }

        //=========================================================================================
        // プロパティ：実行用作業ディレクトリのID（更新やタブ複製があったときに+1される）
        //=========================================================================================
        public int VirtualExecuteId {
            get {
                return m_virtualExecuteId;
            }
        }

        //=========================================================================================
        // プロパティ：元のファイル一覧のコンテキスト情報
        //=========================================================================================
        public IFileListContext BaseFileListContext {
            get {
                return m_baseFileListContext;
            }
        }
    }
}
