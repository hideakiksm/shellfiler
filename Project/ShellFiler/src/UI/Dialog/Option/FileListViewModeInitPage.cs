using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.Option {

    //=========================================================================================
    // ファイル一覧＞表示モード の設定ページ
    //=========================================================================================
    public partial class FileListViewModeInitPage : UserControl, IOptionDialogPage {
        // 左ウィンドウ用のUIの実装
        private UIImpl m_uiLeftImpl;

        // 右ウィンドウ用のUIの実装
        private UIImpl m_uiRightImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileListViewModeInitPage(OptionSettingDialog parent) {
            InitializeComponent();

            // UIのセットを定義
            m_uiLeftImpl = new UIImpl(this.radioButtonLeftPrev, this.radioButtonLeftDetail, this.radioButtonLeftThumb,
                                      this.comboBoxLeftThumbSize, this.comboBoxLeftThumbName, this.panelLeftViewSample);
            m_uiRightImpl = new UIImpl(this.radioButtonRightPrev, this.radioButtonRightDetail, this.radioButtonRightThumb,
                                       this.comboBoxRightThumbSize, this.comboBoxRightThumbName, this.panelRightViewSample);

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
            m_uiLeftImpl.ViewModeToUi(config.DefaultViewModeLeft, true);
            m_uiRightImpl.ViewModeToUi(config.DefaultViewModeRight, true);
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            Configuration.Current.DefaultViewModeLeft = m_uiLeftImpl.UIToViewMode();
            Configuration.Current.DefaultViewModeRight = m_uiRightImpl.UIToViewMode();
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
        // クラス：左右で共通の設定項目となっているUIのセット
        //=========================================================================================
        public class UIImpl {
            // 前回値を使用する（UIが存在しないときnull）
            public RadioButton RadioButtonPrev;

            // 詳細モードを使用する
            public RadioButton RadioButtonDetail;

            // サムネイルモードを使用する
            public RadioButton RadioButtonThumb;

            // サイズの選択用
            public ComboBox ComboBoxSize;

            // ファイル名の選択用
            public ComboBox ComboBoxName;

            // サンプル表示用のビュー
            public Panel PanelSample;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]radioButtonPrev   前回値を使用する（UIが存在しないときnull）
            // 　　　　[in]radioButtonDetail 詳細モードを使用する
            // 　　　　[in]radioButtonThumb  サムネイルモードを使用する
            // 　　　　[in]comboBoxSize      サイズの選択用
            // 　　　　[in]comboBoxName      ファイル名の選択用
            // 　　　　[in]panelSample       サンプル表示用のビュー
            // 戻り値：なし
            //=========================================================================================
            public UIImpl(RadioButton radioButtonPrev, RadioButton radioButtonDetail, RadioButton radioButtonThumb, ComboBox comboBoxSize, ComboBox comboBoxName, Panel panelSample) {
                RadioButtonPrev = radioButtonPrev;
                RadioButtonDetail = radioButtonDetail;
                RadioButtonThumb = radioButtonThumb;
                ComboBoxSize = comboBoxSize;
                ComboBoxName = comboBoxName;
                PanelSample = panelSample;

                // コンボボックスの項目一覧を設定
                string[] comboBoxSizeItems = {
                    FileListViewIconSize.IconSize32.DisplayName, 
                    FileListViewIconSize.IconSize48.DisplayName, 
                    FileListViewIconSize.IconSize64.DisplayName, 
                    FileListViewIconSize.IconSize128.DisplayName, 
                    FileListViewIconSize.IconSize256.DisplayName, 
                };
                comboBoxSize.Items.AddRange(comboBoxSizeItems);

                // コンボボックスの項目一覧を設定
                string[] comboBoxNameItems = {
                    FileListViewThumbnailName.ThumbNameSpearate.DisplayName, 
                    FileListViewThumbnailName.ThumbNameOverray.DisplayName, 
                    FileListViewThumbnailName.ThumbNameNone.DisplayName, 
                };
                comboBoxName.Items.AddRange(comboBoxNameItems);

                // イベントを接続
                if (radioButtonPrev != null) {
                    radioButtonPrev.CheckedChanged += new EventHandler(RadioButtonMode_CheckedChanged);
                }
                radioButtonDetail.CheckedChanged += new EventHandler(RadioButtonMode_CheckedChanged);
                radioButtonThumb.CheckedChanged += new EventHandler(RadioButtonMode_CheckedChanged);
                comboBoxSize.SelectedValueChanged += new EventHandler(ComboBoxThumbnail_SelectedIndexChanged);
                comboBoxName.SelectedValueChanged += new EventHandler(ComboBoxThumbnail_SelectedIndexChanged);
                panelSample.Paint += new PaintEventHandler(PanelSample_Paint);
            }
            
            //=========================================================================================
            // 機　能：表示モードをUIに反映させる
            // 引　数：[in]viewMode    表示モード
            // 　　　　[in]setDefault  デフォルトの設定を行うときtrue
            // 戻り値：なし
            //=========================================================================================
            public void ViewModeToUi(FileListViewMode viewMode, bool setDefault) {
                if (viewMode == null) {
                    RadioButtonPrev.Checked = true;
                    if (setDefault) {
                        FileListViewMode defaultView = new FileListViewMode();
                        ComboBoxSize.SelectedIndex = ThumbSizeToIndex(defaultView.ThumbnailSize);
                        ComboBoxName.SelectedIndex = ThumbNameToIndex(defaultView.ThumbnailName);
                    }
                } else if (!viewMode.ThumbnailModeSwitch) {
                    RadioButtonDetail.Checked = true;
                    ComboBoxSize.SelectedIndex = ThumbSizeToIndex(viewMode.ThumbnailSize);
                    ComboBoxName.SelectedIndex = ThumbNameToIndex(viewMode.ThumbnailName);
                } else {
                    RadioButtonThumb.Checked = true;
                    ComboBoxSize.SelectedIndex = ThumbSizeToIndex(viewMode.ThumbnailSize);
                    ComboBoxName.SelectedIndex = ThumbNameToIndex(viewMode.ThumbnailName);
                }
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：UIの状態を表示モードとして取得する
            // 引　数：なし
            // 戻り値：表示モード
            //=========================================================================================
            public FileListViewMode UIToViewMode() {
                if (RadioButtonPrev != null && RadioButtonPrev.Checked) {
                    return null;
                } else {
                    FileListViewMode viewMode = new FileListViewMode();
                    viewMode.ThumbnailModeSwitch = RadioButtonThumb.Checked;
                    viewMode.ThumbnailSize = IndexToThumbSize(ComboBoxSize.SelectedIndex);
                    viewMode.ThumbnailName = IndexToThumbName(ComboBoxName.SelectedIndex);
                    return viewMode;
                }
            }

            //=========================================================================================
            // 機　能：サイズ情報をコンボボックスのインデックスに変換する
            // 引　数：[in]size  サイズ情報
            // 戻り値：コンボボックスのインデックス
            //=========================================================================================
            private static int ThumbSizeToIndex(FileListViewIconSize size) {
                if (size == FileListViewIconSize.IconSize32) {
                    return 0;
                } else if (size == FileListViewIconSize.IconSize48) {
                    return 1;
                } else if (size == FileListViewIconSize.IconSize64) {
                    return 2;
                } else if (size == FileListViewIconSize.IconSize128) {
                    return 3;
                } else if (size == FileListViewIconSize.IconSize256) {
                    return 4;
                } else {
                    Program.Abort("サイズ設定値が異常です。");
                    return 0;
                }
            }

            //=========================================================================================
            // 機　能：コンボボックスのインデックスをサイズ情報に変換する
            // 引　数：[in]index  コンボボックスのインデックス
            // 戻り値：サイズ情報
            //=========================================================================================
            private static FileListViewIconSize IndexToThumbSize(int index) {
                if (index == 0) {
                    return FileListViewIconSize.IconSize32;
                } else if (index == 1) {
                    return FileListViewIconSize.IconSize48;
                } else if (index == 2) {
                    return FileListViewIconSize.IconSize64;
                } else if (index == 3) {
                    return FileListViewIconSize.IconSize128;
                } else if (index == 4) {
                    return FileListViewIconSize.IconSize256;
                } else {
                    return FileListViewIconSize.IconSize32;
                }
            }

            //=========================================================================================
            // 機　能：名前の表示モードをコンボボックスのインデックスに変換する
            // 引　数：[in]name  名前の表示モード
            // 戻り値：コンボボックスのインデックス
            //=========================================================================================
            private static int ThumbNameToIndex(FileListViewThumbnailName name) {
                if (name == FileListViewThumbnailName.ThumbNameSpearate) {
                    return 0;
                } else if (name == FileListViewThumbnailName.ThumbNameOverray) {
                    return 1;
                } else if (name == FileListViewThumbnailName.ThumbNameNone) {
                    return 2;
                } else {
                    Program.Abort("名前種別の設定値が異常です。");
                    return 0;
                }
            }

            //=========================================================================================
            // 機　能：コンボボックスのインデックスを名前の表示モードに変換する
            // 引　数：[in]index  コンボボックスのインデックス
            // 戻り値：名前の表示モード
            //=========================================================================================
            private static FileListViewThumbnailName IndexToThumbName(int index) {
                if (index == 0) {
                    return FileListViewThumbnailName.ThumbNameSpearate;
                } else if (index == 1) {
                    return FileListViewThumbnailName.ThumbNameOverray;
                } else if (index == 2) {
                    return FileListViewThumbnailName.ThumbNameNone;
                } else {
                    return FileListViewThumbnailName.ThumbNameSpearate;
                }
            }

            //=========================================================================================
            // 機　能：UIの有効/無効を切り替える
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EnableUIItem() {
                bool enabled = RadioButtonThumb.Checked;
                ComboBoxSize.Enabled = enabled;
                ComboBoxName.Enabled = enabled;
            }
            
            //=========================================================================================
            // 機　能：モードのラジオボタンが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void RadioButtonMode_CheckedChanged(object sender, EventArgs evt) {
                EnableUIItem();
                Graphics g = this.PanelSample.CreateGraphics();
                try {
                    UpdateSample(g);
                } finally {
                    g.Dispose();
                }
            }

            //=========================================================================================
            // 機　能：サムネイルの設定用コンボボックスが変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void ComboBoxThumbnail_SelectedIndexChanged(object sender, EventArgs evt) {
                Graphics g = this.PanelSample.CreateGraphics();
                try {
                    UpdateSample(g);
                } finally {
                    g.Dispose();
                }
            }

            //=========================================================================================
            // 機　能：サンプル画面を更新する
            // 引　数：[in]g   表示に使用するグラフィックス
            // 戻り値：なし
            //=========================================================================================
            private void UpdateSample(Graphics g) {
                FileListOptionDialog.FileListColorSetting colorSetting = new FileListOptionDialog.FileListColorSetting(Configuration.Current);
                FileListViewMode viewMode = UIToViewMode();
                FileListOptionDialog.SampleRenderer renderer = new FileListOptionDialog.SampleRenderer(this.PanelSample, colorSetting, viewMode);
                renderer.Draw(g);
            }

            //=========================================================================================
            // 機　能：再描画イベントを処理する
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void PanelSample_Paint(object sender, PaintEventArgs evt) {
                UpdateSample(evt.Graphics);
            }
        }
    }
}
