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
    class ExtractThread : BaseThread {
        // イベントの通知用コールバック
        private BackgroundWaitCallback m_callback;

        // 展開条件の引数
        private VirtualExtractArg m_argument;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]callback   イベントの通知用コールバック
        // 　　　　[in]arg        展開条件の引数
        // 戻り値：なし
        //=========================================================================================
        public ExtractThread(BackgroundWaitCallback callback, VirtualExtractArg arg) : base("ExtractWait", 1) {
            m_callback = callback;
            m_argument = arg;
        }

        //=========================================================================================
        // 機　能：スレッドの入り口（実際のスレッドで実装する）
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        protected override void ThreadEntry() {
            FileOperationStatus status;
            try {
                status = ExtractMain();
            } catch (Exception e) {
                Program.Abort("展開処理中に想定外のエラーが発生しました。\n{0}", e.ToString());
                status = FileOperationStatus.Fail;
            }

            m_callback.ExtractCompleted(status);
        }

        //=========================================================================================
        // 機　能：展開処理のメイン
        // 引　数：なし
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus ExtractMain() {
            FileOperationStatus status = FileOperationStatus.Fail;
            if (m_argument is VirtualExtractMarkedArg) {
                status = ExtractMarked(m_callback.RequestContext, (VirtualExtractMarkedArg)m_argument);
            }
            return status;
        }

        //=========================================================================================
        // 機　能：展開処理を実行する
        // 引　数：[in]context   コンテキスト情報
        // 　　　　[in]arg       展開処理の引数
        // 戻り値：ステータスコード
        //=========================================================================================
        private FileOperationStatus ExtractMarked(FileOperationRequestContext context, VirtualExtractMarkedArg arg) {
            FileOperationStatus status;
            List<string> tempFileList;
            status = ExtractRuntime.ExtractVirtualStoreMultiFiles(context, arg.DispFileList, out tempFileList);
            if (!status.Succeeded) {
                return status;
            }
            arg.TempFileList = tempFileList;
            return FileOperationStatus.Success;
        }
    }
}
