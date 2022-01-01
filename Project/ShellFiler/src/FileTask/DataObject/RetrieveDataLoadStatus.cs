using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // クラス：取得データのロード中ステータス
    //=========================================================================================
    public class RetrieveDataLoadStatus {
        public static readonly RetrieveDataLoadStatus Loading       = new RetrieveDataLoadStatus(0);        // 読み込み中
        public static readonly RetrieveDataLoadStatus CompletedAll  = new RetrieveDataLoadStatus(1);        // すべて読み込み成功
        public static readonly RetrieveDataLoadStatus CompletedPart = new RetrieveDataLoadStatus(2);        // 一部だけ読み込み成功
        public static readonly RetrieveDataLoadStatus Failed        = new RetrieveDataLoadStatus(3);        // 読み込み失敗
        
        // 優先度
        private int m_priority;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]priority  優先度
        // 戻り値：なし
        //=========================================================================================
        private RetrieveDataLoadStatus(int priority) {
            m_priority = priority;
        }
                    
        //=========================================================================================
        // 機　能：文字列化する（デバッグ用）
        // 引　数：なし
        // 戻り値：インスタンスの文字列表現
        //=========================================================================================
        public override string ToString() {
            return "" + m_priority;
        }

        //=========================================================================================
        // プロパティ：優先度
        //=========================================================================================
        public int Priority {
            get {
                return m_priority;
            }
        }
    }
}
