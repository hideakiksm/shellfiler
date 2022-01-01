using System;
using System.IO;
using System.Text;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：データ出力ストリーム
    //=========================================================================================
    public class DataOutputStream {
        // 出力先のストリーム
        private Stream m_baseStream;

        // 文字列に使用するエンコード
        private Encoding m_encoding;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]baseStream   出力先のストリーム
        // 　　　　[in]encoding     文字列に使用するエンコード
        // 戻り値：なし
        //=========================================================================================
        public DataOutputStream(Stream baseStream, Encoding encoding) {
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
        // 機　能：文字列を出力する
        // 引　数：[in]value  文字列
        // 戻り値：なし
        //=========================================================================================
        public void WriteString(string value) {
            if (value == null) {
                WriteInt(-1);
            } else {
                byte[] data = m_encoding.GetBytes(value);
                WriteInt(data.Length);
                m_baseStream.Write(data, 0, data.Length);
            }
        }

        //=========================================================================================
        // 機　能：数値を出力する
        // 引　数：[in]value  数値
        // 戻り値：なし
        //=========================================================================================
        public void WriteInt(int value) {
            byte[] data = new byte[4];
            data[0] = (byte)((value >> 24) & 0xff);
            data[1] = (byte)((value >> 16) & 0xff);
            data[2] = (byte)((value >>  8) & 0xff);
            data[3] = (byte)((value >>  0) & 0xff);
            m_baseStream.Write(data, 0, data.Length);
        }
    }
}
