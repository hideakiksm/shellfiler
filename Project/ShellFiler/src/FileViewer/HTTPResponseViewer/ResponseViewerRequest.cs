using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileTask.DataObject;

namespace ShellFiler.FileViewer.HTTPResponseViewer {

    //=========================================================================================
    // クラス：レスポンスビューア起動のためのリクエスト
    //=========================================================================================
    public class ResponseViewerRequest {
        // リクエスト設定
        private ResponseViewerRequestSetting m_requestSetting;

        // リクエストボディ
        private byte[] m_requestBody;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ResponseViewerRequest() {
        }

        //=========================================================================================
        // プロパティ：リクエスト設定
        //=========================================================================================
        public ResponseViewerRequestSetting RequestSetting {
            get {
                return m_requestSetting;
            }
            set {
                m_requestSetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：リクエストボディ
        //=========================================================================================
        public byte[] RequestBody {
            get {
                return m_requestBody;
            }
            set {
                m_requestBody = value;
            }
        }
    }
}
