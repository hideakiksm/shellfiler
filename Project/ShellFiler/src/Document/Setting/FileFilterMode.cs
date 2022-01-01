using System;
using System.Collections.Generic;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：転送用フィルターのモード
    //=========================================================================================
    public class FileFilterMode {
        // 文字列表現からシンボルへのMap
        private static Dictionary<string, FileFilterMode> m_nameToSymbol = new Dictionary<string, FileFilterMode>();

        public static readonly FileFilterMode DetailMode  = new FileFilterMode("DetailMode");       // 詳細モード
        public static readonly FileFilterMode QuickMode   = new FileFilterMode("QuickMode");        // クイックモード
        public static readonly FileFilterMode DefinedMode = new FileFilterMode("DefinedMode");      // 定義済み設定モード

        // 文字列表現
        private string m_stringName;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]stringName   文字列表現
        // 戻り値：なし
        //=========================================================================================
        public FileFilterMode(string stringName) {
            m_stringName = stringName;
            m_nameToSymbol.Add(stringName, this);
        }
        
        //=========================================================================================
        // 機　能：文字列をシンボルに変換する
        // 引　数：[in]stringName   文字列表現
        // 戻り値：シンボル（対応するシンボルがないときnull）
        //=========================================================================================
        public static FileFilterMode FromString(string stringName) {
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
