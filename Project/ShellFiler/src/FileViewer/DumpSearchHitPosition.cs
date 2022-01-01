using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Text;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ダンプ用のユーティリティクラス
    //=========================================================================================
    public class DumpSearchHitPosition {
        // 状態：ヒットしていない
        public byte FLAG_NOT_HIT = 0;

        // 状態：検索だけヒット
        public byte FLAG_HIT_SEARCH = 1;

        // 状態：自動検索だけヒット
        public byte FLAG_HIT_AUTO = 2;
        
        // 状態：両方ヒット
        public byte FLAG_HIT_BOTH = 3;


        // 表示の際の開始行
        private int m_startLine;

        // 表示の際の終了行
        private int m_endLine;

        // 1行に表示するバイト数
        private int m_lineBytes;

        // 有効なファイルサイズ
        private int m_fileSize;

        // 検索にヒットしたかどうかのフラグ
        private byte[] m_hitFlag = null;

        // m_hitFlagの先頭位置のアドレス
        private int m_bufferStartAddress;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]startLine   表示の際の開始行
        // 　　　　[in]endLine     表示の際の終了行
        // 　　　　[in]lineBytes   1行に表示するバイト数
        // 　　　　[in]fileSize    有効なファイルサイズ
        // 戻り値：なし
        // メ　モ：処理中にファイルサイズが増えた場合、fileSizeまでを有効にする
        //=========================================================================================
        public DumpSearchHitPosition(int startLine, int endLine, int lineBytes, int fileSize) {
            m_startLine = startLine;
            m_endLine = endLine;
            m_lineBytes = lineBytes;
            m_fileSize = fileSize;
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
            int size = EndAddress - StartAddress + 1;
            m_hitFlag = new byte[size];
            m_bufferStartAddress = m_startLine * m_lineBytes;
        }

        //=========================================================================================
        // 機　能：ヒット状態をセットする
        // 引　数：[in]start   ヒット開始位置
        // 　　　　[in]length  キーワードの長さ
        // 　　　　[in]search  検索語句によるときtrue、自動検索のときfalse
        // 戻り値：なし
        //=========================================================================================
        public void FillHitState(int start, int length, bool search) {
            int fillStart = Math.Max(0, start - StartAddress);
            int fillEnd = Math.Min(start + length - 1, EndAddress)  - StartAddress; 
            if (search) {
                for (int i = fillStart; i <= fillEnd; i++) {
                    m_hitFlag[i] |= FLAG_HIT_SEARCH;
                }
            } else {
                for (int i = fillStart; i <= fillEnd; i++) {
                    m_hitFlag[i] |= FLAG_HIT_AUTO;
                }
            }
        }

        //=========================================================================================
        // 機　能：検索にヒットした変化点を求める（自動検索は無視）
        // 引　数：[in]line   変化点を求める行
        // 戻り値：変化点（自動検索は常にfalse、ヒットした場合はhit=trueで始まり、hit=endで終わる）
        // メ　モ：m_hitFlag={0,1,1,2,3,1}のとき、結果は{{1,true,false},{3,false,false},{5,true,false},{6,false,false}}
        //=========================================================================================
        public List<HitDiffPoint> GetHitPointListSearch(int line) {
            List<HitDiffPoint> result = new List<HitDiffPoint>();
            int startAddr = StartAddress;
            int start = line * m_lineBytes - startAddr;
            int end = Math.Min(EndAddress, (line + 1) * m_lineBytes - 1) - startAddr;
            bool prevHit = false;
            for (int i = start; i <= end; i++) {
                bool currHit = (m_hitFlag[i] == FLAG_HIT_SEARCH) || (m_hitFlag[i] == FLAG_HIT_BOTH);
                if (prevHit != currHit) {
                    prevHit = currHit;
                    result.Add(new HitDiffPoint(i + startAddr, currHit, false));
                }
            }
            if (prevHit) {
                result.Add(new HitDiffPoint(end + 1 + startAddr, false, false));
            }
            return result;
        }

        //=========================================================================================
        // 機　能：検索にヒットした変化点を求める（自動検索を含む）
        // 引　数：[in]line   変化点を求める行
        // 戻り値：変化点（ヒットした場合はhit=trueで始まり、hit=endで終わる）
        //=========================================================================================
        public List<HitDiffPoint> GetHitPointListAuto(int line) {
            List<HitDiffPoint> result = new List<HitDiffPoint>();
            int startAddr = StartAddress;
            int start = line * m_lineBytes - startAddr;
            int end = Math.Min(EndAddress, (line + 1) * m_lineBytes - 1) - startAddr;
            bool prevHitSearch = false;
            bool prevHitAuto = false;
            for (int i = start; i <= end; i++) {
                bool currHitSearch = (m_hitFlag[i] == FLAG_HIT_SEARCH) || (m_hitFlag[i] == FLAG_HIT_BOTH);
                bool currHitAuto = (m_hitFlag[i] == FLAG_HIT_AUTO) || (m_hitFlag[i] == FLAG_HIT_BOTH);
                if (prevHitSearch != currHitSearch || prevHitAuto != currHitAuto) {
                    prevHitSearch = currHitSearch;
                    prevHitAuto = currHitAuto;
                    result.Add(new HitDiffPoint(i + startAddr, currHitSearch, currHitAuto));
                }
            }
            if (prevHitSearch || prevHitAuto) {
                result.Add(new HitDiffPoint(end + 1 + startAddr, false, false));
            }
            return result;
        }

        //=========================================================================================
        // プロパティ：表示の際の開始アドレス
        //=========================================================================================
        public int StartAddress {
            get {
                return m_startLine * m_lineBytes;
            }
        }

        //=========================================================================================
        // プロパティ：表示の際の終了アドレス（終了位置そのものでEndAddress自身も有効）
        //=========================================================================================
        public int EndAddress {
            get {
                int address;
                if ((m_fileSize - 1) / m_lineBytes == m_endLine) {
                    address = m_endLine * m_lineBytes + (m_fileSize - 1) % m_lineBytes + 1;
                } else {
                    address = (m_endLine + 1) * m_lineBytes;
                }
                return address;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルサイズ
        //=========================================================================================
        public int FileSize {
            get {
                return m_fileSize;
            }
        }

        //=========================================================================================
        // プロパティ：検索結果が確定状態のときtrue
        //=========================================================================================
        public bool Fixed {
            get {
                return m_hitFlag != null;
            }
        }

        //=========================================================================================
        // プロパティ：検索にヒットしたかどうかのフラグ
        //=========================================================================================
        public byte[] HitFlagBuffer {
            get {
                return m_hitFlag;
            }
        }

        //=========================================================================================
        // プロパティ：m_hitFlagの先頭位置のアドレス
        //=========================================================================================
        public int BufferStartAddress {
            get {
                return m_bufferStartAddress;
            }
        }
    }
}
