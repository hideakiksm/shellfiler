using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Util;

namespace ShellFiler.UI.Log.Logger {

    //=========================================================================================
    // クラス：ログ出力する1行の内容（ファイル操作の結果）
    //=========================================================================================
    class LogLineFile : LogLine {
        // 選択範囲：
        private const int SELECT_NONE = 0;              // 左端
        private const int SELECT_OPERATION = 1;         // 操作種別
        private const int SELECT_OPERATION_NEXT = 2;    // 操作種別の右側の空白
        private const int SELECT_FILENAME = 3;          // ファイル名
        private const int SELECT_FILENAME_NEXT = 4;     // ファイル名の右側の空白
        private const int SELECT_STATUS = 5;            // ステータス
        private const int SELECT_ALL = 6;               // 右端

        // ログ出力するファイル操作の種類
        private FileOperationType m_operation;
        
        // ログ出力するファイルパス
        private string m_filePath;
        
        // ステータス文字列
        private FileOperationStatus m_status;

        // 進捗状況（0～100）
        private int m_progress;

        // 残り時間[秒]（-1:計算不可）
        private int m_remainingTime;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]operation    ファイル操作の種類
        // 　　　　[in]filePath     ファイルパス
        // 戻り値：なし
        //=========================================================================================
        public LogLineFile(FileOperationType operation, string filePath) {
            m_operation = operation;
            m_filePath = filePath;
            m_status = FileOperationStatus.Null;
            m_progress = 0;
        }

        //=========================================================================================
        // 機　能：ログの行内容を描画する
        // 引　数：[in]g           描画に使用するグラフィックス
        // 　　　　[in]scrLine     画面上の描画行
        // 　　　　[in]logPanel    ログパネル
        // 　　　　[in]lineContext 各行を描画するときの情報
        // 戻り値：なし
        //=========================================================================================
        public override void DrawLogLine(LogGraphics g, int scrLine, ILogViewContainer logPanel, DrawingLogLineContext lineContext) {
            // 描画内容を決定
            String strOperation = m_operation.LogString + ":";

            // 描画位置を決定
            Rectangle rcClient = logPanel.View.ClientRectangle;
            LocationX locationX = new LocationX(g, rcClient, strOperation, m_filePath, m_status);
            int logHeight = logPanel.LogLineHeight;
            int yPos = scrLine * logHeight;

            Font font = g.LogWindowFont;
            Font fontBold = g.LogWindowBoldFont;

            for (int i = 0; i < 2; i++) {
                // 0:通常、1:選択
                if (i == 0 && lineContext.SelectionStart == SELECT_NONE && lineContext.SelectionEnd >= SELECT_ALL) {
                    continue;
                } else if (i == 1 && (lineContext.SelectionStart == -1 || lineContext.SelectionStart == lineContext.SelectionEnd)) {
                    continue;
                }

                // リソースを選択
                Brush backBrush, fontBrush, fontStatusBrush;
                Pen leaderPen;
                if (i == 0) {
                    backBrush = g.LogWindowBackBrush;
                    fontBrush = g.LogWindowTextBrush;
                    fontStatusBrush = GetLogStatusFontBrush(g);
                    leaderPen = g.LogWindowLeaderPen;
                } else {
                    backBrush = g.LogWindowSelectBackBrush;
                    fontBrush = g.LogWindowSelectTextBrush;
                    fontStatusBrush = g.LogWindowSelectTextBrush;
                    leaderPen = g.LogWindowLeaderSelectPen;
                }

                // 領域を選択
                if (i == 0 && (lineContext.SelectionStart == -1 || lineContext.SelectionStart == lineContext.SelectionEnd)) {
                    ;
                } else {
                    int xStart = locationX.SelectToPoint(lineContext.SelectionStart);
                    int xEnd = locationX.SelectToPoint(lineContext.SelectionEnd);
                    g.Graphics.SetClip(rcClient, CombineMode.Exclude);
                    if (i == 0) {
                        g.Graphics.SetClip(new Rectangle(0, yPos, xStart - 1, logHeight), CombineMode.Xor);
                        g.Graphics.SetClip(new Rectangle(xEnd + 1, yPos, rcClient.Width - xEnd, logHeight), CombineMode.Xor);
                    } else {
                        g.Graphics.SetClip(new Rectangle(xStart, yPos, xEnd - xStart + 1, logHeight), CombineMode.Xor);
                    }
                }

                // 描画
                if (i == 1) {
                    g.Graphics.FillRectangle(backBrush, new Rectangle(0, yPos, rcClient.Width, logHeight));
                }
                g.Graphics.DrawString(strOperation, fontBold, fontBrush, new RectangleF(locationX.X1OperationType, yPos, locationX.CxOperationType, fontBold.Height), g.StringFormatEllipsis);
                g.Graphics.DrawString(m_filePath, font, fontBrush, new RectangleF(locationX.X1FileName, yPos, locationX.CxFileName, font.Height), g.StringFormatEllipsis);

                if (m_status != FileOperationStatus.Processing) {
                    // ステータスを描画
                    Rectangle rcStatus = new Rectangle(locationX.X1Status, yPos, locationX.CxStatus, logHeight);
                    g.Graphics.FillRectangle(backBrush, rcStatus);
                    g.Graphics.DrawString(m_status.Message, font, fontStatusBrush, rcStatus, g.StringFormatEllipsis);
                }
                if (m_status != FileOperationStatus.Null) {
                    // リーダーを描画
                    int x1Leader = (locationX.X1FileName + Math.Min(locationX.SizeFileName, locationX.CxFileName) + LogLineRenderer.CX_LEADER_MARGIN) / 2 * 2;
                    int x2Leader = locationX.X1Status - LogLineRenderer.CX_LEADER_MARGIN;
                    int yLeader = yPos + g.LogWindowFont.Height / 2;
                    if (x1Leader < x2Leader) {
                        g.Graphics.DrawLine(leaderPen, x1Leader, yLeader, x2Leader, yLeader);
                    }
                }
            }

            // 進捗状況を描画
            if (m_status == FileOperationStatus.Processing) {
                int yPosProgress = yPos + 1;
                int cyProgress = font.Height - 2;
                DrawProgress(g, new Rectangle(locationX.X1Status, yPosProgress, locationX.CxStatus - 2, cyProgress), m_progress);
            }
            g.Graphics.SetClip(rcClient);
        }

