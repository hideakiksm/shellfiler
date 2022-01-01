using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.FileSystem;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：フォルダサイズの取得結果
    //=========================================================================================
    public class RetrieveFolderSizeResult {
        // 集計中のフォルダの登録結果（フォルダ情報は[depth-1]に登録、Windowsではファイルを小文字化して格納、Final後はnull）
        private List<List<FolderInfo>> m_tempFolderResult = new List<List<FolderInfo>>();

        // 集計済みのフォルダの登録結果（フォルダ情報は[depth-1]に登録、Windowsではファイルを小文字化して格納）
        private List<Dictionary<string, long>> m_folderResult = new List<Dictionary<string, long>>();

        // 登録済みのフォルダ総数
        private int m_totalFolderCount = 0;

        // マークフォルダの共通の親（小文字化、最後に「\」）
        private string m_folderBase;

        // パス名の大文字小文字を無視するときtrue
        private bool m_ignoreCase;

        // パス名の「/」を「\」に置換するときtrue
        private bool m_replaceSlashToEn;

        // 最大階層の設定
        private int m_configMaxDepth;

        // 最大フォルダ数の設定
        private int m_configMaxFolder;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileSystemId    実行するファイルシステム
        // 　　　　[in]folderBase      フォルダサイズを取得する際のベースフォルダ（マークした親）
        // 　　　　[in]configMaxDepth  最大階層の設定
        // 　　　　[in]configMaxFolder 最大フォルダ数の設定
        // 戻り値：なし
        //=========================================================================================
        public RetrieveFolderSizeResult(FileSystemID fileSystemId, string folderBase, int configMaxDepth, int configMaxFolder) {
            m_ignoreCase = FileSystemID.IgnoreCaseFolderPath(fileSystemId);
            m_replaceSlashToEn = FileSystemID.IsSSH(fileSystemId);
            m_folderBase = folderBase;
            if (m_ignoreCase) {
                m_folderBase = m_folderBase.ToLower();
            }
            if (m_replaceSlashToEn) {
                m_folderBase = m_folderBase.Replace('/', '\\');
            }
            if (!m_folderBase.EndsWith("\\")) {
                m_folderBase += "\\";
            }
            m_configMaxDepth = configMaxDepth;
            m_configMaxFolder = configMaxFolder;
        }

        //=========================================================================================
        // 機　能：フォルダサイズの結果を格納する
        // 引　数：[in]subPath     結果のパス（dir1\dir2\dir3形式、dir1がマークフォルダ）
        // 　　　　[in]depth       フォルダの深さ（dir1\dir2\dir3は3）
        // 　　　　[in]folderSize  フォルダ以下のファイルの合計
        // 戻り値：なし
        //=========================================================================================
        public void AddResult(string subPath, int depth, long folderSize) {
            if (depth > m_configMaxDepth) {
                // 設定の最大階層を超えるとき
                return;
            }
            if (m_totalFolderCount == m_configMaxFolder) {
                // 登録済みの最大階層と同じ階層か、それ以上に深い階層は登録できない
                if (depth >= m_tempFolderResult.Count) {
                    return;
                }
                // 登録済みの最大階層を削って登録
                int maxDepth = m_tempFolderResult.Count - 1;
                int countDepthI = m_tempFolderResult[maxDepth].Count;
                if (countDepthI == 1) {
                    m_tempFolderResult.RemoveAt(maxDepth);
                } else {
                    m_tempFolderResult[maxDepth].RemoveAt(countDepthI - 1);
                }
                m_totalFolderCount--;
            }

            // 登録
            for (int i = m_tempFolderResult.Count; i < depth; i++) {
                m_tempFolderResult.Add(new List<FolderInfo>());
            }
            if (m_ignoreCase) {
                subPath = subPath.ToLower();
            }
            if (m_replaceSlashToEn) {
                subPath = subPath.Replace('/', '\\');
            }
            m_tempFolderResult[depth - 1].Add(new FolderInfo(subPath, folderSize));
            m_totalFolderCount++;
        }

        //=========================================================================================
        // 機　能：フォルダサイズの結果格納が終わったときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void FinalAddResult() {
            // ListをMapに変換
            List<Dictionary<string, long>> folderResult = new List<Dictionary<string, long>>();
            for (int i = 0; i < m_tempFolderResult.Count; i++) {
                Dictionary<string, long> map = new Dictionary<string, long>();
                folderResult.Add(map);
                int countI = m_tempFolderResult[i].Count;
                for (int j = 0; j < countI; j++) {
                    FolderInfo folderInfo = m_tempFolderResult[i][j];
                    if (!map.ContainsKey(folderInfo.SubPath)) {
                        map.Add(folderInfo.SubPath, folderInfo.FolderSize);
                    } else {
                        map[folderInfo.SubPath] = -1;
                    }
                }
            }

            // 結果を保持
            m_folderResult = folderResult;
            m_tempFolderResult = null;
        }

        //=========================================================================================
        // 機　能：指定フォルダがこの結果に含まれているかどうかを返す
        // 引　数：[in]fileSystem   対象フォルダのファイルシステム
        // 　　　　[in]path         対象フォルダ
        // 　　　　[out]subPath     対象フォルダを、この結果内のサブパス表現に変換した結果を返す変数
        // 　　　　[out]depth       対象フォルダの、この結果内のサブパスでの深さに変換した結果を返す変数
        // 戻り値：指定フォルダが結果に含まれるときtrue
        //=========================================================================================
        public bool IsTargetFolder(FileSystemID fileSystem, string path, out string subPath, out int depth) {
            subPath = null;
            depth = -1;
            if (m_ignoreCase) {
                path = path.ToLower();
            }
            if (m_replaceSlashToEn) {
                path = path.Replace('/', '\\');
            }
            if (!path.EndsWith("\\")) {
                path = path + "\\";
            }
            if (!path.StartsWith(m_folderBase)) {
                return false;
            }
            subPath = path.Substring(m_folderBase.Length);
            depth = StringUtils.CountCharOf(subPath, '\\');
            return true;
        }

        //=========================================================================================
        // 機　能：フォルダサイズを返す
        // 引　数：[in]subPath   取得したいフォルダの、この結果内のサブパス表現
        // 　　　　[in]depth     取得したいフォルダの、この結果内のサブパスでの深さ
        // 戻り値：フォルダサイズ（登録されていないときは-1）
        //=========================================================================================
        public long LookupFolderSize(string subPath, int depth) {
            long result = -1;
            if (depth <= m_folderResult.Count) {
                if (m_ignoreCase) {
                    subPath = subPath.ToLower();
                }
                if (m_replaceSlashToEn) {
                    subPath = subPath.Replace('/', '\\');
                }
                if (m_folderResult[depth - 1].ContainsKey(subPath)) {
                    result = m_folderResult[depth - 1][subPath];
                }
            }
            return result;
        }

        //=========================================================================================
        // プロパティ：マークフォルダの共通の親（小文字化、最後に「\」）
        //=========================================================================================
        public string FolderBase {
            get {
                return m_folderBase;
            }
        }

        //=========================================================================================
        // クラス：内部で使用するフォルダ情報
        //=========================================================================================
        private class FolderInfo {
            // 目標のパスからみたサブパス（dir1\dir2\dir3、dir1がマークフォルダ）
            private string m_subPath;

            // フォルダ内のファイルサイズ合計
            private long m_folderSize;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]subPath     目標のパスからみたサブパス（dir1\dir2\dir3、dir1がマークフォルダ）
            // 　　　　[in]folderSize  フォルダ内のファイルサイズ合計
            // 戻り値：なし
            //=========================================================================================
            public FolderInfo(string subPath, long folderSize) {
                m_subPath = subPath;
                m_folderSize = folderSize;
            }

            //=========================================================================================
            // プロパティ：目標のパスからみたサブパス（dir1\dir2\dir3、dir1がマークフォルダ）
            //=========================================================================================
            public string SubPath {
                get {
                    return m_subPath;
                }
            }

            //=========================================================================================
            // プロパティ：フォルダ内のファイルサイズ合計
            //=========================================================================================
            public long FolderSize {
                get {
                    return m_folderSize;
                }
            }
        }
    }
}
