using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.Command.FileList.ChangeDir {

    //=========================================================================================
    // クラス：コマンドを実行する
    // システムの特別なフォルダ{0}に変更します。
    //   書式 　 ChdirSpecial(string type)
    //   引数  　type:変更先フォルダの種類
    //           type-default:FileSystemHome
    // 　　　　　type-range:FileSystemHome=各ファイルシステムのホーム,Programs=ユーザーのプログラムグループ,Personal=ドキュメントの共通リポジトリ,MyDocuments=マイドキュメント,Favorites=ユーザーのお気に入り項目,Startup=ユーザーの[スタートアップ]プログラム,Recent=ユーザーが最近使用したドキュメント,SendTo=[送る]メニュー項目,StartMenu=[スタート]メニュー項目,MyMusic=マイミュージック,DesktopDirectory=デスクトップ,Templates=ドキュメントテンプレート,ApplicationData=現在のローミングユーザーのアプリケーション固有のデータ,LocalApplicationData=現在の非ローミングユーザーのアプリケーション固有データ,InternetCache=一時インターネットファイル,Cookies=インターネットcookies,History=インターネットの履歴項目,CommonApplicationData=すべてのユーザーが使用するアプリケーション固有のデータ,System=Systemディレクトリ,ProgramFiles=プログラムファイル,MyPictures=マイピクチャ,CommonProgramFiles=アプリケーション間で共有されるコンポーネント用,Download=ダウンロード
    //   戻り値　bool:フォルダの変更に成功または変更を開始できたときtrue、変更できなかったときfalseを返します。
    //   対応Ver 0.0.1
    //=========================================================================================
    class ChdirSpecialCommand : FileListActionCommand {
        // 変更先フォルダの種類
        private string m_type;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ChdirSpecialCommand() {
        }
        
        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        // メ　モ：[0]:変更先フォルダの種類
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_type = (string)param[0];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            IFileSystem fileSystem = FileListViewTarget.FileList.FileSystem;
            string target;
            if (m_type == "FileSystemHome") {                                 // 各ファイルシステムのホームディレクトリ
                string current = FileListViewTarget.FileList.DisplayDirectoryName;
                target = fileSystem.GetHomePath(current);
                if (target == null) {
                    return false;
                }
            } else if (m_type == "Programs") {                          // ユーザーのプログラム グループを格納するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            } else if (m_type == "Personal") {                          // ドキュメントの共通リポジトリとして機能するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            } else if (m_type == "MyDocuments") {                       // マイ ドキュメント フォルダ
                target = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            } else if (m_type == "Favorites") {                         // ユーザーのお気に入り項目の共通リポジトリとして機能するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            } else if (m_type == "Startup") {                           // ユーザーの [スタート アップ] プログラム グループに対応するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            } else if (m_type == "Recent") {                            // ユーザーが最近使用したドキュメントを格納するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            } else if (m_type == "SendTo") {                            // [送る] メニュー項目を格納するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
            } else if (m_type == "StartMenu") {                         // [スタート] メニュー項目を格納するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            } else if (m_type == "MyMusic") {                           // マイ ミュージック フォルダ
                target = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            } else if (m_type == "DesktopDirectory") {                  // デスクトップ上のファイル オブジェクトを物理的に格納するために使用されるディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            } else if (m_type == "MyComputer") {                        // マイ コンピュータ フォルダ
                target = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            } else if (m_type == "Templates") {                         // ドキュメント テンプレートの共通リポジトリとして機能するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.Templates);
            } else if (m_type == "ApplicationData") {                   // 現在のローミング ユーザーのアプリケーション固有のデータの共通リポジトリとして機能するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            } else if (m_type == "LocalApplicationData") {              // 現在の非ローミング ユーザーが使用するアプリケーション固有のデータの共通リポジトリとして機能するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            } else if (m_type == "InternetCache") {                     // 一時インターネット ファイルの共通リポジトリとして機能するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            } else if (m_type == "Cookies") {                           // インターネット cookies の共通リポジトリとして機能するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
            } else if (m_type == "History") {                           // インターネットの履歴項目の共通リポジトリとして機能するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.History);
            } else if (m_type == "CommonApplicationData") {             // すべてのユーザーが使用するアプリケーション固有のデータの共通リポジトリとして機能するディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            } else if (m_type == "System") {                            // System ディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.System);
            } else if (m_type == "ProgramFiles") {                      // プログラム ファイル ディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            } else if (m_type == "MyPictures") {                        // マイ ピクチャ フォルダ
                target = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            } else if (m_type == "CommonProgramFiles") {                // アプリケーション間で共有されるコンポーネント用のディレクトリ
                target = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
            } else if (m_type == "Download") {                          // ダウンロードディレクトリ
                target = KnownUrl.WindowsDownloadFolder;
            } else {
                return false;
            }

            return ChdirCommand.ChangeDirectory(FileListViewTarget, new ChangeDirectoryParam.Direct(target));
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.ChdirSpecialCommand;
            }
        }
    }
}
