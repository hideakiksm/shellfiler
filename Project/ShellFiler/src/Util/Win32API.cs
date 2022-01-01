using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Security;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：Win32APIのラッパー
    //=========================================================================================
    public class Win32API {
        // エラーコード
        public const int S_OK = 0;
        public const int E_FAIL = -2147467259;

        // アプリケーション定義
        public const int CX_SMALL_ICON = 16;                // 小さいアイコンの幅
        public const int CY_SMALL_ICON = 16;                // 小さいアイコンの高さ

        // ウィンドウメッセージ
        public const int WM_HSCROLL = 0x114;
        public const int WM_VSCROLL = 0x115;
        public const int WM_USER = 0x400;
        public const int WM_ACTIVATE = 6;
        public const int WM_INITMENUPOPUP = 0x0117;
        public const int WM_DRAWITEM = 0x002b;
        public const int WM_MEASUREITEM = 0x002c;

        // WM_ACTIVATEのwParam
        public const int WA_INACTIVE = 0;

        // スクロールバーイベント関連
        public const int SB_LINEUP = 0;
        public const int SB_LINELEFT = 0;
        public const int SB_LINEDOWN = 1;
        public const int SB_LINERIGHT = 1;
        public const int SB_PAGEUP = 2;
        public const int SB_PAGELEFT = 2;
        public const int SB_PAGEDOWN = 3;
        public const int SB_PAGERIGHT = 3;
        public const int SB_THUMBPOSITION = 4;
        public const int SB_THUMBTRACK = 5;
        public const int SB_TOP = 6;
        public const int SB_LEFT = 6;
        public const int SB_BOTTOM = 7;
        public const int SB_RIGHT = 7;
        public const int SB_ENDSCROLL = 8;

        // スクロールバーの種類
        public const int SB_HORZ = 0;
        public const int SB_VERT = 1;
        public const int SB_CTL = 2;
        public const int SB_BOTH = 3;
         
        // GetScrollInfo/SetScrollinfo
        public const int SIF_RANGE = 0x1;
        public const int SIF_PAGE = 0x2;
        public const int SIF_POS = 0x4;
        public const int SIF_DISABLENOSCROLL = 0x8;
        public const int SIF_TRACKPOS = 0x10;
        public const int SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS;

        // WNetConnectionDialog/WNetDisconnectDialog
        public const int RESOURCETYPE_ANY = 0x00000000;
        public const int RESOURCETYPE_DISK = 0x00000001;
        public const int RESOURCETYPE_PRINT = 0x00000002;

        // エラーコード
        public const int SUCCESS = 0;
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_PATH_NOT_FOUND = 3;
        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_NOT_SAME_DEVICE = 17;
        public const int ERROR_SHARING_VIOLATION = 32;
        public const int ERROR_FILE_EXISTS = 80;
        public const int ERROR_DISK_FULL = 112;
        public const int ERROR_ALREADY_EXISTS = 183;
        public const int ERROR_REQUEST_ABORTED = 1235;

        // GetStringTypeEx
        public const int LOCALE_SYSTEM_DEFAULT = 2048;
        public const int CT_CTYPE3 = 0x00000004;
        public const int C3_NONSPACING    = 0x0001;      // nonspacing character
        public const int C3_DIACRITIC     = 0x0002;      // diacritic mark
        public const int C3_VOWELMARK     = 0x0004;      // vowel mark
        public const int C3_SYMBOL        = 0x0008;      // symbols
        public const int C3_KATAKANA      = 0x0010;      // katakana character
        public const int C3_HIRAGANA      = 0x0020;      // hiragana character
        public const int C3_HALFWIDTH     = 0x0040;      // half width character
        public const int C3_FULLWIDTH     = 0x0080;      // full width character
        public const int C3_IDEOGRAPH     = 0x0100;      // ideographic character
        public const int C3_KASHIDA       = 0x0200;      // Arabic kashida character
        public const int C3_LEXICAL       = 0x0400;      // lexical character
        public const int C3_HIGHSURROGATE = 0x0800;      // high surrogate code unit
        public const int C3_LOWSURROGATE  = 0x1000;      // low surrogate code unit
        public const int C3_ALPHA         = 0x8000;      // any linguistic char (C1_ALPHA)
        public const int C3_NOTAPPLICABLE = 0x0000;      // ctype 3 is not applicable

        // SHFileOperation
        public const UInt16 FO_MOVE = 0x0001;
        public const UInt16 FO_COPY = 0x0002;
        public const UInt16 FO_DELETE = 0x0003;
        public const UInt16 FO_RENAME = 0x0004;

        public const UInt16 FOF_MULTIDESTFILES = 0x0001;
        public const UInt16 FOF_CONFIRMMOUSE = 0x0002;
        public const UInt16 FOF_SILENT = 0x0004;  // don't create progress/report
        public const UInt16 FOF_RENAMEONCOLLISION = 0x0008;
        public const UInt16 FOF_NOCONFIRMATION = 0x0010; 
        public const UInt16 FOF_WANTMAPPINGHANDLE = 0x0020;             
        public const UInt16 FOF_ALLOWUNDO = 0x0040;
        public const UInt16 FOF_FILESONLY = 0x0080; // on *.*, do only files
        public const UInt16 FOF_SIMPLEPROGRESS = 0x0100; 
        public const UInt16 FOF_NOCONFIRMMKDIR = 0x0200; 
        public const UInt16 FOF_NOERRORUI = 0x0400; // don't put up error UI
        public const UInt16 FOF_NOCOPYSECURITYATTRIBS = 0x0800; 
        public const UInt16 FOF_NORECURSION = 0x1000;     
        public const UInt16 FOF_NO_CONNECTED_ELEMENTS = 0x2000; 
        public const UInt16 FOF_WANTNUKEWARNING = 0x4000;          
        public const UInt16 FOF_NORECURSEREPARSE = 0x8000;
        
        // TrackPopupMenu        
        public const int TPM_RETURNCMD = 0x0100;

        // SHEmptyRecycleBin
        public const int SHERB_NOCONFIRMATION = 0x00000001;
        public const int SHERB_NOPROGRESSUI = 0x00000002;
        public const int SHERB_NOSOUND = 0x00000004;

        // GetFileAttributes
        public const int FILE_ATTRIBUTE_ARCHIVE   = 0x20;
        public const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
        public const int FILE_ATTRIBUTE_HIDDEN    = 0x02;
        public const int FILE_ATTRIBUTE_NORMAL    = 0x80;
        public const int FILE_ATTRIBUTE_READONLY  = 0x01;
        public const int FILE_ATTRIBUTE_SYSTEM    = 0x04;
        public const int FILE_ATTRIBUTE_TEMPORARY = 0x100;

        // ShellExecuteEx
        public const int SEE_MASK_INVOKEIDLIST = 0x0000000c;

        [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)]
        public struct SHFILEOPSTRUCT {
            public IntPtr hwnd;
            public UInt32 wFunc;
            [MarshalAs(UnmanagedType.LPWStr)]public string pFrom;
            [MarshalAs(UnmanagedType.LPWStr)]public string pTo;
            public UInt16 fFlags;
            public Int32 fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]public string lpszProgressTitle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLINFO {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }
 
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO {
            public uint dwOemId;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SHELLEXECUTEINFO {
            public int cbSize;
            public int fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public int dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        [DllImport("user32")] 
        private static extern bool ScrollWindow(IntPtr hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip);

        [DllImport("user32")]
        private static extern int ShowScrollBar(IntPtr handle, int wBar, int bShow);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam); 

        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpace(string lpRootPathName, out UInt32 lpSectorsPerCluster, out UInt32 lpBytesPerSector, out UInt32 lpNumberOfFreeClusters, out UInt32 lpTotalNumberOfClusters);

        [DllImport("user32")]
        private static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("Mpr.dll")]
        private static extern uint WNetConnectionDialog(IntPtr hwnd, uint dwType);

        [DllImport("Mpr.dll")]
        private static extern uint WNetDisconnectDialog(IntPtr hwnd, uint dwType);

        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool GetStringTypeExW(uint locale, uint infoType, char[] sourceString, int count, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] ushort[] charTypes);

        [DllImport("user32.dll")]
        private static extern bool GetCaretPos(out Point lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO info);

        [DllImport("user32.dll")]
        private static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm); 

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation([MarshalAs(UnmanagedType.Struct)]ref SHFILEOPSTRUCT lpFileOp);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpFileName);

        [DllImport("kernel32")]
        private static extern void GetSystemInfo(ref SYSTEM_INFO ptmpsi); 

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport( "shell32.dll", CharSet = CharSet.Auto )]
        internal static extern uint SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetFileAttributes(String name);

        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        public static extern int ShellExecuteEx(SHELLEXECUTEINFO shinfo);
    
        //=========================================================================================
        // 機　能：ウィンドウ内の領域をスクロールする
        // 引　数：[in]hwnd            ウィンドウハンドル
        // 　　　　[in]nXAmount         X方向移動量
        // 　　　　[in]nYAmount         Y方向移動量
        // 　　　　[in]rectScrollRegion スクロールする範囲
        // 　　　　[in]rectClip         クリッピング範囲
        // 戻り値：なし
        //=========================================================================================
        public static void Win32ScrollWindow(IntPtr hwnd, int nXAmount, int nYAmount, Rectangle rectScrollRegion, Rectangle rectClip) {
            RECT rcScrollRegion = new RECT(rectScrollRegion.X, rectScrollRegion.Y, rectScrollRegion.Right, rectScrollRegion.Bottom);
            RECT rcClip = new RECT(rectClip.X, rectClip.Y, rectClip.Right, rectClip.Bottom);
            ScrollWindow(hwnd, nXAmount, nYAmount, ref rcScrollRegion, ref rcClip);
        }

        //=========================================================================================
        // 機　能：スクロールバーの表示状態を変える
        // 引　数：[in]hwnd    ウィンドウハンドル
        // 　　　　[in]wBar    スクロールバーの種類
        // 　　　　[in]bShow   表示するときtrue
        // 戻り値：なし
        //=========================================================================================
        public static int Win32ShowScrollBar(IntPtr hwnd, int wBar, bool bShow) {
            if (bShow) {
                return ShowScrollBar(hwnd, wBar, 1);
            } else {
                return ShowScrollBar(hwnd, wBar, 0);
            }
        }

        //=========================================================================================
        // 機　能：指定された数値の上位16bitを取得する
        // 引　数：[in]number  数値
        // 戻り値：数値の上位ワード
        //=========================================================================================
        public static int HiWord(int number) {
            if ((number & 0x80000000) == 0x80000000) {
                return (number >> 16);
            } else {
                return (number >> 16) & 0xffff;
            }
        }

        //=========================================================================================
        // 機　能：指定された数値の下位16bitを取得する
        // 引　数：[in]number  数値
        // 戻り値：数値の下位ワード
        //=========================================================================================
        public static int LoWord(int number) {
            return number & 0xffff;
        }
        
        //=========================================================================================
        // 機　能：メッセージをPostする
        // 引　数：[in]hwnd      ウィンドウハンドル
        // 　　　　[in]message   ウィンドウメッセージ
        // 　　　　[in]wParam    WPARAM
        // 　　　　[in]lParam    LPARAM
        // 戻り値：なし
        //=========================================================================================
        public static void Win32PostMessage(IntPtr hwnd, int message, int wParam, int lParam) {
            PostMessage(hwnd, (UInt32)message, (Int32)wParam, (Int32)lParam);
        }

        //=========================================================================================
        // 機　能：ディスクの空き情報を取得する
        // 引　数：[in]rootPath              ルートパス
        // 　　　　[in]sectorPerCluster      クラスタ当たりのセクタ数
        // 　　　　[in]bytesPerSector        セクタ当たりのバイト数
        // 　　　　[in]numberOfFreeClusters  空きクラスタ数
        // 　　　　[in]totalNumberOfClusters 全クラスタ数
        // 戻り値：なし
        //=========================================================================================
        public static bool Win32GetDiskFreeSpace(string rootPath, ref uint sectorPerCluster, ref uint bytesPerSector, ref uint numberOfFreeClusters, ref uint totalNumberOfClusters) {
            UInt32 sector;
            UInt32 bytes;
            UInt32 free;
            UInt32 total;
            bool success = GetDiskFreeSpace(rootPath, out sector, out bytes, out free, out total);
            if (!success) {
                return false;
            }
            sectorPerCluster = sector;
            bytesPerSector = bytes;
            numberOfFreeClusters = free;
            totalNumberOfClusters = total;
            return true;
        }

        //=========================================================================================
        // 機　能：非同期でキー入力を取得する
        // 引　数：[in]vKey  調べるキー
        // 戻り値：キーが押されているときtrue
        //=========================================================================================
        public static bool Win32GetAsyncKeyState(Keys vKey) {
            if (GetAsyncKeyState(vKey) < 0) {
                return true;
            } else {
                return false;
            }
        }

        //=========================================================================================
        // 機　能：ネットワークへ接続ダイアログを表示する
        // 引　数：[in]hwnd  親となるウィンドウのハンドル
        // 　　　　[in]type  種別
        // 戻り値：なし
        //=========================================================================================
        public static void Win32WNetConnectionDialog(IntPtr hwnd, int type) {
            WNetConnectionDialog(hwnd, (uint)type);
        }

        //=========================================================================================
        // 機　能：ネットワーク接続を切断ダイアログを表示する
        // 引　数：[in]hwnd  親となるウィンドウのハンドル
        // 　　　　[in]type  種別
        // 戻り値：なし
        //=========================================================================================
        public static void Win32WNetDisconnectDialog(IntPtr hwnd, int type) {
            WNetDisconnectDialog(hwnd, (uint)type);
        }

        //=========================================================================================
        // 機　能：文字列の構成文字を調べる
        // 引　数：[in]str    調べる文字列
        // 　　　　[in]result 文字列の種類（C3_XXXX値）
        // 戻り値：なし
        //=========================================================================================
        public static void Win32GetStringType(char[] str, out ushort[] result) {
            result = new ushort[str.Length];
            bool success = GetStringTypeExW(LOCALE_SYSTEM_DEFAULT, CT_CTYPE3, str, str.Length, result);
            if (!success) {
                throw new Exception("GetStringTypeEx failed");
            }
        }

        //=========================================================================================
        // 機　能：カレットの位置を返す
        // 引　数：[out]point  カレットの位置（クライアント座標）
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public static bool Win32GetCaretPos(out Point point) {
            return GetCaretPos(out point);
        }

        //=========================================================================================
        // 機　能：スクロールバーの状態を返す
        // 引　数：[in]hwnd   ウィンドウのハンドル
        // 　　　　[in]fnBar  対象のスクロールバー
        // 　　　　[in,out]info スクロールバーの情報（cbSizeは自動設定、fMaskは初期化、結果を戻す）
        // 戻り値：取得に成功したときtrue
        //=========================================================================================
        public static bool Win32GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO info) {
            info.cbSize = (uint)Marshal.SizeOf(info);
            return (GetScrollInfo(hwnd, fnBar, ref info) != 0);
        }

        //=========================================================================================
        // 機　能：ショートカットメニューを表示する
        // 引　数：[in]hmenu    ショートカットメニューのハンドル
        // 　　　　[in]fuFlags  オプション
        // 　　　　[in]x        水平位置
        // 　　　　[in]y        垂直位置
        // 　　　　[in]hwnd     所有側ウィンドウのハンドル
        // 　　　　[in]lptpm    無視される
        // 戻り値：メニューの戻り値
        //=========================================================================================
        public static int Win32TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm) {
            return TrackPopupMenuEx(hmenu, fuFlags, x, y, hwnd, lptpm);
        }

        //=========================================================================================
        // 機　能：ファイルまたはディレクトリをゴミ箱に移動する
        // 引　数：[in]hwnd     所有側ウィンドウのハンドル
        // 　　　　[in]path     ファイルまたはディレクトリのパス名
        // 戻り値：メニューの戻り値
        //=========================================================================================
        public static bool DeleteRecycle(IntPtr hwnd, string path) {
		    SHFILEOPSTRUCT sfo;
		    sfo.hwnd					= hwnd;
		    sfo.wFunc					= FO_DELETE;
		    sfo.pFrom					= path + "\0";
		    sfo.pTo						= null;
		    sfo.fFlags					= FOF_ALLOWUNDO | FOF_NOCONFIRMATION | FOF_SILENT | FOF_NOERRORUI;
		    sfo.fAnyOperationsAborted	= 0;
		    sfo.hNameMappings			= IntPtr.Zero;
		    sfo.lpszProgressTitle		= "";

		    int status = SHFileOperation(ref sfo);
            return (status == 0);
        }

        //=========================================================================================
        // 機　能：DLLを読み込む
        // 引　数：[in]dllPath  DLLのパス名
        // 戻り値：DLLのハンドル（0:失敗）
        //=========================================================================================
        public static IntPtr Win32LoadLibrary(string dllPath) {
            return LoadLibrary(dllPath);
        }

        //=========================================================================================
        // 機　能：DLLを解放する
        // 引　数：[in]hModule  DLLのハンドル
        // 戻り値：成功したときtrue
        //=========================================================================================
        public static bool Win32FreeLibrary(IntPtr hModule) {
            return FreeLibrary(hModule);
        }

        //=========================================================================================
        // 機　能：DateTimeをVARIANTのFILETIMEに変換する
        // 引　数：[in]time   設定する時刻
        // 　　　　[in]value  作成するVARIANT
        // 戻り値：なし
        //=========================================================================================
        public static void GetTimeProperty(DateTime time, IntPtr value) {
            Marshal.GetNativeVariantForObject(time.ToFileTime(), value);
            Marshal.WriteInt16(value, (short)VarEnum.VT_FILETIME);
        }

        //=========================================================================================
        // 機　能：ごみ箱を空にする
        // 引　数：[in]hWnd  UI表示の親とするウィンドウハンドル
        // 戻り値：なし
        //=========================================================================================
        public static bool Win32SHEmptyRecycleBin(IntPtr hWnd) {
            uint result = SHEmptyRecycleBin(hWnd, "", SHERB_NOCONFIRMATION);
            return (result == S_OK);
        }
 
        //=========================================================================================
        // 機　能：システム情報を返す
        // 引　数：なし
        // 戻り値：システム情報
        //=========================================================================================
        public static SYSTEM_INFO Win32GetSystemInfo() {
            SYSTEM_INFO sysInfo = new SYSTEM_INFO();
            GetSystemInfo(ref sysInfo);
            return sysInfo;
        }

        //=========================================================================================
        // 機　能：ファイルの属性を取得する
        // 引　数：なし
        // 戻り値：ファイルの属性
        //=========================================================================================
        public static int Win32GetFileAttributes(string path) {
            return GetFileAttributes(path);
        }

        //=========================================================================================
        // 機　能：ショートカットを作成する
        // 引　数：[in]srcFilePath   転送元ファイルパス
        // 　　　　[in]destFilePath  転送先ファイルパス（*.lnkファイル）
        // 戻り値：正しく作成できたときtrue
        //=========================================================================================
        public static bool CreateShortcut(string srcFilePath, string destFilePath) {
            try {
                Win32ShellLink shellLink = new Win32ShellLink();
                try {
                    shellLink.Description = "";
                    shellLink.TargetPath = srcFilePath;
                    shellLink.ShowCommand = SW.NORMAL;
                    if (!Directory.Exists(srcFilePath)) {
                        shellLink.WorkingDirectory = Path.GetDirectoryName(srcFilePath);
                    }
                    shellLink.Save(destFilePath);
                } finally {
                    shellLink.Dispose();
                }
            } catch (Exception) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ショートカットを解決する
        // 引　数：[in]lnkFliePath   ショートカットファイルのパス（*.lnkファイル）
        // 戻り値：ショートカットの参照先のパス（エラーのときnull）
        //=========================================================================================
        public static string ResolveShortcut(string lnkFliePath) {
            try {
                Win32ShellLink shellLink = new Win32ShellLink();
                try {
                    shellLink.Load(lnkFliePath);
                    return shellLink.TargetPath;
                } finally {
                    shellLink.Dispose();
                }
            } catch (Exception) {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：プロパティを表示する
        // 引　数：[in]hWnd   親となるウィンドウのハンドル
        // 　　　　[in]path   表示するパス
        // 戻り値：表示に成功したときtrue
        //=========================================================================================
        public static bool ShowProperty(IntPtr hWnd, string path) {
            try {
                SHELLEXECUTEINFO shinfo = new SHELLEXECUTEINFO();
                shinfo.cbSize = Marshal.SizeOf(typeof(SHELLEXECUTEINFO));
                shinfo.fMask = SEE_MASK_INVOKEIDLIST;
                shinfo.lpVerb = "Properties";
                shinfo.hwnd = hWnd;
                shinfo.lpParameters = null;
                shinfo.lpDirectory = null;
                shinfo.lpFile = path;
                ShellExecuteEx(shinfo);
                return true;
            } catch (Exception) {
                return false;
            }
        }
    }
}
