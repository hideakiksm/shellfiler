using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.SSH;
using ShellFiler.Document.OSSpec;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：圧縮ダイアログ
    //=========================================================================================
    public partial class ArchiveDialog : Form {
        // アーカイブ名の元になるファイル名（file1.zipに対するfile1.txt）
        private string m_archiveBaseFileName;

        // デフォルトの圧縮設定（各実行形式/各形式が切り替わったときの状態を保存）
        private ArchiveSetting m_archiveSetting;

        // ダイアログでの入力結果
        private ArchiveParameter m_resultArchiveParameter;

        // ローカル7z圧縮ページ（はじめに参照されるまではnull）
        private SevenZipPage m_sevenZipPage;

        // リモート圧縮ページ（はじめに参照されるまではnull）
        private RemoteShellPage m_remoteShellPage;

        // リモートのファイルに書き込むときtrue
        private bool m_isRemote;

        // ファイル一覧のコンテキスト情報
        private IFileListContext m_fileListContext;

        // 反対パスのファイル一覧
        private List<UIFile> m_oppositeList;

        // 表示中のページ
        private IArchiveDialogPage m_activePage;

        // コマンド作成クラス（リモート実行できないときはnull）
        private ShellCommandDictionary m_shellCommandDictionary = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting         デフォルトの圧縮設定
        // 　　　　[in]baseFileName    アーカイブ名の元になるファイル名（file1.zipに対するfile1.txt）
        // 　　　　[in]targetPath      作成先の反対パス
        // 　　　　[in]isRemote        リモートファイルに書き込むときtrue
        // 　　　　[in]fileListContext ファイル一覧のコンテキスト情報
        // 　　　　[in]oppositeList    反対パスのファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public ArchiveDialog(ArchiveSetting setting, string baseFileName, string targetPath, bool isRemote, IFileListContext fileListContext, List<UIFile> oppositeList) {
            InitializeComponent();
            m_archiveBaseFileName = baseFileName;
            m_archiveSetting = setting;
            m_isRemote = isRemote;
            m_fileListContext = fileListContext;
            m_oppositeList = oppositeList;

            // リモート実行できるかどうかを確認
            if (m_isRemote) {
                m_shellCommandDictionary = SSHUtils.GetShellCommandDictionary(m_fileListContext);
                string zip = m_shellCommandDictionary.GetCommandArchiveZip(false, 9);
                string tgz = m_shellCommandDictionary.GetCommandArchiveTarGz();
                string tbz2 = m_shellCommandDictionary.GetCommandArchiveTarBz2();
                string tar = m_shellCommandDictionary.GetCommandArchiveTar();
                if (zip == null && tgz == null && tbz2 == null && tar == null) {
                    m_isRemote = false;
                }
            }

            // ローカルのみの場合は必ずsevenzip.dllで実行
            if (!m_isRemote) {
                this.tabPageLocal7z.Text = Resources.DlgArchive_ExtractOption;
                this.tabControl.TabPages.RemoveAt(1);
                m_archiveSetting.ExecuteMetohd = ArchiveExecuteMethod.Local7z;
            }
          
            // ページを初期化
            if (m_archiveSetting.ExecuteMetohd == ArchiveExecuteMethod.Local7z) {
                InitializePage(this.tabPageLocal7z);
                this.tabControl.SelectedTab = this.tabPageLocal7z;
            } else {
                InitializePage(this.tabPageRemote);
                this.tabControl.SelectedTab = this.tabPageRemote;
            }

            this.textBoxDirectory.Text = targetPath;
            this.textBoxArcFileName.Text = ModifyFileNameFromType(m_archiveBaseFileName, m_archiveSetting.ArchiveType);
            this.ActiveControl = this.textBoxArcFileName;
        }
        
        //=========================================================================================
        // 機　能：タブページが変更されたようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void tabControl_Selecting(object sender, TabControlCancelEventArgs evt) {
            if (m_activePage != null) {
                bool success = m_activePage.OnPageDeactivated();
                if (!success) {
                    evt.Cancel = true;
                }
            }
        }
        
        //=========================================================================================
        // 機　能：タブページが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void tabControl_SelectedIndexChanged(object sender, EventArgs evt) {
            InitializePage(this.tabControl.SelectedTab);
        }

        //=========================================================================================
        // 機　能：ページを初期化する
        // 引　数：[in]newPage  新しく選択するページ
        // 戻り値：なし
        //=========================================================================================
        private void InitializePage(TabPage newPage) {
            if (newPage == this.tabPageLocal7z) {
                if (m_sevenZipPage == null) {
                    m_sevenZipPage = new SevenZipPage(this);
                    m_activePage = m_sevenZipPage;
                    m_activePage.Initialize();
                } else {
                    m_activePage = m_sevenZipPage;
                    m_activePage.OnPageActivated();
                }
            } else if (newPage == this.tabPageRemote) {
                if (m_remoteShellPage == null) {
                    m_remoteShellPage = new RemoteShellPage(this, m_shellCommandDictionary);
                    m_activePage = m_remoteShellPage;
                    m_activePage.Initialize();
                } else {
                    m_activePage = m_remoteShellPage;
                    m_activePage.OnPageActivated();
                }
            }

        }

        //=========================================================================================
        // 機　能：ファイル名が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void textBoxArcFileName_TextChanged(object sender, EventArgs evt) {
            m_activePage.OnFileNameTextChanged();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================        
        private void buttonOk_Click(object sender, EventArgs evt) {
            bool success = m_activePage.OnOkClick();
            if (!success) {
                m_resultArchiveParameter = null;
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ArchiveDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && m_resultArchiveParameter == null) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：ファイル名を指定された圧縮形式にあわせて修正する
        // 引　数：[in]orgFile      元のファイル名
        // 　　　　[in]archiveType  圧縮形式
        // 戻り値：指定された圧縮方法に合わせたファイル名
        //=========================================================================================
        private static string ModifyFileNameFromType(string orgName, ArchiveType archiveType) {
            string arcName;
            string orgNameLower = orgName.ToLower();
            if (orgNameLower.EndsWith(".tar.gz")) {
                arcName = orgName.Substring(0, orgName.Length - 7) + archiveType.Extension;
            } else if (orgNameLower.EndsWith(".tar.bz2")) {
                arcName = orgName.Substring(0, orgName.Length - 8) + archiveType.Extension;
            } else {
                string body = GenericFileStringUtils.GetFileNameBody(orgName);
                if (body != "") {
                    arcName = body + archiveType.Extension;
                } else {
                    arcName = orgName + archiveType.Extension;
                }
            }
            return arcName;
        }

        //=========================================================================================
        // 機　能：ファイル名の拡張子から圧縮形式を取得する
        // 引　数：[in]fileName   ファイル名
        // 　　　　[in]method     圧縮の実行形式
        // 戻り値：圧縮形式（認識できないときtrue）
        //=========================================================================================
        private static ArchiveType FileNameToArchiveType(string fileName, ArchiveExecuteMethod method) {
            fileName = fileName.ToLower();
            if (fileName.EndsWith(".zip")) {
                return ArchiveType.Zip;
            } else if (fileName.EndsWith(".7z") && method == ArchiveExecuteMethod.Local7z) {
                return ArchiveType.SevenZip;
            } else if (fileName.EndsWith(".tar.gz")) {
                return ArchiveType.TarGz;
            } else if (fileName.EndsWith(".tar.bz2")) {
                return ArchiveType.TarBz2;
            } else if (fileName.EndsWith(".tar")) {
                return ArchiveType.Tar;
            } else {
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：ダイアログでの入力結果
        //=========================================================================================
        public ArchiveParameter ResultArchiveParameter {
            get {
                return m_resultArchiveParameter;
            }
        }
        
        //=========================================================================================
        // プロパティ：圧縮のオプションの編集結果（各実行形式/各形式が切り替わったときの状態を保存）
        //=========================================================================================
        public ArchiveSetting ArchiveSetting {
            get {
                return m_archiveSetting;
            }
        }

        //=========================================================================================
        // インターフェース：アーカイブ設定の実装ページ
        //=========================================================================================
        private interface IArchiveDialogPage {

            //=========================================================================================
            // 機　能：オプションを初期化する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            void Initialize();

            //=========================================================================================
            // 機　能：ファイル名が変更されたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            void OnFileNameTextChanged();

            //=========================================================================================
            // 機　能：ページがアクティブになったときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================        
            void OnPageActivated();

            //=========================================================================================
            // 機　能：ページがアクティブでなくなったときの処理を行う
            // 引　数：なし
            // 戻り値：設定を正常に取得できたときtrue
            //=========================================================================================        
            bool OnPageDeactivated();

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：設定を正常に取得できたときtrue
            //=========================================================================================        
            bool OnOkClick();
        }

        //=========================================================================================
        // クラス：7z.dllによるローカルPCでの圧縮ページの実装
        //=========================================================================================
        private class SevenZipPage: IArchiveDialogPage {
            // 親ダイアログ
            private ArchiveDialog m_parent;

            // ダイアログ項目をプログラムで更新中のときtrue
            private bool m_updateDialogItem;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親ダイアログ
            // 戻り値：なし
            //=========================================================================================
            public SevenZipPage(ArchiveDialog parent) {
                m_parent = parent;
            }

            //=========================================================================================
            // 機　能：オプションを初期化する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Initialize() {
                m_parent.m_archiveSetting.ArchiveType = ArchiveType.ModifyArchiveType(m_parent.m_archiveSetting.ArchiveType, null);
                ArchiveTypeToUi(m_parent.m_archiveSetting.ArchiveType);
                InitializeOption(m_parent.m_archiveSetting.ArchiveType);

                m_parent.radioButtonL7z.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                m_parent.radioButtonLZip.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                m_parent.radioButtonLTgz.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                m_parent.radioButtonLTbz2.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                m_parent.radioButtonLTar.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                m_parent.checkBoxLEncrypt.CheckedChanged += new EventHandler(CheckBoxEncrypt_CheckedChanged);
                m_parent.comboBoxLEncMethod.SelectedValueChanged += new EventHandler(ComboBoxLEncMethod_SelectedValueChanged);
                m_parent.buttonPasswordLManage.Click += new EventHandler(ButtonPasswordManage_Click);
                m_parent.buttonLRecommend.Click += new EventHandler(ButtonRecommend_Click);
                m_parent.panelLWarning.Paint += new PaintEventHandler(PanelLWarning_Paint);
            }

            //=========================================================================================
            // 機　能：m_parent.m_archiveSettingをUIに反映する
            // 引　数：[in]type  設定する圧縮ファイルフォーマット
            // 戻り値：なし
            //=========================================================================================
            private void InitializeOption(ArchiveType type) {
                SevenZipArchiveFeature feature = SevenZipArchiveFeature.GetFeature(type);
                ArchiveSettingLocal7zOption option = m_parent.m_archiveSetting.GetCurrentLocal7zOption();

                // 更新時刻を合わせる
                m_parent.checkBoxLTimeStamp.Checked = option.ModifyTimestamp;

                // 圧縮アルゴリズム
                if (feature.SupportMethod == null) {
                    m_parent.comboBoxLMethod.Items.Clear();
                    m_parent.comboBoxLMethod.Items.Add(Resources.DlgArchive_ItemDefault);
                    m_parent.comboBoxLMethod.SelectedIndex = 0;
                    m_parent.comboBoxLMethod.Enabled = false;
                } else {
                    m_parent.comboBoxLMethod.Items.Clear();
                    m_parent.comboBoxLMethod.Items.AddRange(feature.SupportMethod);
                    m_parent.comboBoxLMethod.SelectedIndex = StringUtils.GetStringArrayIndex(feature.SupportMethod, option.CompressionMethod);
                    m_parent.comboBoxLMethod.Enabled = true;
                }

                // 圧縮レベル
                if (feature.SupportCompressionLevel) {
                    m_parent.trackBarLLevel.Value = option.CompressionLevel;
                    m_parent.trackBarLLevel.Enabled = true;
                } else {
                    m_parent.trackBarLLevel.Value = 9;
                    m_parent.trackBarLLevel.Enabled = false;
                }

                // 暗号化
                if (feature.SupportEncryption) {
                    m_parent.checkBoxLEncrypt.Checked = option.Encryption;
                    m_parent.checkBoxLEncrypt.Enabled = true;
                    InitializePasswordList(option.PasswordItem);
                    if (feature.SupportEncryptionMethod == null) {
                        m_parent.comboBoxLEncMethod.Items.Clear();
                        m_parent.comboBoxLEncMethod.Items.Add(Resources.DlgArchive_ItemDefault);
                        m_parent.comboBoxLEncMethod.SelectedIndex = 0;
                    } else {
                        m_parent.comboBoxLEncMethod.Items.Clear();
                        m_parent.comboBoxLEncMethod.Items.AddRange(feature.SupportEncryptionMethod);
                        m_parent.comboBoxLEncMethod.SelectedIndex = StringUtils.GetStringArrayIndex(feature.SupportEncryptionMethod, option.EncryptionMethod);
                    }
                } else {
                    m_parent.checkBoxLEncrypt.Checked = false;
                    m_parent.checkBoxLEncrypt.Enabled = false;
                    m_parent.comboBoxLPassword.Items.Clear();
                    m_parent.comboBoxLEncMethod.Items.Clear();
                }
                CheckBoxEncrypt_CheckedChanged(null, null);
            }

            //=========================================================================================
            // 機　能：パスワード一覧を初期化する
            // 引　数：[in]passwordItem  パスワードに設定する値（null:パスワード入力しないため先頭を選択）
            // 戻り値：なし
            //=========================================================================================
            private void InitializePasswordList(ArchiveAutoPasswordItem passwordItem) {
                m_parent.comboBoxLPassword.Items.Clear();
                m_parent.comboBoxLPassword.Items.Add(Resources.DlgArchive_PasswordInput);            // index=0:手入力
                List<ArchiveAutoPasswordItem> passwordList = Program.Document.UserSetting.ArchiveAutoPasswordSetting.AutoPasswordItemList;
                int selectedIndex = 0;
                for (int i = 0; i < passwordList.Count; i++) {
                    ArchiveAutoPasswordItem password = passwordList[i];
                    m_parent.comboBoxLPassword.Items.Add(password);                                  // index=1～:設定済み
                    if (passwordItem != null && passwordItem.DisplayName == password.DisplayName && passwordItem.Password == password.Password) {
                        selectedIndex = i + 1;
                    }
                }
                m_parent.comboBoxLPassword.SelectedIndex = selectedIndex;                            // なければ手入力
            }

            //=========================================================================================
            // 機　能：UIからオプション設定を取得して、m_parent.m_archiveSettingに設定する
            // 引　数：[in]type   取得する圧縮ファイル形式
            // 戻り値：なし
            //=========================================================================================
            private void RetrieveOptionFromUi(ArchiveType type) {
                SevenZipArchiveFeature feature = SevenZipArchiveFeature.GetFeature(type);
                ArchiveSettingLocal7zOption option = m_parent.m_archiveSetting.GetCurrentLocal7zOption();

                // 更新時刻を合わせる
                option.ModifyTimestamp = m_parent.checkBoxLTimeStamp.Checked;

                // 圧縮アルゴリズム
                if (feature.SupportMethod == null) {
                    option.CompressionMethod = null;
                } else {
                    int index = m_parent.comboBoxLMethod.SelectedIndex;
                    option.CompressionMethod = feature.SupportMethod[index];
                }

                // 圧縮レベル
                if (feature.SupportCompressionLevel) {
                    option.CompressionLevel = m_parent.trackBarLLevel.Value;
                } else {
                    option.CompressionLevel = -1;
                }

                // 暗号化
                if (feature.SupportEncryption) {
                    if (m_parent.checkBoxLEncrypt.Checked) {
                        option.Encryption = true;
                        ArchiveAutoPasswordItem pwdItem = null;
                        if (m_parent.comboBoxLPassword.SelectedItem is ArchiveAutoPasswordItem) {
                            pwdItem = (ArchiveAutoPasswordItem)(m_parent.comboBoxLPassword.SelectedItem);
                        } else {
                            pwdItem = null;
                        }
                        option.PasswordItem = pwdItem;
                        if (feature.SupportEncryptionMethod == null) {
                            option.EncryptionMethod = null;
                        } else {
                            option.EncryptionMethod = feature.SupportEncryptionMethod[m_parent.comboBoxLEncMethod.SelectedIndex];
                        }
                    } else {
                        option.PasswordItem = null;
                        option.Encryption = false;
                        option.EncryptionMethod = null;
                    }
                } else {
                    option.PasswordItem = null;
                    option.Encryption = false;
                    option.EncryptionMethod = null;
                }
            }

            //=========================================================================================
            // 機　能：ファイル名が変更されたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnFileNameTextChanged() {
                string fileName = m_parent.textBoxArcFileName.Text.ToLower();
                ArchiveType archiveType = FileNameToArchiveType(fileName, ArchiveExecuteMethod.Local7z);
                if (!m_updateDialogItem) {
                    m_updateDialogItem = true;
                    try {
                        ArchiveTypeToUi(archiveType);
                    } finally {
                        m_updateDialogItem = false;
                    }
                }
            }

            //=========================================================================================
            // 機　能：ページがアクティブになったときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================        
            public void OnPageActivated() {
                m_parent.m_archiveSetting.ExecuteMetohd = ArchiveExecuteMethod.Local7z;
                string arcFileName = m_parent.textBoxArcFileName.Text;
                ArchiveType type = FileNameToArchiveType(arcFileName, ArchiveExecuteMethod.Local7z);
                ArchiveTypeToUi(type);
                m_parent.m_archiveSetting.ArchiveType = UiToArchiveType();
            }

            //=========================================================================================
            // 機　能：ページがアクティブでなくなったときの処理を行う
            // 引　数：なし
            // 戻り値：設定を正常に取得できたときtrue
            //=========================================================================================        
            public bool OnPageDeactivated() {
                // オプションを取得
                ArchiveType type = UiToArchiveType();
                m_parent.m_archiveSetting.ArchiveType = type;
                RetrieveOptionFromUi(type);
                return true;
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：設定を正常に取得できたときtrue
            //=========================================================================================        
            public bool OnOkClick() {
                // 入力されている値を取得
                bool success = OnPageDeactivated();
                if (!success) {
                    return false;
                }
                ArchiveType type = m_parent.m_archiveSetting.ArchiveType;
                ArchiveSettingLocal7zOption sevenOpt = (ArchiveSettingLocal7zOption)(m_parent.m_archiveSetting.GetCurrentLocal7zOption());

                // ファイル名を取得
                string arcName = m_parent.textBoxArcFileName.Text;
                if (arcName == "" || arcName.IndexOf('/') >= 0 || arcName.IndexOf('\\') >= 0) {
                    InfoBox.Warning(m_parent, Resources.DlgArchive_FileName);
                    return false;
                }
                arcName = ArchiveDialog.ModifyFileNameFromType(arcName, type);

                // パスワードが未設定なら設定
                if (sevenOpt.Encryption && sevenOpt.PasswordItem == null) {
                    ArchivePasswordNewDialog passwordDialog = new ArchivePasswordNewDialog();
                    DialogResult passResult = passwordDialog.ShowDialog(m_parent);
                    if (passResult != DialogResult.OK) {
                        return false;
                    }
                    sevenOpt.PasswordItem = new ArchiveAutoPasswordItem.SpecifiedPassword(passwordDialog.Password);
                }

                // 今回の圧縮パラメータを作成
                m_parent.m_resultArchiveParameter = new ArchiveParameter(arcName, type, sevenOpt);
                return true;
            }

            //=========================================================================================
            // 機　能：圧縮形式をダイアログのラジオボタンに反映する
            // 引　数：[in]archiveType  圧縮形式（null:変更しない）
            // 戻り値：なし
            //=========================================================================================
            private void ArchiveTypeToUi(ArchiveType archiveType) {
                if (archiveType == ArchiveType.Zip) {
                    m_parent.radioButtonLZip.Checked = true;
                } else if (archiveType == ArchiveType.SevenZip) {
                    m_parent.radioButtonL7z.Checked = true;
                } else if (archiveType == ArchiveType.TarGz) {
                    m_parent.radioButtonLTgz.Checked = true;
                } else if (archiveType == ArchiveType.TarBz2) {
                    m_parent.radioButtonLTbz2.Checked = true;
                } else if (archiveType == ArchiveType.Tar) {
                    m_parent.radioButtonLTar.Checked = true;
                }
            }

            //=========================================================================================
            // 機　能：ダイアログのラジオボタンから圧縮形式を取得する
            // 引　数：なし
            // 戻り値：圧縮形式
            //=========================================================================================
            private ArchiveType UiToArchiveType() {
                if (m_parent.radioButtonLZip.Checked) {
                    return ArchiveType.Zip;
                } else if (m_parent.radioButtonL7z.Checked) {
                    return ArchiveType.SevenZip;
                } else if (m_parent.radioButtonLTgz.Checked) {
                    return ArchiveType.TarGz;
                } else if (m_parent.radioButtonLTbz2.Checked) {
                    return ArchiveType.TarBz2;
                } else {
                    return ArchiveType.Tar;
                }
            }

            //=========================================================================================
            // 機　能：圧縮形式のラジオボタンの選択項目が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void RadioButton_CheckedChanged(object sender, EventArgs evt) {
                ArchiveType oldType = m_parent.m_archiveSetting.ArchiveType;
                RetrieveOptionFromUi(oldType);
                ArchiveType newType = UiToArchiveType();

                // 圧縮形式→ファイル名
                if (!m_updateDialogItem) {
                    m_updateDialogItem = true;
                    try {
                        string fileName = ArchiveDialog.ModifyFileNameFromType(m_parent.textBoxArcFileName.Text, newType);
                        m_parent.textBoxArcFileName.Text = fileName;
                    } finally {
                        m_updateDialogItem = false;
                    }
                }

                // オプションをUIに反映
                m_parent.panelLWarning.Invalidate();
                m_parent.m_archiveSetting.ArchiveType = newType;
                InitializeOption(newType);
            }

            //=========================================================================================
            // 機　能：暗号化のON/OFFが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void CheckBoxEncrypt_CheckedChanged(object sender, EventArgs evt) {
                SevenZipArchiveFeature feature = SevenZipArchiveFeature.GetFeature(m_parent.m_archiveSetting.ArchiveType);
                if (m_parent.checkBoxLEncrypt.Checked) {
                    m_parent.comboBoxLPassword.Enabled = true;
                    m_parent.buttonPasswordLManage.Enabled = true;
                    if (feature.SupportEncryptionMethod == null) {
                        m_parent.comboBoxLEncMethod.Enabled = false;
                    } else {
                        m_parent.comboBoxLEncMethod.Enabled = true;
                    }
                } else {
                    m_parent.comboBoxLPassword.Enabled = false;
                    m_parent.buttonPasswordLManage.Enabled = false;
                    m_parent.comboBoxLEncMethod.Enabled = false;
                }
                m_parent.panelLWarning.Invalidate();
            }

            //=========================================================================================
            // 機　能：暗号化アルゴリズムの項目が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void ComboBoxLEncMethod_SelectedValueChanged(object sender, EventArgs evt) {
                m_parent.panelLWarning.Invalidate();
            }

            //=========================================================================================
            // 機　能：警告パネルの再描画イベントを処理する
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void PanelLWarning_Paint(object sender, PaintEventArgs evt) {
                ArchiveType type = UiToArchiveType();
                if (type == ArchiveType.Zip && m_parent.checkBoxLEncrypt.Checked) {
                    Graphics g = evt.Graphics;
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    string errorString;
                    if (m_parent.comboBoxLEncMethod.SelectedIndex == 0) {
                        errorString = Resources.DlgArchive_EncryptWarningAes;
                    } else {
                        errorString = Resources.DlgArchive_EncryptWarningZipcrypt;
                    }
                    Brush backBrush = new SolidBrush(Configuration.Current.DialogWarningBackColor);
                    Brush textBrush = new SolidBrush(Configuration.Current.DialogWarningTextColor);
                    try {
                        g.FillRectangle(backBrush, m_parent.panelLWarning.ClientRectangle);
                        g.DrawString(errorString, SystemFonts.DefaultFont, textBrush, m_parent.panelLWarning.ClientRectangle, sf);
                    } finally {
                        backBrush.Dispose();
                        textBrush.Dispose();
                        sf.Dispose();
                    }
                }
            }

            //=========================================================================================
            // 機　能：推奨ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void ButtonRecommend_Click(object sender, EventArgs evt) {
                ArchiveType type = UiToArchiveType();
                SevenZipArchiveFeature feature = SevenZipArchiveFeature.GetFeature(type);
                ArchiveSettingLocal7zOption recOption = feature.RecommendedSetting;
                if (recOption.CompressionMethod != null) {
                    m_parent.comboBoxLMethod.SelectedIndex = StringUtils.GetStringArrayIndex(feature.SupportMethod, recOption.CompressionMethod);
                }
                m_parent.trackBarLLevel.Value = recOption.CompressionLevel;
                if (recOption.EncryptionMethod != null) {
                    m_parent.comboBoxLEncMethod.SelectedIndex = StringUtils.GetStringArrayIndex(feature.SupportEncryptionMethod, recOption.EncryptionMethod);
                }
            }

            //=========================================================================================
            // 機　能：パスワードの管理ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void ButtonPasswordManage_Click(object sender, EventArgs evt) {
                // 現在の入力値を取得
                List<ArchiveAutoPasswordItem> passwordList = Program.Document.UserSetting.ArchiveAutoPasswordSetting.AutoPasswordItemList;
                ArchiveAutoPasswordItem passwordItem;
                int selectedIndex = m_parent.comboBoxLPassword.SelectedIndex;
                if (selectedIndex > 0) {
                    passwordItem = passwordList[selectedIndex - 1];
                } else {
                    passwordItem = null;
                }
                
                // 管理画面で一覧を編集
                ArchivePasswordManageDialog manageDialog = new ArchivePasswordManageDialog();
                manageDialog.ShowDialog(m_parent);

                // UIを差し替え
                ArchiveSettingLocal7zOption option = m_parent.m_archiveSetting.GetCurrentLocal7zOption();
                InitializePasswordList(passwordItem);
            }
        }

        //=========================================================================================
        // クラス：リモートでの圧縮ページの実装
        //=========================================================================================
        private class RemoteShellPage: IArchiveDialogPage {
            // 親ダイアログ
            private ArchiveDialog m_parent;

            // ダイアログ項目をプログラムで更新中のときtrue
            private bool m_updateDialogItem;
            
            // コマンド作成クラス
            private ShellCommandDictionary m_shellCommandDictionary;

            // サポートするアーカイブの種類
            private List<ArchiveType> m_supportArchiveType = new List<ArchiveType>();

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent      親ダイアログ
            // 　　　　[in]commandDic  コマンド作成クラス
            // 戻り値：なし
            //=========================================================================================
            public RemoteShellPage(ArchiveDialog parent, ShellCommandDictionary commandDic) {
                m_parent = parent;
                m_shellCommandDictionary = commandDic;
                if (m_shellCommandDictionary.GetCommandArchiveZip(false, 9) != null) {
                    m_supportArchiveType.Add(ArchiveType.Zip);
                }
                if (m_shellCommandDictionary.GetCommandArchiveTarGz() != null) {
                    m_supportArchiveType.Add(ArchiveType.TarGz);
                }
                if (m_shellCommandDictionary.GetCommandArchiveTarBz2() != null) {
                    m_supportArchiveType.Add(ArchiveType.TarBz2);
                }
                if (m_shellCommandDictionary.GetCommandArchiveTar() != null) {
                    m_supportArchiveType.Add(ArchiveType.Tar);
                }
            }

            //=========================================================================================
            // 機　能：オプションを初期化する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Initialize() {
                m_parent.m_archiveSetting.ArchiveType = ArchiveType.ModifyArchiveType(m_parent.m_archiveSetting.ArchiveType, m_supportArchiveType);
                ArchiveTypeToUi(m_parent.m_archiveSetting.ArchiveType);
                InitializeOption(m_parent.m_archiveSetting.ArchiveType);

                m_parent.radioButtonRZip.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                m_parent.radioButtonRTgz.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                m_parent.radioButtonRTbz2.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                m_parent.radioButtonRTar.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                m_parent.checkBoxRTimeStamp.CheckedChanged += new EventHandler(CheckBoxRTimeStamp_CheckedChanged);
                m_parent.trackBarRLevel.ValueChanged += new EventHandler(TrackBarRLevel_ValueChanged);
                m_parent.buttonRRecommend.Click += new EventHandler(ButtonRecommend_Click);
            }

            //=========================================================================================
            // 機　能：m_parent.m_archiveSettingをUIに反映する
            // 引　数：[in]type  設定する圧縮ファイルフォーマット
            // 戻り値：なし
            //=========================================================================================
            private void InitializeOption(ArchiveType type) {
                RemoteShellArchiveFeature feature = RemoteShellArchiveFeature.GetFeature(type);
                ArchiveSettingRemoteShellOption option = m_parent.m_archiveSetting.GetCurrentRemoteShellOption();

                // 有効/無効の切り替え
                if (!m_supportArchiveType.Contains(ArchiveType.Zip)) {
                    m_parent.radioButtonRZip.Enabled = false;
                }
                if (!m_supportArchiveType.Contains(ArchiveType.TarGz)) {
                    m_parent.radioButtonRTgz.Enabled = false;
                }
                if (!m_supportArchiveType.Contains(ArchiveType.TarBz2)) {
                    m_parent.radioButtonRTbz2.Enabled = false;
                }
                if (!m_supportArchiveType.Contains(ArchiveType.Tar)) {
                    m_parent.radioButtonRTar.Enabled = false;
                }

                // 更新時刻を合わせる
                if (feature.SupportTimestamp) {
                    m_parent.checkBoxRTimeStamp.Enabled = true;
                    m_parent.checkBoxRTimeStamp.Checked = option.ModifyTimestamp;
                } else {
                    m_parent.checkBoxRTimeStamp.Enabled = false;
                    m_parent.checkBoxRTimeStamp.Checked = false;
                }

                // 圧縮レベル
                if (feature.SupportCompressionLevel) {
                    m_parent.trackBarRLevel.Value = option.CompressionLevel;
                    m_parent.trackBarRLevel.Enabled = true;
                } else {
                    m_parent.trackBarRLevel.Value = 9;
                    m_parent.trackBarRLevel.Enabled = false;
                }

                // コマンド
                ResetCommand();

                // 推奨設定
                if (feature.SupportRecommend) {
                    m_parent.buttonRRecommend.Enabled = true;
                } else {
                    m_parent.buttonRRecommend.Enabled = false;
                }
            }

            //=========================================================================================
            // 機　能：コマンド関連のUIをリセットする
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void ResetCommand() {
                string command;
                List<OSSpecLineExpect> expect;
                CreateCommand(out command, out expect);

                string program, argument;
                GenericFileStringUtils.SplitCommandLine(command, out program, out argument);

                m_parent.labelRCommand.Text = program;
                m_parent.textBoxRCommand.Text = argument;
            }

            //=========================================================================================
            // 機　能：ファイル名が変更されたときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnFileNameTextChanged() {
                string fileName = m_parent.textBoxArcFileName.Text.ToLower();
                ArchiveType archiveType = ArchiveDialog.FileNameToArchiveType(fileName, ArchiveExecuteMethod.RemoteShell);
                if (!m_updateDialogItem) {
                    m_updateDialogItem = true;
                    try {
                        ArchiveTypeToUi(archiveType);
                    } finally {
                        m_updateDialogItem = false;
                    }
                }
            }

            //=========================================================================================
            // 機　能：ページがアクティブになったときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================        
            public void OnPageActivated() {
                m_parent.m_archiveSetting.ExecuteMetohd = ArchiveExecuteMethod.RemoteShell;
                string arcFileName = m_parent.textBoxArcFileName.Text;
                ArchiveType type = FileNameToArchiveType(arcFileName, ArchiveExecuteMethod.RemoteShell);
                ArchiveTypeToUi(type);

                // 7zが選択された状態でページ変更した場合は補正
                ArchiveType newType = UiToArchiveType();
                if (type != m_parent.ArchiveSetting.ArchiveType) {
                    m_parent.m_archiveSetting.ArchiveType = newType;
                    m_updateDialogItem = true;
                    try {
                        m_parent.textBoxArcFileName.Text = ArchiveDialog.ModifyFileNameFromType(m_parent.textBoxArcFileName.Text, newType);
                    } finally {
                        m_updateDialogItem = false;
                    }
                }
            }

            //=========================================================================================
            // 機　能：ページがアクティブでなくなったときの処理を行う
            // 引　数：なし
            // 戻り値：設定を正常に取得できたときtrue
            //=========================================================================================        
            public bool OnPageDeactivated() {
                // コマンドラインを確認
                if (m_parent.textBoxRCommand.Text.Trim() == "") {
                    InfoBox.Warning(m_parent, Resources.DlgArchive_RemoteCommand);
                    return false;
                }

                // オプションを取得
                ArchiveType type = UiToArchiveType();
                m_parent.m_archiveSetting.ArchiveType = type;
                RetrieveOptionFromUi(type);
                return true;
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：設定を正常に取得できたときtrue
            //=========================================================================================        
            public bool OnOkClick() {
                // 入力されている値を取得
                bool success = OnPageDeactivated();
                if (!success) {
                    return false;
                }

                ArchiveType type = m_parent.m_archiveSetting.ArchiveType;
                ArchiveSettingRemoteShellOption remoteOpt = (ArchiveSettingRemoteShellOption)(m_parent.m_archiveSetting.GetCurrentRemoteShellOption());

                // ファイル名を取得
                string arcName = m_parent.textBoxArcFileName.Text;
                if (arcName == "" || arcName.IndexOf('/') >= 0 || arcName.IndexOf('\\') >= 0) {
                    InfoBox.Warning(m_parent, Resources.DlgArchive_FileName);
                    return false;
                }
                arcName = ArchiveDialog.ModifyFileNameFromType(arcName, type);
                
                // ファイル名の重複をチェック
                foreach (UIFile oppositeFile in m_parent.m_oppositeList) {
                    if (oppositeFile.FileName == arcName) {
                        if (oppositeFile.Attribute.IsDirectory) {
                            InfoBox.Warning(m_parent, Resources.DlgArchive_RemoteDirectory);
                            return false;
                        } else {
                            DialogResult yesNo = InfoBox.Question(m_parent, MessageBoxButtons.YesNo, Resources.DlgArchive_RemoteOverwrite);
                            if (yesNo != DialogResult.Yes) {
                                return false;
                            }
                            break;
                        }
                    }
                }

                // 今回の圧縮パラメータを作成
                m_parent.m_resultArchiveParameter = new ArchiveParameter(arcName, type, remoteOpt);
                return true;
            }

            //=========================================================================================
            // 機　能：圧縮形式をダイアログのラジオボタンに反映する
            // 引　数：[in]archiveType  圧縮形式（null:変更しない）
            // 戻り値：なし
            //=========================================================================================
            private void ArchiveTypeToUi(ArchiveType archiveType) {
                if (archiveType == ArchiveType.Zip && m_supportArchiveType.Contains(ArchiveType.Zip)) {
                    m_parent.radioButtonRZip.Checked = true;
                } else if (archiveType == ArchiveType.TarGz && m_supportArchiveType.Contains(ArchiveType.TarGz)) {
                    m_parent.radioButtonRTgz.Checked = true;
                } else if (archiveType == ArchiveType.TarBz2 && m_supportArchiveType.Contains(ArchiveType.TarBz2)) {
                    m_parent.radioButtonRTbz2.Checked = true;
                } else if (archiveType == ArchiveType.Tar && m_supportArchiveType.Contains(ArchiveType.Tar)) {
                    m_parent.radioButtonRTar.Checked = true;
                }
            }

            //=========================================================================================
            // 機　能：ダイアログのラジオボタンから圧縮形式を取得する
            // 引　数：なし
            // 戻り値：圧縮形式
            //=========================================================================================
            private ArchiveType UiToArchiveType() {
                if (m_parent.radioButtonRZip.Checked) {
                    return ArchiveType.Zip;
                } else if (m_parent.radioButtonRTgz.Checked) {
                    return ArchiveType.TarGz;
                } else if (m_parent.radioButtonRTbz2.Checked) {
                    return ArchiveType.TarBz2;
                } else {
                    return ArchiveType.Tar;
                }
            }

            //=========================================================================================
            // 機　能：圧縮形式のラジオボタンの選択項目が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void RadioButton_CheckedChanged(object sender, EventArgs evt) {
                ArchiveType oldType = m_parent.m_archiveSetting.ArchiveType;
                RetrieveOptionFromUi(oldType);
                ArchiveType newType = UiToArchiveType();

                // 圧縮形式→ファイル名
                if (!m_updateDialogItem) {
                    m_updateDialogItem = true;
                    try {
                        m_parent.textBoxArcFileName.Text = ArchiveDialog.ModifyFileNameFromType(m_parent.textBoxArcFileName.Text, newType);
                    } finally {
                        m_updateDialogItem = false;
                    }
                }

                // オプションをUIに反映
                m_parent.m_archiveSetting.ArchiveType = newType;
                InitializeOption(newType);
            }

            //=========================================================================================
            // 機　能：UIからオプション設定を取得して、m_parent.m_archiveSettingに設定する
            // 引　数：[in]type   取得する圧縮ファイル形式
            // 戻り値：なし
            //=========================================================================================
            private void RetrieveOptionFromUi(ArchiveType type) {
                RemoteShellArchiveFeature feature = RemoteShellArchiveFeature.GetFeature(type);
                ArchiveSettingRemoteShellOption option = m_parent.m_archiveSetting.GetCurrentRemoteShellOption();

                // コマンド
                string dummyCommand;
                List<OSSpecLineExpect> expect;
                CreateCommand(out dummyCommand, out expect);
                option.CommandLine = GenericFileStringUtils.CombineCommandLine(m_parent.labelRCommand.Text, m_parent.textBoxRCommand.Text);
                option.CommandExpect = expect;

                // 更新時刻を合わせる
                option.ModifyTimestamp = m_parent.checkBoxRTimeStamp.Checked;

                // 圧縮レベル
                if (feature.SupportCompressionLevel) {
                    option.CompressionLevel = m_parent.trackBarRLevel.Value;
                } else {
                    option.CompressionLevel = -1;
                }
            }

            //=========================================================================================
            // 機　能：推奨ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void ButtonRecommend_Click(object sender, EventArgs evt) {
                ArchiveType type = m_parent.m_archiveSetting.ArchiveType;
                RemoteShellArchiveFeature feature = RemoteShellArchiveFeature.GetFeature(type);
                ArchiveSettingRemoteShellOption recOption = feature.RecommendedSetting;
                if (recOption != null) {
                    m_parent.checkBoxRTimeStamp.Checked = recOption.ModifyTimestamp;
                    m_parent.trackBarRLevel.Value = recOption.CompressionLevel;
                }
            }

            //=========================================================================================
            // 機　能：書庫の更新時刻の変更チェックボックスがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void CheckBoxRTimeStamp_CheckedChanged(object sender, EventArgs evt) {
                ResetCommand();
            }

            //=========================================================================================
            // 機　能：圧縮レベルの値が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void TrackBarRLevel_ValueChanged(object sender, EventArgs evt) {
                ResetCommand();
            }

            //=========================================================================================
            // 機　能：UIの値からコマンドラインを作成する
            // 引　数：[out]command   作成したコマンドラインの文字列を返す変数
            // 　　　　[out]expect    実行結果の期待値を返す変数
            // 戻り値：なし
            //=========================================================================================
            private void CreateCommand(out string command, out List<OSSpecLineExpect> expect) {
                ArchiveSettingRemoteShellOption option = m_parent.m_archiveSetting.GetCurrentRemoteShellOption();
                ArchiveType type = m_parent.m_archiveSetting.ArchiveType;
                if (type == ArchiveType.Zip) {
                    command = m_shellCommandDictionary.GetCommandArchiveZip(m_parent.checkBoxRTimeStamp.Checked, m_parent.trackBarRLevel.Value);
                    expect = m_shellCommandDictionary.GetExpectArchive(m_shellCommandDictionary.ExpectArchiveZip);
                } else if (type == ArchiveType.TarGz) {
                    command = m_shellCommandDictionary.GetCommandArchiveTarGz();
                    expect = m_shellCommandDictionary.GetExpectArchive(m_shellCommandDictionary.ExpectArchiveTarGz);
                } else if (type == ArchiveType.TarBz2) {
                    command = m_shellCommandDictionary.GetCommandArchiveTarBz2();
                    expect = m_shellCommandDictionary.GetExpectArchive(m_shellCommandDictionary.ExpectArchiveTarBz2);
                } else if (type == ArchiveType.Tar) {
                    command = m_shellCommandDictionary.GetCommandArchiveTar();
                    expect = m_shellCommandDictionary.GetExpectArchive(m_shellCommandDictionary.ExpectArchiveTar);
                } else {
                    Program.Abort("アーカイブ種別の判定エラーです。");
                    command = null;
                    expect = null;
                }
            }
        }
    }
}
