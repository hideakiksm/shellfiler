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
using ShellFiler.Util;

namespace ShellFiler.Virtual {
    //=========================================================================================
    // クラス：ローカル実行用のディレクトリ
    //=========================================================================================
    public class LocalTemporaryDirectory {
        // 仮想ディレクトリのID
        private int m_virtualId;

        // 作業ディレクトリ
        private string m_temporaryDir;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]id          仮想ディレクトリのID
        // 　　　　[in]dispDir     表示されるディレクトリ
        // 　　　　[in]tempDir  仮想ディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public LocalTemporaryDirectory(int id, string tempDir) {
            m_virtualId = id;
            m_temporaryDir = tempDir;
            try {
                Directory.CreateDirectory(m_temporaryDir);
            } catch (Exception e) {
                throw new SfException(SfException.WorkDirectoryCreate, m_temporaryDir, e.Message);
            }
        }
        
        //=========================================================================================
        // 機　能：テンポラリ空間を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            try {
                Directory.Delete(m_temporaryDir, true);
            } catch (Exception) {
            }
        }

        //=========================================================================================
        // プロパティ：作業ディレクトリ
        //=========================================================================================
        public string VirtualDirectory {
            get {
                return m_temporaryDir;
            }
        }
    }
}
