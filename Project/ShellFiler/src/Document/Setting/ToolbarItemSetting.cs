using System;
using System.Collections.Generic;
using ShellFiler.Api;
using ShellFiler.Command;
using ShellFiler.UI;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ツールバーのカスタマイズ情報
    //=========================================================================================
    class ToolbarItemSetting {
        // アイコンボタンの幅
        private const int CX_ICON_BUTTON = 23;

        // アイコンボタンの高さ
        private const int CY_ICON_BUTTON = 22;

        // 項目の種類
        private ItemType m_itemType;

        // 実行するコマンド
        private ActionCommandMoniker m_commandMoniker;

        // コマンドのUI表現
        private UIResource m_uiResource;

        // 項目を有効にする条件
        private UIEnableCondition m_uiEnableCondition;

        // ツールヒントのテキスト
        private string m_toolHintText;
        
        // サブメニューのリスト
        private List<MenuItemSetting> m_subMenuList = new List<MenuItemSetting>();

        //=========================================================================================
        // 機　能：コンストラクタ（セパレータ／ドライブリスト）
        // 引　数：[in]type  アイコンの種類
        // 戻り値：なし
        //=========================================================================================
        public ToolbarItemSetting(ItemType type) {
            m_itemType = type;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（ボタン）
        // 引　数：[in]moniker       実行するコマンド
        // 　　　　[in]condition     項目を有効にする条件（カスタマイズしないときnull）
        // 　　　　[in]toolHintText  ツールヒントのテキスト（カスタマイズしないときnull）
        // 戻り値：なし
        //=========================================================================================
        public ToolbarItemSetting(ActionCommandMoniker moniker, UIEnableCondition condition, string toolHintText) {
            m_commandMoniker = moniker;
            m_itemType = ItemType.Button;
            ActionCommand actionCommand = moniker.CreateActionCommand();
            m_uiResource = actionCommand.UIResource;
            if (condition != null) {
                m_uiEnableCondition = condition;
            } else {
                m_uiEnableCondition = m_uiResource.UIEnableCondition;
            }
            if (toolHintText != null) {
                m_toolHintText = toolHintText;
            } else {
                m_toolHintText = m_uiResource.Hint;
            }
        }

        //=========================================================================================
        // 機　能：サブメニューを追加する
        // 引　数：[in]item  サブメニュー
        // 戻り値：なし
        //=========================================================================================
        public void AddSubMenu(MenuItemSetting item) {
            m_subMenuList.Add(item);
        }

        //=========================================================================================
        // プロパティ：アイコンボタンの幅
        //=========================================================================================
        public static int CxIconButton {
            get {
                return MainWindowForm.X(CX_ICON_BUTTON);
            }
        }

        //=========================================================================================
        // プロパティ：アイコンボタンの高さ
        //=========================================================================================
        public static int CyIconButton {
            get {
                return MainWindowForm.X(CY_ICON_BUTTON);
            }
        }

        //=========================================================================================
        // プロパティ：ツールバー項目の種類
        //=========================================================================================
        public ItemType Type {
            get {
                return m_itemType;
            }
        }

        //=========================================================================================
        // プロパティ：実行するコマンド
        //=========================================================================================
        public ActionCommandMoniker ActionCommandMoniker {
            get {
                return m_commandMoniker;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public UIResource UIResource {
            get {
                return m_uiResource;
            }
        }

        //=========================================================================================
        // プロパティ：項目を有効にする条件
        //=========================================================================================
        public UIEnableCondition UIEnableCondition {
            get {
                return m_uiEnableCondition;
            }
        }

        //=========================================================================================
        // プロパティ：ツールヒントのテキスト
        //=========================================================================================
        public string ToolHintText {
            get {
                return m_toolHintText;
            }
        }

        //=========================================================================================
        // プロパティ：サブメニューのリスト
        //=========================================================================================
        public List<MenuItemSetting> SubMenuList {
            get {
                return m_subMenuList;
            }
        }

        //=========================================================================================
        // 列挙子：項目の種類
        //=========================================================================================
        public enum ItemType {
            Button,             // ボタン
            Separator,          // セパレータ
            DriveList,          // ドライブ一覧
        }
    }
}
