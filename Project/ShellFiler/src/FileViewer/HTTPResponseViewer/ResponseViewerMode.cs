using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileTask.DataObject;

namespace ShellFiler.FileViewer.HTTPResponseViewer {

    //=========================================================================================
    // クラス：レスポンスビューアのプロトコルモード
    //=========================================================================================
    public enum ResponseViewerMode {
        HttpMode,                   // HTTPモード
        TcpMode,                    // TCPモード
    }
}
