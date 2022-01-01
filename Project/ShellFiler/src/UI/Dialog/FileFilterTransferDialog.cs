using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Reflection;
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
    // クラス：マークファイルのフィルターコピーダイアログ
    //=========================================================================================
    public partial class FileFilterTransferDialog : Form {
        // 使用するモード
        private FileFilterMode m_filterMode;

        // 使用する設定（詳細設定）
        private FileFilterTransferSetting m_detailSetting;

        // 使用する設定（クイック設定）
        private FileFilterTransferQuickSetting m_quickSetting;

        // 使用する設定（定義済み設定）
        private FileFilterTransferDefinedSetting m_definedSetting;

        // 定義済みページの実装（まだ初期化されていないときnull）
        private DefinedPage m_definedPage = null;

        // クイックページの実装（まだ初期化されていないときnull）
        private QuickPage m_quickPage = null;

        // 詳細ページの実装（まだ初期化されていないときnull）
        private DetailPage m_detailPage = null;

        // クリップボードモードでのクイック設定
        private List<FileFilterListClipboard> m_clipQuickList;

        // 入力結果に問題ないときtrue
        private bool m_inputSuccess = true;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]filterMode     初期状態のモード
        // 　　　　[in]detailSetting  使用する設定（詳細設定）
        // 　　　　[in]quickSetting   使用する設定（クイック設定）
        // 　　　　[in]definedSetting 使用する設定（定義済み設定）
        // 　　　　[in]clipQuickList  クリップボードモードでのクイック設定
        // 　　　　[in]srcProvider    転送元のファイル一覧
        // 　　　　[in]destProvider   転送先のファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public FileFilterTransferDialog(FileFilterMode filterMode, FileFilterTransferSetting detailSetting, FileFilterTransferQuickSetting quickSetting, FileFilterTransferDefinedSetting definedSetting, List<FileFilterListClipboard> clipQuickList, IFileProviderSrc srcProvider, IFileProviderDest destProvider) {
            InitializeComponent();
            m_filterMode = filterMode;
            m_detailSetting = detailSetting;
            m_quickSetting = quickSetting;
            m_definedSetting = definedSetting;
            m_clipQuickList = clipQuickList;

            this.textBoxSrc.Text = GetCommonRoot(srcProvider);
            this.textBoxDest.Text = destProvider.DestDirectoryName;
            this.labelSrcFileCount.Text = CreateSrcFileCount(this.labelSrcFileCount.Text, srcProvider);

            if (m_filterMode == FileFilterMode.DefinedMode) {
                this.tabControl.SelectedIndex = 0;
                m_definedPage = new DefinedPage(this);
                this.ActiveControl = this.radioShiftJISToUTF8;
            } else if (m_filterMode == FileFilterMode.QuickMode) {
                this.tabControl.SelectedIndex = 1;
                m_quickPage = new QuickPage(this);
                this.ActiveControl = this.listBoxQuickFilter;
            } else {
                this.tabControl.SelectedIndex = 2;
                m_detailPage = new DetailPage(this);
                this.ActiveControl = this.listBoxDetailFileType;
            }

#if FREE_VERSION
            // Freeware版
            this.labelFreeware.Text = Resources.Dlg_FreewareInfo;
            this.labelFreeware.BackColor = Color.LightYellow;
