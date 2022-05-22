using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.UI.ControlBar;
using ShellFiler.Util;

namespace ShellFiler.UI.FileList {

    //=========================================================================================
    // クラス：アドレスバーのドロップダウンボックス
    //=========================================================================================
    public class AddressBarDropDown : ToolStripComboBox {
        // テキスト入力領域Ｘ位置のマージン
        private const int X_MARGIN_TEXT = 22;

        // テキスト入力領域Ｙ位置のマージン
        private const int Y_MARGIN_TEXT = 5;

        // テキスト入力領域幅のマージン
        private const int CX_MARGIN_TEXT = 39;

        // アイコンボタンの幅
        private const int CX_ICON_BUTTON = 18;

        // アイコンボタンの高さ
        private const int CY_ICON_BUTTON = 18;


        // 左ウィンドウのときtrue
        private bool m_isLeft;

        // このアドレスバーに対応するファイル一覧
        private FileListView m_fileListView;
        
        // 親となるアドレスバー
        private AddressBarStrip m_parent;

        // 入力領域
        private TextBox m_textBoxInput;

        // ディレクトリ名
        private string m_directory;

        // ディレクトリの区切り文字
        private string m_separator;
        
        // パスマスクの文字列
        private string m_pathMask;

        // フォルダアイコン
        private Bitmap m_iconFolder;

        // ファイル一覧フィルターのアイコンボタン
        private IconButton m_buttonFileListFilter;

        // ボタンのアイコンサイズ
        private Size m_iconSize;

        // ドロップダウン項目の情報
        List<AddressBarSettingRuntimeItem> m_dropDownItemList = null;

