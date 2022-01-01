using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Windows;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.GraphicsViewer.Filter;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileViewer;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：グラフィックビューアのフィルター設定（1段分）
    //=========================================================================================
    public class GraphicsViewerFilterItem : ICloneable {
        // フィルターのクラス
        private Type m_filterClass;
        
        // フィルターのパラメータ
        private object[] m_filterParameter;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]filterClass     フィルターのクラス
        // 　　　　[in]filterParameter フィルターのパラメータ
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerFilterItem(Type filterClass, params object[] filterParameter) {
            m_filterClass = filterClass;
            m_filterParameter = filterParameter;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerFilterItem() {
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(GraphicsViewerFilterItem obj1, GraphicsViewerFilterItem obj2) {
            if (obj1.m_filterClass.FullName != obj2.m_filterClass.FullName) {
                return false;
            }
            if (obj1.m_filterParameter.Length != obj2.m_filterParameter.Length) {
                return false;
            }
            for (int i = 0; i < obj1.m_filterParameter.Length; i++) {
                if (obj1.m_filterParameter[i] != obj2.m_filterParameter[i]) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            GraphicsViewerFilterItem clone = new GraphicsViewerFilterItem();
            clone.m_filterClass = m_filterClass;
            clone.m_filterParameter = new object[m_filterParameter.Length];
            for (int i = 0; i < m_filterParameter.Length; i++) {
                clone.m_filterParameter[i] = m_filterParameter[i];
            }

            return clone;
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, out GraphicsViewerFilterItem obj) {
            obj = null;
            bool success;
            bool fit;
            success = loader.PeekTag(SettingTag.GraphicsViewerFilter_FilterItem, SettingTagType.BeginObject, out fit);
            if (!success) {
                return false;
            }
            if (!fit) {
                return true;
            }
            loader.NextTag();
            obj = new GraphicsViewerFilterItem();
            List<object> listParam = new List<object>();
            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == SettingTag.GraphicsViewerFilter_FilterItem) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.GraphicsViewerFilter_FilterClass) {
                    string stringValue = loader.StringValue;
                    obj.m_filterClass = Type.GetType(stringValue);
                    if (obj.m_filterClass == null) {
                        obj = null;
                        return true;
                    }
                } else if (tagType == SettingTagType.BeginObject && tagName == SettingTag.GraphicsViewerFilter_FilterParam) {
                    ;
                } else if (tagType == SettingTagType.EndObject && tagName == SettingTag.GraphicsViewerFilter_FilterParam) {
                    ;
                } else if (tagType == SettingTagType.FloatValue && tagName == SettingTag.GraphicsViewerFilter_FilterParamFloat) {
                    listParam.Add(loader.FloatValue);
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.GraphicsViewerFilter_FilterParamInt) {
                    listParam.Add(loader.IntValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.GraphicsViewerFilter_FilterParamBool) {
                    listParam.Add(loader.BoolValue);
                }
            }
            obj.m_filterParameter = listParam.ToArray();

            // パラメータをチェック
            success = obj.CheckConsistency();
            if (!success) {
                obj = null;
                return true;
            }

            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, GraphicsViewerFilterItem obj) {
            if (obj == null) {
                return true;
            }

            saver.StartObject(SettingTag.GraphicsViewerFilter_FilterItem);
            saver.AddString(SettingTag.GraphicsViewerFilter_FilterClass, obj.m_filterClass.FullName);
            saver.StartObject(SettingTag.GraphicsViewerFilter_FilterParam);

            for (int i = 0; i < obj.m_filterParameter.Length; i++) {
                object value = obj.m_filterParameter[i];
                if (value is float) {
                    saver.AddFloat(SettingTag.GraphicsViewerFilter_FilterParamFloat, (float)value);
                } else if (value is int) {
                    saver.AddInt(SettingTag.GraphicsViewerFilter_FilterParamInt, (int)value);
                } else if (value is bool) {
                    saver.AddBool(SettingTag.GraphicsViewerFilter_FilterParamBool, (bool)value);
                }
            }

            saver.EndObject(SettingTag.GraphicsViewerFilter_FilterParam);
            saver.EndObject(SettingTag.GraphicsViewerFilter_FilterItem);

            return true;
        }

        //=========================================================================================
        // 機　能：設定の一貫性を確認する
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]obj     読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        private bool CheckConsistency() {
            if (m_filterClass == null || m_filterParameter == null) {
                return false;
            }
            IFilterComponent filter;
            try {
                filter = (IFilterComponent)(m_filterClass.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
            } catch (Exception) {
                return false;
            }

            FilterMetaInfo.ParameterInfo[] paramInfoList = filter.MetaInfo.ParameterList;
            if (m_filterParameter.Length != paramInfoList.Length) {
                return false;
            }
            for (int i = 0; i < paramInfoList.Length; i++) {
                if (paramInfoList[i].ParameterValueType == FilterMetaInfo.ParameterValueType.FloatPercent || 
                    paramInfoList[i].ParameterValueType == FilterMetaInfo.ParameterValueType.FloatSignedPercent ||
                    paramInfoList[i].ParameterValueType == FilterMetaInfo.ParameterValueType.FloatValue0 ||
                    paramInfoList[i].ParameterValueType == FilterMetaInfo.ParameterValueType.FloatValue2) {
                    if (!(m_filterParameter[i] is float)) {
                        return false;
                    }
                    float floatValue = (float)(m_filterParameter[i]);
                    if (floatValue < (float)(paramInfoList[i].MinValue) || (float)(paramInfoList[i].MaxValue) < floatValue) {
                        return false;
                    }
                } else {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：フィルターのクラス
        //=========================================================================================
        public  Type FilterClass {
            get {
                return m_filterClass;
            }
        }

        //=========================================================================================
        // プロパティ：フィルターのパラメータ
        //=========================================================================================
        public object[] FilterParameter {
            get {
                return m_filterParameter;
            }
        }
    }
}
