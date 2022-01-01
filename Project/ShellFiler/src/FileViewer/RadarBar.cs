using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ShellFiler.Document;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：検索結果のレーダーバー
    //=========================================================================================
    public partial class RadarBar : Panel {
        // ファイルビューア（初期化前はnull）
        private TextFileViewer m_textFileViewer = null;

        // 検索にヒットした位置
        private BitArray m_hitFlagSearch = new BitArray(1);

        // 自動検索にヒットした位置
        private BitArray m_hitFlagAuto = new BitArray(1);

        // 画面のキャッシュ
        private Bitmap m_bmpScreenCache = null;

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public RadarBar() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]textFileViewer  テキストファイルビューア
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(TextFileViewer textFileViewer) {
            m_textFileViewer = textFileViewer;
        }

        //=========================================================================================
        // 機　能：ウィンドウを再描画するときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadarBar_Paint(object sender, PaintEventArgs evt) {
            int screenSize = GetScreenVerticalSize();
            if (screenSize < 0) {
                ;
            } else if (screenSize != m_hitFlagSearch.Count) {
                // データが無効な場合
                RadarBarGraphics g = new RadarBarGraphics(evt.Graphics, ClientRectangle.Width);
                g.Graphics.FillRectangle(g.RadarBarBackBrush, ClientRectangle);
                g.Dispose();
            } else {
                // データが有効な場合
                if (m_bmpScreenCache == null || m_bmpScreenCache.Width != ClientRectangle.Width || m_bmpScreenCache.Height != ClientRectangle.Height) {
                    // 画面キャッシュを作成
                    if (m_bmpScreenCache != null) {
                        m_bmpScreenCache.Dispose();
                    }
                    m_bmpScreenCache = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                    Graphics gBmp = Graphics.FromImage(m_bmpScreenCache);
                    try {
                        RadarBarGraphics g = new RadarBarGraphics(gBmp, ClientRectangle.Width);
                        try {
                            g.Graphics.FillRectangle(g.RadarBarBackBrush, ClientRectangle);
                        } finally {
                            g.Dispose();
                        }
                    } finally {
                        gBmp.Dispose();
                    }
                    CreateBitmapCache(m_bmpScreenCache);
                }

                evt.Graphics.DrawImage(m_bmpScreenCache, 0, 0);
            }
        }

        //=========================================================================================
        // 機　能：レーダーバーを表示する画面の高さを返す
        // 引　数：なし
        // 戻り値：画面の高さ
        //=========================================================================================
        private int GetScreenVerticalSize() {
            int screenSize = this.ClientRectangle.Height - SystemInformation.VerticalScrollBarArrowHeight * 2;
            if (m_textFileViewer.HorizontalScroll.Visible) {
                screenSize -= SystemInformation.HorizontalScrollBarHeight;
            }
            return screenSize;
        }

        //=========================================================================================
        // 機　能：ビットマップのキャッシュを作成する
        // 引　数：[in]bitmap  作成するビットマップ
        // 戻り値：なし
        //=========================================================================================
        private void CreateBitmapCache(Bitmap bitmap) {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr bitmapPtr = bitmapData.Scan0;
            byte[] data = new byte[bitmapData.Stride * bitmap.Height];
            Marshal.Copy(bitmapPtr, data, 0, data.Length);

            Color colSearch1 = Configuration.Current.TextViewerSearchHitBackColor;
            Color colSearch2 = Color.FromArgb(colSearch1.R / 2, colSearch1.G / 2, colSearch1.B / 2);
            Color colAuto1 = Configuration.Current.TextViewerSearchAutoTextColor;
            Color colAuto2 = Color.FromArgb(colAuto1.R / 2, colAuto1.G / 2, colAuto1.B / 2);
            int xPos1 = 1;
            int xPos2 = 3;
            int yPos = SystemInformation.VerticalScrollBarArrowHeight;
            for (int i = 0; i < m_hitFlagSearch.Count; i++) {
                if (m_hitFlagAuto[i]) {
                    SetMark(xPos2, yPos, data, bitmapData.Stride, colAuto1, colAuto2);
                }
                if (m_hitFlagSearch[i]) {
                    SetMark(xPos1, yPos, data, bitmapData.Stride, colSearch1, colSearch2);
                }
                yPos++;
            }
            Marshal.Copy(data, 0, bitmapPtr, data.Length);
            bitmap.UnlockBits(bitmapData);
        }

        //=========================================================================================
        // 機　能：ヒットのマークを作成する
        // 引　数：[in]xPos    左上のX座標
        // 　　　　[in]yPos    左上のY座標
        // 　　　　[in]data    作成するビットマップデータ
        // 　　　　[in]stride  ストライド値（Y座標の変位）
        // 　　　　[in]col1    色1
        // 　　　　[in]col2    色2
        // 戻り値：なし
        //=========================================================================================
        private void SetMark(int xPos, int yPos, byte[] data, int stride, Color col1, Color col2) {
            byte col1R = col1.R;
            byte col1G = col1.G;
            byte col1B = col1.B;
            byte col2R = col2.R;
            byte col2G = col2.G;
            byte col2B = col2.B;
            int pos;
            pos = xPos * 3 + stride * yPos;
            data[pos++] = col1B;
            data[pos++] = col1G;
            data[pos++] = col1R;
            data[pos++] = col1B;
            data[pos++] = col1G;
            data[pos++] = col1R;
            data[pos++] = col2B;
            data[pos++] = col2G;
            data[pos++] = col2R;
            pos = xPos * 3 + stride * (yPos + 1);
            data[pos++] = col1B;
            data[pos++] = col1G;
            data[pos++] = col1R;
            data[pos++] = col1B;
            data[pos++] = col1G;
            data[pos++] = col1R;
            data[pos++] = col2B;
            data[pos++] = col2G;
            data[pos++] = col2R;
            pos = xPos * 3 + stride * (yPos + 2);
            data[pos++] = col2B;
            data[pos++] = col2G;
            data[pos++] = col2R;
            data[pos++] = col2B;
            data[pos++] = col2G;
            data[pos++] = col2R;
            data[pos++] = col2B;
            data[pos++] = col2G;
            data[pos++] = col2R;            
        }

        //=========================================================================================
        // 機　能：検索が終了したことを通知する
        // 引　数：[in]index  logLine  登録するログ情報
        // 戻り値：なし
        //=========================================================================================
        public void NotifySearchEnd() {
            UpdateHitPosition();
        }

        //=========================================================================================
        // 機　能：ヒット位置の情報を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateHitPosition() {
            // フラグを初期化
            int screenSize = GetScreenVerticalSize();
            if (screenSize <= 0) {
                m_bmpScreenCache = null;
                return;
            }

            m_hitFlagSearch = new BitArray(screenSize);
            m_hitFlagAuto = new BitArray(screenSize);
            for (int i = 0; i < screenSize; i++) {
                m_hitFlagSearch[i] = false;
                m_hitFlagAuto[i] = false;
            }

            // ヒット位置を記録
            if (m_textFileViewer.TextViewerComponent == null) {
                return;
            } else if (m_textFileViewer.TextViewerComponent is TextViewerComponent) {
                UpdateHitPositionTextViewer();
            } else {
                UpdateHitPositionDumpViewer();
            }

            // 再描画
            if (m_bmpScreenCache != null) {
                m_bmpScreenCache.Dispose();
                m_bmpScreenCache = null;
            }
            this.Invalidate();
        }

        //=========================================================================================
        // 機　能：テキストビューアの状態に基づいてヒット位置の情報を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateHitPositionTextViewer() {
            int screenSize = m_hitFlagAuto.Count;
            TextBufferLineInfoList lineInfoList = m_textFileViewer.TextBufferLineInfo;
            int lineCount = lineInfoList.LogicalLineCount;
            for (int i = 0; i < lineCount; i++) {
                TextBufferLogicalLineInfo lineInfo = lineInfoList.GetLineInfo(i);
                if (lineInfo.SearchHitState == SearchHitState.Hit) {
                    int pos = (int)((double)screenSize * (double)i / (double)lineCount);
                    m_hitFlagSearch[pos] = true;
                }
                if (lineInfo.AutoSearchHitState == SearchHitState.Hit) {
                    int pos = (int)((double)screenSize * (double)i / (double)lineCount);
                    m_hitFlagAuto[pos] = true;
                }
            }
        }

        //=========================================================================================
        // 機　能：ダンプビューアの状態に基づいてヒット位置の情報を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateHitPositionDumpViewer() {
            DumpSearchHitStateList hitState = m_textFileViewer.SearchEngine.DumpSearchHitStateList;
            DumpViewerComponent viewer = (DumpViewerComponent)(m_textFileViewer.TextViewerComponent);
            int readSize;
            byte[] readBuffer;
            m_textFileViewer.TextBufferLineInfo.TargetFile.GetBuffer(out readBuffer, out readSize);
            int lineCount = readSize / viewer.DumpLineByteCount;
            hitState.SetRadarBarInfo(lineCount, m_hitFlagSearch, m_hitFlagAuto);
        }

        //=========================================================================================
        // 機　能：ウィンドウサイズが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadarBar_SizeChanged(object sender, EventArgs e) {
            if (m_textFileViewer != null) {
                UpdateHitPosition();
            }
        }
    }
}
