using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ダンプビューアでの選択範囲
    //=========================================================================================
    public class DumpViewerSelectionRange {
        // 選択開始位置（選択していないとき-1） ※必ずm_startAddressがm_endAddressより左上にある
        private int m_startAddress = -1;
        
        // 選択終了行
        private int m_endAddress = -1;

        // 選択ではじめにクリックしたアドレス
        private int m_firstClickAddress = -1;
        
        // 直前の選択位置のアドレス
        private int m_prevAddress = -1;
        
        // ダンプを選択したときtrue
        private bool m_selectDump = false;
        
        // 一度でも選択状態になったときtrue
        private bool m_selectedFlag = false;

        //=========================================================================================
        // 機　能：範囲がはじめに選択状態になったかどうかをチェックする
        // 引　数：なし
        // 戻り値：はじめに選択状態になったときtrue
        //=========================================================================================
        public bool CheckFirstSelected() {
            bool current = Selected;
            bool prev = m_selectedFlag;
            m_selectedFlag |= current;
            return (prev == false && current == true);
        }

        //=========================================================================================
        // プロパティ：選択中のときtrue
        //=========================================================================================
        public bool Selected {
            get {
                if (m_startAddress == -1) {
                    return false;
                }
                if (m_startAddress == m_endAddress) {
                    return false;
                }
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：選択開始アドレス（選択していないとき-1）
        //=========================================================================================
        public int StartAddress {
            get {
                return m_startAddress;
            }
            set {
                m_startAddress = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：選択終了アドレス（選択していないとき-1）
        //=========================================================================================
        public int EndAddress {
            get {
                return m_endAddress;
            }
            set {
                m_endAddress = value;
            }
        }

        //=========================================================================================
        // プロパティ：選択ではじめにクリックしたアドレス（選択していないとき-1）
        //=========================================================================================
        public int FirstClickAddress {
            get {
                return m_firstClickAddress;
            }
            set {
                m_firstClickAddress = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：直前の選択位置のアドレス
        //=========================================================================================
        public int PrevAddress {
            get {
                return m_prevAddress;
            }
            set {
                m_prevAddress = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダンプを選択したときtrue
        //=========================================================================================
        public bool SelectDump {
            get {
                return m_selectDump;
            }
            set {
                m_selectDump = value;
            }
        }
    }
}
