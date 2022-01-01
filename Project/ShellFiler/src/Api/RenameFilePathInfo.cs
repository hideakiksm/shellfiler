using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Properties;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：ファイルのリネームの情報
    //=========================================================================================
    public class RenameFilePathInfo {
        // 元のファイル名またはファイルパス
        private string m_originalName;

        // 新しいファイル名またはファイルパス
        private string m_newName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]originalName   元のファイル名またはファイルパス
        // 　　　　[in]newName        新しいファイル名またはファイルパス
        // 戻り値：なし
        //=========================================================================================
        public RenameFilePathInfo(string originalName, string newName) {
            m_originalName = originalName;
            m_newName = newName;
        }

        //=========================================================================================
        // プロパティ：元のファイル名またはファイルパス
        //=========================================================================================
        public string OriginalName {
            get {
                return m_originalName;
            }
        }

        //=========================================================================================
        // プロパティ：新しいファイル名またはファイルパス
        //=========================================================================================
        public string NewName {
            get {
                return m_newName;
            }
        }
    }
}
