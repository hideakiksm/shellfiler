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
    // クラス：リモートシェルの書庫操作コマンドが持っている機能
    //=========================================================================================
    public class RemoteShellArchiveFeature {
        // 書庫タイムスタンプの設定に対応しているときtrue
        private bool m_supportTimestamp;

        // 圧縮率の指定ができるときtrue
        private bool m_supportCompressionLevel;

        // 推奨設定が有効なときtrue
        private bool m_supportRecommend;

        // 推奨設定
        private ArchiveSettingRemoteShellOption m_recommendedSetting;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]supportTimestamp         書庫タイムスタンプの設定に対応しているときtrue
        // 　　　　[in]supportCompressionLevel  圧縮率の指定ができるときtrue
        // 　　　　[in]supportRecommend         推奨設定が有効なときtrue
        // 　　　　[in]recommendedSetting       推奨設定
        // 戻り値：なし
        //=========================================================================================
        public RemoteShellArchiveFeature(bool supportTimestamp, bool supportCompressionLevel, bool supportRecommend, ArchiveSettingRemoteShellOption recommendedSetting) {
            m_supportTimestamp = supportTimestamp;
            m_supportCompressionLevel = supportCompressionLevel;
            m_supportRecommend = supportRecommend;
            m_recommendedSetting = recommendedSetting;
        }

        //=========================================================================================
        // 機　能：圧縮形式に対応した機能オブジェクトを返す
        // 引　数：[in]format  圧縮形式
        // 戻り値：機能の有無
        //=========================================================================================
        public static RemoteShellArchiveFeature GetFeature(ArchiveType format) {
            if (format == ArchiveType.Zip) {
                ArchiveSettingRemoteShellOption recommendedSetting = new ArchiveSettingRemoteShellOption(false, 6);
                return new RemoteShellArchiveFeature(true, true, true, recommendedSetting);
            } else if (format == ArchiveType.TarGz) {
                ArchiveSettingRemoteShellOption recommendedSetting = new ArchiveSettingRemoteShellOption(false, -1);
                return new RemoteShellArchiveFeature(false, false, false, recommendedSetting);
            } else if (format == ArchiveType.TarBz2) {
                ArchiveSettingRemoteShellOption recommendedSetting = new ArchiveSettingRemoteShellOption(false, -1);
                return new RemoteShellArchiveFeature(false, false, false, recommendedSetting);
            } else if (format == ArchiveType.Tar) {
                ArchiveSettingRemoteShellOption recommendedSetting = new ArchiveSettingRemoteShellOption(false, -1);
                return new RemoteShellArchiveFeature(false, false, false, recommendedSetting);
            } else {
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：書庫タイムスタンプの設定に対応しているときtrue
        //=========================================================================================
        public bool SupportTimestamp {
            get {
                return m_supportTimestamp;
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
        // プロパティ：推奨設定が有効なときtrue
        //=========================================================================================
        public bool SupportRecommend {
            get {
                return m_supportRecommend;
            }
        }

        //=========================================================================================
        // プロパティ：推奨設定
        //=========================================================================================
        public ArchiveSettingRemoteShellOption RecommendedSetting {
            get {
                return m_recommendedSetting;
            }
        }
    }
}
