using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：属性の詳細ダイアログ
    //=========================================================================================
    public partial class AttributeDisplayDialog : Form {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]attr  表示する属性
        // 戻り値：なし
        //=========================================================================================
        public AttributeDisplayDialog(FileAttributes attr) {
            InitializeComponent();

            ColumnHeader name = new ColumnHeader();
            name.Text = Resources.DlgAttribute_HeadName;
            name.Width = 130;
            ColumnHeader detail = new ColumnHeader();
            detail.Text = Resources.DlgAttribute_HeadDetail;
            detail.Width = -2;
            this.listViewAttribute.Columns.AddRange(new ColumnHeader[] { name, detail });
            this.listViewAttribute.SmallImageList = UIIconManager.IconImageList;

            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.Archive) != 0,    Resources.DlgAttribute_ItemArchive1,    Resources.DlgAttribute_ItemArchive2));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.Compressed) != 0, Resources.DlgAttribute_ItemCompressed1, Resources.DlgAttribute_ItemCompressed2));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.Directory) != 0,  Resources.DlgAttribute_ItemDirectory1,  Resources.DlgAttribute_ItemDirectory2));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.Encrypted) != 0, Resources.DlgAttribute_ItemEncrypted1, Resources.DlgAttribute_ItemEncrypted2));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.Hidden) != 0, Resources.DlgAttribute_ItemHidden1, Resources.DlgAttribute_ItemHidden2));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.NotContentIndexed) != 0, Resources.DlgAttribute_ItemNotContentIndexed1, Resources.DlgAttribute_ItemNotContentIndexed2));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.Offline) != 0, Resources.DlgAttribute_ItemOffline1, Resources.DlgAttribute_ItemOffline2));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.ReadOnly) != 0, Resources.DlgAttribute_ItemReadOnly1, Resources.DlgAttribute_ItemReadOnly2));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.ReparsePoint) != 0, Resources.DlgAttribute_ItemReparsePoint1, Resources.DlgAttribute_ItemReparsePoint2));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.SparseFile) != 0, Resources.DlgAttribute_ItemSparseFile1, Resources.DlgAttribute_ItemSparseFile2));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.System) != 0, Resources.DlgAttribute_ItemSystem1, Resources.DlgAttribute_ItemTemporary1));
            this.listViewAttribute.Items.Add(CreateTaskListViewItem((attr & FileAttributes.Temporary) != 0, Resources.DlgAttribute_ItemTemporary1, Resources.DlgAttribute_ItemTemporary2));
        }

        //=========================================================================================
        // 機　能：属性一覧のListView項目を作成する
        // 引　数：[in]check  イメージをチェック状態で作成するときtrue
        // 　　　　[in]name   項目名
        // 　　　　[in]detail 項目の詳細説明
        // 戻り値：ListViewの項目
        //=========================================================================================
        private ListViewItem CreateTaskListViewItem(bool check, string name, string detail) {
            string[] itemString = { name, detail };
            ListViewItem item = new ListViewItem(itemString);
            if (check) {
                item.ImageIndex = (int)IconImageListID.Icon_FileAttributeDetailYes;
            } else {
                item.ImageIndex = (int)IconImageListID.Icon_FileAttributeDetailNo;
            }
            return item;
        }
    }
}
