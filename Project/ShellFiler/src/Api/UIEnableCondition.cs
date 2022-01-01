using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Document;

namespace ShellFiler.Api {

    //=========================================================================================
    // クラス：UIの有効状態
    //=========================================================================================
    public class UIEnableCondition {
        public static readonly UIEnableCondition WithMark       = new UIEnableCondition();       // マークがあるとき有効
        public static readonly UIEnableCondition Always         = new UIEnableCondition();       // 常に有効
        public static readonly UIEnableCondition PathHistP      = new UIEnableCondition();       // パスヒストリ前へ
        public static readonly UIEnableCondition PathHistN      = new UIEnableCondition();       // パスヒストリ次へ
        public static readonly UIEnableCondition MarkCopy       = new UIEnableCondition();       // マークがあるとき有効（コピー）
        public static readonly UIEnableCondition MarkMove       = new UIEnableCondition();       // マークがあるとき有効（移動）
        public static readonly UIEnableCondition MarkDelete     = new UIEnableCondition();       // マークがあるとき有効（削除）
        public static readonly UIEnableCondition MarkShortcut   = new UIEnableCondition();       // マークがあるとき有効（ショートカット）
        public static readonly UIEnableCondition MarkAttribute  = new UIEnableCondition();       // マークがあるとき有効（ファイル属性の一括編集）
        public static readonly UIEnableCondition MarkPack       = new UIEnableCondition();       // マークがあるとき有効（圧縮）
        public static readonly UIEnableCondition MarkUnpack     = new UIEnableCondition();       // マークがあるとき有効（展開）
        public static readonly UIEnableCondition MarkEdit       = new UIEnableCondition();       // マークがあるとき有効（編集）
        public static readonly UIEnableCondition MarkFolderSize = new UIEnableCondition();       // マークがあるとき有効（フォルダサイズの取得）

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private UIEnableCondition() {
        }

        //=========================================================================================
        // 機　能：マーク項目かどうかをチェックする
        // 引　数：[in]condition  確認対象の項目
        // 戻り値：マーク項目のときtrue
        //=========================================================================================
        public static bool CheckMark(UIEnableCondition condition) {
            if (condition == MarkCopy) {
                if (Configuration.Current.MarklessCopy) {
                    return false;
                } else {
                    return true;
                }
            } else if (condition == MarkMove) {
                if (Configuration.Current.MarklessMove) {
                    return false;
                } else {
                    return true;
                }
            } else if (condition == MarkDelete) {
                if (Configuration.Current.MarklessDelete) {
                    return false;
                } else {
                    return true;
                }
            } else if (condition == MarkShortcut) {
                if (Configuration.Current.MarklessShortcut) {
                    return false;
                } else {
                    return true;
                }
            } else if (condition == MarkAttribute) {
                if (Configuration.Current.MarklessAttribute) {
                    return false;
                } else {
                    return true;
                }
            } else if (condition == MarkPack) {
                if (Configuration.Current.MarklessPack) {
                    return false;
                } else {
                    return true;
                }
            } else if (condition == MarkUnpack) {
                if (Configuration.Current.MarklessUnpack) {
                    return false;
                } else {
                    return true;
                }
            } else if (condition == MarkEdit) {
                if (Configuration.Current.MarklessEdit) {
                    return false;
                } else {
                    return true;
                }
            } else if (condition == MarkFolderSize) {
                if (Configuration.Current.MarklessFodlerSize) {
                    return false;
                } else {
                    return true;
                }
            } else if (condition == WithMark) {
                return true;
            }
            return false;
        }
    }
}
