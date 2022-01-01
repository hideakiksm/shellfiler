using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Properties;
using ShellFiler.Archive;
using ShellFiler.Command;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.FileOperationEtc;
using ShellFiler.Command.FileList.Open;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.MoveCursor;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.Command.FileList.SSH;
using ShellFiler.Command.FileList.Window;
using ShellFiler.Command.FileList.Tools;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.Command.GraphicsViewer;
using ShellFiler.Command.GraphicsViewer.Edit;
using ShellFiler.Command.GraphicsViewer.File;
using ShellFiler.Command.GraphicsViewer.View;
using ShellFiler.Command.GraphicsViewer.Filter;
using ShellFiler.Command.MonitoringViewer;
using ShellFiler.Command.MonitoringViewer.File;
using ShellFiler.Command.MonitoringViewer.Edit;
using ShellFiler.Command.MonitoringViewer.ExecutePs;
using ShellFiler.Command.Terminal;
using ShellFiler.Command.Terminal.Console;
using ShellFiler.Command.Terminal.Edit;
using ShellFiler.Command.Terminal.File;
using ShellFiler.Command.Terminal.View;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：キーのカスタマイズ情報
    //=========================================================================================
    public class KeySetting : ICloneable {
        // ファイル一覧のキー定義
        private KeyItemSettingList m_fileListKeyItemList = new KeyItemSettingList();

        // ファイルビューアのキー定義
        private KeyItemSettingList m_fileViewerKeyItemList = new KeyItemSettingList();
        
        // グラフィックビューアのキー定義
        private KeyItemSettingList m_graphicsViewerKeyItemList = new KeyItemSettingList();
        
        // ターミナルのキー定義
        private KeyItemSettingList m_terminalKeyItemList = new KeyItemSettingList();
        
        // モニタリングビューアのキー定義
        private KeyItemSettingList m_monitoringViewerKeyItemList = new KeyItemSettingList();
        
        // 関連付け設定
        private AssociateSetting m_associateSetting = new AssociateSetting();

        // ファイルの書き込み日時
        private DateTime m_lastFileWriteTime = DateTime.MinValue;

        // 設定を読み込んだバージョン
        private int m_loadedFileVersion;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public KeySetting() {
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]warningList  読み込み時に発生した警告を集約する先の警告情報（集約しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(SettingWarningList warningList) {
            LoadSetting(warningList);
        }

        //=========================================================================================
        // 機　能：デフォルトを設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetDefault() {
            InitializeFileListKeySetting();
            InitializeFileViewerKeySetting();
            InitializeGraphicsViewerKeySetting();
            InitializeMonitoringViewerKeySetting();
            InitializeTerminalKeySetting();
            InitializeAssociateAll();
        }

        //=========================================================================================
        // 機　能：ファイル一覧のキー設定を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void InitializeFileListKeySetting() {
            InitializeFileListKeySettingInternal(m_fileListKeyItemList);
        }

        //=========================================================================================
        // 機　能：ファイル一覧のキー設定を初期化する（実際の処理）
        // 引　数：[in]keyItemList  初期化するキー一覧
        // 戻り値：なし
        //=========================================================================================
        private void InitializeFileListKeySettingInternal(KeyItemSettingList keyItemList) {
            keyItemList.ClearAll();
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorUpCommand), 1),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorUpMarkCommand), 1),                  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,        false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(SetFileListBorderRatioCommand), 50),       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,        TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorTopCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorDownCommand), 1),                    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,      true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorDownMarkCommand), 1),                null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,      false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(SetFocusStatePanelCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,      TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorBottomCommand)),                     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Left,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorLeftCommand)),                       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Left,      true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorLeftWindowCommand)),                 null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Left,      false, false, true),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(PathHistoryPrevCommand)),                  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Left,      false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MoveFileListBorderCommand), -16),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Left,      true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MoveFileListBorderCommand), -99999),       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Left,      true,  false, true),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectLeftCommand)),                    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Right,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorRightCommand)),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Right,     true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorRightWindowCommand)),                null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Right,     false, false, true),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(PathHistoryNextCommand)),                  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Right,     false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MoveFileListBorderCommand), 16),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Right,     true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MoveFileListBorderCommand), 99999),        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Right,     true,  false, true),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectRightCommand)),                   null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Prior,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorRollupCommand)),                     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Prior,     true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorRollupMarkCommand)),                 null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Next,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorRolldownCommand)),                   null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Next,      true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CursorRolldownMarkCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Escape,    false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TaskManagerCommand)),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Space,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MarkCommand)),                             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Delete,    false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(DeleteCommand)),                           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Delete,    true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(HighspeedDeleteCommand)),                  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Delete,    true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(DeleteExCommand)),                         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Home,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ReverseAllMarkFileCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Home,      true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ClearAllMarkCommand)),                     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Home,      false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ReverseAllMarkCommand)),                   null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.End,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(RefreshFileListCommand)),                  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Enter,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(OpenFileAssociate1Command)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Enter,     true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(OpenFileAssociate2Command)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Enter,     false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(OpenFileAssociate3Command)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Back,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirParentCommand)),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Back,      false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(BothChdirParentCommand)),                  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Tab,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ContextMenuCommand)),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Tab,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ContextMenuFolderCommand)),                null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemMinus,  false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ToggleLogSizeCommand)),                    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemMinus,  true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ClearLogCommand)),                         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Oemplus,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(FileListViewModeSmallerCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Oemplus,   true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(FileListViewModeDetailCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Oemplus,   false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(FileListViewModeSettingCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Oem1,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(FileListViewModeLargerCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Oem1,      true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(FileListViewModeThumbnailCommand)),        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Oem1,      false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(FileListViewModeSettingCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D1,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectDirectCommand), 0),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D2,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectDirectCommand), 1),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D3,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectDirectCommand), 2),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D4,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectDirectCommand), 3),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D5,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectDirectCommand), 4),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D6,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectDirectCommand), 5),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D7,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectDirectCommand), 6),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D8,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectDirectCommand), 7),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D9,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectDirectCommand), 8),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D0,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabSelectDirectCommand), 9),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(KeyListMenuCommand)),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(OptionSettingCommand)),                    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F2,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(RenameCommand)),                           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F3,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirSpecialCommand), "MyDocuments"),      "Documents"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirSpecialCommand), "DesktopDirectory"), "Desktop"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,        false, false, true ), new ActionCommandMoniker(ActionCommandOption.None,     typeof(QuickExitCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(RefreshFileListCommand)),                  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirSpecialCommand), "Download"),         "Downloads"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F6,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(SortMenuCommand)),                         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F6,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(SortClearCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F7,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(FileListFilterMenuCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F7,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ClearFileListFilterCommand)),              null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F10,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ShellExecuteMenuCommand)),                 null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F10,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ShellCommandCommand)),                     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F10,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(PowerShellCommand)),                       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.A,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(RenameSelectedFileInfoCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.A,         false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(SelectAllMarkCommand)),                    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.A,         true,  true, false),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(GitAddCommand)), null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.A,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "A"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.B,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MirrorCopyCommand)),                       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.B,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "B"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.C,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CopyCommand)),                             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.C,         false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ClipboardCopyCommand)),                    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.C,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CopyExCommand)),                           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.C,         true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ClipboardNameCopyCommand)),                null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.C,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "C"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(DeleteCommand)),                           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(HighspeedDeleteCommand)),                  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D,         true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(DeleteExCommand)),                         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "D"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.E,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(LocalEditCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.E,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(LocalExecuteCommand), "notepad.exe", "$P", "cursor", "none"), "メモ帳で編集"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchCommand)),                null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F,         false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchCommand)),                null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.E,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "E"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "F"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.G,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(SlideShowCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.G,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "G"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.H,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(KeyBindHelpCommand)),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.H,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "H"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.I,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(RetrieveFolderSizeCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.I,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ClearFolderSizeCommand)),                  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.I,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "I"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.J,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirBookmarkFolderCommand)),              null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.J,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirSSHFolderCommand)),                   null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.J,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "J"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.K,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MakeFolderCommand)),                       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.K,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "K"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.L,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirDriveCommand)),                       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.L,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirInputCommand)),                       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.L,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "L"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.M,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MoveCommand)),                             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.M,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MoveExCommand)),                           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.M,         true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(GitMoveCommand)),                          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.M,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "M"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.N,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "N"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.O,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirTargetToOppositeCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.O,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirOppositeToTargetCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.O,         false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirSwapTargetOppositeCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.O,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "O"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.P,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ArchiveCommand)),                          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.P,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "P"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Q,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ExitCommand)),                             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Q,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CloseAllViewerCommand)),                   null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Q,         true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(QuickExitCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Q,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "Q"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.R,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(RenameCommand)),                           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.R,         true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(GitRenameCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.R,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "R"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.S,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(SortMenuCommand)),                         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.S,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(SortClearCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.S,         false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TwoStrokeKey1Command)),                    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.S,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "S"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.T,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(CreateShortcutCommand)),                   null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.T,         false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabCreateCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.T,         true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabReopenCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.T,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "T"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.U,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirVirtualFolderCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.U,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ExtractCommand)),                          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.U,         false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(PrepareVirtualFolderCommand)),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.U,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "U"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.V,         false, false, false), new ActionCommandMoniker(ActionCommandOption.MoveNext, typeof(FileViewerCommand)),                       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.V,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "V"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.W,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(FileCompareCommand)),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.W,         false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(TabDeleteCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.W,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "W"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.X,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ExecuteMenuCommand)),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.X,         false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ClipboardCutCommand)),                     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.X,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "X"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Y,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(DuplicateCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Y,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "Y"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Z,         false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ShowPropertyCommand)),                     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Z,         true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ShowPropertyFolderCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Z,         TwoStrokeType.Key1),  new ActionCommandMoniker(ActionCommandOption.None,     typeof(IncrementalSearchExCommand), "Z"),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Oemtilde,  false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(DiffOppositeCommand)), null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemPeriod, false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(FileListFilterMenuCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemPeriod, true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ClearFileListFilterCommand)),              null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemQuestion,false,false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MarkWithConditionsCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemPipe,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirRootCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Oem7,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirHomeCommand)),                        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Oem4,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirFolderHistoryCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.NumPad0,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirSpecialCommand), "DesktopDirectory"), "Desktop"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.NumPad1,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirCommand), "A:"),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.NumPad2,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirCommand), "B:"),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.NumPad3,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirCommand), "C:"),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.NumPad4,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirCommand), "D:"),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.NumPad5,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirCommand), "E:"),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.NumPad6,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirCommand), "F:"),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.NumPad7,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirCommand), "G:"),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.NumPad8,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirCommand), "H:"),                      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.NumPad9,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(ChdirCommand), "I:"),                      null));

            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.LButton,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MouseDragSelectCommand)),                  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.LButton,   true,  false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MouseExplorerSelectCommand)),              null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.LButton,   false, true,  false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MouseDragMarkCommand)),                    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.MButton,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MouseDragMarkCommand)),                    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.RButton,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None,     typeof(MouseContextMenuCommand)),                 null));
        }

        //=========================================================================================
        // 機　能：ファイルビューアのキー設定を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void InitializeFileViewerKeySetting() {
            InitializeFileViewerKeySettingInternal(m_fileViewerKeyItemList);
        }

        //=========================================================================================
        // 機　能：ファイルビューアのキー設定を初期化する（実際の処理）
        // 引　数：[in]keyItemList  初期化するキー一覧
        // 戻り値：なし
        //=========================================================================================
        private void InitializeFileViewerKeySettingInternal(KeyItemSettingList keyItemList) {
            keyItemList.ClearAll();
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CursorUpCommand), 1),        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CursorUpCommand), 4),        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CursorDownCommand), 1),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,     true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CursorDownCommand), 4),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Prior,    false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CursorRollupCommand)),       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Next,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CursorRolldownCommand)),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Escape,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ExitViewCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Enter,    false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ExitViewCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpTopLineCommand)),        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "SJIS"),    "SJIS"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F2,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "UTF-8"),   "UTF-8"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F3,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), -1),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F3,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "EUC-JP"),  "EUC-JP"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "JIS"),     "JIS"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "UNICODE"), "UNICODE"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F2,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpBottomLineCommand)),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchCommand)),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchForwardNextCommand)),  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchReverseNextCommand)),  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F6,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ChangeTextModeCommand)),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F7,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ChangeDumpModeCommand)),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F11,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_FullScreenCommand)),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F12,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ReturnFileListCommand)),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.A,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_FullScreenCommand)),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.A,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_FullScreenMultiCommand)),    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.A,        false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectAllCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.C,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_RotateTextModeCommand)),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.C,        false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CopyTextCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.C,        true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CopyTextAsCommand)),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.E,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_EditFileCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchCommand)),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F,        false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchCommand)),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.J,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), -1),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.S,        false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SaveTextAsCommand)),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.W,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetLineWidthCommand)),       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemMinus, false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ToggleViewModeCommand)),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemQuotes,false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTabCommand), 4),          "TAB4"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemPipe,  false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTabCommand), 8),          "TAB8"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D0,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), 0),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D1,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), 1),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D2,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), 2),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D3,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), 3),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D4,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), 4),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D5,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), 5),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D6,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), 6),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D7,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), 7),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D8,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), 8),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D9,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), 9),      null));
        }

        //=========================================================================================
        // 機　能：グラフィックビューアのキー設定を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void InitializeGraphicsViewerKeySetting() {
            InitializeGraphicsViewerKeySettingInternal(m_graphicsViewerKeyItemList);
        }

        //=========================================================================================
        // 機　能：グラフィックビューアのキー設定を初期化する（実際の処理）
        // 引　数：[in]keyItemList  初期化するキー一覧
        // 戻り値：なし
        //=========================================================================================
        private void InitializeGraphicsViewerKeySettingInternal(KeyItemSettingList keyItemList) {
            keyItemList.ClearAll();
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ScrollUpCommand), 32),       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ScrollUpCommand), 1),        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ViewTopCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ScrollDownCommand), 32),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,     true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ScrollDownCommand), 1),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,     false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ViewBottomCommand)),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Left,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ScrollLeftCommand), 32),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Left,     true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ScrollLeftCommand), 1),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Left,     false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ViewLeftCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Right,    false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ScrollRightCommand), 32),    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Right,    true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ScrollRightCommand), 1),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Right,    false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ViewRightCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Next,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomOutCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Next,     true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomOutExCommand), 1),       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Prior,    false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomInCommand)),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Prior,    true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomInExCommand), 1),        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Escape,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ExitViewCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Enter,    false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ExitViewOrNextSlideCommand)),null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Enter,    true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ExitViewOrPrevSlideCommand)),null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Home,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ViewCenterCommand)),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.A,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FullScreenCommand)),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.A,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FullScreenMultiCommand)),    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.B,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ToggleBackGroundColorCommand)), null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.B,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SetBackGroundWhiteCommand)), null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.B,        true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SetBackGroundBlackCommand)), null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.C,        false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_CopyImageCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterOnOffCommand)),        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterSettingCommand)),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.G,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SlideShowNextCommand), true),null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.G,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SlideShowPrevCommand), true),null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.H,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MirrorHorizontalCommand)),   null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.L,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_RotateCCWCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.R,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_RotateCWCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.S,        false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SaveImageAsCommand)),        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.V,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MirrorVerticalCommand)),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.W,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FitImageToScreenCommand)),   null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.W,        true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FitShortEdgeToScreenCommand)), null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.W,        false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect100Command)), null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemQuotes,false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_InterpolationHighQualityCommand)), null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.OemPipe,  false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_InterpolationNeighborCommand)), null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D1,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile1Command)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D2,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile2Command)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D3,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile3Command)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D4,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile4Command)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D5,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile5Command)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D6,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile6Command)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D7,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile7Command)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D8,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile8Command)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D9,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile9Command)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.D0,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ClearMarkCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ViewTopCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), 10, 0, 0),    "明るさ↑"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F2,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ViewBottomCommand)),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F2,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), -10, 0, 0),    "明るさ↓"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F3,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ViewLeftCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F3,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), 0, 10, 0),    "コントラスト↑"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ViewRightCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), 0, -10, 0),    "コントラスト↓"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect20Command)),       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), 0, 0, 10),    "ガンマ↑"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F6,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect50Command)),       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F6,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), 0, 0, -10),    "ガンマ↓"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F7,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect100Command)),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F7,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterHsvModifyCommand), 0, 10, 0), "彩度↑"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F8,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect150Command)),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F8,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterHsvModifyCommand), 0, -10, 0), "彩度↓"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F9,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect200Command)),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F9,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterHsvModifyCommand), 0, 0, 10), "明度↑"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F10,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect500Command)),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F10,      true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterHsvModifyCommand), 0, 0, -10), "明度↓"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F12,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ReturnFileListCommand)),     null));
        }

        //=========================================================================================
        // 機　能：モニタリングビューアのキー設定を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void InitializeMonitoringViewerKeySetting() {
            InitializeMonitoringViewerKeySettingInternal(m_monitoringViewerKeyItemList);
        }

        //=========================================================================================
        // 機　能：モニタリングビューアのキー設定を初期化する（実際の処理）
        // 引　数：[in]keyItemList  初期化するキー一覧
        // 戻り値：なし
        //=========================================================================================
        private void InitializeMonitoringViewerKeySettingInternal(KeyItemSettingList keyItemList) {
            keyItemList.ClearAll();
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Escape,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_ExitViewCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Delete,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_PsKillCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Delete,   true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_PsForceTerminateCommand)),  null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Enter,    false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_PsDetailCommand)),          null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F,        false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SearchCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F,        false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SearchCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.S,        false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SaveAsCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_JumpTopLineCommand)),       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F2,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_JumpBottomLineCommand)),    null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SearchCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_RefreshCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F12,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(M_ReturnFileListCommand)),    null));
        }

        //=========================================================================================
        // 機　能：ターミナルのキー設定を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void InitializeTerminalKeySetting() {
            InitializeTerminalKeySettingInternal(m_terminalKeyItemList);
        }

        //=========================================================================================
        // 機　能：ターミナルのキー設定を初期化する（実際の処理）
        // 引　数：[in]keyItemList  初期化するキー一覧
        // 戻り値：なし
        //=========================================================================================
        private void InitializeTerminalKeySettingInternal(KeyItemSettingList keyItemList) {
            keyItemList.ClearAll();
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendCursorUpCommand)),              null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,       false, false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollUpCommand), 1),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Up,       true,  false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollUpCurrentCommand), 1),        null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendCursorDownCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,     false, false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollDownCommand), 1),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Down,     true,  false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollDownCurrentCommand), 1),      null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Left,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendCursorLeftCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Right,    false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendCursorRightCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Prior,    false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyPageUpCommand)),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Prior,    false, false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollRollUpCommand)),              null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Prior,    true,  false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollRollUpCurrentCommand)),       null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Next,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyPageDownCommand)),           null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Next,     false, false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollRollDownCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Next,     true,  false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollRollDownCurrentCommand)),     null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Delete,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyDeleteCommand)),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Insert,   false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyInsertCommand)),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Home,     false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyHomeCommand)),               null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.Home,     false, false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ToggleBackLogViewCommand)),         null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.End,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyEndCommand)),                null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.A,        false, false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SelectAllCommand)),                 null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.C,        false, false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_CopyClipboardCommand)),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.V,        false, false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_PasteClipboardCommand)),            null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  1, false), "F1"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  1, true),  "Ctrl+F1"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 11, false), "F11"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F1,       true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 11, true),  "Ctrl+F11"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F2,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  2, false), "F2"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F2,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  2, true),  "Ctrl+F2"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F2,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 12, false), "F12"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F2,       true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 12, true),  "Ctrl+F12"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F3,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  3, false), "F3"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F3,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  3, true),  "Ctrl+F3"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F3,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 13, false), "F13"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F3,       true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 13, true),  "Ctrl+F13"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  4, false), "F4"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  4, true),  "Ctrl+F4"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 14, false), "F14"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F4,       true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 14, true),  "Ctrl+F14"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  5, false), "F5"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  5, true),  "Ctrl+F5"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 15, false), "F15"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F5,       true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 15, true),  "Ctrl+F15"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F6,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  6, false), "F6"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F6,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  6, true),  "Ctrl+F6"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F6,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 16, false), "F16"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F6,       true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 16, true),  "Ctrl+F16"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F7,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  7, false), "F7"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F7,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  7, true),  "Ctrl+F7"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F7,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 17, false), "F17"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F7,       true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 17, true),  "Ctrl+F17"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F8,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  8, false), "F8"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F8,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  8, true),  "Ctrl+F8"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F8,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 18, false), "F18"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F8,       true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 18, true),  "Ctrl+F18"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F9,       false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  9, false), "F9"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F9,       false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand),  9, true),  "Ctrl+F9"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F9,       true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 19, false), "F19"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F9,       true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 19, true),  "Ctrl+F19"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F10,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 10, false), "F10"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F10,      false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 10, true),  "Ctrl+F10"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F10,      true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 20, false), "F20"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F10,      true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 20, true),  "Ctrl+F20"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F11,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 11, false), "F11"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F11,      false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 11, true),  "Ctrl+F11"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F11,      true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 21, false), "F21"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F11,      true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 21, true),  "Ctrl+F21"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F12,      false, false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ReturnFileListCommand)),             null));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F12,      false, true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 12, true),  "Ctrl+F12"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F12,      true,  false, false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 22, false), "F22"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F12,      true,  true,  false), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 22, true),  "Ctrl+F22"));
            keyItemList.AddSetting(new KeyItemSetting(new KeyState(Keys.F12,      false, false, true ), new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SendKeyFunctionCommand), 12, false), "F12"));
        }

        //=========================================================================================
        // 機　能：関連付けすべてにデフォルト値をセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void InitializeAssociateAll() {
            for (int i = 0; i < m_associateSetting.AssocSettingList.Count; i++) {
                InitializeAssociateDefault(m_associateSetting.AssocSettingList[i], i, true);
            }
        }

        //=========================================================================================
        // 機　能：関連付けにデフォルト値をセットする
        // 引　数：[in]assocItem   対象とするオブジェクト
        // 　　　　[in]index       設定する関連付け設定のインデックス
        // 　　　　[in]itemDefault 項目の設定もデフォルト設定するときtrue
        // 戻り値：なし
        //=========================================================================================
        public static void InitializeAssociateDefault(AssociateKeySetting assocItem, int index, bool resetItem) {
            assocItem.ClearSetting();
            switch (index) {
                case 0:
                    assocItem.DislayName = Resources.MenuName_OpenFileAssociate1;
                    if (resetItem) {
                        assocItem.AddAssociate(FileSystemID.None, null, new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCursorFolderCommand)));
                        assocItem.AddAssociate(FileSystemID.None, new string[] {".com", ".exe"}, new ActionCommandMoniker(ActionCommandOption.None, typeof(ExecuteOrViewerCommand)));
                        assocItem.AddAssociate(FileSystemID.None, new string[] {".bmp", ".jpg", ".jpeg", ".gif", ".png"}, new ActionCommandMoniker(ActionCommandOption.MoveNext, typeof(GraphicsViewerCommand)));
                        assocItem.AddAssociate(FileSystemID.None, SevenZipUtils.SupportedExtList, new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCursorFolderCommand)));
                        assocItem.SetDefaultCommand(new ActionCommandMoniker(ActionCommandOption.MoveNext, typeof(FileViewerCommand)));
                    }
                    break;
                case 1:
                    assocItem.DislayName = Resources.MenuName_OpenFileAssociate2;
                    if (resetItem) {
                        assocItem.AddAssociate(FileSystemID.None, null, new ActionCommandMoniker(ActionCommandOption.None, typeof(OpenExplorerFolderCommand)));
                        assocItem.AddAssociate(FileSystemID.None, SevenZipUtils.SupportedExtList, new ActionCommandMoniker(ActionCommandOption.MoveNext, typeof(ExtractCommand)));
                        assocItem.SetDefaultCommand(new ActionCommandMoniker(ActionCommandOption.MoveNext, typeof(ExecuteOrViewerCommand)));
                    }
                    break;
                case 2:
                    assocItem.DislayName = Resources.MenuName_OpenFileAssociate3;
                    if (resetItem) {
                        assocItem.AddAssociate(FileSystemID.None, null, new ActionCommandMoniker(ActionCommandOption.None, typeof(BothChdirCursorFolderCommand)));
                    }
                    break;
                case 3:
                    assocItem.DislayName = Resources.MenuName_OpenFileAssociate4;
                    break;
                case 4:
                    assocItem.DislayName = Resources.MenuName_OpenFileAssociate5;
                    break;
                case 5:
                    assocItem.DislayName = Resources.MenuName_OpenFileAssociate6;
                    break;
                case 6:
                    assocItem.DislayName = Resources.MenuName_OpenFileAssociate7;
                    break;
                case 7:
                    assocItem.DislayName = Resources.MenuName_OpenFileAssociate8;
                    break;
            }
            assocItem.SortSetting();
        }

        //=========================================================================================
        // 機　能：設定の読み込みを行う
        // 引　数：[in]warningList  読み込み時に発生した警告を集約する先の警告情報（集約しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void LoadSetting(SettingWarningList warningList) {
