using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイル一覧サムネイルビューのファイル名の表示方法
    //=========================================================================================
    public class FileListViewThumbnailName {
        // 文字列表現からシンボルへのMap
        public static Dictionary<string, FileListViewThumbnailName> m_nameToSymbol = new Dictionary<string, FileListViewThumbnailName>();

        // 定数定義
        public static readonly FileListViewThumbnailName ThumbNameSpearate = new FileListViewThumbnailName("ThumbNameSpearate", Resources.FileListViewMode_NameSeparate);
        public static readonly FileListViewThumbnailName ThumbNameOverray  = new FileListViewThumbnailName("ThumbNameOverray",  Resources.FileListViewMode_NameOverray);
        public static readonly FileListViewThumbnailName ThumbNameNone     = new FileListViewThumbnailName("ThumbNameNone",     Resources.FileListViewMode_NameNone);

        // 文字列表現
        private string m_stringName;

        // 表示名
        private string m_displayName;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]stringName   文字列表現
        // 　　　　[in]displayName  表示名
        // 戻り値：なし
        //=========================================================================================
        private FileListViewThumbnailName(string stringName, string displayName) {
            m_stringName = stringName;
            m_displayName = displayName;
        }

        //=========================================================================================
        // 機　能：文字列をシンボルに変換する
        // 引　数：[in]stringName   文字列表現
        // 戻り値：シンボル（対応するシンボルがないときDefaultView）
        //=========================================================================================
        public static FileListViewThumbnailName FromString(string stringName) {
            if (m_nameToSymbol.ContainsKey(stringName)) {
                return m_nameToSymbol[stringName];
            } else {
                return ThumbNameSpearate;
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
        // プロパティ：表示名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
        }
    }
}
