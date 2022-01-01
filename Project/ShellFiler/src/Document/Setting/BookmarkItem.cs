using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ブックマークのカスタマイズ項目
    //=========================================================================================
    public class BookmarkItem : ICloneable {
        // ショートカットとして利用できる文字
        public static readonly string[] SHORTCUT_ITEMS = {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q",
            "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "-", "\\",
        };

        // ショートカットに使用する文字（大文字）
        private char m_shortCut;

        // 表示名
        private string m_displayName;
        
        // ディレクトリ
        private string m_directory;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]shortCut     ショートカットに使用する文字（大文字）
        // 　　　　[in]displayName  表示名
        // 　　　　[in]directory    ディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public BookmarkItem(char shortCut, string displayName, string directory) {
            m_shortCut = shortCut;
            m_displayName = displayName;
            m_directory = directory;
        }
        
        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            return MemberwiseClone();
        }

        //=========================================================================================
        // プロパティ：ショートカットに使用する文字（大文字）
        //=========================================================================================
        public char ShortCut {
            get {
                return m_shortCut;
            }
            set {
                m_shortCut = value;
            }
        }

        //=========================================================================================
        // プロパティ：表示名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
            set {
                m_displayName = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ディレクトリ
        //=========================================================================================
        public string Directory {
            get {
                return m_directory;
            }
            set {
                m_directory = value;
            }
        }
    }
}
