using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：サムネイルの作成クラス
    //=========================================================================================
    public class ThumbNailBuilder {
        private IShellFolder m_ishellFolder = null;
        private IShellFolder m_ishellFolderBinded = null;
        private IMalloc m_imalloc = null;
        private IExtractImage m_iextractImage = null;
        private IntPtr m_ppidlFile = IntPtr.Zero; 
        private IntPtr m_hBmpThumbnail = IntPtr.Zero;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ThumbNailBuilder() {
        }

        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_ishellFolder != null) {
                Marshal.ReleaseComObject(m_ishellFolder);
                m_ishellFolder = null;
            }
            if (m_ishellFolderBinded != null) {
                Marshal.ReleaseComObject(m_ishellFolderBinded);
                m_ishellFolderBinded = null;
            }
            if (m_iextractImage != null) {
                Marshal.ReleaseComObject(m_iextractImage);
                m_iextractImage = null;
            }
            if (m_imalloc != null) {
                if (m_ppidlFile != IntPtr.Zero) {
                    m_imalloc.Free(m_ppidlFile);
                    m_ppidlFile = IntPtr.Zero;
                }
                Marshal.ReleaseComObject(m_imalloc);
                m_imalloc = null;
            }
            if (m_hBmpThumbnail != IntPtr.Zero) {
                ShellFunctions.DeleteObject(m_hBmpThumbnail);
                m_hBmpThumbnail = IntPtr.Zero;
            }
        }

        //=========================================================================================
        // 機　能：サムネイルを作成する
        // 引　数：[in]filePath   目的のファイルのフルパス
        // 　　　　[in]cxImage    作成するサムネイルの幅
        // 　　　　[in]cyImage    作成するサムネイルの高さ
        // 戻り値：作成した画像（作成できなかったときnull）
        //=========================================================================================
        public Bitmap CreateThumbnailImage(string filePath, int cxImage, int cyImage) {
            int err;

            string folder = filePath.Substring(0,filePath.LastIndexOf("\\"))+"\\";
            string fileName = filePath.Substring(filePath.LastIndexOf("\\")+1);

            err = ShellFunctions.SHGetDesktopFolder(ref m_ishellFolder);
            if (err != 0) {
                return null;
            }
            err = ShellFunctions.SHGetMalloc(ref m_imalloc);
            if (err != 0) {
                return null;
            }

            
            uint pchEaten = 0;
            uint pdwAttributes = 0;
            err =  m_ishellFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folder, ref pchEaten, out m_ppidlFile, ref pdwAttributes);
            if (err != 0) {
                return null;
            }
            if (m_ppidlFile == IntPtr.Zero) {
                return null;
            }

            Guid iidShellFolder = new Guid("000214E6-0000-0000-C000-000000000046");	
            err = m_ishellFolder.BindToObject(m_ppidlFile, IntPtr.Zero, ref iidShellFolder, out m_ishellFolderBinded);
            if (err != 0 || m_ishellFolderBinded == null) {
                return null;
            }
            m_imalloc.Free(m_ppidlFile);
            m_ppidlFile = IntPtr.Zero;

            err = m_ishellFolderBinded.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fileName, ref pchEaten, out m_ppidlFile, ref pdwAttributes);
            if (err != 0 || m_ppidlFile == IntPtr.Zero) {
                return null;
            }

            IUnknown iextractImageUnknown = null;
            pchEaten = 0;
            Guid iidExtractImage = new Guid("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1");
            err = m_ishellFolderBinded.GetUIObjectOf(IntPtr.Zero, 1, ref m_ppidlFile, ref iidExtractImage, out pchEaten, out iextractImageUnknown);
            if (iextractImageUnknown != null) {
                m_iextractImage = (IExtractImage)iextractImageUnknown;
   //             Marshal.ReleaseComObject(iextractImageUnknown);
            }
            if (err != 0) {
                return null;
            }

            StringBuilder sbFileName = new StringBuilder(filePath.Length * 2 + 1024);
            SIZE size = new SIZE();
            size.cx = cxImage;
            size.cy = cyImage;
            int pdwPriority = 0;
            int pdwFlags = (int)(EIEIFLAG.IEIFLAG_ORIGSIZE | EIEIFLAG.IEIFLAG_SCREEN);
            err = m_iextractImage.GetLocation(sbFileName, sbFileName.Capacity, ref pdwPriority, ref size, 32, ref pdwFlags);
            if (err != 0) {
                return null;
            }
            err = m_iextractImage.Extract(out m_hBmpThumbnail);
            if (err != 0 || m_hBmpThumbnail == IntPtr.Zero) {
                return null;
            }

            Bitmap bmp = Image.FromHbitmap(m_hBmpThumbnail);
            Rectangle bmBounds = new Rectangle(0,0,bmp.Width,bmp.Height);
            BitmapData bmpd = bmp.LockBits(bmBounds, ImageLockMode.ReadOnly, bmp.PixelFormat);
            Bitmap bmp2 = new Bitmap(bmpd.Width, bmpd.Height, bmpd.Stride, PixelFormat.Format32bppArgb, bmpd.Scan0);
            bmp.UnlockBits(bmpd);
            Bitmap resultBmp = new Bitmap(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(resultBmp);
            try {
                g.DrawImage(bmp2, 0f, 0f);
            } finally {
                g.Dispose();
            }
            bmp.Dispose();
            bmp2.Dispose();

            return resultBmp;
        }
    }

	[ComImport, Guid("00000000-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IUnknown {
		[PreserveSig]
		IntPtr QueryInterface(ref Guid riid, out IntPtr pVoid);
		[PreserveSig]
		IntPtr AddRef();
		[PreserveSig]
		IntPtr Release();
	}

    public enum EIEIFLAG {
      IEIFLAG_ASYNC = 1,
      IEIFLAG_CACHE = 2,
      IEIFLAG_ASPECT = 4,
      IEIFLAG_OFFLINE = 8,
      IEIFLAG_GLEAM = 16,
      IEIFLAG_SCREEN = 32,
      IEIFLAG_ORIGSIZE = 64,
      IEIFLAG_NOSTAMP = 128,
      IEIFLAG_NOBORDER = 256,
      IEIFLAG_QUALITY = 512
    }

    [ComImportAttribute()]
    [GuidAttribute( "BB2E617C-0920-11d1-9A0B-00C04FC2D6C1" )]
    [InterfaceTypeAttribute( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IExtractImage {
        [PreserveSig]
        int GetLocation(
            [Out, MarshalAs( UnmanagedType.LPWStr )] StringBuilder pszPathBuffer,
            int cch,
            ref int pdwPriority,
            ref SIZE prgSize,
            int dwRecClrDepth,
            ref int pdwFlags
            );

        [PreserveSig]
        int Extract(
            out IntPtr phBmpThumbnail
            );
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SIZE {
        public int cx;
        public int cy;
    }

    [StructLayout(LayoutKind.Explicit, Size=264)]
    public struct STRRET {
         [FieldOffset(0)]
         public UInt32 uType;    // One of the STRRET_* values

         [FieldOffset(4)]
         public IntPtr pOleStr;    // must be freed by caller of GetDisplayNameOf

         [FieldOffset(4)]
         public IntPtr pStr;        // NOT USED

         [FieldOffset(4)]
         public UInt32 uOffset;    // Offset into SHITEMID

         [FieldOffset(4)]
         public IntPtr cStr;        // Buffer to fill in (ANSI)
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214E6-0000-0000-C000-000000000046")]
    public interface IShellFolder  {
        [PreserveSig]
        Int32 ParseDisplayName( 
            IntPtr hwnd,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPWStr)] 
            String pszDisplayName,
            ref UInt32 pchEaten,
            out IntPtr ppidl,
            ref UInt32 pdwAttributes);
            
        [PreserveSig]
        Int32 EnumObjects( 
            IntPtr hwnd,
            Int32 grfFlags,
            out IntPtr ppenumIDList);

        [PreserveSig]
        Int32 BindToObject( 
            IntPtr pidl,
            IntPtr pbc,
            ref Guid riid,
            out IShellFolder ppv);
            
        [PreserveSig]
        Int32 BindToStorage( 
            IntPtr pidl,
            IntPtr pbc,
            Guid riid,
            out IntPtr ppv);
            
        [PreserveSig]
        Int32 CompareIDs( 
            Int32 lParam,
            IntPtr pidl1,
            IntPtr pidl2);

        [PreserveSig]
        Int32 CreateViewObject( 
            IntPtr hwndOwner,
            Guid riid,
            out IntPtr ppv);

        [PreserveSig]
        Int32 GetAttributesOf( 
            UInt32 cidl,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)]
            IntPtr[] apidl,
            ref UInt32 rgfInOut);
           
        [PreserveSig]
        Int32 GetUIObjectOf( 
            IntPtr hwndOwner,
            UInt32 cidl,
            ref IntPtr apidl,
            ref Guid riid,
            out UInt32 rgfReserved,
            out IUnknown ppv);

        [PreserveSig]
        Int32 GetDisplayNameOf(
            IntPtr pidl,
            UInt32 uFlags,
            out STRRET pName);
            
        [PreserveSig]
        Int32 SetNameOf( 
            IntPtr hwnd,
            IntPtr pidl,
            [MarshalAs(UnmanagedType.LPWStr)] 
            String pszName,
            UInt32 uFlags,
            out IntPtr ppidlOut);
    }

    internal class ShellFunctions {
        [DllImport("shell32", CharSet = CharSet.Auto)]
        internal extern static int SHGetMalloc(ref IMalloc ppMalloc);

        [DllImport("shell32", CharSet = CharSet.Auto)]
        internal extern static int SHGetDesktopFolder(ref IShellFolder ppshf);

        [DllImport("shell32", CharSet = CharSet.Auto)]
        internal extern static int SHGetPathFromIDList(IntPtr pidl, StringBuilder pszPath);

        [DllImport("gdi32", CharSet = CharSet.Auto)]
        internal extern static int DeleteObject(IntPtr hObject);
    }

    [InterfaceType ( ComInterfaceType.InterfaceIsIUnknown ),
    Guid ( "00000002-0000-0000-C000-000000000046" )]
    public interface IMalloc {
        [PreserveSig]
        IntPtr Alloc([In] int cb);
        
        [PreserveSig]
        IntPtr Realloc([In] IntPtr pv, [In] int cb);

        [PreserveSig]
        void Free([In] IntPtr pv);

        [PreserveSig]
        int GetSize([In] IntPtr pv);

        [PreserveSig]
        int DidAlloc(IntPtr pv);

        [PreserveSig]
        void HeapMinimize();
    }
}