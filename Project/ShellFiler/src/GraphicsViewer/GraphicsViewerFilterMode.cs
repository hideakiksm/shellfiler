using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：画像にフィルターを適用するモード
    //=========================================================================================
    public class GraphicsViewerFilterMode {
        public static readonly GraphicsViewerFilterMode CurrentImageOnly    = new GraphicsViewerFilterMode("CurrentImageOnly");         // 表示中の画像だけにフィルターを適用する
        public static readonly GraphicsViewerFilterMode CurrentWindowImages = new GraphicsViewerFilterMode("CurrentWindowImages");      // 設定したウィンドウの画像だけにフィルターを適用する
        public static readonly GraphicsViewerFilterMode AllImages           = new GraphicsViewerFilterMode("AllImages");                // すべての画像にフィルターを適用する

        // デバッグ用文字列
        private string m_debugString;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]debugString  デバッグ用文字列
        // 戻り値：なし
        //=========================================================================================
        private GraphicsViewerFilterMode(string debugString) {
            m_debugString = debugString;
        }

        //=========================================================================================
        // 機　能：シリアライズされたデータからオブジェクトを復元する
        // 引　数：[in]serialized  シリアライズされたデータ
        // 戻り値：復元した値
        //=========================================================================================
        public static GraphicsViewerFilterMode FromSerializedData(string serialized) {
            if (serialized == "CurrentImageOnly") {
                return CurrentImageOnly;
            } else if (serialized == "CurrentWindowImages") {
                return CurrentWindowImages;
            } else if (serialized == "AllImages") {
                return AllImages;
            } else {
                return CurrentWindowImages;
            }
        }

        //=========================================================================================
        // 機　能：オブジェクトからシリアライズされたデータを作成する
        // 引　数：[in]obj         オブジェクト
        // 戻り値：シリアライズされたデータ
        //=========================================================================================
        public static string ToSerializedData(GraphicsViewerFilterMode obj) {
            if (obj == CurrentImageOnly) {
                return "CurrentImageOnly";
            } else if (obj == CurrentWindowImages) {
                return "CurrentWindowImages";
            } else if (obj == AllImages) {
                return "AllImages";
            } else {
                return "CurrentWindowImages";
            }
        }
    }
}
