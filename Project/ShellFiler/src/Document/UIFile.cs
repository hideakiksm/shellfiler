using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.FileTask.DataObject;
using ShellFiler.FileTask.Condition;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：ファイル一覧UIで扱うファイル1件分の情報
    //=========================================================================================
    public class UIFile : IFile, ICloneable {
        // 扱う対象ファイル
        private IFile m_delegate;

        // マークされた順番、マークされていないときは0
        private int m_markOrder;
        
        // ファイルのアイコンID
        private FileIconID m_fileIconId;

        // デフォルトのファイルのアイコンID（アイコンのロード前、破棄時にm_fileIconIdが読めないときの代替用）
        private FileIconID m_defaultFileIconId;

        // Cloneをサポート

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private UIFile() {
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]file  このクラスで表現するファイルの情報源
        // 戻り値：なし
        //=========================================================================================
        public UIFile(IFile file) {
            m_delegate = file;
            m_markOrder = 0;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            UIFile uiFile = new UIFile();
            uiFile.m_delegate = (IFile)(m_delegate.Clone());
            uiFile.m_markOrder = m_markOrder;
            uiFile.m_fileIconId = m_fileIconId;
            uiFile.m_defaultFileIconId = m_defaultFileIconId;
            return uiFile;
        }

        //=========================================================================================
        // 機　能：マーク状態を設定する（UIFileListからの設定専用）
        // 引　数：[in]markOrder  マークされた順番（0:マーク解除）
        // 戻り値：なし
        //=========================================================================================
        public void SetInternalMarked(int markOrder) {
            m_markOrder = markOrder;
        }

        //=========================================================================================
        // 機　能：このファイルがファイル条件に一致するかどうかを返す
        // 引　数：[in]condition   ファイル条件（条件がないときnull）
        // 　　　　[in]isPositive  正論理で比較するときtrue、負論理で比較するときfalse
        // 戻り値：条件に一致するファイルのときtrue
        //=========================================================================================
        public bool IsTargetFileWithCondition(CompareCondition condition, bool isPositive) {
            if (condition == null) {
                return true;
            }
            bool match = TargetConditionComparetor.IsTarget(condition, m_delegate, isPositive);
            return match;
        }

        //=========================================================================================
        // 機　能：リネーム用のファイル情報を作成する
        // 引　数：[in]fileSystem  対象のファイルシステム
        // 　　　　[in]fullFile    完全な属性情報を取得したファイル情報があるときはその値(SSHShell)、ないときはnull
        // 戻り値：リネーム用のファイル情報
        //=========================================================================================
        public RenameFileInfo CreateRenameFileInfo(FileSystemID fileSystem, IFile fullFile) {
            if (fullFile == null) {
                return RenameFileInfo.CreateRenameFileInfo(fileSystem, m_delegate);
            } else {
                return RenameFileInfo.CreateRenameFileInfo(fileSystem, fullFile);
            }
        }

        //=========================================================================================
        // 機　能：UIFileのリストをSimpleFileDirectoryPathのリストに変換する
        // 引　数：[in]dir         変換元のディレクトリ
        // 　　　　[in]uiFileList  変換元
        // 戻り値：変換結果
        //=========================================================================================
        public static List<SimpleFileDirectoryPath> UIFileListToSimpleFileList(string dir, List<UIFile> uiFileList) {
            List<SimpleFileDirectoryPath> resultList = new List<SimpleFileDirectoryPath>();
            foreach (UIFile uiFile in uiFileList) {
                resultList.Add(new SimpleFileDirectoryPath(dir + uiFile.FileName, uiFile.Attribute.IsDirectory, uiFile.Attribute.IsSymbolicLink));
            }
            return resultList;
        }

        //=========================================================================================
        // プロパティ：ファイル名（パス名は含まない）
        //=========================================================================================
        public string FileName {
            get {
                return m_delegate.FileName;
            }
        }

        //=========================================================================================
        // プロパティ：拡張子
        //=========================================================================================
        public string Extension {
            get {
                return m_delegate.Extension;
            }
        }

        //=========================================================================================
        // プロパティ：表示用拡張子の位置とする「.」の直後の文字のインデックス（-1:拡張子を分離表示しない）
        //=========================================================================================
        public int DisplayExtensionPos {
            get {
                return m_delegate.DisplayExtensionPos;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルの更新時刻
        //=========================================================================================
        public DateTime ModifiedDate {
            get {
                return m_delegate.ModifiedDate;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル属性
        //=========================================================================================
        public FileAttribute Attribute {
            get {
                return m_delegate.Attribute;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルサイズ
        //=========================================================================================
        public long FileSize {
            get {
                return m_delegate.FileSize;
            }
            set {
                m_delegate.FileSize = value;
            }
        }

        //=========================================================================================
        // プロパティ：デフォルトのサイズ順
        //=========================================================================================
        public int DefaultOrder {
            get {
                return m_delegate.DefaultOrder;
            }
            set {
                m_delegate.DefaultOrder = value;
            }
        }

        //=========================================================================================
        // プロパティ：マークされているときtrue
        //=========================================================================================
        public bool Marked {
            get {
                return (m_markOrder != 0);
            }
        }
        
        //=========================================================================================
        // プロパティ：マークされた順番
        //=========================================================================================
        public int MarkOrder {
            get {
                return m_markOrder;
            }
            set {
                m_markOrder = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルのアイコンID
        //=========================================================================================
        public FileIconID FileIconId {
            get {
                return m_fileIconId;
            }
            set {
                m_fileIconId = value;
            }
        }
  
        //=========================================================================================
        // プロパティ：デフォルトのファイルのアイコンID
        //=========================================================================================
        public FileIconID DefaultFileIconId {
            get {
                return m_defaultFileIconId;
            }
            set {
                m_defaultFileIconId = value;
            }
        }
    }
}
