
using System;
using System.Threading;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：TerminalShellChannelのID
    //=========================================================================================
    public class TerminalShellChannelID : IdImpl {
        // 管理外のID
        public static readonly TerminalShellChannelID NullId = new TerminalShellChannelID(0);

        // 次に発行するID
        private static int s_nextId = 1;
        
        //=========================================================================================
        // 機　能：新しいIDを振り出す
        // 引　数：なし
        // 戻り値：タスクID
        //=========================================================================================
        public static TerminalShellChannelID NextId() {
            return new TerminalShellChannelID(Interlocked.Add(ref s_nextId, 1));
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]taskId    設定するID
        // 戻り値：なし
        //=========================================================================================
        private TerminalShellChannelID(int id) : base(id) {
        }
    }
}
