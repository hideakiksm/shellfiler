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
    // クラス：クリップボード用ファイルフィルター設定ダイアログ
    //=========================================================================================
    public partial class FileFilterClipboardDialog : Form {
        // 現在のフィルター設定
        private FileFilterClipboardSetting m_setting;

        // クリップボードのとき、保存する文字コード
        private EncodingType m_clipboardCharset;

        // 保存先として使用するWindows上のファイル（クリップボードに保存するときtrue）
        private string m_saveFilePath;

        // ファイルフィルターUIの実装
        private FileFilterUIImpl m_fileFilterUIImpl;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting       現在のフィルター設定
        // 　　　　[in]saveFilePath  保存先として使用するWindows上のファイル
        // 戻り値：なし
        //=========================================================================================
        public FileFilterClipboardDialog(FileFilterClipboardSetting setting, string saveFilePath) {
            InitializeComponent();
            m_setting = setting;
            m_saveFilePath = saveFilePath;

            using (HighDpiGraphics graphics = new HighDpiGraphics(this)) {
                this.listBoxAllFilter.ItemHeight = graphics.Y(16);
            }

            // クリップボード用UI
            if (m_setting.TargetClipboard) {
                this.radioButtonClipboard.Checked = true;
            } else {
                this.radioButtonFile.Checked = true;
            }
            this.textBoxDest.Text = m_saveFilePath;
            EncodingType[] allTextEncoding = EncodingType.AllTextValue;
            string[] charsetItems = new string[allTextEncoding.Length];
            int charsetSelected = 0;
            for (int i = 0; i < allTextEncoding.Length; i++) {
                charsetItems[i] = allTextEncoding[i].DisplayName;
                if (allTextEncoding[i] == EncodingType.UTF8) {
                    charsetSelected = i;
                }
            }
            this.comboBoxCharset.Items.AddRange(charsetItems);
            this.comboBoxCharset.SelectedIndex = charsetSelected;

            // UIの実装を設定
            m_fileFilterUIImpl = new FileFilterUIImpl(this, m_setting.CurrentSetting.FilterList,
                                                      this.listBoxUse, this.listBoxAllFilter, this.panelFilterProp,
                                                      this.buttonAdd, this.buttonDelete, this.buttonUp, this.buttonDown);

            // その他初期化
            m_fileFilterUIImpl.SetFilterUIItem();
            UpdateQuickSettingUI();
            EnableUIItem();

#if FREE_VERSION
            // Freeware版
            this.labelFreeware.Text = Resources.Dlg_FreewareInfo;
            this.labelFreeware.BackColor = Color.LightYellow;
#endif
        }

        //=========================================================================================
        // 機　能：クイック設定のUIを更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateQuickSettingUI() {
            Button[] buttonList = {
                this.buttonQuick1,
                this.buttonQuick2,
                this.buttonQuick3,
                this.buttonQuick4,
            };
            for (int i = 0; i < buttonList.Length; i++) {
                if (i >= m_setting.QuickSetting.Count || m_setting.QuickSetting[i] == null) {
                    buttonList[i].Text = string.Format(Resources.DlgFileFilter_QuickDefaultDispNameButton, i + 1, i + 1);
                    buttonList[i].Enabled = false;
                } else {
                    buttonList[i].Text = m_setting.QuickSetting[i].QuickSettingName + "(&" + (i + 1) + ")";
                    buttonList[i].Enabled = true;
                }
            }
        }

        //=========================================================================================
        // 機　能：UIの有効/無効を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            if (m_fileFilterUIImpl == null) {
                return;
            }
            m_fileFilterUIImpl.EnableUIItem();
            this.buttonQuick1.Enabled = ((m_setting.QuickSetting.Count >= 1) && (m_setting.QuickSetting[0] != null));
            this.buttonQuick2.Enabled = ((m_setting.QuickSetting.Count >= 2) && (m_setting.QuickSetting[1] != null));
            this.buttonQuick3.Enabled = ((m_setting.QuickSetting.Count >= 3) && (m_setting.QuickSetting[2] != null));
            this.buttonQuick4.Enabled = ((m_setting.QuickSetting.Count >= 4) && (m_setting.QuickSetting[3] != null));
            if (this.radioButtonFile.Checked) {
                this.textBoxDest.Enabled = true;
                this.buttonRef.Enabled = true;
            } else {
                this.textBoxDest.Enabled = false;
                this.buttonRef.Enabled = false;
            }
        }

        //=========================================================================================
        // 機　能：参照ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonRef_Click(object sender, EventArgs evt) {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = GenericFileStringUtils.GetFileName(m_saveFilePath);
            sfd.InitialDirectory = GenericFileStringUtils.GetDirectoryName(m_saveFilePath);
            sfd.Filter = Resources.DlgFileFilter_ClipClipboardFileExtFilter;
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            DialogResult result = sfd.ShowDialog(Program.MainWindow);
            if (result != DialogResult.OK) {
                return;
            }
            this.textBoxDest.Text = sfd.FileName;
        }

        //=========================================================================================
        // 機　能：クイックボタンのいずれかがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonQuick_Click(object sender, EventArgs evt) {
            int quickIndex;
            if (sender == this.buttonQuick1) {
                quickIndex = 0;
            } else if (sender == this.buttonQuick2) {
                quickIndex = 1;
            } else if (sender == this.buttonQuick2) {
                quickIndex = 2;
            } else {
                quickIndex = 3;
            }

            if (quickIndex >= m_setting.QuickSetting.Count || m_setting.QuickSetting[quickIndex] == null) {
                return;
            }
            if (FileFilterListClipboard.EqualsConfig(m_setting.CurrentSetting, m_setting.QuickSetting[quickIndex])) {
                return;
            }
            DialogResult yesNo = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgFileFilter_ClipSetFromQuick, m_setting.QuickSetting[quickIndex].QuickSettingName);
            if (yesNo != DialogResult.Yes) {
                return;
            }
            m_setting.CurrentSetting = (FileFilterListClipboard)(m_setting.QuickSetting[quickIndex].Clone());

            m_fileFilterUIImpl.SetFilterUIItem();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：クイック設定ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonQuickSetting_Click(object sender, EventArgs evt) {
            bool success = CheckFilterItem();
            if (!success) {
                return;
            }

            FileFilterQuickDialog dialog = new FileFilterQuickDialog(m_setting);
            dialog.ShowDialog(this);
            UpdateQuickSettingUI();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：ファイル/クリップボードのラジオボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RadioButton_CheckedChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            if (m_setting.CurrentSetting.FilterList.Count == 0) {
                DialogResult yesNo = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgFileFilter_ClipConfirmEmptyFilter);
                if (yesNo != DialogResult.Yes) {
                    return;
                }
            }
            bool success = CheckFilterItem();
            if (!success) {
                return;
            }
            int charsetIndex = Math.Max(0, this.comboBoxCharset.SelectedIndex);
            m_clipboardCharset = EncodingType.AllTextValue[charsetIndex];
            if (radioButtonFile.Checked) {
                m_saveFilePath = this.textBoxDest.Text;
                m_setting.TargetClipboard = false;
            } else {
                m_saveFilePath = null;
                m_setting.TargetClipboard = true;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // 機　能：フィルターの項目の設定内容を確認する
        // 引　数：なし
        // 戻り値：正しく設定されているときtrue
        //=========================================================================================
        private bool CheckFilterItem() {
            for (int i = 0; i < m_setting.CurrentSetting.FilterList.Count; i++) {
                FileFilterItem item = m_setting.CurrentSetting.FilterList[i];
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
        public FileFilterClipboardSetting FileFilterSetting {
            get {
                return m_setting;
            }
        }

        //=========================================================================================
        // プロパティ：クリップボードのとき、保存する文字コード
        //=========================================================================================
        public EncodingType ClipboardCharset {
            get {
                return m_clipboardCharset;
            }
        }

        //=========================================================================================
        // プロパティ：保存先として使用するWindows上のファイル（クリップボードに保存するときnull）
        //=========================================================================================
        public string SaveFilePath {
            get {
                return m_saveFilePath;
            }
        }

        //=========================================================================================
        // クラス：UIの実装
        //=========================================================================================
        public class FileFilterUIImpl : FileFilterSettingComponent.IFileFilterSettingComponentNotify {
            // 所有フォーム
            private Form m_parent;

            // 現在のフィルター設定
            private List<FileFilterItem> m_setting;
            
            // すべてのファイルフィルター
            private List<IFileFilterComponent> m_allFileFilterList;
            
            // 設定用のUI（初期化前のときnull）
            private FileFilterSettingComponent m_filterSettingPanel = null;


            // コントロール：使用するフィルターの一覧
            private ListBox m_listBoxUse;
            
            // コントロール：すべてのフィルターの一覧
            private ListBox m_listBoxAllFilter;
            
            // コントロール：フィルターの設定パネル
            private Panel m_panelFilterProp;
            
            // コントロール：追加ボタン
            private Button m_buttonAdd;
            
            // コントロール：削除ボタン
            private Button m_buttonDelete;
            
            // コントロール：上へボタン
            private Button m_buttonUp;
            
            // コントロール：下へボタン
            private Button m_buttonDown;

            //=========================================================================================
            // 機　能：フィルター一覧のUIの項目を設定する
            // 引　数：[in]parent            所有フォーム
            // 　　　　[in]setting           現在のフィルター設定
            // 　　　　[in]listBoxUse        使用するフィルターの一覧
            // 　　　　[in]listBoxAllFilter  すべてのフィルターの一覧
            // 　　　　[in]panelFilterProp   フィルターの設定パネル
            // 　　　　[in]buttonAdd         追加ボタン
            // 　　　　[in]buttonDelete      削除ボタン
            // 　　　　[in]buttonUp          上へボタン
            // 　　　　[in]buttonDown        下へボタン
            // 戻り値：なし
            //=========================================================================================
            public FileFilterUIImpl(Form parent, List<FileFilterItem> setting,
                                    ListBox listBoxUse, ListBox listBoxAllFilter, Panel panelFilterProp,
                                    Button buttonAdd, Button buttonDelete, Button buttonUp, Button buttonDown) {
                m_parent = parent;    
                m_setting = setting;
                m_listBoxUse = listBoxUse;
                m_listBoxAllFilter = listBoxAllFilter;
                m_panelFilterProp = panelFilterProp;
                m_buttonAdd = buttonAdd;
                m_buttonDelete = buttonDelete;
                m_buttonUp = buttonUp;
                m_buttonDown = buttonDown;

                // イベントを接続
                m_listBoxUse.SelectedIndexChanged += new EventHandler(listBoxUse_SelectedIndexChanged);
                m_listBoxUse.DrawItem += new DrawItemEventHandler(listBoxUse_DrawItem);
                m_listBoxUse.MeasureItem += new MeasureItemEventHandler(listBoxUse_MeasureItem);
                m_listBoxAllFilter.DrawItem += new DrawItemEventHandler(listBoxAllFilter_DrawItem);
                m_buttonAdd.Click += new EventHandler(buttonAdd_Click);
                m_buttonDelete.Click += new EventHandler(buttonDelete_Click);
                m_buttonUp.Click += new EventHandler(buttonUp_Click);
                m_buttonDown.Click += new EventHandler(buttonDown_Click);

                // フィルター一覧
                m_allFileFilterList = Program.Document.FileFilterManager.GetFileFilterList();
                for (int i = 0; i < m_allFileFilterList.Count; i++) {
                    m_listBoxAllFilter.Items.Add("");
                }
                m_listBoxAllFilter.SelectedIndex = 0;
            }

            //=========================================================================================
            // 機　能：フィルター一覧のUIの項目を設定する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void SetFilterUIItem() {
                // 使用中フィルター
                m_listBoxUse.Items.Clear();
                string[] itemList = new string[m_setting.Count];
                for (int i = 0; i < itemList.Length; i++) {
                    itemList[i] = "";
                }
                m_listBoxUse.Items.AddRange(itemList);
                if (m_listBoxUse.Items.Count > 0) {
                    m_listBoxUse.SelectedIndex = 0;
                }

                // 詳細設定
                FileFilterItem settingItem;
                if (m_setting.Count > 0) {
                    settingItem = m_setting[0];
                } else {
                    settingItem = null;
                }
                if (m_filterSettingPanel == null) {
                    m_filterSettingPanel = new FileFilterSettingComponent(this, settingItem);
                    m_panelFilterProp.Controls.Add(m_filterSettingPanel);
                } else {
                    m_filterSettingPanel.SetFilterItem(settingItem);
                }
            }

            //=========================================================================================
            // 機　能：UIの有効/無効を切り替える
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void EnableUIItem() {
                m_buttonAdd.Enabled = (m_setting.Count < 20);
                m_buttonDelete.Enabled = (m_setting.Count > 0);
                m_buttonUp.Enabled = (m_listBoxUse.SelectedIndex > 0);
                m_buttonDown.Enabled = (m_listBoxUse.SelectedIndex != -1 && m_listBoxUse.SelectedIndex < m_listBoxUse.Items.Count - 1);
            }

            //=========================================================================================
            // 機　能：フィルターの詳細設定でプロパティの変更があったことを通知する
            // 引　数：[in]controlId   変更されたコントロールのID
            // 戻り値：なし
            //=========================================================================================
            public void OnNotifyPropertyChange(string controlId) {
                m_listBoxUse.Invalidate();
            }

            //=========================================================================================
            // 機　能：使用フィルターの項目を描画する
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listBoxUse_DrawItem(object sender, DrawItemEventArgs evt) {
                int index = evt.Index;
                if (evt.Index < 0 || m_setting.Count <= evt.Index) {
                    return;
                }
                FileFilterItem item = m_setting[evt.Index];
                Type componentType = Type.GetType(item.FileFilterClassPath);
                IFileFilterComponent component = (IFileFilterComponent)(componentType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                string[] itemDisplayLine = component.GetDisplayParameter(false, item.PropertyList);

                DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
                doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
                OwnerDrawListBoxGraphics g = new OwnerDrawListBoxGraphics(doubleBuffer.DrawingGraphics, evt.Bounds.Top, evt.Bounds.Height);

                Brush backBrush;
                Pen borderPen;
                
                // 背景色を決定
                if ((evt.State & DrawItemState.Selected) == DrawItemState.Selected) {
                    // 選択中
                    backBrush = g.MarkBackBrush;
                    borderPen = g.BorderPen;
                } else {
                    // 選択中
                    backBrush = SystemBrushes.Window;
                    borderPen = g.BorderPen;
                }

                // 描画
                int cx = evt.Bounds.Width;
                int cy = evt.Bounds.Height;
                bool drawBottom = false;
                if (index == m_listBoxUse.Items.Count - 1) {
                    cy--;
                    drawBottom = true;
                }
                Rectangle rect = new Rectangle(evt.Bounds.Left, evt.Bounds.Top, evt.Bounds.Width - 1, cy);
                g.Graphics.FillRectangle(backBrush, rect);
                g.Graphics.DrawLine(borderPen, new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top));
                g.Graphics.DrawLine(borderPen, new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Bottom));
                g.Graphics.DrawLine(borderPen, new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom));
                if (drawBottom) {
                    g.Graphics.DrawLine(borderPen, new Point(rect.Left, rect.Bottom), new Point(rect.Right, rect.Bottom));
                }

                // 文字情報
                int x = evt.Bounds.Left + g.X(2);
                int y = evt.Bounds.Top + g.Y(2);
                g.Graphics.DrawString(component.FilterName, m_listBoxAllFilter.Font, SystemBrushes.WindowText, new Point(x, y));
                for (int i = 0; i < itemDisplayLine.Length; i++) {
                    g.Graphics.DrawString(itemDisplayLine[i], m_listBoxAllFilter.Font, SystemBrushes.WindowText, new Point(x + g.X(8), y + g.X(i * 12 + 16)));
                }

                // フォーカス
                if ((evt.State & DrawItemState.Focus) == DrawItemState.Focus) {
                    Rectangle rcFocus = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 1, rect.Height - 1);
                    ControlPaint.DrawFocusRectangle(g.Graphics, rcFocus);
                }

                // 矢印
                if (index < m_listBoxUse.Items.Count - 1) {                  // 矢印(上半分)
                    Bitmap img = UIIconManager.GraphicsViewer_FilterArrow1;
                    g.Graphics.DrawImage(img, evt.Bounds.Left + cx / 2, evt.Bounds.Bottom - img.Height, img.Width, img.Height);
                }
                if (index > 0) {
                    Bitmap img = UIIconManager.GraphicsViewer_FilterArrow2;     // 矢印(下半分)
                    g.Graphics.DrawImage(img, evt.Bounds.Left + cx / 2, evt.Bounds.Top, img.Width, img.Height);
                }

                doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
            }

            //=========================================================================================
            // 機　能：使用フィルターの項目の大きさを返す
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listBoxUse_MeasureItem(object sender, MeasureItemEventArgs evt) {
                if (m_setting.Count <= evt.Index) {
                    return;
                }
                FileFilterItem item = m_setting[evt.Index];
                Type componentType = Type.GetType(item.FileFilterClassPath);
                IFileFilterComponent component = (IFileFilterComponent)(componentType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));

                string[] itemDisplayLine = component.GetDisplayParameter(false, item.PropertyList);
                using (HighDpiGraphics graphics = new HighDpiGraphics(m_parent)) {
                    if (itemDisplayLine.Length > 0) {
                        evt.ItemHeight = graphics.Y(24 + itemDisplayLine.Length * 12);
                    } else {
                        evt.ItemHeight = graphics.Y(18);
                    }
                }
            }

            //=========================================================================================
            // 機　能：すべてのフィルターの項目を描画する
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listBoxAllFilter_DrawItem(object sender, DrawItemEventArgs evt) {
                int index = evt.Index;
                IFileFilterComponent component = m_allFileFilterList[index];

                DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
                doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
                OwnerDrawListBoxGraphics g = new OwnerDrawListBoxGraphics(doubleBuffer.DrawingGraphics, evt.Bounds.Top, evt.Bounds.Height);

                Brush backBrush;
                Pen borderPen;
                
                // 背景色を決定
                if ((evt.State & DrawItemState.Selected) == DrawItemState.Selected) {
                    // 選択中
                    backBrush = g.MarkGrayBackBrush;
                    borderPen = g.GrayBorderPen;
                } else {
                    // 選択中
                    backBrush = g.GrayBackBrush;
                    borderPen = g.GrayBorderPen;
                }

                // 描画
                int cy = evt.Bounds.Height;
                if (index == m_listBoxAllFilter.Items.Count - 1) {
                    cy--;
                }
                Rectangle rect = new Rectangle(evt.Bounds.Left, evt.Bounds.Top, evt.Bounds.Width - 1, cy);
                g.Graphics.FillRectangle(backBrush, rect);
                g.Graphics.DrawRectangle(borderPen, rect);
                int x = evt.Bounds.Left + 2;
                int y = evt.Bounds.Top + 2;
                g.Graphics.DrawString(component.FilterName, m_listBoxAllFilter.Font, SystemBrushes.WindowText, new Point(x, y));

                // フォーカス
                if ((evt.State & DrawItemState.Focus) == DrawItemState.Focus) {
                    Rectangle rcFocus = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 1, rect.Height - 1);
                    ControlPaint.DrawFocusRectangle(g.Graphics, rcFocus);
                }

                doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
            }

            //=========================================================================================
            // 機　能：追加ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonAdd_Click(object sender, EventArgs evt) {
                int index = m_listBoxAllFilter.SelectedIndex;
                if (index == -1) {
                    return;
                }
                IFileFilterComponent component = m_allFileFilterList[index];
     
                // データ構造で差し替え
                FileFilterItem item = component.CreateFileFilterItem();
                m_setting.Add(item);

                // 一覧で差し替え
                m_listBoxUse.Items.Add("");
                m_listBoxUse.SelectedIndex = m_listBoxUse.Items.Count - 1;

                // UIを更新
                m_filterSettingPanel.SetFilterItem(item);
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：削除ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDelete_Click(object sender, EventArgs evt) {
                int index = m_listBoxUse.SelectedIndex;
                if (index == -1) {
                    return;
                }
     
                // データ構造で差し替え
                m_setting.RemoveAt(index);

                // 一覧で差し替え
                m_listBoxUse.Items.RemoveAt(index);
                m_listBoxUse.SelectedIndex = Math.Min(index, m_listBoxUse.Items.Count - 1);
                m_listBoxUse.Invalidate();

                // UIを更新
                FileFilterItem item;
                if (m_listBoxUse.Items.Count == 0) {
                    item = null;
                } else {
                    item = m_setting[m_listBoxUse.SelectedIndex];
                }
                m_filterSettingPanel.SetFilterItem(item);
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：上へボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonUp_Click(object sender, EventArgs evt) {
                // 設定を入れ替え
                int index = m_listBoxUse.SelectedIndex;
                if (index <= 0) {
                    return;
                }
                FileFilterItem targetItem = m_setting[index];
                m_setting[index] = m_setting[index - 1];
                m_setting[index - 1] = targetItem;

                // 設定をUIに反映
                m_listBoxUse.DrawMode = DrawMode.Normal;
                m_listBoxUse.DrawMode = DrawMode.OwnerDrawVariable;
                m_listBoxUse.Invalidate();
                m_listBoxUse.SelectedIndex = index - 1;

                m_filterSettingPanel.SetFilterItem(targetItem);
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：下へボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDown_Click(object sender, EventArgs evt) {
                // 設定を入れ替え
                int index = m_listBoxUse.SelectedIndex;
                if (index == m_listBoxUse.Items.Count - 1) {
                    return;
                }
                FileFilterItem targetItem = m_setting[index];
                m_setting[index] = m_setting[index + 1];
                m_setting[index + 1] = targetItem;

                // 設定をUIに反映
                m_listBoxUse.DrawMode = DrawMode.Normal;
                m_listBoxUse.DrawMode = DrawMode.OwnerDrawVariable;
                m_listBoxUse.Invalidate();
                m_listBoxUse.SelectedIndex = index + 1;

                m_filterSettingPanel.SetFilterItem(targetItem);
                EnableUIItem();
            }
            
            //=========================================================================================
            // 機　能：使用フィルター一覧で項目の選択が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listBoxUse_SelectedIndexChanged(object sender, EventArgs evt) {
                if (m_filterSettingPanel == null) {             // 初期化中
                    return;
                }
                int index = m_listBoxUse.SelectedIndex;
                FileFilterItem targetItem;
                if (index == -1) {
                    targetItem = null;
                } else {
                    targetItem = m_setting[index];
                }
                m_filterSettingPanel.SetFilterItem(targetItem);
                EnableUIItem();
            }
        }
    }
}
