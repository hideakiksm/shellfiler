using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Util;
using ShellFiler.FileViewer;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // クラス：オプションダイアログ
    //=========================================================================================
    public partial class OptionSettingDialog : Form {
        // コンフィグの初期状態
        private Configuration m_originalConfig;

        // オプションページの構成
        private OptionStructure m_optionStructure;

        // ツリーノードの一覧
        private List<TreeNode> m_nodeList= new List<TreeNode>();

        // 現在表示中のUI
        private OptionStructureItem m_currentUIItem = null;

        // 入力が確定状態のときtrue
        private bool m_dialogSuccess = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public OptionSettingDialog() {
            InitializeComponent();

            m_optionStructure = new OptionStructure();
            m_optionStructure.AddOptionPage(0, "Install",               Resources.OptionItem10_Install,               typeof(InstallWorkingDirPage));           // インストール情報
            m_optionStructure.AddOptionPage(1, "InstallWorkingDir",     Resources.OptionItem11_InstallWorkingDir,     typeof(InstallWorkingDirPage));           // 作業フォルダ
            m_optionStructure.AddOptionPage(1, "InstallEditor",         Resources.OptionItem12_InstallEditor,         typeof(InstallEditorPage));               // エディタ
            m_optionStructure.AddOptionPage(1, "InstallDiff",           Resources.OptionItem13_InstallDiff,           typeof(InstallDiffPage));                 // 差分表示ツール
            m_optionStructure.AddOptionPage(0, "FileList",              Resources.OptionItem20_FileList,              typeof(FileListGeneralPage));             // ファイル一覧
            m_optionStructure.AddOptionPage(1, "FileListGeneral",       Resources.OptionItem21_FileListGeneral,       typeof(FileListGeneralPage));             // 全般
            m_optionStructure.AddOptionPage(1, "FileListInitial",       Resources.OptionItem22_FileListInitial,       typeof(FileListInitialPage));             // 起動時の状態
            m_optionStructure.AddOptionPage(1, "FileListSort",          Resources.OptionItem23_FileListSort,          typeof(FileListSortPage));                // 起動時の一覧
            m_optionStructure.AddOptionPage(1, "FileListInitVMode",     Resources.OptionItem24_FileListInitVMode,     typeof(FileListViewModeInitPage));        // 起動時の表示モード
            m_optionStructure.AddOptionPage(1, "FileListVMode",         Resources.OptionItem25_FileListVMode,         typeof(FileListViewModePage));            // 表示モード
            m_optionStructure.AddOptionPage(1, "FileListCompare",       Resources.OptionItem26_FileListCompare,       typeof(FileListComparePage));             // 一覧の比較
            m_optionStructure.AddOptionPage(1, "FileListFolderSize",    Resources.OptionItem27_FileListFolderSize,    typeof(FileListFolderSizePage));          // フォルダサイズ
            m_optionStructure.AddOptionPage(1, "FileListAction",        Resources.OptionItem28_FileListAction,        typeof(FileListActionPage));              // 動作
            m_optionStructure.AddOptionPage(0, "FileOpr",               Resources.OptionItem30_FileOpr,               typeof(FileOperationGeneralPage));        // ファイル操作
            m_optionStructure.AddOptionPage(1, "FileOprGeneral",        Resources.OptionItem31_FileOprGeneral,        typeof(FileOperationGeneralPage));        // 全般
            m_optionStructure.AddOptionPage(1, "FileOprCopyAttr",       Resources.OptionItem32_FileOprCopyAttr,       typeof(FileOperationCopyAttributePage));  // 属性のコピー
            m_optionStructure.AddOptionPage(1, "FileOprMarkless",       Resources.OptionItem33_FileOprMarkless,       typeof(FileOperationMarklessPage));       // マークなし操作
            m_optionStructure.AddOptionPage(1, "FileOprEtc",            Resources.OptionItem34_FileOprEtc,            typeof(FileOperationEtcPage));            // 各種操作
            m_optionStructure.AddOptionPage(1, "FileOprTransfer",       Resources.OptionItem35_FileOprTransfer,       typeof(FileOperationTransferPage));       // 転送と削除
            m_optionStructure.AddOptionPage(1, "FileOprClip",           Resources.OptionItem36_FileOprClip,           typeof(FileOperationClipboardPage));      // クリップボード
            m_optionStructure.AddOptionPage(1, "FileOprArchive",        Resources.OptionItem37_FileOprArchive,        typeof(FileOperationArchivePage));        // 圧縮
            m_optionStructure.AddOptionPage(1, "FileOprExtract",        Resources.OptionItem38_FileOprExtract,        typeof(FileOperationExtractPage));        // 展開
            m_optionStructure.AddOptionPage(0, "SSH",                   Resources.OptionItem40_SSH,                   typeof(SSHGeneralPage));                  // SSH
            m_optionStructure.AddOptionPage(1, "SSHGeneral",            Resources.OptionItem41_SSHGeneral,            typeof(SSHGeneralPage));                  // 全般
            m_optionStructure.AddOptionPage(1, "SSHTerminal",           Resources.OptionItem42_SSHTerminal,           typeof(SSHTerminalPage));                 // ターミナル
            m_optionStructure.AddOptionPage(1, "SSHTerminalLog",        Resources.OptionItem43_SSHTerminalLog,        typeof(SSHTerminalLogPage));              // ターミナルログ
            m_optionStructure.AddOptionPage(0, "Privacy",               Resources.OptionItem50_Privacy,               typeof(PrivacyGeneralPage));              // プライバシー
            m_optionStructure.AddOptionPage(1, "PrivacyGeneral",        Resources.OptionItem51_PrivacyGeneral,        typeof(PrivacyGeneralPage));              // 全般
            m_optionStructure.AddOptionPage(1, "PrivacyFolder",         Resources.OptionItem52_PrivacyFolder,         typeof(PrivacyFolderPage));               // フォルダ
            m_optionStructure.AddOptionPage(1, "PrivacyFileViewer",     Resources.OptionItem53_PrivacyFileViewer,     typeof(PrivacyFileViewerPage));           // ファイルビューア
            m_optionStructure.AddOptionPage(1, "PrivacyCommand",        Resources.OptionItem54_PrivacyCommand,        typeof(PrivacyCommandLinePage));          // コマンド
            m_optionStructure.AddOptionPage(0, "TextViewer",            Resources.OptionItem60_TextViewer,            typeof(TextViewerGeneralPage));           // テキストビューア
            m_optionStructure.AddOptionPage(1, "TextViewerGeneral",     Resources.OptionItem61_TextViewerGeneral,     typeof(TextViewerGeneralPage));           // 全般
            m_optionStructure.AddOptionPage(1, "TextViewerVier",        Resources.OptionItem62_TextViewerVier,        typeof(TextViewerViewPage));              // 表示
            m_optionStructure.AddOptionPage(1, "TextViewerLineBreak",   Resources.OptionItem63_TextViewerLineBreak,   typeof(TextViewerLineBreakPage));         // 折返しとタブ
            m_optionStructure.AddOptionPage(1, "TextViewerSearch",      Resources.OptionItem64_TextViewerSearch,      typeof(TextViewerSearchPage));            // 検索オプション
            m_optionStructure.AddOptionPage(1, "TextViewerClipText",    Resources.OptionItem65_TextViewerClipText,    typeof(TextViewerTextClipboardPage));     // クリップボード(テキスト)
            m_optionStructure.AddOptionPage(1, "TextViewerClipDump",    Resources.OptionItem66_TextViewerClipDump,    typeof(TextViewerDumpClipboardPage));     // クリップボード(ダンプ)
            m_optionStructure.AddOptionPage(0, "GraphicsViewer",        Resources.OptionItem70_GraphicsViewer,        typeof(GraphicsViewerGeneralPage));       // グラフィックビューア
            m_optionStructure.AddOptionPage(1, "GraphicsViewerGeneral", Resources.OptionItem71_GraphicsViewerGeneral, typeof(GraphicsViewerGeneralPage));       // 全般
            m_optionStructure.AddOptionPage(1, "GraphicsViewerView",    Resources.OptionItem72_GraphicsViewerView,    typeof(GraphicsViewerViewPage));          // 拡大表示
            m_optionStructure.AddOptionPage(0, "Func",                  Resources.OptionItem80_Func,                  typeof(FuncGeneralPage));                 // ファンクションキー
            m_optionStructure.AddOptionPage(1, "FuncGeneral",           Resources.OptionItem81_FuncGeneral,           typeof(FuncGeneralPage));                 // 全般
            m_optionStructure.AddOptionPage(0, "Log",                   Resources.OptionItem90_Log,                   typeof(LogGeneralPage));                  // ログ
            m_optionStructure.AddOptionPage(1, "LogGeneral",            Resources.OptionItem91_LogGeneral,            typeof(LogGeneralPage));                  // 全般
//          m_optionStructure.AddOptionPage(0, "Color",                 Resources.OptionItemA0_Color,                 typeof(InstallGeneralPage));              // 色
//          m_optionStructure.AddOptionPage(1, "ColorGeneral",          Resources.OptionItemA1_ColorGeneral,          typeof(InstallGeneralPage));              // 全般
//          m_optionStructure.AddOptionPage(1, "ColorFileList",         Resources.OptionItemA2_ColorFileList,         typeof(InstallGeneralPage));              // ファイルリスト
//          m_optionStructure.AddOptionPage(1, "ColorTextViewer",       Resources.OptionItemA3_ColorTextViewer,       typeof(InstallGeneralPage));              // テキストビューア
//          m_optionStructure.AddOptionPage(1, "ColorState",            Resources.OptionItemA4_ColorState,            typeof(InstallGeneralPage));              // 状態一覧パネル
//          m_optionStructure.AddOptionPage(1, "ColorLog",              Resources.OptionItemA5_ColorLog,              typeof(InstallGeneralPage));              // ログ
//          m_optionStructure.AddOptionPage(1, "ColorFont",             Resources.OptionItemA6_ColorFont,             typeof(InstallGeneralPage));              // フォント

            // はじめのページを用意
            string lastId = Configuration.OptionSettingPageLast;
            OptionStructureItem pageItem = m_optionStructure.GetPageItemFromId(lastId);
            object[] args = { this };
            UserControl ui = (UserControl)(pageItem.UIImplType.InvokeMember(null, BindingFlags.CreateInstance, null, null, args));
            this.panelSetting.Controls.Add(ui);
            m_currentUIItem = pageItem;

#if FREE_VERSION
            // Freeware版
            this.labelFreeware.Text = Resources.Dlg_FreewareInfo;
            this.labelFreeware.BackColor = Color.LightYellow;
#endif

            // ツリーの項目を登録
            TreeNode prevRoot = null;
            TreeNode selectedNode = null;
            foreach (OptionStructureItem item in m_optionStructure.PageList) {
                TreeNode node = new TreeNode(item.DisplayName);
                node.Tag = item;
                if (item.Depth == 0) {
                    this.treeViewItems.Nodes.Add(node);
                    prevRoot = node;
                } else {
                    prevRoot.Nodes.Add(node);
                }
                m_nodeList.Add(node);
                if (m_currentUIItem == item) {
                    selectedNode = node;
                }
            }
            this.treeViewItems.ExpandAll();
            if (selectedNode != null) {
                this.treeViewItems.SelectedNode = selectedNode;
            }
            EnableUIItem();

            m_originalConfig = (Configuration)(Configuration.Current.Clone());
        }

        //=========================================================================================
        // 機　能：UIの項目の有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            bool enabled = true;
            string currentId = m_currentUIItem.Id;
            if (currentId == "Privacy" || currentId == "PrivacyGeneral") {
                enabled = false;
            }
            this.buttonReset.Enabled = enabled;
        }

        //=========================================================================================
        // 機　能：ツリーの項目が変更されようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewItems_BeforeSelect(object sender, TreeViewCancelEventArgs evt) {
            OptionStructureItem targetItem = (OptionStructureItem)(evt.Node.Tag);
            bool changed = ChangePage(targetItem, false);
            if (!changed) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：設定ページを切り替える
        // 引　数：[in]targetItem   イベントの送信元
        // 　　　　[in]changeTree   ツリーの選択も更新するときtrue
        // 戻り値：ページを切り替えたときtrue
        //=========================================================================================
        public bool ChangePage(OptionStructureItem targetItem, bool changeTree) {
            Type targetUi = targetItem.UIImplType;
            Type currentUi = m_currentUIItem.UIImplType;

            // UIが異なっていれば切り替え
            if (targetUi != currentUi) {
                if (this.panelSetting.Controls.Count > 0) {
                    // 既存ページを保存して破棄
                    IOptionDialogPage dialogPage = (IOptionDialogPage)(this.panelSetting.Controls[0]);
                    bool ok = dialogPage.GetUIValue();
                    if (!ok) {
                        return false;
                    }
                    UpdateConfig();
                    m_originalConfig = (Configuration)(Configuration.Current.Clone());

                    foreach (Control control in this.panelSetting.Controls) {
                        control.Dispose();
                    }
                    this.panelSetting.Controls.Clear();
                    dialogPage.OnFormClosed();
                }
                // 新しいページを設定
                object[] args = { this };
                UserControl ui = (UserControl)(targetItem.UIImplType.InvokeMember(null, BindingFlags.CreateInstance, null, null, args));
                this.panelSetting.Controls.Add(ui);
                m_currentUIItem = targetItem;
                EnableUIItem();
            }

            // ノードを変更
            if (changeTree) {
                for (int i = m_nodeList.Count - 1; i >= 0; i--) {
                    OptionStructureItem nodeItem = (OptionStructureItem)(m_nodeList[i].Tag);
                    if (nodeItem == targetItem) {
                        this.treeViewItems.SelectedNode = m_nodeList[i];
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：デフォルトに戻すボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonReset_Click(object sender, EventArgs evt) {
            if (this.panelSetting.Controls.Count == 0) {
                return;
            }
            DialogResult result = InfoBox.Question(Program.MainWindow, MessageBoxButtons.YesNo, Resources.Option_ResetSetting);
            if (result != DialogResult.Yes) {
                return;
            }
            IOptionDialogPage dialogPage = (IOptionDialogPage)(this.panelSetting.Controls[0]);
            dialogPage.ResetDefault();
        }

        //=========================================================================================
        // 機　能：ノードが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void treeViewItems_BeforeCollapse(object sender, TreeViewCancelEventArgs evt) {
            evt.Cancel = true;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            m_dialogSuccess = false;
            if (this.panelSetting.Controls.Count == 0) {
                return;
            }
            IOptionDialogPage dialogPage = (IOptionDialogPage)(this.panelSetting.Controls[0]);
            bool ok = dialogPage.GetUIValue();
            if (!ok) {
                return;
            }
            UpdateConfig();
            m_dialogSuccess = true;
            m_originalConfig = (Configuration)(Configuration.Current.Clone());
            DialogResult = DialogResult.OK;
            Configuration.OptionSettingPageLast = m_currentUIItem.Id;
            Close();
        }

        //=========================================================================================
        // 機　能：フォームが閉じられようとしているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void OptionSettingDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            // 入力エラーではフォームを閉じない
            if (DialogResult == DialogResult.OK && !m_dialogSuccess) {
                evt.Cancel = true;
            }
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void OptionSettingDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            if (this.panelSetting.Controls.Count > 0) {
                // 既存ページを保存して破棄
                IOptionDialogPage dialogPage = (IOptionDialogPage)(this.panelSetting.Controls[0]);
                dialogPage.OnFormClosed();
            }
        }

        //=========================================================================================
        // 機　能：コンフィグの値をUIに反映する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateConfig() {
            Configuration prev = m_originalConfig;
            Configuration now = Configuration.Current;
            // ファイル一覧を再描画する
            if (prev.FileListSeparateExt != now.FileListSeparateExt) {
                Program.MainWindow.LeftFileListView.Invalidate();
                Program.MainWindow.RightFileListView.Invalidate();
            }
            
            // テキストファイルビューアを再描画する
            if (prev.TextViewerIsDisplayCtrlChar != now.TextViewerIsDisplayCtrlChar) {
                foreach (FileViewerForm viewer in Program.WindowManager.FileViewerList) {
                    viewer.Redraw();
                }
            }

            // ファンクションバーを再描画する
            if (prev.FunctionBarSplitCount != now.FunctionBarSplitCount ||
                prev.FunctionBarUseOverrayIcon != now.FunctionBarUseOverrayIcon) {
                Program.MainWindow.FunctionBar.OnSizeChanged();
                Program.MainWindow.FunctionBar.Invalidate();
                foreach (FileViewerForm viewer in Program.WindowManager.FileViewerList) {
                    viewer.FunctionBar.OnSizeChanged();
                    viewer.FunctionBar.Invalidate();
                }
            }
        }

        //=========================================================================================
        // プロパティ：オプションページの構成
        //=========================================================================================
        public OptionStructure OptionStructure {
            get {
                return m_optionStructure;
            }
        }
    }
}
