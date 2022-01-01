using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Locale;

namespace ShellFiler.UI.Dialog.FileViewer {
    //=========================================================================================
    // クラス：ダンプビューアでの形式を指定してコピーダイアログ
    //=========================================================================================
    public partial class ViewerDumpCopyAsDialog : Form {
        // ダイアログの実装
        private Impl m_impl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting  デフォルトの設定
        // 　　　　[in]buffer   サンプル表示用のバッファ
        // 　　　　[in]start    開始アドレス
        // 　　　　[in]end      終了アドレス
        // 　　　　[in]width    現在の画面幅
        // 　　　　[in]encoding テキストモードでのエンコーディング方式
        // 戻り値：なし
        //=========================================================================================
        public ViewerDumpCopyAsDialog(DumpClipboardSetting setting, byte[] buffer, int start, int end, int width, EncodingType encoding) {
            InitializeComponent();
            m_impl = new Impl(setting, buffer, start, end, width, encoding);
            m_impl.Initialize(this.radioButtonDump, this.radioButtonText, this.radioButtonScreen, this.radioButtonView, this.radioButtonBase64,
                              this.comboBoxDumpRadix, this.comboBoxDumpWidth, this.textBoxDumpPrefix, this.textBoxDumpPostfix,
                              this.textBoxDumpSeparator, this.numericDumpLineWidth, this.numericBase64Folding, this.textBoxSample,
                              this.buttonDumpDefault, this.buttonDumpC, this.buttonDumpBasic);
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_impl.OnOk();
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：設定情報（コンストラクタとは別インスタンス）
        //=========================================================================================
        public DumpClipboardSetting Setting {
            get {
                return m_impl.Setting;
            }
        }

        //=========================================================================================
        // プロパティ：ダイアログの実装（オプションダイアログとの共通化）
        //=========================================================================================
        public class Impl {
            // サンプル表示の最大バイト数
            private const int MAX_SAMPLE_LENGTH = 1024;

            // 設定情報（コンストラクタとは別インスタンス）
            private DumpClipboardSetting m_setting = new DumpClipboardSetting();

            // サンプル表示用のバッファ
            private byte[] m_buffer;

            // 開始アドレス
            private int m_startAddress;

            // 終了アドレス
            private int m_endAddress;

            // サンプル用に末端のデータを切り詰めたときtrue
            private bool m_trimLastData;

            // 現在の画面幅
            private int m_screenWidth;

            // エンコードの種類
            private EncodingType m_encodingType;


            // 形式選択 ダンプ
            private RadioButton m_radioButtonDump;

            // 形式選択 テキスト
            private RadioButton m_radioButtonText;

            // 形式選択 画面表記
            private RadioButton m_radioButtonScreen;

            // 形式選択 画面表示イメージ
            private RadioButton m_radioButtonView;

            // 形式選択 Base64
            private RadioButton m_radioButtonBase64;

            // 基数
            private ComboBox m_comboBoxDumpRadix;

            // 数値の桁数
            private ComboBox m_comboBoxDumpWidth;
            
            // 接頭文字
            private TextBox m_textBoxDumpPrefix;

            // 接尾文字
            private TextBox m_textBoxDumpPostfix;

            // セパレータ
            private TextBox m_textBoxDumpSeparator;

            // 改行間隔
            private NumericUpDown m_numericDumpLineWidth;

            // フォールディング
            private NumericUpDown m_numericBase64Folding;

            // サンプル
            private TextBox m_textBoxSample;

            // 形式指定 デフォルト
            private Button m_buttonDumpDefault;

            // 形式指定 C/Java
            private Button m_buttonDumpC;

            // 形式指定 Basic
            private Button m_buttonDumpBasic;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]setting     デフォルトの設定
            // 　　　　[in]buffer      サンプル表示用のバッファ
            // 　　　　[in]start       開始アドレス
            // 　　　　[in]end         終了アドレス
            // 　　　　[in]screenWidth 現在の画面幅
            // 　　　　[in]encoding    テキストモードでのエンコーディング方式
            // 戻り値：なし
            //=========================================================================================
            public Impl(DumpClipboardSetting setting, byte[] buffer, int start, int end, int screenWidth, EncodingType encodingType) {
                m_setting = (DumpClipboardSetting)(setting.Clone());
                m_buffer = buffer;
                m_startAddress = start;
                m_screenWidth = screenWidth;
                m_encodingType = encodingType;

                if (end - start > MAX_SAMPLE_LENGTH) {
                    m_endAddress = start + MAX_SAMPLE_LENGTH;
                    m_trimLastData = true;
                } else {
                    m_endAddress = end;
                    m_trimLastData = false;
                }
            }

            //=========================================================================================
            // 機　能：初期化する
            // 引　数：[in]radioButtonDump       形式選択 ダンプ
            // 　　　　[in]radioButtonText       形式選択 テキスト
            // 　　　　[in]radioButtonScreen     形式選択 画面表記
            // 　　　　[in]radioButtonView       形式選択 表示イメージ
            // 　　　　[in]radioButtonBase64     形式選択 Base64
            // 　　　　[in]comboBoxDumpRadix     基数
            // 　　　　[in]comboBoxDumpWidth     数値の桁数
            // 　　　　[in]textBoxDumpPrefix     接頭文字
            // 　　　　[in]textBoxDumpPostfix    接尾文字
            // 　　　　[in]textBoxDumpSeparator  セパレータ
            // 　　　　[in]numericDumpLineWidth  改行間隔
            // 　　　　[in]numericBase64Folding  フォールディング
            // 　　　　[in]textBoxSample         サンプル
            // 　　　　[in]buttonDumpDefault     形式指定 デフォルト
            // 　　　　[in]buttonDumpC           形式指定 C/Java
            // 　　　　[in]buttonDumpBasic       形式指定 Basic
            // 戻り値：なし
            //=========================================================================================
            public void Initialize(RadioButton radioButtonDump, RadioButton radioButtonText, RadioButton radioButtonScreen, RadioButton radioButtonView, RadioButton radioButtonBase64,
                                   ComboBox comboBoxDumpRadix, ComboBox comboBoxDumpWidth, TextBox textBoxDumpPrefix, TextBox textBoxDumpPostfix,
                                   TextBox textBoxDumpSeparator, NumericUpDown numericDumpLineWidth, NumericUpDown numericBase64Folding, TextBox textBoxSample,
                                   Button buttonDumpDefault, Button buttonDumpC, Button buttonDumpBasic) {
                m_radioButtonDump = radioButtonDump;
                m_radioButtonText = radioButtonText;
                m_radioButtonScreen = radioButtonScreen;
                m_radioButtonView = radioButtonView;
                m_radioButtonBase64 = radioButtonBase64;
                m_comboBoxDumpRadix = comboBoxDumpRadix;
                m_comboBoxDumpWidth = comboBoxDumpWidth;
                m_textBoxDumpPrefix = textBoxDumpPrefix;
                m_textBoxDumpPostfix = textBoxDumpPostfix;
                m_textBoxDumpSeparator = textBoxDumpSeparator;
                m_numericDumpLineWidth = numericDumpLineWidth;
                m_numericBase64Folding = numericBase64Folding;
                m_textBoxSample = textBoxSample;
                m_buttonDumpDefault = buttonDumpDefault;
                m_buttonDumpC = buttonDumpC;
                m_buttonDumpBasic = buttonDumpBasic;

                string[] itemRadix = {
                    Resources.DlgDumpCopyAs_Radix8,         // 8進数
                    Resources.DlgDumpCopyAs_Radix10,        // 10進数
                    Resources.DlgDumpCopyAs_Radix16u,	    // 16進数（大文字）
                    Resources.DlgDumpCopyAs_Radix16l,	    // 16進数（小文字）
                };
                m_comboBoxDumpRadix.Items.AddRange(itemRadix);
                string[] itemDigit = {
                    Resources.DlgDumpCopyAs_Digit1,	        // 最低桁数
                    Resources.DlgDumpCopyAs_Digit2z,	    // 2桁（先頭に0を補完）
                    Resources.DlgDumpCopyAs_Digit2s,	    // 2桁（先頭に空白を補完）
                    Resources.DlgDumpCopyAs_Digit3z,	    // 3桁（先頭に0を補完）
                    Resources.DlgDumpCopyAs_Digit3s,	    // 3桁（先頭に空白を補完）
                    Resources.DlgDumpCopyAs_Digit4z,	    // 4桁（先頭に0を補完）
                    Resources.DlgDumpCopyAs_Digit4s,	    // 4桁（先頭に空白を補完）
                    Resources.DlgDumpCopyAs_Digit8z,	    // 8桁（先頭に0を補完）
                    Resources.DlgDumpCopyAs_Digit8s,	    // 8桁（先頭に空白を補完）
                };
                m_comboBoxDumpWidth.Items.AddRange(itemDigit);

                // 入力値の反映
                switch (m_setting.Mode) {
                    case DumpMode.Dump:
                        m_radioButtonDump.Checked = true;
                        break;
                    case DumpMode.Text:
                        m_radioButtonText.Checked = true;
                        break;
                    case DumpMode.Screen:
                        m_radioButtonScreen.Checked = true;
                        break;
                    case DumpMode.View:
                        m_radioButtonView.Checked = true;
                        break;
                    case DumpMode.Base64:
                        m_radioButtonBase64.Checked = true;
                        break;
                }

                if (m_setting.DumpRadix == NumericRadix.Radix8) {
                    m_comboBoxDumpRadix.SelectedIndex = 0;
                } else if (m_setting.DumpRadix == NumericRadix.Radix10) {
                    m_comboBoxDumpRadix.SelectedIndex = 1;
                } else if (m_setting.DumpRadix == NumericRadix.Radix16Upper) {
                    m_comboBoxDumpRadix.SelectedIndex = 2;
                } else if (m_setting.DumpRadix == NumericRadix.Radix16Lower) {
                    m_comboBoxDumpRadix.SelectedIndex = 3;
                }
                if (m_setting.DumpWidth == 0) {
                    m_comboBoxDumpWidth.SelectedIndex = 0;
                } else if (m_setting.DumpWidth == 2 && m_setting.DumpZeroPadding) {
                    m_comboBoxDumpWidth.SelectedIndex = 1;
                } else if (m_setting.DumpWidth == 2 && !m_setting.DumpZeroPadding) {
                    m_comboBoxDumpWidth.SelectedIndex = 2;
                } else if (m_setting.DumpWidth == 3 && m_setting.DumpZeroPadding) {
                    m_comboBoxDumpWidth.SelectedIndex = 3;
                } else if (m_setting.DumpWidth == 3 && !m_setting.DumpZeroPadding) {
                    m_comboBoxDumpWidth.SelectedIndex = 4;
                } else if (m_setting.DumpWidth == 4 && m_setting.DumpZeroPadding) {
                    m_comboBoxDumpWidth.SelectedIndex = 5;
                } else if (m_setting.DumpWidth == 4 && !m_setting.DumpZeroPadding) {
                    m_comboBoxDumpWidth.SelectedIndex = 6;
                } else if (m_setting.DumpWidth == 8 && m_setting.DumpZeroPadding) {
                    m_comboBoxDumpWidth.SelectedIndex = 7;
                } else if (m_setting.DumpWidth == 8 && !m_setting.DumpZeroPadding) {
                    m_comboBoxDumpWidth.SelectedIndex = 8;
                } else {
                    m_comboBoxDumpWidth.SelectedIndex = 0;
                }
                m_textBoxDumpPrefix.Text = m_setting.DumpPrefixString;
                m_textBoxDumpPostfix.Text = m_setting.DumpPostfixString;
                m_textBoxDumpSeparator.Text = m_setting.DumpSeparator;
                m_numericDumpLineWidth.Value = m_setting.DumpLineWidth;
                m_numericBase64Folding.Value = m_setting.Base64FoldingWidth;

                // イベントの設定
                m_radioButtonDump.CheckedChanged += new EventHandler(this.Item_SettingValueChanged);
                m_radioButtonText.CheckedChanged += new EventHandler(this.Item_SettingValueChanged);
                m_radioButtonScreen.CheckedChanged += new EventHandler(this.Item_SettingValueChanged);
                m_radioButtonBase64.CheckedChanged += new EventHandler(this.Item_SettingValueChanged);
                m_radioButtonView.CheckedChanged += new EventHandler(this.Item_SettingValueChanged);
                m_comboBoxDumpRadix.SelectedIndexChanged += new EventHandler(this.Item_SettingValueChanged);
                m_textBoxDumpPrefix.TextChanged += new EventHandler(this.Item_SettingValueChanged);
                m_textBoxDumpPostfix.TextChanged += new EventHandler(this.Item_SettingValueChanged);
                m_textBoxDumpSeparator.TextChanged += new EventHandler(this.Item_SettingValueChanged);
                m_numericDumpLineWidth.ValueChanged += new EventHandler(this.Item_SettingValueChanged);
                m_numericBase64Folding.ValueChanged += new EventHandler(this.Item_SettingValueChanged);
                m_comboBoxDumpWidth.SelectedIndexChanged += new EventHandler(this.Item_SettingValueChanged);
                m_buttonDumpDefault.Click += new EventHandler(buttonDumpDefault_Click);
                m_buttonDumpC.Click += new EventHandler(buttonDumpC_Click);
                m_buttonDumpBasic.Click += new EventHandler(buttonDumpBasic_Click);

                // 状態を切り替え
                ChangeEnabled();
                Item_SettingValueChanged(null, null);
            }

            //=========================================================================================
            // 機　能：モード変更時の状態をダイアログ項目に設定する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void ChangeEnabled() {
                // モードの読み込み
                bool enableDump = false;
                bool enableBase64 = false;
                switch (m_setting.Mode) {
                    case DumpMode.Dump:
                        enableDump = true;
                        enableBase64 = false;
                        break;
                    case DumpMode.Text:
                        enableDump = false;
                        enableBase64 = false;
                        break;
                    case DumpMode.Screen:
                        enableDump = false;
                        enableBase64 = false;
                        break;
                    case DumpMode.View:
                        enableDump = false;
                        enableBase64 = false;
                        break;
                    case DumpMode.Base64:
                        enableDump = false;
                        enableBase64 = true;
                        break;
                }

                // 状態を切り替え
                m_comboBoxDumpRadix.Enabled = enableDump;
                m_comboBoxDumpWidth.Enabled = enableDump;
                m_textBoxDumpPrefix.Enabled = enableDump;
                m_textBoxDumpPostfix.Enabled = enableDump;
                m_textBoxDumpSeparator.Enabled = enableDump;
                m_numericDumpLineWidth.Enabled = enableDump;
                m_buttonDumpDefault.Enabled = enableDump;
                m_buttonDumpC.Enabled = enableDump;
                m_buttonDumpBasic.Enabled = enableDump;
                m_numericBase64Folding.Enabled = enableBase64;
            }

