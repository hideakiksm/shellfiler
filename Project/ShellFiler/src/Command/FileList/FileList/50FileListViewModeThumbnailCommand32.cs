using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Dialog;
using ShellFiler.FileTask;

namespace ShellFiler.Command.FileList.FileList {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファイル一覧でサムネイル表示のサイズ32×32に切り替えます。
    //   書式 　 FileListViewModeThumbnail32()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.2.0
    //=========================================================================================
    class FileListViewModeThumbnail32Command : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListViewModeThumbnail32Command() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            FileListViewMode viewMode = FileListViewTarget.FileList.FileListViewMode;
            viewMode.ThumbnailModeSwitch = true;
            viewMode.ThumbnailSize = FileListViewIconSize.IconSize32;
            FileListViewTarget.RefreshViewMode(viewMode);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.FileListViewModeThumbnail32Command;
            }
        }
    }
}
