using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Virtual;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ローカル実行 編集結果更新確認ダイアログ
    //=========================================================================================
    public partial class LocalExecuteEditEndDialog : Form {
        // ダイアログの結果
        private UpdateResult m_result = UpdateResult.Cancel;

        // プロパティ：操作対象の作業領域の情報
        private LocalExecuteTemporarySpace m_localExecuteTemporarySpace;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]space      作業領域の情報
        // 　　　　[in]fileList   ローカルのファイル一覧
        // 　　　　[in]updateList 更新されたファイルの一覧
        // 戻り値：なし
        //=========================================================================================
        public LocalExecuteEditEndDialog(LocalExecuteTemporarySpace space, List<LocalFileInfo> fileList, List<LocalFileInfo> updateList) {
            InitializeComponent();
            m_localExecuteTemporarySpace = space;
            this.pictureBoxIcon.Image = SystemIcons.Question.ToBitmap();
            this.labelTitle.Text = space.DisplayNameInfo.Text;

            // 更新ファイル一覧をSetに変換
            HashSet<string> updateSet = new HashSet<string>();
            foreach (LocalFileInfo file in updateList) {
                updateSet.Add(file.FilePath.ToLower());
            }

            // リストボックスを初期化
            this.columnHeader.Width = -1;
            this.listViewLocalFile.SmallImageList = UIIconManager.IconImageList;
            foreach (LocalFileInfo fileInfo in fileList) {
                string virtualPath = GenericFileStringUtils.CompleteDirectoryName(space.VirtualDirectory, "\\");
                string remotePath = fileInfo.FilePath.Substring(virtualPath.Length);
                ListViewItem item = new ListViewItem(remotePath);
                if (updateSet.Contains(fileInfo.FilePath.ToLower())) {
                    item.ImageIndex = (int)IconImageListID.Icon_FileAttributeDetailYes;
                } else {
                    item.ImageIndex = (int)IconImageListID.Icon_FileAttributeDetailNo;
                }
                item.Tag = fileInfo.FilePath.ToLower();
                this.listViewLocalFile.Items.Add(item);
            }
        }

        //=========================================================================================
        // 機　能：更新されたファイルの一覧を最新にする
        // 引　数：[in]updateList 更新されたファイルの一覧
        // 戻り値：なし
        //=========================================================================================
        public void UpdateModifiedList(List<LocalFileInfo> updateList) {
            // 更新ファイル一覧をSetに変換
            HashSet<string> updateSet = new HashSet<string>();
            foreach (LocalFileInfo file in updateList) {
                updateSet.Add(file.FilePath.ToLower());
            }

            // リストの一覧を比較
            foreach (ListViewItem item in this.listViewLocalFile.Items) {
                string virtualPath = (string)(item.Tag);
                if (updateSet.Contains(virtualPath)) {
                    item.ImageIndex = (int)IconImageListID.Icon_FileAttributeDetailYes;
                }
            }
        }

        //=========================================================================================
        // 機　能：更新ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonYes_Click(object sender, EventArgs evt) {
            m_result = UpdateResult.Update;
            Close();
        }

        //=========================================================================================
        // 機　能：破棄ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonNo_Click(object sender, EventArgs evt) {
            DialogResult result = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgLocalExec_ConfirmDelete);
            if (result != DialogResult.Yes) {
                return;
            }

            m_result = UpdateResult.Delete;
            Close();
        }

        //=========================================================================================
        // プロパティ：ダイアログの結果
        //=========================================================================================
        public UpdateResult Result {
            get {
                return m_result;
            }
        }

        //=========================================================================================
        // プロパティ：操作対象の作業領域の情報
        //=========================================================================================
        public LocalExecuteTemporarySpace TemporarySpace {
            get {
                return m_localExecuteTemporarySpace;
            }
        }

        //=========================================================================================
        // 列挙子：更新の種類
        //=========================================================================================
        public enum UpdateResult {
            // 更新を実行
            Update,
            // 更新を破棄
            Delete,
            // キャンセル
            Cancel,
        }
    }
}
