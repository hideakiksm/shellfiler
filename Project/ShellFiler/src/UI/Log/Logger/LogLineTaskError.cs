using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Management;
using ShellFiler.Util;

namespace ShellFiler.UI.Log.Logger {

    //=========================================================================================
    // クラス：ログ出力する1行の内容（単純な文字列）
    //=========================================================================================
    public class LogLineTaskError : LogLine {
        // エラーを起こしたバックグラウンドタスクのID
        private BackgroundTaskID m_taskId;

        // メッセージの内容
        private string m_message;

        // 詳細リンクの領域（Y座標は未使用）
        private Rectangle m_rcLink;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]message  メッセージの内容
        // 　　　　[in]arg      可変引数
        // 戻り値：なし
        //=========================================================================================
        public LogLineTaskError(BackgroundTaskID taskId, string message, params object[] arg) {
            m_taskId = taskId;
            m_message = String.Format(message, arg);
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
            LogLineRenderer renderer = new LogLineRenderer(m_message, Resources.LogLine_TaskError, g, g.LogWindowLinkFont);
            renderer.DrawLogLine(g, g.LogErrorTextBrush, scrLine, logPanel, lineContext);
            m_rcLink = renderer.LinkRectangle;
        }

        //=========================================================================================
        // 機　能：クリック可能かどうかを調べる
        // 引　数：[in]cursorPos  マウスカーソルの座標
        // 戻り値：クリック可能なときtrue
        //=========================================================================================
        public override bool HitTest(Point cursorPos) {
            if (m_rcLink.Left <= cursorPos.X && cursorPos.X <= m_rcLink.Right) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：マウスがクリックされたときの処理を行う
        // 引　数：[in]cursorPos  マウスカーソルの座標
        // 戻り値：なし
        //=========================================================================================
        public override void OnClick(Point cursorPos) {
            if (cursorPos.X < m_rcLink.Left || m_rcLink.Right < cursorPos.X) {
                return;
            }
            FileErrorInfo errorInfo = Program.Document.BackgroundTaskManager.GetBackgroundDetailErrorInfo(m_taskId);
            if (errorInfo == null) {
                string message = string.Format(Resources.LogLine_ErrorInfoCleared, BackgroundTaskManager.MAX_ERROR_INFO_TASK_COUNT);
                InfoBox.Information(Program.MainWindow, message);
            } else {
                Program.MainWindow.ShowFileErrorDialog(errorInfo);
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
            LogLineRenderer renderer = new LogLineRenderer(m_message, Resources.LogLine_TaskError, g, g.LogWindowLinkFont);
            renderer.GetMouseHitColumn(logPanel, g, cursorPos, out column, out onChar);
        }

        //=========================================================================================
        // 機　能：選択中の行の内容を文字列で返す
        // 引　数：[in]startColumn   選択開始カラム（全選択のとき0）
        // 　　　　[in]endColumn     選択終了カラム（全選択のときint.MaxValue）
        // 戻り値：選択されている文字列（削除済みの行のときnull）
        //=========================================================================================
        public override string GetSelectedLine(int startColumn, int endColumn) {
            StringBuilder sb = new StringBuilder();
            if (startColumn <= m_message.Length) {
                int end = Math.Min(endColumn, m_message.Length);
                sb.Append(m_message.Substring(startColumn, end - startColumn));
            }
            if (startColumn <= m_message.Length + 1 && m_message.Length + 1 <= endColumn) {
                sb.Append(" ");
            }
            if (startColumn <= m_message.Length + 2 && m_message.Length + 2 <= endColumn) {
                sb.Append(Resources.LogLine_TaskError);
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：文字列化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override string ToString() {
            return m_message;
        }
    }
}
