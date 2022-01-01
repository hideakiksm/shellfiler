using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Condition;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：条件を指定して選択コマンド
    //=========================================================================================
    public partial class MarkWithConditionsDialog : Form {
        // 条件入力のUI
        private DeleteExStartDialog.ConditionImpl m_conditionImpl;

        // ファイル転送条件の設定（元の設定）
        private FileConditionSetting m_settingOld;

        // 入力されたマーク方法
        private MarkAllFileMode m_markMode;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileSystem   対象一覧のファイルシステムID
        // 戻り値：なし
        //=========================================================================================
        public MarkWithConditionsDialog(FileSystemID fileSystem) {
            InitializeComponent();

            m_settingOld = Program.Document.FileConditionSetting;
            m_settingOld.LoadSetting();
            FileConditionSetting setting = (FileConditionSetting)(m_settingOld.Clone());
            MarkConditionsDialogInfo markInfo = setting.MarkConditionsDialogInfo;

            m_conditionImpl = new DeleteExStartDialog.ConditionImpl(
                        this, setting, markInfo.ConditionDialogInfo, fileSystem,
                        null, this.radioButtonSetting, this.radioButtonWild,
                        this.checkedListCondition, this.buttonSetting, this.textBoxWildCard);

            if (markInfo.MarkMode == MarkAllFileMode.SelectAll) {
                this.radioButtonSelectSet.Checked = true;
            } else if (markInfo.MarkMode == MarkAllFileMode.ClearAll) {
                this.radioButtonSelectClear.Checked = true;
            } else {
                this.radioButtonSelectRevert.Checked = true;
            }

            if (markInfo.ConditionDialogInfo.ConditionMode) {
                this.ActiveControl = this.checkedListCondition;
            } else {
                this.ActiveControl = this.textBoxWildCard;
            }
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MarkWithConditionsDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            if (!FileConditionSetting.EqualsConfig(m_settingOld, m_conditionImpl.FileConditionSetting)) {
                m_conditionImpl.FileConditionSetting.SaveSetting();
                Program.Document.FileConditionSetting = m_conditionImpl.FileConditionSetting;
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            bool success = m_conditionImpl.OnOk(FileConditionTarget.FileAndFolder);
            if (!success) {
                return;
            }
            
            // マーク操作の種別を取得
            if (this.radioButtonSelectRevert.Checked) {
                m_markMode = MarkAllFileMode.RevertAll;
            } else if (this.radioButtonSelectSet.Checked) {
                m_markMode = MarkAllFileMode.SelectAll;
            } else {
                m_markMode = MarkAllFileMode.ClearAll;
            }
            m_conditionImpl.FileConditionSetting.MarkConditionsDialogInfo.MarkMode = m_markMode;

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：入力された条件
        //=========================================================================================
        public List<FileConditionItem> ResultConditionList {
            get {
                return m_conditionImpl.ResultConditionList;
            }
        }

        //=========================================================================================
        // プロパティ：入力されたマーク方法
        //=========================================================================================
        public MarkAllFileMode MarkMode {
            get {
                return m_markMode;
            }
        }
    }
}
