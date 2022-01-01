using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;
using ShellFiler.GraphicsViewer.Filter;

namespace ShellFiler.UI.Dialog.GraphicsViewer {

    //=========================================================================================
    // クラス：フィルターのプロパティ設定パネル（未選択の場合のダミー）
    //=========================================================================================
    public partial class FilterPropertyPanelDummy : UserControl {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parentDialog  親ダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FilterPropertyPanelDummy() {
            InitializeComponent();
        }
    }
}
