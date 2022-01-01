using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ShellFiler.Properties;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：SFのヘルパーDLLの呼び出し用
    //=========================================================================================
    public class SfHelper {
        // パスワードソルトと日付のエンコード済みデータ
        private static int s_encodedPasswordSaltDate;

        // モジュール状態が異常のときtrue
        private static bool s_invalidModule = false;

        // ライセンス状態が異常のときtrue
        private static bool s_invalidLicense = false;

        [DllImport("SfHelper.dll")]
        private static extern IntPtr InitializeExplorerMenu();

        [DllImport("SfHelper.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr ShowExplorerMenu(IntPtr hExpMenu, IntPtr hwnd, string path);

        [DllImport("SfHelper.dll")]
        private static extern bool ExecuteExplorerMenuItem(IntPtr hExpMenu, Int32 nCmd);

        [DllImport("SfHelper.dll")]
        private static extern void DeleteExplorerMenu(IntPtr hExpMenu);

        [DllImport("SfHelper.dll")]
        private static extern bool HandleMenuMessage(IntPtr hExpMenu, IntPtr hwnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("SfHelper.dll")]
        private static extern UInt32 GetPasswordSalt();

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：初期化に成功したときtrue
        //=========================================================================================
        public static bool Initialize(Form parent) {
            try {
                s_encodedPasswordSaltDate = (int)(GetPasswordSalt());
            } catch (Exception) {
                InfoBox.Error(parent, Resources.SfHelperInstall);
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：エクスプローラメニューを初期化する
        // 引　数：なし
        // 戻り値：ライブラリハンドル
        //=========================================================================================
        public static IntPtr CallInitializeExplorerMenu() {
            return InitializeExplorerMenu();
        }

        //=========================================================================================
        // 機　能：エクスプローラメニューを作成する
        // 引　数：[in]hExpMenu   ライブラリハンドル
        // 　　　　[in]hwnd       メニューを表示するウィンドウ
        // 　　　　[in]path       表示するファイルのパス
        // 戻り値：メニューのハンドル
        //=========================================================================================
        public static IntPtr CallCreateExplorerMenu(IntPtr hExpMenu, IntPtr hwnd, string path) {
            return ShowExplorerMenu(hExpMenu, hwnd, path);
        }
        
        //=========================================================================================
        // 機　能：メニュー項目を実行する
        // 引　数：[in]hExpMenu   ライブラリハンドル
        // 　　　　[in]nCmd       実行するメニュー項目の項番
        // 戻り値：実行に成功したときtrue
        //=========================================================================================
        public static bool CallExecuteExplorerMenuItem(IntPtr hExpMenu, int nCmd) {
            return ExecuteExplorerMenuItem(hExpMenu, nCmd);
        }
        
        //=========================================================================================
        // 機　能：エクスプローラメニューの後始末を行う
        // 引　数：[in]hExpMenu   ライブラリハンドル
        // 戻り値：なし
        //=========================================================================================
        public static void CallDeleteExplorerMenu(IntPtr hExpMenu) {
            DeleteExplorerMenu(hExpMenu);
        }

        //=========================================================================================
        // 機　能：メニュー表示中のメッセージを中継する
        // 引　数：[in]hExpMenu   ライブラリハンドル
        // 　　　　[in]hwnd       メニューを表示中のウィンドウのハンドル
        // 　　　　[in]message    ウィンドウメッセージ
        // 　　　　[in]wParam     ウィンドウメッセージのパラメータ
        // 　　　　[in]lParam     ウィンドウメッセージのパラメータ
        // 戻り値：メッセージを処理したときtrue
        //=========================================================================================
        public static bool CallHandleMenuMsg(IntPtr hExpMenu, IntPtr hwnd, UInt32 message, IntPtr wParam, IntPtr lParam) {
            return HandleMenuMessage(hExpMenu, hwnd, message, wParam, lParam);
        }

        //=========================================================================================
        // 機　能：パスワードソルトと日付のエンコード済みデータを返す
        // 引　数：なし
        // 戻り値：パスワードソルトと日付のエンコード済みデータ
        //=========================================================================================
        public static int GetPasswordSaltDate() {
            return s_encodedPasswordSaltDate;
        }

        //=========================================================================================
        // プロパティ：モジュール状態が異常のときtrue
        //=========================================================================================
        public static bool InvalidModule {
            get {
                return s_invalidModule;
            }
        }

        //=========================================================================================
        // プロパティ：ライセンス状態が異常のときtrue
        //=========================================================================================
        public static bool InvalidLicense {
            get {
                return s_invalidLicense;
            }
        }
    }
}
