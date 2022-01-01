using System;
using System.Runtime.InteropServices;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：Win32のVARIANTの配列
    //=========================================================================================
    public class Win32VariantArray {
        // 配列サイズ
        private int m_arrayLength;

        // VARIANT要素の先頭ポインタ
        private IntPtr[] m_arrayPtr;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]length  配列のサイズ
        // 戻り値：なし
        //=========================================================================================
        public Win32VariantArray(int length) {
            int variantSize = OSUtils.GetVariantSize();
            m_arrayLength = length;
            m_arrayPtr = new IntPtr[m_arrayLength];
            m_arrayPtr[0] = Marshal.AllocCoTaskMem(variantSize * m_arrayLength);
            for (int i = 1; i < m_arrayLength; i++) {
                m_arrayPtr[i] = new IntPtr(m_arrayPtr[0].ToInt64() + variantSize * i);
            }
        }

        //=========================================================================================
        // 機　能：配列を破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            Marshal.FreeCoTaskMem(m_arrayPtr[0]);
        }

        //=========================================================================================
        // 機　能：配列に値を格納する
        // 引　数：[in]index  格納する位置
        // 　　　　[in]val    格納する値
        // 戻り値：なし
        //=========================================================================================
        public void SetValue(int index, object val) {
            Marshal.GetNativeVariantForObject(val, m_arrayPtr[index]);
        }

        //=========================================================================================
        // プロパティ：配列の先頭ポインタ
        //=========================================================================================
        public IntPtr VariantTop {
            get {
                return m_arrayPtr[0];
            }
        }

        //=========================================================================================
        // プロパティ：配列サイズ
        //=========================================================================================
        public int Length {
            get {
                return m_arrayLength;
            }
        }
    }
}
