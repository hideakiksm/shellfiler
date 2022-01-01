using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.UI;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：一般設定
    //=========================================================================================
    public class InitialSetting {
        // ウィンドウの左端の座標
        public Rectangle m_mainWindowRect = new Rectangle(16, 16, 1000, 640);

        // ウィンドウが最大化状態のときtrue
        public bool m_mainWindowMaximized = false;

        // ファイルビューアウィンドウの座標
        private Rectangle m_fileViewerRect = new Rectangle(16, 16, 1000, 640);

        // ファイルビューアウィンドウが最大化状態のときtrue
        private bool m_fileViewerMaximized = false;
        
        // ファイルビューアの位置情報が有効なときtrue
        private bool m_fileViewerPositionAvailable = false;

        // モニタリングビューアウィンドウの座標
        private Rectangle m_monitoringViewerRect = new Rectangle(16, 16, 1000, 640);

        // モニタリングビューアウィンドウが最大化状態のときtrue
        private bool m_monitoringViewerMaximized = false;
        
        // モニタリングビューアの位置情報が有効なときtrue
        private bool m_monitoringViewerPositionAvailable = false;

        // ターミナルのウィンドウの座標
        private Rectangle m_terminalRect = new Rectangle(16, 16, 1000, 640);

        // ターミナルのウィンドウが最大化状態のときtrue
        private bool m_terminalMaximized = false;

        // ターミナルの位置情報が有効なときtrue
        private bool m_terminalPositionAvailable = false;

        // ディレクトリの左側の初期位置
        private string m_leftInitialFolder = "";

        // ディレクトリの右側の初期位置
        private string m_rightInitialFolder = "";

        // 左ウィンドウのソートモード（設定がない場合はデフォルト）
        private FileListSortMode m_fileListSortModeLeft = new FileListSortMode();

        // 右ウィンドウのソートモード（設定がない場合はデフォルト）
        private FileListSortMode m_fileListSortModeRight = new FileListSortMode();

        // 左ウィンドウのサムネイル表示（設定がない場合はデフォルト）
        private FileListViewMode m_fileListViewModeLeft = new FileListViewMode();

        // 右ウィンドウのサムネイル表示（設定がない場合はデフォルト）
        private FileListViewMode m_fileListViewModeRight = new FileListViewMode();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public InitialSetting() {
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, InitialSetting obj) {
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
            success = loader.ExpectTag(SettingTag.InitialSetting_InitialSetting, SettingTagType.BeginObject);
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
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.InitialSetting_InitialSetting) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.InitialSetting_LeftFolder) {
                    obj.m_leftInitialFolder = loader.StringValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.InitialSetting_RightFolder) {
                    obj.m_rightInitialFolder = loader.StringValue;
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.InitialSetting_WindowMaximized) {
                    obj.m_mainWindowMaximized = loader.BoolValue;
                } else if (tagType == SettingTagType.RectangleValue && tagName == SettingTag.InitialSetting_WindowRectangle) {
                    const int MIN_WINDOW = 100;
                    Rectangle rcData = loader.RectangleValue;
                    Rectangle rcDesktop = FormUtils.GetAllScreenRectangle();
                    if (rcData.Left > rcDesktop.Width - MIN_WINDOW || rcData.Top > rcDesktop.Height - MIN_WINDOW ||
                        rcData.Width < MIN_WINDOW || rcData.Height < MIN_WINDOW) {
                        ;
                    } else {
                        obj.m_mainWindowRect = loader.RectangleValue;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.InitialSetting_ViewerMaximized) {
                    obj.m_fileViewerMaximized = loader.BoolValue;
                } else if (tagType == SettingTagType.RectangleValue && tagName == SettingTag.InitialSetting_ViewerRectangle) {
                    const int MIN_WINDOW = 100;
                    Rectangle rcData = loader.RectangleValue;
                    Rectangle rcDesktop = FormUtils.GetAllScreenRectangle();
                    if (rcData.Left > rcDesktop.Width - MIN_WINDOW || rcData.Top > rcDesktop.Height - MIN_WINDOW ||
                        rcData.Width < MIN_WINDOW || rcData.Height < MIN_WINDOW) {
                        ;
                    } else {
                        obj.m_fileViewerRect = loader.RectangleValue;
                    }
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.InitialSetting_MonitoringViewerMaximized) {
                    obj.m_monitoringViewerMaximized = loader.BoolValue;
                } else if (tagType == SettingTagType.RectangleValue && tagName == SettingTag.InitialSetting_MonitoringViewerRectangle) {
                    const int MIN_WINDOW = 100;
                    Rectangle rcData = loader.RectangleValue;
                    Rectangle rcDesktop = FormUtils.GetAllScreenRectangle();
                    if (rcData.Left > rcDesktop.Width - MIN_WINDOW || rcData.Top > rcDesktop.Height - MIN_WINDOW ||
                        rcData.Width < MIN_WINDOW || rcData.Height < MIN_WINDOW) {
                        ;
                    } else {
                        obj.m_monitoringViewerRect = loader.RectangleValue;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.InitialSetting_FileListSortModeLeft) {
                    success = FileListSortMode.LoadSetting(loader, out obj.m_fileListSortModeLeft);
                    if (obj.m_fileListSortModeLeft == null) {
                        obj.m_fileListSortModeLeft = new FileListSortMode();
                    }
                    if (!success) {
                        return success;
                    }
                    success = loader.ExpectTag(SettingTag.InitialSetting_FileListSortModeLeft, SettingTagType.EndObject);
                    if (!success) {
                        return success;
                    }

                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.InitialSetting_FileListSortModeRight) {
                    success = FileListSortMode.LoadSetting(loader, out obj.m_fileListSortModeRight);
                    if (obj.m_fileListSortModeRight == null) {
                        obj.m_fileListSortModeRight = new FileListSortMode();
                    }
                    if (!success) {
                        return success;
                    }
                    success = loader.ExpectTag(SettingTag.InitialSetting_FileListSortModeRight, SettingTagType.EndObject);
                    if (!success) {
                        return success;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.InitialSetting_FlieListViewModeLeft) {
                    success = FileListViewMode.LoadSetting(loader, out obj.m_fileListViewModeLeft);
                    if (obj.m_fileListViewModeLeft == null) {
                        obj.m_fileListViewModeLeft = new FileListViewMode();
                    }
                    if (!success) {
                        return success;
                    }
                    success = loader.ExpectTag(SettingTag.InitialSetting_FlieListViewModeLeft, SettingTagType.EndObject);
                    if (!success) {
                        return success;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.InitialSetting_FlieListViewModeRight) {
                    success = FileListViewMode.LoadSetting(loader, out obj.m_fileListViewModeRight);
                    if (obj.m_fileListViewModeRight == null) {
                        obj.m_fileListViewModeRight = new FileListViewMode();
                    }
                    if (!success) {
                        return success;
                    }
                    success = loader.ExpectTag(SettingTag.InitialSetting_FlieListViewModeRight, SettingTagType.EndObject);
                    if (!success) {
                        return success;
                    }
                }
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver       保存用クラス
        // 　　　　[in]obj         読み込み対象のオブジェクト
        // 　　　　[in]form        設定変更対象のフォーム
        // 　　　　[in]leftFiles   左のウィンドウ一覧
        // 　　　　[in]rightFiles  右のウィンドウ一覧
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, InitialSetting obj, Form form, UIFileList leftFiles, UIFileList rightFiles) {
            bool success;

            saver.StartObject(SettingTag.InitialSetting_InitialSetting);

            if (FileSystemID.IsWindows(leftFiles.FileSystem.FileSystemId)) {
                saver.AddString(SettingTag.InitialSetting_LeftFolder, leftFiles.DisplayDirectoryName);
            } else {
                saver.AddString(SettingTag.InitialSetting_LeftFolder, "");
            }
            if (FileSystemID.IsWindows(rightFiles.FileSystem.FileSystemId)) {
                saver.AddString(SettingTag.InitialSetting_RightFolder, rightFiles.DisplayDirectoryName);
            } else {
                saver.AddString(SettingTag.InitialSetting_RightFolder, "");
            }

            if (form.WindowState == FormWindowState.Maximized) {
                saver.AddBool(SettingTag.InitialSetting_WindowMaximized, true);
            } else {
                saver.AddBool(SettingTag.InitialSetting_WindowMaximized, false);
            }
            Rectangle rcWindow;
            if (form.WindowState != FormWindowState.Normal) {
                rcWindow = form.RestoreBounds;
            } else {
                rcWindow = new Rectangle(form.Left, form.Top, form.Width, form.Height);
            }
            saver.AddRectangle(SettingTag.InitialSetting_WindowRectangle, rcWindow);

            saver.AddBool(SettingTag.InitialSetting_ViewerMaximized, obj.m_fileViewerMaximized);
            saver.AddRectangle(SettingTag.InitialSetting_ViewerRectangle, obj.m_fileViewerRect);

            saver.AddBool(SettingTag.InitialSetting_MonitoringViewerMaximized, obj.m_monitoringViewerMaximized);
            saver.AddRectangle(SettingTag.InitialSetting_MonitoringViewerRectangle, obj.m_monitoringViewerRect);
            
            saver.StartObject(SettingTag.InitialSetting_FileListSortModeLeft);
            success = FileListSortMode.SaveSetting(saver, leftFiles.SortMode);
            if (!success) {
                return success;
            }
            saver.EndObject(SettingTag.InitialSetting_FileListSortModeLeft);
            saver.StartObject(SettingTag.InitialSetting_FileListSortModeRight);
            success = FileListSortMode.SaveSetting(saver, rightFiles.SortMode);
            if (!success) {
                return success;
            }
            saver.EndObject(SettingTag.InitialSetting_FileListSortModeRight);

            saver.StartObject(SettingTag.InitialSetting_FlieListViewModeLeft);
            success = FileListViewMode.SaveSetting(saver, leftFiles.FileListViewMode);
            if (!success) {
                return success;
            }
            saver.EndObject(SettingTag.InitialSetting_FlieListViewModeLeft);
            saver.StartObject(SettingTag.InitialSetting_FlieListViewModeRight);
            success = FileListViewMode.SaveSetting(saver, rightFiles.FileListViewMode);
            if (!success) {
                return success;
            }
            saver.EndObject(SettingTag.InitialSetting_FlieListViewModeRight);

            saver.EndObject(SettingTag.InitialSetting_InitialSetting);

            return saver.SaveSetting(false);
        }

        //=========================================================================================
        // 機　能：メインウィンドウを初期化する
        // 引　数：[in]form   設定変更対象のフォーム
        // 戻り値：なし
        //=========================================================================================
        public void InitializeMainWindow(Form form) {
            Rectangle rcMain = Configuration.Current.MainWindowRectDefault;
            if (rcMain == Rectangle.Empty) {
                // 前回のサイズを使用する
                form.StartPosition = FormStartPosition.Manual;
                form.Left = m_mainWindowRect.Left;
                form.Top = m_mainWindowRect.Top;
                form.Size = new Size(m_mainWindowRect.Width, m_mainWindowRect.Height);
                if (m_mainWindowMaximized) {
                    form.WindowState = FormWindowState.Maximized;
                } else {
                    form.WindowState = FormWindowState.Normal;
                }
            } else {
                // 指定サイズを使用する
                form.StartPosition = FormStartPosition.Manual;
                form.Left = rcMain.Left;
                form.Top = rcMain.Top;
                form.Size = new Size(rcMain.Width, rcMain.Height);
                form.WindowState = FormWindowState.Normal;
            }
        }

        //=========================================================================================
        // 機　能：ファイルビューアを初期化する
        // 引　数：[in]form   設定変更対象のフォーム
        // 戻り値：なし
        //=========================================================================================
        public void InitializeViewer(Form form) {
            if (m_fileViewerPositionAvailable) {
                form.StartPosition = FormStartPosition.Manual;
                form.Left = m_fileViewerRect.Left;
                form.Top = m_fileViewerRect.Top;
            }
            form.Size = new Size(m_fileViewerRect.Width, m_fileViewerRect.Height);
            if (m_fileViewerMaximized) {
                form.WindowState = FormWindowState.Maximized;
            } else {
                form.WindowState = FormWindowState.Normal;
            }
            m_fileViewerPositionAvailable = false;
        }

        //=========================================================================================
        // 機　能：モニタリングビューアを初期化する
        // 引　数：[in]form   設定変更対象のフォーム
        // 戻り値：なし
        //=========================================================================================
        public void InitializeMonitoringViewer(Form form) {
            if (m_monitoringViewerPositionAvailable) {
                form.StartPosition = FormStartPosition.Manual;
                form.Left = m_monitoringViewerRect.Left;
                form.Top = m_monitoringViewerRect.Top;
            }
            form.Size = new Size(m_monitoringViewerRect.Width, m_monitoringViewerRect.Height);
            if (m_monitoringViewerMaximized) {
                form.WindowState = FormWindowState.Maximized;
            } else {
                form.WindowState = FormWindowState.Normal;
            }
            m_monitoringViewerPositionAvailable = false;
        }

        //=========================================================================================
        // 機　能：ターミナルを初期化する
        // 引　数：[in]form   設定変更対象のフォーム
        // 戻り値：なし
        //=========================================================================================
        public void InitializeTerminal(Form form) {
            if (m_terminalPositionAvailable) {
                form.StartPosition = FormStartPosition.Manual;
                form.Left = m_terminalRect.Left;
                form.Top = m_terminalRect.Top;
            }
            form.Size = new Size(m_terminalRect.Width, m_terminalRect.Height);
            if (m_terminalMaximized) {
                form.WindowState = FormWindowState.Maximized;
            } else {
                form.WindowState = FormWindowState.Normal;
            }
            m_terminalPositionAvailable = false;
        }

        //=========================================================================================
        // 機　能：ファイルビューアが閉じられたときに状態を保存する
        // 引　数：[in]form   設定取得対象のフォーム
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseViewer(Form form) {
            if (form.WindowState == FormWindowState.Maximized) {
                m_fileViewerMaximized = true;
            } else {
                m_fileViewerMaximized = false;
            }
            if (form.WindowState != FormWindowState.Normal) {
                Rectangle rcForm = form.RestoreBounds;
                m_fileViewerRect = new Rectangle(rcForm.Left, rcForm.Top, rcForm.Width, rcForm.Height);
            } else {
                m_fileViewerRect = new Rectangle(form.Left, form.Top, form.Width, form.Height);
            }
            m_fileViewerPositionAvailable = true;
        }

        //=========================================================================================
        // 機　能：モニタリングビューアが閉じられたときに状態を保存する
        // 引　数：[in]form   設定取得対象のフォーム
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseMonitoringViewer(Form form) {
            if (form.WindowState == FormWindowState.Maximized) {
                m_monitoringViewerMaximized = true;
            } else {
                m_monitoringViewerMaximized = false;
            }
            if (form.WindowState != FormWindowState.Normal) {
                Rectangle rcForm = form.RestoreBounds;
                m_monitoringViewerRect = new Rectangle(rcForm.Left, rcForm.Top, rcForm.Width, rcForm.Height);
            } else {
                m_monitoringViewerRect = new Rectangle(form.Left, form.Top, form.Width, form.Height);
            }
            m_monitoringViewerPositionAvailable = true;
        }

        //=========================================================================================
        // 機　能：ターミナルが閉じられたときに状態を保存する
        // 引　数：[in]form   設定取得対象のフォーム
        // 戻り値：なし
        //=========================================================================================
        public void OnCloseTerminal(Form form) {
            if (form.WindowState == FormWindowState.Maximized) {
                m_terminalMaximized = true;
            } else {
                m_terminalMaximized = false;
            }
            if (form.WindowState != FormWindowState.Normal) {
                Rectangle rcForm = form.RestoreBounds;
                m_terminalRect = new Rectangle(rcForm.Left, rcForm.Top, rcForm.Width, rcForm.Height);
            } else {
                m_terminalRect = new Rectangle(form.Left, form.Top, form.Width, form.Height);
            }
            m_terminalPositionAvailable = true;
        }
        
        //=========================================================================================
        // プロパティ：ディレクトリの左側の初期位置
        //=========================================================================================
        public string LeftInitialFolder {
            get {
                return m_leftInitialFolder;
            }
        }

        //=========================================================================================
        // プロパティ：ディレクトリの右側の初期位置
        //=========================================================================================
        public string RightIntialFolder {
            get {
                return m_rightInitialFolder;
            }
        }

        //=========================================================================================
        // プロパティ：左ウィンドウのソートモード（設定がない場合はデフォルト）
        //=========================================================================================
        public FileListSortMode FileListSortModeLeft {
            get {
                return m_fileListSortModeLeft;
            }
        }

        //=========================================================================================
        // プロパティ：右ウィンドウのソートモード（設定がない場合はデフォルト）
        //=========================================================================================
        public FileListSortMode FileListSortModeRight {
            get {
                return m_fileListSortModeRight;
            }
        }

        //=========================================================================================
        // プロパティ：左ウィンドウのサムネイル表示（設定がない場合はデフォルト）
        //=========================================================================================
        public FileListViewMode FileListViewModeLeft {
            get {
                return m_fileListViewModeLeft;
            }
        }

        //=========================================================================================
        // プロパティ：右ウィンドウのサムネイル表示（設定がない場合はデフォルト）
        //=========================================================================================
        public FileListViewMode FileListViewModeRight {
            get {
                return m_fileListViewModeRight;
            }
        }
    }
}
