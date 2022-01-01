using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.UI;

namespace ShellFiler.FileTask.Management {

    //=========================================================================================
    // クラス：バックグラウンドタスクの種類
    //=========================================================================================
    public class BackgroundTaskType {
        // すべての種類
        private static List<BackgroundTaskType> s_typeList = new List<BackgroundTaskType>();

        public static readonly BackgroundTaskType Copy               = new BackgroundTaskType( 0, Resources.DlgTaskMan_BackgroundTaskCopy,               IconImageListID.FileList_Copy);
        public static readonly BackgroundTaskType Move               = new BackgroundTaskType( 1, Resources.DlgTaskMan_BackgroundTaskMove,               IconImageListID.FileList_Move);
        public static readonly BackgroundTaskType Delete             = new BackgroundTaskType( 2, Resources.DlgTaskMan_BackgroundTaskDelete,             IconImageListID.FileList_Delete);
        public static readonly BackgroundTaskType DeleteNoRecycle    = new BackgroundTaskType( 3, Resources.DlgTaskMan_BackgroundTaskDelete,             IconImageListID.FileList_Delete);
        public static readonly BackgroundTaskType MirrorCopy         = new BackgroundTaskType( 4, Resources.DlgTaskMan_BackgroundTaskMirrorCopy,         IconImageListID.FileList_MirrorCopy);
        public static readonly BackgroundTaskType MakeDir            = new BackgroundTaskType( 5, Resources.DlgTaskMan_BackgroundTaskMakeDir,            IconImageListID.FileList_MakeDirectory);
        public static readonly BackgroundTaskType Rename             = new BackgroundTaskType( 6, Resources.DlgTaskMan_BackgroundTaskRename,             IconImageListID.FileList_Rename);
        public static readonly BackgroundTaskType RenameSelected     = new BackgroundTaskType( 7, Resources.DlgTaskMan_BackgroundTaskRenameSelected,     IconImageListID.FileList_RenameSelected);
        public static readonly BackgroundTaskType RetrieveFolderSize = new BackgroundTaskType( 8, Resources.DlgTaskMan_BackgroundTaskRetrieveFolderSize, IconImageListID.FileList_RetrieveFolderSize);
        public static readonly BackgroundTaskType Retrieve           = new BackgroundTaskType( 9, Resources.DlgTaskMan_BackgroundTaskRetrieve,           IconImageListID.FileList_FileViewer);
        public static readonly BackgroundTaskType ShellExecute       = new BackgroundTaskType(10, Resources.DlgTaskMan_BackgroundTaskShellExecute,       IconImageListID.FileList_ShellExecute);
        public static readonly BackgroundTaskType LocalExecute       = new BackgroundTaskType(11 ,Resources.DlgTaskMan_BackgroundTaskLocalExecute,       IconImageListID.FileList_ShellExecute);
        public static readonly BackgroundTaskType LocalExtract       = new BackgroundTaskType(12, Resources.DlgTaskMan_BackgroundTaskLocalExtract,       IconImageListID.FileList_Extract);
        public static readonly BackgroundTaskType LocalArchive       = new BackgroundTaskType(13, Resources.DlgTaskMan_BackgroundTaskLocalArchive,       IconImageListID.FileList_Archive);
        public static readonly BackgroundTaskType LocalUpload        = new BackgroundTaskType(14, Resources.DlgTaskMan_BackgroundTaskLocalUpload,        IconImageListID.FileList_ShellExecute);
        public static readonly BackgroundTaskType CreateShortcut     = new BackgroundTaskType(15, Resources.DlgTaskMan_BackgroundTaskCreateShortcut,     IconImageListID.FileList_Shortcut);
        public static readonly BackgroundTaskType GraphicsViewer     = new BackgroundTaskType(16, Resources.DlgTaskMan_BackgroundTaskGraphicsViewer,     IconImageListID.FileList_GraphicsViewer);
        public static readonly BackgroundTaskType DuplicateFile      = new BackgroundTaskType(17, Resources.DlgTaskMan_BackgroundTaskDuplicateFile,      IconImageListID.FileList_DuplicateFile);
        public static readonly BackgroundTaskType CombineFile        = new BackgroundTaskType(18, Resources.DlgTaskMan_BackgroundTaskCombineFile,        IconImageListID.FileList_CombineFile);
        public static readonly BackgroundTaskType SplitFile          = new BackgroundTaskType(19, Resources.DlgTaskMan_BackgroundTaskSplitFile,          IconImageListID.FileList_SplitFile);
        public static readonly BackgroundTaskType GitAdd             = new BackgroundTaskType(20, Resources.DlgTaskMan_BackgroundTaskGitAdd,             IconImageListID.FileList_Rename);
        public static readonly BackgroundTaskType GitMove            = new BackgroundTaskType(21, Resources.DlgTaskMan_BackgroundTaskGitMove,            IconImageListID.FileList_Move);
        public static readonly BackgroundTaskType GitRename          = new BackgroundTaskType(22, Resources.DlgTaskMan_BackgroundTaskGitRename,          IconImageListID.FileList_Rename);

        // ID
        private int m_id;

        // ダイアログでの表示名
        private string m_displayName;

        // ダイアログでのアイコンのイメージインデックス
        private IconImageListID m_dialogIconId;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]id          ID
        // 　　　　[in]displayName ダイアログでの表示名
        // 　　　　[in]iconId      ダイアログでのアイコンのイメージインデックス
        // 戻り値：なし
        //=========================================================================================
        public BackgroundTaskType(int id, string displayName, IconImageListID iconId) {
            m_id = id;
            m_displayName = displayName;
            m_dialogIconId = iconId;
            s_typeList.Add(this);
        }
        
        //=========================================================================================
        // プロパティ：Id
        //=========================================================================================
        public int Id {
            get {
                return m_id;
            }
        }
        
        //=========================================================================================
        // プロパティ：ダイアログでの表示名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
        }
        
        //=========================================================================================
        // プロパティ：ダイアログでのアイコンのイメージインデックス
        //=========================================================================================
        public IconImageListID DialogIconId {
            get {
                return m_dialogIconId;
            }
        }
        
        //=========================================================================================
        // プロパティ：タスク種別の一覧
        //=========================================================================================
        public static List<BackgroundTaskType> TypeList {
            get {
                return s_typeList;
            }
        }
    }
}
