using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.Setting {
    //=========================================================================================
    // クラス：アドレスバーのカスタマイズ項目
    //=========================================================================================
    class AddressBarSettingItem {
        // ドライブ一覧（ディレクトリ名として設定）
        public const string DRIVE_LIST = "<DriveList>";

        // 表示名
        private string m_displayName;

        // ディレクトリ（DRIVE_LISTのときドライブ一覧）
        private string m_directory;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]displayName  表示名
        // 　　　　[in]directory    ディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public AddressBarSettingItem(string displayName, string directory) {
            m_displayName = displayName;
            m_directory = directory;
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
        // プロパティ：ディレクトリ（DRIVE_LISTのときドライブ一覧）
        //=========================================================================================
        public string Directory {
            get {
                return m_directory;
            }
        }
    }
}
