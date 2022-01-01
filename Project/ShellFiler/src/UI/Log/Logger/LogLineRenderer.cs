using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.UI.Log.Logger {

    //=========================================================================================
    // クラス：ログの行内容を描画するクラス
    //=========================================================================================
    public class LogLineRenderer {
        // 左右のマージン
        public const int CX_LEFT_RIGHT_MARGIN = 4;

        // ファイル情報：リーダーの最小幅
        public const int CX_LEADER = 16;

        // ファイル情報：ファイル名からリーダーへのマージン
        public const int CX_LEADER_MARGIN = 4;


        // メッセージの内容
        private string m_message;

        // 拡張となるリンクメッセージ（メッセージを出力しないときnull）
        private string m_extendLink;

        // リンク情報を表示した領域
        private Rectangle m_rcLink;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]message     メッセージの内容
        // 　　　　[in]extendLink  拡張となるリンクメッセージ（リンクメッセージを出力しないときnull）
        // 　　　　[in]g           リンクの領域計算用のグラフィックス（リンクメッセージを出力しないときnull）
        // 　　　　[in]fontLink    拡張となるリンクメッセージを出力するフォント（リンクメッセージを出力しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public LogLineRenderer(string message, string extendLink, Graphics g, Font fontLink) {
            m_message = message;
            m_extendLink = extendLink;

            if (extendLink != null) {
                const int CX_LINK_MARGIN = 8;
                int xPosLink = CX_LEFT_RIGHT_MARGIN + (int)(GraphicsUtils.MeasureString(g, fontLink, m_message) + CX_LINK_MARGIN);
                int cxLink = (int)(GraphicsUtils.MeasureString(g, fontLink, m_extendLink));
                m_rcLink = new Rectangle(xPosLink, 0, cxLink, 0);
            }
        }
        
        //=========================================================================================
        // 機　能：ログの内容を描画する
        // 引　数：[in]g            グラフィックス
        // 　　　　[in]brush        通常時に使用するフォント
        // 　　　　[in]scrLine      表示する行位置（画面上の一番上が0）
        // 　　　　[in]logPanel     表示するウィンドウの情報
        // 　　　　[in]lineContext  行単位の描画のコンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public void DrawLogLine(LogGraphics g, Brush brush, int scrLine, ILogViewContainer logPanel, DrawingLogLineContext lineContext) {
            // 描画位置を決定
            Font font = g.LogWindowFont;
            Rectangle rcClient = logPanel.View.ClientRectangle;
            int logHeight = logPanel.LogLineHeight;
            int yPos = scrLine * logHeight;
            int x1 = CX_LEFT_RIGHT_MARGIN;
            int cx = logPanel.View.ClientRectangle.Width - CX_LEFT_RIGHT_MARGIN * 2;

            for (int i = 0; i < 2; i++) {
                // 0:通常、1:選択
                int last;
                if (m_extendLink == null) {
                    last = m_message.Length;
                } else {
                    last = m_message.Length + 2;
                }
                if (i == 0 && lineContext.SelectionStart == 0 && lineContext.SelectionEnd >= last) {
                    continue;
                } else if (i == 1 && (lineContext.SelectionStart == -1 || lineContext.SelectionStart == lineContext.SelectionEnd)) {
                    continue;
                }

                // リソースを選択
                Brush backBrush, fontBrush;
                if (i == 0) {
                    backBrush = null;
                    fontBrush = brush;
                } else {
                    backBrush = g.LogWindowSelectBackBrush;
                    fontBrush = g.LogWindowSelectTextBrush;
                }

                // 領域を選択
                if (i == 0 && (lineContext.SelectionStart == -1 || lineContext.SelectionStart == lineContext.SelectionEnd)) {
                    ;
                } else {
                    int xStart = SelectToPoint(g.Graphics, font, lineContext.SelectionStart);
                    int xEnd = SelectToPoint(g.Graphics, font, lineContext.SelectionEnd);
                    g.Graphics.SetClip(rcClient, CombineMode.Exclude);
                    if (i == 0) {
                        g.Graphics.SetClip(new Rectangle(0, yPos, xStart - 1, logHeight), CombineMode.Xor);
                        g.Graphics.SetClip(new Rectangle(xEnd + 1, yPos, rcClient.Width - xEnd, logHeight), CombineMode.Xor);
                    } else {
                        g.Graphics.SetClip(new Rectangle(xStart, yPos, xEnd - xStart + 1, logHeight), CombineMode.Xor);
                    }
                }

                // 描画
                if (i == 1) {
                    g.Graphics.FillRectangle(backBrush, new Rectangle(0, yPos, rcClient.Width, logHeight));
                }
                g.Graphics.DrawString(m_message, font, fontBrush, new RectangleF(x1, yPos, cx, logHeight), g.StringFormatEllipsis);

                // リンク
                if (m_extendLink != null) {
                    Brush brushLink = g.LogWindowLinkTextBrush;
                    Rectangle rcClickDraw = new Rectangle(m_rcLink.X, yPos, m_rcLink.Width, font.Height);
                    g.Graphics.DrawString(m_extendLink, g.LogWindowLinkFont, brushLink, rcClickDraw, g.StringFormatNormal);
                }
                g.Graphics.SetClip(rcClient);
            }
        }

        //=========================================================================================
        // 機　能：選択位置の座標を返す
        // 引　数：[in]g       桁位置計測用のグラフィックス
        // 　　　　[in]font    桁位置計測用のフォント
        // 　　　　[in]select  選択した桁
        // 戻り値：選択位置の座標
        //=========================================================================================
        private int SelectToPoint(Graphics g, Font font, int select) {
            int xPos;
            if (select <= m_message.Length) {
                string sub = m_message.Substring(0, select);
                xPos = CX_LEFT_RIGHT_MARGIN + TextRendererUtils.MeasureStringJustInt(g, font, sub);
            } else {
                if (m_extendLink == null) {
                    xPos = CX_LEFT_RIGHT_MARGIN + TextRendererUtils.MeasureStringJustInt(g, font, m_message);
                } else {
                    if (select == m_message.Length + 1) {
                        xPos = m_rcLink.X;
                    } else {
                        xPos = m_rcLink.X + m_rcLink.Width;
                    }
                }
            }
            return xPos;
        }

        //=========================================================================================
        // 機　能：クリック位置の桁情報を返す
        // 引　数：[in]logPanel   ログパネル
        // 　　　　[in]g          桁位置計測用のグラフィックス
        // 　　　　[in]cursorPos  マウスカーソルの座標
        // 　　　　[out]column    カラム位置を返す変数
        // 　　　　[out]onChar    文字の上の位置にいるときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        public void GetMouseHitColumn(ILogViewContainer logPanel, LogGraphics g, Point cursorPos, out int column, out bool onChar) {
            float xPos = cursorPos.X;
            if (m_message.Length == 0 || xPos < 0) {
                column = 0;
                onChar = false;
                return;
            }

            if (m_extendLink != null) {
                if (m_rcLink.Left <= xPos && xPos <= m_rcLink.Right) {
                    column = m_message.Length + 1;
                    onChar = true;
                    return;
                } else if (xPos > m_rcLink.Right) {
                    column = m_message.Length + 2;
                    onChar = false;
                    return;
                }
            }

            string message = m_message;
            if (message.IndexOf((char)0xfffd) != -1) {       // 不明な文字は誤動作するため「.」に置換
                message = message.Replace((char)0xfffd, '.');
            }

            int strIndexStart = 0;
            int strIndexEnd = message.Length;
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
                Region[] charRegion = g.Graphics.MeasureCharacterRanges(message + ".", g.LogWindowFixedFont, new RectangleF(0, 0, GraphicsUtils.INFINITY_WIDTH, GraphicsUtils.INFINITY_WIDTH), sf);
                sf.Dispose();

                // 位置を比較
                if (xPos < charRegion[0].GetBounds(g.Graphics).Left) {
                    column = rangeList[0].First;
                    onChar = false;
                    return;
                }
                bool found = false;
                for (int i = 0; i < charRegion.Length; i++) {
                    if (xPos < charRegion[i].GetBounds(g.Graphics).Right) {
                        if (rangeList[i].Length == 1) {
                            column = rangeList[i].First;
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
                    if (strIndexEnd == message.Length && xPos >= charRegion[charRegion.Length - 1].GetBounds(g.Graphics).Right) {
                        column = m_message.Length;
                        onChar = false;
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // プロパティ：リンク情報を表示した領域
        //=========================================================================================
        public Rectangle LinkRectangle {
            get {
                return m_rcLink;
            }
        }
    }
}
