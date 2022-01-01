using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：メッセージボックスの表示を共通化するためのクラス
    //=========================================================================================
    public class InfoBox {
        // メッセージボックスのタイトル
        private static String MESSAGEBOX_CAPTION = Resources.MessageBoxCaption;

        //=========================================================================================
        // 機　能：インフォメーションレベルでメッセージを出力する
        // 引　数：[in]parent   親となるウィンドウ
        // 　　　　[in]message  メッセージ
        // 　　　　[in]arg      メッセージの可変引数
        // 戻り値：なし
        //=========================================================================================
        public static void Information(Form parent, String message, params object[] arg) {
            string dispMessage;
            if (arg == null || arg.Length == 0) {
                dispMessage = message;
            } else {
                dispMessage = String.Format(message, arg);
            }
            if (parent == null) {
                MessageBox.Show(dispMessage, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show(parent, dispMessage, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //=========================================================================================
        // 機　能：警告レベルでメッセージを出力する
        // 引　数：[in]parent   親となるウィンドウ
        // 　　　　[in]message  メッセージ
        // 　　　　[in]arg      メッセージの可変引数
        // 戻り値：なし
        //=========================================================================================
        public static void Warning(Form parent, String message, params object[] arg) {
            string dispMessage;
            if (arg == null || arg.Length == 0) {
                dispMessage = message;
            } else {
                dispMessage = String.Format(message, arg);
            }
            if (parent == null) {
                MessageBox.Show(dispMessage, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } else {
                MessageBox.Show(parent, dispMessage, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        //=========================================================================================
        // 機　能：エラーレベルでメッセージを出力する
        // 引　数：[in]parent   親となるウィンドウ
        // 　　　　[in]message  メッセージ
        // 　　　　[in]arg      メッセージの可変引数
        // 戻り値：なし
        //=========================================================================================
        public static void Error(Form parent, String message, params object[] arg) {
            string dispMessage;
            if (arg == null || arg.Length == 0) {
                dispMessage = message;
            } else {
                dispMessage = String.Format(message, arg);
            }
            if (parent == null) {
                MessageBox.Show(dispMessage, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                MessageBox.Show(parent, dispMessage, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //=========================================================================================
        // 機　能：質問レベルでメッセージを出力する
        // 引　数：[in]parent   親となるウィンドウ
        // 　　　　[in]message  メッセージ
        // 　　　　[in]arg      メッセージの可変引数
        // 戻り値：なし
        //=========================================================================================
        public static DialogResult Question(Form parent, MessageBoxButtons buttons, String message, params object[] arg) {
            string dispMessage;
            if (arg == null || arg.Length == 0) {
                dispMessage = message;
            } else {
                dispMessage = String.Format(message, arg);
            }
            if (parent == null) {
                return MessageBox.Show(dispMessage, MESSAGEBOX_CAPTION, buttons, MessageBoxIcon.Question);
            } else {
                return MessageBox.Show(parent, dispMessage, MESSAGEBOX_CAPTION, buttons, MessageBoxIcon.Question);
            }
        }
        
        //=========================================================================================
        // 機　能：メッセージを出力する
        // 引　数：[in]parent   親となるウィンドウ
        // 　　　　[in]buttons  ボタンの種類
        // 　　　　[in]message  メッセージ
        // 　　　　[in]icon     アイコンの種類
        // 　　　　[in]arg      メッセージの可変引数
        // 戻り値：釦の選択結果
        //=========================================================================================
        public static DialogResult Message(Form parent, MessageBoxButtons buttons, MessageBoxIcon icon, String message, params object[] arg) {
            string dispMessage;
            if (arg == null || arg.Length == 0) {
                dispMessage = message;
            } else {
                dispMessage = String.Format(message, arg);
            }
            DialogResult result;
            if (parent == null) {
                result = MessageBox.Show(dispMessage, MESSAGEBOX_CAPTION, buttons, icon);
            } else {
                result = MessageBox.Show(parent, dispMessage, MESSAGEBOX_CAPTION, buttons, icon);
            }
            return result;
        }

        //=========================================================================================
        // 機　能：例外をメッセージを出力する
        // 引　数：[in]e  例外オブジェクト
        // 戻り値：なし
        //=========================================================================================
        public static void ShowException(SfException e) {
            if (Program.MainWindow == null) {
                MessageBox.Show(e.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } else {
                MessageBox.Show(Program.MainWindow, e.Message, MESSAGEBOX_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
