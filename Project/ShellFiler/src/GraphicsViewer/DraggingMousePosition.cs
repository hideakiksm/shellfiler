using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;

namespace ShellFiler.GraphicsViewer {

    //=========================================================================================
    // クラス：ドラッグ中のマウスの位置
    //=========================================================================================
    class DraggingMousePosition {
        // null相当の値
        public static readonly DraggingMousePosition NullValue = new DraggingMousePosition(0, 0, DateTime.MinValue);

        // X位置
        public int X;

        // Y位置
        public int Y;

        // 測定した時刻
        public DateTime Time;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]x     X座標
        // 　　　　[in]y     Y座標
        // 　　　　[in]time  測定した時刻
        // 戻り値：最終画像からさらに進めようとしたときtrue
        //=========================================================================================
        public DraggingMousePosition(int x, int y, DateTime time) {
            X = x;
            Y = y;
            Time = time;
        }
    }
}
