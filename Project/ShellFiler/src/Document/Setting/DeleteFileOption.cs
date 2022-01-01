using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Document;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：削除オプションを表現するためのデータ構造
    //         ダイアログとのやりとり、バックグラウンドタスクで処理方法をまとめるために使用する
    //=========================================================================================
    public class DeleteFileOption : ICloneable {
        // ディレクトリをすべて削除してよいときtrue
        private bool m_deleteDirectoryAll = false;
        
        // システム属性、読み込み専用属性のファイルを削除してよいときtrue
        private bool m_deleteSpecialAttrAll = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DeleteFileOption() {
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
        public static bool EqualsConfig(DeleteFileOption obj1, DeleteFileOption obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_deleteDirectoryAll != obj2.m_deleteDirectoryAll) {
                return false;
            }
            if (obj1.m_deleteSpecialAttrAll != obj2.m_deleteSpecialAttrAll) {
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
        public static bool LoadSetting(SettingLoader loader, out DeleteFileOption obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.DeleteFileOption_DeleteFileOption, SettingTagType.BeginObject, out fit);
            if (!success) {
                return success;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new DeleteFileOption();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return success;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.DeleteFileOption_DeleteFileOption) {
                    break;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.DeleteFileOption_DeleteDirectoryAll) {
                    obj.m_deleteDirectoryAll = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.DeleteFileOption_DeleteSpecialAttrAll) {
                    obj.m_deleteSpecialAttrAll = loader.BoolValue;
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
        public static bool SaveSetting(SettingSaver saver, DeleteFileOption obj) {
            if (obj == null) {
                return true;
            }

            saver.StartObject(SettingTag.DeleteFileOption_DeleteFileOption);
            saver.AddBool(SettingTag.DeleteFileOption_DeleteDirectoryAll, obj.m_deleteDirectoryAll);
            saver.AddBool(SettingTag.DeleteFileOption_DeleteSpecialAttrAll, obj.m_deleteSpecialAttrAll);
            saver.EndObject(SettingTag.DeleteFileOption_DeleteFileOption);

            return true;
        }

        //=========================================================================================
        // プロパティ：ディレクトリをすべて削除してよいときtrue
        //=========================================================================================
        public bool DeleteDirectoryAll {
            get {
                return m_deleteDirectoryAll;
            }
            set {
                m_deleteDirectoryAll = value;
            }
        }

        //=========================================================================================
        // プロパティ：システム属性、読み込み専用属性のファイルを削除してよいときtrue
        //=========================================================================================
        public bool DeleteSpecialAttrAll {
            get {
                return m_deleteSpecialAttrAll;
            }
            set {
                m_deleteSpecialAttrAll = value;
            }
        }
    }
}
