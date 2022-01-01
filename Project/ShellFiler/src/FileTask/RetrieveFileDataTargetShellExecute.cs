using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.UI.Log;
using ShellFiler.UI.Log.Logger;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;
using ShellFiler.Locale;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：シェルの実行結果をログに中継するデータターゲット
    //=========================================================================================
    class RetrieveFileDataTargetShellExecute : IRetrieveFileDataTarget {
        // ストリームから受け取った後の文字列（標準出力／標準エラー出力混在）
        private List<string> m_listReceive = new List<string>();

        // 行の分割の実装
        private ConsoleResultLineSplitter m_splitter;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]encoding   プロセスの出力を文字列化する際のエンコーディング
        // 戻り値：なし
        //=========================================================================================
        public RetrieveFileDataTargetShellExecute(Encoding encoding) {
            m_splitter = new ConsoleResultLineSplitter(encoding);
        }

        //=========================================================================================
        // 機　能：新しいデータを追加する
        // 引　数：[in]buffer  データの入ったバッファ
        // 　　　　[in]offset  buffer中のオフセット
        // 　　　　[in]length  データの長さ
        // 戻り値：なし
        // メ　モ：読み込みスレッドまたはその他外部の作業スレッドからの呼び出しを想定
        //=========================================================================================
        public void AddData(byte[] buffer, int offset, int length) {
            List<string> lines = m_splitter.AddData(buffer, offset, length);
            lock (this) {
                m_listReceive.AddRange(lines);
            }
        }

        //=========================================================================================
        // 機　能：データの追加が終わったことを通知する
        // 引　数：[in]status    読み込み状況のステータス
        // 　　　　[in]errorInfo status=Failedのときエラー情報の文字列、それ以外はnull
        // 戻り値：なし
        // メ　モ：作業スレッドからの呼び出しを想定
        //=========================================================================================
        public void AddCompleted(RetrieveDataLoadStatus status, string errorInfo) {
        }

        //=========================================================================================
        // 機　能：受信したときのイベントを発行する
        // 引　数：[in]final   最後のイベント通知のときtrue
        // 戻り値：なし
        // メ　モ：読み込みスレッドからの呼び出しを想定
        //=========================================================================================
        public void FireEvent(bool final) {
            if (final) {
                string line = m_splitter.GetLastLine();
                if (line != null) {
                    lock (this) {
                        m_listReceive.Add(line);
                    }
                }
            }
            while (true) {
                string logStr = null;
                lock (this) {
                    if (m_listReceive.Count > 0) {
                        logStr = m_listReceive[0];
                        m_listReceive.RemoveAt(0);
                    }
                }
                if (logStr != null) {
                    LogLineSimple logLine = new LogLineSimple(LogColor.Normal, "{0}", logStr);
                    Program.LogWindow.RegistLogLine(logLine);
                } else {
                    break;
                }
            }
        }
    }
}
