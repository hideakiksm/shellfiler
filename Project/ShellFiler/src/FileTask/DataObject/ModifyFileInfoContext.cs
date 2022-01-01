using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Util;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI.Dialog;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // クラス：ファイル情報一括編集を実行するときのコンテキスト情報
    //=========================================================================================
    public class ModifyFileInfoContext {
        // 現在の連番の番号（初期化前のとき-1）
        private long m_sequenceNumber = -1;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]renameInfo  
        // 戻り値：なし
        //=========================================================================================
        public ModifyFileInfoContext() {
        }

        //=========================================================================================
        // プロパティ：現在の連番の番号（初期化前のとき-1）
        //=========================================================================================
        public long SequenceNumber {
            get {
                return m_sequenceNumber;
            }
            set {
                m_sequenceNumber = value;
            }
        }
    }
}
