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
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：連番設定ダイアログ
    //=========================================================================================
    public partial class RenameSelectedSequenceDialog : Form {
        // 編集した連番の設定
        private RenameNumberingInfo m_formResult;

        // ダイアログの実装
        private RenameSequenceDialogImpl m_dialogImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]numberingInfo  連番の設定の初期値
        // 戻り値：なし
        //=========================================================================================
        public RenameSelectedSequenceDialog(RenameNumberingInfo numberingInfo) {
            InitializeComponent();

            m_dialogImpl = new RenameSequenceDialogImpl(this, numberingInfo, this.textBoxFileName, this.numericStart,
                        this.numericIncrease, this.comboBoxRadix, this.comboBoxWidth, this.textBoxSample,
                        new RenameSequenceDialogImpl.GetSampleFileNameDelegate(GetSampleFileName));
        }

        //=========================================================================================
        // 機　能：サンプル表示用のファイル名を返す
        // 引　数：[in]numberingInfo  番号付けの情報
        // 　　　　[in]modifyCtx      連番のための情報
        // 戻り値：サンプル表示用のファイル1件(表示できないときnull)
        //=========================================================================================
        private string GetSampleFileName(RenameNumberingInfo numberingInfo, ModifyFileInfoContext modifyCtx) {
            string fileName = RenameNumberingInfo.CreateSequenceString(numberingInfo, modifyCtx);
            fileName += "." + Resources.DlgRenameSel_SeqSampleExt;
            return fileName;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_formResult = m_dialogImpl.GetUIValue(true);
            if (m_formResult == null) {
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RenameSelectedSequenceDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_formResult == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // プロパティ：編集した連番の設定
        //=========================================================================================
        public RenameNumberingInfo NumberingInfo {
            get {
                return m_formResult;
            }
        }

        //=========================================================================================
        // プロパティ：連番入力のダイアログ実装
        //=========================================================================================
        public class RenameSequenceDialogImpl {
            // 親フォーム
            private Form m_parent;

            // ファイル名の入力コンポーネント
            private TextBox m_textBoxFileName;

            // 開始番号の入力コンポーネント
            private NumericUpDown m_numericStart;

            // 増分の入力コンポーネント
            private NumericUpDown m_numericIncrease;

            // 基数の入力コンポーネント
            private ComboBox m_comboBoxRadix;

            // 桁数の入力コンポーネント
            private ComboBox m_comboBoxWidth;

            // サンプルの出力コンポーネント
            private TextBox m_textBoxSample;

            // サンプル取得のdelegate
            private GetSampleFileNameDelegate m_getSampleDelegate;

            // サンプル取得のdelegate
            public delegate string GetSampleFileNameDelegate(RenameNumberingInfo numberingInfo, ModifyFileInfoContext modifyCtx);

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent            親フォーム
            // 　　　　[in]numberingInfo     フォームの初期値
            // 　　　　[in]textBoxFileName   ファイル名の入力コンポーネント
            // 　　　　[in]numericStart      開始番号の入力コンポーネント
            // 　　　　[in]numericIncrease   増分の入力コンポーネント
            // 　　　　[in]comboBoxRadix     基数の入力コンポーネント
            // 　　　　[in]comboBoxWidth     桁数の入力コンポーネント
            // 　　　　[in]textBoxSample     サンプルの出力コンポーネント
            // 　　　　[in]getSampleDelegate サンプル取得のdelegate
            // 戻り値：なし
            //=========================================================================================
            public RenameSequenceDialogImpl(Form parent, RenameNumberingInfo numberingInfo, TextBox textBoxFileName,
                        NumericUpDown numericStart, NumericUpDown numericIncrease, ComboBox comboBoxRadix,
                        ComboBox comboBoxWidth, TextBox textBoxSample, GetSampleFileNameDelegate getSampleDelegate) {
                m_parent = parent;
                m_textBoxFileName = textBoxFileName;
                m_numericStart = numericStart;
                m_numericIncrease = numericIncrease;
                m_comboBoxRadix = comboBoxRadix;
                m_comboBoxWidth = comboBoxWidth;
                m_textBoxSample = textBoxSample;
                m_getSampleDelegate = getSampleDelegate;

                m_textBoxFileName.Text = numberingInfo.FileNameFormatter;
                m_textBoxFileName.TextChanged += new EventHandler(textBoxFileName_TextChanged);

                m_numericStart.Minimum = 0;
                m_numericStart.Maximum = 0x10000;
                m_numericStart.Value = numberingInfo.StartNumber;
                m_numericStart.ValueChanged += new EventHandler(Numeric_ValueChanged);

                m_numericIncrease.Minimum = 1;
                m_numericIncrease.Maximum = 0x100;
                m_numericIncrease.Value = numberingInfo.IncreaseNumber;
                m_numericIncrease.ValueChanged += new EventHandler(Numeric_ValueChanged);

                string[] itemRadix = {
                    Resources.DlgRenameSel_SeqRadix8,       // 8進数
                    Resources.DlgRenameSel_SeqRadix10,      // 10進数
                    Resources.DlgRenameSel_SeqRadix16u,	    // 16進数（大文字）
                    Resources.DlgRenameSel_SeqRadix16l,	    // 16進数（小文字）
                };
                m_comboBoxRadix.Items.AddRange(itemRadix);
                m_comboBoxRadix.SelectedIndex = RadixToComboboxIndex(numberingInfo.Radix);
                m_comboBoxRadix.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);

                string[] itemDigit = {
                    Resources.DlgRenameSel_SeqDigit1,       // 最低桁数
                    Resources.DlgRenameSel_SeqDigit2,	    // 2桁（先頭に0を補完）
                    Resources.DlgRenameSel_SeqDigit3,	    // 3桁（先頭に0を補完）
                    Resources.DlgRenameSel_SeqDigit4,	    // 4桁（先頭に0を補完）
                    Resources.DlgRenameSel_SeqDigit8,	    // 8桁（先頭に0を補完）
                };
                m_comboBoxWidth.Items.AddRange(itemDigit);
                m_comboBoxWidth.SelectedIndex = WidthToComboboxIndex(numberingInfo.Width);
                m_comboBoxWidth.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
                UpdateSample();
            }

            //=========================================================================================
            // 機　能：基数をコンボボックスの項目に変換する
            // 引　数：[in]radix  基数
            // 戻り値：コンボボックスのインデックス
            //=========================================================================================
            private int RadixToComboboxIndex(NumericRadix radix) {
                if (radix == NumericRadix.Radix8) {
                    return 0;
                } else if (radix == NumericRadix.Radix10) {
                    return 1;
                } else if (radix == NumericRadix.Radix16Upper) {
                    return 2;
                } else if (radix == NumericRadix.Radix16Lower) {
                    return 3;
                } else {
                    return 1;
                }
            }

            //=========================================================================================
            // 機　能：コンボボックスの項目を基数に変換する
            // 引　数：[in]index  コンボボックスのインデックス
            // 戻り値：基数
            //=========================================================================================
            private NumericRadix ComboboxIndexToRadix(int index) {
                switch (index) {
                    case 0:
                        return NumericRadix.Radix8;
                    case 1:
                        return NumericRadix.Radix10;
                    case 2:
                        return NumericRadix.Radix16Upper;
                    case 3:
                        return NumericRadix.Radix16Lower;
                }
                return NumericRadix.Radix10;
            }

            //=========================================================================================
            // 機　能：桁数をコンボボックスの項目に変換する
            // 引　数：[in]width  桁数
            // 戻り値：コンボボックスのインデックス
            //=========================================================================================
            private int WidthToComboboxIndex(int width) {
                switch (width) {
                    case 1:
                        return 0;
                    case 2:
                        return 1;
                    case 3 :
                        return 2;
                    case 4:
                        return 3;
                    case 8:
                        return 4;
                }
                return 3;
            }

            //=========================================================================================
            // 機　能：コンボボックスの項目を桁数に変換する
            // 引　数：[in]index  コンボボックスのインデックス
            // 戻り値：桁数
            //=========================================================================================
            private int ComboboxIndexToWidth(int index) {
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 2;
                    case 2:
                        return 3;
                    case 3:
                        return 4;
                    case 4:
                        return 8;
                }
                return 4;
            }

            //=========================================================================================
            // 機　能：フォーマットの入力値が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void textBoxFileName_TextChanged(object sender, EventArgs evt) {
                UpdateSample();
            }

            //=========================================================================================
            // 機　能：数値入力の値が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void Numeric_ValueChanged(object sender, EventArgs evt) {
                UpdateSample();
            }

            //=========================================================================================
            // 機　能：コンボボックスの選択項目が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void ComboBox_SelectedIndexChanged(object sender, EventArgs evt) {
                UpdateSample();
            }

            //=========================================================================================
            // 機　能：UIの値を取得する
            // 引　数：[in]displayMessage  メッセージを表示するときtrue
            // 戻り値：取り込んだ値（エラーのときnull）
            //=========================================================================================
            public RenameNumberingInfo GetUIValue(bool displayMessage) {
                // 値を取り込む
                RenameNumberingInfo numberingInfo = new RenameNumberingInfo();
                numberingInfo.FileNameFormatter = m_textBoxFileName.Text;
                numberingInfo.StartNumber = (int)m_numericStart.Value;
                numberingInfo.IncreaseNumber = (int)m_numericIncrease.Value;
                numberingInfo.Radix = ComboboxIndexToRadix(m_comboBoxRadix.SelectedIndex);
                numberingInfo.Width = ComboboxIndexToWidth(m_comboBoxWidth.SelectedIndex);

                // 内容を確認
                int questionCount = StringUtils.GetCharCount(numberingInfo.FileNameFormatter, '?');
                if (questionCount != 1) {
                    if (displayMessage) {
                        InfoBox.Warning(m_parent, Resources.DlgRenameSel_SeqInvalidFileNameQuestion);
                    }
                    return null;
                }
                if (numberingInfo.FileNameFormatter.IndexOf('*') != -1) {
                    if (displayMessage) {
                        InfoBox.Warning(m_parent, Resources.DlgRenameSel_SeqInvalidFileNameAster);
                    }
                    return null;
                }

                return numberingInfo;
            }

            //=========================================================================================
            // 機　能：ファイル名のサンプルを更新する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void UpdateSample() {
                // UIの値をm_numeringInfoに取り込む
                RenameNumberingInfo numberingInfo = GetUIValue(false);
                if (numberingInfo == null) {
                    m_textBoxSample.Text = Resources.DlgRenameSel_SeqInvalidFileNameSample;
                    return;
                }

                // サンプルを作成
                StringBuilder sample = new StringBuilder();
                ModifyFileInfoContext modifyCtx = new ModifyFileInfoContext();
                for (int i = 0; i < 20; i++) {
                    string fileName = m_getSampleDelegate(numberingInfo, modifyCtx);
                    if (fileName == null) {
                        sample = new StringBuilder();
                        break;
                    }
                    sample.Append(fileName);
                    sample.Append("\r\n");
                }
                m_textBoxSample.Text = sample.ToString();
            }
        }
    }
}
