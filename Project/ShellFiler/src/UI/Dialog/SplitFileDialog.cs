using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイルの分割ダイアログ
    //=========================================================================================
    public partial class SplitFileDialog : Form {
        // ダイアログ一覧処理の実装
        private DeleteExStartDialog.ListImpl m_listImpl;

        // 連番入力のダイアログ実装
        private RenameSelectedSequenceDialog.RenameSequenceDialogImpl m_dialogImpl;

        // 対象のファイル一覧
        private UIFileList m_targetFileList;

        // 対象のファイル一覧（マークファイル）
        private List<UIFile> m_markFileList;

        // 反対パスのファイル一覧
        private UIFileList m_oppositeFileList;

        // 入力された番号付けの情報
        private RenameNumberingInfo m_numberingInfo;
        
        // 入力された分割方法の情報
        private SplitFileInfo m_splitFileInfo;

        // 最大分割数
        private int m_maxSplitCount;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileList          対象パスのファイル一覧
        // 　　　　[in]oppositeFileList  反対パスのファイル一覧
        // 　　　　[in]numberingInfo     番号付けの情報
        // 　　　　[in]splitFileInfo     分割方法の情報
        // 戻り値：なし
        //=========================================================================================
        public SplitFileDialog(UIFileList fileList, UIFileList oppositeFileList, RenameNumberingInfo numberingInfo, SplitFileInfo splitFileInfo) {
            InitializeComponent();

            m_targetFileList = fileList;
            m_oppositeFileList = oppositeFileList;
            m_maxSplitCount = SplitFileInfo.GetMaxSplitCount(fileList.FileSystem.FileSystemId, oppositeFileList.FileSystem.FileSystemId);
            m_markFileList = fileList.MarkFilesExceptFolder;       // 事前チェック済みのため1件以上あるはず

            // 一覧を作成
            m_listImpl = new DeleteExStartDialog.ListImpl(this.listViewTarget, null, null);
            m_listImpl.InitializeByMarkFile(m_markFileList);
            this.listViewTarget.Items[0].Selected = true;

            // 転送先ファイル名
            this.textBoxFileName.Text = "_?";
            this.textBoxDestFolder.Text = oppositeFileList.DisplayDirectoryName;

            // 分割条件
            if (!splitFileInfo.SplitNumber) {
                this.radioButtonSize.Checked = true;
            } else {
                this.radioButtonNum.Checked = true;
            }
            this.numericSize.Minimum = 1;
            this.numericSize.Maximum = 1024L * 1024L * 1024L * 100L;
            this.numericSize.Value = splitFileInfo.SizeFileSize;
            this.comboBoxSizeUnit.Items.Add(Resources.DlgSplit_SizeUnitByte);
            this.comboBoxSizeUnit.Items.Add(Resources.DlgSplit_SizeUnitKiloByte);
            this.comboBoxSizeUnit.Items.Add(Resources.DlgSplit_SizeUnitMegaByte);
            this.comboBoxSizeUnit.SelectedIndex = SizeUnitSizeToIndex(splitFileInfo.SizeUnit);
            this.numericNum.Minimum = 1;
            this.numericNum.Maximum = m_maxSplitCount;
            this.numericNum.Value = splitFileInfo.CountFileCount;

            m_dialogImpl = new RenameSelectedSequenceDialog.RenameSequenceDialogImpl(this, numberingInfo, this.textBoxFileName, this.numericStart,
                        this.numericIncrease, this.comboBoxRadix, this.comboBoxWidth, this.textBoxSample,
                        new RenameSelectedSequenceDialog.RenameSequenceDialogImpl.GetSampleFileNameDelegate(GetSampleFileName));
            m_dialogImpl.UpdateSample();

            this.ActiveControl = this.numericSize;
        }
        
        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            bool enableSize = this.radioButtonSize.Checked;
            bool enableCount = this.radioButtonNum.Checked;

            this.numericSize.Enabled = enableSize;
            this.buttonFD.Enabled = enableSize;
            this.buttonCD650.Enabled = enableSize;
            this.buttonCD700.Enabled = enableSize;

            this.numericNum.Enabled = enableCount;
        }

        //=========================================================================================
        // 機　能：ファイルサイズの単位をコンボボックスのインデックスに変換する
        // 引　数：[in]sizeUnit    ファイルサイズの単位
        // 戻り値：コンボボックスのインデックス
        //=========================================================================================
        private int SizeUnitSizeToIndex(int sizeUnit) {
            if (sizeUnit == 1) {
                return 0;
            } else if (sizeUnit == 1024) {
                return 1;
            } else if (sizeUnit == 1024 * 1024) {
                return 2;
            } else {
                return 0;
            }
        }

        //=========================================================================================
        // 機　能：コンボボックスのインデックスをファイルサイズの単位に変換する
        // 引　数：[in]index    コンボボックスのインデックス
        // 戻り値：ファイルサイズの単位
        //=========================================================================================
        private int SizeUnitIndexToSize(int index) {
            if (index == 0) {
                return 1;
            } else if (index == 1) {
                return 1024;
            } else if (index == 2) {
                return 1024 * 1024;
            } else {
                return 1;
            }
        }

        //=========================================================================================
        // 機　能：選択中のファイル名を返す
        // 引　数：なし
        // 戻り値：選択中のファイル名（選択中ではないときnull）
        //=========================================================================================
        private string GetSelectedFileName() {
            if (this.listViewTarget.SelectedIndices.Count == 0) {
                return null;
            }
            int index = this.listViewTarget.SelectedIndices[0];
            string fileName = m_markFileList[index].FileName;
            return fileName;
        }

        //=========================================================================================
        // 機　能：一覧の選択中インデックスを返す
        // 引　数：なし
        // 戻り値：インデックス（選択されていないとき-1）
        //=========================================================================================
        private int GetListSelectedIndex() {
            if (this.listViewTarget.SelectedIndices.Count == 0) {
                return -1;
            }
            int index = this.listViewTarget.SelectedIndices[0];
            return index;
        }

        //=========================================================================================
        // 機　能：リストビューの項目の選択が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewTarget_SelectedIndexChanged(object sender, EventArgs evt) {
            m_dialogImpl.UpdateSample();
            UpdateSplitCountMessage();
        }

        //=========================================================================================
        // 機　能：サイズの数値が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void NumericSplit_ValueChanged(object sender, EventArgs evt) {
            UpdateSplitCountMessage();
        }

        //=========================================================================================
        // 機　能：サイズの単位が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ComboBoxSizeUnit_SelectedIndexChanged(object sender, EventArgs evt) {
            UpdateSplitCountMessage();
        }

        //=========================================================================================
        // 機　能：サイズの指定方法が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonSplitSize_CheckedChanged(object sender, EventArgs evt) {
            UpdateSplitCountMessage();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：FDボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonFD_Click(object sender, EventArgs evt) {
            this.numericSize.Value = SplitFileInfo.SPLIT_SIZE_FD;
            this.comboBoxSizeUnit.SelectedIndex = 0;
        }

        //=========================================================================================
        // 機　能：CD(650M)ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCD650_Click(object sender, EventArgs evt) {
            this.numericSize.Value = SplitFileInfo.SPLIT_SIZE_CD650;
            this.comboBoxSizeUnit.SelectedIndex = 0;
        }

        //=========================================================================================
        // 機　能：CD(700M)ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCD700_Click(object sender, EventArgs evt) {
            this.numericSize.Value = SplitFileInfo.SPLIT_SIZE_CD700;
            this.comboBoxSizeUnit.SelectedIndex = 0;
        }

        //=========================================================================================
        // 機　能：分割ファイル数のメッセージを更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateSplitCountMessage() {
            // 情報を取得
            long fileSize = -1;
            int splitCount = -1;
            int fileIndex = GetListSelectedIndex();
            if (fileIndex != -1) {
                fileSize = m_markFileList[fileIndex].FileSize;
                SplitFileInfo splitFileInfo = GetSplitFileInfo(true);
            
                // 分割数を計算
                if (splitFileInfo == null) {
                    splitCount = -1;
                } else {
                    long dummyOneFileSize;
                    bool success = splitFileInfo.GetOneFileSize(fileSize, m_targetFileList.FileSystem.FileSystemId, m_oppositeFileList.FileSystem.FileSystemId, out dummyOneFileSize, out splitCount);
                    if (!success) {
                        splitCount = -1;
                    }
                }
            }

            // メッセージを表示
            string strFileSize;
            if (fileSize == -1) {
                strFileSize = "---";
            } else {
                strFileSize = StringUtils.FileSizeToString(fileSize);
            }

            string strSplitCount;
            if (splitCount == -1) {
                strSplitCount = "---";
            } else {
                strSplitCount = splitCount.ToString();
            }

            string message = string.Format(Resources.DlgSplit_SplitMessage, strFileSize, strSplitCount);
            this.labelMessage.Text = message;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            // 入力値を取得
            m_numberingInfo = m_dialogImpl.GetUIValue(true);
            if (m_numberingInfo == null) {
                return;
            }

            m_splitFileInfo = GetSplitFileInfo(true);
            if (m_splitFileInfo == null) {
                return;
            }

            // 分割先の既存ファイルをチェック
            string existFile = CheckOppositeSplitFile();
            if (existFile != null) {
                DialogResult yesNo = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgSplit_OppositeExist, existFile);
                if (yesNo != DialogResult.Yes) {
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：転送先のファイル名に分割後のファイル名があるかどうかを調べる
        // 引　数：なし
        // 戻り値：分割後のファイル名があるとき、そのファイル名（ないときはnull）
        //=========================================================================================
        private string CheckOppositeSplitFile() {
            // 転送先ファイル名すべてをHashに入れる
            bool ignoreCase = FileSystemID.IgnoreCaseFolderPath(m_oppositeFileList.FileSystem.FileSystemId);
            HashSet<string> oppositeFileSet = new HashSet<string>();
            for (int j = 0; j < m_oppositeFileList.Files.Count; j++) {
                string oppositeFile = m_oppositeFileList.Files[j].FileName;
                if (ignoreCase) {
                    oppositeFileSet.Add(oppositeFile.ToLower());
                } else {
                    oppositeFileSet.Add(oppositeFile);
                }
            }

            // 転送元の分割をシミュレーション
            FileSystemID targetFileSystem = m_targetFileList.FileSystem.FileSystemId;
            FileSystemID oppositeFileSystem = m_oppositeFileList.FileSystem.FileSystemId;
            for (int i = 0; i < m_markFileList.Count; i++) {
                long dummyOneFileSize;
                int fileCount;
                m_splitFileInfo.GetOneFileSize(m_markFileList[i].FileSize, targetFileSystem, oppositeFileSystem, out dummyOneFileSize, out fileCount);

                ModifyFileInfoContext modifyCtx = new ModifyFileInfoContext();
                string destFileTemplate = GenericFileStringUtils.GetFileName(m_markFileList[i].FileName);
                for (int j = 0; j < fileCount; j++) {
                    string destFileName = RenameNumberingInfo.CreateSequenceFileName(destFileTemplate, m_numberingInfo, modifyCtx);
                    if (ignoreCase) {
                        destFileName = destFileName.ToLower();
                    }
                    if (oppositeFileSet.Contains(destFileName)) {
                        return m_markFileList[i].FileName;
                    }
                }
            }
            return null;
        }

        //=========================================================================================
        // 機　能：ファイル分割の情報を取得する
        // 引　数：[in]displayMessage  メッセージを表示するときtrue
        // 戻り値：ファイル分割の情報（エラーのときnull）
        //=========================================================================================
        private SplitFileInfo GetSplitFileInfo(bool displayMessage) {
            SplitFileInfo splitFileInfo = SplitFileInfo.DefaultUI();
            if (this.radioButtonSize.Checked) {
                // サイズ
                splitFileInfo.SplitNumber = false;
                splitFileInfo.SizeFileSize = (long)this.numericSize.Value;
                splitFileInfo.SizeUnit = SizeUnitIndexToSize(this.comboBoxSizeUnit.SelectedIndex);
                long fileSize = splitFileInfo.GetSplitSize();
                for (int i = 0; i < m_markFileList.Count; i++) {
                    int maxSplitCount = SplitFileInfo.GetMaxSplitCount(m_targetFileList.FileSystem.FileSystemId, m_oppositeFileList.FileSystem.FileSystemId);
                    if (m_markFileList[i].FileSize / fileSize > maxSplitCount) {
                        if (displayMessage) {
                            InfoBox.Warning(this, Resources.DlgSplit_TooManySplitCount, m_markFileList[i].FileName, maxSplitCount);
                            return null;
                        }
                    }
                }
            } else {
                // 個数
                splitFileInfo.SplitNumber = true;
                splitFileInfo.CountFileCount = (int)(this.numericNum.Value);
            }
            return splitFileInfo;
        }

        //=========================================================================================
        // 機　能：サンプル表示用のファイル名を返す
        // 引　数：[in]numberingInfo  番号付けの情報
        // 　　　　[in]modifyCtx      連番のための情報
        // 戻り値：サンプル表示用のファイル1件(表示できないときnull)
        //=========================================================================================
        private string GetSampleFileName(RenameNumberingInfo numberingInfo, ModifyFileInfoContext modifyCtx) {
            string srcFileName = GetSelectedFileName();
            if (srcFileName == null) {
                return null;
            }
            string fileName = RenameNumberingInfo.CreateSequenceFileName(srcFileName, numberingInfo, modifyCtx);
            return fileName;
        }

        //=========================================================================================
        // プロパティ：対象のファイル一覧
        //=========================================================================================
        public List<UIFile> FileList {
            get {
                return m_markFileList;
            }
        }

        //=========================================================================================
        // プロパティ：入力された結合先ファイル名
        //=========================================================================================
        public RenameNumberingInfo ResultNumberingInfo {
            get {
                return m_numberingInfo;
            }
        }

        //=========================================================================================
        // プロパティ：入力された分割方法の情報
        //=========================================================================================
        public SplitFileInfo ResultSplitFileInfo {
            get {
                return m_splitFileInfo;
            }
        }
    }
}
