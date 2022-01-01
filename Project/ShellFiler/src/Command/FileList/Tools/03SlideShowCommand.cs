using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.FileTask;
using ShellFiler.GraphicsViewer;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 表示中の対象パスの画像一覧に対してスライドショーを開始します。
    //   書式 　 SlideShow()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class SlideShowCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SlideShowCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 処理開始条件をチェック
            if (FileListViewTarget.FileList.Files.Count == 0) {
                return null;
            }

            // 読み込み対象を準備
            int uiCursor = FileListComponentTarget.CursorLineNo;
            IFileSystem srcFileSystem = FileListViewTarget.FileList.FileSystem;
            List<string> fileList = new List<string>();
            List<UIFile> uiFileList = FileListViewTarget.FileList.Files;
            for (int i = 0; i < uiFileList.Count; i++) {
                if (!uiFileList[i].Attribute.IsDirectory) {
                    string fileName = uiFileList[i].FileName;
                    if (ImageLoader.IsTargetFile(fileName)) {
                        fileList.Add(fileName);
                    }
                }
            }
            if (fileList.Count == 0) {
                InfoBox.Information(Program.MainWindow, Resources.Msg_SlideShowContentsNone);
                return null;
            }

            // ファイルビューアを開く
            IFileListContext fileListContext = BackgroundTaskCommandUtil.CloneFileListContext(FileListViewTarget);               // virtualInfo:ライフサイクル管理
            string basePath = FileListViewTarget.FileList.DisplayDirectoryName;
            GraphicsViewerParameter gvParam = GraphicsViewerParameter.CreateForSlideShow(fileList, 0, srcFileSystem, fileListContext, basePath);
            GraphicsViewerForm viewer = new GraphicsViewerForm(gvParam);
            viewer.Show();

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.SlideShowCommand;
            }
        }
    }
}
