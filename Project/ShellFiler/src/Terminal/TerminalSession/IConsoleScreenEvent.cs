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
    // インターフェイス：コンソール画面に変化があったことを通知するイベント
    //=========================================================================================
    public interface IConsoleScreenEvent {

        //=========================================================================================
        // 機　能：接続処理が完了したときの処理を行う
        // 引　数：[in]sender    　 対象の画面
        // 　　　　[in]status       ステータス
        // 　　　　[in]errorDetail  詳細エラー情報
        // 戻り値：なし
        //=========================================================================================
        void OnConnect(ConsoleScreen sender, FileOperationStatus status, string errorDetail);

        //=========================================================================================
        // 機　能：データが追加されたときの処理を行う
        // 引　数：[in]sender      対象の画面
        // 　　　　[in]changeLog   変化内容
        // 戻り値：なし
        //=========================================================================================
        void OnAddData(ConsoleScreen sender, LogLineChangeLog changeLog);

        //=========================================================================================
        // 機　能：SSHの接続が閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        void OnSSHClose();
    }
}
