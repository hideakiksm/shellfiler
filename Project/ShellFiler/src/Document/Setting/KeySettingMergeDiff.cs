using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.FileList;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.Setting;
using ShellFiler.Command.FileList.Window;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Cursor;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.FileViewer.View;
using ShellFiler.Command.GraphicsViewer;
using ShellFiler.Command.GraphicsViewer.Edit;
using ShellFiler.Command.GraphicsViewer.File;
using ShellFiler.Command.GraphicsViewer.View;
using ShellFiler.Command.GraphicsViewer.Filter;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：キーのカスタマイズ情報のバージョンアップによる差分
    //=========================================================================================
    public class KeySettingMergeDiff {
        // ファイル一覧で自動マージされたキーの一覧
        private List<KeyItemSetting> m_fileListMergedList = new List<KeyItemSetting>();

        // ファイル一覧で自動マージできなかったキーの一覧
        private List<KeyItemSetting> m_fileListConflictList = new List<KeyItemSetting>();

        // ファイルビューアで自動マージされたキーの一覧
        private List<KeyItemSetting> m_fileViewerMergedList = new List<KeyItemSetting>();

        // ファイルビューアで自動マージできなかったキーの一覧
        private List<KeyItemSetting> m_fileViewerConflictList = new List<KeyItemSetting>();

        // グラフィックビューアで自動マージされたキーの一覧
        private List<KeyItemSetting> m_graphicsViewerMergedList = new List<KeyItemSetting>();

        // グラフィックビューアで自動マージできなかったキーの一覧
        private List<KeyItemSetting> m_graphicsViewerConflictList = new List<KeyItemSetting>();

        // モニタリングビューアで自動マージされたキーの一覧
        private List<KeyItemSetting> m_monitoringViewerMergedList = new List<KeyItemSetting>();

        // モニタリングビューアで自動マージできなかったキーの一覧
        private List<KeyItemSetting> m_monitoringViewerConflictList = new List<KeyItemSetting>();

        // 設定ファイルで定義済みのバージョン
        private int m_prevConfigVersion;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]prevConfigVersion  設定ファイルで定義済みのバージョン
        // 戻り値：なし
        //=========================================================================================
        public KeySettingMergeDiff(int pervConfigVersion) {
            m_prevConfigVersion = pervConfigVersion;
        }

        //=========================================================================================
        // 機　能：差分を検出する
        // 引　数：[in]fileList                 ファイル一覧のキー定義
        // 　　　　[in]defaultFileList          ファイル一覧のデフォルトキー定義
        // 　　　　[in]fileViewer               ファイルビューアのキー定義
        // 　　　　[in]defaultFileViewer        ファイルビューアのデフォルトキー定義
        // 　　　　[in]graphicsViewer           グラフィックビューアのキー定義
        // 　　　　[in]defaultGraphicsViewer    グラフィックビューアのデフォルトキー定義
        // 　　　　[in]monitoringViewer         モニタリングビューアのキー定義
        // 　　　　[in]defaultMonitoringViewer  モニタリングビューアのデフォルトキー定義
        // 戻り値：なし
        //=========================================================================================
        public void CheckDifference(KeyItemSettingList fileList, KeyItemSettingList defaultFileList,
                                    KeyItemSettingList fileViewer, KeyItemSettingList defaultFileViewer,
                                    KeyItemSettingList graphicsViewer, KeyItemSettingList defaultGraphicsViewer,
                                    KeyItemSettingList monitoringViewer, KeyItemSettingList defaultMonitoringViewer) {
            CheckKeyList(fileList, defaultFileList, m_fileListMergedList, m_fileListConflictList);
            CheckKeyList(fileViewer, defaultFileViewer, m_fileViewerMergedList, m_fileViewerConflictList);
            CheckKeyList(graphicsViewer, defaultGraphicsViewer, m_graphicsViewerMergedList, m_graphicsViewerConflictList);
            CheckKeyList(monitoringViewer, defaultMonitoringViewer, m_monitoringViewerMergedList, m_monitoringViewerConflictList);
        }

        //=========================================================================================
        // 機　能：指定されたキー一覧内での差分を検出する
        // 引　数：[in]configList    読み込んだキー一覧
        // 　　　　[in]defaultList   デフォルトでのキー一覧
        // 　　　　[in]mergedList    マージした結果を返すリスト
        // 　　　　[in]conflictList  マージできなかったキーを返すリスト
        // 戻り値：なし
        //=========================================================================================
        public void CheckKeyList(KeyItemSettingList configList, KeyItemSettingList defaultList, List<KeyItemSetting> mergedList, List<KeyItemSetting> conflictList) {
            // すべてのコマンドを対象
            List<UIResource> commandList = UIResource.CommandList;
            foreach (UIResource command in commandList) {
                if (command.FirstVersion <= m_prevConfigVersion) {
                    continue;
                }

                // 特定のコマンドデフォルト定義を取得
                List<KeyItemSetting> defaultSettingList = defaultList.GetSettingFromCommandClass(command.CommandType.FullName);
                if (defaultSettingList == null) {
                    continue;
                }

                // デフォルト定義のキーが使用済みか？
                foreach (KeyItemSetting defaultSetting in defaultSettingList) {
                    KeyItemSetting configSetting = configList.GetSettingFromKey(defaultSetting.KeyState);
                    if (configSetting == null) {
                        configList.AddSetting(defaultSetting);
                        mergedList.Add(defaultSetting);
                    } else {
                        conflictList.Add(configSetting);
                    }
                }
            }
        }

        //=========================================================================================
        // プロパティ：差分の検出数
        //=========================================================================================
        public int DifferenceCount {
            get {
                int count = m_fileListMergedList.Count + m_fileListConflictList.Count +
                            m_fileViewerMergedList.Count + m_fileViewerConflictList.Count +
                            m_graphicsViewerMergedList.Count + m_graphicsViewerConflictList.Count +
                            m_monitoringViewerMergedList.Count + m_monitoringViewerConflictList.Count;
                return count;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧で自動マージされたキーの一覧
        //=========================================================================================
        public List<KeyItemSetting> FileListMergedList {
            get {
                return m_fileListMergedList;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧で自動マージできなかったキーの一覧
        //=========================================================================================
        public List<KeyItemSetting> FileListConflictList {
            get {
                return m_fileListConflictList;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルビューアで自動マージされたキーの一覧
        //=========================================================================================
        public List<KeyItemSetting> FileViewerMergedList {
            get {
                return m_fileViewerMergedList;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルビューアで自動マージできなかったキーの一覧
        //=========================================================================================
        public List<KeyItemSetting> FileViewerConflictList {
            get {
                return m_fileViewerConflictList;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアで自動マージされたキーの一覧
        //=========================================================================================
        public List<KeyItemSetting> GraphicsViewerMergedList {
            get {
                return m_graphicsViewerMergedList;
            }
        }

        //=========================================================================================
        // プロパティ：グラフィックビューアで自動マージできなかったキーの一覧
        //=========================================================================================
        public List<KeyItemSetting> GraphicsViewerConflictList {
            get {
                return m_graphicsViewerConflictList;
            }
        }

        //=========================================================================================
        // プロパティ：モニタリングビューアで自動マージされたキーの一覧
        //=========================================================================================
        public List<KeyItemSetting> MonitoringViewerMergedList {
            get {
                return m_monitoringViewerMergedList;
            }
        }

        //=========================================================================================
        // プロパティ：モニタリングビューアで自動マージできなかったキーの一覧
        //=========================================================================================
        public List<KeyItemSetting> MonitoringViewerConflictList {
            get {
                return m_monitoringViewerConflictList;
            }
        }

        //=========================================================================================
        // プロパティ：設定ファイルで定義済みのバージョン
        //=========================================================================================
        public int PrevConvigVersion {
            get {
                return m_prevConfigVersion;
            }
        }
    }
}
