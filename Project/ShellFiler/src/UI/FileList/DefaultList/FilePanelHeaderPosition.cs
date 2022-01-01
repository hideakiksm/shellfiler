using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI.FileList.DefaultList {

    //=========================================================================================
    // クラス：ファイル一覧のヘッダの位置情報
    //=========================================================================================
    class FilePanelHeaderPosition {
        // 表示開始X位置
        private int m_xPos;
        
        // 最小幅
        private int m_minWidth;

        // 現在の幅
        private int m_currentWidth;

        //=========================================================================================
        // プロパティ：表示開始X位置
        //=========================================================================================
        public int XPos {
            get {
                return m_xPos;
            }
            set {
                m_xPos = value;
            }
        }

        //=========================================================================================
        // プロパティ：最小幅
        //=========================================================================================
        public int MinWidth {
            get {
                return m_minWidth;
            }
            set {
                m_minWidth = value;
            }
        }

        //=========================================================================================
        // プロパティ：現在の幅
        //=========================================================================================
        public int CurrentWidth {
            get {
                return m_currentWidth;
            }
            set {
                m_currentWidth = value;
            }
        }
    }
}
