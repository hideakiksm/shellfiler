using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ファイルビューアでのステータスバー
    //=========================================================================================
    public partial class FileViewerStatusBar : StatusStrip {
        // ファイルビューアのパネル
        private TextFileViewer m_textFileViewer;

        // ファイル名領域
        private ToolStripStatusLabel m_fileLabel;

        // 処理時間領域
        private ToolStripStatusLabel m_timeLabel;

        // 選択中情報領域
        private ToolStripStatusLabel m_selectLabel;

        // 検索ヒット情報領域１
        private ToolStripStatusLabel m_searchLabel1;

        // 検索ヒット情報領域２
        private ToolStripStatusLabel m_searchLabel2;

        // タブ幅領域
        private ToolStripStatusLabel m_tabLabel;

        // エンコーディング領域
        private ToolStripStatusLabel m_encodingLabel;

        // 行番号領域
        private ToolStripStatusLabel m_lineNoLabel;

        // テキストでの選択状態のキャッシュ
        private TextSelectionInfoCache m_textSelectionInfoCache = new TextSelectionInfoCache();

        // エラーメッセージ表示の実装
        private StatusBarErrorMessageImpl m_errorMessageImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileViewerStatusBar() {
            InitializeComponent();

            this.SuspendLayout();
            
            m_fileLabel = CreateLabel(118);
            m_fileLabel.Spring = true;
            m_timeLabel = CreateLabel(0);
            m_timeLabel.AutoSize = true;
            m_selectLabel = CreateLabel(50);
            m_selectLabel.AutoSize = true;
            m_searchLabel1 = CreateLabel(50);
            m_searchLabel1.AutoSize = true;
            m_searchLabel1.ForeColor = Configuration.Current.TextViewerSearchHitBackColor;
            m_searchLabel2 = CreateLabel(50);
            m_searchLabel2.AutoSize = true;
            m_searchLabel2.ForeColor = Configuration.Current.TextViewerSearchAutoTextColor;
            m_tabLabel = CreateLabel(40);
            m_tabLabel.Click += new EventHandler(TabLabel_Click);
            m_encodingLabel = CreateLabel(60);
            m_encodingLabel.Click += new EventHandler(EncodingLabel_Click);
            m_lineNoLabel = CreateLabel(110);
            m_lineNoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            m_lineNoLabel.Margin = new System.Windows.Forms.Padding(10, 3, 0, 0);
            m_lineNoLabel.AutoSize = true;
            m_lineNoLabel.Click += new EventHandler(LineNoLabel_Click);

            this.ImageList = UIIconManager.IconImageList;
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {m_fileLabel, m_timeLabel, m_selectLabel, m_searchLabel1, m_searchLabel2, m_tabLabel, m_encodingLabel, m_lineNoLabel});

            m_errorMessageImpl = new StatusBarErrorMessageImpl(this, this.components, m_fileLabel, new StatusBarErrorMessageImpl.RefreshStatusBarDelegate(this.RefreshStatusBar));

            this.ResumeLayout(false);
        }

        //=========================================================================================
        // 機　能：ステータスバーのラベルを作成する
        // 引　数：[in]cx  幅の初期値
        // 戻り値：作成したラベル
        //=========================================================================================
        private ToolStripStatusLabel CreateLabel(int cx) {
            ToolStripStatusLabel label = new ToolStripStatusLabel();
            label.Margin = new System.Windows.Forms.Padding(10, 3, 0, 0);
            label.AutoSize = false;
            label.Size = new System.Drawing.Size(cx, 19);
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label.Text = "";
            return label;
        }
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]viewer  ファイルビューアのパネル
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(TextFileViewer viewer) {
            m_textFileViewer = viewer;
            RefreshStatusBar();
            RefreshLineNo();
        }

        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisposeStatusBar() {
            m_errorMessageImpl.Dispose();
        }

        //=========================================================================================
        // 機　能：ステータスバーを更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshStatusBar() {
            bool done = m_errorMessageImpl.RefreshStatusBar();
            if (done) {
                return;
            }

            // ファイル名
            if (m_textFileViewer.TextBufferLineInfo.TargetFile.Status == RetrieveDataLoadStatus.Loading) {
                m_fileLabel.Text = Resources.FileViewer_Loading;
            } else {
                string filePath;
                if (m_textFileViewer.TextBufferLineInfo.TargetFile.DisplayName == null) {
                    filePath = m_textFileViewer.TextBufferLineInfo.TargetFile.FilePath;
                } else {
                    filePath = m_textFileViewer.TextBufferLineInfo.TargetFile.DisplayName;
                }
                m_fileLabel.Text = filePath;
            }

            // 実行時間
            int responseTime = m_textFileViewer.FileViewerForm.CurrentFile.LoadingResponseTime;
            int turnAroundTime = m_textFileViewer.FileViewerForm.CurrentFile.LoadingTurnAroundTime;
            if (responseTime >= 0 && turnAroundTime >= 0) {
                m_timeLabel.Text = string.Format(Resources.FileViewer_LoadingTimeLabel, responseTime / 1000.0, turnAroundTime / 1000.0);
                m_timeLabel.ToolTipText = string.Format(Resources.FileViewer_LoadingTimeHint, responseTime / 1000.0, turnAroundTime / 1000.0);
                this.ShowItemToolTips = true;
                m_timeLabel.Size = new System.Drawing.Size(150, 19);
            } else {
                m_timeLabel.Text = "";
                m_timeLabel.ToolTipText = "";
                m_timeLabel.Size = new Size(0, 19);
            }
        }
        
        //=========================================================================================
        // 機　能：選択情報を更新する
        // 引　数：[in]lineInfoList  行情報
        // 　　　　[in]textSelect    テキストビューアでの選択情報（選択解除のときnull）
        // 　　　　[in]dumpSelect    ダンプビューアでの選択情報（選択解除のときnull）
        // 戻り値：なし
        //=========================================================================================
        public void RefreshSelectionInfo(TextBufferLineInfoList lineInfoList, TextViewerSelectionRange textSelect, DumpViewerSelectionRange dumpSelect) {
            if (textSelect != null) {
                RefreshSelectionInfoText(lineInfoList, textSelect);
            } else if (dumpSelect != null) {
                RefreshSelectionInfoDump(dumpSelect);
            } else {
                m_selectLabel.Text = "";
            }
        }

        //=========================================================================================
        // 機　能：選択情報をテキスト状態として更新する
        // 引　数：[in]lineInfoList  行情報
        // 　　　　[in]textSelect    テキストビューアでの選択情報（選択解除のときnull）
        // 戻り値：なし
        //=========================================================================================
        private void RefreshSelectionInfoText(TextBufferLineInfoList lineInfoList, TextViewerSelectionRange textSelect) {
            // (99,99)-(99,99)=99行
            // (99,99)-(99,99)=99文字
            int startLogicalLine = textSelect.StartLine;                // 開始の論理行
            int endLogicalLine = textSelect.EndLine;                    // 終了の論理行
            TextBufferLogicalLineInfo startLineInfo = lineInfoList.GetLineInfo(startLogicalLine);
            TextBufferLogicalLineInfo endLineInfo = lineInfoList.GetLineInfo(endLogicalLine);
            int startPhysicalLine = startLineInfo.PhysicalLineNo;       // 開始の物理行
            int endPhysicalLine = endLineInfo.PhysicalLineNo;           // 終了の物理行

            // 桁数を確認
            int startColumn = -1;                                       // 開始カラム
            int endColumn = -1;                                         // 終了カラム
            if (m_textSelectionInfoCache.StartPhysicalLine == startPhysicalLine && m_textSelectionInfoCache.StartColumn == textSelect.StartColumn) {
                startColumn = m_textSelectionInfoCache.StartResultColumn;
            } else {
                int count = 0;
                for (int i = startLogicalLine - 1; i >= 0; i--) {
                    TextBufferLogicalLineInfo lineInfo = lineInfoList.GetLineInfo(i);
                    if (lineInfo.PhysicalLineNo == startPhysicalLine) {
                        count += lineInfo.StrLineOrg.Length;
                    }
                }
                startColumn = count + textSelect.StartColumn;
                if (startPhysicalLine == endPhysicalLine) {
                    for (int i = startLogicalLine; i < endLogicalLine; i++) {
                        TextBufferLogicalLineInfo lineInfo = lineInfoList.GetLineInfo(i);
                        if (lineInfo.PhysicalLineNo == endPhysicalLine) {
                            count += lineInfo.StrLineOrg.Length;
                        }
                    }
                    endColumn = count + textSelect.EndColumn - 1;
                }
            }
            if (m_textSelectionInfoCache.EndPhysicalLine == endPhysicalLine && m_textSelectionInfoCache.EndColumn == textSelect.EndColumn) {
                endColumn = m_textSelectionInfoCache.EndResultColumn;
            } else if (endColumn == -1) {
                int count = 0;
                for (int i = endLogicalLine - 1; i >= 0; i--) {
                    TextBufferLogicalLineInfo lineInfo = lineInfoList.GetLineInfo(i);
                    if (lineInfo.PhysicalLineNo == endPhysicalLine) {
                        count += lineInfo.StrLineOrg.Length;
                    }
                }
                endColumn = count + textSelect.EndColumn - 1;
            }
            m_textSelectionInfoCache.SetResult(startPhysicalLine, textSelect.StartColumn, startColumn, endPhysicalLine, textSelect.EndColumn, endColumn);

            // 整形して修正
            string strLabel;
            if (startPhysicalLine == endPhysicalLine) {
                strLabel = string.Format(Resources.FileViewer_SelectionTextColumn, startPhysicalLine, startColumn + 1, endPhysicalLine, endColumn + 1, endColumn - startColumn + 1); 
            } else {
                int lineCount = endPhysicalLine - startPhysicalLine + 1;
                if (endColumn == -1) {
                    lineCount--;
                }
                strLabel = string.Format(Resources.FileViewer_SelectionTextLine, startPhysicalLine, startColumn + 1, endPhysicalLine, endColumn + 1, lineCount);
            }
            m_selectLabel.Text = strLabel;
        }

        //=========================================================================================
        // 機　能：選択情報をダンプ状態として更新する
        // 引　数：[in]dumpSelect    ダンプビューアでの選択情報（選択解除のときnull）
        // 戻り値：なし
        //=========================================================================================
        private void RefreshSelectionInfoDump(DumpViewerSelectionRange dumpSelect) {
            // (00000000-00000000)=999bytes
            string startAddress = dumpSelect.StartAddress.ToString("X" + DumpViewerComponent.ADDRESS_DIGHT_WIDTH);
            string endAddress = (dumpSelect.EndAddress - 1).ToString("X" + DumpViewerComponent.ADDRESS_DIGHT_WIDTH);
            int size = dumpSelect.EndAddress - dumpSelect.StartAddress;
            string strLabel = string.Format(Resources.FileViewer_SelectionDump, startAddress, endAddress, size);
            m_selectLabel.Text = strLabel;
        }

        //=========================================================================================
        // 機　能：テキスト選択中の桁位置のキャッシュをクリアする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetSelectTextColumnCache() {
            m_textSelectionInfoCache.ResetCache();
        }

        //=========================================================================================
        // 機　能：行番号を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshLineNo() {
            if (m_textFileViewer.TextViewerComponent == null) {
                return;
            }
            if (m_textFileViewer.TextBufferLineInfo.PhysicalLineCount == 0 || m_textFileViewer.TextBufferLineInfo.LogicalLineCount == 0) {
                m_lineNoLabel.Text = "0/0";
            } else {
                string text;
                if (m_textFileViewer.TextViewerComponent is TextViewerComponent) {
                    TextViewerComponent component = (TextViewerComponent)(m_textFileViewer.TextViewerComponent);
                    int current = component.VisibleTopLine;
                    int all = m_textFileViewer.TextBufferLineInfo.PhysicalLineCount;
                    text = string.Format("{0}/{1}", current, all);
                } else {
                    DumpViewerComponent component = (DumpViewerComponent)(m_textFileViewer.TextViewerComponent);
                    int current = component.Address;
                    int all;
                    byte[] readBuffer;
                    m_textFileViewer.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out all);
                    text = string.Format("{0:X8}/{1:X8}", current, all);
                }
                m_lineNoLabel.Text = text;
            }
        }

        //=========================================================================================
        // 機　能：テキスト情報を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void RefreshTextInfo() {
            m_tabLabel.Text = "TAB" + m_textFileViewer.TextBufferLineInfo.TabWidth;
            m_encodingLabel.Text = m_textFileViewer.TextBufferLineInfo.TextEncodingType.DisplayName;
        }

        //=========================================================================================
        // 機　能：エラーメッセージを更新する
        // 引　数：[in]message   エラーメッセージ
        // 　　　　[in]level     エラーのレベル
        // 　　　　[in]icon      使用するアイコン
        // 戻り値：なし
        //=========================================================================================
        public void ShowErrorMessage(string message, FileOperationStatus.LogLevel level, IconImageListID icon) {
            m_errorMessageImpl.ShowErrorMessageWorkThread(message, level, icon, StatusBarErrorMessageImpl.DisplayTime.Default);
        }

        public void SetSearchHitCount(int searchCount, int autoCount) {
            if (searchCount == 0) {
                m_searchLabel1.Text = "";
            } else {
                m_searchLabel1.Text = string.Format(Resources.FileViewer_HitCount, searchCount);
            }

            if (autoCount == 0) {
                m_searchLabel2.Text = "";
            } else {
                m_searchLabel2.Text = string.Format(Resources.FileViewer_AutoHitCount, autoCount);
            }
        }

        //=========================================================================================
        // 機　能：ステータスバーの領域がクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileStatusBar_Click(object sender, EventArgs e) {
        }

        //=========================================================================================
        // 機　能：ステータスバーのTABラベルがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void EncodingLabel_Click(object sender, EventArgs evt) {
            m_textFileViewer.OnLabelEncoding(m_encodingLabel);
        }

        //=========================================================================================
        // 機　能：ステータスバーのエンコーディングラベルがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TabLabel_Click(object sender, EventArgs evt) {
            m_textFileViewer.OnLabelTab(m_tabLabel);
        }

        //=========================================================================================
        // 機　能：ステータスバーの行番号ラベルがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void LineNoLabel_Click(object sender, EventArgs evt) {
            m_textFileViewer.OnLabelLineNo(m_lineNoLabel);
        }
    }
}
