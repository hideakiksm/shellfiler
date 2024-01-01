using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
    // クラス：SSH用転送条件入力ダイアログ
    //=========================================================================================
    public partial class FileConditionSSHDialog : Form {
        // 編集対象の更新条件（元のClone）
        private FileConditionItemSSH m_condition;

        // 編集時にすでに存在する項目のリスト
        private List<FileConditionItem> m_existingList;

        // 編集対象の項目のインデックス（新規作成のとき-1）
        private int m_existingIndex;

        // ドロップダウン 対象
        private LasyComboBoxImpl m_comboBoxImplTarget;

        // ドロップダウン ファイル名
        private LasyComboBoxImpl m_comboBoxImplFileName;

        // ドロップダウン 更新日時
        private LasyComboBoxImpl m_comboBoxImplUpdate;

        // ドロップダウン アクセス日時
        private LasyComboBoxImpl m_comboBoxImplAccess;

        // ドロップダウン ファイルサイズ
        private LasyComboBoxImpl m_comboBoxImplSize;

        // ドロップダウン 所有者 読み込み
        private LasyComboBoxImpl m_comboBoxImplOwnerRead;

        // ドロップダウン 所有者 書き込み
        private LasyComboBoxImpl m_comboBoxImplOwnerWrite;

        // ドロップダウン 所有者 実行
        private LasyComboBoxImpl m_comboBoxImplOwnerExec;

        // ドロップダウン グループ 読み込み
        private LasyComboBoxImpl m_comboBoxImplGroupRead;

        // ドロップダウン グループ書き込み
        private LasyComboBoxImpl m_comboBoxImplGroupWrite;

        // ドロップダウン グループ実行
        private LasyComboBoxImpl m_comboBoxImplGroupExec;

        // ドロップダウン 他人 読み込み
        private LasyComboBoxImpl m_comboBoxImplOtherRead;

        // ドロップダウン 他人書き込み
        private LasyComboBoxImpl m_comboBoxImplOtherWrite;

        // ドロップダウン 他人 実行
        private LasyComboBoxImpl m_comboBoxImplOtherExec;

        // ドロップダウン シンボリックリンク
        private LasyComboBoxImpl m_comboBoxImplSymLink;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public FileConditionSSHDialog(FileConditionItemSSH condition, List<FileConditionItem> existingList, int existingIndex) {
            InitializeComponent();
            m_condition = condition;
            m_existingList = existingList;
            m_existingIndex = existingIndex;

            // UI用に適当な初期値を設定
            m_condition.SetUIDefaultValue();

            // 表示名
            this.textBoxDispName.Text = condition.DisplayName;

            // 対象
            int targetIndex = FileConditionWindowsDialog.TypeComboBoxIndexConverter.FileConditionTargetToIndex(condition.FileConditionTarget);
            m_comboBoxImplTarget = new LasyComboBoxImpl(this.comboBoxTarget, FileConditionWindowsDialog.TypeComboBoxIndexConverter.TargetItems, targetIndex);

            // ファイル名
            int fileNameIndex = FileConditionWindowsDialog.TypeComboBoxIndexConverter.FileNameTypeToIndex(condition.FileNameType);
            m_comboBoxImplFileName = new LasyComboBoxImpl(this.comboBoxFileName, FileConditionWindowsDialog.TypeComboBoxIndexConverter.FileNameItems, fileNameIndex);
            this.textBoxFileName.Text = condition.FileName;

            // 更新日時
            int updateIndex = FileConditionWindowsDialog.TypeComboBoxIndexConverter.DateTimeTypeToIndex(condition.UpdateTimeCondition.TimeType);
            m_comboBoxImplUpdate = new LasyComboBoxImpl(this.comboBoxDateUpdate, FileConditionWindowsDialog.TypeComboBoxIndexConverter.TimeItems, updateIndex);

            // アクセス日時
            int accessIndex = FileConditionWindowsDialog.TypeComboBoxIndexConverter.DateTimeTypeToIndex(condition.AccessTimeCondition.TimeType);
            m_comboBoxImplAccess = new LasyComboBoxImpl(this.comboBoxDateAccess, FileConditionWindowsDialog.TypeComboBoxIndexConverter.TimeItems, accessIndex);

            // ファイルサイズ
            int sizeIndex = FileConditionWindowsDialog.TypeComboBoxIndexConverter.FileSizeTypeToIndex(condition.FileSizeCondition.SizeType);
            m_comboBoxImplSize = new LasyComboBoxImpl(this.comboBoxSize, FileConditionWindowsDialog.TypeComboBoxIndexConverter.FileSizeItems, sizeIndex);

            // 属性
            SetAttributeCombo(this.comboBoxOwnerRead,  out m_comboBoxImplOwnerRead,  condition.OwnerRead);
            SetAttributeCombo(this.comboBoxOwnerWrite, out m_comboBoxImplOwnerWrite, condition.OwnerWrite);
            SetAttributeCombo(this.comboBoxOwnerExec,  out m_comboBoxImplOwnerExec,  condition.OwnerExecute);
            SetAttributeCombo(this.comboBoxGroupRead,  out m_comboBoxImplGroupRead,  condition.GroupRead);
            SetAttributeCombo(this.comboBoxGroupWrite, out m_comboBoxImplGroupWrite, condition.GroupWrite);
            SetAttributeCombo(this.comboBoxGroupExec,  out m_comboBoxImplGroupExec,  condition.GroupExecute);
            SetAttributeCombo(this.comboBoxOtherRead,  out m_comboBoxImplOtherRead,  condition.OtherRead);
            SetAttributeCombo(this.comboBoxOtherWrite, out m_comboBoxImplOtherWrite, condition.OtherWrite);
            SetAttributeCombo(this.comboBoxOtherExec,  out m_comboBoxImplOtherExec,  condition.OtherExecute);
            SetAttributeCombo(this.comboBoxSymLink,    out m_comboBoxImplSymLink,    condition.SymbolicLink);

            // その他初期化
            EnableUIItem();
            FileConditionWindowsDialog.ConditionDialogImpl.UpdateDatePanel(m_comboBoxImplUpdate, this.panelDateUpdate, condition.UpdateTimeCondition);
            FileConditionWindowsDialog.ConditionDialogImpl.UpdateDatePanel(m_comboBoxImplAccess, this.panelDateAccess, condition.AccessTimeCondition);
            FileConditionWindowsDialog.ConditionDialogImpl.UpdateSizePanel(m_comboBoxImplSize, this.panelSize, m_condition.FileSizeCondition);

            // イベントを接続
            this.comboBoxFileName.SelectedIndexChanged += new System.EventHandler(this.comboBoxFileName_SelectedIndexChanged);
            this.comboBoxDateUpdate.SelectedIndexChanged += new System.EventHandler(this.comboBoxDate_SelectedIndexChanged);
            this.comboBoxDateAccess.SelectedIndexChanged += new System.EventHandler(this.comboBoxDate_SelectedIndexChanged);
            this.comboBoxSize.SelectedIndexChanged += new System.EventHandler(this.comboBoxSize_SelectedIndexChanged);
        }

        //=========================================================================================
        // 機　能：属性に対する条件をUIに反映する
        // 引　数：[in]comboBox      コンボボックスのコントロール
        // 　　　　[out]comboBoxImpl 制御に使用するコンボボックスの実装を返す変数
        // 　　　　[in]flag          設定する属性のフラグ
        // 戻り値：なし
        //=========================================================================================
        private void SetAttributeCombo(ComboBox comboBox, out LasyComboBoxImpl comboBoxImpl, BooleanFlag flag) {
            int attributeIndex = FileConditionWindowsDialog.TypeComboBoxIndexConverter.AttributeStateToIndex(flag);
            comboBoxImpl = new LasyComboBoxImpl(comboBox, FileConditionWindowsDialog.TypeComboBoxIndexConverter.AttributeItems, attributeIndex);
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            this.textBoxFileName.Enabled = (m_comboBoxImplFileName.SelectedIndex != 0);
        }

        //=========================================================================================
        // 機　能：ファイル名条件のコンボボックスの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxFileName_SelectedIndexChanged(object sender, EventArgs evt) {
            if (m_comboBoxImplFileName == null) {
                return;
            }
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：日付条件のコンボボックスの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxDate_SelectedIndexChanged(object sender, EventArgs evt) {
            if (m_comboBoxImplUpdate == null || m_comboBoxImplAccess == null) {
                return;
            }
            if (sender == this.comboBoxDateUpdate) {
                FileConditionWindowsDialog.ConditionDialogImpl.UpdateDatePanel(m_comboBoxImplUpdate, this.panelDateUpdate, m_condition.UpdateTimeCondition);
            } else if (sender == this.comboBoxDateAccess) {
                FileConditionWindowsDialog.ConditionDialogImpl.UpdateDatePanel(m_comboBoxImplAccess, this.panelDateAccess, m_condition.AccessTimeCondition);
            }
        }

        //=========================================================================================
        // 機　能：サイズ条件のコンボボックスの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxSize_SelectedIndexChanged(object sender, EventArgs evt) {
            if (m_comboBoxImplSize == null) {
                return;
            }
            if (sender == this.comboBoxSize) {
                FileConditionWindowsDialog.ConditionDialogImpl.UpdateSizePanel(m_comboBoxImplSize, this.panelSize, m_condition.FileSizeCondition);
            }
        }

        //=========================================================================================
        // 機　能：転送条件ヘルプのリンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelTransferHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            HelpMessageDialog dialog = new HelpMessageDialog(Resources.DlgTransferCond_TitleTransferHelp, Resources.HtmlTransferConditionTarget);
            dialog.Width = dialog.Width + 200;
            dialog.ShowDialog(this);
        }

        //=========================================================================================
        // 機　能：ファイル名ヘルプのリンクがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelFileNameHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            RegexTestDialog dialog = new RegexTestDialog();
            dialog.ShowDialog(this);
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            bool success;

            // 表示名
            string dispName = this.textBoxDispName.Text;
            if (dispName == "") {
                InfoBox.Warning(this, Resources.DlgFileCondition_ErrorConditionName);
                return;
            }
            for (int i = 0; i < m_existingList.Count; i++) {
                if (i != m_existingIndex && m_existingList[i].DisplayName == dispName) {
                    InfoBox.Warning(this, Resources.DlgFileCondition_ErrorDuplicateConditionName);
                    return;
                }
            }
            m_condition.DisplayName = dispName;

            // 対象
            FileConditionTarget target = FileConditionWindowsDialog.TypeComboBoxIndexConverter.IndexToFileConditionTarget(m_comboBoxImplTarget.SelectedIndex);
            m_condition.FileConditionTarget = target;

            // ファイル名
            FileNameType fileNameType;
            string fileName;
            success = FileConditionWindowsDialog.ConditionDialogImpl.GetFileNameCondition(this, m_comboBoxImplFileName, this.textBoxFileName, out fileNameType, out fileName);
            if (!success) {
                return;
            }
            m_condition.FileNameType = fileNameType;
            m_condition.FileName = fileName;


            // 更新日時
            success = FileConditionWindowsDialog.ConditionDialogImpl.GetFileDate(this, m_condition.UpdateTimeCondition, this.panelDateUpdate, Resources.DlgFileCondition_ErrorDateUpdate, Resources.DlgFileCondition_ErrorDateUpdateReverse);
            if (!success) {
                return;
            }

            // アクセス日時
            success = FileConditionWindowsDialog.ConditionDialogImpl.GetFileDate(this, m_condition.AccessTimeCondition, this.panelDateAccess, Resources.DlgFileCondition_ErrorDateAccess, Resources.DlgFileCondition_ErrorDateAccessReverse);
            if (!success) {
                return;
            }

            // ファイルサイズ
            success = FileConditionWindowsDialog.ConditionDialogImpl.GetFileSize(this, m_condition.FileSizeCondition, this.panelSize, Resources.DlgFileCondition_ErrorFileSize, Resources.DlgFileCondition_ErrorFileSizeReverse);
            if (!success) {
                return;
            }

            // 属性
            m_condition.OwnerRead    = GetAttribute(m_comboBoxImplOwnerRead);
            m_condition.OwnerWrite   = GetAttribute(m_comboBoxImplOwnerWrite);
            m_condition.OwnerExecute = GetAttribute(m_comboBoxImplOwnerExec);
            m_condition.GroupRead    = GetAttribute(m_comboBoxImplGroupRead);
            m_condition.GroupWrite   = GetAttribute(m_comboBoxImplGroupWrite);
            m_condition.GroupExecute = GetAttribute(m_comboBoxImplGroupExec);
            m_condition.OtherRead    = GetAttribute(m_comboBoxImplOtherRead);
            m_condition.OtherWrite   = GetAttribute(m_comboBoxImplOtherWrite);
            m_condition.OtherExecute = GetAttribute(m_comboBoxImplOtherExec);
            m_condition.SymbolicLink = GetAttribute(m_comboBoxImplSymLink);

            // 入力が空かどうかをチェック
            FileConditionItemSSH check = (FileConditionItemSSH)(m_condition.Clone());
            check.CleanupField();
            if (check.IsEmptyCondition()) {
                InfoBox.Warning(this, Resources.DlgFileCondition_ErrorNoCondition);
                return;
            }

            // 重複チェック
            for (int i = 0; i < m_existingList.Count; i++) {
                if (i != m_existingIndex) {
                    if (m_existingList[i] is FileConditionItemSSH && check.EqualsConfigObject((FileConditionItemSSH)(m_existingList[i]))) {
                        InfoBox.Warning(this, Resources.DlgFileCondition_ErrorSameItem, i + 1);
                        return;
                    }
                }
            }

            // 不要フィールドをクリア
            m_condition.CleanupField();

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：ファイル属性の条件をUIから読み込む
        // 引　数：[in]comboBoxImpl  属性の入力用コンボボックス
        // 戻り値：条件を読み込んだ結果（指定なしのときnull）
        //=========================================================================================
        private BooleanFlag GetAttribute(LasyComboBoxImpl comboBoxImpl) {
            return FileConditionWindowsDialog.TypeComboBoxIndexConverter.IndexToAttributeState(comboBoxImpl.SelectedIndex);
        }
    }
}
