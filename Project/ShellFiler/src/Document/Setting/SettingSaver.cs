using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：設定の保存用クラス
    //=========================================================================================
    public class SettingSaver : SettingSaverBasic {
        // 暗号化ユーティリティ（初期化前はnull）
        private EncryptUtils m_encrypt = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileName   ファイル名
        // 戻り値：なし
        //=========================================================================================
        public SettingSaver(string fileName) : base(fileName) {
        }

        //=========================================================================================
        // 機　能：パスワードを出力する
        // 引　数：[in]tag    タグ
        // 　　　　[in]value  出力する値
        // 戻り値：なし
        //=========================================================================================
        public void AddPassword(SettingTag tag, string value) {
            if (value == null) {
                return;
            }
            if (m_encrypt == null) {
                int saltCode = SfHelperDecoder.GetPasswordSalt();
                m_encrypt = new EncryptUtils(saltCode);
            }
            value = m_encrypt.Encrypt(value);

            string encoded = EncodeString(value);
            m_outWriter.WriteLine(m_indent + "<p:" + tag.TagName + ">" + value);
        }

        //=========================================================================================
        // 機　能：色を出力する
        // 引　数：[in]tag    タグ
        // 　　　　[in]value  出力する値
        // 戻り値：なし
        //=========================================================================================
        public void AddColor(SettingTag tag, Color value) {
            if (value == null) {
                return;
            }

            string strValue;
            if (value == Color.Empty) {
                strValue = "";
            } else {
                int r = value.R;
                int g = value.G;
                int b = value.B;
                strValue = "RGB(" + r + "," + g + "," + b + ")";
            }
            m_outWriter.WriteLine(m_indent + "<c:" + tag.TagName + ">" + strValue);
        }

        //=========================================================================================
        // 機　能：領域を出力する
        // 引　数：[in]tag    タグ
        // 　　　　[in]value  出力する値
        // 戻り値：なし
        //=========================================================================================
        public void AddRectangle(SettingTag tag, Rectangle value) {
            string strValue;
            if (value == Rectangle.Empty) {
                strValue = "";
            } else {
                strValue = "Rectangle(" + value.Left + "," + value.Top + "," + value.Width + "," + value.Height + ")";
            }
            m_outWriter.WriteLine(m_indent + "<r:" + tag.TagName + ">" + strValue);
        }

        //=========================================================================================
        // 機　能：BooleanFlag値を出力する
        // 引　数：[in]tag    タグ
        // 　　　　[in]value  出力する値
        // 戻り値：なし
        //=========================================================================================
        public void AddBooleanFlag(SettingTag tag, BooleanFlag value) {
            if (value != null) {
                return;
            }
            string encoded = value.Value.ToString();
            m_outWriter.WriteLine(m_indent + "<b:" + tag.TagName + ">" + encoded);
        }
    }
}
