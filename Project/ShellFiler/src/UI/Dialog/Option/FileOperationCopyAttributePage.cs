using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル操作＞属性のコピー の設定ページ
    //=========================================================================================
    public partial class FileOperationCopyAttributePage : UserControl, IOptionDialogPage {
        // マトリクス境界のＸ座標マージン
        const int MX = 10;

        // マトリクス境界のＹ座標マージン
        const int MY = 2;

        // マトリクス境界のＸ座標
        int[] X_MATRIX = new int[] { 0, 130 - MX, 220 - MX, 310 - MX, 400 - MX, 480};

        // マトリクス境界のＹ座標
        int[] Y_MATRIX = new int[] { 0, 20 - MY, 40 - MY, 60 - MY, 90 - MY, 118};

        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileOperationCopyAttributePage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;

            // コンフィグ値をUIに反映
            SetInitialValue(Configuration.Current);
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
        }

        //=========================================================================================
        // 機　能：UIに初期値を設定する
        // 引　数：[in]config  取得対象のコンフィグ
        // 戻り値：なし
        //=========================================================================================
        private void SetInitialValue(Configuration config) {
            AttributeSetMode attrMode = config.TransferAttributeSetMode;
            this.checkBoxWindowsAttr.Checked = attrMode.WindowsSetAttributeAll;
            this.checkBoxSSHAttr.Checked = attrMode.SshSetAtributeAll;
            
            checkBoxWindowsAttr_CheckedChanged(null, null);
            checkBoxSSHAttr_CheckedChanged(null, null);
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            AttributeSetMode attrMode = Configuration.Current.TransferAttributeSetMode;
            attrMode.WindowsSetAttributeAll = this.checkBoxWindowsAttr.Checked;
            attrMode.SshSetAtributeAll = this.checkBoxSSHAttr.Checked;
            return true;
        }

        //=========================================================================================
        // 機　能：ページ内の設定をデフォルトに戻す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetDefault() {
            Configuration org = new Configuration();
            SetInitialValue(org);
        }

        //=========================================================================================
        // 機　能：Windows用パネルの内容を描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void panelWindows_Paint(object sender, PaintEventArgs evt) {
            using (HighDpiGraphics g = new HighDpiGraphics(evt.Graphics)) {
                Brush brush = new SolidBrush(GraphicsUtils.BrendColor(SystemColors.ControlDark, SystemColors.Control, SystemColors.Control));
                try {
                    g.Graphics.FillRectangle(brush, new Rectangle(g.X(X_MATRIX[0]), g.Y(Y_MATRIX[0]), g.X(X_MATRIX[1]), g.Y(Y_MATRIX[5])));
                    g.Graphics.FillRectangle(brush, new Rectangle(g.X(X_MATRIX[0]), g.Y(Y_MATRIX[0]), g.X(X_MATRIX[5]), g.Y(Y_MATRIX[1])));
                } finally {
                    brush.Dispose();
                }
                for (int i = 1; i <= 4; i++) {
                    g.DrawLine(SystemPens.ControlLightLight, X_MATRIX[0], Y_MATRIX[i], X_MATRIX[5], Y_MATRIX[i]);
                }
                for (int i = 1; i <= 4; i++) {
                    g.DrawLine(SystemPens.ControlLightLight, X_MATRIX[i], Y_MATRIX[0], X_MATRIX[i], Y_MATRIX[3]);
                    g.DrawLine(SystemPens.ControlLightLight, X_MATRIX[i], Y_MATRIX[4], X_MATRIX[i], Y_MATRIX[5]);
                }
            }
        }

        //=========================================================================================
        // 機　能：SSH用パネルの内容を描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void panelSSH_Paint(object sender, PaintEventArgs evt) {
            using (HighDpiGraphics g = new HighDpiGraphics(evt.Graphics)) {
                Brush brush = new SolidBrush(GraphicsUtils.BrendColor(SystemColors.ControlDark, SystemColors.Control, SystemColors.Control));
                try {
                    g.Graphics.FillRectangle(brush, new Rectangle(g.X(X_MATRIX[0]), g.Y(Y_MATRIX[0]), g.X(X_MATRIX[1]), g.Y(Y_MATRIX[5])));
                    g.Graphics.FillRectangle(brush, new Rectangle(g.X(X_MATRIX[0]), g.Y(Y_MATRIX[0]), g.X(X_MATRIX[5]), g.Y(Y_MATRIX[1])));
                } finally {
                    brush.Dispose();
                }
                for (int i = 1; i <= 4; i++) {
                    g.DrawLine(SystemPens.ControlLightLight, X_MATRIX[0], Y_MATRIX[i], X_MATRIX[5], Y_MATRIX[i]);
                }
                for (int i = 1; i <= 3; i++) {
                    g.DrawLine(SystemPens.ControlLightLight, X_MATRIX[i], Y_MATRIX[0], X_MATRIX[i], Y_MATRIX[5]);
                }
            }
        }

        //=========================================================================================
        // 機　能：Windowsの属性転送のチェックボックスが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxWindowsAttr_CheckedChanged(object sender, EventArgs evt) {
            string text;
            if (this.checkBoxWindowsAttr.Checked) {
                text = Resources.OptionFileOprAttr_AttrCopyYes;
            } else {
                text = Resources.OptionFileOprAttr_AttrCopyNo;
            }
            this.labelWindows1.Text = text;
            this.labelWindows2.Text = text;
            this.labelWindows3.Text = text;
            this.labelWindows4.Text = text;
        }

        //=========================================================================================
        // 機　能：SSHの属性転送のチェックボックスが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxSSHAttr_CheckedChanged(object sender, EventArgs evt) {
            string text;
            if (this.checkBoxSSHAttr.Checked) {
                text = Resources.OptionFileOprAttr_AttrCopyYes;
            } else {
                text = Resources.OptionFileOprAttr_AttrCopyNo;
            }
            this.labelSSHAttr1.Text = text;
            this.labelSSHAttr2.Text = text;
            this.labelSSHAttr3.Text = text;
            this.labelSSHAttr4.Text = text;
            this.labelSSHAttr5.Text = text;
            this.labelSSHAttr6.Text = text;
        }
    }
}