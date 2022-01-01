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

namespace ShellFiler.GraphicsViewer {

    //=========================================================================================
    // クラス：グラフィックビューアで使用する色
    //=========================================================================================
    public class GraphicsViewerColor {
        // グラフィックビューア 背景色
        private Color m_graphicsViewerBackColor = Color.FromArgb(0, 0, 0);

        // グラフィックビューア テキスト表示の色
        private Color m_graphicsViewerTextColor = Color.FromArgb(255, 255, 255);

        // グラフィックビューア テキスト表示影のブラシ
        private Color m_graphicsViewerTextShadowColor = Color.FromArgb(64, 64, 64);

        // グラフィックビューア 読み込み中テキスト表示の色
        private Color m_graphicsViewerLoadingTextColor = Color.FromArgb(32, 64, 192);

        // グラフィックビューア 読み込み中テキスト表示影のブラシ
        private Color m_graphicsViewerLoadingTextShadowColor = Color.FromArgb(8, 16, 48);

        //=========================================================================================
        // プロパティ：グラフィックビューア 背景色
        //=========================================================================================
        public Color GraphicsViewerBackColor {
            get {
                return m_graphicsViewerBackColor;
            }
            set {
                m_graphicsViewerBackColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア テキスト表示の色
        //=========================================================================================
        public Color GraphicsViewerTextColor {
            get {
                return m_graphicsViewerTextColor;
            }
            set {
                m_graphicsViewerTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア テキスト表示影のブラシ
        //=========================================================================================
        public Color GraphicsViewerTextShadowColor {
            get {
                return m_graphicsViewerTextShadowColor;
            }
            set {
                m_graphicsViewerTextShadowColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア 読み込み中テキスト表示の色
        //=========================================================================================
        public Color GraphicsViewerLoadingTextColor {
            get {
                return m_graphicsViewerLoadingTextColor;
            }
            set {
                m_graphicsViewerLoadingTextColor = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューア 読み込み中テキスト表示影のブラシ
        //=========================================================================================
        public Color GraphicsViewerLoadingTextShadowColor {
            get {
                return m_graphicsViewerLoadingTextShadowColor;
            }
            set {
                m_graphicsViewerLoadingTextShadowColor = value;
            }
        }
    }
}
