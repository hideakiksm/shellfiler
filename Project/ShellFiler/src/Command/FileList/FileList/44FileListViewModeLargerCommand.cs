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
    // ファイル一覧のサムネイルを1段階大きくします。一番大きい状態のときはサムネイルモードにします。
    //   書式 　 FileListViewModeLarger()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 2.2.0
    //=========================================================================================
    class FileListViewModeLargerCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListViewModeLargerCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            FileListViewMode viewMode = FileListViewTarget.FileList.FileListViewMode;
            if (viewMode.ThumbnailModeSwitch) {
                viewMode.ThumbnailSize = FileListViewIconSize.GetLarger(viewMode.ThumbnailSize);
            } else {
                viewMode.ThumbnailModeSwitch = true;
                viewMode.ThumbnailSize = FileListViewIconSize.IconSize32;
            }
            FileListViewTarget.RefreshViewMode(viewMode);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.FileListViewModeLargerCommand;
            }
        }
    }
}
