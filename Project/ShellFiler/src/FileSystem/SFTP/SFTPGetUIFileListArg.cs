using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：UI用ファイル一覧をバックグラウンドで取得するリクエスト/レスポンス
    //=========================================================================================
    public class SFTPGetUIFileListArg : AbstractProcedureArg {
        // [in]取得状態にあるファイル一覧（バックグラウンドで取得結果を書き込む）
        private IFileList m_fileList;

        // [in]一覧を作成するディレクトリ
        private string m_directory;

        // [in]ディレクトリ変更のモード
        private ChangeDirectoryParam m_changeDirectoryMode;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]context      コンテキスト情報
        //         [in]fileList     取得状態にあるファイル一覧（バックグラウンドで取得結果を書き込む）
        // 　　　　[in]directory    一覧を作成するディレクトリ
        // 　　　　[in]chdirMode    ディレクトリ変更のモード
        // 戻り値：なし
        //=========================================================================================
        public SFTPGetUIFileListArg(FileOperationRequestContext context, IFileList fileList, string directory, ChangeDirectoryParam chdirMode) : base(context) {
            m_fileList = fileList;
            m_directory = directory;
            m_changeDirectoryMode = chdirMode;
        }

        //=========================================================================================
        // プロパティ：このリクエストを非同期に実行するときtrue（戻りを待たなくてよいときtrue）
        //=========================================================================================
        public override bool IsAsyncRequest {
            get {
                return true;
            }
        }

        //=========================================================================================
        // プロパティ：[in]取得状態にあるファイル一覧（バックグラウンドで取得結果を書き込む）
        //=========================================================================================
        public IFileList FileList {
            get {
                return m_fileList;
            }
            set {
                m_fileList = value;         // 仮想フォルダへの書き換え用
            }
        }

        //=========================================================================================
        // プロパティ：[in]一覧を作成するディレクトリ
        //=========================================================================================
        public string Directory {
            get {
                return m_directory;
            }
        }

        //=========================================================================================
        // [in]ディレクトリ変更のモード
        //=========================================================================================
        public ChangeDirectoryParam ChangeDirectoryMode {
            get {
                return m_changeDirectoryMode;
            }
        }
    }
}
