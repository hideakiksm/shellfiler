using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Document.Serialize.CommandApi {

    //=========================================================================================
    // クラス：コマンドのグループ定義
    //=========================================================================================
    public class XCommandGroup {
        // グループの表示名
        public string GroupDisplayName;

        // パッケージ名
        public string PackageName;

        // グループ内の機能一覧
        public List<XCommandApi> FunctionList = new List<XCommandApi>();
    }
}
