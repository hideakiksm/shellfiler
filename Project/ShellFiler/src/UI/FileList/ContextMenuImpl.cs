using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.ChangeDir;
using ShellFiler.Command.FileList.Mouse;
using ShellFiler.Command.FileList.FileOperation;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileTask.Condition;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.UI.FileList.DefaultList;
using ShellFiler.UI.FileList.ThumbList;
using ShellFiler.Util;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：エクスプローラメニューの実装
    //=========================================================================================
    public class ExplorerMenuImpl {
        // 親となるビュー
        private readonly FileListView m_parentView;

        // エクスプローラメニューを表示中のとき、そのハンドル（IntPtr.Zero:表示中ではない）
        private IntPtr m_hExplorerMenu = IntPtr.Zero;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]uiFileList  ファイル一覧
        // 　　　　[in]component   ファイル一覧コンポーネント
        // 戻り値：なし
        //=========================================================================================
        public ExplorerMenuImpl(FileListView parentView) {
            m_parentView = parentView;
        }

        //=========================================================================================
        // 機　能：コンテキストメニューを表示する
        // 引　数：[in]fileName    対象のファイル名
        // 　　　　[in]menuPos     メニューを表示する位置
        // 戻り値：なし
        //=========================================================================================
        public void ShowMenu(string fileName, Point ptMenu) {
            m_hExplorerMenu = SfHelper.CallInitializeExplorerMenu();
            IntPtr hMenu = SfHelper.CallCreateExplorerMenu(m_hExplorerMenu, m_parentView.Handle, fileName);
            int nCmd = Win32API.Win32TrackPopupMenuEx(hMenu, Win32API.TPM_RETURNCMD, ptMenu.X, ptMenu.Y, m_parentView.Handle, IntPtr.Zero);
            if (nCmd != 0) {
                SfHelper.CallExecuteExplorerMenuItem(m_hExplorerMenu, nCmd);
            }
            SfHelper.CallDeleteExplorerMenu(m_hExplorerMenu);
            m_hExplorerMenu = IntPtr.Zero;
        }

        //=========================================================================================
        // 機　能：エクスプローラメニュー表示中のとき、メッセージを中継する
        // 引　数：[in]message    ウィンドウメッセージ
        // 　　　　[in]wParam     ウィンドウメッセージのパラメータ
        // 　　　　[in]lParam     ウィンドウメッセージのパラメータ
        // 戻り値：なし
        //=========================================================================================
        public void HandleExplorerMenuMessage(int message, IntPtr wParam, IntPtr lParam) {
            if (m_hExplorerMenu != IntPtr.Zero) {
                SfHelper.CallHandleMenuMsg(m_hExplorerMenu, m_parentView.Handle, (UInt32)message, wParam, lParam);
            }
        }

        //=========================================================================================
        // 機　能：マウス上の位置にメニューを表示する際の表示位置を返す
        // 引　数：[in]fileListView  表示するウィンドウ
        // 戻り値：表示位置
        //=========================================================================================
        public static Point GetPointOnMouse(FileListView fileListView) {
            Point ptMouseClient = fileListView.PointToClient(Cursor.Position);
            int mouseX = ptMouseClient.X;
            int mouseY = ptMouseClient.Y;
            mouseX = Math.Min(Math.Max(0, mouseX), fileListView.ClientRectangle.Width);
            mouseY = Math.Min(Math.Max(0, mouseY), fileListView.Height);
            Point ptMenu = fileListView.PointToScreen(new Point(mouseX, mouseY));
            return ptMenu;
        }
    }
}
