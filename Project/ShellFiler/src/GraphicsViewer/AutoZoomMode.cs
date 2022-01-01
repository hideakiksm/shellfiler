using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // 列挙子：自動で拡大率を変えるモード
    //=========================================================================================
    public class GraphicsViewerAutoZoomMode {
        public static readonly GraphicsViewerAutoZoomMode AlwaysOriginal      = new GraphicsViewerAutoZoomMode("AlwaysOriginal");       // 常に元の画像サイズで表示する
        public static readonly GraphicsViewerAutoZoomMode AutoZoom            = new GraphicsViewerAutoZoomMode("AutoZoom");             // 自動で画像全体が表示可能な大きさに補正する
        public static readonly GraphicsViewerAutoZoomMode AutoZoomOff         = new GraphicsViewerAutoZoomMode("AutoZoomOff");          // AutoZoomが一時的にOFFになっている
        public static readonly GraphicsViewerAutoZoomMode AutoZoomAlways      = new GraphicsViewerAutoZoomMode("AutoZoomAlways");       // AutoZoomで常に表示する
        public static readonly GraphicsViewerAutoZoomMode AutoZoomWide        = new GraphicsViewerAutoZoomMode("AutoZoomWide");         // 自動で画像が画面全体に表示される大きさに補正する
        public static readonly GraphicsViewerAutoZoomMode AutoZoomWideOff     = new GraphicsViewerAutoZoomMode("AutoZoomWideOff");      // AutoZoomWideが一時的にOFFになっている
        public static readonly GraphicsViewerAutoZoomMode AutoZoomWideAlways  = new GraphicsViewerAutoZoomMode("AutoZoomWideAlways");   // AutoZoomWideで常に表示する

        // デバッグ用文字列
        private string m_debugString;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]debugString  デバッグ用文字列
        // 戻り値：なし
        //=========================================================================================
        private GraphicsViewerAutoZoomMode(string debugString) {
            m_debugString = debugString;
        }

        //=========================================================================================
        // 機　能：シリアライズされたデータからオブジェクトを復元する
        // 引　数：[in]serialized  シリアライズされたデータ
        // 戻り値：復元した値
        //=========================================================================================
        public static GraphicsViewerAutoZoomMode FromSerializedData(string serialized) {
            if (serialized == "AlwaysOriginal") {
                return AlwaysOriginal;
            } else if (serialized == "AutoZoom") {
                return AutoZoom;
            } else if (serialized == "AutoZoomOff") {
                return AutoZoomOff;
            } else if (serialized == "AutoZoomAlways") {
                return AutoZoomAlways;
            } else if (serialized == "AutoZoomWide") {
                return AutoZoomWide;
            } else if (serialized == "AutoZoomWideOff") {
                return AutoZoomWideOff;
            } else if (serialized == "AutoZoomWideAlways") {
                return AutoZoomWideAlways;
            } else {
                return AutoZoom;
            }
        }

        //=========================================================================================
        // 機　能：オブジェクトからシリアライズされたデータを作成する
        // 引　数：[in]obj         オブジェクト
        // 戻り値：シリアライズされたデータ
        //=========================================================================================
        public static string ToSerializedData(GraphicsViewerAutoZoomMode obj) {
            if (obj == AlwaysOriginal) {
                return "AlwaysOriginal";
            } else if (obj == AutoZoom) {
                return "AutoZoom";
            } else if (obj == AutoZoomOff) {
                return "AutoZoomOff";
            } else if (obj == AutoZoomAlways) {
                return "AutoZoomAlways";
            } else if (obj == AutoZoomWide) {
                return "AutoZoomWide";
            } else if (obj == AutoZoomWideOff) {
                return "AutoZoomWideOff";
            } else if (obj == AutoZoomWideAlways) {
                return "AutoZoomWideAlways";
            } else {
                return "AutoZoom";
            }
        }
    }
}
