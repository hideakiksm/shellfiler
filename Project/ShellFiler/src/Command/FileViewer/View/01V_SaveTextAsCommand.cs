using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.Command.FileViewer.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // テキストを指定のファイル名で保存します。
    //   書式 　 V_SaveTextAs()
    //   引数  　なし
    //   戻り値　保存できたときtrue、キャンセルまたは失敗したときfalse
    //   対応Ver 1.0.0
    //=========================================================================================
    class V_SaveTextAsCommand : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_SaveTextAsCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 初期フォルダを決定
            string folder = ActionCommandUtils.GetDefaultWindowsFolder();

            // ファイル名を決定
            string fileName;
            if (TextFileViewer.TextBufferLineInfo.TargetFile.FilePath == null) {
                fileName = string.Format(Resources.FileViewer_SaveAsFileName, DateTimeFormatter.DateTimeToInformationForFile(DateTime.Now));
            } else {
                string orgFileName = GenericFileStringUtils.GetFileName(TextFileViewer.TextBufferLineInfo.TargetFile.FilePath);
                fileName = GenericFileStringUtils.GetFileNameBody(orgFileName) + DateTimeFormatter.DateTimeToInformationForFile(DateTime.Now) + "." + GenericFileStringUtils.GetExtensionLast(orgFileName);
            }

            // ファイルを開く
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = fileName;
            sfd.InitialDirectory = folder;
            sfd.Filter = Resources.FileViewer_SaveAsFilter;
            sfd.FilterIndex = 3;
            sfd.RestoreDirectory = true;
            DialogResult result = sfd.ShowDialog(TextFileViewer.FileViewerForm);
            if (result != DialogResult.OK) {
                return false;
            }
            bool success = TextFileViewer.SaveTextDataAs(sfd.FileName);
            return success;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_SaveTextAsCommand;
            }
        }
    }
}
