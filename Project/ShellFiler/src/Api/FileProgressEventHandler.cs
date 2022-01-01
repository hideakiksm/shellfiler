using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：ファイル操作の進捗状態のイベントハンドラ
    //=========================================================================================
    public class FileProgressEventHandler {
        // イベントの型
        public delegate void EventHandlerDelegate(object sender, FileProgressEventArgs evt);
        
        // 進捗状態の通知先メソッド（ダミーのときnull）
        private EventHandlerDelegate m_target;

        // 合計容量を何倍にするかの値（デフォルト:1）
        private int m_totalMultiple = 1;

        // 送済みファイル数
        private int m_transferedCount = 0;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]target   進捗状態の通知先メソッド（ダミーのときnull）
        // 戻り値：なし
        //=========================================================================================
        public FileProgressEventHandler(EventHandlerDelegate target) {
            m_target = target;
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
        // 機　能：ファイル転送の状態を通知する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void SetProgress(object sender, FileProgressEventArgs evt) {
            evt.SetMultiply(m_totalMultiple, m_transferedCount);
            if (m_target != null) {
                m_target(sender, evt);
            }
        }

        //=========================================================================================
        // プロパティ：進捗表示を行わない、ダミーインスタンス
        //=========================================================================================
        public static FileProgressEventHandler Dummy {
            get {
                return new FileProgressEventHandler(null);
            }
        }
    }
}
