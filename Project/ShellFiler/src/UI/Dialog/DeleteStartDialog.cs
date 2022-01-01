using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.FileTask.Provider;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：削除開始時の確認ダイアログ
    //=========================================================================================
    public partial class DeleteStartDialog : Form {
        // ダイアログ一覧処理の実装
        private DeleteExStartDialog.ListImpl m_listImpl;

        // ダイアログ削除処理の実装
        private DeleteExStartDialog.DeleteImpl m_deleteImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dirDirect   ごみ箱/rm -rfを使って削除するときtrue
        // 　　　　[in]fileSystem  対象パスのファイルシステム
        // 戻り値：なし
        //=========================================================================================
        public DeleteStartDialog(bool dirDirect, FileSystemID fileSystem) {
            InitializeComponent();
            m_listImpl = new DeleteExStartDialog.ListImpl(this.listViewTarget, this.labelMessage, Resources.DlgOperationStart_Delete);
            m_deleteImpl = new DeleteExStartDialog.DeleteImpl(dirDirect, fileSystem, checkBoxDirectory, checkBoxAttr, checkBoxWithRecycle);
            this.pictureBoxIcon.Image = UIIconManager.IconImageList.Images[(int)(IconImageListID.FileList_Delete)];
        }

        //=========================================================================================
        // 機　能：ファイル一覧から初期化する
        // 引　数：[in]fileList  確認対象のファイルリスト
        // 戻り値：なし
        //=========================================================================================
        public void InitializeByMarkFile(UIFileList fileList) {
            m_listImpl.InitializeByMarkFile(fileList.MarkFiles);
            m_deleteImpl.InitializeByMarkFile(fileList);
        }

        //=========================================================================================
        // 機　能：再試行情報から初期化する
        // 引　数：[in]retryInfo  確認対象の再試行情報
        // 戻り値：なし
        //=========================================================================================
        public void InitializeByRetryInfo(FileErrorRetryInfo retryInfo) {
            m_listImpl.InitializeByRetryInfo(retryInfo);
            m_deleteImpl.InitializeByRetryInfo(retryInfo);
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_deleteImpl.OnOK();

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void DeleteStartDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            m_listImpl.OnFormClosed();
            m_deleteImpl.OnFormClosed();
        }

        //=========================================================================================
        // プロパティ：削除操作のモード
        //=========================================================================================
        public DeleteFileOption DeleteFileOption {
            get {
                return m_deleteImpl.DeleteFileOption;
            }
        }

        //=========================================================================================
        // プロパティ：ごみ箱を使って削除するときtrue
        //=========================================================================================
        public bool DeleteWithRecycle {
            get {
                return m_deleteImpl.DeleteWithRecycle;
            }
        }
    }
}
