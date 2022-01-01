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
    // ShellFilerのコマンド一覧を出力します。
    //   書式 　 CommandListHelp()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class CommandListHelpCommand : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CommandListHelpCommand() {
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
            string html = Resources.HtmlTemplateCommandList;
            html = html.Replace("{0}", fileList);
            html = html.Replace("{1}", fileViewer);
            html = html.Replace("{2}", graphicsViewer);

            HelpMessageDialog dialog = new HelpMessageDialog(Resources.DlgHelp_TitleCommandList, null, html);
            dialog.Width = 780;
            dialog.Height = 550;
            dialog.ShowDialog(Program.MainWindow);
            return null;
        }

        //=========================================================================================
        // 機　能：コマンド一覧のHTMLを作成する
        // 引　数：[in]commandList   作成対象のコマンド一覧
        // 　　　　[in]keySetting    キー設定
        // 戻り値：作成したHTML
        //=========================================================================================
        private string CreateCommandHtml(CommandScene commandList, KeyItemSettingList keySetting) {
            StringBuilder html = new StringBuilder();
            List<CommandGroup> commandGroupList = commandList.CommandGroup;
            for (int i = 0; i < commandGroupList.Count; i++) {
                CommandGroup group = commandGroupList[i];
                List<CommandApi> apiList = group.FunctionList;
                for (int j = 0; j < apiList.Count; j++) {
                    CommandApi api = apiList[j];
                    string[] paramListDummy = new string[api.ArgumentList.Count];
                    for (int k = 0; k < api.ArgumentList.Count; k++) {
                        paramListDummy[k] = Resources.DlgKeySetting_ParameterDummy;
                    }
                    ActionCommand command = api.Moniker.CreateActionCommand();
                    string prototype = GetFunctionPrototype(api, null);
                    string keyList = ToolBarImpl.CreateShortcutDisplayString(keySetting, api.Moniker);
                    if (keyList == null) {
                        keyList = "&nbsp;";
                    }
                    int intVersion = command.UIResource.FirstVersion;
                    string version = (intVersion / 1000000) + "." + ((intVersion % 1000000) / 10000) + "." + ((intVersion % 10000) / 100);
                    string comment = string.Format(api.Comment, paramListDummy);
                    html.Append("<tr><td>");
                    html.Append(group.GroupDisplayName);
                    html.Append("</td><td>");
                    html.Append(command.UIResource.Hint);
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
            }
            return html.ToString();
        }

        //=========================================================================================
        // 機　能：実行する機能の内部名プロトタイプを作成する
        // 引　数：[in]api        プロトタイプを作成する機能のAPI仕様
        // 　　　　[in]paramList  埋め込むパラメータのリスト（パラメータが不定のときnull）
        // 戻り値：関数名の形式の文字列
        //=========================================================================================
        public static string GetFunctionPrototype(CommandApi api, string[] paramList) {
            string resultType = "";
            CommandRetType retType = api.Moniker.CreateActionCommand().UIResource.CommandReturnType;
            switch (retType) {
                case CommandRetType.Void:
                    resultType = "void ";
                    break;
                case CommandRetType.Bool:
                    resultType = "bool ";
                    break;
                case CommandRetType.String:
                    resultType = "string ";
                    break;
                case CommandRetType.Int:
                    resultType = "int ";
                    break;
            }
            List<CommandArgument> argList = api.ArgumentList;
            StringBuilder apiParam = new StringBuilder();
            apiParam.Append(resultType);
            apiParam.Append(api.CommandName);
            apiParam.Append("(");
            for (int i = 0; i < argList.Count; i++) {
                if (i != 0) {
                    apiParam.Append(", ");
                }
                if (paramList != null) {
                    apiParam.Append(paramList[i]);
                } else {
                    switch (argList[i].Type) {
                        case CommandArgument.ArgumentType.TypeString:
                            apiParam.Append("string ");
                            break;
                        case CommandArgument.ArgumentType.TypeInt:
                            apiParam.Append("int ");
                            break;
                        case CommandArgument.ArgumentType.TypeBool:
                            apiParam.Append("bool ");
                            break;
                        case CommandArgument.ArgumentType.TypeMenuItem:
                            apiParam.Append("menu ");
                            break;
                    }
                    apiParam.Append(argList[i].ArgumentName);
                }
            }
            apiParam.Append(")");
            return apiParam.ToString();
        }

        //=========================================================================================
        // 機　能：HTML特殊文字をエスケープする
        // 引　数：[in]value   エスケープ前の文字列
        // 戻り値：エスケープ後の文字列
        //=========================================================================================
        public static string HtmlEscape(string value) {
            if (value != null) {
                value = HttpUtility.HtmlEncode(value);
            } else {
                value = "&nbsp;";
            }
            return value;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.CommandListHelpCommand;
            }
        }
    }
}
