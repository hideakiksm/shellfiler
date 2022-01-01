using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.Document;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ShellFiler内のドロップ時の操作入力ダイアログ
    //=========================================================================================
    public partial class DropActionInternalDialog : Form {
        // ダイアログ入力の結果により決まった操作
        private ResultActionType m_resultAction;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileList  ドロップされたファイルの一覧
        // 戻り値：なし
        //=========================================================================================
        public DropActionInternalDialog() {
            InitializeComponent();
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
            m_resultAction = ResultActionType.ChangeDir;
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
