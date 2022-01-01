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
    // クラス：psコマンドの結果を解析するクラス
    //=========================================================================================
    public class PsResultPraser {
        // エンコード方式
        private Encoding m_encoding;

        // 解析中に発生したエラー
        private string m_errorInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]encoding   エンコード方式
        // 戻り値：なし
        //=========================================================================================
        public PsResultPraser(Encoding encoding) {
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
            int uid = -1;                                                            // ログイン中のユーザーのID
            List<PsHeader> header = null;                                            // 解析したヘッダ
            List<MatrixData.ValueLine> dataBody = new List<MatrixData.ValueLine>();  // 解析した全ユーザーのプロセス一覧
            byte[][] dataLines = ArrayUtils.SeparateLinuxLine(data);
            int startLine = 0;
            int startData = 0;
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
                if (uid == -1) {
                    bool success = int.TryParse(line, out uid);
                    if (!success) {
                        m_errorInfo = Resources.MonitorProcess_IdResult;
                        return null;
                    }
                } else if (header == null) {
                    // 先頭行
                    startData = startLine;
                    header = ParseHeader(line);
                    if (header == null) {
                        return null;
                    }
                } else {
                    // 全ユーザーの結果
                    MatrixData.ValueLine valueLine = ParseValueList(line, header);
                    if (valueLine == null) {
                        return null;
                    }
                    dataBody.Add(valueLine);
                }
                startLine += dataLines[i].Length + 1;
            }
            if (header == null) {
                m_errorInfo = Resources.MonitorProcess_NoAvailableLine;
                return null;
            }
            if (uid == -1) {
                m_errorInfo = Resources.MonitorProcess_IdResult;
                return null;
            }

            // 結果を変換する
            List<MatrixData.HeaderKind> dataHeader = new List<MatrixData.HeaderKind>();
            for (int i = 0; i < header.Count; i++) {
                PsHeader headerItem = header[i];
                string name;
                if (headerItem.HeaderKind == PsHeaderKind.UNKNOWN) {
                    name = headerItem.OriginalValue;
                } else {
                    name = headerItem.HeaderKind.DisplayName;
                }
                dataHeader.Add(new MatrixData.HeaderKind(headerItem.HeaderKind.DataType, headerItem.HeaderKind.Width, headerItem.HeaderKind.AlignRight, name));
            }
            int indexUid = GetHeaderIndex(header, PsHeaderKind.USER);
            if (indexUid == -1) {
                m_errorInfo = Resources.MonitorProcess_UidPidNotFound;
                return null;
            }
            SetPersonalMark(uid, indexUid, dataBody);

            // 結果を格納
            byte[] psData = new byte[data.Length - startData];
            Array.Copy(data, startData, psData, 0, psData.Length);
            MatrixData result = new MatrixData(psData);
            result.Header = dataHeader;
            result.LineList = dataBody;

            return result;
        }

        //=========================================================================================
        // 機　能：ヘッダ行を解析する
        // 引　数：[in]line   解析対象の行
        // 戻り値：ヘッダ行の解析結果（エラーのときnull）
        //=========================================================================================
        private List<PsHeader> ParseHeader(string line) {
            List<PsHeader> header = new List<PsHeader>();
            string[] columnList = StringUtils.SeparateBySpace(line);
            if (columnList.Length < 3) {
                m_errorInfo = string.Format(Resources.MonitorProcess_UnexpectedHeader, columnList.Length);
                return null;
            }
            for (int i = 0; i < columnList.Length; i++) {
                PsHeaderKind headerType = PsHeaderKind.ColumnValueToValue(columnList[i]);
                header.Add(new PsHeader(headerType, columnList[i]));
            }
            return header;
        }

        //=========================================================================================
        // 機　能：ヘッダ行以外を解析する
        // 引　数：[in]line    解析対象の行
        // 　　　　[in]header  ヘッダの解析結果
        // 戻り値：行の解析結果（エラーのときnull）
        //=========================================================================================
        private MatrixData.ValueLine ParseValueList(string line, List<PsHeader> header) {
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
        private int GetHeaderIndex(List<PsHeader> header, PsHeaderKind target) {
            for (int i = 0; i < header.Count; i++) {
                if (header[i].HeaderKind == target) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：全ユーザーの一覧に個人のマークを付ける
        // 引　数：[in]uid       ログインユーザーのユーザーID
        // 　　　　[in]indexUid  UIDの列インデックス
        // 　　　　[in]bodyAll   解析した全ユーザーのプロセス一覧
        // 戻り値：なし
        //=========================================================================================
        private void SetPersonalMark(int uid, int indexUid, List<MatrixData.ValueLine> bodyAll) {
            string strUid = uid.ToString();
            for (int i = 0; i < bodyAll.Count; i++) {
                bool isPersonal = false;
                if (bodyAll[i].ValueList.Count > indexUid) {
                    if (bodyAll[i].ValueList[indexUid] == strUid) {
                        isPersonal = true;
                    }
                }
                if (isPersonal) {
                    bodyAll[i].IconIndex = (int)IconImageListID.MonitoringViewer_PsMyProcess;
                } else {
                    bodyAll[i].IconIndex = (int)IconImageListID.MonitoringViewer_PsOtherProcess;
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
