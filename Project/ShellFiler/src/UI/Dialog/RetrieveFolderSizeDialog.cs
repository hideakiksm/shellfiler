using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：フォルダサイズの取得ダイアログ
    //=========================================================================================
    public partial class RetrieveFolderSizeDialog : Form {
        // 対象パスがSSHのときtrue
        private bool m_isSSH;

        // 対象パスのセクタサイズ（無効なとき-1、m_isSSH=trueのとき無視）
        private int m_sectorTarget;

        // 反対パスのセクタサイズ（無効なとき-1、m_isSSH=trueのとき無視）
        private int m_sectorOpposite;

        // 入力した条件
        private RetrieveFolderSizeCondition m_resultSizeInfo;

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：[in]fileListTarget    対象パスのファイル一覧
        // 　　　　[in]fileListOpposite  反対パスのファイル一覧
        // 　　　　[in]condition         ダイアログのデフォルト
        // 戻り値：なし
        //=========================================================================================
        public RetrieveFolderSizeDialog(FileListView fileListTarget, FileListView fileListOpposite, RetrieveFolderSizeCondition condition) {
            InitializeComponent();

            // セクタサイズを取得
            if (FileSystemID.IsWindows(fileListTarget.FileList.FileSystem.FileSystemId)) {
                m_sectorTarget = GetSectorSize(fileListTarget.FileList.DisplayDirectoryName);
            } else {
                m_sectorTarget = -1;
            }
            if (m_sectorTarget == -1) {
                this.labelTarget.Text = string.Format(this.labelTarget.Text, "---");
            } else {
                this.labelTarget.Text = string.Format(this.labelTarget.Text, m_sectorTarget);
            }
            if (FileSystemID.IsWindows(fileListOpposite.FileList.FileSystem.FileSystemId)) {
                m_sectorOpposite = GetSectorSize(fileListOpposite.FileList.DisplayDirectoryName);
            } else {
                m_sectorOpposite = -1;
            }
            if (m_sectorOpposite == -1) {
                this.labelOpposite.Text = string.Format(this.labelOpposite.Text, "---");
            } else {
                this.labelOpposite.Text = string.Format(this.labelOpposite.Text, m_sectorOpposite);
            }

            // UIのデフォルトを設定
            if (FileSystemID.IsSSH(fileListTarget.FileList.FileSystem.FileSystemId)) {
                m_isSSH = true;
                this.radioButtonTarget.Checked = true;
                this.ActiveControl = this.radioButtonTarget;
            } else {
                m_isSSH = false;
                RadioButton selected;
                if (condition.SizeMode == RetrieveFolderSizeCondition.FolderSizeMode.OriginalSize) {
                    selected = this.radioButtonOriginal;
                } else if (condition.SizeMode == RetrieveFolderSizeCondition.FolderSizeMode.TargetPathCluster && m_sectorTarget != -1) {
                    selected = this.radioButtonTarget;
                } else if (condition.SizeMode == RetrieveFolderSizeCondition.FolderSizeMode.OppositePathCluster && m_sectorOpposite != -1) {
                    selected = this.radioButtonOpposite;
                } else if (condition.SizeMode == RetrieveFolderSizeCondition.FolderSizeMode.SpecifiedSize) {
                    selected = this.radioButtonSpecify;
                } else if (m_sectorTarget != -1) {
                    selected = this.radioButtonTarget;
                } else {
                    selected = this.radioButtonOriginal;
                }
                selected.Checked = true;
                this.ActiveControl = selected;
            }
            this.numericSpecify.Minimum = Configuration.MIN_RETRIEVE_FOLDER_SIZE_UNIT;
            this.numericSpecify.Maximum = Configuration.MAX_RETRIEVE_FOLDER_SIZE_UNIT;
            this.numericSpecify.Value = condition.FolderSizeUnit;
            this.checkBoxLowerCache.Checked = condition.UseCache;

            KeyItemSettingList keySetting = Program.Document.KeySetting.FileListKeyItemList;
            ActionCommandMoniker moniker = new ActionCommandMoniker(ActionCommandOption.None, typeof(ClearFolderSizeCommand));
            this.textBoxDeleteShortcut.Text = string.Format(this.textBoxDeleteShortcut.Text, ToolBarImpl.CreateShortcutDisplayString(keySetting, moniker));

            this.labelLowerMessage.Text = string.Format(this.labelLowerMessage.Text, Configuration.Current.RetrieveFolderSizeKeepLowerDepth, Configuration.Current.RetrieveFolderSizeKeepLowerCount);
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            if (m_isSSH) {
                this.radioButtonOriginal.Enabled = false;
                this.radioButtonTarget.Enabled = true;
                this.labelTarget.Enabled = false;
                this.radioButtonOpposite.Enabled = false;
                this.labelOpposite.Enabled = false;
                this.radioButtonSpecify.Enabled = false;
                this.numericSpecify.Enabled = false;
            } else {
                this.radioButtonOriginal.Enabled = true;
                this.radioButtonTarget.Enabled = (m_sectorTarget != -1);
                this.labelTarget.Enabled = (m_sectorTarget != -1);
                this.radioButtonOpposite.Enabled = (m_sectorOpposite != -1);
                this.labelOpposite.Enabled = (m_sectorOpposite != -1);
                this.radioButtonSpecify.Enabled = true;
                this.numericSpecify.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：指定フォルダのセクタサイズを返す
        // 引　数：[in]path   調べるパス
        // 戻り値：セクタサイズ（エラーのとき-1）
        //=========================================================================================
        private int GetSectorSize(string path) {
            string root, sub;
            WindowsFileSystem winFileSystem = Program.Document.FileSystemFactory.WindowsFileSystem;
            winFileSystem.SplitRootPath(path, out root, out sub);

            uint sectorPerCluster = 0;
            uint bytesPerSector = 0;
            uint numberOfFreeClusters = 0;
            uint totalNumberOfClusters = 0;
            bool success = Win32API.Win32GetDiskFreeSpace(root, ref sectorPerCluster, ref bytesPerSector, ref numberOfFreeClusters, ref totalNumberOfClusters);
            if (!success) {
                return -1;
            }
            return (int)bytesPerSector;
        }

        //=========================================================================================
        // 機　能：結果の削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            bool deleted = ClearFolderSizeCommand.ClearFolderSize();
            if (deleted) {
                InfoBox.Information(Program.MainWindow, Resources.Msg_ClearFolderSize);
            } else {
                InfoBox.Information(Program.MainWindow, Resources.Msg_ClearFolderSizeUnnecessary);
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            RetrieveFolderSizeCondition resultSizeInfo = new RetrieveFolderSizeCondition();
            if (m_isSSH) {
                resultSizeInfo.SizeMode = RetrieveFolderSizeCondition.FolderSizeMode.TargetPathCluster;
                resultSizeInfo.FolderSizeUnit = 1;
                resultSizeInfo.UseCache = this.checkBoxLowerCache.Checked;
            } else {
                if (this.radioButtonOriginal.Checked) {
                    resultSizeInfo.SizeMode = RetrieveFolderSizeCondition.FolderSizeMode.OriginalSize;
                    resultSizeInfo.FolderSizeUnit = 1;
                } else if (this.radioButtonTarget.Checked) {
                    resultSizeInfo.SizeMode = RetrieveFolderSizeCondition.FolderSizeMode.TargetPathCluster;
                    resultSizeInfo.FolderSizeUnit = m_sectorTarget;
                } else if (this.radioButtonOpposite.Checked) {
                    resultSizeInfo.SizeMode = RetrieveFolderSizeCondition.FolderSizeMode.OppositePathCluster;
                    resultSizeInfo.FolderSizeUnit = m_sectorOpposite;
                } else {
                    int unit = (int)(this.numericSpecify.Value);
                    Configuration.ModifyRetrieveFolderSizeUnit(ref unit);
                    resultSizeInfo.SizeMode = RetrieveFolderSizeCondition.FolderSizeMode.SpecifiedSize;
                    resultSizeInfo.FolderSizeUnit = (int)(this.numericSpecify.Value);
                }
                resultSizeInfo.UseCache = this.checkBoxLowerCache.Checked;
            }
            m_resultSizeInfo = resultSizeInfo;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：入力した条件
        //=========================================================================================
        public RetrieveFolderSizeCondition ResultSizeInfo {
            get {
                return m_resultSizeInfo;
            }
        }
    }
}
