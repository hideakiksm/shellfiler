using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル操作＞圧縮 の設定ページ
    //=========================================================================================
    public partial class FileOperationArchivePage : UserControl, IOptionDialogPage {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileOperationArchivePage(OptionSettingDialog parent) {
            InitializeComponent();

            // コンボボックスを初期化
            string[] methodItems = {
                Resources.OptionFileOprArc_MethodLocal,
                Resources.OptionFileOprArc_MethodRemote,
            };
            this.comboBoxMethod.Items.AddRange(methodItems);
            string[] typeItems = {
                Resources.OptionFileOprArc_TypeZip,
                Resources.OptionFileOprArc_Type7Zip,
                Resources.OptionFileOprArc_TypeTarGz,
                Resources.OptionFileOprArc_TypeTarBz2,
                Resources.OptionFileOprArc_TypeTar,
            };
            this.comboBoxType.Items.AddRange(typeItems);
            SevenZipArchiveFeature feature = SevenZipArchiveFeature.GetFeature(ArchiveType.Zip);
            this.comboBoxLZipAlg.Items.AddRange(feature.SupportMethod);
            this.comboBoxLZipEnc.Items.AddRange(feature.SupportEncryptionMethod);

            // コンフィグ値をUIに反映
            SetInitialValue(Configuration.Current);
            RadioButtonPrevFix_CheckedChanged(null, null);

            // イベントを接続
            this.radioButtonFix.CheckedChanged += new EventHandler(RadioButtonPrevFix_CheckedChanged);
            this.radioButtonPrev.CheckedChanged += new EventHandler(RadioButtonPrevFix_CheckedChanged);
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
            ArchiveSetting setting;
            if (config.ArchiveSettingDefault == null) {
                setting = new ArchiveSetting();
                this.radioButtonPrev.Checked = true;
            } else {
                setting = (ArchiveSetting)(config.ArchiveSettingDefault.Clone());
                this.radioButtonFix.Checked = true;
            }
            // 基本設定
            this.comboBoxMethod.SelectedIndex = ExecuteMethodToUiIndex(setting.ExecuteMetohd);
            this.comboBoxType.SelectedIndex = ArchiveTypeToUiIndex(setting.ArchiveType);

            // ローカルZIP
            SevenZipArchiveFeature feature = SevenZipArchiveFeature.GetFeature(ArchiveType.Zip);
            this.checkBoxLZipTime.Checked = setting.Local7zZipOption.ModifyTimestamp;
            this.comboBoxLZipAlg.SelectedIndex = StringUtils.GetStringArrayIndex(feature.SupportMethod, setting.Local7zZipOption.CompressionMethod);
            this.trackBarLZipLevel.Value = setting.Local7zZipOption.CompressionLevel;
            this.comboBoxLZipEnc.SelectedIndex = StringUtils.GetStringArrayIndex(feature.SupportEncryptionMethod, setting.Local7zZipOption.EncryptionMethod);

            // ローカル7-Zip
            this.checkBoxL7zTime.Checked = setting.Local7z7zOption.ModifyTimestamp;
            this.trackBarL7zLevel.Value = setting.Local7z7zOption.CompressionLevel;

            // ローカルtar.gz
            this.checkBoxLTgzTime.Checked = setting.Local7zTarGzOption.ModifyTimestamp;
            this.trackBarLTgzLevel.Value = setting.Local7zTarGzOption.CompressionLevel;

            // ローカルtar.bz2
            this.checkBoxLTbz2Time.Checked = setting.Local7zTarBz2Option.ModifyTimestamp;
            this.trackBarLTBz2Level.Value = setting.Local7zTarBz2Option.CompressionLevel;

            // ローカルtar
            this.checkBoxLTarTime.Checked = setting.Local7zTarOption.ModifyTimestamp;

            // リモートZIP
            this.checkBoxRZipTime.Checked = setting.RemoteZipOption.ModifyTimestamp;
            this.trackBarRZipLevel.Value = setting.RemoteZipOption.CompressionLevel;
        }

        //=========================================================================================
        // 機　能：圧縮の実行方法をコンボボックスのインデックスに変換する
        // 引　数：[in]method  圧縮の実行方法
        // 戻り値：コンボボックスのインデックス
        //=========================================================================================
        private int ExecuteMethodToUiIndex(ArchiveExecuteMethod method) {
            if (method == ArchiveExecuteMethod.Local7z) {
                return 0;
            } else {
                return 1;
            }
        }

        //=========================================================================================
        // 機　能：コンボボックスのインデックスを圧縮の実行方法に変換する
        // 引　数：[in]index   コンボボックスのインデックス
        // 戻り値：圧縮の実行方法
        //=========================================================================================
        private ArchiveExecuteMethod UiIndexToExecuteMethod(int index) {
            if (index == 0) {
                return ArchiveExecuteMethod.Local7z;
            } else {
                return ArchiveExecuteMethod.RemoteShell;
            }
        }

        //=========================================================================================
        // 機　能：ファイルフォーマットをコンボボックスのインデックスに変換する
        // 引　数：[in]type   ファイルフォーマット
        // 戻り値：コンボボックスのインデックス
        //=========================================================================================
        private int ArchiveTypeToUiIndex(ArchiveType type) {
            if (type == ArchiveType.Zip) {
                return 0;
            } else if (type == ArchiveType.SevenZip) {
                return 1;
            } else if (type == ArchiveType.TarGz) {
                return 2;
            } else if (type == ArchiveType.TarBz2) {
                return 3;
            } else {
                return 4;
            }
        }

        //=========================================================================================
        // 機　能：コンボボックスのインデックスをファイルフォーマットに変換する
        // 引　数：[in]index   コンボボックスのインデックス
        // 戻り値：ファイルフォーマット
        //=========================================================================================
        private ArchiveType UiIndexToArchiveType(int index) {
            switch (index) {
                case 0:
                    return ArchiveType.Zip;
                case 1:
                    return ArchiveType.SevenZip;
                case 2:
                    return ArchiveType.TarGz;
                case 3:
                    return ArchiveType.TarBz2;
                default:
                    return ArchiveType.Tar;
            }
        }

        //=========================================================================================
        // 機　能：前回値/固定値のラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void RadioButtonPrevFix_CheckedChanged(object sender, EventArgs evt) {
            bool enabled = (this.radioButtonFix.Checked);

            // 基本設定
            this.comboBoxMethod.Enabled = enabled;
            this.comboBoxType.Enabled = enabled;

            // ローカルZIP
            this.checkBoxLZipTime.Enabled = enabled;
            this.comboBoxLZipAlg.Enabled = enabled;
            this.trackBarLZipLevel.Enabled = enabled;
            this.comboBoxLZipEnc.Enabled = enabled;

            // ローカル7-Zip
            this.checkBoxL7zTime.Enabled = enabled;
            this.trackBarL7zLevel.Enabled = enabled;

            // ローカルtar.gz
            this.checkBoxLTgzTime.Enabled = enabled;
            this.trackBarLTgzLevel.Enabled = enabled;

            // ローカルtar.bz2
            this.checkBoxLTbz2Time.Enabled = enabled;
            this.trackBarLTBz2Level.Enabled = enabled;

            // ローカルtar
            this.checkBoxLTarTime.Enabled = enabled;

            // リモートZIP
            this.checkBoxRZipTime.Enabled = enabled;
            this.trackBarRZipLevel.Enabled = enabled;
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            ArchiveSetting setting;
            if (this.radioButtonPrev.Checked) {
                setting = null;
            } else {
                setting = new ArchiveSetting();
                // 基本設定
                setting.ExecuteMetohd = UiIndexToExecuteMethod(this.comboBoxMethod.SelectedIndex);
                setting.ArchiveType = UiIndexToArchiveType(this.comboBoxType.SelectedIndex);

                // ローカルZIP
                SevenZipArchiveFeature feature = SevenZipArchiveFeature.GetFeature(ArchiveType.Zip);
                setting.Local7zZipOption.ModifyTimestamp = this.checkBoxLZipTime.Checked;
                setting.Local7zZipOption.CompressionMethod = feature.SupportMethod[this.comboBoxLZipAlg.SelectedIndex];
                setting.Local7zZipOption.CompressionLevel = this.trackBarLZipLevel.Value;
                setting.Local7zZipOption.EncryptionMethod = feature.SupportEncryptionMethod[this.comboBoxLZipEnc.SelectedIndex];

                // ローカル7-Zip
                setting.Local7z7zOption.ModifyTimestamp = this.checkBoxL7zTime.Checked;
                setting.Local7z7zOption.CompressionLevel = this.trackBarL7zLevel.Value;

                // ローカルtar.gz
                setting.Local7zTarGzOption.ModifyTimestamp = this.checkBoxLTgzTime.Checked;
                setting.Local7zTarGzOption.CompressionLevel = this.trackBarLTgzLevel.Value;

                // ローカルtar.bz2
                setting.Local7zTarBz2Option.ModifyTimestamp = this.checkBoxLTbz2Time.Checked;
                setting.Local7zTarBz2Option.CompressionLevel = this.trackBarLTBz2Level.Value;

                // ローカルtar
                setting.Local7zTarOption.ModifyTimestamp = this.checkBoxLTarTime.Checked;

                // リモートZIP
                setting.RemoteZipOption.ModifyTimestamp = this.checkBoxRZipTime.Checked;
                setting.RemoteZipOption.CompressionLevel = this.trackBarRZipLevel.Value;
            }

            Configuration.Current.ArchiveSettingDefault = setting;
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
