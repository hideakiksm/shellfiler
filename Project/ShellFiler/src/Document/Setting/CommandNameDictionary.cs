using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileViewer;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：コマンド名の辞書
    // 　　　　環境変数path以下の実行ファイルを自動収集して辞書としたもの
    //=========================================================================================
    public class CommandNameDictionary {
        // ファイルシステムの識別子
        private FileSystemID m_fileSystemId;

        // コマンドの入力履歴のリスト
        private List<CommandNameDictionaryItem> m_dictionaryItemList = new List<CommandNameDictionaryItem>();
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileSystemId  ファイルシステムの識別子
        // 戻り値：なし
        //=========================================================================================
        public CommandNameDictionary(FileSystemID fileSystemId) {
            m_fileSystemId = fileSystemId;
        }

        //=========================================================================================
        // 機　能：全項目を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ClearAllItem() {
            m_dictionaryItemList.Clear();
        }

        //=========================================================================================
        // 機　能：項目を追加する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void AddItem(CommandNameDictionaryItem item) {
            m_dictionaryItemList.Add(item);
        }

        //=========================================================================================
        // プロパティ：コマンドの入力履歴
        //=========================================================================================
        public List<CommandNameDictionaryItem> ItemList {
            get {
                return m_dictionaryItemList;
            }
        }
    }
}
