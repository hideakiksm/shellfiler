using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイル一覧サムネイルビューのサイズ
    //=========================================================================================
    public class FileListViewIconSize {
        // 文字列表現からシンボルへのMap
        public static Dictionary<string, FileListViewIconSize> m_nameToSymbol = new Dictionary<string, FileListViewIconSize>();

        // 定数定義
        public static readonly FileListViewIconSize IconSizeNull = new FileListViewIconSize(null,           0,  null, null);
        public static readonly FileListViewIconSize IconSize16   = new FileListViewIconSize("IconSize16",   16, IconSize.Small16,      Resources.FileListViewMode_Thumb16);
        public static readonly FileListViewIconSize IconSize32   = new FileListViewIconSize("IconSize32",   32, IconSize.Large32,      Resources.FileListViewMode_Thumb32);
        public static readonly FileListViewIconSize IconSize48   = new FileListViewIconSize("IconSize48",   48, IconSize.ExtraLarge48, Resources.FileListViewMode_Thumb48);
        public static readonly FileListViewIconSize IconSize64   = new FileListViewIconSize("IconSize64",   64, IconSize.ExtraLarge48, Resources.FileListViewMode_Thumb64);
        public static readonly FileListViewIconSize IconSize128  = new FileListViewIconSize("IconSize128", 128, IconSize.ExtraLarge48, Resources.FileListViewMode_Thumb128);
        public static readonly FileListViewIconSize IconSize256  = new FileListViewIconSize("IconSize256", 256, IconSize.Jumbo256,     Resources.FileListViewMode_Thumb256);

        // 全サイズ
        public static readonly FileListViewIconSize[] AllSize = {
            IconSize16, IconSize32, IconSize48, IconSize64, IconSize128, IconSize256,
        };

        // 文字列表現
        private string m_stringName;
        
        // 画像サイズ
        private int m_imageSize;
        
        // アイコンサイズ
        private IconSize m_iconSize;

        // 表示名
        private string m_displayName;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]stringName   文字列表現
        // 　　　　[in]imageSize    画像サイズ
        // 　　　　[in]iconSize     アイコンサイズ
        // 　　　　[in]displayName  表示名
        // 戻り値：なし
        //=========================================================================================
        private FileListViewIconSize(string stringName, int imageSize, IconSize iconSize, string displayName) {
            m_stringName = stringName;
            m_imageSize = imageSize;
            m_iconSize = iconSize;
            m_displayName = displayName;
        }

        //=========================================================================================
        // 機　能：文字列をシンボルに変換する
        // 引　数：[in]stringName   文字列表現
        // 戻り値：シンボル（対応するシンボルがないときデフォルト）
        //=========================================================================================
        public static FileListViewIconSize FromString(string stringName) {
            if (m_nameToSymbol.ContainsKey(stringName)) {
                return m_nameToSymbol[stringName];
            } else {
                return IconSize48;
            }
        }

        //=========================================================================================
        // 機　能：サイズをシンボルに変換する
        // 引　数：[in]size  サイズ
        // 戻り値：シンボル（対応するシンボルがないときエラー）
        //=========================================================================================
        public static FileListViewIconSize FromSize(Size size) {
            if (size.Width == 16 && size.Height == 16) {
                return FileListViewIconSize.IconSize16;
            } else if (size.Width == 32 && size.Height == 32) {
                return FileListViewIconSize.IconSize32;
            } else if (size.Width == 48 && size.Height == 48) {
                return FileListViewIconSize.IconSize48;
            } else if (size.Width == 64 && size.Height == 64) {
                return FileListViewIconSize.IconSize64;
            } else if (size.Width == 128 && size.Height == 128) {
                return FileListViewIconSize.IconSize128;
            } else if (size.Width == 256 && size.Height == 256) {
                return FileListViewIconSize.IconSize256;
            } else {
                Program.Abort("サイズ値が異常です。");
                return null;
            }
        }

        //=========================================================================================
        // 機　能：1つ大きなサイズを返す
        // 引　数：[in]size   現在のサイズ
        // 戻り値：大きなサイズ
        //=========================================================================================
        public static FileListViewIconSize GetLarger(FileListViewIconSize size) {
            if (size == IconSize32) {
                return IconSize48;
            } else if (size == IconSize48) {
                return IconSize64;
            } else if (size == IconSize64) {
                return IconSize128;
            } else if (size == IconSize128) {
                return IconSize256;
            } else if (size == IconSize256) {
                return size;
            } else {
                Program.Abort("サイズが未定義です。");
                return null;
            }
        }

        //=========================================================================================
        // 機　能：1つ小さなサイズを返す
        // 引　数：[in]size   現在のサイズ
        // 戻り値：小さなサイズ
        //=========================================================================================
        public static FileListViewIconSize GetSmaller(FileListViewIconSize size) {
            if (size == IconSize32) {
                return size;
            } else if (size == IconSize48) {
                return IconSize32;
            } else if (size == IconSize64) {
                return IconSize48;
            } else if (size == IconSize128) {
                return IconSize64;
            } else if (size == IconSize256) {
                return IconSize128;
            } else {
                Program.Abort("サイズが未定義です。");
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
        // プロパティ：画像サイズ
        //=========================================================================================
        public int ImageSize {
            get {
                return m_imageSize;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンサイズ
        //=========================================================================================
        public IconSize IconSize {
            get {
                return m_iconSize;
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
