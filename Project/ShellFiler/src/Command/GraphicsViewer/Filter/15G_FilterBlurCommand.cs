using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Command.FileList;
using ShellFiler.GraphicsViewer;
using ShellFiler.GraphicsViewer.Filter;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.GraphicsViewer;

namespace ShellFiler.Command.GraphicsViewer.Filter {

    //=========================================================================================
    // クラス：コマンドを実行する
    // ぼかしフィルターをレベル{0}で適用します。
    //   書式 　 G_FilterBlur(int level)
    //   引数  　level:適用レベルの差分(-100%～100%)
    // 　　　　　level-default:0
    // 　　　　　level-range:-100,100
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_FilterBlurCommand : GraphicsViewerActionCommand {
        // 適用レベルの差分
        private float m_levelDelta;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_FilterBlurCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            int iLevelDelta = (int)param[0];
            m_levelDelta = (float)iLevelDelta / 100.0f;
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            object[] deltaList = new object[1];
            deltaList[0] = m_levelDelta;
            ModifyFilterParam(GraphicsViewerPanel, typeof(ComponentBlur), deltaList);
            return null;
        }

        //=========================================================================================
        // 機　能：フィルターのパラメータを変更する
        // 引　数：[in]panel       フィルターのパネル
        // 　　　　[in]filter      フィルターのタイプ情報
        // 　　　　[in]parmaDelta  パラメータの差分
        // 戻り値：なし
        //=========================================================================================
        public static void ModifyFilterParam(GraphicsViewerPanel panel, Type filter, object[] paramDelta) {
            GraphicsViewerFilterSetting setting = panel.FilterSetting;
            List<GraphicsViewerFilterItem> newFilterList = new List<GraphicsViewerFilterItem>();
            GraphicsViewerFilterItem lastFilter = null;
            for (int i = 0; i < setting.FilterList.Count; i++) {
                GraphicsViewerFilterItem filterItem = setting.FilterList[i];
                if (filterItem.FilterClass.FullName == filter.FullName) {
                    // 対象フィルター
                    IFilterComponent filterComponent = (IFilterComponent)(filter.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                    FilterMetaInfo.ParameterInfo[] parameterInfo = filterComponent.MetaInfo.ParameterList;
                    DeltaFilterParam(filterItem.FilterParameter, paramDelta, parameterInfo);
                    lastFilter = filterItem;
                } else {
                    // その他のフィルター
                    newFilterList.Add(filterItem);
                }
            }

            // 最終位置に指定フィルターを設定
            if (lastFilter == null) {
                IFilterComponent filterComponent = (IFilterComponent)(filter.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                FilterMetaInfo.ParameterInfo[] parameterInfo = filterComponent.MetaInfo.ParameterList;
                object[] paramList = new object[parameterInfo.Length];
                for (int i = 0; i < paramList.Length; i++) {
                    paramList[i] = parameterInfo[i].DefaultValue;
                }
                DeltaFilterParam(paramList, paramDelta, parameterInfo);
                lastFilter = new GraphicsViewerFilterItem(filter, paramList);
            }
            newFilterList.Add(lastFilter);

            // コンフィグに設定
            setting.FilterList.Clear();
            setting.UseFilter = true;
            setting.FilterList.AddRange(newFilterList);

            // 再描画
            panel.ResetCurrentImageUI(true);
        }

        //=========================================================================================
        // 機　能：パラメータの差分を反映する
        // 引　数：[in]paramList   パラメータリスト
        // 　　　　[in]paramDelta  パラメータの差分
        // 戻り値：なし
        //=========================================================================================
        private static void DeltaFilterParam(object[] paramList, object[] paramDelta, FilterMetaInfo.ParameterInfo[] parameterInfo) {
            for (int i = 0; i < paramList.Length; i++) {
                if (paramList[i] is float) {
                    float maxValue = (float)(parameterInfo[i].MaxValue);
                    float minValue = (float)(parameterInfo[i].MinValue);
                    float value = (float)(paramList[i]) + (float)(paramDelta[i]);
                    paramList[i] = (float)Math.Max(minValue, Math.Min(maxValue, value));
                } else if (paramList[i] is int) {
                    float maxValue = (int)(parameterInfo[i].MaxValue);
                    float minValue = (int)(parameterInfo[i].MinValue);
                    float value = (int)(paramList[i]) + (int)(paramDelta[i]);
                    paramList[i] = (int)Math.Max(minValue, Math.Min(maxValue, value));
                } else if (paramList[i] is bool) {
                    paramList[i] = (bool)(paramDelta[i]);
                }
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_FilterBlurCommand;
            }
        }
    }
}
