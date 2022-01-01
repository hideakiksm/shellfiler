using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Virtual {

    //=========================================================================================
    // クラス：仮想ディレクトリの表示名
    //=========================================================================================
    public class TemporarySpaceDisplayName {

        // 表示名のテキスト
        private string m_displayName;
        
        // アイコンのイメージインデックス
        private IconImageListID m_iconImageIndex;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]displayName 表示名のテキスト
        // 　　　　[in]icon        アイコンのイメージインデックス
        // 戻り値：なし
        //=========================================================================================
        public TemporarySpaceDisplayName(string displayName, IconImageListID iconImageIndex) {
            m_displayName = displayName;
            m_iconImageIndex = iconImageIndex;
        }

        //=========================================================================================
        // プロパティ：表示名のテキスト
        //=========================================================================================
        public string Text {
            get {
                return m_displayName;
            }
        }
        
        //=========================================================================================
        // プロパティ：アイコンのイメージインデックス
        //=========================================================================================
        public IconImageListID IconImageIndex {
            get {
                return m_iconImageIndex;
            }
        }
    }
}
