using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル一覧＞起動時の一覧 の設定ページ
    //=========================================================================================
    public partial class FileListSortPage : UserControl, IOptionDialogPage {
        // 左ウィンドウ用のUIのセット
        private UIItemSet m_uiItemSetLeft;

        // 右ウィンドウ用のUIのセット
        private UIItemSet m_uiItemSetRight;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileListSortPage(OptionSettingDialog parent) {
            InitializeComponent();

            // UIのセットを定義
            m_uiItemSetLeft = new UIItemSet(this.radioButtonLeftPrev, this.radioButtonLeftFix,
                                            this.comboBoxLeftPrimary, this.checkBoxLeftPrimaryRev,
                                            this.comboBoxLeftSecondary, this.checkBoxLeftSecondaryRev,
                                            this.checkBoxLeftCapital, this.checkBoxLeftDirTop, this.checkBoxLeftNumber);
            m_uiItemSetRight = new UIItemSet(this.radioButtonRightPrev, this.radioButtonRightFix,
                                             this.comboBoxRightPrimary, this.checkBoxRightPrimaryRev,
                                             this.comboBoxRightSecondary, this.checkBoxRightSecondaryRev,
                                             this.checkBoxRightCapital, this.checkBoxRightDirTop, this.checkBoxRightNumber);

            // コンボボックスの項目一覧を設定
            ComboBox[] sortComboBoxList = {
                this.comboBoxLeftPrimary, this.comboBoxLeftSecondary, this.comboBoxRightPrimary, this.comboBoxRightSecondary
            };
            string[] comboboxItemList = {
                Resources.OptionFileListSort_FileName,
                Resources.OptionFileListSort_DateTime,
                Resources.OptionFileListSort_Extension,
                Resources.OptionFileListSort_FileSize,
                Resources.OptionFileListSort_Attribute,
                Resources.OptionFileListSort_NoSort,
            };
            foreach (ComboBox comboBox in sortComboBoxList) {
                comboBox.Items.AddRange(comboboxItemList);
            }

            // コンフィグ値をUIに反映
            SetInitialValue(Configuration.Current);

            // イベントを接続
            this.comboBoxLeftPrimary.SelectedValueChanged += new EventHandler(comboBoxLeftPrimary_SelectedValueChanged);
            this.comboBoxLeftSecondary.SelectedValueChanged += new EventHandler(comboBoxLeftSecondary_SelectedValueChanged);
            this.comboBoxRightPrimary.SelectedValueChanged += new EventHandler(comboBoxRightPrimary_SelectedValueChanged);
            this.comboBoxRightSecondary.SelectedValueChanged += new EventHandler(comboBoxRightSecondary_SelectedValueChanged);
            this.radioButtonLeftPrev.CheckedChanged += new System.EventHandler(this.radioButtonLeft_CheckedChanged);
            this.radioButtonLeftFix.CheckedChanged += new System.EventHandler(this.radioButtonLeft_CheckedChanged);
            this.radioButtonRightPrev.CheckedChanged += new System.EventHandler(this.radioButtonRight_CheckedChanged);
            this.radioButtonRightFix.CheckedChanged += new System.EventHandler(this.radioButtonRight_CheckedChanged);
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
            FileListSortMode sortModeLeft = config.DefaultFileListSortModeLeft;
            FileListSortMode sortModeRight = config.DefaultFileListSortModeRight;

            if (sortModeLeft == null) {
                SortModeToUi(new FileListSortMode(), m_uiItemSetLeft);
                EnableUIItemSet(m_uiItemSetLeft, false);
                this.radioButtonLeftPrev.Checked = true;
            } else {
                SortModeToUi(sortModeLeft, m_uiItemSetLeft);
                EnableUIItemSet(m_uiItemSetLeft, true);
                this.radioButtonLeftFix.Checked = true;
            }

            if (sortModeRight == null) {
                SortModeToUi(new FileListSortMode(), m_uiItemSetRight);
                EnableUIItemSet(m_uiItemSetRight, false);
                this.radioButtonRightPrev.Checked = true;
            } else {
                SortModeToUi(sortModeRight, m_uiItemSetRight);
                EnableUIItemSet(m_uiItemSetRight, true);
                this.radioButtonRightFix.Checked = true;
            }
        }

        //=========================================================================================
        // 機　能：UIセットの有効/無効を切り替える
        // 引　数：[in]uiItemSet 反映するUIのセット
        // 　　　　[in]enabled   項目を有効にするときtrue
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItemSet(UIItemSet uiItemSet, bool enabled) {
            uiItemSet.ComboBoxPrimary.Enabled = enabled;
            uiItemSet.CheckBoxPrimaryRev.Enabled = enabled;
            uiItemSet.ComboBoxSecondary.Enabled = enabled;
            uiItemSet.CheckBoxSecondaryRev.Enabled = enabled;
            uiItemSet.CheckBoxDirTop.Enabled = enabled;
            uiItemSet.CheckBoxCapital.Enabled = enabled;
            uiItemSet.CheckBoxNumber.Enabled = enabled;
        }

        //=========================================================================================
        // 機　能：ソートモードをUIに反映させる
        // 引　数：[in]sortMode  ソートモード
        // 　　　　[in]uiItemSet 反映するUIのセット
        // 戻り値：なし
        //=========================================================================================
        private void SortModeToUi(FileListSortMode sortMode, UIItemSet uiItemSet) {
            SortMethodToUi(sortMode.SortOrder1, uiItemSet.ComboBoxPrimary);
            uiItemSet.CheckBoxPrimaryRev.Checked = (sortMode.SortDirection1 == FileListSortMode.Direction.Reverse);
            SortMethodToUi(sortMode.SortOrder2, uiItemSet.ComboBoxSecondary);
            uiItemSet.CheckBoxSecondaryRev.Checked = (sortMode.SortDirection2 == FileListSortMode.Direction.Reverse);
            uiItemSet.CheckBoxDirTop.Checked = sortMode.TopDirectory;
            uiItemSet.CheckBoxCapital.Checked = sortMode.Capital;
            uiItemSet.CheckBoxNumber.Checked = sortMode.IdentifyNumber;
        }

        //=========================================================================================
        // 機　能：UIからソートモードを取得する
        // 引　数：[in]uiItemSet 取得するUIのセット
        // 戻り値：ソートモード
        //=========================================================================================
        private FileListSortMode UiToSortMode(UIItemSet uiItemSet) {
            FileListSortMode result;
            if (uiItemSet.RadioButtonPrev.Checked) {
                result = null;
            } else {
                result = new FileListSortMode();
                result.SortOrder1 = UiToSortMethod(uiItemSet.ComboBoxPrimary);
                if (uiItemSet.CheckBoxPrimaryRev.Checked) {
                    result.SortDirection1 = FileListSortMode.Direction.Reverse;
                } else {
                    result.SortDirection1 = FileListSortMode.Direction.Normal;
                }
                result.SortOrder2 = UiToSortMethod(uiItemSet.ComboBoxSecondary);
                if (uiItemSet.CheckBoxSecondaryRev.Checked) {
                    result.SortDirection2 = FileListSortMode.Direction.Reverse;
                } else {
                    result.SortDirection2 = FileListSortMode.Direction.Normal;
                }
                result.TopDirectory = uiItemSet.CheckBoxDirTop.Checked;
                result.Capital = uiItemSet.CheckBoxCapital.Checked;
                result.IdentifyNumber = uiItemSet.CheckBoxNumber.Checked;
                result.ModifySortMode();
            }
            return result;
        }

        //=========================================================================================
        // 機　能：ソートキーをUIに反映させる
        // 引　数：[in]sortOrder  ソートキー
        // 　　　　[in]comboBox   設定対象のコンボボックス
        // 戻り値：なし
        //=========================================================================================
        private void SortMethodToUi(FileListSortMode.Method sortOrder, ComboBox comboBox) {
            int item = 0;
            switch (sortOrder) {
                case FileListSortMode.Method.FileName:
                    item = 0;
                    break;
                case FileListSortMode.Method.DateTime:
                    item = 1;
                    break;
                case FileListSortMode.Method.Extension:
                    item = 2;
                    break;
                case FileListSortMode.Method.FileSize:
                    item = 3;
                    break;
                case FileListSortMode.Method.Attribute:
                    item = 4;
                    break;
                case FileListSortMode.Method.NoSort:
                    item = 5;
                    break;
            }
            comboBox.SelectedIndex = item;
        }
        
        //=========================================================================================
        // 機　能：UIからソートキーを得る
        // 引　数：[in]comboBox  取得対象のコンボボックス
        // 戻り値：ソートキー
        //=========================================================================================
        private FileListSortMode.Method UiToSortMethod(ComboBox comboBox) {
            FileListSortMode.Method key = FileListSortMode.Method.NoSort;
            switch (comboBox.SelectedIndex) {
                case 0:
                    key = FileListSortMode.Method.FileName;
                    break;
                case 1:
                    key = FileListSortMode.Method.DateTime;
                    break;
                case 2:
                    key = FileListSortMode.Method.Extension;
                    break;
                case 3:
                    key = FileListSortMode.Method.FileSize;
                    break;
                case 4:
                    key = FileListSortMode.Method.Attribute;
                    break;
                case 5:
                    key = FileListSortMode.Method.NoSort;
                    break;
            }
            return key;
        }
        
        //=========================================================================================
        // 機　能：左ウィンドウの前回値/指定値のラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void radioButtonLeft_CheckedChanged(object sender, EventArgs evt) {
            OnRadioButtonChanged(m_uiItemSetLeft);
        }

        //=========================================================================================
        // 機　能：右ウィンドウの前回値/指定値のラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void radioButtonRight_CheckedChanged(object sender, EventArgs evt) {
            OnRadioButtonChanged(m_uiItemSetRight);
        }

        //=========================================================================================
        // 機　能：前回値/指定値のラジオボタンが変更されたときの処理を行う
        // 引　数：[in]uiItemSet  反映するUIのセット
        // 戻り値：なし
        //=========================================================================================
        private void OnRadioButtonChanged(UIItemSet uiItemSet) {
            if (uiItemSet.RadioButtonPrev.Checked) {
                EnableUIItemSet(uiItemSet, false);
            } else {
                EnableUIItemSet(uiItemSet, true);
            }
        }

        //=========================================================================================
        // 機　能：左ウィンドウ第1ソートキーの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxLeftPrimary_SelectedValueChanged(object sender, EventArgs evt) {
            OnSortKeyPrimaryChanged(m_uiItemSetLeft);
        }

        //=========================================================================================
        // 機　能：左ウィンドウ第2ソートキーの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxLeftSecondary_SelectedValueChanged(object sender, EventArgs evt) {
            OnSortKeySecondaryChanged(m_uiItemSetLeft);
        }

        //=========================================================================================
        // 機　能：右ウィンドウ第1ソートキーの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxRightPrimary_SelectedValueChanged(object sender, EventArgs evt) {
            OnSortKeyPrimaryChanged(m_uiItemSetRight);
        }

        //=========================================================================================
        // 機　能：右ウィンドウ第2ソートキーの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxRightSecondary_SelectedValueChanged(object sender, EventArgs evt) {
            OnSortKeySecondaryChanged(m_uiItemSetRight);
        }

        //=========================================================================================
        // 機　能：第1ソートキーの選択項目が変更されたときの処理を行う
        // 引　数：[in]uiItemSet  反映するUIのセット
        // 戻り値：なし
        //=========================================================================================
        private void OnSortKeyPrimaryChanged(UIItemSet uiItemSet) {
            FileListSortMode.Method sortMode1 = UiToSortMethod(uiItemSet.ComboBoxPrimary);
            FileListSortMode.Method sortMode2 = UiToSortMethod(uiItemSet.ComboBoxSecondary);
            FileListSortMode.Method sortMode2Mod = FileListSortMode.ModifySort2BySort1(sortMode1, sortMode2);
            SortMethodToUi(sortMode2Mod, uiItemSet.ComboBoxSecondary);
        }

        //=========================================================================================
        // 機　能：第2ソートキーの選択項目が変更されたときの処理を行う
        // 引　数：[in]uiItemSet  反映するUIのセット
        // 戻り値：なし
        //=========================================================================================
        private void OnSortKeySecondaryChanged(UIItemSet uiItemSet) {
            FileListSortMode.Method sortMode1 = UiToSortMethod(uiItemSet.ComboBoxPrimary);
            FileListSortMode.Method sortMode2 = UiToSortMethod(uiItemSet.ComboBoxSecondary);
            FileListSortMode.Method sortMode1Mod = FileListSortMode.ModifySort1BySort2(sortMode1, sortMode2);
            SortMethodToUi(sortMode1Mod, uiItemSet.ComboBoxPrimary);
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            FileListSortMode sortModeLeft = UiToSortMode(m_uiItemSetLeft);
            FileListSortMode sortModeRight = UiToSortMode(m_uiItemSetRight);
            Configuration.Current.DefaultFileListSortModeLeft = sortModeLeft;
            Configuration.Current.DefaultFileListSortModeRight = sortModeRight;
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

        //=========================================================================================
        // クラス：左右で共通の設定項目となっているUIのセット
        //=========================================================================================
        private class UIItemSet {
            // 前回値を使用する
            public RadioButton RadioButtonPrev;

            // ソート方法を指定する
            public RadioButton RadioButtonFix;

            // 第1ソートキー
            public ComboBox ComboBoxPrimary;

            // 第1ソートキーの向きが逆順のときtrue
            public CheckBox CheckBoxPrimaryRev;

            // 第2ソートキー
            public ComboBox ComboBoxSecondary;

            // 第2ソートキーの向きが逆順のときtrue
            public CheckBox CheckBoxSecondaryRev;

            // ディレクトリを先頭に集めるときtrue
            public CheckBox CheckBoxDirTop;
            
            // 大文字小文字を区別するときtrue
            public CheckBox CheckBoxCapital;
            
            // 数値の大小を比較するときtrue
            public CheckBox CheckBoxNumber;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]radioButtonPrev      前回値を使用する
            // 　　　　[in]radioButtonFix       ソート方法を指定する
            // 　　　　[in]comboBoxPrimary      第1ソートキー
            // 　　　　[in]checkBoxPrimaryRev   第1ソートキーの向きが逆順のときtrue
            // 　　　　[in]comboBoxSecondary    第2ソートキー
            // 　　　　[in]checkBoxSecondaryRev 第2ソートキーの向きが逆順のときtrue
            // 　　　　[in]checkBoxDirTop       ディレクトリを先頭に集めるときtrue
            // 　　　　[in]checkBoxCapital      大文字小文字を区別するときtrue
            // 　　　　[in]checkBoxNumber       数値の大小を比較するときtrue
            // 戻り値：なし
            //=========================================================================================
            public UIItemSet(RadioButton radioButtonPrev, RadioButton radioButtonFix, ComboBox comboBoxPrimary, CheckBox checkBoxPrimaryRev, ComboBox comboBoxSecondary, CheckBox checkBoxSecondaryRev, CheckBox checkBoxDirTop, CheckBox checkBoxCapital, CheckBox checkBoxNumber) {
                RadioButtonPrev = radioButtonPrev;
                RadioButtonFix = radioButtonFix;
                ComboBoxPrimary = comboBoxPrimary;
                CheckBoxPrimaryRev = checkBoxPrimaryRev;
                ComboBoxSecondary = comboBoxSecondary;
                CheckBoxSecondaryRev = checkBoxSecondaryRev;
                CheckBoxDirTop = checkBoxDirTop;
                CheckBoxCapital = checkBoxCapital;
                CheckBoxNumber = checkBoxNumber;
            }
        }
    }
}
