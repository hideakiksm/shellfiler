using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileTask.DataObject;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：StdOutをAccessibleFileにブリッジするストリーム
    //=========================================================================================
    public class BridgeStdOutStream : AbstractBridgeStream {
        // Closeを通知したときtrue
        private bool m_notifyClose = false;

        // 高速に読み込むときtrue
        private bool m_fastRead;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]accessibleFile  結果の書き込み先
        // 　　　　[in]fastRead        高速に読み込むときtrue
        // 戻り値：なし
        //=========================================================================================
        public BridgeStdOutStream(IRetrieveFileDataTarget dataTarget, bool fastRead) : base(dataTarget) {
            m_fastRead = fastRead;
        }
        
        //
        // 概要:
        //     現在のストリームを閉じ、現在のストリームに関連付けられているすべてのリソース (ソケット、ファイル ハンドルなど) を解放します。
        public override void Close() {
            base.Close();
            if (!m_notifyClose) {
                m_notifyClose = true;
                DataTarget.AddCompleted(RetrieveDataLoadStatus.CompletedAll, null);
            }
        }
        //
        // 概要:
        //     派生クラスによってオーバーライドされた場合は、現在のストリームにバイト シーケンスを書き込み、書き込んだバイト数の分だけストリームの現在位置を進めます。
        //
        // パラメータ:
        //   buffer:
        //     バイト配列。このメソッドは、buffer から現在のストリームに、count で指定されたバイト数だけコピーします。
        //
        //   offset:
        //     現在のストリームへのバイトのコピーを開始する位置を示す buffer 内のバイト オフセット。インデックス番号は 0 から始まります。
        //
        //   count:
        //     現在のストリームに書き込むバイト数。
        //
        // 例外:
        //   System.ArgumentException:
        //     offset と count の合計値が、バッファ長より大きい値です。
        //
        //   System.ArgumentNullException:
        //     buffer が null です。
        //
        //   System.ArgumentOutOfRangeException:
        //     offset または count が負の値です。
        //
        //   System.IO.IOException:
        //     I/O エラーが発生しました。
        //
        //   System.NotSupportedException:
        //     ストリームが書き込みをサポートしません。
        //
        //   System.ObjectDisposedException:
        //     ストリームが閉じられた後でメソッドが呼び出されました。
        public override void Write(byte[] buffer, int offset, int count) {
            if (WriteClosed) {
                throw new System.ObjectDisposedException("closed");
            }
            DataTarget.AddData(buffer, offset, count);
            if (!m_fastRead) {
                Thread.Sleep(10);
            }
        }
        //
        // 概要:
        //     ストリームの現在位置にバイトを書き込み、ストリームの位置を 1 バイトだけ進めます。
        //
        // パラメータ:
        //   value:
        //     ストリームに書き込むバイト。
        //
        // 例外:
        //   System.IO.IOException:
        //     I/O エラーが発生しました。
        //
        //   System.NotSupportedException:
        //     ストリームが書き込みをサポートしていないか、既に閉じています。
        //
        //   System.ObjectDisposedException:
        //     ストリームが閉じられた後でメソッドが呼び出されました。
        public override void WriteByte(byte value) {
            if (WriteClosed) {
                throw new System.ObjectDisposedException("closed");
            }
            byte[] tempBuffer = new byte[1];
            tempBuffer[0] = value;
            DataTarget.AddData(tempBuffer, 0, 1);
        }
    }
}
