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
    // プロパティ：一覧項目のソートクラス
    //=========================================================================================
    public class MatrixItemSorter : IComparer<MatrixData.ValueLine> {
        // ソート対象のカラムのインデックス（ソートされていないとき-1）
        int m_sortIndex = -1;

        // 正方向にソートするときtrue、逆方向のときfalse
        bool m_sortDirectionForward = true;

        // ヘッダの型のリスト
        public List<MatrixData.HeaderDataType> m_headerDataTypeList;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MatrixItemSorter() {
        }

        //=========================================================================================
        // 機　能：ソートモードを変更する
        // 引　数：[in]index   ソート対象のカラムのインデックス
        // 戻り値：なし
        //=========================================================================================
        public void ChangeSortMode(int index) {
            if (m_sortIndex == index) {
                m_sortDirectionForward = !m_sortDirectionForward;
            } else {
                m_sortDirectionForward = true;
            }
            m_sortIndex = index;
        }

        //=========================================================================================
        // 機　能：設定を引き継ぐ
        // 引　数：[in]original   引き継ぐ元のデータ
        // 戻り値：ソートを実行できるときtrue
        //=========================================================================================
        public bool InheritSetting(MatrixItemSorter original) {
            if (original.m_sortIndex == -1) {
                return false;
            }
            if (original.m_sortIndex >= m_headerDataTypeList.Count) {
                return false;
            }
            m_sortIndex = original.m_sortIndex;
            m_sortDirectionForward = original.m_sortDirectionForward;
            return true;
        }

        //=========================================================================================
        // 機　能：項目を比較する
        // 引　数：[in]item1  比較する項目1
        // 　　　　[in]item2  比較する項目2
        // 戻り値：比較結果
        //=========================================================================================
        public int Compare(MatrixData.ValueLine item1, MatrixData.ValueLine item2) {
            // 値を取得
            string value1 = "", value2 = "";
            if (m_sortIndex < item1.ValueList.Count) {
                value1 = item1.ValueList[m_sortIndex];
            }
            if (m_sortIndex < item2.ValueList.Count) {
                value2 = item2.ValueList[m_sortIndex];
            }

            // 比較
            int result = 0;
            MatrixData.HeaderDataType dataType = m_headerDataTypeList[m_sortIndex];
            switch (dataType) {
                case MatrixData.HeaderDataType.TypeString:
                    result = string.Compare(value1, value2);
                    break;
                case MatrixData.HeaderDataType.TypeInt:
                    int intValue1, intValue2;
                    if (int.TryParse(value1, out intValue1) && int.TryParse(value2, out intValue2)) {
                        result = intValue1 - intValue2;
                    } else {
                        result = string.Compare(value1, value2);
                    }
                    break;
                case MatrixData.HeaderDataType.TypeFloat:
                    float floatValue1, floatValue2;
                    if (float.TryParse(value1, out floatValue1) && float.TryParse(value2, out floatValue2)) {
                        if (floatValue1 < floatValue2) {
                            result = -1;
                        } else if (floatValue1 > floatValue2) {
                            result = 1;
                        } else {
                            result = 0;
                        }
                    } else {
                        result = string.Compare(value1, value2);
                    }
                    break;
            }

            // 逆方向への対応
            if (!m_sortDirectionForward) {
                result = -result;
            }

            return result;
        }

        //=========================================================================================
        // プロパティ：ヘッダのリスト
        //=========================================================================================
        public List<MatrixData.HeaderDataType> HeaderDataType {
            get {
                return m_headerDataTypeList;
            }
            set {
                m_headerDataTypeList = value;
            }
        }
    }
}
