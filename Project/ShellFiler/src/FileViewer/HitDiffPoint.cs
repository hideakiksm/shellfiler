using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command.FileViewer;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：検索ヒットした位置の選択箇所の変化点
    //=========================================================================================
    public class HitDiffPoint {
        // 状態が変わる位置
        private int m_position;

        // 検索キーワードがヒット状態のときtrue
        private bool m_hitSearch;

        // 自動検索キーワードがヒット状態のときtrue
        private bool m_hitAutoSearch;

        //=========================================================================================
        // 機　能：マウスホイールイベントを処理する
        // 引　数：[in]position      状態が変わる位置
        // 　　　　[in]hitSearch     検索キーワードがヒット状態のときtrue
        // 　　　　[in]hitAutoSearch 自動検索キーワードがヒット状態のときtrue
        // 戻り値：なし
        //=========================================================================================
        public HitDiffPoint(int position, bool hitSearch, bool hitAutoSearch) {
            m_position = position;
            m_hitSearch = hitSearch;
            m_hitAutoSearch = hitAutoSearch;
        }

        //=========================================================================================
        // プロパティ：状態が変わる位置
        //=========================================================================================
        public int Position {
            get {
                return m_position;
            }
        }

        //=========================================================================================
        // プロパティ：検索キーワードがヒット状態のときtrue
        //=========================================================================================
        public bool HitSearch {
            get {
                return m_hitSearch;
            }
        }

        //=========================================================================================
        // プロパティ：自動検索キーワードがヒット状態のときtrue
        //=========================================================================================
        public bool HitAutoSearch {
            get {
                return m_hitAutoSearch;
            }
        }
    }
}
