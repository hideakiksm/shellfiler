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
    // クラス：明るさコントラストフィルター
    //=========================================================================================
    class ComponentBright : IFilterComponent {
        // 輝度値の変換テーブル
        private byte[] m_convertTable;

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
        public ComponentBright() {
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
        // メ　モ：float parmaList[0]  明るさフィルターのレベル(-1～1)
        // 　　　　float parmaList[1]  コントラストフィルターのレベル(-1～1)
        // 　　　　float parmaList[2]  ガンマフィルターのレベル(0～2)
        //=========================================================================================
        public void SetParameter(params object[] paramList) {
            double brightValue = (float)(paramList[0]) * 256.0;
            double contrastValue = (float)(paramList[1]) + 1.0;
            double gamma = (float)paramList[2];
            m_convertTable = new byte[256];
            for (int i = 0; i < 256; i++) {
                m_convertTable[i] = (byte)Math.Max(0.0, Math.Min(255.0, (Math.Pow((double)i / 255.0, gamma) * 255.0 - 128.0) * contrastValue + 128.0 + brightValue));
            }
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
                    int srcB = m_srcImage[pos];
                    m_destImage[pos++] = m_convertTable[srcB];
                    int srcG = m_srcImage[pos];
                    m_destImage[pos++] = m_convertTable[srcG];
                    int srcR = m_srcImage[pos];
                    m_destImage[pos++] = m_convertTable[srcR];
                    m_destImage[pos] = m_srcImage[pos];
                    pos++;
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
                return FilterMetaInfo.BrightFilter;
            }
        }
    }
}