#endif
        }

        //=========================================================================================
        // 機　能：転送元ファイルの一覧を返す
        // 引　数：[in]srcProvider   転送元ファイルの情報
        // 戻り値：転送元ファイルの一覧
        //=========================================================================================
        private string GetCommonRoot(IFileProviderSrc srcProvider) {
            List<SimpleFileDirectoryPath> srcList = new List<SimpleFileDirectoryPath>();
            int srcCount = srcProvider.SrcItemCount;
            for (int i = 0; i < srcCount; i++) {
                srcList.Add(srcProvider.GetSrcPath(i));
            }
            string root = GenericFileStringUtils.GetCommonRoot(srcList);
            return root;
        }
            
        //=========================================================================================
        // 機　能：転送元ファイルの情報として表示する項目を返す
        // 引　数：[in]srcTemplate   文字列のテンプレート（{0}にファイル数、{1}にフォルダ数）
        // 　　　　[in]srcProvider   転送元ファイルの情報
        // 戻り値：なし
        //=========================================================================================
        private string CreateSrcFileCount(string srcTemplate, IFileProviderSrc srcProvider) {
            int fileCount = 0;
            int dirCount = 0;
            int itemCount = srcProvider.SrcItemCount;
            for (int i = 0; i < itemCount; i++) {
                SimpleFileDirectoryPath path = srcProvider.GetSrcPath(i);
                if (path.IsDirectory) {
                    dirCount++;
                } else {
                    fileCount++;
                }
            }
            string result = string.Format(srcTemplate, fileCount, dirCount);
            return result;
        }

        //=========================================================================================
        // 機　能：フォームが閉じようとされているときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FileFilterTransferDialog_FormClosing(object sender, FormClosingEventArgs evt) {
            if (DialogResult == DialogResult.OK && !m_inputSuccess) {
                evt.Cancel = true;
            }
        }

        //=========================================================================================
        // 機　能：タブが切り替えられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void tabControl_SelectedIndexChanged(object sender, EventArgs evt) {
            int index = this.tabControl.SelectedIndex;
            if (index == 0) {
                if (m_definedPage == null) {
                    m_definedPage = new DefinedPage(this);
                }
                m_definedPage.OnActive();
            } else if (index == 1) {
                if (m_quickPage == null) {
                    m_quickPage = new QuickPage(this);
                }
                m_quickPage.OnActive();
            } else {
                if (m_detailPage == null) {
                    m_detailPage = new DetailPage(this);
                }
                m_detailPage.OnActive();
            }
        }

        //=========================================================================================
        // 機　能：OKボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOk_Click(object sender, EventArgs evt) {
            int index = this.tabControl.SelectedIndex;
            if (index == 0) {
                m_filterMode = FileFilterMode.DefinedMode;
                m_inputSuccess = m_definedPage.OnOkClick();
            } else if (index == 1) {
                m_filterMode = FileFilterMode.QuickMode;
                m_inputSuccess = m_quickPage.OnOkClick();
            } else {
                m_filterMode = FileFilterMode.DetailMode;
                m_inputSuccess = m_detailPage.OnOkClick();
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        //=========================================================================================
        // プロパティ：使用するモード
        //=========================================================================================
        public FileFilterMode FilterMode {
            get {
                return m_filterMode;
            }
        }

        //=========================================================================================
        // プロパティ：使用する設定（詳細設定）
        //=========================================================================================
        public FileFilterTransferSetting DetailSetting {
            get {
                return m_detailSetting;
            }
        }

        //=========================================================================================
        // プロパティ：使用する設定（クイック設定）
        //=========================================================================================
        public FileFilterTransferQuickSetting QuickSetting {
            get {
                return m_quickSetting;
            }
        }

        //=========================================================================================
        // プロパティ：使用する設定（定義済み設定）
        //=========================================================================================
        public FileFilterTransferDefinedSetting DefinedSetting {
            get {
                return m_definedSetting;
            }
        }

        //=========================================================================================
        // クラス：詳細モードページ
        //=========================================================================================
        private class DetailPage {
            // 表示するフィルターの最大件数
            private const int FILTER_DISPLAY_MAX = 3;

            // チェックボックスの表示開始X座標
            private const int X_CHECKBOX_CONTROL = 4;

            // チェックボックスの表示開始X座標
            private const int Y_CHECKBOX_CONTROL = 4;

            // チェックボックスとテキストのX方向のマージン
            private const int CX_CHECKBOX_TEXT_MARGIN = 2;

            // チェックボックスのコントロールのサイズ
            private const int SIZE_CHECKBOX_CONTROL = 13;

            // 親ダイアログ
            private FileFilterTransferDialog m_parent;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親ダイアログ
            // 戻り値：なし
            //=========================================================================================
            public DetailPage(FileFilterTransferDialog parent) {
                m_parent = parent;
                
                // フィルター一覧
                string[] itemList = new string[m_parent.m_detailSetting.TransferList.Count];
                for (int i = 0; i < itemList.Length; i++) {
                    itemList[i] = "";
                }
                m_parent.listBoxDetailFileType.Items.AddRange(itemList);
                if (itemList.Length > 0) {
                    m_parent.listBoxDetailFileType.SelectedIndex = 0;
                }

                // その他のファイル
                string[] otherItems = {
                    Resources.DlgFileFilter_TransferDetailOtherSkip,
                    Resources.DlgFileFilter_TransferDetailOtherSrc,
                };
                m_parent.comboBoxDetailOther.Items.AddRange(otherItems);
                if (m_parent.m_detailSetting.OtherMode == FileFilterListTransferOtherMode.SkipTransfer) {
                    m_parent.comboBoxDetailOther.SelectedIndex = 0;
                } else {
                    m_parent.comboBoxDetailOther.SelectedIndex = 1;
                }

                // その他の設定
                EnableUIItem();

                // イベントを接続
                m_parent.buttonDetailAdd.Click += new EventHandler(buttonDetailAdd_Click);
                m_parent.buttonDetailEdit.Click += new EventHandler(buttonDetailEdit_Click);
                m_parent.buttonDetailDelete.Click += new EventHandler(buttonDetailDelete_Click);
                m_parent.buttonDetailSelectAll.Click += new EventHandler(buttonDetailSelectAll_Click);
                m_parent.buttonDetailSelectClear.Click += new EventHandler(buttonDetailSelectClear_Click);
                m_parent.listBoxDetailFileType.DrawItem += new DrawItemEventHandler(listBoxDetailFileType_DrawItem);
                m_parent.listBoxDetailFileType.MeasureItem += new MeasureItemEventHandler(listBoxDetailFileType_MeasureItem);
                m_parent.listBoxDetailFileType.MouseUp += new MouseEventHandler(listBoxDetailFileType_MouseUp);

                m_parent.listBoxDetailFileType.DrawMode = DrawMode.Normal;
                m_parent.listBoxDetailFileType.DrawMode = DrawMode.OwnerDrawVariable;
            }

            //=========================================================================================
            // 機　能：ページがアクティブになったときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnActive() {
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：UIの有効/無効を切り替える
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            private void EnableUIItem() {
                m_parent.buttonDetailEdit.Enabled = (m_parent.listBoxDetailFileType.Items.Count > 0);
                m_parent.buttonDetailDelete.Enabled = (m_parent.listBoxDetailFileType.Items.Count > 0);
            }

            //=========================================================================================
            // 機　能：追加ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDetailAdd_Click(object sender, EventArgs evt) {
                FileFilterListTransfer filterList = new FileFilterListTransfer();
                filterList.FilterName = CreateUniqueFilterName();
                filterList.TargetFileMask = "*";
                FileFilterTransferEditDialog dialog = new FileFilterTransferEditDialog(filterList, m_parent.m_detailSetting.TransferList, -1);
                DialogResult result = dialog.ShowDialog(m_parent);
                if (result != DialogResult.OK) {
                    return;
                }
                m_parent.m_detailSetting.TransferList.Add(dialog.FileFilterSetting);
                m_parent.listBoxDetailFileType.Items.Add("");
                m_parent.listBoxDetailFileType.SelectedIndex = m_parent.listBoxDetailFileType.Items.Count - 1;
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：編集ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDetailEdit_Click(object sender, EventArgs evt) {
                int index = m_parent.listBoxDetailFileType.SelectedIndex;
                if (index == -1) {
                    return;
                }
                FileFilterListTransfer filterList = (FileFilterListTransfer)(m_parent.m_detailSetting.TransferList[index].Clone());
                FileFilterTransferEditDialog dialog = new FileFilterTransferEditDialog(filterList, m_parent.m_detailSetting.TransferList, index);
                DialogResult result = dialog.ShowDialog(m_parent);
                if (result != DialogResult.OK) {
                    return;
                }
                m_parent.m_detailSetting.TransferList[index] = dialog.FileFilterSetting;
                m_parent.listBoxDetailFileType.DrawMode = DrawMode.Normal;
                m_parent.listBoxDetailFileType.DrawMode = DrawMode.OwnerDrawVariable;
                m_parent.listBoxDetailFileType.Invalidate();
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：削除ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDetailDelete_Click(object sender, EventArgs evt) {
                int index = m_parent.listBoxDetailFileType.SelectedIndex;
                if (index == -1) {
                    return;
                }
                List<FileFilterListTransfer> filterList = m_parent.m_detailSetting.TransferList;
                DialogResult result = InfoBox.Question(m_parent, MessageBoxButtons.YesNo, Resources.DlgFileFilter_TransferDetailDeleteConfirm, filterList[index].FilterName);
                if (result != DialogResult.Yes) {
                    return;
                }
                filterList.RemoveAt(index);
                m_parent.listBoxDetailFileType.Items.RemoveAt(index);
                if (m_parent.listBoxDetailFileType.Items.Count > 0) {
                    m_parent.listBoxDetailFileType.SelectedIndex = Math.Min(m_parent.listBoxDetailFileType.Items.Count - 1, index);
                }
                m_parent.listBoxDetailFileType.DrawMode = DrawMode.Normal;
                m_parent.listBoxDetailFileType.DrawMode = DrawMode.OwnerDrawVariable;
                m_parent.listBoxDetailFileType.Invalidate();
                EnableUIItem();
            }

            //=========================================================================================
            // 機　能：全選択ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDetailSelectAll_Click(object sender, EventArgs evt) {
                List<FileFilterListTransfer> settingList = m_parent.m_detailSetting.TransferList;
                for (int i = 0; i < settingList.Count; i++) {
                    settingList[i].UseFilter = true;
                }
                m_parent.listBoxDetailFileType.Invalidate();
            }

            //=========================================================================================
            // 機　能：全解除ボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void buttonDetailSelectClear_Click(object sender, EventArgs evt) {
                List<FileFilterListTransfer> settingList = m_parent.m_detailSetting.TransferList;
                for (int i = 0; i < settingList.Count; i++) {
                    settingList[i].UseFilter = false;
                }
                m_parent.listBoxDetailFileType.Invalidate();
            }

            //=========================================================================================
            // 機　能：新規に一意のフィルター名を作成する
            // 引　数：なし
            // 戻り値：新しいフィルター名
            //=========================================================================================
            private string CreateUniqueFilterName() {
                List<FileFilterListTransfer> filterList = m_parent.m_detailSetting.TransferList;
                for (int i = 1; i < FileFilterTransferSetting.MAX_TRANSFER_COUNT + 2; i++) {
                    string name = Resources.DlgFileFilter_TransferFiterNameDefault;
                    if (i != 1) {
                        name += "(" + i + ")";
                    }
                    bool found = false;
                    foreach (FileFilterListTransfer filter in filterList) {
                        if (filter.FilterName == name) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        return name;
                    }
                }
                Program.Abort("デフォルトフィルター名の作成に失敗しました。");
                return "";
            }

            //=========================================================================================
            // 機　能：リストボックスの項目を描画する
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listBoxDetailFileType_DrawItem(object sender, DrawItemEventArgs evt) {
                int index = evt.Index;
                if (index == -1 || index >= m_parent.m_detailSetting.TransferList.Count) {
                    return;
                }

                // フィルター名の一覧を取得
                FileFilterListTransfer filterList = m_parent.m_detailSetting.TransferList[index];
                string[] filterNameList = new string[Math.Min(FILTER_DISPLAY_MAX, filterList.FilterList.Count)];
                for (int i = 0; i < filterNameList.Length; i++) {
                    Type componentType = Type.GetType(filterList.FilterList[i].FileFilterClassPath);
                    IFileFilterComponent component = (IFileFilterComponent)(componentType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                    string detail = component.GetDisplayParameter(true, filterList.FilterList[i].PropertyList)[0];
                    if (detail == "") {
                        filterNameList[i] = component.FilterName;
                    } else {
                        filterNameList[i] = component.FilterName + "(" + detail + ")";
                    }
                }

                DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
                doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
                OwnerDrawListBoxGraphics g = new OwnerDrawListBoxGraphics(doubleBuffer.DrawingGraphics, evt.Bounds.Top, evt.Bounds.Height);

                Brush backBrush;
                Pen borderPen;
                Brush textBrush;

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
                if (filterList.UseFilter) {
                    textBrush = SystemBrushes.WindowText;
                } else {
                    textBrush = SystemBrushes.GrayText;
                }

                // 描画
                int cx = evt.Bounds.Width;
                int cy = evt.Bounds.Height;
                bool drawBottom = false;
                if (index == m_parent.listBoxDetailFileType.Items.Count - 1) {
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

                // チェックボックス
                Font font = m_parent.listBoxDetailFileType.Font;
                int x = evt.Bounds.Left + X_CHECKBOX_CONTROL;
                int y = evt.Bounds.Top + Y_CHECKBOX_CONTROL;
                Rectangle rcCheckBox = new Rectangle(x, y, SIZE_CHECKBOX_CONTROL, SIZE_CHECKBOX_CONTROL);
                ButtonState checkState = (filterList.UseFilter ? ButtonState.Checked : ButtonState.Normal);
                ControlPaint.DrawCheckBox(g.Graphics, rcCheckBox, checkState);
                g.Graphics.DrawString(filterList.FilterName, font, SystemBrushes.WindowText, new Point(x + SIZE_CHECKBOX_CONTROL + CX_CHECKBOX_TEXT_MARGIN, y));

                // 対象
                float cxTarget = GraphicsUtils.MeasureString(g.Graphics, font, Resources.DlgFileFilter_TransferDetailListTarget);
                float cxItems = GraphicsUtils.MeasureString(g.Graphics, font, Resources.DlgFileFilter_TransferDetailListItems);
                int xItemStart = (int)(Math.Max(cxTarget, cxItems)) + 5;

                g.Graphics.DrawString(Resources.DlgFileFilter_TransferDetailListTarget, font, textBrush, new Point(x, y + 18));
                g.Graphics.DrawString(filterList.TargetFileMask, font, textBrush, new Point(xItemStart, y + 18));

                // フィルター一覧
                g.Graphics.DrawString(Resources.DlgFileFilter_TransferDetailListItems, font, textBrush, new Point(x, y + 18 + 16));
                if (filterNameList.Length > 0) {
                    for (int i = 0; i < filterNameList.Length; i++) {
                        string dispItem = filterNameList[i];
                        if (i == filterNameList.Length - 1 && filterList.FilterList.Count > FILTER_DISPLAY_MAX) {
                            dispItem += string.Format(Resources.DlgFileFilter_TransferDetailListEtc, filterList.FilterList.Count);
                        }
                        g.Graphics.DrawString(dispItem, font, textBrush, new Point(xItemStart, y + i * 12 + 18 + 16));
                    }
                } else {
                    g.Graphics.DrawString(Resources.DlgFileFilter_TransferDetailListItemNone, font, textBrush, new Point(xItemStart, y + 18 + 16));
                }

                // フォーカス
                if ((evt.State & DrawItemState.Focus) == DrawItemState.Focus) {
                    Rectangle rcFocus = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 1, rect.Height - 1);
                    ControlPaint.DrawFocusRectangle(g.Graphics, rcFocus);
                }

                doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
            }

            //=========================================================================================
            // 機　能：リストボックスの項目の高さを返す
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listBoxDetailFileType_MeasureItem(object sender, MeasureItemEventArgs evt) {
                int index = evt.Index;
                if (index == -1 || index >= m_parent.m_detailSetting.TransferList.Count) {
                    return;
                }
                FileFilterListTransfer item = m_parent.m_detailSetting.TransferList[index];
                int filterCount = Math.Max(1, Math.Min(FILTER_DISPLAY_MAX, item.FilterList.Count));
                evt.ItemHeight = 28 + 16 + filterCount * 12;
            }

            //=========================================================================================
            // 機　能：リストボックス上でマウスのボタンが離されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listBoxDetailFileType_MouseUp(object sender, MouseEventArgs evt) {
                int mouseX = evt.X;
                int mouseY = evt.Y;
                int hitItem = -1;
                int yPos = 0;
                Graphics g = m_parent.listBoxDetailFileType.CreateGraphics();
                try {
                    Rectangle rcTarget = Rectangle.Empty;
                    for (int i = m_parent.listBoxDetailFileType.TopIndex; i < m_parent.listBoxDetailFileType.Items.Count; i++) {
                        // 項目の大きさを取得
                        MeasureItemEventArgs args = new MeasureItemEventArgs(null, i);
                        listBoxDetailFileType_MeasureItem(this, args);

                        // チェックボックスのヒットテスト
                        string strItem = m_parent.m_detailSetting.TransferList[i].FilterName;
                        int x = X_CHECKBOX_CONTROL;
                        int y = yPos + Y_CHECKBOX_CONTROL;
                        int cx = SIZE_CHECKBOX_CONTROL + CX_CHECKBOX_TEXT_MARGIN + (int)(GraphicsUtils.MeasureString(g, m_parent.listBoxDetailFileType.Font, strItem));
                        int cy = SIZE_CHECKBOX_CONTROL;
                        if (x <= mouseX && mouseX <= x + cx && y <= mouseY && mouseY <= y + cy) {
                            hitItem = i;
                            rcTarget = new Rectangle(0, yPos, m_parent.listBoxDetailFileType.Width, args.ItemHeight);
                            break;
                        }

                        // 次の項目へ
                        yPos += args.ItemHeight;
                    }

                    // ヒットした場合は状態を変更して再描画
                    if (hitItem == -1) {
                        return;
                    }
                    m_parent.m_detailSetting.TransferList[hitItem].UseFilter = !m_parent.m_detailSetting.TransferList[hitItem].UseFilter;
                    m_parent.listBoxDetailFileType.Invalidate(rcTarget);
                } finally {
                    g.Dispose();
                }
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：項目を取得できたときtrue
            //=========================================================================================
            public bool OnOkClick() {
                // フィルターの状況を確認
                List<FileFilterListTransfer> filterList = m_parent.m_detailSetting.TransferList;
                if (filterList.Count == 0) {
                    InfoBox.Warning(m_parent, Resources.DlgFileFilter_TransferDetailNoFilter);
                    return false;
                }
                int availableCount = 0;
                for (int i = 0; i < filterList.Count; i++) {
                    if (filterList[i].UseFilter) {
                        availableCount++;
                    }
                }
                if (availableCount == 0) {
                    InfoBox.Warning(m_parent, Resources.DlgFileFilter_TransferDetailNoAvailable);
                    return false;
                }

                // 設定を取得
                if (m_parent.comboBoxDetailOther.SelectedIndex == 0) {
                    m_parent.m_detailSetting.OtherMode = FileFilterListTransferOtherMode.SkipTransfer;
                } else {
                    m_parent.m_detailSetting.OtherMode = FileFilterListTransferOtherMode.UseSourceFile;
                }
                return true;
            }
        }

        //=========================================================================================
        // クラス：クイックモードページ
        //=========================================================================================
        private class QuickPage : FileFilterSettingComponent.IFileFilterSettingComponentNotify {
            // 親ダイアログ
            private FileFilterTransferDialog m_parent;

            // すべてのファイルフィルター
            private List<IFileFilterComponent> m_allFileFilterList;

            // 設定用のUI（初期化前のときnull）
            private FileFilterSettingComponent m_filterSettingPanel = null;

            // 現在設定されているフィルターのパラメータ（未選択のときnull）
            private List<FileFilterItem> m_settingItemList;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親ダイアログ
            // 戻り値：なし
            //=========================================================================================
            public QuickPage(FileFilterTransferDialog parent) {
                m_parent = parent;

                // フィルター一覧
                m_allFileFilterList = Program.Document.FileFilterManager.GetFileFilterList();
                m_settingItemList = new List<FileFilterItem>();
                int selectedFilter = 0;
                for (int i = 0; i < m_allFileFilterList.Count; i++) {
                    m_parent.listBoxQuickFilter.Items.Add("");
                    FileFilterItem filterItemDefault = m_allFileFilterList[i].CreateFileFilterItem();
                    if (m_parent.m_quickSetting.QuickFilterItem != null && filterItemDefault.FileFilterClassPath == m_parent.m_quickSetting.QuickFilterItem.FileFilterClassPath) {
                        selectedFilter = i;
                        m_settingItemList.Add(m_parent.QuickSetting.QuickFilterItem);
                    } else {
                        m_settingItemList.Add(filterItemDefault);
                    }
                }
                m_parent.listBoxQuickFilter.SelectedIndex = selectedFilter;

                // 対象ファイル
                string[] extSample = {
                    Resources.DlgFileFilter_TransferFiterExtText,
                    Resources.DlgFileFilter_TransferFiterExtXml,
                    Resources.DlgFileFilter_TransferFiterExtOffice,
                };
                m_parent.comboBoxQuickTargetExt.Items.AddRange(extSample);
                m_parent.comboBoxQuickTargetExt.Text = m_parent.m_quickSetting.QuickTargetFileMask;

                // その他のファイル
                string[] otherItems = {
                    Resources.DlgFileFilter_TransferDetailOtherSkip,
                    Resources.DlgFileFilter_TransferDetailOtherSrc,
                };
                m_parent.comboBoxQuickOther.Items.AddRange(otherItems);
                if (m_parent.m_quickSetting.QuickOtherMode == FileFilterListTransferOtherMode.SkipTransfer) {
                    m_parent.comboBoxQuickOther.SelectedIndex = 0;
                } else {
                    m_parent.comboBoxQuickOther.SelectedIndex = 1;
                }

                // イベントを接続
                m_parent.listBoxQuickFilter.DrawItem += new DrawItemEventHandler(listBoxQuickFilter_DrawItem);
                m_parent.listBoxQuickFilter.SelectedIndexChanged += new EventHandler(listBoxQuickFilter_SelectedIndexChanged);

                // その他
                SetFilterUIItem();
            }

            //=========================================================================================
            // 機　能：フィルターの詳細UIを設定する
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void SetFilterUIItem() {
                FileFilterItem settingItem;
                int index = m_parent.listBoxQuickFilter.SelectedIndex;
                if (index == -1) {
                    settingItem = null;
                } else {
                    settingItem = m_settingItemList[index];
                }
                if (m_filterSettingPanel == null) {
                    m_filterSettingPanel = new FileFilterSettingComponent(this, settingItem);
                    m_parent.panelQuickComponent.Controls.Add(m_filterSettingPanel);
                } else {
                    m_filterSettingPanel.SetFilterItem(settingItem);
                }
            }

            //=========================================================================================
            // 機　能：フィルター一覧の選択項目が変更されたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listBoxQuickFilter_SelectedIndexChanged(object sender, EventArgs evt) {
                SetFilterUIItem();
            }

            //=========================================================================================
            // 機　能：フィルター一覧の項目を描画する
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void listBoxQuickFilter_DrawItem(object sender, DrawItemEventArgs evt) {
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
                    backBrush = g.MarkBackBrush;
                    borderPen = g.BorderPen;
                } else {
                    // 選択中
                    backBrush = SystemBrushes.Window;
                    borderPen = g.BorderPen;
                }

                // 描画
                int cy = evt.Bounds.Height;
                if (index == m_parent.listBoxQuickFilter.Items.Count - 1) {
                    cy--;
                }
                Rectangle rect = new Rectangle(evt.Bounds.Left, evt.Bounds.Top, evt.Bounds.Width - 1, cy);
                g.Graphics.FillRectangle(backBrush, rect);
                g.Graphics.DrawRectangle(borderPen, rect);
                int x = evt.Bounds.Left + 2;
                int y = evt.Bounds.Top + 2;
                g.Graphics.DrawString(component.FilterName, m_parent.comboBoxQuickTargetExt.Font, SystemBrushes.WindowText, new Point(x, y));

                // フォーカス
                if ((evt.State & DrawItemState.Focus) == DrawItemState.Focus) {
                    Rectangle rcFocus = new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 1, rect.Height - 1);
                    ControlPaint.DrawFocusRectangle(g.Graphics, rcFocus);
                }

                doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
            }

            //=========================================================================================
            // 機　能：フィルターの詳細設定でプロパティの変更があったことを通知する
            // 引　数：[in]controlId   変更されたコントロールのID
            // 戻り値：なし
            //=========================================================================================
            public void OnNotifyPropertyChange(string controlId) {
            }

            //=========================================================================================
            // 機　能：ページがアクティブになったときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnActive() {
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：項目を取得できたときtrue
            //=========================================================================================
            public bool OnOkClick() {
                int index = m_parent.listBoxQuickFilter.SelectedIndex;
                if (index == -1) {
                    return false;
                }

                // 対象ファイル
                string extension = m_parent.comboBoxQuickTargetExt.Text;
                if (extension == "") {
                    InfoBox.Warning(m_parent, Resources.DlgFileFilter_TransferExtensionEmpty);
                    return false;
                }

                // フィルター
                bool success = CheckFilterItem();
                if (!success) {
                    return false;
                }

                // その他のファイル
                if (m_parent.comboBoxQuickOther.SelectedIndex == 0) {
                    m_parent.m_quickSetting.QuickOtherMode = FileFilterListTransferOtherMode.SkipTransfer;
                } else {
                    m_parent.m_quickSetting.QuickOtherMode = FileFilterListTransferOtherMode.UseSourceFile;
                }

                m_parent.m_quickSetting.QuickTargetFileMask = extension;

                // UI設定
                m_parent.m_quickSetting.QuickFilterItem = m_settingItemList[index];

                return true;
            }

            //=========================================================================================
            // 機　能：フィルターの項目の設定内容を確認する
            // 引　数：なし
            // 戻り値：正しく設定されているときtrue
            //=========================================================================================
            private bool CheckFilterItem() {
                int index = m_parent.listBoxQuickFilter.SelectedIndex;
                FileFilterItem item = m_settingItemList[index];
                Type componentType = Type.GetType(item.FileFilterClassPath);
                IFileFilterComponent component = (IFileFilterComponent)(componentType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                string errorMessage = component.CheckParameter(item.PropertyList);
                if (errorMessage != null) {
                    InfoBox.Warning(m_parent, Resources.DlgFileFilter_TransferFilterError, errorMessage);
                    return false;
                }
                return true;
            }
        }

        //=========================================================================================
        // クラス：定義済みモードページ
        //=========================================================================================
        private class DefinedPage {
            // 親ダイアログ
            private FileFilterTransferDialog m_parent;

            // ラジオボタン
            private List<KeyValuePair<RadioButton, FileFilterDefinedMode>> m_radioButtonList = new List<KeyValuePair<RadioButton,FileFilterDefinedMode>>();

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]parent  親ダイアログ
            // 戻り値：なし
            //=========================================================================================
            public DefinedPage(FileFilterTransferDialog parent) {
                m_parent = parent;

                // フィルター選択
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioShiftJISToUTF8, FileFilterDefinedMode.ShiftJISToUTF8));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioShiftJISToEUC,     FileFilterDefinedMode.ShiftJISToEUC));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioUTF8ToShiftJIS,    FileFilterDefinedMode.UTF8ToShiftJIS));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioEUCToShiftJIS,     FileFilterDefinedMode.EUCToShiftJIS));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioReturnCRLF,        FileFilterDefinedMode.ReturnCRLF));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioReturnLF,          FileFilterDefinedMode.ReturnLF));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioDeleteEmptyLine,   FileFilterDefinedMode.DeleteEmptyLine));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioShiftJIS4TabSpace, FileFilterDefinedMode.ShiftJIS4TabSpace));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioShiftJISSpace4Tab, FileFilterDefinedMode.ShiftJISSpace4Tab));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioShiftJIS8TabSpace, FileFilterDefinedMode.ShiftJIS8TabSpace));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioShiftJISSpace8Tab, FileFilterDefinedMode.ShiftJISSpace8Tab));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioUTF84TabSpace,     FileFilterDefinedMode.UTF84TabSpace));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioUTF8Space4Tab,     FileFilterDefinedMode.UTF8Space4Tab));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioUTF88TabSpace,     FileFilterDefinedMode.UTF88TabSpace));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioUTF8Space8Tab,     FileFilterDefinedMode.UTF8Space8Tab));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioShellFilerDump,    FileFilterDefinedMode.ShellFilerDump));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioQuick1,            FileFilterDefinedMode.Quick1));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioQuick2,            FileFilterDefinedMode.Quick2));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioQuick3,            FileFilterDefinedMode.Quick3));
                m_radioButtonList.Add(new KeyValuePair<RadioButton, FileFilterDefinedMode>(m_parent.radioQuick4,            FileFilterDefinedMode.Quick4));
                foreach (KeyValuePair<RadioButton, FileFilterDefinedMode> radioItem in m_radioButtonList) {
                    radioItem.Key.Click += new EventHandler(RadioButton_Click);
                }
                RadioButton selectedRadio = ItemToRadio(m_parent.m_definedSetting.SelectedItem);
                selectedRadio.Checked = true;
                RadioButton[] quickRadio = new RadioButton[] {
                    m_parent.radioQuick1,
                    m_parent.radioQuick2,
                    m_parent.radioQuick3,
                    m_parent.radioQuick4,
                };
                for (int i = 0; i < quickRadio.Length; i++) {
                    if (m_parent.m_clipQuickList[i] == null) {
                        quickRadio[i].Enabled = false;
                    } else {
                        quickRadio[i].Text += ":" + m_parent.m_clipQuickList[i].QuickSettingName;
                    }
                }

                // 対象ファイル
                string[] extSample = {
                    Resources.DlgFileFilter_TransferFiterExtText,
                    Resources.DlgFileFilter_TransferFiterExtXml,
                    Resources.DlgFileFilter_TransferFiterExtOffice,
                };
                m_parent.comboBoxDefinedTargetExt.Items.AddRange(extSample);
                m_parent.comboBoxDefinedTargetExt.Text = m_parent.m_definedSetting.DefinedTargetFileMask;

                // その他のファイル
                string[] otherItems = {
                    Resources.DlgFileFilter_TransferDetailOtherSkip,
                    Resources.DlgFileFilter_TransferDetailOtherSrc,
                };
                m_parent.comboBoxDefinedOther.Items.AddRange(otherItems);
                if (m_parent.m_definedSetting.DefinedOtherMode == FileFilterListTransferOtherMode.SkipTransfer) {
                    m_parent.comboBoxDefinedOther.SelectedIndex = 0;
                } else {
                    m_parent.comboBoxDefinedOther.SelectedIndex = 1;
                }
            }

            //=========================================================================================
            // 機　能：設定項目をラジオボタンに変換する
            // 引　数：[in]item  設定項目
            // 戻り値：ラジオボタン
            //=========================================================================================
            private RadioButton ItemToRadio(FileFilterDefinedMode item) {
                foreach (KeyValuePair<RadioButton, FileFilterDefinedMode> radioItem in m_radioButtonList) {
                    if (radioItem.Value == item) {
                        return radioItem.Key;
                    }
                }
                Program.Abort("未知の項目が選択されています。");
                return null;
            }

            //=========================================================================================
            // 機　能：ラジオボタンを設定項目に変換する
            // 引　数：[in]radio  ラジオボタン
            // 戻り値：設定項目
            //=========================================================================================
            private FileFilterDefinedMode RadioToItem(RadioButton radio) {
                foreach (KeyValuePair<RadioButton, FileFilterDefinedMode> radioItem in m_radioButtonList) {
                    if (radioItem.Key == radio) {
                        return radioItem.Value;
                    }
                }
                Program.Abort("未知の項目が選択されています。");
                return null;
            }

            //=========================================================================================
            // 機　能：ラジオボタンがクリックされたときの処理を行う
            // 引　数：[in]sender   イベントの送信元
            // 　　　　[in]evt      送信イベント
            // 戻り値：なし
            //=========================================================================================
            private void  RadioButton_Click(object sender, EventArgs evt) {
                foreach (KeyValuePair<RadioButton, FileFilterDefinedMode> radioItem in m_radioButtonList) {
                    if (sender != radioItem.Key) {
                        radioItem.Key.Checked = false;
                    }
                }
            }

            //=========================================================================================
            // 機　能：ページがアクティブになったときの処理を行う
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void OnActive() {
            }

            //=========================================================================================
            // 機　能：OKボタンがクリックされたときの処理を行う
            // 引　数：なし
            // 戻り値：項目を取得できたときtrue
            //=========================================================================================
            public bool OnOkClick() {
                // 対象ファイル
                string extension = m_parent.comboBoxDefinedTargetExt.Text;
                if (extension == "") {
                    InfoBox.Warning(m_parent, Resources.DlgFileFilter_TransferExtensionEmpty);
                    return false;
                }

                // その他のファイル
                if (m_parent.comboBoxDefinedOther.SelectedIndex == 0) {
                    m_parent.m_definedSetting.DefinedOtherMode = FileFilterListTransferOtherMode.SkipTransfer;
                } else {
                    m_parent.m_definedSetting.DefinedOtherMode = FileFilterListTransferOtherMode.UseSourceFile;
                }

                // 選択項目
                FileFilterDefinedMode mode = null;
                foreach (KeyValuePair<RadioButton, FileFilterDefinedMode> radioItem in m_radioButtonList) {
                    if (radioItem.Key.Checked) {
                        mode = radioItem.Value;
                    }
                }
                m_parent.m_definedSetting.SelectedItem = mode;
                m_parent.m_definedSetting.DefinedTargetFileMask = extension;

                return true;
            }
        }
    }
}
