using System;
using System.Collections.Generic;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;

namespace ShellFiler.UI.Log {

    //=========================================================================================
    // クラス：ログ出力に使用するバッファ
    // ログIDはLogLineのstaticで振り出される全体でユニークな値
    // 行IDは各LogBufferごとの行を識別するために使用されるバッファごとにユニークな連続値
    //=========================================================================================
    public class LogBuffer {
        // ログ出力する各行の内容（リングバッファ）
        private LogLine[] m_logBuffer;

        // ログIDから行IDへのmap（リングバッファと連動した管理）
        private Dictionary<long, long> m_logIdToLineId;

        // 次に発行する行ID（行IDをバッファサイズで割った余りでバッファにアクセス）
        private long m_nextLineId = 0;

        // リングバッファ上の次の追加位置
        private int m_nextBufferIndex = 0;

        // バッファ上に存在する行数（１行追加するごとに１増えていき、最大m_logBuffer.Length）
        private int m_availableBufferCount = 0;

        // リングバッファの[0]を開始番号として振り出した行ID（バッファをクリアしたときに設定、連続値）
        private long m_firstLineId = 0;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]logLineCount  リングバッファのサイズ
        // 戻り値：なし
        //=========================================================================================
        public LogBuffer(int logLineCount) {
            // リングバッファを初期化
            m_logBuffer = new LogLine[logLineCount];
            for (int i = 0; i < logLineCount; i++) {
                m_logBuffer[i] = null;
            }
            m_logIdToLineId = new Dictionary<long, long>(logLineCount);
            m_nextLineId = 0;
            m_nextBufferIndex = 0;
        }

        //=========================================================================================
        // 機　能：ログの内容を追加する
        // 引　数：[in]logLine   追加する行情報
        // 戻り値：バッファの行ID
        //=========================================================================================
        public long AddLogLine(LogLine logLine) {
            m_availableBufferCount = Math.Min(m_availableBufferCount + 1, m_logBuffer.Length);
            if (m_logBuffer[m_nextBufferIndex] != null) {
                m_logIdToLineId.Remove(m_logBuffer[m_nextBufferIndex].LogId);
            }
            long lineId = m_nextLineId;
            m_logBuffer[m_nextBufferIndex] = logLine;
            m_logIdToLineId[logLine.LogId] = lineId;
            m_nextBufferIndex = (m_nextBufferIndex + 1) % m_logBuffer.Length;
            m_nextLineId++;
            return lineId;
        }

        //=========================================================================================
        // 機　能：バッファの行IDからログの内容を取得する
        // 引　数：[in]lineId  行ID
        // 戻り値：バッファの行
        //=========================================================================================
        public LogLine LineIdToLogLine(long logId) {
            if (logId < FirstAvailableLineId || logId >= FirstAvailableLineId + AvailableLogCount) {
                return null;
            }
            int targetIndex = (int)((logId - m_firstLineId) % m_logBuffer.Length);
            return m_logBuffer[targetIndex];
        }

        //=========================================================================================
        // 機　能：ログIDからログの内容を取得する
        // 引　数：[in]logId  ログID
        // 戻り値：行ID（見つからないときは-1）
        //=========================================================================================
        public long LogIdToLineId(long logId) {
            if (m_logIdToLineId.ContainsKey(logId)) {
                return m_logIdToLineId[logId];
            } else {
                return -1;
            }
        }

        //=========================================================================================
        // 機　能：ログの内容をすべて削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearAll() {
            m_nextBufferIndex = 0;
            m_availableBufferCount = 0;
            m_firstLineId = m_nextLineId;
            for (int i = 0; i < m_logBuffer.Length; i++) {
                m_logBuffer[i] = null;
            }
            m_logIdToLineId.Clear();
        }

        //=========================================================================================
        // プロパティ：利用可能なログ行数
        //=========================================================================================
        public int AvailableLogCount {
            get {
                return m_availableBufferCount;
            }
        }

        //=========================================================================================
        // プロパティ：有効な行IDの先頭（1行も利用できないときは次に有効になるID）
        //=========================================================================================
        public long FirstAvailableLineId {
            get {
                if (m_availableBufferCount == m_logBuffer.Length) {
                    return m_nextLineId - m_availableBufferCount;
                } else {
                    return m_firstLineId;
                }
            }
        }

        //=========================================================================================
        // プロパティ：次に発行する行ID（行IDをバッファサイズで割った余りでバッファにアクセス）
        //=========================================================================================
        public long NextLogId {
            get {
                return m_nextLineId;
            }
        }
    }
}
