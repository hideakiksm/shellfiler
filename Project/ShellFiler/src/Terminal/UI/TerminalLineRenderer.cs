using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Locale;
using ShellFiler.Util;
using ShellFiler.Terminal.TerminalSession;
using ShellFiler.UI.Log;

namespace ShellFiler.Terminal.UI {

    //=========================================================================================
    // クラス：ログ行のレンダリングを行うクラス
    //=========================================================================================
    public class TerminalLineRenderer {
        // メッセージの内容（未登録状態のときnull：選択状態にもならない）
        private string m_message;

        // メッセージに対応する色（ConsoleDecorationの値、m_message.Lengthと同じ長さ、未登録状態のときnull）
        private short[] m_decoration;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]message     表示するメッセージ
        // 　　　　[in]decoration  メッセージに対応する色
        // 戻り値：なし
        //=========================================================================================
        public TerminalLineRenderer(string message, short[] decoration) {
            m_message = message;
            m_decoration = decoration;
        }

        //=========================================================================================
        // 機　能：ログの内容を描画する
        // 引　数：[in]g            グラフィックス
        // 　　　　[in]scrLine      表示する行位置（画面上の一番上が0）
        // 　　　　[in]logPanel     表示するウィンドウの情報
        // 　　　　[in]lineContext  行単位の描画のコンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public void DrawLogLine(LogGraphics g, int scrLine, ILogViewContainer logPanel, DrawingLogLineContext lineContext) {
            // 描画位置を決定
            int logHeight = logPanel.LogLineHeight;
            int yPos = scrLine * logHeight;
            Rectangle rcClient = logPanel.View.ClientRectangle;

            // 空行を描画
            if (m_message == null || m_decoration.Length == 0) {
                Rectangle rcLine = new Rectangle(0, yPos, rcClient.Height, logHeight);
                g.Graphics.FillRectangle(g.LogWindowBackBrush, rcLine);
                return;
            }

            // 色情報を整理
            string message = ResolveHide(m_decoration, m_message);
            short[] decoFore, decoBack;
            short foreColorDefault = ConsoleDecoration.COLOR_DEF_FORE;
            short backColorDefault = (ConsoleDecoration.COLOR_DEF_BACK << ConsoleDecoration.BACK_SHIFT);
            ResolveColor(m_decoration, lineContext, foreColorDefault, backColorDefault, out decoFore, out decoBack);

