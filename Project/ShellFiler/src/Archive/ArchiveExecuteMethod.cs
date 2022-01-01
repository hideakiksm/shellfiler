using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;

namespace ShellFiler.Api {

    //=========================================================================================
    // 列挙子：圧縮の実行方法
    //=========================================================================================
    public enum ArchiveExecuteMethod {
        Local7z,                // ローカルでの7z.dll圧縮
        RemoteShell,            // リモートでのシェルコマンドによる圧縮
    }
}
