using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // 機　能：ダミーのSSH接続用リクエスト/レスポンス（NOP相当）
    //=========================================================================================
    public class NopArg : AbstractProcedureArg {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]cache  キャッシュ情報
        // 戻り値：なし
        //=========================================================================================
        public NopArg(FileOperationRequestContext cache) : base(cache) {
        }
    }
}
