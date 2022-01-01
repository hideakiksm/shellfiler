using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：ファイル転送のイベントで使用する詳細情報
    //=========================================================================================
    public class FileProgressEventArgs : EventArgs {
        // ファイルの総容量
        private long m_totalFileSize;

        // 転送済みファイルサイズ
        private long m_totalBytesTransferred;

        // 合計容量を何倍にするかの値（デフォルト:1）
        private int m_totalMultiple = 1;

        // 送済みファイル数
        private int m_transferedCount = 0;

        // キャンセルするときtrue
        private bool m_cancel = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]totalFileSize   ファイルの総容量
        // 　　　　[in]transferred     転送済みファイルサイズ
        // 戻り値：なし
        //=========================================================================================
        public FileProgressEventArgs(long totalFileSize, long transferred) {
            m_totalFileSize = totalFileSize;
            m_totalBytesTransferred = transferred; 
        }
        
        //=========================================================================================
        // 機　能：合計容量を指定倍にする
        // 引　数：[in]multiply   合計容量を何倍にするかの値（デフォルト:1）
        // 　　　　[in]transferd  転送済みファイル数
        // 戻り値：なし
        //=========================================================================================
        public void SetMultiply(int multiply, int transferd) {
            m_totalMultiple = multiply;
            m_transferedCount = transferd;
        }

        //=========================================================================================
        // プロパティ：ファイルの総容量
        //=========================================================================================
        public long TotalFileSize { 
            get {
                return m_totalFileSize * m_totalMultiple;
            }
        }

        //=========================================================================================
        // プロパティ：転送済みファイルサイズ
        //=========================================================================================
        public long TotalBytesTransferred {
            get {
                return m_totalBytesTransferred + m_transferedCount * m_totalFileSize;
            }
        }

        //=========================================================================================
        // プロパティ：キャンセルするときtrue
        //=========================================================================================
        public bool Cancel {
            get {
                return m_cancel;
            }
            set {
                m_cancel = value;
            }
        }
    }
}
