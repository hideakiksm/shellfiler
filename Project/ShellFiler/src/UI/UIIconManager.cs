using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using ShellFiler.FileTask;

namespace ShellFiler.UI {

    //=========================================================================================
    // クラス：UI用アイコンの管理クラス
    // 　　　　Resourceクラスのbmpに対して、カスタマイズされたリソースをラップする
    //=========================================================================================
    public static class UIIconManager {
        // I1_ImageListIcon.pngの構成画像1つ分の幅
        private const int CX_IMAGE_LIST_ICON = 16;

        // I1_ImageListIcon.pngの構成画像1つ分の幅
        private const int CX_IMAGE_LIST_ICON_SMALL = 13;

        // I1_ImageListBGManagerAnimation.pngの構成画像1つ分の幅
        private const int CX_IMAGE_LIST_BG_MANAGER = 40;

        // 大きいアイコンの幅
        private static int s_cxLargeIcon = 32;

        // 大きいアイコンの高さ
        private static int s_cyLargeIcon = 32;

        // アイコンの幅
        private static int s_cxDefaultIcon = 16;

        // アイコンの高さ
        private static int s_cyDefaultIcon = 16;

        // リストボックス用アイコンの幅
        private static int s_cxListIcon = 12;

        // リストボックス用アイコンの高さ
        private static int s_cyListIcon = 12;

        // バックグラウンドマネージャイメージの幅
        private static int s_cxBgManageAnimation = 40;

        // バックグラウンドマネージャイメージの高さ
        private static int s_cyBgManageAnimation = 24;

        //=========================================================================================
        // プロパティ：大きいアイコンの幅
        //=========================================================================================
        public static int CxLargeIcon {
            get {
                return s_cxLargeIcon;
            }
        }

