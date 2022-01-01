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
        public static readonly SettingTag Config_AutoDirectoryUpdateWait            = Add(new SettingTag("AutoDirectoryUpdateWait"));
        public static readonly SettingTag Config_RefreshFileListTabChange           = Add(new SettingTag("RefreshFileListTabChange"));
        public static readonly SettingTag Config_RefreshFileListTabChangeSSH        = Add(new SettingTag("RefreshFileListTabChangeSSH"));
        public static readonly SettingTag Config_InitialDirectoryLeft               = Add(new SettingTag("InitialDirectoryLeft"));
        public static readonly SettingTag Config_InitialDirectoryRight              = Add(new SettingTag("InitialDirectoryRight"));
        public static readonly SettingTag Config_MainWindowRectDefault              = Add(new SettingTag("MainWindowRectDefault"));
        public static readonly SettingTag Config_DefaultFileListSortModeLeft        = Add(new SettingTag("DefaultFileListSortModeLeft"));
        public static readonly SettingTag Config_DefaultFileListSortModeRight       = Add(new SettingTag("DefaultFileListSortModeRight"));
        public static readonly SettingTag Config_ListViewScrollMarginLineCount      = Add(new SettingTag("ListViewScrollMarginLineCount"));
        public static readonly SettingTag Config_MouseWheelMaxLines                 = Add(new SettingTag("MouseWheelMaxLines"));
        public static readonly SettingTag Config_FileListDragMaxSpeed               = Add(new SettingTag("FileListDragMaxSpeed"));
        public static readonly SettingTag Config_FileListSeparateExt                = Add(new SettingTag("FileListSeparateExt"));
        public static readonly SettingTag Config_ChdirParentOtherSideMove           = Add(new SettingTag("ChdirParentOtherSideMove"));
        public static readonly SettingTag Config_HideWindowDragDrop                 = Add(new SettingTag("HideWindowDragDrop"));
        public static readonly SettingTag Config_MaxBackgroundTaskCountDefault      = Add(new SettingTag("MaxBackgroundTaskCountDefault"));
        public static readonly SettingTag Config_SameFileOptionDefault              = Add(new SettingTag("SameFileOptionDefault"));
        public static readonly SettingTag Config_DeleteFileOptionDefault            = Add(new SettingTag("DeleteFileOptionDefault"));
        public static readonly SettingTag Config_ClipboardCopyNameAsSettingDefault  = Add(new SettingTag("ClipboardCopyNameAsSettingDefault"));
        public static readonly SettingTag Config_FileCompareSettingDefault          = Add(new SettingTag("FileCompareSettingDefault"));
        public static readonly SettingTag Config_ArchiveSettingDefault              = Add(new SettingTag("ArchiveSettingDefault"));
        public static readonly SettingTag Config_IncrementalSearchFromHeadDefault   = Add(new SettingTag("IncrementalSearchFromHeadDefault"));
        public static readonly SettingTag Config_MakeDirectoryMoveCurrentDefault    = Add(new SettingTag("MakeDirectoryMoveCurrentDefault"));
        public static readonly SettingTag Config_MakeDirectoryNewWindowsName        = Add(new SettingTag("MakeDirectoryNewWindowsName"));
        public static readonly SettingTag Config_MakeDirectoryNewSSHName            = Add(new SettingTag("MakeDirectoryNewSSHName"));
        public static readonly SettingTag Config_ShellExecuteReplayModeDefault      = Add(new SettingTag("ShellExecuteReplayModeDefault"));
        public static readonly SettingTag Config_SshShortcutTypeDefault             = Add(new SettingTag("SshShortcutTypeDefault"));
        public static readonly SettingTag Config_CommandHistoryMaxCountDefault      = Add(new SettingTag("CommandHistoryMaxCountDefault"));
        public static readonly SettingTag Config_CommandHistorySaveDisk             = Add(new SettingTag("CommandHistorySaveDisk"));
        public static readonly SettingTag Config_PathHistoryMaxCountDefault         = Add(new SettingTag("PathHistoryMaxCountDefault"));
        public static readonly SettingTag Config_ViewerSearchHistoryMaxCountDefault = Add(new SettingTag("ViewerSearchHistoryMaxCountDefault"));
        public static readonly SettingTag Config_ViewerSearchHistorySaveDisk        = Add(new SettingTag("ViewerSearchHistorySaveDisk"));
        public static readonly SettingTag Config_TextViewerMaxFileSize              = Add(new SettingTag("TextViewerMaxFileSize"));
        public static readonly SettingTag Config_TextViewerMaxLineCount             = Add(new SettingTag("TextViewerMaxLineCount"));
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
        public static readonly SettingTag Config_FunctionBarSplitCount              = Add(new SettingTag("FunctionBarSplitCount"));
        public static readonly SettingTag Config_FunctionBarUseOverrayIcon          = Add(new SettingTag("FunctionBarUseOverrayIcon"));
        public static readonly SettingTag Config_LogLineMaxCountDefault             = Add(new SettingTag("LogLineMaxCountDefault"));
        public static readonly SettingTag Config_FileListBackColor                  = Add(new SettingTag("FileListBackColor"));
        public static readonly SettingTag Config_FileListFileTextColor              = Add(new SettingTag("FileListFileTextColor"));
        public static readonly SettingTag Config_FileListReadOnlyColor              = Add(new SettingTag("FileListReadOnlyColor"));
        public static readonly SettingTag Config_FileListHiddenColor                = Add(new SettingTag("FileListHiddenColor"));
        public static readonly SettingTag Config_FileListSystemColor                = Add(new SettingTag("FileListSystemColor"));
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
        public static readonly SettingTag Config_StateListTitleBackColor1           = Add(new SettingTag("StateListTitleBackColor1"));
        public static readonly SettingTag Config_StateListTitleBackColor2           = Add(new SettingTag("StateListTitleBackColor2"));
        public static readonly SettingTag Config_LogWindowTextColor                 = Add(new SettingTag("LogWindowTextColor"));
        public static readonly SettingTag Config_LogWindowLinkTextColor             = Add(new SettingTag("LogWindowLinkTextColor"));
        public static readonly SettingTag Config_LogErrorTextColor                  = Add(new SettingTag("LogErrorTextColor"));
        public static readonly SettingTag Config_LogStdErrorTextColor               = Add(new SettingTag("LogStdErrorTextColor"));
        public static readonly SettingTag Config_LogWindowBackColor                 = Add(new SettingTag("LogWindowBackColor"));
        public static readonly SettingTag Config_LogWindowProgressColor1            = Add(new SettingTag("LogWindowProgressColor1"));
        public static readonly SettingTag Config_LogWindowProgressColor2            = Add(new SettingTag("LogWindowProgressColor2"));
        public static readonly SettingTag Config_LogWindowProgressColor3            = Add(new SettingTag("LogWindowProgressColor3"));
        public static readonly SettingTag Config_LogWindowProgressColor4            = Add(new SettingTag("LogWindowProgressColor4"));
        public static readonly SettingTag Config_ListViewFontName                   = Add(new SettingTag("ListViewFontName"));
        public static readonly SettingTag Config_ListViewFontSize                   = Add(new SettingTag("ListViewFontSize"));
        public static readonly SettingTag Config_DefaultFileListViewHeight          = Add(new SettingTag("DefaultFileListViewHeight"));
        public static readonly SettingTag Config_TextFontName                       = Add(new SettingTag("TextFontName"));
        public static readonly SettingTag Config_TextFontSize                       = Add(new SettingTag("TextFontSize"));
        public static readonly SettingTag Config_TextFileViewerLineHeight           = Add(new SettingTag("TextFileViewerLineHeight"));
        public static readonly SettingTag Config_FunctionBarFontName                = Add(new SettingTag("FunctionBarFontName"));
        public static readonly SettingTag Config_FunctionBarFontSize                = Add(new SettingTag("FunctionBarFontSize"));
        public static readonly SettingTag Config_LogWindowFontName                  = Add(new SettingTag("LogWindowFontName"));
        public static readonly SettingTag Config_LogWindowFontSize                  = Add(new SettingTag("LogWindowFontSize"));
        public static readonly SettingTag Config_LogWindowHeight                    = Add(new SettingTag("LogWindowHeight"));


        // コンフィグ：FileListSortMode
        public static readonly SettingTag FileListSortMode_FileListSortMode         = Add(new SettingTag("FileListSortMode"));
        public static readonly SettingTag FileListSortMode_SortOrder1               = Add(new SettingTag("SortOrder1"));
        public static readonly SettingTag FileListSortMode_SortOrder2               = Add(new SettingTag("SortOrder2"));
        public static readonly SettingTag FileListSortMode_DirectionReverse1        = Add(new SettingTag("DirectionReverse1"));
        public static readonly SettingTag FileListSortMode_DirectionReverse2        = Add(new SettingTag("DirectionReverse2"));
        public static readonly SettingTag FileListSortMode_TopDirectory             = Add(new SettingTag("TopDirectory"));
        public static readonly SettingTag FileListSortMode_Capital                  = Add(new SettingTag("Capital"));
        public static readonly SettingTag FileListSortMode_IdentifyNumber           = Add(new SettingTag("IdentifyNumber"));

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
        public static readonly SettingTag UserGeneral_ShellExecuteRelayMode           = Add(new SettingTag("GeneralShellExecuteRelayMode"));
        public static readonly SettingTag UserGeneral_SshShortcutType                 = Add(new SettingTag("GeneralSshShortcutType"));
        public static readonly SettingTag UserGeneral_TextViewerLineBreak             = Add(new SettingTag("GeneralTextViewerLineBreak"));
        public static readonly SettingTag UserGeneral_TextSearchOption                = Add(new SettingTag("GeneralTextSearchOption"));
        public static readonly SettingTag UserGeneral_TextClipboardSetting            = Add(new SettingTag("GeneralTextClipboardSetting"));
        public static readonly SettingTag UserGeneral_DumpClipboardSetting            = Add(new SettingTag("GeneralDumpClipboardSetting"));
        public static readonly SettingTag UserGeneral_GraphicsViewerImageFilter       = Add(new SettingTag("GeneralGraphicsViewerImageFilter"));

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
