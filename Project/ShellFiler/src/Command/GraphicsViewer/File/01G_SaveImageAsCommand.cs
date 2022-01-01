using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.FileSystem;
using ShellFiler.Locale;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Command.GraphicsViewer.File {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 画像を指定のファイル名で保存します。
    //   書式 　 G_SaveImageAs()
    //   引数  　なし
    //   戻り値　保存できたときtrue、キャンセルまたは失敗したときfalse
    //   対応Ver 1.0.0
    //=========================================================================================
    class G_SaveImageAsCommand : GraphicsViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_SaveImageAsCommand() {
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
            if (GraphicsViewerPanel.GraphicsViewerForm.GraphicsViewerParameter.GraphicsViewerMode == ShellFiler.GraphicsViewer.GraphicsViewerMode.ClipboardViewer) {
                fileName = string.Format(Resources.GraphicsViewer_SaveAsFileName, DateTimeFormatter.DateTimeToInformationForFile(DateTime.Now));
            } else if (GraphicsViewerPanel.CurrentImage.FilePath != null && GraphicsViewerPanel.CurrentImage.Image != null) {
                string orgFileName = GenericFileStringUtils.GetFileName(GraphicsViewerPanel.CurrentImage.FilePath);
                fileName = GenericFileStringUtils.GetFileNameBody(orgFileName) + DateTimeFormatter.DateTimeToInformationForFile(DateTime.Now) + "." + GenericFileStringUtils.GetExtensionLast(orgFileName);
            } else {
                // スライドショーの最初と最後のメッセージなど
                InfoBox.Information(GraphicsViewerPanel.GraphicsViewerForm, Resources.Msg_GraphicsViewerSaveNotSupported);
                return false;
            }

            // ファイルを開く
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = fileName;
            sfd.InitialDirectory = folder;
            sfd.Filter = Resources.GraphicsViewer_SaveAsFilter;
            sfd.FilterIndex = 4;
            sfd.RestoreDirectory = true;
            DialogResult result = sfd.ShowDialog(GraphicsViewerPanel.GraphicsViewerForm);
            if (result != DialogResult.OK) {
                return false;
            }
            bool success = GraphicsViewerPanel.SaveImageDataAs(sfd.FileName);
            return success;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_SaveImageAsCommand;
            }
        }
    }
}
