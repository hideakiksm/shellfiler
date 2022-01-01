using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Text;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイルフィルターの設定
    //=========================================================================================
    public class FileFilterDefinedMode {
        // 文字列表現からシンボルへのMap
        public static Dictionary<string, FileFilterDefinedMode> m_nameToSymbol = new Dictionary<string, FileFilterDefinedMode>();

        public static readonly FileFilterDefinedMode ShiftJISToUTF8    = new FileFilterDefinedMode("ShiftJISToUTF8");
        public static readonly FileFilterDefinedMode ShiftJISToEUC     = new FileFilterDefinedMode("ShiftJISToEUC");
        public static readonly FileFilterDefinedMode UTF8ToShiftJIS    = new FileFilterDefinedMode("UTF8ToShiftJIS");
        public static readonly FileFilterDefinedMode EUCToShiftJIS     = new FileFilterDefinedMode("EUCToShiftJIS");
        public static readonly FileFilterDefinedMode ReturnCRLF        = new FileFilterDefinedMode("ReturnCRLF");
        public static readonly FileFilterDefinedMode ReturnLF          = new FileFilterDefinedMode("ReturnLF");
        public static readonly FileFilterDefinedMode DeleteEmptyLine   = new FileFilterDefinedMode("DeleteEmptyLine");
        public static readonly FileFilterDefinedMode ShiftJIS4TabSpace = new FileFilterDefinedMode("ShiftJIS4TabSpace");
        public static readonly FileFilterDefinedMode ShiftJISSpace4Tab = new FileFilterDefinedMode("ShiftJISSpace4Tab");
        public static readonly FileFilterDefinedMode ShiftJIS8TabSpace = new FileFilterDefinedMode("ShiftJIS8TabSpace");
        public static readonly FileFilterDefinedMode ShiftJISSpace8Tab = new FileFilterDefinedMode("ShiftJISSpace8Tab");
        public static readonly FileFilterDefinedMode UTF84TabSpace     = new FileFilterDefinedMode("UTF84TabSpace");
        public static readonly FileFilterDefinedMode UTF8Space4Tab     = new FileFilterDefinedMode("UTF8Space4Tab");
        public static readonly FileFilterDefinedMode UTF88TabSpace     = new FileFilterDefinedMode("UTF88TabSpace");
        public static readonly FileFilterDefinedMode UTF8Space8Tab     = new FileFilterDefinedMode("UTF8Space8Tab");
        public static readonly FileFilterDefinedMode ShellFilerDump    = new FileFilterDefinedMode("ShellFilerDump");
        public static readonly FileFilterDefinedMode Quick1            = new FileFilterDefinedMode("Quick1");
        public static readonly FileFilterDefinedMode Quick2            = new FileFilterDefinedMode("Quick2");
        public static readonly FileFilterDefinedMode Quick3            = new FileFilterDefinedMode("Quick3");
        public static readonly FileFilterDefinedMode Quick4            = new FileFilterDefinedMode("Quick4");

        // 文字列表現
        private string m_stringName;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]stringName   文字列表現
        // 戻り値：なし
        //=========================================================================================
        public FileFilterDefinedMode(string stringName) {
            m_stringName = stringName;
            m_nameToSymbol.Add(stringName, this);
        }
        
        //=========================================================================================
        // 機　能：文字列をシンボルに変換する
        // 引　数：[in]stringName   文字列表現
        // 戻り値：シンボル（対応するシンボルがないときnull）
        //=========================================================================================
        public static FileFilterDefinedMode FromString(string stringName) {
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
