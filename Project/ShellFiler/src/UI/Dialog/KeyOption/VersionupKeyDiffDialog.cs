using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document.Setting;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：バージョンアップ時のキー情報ダイアログ
    //=========================================================================================
    public partial class VersionupKeyDiffDialog : Form {
        // キー設定の差分
        private KeySettingMergeDiff m_keySettingMergeDiff;

        // ダイアログを閉じてよいときtrue
        private bool m_closeOk = false;

        //=========================================================================================
        // 機　能：閉じるボタンがクリックされたときの処理を行う
        // 引　数：[in]diff  キー設定の差分
        // 戻り値：なし
        //=========================================================================================
        public VersionupKeyDiffDialog(KeySettingMergeDiff diff) {
            InitializeComponent();
            m_keySettingMergeDiff = diff;
            this.pictureBoxIcon.Image = SystemIcons.Information.ToBitmap();

            // お知らせを作成
            this.textBoxInformation.Text = CreateInformation(m_keySettingMergeDiff.PrevConvigVersion);

            // API一覧を取得
            CommandApiLoader loader = new CommandApiLoader();
            CommandSpec commandSpec = loader.Load();
            CommandScene fileListCommandList = null;
            CommandScene fileViewerCommandList = null;
            CommandScene graphicsViewerCommandList = null;
            if (commandSpec != null) {
                fileListCommandList = commandSpec.FileList;
                fileViewerCommandList = commandSpec.FileViewer;
                graphicsViewerCommandList = commandSpec.GraphicsViewer;
            }

            // ファイル一覧
            CreateListView(this.listViewFileList, fileListCommandList, m_keySettingMergeDiff.FileListMergedList, m_keySettingMergeDiff.FileListConflictList);
            CreateListView(this.listViewFileViewer, fileViewerCommandList, m_keySettingMergeDiff.FileViewerMergedList, m_keySettingMergeDiff.FileViewerConflictList);
            CreateListView(this.listViewGraphicsViewer, graphicsViewerCommandList, m_keySettingMergeDiff.GraphicsViewerMergedList, m_keySettingMergeDiff.GraphicsViewerConflictList);

            this.ActiveControl = this.buttonCopy;
        }

        //=========================================================================================
        // 機　能：お知らせのメッセージを作成する
        // 引　数：[in]prevVersion  直前のバージョン
        // 戻り値：メッセージ
        //=========================================================================================
        private string CreateInformation(int prevVersion) {
            List<string> messageList = new List<string>();
            if (prevVersion < 1000003) {
                messageList.Add(Resources.DlgVerup_Ver1000003_Assosiate);
            }

            // メッセージを整形
            StringBuilder message = new StringBuilder();
            if (messageList.Count == 0) {
                message.Append(Resources.DlgVerup_VerMessageNone);
            } else {
                foreach (string messageItem in messageList) {
                    message.Append(messageItem);
                    message.Append("\r\n");
                }
            }
            return message.ToString();
        }

        //=========================================================================================
        // 機　能：リストビューの項目リストを作成する
        // 引　数：[in]listView         作成対象のリストビュー
        // 　　　　[in]metaCommandList  コマンド一覧のメタ情報
        // 　　　　[in]mergedList       マージされたキー設定の一覧
        // 　　　　[in]conflictList     マージできなかったキー設定の一覧
        // 戻り値：なし
        //=========================================================================================
        private void CreateListView(ListView listView, CommandScene metaCommandList, List<KeyItemSetting> mergedList, List<KeyItemSetting> conflictList) {
            listView.SmallImageList = UIIconManager.IconImageList;
            foreach (KeyItemSetting itemSetting in mergedList) {
                ListViewItem lvItem = CreateListViewItem(itemSetting, metaCommandList, true);
                listView.Items.Add(lvItem);
            }
            foreach (KeyItemSetting itemSetting in conflictList) {
                ListViewItem lvItem = CreateListViewItem(itemSetting, metaCommandList, false);
                listView.Items.Add(lvItem);
            }
        }

        //=========================================================================================
        // 機　能：リストビューの項目1件分を作成する
        // 引　数：[in]itemSetting      作成するキー設定
        // 　　　　[in]metaCommandList  コマンド一覧のメタ情報
        // 　　　　[in]stateMerged      マージ済みの状態で作成するときtrue
        // 戻り値：なし
        //=========================================================================================
        private ListViewItem CreateListViewItem(KeyItemSetting itemSetting, CommandScene metaCommandList, bool stateMerged) {
            // コマンド名
            string commandName = itemSetting.ActionCommandMoniker.CreateActionCommand().UIResource.Hint;
            if (metaCommandList != null) {
                string classFullName = itemSetting.ActionCommandMoniker.CommandType.FullName;
                if (metaCommandList.ClassNameToApi.ContainsKey(classFullName)) {        // マウス関連などはエントリなし
                    string folderName = metaCommandList.ClassNameToApi[classFullName].ParentGroup.GroupDisplayName;
                    commandName = string.Format(Resources.DlgVerupKey_CommandName, folderName, commandName);
                }
            }

            // キー名
            string keyName = itemSetting.KeyState.GetDisplayNameKey(null);
            
            // ステータス
            string state;
            int imageIndex;
            if (stateMerged) {
                state = Resources.DlgVerupKey_StateMerged;
                imageIndex = (int)IconImageListID.Icon_FileAttributeDetailYes;
            } else {
                state = Resources.DlgVerupKey_StateManual;
                imageIndex = (int)IconImageListID.Icon_FileAttributeDetailNo;
            }

            // 項目を作成
            ListViewItem lvItem = new ListViewItem(new string[] { commandName, keyName, state});
            lvItem.ImageIndex = imageIndex;
            return lvItem;
        }

        //=========================================================================================
        // 機　能：クリップボードにコピーボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCopy_Click(object sender, EventArgs evt) {
            string text = GetLogText();
            Clipboard.SetDataObject(text, true);
 
            InfoBox.Information(this, Resources.DlgVerupKey_ClipCompleted);
        }

        //=========================================================================================
        // 機　能：ログのテキスト内容を作成する
        // 引　数：なし
        // 戻り値：ログのテキスト
        //=========================================================================================
        private string GetLogText() {
            StringBuilder sb = new StringBuilder();

            // お知らせ
            sb.Append(Resources.DlgVerupKey_ClipInformation).Append("\r\n");
            string message = CreateInformation(m_keySettingMergeDiff.PrevConvigVersion);
            sb.Append(message);
            sb.Append("\r\n");
            sb.Append("\r\n");

            // ファイル一覧
            sb.Append(Resources.DlgVerupKey_ClipKeybind).Append("\r\n");
            sb.Append(Resources.DlgVerupKey_ClipFileList).Append("\r\n");
            AppendKeyList(sb, this.listViewFileList);
            sb.Append("\r\n");

            // ファイルビューア
            sb.Append(Resources.DlgVerupKey_ClipFileViewer).Append("\r\n");
            AppendKeyList(sb, this.listViewFileViewer);
            sb.Append("\r\n");

            // グラフィックビューア
            sb.Append(Resources.DlgVerupKey_ClipGraphicsViewer).Append("\r\n");
            AppendKeyList(sb, this.listViewGraphicsViewer);

            // クリップボードへ
            string text = sb.ToString();
            return text;
        }

        //=========================================================================================
        // 機　能：キー一覧の文字列を作成する
        // 引　数：[in]sb        一覧を登録する文字列バッファ
        // 　　　　[in]listView  対象のリストビュー
        // 戻り値：なし
        //=========================================================================================
        private void AppendKeyList(StringBuilder sb, ListView listView) {
            for (int i = 0; i < listView.Items.Count; i++) {
                sb.Append(listView.Items[i].SubItems[0].Text).Append("\t");
                sb.Append(listView.Items[i].SubItems[1].Text).Append("\t");
                sb.Append(listView.Items[i].SubItems[2].Text).Append("\r\n");
            }
        }

        //=========================================================================================
        // 機　能：ダイアログが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void VersionupKeyDiffDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            if (!m_closeOk) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：閉じるボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClose_Click(object sender, EventArgs evt) {
            m_closeOk = false;

            // バックアップファイル名を決定
            FileVersionInfo ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            int version = ver.FileMajorPart * 1000000 + ver.FileMinorPart * 1000 + ver.FilePrivatePart;
            string bakFile = DirectoryManager.KeySettingBackupBase + "." + version;
            string logFile = DirectoryManager.KeySettingBackupBase + "." + version + ".log";

            // バックアップ
            bool success = Program.Document.KeySetting.Backup(bakFile);
            if (!success) {
                DialogResult result = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgVerupKey_FailedBackup, bakFile);
                if (result != DialogResult.Yes) {
                    return;
                }
            } else {
                WriteLog(logFile);
            }

            // 保存して結果を報告
            Program.Document.KeySetting.SaveSetting();
            if (success) {
                InfoBox.Information(this, Resources.DlgVerupKey_DialogClose, bakFile);
            } else {
                InfoBox.Information(this, Resources.DlgVerupKey_DialogCloseNoBackup);
            }
            m_closeOk = true;
            Close();
        }

        //=========================================================================================
        // 機　能：ログファイルを出力する
        // 引　数：[in]logFile  ログファイル名
        // 戻り値：なし
        //=========================================================================================
        private void WriteLog(string logFile) {
            string contents = GetLogText();
            string[] contentsLines = StringUtils.SeparateLine(contents);
            try {
                File.WriteAllLines(logFile, contentsLines);
            } catch (Exception) {
            }
        }
    }
}
