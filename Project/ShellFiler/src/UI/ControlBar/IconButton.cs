using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Util;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：アイコンボタン
    //=========================================================================================
    public partial class IconButton : UserControl {
        // マウスカーソルがボタン上にあるときtrue
        private bool m_hover = false;

        // クリック状態にあるときtrue
        private bool m_onClick = false;

        // 選択状態にするときtrue
        private bool m_selected = false;

        // ボタンに使用するイメージリスト
        private ImageList m_imageList;

        // ボタンに使用するイメージリストのインデックス
        private int m_imageIndex;

        // クリックイベント
        public event EventHandler Clicked;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]imageList  ボタンに使用するイメージリスト
        // 　　　　[in]imageIndex ボタンに使用するイメージリストのインデックス
        // 戻り値：なし
        //=========================================================================================
        public IconButton(ImageList imageList, int imageIndex) {
            InitializeComponent();
            m_imageList = imageList;
            m_imageIndex = imageIndex;
        }

        //=========================================================================================
        // 機　能：画面を描画するときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void IconButton_Paint(object sender, PaintEventArgs evt) {
            Draw(evt.Graphics);
        }

        //=========================================================================================
        // 機　能：画面を描画する
        // 引　数：[in]grp  描画に使用するグラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void Draw(Graphics grp) {
            DoubleBuffer doubleBuffer = new DoubleBuffer(grp, this.Width, this.Height);
            ImageButtonGraphics g = new ImageButtonGraphics(doubleBuffer.DrawingGraphics);

            Brush backBrush;
            Pen borderPen;
            if (m_onClick) {
                backBrush = g.PushBrush;
                borderPen = g.PushBorderPen;
            } else if (m_hover) {
                backBrush = g.HighLightBrush;
                borderPen = g.HighlightBorderPen;
            } else if (m_selected) {
                backBrush = g.SelectedBrush;
                borderPen = g.SelectedBorderPen;
            } else {
                backBrush = SystemBrushes.Window;
                borderPen = SystemPens.Window;
            }

            Rectangle rcRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            g.Graphics.FillRectangle(backBrush, rcRect);
            g.Graphics.DrawRectangle(borderPen, rcRect);
            Image image = m_imageList.Images[m_imageIndex];
            int margin = (this.Width - this.Width * 8 / 10) / 2;
            Rectangle rcDest = new Rectangle(margin, margin, this.Width - margin, this.Height - margin);
            Rectangle rcSrc = new Rectangle(0, 0, image.Width, image.Height);
            g.Graphics.DrawImage(m_imageList.Images[m_imageIndex], rcDest, rcSrc, GraphicsUnit.Pixel);

            doubleBuffer.FlushScreen(0, 0);
        }

        //=========================================================================================
        // 機　能：マウスポインタが領域内に入ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void IconButton_MouseEnter(object sender, EventArgs evt) {
            if (m_hover) {
                return;
            }
            m_hover = true;
            Invalidate();
            Update();
        }

        //=========================================================================================
        // 機　能：マウスポインタが領域から出たときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void IconButton_MouseLeave(object sender, EventArgs evt) {
            if (!m_hover) {
                return;
            }
            m_hover = false;
            Invalidate();
            Update();
        }

        //=========================================================================================
        // 機　能：マウスのボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void IconButton_MouseDown(object sender, MouseEventArgs evt) {
            if (m_onClick) {
                return;
            }
            if ((evt.Button & MouseButtons.Left) != MouseButtons.Left) {
                return;
            }
            m_onClick = true;
            Graphics g = CreateGraphics();
            Draw(g);
            g.Dispose();
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void IconButton_MouseUp(object sender, MouseEventArgs evt) {
            if (!m_onClick) {
                return;
            }
            m_onClick = false;
            m_hover = false;
            Graphics g = CreateGraphics();
            Draw(g);
            g.Dispose();

            if (Clicked != null) {
                Clicked(sender, evt);
            }
        }

        //=========================================================================================
        // プロパティ：ボタンが選択状態にあるときtrue
        //=========================================================================================
        public bool Selected {
            get {
                return m_selected;
            }
            set {
                m_selected = value;
            }
        }


        //=========================================================================================
        // クラス：アイコンボタンの表示用グラフィックス
        //=========================================================================================
        private class ImageButtonGraphics {
            // グラフィックス
            private Graphics m_graphics;

            // 描画に使用する色の情報
            private ProfessionalColorTable m_colorTable = new ProfessionalColorTable();

            // ハイライト状態背景の描画用ブラシ（未使用のときnull）
            private Brush m_highLightBrush = null;

            // 選択状態背景の描画用ブラシ（未使用のときnull）
            private Brush m_pushBrush = null;

            // 選択状態背景の描画用ブラシ（未使用のときnull）
            private Brush m_selectedBrush = null;

            // ハイライト状態枠の描画用ペン（未使用のときnull）
            private Pen m_highlightBorderPen = null;

            // 押された状態枠の描画用ペン（未使用のときnull）
            private Pen m_pushBorderPen = null;
            
            // 選択状態背景の描画用ペン（未使用のときnull）
            private Pen m_selectedBorderPen = null;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]g   描画に使用するグラフィックス
            // 戻り値：なし
            //=========================================================================================
            public ImageButtonGraphics(Graphics g) {
                m_graphics = g;
            }

            //=========================================================================================
            // 機　能：終了処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Dispose() {
                if (m_highLightBrush != null) {
                    m_highLightBrush.Dispose();
                    m_highLightBrush = null;
                }
                if (m_pushBrush != null) {
                    m_pushBrush.Dispose();
                    m_pushBrush = null;
                }
                if (m_selectedBrush != null) {
                    m_selectedBrush.Dispose();
                    m_selectedBrush = null;
                }
                if (m_highlightBorderPen != null) {
                    m_highlightBorderPen.Dispose();
                    m_highlightBorderPen = null;
                }
                if (m_pushBorderPen != null) {
                    m_pushBorderPen.Dispose();
                    m_pushBorderPen = null;
                }
                if (m_selectedBorderPen != null) {
                    m_selectedBorderPen.Dispose();
                    m_selectedBorderPen = null;
                }
            }

            //=========================================================================================
            // プロパティ：グラフィックス
            //=========================================================================================
            public Graphics Graphics {
                get {
                    return m_graphics;
                }
            }

            //=========================================================================================
            // プロパティ：ハイライト状態背景の描画用ブラシ
            //=========================================================================================
            public Brush HighLightBrush {
                get {
                    if (m_highLightBrush == null) {
                        m_highLightBrush = new SolidBrush(m_colorTable.ButtonSelectedGradientMiddle);
                    }
                    return m_highLightBrush;
                }
            }

            //=========================================================================================
            // プロパティ：押された状態背景の描画用ブラシ
            //=========================================================================================
            public Brush PushBrush {
                get {
                    if (m_pushBrush == null) {
                        m_pushBrush = new SolidBrush(m_colorTable.ButtonPressedGradientMiddle);
                    }
                    return m_pushBrush;
                }
            }

            //=========================================================================================
            // プロパティ：選択状態背景の描画用ブラシ
            //=========================================================================================
            public Brush SelectedBrush {
                get {
                    if (m_selectedBrush == null) {
                        m_selectedBrush = new SolidBrush(Color.Yellow);
                    }
                    return m_selectedBrush;
                }
            }

            //=========================================================================================
            // プロパティ：ハイライト状態枠の描画用ペン
            //=========================================================================================
            public Pen HighlightBorderPen {
                get {
                    if (m_highlightBorderPen == null) {
                        m_highlightBorderPen = new Pen(m_colorTable.ButtonSelectedBorder);
                    }
                    return m_highlightBorderPen;
                }
            }

            //=========================================================================================
            // プロパティ：押された状態枠の描画用ペン
            //=========================================================================================
            public Pen PushBorderPen {
                get {
                    if (m_pushBorderPen == null) {
                        m_pushBorderPen = new Pen(m_colorTable.ButtonPressedBorder);
                    }
                    return m_pushBorderPen;
                }
            }

            //=========================================================================================
            // プロパティ：選択状態背景の描画用ペン
            //=========================================================================================
            public Pen SelectedBorderPen {
                get {
                    if (m_selectedBorderPen == null) {
                        m_selectedBorderPen = new Pen(Color.FromArgb(130, 150, 50));
                    }
                    return m_selectedBorderPen;
                }
            }
        }
    }
}
