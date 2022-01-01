using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：形式を指定してファイル名をコピーのダイアログ
    //=========================================================================================
    public partial class ClipboardCopyNameAsDialog : Form {
        // UIの実装
        private Impl m_impl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting    設定の初期値
        // 　　　　[in]directory  サンプル用のディレクトリ
        // 　　　　[in]fileList   サンプル用のファイル名主部の一覧
        // 戻り値：なし
        //=========================================================================================
        public ClipboardCopyNameAsDialog(ClipboardCopyNameAsSetting setting, string directory, List<string> fileList) {
            InitializeComponent();
            m_impl = new Impl(directory, fileList, this.textBoxSample,
                              this.radioButtonSeparatorSpace, this.radioButtonSeparatorTab, this.radioButtonSeparatorComma, this.radioButtonSeparatorReturn,
                              this.radioButtonQuoteAlways, this.radioButtonQuoteSpace, this.radioButtonQuoteNone, this.checkBoxFullPath);
            m_impl.Initialize(setting);
        }

        //=========================================================================================
        // 機　能：ラジオボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButton_CheckedChanged(object sender, EventArgs evt) {
            m_impl.OnUiChanged();
        }

        //=========================================================================================
        // 機　能：チェックボタンの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void CheckBox_CheckedChanged(object sender, EventArgs evt) {
            m_impl.OnUiChanged();
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
        // プロパティ：クリップボードのテキスト
        //=========================================================================================
        public string ClipboardText {
            get {
                return m_impl.ClipboardText;
            }
        }

        //=========================================================================================
        // プロパティ：設定済みの値（コンストラクタとは別インスタンス）
        //=========================================================================================
        public ClipboardCopyNameAsSetting Setting {
            get {
                return m_impl.Setting;
            }
        }

        //=========================================================================================
        // クラス：形式を指定してファイル名をコピーのダイアログ
        //=========================================================================================
        public class Impl {
            // 設定済みの値（コンストラクタとは別インスタンス）
            private ClipboardCopyNameAsSetting m_setting;

            // サンプル用のディレクトリ
            private string m_directory;

            // サンプル用のファイル名主部の一覧
            private List<string> m_fileList;

            // テキストボックス：サンプル
            private TextBox m_textBoxSample;

            // ラジオボタン：セパレータ 空白
            private RadioButton m_radioButtonSeparatorSpace;
            
            // ラジオボタン：セパレータ タブ
            private RadioButton m_radioButtonSeparatorTab;
            
            // ラジオボタン：セパレータ カンマ
            private RadioButton m_radioButtonSeparatorComma;
            
            // ラジオボタン：セパレータ 改行
            private RadioButton m_radioButtonSeparatorReturn;
            
            // ラジオボタン：引用符 常に付ける
            private RadioButton m_radioButtonQuoteAlways;
            
            // ラジオボタン：引用符 空白を含む場合だけ
            private RadioButton m_radioButtonQuoteSpace;
            
            // ラジオボタン：引用符 なし
            private RadioButton m_radioButtonQuoteNone;
            
            // チェックボックス：フォルダ名を付加
            private CheckBox m_checkBoxFullPath;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]directory        サンプル用のディレクトリ
            // 　　　　[in]fileList         サンプル用のファイル名主部の一覧
            // 　　　　[in]textSample       テキストボックス：サンプル
            // 　　　　[in]radioSepSpace    ラジオボタン：セパレータ 空白
            // 　　　　[in]radioSepTab      ラジオボタン：セパレータ タブ
            // 　　　　[in]radioSepaComma   ラジオボタン：セパレータ カンマ
            // 　　　　[in]radioSepReturn   ラジオボタン：セパレータ 改行
            // 　　　　[in]radioQuoteAlways ラジオボタン：引用符 常に付ける
            // 　　　　[in]radioQuoteSpace  ラジオボタン：引用符 空白を含む場合だけ
            // 　　　　[in]radioQuoteNone   ラジオボタン：引用符 なし
            // 　　　　[in]checkFullPath    チェックボックス：フォルダ名を付加
            // 戻り値：なし
            //=========================================================================================
            public Impl(string directory, List<string> fileList, TextBox textSample,
                        RadioButton radioSepSpace, RadioButton radioSepTab, RadioButton radioSepaComma, RadioButton radioSepReturn,
                        RadioButton radioQuoteAlways, RadioButton radioQuoteSpace, RadioButton radioQuoteNone, CheckBox checkFullPath) {
                m_directory = directory;
                m_fileList = fileList;
                m_textBoxSample = textSample;
                m_radioButtonSeparatorSpace = radioSepSpace;
                m_radioButtonSeparatorTab = radioSepTab;
                m_radioButtonSeparatorComma = radioSepaComma;
                m_radioButtonSeparatorReturn = radioSepReturn;
                m_radioButtonQuoteAlways = radioQuoteAlways;
                m_radioButtonQuoteSpace = radioQuoteSpace;
                m_radioButtonQuoteNone = radioQuoteNone;
                m_checkBoxFullPath = checkFullPath;
            }

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]setting   設定の初期値
            // 戻り値：なし
            //=========================================================================================
            public void Initialize(ClipboardCopyNameAsSetting setting) {
                switch (setting.Separator) {
                    case ClipboardCopyNameAsSetting.SeparatorMode.SeparatorSpace:
                        m_radioButtonSeparatorSpace.Checked = true;
                        break;
                    case ClipboardCopyNameAsSetting.SeparatorMode.SeparatorTab:
                        m_radioButtonSeparatorTab.Checked = true;
                        break;
                    case ClipboardCopyNameAsSetting.SeparatorMode.SeparatorComma:
                        m_radioButtonSeparatorComma.Checked = true;
                        break;
                    case ClipboardCopyNameAsSetting.SeparatorMode.SeparatorReturn:
                        m_radioButtonSeparatorReturn.Checked = true;
                        break;
                }
                switch (setting.Quote) {
                    case ClipboardCopyNameAsSetting.QuoteMode.QuoteAlways:
                        m_radioButtonQuoteAlways.Checked = true;
                        break;
                    case ClipboardCopyNameAsSetting.QuoteMode.QuoteSpace:
                        m_radioButtonQuoteSpace.Checked = true;
                        break;
                    case ClipboardCopyNameAsSetting.QuoteMode.QuoteNone:
                        m_radioButtonQuoteNone.Checked = true;
                        break;
                }
                m_checkBoxFullPath.Checked = setting.FullPath;

                UpdateSetting();
                m_textBoxSample.Text = CreateSample(m_setting, m_directory, m_fileList);
            }

            //=========================================================================================
            // 機　能：UIの項目が変更されたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnUiChanged() {
                UpdateSetting();
                m_textBoxSample.Text = CreateSample(m_setting, m_directory, m_fileList);
            }

            //=========================================================================================
            // 機　能：ダイアログの入力値から設定項目を更新する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void UpdateSetting() {
                ClipboardCopyNameAsSetting.SeparatorMode separator;
                if (m_radioButtonSeparatorSpace.Checked) {
                    separator = ClipboardCopyNameAsSetting.SeparatorMode.SeparatorSpace;
                } else if (m_radioButtonSeparatorTab.Checked) {
                    separator = ClipboardCopyNameAsSetting.SeparatorMode.SeparatorTab;
                } else if (m_radioButtonSeparatorComma.Checked) {
                    separator = ClipboardCopyNameAsSetting.SeparatorMode.SeparatorComma;
                } else {
                    separator = ClipboardCopyNameAsSetting.SeparatorMode.SeparatorReturn;
                }
                ClipboardCopyNameAsSetting.QuoteMode quoteMode;
                if (m_radioButtonQuoteAlways.Checked) {
                    quoteMode = ClipboardCopyNameAsSetting.QuoteMode.QuoteAlways;
                } else if (m_radioButtonQuoteSpace.Checked) {
                    quoteMode = ClipboardCopyNameAsSetting.QuoteMode.QuoteSpace;
                } else {
                    quoteMode = ClipboardCopyNameAsSetting.QuoteMode.QuoteNone;
                }
                bool fullPath = m_checkBoxFullPath.Checked;
                m_setting = new ClipboardCopyNameAsSetting();
                m_setting.Separator = separator;
                m_setting.Quote = quoteMode;
                m_setting.FullPath = fullPath;
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnOk() {
                UpdateSetting();
            }

            //=========================================================================================
            // 機　能：ファイル一覧からクリップボードの文字列を作成する
            // 引　数：[in]setting    クリップボード形式の設定
            // 　　　　[in]directory  ファイル一覧のディレクトリ
            // 　　　　[in]fileList   ファイル一覧
            // 戻り値：クリップボードの文字列
            //=========================================================================================
            private string CreateSample(ClipboardCopyNameAsSetting setting, string directory,  List<string> fileList) {
                StringBuilder sb = new StringBuilder();
                foreach (string file in fileList) {
                    if (sb.Length > 0) {
                        switch (setting.Separator) {
                            case ClipboardCopyNameAsSetting.SeparatorMode.SeparatorSpace:
                                sb.Append(" ");
                                break;
                            case ClipboardCopyNameAsSetting.SeparatorMode.SeparatorTab:
                                sb.Append("\t");
                                break;
                            case ClipboardCopyNameAsSetting.SeparatorMode.SeparatorComma:
                                sb.Append(",");
                                break;
                            case ClipboardCopyNameAsSetting.SeparatorMode.SeparatorReturn:
                                sb.Append("\r\n");
                                break;
                        }
                    }

                    string path;
                    if (setting.FullPath) {
                        path = directory + file;
                    } else {
                        path = file;
                    }

                    bool quote = false;
                    switch (setting.Quote) {
                        case ClipboardCopyNameAsSetting.QuoteMode.QuoteAlways:
                            quote = true;
                            break;
                        case ClipboardCopyNameAsSetting.QuoteMode.QuoteSpace:
                            if (path.IndexOf(' ') != -1) {
                                quote = true;
                            } else {
                                quote = false;
                            }
                            break;
                        case ClipboardCopyNameAsSetting.QuoteMode.QuoteNone:
                            quote = false;
                            break;
                    }

                    if (quote) {
                        sb.Append('\"');
                        sb.Append(path);
                        sb.Append('\"');
                    } else {
                        sb.Append(path);
                    }
                }
                return sb.ToString();
            }

            //=========================================================================================
            // プロパティ：クリップボードのテキスト
            //=========================================================================================
            public string ClipboardText {
                get {
                    return CreateSample(m_setting, m_directory, m_fileList);
                }
            }

            //=========================================================================================
            // プロパティ：設定済みの値（コンストラクタとは別インスタンス）
            //=========================================================================================
            public ClipboardCopyNameAsSetting Setting {
                get {
                    return m_setting;
                }
            }
        }
    }
}
