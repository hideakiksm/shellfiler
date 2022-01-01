using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommandXmlConverter.Properties;
using ShellFiler.Document.Serialize.CommandApi;
using ShellFiler.Document.Setting;
using ShellFiler.Util;

namespace CommandXmlConverter {

    //=========================================================================================
    // クラス：Commandクラスの保存クラス
    //=========================================================================================
    public class CommandApiSaver {
        // ファイル名
        private string m_fileName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileName     ファイル名
        // 戻り値：なし
        //=========================================================================================
        public CommandApiSaver(string fileName) {
            m_fileName = fileName;
        }

        //=========================================================================================
        // 機　能：設定を書き込む
        // 引　数：[in]obj  書き込むコマンド一覧
        // 戻り値：書き込みに成功したときtrue
        //=========================================================================================
        public bool Save(XCommandSpec obj) {
            try {
                File.Delete(m_fileName);
            } catch (Exception) {
            }

            SettingSaverBasic saver = new SettingSaverBasic(m_fileName);
            bool success = SaveCommandSpec(saver, obj);
            if (!success) {
                return false;
            }
            return saver.SaveSetting(true);
        }

        //=========================================================================================
        // 機　能：設定を書き込む
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    書き込むコマンド一覧
        // 戻り値：書き込みに成功したときtrue
        //=========================================================================================
        private bool SaveCommandSpec(SettingSaverBasic saver, XCommandSpec obj) {
            bool success;

            saver.StartObject(SettingTag.Command_CommandSpec);
           
            saver.StartObject(SettingTag.Command_FileList);
            success = SaveCommandScene(saver, obj.FileList);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Command_FileList);

            saver.StartObject(SettingTag.Command_FileViewer);
            success = SaveCommandScene(saver, obj.FileViewer);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Command_FileViewer);

            saver.StartObject(SettingTag.Command_GraphicsViewer);
            success = SaveCommandScene(saver, obj.GraphicsViewer);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.Command_GraphicsViewer);

            saver.EndObject(SettingTag.Command_CommandSpec);

            return true;
        }

        //=========================================================================================
        // 機　能：XCommandSceneを書き込む
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    書き込むコマンド一覧
        // 戻り値：書き込みに成功したときtrue
        //=========================================================================================
        private bool SaveCommandScene(SettingSaverBasic saver, XCommandScene obj) {
            bool success;

            saver.StartObject(SettingTag.Command_CommandScene);
            foreach (XCommandGroup group in obj.CommandGroup) {
                saver.StartObject(SettingTag.Command_CommandGroupList);
                success = SaveCommandGroup(saver, group);
                if (!success) {
                    return false;
                }
                saver.EndObject(SettingTag.Command_CommandGroupList);
            }
            saver.EndObject(SettingTag.Command_CommandScene);

            return true;
        }

        //=========================================================================================
        // 機　能：XCommandGroupを書き込む
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    書き込むコマンドグループ
        // 戻り値：書き込みに成功したときtrue
        //=========================================================================================
        private bool SaveCommandGroup(SettingSaverBasic saver, XCommandGroup obj) {
            bool success;

            saver.StartObject(SettingTag.Command_CommandGroup);
            
            saver.AddString(SettingTag.Command_GroupDisplayName, obj.GroupDisplayName);
            saver.AddString(SettingTag.Command_PackageName, obj.PackageName);

            saver.StartObject(SettingTag.Command_FunctionList);
            foreach (XCommandApi api in obj.FunctionList) {
                success = SaveCommandApi(saver, api);
                if (!success) {
                    return false;
                }
            }
            saver.EndObject(SettingTag.Command_FunctionList);

            saver.EndObject(SettingTag.Command_CommandGroup);

            return true;
        }

        //=========================================================================================
        // 機　能：XCommandApiを書き込む
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    書き込むコマンド
        // 戻り値：書き込みに成功したときtrue
        //=========================================================================================
        private bool SaveCommandApi(SettingSaverBasic saver, XCommandApi obj) {
            bool success;

            saver.StartObject(SettingTag.Command_CommandApi);
            
            saver.AddString(SettingTag.Command_CommandName, obj.CommandName);
            saver.AddString(SettingTag.Command_Comment, obj.Comment);

            saver.StartObject(SettingTag.Command_ArgumentList);
            foreach (XCommandArgument api in obj.ArgumentList) {
                success = SaveCommandArgument(saver, api);
                if (!success) {
                    return false;
                }
            }
            saver.EndObject(SettingTag.Command_ArgumentList);

            saver.EndObject(SettingTag.Command_CommandApi);

            return true;
        }

        //=========================================================================================
        // 機　能：XCommandApiを書き込む
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    書き込むコマンド
        // 戻り値：書き込みに成功したときtrue
        //=========================================================================================
        private bool SaveCommandArgument(SettingSaverBasic saver, XCommandArgument obj) {
            saver.StartObject(SettingTag.Command_CommandArgument);
            
            saver.AddString(SettingTag.Command_ArgumentName, obj.ArgumentName);
            saver.AddString(SettingTag.Command_ArgumentType, obj.ArgumentType);
            saver.AddString(SettingTag.Command_ArgumentComment, obj.ArgumentComment);
            saver.AddString(SettingTag.Command_DefaultValue, obj.DefaultValue);
            saver.AddString(SettingTag.Command_ValueRange, obj.ValueRange);

            saver.EndObject(SettingTag.Command_CommandArgument);

            return true;
        }
    }
}
