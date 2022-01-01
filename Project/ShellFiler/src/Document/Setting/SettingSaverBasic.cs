using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：設定の保存用クラス
    //=========================================================================================
    public class SettingSaverBasic {
        // ファイル名
        private string m_fileName;

        // 出力内容のバッファ
        private MemoryStream m_outBuffer;
        
        // 出力時に使用するWriter
        protected StreamWriter m_outWriter;

        // インデント
        protected string m_indent;

        // 書き込み済みファイルの書き込み日時
        private DateTime m_lastFileWriteTime = DateTime.MinValue;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SettingSaverBasic(string fileName) {
            m_fileName = fileName;
            m_outBuffer = new MemoryStream();
            m_outWriter = new StreamWriter(m_outBuffer, Encoding.UTF8);
            m_indent = "";

            FileVersionInfo ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            int version = ver.FileMajorPart * 1000000 + ver.FileMinorPart * 10000 + ver.FileBuildPart * 100 + ver.FilePrivatePart;
            m_outWriter.WriteLine(SettingTag.Header.TagName + ":" + version);
        }

        //=========================================================================================
        // 機　能：文字列を出力する
        // 引　数：[in]tag    タグ
        // 　　　　[in]value  出力する値
        // 戻り値：なし
        //=========================================================================================
        public void AddString(SettingTag tag, string value) {
            if (value == null) {
                return;
            }
            value = EncodeString(value);
            m_outWriter.WriteLine(m_indent + "<s:" + tag.TagName + ">" + value);
        }

        //=========================================================================================
        // 機　能：数値を出力する
        // 引　数：[in]tag    タグ
        // 　　　　[in]value  出力する値
        // 戻り値：なし
        //=========================================================================================
        public void AddInt(SettingTag tag, int value) {
            string encoded = value.ToString();
            m_outWriter.WriteLine(m_indent + "<i:" + tag.TagName + ">" + encoded);
        }

        //=========================================================================================
        // 機　能：実数の値を出力する
        // 引　数：[in]tag    タグ
        // 　　　　[in]value  出力する値
        // 戻り値：なし
        //=========================================================================================
        public void AddFloat(SettingTag tag, float value) {
            string encoded = value.ToString();
            m_outWriter.WriteLine(m_indent + "<f:" + tag.TagName + ">" + encoded);
        }

        //=========================================================================================
        // 機　能：Bool値を出力する
        // 引　数：[in]tag    タグ
        // 　　　　[in]value  出力する値
        // 戻り値：なし
        //=========================================================================================
        public void AddBool(SettingTag tag, bool value) {
            string encoded = value.ToString();
            m_outWriter.WriteLine(m_indent + "<b:" + tag.TagName + ">" + encoded);
        }

        //=========================================================================================
        // 機　能：Long値を出力する
        // 引　数：[in]tag    タグ
        // 　　　　[in]value  出力する値
        // 戻り値：なし
        //=========================================================================================
        public void AddLong(SettingTag tag, long value) {
            string encoded = value.ToString();
            m_outWriter.WriteLine(m_indent + "<l:" + tag.TagName + ">" + encoded);
        }

        //=========================================================================================
        // 機　能：オブジェクトを開始する
        // 引　数：[in]tag    タグ
        // 戻り値：なし
        //=========================================================================================
        public void StartObject(SettingTag tag) {
            m_outWriter.WriteLine(m_indent + "<" + tag.TagName + ">");
            m_indent += "  ";
        }

        //=========================================================================================
        // 機　能：オブジェクトを終了する
        // 引　数：[in]tag    タグ
        // 戻り値：なし
        //=========================================================================================
        public void EndObject(SettingTag tag) {
            m_indent = m_indent.Substring(2);
            m_outWriter.WriteLine(m_indent + "</" + tag.TagName + ">");
        }

        //=========================================================================================
        // 機　能：文字列を出力形式にエンコードする
        // 引　数：[in]org  エンコード前の文字列
        // 戻り値：エンコード後の文字列
        //=========================================================================================
        protected string EncodeString(string org) {
            StringBuilder result = new StringBuilder();
            char[] chArray = org.ToCharArray();
            for (int i = 0; i < chArray.Length; i++) {
                char ch = chArray[i];
                if (ch == '\\') {
                    result.Append("\\\\");
                } else if (ch == '\r') {
                    result.Append("\\r");
                } else if (ch == '\n') {
                    result.Append("\\n");
                } else if (ch == '\x00') {
                    result.Append("\\a");
                } else {
                    result.Append(ch);
                }
            }
            return result.ToString();
        }

        //=========================================================================================
        // 機　能：設定を出力する
        // 引　数：[in]encrypt   暗号化するときtrue
        // 戻り値：出力に成功したときtrue
        //=========================================================================================
        public bool SaveSetting(bool encrypt) {
            m_outWriter.Close();
            m_outBuffer.Close();

            if (m_indent != "") {
                Program.Abort("保存データの階層が一致しません。");
            }

            byte[] data = m_outBuffer.ToArray();
            if (encrypt) {
                EncryptUtils encryptUtils = new EncryptUtils(0);
                data = encryptUtils.EncryptBytes(data);
            }

            try {
                FileStream stream = new FileStream(m_fileName, FileMode.Create, FileAccess.Write);
                try {
                    stream.Write(data, 0, data.Length);
                } finally {
                    stream.Close();
                }
                FileInfo fileInfo = new FileInfo(m_fileName);
                m_lastFileWriteTime = fileInfo.LastWriteTime;
            } catch (Exception) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：書き込み済みファイルの書き込み日時
        //=========================================================================================
        public DateTime LastFileWriteTime {
            get {
                return m_lastFileWriteTime;
            }
        }
    }
}
