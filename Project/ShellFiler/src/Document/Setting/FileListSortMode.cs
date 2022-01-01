using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.FileSystem.Windows;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ファイル一覧のソート方法
    //=========================================================================================
    public class FileListSortMode : ICloneable {
        // 第1ソートキー
        private Method m_sortOrder1 = Method.NoSort;

        // 第2ソートキー
        private Method m_sortOrder2 = Method.NoSort;

        // 第1ソートキーのソート方向
        private Direction m_sortDirection1 = Direction.Normal;

        // 第2ソートキーのソート方向
        private Direction m_sortDirection2 = Direction.Normal;

        // ディレクトリを先頭に集めるときtrue
        private bool m_topDirectory = true;

        // 大文字小文字を区別するときtrue
        private bool m_capital = true;

        // 数値の大小を比較するときtrue
        private bool m_identifyNumber = true;

        //=========================================================================================
        // 機　能：コンストラクタ（デフォルト設定で初期化する）
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListSortMode() {
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
        // 機　能：値を設定する
        // 引　数：[in]src  転送元
        // 戻り値：なし
        //=========================================================================================
        public void SetSortMode(FileListSortMode src) {
            m_sortOrder1 = src.m_sortOrder1;
            m_sortOrder2 = src.m_sortOrder2;
            m_sortDirection1 = src.m_sortDirection1;
            m_sortDirection2 = src.m_sortDirection2;
            m_topDirectory = src.m_topDirectory;
            m_capital = src.m_capital;
            m_identifyNumber = src.m_identifyNumber;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileListSortMode obj1, FileListSortMode obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_sortOrder1 != obj2.m_sortOrder1) {
                return false;
            }
            if (obj1.m_sortOrder2 != obj2.m_sortOrder2) {
                return false;
            }
            if (obj1.m_sortDirection1 != obj2.m_sortDirection1) {
                return false;
            }
            if (obj1.m_sortDirection2 != obj2.m_sortDirection2) {
                return false;
            }
            if (obj1.m_topDirectory != obj2.m_topDirectory) {
                return false;
            }
            if (obj1.m_capital != obj2.m_capital) {
                return false;
            }
            if (obj1.m_identifyNumber != obj2.m_identifyNumber) {
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
        public static bool LoadSetting(SettingLoader loader, out FileListSortMode obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.FileListSortMode_FileListSortMode, SettingTagType.BeginObject, out fit);
            if (!success) {
                return success;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new FileListSortMode();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return success;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileListSortMode_FileListSortMode) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileListSortMode_SortOrder1) {
                    obj.m_sortOrder1 = MethodFromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileListSortMode_SortOrder2) {
                    obj.m_sortOrder2 = MethodFromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileListSortMode_DirectionReverse1) {
                    if (loader.BoolValue) {
                        obj.m_sortDirection1 = Direction.Reverse;
                    } else {
                        obj.m_sortDirection1 = Direction.Normal;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileListSortMode_DirectionReverse2) {
                    if (loader.BoolValue) {
                        obj.m_sortDirection2 = Direction.Reverse;
                    } else {
                        obj.m_sortDirection2 = Direction.Normal;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileListSortMode_TopDirectory) {
                    obj.m_topDirectory = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileListSortMode_Capital) {
                    obj.m_capital = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileListSortMode_IdentifyNumber) {
                    obj.m_identifyNumber = loader.BoolValue;
                }
            }
            obj.ModifySortMode();
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, FileListSortMode obj) {
            if (obj == null) {
                return true;
            }
            saver.StartObject(SettingTag.FileListSortMode_FileListSortMode);
            saver.AddString(SettingTag.FileListSortMode_SortOrder1, MethodToSerializedData(obj.m_sortOrder1));
            saver.AddString(SettingTag.FileListSortMode_SortOrder2, MethodToSerializedData(obj.m_sortOrder2));
            saver.AddBool(SettingTag.FileListSortMode_DirectionReverse1, (obj.m_sortDirection1 == Direction.Reverse));
            saver.AddBool(SettingTag.FileListSortMode_DirectionReverse2, (obj.m_sortDirection2 == Direction.Reverse));
            saver.AddBool(SettingTag.FileListSortMode_TopDirectory, obj.m_topDirectory);
            saver.AddBool(SettingTag.FileListSortMode_Capital, obj.m_capital);
            saver.AddBool(SettingTag.FileListSortMode_IdentifyNumber, obj.m_identifyNumber);
            saver.EndObject(SettingTag.FileListSortMode_FileListSortMode);
            return true;
        }

        //=========================================================================================
        // 機　能：シリアライズされたデータからソートキーを復元する
        // 引　数：[in]strMethod  シリアライズされたソートキー
        // 　　　　[out]method    復元されたソートキーを返す変数
        // 戻り値：復元できたときtrue
        //=========================================================================================
        public static Method MethodFromSerializedData(string strMethod) {
            Method method;
            if (strMethod == "NoSort") {
                method = Method.NoSort;
            } else if (strMethod == "FileName") {
                method = Method.FileName;
            } else if (strMethod == "Extension") {
                method = Method.Extension;
            } else if (strMethod == "DateTime") {
                method = Method.DateTime;
            } else if (strMethod == "FileSize") {
                method = Method.FileSize;
            } else if (strMethod == "Attribute") {
                method = Method.Attribute;
            } else {
                method = Method.NoSort;
            }
            return method;
        }
        
        //=========================================================================================
        // 機　能：ソート状態を正しい状態に補正する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ModifySortMode() {
            m_sortOrder2 = ModifySort2BySort1(m_sortOrder1, m_sortOrder2);
        }

        //=========================================================================================
        // 機　能：シリアライズされたデータからソートキーを復元する
        // 引　数：[in]method    ソートキー
        // 戻り値：シリアライズされたソートキー
        //=========================================================================================
        public static string MethodToSerializedData(Method method) {
            if (method == Method.NoSort) {
                return "NoSort";
            } else if (method == Method.FileName) {
                return "FileName";
            } else if (method == Method.Extension) {
                return "Extension";
            } else if (method == Method.DateTime) {
                return "DateTime";
            } else if (method == Method.FileSize) {
                return "FileSize";
            } else {
                return "Attribute";
            }
        }

        //=========================================================================================
        // 機　能：第1ソートキーが変更されたときの状態から第2ソートキーを補正する
        // 引　数：[in]sortMode1     第１ソートキー
        // 　　　　[in]sortMode2     第２ソートキー
        // 戻り値：なし
        //=========================================================================================
        public static Method ModifySort2BySort1(Method sortMode1, Method sortMode2) {
            if (sortMode1 == FileListSortMode.Method.NoSort) {
                if (sortMode2 != FileListSortMode.Method.NoSort) {
                    return Method.NoSort;
                }
            } else if (sortMode1 == sortMode2) {
                if (sortMode2 != FileListSortMode.Method.NoSort) {
                    return Method.NoSort;
                }
            }
            return sortMode2;
        }
        
        //=========================================================================================
        // 機　能：第2ソートキーが変更されたときの状態から第1ソートキーを補正する
        // 引　数：[in]sortMode1     第1ソートキー
        // 　　　　[in]sortMode2     第2ソートキー
        // 戻り値：なし
        //=========================================================================================
        public static Method ModifySort1BySort2(Method sortMode1, Method sortMode2) {
            if (sortMode1 == sortMode2) {
                if (sortMode1 == FileListSortMode.Method.NoSort) {
                    ;
                } else if (sortMode2 == FileListSortMode.Method.FileName) {
                    sortMode1 = FileListSortMode.Method.DateTime;
                } else {
                    sortMode1 = FileListSortMode.Method.FileName;
                }
            }
            return sortMode1;
        }

        //=========================================================================================
        // プロパティ：第1ソートキー
        //=========================================================================================
        public Method SortOrder1 {
            get {
                return m_sortOrder1;
            }
            set {
                m_sortOrder1 = value;
            }
        }

        //=========================================================================================
        // プロパティ：第2ソートキー
        //=========================================================================================
        public Method SortOrder2 {
            get {
                return m_sortOrder2;
            }
            set {
                m_sortOrder2 = value;
            }
        }

        //=========================================================================================
        // プロパティ：第1ソートキーのソート方向
        //=========================================================================================
        public Direction SortDirection1 {
            get {
                return m_sortDirection1;
            }
            set {
                m_sortDirection1 = value;
            }
        }

        //=========================================================================================
        // プロパティ：第2ソートキーのソート方向
        //=========================================================================================
        public Direction SortDirection2 {
            get {
                return m_sortDirection2;
            }
            set {
                m_sortDirection2 = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリを先頭に集めるときtrue
        //=========================================================================================
        public bool TopDirectory {
            get {
                return m_topDirectory;
            }
            set {
                m_topDirectory = value;
            }
        }

        //=========================================================================================
        // プロパティ：大文字小文字を区別するときtrue
        //=========================================================================================
        public bool Capital {
            get {
                return m_capital;
            }
            set {
                m_capital = value;
            }
        }

        //=========================================================================================
        // プロパティ：数値の大小を比較するときtrue
        //=========================================================================================
        public bool IdentifyNumber {
            get {
                return m_identifyNumber;
            }
            set {
                m_identifyNumber = value;
            }
        }

        //=========================================================================================
        // 列挙子：ソートの方法
        //=========================================================================================
        public enum Method {
            // ソートなし
            NoSort,
            // ファイル名
            FileName,
            // 拡張子順
            Extension,
            // 日付順
            DateTime,
            // サイズ順
            FileSize,
            // 属性順
            Attribute,
        }

        //=========================================================================================
        // 列挙子：ソートの方向
        //=========================================================================================
        public enum Direction {
            // 通常
            Normal,
            // 逆方向
            Reverse,
        }
    }
}
