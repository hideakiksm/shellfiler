using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.COM;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using Nomad.Archive.SevenZip;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zのアーカイブ内の構成ファイル1件分の情報
    //=========================================================================================
    public class SevenZipContentsPasswordTest : IProgress, IArchiveExtractCallback, ICryptoGetTextPassword {
        // アーカイブ内構成ファイルの0から始まるインデックス
        private readonly int m_fileIndex;
        
        // パスワード
        private readonly string m_password;

        // ディレクトリのときtrue
        private readonly bool m_isFolder;

        // 処理結果のステータス
        private bool m_success = false;
        
        // 処理対象の合計サイズ
        private long m_totalFileSize = -1;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]archive     オープン済みのアーカイブ
        // 　　　　[in]fileIndex   アーカイブ内構成ファイルの0から始まるインデックス
        // 　　　　[in]password    パスワード
        // 戻り値：なし
        //=========================================================================================
        public SevenZipContentsPasswordTest(IInArchive archive, int fileIndex, string password) {
            m_fileIndex = fileIndex;
            m_password = password;

            // フォルダのときtrue
            PropVariant varIsFolder = new PropVariant();
            archive.GetProperty((uint)m_fileIndex, ItemPropId.kpidIsFolder, ref varIsFolder);
            m_isFolder = (bool)varIsFolder.GetObject();
            varIsFolder.Clear();
        }

        //=========================================================================================
        // 機　能：合計サイズを設定する
        // 引　数：[in]total  合計サイズ
        // 戻り値：なし
        //=========================================================================================
        public int SetTotal(ulong total) {
            m_totalFileSize = (long)(total);
            return 0;
        }

        //=========================================================================================
        // 機　能：処理済みサイズを設定する
        // 引　数：[in,out]completeValue  処理済みサイズ
        // 戻り値：なし
        //=========================================================================================
        public int SetCompleted(ref ulong completeValue) {
            return 0;
        }

        //=========================================================================================
        // 機　能：ストリームを取得する
        // 引　数：[in]index          取得するファイル位置
        // 　　　　[out]outStream     ストリームを返す変数
        // 　　　　[in]askExtractMode モード
        // 戻り値：なし
        //=========================================================================================
        public int GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode) {
            if ((index != m_fileIndex) || (askExtractMode != AskMode.kExtract)) {
                outStream = null;
                return 0;
            }

            if (m_isFolder) {
                // フォルダのとき
                outStream = null;
            } else {
                // ファイルのとき
                outStream = new DummyStream();
            }
            return 0;
        }

        //=========================================================================================
        // 機　能：ファイル操作を準備する
        // 引　数：[in]askExtractMode モード
        // 戻り値：なし
        //=========================================================================================
        public void PrepareOperation(AskMode askExtractMode) {
        }

        //=========================================================================================
        // 機　能：結果を通知する
        // 引　数：[in]result  結果
        // 戻り値：なし
        //=========================================================================================
        public void SetOperationResult(OperationResult result) {
            if (m_totalFileSize > 0 && result == OperationResult.kOK) {
                m_success = true;
            }
        }
        
        //=========================================================================================
        // 機　能：パスワードを返す
        // 引　数：[in]password   パスワードを返す変数
        // 戻り値：ステータスコード
        //=========================================================================================
        public int CryptoGetTextPassword(out string password) {
            password = m_password;
            return 0;
        }

        //=========================================================================================
        // プロパティ：ディレクトリのときtrue
        //=========================================================================================
        public bool IsFolder {
            get {
                return m_isFolder;
            }
        }

        //=========================================================================================
        // プロパティ：処理結果のステータス
        //=========================================================================================
        public bool IsSuccess {
            get {
                return m_success;
            }
        }

        //=========================================================================================
        // クラス：ファイルの書き込みをスキップするためのダミーのストリーム
        //=========================================================================================
        private class DummyStream : ISequentialOutStream, IOutStream {
            public DummyStream() { }
            public int Write(byte[] data, uint size, IntPtr processedSize) {
                return 0;
            }
            public void Seek(long offset, uint seekOrigin, IntPtr newPosition) {
            }
            public int SetSize(long newSize) {
                return 0;
            }
        }
    }
}
