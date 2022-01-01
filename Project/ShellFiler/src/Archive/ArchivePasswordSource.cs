using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;
using ShellFiler.Properties;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：パスワードの提供クラス
    //=========================================================================================
    public class ArchivePasswordSource {
        // 手入力パスワードの最大試行回数
        private const int MAX_MANUAL_PASSWORD_RETRY_COUNT = 3;

        // 展開中のアーカイブ名
        private string m_archiveName;

        // 自動パスワード入力の設定
        private List<ArchiveAutoPasswordItem> m_settingList = null;

        // 実行中の自動入力パスワードのインデックス（最後まで実行したときはm_settingList.Count）
        private int m_nextAutoPasswordIndex = 0;

        // 手入力パスワードのリトライ回数
        private int m_manualPasswordRetryCount = 0;

        // 外部指定のパスワード（ない場合はnull）
        private string m_specifiedPassword;

        // 使用したパスワードに対応する表示名（パスワードが自動入力でないときはnull）
        private string m_usedPasswordDisplayName;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting      自動パスワード入力の設定（ない場合はnull）
        // 　　　　[in]archiveName  アーカイブ名
        // 　　　　[in]clipString   クリップボードの文字列（ない場合はnull）
        // 　　　　[in]specified    指定のパスワード（ない場合はnull）
        // 戻り値：なし
        //=========================================================================================
        public ArchivePasswordSource(ArchiveAutoPasswordSetting setting, string archiveName, string clipString, string specified) {
            m_archiveName = archiveName;
            if (setting != null) {
                setting.LoadData();
            }
            m_specifiedPassword = specified;
            m_settingList = CreatePasswordList(setting, clipString, specified);
        }

        //=========================================================================================
        // 機　能：自動入力するパスワードの一覧を作成する
        // 引　数：[in]setting      自動パスワード入力の設定（ない場合はnull）
        // 　　　　[in]clipString   クリップボードの文字列（ない場合はnull）
        // 　　　　[in]specified    指定のパスワード（ない場合はnull）
        // 戻り値：パスワードの項目
        //=========================================================================================
        private List<ArchiveAutoPasswordItem> CreatePasswordList(ArchiveAutoPasswordSetting setting, string clipString, string specified) {
            List<ArchiveAutoPasswordItem> result = new List<ArchiveAutoPasswordItem>();

            // 指定のパスワード
            if (specified != null) {
                result.Add(new ArchiveAutoPasswordItem(null, specified));
            }

            // 自動設定の一覧をコピー（変更されないようにコピー）
            if (setting != null) {
                result.AddRange(setting.AutoPasswordItemList);
            }

            // クリップボードの各行をコピー
            if (clipString != null) {
                int clipLineCount = 0;
                string[] clipLineList = StringUtils.SeparateLine(clipString);
                foreach (string clipLine in clipLineList) {
                    // そのまま追加
                    if (clipLineCount >= ArchiveAutoPasswordSetting.MAX_PASSWORD_CLIPBOARD_COUNT) {
                        break;
                    }
                    result.Add(new ArchiveAutoPasswordItem(Resources.ArchiveAutoPasswordClipboard, clipLine));
                    clipLineCount++;

                    // 空白をトリムして追加
                    if (clipLineCount >= ArchiveAutoPasswordSetting.MAX_PASSWORD_CLIPBOARD_COUNT) {
                        break;
                    }
                    string clipLineTrim = clipLine.Trim();
                    if (clipLine != clipLineTrim) {
                        result.Add(new ArchiveAutoPasswordItem(Resources.ArchiveAutoPasswordClipboard, clipLineTrim));
                        clipLineCount++;
                    }
                }
            }
            return result;
        }

        //=========================================================================================
        // 機　能：次のパスワードを取得する
        // 引　数：[in]parentDialog   パスワード入力ダイアログの親となるダイアログ
        // 　　　　[out]nextPassword  次のパスワード
        // 　　　　[out]isCancel      キャンセルされたときtrue
        // 　　　　[out]isRetryOver   リトライ回数を超えたときtrue
        // 戻り値：なし
        //=========================================================================================
        public void GetNextPassword(Form parentDialog, out string nextPassword, out bool isCancel, out bool isRetryOver) {
            if (m_nextAutoPasswordIndex < m_settingList.Count) {
                // 自動入力
                nextPassword = m_settingList[m_nextAutoPasswordIndex].Password;
                m_usedPasswordDisplayName = m_settingList[m_nextAutoPasswordIndex].DisplayName;
                m_nextAutoPasswordIndex++;
                isCancel = false;
                isRetryOver = false;
            } else if (m_manualPasswordRetryCount < MAX_MANUAL_PASSWORD_RETRY_COUNT) {
                // 手入力
                nextPassword = InputPassword(parentDialog);
                m_manualPasswordRetryCount++;
                m_usedPasswordDisplayName = null;
                isRetryOver = false;
                if (nextPassword == null) {
                    isCancel = true;
                } else {
                    isCancel = false;
                }
            } else {
                // 再試行エラー
                nextPassword = null;
                m_usedPasswordDisplayName = null;
                isRetryOver = true;
                isCancel = false;
            }
        }

        //=========================================================================================
        // 機　能：パスワードを入力する
        // 引　数：[in]parentDialog   パスワード入力ダイアログの親となるダイアログ
        // 戻り値：入力されたパスワード（null:キャンセル）
        //=========================================================================================
        public string InputPassword(Form parentDialog) {
            object result;
            bool success = BaseThread.InvokeFunctionByMainThread(new InputPasswordDelegate(InputPasswordUI), out result, m_archiveName, parentDialog);
            if (!success) {
                return null;
            }
            return (string)result;
        }
        private delegate string InputPasswordDelegate(string archiveName, Form parentDialog);
        private static string InputPasswordUI(string archiveName, Form parentDialog) {
            ArchivePasswordDialog dialog = new ArchivePasswordDialog(archiveName);
            DialogResult result = dialog.ShowDialog(parentDialog);
            if (result != DialogResult.OK) {
                return null;
            }
            return dialog.Password;
        }

        //=========================================================================================
        // プロパティ：使用したパスワードに対応する表示名（パスワードが自動入力でないときはnull）
        //=========================================================================================
        public string UsedPasswordDisplayName {
            get {
                return m_usedPasswordDisplayName;
            }
        }
    }
}
