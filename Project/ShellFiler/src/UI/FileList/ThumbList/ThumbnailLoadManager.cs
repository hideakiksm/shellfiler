using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.UI.FileList.Crawler;
using ShellFiler.Util;
using ShellFiler.Locale;
using System.Runtime.InteropServices;

namespace ShellFiler.UI.FileList.ThumbList {

    //=========================================================================================
    // クラス：サムネイルの読み込み対象を管理するクラス
    //=========================================================================================
    class ThumbnailLoadManager {
        // 読み込む画像のサイズ
        private Size m_imageSize;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]imageSize  読み込む画像のサイズ
        // 戻り値：なし
        //=========================================================================================
        public ThumbnailLoadManager(Size imageSize) {
            m_imageSize = imageSize;
        }

        //=========================================================================================
        // 機　能：サムネイルを使用できなかったファイルを追加する
        // 引　数：[in]uiFileList  ファイル一覧
        // 　　　　[in]topLine     最上部に表示する行番号
        // 　　　　[in]cursorX     カーソルのX位置
        // 　　　　[in]cursorY     カーソルのY位置
        // 　　　　[in]columnSize  横方向に格納できるファイル数
        // 　　　　[in]lineize    縦方向に格納できるファイル数
        // 戻り値：リクエストに使用するパラメータ（リクエストしないときnull）
        //=========================================================================================
        public FileCrawlerCreateThumbnailRequestParam CreateLoadRequest(UIFileList uiFileList, int topLine, int cursorX, int cursorY, int columnSize, int lineSize) {
            List<UIFileWithIndex> useFiles = new List<UIFileWithIndex>();

            // 表示中のファイル一覧を追加
            List<UIFile> files = uiFileList.Files;
            const int EXTEND_LINE = 1;                       // 上下行のキャッシュ分
            int startIndex = Math.Max(0, topLine * columnSize);
            int endIndex = Math.Min(files.Count - 1, (topLine + lineSize + 1) * columnSize - 1);
            for (int i = startIndex; i <= endIndex; i++) {
                useFiles.Add(new UIFileWithIndex(files[i], i));
            }

            // 上方向に予備行を追加
            if (startIndex != 0) {
                int startExtIndex = Math.Max(0, startIndex - EXTEND_LINE * columnSize);
                for (int i = startIndex - 1; i >= startExtIndex; i--) {
                    useFiles.Add(new UIFileWithIndex(files[i], i));
                }
            }

            // 下方向に予備行を追加
            if (endIndex != files.Count - 1) {
                int endExtIndex = Math.Min(files.Count - 1, endIndex + EXTEND_LINE * columnSize);
                for (int i = endIndex + 1; i <= endExtIndex; i++) {
                    useFiles.Add(new UIFileWithIndex(files[i], i));
                }
            }

            // 情報を整理
            Program.Document.FileIconManager.SetUseThumbnailFiles(uiFileList, useFiles);
            if (useFiles.Count > 0) {
                FileCrawlerCreateThumbnailRequestParam loadRequest = new FileCrawlerCreateThumbnailRequestParam(useFiles, m_imageSize);
                return loadRequest;
            } else {
                return null;
            }
        }
    }
}
