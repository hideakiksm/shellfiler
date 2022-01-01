using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;

namespace ShellFiler.Document.OSSpec {

    //=========================================================================================
    // クラス：トークンの利用コマンド
    //=========================================================================================
    public class OsSpecCommandGroup {
        public static readonly OsSpecCommandGroup Common        = new OsSpecCommandGroup("");                    // 共通
        public static readonly OsSpecCommandGroup GetFileList   = new OsSpecCommandGroup("GetFileList");         // ファイル一覧の取得
        public static readonly OsSpecCommandGroup GetVolumeInfo = new OsSpecCommandGroup("GetVolumeInfo");       // ボリューム情報の取得

        // 定義中でのトークン
        private string m_defFileToken;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]defFileToken  定義中でのトークン
        // 戻り値：なし
        //=========================================================================================
        private OsSpecCommandGroup(string defFileToken) {
            m_defFileToken = defFileToken;
        }
    }
}
