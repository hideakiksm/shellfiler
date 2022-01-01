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
    // クラス：方式を指定してコピー開始時の確認ダイアログ
    //=========================================================================================
    public partial class CopyExStartDialog : Form {
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
        public CopyExStartDialog(FileSystemID srcFileSystem, FileSystemID destFileSystem) {
            InitializeComponent();

            m_settingOld = Program.Document.FileConditionSetting;
            m_settingOld.LoadSetting();
            FileConditionSetting setting = (FileConditionSetting)(m_settingOld.Clone());
            m_listImpl = new DeleteExStartDialog.ListImpl(this.listViewTarget, this.labelMessage, Resources.DlgOperationStart_Copy);
            m_conditionImpl = new DeleteExStartDialog.ConditionImpl(
                        this, setting, setting.TransferConditionDialogInfo, srcFileSystem,
                        this.checkBoxCondition, this.radioButtonSetting, this.radioButtonWild,
                        this.checkedListCondition, this.buttonSetting, this.textBoxWildCard);
            m_etcOptionImpl = new CopyExStartDialog.EtcOptionImpl(this, destFileSystem, this.checkBoxAttrCopy, this.labelAttrCopy, this.checkBoxSuspend);
            this.pictureBoxIcon.Image = UIIconManager.IconImageList.Images[(int)(IconImageListID.FileList_Copy)];
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
            bool success;
            success = m_conditionImpl.OnOk(FileConditionTarget.FileOnly);
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

        //=========================================================================================
        // クラス：その他オプションの入力の実装
        //=========================================================================================
        public class EtcOptionImpl {
            // 親となるフォーム
            private Form m_parent;

            // 対象のファイルシステムID
            private FileSystemID m_destFileSystem;

            // 属性コピー有無のチェックボックス
            private CheckBox m_checkBoxAttrCopy;

            // 属性コピーのデフォルト値
            private AttributeSetMode m_defaultAttributeSetMode;

            // 属性コピーの入力結果
            private AttributeSetMode m_resultAttributeSetMode;

            // 待機状態のタスクのチェックボックス
            private CheckBox m_checkBoxSuspended;

            // 待機状態のタスクを作成するときtrue
            private bool m_suspededTask;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent            親となるフォーム
            // 　　　　[in]destFileSystem    転送先のファイルシステムID
            // 　　　　[in]checkBoxAttrCopy  属性コピー有無のチェックボックス
            // 　　　　[in]labelAttrCopy     属性コピー有無のデフォルト状態のメッセージを表示するラベル
            // 　　　　[in]suspendedTask     待機状態のタスクを作成するかどうかのチェックボックス
            // 戻り値：ダイアログを閉じてよいときtrue
            //=========================================================================================
            public EtcOptionImpl(Form parent, FileSystemID destFileSystem, CheckBox checkBoxAttrCopy, Label labelAttrCopy, CheckBox suspendedTask) {
                m_parent = parent;
                m_destFileSystem = destFileSystem;
                m_checkBoxAttrCopy = checkBoxAttrCopy;
                m_checkBoxSuspended = suspendedTask;
                m_defaultAttributeSetMode = (AttributeSetMode)(Configuration.Current.TransferAttributeSetMode.Clone());

                // UIを設定
                checkBoxAttrCopy.CheckState = CheckState.Indeterminate;
                string currentSetting = GetAttributeUI(m_defaultAttributeSetMode, destFileSystem);
                labelAttrCopy.Text = string.Format(labelAttrCopy.Text, currentSetting);
            }

            //=========================================================================================
            // 機　能：属性の状態に対して、UIの表記に使用するON/OFFの文字列を返す
            // 引　数：[in]attrMode        属性の設定
            // 　　　　[in]destFileSystem  転送先のファイルシステムID
            // 戻り値：ONまたはOFFの文字列
            //=========================================================================================
            public static string GetAttributeUI(AttributeSetMode attrMode, FileSystemID destFileSystem) {
                string currentSetting;
                if (FileSystemID.IsWindows(destFileSystem)) {
                    currentSetting = attrMode.WindowsSetAttributeAll ? "ON" : "OFF";
                } else if (FileSystemID.IsSSH(destFileSystem)) {
                    currentSetting = attrMode.SshSetAtributeAll ? "ON" : "OFF";
                } else {
                    FileSystemID.NotSupportError(destFileSystem);
                    currentSetting = "";
                }
                return currentSetting;
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：ダイアログを閉じてよいときtrue
            //=========================================================================================
            public bool OnOk() {
                if (FileSystemID.IsWindows(m_destFileSystem)) {
                    m_resultAttributeSetMode = new AttributeSetMode();
                    m_resultAttributeSetMode.SshSetAtributeAll = m_defaultAttributeSetMode.SshSetAtributeAll;
                    BooleanFlag checkState = FormUtils.GetCheckThreeState(m_checkBoxAttrCopy);
                    if (checkState == null) {
                        m_resultAttributeSetMode.WindowsSetAttributeAll = m_defaultAttributeSetMode.WindowsSetAttributeAll;
                    } else {
                        m_resultAttributeSetMode.WindowsSetAttributeAll = checkState.Value;
                    }
                } else if (FileSystemID.IsSSH(m_destFileSystem)) {
                    m_resultAttributeSetMode = new AttributeSetMode();
                    m_resultAttributeSetMode.WindowsSetAttributeAll = m_defaultAttributeSetMode.WindowsSetAttributeAll;
                    BooleanFlag checkState = FormUtils.GetCheckThreeState(m_checkBoxAttrCopy);
                    if (checkState == null) {
                        m_resultAttributeSetMode.SshSetAtributeAll = m_defaultAttributeSetMode.SshSetAtributeAll;
                    } else {
                        m_resultAttributeSetMode.SshSetAtributeAll = checkState.Value;
                    }
                } else {
                    FileSystemID.NotSupportError(m_destFileSystem);
                }
                m_suspededTask = m_checkBoxSuspended.Checked;
                return true;
            }

            //=========================================================================================
            // プロパティ：ファイル転送時に属性の転送を行うかどうかのモード
            //=========================================================================================
            public AttributeSetMode AttributeSetMode {
                get {
                    return m_resultAttributeSetMode;
                }
            }

            //=========================================================================================
            // プロパティ：待機状態のタスクを作成するときtrue
            //=========================================================================================
            public bool SuspededTask {
                get {
                    return m_suspededTask;
                }
            }
        }
    }
}
