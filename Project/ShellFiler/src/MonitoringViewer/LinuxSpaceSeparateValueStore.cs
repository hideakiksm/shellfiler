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
    // クラス：Linuxのスペース区切りの値を保存/解析するクラス
    //=========================================================================================
    public class LinuxSpaceSeparateValueStore : IRetrieveFileDataTarget {
        // 解析の種類
        private MonitoringViewerMode m_parseType;

        // エンコード方式
        private Encoding m_encoding;

        // ロード中のタスクのID
        private BackgroundTaskID m_loadingTaskId;

        // ロード完了後に通知するビュー
        private IMonitoringViewer m_viewer;

        // 読み込み時に保存中のデータ（読み込み完了後はnull）
        private MemoryStream m_memoryStream;

        // 読み込み状況のステータス
        private RetrieveDataLoadStatus m_loadingStatus = RetrieveDataLoadStatus.Loading;
        
        // m_loadingStatus=Failedのときエラー情報の文字列、それ以外はnull
        private string m_errorInfo = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parseType  解析の種類
        // 　　　　[in]encoding   エンコード方式
        // 戻り値：なし
        //=========================================================================================
        public LinuxSpaceSeparateValueStore(MonitoringViewerMode parseType, Encoding encoding) {
            m_parseType = parseType;
            m_encoding = encoding;
            m_memoryStream = new MemoryStream();
        }
        
        //=========================================================================================
        // 機　能：新しいデータを追加する
        // 引　数：[in]buffer  データの入ったバッファ
        // 　　　　[in]offset  buffer中のオフセット
        // 　　　　[in]length  データの長さ
        // 戻り値：なし
        // メ　モ：読み込みスレッドまたはその他外部の作業スレッドからの呼び出しを想定
        //=========================================================================================
        public void AddData(byte[] buffer, int offset, int length) {
            m_memoryStream.Write(buffer, offset, length);
        }

        //=========================================================================================
        // 機　能：データの追加が終わったことを通知する
        // 引　数：[in]status    読み込み状況のステータス
        // 　　　　[in]errorInfo status=Failedのときエラー情報の文字列、それ以外はnull
        // 戻り値：なし
        // メ　モ：読み込みスレッドまたはその他外部の作業スレッドからの呼び出しを想定
        //=========================================================================================
        public void AddCompleted(RetrieveDataLoadStatus status, string errorInfo) {
            m_loadingStatus = status;
            if (m_errorInfo == null && errorInfo != null) {
                m_errorInfo = errorInfo;
            }
        }
       
        //=========================================================================================
        // 機　能：受信したときのイベントを発行する
        // 引　数：[in]final   最後のイベント通知のときtrue
        // 戻り値：なし
        // メ　モ：読み込みスレッドからの呼び出しを想定
        //=========================================================================================
        public void FireEvent(bool final) {
            if (!final) {
                return;
            }

            byte[] data = null;
            m_memoryStream.Close();
            if (m_loadingStatus == RetrieveDataLoadStatus.CompletedAll || m_loadingStatus == RetrieveDataLoadStatus.CompletedPart) {
                data = m_memoryStream.ToArray();
            } else if (m_loadingStatus == RetrieveDataLoadStatus.Failed) {
                ;
            }
            m_memoryStream.Dispose();
            m_memoryStream = null;
            IMonitoringViewer view = m_viewer;      // マルチスレッド対応
            if (view != null) {
                BaseThread.InvokeProcedureByMainThread(new OnParseCompletedDelegate(OnParseCompletedUI), view, data, m_errorInfo);
            }            
        }

        delegate void OnParseCompletedDelegate(IMonitoringViewer view, byte[] data, string errorInfo);
        private void OnParseCompletedUI(IMonitoringViewer view, byte[] data, string errorInfo) {
            view.OnParseCompleted(data, m_encoding, errorInfo);
        }

        //=========================================================================================
        // プロパティ：ロード中のタスクのID
        //=========================================================================================
        public BackgroundTaskID LoadingTaskId {
            get {
                return m_loadingTaskId;
            }
            set {
                m_loadingTaskId = value;
            }
        }

        //=========================================================================================
        // プロパティ：ロード完了後に通知するビュー
        //=========================================================================================
        public IMonitoringViewer Viewer {
            set {
                m_viewer = value;
            }
        }
    }
}
