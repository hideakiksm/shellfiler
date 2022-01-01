using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Text;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：転送用ファイルフィルターの設定
    //=========================================================================================
    public class FileFilterTransferSetting : ICloneable {
        // 転送用設定の登録可能数
        public const int MAX_TRANSFER_COUNT = 10;
        
        // 詳細設定でのフィルター設定一覧
        private List<FileFilterListTransfer> m_transferList = new List<FileFilterListTransfer>();

        // 詳細設定でのその他のファイルの扱い
        private FileFilterListTransferOtherMode m_otherMode = FileFilterListTransferOtherMode.SkipTransfer;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterTransferSetting() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileFilterTransferSetting obj = new FileFilterTransferSetting();
            for (int i = 0; i < m_transferList.Count; i++) {
                obj.m_transferList.Add((FileFilterListTransfer)(m_transferList[i].Clone()));
            }
            obj.m_otherMode = m_otherMode;

            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileFilterTransferSetting obj1, FileFilterTransferSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_transferList.Count != obj2.m_transferList.Count) {
                return false;
            }
            for (int i = 0; i < obj1.m_transferList.Count; i++) {
                FileFilterListTransfer obj1List = obj1.m_transferList[i];
                FileFilterListTransfer obj2List = obj2.m_transferList[i];
                if (!FileFilterListTransfer.EqualsConfig(obj1List, obj2List)) {
                    return false;
                }
            }
            if (obj1.m_otherMode != obj2.m_otherMode) {
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
        public static bool LoadSetting(SettingLoader loader, out FileFilterTransferSetting obj) {
            bool success;
            obj = new FileFilterTransferSetting();

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.FileFilter_TransferItem, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileFilter_TransferItem) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileFilter_TransferOtherMode) {
                    obj.m_otherMode = FileFilterListTransferOtherMode.StringToTransferMode(loader.StringValue, obj.m_otherMode);
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileFilter_TransferList) {
                    List<FileFilterListTransfer> list = new List<FileFilterListTransfer>();
                    success = LoadSettingTransferList(loader, list);
                    if (!success) {
                        return false;
                    }
                    obj.m_transferList = list;
                    success = loader.ExpectTag(SettingTag.FileFilter_TransferList, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：転送設定のリスト項目を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]list    読み込んだ結果を返すリスト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSettingTransferList(SettingLoader loader, List<FileFilterListTransfer> list) {
            while (true) {
                bool success;
                bool fit;
                success = loader.PeekTag(SettingTag.FileFilter_TransferList, SettingTagType.EndObject, out fit);
                if (!success) {
                    return success;
                }
                if (fit) {
                    return true;
                }

                FileFilterListTransfer item;
                success = FileFilterListTransfer.LoadSetting(loader, out item);
                if (!success) {
                    return false;
                }
                if (item != null) {
                    list.Add(item);             // エラーの場合は登録を回避
                }
            }
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, FileFilterTransferSetting obj) {
            saver.StartObject(SettingTag.FileFilter_TransferItem);

            saver.AddString(SettingTag.FileFilter_TransferOtherMode, FileFilterListTransferOtherMode.TransferModeToString(obj.m_otherMode));
            saver.StartObject(SettingTag.FileFilter_TransferList);
            foreach (FileFilterListTransfer list in obj.m_transferList) {
                FileFilterListTransfer.SaveSetting(saver, list);
            }
            saver.EndObject(SettingTag.FileFilter_TransferList);

            saver.EndObject(SettingTag.FileFilter_TransferItem);
            
            return true;
        }

        //=========================================================================================
        // プロパティ：詳細設定でのフィルター設定一覧
        //=========================================================================================
        public List<FileFilterListTransfer> TransferList {
            get {
                return m_transferList;
            }
            set {
                m_transferList = value;
            }
        }

        //=========================================================================================
        // プロパティ：詳細設定でのその他のファイルの扱い
        //=========================================================================================
        public FileFilterListTransferOtherMode OtherMode {
            get {
                return m_otherMode;
            }
            set {
                m_otherMode = value;
            }
        }
    }
}
