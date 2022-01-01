using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.FileOperationEtc;
using ShellFiler.Command.FileList.Internal;
using ShellFiler.Command.FileList.Open;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.MoveCursor;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.Command.FileList.SSH;
using ShellFiler.Command.FileList.Tools;
using ShellFiler.Command.FileList.Window;
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
using ShellFiler.Command.Terminal.File;
using ShellFiler.Command.Terminal.Edit;
using ShellFiler.Command.Terminal.View;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Properties;

namespace ShellFiler.Command {

    //=========================================================================================
    // クラス：コマンドのUI表現
    //=========================================================================================
    public class UIResource {
        // コマンドの一覧
        private static List<UIResource> s_commandList = new List<UIResource>();

        public static UIResource AboutExternalSoftwareCommand        = new UIResource(Ver.V0_0_0, typeof(AboutExternalSoftwareCommand),        CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_AboutExternalSoftware);
        public static UIResource AboutShellFilerCommand              = new UIResource(Ver.V0_0_0, typeof(AboutShellFilerCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_AboutShellFiler);
        public static UIResource ArchiveCommand                      = new UIResource(Ver.V0_0_0, typeof(ArchiveCommand),                      CommandRetType.Bool, UIEnableCondition.MarkPack, IconImageListID.FileList_Archive,             false, true,  Resources.MenuName_Archive);
        public static UIResource ArchiveAutoPasswordOptionCommand    = new UIResource(Ver.V0_0_0, typeof(ArchiveAutoPasswordOptionCommand),    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_ArchiveAutoPasswordOption);
        public static UIResource AssociateSettingCommand             = new UIResource(Ver.V0_0_0, typeof(AssociateSettingCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_AssociateSetting);
        public static UIResource BookmarkSettingCommand              = new UIResource(Ver.V0_0_0, typeof(BookmarkSettingCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_BookmarkSetting);
        public static UIResource BothChdirCursorFolderCommand        = new UIResource(Ver.V2_3_0, typeof(BothChdirCursorFolderCommand),        CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_BothChdirCursorFolder);
        public static UIResource BothChdirParentCommand              = new UIResource(Ver.V2_3_0, typeof(BothChdirParentCommand),              CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_BothChdirParent);
        public static UIResource ChdirCommand                        = new UIResource(Ver.V0_0_0, typeof(ChdirCommand),                        CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_Chdir);
        public static UIResource ChdirCursorFolderCommand            = new UIResource(Ver.V0_0_0, typeof(ChdirCursorFolderCommand),            CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ChdirCursorFolder);
        public static UIResource ChdirDriveCommand                   = new UIResource(Ver.V0_0_0, typeof(ChdirDriveCommand),                   CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_ChdirDrive);
        public static UIResource ChdirFolderHistoryCommand           = new UIResource(Ver.V0_0_0, typeof(ChdirFolderHistoryCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_ChdirFolderHistory);
        public static UIResource ChdirInputCommand                   = new UIResource(Ver.V0_0_0, typeof(ChdirInputCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ChdirInput);
        public static UIResource ChdirOppositeToTargetCommand        = new UIResource(Ver.V0_0_0, typeof(ChdirOppositeToTargetCommand),        CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ChdirOppositeToTarget);
        public static UIResource ChdirParentCommand                  = new UIResource(Ver.V0_0_0, typeof(ChdirParentCommand),                  CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.FileList_ChdirParent,         false, false, Resources.MenuName_ChdirParent);
        public static UIResource ChdirBookmarkFolderCommand          = new UIResource(Ver.V0_0_0, typeof(ChdirBookmarkFolderCommand),          CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.FileList_ChdirBookmarkFolder, false, true,  Resources.MenuName_ChdirBookmarkFolder);
        public static UIResource ChdirHomeCommand                    = new UIResource(Ver.V0_0_0, typeof(ChdirHomeCommand),                    CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ChdirHome);
        public static UIResource ChdirRootCommand                    = new UIResource(Ver.V0_0_0, typeof(ChdirRootCommand),                    CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ChdirRoot);
        public static UIResource ChdirSpecialCommand                 = new UIResource(Ver.V0_0_0, typeof(ChdirSpecialCommand),                 CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.FileList_ChdirSpecial,        false, false, Resources.MenuName_ChdirSpecial);
        public static UIResource ChdirSpecifiedPartCommand           = new UIResource(Ver.V2_3_0, typeof(ChdirSpecifiedPartCommand),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ChdirSpecifiedPart);
        public static UIResource ChdirSpecifiedPartReverseCommand    = new UIResource(Ver.V2_3_0, typeof(ChdirSpecifiedPartReverseCommand),    CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ChdirSpecifiedPartRev);
        public static UIResource ChdirSSHFolderCommand               = new UIResource(Ver.V0_0_0, typeof(ChdirSSHFolderCommand),               CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.FileList_ChdirSSHFolder,      false, true,  Resources.MenuName_ChdirSSHFolder);
        public static UIResource ChdirSwapTargetOppositeCommand      = new UIResource(Ver.V0_0_0, typeof(ChdirSwapTargetOppositeCommand),      CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ChdirSwapTargetOpposite);
        public static UIResource ChdirTargetToOppositeCommand        = new UIResource(Ver.V0_0_0, typeof(ChdirTargetToOppositeCommand),        CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ChdirTargetToOpposite);
        public static UIResource ChdirVirtualFolderCommand           = new UIResource(Ver.V1_2_0, typeof(ChdirVirtualFolderCommand),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ChdirVirtualFolder);
        public static UIResource CleanupRecycleBinCommand            = new UIResource(Ver.V0_0_0, typeof(CleanupRecycleBinCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CleanupRecycleBin);
        public static UIResource ClearAllMarkCommand                 = new UIResource(Ver.V0_0_0, typeof(ClearAllMarkCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ClearMarkAll,        false, false, Resources.MenuName_ClearAllMark);
        public static UIResource ClearAllMarkFileCommand             = new UIResource(Ver.V0_0_0, typeof(ClearAllMarkFileCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ClearAllMarkFile);
        public static UIResource ClearAllMarkFolderCommand           = new UIResource(Ver.V0_0_0, typeof(ClearAllMarkFolderCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ClearAllMarkFolder);
        public static UIResource ClearFileListFilterCommand          = new UIResource(Ver.V1_1_0, typeof(ClearFileListFilterCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_FilterClear,         false, false, Resources.MenuName_ClearFileListFilter);
        public static UIResource ClearFolderSizeCommand              = new UIResource(Ver.V1_3_0, typeof(ClearFolderSizeCommand),              CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ClearFolderSize);
        public static UIResource ClearLogCommand                     = new UIResource(Ver.V0_0_0, typeof(ClearLogCommand),                     CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ClearLog);
        public static UIResource ClipboardCopyCommand                = new UIResource(Ver.V0_0_0, typeof(ClipboardCopyCommand),                CommandRetType.Bool, UIEnableCondition.WithMark, IconImageListID.None,                         false, false, Resources.MenuName_ClipboardCopy);
        public static UIResource ClipboardCutCommand                 = new UIResource(Ver.V0_0_0, typeof(ClipboardCutCommand),                 CommandRetType.Bool, UIEnableCondition.WithMark, IconImageListID.None,                         false, false, Resources.MenuName_ClipboardCut);
        public static UIResource ClipboardFilterCommand              = new UIResource(Ver.V1_0_0, typeof(ClipboardFilterCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_ClipboardFilter);
        public static UIResource ClipboardNameCopyCommand            = new UIResource(Ver.V0_0_0, typeof(ClipboardNameCopyCommand),            CommandRetType.Bool, UIEnableCondition.WithMark, IconImageListID.None,                         false, false, Resources.MenuName_ClipboardNameCopy);
        public static UIResource ClipboardNameCopyAsCommand          = new UIResource(Ver.V0_0_0, typeof(ClipboardNameCopyAsCommand),          CommandRetType.Bool, UIEnableCondition.WithMark, IconImageListID.None,                         false, true,  Resources.MenuName_ClipboardNameCopyAs);
        public static UIResource ClipboardViewerCommand              = new UIResource(Ver.V1_0_0, typeof(ClipboardViewerCommand),              CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ClipboardViewer);
        public static UIResource CloseAllViewerCommand               = new UIResource(Ver.V0_0_0, typeof(CloseAllViewerCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CloseAllViewer);
        public static UIResource CombineFileCommand                  = new UIResource(Ver.V2_1_0, typeof(CombineFileCommand),                  CommandRetType.Void, UIEnableCondition.WithMark, IconImageListID.FileList_CombineFile,         true,  true,  Resources.MenuName_CombineFile);
        public static UIResource CommandListHelpCommand              = new UIResource(Ver.V1_3_0, typeof(CommandListHelpCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_CommandListHelp);
        public static UIResource ContextMenuCommand                  = new UIResource(Ver.V0_0_0, typeof(ContextMenuCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ContextMenu,         false, false, Resources.MenuName_ContextMenu);
        public static UIResource ContextMenuFolderCommand            = new UIResource(Ver.V0_0_0, typeof(ContextMenuFolderCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ContextMenuFolder);
        public static UIResource CopyCommand                         = new UIResource(Ver.V0_0_0, typeof(CopyCommand),                         CommandRetType.Bool, UIEnableCondition.MarkCopy, IconImageListID.FileList_Copy,                true,  false, Resources.MenuName_Copy);
        public static UIResource CopyExCommand                       = new UIResource(Ver.V1_1_0, typeof(CopyExCommand),                       CommandRetType.Bool, UIEnableCondition.MarkCopy, IconImageListID.None,                         false, true,  Resources.MenuName_CopyEx);
        public static UIResource CopyLogCommand                      = new UIResource(Ver.V1_4_0, typeof(CopyLogCommand),                      CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CopyLog);            
        public static UIResource CreateShortcutCommand               = new UIResource(Ver.V0_0_0, typeof(CreateShortcutCommand),               CommandRetType.Bool, UIEnableCondition.MarkShortcut, IconImageListID.FileList_Shortcut,        true,  false, Resources.MenuName_CreateShortcut);
        public static UIResource CreateSSHTerminalCommand            = new UIResource(Ver.V2_0_0, typeof(CreateSSHTerminalCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.Terminal_WindowNew,           false, false, Resources.MenuName_CreateSSHTerminal);
        public static UIResource CursorBottomCommand                 = new UIResource(Ver.V1_3_0, typeof(CursorBottomCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorBottom);
        public static UIResource CursorDownCommand                   = new UIResource(Ver.V0_0_0, typeof(CursorDownCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorDown);
        public static UIResource CursorDownMarkCommand               = new UIResource(Ver.V0_0_0, typeof(CursorDownMarkCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorDownMark);
        public static UIResource CursorLeftCommand                   = new UIResource(Ver.V0_0_0, typeof(CursorLeftCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorLeft);
        public static UIResource CursorLeftWindowCommand             = new UIResource(Ver.V2_2_0, typeof(CursorLeftWindowCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorLeftWindow);
        public static UIResource CursorRightCommand                  = new UIResource(Ver.V0_0_0, typeof(CursorRightCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorRight);
        public static UIResource CursorRightWindowCommand            = new UIResource(Ver.V2_2_0, typeof(CursorRightWindowCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorRightWindow);
        public static UIResource CursorRolldownCommand               = new UIResource(Ver.V0_0_0, typeof(CursorRolldownCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorRolldown);
        public static UIResource CursorRolldownMarkCommand           = new UIResource(Ver.V0_0_0, typeof(CursorRolldownMarkCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorRolldownMark);
        public static UIResource CursorRollupCommand                 = new UIResource(Ver.V0_0_0, typeof(CursorRollupCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorRollup);
        public static UIResource CursorRollupMarkCommand             = new UIResource(Ver.V0_0_0, typeof(CursorRollupMarkCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorRollupMark);
        public static UIResource CursorTopCommand                    = new UIResource(Ver.V1_3_0, typeof(CursorTopCommand),                    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorTop);
        public static UIResource CursorUpCommand                     = new UIResource(Ver.V0_0_0, typeof(CursorUpCommand),                     CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorUp);
        public static UIResource CursorUpMarkCommand                 = new UIResource(Ver.V0_0_0, typeof(CursorUpMarkCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_CursorUpMark);
        public static UIResource DeleteCommand                       = new UIResource(Ver.V0_0_0, typeof(DeleteCommand),                       CommandRetType.Bool, UIEnableCondition.MarkDelete, IconImageListID.FileList_Delete,            false, true,  Resources.MenuName_Delete);
        public static UIResource DeleteExCommand                     = new UIResource(Ver.V1_1_0, typeof(DeleteExCommand),                     CommandRetType.Bool, UIEnableCondition.MarkDelete, IconImageListID.None,                       false, true,  Resources.MenuName_DeleteEx);
        public static UIResource DeleteHistoryCommand                = new UIResource(Ver.V0_0_0, typeof(DeleteHistoryCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_DeleteHistory);
        public static UIResource DeleteUserFolderCommand             = new UIResource(Ver.V0_0_0, typeof(DeleteUserFolderCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_DeleteUserFolder);
        public static UIResource DuplicateCommand                    = new UIResource(Ver.V2_1_0, typeof(DuplicateCommand),                    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_Duplicate);
        public static UIResource DiffFolderCompareCommand            = new UIResource(Ver.V0_0_0, typeof(DiffFolderCompareCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_DiffFolder,          false, false, Resources.MenuName_DiffFolderCompare);
        public static UIResource DiffMarkCommand                     = new UIResource(Ver.V0_0_0, typeof(DiffMarkCommand),                     CommandRetType.Void, UIEnableCondition.WithMark, IconImageListID.FileList_DiffMark,            false, false, Resources.MenuName_DiffMark);
        public static UIResource DiffOppositeCommand                 = new UIResource(Ver.V0_0_0, typeof(DiffOppositeCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_DiffOpposite,        false, false, Resources.MenuName_DiffOpposite);
        public static UIResource ExecuteMenuCommand                  = new UIResource(Ver.V0_0_0, typeof(ExecuteMenuCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ExecuteMenu);
        public static UIResource ExecuteOrViewerCommand              = new UIResource(Ver.V1_0_0, typeof(ExecuteOrViewerCommand),              CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ExecuteOrViewer);
        public static UIResource ExitCommand                         = new UIResource(Ver.V0_0_0, typeof(ExitCommand),                         CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_Exit,                false, false, Resources.MenuName_Exit);
        public static UIResource ExtractCommand                      = new UIResource(Ver.V0_0_0, typeof(ExtractCommand),                      CommandRetType.Bool, UIEnableCondition.MarkUnpack, IconImageListID.FileList_Extract,           false, false, Resources.MenuName_Extract);
        public static UIResource FileCompareCommand                  = new UIResource(Ver.V0_0_0, typeof(FileCompareCommand),                  CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_FileCompare);        
        public static UIResource FileListColorSettingCommand         = new UIResource(Ver.V1_2_0, typeof(FileListColorSettingCommand),         CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_FileListColorSetting);
        public static UIResource FileListFilterMenuCommand           = new UIResource(Ver.V1_1_0, typeof(FileListFilterMenuCommand),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.FileList_Filter,              false, true,  Resources.MenuName_FileListFilter);
        public static UIResource FileListKeySettingCommand           = new UIResource(Ver.V0_0_0, typeof(FileListKeySettingCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_FileListKeySetting);
        public static UIResource FileListMenuSettingCommand          = new UIResource(Ver.V1_3_0, typeof(FileListMenuSettingCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_FileListMenuSetting);
        public static UIResource FileListViewModeDetailCommand       = new UIResource(Ver.V2_2_0, typeof(FileListViewModeDetailCommand),       CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ViewModeDetail,      false, false, Resources.MenuName_FileListViewModeDetail);
        public static UIResource FileListViewModeLargerCommand       = new UIResource(Ver.V2_2_0, typeof(FileListViewModeLargerCommand),       CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_FileListViewModeLarger);
        public static UIResource FileListViewModeSmallerCommand      = new UIResource(Ver.V2_2_0, typeof(FileListViewModeSmallerCommand),      CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_FileListViewModeSmaller);
        public static UIResource FileListViewModeSettingCommand      = new UIResource(Ver.V2_2_0, typeof(FileListViewModeSettingCommand),      CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ViewModeSetting,     false, true,  Resources.MenuName_FileListViewModeSetting);
        public static UIResource FileListViewModeThumbnailCommand    = new UIResource(Ver.V2_2_0, typeof(FileListViewModeThumbnailCommand),    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ViewMode32,          false, false, Resources.MenuName_FileListViewModeThumbnail);
        public static UIResource FileListViewModeThumbnail32Command  = new UIResource(Ver.V2_2_0, typeof(FileListViewModeThumbnail32Command),  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ViewMode32,          false, false, Resources.MenuName_FileListViewModeThumbnail32);
        public static UIResource FileListViewModeThumbnail48Command  = new UIResource(Ver.V2_2_0, typeof(FileListViewModeThumbnail48Command),  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ViewMode48,          false, false, Resources.MenuName_FileListViewModeThumbnail48);
        public static UIResource FileListViewModeThumbnail64Command  = new UIResource(Ver.V2_2_0, typeof(FileListViewModeThumbnail64Command),  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ViewMode64,          false, false, Resources.MenuName_FileListViewModeThumbnail64);
        public static UIResource FileListViewModeThumbnail128Command = new UIResource(Ver.V2_2_0, typeof(FileListViewModeThumbnail128Command), CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ViewMode128,         false, false, Resources.MenuName_FileListViewModeThumbnail128);
        public static UIResource FileListViewModeThumbnail256Command = new UIResource(Ver.V2_2_0, typeof(FileListViewModeThumbnail256Command), CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ViewMode256,         false, false, Resources.MenuName_FileListViewModeThumbnail256);
        public static UIResource FileListViewModeToggleCommand       = new UIResource(Ver.V2_2_0, typeof(FileListViewModeToggleCommand),       CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_FileListViewModeToggle);
        public static UIResource FileViewerCommand                   = new UIResource(Ver.V0_0_0, typeof(FileViewerCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_FileViewer,          false, false, Resources.MenuName_FileViewer);
        public static UIResource FileViewerKeySettingCommand         = new UIResource(Ver.V0_0_0, typeof(FileViewerKeySettingCommand),         CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_FileViewerKeySetting);
        public static UIResource FilterCopyCommand                   = new UIResource(Ver.V1_0_0, typeof(FilterCopyCommand),                   CommandRetType.Bool, UIEnableCondition.MarkCopy, IconImageListID.None,                         false, true,  Resources.MenuName_FilterCopy);
        public static UIResource GitAddCommand                       = new UIResource(Ver.V3_0_0, typeof(GitAddCommand),                       CommandRetType.Bool, UIEnableCondition.MarkCopy, IconImageListID.None,                         false, false, Resources.MenuName_GitAdd);
        public static UIResource GitMoveCommand                      = new UIResource(Ver.V3_0_0, typeof(GitMoveCommand),                      CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_GitMove);
        public static UIResource GitRenameCommand                    = new UIResource(Ver.V3_0_0, typeof(GitRenameCommand),                    CommandRetType.Bool, UIEnableCondition.MarkCopy, IconImageListID.None,                         false, true,  Resources.MenuName_GitRename);
        public static UIResource GraphicsViewerCommand               = new UIResource(Ver.V0_0_0, typeof(GraphicsViewerCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_GraphicsViewer,      false, false, Resources.MenuName_GraphicsViewer);
        public static UIResource GraphicsViewerKeySettingCommand     = new UIResource(Ver.V0_0_0, typeof(GraphicsViewerKeySettingCommand),     CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_GraphicsViewerKeySetting);
        public static UIResource HighspeedDeleteCommand              = new UIResource(Ver.V0_0_0, typeof(HighspeedDeleteCommand),              CommandRetType.Bool, UIEnableCondition.MarkDelete, IconImageListID.None,                       false, true,  Resources.MenuName_HighspeedDelete);
        public static UIResource HTTPResponseViewerCommand           = new UIResource(Ver.V1_0_0, typeof(HTTPResponseViewerCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_HTTPResponseViewer);
        public static UIResource IncrementalSearchCommand            = new UIResource(Ver.V0_0_0, typeof(IncrementalSearchCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_IncrementalSearch);
        public static UIResource IncrementalSearchExCommand          = new UIResource(Ver.V1_3_0, typeof(IncrementalSearchExCommand),          CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_IncrementalSearchEx);
        public static UIResource KeyBindHelpCommand                  = new UIResource(Ver.V1_3_0, typeof(KeyBindHelpCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_KeyBindHelp);
        public static UIResource KeyListMenuCommand                  = new UIResource(Ver.V0_0_0, typeof(KeyListMenuCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_KeyListMenu,         false, true,  Resources.MenuName_KeyListMenu);
        public static UIResource KeyListHelpCommand                  = new UIResource(Ver.V1_3_0, typeof(KeyListHelpCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_KeyListHelp);
        public static UIResource LocalEditCommand                    = new UIResource(Ver.V0_0_0, typeof(LocalEditCommand),                    CommandRetType.Void, UIEnableCondition.MarkEdit, IconImageListID.FileList_LocalEdit,           false, false, Resources.MenuName_LocalEdit);
        public static UIResource LocalExecuteCommand                 = new UIResource(Ver.V0_0_0, typeof(LocalExecuteCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_LocalExecute);
        public static UIResource LogoutAllSSHSessionCommand          = new UIResource(Ver.V1_3_0, typeof(LogoutAllSSHSessionCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_LogoutAllSSHSession);
        public static UIResource LogoutCurrentSSHSessionCommand      = new UIResource(Ver.V1_3_0, typeof(LogoutCurrentSSHSessionCommand),      CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_LogoutCurrentSSHSession);
        public static UIResource MakeFolderCommand                   = new UIResource(Ver.V0_0_0, typeof(MakeFolderCommand),                   CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.FileList_MakeDirectory,       false, true,  Resources.MenuName_MakeDirectory);
        public static UIResource MarkCommand                         = new UIResource(Ver.V0_0_0, typeof(MarkCommand),                         CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_Mark);
        public static UIResource MarkWithConditionsCommand           = new UIResource(Ver.V1_1_0, typeof(MarkWithConditionsCommand),           CommandRetType.Int,  UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_MarkWithConditions);
        public static UIResource MenuListHelpCommand                 = new UIResource(Ver.V1_3_0, typeof(MenuListHelpCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_MenuListHelp);
        public static UIResource MirrorCopyCommand                   = new UIResource(Ver.V1_3_0, typeof(MirrorCopyCommand),                   CommandRetType.Bool, UIEnableCondition.MarkCopy, IconImageListID.FileList_MirrorCopy,          true,  true,  Resources.MenuName_MirrorCopy);
        public static UIResource MouseContextMenuCommand             = new UIResource(Ver.V0_0_0, typeof(MouseContextMenuCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_MouseContextMenu);
        public static UIResource MouseDragMarkCommand                = new UIResource(Ver.V0_0_0, typeof(MouseDragMarkCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_MouseDragMark);
        public static UIResource MouseDragSelectCommand              = new UIResource(Ver.V0_0_0, typeof(MouseDragSelectCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_MouseDragSelect);
        public static UIResource MouseExplorerSelectCommand          = new UIResource(Ver.V0_0_0, typeof(MouseExplorerSelectCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_MouseExplorerSelect);
        public static UIResource MouseHelpCommand                    = new UIResource(Ver.V0_0_0, typeof(MouseHelpCommand),                    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_MouseHelp);
        public static UIResource MoveCommand                         = new UIResource(Ver.V0_0_0, typeof(MoveCommand),                         CommandRetType.Bool, UIEnableCondition.MarkMove, IconImageListID.FileList_Move,                true,  false, Resources.MenuName_Move);
        public static UIResource MoveExCommand                       = new UIResource(Ver.V1_1_0, typeof(MoveExCommand),                       CommandRetType.Bool, UIEnableCondition.MarkMove, IconImageListID.None,                         false, true,  Resources.MenuName_MoveEx);
        public static UIResource MoveFileListBorderCommand           = new UIResource(Ver.V0_0_0, typeof(MoveFileListBorderCommand),           CommandRetType.Int,  UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_MoveFileListBorder);
        public static UIResource NopCommand                          = new UIResource(Ver.V0_0_0, typeof(NopCommand),                          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_Nop);
        public static UIResource OpenApplicationFileCommand          = new UIResource(Ver.V1_0_0, typeof(OpenApplicationFileCommand),          CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenApplicationFile);
        public static UIResource OpenControlPanelCommand             = new UIResource(Ver.V0_0_0, typeof(OpenControlPanelCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenControlPanel);
        public static UIResource OpenExplorerFolderCommand           = new UIResource(Ver.V0_0_0, typeof(OpenExplorerFolderCommand),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenExplorerFolder);
        public static UIResource OpenFileAssociate1Command           = new UIResource(Ver.V0_0_0, typeof(OpenFileAssociate1Command),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenFileAssociate1);
        public static UIResource OpenFileAssociate2Command           = new UIResource(Ver.V0_0_0, typeof(OpenFileAssociate2Command),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenFileAssociate2);
        public static UIResource OpenFileAssociate3Command           = new UIResource(Ver.V1_0_0, typeof(OpenFileAssociate3Command),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenFileAssociate3);
        public static UIResource OpenFileAssociate4Command           = new UIResource(Ver.V1_0_0, typeof(OpenFileAssociate4Command),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenFileAssociate4);
        public static UIResource OpenFileAssociate5Command           = new UIResource(Ver.V1_0_0, typeof(OpenFileAssociate5Command),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenFileAssociate5);
        public static UIResource OpenFileAssociate6Command           = new UIResource(Ver.V1_0_0, typeof(OpenFileAssociate6Command),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenFileAssociate6);
        public static UIResource OpenFileAssociate7Command           = new UIResource(Ver.V1_0_0, typeof(OpenFileAssociate7Command),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenFileAssociate7);
        public static UIResource OpenFileAssociate8Command           = new UIResource(Ver.V1_0_0, typeof(OpenFileAssociate8Command),           CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenFileAssociate8);
        public static UIResource OpenRecycleBinCommand               = new UIResource(Ver.V0_0_0, typeof(OpenRecycleBinCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_OpenRecycleBin);
        public static UIResource OptionSettingCommand                = new UIResource(Ver.V0_0_0, typeof(OptionSettingCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_OptionSetting,       false, true,  Resources.MenuName_OptionSetting);
        public static UIResource PathHistoryPrevCommand              = new UIResource(Ver.V0_0_0, typeof(PathHistoryPrevCommand),              CommandRetType.Bool, UIEnableCondition.PathHistP,IconImageListID.FileList_PathHistoryPrev,     false, false, Resources.MenuName_PathHistoryPrev);
        public static UIResource PathHistoryNextCommand              = new UIResource(Ver.V0_0_0, typeof(PathHistoryNextCommand),              CommandRetType.Bool, UIEnableCondition.PathHistN,IconImageListID.FileList_PathHistoryNext,     false, false, Resources.MenuName_PathHistoryNext);
        public static UIResource PopupMenuCommand                    = new UIResource(Ver.V1_3_0, typeof(PopupMenuCommand),                    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_PopupMenu);
        public static UIResource PowerShellCommand                   = new UIResource(Ver.V1_0_0, typeof(PowerShellCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_PowerShell,          false, false, Resources.MenuName_PowerShell);
        public static UIResource PrepareVirtualFolderCommand         = new UIResource(Ver.V1_2_0, typeof(PrepareVirtualFolderCommand),         CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_PrepareVirtualFolder);
        public static UIResource QuickExitCommand                    = new UIResource(Ver.V0_0_0, typeof(QuickExitCommand),                    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_Exit,                false, false, Resources.MenuName_QuickExit);
        public static UIResource RefreshFileListCommand              = new UIResource(Ver.V0_0_0, typeof(RefreshFileListCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_Refresh,             false, false, Resources.MenuName_RefreshFileList);
        public static UIResource RenameCommand                       = new UIResource(Ver.V0_0_0, typeof(RenameCommand),                       CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.FileList_Rename,              false, true,  Resources.MenuName_Rename);
        public static UIResource RenameSelectedFileInfoCommand       = new UIResource(Ver.V0_0_0, typeof(RenameSelectedFileInfoCommand),       CommandRetType.Bool, UIEnableCondition.MarkAttribute, IconImageListID.FileList_RenameSelected, false, true,  Resources.MenuName_RenameSelected);
        public static UIResource ReverseAllMarkCommand               = new UIResource(Ver.V0_0_0, typeof(ReverseAllMarkCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ReverseAllMark);
        public static UIResource ReverseAllMarkFileCommand           = new UIResource(Ver.V0_0_0, typeof(ReverseAllMarkFileCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ReverseMarkFile,     false, false, Resources.MenuName_ReverseAllMarkFile);
        public static UIResource ReverseAllMarkFolderCommand         = new UIResource(Ver.V0_0_0, typeof(ReverseAllMarkFolderCommand),         CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ReverseAllMarkFolder);
        public static UIResource RetrieveFolderSizeCommand           = new UIResource(Ver.V1_3_0, typeof(RetrieveFolderSizeCommand),           CommandRetType.Void, UIEnableCondition.MarkFolderSize, IconImageListID.FileList_RetrieveFolderSize, false, true,  Resources.MenuName_RetrieveFolderSize);
        public static UIResource SelectAllLogLineCommand             = new UIResource(Ver.V1_4_0, typeof(SelectAllLogLineCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_SelectAllLogLine);
        public static UIResource SelectAllMarkCommand                = new UIResource(Ver.V0_0_0, typeof(SelectAllMarkCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_SelectAllMark);
        public static UIResource SelectAllMarkFileCommand            = new UIResource(Ver.V0_0_0, typeof(SelectAllMarkFileCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_SelectAllMarkFile);
        public static UIResource SelectAllMarkFolderCommand          = new UIResource(Ver.V0_0_0, typeof(SelectAllMarkFolderCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_SelectAllMarkFolder);
        public static UIResource SelectSSHTerminalCommand            = new UIResource(Ver.V2_0_0, typeof(SelectSSHTerminalCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_SelectSSHTerminal);
        public static UIResource SetFileListBorderRatioCommand       = new UIResource(Ver.V0_0_0, typeof(SetFileListBorderRatioCommand),       CommandRetType.Int,  UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_SetFileListBorderRatio);
        public static UIResource SetFocusStatePanelCommand           = new UIResource(Ver.V1_1_0, typeof(SetFocusStatePanelCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_SetFocusStatePanel);
        public static UIResource ShellCommandCommand                 = new UIResource(Ver.V0_0_0, typeof(ShellCommandCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_ShellCommand,        false, false, Resources.MenuName_ShellCommand);
        public static UIResource ShellExecuteMenuCommand             = new UIResource(Ver.V0_0_0, typeof(ShellExecuteMenuCommand),             CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.FileList_ShellExecute,        false, true,  Resources.MenuName_ShellExecuteMenu);
        public static UIResource ShowLatestLogLineCommand            = new UIResource(Ver.V1_4_0, typeof(ShowLatestLogLineCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ShowLatestLogLine);
        public static UIResource ShowPropertyCommand                 = new UIResource(Ver.V1_3_0, typeof(ShowPropertyCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ShowProperty);
        public static UIResource ShowPropertyFolderCommand           = new UIResource(Ver.V1_3_0, typeof(ShowPropertyFolderCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ShowFolderProperty);
        public static UIResource SlideShowCommand                    = new UIResource(Ver.V0_0_0, typeof(SlideShowCommand),                    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_SlideShow,           false, false, Resources.MenuName_SlideShow);
        public static UIResource SlideShowMarkResultCommand          = new UIResource(Ver.V1_0_0, typeof(SlideShowMarkResultCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_SlideShowMarkResult);
        public static UIResource SortMenuCommand                     = new UIResource(Ver.V0_0_0, typeof(SortMenuCommand),                     CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.FileList_SortMenu,            false, true,  Resources.MenuName_SortMenu);
        public static UIResource SortClearCommand                    = new UIResource(Ver.V0_0_0, typeof(SortClearCommand),                    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_SortClear,           false, false, Resources.MenuName_SortClear);
        public static UIResource SplitFileCommand                    = new UIResource(Ver.V2_1_0, typeof(SplitFileCommand),                    CommandRetType.Void, UIEnableCondition.WithMark, IconImageListID.FileList_SplitFile,           true,  true,  Resources.MenuName_SplitFile);
        public static UIResource SSHChangeUserCommand                = new UIResource(Ver.V2_0_0, typeof(SSHChangeUserCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileList_SSHChangeUser,       false, true,  Resources.MenuName_SSHChangeUser);
        public static UIResource SSHProcessMonitorCommand            = new UIResource(Ver.V2_0_0, typeof(SSHProcessMonitorCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_SSHProcessMonitor);
        public static UIResource SSHNetworkMonitorCommand            = new UIResource(Ver.V2_0_0, typeof(SSHNetworkMonitorCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_SSHNetworkMonitor);
        public static UIResource TabCreateCommand                    = new UIResource(Ver.V0_0_0, typeof(TabCreateCommand),                    CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TabCreate);
        public static UIResource TabDeleteCommand                    = new UIResource(Ver.V0_0_0, typeof(TabDeleteCommand),                    CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TabDelete);
        public static UIResource TabDeleteLeftCommand                = new UIResource(Ver.V0_0_0, typeof(TabDeleteLeftCommand),                CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TabDeleteLeft);
        public static UIResource TabDeleteOtherCommand               = new UIResource(Ver.V0_0_0, typeof(TabDeleteOtherCommand),               CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TabDeleteOther);
        public static UIResource TabDeleteRightCommand               = new UIResource(Ver.V0_0_0, typeof(TabDeleteRightCommand),               CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TabDeleteRight);
        public static UIResource TabReopenCommand                    = new UIResource(Ver.V0_0_0, typeof(TabReopenCommand),                    CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TabReopen);
        public static UIResource TabSelectDirectCommand              = new UIResource(Ver.V0_0_0, typeof(TabSelectDirectCommand),              CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TabSelectDirect);
        public static UIResource TabSelectLeftCommand                = new UIResource(Ver.V0_0_0, typeof(TabSelectLeftCommand),                CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TabSelectLeft);
        public static UIResource TabSelectRightCommand               = new UIResource(Ver.V0_0_0, typeof(TabSelectRightCommand),               CommandRetType.Bool, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TabSelectRight);
        public static UIResource TaskManagerCommand                  = new UIResource(Ver.V0_0_0, typeof(TaskManagerCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_TaskManager);
        public static UIResource ToggleLogSizeCommand                = new UIResource(Ver.V0_0_0, typeof(ToggleLogSizeCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ToggleLogWindowSize);
        public static UIResource TwoStrokeKey1Command                = new UIResource(Ver.V1_3_0, typeof(TwoStrokeKey1Command),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TwoStrokeKey1);
        public static UIResource TwoStrokeKey2Command                = new UIResource(Ver.V1_3_0, typeof(TwoStrokeKey1Command),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TwoStrokeKey2);
        public static UIResource TwoStrokeKey3Command                = new UIResource(Ver.V1_3_0, typeof(TwoStrokeKey1Command),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TwoStrokeKey3);
        public static UIResource TwoStrokeKey4Command                = new UIResource(Ver.V1_3_0, typeof(TwoStrokeKey1Command),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_TwoStrokeKey4);
        public static UIResource UnwrapFolderCommand                 = new UIResource(Ver.V2_3_0, typeof(UnwrapFolderCommand),                 CommandRetType.Void, UIEnableCondition.WithMark, IconImageListID.None,                         false, true,  Resources.MenuName_UnwrapFolder);
        public static UIResource ShellFilerWebPageCommand            = new UIResource(Ver.V0_0_0, typeof(ShellFilerWebPageCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_ShellFilerWebPage);

        public static UIResource V_ChangeDumpModeCommand             = new UIResource(Ver.V0_0_0, typeof(V_ChangeDumpModeCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_ChangeDump,        false, false, Resources.MenuName_V_ChangeDump);
        public static UIResource V_ChangeTextModeCommand             = new UIResource(Ver.V0_0_0, typeof(V_ChangeTextModeCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_ChangeText,        false, false, Resources.MenuName_V_ChangeText);
        public static UIResource V_CopyTextCommand                   = new UIResource(Ver.V0_0_0, typeof(V_CopyTextCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_CopyText,          false, false, Resources.MenuName_V_CopyText);
        public static UIResource V_CopyTextAsCommand                 = new UIResource(Ver.V0_0_0, typeof(V_CopyTextAsCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_V_CopyTextAs);
        public static UIResource V_CursorDownCommand                 = new UIResource(Ver.V0_0_0, typeof(V_CursorDownCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_CursorDown);
        public static UIResource V_CursorUpCommand                   = new UIResource(Ver.V0_0_0, typeof(V_CursorUpCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_CursorUp);
        public static UIResource V_CursorRolldownCommand             = new UIResource(Ver.V0_0_0, typeof(V_CursorRolldownCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_CursorRolldown);
        public static UIResource V_CursorRollupCommand               = new UIResource(Ver.V0_0_0, typeof(V_CursorRollupCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_CursorRollup);
        public static UIResource V_EditFileCommand                   = new UIResource(Ver.V1_1_0, typeof(V_EditFileCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_EditFile,          false, false, Resources.MenuName_V_EditFile);
        public static UIResource V_ExitViewCommand                   = new UIResource(Ver.V0_0_0, typeof(V_ExitViewCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_ExitView,          false, false, Resources.MenuName_V_ExitView);
        public static UIResource V_FullScreenCommand                 = new UIResource(Ver.V0_0_0, typeof(V_FullScreenCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_FullScreen,        false, false, Resources.MenuName_V_FullScreen);
        public static UIResource V_FullScreenMultiCommand            = new UIResource(Ver.V0_0_0, typeof(V_FullScreenMultiCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_FullScreenMulti);
        public static UIResource V_JumpTopLineCommand                = new UIResource(Ver.V0_0_0, typeof(V_JumpTopLineCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_JumpTopLine,       false, false, Resources.MenuName_V_JumpTopLine);
        public static UIResource V_JumpBottomLineCommand             = new UIResource(Ver.V0_0_0, typeof(V_JumpBottomLineCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_JumpBottomLine,    false, false, Resources.MenuName_V_JumpBottomLine);
        public static UIResource V_JumpDirectCommand                 = new UIResource(Ver.V0_0_0, typeof(V_JumpDirectCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_JumpDirect,        false, true,  Resources.MenuName_V_JumpDirect);
        public static UIResource V_ReturnFileListCommand             = new UIResource(Ver.V1_3_0, typeof(V_ReturnFileListCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_Return,            false, false, Resources.MenuName_V_ReturnFileList);
        public static UIResource V_RotateTextModeCommand             = new UIResource(Ver.V0_0_0, typeof(V_RotateTextModeCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_RotateTextMode);
        public static UIResource V_SaveTextAsCommand                 = new UIResource(Ver.V1_0_0, typeof(V_SaveTextAsCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_Save,              false, true,  Resources.MenuName_V_SaveTextAsCommand);
        public static UIResource V_SearchCommand                     = new UIResource(Ver.V0_0_0, typeof(V_SearchCommand),                     CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_Search,            false, true,  Resources.MenuName_V_Search);
        public static UIResource V_SearchDirectCommand               = new UIResource(Ver.V0_0_0, typeof(V_SearchDirectCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_SearchDirect);
        public static UIResource V_SearchForwardNextCommand          = new UIResource(Ver.V0_0_0, typeof(V_SearchForwardNextCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_SearchForwardNext, false, false, Resources.MenuName_V_SearchForwardNext);
        public static UIResource V_SearchReverseNextCommand          = new UIResource(Ver.V0_0_0, typeof(V_SearchReverseNextCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_SearchReverseNext, false, false, Resources.MenuName_V_SearchReverseNext);
        public static UIResource V_SelectAllCommand                  = new UIResource(Ver.V0_0_0, typeof(V_SelectAllCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.FileViewer_SelectAll,         false, false, Resources.MenuName_V_SelectAll);
        public static UIResource V_SelectionCompareDisplayCommand    = new UIResource(Ver.V1_0_0, typeof(V_SelectionCompareDisplayCommand),    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_V_SelectionCompareDisplay);
        public static UIResource V_SelectionCompareLeftCommand       = new UIResource(Ver.V1_0_0, typeof(V_SelectionCompareLeftCommand),       CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_SelectionCompareLeft);
        public static UIResource V_SelectionCompareRightCommand      = new UIResource(Ver.V1_0_0, typeof(V_SelectionCompareRightCommand),      CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_SelectionCompareRight);
        public static UIResource V_SetLineWidthCommand               = new UIResource(Ver.V0_0_0, typeof(V_SetLineWidthCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_V_SetLineWidth);
        public static UIResource V_SetTabCommand                     = new UIResource(Ver.V0_0_0, typeof(V_SetTabCommand),                     CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_SetTab);
        public static UIResource V_SetTextEncodingCommand            = new UIResource(Ver.V0_0_0, typeof(V_SetTextEncodingCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_SetTextEncoding);
        public static UIResource V_ToggleViewModeCommand             = new UIResource(Ver.V0_0_0, typeof(V_ToggleViewModeCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_ToggleViewMode);
        public static UIResource V_TwoStrokeKey1Command              = new UIResource(Ver.V1_3_0, typeof(V_TwoStrokeKey1Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_TwoStrokeKey1);
        public static UIResource V_TwoStrokeKey2Command              = new UIResource(Ver.V1_3_0, typeof(V_TwoStrokeKey2Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_TwoStrokeKey2);
        public static UIResource V_TwoStrokeKey3Command              = new UIResource(Ver.V1_3_0, typeof(V_TwoStrokeKey3Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_TwoStrokeKey3);
        public static UIResource V_TwoStrokeKey4Command              = new UIResource(Ver.V1_3_0, typeof(V_TwoStrokeKey4Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_V_TwoStrokeKey4);

        public static UIResource G_ClearMarkCommand                  = new UIResource(Ver.V1_0_0, typeof(G_ClearMarkCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_ClearMark);
        public static UIResource G_CopyImageCommand                  = new UIResource(Ver.V0_0_0, typeof(G_CopyImageCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_CopyImage,     false, false, Resources.MenuName_G_CopyImage);
        public static UIResource G_ExitViewCommand                   = new UIResource(Ver.V0_0_0, typeof(G_ExitViewCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ExitView,      false, false, Resources.MenuName_G_ExitView);
        public static UIResource G_ExitViewOrNextSlideCommand        = new UIResource(Ver.V0_0_0, typeof(G_ExitViewOrNextSlideCommand),        CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_SlideShowNext, false, false, Resources.MenuName_G_ExitViewOrNextSlide);
        public static UIResource G_ExitViewOrPrevSlideCommand        = new UIResource(Ver.V0_0_0, typeof(G_ExitViewOrPrevSlideCommand),        CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_SlideShowPrev, false, false, Resources.MenuName_G_ExitViewOrPrevSlide);
        public static UIResource G_FilterBlurCommand                 = new UIResource(Ver.V0_0_0, typeof(G_FilterBlurCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FilterBlure);
        public static UIResource G_FilterBrightCommand               = new UIResource(Ver.V0_0_0, typeof(G_FilterBrightCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FilterBright);
        public static UIResource G_FilterHsvModifyCommand            = new UIResource(Ver.V0_0_0, typeof(G_FilterHsvModifyCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FilterHsvModify);
        public static UIResource G_FilterMonochromeCommand           = new UIResource(Ver.V0_0_0, typeof(G_FilterMonochromeCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FilterMonochrome);
        public static UIResource G_FilterNegativeCommand             = new UIResource(Ver.V0_0_0, typeof(G_FilterNegativeCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FilterNegative);
        public static UIResource G_FilterOnOffCommand                = new UIResource(Ver.V0_0_0, typeof(G_FilterOnOffCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FilterOnOff);
        public static UIResource G_FilterResetCommand                = new UIResource(Ver.V0_0_0, typeof(G_FilterResetCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FilterReset);
        public static UIResource G_FilterReliefCommand               = new UIResource(Ver.V0_0_0, typeof(G_FilterReliefCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FilterRelief);
        public static UIResource G_FilterSettingCommand              = new UIResource(Ver.V0_0_0, typeof(G_FilterSettingCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_G_FilterSetting);
        public static UIResource G_FilterSepiaCommand                = new UIResource(Ver.V0_0_0, typeof(G_FilterSepiaCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FilterSepia);
        public static UIResource G_FilterSharpCommand                = new UIResource(Ver.V0_0_0, typeof(G_FilterSharpCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FilterSharp);
        public static UIResource G_FitImageToScreenCommand           = new UIResource(Ver.V1_3_0, typeof(G_FitImageToScreenCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomFitImage,  false, false, Resources.MenuName_G_FitImageToScreen);
        public static UIResource G_FitShortEdgeToScreenCommand       = new UIResource(Ver.V1_3_0, typeof(G_FitShortEdgeToScreenCommand),       CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomFitEdge,   false, false, Resources.MenuName_G_FitShortEdgeToScreen);
        public static UIResource G_FullScreenCommand                 = new UIResource(Ver.V0_0_0, typeof(G_FullScreenCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_FullScreen,    false, false, Resources.MenuName_G_FullScreen);
        public static UIResource G_FullScreenMultiCommand            = new UIResource(Ver.V0_0_0, typeof(G_FullScreenMultiCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_FullScreenMulti);
        public static UIResource G_InterpolationHighQualityCommand   = new UIResource(Ver.V1_0_0, typeof(G_InterpolationHighQualityCommand),   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_InterpolationHighQuality);
        public static UIResource G_InterpolationNeighborCommand      = new UIResource(Ver.V1_0_0, typeof(G_InterpolationNeighborCommand),      CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_InterpolationNeighborCommand);
        public static UIResource G_MarkFile1Command                  = new UIResource(Ver.V1_0_0, typeof(G_MarkFile1Command),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MarkFile1,     false, false, Resources.MenuName_G_MarkFile1);
        public static UIResource G_MarkFile2Command                  = new UIResource(Ver.V1_0_0, typeof(G_MarkFile2Command),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MarkFile2,     false, false, Resources.MenuName_G_MarkFile2);
        public static UIResource G_MarkFile3Command                  = new UIResource(Ver.V1_0_0, typeof(G_MarkFile3Command),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MarkFile3,     false, false, Resources.MenuName_G_MarkFile3);
        public static UIResource G_MarkFile4Command                  = new UIResource(Ver.V1_0_0, typeof(G_MarkFile4Command),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MarkFile4,     false, false, Resources.MenuName_G_MarkFile4);
        public static UIResource G_MarkFile5Command                  = new UIResource(Ver.V1_0_0, typeof(G_MarkFile5Command),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MarkFile5,     false, false, Resources.MenuName_G_MarkFile5);
        public static UIResource G_MarkFile6Command                  = new UIResource(Ver.V1_0_0, typeof(G_MarkFile6Command),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MarkFile6,     false, false, Resources.MenuName_G_MarkFile6);
        public static UIResource G_MarkFile7Command                  = new UIResource(Ver.V1_0_0, typeof(G_MarkFile7Command),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MarkFile7,     false, false, Resources.MenuName_G_MarkFile7);
        public static UIResource G_MarkFile8Command                  = new UIResource(Ver.V1_0_0, typeof(G_MarkFile8Command),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MarkFile8,     false, false, Resources.MenuName_G_MarkFile8);
        public static UIResource G_MarkFile9Command                  = new UIResource(Ver.V1_0_0, typeof(G_MarkFile9Command),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MarkFile9,     false, false, Resources.MenuName_G_MarkFile9);
        public static UIResource G_MarkFileHelpCommand               = new UIResource(Ver.V1_0_0, typeof(G_MarkFileHelpCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_G_MarkFileHelp);
        public static UIResource G_MarkFileListCommand               = new UIResource(Ver.V1_0_0, typeof(G_MarkFileListCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_G_MarkFileList);
        public static UIResource G_MirrorHorizontalCommand           = new UIResource(Ver.V0_0_0, typeof(G_MirrorHorizontalCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MirrorHorz,    false, false, Resources.MenuName_G_MirrorHorizontal);
        public static UIResource G_MirrorVerticalCommand             = new UIResource(Ver.V0_0_0, typeof(G_MirrorVerticalCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_MirrorVert,    false, false, Resources.MenuName_G_MirrorVertical);
        public static UIResource G_ReturnFileListCommand             = new UIResource(Ver.V1_3_0, typeof(G_ReturnFileListCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_Return,        false, false, Resources.MenuName_G_ReturnFileList);
        public static UIResource G_RotateCCWCommand                  = new UIResource(Ver.V0_0_0, typeof(G_RotateCCWCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_RotateCCW,     false, false, Resources.MenuName_G_RotateCCW);
        public static UIResource G_RotateCWCommand                   = new UIResource(Ver.V0_0_0, typeof(G_RotateCWCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_RotateCW,      false, false, Resources.MenuName_G_RotateCW);
        public static UIResource G_ViewBottomCommand                 = new UIResource(Ver.V0_0_0, typeof(G_ViewBottomCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ViewBottom,    false, false, Resources.MenuName_G_ViewBottom);
        public static UIResource G_ViewLeftCommand                   = new UIResource(Ver.V0_0_0, typeof(G_ViewLeftCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ViewLeft,      false, false, Resources.MenuName_G_ViewLeft);
        public static UIResource G_ViewRightCommand                  = new UIResource(Ver.V0_0_0, typeof(G_ViewRightCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ViewRight,     false, false, Resources.MenuName_G_ViewRight);
        public static UIResource G_ViewCenterCommand                 = new UIResource(Ver.V0_0_0, typeof(G_ViewCenterCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_ViewCenter);
        public static UIResource G_ViewTopCommand                    = new UIResource(Ver.V0_0_0, typeof(G_ViewTopCommand),                    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ViewTop,       false, false, Resources.MenuName_G_ViewTop);
        public static UIResource G_SaveImageAsCommand                = new UIResource(Ver.V1_0_0, typeof(G_SaveImageAsCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_Save,          false, true,  Resources.MenuName_G_SaveTextAsCommand);
        public static UIResource G_ScrollDownCommand                 = new UIResource(Ver.V0_0_0, typeof(G_ScrollDownCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_ScrollDown);
        public static UIResource G_ScrollLeftCommand                 = new UIResource(Ver.V0_0_0, typeof(G_ScrollLeftCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_ScrollLeft);
        public static UIResource G_ScrollRightCommand                = new UIResource(Ver.V0_0_0, typeof(G_ScrollRightCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_ScrollRight);
        public static UIResource G_ScrollUpCommand                   = new UIResource(Ver.V0_0_0, typeof(G_ScrollUpCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_ScrollUp);
        public static UIResource G_SetBackGroundBlackCommand         = new UIResource(Ver.V2_3_1, typeof(G_SetBackGroundBlackCommand),         CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_SetBackGroundBlack);
        public static UIResource G_SetBackGroundWhiteCommand         = new UIResource(Ver.V2_3_1, typeof(G_SetBackGroundWhiteCommand),         CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_SetBackGroundWhite);
        public static UIResource G_SlideShowNextCommand              = new UIResource(Ver.V0_0_0, typeof(G_SlideShowNextCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_SlideShowNext, false, false, Resources.MenuName_G_SlideShowNext);
        public static UIResource G_SlideShowPrevCommand              = new UIResource(Ver.V0_0_0, typeof(G_SlideShowPrevCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_SlideShowPrev, false, false, Resources.MenuName_G_SlideShowPrev);
        public static UIResource G_ToggleBackGroundColorCommand      = new UIResource(Ver.V2_3_1, typeof(G_ToggleBackGroundColorCommand),      CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_ToggleBackGroundColor);
        public static UIResource G_TwoStrokeKey1Command              = new UIResource(Ver.V1_3_0, typeof(G_TwoStrokeKey1Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_TwoStrokeKey1);
        public static UIResource G_TwoStrokeKey2Command              = new UIResource(Ver.V1_3_0, typeof(G_TwoStrokeKey2Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_TwoStrokeKey2);
        public static UIResource G_TwoStrokeKey3Command              = new UIResource(Ver.V1_3_0, typeof(G_TwoStrokeKey3Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_TwoStrokeKey3);
        public static UIResource G_TwoStrokeKey4Command              = new UIResource(Ver.V1_3_0, typeof(G_TwoStrokeKey4Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_TwoStrokeKey4);
        public static UIResource G_ZoomDirectCommand                 = new UIResource(Ver.V0_0_0, typeof(G_ZoomDirectCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_G_ZoomDirect);
        public static UIResource G_ZoomDirect20Command               = new UIResource(Ver.V0_0_0, typeof(G_ZoomDirect20Command),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomDirect20,  false, false, Resources.MenuName_G_ZoomDirect20);
        public static UIResource G_ZoomDirect50Command               = new UIResource(Ver.V0_0_0, typeof(G_ZoomDirect50Command),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomDirect50,  false, false, Resources.MenuName_G_ZoomDirect50);
        public static UIResource G_ZoomDirect100Command              = new UIResource(Ver.V0_0_0, typeof(G_ZoomDirect100Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomDirect100, false, false, Resources.MenuName_G_ZoomDirect100);
        public static UIResource G_ZoomDirect150Command              = new UIResource(Ver.V0_0_0, typeof(G_ZoomDirect150Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomDirect150, false, false, Resources.MenuName_G_ZoomDirect150);
        public static UIResource G_ZoomDirect200Command              = new UIResource(Ver.V0_0_0, typeof(G_ZoomDirect200Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomDirect200, false, false, Resources.MenuName_G_ZoomDirect200);
        public static UIResource G_ZoomDirect500Command              = new UIResource(Ver.V0_0_0, typeof(G_ZoomDirect500Command),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomDirect500, false, false, Resources.MenuName_G_ZoomDirect500);
        public static UIResource G_ZoomInCommand                     = new UIResource(Ver.V0_0_0, typeof(G_ZoomInCommand),                     CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomIn,        false, false, Resources.MenuName_G_ZoomIn);
        public static UIResource G_ZoomInExCommand                   = new UIResource(Ver.V0_0_0, typeof(G_ZoomInExCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomIn,        false, false, Resources.MenuName_G_ZoomInEx);
        public static UIResource G_ZoomOutCommand                    = new UIResource(Ver.V0_0_0, typeof(G_ZoomOutCommand),                    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomOut,       false, false, Resources.MenuName_G_ZoomOut);
        public static UIResource G_ZoomOutExCommand                  = new UIResource(Ver.V0_0_0, typeof(G_ZoomOutExCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.GraphicsViewer_ZoomOut,       false, false, Resources.MenuName_G_ZoomOutEx);

        public static UIResource M_CopyAllAsCommand                  = new UIResource(Ver.V2_0_0, typeof(M_CopyAllAsCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_M_CopyAllAs);
        public static UIResource M_ExitViewCommand                   = new UIResource(Ver.V2_0_0, typeof(M_ExitViewCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_ExitView,    false, false, Resources.MenuName_M_ExitView);
        public static UIResource M_JumpBottomLineCommand             = new UIResource(Ver.V2_0_0, typeof(M_JumpBottomLineCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_JumpBottom,  false, false, Resources.MenuName_M_JumpBottomLine);
        public static UIResource M_JumpTopLineCommand                = new UIResource(Ver.V2_0_0, typeof(M_JumpTopLineCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_JumpTop,     false, false, Resources.MenuName_M_JumpTopLine);
        public static UIResource M_RefreshCommand                    = new UIResource(Ver.V2_0_0, typeof(M_RefreshCommand),                    CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_Refresh,     false, false, Resources.MenuName_M_Refresh);
        public static UIResource M_SearchCommand                     = new UIResource(Ver.V2_0_0, typeof(M_SearchCommand),                     CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_Search,      false, true,  Resources.MenuName_M_Search);
        public static UIResource M_SearchForwardNextCommand          = new UIResource(Ver.V2_0_0, typeof(M_SearchForwardNextCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_SearchFor,   false, false, Resources.MenuName_M_SearchForwardNext);
        public static UIResource M_SearchReverseNextCommand          = new UIResource(Ver.V2_0_0, typeof(M_SearchReverseNextCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_SearchRev,   false, false, Resources.MenuName_M_SearchReverseNext);
        public static UIResource M_SaveAsCommand                     = new UIResource(Ver.V2_0_0, typeof(M_SaveAsCommand),                     CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_Save,        false, true,  Resources.MenuName_M_SaveAs);
        public static UIResource M_PsDetailCommand                   = new UIResource(Ver.V2_0_0, typeof(M_PsDetailCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_M_PsDetail);
        public static UIResource M_PsForceTerminateCommand           = new UIResource(Ver.V2_0_0, typeof(M_PsForceTerminateCommand),           CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_PsForceTerm, false, false, Resources.MenuName_M_PsForceTerminate);
        public static UIResource M_PsKillCommand                     = new UIResource(Ver.V2_0_0, typeof(M_PsKillCommand),                     CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_PsKill,      false, false, Resources.MenuName_M_PsKill);
        public static UIResource M_ReturnFileListCommand             = new UIResource(Ver.V2_0_0, typeof(M_ReturnFileListCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.MonitoringViewer_Return,      false, false, Resources.MenuName_M_ReturnFileList);

        public static UIResource T_ClearBackLogCommand               = new UIResource(Ver.V2_0_0, typeof(T_ClearBackLogCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.Terminal_ClearBackLog,        false, false, Resources.MenuName_T_ClearBackLog);
        public static UIResource T_CopyClipboardCommand              = new UIResource(Ver.V2_0_0, typeof(T_CopyClipboardCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.Terminal_CopyText,            false, false, Resources.MenuName_T_CopyClipboard);
        public static UIResource T_ExitTerminalCommand               = new UIResource(Ver.V2_0_0, typeof(T_ExitTerminalCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.Terminal_Exit,                false, false, Resources.MenuName_T_ExitTerminal);
        public static UIResource T_EditDisplayNameCommand            = new UIResource(Ver.V2_0_0, typeof(T_EditDisplayNameCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, true,  Resources.MenuName_T_EditDisplayName);
        public static UIResource T_NewTerminal                       = new UIResource(Ver.V2_0_0, typeof(T_NewTerminal),                       CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.Terminal_WindowNew,           false, false, Resources.MenuName_T_NewTerminal);
        public static UIResource T_PasteClipboardCommand             = new UIResource(Ver.V2_0_0, typeof(T_PasteClipboardCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.Terminal_PasteText,           false, false, Resources.MenuName_T_PasteClipboard);
        public static UIResource T_ReturnFileListCommand             = new UIResource(Ver.V2_0_0, typeof(T_ReturnFileListCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.Terminal_Return,              false, false, Resources.MenuName_T_ReturnFileList);
        public static UIResource T_SaveAsCommand                     = new UIResource(Ver.V2_0_0, typeof(T_SaveAsCommand),                     CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.Terminal_Save,                false, true,  Resources.MenuName_T_SaveAs);
        public static UIResource T_ScrollRollDownCommand             = new UIResource(Ver.V2_0_0, typeof(T_ScrollRollDownCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_ScrollRollDown);
        public static UIResource T_ScrollRollDownCurrentCommand      = new UIResource(Ver.V2_0_0, typeof(T_ScrollRollDownCurrentCommand),      CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_ScrollRollDownCurrent);
        public static UIResource T_ScrollRollUpCommand               = new UIResource(Ver.V2_0_0, typeof(T_ScrollRollUpCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_ScrollRollUp);
        public static UIResource T_ScrollRollUpCurrentCommand        = new UIResource(Ver.V2_0_0, typeof(T_ScrollRollUpCurrentCommand),        CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_ScrollRollUpCurrent);
        public static UIResource T_ScrollDownCommand                 = new UIResource(Ver.V2_0_0, typeof(T_ScrollDownCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_ScrollDown);
        public static UIResource T_ScrollDownCurrentCommand          = new UIResource(Ver.V2_0_0, typeof(T_ScrollDownCurrentCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_ScrollDownCurrent);
        public static UIResource T_ScrollUpCommand                   = new UIResource(Ver.V2_0_0, typeof(T_ScrollUpCommand),                   CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_ScrollUp);
        public static UIResource T_ScrollUpCurrentCommand            = new UIResource(Ver.V2_0_0, typeof(T_ScrollUpCurrentCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_ScrollUpCurrent);
        public static UIResource T_SelectAllCommand                  = new UIResource(Ver.V2_0_0, typeof(T_SelectAllCommand),                  CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.Terminal_SelectAll,           false, false, Resources.MenuName_T_SelectAllCommand);
        public static UIResource T_SendEscapeCommand                 = new UIResource(Ver.V2_0_0, typeof(T_SendEscapeCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendEscape);
        public static UIResource T_SendCursorDownCommand             = new UIResource(Ver.V2_0_0, typeof(T_SendCursorDownCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendCursorDown);
        public static UIResource T_SendCursorLeftCommand             = new UIResource(Ver.V2_0_0, typeof(T_SendCursorLeftCommand),             CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendCursorLeft);
        public static UIResource T_SendCursorRightCommand            = new UIResource(Ver.V2_0_0, typeof(T_SendCursorRightCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendCursorRight);
        public static UIResource T_SendCursorUpCommand               = new UIResource(Ver.V2_0_0, typeof(T_SendCursorUpCommand),               CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendCursorUp);
        public static UIResource T_SendKeyDeleteCommand              = new UIResource(Ver.V2_0_0, typeof(T_SendKeyDeleteCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendKeyDelete);
        public static UIResource T_SendKeyEndCommand                 = new UIResource(Ver.V2_0_0, typeof(T_SendKeyEndCommand),                 CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendKeyEnd);
        public static UIResource T_SendKeyFunctionCommand            = new UIResource(Ver.V2_0_0, typeof(T_SendKeyFunctionCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendKeyFunction);
        public static UIResource T_SendKeyHomeCommand                = new UIResource(Ver.V2_0_0, typeof(T_SendKeyHomeCommand),                CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendKeyHome);
        public static UIResource T_SendKeyInsertCommand              = new UIResource(Ver.V2_0_0, typeof(T_SendKeyInsertCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendKeyInsert);
        public static UIResource T_SendKeyPageDownCommand            = new UIResource(Ver.V2_0_0, typeof(T_SendKeyPageDownCommand),            CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendKeyPageDown);
        public static UIResource T_SendKeyPageUpCommand              = new UIResource(Ver.V2_0_0, typeof(T_SendKeyPageUpCommand),              CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.None,                         false, false, Resources.MenuName_T_SendKeyPageUp);
        public static UIResource T_ToggleBackLogViewCommand          = new UIResource(Ver.V2_0_0, typeof(T_ToggleBackLogViewCommand),          CommandRetType.Void, UIEnableCondition.Always,   IconImageListID.Terminal_BackLog,             false, false, Resources.MenuName_T_ToggleBackLogView);

        // はじめに組み込まれたバージョン
        private int m_firstVersion;

        // コマンドを実装しているクラス
        private Type m_commandType;

        // 戻り値の型
        private CommandRetType m_commandReturnType;

        // アイコン状態を有効/無効に切り替える条件
        private UIEnableCondition m_enableCondition;

        // アイコンの左ウィンドウ用ID（アイコンを使用しないときNone）
        private IconImageListID m_iconIdLeft;

        // アイコンの右ウィンドウ用ID（アイコンを使用しないときNone）
        private IconImageListID m_iconIdRight;
        
        // 実行するとダイアログを開くときtrue
        private bool m_isOpenDialog;

        // ツールヒントの文字列
        private string m_hint;

        // ツールヒントの文字列（ショート名）
        private string m_hintShort;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]version      はじめに組み込まれたバージョン
        // 　　　　[in]commandType  コマンドを実装しているクラス
        // 　　　　[in]retType      戻り値の型
        // 　　　　[in]enableCond   アイコン状態を有効/無効に切り替える条件
        // 　　　　[in]iconIdLeft   アイコンの左ウィンドウ用ID
        // 　　　　[in]useRightIcon 右アイコンを独自のものにするときtrue、左と同じときfalse
        // 　　　　[in]isOpenDialog 実行するとダイアログを開くときtrue
        // 　　　　[in]hint         ツールヒントの文字列
        // 戻り値：なし
        //=========================================================================================
        public UIResource(int version, Type commandType, CommandRetType retType, UIEnableCondition enableCond, IconImageListID iconIdLeft, bool useRightIcon, bool isOpenDialog, string hint) {
            m_firstVersion = version;
            m_commandType = commandType;
            m_commandReturnType = retType;
            m_enableCondition = enableCond;
            m_iconIdLeft = iconIdLeft;
            if (useRightIcon) {
                m_iconIdRight = (IconImageListID)(iconIdLeft + 1);
            } else {
                m_iconIdRight = iconIdLeft;
            }

            m_isOpenDialog = isOpenDialog;
            string[] hintSplit = hint.Split(',');
            m_hint = hintSplit[0];
            if (hintSplit.Length >= 2) {
                m_hintShort = hintSplit[1];
            } else {
                m_hintShort = hintSplit[0];
            }
            s_commandList.Add(this);
        }

        //=========================================================================================
        // プロパティ：関連付け関連の項目の一覧
        //=========================================================================================
        public static List<UIResource> OpenFileAssociateItems {
            get {
                List<UIResource> result = new List<UIResource>();
                result.Add(UIResource.OpenFileAssociate1Command);
                result.Add(UIResource.OpenFileAssociate2Command);
                result.Add(UIResource.OpenFileAssociate3Command);
                result.Add(UIResource.OpenFileAssociate4Command);
                result.Add(UIResource.OpenFileAssociate5Command);
                result.Add(UIResource.OpenFileAssociate6Command);
                result.Add(UIResource.OpenFileAssociate7Command);
                result.Add(UIResource.OpenFileAssociate8Command);
                return result;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドを実装しているクラス
        //=========================================================================================
        public Type CommandType {
            get {
                return  m_commandType;
            }
        }
        
        //=========================================================================================
        // プロパティ：戻り値の型
        //=========================================================================================
        public CommandRetType CommandReturnType {
            get {
                return  m_commandReturnType;
            }
        }

        //=========================================================================================
        // プロパティ：組み込まれたバージョン
        //=========================================================================================
        public int FirstVersion {
            get {
                return  m_firstVersion;
            }
        }

        //=========================================================================================
        // プロパティ：アイコン状態を有効/無効に切り替える条件
        //=========================================================================================
        public UIEnableCondition UIEnableCondition {
            get {
                return m_enableCondition;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンの左ウィンドウ用ID（アイコンを使用しないときNone）
        //=========================================================================================
        public IconImageListID IconIdLeft {
            get {
                return m_iconIdLeft;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンの右ウィンドウ用ID（アイコンを使用しないときNone）
        //=========================================================================================
        public IconImageListID IconIdRight {
            get {
                return m_iconIdRight;
            }
        }
        
        //=========================================================================================
        // プロパティ：実行するとダイアログを開くときtrue
        //=========================================================================================
        public bool IsOpenDialog {
            get {
                return m_isOpenDialog;
            }
        }

        //=========================================================================================
        // プロパティ：ツールヒントの文字列
        //=========================================================================================
        public string Hint {
            get {
                return m_hint;
            }
        }

        //=========================================================================================
        // プロパティ：ツールヒントの文字列（ショート名）
        //=========================================================================================
        public string HintShort {
            get {
                return m_hintShort;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドの一覧
        //=========================================================================================
        public static List<UIResource> CommandList {
            get {
                return s_commandList;
            }
        }
    }
}
