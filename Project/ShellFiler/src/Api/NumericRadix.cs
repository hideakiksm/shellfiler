using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：基数の選択オプション
    //=========================================================================================
    public class NumericRadix {
        public static readonly NumericRadix Radix8 = new NumericRadix("Radix8", new Converter8());                      // 8進数
        public static readonly NumericRadix Radix10 = new NumericRadix("Radix10", new Converter10());                   // 10進数
        public static readonly NumericRadix Radix16Lower = new NumericRadix("Radix16Lower", new Converter16Upper());    // 16進数小文字
        public static readonly NumericRadix Radix16Upper = new NumericRadix("Radix16Upper", new Converter16Lower());    // 16進数大文字

        // デバッグ用
        private string m_displayName;

        // データを文字列化するインターフェース
        private IDataConverter m_converter;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]displayName  デバッグ用の文字列
        // 　　　　[in]converter    データを文字列化するインターフェース
        // 戻り値：なし
        //=========================================================================================
        private NumericRadix(string displayName, IDataConverter converter) {
            m_displayName = displayName;
            m_converter = converter;
        }

        //=========================================================================================
        // 機　能：byteを文字列に変換する
        // 引　数：[in]data   変換するデータ
        // 戻り値：変換した文字列
        //=========================================================================================
        public string ConvertByte(byte data) {
            return m_converter.ConvertByte(data);
        }

        //=========================================================================================
        // 機　能：longを文字列に変換する
        // 引　数：[in]data   変換するデータ
        // 戻り値：変換した文字列
        //=========================================================================================
        public string ConvertLong(long data) {
            return m_converter.ConvertLong(data);
        }

        //=========================================================================================
        // インターフェース：データを文字列に変換するためのインターフェース
        //=========================================================================================
        private interface IDataConverter {

            //=========================================================================================
            // 機　能：byteを文字列に変換する
            // 引　数：[in]data   変換するバイトデータ
            // 戻り値：変換した文字列
            //=========================================================================================
            string ConvertByte(byte data);

            //=========================================================================================
            // 機　能：longを文字列に変換する
            // 引　数：[in]data   変換するバイトデータ
            // 戻り値：変換した文字列
            //=========================================================================================
            string ConvertLong(long data);
        }

        //=========================================================================================
        // クラス：10進数を文字列に変換するためのインターフェース
        //=========================================================================================
        private class Converter10 : IDataConverter {
            public string ConvertByte(byte data) {
                return Convert.ToString(data, 10);
            }
            public string ConvertLong(long data) {
                return Convert.ToString(data, 10);
            }
        }

        //=========================================================================================
        // クラス：8進数を文字列に変換するためのインターフェース
        //=========================================================================================
        private class Converter8 : IDataConverter {
            public string ConvertByte(byte data) {
                return Convert.ToString(data, 8);
            }
            public string ConvertLong(long data) {
                return Convert.ToString(data, 8);
            }
        }

        //=========================================================================================
        // クラス：16進数を大文字で文字列に変換するためのインターフェース
        //=========================================================================================
        private class Converter16Upper : IDataConverter {
            public string ConvertByte(byte data) {
                return data.ToString("X");
            }
            public string ConvertLong(long data) {
                return data.ToString("X");
            }
        }

        //=========================================================================================
        // クラス：16進数を小文字で文字列に変換するためのインターフェース
        //=========================================================================================
        private class Converter16Lower : IDataConverter {
            public string ConvertByte(byte data) {
                return data.ToString("x");
            }
            public string ConvertLong(long data) {
                return data.ToString("x");
            }
        }
    }
}