        //=========================================================================================
        // 機　能：ログのステータス描画用のブラシを取得する
        // 引　数：[in]g    描画に使用するグラフィックス
        // 戻り値：描画用のブラシ
        //=========================================================================================
        public Brush GetLogStatusFontBrush(LogGraphics g) {
            if (m_status == null) {
                Program.Abort("NullPointerを検出しました。");
            }
            if (m_status.Level == FileOperationStatus.LogLevel.Error) {
                return g.LogErrorTextBrush;
            } else {
                return g.LogWindowTextBrush;
            }
        }

        //=========================================================================================
        // 機　能：進捗率を描画する
        // 引　数：[in]g         描画に使用するグラフィックス
        // 　　　　[in]rect      進捗率の表示領域
        // 　　　　[in]progress  進捗率
        // 戻り値：なし
        //=========================================================================================
        private void DrawProgress(LogGraphics g, Rectangle rect, int progress) {
            // 左側の進捗率
            int cxProgressString = (int)(GraphicsUtils.MeasureString(g.Graphics, g.LogWindowFont, "100%") + 3);
            string strProgress = null;
            strProgress = progress + "%";
            g.Graphics.FillRectangle(g.LogWindowBackBrush, new Rectangle(rect.Left, rect.Top, cxProgressString, rect.Height));
            g.Graphics.DrawString(strProgress, g.LogWindowFont, g.LogWindowTextBrush, rect);

            // 右側のグラフ
            int all = rect.Width - cxProgressString - 16;
            Rectangle rcBar = new Rectangle(rect.Left + cxProgressString, rect.Top, all, rect.Height - 1);
            Rectangle rcBar1 = new Rectangle(rcBar.Left + 1, rect.Top + 1, all * progress / 100 - 1, rect.Height - 2);
            Rectangle rcBar2 = new Rectangle(rcBar1.Right, rect.Top + 1, all - rcBar1.Width - 1, rect.Height - 2);
            if (rcBar1.Width > 0) {
                LinearGradientBrush brush1 = new LinearGradientBrush(rcBar1, Configuration.Current.LogWindowProgressColor1, Configuration.Current.LogWindowProgressColor2, LinearGradientMode.Vertical);
                g.Graphics.FillRectangle(brush1, rcBar1);
            }
            if (rcBar2.Width > 0) {
                LinearGradientBrush brush2 = new LinearGradientBrush(rcBar2, Configuration.Current.LogWindowProgressColor3, Configuration.Current.LogWindowProgressColor4, LinearGradientMode.Vertical);
                g.Graphics.FillRectangle(brush2, rcBar2);
            }
            g.Graphics.DrawRectangle(g.LogWindowProgressPen, rcBar);

            // 残り時間
            string strRemaining = ConvertRemainingTime(m_remainingTime);
            float remainingX = rcBar.Left + (rcBar.Width - GraphicsUtils.MeasureString(g.Graphics, g.LogWindowRemainingTimeFont, strRemaining)) / 2;
            float remainingY = rcBar.Top + (rcBar.Height - g.LogWindowRemainingTimeFont.Height) / 2 + 1;
            g.Graphics.DrawString(strRemaining, g.LogWindowRemainingTimeFont, g.LogWindowRemainingTimeTextBrush2, new PointF(remainingX + 1, remainingY + 1));
            g.Graphics.DrawString(strRemaining, g.LogWindowRemainingTimeFont, g.LogWindowRemainingTimeTextBrush1, new PointF(remainingX, remainingY));
        }

