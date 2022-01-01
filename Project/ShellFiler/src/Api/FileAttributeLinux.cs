using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.OSSpec;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {

    //=========================================================================================
    // クラス：Linuxのファイル属性（FileAttributeとパーミッションを同時に扱うためのクラス）
    //=========================================================================================
    public class FileAttributeLinux {
        // ファイル属性
        private FileAttribute m_fileAttribute;

        // Linuxのパーミッション（8進数値3桁）
        private int m_permissions;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileAttribute  ファイル属性
        // 　　　　[in]permissions    Linuxのパーミッション（8進数値3桁）
        // 戻り値：なし
        //=========================================================================================
        public FileAttributeLinux(FileAttribute fileAttribute, int permissions) {
            m_fileAttribute = fileAttribute;
            m_permissions = permissions;
        }

        //=========================================================================================
        // プロパティ：ファイル属性
        //=========================================================================================
        public FileAttribute FileAttribute {
            get {
                return m_fileAttribute;
            }
        }

        //=========================================================================================
        // プロパティ：Linuxのパーミッション
        //=========================================================================================
        public int Permissions {
            get {
                return m_permissions;
            }
        }
    }
}
