using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：シンボリックリンクのリンク先情報
    //=========================================================================================
    public class SymbolicLinkTarget {
        // リンクの参照先パス名
        private string m_linkTarget;

        // リンク先が存在するときtrue
        private bool m_existTarget;

        // リンク先がディレクトリのときtrue
        private bool m_isDirectory;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]linkTarget  リンクの参照先
        // 　　　　[in]exist       リンク先が存在するときtrueを返す変数
        // 　　　　[in]isDirectory リンク先がディレクトリのときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        public SymbolicLinkTarget(string linkTarget, bool existTarget, bool isDirectory) {
            m_linkTarget = linkTarget;
            m_existTarget = existTarget;
            m_isDirectory = isDirectory;
        }

        //=========================================================================================
        // プロパティ：リンクの参照先パス名
        //=========================================================================================
        public string LinkTarget {
            get {
                return m_linkTarget;
            }
        }

        //=========================================================================================
        // プロパティ：リンク先が存在するときtrue
        //=========================================================================================
        public bool ExistTarget {
            get {
                return m_existTarget;
            }
        }

        //=========================================================================================
        // プロパティ：リンク先がディレクトリのときtrue
        //=========================================================================================
        public bool IsDirectory {
            get {
                return m_isDirectory;
            }
        }
    }
}
