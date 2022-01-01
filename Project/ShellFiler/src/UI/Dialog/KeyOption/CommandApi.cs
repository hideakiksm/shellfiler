using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Command;

namespace ShellFiler.UI.Dialog.KeyOption {
        
    //=========================================================================================
    // クラス：コマンドの機能１つ
    //=========================================================================================
    public class CommandApi {
        // 所属するグループ
        public CommandGroup ParentGroup;

        // コマンド名
        public string CommandName;

        // コマンドのクラス名
        public string CommandClassName;

        // コマンドの説明
        public string Comment;

        // モニカ
        public ActionCommandMoniker Moniker;

        // 引数一覧
        public List<CommandArgument> ArgumentList = new List<CommandArgument>();
    }
}
