using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.UI.Dialog.KeyOption {
        
    //=========================================================================================
    // クラス：用途別のコマンド一覧
    //=========================================================================================
    public class CommandScene {
        // コマンドのグループ
        public List<CommandGroup> CommandGroup = new List<CommandGroup>();

        // 完全修飾クラス名からAPIのマップ
        public Dictionary<string, CommandApi> ClassNameToApi = new Dictionary<string, CommandApi>();

        // コマンドの利用シーン
        public CommandUsingSceneType CommandSceneType;
    }
}
