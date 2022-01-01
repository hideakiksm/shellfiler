using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // クラス：ファイルビューアの対象ファイルの状態が変わったことを通知するイベント
    //=========================================================================================
    public class AccessibleFileStatusChangedEventArgs : EventArgs {
        // チャンクが読み込まれたときtrue
        private bool m_chunkLoaded;

        // ステータスが変わったときtrue
        private bool m_statusChanged;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]chunkLoaded   チャンクが読み込まれたときtrue
        // 　　　　[in]statusChanged ステータスが変わったときtrue
        // 戻り値：なし
        //=========================================================================================
        public AccessibleFileStatusChangedEventArgs(bool chunkLoaded, bool statusChanged) {
            m_chunkLoaded = chunkLoaded;
            m_statusChanged = statusChanged;
        }

        //=========================================================================================
        // プロパティ：チャンクが読み込まれたときtrue
        //=========================================================================================
        public bool ChunkLoaded { 
            get {
                return m_chunkLoaded;
            }
        }
        
        //=========================================================================================
        // プロパティ：ステータスが変わったときtrue
        //=========================================================================================
        public bool StatusChanged { 
            get {
                return m_statusChanged;
            }
        }

        //=========================================================================================
        // 列挙子：変化の種類
        //=========================================================================================
        public enum AccessibleFileChangeType {
            ChunkLoaded,             // ファイルが読み込まれた
            StatusChanged,          // ステータスが変更された
        }
    }
}
