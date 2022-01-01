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
    // クラス：一覧コマンドの整形クラス
    //=========================================================================================
    public class MatrixFormatter {
        // 保存対象のデータ
        private MatrixData m_matrixData;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]matrixData   保存対象のデータ
        // 戻り値：なし
        //=========================================================================================
        public MatrixFormatter(MatrixData matrixData) {
            m_matrixData = matrixData;
        }

        //=========================================================================================
        // 機　能：整形を実行する
        // 引　数：[in]format     保存形式（Original以外）
        // 戻り値：整形した文字列
        //=========================================================================================
        public string Format(SaveFormat format) {
            // 出力条件を決定
            List<MatrixData.ValueLine> lineList = m_matrixData.LineListSorted;
            StringBuilder sb = new StringBuilder();
            char separator;
            bool csvConvert;
            if (format == SaveFormat.Csv) {
                separator = ',';
                csvConvert = true;
            } else {
                separator = '\t';
                csvConvert = false;
            }

            // ヘッダを出力
            List<MatrixData.HeaderKind> header = m_matrixData.Header;
            for (int i = 0; i < header.Count; i++) {
                if (i != 0) {
                    sb.Append(separator);
                }
                if (csvConvert) {
                    string csvValue = header[i].DisplayName;
                    sb.Append(csvValue);
                } else {
                    sb.Append(header[i].DisplayName);
                }
            }
            sb.Append("\r\n");

            // 本体を出力
            for (int i = 0; i < lineList.Count; i++) {
                List<string> columnList = lineList[i].ValueList;
                for (int j = 0; j < columnList.Count; j++) {
                    if (j != 0) {
                        sb.Append(separator);
                    }
                    if (csvConvert) {
                        string csvValue = ConvertCsvColumn(columnList[j]);
                        sb.Append(csvValue);
                    } else {
                        sb.Append(columnList[j]);
                    }
                }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：CSVのカラムを出力形式に変換する
        // 引　数：[in]data  変換する値
        // 戻り値：出力形式
        //=========================================================================================
        private string ConvertCsvColumn(string data) {
            StringBuilder sb = new StringBuilder();
            sb.Append('\"');
            for (int i = 0; i < data.Length; i++) {
                char ch = data[i];
                if (ch == '\"') {
                    sb.Append("\"\"");
                } else {
                    sb.Append(ch);
                }
            }
            sb.Append('\"');
            return sb.ToString();
        }

        //=========================================================================================
        // 列挙子：保存する形式
        //=========================================================================================
        public enum SaveFormat {
            Original,           // 元の形式
            Tsv,                // タブ区切りテキスト
            Csv,                // CSV
        }
    }
}
