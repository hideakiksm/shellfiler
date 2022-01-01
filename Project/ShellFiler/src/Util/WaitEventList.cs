using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace ShellFiler.Util {
    /*
    //=========================================================================================
    // クラス：イベント待機を行うハンドルのリスト
    //=========================================================================================
    public class WaitEventList {
        // 待機に使用するイベントのリスト
        private List<WaitHandle> m_waitEventList = new List<WaitHandle>();
        
        // リソースの管理対象のイベントのリスト
        private List<SafeWaitHandle> m_waitResourceList = new List<SafeWaitHandle>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void WaitEvnetList() {
        }
        
        //=========================================================================================
        // 機　能：イベントを追加する
        // 引　数：[in]handle  待機に使用するハンドル
        // 戻り値：なし
        //=========================================================================================
        public void AddEvent(WaitHandle handle) {
            m_waitEventList.Add(handle);
        }
        
        //=========================================================================================
        // 機　能：待機対象のプロセスからイベントを作成して追加する
        // 引　数：[in]handle  待機に使用するハンドル
        // 戻り値：なし
        //=========================================================================================
        public void CreateEventFromProcess(WaitHandle handle) {
            ManualResetEvent processEvent = new ManualResetEvent(false);
            processEvent.SafeWaitHandle = new SafeWaitHandle(process.Handle, false);
            m_waitResourceList.Add(processEvent);

            m_waitResourceList[0].fr
            m_processExecuteLock = new object();
                List<WaitHandle> waitEventList = new List<WaitHandle>();
                waitEventList.Add(m_endEvent);
                waitEventList.Add(m_watchProcessAddEvent);
                lock (this) {
                    foreach (Process process in m_watchProcess) {
                        waitEventList.AddProcess(process);
                        ManualResetEvent processEvent = new ManualResetEvent(false);
                        processEvent.SafeWaitHandle = new SafeWaitHandle(process.Handle, false);
                        waitEventList.Add(processEvent);
                    }
                }

        //=========================================================================================
        // 機　能：実行中のCPUビット数を返す
        // 引　数：なし
        // 戻り値：ビット数（16または32）
        //=========================================================================================
        public static int GetCurrentProcessBits() {
            if (IntPtr.Size == 4) {
                return 32;
            } else {
                return 64;
            }
        }

        //=========================================================================================
        // 機　能：CPUのコア数を返す
        // 引　数：なし
        // 戻り値：CPUのコア数
        //=========================================================================================
        public static int GetCpuCoreCount() {
            Win32API.SYSTEM_INFO sysInfo = Win32API.Win32GetSystemInfo();
            return (int)(sysInfo.dwNumberOfProcessors);
        }

        //=========================================================================================
        // 機　能：VARIANT1個分のサイズを返す
        // 引　数：なし
        // 戻り値：VARIANT1個分のサイズ[バイト]
        //=========================================================================================
        public static int GetVariantSize() {
            if (IntPtr.Size == 4) {
                return 16;          // 32bit
            } else {
                return 24;          // 64bit
            }
        }

        //=========================================================================================
        // 機　能：プロセスを起動する
        // 引　数：[in]psi     プロセス開始の情報
        // 　　　　[in]dirName プロセス起動時のカレント
        // 戻り値：実行したプロセス
        // メ　モ：起動失敗は例外で通知する
        //=========================================================================================
        public static Process StartProcess(ProcessStartInfo psi, string dirName) {
            Process process;
            lock (m_processExecuteLock) {
                string currentOld = Directory.GetCurrentDirectory();
                try {
                    Directory.SetCurrentDirectory(dirName);
                    process = Process.Start(psi);
                } finally {
                    Directory.SetCurrentDirectory(currentOld);
                }
            }
            return process;
        }

        //=========================================================================================
        // 機　能：ファイルを関連付け実行する
        // 引　数：[in]filePath   実行するファイルのローカルパス
        // 　　　　[in]currentDir カレントパス
        // 戻り値：実行したプロセス
        // メ　モ：起動失敗は例外で通知する
        //=========================================================================================
        public static Process ProcessStartCommandLine(string filePath, string currentDir) {
            Process process;
            lock (m_processExecuteLock) {
                string currentOld = Directory.GetCurrentDirectory();
                try {
                    Directory.SetCurrentDirectory(currentDir);
                    process = Process.Start(filePath);
                } finally {
                    Directory.SetCurrentDirectory(currentOld);
                }
            }
            return process;
        }

        //=========================================================================================
        // 機　能：キーがファンクションキーかどうかを返す
        // 引　数：[in]key   キー
        // 戻り値：ファンクションキーのときtrue
        //=========================================================================================
        public static bool IsFunctionKey(Keys key) {
            bool isFunc = false;
            switch (key) {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.F13:
                case Keys.F14:
                case Keys.F15:
                case Keys.F16:
                case Keys.F17:
                case Keys.F18:
                case Keys.F19:
                case Keys.F20:
                case Keys.F21:
                case Keys.F22:
                case Keys.F23:
                case Keys.F24:
                    isFunc = true;
                    break;
            }
            return isFunc;
        }
    }*/
}
