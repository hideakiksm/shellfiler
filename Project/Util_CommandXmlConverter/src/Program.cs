using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using CommandXmlConverter.Properties;
using ShellFiler.Document.Serialize.CommandApi;
using ShellFiler.Util;

namespace CommandXmlConverter {
    //=========================================================================================
    // クラス：メインクラス
    //=========================================================================================
    class Program {

        //=========================================================================================
        // 機　能：メインプログラム
        // 引　数：[in]args  引数のリスト
        // 戻り値：なし
        //=========================================================================================
        static void Main(string[] _) {
            string assemblyPath = Assembly.GetEntryAssembly().Location;
            string workspacePath = Path.GetDirectoryName(assemblyPath) + @"\..\..\..\..\";

            XCommandSpec commandSpec = new XCommandSpec();
            CommandSceneConverter converter1 = new CommandSceneConverter(commandSpec.FileList, workspacePath + @"ShellFiler\src\Command\FileList\");
            converter1.ConvertFileList();
            CommandSceneConverter converter2 = new CommandSceneConverter(commandSpec.FileViewer, workspacePath + @"ShellFiler\src\Command\FileViewer\");
            converter2.ConvertFileViewer();
            CommandSceneConverter converter3= new CommandSceneConverter(commandSpec.GraphicsViewer, workspacePath + @"ShellFiler\src\Command\GraphicsViewer\");
            converter3.ConvertGraphicsViewer();

            SaveXml(workspacePath + @"bin\Debug\CommandList.dat", commandSpec);
            SaveXml(workspacePath + @"bin\Release\CommandList.dat", commandSpec);
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]fileName     ファイル名
        // 　　　　[in]commandSpec  書き込むコマンド一覧
        // 戻り値：なし
        //=========================================================================================
        private static void SaveXml(string fileName, XCommandSpec commandSpec) {
            // ディレクトリを作成
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            // ファイルに書き込む
            CommandApiSaver saver = new CommandApiSaver(fileName);
            saver.Save(commandSpec);
        }
    }
}
