using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ImageCombine {

    //=========================================================================================
    // クラス：アプリケーションのメインエントリポイント
    //=========================================================================================
    static class Program {

        //=========================================================================================
        // 機　能：アプリケーションのメインエントリポイント
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        [STAThread]
        static void Main() {
            string workspacePath = Assembly.GetEntryAssembly().Location;
            workspacePath = Path.GetDirectoryName(workspacePath) + @"\..\..\..\..\";

            CombineImage(workspacePath + @"ShellFiler\Resources\", "ImageListIcon");
            CombineImage(workspacePath + @"ShellFiler\Resources\", "ImageListBGManagerAnimation");
        }

        //=========================================================================================
        // 機　能：イメージを結合する
        // 引　数：[in]baseFolder   処理対象のフォルダ名フルパス
        // 　　　　[in]subFolder    元画像一覧が入っているフォルダ名/結合後画像のファイル名主部
        // 戻り値：なし
        // メ　モ：baseFolder+subFolderから画像を取得、例えばI1_だけを抽出してbaseFolder+I1+subFolder.pngに結合
        //=========================================================================================
        private static void CombineImage(string baseFolder, string subFolder) {
            string[] imageFileArray = Directory.GetFiles(baseFolder + subFolder);
            List<string> imageFiles100 = SeparateImageList(imageFileArray, "I100_");
            List<string> imageFiles200 = SeparateImageList(imageFileArray, "I200_");
            List<string> imageFiles400 = SeparateImageList(imageFileArray, "I400_");

            CheckImage("I100_".Length, imageFiles100, imageFiles200, imageFiles400);

            SaveImage(imageFiles100, baseFolder + "I100_" + subFolder + ".png");
            SaveImage(imageFiles200, baseFolder + "I200_" + subFolder + ".png");
            SaveImage(imageFiles400, baseFolder + "I400_" + subFolder + ".png");
        }

        //=========================================================================================
        // 機　能：画像ファイルの構成をチェックする
        // 引　数：[in]prefixLength  プレフィックス部分の文字長（この長さ分は無視）
        // 　　　　[in]images        画像一覧リストの配列
        // 戻り値：なし
        //=========================================================================================
        private static List<string> SeparateImageList(string[] org, string prefix) {
            // ファイル一覧を取得
            List<string> imageFileList = org.Where(name => Path.GetFileName(name).StartsWith(prefix)).ToList();
            imageFileList.Sort();
            return imageFileList;
        }

        //=========================================================================================
        // 機　能：画像ファイルの構成をチェックする
        // 引　数：[in]prefixLength  プレフィックス部分の文字長（この長さ分は無視）
        // 　　　　[in]images        画像一覧リストの配列
        // 戻り値：なし
        //=========================================================================================
        private static void CheckImage(int prefixLength, params List<string>[] images) {
            for (int i = 1; i < images.Length; i++) {
                for (int j = 0; j < images[0].Count; j++) {
                    string name0 = Path.GetFileName(images[0][j]);
                    string nameI = Path.GetFileName(images[i][j]);
                    if (name0.Substring(prefixLength) != nameI.Substring(prefixLength)) {
                        throw new Exception("Mismatch name");
                    }
                }
                if (images[0].Count != images[i].Count) {
                    throw new Exception("Mismatch count");
                }
            }
        }

        //=========================================================================================
        // 機　能：イメージを結合して保存する
        // 引　数：[in]imageFileList   結合対象の画像パス一覧
        // 　　　　[in]imageFile       画像の保存先
        // 戻り値：なし
        //=========================================================================================
        private static void SaveImage(List<string> imageFileList, string imageFile) {
            // 幅と高さを取得
            Bitmap bmpTest = new Bitmap(imageFileList[0]);
            int width = bmpTest.Width;
            int height = bmpTest.Height;
            bmpTest.Dispose();

            // 結合イメージを作成
            CombinedImage combinedImage = new CombinedImage(width, height, imageFileList.Count);
            for (int i = 0; i < imageFileList.Count; i++) {
                Bitmap bmp = new Bitmap(imageFileList[i]);
                if (bmp.Width != width || bmp.Height != height) {
                    throw new Exception("Mismatch size");
                }
                combinedImage.AddImage(i, bmp);
            }
            combinedImage.SaveImage(imageFile);
        }
    }
}
