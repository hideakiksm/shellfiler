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
    // ファイル一覧＞動作 の設定ページ
    //=========================================================================================
    public partial class FileListActionPage : UserControl, IOptionDialogPage {
        // 親となるオプションダイアログ
        private OptionSettingDialog m_parent;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent  親となるオプションダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FileListActionPage(OptionSettingDialog parent) {
            InitializeComponent();
            m_parent = parent;
            this.numericScrollMargin.Minimum = Configuration.MIN_LIST_VIEW_SCROLL_MARGIN_LINE_COUNT;
            this.numericScrollMargin.Maximum = Configuration.MAX_LIST_VIEW_SCROLL_MARGIN_LINE_COUNT;
            this.trackBarWheel.Minimum = Configuration.MIN_MOUSE_WHEEL_MAX_LINES;
            this.trackBarWheel.Maximum = Configuration.MAX_MOUSE_WHEEL_MAX_LINES;
            this.trackBarDragMax.Minimum = Configuration.MIN_FILE_LIST_DRAG_MAX_SPEED;
            this.trackBarDragMax.Maximum = Configuration.MAX_FILE_LIST_DRAG_MAX_SPEED;

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
            this.numericScrollMargin.Value = config.ListViewScrollMarginLineCount;
            this.trackBarWheel.Value = config.MouseWheelMaxLines;
            this.trackBarDragMax.Value = config.FileListDragMaxSpeed;
            this.checkBoxSeparateExt.Checked = config.FileListSeparateExt;
            this.checkBoxOppositeParent.Checked = config.ChdirParentOtherSideMove;
            this.checkBoxHideDrag.Checked = config.HideWindowDragDrop;
            this.checkBoxResumeCursor.Checked = config.ResumeFolderCursorFile;
        }

        //=========================================================================================
        // 機　能：UIから設定値を取得する
        // 引　数：なし
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public bool GetUIValue() {
            bool success;

            // ファイル一覧でのスクロールマージンの行数
            int scrollMargin = (int)(this.numericScrollMargin.Value);
            success = Configuration.CheckListViewScrollMarginLineCount(ref scrollMargin, m_parent);
            if (!success) {
                return false;
            }

            // マウスホイールが回転したときの最大移動行数
            int wheelLines = (int)(this.trackBarWheel.Value);
            Configuration.ModifyMouseWheelMaxLines(ref wheelLines);

            // ファイル一覧でドラッグ中に最大速度で移動できる行数
            int dragSpeed = (int)(this.trackBarDragMax.Value);
            Configuration.ModifyFileListDragMaxSpeed(ref dragSpeed);
            
            // ファイル一覧で最後の拡張子を離して表示するときtrue
            bool separate = this.checkBoxSeparateExt.Checked;
    
            // 逆向き←→で親フォルダに戻るときtrue
            bool chdir = this.checkBoxOppositeParent.Checked;

            // ドラッグ中にウィンドウ外に移動した場合にウィンドウを隠すときtrue
            bool hideDrag = this.checkBoxHideDrag.Checked;

            // フォルダ変更時にカーソル位置のレジュームを行うときtrue
            bool resumeCursor = this.checkBoxResumeCursor.Checked;

            // Configに反映
            Configuration.Current.ListViewScrollMarginLineCount = scrollMargin;
            Configuration.Current.MouseWheelMaxLines = wheelLines;
            Configuration.Current.FileListDragMaxSpeed = dragSpeed;
            Configuration.Current.FileListSeparateExt = separate;
            Configuration.Current.ChdirParentOtherSideMove = chdir;
            Configuration.Current.HideWindowDragDrop = hideDrag;
            Configuration.Current.ResumeFolderCursorFile = resumeCursor;

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
    }
}
