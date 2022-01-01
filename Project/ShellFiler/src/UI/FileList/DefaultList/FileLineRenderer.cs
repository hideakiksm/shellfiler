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

namespace ShellFiler.UI.FileList.DefaultList {

    //=========================================================================================
    // クラス：ファイル一覧の行を描画するためのクラス
    //=========================================================================================
    class FileLineRenderer {
        // アイコンの左側のマージン
        public const int MARGIN_ICON_LEFT = 2;

        // アイコンの右側のマージン
        private const int MARGIN_ICON_RIGHT = 2;

        // 項目同士のマージン
        public const int ITEM_MARGIN = 4;

        // フォント描画開始位置の調整用
        private const int CY_FONT_ADJUST = 2;

        // アイコン描画位置の調整用
        public const int CY_ICON_ADJUST = 1;

        // ファイルアイコンの管理クラス
        private FileIconManager m_fileIconManager;

        // 描画対象のビュー
        private FileListView m_view;

        // ヘッダの描画クラス
        private FilePanelHeader m_filePanelHeader;

        // 行の間隔
        private int m_cyLineHeight;

        // ヘッダの高さ
        private int m_cyHeader;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]view      描画対象のビュー
        // 　　　　[in]header    ヘッダの描画クラス
        // 戻り値：なし
        //=========================================================================================
        public FileLineRenderer(FileListView view, FilePanelHeader header) {
            m_fileIconManager = Program.Document.FileIconManager;
            m_view = view;
            m_filePanelHeader = header;

            Font font = new Font(Configuration.Current.ListViewFontName, Configuration.Current.ListViewFontSize);
            m_cyLineHeight = Math.Max(font.Height, Configuration.Current.DefaultFileListViewHeight);
            font.Dispose();

            Refresh();
        }

