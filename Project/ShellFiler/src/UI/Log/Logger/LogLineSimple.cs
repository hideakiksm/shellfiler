using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;

namespace ShellFiler.UI.Log.Logger {

    //=========================================================================================
    // クラス：ログ出力する1行の内容（単純な文字列）
    //=========================================================================================
    public class LogLineSimple : LogLine {
        // メッセージの内容
        private string m_message;

        // ログの出力色
        private LogColor m_logColor;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]message  メッセージの内容
        // 　　　　[in]arg      可変引数
        // 戻り値：なし
        //=========================================================================================
        public LogLineSimple(string message, params object[] arg) {
            m_logColor = LogColor.Normal;
            m_message = String.Format(message, arg);
        }
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]color    色の種類
        // 　　　　[in]message  メッセージの内容
        // 　　　　[in]arg      可変引数
        // 戻り値：なし
        //=========================================================================================
        public LogLineSimple(LogColor color, string message, params object[] arg) {
            m_logColor = color;
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
            Brush fontBrush = null;
            if (m_logColor == LogColor.Normal) {
                fontBrush = g.LogWindowTextBrush;
            } else if (m_logColor == LogColor.Error) {
                fontBrush = g.LogErrorTextBrush;
            } else if (m_logColor == LogColor.StdError) {
                fontBrush = g.LogStdErrorTextBrush;
            }
            LogLineRenderer renderer = new LogLineRenderer(m_message, null, null, null);
            renderer.DrawLogLine(g, fontBrush, scrLine, logPanel, lineContext);
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
            LogLineRenderer renderer = new LogLineRenderer(m_message, null, null, null);
            renderer.GetMouseHitColumn(logPanel, g, cursorPos, out column, out onChar);
        }
        
        //=========================================================================================
        // 機　能：選択中の行の内容を文字列で返す
        // 引　数：[in]startColumn   選択開始カラム（全選択のとき0）
        // 　　　　[in]endColumn     選択終了カラム（全選択のときint.MaxValue）
        // 戻り値：選択されている文字列（削除済みの行のときnull）
        //=========================================================================================
        public override string GetSelectedLine(int startColumn, int endColumn) {
            endColumn = Math.Min(endColumn, m_message.Length);
            string sub = m_message.Substring(startColumn, endColumn - startColumn);
            return sub;
        }

        //=========================================================================================
        // 機　能：文字列化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override string ToString() {
            return m_message;
        }

        //=========================================================================================
        // プロパティ：メッセージ
        //=========================================================================================
        public string Message {
            get {
                return m_message;
            }
            set {
                m_message = value;
            }
        }
    }
}
