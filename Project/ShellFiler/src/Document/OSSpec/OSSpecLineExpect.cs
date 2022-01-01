using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.OSSpec {

    //=========================================================================================
    // クラス：コマンド実行時の期待値
    //=========================================================================================
    public class OSSpecLineExpect {
        // 定義の種類
        private OSSpecLineType m_lineType;

        // 期待値の定義内容
        private List<OSSpecColumnExpect> m_columnExpect = new List<OSSpecColumnExpect>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]lineType      定義の種類
        // 　　　　[in]columnExpect  期待値の定義内容
        // 戻り値：なし
        //=========================================================================================
        public OSSpecLineExpect(OSSpecLineType lineType, params OSSpecColumnExpect[] columnExpect) {
            m_lineType = lineType;
            m_columnExpect.AddRange(columnExpect);
        }

        //=========================================================================================
        // プロパティ：定義の種類
        //=========================================================================================
        public OSSpecLineType LineType {
            get {
                return m_lineType;
            }
        }

        //=========================================================================================
        // プロパティ：ls実行時の出力期待値
        //=========================================================================================
        public List<OSSpecColumnExpect> ColumnExpect {
            get {
                return m_columnExpect;
            }
        }
    }
}
