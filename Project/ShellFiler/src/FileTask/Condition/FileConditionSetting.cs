using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：ファイル操作の対象となる設定
    //=========================================================================================
    public class FileConditionSetting : ICloneable {
        // ユーザー定義設定（Windows）
        private List<FileConditionItemWindows> m_userSettingWindows;

        // ユーザー定義設定（SSH）
        private List<FileConditionItemSSH> m_userSettingSSH;

        // コピー、移動、削除のデフォルト
        private FileConditionDialogInfo m_transferConditionDialogInfo;
        
        // 一括マークのデフォルト
        private MarkConditionsDialogInfo m_markConditionsDialogInfo;

        // ファイル一覧のフィルター
        private FileConditionDialogInfo m_fileListFilterDialogInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileConditionSetting() {
            SetDefaultSetting();
        }

        //=========================================================================================
        // 機　能：デフォルト値を設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetDefaultSetting() {
            m_userSettingWindows = new List<FileConditionItemWindows>();
            m_userSettingSSH = new List<FileConditionItemSSH>();
            m_transferConditionDialogInfo = new FileConditionDialogInfo();
            m_markConditionsDialogInfo = new MarkConditionsDialogInfo();
            m_fileListFilterDialogInfo = new FileConditionDialogInfo();
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            FileConditionSetting obj = new FileConditionSetting();
            foreach (FileConditionItemWindows item in m_userSettingWindows) {
                obj.m_userSettingWindows.Add((FileConditionItemWindows)(item.Clone()));
            }
            foreach (FileConditionItemSSH item in m_userSettingSSH) {
                obj.m_userSettingSSH.Add((FileConditionItemSSH)(item.Clone()));
            }
            obj.m_transferConditionDialogInfo = (FileConditionDialogInfo)(m_transferConditionDialogInfo.Clone());
            obj.m_markConditionsDialogInfo = (MarkConditionsDialogInfo)(m_markConditionsDialogInfo.Clone());
            obj.m_fileListFilterDialogInfo = (FileConditionDialogInfo)(m_fileListFilterDialogInfo.Clone());

            return obj;
        }
        
        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(FileConditionSetting obj1, FileConditionSetting obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_userSettingWindows.Count != obj2.m_userSettingWindows.Count) {
                return false;
            }
            for (int i = 0; i < obj1.m_userSettingWindows.Count; i++) {
                if (!obj1.m_userSettingWindows[i].EqualsConfigObject(obj2.m_userSettingWindows[i])) {
                    return false;
                }
            }

            if (obj1.m_userSettingSSH.Count != obj2.m_userSettingSSH.Count) {
                return false;
            }
            for (int i = 0; i < obj1.m_userSettingSSH.Count; i++) {
                if (!obj1.m_userSettingSSH[i].EqualsConfigObject(obj2.m_userSettingSSH[i])) {
                    return false;
                }
            }

            if (!FileConditionDialogInfo.EqualsConfig(obj1.m_transferConditionDialogInfo, obj2.m_transferConditionDialogInfo)) {
                return false;
            }
            if (!MarkConditionsDialogInfo.EqualsConfig(obj1.m_markConditionsDialogInfo, obj2.m_markConditionsDialogInfo)) {
                return false;
            }
            if (!FileConditionDialogInfo.EqualsConfig(obj1.m_fileListFilterDialogInfo, obj2.m_fileListFilterDialogInfo)) {
                return false;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void LoadSetting() {
#if !FREE_VERSION
            // ファイルを読み込む
            string fileName = DirectoryManager.FileConditionSetting;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = LoadSettingInternal(loader);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSetting, fileName, loader.ErrorDetail);
                SetDefaultSetting();
            }

            // 警告メッセージを表示
            if (loader.WarningList.Count > 0) {
                List<string> message = new List<string>();
                foreach (SettingLoader.Warning warning in loader.WarningList) {
                    if (message.Contains(warning.Message)) {
                        message.Add(warning.Message);
                    }
                }
                string warnings = StringUtils.CombineStringArray(message, "\r\n");
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSettingAll, fileName, warnings);
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool LoadSettingInternal(SettingLoader loader) {
            // ファイルがないときはデフォルト
            SetDefaultSetting();
            if (!File.Exists(loader.FileName)) {
                return true;
            }

            // ファイルから読み込む
            bool success;
            success = loader.LoadSetting(false);
            if (!success) {
                return false;
            }

            // タグを読み込む
            success = loader.ExpectTag(SettingTag.FileCondition, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.FileCondition) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_UserWindows) {
                    success = FileConditionItemWindows.LoadSetting(loader, m_userSettingWindows);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FileCondition_UserWindows, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_UserSSH) {
                    success = FileConditionItemSSH.LoadSetting(loader, m_userSettingSSH);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FileCondition_UserSSH, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_TransferConditionInfo) {
                    success = FileConditionDialogInfo.LoadSetting(loader, out m_transferConditionDialogInfo);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FileCondition_FileListFilterInfo, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_MarkConditionsInfo) {
                    success = MarkConditionsDialogInfo.LoadSetting(loader, out m_markConditionsDialogInfo);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FileCondition_MarkConditionsInfo, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.FileCondition_FileListFilterInfo) {
                    success = FileConditionDialogInfo.LoadSetting(loader, out m_fileListFilterDialogInfo);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.FileCondition_FileListFilterInfo, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SaveSetting() {
#if !FREE_VERSION
            string fileName = DirectoryManager.FileConditionSetting;
            SettingSaver saver = new SettingSaver(fileName);
            bool success = SaveSettingInternal(saver);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotSaveSetting, fileName);
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private bool SaveSettingInternal(SettingSaver saver) {
            bool success;
            saver.StartObject(SettingTag.FileCondition);

            saver.StartObject(SettingTag.FileCondition_UserWindows);
            FileConditionItemWindows.SaveSetting(saver, m_userSettingWindows);
            saver.EndObject(SettingTag.FileCondition_UserWindows);

            saver.StartObject(SettingTag.FileCondition_UserSSH);
            FileConditionItemSSH.SaveSetting(saver, m_userSettingSSH);
            saver.EndObject(SettingTag.FileCondition_UserSSH);

            saver.StartObject(SettingTag.FileCondition_TransferConditionInfo);
            success = FileConditionDialogInfo.SaveSetting(saver, m_transferConditionDialogInfo);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.FileCondition_TransferConditionInfo);

            saver.StartObject(SettingTag.FileCondition_MarkConditionsInfo);
            success = MarkConditionsDialogInfo.SaveSetting(saver, m_markConditionsDialogInfo);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.FileCondition_MarkConditionsInfo);

            saver.StartObject(SettingTag.FileCondition_FileListFilterInfo);
            success = FileConditionDialogInfo.SaveSetting(saver, m_fileListFilterDialogInfo);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.FileCondition_FileListFilterInfo);

            saver.EndObject(SettingTag.FileCondition);
            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // 機　能：すべてのクイック設定を取得する
        // 引　数：[in]srcSystem  使用する転送元のファイルシステム
        // 戻り値：すべてのクイック設定
        //=========================================================================================
        public List<FileConditionItem> GetAllSettingItemUI(FileSystemID srcSystem) {
            List<FileConditionItem> list = new List<FileConditionItem>();
            if (FileSystemID.IsWindows(srcSystem)) {
                foreach (FileConditionItemWindows item in m_userSettingWindows) {
                    list.Add(item);
                }
            } else if (FileSystemID.IsSSH(srcSystem)) {
                foreach (FileConditionItemSSH item in m_userSettingSSH) {
                    list.Add(item);
                }
            } else if (FileSystemID.IsVirtual(srcSystem)) {
                ;
            } else {
                FileSystemID.NotSupportError(srcSystem);
            }
            list.AddRange(DefinedSettingList());
            return list;
        }

        //=========================================================================================
        // 機　能：定義済みの設定リストを返す
        // 引　数：なし
        // 戻り値：定義済みの設定リスト
        //=========================================================================================
        public static List<FileConditionItem> DefinedSettingList() {
            List<FileConditionItem> result = new List<FileConditionItem>();

            // 本日更新されたファイル
            {
                FileConditionItemDefined condition = new FileConditionItemDefined();

                DateTimeCondition dateTimeCondition = new DateTimeCondition();
                dateTimeCondition.TimeType = DateTimeType.RelativeEndXxx;
                dateTimeCondition.RelativeEnd = 0;
                dateTimeCondition.TimeEnd = new TimeInfo(0, 0, 0);
                dateTimeCondition.IncludeEnd = true;
                condition.DisplayName = Resources.FileCondition_DefinedNameToday;
                condition.FileConditionTarget = FileConditionTarget.FileOnly;
                condition.UpdateTimeCondition = dateTimeCondition;
                result.Add(condition);
            }

            // svn/cvsの管理ファイル
            {
                FileConditionItemDefined condition = new FileConditionItemDefined();
                condition.DisplayName = Resources.FileCondition_DefinedSvnCheckout;
                condition.FileConditionTarget = FileConditionTarget.FolderOnly;
                condition.FileNameType = FileNameType.RegularExpression;
                condition.FileName = "^\\.(svn|cvs)$";
                result.Add(condition);
            }

            // アーカイブファイル
            {
                string[] extListOrg = SevenZipUtils.SupportedExtList;
                string[] extList = new string[extListOrg.Length];
                for (int i = 0; i < extList.Length; i++) {
                    extList[i] = extListOrg[i].Substring(1);
                }
                string strExtList = StringUtils.CombineStringArray(extList, "|");
                FileConditionItemDefined condition = new FileConditionItemDefined();
                condition.DisplayName = Resources.FileCondition_DefinedArchive;
                condition.FileConditionTarget = FileConditionTarget.FileOnly;
                condition.FileNameType = FileNameType.RegularExpression;
                condition.FileName = "\\.(" + strExtList + ")$";
                result.Add(condition);
            }

            // 画像関連
            {
                FileConditionItemDefined condition = new FileConditionItemDefined();
                condition.DisplayName = Resources.FileCondition_DefinedGraphics;
                condition.FileConditionTarget = FileConditionTarget.FileOnly;
                condition.FileNameType = FileNameType.RegularExpression;
                condition.FileName = "\\.(jpg|jpeg|gif|png|bmp)$";
                result.Add(condition);
            }

            // 音楽関連
            {
                FileConditionItemDefined condition = new FileConditionItemDefined();
                condition.DisplayName = Resources.FileCondition_DefinedMusic;
                condition.FileConditionTarget = FileConditionTarget.FileOnly;
                condition.FileNameType = FileNameType.RegularExpression;
                condition.FileName = "\\.(mp3|wmv|mp4|wav|mid)$";
                result.Add(condition);
            }

            return result;
        }

        //=========================================================================================
        // 機　能：設定をコピーして作成する
        // 引　数：[in]src           コピー元の設定（コピー元がなく、新規に作成するときnull）
        // 　　　　[in]fileSystemId  利用するファイルシステム
        // 戻り値：作成した設定（コピーをサポートしていないファイルシステムのときnull）
        //=========================================================================================
        public static FileConditionItem CreateConditionItemFrom(FileConditionItem src, FileSystemID fileSystemId) {
            FileConditionItem item;
            if (FileSystemID.IsWindows(fileSystemId)) {
                if (src == null) {
                    item = FileConditionItemWindows.GetDefault();
                } else if (src is FileConditionItemWindows) {
                    item = (FileConditionItem)(src.Clone());
                } else {
                    item = new FileConditionItemWindows();
                    item.DisplayName = src.DisplayName;
                    item.FileNameType = src.FileNameType;
                    item.FileName = src.FileName;
                    if (src.UpdateTimeCondition == null) {
                        item.UpdateTimeCondition = null;
                    } else {
                        item.UpdateTimeCondition = (DateTimeCondition)(src.UpdateTimeCondition.Clone());
                    }
                }
            } else if (FileSystemID.IsSSH(fileSystemId)) {
                if (src == null) {
                    item = FileConditionItemSSH.GetDefault();
                } else if (src is FileConditionItemSSH) {
                    item = (FileConditionItem)(src.Clone());
                } else {
                    item = new FileConditionItemSSH();
                    item.DisplayName = src.DisplayName;
                    item.FileNameType = src.FileNameType;
                    item.FileName = src.FileName;
                    if (src.UpdateTimeCondition == null) {
                        item.UpdateTimeCondition = null;
                    } else {
                        item.UpdateTimeCondition = (DateTimeCondition)(src.UpdateTimeCondition.Clone());
                    }
                }
            } else if (FileSystemID.IsVirtual(fileSystemId)) {
                item = null;
            } else {
                FileSystemID.NotSupportError(fileSystemId);
                item = null;
            }
            return item;
        }

        //=========================================================================================
        // 機　能：指定されたフィルターの条件が変更されていないことを確認する
        // 引　数：[in]selectedName        選択中の表示名（未使用、selectedConditionsのDisplayNameを使用）
        // 　　　　[in]selectedConditions  使用中のフィルター
        // 　　　　[in]fileSystem          利用するファイルシステム（未使用、selectedConditionsの型を使用）
        // 戻り値：作成した設定
        //=========================================================================================
        public List<string> IsFilterChanged(List<string> selectedName, List<FileConditionItem> selectedConditions, FileSystemID fileSystem) {
            List<string> result = new List<string>();
            bool isWindows = false;
            bool isSSH = false;
            foreach (FileConditionItem item in selectedConditions) {
                if (item is FileConditionItemWindows) {
                    isWindows = true;
                } else if (item is FileConditionItemSSH) {
                    isSSH = true;
                }
            }

            // ユーザー定義の項目一覧を作成
            Dictionary<string, FileConditionItem> nameToItem = new Dictionary<string, FileConditionItem>();
            if (isWindows && isSSH) {
                Program.Abort("使用中のフィルター種別が想定外の状態になっています。");
                return result;
            } else if (isWindows) {
                foreach (FileConditionItemWindows item in m_userSettingWindows) {
                    nameToItem.Add(item.DisplayName, item);
                }
            } else if (isSSH) {
                foreach (FileConditionItemSSH item in m_userSettingSSH) {
                    nameToItem.Add(item.DisplayName, item);
                }
            } else {
                // 定義済みしか使用していない
                return result;
            }

            // 使用中の項目が変更されていないことをチェック
            for (int i = 0; i < selectedConditions.Count; i++) {
                if (selectedConditions[i] is FileConditionItemDefined) {
                    continue;
                }
                string dispName = selectedConditions[i].DisplayName;
                if (nameToItem.ContainsKey(dispName)) {
                    if (!selectedConditions[i].EqualsConfigObject(nameToItem[dispName])) {
                        result.Add(string.Format(Resources.FileCondition_FilterChangedItem, dispName));
                    }
                } else {
                    result.Add(string.Format(Resources.FileCondition_FilterLostItem, dispName));
                }
            }
            return result;
        }

        //=========================================================================================
        // プロパティ：ユーザー定義設定（Windows）
        //=========================================================================================
        public List<FileConditionItemWindows> UserSettingWindows {
            get {
                return m_userSettingWindows;
            }
            set {
                m_userSettingWindows = value;
            }
        }

        //=========================================================================================
        // プロパティ：ユーザー定義設定（SSH）
        //=========================================================================================
        public List<FileConditionItemSSH> UserSettingSSH {
            get {
                return m_userSettingSSH;
            }
            set {
                m_userSettingSSH = value;
            }
        }

        //=========================================================================================
        // プロパティ：最後に選択されていた項目の設定名のリスト
        //=========================================================================================
        public FileConditionDialogInfo TransferConditionDialogInfo {
            get {
                return m_transferConditionDialogInfo;
            }
            set {
                m_transferConditionDialogInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：一括マークのダイアログ入力値のデフォルト
        //=========================================================================================
        public MarkConditionsDialogInfo MarkConditionsDialogInfo {
            get {
                return m_markConditionsDialogInfo;
            }
            set {
                m_markConditionsDialogInfo = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧フィルターのダイアログ入力値のデフォルト
        //=========================================================================================
        public FileConditionDialogInfo FileListFilterDialogInfo {
            get {
                return m_fileListFilterDialogInfo;
            }
            set {
                m_fileListFilterDialogInfo = value;
            }
        }
    }
}
