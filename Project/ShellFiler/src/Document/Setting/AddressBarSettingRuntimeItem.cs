using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：アドレスバーのカスタマイズ項目（実行時用）
    //=========================================================================================
    class AddressBarSettingRuntimeItem {
        // 表示名
        private string m_displayName;

        // ディレクトリ
        private string m_directory;

        // アイコン（null:デフォルトのフォルダアイコン）
        private Bitmap m_icon;

        // 深さ（0～）
        private int m_depth;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]displayName  表示名
        // 　　　　[in]directory    ディレクトリ
        // 　　　　[in]icon         アイコン（null:デフォルトのフォルダアイコン）
        // 　　　　[in]depth        深さ（0～）
        // 戻り値：なし
        //=========================================================================================
        public AddressBarSettingRuntimeItem(string displayName, string directory, Bitmap icon, int depth) {
            m_displayName = displayName;
            m_directory = directory;
            m_icon = icon;
            m_depth = depth;
        }

        //=========================================================================================
        // プロパティ：表示名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリ
        //=========================================================================================
        public string Directory {
            get {
                return m_directory;
            }
        }

        //=========================================================================================
        // プロパティ：アイコン（null:デフォルトのフォルダアイコン）
        //=========================================================================================
        public Bitmap Icon {
            get {
                return m_icon;
            }
        }

        //=========================================================================================
        // プロパティ：深さ（0～）
        //=========================================================================================
        public int Depth {
            get {
                return m_depth;
            }
        }
    }
}
