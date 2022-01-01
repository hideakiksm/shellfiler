using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;

namespace ShellFiler.UI {

    //=========================================================================================
    // クラス：マウスによるマーク操作の種類
    //=========================================================================================
    public enum MouseMarkAction {
        // はじめのファイルだけをマークし、その他をマーククリアする（Noneに変化）
        MarkFirstOnly,
        // はじめのファイルのマーク状態を反転し、その後同じ状態に変える（MarkSelect/ClearSelectに変化）
        RevertSelect,
        // エクスプローラ方式でマークする（Noneに変化）
        ExplorerMark,
        // ドラッグ中を含め、選択ファイルをマークする
        MarkSelect,
        // ドラッグ中を含め、選択ファイルをクリアする
        ClearSelect,
        // 何もしない
        None,
    }
}
