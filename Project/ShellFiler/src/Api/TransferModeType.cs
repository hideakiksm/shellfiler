using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;

namespace ShellFiler.Api {

    //=========================================================================================
    // 列挙子：ファイル転送のモード
    //=========================================================================================
    public enum TransferModeType {
        CopyFile,                   // ファイルのコピー
        CopyDirectory,              // ディレクトリのコピー
        MoveFile,                   // ファイルまたはディレクトリの移動／リネーム
        HardLink,                   // ハードリンクの作成
        SymbolicLink,               // シンボリックリンクの作成
    }
}
