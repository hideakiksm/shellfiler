using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.Virtual;

namespace ShellFiler.FileTask.ArgumentConverter {

    //=========================================================================================
    // クラス：ローカル実行に必要なファイルの種類
    //=========================================================================================
    public class LocalExecuteUseFile {
        public static readonly LocalExecuteUseFile None       = new LocalExecuteUseFile("none",       false, false);    // 不要
        public static readonly LocalExecuteUseFile Cursor     = new LocalExecuteUseFile("cursor",     true,  false);    // カーソル位置のファイル
        public static readonly LocalExecuteUseFile CursorMark = new LocalExecuteUseFile("cursormark", true,  true);     // カーソル位置とマークファイル
        public static readonly LocalExecuteUseFile All        = new LocalExecuteUseFile("all",        true,  true);     // すべて

        // 文字列表現
        private string m_strValue;

        // カーソル上のファイルを利用できるときtrue
        private bool m_useCursorFile;

        // マークファイルを利用できるときtrue
        private bool m_useMarkFile;

        //=========================================================================================
        // 機　能：文字列表現からインスタンスを得る
        // 引　数：[in]strValue   文字列表現
        // 　　　　[in]useCursor  カーソル上のファイルを利用できるときtrue
        // 　　　　[in]useMark    マークファイルを利用できるときtrue
        // 戻り値：なし
        //=========================================================================================
        private LocalExecuteUseFile(string strValue, bool useCursor, bool useMark) {
            m_strValue = strValue;
            m_useCursorFile = useCursor;
            m_useMarkFile = useMark;
        }

        //=========================================================================================
        // プロパティ：文字列表現
        //=========================================================================================
        public string StrValue {
            get {
                return m_strValue;
            }
        }

        //=========================================================================================
        // プロパティ：カーソル上のファイルを利用できるときtrue
        //=========================================================================================
        public bool CanUseTarget {
            get {
                return m_useCursorFile;
            }
        }

        //=========================================================================================
        // プロパティ：マークファイルを利用できるときtrue
        //=========================================================================================
        public bool CanUseMark {
            get {
                return m_useMarkFile;
            }
        }

        //=========================================================================================
        // 機　能：文字列表現からインスタンスを得る
        // 引　数：[in]strValue    文字列表現
        // 　　　　[in]enableNone  noneが有効なときtrue
        // 戻り値：なし
        //=========================================================================================
        public static LocalExecuteUseFile GetFromString(string strValue, bool enableNone) {
            if (strValue == "none" && enableNone) {
                return None;
            } else if (strValue == "cursor") {
                return Cursor;
            } else if (strValue == "cursormark") {
                return CursorMark;
            } else if (strValue == "all") {
                return All;
            } else {
                return All;
            }
        }
    }
}
