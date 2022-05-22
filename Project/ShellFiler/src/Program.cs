using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Log.Logger;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.FileSystem;
using ShellFiler.FileViewer;
using ShellFiler.Virtual;

using ShellFiler.Terminal.TerminalSession;

namespace ShellFiler {

    //=========================================================================================
    // クラス：アプリケーションのメインエントリポイント
    //=========================================================================================
    static class Program {
        // インストールされたディレクトリ
        private static string s_installPath = null;

        // プログラム内唯一のドキュメント
        private static SfDocument s_document;

        // メインウィンドウ
        private static MainWindowForm s_mainWindow;

        // ウィンドウの管理クラス
        private static WindowManager s_windowManager;

        // スプラッシュウィンドウ（表示中ではないときnull）
        private static SplashWindow s_splashWindow;

        //=========================================================================================
        // テスト用
        //=========================================================================================
        private static void DoTest() {
#if DEBUG











#endif
        }

        //=========================================================================================
        // 機　能：アプリケーションのメインエントリポイント
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        [STAThread]
        static void Main() {
            bool success;
            try {
                DoTest();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // メインウィンドウを用意
                s_windowManager = new WindowManager();
                s_mainWindow = new MainWindowForm();
                s_splashWindow = new SplashWindow();

                // 初期化
                string exePath = Assembly.GetEntryAssembly().Location;
                s_installPath = GenericFileStringUtils.CompleteDirectoryName(Path.GetDirectoryName(exePath), "\\");
                UIIconManager.InitializeIcon(s_splashWindow, s_installPath);

                // タイトルを表示
                s_splashWindow.Show(s_mainWindow);
                Application.DoEvents();

                // ディレクトリを準備
                DirectoryManager.Initialize();              // Config/使用許諾フラグのxml読み込み元を確定
                DirectoryManager.CreateTemporary();

                // 使用許諾
                // success = LicenseAgreeDialog.ShowLicenseDialog(s_splashWindow);
                // if (!success) {
                //     return;
                // }

                // コンフィグを用意
                s_document = new SfDocument();              // Configを読み込み

                // SfHelperをテスト
                success = SfHelper.Initialize(s_mainWindow);
                if (!success) {
                    return;
                }

                // データの用意
                s_document.Initialize();
                TemporaryManager temporary = s_document.TemporaryManager;
                temporary.DeleteGarbage();

                // メインウィンドウの表示
                BaseThread.MainThread = Thread.CurrentThread;
                BaseThread.InvocationTarget = s_mainWindow;
                s_mainWindow.Initialize();
                s_splashWindow.WaitDisplay();
                s_splashWindow.Close();
                s_splashWindow.Dispose();
                s_splashWindow = null;

                // 期限の確認
                Application.Run(s_mainWindow);

                // 終了
                s_mainWindow = null;
                temporary.DeleteCurrentTemporary();
#if DEBUG
            } finally {
            }
#else
            } catch (Exception e) {
                try {
                    if (s_document != null) {
                        s_document.Dispose();
                        s_document = null;
                    }
                } catch (Exception) {
                }
                s_mainWindow = null;
                Program.UnexpectedExceptionUI(e, Resources.Msg_UnexpectedExceptionMain);
            }
#endif
        }

        //=========================================================================================
        // 機　能：プログラムを失敗させる
        // 引　数：[in]message メッセージ
        // 　　　　[in]param   メッセージに付加するパラメータ
        // 戻り値：なし
        //=========================================================================================
        public static void Abort(string message, params object[] param) {
            string detail = Environment.StackTrace;
            bool callSuccess = BaseThread.InvokeProcedureByMainThread(new AbortDelegate(AbortUI), detail, message, param);
            if (!callSuccess) {
                Program.MainWindow.BeginInvoke(new AbortDelegate(AbortUI), detail, message, param);
            }
        }
        private delegate void AbortDelegate(string detail, string message, params object[] param);
        private static void AbortUI(string detail, string message, params object[] param) {
            UnexpectedExceptionDialog dialog = new UnexpectedExceptionDialog(true, detail, message, param);
            dialog.ShowDialog(Program.MainWindow);
            Application.Exit();
        }

        //=========================================================================================
        // 機　能：想定外の例外が発生したときの処理を行う
        // 引　数：[in]exception  例外
        // 　　　　[in]message    メッセージ
        // 　　　　[in]param      メッセージに付加するパラメータ
        // 戻り値：なし
        //=========================================================================================
        public static void UnexpectedException(Exception exception, string message, params object[] param) {
            bool callSuccess = BaseThread.InvokeProcedureByMainThread(new UnexpectedExceptionDelegate(UnexpectedExceptionUI), exception, message, param);
            if (!callSuccess) {
                if (Program.MainWindow != null) {
                    Program.MainWindow.BeginInvoke(new UnexpectedExceptionDelegate(UnexpectedExceptionUI), exception, message, param);
                }
            }
        }
        private delegate void UnexpectedExceptionDelegate(Exception exception, string message, params object[] param);
        private static void UnexpectedExceptionUI(Exception exception, string message, params object[] param) {
            UnexpectedExceptionDialog dialog = new UnexpectedExceptionDialog(false, exception.ToString(), message, param);
            if (Program.MainWindow != null) {
                dialog.ShowDialog(Program.MainWindow);
            } else {
                dialog.ShowDialog();
            }
        }

        //=========================================================================================
        // プロパティ：ドキュメント
        //=========================================================================================
        public static SfDocument Document {
            get {
                return s_document;
            }
        }

        //=========================================================================================
        // プロパティ：メインウィンドウ
        //=========================================================================================
        public static MainWindowForm MainWindow {
            get {
                return s_mainWindow;
            }
        }

        //=========================================================================================
        // プロパティ：ウィンドウの管理クラス
        //=========================================================================================
        public static WindowManager WindowManager {
            get {
                return s_windowManager;
            }
        }

        //=========================================================================================
        // プロパティ：メインウィンドウ
        //=========================================================================================
        public static SplashWindow SplashWindow {
            get {
                return s_splashWindow;
            }
        }

        //=========================================================================================
        // プロパティ：ログウィンドウ
        //=========================================================================================
        public static LogPanel LogWindow {
            get {
                return s_mainWindow.LogWindow;
            }
        }

        //=========================================================================================
        // プロパティ：インストールされたディレクトリ
        //=========================================================================================
        public static string InstallPath {
            get {
                return s_installPath;
            }
        }
    }
}
