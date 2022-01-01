using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI.Dialog.Option;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：ファイルビューアでの検索ダイアログ
    //=========================================================================================
    public partial class ViewerTextSearchDialog : Form {
        // オプションのコンボボックス 正規表現の項目のインデックス
        private const int OPTION_INDEX_REG_EXP = 4;

        // ダイアログの初期表示位置のマージン（右下からの差分）
        private const int CX_DIALOG_POS_MARGIN = 24;

        // ダイアログの初期表示位置のマージン（右下からの差分）
        private const int CY_DIALOG_POS_MARGIN = 8;

        // ファイルビューア
        private TextFileViewer m_textFileViewer;

        // 検索条件
        private TextSearchCondition m_searchCondition;

        // ポップアップメニュー項目 検索履歴
        private ToolStripItem[] m_inputHelpMenuItemHistory;

        // ポップアップメニュー項目 正規表現入力支援
        private ToolStripItem[] m_inputHelpMenuItemRegExp;

        // ポップアップメニュー項目 正規表現入力例
        private ToolStripItem[] m_inputHelpMenuItemExample;

        // 初期化が完了したときtrue
        private bool m_initialized;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileViewer ファイルビューア
        // 　　　　[in]condition  現在の検索条件（検索していないときはnull）
        // 戻り値：なし
        //=========================================================================================
        public ViewerTextSearchDialog(TextFileViewer fileViewer, TextSearchCondition condition) {
            InitializeComponent();
            m_searchCondition = condition;
            m_textFileViewer = fileViewer;

            this.SuspendLayout();

            this.Location = m_textFileViewer.PointToScreen(new Point(m_textFileViewer.Right - this.Width - CX_DIALOG_POS_MARGIN, m_textFileViewer.Bottom - this.Height - CY_DIALOG_POS_MARGIN));

            if (m_searchCondition == null) {
                m_searchCondition = new TextSearchCondition();
            }
            if (m_searchCondition.SearchString == null) {
                TextSearchOption searchOption;
                if (Configuration.Current.TextViewerSearchOptionDefault == null) {
                    searchOption = (TextSearchOption)(Program.Document.UserGeneralSetting.TextViewerSearchOption.Clone());
                } else {
                    searchOption = (TextSearchOption)(Configuration.Current.TextViewerSearchOptionDefault.Clone());
                }
                List<string> history = Program.Document.UserSetting.ViewerSearchHistory.TextSearchHistory;
                if (history.Count > 0) {
                    m_searchCondition.SearchString = history[history.Count - 1];
                } else {
                    m_searchCondition.SearchString = "";
                }
                m_searchCondition.SearchMode = searchOption.SearchMode;
                m_searchCondition.SearchWord = searchOption.SearchWord;
            }

            // UIを初期化
            this.textBoxInput.Text = m_searchCondition.SearchString;
            string[] optionItemList = {
                Resources.DlgViewerSearch_Option1,
                Resources.DlgViewerSearch_Option2,
                Resources.DlgViewerSearch_Option3,
                Resources.DlgViewerSearch_Option4,
                Resources.DlgViewerSearch_Option5,      // 正規表現:index=4→OPTION_INDEX_REG_EXP
            };
            this.comboBoxOption.Items.AddRange(optionItemList);
            switch (m_searchCondition.SearchMode) {
                case TextSearchMode.NormalIgnoreCase:
                    this.comboBoxOption.SelectedIndex = 0;
                    break;
                case TextSearchMode.NormalCaseSensitive:
                    this.comboBoxOption.SelectedIndex = 1;
                    break;
                case TextSearchMode.WildCardIgnoreCase:
                    this.comboBoxOption.SelectedIndex = 2;
                    break;
                case TextSearchMode.WildCardCaseSensitive:
                    this.comboBoxOption.SelectedIndex = 3;
                    break;
                case TextSearchMode.RegularExpression:
                    this.comboBoxOption.SelectedIndex = 4;
                    break;
            }
            this.checkBoxWord.Checked = m_searchCondition.SearchWord;

            // 検索履歴
            Program.Document.UserSetting.ViewerSearchHistory.LoadData();
            List<string> historyItemList = new List<string>();
            historyItemList.AddRange(Program.Document.UserSetting.ViewerSearchHistory.TextSearchHistory);
            historyItemList.Reverse();
            m_inputHelpMenuItemHistory = new ToolStripItem[historyItemList.Count];
            for (int i = 0; i < historyItemList.Count; i++) {
                string history = historyItemList[i];
                m_inputHelpMenuItemHistory[i] = new ToolStripMenuItem(history, null, new EventHandler(this.PopupMenuItemHistory_Click), history);
            }

            // 入力支援のメニュー項目
            m_inputHelpMenuItemRegExp = new ToolStripItem[] {
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_AnyChar, null, new EventHandler(this.PopupMenuItemRegExp_Click), @"."),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Space,   null, new EventHandler(this.PopupMenuItemRegExp_Click), @"\s"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Decimal, null, new EventHandler(this.PopupMenuItemRegExp_Click), @"\dl"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Meta,    null, new EventHandler(this.PopupMenuItemRegExp_Click), @"\"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Any,     null, new EventHandler(this.PopupMenuItemRegExp_Click), @"[]"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_NotAny,  null, new EventHandler(this.PopupMenuItemRegExp_Click), @"[^]"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Range,   null, new EventHandler(this.PopupMenuItemRegExp_Click), @"[-]"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Start,   null, new EventHandler(this.PopupMenuItemRegExp_Click), @"^"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_End,     null, new EventHandler(this.PopupMenuItemRegExp_Click), @"$"),
                new ToolStripSeparator(),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_LowChar, null, new EventHandler(this.PopupMenuItemRegExp_Click), @"[a-z]+"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_UpChar,  null, new EventHandler(this.PopupMenuItemRegExp_Click), @"[A-Z]+"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_EngChar, null, new EventHandler(this.PopupMenuItemRegExp_Click), @"[a-zA-Z]+"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_EngNum,  null, new EventHandler(this.PopupMenuItemRegExp_Click), @"[a-zA-Z0-9]+"),
            };
            m_inputHelpMenuItemExample = new ToolStripItem[] {
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Mail,    null, new EventHandler(this.PopupMenuItemExample_Click), @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Url,     null, new EventHandler(this.PopupMenuItemExample_Click), @"s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Tel,     null, new EventHandler(this.PopupMenuItemExample_Click), @"0\d{1,4}-\d{1,4}-\d{4}"),
                new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Zip,     null, new EventHandler(this.PopupMenuItemExample_Click), @"\d\d\d-\d\d\d\d"),
            };  
      
            m_initialized = true;

            this.ResumeLayout(false);

            if (m_searchCondition.SearchString.Length > 0) {
                textBoxInput_TextChanged(this, null);
            }
        }

        //=========================================================================================
        // 機　能：ダイアログが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ViewerTextSearchDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            m_textFileViewer.OnCloseViewerSearchDialog(true);
        }

        //=========================================================================================
        // 機　能：「>>」ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonInputHelp_Click(object sender, EventArgs evt) {
            ToolStripMenuItem item1 = new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_History, null, null, @"History");
            ToolStripMenuItem item2 = new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_DelHist, null, new EventHandler(this.PopupMenuItemDelHist_Click), @"DelHist");
            ToolStripMenuItem item3 = new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_Example, null, null, @"InputHelp");
            ToolStripMenuItem item4 = new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_RegExp,  null, null, @"Example");

            item1.DropDownItems.AddRange(m_inputHelpMenuItemHistory);
            item3.DropDownItems.AddRange(m_inputHelpMenuItemRegExp);
            item4.DropDownItems.AddRange(m_inputHelpMenuItemExample);
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add(item1);
            if (m_inputHelpMenuItemHistory.Length == 0) {
                item1.Enabled = false;
            }
            cms.Items.Add(item3);
            cms.Items.Add(item4);
            item1.Select();
            this.ContextMenuStrip = cms;
            this.ContextMenuStrip.Show(this, new Point(this.buttonInputHelp.Location.X + this.buttonInputHelp.Size.Width, this.buttonInputHelp.Location.Y));
        }

        //=========================================================================================
        // 機　能：「>>」ボタンのポップアップメニュー項目（入力履歴）がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void PopupMenuItemHistory_Click(object sender, EventArgs evt) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)(sender);
            string itemData = menuItem.Name;
            this.textBoxInput.Text = itemData;
        }

        //=========================================================================================
        // 機　能：「>>」ボタンのポップアップメニュー項目（入力履歴の削除）がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void PopupMenuItemDelHist_Click(object sender, EventArgs evt) {
            DialogResult result = DeleteHistoryProcedure.ConfirmDeleteHistory(this, Resources.Option_PrivacyViewerConfirm);
            if (result != DialogResult.Yes) {
                return;
            }
            DeleteHistoryProcedure.DeleteViewerHistory();
            InfoBox.Information(this, Resources.Option_PrivacyViewerCompleted);

            // メニューの削除
            for (int i = 0; i < m_inputHelpMenuItemHistory.Length; i++) {
                m_inputHelpMenuItemHistory[i].Dispose();
            }
            m_inputHelpMenuItemHistory = new ToolStripItem[0];
        }

        //=========================================================================================
        // 機　能：「>>」ボタンのポップアップメニュー項目（正規表現の入力支援）がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void PopupMenuItemRegExp_Click(object sender, EventArgs evt) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)(sender);
            string itemData = menuItem.Name;
            string text = this.textBoxInput.Text;
            int inputSelectStart = this.textBoxInput.SelectionStart;
            int inputSelectLength = this.textBoxInput.SelectionLength;

            if (inputSelectStart > text.Length && inputSelectStart + inputSelectLength > text.Length) {
                inputSelectStart = 0;
                inputSelectLength = 0;
            }
            this.textBoxInput.Text = text.Substring(0, inputSelectStart) + itemData + text.Substring(inputSelectStart + inputSelectLength);
            inputSelectStart = inputSelectStart + itemData.Length;
            inputSelectLength = 0;
            this.comboBoxOption.SelectedIndex = OPTION_INDEX_REG_EXP;
        }
        
        //=========================================================================================
        // 機　能：「>>」ボタンのポップアップメニュー項目（正規表現の入力例）がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void PopupMenuItemExample_Click(object sender, EventArgs evt) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)(sender);
            string itemData = menuItem.Name;
            this.textBoxInput.Text = itemData;
            this.comboBoxOption.SelectedIndex = OPTION_INDEX_REG_EXP;
        }

        //=========================================================================================
        // 機　能：テキストボックスでテキストが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxInput_TextChanged(object sender, EventArgs evt) {
            if (m_initialized) {
                GetSearchCondition();
                TextViewerComponent viewer = (TextViewerComponent)(m_textFileViewer.TextViewerComponent);
                viewer.SearchText(m_searchCondition, true, -1);
            }
        }
        
        //=========================================================================================
        // 機　能：単語単位で検索の設定が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxWord_CheckStateChanged(object sender, EventArgs e) {
            textBoxInput_TextChanged(null, null);
        }

        //=========================================================================================
        // 機　能：閉じるボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClose_Click(object sender, EventArgs evt) {
            GetSearchCondition();
            if (this.textBoxInput.Text != "") {
                Program.Document.UserSetting.ViewerSearchHistory.AddTextSearchWord(this.textBoxInput.Text);
                Program.Document.UserSetting.ViewerSearchHistory.SaveData();
            }

            TextSearchOption searchOption = new TextSearchOption();
            searchOption.SearchMode = m_searchCondition.SearchMode;
            searchOption.SearchWord = m_searchCondition.SearchWord;
            Program.Document.UserGeneralSetting.TextViewerSearchOption = searchOption;
            
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：キーが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxInput_KeyDown(object sender, KeyEventArgs evt) {
            TextViewerComponent viewer = (TextViewerComponent)(m_textFileViewer.TextViewerComponent);
            if (evt.KeyCode == Keys.Up) {
                viewer.SearchText(null, false, -1);
                evt.Handled = true;
            } else if (evt.KeyCode == Keys.Down) {
                viewer.SearchText(null, true, -1);
                evt.Handled = true;
            } else if (evt.KeyCode == Keys.Right) {
                int inputSelectStart = this.textBoxInput.SelectionStart;
                int inputSelectLength = this.textBoxInput.SelectionLength;
                if (inputSelectLength == 0 && inputSelectStart == textBoxInput.Text.Length) {
                    buttonInputHelp_Click(null, null);
                }
            } else if (evt.KeyCode == Keys.F1) {
                viewer.MoveDisplayPosition(int.MinValue);
                evt.Handled = true;
            } else if (evt.KeyCode == Keys.F2) {
                viewer.MoveDisplayPosition(int.MaxValue);
                evt.Handled = true;
            }
        }

        //=========================================================================================
        // 機　能：検索条件をUIから取得してメンバにセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void GetSearchCondition() {
            if (this.textBoxInput.Text.Length == 0) {
                m_searchCondition.SearchString = null;
            } else {
                bool trimmed;
                m_searchCondition.SearchString = TextSearchCondition.TrimBySearchLength(this.textBoxInput.Text, out trimmed);
                if (trimmed) {
                    m_textFileViewer.ShowStatusbarMessage(Resources.DlgViewerSearch_KeywordTrimmed, FileOperationStatus.LogLevel.Info, IconImageListID.FileViewer_SearchGeneric);
                }
            }
            switch (this.comboBoxOption.SelectedIndex) {
                case 0:
                    m_searchCondition.SearchMode = TextSearchMode.NormalIgnoreCase;
                    break;
                case 1:
                    m_searchCondition.SearchMode = TextSearchMode.NormalCaseSensitive;
                    break;
                case 2:
                    m_searchCondition.SearchMode = TextSearchMode.WildCardIgnoreCase;
                    break;
                case 3:
                    m_searchCondition.SearchMode = TextSearchMode.WildCardCaseSensitive;
                    break;
                case 4:
                    m_searchCondition.SearchMode = TextSearchMode.RegularExpression;
                    try {
                        Regex regex = new Regex(m_searchCondition.SearchString);
                    } catch (Exception) {
                        m_searchCondition.SearchString = null;
                    }
                    break;
            }
            m_searchCondition.SearchWord = this.checkBoxWord.Checked;
            m_searchCondition.AutoSearchString = null;
        }

        //=========================================================================================
        // プロパティ：検索条件（自動検索は必ずクリア）
        //=========================================================================================
        public TextSearchCondition SearchCondition {
            get {
                return m_searchCondition;
            }
        }
    }
}
