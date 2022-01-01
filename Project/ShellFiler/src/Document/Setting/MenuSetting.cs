using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.Open;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.FileOperationEtc;
using ShellFiler.Command.FileList.MoveCursor;
using ShellFiler.Command.FileList.Mouse;
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
using ShellFiler.Command.Terminal.Console;
using ShellFiler.Command.Terminal.File;
using ShellFiler.Command.Terminal.Edit;
using ShellFiler.Command.Terminal.View;
using ShellFiler.MonitoringViewer;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：メニューのカスタマイズ情報
    //=========================================================================================
    public class MenuSetting : ICloneable {
        // メインメニュー項目の定義
        private List<MenuItemSetting> m_mainMenuItem = new List<MenuItemSetting>();

        // ファイルビューアのメニュー項目の定義
        private List<MenuItemSetting> m_fileViewerMenuItem = new List<MenuItemSetting>();

        // グラフィックビューアのメニュー項目の定義
        private List<MenuItemSetting> m_graphicsViewerMenuItem = new List<MenuItemSetting>();

        // モニタリングビューアのメニュー項目の定義
        private List<MenuItemSetting> m_monitoringViewerMenuItem = new List<MenuItemSetting>();

        // ターミナルのメニュー項目の定義
        private List<MenuItemSetting> m_terminalMenuItem = new List<MenuItemSetting>();
        
        // タブメニュー
        private List<MenuItemSetting> m_tabMenu;
        
        // ログメニュー
        private List<MenuItemSetting> m_logMenu;

        // 実行メニュー
        private List<MenuItemSetting> m_executeMenu;

        // グラフィックビューアのズームメニュー
        private List<MenuItemSetting> m_graphicsViewerZoomMenuList;

        // グラフィックビューアのマークメニュー
        public List<MenuItemSetting> m_graphicsViewerMarkMenuList;

        // グラフィックビューアのフィルターメニュー
        private List<MenuItemSetting> m_graphicsViewerFilterMenuList;

        // ターミナルのコンテキストメニュー
        private List<MenuItemSetting> m_terminalContext;

        // メインメニューのカスタム情報
        private MenuItemCustomSetting m_mainMenuCustom = new MenuItemCustomSetting();

        // ファイルの書き込み日時
        private DateTime m_lastFileWriteTime = DateTime.MinValue;

        // オプションメニューの項目名
        private static string s_optionMenuName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MenuSetting() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            MenuSetting obj = (MenuSetting)(MemberwiseClone());
            obj.m_mainMenuCustom = (MenuItemCustomSetting)(m_mainMenuCustom.Clone());
            return obj;
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]warningList  読み込み時に発生した警告を集約する先の警告情報（集約しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(SettingWarningList warningList) {
            // メインメニュー
            {
                MenuItemSetting menu = new MenuItemSetting("ファイル", 'F');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(OpenFileAssociate1Command)),           'O', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(OpenFileAssociate2Command)),           'X', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(LocalEditCommand)),                    'E', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ContextMenuCommand)),                  'N', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ContextMenuFolderCommand)),            'F', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ShowPropertyCommand)),                 'Z', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CopyCommand)),                         'C', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MoveCommand)),                         'M', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CreateShortcutCommand)),               'T', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(DeleteCommand)),                       'D', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MirrorCopyCommand)),                   'Y', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(RenameCommand)),                       'R', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(RenameSelectedFileInfoCommand)),       'A', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MakeFolderCommand)),                   'K', null));
                MenuItemSetting fileEx = new MenuItemSetting("拡張ファイル操作", 'W');
                fileEx.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CopyExCommand)),                     'C', null));
                fileEx.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MoveExCommand)),                     'M', null));
                fileEx.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(DeleteExCommand)),                   'D', null));
                fileEx.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(HighspeedDeleteCommand)),            'H', null));
                fileEx.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(DuplicateCommand)),                  'P', null));
                fileEx.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                fileEx.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CombineFileCommand)),                'O', null));
                fileEx.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SplitFileCommand)),                  'S', null));
                fileEx.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(UnwrapFolderCommand)),               'U', null));
                menu.AddSubMenu(fileEx);
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirInputCommand)),                   'I', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirBookmarkFolderCommand)),          'B', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirDriveCommand)),                   'L', null));
                MenuItemSetting drive = new MenuItemSetting("ドライブを直接変更", 'V');
                drive.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCommand), "A:"),                 'A', "A:"));
                drive.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCommand), "B:"),                 'B', "B:"));
                drive.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCommand), "C:"),                 'C', "C:"));
                drive.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCommand), "D:"),                 'D', "D:"));
                drive.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCommand), "E:"),                 'E', "E:"));
                drive.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCommand), "F:"),                 'F', "F:"));
                drive.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCommand), "G:"),                 'G', "G:"));
                drive.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCommand), "H:"),                 'H', "H:"));
                drive.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCommand), "I:"),                 'I', "I:"));
                menu.AddSubMenu(drive);
                MenuItemSetting folder = new MenuItemSetting("フォルダ操作", 'Y');
                folder.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirParentCommand)),                'P', null));
                folder.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirRootCommand)),                  'R', null));
                folder.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirHomeCommand)),                  'M', null));
                folder.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirTargetToOppositeCommand)),      'O', null));
                folder.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirOppositeToTargetCommand)),      'T', null));
                folder.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirSwapTargetOppositeCommand)),    'W', null));
                folder.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirFolderHistoryCommand)),         'H', null));
                menu.AddSubMenu(folder);
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ExitCommand)),                         'Q', null));
                m_mainMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("編集", 'E');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClipboardCutCommand)),                 'X', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClipboardCopyCommand)),                'C', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClipboardNameCopyCommand)),            'N', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClipboardNameCopyAsCommand)),          'S', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileCompareCommand)),                  'P', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchCommand)),            'I', null));
                MenuItemSetting search = new MenuItemSetting("カーソル移動", 'F');
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CursorTopCommand)),                  '1', null));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CursorBottomCommand)),               '2', null));
                search.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "A"),   'A', "Aから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "B"),   'B', "Bから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "C"),   'C', "Cから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "D"),   'D', "Dから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "E"),   'E', "Eから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "F"),   'F', "Fから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "G"),   'G', "Gから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "H"),   'H', "Hから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "I"),   'I', "Iから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "J"),   'J', "Jから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "K"),   'K', "Kから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "L"),   'L', "Lから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "M"),   'M', "Mから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "N"),   'N', "Nから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "O"),   'O', "Oから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "P"),   'P', "Pから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "Q"),   'Q', "Qから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "R"),   'R', "Rから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "S"),   'S', "Sから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "T"),   'T', "Tから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "U"),   'U', "Uから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "V"),   'V', "Vから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "W"),   'W', "Wから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "X"),   'X', "Xから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "Y"),   'Y', "Yから始まるファイル"));
                search.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(IncrementalSearchExCommand), "Z"),   'Z', "Zから始まるファイル"));
                menu.AddSubMenu(search);
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ReverseAllMarkFileCommand)),           'R', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SelectAllMarkCommand)),                'A', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClearAllMarkCommand)),                 'L', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MarkWithConditionsCommand)),           'M', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TaskManagerCommand)),                  'T', null));
                m_mainMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("表示", 'V');
                MenuItemSetting border = new MenuItemSetting("左右境界の変更", 'B');
                border.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MoveFileListBorderCommand), 9999),   '1', "左ウィンドウを最大"));
                border.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MoveFileListBorderCommand), 16),     '2', "左ウィンドウを拡大"));
                border.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SetFileListBorderRatioCommand), 50), '3', "左右均等"));
                border.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MoveFileListBorderCommand), -16),    '4', "右ウィンドウを拡大"));
                border.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MoveFileListBorderCommand), -9999),  '5', "右ウィンドウを最大"));
                menu.AddSubMenu(border);
                MenuItemSetting tabMenu = new MenuItemSetting("タブ", 'T');
                tabMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabCreateCommand)),                 'N', null));
                tabMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabDeleteCommand)),                 'D', null));
                tabMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabDeleteOtherCommand)),            'O', null));
                tabMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabDeleteLeftCommand)),             'L', null));
                tabMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabDeleteRightCommand)),            'R', null));
                tabMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabReopenCommand)),                 'T', null));
                tabMenu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                tabMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectLeftCommand)),             'E', null));
                tabMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectRightCommand)),            'I', null));
                MenuItemSetting tabSelMenu = new MenuItemSetting("タブを選択", 'S');
                tabSelMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectDirectCommand), 0),     '1', "タブ1を選択"));
                tabSelMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectDirectCommand), 1),     '2', "タブ2を選択"));
                tabSelMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectDirectCommand), 2),     '3', "タブ3を選択"));
                tabSelMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectDirectCommand), 3),     '4', "タブ4を選択"));
                tabSelMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectDirectCommand), 4),     '5', "タブ5を選択"));
                tabSelMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectDirectCommand), 5),     '6', "タブ6を選択"));
                tabSelMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectDirectCommand), 6),     '7', "タブ7を選択"));
                tabSelMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectDirectCommand), 7),     '8', "タブ8を選択"));
                tabSelMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectDirectCommand), 8),     '9', "タブ9を選択"));
                tabSelMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(TabSelectDirectCommand), 9),     '0', "タブ10を選択"));
                tabMenu.AddSubMenu(tabSelMenu);
                m_tabMenu = tabMenu.SubMenuList;
                menu.AddSubMenu(tabMenu);
                MenuItemSetting logMenu = new MenuItemSetting("ログ", 'L');
                logMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ShowLatestLogLineCommand)),         'N', null));
                logMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ToggleLogSizeCommand)),             'L', null));
                logMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClearLogCommand)),                  'X', null));
                logMenu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                logMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CopyLogCommand)),                   'C', null));
                logMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SelectAllLogLineCommand)),          'A', null));
                m_logMenu = logMenu.SubMenuList;
                menu.AddSubMenu(logMenu);
                MenuItemSetting list = new MenuItemSetting("一覧表示", 'V');
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeSettingCommand)),      'S', null));
                list.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeDetailCommand)),       'D', null));
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnail32Command)),  '1', null));
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnail48Command)),  '2', null));
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnail64Command)),  '3', null));
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnail128Command)), '4', null));
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnail256Command)), '5', null));
                list.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeThumbnailCommand)),    'T', null));
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeToggleCommand)),       'G', null));
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeLargerCommand)),       'L', null));
                list.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListViewModeSmallerCommand)),      'M', null));
                menu.AddSubMenu(list);
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CloseAllViewerCommand)),               'A', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SetFocusStatePanelCommand)),           'P', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(RetrieveFolderSizeCommand)),           'I', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SortMenuCommand)),                     'S', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SortClearCommand)),                    'N', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListFilterMenuCommand)),           'F', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClearFileListFilterCommand)),          'E', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListColorSettingCommand)),         'T', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(RefreshFileListCommand)),              'R', null));
                m_mainMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("実行", 'X');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ShellExecuteMenuCommand)),             'X', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ShellCommandCommand)),                 'S', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(PowerShellCommand)),                   'W', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(OpenExplorerFolderCommand)),           'M', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(OpenControlPanelCommand)),             'C', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(OpenRecycleBinCommand)),               'R', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CleanupRecycleBinCommand)),            'Z', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SlideShowCommand)),                    'G', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SlideShowMarkResultCommand)),          'K', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(HTTPResponseViewerCommand)),           'H', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClipboardViewerCommand)),              'V', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ClipboardFilterCommand)),              'I', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FilterCopyCommand)),                   'T', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ArchiveCommand)),                      'P', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ExtractCommand)),                      'E', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirVirtualFolderCommand)),           'L', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(PrepareVirtualFolderCommand)),         'U', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(DiffMarkCommand)),                     'D', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(DiffOppositeCommand)),                 'O', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(DiffFolderCompareCommand)),            'F', null));
                m_executeMenu = menu.SubMenuList;
                m_mainMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("SSH", 'S');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirSSHFolderCommand)),               'S', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(LogoutCurrentSSHSessionCommand)),      'O', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(LogoutAllSSHSessionCommand)),          'X', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SSHChangeUserCommand)),                'U', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CreateSSHTerminalCommand)),            'T', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SelectSSHTerminalCommand)),            'H', null));
                MenuItemSetting sshTool = new MenuItemSetting("SSHツール", 'T');
                sshTool.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SSHProcessMonitorCommand)),         'P', null));
                sshTool.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(SSHNetworkMonitorCommand)),         'N', null));
                menu.AddSubMenu(sshTool);
                m_mainMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("オプション", 'O');
                s_optionMenuName = menu.ItemNameInput;
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ArchiveAutoPasswordOptionCommand)),    'A', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(BookmarkSettingCommand)),              'B', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(DeleteUserFolderCommand)),             'D', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(DeleteHistoryCommand)),                'T', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListKeySettingCommand)),           'K', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileViewerKeySettingCommand)),         'V', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(GraphicsViewerKeySettingCommand)),     'G', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(AssociateSettingCommand)),             'S', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(FileListMenuSettingCommand)),          'M', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(OptionSettingCommand)),                'O', null));
                m_mainMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("ヘルプ", 'H');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(KeyListMenuCommand)),                  'L', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(KeyBindHelpCommand)),                  'K', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MouseHelpCommand)),                    'M', null));
                MenuItemSetting listMenu = new MenuItemSetting("コマンド一覧", 'C');
                listMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(CommandListHelpCommand)),          'L', null));
                listMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(KeyListHelpCommand)),              'K', null));
                listMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(MenuListHelpCommand)),             'M', null));
                menu.AddSubMenu(listMenu);
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(AboutExternalSoftwareCommand)),        'E', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(ShellFilerWebPageCommand)),            'W', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(AboutShellFilerCommand)),              'V', null));
                m_mainMenuItem.Add(menu);
            }

            // ファイルビューアのメニュー
            {
                MenuItemSetting menu = new MenuItemSetting("ファイル", 'F');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SaveTextAsCommand)),                 'A', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_EditFileCommand)),                   'E', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ReturnFileListCommand)),             'Z', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ExitViewCommand)),                   'Q', null));
                m_fileViewerMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("編集", 'E');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CopyTextCommand)),                   'C', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CopyTextAsCommand)),                 'S', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectAllCommand)),                  'A', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectionCompareLeftCommand)),       'L', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectionCompareRightCommand)),      'R', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectionCompareDisplayCommand)),    'D', null));
                m_fileViewerMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("表示", 'V');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ChangeTextModeCommand)),             'X', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_ChangeDumpModeCommand)),             'D', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_FullScreenCommand)),                 'F', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_FullScreenMultiCommand)),            'M', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTabCommand), 2),                  '2', "TAB 2桁"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTabCommand), 4),                  '4', "TAB 4桁"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTabCommand), 8),                  '8', "TAB 8桁"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTabCommand), 16),                 '1', "TAB 16桁"));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "SJIS"),    'S', "ShiftJIS"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "UTF-8"),   'U', "UTF-8"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "EUC-JP"),  'E', "EUC"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "JIS"),     'J', "JIS"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetTextEncodingCommand), "UNICODE"), 'C', "UNICODE"));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SetLineWidthCommand), 0),            'W', null));
                m_fileViewerMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("検索・ジャンプ", 'S');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpTopLineCommand)),                'T', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpBottomLineCommand)),             'B', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), -1),             'J', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchCommand)),                     'S', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchForwardNextCommand)),          'N', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchReverseNextCommand)),          'P', null));
                m_fileViewerMenuItem.Add(menu);
            }

            // グラフィックビューアのメニュー
            {
                MenuItemSetting menu = new MenuItemSetting("ファイル", 'F');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SaveImageAsCommand)),                'A', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SlideShowNextCommand), true),        'N', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_SlideShowPrevCommand), true),        'P', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ReturnFileListCommand)),             'Z', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ExitViewCommand)),                   'Q', null));
                m_graphicsViewerMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("編集", 'E');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_CopyImageCommand)),                  'C', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MirrorHorizontalCommand)),           'H', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MirrorVerticalCommand)),             'V', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_RotateCWCommand)),                   'R', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_RotateCCWCommand)),                  'L', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                m_graphicsViewerMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("表示", 'V');
                MenuItemSetting zoomMenu = new MenuItemSetting("拡大率", 'Z');
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirectCommand), 6),          '*', "拡大6%"));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirectCommand), 10),         '*', "拡大10%"));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect20Command)),           '*', null));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect50Command)),           '*', null));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirectCommand), 75),         '*', "拡大75%"));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirectCommand), 80),         '*', "拡大80%"));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirectCommand), 90),         '*', "拡大90%"));
                zoomMenu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect100Command)),          '*', null));
                zoomMenu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirectCommand), 120),        '*', "拡大120%"));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect150Command)),          '*', null));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect200Command)),          '*', null));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirect500Command)),          '*', null));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirectCommand), 750),        '*', "拡大750%"));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirectCommand), 1000),       '*', "拡大1000%"));
                zoomMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomDirectCommand), 1600),       '*', "拡大1600%"));
                menu.AddSubMenu(zoomMenu);
                m_graphicsViewerZoomMenuList = zoomMenu.SubMenuList;
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomInCommand)),                     'I', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ZoomOutCommand)),                    'O', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FitImageToScreenCommand)),           'W', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FitShortEdgeToScreenCommand)),       'D', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ViewCenterCommand)),                 'C', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ToggleBackGroundColorCommand)),      'B', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_InterpolationHighQualityCommand)),   'H', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_InterpolationNeighborCommand)),      'X', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FullScreenCommand)),                 'F', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FullScreenMultiCommand)),            'M', null));
                m_graphicsViewerMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("フィルター", 'T');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterSettingCommand)),              'T', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterResetCommand)),                'S', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterOnOffCommand)),                'O', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), 10, 0, 0),     '*', "明るさ↑"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), -10, 0, 0),    '*', "明るさ↓"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), 0, 10, 0),     '*', "コントラスト↑"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), 0, -10, 0),    '*', "コントラスト↓"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), 0, 0, 10),     '*', "ガンマ↑"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBrightCommand), 0, 0, -10),    '*', "ガンマ↓"));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterHsvModifyCommand), 5, 0, 0),   '*', "色相↑"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterHsvModifyCommand), 5, 0, 0),   '*', "色相↓"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterHsvModifyCommand), 0, 10, 0),  '*', "彩度↑"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterHsvModifyCommand), 0, -10, 0), '*', "彩度↓"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterHsvModifyCommand), 0, 0, 10),  '*', "明度↑"));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterHsvModifyCommand), 0, 0, -10), '*', "明度↓"));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterMonochromeCommand)),           '*', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterNegativeCommand)),             '*', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterSepiaCommand), 30),            '*', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterSharpCommand), 30),            '*', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterBlurCommand), 30),             '*', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_FilterReliefCommand), 30),           '*', null));
                m_graphicsViewerMenuItem.Add(menu);
                m_graphicsViewerFilterMenuList = menu.SubMenuList;
            }
            {
                MenuItemSetting menu = new MenuItemSetting("マーク", 'M');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile1Command)),                  '1', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile2Command)),                  '2', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile3Command)),                  '3', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile4Command)),                  '4', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile5Command)),                  '5', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile6Command)),                  '6', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile7Command)),                  '7', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile8Command)),                  '8', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFile9Command)),                  '9', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_ClearMarkCommand)),                  '0', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFileListCommand)),               'F', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(G_MarkFileHelpCommand)),               'H', null));
                m_graphicsViewerMenuItem.Add(menu);
                m_graphicsViewerMarkMenuList = menu.SubMenuList;
            }

            // モニタリングビューアのメニュー
            {
                MenuItemSetting menu = new MenuItemSetting("ファイル", 'F');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SaveAsCommand)),                     'A', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_ReturnFileListCommand)),             'Z', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_ExitViewCommand)),                   'Q', null));
                m_monitoringViewerMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("編集", 'E');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_CopyAllAsCommand)),                  'C', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SearchCommand)),                     'S', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SearchForwardNextCommand)),          'N', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_SearchReverseNextCommand)),          'P', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_JumpTopLineCommand)),                'T', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_JumpBottomLineCommand)),             'B', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_RefreshCommand)),                    'R', null));
                m_monitoringViewerMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MonitoringViewerMenuItemSetting("操作", 'P', MonitoringViewerMode.PS);
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_PsDetailCommand)),                   'V', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_PsKillCommand)),                     'K', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(M_PsForceTerminateCommand)),           'F', null));
                m_monitoringViewerMenuItem.Add(menu);
            }

            // ターミナルのメニュー
            {
                MenuItemSetting menu = new MenuItemSetting("ファイル", 'F');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SaveAsCommand)),                      'A', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_NewTerminal)),                        'N', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_EditDisplayNameCommand)),             'T', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ReturnFileListCommand)),              'Z', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ExitTerminalCommand)),                'Q', null));
                m_terminalMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("編集", 'E');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_CopyClipboardCommand)),               'C', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_PasteClipboardCommand)),              'P', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SelectAllCommand)),                   'A', null));
                m_terminalMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("表示", 'I');
                MenuItemSetting scrollMenu = new MenuItemSetting("スクロール", 'S');
                scrollMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollUpCommand), 1),           'P', null));
                scrollMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollDownCommand), 1),         'N', null));
                scrollMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollRollUpCommand)),          'U', null));
                scrollMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollRollDownCommand)),        'D', null));
                scrollMenu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                scrollMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollUpCurrentCommand), 1),    'C', null));
                scrollMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollDownCurrentCommand), 1),  'W', null));
                scrollMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollRollUpCurrentCommand)),   'O', null));
                scrollMenu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ScrollRollDownCurrentCommand)), 'L', null));
                menu.AddSubMenu(scrollMenu);
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ToggleBackLogViewCommand)),           'L', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ClearBackLogCommand)),                'C', null));
                m_terminalMenuItem.Add(menu);
            }
            {
                MenuItemSetting menu = new MenuItemSetting("表示", 'I');
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_CopyClipboardCommand)),               'C', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_PasteClipboardCommand)),              'P', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_SelectAllCommand)),                   'A', null));
                menu.AddSubMenu(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ToggleBackLogViewCommand)),           'L', null));
                menu.AddSubMenu(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(T_ClearBackLogCommand)),                'C', null));
                m_terminalContext = menu.SubMenuList;
            }
            // カスタマイズ部分を読み込む
            m_mainMenuCustom.Initialize(m_mainMenuItem);
            LoadSetting(warningList);
        }


        //=========================================================================================
        // 機　能：設定の読み込みを行う
        // 引　数：[in]warningList  読み込み時に発生した警告を集約する先の警告情報（集約しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void LoadSetting(SettingWarningList warningList) {
