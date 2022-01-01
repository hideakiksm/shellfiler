using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：ファイル一覧に対するコマンドのユーティリティ
    //=========================================================================================
    class FileListViewCommandUtils {
        // ダイアログ表示位置のX方向マージン
        public const int CX_DIALOG_POSITION_MARGIN = 8;

        // ダイアログ表示位置のY方向マージン
        public const int CY_DIALOG_POSITION_MARGIN = 32;

        //=========================================================================================
        // 機　能：ダイアログを画面表示の影響がない位置に移動させる
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public static void MoveDialogOuterArea(FileListView targetView, Form dialog) {
            bool isLeft = targetView.IsLeft;
            if (isLeft) {
                Point rightBottom = targetView.PointToScreen(new Point(targetView.Right, targetView.Bottom));
                dialog.Left = Math.Max(0, rightBottom.X + CX_DIALOG_POSITION_MARGIN);
                dialog.Top = rightBottom.Y - CY_DIALOG_POSITION_MARGIN - dialog.Height;
            } else {
                Point leftBottom = targetView.PointToScreen(new Point(targetView.Left, targetView.Bottom));
                dialog.Left = Math.Max(0, leftBottom.X - dialog.Width - CX_DIALOG_POSITION_MARGIN);
                dialog.Top = leftBottom.Y - CY_DIALOG_POSITION_MARGIN - dialog.Height;
            }
        }
    }
}
