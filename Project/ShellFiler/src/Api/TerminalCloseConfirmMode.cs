using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：SSHターミナルをクローズするときのモード
    //=========================================================================================
    public class TerminalCloseConfirmMode {
        // ウィンドウを閉じたときシェル接続も閉じる
        public static readonly TerminalCloseConfirmMode ShellClose = new TerminalCloseConfirmMode();

        // ウィンドウを閉じてもシェル接続はそのまま
        public static readonly TerminalCloseConfirmMode KeepChannelConfirm = new TerminalCloseConfirmMode();

        // ウィンドウを閉じてもシェル接続はそのままで、閉じるときのメッセージなし
        public static readonly TerminalCloseConfirmMode KeepChannelSilent = new TerminalCloseConfirmMode();

        //=========================================================================================
        // 機　能：シリアライズされたデータからオブジェクトを復元する
        // 引　数：[in]serialized  シリアライズされたデータ
        // 戻り値：復元した中継方法
        //=========================================================================================
        public static TerminalCloseConfirmMode FromSerializedData(string serialized) {
            if (serialized == "ShellClose") {
                return ShellClose;
            } else if (serialized == "KeepChannelConfirm") {
                return KeepChannelConfirm;
            } else if (serialized == "KeepChannelSilent") {
                return KeepChannelSilent;
            } else {
                return KeepChannelConfirm;
            }
        }

        //=========================================================================================
        // 機　能：オブジェクトからシリアライズされたデータを作成する
        // 引　数：[in]obj         オブジェクト
        // 戻り値：シリアライズされたデータ
        //=========================================================================================
        public static string ToSerializedData(TerminalCloseConfirmMode obj) {
            if (obj == ShellClose) {
                return "ShellClose";
            } else if (obj == KeepChannelConfirm) {
                return "KeepChannelConfirm";
            } else if (obj == KeepChannelSilent) {
                return "KeepChannelSilent";
            } else {
                return "KeepChannelConfirm";
            }
        }
    }
}
