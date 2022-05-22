using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.KeyOption;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：キーバインドヘルプダイアログ
    //=========================================================================================
    public partial class KeyBindHelpDialog : Form {
        // 初期化が完了したときtrue
        private bool m_initialized = false;

        // 読み込まれたコマンド一覧
        private CommandSpec m_commandSpec;
        
        // 入力されたキー
        private KeyState m_resultKey;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public KeyBindHelpDialog() {
            InitializeComponent();

            // カラムを初期化
            ColumnHeader columnCommand = new ColumnHeader();
            columnCommand.Text = Resources.DlgKeyBind_ColumnCommand;
            columnCommand.Width = MainWindowForm.X(280);
            ColumnHeader columnKey = new ColumnHeader();
            columnKey.Text = Resources.DlgKeyBind_ColumnKey;
            columnKey.Width = -2;
            this.listViewKey.Columns.AddRange(new ColumnHeader[] {columnCommand, columnKey});

            this.listViewKey.SmallImageList = UIIconManager.IconImageList;
        }

        //=========================================================================================
        // 機　能：ダイアログの再描画イベントを処理する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void KeyBindHelpDialog_Paint(object sender, PaintEventArgs e) {
            if (!m_initialized) {
                m_initialized = true;
                Program.MainWindow.BeginInvoke(new ExecutePostDelegate(ExecutePost));
            }
        }

        //=========================================================================================
        // 機　能：キー入力を実行する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private delegate void ExecutePostDelegate();
        private void ExecutePost() {
            // コマンド一覧を取得
            CommandApiLoader loader = new CommandApiLoader();
            m_commandSpec = loader.Load();
            if (m_commandSpec == null) {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            KeyBindHelpInputDialog dialog = new KeyBindHelpInputDialog();
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }
            InitializeKeyList(dialog.ResultKeyCode);
        }

        //=========================================================================================
        // 機　能：キー一覧を初期化する
        // 引　数：[in]keyCode  入力されたキー
        // 戻り値：なし
        //=========================================================================================
        private void InitializeKeyList(Keys keyCode) {
            // コマンドを取得
            KeyItemSettingList keySetting = Program.Document.KeySetting.FileListKeyItemList;
            KeyState[] keyList = new KeyState[] {
                new KeyState(keyCode, false, false, false),
                new KeyState(keyCode, true, false, false),
                new KeyState(keyCode, false, true, false),
                new KeyState(keyCode, true, true, false),
                new KeyState(keyCode, false, false, true),
                new KeyState(keyCode, true, false, true),
                new KeyState(keyCode, false, true, true),
                new KeyState(keyCode, true, true, true),
                new KeyState(keyCode, TwoStrokeType.Key1),
                new KeyState(keyCode, TwoStrokeType.Key2),
                new KeyState(keyCode, TwoStrokeType.Key3),
                new KeyState(keyCode, TwoStrokeType.Key4),
            };

            this.listViewKey.Items.Clear();
            for (int i = 0; i < keyList.Length; i++) {
                KeyState key = keyList[i];
                KeyItemSetting keyItem = keySetting.GetSettingFromKey(key);
                if (keyItem == null) {
                    continue;
                }

                CommandApi api = m_commandSpec.FileList.ClassNameToApi[keyItem.ActionCommandMoniker.CommandType.FullName];
                UIResource uiResource = keyItem.ActionCommandMoniker.CreateActionCommand().UIResource;
                int icon = (int)(uiResource.IconIdLeft);
                string command = uiResource.Hint;
                string keyName = key.GetDisplayName(keySetting);

                ListViewItem item = new ListViewItem(command);
                item.SubItems.Add(keyName);
                item.ImageIndex = icon;
                item.Tag = new KeyItemTag(keyItem, api);
                this.listViewKey.Items.Add(item);
            }
            if (this.listViewKey.Items.Count > 0) {
                this.listViewKey.Items[0].Selected = true;
            }
        }

        //=========================================================================================
        // 機　能：キー一覧の選択が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewKey_SelectedIndexChanged(object sender, EventArgs evt) {
            if (this.listViewKey.SelectedItems.Count == 0) {
                return;
            }
            KeyItemTag tag = (KeyItemTag)(this.listViewKey.SelectedItems[0].Tag);
            CommandApi api = tag.Api;
            object[] paramList = tag.KeyItem.ActionCommandMoniker.Parameter;
            string exp = KeySettingOptionDialog.CreateCommandExplanation(api, paramList);
            this.textBoxExplanation.Text = exp;
        }

        //=========================================================================================
        // 機　能：キー一覧がダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewKey_DoubleClick(object sender, EventArgs evt) {
            buttonOk_Click(this, evt);
        }

        //=========================================================================================
        // 機　能：再試行ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonRetry_Click(object sender, EventArgs evt) {
            ExecutePost();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.listViewKey.SelectedItems.Count == 0) {
                return;
            }
            KeyItemTag tag = (KeyItemTag)(this.listViewKey.SelectedItems[0].Tag);
            m_resultKey = tag.KeyItem.KeyState;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：ダイアログで入力されたキー
        //=========================================================================================
        public KeyState ResultKey {
            get {
                return m_resultKey;
            }
        }

        //=========================================================================================
        // クラス：キー一覧の項目のTagに設定される値
        //=========================================================================================
        private class KeyItemTag {
            // 対応するキー設定
            public KeyItemSetting KeyItem;

            // キー設定で定義された機能
            public CommandApi Api;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]key  対応するキー設定
            // 　　　　[in]api  キー設定で定義された機能
            // 戻り値：なし
            //=========================================================================================
            public KeyItemTag(KeyItemSetting key, CommandApi api) {
                KeyItem = key;
                Api = api;
            }
        }
    }
}
