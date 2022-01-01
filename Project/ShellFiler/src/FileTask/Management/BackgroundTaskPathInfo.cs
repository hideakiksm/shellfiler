using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Management {

    //=========================================================================================
    // クラス：バックグラウンドタスクのパス情報
    //=========================================================================================
    public class BackgroundTaskPathInfo {
        // 転送元/転送先情報で作成するファイル情報の最大件数
        protected const int MAX_PATH_INFO_COUNT = 10;

        // 転送元パス情報の短縮名
        private string m_srcShort;

        // 転送元パス情報の詳細情報
        private string m_srcDetail;

        // 転送先パス情報の短縮名
        private string m_destShort;
        
        // 転送先パス情報の詳細情報
        private string m_destDetail;

        // 現在の進捗状態の値
        private int m_progressCurrent;

        // 作業完了時の進捗状態の値（0のとき未使用）
        private int m_progressAll = 0;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcShort   転送元パス情報の短縮名
        // 　　　　[in]srcDetail  転送元パス情報の詳細情報
        // 　　　　[in]destShort  転送先パス情報の短縮名
        // 　　　　[in]destDetail 転送先パス情報の詳細情報
        // 戻り値：なし
        //=========================================================================================
        public BackgroundTaskPathInfo(string srcShort, string srcDetail, string destShort, string destDetail) {
            m_srcShort = srcShort;
            m_srcDetail = srcDetail;
            m_destShort = destShort;
            m_destDetail = destDetail;
        }

        //=========================================================================================
        // 機　能：進捗状態を設定する
        // 引　数：[in]current   現在の進捗状態
        // 　　　　[in]all       進捗状態の最大値
        // 戻り値：なし
        //=========================================================================================
        public void SetProgressCount(int current, int all) {
            m_progressCurrent = current;
            m_progressAll = all;
        }

        //=========================================================================================
        // 機　能：転送元の概要情報を作成する
        // 引　数：[in]provider  転送元の情報
        // 　　　　[in]retryList 再試行するファイルのリスト（再試行情報がないときnull）
        // 戻り値：なし
        //=========================================================================================
        public static string CreateShortTextFileProviderSrc(IFileProviderSrc provider, IRetryInfo[] retryList) {
            if (retryList == null) {
                retryList = new IRetryInfo[0];
            }
            string srcShort = "";
            if (provider.SrcItemCount > 0) {
                SimpleFileDirectoryPath path = provider.GetSrcPath(0);
                srcShort = provider.SrcFileSystem.GetFileName(path.FilePath);
            } else if (retryList.Length > 0) {
                srcShort = provider.SrcFileSystem.GetFileName(retryList[0].SrcMarkObjectPath.FilePath);
            }
            if (provider.SrcItemCount + retryList.Length >= 2) {
                srcShort += Resources.DlgTaskMan_ListShortOther;
            }
            return srcShort;
        }

        //=========================================================================================
        // 機　能：転送元の詳細情報を作成する
        // 引　数：[in]provider  転送元の情報
        // 　　　　[in]retryList 再試行するファイルのリスト（再試行情報がないときnull）
        // 戻り値：なし
        //=========================================================================================
        public static string CreateDetailTextFileProviderSrc(IFileProviderSrc provider, IRetryInfo[] retryList) {
            if (retryList == null) {
                retryList = new IRetryInfo[0];
            }
            int lineCount = 0;
            StringBuilder srcDetailBuf = new StringBuilder();
            
            // 再試行ファイルの一覧
            for (int i = 0; i < retryList.Length; i++) {
                if (lineCount > MAX_PATH_INFO_COUNT) {
                    break;
                }
                srcDetailBuf.Append(retryList[i].SrcMarkObjectPath.FilePath).Append("\r\n");
                lineCount++;
            }
            
            // 対象ファイルの一覧
            for (int i = 0; i < provider.SrcItemCount; i++) {
                if (lineCount > MAX_PATH_INFO_COUNT) {
                    break;
                }
                SimpleFileDirectoryPath pathInfo = provider.GetSrcPath(i);
                srcDetailBuf.Append(pathInfo.FilePath).Append("\r\n");
                lineCount++;
            }

            // 件数オーバーのときは「他」をつける
            if (retryList.Length + provider.SrcItemCount >= MAX_PATH_INFO_COUNT) {
                string other = string.Format(Resources.DlgTaskMan_ListDetailOther, provider.SrcItemCount);
                srcDetailBuf.Append(other);
            }
            string srcDetail = StringUtils.RemoveLastLineBreak(srcDetailBuf.ToString());
            return srcDetail;
        }

        //=========================================================================================
        // プロパティ：転送元パス情報の短縮名
        //=========================================================================================
        public string SrcShort {
            get {
                return m_srcShort;
            }
        }

        //=========================================================================================
        // プロパティ：転送元パス情報の詳細情報
        //=========================================================================================
        public string SrcDetail {
            get {
                return m_srcDetail;
            }
        }

        //=========================================================================================
        // プロパティ：転送先パス情報の短縮名
        //=========================================================================================
        public string DestShort {
            get {
                return m_destShort;
            }
        }

        //=========================================================================================
        // プロパティ：転送先パス情報の詳細情報
        //=========================================================================================
        public string DestDetail {
            get {
                return m_destDetail;
            }
        }

        //=========================================================================================
        // プロパティ：現在の進捗状態の値
        //=========================================================================================
        public int ProgressCurrent {
            get {
                return m_progressCurrent;
            }
        }

        //=========================================================================================
        // プロパティ：作業完了時の進捗状態の値（0のとき未使用）
        //=========================================================================================
        public int ProgressAll {
            get {
                return m_progressAll;
            }
        }
    }
}
