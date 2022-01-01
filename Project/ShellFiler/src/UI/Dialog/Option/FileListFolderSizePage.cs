using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル一覧＞フォルダサイズ の設定ページ
    //=========================================================================================
    public partial class FileListFolderSizePage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileListFolderSizePage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;
            SetInitialValue(Configuration.Current);
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
        }

        //=========================================================================================
        // 機　能：UIに初期値を設定する
        // 引　数：[in]config  取得対象のコンフィグ
        // 戻り値：なし
        //=========================================================================================
        private void SetInitialValue(Configuration config) {
            RetrieveFolderSizeCondition condition = config.RetrieveFolderSizeCondition;
            if (condition == null) {
                this.radioButtonPrev.Checked = true;
                this.ActiveControl = this.radioButtonPrev;
                SetConditionValue(new RetrieveFolderSizeCondition());
            } else {
                this.radioButtonFix.Checked = true;
                this.ActiveControl = this.radioButtonFix;
                SetConditionValue(condition);
            }

            this.numericMaxDepth.Minimum = Configuration.MIN_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_DEPTH;
            this.numericMaxDepth.Maximum = Configuration.MAX_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_DEPTH;
            this.numericMaxDepth.Value = config.RetrieveFolderSizeKeepLowerDepth;

            this.numericMaxFolder.Minimum = Configuration.MIN_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_COUNT;
            this.numericMaxFolder.Maximum = Configuration.MAX_RETRIEVE_FOLDER_SIZE_KEEP_LOWER_COUNT;
            this.numericMaxFolder.Value = config.RetrieveFolderSizeKeepLowerCount;

            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：UIの条件部分に初期値を設定する
        // 引　数：[in]condition  設定する条件
        // 戻り値：なし
        //=========================================================================================
        private void SetConditionValue(RetrieveFolderSizeCondition condition) {
            RadioButton selected = null;
            if (condition.SizeMode == RetrieveFolderSizeCondition.FolderSizeMode.OriginalSize) {
                selected = this.radioButtonOriginal;
            } else if (condition.SizeMode == RetrieveFolderSizeCondition.FolderSizeMode.TargetPathCluster) {
                selected = this.radioButtonTarget;
            } else if (condition.SizeMode == RetrieveFolderSizeCondition.FolderSizeMode.OppositePathCluster) {
                selected = this.radioButtonOpposite;
            } else if (condition.SizeMode == RetrieveFolderSizeCondition.FolderSizeMode.SpecifiedSize) {
                selected = this.radioButtonSpecify;
            }
            selected.Checked = true;
            this.numericSpecify.Minimum = Configuration.MIN_RETRIEVE_FOLDER_SIZE_UNIT;
            this.numericSpecify.Maximum = Configuration.MAX_RETRIEVE_FOLDER_SIZE_UNIT;
            this.numericSpecify.Value = condition.FolderSizeUnit;
            this.checkBoxLowerCache.Checked = condition.UseCache;
        }

        //=========================================================================================
        // 機　能：UIセットの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            if (this.radioButtonPrev.Checked) {
                this.radioButtonOriginal.Enabled = false;
                this.radioButtonTarget.Enabled = false;
                this.radioButtonOpposite.Enabled = false;
                this.radioButtonSpecify.Enabled = false;
                this.numericSpecify.Enabled = false;
                this.checkBoxLowerCache.Enabled = false;
            } else {
                this.radioButtonOriginal.Enabled = true;
                this.radioButtonTarget.Enabled = true;
                this.radioButtonOpposite.Enabled = true;
                this.radioButtonSpecify.Enabled = true;
                this.numericSpecify.Enabled = true;
                this.checkBoxLowerCache.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：前回値使用か固定かのラジオボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void radioButtonPrevFixed_CheckedChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            // 条件
            RetrieveFolderSizeCondition condition;
            if (this.radioButtonPrev.Checked) {
                condition = null;
            } else {
                condition = new RetrieveFolderSizeCondition();
                if (this.radioButtonOriginal.Checked) {
                    condition.SizeMode = RetrieveFolderSizeCondition.FolderSizeMode.OriginalSize;
                    condition.FolderSizeUnit = 1;
                } else if (this.radioButtonTarget.Checked) {
                    condition.SizeMode = RetrieveFolderSizeCondition.FolderSizeMode.TargetPathCluster;
                    condition.FolderSizeUnit = 1;
                } else if (this.radioButtonOpposite.Checked) {
                    condition.SizeMode = RetrieveFolderSizeCondition.FolderSizeMode.OppositePathCluster;
                    condition.FolderSizeUnit = 1;
                } else {
                    int unit = (int)(this.numericSpecify.Value);
                    Configuration.ModifyRetrieveFolderSizeUnit(ref unit);
                    condition.SizeMode = RetrieveFolderSizeCondition.FolderSizeMode.SpecifiedSize;
                    condition.FolderSizeUnit = (int)(this.numericSpecify.Value);
                }
                condition.UseCache = this.checkBoxLowerCache.Checked;
            }

            // 配下の保持数
            int depth = (int)(this.numericMaxDepth.Value);
            Configuration.ModifyRetrieveFolderSizeKeepLowerDepth(ref depth);
            int count = (int)(this.numericMaxFolder.Value);
            Configuration.ModifyRetrieveFolderSizeKeepLowerCount(ref count);


            // Configに反映
            Configuration.Current.RetrieveFolderSizeCondition = condition;
            Configuration.Current.RetrieveFolderSizeKeepLowerDepth = depth;
            Configuration.Current.RetrieveFolderSizeKeepLowerCount = count;

            return true;
        }

        //=========================================================================================
        // 機　能：ページ内の設定をデフォルトに戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetDefault() {
            Configuration org = new Configuration();
            SetInitialValue(org);
        }
    }
}
