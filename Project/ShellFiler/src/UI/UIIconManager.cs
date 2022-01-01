using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        // 大きいアイコンの幅
        public const int CX_LARGE_ICON = 32;

        // 大きいアイコンの高さ
        public const int CY_LARGE_ICON = 32;

        // アイコンの幅
        public const int CX_DEFAULT_ICON = 16;

        // アイコンの高さ
        public const int CY_DEFAULT_ICON = 16;

        // リストボックス用アイコンの幅
        public const int CX_LIST_ICON = 12;

        // リストボックス用アイコンの高さ
        public const int CY_LIST_ICON = 12;

        // バックグラウンドマネージャイメージの幅
        public const int CX_BGMANAGER_ANIMATION = 40;

        // バックグラウンドマネージャイメージの高さ
        public const int CY_BGMANAGER_ANIMATION = 24;

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
        private static Bitmap s_bmpIconOperationFailed = null;
        private static Bitmap s_bmpIconSshSetting = null;
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
        // 引　数：[in]installPath  インストールディレクトリ
        // 戻り値：なし
        //=========================================================================================
        public static void InitializeIcon(string installPath) {
            s_customizePath = Path.Combine(installPath, "image");

            // 汎用アイコン
            s_iconImageList = new ImageList();
            s_iconImageList.ColorDepth = ColorDepth.Depth32Bit;
            s_iconImageList.ImageSize = new Size(CX_DEFAULT_ICON, CY_DEFAULT_ICON);
            s_iconImageList.Images.AddStrip(LoadImageListResource(Resources.ImageListIcon, "ImageListIcon"));
        }

        //=========================================================================================
        // 機　能：待機アイコンを初期化する
        // 引　数：[in]toolStrip  待機アイコンを表示するツールバー
        // 戻り値：なし
        //=========================================================================================
        public static void InitializeBgManager(ToolStrip toolStrip) {
            // 透過pngで入っている画像をツールバーの背景パターンに重ね合わせてImageListに設定
            using (Bitmap bitmapTransparent = LoadImageListResource(Resources.ImageListBGManagerAnimation, "ImageListBGManagerAnimation")) {
                // ImageListに設定する画像
                Bitmap bitmapModified = new Bitmap(bitmapTransparent.Width, bitmapTransparent.Height);
                int iconCount = bitmapTransparent.Width / CX_BGMANAGER_ANIMATION;
                Rectangle rcAnimation = new Rectangle(0, 0, CX_BGMANAGER_ANIMATION, CY_BGMANAGER_ANIMATION);

                // bitmapBack:ツールバーで描画した1個分の背景画像
                using (Bitmap bitmapBack = new Bitmap(rcAnimation.Width, rcAnimation.Height)) {
                    toolStrip.DrawToBitmap(bitmapBack, rcAnimation);
                    using (Graphics g = Graphics.FromImage(bitmapModified)) {
                        // ImageListに設定する画像に背景と透過画像を描画
                        for (int i = 0; i < iconCount; i++) {
                            g.DrawImage(bitmapBack, i * CX_BGMANAGER_ANIMATION, 0);
                        }
                        g.DrawImage(bitmapTransparent, 0, 0);
                    }
                }

                // イメージリストを設定
                s_bgManagerAnimationImageList = new ImageList();
                s_bgManagerAnimationImageList.ColorDepth = ColorDepth.Depth32Bit;
                s_bgManagerAnimationImageList.ImageSize = new Size(CX_BGMANAGER_ANIMATION, CY_BGMANAGER_ANIMATION);
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
        // 機　能：アイコンを取得する
        // 引　数：[in]resource   リソースのビットマップ
        // 　　　　[in,out]member 取得済みキャッシュを保持するクラスメンバ
        // 　　　　[in]fileName   カスタマイズ用アイコンのファイル名
        // 戻り値：使用するビットマップ
        //=========================================================================================
        public static Bitmap GetResource(Bitmap resource, ref Bitmap member, string fileName) {
            if (member != null) {
                return member;
            }
            string bmpPath = s_customizePath + fileName + ".png";
            if (File.Exists(bmpPath)) {
                try {
                    member = new Bitmap(bmpPath);
                    s_customizedIcon.Add(member);
                } catch (Exception) {
                    member = resource;
                }
            } else {
                member = resource;
            }
            return member;
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
                return GetResource(Resources.ButtonFace_Down, ref s_bmpButtonFace_Down, "ButtonFace_Down");
            }
        }
        public static Bitmap ButtonFace_Up {
            get {
                return GetResource(Resources.ButtonFace_Up, ref s_bmpButtonFace_Up, "ButtonFace_Up");
            }
        }
        public static Bitmap ButtonFace_PopupMenu {
            get {
                return GetResource(Resources.ButtonFace_PopupMenu, ref s_bmpButtonFace_PopupMenu, "ButtonFace_PopupMenu");
            }
        }
        public static Bitmap DlgIntelliHddDir {
            get {
                return GetResource(Resources.DlgIntelliHddDir, ref s_bmpDlgIntelliHddDir, "DlgIntelliHddDir");
            }
        }
        public static Bitmap DlgIntelliHddFile {
            get {
                return GetResource(Resources.DlgIntelliHddFile, ref s_bmpDlgIntelliHddFile, "DlgIntelliHddFile");
            }
        }
        public static Bitmap DlgIntelliListDir {
            get {
                return GetResource(Resources.DlgIntelliListDir, ref s_bmpDlgIntelliListDir, "DlgIntelliListDir");
            }
        }
        public static Bitmap DlgIntelliListFile {
            get {
                return GetResource(Resources.DlgIntelliListFile, ref s_bmpDlgIntelliListFile, "DlgIntelliListFile");
            }
        }
        public static Bitmap DlgIntelliShell {
            get {
                return GetResource(Resources.DlgIntelliShell, ref s_bmpDlgIntelliShell, "DlgIntelliShell");
            }
        }
        public static Bitmap FileCondition_Or1 {
            get {
                return GetResource(Resources.FileCondition_Or1, ref s_bmpFileCondition_Or1, "FileCondition_Or1");
            }
        }
        public static Bitmap FileCondition_Or2 {
            get {
                return GetResource(Resources.FileCondition_Or2, ref s_bmpFileCondition_Or2, "FileCondition_Or2");
            }
        }
        public static Bitmap GraphicsViewer_FilterArrow1 {
            get {
                return GetResource(Resources.GraphicsViewer_FilterArrow1, ref s_bmpGraphicsViewer_FilterArrow1, "GraphicsViewer_FilterArrow1");
            }
        }
        public static Bitmap GraphicsViewer_FilterArrow2 {
            get {
                return GetResource(Resources.GraphicsViewer_FilterArrow2, ref s_bmpGraphicsViewer_FilterArrow2, "GraphicsViewer_FilterArrow2");
            }
        }
        public static Bitmap GraphicsViewer_FilterSample {
            get {
                return GetResource(Resources.GraphicsViewer_FilterSample, ref s_bmpGraphicsViewer_FilterSample, "GraphicsViewer_FilterSample");
            }
        }
        public static Bitmap GraphicsViewer_FilterSampleZoom {
            get {
                return GetResource(Resources.GraphicsViewer_FilterSampleZoom, ref s_bmpGraphicsViewer_FilterSampleZoom, "GraphicsViewer_FilterSampleZoom");
            }
        }
        public static Bitmap IconOperationFailed {
            get {
                return GetResource(Resources.IconOperationFailed, ref s_bmpIconOperationFailed, "IconOperationFailed");
            }
        }
        public static Bitmap IconSshSetting {
            get {
                return GetResource(Resources.IconSshSetting, ref s_bmpIconSshSetting, "IconSshSetting");
            }
        }
        public static Bitmap SameDialogInfoArrow {
            get {
                return GetResource(Resources.SameDialogInfoArrow, ref s_bmpSameDialogInfoArrow, "SameDialogInfoArrow");
            }
        }
        public static Bitmap TabClose_Focus {
            get {
                return GetResource(Resources.TabClose_Focus, ref s_bmpTabClose_Focus, "TabClose_Focus");
            }
        }
        public static Bitmap TabClose_Normal {
            get {
                return GetResource(Resources.TabClose_Normal, ref s_bmpTabClose_Normal, "TabClose_Normal");
            }
        }
        public static Bitmap TitleLogo {
            get {
                return GetResource(Resources.TitleLogo, ref s_bmpTitleLogo, "TitleLogo");
            }
        }
        public static Bitmap TwoStrokeKeyNormal {
            get {
                return GetResource(Resources.TwoStrokeKeyNormal, ref s_bmpTwoStrokeKeyNormal, "TwoStrokeKeyNormal");
            }
        }
        public static Bitmap TwoStrokeKeyShiftCtrlAlt {
            get {
                return GetResource(Resources.TwoStrokeKeyShiftCtrlAlt, ref s_bmpTwoStrokeKeyShiftCtrlAlt, "TwoStrokeKeyShiftCtrlAlt");
            }
        }
        public static Bitmap TwoStrokeKeyShiftCtrl {
            get {
                return GetResource(Resources.TwoStrokeKeyShiftCtrl, ref s_bmpTwoStrokeKeyShiftCtrl, "TwoStrokeKeyShiftCtrl");
            }
        }
        public static Bitmap TwoStrokeKeyShiftAlt {
            get {
                return GetResource(Resources.TwoStrokeKeyShiftAlt, ref s_bmpTwoStrokeKeyShiftAlt, "TwoStrokeKeyShiftAlt");
            }
        }
        public static Bitmap TwoStrokeKeyShift {
            get {
                return GetResource(Resources.TwoStrokeKeyShift, ref s_bmpTwoStrokeKeyShift, "TwoStrokeKeyShift");
            }
        }
        public static Bitmap TwoStrokeKeyCtrlAlt {
            get {
                return GetResource(Resources.TwoStrokeKeyCtrlAlt, ref s_bmpTwoStrokeKeyCtrlAlt, "TwoStrokeKeyCtrlAlt");
            }
        }
        public static Bitmap TwoStrokeKeyCtrl {
            get {
                return GetResource(Resources.TwoStrokeKeyCtrl, ref s_bmpTwoStrokeKeyCtrl, "TwoStrokeKeyCtrl");
            }
        }
        public static Bitmap TwoStrokeKeyAlt {
            get {
                return GetResource(Resources.TwoStrokeKeyAlt, ref s_bmpTwoStrokeKeyAlt, "TwoStrokeKeyAlt");
            }
        }
    }
}
