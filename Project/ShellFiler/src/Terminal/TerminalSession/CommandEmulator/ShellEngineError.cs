using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.UI;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {
    
    //=========================================================================================
    // クラス：エミュレータエンジンで解析したときのエラー
    //=========================================================================================
    public class ShellEngineError {
        // ステータスコード
        private FileOperationStatus m_status;

        // エラーメッセージ（正常修了時、表示しないときはnull）
        private string m_errorMessage;

        // エラー発生原因のデータ（正常修了時、表示しないときはnull）
        private string m_errorLine;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]status        ステータスコード
        // 　　　　[in]errorMessage  エラーメッセージ（正常修了時、表示しないときはnull）
        // 　　　　[in]errorLine     エラー発生原因のデータ（正常修了時、表示しないときはnull）
        // 戻り値：なし
        //=========================================================================================
        private ShellEngineError(FileOperationStatus status, string errorMessage, string errorLine) {
            m_status = status;
            m_errorMessage = errorMessage;
            m_errorLine = errorLine;
        }

        //=========================================================================================
        // 機　能：失敗時のエラーコードを生成する
        // 引　数：[in]statis        ステータスコード
        // 　　　　[in]errorMessage  エラーメッセージ（表示しないときnull）
        // 　　　　[in]errorLine     エラー発生原因のデータ（表示しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public static ShellEngineError Fail(FileOperationStatus status, string errorMessage, string errorLine) {
            return new ShellEngineError(status, errorMessage, errorLine);
        }

        //=========================================================================================
        // 機　能：成功時のエラーコードを生成する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static ShellEngineError Success() {
            return new ShellEngineError(FileOperationStatus.Success, null, null);
        }

        //=========================================================================================
        // プロパティ：エラーが発生したときtrue
        //=========================================================================================
        public bool IsFailed {
            get {
                return !m_status.Succeeded;
            }
        }

        //=========================================================================================
        // プロパティ：ステータスコード
        //=========================================================================================
        public FileOperationStatus ErrorCode {
            get {
                return m_status;
            }
        }

        //=========================================================================================
        // プロパティ：エラーメッセージ（正常修了時、表示しないときはnull）
        //=========================================================================================
        public string ErrorMessage {
            get {
                return m_errorMessage;
            }
        }

        //=========================================================================================
        // プロパティ：エラー発生原因のデータ（正常修了時、表示しないときはnull）
        //=========================================================================================
        public string ErrorLine {
            get {
                return m_errorLine;
            }
        }
    }
}