using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Management;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;
using ShellFiler.Virtual;

namespace ShellFiler.UI.StateList {

    //=========================================================================================
    // クラス：状態一覧パネルのメニュー項目
    //=========================================================================================
    public class StateMenu {
        // 項目名
        private string m_menuName;

        // 項目が有効なときtrue
        private bool m_enabled;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]enabled      項目が有効なときtrue
        // 　　　　[in]menuName     項目名
        // 戻り値：なし
        //=========================================================================================
        public StateMenu(bool enabled, string menuName) {
            m_enabled = enabled;
            m_menuName = menuName;
        }

        //=========================================================================================
        // プロパティ：項目名
        //=========================================================================================
        public string MenuName {
            get {
                return m_menuName;
            }
        }

        //=========================================================================================
        // プロパティ：項目が有効なときtrue
        //=========================================================================================
        public bool Enabled {
            get {
                return m_enabled;
            }
        }
    }
}
