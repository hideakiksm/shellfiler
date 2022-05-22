using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Management;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイル捜査中のエラー一覧ダイアログ
    //=========================================================================================
    public partial class FileOperationErrorListDialog : Form {
        // ダイアログに表示するエラー情報
        private FileErrorInfo m_errorInfo;

        // 対象パスのファイル一覧
        private FileListView m_fileListViewTarget;
        
        // 反対パスのファイル一覧
        private FileListView m_fileListViewOpposite;

        // 転送元のルート（定まらないときはnull）
        private string m_srcDirectoryRoot = null;

        // イメージリスト
        private ImageList m_imageList;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]errorInfo         ダイアログに表示するエラー情報
        // 　　　　[in]fileListTarget    対象パスのファイル一覧
        // 　　　　[in]fileListOpposite  反対パスのファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public FileOperationErrorListDialog(FileErrorInfo errorInfo, FileListView fileListTarget, FileListView fileListOpposite) {
            m_errorInfo = errorInfo;
            m_fileListViewTarget = fileListTarget;
            m_fileListViewOpposite = fileListOpposite;

            InitializeComponent();

            // 転送元ルートを決定
            foreach (FileErrorInfoItem errorItem in errorInfo.ErrorList) {
                string rootDir = GenericFileStringUtils.GetDirectoryName(GenericFileStringUtils.RemoveLastDirectorySeparator(errorItem.SrcMarkObjectPath.FilePath));
                if (m_srcDirectoryRoot != null) {               // すべてのrootDirが同じならその値、違うものがあればnull
                    if (m_srcDirectoryRoot != rootDir) {
                        m_srcDirectoryRoot = null;
                        break;
                    }
                } else {
                    m_srcDirectoryRoot = rootDir;
                }
            }
            if (m_srcDirectoryRoot == null) {
                this.buttonMark.Enabled = false;
            }

            // エラー一覧を作成
            this.labelTitle.Text = string.Format(this.labelTitle.Text, errorInfo.TaskType.DisplayName);
            m_imageList = new ImageList();
            m_imageList.ColorDepth = ColorDepth.Depth32Bit;
            m_imageList.ImageSize = new Size(UIIconManager.CxDefaultIcon, UIIconManager.CyDefaultIcon);
            m_imageList.Images.Add(UIIconManager.IconOperationFailed);
            this.listViewErrorList.SmallImageList = m_imageList;

            foreach (FileErrorInfoItem item in errorInfo.ErrorList) {
                string hint, retry, path;
                if (item.RetryInfo != null) {
                    // 再試行可能
                    hint = item.RetryInfo.GetHintString();
                    retry = Resources.DlgFileErrList_RetryOk;
                    path = item.RetryInfo.ErrorFilePath;
                } else {
                    // 再試行不可能
                    hint = string.Format(Resources.DlgFileErrList_WholeMark, item.SrcMarkObjectPath.FilePath);
                    retry = Resources.DlgFileErrList_RetryNg;
                    path = "(" + item.SrcMarkObjectPath.FilePath + ")";
                }
                string[] itemStr = new string[4];
                itemStr[0] = item.FileOperarionType.LogString;
                itemStr[1] = retry;
                itemStr[2] = item.ResultStatus.Message;
                itemStr[3] = path;
                ListViewItem lvItem = new ListViewItem(itemStr);
                lvItem.ImageIndex = 0;
                lvItem.ToolTipText = hint;
                lvItem.Tag = item;
                this.listViewErrorList.Items.Add(lvItem);
            }

            // 再試行ボタン
            if (m_errorInfo.TaskType == BackgroundTaskType.MirrorCopy) {
                this.buttonRetry.Enabled = false;
            }
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileOperationErrorListDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            m_imageList.Dispose();
            Program.MainWindow.OnCloseFileErrorDialog(this);
        }

        //=========================================================================================
        // 機　能：マークボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonMark_Click(object sender, EventArgs evt) {
            if (m_srcDirectoryRoot == null) {
                return;
            }
            HashSet<string> markFileList = new HashSet<string>();
            foreach (FileErrorInfoItem errorItem in m_errorInfo.ErrorList) {
                string fileName = GenericFileStringUtils.GetFileName(GenericFileStringUtils.RemoveLastDirectorySeparator(errorItem.SrcMarkObjectPath.FilePath));
                markFileList.Add(fileName);
            }
            ChangeDirectoryParam chdirParam = new ChangeDirectoryParam.DirectAndMark(m_srcDirectoryRoot, markFileList);
            ChdirCommand.ChangeDirectory(m_fileListViewTarget, chdirParam);
            Close();
        }

        //=========================================================================================
        // 機　能：再試行ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonRetry_Click(object sender, EventArgs evt) {
            // 仮想フォルダでは不可
            bool isVirtual = FileSystemID.IsVirtual(m_errorInfo.SrcFileSystem) || FileSystemID.IsVirtual(m_errorInfo.DestFileSystem);
            if (isVirtual) {
                InfoBox.Warning(this, Resources.DlgFileErrList_CanNotVirtualRetry);
                return;
            }

            // 再試行項目の構成を確認
            bool existRetryOk = false;
            bool existRetryNgFolder = false;
            bool existRetryNgFile = false;
            foreach (FileErrorInfoItem item in m_errorInfo.ErrorList) {
                if (item.RetryInfo != null) {
                    existRetryOk = true;
                } else {
                    if (item.SrcMarkObjectPath.IsDirectory) {
                        existRetryNgFolder = true;
                    } else {
                        existRetryNgFile = true;
                    }
                }
            }

            // 再試行してよいか確認
            bool includeFolder = false;         // 再試行対象にフォルダを含むときtrue
            if (existRetryNgFolder) {
                bool enableFile = (existRetryOk | existRetryNgFile);
                FileOperationErrorRetryNgDialog dialog = new FileOperationErrorRetryNgDialog(enableFile);
                DialogResult result = dialog.ShowDialog(this);
                if (result == DialogResult.Cancel) {
                    return;
                } else if (result == DialogResult.Yes) {
                    includeFolder = true;
                } else {
                    includeFolder = false;
                }
            }

            // 再試行情報にまとめる
            FileErrorRetryInfo retryInfo = new FileErrorRetryInfo(m_errorInfo.TaskType, m_errorInfo.SrcFileSystem, m_errorInfo.DestFileSystem, m_errorInfo.DestDiretoryRoot, m_errorInfo.Option);
            foreach (FileErrorInfoItem item in m_errorInfo.ErrorList) {
                if (item.RetryInfo == null) {
                    // 再試行不可能なものを先に登録
                    if (item.SrcMarkObjectPath.IsDirectory) {
                        if (includeFolder) {
                            retryInfo.AddRetryFileInfo(item.SrcMarkObjectPath);
                        }
                    } else {
                        retryInfo.AddRetryFileInfo(item.SrcMarkObjectPath);
                    }
                }
            }
            foreach (FileErrorInfoItem item in m_errorInfo.ErrorList) {
                if (item.RetryInfo != null) {
                    // 再試行可能なものを登録（不可能なものに含まれていれば登録しない）
                    retryInfo.AddRetryApiInfo(item.RetryInfo);
                }
            }

            // コマンドを実行
            FileListActionCommand command = Program.Document.CommandFactory.CreateFromRetryInfo(retryInfo);
            FileListCommandRuntime runtime = new FileListCommandRuntime(command);
            runtime.Execute();
            Close();
        }

        //=========================================================================================
        // 機　能：キャンセルボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            Close();
        }
    }
}
