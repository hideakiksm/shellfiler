using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document.Setting;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：キー定義のUI用ソートクラス
    //=========================================================================================
    class KeyItemSettingSorter : IComparer<KeyItemSetting> {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public KeyItemSettingSorter() {
        }

        //=========================================================================================
        // 機　能：キー設定をソートする
        // 引　数：[in]list  ソート対象のリスト（ソート結果を返す）
        // 戻り値：なし
        //=========================================================================================
        public void SortKeySetting(List<KeyItemSetting> list) {
            list.Sort(this);
        }

        //=========================================================================================
        // 機　能：2つの設定を比較する
        // 引　数：[in]file1  比較対象の設定１
        // 　　　　[in]file2  比較対象の設定２
        // 戻り値：比較結果
        //=========================================================================================
        public int Compare(KeyItemSetting setting1, KeyItemSetting setting2) {
            KeyState keyState1 = setting1.KeyState;
            KeyState keyState2 = setting2.KeyState;
            return CompareKey(keyState1, keyState2);
        }

        //=========================================================================================
        // 機　能：2つのキーを比較する
        // 引　数：[in]file1  比較対象のキー１
        // 　　　　[in]file2  比較対象のキー２
        // 戻り値：比較結果
        //=========================================================================================
        public int CompareKey(KeyState keyState1, KeyState keyState2) {
            int order1 = KeyNameUtils.GetKeyOrder(keyState1.Key);
            int order2 = KeyNameUtils.GetKeyOrder(keyState2.Key);
            if (order1 < order2) {
                return -1;
            } else if (order1 > order2) {
                return 1;
            }
            int shift1 = (keyState1.IsShift ? 1 : 0) + (keyState1.IsControl ? 2 : 0) + (keyState1.IsAlt ? 4 : 0);
            int shift2 = (keyState2.IsShift ? 1 : 0) + (keyState2.IsControl ? 2 : 0) + (keyState2.IsAlt ? 4 : 0);
            return shift1 - shift2;
        }
    }
}
