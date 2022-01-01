using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;

namespace ShellFiler.GraphicsViewer {

    //=========================================================================================
    // クラス：ドラッグ中の慣性の速度
    //=========================================================================================
    class DraggingMouseAccelaration {
        // X方向の速度[pixel/milliseconds]
        public double XVector;

        // Y方向の速度[pixel/milliseconds]
        public double YVector;

        // X位置[pixel]
        public double XPos;

        // Y位置[pixel]
        public double YPos;

        // 測定した時刻
        public DateTime Time;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]xVector     X方向の速度[pixel/milliseconds]
        // 　　　　[in]yVector     Y方向の速度[pixel/milliseconds]
        // 　　　　[in]xPos        X位置[pixel]
        // 　　　　[in]yPos        Y位置[pixel]
        // 　　　　[in]time  測定した時刻
        // 戻り値：最終画像からさらに進めようとしたときtrue
        //=========================================================================================
        public DraggingMouseAccelaration(double xVector, double yVector, double xPos, double yPos, DateTime time) {
            XVector = xVector;
            YVector = yVector;
            XPos = xPos;
            YPos = yPos;
            Time = time;
        }
    }
}
