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
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：ファイルフィルターのクイック設定ダイアログ
    //=========================================================================================
    public partial class FileFilterQuickDialog : Form {
        // フィルター詳細として表示するフィルター数の最大
        private const int FILTER_DISP_MAX = 3;

        // 現在のフィルター設定
        private FileFilterClipboardSetting m_setting;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]setting   現在のフィルター設定（直接書き換えて、閉じるボタンでそのまま終了）
        // 戻り値：なし
        //=========================================================================================
        public FileFilterQuickDialog(FileFilterClipboardSetting setting) {
            InitializeComponent();
            m_setting = setting;
            
            // 現在のクイック設定一覧
            int shortage = FileFilterClipboardSetting.MAX_QUICK_COUNT - m_setting.QuickSetting.Count;
            for (int i = 0; i < shortage; i++) {
                m_setting.QuickSetting.Add(null);
            }
            string[] items = new string[FileFilterClipboardSetting.MAX_QUICK_COUNT];
            for (int i = 0; i < items.Length; i++) {
                items[i] = "";
            }
            this.listBoxSetting.Items.AddRange(items);
            this.listBoxSetting.SelectedIndex = 0;

            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：UIの有効/無効状態を切り替える
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            int index = this.listBoxSetting.SelectedIndex;
            if (index == -1) {
                // ここは通らないはず
                this.buttonWrite.Enabled = false;
                this.buttonEdit.Enabled = false;
                this.buttonDelete.Enabled = false;
                this.buttonUp.Enabled = false;
                this.buttonDown.Enabled = false;
            } else {
                this.buttonWrite.Enabled = true;
                this.buttonEdit.Enabled = (m_setting.QuickSetting[index] != null);
                this.buttonDelete.Enabled = (m_setting.QuickSetting[index] != null);
                this.buttonUp.Enabled = (index > 0);
                this.buttonDown.Enabled = (index < m_setting.QuickSetting.Count - 1);
            }
        }

        //=========================================================================================
        // 機　能：上書きボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonWrite_Click(object sender, EventArgs evt) {
            int index = this.listBoxSetting.SelectedIndex;
            if (index == -1) {
                return;
            }
            if (m_setting.CurrentSetting.FilterList.Count == 0) {
                InfoBox.Warning(this, Resources.DlgFileFilter_QuickNoCurrentFilter, index + 1);
                return;
            }
            m_setting.QuickSetting[index] = (FileFilterListClipboard)(m_setting.CurrentSetting.Clone());
            m_setting.QuickSetting[index].QuickSettingName = string.Format(Resources.DlgFileFilter_QuickDefaultDispName, index + 1);
            this.listBoxSetting.DrawMode = DrawMode.Normal;
            this.listBoxSetting.DrawMode = DrawMode.OwnerDrawVariable;
            this.listBoxSetting.Invalidate();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            int index = this.listBoxSetting.SelectedIndex;
            if (index == -1) {
                return;
            }
            DialogResult yesNo = InfoBox.Question(this, MessageBoxButtons.YesNo, Resources.DlgFileFilter_QuickDelete, index + 1, m_setting.QuickSetting[index].QuickSettingName);
            if (yesNo != DialogResult.Yes) {
                return;
            }
            m_setting.QuickSetting[index] = null;
            this.listBoxSetting.DrawMode = DrawMode.Normal;
            this.listBoxSetting.DrawMode = DrawMode.OwnerDrawVariable;
            this.listBoxSetting.Invalidate();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：名前変更ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonEdit_Click(object sender, EventArgs evt) {
            int index = this.listBoxSetting.SelectedIndex;
            if (index == -1) {
                return;
            }
            if (m_setting.QuickSetting[index] == null) {
                return;
            }
            string oldName = m_setting.QuickSetting[index].QuickSettingName;
            FileFilterQuickRenameDialog dialog = new FileFilterQuickRenameDialog(oldName);
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.OK) {
                return;
            }
            m_setting.QuickSetting[index].QuickSettingName = dialog.NewName;
            this.listBoxSetting.Invalidate();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：上へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonUp_Click(object sender, EventArgs evt) {
            int index = this.listBoxSetting.SelectedIndex;
            if (index == -1 || index == 0) {
                return;
            }

            FileFilterListClipboard temp = m_setting.QuickSetting[index - 1];
            m_setting.QuickSetting[index - 1] = m_setting.QuickSetting[index];
            m_setting.QuickSetting[index] = temp;

            this.listBoxSetting.SelectedIndex = index - 1;
            this.listBoxSetting.DrawMode = DrawMode.Normal;
            this.listBoxSetting.DrawMode = DrawMode.OwnerDrawVariable;
            this.listBoxSetting.Invalidate();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：下へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs evt) {
            int index = this.listBoxSetting.SelectedIndex;
            if (index == -1 || index == this.listBoxSetting.Items.Count - 1) {
                return;
            }

            FileFilterListClipboard temp = m_setting.QuickSetting[index + 1];
            m_setting.QuickSetting[index + 1] = m_setting.QuickSetting[index];
            m_setting.QuickSetting[index] = temp;

            this.listBoxSetting.SelectedIndex = index + 1;
            this.listBoxSetting.DrawMode = DrawMode.Normal;
            this.listBoxSetting.DrawMode = DrawMode.OwnerDrawVariable;
            this.listBoxSetting.Invalidate();
            EnableUIItem();
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
        
        //=========================================================================================
        // 機　能：一覧の項目の選択位置が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxSetting_SelectedIndexChanged(object sender, EventArgs evt) {
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：一覧の項目を描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxSetting_DrawItem(object sender, DrawItemEventArgs evt) {
            int index = evt.Index;
            if (index == -1) {
                return;
            }

            // 描画
            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
            doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
            OwnerDrawListBoxGraphics g = new OwnerDrawListBoxGraphics(doubleBuffer.DrawingGraphics, evt.Bounds.Top, evt.Bounds.Height);

            Brush backBrush;
            Pen borderPen;
            
            // 背景色を決定
            if ((evt.State & DrawItemState.Focus) == DrawItemState.Focus || (evt.State & DrawItemState.Selected) == DrawItemState.Selected) {
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
            if (index == this.listBoxSetting.Items.Count - 1) {
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
            
            // 描画対象の情報を取得
            FileFilterListClipboard quickSetting = quickSetting = m_setting.QuickSetting[index];
            string dispName;
            List<string> filterNameList = new List<string>();
            if (quickSetting == null) {
                // 設定なし
                dispName = string.Format(Resources.DlgFileFilter_QuickNone, index + 1);
            } else {
                // 設定あり
                // 表示内容を取得
                dispName = string.Format(Resources.DlgFileFilter_QuickName, index + 1, quickSetting.QuickSettingName);
                int filterCount = quickSetting.FilterList.Count;
                for (int i = 0; i < filterCount; i++) {
                    Type componentType = Type.GetType(quickSetting.FilterList[i].FileFilterClassPath);
                    IFileFilterComponent component = (IFileFilterComponent)(componentType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                    if (filterNameList.Count == FILTER_DISP_MAX - 1) {
                        string filterName = string.Format(Resources.DlgFileFilter_QuickEtc, component.FilterName, filterCount);
                        filterNameList.Add(filterName);
                        break;
                    } else {
                        filterNameList.Add(component.FilterName);
                    }
                }
            }

            // 文字情報
            int x = evt.Bounds.Left + 4;
            int y = evt.Bounds.Top + 6;
            g.Graphics.DrawString(dispName, this.listBoxSetting.Font, SystemBrushes.WindowText, new Point(x, y));
            for (int i = 0; i < filterNameList.Count; i++) {
                g.Graphics.DrawString(filterNameList[i], this.listBoxSetting.Font, SystemBrushes.WindowText, new Point(x + 8, y + i * 12 + 16));
            }

            doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
        }

        //=========================================================================================
        // 機　能：一覧の項目の大きさを返す
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxSetting_MeasureItem(object sender, MeasureItemEventArgs evt) {
            int index = evt.Index;
            if (index >= m_setting.QuickSetting.Count) {
                evt.ItemHeight = 24;
            } else {
                int height = 24;
                if (m_setting.QuickSetting[index] != null) {
                    height += Math.Min(FILTER_DISP_MAX, m_setting.QuickSetting[index].FilterList.Count) * 12 + 2;
                }
                evt.ItemHeight = height;
            }
        }
    }
}
