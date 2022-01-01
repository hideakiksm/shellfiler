using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Document.Serialize.CommandApi;
using CommandXmlConverter.Properties;

namespace CommandXmlConverter {

    //=========================================================================================
    // クラス：ファイル一覧、ビューアなどの単位でAPI一覧を変換するクラス
    //=========================================================================================
    public class CommandSceneConverter {
        // 変換結果の書き込み先
        private readonly XCommandScene m_destCommandScene;

        // ソースファイルの取得元ルートフォルダ
        private readonly string m_rootDirectory;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]destCommandScene 変換結果の書き込み先
        // 　　　　[in]rootDirectory    ソースファイルの取得元ルートフォルダ
        // 戻り値：なし
        //=========================================================================================
        public CommandSceneConverter(XCommandScene destCommandScene, string rootDirectory) {
            m_destCommandScene = destCommandScene;
            m_rootDirectory = rootDirectory;
        }

        //=========================================================================================
        // 機　能：ファイル一覧のソースを変換する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ConvertFileList() {
            ConvertCommands("ChangeDir",        "ShellFiler.Command.FileList.ChangeDir",        Resources.GroupName_ChangeDir);         // フォルダの変更
            ConvertCommands("MoveCursor",       "ShellFiler.Command.FileList.MoveCursor",       Resources.GroupName_MoveCursor);        // カーソル移動
            ConvertCommands("FileList",         "ShellFiler.Command.FileList.FileList",         Resources.GroupName_FileList);          // ファイル一覧
            ConvertCommands("FileOperation",    "ShellFiler.Command.FileList.FileOperation",    Resources.GroupName_FileOperation);     // ファイル操作
            ConvertCommands("FileOperationEtc", "ShellFiler.Command.FileList.FileOperationEtc", Resources.GroupName_FileOperationEtc);  // ファイル操作(その他)
            ConvertCommands("Open",             "ShellFiler.Command.FileList.Open",             Resources.GroupName_Open);              // 開く
            ConvertCommands("Tools",            "ShellFiler.Command.FileList.Tools",            Resources.GroupName_Tools);              // ツール
            ConvertCommands("Setting",          "ShellFiler.Command.FileList.Setting",          Resources.GroupName_Setting);           // 設定変更
            ConvertCommands("SSH",              "ShellFiler.Command.FileList.SSH",              Resources.GroupName_SSH);               // SSH
            ConvertCommands("Window",           "ShellFiler.Command.FileList.Window",           Resources.GroupName_Window);            // ウィンドウ操作
	    }

        //=========================================================================================
        // 機　能：ファイルビューアのソースを変換する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ConvertFileViewer() {
            ConvertCommands("Cursor", "ShellFiler.Command.FileViewer.Cursor", Resources.VGroupName_V_Cursor);           // カーソル
            ConvertCommands("View",   "ShellFiler.Command.FileViewer.View",   Resources.VGroupName_V_View);             // 表示
            ConvertCommands("Edit",   "ShellFiler.Command.FileViewer.Edit",   Resources.VGroupName_V_Edit);             // 編集
	    }

        //=========================================================================================
        // 機　能：グラフィックビューアのソースを変換する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ConvertGraphicsViewer() {
            ConvertCommands("File",   "ShellFiler.Command.GraphicsViewer.File",   Resources.VGroupName_G_File);         // ファイル
            ConvertCommands("View",   "ShellFiler.Command.GraphicsViewer.View",   Resources.VGroupName_G_View);         // 表示
            ConvertCommands("Edit",   "ShellFiler.Command.GraphicsViewer.Edit",   Resources.VGroupName_G_Edit);         // 編集
            ConvertCommands("Filter", "ShellFiler.Command.GraphicsViewer.Filter", Resources.VGroupName_G_Filter);       // フィルター
	    }

        //=========================================================================================
        // 機　能：Commandクラスを解析して変換する
        // 引　数：[in]subDirName   サブディレクトリ名
        // 　　　　[in]packageName  パッケージ名
        // 　　　　[in]displayName  APIグループの表示名
        // 戻り値：なし
        //=========================================================================================
        private void ConvertCommands(string subDirName, string packageName, string displayName) {
            XCommandGroup group = new XCommandGroup();
            m_destCommandScene.CommandGroup.Add(group);
            group.GroupDisplayName = displayName;
            group.PackageName = packageName;
            string[] sourceFiles = Directory.GetFiles(m_rootDirectory + subDirName);
            List<string> sourceFileList = new List<string>();
            foreach (string fileName in sourceFiles) {
                sourceFileList.Add(fileName);
            }
            sourceFileList.Sort();
	        foreach (string fileName in sourceFileList) {
                string[] sourceLines = File.ReadAllLines(fileName);
                CommandSourceParser apiParser = new CommandSourceParser(fileName, sourceLines);
                XCommandApi commandApi = apiParser.Parse();
                if (commandApi != null) {
                    group.FunctionList.Add(commandApi);
                }
            }
        }
    }
}
