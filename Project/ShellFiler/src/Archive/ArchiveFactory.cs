using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：圧縮／展開のファクトリ
    //=========================================================================================
    public class ArchiveFactory {
        // 初期化が完了したときtrue
        private bool m_initialized = false;

        // 7z.dllのフルパス名
        private string m_sevenZipDllPath = null;

        // 7zをサポートしているときtrue
        private bool m_isSupportSevenZip = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ArchiveFactory() {
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
            lock (this) {
                if (m_initialized) {
                    return;
                }
                m_initialized = true;

                // 7zipのインストール先を探す
                m_isSupportSevenZip = false;
                RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\7-Zip", false);
                if (regkey != null) {
                    // レジストリがある場合は従う
                    string pathValue = (string)regkey.GetValue("Path");
                    if (pathValue == null) {
                        return;
                    }
                    m_sevenZipDllPath = Path.Combine(pathValue, "7z.dll");
                } else {
                    // レジストリがない場合はShellFilerと同一フォルダ
                    m_sevenZipDllPath = Path.Combine(Program.InstallPath, "7z.dll");
                }
                if (!File.Exists(m_sevenZipDllPath)) {
                    return;
                }
                IntPtr hModule = Win32API.Win32LoadLibrary(m_sevenZipDllPath);
                if (hModule == null) {
                    return;
                }
                m_isSupportSevenZip = true;
                Win32API.Win32FreeLibrary(hModule);
            }
        }

        //=========================================================================================
        // 機　能：圧縮をサポートしているかどうか調べる
        // 引　数：なし
        // 戻り値：サポートしているときtrue
        //=========================================================================================
        public bool IsSupportArchive() {
            Initialize();
            return m_isSupportSevenZip;
        }

        //=========================================================================================
        // 機　能：展開をサポートしているかどうか調べる
        // 引　数：なし
        // 戻り値：サポートしているときtrue
        //=========================================================================================
        public bool IsSupportExtract() {
            Initialize();
            return m_isSupportSevenZip;
        }

        //=========================================================================================
        // 機　能：ファイル一覧をサポートしているかどうか調べる
        // 引　数：なし
        // 戻り値：サポートしているときtrue
        //=========================================================================================
        public bool IsSupportFileList() {
            Initialize();
            return m_isSupportSevenZip;
        }
   
        //=========================================================================================
        // 機　能：展開用インターフェースを取得する
        // 引　数：[in]arcName         アーカイブファイル名
        // 　　　　[in]passwordSource  パスワードの提供クラス
        // 戻り値：展開用インターフェース
        //=========================================================================================
        public IArchiveExtract CreateExtract(string arcName, ArchivePasswordSource passwordSource) {
            return new SevenZipExtract(m_sevenZipDllPath, arcName, passwordSource);
        }

        //=========================================================================================
        // 機　能：作成用インターフェースを取得する
        // 引　数：[in]archiveSetting  アーカイブ設定
        // 　　　　[in]arcFileName     作成するアーカイブファイル名
        // 戻り値：作成用インターフェース
        //=========================================================================================
        public IArchiveCreate CreateArchive(ArchiveParameter archiveSetting, string arcFileName) {
            return new SevenZipCreate(m_sevenZipDllPath, archiveSetting, arcFileName);
        }
   
        //=========================================================================================
        // 機　能：仮想フォルダのファイル一覧取得用インターフェースを取得する
        // 引　数：[in]arcName         アーカイブファイル名
        // 　　　　[in]temporaryPath   作業フォルダ
        // 　　　　[in]passwordSource  パスワードの提供クラス
        // 戻り値：ファイル一覧取得用インターフェース
        //=========================================================================================
        public IArchiveVirtualFileList CreateVirtualFileList(string arcName, string temporaryPath, ArchivePasswordSource passwordSource) {
            return new SevenZipVirtualFileList(m_sevenZipDllPath, arcName, temporaryPath, passwordSource);
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dllPath         DLLのパス名
        // 　　　　[in]arcName         アーカイブファイル名
        // 　　　　[in]passwordSource  パスワードの提供クラス
        // 　　　　[in]parentDialog    パスワード入力ダイアログの親となるダイアログ
        // 戻り値：なし
        //=========================================================================================
        public IArchiveVirtualExtract CreateVirtualExtract(string arcName, ArchivePasswordSource passwordSource, Form parentDialog) {
            return new SevenZipVirtualExtract(m_sevenZipDllPath, arcName, passwordSource, parentDialog);
        }
    }
}
