using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Command;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：取得済みアイコンのプール
    // 　　　　右ウィンドウ用と左ウィンドウ用のそれぞれが存在
    //=========================================================================================
    public class FileIconPool {
        // 同一アイコンをスキップするときtrue
        private bool m_checkSinonym;

        // ロード済みアイコンのリスト
        private Dictionary<FileIconID, FileIcon> m_iconIdToIcon = new Dictionary<FileIconID, FileIcon>();

        // アイコンのハッシュ値からアイコン本体へのmap（m_checkSinonymがfalseのときは未使用）
        private Dictionary<int, List<FileIcon>> m_hashToIconList = new Dictionary<int,List<FileIcon>>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]checkSinonym  同一アイコンをスキップするときtrue
        // 戻り値：なし
        //=========================================================================================
        public FileIconPool(bool checkSinonym) {
            m_checkSinonym = checkSinonym;
        }

        //=========================================================================================
        // 機　能：アイコンを登録する
        // 引　数：[in]icon          登録するアイコン
        // 戻り値：登録したアイコンのID、または既存の同一アイコンのID
        //=========================================================================================
        public FileIconID AddIcon(FileIcon icon) {
            lock (this) {
                if (m_checkSinonym && m_hashToIconList.ContainsKey(icon.GetHashCode())) {
                    List<FileIcon> sinonymList = m_hashToIconList[icon.GetHashCode()];
                    foreach (FileIcon sinonym in sinonymList) {
                        if (sinonym.EqualsIconImage(icon)) {
                            return sinonym.FileIconId;
                        }
                    }
                    sinonymList.Add(icon);
                }
                m_iconIdToIcon.Add(icon.FileIconId, icon);
            }
            return icon.FileIconId;
        }

        //=========================================================================================
        // 機　能：アイコンを取得する
        // 引　数：[in]iconId   アイコンのID
        // 戻り値：アイコン（取得できなかったときnull）
        //=========================================================================================
        public FileIcon GetFileIcon(FileIconID id) {
            lock (this) {
                if (m_iconIdToIcon.ContainsKey(id)) {
                    return m_iconIdToIcon[id];
                } else {
                    return null;
                }
            }
        }

        //=========================================================================================
        // 機　能：全アイコンを削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearAll() {
            lock (this) {
                foreach (FileIcon icon in m_iconIdToIcon.Values) {
//System.Diagnostics.Debug.WriteLine("dispose:" + icon.FileIconId.IdValue);
                    icon.Dispose();
                }
                m_hashToIconList.Clear();
                m_iconIdToIcon.Clear();
            }
        }

        //=========================================================================================
        // プロパティ：マークされている対象が1件でもあればtrue
        //=========================================================================================
        public bool IsMarked {
            get {
                return true;
            }
        }
    }
}
