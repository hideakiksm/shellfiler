using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document;
using ShellFiler.Document.Setting;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：ファイル圧縮のオプション
    //=========================================================================================
    public class ArchiveSetting : ICloneable {
        // 選択された圧縮実行方式
        private ArchiveExecuteMethod m_executeMethod;

        // 圧縮ファイルの形式のデフォルト
        private ArchiveType m_archiveType;

        // sevenzip.dllでの7z圧縮オプション
        private ArchiveSettingLocal7zOption m_local7z7zOption;

        // sevenzip.dllでのzip圧縮オプション
        private ArchiveSettingLocal7zOption m_local7zZipOption;

        // sevenzip.dllでのtar.gz圧縮オプション
        private ArchiveSettingLocal7zOption m_local7zTarGzOption;

        // sevenzip.dllでのtar.bz2圧縮オプション
        private ArchiveSettingLocal7zOption m_local7zTarBz2Option;

        // sevenzip.dllでのtar圧縮オプション
        private ArchiveSettingLocal7zOption m_local7zTarOption;

        // リモートシェルでのzip圧縮オプション
        private ArchiveSettingRemoteShellOption m_remoteZipOption;

        // リモートシェルでのtar.gz圧縮オプション
        private ArchiveSettingRemoteShellOption m_remoteTarGzOption;

        // リモートシェルでのtar.bz2圧縮オプション
        private ArchiveSettingRemoteShellOption m_remoteTarBz2Option;

        // リモートシェルでのtar圧縮オプション
        private ArchiveSettingRemoteShellOption m_remoteTarOption;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]method         選択された圧縮実行方式
        // 　　　　[in]sevenZipOption 7z圧縮オプション
        // 戻り値：なし
        //=========================================================================================
        public ArchiveSetting() {
            m_executeMethod = ArchiveExecuteMethod.Local7z;
            m_archiveType = ArchiveType.Zip;
            m_local7z7zOption = SevenZipArchiveFeature.GetFeature(ArchiveType.SevenZip).RecommendedSetting;
            m_local7zZipOption = SevenZipArchiveFeature.GetFeature(ArchiveType.Zip).RecommendedSetting;
            m_local7zTarGzOption = SevenZipArchiveFeature.GetFeature(ArchiveType.TarGz).RecommendedSetting;
            m_local7zTarBz2Option = SevenZipArchiveFeature.GetFeature(ArchiveType.TarBz2).RecommendedSetting;
            m_local7zTarOption = SevenZipArchiveFeature.GetFeature(ArchiveType.Tar).RecommendedSetting;
            m_remoteZipOption = RemoteShellArchiveFeature.GetFeature(ArchiveType.Zip).RecommendedSetting;
            m_remoteTarGzOption = RemoteShellArchiveFeature.GetFeature(ArchiveType.TarGz).RecommendedSetting;
            m_remoteTarBz2Option = RemoteShellArchiveFeature.GetFeature(ArchiveType.TarBz2).RecommendedSetting;
            m_remoteTarOption = RemoteShellArchiveFeature.GetFeature(ArchiveType.Tar).RecommendedSetting;
        }
        
        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            ArchiveSetting obj = new ArchiveSetting();
            obj.m_executeMethod = m_executeMethod;
            obj.m_archiveType = m_archiveType;
            obj.m_local7z7zOption = (ArchiveSettingLocal7zOption)(m_local7z7zOption.Clone());
            obj.m_local7zZipOption = (ArchiveSettingLocal7zOption)(m_local7zZipOption.Clone());
            obj.m_local7zTarGzOption = (ArchiveSettingLocal7zOption)(m_local7zTarGzOption.Clone());
            obj.m_local7zTarBz2Option = (ArchiveSettingLocal7zOption)(m_local7zTarBz2Option.Clone());
            obj.m_local7zTarOption = (ArchiveSettingLocal7zOption)(m_local7zTarOption.Clone());
            obj.m_remoteZipOption = (ArchiveSettingRemoteShellOption)(m_remoteZipOption.Clone());
            obj.m_remoteTarGzOption = (ArchiveSettingRemoteShellOption)(m_remoteTarGzOption.Clone());
            obj.m_remoteTarBz2Option = (ArchiveSettingRemoteShellOption)(m_remoteTarBz2Option.Clone());
            obj.m_remoteTarOption = (ArchiveSettingRemoteShellOption)(m_remoteTarOption.Clone());

            return obj;
        }
        
        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(ArchiveSetting obj1, ArchiveSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_executeMethod != obj2.m_executeMethod) {
                return false;
            }
            if (obj1.m_archiveType != obj2.m_archiveType) {
                return false;
            }
            if (!ArchiveSettingLocal7zOption.EqualsConfig(obj1.m_local7z7zOption, obj2.m_local7z7zOption)) {
                return false;
            }
            if (!ArchiveSettingLocal7zOption.EqualsConfig(obj1.m_local7zZipOption, obj2.m_local7zZipOption)) {
                return false;
            }
            if (!ArchiveSettingLocal7zOption.EqualsConfig(obj1.m_local7zTarGzOption, obj2.m_local7zTarGzOption)) {
                return false;
            }
            if (!ArchiveSettingLocal7zOption.EqualsConfig(obj1.m_local7zTarBz2Option, obj2.m_local7zTarBz2Option)) {
                return false;
            }
            if (!ArchiveSettingLocal7zOption.EqualsConfig(obj1.m_local7zTarOption, obj2.m_local7zTarOption)) {
                return false;
            }
            if (!ArchiveSettingRemoteShellOption.EqualsConfig(obj1.m_remoteZipOption, obj2.m_remoteZipOption)) {
                return false;
            }
            if (!ArchiveSettingRemoteShellOption.EqualsConfig(obj1.m_remoteTarGzOption, obj2.m_remoteTarGzOption)) {
                return false;
            }
            if (!ArchiveSettingRemoteShellOption.EqualsConfig(obj1.m_remoteTarBz2Option, obj2.m_remoteTarBz2Option)) {
                return false;
            }
            if (!ArchiveSettingRemoteShellOption.EqualsConfig(obj1.m_remoteTarOption, obj2.m_remoteTarOption)) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out ArchiveSetting obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.ArchiveSetting_ArchiveSetting, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();

            SevenZipArchiveFeature featureLZip = SevenZipArchiveFeature.GetFeature(ArchiveType.Zip);
            SevenZipArchiveFeature featureL7Z = SevenZipArchiveFeature.GetFeature(ArchiveType.SevenZip);
            SevenZipArchiveFeature featureLTarGz = SevenZipArchiveFeature.GetFeature(ArchiveType.TarGz);
            SevenZipArchiveFeature featureLTarBz2 = SevenZipArchiveFeature.GetFeature(ArchiveType.TarBz2);
            SevenZipArchiveFeature featureLTar = SevenZipArchiveFeature.GetFeature(ArchiveType.Tar);
            RemoteShellArchiveFeature featureRZip = RemoteShellArchiveFeature.GetFeature(ArchiveType.Zip);
            RemoteShellArchiveFeature featureRTarGz = RemoteShellArchiveFeature.GetFeature(ArchiveType.TarGz);
            RemoteShellArchiveFeature featureRTarBz2 = RemoteShellArchiveFeature.GetFeature(ArchiveType.TarBz2);
            RemoteShellArchiveFeature featureRTar = RemoteShellArchiveFeature.GetFeature(ArchiveType.Tar);

            obj = new ArchiveSetting();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.ArchiveSetting_ArchiveSetting) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.ArchiveSetting_ExecuteMethod) {
                    string stringValue = loader.StringValue;
                    if (stringValue == "Local7z") {
                        obj.m_executeMethod = ArchiveExecuteMethod.Local7z;
                    } else if (stringValue == "RemoteShell") {
                        obj.m_executeMethod = ArchiveExecuteMethod.RemoteShell;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.ArchiveSetting_ArchiveType) {
                    string stringValue = loader.StringValue;
                    if (stringValue == "ZIP") {
                        obj.m_archiveType = ArchiveType.Zip;
                    } else if (stringValue == "SEVENZIP") {
                        obj.m_archiveType = ArchiveType.SevenZip;
                    } else if (stringValue == "TARGZ") {
                        obj.m_archiveType = ArchiveType.TarGz;
                    } else if (stringValue == "TARBZ2") {
                        obj.m_archiveType = ArchiveType.TarBz2;
                    } else if (stringValue == "TAR") {
                        obj.m_archiveType = ArchiveType.Tar;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ArchiveSetting_Local7zZipOption) {
                    success = ArchiveSettingLocal7zOption.LoadSetting(loader, out obj.m_local7zZipOption, featureLZip);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.ArchiveSetting_Local7zZipOption, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ArchiveSetting_Local7z7zOption) {
                    success = ArchiveSettingLocal7zOption.LoadSetting(loader, out obj.m_local7z7zOption, featureL7Z);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.ArchiveSetting_Local7z7zOption, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ArchiveSetting_Local7zTarGzOption) {
                    success = ArchiveSettingLocal7zOption.LoadSetting(loader, out obj.m_local7zTarGzOption, featureLTarGz);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.ArchiveSetting_Local7zTarGzOption, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ArchiveSetting_Local7zTarBz2Option) {
                    success = ArchiveSettingLocal7zOption.LoadSetting(loader, out obj.m_local7zTarBz2Option, featureLTarBz2);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.ArchiveSetting_Local7zTarBz2Option, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ArchiveSetting_Local7zTarOption) {
                    success = ArchiveSettingLocal7zOption.LoadSetting(loader, out obj.m_local7zTarOption, featureLTar);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.ArchiveSetting_Local7zTarOption, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ArchiveSetting_RemoteZipOption) {
                    success = ArchiveSettingRemoteShellOption.LoadSetting(loader, out obj.m_remoteZipOption, featureRZip);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.ArchiveSetting_RemoteZipOption, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ArchiveSetting_RemoteTarGzOption) {
                    success = ArchiveSettingRemoteShellOption.LoadSetting(loader, out obj.m_remoteTarGzOption, featureRTarGz);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.ArchiveSetting_RemoteTarGzOption, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ArchiveSetting_RemoteTarBz2Option) {
                    success = ArchiveSettingRemoteShellOption.LoadSetting(loader, out obj.m_remoteTarBz2Option, featureRTarBz2);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.ArchiveSetting_RemoteTarBz2Option, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.ArchiveSetting_RemoteTarOption) {
                    success = ArchiveSettingRemoteShellOption.LoadSetting(loader, out obj.m_remoteTarOption, featureRTar);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.ArchiveSetting_RemoteTarOption, SettingTagType.EndObject);
                    if (!success) {
                        return false;
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
        public static bool SaveSetting(SettingSaver saver, ArchiveSetting obj) {
            if (obj == null) {
                return true;
            }

            bool success;
            string strExecuteMethod = null;
            if (obj.m_executeMethod == ArchiveExecuteMethod.Local7z) {
                strExecuteMethod = "Local7z";
            } else if (obj.m_executeMethod == ArchiveExecuteMethod.RemoteShell) {
                strExecuteMethod = "RemoteShell";
            }

            string strArchiveType = null;
            if (obj.m_archiveType == ArchiveType.Zip) {
                strArchiveType = "ZIP";
            } else if (obj.m_archiveType == ArchiveType.SevenZip) {
                strArchiveType = "SEVENZIP";
            } else if (obj.m_archiveType == ArchiveType.TarGz) {
                strArchiveType = "TARGZ";
            } else if (obj.m_archiveType == ArchiveType.TarBz2) {
                strArchiveType = "TARBZ2";
            } else if (obj.m_archiveType == ArchiveType.Tar) {
                strArchiveType = "TAR";
            }

            saver.StartObject(SettingTag.ArchiveSetting_ArchiveSetting);
            saver.AddString(SettingTag.ArchiveSetting_ExecuteMethod, strExecuteMethod);
            saver.AddString(SettingTag.ArchiveSetting_ArchiveType, strArchiveType);

            saver.StartObject(SettingTag.ArchiveSetting_Local7zZipOption);
            success = ArchiveSettingLocal7zOption.SaveSetting(saver, obj.m_local7zZipOption);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.ArchiveSetting_Local7zZipOption);

            saver.StartObject(SettingTag.ArchiveSetting_Local7z7zOption);
            success = ArchiveSettingLocal7zOption.SaveSetting(saver, obj.m_local7z7zOption);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.ArchiveSetting_Local7z7zOption);

            saver.StartObject(SettingTag.ArchiveSetting_Local7zTarGzOption);
            success = ArchiveSettingLocal7zOption.SaveSetting(saver, obj.m_local7zTarGzOption);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.ArchiveSetting_Local7zTarGzOption);

            saver.StartObject(SettingTag.ArchiveSetting_Local7zTarBz2Option);
            success = ArchiveSettingLocal7zOption.SaveSetting(saver, obj.m_local7zTarBz2Option);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.ArchiveSetting_Local7zTarBz2Option);

            saver.StartObject(SettingTag.ArchiveSetting_Local7zTarOption);
            success = ArchiveSettingLocal7zOption.SaveSetting(saver, obj.m_local7zTarOption);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.ArchiveSetting_Local7zTarOption);

            saver.StartObject(SettingTag.ArchiveSetting_RemoteZipOption);
            success = ArchiveSettingRemoteShellOption.SaveSetting(saver, obj.m_remoteZipOption);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.ArchiveSetting_RemoteZipOption);

            saver.StartObject(SettingTag.ArchiveSetting_RemoteTarGzOption);
            success = ArchiveSettingRemoteShellOption.SaveSetting(saver, obj.m_remoteTarGzOption);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.ArchiveSetting_RemoteTarGzOption);

            saver.StartObject(SettingTag.ArchiveSetting_RemoteTarBz2Option);
            success = ArchiveSettingRemoteShellOption.SaveSetting(saver, obj.m_remoteTarBz2Option);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.ArchiveSetting_RemoteTarBz2Option);

            saver.StartObject(SettingTag.ArchiveSetting_RemoteTarOption);
            success = ArchiveSettingRemoteShellOption.SaveSetting(saver, obj.m_remoteTarOption);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.ArchiveSetting_RemoteTarOption);

            saver.EndObject(SettingTag.ArchiveSetting_ArchiveSetting);

            return true;
        }

        //=========================================================================================
        // 機　能：現在のファイル形式に対するsevenzip.dll圧縮のオプションを返す
        // 引　数：なし
        // 戻り値：sevenzip.dllのオプション
        //=========================================================================================
        public ArchiveSettingLocal7zOption GetCurrentLocal7zOption() {
            if (m_archiveType == ArchiveType.SevenZip) {
                return m_local7z7zOption;
            } else if (m_archiveType == ArchiveType.Zip) {
                return m_local7zZipOption;
            } else if (m_archiveType == ArchiveType.TarGz) {
                return m_local7zTarGzOption;
            } else if (m_archiveType == ArchiveType.TarBz2) {
                return m_local7zTarBz2Option;
            } else if (m_archiveType == ArchiveType.Tar) {
                return m_local7zTarOption;
            } else {
                return m_local7zZipOption;
            }
        }

        //=========================================================================================
        // 機　能：現在のファイル形式に対するリモートシェル圧縮のオプションを返す
        // 引　数：なし
        // 戻り値：sevenzip.dllのオプション
        //=========================================================================================
        public ArchiveSettingRemoteShellOption GetCurrentRemoteShellOption() {
            if (m_archiveType == ArchiveType.Zip) {
                return m_remoteZipOption;
            } else if (m_archiveType == ArchiveType.TarGz) {
                return m_remoteTarGzOption;
            } else if (m_archiveType == ArchiveType.TarBz2) {
                return m_remoteTarBz2Option;
            } else if (m_archiveType == ArchiveType.Tar) {
                return m_remoteTarOption;
            } else {
                return m_remoteTarGzOption;
            }
        }

        //=========================================================================================
        // プロパティ：選択された圧縮実行方式
        //=========================================================================================
        public ArchiveExecuteMethod ExecuteMetohd {
            get {
                return m_executeMethod;
            }
            set {
                m_executeMethod = value;
            }
        }

        //=========================================================================================
        // プロパティ：圧縮ファイルの形式のデフォルト
        //=========================================================================================
        public ArchiveType ArchiveType {
            get {
                return m_archiveType;
            }
            set {
                m_archiveType = value;
            }
        }

        //=========================================================================================
        // プロパティ：sevenzip.dllでの7z圧縮オプション
        //=========================================================================================
        public ArchiveSettingLocal7zOption Local7z7zOption {
            get {
                return m_local7z7zOption;
            }
            set {
                m_local7z7zOption = value;
            }
        }

        //=========================================================================================
        // プロパティ：sevenzip.dllでのzip圧縮オプション
        //=========================================================================================
        public ArchiveSettingLocal7zOption Local7zZipOption {
            get {
                return m_local7zZipOption;
            }
            set {
                m_local7zZipOption = value;
            }
        }

        //=========================================================================================
        // プロパティ：sevenzip.dllでのtar.gz圧縮オプション
        //=========================================================================================
        public ArchiveSettingLocal7zOption Local7zTarGzOption {
            get {
                return m_local7zTarGzOption;
            }
            set {
                m_local7zTarGzOption = value;
            }
        }

        //=========================================================================================
        // プロパティ：sevenzip.dllでのtar.bz2圧縮オプション
        //=========================================================================================
        public ArchiveSettingLocal7zOption Local7zTarBz2Option {
            get {
                return m_local7zTarBz2Option;
            }
            set {
                m_local7zTarBz2Option = value;
            }
        }

        //=========================================================================================
        // プロパティ：sevenzip.dllでのtar圧縮オプション
        //=========================================================================================
        public ArchiveSettingLocal7zOption Local7zTarOption {
            get {
                return m_local7zTarOption;
            }
            set {
                m_local7zTarOption = value;
            }
        }

        //=========================================================================================
        // プロパティ：リモートシェルでのzip圧縮オプション
        //=========================================================================================
        public ArchiveSettingRemoteShellOption RemoteZipOption {
            get {
                return m_remoteZipOption;
            }
            set {
                m_remoteZipOption = value;
            }
        }

        //=========================================================================================
        // プロパティ：リモートシェルでのtar.gz圧縮オプション
        //=========================================================================================
        public ArchiveSettingRemoteShellOption RemoteTarGzOption {
            get {
                return m_remoteTarGzOption;
            }
            set {
                m_remoteTarGzOption = value;
            }
        }

        //=========================================================================================
        // プロパティ：リモートシェルでのtar.bz2圧縮オプション
        //=========================================================================================
        public ArchiveSettingRemoteShellOption RemoteTarBz2Option {
            get {
                return m_remoteTarBz2Option;
            }
            set {
                m_remoteTarBz2Option = value;
            }
        }

        //=========================================================================================
        // プロパティ：リモートシェルでのtar圧縮オプション
        //=========================================================================================
        public ArchiveSettingRemoteShellOption RemoteTarOption {
            get {
                return m_remoteTarOption;
            }
            set {
                m_remoteTarOption = value;
            }
        }
    }
}
