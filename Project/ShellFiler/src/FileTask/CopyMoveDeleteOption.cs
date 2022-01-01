using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileTask.DataObject;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log;
using ShellFiler.Util;

namespace ShellFiler.FileTask {

    //=========================================================================================
    // クラス：ファイルのコピー、移動のオプション
    //=========================================================================================
    public class CopyMoveDeleteOption {
        // 同名ファイルを発見したときの動作（コピー:○、移動:○、削除:null、ミラーコピー:null、２重化:○、フラット:○）
        private SameFileOperation m_sameFileOperation;

        // 転送時に適用するフィルター（コピー:△、移動:null、削除:null、ミラーコピー:null、２重化:null、フラット:△）
        private FileFilterTransferSetting m_fileFilter;
        
        // 実行条件（コピー:△、移動:△、削除:△、ミラーコピー:null、２重化:null、フラット:△）
        private CompareCondition m_transferCondition;

        // 属性コピーのモード（コピー:○、移動:○、削除:null、ミラーコピー:null、２重化:○、フラット:○）
        private AttributeSetMode m_attributeSetMode;

        // ミラーコピーの設定（コピー:null、移動:null、削除:null、ミラーコピー:○、２重化:null、フラット:null）
        private MirrorCopyOption m_mirrorCopyOption;

        // ２重化の設定（コピー:null、移動:null、削除:null、ミラーコピー:null、２重化:○、フラット:null）
        private DuplicateFileInfo m_duplicateFileInfo;

        // フォルダ階層をフラットにするときtrue（コピー:null、移動:null、削除:null、ミラーコピー:null、２重化:○、フラット:true）
        private bool m_unwrapFolder;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]sameFileOption  同名ファイルを発見したときの動作
        // 　　　　[in]fileFilter      転送時に適用するフィルター（フィルターを使用しない通常コピーのときnull）
        // 　　　　[in]transCond       ファイル操作の実行条件（条件がないときnull）
        // 　　　　[in]attrSetMode     属性コピーのモード（条件がないときnull）
        // 　　　　[in]mirrorOption    ミラーコピーの設定（条件がないときnull）
        // 　　　　[in]duplicateInfo   ２重化の設定（条件がないときnull）
        //         [in]unwrapFolder    フォルダ階層をフラットにするときtrue
        // 戻り値：なし
        //=========================================================================================
        public CopyMoveDeleteOption(SameFileOperation sameFileOption, FileFilterTransferSetting fileFilter, CompareCondition transCond, AttributeSetMode attrSetMode, MirrorCopyOption mirrorOption, DuplicateFileInfo duplicateInfo, bool unwrapFolder) {
            m_sameFileOperation = sameFileOption;
            m_fileFilter = fileFilter;
            m_transferCondition = transCond;
            m_attributeSetMode = attrSetMode;
            m_mirrorCopyOption = mirrorOption;
            m_duplicateFileInfo = duplicateInfo;
            m_unwrapFolder = unwrapFolder;
        }

        //=========================================================================================
        // 機　能：同名ファイルの転送方法のデフォルトを返す
        // 引　数：[in]sameFileOption  同名ファイルを発見したときの動作
        // 　　　　[in]fileFilter   転送時に適用するフィルター（フィルターを使用しない通常コピーのときnull）
        // 　　　　[in]transCond    ファイル操作の実行条件（条件がないときnull）
        // 　　　　[in]setAttrMode  属性コピーのモード
        // 戻り値：なし
        //=========================================================================================
        public static SameFileOperation CreateDefaultSameFileOperation(FileSystemID fileSystemId) {
            SameFileOperation sameFile = SameFileOperation.CreateWithDefaultConfig(fileSystemId);
            sameFile.AllApply = false;
            return sameFile;
        }

        //=========================================================================================
        // プロパティ：同名ファイルを発見したときの動作
        //=========================================================================================
        public SameFileOperation SameFileOperation {
            get {
                return m_sameFileOperation;
            }
            set {
                m_sameFileOperation = value;
            }
        }

        //=========================================================================================
        // プロパティ：転送時に適用するフィルター
        //=========================================================================================
        public FileFilterTransferSetting FileFilter {
            get {
                return m_fileFilter;
            }
        }

        //=========================================================================================
        // プロパティ：実行条件（条件がないときnull）
        //=========================================================================================
        public CompareCondition TransferCondition {
            get {
                return m_transferCondition;
            }
        }

        //=========================================================================================
        // プロパティ：属性コピーのモード
        //=========================================================================================
        public AttributeSetMode AttributeSetMode {
            get {
                return m_attributeSetMode;
            }
        }

        //=========================================================================================
        // プロパティ：ミラーコピーの設定
        //=========================================================================================
        public MirrorCopyOption MirrorCopyOption {
            get {
                return m_mirrorCopyOption;
            }
        }

        //=========================================================================================
        // プロパティ：２重化の設定
        //=========================================================================================
        public DuplicateFileInfo DuplicateFileInfo {
            get {
                return m_duplicateFileInfo;
            }
        }

        //=========================================================================================
        // プロパティ：フォルダ階層をフラットにするときtrue
        //=========================================================================================
        public bool UnwrapFolder {
            get {
                return m_unwrapFolder;
            }
        }
    }
}
