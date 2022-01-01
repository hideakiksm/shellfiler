using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // クラス：表示モードの追加/編集ダイアログ
    //=========================================================================================
    public partial class FileListViewModeEditDialog : Form {
        // 元の設定全体
        FileListViewModeAutoSetting m_autoSetting;

        // 設定の編集対象のインデックス
        private int m_autoSettingTargetIndex;

        // 編集中のエントリ
        FileListViewModeAutoSetting.ModeEntry m_settingEntry;

        // 設定UIの実装
        private FileListViewModeInitPage.UIImpl m_viewModeUiImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]autoSetting   元の設定全体
        // 　　　　[in]targetIndex   設定の編集対象のインデックス
        // 戻り値：なし
        //=========================================================================================
        public FileListViewModeEditDialog(FileListViewModeAutoSetting autoSetting, int targetIndex) {
            InitializeComponent();
            m_autoSetting = autoSetting;
            m_autoSettingTargetIndex = targetIndex;

            // 設定を決定
            if (targetIndex == -1) {
                string dir = Program.MainWindow.LeftFileListView.FileList.DisplayDirectoryName;
                FileListViewMode viewMode = new FileListViewMode();
                m_settingEntry = new FileListViewModeAutoSetting.ModeEntry(dir, viewMode);
            } else {
                FileListViewModeAutoSetting.ModeEntry setting = autoSetting.FolderSetting[targetIndex];
                m_settingEntry = (FileListViewModeAutoSetting.ModeEntry)(setting.Clone());
            }

            // UIのセットを定義
            m_viewModeUiImpl = new FileListViewModeInitPage.UIImpl(
                                        null, this.radioButtonDetail, this.radioButtonThumb,
                                        this.comboBoxThumbSize, this.comboBoxThumbName, this.panelViewSample);
            m_viewModeUiImpl.ViewModeToUi(m_settingEntry.ViewMode, false);

            // フォルダを設定
            this.textBoxTargetFolder.Text = m_settingEntry.FolderName;
        }

        //=========================================================================================
        // 機　能：OKボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            // 指定フォルダの重複チェック
            string dir = this.textBoxTargetFolder.Text;
            for (int i = 0; i < m_autoSetting.FolderSetting.Count; i++) {
                if (m_autoSettingTargetIndex != i && dir == m_autoSetting.FolderSetting[i].FolderName) {
                    InfoBox.Warning(this, Resources.DlgViewModeAuto_DirAlreadyExist);
                    return;
                }
            }

            // 設定して閉じる
            FileListViewMode viewMode = m_viewModeUiImpl.UIToViewMode();
            m_settingEntry = new FileListViewModeAutoSetting.ModeEntry(dir, viewMode);
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：編集したエントリ
        //=========================================================================================
        public FileListViewModeAutoSetting.ModeEntry ResultSetting {
            get {
                return m_settingEntry;
            }
        }
    }
}
