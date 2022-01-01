using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem.Windows;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：ファイル一覧をソートする
    //=========================================================================================
    public class UIFileSorter : IComparer<UIFile> {
        // ソートモード
        private FileListSortMode m_sortMode;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]sortMode  ソートモード
        // 戻り値：なし
        //=========================================================================================
        public UIFileSorter(FileListSortMode sortMode) {
            m_sortMode = sortMode;
        }

        //=========================================================================================
        // 機　能：ソートを実行する
        // 引　数：[in]fileList  ソート対象のファイルリスト
        // 戻り値：なし
        //=========================================================================================
        public void ExecSort(List<UIFile> fileList) {
            if (fileList.Count == 0) {
                return;
            }
            int startIndex = 0;
            if (fileList[0].FileName == "..") {
                startIndex++;
            }
            fileList.Sort(startIndex, fileList.Count - startIndex, this);
        }

        //=========================================================================================
        // 機　能：2つのファイルを比較する
        // 引　数：[in]file1  比較対象のファイル１
        // 　　　　[in]file2  比較対象のファイル２
        // 戻り値：比較結果
        //=========================================================================================
        public int Compare(UIFile file1, UIFile file2) {
            // ディレクトリによる比較
            if (m_sortMode.TopDirectory) {
                if (file1.Attribute.IsDirectory && !file2.Attribute.IsDirectory) {
                    return -1;
                } else if (!file1.Attribute.IsDirectory && file2.Attribute.IsDirectory) {
                    return 1;
                }
            }

            // 逆順にソート
            int sortDirection1 = 1;
            int sortDirection2 = 1;
            if (m_sortMode.SortDirection1 == FileListSortMode.Direction.Reverse) {
                sortDirection1 = -1;
            }
            if (m_sortMode.SortDirection2 == FileListSortMode.Direction.Reverse) {
                sortDirection2 = -1;
            }

            // ファイル名/属性による比較
            int sortResult = 0;
            switch (m_sortMode.SortOrder1) {
                case FileListSortMode.Method.NoSort:
                    sortResult = sortDirection1 * (file1.DefaultOrder - file2.DefaultOrder);
                    break;
                case FileListSortMode.Method.FileName:
                    sortResult = sortDirection1 * CompareString(file1.FileName, file2.FileName);
                    break;
                case FileListSortMode.Method.Extension:
                    sortResult = sortDirection1 * CompareString(file1.Extension, file2.Extension);
                    if (sortResult == 0) {
                        sortResult = sortDirection2 * SortNextOrder(file1, file2, m_sortMode.SortOrder2);
                        if (sortResult == 0) {
                            sortResult = SortNextOrder(file1, file2, FileListSortMode.Method.FileName);
                        }
                    }
                    break;
                case FileListSortMode.Method.FileSize:
                    sortResult = sortDirection1 * Math.Sign(file1.FileSize - file2.FileSize);
                    if (sortResult == 0) {
                        sortResult = sortDirection2 * SortNextOrder(file1, file2, m_sortMode.SortOrder2);
                        if (sortResult == 0) {
                            sortResult = SortNextOrder(file1, file2, FileListSortMode.Method.FileName);
                        }
                    }
                    break;
                case FileListSortMode.Method.DateTime:
                    sortResult = sortDirection1 * Math.Sign(file2.ModifiedDate.Ticks - file1.ModifiedDate.Ticks);
                    if (sortResult == 0) {
                        sortResult = sortDirection2 * SortNextOrder(file1, file2, m_sortMode.SortOrder2);
                        if (sortResult == 0) {
                            sortResult = SortNextOrder(file1, file2, FileListSortMode.Method.FileName);
                        }
                    }
                    break;
                case FileListSortMode.Method.Attribute:
                    sortResult = sortDirection1 * (file1.Attribute.SortOrder - file2.Attribute.SortOrder);
                    if (sortResult == 0) {
                        sortResult = sortDirection2 * SortNextOrder(file1, file2, m_sortMode.SortOrder2);
                        if (sortResult == 0) {
                            sortResult = SortNextOrder(file1, file2, FileListSortMode.Method.FileName);
                        }
                    }
                    break;
            }

            return sortResult;
        }
 
        //=========================================================================================
        // 機　能：ソートキーに従って2つのファイルを比較する
        // 引　数：[in]file1       比較対象のファイル１
        // 　　　　[in]file2       比較対象のファイル２
        // 　　　　[in]sortMethod  ソート方法
        // 戻り値：比較結果
        //=========================================================================================
        private int SortNextOrder(IFile file1, IFile file2, FileListSortMode.Method sortMethod) {
            int sortResult = 0;
            switch (sortMethod) {
                case FileListSortMode.Method.NoSort:
                    sortResult = file1.DefaultOrder - file2.DefaultOrder;
                    break;
                case FileListSortMode.Method.FileName:
                    sortResult = CompareString(file1.FileName, file2.FileName);
                    break;
                case FileListSortMode.Method.Extension:
                    sortResult = CompareString(file1.Extension, file2.Extension);
                    break;
                case FileListSortMode.Method.FileSize:
                    sortResult = Math.Sign(file1.FileSize - file2.FileSize);
                    break;
                case FileListSortMode.Method.DateTime:
                    sortResult = Math.Sign(file2.ModifiedDate.Ticks - file1.ModifiedDate.Ticks);
                    break;
                case FileListSortMode.Method.Attribute:
                    sortResult = file1.Attribute.SortOrder - file2.Attribute.SortOrder;
                    break;
            }
            return sortResult;
        }

        //=========================================================================================
        // 機　能：文字列比較の設定に従って2つのファイルを比較する
        // 引　数：[in]str1   文字列１
        // 　　　　[in]str2   文字列２
        // 戻り値：比較結果
        //=========================================================================================
        private int CompareString(string str1, string str2) {
            if (m_sortMode.Capital) {
                str1 = str1.ToLower();
                str2 = str2.ToLower();
            }
            if (m_sortMode.IdentifyNumber) {
                return IdentifyNumberCompareString(str1, str2);
            } else {
                return string.Compare(str1, str2);
            }
        }

        //=========================================================================================
        // 機　能：文字列の数値部分を認識して2つのファイルを比較する
        // 引　数：[in]str1   文字列１
        // 　　　　[in]str2   文字列２
        // 戻り値：比較結果
        //=========================================================================================
        private static int IdentifyNumberCompareString(string str1, string str2) {
            // 文字列をブロックに分解
            List<string> strBlock1, strBlock2;
            bool blockBeginNum1, blockBeginNum2;
            ParseStringBlock(str1, out strBlock1, out blockBeginNum1);
            ParseStringBlock(str2, out strBlock2, out blockBeginNum2);
            
            // 同形式でないなら文字列比較
            if (blockBeginNum1 != blockBeginNum2) {
                return string.Compare(str1, str2);
            }

            // 数値文字混じりで比較
            int loopCount = Math.Max(strBlock1.Count, strBlock2.Count);
            for (int i = 0; i < loopCount; i++) {
                if (strBlock1.Count <= i) {             // str1が先に終わった:str1<str2
                    return -1;
                } else if (strBlock2.Count <= i) {      // str2が先に終わった:str1>str2
                    return 1;
                }
                if (blockBeginNum1 && i % 2 == 0 || (!blockBeginNum1) && i % 2 == 1) {
                    // 数値の場合桁数を比較
                    int signDigit = strBlock1[i].Length - strBlock2[i].Length;
                    if (signDigit < 0) {
                        return -1;
                    } else if (signDigit > 0) {
                        return 1;
                    }
                }
                // 文字列を比較
                int cmpString = string.Compare(strBlock1[i], strBlock2[i]);
                if (cmpString != 0) {
                    return cmpString;
                }
            }
            return 0;
        }


        //=========================================================================================
        // 機　能：文字列を数値とそれ以外のブロックに分解する
        // 引　数：[in]str       対象文字列
        // 　　　　[out]block    分解したブロックを返す変数
        // 　　　　[out]beginNum 先頭が数値から始まるときtrueを返す変数
        // 戻り値：なし
        // メ　モ：str="123abc456"のとき、block={"123","abc","456"}、beginNum=true
        //=========================================================================================
        private static void ParseStringBlock(string str, out List<string> block, out bool beginNum) {
            block = new List<string>();
            if (str.Length == 0) {
                beginNum = false;
                return;
            }

            bool prevNumber = ('0' <= str[0] && str[0] <= '9');
            beginNum = prevNumber;
            int blockStart = 0;
            for (int i = 1; i < str.Length; i++) {
                char ch = str[i];
                bool nowNumber = ('0' <= ch && ch <= '9');
                if (prevNumber != nowNumber) {
                    block.Add(str.Substring(blockStart, i - blockStart));
                    blockStart = i;
                    prevNumber = nowNumber;
                }
            }

            if (blockStart != str.Length) {
                block.Add(str.Substring(blockStart));
            }
        }

