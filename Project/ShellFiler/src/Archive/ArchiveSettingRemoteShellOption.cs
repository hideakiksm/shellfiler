using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Document.OSSpec;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：リモートシェルでの圧縮のオプション
    //=========================================================================================
    public class ArchiveSettingRemoteShellOption : ICloneable {
        // 書庫の時刻を最新ファイルにあわせるときtrue
        private bool m_modifyTimestamp;

        // 圧縮レベル（0～9、サポートしないとき-1）
        private int m_compressionLevel;

        // 使用するコマンドラインの一部（ArchiveParameterで使用する場合以外はnull）
        private string m_commandLine;

        // コマンドラインの実行期待値
        private List<OSSpecLineExpect> m_commandExpect;

        //=========================================================================================
        // 機　能：コンストラクタ（推奨設定用）
        // 引　数：[in]timestamp  書庫の時刻を最新ファイルにあわせるときtrue
        // 　　　　[in]level      圧縮レベル
        // 戻り値：なし
        //=========================================================================================
        public ArchiveSettingRemoteShellOption(bool timestamp, int level) {
            m_modifyTimestamp = timestamp;
            m_compressionLevel = level;
            m_commandLine = null;
            m_commandExpect = null;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ArchiveSettingRemoteShellOption() {
            m_modifyTimestamp = false;
            m_compressionLevel = -1;
            m_commandLine = null;
            m_commandExpect = null;
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
        public static bool EqualsConfig(ArchiveSettingRemoteShellOption obj1, ArchiveSettingRemoteShellOption obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_modifyTimestamp != obj2.m_modifyTimestamp) {
                return false;
            }
            if (obj1.m_compressionLevel != obj2.m_compressionLevel) {
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
        public static bool LoadSetting(SettingLoader loader, out ArchiveSettingRemoteShellOption obj, RemoteShellArchiveFeature feature) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.ArchiveSettingRemote_ArchiveSettingRemote, SettingTagType.BeginObject, out fit);
            if (!success) {
                return success;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new ArchiveSettingRemoteShellOption();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return success;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.ArchiveSettingRemote_ArchiveSettingRemote) {
                    break;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.ArchiveSettingRemote_ModifyTimestamp) {
                    obj.m_modifyTimestamp = loader.BoolValue;
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.ArchiveSettingRemote_CompressionLevel) {
                    obj.m_compressionLevel = loader.IntValue;
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
        public static bool SaveSetting(SettingSaver saver, ArchiveSettingRemoteShellOption obj) {
            if (obj == null) {
                return true;
            }

            saver.StartObject(SettingTag.ArchiveSettingRemote_ArchiveSettingRemote);
            saver.AddBool(SettingTag.ArchiveSettingRemote_ModifyTimestamp, obj.m_modifyTimestamp);
            saver.AddInt(SettingTag.ArchiveSettingRemote_CompressionLevel, obj.m_compressionLevel);
            saver.EndObject(SettingTag.ArchiveSettingRemote_ArchiveSettingRemote);

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
        // プロパティ：使用するコマンドラインの一部（ArchiveParameterで使用する場合以外はnull）
        //=========================================================================================
        public string CommandLine {
            get {
                return m_commandLine;
            }
            set {
                m_commandLine = value;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドラインの実行期待値
        //=========================================================================================
        public List<OSSpecLineExpect> CommandExpect {
            get {
                return m_commandExpect;
            }
            set {
                m_commandExpect = value;
            }
        }
    }
}
