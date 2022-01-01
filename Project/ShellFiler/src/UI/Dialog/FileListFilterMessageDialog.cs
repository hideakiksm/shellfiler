using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.UI;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイルフィルターでのメッセージダイアログ
    //=========================================================================================
    public partial class FileListFilterMessageDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]message  表示するメッセージ
        // 戻り値：なし
        //=========================================================================================
        public FileListFilterMessageDialog(List<string> message) {
            InitializeComponent();
            this.pictureBoxIcon.Image = SystemIcons.Question.ToBitmap();

            // カラムを初期化
            ColumnHeader column = new ColumnHeader();
            column.Text = Resources.DlgDeleteStart_Column;
            column.Width = -1;
            listViewMessage.Columns.AddRange(new ColumnHeader[] {column});

            // イメージリストを初期化
            this.listViewMessage.SmallImageList = UIIconManager.IconImageList;

            // メッセージを登録
            for (int i = 0; i < message.Count; i++) {
                ListViewItem item = new ListViewItem(message[i]);
                item.ImageIndex = (int)(IconImageListID.Icon_OperationFailed);
                this.listViewMessage.Items.Add(item);
            }
        }
    }
}
