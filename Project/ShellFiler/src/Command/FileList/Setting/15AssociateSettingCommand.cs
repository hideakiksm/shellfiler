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
    // 関連づけの設定を行います。
    //   書式 　 AssociateSetting()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.0.0
    //=========================================================================================
    class AssociateSettingCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AssociateSettingCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // 外部で更新されているかチェック
            KeySetting saved = new KeySetting();
            saved.LoadSetting(null);
            if (saved.LastFileWriteTime > Program.Document.KeySetting.LastFileWriteTime) {
                ConfirmSettingUpdatedDialog confirm = new ConfirmSettingUpdatedDialog(Resources.DlgConfirmSetting_KeySetting);
                DialogResult resultConfirm = confirm.ShowDialog(Program.MainWindow);
                if (resultConfirm != DialogResult.OK) {
                    return null;
                }
                if (confirm.LoadExternalConfig) {
                    Program.Document.KeySetting = saved;
                }
            }

            // API一覧を取得
            CommandApiLoader loader = new CommandApiLoader();
            CommandSpec commandSpec = loader.Load();
            if (commandSpec == null) {
                return null;
            }

            // オプションを編集
            KeySetting prev = (KeySetting)(Program.Document.KeySetting.Clone());
            AssociateSettingDialog dialog = new AssociateSettingDialog(Program.Document.KeySetting, commandSpec);
            DialogResult resultSetting = dialog.ShowDialog(Program.MainWindow);
            if (resultSetting != DialogResult.OK) {
                Program.Document.KeySetting = prev;
                return null;
            }
            if (!KeySetting.EqualsConfig(prev, Program.Document.KeySetting)) {
                // 更新されていれば保存
                Program.Document.KeySetting.SaveSetting();
                Program.MainWindow.FunctionBar.ResetButtonState();
                Program.MainWindow.MainMenu.RefreshItemName();
            }
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.AssociateSettingCommand;
            }
        }
    }
}
