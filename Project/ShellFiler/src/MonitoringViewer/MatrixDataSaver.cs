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
    // クラス：一覧コマンドの保存クラス
    //=========================================================================================
    public class MatrixDataSaver {
        // 保存対象のデータ
        private MatrixData m_matrixData;

        // 発生したエラー（エラーが発生していないときnull）
        private string m_errorMessage = null;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]matrixData   保存対象のデータ
        // 戻り値：なし
        //=========================================================================================
        public MatrixDataSaver(MatrixData matrixData) {
            m_matrixData = matrixData;
        }

        //=========================================================================================
        // 機　能：保存を実行する
        // 引　数：[in]fileName   ファイル名
        // 　　　　[in]format     保存形式
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public bool SaveFile(string fileName, MatrixFormatter.SaveFormat format) {
            try {
                if (format == MatrixFormatter.SaveFormat.Original) {
                    SaveOriginal(fileName);
                } else {
                    SaveAsFormat(fileName, format);
                }
                return true;
            } catch (Exception e) {
                m_errorMessage = string.Format(Resources.MonitorView_SaveFailed, e.Message);
                return false;
            }
        }
        
        //=========================================================================================
        // 機　能：オリジナル形式で保存を実行する
        // 引　数：[in]fileName   ファイル名
        // 戻り値：なし
        //=========================================================================================
        public void SaveOriginal(string fileName) {
            File.WriteAllBytes(fileName, m_matrixData.OriginalData);
        }
        
        //=========================================================================================
        // 機　能：オリジナル形式で保存を実行する
        // 引　数：[in]fileName   ファイル名
        // 　　　　[in]format     保存形式
        // 戻り値：なし
        //=========================================================================================
        public void SaveAsFormat(string fileName, MatrixFormatter.SaveFormat format) {
            MatrixFormatter formatter = new MatrixFormatter(m_matrixData);
            string formatData = formatter.Format(format);
            byte[] data = Encoding.UTF8.GetBytes(formatData);
            File.WriteAllBytes(fileName, data);
        }

        //=========================================================================================
        // プロパティ：発生したエラー（エラーが発生していないときnull）
        //=========================================================================================
        public string ErrorMessage {
            get {
                return m_errorMessage;
            }
        }
    }
}
