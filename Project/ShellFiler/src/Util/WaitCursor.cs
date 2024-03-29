﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：待機状態のカーソル
    //=========================================================================================
    class WaitCursor : IDisposable {
        // 旧カーソル
        private Cursor m_old = null; 

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public WaitCursor() {
            m_old = Cursor.Current; 
            Cursor.Current = Cursors.WaitCursor; 
        }

        //=========================================================================================
        // 機　能：後処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            Cursor.Current = m_old; 
        }
    }
}