        // 一時的に作成したBitmapのリスト（次回初期化時にDispose）
        List<Bitmap> m_temporaryBitmapList = new List<Bitmap>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]isLeft  左ウィンドウのときtrue
        // 　　　　[in]view    対応するファイル一覧
        // 　　　　[in]parent  親となるアドレスバー
        // 戻り値：なし
        //=========================================================================================
        public AddressBarDropDown(bool isLeft, FileListView view, AddressBarStrip parent) {
            m_isLeft = isLeft;
            m_fileListView = view;
            m_parent = parent;

            this.Name = "addressBarComboBox";
            this.Size = new System.Drawing.Size(AddressBarStrip.CX_MIN_COMBO_BOX, AddressBarStrip.CY_COMBO_BOX);
            this.FlatStyle = FlatStyle.Standard;
            this.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            this.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            int x = m_parent.Location.X + this.ComboBox.Location.X + MainWindowForm.X(X_MARGIN_TEXT);
            int y = m_parent.Location.Y + this.ComboBox.Location.Y + MainWindowForm.Y(Y_MARGIN_TEXT);
            int cx = this.ComboBox.Size.Width;
            int cy = this.ComboBox.Size.Height;
            m_textBoxInput = new TextBox();
            m_textBoxInput.Location = new Point(x, y);
            m_textBoxInput.Size = new Size(cx - MainWindowForm.X(CX_MARGIN_TEXT), cy);
            m_textBoxInput.BorderStyle = BorderStyle.None;
            m_textBoxInput.KeyDown += new KeyEventHandler(this.ComboBoxDirectory_KeyDown);
            m_textBoxInput.LostFocus += new EventHandler(this.ComboBoxDirectory_LostFocus);
            this.ComboBox.Controls.Add(m_textBoxInput);

            m_iconSize = new Size(MainWindowForm.X(CX_ICON_BUTTON), MainWindowForm.Y(CY_ICON_BUTTON));
            m_buttonFileListFilter = new IconButton(UIIconManager.IconImageList, (int)(IconImageListID.FileList_FilterMini));
            m_buttonFileListFilter.Size = m_iconSize;
            this.ComboBox.Controls.Add(m_buttonFileListFilter);

            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ComboBoxDirectory_KeyDown);
            this.ComboBox.LostFocus += new System.EventHandler(this.ComboBoxDirectory_LostFocus);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ComboBoxDirectory_MouseClick);
            this.ComboBox.DrawItem += new DrawItemEventHandler(this.ComboBoxDirectory_DrawItem);
            this.ComboBox.DropDown += new EventHandler(this.ComboBoxDirectory_DropDown);
            this.ComboBox.SelectedIndexChanged += new EventHandler(this.ComboBoxDirectory_SelectedIndexChanged);
            m_buttonFileListFilter.Clicked += new EventHandler(ButtonFileListFilter_Click);
        }

        //=========================================================================================
        // 機　能：アドレスバーのサイズをセットする
        // 引　数：[in]cx   幅
        // 　　　　[in]cy   高さ
        // 戻り値：なし
        //=========================================================================================
        public void SetSize(int cx, int cy) {
            this.Size = new Size(cx, cy);
            m_textBoxInput.Size = new Size(cx - MainWindowForm.X(CX_MARGIN_TEXT) - MainWindowForm.X(CX_ICON_BUTTON), cy);
            cy = this.ComboBox.Size.Height;
            m_buttonFileListFilter.Location = new Point(m_textBoxInput.Location.X + cx - MainWindowForm.X(CX_MARGIN_TEXT) - MainWindowForm.X(CX_ICON_BUTTON), (cy - MainWindowForm.Y(CY_ICON_BUTTON)) / 2 + ((cy > MainWindowForm.X(CY_ICON_BUTTON + 2)) ? 1 : 0));
            m_buttonFileListFilter.Invalidate();
        }

        //=========================================================================================
        // 機　能：アドレスバーの項目を描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void ComboBoxDirectory_DrawItem(object sender, DrawItemEventArgs evt) {
            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
            doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
            HighDpiGraphics g = new HighDpiGraphics(doubleBuffer.DrawingGraphics);
            Brush brush = new SolidBrush(this.BackColor);
            g.Graphics.FillRectangle(brush, evt.Bounds);
            brush.Dispose();
            try {
                if (m_iconFolder == null) {
                    FileIconManager iconManager = Program.Document.FileIconManager;
                    FileIcon icon = iconManager.GetFileIcon(iconManager.DefaultFolderIconId, FileIconID.NullId, FileListViewIconSize.IconSize16);
                    m_iconFolder = icon.IconImage;
                }
                int xPos = evt.Bounds.Left + 1;
                int yPos = evt.Bounds.Top + (evt.Bounds.Height - m_iconSize.Height) / 2;
                if (evt.Index == -1 || (evt.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit) {
                    g.Graphics.DrawImage(m_iconFolder, xPos, yPos, m_iconSize.Width, m_iconSize.Height);
                } else {
                    AddressBarSettingRuntimeItem item = m_dropDownItemList[evt.Index];
                    const int CX_SHIFT_DEPTH = 16;
                    xPos += CX_SHIFT_DEPTH * item.Depth;
                    Bitmap icon = item.Icon;
                    if (icon == null) {
                        icon = m_iconFolder;
                    }
                    int iconSize = Math.Min(m_iconSize.Height, evt.Bounds.Height);
                    g.Graphics.DrawImage(icon, xPos, yPos, iconSize, iconSize);
                    const int CX_MARGIN = 8;
                    int xName = xPos + m_iconSize.Width + g.X(CX_MARGIN);
                    int yName = evt.Bounds.Top + (evt.Bounds.Height - this.Font.Height) / 2;
                    int cxName = (int)(GraphicsUtils.MeasureString(g.Graphics, this.Font, item.DisplayName) + 1);
                    int cyName = this.Font.Height;
                    Rectangle rcName = new Rectangle(xName, yName, cxName, cyName);
                    if ((evt.State & DrawItemState.Focus) == DrawItemState.Focus) {
                        Rectangle rcBack = FormUtils.ShrinkRectangle(rcName, 1);
                        g.Graphics.FillRectangle(SystemBrushes.MenuHighlight, rcBack);
                        ControlPaint.DrawFocusRectangle(g.Graphics, rcName);
                        g.Graphics.DrawString(item.DisplayName, this.Font, SystemBrushes.HighlightText, new Point(xName, yName));
                    } else {
                        g.Graphics.DrawString(item.DisplayName, this.Font, SystemBrushes.ControlText, new Point(xName, yName));
                    }
                }
            } finally {
                doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
            }
        }

        //=========================================================================================
        // 機　能：コンボボックスのドロップダウンが開かれたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ComboBoxDirectory_DropDown(object sender, EventArgs evt) {
            // 画像を破棄
            foreach (Bitmap bmp in m_temporaryBitmapList) {
                bmp.Dispose();
            }
            m_temporaryBitmapList.Clear();

            // 新しいリストを作成
            string current = m_fileListView.FileList.DisplayDirectoryName;
            m_dropDownItemList = Program.Document.AddressBarSetting.GetRuntimeItemList(current, m_temporaryBitmapList);
            int add = m_dropDownItemList.Count - this.Items.Count;
            if (add > 0) {
                for (int i = 0; i < add; i++) {
                    this.Items.Add("");
                }
            } else if (add < 0) {
                for (int i = add; i < 0; i++) {
                    this.Items.RemoveAt(this.Items.Count - 1);
                }
            }
        }

        //=========================================================================================
        // 機　能：コンボボックスの項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ComboBoxDirectory_SelectedIndexChanged(object sender, EventArgs evt) {
            // カーソルをこちらへ移動
            if (m_isLeft != Program.Document.CurrentTabPage.IsCursorLeft) {
                Program.MainWindow.ToggleCursorLeftRight();
            }
            m_fileListView.Focus();

            // コマンドを実行
            if (this.SelectedIndex != -1) {
                AddressBarSettingRuntimeItem item = m_dropDownItemList[this.SelectedIndex];
                Program.MainWindow.OnAddressBarCommand(item.Directory);
            }
        }

        //=========================================================================================
        // 機　能：コンボボックス上でキーが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ComboBoxDirectory_KeyDown(object sender, KeyEventArgs evt) {
            // カーソルをこちらへ移動
            if (m_isLeft != Program.Document.CurrentTabPage.IsCursorLeft) {
                Program.MainWindow.ToggleCursorLeftRight();
            }

            if (evt.KeyCode == Keys.Enter) {
                // コマンドを実行
                Program.MainWindow.OnAddressBarCommand(m_textBoxInput.Text);
                m_fileListView.Focus();
                evt.Handled = true;
                evt.SuppressKeyPress = true;
            } else if (evt.KeyCode == Keys.Cancel) {
                // 終了
                ResetComboboxString();
                m_fileListView.Focus();
                evt.Handled = true;
                evt.SuppressKeyPress = true;
            }
        }

        //=========================================================================================
        // 機　能：ファイルフィルターのボタン上でマウスがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ButtonFileListFilter_Click(object sender, EventArgs evt) {
            Program.MainWindow.OnAddressBarCommand(null);
            m_fileListView.Focus();
        }

        //=========================================================================================
        // 機　能：コンボボックス上でマウスがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ComboBoxDirectory_MouseClick(object sender, MouseEventArgs evt) {
            // カーソルをこちらへ移動
            if (m_isLeft != Program.Document.CurrentTabPage.IsCursorLeft) {
                Program.MainWindow.ToggleCursorLeftRight();
            }
        }

        //=========================================================================================
        // 機　能：ディレクトリ情報を設定する
        // 引　数：[in]directory  ディレクトリ名
        // 　　　　[in]separator  ディレクトリの区切り文字
        // 　　　　[in]filter     ファイル一覧のフィルター（フィルターを使用しないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void SetDirectoryName(string directory, string separator, FileListFilterMode filter) {
            m_directory = directory;
            m_separator = separator;
            if (filter == null) {
                m_pathMask = "*";
            } else {
                m_pathMask = filter.GetDisplayString();
            }
            ResetComboboxString();
        }

        //=========================================================================================
        // 機　能：コンボボックスの文字列をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ResetComboboxString() {
            string dispStr = null;
            if (m_directory.EndsWith(m_separator)) {
                dispStr = m_directory + m_pathMask;
            } else {
                dispStr = m_directory + m_separator + m_pathMask;
            }
            m_textBoxInput.Text = dispStr;
            Color color;
            if (m_pathMask != "*") {
                color = Color.LightYellow;
            } else {
                color = SystemColors.Window;
            }
            this.BackColor = color;
            this.m_textBoxInput.BackColor = color;

            m_buttonFileListFilter.Selected = (m_pathMask != "*");
            m_buttonFileListFilter.Invalidate();
        }

        //=========================================================================================
        // 機　能：フォーカスが失われたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void ComboBoxDirectory_LostFocus(object sender, EventArgs evt) {
            ResetComboboxString();
            m_fileListView.Focus();
            this.ComboBox.Invalidate();
            this.ComboBox.Update();
        }

        //=========================================================================================
        // プロパティ：入力領域
        //=========================================================================================
        public TextBox TextBoxInput {
            get {
                return m_textBoxInput;
            }
        }
    }
}