            // 描画
            DrawBack(g, message, decoBack, yPos, logHeight, rcClient);
            DrawString(g, message, decoFore, yPos, logHeight, rcClient);
        }

        //=========================================================================================
        // 機　能：ログ１行分の背景を描画する
        // 引　数：[in]g          グラフィックス
        // 　　　　[in]message    表示するメッセージ
        // 　　　　[in]decoBack   表示するメッセージの修飾情報（背景色の要素だけに限定）
        // 　　　　[in]yPos       表示するY位置
        // 　　　　[in]logHeight  ログの行の高さ[ピクセル]
        // 　　　　[in]rcClient   クライアント領域の大きさ
        // 戻り値：なし
        //=========================================================================================
        private void DrawBack(LogGraphics g, string message, short[] decoBack, int yPos, int logHeight, Rectangle rcClient) {
            // 変換点を抽出
            int[] backDiffPoint = GetDecorationDiffPoint(message, decoBack);
            Font baseFont = g.LogWindowFixedFont;
            float[] backBorder = TextRendererUtils.MeasureStringRegion(g.Graphics, baseFont, message, backDiffPoint);
            HashSet<short> renderBack = ConvertDecorationToRenderCommand(decoBack);

            // 描画
            foreach (short deco in renderBack) {
                g.Graphics.SetClip(rcClient, CombineMode.Exclude);
                RectangleF[] targetRegion = ConvertBorderToRegion(decoBack, deco, backBorder, yPos, logHeight);
                if (targetRegion.Length == 0) {
                    continue;
                }
                foreach (RectangleF rc in targetRegion) {
                    g.Graphics.SetClip(rc, CombineMode.Xor);
                }

                Brush brush = g.GetConsoleDecorationBrush(deco);
                RectangleF rcLine = new RectangleF(0, yPos, targetRegion[targetRegion.Length - 1].Right, logHeight);
                g.Graphics.FillRectangle(brush, rcLine);
            }
            g.Graphics.SetClip(rcClient);
        }

        //=========================================================================================
        // 機　能：ログ１行分の文字列を描画する
        // 引　数：[in]g          グラフィックス
        // 　　　　[in]message    表示するメッセージ
        // 　　　　[in]decoBack   表示するメッセージの修飾情報（前景色の要素だけに限定）
        // 　　　　[in]yPos       表示するY位置
        // 　　　　[in]logHeight  ログの行の高さ[ピクセル]
        // 　　　　[in]rcClient   クライアント領域の大きさ
        // 戻り値：なし
        //=========================================================================================
        private void DrawString(LogGraphics g, string message, short[] decoFore, int yPos, int logHeight, Rectangle rcClient) {
            // 変換点を抽出
            int[] foreDiffPoint = GetDecorationDiffPoint(message, decoFore);
            Font baseFont = g.LogWindowFixedFont;
            float[] foreBorder = TextRendererUtils.MeasureStringRegion(g.Graphics, baseFont, message, foreDiffPoint);
            HashSet<short> renderFore = ConvertDecorationToRenderCommand(decoFore);

            // 描画
            foreach (short deco in renderFore) {
                g.Graphics.SetClip(rcClient, CombineMode.Exclude);
                RectangleF[] targetRegion = ConvertBorderToRegion(decoFore, deco, foreBorder, yPos, logHeight);
                if (targetRegion.Length == 0) {
                    continue;
                }
                foreach (RectangleF rc in targetRegion) {
                    g.Graphics.SetClip(rc, CombineMode.Xor);
                }

                Font font = g.GetConsoleDecorationFont(deco);
                Brush brush = g.GetConsoleDecorationBrush(deco);
                g.Graphics.DrawString(message, font, brush, new PointF(0.0f, yPos));
                if ((deco & ConsoleDecoration.BOLD) != 0) {
                    g.Graphics.DrawString(message, font, brush, new PointF(1.0f, yPos));
                }
            }
            g.Graphics.SetClip(rcClient);
        }

        //=========================================================================================
        // 機　能：HIDE属性を解決し、メッセージの必要区間を空白に置き換える
        // 引　数：[in]decoration  修飾情報
        // 　　　　[in]orgMessage  元のメッセージ
        // 戻り値：HIDE属性を反映したメッセージ
        //=========================================================================================
        public string ResolveHide(short[] decoration, string orgMessage) {
            // HIDE属性が含まれるか確認
            bool found = false;
            for (int i = 0; i < decoration.Length; i++) {
                if ((decoration[i] & ConsoleDecoration.HIDE) != 0) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                return orgMessage;
            }
            
            // HIDE部分を空白に置き換え
            CharWidth charWidth = new CharWidth();
            StringBuilder sb = new StringBuilder();
            int width = 0;
            for (int i = 0; i < orgMessage.Length; i++) {
                char replacementChar = ' ';
                int currentWidth = ConsoleScreen.CHAR_WIDTH_UNKNOWN;
                CharWidth.CharType charType = charWidth.GetCharType(orgMessage[i]);
                if (charType == CharWidth.CharType.FullWidth) {
                    replacementChar = '　';
                    currentWidth = 2;
                } else if (charType == CharWidth.CharType.HalfWidth) {
                    currentWidth = 1;
                }
                if ((decoration[width] & ConsoleDecoration.HIDE) != 0) {
                    sb.Append(replacementChar);
                } else {
                    sb.Append(orgMessage[i]);
                }
                width += currentWidth;
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：修飾情報を解決し、必要な色情報だけに整理する
        // 引　数：[in]decoration   修飾情報
        // 　　　　[in]lineContext  行単位の描画のコンテキスト情報
        // 　　　　[in]defaultColor デフォルトの前景色
        // 　　　　[in]defaultBack  デフォルトの背景色
        // 　　　　[out]decoFore    解決した修飾情報のうち、前景色の成分だけ抽出した修飾情報を返す変数
        // 　　　　[out]decoBack    解決した修飾情報のうち、背景色の成分だけ抽出した修飾情報を返す変数
        // 戻り値：なし
        //=========================================================================================
        public void ResolveColor(short[] decoration, DrawingLogLineContext lineContext, short defaultColor, short defaultBack, out short[] decoFore, out short[] decoBack) {
            decoFore = new short[decoration.Length];
            decoBack = new short[decoration.Length];

            // 選択の開始・終了位置を決定
            int start, end;
            if (lineContext.SelectionStart == -1) {
                start = decoration.Length;
            } else {
                start = lineContext.SelectionStart;
            }
            if (lineContext.SelectionEnd == -1) {
                end = decoration.Length;
            } else {
                end = Math.Min(lineContext.SelectionEnd, decoration.Length);
            }

            // そのまま
            for (int i = 0; i < decoration.Length; i++) {
                int deco = decoration[i];
                int color = deco & ConsoleDecoration.COLOR_MASK;
                int backColor = (deco & ConsoleDecoration.BACK_MASK) >> ConsoleDecoration.BACK_SHIFT;
                if (color == 0) {
                    color = defaultColor;
                }
                if (backColor == 0) {
                    backColor = (defaultBack >> ConsoleDecoration.BACK_SHIFT);
                }

                bool isReverse = ((deco & ConsoleDecoration.REVERSE) != 0);
                if (start <= i && i < end) {
                    isReverse = !isReverse;
                }
                if (isReverse) {
                    int temp = color;
                    color = backColor;
                    backColor = temp;
                }

                deco = (deco & ConsoleDecoration.COLOR_NOT | color) & ConsoleDecoration.BACK_NOT | (backColor << ConsoleDecoration.BACK_SHIFT);
                decoFore[i] = (short)(deco & (0x7fff ^ ConsoleDecoration.BACK_MASK ^ ConsoleDecoration.BLINK ^ ConsoleDecoration.REVERSE ^ ConsoleDecoration.HIDE ^ ConsoleDecoration.VERTIVAL));
                decoBack[i] = (short)(deco & (0x7fff ^ ConsoleDecoration.COLOR_MASK ^ ConsoleDecoration.BLINK ^ ConsoleDecoration.REVERSE ^ ConsoleDecoration.HIDE ^ ConsoleDecoration.VERTIVAL ^ ConsoleDecoration.UNDER_LINE ^ ConsoleDecoration.BOLD));
            }
        }

        //=========================================================================================
        // 機　能：修飾情報から、色の変化点の位置を返す
        // 引　数：[in]message     表示に使用するメッセージ
        // 　　　　[in]decoration  修飾情報
        // 戻り値：変化点の位置情報（messageのString文字単位、全角に注意）
        //=========================================================================================
        private int[] GetDecorationDiffPoint(string message, short[] decoration) {
            CharWidth charWidth = new CharWidth();
            int currentWidth = 0;
            List<int> diffPoint = new List<int>();
            short decoPrev = decoration[0];
            for (int i = 0; i < message.Length; i++) {
                // 変化点を記憶
                if (decoPrev != decoration[currentWidth]) {
                    diffPoint.Add(i);
                    decoPrev = decoration[currentWidth];
                }
                // 次の文字へ
                CharWidth.CharType charType = charWidth.GetCharType(message[i]);
                if (charType == CharWidth.CharType.FullWidth) {
                    currentWidth += 2;
                } else if (charType == CharWidth.CharType.HalfWidth) {
                    currentWidth++;
                } else {
                    currentWidth += ConsoleScreen.CHAR_WIDTH_UNKNOWN;
                }
            }
            diffPoint.Add(message.Length);
            return diffPoint.ToArray();
        }

        //=========================================================================================
        // 機　能：修飾情報に含まれる要素の種類をHashSetのコマンド一覧として返す
        // 引　数：[in]decoration   修飾情報
        // 戻り値：修飾情報に含まれるコマンド
        //=========================================================================================
        private HashSet<short> ConvertDecorationToRenderCommand(short[] decoration) {
            HashSet<short> result = new HashSet<short>();
            for (int i = 0; i < decoration.Length; i++) {
                result.Add(decoration[i]);
            }
            return result;
        }

        //=========================================================================================
        // 機　能：メッセージの文字単位の表示位置の境界点を領域情報に変換する
        // 引　数：[in]decoration   修飾情報
        // 　　　　[in]target       decorationのうち、出力する領域情報の対象となる修飾情報
        // 　　　　[in]borderX      修飾情報の変化点のX座標配列（[0]を除く）
        // 　　　　[in]y1           領域の上座標
        // 　　　　[in]cy           領域の高さ
        // 戻り値：領域情報
        // メ　モ：decoration={1, 1, 1, 2, 2, 3}、borderX={30.0, 50.0}のとき（半角1文字10.0の例）、
        // 　　　　target=2なら{30.0, 50.0}の領域を返す。
        //=========================================================================================
        private RectangleF[] ConvertBorderToRegion(short[] decoration, short target, float[] borderX, int y1, int cy) {
            List<RectangleF> result = new List<RectangleF>();
            int diffCount = 0;
            short decoPrev = -1;
            for (int i = 0; i < decoration.Length; i++) {
                if (decoPrev != decoration[i]) {
                    if (decoration[i] == target) {
                        float startX, endX;
                        if (diffCount == 0) {
                            startX = 0.0f;
                            endX = borderX[diffCount];
                        } else {
                            startX = borderX[diffCount - 1];
                            endX = borderX[diffCount];
                        }
                        RectangleF rc = new RectangleF(startX, y1, endX - startX, cy);
                        result.Add(rc);
                    }
                    decoPrev = decoration[i];
                    diffCount++;
                }
            }
            return result.ToArray();
        }
    }
}
