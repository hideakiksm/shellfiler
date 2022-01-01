using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ShellFiler.Api;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：配列ライブラリ
    //=========================================================================================
    public class ArrayUtils {

        //=========================================================================================
        // 機　能：byte配列の指定範囲が同一であることを確認する
        // 引　数：[in]str    繰り返しを作成する文字列
        // 　　　　[in]count  繰り返し回数
        // 戻り値：作成済みの文字列
        //=========================================================================================
        public static bool CompareByteArray(byte[] array1, int start1, int length1, byte[] array2, int start2, int length2) {
            if (length1 != length2) {
                return false;
            }
            int index1 = start1;
            int index2 = start2;
            for (int i = 0; i < length1; i++) {
                if (array1[index1] != array2[index2]) {
                    return false;
                }
                index1++;
                index2++;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：byte配列に対して0x0aを区切りとして行に分割する
        // 引　数：[in]data   元のデータ
        // 戻り値：行単位に分割したデータ
        //=========================================================================================
        public static byte[][] SeparateLinuxLine(byte[] data) {
            List<byte[]> result = new List<byte[]>();
            int index = 0;
            int lineStart = 0;
            while (index < data.Length) {
                if (data[index] == 0x0a) {
                    byte[] line = new byte[index - lineStart];
                    Array.Copy(data, lineStart, line, 0, line.Length);
                    result.Add(line);
                    lineStart = index + 1;
                }
                index++;
            }
            if (index - lineStart > 0) {
                byte[] line = new byte[index - lineStart];
                Array.Copy(data, lineStart, line, 0, line.Length);
                result.Add(line);
            }
            return result.ToArray();
        }

        //=========================================================================================
        // 機　能：ListをHashSetに変換する
        // 引　数：[in]list  変換元のリスト
        // 戻り値：変換結果
        //=========================================================================================
        public static HashSet<T> ListToHash<T>(List<T> list) {
            HashSet<T> hash = new HashSet<T>();
            foreach (T item in list) {
                if (!hash.Contains(item)) {
                    hash.Add(item);
                }
            }
            return hash;
        }

        //=========================================================================================
        // 機　能：配列をListに変換する
        // 引　数：[in]array  変換元の配列
        // 戻り値：変換結果
        //=========================================================================================
        public static List<T> ArrayToList<T>(T[] array) {
            List<T> list = new List<T>(array.Length);
            for (int i = 0; i< array.Length; i++) {
                list.Add(array[i]);
            }
            return list;
        }

        //=========================================================================================
        // 機　能：Listを配列に変換する
        // 引　数：[in]list  変換元のList
        // 戻り値：変換結果
        //=========================================================================================
        public static T[] ListToArray<T>(List<T> list) {
            T[] array = new T[list.Count];
            for (int i = 0; i< list.Count; i++) {
                array[i] = list[i];
            }
            return array;
        }

        //=========================================================================================
        // 機　能：2つのリストが同じ項目を持っているかどうか確認する
        // 引　数：[in]list1  リスト1
        // 　　　　[in]list2  リスト2
        // 戻り値：内容が同じときtrue
        //=========================================================================================
        public static bool CompareListContents<T>(List<T> list1, List<T> list2) {
            HashSet<T> hash1 = ListToHash<T>(list1);
            HashSet<T> hash2 = ListToHash<T>(list2);
            if (hash1.Count != hash2.Count) {
                return false;
            }
            foreach (T item in hash1) {
                if (!hash2.Contains(item)) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：配列の指定範囲を新しいバッファを返す
        // 引　数：[in]buffer   転送元のバッファ
        // 　　　　[in]offset   有効データの開始位置
        // 　　　　[in]length   有効データの長さ
        // 戻り値：作成したバッファ
        //=========================================================================================
        public static T[] CreateCleanedBuffer<T>(T[] buffer, int offset, int length) {
            if (length == 0) {
                return new T[0];
            }
            T[] newBuffer = new T[length];
            Array.Copy(buffer, offset, newBuffer, 0, length);
            return newBuffer;
        }

        //=========================================================================================
        // 機　能：２つの配列をつなげた新しいバッファを返す
        // 引　数：[in]buffer1   転送元のバッファ１
        // 　　　　[in]offset1   バッファ１の開始位置
        // 　　　　[in]length1   有効データ１の長さ
        // 　　　　[in]buffer2   転送元のバッファ２
        // 　　　　[in]offset2   バッファ２の開始位置
        // 　　　　[in]length2   有効データ２の長さ
        // 戻り値：作成したバッファ
        //=========================================================================================
        public static T[] AppendBuffer<T>(T[] buffer1, int buffer1Offset, int buffer1Length, T[] buffer2, int buffer2Offset, int buffer2Length) {
            if (buffer1Length == 0) {
                return buffer2;
            } else if (buffer2Length == 0) {
                return buffer1;
            }
            T[] newBuffer = new T[buffer1Length + buffer2Length];
            if (buffer1Length > 0) {
                Array.Copy(buffer1, buffer1Offset, newBuffer, 0, buffer1Length);
            }
            if (buffer2Length > 0) {
                Array.Copy(buffer2, buffer2Offset, newBuffer, buffer1Length, buffer2Length);
            }
            return newBuffer;
        }

        //=========================================================================================
        // 機　能：配列のクローンを作成する
        // 引　数：[in]buffer   元の配列
        // 戻り値：作成したクローン
        //=========================================================================================
        public static T[] CloneArray<T>(T[] buffer) {
            T[] newBuffer = new T[buffer.Length];
            Array.Copy(buffer, newBuffer, buffer.Length);
            return newBuffer;
        }

        //=========================================================================================
        // 機　能：配列中からキーワードに相当する別の配列を探す
        // 引　数：[in]buffer   転送元のバッファ
        // 　　　　[in]offset   有効データの開始位置
        // 　　　　[in]length   有効データの長さ
        // 　　　　[in]keyword  検索対象のデータ
        // 戻り値：作成したクローン
        //=========================================================================================
        public static int FindIndexByte(byte[] buffer, int offset, int length, byte[] keyword) {
            if (length == 0) {
                return -1;
            }
            int lastIndex = length + offset - keyword.Length + 1;
            for (int i = offset; i < lastIndex; i++) {
                bool hit = true;
                for (int j = 0; j < keyword.Length; j++) {
                    if (buffer[i + j] != keyword[j]) {
                        hit = false;
                        break;
                    }
                }
                if (hit) {
                    return i;
                }
            }
            return -1;
        }
    }
}
