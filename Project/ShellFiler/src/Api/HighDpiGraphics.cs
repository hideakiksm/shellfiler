
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：高解像度ディスプレイ対応のGraphics
    //=========================================================================================
    public class HighDpiGraphics : IDisposable {
        // 標準のX方向解像度
        public const float STANDARD_DPI_X = 96f;
        // 標準のY方向解像度
        public const float STANDARD_DPI_Y = 96f;
        // グラフィック
        private Graphics m_graphics;
        // グラフィック（Dispose予定）
        private Graphics m_graphicsForDispose;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public HighDpiGraphics(Graphics graphics) {
            m_graphics = graphics;
            m_graphicsForDispose = null;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（コントロール指定）
        // 引　数：[in]control   描画対象のコントロール
        // 戻り値：なし
        //=========================================================================================
        public HighDpiGraphics(Control control) {
            m_graphicsForDispose = control.CreateGraphics();
            m_graphics = m_graphicsForDispose;
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public virtual void Dispose() {
            if (m_graphicsForDispose != null) {
                m_graphicsForDispose.Dispose();
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックス
        //=========================================================================================
        public virtual Graphics Graphics {
            get {
                return m_graphics;
            }
        }

        //=========================================================================================
        // 機　能：X座標を解像度に合わせて座標変換する
        // 引　数：[in]x    X座標
        // 戻り値：高解像度対応のX座標
        //=========================================================================================
        public int X(int x) {
            int xHighDpi = (int)(Graphics.DpiX / STANDARD_DPI_X * x);
            return xHighDpi;
        }

        //=========================================================================================
        // 機　能：X座標を解像度に合わせて座標変換する
        // 引　数：[in]x    X座標
        // 戻り値：高解像度対応のX座標
        //=========================================================================================
        public float Xf(float x) {
            float xHighDpi = Graphics.DpiX / STANDARD_DPI_X * x;
            return xHighDpi;
        }

        //=========================================================================================
        // 機　能：Y座標を解像度に合わせて座標変換する
        // 引　数：[in]y    Y座標
        // 戻り値：高解像度対応のY座標
        //=========================================================================================
        public int Y(int y) {
            int yHighDpi = (int)(Graphics.DpiY / STANDARD_DPI_Y * y);
            return yHighDpi;
        }

        //=========================================================================================
        // 機　能：Y座標を解像度に合わせて座標変換する
        // 引　数：[in]y    Y座標
        // 戻り値：高解像度対応のY座標
        //=========================================================================================
        public float Yf(float y) {
            float yHighDpi = Graphics.DpiY / STANDARD_DPI_Y * y;
            return yHighDpi;
        }

        //=========================================================================================
        // 機　能：Rectangleを解像度に合わせて座標変換する
        // 引　数：[in]rect  Rectangle
        // 戻り値：高解像度対応のRectangle
        //=========================================================================================
        public Rectangle Rect(Rectangle rect) {
            int left = (int)(Graphics.DpiX / STANDARD_DPI_X * rect.Left);
            int right = (int)(Graphics.DpiX / STANDARD_DPI_X * rect.Right);
            int top = (int)(Graphics.DpiY / STANDARD_DPI_X * rect.Top);
            int bottom = (int)(Graphics.DpiY / STANDARD_DPI_X * rect.Bottom);
            return new Rectangle(left, top, right, bottom);
        }

        public void DrawLine(Pen pen, int x1, int y1, int x2, int y2) {
            Graphics.DrawLine(pen, X(x1), Y(y1), X(x2), Y(y2));
        }

        public void DrawLine(Pen pen, Point pt1, Point pt2) {
            Graphics.DrawLine(pen, new Point(X(pt1.X), Y(pt1.Y)), new Point(X(pt2.X), Y(pt2.Y)));
        }

        public void DrawRectangle(Pen pen, Rectangle rect) {
            Graphics.DrawRectangle(pen, new Rectangle(X(rect.X), Y(rect.Y), X(rect.Right), Y(rect.Bottom)));
        }

        public void DrawRectangle(Pen pen, int x, int y, int width, int height) {
            Graphics.DrawRectangle(pen, new Rectangle(X(x), Y(y), X(x + width), Y(y + height)));
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y) {
            Graphics.DrawString(s, font, brush, Xf(x), Yf(y));
        }

        public void DrawString(string s, Font font, Brush brush, PointF point) {
            Graphics.DrawString(s, font, brush, new PointF(Xf(point.X), Yf(point.Y)));
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format) {
            Graphics.DrawString(s, font, brush, Xf(x), Yf(y));
        }

        public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format) {
            Graphics.DrawString(s, font, brush, new PointF(Xf(point.X), Yf(point.Y)), format);
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle) {
            Graphics.DrawString(s, font, brush,
                new RectangleF(Xf(layoutRectangle.X), Yf(layoutRectangle.Y), Xf(layoutRectangle.Right), Yf(layoutRectangle.Bottom)));
        }

        public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format) {
            Graphics.DrawString(s, font, brush,
                new RectangleF(Xf(layoutRectangle.X), Yf(layoutRectangle.Y), Xf(layoutRectangle.Right), Yf(layoutRectangle.Bottom)),
                format);
        }
    }
}
