using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル一覧＞起動時の状態 の設定ページ
    //=========================================================================================
    public partial class FileListInitialPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileListInitialPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;
            this.trackBarSplash.Minimum = Configuration.MIN_SPLASH_WINDOW_WAIT;
            this.trackBarSplash.Maximum = Configuration.MAX_SPLASH_WINDOW_WAIT;
            this.trackBarSplash.LargeChange = 500;
            this.trackBarSplash.TickFrequency = 500;
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
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 左ウィンドウの初期ディレクトリ
            string leftDir = config.InitialDirectoryLeft;
            if (leftDir == "") {
                this.radioButtonLeftPrev.Checked = true;
                this.textBoxLeftFolder.Text = desktop;
            } else {
                this.radioButtonLeftFix.Checked = true;
                this.textBoxLeftFolder.Text = leftDir;
            }
            RadioButtonLeft_CheckedChanged(this, null);

            // 右ウィンドウの初期ディレクトリ
            string rightDir = config.InitialDirectoryRight;
            if (rightDir == "") {
                this.radioButtonRightPrev.Checked = true;
                this.textBoxRightFolder.Text = desktop;
            } else {
                this.radioButtonRightFix.Checked = true;
                this.textBoxRightFolder.Text = rightDir;
            }
            RadioButtonRight_CheckedChanged(this, null);

            // メインウィンドウのデフォルトサイズ
            Rectangle wndRect = config.MainWindowRectDefault;
            if (wndRect == Rectangle.Empty) {
                this.radioButtonWndPrev.Checked = true;
                this.numericWndX1.Value = 32;
                this.numericWndY1.Value = 16;
                this.numericWndX2.Value = 32 + 600;
                this.numericWndY2.Value = 16 + 450;
            } else {
                this.radioButtonWndFix.Checked = true;
                this.numericWndX1.Value = wndRect.Left;
                this.numericWndY1.Value = wndRect.Top;
                this.numericWndX2.Value = wndRect.Right;
                this.numericWndY2.Value = wndRect.Bottom;
            }
            RadioButtonWnd_CheckedChanged(this, null);

            // スプラッシュウィンドウでの待ち時間
            this.trackBarSplash.Value = config.SplashWindowWait;
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // 左ウィンドウの初期ディレクトリ
            string leftDir;
            if (this.radioButtonLeftPrev.Checked) {
                leftDir = "";
            } else {
                leftDir = this.textBoxLeftFolder.Text;
                success = Configuration.CheckInitialDirectoryLeft(ref leftDir, m_parent);
                if (!success) {
                    return false;
                }
            }

            // 右ウィンドウの初期ディレクトリ
            string rightDir;
            if (this.radioButtonRightPrev.Checked) {
                rightDir = "";
            } else {
                rightDir = this.textBoxRightFolder.Text;
                success = Configuration.CheckInitialDirectoryLeft(ref rightDir, m_parent);
                if (!success) {
                    return false;
                }
            }

            // ウィンドウの初期サイズ
            Rectangle rect;
            if (this.radioButtonWndPrev.Checked) {
                rect = Rectangle.Empty;
            } else {
                int x1 = (int)(this.numericWndX1.Value);
                int y1 = (int)(this.numericWndY1.Value);
                int x2 = (int)(this.numericWndX2.Value);
                int y2 = (int)(this.numericWndY2.Value);
                rect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                success = Configuration.CheckMainWindowRectDefault(ref rect, m_parent);
                if (!success) {
                    return false;
                }
            }

            // スプラッシュウィンドウでの待ち時間
            int splash = this.trackBarSplash.Value;
            Configuration.ModifySplashWindowWait(ref splash);

            // Configに反映
            Configuration.Current.InitialDirectoryLeft = leftDir;
            Configuration.Current.InitialDirectoryRight = rightDir;
            Configuration.Current.MainWindowRectDefault = rect;
            Configuration.Current.SplashWindowWait = splash;

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
        // 機　能：左ウィンドウのフォルダのラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonLeft_CheckedChanged(object sender, EventArgs evt) {
            if (this.radioButtonLeftPrev.Checked) {
                this.textBoxLeftFolder.Enabled = false;
                this.buttonLeftRef.Enabled = false;
            } else {
                this.textBoxLeftFolder.Enabled = true;
                this.buttonLeftRef.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：左ウィンドウのフォルダの参照ボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonLeftRef_Click(object sender, EventArgs evt) {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = this.textBoxLeftFolder.Text;
            DialogResult dr = fbd.ShowDialog(this);
            if (dr == DialogResult.OK) {
                this.textBoxLeftFolder.Text = fbd.SelectedPath;
            }
        }

        //=========================================================================================
        // 機　能：右ウィンドウのフォルダのラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonRight_CheckedChanged(object sender, EventArgs evt) {
            if (this.radioButtonRightPrev.Checked) {
                this.textBoxRightFolder.Enabled = false;
                this.buttonRightRef.Enabled = false;
            } else {
                this.textBoxRightFolder.Enabled = true;
                this.buttonRightRef.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：右ウィンドウのフォルダの参照ボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonRightRef_Click(object sender, EventArgs evt) {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = this.textBoxRightFolder.Text;
            DialogResult dr = fbd.ShowDialog(this);
            if (dr == DialogResult.OK) {
                this.textBoxRightFolder.Text = fbd.SelectedPath;
            }
        }

        //=========================================================================================
        // 機　能：ウィンドウ初期位置のラジオボタンが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButtonWnd_CheckedChanged(object sender, EventArgs evt) {
            if (this.radioButtonWndPrev.Checked) {
                this.numericWndX1.Enabled = false;
                this.numericWndY1.Enabled = false;
                this.numericWndX2.Enabled = false;
                this.numericWndY2.Enabled = false;
                this.buttonWndRef.Enabled = false;
            } else {
                this.numericWndX1.Enabled = true;
                this.numericWndY1.Enabled = true;
                this.numericWndX2.Enabled = true;
                this.numericWndY2.Enabled = true;
                this.buttonWndRef.Enabled = true;
            }
        }

        //=========================================================================================
        // 機　能：ウィンドウ初期位置の参照ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonWndRef_Click(object sender, EventArgs evt) {
            // UIの値を取得
            int x1 = (int)(this.numericWndX1.Value);
            int y1 = (int)(this.numericWndY1.Value);
            int x2 = (int)(this.numericWndX2.Value);
            int y2 = (int)(this.numericWndY2.Value);

            // フォームで大きさを指定
            WindowSizeInputForm testForm = new WindowSizeInputForm();
            testForm.Location = new Point(x1, y1);
            testForm.Size = new Size(x2 - x1, y2 - y1);
            testForm.MinimumSize = new Size(MainWindowForm.FILE_WINDOW_MIN_CX, MainWindowForm.FILE_WINDOW_MIN_CY);
            DialogResult result = testForm.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }

            // UIに反映
            this.numericWndX1.Value = testForm.Left;
            this.numericWndY1.Value = testForm.Top;
            this.numericWndX2.Value = testForm.Right;
            this.numericWndY2.Value = testForm.Bottom;
        }
    }
}
