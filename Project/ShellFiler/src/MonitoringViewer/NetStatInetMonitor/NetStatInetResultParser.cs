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
    // クラス：netstat inetコマンドの結果を解析するクラス
    //=========================================================================================
    public class NetStatInetResultPraser {
        // エンコード方式
        private Encoding m_encoding;

        // 解析中に発生したエラー
        private string m_errorInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]encoding   エンコード方式
        // 戻り値：なし
        //=========================================================================================
        public NetStatInetResultPraser(Encoding encoding) {
            m_encoding = encoding;
        }

        //=========================================================================================
        // 機　能：バイト列を解析する
        // 引　数：[in]data   解析対象のデータ
        // 戻り値：解析結果（エラーのときnull）
        // メ　モ：dataの形式は以下の通り
        // UID
        // ---   (PS_SEPARATOR)              ←これ以降、psList=true
        // 全ユーザーのプロセス一覧ヘッダ
        // 全ユーザーのプロセス一覧1行目
        // 全ユーザーのプロセス一覧2行目
        // …
        //=========================================================================================
        public MatrixData ParseData(byte[] data) {
            List<NetStatInetHeader> header = null;                                   // 解析したヘッダ
            List<MatrixData.ValueLine> dataBody = new List<MatrixData.ValueLine>();  // 解析した全ユーザーのプロセス一覧
            byte[][] dataLines = ArrayUtils.SeparateLinuxLine(data);
            for (int i = 0; i < dataLines.Length; i++) {
                // 文字列に変換
                string line;
                try {
                    line = m_encoding.GetString(dataLines[i]);
                } catch (Exception) {
                    m_errorInfo = Resources.MonitorProcess_UnknownChar;
                    return null;
                }
                line = line.Trim();
                if (line.Length == 0) {
                    continue;
                }

                // 解析
                if (header == null) {
                    // ヘッダ
                    header = ParseHeader(line);
                } else {
                    // 結果
                    MatrixData.ValueLine valueLine = ParseValueList(line, header);
                    if (valueLine == null) {
                        return null;
                    }
                    dataBody.Add(valueLine);
                }
            }
            if (header == null) {
                m_errorInfo = Resources.MonitorProcess_NoAvailableLine;
                return null;
            }

            // 結果を変換する
            List<MatrixData.HeaderKind> dataHeader = new List<MatrixData.HeaderKind>();
            for (int i = 0; i < header.Count; i++) {
                NetStatInetHeader headerItem = header[i];
                string name;
                if (headerItem.HeaderKind == NetStatInetHeaderKind.Unknown) {
                    name = headerItem.OriginalValue;
                } else {
                    name = headerItem.HeaderKind.DisplayName;
                }
                dataHeader.Add(new MatrixData.HeaderKind(headerItem.HeaderKind.DataType, headerItem.HeaderKind.Width, headerItem.HeaderKind.AlignRight, name));
            }
            SetIcon(header,dataBody);

            // 結果を格納
            MatrixData result = new MatrixData(data);
            result.Header = dataHeader;
            result.LineList = dataBody;

            return result;
        }

        //=========================================================================================
        // 機　能：ヘッダ行を解析する
        // 引　数：[in]line   解析対象の行
        // 戻り値：ヘッダ行の解析結果（エラーのときnull）
        //=========================================================================================
        private List<NetStatInetHeader> ParseHeader(string line) {
            // 行を空白区切りで分割
            string[] columnList = StringUtils.SeparateBySpace(line);
            if (columnList.Length < 3) {
                m_errorInfo = string.Format(Resources.MonitorNetStatInet_UnexpectedHeader, columnList.Length);
                return null;
            }

            // ヘッダの構成単語から識別子へのMapを作成
            // {{"Proto"}, Proto}, {{"Local", "Address"}, LocalAddr}, …
            List<NetStatInetHeaderKind> allHeader = NetStatInetHeaderKind.AllValue;
            Dictionary<string[], NetStatInetHeaderKind> wordToHeaderKind = new Dictionary<string[],NetStatInetHeaderKind>();
            for (int i = 0; i < allHeader.Count; i++) {
                string[] headerValueCandidate = allHeader[i].ColumnValue;
                for (int j = 0; j < headerValueCandidate.Length; j++) {
                    string[] headerValueWord = StringUtils.SeparateBySpace(headerValueCandidate[j].ToLower());
                    wordToHeaderKind.Add(headerValueWord, allHeader[i]);
                }
            }

            // 内容を照合
            int foundCount = 0;
            List<NetStatInetHeader> header = new List<NetStatInetHeader>();
            int index = 0;
            while (index < columnList.Length) {
                bool found = false;
                foreach (string[] wordList in wordToHeaderKind.Keys) {
                    if (HeaderColumnBeginWith(index, columnList, wordList)) {
                        found = true;
                        string columnName = StringUtils.AppendString(columnList, index, wordList.Length);
                        header.Add(new NetStatInetHeader(wordToHeaderKind[wordList], columnName));
                        index += wordList.Length;
                        foundCount++;
                        break;
                    }
                }
                if (!found) {
                    header.Add(new NetStatInetHeader(NetStatInetHeaderKind.Unknown, columnList[index]));
                    index++;
                }
            }
            if (foundCount < 2) {
                return null;
            }
            return header;
        }

        //=========================================================================================
        // 機　能：行が指定されたキーワードで始まっているかどうかを返す
        // 引　数：[in]start    lineの比較開始位置
        // 　　　　[in]line     解析対象の行
        // 　　　　[in]keyword  キーワード
        // 戻り値：指定されたキーワードで始まっているときtrue
        //=========================================================================================
        private bool HeaderColumnBeginWith(int start, string[] line, string[] keyword) {
            if (start + keyword.Length > line.Length) {
                return false;
            }
            for (int i = 0; i < keyword.Length; i++) {
                if (!keyword[i].Equals(line[i + start], StringComparison.OrdinalIgnoreCase)) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ヘッダ行以外を解析する
        // 引　数：[in]line    解析対象の行
        // 　　　　[in]header  ヘッダの解析結果
        // 戻り値：行の解析結果（エラーのときnull）
        //=========================================================================================
        private MatrixData.ValueLine ParseValueList(string line, List<NetStatInetHeader> header) {
            string[] columnList = StringUtils.SeparateBySpace(line, header.Count);
            List<string> result = ArrayUtils.ArrayToList<string>(columnList);
            for (int i = result.Count; i < header.Count; i++) {
                result.Add("");
            }
            return new MatrixData.ValueLine(result);
        }

        //=========================================================================================
        // 機　能：ヘッダから指定された型のはじめのカラムインデックスを返す
        // 引　数：[in]header   ヘッダのカラムのリスト
        // 　　　　[in]target   検索するヘッダ種別
        // 戻り値：見つかったはじめのカラムインデックス（見つからなかったとき-1）
        //=========================================================================================
        private int GetHeaderIndex(List<NetStatInetHeader> header, NetStatInetHeaderKind target) {
            for (int i = 0; i < header.Count; i++) {
                if (header[i].HeaderKind == target) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：全ユーザーの一覧に個人のマークを付ける
        // 引　数：[in]header   解析したヘッダの一覧
        // 　　　　[in]bodyAll  解析した接続の一覧
        // 戻り値：なし
        //=========================================================================================
        private void SetIcon(List<NetStatInetHeader> header, List<MatrixData.ValueLine> bodyAll) {
            // Protoのカラムを探す
            int indexProto = -1;
            for (int i = 0; i < header.Count; i++) {
                NetStatInetHeader headerItem = header[i];
                if (headerItem.HeaderKind == NetStatInetHeaderKind.Proto) {
                    indexProto = i;
                }
            }
            if (indexProto == -1) {
                return;
            }

            // Protoカラムを比較してアイコンを設定
            for (int i = 0; i < bodyAll.Count; i++) {
                if (bodyAll[i].ValueList.Count > indexProto) {
                    if (bodyAll[i].ValueList[indexProto].IndexOf("tcp", 0, StringComparison.OrdinalIgnoreCase) >= 0) {
                        bodyAll[i].IconIndex = (int)(IconImageListID.MonitoringViewer_NetTcp);
                    } else if (bodyAll[i].ValueList[indexProto].IndexOf("udp", 0, StringComparison.OrdinalIgnoreCase) >= 0) {
                        bodyAll[i].IconIndex = (int)(IconImageListID.MonitoringViewer_NetUdp);
                    }
                }
            }
        }

        //=========================================================================================
        // プロパティ：解析中に発生したエラー
        //=========================================================================================
        public string ErrorInfo {
            get {
                return m_errorInfo;
            }
        }
    }
}
