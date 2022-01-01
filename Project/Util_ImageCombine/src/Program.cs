using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
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

            CombineImage(workspacePath + @"ShellFiler\Resources\ImageListIcon");
            CombineImage(workspacePath + @"ShellFiler\Resources\ImageListBGManagerAnimation");
        }

        //=========================================================================================
        // 機　能：イメージを結合する
        // 引　数：[in]folder   処理対象のフォルダ
        // 戻り値：なし
        //=========================================================================================
        private static void CombineImage(string folder) {
            string[] imageFileArray = Directory.GetFiles(folder);

            // 幅と高さを取得
            Bitmap bmpTest = new Bitmap(imageFileArray[0]);
            int width = bmpTest.Width;
            int height = bmpTest.Height;
            bmpTest.Dispose();

            // ファイル一覧を取得
            List<string> imageFileList = new List<string>();
            imageFileList.AddRange(imageFileArray);
            imageFileList.Sort();

            // 結合イメージを作成
            CombinedImage combinedImage = new CombinedImage(width, height, imageFileList.Count);
            for (int i = 0; i < imageFileList.Count; i++) {
                Bitmap bmp = new Bitmap(imageFileList[i]);
                combinedImage.AddImage(i, bmp);
            }
            combinedImage.SaveImage(folder + ".png");
        }
    }
}
