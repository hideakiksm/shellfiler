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
using ShellFiler.Locale;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：アドレスバーのカスタマイズ情報
    //=========================================================================================
    class AddressBarSetting {
        // 展開して表示する最大パス構成数
        private const int MAX_PATH_LIST = 10;

        // アドレスバーの設定一覧
        private List<AddressBarSettingItem> m_addressBarSettingItem = new List<AddressBarSettingItem>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AddressBarSetting() {
            m_addressBarSettingItem.Add(new AddressBarSettingItem(Resources.AddressBar_Desktop, Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
            m_addressBarSettingItem.Add(new AddressBarSettingItem(Resources.AddressBar_MyDocuments, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
            m_addressBarSettingItem.Add(new AddressBarSettingItem(Resources.AddressBar_Music, Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)));
            m_addressBarSettingItem.Add(new AddressBarSettingItem(Resources.AddressBar_Picture, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)));
            m_addressBarSettingItem.Add(new AddressBarSettingItem(Resources.AddressBar_Download, KnownUrl.WindowsDownloadFolder));
            m_addressBarSettingItem.Add(new AddressBarSettingItem(null, AddressBarSettingItem.DRIVE_LIST));
        }

        //=========================================================================================
        // 機　能：アドレスバーのランタイムでの一覧を返す
        // 引　数：[in]currentPath    カレントディレクトリ
        // 　　　　[in]tempBmpList    一時的に作成した画像一覧を返すリスト
        // 戻り値：ランタイムでの項目一覧を返す
        //=========================================================================================
        public List<AddressBarSettingRuntimeItem> GetRuntimeItemList(string currentPath, List<Bitmap> tempBmpList) {
            // カレントパスを分解
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            List<string> currentParsedPath;
            string currentSeparator;
            FileSystemID currentFileSystemId;
            factory.ParsePath(currentPath, out currentParsedPath, out currentSeparator, out currentFileSystemId);

            // 設定されている項目を展開
            List<AddressBarSettingRuntimeItem> runtimeList = new List<AddressBarSettingRuntimeItem>();      // ディレクトリ名の最後はセパレータ
            foreach (AddressBarSettingItem setting in m_addressBarSettingItem) {
                if (setting.Directory == AddressBarSettingItem.DRIVE_LIST) {
                    List<AddressBarSettingRuntimeItem> driveList = CreateDriveList(tempBmpList);
                    runtimeList.AddRange(driveList);
                } else {
                    AddressBarSettingRuntimeItem item = CreateAddressBarItem(setting);
                    if (item != null) {
                        runtimeList.Add(item);
                    }
                }
            }

            // カレントの該当箇所を検索
            for (int i = currentParsedPath.Count - 1; i >= 0; i--) {
                string currentSub = "";
                for (int j = 0; j <= i; j++) {
                    currentSub += currentParsedPath[j] + currentSeparator;
                }
                for (int j = 0; j < runtimeList.Count; j++) {
                    if (currentSub == runtimeList[j].Directory) {
                        if (i != currentParsedPath.Count - 1) {
                            List<AddressBarSettingRuntimeItem> subList = CreateSubPathItem(currentParsedPath, currentSeparator, i + 1);
                            runtimeList.InsertRange(j + 1, subList);
                            return runtimeList;
                        } else {
                            return runtimeList;
                        }
                    }
                }
            }
            List<AddressBarSettingRuntimeItem> currentList = CreateSubPathItem(currentParsedPath, currentSeparator, 0);
            runtimeList.AddRange(currentList);
            return runtimeList;
        }

        //=========================================================================================
        // 機　能：アドレスバーのドライブ一覧を作成して返す
        // 引　数：[in]tempBmpList    一時的に作成した画像一覧を返すリスト
        // 戻り値：ドライブ一覧のランタイム項目のリスト
        //=========================================================================================
        public List<AddressBarSettingRuntimeItem> CreateDriveList(List<Bitmap> tempBmpList) {
            List<AddressBarSettingRuntimeItem> result = new List<AddressBarSettingRuntimeItem>();
            string[] driveList = Directory.GetLogicalDrives();
            foreach (string driveLetter in driveList) {
                if (driveLetter.Length < 1) {
                    continue;
                }
                DriveInfo driveInfo = new DriveInfo(driveLetter);

                Bitmap icon = DriveItem.GetDriveIcon(driveInfo);
                tempBmpList.Add(icon);
                string dispName = DriveItem.GetDisplayName(driveInfo);
                string directory = driveLetter[0] + ":\\";
                AddressBarSettingRuntimeItem item = new AddressBarSettingRuntimeItem(dispName, directory, icon, 0);
                result.Add(item);
            }
            return result;
        }

        //=========================================================================================
        // 機　能：アドレスバーのランタイムでの項目を返す
        // 引　数：[in]setting  アドレスバーの項目設定
        // 戻り値：項目設定に対応するランタイム項目
        //=========================================================================================
        public AddressBarSettingRuntimeItem CreateAddressBarItem(AddressBarSettingItem setting) {
            FileSystemFactory factory = Program.Document.FileSystemFactory;
            List<string> settingParsedPath;
            string settingSeparator;
            FileSystemID settingFileSystemId;
            factory.ParsePath(setting.Directory, out settingParsedPath, out settingSeparator, out settingFileSystemId);

            Bitmap icon = null;
            if (settingFileSystemId == FileSystemID.Windows) {
                if (!Directory.Exists(setting.Directory)) {
                    return null;
                }
                icon = Win32IconUtils.GetFileIconBitmap(setting.Directory, IconSize.Small16, Win32IconUtils.ICONMODE_NORMAL);
            } else if (settingFileSystemId == FileSystemID.SFTP) {
                icon = UIIconManager.IconSshSetting;
            } else {
                Program.Abort("未知のファイルシステムでアドレスバーを初期化しようとしています。");
            }
            AddressBarSettingRuntimeItem item = new AddressBarSettingRuntimeItem(setting.DisplayName, setting.Directory, icon, 0);
            return item;
        }

        //=========================================================================================
        // 機　能：アドレスバーのカレントディレクトリに対するサブパス一覧を作成して返す
        // 引　数：[in]parsedPath   カレントディレクトリのサブパス
        // 　　　　[in]separator    セパレータ
        // 　　　　[in]firstDepth   はじめの項目の深さ
        // 戻り値：サブパスのランタイム項目のリスト
        //=========================================================================================
        public List<AddressBarSettingRuntimeItem> CreateSubPathItem(List<string> parsedPath, string separator, int firstDepth) {
            List<AddressBarSettingRuntimeItem> result = new List<AddressBarSettingRuntimeItem>();
            string path = "";
            for (int i = 0; i < firstDepth; i++) {
                path += parsedPath[i];
                path += separator;
            }
            int depth = firstDepth;
            for (int i = firstDepth; i < parsedPath.Count; i++) {
                if (depth > MAX_PATH_LIST) {
                    break;
                }
                path += parsedPath[i];
                path += separator;
                AddressBarSettingRuntimeItem item = new AddressBarSettingRuntimeItem(parsedPath[i], path, null, depth);
                result.Add(item);
                depth++;
            }
            return result;
        }
    }
}
