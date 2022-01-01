using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：ファイル一覧、アドレスバー、ステータスバーを埋め込むためのWindows UIパネル
    //=========================================================================================
    public partial class FilePanel : Panel {
        // 反対ウィンドウ
        private FilePanel m_opposite;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FilePanel() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]mainWindowForm メインウィンドウ
        // 　　　　[in]tabPageInfo    読み込んだタブページの情報
        // 　　　　[in]isLeft         左側ウィンドウのときtrue
        // 　　　　[in]opposite       反対側ウィンドウ
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(MainWindowForm mainWindowForm, TabPageInfo tabPageInfo, bool isLeft, FilePanel opposite) {
            m_opposite = opposite;
            this.fileViewStatus.Initialize();
            UIFileList fileList = tabPageInfo.GetFileList(isLeft);
            IFileListViewState viewState = tabPageInfo.GetFileListViewState(isLeft);

            this.addressBar.Initialize(isLeft, this.fileListView);
            this.fileViewStatus.Initialize(isLeft, fileList);

            this.fileListView.Initialize(mainWindowForm, m_opposite.fileListView, this, isLeft, fileList, viewState);
            this.fileViewStatus.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(Program.MainWindow.MainWindowForm_PreviewKeyDown);
            this.fileViewStatus.KeyDown += new System.Windows.Forms.KeyEventHandler(Program.MainWindow.MainWindowForm_KeyDown);
            this.fileViewStatus.KeyUp += new System.Windows.Forms.KeyEventHandler(Program.MainWindow.MainWindowForm_KeyUp);
            this.fileListView.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(Program.MainWindow.MainWindowForm_PreviewKeyDown);
            this.fileListView.KeyDown += new System.Windows.Forms.KeyEventHandler(Program.MainWindow.MainWindowForm_KeyDown);
            this.fileListView.KeyUp += new System.Windows.Forms.KeyEventHandler(Program.MainWindow.MainWindowForm_KeyUp);
        }

        //=========================================================================================
        // 機　能：特殊キーかどうかを判断する
        // 引　数：[in]keyData 判断するキー
        // 戻り値：通常のキーとするときtrue
        //=========================================================================================
        protected override bool IsInputKey(Keys keyData) {
            return true;
        }

        //=========================================================================================
        // 機　能：ファイル一覧を無効にする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void InvalidateView() {
            this.fileListView.Invalidate();
        }

        //=========================================================================================
        // 機　能：エラーメッセージを更新する
        // 引　数：[in]message   エラーメッセージ
        // 　　　　[in]level     エラーのレベル
        // 　　　　[in]icon      使用するアイコン
        // 戻り値：なし
        //=========================================================================================
        public void ShowErrorMessage(string message, FileOperationStatus.LogLevel level, IconImageListID icon) {
            this.fileViewStatus.ShowErrorMessage(message, level, icon);
            this.fileListView.ShowVisualBell();
        }

        //=========================================================================================
        // プロパティ：反対ウィンドウ
        //=========================================================================================
        public FilePanel Opposite {
            get {
                return m_opposite;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルリストのビュー
        //=========================================================================================
        public FileListView FileListView {
            get {
                return this.fileListView;
            }
        }

        //=========================================================================================
        // プロパティ：アドレスバー
        //=========================================================================================
        public AddressBarStrip AddressBar {
            get {
                return this.addressBar;
            }
        }

        //=========================================================================================
        // プロパティ：ステータスバー
        //=========================================================================================
        public FileStatusBar StatusBar {
            get {
                return this.fileViewStatus;
            }
        }
    }
}
