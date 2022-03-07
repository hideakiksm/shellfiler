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
    // クラス：フィルターのプロパティ設定パネル
    //=========================================================================================
    public partial class FilterPropertyPanel : UserControl {
        // 親ダイアログ
        private SelectFilterDialog m_parentDialog;

        // フィルター適用済みのイメージ
        private Bitmap m_filteredImage;

        // 現在のフィルター
        private SelectFilterDialogItem m_uiItem;

        // プログラムからパラメータを書き換えているときtrue
        private bool m_trackbarValueChanging = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parentDialog  親ダイアログ
        // 戻り値：なし
        //=========================================================================================
        public FilterPropertyPanel(SelectFilterDialog parentDialog) {
            InitializeComponent();
            m_parentDialog = parentDialog;

            // 適用前イメージ
            Bitmap bmp = UIIconManager.GraphicsViewer_FilterSample;
            this.pictureBoxSrc.Image = bmp;
        }

        //=========================================================================================
        // 機　能：パネルの後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisposePanel() {
            if (m_filteredImage != null) {
                m_filteredImage.Dispose();
                m_filteredImage = null;
            }
        }

        //=========================================================================================
        // 機　能：新しいフィルターの設定に基づいてUIを初期化する
        // 引　数：[in]uiItem  新しく設定するUIの設定
        // 戻り値：なし
        //=========================================================================================
        public void SetFilter(SelectFilterDialogItem uiItem) {
            m_uiItem = uiItem;
            UpdateFilteredImage();

            // タイトルを設定
            string title = m_uiItem.FilterMetaInfo.DisplayName;
            if (!uiItem.UseFilter) {
                title += Resources.GVFilter_Disabled;
            }
            this.labelFilterName.Text = title;

            this.pictureBoxSrc.Enabled = uiItem.UseFilter;

            // UIを初期化
            Label[] labelList = new Label[] {
                this.labelParam1, this.labelParam2, this.labelParam3,
            };
            TrackBar[] trackbarList = new TrackBar[] {
                this.trackBarParam1, this.trackBarParam2, this.trackBarParam3,
            };
            Label[] labelLevelLList = new Label[] {
                this.labelLevelLeft1, this.labelLevelLeft2, this.labelLevelLeft3,
            };
            Label[] labelLevelRList = new Label[] {
                this.labelLevelRight1, this.labelLevelRight2, this.labelLevelRight3,
            };
            m_trackbarValueChanging = true;
            for (int i = 0; i < labelList.Length; i++) {
                if (i < uiItem.FilterMetaInfo.ParameterList.Length) {
                    FilterMetaInfo.ParameterInfo parameterInfo = uiItem.FilterMetaInfo.ParameterList[i];
                    labelList[i].Text = parameterInfo.DisplayName + "(&" + parameterInfo.ParameterShortcut + ")";
                    int minValue, maxValue, currentValue;
                    ParameterSettingToTrackbar(parameterInfo, uiItem.FilterParameter[i], out minValue, out maxValue, out currentValue);
                    trackbarList[i].Minimum = minValue;
                    trackbarList[i].Maximum = maxValue;
                    trackbarList[i].Value = currentValue;
                    trackbarList[i].Tag = i;
                    labelLevelLList[i].Text = parameterInfo.LowLevelDisplayName;
                    labelLevelRList[i].Text = parameterInfo.HighLevelDisplayName;
                    labelList[i].Visible = true;
                    trackbarList[i].Visible = true;
                    labelLevelLList[i].Visible = true;
                    labelLevelRList[i].Visible = true;
                    if (uiItem.UseFilter) {
                        trackbarList[i].Enabled = true;
                    } else {
                        trackbarList[i].Enabled = false;
                    }
                } else {
                    labelList[i].Visible = false;
                    trackbarList[i].Visible = false;
                    labelLevelLList[i].Visible = false;
                    labelLevelRList[i].Visible = false;
                }
            }
            if (uiItem.FilterMetaInfo.ParameterList.Length == 0 || !uiItem.UseFilter) {
                this.buttonReset.Enabled = false;
            } else {
                this.buttonReset.Enabled = true;
            }
            m_trackbarValueChanging = false;
        }

        //=========================================================================================
        // 機　能：フィルターのサンプルイメージを更新する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateFilteredImage() {
            // フィルターを用意
            IFilterComponent component = m_uiItem.Filter;
            component.SetParameter(m_uiItem.FilterParameter);
            IFilterComponent[] componentList = new IFilterComponent[1];
            componentList[0] = component;

            // 適用後イメージ
            Bitmap bmp = UIIconManager.GraphicsViewer_FilterSample;
            Bitmap filtered = new Bitmap(bmp);
            ImageFilter imageFilter = new ImageFilter();
            imageFilter.SetFilter(componentList);
            imageFilter.ApplyFilter(filtered);
            this.pictureBoxDest.Image = filtered;

            if (m_filteredImage != null) {
                m_filteredImage.Dispose();
            }
            m_filteredImage = filtered;
        }

        //=========================================================================================
        // 機　能：パラメータ設定をトラックバーのパラメータに変換する
        // 引　数：[in]parameterInfo    パラメータ情報
        // 　　　　[in]parameterValue   パラメータの値
        // 　　　　[out]minValue        トラックバーの最小値を返す変数
        // 　　　　[out]maxValue        トラックバーの最大値を返す変数
        // 　　　　[out]currentValue    トラックバーの値を返す変数
        // 戻り値：なし
        //=========================================================================================
        private void ParameterSettingToTrackbar(FilterMetaInfo.ParameterInfo parameterInfo, object parameterValue, out int minValue, out int maxValue, out int currentValue) {
            minValue = 0;
            maxValue = 0;
            currentValue = 0;
            float floatMin = (float)(parameterInfo.MinValue);
            float floatMax = (float)(parameterInfo.MaxValue);
            float floatValue = (float)parameterValue;
            switch (parameterInfo.ParameterValueType) {
                case FilterMetaInfo.ParameterValueType.FloatPercent:                // float型 0～1で0%～100%
                    minValue = 0;
                    maxValue = 100;
                    currentValue = (int)(floatValue * 100.0f);
                    break;
                case FilterMetaInfo.ParameterValueType.FloatSignedPercent:          // float型 -1～1で-100%～100%
                    minValue = 0;
                    maxValue = 200;
                    currentValue = (int)((floatValue + 1.0f) * 100.0);
                    break;
                case FilterMetaInfo.ParameterValueType.FloatValue0:                 // float型 小数点以下切り捨ての数値
                    minValue = 0;
                    maxValue = (int)(floatMax - floatMin);
                    currentValue = (int)(floatValue - floatMin);
                    break;
                case FilterMetaInfo.ParameterValueType.FloatValue2:                 // float型 少数点以下2桁の数値
                    minValue = 0;
                    maxValue = (int)((floatMax - floatMin) * 100);
                    currentValue = (int)(floatValue - floatMin) * 100;
                    break;
            }
            currentValue = Math.Max(minValue, Math.Min(maxValue, currentValue));
        }

        //=========================================================================================
        // 機　能：トラックバーのパラメータをパラメータ設定に変換する
        // 引　数：[in]value          トラックバーの値
        // 　　　　[in]parameterInfo  パラメータ設定を返す変数
        // 戻り値：なし
        //=========================================================================================
        private object TrackbarToParameterSetting(int value,  FilterMetaInfo.ParameterInfo parameterInfo) {
            float result = 0.0f;
            float floatMin = (float)(parameterInfo.MinValue);
            float floatMax = (float)(parameterInfo.MaxValue);
            switch (parameterInfo.ParameterValueType) {
                case FilterMetaInfo.ParameterValueType.FloatPercent:                // float型 0～1で0%～100%
                    result = ((float)value) / 100.0f;
                    break;
                case FilterMetaInfo.ParameterValueType.FloatSignedPercent:          // float型 -1～1で-100%～100%
                    result = ((float)value) / 100.0f - 1.0f;
                    break;
                case FilterMetaInfo.ParameterValueType.FloatValue0:                 // float型 小数点以下切り捨ての数値
                    result = ((float)value + floatMin);
                    break;
                case FilterMetaInfo.ParameterValueType.FloatValue2:                 // float型 少数点以下2桁の数値
                    result = (float)value / 100.0f + floatMin;
                    break;
            }
            return result;
        }

        //=========================================================================================
        // 機　能：設定のリセットボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void RuttonReset_Click(object sender, EventArgs evt) {
            // UIを初期化
            m_trackbarValueChanging = true;
            TrackBar[] trackbarList = new TrackBar[] {
                this.trackBarParam1, this.trackBarParam2, this.trackBarParam3,
            };
            for (int i = 0; i < m_uiItem.FilterMetaInfo.ParameterList.Length; i++) {
                FilterMetaInfo.ParameterInfo parameterInfo = m_uiItem.FilterMetaInfo.ParameterList[i];
                m_uiItem.FilterParameter[i] = parameterInfo.DefaultValue;
                int minValue, maxValue, currentValue;
                ParameterSettingToTrackbar(parameterInfo, m_uiItem.FilterParameter[i], out minValue, out maxValue, out currentValue);
                trackbarList[i].Value = currentValue;
            }
            UpdateFilteredImage();
            m_parentDialog.OnFilterValueChanged();
            m_trackbarValueChanging = false;
        }

        //=========================================================================================
        // 機　能：トラックバーの値が変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void TrackBarParam_ValueChanged(object sender, EventArgs evt) {
            if (m_trackbarValueChanging) {
                return;
            }
            TrackBar trackbar = (TrackBar)sender;
            int paramIndex = (int)(trackbar.Tag);
            FilterMetaInfo.ParameterInfo parameterInfo = m_uiItem.FilterMetaInfo.ParameterList[paramIndex];
            object paramValue = TrackbarToParameterSetting(trackbar.Value, parameterInfo);
            m_uiItem.FilterParameter[paramIndex] = paramValue;
            UpdateFilteredImage();
            m_parentDialog.OnFilterValueChanged();
        }
    }
}
