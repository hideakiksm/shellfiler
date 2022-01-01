using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileTask.DataObject;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：AccessibleFileにブリッジするストリームの基底クラス
    //=========================================================================================
    public abstract class AbstractBridgeStream : Stream {
        // 結果の書き込み先
        private IRetrieveFileDataTarget m_retrieveDataTarget;
        
        // ストリームがクローズされたときtrue
        private bool m_writeClosed;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]dataTarget  結果の書き込み先
        // 戻り値：なし
        //=========================================================================================
        public AbstractBridgeStream(IRetrieveFileDataTarget dataTarget) {
            m_retrieveDataTarget = dataTarget;
            m_writeClosed = false;
        }

        //=========================================================================================
        // プロパティ：結果の書き込み先
        //=========================================================================================
        protected IRetrieveFileDataTarget DataTarget {
            get {
                return m_retrieveDataTarget;
            }
        }
        
        //=========================================================================================
        // プロパティ：ストリームがクローズされたときtrue
        //=========================================================================================
        protected bool WriteClosed {
            get {
                return m_writeClosed;
            }
        }

        // 概要:
        //     派生クラスでオーバーライドされた場合は、現在のストリームが読み取りをサポートするかどうかを示す値を取得します。
        //
        // 戻り値:
        //     ストリームが読み込みをサポートしている場合は true。それ以外の場合は false。
        public override bool CanRead {
            get {
                return true;
            }
        }
        //
        // 概要:
        //     派生クラスでオーバーライドされた場合は、現在のストリームがシークをサポートするかどうかを示す値を取得します。
        //
        // 戻り値:
        //     ストリームがシークをサポートしている場合は true。それ以外の場合は false。
        public override bool CanSeek {
            get {
                return false;
            }
        }
        //
        // 概要:
        //     現在のストリームがタイムアウトできるかどうかを決定する値を取得します。
        //
        // 戻り値:
        //     現在のストリームがタイムアウトできるかどうかを決定する値。
        [ComVisible(false)]
        public override bool CanTimeout {
            get {
                return false;
            }
        }
        //
        // 概要:
        //     派生クラスでオーバーライドされた場合は、現在のストリームが書き込みをサポートするかどうかを示す値を取得します。
        //
        // 戻り値:
        //     ストリームが書き込みをサポートしている場合は true。それ以外の場合は false。
        public override bool CanWrite {
            get {
                return true;
            }
        }
        //
        // 概要:
        //     派生クラスでオーバーライドされた場合は、ストリームの長さをバイト単位で取得します。
        //
        // 戻り値:
        //     ストリーム長 (バイト単位) を表す long 値。
        //
        // 例外:
        //   System.NotSupportedException:
        //     Stream から派生したクラスがシークをサポートしていません。
        //
        //   System.ObjectDisposedException:
        //     ストリームが閉じられた後でメソッドが呼び出されました。
        public override long Length {
            get {
                throw new System.NotSupportedException("unsupported operation");
            }
        }
        //
        // 概要:
        //     派生クラスでオーバーライドされた場合は、現在のストリーム内の位置を取得または設定します。
        //
        // 戻り値:
        //     ストリーム内の現在位置。
        //
        // 例外:
        //   System.IO.IOException:
        //     I/O エラーが発生しました。
        //
        //   System.NotSupportedException:
        //     ストリームがシークをサポートしていません。
        //
        //   System.ObjectDisposedException:
        //     ストリームが閉じられた後でメソッドが呼び出されました。
        public override long Position {
            get {
                throw new System.NotSupportedException("unsupported operation");
            }
            set {
                throw new System.NotSupportedException("unsupported operation");
            }
        }
        //
        // 概要:
        //     ストリームがタイムアウト前に読み取りを試行する期間を決定する値 (ミリ秒単位) を取得または設定します。
        //
        // 戻り値:
        //     ストリームがタイムアウト前に読み取りを試行する期間を決定する値 (ミリ秒単位)。
        //
        // 例外:
        //   System.InvalidOperationException:
        //     System.IO.Stream.ReadTimeout メソッドは、常に System.InvalidOperationException をスローします。
        [ComVisible(false)]
        public override int ReadTimeout {
            get {
                throw new System.InvalidOperationException("unsupported operation");
            }
            set {
                throw new System.InvalidOperationException("unsupported operation");
            }
        }
        //
        // 概要:
        //     ストリームがタイムアウト前に書き込みを試行する期間を決定する値 (ミリ秒単位) を取得または設定します。
        //
        // 戻り値:
        //     ストリームがタイムアウト前に書き込みを試行する期間を決定する値 (ミリ秒単位)。
        //
        // 例外:
        //   System.InvalidOperationException:
        //     System.IO.Stream.WriteTimeout メソッドは、常に System.InvalidOperationException をスローします。
        [ComVisible(false)]
        public override int WriteTimeout {
            get {
                throw new System.InvalidOperationException("unsupported operation");
            }
            set {
                throw new System.InvalidOperationException("unsupported operation");
            }
        }
        // 概要:
        //     非同期の読み込み動作を開始します。
        //
        // パラメータ:
        //   buffer:
        //     データを読み込むバッファ。
        //
        //   offset:
        //     ストリームから読み込んだデータの書き込み開始位置を示す buffer 内のバイト オフセット。
        //
        //   count:
        //     読み取る最大バイト数。
        //
        //   callback:
        //     読み取り完了時に呼び出されるオプションの非同期コールバック。
        //
        //   state:
        //     この特定の非同期読み取り要求を他の要求と区別するために使用するユーザー指定のオブジェクト。
        //
        // 戻り値:
        //     非同期の読み込みを表す System.IAsyncResult。まだ保留状態の場合もあります。
        //
        // 例外:
        //   System.IO.IOException:
        //     ストリームの末尾を越えて非同期の読み込みを実行しようとしました。または、ディスク エラーが発生しました。
        //
        //   System.ArgumentException:
        //     1 つ以上の引数が無効です。
        //
        //   System.ObjectDisposedException:
        //     ストリームが閉じられた後でメソッドが呼び出されました。
        //
        //   System.NotSupportedException:
        //     現在の Stream 実装は、読み取り操作をサポートしていません。
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            throw new System.NotSupportedException("unsupported operation");
        }
        //
        // 概要:
        //     非同期の書き込み操作を開始します。
        //
        // パラメータ:
        //   buffer:
        //     データを書き込む元となるバッファ。
        //
        //   offset:
        //     書き込むデータの開始位置を示す buffer 内のバイト オフセット。
        //
        //   count:
        //     書き込む最大バイト数。
        //
        //   callback:
        //     書き込みの完了時に呼び出されるオプションの非同期コールバック。
        //
        //   state:
        //     この特定の非同期書き込み要求を他の要求と区別するために使用するユーザー指定のオブジェクト。
        //
        // 戻り値:
        //     非同期の書き込みを表す IAsyncResult。まだ保留状態の場合もあります。
        //
        // 例外:
        //   System.IO.IOException:
        //     ファイルの末尾を越えて非同期の書き込みを実行しようとしました。または、ディスク エラーが発生しました。
        //
        //   System.ArgumentException:
        //     1 つ以上の引数が無効です。
        //
        //   System.ObjectDisposedException:
        //     ストリームが閉じられた後でメソッドが呼び出されました。
        //
        //   System.NotSupportedException:
        //     現在の Stream 実装は、書き込み操作をサポートしていません。
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            throw new System.NotSupportedException("unsupported operation");
        }
        //
        // 概要:
        //     現在のストリームを閉じ、現在のストリームに関連付けられているすべてのリソース (ソケット、ファイル ハンドルなど) を解放します。
        public override void Close() {
            m_writeClosed = true;
        }
        //
        // 概要:
        //     System.Threading.WaitHandle オブジェクトを割り当てます。
        //
        // 戻り値:
        //     割り当てられた WaitHandle への参照。
        [Obsolete(@"CreateWaitHandle will be removed eventually.  Please use ""new ManualResetEvent(false)"" instead.")]
        protected override WaitHandle CreateWaitHandle() {
            return null;
        }
        //
        // 概要:
        //     System.IO.Stream によって使用されているアンマネージ リソースを解放し、オプションでマネージ リソースも解放します。
        //
        // パラメータ:
        //   disposing:
        //     マネージ リソースとアンマネージ リソースの両方を解放する場合は true。アンマネージ リソースだけを解放する場合は false。
        protected override void Dispose(bool disposing) {
        }
        //
        // 概要:
        //     保留中の非同期読み取りが完了するまで待機します。
        //
        // パラメータ:
        //   asyncResult:
        //     終了させる保留状態の非同期リクエストへの参照。
        //
        // 戻り値:
        //     ストリームから読み取ったバイト数 (0 ～要求したバイト数の間の数値)。ゼロ (0) が返されるのは、ストリームの末尾で読み取ろうとしたときだけです。それ以外の場合は、少なくとも
        //     1 バイトが読み込み可能になるまでブロックします。
        //
        // 例外:
        //   System.ArgumentNullException:
        //     asyncResult が null です。
        //
        //   System.ArgumentException:
        //     asyncResult は、現在のストリームの System.IO.Stream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        //     メソッドで開始されたものではありません。
        //
        //   System.IO.IOException:
        //     ストリームが閉じているか、内部エラーが発生しました。
        public override int EndRead(IAsyncResult asyncResult) {
            throw new System.ArgumentException("unsupported operation");
        }
        //
        // 概要:
        //     非同期書き込み操作を終了します。
        //
        // パラメータ:
        //   asyncResult:
        //     保留状態の非同期 I/O リクエストへの参照。
        //
        // 例外:
        //   System.ArgumentNullException:
        //     asyncResult が null です。
        //
        //   System.ArgumentException:
        //     asyncResult は、現在のストリームの System.IO.Stream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        //     メソッドで開始されたものではありません。
        //
        //   System.IO.IOException:
        //     ストリームが閉じているか、内部エラーが発生しました。
        public override void EndWrite(IAsyncResult asyncResult) {
            throw new System.ArgumentException("unsupported operation");
        }
        //
        // 概要:
        //     派生クラスによってオーバーライドされた場合は、ストリームに対応するすべてのバッファをクリアし、バッファ内のデータを基になるデバイスに書き込みます。
        //
        // 例外:
        //   System.IO.IOException:
        //     I/O エラーが発生しました。
        public override void Flush() {
        }
        //
        // 概要:
        //     派生クラスによってオーバーライドされた場合は、現在のストリームからバイト シーケンスを読み取り、読み取ったバイト数の分だけストリームの位置を進めます。
        //
        // パラメータ:
        //   buffer:
        //     バイト配列。このメソッドが戻るとき、指定したバイト配列の offset から (offset + count -1) までの値が、現在のソースから読み取られたバイトに置き換えられます。
        //
        //   offset:
        //     現在のストリームから読み取ったデータの格納を開始する位置を示す buffer 内のバイト オフセット。インデックス番号は 0 から始まります。
        //
        //   count:
        //     現在のストリームから読み取る最大バイト数。
        //
        // 戻り値:
        //     バッファに読み取られた合計バイト数。要求しただけのバイト数を読み取ることができなかった場合、この値は要求したバイト数より小さくなります。ストリームの末尾に到達した場合は
        //     0 (ゼロ) になることがあります。
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
        //     ストリームが読み取りをサポートしていません。
        //
        //   System.ObjectDisposedException:
        //     ストリームが閉じられた後でメソッドが呼び出されました。
        public override int Read(byte[] buffer, int offset, int count) {
            throw new System.NotSupportedException("unsupported operation");
        }
        //
        // 概要:
        //     ストリームから 1 バイトを読み取り、ストリーム内の位置を 1 バイト進めます。ストリームの末尾の場合は -1 を返します。
        //
        // 戻り値:
        //     Int32 にキャストされた符号なしバイト。ストリームの末尾の場合は -1。
        //
        // 例外:
        //   System.NotSupportedException:
        //     ストリームが読み取りをサポートしていません。
        //
        //   System.ObjectDisposedException:
        //     ストリームが閉じられた後でメソッドが呼び出されました。
        public override int ReadByte() {
            throw new System.NotSupportedException("unsupported operation");
        }
        //
        // 概要:
        //     派生クラスでオーバーライドされた場合は、現在のストリーム内の位置を設定します。
        //
        // パラメータ:
        //   offset:
        //     origin パラメータからのバイト オフセット。
        //
        //   origin:
        //     新しい位置を取得するために使用する参照ポイントを示す System.IO.SeekOrigin 型の値。
        //
        // 戻り値:
        //     現在のストリーム内の新しい位置。
        //
        // 例外:
        //   System.IO.IOException:
        //     I/O エラーが発生しました。
        //
        //   System.NotSupportedException:
        //     ストリームがシークをサポートしていません。たとえば、ストリームがパイプまたはコンソール出力によって生成された可能性があります。
        //
        //   System.ObjectDisposedException:
        //     ストリームが閉じられた後でメソッドが呼び出されました。
        public override long Seek(long offset, SeekOrigin origin) {
            throw new System.NotSupportedException("unsupported operation");
        }
        //
        // 概要:
        //     派生クラスでオーバーライドされた場合は、現在のストリームの長さを設定します。
        //
        // パラメータ:
        //   value:
        //     現在のストリームの希望の長さ (バイト数)。
        //
        // 例外:
        //   System.IO.IOException:
        //     I/O エラーが発生しました。
        //
        //   System.NotSupportedException:
        //     ストリームが書き込みとシークの両方をサポートしていません。たとえば、ストリームがパイプまたはコンソール出力によって生成された可能性があります。
        //
        //   System.ObjectDisposedException:
        //     ストリームが閉じられた後でメソッドが呼び出されました。
        public override void SetLength(long value) {
            throw new System.NotSupportedException("unsupported operation");
        }
    }
}
