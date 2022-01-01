using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：SSHプロトコルの種類
    //=========================================================================================
    public class SSHProtocolType {
        public static readonly SSHProtocolType SSHShell = new SSHProtocolType("ssh");    // SSH
        public static readonly SSHProtocolType SFTP = new SSHProtocolType("sftp");       // SFTP
        public static readonly SSHProtocolType None = new SSHProtocolType(null);         // エラー相当
        public static readonly SSHProtocolType All = new SSHProtocolType(null);          // SSHとSFTPの総称

        // フォルダ名の先頭に付けるプロトコル名
        private string m_folderProtocol;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]folder  フォルダ名の先頭に付けるプロトコル名
        // 戻り値：なし
        //=========================================================================================
        public SSHProtocolType(string folder) {
            m_folderProtocol = folder;
        }

        //=========================================================================================
        // 機　能：シリアライズされたデータからオブジェクトを復元する
        // 引　数：[in]serialized  シリアライズされたデータ
        // 戻り値：復元したショートカットの種類
        //=========================================================================================
        public static SSHProtocolType FromSerializedData(string serialized) {
            if (serialized == "SSHShell") {
                return SSHShell;
            } else if (serialized == "SFTP") {
                return SFTP;
            } else {
                return SFTP;
            }
        }

        //=========================================================================================
        // 機　能：オブジェクトからシリアライズされたデータを作成する
        // 引　数：[in]obj   ショートカットの種類
        // 戻り値：シリアライズされたデータ
        //=========================================================================================
        public static string ToSerializedData(SSHProtocolType obj) {
            if (obj == SSHShell) {
                return "SSHShell";
            } else if (obj == SFTP) {
                return "SFTP";
            } else {
                return "SFTP";
            }
        }

        //=========================================================================================
        // プロパティ：フォルダ名の先頭に付けるプロトコル名
        //=========================================================================================
        public string FolderProtocol {
            get {
                return m_folderProtocol;
            }
        }
    }
}
