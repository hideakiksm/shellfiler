using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // 列挙子：IFileSystem.DeleteFileFolderへの削除条件
    //=========================================================================================
    public enum DeleteFileFolderFlag {
        FILE = 1,                   // ファイルを削除
        FOLDER = 2,                 // フォルダを削除
        RECYCLE_OR_RF = 4,          // ごみ箱で削除または-rfオプション付きで削除
        CHANGE_ATTR = 8,            // 属性を変更
    }
}
