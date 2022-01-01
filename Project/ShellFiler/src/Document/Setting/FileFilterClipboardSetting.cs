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
    // クラス：クリップボード用ファイルフィルターの設定
    //=========================================================================================
    public class FileFilterClipboardSetting : ICloneable {
        // クイック設定の登録可能数
        public const int MAX_QUICK_COUNT = 4;

        // クリップボードに転送するときtrue、ファイルに転送するときfalse
        private bool m_targetClipboard = false;

        // 現在の設定
        private FileFilterListClipboard m_currentSetting = new FileFilterListClipboard();
        
        // クイック設定（設定がない要素はnull）
        private List<FileFilterListClipboard> m_quickSetting = new List<FileFilterListClipboard>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileFilterClipboardSetting() {
            for (int i = 0; i < MAX_QUICK_COUNT; i++) {
                m_quickSetting.Add(null);
            }
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileFilterClipboardSetting obj = new FileFilterClipboardSetting();
            obj.m_targetClipboard = m_targetClipboard;
            if (m_currentSetting == null) {
                obj.m_currentSetting = null;
            } else {
                obj.m_currentSetting = (FileFilterListClipboard)(m_currentSetting.Clone());
            }
            for (int i = 0; i < m_quickSetting.Count; i++) {
                if (m_quickSetting[i] == null) {
                    obj.m_quickSetting.Add(null);
                } else {
                    obj.m_quickSetting.Add((FileFilterListClipboard)(m_quickSetting[i].Clone()));
                }
            }
            return obj;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileFilterClipboardSetting obj1, FileFilterClipboardSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_targetClipboard != obj2.m_targetClipboard) {
                return false;
            }
            if (!FileFilterListClipboard.EqualsConfig(obj1.m_currentSetting, obj2.m_currentSetting)) {
                return false;
            }
            int count = Math.Max(obj1.m_quickSetting.Count, obj2.m_quickSetting.Count);
            for (int i = 0; i < count; i++) {
                FileFilterListClipboard obj1Quick = null;
                FileFilterListClipboard obj2Quick = null;
                if (i < obj1.m_quickSetting.Count) {
                    obj1Quick = obj1.m_quickSetting[i];
                }
                if (i < obj2.m_quickSetting.Count) {
                    obj2Quick = obj2.m_quickSetting[i];
                }
                if (obj1Quick == null && obj2Quick == null) {
                    ;
                } else if (obj1Quick != null || obj2Quick != null) {
                    return false;
                } else {
                    if (!FileFilterListClipboard.EqualsConfig(obj1Quick, obj2Quick)) {
                        return false;
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：デフォルト値を設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetDefaultSetting() {
            m_currentSetting = new FileFilterListClipboard();
            m_quickSetting = new List<FileFilterListClipboard>();
            for (int i = 0; i < MAX_QUICK_COUNT; i++) {
                m_quickSetting.Add(null);
            }
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト（フィルターのどれかにエラーがあった場合はnull）
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out FileFilterClipboardSetting obj) {
            bool success;
            obj = new FileFilterClipboardSetting();

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.FileFilter_ClipboardItem, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileFilter_ClipboardItem) {
                    break;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileFilter_TargetClipboard) {
                    obj.m_targetClipboard = loader.BoolValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileFilter_CurrentSetting) {
                    FileFilterListClipboard list;
                    success = FileFilterListClipboard.LoadSetting(loader, out list);
                    if (!success) {
                        return false;
                    }
                    if (list != null) {     // エラーの場合はデフォルトのまま
                        obj.m_currentSetting = list;
                    }
                    success = loader.ExpectTag(SettingTag.FileFilter_CurrentSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileFilter_QuickSetting) {
                    List<FileFilterListClipboard> list = new List<FileFilterListClipboard>();
                    success = LoadSettingQuickList(loader, list);
                    if (!success) {
                        return false;
                    }
                    obj.m_quickSetting = list;
                    success = loader.ExpectTag(SettingTag.FileFilter_QuickSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }
            return true;
        }
        
        //=========================================================================================
        // 機　能：クイック設定のリスト項目を読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]list    読み込んだ結果を返すリスト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSettingQuickList(SettingLoader loader, List<FileFilterListClipboard> list) {
            while (true) {
                bool success;
                bool fit;
                success = loader.PeekTag(SettingTag.FileFilter_QuickSetting, SettingTagType.EndObject, out fit);
                if (!success) {
                    return false;
                }
                if (fit) {
                    for (int i = list.Count; i < MAX_QUICK_COUNT; i++) {
                        list.Add(null);
                    }
                    if (list.Count > MAX_QUICK_COUNT) {
                        list.RemoveRange(MAX_QUICK_COUNT, list.Count - MAX_QUICK_COUNT);
                    }
                    return true;
                }

                FileFilterListClipboard item;
                success = FileFilterListClipboard.LoadSetting(loader, out item);
                if (!success) {
                    return false;
                }
                list.Add(item);             // エラーの場合はnullのまま登録
            }
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, FileFilterClipboardSetting obj) {
            saver.StartObject(SettingTag.FileFilter_ClipboardItem);

            saver.AddBool(SettingTag.FileFilter_TargetClipboard, obj.m_targetClipboard);

            saver.StartObject(SettingTag.FileFilter_CurrentSetting);
            FileFilterListClipboard.SaveSetting(saver, obj.m_currentSetting);
            saver.EndObject(SettingTag.FileFilter_CurrentSetting);
            
            saver.StartObject(SettingTag.FileFilter_QuickSetting);
            foreach (FileFilterListClipboard list in obj.m_quickSetting) {
                FileFilterListClipboard.SaveSetting(saver, list);
            }
            saver.EndObject(SettingTag.FileFilter_QuickSetting);

            saver.EndObject(SettingTag.FileFilter_ClipboardItem);
            
            return true;
        }

        //=========================================================================================
        // プロパティ：クリップボードに転送するときtrue、ファイルに転送するときfalse
        //=========================================================================================
        public bool TargetClipboard {
            get {
                return m_targetClipboard;
            }
            set {
                m_targetClipboard = value;
            }
        }

        //=========================================================================================
        // プロパティ：現在の設定
        //=========================================================================================
        public FileFilterListClipboard CurrentSetting {
            get {
                return m_currentSetting;
            }
            set {
                m_currentSetting = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：クイック設定（設定がない要素はnull）
        //=========================================================================================
        public List<FileFilterListClipboard> QuickSetting {
            get {
                return m_quickSetting;
            }
            set {
                m_quickSetting = value;
            }
        }
    }
}
