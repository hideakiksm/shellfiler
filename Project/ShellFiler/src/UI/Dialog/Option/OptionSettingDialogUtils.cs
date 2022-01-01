using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // インターフェース：オプションダイアログ用のユーティリティ
    //=========================================================================================
    public class OptionSettingDialogUtils {

        //=========================================================================================
        // 機　能：指定ディレクトリの存在を確認する
        // 引　数：[in]tempDir    作業ディレクトリ
        // 　　　　[in]msgCreate  ディレクトリを作成するときに出力するメッセージ
        // 戻り値：指定ディレクトリが適切なときtrue
        //=========================================================================================
        public static bool CheckDirctory(string tempDir, string msgCreate) {
            if (Directory.Exists(tempDir)) {
                return true;
            }

            DialogResult result = InfoBox.Question(Program.MainWindow, MessageBoxButtons.YesNo, msgCreate);
            if (result != DialogResult.Yes) {
                return false;
            }

            try {
                Directory.CreateDirectory(tempDir);
            } catch (Exception) {
                return false;
            }
            return true;
        }
    }
}
