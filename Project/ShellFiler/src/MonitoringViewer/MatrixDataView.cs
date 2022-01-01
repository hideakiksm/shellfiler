using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.UI;
using ShellFiler.Util;

namespace ShellFiler.MonitoringViewer {

    //=========================================================================================
    // クラス：マトリックスのビュー
    //=========================================================================================
    public partial class MatrixDataView : UserControl {
        // フォーム
        private MonitoringViewerForm m_parentForm;

        // Viewerインターフェイス
        private IMonitoringViewer m_parentViewer;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]form    フォーム
        // 　　　　[in]viewer  Viewerインターフェイス
        // 戻り値：なし
        //=========================================================================================
        public MatrixDataView(MonitoringViewerForm form, IMonitoringViewer viewer) {
            InitializeComponent();
            m_parentForm = form;
            m_parentViewer = viewer;
        }

        //=========================================================================================
        // 機　能：ビューを初期化する
        // 引　数：[in]matrixData  実行結果（エラーのときnull）
        // 　　　　[in]cursor      カーソル行
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(MatrixData matrixData, int cursor) {
            if (matrixData != null) {
                // ヘッダを追加
                List<MatrixData.HeaderKind> headerList = matrixData.Header;
                List<DataGridViewColumn> columnList = new List<DataGridViewColumn>();
                ColumnHeader[] columnHeader = new ColumnHeader[headerList.Count];
                for (int i = 0; i < headerList.Count; i++) {
                    ColumnHeader column = new ColumnHeader();
                    columnHeader[i] = column;
                    column.Text = headerList[i].DisplayName;
                    if (headerList[i].Width < 0) {
                        column.Width = -2;
                    } else {
                        column.Width = headerList[i].Width;
                    }
                }

                // データを初期化
                this.listView.SmallImageList = UIIconManager.IconImageList;
                this.listView.Columns.Clear();
                this.listView.Columns.AddRange(columnHeader);
                this.listView.VirtualListSize = matrixData.LineList.Count;
                if (cursor < matrixData.LineList.Count) {
                    this.CursorLine = cursor;
                } else {
                    this.CursorLine = matrixData.LineList.Count - 1;
                }
            }
        }

        //=========================================================================================
        // 機　能：セルの値が必要になったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs evt) {
            int rowIndex = evt.ItemIndex;
            MatrixData.ValueLine line = m_parentViewer.MatrixData.LineListSorted[rowIndex];
            string[] itemStr = new string[line.ValueList.Count];
            for (int i = 0; i < itemStr.Length; i++) {
                itemStr[i] = line.ValueList[i];
            }
            ListViewItem item = new ListViewItem(itemStr);
            item.ImageIndex = line.IconIndex;
            evt.Item = item;
        }

        //=========================================================================================
        // 機　能：ヘッダの再描画が必要になったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs evt) {
            evt.DrawDefault = true;
        }

        //=========================================================================================
        // 機　能：リストの行の再描画が必要になったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listView_DrawItem(object sender, DrawListViewItemEventArgs evt) {
            MatrixDataViewGraphics g = new MatrixDataViewGraphics(evt.Graphics);
            try {
                if (evt.ItemIndex == this.listView.SelectedIndices[0]) {
                    evt.Graphics.FillRectangle(g.SelectBackBrush, evt.Bounds);
                }
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：リストのセルの再描画が必要になったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listView_DrawSubItem(object sender, DrawListViewSubItemEventArgs evt) {
            int rowIndex = evt.ItemIndex;
            int columnIndex = evt.ColumnIndex;
            string str = evt.Item.SubItems[columnIndex].Text;
            MatrixData matrixData = m_parentViewer.MatrixData;
            MatrixData.ValueLine line = matrixData.LineListSorted[rowIndex];

            MatrixDataViewGraphics g = new MatrixDataViewGraphics(evt.Graphics);
            try {
                // 領域を計算
                Rectangle rc;
                if (columnIndex == 0) {
                    rc = new Rectangle(evt.Bounds.Left + UIIconManager.CX_DEFAULT_ICON + 4, evt.Bounds.Top,
                                       evt.Bounds.Width - (UIIconManager.CX_DEFAULT_ICON + 4), evt.Bounds.Height);
                } else {
                    rc = evt.Bounds;
                }

                // 色を決定
                Brush textBrush = null;
                Brush backBrush = null;                 // nullのとき背景描画なし
                BitArray columnHit = line.ColumnHitFlag;
                if (columnHit != null) {
                    if (columnHit[columnIndex]) {       // ヒット中
                        backBrush = g.HitBackBrush;
                        textBrush = g.HitTextBrush;
                    }
                }
                if (evt.ItemIndex == this.listView.SelectedIndices[0]) {
                    if (textBrush == null) {
                        textBrush = g.SelectTextBrush;
                    }
                }
                if (textBrush == null) {
                    textBrush = g.TextBrush;
                }

                // 描画
                if (backBrush != null) {
                    evt.Graphics.FillRectangle(backBrush, rc);
                }
                evt.Graphics.DrawString(str, this.listView.Font, textBrush, rc, g.StringFormatEllipsis);

                // アイコン
                if (columnIndex == 0) {
                    Point pt = new Point(evt.Bounds.Left + 2, evt.Bounds.Top + evt.Bounds.Height - UIIconManager.CY_DEFAULT_ICON);
                    UIIconManager.IconImageList.Draw(evt.Graphics, pt, evt.Item.ImageIndex);
                }
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：カラムのヘッダがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listView_ColumnClick(object sender, ColumnClickEventArgs evt) {
            int index = evt.Column;
            int line = this.CursorLine;
            MatrixData.ValueLine lineObj = m_parentViewer.MatrixData.LineListSorted[line];
            m_parentViewer.MatrixData.ChangeSortMode(index);
            int newLine = m_parentViewer.MatrixData.LineListSorted.IndexOf(lineObj);
            this.listView.VirtualMode = false;
            this.listView.VirtualMode = true;
            this.CursorLine = newLine;
        }

        //=========================================================================================
        // 機　能：キーが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ProcessView_KeyDown(object sender, KeyEventArgs evt) {
        }

        //=========================================================================================
        // 機　能：キーが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ProcessView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs evt) {
            m_parentForm.OnKeyDown(new KeyCommand(evt));
        }

        //=========================================================================================
        // 機　能：キーが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ProcessView_KeyUp(object sender, KeyEventArgs evt) {
            m_parentForm.OnKeyUp(new KeyCommand(evt));
        }

        //=========================================================================================
        // 機　能：マウスがダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listView_DoubleClick(object sender, EventArgs evt) {
            m_parentForm.OnKeyDown(new KeyCommand(Keys.Enter, false, false, false));
        }

        //=========================================================================================
        // 機　能：表示内容を更新する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void RefreshView() {
            this.listView.Refresh();
        }

        //=========================================================================================
        // プロパティ：カーソル位置の行
        //=========================================================================================
        public int CursorLine {
            get {
                int rowIndex = this.listView.SelectedIndices[0];
                return rowIndex;
            }
            set {
                this.listView.EnsureVisible(value);
                this.listView.Items[value].Selected = true;
            }
        }

        //=========================================================================================
        // プロパティ：カーソル位置の領域
        //=========================================================================================
        public Rectangle CursorPosition {
            get {
                Rectangle rc = this.listView.GetItemRect(this.listView.SelectedIndices[0]);
                return rc;
            }
        }
    }
}
