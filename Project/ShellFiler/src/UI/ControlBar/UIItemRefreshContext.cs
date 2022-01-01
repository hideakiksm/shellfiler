using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Api;
using ShellFiler.Command;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：メニュー項目とツールバーの更新情報
    //=========================================================================================
    public class UIItemRefreshContext {
        // マークされている対象が1件でもあればtrue
        private bool m_marked;

        // 直前のパスヒストリ（左ウィンドウ）が有効のときtrue
        private bool m_pathHistPrevLeft;

        // 直前のパスヒストリ（右ウィンドウ）が有効のときtrue
        private bool m_pathHistPrevRight;

        // 直前のパスヒストリ（対象ウィンドウ）が有効のときtrue
        private bool m_pathHistPrevTarget;

        // 直後のパスヒストリ（左ウィンドウ）が有効のときtrue
        private bool m_pathHistNextLeft;

        // 直後のパスヒストリ（右ウィンドウ）が有効のときtrue
        private bool m_pathHistNextRight;

        // 直後のパスヒストリ（対象ウィンドウ）が有効のときtrue
        private bool m_pathHistNextTarget;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public UIItemRefreshContext() {
            // マーク状態を確認
            bool isCursorLeft = Program.Document.CurrentTabPage.IsCursorLeft;
            UIFileList fileList = Program.Document.CurrentTabPage.GetFileList(isCursorLeft);
            m_marked = ((fileList.MarkedFileCount + fileList.MarkedDirectoryCount) > 0);

            // パスヒストリを確認
            PathHistory leftHistory = Program.Document.CurrentTabPage.LeftFileList.PathHistory;
            PathHistory rightHistory = Program.Document.CurrentTabPage.RightFileList.PathHistory;
            m_pathHistPrevLeft = leftHistory.EnablePrev;
            m_pathHistPrevRight = rightHistory.EnablePrev;
            m_pathHistNextLeft = leftHistory.EnableNext;
            m_pathHistNextRight = rightHistory.EnableNext;
            if (Program.Document.CurrentTabPage.IsCursorLeft) {
                m_pathHistPrevTarget = m_pathHistPrevLeft;
                m_pathHistNextTarget = m_pathHistNextLeft;
            } else {
                m_pathHistPrevTarget = m_pathHistPrevRight;
                m_pathHistNextTarget = m_pathHistNextRight;
            }
        }

        //=========================================================================================
        // 機　能：直前のパスヒストリに移動ボタンを有効にするかどうかを返す
        // 引　数：[in]type  調べるコマンドの種類
        // 戻り値：有効にするときtrue
        //=========================================================================================
        public bool GetPathHistoryPrev(UICommandSender type) {
            if (type == UICommandSender.AddressBarLeft) {
                return m_pathHistPrevLeft;
            } else if (type == UICommandSender.AddressBarRight) {
                return m_pathHistPrevRight;
            } else {
                return m_pathHistPrevTarget;
            }
        }
        
        //=========================================================================================
        // 機　能：直後のパスヒストリに移動ボタンを有効にするかどうかを返す
        // 引　数：[in]type  調べるコマンドの種類
        // 戻り値：有効にするときtrue
        //=========================================================================================
        public bool GetPathHistoryNext(UICommandSender type) {
            if (type == UICommandSender.AddressBarLeft) {
                return m_pathHistNextLeft;
            } else if (type == UICommandSender.AddressBarRight) {
                return m_pathHistNextRight;
            } else {
                return m_pathHistNextTarget;
            }
        }

        //=========================================================================================
        // プロパティ：マークされている対象が1件でもあればtrue
        //=========================================================================================
        public bool IsMarked {
            get {
                return m_marked;
            }
        }
    }
}
