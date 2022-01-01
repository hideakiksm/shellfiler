using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.FileList.Open;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：キー設定のオプション指定ダイアログ
    //=========================================================================================
    public partial class KeySettingOptionDialog : Form {
        // 対象となるコマンドの詳細情報
        private CommandApi m_commandApi;
        
        // コマンドの利用シーン
        private CommandUsingSceneType m_commandSceneType;

        // 割り当てようとしているキー（関連付けに対する割り当て変更のときはnull）
        private KeyState m_targetKey;
        
        // キーに割り当てられていたコマンド（割り当てられていなかったときはnull）
        private ActionCommandMoniker m_oldCommand;
        
        // 対象となるキーに新しく割り当てるコマンド
        private ActionCommandMoniker m_newCommand;

        // 設定されていたファンクションバーの表示名（割り当てがないときはnull）
        private string m_commandDispName;

        // 入力領域のコントロール
        private List<Control> m_controlUI = new List<Control>();

        // 入力した値（入力するごとに、常に最新）
        private object[] m_paramList;

        // 入力したオプション
        private ActionCommandOption m_resultCommandOption;

        // ファンクションバーの表示名（ファンクションキー以外のとき、または、デフォルトを使用するときはnull）
        private string m_resultDisplayNameFunctionBar;

        // ヘルプウィンドウ
        private CommandArgumentHelp m_commandHelp;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]commandApi      対象となるコマンドの詳細情報
        // 　　　　[in]sceneType       コマンドの利用シーン
        // 　　　　[in]targetKey       割り当てようとしているキー（関連付けに対する割り当て変更のときはnull）
        // 　　　　[in]oldCommand      キーに割り当てられていたコマンド（割り当てられていなかったときはnull）
        // 　　　　[in]newCommand      対象となるキーに新しく割り当てるコマンド
        // 　　　　[in]commandDispName 設定されていたファンクションバーの表示名（割り当てがないときはnull）
        // 戻り値：なし
        //=========================================================================================
        public KeySettingOptionDialog(CommandApi commandApi, CommandUsingSceneType sceneType, KeyState targetKey, ActionCommandMoniker oldCommand, ActionCommandMoniker newCommand, string commandDispName) {
            InitializeComponent();
            m_commandApi = commandApi;
            m_commandSceneType = sceneType;
            m_targetKey = targetKey;
            m_oldCommand = oldCommand;
            m_newCommand = newCommand;
            m_commandDispName = commandDispName;
        }

        //=========================================================================================
        // 機　能：フォームが開かれたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void KeySettingOptionDialog_Load(object sender, EventArgs evt) {
            // パラメータの初期値を決定
            m_paramList = new object[m_commandApi.ArgumentList.Count];
            if (m_oldCommand != null && m_newCommand.CommandType.Equals(m_oldCommand.CommandType)) {
                // 前回のパラメータを初期値に設定
                for (int i = 0; i < m_paramList.Length; i++) {
                    m_paramList[i] = m_oldCommand.Parameter[i];
                }
            } else {
                // 設定XMLのデフォルトを初期値に設定
                for (int i = 0; i < m_paramList.Length; i++) {
                    m_paramList[i] = m_commandApi.ArgumentList[i].DefaultValue;
                } 
            }

            // パラメータの初期値をUIに反映
            InitializeParameterUI();
            this.groupBoxOption.Location = new Point(this.groupBoxOption.Location.X, this.groupBoxParameter.Location.Y + this.groupBoxParameter.Size.Height + 8);
            int yPosButton = this.groupBoxOption.Location.Y + this.groupBoxOption.Height + 8;
            int yPosButtonOld = this.buttonOk.Location.Y;
            this.buttonOk.Location = new Point(this.buttonOk.Location.X, yPosButton);
            this.buttonCancel.Location = new Point(this.buttonCancel.Location.X, yPosButton);
            int cyDialog = yPosButton + this.buttonOk.Size.Height + 8;
            this.Size = new Size(this.Size.Width, this.Size.Height + (yPosButton - yPosButtonOld));

            // パラメータの説明を設定
            UpdateCommandExplanation();

            // 表示名の初期値を設定
            string displayName;
            if (m_commandDispName != null) {
                displayName = m_commandDispName;
            } else {
                displayName = m_newCommand.CreateActionCommand().UIResource.Hint;
            }
            this.textBoxDisplayName.Text = displayName;
            if (m_targetKey == null || (m_targetKey != null && !OSUtils.IsFunctionKey(m_targetKey.Key))) {
                this.textBoxDisplayName.Enabled = false;
            }

            // フォーカスを設定
            if (m_controlUI.Count > 0) {
                this.ActiveControl = m_controlUI[0];
            } else {
                this.ActiveControl = this.buttonOk;
            }

            // ヘルプ
            if (m_commandApi.CommandClassName == typeof(LocalExecuteCommand).FullName) {
                m_commandHelp = new CommandArgumentHelp();
                m_commandHelp.AdjustLocation(this);
                m_commandHelp.Show(this);
            }
        }

        //=========================================================================================
        // 機　能：パラメータ入力用のUIを初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void InitializeParameterUI() {
            if (m_commandApi.ArgumentList.Count == 0) {
                return;
            }
            this.labelParameter.Visible = false;
            int xPos1 = this.labelParameter.Location.X;
            int xPos2 = this.groupBoxParameter.Width - 300 - 8;
            int yPos = this.labelParameter.Location.Y;
            this.groupBoxParameter.SuspendLayout();
            for (int i = 0; i < m_commandApi.ArgumentList.Count; i++) {
                CommandArgument argument = m_commandApi.ArgumentList[i];
                if (argument.Type == CommandArgument.ArgumentType.TypeInt) {
                    CreateControlInt(argument, i, xPos1, xPos2, ref yPos);
                } else if (argument.Type == CommandArgument.ArgumentType.TypeString) {
                    if (argument.ValueRangeString.Count == 0) {
                        CreateControlString(argument, i, xPos1, xPos2, ref yPos);
                    } else {
                        CreateControlDropdown(argument, i, xPos1, xPos2, ref yPos);
                    }
                } else if (argument.Type == CommandArgument.ArgumentType.TypeBool) {
                    CreateControlBool(argument, i, xPos1, xPos2, ref yPos);
                } else if (argument.Type == CommandArgument.ArgumentType.TypeMenuItem) {
                    CreateControlMenu(argument, i, xPos1, xPos2, ref yPos);
                }
                yPos += 25;
            }
            this.groupBoxParameter.Size = new Size(this.groupBoxParameter.Size.Width, yPos + 8);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        //=========================================================================================
        // 機　能：int型引数値を入力するためのコントロールを作成する
        // 引　数：[in]argument  コマンド引数の詳細情報
        // 　　　　[in]index     コマンド引数のインデックス
        // 　　　　[in]xPos1     UI説明の表示開始X座標（左端）
        // 　　　　[in]xPos2     UI本体の表示開始X座標（中央付近）
        // 　　　　[in]yPos      UIの表示開始Y座標
        // 戻り値：なし
        //=========================================================================================
        private void CreateControlInt(CommandArgument argument, int index, int xPos1, int xPos2, ref int yPos) {
            Label label = new Label();
            NumericUpDown numericUpDown = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(numericUpDown)).BeginInit();
            label.AutoSize = true;
            label.Location = new Point(xPos1, yPos + 4);
            label.Size = new Size(8, 12);
            label.TabIndex = index * 2;
            label.Text = argument.ArgumentComment + "(&" + (index + 1) + "):";
            
            numericUpDown.Location = new System.Drawing.Point(xPos2, yPos);
            numericUpDown.Minimum = argument.ValueRangeIntMin;
            numericUpDown.Maximum = argument.ValueRangeIntMax;
            numericUpDown.Value = (int)(m_paramList[index]);
            numericUpDown.Size = new Size(120, 19);
            numericUpDown.TabIndex = index * 2 + 1;
            numericUpDown.ValueChanged += new EventHandler(NumericUpDown_ValueChanged);
            
            this.groupBoxParameter.Controls.Add(numericUpDown);
            this.groupBoxParameter.Controls.Add(label);
            ((System.ComponentModel.ISupportInitialize)(numericUpDown)).EndInit();
            m_controlUI.Add(numericUpDown);
        }

        //=========================================================================================
        // 機　能：string型引数値を入力するためのコントロールを作成する
        // 引　数：[in]argument  コマンド引数の詳細情報
        // 　　　　[in]index     コマンド引数のインデックス
        // 　　　　[in]xPos1     UI説明の表示開始X座標（左端）
        // 　　　　[in]xPos2     UI本体の表示開始X座標（中央付近）
        // 　　　　[in]yPos      UIの表示開始Y座標
        // 戻り値：なし
        //=========================================================================================
        private void CreateControlString(CommandArgument argument, int index, int xPos1, int xPos2, ref int yPos) {
            Label label = new Label();
            TextBox textBox = new TextBox();
            label.AutoSize = true;
            label.Location = new Point(xPos1, yPos + 4);
            label.Size = new Size(8, 12);
            label.TabIndex = index * 2;
            label.Text = argument.ArgumentComment + "(&" + (index + 1) + "):";

            textBox.Location = new Point(xPos2, yPos);
            textBox.Size = new Size(300, 19);
            textBox.TabIndex = index * 2 + 1;
            textBox.Text = (string)(m_paramList[index]);
            textBox.TextChanged += new EventHandler(TextBox_TextChanged);

            this.groupBoxParameter.Controls.Add(textBox);
            this.groupBoxParameter.Controls.Add(label);
            m_controlUI.Add(textBox);
        }

        //=========================================================================================
        // 機　能：string型の多岐選択による引数値を入力するためのコントロールを作成する
        // 引　数：[in]argument  コマンド引数の詳細情報
        // 　　　　[in]index     コマンド引数のインデックス
        // 　　　　[in]xPos1     UI説明の表示開始X座標（左端）
        // 　　　　[in]xPos2     UI本体の表示開始X座標（中央付近）
        // 　　　　[in]yPos      UIの表示開始Y座標
        // 戻り値：なし
        //=========================================================================================
        private void CreateControlDropdown(CommandArgument argument, int index, int xPos1, int xPos2, ref int yPos) {
            int selectedIndex;
            string[] uiList, idList;
            GetComboboxStringUIItem(index, out uiList, out idList, out selectedIndex);

            Label label = new Label();
            ComboBox comboBox = new ComboBox();
            label.AutoSize = true;
            label.Location = new Point(xPos1, yPos + 4);
            label.Size = new Size(8, 12);
            label.TabIndex = index * 2;
            label.Text = argument.ArgumentComment + "(&" + (index + 1) + "):";

            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FormattingEnabled = true;
            comboBox.Location = new Point(xPos2, yPos);
            comboBox.Size = new Size(300, 20);
            comboBox.TabIndex = index * 2 + 1;
            comboBox.Items.AddRange(uiList);
            comboBox.SelectedIndex = selectedIndex;
            comboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);

            this.groupBoxParameter.Controls.Add(comboBox);
            this.groupBoxParameter.Controls.Add(label);
            m_controlUI.Add(comboBox);
        }

        //=========================================================================================
        // 機　能：string型多岐選択項目のUI用情報を取得する
        // 引　数：[in]index          コマンド引数のインデックス
        // 　　　　[out]uiList        UIの表示名一覧を返す配列
        // 　　　　[out]idList        引数値として使用するID一覧を返す配列
        // 　　　　[out]selectedIndex ドロップダウン項目で選択中とする項目のインデックス
        // 戻り値：なし
        //=========================================================================================
        private void GetComboboxStringUIItem(int index, out string[] uiList, out string[] idList, out int selectedIndex) {
            List<CommandArgument.StringSelect> comboboxItemSetting = m_commandApi.ArgumentList[index].ValueRangeString;
            uiList = new string[comboboxItemSetting.Count];
            idList = new string[comboboxItemSetting.Count];
            selectedIndex = 0;
            string currentParam = (string)(m_paramList[index]);
            for (int i = 0; i < comboboxItemSetting.Count; i++) {
                uiList[i] = comboboxItemSetting[i].DisplayName;
                idList[i] = comboboxItemSetting[i].InnerValue;
                if (currentParam == idList[i]) {
                    selectedIndex = i;
                }
            }
        }

        //=========================================================================================
        // 機　能：bool型引数値を入力するためのコントロールを作成する
        // 引　数：[in]argument  コマンド引数の詳細情報
        // 　　　　[in]index     コマンド引数のインデックス
        // 　　　　[in]xPos1     UI説明の表示開始X座標（左端）
        // 　　　　[in]xPos2     UI本体の表示開始X座標（中央付近）
        // 　　　　[in]yPos      UIの表示開始Y座標
        // 戻り値：なし
        //=========================================================================================
        private void CreateControlBool(CommandArgument argument, int index, int xPos1, int xPos2, ref int yPos) {
            CheckBox checkBox = new CheckBox();
            checkBox.Location = new Point(xPos1, yPos);
            checkBox.Size = new Size(200, 19);
            checkBox.TabIndex = index * 2;
            checkBox.Text = (string)(argument.ArgumentComment) + "(&" + (index + 1) + "):";
            checkBox.Checked = (bool)(m_paramList[index]);
            checkBox.CheckedChanged += new EventHandler(CheckBox_CheckedChanged);
            
            this.groupBoxParameter.Controls.Add(checkBox);
            m_controlUI.Add(checkBox);
        }

        //=========================================================================================
        // 機　能：menu型による引数値を入力するためのコントロールを作成する
        // 引　数：[in]argument  コマンド引数の詳細情報
        // 　　　　[in]index     コマンド引数のインデックス
        // 　　　　[in]xPos1     UI説明の表示開始X座標（左端）
        // 　　　　[in]xPos2     UI本体の表示開始X座標（中央付近）
        // 　　　　[in]yPos      UIの表示開始Y座標
        // 戻り値：なし
        //=========================================================================================
        private void CreateControlMenu(CommandArgument argument, int index, int xPos1, int xPos2, ref int yPos) {
            int selectedIndex;
            string[] uiList, idList;
            GetComboboxMenuUIItem(index, out uiList, out idList, out selectedIndex);
            if ((string)(m_paramList[index]) != idList[index]) {
                m_paramList[index] = idList[index];
            }

            Label label = new Label();
            ComboBox comboBox = new ComboBox();
            label.AutoSize = true;
            label.Location = new Point(xPos1, yPos + 4);
            label.Size = new Size(8, 12);
            label.TabIndex = index * 2;
            label.Text = argument.ArgumentComment + "(&" + (index + 1) + "):";

            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FormattingEnabled = true;
            comboBox.Location = new Point(xPos2, yPos);
            comboBox.Size = new Size(300, 20);
            comboBox.TabIndex = index * 2 + 1;
            comboBox.Items.AddRange(uiList);
            comboBox.SelectedIndex = selectedIndex;
            comboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexMenuChanged);

            this.groupBoxParameter.Controls.Add(comboBox);
            this.groupBoxParameter.Controls.Add(label);
            m_controlUI.Add(comboBox);
        }

        //=========================================================================================
        // 機　能：menu型多岐選択項目のUI用情報を取得する
        // 引　数：[in]index          コマンド引数のインデックス
        // 　　　　[out]uiList        UIの表示名一覧を返す配列
        // 　　　　[out]idList        引数値として使用するID一覧を返す配列
        // 　　　　[out]selectedIndex ドロップダウン項目で選択中とする項目のインデックス
        // 戻り値：なし
        //=========================================================================================
        private void GetComboboxMenuUIItem(int index, out string[] uiList, out string[] idList, out int selectedIndex) {
            uiList = null;
            idList = null;
            selectedIndex= 0;
            List<MenuItemSetting> rootMenu = Program.Document.MenuSetting.CreateMenuCustomizedList(m_commandSceneType);

            uiList = new string[rootMenu.Count];
            idList = new string[rootMenu.Count];
            selectedIndex = 0;
            string currentParam = (string)(m_paramList[index]);
            for (int i = 0; i < rootMenu.Count; i++) {
                uiList[i] = rootMenu[i].ItemNameInput;
                idList[i] = rootMenu[i].ItemNameInput;
                if (currentParam == idList[i]) {
                    selectedIndex = i;
                }
            }
        }


        //=========================================================================================
        // 機　能：int型項目の値が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void NumericUpDown_ValueChanged(object sender, EventArgs evt) {
            for (int i = 0; i < m_controlUI.Count; i++) {
                Control control = m_controlUI[i];
                if (sender == control) {
                    NumericUpDown numericUpDown = (NumericUpDown)sender;
                    m_paramList[i] = (int)(numericUpDown.Value);
                    UpdateCommandExplanation();
                    break;
                }
            }
        }

        //=========================================================================================
        // 機　能：string型項目の値が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TextBox_TextChanged(object sender, EventArgs evt) {
            for (int i = 0; i < m_controlUI.Count; i++) {
                Control control = m_controlUI[i];
                if (sender == control) {
                    TextBox textBox = (TextBox)sender;
                    m_paramList[i] = textBox.Text;
                    UpdateCommandExplanation();
                    break;
                }
            }
        }

        //=========================================================================================
        // 機　能：string型多岐選択項目の値が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs evt) {
            for (int i = 0; i < m_controlUI.Count; i++) {
                Control control = m_controlUI[i];
                if (sender == control) {
                    int selectedIndexOld;
                    string[] uiList, idList;
                    GetComboboxStringUIItem(i, out uiList, out idList, out selectedIndexOld);

                    ComboBox comboBox = (ComboBox)sender;
                    m_paramList[i] = idList[comboBox.SelectedIndex];
                    UpdateCommandExplanation();
                    break;
                }
            }
        }

        //=========================================================================================
        // 機　能：bool型項目の値が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void CheckBox_CheckedChanged(object sender, EventArgs evt) {
            for (int i = 0; i < m_controlUI.Count; i++) {
                Control control = m_controlUI[i];
                if (sender == control) {
                    CheckBox checkBox = (CheckBox)sender;
                    m_paramList[i] = checkBox.Checked;
                    UpdateCommandExplanation();
                    break;
                }
            }
        }

        //=========================================================================================
        // 機　能：menu型多岐選択項目の値が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ComboBox_SelectedIndexMenuChanged(object sender, EventArgs evt) {
            for (int i = 0; i < m_controlUI.Count; i++) {
                Control control = m_controlUI[i];
                if (sender == control) {
                    int selectedIndexOld;
                    string[] uiList, idList;
                    GetComboboxMenuUIItem(i, out uiList, out idList, out selectedIndexOld);

                    ComboBox comboBox = (ComboBox)sender;
                    m_paramList[i] = idList[comboBox.SelectedIndex];
                    UpdateCommandExplanation();
                    break;
                }
            }
        }

        //=========================================================================================
        // 機　能：コマンドの説明を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateCommandExplanation() {
            this.textBoxExplanation.Text = CreateCommandExplanation(m_commandApi, m_paramList);
        }

        //=========================================================================================
        // 機　能：コマンドの説明の文字列を作成する
        // 引　数：[in]commandApi  コマンドのAPI仕様
        // 　　　　[in]paramList   パラメータ
        // 戻り値：なし
        //=========================================================================================
        public static string CreateCommandExplanation(CommandApi commandApi, object[] paramList) {
            string comment;
            if (paramList.Length > 0) {
                string[] dispParamList = new string[paramList.Length];
                for (int i = 0; i < dispParamList.Length; i++) {
                    if (paramList[i] is bool) {
                        string val;
                        if ((bool)(paramList[i])) {
                            val = "Yes";
                        } else {
                            val = "No";
                        }
                        dispParamList[i] = string.Format(Resources.DlgKeySettingOption_ArgumentQuote, val);
                    } else {
                        dispParamList[i] = string.Format(Resources.DlgKeySettingOption_ArgumentQuote, paramList[i]);
                    }
                }
                comment = string.Format(commandApi.Comment, dispParamList);
            } else {
                comment = commandApi.Comment;
            }
            return comment;
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void KeySettingOptionDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            if (m_commandHelp != null) {
                m_commandHelp.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            // ファンクションバーの表示名
            if (m_targetKey != null && OSUtils.IsFunctionKey(m_targetKey.Key)) {
                if (this.textBoxDisplayName.Text == m_newCommand.CreateActionCommand().UIResource.Hint) {
                    m_resultDisplayNameFunctionBar = null;
                } else {
                    m_resultDisplayNameFunctionBar = this.textBoxDisplayName.Text;
                }
            } else {
                m_resultDisplayNameFunctionBar = null;
            }

            // オプション
            if (this.checkBoxNextCursor.Checked) {
                m_resultCommandOption = ActionCommandOption.MoveNext;
            } else {
                m_resultCommandOption = ActionCommandOption.None;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：入力した値（入力するごとに、常に最新）
        //=========================================================================================
        public object[] ParamList {
            get {
                return m_paramList;
            }
        }

        //=========================================================================================
        // プロパティ：入力したオプション
        //=========================================================================================
        public ActionCommandOption CommandOption {
            get {
                return m_resultCommandOption;
            }
        }

        //=========================================================================================
        // プロパティ：ファンクションバーの表示名（ファンクションキー以外のとき、または、デフォルトを使用するときはnull）
        //=========================================================================================
        public string DisplayNameFunctionBar {
            get {
                return m_resultDisplayNameFunctionBar;
            }
        }
    }
}
