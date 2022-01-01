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
    // クラス：ぼかしフィルター
    //=========================================================================================
    class ComponentBlur : IFilterComponent {
        // フィルターのレベル
        private float m_filterLevel;

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
        public ComponentBlur() {
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
        // メ　モ：float parmaList[0]  ぼかしのレベル(0～1)
        //=========================================================================================
        public void SetParameter(params object[] paramList) {
            m_filterLevel = (float)paramList[0] / 8.0f;
        }

        //=========================================================================================
        // 機　能：フィルター処理を実行する
        // 引　数：[in]startLine   開始行
        // 　　　　[in]endLine     終了行
        // 　　　　[in]cancelEvent キャンセル時にtrueになるフラグ
        // 戻り値：なし
        //=========================================================================================
        public void FilterExecute(int startLine, int endLine, BooleanFlag cancelEvent) {
            // フィルター値
            // 1  1  1
            // 1 -8  1
            // 1  1  1
            if (m_cxImage <= 3) {
                for (int y = startLine; y <= endLine; y++) {
                    FilterSafe(y);
                    if (cancelEvent.Value) {
                        return;
                    }
                }
            } else {
                for (int y = startLine; y <= endLine; y++) {
                    FilterOptimize(y);
                    if (cancelEvent.Value) {
                        return;
                    }
                }
            }
        }

        //=========================================================================================
        // 機　能：安全な実装により端数処理を行う
        // 引　数：[in]y   処理対象の行
        // 戻り値：なし
        //=========================================================================================
        private void FilterSafe(int y) {
            int y1 = Math.Max(0, y - 1);
            int y2 = y;
            int y3 = Math.Min(m_cyImage - 1, y + 1);
            int pos1 = y1 * m_stride;
            int pos2 = y2 * m_stride;
            int pos3 = y3 * m_stride;
            for (int x = 0; x < m_cxImage; x++) {
                SetPixel(pos1, pos2, pos3, x);
            }
        }

        //=========================================================================================
        // 機　能：安全な実装により端数処理を行う（ピクセルのセット）
        // 引　数：[in]pos1  (0, y-1)のピクセルデータのインデックス
        // 　　　　[in]pos2  (0, y)のピクセルデータのインデックス
        // 　　　　[in]pos3  (0, y+1)のピクセルデータのインデックス
        // 　　　　[in]x     設定するx位置
        // 戻り値：なし
        //=========================================================================================
        private void SetPixel(int pos1, int pos2, int pos3, int x) {
            int x1 = Math.Max(0, x - 1);
            int x2 = x;
            int x3 = Math.Min(m_cxImage - 1, x + 1);

            int srcB11 = m_srcImage[pos1 + x1 * 3];
            int srcB21 = m_srcImage[pos1 + x2 * 3];
            int srcB31 = m_srcImage[pos1 + x3 * 3];
            int srcB12 = m_srcImage[pos2 + x1 * 3];
            int srcB22 = m_srcImage[pos2 + x2 * 3];
            int srcB32 = m_srcImage[pos2 + x3 * 3];
            int srcB13 = m_srcImage[pos3 + x1 * 3];
            int srcB23 = m_srcImage[pos3 + x2 * 3];
            int srcB33 = m_srcImage[pos3 + x3 * 3];

            int srcG11 = m_srcImage[pos1 + x1 * 3 + 1];
            int srcG21 = m_srcImage[pos1 + x2 * 3 + 1];
            int srcG31 = m_srcImage[pos1 + x3 * 3 + 1];
            int srcG12 = m_srcImage[pos2 + x1 * 3 + 1];
            int srcG22 = m_srcImage[pos2 + x2 * 3 + 1];
            int srcG32 = m_srcImage[pos2 + x3 * 3 + 1];
            int srcG13 = m_srcImage[pos3 + x1 * 3 + 1];
            int srcG23 = m_srcImage[pos3 + x2 * 3 + 1];
            int srcG33 = m_srcImage[pos3 + x3 * 3 + 1];

            int srcR11 = m_srcImage[pos1 + x1 * 3 + 2];
            int srcR21 = m_srcImage[pos1 + x2 * 3 + 2];
            int srcR31 = m_srcImage[pos1 + x3 * 3 + 2];
            int srcR12 = m_srcImage[pos2 + x1 * 3 + 2];
            int srcR22 = m_srcImage[pos2 + x2 * 3 + 2];
            int srcR32 = m_srcImage[pos2 + x3 * 3 + 2];
            int srcR13 = m_srcImage[pos3 + x1 * 3 + 2];
            int srcR23 = m_srcImage[pos3 + x2 * 3 + 2];
            int srcR33 = m_srcImage[pos3 + x3 * 3 + 2];

            m_destImage[pos2 + x * 3    ] = (byte)(Math.Min(255f, Math.Max(0f, (srcB11 + srcB21 + srcB31 + srcB12 + srcB32 + srcB13 + srcB23 + srcB33 - srcB22 * 8) * m_filterLevel + srcB22)));
            m_destImage[pos2 + x * 3 + 1] = (byte)(Math.Min(255f, Math.Max(0f, (srcG11 + srcG21 + srcG31 + srcG12 + srcG32 + srcG13 + srcG23 + srcG33 - srcG22 * 8) * m_filterLevel + srcG22)));
            m_destImage[pos2 + x * 3 + 2] = (byte)(Math.Min(255f, Math.Max(0f, (srcR11 + srcR21 + srcR31 + srcR12 + srcR32 + srcR13 + srcR23 + srcR33 - srcR22 * 8) * m_filterLevel + srcR22)));
        }

        //=========================================================================================
        // 機　能：最適化された実装によりフィルター処理を行う
        // 引　数：[in]y   処理対象の行
        // 戻り値：なし
        //=========================================================================================
        private void FilterOptimize(int y) {
            int y1 = Math.Max(0, y - 1);
            int y2 = y;
            int y3 = Math.Min(m_cyImage - 1, y + 1);
            int pos1 = y1 * m_stride;
            int pos2 = y2 * m_stride;
            int pos3 = y3 * m_stride;

            // x=0位置の端数処理
            SetPixel(pos1, pos2, pos3, 0);

            pos1 += 3;
            pos2 += 3;
            pos3 += 3;

            // x=中間ピクセルの処理
            for (int x = 1; x < m_cxImage - 1; x++) {
                int srcB11 = m_srcImage[pos1 - 3];
                int srcB21 = m_srcImage[pos1];
                int srcB31 = m_srcImage[pos1 + 3];
                int srcB12 = m_srcImage[pos2 - 3];
                int srcB22 = m_srcImage[pos2];
                int srcB32 = m_srcImage[pos2 + 3];
                int srcB13 = m_srcImage[pos3 - 3];
                int srcB23 = m_srcImage[pos3];
                int srcB33 = m_srcImage[pos3 + 3];

                int srcG11 = m_srcImage[pos1 - 3 + 1];
                int srcG21 = m_srcImage[pos1     + 1];
                int srcG31 = m_srcImage[pos1 + 3 + 1];
                int srcG12 = m_srcImage[pos2 - 3 + 1];
                int srcG22 = m_srcImage[pos2     + 1];
                int srcG32 = m_srcImage[pos2 + 3 + 1];
                int srcG13 = m_srcImage[pos3 - 3 + 1];
                int srcG23 = m_srcImage[pos3     + 1];
                int srcG33 = m_srcImage[pos3 + 3 + 1];

                int srcR11 = m_srcImage[pos1 - 3 + 2];
                int srcR21 = m_srcImage[pos1     + 2];
                int srcR31 = m_srcImage[pos1 + 3 + 2];
                int srcR12 = m_srcImage[pos2 - 3 + 2];
                int srcR22 = m_srcImage[pos2     + 2];
                int srcR32 = m_srcImage[pos2 + 3 + 2];
                int srcR13 = m_srcImage[pos3 - 3 + 2];
                int srcR23 = m_srcImage[pos3     + 2];
                int srcR33 = m_srcImage[pos3 + 3 + 2];

                m_destImage[pos2    ] = (byte)(Math.Min(255f, Math.Max(0f, (srcB11 + srcB21 + srcB31 + srcB12 + srcB32 + srcB13 + srcB23 + srcB33 - srcB22 * 8) * m_filterLevel + srcB22)));
                m_destImage[pos2 + 1] = (byte)(Math.Min(255f, Math.Max(0f, (srcG11 + srcG21 + srcG31 + srcG12 + srcG32 + srcG13 + srcG23 + srcG33 - srcG22 * 8) * m_filterLevel + srcG22)));
                m_destImage[pos2 + 2] = (byte)(Math.Min(255f, Math.Max(0f, (srcR11 + srcR21 + srcR31 + srcR12 + srcR32 + srcR13 + srcR23 + srcR33 - srcR22 * 8) * m_filterLevel + srcR22)));

                pos1 += 3;
                pos2 += 3;
                pos3 += 3;
            }

            // x=n-1位置の端数処理
            SetPixel(y1 * m_stride, y2 * m_stride, y3 * m_stride, m_cxImage - 1);
        }

        //=========================================================================================
        // プロパティ：フィルターの持つ機能のメタ情報
        //=========================================================================================
        public FilterMetaInfo MetaInfo {
            get {
                return FilterMetaInfo.BlurFilter;
            }
        }
    }
}
