using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：暗号化ユーティリティ
    //=========================================================================================
    public class EncryptUtils {
        // パスワード
        private static readonly byte[] PASSWORD_BYTES1 = {
            0x24, 0x57, 0x69, 0x6E, 0x53, 0x68, 0x65, 0x6C, 0x6C, 0x46, 0x69, 0x6C, 0x65, 0x72, 0x24, 0x50
        };

        // パスワードの暗号化マスク
        private static readonly byte[] PASSWORD_BYTES2 = {
            0x28, 0x00, 0x26, 0x00, 0x4B, 0x00, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x50
        };

        // ソルトに使用する値
        private static readonly byte[] SALT_BYTES = {
            0x10, 0x04, 0xFF, 0xFF, 0x80, 0x00, 0xD6, 0x53, 0x8A, 0x30, 0x88, 0x6D, 0x57, 0x30, 0xDA, 0x7D
        };

        // 暗号化のマネージャ
        private static RijndaelManaged m_encryptManager;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]saltCode  外部指定のソルト
        // 戻り値：なし
        //=========================================================================================
        public EncryptUtils(int saltCode) {
            byte[] passData = new byte[PASSWORD_BYTES1.Length];
            byte[] saltData = new byte[PASSWORD_BYTES1.Length];
            for (int i = 0; i < PASSWORD_BYTES1.Length; i++) {
                passData[i] = (byte)(PASSWORD_BYTES1[i] ^ PASSWORD_BYTES2[i]);
                saltData[i] = (byte)(SALT_BYTES[i] ^ PASSWORD_BYTES2[i] ^ saltCode);
                saltCode *= 3;
            }

            RijndaelManaged rijndael = new RijndaelManaged();
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Convert.ToBase64String(passData), saltData);
            deriveBytes.IterationCount = 1000;
            rijndael.Key = deriveBytes.GetBytes(rijndael.KeySize / 8);
            rijndael.IV = deriveBytes.GetBytes(rijndael.BlockSize / 8);
            m_encryptManager = rijndael;
        }

        //=========================================================================================
        // 機　能：文字列を暗号化する
        // 引　数：[in]original  暗号化する文字列
        // 戻り値：暗号化された文字列
        //=========================================================================================
        public string Encrypt(string original) {
            byte[] byOriginal = Encoding.UTF8.GetBytes(original);
            ICryptoTransform encryptor = m_encryptManager.CreateEncryptor();
            byte[] byEncrypted = encryptor.TransformFinalBlock(byOriginal, 0, byOriginal.Length);
            encryptor.Dispose();
            return Convert.ToBase64String(byEncrypted);
        }

        //=========================================================================================
        // 機　能：バイト列を暗号化する
        // 引　数：[in]byOriginal  暗号化するバイト列
        // 戻り値：暗号化された文字列
        //=========================================================================================
        public byte[] EncryptBytes(byte[] byOriginal) {
            ICryptoTransform encryptor = m_encryptManager.CreateEncryptor();
            byte[] byEncrypted = encryptor.TransformFinalBlock(byOriginal, 0, byOriginal.Length);
            encryptor.Dispose();
            return byEncrypted;
        }

        //=========================================================================================
        // 機　能：暗号化された文字列を復号化する
        // 引　数：[in]encrypted  暗号化された文字列
        // 戻り値：復号化された文字列（失敗したときnull）
        //=========================================================================================
        public string Decrypt(string encrypted) {
            try {
                byte[] byEncrypted = Convert.FromBase64String(encrypted);
                ICryptoTransform decryptor = m_encryptManager.CreateDecryptor();
                byte[] byOriginal = decryptor.TransformFinalBlock(byEncrypted, 0, byEncrypted.Length);
                decryptor.Dispose();
                return Encoding.UTF8.GetString(byOriginal);
            } catch (Exception) {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：暗号化されたバイト列を復号化する
        // 引　数：[in]byEncrypted  暗号化されたバイト列
        // 戻り値：復号化されたバイト列（失敗したときnull）
        //=========================================================================================
        public byte[] DecryptBytes(byte[] byEncrypted) {
            try {
                ICryptoTransform decryptor = m_encryptManager.CreateDecryptor();
                byte[] byOriginal = decryptor.TransformFinalBlock(byEncrypted, 0, byEncrypted.Length);
                decryptor.Dispose();
                return byOriginal;
            } catch (Exception) {
                return null;
            }
        }
    }
}
