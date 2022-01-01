using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイル操作のAPI種別
    //=========================================================================================
    public class FileOperationApiType {
        public static FileOperationApiType FileCopy = new FileOperationApiType();       // ファイルのコピー
        public static FileOperationApiType FileMove = new FileOperationApiType();       // ファイルの移動
        public static FileOperationApiType FileDelete = new FileOperationApiType();     // ファイルの削除
    }
}
