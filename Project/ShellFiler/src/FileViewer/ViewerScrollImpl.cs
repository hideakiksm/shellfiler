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
using ShellFiler.Command.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ビューアのスクロール実装
    //=========================================================================================
    public class ViewerScrollImpl {
        // 末端行のマージン
        public const int CY_BOTTOM_MARGIN = 8;


        // スクロール対象のビューア
        private TextFileViewer m_viewer;

        // スクロールの制御対象
        private IScrollValueSource m_valueSource;

        // 行の高さ[ピクセル]
        private int m_cyLineHeight;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]viewer       スクロール対象のビューア
        // 　　　　[in]valueSource  スクロールの制御対象
        // 　　　　[in]cyLineHeight 行の高さ
        // 戻り値：なし
        //=========================================================================================
        public ViewerScrollImpl(TextFileViewer viewer, IScrollValueSource valueSource, int cyLineHeight) {
            m_viewer = viewer;
            m_valueSource = valueSource;
            m_cyLineHeight = cyLineHeight;
        }

        //=========================================================================================
        // 機　能：サイズ変更時の処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnSizeChange() {
            SetHorizontalScrollbar();
            Rectangle rcClient = m_viewer.ClientRectangle;
            m_valueSource.ScrollXPosition = Math.Max(0, Math.Min(m_valueSource.ScrollXPosition, m_valueSource.ScrollCxNeed - rcClient.Width));
            
            m_valueSource.ScrollYCompleteLineSize = (rcClient.Height - CY_BOTTOM_MARGIN) / m_cyLineHeight;
            m_valueSource.ScrollYPosition = Math.Max(0, Math.Min(m_valueSource.ScrollYPosition, m_valueSource.ScrollCyNeed - m_valueSource.ScrollYCompleteLineSize));

            m_valueSource.UpdateLinePosition();
        }
        
        //=========================================================================================
        // 機　能：水平スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetHorizontalScrollbar() {
            if (m_viewer.ClientRectangle.Width > m_valueSource.ScrollCxNeed) {
                // スクロールバーを表示しない
                m_viewer.HorizontalScroll.Visible = false;
            } else {
                // スクロールバーを表示する
                m_viewer.HorizontalScroll.SmallChange = 1;
                m_viewer.HorizontalScroll.LargeChange = m_viewer.ClientRectangle.Width;
                m_viewer.HorizontalScroll.Minimum = 0;
                m_viewer.HorizontalScroll.Maximum = m_valueSource.ScrollCxNeed - 1;
                m_viewer.HorizontalScroll.Value = m_valueSource.ScrollXPosition;
                m_viewer.HorizontalScroll.Visible = true;
                m_viewer.HorizontalScroll.Enabled = true;
            }
        }
        
        //=========================================================================================
        // 機　能：垂直スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetVerticalScrollbar() {
            try {
                if (m_valueSource.ScrollCyNeed <= m_valueSource.ScrollYCompleteLineSize) {
                    // スクロールバーを表示しない
                    m_viewer.VerticalScroll.Enabled = false;
                } else {
                    // スクロールバーを表示する
                    m_viewer.VerticalScroll.SmallChange = 1;
                    m_viewer.VerticalScroll.LargeChange = m_valueSource.ScrollYCompleteLineSize;
                    m_viewer.VerticalScroll.Minimum = 0;
                    m_viewer.VerticalScroll.Maximum = m_valueSource.ScrollCyNeed - 1;
                    m_viewer.VerticalScroll.Value = m_valueSource.ScrollYPosition;
                    m_viewer.VerticalScroll.Visible = true;
                    m_viewer.VerticalScroll.Enabled = true;
                }
            } catch (ObjectDisposedException) {
                // 読み込み中にウィンドウを閉じると発生
            }
        }

        //=========================================================================================
        // 機　能：スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnHScroll(ScrollEventArgs evt) {
            // スクロールを処理
            int newPosition = m_valueSource.ScrollXPosition;
            switch(evt.Type) {
                case ScrollEventType.SmallDecrement:
                    newPosition--;
                    break;
                case ScrollEventType.LargeDecrement:
                    newPosition -= m_valueSource.ScrollYCompleteLineSize;
                    break;
                case ScrollEventType.SmallIncrement:
                    newPosition++;
                    break;
                case ScrollEventType.LargeIncrement:
                    newPosition += m_valueSource.ScrollYCompleteLineSize;
                    break;
                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:
                case ScrollEventType.EndScroll:
                    newPosition = evt.NewValue;
                    break;
            }

            // 新しい位置を検証
            int limitSize = m_valueSource.ScrollCxNeed - m_viewer.ClientRectangle.Width;
            newPosition = Math.Max(0, Math.Min(newPosition, limitSize));

            // 移動に応じてスクロール
            int dx = newPosition - m_valueSource.ScrollXPosition;
            if (dx != 0) {
                TextViewerGraphics g = new TextViewerGraphics(m_viewer, m_viewer.TextViewerComponent.LineNoAreaWidth);
                try {
                    Win32API.Win32ScrollWindow(m_viewer.Handle, -dx, 0, m_viewer.ClientRectangle, m_viewer.ClientRectangle);
                    m_valueSource.ScrollXPosition = newPosition;
                } finally {
                    g.Dispose();
                }

                // スクロールバーを設定
                SetHorizontalScrollbar();
            }
        }

        //=========================================================================================
        // 機　能：スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnVScroll(ScrollEventArgs evt) {
            // スクロールを処理
            int newPosition = m_valueSource.ScrollYPosition;
            switch(evt.Type) {
                case ScrollEventType.SmallDecrement:
                    newPosition--;
                    break;
                case ScrollEventType.LargeDecrement:
                    newPosition -= m_valueSource.ScrollYCompleteLineSize;
                    break;
                case ScrollEventType.SmallIncrement:
                    newPosition++;
                    break;
                case ScrollEventType.LargeIncrement:
                    newPosition += m_valueSource.ScrollYCompleteLineSize;
                    break;
                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:
                case ScrollEventType.EndScroll:
                    newPosition = evt.NewValue;
                    break;
            }

            // 新しい位置を検証
            newPosition = Math.Max(0, Math.Min(newPosition, m_valueSource.ScrollCyNeed - m_valueSource.ScrollYCompleteLineSize));

            // 移動に応じてスクロール
            int dy = newPosition - m_valueSource.ScrollYPosition;
            if (dy != 0) {
                Win32API.Win32ScrollWindow(m_viewer.Handle, 0, -dy * m_cyLineHeight, m_viewer.ClientRectangle, m_viewer.ClientRectangle);
                m_valueSource.ScrollYPosition = newPosition;

                // 位置を更新
                m_valueSource.UpdateLinePosition();
            }
        }
        
        //=========================================================================================
        // 機　能：マウスホイールイベントを処理する
        // 引　数：[in]evt  マウスイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnMouseWheel(MouseEventArgs evt) {
            if (evt.Delta < 0) {
                int moveLine = Math.Max(1, (-evt.Delta) / 120);
                if (moveLine >= 2) {
                    moveLine *= 2;
                }
                MoveDisplayPosition(moveLine);
            } else if (evt.Delta > 0) {
                int moveLine = Math.Max(1, evt.Delta / 120);
                if (moveLine >= 2) {
                    moveLine *= 2;
                }
                MoveDisplayPosition(-moveLine);
            }
        }

        //=========================================================================================
        // 機　能：表示位置を上下に移動する
        // 引　数：[in]lines    移動する行数（下方向が＋）
        // 戻り値：なし
        //=========================================================================================
        public void MoveDisplayPosition(int lines) {
            if (lines < int.MinValue / 2) {
                lines = int.MinValue / 2;
            } else if (lines > int.MaxValue / 2) {
                lines = int.MaxValue / 2;
            }

            // 一番下なら移動しない
            int newPosition = m_valueSource.ScrollYPosition + lines;
            newPosition = Math.Max(0, Math.Min(newPosition, m_valueSource.ScrollCyNeed - m_valueSource.ScrollYCompleteLineSize));

            if (m_valueSource.ScrollSearchCursorLine == -1) {
                // 検索カーソルがない場合はスクロール
                int dy = newPosition - m_valueSource.ScrollYPosition;
                if (dy != 0) {
                    // 移動に応じてスクロール
                    Win32API.Win32ScrollWindow(m_viewer.Handle, 0, -dy * m_cyLineHeight, m_viewer.ClientRectangle, m_viewer.ClientRectangle);
                    m_valueSource.ScrollYPosition = newPosition;

                    // 位置を更新
                    m_valueSource.UpdateLinePosition();
                }
            } else {
                // 検索カーソルがあるときは消去するため再描画
                m_valueSource.ScrollSearchCursorLine = -1;
                m_valueSource.ScrollYPosition = newPosition;
                m_viewer.Invalidate();
                m_valueSource.UpdateLinePosition();
            }
        }

        //=========================================================================================
        // 機　能：マウスによるスクロールを行う
        // 引　数：[in]lines  目的の論理行
        // 戻り値：なし
        //=========================================================================================
        public void MouseScroll(int line) {           
            int newPosition = m_valueSource.ScrollYPosition;
            if (line < m_valueSource.ScrollYPosition) {
                newPosition = line;
            } else if (line > m_valueSource.ScrollYPosition + m_valueSource.ScrollYCompleteLineSize) {
                newPosition = line - m_valueSource.ScrollYCompleteLineSize;
            }
            int dy;
            int sleepTime;
            MouseOffsetToScrollLine(newPosition - m_valueSource.ScrollYPosition, out dy, out sleepTime);
            newPosition = Math.Max(0, Math.Min(m_valueSource.ScrollYPosition + dy, m_valueSource.ScrollCyNeed - m_valueSource.ScrollYCompleteLineSize));
            dy = newPosition - m_valueSource.ScrollYPosition;
            if (dy != 0) {
                Win32API.Win32ScrollWindow(m_viewer.Handle, 0, -dy * m_cyLineHeight, m_viewer.ClientRectangle, m_viewer.ClientRectangle);
                m_valueSource.ScrollYPosition = m_valueSource.ScrollYPosition + dy;
                m_valueSource.UpdateLinePosition();

                if (sleepTime > 0) {
                    m_viewer.Update();
                    Thread.Sleep(sleepTime);
                }
            }
        }

        //=========================================================================================
        // 機　能：マウスのオフセット位置からスクロール行数を求める
        // 引　数：[in]offset     マウス位置の起点からのオフセット
        // 　　　　[out]line      スクロールさせる行数を返す変数
        // 　　　　[out]sleepTime タイミング制御用のスリープ時間を返す変数
        // 戻り値：なし
        //=========================================================================================
        public static void MouseOffsetToScrollLine(int offset, out int line, out int sleepTime) {
            int sign = (offset >= 0) ? 1 : -1;
            int absOffset = Math.Abs(offset);
            if (absOffset == 0) {
                line = 0;
                sleepTime = 0;
            } else if (absOffset < 1) {
                line = 1 * sign;
                sleepTime = 200;
            } else if (absOffset < 2) {
                line = 1 * sign;
                sleepTime = 100;
            } else if (absOffset < 3) {
                line = 2 * sign;
                sleepTime = 50;
            } else if (absOffset < 4) {
                line = 5 * sign;
                sleepTime = 30;
            } else {
                line = 10 * sign;
                sleepTime = 0;
            }
        }

        //=========================================================================================
        // インターフェース：スクロール対象のビューアコンポーネント
        //=========================================================================================
        public interface IScrollValueSource {

            //=========================================================================================
            // 機　能：行位置を更新する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            void UpdateLinePosition();

            //=========================================================================================
            // プロパティ：水平スクロールに必要な幅
            //=========================================================================================
            int ScrollCxNeed {
                get;
            }

            //=========================================================================================
            // プロパティ：水平スクロールバーの位置
            //=========================================================================================
            int ScrollXPosition {
                get;
                set;
            }

            //=========================================================================================
            // プロパティ：垂直スクロールに必要な高さ
            //=========================================================================================
            int ScrollCyNeed {
                get;
            }

            //=========================================================================================
            // プロパティ：水平スクロールバーの位置
            //=========================================================================================
            int ScrollYPosition {
                get;
                set;
            }

            //=========================================================================================
            // プロパティ：画面上に完全に表示できる行数
            //=========================================================================================
            int ScrollYCompleteLineSize {
                get;
                set;
            }

            //=========================================================================================
            // プロパティ：検索時のカーソル行（-1:表示しない）
            //=========================================================================================
            int ScrollSearchCursorLine {
                get;
                set;
            }
        }
    }
}
