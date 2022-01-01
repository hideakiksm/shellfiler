using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：すべてのファイルをマークするモード
    //=========================================================================================
    public class MarkAllFileMode {
        // 文字列表現からシンボルへのMap
        private static Dictionary<string, MarkAllFileMode> m_nameToSymbol = new Dictionary<string, MarkAllFileMode>();

        public static readonly MarkAllFileMode ClearAll        = new MarkAllFileMode("ClearAll",        true,  true,  MarkOperation.Clear);
        public static readonly MarkAllFileMode SelectAll       = new MarkAllFileMode("SelectAll",       true,  true,  MarkOperation.Mark);
        public static readonly MarkAllFileMode RevertAll       = new MarkAllFileMode("RevertAll",       true,  true,  MarkOperation.Revert);
        public static readonly MarkAllFileMode ClearAllFolder  = new MarkAllFileMode("ClearAllFolder",  true,  false, MarkOperation.Clear);
        public static readonly MarkAllFileMode SelectAllFolder = new MarkAllFileMode("SelectAllFolder", true,  false, MarkOperation.Mark);
        public static readonly MarkAllFileMode RevertAllFolder = new MarkAllFileMode("RevertAllFolder", true,  false, MarkOperation.Revert);
        public static readonly MarkAllFileMode ClearAllFile    = new MarkAllFileMode("ClearAllFile",    false, true,  MarkOperation.Clear);
        public static readonly MarkAllFileMode SelectAllFile   = new MarkAllFileMode("SelectAllFile",   false, true,  MarkOperation.Mark);
        public static readonly MarkAllFileMode RevertAllFile   = new MarkAllFileMode("RevertAllFile",   false, true,  MarkOperation.Revert);
        
        // 文字列表現
        private string m_stringName;

        // ディレクトリがターゲットのときtrue
        private bool m_targetDirectory;

        // ファイルがターゲットのときtrue
        private bool m_targetFile;

        // マーク操作
        private MarkOperation m_operation;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]stringName   文字列表現
        // 　　　　[in]targetDir   ディレクトリがターゲットのときtrue
        // 　　　　[in]targetFile  ファイルがターゲットのときtrue
        // 　　　　[in]operation   マーク操作
        // 戻り値：反転したマークの数
        //=========================================================================================
        private MarkAllFileMode(string stringName, bool targetDir, bool targetFile, MarkOperation operation) {
            m_stringName = stringName;
            m_nameToSymbol.Add(stringName, this);
            m_targetDirectory = targetDir;
            m_targetFile = targetFile;
            m_operation = operation;
        }

        //=========================================================================================
        // 機　能：文字列をシンボルに変換する
        // 引　数：[in]stringName   文字列表現
        // 戻り値：シンボル（対応するシンボルがないときnull）
        //=========================================================================================
        public static MarkAllFileMode FromString(string stringName) {
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
        
        //=========================================================================================
        // ディレクトリがターゲットのときtrue
        //=========================================================================================
        public bool TargetDirectory {
            get {
                return m_targetDirectory;
            }
        }

        //=========================================================================================
        // ファイルがターゲットのときtrue
        //=========================================================================================
        public bool TargetFile {
            get {
                return m_targetFile;
            }
        }

        //=========================================================================================
        // マーク操作
        //=========================================================================================
        public MarkOperation Operation {
            get {
                return m_operation;
            }
        }
    }
}
