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
    public partial class FileConditionDateControl : UserControl {
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

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]timeCondition  表示対象のファイル時刻
        // 戻り値：なし
        //=========================================================================================
        public FileConditionDateControl(DateTimeCondition timeCondition) {
            InitializeComponent();
            m_timeCondition = timeCondition;

            m_positionStartLeft = this.dateTimeStart.Left;
            m_positionEndLeft = this.dateTimeEnd.Left;
            m_positionCenterLeft = (this.Width - this.dateTimeStart.Width) / 2;
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
            if (dateType == DateTimeType.DateXxxStart) {
                startEnabled = true;
                endEnabled = false;
                includeEnabled = true;
            } else if (dateType == DateTimeType.DateEndXxx) {
                startEnabled = false;
                endEnabled = true;
                includeEnabled = true;
            } else if (dateType == DateTimeType.DateStartXxxEnd) {
                startEnabled = true;
                endEnabled = true;
                includeEnabled = true;
            } else if (dateType == DateTimeType.DateXxxStartEndXxx) {
                startEnabled = true;
                endEnabled = true;
                includeEnabled = true;
            } else if (dateType == DateTimeType.DateXxx) {
                startEnabled = true;
                endEnabled = false;
                includeEnabled = false;
            } else {
                Program.Abort("dateTypeの値が想定外です。");
                return;
            }

            // UIに反映
            if (startEnabled && !endEnabled) {
                this.dateTimeStart.Left = m_positionCenterLeft;
                this.checkBoxStart.Left = m_positionCenterLeft + this.dateTimeStart.Width + 1;
            } else if (!startEnabled && endEnabled) {
                this.dateTimeEnd.Left = m_positionCenterLeft;
                this.checkBoxEnd.Left = m_positionCenterLeft + this.dateTimeEnd.Width + 1;
            } else {
                this.dateTimeStart.Left = m_positionStartLeft;
                this.checkBoxStart.Left = m_positionStartLeft + this.dateTimeStart.Width + 1;
                this.dateTimeEnd.Left = m_positionEndLeft;
                this.checkBoxEnd.Left = m_positionEndLeft + this.dateTimeEnd.Width + 1;
            }
            if (startEnabled) {
                this.dateTimeStart.Show();
                this.dateTimeStart.Value = m_timeCondition.DateStart;
                if (includeEnabled) {
                    this.checkBoxStart.Show();
                    this.checkBoxStart.Checked = m_timeCondition.IncludeStart;
                } else {
                    this.checkBoxStart.Hide();
                }
            } else {
                this.dateTimeStart.Hide();
                this.checkBoxStart.Hide();
            }
            if (endEnabled) {
                this.dateTimeEnd.Show();
                this.dateTimeEnd.Value = m_timeCondition.DateEnd;
                if (includeEnabled) {
                    this.checkBoxEnd.Show();
                    this.checkBoxEnd.Checked = m_timeCondition.IncludeEnd;
                } else {
                    this.checkBoxEnd.Hide();
                }
            } else {
                this.dateTimeEnd.Hide();
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
            if (sender == this.dateTimeStart) {
                m_timeCondition.DateStart = this.dateTimeStart.Value;
            } else {
                m_timeCondition.DateEnd = this.dateTimeEnd.Value;
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
            TimeRangeMode mode;
            bool includeLeft;
            bool includeRight;
            int xPosLeft;
            int xPosRight;

            // 描画条件を決定
            int dateTimeWidth = this.dateTimeStart.Width;
            bool valid = true;
            if (m_dateTimeType == DateTimeType.DateXxxStart) {
                mode = TimeRangeMode.XxxMiddle;
                includeLeft = m_timeCondition.IncludeStart;
                includeRight = false;
                xPosLeft = m_positionCenterLeft + dateTimeWidth / 2;
                xPosRight = -1;
            } else if (m_dateTimeType == DateTimeType.DateEndXxx) {
                mode = TimeRangeMode.MiddleXxx;
                includeLeft = m_timeCondition.IncludeEnd;
                includeRight = false;
                xPosLeft = m_positionCenterLeft + dateTimeWidth / 2;
                xPosRight = -1;
            } else if (m_dateTimeType == DateTimeType.DateStartXxxEnd) {
                mode = TimeRangeMode.LeftXxxRight;
                includeLeft = m_timeCondition.IncludeStart;
                includeRight = m_timeCondition.IncludeEnd;
                xPosLeft = m_positionStartLeft + dateTimeWidth / 2;
                xPosRight = m_positionEndLeft + dateTimeWidth / 2;
                valid = (m_timeCondition.DateStart < m_timeCondition.DateEnd);
            } else if (m_dateTimeType == DateTimeType.DateXxxStartEndXxx) {
                mode = TimeRangeMode.XxxLeftRightXxx;
                includeLeft = m_timeCondition.IncludeStart;
                includeRight = m_timeCondition.IncludeEnd;
                xPosLeft = m_positionStartLeft + dateTimeWidth / 2;
                xPosRight = m_positionEndLeft + dateTimeWidth / 2;
                valid = (m_timeCondition.DateStart < m_timeCondition.DateEnd);
            } else if (m_dateTimeType == DateTimeType.DateXxx) {
                mode = TimeRangeMode.Xxx;
                includeLeft = false;
                includeRight = false;
                xPosLeft = m_positionCenterLeft + dateTimeWidth / 2;
                xPosRight = -1;
            } else {
                Program.Abort("dateTypeの値が想定外です。");
                return;
            }

            // 表示クラスに委譲
            TimeRangePaintImpl paintImpl = new TimeRangePaintImpl(
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
            return true;
        }

        //=========================================================================================
        // クラス：時刻範囲を表示するモード
        //=========================================================================================
        public enum TimeRangeMode {
            XxxMiddle,                          // 中央より左側を選択
            MiddleXxx,                          // 中央より右側を選択
            LeftXxxRight,                       // 左右の中央部分を選択
            XxxLeftRightXxx,                    // 左右の両端を選択
            Xxx,                                // 指定部分のみを選択
        }

        //=========================================================================================
        // クラス：時刻範囲の描画を実装するクラス
        //=========================================================================================
        public class TimeRangePaintImpl {
            // 描画座標
            const int X_INCLUDE_TOP = -4;               // 入力点を含む場合の上部X座標
            const int X_INCLUDE_BOTTOM = -4;            // 入力点を含む場合の下部X座標
            const int X_EXCLUDE_TOP = 4;                // 入力点を含まない場合の上部X座標
            const int X_EXCLUDE_BOTTOM = 8;             // 入力点を含まない場合の下部X座標
            const int Y_TOP = 6;                        // 上部Y座標
            const int Y_BOTTOM = 20;                    // 下部Y座標

            // 描画対象のパネル
            private Panel m_targetPanel;

            // 描画モード
            private TimeRangeMode m_mode;

            // 左側または中央の値そのものを含むときtrue
            private bool m_includeLeftMiddle;

            // 右側の値を含むときtrue（右側が存在しないときfalse）
            private bool m_includeRight;

            // 左側または中央のX座標
            private int m_xPosLeftMiddle;

            // 右側のX座標（右側が存在しないとき-1）
            private int m_xPosRight;

            // 選択範囲が正常なときtrue
            private bool m_validRange;

            // 左側に表示するラベル
            private string m_leftLabel;

            // 右側に表示するラベル
            private string m_rightLabel;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]targetPanel       描画対象のパネル
            // 　　　　[in]mode              描画モード
            // 　　　　[in]includeLeftMiddle 左側または中央の値そのものを含むときtrue
            // 　　　　[in]includeRight      右側の値を含むときtrue（右側が存在しないときfalse）
            // 　　　　[in]xPosLeftMiddle    左側または中央のX座標
            // 　　　　[in]xPosRight         右側のX座標（右側が存在しないとき-1）
            // 　　　　[in]validRange        選択範囲が正常なときtrue
            // 　　　　[in]leftLabel         左側に表示するラベル
            // 　　　　[in]rightLabel        右側に表示するラベル
            // 戻り値：入力状態が正しいときtrue（その他、逆転状態のチェックが必要）
            //=========================================================================================
            public TimeRangePaintImpl(Panel targetPanel, TimeRangeMode mode, bool includeLeftMiddle, bool includeRight, int xPosLeftMiddle, int xPosRight, bool validRange, string leftLabel, string rightLabel) {
                m_targetPanel = targetPanel;
                m_mode = mode;
                m_includeLeftMiddle = includeLeftMiddle;
                m_includeRight = includeRight;
                m_xPosLeftMiddle = xPosLeftMiddle;
                m_xPosRight = xPosRight;
                m_validRange = validRange;
                m_leftLabel = leftLabel;
                m_rightLabel = rightLabel;
            }

            //=========================================================================================
            // 機　能：画面を描画する
            // 引　数：[in]grp   表示に使用するグラフィックス
            // 戻り値：入力状態が正しいときtrue（その他、逆転状態のチェックが必要）
            //=========================================================================================
            public void Draw(Graphics grp) {
                DoubleBuffer doubleBuffer = new DoubleBuffer(grp, m_targetPanel.Width, m_targetPanel.Height);
                doubleBuffer.SetDrawOrigin(0, 0);
                TimeRangeGraphics g = new TimeRangeGraphics(doubleBuffer.DrawingGraphics, m_targetPanel);
                g.Graphics.FillRectangle(SystemBrushes.Window, m_targetPanel.ClientRectangle);
                try {
                    DrawRange(g);
                    DrawTimebar(g);
                } finally {
                    g.Dispose();
                }

                doubleBuffer.FlushScreen(0, 0);
            }

            //=========================================================================================
            // 機　能：対象範囲を描画する
            // 引　数：[in]grp   表示に使用するグラフィックス
            // 戻り値：なし
            //=========================================================================================
            private void DrawRange(TimeRangeGraphics g) {
                // 選択範囲の妥当性を判定
                Pen pen;
                Brush brush;
                if (m_validRange) {
                    pen = g.TimeRangeOkPen;
                    brush = g.TimeRangeOkBrush;
                } else {
                    pen = g.TimeRangeNgPen;
                    brush = g.TimeRangeNgBrush;
                }

                // 選択範囲
                switch (m_mode) {
                    case TimeRangeMode.MiddleXxx:
                        DrawRangeInputXxx(g, pen, brush, m_xPosLeftMiddle, m_includeLeftMiddle);
                        break;
                    case TimeRangeMode.XxxMiddle:
                        DrawRangeXxxInput(g, pen, brush, m_xPosLeftMiddle, m_includeLeftMiddle);
                        break;
                    case TimeRangeMode.XxxLeftRightXxx:
                        DrawRangeXxxInput(g, pen, brush, m_xPosLeftMiddle, m_includeLeftMiddle);
                        DrawRangeInputXxx(g, pen, brush, m_xPosRight, m_includeRight);
                        break;
                    case TimeRangeMode.LeftXxxRight:
                        DrawRangeInputXxxInput(g, pen, brush);
                        break;
                    case TimeRangeMode.Xxx:
                        DrawRangeInputXxx(g, pen, brush);
                        break;
                }
            }

            //=========================================================================================
            // 機　能：入力位置の右側を選択状態にして表示する
            // 引　数：[in]g          表示に使用するグラフィックス
            // 　　　　[in]pen        選択範囲の描画に使用するペン（通常状態またはエラー状態）
            // 　　　　[in]brush      選択範囲の内側の描画に使用するブラシ（通常状態またはエラー状態）
            // 　　　　[in]xPosInput  入力領域に対応するX座標
            // 　　　　[in]include    入力領域を含むときtrue
            // 戻り値：なし
            //=========================================================================================
            private void DrawRangeInputXxx(TimeRangeGraphics g, Pen pen, Brush brush, int xPosInput, bool include) {
                int width = m_targetPanel.Width;
                int rightBorder = width - 17;
                Point[] ptRange1 = new Point[4];
                Point[] ptRange2 = new Point[3];
                if (include) {
                    ptRange1[0] = new Point(xPosInput + X_INCLUDE_TOP, Y_TOP);
                    ptRange1[1] = new Point(xPosInput + X_INCLUDE_BOTTOM, Y_BOTTOM);
                    ptRange2[0] = new Point(xPosInput + X_INCLUDE_TOP, Y_TOP);
                    ptRange2[1] = new Point(xPosInput + X_INCLUDE_BOTTOM, Y_BOTTOM);
                } else {
                    ptRange1[0] = new Point(xPosInput + X_EXCLUDE_TOP, Y_TOP);
                    ptRange1[1] = new Point(xPosInput + X_EXCLUDE_BOTTOM, Y_BOTTOM);
                    ptRange2[0] = new Point(xPosInput + X_EXCLUDE_TOP, Y_TOP);
                    ptRange2[1] = new Point(xPosInput + X_EXCLUDE_BOTTOM, Y_BOTTOM);
                }
                ptRange1[2] = new Point(rightBorder, Y_BOTTOM);
                ptRange1[3] = new Point(rightBorder, Y_TOP);
                ptRange2[2] = new Point(rightBorder - 1, Y_BOTTOM);
                g.Graphics.FillPolygon(brush, ptRange1);
                g.Graphics.DrawLines(pen, ptRange2);
            }

            //=========================================================================================
            // 機　能：入力位置の左側を選択状態にして表示する
            // 引　数：[in]g          表示に使用するグラフィックス
            // 　　　　[in]pen        選択範囲の描画に使用するペン（通常状態またはエラー状態）
            // 　　　　[in]brush      選択範囲の内側の描画に使用するブラシ（通常状態またはエラー状態）
            // 　　　　[in]xPosInput  入力領域に対応するX座標
            // 　　　　[in]include    入力領域を含むときtrue
            // 戻り値：なし
            //=========================================================================================
            private void DrawRangeXxxInput(TimeRangeGraphics g, Pen pen, Brush brush, int xPosInput, bool include) {
                int width = m_targetPanel.Width;
                int leftBorder = 8;
                Point[] ptRange1 = new Point[4];
                Point[] ptRange2 = new Point[3];
                if (include) {
                    ptRange1[0] = new Point(xPosInput - X_INCLUDE_TOP, Y_TOP);
                    ptRange1[1] = new Point(xPosInput - X_INCLUDE_BOTTOM, Y_BOTTOM);
                    ptRange2[0] = new Point(xPosInput - X_INCLUDE_TOP, Y_TOP);
                    ptRange2[1] = new Point(xPosInput - X_INCLUDE_BOTTOM, Y_BOTTOM);
                } else {
                    ptRange1[0] = new Point(xPosInput - X_EXCLUDE_TOP, Y_TOP);
                    ptRange1[1] = new Point(xPosInput - X_EXCLUDE_BOTTOM, Y_BOTTOM);
                    ptRange2[0] = new Point(xPosInput - X_EXCLUDE_TOP, Y_TOP);
                    ptRange2[1] = new Point(xPosInput - X_EXCLUDE_BOTTOM, Y_BOTTOM);
                }
                ptRange1[2] = new Point(leftBorder, Y_BOTTOM);
                ptRange1[3] = new Point(leftBorder, Y_TOP);
                ptRange2[2] = new Point(leftBorder, Y_BOTTOM);
                g.Graphics.FillPolygon(brush, ptRange1);
                g.Graphics.DrawLines(pen, ptRange2);
            }

            //=========================================================================================
            // 機　能：入力位置の内側を選択状態にして表示する
            // 引　数：[in]g          表示に使用するグラフィックス
            // 　　　　[in]pen        選択範囲の描画に使用するペン（通常状態またはエラー状態）
            // 　　　　[in]brush      選択範囲の内側の描画に使用するブラシ（通常状態またはエラー状態）
            // 戻り値：なし
            //=========================================================================================
            private void DrawRangeInputXxxInput(TimeRangeGraphics g, Pen pen, Brush brush) {
                Point[] ptRange = new Point[4];
                if (m_includeLeftMiddle) {
                    ptRange[0] = new Point(m_xPosLeftMiddle + X_INCLUDE_TOP, Y_TOP);
                    ptRange[1] = new Point(m_xPosLeftMiddle + X_INCLUDE_BOTTOM, Y_BOTTOM);
                } else {
                    ptRange[0] = new Point(m_xPosLeftMiddle + X_EXCLUDE_TOP, Y_TOP);
                    ptRange[1] = new Point(m_xPosLeftMiddle + X_EXCLUDE_BOTTOM, Y_BOTTOM);
                }
                if (m_includeRight) {
                    ptRange[2] = new Point(m_xPosRight - X_INCLUDE_BOTTOM, Y_BOTTOM);
                    ptRange[3] = new Point(m_xPosRight - X_INCLUDE_TOP, Y_TOP);
                } else {
                    ptRange[2] = new Point(m_xPosRight - X_EXCLUDE_BOTTOM, Y_BOTTOM);
                    ptRange[3] = new Point(m_xPosRight - X_EXCLUDE_TOP, Y_TOP);
                }
                g.Graphics.FillPolygon(brush, ptRange);
                g.Graphics.DrawLines(pen, ptRange);
            }

            //=========================================================================================
            // 機　能：入力位置の位置そのものを選択状態にして表示する
            // 引　数：[in]g          表示に使用するグラフィックス
            // 　　　　[in]pen        選択範囲の描画に使用するペン（通常状態またはエラー状態）
            // 　　　　[in]brush      選択範囲の内側の描画に使用するブラシ（通常状態またはエラー状態）
            // 戻り値：なし
            //=========================================================================================
            private void DrawRangeInputXxx(TimeRangeGraphics g, Pen pen, Brush brush) {
                Point[] ptRange = new Point[4];
                ptRange[0] = new Point(m_xPosLeftMiddle + X_INCLUDE_TOP, Y_TOP);
                ptRange[1] = new Point(m_xPosLeftMiddle + X_INCLUDE_BOTTOM, Y_BOTTOM);
                ptRange[2] = new Point(m_xPosLeftMiddle - X_INCLUDE_BOTTOM, Y_BOTTOM);
                ptRange[3] = new Point(m_xPosLeftMiddle - X_INCLUDE_TOP, Y_TOP);
                g.Graphics.FillPolygon(brush, ptRange);
                g.Graphics.DrawLines(pen, ptRange);
            }

            //=========================================================================================
            // 機　能：時刻を表す矢印付きのバーを表示する
            // 引　数：[in]g   表示に使用するグラフィックス
            // 戻り値：なし
            //=========================================================================================
            private void DrawTimebar(TimeRangeGraphics g) {
                int width = m_targetPanel.Width;

                // タイムライン
                const int YBAR = 6;
                g.Graphics.DrawLine(g.TimeBarPen1, new Point(8, YBAR + 1), new Point(width - 11, YBAR + 1));
                g.Graphics.DrawLine(g.TimeBarPen2, new Point(8, YBAR), new Point(width - 11, YBAR));

                g.Graphics.DrawString(m_leftLabel, g.CommentFont, g.InputPointBrush, new PointF(12, YBAR + 3));
                g.Graphics.DrawString(m_rightLabel, g.CommentFont, g.InputPointBrush, new PointF(width - 44, YBAR + 3));
                
                // 入力ポイント
                Point[] timePointPosLeftMiddle = new Point[] { new Point(m_xPosLeftMiddle - 5, 0), new Point(m_xPosLeftMiddle, 5), new Point(m_xPosLeftMiddle + 5, 0) };
                g.Graphics.FillPolygon(g.InputPointBrush, timePointPosLeftMiddle, FillMode.Winding);

                if (m_xPosRight != -1) {
                    Point[] timePointPosRight = new Point[] { new Point(m_xPosRight - 5, 0), new Point(m_xPosRight, 5), new Point(m_xPosRight + 5, 0) };
                    g.Graphics.FillPolygon(g.InputPointBrush, timePointPosRight, FillMode.Winding);
                }

                // 範囲
                if (!m_validRange) {
                    g.Graphics.DrawString(Resources.DlgTransferCond_DateControlError, g.CommentFont, g.InputPointBrush, new PointF(width / 2 - 32, YBAR + 3));
                }
            }
        }

        //=========================================================================================
        // クラス：ファンクションバーの描画用グラフィックス
        //=========================================================================================
        public class TimeRangeGraphics {
            // タイムバー
            private static readonly Color TIME_BAR_COLOR_SHADOW = Color.FromArgb(52, 72, 98);         // 線の影の部分
            private static readonly Color TIME_BAR_COLOR_MAIN   = Color.FromArgb(149, 164, 186);      // 線の描画色
            
            // OKの場合
            private static readonly Color RANGE_COLOR_OK_LINE   = Color.FromArgb(107,148,180);        // 線の描画
            private static readonly Color RANGE_COLOR_OK_BACK   = Color.FromArgb(222,233,255);        // 塗りつぶし部分
            private static readonly Color RANGE_COLOR_OK_HATCH  = Color.FromArgb(185,220,255);        // 塗りつぶしハッチ

            // NGの場合
            private static readonly Color RANGE_COLOR_NG_LINE   = Color.FromArgb(220,98,170);         // 線の描画
            private static readonly Color RANGE_COLOR_NG_BACK   = Color.FromArgb(255,222,233);        // 塗りつぶし部分
            private static readonly Color RANGE_COLOR_NG_HATCH  = Color.FromArgb(255,185,220);        // 塗りつぶしハッチ

            // グラフィック
            private Graphics m_graphics;

            // 描画対象のパネル
            private Panel m_targetPanel;

            // タイムバー表示用のペン１
            private Pen m_timeBarPen1 = null;

            // タイムバー表示用のペン２
            private Pen m_timeBarPen2 = null;

            // コメント描画用のフォント
            private Font m_commentFont = null;

            // 時刻入力点表示用のブラシ
            private Brush m_inputPointBrush = null;

            // 時間区間OK表示用のペン
            private Pen m_timeRangeOkPen = null;

            // 時間区間OK表示用のブラシ
            private Brush m_timeRangeOkBrush = null;

            // 時間区間NG表示用のペン
            private Pen m_timeRangeNgPen = null;

            // 時間区間NG表示用のブラシ
            private Brush m_timeRangeNgBrush = null;

            //=========================================================================================
            // 機　能：コンストラクタ（グラフィックス指定）
            // 引　数：[in]graphics     グラフィックス
            // 　　　　[in]targetPanel  描画対象のパネル
            // 戻り値：なし
            //=========================================================================================
            public TimeRangeGraphics(Graphics graphics, Panel targetPanel) {
                m_graphics = graphics;
                m_targetPanel = targetPanel;
            }

            //=========================================================================================
            // 機　能：グラフィックスを破棄する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Dispose() {
                if (m_timeBarPen1 != null) {
                    m_timeBarPen1.Dispose();
                    m_timeBarPen1 = null;
                }
                if (m_timeBarPen2 != null) {
                    m_timeBarPen2.Dispose();
                    m_timeBarPen2 = null;
                }
                if (m_commentFont != null) {
                    m_commentFont.Dispose();
                    m_commentFont = null;
                }
                if (m_inputPointBrush != null) {
                    m_inputPointBrush.Dispose();
                    m_inputPointBrush = null;
                }
                if (m_timeRangeOkPen != null) {
                    m_timeRangeOkPen.Dispose();
                    m_timeRangeOkPen = null;
                }
                if (m_timeRangeOkBrush != null) {
                    m_timeRangeOkBrush.Dispose();
                    m_timeRangeOkBrush = null;
                }
                if (m_timeRangeNgPen != null) {
                    m_timeRangeNgPen.Dispose();
                    m_timeRangeNgPen = null;
                }
                if (m_timeRangeNgBrush != null) {
                    m_timeRangeNgBrush.Dispose();
                    m_timeRangeNgBrush = null;
                }
            }

            //=========================================================================================
            // プロパティ：グラフィックス
            //=========================================================================================
            public Graphics Graphics {
                get {
                    return m_graphics;
                }
            }

            //=========================================================================================
            // プロパティ：タイムバー表示用のペン１
            //=========================================================================================
            public Pen TimeBarPen1 {
                get {
                    if (m_timeBarPen1 == null) {
                        m_timeBarPen1 = new Pen(TIME_BAR_COLOR_SHADOW, 2.0f);
                        m_timeBarPen1.EndCap = LineCap.ArrowAnchor;
                        m_timeBarPen1.CustomEndCap = new AdjustableArrowCap(6.0f, 3.0f);
                    }
                    return m_timeBarPen1;
                }
            }

            //=========================================================================================
            // プロパティ：タイムバー表示用のペン２
            //=========================================================================================
            public Pen TimeBarPen2 {
                get {
                    if (m_timeBarPen2 == null) {
                        m_timeBarPen2 = new Pen(TIME_BAR_COLOR_MAIN, 2.0f);
                        m_timeBarPen2.EndCap = LineCap.ArrowAnchor;
                        m_timeBarPen2.CustomEndCap = new AdjustableArrowCap(6.0f, 3.0f);
                    }
                    return m_timeBarPen2;
                }
            }

            //=========================================================================================
            // プロパティ：コメント描画用のフォント
            //=========================================================================================
            public Font CommentFont {
                get {
                    if (m_commentFont == null) {
                        m_commentFont = new Font(m_targetPanel.Font.FontFamily, 8, FontStyle.Regular);
                    }
                    return m_commentFont;
                }
            }
            
            //=========================================================================================
            // プロパティ：時刻入力点表示用のブラシ
            //=========================================================================================
            public Brush InputPointBrush {
                get {
                    if (m_inputPointBrush == null) {
                        m_inputPointBrush = new SolidBrush(TIME_BAR_COLOR_SHADOW);
                    }
                    return m_inputPointBrush;
                }
            }

            //=========================================================================================
            // プロパティ：時間区間OK表示用のペン
            //=========================================================================================
            public Pen TimeRangeOkPen {
                get {
                    if (m_timeRangeOkPen == null) {
                        m_timeRangeOkPen = new Pen(RANGE_COLOR_OK_LINE, 1.0f);
                    }
                    return m_timeRangeOkPen;
                }
            }

            //=========================================================================================
            // プロパティ：時間区間OK表示用のブラシ
            //=========================================================================================
            public Brush TimeRangeOkBrush {
                get {
                    if (m_timeRangeOkBrush == null) {
                        m_timeRangeOkBrush = new HatchBrush(HatchStyle.ForwardDiagonal, RANGE_COLOR_OK_BACK, RANGE_COLOR_OK_HATCH);
                    }
                    return m_timeRangeOkBrush;
                }
            }

            //=========================================================================================
            // プロパティ：時間区間NG表示用のペン
            //=========================================================================================
            public Pen TimeRangeNgPen {
                get {
                    if (m_timeRangeNgPen == null) {
                        m_timeRangeNgPen = new Pen(RANGE_COLOR_NG_LINE, 1.0f);
                    }
                    return m_timeRangeNgPen;
                }
            }

            //=========================================================================================
            // プロパティ：時間区間NG表示用のブラシ
            //=========================================================================================
            public Brush TimeRangeNgBrush {
                get {
                    if (m_timeRangeNgBrush == null) {
                        m_timeRangeNgBrush = new HatchBrush(HatchStyle.ForwardDiagonal, RANGE_COLOR_NG_BACK, RANGE_COLOR_NG_HATCH);
                    }
                    return m_timeRangeNgBrush;
                }
            }
        }
    }
}
