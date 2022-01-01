using System;
using System.IO;
using System.Text;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：データ入力ストリーム
    //=========================================================================================
    public class DataInputStream {
        // 入力先のストリーム
        private Stream m_baseStream;

        // 文字列に使用するエンコード
        private Encoding m_encoding;


        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]baseStream   入力先のストリーム
        // 　　　　[in]encoding     文字列に使用するエンコード
        // 戻り値：なし
        //=========================================================================================
        public DataInputStream(Stream baseStream, Encoding encoding) {
            m_baseStream = baseStream;
            m_encoding = encoding;
        }

        //=========================================================================================
        // 機　能：ストリームを閉じる
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Close() {
            m_baseStream.Close();
        }

        //=========================================================================================
        // 機　能：文字列を入力する
        // 引　数：なし
        // 戻り値：文字列
        //=========================================================================================
        public string ReadString() {
            int size = ReadInt();
            if (size < 0 || size > 0xffffff) {
                throw new Exception();
            }
            byte[] data = new byte[size];
            int readSize = m_baseStream.Read(data, 0, data.Length);
            if (readSize != data.Length) {
                throw new Exception();
            }
            return m_encoding.GetString(data);
        }

        //=========================================================================================
        // 機　能：数値を入力する
        // 引　数：なし
        // 戻り値：数値
        //=========================================================================================
        public int ReadInt() {
            byte[] data = new byte[4];
            int readSize = m_baseStream.Read(data, 0, data.Length);
            if (readSize != data.Length) {
                throw new Exception();
            }
            int intData = ((int)(data[0]) << 24) |
                          ((int)(data[1]) << 16) |
                          ((int)(data[2]) <<  8) |
                          (int)(data[3]);
            return intData;
        }
    }
}
