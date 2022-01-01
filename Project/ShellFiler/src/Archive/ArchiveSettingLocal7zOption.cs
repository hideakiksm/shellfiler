using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Util;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：ローカルでの7z.dll圧縮のオプション
    //=========================================================================================
    public class ArchiveSettingLocal7zOption : ICloneable {
        // ZIPの設定
        public const string DEFAULT_METHOD_ZIP = METHOD_ZIP_DEFLATE;
        public const string DEFAULT_ENC_METHOD_ZIP = ENC_METHOD_ZIP_AES256;
        public const string METHOD_ZIP_DEFLATE = "DEFLATE";
        public const string METHOD_ZIP_DEFLATE64 = "DEFLATE64";
        public const string METHOD_ZIP_BZIP2 = "BZIP2";
        public const string METHOD_ZIP_LZMA = "LZMA";
        public const string METHOD_ZIP_PPMD = "PPMD";
        public const string ENC_METHOD_ZIP_AES256 = "AES256";
        public const string ENC_METHOD_ZIP_ZIPCRYPTO = "ZIPCRYPTO";

        // 書庫の時刻を最新ファイルにあわせるときtrue
        private bool m_modifyTimestamp;

        // 圧縮方法（サポートしないときnull）
        private string m_compressionMethod;

        // 圧縮レベル（0～9、サポートしないとき-1）
        private int m_compressionLevel;

        // 暗号化するときtrue
        private bool m_encryption;

        // 暗号化方式（サポートしないときnull）
        private string m_encryptionMethod;

        // 暗号化に使用するパスワード（暗号化しないときnull）
        private ArchiveAutoPasswordItem m_passwordItem;
        
        //=========================================================================================
        // 機　能：コンストラクタ（推奨設定用）
        // 引　数：[in]method     圧縮方法
        // 　　　　[in]level      圧縮レベル
        // 　　　　[in]encMethod  暗号化方式
        // 戻り値：なし
        //=========================================================================================
        public ArchiveSettingLocal7zOption(string method, int level, string encMethod) {
            m_modifyTimestamp = true;
            m_compressionMethod = method;
            m_compressionLevel = level;
            m_encryption = false;
            m_encryptionMethod = encMethod;
            m_passwordItem = null;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ArchiveSettingLocal7zOption() {
            m_compressionMethod = DEFAULT_METHOD_ZIP;
            m_modifyTimestamp = true;
            m_compressionLevel = 9;
            m_encryption = false;
            m_encryptionMethod = DEFAULT_ENC_METHOD_ZIP;
            m_passwordItem = null;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            return MemberwiseClone();
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(ArchiveSettingLocal7zOption obj1, ArchiveSettingLocal7zOption obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_modifyTimestamp != obj2.m_modifyTimestamp) {
                return false;
            }
            if (obj1.m_compressionMethod != obj2.m_compressionMethod) {
                return false;
            }
            if (obj1.m_compressionLevel != obj2.m_compressionLevel) {
                return false;
            }
            if (obj1.m_encryptionMethod != obj2.m_encryptionMethod) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 　　　　[in]feature 圧縮フォーマットが提供する機能
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out ArchiveSettingLocal7zOption obj, SevenZipArchiveFeature feature) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.ArchiveSettingLocal7z_ArchiveSettingLocal7z, SettingTagType.BeginObject, out fit);
            if (!success) {
                return success;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new ArchiveSettingLocal7zOption();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return success;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.ArchiveSettingLocal7z_ArchiveSettingLocal7z) {
                    break;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.ArchiveSettingLocal7z_ModifyTimestamp) {
                    obj.m_modifyTimestamp = loader.BoolValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.ArchiveSettingLocal7z_CompressionMethod) {
                    if (feature.SupportMethod == null) {
                        obj.m_compressionMethod = null;
                    } else {
                        int indexMethod = StringUtils.GetStringArrayIndex(feature.SupportMethod, loader.StringValue);
                        obj.m_compressionMethod = feature.SupportMethod[indexMethod];
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.ArchiveSettingLocal7z_CompressionLevel) {
                    obj.m_compressionLevel = loader.IntValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.ArchiveSettingLocal7z_EncryptionMethod) {
                    if (feature.SupportEncryptionMethod == null) {
                        obj.m_encryptionMethod = null;
                    } else {
                        int indexEncrypt = StringUtils.GetStringArrayIndex(feature.SupportEncryptionMethod, loader.StringValue);
                        obj.m_encryptionMethod = feature.SupportEncryptionMethod[indexEncrypt];
                    }
                }
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, ArchiveSettingLocal7zOption obj) {
            if (obj == null) {
                return true;
            }

            saver.StartObject(SettingTag.ArchiveSettingLocal7z_ArchiveSettingLocal7z);
            saver.AddBool(SettingTag.ArchiveSettingLocal7z_ModifyTimestamp, obj.m_modifyTimestamp);
            saver.AddString(SettingTag.ArchiveSettingLocal7z_CompressionMethod, obj.m_compressionMethod);
            saver.AddInt(SettingTag.ArchiveSettingLocal7z_CompressionLevel, obj.m_compressionLevel);
            saver.AddString(SettingTag.ArchiveSettingLocal7z_EncryptionMethod, obj.m_encryptionMethod);
            saver.EndObject(SettingTag.ArchiveSettingLocal7z_ArchiveSettingLocal7z);

            return true;
        }

        //=========================================================================================
        // プロパティ：書庫の時刻を最新ファイルにあわせるときtrue
        //=========================================================================================
        public bool ModifyTimestamp {
            get {
                return m_modifyTimestamp;
            }
            set {
                m_modifyTimestamp = value;
            }
        }

        //=========================================================================================
        // プロパティ：圧縮方法（サポートしないときnull）
        //=========================================================================================
        public string CompressionMethod {
            get {
                return m_compressionMethod;
            }
            set {
                m_compressionMethod = value;
            }
        }

        //=========================================================================================
        // プロパティ：圧縮レベル（0～9、サポートしないとき-1）
        //=========================================================================================
        public int CompressionLevel {
            get {
                return m_compressionLevel;
            }
            set {
                m_compressionLevel = value;
            }
        }

        //=========================================================================================
        // プロパティ：暗号化するときtrue
        //=========================================================================================
        public bool Encryption {
            get {
                return m_encryption;
            }
            set {
                m_encryption = value;
            }
        }

        //=========================================================================================
        // プロパティ：暗号化方式（サポートしないときnull）
        //=========================================================================================
        public string EncryptionMethod {
            get {
                return m_encryptionMethod;
            }
            set {
                m_encryptionMethod = value;
            }
        }

        //=========================================================================================
        // プロパティ：暗号化に使用するパスワード（暗号化しないときnull）
        //=========================================================================================
        public ArchiveAutoPasswordItem PasswordItem {
            get {
                return m_passwordItem;
            }
            set {
                m_passwordItem = value;
            }
        }

        //=========================================================================================
        // プロパティ：Zipで設定可能な圧縮方式
        //=========================================================================================
        public static string[] ZipCompressionMethods {
            get {
                string[] methods = new string[] {
                        METHOD_ZIP_DEFLATE,
                        METHOD_ZIP_DEFLATE64,
                        METHOD_ZIP_BZIP2,
                        METHOD_ZIP_LZMA,
                        METHOD_ZIP_PPMD,
                };
                return methods;
            }
        }

        //=========================================================================================
        // プロパティ：Zipで設定可能な暗号化方式
        //=========================================================================================
        public static string[] ZipEncryptionMethods {
            get {
                string[] methods = new string[] {
                    ENC_METHOD_ZIP_AES256,
                    ENC_METHOD_ZIP_ZIPCRYPTO,
                };
                return methods;
            }
        }
    }
}
