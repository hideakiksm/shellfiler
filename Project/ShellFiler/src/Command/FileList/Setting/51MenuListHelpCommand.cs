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
    // 現在、メニューに割り当てられている機能のコマンド一覧を出力します。
    //   書式 　 MenuListHelp()
    //   引数  　なし
    //   戻り値　なし
    //   対応Ver 1.3.0
    //=========================================================================================
    class MenuListHelpCommand : FileListActionCommand {
        // メニュー階層表現用の色数
        private const int MENU_BACK_COLOR_COUNT = 7;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MenuListHelpCommand() {
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
            string fileList, fileViewer, graphicsViewer;
            int fileListDepth, fileViewerDepth, graphicsViewerDepth;
            CreateCommandHtml(Program.Document.MenuSetting.CreateMenuCustomizedList(CommandUsingSceneType.FileList),
                              commandSpec.FileList, Program.Document.KeySetting.FileListKeyItemList,
                              out fileList, out fileListDepth);
            CreateCommandHtml(Program.Document.MenuSetting.CreateMenuCustomizedList(CommandUsingSceneType.FileViewer),
                              commandSpec.FileViewer, Program.Document.KeySetting.FileViewerKeyItemList,
                              out fileViewer, out fileViewerDepth);
            CreateCommandHtml(Program.Document.MenuSetting.CreateMenuCustomizedList(CommandUsingSceneType.GraphicsViewer),
                              commandSpec.GraphicsViewer, Program.Document.KeySetting.GraphicsViewerKeyItemList,
                              out graphicsViewer, out graphicsViewerDepth);
            string html = Resources.HtmlTemplateMenuList;
            html = html.Replace("{0}", fileListDepth.ToString());
            html = html.Replace("{1}", fileList);
            html = html.Replace("{2}", fileViewerDepth.ToString());
            html = html.Replace("{3}", fileViewer);
            html = html.Replace("{4}", graphicsViewerDepth.ToString());
            html = html.Replace("{5}", graphicsViewer);

            HelpMessageDialog dialog = new HelpMessageDialog(Resources.DlgHelp_TitleMenuList, html);
            dialog.Width = 780;
            dialog.Height = 550;
            dialog.ShowDialog(Program.MainWindow);
            return null;
        }

        //=========================================================================================
        // 機　能：コマンド一覧のHTMLを作成する
        // 引　数：[in]menuList      一覧作成対象のメニュー（各利用シーンのルート項目）
        // 　　　　[in]commandList   作成対象のコマンド一覧
        // 　　　　[in]keySetting    キー設定
        // 　　　　[out]html         作成したHTMLを返す変数
        // 　　　　[out]menuDepthMax メニュー階層の深さの最大を返す変数（[ファイル-開く]は2）
        // 戻り値：なし
        //=========================================================================================
        private void CreateCommandHtml(List<MenuItemSetting> menuList, CommandScene commandList, KeyItemSettingList keySetting, out string html, out int menuDepthMax) {
            menuDepthMax = GetMenuDepthMax(1, menuList);
            StringBuilder htmlBuilder = new StringBuilder();
            CreateCommandHtmlSub(0, 1, menuDepthMax, menuList, commandList, keySetting, htmlBuilder);
            html = htmlBuilder.ToString();
        }

        //=========================================================================================
        // 機　能：コマンド一覧のHTMLを実際に作成する
        // 引　数：[in]color         メニューの背景色に使う色番号
        // 　　　　[in]depth         作成中のメニューの階層の深さ（[ファイル-開く]は2）
        // 　　　　[in]depthMax      メニュー全体での階層の最大の深さ（[ファイル-開く]は2）
        // 　　　　[in]menuList      処理対象のメニュー
        // 　　　　[in]commandList   作成対象のコマンド一覧
        // 　　　　[in]keySetting    キー設定
        // 　　　　[in]html          HTML作成用のバッファ
        // 戻り値：なし
        //=========================================================================================
        private void CreateCommandHtmlSub(int color, int depth, int depthMax, List<MenuItemSetting> menuList, CommandScene commandList, KeyItemSettingList keySetting, StringBuilder html) {
            int childCount = GetChildMenuItemCount(menuList);
            int childColor = color;
            List<CommandGroup> commandGroupList = commandList.CommandGroup;
            for (int i = 0; i < menuList.Count; i++) {
                MenuItemSetting menu = menuList[i];

                // 表示項目を準備
                int currentColor = color;
                string itemName = null;
                string shortcut = null;
                string funcGroup = null;
                string hint = null;
                string keyList = null;
                string comment = null;
                string version = null;
                string prototype = null;
                if (menu.Type == MenuItemSetting.ItemType.SubMenu) {
                    childColor = (childColor + 1) % MENU_BACK_COLOR_COUNT;
                    currentColor = childColor;
                    itemName = menu.ItemNameInput;
                } else if (menu.Type == MenuItemSetting.ItemType.Separator) {
                    itemName = "--------";
                } else if (menu.Type == MenuItemSetting.ItemType.MenuItem) {
                    if (!commandList.ClassNameToApi.ContainsKey(menu.ActionCommandMoniker.CommandType.FullName)) {
                        itemName = "Error: " + menu.ActionCommandMoniker.CommandType.Name;
                    } else {
                        CommandApi api = commandList.ClassNameToApi[menu.ActionCommandMoniker.CommandType.FullName];
                        string[] paramList = GetCommandArgumentValueForDisplay(api, menu.ActionCommandMoniker);
                        string[] paramListApi = GetCommandArgumentValueForApi(api, menu.ActionCommandMoniker);
                        ActionCommand command = api.Moniker.CreateActionCommand();

                        currentColor = color;
                        itemName = (menu.ItemNameInput != null ? menu.ItemNameInput : command.UIResource.Hint);
                        shortcut = (menu.ShortcutKey == MenuItemSetting.SHORTCUT_KEY_NONE ? null : menu.ShortcutKey.ToString());
                        funcGroup = api.ParentGroup.GroupDisplayName;
                        hint = command.UIResource.Hint;
                        keyList = ToolBarImpl.CreateShortcutDisplayString(keySetting, menu.ActionCommandMoniker);
                        comment = string.Format(api.Comment, paramList);
                        int intVersion = command.UIResource.FirstVersion;
                        version = (intVersion / 1000000) + "." + ((intVersion % 1000000) / 10000) + "." + ((intVersion % 10000) / 100);
                        prototype = CommandListHelpCommand.GetFunctionPrototype(api, paramListApi);
                    }
                }

                // エスケープ
                itemName = CommandListHelpCommand.HtmlEscape(itemName);
                shortcut = CommandListHelpCommand.HtmlEscape(shortcut);
                funcGroup = CommandListHelpCommand.HtmlEscape(funcGroup);
                hint = CommandListHelpCommand.HtmlEscape(hint);
                keyList = CommandListHelpCommand.HtmlEscape(keyList);
                comment = CommandListHelpCommand.HtmlEscape(comment);
                version = CommandListHelpCommand.HtmlEscape(version);
                prototype = CommandListHelpCommand.HtmlEscape(prototype);

                // 整形
                html.Append("<tr>");
                if (i == 0 && depth != 1) {
                    html.Append("<td class=\"c" + color + "\" rowspan=\"" + childCount + "\">&nbsp;</td>");
                }
                if (depthMax - depth + 1 < 2) {
                    html.Append("<td class=\"c" + currentColor + "\">");
                } else {
                    html.Append("<td class=\"c" + currentColor + "\" colspan=\"" + (depthMax - depth + 1) + "\">");
                }
                html.Append(itemName);
                html.Append("</td><td>");
                html.Append(shortcut);
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

                // サブメニューのHTMLを作成
                if (menu.Type == MenuItemSetting.ItemType.SubMenu) {
                    CreateCommandHtmlSub(currentColor, depth + 1, depthMax, menu.SubMenuList, commandList, keySetting, html);
                }
            }
        }

        //=========================================================================================
        // 機　能：子のメニュー階層全体の項目数を数える
        // 引　数：[in]menuList      処理対象のメニュー
        // 戻り値：項目数
        //=========================================================================================
        private int GetChildMenuItemCount(List<MenuItemSetting> menuList) {
            int childCount = 0;
            for (int i = 0; i < menuList.Count; i++) {
                MenuItemSetting menu = menuList[i];
                if (menu.Type == MenuItemSetting.ItemType.SubMenu) {
                    childCount += GetChildMenuItemCount(menu.SubMenuList);
                }
                childCount++;
            }
            return childCount;
        }

        //=========================================================================================
        // 機　能：メニュー階層全体での最大の深さを返す
        // 引　数：[in]menuList      処理対象のメニュー
        // 戻り値：処理対象メニュー以下の深さ
        //=========================================================================================
        private int GetMenuDepthMax(int depth, List<MenuItemSetting> menuList) {
            int depthMax = depth;
            for (int i = 0; i < menuList.Count; i++) {
                if (menuList[i].Type == MenuItemSetting.ItemType.SubMenu) {
                    depthMax = Math.Max(depthMax, GetMenuDepthMax(depth + 1, menuList[i].SubMenuList));
                }
            }
            return depthMax;
        }

        //=========================================================================================
        // 機　能：モニカに割り当てられている引数値を表示説明用の文字列表現した配列を返す
        // 引　数：[in]api      API仕様
        // 　　　　[in]moniker  実行対象のモニカ
        // 戻り値：引数値を文字列表現した配列
        //=========================================================================================
        public static string[] GetCommandArgumentValueForDisplay(CommandApi api, ActionCommandMoniker moniker) {
            string[] paramList = new string[api.ArgumentList.Count];
            for (int i = 0; i < api.ArgumentList.Count; i++) {
                object argValue = moniker.Parameter[i];
                string strValue = "";
                if (api.ArgumentList[i].Type == CommandArgument.ArgumentType.TypeBool) {
                    strValue = ((bool)argValue ? "Yes" : "No");
                } else {
                    if (argValue == null) {
                        strValue = "";
                    } else {
                        strValue = argValue.ToString();
                    }
                }
                paramList[i] = string.Format(Resources.DlgKeySettingOption_ArgumentQuote, strValue);
            }
            return paramList;
        }

        //=========================================================================================
        // 機　能：モニカに割り当てられている引数値をAPI説明用の文字列表現した配列を返す
        // 引　数：[in]api      API仕様
        // 　　　　[in]moniker  実行対象のモニカ
        // 戻り値：引数値を文字列表現した配列
        //=========================================================================================
        public static string[] GetCommandArgumentValueForApi(CommandApi api, ActionCommandMoniker moniker) {
            string[] paramList = new string[api.ArgumentList.Count];
            for (int i = 0; i < api.ArgumentList.Count; i++) {
                object argValue = moniker.Parameter[i];
                string strValue = "";
                CommandArgument.ArgumentType argType = api.ArgumentList[i].Type;
                if (argType == CommandArgument.ArgumentType.TypeBool) {
                    strValue = ((bool)argValue ? "true" : "false");
                } else {
                    if (argValue == null) {
                        strValue = "";
                    } else {
                        strValue = argValue.ToString();
                    }
                    if (argType == CommandArgument.ArgumentType.TypeString || argType == CommandArgument.ArgumentType.TypeMenuItem) {
                        strValue = "\"" + strValue + "\"";
                    }
                }
                paramList[i] = strValue;
            }
            return paramList;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.MenuListHelpCommand;
            }
        }
    }
}
