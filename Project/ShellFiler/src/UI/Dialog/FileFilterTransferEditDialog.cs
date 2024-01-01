using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.FileTask.Provider;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.FileSystem;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイル転送用ファイルフィルター設定ダイアログ
    //=========================================================================================
    public partial class FileFilterTransferEditDialog : Form {
        // 現在のフィルター設定
        private FileFilterListTransfer m_setting;

        // 転送用フィルター設定の一覧（Clone()していないため読み取り専用）
        private List<FileFilterListTransfer> m_settingList;

        // ファイルフィルターUIの実装
        private FileFilterClipboardDialog.FileFilterUIImpl m_fileFilterUIImpl;
        
        // settingListで編集中の項目のインデックス（新規のとき-1）
        private int m_targetIndex;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting     現在のフィルター設定
        // 　　　　[in]settingList 転送用フィルター設定の一覧（Clone()していないため読み取り専用）
        // 　　　　[in]targetIndex settingListで編集中の項目のインデックス（新規のとき-1）
        // 戻り値：なし
        //=========================================================================================
        public FileFilterTransferEditDialog(FileFilterListTransfer setting, List<FileFilterListTransfer> settingList, int targetIndex) {
            InitializeComponent();
            m_setting = setting;
            m_settingList = settingList;
            m_targetIndex = targetIndex;

            // 転送設定用のUIを初期化
            string[] nameSample = {
                Resources.DlgFileFilter_TransferFiterNameText,
                Resources.DlgFileFilter_TransferFiterNameXml,
                Resources.DlgFileFilter_TransferFiterNameOffice,
            };
            string[] extSample = {
                Resources.DlgFileFilter_TransferFiterExtText,
                Resources.DlgFileFilter_TransferFiterExtXml,
                Resources.DlgFileFilter_TransferFiterExtOffice,
            };
            this.comboBoxFilterName.Items.AddRange(nameSample);
            this.comboBoxFilterName.Text = setting.FilterName;
            this.comboBoxExtension.Items.AddRange(extSample);
            this.comboBoxExtension.Text = setting.TargetFileMask;
            using (Graphics graphics = this.CreateGraphics())
            using (HighDpiGraphics g = new HighDpiGraphics(graphics)) {
                this.listBoxAllFilter.Size = new System.Drawing.Size(g.X(283), g.Y(148));
                this.listBoxAllFilter.ItemHeight = g.Y(16);
                this.listBoxUse.Size = new System.Drawing.Size(g.X(283), g.Y(148));
            }

            m_fileFilterUIImpl = new FileFilterClipboardDialog.FileFilterUIImpl(this, m_setting.FilterList,
                                        this.listBoxUse, this.listBoxAllFilter, this.panelFilterProp,
                                        this.buttonAdd, this.buttonDelete, this.buttonUp, this.buttonDown);

            m_fileFilterUIImpl.SetFilterUIItem();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            m_fileFilterUIImpl.EnableUIItem();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            // ファイル名と拡張子を確認
            string filterName = this.comboBoxFilterName.Text;
            string extension = this.comboBoxExtension.Text;
            if (filterName == "") {
                InfoBox.Warning(this, Resources.DlgFileFilter_TransferEditFilterNameEmpty);
                return;
            }
            if (extension == "") {
                InfoBox.Warning(this, Resources.DlgFileFilter_TransferEditExtensionEmpty);
                return;
            }

            // 重複を確認
            string[] extensionList = extension.ToLower().Split(';');
            for (int index = 0; index < m_settingList.Count; index++) {
                FileFilterListTransfer filter = m_settingList[index];
                if (index == m_targetIndex) {
                    continue;
                }
                if (filterName == filter.FilterName) {
                    InfoBox.Warning(this, Resources.DlgFileFilter_TransferEditFilterNameDuplicate, filterName);
                    return;
                }
                string[] oldExtensionList = filter.TargetFileMask.ToLower().Split(';');
                for (int i = 0; i < extensionList.Length; i++) {
                    for (int j = 0; j < oldExtensionList.Length; j++) {
                        if (extensionList[i] == oldExtensionList[j]) {
                            InfoBox.Warning(this, Resources.DlgFileFilter_TransferEditExtensionDuplicate, extensionList[i], filter.FilterName);
                            return;
                        }
                    }
                }
            }

            // フィルターを確認
            if (m_setting.FilterList.Count == 0) {
                DialogResult yesNo = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgFileFilter_ClipConfirmEmptyFilter);
                if (yesNo != DialogResult.Yes) {
                    return;
                }
            }
            bool success = CheckFilterItem();
            if (!success) {
                return;
            }

            m_setting.FilterName = filterName;
            m_setting.TargetFileMask = extension;

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フィルターの項目の設定内容を確認する
        // 引　数：なし
        // 戻り値：正しく設定されているときtrue
        //=========================================================================================
        private bool CheckFilterItem() {
            for (int i = 0; i < m_setting.FilterList.Count; i++) {
                FileFilterItem item = m_setting.FilterList[i];
                Type componentType = Type.GetType(item.FileFilterClassPath);
                IFileFilterComponent component = (IFileFilterComponent)(componentType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                string errorMessage = component.CheckParameter(item.PropertyList);
                if (errorMessage != null) {
                    InfoBox.Warning(this, Resources.DlgFileFilter_ClipFilterError, i + 1, component.FilterName, errorMessage);
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：編集したファイルフィルターの設定
        //=========================================================================================
        public FileFilterListTransfer FileFilterSetting {
            get {
                return m_setting;
            }
        }
    }
}
