using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.UI.FileList;
using ShellFiler.FileViewer;
using ShellFiler.GraphicsViewer;
using ShellFiler.MonitoringViewer;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Management;
using ShellFiler.Terminal.UI;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.Command.FileList.SSH;
using ShellFiler.Command.FileList.Window;
using ShellFiler.Command.FileList.Internal;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.Command.MonitoringViewer;
using ShellFiler.Command.GraphicsViewer;
using ShellFiler.Command.GraphicsViewer.View;
using ShellFiler.Command.GraphicsViewer.File;
using ShellFiler.Command.Terminal;
using ShellFiler.FileSystem;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：キーやマウス入力からCommandを生成するためのファクトリ
    //=========================================================================================
    class CommandFactory {
        // ２ストロークキーの状態
        TwoStrokeKeyState m_twoStrokeKeyState = new TwoStrokeKeyState();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public CommandFactory() {
        }

        //=========================================================================================
        // 機　能：キー入力からコマンドを生成する
        // 引　数：[in]evt           キーイベント
        // 　　　　[in]fileListView  対象のファイルリスト
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public FileListActionCommand CreateFromKeyInput(KeyCommand evt, FileListView fileListView) {
            // 実行対象のキーを作成
            KeyState keyState;
            TwoStrokeType twoStroke = m_twoStrokeKeyState.GetTwoStrokeState(CommandUsingSceneType.FileList, Program.MainWindow);
            if (twoStroke == TwoStrokeType.None) {
                keyState = new KeyState(evt.KeyCode, evt.Shift, evt.Control, evt.Alt);
            } else {
                keyState = new KeyState(evt.KeyCode, twoStroke);
            }
            m_twoStrokeKeyState.ResetKeyState();
            FileListActionCommand command = CreateFromKeyInputDirect(keyState, fileListView);
            return command;
        }

        //=========================================================================================
        // 機　能：キー入力からコマンドを生成する
        // 引　数：[in]keyState      入力したキー
        // 　　　　[in]fileListView  対象のファイルリスト
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public FileListActionCommand CreateFromKeyInputDirect(KeyState keyState, FileListView fileListView) {
            // コマンドを取得
            KeyItemSetting setting = Program.Document.KeySetting.FileListKeyItemList.GetSettingFromKey(keyState);
            if (setting == null) {
                return null;
            }

            ActionCommandMoniker moniker = setting.ActionCommandMoniker;
            FileListActionCommand command = null;
            if (moniker != null) {
                command = (FileListActionCommand)(moniker.CreateActionCommand());
                command.Initialize(fileListView, null, moniker.Option);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：マウス入力からコマンドを生成する
        // 引　数：[in]evt           マウスイベント
        // 　　　　[in]fileListView  対象のファイルリスト
        // 　　　　[in]dblClick      ダブルクリックのときtrue
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public FileListActionCommand CreateFromMouseInput(MouseEventArgs evt, FileListView fileListView, bool dblClick) {
            Keys modifierKeys = Control.ModifierKeys;
            bool shift = ((modifierKeys & Keys.Shift) == Keys.Shift);
            bool control = ((modifierKeys & Keys.Control) == Keys.Control);
            bool alt = ((modifierKeys & Keys.Alt) == Keys.Alt);

            FileListActionCommand command = null;
            ActionCommandMoniker moniker = null;
            ActionCommandOption option = ActionCommandOption.None;
            if (dblClick) {
                // ダブルクリック
                switch (evt.Button) {
                    case MouseButtons.Left:
                        KeyState keyState = new KeyState(Keys.Enter, shift, control, alt);
                        KeyItemSetting setting = Program.Document.KeySetting.FileListKeyItemList.GetSettingFromKey(keyState);
                        if (setting != null) {
                            moniker = setting.ActionCommandMoniker;
                            option = setting.ActionCommandMoniker.Option;
                        }
                        break;
                    case MouseButtons.Middle:
                        break;
                    case MouseButtons.Right:
                        break;
                }
                if (moniker != null) {
                    command = (FileListActionCommand)(moniker.CreateActionCommand());
                    option = moniker.Option;
                }
            } else {
                // マウスクリック
                Keys keyCode = Keys.None;
                switch (evt.Button) {
                    case MouseButtons.Left:
                        keyCode = Keys.LButton;
                        break;
                    case MouseButtons.Middle:
                        keyCode = Keys.MButton;
                        break;
                    case MouseButtons.Right:
                        keyCode = Keys.RButton;
                        break;
                }
                // キー定義からコマンドを作成
                KeyState keyState = new KeyState(keyCode, shift, control, alt);
                KeyItemSetting setting = Program.Document.KeySetting.FileListKeyItemList.GetSettingFromKey(keyState);
                if (setting == null) {
                    return null;
                }
                moniker = setting.ActionCommandMoniker;
                if (moniker != null) {
                    command = (FileListActionCommand)(moniker.CreateActionCommand());
                    option = moniker.Option;
                }
            }

            // コマンドを初期化
            if (command != null) {
                command.Initialize(fileListView, evt, option);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：UIによるコマンド入力からコマンドを生成する
        // 引　数：[in]sender  イベント発生原因の送信元の種別
        // 　　　　[in]item    発生したイベントの項目
        // 　　　　[in]fileListView  対象のファイルリスト
        // 戻り値：コマンド（対応するコマンドがないときはnull）
        //=========================================================================================
        public FileListActionCommand CreateFromUICommand(UICommandSender sender, UICommandItem item, FileListView fileListView) {
            FileListActionCommand command = null;
            ActionCommandOption option = ActionCommandOption.None;
            if (item == UICommandItem.TaskManager) {
                command = new TaskManagerCommand();
            } else if (item == UICommandItem.StatusBar) {
                command = new ChdirTargetToOppositeCommand();
            } else if (item == UICommandItem.StateList_NoTask) {
                command = new TaskManagerCommand();
            } else if (item == UICommandItem.StateList_NoSSH) {
                command = new ChdirSSHFolderCommand();
            } else if (item is UICommandItem.HeaderSortCommand) {
                UICommandItem.HeaderSortCommand headerItem = (UICommandItem.HeaderSortCommand)item;
                command = new InternalSortDirectCommand();
                command.SetParameter(headerItem.SortMode, headerItem.SortDirection);
            } else {
                command = (FileListActionCommand)(item.ActionCommandMoniker.CreateActionCommand());
                option = item.ActionCommandMoniker.Option;
            }
            if (command != null) {
                command.Initialize(fileListView, null, option);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：モニカからコマンドを生成する
        // 引　数：[in]moniker       コマンド作成のためのモニカ
        // 　　　　[in]fileListView  対象のファイルリスト
        // 戻り値：コマンド
        //=========================================================================================
        public FileListActionCommand CreateFromMoniker(ActionCommandMoniker moniker, FileListView fileListView) {
            FileListActionCommand command = (FileListActionCommand)(moniker.CreateActionCommand());
            ActionCommandOption option = moniker.Option;
            command.Initialize(fileListView, null, option);
            return command;
        }

        //=========================================================================================
        // 機　能：アドレスバーからディレクトリ変更コマンドを生成する
        // 引　数：[in]inputDir      入力されたディレクトリ名（フィルター設定のときnull）
        // 　　　　[in]fileListView  対象のファイルリスト
        // 戻り値：コマンド（対応するコマンドがないときはnull）
        //=========================================================================================
        public FileListActionCommand CreateFromAddressBarCommand(string inputDir, FileListView fileListView) {
            FileListActionCommand command;
            if (inputDir == null) {
                command = new FileListFilterMenuCommand();
                command.Initialize(fileListView, null, ActionCommandOption.None);
            } else {
                string targetDir = inputDir;
                string file = GenericFileStringUtils.GetFileName(inputDir);
                if (file.Contains("*") || file.Contains("?")) {
                    targetDir = GenericFileStringUtils.GetDirectoryName(inputDir);
                } else if (file.Equals("<filter>")) {
                    targetDir = GenericFileStringUtils.GetDirectoryName(inputDir);
                    file = null;
                } else {
                    file = null;
                }
                command = new AddressBarChdirCommand();
                command.Initialize(fileListView, null, ActionCommandOption.None);
                command.SetParameter(targetDir, file);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：アドレスバーからディレクトリ変更コマンドを生成する
        // 引　数：[in]sender  イベント発生原因の送信元の種別
        // 　　　　[in]item    発生したイベントの項目
        // 　　　　[in]fileListView  対象のファイルリスト
        // 戻り値：コマンド（対応するコマンドがないときはnull）
        //=========================================================================================
        public FileListActionCommand CreateFromRetryInfo(FileErrorRetryInfo retryInfo) {
            FileListActionCommand command = null;
            if (retryInfo.TaskType == BackgroundTaskType.Copy) {
                command = new CopyRetryInternalCommand();
            } else if (retryInfo.TaskType == BackgroundTaskType.Move) {
                command = new MoveRetryInternalCommand();
            } else if (retryInfo.TaskType == BackgroundTaskType.Delete) {
                command = new DeleteRetryInternalCommand();
            } else if (retryInfo.TaskType == BackgroundTaskType.DeleteNoRecycle) {
                command = new DeleteRetryInternalCommand();
            } else if (retryInfo.TaskType == BackgroundTaskType.CreateShortcut) {
                command = new CreateShortcutRetryInternalCommand();
            } else {
                Program.Abort("不明な再試行タイプ{0}が使用されました。", retryInfo.TaskType.DisplayName);
            }
            command.Initialize(null, null, ActionCommandOption.None);
            command.SetParameter(retryInfo, true);
            return command;
        }
        
        //=========================================================================================
        // 機　能：キー入力からテキストビューアコマンドを生成する
        // 引　数：[in]evt           キーイベント
        // 　　　　[in]fileListView  対象のファイルリスト
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public FileViewerActionCommand CreateFileViewerCommandFromKeyInput(KeyCommand evt, TextFileViewer textFileView) {
            // 実行対象のキーを作成
            KeyState keyState;
            TwoStrokeType twoStroke = m_twoStrokeKeyState.GetTwoStrokeState(CommandUsingSceneType.FileViewer, textFileView);
            if (twoStroke == TwoStrokeType.None) {
                keyState = new KeyState(evt.KeyCode, evt.Shift, evt.Control, evt.Alt);
            } else {
                keyState = new KeyState(evt.KeyCode, twoStroke);
            }
            m_twoStrokeKeyState.ResetKeyState();

            // コマンドを実行
            KeyItemSetting setting = Program.Document.KeySetting.FileViewerKeyItemList.GetSettingFromKey(keyState);
            if (setting == null) {
                return null;
            }
            FileViewerActionCommand command = (FileViewerActionCommand)(setting.ActionCommandMoniker.CreateActionCommand());
            command.Initialize(textFileView, evt);
            return command;
        }

        //=========================================================================================
        // 機　能：マウス入力からテキストビューアコマンドを生成する
        // 引　数：[in]evt           マウスイベント
        // 　　　　[in]fileListView  対象のファイルリスト
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public FileViewerActionCommand CreateFileViewerCommandFromMouseInput(MouseEventArgs evt, TextFileViewer textFileView) {
            FileViewerActionCommand command = null;
            switch (evt.Button) {
                case MouseButtons.Left:
                    command = new V_ExitViewCommand();
                    break;
                case MouseButtons.Middle:
                    break;
                case MouseButtons.Right:
                    break;
            }
            if (command != null) {
                command.Initialize(textFileView, null);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：UIによるコマンド入力からコマンドを生成する
        // 引　数：[in]item    発生したイベントの項目
        // 　　　　[in]viewer  ファイルビューア
        // 戻り値：コマンド（対応するコマンドがないときはnull）
        //=========================================================================================
        public FileViewerActionCommand CreateFileViewerCommandFromUICommand(UICommandItem item, TextFileViewer viewer) {
            FileViewerActionCommand command = (FileViewerActionCommand)(item.ActionCommandMoniker.CreateActionCommand());
            if (command != null) {
                command.Initialize(viewer, null);
            }
            return command;
        }
        
        //=========================================================================================
        // 機　能：キー入力からグラフィックビューアコマンドを生成する
        // 引　数：[in]evt           キーイベント
        // 　　　　[in]fileListView  対象のファイルリスト
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public GraphicsViewerActionCommand CreateGraphicsViewerCommandFromKeyInput(KeyCommand evt, GraphicsViewerPanel viewer) {
            // 実行対象のキーを作成
            KeyState keyState;
            TwoStrokeType twoStroke = m_twoStrokeKeyState.GetTwoStrokeState(CommandUsingSceneType.GraphicsViewer, viewer);
            if (twoStroke == TwoStrokeType.None) {
                keyState = new KeyState(evt.KeyCode, evt.Shift, evt.Control, evt.Alt);
            } else {
                keyState = new KeyState(evt.KeyCode, twoStroke);
            }
            m_twoStrokeKeyState.ResetKeyState();

            // コマンドを実行
            KeyItemSetting setting = Program.Document.KeySetting.GraphicsViewerKeyItemList.GetSettingFromKey(keyState);
            if (setting == null) {
                return null;
            }
            GraphicsViewerActionCommand command = (GraphicsViewerActionCommand)(setting.ActionCommandMoniker.CreateActionCommand());
            command.Initialize(viewer, evt);
            return command;
        }

        //=========================================================================================
        // 機　能：マウス入力からグラフィックビューアコマンドを生成する
        // 引　数：[in]evt           マウスイベント
        // 　　　　[in]fileListView  対象のファイルリスト
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public GraphicsViewerActionCommand CreateGraphicsViewerCommandFromMouseInput(MouseEventArgs evt, GraphicsViewerPanel viewer) {
            GraphicsViewerActionCommand command = null;
            switch (evt.Button) {
                case MouseButtons.Left:
                    command = new G_ExitViewCommand();
                    break;
                case MouseButtons.Middle:
                    break;
                case MouseButtons.Right:
                    break;
            }
            if (command != null) {
                command.Initialize(viewer, null);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：UIによるコマンド入力からコマンドを生成する
        // 引　数：[in]item    発生したイベントの項目
        // 　　　　[in]viewer  グラフィックビューア
        // 戻り値：コマンド（対応するコマンドがないときはnull）
        //=========================================================================================
        public GraphicsViewerActionCommand CreateGraphicsViewerCommandFromUICommand(UICommandItem item, GraphicsViewerPanel viewer) {
            GraphicsViewerActionCommand command = (GraphicsViewerActionCommand)(item.ActionCommandMoniker.CreateActionCommand());
            if (command != null) {
                command.Initialize(viewer, null);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：キー入力からコマンドを生成する
        // 引　数：[in]evt         キーイベント
        // 　　　　[in]monitoring  対象のモニタリングビューア
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public MonitoringViewerActionCommand CreateFromKeyInput(KeyCommand evt, IMonitoringViewer monitoring) {
            // 実行対象のキーを作成
            KeyState keyState;
            TwoStrokeType twoStroke = m_twoStrokeKeyState.GetTwoStrokeState(CommandUsingSceneType.MonitoringViewer, monitoring.MonitoringViewerForm);
            if (twoStroke == TwoStrokeType.None) {
                keyState = new KeyState(evt.KeyCode, evt.Shift, evt.Control, evt.Alt);
            } else {
                keyState = new KeyState(evt.KeyCode, twoStroke);
            }
            m_twoStrokeKeyState.ResetKeyState();
            MonitoringViewerActionCommand command = CreateFromKeyInputDirect(keyState, monitoring);
            return command;
        }

        //=========================================================================================
        // 機　能：キー入力からコマンドを生成する
        // 引　数：[in]keyState    入力したキー
        // 　　　　[in]monitoring  対象のモニタリングビューア
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public MonitoringViewerActionCommand CreateFromKeyInputDirect(KeyState keyState, IMonitoringViewer monitoring) {
            // コマンドを取得
            KeyItemSetting setting = Program.Document.KeySetting.MonitoringViewerKeyItemList.GetSettingFromKey(keyState);
            if (setting == null) {
                return null;
            }

            ActionCommandMoniker moniker = setting.ActionCommandMoniker;
            MonitoringViewerActionCommand command = null;
            if (moniker != null) {
                command = (MonitoringViewerActionCommand)(moniker.CreateActionCommand());
                command.Initialize(monitoring);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：UIによるコマンド入力からコマンドを生成する
        // 引　数：[in]sender  イベント発生原因の送信元の種別
        // 　　　　[in]item    発生したイベントの項目
        // 　　　　[in]monitoring  対象のモニタリングビューア
        // 戻り値：コマンド（対応するコマンドがないときはnull）
        //=========================================================================================
        public MonitoringViewerActionCommand CreateMonitoringViewerCommandFromUICommand(UICommandSender sender, UICommandItem item, IMonitoringViewer monitoring) {
            MonitoringViewerActionCommand command = (MonitoringViewerActionCommand)(item.ActionCommandMoniker.CreateActionCommand());
            if (command != null) {
                command.Initialize(monitoring);
            }
            return command;
        }
        
        //=========================================================================================
        // 機　能：キー入力からコマンドを生成する
        // 引　数：[in]evt    キーイベント
        // 　　　　[in]panel  対象のパネル
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public TerminalActionCommand CreateTerminalCommandFromKeyInput(KeyCommand evt, TerminalPanel panel) {
            // 実行対象のキーを作成
            KeyState keyState;
            TwoStrokeType twoStroke = m_twoStrokeKeyState.GetTwoStrokeState(CommandUsingSceneType.Terminal, panel.TwoStrokeKeyForm);
            if (twoStroke == TwoStrokeType.None) {
                keyState = new KeyState(evt.KeyCode, evt.Shift, evt.Control, evt.Alt);
            } else {
                keyState = new KeyState(evt.KeyCode, twoStroke);
            }
            m_twoStrokeKeyState.ResetKeyState();
            TerminalActionCommand command = CreateFromKeyInputDirect(keyState, panel);
            return command;
        }

        //=========================================================================================
        // 機　能：キー入力からコマンドを生成する
        // 引　数：[in]keyState    入力したキー
        // 　　　　[in]panel  対象のパネル
        // 戻り値：コマンド（対応するキーがないときはnull）
        //=========================================================================================
        public TerminalActionCommand CreateFromKeyInputDirect(KeyState keyState, TerminalPanel panel) {
            // コマンドを取得
            KeyItemSetting setting = Program.Document.KeySetting.TerminalKeyItemList.GetSettingFromKey(keyState);
            if (setting == null) {
                return null;
            }

            ActionCommandMoniker moniker = setting.ActionCommandMoniker;
            TerminalActionCommand command = null;
            if (moniker != null) {
                command = (TerminalActionCommand)(moniker.CreateActionCommand());
                command.Initialize(panel);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：UIによるコマンド入力からコマンドを生成する
        // 引　数：[in]item    発生したイベントの項目
        // 　　　　[in]panel   対象のパネル
        // 戻り値：コマンド（対応するコマンドがないときはnull）
        //=========================================================================================
        public TerminalActionCommand CreateTerminalCommandFromUICommand(UICommandItem item, TerminalPanel panel) {
            TerminalActionCommand command = (TerminalActionCommand)(item.ActionCommandMoniker.CreateActionCommand());
            if (command != null) {
                command.Initialize(panel);
            }
            return command;
        }

        //=========================================================================================
        // 機　能：2ストロークキーの動作を開始する
        // 引　数：[in]scene     入力シーン
        // 　　　　[in]target    入力中のウィンドウ
        // 　　　　[in]keyType   2ストロークキーの種類
        // 戻り値：なし
        //=========================================================================================
        public void StartTwoStrokeKey(CommandUsingSceneType scene, ITwoStrokeKeyForm target, TwoStrokeType keyType) {
            m_twoStrokeKeyState.SetKeyState(scene, target, keyType);
        }
    }
}
