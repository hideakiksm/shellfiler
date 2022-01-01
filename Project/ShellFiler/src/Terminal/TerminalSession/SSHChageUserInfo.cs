using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using ShellFiler.Api;

namespace ShellFiler.Terminal.TerminalSession {
    
    //=========================================================================================
    // クラス：SSHでユーザー変更をするときの情報
    //=========================================================================================
    public class SSHChangeUserInfo {
        // ユーザー変更のモード
        private ChangeUserMode m_changeMode;

        // 現在実行中のユーザーがスーパーユーザーのときtrue
        private bool m_isSuperUserNow;

        // ユーザー名（su以外はnull）
        private string m_userName;

        // パスワード（suは""）
        private string m_password;

        // ログインシェルを使用するときtrue
        private bool m_useLoginShell;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]changeMode     ユーザー変更のモード
        // 　　　　[in]isSuperUserNow 現在実行中のユーザーがスーパーユーザーのときtrue
        // 　　　　[in]userName       ユーザー名（su以外はnull）
        // 　　　　[in]password       パスワード（suは""）
        // 　　　　[in]useLoginShell  ログインシェルを使用するときtrue
        // 戻り値：実行結果
        //=========================================================================================
        public SSHChangeUserInfo(ChangeUserMode changeMode, bool isSuperUserNow, string userName, string password, bool useLoginShell) {
            m_changeMode = changeMode;
            m_isSuperUserNow = isSuperUserNow;
            m_userName = userName;
            m_password = password;
            m_useLoginShell = useLoginShell;
        }

        //=========================================================================================
        // プロパティ：現在実行中のユーザーがスーパーユーザーのときtrue
        //=========================================================================================
        public bool IsSuperUserNow {
            get {
                return m_isSuperUserNow;
            }
        }

        //=========================================================================================
        // プロパティ：ユーザー変更のモード
        //=========================================================================================
        public ChangeUserMode ChangeMode {
            get {
                return m_changeMode;
            }
        }

        //=========================================================================================
        // プロパティ：ユーザー名（su以外はnull）
        //=========================================================================================
        public string UserName {
            get {
                return m_userName;
            }
        }

        //=========================================================================================
        // プロパティ：パスワード（suは""）
        //=========================================================================================
        public string Password {
            get {
                return m_password;
            }
        }

        //=========================================================================================
        // プロパティ：ログインシェルを使用するときtrue
        //=========================================================================================
        public bool UseLoginShell {
            get {
                return m_useLoginShell;
            }
        }
        
        //=========================================================================================
        // 列挙子：ユーザー変更のモード
        //=========================================================================================
        public enum ChangeUserMode {
            Su,             // ユーザーを切り替える
            Exit,           // 元に戻る
        }
    }
}
