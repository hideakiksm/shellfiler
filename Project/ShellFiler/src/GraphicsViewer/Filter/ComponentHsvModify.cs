using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.GraphicsViewer;
using ShellFiler.Command.GraphicsViewer.View;
using ShellFiler.Util;
using ShellFiler.FileSystem;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.Locale;

namespace ShellFiler.GraphicsViewer.Filter {

    //=========================================================================================
    // クラス：色調調整フィルター
    //=========================================================================================
    class ComponentHsvModify : IFilterComponent {
        // 色相の修正量（-180～180）
        private float m_hModify;
        
        // 彩度の修正量（-1～1）
        private float m_sModify;

        // 明度の修正量（-1～1）
        private float m_vModify;
        
        // 元のイメージ
        private byte[] m_srcImage;

        // 結果を書き込むイメージ
        private byte[] m_destImage;

        // 画像の幅
        private int m_cxImage;

        // 画像の高さ
        private int m_cyImage;

        // ストライド（次のラインまでのバイト数）
        private int m_stride;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ComponentHsvModify() {
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcImage    元のイメージ
        // 　　　　[in]destImage   結果を書き込むイメージ
        // 　　　　[in]cxImage     画像の幅
        // 　　　　[in]cyImage     画像の高さ
        // 　　　　[in]stride      ストライド（次のラインまでのバイト数）
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(byte[] srcImage, byte[] destImage, int cxImage, int cyImage, int stride) {
            m_srcImage = srcImage;
            m_destImage = destImage;
            m_cxImage = cxImage;
            m_cyImage = cyImage;
            m_stride = stride;
        }

        //=========================================================================================
        // 機　能：パラメータを設定する
        // 引　数：[in]paramList  パラメータ
        // 戻り値：なし
        // メ　モ：float parmaList[0]  色相の修正量（-180～180）
        // 　　　　float parmaList[1]  彩度の修正量（-1～1）
        // 　　　　float parmaList[2]  明度の修正量（-1～1）
        //=========================================================================================
        public void SetParameter(params object[] paramList) {
            m_hModify = (float)(paramList[0]);
            m_sModify = (float)(paramList[1]);
            m_vModify = (float)(paramList[2]);
        }

        //=========================================================================================
        // 機　能：フィルター処理を実行する
        // 引　数：[in]startLine   開始行
        // 　　　　[in]endLine     終了行
        // 　　　　[in]cancelEvent キャンセル時にtrueになるフラグ
        // 戻り値：なし
        //=========================================================================================
        public void FilterExecute(int startLine, int endLine, BooleanFlag cancelEvent) {
            for (int y = startLine; y <= endLine; y++) {
                int pos = y * m_stride;
                for (int x = 0; x < m_cxImage; x++) {
                    double b = (double)(m_srcImage[pos]) / 255.0;
                    double g = (double)(m_srcImage[pos + 1]) / 255.0;
                    double r = (double)(m_srcImage[pos + 2]) / 255.0;

                    // RGB→HSV変換
                    double maxColor = Math.Max(b, Math.Max(g, r));
                    double minColor = Math.Min(b, Math.Min(g, r));
                    double v = maxColor;
                    double z = minColor;
                    double s;
                    if (v == 0.0) {
                        s = 0.0;
                    } else {
                        s = (v - z) / v;
                    }
                    double rr, gg, bb;
                    if ((v - z) == 0.0) {
                        rr = 0.0;
                        gg = 0.0;
                        bb = 0.0;
                    } else {
                        rr = (v - r) / (v - z);
                        gg = (v - g) / (v - z);
                        bb = (v - b) / (v - z);
                    }
                    double h;
                    if (v == r) {
                        h = 60.0 * (bb - gg);
                    } else if (v == g) {
                        h = 60.0 * (2.0 + rr - bb);
                    } else {
                        h = 60.0 * (4.0 + gg - rr);
                    }
                    if (h < 0.0) {
                        h = h + 360.0;
                    }

                    // 補正
                    h = h + m_hModify;
                    while (h < 0.0 || 360.0 <= h) {
                        if (h < 0.0) {
                            h = h + 360.0;
                        } else if (360.0 <= h) {
                            h = h - 360.0;
                        }
                    }
                    s = Math.Max(0.0, Math.Min(1.0, s + m_sModify));
                    v = Math.Max(0.0, Math.Min(1.0, v + m_vModify));

                    // HSV→RGB変換
                    int hi = (int)(h / 60.0);
                    double fl = (h / 60.0) - hi;
                    double p = v * (1.0 - s);
                    double q = v * (1.0 - fl * s);
                    double t = v * (1.0 - (1.0 - fl) * s);
                    switch (hi) {
                       case 0: r = v; g = t; b = p; break;
                       case 1: r = q; g = v; b = p; break;
                       case 2: r = p; g = v; b = t; break;
                       case 3: r = p; g = q; b = v; break;
                       case 4: r = t; g = p; b = v; break;
                       case 5: r = v; g = p; b = q; break;
                    }

                    m_destImage[pos] = (byte)(b * 255.0);
                    m_destImage[pos + 1] = (byte)(g * 255.0);
                    m_destImage[pos + 2] = (byte)(r * 255.0);
                    pos += 3;
                }
                if (cancelEvent.Value) {
                    return;
                }
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの持つ機能のメタ情報
        //=========================================================================================
        public FilterMetaInfo MetaInfo {
            get {
                return FilterMetaInfo.HsvModifyFilter;
            }
        }
    }
}
