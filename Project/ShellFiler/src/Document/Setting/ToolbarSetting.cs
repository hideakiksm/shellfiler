using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.FileOperationEtc;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.MoveCursor;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.Command.FileList.Tools;
using ShellFiler.Command.FileList.Window;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.Command.GraphicsViewer;
using ShellFiler.Command.GraphicsViewer.Edit;
using ShellFiler.Command.GraphicsViewer.File;
using ShellFiler.Command.GraphicsViewer.Filter;
using ShellFiler.Command.GraphicsViewer.View;
using ShellFiler.Command.MonitoringViewer;
using ShellFiler.Command.MonitoringViewer.File;
using ShellFiler.Command.MonitoringViewer.Edit;
using ShellFiler.Command.MonitoringViewer.ExecutePs;
using ShellFiler.Command.Terminal.Console;
using ShellFiler.Command.Terminal.File;
using ShellFiler.Command.Terminal.Edit;
using ShellFiler.Command.Terminal.View;
using ShellFiler.MonitoringViewer;
using ShellFiler.UI.Dialog.KeyOption;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ツールバーのカスタマイズ情報
    //=========================================================================================
    class ToolbarSetting {
        // メインツールバー項目の定義
        private List<ToolbarItemSetting> m_mainToolbarItem = new List<ToolbarItemSetting>();

        // アドレスバー項目の定義
        private List<ToolbarItemSetting> m_addressToolbarItem = new List<ToolbarItemSetting>();

        // ファイルビューアのツールバー項目の定義
        private List<ToolbarItemSetting> m_fileViewerToolbarItem = new List<ToolbarItemSetting>();

        // グラフィックビューアのツールバー項目の定義
        private List<ToolbarItemSetting> m_graphicsViewerToolbarItem = new List<ToolbarItemSetting>();

        // ターミナルのツールバー項目の定義
        private List<ToolbarItemSetting> m_terminalToolbarItem = new List<ToolbarItemSetting>();

        // モニタリングビューアのツールバー項目の定義
        private List<ToolbarItemSetting> m_monitoringViewerToolbarItem = new List<ToolbarItemSetting>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ToolbarSetting() {
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
            // ファイル一覧
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CopyCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MoveCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CreateShortcutCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(DeleteCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(RenameCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(RenameSelectedFileInfoCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MakeFolderCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SortMenuCommand)),null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(RefreshFileListCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirParentCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirBookmarkFolderCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(LocalEditCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ArchiveCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ExtractCommand)), null, null));
            m_mainToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_mainToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.DriveList));

            // アドレスバー
            {
                ToolbarItemSetting item = new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ReverseAllMarkFileCommand)), null, null);
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClearAllMarkCommand)),         'C', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClearAllMarkFileCommand)),     'I', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClearAllMarkFolderCommand)),   'O', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SelectAllMarkCommand)),        'A', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SelectAllMarkFileCommand)),    'L', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SelectAllMarkFolderCommand)),  'E', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ReverseAllMarkCommand)),       'R', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ReverseAllMarkFileCommand)),   'F', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ReverseAllMarkFolderCommand)), 'D', null));
                item.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MarkWithConditionsCommand)),   'M', null));
                m_addressToolbarItem.Add(item);
            }
            {
                ToolbarItemSetting item = new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeSettingCommand)), null, null);
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeDetailCommand)),       'D', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnail32Command)),  '1', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnail48Command)),  '2', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnail64Command)),  '3', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnail128Command)), '4', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnail256Command)), '5', null));
                item.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnailCommand)),    'T', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeToggleCommand)),       'G', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeLargerCommand)),       'L', null));
                item.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeSmallerCommand)),      'M', null));
                m_addressToolbarItem.Add(item);
            }
            m_addressToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(PathHistoryPrevCommand)), null, null));
            m_addressToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(PathHistoryNextCommand)), null, null));

            // ファイルビューア
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ExitViewCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SaveTextAsCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_EditFileCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CopyTextCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectAllCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ChangeTextModeCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ChangeDumpModeCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_FullScreenCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpTopLineCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpBottomLineCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), -1), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchForwardNextCommand)), null, null));
            m_fileViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchReverseNextCommand)), null, null));

            // グラフィックビューア
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ExitViewCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SaveImageAsCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_CopyImageCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomInCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomOutCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FitImageToScreenCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FitShortEdgeToScreenCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FullScreenCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MirrorVerticalCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MirrorHorizontalCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_RotateCWCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_RotateCCWCommand)), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SlideShowNextCommand), true), null, null));
            m_graphicsViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SlideShowPrevCommand), true), null, null));

            // モニタリングビューア
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_ExitViewCommand)), null, null));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SaveAsCommand)), null, null));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_JumpTopLineCommand)), null, null));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_JumpBottomLineCommand)), null, null));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_RefreshCommand)), null, null));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SearchCommand)), null, null));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SearchForwardNextCommand)), null, null));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SearchReverseNextCommand)), null, null));
            m_monitoringViewerToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_monitoringViewerToolbarItem.Add(new MonitoringViewerToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_PsKillCommand)), null, null, MonitoringViewerMode.PS));
            m_monitoringViewerToolbarItem.Add(new MonitoringViewerToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_PsForceTerminateCommand)), null, null, MonitoringViewerMode.PS));

            // ターミナル
            m_terminalToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ExitTerminalCommand)), null, null));
            m_terminalToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SaveAsCommand)), null, null));
            m_terminalToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_terminalToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_CopyClipboardCommand)), null, null));
            m_terminalToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_PasteClipboardCommand)), null, null));
            m_terminalToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SelectAllCommand)), null, null));
            m_terminalToolbarItem.Add(new ToolbarItemSetting(ToolbarItemSetting.ItemType.Separator));
            m_terminalToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ToggleBackLogViewCommand)), null, null));
            m_terminalToolbarItem.Add(new ToolbarItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ClearBackLogCommand)), null, null));
        }

        //=========================================================================================
        // 機　能：ツールバー設定の一覧を取得する
        // 引　数：[in]commadScene   コマンドの利用シーン
        // 戻り値：ツールバー設定の一覧
        //=========================================================================================
        public List<ToolbarItemSetting> GetItemList(CommandUsingSceneType commandScene) {
            if (commandScene == CommandUsingSceneType.FileList) {
                return m_mainToolbarItem;
            } else if (commandScene == CommandUsingSceneType.FileViewer) {
                return m_fileViewerToolbarItem;
            } else if (commandScene == CommandUsingSceneType.GraphicsViewer) {
                return m_graphicsViewerToolbarItem;
            } else if (commandScene == CommandUsingSceneType.MonitoringViewer) {
                return m_monitoringViewerToolbarItem;
            } else if (commandScene == CommandUsingSceneType.Terminal) {
                return m_terminalToolbarItem;
            } else {
                Program.Abort("未定義のキー利用シーンです。");
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：メインツールバー項目の定義
        //=========================================================================================
        public List<ToolbarItemSetting> MainToolbarItemList {
            get {
                return m_mainToolbarItem;
            }
        }

        //=========================================================================================
        // プロパティ：アドレスバー項目の定義
        //=========================================================================================
        public List<ToolbarItemSetting> AddressBarItemList {
            get {
                return m_addressToolbarItem;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルビューアのツールバー項目の定義
        //=========================================================================================
        public List<ToolbarItemSetting> FileViewerToolbarItemList {
            get {
                return m_fileViewerToolbarItem;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアのツールバー項目の定義
        //=========================================================================================
        public List<ToolbarItemSetting> GraphicsViewerToolbarItemList {
            get {
                return m_graphicsViewerToolbarItem;
            }
        }

        //=========================================================================================
        // プロパティ：モニタリングビューアのツールバー項目の定義
        //=========================================================================================
        public List<ToolbarItemSetting> MonitoringViewerToolbarItemList {
            get {
                return m_monitoringViewerToolbarItem;
            }
        }

        //=========================================================================================
        // プロパティ：ターミナルのツールバー項目の定義
        //=========================================================================================
        public List<ToolbarItemSetting> TerminalToolbarItemList {
            get {
                return m_terminalToolbarItem;
            }
        }
    }
}
