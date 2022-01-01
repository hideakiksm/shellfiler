using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.UI.Dialog.Option;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：コマンド入力ダイアログ
    //=========================================================================================
    public partial class CommandInputDialog : Form {
        // 標準出力を中継する方法のデフォルト
        public static readonly ShellExecuteRelayMode SHELL_EXECUTE_RELAY_MODE_DEFAULT = ShellExecuteRelayMode.RelayLogWindow;

        // インテリセンスのUI
        private IntelliSensePopup m_intelliSense;

        // ファイル一覧
        private UIFileList m_uiFileList;
        
        // コマンドヒストリ
        private CommandHistory m_commandHistory;

        // 入力されたコマンド
        private string m_command;

        // 標準出力を中継する方法
        private ShellExecuteRelayMode m_relayMode;

        // プログラムでComboboxのテキストを更新中のときtrue
        private bool m_isChangeTextWithProgram = false;

        // インテリセンスを表示させようとしているときtrue
        private bool m_intelliSenseShowProcess = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]commandHistory  コマンドヒストリ
        // 　　　　[in]dictionary      コマンド名辞書
        // 　　　　[in]fileList        対象パスのファイル一覧
        // 　　　　[in]cursor          対象パスのカーソル位置
        // 　　　　[in]relayMode       標準出力の中継モード
        // 戻り値：なし
        //=========================================================================================
        public CommandInputDialog(CommandHistory commandHistory, CommandNameDictionary dictionary, UIFileList fileList, int cursor, ShellExecuteRelayMode relayMode) {
            InitializeComponent();
            m_uiFileList = fileList;
            m_commandHistory = commandHistory;
            m_relayMode = relayMode;

            // ヒストリを登録
            commandHistory.LoadData();
            List<string> commandList = new List<string>();
            for (int i = commandHistory.ItemList.Count - 1; i >= 0; i--) {
                CommandHistoryItem commandItem = commandHistory.ItemList[i];
                commandList.Add(commandItem.CommandString);
            }
            this.comboBoxCommand.Items.AddRange(commandList.ToArray());
            
            // インテリセンスを初期化
            IntelliSensePopup.ItemList itemList = CreateIntelliSenseItemList(dictionary, fileList);
            m_intelliSense = new IntelliSensePopup(this.comboBoxCommand, itemList, fileList);
            m_intelliSense.Owner = this;
            MoveIntelliSensePopup("");
            this.ActiveControl = this.comboBoxCommand;

            // コマンドラインを初期化
            this.comboBoxCommand.Text = CreateComboboxInitialText(fileList, cursor);

            // 標準出力の中継モード
            // 0:ログに出力、1:ファイルビューアに出力、2:中継しない
            string[] resultItems = {
                Resources.OptionFileOprEtc_ExecOutputLog,
                Resources.OptionFileOprEtc_ExecOutputViewer,
                Resources.OptionFileOprEtc_ExecOutputNone,
            };
            this.comboBoxResultMode.Items.AddRange(resultItems);
            if (relayMode == ShellExecuteRelayMode.RelayLogWindow) {
                this.comboBoxResultMode.SelectedIndex = 0;
            } else if (relayMode == ShellExecuteRelayMode.RelayFileViewer) {
                this.comboBoxResultMode.SelectedIndex = 1;
            } else if (relayMode == ShellExecuteRelayMode.None) {
                if (fileList.FileSystem.FileSystemId == FileSystemID.Windows) {
                    this.comboBoxResultMode.SelectedIndex = 2;
                } else {
                    this.comboBoxResultMode.SelectedIndex = 0;
                }
            }
        }

        //=========================================================================================
        // 機　能：インテリセンスの項目一覧を作成する
        // 引　数：[in]dictionary      コマンド名辞書
        // 　　　　[in]fileList        対象パスのファイル一覧
        // 戻り値：なし
        //=========================================================================================
        private IntelliSensePopup.ItemList CreateIntelliSenseItemList(CommandNameDictionary dictionary, UIFileList fileList) {
            IntelliSensePopup.ItemList itemList = new IntelliSensePopup.ItemList();

            // コマンド名辞書を登録
            foreach (CommandNameDictionaryItem item in dictionary.ItemList) {
                itemList.Add(IntelliSensePopup.ItemType.Dictionary, item.CommandString);
                itemList.Add(IntelliSensePopup.ItemType.Dictionary, item.FullPath);
            }

            // ファイル一覧を登録
            string uiDirName = fileList.DisplayDirectoryName;
            foreach (UIFile fileName in fileList.Files) {
                if (fileName.Attribute.IsDirectory) {
                    itemList.Add(IntelliSensePopup.ItemType.TargetPathDir, fileName.FileName);
                    itemList.Add(IntelliSensePopup.ItemType.TargetPathDir, uiDirName + fileName.FileName);
                } else {
                    itemList.Add(IntelliSensePopup.ItemType.TargetPathFile, fileName.FileName);
                    itemList.Add(IntelliSensePopup.ItemType.TargetPathFile, uiDirName + fileName.FileName);
                }
            }

            // ソート
            itemList.Sort();
            return itemList;
        }

        //=========================================================================================
        // 機　能：コマンドラインの初期文字列を作成する
        // 引　数：[in]fileList        対象パスのファイル一覧
        // 　　　　[in]cursor          対象パスのカーソル位置
        // 戻り値：なし
        //=========================================================================================
        private string CreateComboboxInitialText(UIFileList fileList, int cursor) {
            if (fileList.Files.Count == 0) {
                // ファイルなし
                return "";
            } else if (fileList.MarkedFileCount + fileList.MarkedDirectoryCount > 0) {
                // マーク中
                List<UIFile> markList = fileList.MarkFiles;
                StringBuilder command = new StringBuilder();
                bool first = true;
                foreach (UIFile file in markList) {
                    if (first) {
                        first = false;
                    } else {
                        command.Append(' ');
                    }
                    command.Append(file.FileName);
                }
                return command.ToString();
            } else {
                // マークなし
                string command = fileList.DisplayDirectoryName + fileList.Files[cursor].FileName;
                return command;
            }
        }

        //=========================================================================================
        // 機　能：ウィンドウプロシージャ
        // 引　数：[in]message  ウィンドウメッセージ
        // 戻り値：なし
        //=========================================================================================
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message message) {
            if (message.Msg == Win32API.WM_ACTIVATE && message.WParam == (IntPtr)(Win32API.WA_INACTIVE)) {
                // 入力ダイアログがアクティブではなくなったとき、インテリセンスを隠す
                if (!m_intelliSenseShowProcess && m_intelliSense.Visible) {
                    HideIntellisence();
                }
            }
            base.WndProc(ref message);
        }

        //=========================================================================================
        // 機　能：コンボボックスのテキストが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxCommand_TextChanged(object sender, EventArgs evt) {
            // プログラムから書き換えたときはインテリセンスを抑制
            if (m_isChangeTextWithProgram) {
                return;
            }

            // インテリセンスを制御
            string text = this.comboBoxCommand.Text;
            int cursorPos = this.comboBoxCommand.SelectionStart;
            string intelliSenseWord = GetIntelliSenseWord(text, cursorPos);
            if (intelliSenseWord == null && m_intelliSense.Visible) {
                // 表示終了
                HideIntellisence();
            } else if (intelliSenseWord != null && m_intelliSense.Visible) {
                // 検索
                bool available = m_intelliSense.SetSearchWord(intelliSenseWord);
                if (!available) {
                    HideIntellisence();
                }
            }
        }

        //=========================================================================================
        // 機　能：インテリセンスの入力候補となる文字列を取得する
        // 引　数：[in]text       現在の入力文字列全体
        // 　　　　[in]cursorPos  カーソル位置
        // 戻り値：なし
        //=========================================================================================
        private string GetIntelliSenseWord(string text, int cursorPos) {
            if (cursorPos == 0) {
                return null;
            } else if (text[cursorPos - 1] == ' ') {
                return null;
            } else if (text.Length == 0) {
                return null;
            } else {
                int startPos = 0;
                int endPos = text.Length;
                int index = cursorPos - 1;
                while (index >= 0) {
                    if (text[index] == ' ') {
                        startPos = index + 1;
                        break;
                    }
                    index--;
                }
                index = cursorPos - 1;
                while (index >= 0 && index < text.Length) {
                    if (text[index] == ' ') {
                        endPos = index;
                    }
                    index++;
                }
                if (startPos != endPos) {
                    string targetText = text.Substring(startPos, endPos - startPos);
                    return targetText;
                } else {
                    return null;
                }
            }
        }

        //=========================================================================================
        // 機　能：コンボボックスのテキストを変更する
        // 引　数：[in]text    変更するテキスト
        // 戻り値：なし
        //=========================================================================================
        private void SetComboboxText(string text) {
            // イベントを抑制して変更
            m_isChangeTextWithProgram = true;
            this.comboBoxCommand.Text = text;
            m_isChangeTextWithProgram = false;
        }

        //=========================================================================================
        // 機　能：コマンド文字列を置き換える
        // 引　数：[in]keword  カーソル位置に置き換えるキーワード
        // 　　　　[in]isFix   変更内容を完全に固定するきとtrue、選択状態とするときfalse
        // 戻り値：なし
        //=========================================================================================
        private void ReplaceCommand(string keyword, bool isFix) {
            string text = this.comboBoxCommand.Text;
            int cursorPos = this.comboBoxCommand.SelectionStart;
            int selectLength = this.comboBoxCommand.SelectionLength;
            if (selectLength > 0) {
                // 選択中の範囲がある場合
                text = text.Substring(0, cursorPos) + keyword + text.Substring(cursorPos + selectLength);
                SetComboboxText(text);
                if (isFix) {
                    this.comboBoxCommand.SelectionStart = cursorPos + keyword.Length;
                    this.comboBoxCommand.SelectionLength = 0;
                } else {
                    this.comboBoxCommand.SelectionStart = cursorPos;
                    this.comboBoxCommand.SelectionLength = keyword.Length;
                }
            } else {
                // 選択中の範囲がない場合
                // 現在のカーソル位置の前後で置き換え対象の文字列を探す
                int startPos = 0;
                int endPos = text.Length;
                int index = cursorPos - 1;
                while (index >= 0) {
                    if (text[index] == ' ') {
                        startPos = index + 1;
                        break;
                    }
                    index--;
                }
                index = cursorPos - 1;
                while (index >= 0 && index < text.Length) {
                    if (text[index] == ' ') {
                        endPos = index;
                    }
                    index++;
                }
                // テキストを置き換える
                text = text.Substring(0, startPos) + keyword + text.Substring(endPos);
                SetComboboxText(text);
                if (isFix) {
                    this.comboBoxCommand.SelectionStart = startPos + keyword.Length;
                    this.comboBoxCommand.SelectionLength = 0;
                } else {
                    this.comboBoxCommand.SelectionStart = startPos;
                    this.comboBoxCommand.SelectionLength = keyword.Length;
                }
            }
        }

        //=========================================================================================
        // 機　能：ダイアログが移動したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void CommandInputDialog_Move(object sender, EventArgs evt) {
            // インテリセンスも一緒に移動
            MoveIntelliSensePopup("");
        }

        //=========================================================================================
        // 機　能：インテリセンスのポップアップの表示位置を変更する
        // 引　数：[in]searchWord  検索文字列
        // 戻り値：なし
        //=========================================================================================
        private void MoveIntelliSensePopup(string searchWord) {
            int widthWord = 0;
            if (searchWord.Length > 0) {
                Graphics g = this.comboBoxCommand.CreateGraphics();
                try {
                    widthWord = (int)(GraphicsUtils.MeasureString(g, this.comboBoxCommand.Font, searchWord));
                } finally {
                    g.Dispose();
                }
            }
            Point ptCaret;
            Win32API.Win32GetCaretPos(out ptCaret);
            ptCaret = this.comboBoxCommand.PointToScreen(ptCaret);
            Point ptCombo = PointToScreen(new Point(0, this.comboBoxCommand.Bottom));
            m_intelliSense.Left = Math.Max(0, ptCaret.X - widthWord - IntelliSensePopup.CX_ICON_MARGIN);
            m_intelliSense.Top = ptCombo.Y;
        }

        //=========================================================================================
        // 機　能：キーボードが入力されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxCommand_KeyDown(object sender, KeyEventArgs evt) {
            if (m_intelliSense.Visible) {
                if (evt.KeyCode == Keys.Up) {
                    // 上：インテリセンスの上方向検索
                    m_intelliSense.OnComboboxKeyDown(evt);
                    InputCurrentKeywordCursorMove();
                    evt.Handled = true;
                } else if (evt.KeyCode == Keys.Down) {
                    // 下：インテリセンスの下方向検索
                    m_intelliSense.OnComboboxKeyDown(evt);
                    InputCurrentKeywordCursorMove();
                    evt.Handled = true;
                } else if (evt.KeyCode == Keys.Prior) {
                    // RollUp：インテリセンスの上方向移動
                    m_intelliSense.OnComboboxKeyDown(evt);
                    InputCurrentKeywordCursorMove();
                    evt.Handled = true;
                } else if (evt.KeyCode == Keys.Next) {
                    // RollDown：インテリセンスの下方向移動
                    m_intelliSense.OnComboboxKeyDown(evt);
                    InputCurrentKeywordCursorMove();
                    evt.Handled = true;
                } else if (evt.KeyCode == Keys.Escape) {
                    // ESC：表示中のインテリセンスを閉じる
                    HideIntellisence();
                    evt.Handled = true;
                } else if (evt.KeyCode == Keys.Space && evt.Control && evt.Shift) {
                    // Shift+Ctrl+Space:前の候補を検索
                    InputCurrentKeyword(false, IntelliSensePopup.CursorMode.Backword);
                    evt.SuppressKeyPress = true;
                    evt.Handled = true;
                } else if (evt.KeyCode == Keys.Space && evt.Control) {
                    // Ctrl+Space:次の候補を検索
                    InputCurrentKeyword(false, IntelliSensePopup.CursorMode.Forward);
                    evt.SuppressKeyPress = true;
                    evt.Handled = true;
                } else if (evt.KeyCode == Keys.Enter || evt.KeyCode == Keys.Tab) {
                    // Enter:この候補で確定
                    // Tab:この候補で確定
                    InputCurrentKeyword(true, IntelliSensePopup.CursorMode.Stay);
                    evt.Handled = true;
                    evt.SuppressKeyPress = true;
                }
            } else {
                if (evt.KeyCode == Keys.Escape) {
                    // ESC:キャンセル
                    Close();
                    evt.Handled = true;
                } else if (evt.KeyCode == Keys.Enter) {
                    // Enter:確定
                    buttonOk_Click(null, null);
                    evt.Handled = true;
                } else if (evt.KeyCode == Keys.Space && evt.Control) {
                    // Ctrl+Space:インテリセンスを制御
                    string text = this.comboBoxCommand.Text;
                    int cursorPos = this.comboBoxCommand.SelectionStart;
                    string intelliSenseWord = GetIntelliSenseWord(text, cursorPos);
                    if (!m_intelliSense.Visible) {
                        // 表示開始
                        if (intelliSenseWord == null) {
                            intelliSenseWord = "";
                        }
                        int oldSelectionStart = this.comboBoxCommand.SelectionStart;
                        int oldSelectionLength = this.comboBoxCommand.SelectionLength;
                        m_intelliSenseShowProcess = true;
                        MoveIntelliSensePopup(intelliSenseWord);
                        bool available = m_intelliSense.SetSearchWord(intelliSenseWord);
                        if (available) {
                            ShowIntellisence();
                            this.ActiveControl = this.comboBoxCommand;
                            this.Focus();
                            m_intelliSenseShowProcess = false;
                            this.comboBoxCommand.SelectionStart = oldSelectionStart;
                            this.comboBoxCommand.SelectionLength = oldSelectionLength;
                        }
                    }
                    evt.SuppressKeyPress = true;
                    evt.Handled = true;
                }
            }
        }
        
        //=========================================================================================
        // 機　能：インテリセンスを表示状態にする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ShowIntellisence() {
            m_intelliSense.Show();
            this.comboBoxCommand.EnableTab(true);
        }
        
        //=========================================================================================
        // 機　能：インテリセンスを非表示状態にする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void HideIntellisence() {
            m_intelliSense.Hide();
            this.comboBoxCommand.EnableTab(false);
        }

        //=========================================================================================
        // 機　能：キーボードが入力されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxCommand_KeyPress(object sender, KeyPressEventArgs evt) {
            // BEEP音が鳴らないようにする
            if (evt.KeyChar == (char)Keys.Enter || evt.KeyChar == (char)Keys.Escape) {
                evt.Handled = true;
            } else if (m_intelliSense.Visible && evt.KeyChar == (char)Keys.Tab) {
                evt.Handled = true;
            }
        }
        
        //=========================================================================================
        // 機　能：インテリセンスで選択されたキーワードをコンボボックスに入力する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void InputCurrentKeywordCursorMove() {
            string keyword;
            bool existOther;
            m_intelliSense.InputCurrentKeyword(IntelliSensePopup.CursorMode.Stay, true, out keyword, out existOther);
            ReplaceCommand(keyword, false);
        }

        //=========================================================================================
        // 機　能：インテリセンスで選択されたキーワードをコンボボックスに入力する
        // 引　数：[in]fix         固定状態とするときtrue
        // 　　　　[in]cursorMode  入力後のカーソル移動方法
        // 戻り値：なし
        //=========================================================================================
        public void InputCurrentKeyword(bool fix, IntelliSensePopup.CursorMode cursorMode) {
            string keyword;
            bool existOther;
            m_intelliSense.InputCurrentKeyword(cursorMode, false, out keyword, out existOther);
            if (fix) {
                existOther = false;
            }
            if (keyword != null) {
                bool isFixed = !existOther;             // 他の候補がないなら固定
                ReplaceCommand(keyword, isFixed);
            }
            if (!existOther) {                          // 他の候補がないなら閉じる
                HideIntellisence();
                if (fix && this.comboBoxCommand.SelectionLength > 1) {
                    this.comboBoxCommand.SelectionStart = this.comboBoxCommand.SelectionStart + this.comboBoxCommand.SelectionLength;
                    this.comboBoxCommand.SelectionLength = 0;
                }
            }
        }

        //=========================================================================================
        // 機　能：キーボード入力時にキーが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxCommand_KeyUp(object sender, KeyEventArgs evt) {
            if (m_intelliSense.Visible) {
                if (evt.KeyCode == Keys.Left || evt.KeyCode == Keys.Right) {
                    // 表示終了
                    HideIntellisence();
                }
            }
        }

        //=========================================================================================
        // 機　能：フォーカスが失われたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxCommand_Leave(object sender, EventArgs evt) {
            if (m_intelliSense.Visible) {
                HideIntellisence();
            }
        }

        //=========================================================================================
        // 機　能：リンクの「?」がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            InfoBox.Information(Program.MainWindow, Resources.DlgCommand_Help);
        }

        //=========================================================================================
        // 機　能：履歴削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDeleteHistory_Click(object sender, EventArgs evt) {
            DialogResult result = DeleteHistoryProcedure.ConfirmDeleteHistory(this, Resources.DlgCommand_DelHistory);
            if (result != DialogResult.Yes) {
                return;
            }
            DeleteHistoryProcedure.DeleteCommandHistory();
            InfoBox.Information(this, Resources.Option_PrivacyCommandCompleted);
            this.comboBoxCommand.Items.Clear();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_command = null;
            if (this.comboBoxCommand.Text == "") {
                InfoBox.Warning(this, Resources.DlgCommand_CommandEmpty);
                return;
            }
            m_command = this.comboBoxCommand.Text;
            if (this.comboBoxResultMode.SelectedIndex == 0) {
                m_relayMode = ShellExecuteRelayMode.RelayLogWindow;
            } else if (this.comboBoxResultMode.SelectedIndex == 1) {
                m_relayMode = ShellExecuteRelayMode.RelayFileViewer;
            } else {
                m_relayMode = ShellExecuteRelayMode.None;
            }
            m_commandHistory.AddHistory(new CommandHistoryItem(m_command));
            m_commandHistory.SaveData();
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void CommandInputDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_command == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // プロパティ：入力されたコマンド
        //=========================================================================================
        public string Command {
            get {
                return m_command;
            }
        }

        //=========================================================================================
        // プロパティ：標準出力の中継モード
        //=========================================================================================
        public ShellExecuteRelayMode RelayMode {
            get {
                return m_relayMode;
            }
        }

        //=========================================================================================
        // クラス：インテリセンス対応のコンボボックス
        //=========================================================================================
        public class CommandComboBox : ComboBox {
            // TABキーをアプリ定義のキーに使用するときtrue
            private bool m_enableTab = false;

            //=========================================================================================
            // 機　能：TABキーによる入力を許可する
            // 引　数：[in]enabled  TABキーをアプリ定義のキーに使用するときtrue
            // 戻り値：なし
            //=========================================================================================
            public void EnableTab(bool enabled) {
                m_enableTab = enabled;
            }

            //=========================================================================================
            // 機　能：入力処理を行ってもよいキーかどうかを判断する
            // 引　数：[in]keyData  キー
            // 戻り値：keyDataの処理をアプリケーション側で行う予定のときtrue
            //=========================================================================================
            protected override bool IsInputKey(Keys keyData) {
                if (m_enableTab && keyData == Keys.Tab) {
                    return true;
                }
                return base.IsInputKey(keyData);
            }
        }
    }
}
