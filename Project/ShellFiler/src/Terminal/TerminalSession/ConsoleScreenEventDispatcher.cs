using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Locale;
using ShellFiler.Terminal.UI;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession {

    //=========================================================================================
    // クラス：仮想コンソールに対するイベント送出を行うクラス
    //=========================================================================================
    public class ConsoleScreenEventDispatcher {
        // イベントの送信先
        private List<IConsoleScreenEvent> m_eventSink = new List<IConsoleScreenEvent>();

        // イベント対象の仮想コンソール
        private ConsoleScreen m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent   イベント対象の仮想コンソール
        // 戻り値：なし
        //=========================================================================================
        public ConsoleScreenEventDispatcher(ConsoleScreen parent) {
            m_parent = parent;
        }

        //=========================================================================================
        // 機　能：イベントの送信先を登録する
        // 引　数：[in]eventSink  登録するイベントの送信先
        // 戻り値：なし
        //=========================================================================================
        public void AddEventHandler(IConsoleScreenEvent eventSink) {
            m_eventSink.Add(eventSink);
        }

        //=========================================================================================
        // 機　能：イベントの送信先を削除する
        // 引　数：[in]eventSink  削除するイベントの送信先
        // 戻り値：なし
        //=========================================================================================
        public void DeleteEventHandler(IConsoleScreenEvent eventSink) {
            m_eventSink.Remove(eventSink);
        }

        //=========================================================================================
        // 機　能：接続処理が完了したときの処理を行う
        // 引　数：[in]status       ステータス
        // 　　　　[in]errorDetail  詳細エラー情報
        // 戻り値：なし
        //=========================================================================================
        public void NotifyConnect(FileOperationStatus status, string errorDetail) {
            for (int i = 0; i < m_eventSink.Count; i++) {
                m_eventSink[i].OnConnect(m_parent, status, errorDetail);
            }
        }

        //=========================================================================================
        // 機　能：データが追加されたときの処理を行う
        // 引　数：[in]changed  ログの更新内容
        // 戻り値：なし
        // メ　モ：1回分の更新として、重複行は取り除かれている
        //=========================================================================================
        public void AddData(LogLineChangeLog changes) {
            for (int i = 0; i < m_eventSink.Count; i++) {
                m_eventSink[i].OnAddData(m_parent, changes);
            }
        }

        //=========================================================================================
        // 機　能：SSHの接続が閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void NotifyClose() {
            for (int i = 0; i < m_eventSink.Count; i++) {
                m_eventSink[i].OnSSHClose();
            }
        }
    }
}
