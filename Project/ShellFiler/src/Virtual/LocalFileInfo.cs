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
    // クラス：ローカルにダウンロードしたファイルの情報
    //=========================================================================================
    public class LocalFileInfo {
        // ファイルのパス名
        private string m_filePath;

        // 更新時刻
        private DateTime m_lastWriteTime;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]filePath      ファイルのパス名
        // 　　　　[in]lastWriteTime 更新時刻
        // 戻り値：なし
        //=========================================================================================
        public LocalFileInfo(string filePath, DateTime lastWriteTime) {
            m_filePath = filePath;
            m_lastWriteTime = lastWriteTime;
        }

        //=========================================================================================
        // プロパティ：ファイルのパス名
        //=========================================================================================
        public string FilePath {
            get {
                return m_filePath;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルのパス名
        //=========================================================================================
        public DateTime LastWriteTime {
            get {
                return m_lastWriteTime;
            }
        }
    }
}
