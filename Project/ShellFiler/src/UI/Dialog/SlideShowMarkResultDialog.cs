using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.GraphicsViewer;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：スライドショーでのマーク結果ダイアログ
    //=========================================================================================
    public partial class SlideShowMarkResultDialog : Form {
        // スライドショーでのマーク結果
        private SlideShowMarkResult m_markResult;

        // マークボタンの一覧
        private Button[] m_buttonMarkList;

        // 強制クローズするときtrue
        private bool m_forceClose = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]markResult  スライドショーでのマーク結果
        // 戻り値：なし
        //=========================================================================================
        public SlideShowMarkResultDialog(SlideShowMarkResult markResult) {
            InitializeComponent();
            m_markResult = markResult;
            this.textBoxFolder.Text = markResult.Folder;

            // ファイル一覧
            this.listViewFiles.SmallImageList = UIIconManager.IconImageList;
            foreach (SlideShowMarkResult.ImageFileInfo fileInfo in markResult.FileList) {
                string itemMark;
                if (fileInfo.MarkState != 0) {
                    itemMark = string.Format(Resources.DlgSlideShowResult_MarkItemName, fileInfo.MarkState);
                } else {
                    itemMark = "-";
                }
                string itemFileName = string.Format(fileInfo.FileName);
                string[] itemName = { itemMark, itemFileName };
                ListViewItem lvItem = new ListViewItem(itemName);
                if (fileInfo.MarkState != 0) {
                    lvItem.ImageIndex = (int)(IconImageListID.GraphicsViewer_MarkFile1) + fileInfo.MarkState - 1;
                }
                this.listViewFiles.Items.Add(lvItem);
            }

            // ボタン
            m_buttonMarkList = new Button[] {
                this.buttonMark1, this.buttonMark2, this.buttonMark3, this.buttonMark4, this.buttonMark5, 
                this.buttonMark6, this.buttonMark7, this.buttonMark8, this.buttonMark9
            };
            for (int i = 0; i < m_buttonMarkList.Length; i++) {
                int count = m_markResult.GetMarkImageCount(i + 1);
                m_buttonMarkList[i].ImageList = UIIconManager.IconImageList;
                m_buttonMarkList[i].ImageIndex = (int)(IconImageListID.GraphicsViewer_MarkFile1) + i;
                m_buttonMarkList[i].Enabled = (count > 0);
                m_buttonMarkList[i].Click += new EventHandler(ButtonMark_Click);
            }

            this.ActiveControl = this.buttonMark1;
        }

        //=========================================================================================
        // 機　能：ダイアログが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SlideShowMarkResultDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            if (!m_forceClose) {
                Program.MainWindow.OnCloseSlideShowResult();
            }
        }

        //=========================================================================================
        // 機　能：マークボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ButtonMark_Click(object sender, EventArgs evt) {
            // フォルダをチェック
            string current = GenericFileStringUtils.RemoveLastDirectorySeparator(TargetFileListView.FileList.DisplayDirectoryName);
            string folder = GenericFileStringUtils.RemoveLastDirectorySeparator(m_markResult.Folder);
            if (current != folder || TargetFileListView.FileList.LoadingStatus != FileListLoadingStatus.Completed) {
                InfoBox.Warning(this, Resources.DlgSlideShowResult_NeedFolderChange);
                return;
            }

            // クリックされたボタンを取得
            int buttonIndex = -1;
            for (int i = 0; i < m_buttonMarkList.Length; i++) {
                if (m_buttonMarkList[i] == sender) {
                    buttonIndex = i;
                    break;
                }
            }
            if (buttonIndex == -1) {
                return;
            }

            // マーク一覧を作成
            List<string> targetFileList = m_markResult.GetMarkImage(buttonIndex + 1);
            TargetFileListView.FileListViewComponent.MarkSpecifiedFile(targetFileList, MarkOperation.Revert);
        }

        //=========================================================================================
        // 機　能：クリアボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClear_Click(object sender, EventArgs evt) {
            TargetFileListView.FileListViewComponent.MarkAllFile(MarkAllFileMode.ClearAll, true, null);
        }

        //=========================================================================================
        // 機　能：ヘルプボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SlideShowMarkResultDialog_HelpButtonClicked(object sender, CancelEventArgs evt) {
            HelpMessageDialog dialog = new HelpMessageDialog(Resources.DlgMarkFileHelp_Title, Resources.HtmlSlideShowMark);
            dialog.ShowDialog(this);
        }

        //=========================================================================================
        // 機　能：フォルダ変更ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonChdir_Click(object sender, EventArgs evt) {
            string current = GenericFileStringUtils.RemoveLastDirectorySeparator(TargetFileListView.FileList.DisplayDirectoryName);
            string folder = GenericFileStringUtils.RemoveLastDirectorySeparator(m_markResult.Folder);
            if (current == folder) {
                return;
            }
            ChdirCommand.ChangeDirectory(TargetFileListView, new ChangeDirectoryParam.Direct(m_markResult.Folder));
        }

        //=========================================================================================
        // 機　能：閉じるボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClose_Click(object sender, EventArgs evt) {
            Close();
        }

        //=========================================================================================
        // プロパティ：対象ファイル一覧のビュー
        //=========================================================================================
        private FileListView TargetFileListView {
            get {
                if (Program.Document.CurrentTabPage.IsCursorLeft) {
                    return Program.MainWindow.LeftFileListView;
                } else {
                    return Program.MainWindow.RightFileListView;
                }
            }
        }

        //=========================================================================================
        // プロパティ：強制クローズするときtrue
        //=========================================================================================
        public bool ForceClose {
            set {
                m_forceClose = value;
            }
        }
    }
}
