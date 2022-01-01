using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：ファイルの結合でのファイル名指定方法
    //=========================================================================================
    public class CombineDefaultFileType {
        // 指定されたファイル名
        public static readonly CombineDefaultFileType Specified = new CombineDefaultFileType();

        // ファイルビューアに中継
        public static readonly CombineDefaultFileType FirstMark = new CombineDefaultFileType();

        // 中継しない（Windowsのみ有効）
        public static readonly CombineDefaultFileType Previous = new CombineDefaultFileType();

        //=========================================================================================
        // 機　能：シリアライズされたデータからオブジェクトを復元する
        // 引　数：[in]serialized  シリアライズされたデータ
        // 戻り値：復元した中継方法
        //=========================================================================================
        public static CombineDefaultFileType FromSerializedData(string serialized) {
            if (serialized == "Specified") {
                return Specified;
            } else if (serialized == "FirstMark") {
                return FirstMark;
            } else if (serialized == "Previous") {
                return Previous;
            } else {
                return Specified;
            }
        }

        //=========================================================================================
        // 機　能：オブジェクトからシリアライズされたデータを作成する
        // 引　数：[in]obj         オブジェクト
        // 戻り値：シリアライズされたデータ
        //=========================================================================================
        public static string ToSerializedData(CombineDefaultFileType obj) {
            if (obj == Specified) {
                return "Specified";
            } else if (obj == FirstMark) {
                return "FirstMark";
            } else if (obj == Previous) {
                return "Previous";
            } else {
                return "Specified";
            }
        }
    }
}
