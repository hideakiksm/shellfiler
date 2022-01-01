using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ShellFiler.Api;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：仮想フォルダファイルシステムでのファイル一覧のコンテキスト情報
    //=========================================================================================
    class VirtualFileListContext : IFileListContext {
        // 仮想フォルダの情報
        private VirtualFolderInfo m_virtualFolderInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]virtualInfo   仮想フォルダの情報
        // 戻り値：なし
        //=========================================================================================
        public VirtualFileListContext(VirtualFolderInfo virtualInfo) {
            m_virtualFolderInfo = virtualInfo;
        }

        //=========================================================================================
        // 機　能：タブの複製時にクローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public IFileListContext CloneForTabCopy() {
            return new VirtualFileListContext(m_virtualFolderInfo.CloneForTabCopy());
        }

        //=========================================================================================
        // 機　能：バックグラウンドタスク実行用にクローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public IFileListContext CloneForBackgroundTask() {
            return new VirtualFileListContext(m_virtualFolderInfo.CloneForBackgroundTask());
        }

        //=========================================================================================
        // 機　能：実行時のファイルを取得する
        // 引　数：[in]path  元のファイル
        // 戻り値：仮想フォルダ等を変換した後のファイル
        //=========================================================================================
        public string GetExecuteLocalPath(string path) {
            return m_virtualFolderInfo.GetExecuteLocalPath(path);
        }

        //=========================================================================================
        // 機　能：実行時のファイル一覧を取得する
        // 引　数：[in]pathList  元のファイル一覧
        // 戻り値：仮想フォルダ等を変換した後のファイル一覧
        //=========================================================================================
        public List<string> GetExecuteLocalPathList(List<string> pathList) {
            return m_virtualFolderInfo.GetExecuteLocalPathList(pathList);
        }

        //=========================================================================================
        // プロパティ：仮想フォルダの情報
        //=========================================================================================
        public VirtualFolderInfo VirtualFolderInfo {
            get {
                return m_virtualFolderInfo;
            }
        }
    }
}
