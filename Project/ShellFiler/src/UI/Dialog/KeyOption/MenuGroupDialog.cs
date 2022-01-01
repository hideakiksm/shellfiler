using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：メニューのポップアップグループ名を入力するダイアログ
    //=========================================================================================
    public partial class MenuGroupDialog : Form {
        // グループ名
        private string m_groupName;

        // ショートカットキー
        private char m_shortcutKey;

        // 親階層のメニュー一覧（重複チェック用）
        private List<MenuItemSetting> m_parentMenuList;

        // 編集中のメニュー項目（新規作成のときnull）
        private MenuItemSetting m_editTargetMenu;

        // ショートカットキーのコンボボックスの実装
        private LasyComboBoxImpl m_comboboxImplShortcut;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]groupName     グループ名の初期値
        // 　　　　[in]shortcutKey   ショートカットキーの初期値
        // 　　　　[in]parentList    親階層のメニュー一覧（重複チェック用）
        // 　　　　[in]editTarget    編集中のメニュー項目（新規作成のときnull）
        // 戻り値：なし
        //=========================================================================================
        public MenuGroupDialog(string groupName, char shortcutKey, List<MenuItemSetting> parentList, MenuItemSetting editTarget) {
            InitializeComponent();
            m_parentMenuList = parentList;
            m_editTargetMenu = editTarget;

            this.textBoxGroupName.Text = groupName;
            int shortcutIndex = MenuListSettingDialog.ShortcutToIndex(shortcutKey);
            m_comboboxImplShortcut = new LasyComboBoxImpl(this.comboBoxShortcut, MenuListSettingDialog.SHORTCUT_ITEMS, shortcutIndex);
            m_comboboxImplShortcut.SelectedIndexChanged += new EventHandler(comboBoxShortcut_SelectedIndexChanged);
            comboBoxShortcut_SelectedIndexChanged(this, null);
        }

        //=========================================================================================
        // 機　能：ショートカットキーのコンボボックスの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxShortcut_SelectedIndexChanged(object sender, EventArgs evt) {
            if (m_comboboxImplShortcut == null) {
                return;
            }
            int index = m_comboboxImplShortcut.SelectedIndex;
            bool unique = MenuListSettingDialog.CheckShortcutUnique(index, m_parentMenuList, m_editTargetMenu);
            this.labelShortcutWarning.Visible = !unique;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.textBoxGroupName.Text.Trim().Length == 0) {
                InfoBox.Warning(this, Resources.DlgMenuSetting_NoGroupName);
                return;
            }
            m_groupName = this.textBoxGroupName.Text;
            m_shortcutKey = MenuListSettingDialog.IndexToShortcut(m_comboboxImplShortcut.SelectedIndex);

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：グループ名
        //=========================================================================================
        public string GroupName {
            get {
                return m_groupName;
            }
        }

        //=========================================================================================
        // プロパティ：ショートカットキー
        //=========================================================================================
        public char ShortcutKey {
            get {
                return m_shortcutKey;
            }
        }
    }
}
