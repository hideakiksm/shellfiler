using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;

namespace ShellFiler.GraphicsViewer.Filter {

    //=========================================================================================
    // クラス：バックグラウンドで画像のフィルタリング処理を行うスレッド
    //=========================================================================================
    public class FilterThread : BaseThread {
        // フィルタリング処理のリクエスト（添字の小さい方から処理）
        private List<FilterRequest> m_listRequest = new List<FilterRequest>();

        // リクエストが入ったことを表すイベント
        private AutoResetEvent m_requestEvent = new AutoResetEvent(false);

        // 終了イベント
        private ManualResetEvent m_endEvent = new ManualResetEvent(false);

        // スレッドを終了するときtrue
        private BooleanFlag m_cancelFlag = new BooleanFlag(false);

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]theradCount  同時実行するスレッドの数
        // 戻り値：なし
        //=========================================================================================
        public FilterThread(int theradCount) : base("FilterThread", theradCount) {
            StartThread();
        }
        
        //=========================================================================================
        // 機　能：処理を終了する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            m_endEvent.Set();
            m_cancelFlag.Value = true;
            JoinThread();
        }

        //=========================================================================================
        // 機　能：再読込をリクエストする
        // 引　数：[in]fileList  再読込するファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public void Request(IFilterComponent filter, int startLine, int endLine, ManualResetEvent endEvent) {
            lock (this) {
                m_listRequest.Add(new FilterRequest(filter, startLine, endLine, endEvent));
                m_requestEvent.Set();
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
            while (true) {
                // イベントを待つ
                bool wait = false;
                lock (this) {
                    wait = (m_listRequest.Count == 0);
                }
                if (wait) {
                    WaitHandle[] waitEventList = { m_endEvent, m_requestEvent };
                    int index = WaitHandle.WaitAny(waitEventList);
                    if (index == 0) {
                        return;               // 終了イベント
                    }
                }

                // リクエストを取得
                FilterRequest request = null;
                lock (this) {
                    if (m_listRequest.Count > 0) {
                        request = m_listRequest[0];
                        m_listRequest.RemoveAt(0);
                    }
                }

                // 実行
                if (request != null) {
                    ExecuteFilter(request);
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルのクロールを実行する
        // 引　数：[in]request   リクエスト
        // 戻り値：なし
        //=========================================================================================
        private void ExecuteFilter(FilterRequest request) {
            int startLine = request.StartLine;
            int endLine = request.EndLine;
            request.Filter.FilterExecute(startLine, endLine, m_cancelFlag);
            request.EndEvent.Set();
        }

        //=========================================================================================
        // クラス：フィルタリング処理のリクエスト
        //=========================================================================================
        private class FilterRequest {
            // フィルタ
            public IFilterComponent Filter;

            // 開始行
            public int StartLine;

            // 終了行
            public int EndLine;

            // 終了時にシグナル状態になるイベント
            public ManualResetEvent EndEvent;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]filter     実行するフィルタ
            // 　　　　[in]startLine  開始行
            // 　　　　[in]endLine    終了行
            // 　　　　[in]endEvent   終了時にシグナル状態になるイベント
            // 戻り値：なし
            //=========================================================================================
            public FilterRequest(IFilterComponent filter, int startLine, int endLine, ManualResetEvent endEvent) {
                Filter = filter;
                StartLine = startLine;
                EndLine = endLine;
                EndEvent = endEvent;
            }
        }
    }
}
