using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Provider;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：外部からのドロップ時の操作入力ダイアログ
    //=========================================================================================
    public partial class DropActionExternalDialog : Form {
        private const int ICON_INDEX_FILE   = 0;        // イメージリストのインデックス：ファイル
        private const int ICON_INDEX_FOLDER = 1;        // イメージリストのインデックス：フォルダ

        // ドロップされたファイル情報の一覧
        private List<SimpleFileDirectoryPath> m_dropFileList;

        // 共通のルート（共通のルートがないときはnull）
        private string m_commonRoot;

        // イメージリスト
        private ImageList m_imageList = new ImageList();

        // ダイアログ入力の結果により決まった操作
        private ResultActionType m_resultAction;

        // 操作がフォルダ変更の場合のパラメータ（フォルダ変更以外の場合はnull）
        private ChangeDirectoryParam m_changeDirectoryParam;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileList     ドロップされたファイルの一覧
        // 　　　　[in]enableTrans  表示中フォルダへの転送が有効なときtrue
        // 戻り値：なし
        //=========================================================================================
        public DropActionExternalDialog(List<SimpleFileDirectoryPath> fileList, bool enableTrans) {
            InitializeComponent();
            m_dropFileList = fileList;
            if (!enableTrans) {
                this.buttonCopy.Enabled = false;
                this.buttonMove.Enabled = false;
                this.buttonShortcut.Enabled = false;
            }
        }

        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void DropActionDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            m_imageList.Dispose();
        }

        //=========================================================================================
        // 機　能：ダイアログが読み込まれたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void DropActionDialog_Load(object sender, EventArgs evt) {
            // 共通のルートを確認
            m_commonRoot = GenericFileStringUtils.GetCommonRoot(m_dropFileList);
            if (m_commonRoot == null) {
                this.buttonChdir.Enabled = false;
            }

            // 一覧の項目を作成
            ColumnHeader column = new ColumnHeader();
            column.Width = -2;
            this.listViewFiles.Columns.AddRange(new ColumnHeader[] {column});

            // 一覧をUIに反映
            FileIconManager manager = Program.Document.FileIconManager;
            m_imageList.ImageSize = new Size(UIIconManager.CX_DEFAULT_ICON, UIIconManager.CY_DEFAULT_ICON);
            m_imageList.Images.Add(manager.GetFileIcon(manager.DefaultFileIconId, manager.DefaultFileIconId, FileListViewIconSize.IconSize16).IconImage);
            m_imageList.Images.Add(manager.GetFileIcon(manager.DefaultFolderIconId, manager.DefaultFolderIconId, FileListViewIconSize.IconSize16).IconImage);
            this.listViewFiles.SmallImageList = m_imageList;
            foreach (SimpleFileDirectoryPath dropFile in m_dropFileList) {
                ListViewItem item = new ListViewItem(FormUtils.CreateListViewString(dropFile.FilePath));
                if (dropFile.IsDirectory) {
                    item.ImageIndex = ICON_INDEX_FOLDER;
                } else {
                    item.ImageIndex = ICON_INDEX_FILE;
                }
                this.listViewFiles.Items.Add(item);
            }

            // ファイル/フォルダ数を反映
            int fileCount = 0;
            int folderCount = 0;
            foreach (SimpleFileDirectoryPath dropFile in m_dropFileList) {
                if (dropFile.IsDirectory) {
                    folderCount++;
                } else {
                    fileCount++;
                }
            }
            this.labelFileCount.Text = fileCount.ToString();
            this.labelFolderCount.Text = folderCount.ToString();
        }

        //=========================================================================================
        // 機　能：ドロップされた一覧から実在するファイルとフォルダのリストを作成する
        // 引　数：[in]fileList  ドロップされたファイルの一覧
        // 戻り値：実在するファイルとフォルダのリスト
        //=========================================================================================
        public static List<SimpleFileDirectoryPath> CreateDropFileList(string[] fileList) {
            List<SimpleFileDirectoryPath> result = new List<SimpleFileDirectoryPath>();
            foreach (string file in fileList) {
                if (File.Exists(file)) {
                    result.Add(new SimpleFileDirectoryPath(file, false, false));
                } else if (Directory.Exists(file)) {
                    result.Add(new SimpleFileDirectoryPath(file, true, false));
                }
            }
            return result;
        }

        //=========================================================================================
        // 機　能：コピーボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCopy_Click(object sender, EventArgs evt) {
            m_resultAction = ResultActionType.Copy;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：移動ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonMove_Click(object sender, EventArgs evt) {
            m_resultAction = ResultActionType.Move;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：ショートカットの作成ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonShortcut_Click(object sender, EventArgs evt) {
            m_resultAction = ResultActionType.Shortcut;
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：カレントの変更ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonChdir_Click(object sender, EventArgs evt) {
            if (m_commonRoot == null) {
                return;
            }

            // 先頭のドロップファイルをカーソルとする
            string cursorFile = m_dropFileList[0].FilePath.Substring(m_commonRoot.Length);
            if (cursorFile.StartsWith("\\")) {
                cursorFile = cursorFile.Substring(1);
            }
            cursorFile = GenericFileStringUtils.SplitSubDirectoryList(cursorFile)[0];
            if (cursorFile == "") {
                cursorFile = null;
            }

            // フォルダを変更
            m_resultAction = ResultActionType.ChangeDir;
            m_changeDirectoryParam = new ChangeDirectoryParam.DirectAndSetCursor(m_commonRoot, cursorFile);
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：ダイアログ入力の結果により決まった操作
        //=========================================================================================
        public ResultActionType ResultAction {
            get {
                return m_resultAction;
            }
        }

        //=========================================================================================
        // プロパティ：操作がフォルダ移動の場合のパラメータ
        //=========================================================================================
        public ChangeDirectoryParam ChangeDirectoryParam {
            get {
                return m_changeDirectoryParam;
            }
        }

        //=========================================================================================
        // 列挙子：ダイアログの入力結果のアクション
        //=========================================================================================
        public enum ResultActionType {
            Copy,                   // コピーを実行
            Move,                   // 移動を実行
            Shortcut,               // ショートカットの作成を実行
            ChangeDir,              // カレントフォルダを変更
        }
    }
}
