using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Properties;

namespace ShellFiler.UI.Log.Logger {

    //=========================================================================================
    // クラス：ログ出力する1行の内容（単純な文字列）
    //=========================================================================================
    public class LogLineArchiveDetail : LogLineSimple {
        // 全体サイズ
        private long m_totalSize = -1;

        // 処理済みサイズ
        private long m_completedSize = -1;

        // 詳細ファイル
        private string m_detailFile = "";

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public LogLineArchiveDetail() : base(Resources.Log_ArchiveDetail) {
        }

        //=========================================================================================
        // 機　能：現在の内部状態を元にメッセージをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void SetMessage() {
            Message = string.Format(Resources.Log_ArchiveDetailFormat, m_totalSize, m_completedSize, m_detailFile);
        }

        //=========================================================================================
        // 機　能：作業完了時のメッセージに切り替える
        // 引　数：[in]realSize    圧縮後のサイズ
        // 戻り値：なし
        //=========================================================================================
        public void SetCompletedMessage(long realSize) {
            int ratio;
            if (m_totalSize == 0) {
                ratio = 100;
            } else {
                ratio = (int)(realSize * 100 / m_totalSize);
            }
            Message = string.Format(Resources.Log_ArchiveDetailEnd, m_totalSize, realSize, ratio);
        }

        //=========================================================================================
        // 機　能：キャンセル時のメッセージに切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetCancelMessage() {
            Message = string.Format(Resources.Log_ArchiveDetailCancel);
        }

        //=========================================================================================
        // プロパティ：全体サイズ
        //=========================================================================================
        public long TotalSize {
            set {
                m_totalSize = value;
                SetMessage();
            }
        }

        //=========================================================================================
        // プロパティ：処理済みサイズ
        //=========================================================================================
        public long CompletedSize {
            set {
                m_completedSize = value;
                SetMessage();
            }
        }

        //=========================================================================================
        // プロパティ：詳細ファイル
        //=========================================================================================
        public string DetailFile {
            set {
                m_detailFile = value;
                SetMessage();
            }
        }
    }
}
