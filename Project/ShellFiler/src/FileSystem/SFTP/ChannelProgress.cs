using System;
using System.Collections.Generic;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using ShellFiler.Api;

namespace ShellFiler.FileSystem.SFTP {

    //=========================================================================================
    // クラス：SFTPファイル操作の進捗状況
    //=========================================================================================
    class ChannelProgress : SftpProgressMonitor {
        // 進捗状況の通知delegate
        FileProgressEventHandler m_progress;

        // 現在のサイズ
        private long m_totalBytesTransferred = 0;
		
        // 最大サイズ
		private long m_totalFileSize = 0;

        // 転送元、転送先の両方を処理するときtrue（totalの2倍を全体サイズとして処理する）
        private bool m_srcDestMode = false;

        // キャンセルされたときtrue
        private bool m_canceled = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]progress  進捗状況の通知delegate
        // 戻り値：なし
        //=========================================================================================
        public ChannelProgress(FileProgressEventHandler progress) {
            m_progress = progress;
        }
        
        //=========================================================================================
        // 機　能：転送元と転送先の両方を処理することを設定する
        // 引　数：[in]nowSrc  現在が転送元の処理中のときtrue
        // 戻り値：なし
        //=========================================================================================
        public void SetSrcDestMode(bool nowSrc) {
            m_srcDestMode = true;
        }

        //=========================================================================================
        // プロパティ：キャンセルされたときtrue
        //=========================================================================================
        public bool Canceled {
            get {
                return m_canceled;
            }
        }

        //=========================================================================================
        // 機　能：転送を初期化する
        // 引　数：[in]op    操作
        // 　　　　[in]src   転送元ファイル名
        // 　　　　[in]dest  転送先ファイル名
        // 　　　　[int]max  最大サイズ
        // 戻り値：なし
        //=========================================================================================
        public override void init(int op, String src, String dest, long max) {
            if (m_srcDestMode) {
                m_totalFileSize = max * 2;
            } else {
                m_totalFileSize = max;
            }
        }

        //=========================================================================================
        // 機　能：進捗を通知する
        // 引　数：[in]c   追加のカウント
        // 戻り値：なし
        //=========================================================================================
        public override bool count(long c) {
            m_totalBytesTransferred += c;

            FileProgressEventArgs e = new FileProgressEventArgs(m_totalFileSize, m_totalBytesTransferred);
            m_progress.SetProgress(this, e);

            m_canceled |= e.Cancel;
			return !e.Cancel;
		}

        //=========================================================================================
        // 機　能：転送の終了を通知する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override void end() {
		}
    }
}
