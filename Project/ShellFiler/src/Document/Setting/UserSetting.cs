using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileTask;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Dialog;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.FileSystem;
using ShellFiler.FileViewer.HTTPResponseViewer;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ユーザーの入力した設定全般
    //=========================================================================================
    class UserSetting {
        // 一般設定
        private InitialSetting m_initialSetting = null;

        // コマンドヒストリ
        private CommandHistory m_commandHistory = null;
        
        // ブックマーク
        private BookmarkSetting m_bookmarkSetting = null;
        
        // アーカイブ内の自動パスワード入力の設定
        private ArchiveAutoPasswordSetting m_archiveAutoPasswordSetting = null;

        // ファイルビュアーでの検索文字列ヒストリ
        private ViewerSearchHistory m_viewerSearchHistory = null;
        
        // HTTPレスポンスビューアの初期値
        private ResponseViewerRequestSetting m_responseViewerDefaultSetting = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public UserSetting() {
            m_initialSetting = new InitialSetting();
            m_commandHistory = new CommandHistory();
            m_bookmarkSetting = new BookmarkSetting();
            m_archiveAutoPasswordSetting = new ArchiveAutoPasswordSetting();
            m_viewerSearchHistory = new ViewerSearchHistory();
            m_responseViewerDefaultSetting = ResponseViewerRequestSetting.GetDefaultValue();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
#if !FREE_VERSION
            string fileName = DirectoryManager.InitialSetting;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = InitialSetting.LoadSetting(loader, m_initialSetting);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSetting, fileName, loader.ErrorDetail);
            }
#endif
            m_bookmarkSetting.SetDefaultSetting();
        }
        
        //=========================================================================================
        // 機　能：設定の保存を行う
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：メインウィンドウのクローズ処理中に呼ばれる
        //=========================================================================================
        public void SaveSetting() {
#if !FREE_VERSION
            string fileName = DirectoryManager.InitialSetting;
            SettingSaver saver = new SettingSaver(fileName);
            bool success = InitialSetting.SaveSetting(saver, m_initialSetting, Program.MainWindow, Program.Document.CurrentTabPage.LeftFileList, Program.Document.CurrentTabPage.RightFileList);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotSaveSetting, fileName);
            }
#endif
        }

        //=========================================================================================
        // プロパティ：初期設定
        //=========================================================================================
        public InitialSetting InitialSetting {
            get {
                return m_initialSetting;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドヒストリ
        //=========================================================================================
        public CommandHistory CommandHistory {
            get {
                return m_commandHistory;
            }
        }

        //=========================================================================================
        // プロパティ：登録ディレクトリの情報
        //=========================================================================================
        public BookmarkSetting BookmarkSetting {
            get {
                return m_bookmarkSetting;
            }
            set {
                m_bookmarkSetting = value;
            }
        }
        
        //=========================================================================================        
        // プロパティ：アーカイブ内の自動パスワード入力の設定
        //=========================================================================================
        public ArchiveAutoPasswordSetting ArchiveAutoPasswordSetting {
            get {
                return m_archiveAutoPasswordSetting;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルビュアーでの検索文字列ヒストリ
        //=========================================================================================
        public ViewerSearchHistory ViewerSearchHistory {
            get {
                return m_viewerSearchHistory;
            }
        }

        //=========================================================================================
        // プロパティ：HTTPレスポンスビューアの初期値
        //=========================================================================================
        public ResponseViewerRequestSetting ResponseViewerDefaultSetting {
            get {
                return m_responseViewerDefaultSetting;
            }
        }
    }
}
