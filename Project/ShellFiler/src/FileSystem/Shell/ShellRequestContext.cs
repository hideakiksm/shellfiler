using System;
using System.Collections.Generic;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document.SSH;
using ShellFiler.Terminal.TerminalSession;

namespace ShellFiler.FileSystem.Shell {

    //=========================================================================================
    // クラス：SSHシェルリクエストのコンテキスト情報
    //=========================================================================================
    public class ShellRequestContext {
        // 使用中のシェルチャネルのリスト
        private Dictionary<TerminalShellChannelID, TerminalShellChannel> m_mapIdToChannel = new Dictionary<TerminalShellChannelID, TerminalShellChannel>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ShellRequestContext() {
        }

        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            m_mapIdToChannel.Clear();
        }

        //=========================================================================================
        // 機　能：シェルチャネルを追加する
        // 引　数：[in]id       使用するチャネルのID
        // 　　　　[in]channel  シェルチャネル
        // 戻り値：なし
        //=========================================================================================
        public void Add(TerminalShellChannelID id, TerminalShellChannel channel) {
            m_mapIdToChannel.Add(id, channel);
        }

        //=========================================================================================
        // 機　能：シェルチャネルを返す
        // 引　数：[in]id  使用するチャネルのID
        // 戻り値：シェルチャネル
        //=========================================================================================
        public TerminalShellChannel GetTerminalShellChannel(TerminalShellChannelID id) {
            if (id == TerminalShellChannelID.NullId) {
                return null;
            }
            lock (this) {
                if (m_mapIdToChannel.ContainsKey(id)) {
                    return m_mapIdToChannel[id];
                }
            }
            return null;
        }
    }
}
