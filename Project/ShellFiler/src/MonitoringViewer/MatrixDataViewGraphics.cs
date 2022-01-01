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

namespace ShellFiler.MonitoringViewer {

    //=========================================================================================
    // クラス：マトリックスの描画用グラフィックス
    //=========================================================================================
    public class MatrixDataViewGraphics {
        // グラフィック（null:未初期化）
        private Graphics m_graphics;

        // 選択中背景描画用のブラシ
        private Brush m_selectBackBrush = null;

        // 選択中文字描画用のブラシ
        private Brush m_selectTextBrush = null;

        // ヒット中背景描画用のブラシ
        private Brush m_hitBackBrush = null;

        // ヒット中文字描画用のブラシ
        private Brush m_hitTextBrush = null;

        // 文字描画用のブラシ
        private Brush m_textBrush = null;

        // 描画時のフォーマッタ（省略記号付き）
        private StringFormat m_stringFormatEllipsis = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]graphics  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public MatrixDataViewGraphics(Graphics graphics) {
            m_graphics = graphics;
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_selectBackBrush != null) {
                m_selectBackBrush.Dispose();
                m_selectBackBrush = null;
            }
            if (m_selectTextBrush != null) {
                m_selectTextBrush.Dispose();
                m_selectTextBrush = null;
            }
            if (m_hitBackBrush != null) {
                m_hitBackBrush.Dispose();
                m_hitBackBrush = null;
            }
            if (m_hitTextBrush != null) {
                m_hitTextBrush.Dispose();
                m_hitTextBrush = null;
            }
            if (m_stringFormatEllipsis != null) {
                m_stringFormatEllipsis.Dispose();
                m_stringFormatEllipsis = null;
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
        // プロパティ：選択中背景描画用のブラシ
        //=========================================================================================
        public Brush SelectBackBrush {
            get {
                if (m_selectBackBrush == null) {
                    m_selectBackBrush = new SolidBrush(Configuration.Current.TextViewerSelectBackColor);
                }
                return m_selectBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：選択中文字描画用のブラシ
        //=========================================================================================
        public Brush SelectTextBrush {
            get {
                if (m_selectTextBrush == null) {
                    m_selectTextBrush = new SolidBrush(Configuration.Current.TextViewerSelectTextColor);
                }
                return m_selectTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ヒット中背景描画用のブラシ
        //=========================================================================================
        public Brush HitBackBrush {
            get {
                if (m_hitBackBrush == null) {
                    m_hitBackBrush = new SolidBrush(Configuration.Current.TextViewerSearchHitBackColor);
                }
                return m_hitBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ヒット中文字描画用のブラシ
        //=========================================================================================
        public Brush HitTextBrush {
            get {
                if (m_hitTextBrush == null) {
                    m_hitTextBrush = new SolidBrush(Configuration.Current.TextViewerSearchHitTextColor);
                }
                return m_hitTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：文字描画用のブラシ
        //=========================================================================================
        public Brush TextBrush {
            get {
                if (m_textBrush == null) {
                    m_textBrush = new SolidBrush(Configuration.Current.TextViewerTextColor);
                }
                return m_textBrush;
            }
        }
        
        //=========================================================================================
        // プロパティ：省略記号付きのフォーマッタ
        //=========================================================================================
        public StringFormat StringFormatEllipsis {
            get {
                if (m_stringFormatEllipsis == null) {
                    m_stringFormatEllipsis = new StringFormat();
                    m_stringFormatEllipsis.Trimming = StringTrimming.EllipsisCharacter;
                    m_stringFormatEllipsis.LineAlignment = StringAlignment.Center;
                    m_stringFormatEllipsis.FormatFlags = StringFormatFlags.NoWrap;
                }
                return m_stringFormatEllipsis;
            }
        }
    }
}
