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
using ShellFiler.Locale;

namespace ShellFiler.MonitoringViewer {

    //=========================================================================================
    // クラス：一覧コマンドの結果
    //=========================================================================================
    public class MatrixData {
        // 元のデータ
        private byte[] m_originalData;

        // ヘッダのリスト
        private List<HeaderKind> m_headerList;
        
        // スペース区切りの行のリスト
        private List<ValueLine> m_lineList;
        
        // ソート済みスペース区切りの行のリスト
        private List<ValueLine> m_lineListSorted;

        // ヘッダのソータ
        private MatrixItemSorter m_itemSorter;

        // 検索キーワード（検索するまではnull）
        private MonitoringSearchCondition m_searchKeyword = null;

        // 検索にヒットした行数（検索キーワードが有効な場合のみ）
        private int m_searchHitLineCount;
        
        // 検索にヒットした項目数（検索キーワードが有効な場合のみ）
        private int m_searchHitColumnCount;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]originalData   元のデータ
        // 戻り値：なし
        //=========================================================================================
        public MatrixData(byte[] originalData) {
            m_itemSorter = new MatrixItemSorter();
            m_originalData = originalData;
        }

        //=========================================================================================
        // 機　能：設定を引き継ぐ
        // 引　数：[in]original   引き継ぐ元のデータ
        // 戻り値：なし
        //=========================================================================================
        public void InheritSetting(MatrixData original) {
            bool doSort = m_itemSorter.InheritSetting(original.m_itemSorter);
            if (doSort) {
                m_lineListSorted.Sort(m_itemSorter);
            }
        }

        //=========================================================================================
        // 機　能：ソートモードを変更する
        // 引　数：[in]index   ソート対象のカラムのインデックス
        // 戻り値：なし
        //=========================================================================================
        public void ChangeSortMode(int index) {
            m_itemSorter.ChangeSortMode(index);
            m_lineListSorted.Sort(m_itemSorter);
        }

        //=========================================================================================
        // プロパティ：元のデータ
        //=========================================================================================
        public byte[] OriginalData {
            get {
                return m_originalData;
            }
        }

        //=========================================================================================
        // プロパティ：ヘッダのリスト
        //=========================================================================================
        public List<HeaderKind> Header {
            get {
                return m_headerList;
            }
            set {
                m_headerList = value;
                List<HeaderDataType> dataTypeList = new List<HeaderDataType>();
                for (int i = 0; i < m_headerList.Count; i++) {
                    dataTypeList.Add(m_headerList[i].DataType);
                }
                m_itemSorter.HeaderDataType = dataTypeList;
            }
        }
        
        //=========================================================================================
        // プロパティ：スペース区切りの行のリスト
        //=========================================================================================
        public List<ValueLine> LineList {
            get {
                return m_lineList;
            }
            set {
                m_lineList = value;
                m_lineListSorted = new List<ValueLine>();
                m_lineListSorted.AddRange(m_lineList);
            }
        }

        //=========================================================================================
        // プロパティ：ソート済みスペース区切りの行のリスト
        //=========================================================================================
        public List<ValueLine> LineListSorted {
            get {
                return m_lineListSorted;
            }
        }

        //=========================================================================================
        // プロパティ：検索キーワード（検索するまではnull）
        //=========================================================================================
        public MonitoringSearchCondition SearchKeyword {
            get {
                return m_searchKeyword;
            }
            set {
                m_searchKeyword = value;
            }
        }

        //=========================================================================================
        // プロパティ：検索にヒットした行数（検索キーワードが有効な場合のみ）
        //=========================================================================================
        public int SearchHitLineCount {
            get {
                return m_searchHitLineCount;
            }
            set {
                m_searchHitLineCount = value;
            }
        }

        //=========================================================================================
        // プロパティ：検索にヒットした項目数（検索キーワードが有効な場合のみ）
        //=========================================================================================
        public int SearchHitColumnCount {
            get {
                return m_searchHitColumnCount;
            }
            set {
                m_searchHitColumnCount = value;
            }
        }

        //=========================================================================================
        // クラス：解析済みの行1行分の情報
        //=========================================================================================
        public class ValueLine {
            // 行内のカラム
            private List<string> m_valueList;

            // アイコンのインデックス
            private int m_iconIndex = 0;

            // キーワード検索にヒットしたカラムがtrueとなる配列（ヒットしていないときnull）
            private BitArray m_columnHitFlag = null;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]valueList  行内のカラム
            // 戻り値：なし
            //=========================================================================================
            public ValueLine(List<string> valueList) {
                m_valueList = valueList;
            }
        
            //=========================================================================================
            // プロパティ：スペース区切りの行のリスト
            //=========================================================================================
            public List<string> ValueList {
                get {
                    return m_valueList;
                }
            }

            //=========================================================================================
            // プロパティ：個人が所有するプロセスのときtrue
            //=========================================================================================
            public int IconIndex {
                get {
                    return m_iconIndex;
                }
                set {
                    m_iconIndex = value;
                }
            }

            //=========================================================================================
            // プロパティ：キーワード検索にヒットしたカラムがtrueとなる配列（ヒットしていないときnull）
            //=========================================================================================
            public BitArray ColumnHitFlag {
                get {
                    return m_columnHitFlag;
                }
                set {
                    m_columnHitFlag = value;
                }
            }
        }

        //=========================================================================================
        // クラス：データ型
        //=========================================================================================
        public enum HeaderDataType {
            TypeString,             // 文字列
            TypeInt,                // 数値
            TypeFloat,              // 実数
        }

        //=========================================================================================
        // クラス：ヘッダの種類
        //=========================================================================================
        public class HeaderKind {
            // データ型
            private HeaderDataType m_dataType;

            // カラムの表示幅
            private int m_width;

            // 値を右寄せで表示するときtrue
            private bool m_alignRight;

            // カラムの表示名
            private string m_displayName;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]dataType      データ型
            // 　　　　[in]width         カラムの表示幅
            // 　　　　[in]alignRight    値を右寄せで表示するときtrue
            // 　　　　[in]displayName   カラムの表示名
            // 戻り値：なし
            //=========================================================================================
            public HeaderKind(HeaderDataType dataType, int width, bool alignRight, string displayName) {
                m_dataType = dataType;
                m_width = width;
                m_alignRight = alignRight;
                m_displayName = displayName;
            }

            //=========================================================================================
            // プロパティ：データ型
            //=========================================================================================
            public HeaderDataType DataType {
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
}
