using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Util;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Virtual {

    //=========================================================================================
    // クラス：仮想ディレクトリ用展開処理のリクエスト／レスポンス
    //=========================================================================================
    public abstract class VirtualExtractArg {
        // [in]ファイル一覧のコンテキスト情報
        private IFileListContext m_fileListContext;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileListCtx    ファイル一覧のコンテキスト情報
        // 戻り値：なし
        //=========================================================================================
        public VirtualExtractArg(IFileListContext fileListCtx) {
            m_fileListContext = fileListCtx;
        }

        //=========================================================================================
        // プロパティ：[in]ファイル一覧のコンテキスト情報
        //=========================================================================================
        public IFileListContext FileListContext {
            get {
                return m_fileListContext;
            }
        }
    }
}
