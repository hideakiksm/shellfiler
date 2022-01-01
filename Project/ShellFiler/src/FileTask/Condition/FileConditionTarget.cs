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
    // クラス：条件付きファイル転送で処理対象となるファイルやフォルダの種類
    //=========================================================================================
    public class FileConditionTarget {
        // 文字列表現からシンボルへのMap
        private static Dictionary<string, FileConditionTarget> m_nameToSymbol = new Dictionary<string, FileConditionTarget>();

        public static readonly FileConditionTarget FileOnly      = new FileConditionTarget("FileOnly");         // ファイル
        public static readonly FileConditionTarget FolderOnly    = new FileConditionTarget("FolderOnly");       // フォルダ
        public static readonly FileConditionTarget FileAndFolder = new FileConditionTarget("FileAndFolder");    // ファイルとフォルダ

        // 文字列表現
        private string m_stringName;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]stringName   文字列表現
        // 戻り値：なし
        //=========================================================================================
        public FileConditionTarget(string stringName) {
            m_stringName = stringName;
            m_nameToSymbol.Add(stringName, this);
        }
        
        //=========================================================================================
        // 機　能：文字列をシンボルに変換する
        // 引　数：[in]stringName   文字列表現
        // 戻り値：シンボル（対応するシンボルがないときnull）
        //=========================================================================================
        public static FileConditionTarget FromString(string stringName) {
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
