using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using System.Runtime.InteropServices;

namespace ShellFiler.UI.FileList.ThumbList {

    //=========================================================================================
    // クラス：サムネイルでのファイル一覧の行を描画するためのクラス
    //=========================================================================================
    class ThumbListRenderer {
        // 画像から境界線までのマージン
        public const int MARGIN_IMAGE_BORDER = 3;

        // 画像からフォントまでのマージン
        private const int MARGIN_IMAGE_FONT = 0;

        // 項目同士のマージン
        public const int MARGIN_ITEM = 1;

        // 項目同士のマージン
        public const int ITEM_MARGIN = 4;

        // フォント描画開始位置の調整用
        private const int CY_FONT_ADJUST = 2;

        // アイコン描画位置の調整用
        public const int CY_ICON_ADJUST = 1;

        // ファイルアイコンの管理クラス
        private FileIconManager m_fileIconManager;

        // ビューの表示モード
        private FileListViewMode m_fileListViewMode;

        // ファイル1つ分の大きさ
        private Size m_fileItemSize;

        // 画像の大きさ
        private Size m_imageSize;

        // フォント描画位置までのYオフセット
        private int m_yFontOffset;

        // フォント描画位置までのXオフセット
        private int m_xFontOffset;

        // フォントの高さ
        private int m_cyFont;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]viewMode  ビューの表示モード
        // 戻り値：なし
        //=========================================================================================
        public ThumbListRenderer(FileListViewMode viewMode) {
            m_fileIconManager = Program.Document.FileIconManager;

            InitializeLayout(viewMode);

            Refresh();
        }

        //=========================================================================================
        // 機　能：ビューのレイアウトを初期化する
        // 引　数：[in]viewMode  ビューの表示モード
        // 戻り値：なし
        //=========================================================================================
        private void InitializeLayout(FileListViewMode viewMode) {
            m_fileListViewMode = viewMode;
            if (viewMode.ThumbnailName == FileListViewThumbnailName.ThumbNameSpearate) {
                Font font = new Font(Configuration.Current.ThumbFileListViewFontName, Configuration.Current.ThumbFileListViewFontSize);
                m_cyFont = font.Height;
                font.Dispose();

                int size = viewMode.ThumbnailSize.ImageSize;
                m_imageSize = new Size(size, size);
                m_fileItemSize = new Size(m_imageSize.Width + MARGIN_IMAGE_BORDER * 2, m_imageSize.Height + MARGIN_IMAGE_BORDER * 2 + MARGIN_IMAGE_FONT + m_cyFont);
                m_xFontOffset = 0;
                m_yFontOffset = m_imageSize.Height + MARGIN_IMAGE_BORDER + MARGIN_IMAGE_FONT;
            } else if (viewMode.ThumbnailName == FileListViewThumbnailName.ThumbNameOverray ||
                       viewMode.ThumbnailName == FileListViewThumbnailName.ThumbNameNone) {
                Font font = new Font(Configuration.Current.ThumbFileListViewFontName, Configuration.Current.ThumbFileListViewSmallFontSize);
                m_cyFont = font.Height;
                font.Dispose();

                int size = viewMode.ThumbnailSize.ImageSize;
                m_imageSize = new Size(size, size);
                m_fileItemSize = new Size(m_imageSize.Width + MARGIN_IMAGE_BORDER * 2, m_imageSize.Height + MARGIN_IMAGE_BORDER * 2);
                m_xFontOffset = 2;
                m_yFontOffset = m_imageSize.Height - m_cyFont + MARGIN_IMAGE_BORDER;
            } else {
                Program.Abort("サムネイルのファイル名表示モードが未定義です。");
            }
        }

