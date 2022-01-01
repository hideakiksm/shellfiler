using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：折り返し設定の変更ダイアログ
    //=========================================================================================
    public partial class ViewerLineWidthDialog : Form {
        // ダイアログの実装
        private Impl m_impl;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting  設定情報
        // 戻り値：なし
        //=========================================================================================
        public ViewerLineWidthDialog(TextViewerLineBreakSetting setting) {
            InitializeComponent();

            m_impl = new Impl(this, setting);
            m_impl.Initialize(this.radioButtonChar, this.radioButtonPixel, this.radioButtonNone,
                              this.numericChar, this.numericPixel);

            switch (setting.LineBreakMode) {
                case TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByChar:
                    this.ActiveControl = this.numericChar;
                    this.numericChar.Select(0, (this.numericChar.Value).ToString().Length);
                    break;
                case TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByPixel:
                    this.ActiveControl = this.numericPixel;
                    this.numericPixel.Select(0, (this.numericPixel.Value).ToString().Length);
                    break;
                case TextViewerLineBreakSetting.TextViewerLineBreakMode.NoBreak:
                    this.ActiveControl = this.radioButtonNone;
                    break;
            }
        }

        //=========================================================================================
        // 機　能：ラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButton_CheckedChanged(object sender, EventArgs evt) {
            m_impl.OnRadioButtonCheckedChanged();
        }

        //=========================================================================================
        // 機　能：OKボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            bool success = m_impl.OnOk();
            if (!success) {
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：編集した折り返し設定（コンストラクタとは別インスタンス）
        //=========================================================================================
        public TextViewerLineBreakSetting Setting {
            get {
                return m_impl.Setting;
            }
        }

        //=========================================================================================
        // クラス：ダイアログの実装（オプションダイアログとの共通化）
        //=========================================================================================
        public class Impl {
            // 元の設定情報
            private TextViewerLineBreakSetting m_orgSetting;

            // 編集した折り返し設定（コンストラクタとは別インスタンス）
            private TextViewerLineBreakSetting m_setting;

            // 親となるフォーム
            private Form m_parent;

            // ラジオボタン 文字数指定
            private RadioButton m_radioButtonChar;
            
            // ラジオボタン ピクセル数指定
            private RadioButton m_radioButtonPixel;

            // ラジオボタン 折返しなし
            private RadioButton m_radioButtonNone;

            // 数値入力 文字数指定の値
            private NumericUpDown m_numericChar;

            // 数値入力 ピクセル数の値
            private NumericUpDown m_numericPixel;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent   親となるフォーム
            // 　　　　[in]setting  設定情報
            // 戻り値：なし
            //=========================================================================================
            public Impl(Form parent, TextViewerLineBreakSetting setting) {
                m_parent = parent;
                m_orgSetting = setting;
                m_setting = (TextViewerLineBreakSetting)setting.Clone();
            }

            //=========================================================================================
            // 機　能：初期化する
            // 引　数：[in]setting  設定情報
            // 戻り値：なし
            //=========================================================================================
            public void Initialize(RadioButton radioButtonChar, RadioButton radioButtonPixel, RadioButton radioButtonNone,
                                   NumericUpDown numericChar, NumericUpDown numericPixel) {
                m_radioButtonChar = radioButtonChar;
                m_radioButtonPixel = radioButtonPixel;
                m_radioButtonNone = radioButtonNone;
                m_numericChar = numericChar;
                m_numericPixel = numericPixel;
                
                m_numericChar.Minimum = 0;
                m_numericChar.Maximum = TextViewerLineBreakSetting.MAX_TEXT_BREAK_CHAR_COUNT;
                m_numericChar.Value = m_setting.BreakCharCount;
                m_numericPixel.Minimum = 0;
                m_numericPixel.Maximum = TextViewerLineBreakSetting.MAX_TEXT_BREAK_PIXEL_COUNT;
                m_numericPixel.Value = m_setting.BreakPixel;

                switch (m_orgSetting.LineBreakMode) {
                    case TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByChar:
                        m_radioButtonChar.Checked = true;
                        m_numericChar.Enabled = true;
                        m_numericPixel.Enabled = false;
                        break;
                    case TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByPixel:
                        m_radioButtonPixel.Checked = true;
                        m_numericChar.Enabled = false;
                        m_numericPixel.Enabled = true;
                        break;
                    case TextViewerLineBreakSetting.TextViewerLineBreakMode.NoBreak:
                        m_radioButtonNone.Checked = true;
                        m_numericChar.Enabled = false;
                        m_numericPixel.Enabled = false;
                        break;
                }
            }

            //=========================================================================================
            // 機　能：ラジオボタンが変更されたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnRadioButtonCheckedChanged() {
                if (m_radioButtonChar.Checked) {
                    m_numericChar.Enabled = true;
                    m_numericPixel.Enabled = false;
                } else if (m_radioButtonPixel.Checked) {
                    m_numericChar.Enabled = false;
                    m_numericPixel.Enabled = true;
                } else {
                    m_numericChar.Enabled = false;
                    m_numericPixel.Enabled = false;
                }
            }

            //=========================================================================================
            // 機　能：OKボタンが押されたときの処理を行う
            // 引　数：なし
            // 戻り値：正しく取得できたときtrue
            //=========================================================================================
            public bool OnOk() {
                if (m_radioButtonChar.Checked) {
                    int breakCharCount = (int)(m_numericChar.Value);
                    if (breakCharCount < 0 || breakCharCount > 2 && breakCharCount < TextViewerLineBreakSetting.MIN_TEXT_BREAK_CHAR_COUNT || breakCharCount > TextViewerLineBreakSetting.MAX_TEXT_BREAK_CHAR_COUNT) {
                        string message = string.Format(Resources.DlgViewerLineWidth_ByCharError, TextViewerLineBreakSetting.MIN_TEXT_BREAK_CHAR_COUNT, TextViewerLineBreakSetting.MAX_TEXT_BREAK_CHAR_COUNT);
                        InfoBox.Warning(m_parent, message);
                        return false;
                    }
                    m_setting.BreakCharCount = breakCharCount;
                } else {
                    m_setting.BreakCharCount = m_orgSetting.BreakCharCount;
                }

                if (m_radioButtonPixel.Checked) {
                    int breakPixelCount = (int)(m_numericPixel.Value);
                    if (breakPixelCount < 0 || breakPixelCount > 2 && breakPixelCount < TextViewerLineBreakSetting.MIN_TEXT_BREAK_PIXEL_COUNT || breakPixelCount > TextViewerLineBreakSetting.MAX_TEXT_BREAK_PIXEL_COUNT) {
                        string message = string.Format(Resources.DlgViewerLineWidth_ByPixelError, TextViewerLineBreakSetting.MIN_TEXT_BREAK_PIXEL_COUNT, TextViewerLineBreakSetting.MAX_TEXT_BREAK_PIXEL_COUNT);
                        InfoBox.Warning(m_parent, message);
                        return false;
                    }
                    m_setting.BreakPixel = breakPixelCount;
                } else {
                    m_setting.BreakPixel = m_orgSetting.BreakPixel;
                }

                if (m_radioButtonChar.Checked) {
                    m_setting.LineBreakMode = TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByChar;
                } else if (m_radioButtonPixel.Checked) {
                    m_setting.LineBreakMode = TextViewerLineBreakSetting.TextViewerLineBreakMode.BreakByPixel;
                } else {
                    m_setting.LineBreakMode = TextViewerLineBreakSetting.TextViewerLineBreakMode.NoBreak;
                }
                return true;
            }

            //=========================================================================================
            // プロパティ：編集した折り返し設定（コンストラクタとは別インスタンス）
            //=========================================================================================
            public TextViewerLineBreakSetting Setting {
                get {
                    return m_setting;
                }
            }
        }
    }
}
