using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：初期化確認ダイアログ
    // 　　　　DialogResultによる戻り値は以下の通り
    // 　　　　Yes:はい  No:いいえ  Ignore:すべて  Cancel:キャンセル
    //=========================================================================================
    public partial class KeySettingResetConfirmDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public KeySettingResetConfirmDialog() {
            InitializeComponent();
            this.pictureBoxIcon.Image = SystemIcons.Warning.ToBitmap();
        }

        //=========================================================================================
        // 機　能：キー設定ダイアログ用にメッセージを作成する
        // 引　数：[in]scene      キー設定のシーン
        // 戻り値：なし
        //=========================================================================================
        public void InitializeForKeySetting(CommandUsingSceneType scene) {
            string strScene = "";
            string strSceneOther1 = "";
            string strSceneOther2 = "";
            switch (scene) {
                case CommandUsingSceneType.FileList:
                    strScene = Resources.DlgKeySettingOption_SceneFileList;
                    strSceneOther1 = Resources.DlgKeySettingOption_SceneFileViewer;
                    strSceneOther2 = Resources.DlgKeySettingOption_SceneGraphicsViewer;
                    break;
                case CommandUsingSceneType.FileViewer:
                    strScene = Resources.DlgKeySettingOption_SceneFileViewer;
                    strSceneOther1 = Resources.DlgKeySettingOption_SceneFileList;
                    strSceneOther2 = Resources.DlgKeySettingOption_SceneGraphicsViewer;
                    break;
                case CommandUsingSceneType.GraphicsViewer:
                    strScene = Resources.DlgKeySettingOption_SceneGraphicsViewer;
                    strSceneOther1 = Resources.DlgKeySettingOption_SceneFileList;
                    strSceneOther2 = Resources.DlgKeySettingOption_SceneFileViewer;
                    break;
            }
            this.labelMessage.Text = string.Format(this.labelMessage.Text, strScene);
            this.labelMessage2.Text = string.Format(this.labelMessage2.Text, strSceneOther1, strSceneOther2);
        }

        //=========================================================================================
        // 機　能：関連付け設定ダイアログ用にメッセージを作成する
        // 引　数：[in]scene      キー設定のシーン
        // 戻り値：なし
        //=========================================================================================
        public void InitializeForAssociate() {
            this.labelMessage.Text = string.Format(this.labelMessage.Text, Resources.DlgKeySettingOption_SceneAssociate1);
            this.labelMessage2.Text = Resources.DlgKeySettingOption_SceneAssociate2;
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (this.textBoxCommand.Text != "reset") {
                InfoBox.Warning(this, Resources.DlgKeySettingReset_InvalidResetCommand);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
