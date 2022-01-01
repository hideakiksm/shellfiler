using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Condition;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.Util;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：バックグラウンド処理完了時のUIリフレッシュを行うための情報
    //=========================================================================================
    public class RefreshUITarget {
        // 対象パスのUI(処理しないときnull可)
        private FileListView m_targetView;

        // 反対パスのUI(処理しないときnull可)
        private FileListView m_oppositeView;

        // リフレッシュするモード
        private RefreshMode m_refreshMode;

        // リフレッシュのオプション
        private RefreshOption m_option;
        
        // リフレッシュのオプションに対応するパラメータ
        private object[] m_paramList;

        // リフレッシュ前に実行する処理のdelegate型
        public delegate void PreRefreshDelegate(int successCount, int skipCount, int failCount);
        
        // リフレッシュ前に実行する処理（UIスレッドで実行）
        public event PreRefreshDelegate PreRefreshEvent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]targetView   対象パスのUI(処理しないときnull可)
        // 　　　　[in]oppositeView 反対パスのUI(処理しないときnull可)
        // 　　　　[in]refreshMode  リフレッシュするモード
        // 　　　　[in]option       リフレッシュのオプション
        // 　　　　[in]paramList    リフレッシュのオプションに対応するパラメータ
        // 戻り値：なし
        //=========================================================================================
        public RefreshUITarget(FileListView targetView, FileListView oppositeView, RefreshMode refreshMode, RefreshOption option, params object[] paramList) {
            m_targetView = targetView;
            m_oppositeView = oppositeView;
            m_refreshMode = refreshMode;
            m_option = option;
            m_paramList = paramList;
        }
        
        //=========================================================================================
        // 機　能：処理完了時にディレクトリをリフレッシュする
        // 引　数：[in]successCount  成功件数
        // 　　　　[in]skipCount     スキップ件数
        // 　　　　[in]failCount     失敗件数
        // 戻り値：なし
        //=========================================================================================
        public void RefreshDirectory(int successCount, int skipCount, int failCount) {
            if (m_refreshMode != RefreshMode.NoRefresh || PreRefreshEvent != null) {
                BaseThread.InvokeProcedureByMainThread(new RefreshDirectoryDelegate(RefreshDirectoryUI), this, successCount, skipCount, failCount);
            }
        }
        private delegate void RefreshDirectoryDelegate(RefreshUITarget obj, int successCount, int skipCount, int failCount);
        private static void RefreshDirectoryUI(RefreshUITarget obj, int successCount, int skipCount, int failCount) {
            obj.RefreshDirectoryUI(successCount, skipCount, failCount);
        }
        private void RefreshDirectoryUI(int successCount, int skipCount, int failCount) {
            if (PreRefreshEvent != null) {
                PreRefreshEvent(successCount, skipCount, failCount);
            }
            
            if (m_refreshMode == RefreshMode.NoRefresh) {
                return;
            }

            string targetDir = m_targetView.FileList.DisplayDirectoryName;
            string oppositeDir = m_oppositeView.FileList.DisplayDirectoryName;
            if (targetDir == oppositeDir && (m_refreshMode == RefreshMode.RefreshTarget || m_refreshMode == RefreshMode.RefreshOpposite)) {
                m_refreshMode = RefreshMode.RefreshBoth;
            }

            bool targetRefresh = (m_refreshMode == RefreshMode.RefreshTarget || m_refreshMode == RefreshMode.RefreshBoth);
            bool oppositeRefresh = (m_refreshMode == RefreshMode.RefreshOpposite || m_refreshMode == RefreshMode.RefreshBoth);
            if (targetRefresh && m_targetView.FileList.DisplayDirectoryName == targetDir) {
                ReloadDirectoryInternal(m_targetView, successCount, skipCount, failCount);
            }
            if (oppositeRefresh && m_oppositeView.FileList.DisplayDirectoryName == oppositeDir) {
                ReloadDirectoryInternal(m_oppositeView, successCount, skipCount, failCount);
            }

            bool folderSizeRefresh = (m_refreshMode == RefreshMode.FolderSizeBoth || m_refreshMode == RefreshMode.FolderSizeBothAndClear);
            if (folderSizeRefresh && m_targetView.FileList.DisplayDirectoryName == targetDir) {
                m_targetView.FileList.SetFolderSize();
                m_targetView.Invalidate();
            }
            if (folderSizeRefresh && m_oppositeView.FileList.DisplayDirectoryName == oppositeDir) {
                m_oppositeView.FileList.SetFolderSize();
                m_oppositeView.Invalidate();
            }
            if (m_refreshMode == RefreshMode.FolderSizeBothAndClear) {
                Program.Document.RetrieveFolderSizeResult = null;
            }
        }

        //=========================================================================================
        // 機　能：指定されたビューを最新の状態に更新する
        // 引　数：[in]view   対象となるビュー
        // 　　　　[in]successCount  成功件数
        // 　　　　[in]skipCount     スキップ件数
        // 　　　　[in]failCount     失敗件数
        // 戻り値：なし
        //=========================================================================================
        private void ReloadDirectoryInternal(FileListView view, int successCount, int skipCount, int failCount) {
            // カーソル上のファイルを取得
            string cursorFile = null;
            if (m_option == RefreshOption.SpecifyCursorFile && failCount == 0) {
                cursorFile = (string)(m_paramList[0]);
            } else {
                int index = view.FileListViewComponent.CursorLineNo;
                if (index < view.FileList.Files.Count) {
                    cursorFile = view.FileList.Files[index].FileName;
                }
            }

            // リフレッシュを実行
            ChangeDirectoryParam param = new ChangeDirectoryParam.Refresh(view.FileList.DisplayDirectoryName, cursorFile);
            ChdirCommand.ChangeDirectory(view, param);
        }

        //=========================================================================================
        // 機　能：指定されたビューを最新の状態に更新する
        // 引　数：[in]view        対象となるビュー
        // 　　　　[in]filterSet   ファイル一覧のフィルターを指定しているときtrue
        // 　　　　[in]filterMode  ファイル一覧のフィルター（フィルターを解除するとき、フィルター指定がないときnull）
        // 戻り値：なし
        //=========================================================================================
        public static void ReloadDirectory(FileListView view, bool filterSet, FileListFilterMode filterMode) {
            // カーソル上のファイルを取得
            string cursorFile = null;
            int index = view.FileListViewComponent.CursorLineNo;
            if (index < view.FileList.Files.Count) {
                cursorFile = view.FileList.Files[index].FileName;
            }

            // リフレッシュを実行
            ChangeDirectoryParam param;
            if (filterSet) {
                param = new ChangeDirectoryParam.Refresh(view.FileList.DisplayDirectoryName, cursorFile, filterMode);
            } else {
                param = new ChangeDirectoryParam.Refresh(view.FileList.DisplayDirectoryName, cursorFile);
            }
            ChdirCommand.ChangeDirectory(view, param);
        }

        //=========================================================================================
        // 列挙子：ディレクトリをリフレッシュするモード
        //=========================================================================================
        public enum RefreshMode {
            RefreshTarget,                  // 対象のみリフレッシュ
            RefreshOpposite,                // 反対のみリフレッシュ
            RefreshBoth,                    // 両方をリフレッシュ
            NoRefresh,                      // リフレッシュしない
            FolderSizeBoth,                 // 両方のフォルダ情報のみをリフレッシュ
            FolderSizeBothAndClear,         // 両方のフォルダ情報のみをリフレッシュした後、フォルダサイズ取得結果をクリア
        }

        //=========================================================================================
        // 列挙子：リフレッシュ時のオプション
        //=========================================================================================
        public enum RefreshOption {
            None,                           // なし
            SpecifyCursorFile,              // カーソル位置のファイルを指定（引数[0]にstringでカーソルファイル名）
        }
    }
}
