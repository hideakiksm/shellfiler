using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.FileList.ThumbList {

    //=========================================================================================
    // クラス：ファイル一覧の描画やUI操作を切り替えて使用するためのインターフェース
    //=========================================================================================
    public class ThumbListViewState : IFileListViewState {
        // ビューのモード
        private FileListViewMode m_fileListViewMode;

        // 画面表示中の先頭行のファイル全体でのインデックス
        private int m_topLine;

        // カーソル位置の画面内でのX位置
        private int m_cursorScreenX;

        // カーソル位置の画面内でのY位置
        private int m_cursorScreenY;

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]mode           ファイル一覧の表示モード
        // 　　　　[in]topLine        画面表示中の先頭行のファイル全体でのインデックス
        // 　　　　[in]cursorScreenX  カーソル位置の画面内でのX位置
        // 　　　　[in]cursorScreenY  カーソル位置の画面内でのY位置
        // 戻り値：なし
        //=========================================================================================
        public ThumbListViewState(FileListViewMode mode, int topLine, int cursorScreenX, int cursorScreenY) {
            m_fileListViewMode = mode;
            m_topLine = topLine;
            m_cursorScreenX = cursorScreenX;
            m_cursorScreenY = cursorScreenY;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileListViewMode mode = (FileListViewMode)(m_fileListViewMode.Clone());
            ThumbListViewState obj = new ThumbListViewState(mode, m_topLine, m_cursorScreenX, m_cursorScreenY);
            return obj;
        }

        //=========================================================================================
        // プロパティ：ビューのモード
        //=========================================================================================
        public FileListViewMode FileListViewMode {
            get {
                return m_fileListViewMode;
            }
            set {
                m_fileListViewMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：画面表示中の先頭行のファイル全体でのインデックス
        //=========================================================================================
        public int TopLine {
            get {
                return m_topLine;
            }
            set {
                m_topLine = value;
            }
        }

        //=========================================================================================
        // プロパティ：カーソル位置の画面内でのX位置
        //=========================================================================================
        public int CursorScreenX {
            get {
                return m_cursorScreenX;
            }
            set {
                m_cursorScreenX = value;
            }
        }

        //=========================================================================================
        // プロパティ：カーソル位置の画面内でのY位置
        //=========================================================================================
        public int CursorScreenY {
            get {
                return m_cursorScreenY;
            }
            set {
                m_cursorScreenY = value;
            }
        }
    }
}
