﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイルの転送元と転送先を持つ再試行情報
    //=========================================================================================
    public interface IRetryInfo {

        //=========================================================================================
        // 機　能：ヒント文字列を取得する
        // 引　数：なし
        // 戻り値：ヒント文字列
        //=========================================================================================
        string GetHintString();

        //=========================================================================================
        // プロパティ：エラーが発生した操作
        //=========================================================================================
        FileOperationApiType OperationApiType {
            get;
        }

        //=========================================================================================
        // プロパティ：エラーが発生したファイルそのもの
        //=========================================================================================
        string ErrorFilePath {
            get;
        }

        //=========================================================================================
        // プロパティ：再試行用のマークされたファイルのパス情報
        //=========================================================================================
        SimpleFileDirectoryPath SrcMarkObjectPath {
            get;
            set;
        }
    }
}
