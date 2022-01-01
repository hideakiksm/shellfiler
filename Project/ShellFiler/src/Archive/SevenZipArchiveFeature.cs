using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using Nomad.Archive.SevenZip;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zの圧縮ライブラリが持っている機能
    //=========================================================================================
    public class SevenZipArchiveFeature {
        // 圧縮方法の選択子（設定できないときnull）
        private string[] m_supportMethod;

        // 圧縮率の指定ができるときtrue
        private bool m_supportCompressionLevel;

        // 暗号化に対応しているときtrue
        private bool m_supportEncryption;

        // 暗号化方式の選択子（設定できないときnull）
        private string[] m_supportEncryptionMethod;

        // 推奨設定
        private ArchiveSettingLocal7zOption m_recommendedSetting;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]supportMethod            圧縮方法の指定ができるときtrue
        // 　　　　[in]supportCompressionLevel  圧縮率の指定ができるときtrue
        // 　　　　[in]supportEncryption        暗号化に対応しているときtrue
        // 　　　　[in]supportEncryptionMethod  暗号化方式を指定できるときtrue
        // 　　　　[in]recommendedSetting       推奨設定
        // 戻り値：なし
        //=========================================================================================
        public SevenZipArchiveFeature(string[] supportMethod, bool supportCompressionLevel, bool supportEncryption, string[] supportEncryptionMethod, ArchiveSettingLocal7zOption recommendedSetting) {
            m_supportMethod = supportMethod;
            m_supportCompressionLevel = supportCompressionLevel;
            m_supportEncryption = supportEncryption;
            m_supportEncryptionMethod = supportEncryptionMethod;
            m_recommendedSetting = recommendedSetting;
        }

        //=========================================================================================
        // 機　能：圧縮形式に対応した機能オブジェクトを返す
        // 引　数：[in]format  圧縮形式
        // 戻り値：機能の有無
        //=========================================================================================
        public static SevenZipArchiveFeature GetFeature(ArchiveType format) {
            if (format == ArchiveType.SevenZip) {
                string[] supportMethod = null;
                bool supportCompressionLevel = true;
                bool supportEncryption = true;
                string[] supportEncryptionMethod = null;
                ArchiveSettingLocal7zOption recommendedSetting = new ArchiveSettingLocal7zOption(null, 9, null);
                return new SevenZipArchiveFeature(supportMethod, supportCompressionLevel, supportEncryption, supportEncryptionMethod, recommendedSetting);
            } else if (format == ArchiveType.Zip) {
                string[] supportMethod = ArchiveSettingLocal7zOption.ZipCompressionMethods;
                bool supportCompressionLevel = true;
                bool supportEncryption = true;
                string[] supportEncryptionMethod = ArchiveSettingLocal7zOption.ZipEncryptionMethods;
                ArchiveSettingLocal7zOption recommendedSetting = new ArchiveSettingLocal7zOption(ArchiveSettingLocal7zOption.DEFAULT_METHOD_ZIP, 9, ArchiveSettingLocal7zOption.DEFAULT_ENC_METHOD_ZIP);
                return new SevenZipArchiveFeature(supportMethod, supportCompressionLevel, supportEncryption, supportEncryptionMethod, recommendedSetting);
            } else if (format == ArchiveType.TarGz) {
                string[] supportMethod = null;
                bool supportCompressionLevel = true;
                bool supportEncryption = false;
                string[] supportEncryptionMethod = null;
                ArchiveSettingLocal7zOption recommendedSetting = new ArchiveSettingLocal7zOption(null, 9, null);
                return new SevenZipArchiveFeature(supportMethod, supportCompressionLevel, supportEncryption, supportEncryptionMethod, recommendedSetting);
            } else if (format == ArchiveType.TarBz2) {
                string[] supportMethod = null;
                bool supportCompressionLevel = true;
                bool supportEncryption = false;
                string[] supportEncryptionMethod = null;
                ArchiveSettingLocal7zOption recommendedSetting = new ArchiveSettingLocal7zOption(null, 9, null);
                return new SevenZipArchiveFeature(supportMethod, supportCompressionLevel, supportEncryption, supportEncryptionMethod, recommendedSetting);
            } else if (format == ArchiveType.Tar) {
                string[] supportMethod = null;
                bool supportCompressionLevel = false;
                bool supportEncryption = false;
                string[] supportEncryptionMethod = null;
                ArchiveSettingLocal7zOption recommendedSetting = new ArchiveSettingLocal7zOption(null, -1, null);
                return new SevenZipArchiveFeature(supportMethod, supportCompressionLevel, supportEncryption, supportEncryptionMethod, recommendedSetting);
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：圧縮レベルを補正する
        // 引　数：[in]level   補正前の値
        // 戻り値：補正後の値
        //=========================================================================================
        public static int ModifyCompressionLevel(int level) {
            if (level < 0) {
                return 0;
            } else if (level > 9) {
                return 9;
            } else {
                return level;
            }
        }

        //=========================================================================================
        // プロパティ：圧縮方法の選択子（設定できないときnull）
        //=========================================================================================
        public string[] SupportMethod {
            get {
                return m_supportMethod;
            }
        }

        //=========================================================================================
        // プロパティ：圧縮率の指定ができるときtrue
        //=========================================================================================
        public bool SupportCompressionLevel {
            get {
                return m_supportCompressionLevel;
            }
        }

        //=========================================================================================
        // プロパティ：暗号化に対応しているときtrue
        //=========================================================================================
        public bool SupportEncryption {
            get {
                return m_supportEncryption;
            }
        }

        //=========================================================================================
        // プロパティ：暗号化方式の選択子（設定できないときnull）
        //=========================================================================================
        public string[] SupportEncryptionMethod {
            get {
                return m_supportEncryptionMethod;
            }
        }

        //=========================================================================================
        // プロパティ：推奨設定
        //=========================================================================================
        public ArchiveSettingLocal7zOption RecommendedSetting {
            get {
                return m_recommendedSetting;
            }
        }
    }
}
