using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.Option;

namespace ShellFiler.Command.FileList.Setting {

    //=========================================================================================
    // クラス：コマンドを実行する
    // オプション設定を行います。
    //   書式 　 OptionSetting()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class OptionSettingCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public OptionSettingCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            OpenOptionSettingDialog(Program.MainWindow);
            return null;
        }

        //=========================================================================================
        // 機　能：オプション設定画面を表示する
        // 引　数：[in]parent   ダイアログの親になるフォーム
        // 戻り値：なし
        //=========================================================================================
        public static void OpenOptionSettingDialog(Form parent) {
            // 外部で更新されているかチェック
            bool success = CheckConfigurationUpdate(parent);
            if (!success) {
                return;
            }

            // オプションを編集
            Configuration prev = (Configuration)(Configuration.Current.Clone());
            OptionSettingDialog dialog = new OptionSettingDialog();
            dialog.ShowDialog(parent);
            if (!Configuration.EqualsConfig(prev, Configuration.Current)) {
                // 更新されていれば保存
                Configuration.Current.SaveSetting();
            }
        }

        //=========================================================================================
        // 機　能：コンフィグが外部で更新されているかどうかを確認する
        // 引　数：[in]parent   ダイアログの親になるフォーム
        // 戻り値：処理を続けてよいときtrue
        //=========================================================================================
        public static bool CheckConfigurationUpdate(Form parent) {
            DateTime savedTime = Configuration.SavedConfigLastWriteTime;
            if (savedTime > Configuration.Current.LastFileWriteTime) {
                ConfirmSettingUpdatedDialog confirm = new ConfirmSettingUpdatedDialog(Resources.DlgConfirmSetting_Option);
                DialogResult result = confirm.ShowDialog(parent);
                if (result != DialogResult.OK) {
                    return false;
                }
                if (confirm.LoadExternalConfig) {
                    Configuration saved = new Configuration();
                    saved.LoadSetting();
                    Configuration.Current = saved;
                } else {
                    Configuration.Current.LastFileWriteTime = DateTime.Now;
                }
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.OptionSettingCommand;
            }
        }
    }
}
