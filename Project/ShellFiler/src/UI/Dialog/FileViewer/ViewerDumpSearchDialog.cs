using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Locale;
using ShellFiler.UI.Dialog.Option;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.FileViewer {

    //=========================================================================================
    // クラス：ダンプビューアでの検索ダイアログ
    //=========================================================================================
    public partial class ViewerDumpSearchDialog : Form {
        // ダイアログの初期表示位置のマージン（右下からの差分）
        private const int CX_DIALOG_POS_MARGIN = 24;

        // ダイアログの初期表示位置のマージン（右下からの差分）
        private const int CY_DIALOG_POS_MARGIN = 8;

        // ダンプ領域とテキスト領域の間のマージン
        public const int CX_DUMP_TEXT_MARGIN = 25;

        // ダンプで1行に表示するバイト数
        private const int DUMP_LINE_BYTES = 8;

        // ファイルビューア
        private TextFileViewer m_textFileViewer;

        // 検索条件
        private DumpSearchCondition m_searchCondition;

        // ポップアップメニュー項目 検索履歴
        private ToolStripItem[] m_inputHelpMenuItemHistory;

        // ポップアップメニュー項目 入力例
        private ToolStripItem[] m_inputHelpMenuItemExample;

        // サンプル表示域のテキストの開始位置
        private float m_xPosText;

        // サンプル表示域の行の高さ
        private int m_cyLineHeight;

        // 初期化が完了したときtrue
        private bool m_initialized;
        
        // テキストの入力領域を更新中のときtrue
        private bool m_onUpdateInputText = false;

        // ダンプの入力領域を更新中のときtrue
        private bool m_onUpdateInputDump = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileViewer ファイルビューア
        // 　　　　[in]condition  現在の検索条件（検索していないときはnull）
        // 戻り値：なし
        //=========================================================================================
        public ViewerDumpSearchDialog(TextFileViewer fileViewer, DumpSearchCondition condition) {
            InitializeComponent();
            m_searchCondition = condition;
            m_textFileViewer = fileViewer;

            this.Location = m_textFileViewer.PointToScreen(new Point(m_textFileViewer.Right - this.Width - CX_DIALOG_POS_MARGIN, m_textFileViewer.Bottom - this.Height - CY_DIALOG_POS_MARGIN));

            if (m_searchCondition == null) {
                m_searchCondition = new DumpSearchCondition();
            }
            if (m_searchCondition.SearchBytes == null) {
                List<string> history = Program.Document.UserSetting.ViewerSearchHistory.DumpSearchHistory;
                if (history.Count > 0) {
                    m_searchCondition.SearchBytes = DumpUtils.StringToBytes(history[history.Count - 1]);
                } else {
                    m_searchCondition.SearchBytes = new byte[0];
                }
            }

            // UIを初期化
            this.textBoxInput.Text = DumpUtils.BytesToString(m_searchCondition.SearchBytes);

            // 検索履歴
            Program.Document.UserSetting.ViewerSearchHistory.LoadData();
            List<string> historyList = new List<string>();
            historyList.AddRange(Program.Document.UserSetting.ViewerSearchHistory.DumpSearchHistory);
            historyList.Reverse();
            m_inputHelpMenuItemHistory = new ToolStripItem[historyList.Count];
            for (int i = 0; i < historyList.Count; i++) {
                string history = historyList[i];
                m_inputHelpMenuItemHistory[i] = new ToolStripMenuItem(history, null, new EventHandler(this.PopupMenuItemHistory_Click), history);
            }

            // 入力支援のメニュー項目
            m_inputHelpMenuItemExample = new ToolStripItem[] {
                new ToolStripMenuItem(Resources.DlgViewerDSearch_IHelp_Crlf,    null, new EventHandler(this.PopupMenuItemExample_Click), @"0d,0a"),
                new ToolStripMenuItem(Resources.DlgViewerDSearch_IHelp_Utf8Bom, null, new EventHandler(this.PopupMenuItemExample_Click), @"ef,bb,bf"),
                new ToolStripMenuItem(Resources.DlgViewerDSearch_IHelp_UniLBom, null, new EventHandler(this.PopupMenuItemExample_Click), @"ff,fe"),
                new ToolStripMenuItem(Resources.DlgViewerDSearch_IHelp_UniBBom, null, new EventHandler(this.PopupMenuItemExample_Click), @"fe,ff"),
                new ToolStripMenuItem(Resources.DlgViewerDSearch_IHelp_RepUtf8, null, new EventHandler(this.PopupMenuItemExample_Click), @"ef,bf,bd"),
                new ToolStripMenuItem(Resources.DlgViewerDSearch_IHelp_RepUni,  null, new EventHandler(this.PopupMenuItemExample_Click), @"ff,fd"),
            };

            // ダンプのサンプル表示
            TextViewerGraphics g = new TextViewerGraphics(this, 0);
            try {
                byte[] testBuffer = new byte[DUMP_LINE_BYTES];
                string testDump;
                List<int> dumpPosition;
                DumpHexFormatter formatter = new DumpHexFormatter();
                formatter.CreateDumpHexStr(testBuffer, 0, testBuffer.Length, DUMP_LINE_BYTES, out testDump, out dumpPosition);
                m_cyLineHeight = g.TextFont.Height;
                m_xPosText = GraphicsUtils.MeasureString(g.Graphics, g.TextFont, testDump) + CX_DUMP_TEXT_MARGIN;

                this.panelScroll.Location = new Point(0, 0);
                this.panelScroll.Size = new Size(this.panelParent.ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth - 1, this.panelParent.ClientRectangle.Height);
            } finally {
                g.Dispose();
            }

            m_initialized = true;
            if (m_searchCondition.SearchBytes.Length > 0) {
                textBoxInput_TextChanged(this, null);
            }
        }
        
        //=========================================================================================
        // 機　能：ダイアログが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ViewerDumpSearchDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            m_textFileViewer.OnCloseViewerSearchDialog(false);
        }

        //=========================================================================================
        // 機　能：「>>」ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonInputHelp_Click(object sender, EventArgs evt) {
            ToolStripMenuItem item1 = new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_History, null, null, @"History");
            ToolStripMenuItem item2 = new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_DelHist, null, new EventHandler(this.PopupMenuItem_Click), @"DelHist");
            ToolStripMenuItem item3 = new ToolStripMenuItem(Resources.DlgViewerSearch_IHelp_RegExp,  null, null, @"Example");

            item1.DropDownItems.AddRange(m_inputHelpMenuItemHistory);
            item3.DropDownItems.AddRange(m_inputHelpMenuItemExample);
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add(item1);
            if (m_inputHelpMenuItemHistory.Length == 0) {
                item1.Enabled = false;
            }
            cms.Items.Add(item2);
            cms.Items.Add(item3);
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
        // 機　能：「>>」ボタンのポップアップメニュー項目（履歴削除）がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void PopupMenuItem_Click(object sender, EventArgs evt) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)(sender);
            string itemData = menuItem.Name;
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
        // 機　能：「>>」ボタンのポップアップメニュー項目（入力例）がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void PopupMenuItemExample_Click(object sender, EventArgs evt) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)(sender);
            string itemData = menuItem.Name;
            this.textBoxInput.Text = itemData;
        }
 
        //=========================================================================================
        // 機　能：コンボボックスでテキストが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxInput_TextChanged(object sender, EventArgs evt) {
            m_onUpdateInputDump = true;
            if (m_initialized) {
                GetSearchCondition();
                TextViewerGraphics g = new TextViewerGraphics(this, 0);
                try {
                    int cy = (m_searchCondition.SearchBytes.Length / DUMP_LINE_BYTES + 1) * g.TextFont.Height;
                    this.panelScroll.Size = new Size(this.panelScroll.Width, Math.Max(cy, this.panelParent.ClientRectangle.Height));
                } finally {
                    g.Dispose();
                }
                if (!m_onUpdateInputText) {
                    this.textBoxText.Text = "";
                }
                panelScroll.Invalidate();
                DumpViewerComponent viewer = (DumpViewerComponent)(m_textFileViewer.TextViewerComponent);
                viewer.SearchBytes(m_searchCondition, true, -1);
            }
            m_onUpdateInputDump = false;
        }
        
        //=========================================================================================
        // 機　能：文字列指定でのテキストが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxText_TextChanged(object sender, EventArgs evt) {
            m_onUpdateInputText = true;
            if (!m_onUpdateInputDump) {
                Encoding encoding = m_textFileViewer.TextBufferLineInfo.TextEncodingType.Encoding;
                byte[] dump = encoding.GetBytes(this.textBoxText.Text);
                this.textBoxInput.Text = DumpUtils.BytesToString(dump);
            }
            m_onUpdateInputText = false;
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
                Program.Document.UserSetting.ViewerSearchHistory.AddDumpSearchWord(this.textBoxInput.Text);
                Program.Document.UserSetting.ViewerSearchHistory.SaveData();
            }
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
            DumpViewerComponent viewer = (DumpViewerComponent)(m_textFileViewer.TextViewerComponent);
            if (evt.KeyCode == Keys.Up) {
                viewer.SearchBytes(null, false, -1);
                evt.Handled = true;
            } else if (evt.KeyCode == Keys.Down) {
                viewer.SearchBytes(null, true, -1);
                evt.Handled = true;
            } else if (evt.KeyCode == Keys.Right) {
                if (sender == this.textBoxInput) {
                    int inputSelectStart = this.textBoxInput.SelectionStart;
                    int inputSelectLength = this.textBoxInput.SelectionLength;
                    if (inputSelectLength == 0 && inputSelectStart == textBoxInput.Text.Length) {
                        buttonInputHelp_Click(null, null);
                    }
                } else {
                    int inputSelectStart = this.textBoxText.SelectionStart;
                    int inputSelectLength = this.textBoxText.SelectionLength;
                    if (inputSelectLength == 0 && inputSelectStart == textBoxText.Text.Length) {
                        buttonInputHelp_Click(null, null);
                    }
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
            bool trimmed;
            m_searchCondition.SearchBytes = DumpSearchCondition.TrimBySearchLength(DumpUtils.StringToBytes(this.textBoxInput.Text), out trimmed);
            if (trimmed) {
                m_textFileViewer.ShowStatusbarMessage(Resources.DlgViewerDSearch_KeywordTrimmed, FileOperationStatus.LogLevel.Info, IconImageListID.FileViewer_SearchGeneric);
            }
            m_searchCondition.AutoSearchBytes = null;
        }
        
        //=========================================================================================
        // 機　能：サンプル領域の描画処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void panelScroll_Paint(object sender, PaintEventArgs evt) {
            TextViewerGraphics g = new TextViewerGraphics(evt.Graphics, 0);
            try {
                int line = m_searchCondition.SearchBytes.Length / DUMP_LINE_BYTES + 1;
                EncodingType encoding = m_textFileViewer.TextBufferLineInfo.TextEncodingType;
                string strDump;
                List<int> dumpPos;
                string strText;
                List<int> charToByte, byteToChar;
                DumpHexFormatter dumpFormat = new DumpHexFormatter();
                DumpTextFormatter textForamat = new DumpTextFormatter();
                for (int i = 0; i < line; i++) {
                    int start = i * DUMP_LINE_BYTES;
                    int length = Math.Min(DUMP_LINE_BYTES, m_searchCondition.SearchBytes.Length - start);
                    dumpFormat.CreateDumpHexStr(m_searchCondition.SearchBytes, start, length, DUMP_LINE_BYTES, out strDump, out dumpPos);
                    textForamat.Convert(encoding, m_searchCondition.SearchBytes, start, length, out strText, out charToByte, out byteToChar);

                    g.Graphics.DrawString(strDump, g.TextFont, SystemBrushes.WindowText, new PointF(0, i * m_cyLineHeight));
                    g.Graphics.DrawString(strText, g.TextFont, SystemBrushes.WindowText, new PointF(m_xPosText, i * m_cyLineHeight));
                }
                float xLine = m_xPosText - CX_DIALOG_POS_MARGIN / 2;
                g.Graphics.DrawLine(SystemPens.WindowText, new PointF(xLine, 0), new PointF(xLine, this.panelScroll.Height)); 
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // プロパティ：検索条件（自動検索は必ずクリア）
        //=========================================================================================
        public DumpSearchCondition SearchCondition {
            get {
                return m_searchCondition;
            }
        }
    }
}
