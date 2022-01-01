using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;

namespace ShellFiler.Virtual {

    //=========================================================================================
    // クラス：テンポラリ領域の一時的な管理クラス
    //=========================================================================================
    public class TempFileHolder {
        // 監視対象
        private List<string> m_tempFileList = new List<string>();
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]manager  監視対象
        // 戻り値：なし
        //=========================================================================================
        public TempFileHolder() {
        }

        //=========================================================================================
        // 機　能：テンポラリファイルを作成する
        // 引　数：なし
        // 戻り値：作成したテンポラリファイル
        //=========================================================================================
        public string CreateNew() {
            string fileName = Program.Document.TemporaryManager.GetTemporaryFile();
            m_tempFileList.Add(fileName);
            return fileName;
        }

        //=========================================================================================
        // 機　能：テンポラリフォルダ名を返す
        // 引　数：なし
        // 戻り値：テンポラリフォルダ名
        //=========================================================================================
        public string GetTempFolderPath() {
            string path = Program.Document.TemporaryManager.GetTemporaryFileFolder();
            return path;
        }

        //=========================================================================================
        // 機　能：テンポラリファイルをすべて削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DeleteAll() {
            for (int i = 0; i < m_tempFileList.Count; i++) {
                try {
                    File.Delete(m_tempFileList[i]);
                } catch (Exception) {
                    // 失敗のときは無視、そのまま残る
                }
            }
        }
    }
}
