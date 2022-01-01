using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // クラス：ファイル情報一括編集となる対象のファイル情報
    //=========================================================================================
    public class RenameSelectedFileInfo {
        // デフォルトの拡張子
        public const string DEFAULT_EXTENSION = "dat";

        // 対象ファイルシステムがWindowsのときのリネーム情報
        private WindowsRenameInfo m_windowsInfo;

        // 対象ファイルシステムがSSHのときのリネーム情報
        private SSHRenameInfo m_sshInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]file  リネーム情報
        // 戻り値：なし
        //=========================================================================================
        public RenameSelectedFileInfo(WindowsRenameInfo info) {
            m_windowsInfo = info;
            m_sshInfo = null;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]file  リネーム情報
        // 戻り値：なし
        //=========================================================================================
        public RenameSelectedFileInfo(SSHRenameInfo info) {
            m_windowsInfo = null;
            m_sshInfo = info;
        }

        //=========================================================================================
        // 機　能：ファイル情報の変更が発生しているかどうかを確認する
        // 引　数：なし
        // 戻り値：ファイル情報の変更が必要なときtrue
        //=========================================================================================
        public bool IsModified() {
            if (m_windowsInfo != null) {
                return m_windowsInfo.IsModified();
            } else {
                return m_sshInfo.IsModified();
            }
        }

        //=========================================================================================
        // 機　能：フォルダの処理方法を返す
        // 引　数：なし
        // 戻り値：フォルダの処理方法
        //=========================================================================================
        public TargetFolder GetTargetDirectoryMode() {
            if (m_windowsInfo != null) {
                return m_windowsInfo.TargetFolder;
            } else {
                return m_sshInfo.TargetFolder;
            }
        }

        //=========================================================================================
        // 機　能：リネームダイアログを作成する
        // 引　数：[in]fileSystemID  扱うファイルシステムのID
        // 　　　　[in]fileList      ファイル一覧
        // 戻り値：リネームダイアログ
        //=========================================================================================
        public static IRenameSelectedFileDialog CreateRenameDialog(FileSystemID fileSystemID, UIFileList fileList) {
            IRenameSelectedFileDialog result = null;
            if (FileSystemID.IsWindows(fileSystemID)) {
                result = new RenameSelectedFilesWindowsDialog(fileList);
            } else if (FileSystemID.IsSSH(fileSystemID)) {
                result = new RenameSelectedFilesSSHDialog(fileList, fileSystemID == FileSystemID.SSHShell);
            } else if (FileSystemID.IsVirtual(fileSystemID)) {
                result = null;
            } else {
                FileSystemID.NotSupportError(fileSystemID);
            }
            return result;
        }

        //=========================================================================================
        // プロパティ：Windows用のリネーム情報
        //=========================================================================================
        public WindowsRenameInfo WindowsInfo {
            get {
                return m_windowsInfo;
            }
        }

        //=========================================================================================
        // プロパティ：SSH用のリネーム情報
        //=========================================================================================
        public SSHRenameInfo SSHInfo {
            get {
                return m_sshInfo;
            }
        }

        //=========================================================================================
        // クラス：リネームダイアログを表示するためのインターフェース
        //=========================================================================================
        public interface IRenameSelectedFileDialog {

            //=========================================================================================
            // 機　能：編集ダイアログを表示する
            // 引　数：[in]parent    親ウィンドウ
            // 戻り値：ダイアログの結果
            //=========================================================================================
            DialogResult ShowRenameDialog(Form parent);

            //=========================================================================================
            // プロパティ：編集対象のリネーム情報（編集結果）
            //=========================================================================================
            RenameSelectedFileInfo RenameSelectedFileInfo {
                get;
            }
        }

        //=========================================================================================
        // クラス：ファイル名のリネーム方法
        //=========================================================================================
        public enum RenameMode {
            None,                   // そのまま
            ToUpper,                // 大文字に変更
            ToLower,                // 小文字に変更
            ToCapital,              // 先頭だけ大文字に変更
            Specify,                // 連番の指定/指定の拡張子に変更
        }

        //=========================================================================================
        // クラス：フォルダを対象とするかどうかのモード
        //=========================================================================================
        public class TargetFolder {
            public static readonly TargetFolder ExcludeSubfolder   = new TargetFolder(true, false);     // フォルダ自身だけ
            public static readonly TargetFolder SubfolderFileOnly  = new TargetFolder(false, true);     // サブフォルダのファイルだけ
            public static readonly TargetFolder SubfolderAndFolder = new TargetFolder(true, true);      // フォルダ自身とサブフォルダの両方

            // フォルダ自身を対象とするときtrue
            private bool m_modifyFolder;

            // サブフォルダのファイルを対象とするときtrue
            private bool m_modifyFolderFile;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]modifyFolder     フォルダ自身を対象とするときtrue
            // 　　　　[in]modifySubfolder  サブフォルダのファイルを対象とするときtrue
            // 戻り値：なし
            //=========================================================================================
            public TargetFolder(bool modifyFolder, bool modifySubfolder) {
                m_modifyFolder = modifyFolder;
                m_modifyFolderFile = modifySubfolder;
            }

            //=========================================================================================
            // プロパティ：フォルダ自身を対象とするときtrue
            //=========================================================================================
            public bool ModifyFolder {
                get {
                    return m_modifyFolder;
                }
            }

            //=========================================================================================
            // プロパティ：サブフォルダのファイルを対象とするときtrue
            //=========================================================================================
            public bool ModifySubfolderFile {
                get {
                    return m_modifyFolderFile;
                }
            }
        }

        //=========================================================================================
        // クラス：ファイル名部分のリネーム方法
        //=========================================================================================
        public class ModifyFileNameInfo {
            // ファイル名主部のリネーム方法
            private RenameMode m_renameModeFileBody;
            
            // ファイル名主部の連番指定時の情報（Specify以外のときはnull）
            private RenameNumberingInfo m_renameFileBodyNumbering = null;

            // ファイル名拡張子のリネーム方法
            private RenameMode m_renameModeFileExt;

            // ファイル名拡張子の指定拡張子（Specify以外のときはnull）
            private string m_renameFileExtString;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public ModifyFileNameInfo() {
            }

            //=========================================================================================
            // 機　能：ファイル情報の変更が発生しているかどうかを確認する
            // 引　数：なし
            // 戻り値：ファイル情報の変更が必要なときtrue
            //=========================================================================================
            public bool IsModified() {
                if (m_renameModeFileBody != RenameMode.None) {
                    return true;
                }
                if (m_renameModeFileExt != RenameMode.None) {
                    return true;
                }
                return false;
            }

            //=========================================================================================
            // プロパティ：ファイル名主部のリネーム方法
            //=========================================================================================
            public RenameMode RenameModeFileBody {
                get {
                    return m_renameModeFileBody;
                }
                set {
                    m_renameModeFileBody = value;
                }
            }
            
            //=========================================================================================
            // プロパティ：ファイル名主部の連番指定時の情報（Specify以外のときはnull）
            //=========================================================================================
            public RenameNumberingInfo RenameFileBodyNumbering {
                get {
                    return m_renameFileBodyNumbering;
                }
                set {
                    m_renameFileBodyNumbering = value;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル名拡張子のリネーム方法
            //=========================================================================================
            public RenameMode RenameModeFileExt {
                get {
                    return m_renameModeFileExt;
                }
                set {
                    m_renameModeFileExt = value;
                }
            }

            //=========================================================================================
            // プロパティ：ファイル名拡張子の指定拡張子（Specify以外のときはnull）
            //=========================================================================================
            public string RenameFileExtString {
                get {
                    return m_renameFileExtString;
                }
                set {
                    m_renameFileExtString = value;
                }
            }
        }

        //=========================================================================================
        // クラス：Window用のリネーム対象のファイル情報
        //=========================================================================================
        public class WindowsRenameInfo {
            // ファイル名部分のリネーム方法
            private ModifyFileNameInfo m_modifyFileNameInfo;

            // 属性を読み取り専用にするときtrue（変更しないときnull）
            private BooleanFlag m_attributeReadonly = null;

            // 属性を隠しにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeHidden = null;

            // 属性をアーカイブにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeArchive = null;

            // 属性をシステムにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeSystem = null;

            // 更新日時の日付を変えるとき、その日付（変更しないときnull）
            private DateInfo m_updateDate = null;

            // 更新日時の時刻を変えるとき、その時刻（変更しないときnull）
            private TimeInfo m_updateTime = null;

            // 作成日時の日付を変えるとき、その日付（変更しないときnull）
            private DateInfo m_createDate = null;

            // 作成日時の時刻を変えるとき、その時刻（変更しないときnull）
            private TimeInfo m_createTime = null;

            // アクセス日時の日付を変えるとき、その日付（変更しないときnull）
            private DateInfo m_accessDate = null;

            // アクセス日時の時刻を変えるとき、その時刻（変更しないときnull）
            private TimeInfo m_accessTime = null;

            // フォルダの扱い
            private TargetFolder m_targetFolder = null;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public WindowsRenameInfo() {
            }

            //=========================================================================================
            // 機　能：ファイル情報の変更が発生しているかどうかを確認する
            // 引　数：なし
            // 戻り値：ファイル情報の変更が必要なときtrue
            //=========================================================================================
            public bool IsModified() {
                if (m_modifyFileNameInfo.IsModified()) {
                    return true;
                }
                if (m_attributeReadonly != null) {
                    return true;
                }
                if (m_attributeHidden != null) {
                    return true;
                }
                if (m_attributeArchive != null) {
                    return true;
                }
                if (m_attributeSystem != null) {
                    return true;
                }
                if (m_updateDate != null) {
                    return true;
                }
                if (m_updateTime != null) {
                    return true;
                }
                if (m_createDate != null) {
                    return true;
                }
                if (m_createTime != null) {
                    return true;
                }
                if (m_accessDate != null) {
                    return true;
                }
                if (m_accessTime != null) {
                    return true;
                }
                return false;
            }

            //=========================================================================================
            // プロパティ：ファイル名部分のリネーム方法
            //=========================================================================================
            public ModifyFileNameInfo ModifyFileNameInfo {
                get {
                    return m_modifyFileNameInfo;
                }
                set {
                    m_modifyFileNameInfo = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性を読み取り専用にするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeReadonly {
                get {
                    return m_attributeReadonly;
                }
                set {
                    m_attributeReadonly = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性を隠しにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeHidden {
                get {
                    return m_attributeHidden;
                }
                set {
                    m_attributeHidden = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性をアーカイブにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeArchive {
                get {
                    return m_attributeArchive;
                }
                set {
                    m_attributeArchive = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性をシステムにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeSystem {
                get {
                    return m_attributeSystem;
                }
                set {
                    m_attributeSystem = value;
                }
            }

            //=========================================================================================
            // プロパティ：更新日時の日付を変えるとき、その日付（変更しないときnull）
            //=========================================================================================
            public DateInfo UpdateDate {
                get {
                    return m_updateDate;
                }
                set {
                    m_updateDate = value;
                }
            }

            //=========================================================================================
            // プロパティ：更新日時の時刻を変えるとき、その時刻（変更しないときnull）
            //=========================================================================================
            public TimeInfo UpdateTime {
                get {
                    return m_updateTime;
                }
                set {
                    m_updateTime = value;
                }
            }

            //=========================================================================================
            // プロパティ：作成日時の日付を変えるとき、その日付（変更しないときnull）
            //=========================================================================================
            public DateInfo CreateDate {
                get {
                    return m_createDate;
                }
                set {
                    m_createDate = value;
                }
            }

            //=========================================================================================
            // プロパティ：作成日時の時刻を変えるとき、その時刻（変更しないときnull）
            //=========================================================================================
            public TimeInfo CreateTime {
                get {
                    return m_createTime;
                }
                set {
                    m_createTime = value;
                }
            }

            //=========================================================================================
            // プロパティ：アクセス日時の日付を変えるとき、その日付（変更しないときnull）
            //=========================================================================================
            public DateInfo AccessDate {
                get {
                    return m_accessDate;
                }
                set {
                    m_accessDate = value;
                }
            }

            //=========================================================================================
            // プロパティ：アクセス日時の時刻を変えるとき、その時刻（変更しないときnull）
            //=========================================================================================
            public TimeInfo AccessTime {
                get {
                    return m_accessTime;
                }
                set {
                    m_accessTime = value;
                }
            }

            //=========================================================================================
            // プロパティ：フォルダの扱い
            //=========================================================================================
            public TargetFolder TargetFolder {
                get {
                    return m_targetFolder;
                }
                set {
                    m_targetFolder = value;
                }
            }
        }

        //=========================================================================================
        // クラス：SSH用のリネーム対象のファイル情報
        //=========================================================================================
        public class SSHRenameInfo {
            // ファイル名部分のリネーム方法
            private ModifyFileNameInfo m_modifyFileNameInfo;

            // 属性の所有者読み込みをONにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeOwnerRead = null;

            // 属性の所有者書き込みをONにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeOwnerWrite = null;

            // 属性の所有者実行をONにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeOwnerExecute = null;

            // 属性のグループ読み込みをONにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeGroupRead = null;

            // 属性のグループ書き込みをONにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeGroupWrite = null;

            // 属性のグループ実行をONにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeGroupExecute = null;

            // 属性の他人読み込みをONにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeOtherRead = null;

            // 属性の他人書き込みをONにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeOtherWrite = null;

            // 属性の他人実行をONにするときtrue（変更しないときnull）
            private BooleanFlag m_attributeOtherExecute = null;

            // 更新日時の日付を変えるとき、その日付（変更しないときnull）
            private DateInfo m_updateDate = null;

            // 更新日時の時刻を変えるとき、その時刻（変更しないときnull）
            private TimeInfo m_updateTime = null;

            // アクセス日時の日付を変えるとき、その日付（変更しないときnull）
            private DateInfo m_accessDate = null;

            // アクセス日時の時刻を変えるとき、その時刻（変更しないときnull）
            private TimeInfo m_accessTime = null;

            // オーナー（変更しないときnull）
            private string m_owner = null;

            // グループ（変更しないときnull）
            private string m_group = null;

            // フォルダの扱い
            private TargetFolder m_targetFolder = null;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public SSHRenameInfo() {
            }

            //=========================================================================================
            // 機　能：ファイル情報の変更が発生しているかどうかを確認する
            // 引　数：なし
            // 戻り値：ファイル情報の変更が必要なときtrue
            //=========================================================================================
            public bool IsModified() {
                if (m_modifyFileNameInfo.IsModified()) {
                    return true;
                }
                if (m_attributeOwnerRead != null) {
                    return true;
                }
                if (m_attributeOwnerWrite != null) {
                    return true;
                }
                if (m_attributeOwnerExecute != null) {
                    return true;
                }
                if (m_attributeGroupRead != null) {
                    return true;
                }
                if (m_attributeGroupWrite != null) {
                    return true;
                }
                if (m_attributeGroupExecute != null) {
                    return true;
                }
                if (m_attributeOtherRead != null) {
                    return true;
                }
                if (m_attributeOtherWrite != null) {
                    return true;
                }
                if (m_attributeOtherExecute != null) {
                    return true;
                }
                if (m_updateDate != null) {
                    return true;
                }
                if (m_updateTime != null) {
                    return true;
                }
                if (m_accessDate != null) {
                    return true;
                }
                if (m_accessTime != null) {
                    return true;
                }
                if (m_owner != null) {
                    return true;
                }
                if (m_group != null) {
                    return true;
                }
                return false;
            }

            //=========================================================================================
            // 機　能：パーミッションを変更する
            // 引　数：[in]permissions  元のパーミッション
            // 　　　　[in]renameInfo   パーミッションの設定を読み出すリネーム後の情報
            // 戻り値：設定するパーミッション
            //=========================================================================================
            public static int ModifyPermissions(int permissions, RenameSelectedFileInfo.SSHRenameInfo renameInfo) {
                // 所有者
                if (renameInfo.AttributeOwnerRead != null) {
                    if (renameInfo.AttributeOwnerRead.Value == true) {
                        permissions |= FileAttribute.S_IRUSR;
                    } else {
                        permissions &= ~FileAttribute.S_IRUSR;
                    }
                }
                if (renameInfo.AttributeOwnerWrite != null) {
                    if (renameInfo.AttributeOwnerWrite.Value == true) {
                        permissions |= FileAttribute.S_IWUSR;
                    } else {
                        permissions &= ~FileAttribute.S_IWUSR;
                    }
                }
                if (renameInfo.AttributeOwnerExecute != null) {
                    if (renameInfo.AttributeOwnerExecute.Value == true) {
                        permissions |= FileAttribute.S_IXUSR;
                    } else {
                        permissions &= ~FileAttribute.S_IXUSR;
                    }
                }

                // グループ
                if (renameInfo.AttributeGroupRead != null) {
                    if (renameInfo.AttributeGroupRead.Value == true) {
                        permissions |= FileAttribute.S_IRGRP;
                    } else {
                        permissions &= ~FileAttribute.S_IRGRP;
                    }
                }
                if (renameInfo.AttributeGroupWrite != null) {
                    if (renameInfo.AttributeGroupWrite.Value == true) {
                        permissions |= FileAttribute.S_IWGRP;
                    } else {
                        permissions &= ~FileAttribute.S_IWGRP;
                    }
                }
                if (renameInfo.AttributeGroupExecute != null) {
                    if (renameInfo.AttributeGroupExecute.Value == true) {
                        permissions |= FileAttribute.S_IXGRP;
                    } else {
                        permissions &= ~FileAttribute.S_IXGRP;
                    }
                }

                // 他人
                if (renameInfo.AttributeOtherRead != null) {
                    if (renameInfo.AttributeOtherRead.Value == true) {
                        permissions |= FileAttribute.S_IROTH;
                    } else {
                        permissions &= ~FileAttribute.S_IROTH;
                    }
                }
                if (renameInfo.AttributeOtherWrite != null) {
                    if (renameInfo.AttributeOtherWrite.Value == true) {
                        permissions |= FileAttribute.S_IWOTH;
                    } else {
                        permissions &= ~FileAttribute.S_IWOTH;
                    }
                }
                if (renameInfo.AttributeOtherExecute != null) {
                    if (renameInfo.AttributeOtherExecute.Value == true) {
                        permissions |= FileAttribute.S_IXOTH;
                    } else {
                        permissions &= ~FileAttribute.S_IXOTH;
                    }
                }

                return permissions;
            }

            //=========================================================================================
            // プロパティ：ファイル名部分のリネーム方法
            //=========================================================================================
            public ModifyFileNameInfo ModifyFileNameInfo {
                get {
                    return m_modifyFileNameInfo;
                }
                set {
                    m_modifyFileNameInfo = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性の所有者読み込みをONにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeOwnerRead {
                get {
                    return m_attributeOwnerRead;
                }
                set {
                    m_attributeOwnerRead = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性の所有者書き込みをONにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeOwnerWrite {
                get {
                    return m_attributeOwnerWrite;
                }
                set {
                    m_attributeOwnerWrite = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性の所有者実行をONにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeOwnerExecute {
                get {
                    return m_attributeOwnerExecute;
                }
                set {
                    m_attributeOwnerExecute = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性のグループ読み込みをONにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeGroupRead {
                get {
                    return m_attributeGroupRead;
                }
                set {
                    m_attributeGroupRead = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性のグループ書き込みをONにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeGroupWrite {
                get {
                    return m_attributeGroupWrite;
                }
                set {
                    m_attributeGroupWrite = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性のグループ実行をONにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeGroupExecute {
                get {
                    return m_attributeGroupExecute;
                }
                set {
                    m_attributeGroupExecute = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性の他人読み込みをONにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeOtherRead {
                get {
                    return m_attributeOtherRead;
                }
                set {
                    m_attributeOtherRead = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性の他人書き込みをONにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeOtherWrite {
                get {
                    return m_attributeOtherWrite;
                }
                set {
                    m_attributeOtherWrite = value;
                }
            }

            //=========================================================================================
            // プロパティ：属性の他人実行をONにするときtrue（変更しないときnull）
            //=========================================================================================
            public BooleanFlag AttributeOtherExecute {
                get {
                    return m_attributeOtherExecute;
                }
                set {
                    m_attributeOtherExecute = value;
                }
            }

            //=========================================================================================
            // プロパティ：更新日時の日付を変えるとき、その日付（変更しないときnull）
            //=========================================================================================
            public DateInfo UpdateDate {
                get {
                    return m_updateDate;
                }
                set {
                    m_updateDate = value;
                }
            }

            //=========================================================================================
            // プロパティ：更新日時の時刻を変えるとき、その時刻（変更しないときnull）
            //=========================================================================================
            public TimeInfo UpdateTime {
                get {
                    return m_updateTime;
                }
                set {
                    m_updateTime = value;
                }
            }

            //=========================================================================================
            // プロパティ：アクセス日時の日付を変えるとき、その日付（変更しないときnull）
            //=========================================================================================
            public DateInfo AccessDate {
                get {
                    return m_accessDate;
                }
                set {
                    m_accessDate = value;
                }
            }

            //=========================================================================================
            // プロパティ：アクセス日時の時刻を変えるとき、その時刻（変更しないときnull）
            //=========================================================================================
            public TimeInfo AccessTime {
                get {
                    return m_accessTime;
                }
                set {
                    m_accessTime = value;
                }
            }

            //=========================================================================================
            // プロパティ：オーナー（変更しないときnull）
            //=========================================================================================
            public string Owner {
                get {
                    return m_owner;
                }
                set {
                    m_owner = value;
                }
            }

            //=========================================================================================
            // プロパティ：グループ（変更しないときnull）
            //=========================================================================================
            public string Group {
                get {
                    return m_group;
                }
                set {
                    m_group = value;
                }
            }

            //=========================================================================================
            // プロパティ：フォルダの扱い
            //=========================================================================================
            public TargetFolder TargetFolder {
                get {
                    return m_targetFolder;
                }
                set {
                    m_targetFolder = value;
                }
            }
        }
    }
}
