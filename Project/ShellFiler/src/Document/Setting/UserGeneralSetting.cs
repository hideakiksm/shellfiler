using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using ShellFiler.Api;
using ShellFiler.FileTask;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.UI.Dialog;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.FileSystem;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ユーザの設定全般のシリアライザ
    //=========================================================================================
    class UserGeneralSetting {
        // 同名ファイルのオプション
        private SameFileOption m_sameFileOption = new SameFileOption();

        // ディレクトリ削除のオプション
        private DeleteFileOption m_deleteFileOption = new DeleteFileOption();

        // ファイル名を指定してコピーの方法
        private ClipboardCopyNameAsSetting m_clipboardCopyNameAsSetting = new ClipboardCopyNameAsSetting();

        // ファイル一覧の比較方法
        private FileCompareSetting m_fileCompareSetting = new FileCompareSetting();
        
        // 圧縮オプション
        private ArchiveSetting m_archiveSetting = new ArchiveSetting();

        // インクリメンタルサーチで文字列の先頭から検索するときtrue
        private bool m_incrementalSearchFromHead = FileIncrementalSearchDialog.SEARCH_FROM_HEAD_DEFAULT;

        // 新規ディレクトリに自動的にカレントを移動してよいときtrue
        private bool m_makeDirectoryMoveCurrent = MakeDirectoryDialog.MOVE_CURRENT_DEFAULT;

        // Windowsでコマンドの実行で標準出力の結果をログに中継する方法
        private ShellExecuteRelayMode m_shellExecuteRelayModeWindows = CommandInputDialog.SHELL_EXECUTE_RELAY_MODE_DEFAULT;

        // SSHでコマンドの実行で標準出力の結果をログに中継する方法
        private ShellExecuteRelayMode m_shellExecuteRelayModeSSH = CommandInputDialog.SHELL_EXECUTE_RELAY_MODE_DEFAULT;

        // SSHショートカットの作成方法
        private ShortcutType m_sshShortcutType = ShortcutType.SymbolicLink;

        // テキストの比較用バッファを差分表示ツール起動後に削除するときtrue
        private bool m_textViewerClearCompareBuffer = true;

        // テキストビューアでの折り返し設定
        private TextViewerLineBreakSetting m_textViewerLineBreak = new TextViewerLineBreakSetting();

        // テキストビューアでの検索オプション
        private TextSearchOption m_textSearchOption = new TextSearchOption();

        // テキストビューアでのクリップボードコピー形式
        private TextClipboardSetting m_textClipboardSetting = new TextClipboardSetting();

        // ダンプビューアでのクリップボードコピー形式
        private DumpClipboardSetting m_dumpClipboardSetting = new DumpClipboardSetting();

        // グラフィックビューアが使用するフィルター（コンフィグのGraphicsViewerFilterModeがAllImages以外はnull）
        private GraphicsViewerFilterSetting m_graphicsViewerImageFilter = new GraphicsViewerFilterSetting();

        // ファイルフィルターの設定
        private FileFilterSetting m_fileFilterSetting = new FileFilterSetting();

        // ファイルの結合での前回ファイル名
        private string m_combineDefaultFileName = "";

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public UserGeneralSetting() {
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
#if !FREE_VERSION
            string fileName = DirectoryManager.GeneralSetting;
            SettingLoader loader = new SettingLoader(fileName);
            bool success = LoadSetting(loader, this);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotLoadSetting, fileName, loader.ErrorDetail);
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private static bool LoadSetting(SettingLoader loader, UserGeneralSetting obj) {
            // ファイルがないときはデフォルト
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
            success = loader.ExpectTag(SettingTag.UserGeneral_UserGeneral, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.UserGeneral_UserGeneral) {
                    break;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.UserGeneral_SameFileOption) {
                    success = SameFileOption.LoadSetting(loader, out obj.m_sameFileOption);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.UserGeneral_SameFileOption, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.UserGeneral_DeleteFileOption) {
                    success = DeleteFileOption.LoadSetting(loader, out obj.m_deleteFileOption);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.UserGeneral_DeleteFileOption, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.UserGeneral_ClipboardCopyNameAsSetting) {
                    success = ClipboardCopyNameAsSetting.LoadSetting(loader, out obj.m_clipboardCopyNameAsSetting);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.UserGeneral_ClipboardCopyNameAsSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.UserGeneral_FileCompareSetting) {
                    success = FileCompareSetting.LoadSetting(loader, out obj.m_fileCompareSetting);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.UserGeneral_FileCompareSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.UserGeneral_ArchiveSetting) {
                    success = ArchiveSetting.LoadSetting(loader, out obj.m_archiveSetting);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.UserGeneral_ArchiveSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.UserGeneral_IncrementalSearchFromHead) {
                    obj.m_incrementalSearchFromHead = loader.BoolValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.UserGeneral_MakeDirectoryMoveCurrent) {
                    obj.m_makeDirectoryMoveCurrent = loader.BoolValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.UserGeneral_ShellExecuteRelayModeWindows) {
                    obj.m_shellExecuteRelayModeWindows = ShellExecuteRelayMode.FromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.UserGeneral_ShellExecuteRelayModeSSH) {
                    obj.m_shellExecuteRelayModeSSH = ShellExecuteRelayMode.FromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.UserGeneral_SshShortcutType) {
                    obj.m_sshShortcutType = ShortcutType.FromSerializedData(loader.StringValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.UserGeneral_TextViewerClearCompareBuffer) {
                    obj.m_textViewerClearCompareBuffer = loader.BoolValue;
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.UserGeneral_TextViewerLineBreak) {
                    success = TextViewerLineBreakSetting.LoadSetting(loader, out obj.m_textViewerLineBreak);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.UserGeneral_TextViewerLineBreak, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.UserGeneral_TextSearchOption) {
                    success = TextSearchOption.LoadSetting(loader, out obj.m_textSearchOption);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.UserGeneral_TextSearchOption, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.UserGeneral_TextClipboardSetting) {
                    success = TextClipboardSetting.LoadSetting(loader, out obj.m_textClipboardSetting);
                    if (!success) {
                        return success;
                    }
                    success = loader.ExpectTag(SettingTag.UserGeneral_TextClipboardSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.UserGeneral_DumpClipboardSetting) {
                    success = DumpClipboardSetting.LoadSetting(loader, out obj.m_dumpClipboardSetting);
                    if (!success) {
                        return false;
                    }
                    success = loader.ExpectTag(SettingTag.UserGeneral_DumpClipboardSetting, SettingTagType.EndObject);
                    if (!success) {
                        return false;
                    }
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.UserGeneral_CombineDefaultFileName) {
                    obj.m_combineDefaultFileName = loader.StringValue;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：設定の保存を行う
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：メインウィンドウのクローズ処理中に呼ばれる
        //=========================================================================================
        public void SaveSetting() {
#if !FREE_VERSION
            string fileName = DirectoryManager.GeneralSetting;
            SettingSaver saver = new SettingSaver(fileName);
            bool success = SaveSetting(saver, this);
            if (!success) {
                InfoBox.Warning(Program.MainWindow, Resources.Msg_CannotSaveSetting, fileName);
            }
#endif
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        private static bool SaveSetting(SettingSaver saver, UserGeneralSetting obj) {
            bool success;

            saver.StartObject(SettingTag.UserGeneral_UserGeneral);

            // ファイル一覧＞起動時の一覧
            saver.StartObject(SettingTag.UserGeneral_SameFileOption);
            success = SameFileOption.SaveSetting(saver, obj.m_sameFileOption);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.UserGeneral_SameFileOption);

            saver.StartObject(SettingTag.UserGeneral_DeleteFileOption);
            success = DeleteFileOption.SaveSetting(saver, obj.m_deleteFileOption);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.UserGeneral_DeleteFileOption);

            saver.StartObject(SettingTag.UserGeneral_ClipboardCopyNameAsSetting);
            success = ClipboardCopyNameAsSetting.SaveSetting(saver, obj.m_clipboardCopyNameAsSetting);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.UserGeneral_ClipboardCopyNameAsSetting);

            saver.StartObject(SettingTag.UserGeneral_FileCompareSetting);
            success = FileCompareSetting.SaveSetting(saver, obj.m_fileCompareSetting);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.UserGeneral_FileCompareSetting);

            saver.StartObject(SettingTag.UserGeneral_ArchiveSetting);
            success = ArchiveSetting.SaveSetting(saver, obj.m_archiveSetting);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.UserGeneral_ArchiveSetting);

            saver.AddBool(SettingTag.UserGeneral_IncrementalSearchFromHead, obj.m_incrementalSearchFromHead);
            saver.AddBool(SettingTag.UserGeneral_MakeDirectoryMoveCurrent, obj.m_makeDirectoryMoveCurrent);
            saver.AddString(SettingTag.UserGeneral_ShellExecuteRelayModeWindows, ShellExecuteRelayMode.ToSerializedData(obj.m_shellExecuteRelayModeWindows));
            saver.AddString(SettingTag.UserGeneral_ShellExecuteRelayModeSSH, ShellExecuteRelayMode.ToSerializedData(obj.m_shellExecuteRelayModeSSH));
            saver.AddString(SettingTag.UserGeneral_SshShortcutType, ShortcutType.ToSerializedData(obj.m_sshShortcutType));

            saver.AddBool(SettingTag.UserGeneral_TextViewerClearCompareBuffer, obj.m_textViewerClearCompareBuffer);
            saver.StartObject(SettingTag.UserGeneral_TextViewerLineBreak);
            success = TextViewerLineBreakSetting.SaveSetting(saver, obj.m_textViewerLineBreak);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.UserGeneral_TextViewerLineBreak);

            saver.StartObject(SettingTag.UserGeneral_TextSearchOption);
            success = TextSearchOption.SaveSetting(saver, obj.m_textSearchOption);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.UserGeneral_TextSearchOption);

            saver.StartObject(SettingTag.UserGeneral_TextClipboardSetting);
            success = TextClipboardSetting.SaveSetting(saver, obj.m_textClipboardSetting);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.UserGeneral_TextClipboardSetting);

            saver.StartObject(SettingTag.UserGeneral_DumpClipboardSetting);
            success = DumpClipboardSetting.SaveSetting(saver, obj.m_dumpClipboardSetting);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.UserGeneral_DumpClipboardSetting);

            saver.StartObject(SettingTag.UserGeneral_GraphicsViewerImageFilter);
            success = GraphicsViewerFilterSetting.SaveSetting(saver, obj.m_graphicsViewerImageFilter);
            if (!success) {
                return false;
            }
            saver.EndObject(SettingTag.UserGeneral_GraphicsViewerImageFilter);

            saver.AddString(SettingTag.UserGeneral_CombineDefaultFileName, obj.m_combineDefaultFileName);

            saver.EndObject(SettingTag.UserGeneral_UserGeneral);
            
            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // プロパティ：同名ファイルのオプション
        //=========================================================================================
        public SameFileOption SameFileOption {
            get {
                return m_sameFileOption;
            }
            set {
                m_sameFileOption = value;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリ削除のオプション
        //=========================================================================================
        public DeleteFileOption DeleteFileOption {
            get {
                return m_deleteFileOption;
            }
            set {
                m_deleteFileOption = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル名を指定してコピーの方法
        //=========================================================================================
        public ClipboardCopyNameAsSetting ClipboardCopyNameAsSetting {
            get {
                return m_clipboardCopyNameAsSetting;
            }
            set {
                m_clipboardCopyNameAsSetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧の比較方法
        //=========================================================================================
        public FileCompareSetting FileCompareSetting {
            get {
                return m_fileCompareSetting;
            }
            set {
                m_fileCompareSetting = value;
            }
        }
                
        //=========================================================================================
        // プロパティ：圧縮オプション
        //=========================================================================================
        public ArchiveSetting ArchiveSetting {
            get {
                return m_archiveSetting;
            }
            set {
                m_archiveSetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：インクリメンタルサーチで文字列の先頭から検索するときtrue
        //=========================================================================================
        public bool IncrementalSearchFromHead {
            get {
                return m_incrementalSearchFromHead;
            }
            set {
                m_incrementalSearchFromHead = value;
            }
        }

        //=========================================================================================
        // プロパティ：新規ディレクトリに自動的にカレントを移動してよいときtrue
        //=========================================================================================
        public bool MakeDirectoryMoveCurrent {
            get {
                return m_makeDirectoryMoveCurrent;
            }
            set {
                m_makeDirectoryMoveCurrent = value;
            }
        }

        //=========================================================================================
        // プロパティ：Windowsでコマンドの実行で標準出力の結果をログに中継する方法
        //=========================================================================================
        public ShellExecuteRelayMode ShellExecuteRelayModeWindows {
            get {
                return m_shellExecuteRelayModeWindows;
            }
            set {
                m_shellExecuteRelayModeWindows = value;
            }
        }

        //=========================================================================================
        // プロパティ：SSHでコマンドの実行で標準出力の結果をログに中継する方法
        //=========================================================================================
        public ShellExecuteRelayMode ShellExecuteRelayModeSSH {
            get {
                return m_shellExecuteRelayModeSSH;
            }
            set {
                m_shellExecuteRelayModeSSH = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：SSHショートカットの作成方法
        //=========================================================================================
        public ShortcutType SSHShortcutType {
            get {
                return m_sshShortcutType;
            }
            set {
                m_sshShortcutType = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストの比較用バッファを差分表示ツール起動後に削除するときtrue
        //=========================================================================================
        public bool TextViewerClearCompareBuffer {
            get {
                return m_textViewerClearCompareBuffer;
            }
            set {
                m_textViewerClearCompareBuffer = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューアでの折り返し設定
        //=========================================================================================
        public TextViewerLineBreakSetting TextViewerLineBreak {
            get {
                return m_textViewerLineBreak;
            }
            set {
                m_textViewerLineBreak = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューアでの検索オプション
        //=========================================================================================
        public TextSearchOption TextViewerSearchOption {
            get {
                return m_textSearchOption;
            }
            set {
                m_textSearchOption = value;
            }
        }

        //=========================================================================================
        // プロパティ：テキストビューアでのクリップボードコピー形式
        //=========================================================================================
        public TextClipboardSetting TextClipboardSetting {
            get {
                return m_textClipboardSetting;
            }
            set {
                m_textClipboardSetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：ダンプビューアでのクリップボードコピー形式
        //=========================================================================================
        public DumpClipboardSetting DumpClipboardSetting {
            get {
                return m_dumpClipboardSetting;
            }
            set {
                m_dumpClipboardSetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアが使用するフィルター（コンフィグのGraphicsViewerFilterModeがAllImages以外はnull）
        //=========================================================================================
        public GraphicsViewerFilterSetting GraphicsViewerImageFilter {
            get {
                return m_graphicsViewerImageFilter;
            }
            set {
                m_graphicsViewerImageFilter = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルフィルターの設定
        //=========================================================================================
        public FileFilterSetting FileFilterSetting {
            get {
                return m_fileFilterSetting;
            }
            set {
                m_fileFilterSetting = value;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルの結合での前回ファイル名
        //=========================================================================================
        public string CombineDefaultFileName {
            get {
                return m_combineDefaultFileName;
            }
            set {
                m_combineDefaultFileName = value;
            }
        }
    }
}
