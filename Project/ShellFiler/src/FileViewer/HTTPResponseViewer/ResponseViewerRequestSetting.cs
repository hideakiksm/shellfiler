using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileTask.DataObject;

namespace ShellFiler.FileViewer.HTTPResponseViewer {

    //=========================================================================================
    // クラス：レスポンスビューア起動のためのリクエスト設定
    //=========================================================================================
    public class ResponseViewerRequestSetting : ICloneable {
        // 選択されたモード
        private ResponseViewerMode m_selectedMode;

        // HTTPモードのパラメータ
        private ResponseViewerHttpRequest m_httpModeRequest;

        // TCPモードのパラメータ
        private ResponseViewerTcpRequest m_tcpModeRequest;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ResponseViewerRequestSetting() {
        }

        //=========================================================================================
        // 機　能：デフォルト値を作成して返す
        // 引　数：なし
        // 戻り値：デフォルト値
        //=========================================================================================
        public static ResponseViewerRequestSetting GetDefaultValue() {
            ResponseViewerRequestSetting request = new ResponseViewerRequestSetting();
            request.m_selectedMode = ResponseViewerMode.HttpMode;
            request.m_httpModeRequest = ResponseViewerHttpRequest.GetDefaultValue();
            request.m_tcpModeRequest = ResponseViewerTcpRequest.GetDefaultValue();
            return request;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            ResponseViewerRequestSetting obj = new ResponseViewerRequestSetting();
            obj.m_selectedMode = m_selectedMode;
            if (m_httpModeRequest != null) {
                obj.m_httpModeRequest = (ResponseViewerHttpRequest)(m_httpModeRequest.Clone());
            } else {
                obj.m_httpModeRequest = null;
            }
            if (m_tcpModeRequest != null) {
                obj.m_tcpModeRequest = (ResponseViewerTcpRequest)(m_tcpModeRequest.Clone());
            } else {
                obj.m_tcpModeRequest = null;
            }

            return obj;
        }

        //=========================================================================================
        // プロパティ：選択されたモード
        //=========================================================================================
        public ResponseViewerMode SelectedMode {
            get {
                return m_selectedMode;
            }
            set {
                m_selectedMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：HTTPモードのパラメータ
        //=========================================================================================
        public ResponseViewerHttpRequest HttpModeRequest {
            get {
                return m_httpModeRequest;
            }
            set {
                m_httpModeRequest = value;
            }
        }

        //=========================================================================================
        // プロパティ：TCPモードのパラメータ
        //=========================================================================================
        public ResponseViewerTcpRequest TcpModeRequest {
            get {
                return m_tcpModeRequest;
            }
            set {
                m_tcpModeRequest = value;
            }
        }
    }
}
