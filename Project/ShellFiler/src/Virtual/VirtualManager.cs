using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Util;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Virtual {

    //=========================================================================================
    // クラス：仮想ディレクトリの管理クラス
    //=========================================================================================
    public class VirtualManager {
        // 親クラス
        private TemporaryManager m_parent;

        // 仮想フォルダの払い出しに使用されるID
        // 仮想フォルダ作業領域のTempXXのXX部分に使用される数値
        private int m_nextVirtualld = 0;

        // 仮想フォルダの実行領域の払い出しに使用されるID
        // 仮想フォルダ作業領域のExecXXのXX部分に使用される数値、ファイル一覧の更新ごとに+1
        private int m_nextVirtualExecuteld = 0;

        // 作成済みの仮想ディレクトリの一覧
        private List<VirtualFolderTemporaryDirectory> m_virtualFolderList = new List<VirtualFolderTemporaryDirectory>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent   親クラス
        // 戻り値：なし
        //=========================================================================================
        public VirtualManager(TemporaryManager parent) {
            m_parent = parent;
        }

        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            foreach (VirtualFolderTemporaryDirectory tempInfo in m_virtualFolderList) {
                tempInfo.Dispose();
            }
            m_virtualFolderList.Clear();
        }

        //=========================================================================================
        // 機　能：次の仮想ディレクトリ実行IDを返す
        // 引　数：なし
        // 戻り値：仮想フォルダ実行ID
        //=========================================================================================
        public int GetNextVirtualExecuteId() {
            int id = Interlocked.Add(ref m_nextVirtualExecuteld, 1);
            return id;
        }

        //=========================================================================================
        // 機　能：仮想フォルダ用の作業ディレクトリを作成する
        // 引　数：なし
        // 戻り値：仮想フォルダの作業パス情報
        //=========================================================================================
        public VirtualFolderTemporaryDirectory CreateVirtualFolder() {
            lock (this) {
                m_nextVirtualld = Interlocked.Add(ref m_nextVirtualld, 1);
                string path = m_parent.AllVirtualFolderRoot + "Temp" + m_nextVirtualld;
                VirtualFolderTemporaryDirectory tempInfo = new VirtualFolderTemporaryDirectory(m_nextVirtualld, path);
                m_virtualFolderList.Add(tempInfo);
                return tempInfo;
            }
        }

        //=========================================================================================
        // 機　能：仮想フォルダの利用を開始する
        // 引　数：[in]virtualInfo  利用する仮想フォルダ
        // 　　　　[in]usingType    仮想フォルダの利用形態
        // 戻り値：なし
        // メ　モ：VirtualFolderTemporaryDirectory内にある仮想フォルダを参照するインスタンス一覧に
        // 　　　　渡されたVirtualFolderInfoを登録し、参照カウント++状態にする。
        // 　　　　ユーザーインターフェイススレッドから呼び出す
        //=========================================================================================
        public void BeginUsingVirtualFolder(VirtualFolderInfo virtualInfo, VirtualFolderTemporaryDirectory.UsingType usingType) {
            lock (this) {
                VirtualFolderTemporaryDirectory tempDir = virtualInfo.TemporaryInfo;
                tempDir.BeginUsing(virtualInfo, usingType);
            }
        }
 
        //=========================================================================================
        // 機　能：仮想フォルダの利用を終了する
        // 引　数：[in]virtualInfo  利用する仮想フォルダ
        // 戻り値：なし
        // メ　モ：ユーザーインターフェイススレッドから呼び出す
        //=========================================================================================
        public void EndUsingVirtualFolder(VirtualFolderInfo virtualInfo) {
            lock (this) {
                VirtualFolderTemporaryDirectory tempDir = virtualInfo.TemporaryInfo;
                tempDir.EndUsing(virtualInfo);
            }
        }

        //=========================================================================================
        // 機　能：仮想フォルダを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DeleteVirtualFolder(VirtualFolderTemporaryDirectory tempDir) {
            lock (this) {
                m_virtualFolderList.Remove(tempDir);
                tempDir.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：実行用の仮想フォルダ領域を返す
        // 引　数：[in]executeId   実行用のID
        // 戻り値：実行用の仮想フォルダ（最後は「\」、失敗したときnull）
        //=========================================================================================
        public string CreateVirtualExecuteFolder(VirtualFolderInfo virtualInfo) {
            lock (this) {
                // 作業ディレクトリを作成
                int executeId = virtualInfo.VirtualExecuteId;
                string tempDirRoot = virtualInfo.TemporaryInfo.TemporaryDirectoryRoot;
                string tempDirExecute = tempDirRoot +"\\Execute" + executeId;
                if (!Directory.Exists(tempDirExecute)) {
                    try {
                        Directory.CreateDirectory(tempDirExecute);
                    } catch (Exception) {
                        return null;
                    }
                }
                tempDirExecute = tempDirExecute + "\\";

                // executeIdが同じ管理クラスに保持
                for (int i = 0; i < m_virtualFolderList.Count; i++) {
                    m_virtualFolderList[i].SetVirtualExecuteFolder(executeId, tempDirExecute);
                }
                return tempDirExecute;
            }
        }
    }
}
