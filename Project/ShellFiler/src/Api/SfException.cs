using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Properties;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：アプリケーションとしての例外
    //=========================================================================================
    public class SfException : Exception {
        public static readonly Msg SSHNotFoundAuthSetting = new Msg(Resources.Ex_SSHNotFoundAuthSetting);
        public static readonly Msg SSHCanNotParsePath     = new Msg(Resources.Ex_SSHCanNotParsePath);
        public static readonly Msg WorkDirectoryCreate    = new Msg(Resources.Ex_CreateWorkFolder);
        public static readonly Msg Canceled               = new Msg("");

        // メッセージ
        private Msg m_msg;

        // パラメータ
        private string[] m_parameter;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]message  メッセージ
        // 　　　　[in]param    パラメータ
        // 戻り値：なし
        //=========================================================================================
        public SfException(Msg msg, params string[] param) {
            m_msg = msg;
            m_parameter = param;
        }

        //=========================================================================================
        // プロパティ：メッセージ
        //=========================================================================================
        public override string Message {
            get {
                string msg = string.Format(m_msg.Message, m_parameter);
                return msg;
            }
        }
        
        //=========================================================================================
        // プロパティ：メッセージ種別
        //=========================================================================================
        public Msg MessageType {
            get {
                return m_msg;
            }
        }

        //=========================================================================================
        // クラス：例外に使用するメッセージ
        //=========================================================================================
        public class Msg {
            // メッセージ
            public string Message;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]message  メッセージ
            // 戻り値：なし
            //=========================================================================================
            public Msg(String message) {
                Message = message;
            }
        }
    }
}
