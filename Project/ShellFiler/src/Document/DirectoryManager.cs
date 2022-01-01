using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.FileSystem;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：ディレクトリ名の管理クラス
    //=========================================================================================
    class DirectoryManager {
        // 設定フォルダ
        public const string FOLDER_NAME_SETTING = "Setting\\";

        // 設定のバックアップフォルダ
        public const string FOLDER_NAME_BACKUP = "Backup\\";

        // ターミナルログのデフォルト出力フォルダ
        public const string FOLDER_NAME_TERMINAL_LOG = "ShellFiler\\TerminalLog\\";

        // 初期設定のファイル名
        public const string SETTING_NAME_INITIAL = "InitialSetting.sfs";

        // 一般設定のファイル名
        public const string SETTINGL_NAME_GENERAL = "GeneralSetting.sfs";

        // コンフィグレーションのファイル名
        public const string SETTING_NAME_CONFIGURATION = "Configuration.sfs";

        // キー設定のファイル名
        public const string SETTING_NAME_KEY = "KeyAssign.sfs";

        // メニュー設定のファイル名
        public const string SETTING_NAME_MENU = "MenuAssign.sfs";

        // アーカイブの自動展開パスワード設定のファイル名
        public const string SETTING_NAME_ARCHIVE_AUTO_PASSWORD = "ArchiveAutoPassword.sfs";

        // ファイルビュアーでの検索文字列ヒストリ設定のファイル名
        public const string SETTING_NAME_VIEWER_SEARCH_HISTORY = "ViewerSearchHistory.sfs";

        // コマンドヒストリ設定のファイル名
        public const string SETTING_NAME_COMMAND_HISTORY = "CommandHistory.sfs";
        
        // SSH認証情報設定のファイル名
        public const string SETTING_NAME_SSH_USER_AUTHENTICATE = "SSHUserAuthenticate.sfs";
        
        // ブックマーク設定のファイル名
        public const string SETTING_NAME_BOOKMARK = "BookmarkSetting.sfs";
        
        // ファイルフィルター設定のファイル名
        public const string SETTING_NAME_FILEFILTER = "FileFilterSetting.sfs";

        // ファイル転送条件設定のファイル名
        public const string SETTING_NAME_FILECONDITION = "FileConditionSetting.sfs";

        // フォルダ履歴のファイル名
        public const string FOLDER_HISTORY_WHOLE = "FolderHistory.sfs";

        // コマンド一覧のファイル名
        public const string COMMAND_API_LIST = "CommandList.dat";

        // シェアウェア用ライセンスファイル
        public const string LICENSE_FILE = "License.dat";

        // 使用許諾同意ファイル
        public const string AGREE_FILE = "Agree.dat";

        // ローカルデータのルート（最後は"\"）
        private static string s_localData;

        // アプリケーションローミングデータのルート（最後は"\"）
        private static string s_applicationData;

        // 設定関連のルート（最後は"\"）
        private static string s_settingRoot;
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static void Initialize() {
            s_localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            s_localData = GenericFileStringUtils.CompleteDirectoryName(s_localData, "\\");
            s_applicationData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ShellFiler\\");
            s_settingRoot = s_applicationData + FOLDER_NAME_SETTING;
        }

        //=========================================================================================
        // 機　能：一時ディレクトリを作成する
        // 引　数：なし
        // 戻り値：なし
        // メ　モ：EULA同意後に一時ディレクトリを作成する
        //=========================================================================================
        public static void CreateTemporary() {
            // 一時ディレクトリを作成
            string dirName = null;
            try {
                dirName = s_localData;
                if (!Directory.Exists(dirName)) {
                    Directory.CreateDirectory(dirName);
                }
                dirName = s_localData + FOLDER_NAME_TERMINAL_LOG;
                if (!Directory.Exists(dirName)) {
                    Directory.CreateDirectory(dirName);
                }
                dirName = s_settingRoot;
                if (!Directory.Exists(dirName)) {
                    Directory.CreateDirectory(dirName);
                }
                dirName = s_settingRoot + FOLDER_NAME_BACKUP;
                if (!Directory.Exists(dirName)) {
                    Directory.CreateDirectory(dirName);
                }
            } catch (Exception e) {
                throw new SfException(SfException.WorkDirectoryCreate, dirName, e.Message);
            }        
        }

        //=========================================================================================
        // 機　能：作業ディレクトリ名を返す
        // 引　数：[in]config  コンフィグでの設定（"":デフォルト）
        // 戻り値：作業ディレクトリ名（最後は"\"）
        //=========================================================================================
        public static string GetTemporaryDirectory(string config) {
            string path;
            if (config == null || config == "") {
                path = Path.Combine(s_localData, "ShellFiler\\");
            } else {
                path = config;
            }
            return path;
        }

        //=========================================================================================
        // プロパティ：テンポラリのルート
        //=========================================================================================
        public static string TemporaryRoot {
            get {
                return DirectoryManager.GetTemporaryDirectory(Configuration.TemporaryDirectory);
            }
        }

        //=========================================================================================
        // プロパティ：SSHターミナルログの出力先
        //=========================================================================================
        public static string TerminalLogOutputFolder {
            get {
                string logFolder = Configuration.Current.TerminalLogOutputFolder;
                if (logFolder == null) {
                    logFolder = s_localData + FOLDER_NAME_TERMINAL_LOG;
                }
                return logFolder;
            }
        }

        //=========================================================================================
        // プロパティ：アプリケーションローミングデータのルート（最後は"\"）
        //=========================================================================================
        public static string ApplicationDataRoot {
            get {
                return s_applicationData;
            }
        }

        //=========================================================================================
        // プロパティ：初期設定のファイル名
        //=========================================================================================
        public static string InitialSetting {
            get {
                return s_settingRoot + SETTING_NAME_INITIAL;
            }
        }

        //=========================================================================================
        // プロパティ：一般設定のファイル名
        //=========================================================================================
        public static string GeneralSetting {
            get {
                return s_settingRoot + SETTINGL_NAME_GENERAL;
            }
        }

        //=========================================================================================
        // プロパティ：コンフィグレーションのファイル名
        //=========================================================================================
        public static string ConfigurationSetting {
            get {
                return s_settingRoot + SETTING_NAME_CONFIGURATION;
            }
        }

        //=========================================================================================
        // プロパティ：キー設定のファイル名
        //=========================================================================================
        public static string KeySetting {
            get {
                return s_settingRoot + SETTING_NAME_KEY;
            }
        }

        //=========================================================================================
        // プロパティ：キー設定のバックアップファイル名のベース
        //=========================================================================================
        public static string KeySettingBackupBase {
            get {
                return s_settingRoot + FOLDER_NAME_BACKUP + SETTING_NAME_KEY;
            }
        }

        //=========================================================================================
        // プロパティ：メニュー設定のファイル名
        //=========================================================================================
        public static string MenuSetting {
            get {
                return s_settingRoot + SETTING_NAME_MENU;
            }
        }

        //=========================================================================================
        // プロパティ：アーカイブの自動展開パスワード設定のファイル名
        //=========================================================================================
        public static string ArchiveAutoPasswordSetting {
            get {
                return s_settingRoot + SETTING_NAME_ARCHIVE_AUTO_PASSWORD;
            }
        }
       
        //=========================================================================================
        // プロパティ：ファイルビュアーでの検索文字列ヒストリ設定のファイル名
        //=========================================================================================
        public static string ViewerSearchHistorySetting {
            get {
                return s_settingRoot + SETTING_NAME_VIEWER_SEARCH_HISTORY;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドヒストリ設定のファイル名
        //=========================================================================================
        public static string CommandHistorySetting {
            get {
                return s_settingRoot + SETTING_NAME_COMMAND_HISTORY;
            }
        }

        //=========================================================================================
        // プロパティ：SSH認証情報設定のファイル名
        //=========================================================================================
        public static string SSHUserAuthenticate {
            get {
                return s_settingRoot + SETTING_NAME_SSH_USER_AUTHENTICATE;
            }
        }

        //=========================================================================================
        // プロパティ：ブックマーク設定のファイル名
        //=========================================================================================
        public static string BookmarkSetting {
            get {
                return s_settingRoot + SETTING_NAME_BOOKMARK;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルフィルター設定のファイル名
        //=========================================================================================
        public static string FileFilterSetting {
            get {
                return s_settingRoot + SETTING_NAME_FILEFILTER;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル転送条件設定のファイル名
        //=========================================================================================
        public static string FileConditionSetting {
            get {
                return s_settingRoot + SETTING_NAME_FILECONDITION;
            }
        }
        
        //=========================================================================================
        // プロパティ：フォルダ履歴のファイル名
        //=========================================================================================
        public static string FolderHistoryWhole {
            get {
                return s_settingRoot + FOLDER_HISTORY_WHOLE;
            }
        }

        //=========================================================================================
        // プロパティ：シェアウェア用ライセンスファイル
        //=========================================================================================
        public static string LicenseFile {
            get {
                return s_applicationData + LICENSE_FILE;
            }
        }

        //=========================================================================================
        // プロパティ：使用許諾同意ファイル
        //=========================================================================================
        public static string LicenseAgreeFile {
            get {
                return s_applicationData + AGREE_FILE;
            }
        }

        //=========================================================================================
        // プロパティ：コマンド一覧のファイル名
        //=========================================================================================
        public static string CommandApiList {
            get {
                return Program.InstallPath + COMMAND_API_LIST;
            }
        }
    }
}
