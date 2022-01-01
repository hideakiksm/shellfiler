using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;

namespace ShellFiler.Api {

    //=========================================================================================
    // 列挙子：マウス以外でのファイルのマーク操作の種類
    //=========================================================================================
    public class MarkOperation {
        // 文字列表現からシンボルへのMap
        private static Dictionary<string, MarkOperation> m_nameToSymbol = new Dictionary<string, MarkOperation>();

        public static readonly MarkOperation Mark   = new MarkOperation("Mark");               // マークする
        public static readonly MarkOperation Clear  = new MarkOperation("Clear");              // マークをクリアする
        public static readonly MarkOperation Revert = new MarkOperation("Revert");             // マークを反転する
        public static readonly MarkOperation None   = new MarkOperation("None");               // 何もしない

        // 文字列表現
        private string m_stringName;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]stringName   文字列表現
        // 戻り値：なし
        //=========================================================================================
        public MarkOperation(string stringName) {
            m_stringName = stringName;
            m_nameToSymbol.Add(stringName, this);
        }
        
        //=========================================================================================
        // 機　能：文字列をシンボルに変換する
        // 引　数：[in]stringName   文字列表現
        // 戻り値：シンボル（対応するシンボルがないときnull）
        //=========================================================================================
        public static MarkOperation FromString(string stringName) {
            if (m_nameToSymbol.ContainsKey(stringName)) {
                return m_nameToSymbol[stringName];
            } else {
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：文字列表現
        //=========================================================================================
        public string StringName {
            get {
                return m_stringName;
            }
        }
    }
}
