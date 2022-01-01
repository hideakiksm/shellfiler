using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Virtual {
    //=========================================================================================
    // クラス：仮想フォルダの作業領域の情報
    //=========================================================================================
    public class VirtualFolderTemporaryDirectory {
        // テンポラリのID
        private int m_tempId;
        
        // 仮想フォルダの作業ディレクトリのルート
        private string m_tempDirRoot;

        // 仮想フォルダのアーカイブ格納領域
        private string m_tempDirArchive;

        // 仮想フォルダの使用元
        private List<VirtualFolderClient> m_folderClientList = new List<VirtualFolderClient>();
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]tempId       テンポラリのID
        // 　　　　[in]tempDirRoot  仮想フォルダの作業ディレクトリのルート
        // 戻り値：なし
        //=========================================================================================
        public VirtualFolderTemporaryDirectory(int tempId, string tempDirRoot) {
            m_tempId = tempId;
            m_tempDirRoot = tempDirRoot;
            m_tempDirArchive = tempDirRoot +"\\Arc";
            try {
                Directory.CreateDirectory(m_tempDirArchive);
            } catch (Exception e) {
                throw new SfException(SfException.WorkDirectoryCreate, m_tempDirRoot, e.Message);
            }
        }

        //=========================================================================================
        // 機　能：テンポラリ空間を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            try {
                FileUtils.DeleteDirectory(m_tempDirRoot);
            } catch (Exception) {
            }
        }

        //=========================================================================================
        // 機　能：仮想フォルダ情報の利用を開始する
        // 引　数：[in]virtualInfo   利用する仮想フォルダ
        // 　　　　[in]usingType     用途
        // 戻り値：コピーに成功したときtrue
        // メ　モ：virtualInfoのVirtualFolderTemporaryDirectoryはthisと同じインスタンスを指していること
        // 　　　　登録済みのvirtualInfoを再登録することも可能
        //=========================================================================================
        public void BeginUsing(VirtualFolderInfo virtualInfo, UsingType usingType) {
            for (int i = 0; i < m_folderClientList.Count; i++) {
                if (m_folderClientList[i].VirtualFolderInfo == virtualInfo) {
                    m_folderClientList[i].ClientType = usingType;
                    return;
                }
            }
            m_folderClientList.Add(new VirtualFolderClient(virtualInfo, usingType));
        }

        //=========================================================================================
        // 機　能：仮想フォルダ情報の利用を終了する
        // 引　数：[in]virtualInfo   利用を終了する仮想フォルダ
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public void EndUsing(VirtualFolderInfo virtualInfo) {
            // リストを整理
            List<VirtualFolderClient> prevList = m_folderClientList;
            List<VirtualFolderClient> newList = new List<VirtualFolderClient>();
            for (int i = 0; i < m_folderClientList.Count; i++) {
                if (m_folderClientList[i].VirtualFolderInfo != virtualInfo) {
                    newList.Add(m_folderClientList[i]);
                }
            }
            m_folderClientList = newList;

            if (m_folderClientList.Count == 0) {
                // すべての利用を終了：仮想フォルダのルートから削除
                FileUtils.DeleteDirectory(m_tempDirRoot);
                Program.Document.TemporaryManager.VirtualManager.DeleteVirtualFolder(this);
            } else {
                // 前回との差分を削除
                HashSet<string> prevArchiveList = GetManagedTemporaryFile(prevList);
                HashSet<string> nextArchiveList = GetManagedTemporaryFile(newList);
                foreach (string prevUsed in prevArchiveList) {
                    if (!nextArchiveList.Contains(prevUsed)) {
                        // 不要なファイルまたはディレクトリを削除
                        if (prevUsed.EndsWith("\\")) {
                            try {
                                FileUtils.DeleteDirectory(prevUsed);
                            } catch (Exception) {
                            }
                        } else {
                            try {
                                File.Delete(prevUsed);
                            } catch (Exception) {
                            }
                        }
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：ライフサイクルが管理状態にある一時ファイルのリストを返す
        // 引　数：[in]clientList  作成する仮想フォルダの一覧
        // 戻り値：一時ファイルのリスト（最後が「\」のものはディレクトリ、それ以外はファイル）
        //=========================================================================================
        private HashSet<string> GetManagedTemporaryFile(List<VirtualFolderClient> clientList) {
            HashSet<string> result = new HashSet<string>();
            foreach (VirtualFolderClient client in clientList) {
                List<string> managedTemporaryList = client.VirtualFolderInfo.GetManagedTemporaryList();
                foreach (string tempFile in managedTemporaryList) {
                    result.Add(tempFile);
                }

                foreach (VirtualFolderArchiveInfo arcInfo in client.VirtualFolderInfo.VirtualFolderItemList) {
                    managedTemporaryList = arcInfo.GetManagedTemporaryList();
                    foreach (string tempFile in managedTemporaryList) {
                        result.Add(tempFile);
                    }
                }
            }
            return result;
        }

        //=========================================================================================
        // 機　能：同一の実行IDを持った仮想フォルダ情報に実行用の仮想フォルダ領域を設定する
        // 引　数：[in]executeId       実行用のID
        // 　　　　[in]tempDirExecute  実行用の仮想フォルダ領域（最後は「\」）
        // 戻り値：なし
        //=========================================================================================
        public void SetVirtualExecuteFolder(int executeId, string tempDirExecute) {
            for (int i = 0; i < m_folderClientList.Count; i++) {
                if (m_folderClientList[i].VirtualFolderInfo.VirtualExecuteId == executeId) {
                    m_folderClientList[i].VirtualFolderInfo.AttachTempDirectoryForExecute(tempDirExecute);
                }
            }
        }

        //=========================================================================================
        // プロパティ：テンポラリのID
        //=========================================================================================
        public int TempId {
            get {
                return m_tempId;
            }
        }
        
        //=========================================================================================
        // プロパティ：仮想フォルダの作業ディレクトリのルート
        //=========================================================================================
        public string TemporaryDirectoryRoot {
            get {
                return m_tempDirRoot;
            }
        }
        
        //=========================================================================================
        // プロパティ：仮想フォルダの作業ディレクトリのアーカイブ格納領域
        //=========================================================================================
        public string TemporaryDirectoryArchive {
            get {
                return m_tempDirArchive;
            }
        }
        
        //=========================================================================================
        // プロパティ：仮想フォルダの使用元
        //=========================================================================================
        public class VirtualFolderClient {
            // 使用しているファイルやフォルダの構成
            public VirtualFolderInfo VirtualFolderInfo;

            // 使用元の種類
            public UsingType ClientType;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]virtualFolderInfo  使用しているファイルやフォルダの構成
            // 　　　　[in]clientType         使用元の情報
            // 戻り値：なし
            //=========================================================================================
            public VirtualFolderClient(VirtualFolderInfo virtualFolderInfo, UsingType clientType) {
                VirtualFolderInfo = virtualFolderInfo;
                ClientType = clientType;
            }
        }

        //=========================================================================================
        // プロパティ：仮想フォルダ情報の用途
        //=========================================================================================
        public enum UsingType {
            FileListUsing,                  // ファイル一覧で使用中
            FileListLoading,                // ファイル一覧用に読み込み中
            FileOperationRunning,           // ファイル処理を実行中
        }
    }
}
