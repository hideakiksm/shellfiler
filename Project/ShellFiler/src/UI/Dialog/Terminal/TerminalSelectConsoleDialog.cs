using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.Locale;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Terminal.UI;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.Terminal;

namespace ShellFiler.UI.Dialog.Terminal {

    //=========================================================================================
    // クラス：接続先コンソール選択ダイアログ
    //=========================================================================================
    public partial class TerminalSelectConsoleDialog : Form {
        // 選択されたチャネル（新規のときnull）
        private TerminalShellChannel m_channel;

        // 選択されたコンソール（新規のときnull）
        private ConsoleScreen m_console;
        
        // listView.Items.Tag
        // 既存ウィンドウはTerminalShellChannel
        // 新規ウィンドウはnull


        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]userServer  接続先のユーザー名@サーバー名（nullのときすべて）
        // 　　　　[in]emptyExist  未割り当てのシェルを発見したときの起動の場合true
        // 　　　　[in]shellList   接続先に対する既存のシェルチャネル一覧
        // 戻り値：なし
        //=========================================================================================
        public TerminalSelectConsoleDialog(string userServer, bool emptyExist, List<TerminalShellChannel> shellList) {
            InitializeComponent();

            // ラベル
            if (userServer == null) {
                this.labelMessage.Text = Resources.DlgTerminalSelectShell_LabelAll;
            } else if (emptyExist) {
                this.labelMessage.Text = string.Format(Resources.DlgTerminalSelectShell_LabelExistShell);
            } else {
                this.labelMessage.Text = string.Format(Resources.DlgTerminalSelectShell_LabelUserServer, userServer);
            }

            // 項目を初期化
            this.listView.SmallImageList = UIIconManager.IconImageList;
            for (int i = 0; i < shellList.Count; i++) {
                // 既存シェルの項目
                if (shellList[i].ForFileSystem) {
                    continue;
                }
                ConsoleScreen console = shellList[i].ConsoleScreen;
                
                string dispName;
                int iconId;
                TerminalForm form = Program.WindowManager.GetTerminalForm(console);
                if (form == null) {
                    dispName = Resources.DlgTerminalSelectShell_WindowNotExist;
                    iconId = (int)(IconImageListID.Icon_TerminalNotAssigned);
                } else {
                    dispName = Resources.DlgTerminalSelectShell_WindowExist;
                    iconId = (int)(IconImageListID.Icon_TerminalAssigned);
                }
                string[] column = new string[] {
                    console.DisplayName,
                    dispName,
                    DateTimeFormatter.DateTimeToInformation(console.StartTime),
                };
                ListViewItem item = new ListViewItem(column);
                item.ImageIndex = iconId;
                item.Tag = shellList[i];
                this.listView.Items.Add(item);
                if (i == 0) {
                    item.Selected = true;
                }
            }

            // 新規シェルの項目
            if (userServer != null) {
                ListViewItem itemNew = new ListViewItem(new string[] { Resources.DlgTerminalSelect_ConsoleNew, "----", "----" });
                itemNew.ImageIndex = (int)(IconImageListID.Icon_TerminalNew);
                itemNew.Tag = null;
                this.listView.Items.Add(itemNew);
            }
        }

        //=========================================================================================
        // 機　能：項目がダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listView_DoubleClick(object sender, EventArgs e) {
            buttonOk_Click(this, null);
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.listView.SelectedItems.Count == 0) {
                return;
            }
            m_channel = (TerminalShellChannel)(this.listView.SelectedItems[0].Tag);
            if (m_channel == null) {
                m_console = null;
            } else {
                m_console = m_channel.ConsoleScreen;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：入力結果を取得する
        // 引　数：[out]userServer  選択されたコンソールに接続したユーザー名@サーバー名（新規のときnull）
        // 　　　　[out]channel     選択されたチャネルを返す変数（新規のときnull）
        // 　　　　[in]console      選択されたコンソールを返す変数（新規のときnull）
        // 戻り値：なし
        //=========================================================================================
        public void GetResult(out string userServer, out TerminalShellChannel channel, out ConsoleScreen console) {
            if (m_console == null) {
                userServer = null;
                channel = null;
                console = null;
            } else {
                userServer = m_console.UserServer;
                channel = m_channel;
                console = m_console;
            }
        }
    }
}
