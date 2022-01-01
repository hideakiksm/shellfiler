using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Properties;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：ライセンス情報
    //=========================================================================================
    public class UserLicenseInfo {
        // ユーザー名
        private string m_userName;

        // 有効期限（YYYYMM）
        private int m_expireDate;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]userName    ユーザー名
        // 　　　　[in]expireDate  有効期限（YYYYMM）
        // 戻り値：なし
        //=========================================================================================
        public UserLicenseInfo(string userName, int expireDate) {
            m_userName = userName;
            m_expireDate = expireDate;
        }

        //=========================================================================================
        // プロパティ：ユーザー名
        //=========================================================================================
        public string UserName {
            get {
                return  m_userName;
            }
        }

        //=========================================================================================
        // プロパティ：有効期限（YYYYMM）
        //=========================================================================================
        public int ExpireDate {
            get {
                return  m_expireDate;
            }
        }
    }
}
