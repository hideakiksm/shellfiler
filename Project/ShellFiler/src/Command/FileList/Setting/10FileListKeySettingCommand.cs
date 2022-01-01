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
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.UI.Dialog.Option;

namespace ShellFiler.Command.FileList.Setting {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ファイル一覧でのキー割り当ての変更を行います。
    //   書式 　 FileListKeySetting()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class FileListKeySettingCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListKeySettingCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            ExecuteKeySetting(CommandUsingSceneType.FileList);
            return null;
        }

        //=========================================================================================
        // 機　能：キー設定を実行する
        // 引　数：[in]scene  キーの利用シーン（ファイル一覧/ファイルビューア/グラフィックビューア）
        // 戻り値：なし
        //=========================================================================================
        public static void ExecuteKeySetting(CommandUsingSceneType scene) {
            // 外部で更新されているかチェック
            KeySetting saved = new KeySetting();
            saved.LoadSetting(null);
            if (saved.LastFileWriteTime > Program.Document.KeySetting.LastFileWriteTime) {
                ConfirmSettingUpdatedDialog confirm = new ConfirmSettingUpdatedDialog(Resources.DlgConfirmSetting_KeySetting);
                DialogResult resultConfirm = confirm.ShowDialog(Program.MainWindow);
                if (resultConfirm != DialogResult.OK) {
                    return;
                }
                if (confirm.LoadExternalConfig) {
                    Program.Document.KeySetting = saved;
                }
            }

            // API一覧を取得
            CommandApiLoader loader = new CommandApiLoader();
            CommandSpec commandSpec = loader.Load();
            if (commandSpec == null) {
                return;
            }

            // オプションを編集
            KeySetting prev = (KeySetting)(Program.Document.KeySetting.Clone());
            KeySettingDialog dialog = new KeySettingDialog(Program.Document.KeySetting, commandSpec, scene);
            DialogResult resultSetting = dialog.ShowDialog(Program.MainWindow);
            if (resultSetting != DialogResult.OK) {
                Program.Document.KeySetting = prev;
                return;
            }
            if (!KeySetting.EqualsConfig(prev, Program.Document.KeySetting)) {
                // 更新されていれば保存
                Program.Document.KeySetting.SaveSetting();
                Program.MainWindow.ResetUIItems();
                Program.MainWindow.FunctionBar.ResetButtonState();
            }

            return;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.FileListKeySettingCommand;
            }
        }
    }
}
