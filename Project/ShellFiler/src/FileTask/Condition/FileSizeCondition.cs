using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：ファイルサイズの条件
    //=========================================================================================
    public class FileSizeCondition {
        // サイズの指定条件
        private FileSizeType m_sizeType = FileSizeType.None;

        // 下限値（指定しないとき-1）
        private long m_minSize = -1;

        // 下限値そのものを含むときtrue（指定しないときfalse）
        private bool m_includeMin = false;

        // 上限値（指定しないとき-1）
        private long m_maxSize = -1;

        // 上限値そのものを含むときtrue（指定しないときfalse）
        private bool m_includeMax = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileSizeCondition() {
        }
        
        //=========================================================================================
        // 機　能：UI設定用のデフォルト値を設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetUIDefaultValue() {
            // 日付を初期化
            if (m_minSize == -1 && m_maxSize == -1) {
                m_minSize = 1024 * 1024;
                m_maxSize = m_minSize;
            } else if (m_minSize == -1) {
                m_minSize = m_maxSize;
            } else if (m_maxSize == -1) {
                m_maxSize = m_minSize;
            }
        }

        //=========================================================================================
        // 機　能：UI設定用の値から不要部分を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CleanupField() {
            bool useMinSize, useIncludeMin, useMaxSize, useIncludeMax;
            CheckMandatoryField(m_sizeType, out useMinSize, out useIncludeMin, out useMaxSize, out useIncludeMax);

            if (!useMinSize) {
                m_minSize = -1;
            }
            if (!useIncludeMin) {
                m_includeMin = false;
            }
            if (!useMaxSize) {
                m_maxSize = -1;
            }
            if (!useIncludeMax) {
                m_includeMax = false;
            }
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileSizeCondition clone = new FileSizeCondition();
            clone.m_sizeType = m_sizeType;
            clone.m_minSize = m_minSize;
            clone.m_includeMin = m_includeMin;
            clone.m_maxSize = m_maxSize;
            clone.m_includeMax = m_includeMax;

            return clone;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileSizeCondition obj1, FileSizeCondition obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_sizeType != obj2.m_sizeType) {
                return false;
            }
            if (obj1.m_minSize != obj2.m_minSize) {
                return false;
            }
            if (obj1.m_includeMin != obj2.m_includeMin) {
                return false;
            }
            if (obj1.m_maxSize != obj2.m_maxSize) {
                return false;
            }
            if (obj1.m_includeMax != obj2.m_includeMax) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：実際のファイルサイズと、このオブジェクトの条件を比較する
        // 引　数：[in]size  比較対象のファイルサイズ
        // 戻り値：条件に一致するファイルのときtrue
        //=========================================================================================
        public bool CompareFile(long size) {
            if (m_sizeType == FileSizeType.None) {
                return true;
            } else if (m_sizeType == FileSizeType.XxxSize) {
                if (m_includeMin) {
                    return (size <= m_minSize);
                } else {
                    return (size < m_minSize);
                }
            } else if (m_sizeType == FileSizeType.SizeXxx) {
                if (m_includeMax) {
                    return (m_maxSize <= size);
                } else {
                    return (m_maxSize < size);
                }
            } else if (m_sizeType == FileSizeType.SizeXxxSize) {
                if (m_includeMin && m_includeMax) {
                    return (m_minSize <= size && size <= m_maxSize);
                } else if (m_includeMin) {
                    return (m_minSize <= size && size < m_maxSize);
                } else if (m_includeMax) {
                    return (m_minSize < size && size <= m_maxSize);
                } else {
                    return (m_minSize < size && size < m_maxSize);
                }
            } else if (m_sizeType == FileSizeType.XxxSizeXxx) {
                if (m_includeMin && m_includeMax) {
                    return (size <= m_minSize && m_maxSize <= size);
                } else if (m_includeMin) {
                    return (size <= m_minSize && m_maxSize < size);
                } else if (m_includeMax) {
                    return (size < m_minSize && m_maxSize <= size);
                } else {
                    return (size < m_minSize && m_maxSize < size);
                }
            } else if (m_sizeType == FileSizeType.Size) {
                    return (size == m_minSize);
            } else {
                Program.Abort("sizeTypeの値が想定外です。");
                return false;
            }
        }

        //=========================================================================================
        // 機　能：内容に矛盾がないかどうかを検証する
        // 引　数：なし
        // 戻り値：内容が正しいときtrue
        //=========================================================================================
        public bool Validate() {
            if (m_sizeType == null) {
                return false;
            }
            bool useMinSize, useIncludeMin, useMaxSize, useIncludeMax;
            CheckMandatoryField(m_sizeType, out useMinSize, out useIncludeMin, out useMaxSize, out useIncludeMax);

            if (useMinSize) {
                if (m_minSize == -1) {
                    return false;
                }
            } else {
                if (m_minSize != -1) {
                    return false;
                }
            }

            if (useIncludeMin) {
                ;
            } else {
                if (m_includeMin != false) {
                    return false;
                }
            }
            
            if (useMaxSize) {
                if (m_maxSize == -1) {
                    return false;
                }
            } else {
                if (m_maxSize != -1) {
                    return false;
                }
            }

            if (useIncludeMax) {
                ;
            } else {
                if (m_includeMax != false) {
                    return false;
                }
            }

            return true;
        }

        //=========================================================================================
        // 機　能：モードごとに必要なフィールドを返す
        // 引　数：[in]sizeType       サイズ指定のモード
        // 　　　　[in]useMinSize     m_minSizeが必要なときtrue
        // 　　　　[in]useIncludeMin  m_includeMinが必要なときtrue
        // 　　　　[in]useMaxSize     m_maxSizeが必要なときtrue
        // 　　　　[in]useIncludeMax  m_includeMaxが必要なときtrue
        // 戻り値：なし
        //=========================================================================================
        public void CheckMandatoryField(FileSizeType sizeType, out bool useMinSize, out bool useIncludeMin, out bool useMaxSize, out bool useIncludeMax) {
            useMinSize = true;
            useIncludeMin = true;
            useMaxSize = true;
            useIncludeMax = true;

            if (sizeType == FileSizeType.None) {
                useMinSize = false;
                useIncludeMin = false;
                useMaxSize = false;
                useIncludeMax = false;
            } else if (sizeType == FileSizeType.XxxSize) {
                useMaxSize = false;
                useIncludeMax = false;
            } else if (sizeType == FileSizeType.SizeXxx) {
                useMinSize = false;
                useIncludeMin = false;
            } else if (sizeType == FileSizeType.SizeXxxSize) {
            } else if (sizeType == FileSizeType.XxxSizeXxx) {
            } else if (sizeType == FileSizeType.Size) {
                useMaxSize = false;
                useIncludeMax = false;
            } else {
                Program.Abort("sizeTypeの値が想定外です。");
                return;
            }
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]endTag  期待される終了タグ
        // 　　　　[out]obj    読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, SettingTag endTag, out FileSizeCondition obj) {
            bool success;
            obj = new FileSizeCondition();

            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == endTag) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileCondition_FileSizeType) {
                    obj.m_sizeType = FileSizeType.FromString(loader.StringValue);                       // NGのときnullとしてValidate()
                } else if (tagType == SettingTagType.LongValue && tagName == SettingTag.FileCondition_FileSizeMinSize) {
                    obj.m_minSize = loader.LongValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_FileSizeIncludeMin) {
                    obj.m_includeMin = loader.BoolValue;
                } else if (tagType == SettingTagType.LongValue && tagName == SettingTag.FileCondition_FileSizeMaxSize) {
                    obj.m_maxSize = loader.LongValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_FileSizeIncludeMax) {
                    obj.m_includeMax = loader.BoolValue;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに項目1件を保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, FileSizeCondition obj) {
            saver.AddString(SettingTag.FileCondition_FileSizeType, obj.m_sizeType.StringName);
            saver.AddLong(SettingTag.FileCondition_FileSizeMinSize, obj.m_minSize);
            saver.AddBool(SettingTag.FileCondition_FileSizeIncludeMin, obj.m_includeMin);
            saver.AddLong(SettingTag.FileCondition_FileSizeMaxSize, obj.m_maxSize);
            saver.AddBool(SettingTag.FileCondition_FileSizeIncludeMax, obj.m_includeMax);

            return true;
        }

        //=========================================================================================
        // プロパティ：サイズの指定条件
        //=========================================================================================
        public FileSizeType SizeType {
            get {
                return m_sizeType;
            }
            set {
                m_sizeType = value;
            }
        }

        //=========================================================================================
        // プロパティ：下限値
        //=========================================================================================
        public long MinSize {
            get {
                return m_minSize;
            }
            set {
                m_minSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：下限値そのものを含むときtrue（指定しないときfalse）
        //=========================================================================================
        public bool IncludeMin {
            get {
                return m_includeMin;
            }
            set {
                m_includeMin = value;
            }
        }

        //=========================================================================================
        // プロパティ：上限値
        //=========================================================================================
        public long MaxSize {
            get {
                return m_maxSize;
            }
            set {
                m_maxSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：上限値そのものを含むときtrue（指定しないときfalse）
        //=========================================================================================
        public bool IncludeMax {
            get {
                return m_includeMax;
            }
            set {
                m_includeMax = value;
            }
        }
    }
}
