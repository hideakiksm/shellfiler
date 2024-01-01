using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.Util;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Dialog.Option;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.Properties;
using ShellFiler.Locale;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ディレクトリへのログインダイアログ
    //=========================================================================================
    public partial class LogInDirectoryDialog : Form {
        // ドライブページの実装（タブの初回表示まではnull）
        private DrivePage m_drivePage = null;
        
        // ブックマークページの実装（タブの初回表示まではnull）
        private BookmarkPage m_bookmarkPage = null;

        // SSHページの実装（タブの初回表示まではnull）
        private SSHPage m_sshPage = null;

        // SSH無効ページの実装（タブの初回表示まではnull）
        private SSHDisablePage m_sshDisablePage = null;

        // フォルダ履歴ページの実装（タブの初回表示まではnull）
        private PathHistoryPage m_pathHistoryPage = null;

        // SSHページとして使用するタブUI
        private TabPage m_sshPageTabUI;

        // SSHページが有効なときtrue
        private bool m_enableSSHPage;

        // 開始時点と終了時点でアクティブなページ
        private LogInDirectoryPage m_activePage;

        // SSHの一時ログインをするときtrue
        private bool m_sshTemporaryLogin = false;

        // 開始時点と終了時点での対象ディレクトリ（SSHの一時ログインのときnull）
        private string m_targetDirectory = null;

        // SSHシェルが選択され、常に新しいチャネルで接続するときtrue
        private bool m_shellNewChannel;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]activePage  アクティブにするページ
        // 　　　　[in]directory   現在の対象パスのディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public LogInDirectoryDialog(LogInDirectoryPage activePage, string directory) {
            InitializeComponent();
            m_activePage = activePage;
            m_targetDirectory = directory;
            if (Program.Document.FileSystemFactory.SFTPFileSystem == null) {
                this.tabPages.Controls.Remove(this.tabPageSSH);
                m_sshPageTabUI = this.tabPageSSHDisable;
                m_enableSSHPage = false;
            } else {
                this.tabPages.Controls.Remove(this.tabPageSSHDisable);
                m_sshPageTabUI = this.tabPageSSH;
                m_enableSSHPage = true;
            }
        }
        
        //=========================================================================================
        // 機　能：フォームが初期化されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogInDirectoryDialog_Load(object sender, EventArgs evt) {
            InitializePage();
        }

        //=========================================================================================
        // 機　能：タブページが切り替えられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void tabPages_SelectedIndexChanged(object sender, EventArgs evt) {
            if (this.tabPages.SelectedTab == this.tabPageDrive) {
                m_activePage = LogInDirectoryPage.LogInDrive;
            } else if (this.tabPages.SelectedTab == this.tabPageRegDir) {
                m_activePage = LogInDirectoryPage.LogInRegistered;
            } else if (this.tabPages.SelectedTab == this.tabPageHistory) {
                m_activePage = LogInDirectoryPage.LogInHistory;
            } else {
                m_activePage = LogInDirectoryPage.LogInSSH;
            }
            InitializePage();
        }

        //=========================================================================================
        // 機　能：ページを初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void InitializePage() {
            if (m_activePage == LogInDirectoryPage.LogInDrive) {
                if (m_drivePage == null) {
                    m_drivePage = new DrivePage(this);
                    this.tabPages.SelectedTab = this.tabPageDrive;
                }
                this.ActiveControl = this.listViewDrive;
            } else if (m_activePage == LogInDirectoryPage.LogInRegistered) {
                if (m_bookmarkPage == null) {
                    m_bookmarkPage = new BookmarkPage(this);
                    this.tabPages.SelectedTab = this.tabPageRegDir;
                }
                this.ActiveControl = this.listViewRegDir;
            } else if (m_activePage == LogInDirectoryPage.LogInSSH) {
                if (m_enableSSHPage) {
                    if (m_sshPage == null) {
                        m_sshPage = new SSHPage(this);
                        this.tabPages.SelectedTab = m_sshPageTabUI;
                    }
                    this.ActiveControl = this.treeViewSSH;
                } else {
                    if (m_sshDisablePage == null) {
                        m_sshDisablePage = new SSHDisablePage(this);
                        this.tabPages.SelectedTab = m_sshPageTabUI;
                    }
                    this.ActiveControl = this.linkLabelSSHDL;
                    this.linkLabelSSHDL.Focus();
                }
            } else if (m_activePage == LogInDirectoryPage.LogInHistory) {
                if (m_pathHistoryPage == null) {
                    m_pathHistoryPage = new PathHistoryPage(this);
                    this.tabPages.SelectedTab = this.tabPageHistory;
                }
            }
        }
        
        //=========================================================================================
        // 機　能：キーが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogInDirectoryDialog_KeyDown(object sender, KeyEventArgs evt) {
            if (this.tabPages.SelectedTab == this.tabPageDrive) {
                m_drivePage.LogInDirectoryDialog_KeyDown(sender, evt);
            } else if (this.tabPages.SelectedTab == this.tabPageRegDir) {
                m_bookmarkPage.LogInDirectoryDialog_KeyDown(sender, evt);
            } else if (this.tabPages.SelectedTab == this.tabPageSSH) {
                m_sshPage.LogInDirectoryDialog_KeyDown(sender, evt);
            } else if (this.tabPages.SelectedTab == this.tabPageHistory) {
                m_pathHistoryPage.LogInDirectoryDialog_KeyDown(sender, evt);
            } else {
                m_sshDisablePage.LogInDirectoryDialog_KeyDown(sender, evt);
            }
        }

        //=========================================================================================
        // 機　能：[ドライブ]ページ、[ネットワークの追加]ボタンが押されたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonConnect_Click(object sender, EventArgs evt) {
            m_drivePage.buttonConnect_Click(sender, evt);
        }

        //=========================================================================================
        // 機　能：[ドライブ]ページ、[ネットワークの切断]ボタンが押されたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDisconnect_Click(object sender, EventArgs evt) {
            m_drivePage.buttonDisconnect_Click(sender, evt);
        }
        
        //=========================================================================================
        // 機　能：[ドライブ]ページ、ドライブ一覧でマウスに変化があったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewDrive_MouseStateChange(object sender, MouseEventArgs evt) {
            m_drivePage.listViewDrive_MouseStateChange(sender, evt);
        }
        
        //=========================================================================================
        // 機　能：[ドライブ]ページ、ドライブ一覧でダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewDrive_DoubleClick(object sender, EventArgs evt) {
            buttonOk_Click(sender, null);
        }

        //=========================================================================================
        // 機　能：[登録フォルダ]ページ、グループ一覧で項目が変化したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void comboBoxRegDirGroup_SelectedValueChanged(object sender, EventArgs evt) {
            if (m_bookmarkPage == null) {
                return;
            }
            m_bookmarkPage.comboBoxRegDirGroup_SelectedValueChanged(sender, evt);
        }

        //=========================================================================================
        // 機　能：[登録フォルダ]ページ、フォルダ一覧で項目が変化したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewRegDir_SelectedIndexChanged(object sender, EventArgs evt) {
            if (m_bookmarkPage == null) {
                return;
            }
            m_bookmarkPage.listViewRegDir_SelectedIndexChanged(sender, evt);
        }

        //=========================================================================================
        // 機　能：[登録フォルダ]ページ、フォルダ一覧でマウスに変化があったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewRegDir_MouseStateChange(object sender, MouseEventArgs evt) {
            m_bookmarkPage.listViewRegDir_MouseStateChange(sender, evt);
        }
        
        //=========================================================================================
        // 機　能：[登録フォルダ]ページ、フォルダ一覧でダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewRegDir_DoubleClick(object sender, EventArgs evt) {
            buttonOk_Click(sender, null);
        }

        //=========================================================================================
        // 機　能：[登録フォルダ]ページ、設定ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonSetting_Click(object sender, EventArgs evt) {
            m_bookmarkPage.buttonSetting_Click(sender, evt);
        }

        //=========================================================================================
        // 機　能：OKボタンが押されたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.tabPages.SelectedTab == this.tabPageDrive) {
                // ドライブ一覧ページが開かれていたとき
                m_targetDirectory = m_drivePage.GetSelectedDriveRoot();
                if (m_targetDirectory == null) {
                    InfoBox.Warning(this, Resources.DlgLogInDir_NoSelectDrive);
                    return;
                }
                m_activePage = LogInDirectoryPage.LogInDrive;
            } else if (this.tabPages.SelectedTab == this.tabPageRegDir) {
                // 登録ディレクトリ一覧ページが開かれていたとき
                m_targetDirectory = this.textBoxRegDir.Text;
                if (m_targetDirectory == "") {
                    InfoBox.Warning(this, Resources.DlgLogInDir_NoSelectRegDir);
                    m_targetDirectory = null;
                    return;
                }
                m_activePage = LogInDirectoryPage.LogInRegistered;
            } else if (this.tabPages.SelectedTab == this.tabPageSSH) {
                // SSHページが開かれていたとき
                if (!m_enableSSHPage) {
                    DialogResult = DialogResult.Cancel;
                    Close();
                    return;
                }
                m_targetDirectory = m_sshPage.GetTargetPath();
                if (m_targetDirectory == "") {
                    InfoBox.Warning(this, Resources.DlgLogInDir_NoSelectSSH);
                    m_targetDirectory = null;
                    return;
                }
                m_shellNewChannel = this.checkBoxSSHNewChannel.Checked;
                m_activePage = LogInDirectoryPage.LogInSSH;
            } else if (this.tabPages.SelectedTab == this.tabPageHistory) {
                // フォルダ履歴ページが開かれていたとき
                string folder = m_pathHistoryPage.GetTargetPath();
                if (folder == null) {
                    InfoBox.Warning(this, Resources.DlgLogInDir_NoSelectRegDir);
                    m_targetDirectory = null;
                    return;
                }
                m_targetDirectory = folder;
                m_activePage = LogInDirectoryPage.LogInHistory;
            }

            // ダイアログを閉じる
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogInDirectoryDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_targetDirectory == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：ダイアログが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LogInDirectoryDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            Program.Document.SSHUserAuthenticateSetting.SaveData();
        }

        //=========================================================================================
        // 機　能：[SSH]ページ、リンクボタンが押されたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelSSHDL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
            InfoBox.Information(Program.MainWindow, Resources.DlgLoginDir_SSHDownloadInfo);
            Process.Start(KnownUrl.SharpSSHUrl);
        }

        //=========================================================================================
        // 機　能：[SSH]ページ、一時接続ボタンが押されたされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonSSHTemp_Click(object sender, EventArgs evt) {
            m_sshTemporaryLogin = true;
            DialogResult = DialogResult.OK;
            Close();
        }
        
        //=========================================================================================
        // 機　能：[SSH]ページ、キー入力を処理する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void linkLabelSSHDL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs evt) {
            if (m_sshDisablePage != null) {
                m_sshDisablePage.LogInDirectoryDialog_KeyDown(this, new KeyEventArgs(evt.KeyCode));
            }
        }

        //=========================================================================================
        // 機　能：[フォルダ履歴]ページ、履歴削除釦がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            m_pathHistoryPage.DeleteHistory();
        }

        //=========================================================================================
        // プロパティ：SSHの一時ログインをするときtrue
        //=========================================================================================
        public bool SSHTemporaryLogin {
            get {
                return m_sshTemporaryLogin;
            }
        }

        //=========================================================================================
        // プロパティ：移動先のディレクトリ（SSHの一時ログインのときnull）
        //=========================================================================================
        public string TargetDirectory {
            get {
                return m_targetDirectory;
            }
        }

        //=========================================================================================
        // プロパティ：SSHシェルが選択され、常に新しいチャネルで接続するときtrue
        //=========================================================================================
        public bool ShellNewChannel {
            get {
                return m_shellNewChannel;
            }
        }

        //=========================================================================================
        // クラス：[ドライブ]ページ
        //=========================================================================================
        private class DrivePage {
            // アイコンの幅
            private const int CX_ICON = 16;

            // アイコンの高さ
            private const int CY_ICON = 16;

            // 親ダイアログ
            private LogInDirectoryDialog m_parent;

            // カスタマイズ用アイコンのリスト
            private List<Bitmap> m_customizedIconList = new List<Bitmap>();

            // イメージリスト
            private ImageList m_imageList;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親となるプロパティシート
            // 戻り値：なし
            //=========================================================================================
            public DrivePage(LogInDirectoryDialog parent) {
                m_parent = parent;

                // ヘッダを追加
                ColumnHeader columnName = new ColumnHeader();
                columnName.Text = Resources.DlgLogInDir_DriveName;
                columnName.Width = 200;
                ColumnHeader columnTotal = new ColumnHeader();
                columnTotal.Text = Resources.DlgLogInDir_DriveTotal;
                columnTotal.Width = 120;
                columnTotal.TextAlign = HorizontalAlignment.Right;
                ColumnHeader columnFree = new ColumnHeader();
                columnFree.Text = Resources.DlgLogInDir_DriveFree;
                columnFree.Width = 120;
                columnFree.TextAlign = HorizontalAlignment.Right;
                m_parent.listViewDrive.Columns.AddRange(new ColumnHeader[] {columnName, columnTotal, columnFree});

                // イメージリストを追加
                m_imageList = new ImageList();
                m_imageList.ImageSize = new Size(CX_ICON, CY_ICON);
                m_parent.listViewDrive.SmallImageList = m_imageList;
                string drive = m_parent.m_targetDirectory;
                if (drive == null) {
                    drive = "";
                }
                RefreshDriveList(drive);
            }

            //=========================================================================================
            // 機　能：ドライブ一覧をリフレッシュする
            // 引　数：[in]selectDrive  新しく選択するドライブのルートが含まれるディレクトリ("":指定なし)
            // 戻り値：なし
            //=========================================================================================
            private void RefreshDriveList(string selectDrive) {
                // 現在の項目を破棄
                foreach (Bitmap bmp in m_customizedIconList) {
                    bmp.Dispose();
                }
                m_imageList.Images.Clear();
                m_customizedIconList.Clear();
                m_parent.listViewDrive.Items.Clear();

                // ドライブ情報を更新
                int selectedItem = 0;
                string[] driveList = Directory.GetLogicalDrives();
                foreach (string driveLetter in driveList) {
                    // ドライブ情報を取得
                    if (driveLetter.Length < 1) {
                        continue;
                    }
                    DriveInfo driveInfo = new DriveInfo(driveLetter);
                    
                    // アイコンをロード
                    Bitmap icon = DriveItem.GetDriveIcon(driveInfo);
                    m_customizedIconList.Add(icon);
                    m_imageList.Images.Add(icon);

                    // 項目を追加
                    ListViewItem item = CreateDriveItem(driveInfo);
                    m_parent.listViewDrive.Items.Add(item);
                    if ((driveLetter.ToUpper())[0] == (selectDrive.ToUpper())[0]) {
                        item.Selected = true;
                        selectedItem = m_parent.listViewDrive.Items.Count - 1;
                    }
                }

                // 選択状態にする
                if (m_parent.listViewDrive.Items.Count > 0) {
                    m_parent.listViewDrive.Select();
                    m_parent.listViewDrive.Items[selectedItem].Selected = true;
                    m_parent.listViewDrive.FocusedItem = m_parent.listViewDrive.Items[selectedItem];
                }

                // ツールバーのドライブ一覧も更新
                Program.MainWindow.RefreshDriveList();
            }

            //=========================================================================================
            // 機　能：ドライブ一覧のリスト項目を作成する
            // 引　数：[in]driveInfo  ドライブ情報
            // 戻り値：リスト項目
            //=========================================================================================
            private ListViewItem CreateDriveItem(DriveInfo driveInfo) {
                // 表示名を作成
                string driveLetter = driveInfo.Name;
                string driveName = "";
                switch (driveInfo.DriveType) {
                    case DriveType.CDRom:
                        driveName = Resources.ToolbarItem_DriveTypeCDROM + " (" + driveLetter[0] + ":)";
                        break;
                    case DriveType.Removable:
                        driveName = Resources.ToolbarItem_DriveTypeRemovable + " (" + driveLetter[0] + ":)";
                        break;
                    case DriveType.Network:
                        driveName = Resources.ToolbarItem_DriveTypeNetwork + " (" + driveLetter[0] + ":)";
                        break;
                    case DriveType.Ram:
                        driveName = Resources.ToolbarItem_DriveTypeRam + " (" + driveLetter[0] + ":)";
                        break;
                    case DriveType.Fixed:
                        driveName = Resources.ToolbarItem_DriveTypeFixed + " (" + driveLetter[0] + ":)";
                        break;
                    default:
                        driveName = "(" + driveLetter[0] + ":)";
                        break;
                }

                // 容量を取得
                string totalSize = "";
                string freeSize = "";
                switch (driveInfo.DriveType) {
                    case DriveType.Network:
                    case DriveType.Ram:
                    case DriveType.Fixed:
                        try {
                            totalSize = StringUtils.FileSizeToString(driveInfo.TotalSize);
                            freeSize = StringUtils.FileSizeToString(driveInfo.TotalFreeSpace);
                        } catch (Exception) {
                        }
                        break;
                }

                // 項目を作成
                string[] itemString = {driveName, totalSize, freeSize};
                ListViewItem item = new ListViewItem(itemString);
                item.ImageIndex = m_imageList.Images.Count - 1;
                item.Tag = driveLetter.Substring(0, 2);
                return item;
            }

            //=========================================================================================
            // 機　能：キーが押されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void LogInDirectoryDialog_KeyDown(object sender, KeyEventArgs evt) {
                if (evt.KeyCode == Keys.Right) {
                    // ページ変更
                    m_parent.tabPages.SelectedTab = m_parent.tabPageRegDir;
                    evt.SuppressKeyPress = true;
                } else if (evt.KeyCode == Keys.Enter) {
                    // 決定
                    m_parent.buttonOk_Click(this, null);
                } else if (!evt.Alt) {
                    // ドライブ変更
                    int value = evt.KeyValue;
                    if ('a' <= value && value <= 'z') {
                        value = value - 'a' + 'A';
                    }
                    if ('A' <= value && value <= 'Z') {
                        char driveChar = (char)value;
                        ListViewItem selectedItem = null;
                        foreach (ListViewItem item in m_parent.listViewDrive.Items) {
                            string drive = (string)(item.Tag);
                            drive = drive.ToUpper();
                            if (drive.Length > 1 && drive[0] == driveChar) {
                                selectedItem = item;
                                evt.SuppressKeyPress = true;
                                break;
                            }
                        }
                        if (selectedItem != null) {
                            selectedItem.Selected = true;
                            m_parent.listViewDrive.FocusedItem = selectedItem;
                            m_parent.buttonOk_Click(this, null);
                        }
                    }
                }
            }

            //=========================================================================================
            // 機　能：ドライブ一覧でマウスに変化があったときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void listViewDrive_MouseStateChange(object sender, MouseEventArgs evt) {
                if (m_parent.listViewDrive.SelectedItems.Count == 0) {
                    if (m_parent.listViewDrive.FocusedItem != null) {
                        m_parent.listViewDrive.FocusedItem.Selected = true;
                    } else if (m_parent.listViewDrive.Items.Count > 0) {
                        m_parent.listViewDrive.Items[0].Selected = true;
                    }
                }
            }

            //=========================================================================================
            // 機　能：[ネットワークの追加]ボタンが押されたされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void buttonConnect_Click(object sender, EventArgs evt) {
                string[] oldDriveList = Directory.GetLogicalDrives();
                Win32API.Win32WNetConnectionDialog(m_parent.Handle, Win32API.RESOURCETYPE_DISK);
                string[] newDriveList = Directory.GetLogicalDrives();

                // 追加ドライブを検索
                string newDriveRoot = null;
                HashSet<string> oldDriveSet= new HashSet<string>();
                foreach (string oldDrive in oldDriveList) {
                    oldDriveSet.Add(oldDrive);
                }
                foreach (string newDrive in newDriveList) {
                    if (!oldDriveSet.Contains(newDrive)) {
                        newDriveRoot = newDrive;
                        break;
                    }
                }

                // 追加ドライブがなかったら現在選択中のドライブ
                if (newDriveRoot == null) {
                    newDriveRoot = GetSelectedDriveRoot();
                }

                // ドライブリストを更新
                if (newDriveRoot != null) {
                    RefreshDriveList(newDriveRoot);
                } else {
                    RefreshDriveList("");
                }
            }

            //=========================================================================================
            // 機　能：[ネットワークの切断]ボタンが押されたされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void buttonDisconnect_Click(object sender, EventArgs evt) {
                Win32API.Win32WNetDisconnectDialog(m_parent.Handle, Win32API.RESOURCETYPE_DISK);
                string selectedDrive = GetSelectedDriveRoot();
                if (selectedDrive != null) {
                    RefreshDriveList(selectedDrive);
                } else {
                    RefreshDriveList("");
                }
            }
            
            //=========================================================================================
            // 機　能：選択中のドライブのルートディレクトリを返す
            // 引　数：なし
            // 戻り値：選択中のドライブのルートディレクトリ（異常時はnull）
            //=========================================================================================
            public string GetSelectedDriveRoot() {
                string selected = null;
                if (m_parent.listViewDrive.SelectedItems.Count > 0) {
                    selected = (string)(m_parent.listViewDrive.SelectedItems[0].Tag);
                }
                return selected;
            }
        }

        //=========================================================================================
        // クラス：[ブックマーク]ページ
        //=========================================================================================
        private class BookmarkPage {
            // 親ダイアログ
            private LogInDirectoryDialog m_parent;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親となるプロパティシート
            // 戻り値：なし
            //=========================================================================================
            public BookmarkPage(LogInDirectoryDialog parent) {
                m_parent = parent;
                BookmarkSetting bookmarkSetting = Program.Document.UserSetting.BookmarkSetting;
                bookmarkSetting.LoadData();

                // ヘッダを追加
                ColumnHeader columnName = new ColumnHeader();
                columnName.Width = m_parent.listViewRegDir.Width - 20;
                m_parent.listViewRegDir.Columns.Add(columnName);

                // イメージリストを追加
                FileIconManager iconManager = Program.Document.FileIconManager;
                m_parent.listViewRegDir.SmallImageList = UIIconManager.IconImageList;

                RefreshGroupList(bookmarkSetting, 0);
                RefreshDirectoryList(bookmarkSetting.BookmarkGroupList[0]);

#if FREE_VERSION
            // Freeware版
            m_parent.labelFreewareBook.Text = Resources.Dlg_FreewareInfo;
            m_parent.labelFreewareBook.BackColor = Color.LightYellow;
#endif
            }

            //=========================================================================================
            // 機　能：グループ一覧をリフレッシュする
            // 引　数：[in]setting       ブックマークの設定
            // 　　　　[in]initialGroup  はじめに表示するグループ
            // 戻り値：なし
            //=========================================================================================
            private void RefreshGroupList(BookmarkSetting setting, int initialGroup) {
                List<string> groupNameList = new List<string>();
                foreach (BookmarkGroup groupSetting in setting.BookmarkGroupList) {
                    groupNameList.Add(groupSetting.GroupName);
                }
                m_parent.comboBoxRegDirGroup.Items.Clear();
                m_parent.comboBoxRegDirGroup.Items.AddRange(groupNameList.ToArray());
                m_parent.comboBoxRegDirGroup.SelectedIndex = initialGroup;
            }

            //=========================================================================================
            // 機　能：登録ディレクトリ一覧をリフレッシュする
            // 引　数：[in]groupSetting  グループの設定
            // 戻り値：なし
            //=========================================================================================
            private void RefreshDirectoryList(BookmarkGroup groupSetting) {
                // 現在の項目を破棄
                m_parent.listViewRegDir.Items.Clear();

                // 一覧を更新
                List<BookmarkItem> regItemList = groupSetting.ItemList;
                foreach (BookmarkItem regItem in regItemList) {
                    string strItem = regItem.DisplayName + "(" + regItem.ShortCut + ")";
                    ListViewItem item = new ListViewItem(strItem);
                    item.ImageIndex = (int)IconImageListID.FileList_ChdirSpecial;
                    item.Tag = regItem;
                    m_parent.listViewRegDir.Items.Add(item);
                }

                // 選択状態にする
                if (m_parent.listViewRegDir.Items.Count > 0) {
                    m_parent.listViewRegDir.Select();
                    m_parent.listViewRegDir.Items[0].Selected = true;
                    m_parent.listViewRegDir.FocusedItem = m_parent.listViewRegDir.Items[0];
                }
            }

            //=========================================================================================
            // 機　能：キーが押されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void LogInDirectoryDialog_KeyDown(object sender, KeyEventArgs evt) {
                if (m_parent.ActiveControl is TextBox) {
                    // テキスト入力中
                    if (evt.KeyCode == Keys.Up) {
                        m_parent.ActiveControl = m_parent.listViewRegDir;
                    } else {
                        return;
                    }
                } else if (evt.KeyCode == Keys.Right) {
                    // ページ変更
                    m_parent.tabPages.SelectedTab = m_parent.m_sshPageTabUI;
                    evt.SuppressKeyPress = true;
                } else if (evt.KeyCode == Keys.Left) {
                    // ページ変更
                    m_parent.tabPages.SelectedTab = m_parent.tabPageDrive;
                    evt.SuppressKeyPress = true;
                } else if (evt.KeyCode == Keys.Enter) {
                    // 決定
                    m_parent.buttonOk_Click(this, null);
                } else if (!evt.Alt) {
                    // ショートカット
                    int value = evt.KeyValue;
                    if ('a' <= value && value <= 'z') {
                        value = value - 'a' + 'A';
                    } else if (value == (int)(Keys.OemMinus)) {
                        value = '-';
                    } else if (value == (int)(Keys.OemPipe)) {
                        value = '\\';
                    }
                    char shortCutChar = (char)value;
                    ListViewItem selectedItem = null;
                    foreach (ListViewItem item in m_parent.listViewRegDir.Items) {
                        BookmarkItem regItem = (BookmarkItem)(item.Tag);
                        if (regItem.ShortCut == shortCutChar) {
                            selectedItem = item;
                            evt.SuppressKeyPress = true;
                            break;
                        }
                    }
                    if (selectedItem != null) {
                        selectedItem.Selected = true;
                        m_parent.listViewRegDir.FocusedItem = selectedItem;
                        m_parent.buttonOk_Click(this, null);
                    }
                }
            }

            //=========================================================================================
            // 機　能：フォルダ一覧でマウスに変化があったときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void listViewRegDir_MouseStateChange(object sender, MouseEventArgs evt) {
                if (m_parent.listViewRegDir.SelectedItems.Count == 0) {
                    if (m_parent.listViewRegDir.FocusedItem != null) {
                        m_parent.listViewRegDir.FocusedItem.Selected = true;
                    } else if (m_parent.listViewRegDir.Items.Count > 0) {
                        m_parent.listViewRegDir.Items[0].Selected = true;
                    }
                }
            }

            //=========================================================================================
            // 機　能：[登録フォルダ]ページ、グループ一覧で項目が変化したときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void comboBoxRegDirGroup_SelectedValueChanged(object sender, EventArgs evt) {
                BookmarkSetting regDirSetting = Program.Document.UserSetting.BookmarkSetting;
                int index = m_parent.comboBoxRegDirGroup.SelectedIndex;
                BookmarkGroup group = regDirSetting.BookmarkGroupList[index];
                RefreshDirectoryList(group);
            }

            //=========================================================================================
            // 機　能：[登録フォルダ]ページ、フォルダ一覧で項目が変化したときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void listViewRegDir_SelectedIndexChanged(object sender, EventArgs evt) {
                BookmarkItem item = GetSelectedRegDir();
                if (item != null) {
                    m_parent.textBoxRegDir.Text = item.Directory;
                }
            }

            //=========================================================================================
            // 機　能：選択中の登録ディレクトリの設定項目を返す
            // 引　数：なし
            // 戻り値：選択中の登録ディレクトリの設定項目（異常時はnull）
            //=========================================================================================
            public BookmarkItem GetSelectedRegDir() {
                BookmarkItem item = null;
                if (m_parent.listViewRegDir.SelectedItems.Count > 0) {
                    item = (BookmarkItem)(m_parent.listViewRegDir.SelectedItems[0].Tag);
                }
                return item;
            }

            //=========================================================================================
            // 機　能：[登録フォルダ]ページ、設定ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void buttonSetting_Click(object sender, EventArgs evt) {
                BookmarkSetting bookmarkSetting = Program.Document.UserSetting.BookmarkSetting;
                BookmarkSettingDialog dialog = new BookmarkSettingDialog(bookmarkSetting, m_parent.m_targetDirectory);
                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK) {
                    return;
                }

                // グループを追加
                bookmarkSetting = dialog.BookmarkSetting;
                Program.Document.UserSetting.BookmarkSetting = bookmarkSetting;
                bookmarkSetting.SaveData();
                int currentIndex = Math.Min(m_parent.comboBoxRegDirGroup.SelectedIndex, bookmarkSetting.BookmarkGroupList.Count - 1);
                RefreshGroupList(bookmarkSetting, currentIndex);
                RefreshDirectoryList(bookmarkSetting.BookmarkGroupList[currentIndex]);
            }
        }
        
        //=========================================================================================
        // クラス：[SSH]ページ
        //=========================================================================================
        private class SSHPage {
            // アイコンの幅
            private const int CX_ICON = 16;

            // アイコンの高さ
            private const int CY_ICON = 16;

            // 親ダイアログ
            private LogInDirectoryDialog m_parent;

            // イメージリスト
            private ImageList m_imageList;

            // 編集対象の認証情報の全体
            SSHUserAuthenticateSetting m_authDatabase;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親となるプロパティシート
            // 戻り値：なし
            //=========================================================================================
            public SSHPage(LogInDirectoryDialog parent) {
                m_parent = parent;

                m_imageList = new ImageList();
                m_imageList.Images.Add(UIIconManager.IconSshSetting);

                m_parent.buttonSSHNew.Click += new EventHandler(this.buttonSSHNew_Click);
                m_parent.buttonSSHEdit.Click += new EventHandler(this.buttonSSHEdit_Click);
                m_parent.buttonSSHDelete.Click += new EventHandler(this.buttonSSHDelete_Click);
                m_parent.treeViewSSH.AfterLabelEdit += new NodeLabelEditEventHandler(this.treeViewSSH_AfterLabelEdit);
                m_parent.treeViewSSH.AfterSelect += new TreeViewEventHandler(this.treeViewSSH_AfterSelect);
                m_parent.treeViewSSH.DoubleClick += new EventHandler(this.treeViewSSH_DoubleClick);
                m_parent.radioButtonSSHSFTP.CheckedChanged += new EventHandler(this.radioButtonSSH_CheckedChanged);

                // プロトコル
                SSHProtocolType protocol = Configuration.Current.SshFileSystemDefault;
                if (protocol == SSHProtocolType.SFTP) {
                    m_parent.radioButtonSSHSFTP.Checked = true;
                } else {
                    m_parent.radioButtonSSHShell.Checked = true;
                }

                // ツリーに項目を追加
                m_authDatabase = Program.Document.SSHUserAuthenticateSetting;
                m_authDatabase.LoadData();
                List<TreeNode> nodeList = new List<TreeNode>();
                for (int i = 0; i < m_authDatabase.UserAuthenticateSettingList.Count; i++) {
                    SSHUserAuthenticateSettingItem setting = m_authDatabase.UserAuthenticateSettingList[i];
                    TreeNode node = new TreeNode(setting.DisplayName);
                    node.Tag = setting;
                    node.ImageIndex = 0;
                    nodeList.Add(node);
                }
                m_parent.treeViewSSH.ImageList = m_imageList;
                m_parent.treeViewSSH.Nodes.AddRange(nodeList.ToArray());
                m_parent.treeViewSSH.LabelEdit = true;
                if (nodeList.Count > 0) {
                    m_parent.treeViewSSH.SelectedNode = nodeList[0];
                    SSHUserAuthenticateSettingItem selectedSetting = (SSHUserAuthenticateSettingItem)(nodeList[0].Tag);
                    m_parent.textBoxSSHFolder.Text = SSHUtils.CreateUserServer(protocol, selectedSetting.UserName, selectedSetting.ServerName, selectedSetting.PortNo) + ":~/";
                } else {
                    m_parent.textBoxSSHFolder.Text = "";
                }
                m_parent.linkLabelHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSSHHelp_LinkClicked);

                EnableUIItem();

#if FREE_VERSION
            // Freeware版
            m_parent.labelFreewareSSH.Text = Resources.Dlg_FreewareInfo;
            m_parent.labelFreewareSSH.BackColor = Color.LightYellow;
#endif
            }

            //=========================================================================================
            // 機　能：ログイン先のパスを取得する
            // 引　数：なし
            // 戻り値：エラーのときnull
            //=========================================================================================
            public string GetTargetPath() {
                return m_parent.textBoxSSHFolder.Text;
            }

            //=========================================================================================
            // 機　能：キーが押されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void LogInDirectoryDialog_KeyDown(object sender, KeyEventArgs evt) {
                if (m_parent.ActiveControl is TextBox) {
                    // テキスト入力中
                    if (evt.KeyCode == Keys.Up) {
                        m_parent.ActiveControl = m_parent.treeViewSSH;
                    } else {
                        return;
                    }
                } else if (evt.KeyCode == Keys.Left) {
                    // ページ変更
                    m_parent.tabPages.SelectedTab = m_parent.tabPageRegDir;
                    evt.SuppressKeyPress = true;
                } else if (evt.KeyCode == Keys.Right) {
                    // ページ変更
                    m_parent.tabPages.SelectedTab = m_parent.tabPageHistory;
                    evt.SuppressKeyPress = true;
                } else if (evt.KeyCode == Keys.Enter) {
                    // 決定
                    m_parent.buttonOk_Click(this, null);
                }
            }

            //=========================================================================================
            // 機　能：[SSH]ページ、新規ボタンが押されたされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void buttonSSHNew_Click(object sender, EventArgs evt) {
                SSHConnectionDialog dialog = new SSHConnectionDialog(null, SSHConnectionDialog.ConnectMode.EditForSave);
                DialogResult result = dialog.ShowDialog(m_parent);
                if (result != DialogResult.OK) {
                    return;
                }

                SSHUserAuthenticateSettingItem setting = dialog.AuthenticateSetting;
                m_authDatabase.AddSetting(setting);

                TreeNode node = new TreeNode(setting.DisplayName);
                node.Tag = setting;
                node.ImageIndex = 0;
                m_parent.treeViewSSH.Nodes.Add(node);
                m_parent.treeViewSSH.SelectedNode = node;
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：[SSH]ページ、削除ボタンが押されたされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void buttonSSHDelete_Click(object sender, EventArgs evt) {
                TreeNode node = m_parent.treeViewSSH.SelectedNode;
                if (node == null) {
                    return;
                }
                SSHUserAuthenticateSettingItem setting = (SSHUserAuthenticateSettingItem)(node.Tag);

                DialogResult result = InfoBox.Question(m_parent, MessageBoxButtons.YesNo, Resources.DlgLoginDir_SSHDeleteConfirm, setting.DisplayName, setting.ServerName, setting.UserName);
                if (result != DialogResult.Yes) {
                    return;
                }

                m_parent.treeViewSSH.Nodes.Remove(node);
                m_authDatabase.DeleteUserAuthenticateSetting(setting.ServerName, setting.UserName, setting.PortNo);
                EnableUIItem();
            }
            
            //=========================================================================================
            // 機　能：[SSH]ページ、編集ボタンが押されたされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void buttonSSHEdit_Click(object sender, EventArgs evt) {
                TreeNode node = m_parent.treeViewSSH.SelectedNode;
                if (node == null) {
                    return;
                }
                SSHUserAuthenticateSettingItem setting = (SSHUserAuthenticateSettingItem)(node.Tag);
                SSHConnectionDialog dialog = new SSHConnectionDialog(setting, SSHConnectionDialog.ConnectMode.EditForSave);
                DialogResult result = dialog.ShowDialog(m_parent);
                if (result != DialogResult.OK) {
                    return;
                }
                node.Text = setting.DisplayName;
                SSHProtocolType protocol = GetSelectedProtocol();
                string directory = SSHUtils.CreateUserServer(protocol, setting.UserName, setting.ServerName, setting.PortNo) + ":~/";
                m_parent.textBoxSSHFolder.Text = directory;
            }

            //=========================================================================================
            // 機　能：[SSH]ページ、名前の変更が行われたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void treeViewSSH_AfterLabelEdit(object sender, NodeLabelEditEventArgs evt) {
                if (evt.Label == null) {
                    return;
                }
                if (evt.Label == "") {
                    evt.CancelEdit = true;
                    return;
                }
                SSHUserAuthenticateSettingItem targetSetting = (SSHUserAuthenticateSettingItem)(evt.Node.Tag);
                foreach (SSHUserAuthenticateSettingItem setting in m_authDatabase.UserAuthenticateSettingList) {
                    if (targetSetting != setting) {
                        if (setting.DisplayName == evt.Label) {
                            InfoBox.Warning(m_parent, Resources.DlgLoginDir_SSHDeleteSameName);
                            evt.CancelEdit = true;
                            return;
                        }
                    }
                }
                targetSetting.DisplayName = evt.Label;
            }

            //=========================================================================================
            // 機　能：[SSH]ページ、項目がダブルクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void treeViewSSH_DoubleClick(object sender, EventArgs evt) {
                m_parent.buttonOk_Click(null, null);
            }

            //=========================================================================================
            // 機　能：[SSH]ページ、ツリー上の項目が選択されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void treeViewSSH_AfterSelect(object sender, TreeViewEventArgs evt) {
                TreeNode node = m_parent.treeViewSSH.SelectedNode;
                if (node == null) {
                    m_parent.textBoxSSHFolder.Text = "";
                } else {
                    SSHUserAuthenticateSettingItem setting = (SSHUserAuthenticateSettingItem)(node.Tag);
                    SSHProtocolType protocol = GetSelectedProtocol();
                    string directory = SSHUtils.CreateUserServer(protocol, setting.UserName, setting.ServerName, setting.PortNo) + ":~/";
                    m_parent.textBoxSSHFolder.Text = directory;
                }
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：[SSH]ページ、ラジオボタンで選択されているプロトコルを返す
            // 引　数：なし
            // 戻り値：選択されているプロトコル
            //=========================================================================================
            private SSHProtocolType GetSelectedProtocol() {
                SSHProtocolType protocol;
                if (m_parent.radioButtonSSHSFTP.Checked) {
                    protocol = SSHProtocolType.SFTP;
                } else {
                    protocol = SSHProtocolType.SSHShell;
                }
                return protocol;
            }

            //=========================================================================================
            // 機　能：[SSH]ページ、SSH/SFTPのチェック状態が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void radioButtonSSH_CheckedChanged(object sender, EventArgs evt) {
                string text =  m_parent.textBoxSSHFolder.Text;
                if (m_parent.radioButtonSSHSFTP.Checked) {
                    if (text.StartsWith("ssh:")) {
                        text = "sftp:" + text.Substring(4);
                    }
                } else if (m_parent.radioButtonSSHShell.Checked) {
                    if (text.StartsWith("sftp:")) {
                        text = "ssh:" + text.Substring(5);
                    }
                }
                m_parent.textBoxSSHFolder.Text = text;
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：[SSH]ページのボタンの状態を変更する
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void EnableUIItem() {
                if (m_parent.treeViewSSH.SelectedNode != null) {
                    m_parent.buttonSSHEdit.Enabled = true;
                } else {
                    m_parent.buttonSSHEdit.Enabled = false;
                }
                if (m_parent.treeViewSSH.Nodes.Count == 0) {
                    m_parent.buttonSSHDelete.Enabled = false;
                    m_parent.textBoxSSHFolder.Text = "";
                } else {
                    m_parent.buttonSSHDelete.Enabled = true;
                }
                if (m_parent.radioButtonSSHSFTP.Checked) {
                    m_parent.checkBoxSSHNewChannel.Enabled = false;
                } else {
                    m_parent.checkBoxSSHNewChannel.Enabled = true;
                }
            }

            //=========================================================================================
            // 機　能：[SSH]ページのプロトコルヘルプのリンクがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void linkLabelSSHHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs evt) {
                HelpMessageDialog dialog = new HelpMessageDialog(Resources.DlgLoginDir_SSHProtocolTitle, Resources.HtmlSSHProtocol);
                dialog.ShowDialog(m_parent);
            }
        }

        //=========================================================================================
        // クラス：[SSH(無効状態)]ページ
        //=========================================================================================
        private class SSHDisablePage {
            // 親ダイアログ
            private LogInDirectoryDialog m_parent;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親となるプロパティシート
            // 戻り値：なし
            //=========================================================================================
            public SSHDisablePage(LogInDirectoryDialog parent) {
                m_parent = parent;
            }

            //=========================================================================================
            // 機　能：キーが押されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void LogInDirectoryDialog_KeyDown(object sender, KeyEventArgs evt) {
                if (evt.KeyCode == Keys.Left) {
                    // ページ変更
                    m_parent.tabPages.SelectedTab = m_parent.tabPageRegDir;
                    evt.SuppressKeyPress = true;
                } else if (evt.KeyCode == Keys.Right) {
                    // ページ変更
                    m_parent.tabPages.SelectedTab = m_parent.tabPageHistory;
                    evt.SuppressKeyPress = true;
                } else if (evt.KeyCode == Keys.Enter) {
                    // 決定
                    m_parent.buttonOk_Click(this, null);
                }
            }
        }

        //=========================================================================================
        // クラス：[フォルダ履歴]ページ
        //=========================================================================================
        private class PathHistoryPage {
            // 親ダイアログ
            private LogInDirectoryDialog m_parent;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親となるプロパティシート
            // 戻り値：なし
            //=========================================================================================
            public PathHistoryPage(LogInDirectoryDialog parent) {
                m_parent = parent;

                // 項目を追加
                List<PathHistoryItem> historyList = Program.Document.FolderHistoryWhole.HistoryList;
                string[] items = new string[historyList.Count];
                for (int i = 0; i < historyList.Count; i++) {
                    PathHistoryItem historyItem = historyList[historyList.Count - 1 - i];
                    items[i] = historyItem.Directory;
                }
                m_parent.listBoxHistory.Items.AddRange(items);
                if (items.Length > 0) {
                    m_parent.listBoxHistory.SelectedIndex = 0;
                }

                m_parent.ActiveControl = m_parent.listBoxHistory;
            }

            //=========================================================================================
            // 機　能：キーが押されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            public void LogInDirectoryDialog_KeyDown(object sender, KeyEventArgs evt) {
                if (evt.KeyCode == Keys.Left) {
                    // ページ変更
                    m_parent.tabPages.SelectedTab = m_parent.m_sshPageTabUI;
                    evt.SuppressKeyPress = true;
                } else if (evt.KeyCode == Keys.Enter) {
                    // 決定
                    m_parent.buttonOk_Click(this, null);
                }
            }

            //=========================================================================================
            // 機　能：履歴削除ボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void DeleteHistory() {
                DialogResult result = DeleteHistoryProcedure.ConfirmDeleteHistory(m_parent, Resources.Option_PrivacyFolderConfirm);
                if (result != DialogResult.Yes) {
                    return;
                }
                DeleteHistoryProcedure.DeleteFolderHistory();
                InfoBox.Information(m_parent, Resources.Option_PrivacyFolderCompleted);
                m_parent.listBoxHistory.Items.Clear();
            }

            //=========================================================================================
            // 機　能：ログイン先のパスを取得する
            // 引　数：なし
            // 戻り値：エラーのときnull
            //=========================================================================================
            public string GetTargetPath() {
                string folder;
                if (m_parent.listBoxHistory.Items.Count > 0) {
                    folder = m_parent.listBoxHistory.SelectedItem.ToString();
                } else {
                    folder = null;
                }
                return folder;
            }
        }

        //=========================================================================================
        // 列挙子：ダイアログの各ページを表現する識別子
        //=========================================================================================
        public enum LogInDirectoryPage {
            LogInDrive,             // ドライブ一覧
            LogInRegistered,        // 登録フォルダ一覧
            LogInSSH,               // SSH
            LogInHistory,           // フォルダ履歴
        }
    }
}