        //=========================================================================================
        // 機　能：再読込時に状態をリフレッシュする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Refresh() {
            m_cyHeader = m_filePanelHeader.CyHeader;
        }

        // 行の描画用：描画対象のファイル
        private UIFile m_lineFile;

        // 行の描画用：Y表示位置
        private int m_lineYPos;

        // 行の描画用：Y表示位置からのフォント描画位置のずれ
        private int m_lineYPosFont;

        // 行の描画用：フォントの背景ブラシ
        private Brush m_lineFontBrush;

        // 行の描画用：描画する項目（カラム）のインデックス
        private int m_itemIndex;

        //=========================================================================================
        // 機　能：ファイルリストの1行分をカーソルなしで描画する
        // 引　数：[in]g         描画に使用するグラフィックス
        // 　　　　[in]yPos      画面上の表示Ｙ位置
        // 　　　　[in]index     ファイル一覧中のインデックス
        // 　　　　[in]fillBack  マークがない場合にも背景を塗りつぶすときtrue
        // 戻り値：なし
        //=========================================================================================
        public void DrawLine(FileListGraphics g, int yPos, int index, bool fillBack) {
            // 拡張子の表示上の長さを計算
            UIFileList fileList = m_view.FileList;
            fileList.CalcViewWidth(g.Graphics, g.FileListFont);

            // 描画内容を決定
            FileListHeaderItem[] headerItemList = m_filePanelHeader.FileListHeaderItemList;
            m_lineYPos = yPos;
            m_lineYPosFont = Math.Max(0, (m_cyLineHeight - g.FileListFont.Height) / 2) + CY_FONT_ADJUST;

            m_lineFile = fileList.Files[index];
            m_lineFontBrush = GetTextDrawBrush(g, m_lineFile);
            m_itemIndex = 0;
            try {
                // 背景を描画
                Brush backBrush = GetBackDrawBrush(g, m_lineFile, fillBack);
                if (backBrush != null) {
                    Rectangle rectLine = new Rectangle(0, m_lineYPos, m_view.ClientRectangle.Right, m_cyLineHeight);
                    g.Graphics.FillRectangle(backBrush, rectLine);
                }

                // 枠を描画
                Rectangle rectBorder = new Rectangle(0, m_lineYPos, m_view.ClientRectangle.Right - 1, m_cyLineHeight);
                Pen markBorderPen;
                if (IsActiveDraw) {
                    markBorderPen = g.FileListMarkBoderPen;
                } else {
                    markBorderPen = g.FileListMarkGrayBoderPen;
                }
                if (fileList.Files[index].Marked) {                   // マーク
                    DrawMarkBorder(g, rectBorder, markBorderPen, true, true, true);
                } else {
                    if (fileList.Files.Count == 1) {                  // マーク解除：1件
                        DrawMarkBorder(g, rectBorder, null, false, false, false);
                    } else if (index == 0) {                          // マーク解除：一番上
                        DrawMarkBorder(g, rectBorder, markBorderPen, false, false, fileList.Files[1].Marked);
                    } else if (index == fileList.Files.Count - 1) {   // マーク解除：一番下
                        DrawMarkBorder(g, rectBorder, markBorderPen, false, fileList.Files[index - 1].Marked, false);
                    } else {                                          // マーク解除：通常
                        DrawMarkBorder(g, rectBorder, markBorderPen, false, fileList.Files[index - 1].Marked, fileList.Files[index + 1].Marked);
                    }
                }

                // アイコンを描画
                int xPos = m_filePanelHeader.GetItemXPos(headerItemList[0].ItemID) + MARGIN_ICON_LEFT;
                DrawFileIcon(g, xPos, yPos, index);

                // 項目を描画
                foreach (FileListHeaderItem item in headerItemList) {
                    switch (item.ItemID) {
                        case FileListHeaderItem.FileListHeaderItemId.FileName:
                            if (Configuration.Current.FileListSeparateExt && !m_lineFile.Attribute.IsDirectory) {
                                DrawFileNameSeparate(g);
                            } else {
                                DrawFileNameCombine(g);
                            }
                            break;
                        case FileListHeaderItem.FileListHeaderItemId.FileSize:
                            DrawFileSize(g);
                            break;
                        case FileListHeaderItem.FileListHeaderItemId.ModifiedTime:
                            DrawFileModifiedTime(g);
                            break;
                        case FileListHeaderItem.FileListHeaderItemId.Attribute:
                            DrawFileAttribute(g);
                            break;
                    }
                    m_itemIndex++;
                }
            } finally {
                m_lineFile = null;
                m_lineYPos = 0;
                m_lineYPosFont = 0;
                m_lineFontBrush = null;
                m_itemIndex = 0;
            }
        }
        
        //=========================================================================================
        // 機　能：マーク時の枠を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]rect      枠の長方形
        // 　　　　[in]borderPen 枠の描画に使用するペン
        // 　　　　[in]self      左右（自分自身）を描画するときtrue
        // 　　　　[in]top       上辺（直前の枠と共有）を描画するときtrue
        // 　　　　[in]bottom    下辺（直後の枠と共有）を描画するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void DrawMarkBorder(FileListGraphics g, Rectangle rect, Pen borderPen, bool self, bool top, bool bottom) {
            if (self) {
                g.Graphics.DrawLine(borderPen, rect.Left, rect.Top + 1, rect.Left, rect.Bottom - 1);
                g.Graphics.DrawLine(borderPen, rect.Right, rect.Top + 1, rect.Right, rect.Bottom -1);
            } else {
                g.Graphics.DrawLine(g.FileListBackPen, rect.Left, rect.Top + 1, rect.Left, rect.Bottom - 1);
                g.Graphics.DrawLine(g.FileListBackPen, rect.Right, rect.Top + 1, rect.Right, rect.Bottom - 1);
            }
            if (top) {
                g.Graphics.DrawLine(borderPen, rect.Left + 1, rect.Top, rect.Right - 1, rect.Top);
            } else {
                g.Graphics.DrawLine(g.FileListBackPen, rect.Left + 1, rect.Top, rect.Right - 1, rect.Top);
            }
            if (bottom) {
                g.Graphics.DrawLine(borderPen, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
            } else {
                g.Graphics.DrawLine(g.FileListBackPen, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
            }
        }

        //=========================================================================================
        // 機　能：ファイル名をファイル名本体と拡張子を離して表示する
        // 引　数：[in]g    描画に使用するグラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawFileNameSeparate(FileListGraphics g) {
            // 描画位置を決定
            int xPos, cx;
            GetItemPosition(FileListHeaderItem.FileListHeaderItemId.FileName, out xPos, out cx);
            int xPosName, xPosExt, cxName, cxExt;
            UIFileList fileList = m_view.FileList;
            if (fileList.CxViewExtension == 0) {
                xPosName = xPos;
                cxName = cx;
                xPosExt = xPos + cx;
                cxExt = 0;
            } else {
                xPosName = xPos;
                cxName = Math.Max(0, Math.Min(m_view.FileList.CxViewFileBody, cx - fileList.CxViewExtension));
                xPosExt = xPos + cxName;
                cxExt = fileList.CxViewExtension;
            }
            int yPos = m_lineYPos + m_lineYPosFont;

            // 描画内容を決定
            string fileName = m_lineFile.FileName;

            // 描画
            if (m_lineFile.DisplayExtensionPos == -1) {
                g.Graphics.DrawString(fileName, g.FileListFont, m_lineFontBrush, new RectangleF(xPos, yPos, cxName, g.FileListFont.Height), g.StringFormatEllipsis);
            } else {
                string dispFileBody = fileName.Substring(0, m_lineFile.DisplayExtensionPos - 1);
                string dispExt = fileName.Substring(m_lineFile.DisplayExtensionPos - 1);
                g.Graphics.DrawString(dispFileBody, g.FileListFont, m_lineFontBrush, new RectangleF(xPosName, yPos, cxName, g.FileListFont.Height), g.StringFormatEllipsis);
                g.Graphics.DrawString(dispExt, g.FileListFont, m_lineFontBrush, new RectangleF(xPosExt, yPos, cxExt, g.FileListFont.Height), g.StringFormatEllipsis);
            }
        }
        
        //=========================================================================================
        // 機　能：ファイル名をファイル名本体と拡張子をつなげて表示する
        // 引　数：[in]g    描画に使用するグラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawFileNameCombine(FileListGraphics g) {
            // 描画位置を決定
            int xPos, cx;
            GetItemPosition(FileListHeaderItem.FileListHeaderItemId.FileName, out xPos, out cx);
            int yPos = m_lineYPos + m_lineYPosFont;

            // 描画内容を決定
            string fileName = m_lineFile.FileName;

            // 描画
            g.Graphics.DrawString(fileName, g.FileListFont, m_lineFontBrush, new RectangleF(xPos, yPos, cx, g.FileListFont.Height), g.StringFormatEllipsis);
        }

        //=========================================================================================
        // 機　能：ファイルサイズを表示する
        // 引　数：[in]g    描画に使用するグラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawFileSize(FileListGraphics g) {
            // 描画位置を決定
            int xPos, cx;
            GetItemPosition(FileListHeaderItem.FileListHeaderItemId.FileSize, out xPos, out cx);
            int yPos = m_lineYPos + m_lineYPosFont;

            // 描画内容を決定
            string strSize;
            if (m_lineFile.Attribute.IsDirectory) {
                if (m_lineFile.FileSize == -1) {
                    strSize = "<DIR>";
                } else {
                    strSize = StringUtils.FileSizeToString(m_lineFile.FileSize);
                }
            } else {
                strSize = StringUtils.FileSizeToString(m_lineFile.FileSize);
            }

            // 描画
            g.Graphics.DrawString(strSize, g.FileListFont, m_lineFontBrush, new RectangleF(xPos, yPos, cx, g.FileListFont.Height), g.StringFormatRight);
        }

        //=========================================================================================
        // 機　能：ファイルの更新時刻を表示する
        // 引　数：[in]g    描画に使用するグラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawFileModifiedTime(FileListGraphics g) {
            // 描画位置を決定
            int xPos, cx;
            GetItemPosition(FileListHeaderItem.FileListHeaderItemId.ModifiedTime, out xPos, out cx);
            int yPos = m_lineYPos + m_lineYPosFont;

            // 描画内容を決定
            DateTime modifiedTime = m_lineFile.ModifiedDate; 
            string strModTime = DateTimeFormatter.DateTimeToDefaultFileList(modifiedTime);

            // 描画
            g.Graphics.DrawString(strModTime, g.FileListFont, m_lineFontBrush, new RectangleF(xPos, yPos, cx, g.FileListFont.Height), g.StringFormatEllipsis);
        }

        //=========================================================================================
        // 機　能：ファイル属性を表示する
        // 引　数：[in]g    描画に使用するグラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawFileAttribute(FileListGraphics g) {
            // 描画位置を決定
            int xPos, cx;
            GetItemPosition(FileListHeaderItem.FileListHeaderItemId.Attribute, out xPos, out cx);
            int yPos = m_lineYPos + m_lineYPosFont;

            // 描画内容を決定
            string strAttr = m_lineFile.Attribute.AttributeString;

            // 描画
            g.Graphics.DrawString(strAttr, g.FileListFont, m_lineFontBrush, new RectangleF(xPos, yPos, cx, g.FileListFont.Height), g.StringFormatEllipsis);
        }

        //=========================================================================================
        // 機　能：指定項目の表示位置を取得する
        // 引　数：[in]itemId  表示する項目
        // 　　　　[out]xPos   X位置を返す変数
        // 　　　　[out]cx     表示幅を返す変数
        // 戻り値：なし
        //=========================================================================================
        private void GetItemPosition(FileListHeaderItem.FileListHeaderItemId itemId, out int xPos, out int cx) {
            xPos = m_filePanelHeader.GetItemXPos(itemId);
            cx = m_filePanelHeader.GetItemWidth(itemId);
            if (m_itemIndex == 0) {
                xPos += IconSize.Small16.CxIconSize + MARGIN_ICON_LEFT + MARGIN_ICON_RIGHT;
                cx -= IconSize.Small16.CxIconSize + MARGIN_ICON_LEFT + MARGIN_ICON_RIGHT + ITEM_MARGIN;
            } else if (m_itemIndex == m_filePanelHeader.FileListHeaderItemList.Length - 1) {
                xPos += ITEM_MARGIN;
                cx -= ITEM_MARGIN;
            } else {
                xPos += ITEM_MARGIN;
                cx -= ITEM_MARGIN;
            }
        }

        //=========================================================================================
        // 機　能：フォント描画用のブラシを返す
        // 引　数：[in]g     グラフィックス
        // 　　　　[in]file  対象となっているファイル（ファイルがないときnull）
        // 戻り値：描画用のブラシ
        //=========================================================================================
        public Brush GetTextDrawBrush(FileListGraphics g, UIFile file) {
            if (IsActiveDraw) {
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
        // 戻り値：描画用のブラシ（描画が不要なときはnull）
        //=========================================================================================
        private Brush GetBackDrawBrush(FileListGraphics g, UIFile file, bool fillBack) {
            if (IsActiveDraw) {
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
        // 機　能：背景を描画する
        // 引　数：[in]g      描画に使用するグラフィックス
        // 　　　　[in]index  ファイル一覧中のインデックス
        // 　　　　[in]xPos   表示Ｘ位置
        // 　　　　[in]yPos   表示Ｙ位置
        // 　　　　[in]cx     表示する幅
        // 　　　　[in]cy     表示する高さ
        // 戻り値：なし
        //=========================================================================================
        public void FillBack(FileListGraphics g, int index, int xPos, int yPos, int cx, int cy) {
            UIFile uiFile = m_view.FileList.Files[index];
            Brush backBrush = GetBackDrawBrush(g, uiFile, true);
            g.Graphics.FillRectangle(backBrush, xPos, yPos, cx, cy);
        }

        //=========================================================================================
        // 機　能：アイコンを描画する
        // 引　数：[in]g         描画に使用するグラフィックス
        // 　　　　[in]yPosLine  各行の表示上のＹ位置
        // 　　　　[in]index     ファイル一覧中のインデックス
        // 戻り値：なし
        //=========================================================================================
        public void DrawFileIcon(FileListGraphics g, int xPos, int yPosLine, int index) {
            int yPos = yPosLine + CY_ICON_ADJUST;
            UIFileList fileList = m_view.FileList;
            FileIconID iconId = fileList.Files[index].FileIconId;
            FileIconID defaultIconId = fileList.Files[index].DefaultFileIconId;

            FileIconManager.DrawIconDelegate drawDelegate = delegate(FileIcon icon) {
                if (icon == null) {
                    return false;
                }
                Bitmap bmp = icon.IconImage;
                Rectangle rcDest = new Rectangle(xPos, yPos, bmp.Width, bmp.Height);
                if (IsActiveDraw) {
                    g.Graphics.DrawImage(bmp, xPos, yPos);
                } else {
                    g.Graphics.DrawImage(bmp, rcDest, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, g.MonochromeAttributes);
                }
                return true;
            };

            m_fileIconManager.DrawFileIcon(iconId, defaultIconId, FileListViewIconSize.IconSize16, drawDelegate);
        }
        
        //=========================================================================================
        // 機　能：カーソルを描画する
        // 引　数：[in]g      描画に使用するグラフィックス
        // 　　　　[in]yPos   カーソルのラインを表示するＹ位置
        // 　　　　[in]width  クライアント領域の幅
        // 戻り値：なし
        //=========================================================================================
        public static void DrawCursorLine(FileListGraphics g, int yPos, int width) {
            Pen pen;
            if (Program.MainWindow.IsFileListViewActive) {
                pen = g.FileListCursorPen;
            } else {
                pen = g.FileListCursorDisablePen;
            }
            g.Graphics.DrawLine(pen, new Point(0, yPos), new Point(width, yPos));
        }

        //=========================================================================================
        // プロパティ：アイコンを描画する領域の右端
        //=========================================================================================
        public static int IconRegionRight {
            get {
                return MARGIN_ICON_LEFT + IconSize.Small16.CxIconSize;
            }
        }
        
        //=========================================================================================
        // プロパティ：行の高さ
        //=========================================================================================
        public int LineHeight {
            get {
                return m_cyLineHeight;
            }
        }

        //=========================================================================================
        // プロパティ：アクティブ状態で描画するときtrue
        //=========================================================================================
        private bool IsActiveDraw {
            get {
                if (m_view.HasCursor && Program.MainWindow.IsFileListViewActive) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
}
