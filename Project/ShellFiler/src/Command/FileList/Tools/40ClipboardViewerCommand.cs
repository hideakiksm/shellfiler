using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI.Dialog;
using ShellFiler.FileViewer;
using ShellFiler.GraphicsViewer;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // クリップボードの内容をビューアで表示します。
    //   書式 　 ClipboardViewer()
    //   引数  　なし
    //   戻り値　bool:ビューアを起動したときtrue、キャンセルしたときfalseを返します。
    //   対応Ver 1.0.0
    //=========================================================================================
    class ClipboardViewerCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ClipboardViewerCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public override object Execute() {
            // クリップボードの内容を確認
            IDataObject data = Clipboard.GetDataObject();
            string text = null;
            if (data.GetDataPresent(DataFormats.OemText)) {
                text = (string)data.GetData(DataFormats.OemText, false);
            }
            BufferedImage image = null;
            if (data.GetDataPresent(DataFormats.Bitmap)) {
                image = new BufferedImage();
                image.Image = (Image)data.GetData(DataFormats.Bitmap);
            }
            if (text == null && image == null) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotUseClipboardData);
            }

            // 両方の場合は選択
            if (text != null && image != null) {
                SelectClipboardViewerTypeDialog dialog = new SelectClipboardViewerTypeDialog();
                DialogResult result = dialog.ShowDialog(Program.MainWindow);
                if (result != DialogResult.OK) {
                    return false;
                }
                if (dialog.IsImage) {
                    text = null;
                } else {
                    image = null;
                }
            }

            // ビューアを起動
            if (text != null) {
                ClipboardDataSource dataSource = new ClipboardDataSource(text);
                FileViewerForm fileViewer = new FileViewerForm(FileViewerForm.ViewerMode.TextView, dataSource, BackgroundTaskID.NullId);
                fileViewer.Show();
                dataSource.FireEvent();
            } else if (image != null) {
                int colorBits = Image.GetPixelFormatSize(image.Image.PixelFormat);
                ImageInfo imageInfo = ImageInfo.Success("", image, colorBits);
                GraphicsViewerParameter gvParam = GraphicsViewerParameter.CreateForClipboardViewer(imageInfo);
                GraphicsViewerForm graphicsViewer = new GraphicsViewerForm(gvParam);
                graphicsViewer.Show();
            }

            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ClipboardViewerCommand;
            }
        }
    }
}
