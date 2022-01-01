using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Nomad.Archive.SevenZip;
using Microsoft.COM;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zのパスワードテストを行う実装
    //=========================================================================================
    public class SevenZipPasswordTestImpl {
        // 現在のアーカイブに使用するパスワード文字列（null:使用しない）
        private string m_passwordString = null;

        // 現在のパスワードに対応する表示名（パスワードが自動入力でないときはnull）
        private string m_passwordDisplayName = null;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SevenZipPasswordTestImpl() {
        }

        //=========================================================================================
        // 機　能：アーカイブが暗号化されているかどうかを返す
        // 引　数：[in]archive    アーカイブインタフェース
        // 戻り値：暗号化されているときtrue
        //=========================================================================================
        public bool IsEncrypted(IInArchive archive) {
            int fileCount = (int)(archive.GetNumberOfItems());
            for (int i = 0; i < fileCount; i++) {
                PropVariant varIsFolder = new PropVariant();
                archive.GetProperty((uint)i, ItemPropId.kpidIsFolder, ref varIsFolder);
                if (varIsFolder.GetObject() != null && ((bool)varIsFolder.GetObject()) == false) {
                    // ファイルの場合だけ暗号化チェック
                    PropVariant varIsEncrypted = new PropVariant();
                    archive.GetProperty((uint)i, ItemPropId.kpidEncrypted, ref varIsEncrypted);
                    if (varIsEncrypted.GetObject() == null) {
                        return false;
                    }
                    bool isEncrypted = (bool)varIsEncrypted.GetObject();
                    varIsEncrypted.Clear();
                    return isEncrypted;
                }
                varIsFolder.Clear();
            }
            return false;
        }

        //=========================================================================================
        // 機　能：パスワード付きで展開テストを行う
        // 引　数：[in]archive            アーカイブインタフェース
        // 　　　　[in,out]archiveStream  アーカイブのストリーム（開き直した場合は再設定）
        // 　　　　[in]archiveFileName    アーカイブのファイル名
        // 　　　　[in]passwordSource     パスワードの情報源
        // 　　　　[in]parentDialog       パスワード入力ダイアログの親となるダイアログ
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus OpenWithPasswordTest(IInArchive archive, ref InStreamWrapper archiveStream, string archiveFileName, ArchivePasswordSource passwordSource, Form parentDialog) {
            while (true) {
                // パスワードを取得
                bool isCancel, isRetryOver;
                passwordSource.GetNextPassword(parentDialog, out m_passwordString, out isCancel, out isRetryOver);
                if (isCancel) {
                    return FileOperationStatus.Canceled;
                }
                if (isRetryOver) {
                    return FileOperationStatus.ArcBadPassword;
                }
                m_passwordDisplayName = passwordSource.UsedPasswordDisplayName;

                // アーカイブを開く
                archiveStream = new InStreamWrapper(File.OpenRead(archiveFileName));
                SevenZipArchiveOpenCallback openCallback = SevenZipArchiveOpenCallback.NewInstance(m_passwordString);
                ulong checkPos = 128 * 1024;
                if (archive.Open(archiveStream, ref checkPos, openCallback) != 0) {
                    return FileOperationStatus.ErrorOpen;
                }

                // ファイルを最低1個開く
                int itemCount = (int)(archive.GetNumberOfItems());
                bool allFolder = true;
                for (int i = 0; i < itemCount; i++) {
                    SevenZipContentsPasswordTest testContents = new SevenZipContentsPasswordTest(archive, i, m_passwordString);
                    if (testContents.IsFolder) {
                        continue;
                    }
                    archive.Extract(new uint[] { (uint)i }, 1, 0, testContents);
                    if (testContents.IsSuccess) {               // パスワード一致
                        return FileOperationStatus.Success;
                    } else {                                    // パスワード不一致による失敗
                        allFolder = false;
                        break;
                    }
                }
                
                // フォルダだけのアーカイブなら成功
                if (allFolder) {
                    m_passwordString = null;
                    m_passwordDisplayName = null;
                    return FileOperationStatus.Success;
                }
            }
        }

        //=========================================================================================
        // プロパティ：現在のアーカイブに使用するパスワード文字列（null:使用しない）
        //=========================================================================================
        public string PasswordString {
            get {
                return m_passwordString;
            }
        }

        //=========================================================================================
        // プロパティ：現在のパスワードに対応する表示名（パスワードが自動入力でないときはnull）
        //=========================================================================================
        public string PasswordDisplayName {
            get {
                return m_passwordDisplayName;
            }
        }
    }
}
