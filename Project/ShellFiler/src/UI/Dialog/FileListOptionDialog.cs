using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask.Condition;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.UI.FileList.ThumbList;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイル一覧のオプションダイアログ
    //=========================================================================================
    public partial class FileListOptionDialog : Form {
        // 開始時点と終了時点でアクティブなページ
        private OptionPage m_activePage;

        // ソートページの実装
        private SortPage m_sortPage = null;

        // フィルターページの実装
        private FilterPage m_filterPage = null;

        // カラー表示ページの実装
        private ColorPage m_colorPage = null;

        // ビューモードページの実装
        private ViewModePage m_viewModePage = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListOptionDialog() {
            InitializeComponent();
        }
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]initialPage  初期状態で表示するページ
        // 　　　　[in]sortMode     ソート方法
        // 　　　　[in]filterMode   現在のフィルター設定（フィルターを使用していないときnull）
        // 　　　　[in]viewMode     表示モード
        // 　　　　[in]fileSystem   ファイルシステム
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(OptionPage initialPage, FileListSortMode sortMode, FileListFilterMode filterMode, FileListViewMode viewMode, FileSystemID fileSystem) {
            m_activePage = initialPage;

            // ソート方法
            m_sortPage = new SortPage(this, sortMode);
            m_filterPage = new FilterPage(this, filterMode, fileSystem);
            m_colorPage = new ColorPage(this);
            m_viewModePage = new ViewModePage(this, viewMode);

            // UI
            if (filterMode != null) {
                this.tabPageFilter.Text = this.tabPageFilter.Text + Resources.DlgFileListOption_FilterInUse;
            }
        }
        
        //=========================================================================================
        // 機　能：フォームが初期化されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListOptionDialog_Load(object sender, EventArgs evt) {
            if (m_activePage == OptionPage.Sort) {
                this.tabControl.SelectedTab = this.tabPageSort;
                m_sortPage.ActivateControl();
            } else if (m_activePage == OptionPage.Filter) {
                this.tabControl.SelectedTab = this.tabPageFilter;
                m_filterPage.ActivateControl();
            } else if (m_activePage == OptionPage.Color) {
                this.tabControl.SelectedTab = this.tabPageColor;
                m_colorPage.ActivateControl();
            } else if (m_activePage == OptionPage.ViewMode) {
                this.tabControl.SelectedTab = this.tabPageViewMode;
                m_viewModePage.ActivateControl();
            }
        }

        //=========================================================================================
        // 機　能：タブページが切り替えられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void tabControl_SelectedIndexChanged(object sender, EventArgs evt) {
            TabPage selected = this.tabControl.SelectedTab;
            if (selected == this.tabPageSort) {
                m_activePage = OptionPage.Sort;
                m_sortPage.ActivateControl();
            } else if (selected == this.tabPageFilter) {
                m_activePage = OptionPage.Filter;
                m_filterPage.ActivateControl();
            } else if (selected == this.tabPageColor) {
                m_activePage = OptionPage.Color;
                m_colorPage.ActivateControl();
            } else if (selected == this.tabPageViewMode) {
                m_activePage = OptionPage.ViewMode;
                m_viewModePage.ActivateControl();
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            bool ok = false;
            TabPage selected = this.tabControl.SelectedTab;
            if (selected == this.tabPageSort) {
                ok = m_sortPage.OnOk();
            } else if (selected == this.tabPageFilter) {
                ok = m_filterPage.OnOk();
            } else if (selected == this.tabPageColor) {
                ok = m_colorPage.OnOk();
            } else if (selected == this.tabPageViewMode) {
                ok = m_viewModePage.OnOk();
            }
            if (ok) {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        //=========================================================================================
        // 機　能：ダイアログ ボックスのキーを処理する
        // 引　数：[in]keyData   処理するキー
        // 戻り値：キーストロークがコントロールによって処理および使用された場合はtrue
        //=========================================================================================
        protected override bool ProcessDialogKey(Keys keyData) {
            switch (keyData) {
                case Keys.Left:
                case Keys.Right:
                    FileListOptionDialog_KeyDown(this, new KeyEventArgs(keyData));
                    break;
                default:
                    return base.ProcessDialogKey(keyData);
            }
            return true;
        }

        //=========================================================================================
        // 機　能：キー入力をチェックする
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileListOptionDialog_KeyDown(object sender, KeyEventArgs evt) {
            TabPage selected = this.tabControl.SelectedTab;
            if (selected == this.tabPageSort) {
                m_sortPage.OnKeyDown(evt);
            } else if (selected == this.tabPageFilter) {
                m_filterPage.OnKeyDown(evt);
            } else if (selected == this.tabPageColor) {
                m_colorPage.OnKeyDown(evt);
            } else if (selected == this.tabPageViewMode) {
                m_viewModePage.OnKeyDown(evt);
            }
        }

        //=========================================================================================
        // プロパティ：開始時点と終了時点でアクティブなページ
        //=========================================================================================
        public OptionPage ActivePage {
            get {
                return m_activePage;
            }
        }

        //=========================================================================================
        // プロパティ：入力されたソート方法（初期化時とは別のインスタンス）
        //=========================================================================================
        public FileListSortMode ResultSortMode {
            get {
                return m_sortPage.ResultSortMode;
            }
        }

        //=========================================================================================
        // プロパティ：入力された条件（初期化時とは別のインスタンス）
        //=========================================================================================
        public FileListFilterMode ResultFilter {
            get {
                return m_filterPage.ResultFilter;
            }
        }

        //=========================================================================================
        // プロパティ：入力された色情報
        //=========================================================================================
        public FileListColorSetting ResultColorSetting {
            get {
                return m_colorPage.ResultColorSetting;
            }
        }

        //=========================================================================================
        // プロパティ：入力されたビューモード
        //=========================================================================================
        public FileListViewMode ResultViewMode {
            get {
                return m_viewModePage.ResultViewMode;
            }
        }

        //=========================================================================================
        // クラス：ソートページの実装
        //=========================================================================================
        private class SortPage {
            // 親ダイアログ
            private FileListOptionDialog m_parent;

            // ソート：入力されたソート方法（初期化時とは別のインスタンス）
            private FileListSortMode m_resultSortMode;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]_parent   親ダイアログ
            // 　　　　[in]sortMode  ソート方法
            // 戻り値：なし
            //=========================================================================================
            public SortPage(FileListOptionDialog parent, FileListSortMode sortMode) {
                m_parent = parent;

                m_parent.radioSort1Name.CheckedChanged   += new EventHandler(radioSort1_CheckedChanged);
                m_parent.radioSort1Time.CheckedChanged   += new EventHandler(radioSort1_CheckedChanged);
                m_parent.radioSort1Ext.CheckedChanged    += new EventHandler(radioSort1_CheckedChanged);
                m_parent.radioSort1Length.CheckedChanged += new EventHandler(radioSort1_CheckedChanged);
                m_parent.radioSort1Attr.CheckedChanged   += new EventHandler(radioSort1_CheckedChanged);
                m_parent.radioSort1None.CheckedChanged   += new EventHandler(radioSort1_CheckedChanged);

                m_parent.radioSort2Name.CheckedChanged   += new EventHandler(radioSort2_CheckedChanged);
                m_parent.radioSort2Time.CheckedChanged   += new EventHandler(radioSort2_CheckedChanged);
                m_parent.radioSort2Ext.CheckedChanged    += new EventHandler(radioSort2_CheckedChanged);
                m_parent.radioSort2Length.CheckedChanged += new EventHandler(radioSort2_CheckedChanged);
                m_parent.radioSort2Attr.CheckedChanged   += new EventHandler(radioSort2_CheckedChanged);
                m_parent.radioSort2None.CheckedChanged   += new EventHandler(radioSort2_CheckedChanged);

                m_resultSortMode = (FileListSortMode)(sortMode.Clone());
                SetSort1Check(sortMode.SortOrder1);
                SetSort2Check(sortMode.SortOrder2);

                m_parent.checkBoxDirection1.Checked = (sortMode.SortDirection1 == FileListSortMode.Direction.Reverse);
                m_parent.checkBoxDirection2.Checked = (sortMode.SortDirection2 == FileListSortMode.Direction.Reverse);

                if (sortMode.TopDirectory) {
                    m_parent.checkBoxDirectory.Checked = true;
                } else {
                    m_parent.checkBoxDirectory.Checked = false;
                }

                if (sortMode.Capital) {
                    m_parent.checkBoxCapital.Checked = true;
                } else {
                    m_parent.checkBoxCapital.Checked = false;
                }

                if (sortMode.IdentifyNumber) {
                    m_parent.checkBoxIdentifyNum.Checked = true;
                } else {
                    m_parent.checkBoxIdentifyNum.Checked = false;
                }
            }

            //=========================================================================================
            // 機　能：UI内のコントロールにフォーカスを合わせる
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void ActivateControl() {
                if (m_parent.radioSort1Name.Checked) {
                    m_parent.ActiveControl = m_parent.radioSort1Name;
                } else if (m_parent.radioSort1Time.Checked) {
                    m_parent.ActiveControl = m_parent.radioSort1Time;
                } else if (m_parent.radioSort1Ext.Checked) {
                    m_parent.ActiveControl = m_parent.radioSort1Ext;
                } else if (m_parent.radioSort1Length.Checked) {
                    m_parent.ActiveControl = m_parent.radioSort1Length;
                } else if (m_parent.radioSort1Attr.Checked) {
                    m_parent.ActiveControl = m_parent.radioSort1Attr;
                } else if (m_parent.radioSort1None.Checked) {
                    m_parent.ActiveControl = m_parent.radioSort1None;
                }
            }

            //=========================================================================================
            // 機　能：第1ソートキーをUIに反映させる
            // 引　数：[in]sortOrder1  第1ソートキー
            // 戻り値：なし
            //=========================================================================================
            private void SetSort1Check(FileListSortMode.Method sortOrder1) {
                switch (sortOrder1) {
                    case FileListSortMode.Method.FileName:
                        m_parent.radioSort1Name.Checked = true;
                        break;
                    case FileListSortMode.Method.DateTime:
                        m_parent.radioSort1Time.Checked = true;
                        break;
                    case FileListSortMode.Method.Extension:
                        m_parent.radioSort1Ext.Checked = true;
                        break;
                    case FileListSortMode.Method.FileSize:
                        m_parent.radioSort1Length.Checked = true;
                        break;
                    case FileListSortMode.Method.Attribute:
                        m_parent.radioSort1Attr.Checked = true;
                        break;
                    case FileListSortMode.Method.NoSort:
                        m_parent.radioSort1None.Checked = true;
                        break;
                }
            }

            //=========================================================================================
            // 機　能：第2ソートキーをUIに反映させる
            // 引　数：[in]sortOrder2  第2ソートキー
            // 戻り値：なし
            //=========================================================================================
            private void SetSort2Check(FileListSortMode.Method sortOrder2) {
                switch (sortOrder2) {
                    case FileListSortMode.Method.FileName:
                        m_parent.radioSort2Name.Checked = true;
                        break;
                    case FileListSortMode.Method.DateTime:
                        m_parent.radioSort2Time.Checked = true;
                        break;
                    case FileListSortMode.Method.Extension:
                        m_parent.radioSort2Ext.Checked = true;
                        break;
                    case FileListSortMode.Method.FileSize:
                        m_parent.radioSort2Length.Checked = true;
                        break;
                    case FileListSortMode.Method.Attribute:
                        m_parent.radioSort2Attr.Checked = true;
                        break;
                    case FileListSortMode.Method.NoSort:
                        m_parent.radioSort2None.Checked = true;
                        break;
                }
            }

            //=========================================================================================
            // 機　能：第1ソートキーが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void radioSort1_CheckedChanged(object sender, EventArgs e) {
                FileListSortMode.Method sortMode1 = GetSortMode1();
                FileListSortMode.Method sortMode2 = GetSortMode2();
                FileListSortMode.Method sortMode2Mod = FileListSortMode.ModifySort2BySort1(sortMode1, sortMode2);
                SetSort2Check(sortMode2Mod);
            }

            //=========================================================================================
            // 機　能：第2ソートキーが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void radioSort2_CheckedChanged(object sender, EventArgs evt) {
                FileListSortMode.Method sortMode1 = GetSortMode1();
                FileListSortMode.Method sortMode2 = GetSortMode2();
                FileListSortMode.Method sortMode1Mod = FileListSortMode.ModifySort1BySort2(sortMode1, sortMode2);
                SetSort1Check(sortMode1Mod);
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：ダイアログを閉じてよいときtrue
            //=========================================================================================
            public bool OnOk() {
                m_resultSortMode.SortOrder1 = GetSortMode1();
                m_resultSortMode.SortOrder2 = GetSortMode2();
                if (!m_parent.checkBoxDirection1.Checked) {
                    m_resultSortMode.SortDirection1 = FileListSortMode.Direction.Normal;
                } else {
                    m_resultSortMode.SortDirection1 = FileListSortMode.Direction.Reverse;
                }
                if (!m_parent.checkBoxDirection2.Checked) {
                    m_resultSortMode.SortDirection2 = FileListSortMode.Direction.Normal;
                } else {
                    m_resultSortMode.SortDirection2 = FileListSortMode.Direction.Reverse;
                }
                m_resultSortMode.TopDirectory = m_parent.checkBoxDirectory.Checked;
                m_resultSortMode.Capital = m_parent.checkBoxCapital.Checked;
                m_resultSortMode.IdentifyNumber = m_parent.checkBoxIdentifyNum.Checked;
                m_resultSortMode.ModifySortMode();
                return true;
            }
            
            //=========================================================================================
            // 機　能：キー入力をチェックする
            // 引　数：[in]evt   キー入力イベント
            // 戻り値：なし
            //=========================================================================================
            public void OnKeyDown(KeyEventArgs evt) {
                if (evt.Alt) {
                    return;
                }
                switch (evt.KeyCode) {
                    case Keys.Right:
                        // ページ変更
                        m_parent.tabControl.SelectedTab = m_parent.tabPageFilter;
                        break;
                    case Keys.D1:
                        m_parent.checkBoxDirection1.Checked = !m_parent.checkBoxDirection1.Checked;
                        m_parent.buttonOk_Click(this, new EventArgs());
                        break;
                    case Keys.D2:
                        m_parent.checkBoxDirection2.Checked = !m_parent.checkBoxDirection2.Checked;
                        m_parent.buttonOk_Click(this, new EventArgs());
                        break;
                    case Keys.F:
                        m_parent.radioSort1Name.Checked = true;
                        m_parent.buttonOk_Click(this, new EventArgs());
                        break;
                    case Keys.T:
                        m_parent.radioSort1Time.Checked = true;
                        m_parent.buttonOk_Click(this, new EventArgs());
                        break;
                    case Keys.E:
                        m_parent.radioSort1Ext.Checked = true;
                        m_parent.buttonOk_Click(this, new EventArgs());
                        break;
                    case Keys.L:
                        m_parent.radioSort1Length.Checked = true;
                        m_parent.buttonOk_Click(this, new EventArgs());
                        break;
                    case Keys.A:
                        m_parent.radioSort1Attr.Checked = true;
                        m_parent.buttonOk_Click(this, new EventArgs());
                        break;
                    case Keys.N:
                        m_parent.radioSort1None.Checked = true;
                        m_parent.buttonOk_Click(this, new EventArgs());
                        break;
                }
            }

            //=========================================================================================
            // 機　能：UIから第1ソートキーを取得する
            // 引　数：なし
            // 戻り値：ソート方法
            //=========================================================================================
            private FileListSortMode.Method GetSortMode1() {
                if (m_parent.radioSort1Name.Checked) {
                    return FileListSortMode.Method.FileName;
                } else if (m_parent.radioSort1Time.Checked) {
                    return FileListSortMode.Method.DateTime;
                } else if (m_parent.radioSort1Ext.Checked) {
                    return FileListSortMode.Method.Extension;
                } else if (m_parent.radioSort1Length.Checked) {
                    return FileListSortMode.Method.FileSize;
                } else if (m_parent.radioSort1Attr.Checked) {
                    return FileListSortMode.Method.Attribute;
                } else {
                    return FileListSortMode.Method.NoSort;
                }
            }

            //=========================================================================================
            // 機　能：UIから第2ソートキーを取得する
            // 引　数：なし
            // 戻り値：ソート方法
            //=========================================================================================
            private FileListSortMode.Method GetSortMode2() {
                if (m_parent.radioSort2Name.Checked) {
                    return FileListSortMode.Method.FileName;
                } else if (m_parent.radioSort2Time.Checked) {
                    return FileListSortMode.Method.DateTime;
                } else if (m_parent.radioSort2Ext.Checked) {
                    return FileListSortMode.Method.Extension;
                } else if (m_parent.radioSort2Length.Checked) {
                    return FileListSortMode.Method.FileSize;
                } else if (m_parent.radioSort2Attr.Checked) {
                    return FileListSortMode.Method.Attribute;
                } else {
                    return FileListSortMode.Method.NoSort;
                }
            }

            //=========================================================================================
            // プロパティ：入力されたソート方法（初期化時とは別のインスタンス）
            //=========================================================================================
            public FileListSortMode ResultSortMode {
                get {
                    return m_resultSortMode;
                }
            }
        }

        //=========================================================================================
        // クラス：フィルターページの実装
        //=========================================================================================
        private class FilterPage {
            // 親ダイアログ
            private FileListOptionDialog m_parent;

            // フィルター条件の設定（元の設定）
            private FileConditionSetting m_filterSettingOld;

            // 入力された条件（初期化時とは別のインスタンス）
            private FileListFilterMode m_resultFilter;
            
            // 条件入力のUI
            private DeleteExStartDialog.ConditionImpl m_conditionImpl;

            // ダイアログを開いたときに表示する条件変更のメッセージ
            private List<string> m_initialDialogMessage = new List<string>();

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]_parent      親ダイアログ
            // 　　　　[in]filterMode   現在のフィルター設定（フィルターを使用していないときnull）
            // 　　　　[in]fileSystem   ファイルシステム
            // 戻り値：なし
            //=========================================================================================
            public FilterPage(FileListOptionDialog parent, FileListFilterMode filterMode, FileSystemID fileSystem) {
                m_parent = parent;
                m_filterSettingOld = Program.Document.FileConditionSetting;
                m_filterSettingOld.LoadSetting();
                FileConditionSetting setting = (FileConditionSetting)(m_filterSettingOld.Clone());
                FileConditionDialogInfo dialogInfo;
                if (filterMode == null) {
                    dialogInfo = setting.FileListFilterDialogInfo;
                } else {
                    setting.FileListFilterDialogInfo = (FileConditionDialogInfo)(filterMode.DialogInfo.Clone());
                    dialogInfo = setting.FileListFilterDialogInfo;
                    CheckFilterChange(filterMode, fileSystem);
                }

                m_conditionImpl = new DeleteExStartDialog.ConditionImpl(
                            m_parent, setting, dialogInfo, fileSystem,
                            null, m_parent.radioButtonSetting, m_parent.radioButtonWild,
                            m_parent.checkedListCondition, m_parent.buttonSetting,  m_parent.textBoxWildCard);
                m_conditionImpl.EnableUIItemHandler = new DeleteExStartDialog.ConditionImpl.EnableUIItemDelegate(EnableUIItem);

                if (filterMode == null) {
                    m_parent.radioButtonPositive.Checked = true;
                } else if (filterMode.IsPositive) {
                    m_parent.radioButtonPositive.Checked = true;
                } else {
                    m_parent.radioButtonNegative.Checked = true;
                }

                // UIに反映
                m_conditionImpl.EnableUIItem();
                m_parent.tabPageFilter.Enter += new EventHandler(FileListFilterDialog_Enter);
                m_parent.FormClosed += new FormClosedEventHandler(MarkWithConditionsDialog_FormClosed);
                m_parent.checkedListCondition.KeyDown += new KeyEventHandler(UIItem_KeyDown);
                m_parent.textBoxWildCard.KeyDown += new KeyEventHandler(UIItem_KeyDown);
                m_parent.radioButtonAll.KeyDown += new KeyEventHandler(UIItem_KeyDown);
                m_parent.radioButtonAll.PreviewKeyDown += new PreviewKeyDownEventHandler(radioButtonAll_PreviewKeyDown);
            }
            
            //=========================================================================================
            // 機　能：UI内のコントロールにフォーカスを合わせる
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void ActivateControl() {
                if (m_parent.radioButtonSetting.Checked) {
                    m_parent.ActiveControl = m_parent.checkedListCondition;
                } else if (m_parent.radioButtonWild.Checked) {
                    m_parent.ActiveControl = m_parent.textBoxWildCard;
                }
            }

            //=========================================================================================
            // 機　能：フィルターのチェック状態を変更する
            // 引　数：[in]filterMode   現在のフィルター設定
            // 　　　　[in]fileSystem   ファイルシステム
            // 戻り値：なし
            //=========================================================================================
            private void CheckFilterChange(FileListFilterMode filterMode, FileSystemID fileSystem) {
                // 現在使用中の条件が手入力ワイルドカードなら問題なし
                if (!filterMode.DialogInfo.ConditionMode) {
                    return;
                }

                // 条件の変更をチェック
                List<FileConditionItem> selectedConditions = filterMode.ConditionList;
                List<string> selectedName = filterMode.DialogInfo.SelectedConditionList;
                List<string> changedName = m_filterSettingOld.IsFilterChanged(selectedName, selectedConditions, fileSystem);
                if (changedName.Count == 0) {
                    return;
                }

                m_initialDialogMessage = changedName;
            }

            //=========================================================================================
            // 機　能：ダイアログが表示されるときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void FileListFilterDialog_Enter(object sender, EventArgs evt) {
                if (m_initialDialogMessage.Count == 0) {
                    return;
                }
                FileListFilterMessageDialog dialog = new FileListFilterMessageDialog(m_initialDialogMessage);
                dialog.ShowDialog(m_parent);
                m_initialDialogMessage.Clear();
            }

            //=========================================================================================
            // 機　能：フォームが閉じられたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void MarkWithConditionsDialog_FormClosed(object sender, FormClosedEventArgs evt) {
                if (!FileConditionSetting.EqualsConfig(m_filterSettingOld, m_conditionImpl.FileConditionSetting)) {
                    m_conditionImpl.FileConditionSetting.SaveSetting();
                    Program.Document.FileConditionSetting = m_conditionImpl.FileConditionSetting;
                }
            }

            //=========================================================================================
            // 機　能：UIの有効/無効を切り替える
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EnableUIItem() {
                if (m_parent.radioButtonSetting.Checked) {
                    m_parent.radioButtonPositive.Enabled = true;
                    m_parent.radioButtonNegative.Enabled = true;
                } else if (m_parent.radioButtonWild.Checked) {
                    m_parent.radioButtonPositive.Enabled = true;
                    m_parent.radioButtonNegative.Enabled = true;
                } else {
                    m_parent.checkedListCondition.Enabled = false;
                    m_parent.buttonSetting.Enabled = false;
                    m_parent.textBoxWildCard.Enabled = false;
                    m_parent.radioButtonPositive.Enabled = false;
                    m_parent.radioButtonNegative.Enabled = false;
                }
            }

            //=========================================================================================
            // 機　能：キーが押されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void UIItem_KeyDown(object sender, KeyEventArgs evt) {
                if (evt.KeyCode == Keys.Up && sender == m_parent.checkedListCondition && m_parent.checkedListCondition.SelectedIndex == 0) {
                    m_parent.radioButtonAll.Checked = true;
                    m_conditionImpl.HideToolHint();
                    m_parent.ActiveControl = m_parent.radioButtonAll;
                    m_parent.radioButtonAll.Focus();
                    evt.Handled = true;
                    evt.SuppressKeyPress = true;
                } else if (evt.KeyCode == Keys.Down && sender == m_parent.textBoxWildCard) {
                    m_parent.radioButtonAll.Checked = true;
                    m_parent.ActiveControl = m_parent.radioButtonAll;
                    m_parent.radioButtonAll.Focus();
                    evt.Handled = true;
                    evt.SuppressKeyPress = true;
                }
                // radioButtonAllはPrevierKeyDownで処理（KeyDownイベントは届かない）
            }

            //=========================================================================================
            // 機　能：キーが押されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void radioButtonAll_PreviewKeyDown(object sender, PreviewKeyDownEventArgs evt) {
                if (evt.KeyCode == Keys.Up && sender == m_parent.radioButtonAll) {
                    m_parent.radioButtonWild.Checked = true;
                    m_parent.ActiveControl = m_parent.textBoxWildCard;
                    m_parent.textBoxWildCard.Focus();
                }
                // radioButtonAll以外はKeyDownで処理
            }
            
            //=========================================================================================
            // 機　能：条件指定/ワイルドカードのラジオボタンが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void radioButton_CheckedChanged(object sender, EventArgs evt) {
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：ダイアログを閉じてよいときtrue
            //=========================================================================================
            public bool OnOk() {
                m_resultFilter = new FileListFilterMode();
                m_resultFilter.DialogInfo = new FileConditionDialogInfo();

                if (m_parent.radioButtonSetting.Checked || m_parent.radioButtonWild.Checked) {
                    // 条件選択／ワイルドカード入力による指定
                    bool success = m_conditionImpl.OnOk(FileConditionTarget.FileOnly);
                    if (!success) {
                        return false;
                    }

                    // ファイルへの条件を確認
                    int itemForFile = GetItemForFileCount();
                    if (itemForFile == 0) {
                        InfoBox.Warning(m_parent, Resources.DlgCond_NoItemForFile);
                        return false;
                    }

                    // 否定のときは警告
                    if (m_parent.radioButtonNegative.Checked) {
                        if (InfoBox.Message(m_parent, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, Resources.DlgCond_Negative) != DialogResult.OK) {
                            return false;
                        }
                    }
                    m_resultFilter.DialogInfo = (FileConditionDialogInfo)(m_conditionImpl.FileConditionSetting.FileListFilterDialogInfo.Clone());
                    m_resultFilter.ConditionList = m_conditionImpl.ResultConditionList;
                    m_resultFilter.IsPositive = m_parent.radioButtonPositive.Checked;
                } else {
                    // すべて表示
                    m_resultFilter = null;
                }
                return true;
            }

            //=========================================================================================
            // 機　能：ファイルに対して適用できる項目数を返す
            // 引　数：なし
            // 戻り値：ファイルに対し適用できる項目数
            //=========================================================================================
            private int GetItemForFileCount() {
                int itemCount = 0;
                foreach (FileConditionItem condItem in m_conditionImpl.ResultConditionList) {
                    if (condItem.FileConditionTarget == FileConditionTarget.FileOnly || condItem.FileConditionTarget == FileConditionTarget.FileAndFolder) {
                        itemCount++;
                    }
                }
                return itemCount;
            }
            
            //=========================================================================================
            // 機　能：キー入力をチェックする
            // 引　数：[in]evt   キー入力イベント
            // 戻り値：なし
            //=========================================================================================
            public void OnKeyDown(KeyEventArgs evt) {
                switch (evt.KeyCode) {
                    case Keys.Left:
                        // ページ変更
                        m_parent.tabControl.SelectedTab = m_parent.tabPageSort;
                        break;
                    case Keys.Right:
                        // ページ変更
                        m_parent.tabControl.SelectedTab = m_parent.tabPageColor;
                        break;
                }
            }

            //=========================================================================================
            // プロパティ：入力された条件（初期化時とは別のインスタンス）
            //=========================================================================================
            public FileListFilterMode ResultFilter {
                get {
                    return m_resultFilter;
                }
            }
        }

        //=========================================================================================
        // クラス：カラー表示ページの実装
        //=========================================================================================
        private class ColorPage {
            // 親ダイアログ
            private FileListOptionDialog m_parent;

            // 色設定
            private FileListColorSetting m_colorSetting;

            // 有効になっている状態の色設定
            private FileListColorSetting m_colorSettingEnabled;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]_parent   親ダイアログ
            // 戻り値：なし
            //=========================================================================================
            public ColorPage(FileListOptionDialog parent) {
                m_parent = parent;

                // イベントを接続
                m_parent.checkBoxColReadonly.CheckedChanged += new EventHandler(CheckBoxColor_CheckedChanged);
                m_parent.checkBoxColHidden.CheckedChanged   += new EventHandler(CheckBoxColor_CheckedChanged);
                m_parent.checkBoxColSystem.CheckedChanged   += new EventHandler(CheckBoxColor_CheckedChanged);
                m_parent.checkBoxColArchive.CheckedChanged  += new EventHandler(CheckBoxColor_CheckedChanged);
                m_parent.checkBoxColSymLink.CheckedChanged  += new EventHandler(CheckBoxColor_CheckedChanged);
                m_parent.buttonColReadonly.Click += new EventHandler(ButtonColor_Click);
                m_parent.buttonColHidden.Click   += new EventHandler(ButtonColor_Click);
                m_parent.buttonColSystem.Click   += new EventHandler(ButtonColor_Click);
                m_parent.buttonColArchive.Click  += new EventHandler(ButtonColor_Click);
                m_parent.buttonColSymLink.Click  += new EventHandler(ButtonColor_Click);
                m_parent.buttonResetReadonly.Click += new EventHandler(ButtonReset_Click);
                m_parent.buttonResetHidden.Click   += new EventHandler(ButtonReset_Click);
                m_parent.buttonResetSystem.Click   += new EventHandler(ButtonReset_Click);
                m_parent.buttonResetArchive.Click  += new EventHandler(ButtonReset_Click);
                m_parent.buttonResetSymLink.Click  += new EventHandler(ButtonReset_Click);
                m_parent.panelColor.Paint += new PaintEventHandler(PanelColor_Paint);

                // UIを初期化
                Color normal = Configuration.Current.FileListFileTextColor;
                m_colorSetting = new FileListColorSetting(Configuration.Current);
                m_colorSettingEnabled = new FileListColorSetting(m_colorSetting);
                m_parent.labelColReadOnly.ForeColor = m_colorSetting.FileListReadOnlyColor;
                m_parent.checkBoxColReadonly.Checked = (m_colorSetting.FileListReadOnlyColor != normal);
                m_parent.labelColHidden.ForeColor = m_colorSetting.FileListHiddenColor;
                m_parent.checkBoxColHidden.Checked   = (m_colorSetting.FileListHiddenColor != normal);
                m_parent.labelColSystem.ForeColor = m_colorSetting.FileListSystemColor;
                m_parent.checkBoxColSystem.Checked   = (m_colorSetting.FileListSystemColor != normal);
                m_parent.labelColArchive.ForeColor = m_colorSetting.FileListArchiveColor;
                m_parent.checkBoxColArchive.Checked  = (m_colorSetting.FileListArchiveColor != normal);
                m_parent.labelColSymLink.ForeColor = m_colorSetting.FileListSymlinkColor;
                m_parent.checkBoxColSymLink.Checked  = (m_colorSetting.FileListSymlinkColor != normal);
                m_parent.panelColor.BackColor = Configuration.Current.FileListBackColor;
            }
            
            //=========================================================================================
            // 機　能：UI内のコントロールにフォーカスを合わせる
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void ActivateControl() {
                m_parent.ActiveControl = m_parent.checkBoxColReadonly;
            }

            //=========================================================================================
            // 機　能：カラー表示を行うかどうかのチェックボックスが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void CheckBoxColor_CheckedChanged(object sender, EventArgs evt) {
                if (sender == m_parent.checkBoxColReadonly) {
                    bool check = m_parent.checkBoxColReadonly.Checked;
                    if (check) {
                        m_colorSetting.FileListReadOnlyColor = m_colorSettingEnabled.FileListReadOnlyColor;
                    } else {
                        m_colorSetting.FileListReadOnlyColor = Configuration.Current.FileListFileTextColor;
                    }
                    m_parent.buttonColReadonly.Enabled = check;
                    m_parent.buttonResetReadonly.Enabled = check;
                    m_parent.labelColReadOnly.ForeColor = m_colorSetting.FileListReadOnlyColor;
                } else if (sender == m_parent.checkBoxColHidden) {
                    bool check = m_parent.checkBoxColHidden.Checked;
                    if (check) {
                        m_colorSetting.FileListHiddenColor = m_colorSettingEnabled.FileListHiddenColor;
                    } else {
                        m_colorSetting.FileListHiddenColor = Configuration.Current.FileListFileTextColor;
                    }
                    m_parent.buttonColHidden.Enabled = check;
                    m_parent.buttonResetHidden.Enabled = check;
                    m_parent.labelColHidden.ForeColor = m_colorSetting.FileListHiddenColor;
                } else if (sender == m_parent.checkBoxColSystem) {
                    bool check = m_parent.checkBoxColSystem.Checked;
                    if (check) {
                        m_colorSetting.FileListSystemColor = m_colorSettingEnabled.FileListSystemColor;
                    } else {
                        m_colorSetting.FileListSystemColor = Configuration.Current.FileListFileTextColor;
                    }
                    m_parent.buttonColSystem.Enabled = check;
                    m_parent.buttonResetSystem.Enabled = check;
                    m_parent.labelColSystem.ForeColor = m_colorSetting.FileListSystemColor;
                } else if (sender == m_parent.checkBoxColArchive) {
                    bool check = m_parent.checkBoxColArchive.Checked;
                    if (check) {
                        m_colorSetting.FileListArchiveColor = m_colorSettingEnabled.FileListArchiveColor;
                    } else {
                        m_colorSetting.FileListArchiveColor = Configuration.Current.FileListFileTextColor;
                    }
                    m_parent.buttonColArchive.Enabled = check;
                    m_parent.buttonResetArchive.Enabled = check;
                    m_parent.labelColArchive.ForeColor = m_colorSetting.FileListArchiveColor;
                } else if (sender == m_parent.checkBoxColSymLink) {
                    bool check = m_parent.checkBoxColSymLink.Checked;
                    if (check) {
                        m_colorSetting.FileListSymlinkColor = m_colorSettingEnabled.FileListSymlinkColor;
                    } else {
                        m_colorSetting.FileListSymlinkColor = Configuration.Current.FileListFileTextColor;
                    }
                    m_parent.buttonColSymLink.Enabled = check;
                    m_parent.buttonResetSymLink.Enabled = check;
                    m_parent.labelColSymLink.ForeColor = m_colorSetting.FileListSymlinkColor;
                }
                m_parent.panelColor.Invalidate();
            }
 
            //=========================================================================================
            // 機　能：色変更のボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void ButtonColor_Click(object sender, EventArgs evt) {
                Color color;
                if (sender == m_parent.buttonColReadonly) {
                    color = m_colorSetting.FileListReadOnlyColor;
                } else if (sender == m_parent.buttonColHidden) {
                    color = m_colorSetting.FileListHiddenColor;
                } else if (sender == m_parent.buttonColSystem) {
                    color = m_colorSetting.FileListSystemColor;
                } else if (sender == m_parent.buttonColArchive) {
                    color = m_colorSetting.FileListArchiveColor;
                } else if (sender == m_parent.buttonColSymLink) {
                    color = m_colorSetting.FileListSymlinkColor;
                } else {
                    return;
                }

                // ダイアログで入力
                ColorDialog colorDialog = new ColorDialog();
                colorDialog.Color = color;
                colorDialog.AllowFullOpen = true;
                colorDialog.FullOpen = true;
                colorDialog.AnyColor = true;
                colorDialog.SolidColorOnly = true;
                colorDialog.CustomColors = new int[] {};
                colorDialog.ShowHelp = false;
                if (colorDialog.ShowDialog() != DialogResult.OK) {
                    return;
                }
                color = colorDialog.Color;
                if (sender == m_parent.buttonColReadonly) {
                    m_colorSetting.FileListReadOnlyColor = color;
                    m_colorSettingEnabled.FileListReadOnlyColor = color;
                    m_parent.labelColReadOnly.ForeColor = color;
                } else if (sender == m_parent.buttonColHidden) {
                    m_colorSetting.FileListHiddenColor = color;
                    m_colorSettingEnabled.FileListHiddenColor = color;
                    m_parent.labelColHidden.ForeColor = color;
                } else if (sender == m_parent.buttonColSystem) {
                    m_colorSetting.FileListSystemColor = color;
                    m_colorSettingEnabled.FileListSystemColor = color;
                    m_parent.labelColSystem.ForeColor = color;
                } else if (sender == m_parent.buttonColArchive) {
                    m_colorSetting.FileListArchiveColor = color;
                    m_colorSettingEnabled.FileListArchiveColor = color;
                    m_parent.labelColArchive.ForeColor = color;
                } else if (sender == m_parent.buttonColSymLink) {
                    m_colorSetting.FileListSymlinkColor = color;
                    m_colorSettingEnabled.FileListSymlinkColor = color;
                    m_parent.labelColSymLink.ForeColor = color;
                }
                m_parent.panelColor.Invalidate();
            }
 
            //=========================================================================================
            // 機　能：リセットボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void ButtonReset_Click(object sender, EventArgs evt) {
                Configuration defaultValue = new Configuration();
                if (sender == m_parent.buttonResetReadonly) {
                    Color color = defaultValue.FileListReadOnlyColor;
                    m_colorSetting.FileListReadOnlyColor = color;
                    m_colorSettingEnabled.FileListReadOnlyColor = color;
                    m_parent.labelColReadOnly.ForeColor = color;
                } else if (sender == m_parent.buttonResetHidden) {
                    Color color = defaultValue.FileListHiddenColor;
                    m_colorSetting.FileListHiddenColor = color;
                    m_colorSettingEnabled.FileListHiddenColor = color;
                    m_parent.labelColHidden.ForeColor = color;
                } else if (sender == m_parent.buttonResetSystem) {
                    Color color = defaultValue.FileListSystemColor;
                    m_colorSetting.FileListSystemColor = color;
                    m_colorSettingEnabled.FileListSystemColor = color;
                    m_parent.labelColSystem.ForeColor = color;
                } else if (sender == m_parent.buttonResetArchive) {
                    Color color = defaultValue.FileListArchiveColor;
                    m_colorSetting.FileListArchiveColor = color;
                    m_colorSettingEnabled.FileListArchiveColor = color;
                    m_parent.labelColArchive.ForeColor = color;
                } else if (sender == m_parent.buttonResetSymLink) {
                    Color color = defaultValue.FileListSymlinkColor;
                    m_colorSetting.FileListSymlinkColor = color;
                    m_colorSettingEnabled.FileListSymlinkColor = color;
                    m_parent.labelColSymLink.ForeColor = color;
                }
                m_parent.panelColor.Invalidate();
            }

            //=========================================================================================
            // 機　能：サンプル領域の再描画イベントを処理する
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void PanelColor_Paint(object sender, PaintEventArgs evt) {
                FileListViewMode viewMode = new FileListViewMode();
                viewMode.ThumbnailModeSwitch = false;
                SampleRenderer renderer = new SampleRenderer(m_parent.panelColor, m_colorSetting, viewMode);
                renderer.Draw(evt.Graphics);
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：ダイアログを閉じてよいときtrue
            //=========================================================================================
            public bool OnOk() {
                return true;
            }
            
            //=========================================================================================
            // 機　能：キー入力をチェックする
            // 引　数：[in]evt   キー入力イベント
            // 戻り値：なし
            //=========================================================================================
            public void OnKeyDown(KeyEventArgs evt) {
                switch (evt.KeyCode) {
                    case Keys.Left:
                        // ページ変更
                        m_parent.tabControl.SelectedTab = m_parent.tabPageFilter;
                        break;
                    case Keys.Right:
                        // ページ変更
                        m_parent.tabControl.SelectedTab = m_parent.tabPageViewMode;
                        break;
                }
            }

            //=========================================================================================
            // プロパティ：入力された色情報
            //=========================================================================================
            public FileListColorSetting ResultColorSetting {
                get {
                    return m_colorSetting;
                }
            }
        }

        //=========================================================================================
        // クラス：カラー表示の設定内容
        //=========================================================================================
        public class FileListColorSetting {
            // 読み取り専用ファイルの色
            private Color m_fileListReadOnlyColor;

            // 隠しファイルの色
            private Color m_fileListHiddenColor;

            // システムファイルの色
            private Color m_fileListSystemColor;

            // アーカイブファイルの色
            private Color m_fileListArchiveColor;

            // シンボリックリンクファイルの色
            private Color m_fileListSymlinkColor;

            //=========================================================================================
            // 機　能：コンストラクタ（現在の設定用）
            // 引　数：[in]config   コンフィグ
            // 戻り値：なし
            //=========================================================================================
            public FileListColorSetting(Configuration config) {
                m_fileListReadOnlyColor = config.FileListReadOnlyColor;
                m_fileListHiddenColor = config.FileListHiddenColor;
                m_fileListSystemColor = config.FileListSystemColor;
                m_fileListArchiveColor = config.FileListArchiveColor;
                m_fileListSymlinkColor = config.FileListSymlinkColor;
            }

            //=========================================================================================
            // 機　能：コンストラクタ（デフォルト設定用）
            // 引　数：[in]src   元の色情報
            // 戻り値：なし
            //=========================================================================================
            public FileListColorSetting(FileListColorSetting src) {
                Configuration config = new Configuration();
                Color black = Color.Black;
                if (ObjectUtils.EqualsColor(src.m_fileListReadOnlyColor, black)) {
                    m_fileListReadOnlyColor = config.FileListReadOnlyColor;
                } else {
                    m_fileListReadOnlyColor = src.m_fileListReadOnlyColor;
                }
                if (ObjectUtils.EqualsColor(src.m_fileListHiddenColor, black)) {
                    m_fileListHiddenColor = config.FileListHiddenColor;
                } else {
                    m_fileListHiddenColor = src.m_fileListHiddenColor;
                }
                if (ObjectUtils.EqualsColor(src.m_fileListSystemColor, black)) {
                    m_fileListSystemColor = config.FileListSystemColor;
                } else {
                    m_fileListSystemColor = src.m_fileListSystemColor;
                }
                if (ObjectUtils.EqualsColor(src.m_fileListArchiveColor, black)) {
                    m_fileListArchiveColor = Color.FromArgb(128, 128, 128);
                } else {
                    m_fileListArchiveColor = src.m_fileListArchiveColor;
                }
                if (ObjectUtils.EqualsColor(src.m_fileListSymlinkColor, black)) {
                    m_fileListSymlinkColor = config.FileListSymlinkColor;
                } else {
                    m_fileListSymlinkColor = src.m_fileListSymlinkColor;
                }
            }

            //=========================================================================================
            // 機　能：色設定をコンフィグに反映させる
            // 引　数：[in]src   設定元の色情報
            // 　　　　[in]dest  設定先のコンフィグ
            // 戻り値：なし
            //=========================================================================================
            public static void SetConfiguration(FileListColorSetting src, Configuration dest) {
                dest.FileListReadOnlyColor = src.m_fileListReadOnlyColor;
                dest.FileListHiddenColor = src.m_fileListHiddenColor;
                dest.FileListSystemColor = src.m_fileListSystemColor;
                dest.FileListArchiveColor = src.m_fileListArchiveColor;
                dest.FileListSymlinkColor = src.m_fileListSymlinkColor;
            }

            //=========================================================================================
            // プロパティ：読み取り専用ファイルの色
            //=========================================================================================
            public Color FileListReadOnlyColor {
                get {
                    return m_fileListReadOnlyColor;
                }
                set {
                    m_fileListReadOnlyColor = value;
                }
            }

            //=========================================================================================
            // プロパティ：隠しファイルの色
            //=========================================================================================
            public Color FileListHiddenColor {
                get {
                    return m_fileListHiddenColor;
                }
                set {
                    m_fileListHiddenColor = value;
                }
            }

            //=========================================================================================
            // プロパティ：システムファイルの色
            //=========================================================================================
            public Color FileListSystemColor {
                get {
                    return m_fileListSystemColor;
                }
                set {
                    m_fileListSystemColor = value;
                }
            }

            //=========================================================================================
            // プロパティ：アーカイブファイルの色
            //=========================================================================================
            public Color FileListArchiveColor {
                get {
                    return m_fileListArchiveColor;
                }
                set {
                    m_fileListArchiveColor = value;
                }
            }

            //=========================================================================================
            // プロパティ：シンボリックリンクファイルの色
            //=========================================================================================
            public Color FileListSymlinkColor {
                get {
                    return m_fileListSymlinkColor;
                }
                set {
                    m_fileListSymlinkColor = value;
                }
            }
        }

        //=========================================================================================
        // クラス：表示モードページの実装
        //=========================================================================================
        private class ViewModePage {
            // 親ダイアログ
            private FileListOptionDialog m_parent;

            // 入力された表示方法（初期化時とは別のインスタンス）
            private FileListViewMode m_resultViewMode;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent    親ダイアログ
            // 　　　　[in]viewMode  表示オプション
            // 戻り値：なし
            //=========================================================================================
            public ViewModePage(FileListOptionDialog parent, FileListViewMode viewMode) {
                m_parent = parent;

                // 初期値を設定
                if (!viewMode.ThumbnailModeSwitch) {
                    m_parent.radioViewDetail.Checked = true;
                } else {
                    m_parent.radioViewThumb.Checked = true;
                }
                
                FileListViewIconSize size = viewMode.ThumbnailSize;
                if (size == FileListViewIconSize.IconSize32) {
                    m_parent.radioViewThumb1.Checked = true;
                } else if (size == FileListViewIconSize.IconSize48) {
                    m_parent.radioViewThumb2.Checked = true;
                } else if (size == FileListViewIconSize.IconSize64) {
                    m_parent.radioViewThumb3.Checked = true;
                } else if (size == FileListViewIconSize.IconSize128) {
                    m_parent.radioViewThumb4.Checked = true;
                } else if (size == FileListViewIconSize.IconSize256) {
                    m_parent.radioViewThumb5.Checked = true;
                }

                FileListViewThumbnailName name = viewMode.ThumbnailName;
                if (name == FileListViewThumbnailName.ThumbNameSpearate) {
                    m_parent.radioViewNameSep.Checked = true;
                } else if (name == FileListViewThumbnailName.ThumbNameOverray) {
                    m_parent.radioViewNameOver.Checked = true;
                } else {
                    m_parent.radioViewNameNone.Checked = true;
                }

                // イベントを接続
                m_parent.radioViewDetail.CheckedChanged += new EventHandler(RadioViewMode_CheckedChanged);
                m_parent.radioViewThumb.CheckedChanged  += new EventHandler(RadioViewMode_CheckedChanged);
                m_parent.radioViewThumb1.CheckedChanged += new EventHandler(RadioViewThumb_CheckedChanged);
                m_parent.radioViewThumb2.CheckedChanged += new EventHandler(RadioViewThumb_CheckedChanged);
                m_parent.radioViewThumb3.CheckedChanged += new EventHandler(RadioViewThumb_CheckedChanged);
                m_parent.radioViewThumb4.CheckedChanged += new EventHandler(RadioViewThumb_CheckedChanged);
                m_parent.radioViewThumb5.CheckedChanged += new EventHandler(RadioViewThumb_CheckedChanged);
                m_parent.radioViewNameSep.CheckedChanged += new EventHandler(RadioViewThumb_CheckedChanged);
                m_parent.radioViewNameOver.CheckedChanged += new EventHandler(RadioViewThumb_CheckedChanged);
                m_parent.radioViewNameNone.CheckedChanged += new EventHandler(RadioViewThumb_CheckedChanged);
                m_parent.panelViewSample.Paint += new PaintEventHandler(PanelViewMode_Paint);

                EnableUIItem();
                RefreshSample();
            }

            //=========================================================================================
            // 機　能：入力結果を取得する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void GetResultViewMode() {
                m_resultViewMode = new FileListViewMode();

                // 初期値を設定
                bool thumbMode;
                if (m_parent.radioViewThumb.Checked) {
                    thumbMode = true;
                } else {
                    thumbMode = false;
                }
                
                FileListViewIconSize size;
                if (m_parent.radioViewThumb1.Checked) {
                    size = FileListViewIconSize.IconSize32;
                } else if (m_parent.radioViewThumb2.Checked) {
                    size = FileListViewIconSize.IconSize48;
                } else if (m_parent.radioViewThumb3.Checked) {
                    size = FileListViewIconSize.IconSize64;
                } else if (m_parent.radioViewThumb4.Checked) {
                    size = FileListViewIconSize.IconSize128;
                } else {
                    size = FileListViewIconSize.IconSize256;
                }

                FileListViewThumbnailName name;
                if (m_parent.radioViewNameSep.Checked) {
                    name = FileListViewThumbnailName.ThumbNameSpearate;
                } else if (m_parent.radioViewNameOver.Checked) {
                    name = FileListViewThumbnailName.ThumbNameOverray;
                } else {
                    name = FileListViewThumbnailName.ThumbNameNone;
                }

                m_resultViewMode.ThumbnailModeSwitch = thumbMode;
                m_resultViewMode.ThumbnailSize = size;
                m_resultViewMode.ThumbnailName = name;
            }
            
            //=========================================================================================
            // 機　能：UI内のコントロールにフォーカスを合わせる
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void ActivateControl() {
                if (m_parent.radioViewDetail.Checked) {
                    m_parent.ActiveControl = m_parent.radioViewDetail;
                } else {
                    m_parent.ActiveControl = m_parent.radioViewThumb;
                }

                // アクティブなコントロールを設定
                if (m_parent.radioViewDetail.Checked) {
                    m_parent.ActiveControl = m_parent.radioViewDetail;
                } else {
                    if (m_parent.radioViewThumb1.Checked) {
                        m_parent.ActiveControl = m_parent.radioViewThumb1;
                    } else if (m_parent.radioViewThumb2.Checked) {
                        m_parent.ActiveControl = m_parent.radioViewThumb2;
                    } else if (m_parent.radioViewThumb3.Checked) {
                        m_parent.ActiveControl = m_parent.radioViewThumb3;
                    } else if (m_parent.radioViewThumb4.Checked) {
                        m_parent.ActiveControl = m_parent.radioViewThumb4;
                    } else if (m_parent.radioViewThumb5.Checked) {
                        m_parent.ActiveControl = m_parent.radioViewThumb5;
                    }
                }
            }

            //=========================================================================================
            // 機　能：UIの有効/無効を切り替える
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EnableUIItem() {
                bool enableThumb = m_parent.radioViewThumb.Checked;
                m_parent.radioViewThumb1.Enabled = enableThumb;
                m_parent.radioViewThumb2.Enabled = enableThumb;
                m_parent.radioViewThumb3.Enabled = enableThumb;
                m_parent.radioViewThumb4.Enabled = enableThumb;
                m_parent.radioViewThumb5.Enabled = enableThumb;
                m_parent.radioViewNameSep.Enabled = enableThumb;
                m_parent.radioViewNameOver.Enabled = enableThumb;
                m_parent.radioViewNameNone.Enabled = enableThumb;
            }

            //=========================================================================================
            // 機　能：サンプル表示をリフレッシュする
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void RefreshSample() {
                GetResultViewMode();
                m_parent.panelViewSample.Invalidate();
            }

            //=========================================================================================
            // 機　能：表示モードの選択が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void RadioViewMode_CheckedChanged(object sender, EventArgs evt) {
                EnableUIItem();
                RefreshSample();
            }

            //=========================================================================================
            // 機　能：サムネイルの詳細が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void RadioViewThumb_CheckedChanged(object sender, EventArgs evt) {
                RefreshSample();
            }

            //=========================================================================================
            // 機　能：サムネイルの名前モードが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void ComboBoxViewThumbName_SelectedIndexChanged(object sender, EventArgs evt) {
                RefreshSample();
            }

            //=========================================================================================
            // 機　能：サンプル領域の再描画イベントを処理する
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void PanelViewMode_Paint(object sender, PaintEventArgs evt) {
                SampleRenderer renderer = new SampleRenderer(m_parent.panelViewSample, m_parent.m_colorPage.ResultColorSetting, m_resultViewMode);
                renderer.Draw(evt.Graphics);
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：ダイアログを閉じてよいときtrue
            //=========================================================================================
            public bool OnOk() {
                return true;
            }
            
            //=========================================================================================
            // 機　能：キー入力をチェックする
            // 引　数：[in]evt   キー入力イベント
            // 戻り値：なし
            //=========================================================================================
            public void OnKeyDown(KeyEventArgs evt) {
                switch (evt.KeyCode) {
                    case Keys.Left:
                        // ページ変更
                        m_parent.tabControl.SelectedTab = m_parent.tabPageColor;
                        break;
                }
            }

            //=========================================================================================
            // プロパティ：入力された表示方法（初期化時とは別のインスタンス）
            //=========================================================================================
            public FileListViewMode ResultViewMode {
                get {
                    return m_resultViewMode;
                }
            }
        }

        //=========================================================================================
        // クラス：サンプルのレンダリングを行うクラス
        //=========================================================================================
        public class SampleRenderer {
            // 描画対象のパネル
            private Panel m_targetPanel;

            // 色の表示方法
            private FileListColorSetting m_colorSetting;

            // 画面の表示モード
            private FileListViewMode m_viewMode;
            
            //=========================================================================================
            // 機　能：キー入力をチェックする
            // 引　数：[in]targetPanel  描画対象のパネル
            // 　　　　[in]colorSetting 色の表示方法
            // 　　　　[in]viewMode     画面の表示モード
            // 戻り値：なし
            //=========================================================================================
            public SampleRenderer(Panel targetPanel, FileListColorSetting colorSetting, FileListViewMode viewMode) {
                m_targetPanel = targetPanel;
                m_colorSetting = colorSetting;
                m_viewMode = viewMode;
            }

            //=========================================================================================
            // 機　能：画面を描画する
            // 引　数：[in]grp  描画に使用するグラフィックス
            // 戻り値：なし
            //=========================================================================================
            public void Draw(Graphics grp) {
                DoubleBuffer buffer = new DoubleBuffer(grp, m_targetPanel.Width, m_targetPanel.Height);
                FileListSampleGraphics g = new FileListSampleGraphics(buffer.DrawingGraphics, m_colorSetting);
                try {
                    if (m_viewMode == null) {
                        g.Graphics.FillRectangle(g.FileListBackBrush, buffer.DrawingRectangle);
                    } else if (m_viewMode.ThumbnailModeSwitch) {
                        DrawThumbnailMode(g, buffer.DrawingRectangle);
                    } else {
                        DrawDetailMode(g, buffer.DrawingRectangle);
                    }
                } finally {
                    g.Dispose();
                    buffer.FlushScreen(0, 0);
                }
            }

            //=========================================================================================
            // 機　能：サンプル行として表示する内容を作成する
            // 引　数：[in]g  描画に使用するグラフィックス
            // 戻り値：描画に使用するデータ
            //=========================================================================================
            private List<SampleLine> CreateSampleLines(FileListSampleGraphics g) {
                List<SampleLine> resultLines = new List<SampleLine>();
                resultLines.Add(new SampleLine(g.FileListTextBrush,     true,  "NormalFolder",       0,       FileAttribute.FromFileInfo(FileAttributes.Normal, false),   "11/10/23 12:00"));
                resultLines.Add(new SampleLine(g.FileListReadonlyBrush, true,  "ReadOnlyFolder",     0,       FileAttribute.FromFileInfo(FileAttributes.ReadOnly, false), "11/10/24 00:01"));
                resultLines.Add(new SampleLine(g.FileListHiddenBrush,   true,  "HiddenFolder",       0,       FileAttribute.FromFileInfo(FileAttributes.Hidden, false),   "12/01/15 01:00"));
                resultLines.Add(new SampleLine(g.FileListSystemBrush,   true,  "SystemFolder",       0,       FileAttribute.FromFileInfo(FileAttributes.System, false),   "12/01/28 13:01"));
                resultLines.Add(new SampleLine(g.FileListArchiveBrush,  true,  "ArchiveFolder",      0,       FileAttribute.FromFileInfo(FileAttributes.Archive, false),  "12/04/01 01:10"));
                resultLines.Add(new SampleLine(g.FileListSymlinkBrush,  true,  "SymbolicLinkFolder", 0,       FileAttribute.FromFileInfo(FileAttributes.Normal, true),    "11/10/23 12:00"));
                resultLines.Add(new SampleLine(g.FileListTextBrush,     false, "NormalFile",         2097673, FileAttribute.FromFileInfo(FileAttributes.Normal, false),   "10/12/07 14:04"));
                resultLines.Add(new SampleLine(g.FileListReadonlyBrush, false, "ReadOnlyFile",       2097407, FileAttribute.FromFileInfo(FileAttributes.ReadOnly, false), "07/02/03 02:24"));
                resultLines.Add(new SampleLine(g.FileListHiddenBrush,   false, "HiddenFile",         53248,   FileAttribute.FromFileInfo(FileAttributes.Hidden, false),   "06/06/25 14:23"));
                resultLines.Add(new SampleLine(g.FileListSystemBrush,   false, "SystemFile",         72368,   FileAttribute.FromFileInfo(FileAttributes.System, false),   "04/05/19 02:22"));
                resultLines.Add(new SampleLine(g.FileListArchiveBrush,  false, "ArchiveFile",        212992,  FileAttribute.FromFileInfo(FileAttributes.Archive, false),  "02/03/19 14:21"));
                resultLines.Add(new SampleLine(g.FileListSymlinkBrush,  false, "SymbolicLinkFile",   184320,  FileAttribute.FromFileInfo(FileAttributes.Normal, true),    "01/01/08 02:20"));
                return resultLines;
            }

            //=========================================================================================
            // 機　能：詳細モードで描画する
            // 引　数：[in]g       描画に使用するグラフィックス
            // 　　　　[in]rcDraw  描画する領域
            // 戻り値：なし
            //=========================================================================================
            private void DrawDetailMode(FileListSampleGraphics g, Rectangle rcDraw) {
                g.Graphics.FillRectangle(g.FileListBackBrush, rcDraw);
                List<SampleLine> resultLineList = CreateSampleLines(g);
                for (int i = 0; i < resultLineList.Count; i++) {
                    DrawDetailSampleLine(g, resultLineList[i], i);
                }
            }

            //=========================================================================================
            // 機　能：詳細モードでサンプル領域の行を描画する
            // 引　数：[in]g         グラフィックス
            // 　　　　[in]fileInfo  描画するファイル情報
            // 　　　　[in]line      表示する行位置
            // 戻り値：なし
            //=========================================================================================
            private void DrawDetailSampleLine(FileListSampleGraphics g, SampleLine fileInfo, int line) {
                // 表示位置を決定
                int cy = g.FileListFont.Height;
                int yPos = line * cy + 2;
                RectangleF rectFile = new RectangleF(20, yPos, 120, cy);
                RectangleF rectSize = new RectangleF(rectFile.Right + 10, yPos, 60, cy);
                RectangleF rectDate = new RectangleF(rectSize.Right + 10, yPos, 200, cy);

                // アイコンを決定
                FileIconID iconId;
                if (fileInfo.IsFolder) {
                    iconId = Program.Document.FileIconManager.DefaultFolderIconId;
                } else {
                    iconId = Program.Document.FileIconManager.DefaultFileIconId;
                }
                FileIcon icon = Program.Document.FileIconManager.GetFileIcon(iconId, FileIconID.NullId, FileListViewIconSize.IconSize16);
                Bitmap bmpIcon = icon.IconImage;
                
                // 描画
                Brush brush = fileInfo.TextBrush;
                g.Graphics.DrawImage(bmpIcon, 4, yPos);
                g.Graphics.DrawString(fileInfo.FileName, g.FileListFont, brush, rectFile, g.StringFormatEllipsis);
                g.Graphics.DrawString(fileInfo.FileSize, g.FileListFont, brush, rectSize, g.StringFormatRight);
                g.Graphics.DrawString(fileInfo.Timestamp, g.FileListFont, brush, rectDate, g.StringFormatEllipsis);
            }

            //=========================================================================================
            // 機　能：サムネイルモードで描画する
            // 引　数：[in]g       描画に使用するグラフィックス
            // 　　　　[in]rcDraw  描画する領域
            // 戻り値：なし
            //=========================================================================================
            private void DrawThumbnailMode(FileListSampleGraphics g, Rectangle rcDraw) {
                g.Graphics.FillRectangle(g.FileListBackBrush, rcDraw);
                ThumbListRenderer renderer = new ThumbListRenderer(m_targetPanel, m_viewMode);
                int cxItemToItem = renderer.FileItemSizeDpiModified.Width + g.X(ThumbListRenderer.MARGIN_ITEM);
                int cyItemToItem = renderer.FileItemSizeDpiModified.Height + g.Y(ThumbListRenderer.MARGIN_ITEM);
                FileListGraphics gFile = new FileListGraphics(g.Graphics, 0, renderer.FileItemSizeDpiModified.Height + g.Y(ThumbListRenderer.MARGIN_ITEM));
                int xCount = Math.Max(1, rcDraw.Width / cxItemToItem);
                int yCount = rcDraw.Height / cyItemToItem + 1;
                List<SampleLine> resultLineList = CreateSampleLines(g);
                for (int y = 0; y < yCount; y++) {
                    for (int x = 0; x < xCount; x++) {
                        int index = y * xCount + x;
                        if (index >= resultLineList.Count) {
                            break;
                        }
                        DrawThumbSampleLine(gFile, renderer, resultLineList[index], index, x * cxItemToItem, y * cyItemToItem);
                    }
                }
            }

            //=========================================================================================
            // 機　能：詳細モードでサンプル領域の行を描画する
            // 引　数：[in]g         グラフィックス
            // 　　　　[in]fileInfo  描画するファイル情報
            // 　　　　[in]line      表示する行位置
            // 戻り値：なし
            //=========================================================================================
            private void DrawThumbSampleLine(FileListGraphics g, ThumbListRenderer renderer, SampleLine fileInfo, int line, int xPos, int yPos) {
                fileInfo.UIFile.DefaultFileIconId = Program.Document.FileIconManager.GetDefaultIconId(fileInfo.UIFile);
                fileInfo.UIFile.FileIconId = fileInfo.UIFile.DefaultFileIconId;
                renderer.DrawItem(g, xPos, yPos, true, fileInfo.UIFile, false);
            }
        }

        //=========================================================================================
        // クラス：ファイル一覧のサンプル描画用グラフィックス
        //=========================================================================================
        private class FileListSampleGraphics : HighDpiGraphics {
            // グラフィック
            private Graphics m_graphics;

            // 色設定
            private FileListColorSetting m_colorSetting;

            // ファイル一覧 カーソル描画用のペン
            private Pen m_fileListCursorPen = null;

            // ファイル一覧 背景描画用のブラシ
            private Brush m_fileListBackBrush = null;

            // ファイル一覧 通常のファイルのブラシ
            private Brush m_fileListTextBrush = null;

            // ファイル一覧 読み込み専用ファイルのブラシ
            private Brush m_fileListReadonlyBrush = null;

            // ファイル一覧 隠しファイルのブラシ
            private Brush m_fileListHiddenBrush = null;

            // ファイル一覧 システムファイルのブラシ
            private Brush m_fileListSystemBrush = null;

            // ファイル一覧 アーカイブファイルのブラシ
            private Brush m_fileListArchiveBrush = null;

            // ファイル一覧 シンボリックリンクのブラシ
            private Brush m_fileListSymlinkBrush = null;

            // ファイル一覧 描画用のフォント
            private Font m_fileListFont = null;

            // 描画時のフォーマッタ（省略記号付き）
            private StringFormat m_stringFormatEllipsis = null;

            // 描画時のフォーマッタ（右寄せ）
            private StringFormat m_stringFormatRight = null;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]graphics     グラフィックス
            // 　　　　[in]colorSetting 色設定
            // 戻り値：なし
            //=========================================================================================
            public FileListSampleGraphics(Graphics graphics, FileListColorSetting colorSetting) : base(graphics) {
                m_graphics = graphics;
                m_colorSetting = colorSetting;
            }

            //=========================================================================================
            // 機　能：グラフィックスを破棄する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public override void Dispose() {
                if (m_fileListCursorPen != null) {
                    m_fileListCursorPen.Dispose();
                    m_fileListCursorPen = null;
                }
                if (m_fileListBackBrush != null) {
                    m_fileListBackBrush.Dispose();
                    m_fileListBackBrush = null;
                }
                if (m_fileListTextBrush != null) {
                    m_fileListTextBrush.Dispose();
                    m_fileListTextBrush = null;
                }
                if (m_fileListReadonlyBrush != null) {
                    m_fileListReadonlyBrush.Dispose();
                    m_fileListReadonlyBrush = null;
                }
                if (m_fileListHiddenBrush != null) {
                    m_fileListHiddenBrush.Dispose();
                    m_fileListHiddenBrush = null;
                }
                if (m_fileListSystemBrush != null) {
                    m_fileListSystemBrush.Dispose();
                    m_fileListSystemBrush = null;
                }
                if (m_fileListArchiveBrush != null) {
                    m_fileListArchiveBrush.Dispose();
                    m_fileListArchiveBrush = null;
                }
                if (m_fileListSymlinkBrush != null) {
                    m_fileListSymlinkBrush.Dispose();
                    m_fileListSymlinkBrush = null;
                }
                if (m_fileListFont != null) {
                    m_fileListFont.Dispose();
                    m_fileListFont = null;
                }
                if (m_stringFormatEllipsis != null) {
                    m_stringFormatEllipsis.Dispose();
                    m_stringFormatEllipsis = null;
                }
                if (m_stringFormatRight != null) {
                    m_stringFormatRight.Dispose();
                    m_stringFormatRight = null;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル一覧 カーソル描画用のペン
            //=========================================================================================
            public Pen FileListCursorPen {
                get {
                    if (m_fileListCursorPen == null) {
                        m_fileListCursorPen = new Pen(Configuration.Current.FileListCursorColor);
                    }
                    return m_fileListCursorPen;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル一覧 背景描画用のブラシ
            //=========================================================================================
            public Brush FileListBackBrush {
                get {
                    if (m_fileListBackBrush == null) {
                        m_fileListBackBrush = new SolidBrush(Configuration.Current.FileListBackColor);
                    }
                    return m_fileListBackBrush;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル一覧 通常のファイルのブラシ
            //=========================================================================================
            public Brush FileListTextBrush {
                get {
                    if (m_fileListTextBrush == null) {
                        m_fileListTextBrush = new SolidBrush(Configuration.Current.FileListFileTextColor);
                    }
                    return m_fileListTextBrush;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル一覧 読み込み専用ファイルのブラシ
            //=========================================================================================
            public Brush FileListReadonlyBrush {
                get {
                    if (m_fileListReadonlyBrush == null) {
                        m_fileListReadonlyBrush = new SolidBrush(m_colorSetting.FileListReadOnlyColor);
                    }
                    return m_fileListReadonlyBrush;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル一覧 隠しファイルのブラシ
            //=========================================================================================
            public Brush FileListHiddenBrush {
                get {
                    if (m_fileListHiddenBrush == null) {
                        m_fileListHiddenBrush = new SolidBrush(m_colorSetting.FileListHiddenColor);
                    }
                    return m_fileListHiddenBrush;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル一覧 システムファイルのブラシ
            //=========================================================================================
            public Brush FileListSystemBrush {
                get {
                    if (m_fileListSystemBrush == null) {
                        m_fileListSystemBrush = new SolidBrush(m_colorSetting.FileListSystemColor);
                    }
                    return m_fileListSystemBrush;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル一覧 アーカイブファイルのブラシ
            //=========================================================================================
            public Brush FileListArchiveBrush {
                get {
                    if (m_fileListArchiveBrush == null) {
                        m_fileListArchiveBrush = new SolidBrush(m_colorSetting.FileListArchiveColor);
                    }
                    return m_fileListArchiveBrush;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル一覧 シンボリックリンクのブラシ
            //=========================================================================================
            public Brush FileListSymlinkBrush {
                get {
                    if (m_fileListSymlinkBrush == null) {
                        m_fileListSymlinkBrush = new SolidBrush(m_colorSetting.FileListSymlinkColor);
                    }
                    return m_fileListSymlinkBrush;
                }
            }
            
            //=========================================================================================
            // プロパティ：ファイル一覧 描画用のフォント
            //=========================================================================================
            public Font FileListFont {
                get {
                    if (m_fileListFont == null) {
                        m_fileListFont = new Font(Configuration.Current.ListViewFontName, Configuration.Current.ListViewFontSize);
                    }
                    return m_fileListFont;
                }
            }
            
            //=========================================================================================
            // プロパティ：省略記号付きのフォーマッタ
            //=========================================================================================
            public StringFormat StringFormatEllipsis {
                get {
                    if (m_stringFormatEllipsis == null) {
                        m_stringFormatEllipsis = new StringFormat();
                        m_stringFormatEllipsis.Trimming = StringTrimming.EllipsisCharacter;
                    }
                    return m_stringFormatEllipsis;
                }
            }

            //=========================================================================================
            // プロパティ：右寄せのフォーマッタ
            //=========================================================================================
            public StringFormat StringFormatRight {
                get {
                    if (m_stringFormatRight == null) {
                        m_stringFormatRight = new StringFormat();
                        m_stringFormatRight.Alignment = StringAlignment.Far;
                    }
                    return m_stringFormatRight;
                }
            }
        }

        //=========================================================================================
        // クラス：サンプルの行情報
        //=========================================================================================
        private class SampleLine {
            // テキストの描画に使用するブラシ
            public Brush TextBrush;

            // フォルダのときtrue
            public bool IsFolder;
            
            // ファイル名
            public string FileName;

            // ファイルサイズ
            public string FileSize;

            // タイムスタンプ
            public string Timestamp;

            // ファイル情報
            public UIFile UIFile;

            //=========================================================================================
            // 機　能：サンプル領域の行を描画する
            // 引　数：[in]textBrush  テキストの描画に使用するブラシ
            // 　　　　[in]isFolder   フォルダのときtrue
            // 　　　　[in]fileName   ファイル名
            // 　　　　[in]fileSize   ファイルサイズ
            // 　　　　[in]timestamp  タイムスタンプ
            // 戻り値：なし
            //=========================================================================================
            public SampleLine(Brush textBrush, bool isFolder, string fileName, long fileSize, FileAttribute attr, string timestamp) {
                TextBrush = textBrush;
                IsFolder = isFolder;
                FileName = fileName;
                FileSize = StringUtils.FileSizeToString(fileSize);
                Timestamp = timestamp;
                DateTime dateTime = DateTime.ParseExact(timestamp, "yy'/'MM'/'dd HH':'mm", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
                UIFile = new UIFile(new SimpleFile(fileName, dateTime, attr, fileSize)); 
            }
        }

        //=========================================================================================
        // 列挙子：ダイアログの各ページを表現する識別子
        //=========================================================================================
        public enum OptionPage {
            Sort,               // ソート
            Filter,             // フィルター
            Color,              // カラー表示
            ViewMode,           // 表示モード
        }
    }
}