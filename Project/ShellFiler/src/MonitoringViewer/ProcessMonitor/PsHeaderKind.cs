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

namespace ShellFiler.MonitoringViewer.ProcessMonitor {

    //=========================================================================================
    // クラス：psコマンドのヘッダの種類
    //=========================================================================================
    public class PsHeaderKind {
        // カラム名から値へのMap
        private static Dictionary<string, PsHeaderKind> s_columnToValue = new Dictionary<string, PsHeaderKind>();
        
        public static readonly PsHeaderKind USER    = new PsHeaderKind("USER",    MatrixData.HeaderDataType.TypeString, 80, false, Resources.MonitorProcess_HeaderUser);             // 所有ユーザー
        public static readonly PsHeaderKind PID     = new PsHeaderKind("PID",     MatrixData.HeaderDataType.TypeInt,    80, true,  Resources.MonitorProcess_HeaderPid);              // プロセス番号
        public static readonly PsHeaderKind CPU     = new PsHeaderKind("%CPU",    MatrixData.HeaderDataType.TypeFloat,  80, true,  Resources.MonitorProcess_HeaderCpu);              // CPU占有率[%]
        public static readonly PsHeaderKind MEM     = new PsHeaderKind("%MEM",    MatrixData.HeaderDataType.TypeFloat,  80, true,  Resources.MonitorProcess_HeaderMem);              // 実メモリ占有率[%]
        public static readonly PsHeaderKind SIZE    = new PsHeaderKind("SIZE",    MatrixData.HeaderDataType.TypeInt,    80, true,  Resources.MonitorProcess_HeaderSize);             // 仮想メモリ[KB]
        public static readonly PsHeaderKind VSZ     = new PsHeaderKind("VSZ",     MatrixData.HeaderDataType.TypeInt,    80, true,  Resources.MonitorProcess_HeaderSize);             // 仮想メモリ[KB]
        public static readonly PsHeaderKind RSS     = new PsHeaderKind("RSS",     MatrixData.HeaderDataType.TypeInt,    80, true,  Resources.MonitorProcess_HeaderRss);              // 実メモリ[KB]
        public static readonly PsHeaderKind TTY     = new PsHeaderKind("TTY",     MatrixData.HeaderDataType.TypeString, 60, false, Resources.MonitorProcess_HeaderTty);              // 端末名
        public static readonly PsHeaderKind STAT    = new PsHeaderKind("STAT",    MatrixData.HeaderDataType.TypeString, 60, false, Resources.MonitorProcess_HeaderStat);             // 状態
        public static readonly PsHeaderKind N       = new PsHeaderKind("N",       MatrixData.HeaderDataType.TypeString, 60, false, Resources.MonitorProcess_HeaderN);                // nice値
        public static readonly PsHeaderKind START   = new PsHeaderKind("START",   MatrixData.HeaderDataType.TypeString, 80, false, Resources.MonitorProcess_HeaderStart);            // 開始時刻
        public static readonly PsHeaderKind TIME    = new PsHeaderKind("TIME",    MatrixData.HeaderDataType.TypeString, 80, false, Resources.MonitorProcess_HeaderTime);             // 総実行時間
        public static readonly PsHeaderKind COMMAND = new PsHeaderKind("COMMAND", MatrixData.HeaderDataType.TypeString, -2, false, Resources.MonitorProcess_HeaderCommand);          // 実行コマンド名
        public static readonly PsHeaderKind UNKNOWN = new PsHeaderKind("",        MatrixData.HeaderDataType.TypeString, 60, false, "");                                              // 不明

        // コマンドのカラムの内容
        private string m_columnValue;

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
        private PsHeaderKind(string columnValue, MatrixData.HeaderDataType dataType, int width, bool alignRight, string displayName) {
            m_columnValue = columnValue;
            m_dataType = dataType;
            m_width = width;
            m_alignRight = alignRight;
            m_displayName = displayName;
            s_columnToValue.Add(columnValue, this);
        }

        //=========================================================================================
        // 機　能：カラム値に対応する値を返す
        // 引　数：[in]columnValue   コマンドのカラムの内容
        // 戻り値：なし
        //=========================================================================================
        public static PsHeaderKind ColumnValueToValue(string columnValue) {
            if (s_columnToValue.ContainsKey(columnValue)) {
                return s_columnToValue[columnValue];
            } else {
                return UNKNOWN;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのカラムの内容
        //=========================================================================================
        public string ColumnValue {
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
    }
}
