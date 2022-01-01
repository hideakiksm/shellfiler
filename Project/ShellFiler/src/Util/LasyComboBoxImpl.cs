using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.UI;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：遅延初期化機能付きのコンボボックス
    //=========================================================================================
    public class LasyComboBoxImpl {
        // 制御対象のコンボボックス
        private ComboBox m_target;

        // 項目の一覧
        private object[] m_itemList;

        // 選択された項目のインデックス
        private int m_selectedIndex;

        // 項目が初期化されたときtrue
        private bool m_itemInitialized = false;

        // 選択が変更されたときのイベント
        public event EventHandler SelectedIndexChanged;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]target         制御対象のコンボボックス
        // 　　　　[in]itemList       項目の一覧
        // 　　　　[in]selectedIndex  初期選択状態の項目のインデックス
        // 戻り値：モノクロになったビットマップ
        //=========================================================================================
        public LasyComboBoxImpl(ComboBox target, object[] itemList, int selectedIndex) {
            m_target = target;
            m_itemList = itemList;
            m_selectedIndex = selectedIndex;

            // コンボボックスの初期化
            m_target.Items.Clear();
            m_target.Items.Add(itemList[selectedIndex]);
            m_target.SelectedIndex = 0;
            m_target.DropDown += new EventHandler(ComboBox_DropDown_GotFocus);
            m_target.GotFocus += new EventHandler(ComboBox_DropDown_GotFocus);
            m_target.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
        }

        //=========================================================================================
        // 機　能：コンボボックスのドロップダウンが表示された/フォーカスを得たときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ComboBox_DropDown_GotFocus(object sender, EventArgs evt) {
            if (m_itemInitialized) {
                return;
            }
            m_itemInitialized = true;
            m_target.Items.Clear();
            m_target.Items.AddRange(m_itemList);
            m_target.SelectedIndex = m_selectedIndex;
        }

        //=========================================================================================
        // 機　能：選択中のインデックスが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs evt) {
            m_selectedIndex = m_target.SelectedIndex;
            if (SelectedIndexChanged != null) {
                SelectedIndexChanged(sender, evt);
            }
        }

        //=========================================================================================
        // プロパティ：選択された項目のインデックス
        //=========================================================================================
        public int SelectedIndex {
            get {
                return m_selectedIndex;
            }
            set {
                if (!m_itemInitialized) {
                    ComboBox_DropDown_GotFocus(this, null);
                }
                m_target.SelectedIndex = value;
            }
        }
    }
}
