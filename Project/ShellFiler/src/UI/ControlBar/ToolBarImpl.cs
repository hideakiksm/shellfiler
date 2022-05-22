using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.Command.FileList.Window;
using ShellFiler.Command.FileViewer;
using ShellFiler.Util;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：ツールバーのアイコン一覧の実装
    //=========================================================================================
    class ToolBarImpl {
        // ターゲットのツールバーUI
        private ToolStrip m_toolStrip;

        // ドロップダウンメニューの実装
        private MenuImpl m_menuImpl;

        // コマンドの送信元の識別用
        private UICommandSender m_commandSender;
        
        // コマンドの送信先
        private IUICommandTarget m_commandTarget;

        // ツールバーボタンの項目一覧
        private List<ToolStripButton> m_toolButtonList = new List<ToolStripButton>();
        
        // ツールバースプリットボタンの項目一覧
        private List<ToolStripSplitButton> m_toolSplitButtonList = new List<ToolStripSplitButton>();

        // ドライブのアイコンイメージ
        private List<Bitmap> m_driveIconList = new List<Bitmap>();

        // 現在のドライブ一覧
        private string[] m_currentDriveList = new string[0];

        // ボタンの状態（マークされている状態として更新されているときtrue）
        private bool m_uiStatusMarked = false;

        // ボタンの状態（直前のパスヒストリが有効として更新されているときtrue）
        private bool m_uiStatusPathHisPrev = true;

        // ボタンの状態（直後のパスヒストリが有効として更新されているときtrue）
        private bool m_uiStatusPathHisNext = true;

        // ボタンサイズ
        private Size m_buttonSize;

        // ボタンのpadding
        private Padding m_buttonPadding;

        // 作成しようとしているルート項目の追加が適切なときtrueを返すdelegate
        public delegate bool IsValidToolBarItemDelegate(ToolbarItemSetting item);

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]toolStrip     ターゲットのツールバーUI
        // 　　　　[in]commandSender コマンドの送信元の識別用
        // 　　　　[in]commandTarget コマンドの送信先
        // 戻り値：なし
        //=========================================================================================
        public ToolBarImpl(ToolStrip toolStrip, UICommandSender commandSender, IUICommandTarget commandTarget) {
            m_toolStrip = toolStrip;
            m_toolStrip.ImageList = UIIconManager.IconImageList;
            m_commandSender = commandSender;
            m_commandTarget = commandTarget;
            m_menuImpl = new MenuImpl(commandSender, commandTarget);
            m_uiStatusMarked = true;
            m_uiStatusPathHisPrev = true;
            m_uiStatusPathHisNext = true;

            m_buttonSize = new Size(ToolbarItemSetting.CxIconButton, ToolbarItemSetting.CyIconButton);
            m_toolStrip.ImageScalingSize = m_buttonSize;
            if (MainWindowForm.XDpiRatio > 100) {
                m_buttonPadding = new Padding(MainWindowForm.X(2), MainWindowForm.Y(2), MainWindowForm.X(4), MainWindowForm.Y(2));
            } else {
                m_buttonPadding = new Padding(0, 0, 0, 0);
            }
        }

        //=========================================================================================
        // 機　能：設定からボタンを追加する
        // 引　数：[in]itemSettingList  ボタンの設定
        // 　　　　[in]keySetting       キーの設定
        // 　　　　[in]isValid          作成しようとしているルート項目の追加が適切なときtrueを返すdelegate（常に作成するときnull）
        // 戻り値：なし
        //=========================================================================================
        public void AddButtonsFromSetting(List<ToolbarItemSetting> itemSettingList, KeyItemSettingList keySetting, IsValidToolBarItemDelegate isValid) {
            m_toolStrip.SuspendLayout();

            foreach (ToolbarItemSetting itemSetting in itemSettingList) {
                if (isValid != null && !isValid(itemSetting)) {
                    continue;
                }
                switch (itemSetting.Type) {
                    case ToolbarItemSetting.ItemType.Button:
                        CreateToolbarButton(itemSetting, keySetting);
                        break;
                    case ToolbarItemSetting.ItemType.Separator:
                        m_toolStrip.Items.Add(new ToolStripSeparator());
                        break;
                    case ToolbarItemSetting.ItemType.DriveList:
                        RefreshToolbarDriveList();
                        break;
                }
            }

            m_toolStrip.ResumeLayout(true);
            m_toolStrip.PerformLayout();
        }

        //=========================================================================================
        // 機　能：ツールバーのボタンを追加する
        // 引　数：[in]itemSettingList  ボタンの設定
        // 　　　　[in]keySetting       キーの設定
        // 戻り値：なし
        //=========================================================================================
        private void CreateToolbarButton(ToolbarItemSetting itemSetting, KeyItemSettingList keySetting) {
            IconImageListID icon;
            string toolHint;
            ToolbarItemTag tag;
            CreateToolButtonComponent(itemSetting, keySetting, out icon, out toolHint, out tag);

            int padding = MainWindowForm.X(2);
            if (itemSetting.SubMenuList.Count == 0) {
                ToolStripButton button = new ToolStripButton();
                button.ImageIndex = (int)icon;
                button.Text = toolHint;
                button.DisplayStyle = ToolStripItemDisplayStyle.Image;
                button.Size = m_buttonSize;
                button.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                button.Margin = m_buttonPadding;
                button.Tag = tag;
                button.Click += new EventHandler(toolbarIconClicked);
                m_toolButtonList.Add(button);
                m_toolStrip.Items.Add(button);
            } else {
                ToolStripSplitButton button = new ToolStripSplitButton();
                button.ImageIndex = (int)icon;
                button.Text = toolHint;
                button.DisplayStyle = ToolStripItemDisplayStyle.Image;
                button.Size = m_buttonSize;
                button.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                button.Margin = m_buttonPadding;
                button.Tag = tag;
                button.ButtonClick += new EventHandler(toolbarIconClicked);
                button.DropDown.ImageList = UIIconManager.IconImageList;
                m_menuImpl.CreateDropdownMenuItem(button.DropDownItems, itemSetting.SubMenuList, keySetting);
                m_toolSplitButtonList.Add(button);
                m_toolStrip.Items.Add(button);
            }
        }

        //=========================================================================================
        // 機　能：ツールバーボタンの構成要素を作成する
        // 引　数：[in]itemSettingList  ボタンの設定
        // 　　　　[in]keySetting       キーの設定
        // 　　　　[out]icon            アイコン
        // 　　　　[out]toolHint        ツールヒント
        // 　　　　[out]tag             タグに使用するToolbarItemTag
        // 戻り値：なし
        //=========================================================================================
        private void CreateToolButtonComponent(ToolbarItemSetting itemSetting, KeyItemSettingList keySetting, out IconImageListID icon, out string toolHint, out ToolbarItemTag tag) {
            // アイコンを初期化
            IconImageListID leftIcon = itemSetting.UIResource.IconIdLeft;
            IconImageListID rightIcon = itemSetting.UIResource.IconIdRight;
            icon = leftIcon;

            // ツールヒント
            string shortcut = CreateShortcutDisplayString(keySetting, itemSetting.ActionCommandMoniker);
            toolHint = itemSetting.ToolHintText;
            if (shortcut != null) {
                toolHint += " (" + shortcut + ")";
            }

            // タグ
            tag = new ToolbarItemTag(itemSetting, leftIcon, rightIcon, null, false);
        }

        //=========================================================================================
        // 機　能：ショートカットキーの文字列を作成する
        // 引　数：[in]keySetting   キーの設定
        // 　　　　[in]command      実行するコマンドのモニカ
        // 戻り値：ショートカットキーの文字列（登録されていないコマンドの場合、null）
        //=========================================================================================
        public static string CreateShortcutDisplayString(KeyItemSettingList keySetting, ActionCommandMoniker command) {
            // 指定コマンドと同じキー割り当てのキー設定一覧を取得
            List<KeyItemSetting> keySettingList = keySetting.GetSettingFromCommand(command);
            if (keySettingList == null) {
                return null;
            }

            // ショートカットキー一覧を作成
            List<KeyState> shortcutKeyList = new List<KeyState>();
            foreach (KeyItemSetting setting in keySettingList) {
                if (!shortcutKeyList.Contains(setting.KeyState)) {
                    shortcutKeyList.Add(setting.KeyState);
                }
            }

            // 文字列に変換
            string strShortcut = "";
            if (shortcutKeyList.Count >= 1) {
                strShortcut += shortcutKeyList[0].GetDisplayName(keySetting);
            }
            if (shortcutKeyList.Count >= 2) {
                strShortcut += ", " + shortcutKeyList[1].GetDisplayName(keySetting);
            }

            return strShortcut;
        }

        //=========================================================================================
        // 機　能：ツールバーのドライブ一覧ボタンを追加する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshToolbarDriveList() {
            // ドライブ一覧を取得
            string[] driveList = Directory.GetLogicalDrives();
            if (m_currentDriveList.Equals(driveList)) {
                return;
            }
            m_currentDriveList = driveList;

            // 古いボタンを破棄
            foreach (Bitmap bmp in m_driveIconList) {
                bmp.Dispose();
            }
            m_driveIconList.Clear();

            int itemCount = m_toolStrip.Items.Count;
            for (int i = itemCount - 1; i >= 0; i--) {
                object objTag = m_toolStrip.Items[i].Tag;
                if (objTag is ToolbarItemTag) {
                    ToolbarItemTag tag = (ToolbarItemTag)objTag;
                    if (tag.DriveButton) {
                        this.m_toolStrip.Items.RemoveAt(i);
                    }
                }
            }

            // 新しい状態で登録
            foreach (string driveLetter in driveList) {
                // ドライブ情報を取得
                if (driveLetter.Length < 1) {
                    continue;
                }
                DriveInfo driveInfo = new DriveInfo(driveLetter);
                char driveChar = driveLetter[0];

                // アイコンをロード
                Bitmap icon = DriveItem.GetDriveIcon(driveInfo);
                m_driveIconList.Add(icon);
                
                // 項目情報を作成
                ActionCommandMoniker moniker = new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirCommand), driveChar + ":");
                ToolbarItemSetting itemSetting = new ToolbarItemSetting(moniker, null, null);

                // ツールヒントを作成
                string buttonText = DriveItem.GetDisplayName(driveInfo);

                // ツールバーボタンを作成
                ToolStripButton button = new ToolStripButton();
                button.Image = icon;
                button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
                button.Size = m_buttonSize;
                button.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                button.Margin = m_buttonPadding;
                button.Text = buttonText;
                button.Tag = new ToolbarItemTag(itemSetting, IconImageListID.None, IconImageListID.None, icon, true);
                button.Click += new EventHandler(toolbarIconClicked);
                m_toolStrip.Items.Add(button);
            }
        }

        //=========================================================================================
        // 機　能：外部で定義したツールバーの項目を追加する
        // 引　数：[in]item  ツールバーの項目
        // 戻り値：なし
        //=========================================================================================
        public void AddExternalItem(ToolStripItem item) {
            m_toolStrip.Items.Add(item);
        }

        //=========================================================================================
        // 機　能：項目がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void toolbarIconClicked(object sender, EventArgs evt) {
            ToolbarItemTag tag = null;
            if (sender is ToolStripButton) {
                tag = (ToolbarItemTag)(((ToolStripButton)sender).Tag);
            } else {
                tag = (ToolbarItemTag)(((ToolStripSplitButton)sender).Tag);
            }
            ToolbarItemSetting setting = tag.ItemSetting;
            UICommandItem item = new UICommandItem(setting.ActionCommandMoniker);
            m_commandTarget.OnUICommand(m_commandSender, item);
        }
        
        //=========================================================================================
        // 機　能：メインウィンドウのカーソルの左右に変化が生じたときの処理を行う
        // 引　数：[in]isLeft  カーソルが左側にあるときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnCursorLRChanged(bool isLeft) {
            foreach (ToolStripItem item in m_toolStrip.Items) {
                if (item.Tag == null) {
                    continue;
                }
                ToolbarItemTag tag = (ToolbarItemTag)(item.Tag);
                if (tag.OriginalIcon != null) {
                    item.Image = tag.OriginalIcon;
                } else {
                    item.Image = null;
                    if (tag.LeftIconId != tag.RightIconId) {
                        if (isLeft) {
                            item.ImageIndex = (int)tag.LeftIconId;
                        } else {
                            item.ImageIndex = (int)tag.RightIconId;
                        }
                    }
                }
            }

            m_menuImpl.OnCursorLRChanged(isLeft);
        }

        //=========================================================================================
        // 機　能：ツールバーのステータス状態を更新する
        // 引　数：[in]context   ツールバーの更新状態のコンテキスト
        // 戻り値：なし
        //=========================================================================================
        public void RefreshToolbarStatus(UIItemRefreshContext context) {
            // マーク中のみ有効
            if (m_uiStatusMarked != context.IsMarked) {                
                m_uiStatusMarked = context.IsMarked;
                foreach (ToolStripButton button in m_toolButtonList) {
                    ToolbarItemSetting itemSetting = ((ToolbarItemTag)(button.Tag)).ItemSetting;
                    if (UIEnableCondition.CheckMark(itemSetting.UIEnableCondition)) {
                        button.Enabled = m_uiStatusMarked;
                    }
                }
            }

            // 前のパスヒストリが有効
            if (m_uiStatusPathHisPrev != context.GetPathHistoryPrev(m_commandSender)) {
                m_uiStatusPathHisPrev = context.GetPathHistoryPrev(m_commandSender);
                foreach (ToolStripButton button in m_toolButtonList) {
                    ToolbarItemSetting itemSetting = ((ToolbarItemTag)(button.Tag)).ItemSetting;
                    if (itemSetting.UIEnableCondition == UIEnableCondition.PathHistP) {
                        button.Enabled = m_uiStatusPathHisPrev;
                    }
                }
            }

            // 次のパスヒストリが有効
            if (m_uiStatusPathHisNext != context.GetPathHistoryNext(m_commandSender)) {
                m_uiStatusPathHisNext = context.GetPathHistoryNext(m_commandSender);
                foreach (ToolStripButton button in m_toolButtonList) {
                    ToolbarItemSetting itemSetting = ((ToolbarItemTag)(button.Tag)).ItemSetting;
                    if (itemSetting.UIEnableCondition == UIEnableCondition.PathHistN) {
                        button.Enabled = m_uiStatusPathHisNext;
                    }
                }
            }

            m_menuImpl.RefreshToolbarStatus(context);
        }

        //=========================================================================================
        // プロパティ：ボタン一覧の幅
        //=========================================================================================
        public int ButtonWidth {
            get {
                int width = 0;
                foreach (ToolStripButton button in m_toolButtonList) {
                    width += button.Size.Width;
                    width += button.Margin.Horizontal;
                }
                foreach (ToolStripSplitButton button in m_toolSplitButtonList) {
                    width += button.Size.Width;
                    width += button.Margin.Horizontal;
                }
                return width;
            }
        }

        //=========================================================================================
        // プロパティ：ツールバーボタンのTagプロパティに保持する情報
        //=========================================================================================
        private class ToolbarItemTag {
            // ツールバーの設定情報
            public ToolbarItemSetting ItemSetting;

            // 左側にカーソルがあるときのアイコンID
            public IconImageListID LeftIconId;
            
            // 右側にカーソルがあるときのアイコンID
            public IconImageListID RightIconId;
            
            // 作成したオリジナルのアイコン（イメージリストを使用する場合はnull、null以外の場合はこちらが優先）
            public Bitmap OriginalIcon;

            // ドライブ一覧のボタンのときtrue
            public bool DriveButton;

            public ToolbarItemTag(ToolbarItemSetting itemSetting, IconImageListID leftIcon, IconImageListID rightIcon, Bitmap originalIcon, bool driveButton) {
                this.ItemSetting = itemSetting;
                this.LeftIconId = leftIcon;
                this.RightIconId = rightIcon;
                this.OriginalIcon = originalIcon;
                this.DriveButton = driveButton;
            }
        }
    }
}
