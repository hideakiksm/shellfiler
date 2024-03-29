﻿using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask.DataObject;

namespace ShellFiler.Api {

    //=========================================================================================
    // インターフェース：ファイル群の情報を取得するための定義
    //=========================================================================================
    public interface IFileList {

        //=========================================================================================
        // 機　能：一覧を取得する
        // 引　数：[in]chdirMode    ディレクトリ変更のモード
        // 戻り値：一覧取得のステータス
        //=========================================================================================
        ChangeDirectoryStatus GetFileList(ChangeDirectoryParam chdirMode);

        //=========================================================================================
        // 機　能：並列読み込みを開始する
        // 引　数：[in]chdirMode   ディレクトリ変更のモード
        // 戻り値：なし
        //=========================================================================================
        void StartLoading(ChangeDirectoryParam chdirMode);

        //=========================================================================================
        // プロパティ：一覧取得に使用するファイルシステム
        //=========================================================================================
        IFileSystem FileSystem {
            get;
        }
        
        //=========================================================================================
        // プロパティ：ファイル一覧
        //=========================================================================================
        List<IFile> Files {
            get;
        }
        
        //=========================================================================================
        // プロパティ：ディレクトリ名（最後は必ずセパレータ）
        //=========================================================================================
        string DirectoryName {
            get;
        }

        //=========================================================================================
        // プロパティ：左ウィンドウで表示される一覧のときtrue
        //=========================================================================================
        bool IsLeftWindow {
            get;
        }

        //=========================================================================================
        // プロパティ：ファイル再読込の世代番号
        //=========================================================================================
        int LoadingGeneration {
            get;
        }
                
        //=========================================================================================
        // プロパティ：ボリューム情報
        //=========================================================================================
        VolumeInfo VolumeInfo {
            get;
        }

        //=========================================================================================
        // プロパティ：ファイルシステムごとのコンテキスト情報（一覧取得前はnull）
        //=========================================================================================
        IFileListContext FileListContext {
            get;
        }
    }
}
