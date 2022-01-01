using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.UI.ControlBar;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：アドレスバーの本体
    //=========================================================================================
    public partial class AddressBarStrip : ToolStrip {
        // コンボボックスの最小幅
        public const int CX_MIN_COMBO_BOX = 50;

        // コンボボックスの高さ
        public const int CY_COMBO_BOX = 25;

        // 左ウィンドウのときtrue
        private bool m_isLeft;

        // このアドレスバーに対応するファイル一覧
        private FileListView m_fileListView;

        // ディレクトリ表示用のコンボボックス
        private AddressBarDropDown m_addressBarDropDown;

        // ツールバーの実装
        private ToolBarImpl m_toolbarImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AddressBarStrip() {
            InitializeComponent();
        }
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]isLeft  左ウィンドウのときtrue
        // 　　　　[in]view    対応するファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(bool isLeft, FileListView view) {
            m_isLeft = isLeft;
            m_fileListView = view;
            
            if (isLeft) {
                m_toolbarImpl = new ToolBarImpl(this, UICommandSender.AddressBarLeft, Program.MainWindow);
            } else {
                m_toolbarImpl = new ToolBarImpl(this, UICommandSender.AddressBarRight, Program.MainWindow);
            }
            Program.MainWindow.CursorLRChanged += new MainWindowForm.CursorLRChangedHandler(MainWindowForm_CursorLRChanged);

            // コンボボックス
            List<ToolbarItemSetting> itemSettingList = Program.Document.ToolbarSetting.AddressBarItemList;
            int cxComboBox = ClientRectangle.Width - itemSettingList.Count * ToolbarItemSetting.CX_ICON_BUTTON;
            m_addressBarDropDown = new AddressBarDropDown(isLeft, view, this);
            m_toolbarImpl.AddExternalItem(m_addressBarDropDown);

            // アイコン
            KeyItemSettingList keySetting = Program.Document.KeySetting.FileListKeyItemList;
            m_toolbarImpl.AddButtonsFromSetting(itemSettingList, keySetting, null);
        }


        //=========================================================================================
        // 機　能：ディレクトリ情報を設定する
        // 引　数：[in]directory  ディレクトリ名
        // 　　　　[in]separator  ディレクトリの区切り文字
        // 　　　　[in]filter     ファイル一覧のフィルター（フィルターを使用しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void SetDirectoryName(string directory, string separator, FileListFilterMode filter) {
            // ツールチップを作成
            ChdirInputCommand inputCommand = new ChdirInputCommand();
            string hint = inputCommand.UIResource.Hint;
            KeyItemSettingList keySetting = Program.Document.KeySetting.FileListKeyItemList;
            string shortcut = ToolBarImpl.CreateShortcutDisplayString(keySetting, new ActionCommandMoniker(ActionCommandOption.None, typeof(ChdirInputCommand)));
            if (shortcut != null) {
                hint = hint + "(" + shortcut + ")";
            }

            // アドレスバーを作成
            m_addressBarDropDown.SetDirectoryName(directory, separator, filter);
            m_addressBarDropDown.ToolTipText = hint;
        }

        //=========================================================================================
        // 機　能：ツールバーのステータス状態を更新する
        // 引　数：[in]context   ツールバーの更新状態のコンテキスト
        // 戻り値：なし
        //=========================================================================================
        public void RefreshToolbarStatus(UIItemRefreshContext context) {
            if (m_toolbarImpl == null) {
                return;
            }
            m_toolbarImpl.RefreshToolbarStatus(context);
        }

        //=========================================================================================
        // 機　能：レイアウトが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void AddressBarStrip_Layout(object sender, LayoutEventArgs evt) {
            if (Program.Document == null || m_toolbarImpl == null) {
                return;         // デザイナーからの呼び出し
            }

            const int CX_COMBO_BOX_MARGIN = 8;
            int cxComboBox = DisplayRectangle.Width - CX_COMBO_BOX_MARGIN;
            cxComboBox -= m_toolbarImpl.ButtonWidth;
            cxComboBox = Math.Max(CX_MIN_COMBO_BOX, cxComboBox);
            m_addressBarDropDown.SetSize(cxComboBox, CY_COMBO_BOX);
        }

        //=========================================================================================
        // 機　能：項目がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void AddressBarStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs evt) {
            // カーソルをこちらへ移動
            if (m_isLeft != Program.Document.CurrentTabPage.IsCursorLeft) {
                Program.MainWindow.ToggleCursorLeftRight();
            }
        }

        //=========================================================================================
        // 機　能：マウスがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void AddressBarStrip_MouseClick(object sender, MouseEventArgs evt) {
            // カーソルをこちらへ移動
            if (m_isLeft != Program.Document.CurrentTabPage.IsCursorLeft) {
                Program.MainWindow.ToggleCursorLeftRight();
            }
        }

        //=========================================================================================
        // 機　能：メインウィンドウのカーソルの左右に変化が生じたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainWindowForm_CursorLRChanged(object sender, EventArgs evt) {
            m_toolbarImpl.OnCursorLRChanged(Program.Document.CurrentTabPage.IsCursorLeft);
        }

        //=========================================================================================
        // プロパティ：ディレクトリ表示用のコンボボックス
        //=========================================================================================
        public AddressBarDropDown AddressBarDropDown {
            get {
                return m_addressBarDropDown;
            }
        }
    }
}
