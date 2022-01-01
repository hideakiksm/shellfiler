using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ImageCombine {

    //=========================================================================================
    // クラス：結合されたイメージの作成クラス
    //=========================================================================================
    class CombinedImage {
        // 元のイメージ1件あたりの幅
        private int m_imageWidth;

        // 元のイメージ1件あたりの高さ
        private int m_imageHeight;

        // 結合したイメージ
        private Bitmap m_resultImage;

        // 結合したイメージへの書き込み用グラフィックス
        private Graphics m_graphics;

        //=========================================================================================
        // 機　能：イメージを結合する
        // 引　数：[in]width    元のイメージ1件あたりの幅
        // 　　　　[in]height   元のイメージ1件あたりの高さ
        // 　　　　[in]count    イメージ数
        // 戻り値：なし
        //=========================================================================================
        public CombinedImage(int width, int height, int count) {
            m_imageWidth = width;
            m_imageHeight = height;
            m_resultImage = new Bitmap(m_imageWidth * count, m_imageHeight, PixelFormat.Format32bppArgb);
            m_graphics = Graphics.FromImage(m_resultImage);
        }

        //=========================================================================================
        // 機　能：イメージを追加する
        // 引　数：[in]index    追加するインデックス
        // 　　　　[in]bmp      追加するイメージ
        // 戻り値：なし
        //=========================================================================================
        public void AddImage(int index, Bitmap bmp) {
            Rectangle rcSrc = new Rectangle(0, 0, m_imageWidth, m_imageHeight);
            Rectangle rcDest = new Rectangle(index * m_imageWidth, 0, m_imageWidth, m_imageHeight);
            m_graphics.DrawImage(bmp, rcDest, rcSrc,GraphicsUnit.Pixel); 
        }

        //=========================================================================================
        // 機　能：結合したイメージを保存する
        // 引　数：[in]filePath  出力先ファイルのパス
        // 戻り値：なし
        //=========================================================================================
        public void SaveImage(string filePath) {
            m_graphics.Dispose();
            m_graphics = null;

            // イメージを作成
            MemoryStream streamPng = new MemoryStream();
            try {
                m_resultImage.Save(streamPng, ImageFormat.Png);
            } finally {
                streamPng.Close();
            }
            byte[] pngData = streamPng.ToArray();

            // ファイル出力
            Stream outStream = new FileStream(filePath, FileMode.Create);
            try {
                outStream.Write(pngData, 0, pngData.Length);
            } finally {
                outStream.Close();
            }
        }
    }
}
