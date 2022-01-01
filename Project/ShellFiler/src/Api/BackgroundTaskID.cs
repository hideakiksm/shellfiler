
using System;
using System.Threading;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：バックグラウンドタスクのID
    //=========================================================================================
    public class BackgroundTaskID : IdImpl {
        // 管理外のID
        public static readonly BackgroundTaskID NullId = new BackgroundTaskID(0);

        // 次に発行するID
        private static int s_nextId = 1;
        
        //=========================================================================================
        // 機　能：新しいIDを振り出す
        // 引　数：なし
        // 戻り値：タスクID
        //=========================================================================================
        public static BackgroundTaskID NextId() {
            return new BackgroundTaskID(Interlocked.Add(ref s_nextId, 1));
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]taskId    設定するID
        // 戻り値：なし
        //=========================================================================================
        private BackgroundTaskID(int id) : base(id) {
        }
    }
}
