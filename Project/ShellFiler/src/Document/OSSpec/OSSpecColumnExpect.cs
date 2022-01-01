using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.OSSpec {

    //=========================================================================================
    // クラス：コマンド実行時の期待値
    //=========================================================================================
    public class OSSpecColumnExpect {
        // トークンの種類
        private OSSpecTokenType m_tokenType;

        // トークンに応じた期待値定義のパラメータ（TokenType=Specify以外はnull）
        private object[] m_expectParam;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]tokenType  トークンの種類
        // 　　　　[in]param      トークンに応じた期待値定義のパラメータ
        // 戻り値：なし
        // メ　モ：Specify : [0]期待する文字列
        // 　　　　それ以外はなし
        //=========================================================================================
        public OSSpecColumnExpect(OSSpecTokenType tokenType, params object[] param) {
            m_tokenType = tokenType;
            if (tokenType == OSSpecTokenType.Specify) {
                m_expectParam = param;
            } else {
                m_expectParam = null;
            }
        }

        //=========================================================================================
        // プロパティ：トークンの種類
        //=========================================================================================
        public OSSpecTokenType TokenType {
            get {
                return m_tokenType;
            }
        }

        //=========================================================================================
        // プロパティ：トークンに応じた期待値定義のパラメータ（TokenType=Specify以外はnull）
        //=========================================================================================
        public object[] ExpectParam {
            get {
                return m_expectParam;
            }
        }
    }
}
