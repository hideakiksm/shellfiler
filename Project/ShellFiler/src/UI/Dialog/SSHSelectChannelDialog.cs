using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem.Shell;
using ShellFiler.Locale;
using ShellFiler.Properties;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：SSHの使用チャネル選択ダイアログ
    //=========================================================================================
    public partial class SSHSelectChannelDialog : Form {
        // 選択されたチャネルのインデックス（新規のとき、指定されたIDリストの範囲外）
        private int m_selectedChannelIndex;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]channelList  チャネルの候補一覧
        // 　　　　[in]targetDir    対象ディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public SSHSelectChannelDialog(List<TerminalShellChannel> channelList, string targetDir) {
            InitializeComponent();

            this.textBoxTarget.Text = targetDir;

            // 既存のチャネルを追加
            ListViewItem selectedItem = null;
            int priorityMax = -1;
            this.listView.SmallImageList = UIIconManager.IconImageList;
            for (int i = 0; i < channelList.Count; i++) {
                TerminalShellChannel channel = channelList[i];
                if (!channel.ForFileSystem) {
                    continue;
                }
                ConsoleScreen console = channel.ConsoleScreen;
                int priority;
                bool notUsed;
                string state = CreateUsedInfoString(channel.ID, out priority, out notUsed);
                string[] items = new string[] {
                    console.DisplayName,
                    console.UserServer,
                    DateTimeFormatter.DateTimeToInformation(console.StartTime),
                    state,
                };
                ListViewItem lvItem = new ListViewItem(items);
                lvItem.Tag = i;
                lvItem.ImageIndex = (int)(notUsed ? IconImageListID.Icon_TerminalNotAssigned : IconImageListID.Icon_TerminalAssigned);
                this.listView.Items.Add(lvItem);
                if (priority > priorityMax) {
                    selectedItem = lvItem;
                    priorityMax = priority;
                }
            }

            // 新しいチャネルを追加
            string[] newItems = new string[] {
                 Resources.DlgSSHChannelSelect_ChannelNew,
                 "----",
                 "----",
                 "----",
            };
            ListViewItem lvItemNew = new ListViewItem(newItems);
            lvItemNew.Tag = channelList.Count;
            lvItemNew.ImageIndex = (int)(IconImageListID.Icon_TerminalNew);
            this.listView.Items.Add(lvItemNew);
            if (priorityMax < 1 || selectedItem == null) {
                selectedItem = lvItemNew;
            }

            // 選択状態にする
            selectedItem.Selected = true;
            this.ActiveControl = this.listView;
        }

        //=========================================================================================
        // 機　能：指定されたIDのチャネルが他で使用されているかどうかの文字列を作成する
        // 引　数：[in]id         確認するID
        // 　　　　[out]priority  一覧中で優先的にデフォルトの選択状態にする項目の優先度を返す変数（値が大きいほど優先）
        // 　　　　[out]notUsed   使用中ではないチャネルのときtrueを返す変数
        // 戻り値：使用されているかどうかの項目文字列
        //=========================================================================================
        private string CreateUsedInfoString(TerminalShellChannelID id, out int priority, out bool notUsed) {
            List<string> usedList = new List<string>();
            bool used;
            priority = 2;
            notUsed = false;

            // 左右画面をチェック
            used = CheckShellContextUsed(id, Program.Document.CurrentTabPage.LeftFileList);
            if (used) {
                usedList.Add(Resources.DlgSSHChannelSelect_IdLeft);
                priority = 3;
            }
            used = CheckShellContextUsed(id, Program.Document.CurrentTabPage.RightFileList);
            if (used) {
                usedList.Add(Resources.DlgSSHChannelSelect_IdRight);
                priority = 3;
            }

            // 裏のタブをチェック
            bool usedInTab = false;
            List<TabPageInfo> tabList = Program.Document.TabPageList.AllList;
            for (int i = 0; i < tabList.Count; i++) {
                if (tabList[i] != Program.Document.CurrentTabPage) {
                    used = CheckShellContextUsed(id, tabList[i].LeftFileList);
                    if (used) {
                        usedInTab = true;
                        break;
                    }
                    used = CheckShellContextUsed(id, tabList[i].RightFileList);
                    if (used) {
                        usedInTab = true;
                        break;
                    }
                }
            }
            if (usedInTab) {
                usedList.Add(Resources.DlgSSHChannelSelect_IdTab);
                if (priority != 3) {
                    priority = 0;
                }
            }

            // 整形
            if (usedList.Count == 0) {
                notUsed = true;
                return Resources.DlgSSHChannelSelect_IdNouse;
            } else {
                string itemName = StringUtils.CombineStringArray(usedList, Resources.DlgSSHChannelSelect_IdSeparator);
                return itemName;
            }
        }

        //=========================================================================================
        // 機　能：指定されたファイル一覧で指定されたIDが使用されているかどうかを調べる
        // 引　数：[in]id        確認するID
        // 　　　　[in]fileList  確認するファイル一覧
        // 戻り値：IDが使用されているときtrue
        //=========================================================================================
        private bool CheckShellContextUsed(TerminalShellChannelID id, UIFileList fileList) {
            if (fileList.FileSystem.FileSystemId == FileSystemID.SSHShell) {
                ShellFileListContext context = (ShellFileListContext)(fileList.FileListContext);
                if (context != null) {
                    if (context.TerminalShellChannelId == id) {
                        return true;
                    }
                }
            }
            return false;
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
            m_selectedChannelIndex = (int)(this.listView.SelectedItems[0].Tag);
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：チャネルの選択のリンクボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelChannel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            HelpMessageDialog dialog = new HelpMessageDialog(Resources.DlgSSHChannelSelect_HelpTitle, Resources.HtmlSSHSession);
            dialog.ShowDialog(this);
        }

        //=========================================================================================
        // プロパティ：選択されたチャネルのインデックス（新規のとき、指定されたIDリストの範囲外）
        //=========================================================================================
        public int SelectedChannelIndex {
            get {
                return m_selectedChannelIndex;
            }
        }
    }
}