            //=========================================================================================
            // 機　能：項目の値をダイアログから取得する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void GetItemValue() {
                if (m_radioButtonText.Checked) {
                    m_setting.Mode = DumpMode.Text;
                } else if (m_radioButtonBase64.Checked) {
                    m_setting.Mode = DumpMode.Base64;
                } else if (m_radioButtonScreen.Checked) {
                    m_setting.Mode = DumpMode.Screen;
                } else if (m_radioButtonView.Checked) {
                    m_setting.Mode = DumpMode.View;
                } else {
                    m_setting.Mode = DumpMode.Dump;
                }

                switch (m_comboBoxDumpRadix.SelectedIndex) {
                    case 0:
                        m_setting.DumpRadix = NumericRadix.Radix8;
                        break;
                    case 1:
                        m_setting.DumpRadix = NumericRadix.Radix10;
                        break;
                    case 2:
                        m_setting.DumpRadix = NumericRadix.Radix16Upper;
                        break;
                    case 3:
                        m_setting.DumpRadix = NumericRadix.Radix16Lower;
                        break;
                    default:
                        m_setting.DumpRadix = NumericRadix.Radix10;
                        break;
                }

                switch (m_comboBoxDumpWidth.SelectedIndex) {
                    case 0:
                        m_setting.DumpWidth = 0;
                        m_setting.DumpZeroPadding = false;
                        break;
                    case 1:
                        m_setting.DumpWidth = 2;
                        m_setting.DumpZeroPadding = true;
                        break;
                    case 2:
                        m_setting.DumpWidth = 2;
                        m_setting.DumpZeroPadding = false;
                        break;
                    case 3:
                        m_setting.DumpWidth = 3;
                        m_setting.DumpZeroPadding = true;
                        break;
                    case 4:
                        m_setting.DumpWidth = 3;
                        m_setting.DumpZeroPadding = false;
                        break;
                    case 5:
                        m_setting.DumpWidth = 4;
                        m_setting.DumpZeroPadding = true;
                        break;
                    case 6:
                        m_setting.DumpWidth = 4;
                        m_setting.DumpZeroPadding = false;
                        break;
                    case 7:
                        m_setting.DumpWidth = 8;
                        m_setting.DumpZeroPadding = true;
                        break;
                    case 8:
                        m_setting.DumpWidth = 8;
                        m_setting.DumpZeroPadding = false;
                        break;
                }
                m_setting.DumpPrefixString = m_textBoxDumpPrefix.Text;
                m_setting.DumpPostfixString = m_textBoxDumpPostfix.Text;
                m_setting.DumpSeparator = m_textBoxDumpSeparator.Text;
                m_setting.DumpLineWidth = (int)(m_numericDumpLineWidth.Value);
                m_setting.Base64FoldingWidth = (int)(m_numericBase64Folding.Value);
                m_setting.ModifyValue();
            }

