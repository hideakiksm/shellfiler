using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：コマンドのグループ定義
    //=========================================================================================
    public class CommandGroup {
        // グループの表示名
        public string GroupDisplayName;

        // グループ内の機能一覧
        public List<CommandApi> FunctionList = new List<CommandApi>();
    }
}
