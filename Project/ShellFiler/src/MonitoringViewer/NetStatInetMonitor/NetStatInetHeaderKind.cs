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
    // クラス：netstat inetコマンドのヘッダの種類
    //=========================================================================================
    public class NetStatInetHeaderKind {
        // カラム名から値へのMap
        private static Dictionary<string, NetStatInetHeaderKind> s_columnToValue = new Dictionary<string, NetStatInetHeaderKind>();
        
        // すべてのカラム名
        private static List<NetStatInetHeaderKind> s_allValue = new List<NetStatInetHeaderKind>();

        public static readonly NetStatInetHeaderKind Proto       = new NetStatInetHeaderKind(new string[] {"Proto"},                            MatrixData.HeaderDataType.TypeString,  100, false, Resources.MonitorNetInet_HeaderProto);         // プロトコル
        public static readonly NetStatInetHeaderKind RecvQ       = new NetStatInetHeaderKind(new string[] {"Recv-Q", "受信-Q"},                 MatrixData.HeaderDataType.TypeInt,     100, false, Resources.MonitorNetInet_HeaderRecvQ);         // 受信-Q
        public static readonly NetStatInetHeaderKind SendQ       = new NetStatInetHeaderKind(new string[] {"Send-Q", "送信-Q"},                 MatrixData.HeaderDataType.TypeInt,     100, false, Resources.MonitorNetInet_HeaderSendQ);         // 送信-Q
        public static readonly NetStatInetHeaderKind LocalAddr   = new NetStatInetHeaderKind(new string[] {"Local Address", "内部アドレス"},    MatrixData.HeaderDataType.TypeString,  200, false, Resources.MonitorNetInet_HeaderLocalAddr);     // 内部アドレス
        public static readonly NetStatInetHeaderKind ForeignAddr = new NetStatInetHeaderKind(new string[] {"Foreign Address", "外部アドレス"},  MatrixData.HeaderDataType.TypeString,  200, false, Resources.MonitorNetInet_HeaderForeignAddr);   // 外部アドレス
        public static readonly NetStatInetHeaderKind State       = new NetStatInetHeaderKind(new string[] {"State", "状態"},                    MatrixData.HeaderDataType.TypeString,  150, true,  Resources.MonitorNetInet_HeaderState);         // 状態
        public static readonly NetStatInetHeaderKind Unknown     = new NetStatInetHeaderKind(new string[] {},                                   MatrixData.HeaderDataType.TypeString,  150, false, "");                                           // 不明

        // コマンドのカラムの内容
        private string[] m_columnValue;

        // データ型
        private MatrixData.HeaderDataType m_dataType;

        // カラムの表示幅
        private int m_width;

        // 値を右寄せで表示するときtrue
        private bool m_alignRight;

        // カラムの表示名
        private string m_displayName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]columnValue   コマンドのカラムの内容
        // 　　　　[in]dataType      データ型
        // 　　　　[in]width         カラムの表示幅
        // 　　　　[in]alignRight    値を右寄せで表示するときtrue
        // 　　　　[in]displayName   カラムの表示名
        // 戻り値：なし
        //=========================================================================================
        private NetStatInetHeaderKind(string[] columnValue, MatrixData.HeaderDataType dataType, int width, bool alignRight, string displayName) {
            m_columnValue = columnValue;
            m_dataType = dataType;
            m_width = width;
            m_alignRight = alignRight;
            m_displayName = displayName;

            for (int i = 0; i < columnValue.Length; i++) {
                s_columnToValue.Add(columnValue[i], this);
            }
            s_allValue.Add(this);
        }

        //=========================================================================================
        // 機　能：カラム値に対応する値を返す
        // 引　数：[in]columnValue   コマンドのカラムの内容
        // 戻り値：なし
        //=========================================================================================
        public static NetStatInetHeaderKind ColumnValueToValue(string columnValue) {
            if (s_columnToValue.ContainsKey(columnValue)) {
                return s_columnToValue[columnValue];
            } else {
                return Unknown;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのカラムの内容
        //=========================================================================================
        public string[] ColumnValue {
            get {
                return m_columnValue;
            }
        }

        //=========================================================================================
        // プロパティ：データ型
        //=========================================================================================
        public MatrixData.HeaderDataType DataType {
            get {
                return m_dataType;
            }
        }

        //=========================================================================================
        // プロパティ：カラムの表示幅
        //=========================================================================================
        public int Width {
            get {
                return m_width;
            }
        }

        //=========================================================================================
        // プロパティ：値を右寄せで表示するときtrue
        //=========================================================================================
        public bool AlignRight {
            get {
                return m_alignRight;
            }
        }

        //=========================================================================================
        // プロパティ：カラムの表示名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
        }

        //=========================================================================================
        // プロパティ：すべてのカラム名
        //=========================================================================================
        public static List<NetStatInetHeaderKind> AllValue {
            get {
                return s_allValue;
            }
        }
    }
}
