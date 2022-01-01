using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：標準出力の中継方法
    //=========================================================================================
    public class ShellExecuteRelayMode {
        // ログウィンドウに中継
        public static readonly ShellExecuteRelayMode RelayLogWindow = new ShellExecuteRelayMode();

        // ファイルビューアに中継
        public static readonly ShellExecuteRelayMode RelayFileViewer = new ShellExecuteRelayMode();

        // 中継しない（Windowsのみ有効）
        public static readonly ShellExecuteRelayMode None = new ShellExecuteRelayMode();

        //=========================================================================================
        // 機　能：シリアライズされたデータからオブジェクトを復元する
        // 引　数：[in]serialized  シリアライズされたデータ
        // 戻り値：復元した中継方法
        //=========================================================================================
        public static ShellExecuteRelayMode FromSerializedData(string serialized) {
            if (serialized == "RelayLogWindow") {
                return RelayLogWindow;
            } else if (serialized == "RelayFileViewer") {
                return RelayFileViewer;
            } else if (serialized == "None") {
                return None;
            } else {
                return RelayLogWindow;
            }
        }

        //=========================================================================================
        // 機　能：オブジェクトからシリアライズされたデータを作成する
        // 引　数：[in]obj         オブジェクト
        // 戻り値：シリアライズされたデータ
        //=========================================================================================
        public static string ToSerializedData(ShellExecuteRelayMode obj) {
            if (obj == RelayLogWindow) {
                return "RelayLogWindow";
            } else if (obj == RelayFileViewer) {
                return "RelayFileViewer";
            } else if (obj == None) {
                return "None";
            } else {
                return "RelayLogWindow";
            }
        }
    }
}
