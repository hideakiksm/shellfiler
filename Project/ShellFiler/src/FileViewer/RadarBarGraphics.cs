using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using System.Runtime.InteropServices;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：レーダーバーの描画用グラフィックス
    //=========================================================================================
    public class RadarBarGraphics {
        // グラフィック（null:未初期化）
        private Graphics m_graphics;

        // 描画対象のコントロール（null:グラフィック指定）
        private Control m_control;

        // クライアント領域の幅
        private int m_cxWidth;

        // レーダーバーの背景色のブラシ
        private Brush m_radarBarBackBrush = null;

        // レーダーバーの検索ヒット位置のペン１
        private Pen m_radarBarHitSearchPen1 = null;

        // レーダーバーの検索ヒット位置のペン２
        private Pen m_radarBarHitSearchPen2 = null;

        // レーダーバーの自動検索ヒット位置のペン１
        private Pen m_radarBarHitAutoPen1 = null;

        // レーダーバーの自動検索ヒット位置のペン２
        private Pen m_radarBarHitAutoPen2 = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics  グラフィックス
        // 　　　　[in]cxWidth   クライアント領域の幅
        // 戻り値：なし
        //=========================================================================================
        public RadarBarGraphics(Graphics graphics, int cxWidth) {
            m_control = null;
            m_graphics = graphics;
            m_cxWidth = cxWidth;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（コントロール指定で必要時に初期化）
        // 引　数：[in]control   描画対象のコントロール
        // 　　　　[in]cxWidth   クライアント領域の幅
        // 戻り値：なし
        //=========================================================================================
        public RadarBarGraphics(Control control, int cxWidth) {
            m_control = control;
            m_graphics = null;
            m_cxWidth = cxWidth;
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_control != null && m_graphics != null) {
                m_graphics.Dispose();
            }
            if (m_radarBarBackBrush != null) {
                m_radarBarBackBrush.Dispose();
                m_radarBarBackBrush = null;
            }
            if (m_radarBarHitSearchPen1 != null) {
                m_radarBarHitSearchPen1.Dispose();
                m_radarBarHitSearchPen1 = null;
            }
            if (m_radarBarHitSearchPen2 != null) {
                m_radarBarHitSearchPen2.Dispose();
                m_radarBarHitSearchPen2 = null;
            }
            if (m_radarBarHitAutoPen1 != null) {
                m_radarBarHitAutoPen1.Dispose();
                m_radarBarHitAutoPen1 = null;
            }
            if (m_radarBarHitAutoPen2 != null) {
                m_radarBarHitAutoPen2.Dispose();
                m_radarBarHitAutoPen2 = null;
            }
        }
        
        //=========================================================================================
        // プロパティ：グラフィックス
        //=========================================================================================
        public Graphics Graphics {
            get {
                if (m_graphics == null) {
                    m_graphics = m_control.CreateGraphics();
                }
                return m_graphics;
            }
        }

        //=========================================================================================
        // プロパティ：レーダーバーの背景色のブラシ
        //=========================================================================================
        public Brush RadarBarBackBrush {
            get {
                if (m_radarBarBackBrush == null) {
                    if (Configuration.Current.RadarBarBackColor1.Equals(Configuration.Current.RadarBarBackColor2)) {
                        m_radarBarBackBrush = new SolidBrush(Configuration.Current.TextViewerLineNoBackColor1);
                    } else {
                        Rectangle rc = new Rectangle(0, 0, m_cxWidth, 10);
                        m_radarBarBackBrush = new LinearGradientBrush(rc, Configuration.Current.RadarBarBackColor1, Configuration.Current.RadarBarBackColor2, LinearGradientMode.Horizontal);
                    }
                }
                return m_radarBarBackBrush;
            }
        }
 
        //=========================================================================================
        // プロパティ：レーダーバーの検索ヒット位置のペン１
        //=========================================================================================
        public Pen RadarBarHitSearchPen1 {
            get {
                if (m_radarBarHitSearchPen1 == null) {
                    m_radarBarHitSearchPen1 = new Pen(Configuration.Current.TextViewerSearchHitBackColor);
                }
                return m_radarBarHitSearchPen1;
            }
        }
 
        //=========================================================================================
        // プロパティ：レーダーバーの検索ヒット位置のペン２
        //=========================================================================================
        public Pen RadarBarHitSearchPen2 {
            get {
                if (m_radarBarHitSearchPen2 == null) {
                    Color col = Configuration.Current.TextViewerSearchHitBackColor;
                    Color colDark = Color.FromArgb(col.R / 2, col.G / 2, col.B / 2);
                    m_radarBarHitSearchPen2 = new Pen(colDark);
                }
                return m_radarBarHitSearchPen2;
            }
        }
 
        //=========================================================================================
        // プロパティ：レーダーバーの自動検索ヒット位置のペン１
        //=========================================================================================
        public Pen RadarBarHitAutoPen1 {
            get {
                if (m_radarBarHitAutoPen1 == null) {
                    m_radarBarHitAutoPen1 = new Pen(Configuration.Current.TextViewerSearchAutoTextColor);
                }
                return m_radarBarHitAutoPen1;
            }
        }
 
        //=========================================================================================
        // プロパティ：レーダーバーの自動検索ヒット位置のペン２
        //=========================================================================================
        public Pen RadarBarHitAutoPen2 {
            get {
                if (m_radarBarHitAutoPen2 == null) {
                    Color col = Configuration.Current.TextViewerSearchAutoTextColor;
                    Color colDark = Color.FromArgb(col.R / 2, col.G / 2, col.B / 2);
                    m_radarBarHitAutoPen2 = new Pen(colDark);
                }
                return m_radarBarHitAutoPen2;
            }
        }
    }
}
