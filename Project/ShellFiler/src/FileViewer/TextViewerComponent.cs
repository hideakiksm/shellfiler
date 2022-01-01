using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Dialog.FileViewer;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：テキストビューアの実装
    //=========================================================================================
    public class TextViewerComponent : IViewerComponent, ViewerScrollImpl.IScrollValueSource {
        // 画面の左側に作成する行番号とコンテンツの間の描画しない領域のマージン
        public const int CX_DISPLAY_LEFT_MARGIN = 2;
        
        // 画面の右側に作成する描画しない領域のマージン
        public const int CX_DISPLAY_RIGHT_MARGIN = 4;


        // この実装の所有ビュー
        private TextFileViewer m_parent;

        // スクロール機能の実装
        private ViewerScrollImpl m_viewerScrollImpl;
        
        // 行の高さ[ピクセル]
        private int m_cyLineHeight;

        // セパレータの表示Ｘ位置
        private float m_xPosSeparator;

        // コンテンツの表示Ｘ位置
        private float m_xPosContents;

        // 水平スクロールバーの位置
        private int m_scrollXPosition;

        // 垂直スクロールバーの位置（先頭行に表示している表示上の行数）
        private int m_scrollYPosition = 0;

        // 画面上に完全に表示できる行数
        private int m_scrollYCompleteLineSize = 0;

        // 検索時のカーソル行（-1:表示しない）
        private int m_scrollSearchCursorLine = -1;

        // 半角１文字分の大きさの期待値
        private SizeF m_fontSize;

        // 選択範囲（選択していないときnull）
        private TextViewerSelectionRange m_selectionRange = null;

        // 検索条件（検索していないときnull）
        private TextSearchCondition m_searchCondition = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent   所有ビュー
        // 　　　　[in]fontSize 半角１文字分の大きさの期待値
        // 戻り値：なし
        //=========================================================================================
        public TextViewerComponent(TextFileViewer parent, SizeF fontSize) {
            m_parent = parent;
            m_fontSize = fontSize;

            // 表示位置を決める
            TextViewerGraphics g = new TextViewerGraphics(m_parent, 0);
            try {
                m_cyLineHeight = g.TextFont.Height;
                if (m_parent.TextBufferLineInfo.IsDisplayLineNumber) {
                    m_xPosSeparator = (int)(Math.Ceiling(GetLineDigit(m_parent.TextBufferLineInfo.MaxLineCount) * m_fontSize.Width + CX_DISPLAY_LEFT_MARGIN));
                    m_xPosContents = m_xPosSeparator + CX_DISPLAY_LEFT_MARGIN;
                } else {
                    m_xPosSeparator = 0;
                    m_xPosContents = 0;
                }
            } finally {
                g.Dispose();
            }

            m_viewerScrollImpl = new ViewerScrollImpl(parent, this, m_cyLineHeight);
        }

        //=========================================================================================
        // 機　能：該当の行番号が描画対象となるかどうかを返す
        // 引　数：[in]minLine  最小行番号
        // 　　　　[in]maxLine  最大行番号
        // 　　　　[in]minByte  最小ファイル位置
        // 　　　　[in]maxByte  最大ファイル位置
        // 戻り値：描画対象のときtrue
        //=========================================================================================
        public bool IsDisplay(int minLine, int maxLine, int minByte, int maxByte) {
            int extendLine = SystemInformation.HorizontalScrollBarHeight / m_cyLineHeight + 1;
            if (maxLine < ScrollYPosition || minLine > ScrollYPosition + ScrollYCompleteLineSize + extendLine) {
                return false;
            } else {
                return true;
            }
        }

        //=========================================================================================
        // 機　能：サイズ変更時の処理を行う
        // 引　数：[in]rcClient     クライアント領域
        // 戻り値：なし
        //=========================================================================================
        public void OnSizeChange(Rectangle rcClient) {
            m_viewerScrollImpl.OnSizeChange();
        }

        //=========================================================================================
        // 機　能：水平スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetHorizontalScrollbar() {
            m_viewerScrollImpl.SetHorizontalScrollbar();
        }
        
        //=========================================================================================
        // 機　能：垂直スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetVerticalScrollbar() {
            m_viewerScrollImpl.SetVerticalScrollbar();
        }
        
        //=========================================================================================
        // 機　能：画面を描画する
        // 引　数：[in]g             グラフィックス
        // 　　　　[in]fillBack      背景を塗りつぶすときtrue
        // 　　　　[in]dispStartLine 画面上の開始行（-1で全体を描画）
        // 　　　　[in]dispEndLine   画面上の終了行
        // 戻り値：なし
        //=========================================================================================
        public void OnPaint(TextViewerGraphics g, bool fillBack, int dispStartLine, int dispEndLine) {
            int extendLine = SystemInformation.HorizontalScrollBarHeight / m_cyLineHeight + 1;
            if (dispStartLine == -1) {
                dispStartLine = 0;
                dispEndLine = ScrollYCompleteLineSize + extendLine;
            }

            // 行番号背景
            if (m_parent.TextBufferLineInfo.IsDisplayLineNumber) {
                float yPos1 = dispStartLine * m_cyLineHeight;
                float yPos2 = (dispEndLine + 1) * m_cyLineHeight;
                RectangleF rcLine = new RectangleF(-ScrollXPosition, yPos1, m_xPosSeparator, yPos2 - yPos1);
                g.Graphics.FillRectangle(g.TextViewerLineNoBackBrush, rcLine);
                g.Graphics.DrawLine(g.TextViewerLineNoSeparatorPen, rcLine.Right, yPos1, m_xPosSeparator - ScrollXPosition, rcLine.Bottom);
            }

            // 領域外
            if (TextRightLimitBorder > 0 && TextRightLimitBorder < m_parent.ClientRectangle.Width) {
                float yPos1 = dispStartLine * m_cyLineHeight;
                float yPos2 = (dispEndLine + 1) * m_cyLineHeight;
                RectangleF rcOut = new RectangleF(TextRightLimitBorder, yPos1, m_parent.ClientRectangle.Width - TextRightLimitBorder, yPos2 - yPos1);
                g.Graphics.FillRectangle(g.TextViewerOutOfAreaBackBrush, rcOut);
                g.Graphics.DrawLine(g.TextViewerOutOfAreaSeparatorPen, rcOut.Left, yPos1, rcOut.Left, rcOut.Bottom);
            }

            // 行の内容表示
            for (int line = dispStartLine; line <= dispEndLine; line++) {
                if (m_parent.TextBufferLineInfo.LogicalLineCount <= line + ScrollYPosition) {
                    continue;
                }

                TextBufferLogicalLineInfo lineInfo = m_parent.TextBufferLineInfo.GetLineInfo(line + ScrollYPosition);
                DrawLineNumber(g, line, lineInfo);
                if (m_selectionRange != null && m_selectionRange.Selected) {
                    // 選択中
                    DrawLineSelect(g, fillBack, line, lineInfo);
                } else if (lineInfo.SearchHitState == SearchHitState.Hit || lineInfo.AutoSearchHitState == SearchHitState.Hit) {
                    // 検索ヒット状態
                    DrawLineSearchHit(g, fillBack, line, lineInfo);
                } else {
                    // 選択・検索なし
                    DrawLineNormal(g, fillBack, line, lineInfo);
                }
            }
            // カーソル
            if (ScrollSearchCursorLine != -1) {
                float xPos = m_xPosContents - ScrollXPosition - 1;
                int yPos = (ScrollSearchCursorLine - ScrollYPosition + 1) * m_cyLineHeight - 1;
                g.Graphics.DrawLine(g.SearchCursorPen, xPos, yPos, TextRightLimitBorder - 1, yPos);
            }
        }

        //=========================================================================================
        // 機　能：行番号を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]fillBack  背景を塗りつぶすときtrue
        // 　　　　[in]dispLine  表示上の行数
        // 　　　　[in]lineInfo  描画する論理行の情報
        // 戻り値：なし
        //=========================================================================================
        private void DrawLineNumber(TextViewerGraphics g, int dispLine, TextBufferLogicalLineInfo lineInfo) {
            int yPos = dispLine * m_cyLineHeight;
            if (m_parent.TextBufferLineInfo.IsDisplayLineNumber) {
                bool isDispLineNo = true;
                if (dispLine + ScrollYPosition > 0) {
                    TextBufferLogicalLineInfo lineInfoPrev = m_parent.TextBufferLineInfo.GetLineInfo(dispLine + ScrollYPosition - 1);
                    if (lineInfo.PhysicalLineNo == lineInfoPrev.PhysicalLineNo) {
                        isDispLineNo = false;
                    }
                }
                if (isDispLineNo) {
                    int lineDigit = GetLineDigit(m_parent.TextBufferLineInfo.MaxLineCount);
                    string strLine = string.Format("{0, " + lineDigit + "}", lineInfo.PhysicalLineNo);
                    g.Graphics.DrawString(strLine, g.TextFont, g.TextViewerLineNoTextBrush, -ScrollXPosition, yPos);
                }
            }
        }

        //=========================================================================================
        // 機　能：検索／選択なし状態で行の内容を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]fillBack  背景を塗りつぶすときtrue
        // 　　　　[in]dispLine  表示上の行数
        // 　　　　[in]lineInfo  描画する論理行の情報
        // 戻り値：なし
        //=========================================================================================
        private void DrawLineNormal(TextViewerGraphics g, bool fillBack, int dispLine, TextBufferLogicalLineInfo lineInfo) {
            g.Graphics.SetClip(m_parent.ClientRectangle);

            FillLineBack(g, fillBack, dispLine);

            int yPos = dispLine * m_cyLineHeight;
            string dispStr;
            List<int> orgToTab, tabToOrg;
            m_parent.TextBufferLineInfo.ExtractTab(g, lineInfo.StrLineOrg, out dispStr, out orgToTab, out tabToOrg);
            g.Graphics.DrawString(dispStr, g.TextFont, g.TextViewerTextBrush, new PointF(m_xPosContents - ScrollXPosition, yPos));

            float width = GraphicsUtils.MeasureString(g.Graphics, g.TextFont, dispStr);
            DrawReturnCode(g, m_xPosContents + width - ScrollXPosition, yPos, lineInfo.LineBreakChar);
        }

        //=========================================================================================
        // 機　能：1行分の背景を塗りつぶす
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]fillBack  背景を塗りつぶすときtrue
        // 　　　　[in]dispLine  表示上の行数
        // 戻り値：なし
        //=========================================================================================
        private void FillLineBack(TextViewerGraphics g, bool fillBack, int dispLine) {
            if (fillBack) {
                g.Graphics.SetClip(m_parent.ClientRectangle);
                float xPos = m_xPosContents - ScrollXPosition;
                float yPos = dispLine * m_cyLineHeight;
                g.Graphics.FillRectangle(SystemBrushes.Window, new RectangleF(xPos, yPos, TextRightLimitBorder - xPos, m_cyLineHeight));
            }
        }

        //=========================================================================================
        // 機　能：選択状態で行の内容を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]dispLine  表示上の行数
        // 　　　　[in]lineInfo  描画する論理行の情報
        // 戻り値：なし
        //=========================================================================================
        private void DrawLineSelect(TextViewerGraphics g, bool fillBack, int dispLine, TextBufferLogicalLineInfo lineInfo) {
            int yPos = dispLine * m_cyLineHeight;
            int yPosNext = (dispLine + 1) * m_cyLineHeight;
            int currentLine = dispLine + ScrollYPosition;
            float xPos = m_xPosContents - ScrollXPosition;

            // タブを展開
            string dispStr;
            List<int> orgToTab, tabToOrg;
            m_parent.TextBufferLineInfo.ExtractTab(g, lineInfo.StrLineOrg, out dispStr, out orgToTab, out tabToOrg);
            int dispStartColumn = 0;
            if (currentLine == m_selectionRange.StartLine) {
                dispStartColumn = orgToTab[m_selectionRange.StartColumn];
            }
            int dispEndColumn = 0;
            if (currentLine == m_selectionRange.EndLine) {
                dispEndColumn = orgToTab[m_selectionRange.EndColumn];
            }

            RectangleF rcSelect = new RectangleF(0, 0, 0, 0);
            if (m_selectionRange.StartLine == m_selectionRange.EndLine && m_selectionRange.StartLine == currentLine) {
                // 同じ行で開始・終了
                string str1 = dispStr.Substring(0, dispStartColumn);
                string str2 = dispStr.Substring(dispStartColumn, dispEndColumn - dispStartColumn);
                string str3 = dispStr.Substring(dispEndColumn);
                float[] charRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, dispStr, dispStartColumn, dispEndColumn, dispStr.Length);
                float x2 = xPos + charRegion[0];
                float x3 = xPos + charRegion[1];
                float margin = (charRegion[0] == 0) ? 0 : 1;
                rcSelect = new RectangleF(x2 - margin, yPos, x3 - x2, m_cyLineHeight);
            } else if (currentLine < m_selectionRange.StartLine || m_selectionRange.EndLine < currentLine) {
                // 選択なし
                ;
            } else if (currentLine == m_selectionRange.StartLine && dispStartColumn > 0) {
                // 行の途中から選択
                string str1 = dispStr.Substring(0, dispStartColumn);
                string str2 = dispStr.Substring(dispStartColumn);
                float[] charRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, dispStr, dispStartColumn, dispStr.Length);
                float x2 = xPos + charRegion[0];
                float x3 = xPos + charRegion[1];
                if (lineInfo.LineBreakChar == LineBreakChar.Cr || lineInfo.LineBreakChar == LineBreakChar.CrLf) {
                    x3 += m_fontSize.Width;
                }
                rcSelect = new RectangleF(x2 - 1, yPos, x3 - x2, m_cyLineHeight);
            } else if (currentLine == m_selectionRange.EndLine && dispStr.Length >= dispEndColumn) {
                // 行の途中まで選択
                string str1 = dispStr.Substring(0, dispEndColumn);
                string str2 = dispStr.Substring(dispEndColumn);
                float[] charRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, dispStr, dispEndColumn, dispStr.Length);
                float x1 = xPos;
                float x2 = xPos + charRegion[0];
                rcSelect = new RectangleF(x1, yPos, x2 - x1, m_cyLineHeight);
            } else {
                // すべて選択
                float[] charRegion = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, dispStr, dispStr.Length);
                float x1 = xPos;
                float x2 = xPos + charRegion[0];
                if (lineInfo.LineBreakChar == LineBreakChar.Cr || lineInfo.LineBreakChar == LineBreakChar.CrLf) {
                    x2 += m_fontSize.Width;
                }
                rcSelect = new RectangleF(x1, yPos, x2 - x1, m_cyLineHeight);
            }

            // 通常状態で描画
            g.Graphics.SetClip(rcSelect, CombineMode.Exclude);
            if (fillBack) {
                g.Graphics.FillRectangle(SystemBrushes.Window, new RectangleF(xPos, yPos, TextRightLimitBorder - xPos, m_cyLineHeight));
            }
            g.Graphics.DrawString(dispStr, g.TextFont, g.TextViewerTextBrush, new PointF(xPos, yPos));

            // 選択状態で描画
            if (rcSelect.Width > 0) {
                g.Graphics.SetClip(rcSelect);
                g.Graphics.FillRectangle(g.TextViewerSelectBackBrush, rcSelect);
                g.Graphics.DrawString(dispStr, g.TextFont, g.TextViewerSelectTextBrush, new PointF(xPos, yPos));
            }
            g.Graphics.SetClip(m_parent.ClientRectangle);

            // 改行を描画
            float width = GraphicsUtils.MeasureString(g.Graphics, g.TextFont, dispStr);
            DrawReturnCode(g, m_xPosContents + width - ScrollXPosition, yPos, lineInfo.LineBreakChar);
        }

        //=========================================================================================
        // 機　能：検索ヒット状態で行の内容を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]fillBack  背景を塗りつぶすときtrue
        // 　　　　[in]dispLine  表示上の行数
        // 　　　　[in]lineInfo  描画する論理行の情報
        // 戻り値：なし
        //=========================================================================================
        private void DrawLineSearchHit(TextViewerGraphics g, bool fillBack, int dispLine, TextBufferLogicalLineInfo lineInfo) {
            float xPos = m_xPosContents - ScrollXPosition;
            float yPos = dispLine * m_cyLineHeight;

            // 同じ物理行の文字列を作成
            int line = ScrollYPosition + dispLine;
            int startLine;
            int endLine;
            m_parent.TextBufferLineInfo.GetSameLineRange(line, out startLine, out endLine);
            int startColumn = 0;
            StringBuilder sbLine = new StringBuilder();
            for (int i = startLine; i <= endLine; i++) {
                TextBufferLogicalLineInfo infoI = m_parent.TextBufferLineInfo.GetLineInfo(i);
                if (i == line) {
                    startColumn = sbLine.Length;
                }
                sbLine.Append(infoI.StrLineOrg);
            }
            string strPhysical = sbLine.ToString();

            // 検索文字列のヒット範囲を確認
            List<int> hitPosSearch, hitLengthSearch, hitPosAuto, hitLengthAuto;
            TextSearchCore searchCore = new TextSearchCore();
            searchCore.SearchLine(strPhysical, m_searchCondition, out hitPosSearch, out hitLengthSearch, out hitPosAuto, out hitLengthAuto);

            // 表示文字列を取得
            string dispStr;
            List<int> orgToTab, tabToOrg;
            m_parent.TextBufferLineInfo.ExtractTab(g, lineInfo.StrLineOrg, out dispStr, out orgToTab, out tabToOrg);

            // 背景色を描画
            bool drawBack = false;
            if (hitPosSearch != null && hitPosSearch.Count > 0) {
                List<HitDiffPoint> searchDiffPoint = searchCore.GetHitDiffPointSearch(hitPosSearch, hitLengthSearch, startColumn, lineInfo.StrLineOrg.Length);
                if (searchDiffPoint.Count > 0) {     // 必ずhit=trueで始まり、hit=falseで終わる
                    DrawSearchHitBack(g, fillBack, dispLine, lineInfo, searchDiffPoint, startColumn, dispStr, orgToTab);
                    drawBack = true;
                }
            }
            if (!drawBack) {
                FillLineBack(g, fillBack, dispLine);
            }

            // 文字を描画
            bool drawString = false;
            if (hitPosAuto != null && hitPosAuto.Count > 0 || hitPosSearch != null && hitPosSearch.Count > 0) {
                List<HitDiffPoint> autoDiffPoint = searchCore.GetHitDiffPointAutoAndSearch(hitPosSearch, hitLengthSearch, hitPosAuto, hitLengthAuto, startColumn, lineInfo.StrLineOrg.Length);
                if (autoDiffPoint.Count > 0) {     // 必ずいずれかのhit=trueで始まり、両方のhit=falseで終わる
                    DrawSearchHitText(g, fillBack, dispLine, lineInfo, autoDiffPoint, startColumn, dispStr, orgToTab);
                    drawString = true;
                }
            }
            if (!drawString) {
                g.Graphics.DrawString(dispStr, g.TextFont, g.TextViewerTextBrush, m_xPosContents - ScrollXPosition, yPos);
            }
            
            // 改行を描画
            float width = GraphicsUtils.MeasureString(g.Graphics, g.TextFont, dispStr);
            DrawReturnCode(g, m_xPosContents + width - ScrollXPosition, yPos, lineInfo.LineBreakChar);
        }

        //=========================================================================================
        // 機　能：検索ヒット状態で行の背景部分を描画する
        // 引　数：[in]g            グラフィックス
        // 　　　　[in]fillBack     背景を塗りつぶすときtrue
        // 　　　　[in]dispLine     表示上の行数
        // 　　　　[in]lineInfo     描画する論理行の情報
        // 　　　　[in]hitDiffPoint 検索ヒットしたときの変化点の情報（HitAutoSearchは未使用）
        // 　　　　[in]startPos     論理行が開始される、物理行文字列中のインデックス
        // 　　　　[in]dispStr      タブ展開済みの表示文字列
        // 　　　　[in]orgToTab     元の文字列からタブ展開済み文字列へのインデックスの対応
        // 戻り値：なし
        //=========================================================================================
        private void DrawSearchHitBack(TextViewerGraphics g, bool fillBack, int dispLine, TextBufferLogicalLineInfo lineInfo, List<HitDiffPoint> hitDiffPoint, int startPos, string dispStr, List<int> orgToTab) {
            float xPos = m_xPosContents - ScrollXPosition;
            float yPos = dispLine * m_cyLineHeight;

            // 文字単位の表示座標を計算
            List<int> measure = new List<int>();
            foreach (HitDiffPoint hit in hitDiffPoint) {
                measure.Add(orgToTab[hit.Position - startPos]);
            }

            float[] border = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, dispStr, measure.ToArray());
            List<RectangleF> rcList = new List<RectangleF>();
            for (int i = 0; i < border.Length - 1; i += 2) {
                rcList.Add(new RectangleF(xPos + border[i] - 1, yPos, border[i + 1] - border[i], m_cyLineHeight));
            }

            // 描画
            if (fillBack) {
                g.Graphics.SetClip(m_parent.ClientRectangle);
                foreach (RectangleF rc in rcList) {
                    g.Graphics.SetClip(rc, CombineMode.Xor);
                }
                g.Graphics.FillRectangle(SystemBrushes.Window, new RectangleF(xPos, yPos, TextRightLimitBorder - xPos, m_cyLineHeight));
            }
            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Exclude);
            foreach (RectangleF rc in rcList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.FillRectangle(g.TextViewerSearchHitBackBrush, new RectangleF(xPos, yPos, TextRightLimitBorder - xPos, m_cyLineHeight));
            g.Graphics.SetClip(m_parent.ClientRectangle);
        }

        //=========================================================================================
        // 機　能：検索ヒット状態で行の文字部分を描画する
        // 引　数：[in]g            グラフィックス
        // 　　　　[in]fillBack     背景を塗りつぶすときtrue
        // 　　　　[in]dispLine     表示上の行数
        // 　　　　[in]lineInfo     描画する論理行の情報
        // 　　　　[in]hitDiffPoint 検索ヒットしたときの変化点の情報
        // 　　　　[in]startPos     論理行が開始される、物理行文字列中のインデックス
        // 　　　　[in]dispStr      タブ展開済みの表示文字列
        // 　　　　[in]orgToTab     元の文字列からタブ展開済み文字列へのインデックスの対応
        // 戻り値：なし
        //=========================================================================================
        private void DrawSearchHitText(TextViewerGraphics g, bool fillBack, int dispLine, TextBufferLogicalLineInfo lineInfo, List<HitDiffPoint> hitDiffPoint, int startPos, string dispStr, List<int> orgToTab) {
            float xPos = m_xPosContents - ScrollXPosition;
            float yPos = dispLine * m_cyLineHeight;

            // 文字単位の表示座標を計算
            List<int> measure = new List<int>();
            foreach (HitDiffPoint hit in hitDiffPoint) {
                measure.Add(orgToTab[hit.Position - startPos]);
            }

            float[] border = TextRendererUtils.MeasureStringRegion(g.Graphics, g.TextFont, dispStr, measure.ToArray());
            List<RectangleF> rcSearchList = new List<RectangleF>();
            List<RectangleF> rcAutoList = new List<RectangleF>();
            for (int i = 0; i < border.Length - 1; i++) {
                if (hitDiffPoint[i].HitAutoSearch) {
                    rcAutoList.Add(new RectangleF(xPos + border[i] - 1, yPos, border[i + 1] - border[i], m_cyLineHeight));
                } else if (hitDiffPoint[i].HitSearch) {
                    rcSearchList.Add(new RectangleF(xPos + border[i] - 1, yPos, border[i + 1] - border[i], m_cyLineHeight));
                }
            }
            
            // 描画
            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Exclude);
            foreach (RectangleF rc in rcSearchList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.DrawString(dispStr, g.TextFont, g.TextViewerSearchHitTextBrush, m_xPosContents - ScrollXPosition, yPos);

            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Exclude);
            foreach (RectangleF rc in rcAutoList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.DrawString(dispStr, g.TextFont, g.TextViewerSearchAutoTextBrush, m_xPosContents - ScrollXPosition, yPos);

            foreach (RectangleF rc in rcSearchList) {
                g.Graphics.SetClip(rc, CombineMode.Xor);
            }
            g.Graphics.SetClip(m_parent.ClientRectangle, CombineMode.Xor);
            g.Graphics.DrawString(dispStr, g.TextFont, g.TextViewerTextBrush, m_xPosContents - ScrollXPosition, yPos);

            g.Graphics.SetClip(m_parent.ClientRectangle);
        }

        //=========================================================================================
        // 機　能：行番号の桁数を返す
        // 引　数：[in]maxLine  行番号に表示する数値
        // 戻り値：表示に必要な桁数
        //=========================================================================================
        private int GetLineDigit(int maxLine) {
            if (maxLine >= 10000000) {
                return 8;
            } else if (maxLine >= 1000000) {
                return 7;
            } else if (maxLine >= 100000) {
                return 6;
            } else if (maxLine >= 10000) {
                return 5;
            } else {
                return 4;
            }
        }

        //=========================================================================================
        // 機　能：改行コードを描画する
        // 引　数：[in]g     グラフィックス
        // 　　　　[in]xPos  表示Ｘ位置
        // 　　　　[in]yPos  表示Ｙ位置
        // 　　　　[in]crlf  表示する改行位置
        // 戻り値：なし
        //=========================================================================================
        private void DrawReturnCode(TextViewerGraphics g, float xPos, float yPos, LineBreakChar crlf) {
            if (crlf == LineBreakChar.None) {
                return;
            }
            if (!Configuration.Current.TextViewerIsDisplayCtrlChar) {
                return;
            }
            if (crlf == LineBreakChar.CrLf) {
                float cx = m_fontSize.Width;
                float cy = m_fontSize.Height;
                PointF[] ptArrow = null;
                if (cx >= 6) {
                    int arrowSize = ((int)cx - 1) / 2 + 1;
                    ptArrow = new PointF[3];
                    ptArrow[0] = new PointF(xPos + 1,         yPos + cy - arrowSize - 2);
                    ptArrow[1] = new PointF(xPos + arrowSize, yPos + cy - arrowSize * 2 - 1);
                    ptArrow[2] = new PointF(xPos + arrowSize, yPos + cy - 3);
                    g.Graphics.FillPolygon(g.TextViewerControlBrush, ptArrow);
                    ptArrow[0] = new PointF(xPos + 1,      yPos + cy - arrowSize - 2);
                    ptArrow[1] = new PointF(xPos + cx - 2, yPos + cy - arrowSize - 2);
                    ptArrow[2] = new PointF(xPos + cx - 2, yPos + cy / 4);
                    g.Graphics.DrawLines(g.TextViewerControlPen, ptArrow);
                } else {
                    ptArrow = new PointF[4];
                    ptArrow[0] = new PointF(xPos + 1,      yPos + 1);
                    ptArrow[1] = new PointF(xPos + 1,      yPos + cy - 1);
                    ptArrow[2] = new PointF(xPos + cx - 1, yPos + 1);
                    ptArrow[3] = ptArrow[0];
                    g.Graphics.DrawLines(g.TextViewerControlPen, ptArrow);
                }
            } else if (crlf == LineBreakChar.Cr) {
                float cx = m_fontSize.Width;
                float cy = m_fontSize.Height;
                PointF[] ptArrow = null;
                if (cx >= 6) {
                    int arrowSize = ((int)cx - 1) / 2;
                    ptArrow = new PointF[3];
                    ptArrow[0] = new PointF(xPos + 1,             yPos + cy - arrowSize - 2);
                    ptArrow[1] = new PointF(xPos + arrowSize,     yPos + cy - 2);
                    ptArrow[2] = new PointF(xPos + arrowSize * 2, yPos + cy - arrowSize - 2);
                    g.Graphics.FillPolygon(g.TextViewerControlBrush, ptArrow);
                    ptArrow[0] = new PointF(xPos + arrowSize, yPos + cy - 2);
                    ptArrow[1] = new PointF(xPos + arrowSize, yPos + cy / 4);
                    g.Graphics.DrawLine(g.TextViewerControlPen, ptArrow[0], ptArrow[1]);
                } else {
                    ptArrow = new PointF[4];
                    ptArrow[0] = new PointF(xPos + 1,      yPos + 1);
                    ptArrow[1] = new PointF(xPos + 1,      yPos + cy - 1);
                    ptArrow[2] = new PointF(xPos + cx - 1, yPos + 1);
                    ptArrow[3] = ptArrow[0];
                    g.Graphics.DrawLines(g.TextViewerControlPen, ptArrow);
                }
            } else {
                m_parent.DrawEofMark(g, xPos, yPos, crlf);
            }
        }

        //=========================================================================================
        // 機　能：スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnHScroll(ScrollEventArgs evt) {
            m_viewerScrollImpl.OnHScroll(evt);
        }

        //=========================================================================================
        // 機　能：スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnVScroll(ScrollEventArgs evt) {
            m_viewerScrollImpl.OnVScroll(evt);
        }
        
        //=========================================================================================
        // 機　能：マウスホイールイベントを処理する
        // 引　数：[in]evt  マウスイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseWheel(MouseEventArgs evt) {
            m_viewerScrollImpl.OnMouseWheel(evt);
        }

        //=========================================================================================
        // 機　能：表示位置を上下に移動する
        // 引　数：[in]lines    移動する行数（下方向が＋）
        // 戻り値：なし
        //=========================================================================================
        public void MoveDisplayPosition(int lines) {
            m_viewerScrollImpl.MoveDisplayPosition(lines);
        }
        
        //=========================================================================================
        // 機　能：マウスのボタンがダブルクリックされたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseDoubleClick(MouseEventArgs evt) {
            int line;
            int column;
            bool onChar;
            GetLineColumnPosition(evt.X, evt.Y, out line, out column, out onChar);
            if (onChar) {
                // 文字列が見つかった
                int startLine;
                int startColumn;
                int endLine;
                int endColumn;
                PickupKeyword(line, column, out startLine, out startColumn, out endLine, out endColumn);
                m_selectionRange = new TextViewerSelectionRange();
                m_selectionRange.StartLine = startLine;
                m_selectionRange.StartColumn = startColumn;
                m_selectionRange.EndLine = endLine;
                m_selectionRange.EndColumn = endColumn;
                m_parent.Invalidate();
                m_parent.FileViewerStatusBar.RefreshSelectionInfo(m_parent.TextBufferLineInfo, m_selectionRange, null);
            } else {
                // 周辺を調査
                int mouseX = (int)(evt.X - m_fontSize.Width);
                bool onChar1;
                bool onChar2;
                bool onChar3;
                GetLineColumnPosition(mouseX, evt.Y - m_cyLineHeight, out line, out column, out onChar1);
                GetLineColumnPosition(mouseX, evt.Y,                  out line, out column, out onChar2);
                GetLineColumnPosition(mouseX, evt.Y + m_cyLineHeight, out line, out column, out onChar3);
                if (onChar1 || onChar2 || onChar3) {
                    return;
                }

                // 終了コマンドを取得して実行
                FileViewerActionCommand command = Program.Document.CommandFactory.CreateFileViewerCommandFromMouseInput(evt, m_parent);
                if (command != null) {
                    command.Execute();
                }
            }
        }

        //=========================================================================================
        // 機　能：画面からキーワードを取得する
        // 引　数：[in]line         取得する論理行
        // 　　　　[in]column       取得するカラム位置
        // 　　　　[out]startLine   開始行
        // 　　　　[out]startColumn 開始桁
        // 　　　　[out]endLine     終了行
        // 　　　　[out]endColumn   終了桁
        // 戻り値：取得したキーワード
        //=========================================================================================
        private string PickupKeyword(int line, int column, out int startLine, out int startColumn, out int endLine, out int endColumn) {
            // 同一行の範囲を検索
            int startLinePick;
            int endLinePick;
            m_parent.TextBufferLineInfo.GetSameLineRange(line, out startLinePick, out endLinePick);

            // 1つの文字列にまとめる
            int sbColumn = 0;
            StringBuilder sbLine = new StringBuilder();
            for (int i = startLinePick; i <= endLinePick; i++) {
                TextBufferLogicalLineInfo lineInfo = m_parent.TextBufferLineInfo.GetLineInfo(i);
                if (i == line) {
                    sbColumn = sbLine.Length + column;
                }
                sbLine.Append(lineInfo.StrLineOrg);
            }

            // テキスト中の文字範囲を検索
            int startColumnPick = sbColumn;
            int endColumnPick = sbColumn;

            string text = sbLine.ToString();
            ViewerCharType chTypeOrg = StringCategory.GetViewerCharType(text[sbColumn]);
            for (int i = sbColumn; i >= 0; i--) {
                ViewerCharType chType = StringCategory.GetViewerCharType(text[i]);
                if (chTypeOrg == chType) {
                    startColumnPick = i;
                } else {
                    break;
                }
            }
            for (int i = sbColumn; i < text.Length; i++) {
                ViewerCharType chType = StringCategory.GetViewerCharType(text[i]);
                if (chTypeOrg == chType) {
                    endColumnPick = i;
                } else {
                    break;
                }
            }
            endColumnPick++;

            // キーワードをピックアップ
            string keyword = text.Substring(startColumnPick, endColumnPick - startColumnPick);

            // 開始位置を検索
            int currentIndex = 0;
            startLine = -1;
            startColumn = -1;
            endLine = -1;
            endColumn = -1;
            for (int i = startLinePick; i <= endLinePick; i++) {
                TextBufferLogicalLineInfo lineInfo = m_parent.TextBufferLineInfo.GetLineInfo(i);
                if (currentIndex <= startColumnPick && startColumnPick < currentIndex + lineInfo.StrLineOrg.Length) {
                    startLine = i;
                    startColumn = startColumnPick - currentIndex;
                    break;
                }
                currentIndex += lineInfo.StrLineOrg.Length;
            }
            currentIndex = 0;
            for (int i = startLinePick; i <= endLinePick; i++) {
                TextBufferLogicalLineInfo lineInfo = m_parent.TextBufferLineInfo.GetLineInfo(i);
                if (currentIndex <= endColumnPick && endColumnPick <= currentIndex + lineInfo.StrLineOrg.Length) {
                    endLine = i;
                    endColumn = endColumnPick - currentIndex;
                    break;
                }
                currentIndex += lineInfo.StrLineOrg.Length;
            }

            return keyword;
        }

        //=========================================================================================
        // 機　能：マウスのボタンが押されたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseDown(MouseEventArgs evt) {
            int line;
            int column;
            bool onChar;
            GetLineColumnPosition(evt.X, evt.Y, out line, out column, out onChar);
            if (m_selectionRange != null && onChar && m_selectionRange.StartLine == m_selectionRange.EndLine &&
                    m_selectionRange.StartColumn < m_selectionRange.EndColumn && m_selectionRange.StartLine == line &&
                    m_selectionRange.StartColumn <= column && column <= m_selectionRange.EndColumn) {
                // 選択状態の上を再度クリック：自動検索
                TextBufferLogicalLineInfo lineInfo = m_parent.TextBufferLineInfo.GetLineInfo(line);
                string keyword = lineInfo.StrLineOrg.Substring(m_selectionRange.StartColumn, m_selectionRange.EndColumn - m_selectionRange.StartColumn);
                if (m_searchCondition == null) {
                    m_searchCondition = new TextSearchCondition();
                }
                bool trimmed;
                m_searchCondition.AutoSearchString = TextSearchCondition.TrimBySearchLength(keyword, out trimmed);
                m_parent.SearchEngine.SearchTextAutoNew(m_searchCondition);
            } else if (onChar) {
                // 文字列が見つかった
                int startLine;
                int startColumn;
                int endLine;
                int endColumn;
                string keyword = PickupKeyword(line, column, out startLine, out startColumn, out endLine, out endColumn);
                if (m_searchCondition == null) {
                    m_searchCondition = new TextSearchCondition();
                }
                bool trimmed;
                m_searchCondition.AutoSearchString = TextSearchCondition.TrimBySearchLength(keyword, out trimmed);
                m_parent.SearchEngine.SearchTextAutoNew(m_searchCondition);
            } else {
                // 表示中の自動検索を削除
                if (m_searchCondition != null) {
                    m_searchCondition.AutoSearchString = null;
                    m_parent.SearchEngine.SearchTextAutoNew(m_searchCondition);
                }
            }        

            // 新しい選択状態
            m_selectionRange = new TextViewerSelectionRange();
            m_selectionRange.StartLine = line;
            m_selectionRange.StartColumn = column;
            m_selectionRange.EndLine = line;
            m_selectionRange.EndColumn = column;
            m_selectionRange.FirstClickLine = line;
            m_selectionRange.FirstClickColumn = column;
            m_selectionRange.PrevLine = line;
            m_selectionRange.PrevColumn = column;
            m_parent.Invalidate();
            m_parent.FileViewerStatusBar.ResetSelectTextColumnCache();
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]mouseX  マウスカーソルのX位置
        // 　　　　[in]mouseY  マウスカーソルのY位置
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseMove(int mouseX, int mouseY) {
            if (m_selectionRange == null) {
                return;
            }
            int line;
            int column;
            bool onCharDummy;
            GetLineColumnPosition(mouseX, mouseY, out line, out column, out onCharDummy);
            
            // 場所の更新
            if (line < m_selectionRange.FirstClickLine || line == m_selectionRange.FirstClickLine && column <= m_selectionRange.FirstClickColumn) {
                m_selectionRange.StartLine = line;
                m_selectionRange.StartColumn = column;
                m_selectionRange.EndLine = m_selectionRange.FirstClickLine;
                m_selectionRange.EndColumn = m_selectionRange.FirstClickColumn;
            } else {
                m_selectionRange.StartLine = m_selectionRange.FirstClickLine;
                m_selectionRange.StartColumn = m_selectionRange.FirstClickColumn;
                m_selectionRange.EndLine = line;
                m_selectionRange.EndColumn = column;
            }

            // 再描画
            if (m_selectionRange.CheckFirstSelected()) {
                m_parent.Invalidate();
                m_parent.Update();
            } else {
                // 直前の選択を消す
                int origin = Math.Min(m_selectionRange.PrevLine, line) - ScrollYPosition;
                int height = Math.Abs(m_selectionRange.PrevLine - line);
                DoubleBuffer db = new DoubleBuffer(m_parent, m_parent.ClientRectangle.Width, (height + 1) * m_cyLineHeight);
                try {
                    TextViewerGraphics g = new TextViewerGraphics(db.DrawingGraphics, LineNoAreaWidth);
                    try {
                        g.Graphics.FillRectangle(SystemBrushes.Window, db.DrawingRectangle);
                        g.Graphics.TranslateTransform(0, -origin * m_cyLineHeight);
                        OnPaint(g, true, origin, origin + height);
                    } finally {
                        g.Dispose();
                    }
                } finally {
                    db.FlushScreen(0, origin * m_cyLineHeight);
                }
            }

            // スクロール
            m_viewerScrollImpl.MouseScroll(line);

            // ステータスバーを更新
            if (line != m_selectionRange.PrevLine || column != m_selectionRange.PrevColumn) {
                m_parent.FileViewerStatusBar.RefreshSelectionInfo(m_parent.TextBufferLineInfo, m_selectionRange, null);
            }

            // 現在位置を記憶
            m_selectionRange.PrevLine = line;
            m_selectionRange.PrevColumn = column;
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseUp(MouseEventArgs evt) {
            if (m_selectionRange == null || m_parent.IsDisposed) {
                return;
            }
            int line;
            int column;
            bool onChar;
            GetLineColumnPosition(evt.X, evt.Y, out line, out column, out onChar);
            if (line == m_selectionRange.FirstClickLine && column == m_selectionRange.FirstClickColumn) {
                m_selectionRange = null;
                m_parent.Invalidate();
                m_parent.FileViewerStatusBar.RefreshSelectionInfo(null, null, null);
            }
        }

        //=========================================================================================
        // 機　能：右クリックによりコンテキストメニューの表示指示が行われたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        public void OnContextMenu(MouseEventArgs evt) {
            // 項目を追加
            List<MenuItemSetting> menu = new List<MenuItemSetting>();
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpTopLineCommand)),             'T', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpBottomLineCommand)),          'B', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_JumpDirectCommand), -1),          'J', null));
            menu.Add(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CopyTextCommand)),                'C', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_CopyTextAsCommand)),              'F', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectAllCommand)),               'A', null));
            menu.Add(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectionCompareLeftCommand)),    'L', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectionCompareRightCommand)),   'R', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SelectionCompareDisplayCommand)), 'D', null));
            menu.Add(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchCommand)),                  'S', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchForwardNextCommand)),       'N', null));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchReverseNextCommand)),       'P', null));
            AddAutoSearchMenu(evt.X, evt.Y, menu);

            // コンテキストメニューを表示
            ContextMenuStrip cms = new ContextMenuStrip();
            MenuImpl menuImpl = new MenuImpl(UICommandSender.FileViewerMenu, m_parent);
            menuImpl.AddItemsFromSetting(cms, cms.Items, menu, Program.Document.KeySetting.FileViewerKeyItemList, false, null);
            menuImpl.RefreshToolbarStatus(new UIItemRefreshContext());
            m_parent.ContextMenuStrip = cms;
            m_parent.ContextMenuStrip.Show(m_parent, evt.Location);
            m_parent.ContextMenuStrip = null;
        }

        //=========================================================================================
        // 機　能：コンテキストメニューに自動検索の項目を追加する
        // 引　数：[in]mouseX  マウスのX位置
        // 　　　　[in]mouseY  マウスのY位置
        // 　　　　[in]menu    メニューを追加する変数
        // 戻り値：なし
        //=========================================================================================
        private void AddAutoSearchMenu(int mouseX, int mouseY, List<MenuItemSetting> menu) {
            int targetStartLine = -1;
            int targetStartColumn = -1;
            int targetEndColumn = -1;
            if (m_selectionRange != null && m_selectionRange.Selected) {
                // 範囲選択されているとき
                if (m_selectionRange.StartLine == m_selectionRange.EndLine) {
                    targetStartLine = m_selectionRange.StartLine;
                    targetStartColumn = m_selectionRange.StartColumn;
                    targetEndColumn = m_selectionRange.EndColumn;
                } else {
                    TextBufferLogicalLineInfo lineInfo = m_parent.TextBufferLineInfo.GetLineInfo(m_selectionRange.StartLine);
                    targetStartLine = m_selectionRange.StartLine;
                    targetStartColumn = m_selectionRange.StartColumn;
                    targetEndColumn = lineInfo.StrLineOrg.Length;
                }
            } else {
                // 自動ピックアップ
                int line;
                int column;
                bool onChar;
                GetLineColumnPosition(mouseX, mouseY, out line, out column, out onChar);
                if (onChar) {
                    // 文字列が見つかった
                    int targetEndLine;
                    PickupKeyword(line, column, out targetStartLine, out targetStartColumn, out targetEndLine, out targetEndColumn);
                    if (targetStartLine != targetEndLine) {
                        TextBufferLogicalLineInfo lineInfo  = m_parent.TextBufferLineInfo.GetLineInfo(targetStartLine);
                        targetEndColumn = lineInfo.StrLineOrg.Length;
                    }
                }
            }

            // 検索文字列を取得
            if (targetStartLine == -1) {
                return;
            }

            TextBufferLogicalLineInfo targetLineInfo = m_parent.TextBufferLineInfo.GetLineInfo(targetStartLine);
            string strSelected = targetLineInfo.StrLineOrg.Substring(targetStartColumn, targetEndColumn - targetStartColumn);
            if (strSelected.Length == 0) {
                return;
            }
            const int MAX_MENU_ITEM_STR = 10;
            string strKeywordSample = StringUtils.MakeOmittedString(strSelected, MAX_MENU_ITEM_STR);

            // メニュー項目を作成
            string itemNameForward = string.Format(Resources.MenuName_V_Context_SearchForward, strKeywordSample);
            string itemNameReverse = string.Format(Resources.MenuName_V_Context_SearchReverse, strKeywordSample);
            menu.Add(new MenuItemSetting(MenuItemSetting.ItemType.Separator));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchDirectCommand), true, strSelected, targetStartLine),  '*', itemNameForward));
            menu.Add(new MenuItemSetting(new ActionCommandMoniker(ActionCommandOption.None, typeof(V_SearchDirectCommand), false, strSelected, targetStartLine), '*', itemNameReverse));
        }

        //=========================================================================================
        // 機　能：選択をキャンセルする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CancelSelect() {
            m_selectionRange = null;
            m_parent.Invalidate();
            m_parent.FileViewerStatusBar.RefreshSelectionInfo(null, null, null);
        }

        //=========================================================================================
        // 機　能：指定された座標に対する行と桁の位置を返す
        // 引　数：[in]mouseX  マウスのX位置
        // 　　　　[in]mouseY  マウスのY位置
        // 　　　　[out]line   行位置を返す変数
        // 　　　　[out]column カラム位置を返す変数
        // 　　　　[out]onChar 文字の上の位置にいるときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        private void GetLineColumnPosition(int mouseX, int mouseY, out int line, out int column, out bool onChar) {
            int mouseLine = mouseY / m_cyLineHeight + ScrollYPosition;
            int maxLine = m_parent.TextBufferLineInfo.LogicalLineCount;
            if (mouseLine < 0 || maxLine == 0) {
                // 行開始前
                line = 0;
                column = 0;
                onChar = false;
            } else if (mouseLine >= maxLine) {
                // 行終了後
                line = maxLine - 1;
                TextBufferLogicalLineInfo lineInfo = m_parent.TextBufferLineInfo.GetLineInfo(line);
                column = lineInfo.StrLineOrg.Length;
                onChar = false;
            } else if (mouseX < m_xPosContents - ScrollXPosition) {
                // 桁開始前
                line = mouseLine;
                column = 0;
                onChar = false;
            } else {
                // 各行の文字単位判定
                line = mouseLine;
                TextBufferLogicalLineInfo lineInfo = m_parent.TextBufferLineInfo.GetLineInfo(line);
                TextViewerGraphics g = new TextViewerGraphics(m_parent, LineNoAreaWidth);
                try {
                    float xPos = mouseX - m_xPosContents + ScrollXPosition;
                    GetColumnPosition(g, lineInfo, xPos, out column, out onChar);
                } finally {
                    g.Dispose();
                }
            }
        }

        //=========================================================================================
        // 機　能：指定されたX位置に対する桁の位置を返す
        // 引　数：[in]g       グラフィックス
        // 　　　　[in]lineInfo 行情報
        // 　　　　[in]xPos     表示X座標（スクロール補正済み、1文字目の左側が0）
        // 　　　　[out]column カラム位置を返す変数
        // 　　　　[out]onChar 文字の上の位置にいるときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        private void GetColumnPosition(TextViewerGraphics g, TextBufferLogicalLineInfo lineInfo, float xPos, out int column, out bool onChar) {
            string strOrg = lineInfo.StrLineOrg;
            if (strOrg.Length == 0 || xPos < 0) {
                column = 0;
                onChar = false;
                return;
            }
            if (strOrg.IndexOf((char)0xfffd) != -1) {       // 不明な文字は誤動作するため「.」に置換
                strOrg = strOrg.Replace((char)0xfffd, '.');
            }

            string strDisp;
            List<int> orgToTab, tabToOrg;
            m_parent.TextBufferLineInfo.ExtractTab(g, strOrg, out strDisp, out orgToTab, out tabToOrg);

            int strIndexStart = 0;
            int strIndexEnd = strDisp.Length;
            while (true) {
                // 分割方法を決定
                // 文字列中の調査領域を最大10分割
                const int DIV_STRING = 10;               // 1回での領域調査の分割数
                CharacterRange[] rangeList = new CharacterRange[Math.Min(DIV_STRING, strIndexEnd - strIndexStart)];
                int rangeStep = (strIndexEnd - strIndexStart) / rangeList.Length;
                for (int i = 0; i < rangeList.Length; i++) {
                    rangeList[i] = new CharacterRange(strIndexStart + i * rangeStep, rangeStep);
                }
                rangeList[rangeList.Length - 1] = new CharacterRange(rangeList[rangeList.Length - 1].First, strIndexEnd - rangeList[rangeList.Length - 1].First);

                // 領域を計測
                StringFormat sf = new StringFormat();
                sf.SetMeasurableCharacterRanges(rangeList);
                Region[] charRegion = g.Graphics.MeasureCharacterRanges(strDisp + ".", g.TextFont, new RectangleF(0, 0, GraphicsUtils.INFINITY_WIDTH, GraphicsUtils.INFINITY_WIDTH), sf);
                sf.Dispose();

                // 位置を比較
                if (xPos < charRegion[0].GetBounds(g.Graphics).Left) {
                    column = tabToOrg[rangeList[0].First];
                    onChar = false;
                    return;
                }
                bool found = false;
                for (int i = 0; i < charRegion.Length; i++) {
                    if (xPos < charRegion[i].GetBounds(g.Graphics).Right) {
                        if (rangeList[i].Length == 1) {
                            column = tabToOrg[rangeList[i].First];
                            onChar = true;
                            return;
                        } else {
                            found = true;
                            strIndexStart = rangeList[i].First;
                            strIndexEnd = strIndexStart + rangeList[i].Length;
                            break;
                        }
                    }
                }
                if (!found) {
                    if (strIndexEnd == strDisp.Length && xPos >= charRegion[charRegion.Length - 1].GetBounds(g.Graphics).Right) {
                        column = strOrg.Length;
                        onChar = false;
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：指定された物理行を先頭に表示する
        // 引　数：[in]physicalLine  物理行
        // 戻り値：なし
        //=========================================================================================
        public void MoveSpecifiedPhysicalLineNoOnTop(int physicalLine) {
            int dispLine = m_parent.TextBufferLineInfo.GetLineNumberFromPhysicalLineNumber(physicalLine);
            MoveDisplayPosition(dispLine - ScrollYPosition);
        }
        
        //=========================================================================================
        // 機　能：指定された論理行を画面上に表示する
        // 引　数：[in]line             論理行
        // 　　　　[in]invalidateAlways 常に再描画するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void MoveSpecifiedLogicalLineNoOnScreen(int line, bool invalidateAlways) {
            int newPosition = ScrollYPosition;
            if (line < ScrollYPosition ||  ScrollYPosition + ScrollYCompleteLineSize < line) {
                newPosition = line - ScrollYCompleteLineSize / 2;
            }
            if (newPosition != ScrollYPosition) {
                newPosition = Math.Max(0, Math.Min(newPosition, ScrollCyNeed - ScrollYCompleteLineSize));
                ScrollYPosition = newPosition;
                UpdateLinePosition();
                m_parent.Invalidate();
            } else {
                if (invalidateAlways) {
                    m_parent.Invalidate();
                }
            }
        }

        //=========================================================================================
        // 機　能：行位置を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void UpdateLinePosition() {
            SetVerticalScrollbar();
            m_parent.FileViewerStatusBar.RefreshLineNo();
        }
        
        //=========================================================================================
        // 機　能：すべて選択する
        // 引　数：なし
        // 戻り値：選択に成功したときtrue
        //=========================================================================================
        public bool SelectAll() {
            int lineCount = m_parent.TextBufferLineInfo.LogicalLineCount;
            if (lineCount == 0) {
                return false;
            }
            TextBufferLogicalLineInfo lineInfo = m_parent.TextBufferLineInfo.GetLineInfo(lineCount - 1);
            m_selectionRange = new TextViewerSelectionRange();
            m_selectionRange.StartLine = 0;
            m_selectionRange.StartColumn = 0;
            m_selectionRange.EndLine = lineCount - 1;
            m_selectionRange.EndColumn = lineInfo.StrLineOrg.Length;
            m_selectionRange.FirstClickLine = 0;
            m_selectionRange.FirstClickColumn = 0;
            m_selectionRange.PrevLine = 0;
            m_selectionRange.PrevColumn = 0;
            m_parent.Invalidate();
            m_parent.FileViewerStatusBar.RefreshSelectionInfo(m_parent.TextBufferLineInfo, m_selectionRange, null);
            return true;
        }

        //=========================================================================================
        // 機　能：選択位置をクリップボードにコピーする
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public bool CopyText() {
            if (m_selectionRange == null || !m_selectionRange.Selected) {
                return false;
            }
            CopyTextClipboard(TextClipboardSetting.GetDefaultSetting());

            // 選択範囲を点滅
            TextViewerSelectionRange tempSel = m_selectionRange;
            m_selectionRange = null;
            m_parent.Invalidate();
            m_parent.Update();
            Thread.Sleep(50);
            m_selectionRange = tempSel;
            m_parent.Invalidate();
            m_parent.Update();

            return true;
        }

        //=========================================================================================
        // 機　能：選択位置を形式を指定してクリップボードにコピーする
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        public bool CopyTextAs() {
            if (m_selectionRange == null || !m_selectionRange.Selected) {
                return false;
            }
            TextClipboardSetting setting;
            if (Configuration.Current.TextClipboardSettingDefault == null) {
                setting = (TextClipboardSetting)(Program.Document.UserGeneralSetting.TextClipboardSetting.Clone());
            } else {
                setting = (TextClipboardSetting)(Configuration.Current.TextClipboardSettingDefault.Clone());
            }
            ViewerTextCopyAsDialog dialog = new ViewerTextCopyAsDialog(setting);
            DialogResult result = dialog.ShowDialog(m_parent);
            if (result != DialogResult.OK) {
                return false;
            }
            setting = dialog.Setting;
            Program.Document.UserGeneralSetting.TextClipboardSetting = (TextClipboardSetting)(setting.Clone());
            CopyTextClipboard(setting);

            return false;
        }

        //=========================================================================================
        // 機　能：選択位置を指定形式でクリップボードにコピーする
        // 引　数：[in]setting  フォーマット形式
        // 戻り値：なし
        //=========================================================================================
        private void CopyTextClipboard(TextClipboardSetting setting) {
            // テキストを用意
            string text;
            if (setting.TabMode == TextClipboardSetting.CopyTabMode.Original) {
                text = GetTextClipboardOriginalTab(setting.LineBreakMode);
            } else {
                TextViewerGraphics g = new TextViewerGraphics(m_parent, 0);
                try {
                    text = GetTextClipboardConvertTab(setting.LineBreakMode, g);
                } finally {
                    g.Dispose();
                }
            }

            // クリップボードにコピー
            Clipboard.SetDataObject(text, true);
        }

        //=========================================================================================
        // 機　能：オリジナルのタブ状態を維持して選択位置を文字列で返す
        // 引　数：[in]lineBreak  改行モード
        // 戻り値：作成した文字列
        //=========================================================================================
        public string GetTextClipboardOriginalTab(TextClipboardSetting.CopyLineBreakMode lineBreak) {
            string text;
            if (m_selectionRange.StartLine == m_selectionRange.EndLine) {
                TextBufferLogicalLineInfo line = m_parent.TextBufferLineInfo.GetLineInfo(m_selectionRange.StartLine);
                int start = m_selectionRange.StartColumn;
                int end = m_selectionRange.EndColumn;
                text = line.StrLineOrg.Substring(start, end - start);
            } else {
                StringBuilder sb = new StringBuilder();

                // 1行目
                TextBufferLogicalLineInfo lineStart = m_parent.TextBufferLineInfo.GetLineInfo(m_selectionRange.StartLine);
                sb.Append(lineStart.StrLineOrg.Substring(m_selectionRange.StartColumn));
                sb.Append(GetReturnCode(lineStart.LineBreakChar, lineBreak, lineStart, m_parent.TextBufferLineInfo.GetLineInfo(m_selectionRange.StartLine + 1)));

                // 2行目以降
                for (int i = m_selectionRange.StartLine + 1; i <= m_selectionRange.EndLine - 1; i++) {
                    TextBufferLogicalLineInfo lineMiddle = m_parent.TextBufferLineInfo.GetLineInfo(i);
                    sb.Append(lineMiddle.StrLineOrg);
                    sb.Append(GetReturnCode(lineMiddle.LineBreakChar, lineBreak, lineMiddle, m_parent.TextBufferLineInfo.GetLineInfo(i + 1)));
                }

                // 最終行
                TextBufferLogicalLineInfo lineEnd = m_parent.TextBufferLineInfo.GetLineInfo(m_selectionRange.EndLine);
                sb.Append(lineEnd.StrLineOrg.Substring(0, m_selectionRange.EndColumn));
                text = sb.ToString();
            }
            return text;
        }

        //=========================================================================================
        // 機　能：タブを空白に変換して選択位置を文字列で返す
        // 引　数：[in]lineBreak  改行モード
        // 　　　　[in]g          全角半角の判定に使用するグラフィックス
        // 戻り値：作成した文字列
        //=========================================================================================
        private string GetTextClipboardConvertTab(TextClipboardSetting.CopyLineBreakMode lineBreak, TextViewerGraphics g) {
            string text;
            if (m_selectionRange.StartLine == m_selectionRange.EndLine) {
                TextBufferLogicalLineInfo line = m_parent.TextBufferLineInfo.GetLineInfo(m_selectionRange.StartLine);
                string dispStr;
                List<int> orgToTab, tabToOrg;
                m_parent.TextBufferLineInfo.ExtractTab(g, line.StrLineOrg, out dispStr, out orgToTab, out tabToOrg);
                int start = orgToTab[m_selectionRange.StartColumn];
                int end = orgToTab[m_selectionRange.EndColumn];
                text = dispStr.Substring(start, end - start);
            } else {
                StringBuilder sb = new StringBuilder();

                // 1行目
                TextBufferLogicalLineInfo lineStart = m_parent.TextBufferLineInfo.GetLineInfo(m_selectionRange.StartLine);
                string firstDispStr;
                List<int> firstOrgToTab, firstTabToOrg;
                m_parent.TextBufferLineInfo.ExtractTab(g, lineStart.StrLineOrg, out firstDispStr, out firstOrgToTab, out firstTabToOrg);
                int firstStart = firstOrgToTab[m_selectionRange.StartColumn];
                int firstEnd = firstDispStr.Length;
                string firstText = firstDispStr.Substring(firstStart, firstEnd - firstStart);
                sb.Append(firstText);
                sb.Append(GetReturnCode(lineStart.LineBreakChar, lineBreak, lineStart, m_parent.TextBufferLineInfo.GetLineInfo(m_selectionRange.StartLine + 1)));

                // 2行目以降
                for (int i = m_selectionRange.StartLine + 1; i <= m_selectionRange.EndLine - 1; i++) {
                    TextBufferLogicalLineInfo lineMiddle = m_parent.TextBufferLineInfo.GetLineInfo(i);
                    string middleDispStr;
                    List<int> middleOrgToTab, middleTabToOrg;
                    m_parent.TextBufferLineInfo.ExtractTab(g, lineMiddle.StrLineOrg, out middleDispStr, out middleOrgToTab, out middleTabToOrg);
                    sb.Append(middleDispStr);
                    sb.Append(GetReturnCode(lineMiddle.LineBreakChar, lineBreak, lineMiddle, m_parent.TextBufferLineInfo.GetLineInfo(i + 1)));
                }

                // 最終行
                TextBufferLogicalLineInfo endLine = m_parent.TextBufferLineInfo.GetLineInfo(m_selectionRange.EndLine);
                string endDispStr;
                List<int> endOrgToTab, endTabToOrg;
                m_parent.TextBufferLineInfo.ExtractTab(g, endLine.StrLineOrg, out endDispStr, out endOrgToTab, out endTabToOrg);
                sb.Append(endDispStr.Substring(0, endOrgToTab[m_selectionRange.EndColumn]));
                text = sb.ToString();
            }
            return text;
        }

        //=========================================================================================
        // 機　能：改行コードを文字列にして返す
        // 引　数：[in]lineBreakChar  テキスト中の文字コード
        // 　　　　[in]breakSetting   改行コードの設定
        // 　　　　[in]currentInfo    現在行の情報
        // 　　　　[in]nextInfo       次の行の情報
        // 戻り値：改行コードの文字列
        //=========================================================================================
        private string GetReturnCode(LineBreakChar lineBreakChar, TextClipboardSetting.CopyLineBreakMode breakSetting, TextBufferLogicalLineInfo currentInfo, TextBufferLogicalLineInfo nextInfo) {
            if (currentInfo.PhysicalLineNo == nextInfo.PhysicalLineNo) {
                return "";
            }

            string code;
            if (breakSetting == TextClipboardSetting.CopyLineBreakMode.Original) {
                if (lineBreakChar == LineBreakChar.Cr) {
                    code = "\n";
                } else {
                    code = "\r\n";
                }
            } else if(breakSetting == TextClipboardSetting.CopyLineBreakMode.Cr) {
                code = "\n";
            } else {
                code = "\r\n";
            }
            return code;
        }

        //=========================================================================================
        // 機　能：テキストを検索する
        // 引　数：[in]condition  検索条件（前回の条件を維持するときnull）
        // 　　　　[in]forward    前方検索するときtrue、後方検索するときfalse
        // 　　　　[in]startLine  検索開始行（-1のとき画面端または前回の続きから）
        // 戻り値：なし
        //=========================================================================================
        public void SearchText(TextSearchCondition condition, bool forward, int startLine) {
            if (condition != null) {
                m_searchCondition = condition;
            }
            if (m_searchCondition == null) {
                return;
            }
            if (m_searchCondition == null || m_searchCondition.SearchString == null) {
                // 検索クリア
                m_parent.SearchEngine.ClearSearchResult();
                m_parent.NotifySearchEnd(true);
                m_parent.Invalidate();
            } else {
                // カーソル位置を決定
                if (startLine != -1) {
                    // 指定がある場合
                    ScrollSearchCursorLine = startLine;
                    if (forward) {
                        ScrollSearchCursorLine++;
                    } else {
                        ScrollSearchCursorLine--;
                    }
                    ScrollSearchCursorLine = Math.Min(Math.Max(0, ScrollSearchCursorLine), m_parent.TextBufferLineInfo.LogicalLineCount);
                } else {
                    // 前回位置が未決定のときは画面の端から検索
                    if (ScrollSearchCursorLine == -1 || condition != null) {
                        if (forward) {
                            ScrollSearchCursorLine = ScrollYPosition;
                        } else {
                            ScrollSearchCursorLine = Math.Min(ScrollYPosition + ScrollYCompleteLineSize, m_parent.TextBufferLineInfo.LogicalLineCount - 1);
                        }
                    }
                }

                // 検索を実行
                int hitLine;
                WaitCursor waitCursor = new WaitCursor();
                try {
                    if (condition != null) {
                        // 新規検索
                        hitLine = m_parent.SearchEngine.SearchTextNew(condition, forward, ScrollSearchCursorLine);
                    } else {
                        // 前回検索の続き
                        hitLine = m_parent.SearchEngine.SearchTextNext(forward, ScrollSearchCursorLine);
                    }
                } finally {
                    waitCursor.Dispose();
                }

                // 検索結果を反映
                if (hitLine == -1) {
                    if (condition != null) {
                        ScrollSearchCursorLine = -1;
                    }
                    m_parent.ShowStatusbarMessage(Resources.FileViewer_SearchNotFound + m_searchCondition.SearchString, FileOperationStatus.LogLevel.Info, IconImageListID.FileViewer_SearchGeneric);
                } else {
                    ScrollSearchCursorLine = hitLine;
                    MoveSpecifiedLogicalLineNoOnScreen(hitLine, true);
                }
            }
        }

        //=========================================================================================
        // 機　能：タブ幅を変更する
        // 引　数：[in]tab  新しいタブ幅
        // 戻り値：タブ幅の変更に成功したときtrue
        //=========================================================================================
        public bool SetTab(int tab) {
            if (!m_parent.TextBufferLineInfo.CompletedLoading) {
                m_parent.ShowStatusbarMessage(Resources.FileViewer_CannotRunCopyTextAs, FileOperationStatus.LogLevel.Info, IconImageListID.None);
                return false;
            }
            m_parent.TextBufferLineInfo.TabWidth = tab;
            ResetLineInfo(false);
            return true;
        }

        //=========================================================================================
        // 機　能：エンコードを変更する
        // 引　数：[in]encoding  新しいエンコード
        // 戻り値：エンコードの変更に成功したときtrue
        //=========================================================================================
        public bool SetEncoding(EncodingType encoding) {
            if (!m_parent.TextBufferLineInfo.CompletedLoading) {
                m_parent.ShowStatusbarMessage(Resources.FileViewer_CannotRunCopyTextAs, FileOperationStatus.LogLevel.Info, IconImageListID.None);
                return false;
            }
            m_parent.TextBufferLineInfo.TextEncodingType = encoding;
            ResetLineInfo(false);
            return true;
        }

        //=========================================================================================
        // 機　能：改行幅をセットする
        // 引　数：[in]setting  改行幅の設定
        // 戻り値：改行幅の変更に成功したときtrue
        //=========================================================================================
        public bool SetLineWidth(TextViewerLineBreakSetting setting) {
            if (!m_parent.TextBufferLineInfo.CompletedLoading) {
                m_parent.ShowStatusbarMessage(Resources.FileViewer_CannotRunCopyTextAs, FileOperationStatus.LogLevel.Info, IconImageListID.None);
                return false;
            }
            m_parent.TextBufferLineInfo.LineBreakSetting = setting;
            ResetLineInfo(true);
            return true;
        }

        //=========================================================================================
        // 機　能：行情報をリセットする
        // 引　数：[in]resetWidth  画面幅も初期化するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void ResetLineInfo(bool resetWidth) {
            WaitCursor waitCursor = new WaitCursor();
            try {
                int topPhysicalLineNo = m_parent.TextBufferLineInfo.GetLineInfo(ScrollYPosition).PhysicalLineNo;
                m_selectionRange = null;
                m_parent.TextBufferLineInfo.ResetParse(resetWidth);
                if (m_searchCondition != null) {
                    m_parent.SearchEngine.SearchTextNew(m_searchCondition, true, 0);
                }
                m_parent.FileViewerStatusBar.RefreshTextInfo();

                topPhysicalLineNo = Math.Min(m_parent.TextBufferLineInfo.PhysicalLineCount, topPhysicalLineNo);
                MoveSpecifiedPhysicalLineNoOnTop(topPhysicalLineNo);
                m_viewerScrollImpl.OnSizeChange();
                m_parent.Invalidate();
            } finally {
                waitCursor.Dispose();
            }
        }

        //=========================================================================================
        // プロパティ：画面右端の限界ピクセル位置
        //=========================================================================================
        private float TextRightLimitBorder {
            get {
                float maxText = m_parent.TextBufferLineInfo.MaxTextPixelWidth;
                return m_xPosContents + maxText - ScrollXPosition;
            }
        }

        //=========================================================================================
        // プロパティ：水平スクロールに必要な幅
        //=========================================================================================
        public int ScrollCxNeed {
            get {
                return (int)(m_parent.TextBufferLineInfo.MaxTextPixelWidth + m_xPosContents + CX_DISPLAY_LEFT_MARGIN);
            }
        }
        
        //=========================================================================================
        // プロパティ：水平スクロールバーの位置
        //=========================================================================================
        public int ScrollXPosition {
            get {
                return m_scrollXPosition;
            }
            set {
                m_scrollXPosition = value;
            }
        }

        //=========================================================================================
        // プロパティ：垂直スクロールに必要な高さ
        //=========================================================================================
        public int ScrollCyNeed {
            get {
                return m_parent.TextBufferLineInfo.LogicalLineCount;
            }
        }

        //=========================================================================================
        // プロパティ：垂直スクロールバーの位置
        //=========================================================================================
        public int ScrollYPosition {
            get {
                return m_scrollYPosition;
            }
            set {
                m_scrollYPosition = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：画面上に完全に表示できる行数
        //=========================================================================================
        public int ScrollYCompleteLineSize {
            get {
                return m_scrollYCompleteLineSize;
            }
            set {
                m_scrollYCompleteLineSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：検索時のカーソル行（-1:表示しない）
        //=========================================================================================
        public int ScrollSearchCursorLine {
            get {
                return m_scrollSearchCursorLine;
            }
            set {
                m_scrollSearchCursorLine = value;
            }
        }

        //=========================================================================================
        // プロパティ：行番号表示領域の幅
        //=========================================================================================
        public float LineNoAreaWidth {
            get {
                return m_xPosSeparator;
            }
        }

        //=========================================================================================
        // プロパティ：画面上に表示可能な行数行番号表示領域の幅
        //=========================================================================================
        public int CompleteLineSize {
            get {
                return ScrollYCompleteLineSize;
            }
        }

        //=========================================================================================
        // プロパティ：先頭行に表示している表示上の行数
        //=========================================================================================
        public int VisibleTopLine {
            get {
                return m_parent.TextBufferLineInfo.GetLineInfo(ScrollYPosition).PhysicalLineNo;
            }
        }

        //=========================================================================================
        // プロパティ：選択範囲（選択していないときnull）
        //=========================================================================================
        public TextViewerSelectionRange SelectionRange {
            get {
                return m_selectionRange;
            }
        }

        //=========================================================================================
        // プロパティ：検索条件（検索していないときnull）
        //=========================================================================================
        public TextSearchCondition SearchCondition {
            get {
                return m_searchCondition;
            }
        }
    }
}
