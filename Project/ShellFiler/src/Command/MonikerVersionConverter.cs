using System;
using ShellFiler.Api;
using ShellFiler.FileViewer;

namespace ShellFiler.Command {
    
    //=========================================================================================
    // クラス：ActionCommandMonikerのバージョン間の相違を吸収するクラス
    //=========================================================================================
    public class MonikerVersionConverter {
        // はじめに関連づけが実装されたバージョン
        public const int VERSION_ASSOC_SETTING = 1000003;
        
        //=========================================================================================
        // 機　能：クラス名のバージョンによる相違を吸収する
        // 引　数：[in]name  モニカのクラス名
        // 戻り値：新しいクラス名
        //=========================================================================================
        public static string ConvertMonikerClassName(string name) {
            // Ver.1.0.0
            if (name =="ShellFiler.Command.FileList.Execute.ExecuteOrViewerCommand") {
                name = "ShellFiler.Command.FileList.Open.OpenFileAssociate1Command";
            } else if (name == "ShellFiler.Command.FileList.Execute.OpenApplicationFileCommand") {
                name = "ShellFiler.Command.FileList.Open.OpenFileAssociate2Command";

            // Ver.1.3.0
            } else if (name == "ShellFiler.Command.FileList.ChangeDir.ChdirSSHFolderCommand") {
                name = "ShellFiler.Command.FileList.SSH.ChdirSSHFolderCommand";
            }
            return name;
        }
    }
}
