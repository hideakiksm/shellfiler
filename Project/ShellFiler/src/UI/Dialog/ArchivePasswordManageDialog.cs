using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：展開用パスワードの管理ダイアログ
    //=========================================================================================
    public partial class ArchivePasswordManageDialog : Form {
        // パスワードの表記
        private const string PASSWORD_DISPLAY_STRING = "********";

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ArchivePasswordManageDialog() {
            InitializeComponent();

            Program.Document.UserSetting.ArchiveAutoPasswordSetting.LoadData();
            this.listViewPasswordList.SmallImageList = UIIconManager.IconImageList;
            ArchiveAutoPasswordSetting setting = Program.Document.UserSetting.ArchiveAutoPasswordSetting;
            foreach(ArchiveAutoPasswordItem settingItem in setting.AutoPasswordItemList) {
                ListViewItem item = CreateListViewItem(settingItem);
                this.listViewPasswordList.Items.Add(item);
            }
            UpdateButtonState();

#if FREE_VERSION
            // Freeware版
            this.labelFreeware.Text = Resources.Dlg_FreewareInfo;
            this.labelFreeware.BackColor = Color.LightYellow;
#endif
        }

        //=========================================================================================
        // 機　能：リストビューの項目を作成する
        // 引　数：[in]settingItem  作成する項目の設定
        // 戻り値：リストビューの項目
        //=========================================================================================
        private ListViewItem CreateListViewItem(ArchiveAutoPasswordItem settingItem) {
            ListViewItem item = new ListViewItem(settingItem.DisplayName);
            item.SubItems.Add(PASSWORD_DISPLAY_STRING);
            item.ImageIndex = (int)IconImageListID.Icon_ArchivePassword;
            return item;
        }

        //=========================================================================================
        // 機　能：リストビューで選択中の項目が変わったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewPasswordList_SelectedIndexChanged(object sender, EventArgs evt) {
            UpdateButtonState();
        }

        //=========================================================================================
        // 機　能：ボタンの有効/無効の状態を変更する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateButtonState() {
            if (Program.Document.UserSetting.ArchiveAutoPasswordSetting.ItemFull) {
                this.buttonAdd.Enabled = false;
            } else {
                this.buttonAdd.Enabled = true;
            }

            int index = GetSelectedIndex();
            int itemCount = this.listViewPasswordList.Items.Count;
            if (index == -1 || itemCount == 0) {
                this.buttonDelete.Enabled = false;
                this.buttonUp.Enabled = false;
                this.buttonDown.Enabled = false;
            } else {
                this.buttonDelete.Enabled = true;
                if (index == 0) {
                    this.buttonUp.Enabled = false;
                } else {
                    this.buttonUp.Enabled = true;
                }
                if (index == itemCount - 1) {
                    this.buttonDown.Enabled = false;
                } else {
                    this.buttonDown.Enabled = true;
                }
            }
        }

        //=========================================================================================
        // 機　能：選択中の項目のインデックスを取得する
        // 引　数：なし
        // 戻り値：選択中の項目の0から始まるインデックス（-1:選択中ではない）
        //=========================================================================================
        private int GetSelectedIndex() {
            if (this.listViewPasswordList.SelectedIndices.Count == 0) {
                return -1;
            } else {
                return this.listViewPasswordList.SelectedIndices[0];
            }
        }

        //=========================================================================================
        // 機　能：追加ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAdd_Click(object sender, EventArgs evt) {
            if (Program.Document.UserSetting.ArchiveAutoPasswordSetting.ItemFull) {
                return;
            }

            // 新しいパスワードを入力
            ArchivePasswordAddDialog addDialog = new ArchivePasswordAddDialog();
            DialogResult result = addDialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }
            if (Program.Document.UserSetting.ArchiveAutoPasswordSetting.ItemFull) {
                return;
            }
            ArchiveAutoPasswordItem settingItem = addDialog.NewItem;
            ListViewItem item = CreateListViewItem(settingItem);

            // 一覧に反映
            int index = GetSelectedIndex();
            if (index == -1) {
                this.listViewPasswordList.Items.Add(item);
                Program.Document.UserSetting.ArchiveAutoPasswordSetting.AutoPasswordItemList.Add(settingItem);
            } else {
                this.listViewPasswordList.Items.Insert(index, item);
                Program.Document.UserSetting.ArchiveAutoPasswordSetting.AutoPasswordItemList.Insert(index, settingItem);
            }
            
            UpdateButtonState();
        }

        //=========================================================================================
        // 機　能：削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            int index = GetSelectedIndex();
            if (index == -1) {
                return;
            }

            // 削除の確認
            DialogResult result = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgArcPassMng_ConfirmDelete);
            if (result != DialogResult.Yes) {
                return;
            }

            // 一覧から削除
            this.listViewPasswordList.Items.RemoveAt(index);
            Program.Document.UserSetting.ArchiveAutoPasswordSetting.AutoPasswordItemList.RemoveAt(index);
            UpdateButtonState();
        }

        //=========================================================================================
        // 機　能：上へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonUp_Click(object sender, EventArgs evt) {
            int index = GetSelectedIndex();
            if (index == -1 || index == 0) {
                return;
            }
            ListViewItem itemTemp = this.listViewPasswordList.Items[index - 1];
            this.listViewPasswordList.Items.RemoveAt(index - 1);
            this.listViewPasswordList.Items.Insert(index, itemTemp);
            List<ArchiveAutoPasswordItem> list = Program.Document.UserSetting.ArchiveAutoPasswordSetting.AutoPasswordItemList;
            ArchiveAutoPasswordItem settingTemp = list[index - 1];
            list.RemoveAt(index - 1);
            list.Insert(index, settingTemp);
            UpdateButtonState();
        }

        //=========================================================================================
        // 機　能：下へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs evt) {
            int index = GetSelectedIndex();
            int count = this.listViewPasswordList.Items.Count;
            if (index >= count - 1) {
                return;
            }
            ListViewItem itemTemp = this.listViewPasswordList.Items[index + 1];
            this.listViewPasswordList.Items.RemoveAt(index + 1);
            this.listViewPasswordList.Items.Insert(index, itemTemp);
            List<ArchiveAutoPasswordItem> list = Program.Document.UserSetting.ArchiveAutoPasswordSetting.AutoPasswordItemList;
            ArchiveAutoPasswordItem settingTemp = list[index + 1];
            list.RemoveAt(index + 1);
            list.Insert(index, settingTemp);
            UpdateButtonState();
        }
        
        //=========================================================================================
        // 機　能：パスワード保存のリスク説明のリンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            InfoBox.Information(this, Resources.Dlg_PasswordRisk);
        }

        //=========================================================================================
        // 機　能：閉じるボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            Program.Document.UserSetting.ArchiveAutoPasswordSetting.SaveData();
            Close();
        }
    }
}
