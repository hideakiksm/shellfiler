using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：ドラッグで慣性を使用するかどうか
    //=========================================================================================
    public class DragInertiaType {
        public static readonly DragInertiaType Always    = new DragInertiaType("Always");       // 常に使用する
        public static readonly DragInertiaType LocalOnly = new DragInertiaType("LocalOnly");    // リモートデスクトップでは使用しない
        public static readonly DragInertiaType NotUse    = new DragInertiaType("NotUse");       // 使用しない

        // デバッグ用文字列
        private string m_debugString;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]debugString  デバッグ用文字列
        // 戻り値：なし
        //=========================================================================================
        private DragInertiaType(string debugString) {
            m_debugString = debugString;
        }

        //=========================================================================================
        // 機　能：シリアライズされたデータからオブジェクトを復元する
        // 引　数：[in]serialized  シリアライズされたデータ
        // 戻り値：復元した値
        //=========================================================================================
        public static DragInertiaType FromSerializedData(string serialized) {
            if (serialized == "Always") {
                return Always;
            } else if (serialized == "LocalOnly") {
                return LocalOnly;
            } else if (serialized == "NotUse") {
                return NotUse;
            } else {
                return LocalOnly;
            }
        }

        //=========================================================================================
        // 機　能：オブジェクトからシリアライズされたデータを作成する
        // 引　数：[in]obj         オブジェクト
        // 戻り値：シリアライズされたデータ
        //=========================================================================================
        public static string ToSerializedData(DragInertiaType obj) {
            if (obj == Always) {
                return "Always";
            } else if (obj == LocalOnly) {
                return "LocalOnly";
            } else if (obj == NotUse) {
                return "NotUse";
            } else {
                return "LocalOnly";
            }
        }
    }
}
