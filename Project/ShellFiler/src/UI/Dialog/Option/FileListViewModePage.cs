using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル一覧＞起動時の表示モード の設定ページ
    //=========================================================================================
    public partial class FileListViewModePage : UserControl, IOptionDialogPage {
        // 親ダイアログ
        private OptionSettingDialog m_parent;

        // フォルダ変更時のUIの実装
        private FileListViewModeInitPage.UIImpl m_uiLeftImpl;

        // アイコン用のイメージリスト
        private ImageList m_imageList;

        // 編集中の設定
        private FileListViewModeAutoSetting m_autoSetting;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileListViewModePage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;

            // UIのセットを定義
            m_uiLeftImpl = new FileListViewModeInitPage.UIImpl(
                                    this.radioButtonLeftPrev, this.radioButtonLeftDetail, this.radioButtonLeftThumb,
                                    this.comboBoxLeftThumbSize, this.comboBoxLeftThumbName, this.panelLeftViewSample);

            // イメージリストを初期化
            FileIconManager iconManager = Program.Document.FileIconManager;
            FileIcon icon = iconManager.GetFileIcon(iconManager.DefaultFolderIconId, FileIconID.NullId, FileListViewIconSize.IconSize16);
            m_imageList = new ImageList();
            m_imageList.ColorDepth = ColorDepth.Depth32Bit;
            m_imageList.ImageSize = new Size(UIIconManager.CxDefaultIcon, UIIconManager.CyDefaultIcon);
            m_imageList.Images.Add(icon.IconImage);

            // コンフィグ値をUIに反映
            SetInitialValue(Configuration.Current);
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
            m_imageList.Dispose();
        }

        //=========================================================================================
        // 機　能：UIに初期値を設定する
        // 引　数：[in]config  取得対象のコンフィグ
        // 戻り値：なし
        //=========================================================================================
        private void SetInitialValue(Configuration config) {
            m_uiLeftImpl.ViewModeToUi(config.FileListViewChangeMode, true);

            // リストボックスを初期化
            m_autoSetting = (FileListViewModeAutoSetting)(Configuration.Current.FileListViewModeAutoSetting.Clone());
            this.listViewFolder.Items.Clear();
            this.listViewFolder.SmallImageList = m_imageList;
            List<FileListViewModeAutoSetting.ModeEntry> settingList = m_autoSetting.FolderSetting;
            for (int i = 0; i < settingList.Count; i++) {
                ListViewItem item = CreateListViewItem(settingList[i]);
                this.listViewFolder.Items.Add(item);
            }

            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：リストビューの項目を作成する
        // 引　数：[in]setting  作成する設定項目
        // 戻り値：リストビューの項目
        //=========================================================================================
        private ListViewItem CreateListViewItem(FileListViewModeAutoSetting.ModeEntry setting) {
            string[] strItem = new string[2];
            strItem[0] = setting.FolderName;
            strItem[1] = setting.ViewMode.ToDisplayString();
            ListViewItem item = new ListViewItem(strItem);
            item.ImageIndex = 0;
            return item;
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            Configuration.Current.FileListViewChangeMode = m_uiLeftImpl.UIToViewMode();
            Configuration.Current.FileListViewModeAutoSetting = (FileListViewModeAutoSetting)(m_autoSetting.Clone());
            return true;
        }

        //=========================================================================================
        // 機　能：ページ内の設定をデフォルトに戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetDefault() {
            DialogResult result = InfoBox.Question(m_parent, MessageBoxButtons.OKCancel, Resources.DlgViewModeAuto_ResetConfirm);
            if (result != DialogResult.OK) {
                return;
            }

            Configuration org = new Configuration();
            SetInitialValue(org);
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            int index = GetFolderListIndex();
            this.buttonFolderAdd.Enabled = true;
            this.buttonFolderEdit.Enabled = (index != -1);
            this.buttonFolderDelete.Enabled = (index != -1);
            this.buttonFolderUp.Enabled = (index != -1 && index != 0);
            this.buttonFolderDown.Enabled = (index != -1 && index != this.listViewFolder.Items.Count - 1);
        }

        //=========================================================================================
        // 機　能：フォルダ一覧の選択中のインデックスを返す
        // 引　数：なし
        // 戻り値：ファイル一覧でのインデックス（選択されていないとき-1）
        //=========================================================================================
        private int GetFolderListIndex() {
            ListView.SelectedIndexCollection collection = this.listViewFolder.SelectedIndices;
            if (collection.Count != 1) {
                return -1;
            }
            return collection[0];
        }

        //=========================================================================================
        // 機　能：追加ボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonFolderAdd_Click(object sender, EventArgs evt) {
            FileListViewModeEditDialog dialog = new FileListViewModeEditDialog(m_autoSetting, -1);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }
            m_autoSetting.FolderSetting.Add(dialog.ResultSetting);
            this.listViewFolder.Items.Add(CreateListViewItem(dialog.ResultSetting));
            this.listViewFolder.Items[this.listViewFolder.Items.Count - 1].Selected = true;
        }

        //=========================================================================================
        // 機　能：編集ボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonFolderEdit_Click(object sender, EventArgs evt) {
            int index = GetFolderListIndex();
            if (index == -1) {
                return;
            }
            FileListViewModeEditDialog dialog = new FileListViewModeEditDialog(m_autoSetting, index);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }
            FileListViewModeAutoSetting.ModeEntry setting = dialog.ResultSetting;
            m_autoSetting.FolderSetting[index].FolderName = setting.FolderName;
            m_autoSetting.FolderSetting[index].ViewMode = setting.ViewMode;
            this.listViewFolder.Items[index].SubItems[0].Text = setting.FolderName;
            this.listViewFolder.Items[index].SubItems[1].Text = setting.ViewMode.ToDisplayString();
        }

        //=========================================================================================
        // 機　能：削除ボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonFolderDelete_Click(object sender, EventArgs evt) {
            int index = GetFolderListIndex();
            if (index == -1) {
                return;
            }

            DialogResult result = InfoBox.Question(m_parent, MessageBoxButtons.OKCancel, Resources.DlgViewModeAuto_DeleteConfirm);
            if (result != DialogResult.OK) {
                return;
            }

            m_autoSetting.FolderSetting.RemoveAt(index);
            this.listViewFolder.Items.RemoveAt(index);
            if (this.listViewFolder.Items.Count > 0) {
                index = Math.Min(index, this.listViewFolder.Items.Count - 1);
                this.listViewFolder.Items[index].Selected = true;
            }
        }

        //=========================================================================================
        // 機　能：上へボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonFolderUp_Click(object sender, EventArgs evt) {
            int index = GetFolderListIndex();
            if (index == -1 || index == 0) {
                return;
            }
            FileListViewModeAutoSetting.ModeEntry temp = m_autoSetting.FolderSetting[index];
            m_autoSetting.FolderSetting.RemoveAt(index);
            m_autoSetting.FolderSetting.Insert(index - 1, temp);

            ListViewItem tempItem = this.listViewFolder.Items[index];
            this.listViewFolder.Items.RemoveAt(index);
            this.listViewFolder.Items.Insert(index - 1, tempItem);
        }

        //=========================================================================================
        // 機　能：下へボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonFolderDown_Click(object sender, EventArgs evt) {
            int index = GetFolderListIndex();
            if (index == -1 || index == this.listViewFolder.Items.Count - 1) {
                return;
            }
            FileListViewModeAutoSetting.ModeEntry temp = m_autoSetting.FolderSetting[index];
            m_autoSetting.FolderSetting.RemoveAt(index);
            m_autoSetting.FolderSetting.Insert(index + 1, temp);

            ListViewItem tempItem = this.listViewFolder.Items[index];
            this.listViewFolder.Items.RemoveAt(index);
            this.listViewFolder.Items.Insert(index + 1, tempItem);
        }

        //=========================================================================================
        // 機　能：一覧の選択状態が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewFolder_SelectedIndexChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }
    }
}
