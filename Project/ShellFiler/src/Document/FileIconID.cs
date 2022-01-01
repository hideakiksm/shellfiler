using System;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document.Setting;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：ファイルアイコンのID
    //=========================================================================================
    public struct FileIconID {
        // 管理外のID
        public static readonly FileIconID NullId = new FileIconID(FileIconType.DefaultIcon, FileListViewIconSize.IconSizeNull, UIFileListId.NullId, 0);

        // 次に発行するID
        private static int s_nextIdDefault = 1;

        // 次に発行するID（ファイル一覧）
        private static int s_nextIdFileList = 1;
        
        // アイコンの種類
        private FileIconType m_iconType;

        // アイコンのサイズ
        private FileListViewIconSize m_iconSize;

        // ファイル一覧のID
        private UIFileListId m_fileListId;

        // ID
        private int m_id;
        
        //=========================================================================================
        // 機　能：新しいアイコンIDを振り出す
        // 引　数：[in]type       アイコンの種類
        // 　　　　[in]iconSize   アイコンのサイズ（デフォルトアイコンのときIconSizeNull）
        // 　　　　[in]fileListId ファイル一覧のID（DefaultIconのときはUIFileListId.NullId）
        // 戻り値：アイコンID
        //=========================================================================================
        public static FileIconID NextId(FileIconType type, FileListViewIconSize iconSize, UIFileListId fileListId) {
            switch (type) {
                case FileIconType.DefaultIcon:
                    return new FileIconID(type, iconSize, UIFileListId.NullId, Interlocked.Add(ref s_nextIdDefault, 1));
                case FileIconType.FileListIcon:
                    return new FileIconID(type, iconSize, fileListId, Interlocked.Add(ref s_nextIdFileList, 1));
                default:
                    return FileIconID.NullId;
            }
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]type       アイコンの種類
        // 　　　　[in]iconSize   アイコンのサイズ（デフォルトアイコンのときIconSizeNull）
        // 　　　　[in]fileListId ファイル一覧のID
        // 　　　　[in]id         設定するID
        // 戻り値：なし
        //=========================================================================================
        private FileIconID(FileIconType type, FileListViewIconSize iconSize, UIFileListId fileListId, int id) {
            m_iconType = type;
            m_iconSize = iconSize;
            m_fileListId = fileListId;
            m_id = id;
        }

        //=========================================================================================
        // 機　能：比較演算子
        // 引　数：[in]a  比較対象1
        // 　　　　[in]b  比較対象2
        // 戻り値：等しいときtrue
        //=========================================================================================
        public static bool operator==(FileIconID a, FileIconID b) {
            return ((a.m_iconType == b.m_iconType) && (a.m_fileListId == b.m_fileListId) && (a.m_id == b.m_id));
        }

        //=========================================================================================
        // 機　能：比較演算子
        // 引　数：[in]a  比較対象1
        // 　　　　[in]b  比較対象2
        // 戻り値：等しくないときtrue
        //=========================================================================================
        public static bool operator!=(FileIconID a, FileIconID b) {
            return ((a.m_iconType != b.m_iconType) || (a.m_fileListId != b.m_fileListId) || (a.m_id != b.m_id));
        }

        //=========================================================================================
        // 機　能：比較する
        // 引　数：[in]other  比較対象
        // 戻り値：等しいときtrue
        //=========================================================================================
        public override bool Equals(object other) {
            FileIconID b = (FileIconID)other;
            return ((m_iconType == b.m_iconType) && (m_fileListId == b.m_fileListId) && (this.m_id == b.m_id));
        }

        //=========================================================================================
        // 機　能：ハッシュコードを計算する
        // 引　数：なし
        // 戻り値：ハッシュコード
        //=========================================================================================
        public override int GetHashCode() {
            return m_id;
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のID
        //=========================================================================================
        public UIFileListId FileListId {
            get {
                return m_fileListId;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンのID値
        //=========================================================================================
        public int IdValue {
            get {
                return m_id;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンの種類
        //=========================================================================================
        public FileIconType IconType {
            get {
                return m_iconType;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンのサイズ
        //=========================================================================================
        public FileListViewIconSize FileListViewIconSize {
            get {
                return m_iconSize;
            }
        }

        //=========================================================================================
        // 列挙子：アイコンの種類
        //=========================================================================================
        public enum FileIconType {
            DefaultIcon,                // 定義済みアイコン
            FileListIcon,               // ファイル一覧のアイコン
        }
    }
}
