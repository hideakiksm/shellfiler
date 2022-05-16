using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイルフィルターの設定用コンポーネント
    //=========================================================================================
    public partial class FileFilterSettingComponent : UserControl {
        // 所有ダイアログの実装
        private IFileFilterSettingComponentNotify m_parent;

        // 設定対象のフィルターコンポーネント（フィルターが未設定のときnull）
        private FileFilterItem m_filterItem;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  このコンポーネントの所有元
        // 　　　　[in]item    フィルター1件の設定（フィルターが未設定のときnull）
        // 戻り値：なし
        //=========================================================================================
        public FileFilterSettingComponent(IFileFilterSettingComponentNotify parent, FileFilterItem item) {
            m_parent = parent;
            InitializeComponent();
            SetFilterItem(item);
        }

        //=========================================================================================
        // 機　能：フィルターの項目をセットする
        // 引　数：[in]item   フィルター1件の設定（フィルターが未設定のときnull）
        // 戻り値：なし
        //=========================================================================================
        public void SetFilterItem(FileFilterItem item) {
            m_filterItem = item;

            if (item == null) {
                // コンポーネントを再構築
                this.SuspendLayout();
                this.panelUI.Controls.Clear();
                this.panelName.Visible = false;
                this.labelNoFilter1.Visible = true;
                this.labelNoFilter2.Visible = true;
                this.ResumeLayout(false);
                this.PerformLayout();
            } else {
                Type componentType = Type.GetType(item.FileFilterClassPath);
                IFileFilterComponent component = (IFileFilterComponent)(componentType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));

                // コンポーネントを再構築
                this.SuspendLayout();
                this.panelUI.Controls.Clear();
                int xPos = 0;
                int yPos = 0;
                List<SettingUIItem> uiList = component.GetSettingUI();
                Dictionary<string, object> param = item.PropertyList;
                for (int i = 0; i < uiList.Count; i++) {
                    SettingUIItem uiItem = uiList[i];
                    CreateComponent(uiItem, param, xPos, ref yPos);
                }

                // ラベル
                this.labelFilterName.Text = component.FilterName;
                this.textBoxExplain.Text = component.FilterExplain;

                this.panelName.Visible = true;
                this.labelNoFilter1.Visible = false;
                this.labelNoFilter2.Visible = false;

                this.ResumeLayout(false);
                this.PerformLayout();
            }
        }

        //=========================================================================================
        // 機　能：コンポーネントを作成する
        // 引　数：[in]uiItem   コンポーネントの作成指示
        // 　　　　[in]param    フィルターへのパラメータ
        // 　　　　[in]xPos     表示開始X位置
        // 　　　　[in]yPos     表示開始Y位置
        // 戻り値：なし
        //=========================================================================================
        public void CreateComponent(SettingUIItem uiItem, Dictionary<string, object> param, int xPos, ref int yPos) {
            using (Graphics g = CreateGraphics())
            using (HighDpiGraphics graphics = new HighDpiGraphics(g)) {
                if (uiItem is SettingUIItem.Space) {
                    yPos += graphics.Y(16);
                } else if (uiItem is SettingUIItem.Label) {
                    SettingUIItem.Label settingLabel = (SettingUIItem.Label)uiItem;
                    Label label = new Label();
                    label.AutoSize = true;
                    label.Location = new Point(xPos + graphics.X(settingLabel.Indent * 16), yPos);
                    label.Size = new Size(graphics.X(35), graphics.Y(12));
                    label.TabIndex = 0;
                    label.Text = settingLabel.Text;
                    this.panelUI.Controls.Add(label);
                    yPos += graphics.Y(16);
                } else if (uiItem is SettingUIItem.Combobox) {
                    SettingUIItem.Combobox settingCombobox = (SettingUIItem.Combobox)uiItem;
                    Label label = new Label();
                    label.AutoSize = true;
                    label.Location = new Point(xPos, yPos + graphics.Y(4));
                    label.Size = new Size(graphics.X(35), graphics.Y(12));
                    label.TabIndex = 0;
                    label.Text = settingCombobox.LabelText;
                    this.panelUI.Controls.Add(label);

                    string value = (string)(param[settingCombobox.ControlId]);
                    ComboBox combobox = new ComboBox();
                    combobox.DropDownStyle = ComboBoxStyle.DropDownList;
                    combobox.FormattingEnabled = true;
                    combobox.Location = new Point(xPos + graphics.X(150), yPos);
                    combobox.Size = new Size(graphics.X(180), graphics.Y(20));
                    combobox.Items.AddRange(settingCombobox.ItemList);
                    combobox.Tag = settingCombobox;
                    combobox.SelectedIndex = StringUtils.SearchElementIndex(settingCombobox.IdList, value);
                    combobox.SelectedIndexChanged += new EventHandler(Combobox_SelectedIndexChanged);
                    this.panelUI.Controls.Add(combobox);
                    yPos += graphics.Y(24);
                } else if (uiItem is SettingUIItem.TextBox) {
                    SettingUIItem.TextBox settingTextbox = (SettingUIItem.TextBox)uiItem;
                    Label label = new Label();
                    label.AutoSize = true;
                    label.Location = new Point(xPos, yPos + graphics.Y(4));
                    label.Size = new Size(graphics.X(35), graphics.Y(12));
                    label.TabIndex = 0;
                    label.Text = settingTextbox.LabelText;
                    this.panelUI.Controls.Add(label);

                    string value = (string)(param[settingTextbox.ControlId]);
                    TextBox textbox = new TextBox();
                    textbox.Location = new Point(xPos + graphics.X(150), yPos);
                    textbox.Size = new Size(graphics.X(310), graphics.Y(19));
                    textbox.Tag = settingTextbox;
                    textbox.Text = value;
                    textbox.TextChanged += new EventHandler(Textbox_TextChanged);
                    this.panelUI.Controls.Add(textbox);
                    yPos += graphics.Y(24);
                } else if (uiItem is SettingUIItem.Checkbox) {
                    SettingUIItem.Checkbox settingCheckbox = (SettingUIItem.Checkbox)uiItem;
                    bool value = (bool)(param[settingCheckbox.ControlId]);
                    CheckBox checkbox = new CheckBox();
                    checkbox.AutoSize = true;
                    checkbox.Location = new Point(xPos, yPos);
                    checkbox.Size = new System.Drawing.Size(graphics.X(107), graphics.Y(16));
                    checkbox.Text = settingCheckbox.LabelText;
                    checkbox.UseVisualStyleBackColor = true;
                    checkbox.Tag = settingCheckbox;
                    checkbox.Checked = value;
                    checkbox.CheckedChanged += new EventHandler(Checkbox_CheckedChanged);
                    this.panelUI.Controls.Add(checkbox);
                    yPos += graphics.Y(24);
                } else if (uiItem is SettingUIItem.Numeric) {
                    SettingUIItem.Numeric settingNumeric = (SettingUIItem.Numeric)uiItem;
                    Label label = new Label();
                    label.AutoSize = true;
                    label.Location = new Point(xPos, yPos + graphics.Y(4));
                    label.Size = new Size(graphics.X(35), graphics.Y(12));
                    label.TabIndex = 0;
                    label.Text = settingNumeric.LabelText;
                    this.panelUI.Controls.Add(label);

                    int value = (int)(param[settingNumeric.ControlId]);
                    NumericUpDown numeric = new NumericUpDown();
                    numeric.Location = new Point(xPos + graphics.X(150), yPos);
                    numeric.Size = new Size(graphics.X(180), graphics.Y(19));
                    numeric.Maximum = settingNumeric.MaxValue;
                    numeric.Minimum = settingNumeric.MinValue;
                    numeric.Value = value;
                    numeric.Tag = settingNumeric;
                    numeric.ValueChanged += new EventHandler(Numeric_ValueChanged);
                    this.panelUI.Controls.Add(numeric);
                    yPos += graphics.Y(24);
                }
            }
        }

        //=========================================================================================
        // 機　能：コンボボックスの選択項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void Combobox_SelectedIndexChanged(object sender, EventArgs evt) {
            ComboBox combobox = (ComboBox)sender;
            SettingUIItem.Combobox settingCombobox = (SettingUIItem.Combobox)combobox.Tag;
            int index = combobox.SelectedIndex;
            if (index == -1) {
                return;
            }
            string controlId = settingCombobox.ControlId;
            string newValue = settingCombobox.IdList[index];
            string oldValue = (string)m_filterItem.PropertyList[controlId];
            if (newValue != oldValue) {
                m_filterItem.PropertyList[controlId] = newValue;
                m_parent.OnNotifyPropertyChange(controlId);
            }
        }

        //=========================================================================================
        // 機　能：チェックボックスのチェック状態が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void Checkbox_CheckedChanged(object sender, EventArgs evt) {
            CheckBox checkbox = (CheckBox)sender;
            SettingUIItem.Checkbox settingCheckbox = (SettingUIItem.Checkbox)checkbox.Tag;
            string controlId = settingCheckbox.ControlId;
            bool newValue = checkbox.Checked;
            bool oldValue = (bool)m_filterItem.PropertyList[controlId];
            if (newValue != oldValue) {
                m_filterItem.PropertyList[controlId] = newValue;
                m_parent.OnNotifyPropertyChange(controlId);
            }
        }

        //=========================================================================================
        // 機　能：テキストボックスの内容が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void Textbox_TextChanged(object sender, EventArgs evt) {
            TextBox textBox = (TextBox)sender;
            SettingUIItem.TextBox settingTextbox = (SettingUIItem.TextBox)textBox.Tag;
            string controlId = settingTextbox.ControlId;
            string newValue = textBox.Text;
            string oldValue = (string)m_filterItem.PropertyList[controlId];
            if (newValue != oldValue) {
                m_filterItem.PropertyList[controlId] = newValue;
                m_parent.OnNotifyPropertyChange(controlId);
            }
        }

        //=========================================================================================
        // 機　能：数値入力の内容が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void Numeric_ValueChanged(object sender, EventArgs evt) {
            NumericUpDown numeric = (NumericUpDown)sender;
            SettingUIItem.Numeric settingNumeric = (SettingUIItem.Numeric)numeric.Tag;
            string controlId = settingNumeric.ControlId;
            int newValue = (int)(numeric.Value);
            int oldValue = (int)m_filterItem.PropertyList[controlId];
            if (newValue != oldValue) {
                m_filterItem.PropertyList[controlId] = newValue;
                m_parent.OnNotifyPropertyChange(controlId);
            }
        }

        //=========================================================================================
        // クラス：設定コンポーネントでの変更通知先が実装するインターフェース
        //=========================================================================================
        public interface IFileFilterSettingComponentNotify {

            //=========================================================================================
            // 機　能：フィルターの詳細設定でプロパティの変更があったことを通知する
            // 引　数：[in]controlId   変更されたコントロールのID
            // 戻り値：なし
            //=========================================================================================
            void OnNotifyPropertyChange(string controlId);
        }
    }
}
