using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Util;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：カレット
    //=========================================================================================
    public class Win32Caret {
        // カレット
        [DllImport("user32.dll")]
        public static extern int CreateCaret(IntPtr hwnd, IntPtr hbm, int cx, int cy);
        [DllImport("user32.dll")]
        public static extern int DestroyCaret();
        [DllImport("user32.dll")]
        public static extern int SetCaretPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern int ShowCaret(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern int HideCaret(IntPtr hwnd);

        // IME
        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct COMPOSITIONFORM {
            public uint dwStyle;
            public POINTAPI ptCurrentPos;
            public RECT rcArea;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LOGFONT {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            public string lfFaceName;
        }
        public const int CFS_DEFAULT        = 0x0000;
        public const int CFS_RECT           = 0x0001;
        public const int CFS_POINT          = 0x0002;
        public const int CFS_FORCE_POSITION = 0x0020;
        public const int CFS_CANDIDATEPOS   = 0x0040;
        public const int CFS_EXCLUDE        = 0x0080;

        public const int WM_IME_STARTCOMPOSITION = 0x010D;
        public const int WM_IME_ENDCOMPOSITION   = 0x010E;
        public const int WM_IME_COMPOSITION      = 0x010F;

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);
        [DllImport("imm32.dll")]
        public static extern int ImmSetCompositionFont(IntPtr hIMC, ref LOGFONT lplf);
        [DllImport("imm32.dll")]
        public static extern int ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll")]
        public static extern int ImmGetCompositionString(IntPtr hIMC, uint dwIndex, StringBuilder lpBuf, uint dwBufLen);

        // カレットを表示するコントロール
        private Control m_control;

        // カレットのサイズ[ピクセル]
        private Size m_caretSize;

        // カレットの位置[ピクセル]
        private Point m_caretPosition;

        // カレットを画面上に表示するときtrue
        private bool  m_caretVisible;

        // IME制御（制御しないときnull）
        private ImeControler m_imeControler = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]control    カレットを表示するコントロール
        // 　　　　[in]caretSize  カレットのサイズ[ピクセル]
        // 　　　　[in]imeControl IME制御を行うときtrue
        // 　　　　[in]fontName   IMEの入力に使用するフォント（imeContorlがfalseのときnull）
        // 戻り値：なし
        //=========================================================================================
        public Win32Caret(Control control, Size caretSize, bool imeControl, string fontName) {
            m_control = control;
            m_caretPosition = Point.Empty;
            m_caretSize = caretSize;

            if (imeControl) {
                m_imeControler = new ImeControler(control);
                m_imeControler.Initialize(caretSize.Height, fontName);
            }

            if (m_control.Focused) {
                OnGotFocus();
            }
        }

        //=========================================================================================
        // 機　能：カレットを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_control.Focused) {
                OnLostFocus();
            }
        }

        //=========================================================================================
        // 機　能：フォーカスを受け取ったときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnGotFocus() {
            CreateCaret(m_control.Handle, IntPtr.Zero, m_caretSize.Width, m_caretSize.Height);
            SetCaretPos(m_caretPosition.X, m_caretPosition.Y);
            Visible = true;
        }

        //=========================================================================================
        // 機　能：フォーカスを失ったときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnLostFocus() {
            Visible = false;
            DestroyCaret();
        }

        //=========================================================================================
        // プロパティ：カレットのサイズ[ピクセル]
        //=========================================================================================
        public Size CaretSize {
            get {
                return m_caretSize;
            }
            set {
                m_caretSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：カレットの位置[ピクセル]
        //=========================================================================================
        public Point CaretPosition {
            get {
                return m_caretPosition;
            }
            set {
                m_caretPosition = value;
                SetCaretPos(m_caretPosition.X, m_caretPosition.Y);
                if (m_imeControler != null) {
                    m_imeControler.SetInputPosition(m_caretPosition.X, m_caretPosition.Y);
                }
            }
        }

        //=========================================================================================
        // プロパティ：カレットを画面上に表示するときtrue
        //=========================================================================================
        public bool Visible {
            get {
                return m_caretVisible;
            }
            set {
                m_caretVisible = value;
                if (m_caretVisible) {
                    ShowCaret(m_control.Handle);
                    if (m_imeControler != null) {
                        m_imeControler.SetInputPosition(m_caretPosition.X, m_caretPosition.Y);
                    }
                } else {
                    HideCaret(m_control.Handle);
                }
            }
        }

        //=========================================================================================
        // クラス：IMEのコントローラ
        //=========================================================================================
        private class ImeControler {
            // カレットを表示するコントロール
            private Control m_control;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]control   カレットを表示するコントロール
            // 戻り値：なし
            //=========================================================================================
            public ImeControler(Control control) {
                m_control = control;
            }

            //=========================================================================================
            // 機　能：初期化する
            // 引　数：[in]fontHeight  埋め込みで使用するフォントの高さ
            // 　　　　[in]fontName    埋め込みで使用するフォント名
            // 戻り値：なし
            //=========================================================================================
            public void Initialize(int fontHeight, string fontName) {
                IntPtr hIMC = ImmGetContext(m_control.Handle); 
                if (hIMC == null) {
                    return;
                }
                try {
                    LOGFONT lf = new LOGFONT(); 
                    lf.lfHeight = fontHeight; 
                    lf.lfFaceName = fontName;
                    ImmSetCompositionFont(hIMC, ref lf); 
                } finally {
                    ImmReleaseContext(m_control.Handle, hIMC); 
                } 
            }

            //=========================================================================================
            // 機　能：IMEでの入力位置を指定する
            // 引　数：[in]caretX  コンストラクタで指定したウィンドウ内の入力X座標
            // 　　　　[in]caretY  コンストラクタで指定したウィンドウ内の入力Y座標
            // 戻り値：なし
            //=========================================================================================
            public void SetInputPosition(int caretX, int caretY) {
                IntPtr hIMC = ImmGetContext(m_control.Handle); 
                if (hIMC == null) {
                    return;
                }
                try {
                    POINTAPI caret = new POINTAPI();
                    caret.x = caretX;
                    caret.y = caretY;
                    COMPOSITIONFORM cf; 
                    cf.dwStyle = CFS_POINT; 
                    cf.ptCurrentPos = caret;
                    cf.rcArea.Left = 0; 
                    cf.rcArea.Top = 0; 
                    cf.rcArea.Right = 0; 
                    cf.rcArea.Bottom = 0; 
                    ImmSetCompositionWindow(hIMC, ref cf); 
                } finally {
                    ImmReleaseContext(m_control.Handle, hIMC); 
                } 
            }
        }
    }
}
