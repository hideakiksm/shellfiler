using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem.Windows;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList.Crawler;
using ShellFiler.UI.FileList;

namespace ShellFiler.UI.FileList.Crawler {

    //=========================================================================================
    // クラス：バックグラウンドでファイルのサムネイルを読み込むスレッド
    //=========================================================================================
    class FileCrawlerCreateThumbnail : FileClawler {
        // 親となるスレッド
        private FileCrawlThread m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileCrawlerCreateThumbnail(FileCrawlThread parent) {
            m_parent = parent;
        }

        //=========================================================================================
        // 機　能：クロールを実行する
        // 引　数：[in]request   リクエスト
        // 戻り値：なし
        //=========================================================================================
        public override void Execute(FileCrawlRequest request) {
            UIFileList fileList = request.FileList;             // このスレッドからはアクセスできない
            UIFileListId uiFileListId = request.UiFileListId;
            FileSystemID fileSystemId = request.FileSystemId;
            string directoryName = request.DirectoryName;
            FileCrawlerCreateThumbnailRequestParam param = (FileCrawlerCreateThumbnailRequestParam)request.Param;
            Size imageSize = param.ImageSize;
            List<UIFileWithIndex> targetFiles = param.TargetFiles;

            for (int i = 0; i < targetFiles.Count; i++) {
                if (m_parent.CrawlEnd) {
                    break;
                }

                UIFile uiFile = targetFiles[i].UIFile;
                int index = targetFiles[i].Index;
                Bitmap bmp = LoadThumbnail(uiFile, directoryName, fileSystemId, imageSize);

                if (bmp != null) {
                    FileListViewIconSize iconSize = FileListViewIconSize.FromSize(imageSize);
                    FileIconID iconId = Program.Document.FileIconManager.AddIconBitmap(uiFileListId, iconSize, bmp);
                    if (iconId != FileIconID.NullId) {
                        uiFile.FileIconId = iconId;
                        m_parent.DrawFileIcon(fileList, index, uiFile);
                    }
                }

                Thread.Sleep(10);

                bool requestCanceled = m_parent.CheckRequestCanceled(request);
                if (requestCanceled) {
                    break;
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルのサムネイルを読み込む
        // 引　数：[in]uiFile         対象のファイル
        // 　　　　[in]directoryName  ファイル一覧のディレクトリ名
        // 　　　　[in]fileSystemId   ファイル一覧のファイルシステムのID
        // 　　　　[in]imageSize      読み込む画像のサイズ
        // 戻り値：読み込んだサムネイル（読み込めなかったときnull）
        //=========================================================================================
        private Bitmap LoadThumbnail(UIFile uiFile, string directoryName, FileSystemID fileSystemId, Size imageSize) {
            IconSize iconSize = IconSize.GetInsideIconSize(imageSize);
            if (uiFile.FileIconId.IconType != FileIconID.FileIconType.DefaultIcon) {
                FileIcon iconLoaded = Program.Document.FileIconManager.GetFileIcon(uiFile.FileIconId, FileIconID.NullId, FileListViewIconSize.FromSize(imageSize));
                if (iconLoaded != null) {
                    return null;
                }
            }

            // アイコン情報を取得
            Bitmap bmp = null;
            if (uiFile.FileName == "..") {
                bmp = null;
            } else if (FileSystemID.IsWindows(fileSystemId)) {
                // Windowsなら完全にファイル情報を取得
                string path = WindowsFileUtils.CombineFilePath(directoryName, uiFile.FileName);
                ThumbNailBuilder builder = new ThumbNailBuilder();
                try {
                    bmp = builder.CreateThumbnailImage(path, imageSize.Width, imageSize.Height);
                } finally {
                    builder.Dispose();
                }
                if (bmp == null) {
                    // サムネイルが作れなければアイコンで代用
                    bmp = Win32IconUtils.GetFileIconBitmap(path, iconSize, Win32IconUtils.ICONMODE_WITH_OVERRAY);
                }
            } else {
                // Windows以外はファイルから簡易的に取得（ディレクトリはそのまま）
                if (!uiFile.Attribute.IsDirectory) {
                    bmp = Win32IconUtils.GetFileIconBitmap(uiFile.FileName, iconSize, Win32IconUtils.ICONMODE_NORMAL);
                } else {
                    bmp = null;
                }
            }
            return bmp;
        }
    }
}
