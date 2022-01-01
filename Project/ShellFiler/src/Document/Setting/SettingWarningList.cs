using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：設定の読み込み時の警告情報をまとめるクラス
    //=========================================================================================
    public class SettingWarningList {
        // 発生した警告
        private List<SettingLoader.Warning> m_warningList = new List<SettingLoader.Warning>();
        
        // 警告が発生したファイル名のリスト
        private List<string> m_fileNameList = new List<string>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SettingWarningList() {
        }

        //=========================================================================================
        // 機　能：警告情報を表示する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisplayWarningInfo() {
            if (m_warningList.Count > 0) {
                SettingFileWarningDialog dialog = new SettingFileWarningDialog(m_fileNameList, m_warningList);
                dialog.ShowDialog(Program.MainWindow);
            }
        }

        //=========================================================================================
        // 機　能：警告情報を追加する
        // 引　数：[in]loader       情報の読み込みに使ったローダー
        // 　　　　[in]warningList  新しい警告を集約する先の警告情報（集約しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public static void AddWarningInfo(SettingLoader loader, SettingWarningList warningList) {
            if (warningList == null) {
                // 集約しないときはダイアログで直接表示
                List<string> fileNameList = new List<string>();
                fileNameList.Add(loader.FileName);
                SettingFileWarningDialog dialog = new SettingFileWarningDialog(fileNameList, loader.WarningList);
                dialog.ShowDialog(Program.MainWindow);
            } else {
                // 集約する
                warningList.m_fileNameList.Add(loader.FileName);
                warningList.m_warningList.AddRange(loader.WarningList);
            }
        }
   }
}
