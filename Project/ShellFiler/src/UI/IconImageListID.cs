﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using ShellFiler.FileTask;

namespace ShellFiler.UI {

    //=========================================================================================
    // 列挙子：UI用アイコンのイメージリストのID
    //=========================================================================================
    public enum IconImageListID {
        None = 0,
        FileList_Copy,
        FileList_Copy_,
        FileList_Move,
        FileList_Move_,
        FileList_Shortcut,
        FileList_Shortcut_,
        FileList_Delete,
        FileList_MirrorCopy,
        FileList_MirrorCopy_,
        FileList_Rename,
        FileList_RenameSelected,
        FileList_MakeDirectory,
        FileList_SortMenu,
        FileList_SortClear,
        FileList_RetrieveFolderSize,
        FileList_DuplicateFile,
        FileList_Refresh,
        FileList_LocalEdit,
        FileList_FileViewer,
        FileList_GraphicsViewer,
        FileList_SlideShow,
        FileList_ContextMenu,
        FileList_Archive,
        FileList_Extract,
        FileList_Filter,
        FileList_FilterClear,
        FileList_FilterMini,
        FileList_ViewModeSetting,
        FileList_ViewModeDetail,
        FileList_ViewMode32,
        FileList_ViewMode48,
        FileList_ViewMode64,
        FileList_ViewMode128,
        FileList_ViewMode256,
        FileList_ReverseMarkFile,
        FileList_ClearMarkAll,
        FileList_ChdirBookmarkFolder,
        FileList_ChdirParent,
        FileList_ChdirSpecial,
        FileList_ChdirSSHFolder,
        FileList_PathHistoryPrev,
        FileList_PathHistoryNext,
        FileList_Exit,
        FileList_ShellCommand,
        FileList_PowerShell,
        FileList_ShellExecute,
        FileList_DiffFolder,
        FileList_DiffMark,
        FileList_DiffOpposite,
        FileList_CombineFile,
        FileList_CombineFile_,
        FileList_SplitFile,
        FileList_SplitFile_,
        FileList_OptionSetting,
        FileList_KeyListMenu,
        FileList_SSHChangeUser,
        FileViewer_ExitView,
        FileViewer_CopyText,
        FileViewer_JumpBottomLine,
        FileViewer_JumpTopLine,
        FileViewer_JumpDirect,
        FileViewer_FullScreen,
        FileViewer_SelectAll,
        FileViewer_Save,
        FileViewer_Return,
        FileViewer_ChangeText,
        FileViewer_ChangeDump,
        FileViewer_EditFile,
        FileViewer_Search,
        FileViewer_SearchForwardNext,
        FileViewer_SearchReverseNext,
        FileViewer_SearchGeneric,
        GraphicsViewer_ExitView,
        GraphicsViewer_CopyImage,
        GraphicsViewer_FullScreen,
        GraphicsViewer_SlideShowNext,
        GraphicsViewer_SlideShowPrev,
        GraphicsViewer_ZoomIn,
        GraphicsViewer_ZoomOut,
        GraphicsViewer_Save,
        GraphicsViewer_Return,
        GraphicsViewer_ViewTop,
        GraphicsViewer_ViewBottom,
        GraphicsViewer_ViewLeft,
        GraphicsViewer_ViewRight,
        GraphicsViewer_MarkFile1,
        GraphicsViewer_MarkFile2,
        GraphicsViewer_MarkFile3,
        GraphicsViewer_MarkFile4,
        GraphicsViewer_MarkFile5,
        GraphicsViewer_MarkFile6,
        GraphicsViewer_MarkFile7,
        GraphicsViewer_MarkFile8,
        GraphicsViewer_MarkFile9,
        GraphicsViewer_MirrorHorz,
        GraphicsViewer_MirrorVert,
        GraphicsViewer_RotateCCW,
        GraphicsViewer_RotateCW,
        GraphicsViewer_ZoomDirect20,
        GraphicsViewer_ZoomDirect50,
        GraphicsViewer_ZoomDirect100,
        GraphicsViewer_ZoomDirect150,
        GraphicsViewer_ZoomDirect200,
        GraphicsViewer_ZoomDirect500,
        GraphicsViewer_ZoomFitImage,
        GraphicsViewer_ZoomFitEdge,
        MonitoringViewer_ExitView,
        MonitoringViewer_Save,
        MonitoringViewer_Return,
        MonitoringViewer_Search,
        MonitoringViewer_SearchFor,
        MonitoringViewer_SearchRev,
        MonitoringViewer_JumpTop,
        MonitoringViewer_JumpBottom,
        MonitoringViewer_Refresh,
        MonitoringViewer_PsMyProcess,
        MonitoringViewer_PsOtherProcess,
        MonitoringViewer_PsKill,
        MonitoringViewer_PsForceTerm,
        MonitoringViewer_NetTcp,
        MonitoringViewer_NetUdp,
        Terminal_Exit,
        Terminal_Save,
        Terminal_Return,
        Terminal_CopyText,
        Terminal_PasteText,
        Terminal_SelectAll,
        Terminal_BackLog,
        Terminal_ClearBackLog,
        Terminal_Window,
        Terminal_WindowNew,
        Icon_DriveCD,
        Icon_DriveHDD,
        Icon_DriveNet,
        Icon_DriveRam,
        Icon_DriveRemovable,
        Icon_DriveNameA,
        Icon_DriveNameB,
        Icon_DriveNameC,
        Icon_DriveNameD,
        Icon_DriveNameE,
        Icon_DriveNameF,
        Icon_DriveNameG,
        Icon_DriveNameH,
        Icon_DriveNameI,
        Icon_DriveNameJ,
        Icon_DriveNameK,
        Icon_DriveNameL,
        Icon_DriveNameM,
        Icon_DriveNameN,
        Icon_DriveNameO,
        Icon_DriveNameP,
        Icon_DriveNameQ,
        Icon_DriveNameR,
        Icon_DriveNameS,
        Icon_DriveNameT,
        Icon_DriveNameU,
        Icon_DriveNameV,
        Icon_DriveNameW,
        Icon_DriveNameX,
        Icon_DriveNameY,
        Icon_DriveNameZ,
        Icon_Func01,
        Icon_Func02,
        Icon_Func03,
        Icon_Func04,
        Icon_Func05,
        Icon_Func06,
        Icon_Func07,
        Icon_Func08,
        Icon_Func09,
        Icon_Func10,
        Icon_Func11,
        Icon_Func12,
        Icon_TaskWait,
        Image_Error,
        Icon_BookmarkGroup,
        Icon_BookmarkItem,
        Icon_ArchivePassword,
        Icon_BackgroundTaskFinish,
        Icon_BackgroundTaskSSH,
        Icon_FileAttributeDetailNo,
        Icon_FileAttributeDetailYes,
        Icon_KeySettingApi,
        Icon_KeySettingApiGroup,
        Icon_KeySettingApiNone,
        Icon_KeySettingCategory,
        Icon_KeySettingKey,
        Icon_KeySettingKeyGroup,
        Icon_TerminalNotAssigned,
        Icon_TerminalAssigned,
        Icon_TerminalNew,
        Icon_OperationFailed,
        StatusWait01,
        StatusWait02,
        StatusWait03,
        StatusWait04,
        StatusWait05,
        StatusWait06,
        StatusWait07,
        StatusWait08,
        StatusWait09,
        StatusWait10,
        StatusWait11,
        StatusWait12,
        ButtonRenameTimeSame1,
        ButtonRenameTimeSame2,
        ButtonRenameTimeSame3,
        ButtonRenameTimeSame4,
        ButtonRenameTimeSame5,
    }
}
