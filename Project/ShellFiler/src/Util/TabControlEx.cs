using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ShellFiler.UI;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：閉じるボタン付きのタブコントロール
    // 参考ソース：TZ NOTE http://www.pc.zaq.jp/tznote/cs/cs_tabControl_001.html
    //=========================================================================================
    public class TabControlEx : TabControl {
        // 閉じるボタンの幅
        const int CX_BUTTON = 11;

        // 閉じるボタンの高さ
        const int CY_BUTTON = 11;

        // 直前のマウスの位置
        private Point m_ptPrevMouse;

        // タブの閉じるボタンクリックイベント
        public event EventHandler TabCloseButtonClick;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TabControlEx() {
            this.MouseMove += new MouseEventHandler(TabControlEx_MouseMove);
        }

        //=========================================================================================
        // 機　能：マウスのボタンが放されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        protected override void OnMouseUp(MouseEventArgs evt) {
            base.OnMouseUp(evt);
            if (this.TabCount <= 1) {
                return;
            }
            int tabIndex = this.GetTabCloseButtonIndex(evt.X, evt.Y);
            if (tabIndex != -1) {
                this.Invalidate(this.GetTabRect(tabIndex));
                if (this.TabCloseButtonClick != null) {
                    this.TabCloseButtonClick(this.TabPages[tabIndex], new EventArgs());
                }
            }
        }

        //=========================================================================================
        // 機　能：「閉じる」ボタンのX,Y座標に応じたタブのインデックスを返す
        // 引　数：なし
        // 戻り値：タブのインデックス（-1:該当する位置がない）
        //=========================================================================================
        private int GetTabCloseButtonIndex(int x, int y) {
            for (int i = 0; i < base.TabCount; i++) {
                Rectangle rect = this.GetTabCloseButtonRect(i);
                if (rect.Contains(x, y)) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：タブの閉じるボタン場所を取得する
        // 引　数：[in]index   タブのインデックス
        // 戻り値：タブの閉じるボタンの領域
        //=========================================================================================
        private Rectangle GetTabCloseButtonRect(int index) {
            Rectangle rect = this.GetTabRect(index);
            int yMargin = (rect.Height - CY_BUTTON) / 2 - 1;
            rect.X = rect.Right - CX_BUTTON - yMargin;
            rect.Y = rect.Top + yMargin;
            rect.Width = CX_BUTTON;
            rect.Height = CY_BUTTON;

            if (this.SelectedIndex != index) {
                rect.Offset(-3, 2);
            }

            return rect;
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void TabControlEx_MouseMove(object sender, MouseEventArgs evt) {
            if (this.TabCount <= 1) {
                return;
            }
            Point ptMouse = this.PointToClient(Cursor.Position);
            int currentIndex = GetTabCloseButtonIndex(ptMouse.X, ptMouse.Y);
            int prevIndex = GetTabCloseButtonIndex(m_ptPrevMouse.X, m_ptPrevMouse.Y);
            if (currentIndex != prevIndex) {
                DrawTabCloseButton();
            }
            m_ptPrevMouse = ptMouse;
        }

        //=========================================================================================
        // 機　能：すべてのタブの閉じるボタンを描画する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void DrawTabCloseButton() {
            if (this.TabCount <= 1) {
                return;
            }
            Graphics g = this.CreateGraphics();
            try {
                Rectangle rect = Rectangle.Empty;
                Point pt = this.PointToClient(Cursor.Position);
                for (int i = 0; i < this.TabPages.Count; i++) {
                    rect = this.GetTabCloseButtonRect(i);
                    if (rect.Contains(pt)) {
                        g.DrawImage(UIIconManager.TabClose_Focus, rect.X, rect.Y, CX_BUTTON, CY_BUTTON);
                    } else {
                        g.DrawImage(UIIconManager.TabClose_Normal, rect.X, rect.Y, CX_BUTTON, CY_BUTTON);
                    }
                }
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：ウィンドウプロシージャ
        // 引　数：[in]msg   メッセージ
        // 戻り値：なし
        //=========================================================================================
        protected override void WndProc(ref Message msg) {
            base.WndProc(ref msg);
            switch (msg.Msg) {
                case 15:
                    this.DrawTabCloseButton();
                    break;
                default:
                    break;
            }
        }

        //=========================================================================================
        // 機　能：入力処理を行ってもよいキーかどうかを判断する
        // 引　数：[in]keyData  キー
        // 戻り値：keyDataの処理をアプリケーション側で行う予定のときtrue
        //=========================================================================================
        protected override bool IsInputKey(Keys keyData) {
            return true;
        }
    }
}
