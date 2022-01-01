using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：ファイル圧縮を実行するときのパラメータ
    //=========================================================================================
    public class ArchiveParameter {
        // ファイル名
        private string m_fileName;
        
        // 圧縮の実行方法
        private ArchiveExecuteMethod m_executeMethod;
        
        // 圧縮ファイルのフォーマット形式
        private ArchiveType m_archiveType;

        // 圧縮の実行方法がsevenzip.dllの場合の圧縮オプション（sevenzip.dll以外の場合はnull）
        private ArchiveSettingLocal7zOption m_local7zOption;

        // 圧縮の実行方法がリモートシェルの場合の圧縮オプション（リモートシェル以外の場合はnull）
        private ArchiveSettingRemoteShellOption m_remoteShellOption;

        //=========================================================================================
        // 機　能：コンストラクタ（sevenzip.dll用）
        // 引　数：[in]fileName       ファイル名
        // 　　　　[in]archiveType    圧縮ファイルのフォーマット形式
        // 　　　　[in]option         オプション
        // 戻り値：なし
        //=========================================================================================
        public ArchiveParameter(string fileName, ArchiveType archiveType, ArchiveSettingLocal7zOption option) {
            m_fileName = fileName;
            m_executeMethod = ArchiveExecuteMethod.Local7z;
            m_archiveType = archiveType;
            m_local7zOption = option;
            m_remoteShellOption = null;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（sevenzip.dll用）
        // 引　数：[in]fileName       ファイル名
        // 　　　　[in]archiveType    圧縮ファイルのフォーマット形式
        // 　　　　[in]option         オプション
        // 戻り値：なし
        //=========================================================================================
        public ArchiveParameter(string fileName, ArchiveType archiveType, ArchiveSettingRemoteShellOption option) {
            m_fileName = fileName;
            m_executeMethod = ArchiveExecuteMethod.RemoteShell;
            m_archiveType = archiveType;
            m_local7zOption = null;
            m_remoteShellOption = option;
        }

        //=========================================================================================
        // プロパティ：ファイル名
        //=========================================================================================
        public string FileName {
            get {
                return m_fileName;
            }
        }
        
        //=========================================================================================
        // プロパティ：圧縮の実行方法
        //=========================================================================================
        public ArchiveExecuteMethod ExecuteMethod {
            get {
                return m_executeMethod;
            }
        }

        //=========================================================================================
        // プロパティ：圧縮ファイルのフォーマット形式
        //=========================================================================================
        public ArchiveType ArchiveType {
            get {
                return m_archiveType;
            }
        }

        //=========================================================================================
        // プロパティ：圧縮の実行方法がsevenzip.dllの場合の圧縮オプション（sevenzip.dll以外の場合はnull）
        //=========================================================================================
        public ArchiveSettingLocal7zOption Local7zOption {
            get {
                return m_local7zOption;
            }
        }

        //=========================================================================================
        // プロパティ：圧縮の実行方法がリモートシェルの場合の圧縮オプション（リモートシェル以外の場合はnull）
        //=========================================================================================
        public ArchiveSettingRemoteShellOption RemoteShellOption {
            get {
                return m_remoteShellOption;
            }
        }
    }
}
