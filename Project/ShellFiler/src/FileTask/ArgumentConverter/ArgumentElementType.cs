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
    // 列挙子：プログラム引数の構成要素の種類
    //=========================================================================================
    public enum ArgumentElementType {
        StringPart,             // 単純な文字列
        DollarMark,             // $$    $マーク
        TargetFile,             // $F    対象パスのカーソル上のファイルのファイル名（例　MyFile.txt）
        TargetPath,             // $P    対象パスのカーソル上のファイルのフルパス名（例　C:\data\MyFile.txt）
        TargetMark,             // $M    対象パスのマークファイル名（例　C:\data\MyFile.txt C:\data\sample.jpg）
        TargetDirectory,        // $D    対象パスのフォルダ名（例　C:）
        OppositeFile,           // $OF   反対パスのカーソル上のファイルのファイル名
        OppositePath,           // $OP   反対パスのカーソル上のファイルのフルパス名
        OppositeMark,           // $OM   反対パスのマークファイル名
        OppositeDirectory,      // $OD   反対パスのフォルダ名
        KeyInput,               // $<メッセージ> この位置でキー入力を行います。
    }
}
