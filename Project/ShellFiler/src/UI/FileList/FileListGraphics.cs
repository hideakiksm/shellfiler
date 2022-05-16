using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using System.Runtime.InteropServices;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：ファイル一覧の描画用グラフィックス
    //=========================================================================================
    public class FileListGraphics : HighDpiGraphics {
        // 一覧の行の開始Y位置
        private int m_lineStart;

        // 一覧の行の高さ
        private int m_lineHeight;
        
        // ファイル一覧 カーソル描画用のペン
        private Pen m_fileListCursorPen = null;
        
        // ファイル一覧 無効状態でのカーソル描画用のペン
        private Pen m_fileListCursorDisablePen = null;

        // ファイル一覧 背景色での描画用のペン（消去用）
        private Pen m_fileListBackPen = null;
        
        // ファイル一覧 背景の枠描画用のペン
        private Pen m_fileListMarkBorderPen = null;

        // ファイル一覧 グレーアウト背景の枠描画用のペン
        private Pen m_fileListMarkGrayBorderPen = null;

        // ファイル一覧 サムネイルの枠の色1
        private Pen m_fileListThumbnailFramePen1 = null;

        // ファイル一覧 サムネイルの枠の色2
        private Pen m_fileListThumbnailFramePen2 = null;

        // ファイル一覧 背景描画用のブラシ
        private Brush m_fileListBackBrush = null;

        // ファイル一覧 通常のファイルのブラシ
        private Brush m_fileListTextBrush = null;

        // ファイル一覧 読み込み専用ファイルのブラシ
        private Brush m_fileListReadonlyBrush = null;

        // ファイル一覧 隠しファイルのブラシ
        private Brush m_fileListHiddenBrush = null;

        // ファイル一覧 システムファイルのブラシ
        private Brush m_fileListSystemBrush = null;

        // ファイル一覧 シンボリックリンクのブラシ
        private Brush m_fileListSymlinkBrush = null;

        // ファイル一覧 アーカイブのブラシ
        public Brush m_fileListArchiveBrush = null;

        // ファイル一覧 グレーアウトテキストのブラシ
        private Brush m_fileListGrayBrush = null;

        // ファイル一覧 グレーアウト背景色のブラシ
        private Brush m_fileListGrayBackBrush = null;

        // ファイル一覧 マーク中文字色のブラシ
        private Brush m_fileListMarkBrush = null;

        // ファイル一覧 マーク中背景のブラシ
        private Brush m_fileListMarkBackBrush = null;

        // ファイル一覧 マーク中グレーアウト文字色のブラシ
        private Brush m_fileListMarkGrayBrush = null;

        // ファイル一覧 マーク中グレーアウト背景色のブラシ
        private Brush m_fileListMarkGrayBackBrush = null;

        // ファイル一覧 描画用のフォント
        private Font m_fileListFont = null;

        // サムネイルファイル一覧 描画用のフォント
        private Font m_thumbFileListFont = null;

        // サムネイルファイル一覧 描画用のフォント（小）
        private Font m_thumbFileListSmallFont = null;

        // 描画時のフォーマッタ（省略記号付き）
        private StringFormat m_stringFormatEllipsis = null;

        // 描画時のフォーマッタ（省略記号付き中央寄せ）
        private StringFormat m_stringFormatEllipsisCenter = null;

        // 描画時のフォーマッタ（右寄せ）
        private StringFormat m_stringFormatRight = null;

        // モノクロ表示用のImageAttributes
        private ImageAttributes m_monochromeAttributes = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics  グラフィックス
        // 　　　　[in]lineStart  一覧の行の開始Y位置
        // 　　　　[in]lineHeight 一覧の行の高さ
        // 戻り値：なし
        //=========================================================================================
        public FileListGraphics(Graphics graphics, int lineStart, int lineHeight) : base(graphics) {
            m_lineStart = lineStart;
            m_lineHeight = lineHeight;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（コントロール指定で必要時に初期化）
        // 引　数：[in]control    描画対象のコントロール
        // 　　　　[in]lineStart  一覧の行の開始Y位置
        // 　　　　[in]lineHeight 一覧の行の高さ
        // 戻り値：なし
        //=========================================================================================
        public FileListGraphics(Control control, int lineStart, int lineHeight) : base(control) {
            m_lineStart = lineStart;
            m_lineHeight = lineHeight;
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public new void Dispose() {
            if (m_fileListCursorPen != null) {
                m_fileListCursorPen.Dispose();
                m_fileListCursorPen = null;
            }
            if (m_fileListCursorDisablePen != null) {
                m_fileListCursorDisablePen.Dispose();
                m_fileListCursorDisablePen = null;
            }
            if (m_fileListBackPen != null) {
                m_fileListBackPen.Dispose();
                m_fileListBackPen = null;
            }
            if (m_fileListMarkBorderPen != null) {
                m_fileListMarkBorderPen.Dispose();
                m_fileListMarkBorderPen = null;
            }
            if (m_fileListMarkGrayBorderPen != null) {
                m_fileListMarkGrayBorderPen.Dispose();
                m_fileListMarkGrayBorderPen = null;
            }
            if (m_fileListThumbnailFramePen1 != null) {
                m_fileListThumbnailFramePen1.Dispose();
                m_fileListThumbnailFramePen1 = null;
            }
            if (m_fileListThumbnailFramePen2 != null) {
                m_fileListThumbnailFramePen2.Dispose();
                m_fileListThumbnailFramePen2 = null;
            }
            if (m_fileListBackBrush != null) {
                m_fileListBackBrush.Dispose();
                m_fileListBackBrush = null;
            }
            if (m_fileListTextBrush != null) {
                m_fileListTextBrush.Dispose();
                m_fileListTextBrush = null;
            }
            if (m_fileListReadonlyBrush != null) {
                m_fileListReadonlyBrush.Dispose();
                m_fileListReadonlyBrush = null;
            }
            if (m_fileListHiddenBrush != null) {
                m_fileListHiddenBrush.Dispose();
                m_fileListHiddenBrush = null;
            }
            if (m_fileListSystemBrush != null) {
                m_fileListSystemBrush.Dispose();
                m_fileListSystemBrush = null;
            }
            if (m_fileListSymlinkBrush != null) {
                m_fileListSymlinkBrush.Dispose();
                m_fileListSymlinkBrush = null;
            }
            if (m_fileListArchiveBrush != null) {
                m_fileListArchiveBrush.Dispose();
                m_fileListArchiveBrush = null;
            }
            if (m_fileListGrayBrush != null) {
                m_fileListGrayBrush.Dispose();
                m_fileListGrayBrush = null;
            }
            if (m_fileListGrayBackBrush != null) {
                m_fileListGrayBackBrush.Dispose();
                m_fileListGrayBackBrush = null;
            }
            if (m_fileListMarkBrush != null) {
                m_fileListMarkBrush.Dispose();
                m_fileListMarkBrush = null;
            }
            if (m_fileListMarkBackBrush != null) {
                m_fileListMarkBackBrush.Dispose();
                m_fileListMarkBackBrush = null;
            }
            if (m_fileListMarkGrayBrush != null) {
                m_fileListMarkGrayBrush.Dispose();
                m_fileListMarkGrayBrush = null;
            }
            if (m_fileListMarkGrayBackBrush != null) {
                m_fileListMarkGrayBackBrush.Dispose();
                m_fileListMarkGrayBackBrush = null;
            }
            if (m_fileListFont != null) {
                m_fileListFont.Dispose();
                m_fileListFont = null;
            }
            if (m_thumbFileListFont != null) {
                m_thumbFileListFont.Dispose();
                m_thumbFileListFont = null;
            }
            if (m_thumbFileListSmallFont != null) {
                m_thumbFileListSmallFont.Dispose();
                m_thumbFileListSmallFont = null;
            }
            if (m_stringFormatEllipsis != null) {
                m_stringFormatEllipsis.Dispose();
                m_stringFormatEllipsis = null;
            }
            if (m_stringFormatEllipsisCenter != null) {
                m_stringFormatEllipsisCenter.Dispose();
                m_stringFormatEllipsisCenter = null;
            }
            if (m_stringFormatRight != null) {
                m_stringFormatRight.Dispose();
                m_stringFormatRight = null;
            }
            if (m_monochromeAttributes != null) {
                m_monochromeAttributes.Dispose();
                m_monochromeAttributes = null;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 カーソル描画用のペン
        //=========================================================================================
        public Pen FileListCursorPen {
            get {
                if (m_fileListCursorPen == null) {
                    int size = (int) (MainWindowForm.Xf(1.0f) + 0.5f);
                    m_fileListCursorPen = new Pen(Configuration.Current.FileListCursorColor, size);
                }
                return m_fileListCursorPen;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 無効状態でのカーソル描画用のペン
        //=========================================================================================
        public Pen FileListCursorDisablePen {
            get {
                if (m_fileListCursorDisablePen == null) {
                    int size = (int)(MainWindowForm.Xf(1.0f) + 0.5f);
                    m_fileListCursorDisablePen = new Pen(Configuration.Current.FileListCursorDisableColor, size);
                }
                return m_fileListCursorDisablePen;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイル一覧 背景色での描画用のペン（消去用）
        //=========================================================================================
        public Pen FileListBackPen {
            get {
                if (m_fileListBackPen == null) {
                    int size = (int)(MainWindowForm.Xf(1.0f) + 0.5f);
                    m_fileListBackPen = new Pen(Configuration.Current.FileListBackColor, size);
                }
                return m_fileListBackPen;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 背景の枠描画用のペン
        //=========================================================================================
        public Pen FileListMarkBoderPen {
            get {
                if (m_fileListMarkBorderPen == null) {
                    m_fileListMarkBorderPen = new Pen(Configuration.Current.FileListMarkBackBorderColor);
                }
                return m_fileListMarkBorderPen;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 グレーアウト背景の枠描画用のペン
        //=========================================================================================
        public Pen FileListMarkGrayBoderPen {
            get {
                if (m_fileListMarkGrayBorderPen == null) {
                    m_fileListMarkGrayBorderPen = new Pen(Configuration.Current.FileListMarkGrayBackBorderColor);
                }
                return m_fileListMarkGrayBorderPen;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 サムネイルの枠の色1描画用のペン
        //=========================================================================================
        public Pen FileListThumbnailFramePen1 {
            get {
                if (m_fileListThumbnailFramePen1 == null) {
                    m_fileListThumbnailFramePen1 = new Pen(Configuration.Current.FileListThumbnailFrameColor1);
                }
                return m_fileListThumbnailFramePen1;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 サムネイルの枠の色2描画用のペン
        //=========================================================================================
        public Pen FileListThumbnailFramePen2 {
            get {
                if (m_fileListThumbnailFramePen2 == null) {
                    m_fileListThumbnailFramePen2 = new Pen(Configuration.Current.FileListThumbnailFrameColor2);
                }
                return m_fileListThumbnailFramePen2;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 背景描画用のブラシ
        //=========================================================================================
        public Brush FileListBackBrush {
            get {
                if (m_fileListBackBrush == null) {
                    m_fileListBackBrush = new SolidBrush(Configuration.Current.FileListBackColor);
                }
                return m_fileListBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 通常のファイルのブラシ
        //=========================================================================================
        public Brush FileListTextBrush {
            get {
                if (m_fileListTextBrush == null) {
                    m_fileListTextBrush = new SolidBrush(Configuration.Current.FileListFileTextColor);
                }
                return m_fileListTextBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 読み込み専用ファイルのブラシ
        //=========================================================================================
        public Brush FileListReadonlyBrush {
            get {
                if (m_fileListReadonlyBrush == null) {
                    m_fileListReadonlyBrush = new SolidBrush(Configuration.Current.FileListReadOnlyColor);
                }
                return m_fileListReadonlyBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 隠しファイルのブラシ
        //=========================================================================================
        public Brush FileListHiddenBrush {
            get {
                if (m_fileListHiddenBrush == null) {
                    m_fileListHiddenBrush = new SolidBrush(Configuration.Current.FileListHiddenColor);
                }
                return m_fileListHiddenBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 システムファイルのブラシ
        //=========================================================================================
        public Brush FileListSystemBrush {
            get {
                if (m_fileListSystemBrush == null) {
                    m_fileListSystemBrush = new SolidBrush(Configuration.Current.FileListSystemColor);
                }
                return m_fileListSystemBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 シンボリックリンクのブラシ
        //=========================================================================================
        public Brush FileListSymlinkBrush {
            get {
                if (m_fileListSymlinkBrush == null) {
                    m_fileListSymlinkBrush = new SolidBrush(Configuration.Current.FileListSymlinkColor);
                }
                return m_fileListSymlinkBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 アーカイブのブラシ
        //=========================================================================================
        public Brush FileListArchiveBrush {
            get {
                if (m_fileListArchiveBrush == null) {
                    m_fileListArchiveBrush = new SolidBrush(Configuration.Current.FileListArchiveColor);
                }
                return m_fileListArchiveBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 グレーアウトテキストのブラシ
        //=========================================================================================
        public Brush FileListGrayBrush {
            get {
                if (m_fileListGrayBrush == null) {
                    m_fileListGrayBrush = new SolidBrush(Configuration.Current.FileListGrayColor);
                }
                return m_fileListGrayBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 グレーアウト背景色のブラシ
        //=========================================================================================
        public Brush FileListGrayBackBrush {
            get {
                if (m_fileListGrayBackBrush == null) {
                    m_fileListGrayBackBrush = new SolidBrush(Configuration.Current.FileListGrayBackColor);
                }
                return m_fileListGrayBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中文字色のブラシ
        //=========================================================================================
        public Brush FileListMarkBrush {
            get {
                if (m_fileListMarkBrush == null) {
                    m_fileListMarkBrush = new SolidBrush(Configuration.Current.FileListMarkColor);
                }
                return m_fileListMarkBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中背景色のブラシ
        //=========================================================================================
        public Brush FileListMarkBackBrush {
            get {
                if (m_fileListMarkBackBrush == null) {
                    if (Configuration.Current.FileListMarkBackColor1.Equals(Configuration.Current.FileListMarkBackColor2)) {
                        m_fileListMarkBackBrush = new SolidBrush(Configuration.Current.FileListMarkBackColor1);
                    } else {
                        Rectangle rc = new Rectangle(0, m_lineStart, 10, m_lineHeight);
                        m_fileListMarkBackBrush = new LinearGradientBrush(rc, Configuration.Current.FileListMarkBackColor1, Configuration.Current.FileListMarkBackColor2, LinearGradientMode.Vertical);
                    }
                }
                return m_fileListMarkBackBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中グレーアウト文字色のブラシ
        //=========================================================================================
        public Brush FileListMarkGrayBrush {
            get {
                if (m_fileListMarkGrayBrush == null) {
                    m_fileListMarkGrayBrush = new SolidBrush(Configuration.Current.FileListMarkGrayColor);
                }
                return m_fileListMarkGrayBrush;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧 マーク中グレーアウト背景色のブラシ
        //=========================================================================================
        public Brush FileListMarkGrayBackBrush {
            get {
                if (m_fileListMarkGrayBackBrush == null) {
                    if (Configuration.Current.FileListMarkGrayBackColor1.Equals(Configuration.Current.FileListMarkGrayBackColor2)) {
                        m_fileListMarkGrayBackBrush = new SolidBrush(Configuration.Current.FileListMarkGrayBackColor1);
                    } else {
                        Rectangle rc = new Rectangle(0, m_lineStart, 10, m_lineHeight);
                        m_fileListMarkGrayBackBrush = new LinearGradientBrush(rc, Configuration.Current.FileListMarkGrayBackColor1, Configuration.Current.FileListMarkGrayBackColor2, LinearGradientMode.Vertical);
                    } 
                }
                return m_fileListMarkGrayBackBrush;
            }
        }
        
        //=========================================================================================
        // プロパティ：ファイル一覧 描画用のフォント
        //=========================================================================================
        public Font FileListFont {
            get {
                if (m_fileListFont == null) {
                    m_fileListFont = new Font(Configuration.Current.ListViewFontName, Configuration.Current.ListViewFontSize);
                }
                return m_fileListFont;
            }
        }
        
        //=========================================================================================
        // プロパティ：サムネイルファイル一覧 描画用のフォント
        //=========================================================================================
        public Font ThumbFileListFont {
            get {
                if (m_thumbFileListFont == null) {
                    m_thumbFileListFont = new Font(Configuration.Current.ThumbFileListViewFontName, Configuration.Current.ThumbFileListViewFontSize);
                }
                return m_thumbFileListFont;
            }
        }
        
        //=========================================================================================
        // プロパティ：サムネイルファイル一覧 描画用のフォント（小）
        //=========================================================================================
        public Font ThumbFileListSmallFont {
            get {
                if (m_thumbFileListSmallFont == null) {
                    m_thumbFileListSmallFont = new Font(Configuration.Current.ThumbFileListViewFontName, Configuration.Current.ThumbFileListViewSmallFontSize);
                }
                return m_thumbFileListSmallFont;
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
                }
                return m_stringFormatEllipsis;
            }
        }

        //=========================================================================================
        // プロパティ：省略記号付き中央寄せのフォーマッタ
        //=========================================================================================
        public StringFormat StringFormatEllipsisCenter {
            get {
                if (m_stringFormatEllipsisCenter == null) {
                    m_stringFormatEllipsisCenter = new StringFormat();
                    m_stringFormatEllipsisCenter.Alignment = StringAlignment.Center;
                    m_stringFormatEllipsisCenter.Trimming = StringTrimming.EllipsisCharacter;
                }
                return m_stringFormatEllipsisCenter;
            }
        }

        //=========================================================================================
        // プロパティ：右寄せのフォーマッタ
        //=========================================================================================
        public StringFormat StringFormatRight {
            get {
                if (m_stringFormatRight == null) {
                    m_stringFormatRight = new StringFormat();
                    m_stringFormatRight.Alignment = StringAlignment.Far;
                }
                return m_stringFormatRight;
            }
        }

        //=========================================================================================
        // プロパティ：モノクロ表示用のImageAttributes
        //=========================================================================================
        public ImageAttributes MonochromeAttributes {
            get {
                if (m_monochromeAttributes == null) {
                    ColorMatrix cm = new ColorMatrix(
                        new float[][]{
                            new float[]{0.299f, 0.299f, 0.299f, 0 ,0},
                            new float[]{0.587f, 0.587f, 0.587f, 0, 0},
                            new float[]{0.114f, 0.114f, 0.114f, 0, 0},
                            new float[]{0, 0, 0, 1, 0},
                            new float[]{0, 0, 0, 0, 1}
                    });
                    m_monochromeAttributes = new System.Drawing.Imaging.ImageAttributes();
                    m_monochromeAttributes.SetColorMatrix(cm);
                }
                return m_monochromeAttributes;
            }
        }
    }
}
