using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ShellFiler.FileSystem;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：Win32APIのアイコン関係のユーティリティクラス
    //=========================================================================================
    public class Win32IconUtils {
        // アプリケーション定義
        public const int ICONMODE_NORMAL = 0;
        public const int ICONMODE_WITH_OVERRAY = 1;
        public const int ICONMODE_EXTENSION_ONLY = 2;

        // SHGetFileInfoのフラグ
        private const uint SHGFI_ICON             = 0x00000100;
        private const uint SHGFI_LARGEICON        = 0x00000000;
        private const uint SHGFI_SMALLICON        = 0x00000001;
        private const uint SHGFI_SHELLICONSIZE    = 0x00000004;
        private const uint SHGFI_ADDOVERLAYS      = 0x00000020;
        private const uint SHGFI_OVERLAYINDEX     = 0x00000040;
        private const uint SHGFI_SYSICONINDEX     = 0x00004000;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x00000010;

        // ImageList_GetIconのフラグ
        private const int ILD_TRANSPARENT= 0x1;

        // SHGetFileInfoで使用する構造体
        private struct SHFILEINFO {
            public IntPtr hIcon;
            public uint iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }; 

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("comctl32.dll", CharSet=CharSet.Auto)]
        private static extern IntPtr ImageList_GetIcon(IntPtr himl, int i,[MarshalAs(UnmanagedType.U4)] int flags);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT {
            public int left, top, right, bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT {
            int x;
            int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGELISTDRAWPARAMS {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;    // x offest from the upperleft of bitmap
            public int yBitmap;    // y offset from the upperleft of bitmap
            public int rgbBk;
            public int rgbFg;
            public int fStyle;
            public int dwRop;
            public int fState;
            public int Frame;
            public int crEffect;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGEINFO {
            public IntPtr hbmImage;
            public IntPtr hbmMask;
            public int Unused1;
            public int Unused2;
            public RECT rcImage;
        }

        [ComImportAttribute()]
        [GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        interface IImageList {
            [PreserveSig]
            int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);

            [PreserveSig]
            int ReplaceIcon(int i, IntPtr hicon, ref int pi);

            [PreserveSig]
            int SetOverlayImage(int iImage, int iOverlay);

            [PreserveSig]
            int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);

            [PreserveSig]
            int AddMasked(IntPtr hbmImage, int crMask, ref int pi);

            [PreserveSig]
            int Draw(ref IMAGELISTDRAWPARAMS pimldp);

            [PreserveSig]
            int Remove(int i);

            [PreserveSig]
            int GetIcon(int i, int flags, ref IntPtr picon);

            [PreserveSig]
            int GetImageInfo(int i, ref IMAGEINFO pImageInfo);

            [PreserveSig]
            int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);

            [PreserveSig]
            int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref IntPtr ppv);

            [PreserveSig]
            int Clone(ref Guid riid, ref IntPtr ppv);

            [PreserveSig]
            int GetImageRect(int i, ref RECT prc);

            [PreserveSig]
            int GetIconSize(ref int cx, ref int cy);

            [PreserveSig]
            int SetIconSize(int cx, int cy);

            [PreserveSig]
            int GetImageCount(ref int pi);

            [PreserveSig]
            int SetImageCount(int uNewCount);

            [PreserveSig]
            int SetBkColor(int clrBk, ref int pclr);

            [PreserveSig]
            int GetBkColor(ref int pclr);

            [PreserveSig]
            int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);

            [PreserveSig]
            int EndDrag();

            [PreserveSig]
            int DragEnter(IntPtr hwndLock, int x, int y);

            [PreserveSig]
            int DragLeave(IntPtr hwndLock);

            [PreserveSig]
            int DragMove(int x, int y);

            [PreserveSig]
            int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);

            [PreserveSig]
            int DragShowNolock(int fShow);

            [PreserveSig]
            int GetDragImage(ref POINT ppt, ref POINT pptHotspot, ref Guid riid, ref IntPtr ppv);

            [PreserveSig]
            int GetItemFlags(int i, ref int dwFlags);

            [PreserveSig]
            int GetOverlayImage(int iOverlay, ref int piIndex);
        };

        [DllImport("shell32.dll", EntryPoint = "#727")]
        private extern static int SHGetImageList(int iImageList, ref Guid riid, out IImageList ppv);

        //=========================================================================================
        // 機　能：実在するファイルに対応するアイコンを取得する
        // 引　数：[in]fileName    実在するファイル名
        // 　　　　[in]isLargeIcon 大きいアイコンを取得するときtrue
        // 　　　　[in]withOverray オーバーレイアイコン付きで取得するときtrue
        // 戻り値：アイコンのビットマップ（失敗したときはnull）
        //=========================================================================================
        public static Bitmap GetFileIconBitmap(string fileName, IconSize iconSize, int iconMode) {
            fileName = GenericFileStringUtils.CleanupWindowsFullPath(fileName);
            uint flag = SHGFI_SYSICONINDEX | SHGFI_ICON;
            if ((iconMode & ICONMODE_WITH_OVERRAY) != 0) {
                flag |= SHGFI_ADDOVERLAYS | SHGFI_OVERLAYINDEX;
            }
            if ((iconMode & ICONMODE_EXTENSION_ONLY) != 0) {
                flag |= SHGFI_USEFILEATTRIBUTES;
            }
            Bitmap bmp = new Bitmap(iconSize.CxIconSize, iconSize.CyIconSize);
            Graphics g = Graphics.FromImage(bmp);
            try {
                // シェルからアイコンを取得
                SHFILEINFO shinfo = new SHFILEINFO();
                IntPtr retVal = SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flag);
                if (shinfo.hIcon == IntPtr.Zero) {
                    return null;
                }
                try {
                    IImageList iImageList;
                    Guid imageListGuid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
                    int ret = SHGetImageList(iconSize.ApiSize, ref imageListGuid, out iImageList);
                    if (ret != 0 || iImageList == null) {
                        return null;
                    }
                    try {
                        // アイコン本体を描画
                        int iconIndex = (int)((long)(shinfo.iIcon) & 0xffffffL);
                        IntPtr hIconMain = IntPtr.Zero;
                        iImageList.GetIcon(iconIndex, (int)ILD_TRANSPARENT, ref hIconMain);
                        if (hIconMain != IntPtr.Zero) {
                            Icon iconMain = Icon.FromHandle(hIconMain);
                            g.DrawIcon(iconMain, new Rectangle(0, 0, iconMain.Width, iconMain.Height));
                            iconMain.Dispose();
                            DestroyIcon(hIconMain);
                        }

                        // オーバーレイアイコンを描画
                        int overlayIndex = (int)(((long)(shinfo.iIcon) >> 24) & 0xffL);
                        if (overlayIndex != 0) {
                            iImageList.GetOverlayImage(overlayIndex, ref iconIndex);
                            IntPtr hIconOverray = IntPtr.Zero;
                            iImageList.GetIcon(iconIndex, (int)ILD_TRANSPARENT, ref hIconOverray);
                            if (hIconOverray != IntPtr.Zero) {
                                Icon iconOverray = Icon.FromHandle(hIconOverray);
                                g.DrawIcon(iconOverray, 0, 0);
                                iconOverray.Dispose();
                                DestroyIcon(hIconOverray);
                            }
                        }
                    } finally {
                        Marshal.ReleaseComObject(iImageList);
                    }
                } finally {
                    DestroyIcon(shinfo.hIcon);
                }
            } finally {
                g.Dispose();
            }
            return bmp;
        }

        //=========================================================================================
        // 機　能：指定されたファイルに対応するアイコンを取得する
        // 引　数：[in]fileName  ファイル名
        // 　　　　[in]isLargeIcon 大きいアイコンを取得するときtrue
        // 戻り値：アイコン（失敗したときはnull）
        //=========================================================================================
        public static Icon GetFileIcon(string fileName, bool isLargeIcon) {
            uint flag = SHGFI_SYSICONINDEX | SHGFI_SHELLICONSIZE | SHGFI_ICON;
            if (isLargeIcon) {
                flag |= SHGFI_LARGEICON;
            } else {
                flag |= SHGFI_SMALLICON;
            }
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImageList = SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flag);
            if (hImageList == IntPtr.Zero) {
                return null;
            }
            Bitmap bmp = new Bitmap(IconSize.Small16.CxIconSize, IconSize.Small16.CyIconSize);
            Graphics g = Graphics.FromImage(bmp);
            try {
                if (shinfo.hIcon != IntPtr.Zero) {
                    return Icon.FromHandle(shinfo.hIcon);
                } else {
                    try {
                        int iconId = ((int)(shinfo.iIcon) & 0xffffff);
                        return Icon.FromHandle(ImageList_GetIcon(hImageList, iconId, 0));
                    } catch (OverflowException) {
                        return null;
                    }
                }
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：拡張子に対応するアイコンを取得する
        // 引　数：[in]fileName  ファイル名（存在しなくてよい）
        // 　　　　[in]isLargeIcon 大きいアイコンを取得するときtrue
        // 戻り値：アイコン（失敗したときはnull）
        //=========================================================================================
        public static Icon GetFileIconExtension(string fileName, bool isLargeIcon) {
            uint flag = SHGFI_SYSICONINDEX | SHGFI_SHELLICONSIZE | SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
            if (isLargeIcon) {
                flag |= SHGFI_LARGEICON;
            } else {
                flag |= SHGFI_SMALLICON;
            }
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImageList = SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flag);
            if (hImageList == IntPtr.Zero) {
                return null;
            }
            Bitmap bmp = new Bitmap(IconSize.Small16.CxIconSize, IconSize.Small16.CyIconSize);
            Graphics g = Graphics.FromImage(bmp);
            try {
                if (shinfo.hIcon != IntPtr.Zero) {
                    return Icon.FromHandle(shinfo.hIcon);
                } else {
                    int iconId = ((int)(shinfo.iIcon) & 0xffffff);
                    return Icon.FromHandle(ImageList_GetIcon(hImageList, iconId, 0));
                }
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // プロパティ：フォルダアイコンの取得に使用するディレクトリ
        //=========================================================================================
        public static string SampleFolderPath {
            get {
                return Environment.GetFolderPath(Environment.SpecialFolder.System);
            }
        }
    }
}
