using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.UI.FileList;
using ShellFiler.Util;

namespace ShellFiler.FileTask.FileFilter {

    //=========================================================================================
    // クラス：BASE64エンコードを実行するフィルター
    //=========================================================================================
    public class FileFilterBase64Encode : IFileFilterComponent {
        // プロパティ
        private const string PROP_FOLDING = "Folding";          // フォールディングの桁数（0:しない）
        private const string PROP_HEAD_SPACE = "HeadSpace";     // 行頭にスペースを付けるときtrue

        // BASE64の構成文字
        private const string BASE64STRING = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        // フォールディングの最大桁数
        private const int MAX_FOLDING = 1024;

        // フォールディングの最小桁数
        private const int MIN_FOLDING = 20;

        // CR/LFのコード定義
        private const byte BYTE_CR = (byte)0x0d;                                // CR
        private const byte BYTE_LF = (byte)0x0a;                                // LF

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterBase64Encode() {
        }

        //=========================================================================================
        // 機　能：現在の設定に対する表示用のパラメータを取得する
        // 引　数：[in]single  パラメータ情報を1行で作成するときtrue
        // 　　　　[in]param   パラメータ
        // 戻り値：表示用のパラメータ
        //=========================================================================================
        public string[] GetDisplayParameter(bool single, Dictionary<string, object> param) {
            string dispFolding, dispSpace;
            int folding = (int)param[PROP_FOLDING];
            if (folding == 0) {
                dispFolding = string.Format(Resources.FileFilter_B64EFoling, "-");
            } else {
                dispFolding = string.Format(Resources.FileFilter_B64EFoling, folding);
            }
            bool headSpace = (bool)param[PROP_HEAD_SPACE];
            if (headSpace) {
                dispSpace = Resources.FileFilter_B64EHeadSpaceOn;
            } else {
                dispSpace = Resources.FileFilter_B64EHeadSpaceOff;
            }
            if (single) {
                return new string[] { dispFolding + ", " + dispSpace };
            } else {
                return new string[] { dispFolding, dispSpace };
            }
        }
        
        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public FileFilterItem CreateFileFilterItem() {
            FileFilterItem item = new FileFilterItem();
            item.FileFilterClassPath = this.GetType().FullName;
            item.PropertyList.Add(PROP_FOLDING, 0);
            item.PropertyList.Add(PROP_HEAD_SPACE, false);
            return item;
        }

