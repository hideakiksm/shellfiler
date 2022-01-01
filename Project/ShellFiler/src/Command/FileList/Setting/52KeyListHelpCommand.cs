using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Web;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command.FileList.Internal;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.Properties;
using ShellFiler.Locale;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Dialog.KeyOption;

namespace ShellFiler.Command.FileList.Setting {

    //=========================================================================================
    // クラス：コマンドを実行する
    // 現在、キーに割り当てられている機能のコマンド一覧を出力します。
    //   書式 　 KeyListHelp()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class KeyListHelpCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public KeyListHelpCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            // コマンド一覧を取得
            CommandApiLoader loader = new CommandApiLoader();
            CommandSpec commandSpec = loader.Load();
            if (commandSpec == null) {
                return null;
            }

            // HTMLを整形
            string fileList = CreateCommandHtml(commandSpec.FileList, Program.Document.KeySetting.FileListKeyItemList);
            string fileViewer = CreateCommandHtml(commandSpec.FileViewer, Program.Document.KeySetting.FileViewerKeyItemList);
            string graphicsViewer = CreateCommandHtml(commandSpec.GraphicsViewer, Program.Document.KeySetting.GraphicsViewerKeyItemList);
            string html = Resources.HtmlTemplateKeyList;
            html = html.Replace("{0}", fileList);
            html = html.Replace("{1}", fileViewer);
            html = html.Replace("{2}", graphicsViewer);

            HelpMessageDialog dialog = new HelpMessageDialog(Resources.DlgHelp_TitleKeyList, null, html);
            dialog.Width = 780;
            dialog.Height = 550;
            dialog.ShowDialog(Program.MainWindow);
            return null;
        }

        //=========================================================================================
        // 機　能：キー一覧のHTMLを作成する
        // 引　数：[in]commandList   コマンド一覧
        // 　　　　[in]keySetting    作成対象のキー設定一覧
        // 戻り値：作成したHTML
        //=========================================================================================
        private string CreateCommandHtml(CommandScene commandList, KeyItemSettingList keySetting) {
            StringBuilder html = new StringBuilder();
            for (int i = 0; i < keySetting.AllKeySettingList.Count; i++) {
                KeyItemSetting key = keySetting.AllKeySettingList[i];

                // 表示項目を準備
                string monikerName = key.ActionCommandMoniker.CommandType.FullName;
                if (!commandList.ClassNameToApi.ContainsKey(monikerName)) {
                    continue;
                }
                CommandApi api = commandList.ClassNameToApi[monikerName];
                string[] paramList = MenuListHelpCommand.GetCommandArgumentValueForDisplay(api, key.ActionCommandMoniker);
                string[] paramListApi = MenuListHelpCommand.GetCommandArgumentValueForApi(api, key.ActionCommandMoniker);
                ActionCommand command = api.Moniker.CreateActionCommand();

                string keyName = key.KeyState.GetDisplayName(keySetting);
                string funcGroup = api.ParentGroup.GroupDisplayName;
                string hint = command.UIResource.Hint;
                string keyList = ToolBarImpl.CreateShortcutDisplayString(keySetting, key.ActionCommandMoniker);
                string comment = string.Format(api.Comment, paramList);
                int intVersion = command.UIResource.FirstVersion;
                string version = (intVersion / 1000000) + "." + ((intVersion % 1000000) / 10000) + "." + ((intVersion % 10000) / 100);
                string prototype = CommandListHelpCommand.GetFunctionPrototype(api, paramListApi);

                // エスケープ
                keyName = CommandListHelpCommand.HtmlEscape(keyName);
                funcGroup = CommandListHelpCommand.HtmlEscape(funcGroup);
                hint = CommandListHelpCommand.HtmlEscape(hint);
                keyList = CommandListHelpCommand.HtmlEscape(keyList);
                comment = CommandListHelpCommand.HtmlEscape(comment);
                version = CommandListHelpCommand.HtmlEscape(version);
                prototype = CommandListHelpCommand.HtmlEscape(prototype);

                // 整形
                html.Append("<tr><td>");
                html.Append(keyName);
                html.Append("</td><td>");
                html.Append(funcGroup);
                html.Append("</td><td>");
                html.Append(hint);
                html.Append("</td><td>");
                html.Append(keyList);
                html.Append("</td><td>");
                html.Append(comment);
                html.Append("</td><td>");
                html.Append(version);
                html.Append("</td><td>");
                html.Append(prototype);
                html.Append("</td></tr>");
            }
            return html.ToString();
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.KeyListHelpCommand;
            }
        }
    }
}