#if !FREE_VERSION
            m_lastFileWriteTime = DateTime.MinValue;

            string fileName = DirectoryManager.MenuSetting;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = LoadSettingInternal(loader, this);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSetting, fileName, loader.ErrorDetail);
            } else {
                m_lastFileWriteTime = loader.LastFileWriteTime;

                // 警告を処理
                if (loader.WarningList.Count > 0) {
                    SettingWarningList.AddWarningInfo(loader, warningList);
                }
            }
#endif
        }
        
        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSettingInternal(SettingLoader loader, MenuSetting obj) {
            // ファイルがないときはそのまま
            if (!File.Exists(loader.FileName)) {
                obj.m_mainMenuCustom.Initialize(obj.m_mainMenuItem);
                return true;
            }

            // ファイルから読み込む
            bool success;
            success = loader.LoadSetting(false);
            if (!success) {
                obj.m_mainMenuCustom.Initialize(obj.m_mainMenuItem);
                return false;
            }

            // ファイル一覧
            loader.SetCurrentWarningGroup(SettingLoader.WarningGroup.MenuFileList);
            loader.ResetReadPosition();
            success = LoadSettingMenuList(loader, SettingTag.MenuSetting_FileList, obj.m_mainMenuItem, out obj.m_mainMenuCustom);
            if (!success) {
                obj.m_mainMenuCustom = new MenuItemCustomSetting();
                obj.m_mainMenuCustom.Initialize(obj.m_mainMenuItem);
                loader.SetWarning(Resources.SettingLoader_LoadMenuSettingParseFailed);
            }
            return true;
        }

        //=========================================================================================
        // 機　能：キー設定をファイルから読み込む
        // 引　数：[in]loader      読み込み用クラス
        // 　　　　[in]targetTag   読み込み範囲となるタグ
        // 　　　　[in]definedList 定義済みメニュー
        // 　　　　[out]obj        読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSettingMenuList(SettingLoader loader, SettingTag targetTag, List<MenuItemSetting> definedList, out MenuItemCustomSetting obj) {
            obj = null;
            bool success;
            success = loader.ExpectTag(SettingTag.MenuSetting_MenuSetting, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.MenuSetting_MenuSetting) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == targetTag) {
                    success = MenuItemCustomSetting.LoadSetting(loader, definedList, out obj);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(targetTag, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                    break;
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
            string fileName = DirectoryManager.MenuSetting;
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
        private static bool SaveSettingInternal(SettingSaver saver, MenuSetting obj) {
            bool success;

            saver.StartObject(SettingTag.MenuSetting_MenuSetting);

            // ファイル一覧
            saver.StartObject(SettingTag.MenuSetting_FileList);
            success = MenuItemCustomSetting.SaveSetting(saver, obj.m_mainMenuCustom);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.MenuSetting_FileList);

            saver.EndObject(SettingTag.MenuSetting_MenuSetting);

            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(MenuSetting obj1, MenuSetting obj2) {
            if (!MenuItemCustomSetting.EqualsConfig(obj1.m_mainMenuCustom, obj2.m_mainMenuCustom)) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：メニューのカスタマイズ済み項目一覧を返す
        // 引　数：[in]scene   メニューの利用シーン
        // 戻り値：メニュー項目の定義
        //=========================================================================================
        public List<MenuItemSetting> CreateMenuCustomizedList(CommandUsingSceneType scene) {
            List<MenuItemSetting> defaultMenu = null;
            MenuItemCustomSetting menuCustom = null;
            if (scene == CommandUsingSceneType.FileList) {
                defaultMenu = m_mainMenuItem;
                menuCustom = m_mainMenuCustom;
            } else if (scene == CommandUsingSceneType.FileViewer) {
                defaultMenu = m_fileViewerMenuItem;
                menuCustom = new MenuItemCustomSetting();
            } else if (scene == CommandUsingSceneType.GraphicsViewer) {
                defaultMenu = m_graphicsViewerMenuItem;
                menuCustom = new MenuItemCustomSetting();
            } else if (scene == CommandUsingSceneType.MonitoringViewer) {
                defaultMenu = m_monitoringViewerMenuItem;
                menuCustom = new MenuItemCustomSetting();
            }

            List<MenuItemSetting> result = new List<MenuItemSetting>();
            List<MenuItemCustomRoot> customItems = new List<MenuItemCustomRoot>();
            customItems.AddRange(menuCustom.CustomRootList);
            List<MenuItemSetting> menuItems = defaultMenu;
            string prevMenu = "";
            for (int i = 0; i < menuItems.Count; i++) {
                for (int j = 0; j < customItems.Count; j++) {
                    if (customItems[j] != null && customItems[j].DisplayRoot && customItems[j].PrevItemName == prevMenu) {
                        result.Add(customItems[j].MenuSetting);
                        customItems[j] = null;
                    }
                }
                bool display = menuCustom.IsDisplayMenu(menuItems[i].ItemNameInput);
                if (display) {
                    result.Add(menuItems[i]);
                }
                prevMenu = menuItems[i].ItemNameInput;
            }
            for (int i = 0; i < customItems.Count; i++) {
                if (customItems[i] != null && customItems[i].DisplayRoot) {
                    result.Add(customItems[i].MenuSetting);
                }
            }
            return result;
        }

        //=========================================================================================
        // プロパティ：メインメニュー項目の定義
        //=========================================================================================
        public List<MenuItemSetting> MainMenuItemList {
            get {
                return m_mainMenuItem;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルビューアのメニュー項目の定義
        //=========================================================================================
        public List<MenuItemSetting> FileViewerMenuItemList {
            get {
                return m_fileViewerMenuItem;
            }
        }
        
        //=========================================================================================
        // プロパティ：グラフィックビューアのメニュー項目の定義
        //=========================================================================================
        public List<MenuItemSetting> GraphicsViewerMenuItemList {
            get {
                return m_graphicsViewerMenuItem;
            }
        }
        
        //=========================================================================================
        // プロパティ：モニタリングビューアのメニュー項目の定義
        //=========================================================================================
        public List<MenuItemSetting> MonitoringViewerMenuItemList {
            get {
                return m_monitoringViewerMenuItem;
            }
        }
        
        //=========================================================================================
        // プロパティ：ターミナルのメニュー項目の定義
        //=========================================================================================
        public List<MenuItemSetting> TerminalMenuItem {
            get {
                return m_terminalMenuItem;
            }
        }

        //=========================================================================================
        // プロパティ：タブメニュー
        //=========================================================================================
        public List<MenuItemSetting> TabMenu {
            get {
                return m_tabMenu;
            }
        }

        //=========================================================================================
        // プロパティ：ログメニュー
        //=========================================================================================
        public List<MenuItemSetting> LogMenu {
            get {
                return m_logMenu;
            }
        }

        //=========================================================================================
        // プロパティ：実行メニュー
        //=========================================================================================
        public List<MenuItemSetting> ExecuteMenu {
            get {
                return m_executeMenu;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアのズームメニュー
        //=========================================================================================
        public List<MenuItemSetting> GraphicsViewerZoomMenuList {
            get {
                return m_graphicsViewerZoomMenuList;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアのマークメニュー
        //=========================================================================================
        public List<MenuItemSetting> GraphicsViewerMarkMenuList {
            get {
                return m_graphicsViewerMarkMenuList;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアのフィルターメニュー
        //=========================================================================================
        public List<MenuItemSetting> GraphicsViewerFilterMenuList {
            get {
                return m_graphicsViewerFilterMenuList;
            }
        }

        //=========================================================================================
        // プロパティ：ターミナルのコンテキストメニュー
        //=========================================================================================
        public List<MenuItemSetting> TerminalContext {
            get {
                return m_terminalContext;
            }
        }

        //=========================================================================================
        // プロパティ：メインメニューのカスタム情報
        //=========================================================================================
        public MenuItemCustomSetting MainMenuCustom {
            get {
                return m_mainMenuCustom;
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
        // プロパティ：オプションメニューの項目名
        //=========================================================================================
        public static string OptionMenuName {
            get {
                return s_optionMenuName;
            }
        }
   }
}
