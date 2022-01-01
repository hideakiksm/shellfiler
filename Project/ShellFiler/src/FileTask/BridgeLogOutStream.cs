using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileTask.DataObject;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：StdOutをログ出力用に行単位でブリッジするストリーム
    //=========================================================================================
    public class BridgeLogOutStream : AbstractBridgeStream {
        // Closeを通知したときtrue
        private bool m_notifyClose = false;

        // 現在のバッファ
        private byte[] m_currentBuffer = new byte[0];

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]accessibleFile  結果の書き込み先
        // 戻り値：なし
        //=========================================================================================
        public BridgeLogOutStream(IRetrieveFileDataTarget dataTarget) : base(dataTarget) {
        }
        
        //
        // 概要:
        //     現在のストリームを閉じ、現在のストリームに関連付けられているすべてのリソース (ソケット、ファイル ハンドルなど) を解放します。
        public override void Close() {
            base.Close();
            byte[] tempBuffer = new byte[0];
            m_currentBuffer = ParseBuffer(m_currentBuffer, tempBuffer, 0, 0, true);
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

            m_currentBuffer = ParseBuffer(m_currentBuffer, buffer, offset, count, false);
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
            m_currentBuffer = ParseBuffer(m_currentBuffer, tempBuffer, 0, 1, false);
        }

        //=========================================================================================
        // 機　能：プロセスの出力を行単位に分解して記録する（標準出力/標準エラー出力の共通化）
        // 引　数：[in]prevBuffer  直前までの解析結果の残り
        // 　　　　[in]chunkBuffer 受け取った文字列のバッファ
        // 　　　　[in]count       bufferで有効な部分の長さ
        // 　　　　[in]isStdOut    標準出力に書き込むときtrue/標準エラー出力に書き込むときfalse
        // 　　　　[in]final       最後の受信のときtrue
        // 戻り値：次回にprevBufferとして渡す、解析中のバッファ
        //=========================================================================================
        private byte[] ParseBuffer(byte[] prevBuffer, byte[] chunkBuffer, int chunkStart, int chunkLength, bool final) {
            byte[] newBuffer = new byte[prevBuffer.Length + chunkLength];
            Array.Copy(prevBuffer, 0, newBuffer, 0, prevBuffer.Length);
            Array.Copy(chunkBuffer, chunkStart, newBuffer, prevBuffer.Length, chunkLength);
            int start = 0;
            int index = 0;
            while (index < newBuffer.Length) {
                if (newBuffer[index] == '\n') {
                    int end = index - 1;
                    StoreData(newBuffer, start, end - start + 1);
                    start = index + 1;
                }
                index++;
            }

            // chunkBufferにもう改行がない
            if (final) {
                // 最後の場合は残りを行に格納
                if (start != index) {
                    StoreData(newBuffer, start, newBuffer.Length - start);
                    prevBuffer = new byte[0];
                }
            } else {
                // 途中の場合は次回の解析に回す
                prevBuffer = new byte[newBuffer.Length - start];
                Array.Copy(newBuffer, start, prevBuffer, 0, prevBuffer.Length);
            }
            return prevBuffer;
        }

        //=========================================================================================
        // 機　能：取得したデータを格納する
        // 引　数：[in]buffer  取得したデータの格納バッファ
        // 　　　　[in]start   データの先頭位置
        // 　　　　[in]length  データの長さ
        // 戻り値：なし
        //=========================================================================================
        private void StoreData(byte[] buffer, int start, int length) {
            byte[] lineBuffer = new byte[length + 1];
            Array.Copy(buffer, start, lineBuffer, 0, length);
            lineBuffer[length] = (byte)'\n';
            DataTarget.AddData(lineBuffer, 0, lineBuffer.Length);
        }
    }
}