        //=========================================================================================
        // プロパティ：大きいアイコンの高さ
        //=========================================================================================
        public static int CyLargeIcon {
            get {
                return s_cyLargeIcon;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンの幅
        //=========================================================================================
        public static int CxDefaultIcon {
            get {
                return s_cxDefaultIcon;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンの高さ
        //=========================================================================================
        public static int CyDefaultIcon {
            get {
                return s_cyDefaultIcon;
            }
        }

        //=========================================================================================
        // プロパティ：リストボックス用アイコンの幅
        //=========================================================================================
        public static int CxListIcon {
            get {
                return s_cxListIcon;
            }
        }

        //=========================================================================================
        // プロパティ：リストボックス用アイコンの高さ
        //=========================================================================================
        public static int CyListIcon {
            get {
                return s_cyListIcon;
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドマネージャイメージの幅
        //=========================================================================================
        public static int CxBgManageAnimation {
            get {
                return s_cxBgManageAnimation;
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドマネージャイメージの高さ
        //=========================================================================================
        public static int CyBgManageAnimation {
            get {
                return s_cyBgManageAnimation;
            }
        }

        // リストボックス用アイコンの幅
        public const int CX_LIST_ICON = 12;

        // リストボックス用アイコンの高さ
        public const int CY_LIST_ICON = 12;

        // s_dpiRatioは画像の拡大縮小比で100%のとき100
        // s_dpiRatioが100、200、400はそのままの画像を保持している（s_dpiRatioFrom == s_dpiRatio）
        // それ以外の解像度は、s_dpiRatioFromを拡大縮小し、Dictionaryに保存

        // ディスプレイ解像度のデフォルトに対する比[%]
        private static int s_dpiRatio;
        // 使用するビットマッププレフィックス
        private static string s_bmpPrefix;
        // 拡大/縮小を行うときの元になる画像の、ディスプレイ解像度のデフォルトに対する比[%]
        private static int s_dpiRatioFrom;

        // カスタマイズアイコンのパス
        private static string s_customizePath;

        // カスタマイズ済みのアイコン
        private static List<Bitmap> s_customizedIcon = new List<Bitmap>();

        // アイコン用のイメージリスト
        private static ImageList s_iconImageList = null;

        // バックグラウンドマネージャアニメーション用のイメージリスト
        private static ImageList s_bgManagerAnimationImageList = null;

        // つかむカーソル
        public static Cursor s_handCursor = null;

        // 透明カーソル
        public static Cursor s_nullCursor = null;

        private static Bitmap s_bmpButtonFace_Down = null;
        private static Bitmap s_bmpButtonFace_Up = null;
        private static Bitmap s_bmpButtonFace_PopupMenu = null;
        private static Bitmap s_bmpDlgIntelliHddDir = null;
        private static Bitmap s_bmpDlgIntelliHddFile = null;
        private static Bitmap s_bmpDlgIntelliListDir = null;
        private static Bitmap s_bmpDlgIntelliListFile = null;
        private static Bitmap s_bmpDlgIntelliShell = null;
        private static Bitmap s_bmpFileCondition_Or1 = null;
        private static Bitmap s_bmpFileCondition_Or2 = null;
        private static Bitmap s_bmpGraphicsViewer_FilterArrow1 = null;
        private static Bitmap s_bmpGraphicsViewer_FilterArrow2 = null;
        private static Bitmap s_bmpGraphicsViewer_FilterSample = null;
        private static Bitmap s_bmpGraphicsViewer_FilterSampleZoom = null;
        private static Bitmap s_bmpImageListBGManagerAnimation = null;
        private static Bitmap s_bmpImageListIcon = null;
        private static Bitmap s_bmpIconOperationFailed = null;
        private static Bitmap s_bmpIconSshSetting = null;
        private static Bitmap s_bmpMainIcon48 = null;
        private static Bitmap s_bmpSameDialogInfoArrow = null;
        private static Bitmap s_bmpTabClose_Focus = null;
        private static Bitmap s_bmpTabClose_Normal = null;
        private static Bitmap s_bmpTitleLogo = null;
        private static Bitmap s_bmpTwoStrokeKeyNormal = null;
        private static Bitmap s_bmpTwoStrokeKeyShiftCtrlAlt = null;
        private static Bitmap s_bmpTwoStrokeKeyShiftCtrl = null;
        private static Bitmap s_bmpTwoStrokeKeyShiftAlt = null;
        private static Bitmap s_bmpTwoStrokeKeyShift = null;
        private static Bitmap s_bmpTwoStrokeKeyCtrlAlt = null;
        private static Bitmap s_bmpTwoStrokeKeyCtrl = null;
        private static Bitmap s_bmpTwoStrokeKeyAlt = null;

        //=========================================================================================
        // 機　能：汎用アイコンを初期化する
        // 引　数：[in]form    Graphicsの取得元となるForm
        //         [in]installPath  インストールディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public static void InitializeIcon(Form form, string installPath) {
            s_customizePath = Path.Combine(installPath, "image");

            using (Graphics g = form.CreateGraphics()) {
                s_dpiRatio = (int)(g.DpiX * 100 / 96);
                if (s_dpiRatio <= 100) {
                    s_bmpPrefix = "I100_";
                    s_dpiRatioFrom = 100;
                } else if (s_dpiRatio <= 200) {
                    s_bmpPrefix = "I200_";
                    s_dpiRatioFrom = 200;
                } else {
                    s_bmpPrefix = "I400_";
                    s_dpiRatioFrom = 400;
                }
                s_cxLargeIcon = 32 * s_dpiRatio / 100;
                s_cyLargeIcon = 32 * s_dpiRatio / 100;
                s_cxDefaultIcon = 16 * s_dpiRatio / 100;
                s_cyDefaultIcon = 16 * s_dpiRatio / 100;
                s_cxListIcon = 12 * s_dpiRatio / 100;
                s_cyListIcon = 12 * s_dpiRatio / 100;
                s_cxBgManageAnimation = 40 * s_dpiRatio / 100;
                s_cyBgManageAnimation = 24 * s_dpiRatio / 100;
            }

            // 汎用アイコン
            int iconSize = UIIconManager.ImageListIcon.Height;
            s_iconImageList = new ImageList();
            s_iconImageList.ColorDepth = ColorDepth.Depth32Bit;
            s_iconImageList.ImageSize = new Size(iconSize, iconSize);
            s_iconImageList.Images.AddStrip(s_bmpImageListIcon);
        }

        //=========================================================================================
        // 機　能：待機アイコンを初期化する
        // 引　数：[in]toolStrip  待機アイコンを表示するツールバー
        // 戻り値：なし
        //=========================================================================================
        public static void InitializeBgManager(ToolStrip toolStrip) {
            // 透過pngで入っている画像をツールバーの背景パターンに重ね合わせてImageListに設定
            using (Bitmap bitmapTransparent = LoadImageListResource(UIIconManager.ImageListBGManagerAnimation, "ImageListBGManagerAnimation")) {
                // ImageListに設定する画像
                Bitmap bitmapModified = new Bitmap(bitmapTransparent.Width, bitmapTransparent.Height);
                int iconCount = bitmapTransparent.Width / CxBgManageAnimation;
                Rectangle rcAnimation = new Rectangle(0, 0, CxBgManageAnimation, CyBgManageAnimation);

                // bitmapBack:ツールバーで描画した1個分の背景画像
                using (Bitmap bitmapBack = new Bitmap(rcAnimation.Width, rcAnimation.Height)) {
                    toolStrip.DrawToBitmap(bitmapBack, rcAnimation);
                    using (Graphics g = Graphics.FromImage(bitmapModified)) {
                        // ImageListに設定する画像に背景と透過画像を描画
                        for (int i = 0; i < iconCount; i++) {
                            g.DrawImage(bitmapBack, i * CxBgManageAnimation, 0);
                        }
                        g.DrawImage(bitmapTransparent, 0, 0);
                    }
                }

                // イメージリストを設定
                s_bgManagerAnimationImageList = new ImageList();
                s_bgManagerAnimationImageList.ColorDepth = ColorDepth.Depth32Bit;
                s_bgManagerAnimationImageList.ImageSize = new Size(CxBgManageAnimation, CyBgManageAnimation);
                s_bgManagerAnimationImageList.Images.AddStrip(bitmapModified);
            }
        }

        //=========================================================================================
        // 機　能：アイコンを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static void Dispose() {
            foreach (Bitmap bmp in s_customizedIcon) {
                bmp.Dispose();
            }
            s_customizedIcon.Clear();

            s_iconImageList.Dispose();
            s_bgManagerAnimationImageList.Dispose();
        }

        //=========================================================================================
        // 機　能：イメージリスト用のリソースを取得する
        // 引　数：[in]resource   リソースのビットマップ
        // 　　　　[in]fileName   カスタマイズ用アイコンのファイル名
        // 戻り値：使用するビットマップ
        //=========================================================================================
        public static Bitmap LoadImageListResource(Bitmap resource, string fileName) {
            Bitmap result = resource;
            string bmpPath = s_customizePath + fileName + ".png";
            if (File.Exists(bmpPath)) {
                try {
                    result = new Bitmap(bmpPath);
                    s_customizedIcon.Add(result);
                } catch (Exception) {
                    result = resource;
                }
            }
            return result;
        }

        //=========================================================================================
        // 機　能：画像をオリジナルサイズのまま取得する
        // 引　数：[in,out]member 取得済みキャッシュを保持するクラスメンバ
        // 　　　　[in]nameBody   リソース名またはカスタマイズ用アイコンのファイル名本体
        // 戻り値：使用するビットマップ
        //=========================================================================================
        public static Bitmap GetResourceOriginal(ref Bitmap member, string nameBody) {
            if (member != null) {
                return member;
            }

            // カスタマイズから取得
            string name = s_bmpPrefix + nameBody;
            string bmpPath = s_customizePath + name + ".png";
            if (File.Exists(bmpPath)) {
                try {
                    member = new Bitmap(bmpPath);
                    s_customizedIcon.Add(member);
                } catch (Exception) {
                }
            }

            // リソースから取得
            if (member == null) {
                member = (Bitmap)Resources.ResourceManager.GetObject(name, Resources.Culture);
            }
            return member;
        }

        //=========================================================================================
        // 機　能：画像をモニター解像度に合わせて取得する
        // 引　数：[in,out]member 取得済みキャッシュを保持するクラスメンバ
        // 　　　　[in]nameBody   リソース名またはカスタマイズ用アイコンのファイル名本体
        //         [in]cxStepSrc  複合画像の１つ当たりのサイズ
        // 戻り値：使用するビットマップ
        //=========================================================================================
        public static Bitmap GetResource(ref Bitmap member, string nameBody, int cxStepSrc = -1) {
            if (member != null) {
                return member;
            }

            // カスタマイズから取得
            string name = s_bmpPrefix + nameBody;
            string bmpPath = s_customizePath + name + ".png";
            if (File.Exists(bmpPath)) {
                try {
                    member = new Bitmap(bmpPath);
                    s_customizedIcon.Add(member);
                } catch (Exception) {
                }
            }

            // リソースから取得
            if (member == null) {
                member = (Bitmap)Resources.ResourceManager.GetObject(name, Resources.Culture);
            }
            if (s_dpiRatio == s_dpiRatioFrom) {
                return member;
            } else {
                // 元画像が400pxでdpiRatio=150、dpiRatioFrom=200なら400*150/200=300
                if (cxStepSrc == -1) {
                    // 単独画像の場合はそのまま縮小
                    Bitmap bitmapFrom = member;
                    int width = bitmapFrom.Width * s_dpiRatio / s_dpiRatioFrom;
                    int height = bitmapFrom.Height * s_dpiRatio / s_dpiRatioFrom;

                    member = new Bitmap(width, height);
                    using (Graphics graphics = Graphics.FromImage(member))
                    using (ImageAttributes wrapMode = new ImageAttributes()) {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(bitmapFrom, new Rectangle(0, 0, width, height), 0, 0, bitmapFrom.Width, bitmapFrom.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                } else {
                    // ImageList用の複数画像で構成されている場合は1つずつ縮小
                    Bitmap bitmapFrom = member;
                    int cxElementSrc = cxStepSrc * s_dpiRatioFrom / 100;
                    int cxElementDest = cxElementSrc * s_dpiRatio / s_dpiRatioFrom;
                    int cyElementDest = bitmapFrom.Height * s_dpiRatio / s_dpiRatioFrom;
                    int countElement = member.Width * 100 / s_dpiRatioFrom / cxStepSrc;

                    member = new Bitmap(cxElementDest * countElement, cyElementDest, PixelFormat.Format32bppArgb);
                    member.MakeTransparent();
                    using (Graphics graphics = Graphics.FromImage(member))
                    using (ImageAttributes wrapMode = new ImageAttributes()) {
                        graphics.FillRectangle(Brushes.Transparent, 0, 0, member.Width, member.Height);
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        for (int i = 0; i < countElement; i++) {
                            graphics.DrawImage(bitmapFrom, new Rectangle(cxElementDest * i, 0, cxElementDest, cyElementDest), cxElementSrc * i, 0, cxElementSrc, bitmapFrom.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }
                }
                return member;
            }
        }

        //=========================================================================================
        // プロパティ：つかむカーソル
        //=========================================================================================
        public static Cursor HandCursor {
            get {
                if (s_handCursor == null) {
                    Assembly asm = Assembly.GetExecutingAssembly();
                    s_handCursor = new Cursor(asm.GetManifestResourceStream("ShellFiler.Resources.embedded.hand.cur"));
                }
                return s_handCursor;
            }
        }

        //=========================================================================================
        // プロパティ：透明カーソル
        //=========================================================================================
        public static Cursor NullCursor {
            get {
                if (s_nullCursor == null) {
                    Assembly asm = Assembly.GetExecutingAssembly();
                    s_nullCursor = new Cursor(asm.GetManifestResourceStream("ShellFiler.Resources.embedded.null.cur"));
                }
                return s_nullCursor;
            }
        }

        //=========================================================================================
        // プロパティ：アイコン用のイメージリスト
        //=========================================================================================
        public static ImageList IconImageList {
            get {
                return s_iconImageList;
            }
        }

        //=========================================================================================
        // プロパティ：バックグラウンドマネージャ用のアニメーション
        //=========================================================================================
        public static ImageList BGManagerAnimationImageList {
            get {
                return s_bgManagerAnimationImageList;
            }
        }


        public static Bitmap ButtonFace_Down {
            get {
                return GetResource(ref s_bmpButtonFace_Down, "ButtonFace_Down");
            }
        }

        public static Bitmap ButtonFace_Up {
            get {
                return GetResource(ref s_bmpButtonFace_Up, "ButtonFace_Up");
            }
        }

        public static Bitmap ButtonFace_PopupMenu {
            get {
                return GetResource(ref s_bmpButtonFace_PopupMenu, "ButtonFace_PopupMenu");
            }
        }

        public static Bitmap DlgIntelliHddDir {
            get {
                return GetResource(ref s_bmpDlgIntelliHddDir, "DlgIntelliHddDir");
            }
        }

        public static Bitmap DlgIntelliHddFile {
            get {
                return GetResource(ref s_bmpDlgIntelliHddFile, "DlgIntelliHddFile");
            }
        }

        public static Bitmap DlgIntelliListDir {
            get {
                return GetResource(ref s_bmpDlgIntelliListDir, "DlgIntelliListDir");
            }
        }

        public static Bitmap DlgIntelliListFile {
            get {
                return GetResource(ref s_bmpDlgIntelliListFile, "DlgIntelliListFile");
            }
        }

        public static Bitmap DlgIntelliShell {
            get {
                return GetResource(ref s_bmpDlgIntelliShell, "DlgIntelliShell");
            }
        }

        public static Bitmap FileCondition_Or1 {
            get {
                return GetResource(ref s_bmpFileCondition_Or1, "FileCondition_Or1");
            }
        }

        public static Bitmap FileCondition_Or2 {
            get {
                return GetResource(ref s_bmpFileCondition_Or2, "FileCondition_Or2");
            }
        }

        public static Bitmap GraphicsViewer_FilterArrow1 {
            get {
                return GetResource(ref s_bmpGraphicsViewer_FilterArrow1, "GraphicsViewer_FilterArrow1");
            }
        }

        public static Bitmap GraphicsViewer_FilterArrow2 {
            get {
                return GetResource(ref s_bmpGraphicsViewer_FilterArrow2, "GraphicsViewer_FilterArrow2");
            }
        }

        public static Bitmap GraphicsViewer_FilterSample {
            get {
                return GetResource(ref s_bmpGraphicsViewer_FilterSample, "GraphicsViewer_FilterSample");
            }
        }

        public static Bitmap GraphicsViewer_FilterSampleZoom {
            get {
                return GetResource(ref s_bmpGraphicsViewer_FilterSampleZoom, "GraphicsViewer_FilterSampleZoom");
            }
        }

        public static Bitmap ImageListBGManagerAnimation {
            get {
                return GetResource(ref s_bmpImageListBGManagerAnimation, "ImageListBGManagerAnimation", CX_IMAGE_LIST_BG_MANAGER);
            }
        }

        public static Bitmap ImageListIcon {
            get {
                return GetResourceOriginal(ref s_bmpImageListIcon, "ImageListIcon");
            }
        }

        public static Bitmap IconOperationFailed {
            get {
                return GetResource(ref s_bmpIconOperationFailed, "IconOperationFailed");
            }
        }

        public static Bitmap IconSshSetting {
            get {
                return GetResource(ref s_bmpIconSshSetting, "IconSshSetting");
            }
        }

        public static Bitmap MainIcon48 {
            get {
                return GetResource(ref s_bmpMainIcon48, "MainIcon48");
            }
        }

        public static Bitmap SameDialogInfoArrow {
            get {
                return GetResource(ref s_bmpSameDialogInfoArrow, "SameDialogInfoArrow");
            }
        }

        public static Bitmap TabClose_Focus {
            get {
                return GetResource(ref s_bmpTabClose_Focus, "TabClose_Focus");
            }
        }

        public static Bitmap TabClose_Normal {
            get {
                return GetResource(ref s_bmpTabClose_Normal, "TabClose_Normal");
            }
        }

        public static Bitmap TitleLogo {
            get {
                return GetResource(ref s_bmpTitleLogo, "TitleLogo");
            }
        }

        public static Bitmap TwoStrokeKeyNormal {
            get {
                return GetResource(ref s_bmpTwoStrokeKeyNormal, "TwoStrokeKeyNormal");
            }
        }

        public static Bitmap TwoStrokeKeyShiftCtrlAlt {
            get {
                return GetResource(ref s_bmpTwoStrokeKeyShiftCtrlAlt, "TwoStrokeKeyShiftCtrlAlt");
            }
        }

        public static Bitmap TwoStrokeKeyShiftCtrl {
            get {
                return GetResource(ref s_bmpTwoStrokeKeyShiftCtrl, "TwoStrokeKeyShiftCtrl");
            }
        }

        public static Bitmap TwoStrokeKeyShiftAlt {
            get {
                return GetResource(ref s_bmpTwoStrokeKeyShiftAlt, "TwoStrokeKeyShiftAlt");
            }
        }

        public static Bitmap TwoStrokeKeyShift {
            get {
                return GetResource(ref s_bmpTwoStrokeKeyShift, "TwoStrokeKeyShift");
            }
        }

        public static Bitmap TwoStrokeKeyCtrlAlt {
            get {
                return GetResource(ref s_bmpTwoStrokeKeyCtrlAlt, "TwoStrokeKeyCtrlAlt");
            }
        }

        public static Bitmap TwoStrokeKeyCtrl {
            get {
                return GetResource(ref s_bmpTwoStrokeKeyCtrl, "TwoStrokeKeyCtrl");
            }
        }

        public static Bitmap TwoStrokeKeyAlt {
            get {
                return GetResource(ref s_bmpTwoStrokeKeyAlt, "TwoStrokeKeyAlt");
            }
        }
    }
}
