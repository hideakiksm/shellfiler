using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.Virtual;

namespace ShellFiler.FileTask.ArgumentConverter {

    //=========================================================================================
    // 列挙子：プログラム引数の構成要素の修飾子
    //=========================================================================================
    public enum ArgumentElementDecorator {
        None,               // 修飾なし
        Extension,          // :e    拡張子（例：txt）
        RemoveExtension,    // :r    拡張子をのぞいた部分（例：LOG）
        DriveName,          // :d    ドライブ名（例：C）ネットワークの場合、空文字列となります。
        PathName,           // :h    パス名（例：C:Data）
        FileBody,           // :t    ファイル名本体（例：LOG.txt）
        NotQuote,           // :n    クォーティングがある場合にはずす（例：C:\DAT\LOG.txt）
        Quote,              // :q    クォーティングがない場合に付ける（例：C:\DAT\LOG.txt）
        ToUpper,            // :au   大文字化（例：C:\DATA\LOG.TXT）
        ToLower,            // :al   小文字化（例：c:\data\log.txt）
        ToCapital,          // :ac   先頭大文字化（例：C:\Data\Log.Sys）
    }
}
