using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：スレッドの基底クラス
    // 　　　　タイムアウト付きで安全なInvokeを行う手段を提供する
    // 　　　　例 void MethodInSubThread() {
    // 　　　　       object result;
    // 　　　　       bool success = InvokeMethod(new AddNumberDelegate(AddNumberUI), out result, 1, 2);
    // 　　　　   }
    // 　　　　   private delegate int AddNumberDelegate(int a, int b);
    // 　　　　   private int AddNumberUI(int a, int b) {
    // 　　　　       return a+b;
    // 　　　　   }
    //=========================================================================================
    public abstract class BaseThread {
        // Invokeのリトライ回数（合計10秒）
        private const int INVOKE_RETRY_COUNT = 100;

        // Invoke中のJoinまでの確認時間
        private const int INVOKE_TIMEOUT = 100;

        // このスレッドの名前
        private string m_threadName;

        // このスレッドを実行する.NetのThreadクラス
        private Thread[] m_threadList;

        // スレッドがJoinされたときtrue
        private volatile bool m_threadJoined = false;

        // メインスレッドを実行するコントロール（マーシャリングのターゲット）
        private static Control s_invokationTarget;

        // メインスレッド
        private static Thread s_mainThread;
        
        // このスレッドに関連づけられたオブジェクト
        [ThreadStatic]
        private static BaseThread t_currentThread;

        //=========================================================================================
        // 機　能：アプリケーションのメインエントリポイント
        // 引　数：[in]threadName  デバッグに使用するスレッドの表示名
        // 　　　　[in]threadCount 同時実行するスレッドの数
        // 戻り値：なし
        //=========================================================================================
        public BaseThread(string threadName, int threadCount) {
            m_threadName = threadName;
            m_threadList = new Thread[threadCount];
            for (int i = 0; i < m_threadList.Length; i++) {
                m_threadList[i] =  new Thread(new ThreadStart(ThreadEntryBase));
                m_threadList[i].Name = threadName;
            }
        }
        
        //=========================================================================================
        // 機　能：スレッドを開始する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void StartThread() {
            for (int i = 0; i < m_threadList.Length; i++) {
                m_threadList[i].Start();
            }
        }

        //=========================================================================================
        // 機　能：スレッドの終了まで待機する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void JoinThread() {
            if (!m_threadJoined) {
                m_threadJoined = true;
                OnThreadJoined();
                for (int i = 0; i < m_threadList.Length; i++) {
                    m_threadList[i].Join();
                }
            }
        }
        
        //=========================================================================================
        // 機　能：スレッドの入り口（実際のスレッドで実装する）
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ThreadEntryBase() {
            try {
                t_currentThread = this;
                ThreadEntry();
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionThread, m_threadList[0].Name);
            }
        }

        //=========================================================================================
        // 機　能：スレッドの終了まで待機する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RestartThread() {
            JoinThread();
            m_threadJoined = false;
            for (int i = 0; i < m_threadList.Length; i++) {
                m_threadList[i] =  new Thread(new ThreadStart(ThreadEntryBase));
                m_threadList[i].Name = m_threadName;
            }
            StartThread();
        }

        //=========================================================================================
        // 機　能：スレッドの入り口（実際のスレッドで実装する）
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        protected abstract void ThreadEntry();

        //=========================================================================================
        // 機　能：スレッドがJoinされるタイミングでJoinThread()を呼び出したスレッドで呼び出される
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        protected virtual void OnThreadJoined() {
        }

        //=========================================================================================
        // 機　能：メインスレッドで戻り値なしのメソッドを実行する
        // 引　数：[in]method    メインスレッドで実行されるdelegate先メソッド
        // 　　　　[in]paramList メソッドの入力パラメータ
        // 戻り値：メソッドの実行に成功したときtrue
        //=========================================================================================
        public static bool InvokeProcedureByMainThread(Delegate method, params object[] paramList) {
            object result;
            return InvokeFunctionByMainThread(method, out result, paramList);
        }

        //=========================================================================================
        // 機　能：メインスレッドで戻り値ありのメソッドを実行する
        // 引　数：[in]method    メインスレッドで実行されるdelegate先メソッド
        // 　　　　[out]result   メソッドの戻り値
        // 　　　　[in]paramList メソッドの入力パラメータ
        // 戻り値：メソッドの実行に成功したときtrue
        //=========================================================================================
        public static bool InvokeFunctionByMainThread(Delegate method, out object result, params object[] paramList) {
            if (t_currentThread == null) {
                // メインスレッドからの実行ではそのまま呼び出す
                if (Thread.CurrentThread == s_mainThread) {
                    result = method.Method.Invoke(method.Target, paramList);
                } else {
                    // 通常ここに来ないようにする
                    // 7z.dllなど、callbackが別スレッドになる場合はここを通る
                    if (s_invokationTarget.IsDisposed) {
                        result = null;
                        return false;
                    }
                    result = s_invokationTarget.Invoke(method, paramList);
                }
                return true;
            } else {
                // メインスレッド以外はタイムアウト付きで呼び出す
                result = null;
                if (t_currentThread.m_threadJoined) {
                    return false;
                }

                // 非同期でDelegateメソッドを呼び出す
                BooleanFlag startRequest = new BooleanFlag(false);
                MethodResultValue resultValue = new MethodResultValue();
                resultValue.Value = result;
                ManualResetEvent returnEvent = new ManualResetEvent(false);
                try {
                    s_invokationTarget.BeginInvoke(new InvokeMethodDelegate(InvokeMethodUI), returnEvent, method, startRequest, resultValue, paramList);
                } catch (InvalidOperationException) {
                    return false;
                }

                // メソッドの終了まで待つ
                int waitTime = 0;
                while (true) {
                    if (startRequest.Value == false) {
                        if (waitTime > INVOKE_RETRY_COUNT) {
                            break;
                        }
                    }
                    if (t_currentThread.m_threadJoined) {
                        return false;
                    }
                    bool signal = returnEvent.WaitOne(INVOKE_TIMEOUT, false);
                    if (signal) {
                        // 応答があった
                        if (resultValue.Exception != null) {
                            return false;
                        }
                        result = resultValue.Value;
                        return true;
                    }
                    waitTime++;
                }
                return false;
            }
        }

        //=========================================================================================
        // 機　能：内部的にメインスレッドに切り替えるためのInvoke先
        // 引　数：[in]returnEvent    戻り値が確定したことを呼び出しもとスレッドに伝えるためのイベント
        // 　　　　[in]method         実際のdelegate先メソッド
        // 　　　　[in]startFlag      メソッドの実行を開始したときtrueを返すフラグ
        // 　　　　[in,out]result     メソッドの戻り値を返すラップオブジェクト
        // 　　　　[in]paramList      メソッドの入力パラメータ
        // 戻り値：なし
        //=========================================================================================
        private delegate void InvokeMethodDelegate(ManualResetEvent returnEvent, Delegate method, BooleanFlag startFlag, MethodResultValue result, object[] paramList);
        private static void InvokeMethodUI(ManualResetEvent returnEvent, Delegate method, BooleanFlag startFlag, MethodResultValue result, object[] paramList) {
            try {
                startFlag.Value = true;
                result.Value = method.Method.Invoke(method.Target, paramList);
                returnEvent.Set();
            } catch (Exception e) {
                Program.UnexpectedException(e, Resources.Msg_UnexpectedExceptionUI, method.Method.Name);
                result.Exception = e;
                returnEvent.Set();
            }
        }

        //=========================================================================================
        // プロパティ：処理を中止するときtrue
        //=========================================================================================
        public virtual bool IsCancel {
            get {
                return m_threadJoined;
            }
        }

        //=========================================================================================
        // プロパティ：メインスレッドを実行するコントロール（マーシャリングのターゲット）
        //=========================================================================================
        public static Control InvocationTarget {
            get {
                return s_invokationTarget;
            }
            set {
                s_invokationTarget = value;
            }
        }

        //=========================================================================================
        // プロパティ：メインスレッド
        //=========================================================================================
        public static Thread MainThread {
            set {
                s_mainThread = value;
            }
        }

        //=========================================================================================
        // プロパティ：呼び出したスレッド
        //=========================================================================================
        public static BaseThread CurrentThread {
            get {
                return t_currentThread;
            }
        }

        //=========================================================================================
        // クラス：メソッドの戻り値を返すためのラップオブジェクト
        //=========================================================================================
        private class MethodResultValue {
            // メソッドからの戻り値
            public object Value;

            // 発生した例外（例外がないときnull）
            public Exception Exception = null;
        }
    }
}
