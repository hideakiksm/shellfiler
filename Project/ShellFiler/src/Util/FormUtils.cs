using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：フォーム関連のライブラリ
    //=========================================================================================
    public class FormUtils {

        //=========================================================================================
        // 機　能：四角形を小さくする
        // 引　数：[in]org   元の四角形
        // 　　　　[in]size  小さくする値
        // 戻り値：小さくした後の四角形
        //=========================================================================================
        public static Rectangle ShrinkRectangle(Rectangle org, int size) {
            return new Rectangle(org.Left + size, org.Top + size, org.Width - size * 2, org.Height - size * 2);
        }

        //=========================================================================================
        // 機　能：リストビュー用の文字列を作成する
        // 引　数：[in]str  文字列
        // 戻り値：リストビュー用に259文字以内に整形した文字列
        //=========================================================================================
        public static string CreateListViewString(string str) {
            return StringUtils.MakeOmittedString(str, 259);
        }

        //=========================================================================================
        // 機　能：カーソル位置を拡張子に設定する
        // 引　数：[in]textBox  設定するコントロール
        // 戻り値：なし
        //=========================================================================================
        public static void SetCursorPosExtension(TextBox textBox) {
            string fileName = textBox.Text;
            int lastExt = fileName.LastIndexOf('.');
            if (lastExt >= 1) {
                textBox.Select(lastExt, 0);
            } else {
                textBox.Select(fileName.Length, 0);
            }
        }

        //=========================================================================================
        // 機　能：すべてのディスプレイの最大領域を取得する
        // 引　数：なし
        // 戻り値：全ディスプレイの領域
        //=========================================================================================
        public static Rectangle GetAllScreenRectangle() {
            int x1 = 0;
            int y1 = 0;
            int x2 = 0;
            int y2 = 0;
            foreach (Screen screen in Screen.AllScreens) {
                x1 = Math.Min(screen.Bounds.Left, x1);
                x2 = Math.Max(screen.Bounds.Right, x2);
                y1 = Math.Min(screen.Bounds.Top, y1);
                y2 = Math.Max(screen.Bounds.Bottom, y2);
            }
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        //=========================================================================================
        // 機　能：3状態のチェックボックスの内容を取得する
        // 引　数：[in]checkBox  チェックボックス
        // 戻り値：入力状態（true/false/null）
        //=========================================================================================
        public static BooleanFlag GetCheckThreeState(CheckBox checkBox) {
            if (checkBox.CheckState == CheckState.Indeterminate) {
                return null;
            } else if (checkBox.CheckState == CheckState.Checked) {
                return new BooleanFlag(true);
            } else {
                return new BooleanFlag(false);
            }
        }
    }
}
