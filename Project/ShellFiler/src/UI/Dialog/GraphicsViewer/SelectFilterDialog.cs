using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.GraphicsViewer;
using ShellFiler.GraphicsViewer.Filter;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.GraphicsViewer {

    //=========================================================================================
    // クラス：グラフィックビューアのフィルター設定ダイアログ
    //=========================================================================================
    public partial class SelectFilterDialog : Form {
        // 設定対象のパネル
        private GraphicsViewerPanel m_viewerPanel;

        // 編集対象のフィルター
        private GraphicsViewerFilterSetting m_filterSetting;

        // グラフィックビューアのフィルター設定（元の状態）
        private GraphicsViewerFilterSetting m_settingOrg;

        // フィルター一覧のUI用内部形式
        private SelectFilterDialogItemList m_dialogItemList;

        // プロパティパネル
        private FilterPropertyPanel m_propertyPanel;

        // プロパティパネル（フィルターなしのダミー）
        private FilterPropertyPanelDummy m_propertyPanelDummy;

        // 画像の再更新が必要なときtrue
        private bool m_dirtyImage = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]viewerPanel  編集対象となるファイルビューアのパネル
        // 戻り値：なし
        //=========================================================================================
        public SelectFilterDialog(GraphicsViewerPanel viewerPanel) {
            InitializeComponent();
            m_viewerPanel = viewerPanel;
            m_filterSetting = viewerPanel.FilterSetting;

            // 元の設定を保存
            // ダイアログ終了時、新しい設定に書き換えて戻る
            m_settingOrg = (GraphicsViewerFilterSetting)viewerPanel.FilterSetting.Clone();

            // フィルター一覧を内部形式に変換(UseFilterは無視、Cloneに相当)
            m_dialogItemList = new SelectFilterDialogItemList(m_settingOrg);

            // 現状のフィルターを初期化
            m_viewerPanel.ResetCurrentImageUI(true);

            // UIを初期化
            foreach (SelectFilterDialogItem uiItem in m_dialogItemList.FilterItemListOn) {
                this.listBoxUseFilter.Items.Add(uiItem);
            }
            foreach (SelectFilterDialogItem uiItem in m_dialogItemList.FilterItemListOff) {
                this.listBoxNotUse.Items.Add(uiItem);
            }

            // プロパティパネルを初期化
            m_propertyPanel = new FilterPropertyPanel(this);
            m_propertyPanel.Location = new Point(0, 0);
            m_propertyPanelDummy = new FilterPropertyPanelDummy();
            m_propertyPanel.Location = new Point(0, 0);
            this.panelProperty.Controls.Add(m_propertyPanel);
            this.panelProperty.Controls.Add(m_propertyPanelDummy);
            Application.Idle += new EventHandler(Application_Idle);

            // UIに反映
            if (this.listBoxUseFilter.Items.Count > 0) {
                this.listBoxUseFilter.SelectedIndex = 0;
            }
            if (this.listBoxNotUse.Items.Count > 0) {
                this.listBoxNotUse.SelectedIndex = 0;
            }
            SetFilter();
            if (this.listBoxUseFilter.Items.Count > 0) {
                GetSettingFromUI(true);
                m_viewerPanel.ResetCurrentImageUI(true);
            }

            // 表示位置を調整
            int xPos = viewerPanel.GraphicsViewerForm.Right - this.Width;
            int yPos = viewerPanel.GraphicsViewerForm.Bottom - this.Height;
            this.Location = new Point(xPos, yPos);
            EnableUIItem();
        }
        
        //=========================================================================================
        // 機　能：フォームが閉じられたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SelectFilterDialog_FormClosed(object sender, FormClosedEventArgs evt) {
            m_propertyPanel.DisposePanel();
            Application.Idle -= new EventHandler(Application_Idle);
            m_viewerPanel.GraphicsViewerForm.OnCloseFilterSettingDialog();
        }

        //=========================================================================================
        // 機　能：UIの有効/無効の状態を更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void EnableUIItem() {
            this.buttonAdd.Enabled = (m_dialogItemList.FilterItemListOff.Count > 0);
            this.buttonDelete.Enabled = (m_dialogItemList.FilterItemListOn.Count > 0);
            this.buttonUp.Enabled = (this.listBoxUseFilter.SelectedIndex > 0);
            this.buttonDown.Enabled = (this.listBoxUseFilter.SelectedIndex != -1 && this.listBoxUseFilter.SelectedIndex < this.listBoxUseFilter.Items.Count - 1);
        }

        //=========================================================================================
        // 機　能：UIの状態に基づいて最終状態のコンフィグ設定を取得する
        // 引　数：[in]useFilter   フィルターを使用するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void GetSettingFromUI(bool useFilter) {
            if (m_dialogItemList.FilterItemListOn.Count == 0) {
                useFilter = false;
            }
            m_filterSetting.ResetFilter(useFilter);
            for (int i = 0; i < m_dialogItemList.FilterItemListOn.Count; i++) {
                SelectFilterDialogItem uiItem = m_dialogItemList.FilterItemListOn[i];
                GraphicsViewerFilterItem item = new GraphicsViewerFilterItem(uiItem.Filter.GetType(), uiItem.FilterParameter);
                m_filterSetting.FilterList.Add(item);
            }
            SetUserSettingFilter();
        }

        //=========================================================================================
        // 機　能：現在のダイアログの設定に基づいてユーザー設定のフィルターを設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void SetUserSettingFilter() {
            if (Configuration.Current.GraphicsViewerFilterMode == GraphicsViewerFilterMode.AllImages) {
                Program.Document.UserGeneralSetting.GraphicsViewerImageFilter = (GraphicsViewerFilterSetting)m_filterSetting.Clone();
            } else {
                Program.Document.UserGeneralSetting.GraphicsViewerImageFilter = new GraphicsViewerFilterSetting();
            }
        }

        //=========================================================================================
        // 機　能：新しいフィルターの設定に基づいてUIを初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetFilter() {
            int index = this.listBoxUseFilter.SelectedIndex;
            if (index == -1) {
                m_propertyPanel.Visible = false;
                m_propertyPanelDummy.Visible = true;
            } else {
                SelectFilterDialogItem uiItem = m_dialogItemList.FilterItemListOn[index];
                m_propertyPanel.Visible = true;
                m_propertyPanelDummy.Visible = false;
                m_propertyPanel.SetFilter(uiItem);
            }
        }

        //=========================================================================================
        // 機　能：フィルターのプロパティの値が変更されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnFilterValueChanged() {
            this.listBoxUseFilter.Invalidate();
            m_dirtyImage = true;
        }

        //=========================================================================================
        // 機　能：アプリケーションがアイドル状態になったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void Application_Idle(object sender, EventArgs evt) {
            if (!m_dirtyImage) {
                return;
            }
            m_dirtyImage = false;

            GetSettingFromUI(true);
            m_viewerPanel.ResetCurrentImageUI(true);
        }
        
        //=========================================================================================
        // 機　能：使用するフィルター一覧の項目を描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxUseFilter_DrawItem(object sender, DrawItemEventArgs evt) {
            int index = evt.Index;
            if (index == -1) {
                return;
            }
            SelectFilterDialogItem item = m_dialogItemList.FilterItemListOn[index];
            FilterMetaInfo metaInfo = item.FilterMetaInfo;

            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
            doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
            OwnerDrawListBoxGraphics g = new OwnerDrawListBoxGraphics(doubleBuffer.DrawingGraphics, evt.Bounds.Top, evt.Bounds.Height);

            Brush backBrush;
            Pen borderPen;
            
            // 背景色を決定
            if ((evt.State & DrawItemState.Selected) == DrawItemState.Selected) {
                // 選択中、このフィルターを使用しない
                backBrush = g.MarkBackBrush;
                borderPen = g.BorderPen;
            } else {
                // 選択中でない、このフィルターを使用しない
                backBrush = SystemBrushes.Window;
                borderPen = g.BorderPen;
            }

            // 描画
            int cx = evt.Bounds.Width;
            int cy = evt.Bounds.Height;
            bool drawBottom = false;
            if (index == m_dialogItemList.FilterItemListOn.Count - 1) {
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
            int x = evt.Bounds.Left + 2;
            int y = evt.Bounds.Top + 2;
            g.Graphics.DrawString(metaInfo.DisplayName, this.listBoxNotUse.Font, SystemBrushes.WindowText, new Point(x, y));
            for (int i = 0; i < metaInfo.ParameterList.Length; i++) {
                FilterMetaInfo.ParameterInfo paramInfo = metaInfo.ParameterList[i];
                string paramLabel = paramInfo.DisplayName;
                string paramValue = GetParameterValueDisplay(item.FilterParameter[i], paramInfo.ParameterValueType);
                g.Graphics.DrawString(paramLabel, this.listBoxNotUse.Font, SystemBrushes.WindowText, new Point(g.X(x + 8), g.Y(y + i * 12 + 16)));
                g.Graphics.DrawString(paramValue, this.listBoxNotUse.Font, SystemBrushes.WindowText, new Point(g.X(x + 120), g.Y(y + i * 12 + 16)));
            }

            // 矢印
            if (index < this.listBoxUseFilter.Items.Count - 1) {            // 矢印(上半分)
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
        // 機　能：使用するフィルター一覧の項目の大きさを取得する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxUseFilter_MeasureItem(object sender, MeasureItemEventArgs evt) {
            int index = evt.Index;
            if (index == -1) {
                return;
            }
            using (HighDpiGraphics g = new HighDpiGraphics(this)) {
                SelectFilterDialogItem item = m_dialogItemList.FilterItemListOn[evt.Index];
                FilterMetaInfo metaInfo = item.FilterMetaInfo;
                if (metaInfo.ParameterList.Length > 0) {
                    evt.ItemHeight = g.Y(24 + metaInfo.ParameterList.Length * 12);
                } else {
                    evt.ItemHeight = g.Y(18);
                }
            }
        }

        //=========================================================================================
        // 機　能：使用しないフィルター一覧の項目を描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxNotUse_DrawItem(object sender, DrawItemEventArgs evt) {
            int index = evt.Index;
            SelectFilterDialogItem item = m_dialogItemList.FilterItemListOff[index];
            FilterMetaInfo metaInfo = item.FilterMetaInfo;

            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, evt.Bounds.Width, evt.Bounds.Height);
            doubleBuffer.SetDrawOrigin(-evt.Bounds.Left, -evt.Bounds.Top);
            OwnerDrawListBoxGraphics g = new OwnerDrawListBoxGraphics(doubleBuffer.DrawingGraphics, evt.Bounds.Top, evt.Bounds.Height);

            Brush backBrush;
            Pen borderPen;
            
            // 背景色を決定
            if ((evt.State & DrawItemState.Selected) == DrawItemState.Selected) {
                // 選択中、このフィルターを使用しない
                backBrush = g.MarkGrayBackBrush;
                borderPen = g.GrayBorderPen;
            } else {
                // 選択中でない、このフィルターを使用しない
                backBrush = g.GrayBackBrush;
                borderPen = g.GrayBorderPen;
            }

            // 描画
            int cy = evt.Bounds.Height;
            if (index == m_dialogItemList.FilterItemListOff.Count - 1) {
                cy--;
            }
            Rectangle rect = new Rectangle(evt.Bounds.Left, evt.Bounds.Top, evt.Bounds.Width - 1, cy);
            g.Graphics.FillRectangle(backBrush, rect);
            g.Graphics.DrawRectangle(borderPen, rect);
            int x = evt.Bounds.Left + 2;
            int y = evt.Bounds.Top + 2;
            g.Graphics.DrawString(metaInfo.DisplayName, this.listBoxNotUse.Font, SystemBrushes.WindowText, new Point(x, y));

            doubleBuffer.FlushScreen(evt.Bounds.Left, evt.Bounds.Top);
        }

        //=========================================================================================
        // 機　能：使用しないフィルター一覧の項目の大きさを取得する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxNotUse_MeasureItem(object sender, MeasureItemEventArgs evt) {
            using (Graphics graphics = CreateGraphics())
            using (HighDpiGraphics g = new HighDpiGraphics(graphics)) {
                evt.ItemHeight = g.Y(16);
            }
        }

        //=========================================================================================
        // 機　能：フィルターに与えるパラメータ値をUIの表示用の文字列に変換する
        // 引　数：[in]valuue     フィルターに与えるパラメータ値
        // 　　　　[in]paramType  パラメータの設定
        // 戻り値：なし
        //=========================================================================================
        private string GetParameterValueDisplay(object value, FilterMetaInfo.ParameterValueType paramType) {
            float floatValue = (float)value;
            string dispValue = "";
            switch (paramType) {
                case FilterMetaInfo.ParameterValueType.FloatPercent:              // float型 0～1で0%～100%
                    dispValue = string.Format("{0:f0}%", floatValue * 100.0f);
                    break;
                case FilterMetaInfo.ParameterValueType.FloatSignedPercent:        // float型 -1～1で-100%～100%
                    dispValue = string.Format("{0:f0}%", floatValue * 100.0f);
                    break;
                case FilterMetaInfo.ParameterValueType.FloatValue0:               // float型 小数点以下切り捨ての数値
                    dispValue = string.Format("{0:f0}", floatValue);
                    break;
                case FilterMetaInfo.ParameterValueType.FloatValue2:               // float型 少数点以下2桁の数値
                    dispValue = string.Format("{0:f2}", floatValue);
                    break;
            }
            return dispValue;
        }

        //=========================================================================================
        // 機　能：追加ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonAdd_Click(object sender, EventArgs evt) {
            int index = this.listBoxNotUse.SelectedIndex;
            if (index == -1) {
                return;
            }
            SelectFilterDialogItem item = m_dialogItemList.FilterItemListOff[index];
 
            // データ構造で差し替え
            item.UseFilter = true;
            m_dialogItemList.SwapFilterItemOnOff(item);

            // 一覧で差し替え
            this.listBoxNotUse.Items.Remove(item);
            this.listBoxNotUse.Invalidate();
            this.listBoxUseFilter.Items.Add(item);
            this.listBoxUseFilter.SelectedIndex = this.listBoxUseFilter.Items.Count - 1;

            // UIを更新
            SetFilter();
            GetSettingFromUI(true);
            m_viewerPanel.ResetCurrentImageUI(true);
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：削除ボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDelete_Click(object sender, EventArgs evt) {
            int index = this.listBoxUseFilter.SelectedIndex;
            if (index == -1) {
                return;
            }
            SelectFilterDialogItem item = m_dialogItemList.FilterItemListOn[index];
 
            // データ構造で差し替え
            item.UseFilter = false;
            m_dialogItemList.SwapFilterItemOnOff(item);

            // 一覧で差し替え
            this.listBoxUseFilter.Items.Remove(item);
            this.listBoxUseFilter.SelectedIndex = Math.Min(index, this.listBoxUseFilter.Items.Count - 1);
            this.listBoxUseFilter.Invalidate();
            this.listBoxNotUse.Items.Add(item);
            this.listBoxNotUse.SelectedIndex = this.listBoxNotUse.Items.Count - 1;

            // UIを更新
            SetFilter();
            GetSettingFromUI(true);
            m_viewerPanel.ResetCurrentImageUI(true);
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
            int index = this.listBoxUseFilter.SelectedIndex;
            if (index <= 0) {
                return;
            }
            SelectFilterDialogItem targetItem = m_dialogItemList.FilterItemListOn[index];
            m_dialogItemList.FilterItemListOn.RemoveAt(index);
            m_dialogItemList.FilterItemListOn.Insert(index - 1, targetItem);

            // 設定をUIに反映
            this.listBoxUseFilter.DrawMode = DrawMode.Normal;
            this.listBoxUseFilter.DrawMode = DrawMode.OwnerDrawVariable;
            this.listBoxUseFilter.Invalidate();
            this.listBoxUseFilter.SelectedIndex = index - 1;
            GetSettingFromUI(true);
            m_viewerPanel.ResetCurrentImageUI(true);
        }

        //=========================================================================================
        // 機　能：下へボタンがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonDown_Click(object sender, EventArgs e) {
            // 設定を入れ替え
            int index = this.listBoxUseFilter.SelectedIndex;
            if (index == this.listBoxUseFilter.Items.Count - 1) {
                return;
            }
            SelectFilterDialogItem targetItem = m_dialogItemList.FilterItemListOn[index];
            m_dialogItemList.FilterItemListOn.RemoveAt(index);
            m_dialogItemList.FilterItemListOn.Insert(index + 1, targetItem);

            // 設定をUIに反映
            this.listBoxUseFilter.DrawMode = DrawMode.Normal;
            this.listBoxUseFilter.DrawMode = DrawMode.OwnerDrawVariable;
            this.listBoxUseFilter.Invalidate();
            this.listBoxUseFilter.SelectedIndex = index + 1;
            GetSettingFromUI(true);
            m_viewerPanel.ResetCurrentImageUI(true);
        }

        //=========================================================================================
        // 機　能：使用するフィルター一覧の項目が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listBoxUseFilter_SelectedIndexChanged(object sender, EventArgs evt) {
            SetFilter();
            EnableUIItem();
        }

        //=========================================================================================
        // 機　能：設定ONのボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOn_Click(object sender, EventArgs evt) {
            GetSettingFromUI(true);
            m_viewerPanel.ResetCurrentImageUI(true);
            Close();
        }

        //=========================================================================================
        // 機　能：設定OFFのボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonOff_Click(object sender, EventArgs evt) {
            GetSettingFromUI(false);
            m_viewerPanel.ResetCurrentImageUI(true);
            Close();
        }

        //=========================================================================================
        // 機　能：設定キャンセルのボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void buttonCancel_Click(object sender, EventArgs evt) {
            m_filterSetting.CopyFrom(m_settingOrg);
            SetUserSettingFilter();
            m_viewerPanel.ResetCurrentImageUI(true);
            Close();
        }
    }
}
