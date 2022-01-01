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
using ShellFiler.FileTask;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Util;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;

namespace ShellFiler.Virtual {

    //=========================================================================================
    // クラス：展開処理を実行するスレッド
    //=========================================================================================
    class GetFileInfoThread : BaseThread {
        // イベントの通知用コールバック
        private BackgroundWaitCallback m_callback;

        // 取得対象のパス
        private string m_path;

        // 取得対象のファイルシステムのID
        private FileSystemID m_fileSystemId;

        // 取得結果
        private IFile m_resultFileInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]callback     イベントの通知用コールバック
        // 　　　　[in]path         取得対象のパス
        // 　　　　[in]fileSystemId 取得対象のファイルシステムのID
        // 戻り値：なし
        //=========================================================================================
        public GetFileInfoThread(BackgroundWaitCallback callback, string path, FileSystemID fileSystemId) : base("GetFileInfoThread", 1) {
            m_callback = callback;
            m_path = path;
            m_fileSystemId = fileSystemId;
        }

        //=========================================================================================
        // 機　能：スレッドの入り口（実際のスレッドで実装する）
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        protected override void ThreadEntry() {
            FileOperationStatus status;
            try {
                DateTime startTime = DateTime.Now;
                status = GetFileInfoMain();
                DateTime endTime = DateTime.Now;

                // ダイアログが見えないため、少し待ってから復帰
                double spent = (endTime - startTime).TotalMilliseconds;
                const int MIN_TIME_WAIT = 500;
                if (spent < MIN_TIME_WAIT) {
                    Thread.Sleep((int)(MIN_TIME_WAIT - spent));
                }
            } catch (Exception e) {
                Program.Abort("展開処理中に想定外のエラーが発生しました。\n{0}", e.ToString());
                status = FileOperationStatus.Fail;
            }

            m_callback.ExtractCompleted(status);
        }

        //=========================================================================================
        // 機　能：取得処理のメイン
        // 引　数：なし
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus GetFileInfoMain() {
            FileOperationStatus status;
            IFileSystem fileSystem = Program.Document.FileSystemFactory.CreateFileSystemForFileOperation(m_fileSystemId);
            status = fileSystem.GetFileInfo(m_callback.RequestContext, m_path, true, out m_resultFileInfo);
            return status;
        }

        //=========================================================================================
        // プロパティ：取得結果
        //=========================================================================================
        public IFile ResultFileInfo {
            get {
                return m_resultFileInfo;
            }
        }
    }
}
