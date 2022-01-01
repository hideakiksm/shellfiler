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
    // クラス：ファイル時刻の日時指定を画面表示するユーザーコントロール
    //=========================================================================================
    public partial class FileConditionRelativeDateControl : UserControl {
        // 初期状態での開始時刻コンポーネントの左端座標
        private int m_positionStartLeft;

        // 初期状態での開始時刻コンポーネントの右端座標
        private int m_positionEndLeft;

        // 時刻コンポーネント中央時の左端座標
        private int m_positionCenterLeft;

        // 選択中の時刻モード
        private DateTimeType m_dateTimeType;

        // 編集対象の時刻情報（親ダイアログのインスタンスを共有）
        private DateTimeCondition m_timeCondition;

        // 時刻の入力状態がエラーのときtrue
        private bool m_timeError = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]timeCondition  表示対象のファイル時刻
        // 戻り値：なし
        //=========================================================================================
        public FileConditionRelativeDateControl(DateTimeCondition timeCondition) {
            InitializeComponent();
            m_timeCondition = timeCondition;

            m_positionStartLeft = this.numericStart.Left;
            m_positionEndLeft = this.numericEnd.Left;
            int cx = this.checkBoxStart.Right - this.numericStart.Left;
            m_positionCenterLeft = (this.Width - cx) / 2;
            this.numericStart.Minimum = 0;
            this.numericStart.Maximum = DateTimeCondition.MAX_RELATIVE_VALUE;
            this.numericEnd.Minimum = 0;
            this.numericEnd.Maximum = DateTimeCondition.MAX_RELATIVE_VALUE;
        }
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]dateType  新しく選択する日付の種類
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(DateTimeType dateType) {
            m_dateTimeType = dateType;
            bool startEnabled;                // 開始時刻が有効なときtrue
            bool endEnabled;                  // 終了時刻が有効なときtrue
            bool includeEnabled;              // 含むが有効なときtrue
            if (dateType == DateTimeType.RelativeXxxStart) {
                startEnabled = true;
                endEnabled = false;
                includeEnabled = true;
            } else if (dateType == DateTimeType.RelativeEndXxx) {
                startEnabled = false;
                endEnabled = true;
                includeEnabled = true;
            } else if (dateType == DateTimeType.RelativeStartXxxEnd) {
                startEnabled = true;
                endEnabled = true;
                includeEnabled = true;
            } else if (dateType == DateTimeType.RelativeXxxStartEndXxx) {
                startEnabled = true;
                endEnabled = true;
                includeEnabled = true;
            } else if (dateType == DateTimeType.RelativeXxx) {
                startEnabled = true;
                endEnabled = false;
                includeEnabled = false;
            } else {
                Program.Abort("dateTypeの値が想定外です。");
                return;
            }

            // UIに反映
            if (startEnabled && !endEnabled) {
                this.numericStart.Left = m_positionCenterLeft;
                this.labelStart.Left = this.numericStart.Right + 2;
                this.maskedStart.Left = this.labelStart.Right + 2;
                this.checkBoxStart.Left = this.maskedStart.Right + 2;
            } else if (!startEnabled && endEnabled) {
                this.numericEnd.Left = m_positionCenterLeft;
                this.labelEnd.Left = this.numericEnd.Right + 2;
                this.maskedEnd.Left = this.labelEnd.Right + 2;
                this.checkBoxEnd.Left = this.maskedEnd.Right + 2;
            } else {
                this.numericStart.Left = m_positionStartLeft;
                this.labelStart.Left = this.numericStart.Right + 2;
                this.maskedStart.Left = this.labelStart.Right + 2;
                this.checkBoxStart.Left = this.maskedStart.Right + 2;
                this.numericEnd.Left = m_positionEndLeft;
                this.labelEnd.Left = this.numericEnd.Right + 2;
                this.maskedEnd.Left = this.labelEnd.Right + 2;
                this.checkBoxEnd.Left = this.maskedEnd.Right + 2;
            }
            if (startEnabled) {
                this.numericStart.Show();
                this.numericStart.Value = m_timeCondition.RelativeStart;
                this.labelStart.Show();
                this.maskedStart.Show();
                this.maskedStart.Text = m_timeCondition.TimeStart.ToString();
                if (includeEnabled) {
                    this.checkBoxStart.Show();
                    this.checkBoxStart.Checked = m_timeCondition.IncludeStart;
                } else {
                    this.checkBoxStart.Hide();
                }
            } else {
                this.numericStart.Hide();
                this.labelStart.Hide();
                this.maskedStart.Hide();
                this.checkBoxStart.Hide();
            }
            if (endEnabled) {
                this.numericEnd.Show();
                this.numericEnd.Value = m_timeCondition.RelativeEnd;
                this.labelEnd.Show();
                this.maskedEnd.Show();
                this.maskedEnd.Text = m_timeCondition.TimeEnd.ToString();
                if (includeEnabled) {
                    this.checkBoxEnd.Show();
                    this.checkBoxEnd.Checked = m_timeCondition.IncludeEnd;
                } else {
                    this.checkBoxEnd.Hide();
                }
            } else {
                this.numericEnd.Hide();
                this.labelEnd.Hide();
                this.maskedEnd.Hide();
                this.checkBoxEnd.Hide();
            }

            this.panelRange.Refresh();
        }


        //=========================================================================================
        // 機　能：開始または終了時刻の値が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void dateTimeStartEnd_ValueChanged(object sender, EventArgs evt) {
            if (sender == this.numericStart || sender == this.maskedStart) {
                m_timeCondition.RelativeStart = (int)(this.numericStart.Value);
                TimeInfo time = TimeInfo.ParseTimeInfo(this.maskedStart.Text);
                if (time != null) {
                    m_timeError = false;
                    m_timeCondition.TimeStart = time;
                } else {
                    m_timeError = true;
                }
            } else {
                m_timeCondition.RelativeEnd = (int)(this.numericEnd.Value);
                TimeInfo time = TimeInfo.ParseTimeInfo(this.maskedEnd.Text);
                if (time != null) {
                    m_timeError = false;
                    m_timeCondition.TimeEnd = time;
                } else {
                    m_timeError = true;
                }
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
                m_timeCondition.IncludeStart = this.checkBoxStart.Checked;
            } else {
                m_timeCondition.IncludeEnd = this.checkBoxEnd.Checked;
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
            int dateTimeWidth = this.checkBoxStart.Right - this.numericStart.Left;
            bool valid = true;
            if (m_dateTimeType == DateTimeType.RelativeXxxStart) {
                mode = FileConditionDateControl.TimeRangeMode.XxxMiddle;
                includeLeft = m_timeCondition.IncludeStart;
                includeRight = false;
                xPosLeft = m_positionCenterLeft + dateTimeWidth / 2;
                xPosRight = -1;
            } else if (m_dateTimeType == DateTimeType.RelativeEndXxx) {
                mode = FileConditionDateControl.TimeRangeMode.MiddleXxx;
                includeLeft = m_timeCondition.IncludeEnd;
                includeRight = false;
                xPosLeft = m_positionCenterLeft + dateTimeWidth / 2;
                xPosRight = -1;
            } else if (m_dateTimeType == DateTimeType.RelativeStartXxxEnd) {
                mode = FileConditionDateControl.TimeRangeMode.LeftXxxRight;
                includeLeft = m_timeCondition.IncludeStart;
                includeRight = m_timeCondition.IncludeEnd;
                xPosLeft = m_positionStartLeft + dateTimeWidth / 2;
                xPosRight = m_positionEndLeft + dateTimeWidth / 2;
                valid = ((m_timeCondition.RelativeStart > m_timeCondition.RelativeEnd) ||
                         (m_timeCondition.RelativeStart == m_timeCondition.RelativeEnd && m_timeCondition.TimeStart.ToIntValue() < m_timeCondition.TimeEnd.ToIntValue()));
            } else if (m_dateTimeType == DateTimeType.RelativeXxxStartEndXxx) {
                mode = FileConditionDateControl.TimeRangeMode.XxxLeftRightXxx;
                includeLeft = m_timeCondition.IncludeStart;
                includeRight = m_timeCondition.IncludeEnd;
                xPosLeft = m_positionStartLeft + dateTimeWidth / 2;
                xPosRight = m_positionEndLeft + dateTimeWidth / 2;
                valid = ((m_timeCondition.RelativeStart > m_timeCondition.RelativeEnd) ||
                         (m_timeCondition.RelativeStart == m_timeCondition.RelativeEnd && m_timeCondition.TimeStart.ToIntValue() < m_timeCondition.TimeEnd.ToIntValue()));
            } else if (m_dateTimeType == DateTimeType.RelativeXxx) {
                mode = FileConditionDateControl.TimeRangeMode.Xxx;
                includeLeft = false;
                includeRight = false;
                xPosLeft = m_positionCenterLeft + dateTimeWidth / 2;
                xPosRight = -1;
            } else {
                Program.Abort("dateTypeの値が想定外です。");
                return;
            }

            // 表示クラスに委譲
            FileConditionDateControl.TimeRangePaintImpl paintImpl = new FileConditionDateControl.TimeRangePaintImpl(
                    this.panelRange, mode, includeLeft, includeRight, xPosLeft, xPosRight, valid,
                    Resources.DlgTransferCond_RangeBarPast, Resources.DlgTransferCond_RangeBarFuture);
            paintImpl.Draw(evt.Graphics);
        }
        
        //=========================================================================================
        // 機　能：正しい入力状態かどうかを返す
        // 引　数：なし
        // 戻り値：入力状態が正しいときtrue（その他、逆転状態のチェックが必要）
        //=========================================================================================
        public bool IsValidInput() {
            return !m_timeError;
        }
    }
}
