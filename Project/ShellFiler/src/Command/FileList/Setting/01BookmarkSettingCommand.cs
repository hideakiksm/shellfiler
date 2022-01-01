using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileList.Setting {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ブックマークの設定を行います。
    //   書式 　 BookmarkSetting()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class BookmarkSettingCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public BookmarkSettingCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            BookmarkSetting bookmarkSetting = Program.Document.UserSetting.BookmarkSetting;
            BookmarkSettingDialog dialog = new BookmarkSettingDialog(bookmarkSetting, FileListViewTarget.FileList.DisplayDirectoryName);
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK) {
                return null;
            }

            bookmarkSetting = dialog.BookmarkSetting;
            Program.Document.UserSetting.BookmarkSetting = bookmarkSetting;
            bookmarkSetting.SaveData();

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.BookmarkSettingCommand;
            }
        }
    }
}