        //=========================================================================================
        // 機　能：フィルター設定用の項目を作成する
        // 引　数：なし
        // 戻り値：フィルター設定用の項目
        //=========================================================================================
        public List<SettingUIItem> GetSettingUI() {
            List<SettingUIItem> itemList = new List<SettingUIItem>();
            itemList.Add(new SettingUIItem.Numeric(Resources.FileFilter_B64EUI01Folding, PROP_FOLDING, 0, 1024));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_B64EUI02Label, 1));
            itemList.Add(new SettingUIItem.Space());
            itemList.Add(new SettingUIItem.Checkbox(Resources.FileFilter_B64EUI03HeadSpace, PROP_HEAD_SPACE));
            itemList.Add(new SettingUIItem.Label(Resources.FileFilter_B64EUI04Label, 1));
            return itemList;
        }

        //=========================================================================================
        // 機　能：パラメータが正しいかどうかを確認する
        // 引　数：[in]param   確認するパラメータ
        // 戻り値：エラーメッセージ（エラーがないときnull）
        //=========================================================================================
        public string CheckParameter(Dictionary<string, object> param) {
            if (!param.ContainsKey(PROP_FOLDING) || !(param[PROP_FOLDING] is int)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            int folding = (int)param[PROP_FOLDING];
            if (folding < 0 || folding > MAX_FOLDING) {
                return Resources.FileFilter_MsgSerializeError;
            }
            if (!param.ContainsKey(PROP_HEAD_SPACE) || !(param[PROP_HEAD_SPACE] is bool)) {
                return Resources.FileFilter_MsgSerializeError;
            }
            return null;
        }

        //=========================================================================================
        // 機　能：変換を実行する
        // 引　数：[in]orgFileName 元のファイルパス（クリップボードのときnull）
        // 　　　　[in]src         変換元のバイト列
        // 　　　　[out]dest       変換先のバイト列を返す変数（変換元と同一になる可能性あり）
        // 　　　　[in]param       変換に使用するパラメータ
        // 　　　　[in]cancelEvent キャンセル時にシグナル状態になるイベント
        // 戻り値：ステータス
        //=========================================================================================
        public FileOperationStatus Convert(string orgFileName, byte[] src, out byte[] dest, Dictionary<string, object> param, WaitHandle cancelEvent) {
            dest = null;

            // パラメータを取得
            int folding = (int)param[PROP_FOLDING];
            if (folding > 0 && folding < MIN_FOLDING) {
                return FileOperationStatus.FailSetting;
            }
            bool headSpace = (bool)param[PROP_HEAD_SPACE];

            FoldingMemoryStream outStream = new FoldingMemoryStream(folding, headSpace);
            int byteCount = 0;
            int prevByte = 0;
            int length = src.Length;
            for (int i = 0; i < length; i++) {
                // 00000000 11111111 22222222
                // 000000 001111 111122 222222
                if (byteCount == 0) {
                    int charIndex1 = (((int)src[i]) >> 2) & 0x3f;
                    outStream.WriteByte(false, (byte)(BASE64STRING[charIndex1]));
                    prevByte = (((int)src[i]) << 4) & 0x3f;
                    byteCount++;
                } else if (byteCount == 1) {
                    int charIndex2 = (prevByte | (((int)src[i]) >> 4)) & 0x3f;
                    outStream.WriteByte(false, (byte)(BASE64STRING[charIndex2]));
                    prevByte = (((int)src[i]) << 2) & 0x3f;
                    byteCount++;
                } else {
                    int charIndex3 = (prevByte | (((int)src[i]) >> 6)) & 0x3f;
                    int charIndex4 = ((int)src[i]) & 0x3f;
                    outStream.WriteByte(false, (byte)(BASE64STRING[charIndex3]));
                    outStream.WriteByte((i == length - 1), (byte)(BASE64STRING[charIndex4]));
                    byteCount = 0;
                }
            }
            if (byteCount == 1) {
                outStream.WriteByte(false, (byte)(BASE64STRING[prevByte]));
                outStream.WriteByte(false, (byte)'=');
                outStream.WriteByte(true,  (byte)'=');
            } else if (byteCount == 2) {
                outStream.WriteByte(false, (byte)(BASE64STRING[prevByte]));
                outStream.WriteByte(true,  (byte)'=');
            }
            dest = outStream.ToArray(); 
            return FileOperationStatus.Success;
        }

        //=========================================================================================
        // プロパティ：フィルターの表示名
        //=========================================================================================
        public string FilterName {
            get {
                return Resources.FileFilter_B64EncodeName;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターの説明文
        //=========================================================================================
        public string FilterExplain {
            get {
                return Resources.FileFilter_B64EncodeNameExp;
            }
        }

        //=========================================================================================
        // クラス：フォールディング機能付きのメモリストリーム
        //=========================================================================================
        private class FoldingMemoryStream {
            // フォールディングの桁数(0:フォールディングしない)
            private int m_foldingSize;

            // 先頭に空白を含むときtrue
            private bool m_headSpace;

            // 出力対象のストリーム
            private MemoryStream m_stream;

            // 1行に出力した文字数
            private int m_outputCount;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]foldingSize  フォールディングの桁数(0:フォールディングしない)
            // 　　　　[in]headSpace    先頭に空白を含むときtrue
            // 戻り値：なし
            //=========================================================================================
            public FoldingMemoryStream(int foldingSize, bool headSpace) {
                m_foldingSize = foldingSize;
                m_headSpace = headSpace;
                m_outputCount = 0;
                m_stream = new MemoryStream();
                if (m_headSpace) {
                    m_stream.WriteByte((byte)' ');
                    m_outputCount = 1;
                }
            }

            //=========================================================================================
            // 機　能：1バイト出力する
            // 引　数：[in]isLast   出力データの最終位置のときtrue
            // 　　　　[in]data     出力するデータ
            // 戻り値：なし
            //=========================================================================================
            public void WriteByte(bool isLast, byte data) {
                m_stream.WriteByte(data);
                m_outputCount++;
                if (isLast) {
                    return;
                }
                if (m_outputCount == m_foldingSize) {
                    m_stream.WriteByte(BYTE_CR);
                    m_stream.WriteByte(BYTE_LF);
                    if (m_headSpace) {
                        m_stream.WriteByte((byte)' ');
                        m_outputCount = 1;
                    } else {
                        m_outputCount = 0;
                    }
                }
            }

            //=========================================================================================
            // プロパティ：作成したバイト列
            //=========================================================================================
            public byte[] ToArray() {
                return m_stream.ToArray();
            }
        }
    }
}
