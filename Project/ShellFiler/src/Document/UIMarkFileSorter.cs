using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.FileSystem.Windows;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：マークされたファイル一覧をマーク順にソートする
    //=========================================================================================
    public class UIMarkFileSorter : IComparer<UIFile> {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public UIMarkFileSorter() {
        }

        //=========================================================================================
        // 機　能：ソートを実行する
        // 引　数：[in]fileList  ソート対象のファイルリスト
        // 戻り値：ソート結果（マークファイルのみ）
        //=========================================================================================
        public List<UIFile> ExecSort(List<UIFile> fileList) {
            List<UIFile> markFileList = new List<UIFile>();
            if (fileList.Count == 0) {
                return markFileList;
            }

            foreach (UIFile file in fileList) {
                if (file.Marked) {
                    markFileList.Add(file);
                }
            }

            markFileList.Sort(this);
            return markFileList;
        }

        //=========================================================================================
        // 機　能：2つのファイルを比較する
        // 引　数：[in]file1  比較対象のファイル１
        // 　　　　[in]file2  比較対象のファイル２
        // 戻り値：比較結果
        //=========================================================================================
        public int Compare(UIFile file1, UIFile file2) {
            return file1.MarkOrder - file2.MarkOrder;
        }
    }
}