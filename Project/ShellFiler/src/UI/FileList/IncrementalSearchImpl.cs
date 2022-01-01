using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.UI.FileList.DefaultList;
using ShellFiler.UI.FileList.ThumbList;
using ShellFiler.Util;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：インクリメンタルサーチの実装
    //=========================================================================================
    public class IncrementalSearchImpl {
        // ファイル一覧
        private UIFileList m_uiFileList;

        // ファイル一覧コンポーネント
        private IFileListViewComponent m_component;

        // マークを実施する際のdelegate
        private MarkDelegate m_markDelegate;

        // マークを実施する際のdeletateの型定義
        public delegate void MarkDelegate();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]uiFileList  ファイル一覧
        // 　　　　[in]component   ファイル一覧コンポーネント
        // 戻り値：なし
        //=========================================================================================
        public IncrementalSearchImpl(UIFileList uiFileList, IFileListViewComponent component, MarkDelegate markDelegate) {
            m_uiFileList = uiFileList;
            m_component = component;
            m_markDelegate = markDelegate;
        }

        //=========================================================================================
        // 機　能：ファイルをインクリメンタルサーチする
        // 引　数：[in]searchText  検索文字列
        // 　　　　[in]fromHead    ファイル名の先頭から比較するときtrue
        // 　　　　[in]operation   ファイル操作の種類
        // 戻り値：検索にヒットしたときtrue、見つからないとき/これ以上操作できないときfalse
        //=========================================================================================
        public bool IncrementalSearch(string searchText, bool fromHead, IncrementalSearchOperation operation) {
            if (m_uiFileList.Files.Count == 0) {
                return false;
            }
            searchText = searchText.ToUpper();
            switch (operation) {
                case IncrementalSearchOperation.FromTop: {
                    if (searchText == "") {
                        // 文字列なしは先頭に移動
                        m_component.MoveCursorLine(0);
                    } else {
                        // 次を検索
                        int line = IncrementalSearchNextLine(searchText, fromHead, 0, true);
                        if (line == -1) {
                            return false;
                        }
                        m_component.MoveCursorLine(line);
                    }
                    break;
                }
                case IncrementalSearchOperation.MoveDown: {
                    int line = IncrementalSearchNextLine(searchText, fromHead, m_component.CursorLineNo + 1, true);
                    if (line == -1) {
                        return false;
                    }
                    m_component.MoveCursorLine(line);
                    break;
                }
                case IncrementalSearchOperation.MoveUp: {
                    int line = IncrementalSearchNextLine(searchText, fromHead, m_component.CursorLineNo - 1, false);
                    if (line == -1) {
                        return false;
                    }
                    m_component.MoveCursorLine(line);
                    break;
                }
                case IncrementalSearchOperation.Mark: {
                    m_markDelegate();
                    int line = IncrementalSearchNextLine(searchText, fromHead, m_component.CursorLineNo + 1, true);
                    if (line == -1) {
                        return false;
                    }
                    m_component.MoveCursorLine(line);
                    break;
                }
            }
            return true;
        }
        
        //=========================================================================================
        // 機　能：ファイルをインクリメンタルサーチして見つかったファイルのインデックスを返す
        // 引　数：[in]searchText  検索文字列（すべて大文字）
        // 　　　　[in]fromHead    ファイル名の先頭から比較するときtrue
        // 　　　　[in]startIndex  検索開始行番号
        // 　　　　[in]down        下方向への検索のときtrue
        // 戻り値：検索にヒットしたときヒットした行番号、見つからなかったとき-1
        //=========================================================================================
        private int IncrementalSearchNextLine(string searchText, bool fromHead, int startIndex, bool down) {
            if (down) {
                // 下方向に検索
                for (int i = startIndex; i < m_uiFileList.Files.Count; i++) {
                    if (IncrementalSearchEqualsFile(searchText, m_uiFileList.Files[i].FileName, fromHead)) {
                        return i;
                    }
                }
                return -1;
            } else {
                // 上向きに検索
                for (int i = startIndex; i >= 0; i--) {
                    if (IncrementalSearchEqualsFile(searchText, m_uiFileList.Files[i].FileName, fromHead)) {
                        return i;
                    }
                }
                return -1;
            }
        }

        //=========================================================================================
        // 機　能：インクリメンタルサーチでファイル名が一致するかどうかを確認する
        // 引　数：[in]searchText  検索文字列（すべて大文字）
        // 　　　　[in]fileName    ファイル名
        // 　　　　[in]startIndex  検索開始行番号
        // 　　　　[in]down        下方向への検索のときtrue
        // 戻り値：検索にヒットしたときヒットした行番号、見つからなかったとき-1
        //=========================================================================================
        public static bool IncrementalSearchEqualsFile(string searchText, string fileName, bool fromHead) {
            fileName = fileName.ToUpper();
            if (fromHead) {
                // 先頭から比較
                return fileName.StartsWith(searchText);
            } else {
                // 途中から比較
                return fileName.IndexOf(searchText) != -1;
            }
        }
    }
}
