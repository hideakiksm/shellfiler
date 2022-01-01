using System;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Command;
using ShellFiler.UI;
using ShellFiler.UI.FileList;

namespace ShellFiler.Command.FileList {
    
    //=========================================================================================
    // クラス：マウス、キー、ツールバーなどのイベントを受けて実行するコマンド
    //=========================================================================================
    public abstract class FileListActionCommand : ActionCommand {
        // ファイル一覧のビュー（ファイルリストを使用しないときnull）
        private FileListView m_fileListView;

        // 起動の原因となったマウスイベント（マウス起因ではないときnull）
        private MouseEventArgs m_mouseEvent;

        // 実行時のオプション
        private ActionCommandOption m_commandOption;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FileListActionCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public virtual void SetParameter(params object[] param) {
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]fileListView 対象のファイルリスト（ファイルリストを使用しないときnull）
        // 　　　　[in]mouseEvt     マウス入力イベント（マウス起因ではないときnull）
        // 　　　　[in]option       実行時のオプション
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(FileListView fileListView, MouseEventArgs mouseEvt, ActionCommandOption option) {
            m_fileListView = fileListView;
            m_mouseEvent = mouseEvt;
            m_commandOption = option;
        }

        //=========================================================================================
        // 機　能：別コマンドのコンテキストから初期化する
        // 引　数：[in]parent         移行前のコンテキスト
        // 　　　　[in]optionInherit  移行前から引き継ぐときnull
        // 　　　　[in]option         実行時のオプション（移行前から引き継ぐときNone）
        // 戻り値：なし
        //=========================================================================================
        public void InitializeFromParent(FileListActionCommand parent, bool optionInherit, ActionCommandOption option) {
            m_fileListView = parent.m_fileListView;
            m_mouseEvent = parent.m_mouseEvent;
            if (optionInherit) {
                m_commandOption = parent.m_commandOption;
            } else {
                m_commandOption = option;
            }
        }
        
        //=========================================================================================
        // 機　能：処理後の機能を実行する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void PostExecute() {
            if ((m_commandOption & ActionCommandOption.MoveNext) == ActionCommandOption.MoveNext) {
                // 実行後、カーソルを下に移動
                m_fileListView.FileListViewComponent.CursorNext();
            }
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：[in]param パラメータ
        // 戻り値：実行結果
        //=========================================================================================
        public abstract object Execute();

        //=========================================================================================
        // プロパティ：メインウィンドウ
        //=========================================================================================
        public MainWindowForm MainWindow {
            get {
                return m_fileListView.MainWindow;
            }
        }

        //=========================================================================================
        // プロパティ：対象パスのファイルリストビュー
        //=========================================================================================
        public FileListView FileListViewTarget {
            get {
                return m_fileListView;
            }
        }

        //=========================================================================================
        // プロパティ：対象パスのファイルリストビュー
        //=========================================================================================
        public FileListView FileListViewOpposite {
            get {
                return m_fileListView.ParentPanel.Opposite.FileListView;
            }
        }

        //=========================================================================================
        // プロパティ：対象パスのファイルリストビューコンポーネント
        //=========================================================================================
        public IFileListViewComponent FileListComponentTarget {
            get {
                return m_fileListView.FileListViewComponent;
            }
        }

        //=========================================================================================
        // プロパティ：反対パスのファイルリストビューコンポーネント
        //=========================================================================================
        public IFileListViewComponent FileListComponentOpposite {
            get {
                return m_fileListView.ParentPanel.Opposite.FileListView.FileListViewComponent;
            }
        }

        //=========================================================================================
        // プロパティ：起動の原因となったマウスイベント（マウス起因ではないときnull）
        //=========================================================================================
        public MouseEventArgs MouseEvent {
            get {
                return m_mouseEvent;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public abstract UIResource UIResource {
            get;
        }
    }
}
