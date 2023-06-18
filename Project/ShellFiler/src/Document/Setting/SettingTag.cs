using System.Collections.Generic;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：設定情報のファイル内のタグ
    //=========================================================================================
    public class SettingTag {
        // タグ名からタグオブジェクトへのMap
        private static Dictionary<string, SettingTag> s_tagNameToSettingTag = new Dictionary<string, SettingTag>();

        // 不明値
        public static SettingTag None                                               = new SettingTag(null);

        // ヘッダ
        public static readonly SettingTag Header                                    = Add(new SettingTag("ShellFilerSetting"));

        // ブックマーク
        public static readonly SettingTag Bookmark_Bookmark                         = Add(new SettingTag("Bookmark"));
        public static readonly SettingTag Bookmark_BookmarkGroup                    = Add(new SettingTag("Group"));
        public static readonly SettingTag Bookmark_GroupName                        = Add(new SettingTag("GroupName"));
        public static readonly SettingTag Bookmark_BookmarkItem                     = Add(new SettingTag("BookmarkItem"));
        public static readonly SettingTag Bookmark_ItemDisplayName                  = Add(new SettingTag("DisplayName"));
        public static readonly SettingTag Bookmark_ItemDirectory                    = Add(new SettingTag("Directory"));
        public static readonly SettingTag Bookmark_ItemShortCut                     = Add(new SettingTag("Shortcut"));

        // コンフィグ
        public static readonly SettingTag Config_Configuration                      = Add(new SettingTag("Configuration"));
        public static readonly SettingTag Config_TemporaryDirectoryDefault          = Add(new SettingTag("TemporaryDirectoryDefault"));
        public static readonly SettingTag Config_TextEditorCommandLine              = Add(new SettingTag("TextEditorCommandLine"));
        public static readonly SettingTag Config_TextEditorCommandLineSSH           = Add(new SettingTag("TextEditorCommandLineSSH"));
        public static readonly SettingTag Config_TextEditorCommandLineWithLineNumber    = Add(new SettingTag("TextEditorCommandLineWithLineNumber"));
        public static readonly SettingTag Config_TextEditorCommandLineWithLineNumberSSH = Add(new SettingTag("TextEditorCommandLineWithLineNumberSSH"));
        public static readonly SettingTag Config_DiffCommandLine                    = Add(new SettingTag("DiffCommandLine"));
        public static readonly SettingTag Config_DiffDirectoryCommandLine           = Add(new SettingTag("DiffDirectoryCommandLine"));
        public static readonly SettingTag Config_AutoDirectoryUpdateWait            = Add(new SettingTag("AutoDirectoryUpdateWait"));
        public static readonly SettingTag Config_RefreshFileListTabChange           = Add(new SettingTag("RefreshFileListTabChange"));
        public static readonly SettingTag Config_RefreshFileListTabChangeSSH        = Add(new SettingTag("RefreshFileListTabChangeSSH"));
        public static readonly SettingTag Config_InitialDirectoryLeft               = Add(new SettingTag("InitialDirectoryLeft"));
        public static readonly SettingTag Config_InitialDirectoryRight              = Add(new SettingTag("InitialDirectoryRight"));
        public static readonly SettingTag Config_MainWindowRectDefault              = Add(new SettingTag("MainWindowRectDefault"));
        public static readonly SettingTag Config_SplashWindowWait                   = Add(new SettingTag("SplashWindowWait"));
        public static readonly SettingTag Config_DefaultFileListSortModeLeft        = Add(new SettingTag("DefaultFileListSortModeLeft"));
        public static readonly SettingTag Config_DefaultFileListSortModeRight       = Add(new SettingTag("DefaultFileListSortModeRight"));
        public static readonly SettingTag Config_RetrieveFolderSizeCondition        = Add(new SettingTag("RetrieveFolderSizeCondition"));
        public static readonly SettingTag Config_RetrieveFolderSizeKeepLowerDepth   = Add(new SettingTag("RetrieveFolderSizeKeepLowerDepth"));
        public static readonly SettingTag Config_RetrieveFolderSizeKeepLowerCount   = Add(new SettingTag("RetrieveFolderSizeKeepLowerCount"));
        public static readonly SettingTag Config_ListViewScrollMarginLineCount      = Add(new SettingTag("ListViewScrollMarginLineCount"));
        public static readonly SettingTag Config_MouseWheelMaxLines                 = Add(new SettingTag("MouseWheelMaxLines"));
        public static readonly SettingTag Config_FileListDragMaxSpeed               = Add(new SettingTag("FileListDragMaxSpeed"));
        public static readonly SettingTag Config_FileListSeparateExt                = Add(new SettingTag("FileListSeparateExt"));
        public static readonly SettingTag Config_ChdirParentOtherSideMove           = Add(new SettingTag("ChdirParentOtherSideMove"));
        public static readonly SettingTag Config_HideWindowDragDrop                 = Add(new SettingTag("HideWindowDragDrop"));
        public static readonly SettingTag Config_ResumeFolderCursorFile             = Add(new SettingTag("ResumeFolderCursorFile"));
        public static readonly SettingTag Config_FileListCursorOpenFolder           = Add(new SettingTag("FileListCursorOpenFolder"));
        public static readonly SettingTag Config_DefaultViewModeLeft                = Add(new SettingTag("DefaultViewModeLeft"));
        public static readonly SettingTag Config_DefaultViewModeRight               = Add(new SettingTag("DefaultViewModeRight"));
        public static readonly SettingTag Config_FileListViewChangeMode             = Add(new SettingTag("FileListViewChangeMode"));
        public static readonly SettingTag Config_FileListViewModeAutoSetting        = Add(new SettingTag("FileListViewModeAutoSetting"));
        public static readonly SettingTag Config_MaxBackgroundTaskCountDefault      = Add(new SettingTag("MaxBackgroundTaskCountDefault"));
        public static readonly SettingTag Config_MaxBackgroundTaskLimitedCountDefault = Add(new SettingTag("MaxBackgroundTaskCountLimitedDefault"));
        public static readonly SettingTag Config_SameFileOptionDefault              = Add(new SettingTag("SameFileOptionDefault"));
        public static readonly SettingTag Config_DeleteFileOptionDefault            = Add(new SettingTag("DeleteFileOptionDefault"));
        public static readonly SettingTag Config_TransferAttributeSetMode           = Add(new SettingTag("TransferAttributeSetMode"));
        public static readonly SettingTag Config_MarklessCopy                       = Add(new SettingTag("MarklessCopy"));
        public static readonly SettingTag Config_MarklessMove                       = Add(new SettingTag("MarklessMove"));
        public static readonly SettingTag Config_MarklessDelete                     = Add(new SettingTag("MarklessDelete"));
        public static readonly SettingTag Config_MarklessShortcut                   = Add(new SettingTag("MarklessShortcut"));
        public static readonly SettingTag Config_MarklessAttribute                  = Add(new SettingTag("MarklessAttribute"));
        public static readonly SettingTag Config_MarklessPack                       = Add(new SettingTag("MarklessPack"));
        public static readonly SettingTag Config_MarklessUnpack                     = Add(new SettingTag("MarklessUnpack"));
        public static readonly SettingTag Config_MarklessEdit                       = Add(new SettingTag("MarklessEdit"));
        public static readonly SettingTag Config_MarklessFolderSize                 = Add(new SettingTag("MarklessFolderSize"));
        public static readonly SettingTag Config_ClipboardCopyNameAsSettingDefault  = Add(new SettingTag("ClipboardCopyNameAsSettingDefault"));
        public static readonly SettingTag Config_FileCompareSettingDefault          = Add(new SettingTag("FileCompareSettingDefault"));
        public static readonly SettingTag Config_ArchiveSettingDefault              = Add(new SettingTag("ArchiveSettingDefault"));
        public static readonly SettingTag Config_ArchiveExtractPathMode             = Add(new SettingTag("ArchiveExtractPathMode"));
        public static readonly SettingTag Config_IncrementalSearchFromHeadDefault   = Add(new SettingTag("IncrementalSearchFromHeadDefault"));
        public static readonly SettingTag Config_MakeDirectoryMoveCurrentDefault    = Add(new SettingTag("MakeDirectoryMoveCurrentDefault"));
        public static readonly SettingTag Config_MakeDirectoryNewWindowsName        = Add(new SettingTag("MakeDirectoryNewWindowsName"));
        public static readonly SettingTag Config_MakeDirectoryNewSSHName            = Add(new SettingTag("MakeDirectoryNewSSHName"));
        public static readonly SettingTag Config_ShellExecuteReplayModeWindowsDefault = Add(new SettingTag("ShellExecuteReplayModeWindowsDefault"));
        public static readonly SettingTag Config_ShellExecuteReplayModeSSHDefault   = Add(new SettingTag("ShellExecuteReplayModeSSHDefault"));
        public static readonly SettingTag Config_MirrorCopyExceptFiles              = Add(new SettingTag("MirrorCopyExceptFiles"));
        public static readonly SettingTag Config_ShellExecuteReplayModeDefault      = Add(new SettingTag("ShellExecuteReplayModeDefault"));
        public static readonly SettingTag Config_TerminalLogLineCount               = Add(new SettingTag("TerminalLogLineCount"));
        public static readonly SettingTag Config_TerminalShellCommandSSH            = Add(new SettingTag("TerminalShellCommandSSH"));
        public static readonly SettingTag Config_TerminalCloseConfirmMode           = Add(new SettingTag("TerminalCloseConfirmMode"));
        public static readonly SettingTag Config_SshShortcutTypeDefault             = Add(new SettingTag("SshShortcutTypeDefault"));
        public static readonly SettingTag Config_SshFileSystemDefault               = Add(new SettingTag("SshFileSystemDefault"));
        public static readonly SettingTag Config_TerminalLogType                    = Add(new SettingTag("TerminalLogType"));
        public static readonly SettingTag Config_TerminalLogMaxSize                 = Add(new SettingTag("TerminalLogMaxSize"));
        public static readonly SettingTag Config_TerminalLogFileCount               = Add(new SettingTag("TerminalLogFileCount"));
        public static readonly SettingTag Config_TerminalLogOutputFolder            = Add(new SettingTag("TerminalLogOutputFolder"));
        public static readonly SettingTag Config_CommandHistoryMaxCountDefault      = Add(new SettingTag("CommandHistoryMaxCountDefault"));
        public static readonly SettingTag Config_CommandHistorySaveDisk             = Add(new SettingTag("CommandHistorySaveDisk"));
        public static readonly SettingTag Config_PathHistoryMaxCountDefault         = Add(new SettingTag("PathHistoryMaxCountDefault"));
        public static readonly SettingTag Config_PathHistoryWholeMaxCountDefault    = Add(new SettingTag("PathHistoryWholeMaxCountDefault"));
        public static readonly SettingTag Config_PathHistoryWholeSaveDisk           = Add(new SettingTag("PathHistoryWholeSaveDisk"));
        public static readonly SettingTag Config_ViewerSearchHistoryMaxCountDefault = Add(new SettingTag("ViewerSearchHistoryMaxCountDefault"));
        public static readonly SettingTag Config_ViewerSearchHistorySaveDisk        = Add(new SettingTag("ViewerSearchHistorySaveDisk"));
        public static readonly SettingTag Config_TextViewerMaxFileSize              = Add(new SettingTag("TextViewerMaxFileSize"));
        public static readonly SettingTag Config_TextViewerMaxLineCount             = Add(new SettingTag("TextViewerMaxLineCount"));
        public static readonly SettingTag Config_TextViewerClearCompareBufferDefault = Add(new SettingTag("TextViewerClearCompareBufferDefault"));
        public static readonly SettingTag Config_TextViewerIsDisplayLineNumber      = Add(new SettingTag("TextViewerIsDisplayLineNumber"));
        public static readonly SettingTag Config_TextViewerIsDisplayCtrlChar        = Add(new SettingTag("TextViewerIsDisplayCtrlChar"));
        public static readonly SettingTag Config_TextViewerLineBreakDefault         = Add(new SettingTag("TextViewerLineBreakDefault"));
        public static readonly SettingTag Config_TextViewerTab4Extension            = Add(new SettingTag("TextViewerTab4Extension"));
        public static readonly SettingTag Config_TextSearchOptionDefault            = Add(new SettingTag("TextSearchOptionDefault"));
        public static readonly SettingTag Config_TextClipboardSettingDefault        = Add(new SettingTag("TextClipboardSettingDefault"));
        public static readonly SettingTag Config_DumpClipboardSettingDefault        = Add(new SettingTag("DumpClipboardSettingDefault"));
        public static readonly SettingTag Config_GraphicsViewerMaxFileSize          = Add(new SettingTag("GraphicsViewerMaxFileSize"));
        public static readonly SettingTag Config_GraphicsViewerDragInertia          = Add(new SettingTag("GraphicsViewerDragInertia"));
        public static readonly SettingTag Config_GraphicsViewerDragBreaking         = Add(new SettingTag("GraphicsViewerDragBreaking"));
        public static readonly SettingTag Config_GraphicsViewerFullScreenHideTimer  = Add(new SettingTag("GraphicsViewerFullScreenHideTimer"));
        public static readonly SettingTag Config_GraphicsViewerFullScreenAutoHideCursor = Add(new SettingTag("GraphicsViewerFullScreenAutoHideCursor"));
        public static readonly SettingTag Config_GraphicsViewerFullScreenAutoHideInfo   = Add(new SettingTag("GraphicsViewerFullScreenAutoHideInfo"));
        public static readonly SettingTag Config_GraphicsViewerFullScreenHideInfoAlways = Add(new SettingTag("GraphicsViewerFullScreenHideInfoAlways"));
        public static readonly SettingTag Config_GraphicsViewerAutoZoomMode         = Add(new SettingTag("GraphicsViewerAutoZoomMode"));
        public static readonly SettingTag Config_GraphicsViewerZoomInLarger         = Add(new SettingTag("Config_GraphicsViewerZoomInLarger"));
        public static readonly SettingTag Config_GraphicsViewerFilterMode           = Add(new SettingTag("GraphicsViewerFilterMode"));
        public static readonly SettingTag Config_FunctionBarSplitCount              = Add(new SettingTag("FunctionBarSplitCount"));
        public static readonly SettingTag Config_FunctionBarUseOverrayIcon          = Add(new SettingTag("FunctionBarUseOverrayIcon"));
        public static readonly SettingTag Config_LogLineMaxCountDefault             = Add(new SettingTag("LogLineMaxCountDefault"));
        public static readonly SettingTag Config_FileListBackColor                  = Add(new SettingTag("FileListBackColor"));
        public static readonly SettingTag Config_FileListFileTextColor              = Add(new SettingTag("FileListFileTextColor"));
        public static readonly SettingTag Config_FileListReadOnlyColor              = Add(new SettingTag("FileListReadOnlyColor"));
        public static readonly SettingTag Config_FileListHiddenColor                = Add(new SettingTag("FileListHiddenColor"));
        public static readonly SettingTag Config_FileListSystemColor                = Add(new SettingTag("FileListSystemColor"));
        public static readonly SettingTag Config_FileListArchiveColor               = Add(new SettingTag("FileListArchiveColor"));
        public static readonly SettingTag Config_FileListSymlinkColor               = Add(new SettingTag("FileListSymlinkColor"));
        public static readonly SettingTag Config_FileListGrayColor                  = Add(new SettingTag("FileListGrayColor"));
        public static readonly SettingTag Config_FileListMarkColor                  = Add(new SettingTag("FileListMarkColor"));
        public static readonly SettingTag Config_FileListMarkBackColor1             = Add(new SettingTag("FileListMarkBackColor1"));
        public static readonly SettingTag Config_FileListMarkBackColor2             = Add(new SettingTag("FileListMarkBackColor2"));
        public static readonly SettingTag Config_FileListMarkBackBorderColor        = Add(new SettingTag("FileListMarkBackBorderColor"));
        public static readonly SettingTag Config_FileListGrayBackColor              = Add(new SettingTag("FileListGrayBackColor"));
        public static readonly SettingTag Config_FileListMarkGrayColor              = Add(new SettingTag("FileListMarkGrayColor"));
        public static readonly SettingTag Config_FileListMarkGrayBackColor1         = Add(new SettingTag("FileListMarkGrayBackColor1"));
        public static readonly SettingTag Config_FileListMarkGrayBackColor2         = Add(new SettingTag("FileListMarkGrayBackColor2"));
        public static readonly SettingTag Config_FileListMarkGrayBackBorderColor    = Add(new SettingTag("FileListMarkGrayBackBorderColor"));
        public static readonly SettingTag Config_FileListCursorColor                = Add(new SettingTag("FileListCursorColor"));
        public static readonly SettingTag Config_FileListCursorDisableColor         = Add(new SettingTag("FileListCursorDisableColor"));
        public static readonly SettingTag Config_FileListStatusBarSuperUserColor    = Add(new SettingTag("FileListStatusBarSuperUserColor"));
        public static readonly SettingTag Config_FileListThumbnailFrameColor1       = Add(new SettingTag("FileListThumbnailFrameColor1"));
        public static readonly SettingTag Config_FileListThumbnailFrameColor2       = Add(new SettingTag("FileListThumbnailFrameColor2"));
        public static readonly SettingTag Config_DialogErrorBackColor               = Add(new SettingTag("DialogErrorBackColor"));
        public static readonly SettingTag Config_DialogErrorTextColor               = Add(new SettingTag("DialogErrorTextColor"));
        public static readonly SettingTag Config_DialogWarningBackColor             = Add(new SettingTag("DialogWarningBackColor"));
        public static readonly SettingTag Config_DialogWarningTextColor             = Add(new SettingTag("DialogWarningTextColor"));
        public static readonly SettingTag Config_TextViewerErrorBackColor           = Add(new SettingTag("TextViewerErrorBackColor"));
        public static readonly SettingTag Config_TextViewerErrorStatusBackColor     = Add(new SettingTag("TextViewerErrorStatusBackColor"));
        public static readonly SettingTag Config_TextViewerErrorStatusTextColor     = Add(new SettingTag("TextViewerErrorStatusTextColor"));
        public static readonly SettingTag Config_TextViewerInfoStatusBackColor      = Add(new SettingTag("TextViewerInfoStatusBackColor"));
        public static readonly SettingTag Config_TextViewerInfoStatusTextColor      = Add(new SettingTag("TextViewerInfoStatusTextColor"));
        public static readonly SettingTag Config_TextViewerLineNoTextColor          = Add(new SettingTag("TextViewerLineNoTextColor"));
        public static readonly SettingTag Config_TextViewerLineNoBackColor1         = Add(new SettingTag("TextViewerLineNoBackColor1"));
        public static readonly SettingTag Config_TextViewerLineNoBackColor2         = Add(new SettingTag("TextViewerLineNoBackColor2"));
        public static readonly SettingTag Config_TextViewerLineNoSeparatorColor     = Add(new SettingTag("TextViewerLineNoSeparatorColor"));
        public static readonly SettingTag Config_TextViewerOutOfAreaBackColor       = Add(new SettingTag("TextViewerOutOfAreaBackColor"));
        public static readonly SettingTag Config_TextViewerOutOfAreaSeparatorColor  = Add(new SettingTag("TextViewerOutOfAreaSeparatorColor"));
        public static readonly SettingTag Config_TextViewerSearchCursorColor        = Add(new SettingTag("TextViewerSearchCursorColor"));
        public static readonly SettingTag Config_TextViewerControlColor             = Add(new SettingTag("TextViewerControlColor"));
        public static readonly SettingTag Config_TextViewerTextColor                = Add(new SettingTag("TextViewerTextColor"));
        public static readonly SettingTag Config_TextViewerSelectTextColor          = Add(new SettingTag("TextViewerSelectTextColor"));
        public static readonly SettingTag Config_TextViewerSelectTextColor2         = Add(new SettingTag("TextViewerSelectTextColor2"));
        public static readonly SettingTag Config_TextViewerSelectBackColor          = Add(new SettingTag("TextViewerSelectBackColor"));
        public static readonly SettingTag Config_TextViewerSelectBackColor2         = Add(new SettingTag("TextViewerSelectBackColor2"));
        public static readonly SettingTag Config_TextViewerSearchHitBackColor       = Add(new SettingTag("TextViewerSearchHitBackColor"));
        public static readonly SettingTag Config_TextViewerSearchHitTextColor       = Add(new SettingTag("TextViewerSearchHitTextColor"));
        public static readonly SettingTag Config_TextViewerSearchAutoTextColor      = Add(new SettingTag("TextViewerSearchAutoTextColor"));
        public static readonly SettingTag Config_RadarBarBackColor1                 = Add(new SettingTag("RadarBarBackColor1"));
        public static readonly SettingTag Config_RadarBarBackColor2                 = Add(new SettingTag("RadarBarBackColor2"));
        public static readonly SettingTag Config_GraphicsViewerBackColor            = Add(new SettingTag("GraphicsViewerBackColor"));
        public static readonly SettingTag Config_GraphicsViewerTextColor            = Add(new SettingTag("GraphicsViewerTextColor"));
        public static readonly SettingTag Config_GraphicsViewerTextShadowColor      = Add(new SettingTag("GraphicsViewerTextShadowColor"));
        public static readonly SettingTag Config_GraphicsViewerLoadingTextColor     = Add(new SettingTag("GraphicsViewerLoadingTextColor"));
        public static readonly SettingTag Config_GraphicsViewerLoadingTextShadowColor = Add(new SettingTag("GraphicsViewerLoadingTextShadowColor"));
        public static readonly SettingTag Config_StateListTitleBackColor1           = Add(new SettingTag("StateListTitleBackColor1"));
        public static readonly SettingTag Config_StateListTitleBackColor2           = Add(new SettingTag("StateListTitleBackColor2"));
        public static readonly SettingTag Config_LogWindowTextColor                 = Add(new SettingTag("LogWindowTextColor"));
        public static readonly SettingTag Config_LogWindowLinkTextColor             = Add(new SettingTag("LogWindowLinkTextColor"));
        public static readonly SettingTag Config_LogErrorTextColor                  = Add(new SettingTag("LogErrorTextColor"));
        public static readonly SettingTag Config_LogWindowSelectTextColor           = Add(new SettingTag("LogWindowSelectTextColor"));
        public static readonly SettingTag Config_LogStdErrorTextColor               = Add(new SettingTag("LogStdErrorTextColor"));
        public static readonly SettingTag Config_LogWindowBackColor                 = Add(new SettingTag("LogWindowBackColor"));
        public static readonly SettingTag Config_LogWindowSelectBackColor           = Add(new SettingTag("LogWindowSelectBackColor"));
        public static readonly SettingTag Config_LogWindowBackBellColor             = Add(new SettingTag("LogWindowBackBellColor"));
        public static readonly SettingTag Config_LogWindowBackClosedColor           = Add(new SettingTag("LogWindowBackClosedColor"));
        public static readonly SettingTag Config_LogWindowProgressColor1            = Add(new SettingTag("LogWindowProgressColor1"));
        public static readonly SettingTag Config_LogWindowProgressColor2            = Add(new SettingTag("LogWindowProgressColor2"));
        public static readonly SettingTag Config_LogWindowProgressColor3            = Add(new SettingTag("LogWindowProgressColor3"));
        public static readonly SettingTag Config_LogWindowProgressColor4            = Add(new SettingTag("LogWindowProgressColor4"));
        public static readonly SettingTag Config_LogWindowRemainingTimeTextColor1   = Add(new SettingTag("LogWindowRemainingTimeTextColor1"));
        public static readonly SettingTag Config_LogWindowRemainingTimeTextColor2   = Add(new SettingTag("LogWindowRemainingTimeTextColor2"));
        public static readonly SettingTag Config_ListViewFontName                   = Add(new SettingTag("ListViewFontName"));
        public static readonly SettingTag Config_ListViewFontSize                   = Add(new SettingTag("ListViewFontSize"));
        public static readonly SettingTag Config_DefaultFileListViewHeight          = Add(new SettingTag("DefaultFileListViewHeight"));
        public static readonly SettingTag Config_ThumbFileListViewFontName          = Add(new SettingTag("ThumbFileListViewFontName"));
        public static readonly SettingTag Config_ThumbFileListViewFontSize          = Add(new SettingTag("ThumbFileListViewFontSize"));
        public static readonly SettingTag Config_ThumbFileListViewSmallFontSize     = Add(new SettingTag("ThumbFileListViewSmallFontSize"));
        public static readonly SettingTag Config_TextFontName                       = Add(new SettingTag("TextFontName"));
        public static readonly SettingTag Config_TextFontSize                       = Add(new SettingTag("TextFontSize"));
        public static readonly SettingTag Config_TextFileViewerLineHeight           = Add(new SettingTag("TextFileViewerLineHeight"));
        public static readonly SettingTag Config_FunctionBarFontName                = Add(new SettingTag("FunctionBarFontName"));
        public static readonly SettingTag Config_FunctionBarFontSize                = Add(new SettingTag("FunctionBarFontSize"));
        public static readonly SettingTag Config_LogWindowFontName                  = Add(new SettingTag("LogWindowFontName"));
        public static readonly SettingTag Config_LogWindowFixedFontName             = Add(new SettingTag("LogWindowFixedFontName"));
        public static readonly SettingTag Config_LogWindowFontSize                  = Add(new SettingTag("LogWindowFontSize"));
        public static readonly SettingTag Config_LogWindowHeight                    = Add(new SettingTag("LogWindowHeight"));
        public static readonly SettingTag Config_LogWindowTerminalFontSize          = Add(new SettingTag("LogWindowTerminalFontSize"));

        // コンフィグ：FileListSortMode
        public static readonly SettingTag FileListSortMode_FileListSortMode         = Add(new SettingTag("FileListSortMode"));
        public static readonly SettingTag FileListSortMode_SortOrder1               = Add(new SettingTag("SortOrder1"));
        public static readonly SettingTag FileListSortMode_SortOrder2               = Add(new SettingTag("SortOrder2"));
        public static readonly SettingTag FileListSortMode_DirectionReverse1        = Add(new SettingTag("DirectionReverse1"));
        public static readonly SettingTag FileListSortMode_DirectionReverse2        = Add(new SettingTag("DirectionReverse2"));
        public static readonly SettingTag FileListSortMode_TopDirectory             = Add(new SettingTag("TopDirectory"));
        public static readonly SettingTag FileListSortMode_Capital                  = Add(new SettingTag("Capital"));
        public static readonly SettingTag FileListSortMode_IdentifyNumber           = Add(new SettingTag("IdentifyNumber"));

        // コンフィグ：FileListSortMode
        public static readonly SettingTag FileListViewMode_FileListViewMode         = Add(new SettingTag("FileListViewMode"));
        public static readonly SettingTag FileListViewMode_ThumbModeSwitch          = Add(new SettingTag("ThumbModeSwitch"));
        public static readonly SettingTag FileListViewMode_ThumbnailSize            = Add(new SettingTag("ThumbnailSize"));
        public static readonly SettingTag FileListViewMode_ThumbnailName            = Add(new SettingTag("ThumbnailName"));

        // コンフィグ：FileListSortModeAutoSetting
        public static readonly SettingTag FileListViewMode_FileListViewModeAuto     = Add(new SettingTag("FileListViewModeAuto"));
        public static readonly SettingTag FileListViewMode_FileListViewModeFolder   = Add(new SettingTag("FileListViewModeFolder"));
        public static readonly SettingTag FileListViewMode_FileListViewModeSetting  = Add(new SettingTag("FileListViewModeSetting"));
        public static readonly SettingTag FileListViewMode_FileListViewModeItem     = Add(new SettingTag("FileListViewModeItem"));

        // コンフィグ：SameFileOption
        public static readonly SettingTag SameFileOption_SameFileOption             = Add(new SettingTag("SameFileOption"));
        public static readonly SettingTag SameFileOption_SameFileMode               = Add(new SettingTag("SameFileMode"));
        public static readonly SettingTag SameFileOption_AutoUpdateModeWindows      = Add(new SettingTag("AutoUpdateModeWindows"));
        public static readonly SettingTag SameFileOption_AutoUpdateModeSSH          = Add(new SettingTag("AutoUpdateModeSSH"));

        // コンフィグ：DeleteFileOption
        public static readonly SettingTag DeleteFileOption_DeleteFileOption         = Add(new SettingTag("DeleteFileOption"));
        public static readonly SettingTag DeleteFileOption_DeleteDirectoryAll       = Add(new SettingTag("DeleteDirectoryAll"));
        public static readonly SettingTag DeleteFileOption_DeleteSpecialAttrAll     = Add(new SettingTag("DeleteSpecialAttrAll"));

        // コンフィグ：ClipboardCopyNameAs
        public static readonly SettingTag ClipboardCopyNameAs_ClipboardCopyNameAs   = Add(new SettingTag("ClipboardCopyNameAs"));
        public static readonly SettingTag ClipboardCopyNameAs_SeparatorMode         = Add(new SettingTag("SeparatorMode"));
        public static readonly SettingTag ClipboardCopyNameAs_QuotaMode             = Add(new SettingTag("QuotaMode"));
        public static readonly SettingTag ClipboardCopyNameAs_FullPath              = Add(new SettingTag("FullPath"));

        // コンフィグ：FileCompareSetting
        public static readonly SettingTag FileCompareSetting_FileCompareSetting     = Add(new SettingTag("FileCompareSetting"));
        public static readonly SettingTag FileCompareSetting_FileTimeMode           = Add(new SettingTag("FileTimeMode"));
        public static readonly SettingTag FileCompareSetting_FileSizeMode           = Add(new SettingTag("FileSizeMode"));
        public static readonly SettingTag FileCompareSetting_ExceptFolder           = Add(new SettingTag("ExceptFolder"));
        public static readonly SettingTag FileCompareSetting_CheckContents          = Add(new SettingTag("CheckContents"));

        // コンフィグ：ArchiveSetting
        public static readonly SettingTag ArchiveSetting_ArchiveSetting             = Add(new SettingTag("ArchiveSetting"));
        public static readonly SettingTag ArchiveSetting_ExecuteMethod              = Add(new SettingTag("ExecuteMethod"));
        public static readonly SettingTag ArchiveSetting_ArchiveType                = Add(new SettingTag("ArchiveType"));
        public static readonly SettingTag ArchiveSetting_Local7zZipOption           = Add(new SettingTag("Local7zZipOption"));
        public static readonly SettingTag ArchiveSetting_Local7z7zOption            = Add(new SettingTag("Local7z7zOption"));
        public static readonly SettingTag ArchiveSetting_Local7zTarGzOption         = Add(new SettingTag("Local7zTarGzOption"));
        public static readonly SettingTag ArchiveSetting_Local7zTarBz2Option        = Add(new SettingTag("Local7zTarBz2Option"));
        public static readonly SettingTag ArchiveSetting_Local7zTarOption           = Add(new SettingTag("Local7zTarOption"));
        public static readonly SettingTag ArchiveSetting_RemoteZipOption            = Add(new SettingTag("RemoteZipOption"));
        public static readonly SettingTag ArchiveSetting_RemoteTarGzOption          = Add(new SettingTag("RemoteTarGzOption"));
        public static readonly SettingTag ArchiveSetting_RemoteTarBz2Option         = Add(new SettingTag("RemoteTarBz2Option"));
        public static readonly SettingTag ArchiveSetting_RemoteTarOption            = Add(new SettingTag("RemoteTarOption"));

        // コンフィグ：ArchiveSettingLocal7zOption
        public static readonly SettingTag ArchiveSettingLocal7z_ArchiveSettingLocal7z = Add(new SettingTag("ArchiveSettingLocal7z"));
        public static readonly SettingTag ArchiveSettingLocal7z_ModifyTimestamp       = Add(new SettingTag("Local7zModifyTimestamp"));
        public static readonly SettingTag ArchiveSettingLocal7z_CompressionMethod     = Add(new SettingTag("Local7zCompressionMethod"));
        public static readonly SettingTag ArchiveSettingLocal7z_CompressionLevel      = Add(new SettingTag("Local7zCompressionLevel"));
        public static readonly SettingTag ArchiveSettingLocal7z_EncryptionMethod      = Add(new SettingTag("Local7zEncryptionMethod"));

        // コンフィグ：ArchiveSettingRemote
        public static readonly SettingTag ArchiveSettingRemote_ArchiveSettingRemote   = Add(new SettingTag("ArchiveSettingRemote"));
        public static readonly SettingTag ArchiveSettingRemote_ModifyTimestamp        = Add(new SettingTag("RemoteModifyTimestamp"));
        public static readonly SettingTag ArchiveSettingRemote_CompressionLevel       = Add(new SettingTag("RemoteCompressionLevel"));

        // コンフィグ：TextViewerLineBreakSetting
        public static readonly SettingTag TextViewerLineBreak_TextViewerLineBreak     = Add(new SettingTag("TextViewerLineBreak"));
        public static readonly SettingTag TextViewerLineBreak_LineBreakMode           = Add(new SettingTag("LineBreakMode"));
        public static readonly SettingTag TextViewerLineBreak_BreakPixel              = Add(new SettingTag("BreakPixel"));
        public static readonly SettingTag TextViewerLineBreak_BreakCharCount          = Add(new SettingTag("BreakCharCount"));
        public static readonly SettingTag TextViewerLineBreak_DumpLineByteCount       = Add(new SettingTag("DumpLineByteCount"));

        // コンフィグ：TextSearchOption
        public static readonly SettingTag TextSearchOption_TextSearchOption           = Add(new SettingTag("TextSearchOption"));
        public static readonly SettingTag TextSearchOption_SearchMode                 = Add(new SettingTag("SearchMode"));
        public static readonly SettingTag TextSearchOption_SearchWord                 = Add(new SettingTag("SearchWord"));

        // コンフィグ：TextClipboardSetting
        public static readonly SettingTag TextClipboardSetting_TextClipboardSetting   = Add(new SettingTag("TextClipboardSetting"));
        public static readonly SettingTag TextClipboardSetting_LineBreakMode          = Add(new SettingTag("TextLineBreakMode"));
        public static readonly SettingTag TextClipboardSetting_TabMode                = Add(new SettingTag("TabMode"));

        // コンフィグ：DumpClipboardSetting
        public static readonly SettingTag DumpClipboardSetting_DumpClipboardSetting   = Add(new SettingTag("DumpClipboardSetting"));
        public static readonly SettingTag DumpClipboardSetting_DumpMode               = Add(new SettingTag("DumpMode"));
        public static readonly SettingTag DumpClipboardSetting_DumpRadix              = Add(new SettingTag("DumpRadix"));
        public static readonly SettingTag DumpClipboardSetting_DumpWidth              = Add(new SettingTag("DumpWidth"));
        public static readonly SettingTag DumpClipboardSetting_DumpZeroPadding        = Add(new SettingTag("DumpZeroPadding"));
        public static readonly SettingTag DumpClipboardSetting_DumpPrefixString       = Add(new SettingTag("DumpPrefixString"));
        public static readonly SettingTag DumpClipboardSetting_DumpPostfixString      = Add(new SettingTag("DumpPostfixString"));
        public static readonly SettingTag DumpClipboardSetting_DumpSeparator          = Add(new SettingTag("DumpSeparator"));
        public static readonly SettingTag DumpClipboardSetting_DumpLineWidth          = Add(new SettingTag("DumpLineWidth"));
        public static readonly SettingTag DumpClipboardSetting_Base64FoldingWidth     = Add(new SettingTag("Base64FoldingWidth"));

        // コンフィグ：GraphicsViewerFilterSetting
        public static readonly SettingTag GraphicsViewerFilter_GraphicsViewerFilter   = Add(new SettingTag("GraphicsViewerFilter"));
        public static readonly SettingTag GraphicsViewerFilter_UseFilter              = Add(new SettingTag("UseFilter"));
        public static readonly SettingTag GraphicsViewerFilter_FilterList             = Add(new SettingTag("FilterList"));

        public static readonly SettingTag GraphicsViewerFilter_FilterItem             = Add(new SettingTag("FilterItem"));
        public static readonly SettingTag GraphicsViewerFilter_FilterClass            = Add(new SettingTag("FilterClass"));
        public static readonly SettingTag GraphicsViewerFilter_FilterParam            = Add(new SettingTag("FilterParam"));
        public static readonly SettingTag GraphicsViewerFilter_FilterParamFloat       = Add(new SettingTag("FilterParamFloat"));
        public static readonly SettingTag GraphicsViewerFilter_FilterParamInt         = Add(new SettingTag("FilterParamInt"));
        public static readonly SettingTag GraphicsViewerFilter_FilterParamBool        = Add(new SettingTag("FilterParamBool"));

        // 一般設定
        public static readonly SettingTag UserGeneral_UserGeneral                     = Add(new SettingTag("UserGeneral"));
        public static readonly SettingTag UserGeneral_SameFileOption                  = Add(new SettingTag("GeneralSameFileOption"));
        public static readonly SettingTag UserGeneral_DeleteFileOption                = Add(new SettingTag("GeneralDeleteFileOption"));
        public static readonly SettingTag UserGeneral_ClipboardCopyNameAsSetting      = Add(new SettingTag("GeneralClipboardCopyNameAsSetting"));
        public static readonly SettingTag UserGeneral_FileCompareSetting              = Add(new SettingTag("GeneralFileCompareSetting"));
        public static readonly SettingTag UserGeneral_ArchiveSetting                  = Add(new SettingTag("GeneralArchiveSetting"));
        public static readonly SettingTag UserGeneral_IncrementalSearchFromHead       = Add(new SettingTag("GeneralIncrementalSearchFromHead"));
        public static readonly SettingTag UserGeneral_MakeDirectoryMoveCurrent        = Add(new SettingTag("GeneralMakeDirectoryMoveCurrent"));
        public static readonly SettingTag UserGeneral_ShellExecuteRelayModeWindows    = Add(new SettingTag("GeneralShellExecuteRelayModeWindows"));
        public static readonly SettingTag UserGeneral_ShellExecuteRelayModeSSH        = Add(new SettingTag("GeneralShellExecuteRelayModeSSH"));
        public static readonly SettingTag UserGeneral_SshShortcutType                 = Add(new SettingTag("GeneralSshShortcutType"));
        public static readonly SettingTag UserGeneral_TextViewerClearCompareBuffer    = Add(new SettingTag("TextViewerClearCompareBuffer"));
        public static readonly SettingTag UserGeneral_TextViewerLineBreak             = Add(new SettingTag("GeneralTextViewerLineBreak"));
        public static readonly SettingTag UserGeneral_TextSearchOption                = Add(new SettingTag("GeneralTextSearchOption"));
        public static readonly SettingTag UserGeneral_TextClipboardSetting            = Add(new SettingTag("GeneralTextClipboardSetting"));
        public static readonly SettingTag UserGeneral_DumpClipboardSetting            = Add(new SettingTag("GeneralDumpClipboardSetting"));
        public static readonly SettingTag UserGeneral_GraphicsViewerImageFilter       = Add(new SettingTag("GeneralGraphicsViewerImageFilter"));
        public static readonly SettingTag UserGeneral_CombineDefaultFileName          = Add(new SettingTag("CombineDefaultFileName"));

        // InitialSetting
        public static readonly SettingTag InitialSetting_InitialSetting               = Add(new SettingTag("InitialSetting"));
        public static readonly SettingTag InitialSetting_LeftFolder                   = Add(new SettingTag("InitialLeftFolder"));
        public static readonly SettingTag InitialSetting_RightFolder                  = Add(new SettingTag("InitialRightFolder"));
        public static readonly SettingTag InitialSetting_WindowMaximized              = Add(new SettingTag("InitialWindowMaximized"));
        public static readonly SettingTag InitialSetting_WindowRectangle              = Add(new SettingTag("InitialWindowRectangle"));
        public static readonly SettingTag InitialSetting_ViewerMaximized              = Add(new SettingTag("InitialViewerMaximized"));
        public static readonly SettingTag InitialSetting_ViewerRectangle              = Add(new SettingTag("InitialViewerRectangle"));
        public static readonly SettingTag InitialSetting_MonitoringViewerMaximized    = Add(new SettingTag("InitialMonitoringViewerMaximized"));
        public static readonly SettingTag InitialSetting_MonitoringViewerRectangle    = Add(new SettingTag("InitialMonitoringViewerRectangle"));
        public static readonly SettingTag InitialSetting_FileListSortModeLeft         = Add(new SettingTag("InitialFileListSortModeLeft"));
        public static readonly SettingTag InitialSetting_FileListSortModeRight        = Add(new SettingTag("InitialFileListSortModeRight"));
        public static readonly SettingTag InitialSetting_FlieListViewModeLeft         = Add(new SettingTag("InitialSettingFlieListViewModeLeft"));
        public static readonly SettingTag InitialSetting_FlieListViewModeRight        = Add(new SettingTag("InitialSetting_FlieListViewModeRight"));

        // CommandApi
        public static readonly SettingTag Command_CommandSpec                         = Add(new SettingTag("CommandSpec"));
        public static readonly SettingTag Command_FileList                            = Add(new SettingTag("FileList"));
        public static readonly SettingTag Command_FileViewer                          = Add(new SettingTag("FileViewer"));
        public static readonly SettingTag Command_GraphicsViewer                      = Add(new SettingTag("GraphicsViewer"));
        public static readonly SettingTag Command_CommandScene                        = Add(new SettingTag("CommandScene"));
        public static readonly SettingTag Command_CommandGroupList                    = Add(new SettingTag("CommandGroupList"));
        public static readonly SettingTag Command_CommandGroup                        = Add(new SettingTag("CommandGroup"));
        public static readonly SettingTag Command_GroupDisplayName                    = Add(new SettingTag("GroupDisplayName"));
        public static readonly SettingTag Command_PackageName                         = Add(new SettingTag("PackageName"));
        public static readonly SettingTag Command_FunctionList                        = Add(new SettingTag("FunctionList"));
        public static readonly SettingTag Command_CommandApi                          = Add(new SettingTag("CommandApi"));
        public static readonly SettingTag Command_CommandName                         = Add(new SettingTag("CommandName"));
        public static readonly SettingTag Command_Comment                             = Add(new SettingTag("Comment"));
        public static readonly SettingTag Command_ArgumentList                        = Add(new SettingTag("ArgumentList"));
        public static readonly SettingTag Command_CommandArgument                     = Add(new SettingTag("CommandArgument"));
        public static readonly SettingTag Command_ArgumentName                        = Add(new SettingTag("ArgumentName"));
        public static readonly SettingTag Command_ArgumentType                        = Add(new SettingTag("ArgumentType"));
        public static readonly SettingTag Command_ArgumentComment                     = Add(new SettingTag("ArgumentComment"));
        public static readonly SettingTag Command_DefaultValue                        = Add(new SettingTag("DefaultValue"));
        public static readonly SettingTag Command_ValueRange                          = Add(new SettingTag("ValueRange"));

        // キー設定
        public static readonly SettingTag KeySetting_KeySetting                       = Add(new SettingTag("KeySetting"));
        public static readonly SettingTag KeySetting_FileList                         = Add(new SettingTag("KeyFileList"));
        public static readonly SettingTag KeySetting_FileViewer                       = Add(new SettingTag("KeyFileViewer"));
        public static readonly SettingTag KeySetting_GraphicsViewer                   = Add(new SettingTag("KeyGraphicsViewer"));
        public static readonly SettingTag KeySetting_MonitoringViewer                 = Add(new SettingTag("KeyMonitoringViewer"));
        public static readonly SettingTag KeySetting_KeyItemSettingList               = Add(new SettingTag("KeyItemSettingList"));
        public static readonly SettingTag KeySetting_KeyItemSetting                   = Add(new SettingTag("KeyItemSetting"));
        public static readonly SettingTag KeySetting_KeyCode                          = Add(new SettingTag("KeyCode"));
        public static readonly SettingTag KeySetting_KeyIsShift                       = Add(new SettingTag("KeyIsShift"));
        public static readonly SettingTag KeySetting_KeyIsControl                     = Add(new SettingTag("KeyIsControl"));
        public static readonly SettingTag KeySetting_KeyIsAlt                         = Add(new SettingTag("KeyIsAlt"));
        public static readonly SettingTag KeySetting_KeyTwoStrokeType                 = Add(new SettingTag("KeyTwoStrokeType"));
        public static readonly SettingTag KeySetting_CommandDisplayName               = Add(new SettingTag("KeyCommandDisplayName"));
        public static readonly SettingTag KeySetting_CommandClassFullName             = Add(new SettingTag("KeyCommandClassFullName"));
        public static readonly SettingTag KeySetting_CommandOption                    = Add(new SettingTag("KeyCommandOption"));
        public static readonly SettingTag KeySetting_CommandParameterList             = Add(new SettingTag("KeyCommandParameterList"));
        public static readonly SettingTag KeySetting_CommandParameter                 = Add(new SettingTag("KeyCommandParameter"));

        public static readonly SettingTag KeySetting_AssociateSetting                 = Add(new SettingTag("AssociateSetting"));
        public static readonly SettingTag KeySetting_AssocSettingList                 = Add(new SettingTag("AssocSettingList"));
        public static readonly SettingTag KeySetting_AssocSettingListItem             = Add(new SettingTag("AssocSettingListItem"));
        public static readonly SettingTag KeySetting_AssocSettingListIndex            = Add(new SettingTag("AssocSettingListIndex"));
        public static readonly SettingTag KeySetting_AssocKeySetting                  = Add(new SettingTag("AssocKeySetting"));
        public static readonly SettingTag KeySetting_AssocKeyDisplayName              = Add(new SettingTag("AssocKeyDisplayName"));
        public static readonly SettingTag KeySetting_AssocKeyExtList                  = Add(new SettingTag("AssocKeyExtList"));
        public static readonly SettingTag KeySetting_AssocKeyDefaultCommand           = Add(new SettingTag("AssocKeyDefaultCommand"));
        public static readonly SettingTag KeySetting_AssocKeyInfo                     = Add(new SettingTag("AssocKeyInfo"));
        public static readonly SettingTag KeySetting_AssocKeyInfoExt                  = Add(new SettingTag("AssocKeyInfoExt"));
        public static readonly SettingTag KeySetting_AssocKeyInfoFileSystem           = Add(new SettingTag("AssocKeyInfoFileSystem"));
        public static readonly SettingTag KeySetting_AssocKeyInfoCommand              = Add(new SettingTag("AssocKeyInfoCommand"));

        // メニュー設定
        public static readonly SettingTag MenuSetting_MenuSetting                     = Add(new SettingTag("MenuSetting"));
        public static readonly SettingTag MenuSetting_FileList                        = Add(new SettingTag("MenuSettingFileList"));
        public static readonly SettingTag MenuSetting_MenuCustom                      = Add(new SettingTag("MenuSettingMenuCustom"));
        public static readonly SettingTag MenuSetting_MenuCustomRootList              = Add(new SettingTag("MenuSettingMenuCustomRootList"));
        public static readonly SettingTag MenuSetting_MenuCustomDefinedOffList        = Add(new SettingTag("MenuSettingMenuCustomDefinedOffList"));
        public static readonly SettingTag MenuSetting_MenuCustomDefinedOff            = Add(new SettingTag("MenuSettingMenuCustomDefinedOff"));
        public static readonly SettingTag MenuSetting_MenuCustomRoot                  = Add(new SettingTag("MenuSettingMenuCustomRoot"));
        public static readonly SettingTag MenuSetting_MenuCustomSetting               = Add(new SettingTag("MenuSettingMenuCustomSetting"));
        public static readonly SettingTag MenuSetting_MenuCustomDisplayRoot           = Add(new SettingTag("MenuSettingMenuCustomDisplayRoot"));
        public static readonly SettingTag MenuSetting_MenuCustomPrevItemName          = Add(new SettingTag("MenuSettingMenuCustomPrevItemName"));
        public static readonly SettingTag MenuSetting_MenuItemSetting                 = Add(new SettingTag("MenuSettingMenuItemSetting"));
        public static readonly SettingTag MenuSetting_MenuItemCommand                 = Add(new SettingTag("MenuSettingMenuItemCommand"));
        public static readonly SettingTag MenuSetting_MenuItemType                    = Add(new SettingTag("MenuSettingMenuItemType"));
        public static readonly SettingTag MenuSetting_MenuItemName                    = Add(new SettingTag("MenuSettingMenuItemName"));
        public static readonly SettingTag MenuSetting_MenuItemShortcut                = Add(new SettingTag("MenuSettingMenuItemShortcut"));
        public static readonly SettingTag MenuSetting_MenuItemSubMenuList             = Add(new SettingTag("MenuSettingMenuItemSubMenuList"));
        public static readonly SettingTag MenuSetting_MenuItemSubMenu                 = Add(new SettingTag("MenuSettingMenuItemSubMenu"));

        // SSH認証情報
        public static readonly SettingTag SSHAuthentivate_SSHAuthenticate             = Add(new SettingTag("SSHAuthenticate"));
        public static readonly SettingTag SSHAuthentivate_Item                        = Add(new SettingTag("SSHItem"));
        public static readonly SettingTag SSHAuthentivate_DisplayName                 = Add(new SettingTag("SSHDisplayName"));
        public static readonly SettingTag SSHAuthentivate_ServerName                  = Add(new SettingTag("SSHServerName"));
        public static readonly SettingTag SSHAuthentivate_UserName                    = Add(new SettingTag("SSHUserName"));
        public static readonly SettingTag SSHAuthentivate_KeyAuthentication           = Add(new SettingTag("SSHKeyAuthentication"));
        public static readonly SettingTag SSHAuthentivate_Password                    = Add(new SettingTag("SSHPassword"));
        public static readonly SettingTag SSHAuthentivate_PrivateKey                  = Add(new SettingTag("SSHPrivateKey"));
        public static readonly SettingTag SSHAuthentivate_PortNo                      = Add(new SettingTag("SSHPortNo"));
        public static readonly SettingTag SSHAuthentivate_Timeout                     = Add(new SettingTag("SSHTimeout"));
        public static readonly SettingTag SSHAuthentivate_Encoding                    = Add(new SettingTag("SSHEncoding"));
        public static readonly SettingTag SSHAuthentivate_TargetOS                    = Add(new SettingTag("SSHTargetOS"));

        // アーカイブパスワード
        public static readonly SettingTag ArchivePassword_ArchivePassword             = Add(new SettingTag("ArchiveAutoPasswordSetting"));
        public static readonly SettingTag ArchivePassword_Item                        = Add(new SettingTag("ArchiveItem"));
        public static readonly SettingTag ArchivePassword_DisplayName                 = Add(new SettingTag("ArchiveDisplayName"));
        public static readonly SettingTag ArchivePassword_Password                    = Add(new SettingTag("ArchiveAutoPassword"));

        // コマンドヒストリ
        public static readonly SettingTag CommandHistory_CommandHistory               = Add(new SettingTag("CommandHistory"));
        public static readonly SettingTag CommandHistory_Item                         = Add(new SettingTag("CommandHistoryItem"));
        public static readonly SettingTag CommandHistory_Command                      = Add(new SettingTag("CommandHistoryCommand"));

        // ビューアのヒストリ
        public static readonly SettingTag ViewerSearchHistory_ViewerSearchHistory     = Add(new SettingTag("ViewerSearchHistory"));
        public static readonly SettingTag ViewerSearchHistory_SearchTextList          = Add(new SettingTag("ViewerHistorySearchTextList"));
        public static readonly SettingTag ViewerSearchHistory_SearchText              = Add(new SettingTag("ViewerHistoryText"));
        public static readonly SettingTag ViewerSearchHistory_SearchDumpList          = Add(new SettingTag("ViewerHistorySearchDumpList"));
        public static readonly SettingTag ViewerSearchHistory_SearchDump              = Add(new SettingTag("ViewerHistoryDump"));

        // フォルダ履歴
        public static readonly SettingTag FolderHistory_FolderHistory                 = Add(new SettingTag("FolderHistory"));
        public static readonly SettingTag FolderHistory_HistoryItem                   = Add(new SettingTag("FolderHistoryItem"));
        public static readonly SettingTag FolderHistory_HistoryInfo                   = Add(new SettingTag("FolderHistoryInfo"));
        public static readonly SettingTag FolderHistory_FolderName                    = Add(new SettingTag("FolderHistoryFolderName"));
        public static readonly SettingTag FolderHistory_FileName                      = Add(new SettingTag("FolderHistoryFileName"));
        public static readonly SettingTag FolderHistory_FileSystem                    = Add(new SettingTag("FolderHistoryFileSystem"));

        // ファイルフィルター
        public static readonly SettingTag FileFilter_FileFilter                       = Add(new SettingTag("FileFilter"));
        public static readonly SettingTag FileFilter_ClipboardSetting                 = Add(new SettingTag("FileFilterClipboardSetting"));
        public static readonly SettingTag FileFilter_ClipboardItem                    = Add(new SettingTag("FileFilterClipboardItem"));
        public static readonly SettingTag FileFilter_TransferDetailSettingMode        = Add(new SettingTag("FileFilterTransferDetailSettingMode"));
        public static readonly SettingTag FileFilter_TransferDetailSetting            = Add(new SettingTag("FileFilterTransferDetailSetting"));
        public static readonly SettingTag FileFilter_TransferQuickSetting             = Add(new SettingTag("FileFilterTransferQuickSetting"));
        public static readonly SettingTag FileFilter_TransferDefinedSetting           = Add(new SettingTag("FileFilterTransferDefinedSetting"));
        public static readonly SettingTag FileFilter_TransferItem                     = Add(new SettingTag("FileFilterTransferItem"));
        public static readonly SettingTag FileFilter_CurrentSetting                   = Add(new SettingTag("FileFilterCurrentSetting"));
        public static readonly SettingTag FileFilter_QuickSetting                     = Add(new SettingTag("FileFilterQuickSetting"));
        public static readonly SettingTag FileFilter_TransferOtherMode                = Add(new SettingTag("FileFilterTransferOtherMode"));
        public static readonly SettingTag FileFilter_TransferList                     = Add(new SettingTag("FileFilterTransferList"));
        public static readonly SettingTag FileFilter_QuickItem                        = Add(new SettingTag("FileFilterQuickItem"));
        public static readonly SettingTag FileFilter_QuickTargetFileMask              = Add(new SettingTag("FileFilterQuickTargetFileMask"));
        public static readonly SettingTag FileFilter_QuickOtherMode                   = Add(new SettingTag("FileFilterQuickOtherMode"));
        public static readonly SettingTag FileFilter_QuickFilterItem                  = Add(new SettingTag("FileFilterQuickFilterItem"));
        public static readonly SettingTag FileFilter_DefinedItem                      = Add(new SettingTag("FileFilter_DefinedItem"));
        public static readonly SettingTag FileFilter_DefinedTargetFileMask            = Add(new SettingTag("FileFilter_DefinedTargetFileMask"));
        public static readonly SettingTag FileFilter_DefinedOtherMode                 = Add(new SettingTag("FileFilter_DefinedOtherMode"));
        public static readonly SettingTag FileFilter_DefinedSelectedItem              = Add(new SettingTag("FileFilter_DefinedSelectedItem"));
        public static readonly SettingTag FileFilter_FilterList                       = Add(new SettingTag("FileFilterFilterList"));
        public static readonly SettingTag FileFilter_TargetClipboard                  = Add(new SettingTag("FileFilterTargetClipboard"));
        public static readonly SettingTag FileFilter_QuickName                        = Add(new SettingTag("FileFilterQuickName"));
        public static readonly SettingTag FileFilter_FilterItemList                   = Add(new SettingTag("FileFilterFilterItemList"));
        public static readonly SettingTag FileFilter_FilterItem                       = Add(new SettingTag("FileFilterFilterItem"));
        public static readonly SettingTag FileFilter_ClassPath                        = Add(new SettingTag("FileFilterClassPath"));
        public static readonly SettingTag FileFilter_ParamList                        = Add(new SettingTag("FileFilterParamList"));
        public static readonly SettingTag FileFilter_ParamName                        = Add(new SettingTag("FileFilterParamName"));
        public static readonly SettingTag FileFilter_ParamValue                       = Add(new SettingTag("FileFilterParamValue"));
        public static readonly SettingTag FileFilter_UseFilter                        = Add(new SettingTag("FileFilterUseFilter"));
        public static readonly SettingTag FileFilter_FilterName                       = Add(new SettingTag("FileFilterFilterName"));
        public static readonly SettingTag FileFilter_TargetFileMask                   = Add(new SettingTag("FileFilterTargetFileMask"));

        // ファイル転送条件
        public static readonly SettingTag FileCondition                               = Add(new SettingTag("FileCondition"));
        public static readonly SettingTag FileCondition_UserWindows                   = Add(new SettingTag("FileConditionUserWindows"));
        public static readonly SettingTag FileCondition_UserSSH                       = Add(new SettingTag("FileConditionUserSSH"));
        public static readonly SettingTag FileCondition_TransferConditionInfo         = Add(new SettingTag("FileConditionTransferConditionInfo"));
        public static readonly SettingTag FileCondition_MarkConditionsInfo            = Add(new SettingTag("FileConditionMarkConditionsInfo"));
        public static readonly SettingTag FileCondition_FileListFilterInfo            = Add(new SettingTag("FileConditionFileListFilterInfo"));

        public static readonly SettingTag FileCondition_ItemWindowsList               = Add(new SettingTag("FileConditionItemWindowsList"));
        public static readonly SettingTag FileCondition_ItemWindows                   = Add(new SettingTag("FileConditionItemWindows"));
        public static readonly SettingTag FileCondition_ItemSSHList                   = Add(new SettingTag("FileConditionItemSSHList"));
        public static readonly SettingTag FileCondition_ItemSSH                       = Add(new SettingTag("FileConditionItemSSH"));
        public static readonly SettingTag FileCondition_ItemDisplayName               = Add(new SettingTag("FileConditionItemDisplayName"));
        public static readonly SettingTag FileCondition_ItemTarget                    = Add(new SettingTag("FileConditionItemTarget"));
        public static readonly SettingTag FileCondition_ItemFileNameType              = Add(new SettingTag("FileConditionItemFileNameType"));
        public static readonly SettingTag FileCondition_ItemFileName                  = Add(new SettingTag("FileConditionItemFileName"));
        public static readonly SettingTag FileCondition_ItemUpdateTime                = Add(new SettingTag("FileConditionItemUpdateTime"));
        public static readonly SettingTag FileCondition_ItemCreateTime                = Add(new SettingTag("FileConditionItemCreateTime"));
        public static readonly SettingTag FileCondition_ItemAccessTime                = Add(new SettingTag("FileConditionItemAccessTime"));
        public static readonly SettingTag FileCondition_ItemFileSize                  = Add(new SettingTag("FileConditionItemFileSize"));
        public static readonly SettingTag FileCondition_ItemAttributeReadOnly         = Add(new SettingTag("FileConditionItemAttributeReadOnly"));
        public static readonly SettingTag FileCondition_ItemAttributeHidden           = Add(new SettingTag("FileConditionItemAttributeHidden"));
        public static readonly SettingTag FileCondition_ItemAttributeArchive          = Add(new SettingTag("FileConditionItemAttributeArchive"));
        public static readonly SettingTag FileCondition_ItemAttributeSystem           = Add(new SettingTag("FileConditionItemAttributeSystem"));
        public static readonly SettingTag FileCondition_ItemAttributeOwnerRead        = Add(new SettingTag("FileConditionItemAttributeOwnerRead"));
        public static readonly SettingTag FileCondition_ItemAttributeOwnerWrite       = Add(new SettingTag("FileConditionItemAttributeOwnerWrite"));
        public static readonly SettingTag FileCondition_ItemAttributeOwnerExecute     = Add(new SettingTag("FileConditionItemAttributeOwnerExecute"));
        public static readonly SettingTag FileCondition_ItemAttributeGroupRead        = Add(new SettingTag("FileConditionItemAttributeGroupRead"));
        public static readonly SettingTag FileCondition_ItemAttributeGroupWrite       = Add(new SettingTag("FileConditionItemAttributeGroupWrite"));
        public static readonly SettingTag FileCondition_ItemAttributeGroupExecute     = Add(new SettingTag("FileConditionItemAttributeGroupExecute"));
        public static readonly SettingTag FileCondition_ItemAttributeOtherRead        = Add(new SettingTag("FileConditionItemAttributeOtherRead"));
        public static readonly SettingTag FileCondition_ItemAttributeOtherWrite       = Add(new SettingTag("FileConditionItemAttributeOtherWrite"));
        public static readonly SettingTag FileCondition_ItemAttributeOtherExecute     = Add(new SettingTag("FileConditionItemAttributeOtherExecute"));
        public static readonly SettingTag FileCondition_ItemAttributeSymbolicLink     = Add(new SettingTag("FileConditionItemAttributeSymbolicLink"));

        public static readonly SettingTag FileCondition_DateTimeType                  = Add(new SettingTag("FileConditionDateTimeType"));
        public static readonly SettingTag FileCondition_DateTimeDateStart             = Add(new SettingTag("FileConditionDateTimeDateStart"));
        public static readonly SettingTag FileCondition_DateTimeRelativeStart         = Add(new SettingTag("FileConditionDateTimeRelativeStart"));
        public static readonly SettingTag FileCondition_DateTimeTimeStart             = Add(new SettingTag("FileConditionDateTimeTimeStart"));
        public static readonly SettingTag FileCondition_DateTimeIncludeStart          = Add(new SettingTag("FileConditionDateTimeIncludeStart"));
        public static readonly SettingTag FileCondition_DateTimeDateEnd               = Add(new SettingTag("FileConditionDateTimeDateEnd"));
        public static readonly SettingTag FileCondition_DateTimeRelativeEnd           = Add(new SettingTag("FileConditionDateTimeRelativeEnd"));
        public static readonly SettingTag FileCondition_DateTimeTimeEnd               = Add(new SettingTag("FileConditionDateTimeTimeEnd"));
        public static readonly SettingTag FileCondition_DateTimeIncludeEnd            = Add(new SettingTag("FileConditionDateTimeIncludeEnd"));

        public static readonly SettingTag FileCondition_FileSizeType                  = Add(new SettingTag("FileConditionFileSizeType"));
        public static readonly SettingTag FileCondition_FileSizeMinSize               = Add(new SettingTag("FileConditionFileSizeMinSize"));
        public static readonly SettingTag FileCondition_FileSizeIncludeMin            = Add(new SettingTag("FileConditionFileSizeIncludeMin"));
        public static readonly SettingTag FileCondition_FileSizeMaxSize               = Add(new SettingTag("FileConditionFileSizeMaxSize"));
        public static readonly SettingTag FileCondition_FileSizeIncludeMax            = Add(new SettingTag("FileConditionFileSizeIncludeMax"));

        // 一括マーク
        public static readonly SettingTag MarkConditionsDialogInfo                    = Add(new SettingTag("MarkConditionsDialogInfo"));
        public static readonly SettingTag MarkConditionsDialogInfo_DialogInfo         = Add(new SettingTag("MarkConditionsDialogInfoDialogInfo"));
        public static readonly SettingTag MarkConditionsDialogInfo_MarkMode           = Add(new SettingTag("MarkConditionsDialogInfoMarkMode"));

        // ファイル一覧フィルター
        public static readonly SettingTag FileListFilterDialogInfo                    = Add(new SettingTag("FileListFilterDialogInfo"));
        public static readonly SettingTag FileListFilterDialogInfo_ConditionMode      = Add(new SettingTag("FileListFilterDialogInfoConditionMode"));
        public static readonly SettingTag FileListFilterDialogInfo_ConditionList      = Add(new SettingTag("FileListFilterDialogInfoConditionList"));
        public static readonly SettingTag FileListFilterDialogInfo_ConditionListName  = Add(new SettingTag("FileListFilterDialogInfoConditionListName"));
        public static readonly SettingTag FileListFilterDialogInfo_WildCard           = Add(new SettingTag("FileListFilterDialogInfoWildCard"));

        // 転送時の属性コピー
        public static readonly SettingTag AttributeSetMode                            = Add(new SettingTag("AttributeSetMode"));
        public static readonly SettingTag AttributeSetMode_WindowsSetAttributeAll     = Add(new SettingTag("AttributeSetModeWindowsSetAttributeAll"));
        public static readonly SettingTag AttributeSetMode_SshSetAtributeAll          = Add(new SettingTag("AttributeSetModeSshSetAtributeAll"));
        public static readonly SettingTag AttributeSetMode_SshAvoidRecursive          = Add(new SettingTag("AttributeSetModeSshAvoidRecursive"));

        // フォルダサイズの取得モード
        public static readonly SettingTag RetrieveFolderSizeInfo                      = Add(new SettingTag("RetrieveFolderSizeInfo"));
        public static readonly SettingTag RetrieveFolderSizeInfo_Mode                 = Add(new SettingTag("RetrieveFolderSizeInfoMode"));
        public static readonly SettingTag RetrieveFolderSizeInfo_SizeUnit             = Add(new SettingTag("RetrieveFolderSizeInfoSizeUnit"));
        public static readonly SettingTag RetrieveFolderSizeInfo_UseCache             = Add(new SettingTag("RetrieveFolderSizeInfoUseCache"));

        // タグ名
        private string m_tagName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]tagName  タグ名
        // 戻り値：なし
        //=========================================================================================
        private SettingTag(string tagName) {
            m_tagName = tagName;
        }

        //=========================================================================================
        // 機　能：タグをMapに追加する
        // 引　数：[in]obj  追加するタグ
        // 戻り値：objそのもの
        //=========================================================================================
        private static SettingTag Add(SettingTag obj) {
            if (s_tagNameToSettingTag.ContainsKey(obj.TagName)) {
                Program.Abort("{0}が重複しています。", obj.TagName);
            }
            s_tagNameToSettingTag.Add(obj.TagName, obj);
            return obj;
        }

        //=========================================================================================
        // 機　能：タグ名からタグを返す
        // 引　数：[in]tagName  タグ名
        // 戻り値：対応するタグ
        //=========================================================================================
        public static SettingTag FromTagName(string tagName) {
            if (s_tagNameToSettingTag.ContainsKey(tagName)) {
                return s_tagNameToSettingTag[tagName];
            } else {
                return SettingTag.None;
            }
        }

        //=========================================================================================
        // プロパティ：タグ名
        //=========================================================================================
        public string TagName {
            get {
                return m_tagName;
            }
        }
    }
}
