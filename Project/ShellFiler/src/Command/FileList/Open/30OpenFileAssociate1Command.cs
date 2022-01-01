using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;

namespace ShellFiler.Command.FileList.Open {

    //=========================================================================================
    // クラス：コマンドを実行する
    // カーソル位置のファイルをShellFilerで定義された関連付け設定1により開きます。
    //   書式 　 OpenFileAssociate1()
    //   引数  　なし
    //   戻り値　bool:実行に成功したときtrue、実行できなかったときfalseを返します。
    //   対応Ver 1.0.0
    //=========================================================================================
    class OpenFileAssociate1Command : FileListActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public OpenFileAssociate1Command() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            return OpenFileAssociate(this, 0);
        }

        //=========================================================================================
        // 機　能：関連づけ実行を行う
        // 引　数：[in]parent  実行の親となるコマンド
        // 　　　　[in]id      関連づけのID(0～)
        // 戻り値：実行に成功したときtrue、未定義のときfalse
        //=========================================================================================
        public static bool OpenFileAssociate(FileListActionCommand parent, int id) {
            // 関連付け情報を取得
            AssociateSetting setting = Program.Document.KeySetting.AssociateSetting;
            AssociateKeySetting keySetting = setting.AssocSettingList[id];
            if (keySetting == null) {
                return false;
            }

            // ファイル情報を取得
            int lineNo = parent.FileListComponentTarget.CursorLineNo;
            UIFile file = parent.FileListViewTarget.FileList.Files[lineNo];

            // 実行するコマンドを決定
            FileSystemID fileSystem = parent.FileListViewTarget.FileList.FileSystem.FileSystemId;
            ActionCommandMoniker moniker = null;
            if (file.Attribute.IsDirectory) {
                moniker = keySetting.GetFolderCommand(fileSystem);
            } else {
                moniker = keySetting.GetAssociateCommand(file.FileName, fileSystem);
            }
            if (moniker == null) {
                moniker = keySetting.GetDefaultCommand();
            }
            if (moniker != null) {
                FileListActionCommand command = (FileListActionCommand)(moniker.CreateActionCommand());
                command.InitializeFromParent(parent, false, moniker.Option);
                FileListCommandRuntime runtime = new FileListCommandRuntime(command);
                runtime.Execute();
            }

            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.OpenFileAssociate1Command;
            }
        }
    }
}