        //=========================================================================================
        // 機　能：再読込時に状態をリフレッシュする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Refresh() {
        }

        // 行の描画用：描画対象のファイル
        private UIFile m_lineFile;

        // 行の描画用：フォントの背景ブラシ
        private Brush m_lineFontBrush;
        
        //=========================================================================================
        // 機　能：カーソルを描画する
        // 引　数：[in]g          描画に使用するグラフィックス
        // 　　　　[in]xPos       画面上の表示Ｘ位置
        // 　　　　[in]yPos       画面上の表示Ｙ位置
        // 　　　　[in]idxTarget  対象のファイル一覧中のインデックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawCursor(FileListGraphics g, int xPos, int yPos, bool withCursor) {
            Rectangle rect = new Rectangle(xPos, yPos, FileItemSize.Width - 1, FileItemSize.Height - 1);
            if (withCursor) {
                g.Graphics.DrawRectangle(g.FileListCursorPen, rect);
            } else {
                g.Graphics.DrawRectangle(g.FileListBackPen, rect);
            }
        }

        //=========================================================================================
        // 機　能：画面を描画する
        // 引　数：[in]g           描画に使用するグラフィックス
        // 　　　　[in]xPos        画面上の表示Ｘ位置
        // 　　　　[in]yPos        画面上の表示Ｙ位置
        // 　　　　[in]isActive    アクティブ状態で描画するときtrue
        // 　　　　[in]withCursor  カーソルを描画するときtrue
        // 　　　　[in]targetFile  対象のファイル
        // 戻り値：なし
        //=========================================================================================
        public void DrawItem(FileListGraphics g, int xPos, int yPos, bool isActive, UIFile targetFile, bool withCursor) {
            // 描画内容を決定
            m_lineFile = targetFile;
            m_lineFontBrush = GetTextDrawBrush(g, m_lineFile, isActive);
            try {
                Rectangle rectItem = new Rectangle(xPos, yPos, FileItemSize.Width, FileItemSize.Height);
                DrawItemImpl(g, rectItem, isActive);
                DrawCursor(g, xPos, yPos, withCursor);
            } finally {
                m_lineFile = null;
                m_lineFontBrush = null;
            }
        }
        
        //=========================================================================================
        // 機　能：描画処理の実装
        // 引　数：[in]g          描画に使用するグラフィックス
        // 　　　　[in]rectItem   描画する１項目全体（FileItemSize相当）の領域
        // 　　　　[in]isActive   アクティブ状態で描画するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void DrawItemImpl(FileListGraphics g, Rectangle rectItem, bool isActive) {
            // 背景を描画
            Brush backBrush = GetBackDrawBrush(g, m_lineFile, true, isActive);
            if (backBrush != null) {
                if (m_lineFile.Marked) {
                    Rectangle rect = new Rectangle(rectItem.Left, rectItem.Top, FileItemSize.Width - 1, FileItemSize.Height - 1);
                    g.Graphics.DrawRectangle(g.FileListBackPen, rect);
                    Rectangle rectBack = new Rectangle(rectItem.Left + 2, rectItem.Top + 2, rectItem.Width - 4, rectItem.Height - 4);
                    g.Graphics.FillRectangle(backBrush, rectBack);
                } else {
                    g.Graphics.FillRectangle(backBrush, rectItem);
                }
            }

            // 枠を描画
            Pen markBorderPen;
            if (isActive) {
                markBorderPen = g.FileListMarkBoderPen;
            } else {
                markBorderPen = g.FileListMarkGrayBoderPen;
            }
            if (m_lineFile.Marked) {                   // マーク
                g.Graphics.DrawLine(g.FileListMarkBoderPen, rectItem.Left + 2, rectItem.Top + 1, rectItem.Right - 3, rectItem.Top + 1);
                g.Graphics.DrawLine(g.FileListMarkBoderPen, rectItem.Left + 2, rectItem.Bottom - 2, rectItem.Right - 3, rectItem.Bottom - 2);
                g.Graphics.DrawLine(g.FileListMarkBoderPen, rectItem.Left + 1, rectItem.Top + 2, rectItem.Left + 1, rectItem.Bottom - 3);
                g.Graphics.DrawLine(g.FileListMarkBoderPen, rectItem.Right - 2, rectItem.Top + 2, rectItem.Right - 2, rectItem.Bottom - 3);
            }

            // 画像を描画
            DrawImage(g, rectItem, isActive);

            // ファイル名
            string fileName = m_lineFile.FileName;
            Rectangle rcFileName = new Rectangle(rectItem.Left + m_xFontOffset, rectItem.Top + m_yFontOffset, rectItem.Width - m_xFontOffset * 2, m_cyFont);
            if (m_fileListViewMode.ThumbnailName == FileListViewThumbnailName.ThumbNameSpearate) {
                g.Graphics.DrawString(fileName, g.ThumbFileListFont, m_lineFontBrush, rcFileName, g.StringFormatEllipsisCenter);
            } else if (m_fileListViewMode.ThumbnailName == FileListViewThumbnailName.ThumbNameOverray) {
                rcFileName.Offset(1, 1);
                g.Graphics.DrawString(fileName, g.ThumbFileListSmallFont, g.FileListBackBrush, rcFileName, g.StringFormatEllipsis);
                rcFileName.Offset(-1, -1);
                g.Graphics.DrawString(fileName, g.ThumbFileListSmallFont, m_lineFontBrush, rcFileName, g.StringFormatEllipsis);
            } else if (m_fileListViewMode.ThumbnailName == FileListViewThumbnailName.ThumbNameNone) {
            } else {
                Program.Abort("サムネイルのファイル名表示モードが未定義です。");
            }
        }
        
        //=========================================================================================
        // 機　能：サムネイル画像を描画する
        // 引　数：[in]g          描画に使用するグラフィックス
        // 　　　　[in]rectItem   描画する１項目全体（FileItemSize相当）の領域
        // 　　　　[in]isActive   アクティブ状態で描画するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void DrawImage(FileListGraphics g, Rectangle rectItem, bool isActive) {
            FileListViewIconSize iconSize = m_fileListViewMode.ThumbnailSize;

            // 画像を描画
            int xImage = rectItem.X + MARGIN_IMAGE_BORDER;
            int yImage = rectItem.Y + MARGIN_IMAGE_BORDER;

            FileIconManager.DrawIconDelegate drawDelegate = delegate(FileIcon icon) {
                if (icon == null) {
                    return false;
                }
                Bitmap bmp = icon.IconImage;
                int xPosIcon = xImage + (m_imageSize.Width - bmp.Width) / 2;
                int yPosIcon = yImage + (m_imageSize.Height - bmp.Height) / 2;
                Rectangle rcDest = new Rectangle(xPosIcon, yPosIcon, bmp.Width, bmp.Height);
                if (isActive) {
                    g.Graphics.DrawImage(bmp, xPosIcon, yPosIcon);
                } else {
                    g.Graphics.DrawImage(bmp, rcDest, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, g.MonochromeAttributes);
                }
                return true;
            };

            Program.Document.FileIconManager.DrawFileIcon(m_lineFile.FileIconId, m_lineFile.DefaultFileIconId, iconSize, drawDelegate);

            // 枠を描画
            g.Graphics.DrawLine(g.FileListThumbnailFramePen1, xImage, yImage, xImage + m_imageSize.Width - 1, yImage);
            g.Graphics.DrawLine(g.FileListThumbnailFramePen1, xImage, yImage, xImage, yImage + m_imageSize.Height - 1);
            g.Graphics.DrawLine(g.FileListThumbnailFramePen2, xImage, yImage + m_imageSize.Height - 1, xImage + m_imageSize.Width - 1, yImage + m_imageSize.Height - 1);
            g.Graphics.DrawLine(g.FileListThumbnailFramePen2, xImage + m_imageSize.Width - 1, yImage, xImage + m_imageSize.Width - 1, yImage + m_imageSize.Height - 1);
        }

        //=========================================================================================
        // 機　能：フォント描画用のブラシを返す
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]file      対象となっているファイル（ファイルがないときnull）
        // 　　　　[in]isActive  アクティブ状態で描画するときtrue
        // 戻り値：描画用のブラシ
        //=========================================================================================
        public Brush GetTextDrawBrush(FileListGraphics g, UIFile file, bool isActive) {
            if (isActive) {
                // 有効
                if (file == null) {
                    return g.FileListTextBrush;
                } else if (!file.Marked || Configuration.Current.FileListMarkColor == Color.Empty) {
                    if (file.Attribute.IsSymbolicLink) {
                        return g.FileListSymlinkBrush;
                    } else if (file.Attribute.IsSystem) {
                        return g.FileListSystemBrush;
                    } else if (file.Attribute.IsHidden) {
                        return g.FileListHiddenBrush;
                    } else if (file.Attribute.IsReadonly) {
                        return g.FileListReadonlyBrush;
                    } else if (file.Attribute.IsHidden) {
                        return g.FileListHiddenBrush;
                    } else if (file.Attribute.IsArchive) {
                        return g.FileListArchiveBrush;
                    } else {
                        return g.FileListTextBrush;
                    }
                } else {
                    return g.FileListMarkBrush;
                }
            } else {
                // 無効
                if (file == null) {
                    return g.FileListGrayBrush;
                } else if (!file.Marked) {
                    return g.FileListGrayBrush;
                } else {
                    return g.FileListMarkGrayBrush;
                }
            }
        }

        //=========================================================================================
        // 機　能：背景色描画用のブラシを返す
        // 引　数：[in]g     グラフィックス
        // 　　　　[in]file      対象となっているファイル（ファイルがないときnull）
        // 　　　　[in]fillBack  マークがない場合にも背景を塗りつぶすときtrue
        // 　　　　[in]isActive  アクティブ状態で描画するときtrue
        // 戻り値：描画用のブラシ（描画が不要なときはnull）
        //=========================================================================================
        private Brush GetBackDrawBrush(FileListGraphics g, UIFile file, bool fillBack, bool isActive) {
            if (isActive) {
                // 有効
                if (file == null || !file.Marked) {
                    if (fillBack) {
                        return g.FileListBackBrush;
                    } else {
                        return null;
                    }
                } else {
                    return g.FileListMarkBackBrush;
                }
            } else {
                // 無効
                if (file == null || !file.Marked) {
                    if (fillBack) {
                        return g.FileListGrayBackBrush;
                    } else {
                        return null;
                    }
                } else {
                    return g.FileListMarkGrayBackBrush;
                }
            }
        }

        //=========================================================================================
        // プロパティ：ファイル1つ分の大きさ
        //=========================================================================================
        public Size FileItemSize {
            get {
                return m_fileItemSize;
            }
        }

        //=========================================================================================
        // プロパティ：画像の大きさ
        //=========================================================================================
        public Size ImageSize {
            get {
                return m_imageSize;
            }
        }

        //=========================================================================================
        // プロパティ：フォント描画位置までのYオフセット
        //=========================================================================================
        public int YFontOffset {
            get {
                return m_yFontOffset;
            }
        }
    }
}
