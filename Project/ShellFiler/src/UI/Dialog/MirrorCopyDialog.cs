using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ミラーコピーの設定ダイアログ
    //=========================================================================================
    public partial class MirrorCopyDialog : Form {
        // ダイアログ一覧処理の実装
        private DeleteExStartDialog.ListImpl m_listImpl;

        // 属性を設定するかどうかのデフォルト
        private AttributeSetMode m_attrSetModeDefault;

        // 転送元のファイルシステム
        private FileSystemID m_srcFileSystem;

        // 転送先のファイルシステム
        private FileSystemID m_destFileSystem;

        // 入力したオプション
        private MirrorCopyOption m_mirrorCopyOption;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]attrSetModeDefault  属性コピーのデフォルト設定
        // 　　　　[in]srcFileSystem       転送元のファイルシステム
        // 　　　　[in]destFileSystem      転送先のファイルシステム
        // 戻り値：なし
        //=========================================================================================
        public MirrorCopyDialog(AttributeSetMode attrSetModeDefault, FileSystemID srcFileSystem, FileSystemID destFileSystem) {
            InitializeComponent();
            m_attrSetModeDefault = attrSetModeDefault;
            m_srcFileSystem = srcFileSystem;
            m_destFileSystem = destFileSystem;
            m_listImpl = new DeleteExStartDialog.ListImpl(this.listViewTarget, this.labelMessage, Resources.DlgOperationStart_MirrorCopy);

            this.checkBoxAttrCopy.CheckState = CheckState.Indeterminate;
            string attrOnOff = CopyExStartDialog.EtcOptionImpl.GetAttributeUI(m_attrSetModeDefault, m_destFileSystem);
            this.labelAttrMessage.Text = string.Format(this.labelAttrMessage.Text, attrOnOff);
            this.radioButtonIfNewer.Checked = true;
            this.ActiveControl = this.radioButtonIfNewer;
            this.textBoxExcept.Text = Configuration.Current.MirrorCopyExceptFiles;
            this.pictureBoxIcon.Image = UIIconManager.IconImageList.Images[(int)(IconImageListID.FileList_MirrorCopy)];
            if (Configuration.Current.DeleteFileOptionDefault == null) {
                this.checkBoxDirectory.Checked = Program.Document.UserGeneralSetting.DeleteFileOption.DeleteDirectoryAll;
                this.checkBoxAttr.Checked = Program.Document.UserGeneralSetting.DeleteFileOption.DeleteSpecialAttrAll;
            } else {
                this.checkBoxDirectory.Checked = Configuration.Current.DeleteFileOptionDefault.DeleteDirectoryAll;
                this.checkBoxAttr.Checked = Configuration.Current.DeleteFileOptionDefault.DeleteSpecialAttrAll;
            }
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：ファイル一覧から初期化する
        // 引　数：[in]targetList  対象パスのファイルリスト
        // 　　　　[in]oppList     反対パスのファイルリスト
        // 戻り値：なし
        //=========================================================================================
        public void InitializeByMarkFile(UIFileList targetList, UIFileList oppList) {
            m_listImpl.InitializeByMarkFile(targetList.MarkFiles);
            this.textBoxDest.Text = oppList.DisplayDirectoryName;
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            this.labelDirectory.Visible = !this.checkBoxDirectory.Checked;
            this.labelAttr.Visible = !this.checkBoxAttr.Checked;
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MirrorCopyDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            m_listImpl.OnFormClosed();
        }
        
        //=========================================================================================
        // 機　能：削除関連のチェックボックスがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxDelete_CheckedChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            MirrorCopyOption.MirrorCopyTransferMode transferMode;
            if (this.radioButtonForce.Checked) {
                transferMode = MirrorCopyOption.MirrorCopyTransferMode.ForceOverwrite;
            } else if (this.radioButtonIfNewer.Checked) {
                transferMode = MirrorCopyOption.MirrorCopyTransferMode.OverwriteIfNewer;
            } else if (this.radioButtonNotOverwrite.Checked) {
                transferMode = MirrorCopyOption.MirrorCopyTransferMode.NotOverwrite;
            } else {
                transferMode = MirrorCopyOption.MirrorCopyTransferMode.DifferenceAttribute;
            }

            AttributeSetMode attributeSetMode = new AttributeSetMode();
            if (this.checkBoxAttrCopy.CheckState == CheckState.Indeterminate) {
                attributeSetMode = (AttributeSetMode)(m_attrSetModeDefault.Clone());
            } else {
                attributeSetMode.SshSetAtributeAll = this.checkBoxAttrCopy.Checked;
                attributeSetMode.WindowsSetAttributeAll = this.checkBoxAttrCopy.Checked;
            }

            List<string> exceptFileList = new List<string>();
            string[] exceptFileListArray = this.textBoxExcept.Text.Split(':');
            exceptFileList.AddRange(exceptFileListArray);

            DeleteFileOption deleteFileOption = new DeleteFileOption();
            deleteFileOption.DeleteDirectoryAll = this.checkBoxDirectory.Checked;
            deleteFileOption.DeleteSpecialAttrAll = this.checkBoxAttr.Checked;

            bool useRecycleBin = this.checkBoxWithRecycle.Checked;

            m_mirrorCopyOption = new MirrorCopyOption();
            m_mirrorCopyOption.TransferMode = transferMode;
            m_mirrorCopyOption.AttributeSetMode = attributeSetMode;
            m_mirrorCopyOption.ExceptFileList = exceptFileList;
            m_mirrorCopyOption.DeleteFileOption = deleteFileOption;
            m_mirrorCopyOption.UseRecycleBin = useRecycleBin;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：入力したオプション
        //=========================================================================================
        public MirrorCopyOption MirrorCopyOption {
            get {
                return m_mirrorCopyOption;
            }
        }
    }
}
