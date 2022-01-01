using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.FileList.DefaultList {

    //=========================================================================================
    // クラス：ファイル一覧の描画やUI操作を切り替えて使用するためのインターフェース
    //=========================================================================================
    public class DefaultFileListViewState : IFileListViewState {
        // 画面表示中の先頭行のファイル全体でのインデックス
        private int m_topLine;

        // カーソル行の画面内でのインデックス
        private int m_cursorScreenLine;

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]topLine           画面表示中の先頭行のファイル全体でのインデックス
        // 　　　　[in]cursorScreenLine  カーソル行の画面内でのインデックス
        // 戻り値：なし
        //=========================================================================================
        public DefaultFileListViewState(int topLine, int cursorScreenLine) {
            m_topLine = topLine;
            m_cursorScreenLine = cursorScreenLine;
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
        // プロパティ：カーソル行の画面内でのインデックス
        //=========================================================================================
        public int CursorScreenLine {
            get {
                return m_cursorScreenLine;
            }
            set {
                m_cursorScreenLine = value;
            }
        }
    }
}
