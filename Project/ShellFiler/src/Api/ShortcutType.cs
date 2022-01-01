using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：ショートカットの種類
    //=========================================================================================
    public class ShortcutType {
        public static readonly ShortcutType WindowsShortcut = new ShortcutType();   // Windowsのショートカット
        public static readonly ShortcutType SymbolicLink = new ShortcutType();      // シンボリックリンク
        public static readonly ShortcutType HardLink = new ShortcutType();          // ハードリンク

        //=========================================================================================
        // 機　能：シリアライズされたデータからオブジェクトを復元する
        // 引　数：[in]serialized  シリアライズされたデータ
        // 戻り値：復元したショートカットの種類
        //=========================================================================================
        public static ShortcutType FromSerializedData(string serialized) {
            if (serialized == "WindowsShortcut") {
                return WindowsShortcut;
            } else if (serialized == "SymbolicLink") {
                return SymbolicLink;
            } else if (serialized == "HardLink") {
                return HardLink;
            } else {
                return SymbolicLink;
            }
        }

        //=========================================================================================
        // 機　能：オブジェクトからシリアライズされたデータを作成する
        // 引　数：[in]obj   ショートカットの種類
        // 戻り値：シリアライズされたデータ
        //=========================================================================================
        public static string ToSerializedData(ShortcutType obj) {
            if (obj == WindowsShortcut) {
                return "WindowsShortcut";
            } else if (obj == SymbolicLink) {
                return "SymbolicLink";
            } else if (obj == HardLink) {
                return "HardLink";
            } else {
                return "SymbolicLink";
            }
        }
    }
}
