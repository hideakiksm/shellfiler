using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.FileTask.Condition;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイルサイズの時刻指定を画面表示するユーザーコントロール
    //=========================================================================================
    public partial class FileConditionSizeControl : UserControl {
        // 初期状態でのサイズコンポーネントの左端座標
        private int m_positionStartLeft;

        // 初期状態でのサイズコンポーネントの右端座標
        private int m_positionEndLeft;

        // サイズコンポーネント中央時の左端座標
        private int m_positionCenterLeft;

        // 選択中のサイズモード
        private FileSizeType m_fileSizeType;

        // 編集対象の時刻情報（親ダイアログのインスタンスを共有）
        private FileSizeCondition m_sizeCondition;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]sizeCondition  表示対象のファイルサイズ
        // 戻り値：なし
        //=========================================================================================
        public FileConditionSizeControl(FileSizeCondition sizeCondition) {
            InitializeComponent();
            m_sizeCondition = sizeCondition;

            m_positionStartLeft = this.numericStart.Left;
            m_positionEndLeft = this.numericEnd.Left;
            m_positionCenterLeft = (this.Width - this.numericStart.Width) / 2;
        }
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]dateType  新しく選択するサイズの種類
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(FileSizeType sizeType) {
            m_fileSizeType = sizeType;
            bool startEnabled;                // 開始サイズが有効なときtrue
            bool endEnabled;                  // 終了サイズが有効なときtrue
            bool includeEnabled;              // 含むが有効なときtrue
            if (sizeType == FileSizeType.XxxSize) {
                startEnabled = true;
                endEnabled = false;
                includeEnabled = true;
            } else if (sizeType == FileSizeType.SizeXxx) {
                startEnabled = false;
                endEnabled = true;
                includeEnabled = true;
            } else if (sizeType == FileSizeType.SizeXxxSize) {
                startEnabled = true;
                endEnabled = true;
                includeEnabled = true;
            } else if (sizeType == FileSizeType.XxxSizeXxx) {
                startEnabled = true;
                endEnabled = true;
                includeEnabled = true;
            } else if (sizeType == FileSizeType.Size) {
                startEnabled = true;
                endEnabled = false;
                includeEnabled = false;
            } else {
                Program.Abort("sizeTypeの値が想定外です。");
                return;
            }

            // UIに反映
            if (startEnabled && !endEnabled) {
                this.numericStart.Left = m_positionCenterLeft;
                this.checkBoxStart.Left = m_positionCenterLeft + this.numericStart.Width + 1;
            } else if (!startEnabled && endEnabled) {
                this.numericEnd.Left = m_positionCenterLeft;
                this.checkBoxEnd.Left = m_positionCenterLeft + this.numericEnd.Width + 1;
            } else {
                this.numericStart.Left = m_positionStartLeft;
                this.checkBoxStart.Left = m_positionStartLeft + this.numericStart.Width + 1;
                this.numericEnd.Left = m_positionEndLeft;
                this.checkBoxEnd.Left = m_positionEndLeft + this.numericEnd.Width + 1;
            }
            if (startEnabled) {
                this.numericStart.Show();
                this.numericStart.Value = m_sizeCondition.MinSize;
                if (includeEnabled) {
                    this.checkBoxStart.Show();
                    this.checkBoxStart.Checked = m_sizeCondition.IncludeMin;
                } else {
                    this.checkBoxStart.Hide();
                }
            } else {
                this.numericStart.Hide();
                this.checkBoxStart.Hide();
            }
            if (endEnabled) {
                this.numericEnd.Show();
                this.numericEnd.Value = m_sizeCondition.MaxSize;
                if (includeEnabled) {
                    this.checkBoxEnd.Show();
                    this.checkBoxEnd.Checked = m_sizeCondition.IncludeMax;
                } else {
                    this.checkBoxEnd.Hide();
                }
            } else {
                this.numericEnd.Hide();
                this.checkBoxEnd.Hide();
            }

            this.panelRange.Refresh();
        }

        //=========================================================================================
        // 機　能：最小または最大サイズの値が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void numericStartEnd_ValueChanged(object sender, EventArgs evt) {
            if (sender == this.numericStart) {
                m_sizeCondition.MinSize = (long)(this.numericStart.Value);
            } else {
                m_sizeCondition.MaxSize = (long)(this.numericEnd.Value);
            }

            this.panelRange.Refresh();
        }

        //=========================================================================================
        // 機　能：開始または終了時刻を含めるかどうかのチェックボックスが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void checkBoxStartEnd_CheckedChanged(object sender, EventArgs evt) {
            if (sender == this.checkBoxStart) {
                m_sizeCondition.IncludeMin = this.checkBoxStart.Checked;
            } else {
                m_sizeCondition.IncludeMax = this.checkBoxEnd.Checked;
            }

            this.panelRange.Refresh();
        }

        //=========================================================================================
        // 機　能：画面を再描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void panelRange_Paint(object sender, PaintEventArgs evt) {
            FileConditionDateControl.TimeRangeMode mode;
            bool includeLeft;
            bool includeRight;
            int xPosLeft;
            int xPosRight;

            // 描画条件を決定
            int dateTimeWidth = this.numericStart.Width;
            bool valid = true;
            if (m_fileSizeType == FileSizeType.XxxSize) {
                mode = FileConditionDateControl.TimeRangeMode.XxxMiddle;
                includeLeft = m_sizeCondition.IncludeMin;
                includeRight = false;
                xPosLeft = m_positionCenterLeft + dateTimeWidth / 2;
                xPosRight = -1;
            } else if (m_fileSizeType == FileSizeType.SizeXxx) {
                mode = FileConditionDateControl.TimeRangeMode.MiddleXxx;
                includeLeft = m_sizeCondition.IncludeMax;
                includeRight = false;
                xPosLeft = m_positionCenterLeft + dateTimeWidth / 2;
                xPosRight = -1;
            } else if (m_fileSizeType == FileSizeType.SizeXxxSize) {
                mode = FileConditionDateControl.TimeRangeMode.LeftXxxRight;
                includeLeft = m_sizeCondition.IncludeMin;
                includeRight = m_sizeCondition.IncludeMax;
                xPosLeft = m_positionStartLeft + dateTimeWidth / 2;
                xPosRight = m_positionEndLeft + dateTimeWidth / 2;
                valid = (m_sizeCondition.MinSize < m_sizeCondition.MaxSize);
            } else if (m_fileSizeType == FileSizeType.XxxSizeXxx) {
                mode = FileConditionDateControl.TimeRangeMode.XxxLeftRightXxx;
                includeLeft = m_sizeCondition.IncludeMin;
                includeRight = m_sizeCondition.IncludeMax;
                xPosLeft = m_positionStartLeft + dateTimeWidth / 2;
                xPosRight = m_positionEndLeft + dateTimeWidth / 2;
                valid = (m_sizeCondition.MinSize < m_sizeCondition.MaxSize);
            } else if (m_fileSizeType == FileSizeType.Size) {
                mode = FileConditionDateControl.TimeRangeMode.Xxx;
                includeLeft = false;
                includeRight = false;
                xPosLeft = m_positionCenterLeft + dateTimeWidth / 2;
                xPosRight = -1;
            } else {
                Program.Abort("sizeTypeの値が想定外です。");
                return;
            }

            // 表示クラスに委譲
            FileConditionDateControl.TimeRangePaintImpl paintImpl = new FileConditionDateControl.TimeRangePaintImpl(
                    this.panelRange, mode, includeLeft, includeRight, xPosLeft, xPosRight, valid,
                    Resources.DlgTransferCond_RangeBarSmall, Resources.DlgTransferCond_RangeBarBig);
            paintImpl.Draw(evt.Graphics);
        }

        //=========================================================================================
        // 機　能：正しい入力状態かどうかを返す
        // 引　数：なし
        // 戻り値：入力状態が正しいときtrue（その他、逆転状態のチェックが必要）
        //=========================================================================================
        public bool IsValidInput() {
            return true;
        }
   }
}
