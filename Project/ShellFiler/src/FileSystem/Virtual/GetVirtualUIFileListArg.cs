using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：仮想ディレクトリでUI用のファイル一覧をバックグラウンドで取得するリクエスト/レスポンス
    //=========================================================================================
    public class GetVirtualUIFileListArg : AbstractProcedureArg {
        // [in]直前の仮想フォルダの情報
        private VirtualFolderInfo m_virtualFolder;

        // [in]一覧取得する表示ディレクトリ
        private string m_displayDirectory;

        // [in]左ウィンドウに対する一覧取得のときtrue
        private bool m_isLeftWindow;

        // [in]ディレクトリ変更のモード
        private ChangeDirectoryParam m_changeDirectoryMode;

        // [in]IFileListでの読み込みの世代
        private int m_loadingGeneration;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]context           コンテキスト情報
        // 　　　　[in]virtualFolder     直前の仮想フォルダの情報
        // 　　　　[in]dispDir           一覧取得する表示ディレクトリ
        // 　　　　[in]isLeftWindow      左ウィンドウに対する一覧取得のときtrue
        // 　　　　[in]chdirMode         ディレクトリ変更のモード
        // 　　　　[in]loadingGeneration IFileListでの読み込みの世代
        // 戻り値：なし
        //=========================================================================================
        public GetVirtualUIFileListArg(FileOperationRequestContext context, VirtualFolderInfo virtualFolder, string dispDir, bool isLeftWindow, ChangeDirectoryParam chdirMode, int loadingGeneration) : base(context) {
            m_virtualFolder = virtualFolder;
            m_displayDirectory = dispDir;
            m_isLeftWindow = isLeftWindow;
            m_changeDirectoryMode = chdirMode;
            m_loadingGeneration = loadingGeneration;
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
        // プロパティ：[in]直前の仮想フォルダの情報
        //=========================================================================================
        public VirtualFolderInfo VirtualFolder {
            get {
                return m_virtualFolder;
            }
        }

        //=========================================================================================
        // プロパティ：[in]一覧取得する表示ディレクトリ
        //=========================================================================================
        public string DisplayDirectory {
            get {
                return m_displayDirectory;
            }
        }

        //=========================================================================================
        // プロパティ：[in]左ウィンドウに対する一覧取得のときtrue
        //=========================================================================================
        public bool IsLeftWindow {
            get {
                return m_isLeftWindow;
            }
        }

        //=========================================================================================
        // プロパティ：[in]ディレクトリ変更のモード
        //=========================================================================================
        public ChangeDirectoryParam ChangeDirectoryMode {
            get {
                return m_changeDirectoryMode;
            }
        }

        //=========================================================================================
        // プロパティ：[in]IFileListでの読み込みの世代
        //=========================================================================================
        public int LoadingGeneration {
            get {
                return m_loadingGeneration;
            }
        }
    }
}
