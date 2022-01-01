using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;

namespace ShellFiler.UI.FileList.Crawler {

    //=========================================================================================
    // クラス：バックグラウンドでファイルをクロールする処理の基底クラス
    //=========================================================================================
    public abstract class FileClawler {

        //=========================================================================================
        // 機　能：クロールを実行する
        // 引　数：[in]request   リクエスト
        // 戻り値：なし
        //=========================================================================================
        public abstract void Execute(FileCrawlRequest request);
    }
}
