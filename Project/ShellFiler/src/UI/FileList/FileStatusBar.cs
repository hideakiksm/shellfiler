using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.SSH;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem.Shell;
using ShellFiler.Terminal.TerminalSession;

namespace ShellFiler.UI {

    //=========================================================================================
    // クラス：ファイル一覧でのステータスバー
    //=========================================================================================
    public partial class FileStatusBar : StatusStrip {
        // 待機中イメージのイメージインデックス
        private IconImageListID[] m_statusWaitImageIndex = {
            IconImageListID.StatusWait01,
            IconImageListID.StatusWait02,
            IconImageListID.StatusWait03,
            IconImageListID.StatusWait04,
            IconImageListID.StatusWait05,
            IconImageListID.StatusWait06,
            IconImageListID.StatusWait07,
            IconImageListID.StatusWait08,
            IconImageListID.StatusWait09,
            IconImageListID.StatusWait10,
            IconImageListID.StatusWait11,
            IconImageListID.StatusWait12,
        };

        // 左側の領域
        private ToolStripStatusLabel toolLabelLeft;

        // 右側の領域
        private ToolStripStatusLabel toolLabelRight;

        // 現在のアニメーションのインデックス
        private int m_currentStatusWaitIndex = 0;

        // 左側のステータスバーのときtrue
        private bool m_isLeft;

        // ステータスバーの背景色
        private Color m_statusBarBackColorDefault;

        // ファイル一覧を読み込み中のときtrue
        private bool m_loading = false;

        // エラーメッセージ表示の実装
        private StatusBarErrorMessageImpl m_errorMessageImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileStatusBar() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
            this.ImageList = UIIconManager.IconImageList;

            this.Padding = new Padding(this.Padding.Left, this.Padding.Top, this.Padding.Left, this.Padding.Bottom); 
            this.GripMargin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            
            // 左側の領域
            this.toolLabelLeft = new ToolStripStatusLabel();
            this.toolLabelLeft.Margin = new Padding(0, 3, 0, 0);
            this.toolLabelLeft.Name = "toolLabelRight";
            this.toolLabelLeft.Size = new System.Drawing.Size(118, 19);
            this.toolLabelLeft.Spring = true;
            this.toolLabelLeft.DoubleClickEnabled = true;
            this.toolLabelLeft.Text = "";
            this.toolLabelLeft.TextAlign = ContentAlignment.MiddleLeft;
            this.toolLabelLeft.ImageAlign = ContentAlignment.MiddleLeft;
            this.toolLabelLeft.Image = null;
            m_statusBarBackColorDefault = this.toolLabelLeft.BackColor;

            // 右側の領域
            this.toolLabelRight = new ToolStripStatusLabel();
            this.toolLabelRight.AutoToolTip = true;
            this.toolLabelRight.Margin = new Padding(0, 3, 0, 0);
            this.toolLabelRight.Name = "toolLabelRight";
            this.toolLabelRight.Size = new Size(118, 19);
            this.toolLabelRight.AutoSize = true;
            this.toolLabelRight.Spring = false;
            this.toolLabelRight.DoubleClickEnabled = true;
            this.toolLabelRight.Text = "";
            this.toolLabelRight.TextAlign = ContentAlignment.MiddleRight;
            
            this.Items.AddRange(new ToolStripItem[] {this.toolLabelLeft, this.toolLabelRight});

