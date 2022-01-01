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
using ShellFiler.UI.Dialog.MonitoringViewer;
using ShellFiler.Locale;

namespace ShellFiler.MonitoringViewer {

    //=========================================================================================
    // クラス：インクリメンタルサーチの実装
    //=========================================================================================
    public class IncrementalSearchLogic {
        // 親フォーム
        private MonitoringViewerForm m_parentForm;
        
        // 親ビュー
        private IMonitoringViewer m_parentViewer;

        // 検索ダイアログ（ダイアログを開いていないときnull）
        private MonitoringSearchDialog m_searchDialog = null;

        //=========================================================================================
        // 機　能：UIパネルを作成する
        // 引　数：[in]parentForm    親フォーム
        // 　　　　[in]parentViewer  親ビュー
        // 戻り値：なし
        //=========================================================================================
        public IncrementalSearchLogic(MonitoringViewerForm parentForm, IMonitoringViewer parentViewer) {
            m_parentForm = parentForm;
            m_parentViewer = parentViewer;
        }

        //=========================================================================================
        // 機　能：ビューアが閉じられるときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CloseViewer() {
            if (m_searchDialog != null) {
                m_searchDialog.Close();
                m_searchDialog = null;
            }
        }

        //=========================================================================================
        // 機　能：検索処理を開始する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OpenSearchDialog() {
            if (m_searchDialog == null) {
                m_searchDialog = new MonitoringSearchDialog(this);
                m_searchDialog.Show(m_parentForm);
            } else {
                m_searchDialog.Focus();
            }
        }

        //=========================================================================================
        // 機　能：検索ダイアログが閉じられたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseDialog() {
            m_searchDialog = null;
        }

        //=========================================================================================
        // 機　能：文字列を検索する
        // 引　数：[in]keyword    検索キーワード（nullのときクリア）
        // 　　　　[in]operation  検索に対する処理
        // 戻り値：なし
        //=========================================================================================
        public bool SearchString(MonitoringSearchCondition keyword, IncrementalSearchOperation operation) {
            MatrixData matrixData = m_parentViewer.MatrixData;
            if (keyword == null) {
                // クリア
                matrixData.SearchKeyword = null;
                for (int i = 0; i < matrixData.LineListSorted.Count; i++) {
                    matrixData.LineListSorted[i].ColumnHitFlag = null;
                }
                m_parentForm.RefreshView();
                return false;
            }

            // 検索を実行
            if (!keyword.SameCondition(matrixData.SearchKeyword)) {
                SearchKeywordFromMatrixData(matrixData, keyword);
                m_parentForm.RefreshView();
            }

            // 次のヒット位置へ移動
            int cursor = -1;
            switch (operation) {
                case IncrementalSearchOperation.FromTop:
                    for (int i = 0; i < matrixData.LineListSorted.Count; i++) {
                        if (matrixData.LineListSorted[i].ColumnHitFlag != null) {
                            cursor = i;
                            break;
                        }
                    }
                    break;
                case IncrementalSearchOperation.MoveDown:
                    cursor = m_parentViewer.MatrixDataView.CursorLine;
                    for (int i = cursor + 1; i < matrixData.LineListSorted.Count; i++) {
                        if (matrixData.LineListSorted[i].ColumnHitFlag != null) {
                            cursor = i;
                            break;
                        }
                    }
                    break;
                case IncrementalSearchOperation.MoveUp:
                    cursor = m_parentViewer.MatrixDataView.CursorLine;
                    for (int i = cursor - 1; i >= 0; i--) {
                        if (matrixData.LineListSorted[i].ColumnHitFlag != null) {
                            cursor = i;
                            break;
                        }
                    }
                    break;
            }

            // カーソルを補正
            if (cursor == -1) {
                return false;
            } else {
                m_parentViewer.MatrixDataView.CursorLine = cursor;
                if (m_searchDialog != null) {
                    CheckSearchDialogPosition();
                }
                return true;
            }
        }

        //=========================================================================================
        // 機　能：マトリクスデータから文字列を検索する
        // 引　数：[in]matrixData  検索対象のデータ
        // 　　　　[in]keyword     検索キーワード（nullのときクリア）
        // 戻り値：なし
        //=========================================================================================
        private void SearchKeywordFromMatrixData(MatrixData matrixData, MonitoringSearchCondition keyword) {
            string strKeyword = keyword.Keyword;
            bool searchTop = keyword.SearchOnTop;
            int hitLineCount = 0;
            int hitColumnCount = 0;
            for (int i = 0; i < matrixData.LineListSorted.Count; i++) {
                bool hitLine = false;
                List<string> valueList = matrixData.LineListSorted[i].ValueList;
                BitArray hitFlag = new BitArray(valueList.Count);
                for (int j = 0; j < valueList.Count; j++) {
                    bool hitColumn = false;
                    if (searchTop) {
                        if (valueList[j].StartsWith(strKeyword, StringComparison.OrdinalIgnoreCase)) {
                            hitColumn = true;
                            hitColumnCount++;
                        }
                    } else {
                        if (valueList[j].IndexOf(strKeyword, StringComparison.OrdinalIgnoreCase) != -1) {
                            hitColumn = true;
                            hitColumnCount++;
                        }
                    }
                    hitFlag[j] = hitColumn;
                    hitLine |= hitColumn;
                }
                if (hitLine) {
                    matrixData.LineListSorted[i].ColumnHitFlag = hitFlag;
                    hitLineCount++;
                } else {
                    matrixData.LineListSorted[i].ColumnHitFlag = null;
                }
            }
            matrixData.SearchKeyword = keyword;
            matrixData.SearchHitLineCount = hitLineCount;
            matrixData.SearchHitColumnCount = hitColumnCount;
        }

        //=========================================================================================
        // 機　能：検索ダイアログと検索ヒット位置が重ならないようにダイアログ位置を調整する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void CheckSearchDialogPosition() {
            Point ptOrg = new Point(m_searchDialog.Left, m_searchDialog.Top);

            Rectangle rcCursor = m_parentViewer.MatrixDataView.CursorPosition;
            Point ptCursorScr = m_parentViewer.MatrixDataView.PointToScreen(new Point(rcCursor.Left, rcCursor.Top));
            Rectangle rcCursorScr = new Rectangle(0, ptCursorScr.Y, 10, rcCursor.Height);
            Rectangle rcDialogScr = new Rectangle(0, m_searchDialog.Top, 10, m_searchDialog.Height);
            
            Point ptNewPos = ptOrg;
            if (rcCursorScr.IntersectsWith(rcDialogScr)) {
                // 下に移動してみる
                ptNewPos = MonitoringSearchDialog.GetDialogPos(true, m_searchDialog, m_parentForm);
                rcDialogScr = new Rectangle(0, ptNewPos.Y, 10, m_searchDialog.Height);
                if (rcCursorScr.IntersectsWith(rcDialogScr)) {
                    // 上に移動してみる
                    ptNewPos = MonitoringSearchDialog.GetDialogPos(false, m_searchDialog, m_parentForm);
                    rcDialogScr = new Rectangle(0, ptNewPos.Y, 10, m_searchDialog.Height);
                    if (rcCursorScr.IntersectsWith(rcDialogScr)) {
                        // 元の位置に戻す
                        ptNewPos = ptOrg;
                    }
                }
            }
            m_searchDialog.Left = ptNewPos.X;
            m_searchDialog.Top = ptNewPos.Y;
        }

        //=========================================================================================
        // プロパティ：親フォーム
        //=========================================================================================
        public MonitoringViewerForm MonitoringViewerForm {
            get {
                return m_parentForm;
            }
        }

        //=========================================================================================
        // プロパティ：親ビュー
        //=========================================================================================
        public IMonitoringViewer MonitoringViewer {
            get {
                return m_parentViewer;
            }
        }
    }
}
