using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：パスヒストリ
    //=========================================================================================
    public class PathHistory : ICloneable {
        // パスヒストリの最大記憶件数
        private int m_maxPathHistoryCount;
        
        // パスヒストリのリスト（添字が大きいほど新しい）
        private List<PathHistoryItem> m_historyList = new List<PathHistoryItem>();
        
        // 現在の参照位置
        private int m_currentIndex = -1;

        // Cloneをサポート

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public PathHistory() {
             m_maxPathHistoryCount = Configuration.PathHistoryMaxCount;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            PathHistory pathHistory = new PathHistory();
            pathHistory.m_historyList = new List<PathHistoryItem>();
            foreach (PathHistoryItem item in m_historyList) {
                pathHistory.m_historyList.Add((PathHistoryItem)item.Clone());
            }
            pathHistory.m_currentIndex = m_currentIndex;
            return pathHistory;
        }

        //=========================================================================================
        // 機　能：項目を追加する
        // 引　数：[in]directory    ディレクトリ
        // 　　　　[in]fileName     ファイル名
        // 　　　　[in]fileSystemId ファイルシステムのID
        // 戻り値：なし
        //=========================================================================================
        public void AddItem(string directory, string fileName, FileSystemID fileSystemId) {
            PathHistoryItem item = new PathHistoryItem(directory, fileName, fileSystemId);

            // 切り詰める
            if (m_currentIndex != m_historyList.Count - 1) {
                m_historyList.RemoveRange(m_currentIndex + 1, m_historyList.Count - (m_currentIndex + 1));
            }

            if (m_historyList.Count >= m_maxPathHistoryCount) {
                // 追加しようとしたが、最大件数の場合は古いものを削除
                for (int i = 0; i < m_historyList.Count - 1; i++) {
                    m_historyList[i] = m_historyList[i + 1];
                }
                m_historyList[m_historyList.Count - 1] = item;
            } else {
                // まだ追加できる場合
                m_historyList.Add(item);
                m_currentIndex++;
            }
        }

        //=========================================================================================
        // 機　能：現在位置のファイル名を更新する
        // 引　数：[in]fileName     ファイル名
        // 戻り値：なし
        //=========================================================================================
        public void UpdateCurrentDirectory(string fileName) {
            m_historyList[m_currentIndex].FileName = fileName;
        }

        //=========================================================================================
        // 機　能：履歴をすべて削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearAllHistory() {
            m_historyList.Clear();
            m_currentIndex = -1;
        }

        //=========================================================================================
        // プロパティ：直前のパスヒストリに移動できるときtrue
        //=========================================================================================
        public bool EnablePrev {
            get {
                return (m_currentIndex >= 1);
            }
        }

        //=========================================================================================
        // プロパティ：直後のパスヒストリに移動できるときtrue
        //=========================================================================================
        public bool EnableNext {
            get {
                return (m_currentIndex < m_historyList.Count - 1);
            }
        }

        //=========================================================================================
        // プロパティ：パスヒストリのリスト（添字が大きいほど新しい）
        //=========================================================================================
        public List<PathHistoryItem> HistoryList {
            get {
                return m_historyList;
            }
        }

        //=========================================================================================
        // プロパティ：現在の参照位置
        //=========================================================================================
        public int CurrentIndex {
            get {
                return m_currentIndex;
            }
            set {
                m_currentIndex = value;
            }
        }

        //=========================================================================================
        // 機　能：指定されたディレクトリの最新ファイル位置を返す
        // 引　数：[in]history1    左ウィンドウでのパスヒストリ
        // 　　　　[in]history2    右ウィンドウでのパスヒストリ
        // 　　　　[in]directory   探索するディレクトリ
        // 　　　　[in]ignoreCase  大文字小文字を無視するときtrue
        // 　　　　[in]rootSearch  direstoryにルートディレクトリを入れて検索するときtrue
        // 戻り値：ヒストリの項目情報
        //=========================================================================================
        public static PathHistoryItem GetLatestHistoryItem(PathHistory history1, PathHistory history2, string directory, bool ignoreCase, bool rootSearch) {
            if (directory.EndsWith("\\") || directory.EndsWith("/")) {
                directory = directory.Substring(0, directory.Length - 1);
            }
            int index1 = history1.CurrentIndex;
            int index2 = history2.CurrentIndex;
            PathHistoryItem item = null;
            while (true) {
                PathHistoryItem item1 = GetNextHistoryItem(ref index1, history1.CurrentIndex, history1.HistoryList);
                PathHistoryItem item2 = GetNextHistoryItem(ref index2, history2.CurrentIndex, history2.HistoryList);
                if (item1 == null & item2 == null) {
                    item = null;
                    break;
                } else if (item1 != null && item2 == null) {
                    item = item1;
                } else if (item1 == null && item2 != null) {
                    item = item2;
                } else if (item1.SequenceNo > item2.SequenceNo) {
                    item = item1;
                } else {
                    item = item2;
                }
                if (rootSearch) {
                    string itemDirectory = item.Directory.ToUpper();
                    itemDirectory = itemDirectory.Substring(0, Math.Min(directory.Length, itemDirectory.Length));
                    if (directory.ToUpper() == itemDirectory) {
                        break;
                    }
                } else if (ignoreCase) {
                    if (directory.ToUpper() == item.Directory.ToUpper()) {
                        break;
                    }
                } else {
                    if (directory == item.Directory) {
                        break;
                    }
                }
            }
            return item;
        }

        //=========================================================================================
        // 機　能：ヒストリの中から次の優先度の位置の項目を取得する
        // 引　数：[in,out]nextIndex  次に探す項目のリスト中の位置（次の位置に更新して返す）
        // 　　　　[in]dispIndex      現在表示中の項目のリスト中の位置
        // 　　　　[in]itemList       ヒストリの項目リスト
        // 戻り値：ヒストリの項目情報（null:これ以上ない）
        //=========================================================================================
        private static PathHistoryItem GetNextHistoryItem(ref int nextIndex, int dispIndex, List<PathHistoryItem> itemList) {
            if (nextIndex == -1 || itemList.Count == 0) {
                return null;
            }
            PathHistoryItem result = null;
            if (nextIndex < dispIndex) {
                // 過去に向かって移動中
                result = itemList[nextIndex];
                nextIndex--;
            } else {
                // 未来に向かって移動中
                result = itemList[nextIndex];
                nextIndex++;
                if (itemList.Count <= nextIndex) {
                    nextIndex = dispIndex - 1;
                }
            }
            return result;
        }
    }
}
