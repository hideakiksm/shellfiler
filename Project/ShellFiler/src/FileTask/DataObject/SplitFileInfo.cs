using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // クラス：ファイルの分割方法
    //=========================================================================================
    public class SplitFileInfo : ICloneable {
        // 分割数の最大(Windows)
        public const int MAX_SPLIT_COUNT_WINDOWS = 9999;

        // 分割数の最大(SSH)
        public const int MAX_SPLIT_COUNT_SSH = 99;

        // FDのサイズ
        public const int SPLIT_SIZE_FD = 1457664;

        // CD(650MB)のサイズ
        public const int SPLIT_SIZE_CD650 = 681574400;

        // CD(700MB)のサイズ
        public const int SPLIT_SIZE_CD700 = 734003200;

        // 分割方法
        private bool m_splitNumber;

        // サイズで分割：ファイルサイズ
        private long m_sizeFileSize;

        // サイズで分割：単位（1, 1024, 1024*1024）
        private int m_sizeUnit;
        
        // 個数で分割：ファイル数
        private int m_countFileCount;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SplitFileInfo() {
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]splitNumber     分割方法
        // 　　　　[in]sizeFileSize    サイズで分割：ファイルサイズ
        // 　　　　[in]sizeUnit        サイズで分割：単位（1, 1024, 1024*1024）
        // 　　　　[in]countFileCount  個数で分割：ファイル数
        // 戻り値：なし
        //=========================================================================================
        public SplitFileInfo(bool splitNumber, long sizeFileSize, int sizeUnit, int fileCount) {
            m_splitNumber = splitNumber;
            m_sizeFileSize = sizeFileSize;
            m_sizeUnit = sizeUnit;
            m_countFileCount = fileCount;
        }

        //=========================================================================================
        // 機　能：UIでのデフォルト設定を返す
        // 引　数：なし
        // 戻り値：UIでのデフォルト設定
        //=========================================================================================
        public static SplitFileInfo DefaultUI() {
            return new SplitFileInfo(false, 1, 1024 * 1024, 10);
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            return MemberwiseClone();
        }

        //=========================================================================================
        // 機　能：分割ファイルサイズを取得する
        // 引　数：なし
        // 戻り値：分割ファイルサイズ
        //=========================================================================================
        public long GetSplitSize() {
            long fileSize = (long)m_sizeFileSize * (long)m_sizeUnit;
            return fileSize;
        }
        
        //=========================================================================================
        // 機　能：分割ファイル１件の大きさを取得する
        // 引　数：[in]fileLength       分割元ファイルの長さ
        // 　　　　[in]srcFileSystem    対象パスのファイルシステム
        // 　　　　[in]destFileSystem   反対パスのファイルシステム
        // 　　　　[out]oneFileSize     ファイル１件分の長さを返す変数（最後はファイルサイズ以下に呼び出し元で調整）
        // 　　　　[out]totalFileCount  全体のファイル数を返す変数
        // 戻り値：分割ファイル1件の長さ
        //=========================================================================================
        public bool GetOneFileSize(long fileLength, FileSystemID srcFileSystem, FileSystemID destFileSystem, out long oneFileSize, out int totalFileCount) {
            long longTotalFileCount;
            if (!m_splitNumber) {
                // サイズで分割
                oneFileSize = m_sizeFileSize * m_sizeUnit;
                longTotalFileCount = (fileLength + oneFileSize - 1) / oneFileSize; 
            } else {
                // 個数で分割
                oneFileSize = (fileLength + m_countFileCount - 1) / m_countFileCount;
                longTotalFileCount = (fileLength + oneFileSize - 1) / oneFileSize; 
            }

            // 全体サイズを調整
            int maxSplitCount = GetMaxSplitCount(srcFileSystem, destFileSystem);
            if (longTotalFileCount == 0) {
                totalFileCount = 1;
                return true;
            } else if (longTotalFileCount <= maxSplitCount) {
                totalFileCount = (int)longTotalFileCount;
                return true;
            } else {
                totalFileCount = 0;
                return false;
            }
        }
        
        //=========================================================================================
        // 機　能：ファイルの最大分割数を取得する
        // 引　数：[in]srcFileSystem   対象パスのファイルシステム
        // 　　　　[in]destFileSystem  反対パスのファイルシステム
        // 戻り値：ファイルの最大分割数
        //=========================================================================================
        public static int GetMaxSplitCount(FileSystemID srcFileSystem, FileSystemID destFileSystem) {
            if (FileSystemID.IsSSH(srcFileSystem) && FileSystemID.IsSSH(destFileSystem)) {
                return MAX_SPLIT_COUNT_SSH;
            } else {
                return MAX_SPLIT_COUNT_WINDOWS;
            }
        }

        //=========================================================================================
        // プロパティ：分割方法
        //=========================================================================================
        public bool SplitNumber {
            get {
                return m_splitNumber;
            }
            set {
                m_splitNumber = value;
            }
        }

        //=========================================================================================
        // プロパティ：サイズで分割：ファイルサイズ
        //=========================================================================================
        public long SizeFileSize {
            get {
                return m_sizeFileSize;
            }
            set {
                m_sizeFileSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：サイズで分割：単位（1, 1024, 1024*1024）
        //=========================================================================================
        public int SizeUnit {
            get {
                return m_sizeUnit;
            }
            set {
                m_sizeUnit = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：個数で分割：ファイル数
        //=========================================================================================
        public int CountFileCount {
            get {
                return m_countFileCount;
            }
            set {
                m_countFileCount = value;
            }
        }
    }
}
