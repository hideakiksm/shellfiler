using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using Nomad.Archive.SevenZip;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zでアーカイブを展開するときのコールバック
    //=========================================================================================
    public class SevenZipArchiveOpenCallback : IArchiveOpenCallback {

        //=========================================================================================
        // 機　能：インスタンスを生成する
        // 引　数：[in]password  パスワード（使用しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public static SevenZipArchiveOpenCallback NewInstance(string password) {
            if (password == null) {
                return new SevenZipArchiveOpenCallback();
            } else {
                return new SevenZipArchiveOpenCallback.WithPassword(password);
            }
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private SevenZipArchiveOpenCallback() {
        }

        //=========================================================================================
        // 機　能：合計サイズを設定する
        // 引　数：[in]files
        // 　　　　[in]bytes  合計サイズ
        // 戻り値：なし
        //=========================================================================================
        public void SetTotal(IntPtr files, IntPtr bytes) {
        }

        //=========================================================================================
        // 機　能：処理済みサイズを設定する
        // 引　数：[in]files
        // 　　　　[in]bytes  処理済みサイズ
        // 戻り値：なし
        //=========================================================================================
        public void SetCompleted(IntPtr files, IntPtr bytes) {
        }

        //=========================================================================================
        // クラス：パスワード付きの実装
        //=========================================================================================
        private class WithPassword : SevenZipArchiveOpenCallback, ICryptoGetTextPassword {
            // パスワード（使用しないときnull）
            private string m_password;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]password  パスワード（使用しないときnull）
            // 戻り値：なし
            //=========================================================================================
            public WithPassword(string password) {
                m_password = password;
            }

            //=========================================================================================
            // 機　能：パスワードを返す
            // 引　数：[in]password   パスワードを返す変数
            // 戻り値：ステータスコード
            //=========================================================================================
            public int CryptoGetTextPassword(out string password) {
                password = m_password;
                return 0;
            }
        }
    }
}
