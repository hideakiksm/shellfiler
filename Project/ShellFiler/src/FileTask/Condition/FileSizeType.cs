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
    // クラス：ファイルサイズの条件指定方法
    //=========================================================================================
    public class FileSizeType {
        // 文字列表現からシンボルへのMap
        private static Dictionary<string, FileSizeType> m_nameToSymbol = new Dictionary<string, FileSizeType>();

        public static readonly FileSizeType None        = new FileSizeType("None");             // 指定しない
        public static readonly FileSizeType XxxSize     = new FileSizeType("XxxSize");          // 指定サイズ以下
        public static readonly FileSizeType SizeXxx     = new FileSizeType("SizeXxx");          // 指定サイズ以上
        public static readonly FileSizeType SizeXxxSize = new FileSizeType("SizeXxxSize");      // 指定範囲
        public static readonly FileSizeType XxxSizeXxx  = new FileSizeType("XxxSizeXxx");       // 指定範囲以外
        public static readonly FileSizeType Size        = new FileSizeType("Size");             // 指定サイズ

        // 文字列表現
        private string m_stringName;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]stringName   文字列表現
        // 戻り値：なし
        //=========================================================================================
        public FileSizeType(string stringName) {
            m_stringName = stringName;
            m_nameToSymbol.Add(stringName, this);
        }
        
        //=========================================================================================
        // 機　能：文字列をシンボルに変換する
        // 引　数：[in]stringName   文字列表現
        // 戻り値：シンボル（対応するシンボルがないときnull）
        //=========================================================================================
        public static FileSizeType FromString(string stringName) {
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
