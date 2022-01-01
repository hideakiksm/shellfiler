using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using ShellFiler.FileTask;

namespace ShellFiler.UI {

    //=========================================================================================
    // クラス：ステータスバーのエラーメッセージ表示用の実装
    //=========================================================================================
    public class StatusBarErrorMessageImpl {
        // エラーメッセージ消去までのカウント（100ms×指定回数）
        private int ERROR_MESSAGE_COUNT = 30;

        // 対象のステータスバー本体
        private StatusStrip m_parent;

        // ステータスバー本体のIContainer
        private IContainer m_parentContainer;

        // メッセージを表示する領域
        private ToolStripStatusLabel m_messageLabel;

        // 発生したエラーメッセージ（null:エラーなし）
        private string m_errorMessage = null;

        // エラーメッセージのレベル
        private FileOperationStatus.LogLevel m_errorLevel = FileOperationStatus.LogLevel.Null;

        // 使用するアイコン
        private IconImageListID m_errorMessageIcon = IconImageListID.None;

        // 元のラベルの背景色
        private Color m_originalBackColor;

        // 元のラベルの前景色
        private Color m_originalForeColor;

        // エラーメッセージ表示用のタイマー
        private Timer m_timerShowError = null;

        // エラーメッセージ消去までの時間
        private int m_errorMessageRemainCount = 0;

        // オブジェクトが破棄されたときtrue
        private bool m_disposed = false;

        // エラー表示完了後、ステータスバーの再描画を行うdelegate（空文字に戻すときnull）
        private RefreshStatusBarDelegate m_refreshStatusBar;

        // エラー表示完了後、ステータスバーの再描画を行うdelegate
        public delegate void RefreshStatusBarDelegate();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent           この実装を所有するステータスバーコンポーネント
        // 　　　　[in]parentContainer  ステータスバーのIContainer
        // 　　　　[in]messageLabel     エラーメッセージを表示するラベル
        // 　　　　[in]refreshDelegate  エラー表示完了後、ステータスバーの再描画を行うdelegate（空文字に戻すときnull）
        // 戻り値：なし
        //=========================================================================================
        public StatusBarErrorMessageImpl(StatusStrip parent, IContainer parentContainer, ToolStripStatusLabel messageLabel, RefreshStatusBarDelegate refreshDelegate) {
            m_parent = parent;
            m_parentContainer = parentContainer;
            m_messageLabel = messageLabel; 
            m_refreshStatusBar = refreshDelegate;

            m_originalBackColor = m_messageLabel.BackColor;
            m_originalForeColor = m_messageLabel.ForeColor;
        }

        //=========================================================================================
        // 機　能：破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            m_disposed = true;
            if (m_timerShowError != null) {
                m_timerShowError.Dispose();
                m_timerShowError = null;
            }
        }

        //=========================================================================================
        // 機　能：エラーメッセージを更新する
        // 引　数：[in]message   エラーメッセージ
        // 　　　　[in]level     エラーのレベル
        // 　　　　[in]icon      使用するアイコン
        // 　　　　[in]wait      表示時間
        // 戻り値：なし
        //=========================================================================================
        public void ShowErrorMessageWorkThread(string message, FileOperationStatus.LogLevel level, IconImageListID icon, DisplayTime wait) {
            BaseThread.InvokeProcedureByMainThread(new ShowErrorMessageDelegate(ShowErrorMessageUI), message, level, icon, wait);
        }
        private delegate void ShowErrorMessageDelegate(string message, FileOperationStatus.LogLevel level, IconImageListID icon, DisplayTime wait);
        public void ShowErrorMessageUI(string message, FileOperationStatus.LogLevel level, IconImageListID icon, DisplayTime wait) {
            if (m_disposed) {
                return;
            }
            m_errorMessage = message;
            m_errorLevel = level;
            m_errorMessageIcon = icon;
            if (wait == DisplayTime.Infinite) {
                m_errorMessageRemainCount = int.MaxValue / 2;
            } else if (wait == DisplayTime.Default) {
                m_errorMessageRemainCount = ERROR_MESSAGE_COUNT;
            } else {
                Program.Abort("時間識別子が未定義です。");
            }
            RefreshStatusBar();

            if (m_timerShowError == null) {
                m_timerShowError = new System.Windows.Forms.Timer(m_parentContainer);
                m_timerShowError.Tick += new System.EventHandler(this.TimerShowError_Tick);
                m_timerShowError.Start();
            }
        }

        //=========================================================================================
        // 機　能：ステータスバーを更新する
        // 引　数：なし
        // 戻り値：更新を完了したときtrue、メッセージ未設定のときfalse
        //=========================================================================================
        public bool RefreshStatusBar() {
            if (m_errorMessage == null) {
                return false;
            }

            if (m_errorLevel == FileOperationStatus.LogLevel.Info) {
                m_messageLabel.BackColor = Configuration.Current.TextViewerInfoStatusBackColor;
                m_messageLabel.ForeColor = Configuration.Current.TextViewerInfoStatusTextColor;
            } else if (m_errorLevel == FileOperationStatus.LogLevel.Error) {
                m_messageLabel.BackColor = Configuration.Current.TextViewerErrorStatusBackColor;
                m_messageLabel.ForeColor = Configuration.Current.TextViewerErrorStatusTextColor;
            } else {
                m_messageLabel.BackColor = SystemColors.Control;
                m_messageLabel.ForeColor = SystemColors.ControlText;
            }
            m_messageLabel.ImageAlign = ContentAlignment.MiddleLeft;
            m_messageLabel.ImageIndex = (int)m_errorMessageIcon;
            m_messageLabel.Text = m_errorMessage;
            return true;
        }

        //=========================================================================================
        // 機　能：エラー表示用タイマーの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TimerShowError_Tick(object sender, EventArgs evt) {
            m_errorMessageRemainCount--;
            if (m_errorMessageRemainCount <= 0) {
                m_errorMessage = null;
                m_errorLevel = FileOperationStatus.LogLevel.Null;
                m_errorMessageRemainCount = 0;
                m_messageLabel.BackColor = m_originalBackColor;
                m_messageLabel.ForeColor = m_originalForeColor;
                m_messageLabel.ImageIndex = -1;
                if (m_refreshStatusBar != null) {
                    m_refreshStatusBar();
                } else {
                    m_messageLabel.Text = "";
                }
                m_timerShowError.Stop();
                m_timerShowError.Dispose();
                m_timerShowError = null;
            }
        }

        //=========================================================================================
        // プロパティ：エラーを表示中のときtrue
        //=========================================================================================
        public bool HasError {
            get {
                return (m_timerShowError != null);
            }
        }

        //=========================================================================================
        // プロパティ：エラーの表示時間
        //=========================================================================================
        public enum DisplayTime {
            Infinite,                   // 表示したまま
            Default,                    // デフォルト
        }
    }
}
