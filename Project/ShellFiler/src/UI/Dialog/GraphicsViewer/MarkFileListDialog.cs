using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.GraphicsViewer;
using ShellFiler.UI;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog.GraphicsViewer {

    //=========================================================================================
    // クラス：マーク結果表示ダイアログ
    //=========================================================================================
    public partial class MarkFileListDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]markResult  マーク結果一覧
        // 戻り値：なし
        //=========================================================================================
        public MarkFileListDialog(SlideShowMarkResult markResult) {
            InitializeComponent();

            // ファイル件数
            int[] fileCountList = new int[9];
            for (int i = 0; i < fileCountList.Length; i++) {
                fileCountList[i] = 0;
            }

            // ファイル一覧
            this.listViewFiles.SmallImageList = UIIconManager.IconImageList;
            foreach (SlideShowMarkResult.ImageFileInfo fileInfo in markResult.FileList) {
                string itemMark;
                if (fileInfo.MarkState != 0) {
                    itemMark = string.Format(Resources.DlgSlideShowResult_MarkItemName, fileInfo.MarkState);
                } else {
                    itemMark = "-";
                }
                string itemFileName = string.Format(fileInfo.FileName);
                string[] itemName = { itemMark, itemFileName };
                ListViewItem lvItem = new ListViewItem(itemName);
                if (fileInfo.MarkState != 0) {
                    lvItem.ImageIndex = (int)(IconImageListID.GraphicsViewer_MarkFile1) + fileInfo.MarkState - 1;
                    fileCountList[fileInfo.MarkState - 1]++;
                } else {
                    lvItem.ImageIndex = 0;
                }
                this.listViewFiles.Items.Add(lvItem);
            }

            // ラベル
            Label[] labelList = {
                    this.labelMark1, this.labelMark2, this.labelMark3, this.labelMark4, this.labelMark5,
                    this.labelMark6, this.labelMark7, this.labelMark8, this.labelMark9
            };
            for (int i = 0; i < labelList.Length; i++) {
                Label label = labelList[i];
                label.ImageList = UIIconManager.IconImageList;
                label.ImageIndex = (int)(IconImageListID.GraphicsViewer_MarkFile1) + i;
                label.Text = string.Format(label.Text, fileCountList[i]);
            }
        }

        //=========================================================================================
        // 機　能：閉じるボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonClose_Click(object sender, EventArgs evt) {
            Close();
        }
    }
}
