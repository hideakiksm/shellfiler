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
using ShellFiler.MonitoringViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.MonitoringViewer.NetStatInetMonitor {

    //=========================================================================================
    // クラス：netstat inetコマンドのヘッダの値
    //=========================================================================================
    public class NetStatInetHeader {
        // ヘッダの種類
        private NetStatInetHeaderKind m_headerKind;
        
        // ヘッダの元の値
        private string m_originalValue;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]columnValue   コマンドのカラムの内容
        // 　　　　[in]dataType      データ型
        // 　　　　[in]width         カラムの表示幅
        // 　　　　[in]alignRight    値を右寄せで表示するときtrue
        // 　　　　[in]displayName   カラムの表示名
        // 戻り値：なし
        //=========================================================================================
        public NetStatInetHeader(NetStatInetHeaderKind headerKind, string originalValue) {
            m_headerKind = headerKind;
            m_originalValue = originalValue;
        }

        //=========================================================================================
        // プロパティ：ヘッダの種類
        //=========================================================================================
        public NetStatInetHeaderKind HeaderKind {
            get {
                return m_headerKind;
            }
        }

        //=========================================================================================
        // プロパティ：ヘッダの元の値
        //=========================================================================================
        public string OriginalValue {
            get {
                return m_originalValue;
            }
        }
    }
}
