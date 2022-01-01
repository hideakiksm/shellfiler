using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Command.MonitoringViewer;
using ShellFiler.MonitoringViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.MonitoringViewer;
using ShellFiler.Locale;

namespace ShellFiler.MonitoringViewer.ProcessMonitor {

    //=========================================================================================
    // クラス：モニタリングビューアのコンポーネント本体
    //=========================================================================================
    public class ProcessMonitoringViewer : IMonitoringViewer {
        // 親となるフォーム（初期化するまではnull）
        private MonitoringViewerForm m_parent = null;

        // 結果を表示するビュー（初期化するまではnull）
        private MatrixDataView m_dataMatrixView = null;

        // プロセス一覧の解析結果取得クラス（読み込み中以外はnull）
        private LinuxSpaceSeparateValueStore m_dataStore;

        // 処理対象のデータ
        private MatrixData m_matrixData;

        // コマンドの再試行情報
        private CommandRetryInfo m_retryInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dataStore   プロセス一覧の解析結果
        // 　　　　[in]retryInfo   コマンドの再試行情報
        // 戻り値：なし
        //=========================================================================================
        public ProcessMonitoringViewer(LinuxSpaceSeparateValueStore dataStore, CommandRetryInfo retryInfo) {
            m_dataStore = dataStore;
            m_retryInfo = retryInfo;
        }

        //=========================================================================================
        // 機　能：再読込を行う
        // 引　数：[in]dataStore   プロセス一覧の解析結果
        // 戻り値：なし
        //=========================================================================================
        public void Reload(LinuxSpaceSeparateValueStore dataStore) {
            m_dataStore = dataStore;
            m_parent.StatusBar.RefreshStatusBar();
        }

        //=========================================================================================
        // 機　能：UIパネルを作成する
        // 引　数：[in]form   フォーム
        // 戻り値：作成したパネル
        //=========================================================================================
        public UserControl CreateMonitorPanel(MonitoringViewerForm form) {
            m_parent = form;
            m_dataMatrixView = new MatrixDataView(form, this);
            return m_dataMatrixView;
        }

        //=========================================================================================
        // 機　能：フォームが閉じられるときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFormClosed() {
            if (m_dataStore != null) {
                m_dataStore.Viewer = null;      // この後の通知拒否
            }
        }

        //=========================================================================================
        // 機　能：表示する値の読み込みが終わったときの処理を行う
        // 引　数：[in]data       受信結果（エラーのときnull）
        // 　　　　[in]encoding   dataのエンコード方式
        // 　　　　[in]errorInfo  エラー情報（成功のときnull）
        // 戻り値：なし
        //=========================================================================================
        public void OnParseCompleted(byte[] data, Encoding encoding, string errorInfo) {
            // 解析
            MatrixData matrixData = null;
            if (data != null) {
                PsResultPraser parser = new PsResultPraser(encoding);
                matrixData = parser.ParseData(data);
                if (m_matrixData == null) {
                    errorInfo = parser.ErrorInfo;
                }
            }

            // データを差し替え
            if (matrixData != null) {
                int cursor = 0;
                if (m_matrixData != null) {
                    cursor = m_dataMatrixView.CursorLine;
                    matrixData.InheritSetting(m_matrixData);
                }
                m_matrixData = matrixData;
                m_dataMatrixView.Initialize(m_matrixData, cursor);
            } else {
                m_parent.StatusBar.ShowErrorMessage(errorInfo, FileOperationStatus.LogLevel.Error, IconImageListID.None);
            }
            m_parent.StatusBar.RefreshStatusBar();
        }

        //=========================================================================================
        // プロパティ：モニタリングビューアの動作モード
        //=========================================================================================
        public MonitoringViewerMode Mode {
            get {
                return MonitoringViewerMode.PS;
            }
        }

        //=========================================================================================
        // プロパティ：モニタリングビューアのフォーム
        //=========================================================================================
        public MonitoringViewerForm MonitoringViewerForm {
            get {
                return m_parent;
            }
        }

        //=========================================================================================
        // プロパティ：ウィンドウのタイトル
        //=========================================================================================
        public string Title {
            get {
                return Resources.MonitorView_TitleProcess;
            }
        }

        //=========================================================================================
        // プロパティ：ビューが利用可能なときtrue
        //=========================================================================================
        public bool Available {
            get {
                return (m_matrixData != null);
            }
        }

        //=========================================================================================
        // プロパティ：処理対象のデータ
        //=========================================================================================
        public MatrixData MatrixData {
            get {
                return m_matrixData;
            }
        }

        //=========================================================================================
        // プロパティ：結果を表示するビュー（初期化するまではnull）
        //=========================================================================================
        public MatrixDataView MatrixDataView {
            get {
                return m_dataMatrixView;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドの再試行情報
        //=========================================================================================
        public CommandRetryInfo RetryInfo {
            get {
                return m_retryInfo;
            }
        }
    }
}