            //=========================================================================================
            // 機　能：項目の入力値が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void Item_SettingValueChanged(object sender, EventArgs evt) {
                GetItemValue();
                ChangeEnabled();
                DumpClipboardFormatter formatter = new DumpClipboardFormatter();
                string text = formatter.Format(m_buffer, m_startAddress, m_endAddress, m_setting, m_screenWidth, m_encodingType);
                if (m_trimLastData) {
                    text += "...";
                }
                m_textBoxSample.Text = text;
            }

            //=========================================================================================
            // 機　能：ダンプのデフォルトボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDumpDefault_Click(object sender, EventArgs evt) {
                m_comboBoxDumpRadix.SelectedIndex = 2;
                m_comboBoxDumpWidth.SelectedIndex = 1;
                m_textBoxDumpPrefix.Text = "";
                m_textBoxDumpPostfix.Text = "";
            }

            //=========================================================================================
            // 機　能：ダンプのCボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDumpC_Click(object sender, EventArgs evt) {
                m_comboBoxDumpRadix.SelectedIndex = 2;
                m_comboBoxDumpWidth.SelectedIndex = 1;
                m_textBoxDumpPrefix.Text = "0x";
                m_textBoxDumpPostfix.Text = "";
            }

            //=========================================================================================
            // 機　能：ダンプのBASICボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDumpBasic_Click(object sender, EventArgs evt) {
                m_comboBoxDumpRadix.SelectedIndex = 2;
                m_comboBoxDumpWidth.SelectedIndex = 1;
                m_textBoxDumpPrefix.Text = "&H";
                m_textBoxDumpPostfix.Text = "";
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnOk() {
                Item_SettingValueChanged(null, null);
            }

            //=========================================================================================
            // プロパティ：設定情報（コンストラクタとは別インスタンス）
            //=========================================================================================
            public DumpClipboardSetting Setting {
                get {
                    return m_setting;
                }
            }
        }
    }
}
