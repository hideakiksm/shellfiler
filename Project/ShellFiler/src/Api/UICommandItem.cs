using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Command;
using ShellFiler.Document;
using ShellFiler.Document.Setting;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：コマンドの種類
    //=========================================================================================
    public class UICommandItem {
        // タスクマネージャ
        public static UICommandItem TaskManager = new UICommandItem.Controlbar(-1);
        // ステータスバー
        public static UICommandItem StatusBar = new UICommandItem.Controlbar(-2);
        // 状態一覧パネル 実行中のタスクはありません
        public static UICommandItem StateList_NoTask = new UICommandItem.Controlbar(-3);
        // 状態一覧パネル SSH接続がありません
        public static UICommandItem StateList_NoSSH = new UICommandItem.Controlbar(-4);

        // 実行するコマンド（サブクラスによる指定のときnull）
        private ActionCommandMoniker m_command;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        protected UICommandItem() {
            m_command = null;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]command  実行するコマンド
        // 戻り値：なし
        //=========================================================================================
        public UICommandItem(ActionCommandMoniker command) {
            m_command = command;
        }

        //=========================================================================================
        // プロパティ：実行するコマンド（サブクラスによる指定のときnull）
        //=========================================================================================
        public ActionCommandMoniker ActionCommandMoniker {
            get {
                return m_command;
            }
        }

        //=========================================================================================
        // クラス：コントロールバーからのコマンド
        //=========================================================================================
        public class Controlbar : UICommandItem {
            // IDの数値
            private int m_id;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]id  IDの値
            // 戻り値：なし
            //=========================================================================================
            public Controlbar(int id) {
                m_id = id;
                m_command = null;
            }
        }

        //=========================================================================================
        // クラス：ヘッダのクリックによるソートコマンド
        //=========================================================================================
        public class HeaderSortCommand : UICommandItem {
            // ソートする項目
            private FileListSortMode.Method m_sortMethod;
            
            // ソートの向き
            private FileListSortMode.Direction m_sortDirection;
        
            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]sortMethod     ソートする項目
            // 　　　　[in]sortDirection  ソートの向き
            // 戻り値：表示幅
            //=========================================================================================
            public HeaderSortCommand(FileListSortMode.Method sortMethod, FileListSortMode.Direction sortDirection) {
                m_sortMethod = sortMethod;
                m_sortDirection = sortDirection;
            }

            //=========================================================================================
            // プロパティ：ソートする項目
            //=========================================================================================
            public FileListSortMode.Method SortMode {
                get {
                    return m_sortMethod;
                }
            }
            
            //=========================================================================================
            // プロパティ：ソートの向き
            //=========================================================================================
            public FileListSortMode.Direction SortDirection {
                get {
                    return m_sortDirection;
                }
            }
        }
    }
}
