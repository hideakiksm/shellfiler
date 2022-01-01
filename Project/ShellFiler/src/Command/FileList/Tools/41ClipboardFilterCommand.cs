using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI.Dialog;
using ShellFiler.FileViewer;
using ShellFiler.GraphicsViewer;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.Properties;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.Command.FileList.Tools {

    //=========================================================================================
    // クラス：コマンドを実行する
    // クリップボードの内容をテキストフィルターで変換してから保存します。
    //   書式 　 ClipboardFilter()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.0.0
    //=========================================================================================
    class ClipboardFilterCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ClipboardFilterCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override object Execute() {
            // クリップボードの内容を確認
            IDataObject data = Clipboard.GetDataObject();
            string text = null;
            if (data.GetDataPresent(DataFormats.OemText)) {
                text = (string)data.GetData(DataFormats.OemText, false);
            }
            if (text == null) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotUseClipboardData);
                return null;
            }

            // 保存先ファイルのデフォルトを決定
            string folder = ActionCommandUtils.GetDefaultWindowsFolder();
            string fileName = folder + string.Format(Resources.DlgFileFilter_ClipClipboardFileName, DateTimeFormatter.DateTimeToInformationForFile(DateTime.Now));

            // フィルター選択ダイアログを表示
            FileFilterSetting setting = Program.Document.UserGeneralSetting.FileFilterSetting;
            setting.LoadData();
            FileFilterClipboardSetting clipboardSetting = (FileFilterClipboardSetting)(setting.ClipboardSetting.Clone());
            FileFilterClipboardDialog dialog = new FileFilterClipboardDialog(clipboardSetting, fileName);
            DialogResult result = dialog.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return null;
            }
            setting.ClipboardSetting = dialog.FileFilterSetting;
            setting.SaveData();

            // フィルタリング処理を実行
            EncodingType encoding = dialog.ClipboardCharset;
            byte[] fileContents = encoding.Encoding.GetBytes(text);
            FileFilterManager filterManager = Program.Document.FileFilterManager;
            byte[] filteredContents;
            WaitHandle dummyEvent = new ManualResetEvent(false);
            FileOperationStatus status = filterManager.Convert(null, fileContents, out filteredContents, clipboardSetting.CurrentSetting.FilterList, dummyEvent);
            if (!status.Succeeded) {
                InfoBox.Warning(Program.MainWindow, Resources.FileFilter_MsgConvertError, status.Message);
                return null;
            }

            // 保存
            string destPath = dialog.SaveFilePath;
            if (destPath != null) {
                try {
                    File.WriteAllBytes(destPath, filteredContents);
                } catch (Exception e) {
                    InfoBox.Warning(Program.MainWindow, Resources.FileFilter_MsgFileWriteError, destPath, e.Message);
                    return null;
                }
                InfoBox.Information(Program.MainWindow, Resources.FileFilter_MsgFilterSuccess);
            } else {
                string strResult = encoding.Encoding.GetString(filteredContents);
                Clipboard.SetDataObject(strResult, true);
                InfoBox.Information(Program.MainWindow, Resources.FileFilter_MsgFilterSuccess2);
            }

            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ClipboardFilterCommand;
            }
        }
    }
}