/*        public static void Test() {
            List<string> block;
            bool beginNum;

            ParseStringBlock("abc123def", out block, out beginNum);
            System.Diagnostics.Debug.Assert(beginNum == false);
            System.Diagnostics.Debug.Assert(block.Count == 3);
            System.Diagnostics.Debug.Assert(block[0] == "abc");
            System.Diagnostics.Debug.Assert(block[1] == "123");
            System.Diagnostics.Debug.Assert(block[2] == "def");

            ParseStringBlock("123def456", out block, out beginNum);
            System.Diagnostics.Debug.Assert(beginNum == true);
            System.Diagnostics.Debug.Assert(block.Count == 3);
            System.Diagnostics.Debug.Assert(block[0] == "123");
            System.Diagnostics.Debug.Assert(block[1] == "def");
            System.Diagnostics.Debug.Assert(block[2] == "456");

            ParseStringBlock("123", out block, out beginNum);
            System.Diagnostics.Debug.Assert(beginNum == true);
            System.Diagnostics.Debug.Assert(block.Count == 1);
            System.Diagnostics.Debug.Assert(block[0] == "123");

            ParseStringBlock("abc", out block, out beginNum);
            System.Diagnostics.Debug.Assert(beginNum == false);
            System.Diagnostics.Debug.Assert(block.Count == 1);
            System.Diagnostics.Debug.Assert(block[0] == "abc");

            ParseStringBlock("", out block, out beginNum);
            System.Diagnostics.Debug.Assert(block.Count == 0);

            ParseStringBlock("a123def", out block, out beginNum);
            System.Diagnostics.Debug.Assert(beginNum == false);
            System.Diagnostics.Debug.Assert(block.Count == 3);
            System.Diagnostics.Debug.Assert(block[0] == "a");
            System.Diagnostics.Debug.Assert(block[1] == "123");
            System.Diagnostics.Debug.Assert(block[2] == "def");
            
            ParseStringBlock("1def456", out block, out beginNum);
            System.Diagnostics.Debug.Assert(beginNum == true);
            System.Diagnostics.Debug.Assert(block.Count == 3);
            System.Diagnostics.Debug.Assert(block[0] == "1");
            System.Diagnostics.Debug.Assert(block[1] == "def");
            System.Diagnostics.Debug.Assert(block[2] == "456");

            ParseStringBlock("123", out block, out beginNum);
            System.Diagnostics.Debug.Assert(beginNum == true);
            System.Diagnostics.Debug.Assert(block.Count == 1);
            System.Diagnostics.Debug.Assert(block[0] == "123");

            ParseStringBlock("abc", out block, out beginNum);
            System.Diagnostics.Debug.Assert(beginNum == false);
            System.Diagnostics.Debug.Assert(block.Count == 1);
            System.Diagnostics.Debug.Assert(block[0] == "abc");

            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("", "") == 0);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abc", "123") == 1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("123", "def") == -1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abc", "abc") == 0);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abc", "def") == -1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("def", "abc") == 1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abc", "abcd") == -1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abcd", "abc") == 1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abc123d", "abc123d") == 0);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abc23d", "abc123d") == -1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abc123d", "abc23d") == 1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abc1-123", "abc1-123") == 0);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abc1-23", "abc1-123") == -1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("abc1-123", "abc1-23") == 1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("123d", "123d") == 0);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("23d", "123d") == -1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("123d", "23d") == 1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("123", "123") == 0);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("23", "123") == -1);
            System.Diagnostics.Debug.Assert(IdentifyNumberCompareString("123", "23") == 1);
        }
*/    }
}