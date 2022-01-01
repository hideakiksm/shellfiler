using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document.Setting;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：コマンドAPI一覧のローダ
    //=========================================================================================
    class CommandApiLoader {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CommandApiLoader() {
        }

        //=========================================================================================
        // 機　能：API一覧を読み込む
        // 引　数：なし
        // 戻り値：読み込んだAPI一覧（失敗したときnull）
        //=========================================================================================
        public CommandSpec Load() {
            // ファイルから読み込む
            string fileName = DirectoryManager.CommandApiList;
            SettingLoader loader = new SettingLoader(fileName);
            CommandSpec spec;
            bool success = LoadSetting(loader, out spec);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CommandApiFileLoad, fileName, loader.ErrorDetail);
                return null;
            }
            return spec;
        }

        //=========================================================================================
        // 機　能：API一覧を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[out]obj    読み込んだ結果を返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadSetting(SettingLoader loader, out CommandSpec obj) {
            // ファイルから読み込む
            obj = null;
            bool success = loader.LoadSetting(true);
            if (!success) {
                return false;
            }

            // タグを読み込む
            obj = new CommandSpec();
            success = loader.ExpectTag(SettingTag.Command_CommandSpec, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.Command_CommandSpec) {
                    break;

                // インストール情報＞全般
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Command_FileList) {
                    success = LoadCommandScene(loader, out obj.FileList, CommandUsingSceneType.FileList);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Command_FileList, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Command_FileViewer) {
                    success = LoadCommandScene(loader, out obj.FileViewer, CommandUsingSceneType.FileViewer);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Command_FileViewer, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Command_GraphicsViewer) {
                    success = LoadCommandScene(loader, out obj.GraphicsViewer, CommandUsingSceneType.GraphicsViewer);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.Command_GraphicsViewer, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：APIのシーン一覧を読み込む
        // 引　数：[in]loader    読み込み用クラス
        // 　　　　[out]obj      読み込んだ結果を返す変数
        // 　　　　[in]sceneType コマンドの利用シーン
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadCommandScene(SettingLoader loader, out CommandScene obj, CommandUsingSceneType sceneType) {
            obj = null;
            bool success;
            success = loader.ExpectTag(SettingTag.Command_CommandScene, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            obj = new CommandScene();
            obj.CommandSceneType = sceneType;
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.Command_CommandScene) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Command_CommandGroupList) {
                    while (true) {
                        bool fit;
                        success = loader.PeekTag(SettingTag.Command_CommandGroup, SettingTagType.BeginObject, out fit);
                        if (!success) {
                            return false;
                        }
                        if (!fit) {
                            success = loader.ExpectTag(SettingTag.Command_CommandGroupList, SettingTagType.EndObject);
                            if (!success) {
                                return false;
                            }
                            break;
                        }
                        CommandGroup group;
                        success = LoadCommandGroup(loader, obj, out group);
                        if (!success)  {
                            return false;
                        }
                        obj.CommandGroup.Add(group);
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：APIのグループ一覧を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]scene   読み込み中のコマンドの利用シーン
        // 　　　　[out]obj    読み込んだ結果を返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadCommandGroup(SettingLoader loader, CommandScene scene, out CommandGroup obj) {
            obj = null;
            bool success;
            success = loader.ExpectTag(SettingTag.Command_CommandGroup, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            string package = null;
            obj = new CommandGroup();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.Command_CommandGroup) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Command_GroupDisplayName) {
                    obj.GroupDisplayName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Command_PackageName) {
                    package = loader.StringValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Command_FunctionList) {
                    if (package == null) {
                        return false;
                    }
                    while (true) {
                        bool fit;
                        success = loader.PeekTag(SettingTag.Command_CommandApi, SettingTagType.BeginObject, out fit);
                        if (!success) {
                            return false;
                        }
                        if (!fit) {
                            success = loader.ExpectTag(SettingTag.Command_FunctionList, SettingTagType.EndObject);
                            if (!success) {
                                return false;
                            }
                            break;
                        }
                        CommandApi api;
                        success = LoadCommandGroup(loader, scene, package, out api);
                        if (!success) {
                            return false;
                        }
                        api.ParentGroup = obj;
                        obj.FunctionList.Add(api);
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：APIの機能一覧を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]scene   読み込み中のコマンドの利用シーン
        // 　　　　[in]package APIを実装したクラスのパッケージ
        // 　　　　[out]obj    読み込んだ結果を返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadCommandGroup(SettingLoader loader, CommandScene scene, string package, out CommandApi obj) {
            obj = null;
            bool success;
            success = loader.ExpectTag(SettingTag.Command_CommandApi, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            obj = new CommandApi();
            List<object> defaultParamList = new List<object>();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.Command_CommandApi) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Command_CommandName) {
                    obj.CommandName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Command_Comment) {
                    obj.Comment = loader.StringValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.Command_ArgumentList) {
                    while (true) {
                        bool fit;
                        success = loader.PeekTag(SettingTag.Command_CommandArgument, SettingTagType.BeginObject, out fit);
                        if (!success) {
                            return false;
                        }
                        if (!fit) {
                            success = loader.ExpectTag(SettingTag.Command_ArgumentList, SettingTagType.EndObject);
                            if (!success) {
                                return false;
                            }
                            break;
                        }
                        CommandArgument argument;
                        success = LoadCommandArgument(loader, out argument);
                        if (!success) {
                            return false;
                        }
                        obj.ArgumentList.Add(argument);
                        defaultParamList.Add(argument.DefaultValue);
                    }
                }
            }
            if (obj.CommandName == null || obj.Comment == null) {
                return false;
            }

            string className = package + "." + obj.CommandName + "Command";
            obj.CommandClassName = className;
            Type type = Type.GetType(className);
            if (type == null) {
                return false;
            }
            ActionCommandMoniker moniker = new ActionCommandMoniker(ActionCommandOption.None, type, defaultParamList.ToArray());
            obj.Moniker = moniker;
            scene.ClassNameToApi.Add(className, obj);

            return true;
        }

        //=========================================================================================
        // 機　能：APIの引数一覧を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[out]obj    読み込んだ結果を返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadCommandArgument(SettingLoader loader, out CommandArgument obj) {
            obj = null;
            bool success;
            success = loader.ExpectTag(SettingTag.Command_CommandArgument, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            string argumentName = null;
            string argumentType = null;
            string argumentComment = null;
            string defaultValue = null;
            string valueRange = null;
            obj = new CommandArgument();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.Command_CommandArgument) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Command_ArgumentName) {
                    argumentName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Command_ArgumentType) {
                    argumentType = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Command_ArgumentComment) {
                    argumentComment = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Command_DefaultValue) {
                    defaultValue = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.Command_ValueRange) {
                    valueRange = loader.StringValue;
                }
            }
            if (argumentName == null || argumentComment == null || argumentType == null) {
                return false;
            }
            obj.ArgumentName = argumentName;
            obj.ArgumentComment = argumentComment;
            success = ConvertCommandArgument(argumentType, defaultValue, valueRange, obj);
            if (!success) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：CommandArgumentを内部形式に変換する
        // 引　数：[in]argumentType   引数の型の文字列表現
        // 　　　　[in]defaultValue   デフォルト値の文字列表現
        // 　　　　[in]valueRange     値の範囲の文字列表現
        // 　　　　[in]arg            変換した内部形式を返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool ConvertCommandArgument(string argumentType, string defaultValue, string valueRange, CommandArgument arg) {
            if (argumentType == "int") {
                // int型引数の変換
                arg.Type = CommandArgument.ArgumentType.TypeInt;
                arg.DefaultValue = int.Parse(defaultValue);
                string[] minMax = valueRange.Split(',');
                if (!int.TryParse(minMax[0], out arg.ValueRangeIntMin)) {
                    return false;
                }
                if (!int.TryParse(minMax[1], out arg.ValueRangeIntMax)) {
                    return false;
                }
                arg.ValueRangeString = null;
            } else if (argumentType == "string") {
                // string型引数の変換
                arg.Type = CommandArgument.ArgumentType.TypeString;
                arg.DefaultValue = defaultValue;
                arg.ValueRangeIntMin = -1;
                arg.ValueRangeIntMax = -1;
                arg.ValueRangeString = new List<CommandArgument.StringSelect>();
                if (valueRange != "") {
                    string[] valueCommentList = valueRange.Split(',');
                    foreach (string valueComment in valueCommentList) {
                        string[] valueCommentSplit = valueComment.Split('=');
                        CommandArgument.StringSelect select = new CommandArgument.StringSelect();
                        select.InnerValue = valueCommentSplit[0];
                        select.DisplayName = valueCommentSplit[1];
                        arg.ValueRangeString.Add(select);
                    }
                }
            } else if (argumentType == "bool") {
                // bool型引数の変換
                arg.Type = CommandArgument.ArgumentType.TypeBool;
                arg.DefaultValue = bool.Parse(defaultValue);
                arg.ValueRangeIntMin = -1;
                arg.ValueRangeIntMax = -1;
                arg.ValueRangeString = null;
            } else if (argumentType == "menu") {
                // menu型引数の変換
                arg.Type = CommandArgument.ArgumentType.TypeMenuItem;
                arg.DefaultValue = "";
                arg.ValueRangeIntMin = -1;
                arg.ValueRangeIntMax = -1;
                arg.ValueRangeString = null;
            }
            return true;
        }
    }
}
