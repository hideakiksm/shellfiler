using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：ファイル名の条件指定方法
    //=========================================================================================
    public class FileNameType {
        // 文字列表現からシンボルへのMap
        private static Dictionary<string, FileNameType> m_nameToSymbol = new Dictionary<string, FileNameType>();

        public static readonly FileNameType None               = new FileNameType("None");                      // 指定しない
        public static readonly FileNameType WildCard           = new FileNameType("WildCard");                  // ワイルドカード指定
        public static readonly FileNameType RegularExpression  = new FileNameType("RegularExpression");         // 正規表現指定

        // 文字列表現
        private string m_stringName;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]stringName   文字列表現
        // 戻り値：なし
        //=========================================================================================
        public FileNameType(string stringName) {
            m_stringName = stringName;
            m_nameToSymbol.Add(stringName, this);
        }
        
        //=========================================================================================
        // 機　能：文字列をシンボルに変換する
        // 引　数：[in]stringName   文字列表現
        // 戻り値：シンボル（対応するシンボルがないときnull）
        //=========================================================================================
        public static FileNameType FromString(string stringName) {
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
