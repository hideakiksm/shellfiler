﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // インターフェース：ファイル１つの情報を取得するための定義
    //=========================================================================================
    public interface IFile : ICloneable {
        
        //=========================================================================================
        // プロパティ：ファイル名（パス名は含まない）
        //=========================================================================================
        string FileName {
            get;
        }

        //=========================================================================================
        // プロパティ：拡張子
        //=========================================================================================
        string Extension {
            get;
        }
        
        //=========================================================================================
        // プロパティ：表示用拡張子の位置とする「.」の直後の文字のインデックス（-1:拡張子を分離表示しない）
        //=========================================================================================
        int DisplayExtensionPos {
            get;
        }

        //=========================================================================================
        // プロパティ：ファイルの更新時刻
        //=========================================================================================
        DateTime ModifiedDate {
            get;
        }

        //=========================================================================================
        // プロパティ：ファイル属性
        //=========================================================================================
        FileAttribute Attribute {
            get;
        }

        //=========================================================================================
        // プロパティ：ファイルサイズ
        //=========================================================================================
        long FileSize {
            get;
            set;
        }

        //=========================================================================================
        // プロパティ：デフォルトのサイズ順
        //=========================================================================================
        int DefaultOrder {
            get;
            set;
        }
    }
}
