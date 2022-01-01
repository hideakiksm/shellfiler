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
    // クラス：仮想ディレクトリ用マークファイルの展開処理のリクエスト／レスポンス
    //=========================================================================================
    public class VirtualExtractMarkedArg : VirtualExtractArg {
        // [in]対象ファイルのパス名のリスト
        private List<string> m_dispFileList;

        // [out]展開したファイルの作業フォルダ内でのリスト（結果が決定するまではnull）
        private List<string> m_tempFileList;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileListCtx    ファイル一覧のコンテキスト情報
        // 　　　　[in]dispFileList   対象ファイルのパス名のリスト
        // 戻り値：なし
        //=========================================================================================
        public VirtualExtractMarkedArg(IFileListContext fileListCtx, List<string> dispFileList) : base(fileListCtx) {
            m_dispFileList = dispFileList;
        }

        //=========================================================================================
        // プロパティ：[in]対象ファイルのパス名のリスト
        //=========================================================================================
        public List<string> DispFileList {
            get {
                return m_dispFileList;
            }
        }

        //=========================================================================================
        // プロパティ：[out]展開したファイルの作業フォルダ内でのリスト
        //=========================================================================================
        public List<string> TempFileList {
            get {
                return m_tempFileList;
            }
            set {
                m_tempFileList = value;
            }
        }
    }
}
