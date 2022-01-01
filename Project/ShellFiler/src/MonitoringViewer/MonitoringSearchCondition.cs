using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.MonitoringViewer;
using ShellFiler.Locale;

namespace ShellFiler.MonitoringViewer {

    //=========================================================================================
    // クラス：インクリメンタルサーチの条件
    //=========================================================================================
    public class MonitoringSearchCondition {
        // 検索キーワード
        private string m_keyword;

        // 先頭から検索するときtrue
        private bool m_searchOnTop;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]keyword      検索キーワード
        // 　　　　[in]searchOnTop  先頭から検索するときtrue
        // 戻り値：なし
        //=========================================================================================
        public MonitoringSearchCondition(string keyword, bool searchOnTop) {
            m_keyword = keyword;
            m_searchOnTop = searchOnTop;
        }

        //=========================================================================================
        // 機　能：同じ検索条件かどうかを調べる
        // 引　数：[in]other   比較対象
        // 戻り値：同じ検索条件のときtrue
        //=========================================================================================
        public bool SameCondition(MonitoringSearchCondition other) {
            if (other == null) {
                return false;
            }
            if (m_keyword != other.m_keyword) {
                return false;
            }
            if (m_searchOnTop != other.m_searchOnTop) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：検索キーワード
        //=========================================================================================
        public string Keyword {
            get {
                return m_keyword;
            }
        }

        //=========================================================================================
        // プロパティ：先頭から検索するときtrue
        //=========================================================================================
        public bool SearchOnTop {
            get {
                return m_searchOnTop;
            }
        }
    }
}
