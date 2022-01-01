using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.UI.FileList.Crawler {

    //=========================================================================================
    // クラス：バックグラウンドでファイルをクロールするスレッド
    //=========================================================================================
    class FileCrawlerExtractIcon : FileClawler {
        // 親となるスレッド
        private FileCrawlThread m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileCrawlerExtractIcon(FileCrawlThread parent) {
            m_parent = parent;
        }

        //=========================================================================================
        // 機　能：クロールを実行する
        // 引　数：[in]request   リクエスト
        // 戻り値：なし
        //=========================================================================================
        public override void Execute(FileCrawlRequest request) {
            UIFileList fileList = request.FileList;             // このスレッドからはアクセスできない
            List<UIFile> fileListFiles = request.FileListFiles;
            UIFileListId uiFileListId = request.UiFileListId;
            FileSystemID fileSystemId = request.FileSystemId;
            string directoryName = request.DirectoryName;
            Program.Document.FileIconManager.ClearWindowIcon(uiFileListId);

            for (int index = 0; index < fileListFiles.Count; index++) {
                if (m_parent.CrawlEnd) {
                    break;
                }

                Bitmap bmp = ExtractIcon(fileListFiles, directoryName, fileSystemId, index);
                if (bmp != null) {
                    FileIconID iconId = Program.Document.FileIconManager.AddIconBitmap(uiFileListId, FileListViewIconSize.IconSize16, bmp);
                    if (iconId != FileIconID.NullId && index < fileListFiles.Count) {
                        UIFile file = fileListFiles[index];
                        file.FileIconId = iconId;
                        m_parent.DrawFileIcon(fileList, index, file);
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
        // 機　能：ファイルアイコンの抽出用にクロールを実行する
        // 引　数：[in]fileListFiles  対象のファイル一覧
        // 　　　　[in]directoryName  ファイル一覧のディレクトリ名
        // 　　　　[in]fileSystemId   ファイル一覧のファイルシステムのID
        // 　　　　[in]index          一覧中の対象ファイルのインデックス
        // 戻り値：なし
        //=========================================================================================
        public static Bitmap ExtractIcon(List<UIFile> fileListFiles, string directoryName, FileSystemID fileSystemId, int index) {
            UIFile file = fileListFiles[index];

            // アイコン情報を取得
            Bitmap bmp = null;
            if (file.FileName == "..") {
                bmp = null;
            } else if (FileSystemID.IsWindows(fileSystemId)) {
                // Windowsなら完全にファイル情報を取得
                string path = WindowsFileUtils.CombineFilePath(directoryName, file.FileName);
                bmp = Win32IconUtils.GetFileIconBitmap(path, IconSize.Small16, Win32IconUtils.ICONMODE_WITH_OVERRAY);
            } else {
                // Windows以外はファイルから簡易的に取得（ディレクトリはそのまま）
                if (!file.Attribute.IsDirectory) {
                    bmp = Win32IconUtils.GetFileIconBitmap(file.FileName, IconSize.Small16, Win32IconUtils.ICONMODE_NORMAL);
                } else {
                    bmp = null;
                }
            }
            return bmp;
        }
    }
}