#if !FREE_VERSION
            m_lastFileWriteTime = DateTime.MinValue;

            string fileName = DirectoryManager.KeySetting;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = LoadSettingInternal(loader, this);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSetting, fileName, loader.ErrorDetail);
            } else {
                m_loadedFileVersion = loader.FileVersion;
                m_lastFileWriteTime = loader.LastFileWriteTime;

                // 警告を処理
                if (loader.WarningList.Count > 0) {
                    SettingWarningList.AddWarningInfo(loader, warningList);
                }
            }
#else
            InitializeFileListKeySetting();
            InitializeFileViewerKeySetting();
            InitializeGraphicsViewerKeySetting();
            InitializeAssociateAll();
#endif
        }
        
        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSettingInternal(SettingLoader loader, KeySetting obj) {
            // ファイルがないときはそのまま
            if (!File.Exists(loader.FileName)) {
                obj.InitializeFileListKeySetting();
                obj.InitializeFileViewerKeySetting();
                obj.InitializeGraphicsViewerKeySetting();
                obj.InitializeMonitoringViewerKeySetting();
                obj.InitializeTerminalKeySetting();
                obj.InitializeAssociateAll();
                return true;
            }

            // ファイルから読み込む
            bool success;
            success = loader.LoadSetting(false);
            if (!success) {
                obj.InitializeFileListKeySetting();
                obj.InitializeFileViewerKeySetting();
                obj.InitializeGraphicsViewerKeySetting();
                obj.InitializeMonitoringViewerKeySetting();
                obj.InitializeTerminalKeySetting();
                obj.InitializeAssociateAll();
                return false;
            }

            bool initFileList, initFileViewer, initGraphicsViewer, initMonitoringViewer, initTerminal, initAssociate;
            success = LoadSettingKeyList(loader, obj, out initFileList, out initFileViewer, out initGraphicsViewer, out initMonitoringViewer, out initTerminal, out initAssociate);
            if (!initFileList) {
                obj.InitializeFileListKeySetting();
                loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyFileList);
                loader.SetWarning(Resources.SettingLoader_LoadKeySettingParseFailed);
            }
            if (!initFileViewer) {
                obj.InitializeFileListKeySetting();
                loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyFileViewer);
                loader.SetWarning(Resources.SettingLoader_LoadKeySettingParseFailed);
            }
            if (!initGraphicsViewer) {
                obj.InitializeGraphicsViewerKeySetting();
                loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyGraphicsViewer);
                loader.SetWarning(Resources.SettingLoader_LoadKeySettingParseFailed);
            }
            if (!initMonitoringViewer) {
                obj.InitializeMonitoringViewerKeySetting();
                if (loader.FileVersion < Ver.V1_3_0) {
                    loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyMonitoringViewer);
                    loader.SetWarning(Resources.SettingLoader_LoadKeySettingParseFailed);
                }
            }
            if (!initTerminal) {
                obj.InitializeTerminalKeySetting();
                if (loader.FileVersion < Ver.V1_3_0) {
                    loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyMonitoringViewer);
                    loader.SetWarning(Resources.SettingLoader_LoadKeySettingParseFailed);
                }
            }
            if (!initAssociate) {
                // 保存済みバージョンで未対応なら警告なしでデフォルトを設定するのみ
                obj.InitializeAssociateAll();
                if (loader.FileVersion >= MonikerVersionConverter.VERSION_ASSOC_SETTING) {
                    loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyAssociate);
                    loader.SetWarning(Resources.SettingLoader_LoadKeySettingParseFailed);
                }
            }

            return true;
        }

        //=========================================================================================
        // 機　能：キー設定をファイルから読み込む
        // 引　数：[in]loader                読み込み用クラス
        // 　　　　[in]obj                   読み込み対象のオブジェクト
        // 　　　　[out]initFileList         ファイル一覧を初期化できたときtrueを返す変数
        // 　　　　[out]initFileViewer       ファイルビューアを初期化できたときtrueを返す変数
        // 　　　　[out]initGraphicsViewer   グラフィックビューアを初期化できたときtrueを返す変数
        // 　　　　[out]initMonitoringViewer モニタリングビューアを初期化できたときtrueを返す変数
        // 　　　　[out]initTerminal         ターミナルを初期化できたときtrueを返す変数
        // 　　　　[out]initAssociate        関連づけを初期化できたときtrueを返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSettingKeyList(SettingLoader loader, KeySetting obj, out bool initFileList, out bool initFileViewer, out bool initGraphicsViewer, out bool initMonitoringViewer, out bool initTerminal, out bool initAssociate) {
            bool success;
            initFileList = false;
            initFileViewer = false;
            initGraphicsViewer = false;
            initMonitoringViewer = false;
            initTerminal = false;
            initAssociate = false;
            
            success = loader.ExpectTag(SettingTag.KeySetting_KeySetting, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.KeySetting_KeySetting) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_FileList) {
                    loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyFileList);
                    obj.m_fileListKeyItemList.ClearAll();
                    success = KeyItemSettingList.LoadSetting(loader, obj.m_fileListKeyItemList);
                    if (!success) {
                        return false;
                    }
                    initFileList = true;
                    success = loader.ExpectTag(SettingTag.KeySetting_FileList, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_FileViewer) {
                    loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyFileViewer);
                    obj.m_fileViewerKeyItemList.ClearAll();
                    success = KeyItemSettingList.LoadSetting(loader, obj.m_fileViewerKeyItemList);
                    if (!success) {
                        return false;
                    }
                    initFileViewer = true;
                    success = loader.ExpectTag(SettingTag.KeySetting_FileViewer, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_GraphicsViewer) {
                    loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyGraphicsViewer);
                    obj.m_graphicsViewerKeyItemList.ClearAll();
                    success = KeyItemSettingList.LoadSetting(loader, obj.m_graphicsViewerKeyItemList);
                    if (!success) {
                        return false;
                    }
                    initGraphicsViewer = true;
                    success = loader.ExpectTag(SettingTag.KeySetting_GraphicsViewer, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
/*              } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_MonitoringViewer) {
                    loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyMonitoringViewer);
                    obj.m_monitoringViewerKeyItemList.ClearAll();
                    success = KeyItemSettingList.LoadSetting(loader, obj.m_monitoringViewerKeyItemList);
                    if (!success) {
                        return false;
                    }
                    initMonitoringViewer = true;
                    success = loader.ExpectTag(SettingTag.KeySetting_MonitoringViewer, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
*/
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.KeySetting_AssociateSetting) {
                    loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.KeyAssociate);
                    obj.m_associateSetting.ClearAll();
                    success = AssociateSetting.LoadSetting(loader, obj.m_associateSetting);
                    if (!success) {
                        return false;
                    }
                    initAssociate = true;
                    success = loader.ExpectTag(SettingTag.KeySetting_AssociateSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }

            return true;
        }

        //=========================================================================================
        // 機　能：設定の保存を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SaveSetting() {