        //=========================================================================================
        // 機　能：残り時間を表示形式に変換する
        // 引　数：[in]timeSeconds   残り時間
        // 戻り値：表示形式
        //=========================================================================================
        private string ConvertRemainingTime(int timeSeconds) {
            if (timeSeconds < 0) {
                return "";
            } else if (timeSeconds >= 3600) {
                return string.Format("{0}:{1:00}:{2:00}", timeSeconds / 3600, (timeSeconds / 60) % 60, timeSeconds % 60);
            } else {
                return string.Format("{0}:{1:00}", (timeSeconds / 60) % 60, timeSeconds % 60);
            }
        }

        //=========================================================================================
        // 機　能：クリック位置の桁情報を返す
        // 引　数：[in]logPanel   ログパネル
        // 　　　　[in]g          桁位置計測用のグラフィックス
        // 　　　　[in]cursorPos  マウスカーソルの座標
        // 　　　　[out]column    カラム位置を返す変数
        // 　　　　[out]onChar    文字の上の位置にいるときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        public override void GetMouseHitColumn(ILogViewContainer logPanel, LogGraphics g, Point cursorPos, out int column, out bool onChar) {
            LocationX locationX = new LocationX(g, logPanel.View.ClientRectangle, m_operation.LogString, m_filePath, m_status);
            column = locationX.PointToSelect(cursorPos.X);
            onChar = false;
        }

        //=========================================================================================
        // 機　能：選択中の行の内容を文字列で返す
        // 引　数：[in]startColumn   選択開始カラム（全選択のとき0）
        // 　　　　[in]endColumn     選択終了カラム（全選択のときint.MaxValue）
        // 戻り値：選択されている文字列（削除済みの行のときnull）
        //=========================================================================================
        public override string GetSelectedLine(int startColumn, int endColumn) {
            if (startColumn == 0) {
                startColumn = SELECT_OPERATION;
            } else if (startColumn > SELECT_STATUS) {
                startColumn = SELECT_STATUS;
            }
            if (endColumn == 0) {
                endColumn = SELECT_OPERATION;
            } else if (endColumn > SELECT_STATUS) {
                endColumn = SELECT_STATUS;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = startColumn; i <= endColumn; i++) {
                switch (i) {
                    case SELECT_OPERATION:              // 操作種別
                        sb.Append(m_operation.LogString);
                        break;
                    case SELECT_OPERATION_NEXT:         // 操作種別の右側の空白
                        sb.Append("\t");
                        break;
                    case SELECT_FILENAME:               // ファイル名
                        sb.Append(m_filePath);
                        break;
                    case SELECT_FILENAME_NEXT:          // ファイル名の右側の空白
                        sb.Append("\t");
                        break;
                    case SELECT_STATUS:                 // ステータス
                        if (m_status != FileOperationStatus.Processing) {
                            sb.Append(m_status.Message);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：文字列化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override string ToString() {
            return m_operation + ":" + m_filePath + ":" + m_status;
        }

        //=========================================================================================
        // プロパティ：ログ出力するファイル操作の種類
        //=========================================================================================
        public FileOperationType OperationType {
            get {
                return  m_operation;
            }
        }
        
        //=========================================================================================
        // プロパティ：ログ出力するファイルパス
        //=========================================================================================
        public string TargetPath {
            get {
                return m_filePath;
            }
        }

        //=========================================================================================
        // プロパティ：ステータスを表すメッセージ
        //=========================================================================================
        public FileOperationStatus Status {
            set {
                m_status = value;
            }
        }

        //=========================================================================================
        // プロパティ：進捗状況（0～100）
        //=========================================================================================
        public int Progress {
            set {
                m_progress = value;
            }
        }

        //=========================================================================================
        // プロパティ：残り時間[秒]
        //=========================================================================================
        public int RemainingTime {
            set {
                m_remainingTime = value;
            }
        }

        //=========================================================================================
        // クラス：描画対象のX座標の情報
        //=========================================================================================
        private class LocationX {
            // クライアント領域の大きさ
            private Rectangle m_rcClient;

            // 操作の文字列のサイズ
            private int m_sizeOperation;

            // ファイル名の文字列のサイズ
            private int m_sizeFileName;
            
            // ステータスの文字列または進捗表示のサイズ
            private int m_sizeStatus;
            
            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]g          グラフィックス
            // 　　　　[in]rcClient   クライアント領域の大きさ
            // 　　　　[in]operation  操作の文字列
            // 　　　　[in]filePath   ファイル名
            // 　　　　[in]status     ステータス
            // 戻り値：なし
            //=========================================================================================
            public LocationX(LogGraphics g, Rectangle rcClient, string operation, string filePath, FileOperationStatus status) {
                m_rcClient = rcClient;
                Font font = g.LogWindowFont;
                Font fontBold = g.LogWindowBoldFont;
                m_sizeOperation = TextRendererUtils.MeasureStringJustInt(g.Graphics, fontBold, operation);
                m_sizeFileName = TextRendererUtils.MeasureStringJustInt(g.Graphics, font, filePath);
                if (status != FileOperationStatus.Processing) {
                    m_sizeStatus = TextRendererUtils.MeasureStringJustInt(g.Graphics, font, status.Message);
                } else {
                    m_sizeStatus = CxStatus;
                }
            }

            //=========================================================================================
            // 機　能：選択位置を座標に変換する
            // 引　数：[in]select   選択位置
            // 戻り値：X座標
            //=========================================================================================
            public int SelectToPoint(int select) {
                int xStart;
                switch (select) {
                    default:
                    case -1:
                        xStart = X1OperationType;
                        break;
                    case SELECT_NONE:                  // 左端
                        xStart = X1OperationType;
                        break;
                    case SELECT_OPERATION:             // 操作種別
                        xStart = X1OperationType;
                        break;
                    case SELECT_OPERATION_NEXT:        // 操作種別の右側の空白
                        xStart = X1OperationType + m_sizeOperation;
                        break;
                    case SELECT_FILENAME:              // ファイル名
                        xStart = X1FileName;
                        break;
                    case SELECT_FILENAME_NEXT:         // ファイル名の右側の空白
                        xStart = X1FileName + m_sizeFileName;
                        break;
                    case SELECT_STATUS:                // ステータス
                        xStart = X1Status;
                        break;
                    case SELECT_ALL:                   // 右端
                    case int.MaxValue:
                        xStart = X1Status + m_sizeStatus;
                        break;
                }
                return xStart;
            }

            //=========================================================================================
            // 機　能：座標を選択位置に変換する
            // 引　数：[in]xPos   X座標
            // 戻り値：選択位置
            //=========================================================================================
            public int PointToSelect(int xPos) {
                int select;
                if (xPos < X1OperationType) {                                   // 左端
                    select = SELECT_NONE;
                } else if (xPos < X1OperationType + m_sizeOperation) {          // 操作種別
                    select = SELECT_OPERATION;
                } else if (xPos < X1FileName) {                                 // 操作種別の右側の空白
                    select = SELECT_OPERATION_NEXT;
                } else if (xPos < X1FileName + m_sizeFileName) {                // ファイル名
                    select = SELECT_FILENAME;
                } else if (xPos < X1Status) {                                   // ファイル名の右側の空白
                    select = SELECT_FILENAME_NEXT;
                } else if (xPos < X1Status + m_sizeStatus) {                    // ステータス
                    select = SELECT_STATUS;
                } else {                                                        // 右端
                    select = SELECT_ALL;
                }
                return select;
            }
 
            //=========================================================================================
            // プロパティ：操作種別の描画幅
            //=========================================================================================
            public int CxOperationType {
                get {
                    return LogPanel.CxOperationString;
                }
            }

            //=========================================================================================
            // プロパティ：操作種別の描画開始X座標
            //=========================================================================================
            public int X1OperationType {
                get {
                    return LogLineRenderer.CX_LEFT_RIGHT_MARGIN;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル名の描画幅
            //=========================================================================================
            public int CxFileName {
                get {
                    return m_rcClient.Width - CxOperationType - CxStatus - LogLineRenderer.CX_LEADER - LogLineRenderer.CX_LEFT_RIGHT_MARGIN * 2;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル名の描画開始X座標
            //=========================================================================================
            public int X1FileName {
                get {
                    return LogLineRenderer.CX_LEFT_RIGHT_MARGIN + LogPanel.CxOperationString;
                }
            }

            //=========================================================================================
            // プロパティ：ステータスの描画幅
            //=========================================================================================
            public int CxStatus {
                get {
                    return LogPanel.CxStatusString;
                }
            }

            //=========================================================================================
            // プロパティ：ステータスの描画開始X座標
            //=========================================================================================
            public int X1Status {
                get {
                    return m_rcClient.Width - LogLineRenderer.CX_LEFT_RIGHT_MARGIN - LogPanel.CxStatusString;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル名の領域サイズ
            //=========================================================================================
            public int SizeFileName {
                get {
                    return m_sizeFileName;
                }
            }
        }
    }
}
