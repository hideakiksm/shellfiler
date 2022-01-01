using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Condition;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：方式を指定して移動開始時の確認ダイアログ
    //=========================================================================================
    public partial class MoveExStartDialog : Form {
        // ダイアログ一覧処理の実装
        private DeleteExStartDialog.ListImpl m_listImpl;

        // 転送条件の入力の実装
        private DeleteExStartDialog.ConditionImpl m_conditionImpl;

        // その他オプションの入力の実装
        private CopyExStartDialog.EtcOptionImpl m_etcOptionImpl;

        // 編集前のファイル転送条件の設定
        private FileConditionSetting m_settingOld;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcFileSystem   転送元のファイルシステム
        // 　　　　[in]destFileSystem  転送先のファイルシステム
        // 戻り値：なし
        //=========================================================================================
        public MoveExStartDialog(FileSystemID srcFileSystem, FileSystemID destFileSystem) {
            InitializeComponent();

            m_settingOld = Program.Document.FileConditionSetting;
            m_settingOld.LoadSetting();
            FileConditionSetting setting = (FileConditionSetting)(m_settingOld.Clone());
            m_listImpl = new DeleteExStartDialog.ListImpl(this.listViewTarget, this.labelMessage, Resources.DlgOperationStart_Move);
            m_conditionImpl = new DeleteExStartDialog.ConditionImpl(
                        this, setting, setting.TransferConditionDialogInfo, srcFileSystem,
                        this.checkBoxCondition, this.radioButtonSetting, this.radioButtonWild,
                        this.checkedListCondition, this.buttonSetting, this.textBoxWildCard);
            m_etcOptionImpl = new CopyExStartDialog.EtcOptionImpl(this, destFileSystem, this.checkBoxAttrCopy, this.labelAttrCopy, this.checkBoxSuspend);
            this.pictureBoxIcon.Image = UIIconManager.IconImageList.Images[(int)(IconImageListID.FileList_Move)];
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
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            bool success = m_conditionImpl.OnOk(FileConditionTarget.FileOnly);
            if (!success) {
                return;
            }
            success = m_etcOptionImpl.OnOk();
            if (!success) {
                return;
            }

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
            if (!FileConditionSetting.EqualsConfig(m_settingOld, m_conditionImpl.FileConditionSetting)) {
                m_conditionImpl.FileConditionSetting.SaveSetting();
            }
            m_listImpl.OnFormClosed();
        }
        
        //=========================================================================================
        // プロパティ：入力した転送条件
        //=========================================================================================
        public CompareCondition TransferCondition {
            get {
                if (m_conditionImpl.ResultConditionList != null) {
                    return new CompareCondition(m_conditionImpl.ResultConditionList);
                } else {
                    return null;
                }
            }
        }

        //=========================================================================================
        // プロパティ：ファイル転送時に属性の転送を行うかどうかのモード
        //=========================================================================================
        public AttributeSetMode AttributeSetMode {
            get {
                return m_etcOptionImpl.AttributeSetMode;
            }
        }

        //=========================================================================================
        // プロパティ：待機状態のタスクを作成するときtrue
        //=========================================================================================
        public bool SuspededTask {
            get {
                return m_etcOptionImpl.SuspededTask;
            }
        }
    }
}