#if !FREE_VERSION
            string fileName = DirectoryManager.KeySetting;
            SettingSaver saver = new SettingSaver(fileName);
            bool success = SaveSettingInternal(saver, this);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotSaveSetting, fileName);
                return;
            }
            try {
                m_lastFileWriteTime = File.GetLastWriteTime(fileName);
            } catch (Exception) {
                m_lastFileWriteTime = DateTime.MinValue;
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private static bool SaveSettingInternal(SettingSaver saver, KeySetting obj) {
            bool success;

            saver.StartObject(SettingTag.KeySetting_KeySetting);

            // ファイル一覧
            saver.StartObject(SettingTag.KeySetting_FileList);
            success = KeyItemSettingList.SaveSetting(saver, obj.m_fileListKeyItemList);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.KeySetting_FileList);

            // ファイルビューア
            saver.StartObject(SettingTag.KeySetting_FileViewer);
            success = KeyItemSettingList.SaveSetting(saver, obj.m_fileViewerKeyItemList);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.KeySetting_FileViewer);

            // グラフィックビューア
            saver.StartObject(SettingTag.KeySetting_GraphicsViewer);
            success = KeyItemSettingList.SaveSetting(saver, obj.m_graphicsViewerKeyItemList);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.KeySetting_GraphicsViewer);

