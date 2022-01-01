using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.FileSystem.Shell;
using ShellFiler.UI.Dialog;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // クラス：ファイル/フォルダの２重化のための情報
    //=========================================================================================
    public class DuplicateFileInfo {
        // 対象がディレクトリのときtrue
        private bool m_isDirectory;

        // 新しいファイル名
        private string m_newFileName;

        // 属性のコピーのモード
        private AttributeSetMode m_attributeSetMode;

        //=========================================================================================
        // 機　能：リネーム対象のファイル情報を作成して返す
        // 引　数：[in]isDirectory   対象がディレクトリのときtrue
        // 　　　　[in]newFileName   新しいファイル名
        // 　　　　[in]attrSetMode   属性のコピーのモード
        // 戻り値：リネーム対象のファイル情報
        //=========================================================================================
        public DuplicateFileInfo(bool isDirectory, string newFileName, AttributeSetMode attrSetMode) {
            m_isDirectory = isDirectory;
            m_newFileName = newFileName;
            m_attributeSetMode = attrSetMode;
        }

        //=========================================================================================
        // プロパティ：対象がディレクトリのときtrue
        //=========================================================================================
        public bool IsDirectory {
            get {
                return m_isDirectory;
            }
        }

        //=========================================================================================
        // プロパティ：新しいファイル名
        //=========================================================================================
        public string NewFileName {
            get {
                return m_newFileName;
            }
        }

        //=========================================================================================
        // プロパティ：属性のコピーを行うときtrue
        //=========================================================================================
        public AttributeSetMode AttributeSetMode {
            get {
                return m_attributeSetMode;
            }
        }
    }
}
