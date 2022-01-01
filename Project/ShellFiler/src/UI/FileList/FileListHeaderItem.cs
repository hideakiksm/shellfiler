using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Util;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：ファイル一覧のヘッダに出力する項目１件分の情報
    //=========================================================================================
    public class FileListHeaderItem {
        // 表示項目
        private FileListHeaderItemId m_itemId;

        // 項目の表示名
        private string m_displayName;

        // 画面の幅の基準となるサンプル文字
        private string m_widthSample;

        // 可変長フィールドのときtrue
        private bool m_variable;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]itemId       表示の親となるビュー
        // 　　　　[in]displayName  表示名
        // 　　　　[in]widthSample  画面の幅の基準となるサンプル文字
        // 　　　　[in]variable     可変長フィールドのときtrue
        // 戻り値：なし
        //=========================================================================================
        public FileListHeaderItem(FileListHeaderItemId itemId, string displayName, string widthSample, bool variable) {
            m_itemId = itemId;
            m_displayName = displayName;
            m_widthSample = widthSample;
            m_variable = variable;
        }

        //=========================================================================================
        // プロパティ：表示項目
        //=========================================================================================
        public FileListHeaderItemId ItemID {
            get {
                return m_itemId;
            }
        }

        //=========================================================================================
        // プロパティ：項目の表示名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
        }
        
        //=========================================================================================
        // プロパティ：画面の幅の基準となるサンプル文字
        //=========================================================================================
        public string WidthSample {
            get {
                return m_widthSample;
            }
        }

        //=========================================================================================
        // プロパティ：可変長フィールドのときtrue
        //=========================================================================================
        public bool Variable {
            get {
                return m_variable;
            }
        }

        //=========================================================================================
        // 列挙子：項目の種類
        //=========================================================================================
        public enum FileListHeaderItemId {
            FileName,               // ファイル名
            FileSize,               // ファイルサイズ
            ModifiedTime,           // 更新時刻
            Attribute,              // 属性
        };
    }
}
