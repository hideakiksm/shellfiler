using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;

namespace ShellFiler.UI.FileList.Crawler {

    //=========================================================================================
    // 列挙子：ファイルクロールの種類
    //=========================================================================================
    public enum CrawlType {
        Icon,               // アイコンの抽出
        Thumbnail,          // サムネイルの抽出
    }
}
