using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：ダブルバッファリング用のバッファ
    //=========================================================================================
    public class DoubleBuffer {
        // 描画対象のコントロール（null:グラフィックス指定）
        private Control m_control;

        // コントロールと関連づけられたグラフィックス
        private Graphics m_graphicsControl;

        // ダブルバッファに使用するビットマップ
        private Bitmap m_bitmapBuffer;

        // ビットマップに描画するためのグラフィックス
        private Graphics m_bitmapGraphics;

        //=========================================================================================
        // 機　能：コンストラクタ（コントロール指定）
        // 引　数：[in]control  描画対象のコントロール
        // 　　　　[in]cx       描画対象の画面の幅
        // 　　　　[in]cy       描画対象の画面の高さ
        // 戻り値：なし
        //=========================================================================================
        public DoubleBuffer(Control control, int cx, int cy) {
            m_control = control;
            m_graphicsControl = control.CreateGraphics();
            m_bitmapBuffer = new Bitmap(cx, cy, PixelFormat.Format24bppRgb);
            m_bitmapGraphics = Graphics.FromImage(m_bitmapBuffer);
        }

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]g   描画に使用するグラフィックス
        // 　　　　[in]cx  描画対象の画面の幅
        // 　　　　[in]cy  描画対象の画面の高さ
        // 戻り値：なし
        //=========================================================================================
        public DoubleBuffer(Graphics g, int cx, int cy) {
            m_control = null;
            m_graphicsControl = g;
            m_bitmapBuffer = new Bitmap(cx, cy, PixelFormat.Format24bppRgb);
            m_bitmapGraphics = Graphics.FromImage(m_bitmapBuffer);
        }

        //=========================================================================================
        // 機　能：ビットマップの画面をコントロールにフラッシュする
        // 引　数：[in]drawX    最終描画X位置
        // 　　　　[in]drawY    最終描画Y位置
        // 戻り値：なし
        //=========================================================================================
        public void FlushScreen(int drawX, int drawY) {
            m_graphicsControl.DrawImage(m_bitmapBuffer, drawX, drawY);
            m_bitmapGraphics.Dispose();
            m_bitmapBuffer.Dispose();
            if (m_control != null) {
                m_graphicsControl.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：ビットマップの画面を未使用状態で破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void FlushNoUse() {
            m_bitmapGraphics.Dispose();
            m_bitmapBuffer.Dispose();
            if (m_control != null) {
                m_graphicsControl.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：描画の開始点を設定する
        // 引　数：[in]drawX    描画X位置
        // 　　　　[in]drawY    描画Y位置
        // 戻り値：なし
        //=========================================================================================
        public void SetDrawOrigin(int drawX, int drawY) {
            m_bitmapGraphics.TranslateTransform(drawX, drawY);
        }

        //=========================================================================================
        // プロパティ：描画対象の長方形
        //=========================================================================================
        public Rectangle DrawingRectangle {
            get {
                return new Rectangle(0, 0, m_bitmapBuffer.Width, m_bitmapBuffer.Height);
            }
        }

        //=========================================================================================
        // プロパティ：描画に使用するバッファ側のグラフィックス
        //=========================================================================================
        public Graphics DrawingGraphics {
            get {
                return m_bitmapGraphics;
            }
        }

        //=========================================================================================
        // プロパティ：ダブルバッファに使用するビットマップ
        //=========================================================================================
        public Bitmap BitmapBuffer {
            get {
                return m_bitmapBuffer;
            }
        }
    }
}
