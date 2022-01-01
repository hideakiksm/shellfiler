using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using ShellFiler.Api;
using ShellFiler.Locale;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.DataObject;
using ShellFiler.Util;
using ShellFiler.UI.FileList;
using ShellFiler.Properties;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイル転送時に属性の転送を行うかどうかのモード
    //=========================================================================================
    public class AttributeSetMode {
        // 転送先がWindows:転送後にすべての属性をセットする
        private bool m_windowsSetAttributeAll = false;

        // 転送先がSSH:転送後にすべての属性をセットする
        private bool m_sshSetAtributeAll = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AttributeSetMode() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            AttributeSetMode obj = new AttributeSetMode();
            obj.m_windowsSetAttributeAll = m_windowsSetAttributeAll;
            obj.m_sshSetAtributeAll = m_sshSetAtributeAll;
            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(AttributeSetMode obj1, AttributeSetMode obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_windowsSetAttributeAll != obj2.m_windowsSetAttributeAll) {
                return false;
            }
            if (obj1.m_sshSetAtributeAll != obj2.m_sshSetAtributeAll) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト（フィルターのどれかにエラーがあった場合はnull）
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out AttributeSetMode obj) {
            bool success;
            obj = new AttributeSetMode();

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.AttributeSetMode, SettingTagType.BeginObject);
            if (!success) {
                return false;
            }
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.AttributeSetMode) {
                    break;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.AttributeSetMode_WindowsSetAttributeAll) {
                    obj.m_windowsSetAttributeAll = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.AttributeSetMode_SshSetAtributeAll) {
                    obj.m_sshSetAtributeAll = loader.BoolValue;
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
        public static bool SaveSetting(SettingSaver saver, AttributeSetMode obj) {
            saver.StartObject(SettingTag.AttributeSetMode);

            saver.AddBool(SettingTag.AttributeSetMode_WindowsSetAttributeAll, obj.m_windowsSetAttributeAll);
            saver.AddBool(SettingTag.AttributeSetMode_SshSetAtributeAll, obj.m_sshSetAtributeAll);

            saver.EndObject(SettingTag.AttributeSetMode);
            
            return true;
        }

        //=========================================================================================
        // 機　能：ファイル転送時に属性のコピーも行うかどうかを返す
        // 引　数：[in]fileSystem  転送先のファイルシステム
        // 戻り値：属性をコピーするときtrue
        //=========================================================================================
        public bool IsSetAttribute(FileSystemID fileSystem) {
            if (FileSystemID.IsWindows(fileSystem)) {
                return m_windowsSetAttributeAll;
            } else if (FileSystemID.IsSSH(fileSystem)) {
                return m_sshSetAtributeAll;
            } else if (FileSystemID.IsVirtual(fileSystem)) {
                return false;
            } else {
                FileSystemID.NotSupportError(fileSystem);
                return false;
            }
        }

        //=========================================================================================
        // プロパティ：Windows:転送後にすべての属性をセットする
        //=========================================================================================
        public bool WindowsSetAttributeAll {
            get {
                return m_windowsSetAttributeAll;
            }
            set {
                m_windowsSetAttributeAll = value;
            }
        }

        //=========================================================================================
        // プロパティ：SSH:転送後にすべての属性をセットする
        //=========================================================================================
        public bool SshSetAtributeAll {
            get {
                return m_sshSetAtributeAll;
            }
            set {
                m_sshSetAtributeAll = value;
            }
        }
    }
}
