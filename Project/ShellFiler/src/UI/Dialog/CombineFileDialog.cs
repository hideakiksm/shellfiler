using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイルの結合ダイアログ
    //=========================================================================================
    public partial class CombineFileDialog : Form {
        // ダイアログ一覧処理の実装
        private DeleteExStartDialog.ListImpl m_listImpl;

        // 対象のファイル一覧
        private List<UIFile> m_fileList;

        // 反対パスのファイル一覧
        private UIFileList m_oppositeFileList;

        // 入力された結合先ファイル名
        private string m_combineFileName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileList          対象パスのファイル一覧
        // 　　　　[in]oppositeFileList  反対パスのファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public CombineFileDialog(UIFileList fileList, UIFileList oppositeFileList) {
            InitializeComponent();

            m_oppositeFileList = oppositeFileList;

            // マークファイルを抽出
            m_fileList = fileList.MarkFilesExceptFolder;       // 事前チェック済みのため1件以上あるはず

            // 一覧を作成
            m_listImpl = new DeleteExStartDialog.ListImpl(listViewTarget, null, null);
            m_listImpl.InitializeByMarkFile(m_fileList);
            this.listViewTarget.Items[0].Selected = true;

            // 転送先ファイル名を決定
            CombineDefaultFileType fileType = Configuration.Current.CombineDefaultFileType;
            if (fileType == CombineDefaultFileType.Specified) {
                this.textBoxDestName.Text = Configuration.Current.CombineDefaultFileName;
            } else if (fileType == CombineDefaultFileType.FirstMark) {
                string firstFile = m_fileList[0].FileName;
                this.textBoxDestName.Text = firstFile;
            } else if (fileType == CombineDefaultFileType.FirstMark) {
                string prevFileName = Program.Document.UserGeneralSetting.CombineDefaultFileName;
                this.textBoxDestName.Text = prevFileName;
            }
            this.textBoxDestFolder.Text = oppositeFileList.DisplayDirectoryName;

            this.ActiveControl = this.textBoxDestName;
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            int selectedIndex = GetListSelectedIndex();
            this.buttonUp.Enabled = (selectedIndex > 0);
            this.buttonDown.Enabled = (selectedIndex != -1 && selectedIndex < this.listViewTarget.Items.Count - 1);
        }

        //=========================================================================================
        // 機　能：一覧の選択中インデックスを返す
        // 引　数：なし
        // 戻り値：インデックス（選択されていないとき-1）
        //=========================================================================================
        private int GetListSelectedIndex() {
            if (this.listViewTarget.SelectedIndices.Count == 0) {
                return -1;
            }
            int index = this.listViewTarget.SelectedIndices[0];
            return index;
        }

        //=========================================================================================
        // 機　能：上へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonUp_Click(object sender, EventArgs evt) {
            // 設定を入れ替え
            int index = GetListSelectedIndex();
            if (index <= 0) {
                return;
            }
            UIFile targetItem = m_fileList[index];
            m_fileList[index] = m_fileList[index - 1];
            m_fileList[index - 1] = targetItem;

            // 設定をUIに反映
            ListViewItem itemTemp = this.listViewTarget.Items[index];
            this.listViewTarget.Items.RemoveAt(index);
            this.listViewTarget.Items.Insert(index - 1, itemTemp);

            this.listViewTarget.Items[index - 1].Selected = true;
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：下へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs evt) {
            // 設定を入れ替え
            int index = GetListSelectedIndex();
            if (index == this.listViewTarget.Items.Count - 1) {
                return;
            }
            UIFile targetItem = m_fileList[index];
            m_fileList[index] = m_fileList[index + 1];
            m_fileList[index + 1] = targetItem;

            // 設定をUIに反映
            ListViewItem itemTemp = this.listViewTarget.Items[index];
            this.listViewTarget.Items.RemoveAt(index);
            this.listViewTarget.Items.Insert(index + 1, itemTemp);

            this.listViewTarget.Items[index + 1].Selected = true;
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：リストビューの項目の選択が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewTarget_SelectedIndexChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：名前順ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonSortName_Click(object sender, EventArgs evt) {
            FileListSortMode sortMode = new FileListSortMode();
            sortMode.SortOrder1 = FileListSortMode.Method.FileName;
            UIFileSorter sorter = new UIFileSorter(sortMode);
            sorter.ExecSort(m_fileList);
            m_listImpl.InitializeByMarkFile(m_fileList);
        }

        //=========================================================================================
        // 機　能：マーク順ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonSortMark_Click(object sender, EventArgs evt) {
            UIMarkFileSorter sorter = new UIMarkFileSorter();
            m_fileList = sorter.ExecSort(m_fileList);
            m_listImpl.InitializeByMarkFile(m_fileList);
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            // 結合ファイルのファイル名
            string combineFileName = this.textBoxDestName.Text;
            if (combineFileName == "") {
                InfoBox.Warning(this, Resources.DlgCombine_NoDestFile);
                return;
            }
            
            // 結合ファイルのファイル名重複確認
            if (FileSystemID.IgnoreCaseFolderPath(m_oppositeFileList.FileSystem.FileSystemId)) {
                foreach (UIFile file in m_oppositeFileList.Files) {
                    if (file.FileName.Equals(combineFileName, StringComparison.CurrentCultureIgnoreCase)) {
                        InfoBox.Warning(this, Resources.DlgCombine_ExistFileName);
                        return;
                    }
                }
            } else {
                foreach (UIFile file in m_oppositeFileList.Files) {
                    if (file.FileName == combineFileName) {
                        InfoBox.Warning(this, Resources.DlgCombine_ExistFileName);
                        return;
                    }
                }
            }

            // 設定
            m_combineFileName = combineFileName;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：対象のファイル一覧
        //=========================================================================================
        public List<UIFile> FileList {
            get {
                return m_fileList;
            }
        }

        //=========================================================================================
        // プロパティ：入力された結合先ファイル名
        //=========================================================================================
        public string CombineFileName {
            get {
                return m_combineFileName;
            }
        }
    }
}
