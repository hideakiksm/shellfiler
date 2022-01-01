using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.UI;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：画像関連ライブラリ
    //=========================================================================================
    public class GraphicsUtils {
        // MeasureStringで幅指定をしない場合の無限幅値
        public const int INFINITY_WIDTH = 100000;

        // モノクロ変換カラーマトリックス用値配列
        private static readonly float[][] s_colorMatrixElements = new float[][] {
            new float[]{0.299F, 0.299F, 0.299F, 0, 0},
            new float[]{0.587F, 0.587F, 0.587F, 0, 0},
            new float[]{0.114F, 0.114F, 0.114F, 0, 0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{0, 0, 0, 0, 1}
        };

        //=========================================================================================
        // 機　能：指定されたビットマップをモノクロにする
        // 引　数：[in]bmpBase  元のビットマップ
        // 戻り値：モノクロになったビットマップ
        //=========================================================================================
        public static Bitmap TransformToMono(Bitmap bmpBase) {
            // 変換作業用 BitMap イメージを生成
            Bitmap bmp = new Bitmap(bmpBase.Width, bmpBase.Height);
            Graphics g = Graphics.FromImage(bmp);
            try {
                // 変換オブジェクトの作成
                ColorMatrix cm = new ColorMatrix(s_colorMatrixElements);
                ImageAttributes ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                // ImageAttributesを使用してモノクロ画像を描画
                g.DrawImage(bmpBase, new Rectangle(0, 0, bmpBase.Width, bmpBase.Height), 0, 0, bmpBase.Width, bmpBase.Height, GraphicsUnit.Pixel, ia);
            } finally {
                g.Dispose();
            }

            return bmp;
        }

        //=========================================================================================
        // 機　能：文字列の表示幅を取得する
        // 引　数：[in]g        描画に使用するグラフィックス
        // 　　　　[in]font     描画に使用するフォント
        // 　　　　[in]strLine  表示する文字
        // 戻り値：文字幅のピクセル数
        //=========================================================================================
        public static float MeasureString(Graphics g, Font font, string strLine) {
            if (strLine == "") {
                return 0;
            }
            if (strLine.IndexOf((char)0xfffd) != -1) {       // 不明な文字は誤動作するため「.」に置換
                strLine = strLine.Replace((char)0xfffd, '.');
            }
            Region[] charRegion;
            StringFormat sf = new StringFormat();
            try {
                sf.SetMeasurableCharacterRanges(new CharacterRange[1] { new CharacterRange(0, strLine.Length) });
                charRegion = g.MeasureCharacterRanges(strLine + ".", font, new RectangleF(0, 0, INFINITY_WIDTH, INFINITY_WIDTH), sf);
            } finally {
                sf.Dispose();
            }
            float width = charRegion[0].GetBounds(g).Right;
            return width;
        }

        //=========================================================================================
        // 機　能：画面幅に対する固定幅の最大文字数を取得する
        // 引　数：[in]g        描画に使用するグラフィックス
        // 　　　　[in]font     描画に使用するフォント
        // 　　　　[in]width    画面のピクセル数
        // 　　　　[in]maxChar  予測される最大文字数
        // 戻り値：最大文字数
        //=========================================================================================
        public static int GetCharWidth(Graphics g, Font font, float width, int maxChar) {
            int left = 1;
            int right = maxChar;
            float  widthStr;
            while (left < right) {
                int middle = (left + right) / 2;
                string str = StringUtils.Repeat("M", middle);
                 widthStr = GraphicsUtils.MeasureString(g, font, str);
                if (widthStr > width) {
                    right = middle - 1;
                } else {
                    left = middle + 1;
                }
            }
            return right;
        }

        //=========================================================================================
        // 機　能：イメージリストの項目を無効状態で描画する
        // 引　数：[in]g           描画に使用するグラフィックス
        // 　　　　[in]imageList   イメージリスト
        // 　　　　[in]imageIndex  描画対象のイメージ
        // 　　　　[in]xPos        描画X位置
        // 　　　　[in]yPos        描画Y位置
        // 　　　　[in]colorBase   イメージの背景の色
        // 戻り値：なし
        //=========================================================================================
        public static void DrawDisabledImageList(Graphics g, ImageList imageList, int imageIndex, int xPos, int yPos, Color colorBase) {
            Bitmap bmp = new Bitmap(imageList.ImageSize.Width, imageList.ImageSize.Height, PixelFormat.Format32bppArgb);
            try {
                Graphics gBmp = Graphics.FromImage(bmp);
                try {
                    imageList.Draw(gBmp, 0, 0, imageIndex);
                } finally {
                    gBmp.Dispose();
                }
                ControlPaint.DrawImageDisabled(g, bmp, xPos, yPos, colorBase);
            } finally {
                bmp.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：色を混ぜた結果を返す
        // 引　数：[in]colList  混ぜる色１
        // 戻り値：なし
        //=========================================================================================
        public static Color BrendColor(params Color[] colList) {
            int r = 0;
            int g = 0;
            int b = 0;
            for (int i = 0; i < colList.Length; i++) {
                r += colList[i].R;
                g += colList[i].G;
                b += colList[i].B;
            }
            r = r / colList.Length;
            g = g / colList.Length;
            b = b / colList.Length;
            Color col = Color.FromArgb(r, g, b);
            return col;
        }

        //=========================================================================================
        // 機　能：指定された色をモノクロにした場合の色を返す
        // 引　数：[in]color  元の色
        // 戻り値：モノクロにした色
        //=========================================================================================
        public static Color ToMonochrome(Color color) {
            int y = Math.Min(255, (int)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B));
            return Color.FromArgb(y, y, y);
        }

        //=========================================================================================
        // 機　能：ビットマップ画像のハッシュ値を返す
        // 引　数：[in]image  画像
        // 戻り値：ハッシュ値
        //=========================================================================================
        public static long GetImageDataHash(Bitmap image) {
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] byteData;
            if (bitmapData.Stride > 0) {
                int byteSize = bitmapData.Stride * image.Height;
                byteData = new byte[byteSize];
                Marshal.Copy(bitmapData.Scan0, byteData, 0, byteSize);
            } else {
                int absStride = -bitmapData.Stride;
                byteData = new byte[absStride * image.Height];
                for (int i = 0; i < image.Height; i++) {
                    IntPtr pointer = new IntPtr(bitmapData.Scan0.ToInt32() + (bitmapData.Stride * i));
                    Marshal.Copy(pointer, byteData, absStride * (image.Height - i - 1), absStride);
                }
            }
            image.UnlockBits(bitmapData);
            long hash = ObjectUtils.GetMD5HashLong(byteData);
            return hash;
        }
    }
}
