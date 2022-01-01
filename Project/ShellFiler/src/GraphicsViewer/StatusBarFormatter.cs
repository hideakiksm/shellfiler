using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Text;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.GraphicsViewer.Filter;
using ShellFiler.UI;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：ステータスバーの表示内容のフォーマッタ
    //=========================================================================================
    class StatusBarFormatter {
        // スライドショーの現在位置
        private string m_slideShowPosition;

        // 表示中画像のフルパス
        private string m_filePath;

        // 表示中画像のファイル名
        private string m_fileName;

        // 拡大率
        private string m_zoomRatio;

        // 拡大率の背景色
        private Color m_zoomRatioBackColor;

        // サイズ
        private string m_sizeInfo;

        // マークの状態テキスト
        private string m_markState;

        // マークのアイコンインデックス（0:アイコンなし）
        private int m_markImageIndex;

        // フィルターのテキスト
        private string m_filterText;

        // フィルターのツールチップテキスト（使用しないときnull）
        private string m_filterHint;
        
        // フィルター領域の背景色
        private Color m_filterBackColor;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]graphicsViewer  グラフィックビューア
        // 　　　　[in]imageInfo       表示中の有効な画像
        // 戻り値：なし
        //=========================================================================================
        public StatusBarFormatter(GraphicsViewerPanel graphicsViewer, ImageInfo imageInfo) {
            // ファイル名
            m_slideShowPosition = CreateSlideShowPosition(graphicsViewer);
            m_filePath = imageInfo.FilePath;
            m_fileName = GenericFileStringUtils.GetFileName(m_filePath);

            // 拡大率
            int zoom = (int)(graphicsViewer.ZoomRatio * 100);
            if (graphicsViewer.AutoZoomModeSet) {
                m_zoomRatio = string.Format(Resources.GraphicsViewer_StatusZoomAuto, zoom);
                m_zoomRatioBackColor = Configuration.Current.DialogWarningBackColor;
            } else {
                m_zoomRatio = string.Format(Resources.GraphicsViewer_StatusZoom, zoom);
                m_zoomRatioBackColor = SystemColors.Control;
            }

            // サイズ
            string imageWidth = string.Format("{0}", imageInfo.Image.Image.Width);
            string imageHeight = string.Format("{0}", imageInfo.Image.Image.Height);
            string imageColors;
            int colorBit = imageInfo.ImageColorBits;
            if (colorBit == -1) {
                imageColors = "-";
            } else {
                imageColors = colorBit.ToString();
            }
            m_sizeInfo = string.Format(Resources.GraphicsViewer_StatusSize, imageWidth, imageHeight, imageColors);

            // マーク情報
            if (imageInfo.MarkState != 0) {
                m_markState = string.Format(Resources.GraphicsViewer_StatusMark, imageInfo.MarkState.ToString());
                m_markImageIndex = (int)(IconImageListID.GraphicsViewer_MarkFile1) + imageInfo.MarkState - 1;
            } else {
                m_markState = string.Format(Resources.GraphicsViewer_StatusMark, "-");
                m_markImageIndex = 0;
            }

            // フィルター
            GraphicsViewerFilterSetting filterSetting = graphicsViewer.FilterSetting;
            if (filterSetting.FilterList.Count > 0 && filterSetting.UseFilter) {
                m_filterText = string.Format(Resources.GraphicsViewer_StatusFilter, "ON");
                m_filterHint = CreateFilterHint(filterSetting.FilterList);
                m_filterBackColor = Configuration.Current.DialogWarningBackColor;
            } else {
                m_filterText = string.Format(Resources.GraphicsViewer_StatusFilter, "OFF");
                m_filterHint = null;
                m_filterBackColor = SystemColors.Control;
            }
        }

        //=========================================================================================
        // 機　能：フィルターのヒントテキストの文字列を作成する
        // 引　数：[in]filterList  フィルター設定のリスト
        // 戻り値：フィルターのヒントテキスト
        //=========================================================================================
        private string CreateFilterHint(List<GraphicsViewerFilterItem> filterList) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < filterList.Count; i++) {
                GraphicsViewerFilterItem filter = filterList[i];
                IFilterComponent filterComponent = (IFilterComponent)(filter.FilterClass.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                sb.Append(filterComponent.MetaInfo.DisplayName).Append("\n");
            }
            return sb.ToString();
        }

        //=========================================================================================
        // 機　能：スライドショーの現在位置のテキスト文字列を作成する
        // 引　数：[in]graphicsViewer  グラフィックビューア
        // 戻り値：現在位置の文字列[current/all]
        //=========================================================================================
        public static string CreateSlideShowPosition(GraphicsViewerPanel graphicsViewer) {
            if (graphicsViewer.GraphicsViewerForm.SlideShowMode) {
                int slideCurrent = graphicsViewer.SlideShowList.CurrentIndex;
                int slideAll = graphicsViewer.SlideShowList.AllImageCount;              // 最初と最後の2枚を含む
                if (slideCurrent == 0 || slideCurrent + 1 == slideAll) {
                    string label = string.Format(Resources.GraphicsViewer_StatusFileNameSlideShow, "-", slideAll - 2);
                    return label;
                } else {
                    string label = string.Format(Resources.GraphicsViewer_StatusFileNameSlideShow, slideCurrent, slideAll - 2);
                    return label;
                }
            } else {
                return "";
            }
        }

        //=========================================================================================
        // プロパティ：スライドショーの現在位置
        //=========================================================================================
        public string SlideShowPosition {
            get {
                return m_slideShowPosition;
            }
        }

        //=========================================================================================
        // プロパティ：表示中画像のフルパス
        //=========================================================================================
        public string FilePath {
            get {
                return m_filePath;
            }
        }

        //=========================================================================================
        // プロパティ：表示中画像のファイル名
        //=========================================================================================
        public string FileName {
            get {
                return m_fileName;
            }
        }

        //=========================================================================================
        // プロパティ：拡大率
        //=========================================================================================
        public string ZoomRatio {
            get {
                return m_zoomRatio;
            }
        }

        //=========================================================================================
        // プロパティ：拡大率の背景色
        //=========================================================================================
        public Color ZoomRatioBackColor {
            get {
                return m_zoomRatioBackColor;
            }
        }

        //=========================================================================================
        // プロパティ：サイズ
        //=========================================================================================
        public string SizeInfo {
            get {
                return m_sizeInfo;
            }
        }

        //=========================================================================================
        // プロパティ：マークの状態テキスト
        //=========================================================================================
        public string MarkState {
            get {
                return m_markState;
            }
        }

        //=========================================================================================
        // プロパティ：マークのアイコンインデックス（0:アイコンなし）
        //=========================================================================================
        public int MarkImageIndex {
            get {
                return m_markImageIndex;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターのテキスト
        //=========================================================================================
        public string FilterText {
            get {
                return m_filterText;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターのツールチップテキスト（使用しないときnull）
        //=========================================================================================
        public string FilterHint {
            get {
                return m_filterHint;
            }
        }

        //=========================================================================================
        // プロパティ：フィルター領域の背景色
        //=========================================================================================
        public Color FilterBackColor {
            get {
                return m_filterBackColor;
            }
        }
    }
}
