using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：SSHターミナルのログ出力モード
    //=========================================================================================
    public class TerminalLogType {
        // ログファイルを出力しない
        public static readonly TerminalLogType None = new TerminalLogType();

        // 全体で１つのログファイルに出力
        public static readonly TerminalLogType Integrate = new TerminalLogType();

        // セッションごとに別のログファイルに出力
        public static readonly TerminalLogType EachSession = new TerminalLogType();

        //=========================================================================================
        // 機　能：シリアライズされたデータからオブジェクトを復元する
        // 引　数：[in]serialized  シリアライズされたデータ
        // 戻り値：復元した中継方法
        //=========================================================================================
        public static TerminalLogType FromSerializedData(string serialized) {
            if (serialized == "None") {
                return None;
            } else if (serialized == "Integrate") {
                return Integrate;
            } else if (serialized == "EachSession") {
                return EachSession;
            } else {
                return None;
            }
        }

        //=========================================================================================
        // 機　能：オブジェクトからシリアライズされたデータを作成する
        // 引　数：[in]obj         オブジェクト
        // 戻り値：シリアライズされたデータ
        //=========================================================================================
        public static string ToSerializedData(TerminalLogType obj) {
            if (obj == None) {
                return "None";
            } else if (obj == Integrate) {
                return "Integrate";
            } else if (obj == EachSession) {
                return "EachSession";
            } else {
                return "None";
            }
        }
    }
}