            m_errorMessageImpl = new StatusBarErrorMessageImpl(this, this.components, this.toolLabelLeft, new StatusBarErrorMessageImpl.RefreshStatusBarDelegate(RefreshMarkInfo));
        }
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]isLeft    左側ウィンドウのときtrue
        // 　　　　[in]fileList  対応するファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(bool isLeft, UIFileList fileList) {
            m_isLeft = isLeft;
            fileList.LoadStateChanged += new UIFileList.LoadStateChangedEventHandler(FileList_LoadStateChanged);
        }

        //=========================================================================================
        // 機　能：ステータスバーの情報を最新に更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshStatusBar() {
            RefreshMarkInfo();
            RefreshVolumeInfo();
        }

        //=========================================================================================
        // 機　能：ディスク情報を整形する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshMarkInfo() {
            bool done = m_errorMessageImpl.RefreshStatusBar();
            if (done) {
                return;
            }

            UIFileList fileList = Program.Document.CurrentTabPage.GetFileList(m_isLeft);
            if (m_loading) {
                // 読み込み中
                this.toolLabelLeft.ImageIndex = (int)m_statusWaitImageIndex[m_currentStatusWaitIndex];
                this.toolLabelLeft.Text = Resources.StatusBarLoading;
            } else if (fileList.MarkedDirectoryCount > 0 || fileList.MarkedFileCount > 0) {
                // 選択中: 0dir  99file  99MB
                string text = "";
                if (fileList.MarkedDirectoryCount > 0 || fileList.MarkedFileCount > 0) {
                    string strMarkSize = FormatDiskSize(fileList.MarkedFileSize, true);
                    text = string.Format(Resources.StatusBarMarkInfo, fileList.MarkedDirectoryCount, fileList.MarkedFileCount, strMarkSize);
                }
                this.toolLabelLeft.Text = text;
                this.toolLabelLeft.ImageIndex = -1;
                this.toolLabelLeft.Image = null;
            } else if (fileList.FileSystem.FileSystemId == FileSystemID.SSHShell) {
                // SSHシェルの一覧を表示中はユーザー名@サーバー名を表示
                TerminalShellChannelID channelId = ((ShellFileListContext)(fileList.FileListContext)).TerminalShellChannelId;
                List<TerminalShellChannel> channelList = Program.Document.FileSystemFactory.SSHConnectionManager.GetAuthorizedTerminalChannel(null);
                string userServer = "";
                bool isRoot = false;
                for (int i = 0; i < channelList.Count; i++) {
                    if (channelList[i].ID == channelId) {
                        userServer = channelList[i].ActiveUserServer;
                        ShellFileListContext context = (ShellFileListContext)(fileList.FileListContext);
                        string rootUserName = context.ShellCommandDictionary.ValueRootUserName;
                        isRoot = SSHUtils.IsSuperUser(userServer, rootUserName);
                        break;
                    }
                }
                if (isRoot) {
                    this.toolLabelLeft.BackColor = Configuration.Current.FileListStatusBarSuperUserColor;
                } else {
                    this.toolLabelLeft.BackColor = m_statusBarBackColorDefault;
                }
                this.toolLabelLeft.Text = userServer;
                this.toolLabelLeft.ImageIndex = -1;
                this.toolLabelLeft.Image = null;
            } else {
                this.toolLabelLeft.Text = "";
                this.toolLabelLeft.ImageIndex = -1;
                this.toolLabelLeft.Image = null;
            }
        }

        //=========================================================================================
        // 機　能：ディスク情報を整形する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshVolumeInfo() {
            UIFileList fileList = Program.Document.CurrentTabPage.GetFileList(m_isLeft);
            VolumeInfo volumeInfo = fileList.VolumeInfo;
            string text = "";
            string hint = "";
            if (volumeInfo != null && volumeInfo.FreeSize != -1) {
                // 使用:34.553GB  空き:41.903GB(54%)
                string strUsedSize = FormatDiskSize(volumeInfo.UsedDiskSize, false);
                string strFreeSize = FormatDiskSize(volumeInfo.FreeSize, false);
                string strRatio = "";
                if (volumeInfo.FreeRatio >= 0) {
                    strRatio = string.Format("{0}%", volumeInfo.FreeRatio.ToString());
                } else {
                    strRatio = "??%";
                }
                text = string.Format(Resources.StatusBarDiskInfo, strUsedSize, strFreeSize, strRatio);
                hint = volumeInfo.DriveEtcInfo;
            } else if (volumeInfo != null) {
                text = "";
                hint = volumeInfo.DriveEtcInfo;
            }
            this.toolLabelRight.Text = text;
            this.toolLabelRight.ToolTipText = hint;
            this.ShowItemToolTips = true;
        }

        //=========================================================================================
        // 機　能：ディスクサイズを整形する
        // 引　数：[in]size   ディスクサイズ
        // 　　　　[in]isMark マーク用のサイズ文字列を作成するときtrue
        // 戻り値：ディスクサイズの文字列表現
        //=========================================================================================
        private string FormatDiskSize(long size, bool isMark) {
            const long TERA = 1024L * 1024L * 1024L * 1024L;
            const long GIGA = 1024L * 1024L * 1024L;
            const long MEGA = 1024L * 1024L;
            const long KIRO = 1024L;
            string str = "";
            if (isMark && size < MEGA) {
                str = string.Format("{0}KB", size / KIRO);
            } else if (size >= TERA) {
                str = string.Format("{0}.{1:D3}TB", size / TERA, (size / GIGA) % 1024L * 1000L / 1024L);
            } else if (size >= GIGA) {
                str = string.Format("{0}.{1:D3}GB", size / GIGA, (size / MEGA) % 1024L * 1000L / 1024L);
            } else if (size >= MEGA) {
                str = string.Format("{0}.{1:D3}MB", size / MEGA, (size / KIRO) % 1024L * 1000L / 1024L);
            } else if (size >= KIRO) {
                str = string.Format("{0}.{1:D3}KB", size / KIRO, size % 1024L * 1000L / 1024L);
            } else {
                str = string.Format("{0}B", size);
            }
            return str;
        }

        //=========================================================================================
        // 機　能：ファイル一覧の読み込み状態が変化したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void FileList_LoadStateChanged(object sender, UIFileList.LoadStateChangedEventArgs evt) {
            if (evt.EventType == ChangeDirectoryStatus.Loading) {
                m_loading = true;
                m_currentStatusWaitIndex = 0;
                this.timerAnimation.Start();
            } else {
                m_loading = false;
                this.timerAnimation.Stop();
            }
            RefreshMarkInfo();
        }

        //=========================================================================================
        // 機　能：ステータスバーの領域がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileStatusBar_Click(object sender, EventArgs e) {
            // カーソルをこちらへ移動
            if (m_isLeft != Program.Document.CurrentTabPage.IsCursorLeft) {
                Program.MainWindow.ToggleCursorLeftRight();
            }
        }

        //=========================================================================================
        // 機　能：ステータスバーの領域がダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileStatusBar_DoubleClick(object sender, EventArgs evt) {
            // カーソルをこちらへ移動
            if (m_isLeft != Program.Document.CurrentTabPage.IsCursorLeft) {
                Program.MainWindow.ToggleCursorLeftRight();
            }
            Program.MainWindow.OnUICommand(UICommandSender.MainStatusBar, UICommandItem.StatusBar);
        }

        //=========================================================================================
        // 機　能：アニメーションタイマーのイベント受信時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void timerAnimation_Tick(object sender, EventArgs evt) {
            if (m_errorMessageImpl.HasError) {
                return;
            }
            m_currentStatusWaitIndex = (m_currentStatusWaitIndex + 1) % m_statusWaitImageIndex.Length;
            this.toolLabelLeft.ImageIndex = (int)m_statusWaitImageIndex[m_currentStatusWaitIndex];
        }

        //=========================================================================================
        // 機　能：エラーメッセージを更新する
        // 引　数：[in]message   エラーメッセージ
        // 　　　　[in]level     エラーのレベル
        // 　　　　[in]icon      使用するアイコン
        // 戻り値：なし
        //=========================================================================================
        public void ShowErrorMessage(string message, FileOperationStatus.LogLevel level, IconImageListID icon) {
            m_errorMessageImpl.ShowErrorMessageWorkThread(message, level, icon, StatusBarErrorMessageImpl.DisplayTime.Default);
        }
    }
}
