using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：SSH接続のための設定
    //=========================================================================================
    public class SSHUserAuthenticateSetting {
        // 接続情報のリスト
        private List<SSHUserAuthenticateSettingItem> m_listSetting = new List<SSHUserAuthenticateSettingItem>();

        // 一時接続情報のリスト
        private List<SSHUserAuthenticateSettingItem> m_listTempSetting = new List<SSHUserAuthenticateSettingItem>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SSHUserAuthenticateSetting() {
            LoadData();
        }
        
        //=========================================================================================
        // 機　能：データを読み込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void LoadData() {
#if !FREE_VERSION
            string fileName = DirectoryManager.SSHUserAuthenticate;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = LoadSetting(loader, this);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSetting, fileName, loader.ErrorDetail);
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSetting(SettingLoader loader, SSHUserAuthenticateSetting obj) {
            // ファイルがないときはそのまま
            if (!File.Exists(loader.FileName)) {
                return true;
            }

            // ファイルから読み込む
            bool success;
            success = loader.LoadSetting(false);
            if (!success) {
                return false;
            }

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.SSHAuthentivate_SSHAuthenticate, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            List<SSHUserAuthenticateSettingItem> listSetting = new List<SSHUserAuthenticateSettingItem>();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.SSHAuthentivate_SSHAuthenticate) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.SSHAuthentivate_Item) {
                    SSHUserAuthenticateSettingItem item;
                    success = LoadAuthenticateItem(loader, out item);
                    if (!success) {
                        return false;
                    }
                    listSetting.Add(item);
                }
            }
            obj.m_listSetting.Clear();
            obj.m_listSetting.AddRange(listSetting);
            return true;
        }

        //=========================================================================================
        // 機　能：認証情報の項目を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadAuthenticateItem(SettingLoader loader, out SSHUserAuthenticateSettingItem obj) {
            bool success;
            obj = new SSHUserAuthenticateSettingItem();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.SSHAuthentivate_Item) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.SSHAuthentivate_DisplayName) {
                    obj.DisplayName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.SSHAuthentivate_ServerName) {
                    obj.ServerName = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.SSHAuthentivate_UserName) {
                    obj.UserName = loader.StringValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.SSHAuthentivate_KeyAuthentication) {
                    obj.KeyAuthentication = loader.BoolValue;
                } else if (tagType == SettingTagType.PasswordValue && tagName == SettingTag.SSHAuthentivate_Password) {
                    obj.Password = loader.PasswordValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.SSHAuthentivate_PrivateKey) {
                    obj.PrivateKeyFilePath = loader.StringValue;
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.SSHAuthentivate_PortNo) {
                    obj.PortNo = loader.IntValue;
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.SSHAuthentivate_Timeout) {
                    obj.Timeout = loader.IntValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.SSHAuthentivate_Encoding) {
                    Encoding encoding = SSHUserAuthenticateSettingItem.NameToEncoding(loader.StringValue);
                    if (encoding == null) {
                        return false;
                    }
                    obj.Encoding = encoding;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.SSHAuthentivate_TargetOS) {
                    SSHUserAuthenticateSettingItem.OSType osType = SSHUserAuthenticateSettingItem.NameToOSType(loader.StringValue);
                    if (osType == null) {
                        return false;
                    }
                    obj.TargetOS = osType;
                }
            }
            if (obj.DisplayName == null || obj.ServerName == null || obj.UserName == null ||
                    obj.PortNo == -1 || obj.Timeout == -1 ||
                    obj.Encoding == null || obj.TargetOS == null) {
                return false;
            }
            if (obj.KeyAuthentication && obj.PrivateKeyFilePath == null) {
                return false;
            }
            if (!obj.KeyAuthentication) {
                obj.PrivateKeyFilePath = null;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：データを書き込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SaveData() {
#if !FREE_VERSION
            string fileName = DirectoryManager.SSHUserAuthenticate;
            SettingSaver saver = new SettingSaver(fileName);
            bool success = SaveSettingInternal(saver);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotSaveSetting, fileName);
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private bool SaveSettingInternal(SettingSaver saver) {
            saver.StartObject(SettingTag.SSHAuthentivate_SSHAuthenticate);
            foreach (SSHUserAuthenticateSettingItem item in m_listSetting) {
                saver.StartObject(SettingTag.SSHAuthentivate_Item);
                saver.AddString(SettingTag.SSHAuthentivate_DisplayName, item.DisplayName);
                saver.AddString(SettingTag.SSHAuthentivate_ServerName, item.ServerName);
                saver.AddString(SettingTag.SSHAuthentivate_UserName, item.UserName);
                saver.AddBool(SettingTag.SSHAuthentivate_KeyAuthentication, item.KeyAuthentication);
                saver.AddPassword(SettingTag.SSHAuthentivate_Password, item.Password);
                saver.AddString(SettingTag.SSHAuthentivate_PrivateKey, item.PrivateKeyFilePath);
                saver.AddInt(SettingTag.SSHAuthentivate_PortNo, item.PortNo);
                saver.AddInt(SettingTag.SSHAuthentivate_Timeout, item.Timeout);
                saver.AddString(SettingTag.SSHAuthentivate_Encoding, SSHUserAuthenticateSettingItem.EncodingToName(item.Encoding));
                saver.AddString(SettingTag.SSHAuthentivate_TargetOS, SSHUserAuthenticateSettingItem.OSTypeToName(item.TargetOS));
                saver.EndObject(SettingTag.SSHAuthentivate_Item);
            }
            saver.EndObject(SettingTag.SSHAuthentivate_SSHAuthenticate);

            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // 機　能：指定されたユーザー名とサーバー名に対応する接続情報を返す
        // 引　数：[in]server   接続先サーバー名
        // 　　　　[in]user     接続に使用するユーザー名
        // 　　　　[in]port     接続に使用するポート番号
        // 　　　　[out]isTemp  一時接続用のセッションの場合trueを返す変数
        // 戻り値：取得した接続情報（取得できなかったときnull）
        //=========================================================================================
        public SSHUserAuthenticateSettingItem GetUserAuthenticateSetting(string server, string user, int port, out bool isTemp) {
            isTemp = true;
            for (int i = 0; i < m_listTempSetting.Count; i++) {
                SSHUserAuthenticateSettingItem item = m_listTempSetting[i];
                if (item.ServerName == server && item.UserName == user && item.PortNo == port) {
                    m_listTempSetting.RemoveAt(i);
                    return item;
                }
            }

            isTemp = false;
            for (int i = 0; i < m_listSetting.Count; i++) {
                SSHUserAuthenticateSettingItem item = m_listSetting[i];
                if (item.ServerName == server && item.UserName == user && item.PortNo == port) {
                    return item;
                }
            }

            return null;
        }
        
        //=========================================================================================
        // 機　能：指定されたユーザー名とサーバー名に対応する接続情報を参照する
        // 引　数：[in]server   接続先サーバー名
        // 　　　　[in]user     接続に使用するユーザー名
        // 　　　　[in]port     接続に使用するポート番号
        // 　　　　[out]isTemp  一時接続用のセッションの場合trueを返す変数
        // 戻り値：取得した接続情報（取得できなかったときnull）
        //=========================================================================================
        public SSHUserAuthenticateSettingItem ReferAuthenticateSetting(string server, string user, int port, out bool isTemp) {
            isTemp = true;
            for (int i = 0; i < m_listTempSetting.Count; i++) {
                SSHUserAuthenticateSettingItem item = m_listTempSetting[i];
                if (item.ServerName == server && item.UserName == user && item.PortNo == port) {
                    return item;
                }
            }

            isTemp = false;
            for (int i = 0; i < m_listSetting.Count; i++) {
                SSHUserAuthenticateSettingItem item = m_listSetting[i];
                if (item.ServerName == server && item.UserName == user && item.PortNo == port) {
                    return item;
                }
            }

            return null;
        }
        
        //=========================================================================================
        // 機　能：設定情報を追加する
        // 引　数：[in]setting  追加する設定情報
        // 戻り値：なし
        //=========================================================================================
        public void AddSetting(SSHUserAuthenticateSettingItem setting) {
            m_listSetting.Add(setting);
        }

        //=========================================================================================
        // 機　能：一時接続情報として設定情報を追加する
        // 引　数：[in]setting  追加する設定情報
        // 戻り値：なし
        //=========================================================================================
        public void AddTemporarySetting(SSHUserAuthenticateSettingItem setting) {
            for (int i = 0; i < m_listTempSetting.Count; i++) {
                SSHUserAuthenticateSettingItem item = m_listTempSetting[i];
                if (item.ServerName == setting.ServerName && item.UserName == setting.UserName && item.PortNo == setting.PortNo) {
                    m_listTempSetting[i] = setting;
                    return;
                }
            }
            m_listTempSetting.Add(setting);
        }

        //=========================================================================================
        // 機　能：指定されたユーザー名とサーバー名に対応する接続情報を削除する
        // 引　数：[in]server   サーバー名
        // 　　　　[in]user     ユーザー名
        // 　　　　[in]port     ポート番号
        // 戻り値：なし
        //=========================================================================================
        public void DeleteUserAuthenticateSetting(string server, string user, int port) {
            for (int i = 0; i < m_listSetting.Count; i++) {
                SSHUserAuthenticateSettingItem item = m_listSetting[i];
                if (item.ServerName == server && item.UserName == user && item.PortNo == port) {
                    m_listSetting.RemoveAt(i);
                    break;
                }
            }
        }

        //=========================================================================================
        // プロパティ：接続情報のリスト
        //=========================================================================================
        public List<SSHUserAuthenticateSettingItem> UserAuthenticateSettingList {
            get {
                return m_listSetting;
            }
        }
    }
}