/*          // モニタリングビューア
            saver.StartObject(SettingTag.KeySetting_MonitoringViewer);
            success = KeyItemSettingList.SaveSetting(saver, obj.m_monitoringViewerKeyItemList);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.KeySetting_MonitoringViewer);
*/
            // 関連付け
            saver.StartObject(SettingTag.KeySetting_AssociateSetting);
            success = AssociateSetting.SaveSetting(saver, obj.m_associateSetting);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.KeySetting_AssociateSetting);

            saver.EndObject(SettingTag.KeySetting_KeySetting);

            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // 機　能：バージョンアップ時のキー設定のマージ状況をチェックしてレポートする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CheckKeySettingMerge() {
            // 成功時はバージョンアップによるコマンドの差分をチェック
            FileVersionInfo ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            int currentVersion = ver.FileMajorPart * 1000000 + ver.FileMinorPart * 10000 + ver.FileBuildPart * 100 + ver.FilePrivatePart;
            if (m_loadedFileVersion != -1 && m_loadedFileVersion < currentVersion) {
                // 差分チェック
                KeySettingMergeDiff diff = new KeySettingMergeDiff(m_loadedFileVersion);
                KeySetting defaultSetting = new KeySetting();
                defaultSetting.SetDefault();
                diff.CheckDifference(m_fileListKeyItemList, defaultSetting.m_fileListKeyItemList,
                                     m_fileViewerKeyItemList, defaultSetting.m_fileViewerKeyItemList,
                                     m_graphicsViewerKeyItemList, defaultSetting.m_graphicsViewerKeyItemList,
                                     m_monitoringViewerKeyItemList, defaultSetting.m_monitoringViewerKeyItemList);
                if (diff.DifferenceCount > 0) {
                    VersionupKeyDiffDialog dialog = new VersionupKeyDiffDialog(diff);
                    dialog.ShowDialog(Program.MainWindow);
                } else {
                    SaveSetting();
                }
            }
        }

        //=========================================================================================
        // 機　能：設定をバックアップする
        // 引　数：[in]bakFilePath   バックアップファイルの名前
        // 戻り値：バックアップに成功したときtrue
        //=========================================================================================
        public bool Backup(string bakFilePath) {
            string fileName = DirectoryManager.KeySetting;
            try {
                File.Copy(fileName, bakFilePath);
            } catch (Exception) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(KeySetting obj1, KeySetting obj2) {
            if (!KeyItemSettingList.EqualsConfig(obj1.m_fileListKeyItemList, obj2.m_fileListKeyItemList)) {
                return false;
            }
            if (!KeyItemSettingList.EqualsConfig(obj1.m_fileViewerKeyItemList, obj2.m_fileViewerKeyItemList)) {
                return false;
            }
            if (!KeyItemSettingList.EqualsConfig(obj1.m_graphicsViewerKeyItemList, obj2.m_graphicsViewerKeyItemList)) {
                return false;
            }
            if (!KeyItemSettingList.EqualsConfig(obj1.m_monitoringViewerKeyItemList, obj2.m_monitoringViewerKeyItemList)) {
                return false;
            }
            if (!AssociateSetting.EqualsConfig(obj1.m_associateSetting, obj2.m_associateSetting)) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            KeySetting clone = new KeySetting();

            clone.m_fileListKeyItemList = (KeyItemSettingList)(m_fileListKeyItemList.Clone());
            clone.m_fileViewerKeyItemList = (KeyItemSettingList)(m_fileViewerKeyItemList.Clone());
            clone.m_graphicsViewerKeyItemList = (KeyItemSettingList)(m_graphicsViewerKeyItemList.Clone());
            clone.m_monitoringViewerKeyItemList = (KeyItemSettingList)(m_monitoringViewerKeyItemList.Clone());
            clone.m_associateSetting = (AssociateSetting)(m_associateSetting.Clone());
            clone.m_lastFileWriteTime = m_lastFileWriteTime;
            clone.m_loadedFileVersion = m_loadedFileVersion;

            return clone;
        }

        //=========================================================================================
        // 機　能：キー設定の一覧を取得する
        // 引　数：[in]commadScene   コマンドの利用シーン
        // 戻り値：キー設定の一覧
        //=========================================================================================
        public KeyItemSettingList GetKeyList(CommandUsingSceneType commandScene) {
            if (commandScene == CommandUsingSceneType.FileList) {
                return m_fileListKeyItemList;
            } else if (commandScene == CommandUsingSceneType.FileViewer) {
                return m_fileViewerKeyItemList;
            } else if (commandScene == CommandUsingSceneType.GraphicsViewer) {
                return m_graphicsViewerKeyItemList;
            } else if (commandScene == CommandUsingSceneType.MonitoringViewer) {
                return m_monitoringViewerKeyItemList;
            } else if (commandScene == CommandUsingSceneType.Terminal) {
                return m_terminalKeyItemList;
            } else {
                Program.Abort("未定義のキー利用シーンです。");
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルの書き込み日時
        //=========================================================================================
        public DateTime LastFileWriteTime {
            get {
                return m_lastFileWriteTime;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のキー定義
        //=========================================================================================
        public KeyItemSettingList FileListKeyItemList {
            get {
                return m_fileListKeyItemList;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルビューアのキー定義
        //=========================================================================================
        public KeyItemSettingList FileViewerKeyItemList {
            get {
                return m_fileViewerKeyItemList;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアのキー定義
        //=========================================================================================
        public KeyItemSettingList GraphicsViewerKeyItemList {
            get {
                return m_graphicsViewerKeyItemList;
            }
        }

        //=========================================================================================
        // プロパティ：ターミナルのキー定義
        //=========================================================================================
        public KeyItemSettingList TerminalKeyItemList {
            get {
                return m_terminalKeyItemList;
            }
        }

        //=========================================================================================
        // プロパティ：モニタリングビューアのキー定義
        //=========================================================================================
        public KeyItemSettingList MonitoringViewerKeyItemList {
            get {
                return m_monitoringViewerKeyItemList;
            }
        }

        //=========================================================================================
        // プロパティ：関連付け設定
        //=========================================================================================
        public AssociateSetting AssociateSetting {
            get {
                return m_associateSetting;
            }
        }
    }
}
