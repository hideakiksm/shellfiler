using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：同名ファイルの扱い方を表現するためのデータ構造
    //         ダイアログとのやりとり、バックグラウンドタスクで処理方法をまとめるために使用する
    //=========================================================================================
    public class SameFileOperation {
        // 同名ファイルのモード
        private SameFileTransferMode m_sameFileMode = SameFileTransferMode.RenameNew;

        // 別名に変更するときの名前
        private String m_newName = "";

        // 自動的に更新するときのモード
        private SameFileAutoUpdateMode m_autoUpdateMode = SameFileAutoUpdateMode.AddUnderBarNumber;

        // すべてのファイルに適用するときtrue
        private bool m_allApply = false;

        // 転送先のファイルシステム
        private FileSystemID m_destFileSystemId;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]option           オプション
        // 　　　　[in]destFileSystem   転送先ファイルシステムのID
        // 戻り値：なし
        //=========================================================================================
        private SameFileOperation(SameFileOption option, FileSystemID destFileSystem) {
            m_sameFileMode = option.SameFileMode;
            if (FileSystemID.IsWindows(destFileSystem)) {
                m_autoUpdateMode = option.AutoUpdateModeWindows;
            } else if (FileSystemID.IsSSH(destFileSystem)) {
                m_autoUpdateMode = option.AutoUpdateModeSSH;
            } else {
                FileSystemID.NotSupportError(destFileSystem);
                m_autoUpdateMode = option.AutoUpdateModeWindows;
            }
            m_destFileSystemId = destFileSystem;
        }

        //=========================================================================================
        // 機　能：コンフィグに基づいて同名ファイル転送操作を返す
        // 引　数：[in]destFileSystem   転送先ファイルシステムのID
        // 戻り値：同名ファイル転送操作
        //=========================================================================================
        public static SameFileOperation CreateWithDefaultConfig(FileSystemID destFileSystem) {
            SameFileOption option;
            if (Configuration.Current.SameFileOptionDefault == null) {
                option = (SameFileOption)(Program.Document.UserGeneralSetting.SameFileOption.Clone());
            } else {
                option = (SameFileOption)(Configuration.Current.SameFileOptionDefault.Clone());
            }
            SameFileOperation operation = new SameFileOperation(option, destFileSystem);
            return operation;
        }

        //=========================================================================================
        // プロパティ：同名ファイルのモード
        //=========================================================================================
        public SameFileTransferMode SameFileMode {
            get {
                return m_sameFileMode;
            }
            set {
                m_sameFileMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：別名に変更するときの名前
        //=========================================================================================
        public String NewName {
            get {
                return m_newName;
            }
            set {
                m_newName = value;
            }
        }

        //=========================================================================================
        // プロパティ：自動的に更新するときのモード
        //=========================================================================================
        public SameFileAutoUpdateMode AutoUpdateMode {
            get {
                return m_autoUpdateMode;
            }
            set {
                m_autoUpdateMode = value;
            }
        }

        //=========================================================================================
        // プロパティ：すべてのファイルに適用するときtrue
        //=========================================================================================
        public bool AllApply {
            get {
                return m_allApply;
            }
            set {
                m_allApply = value;
            }
        }

        //=========================================================================================
        // プロパティ：転送先のファイルシステム
        //=========================================================================================
        public FileSystemID DestFileSystemId {
            get {
                return m_destFileSystemId;
            }
            set {
                m_destFileSystemId = value;
            }
        }

        //=========================================================================================
        // 列挙子：同名ファイルのモード
        //=========================================================================================
        public enum SameFileTransferMode {
            // 強制的に上書き
            ForceOverwrite,
            // 自分が新しければ上書き
            OverwriteIfNewer,
            // 名前を変更して転送
            RenameNew,
            // 転送しない
            NotOverwrite,
            // ファイル名を自動的に変更して転送
            AutoRename,
            // 状況判断でファイル名を自動的に変更して転送
            FullAutoTransfer,
        }

        //=========================================================================================
        // 列挙子：自動的に更新するときのモード
        //=========================================================================================
        public enum SameFileAutoUpdateMode {
            // ファイル名主部に_2 _3を付加
            AddUnderBarNumber,
            // ファイル名主部に(2)(3)を付加
            AddParentheses,
            // ファイル名主部に[2][3]を付加
            AddBracket,
            // ファイル名主部に_を付加
            AddUnderBar,
        }
    }
}
