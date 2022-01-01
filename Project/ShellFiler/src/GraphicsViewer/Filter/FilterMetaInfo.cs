using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.GraphicsViewer;
using ShellFiler.Command.GraphicsViewer.View;
using ShellFiler.Util;
using ShellFiler.FileSystem;
using ShellFiler.UI;
using ShellFiler.UI.ControlBar;
using ShellFiler.Locale;

namespace ShellFiler.GraphicsViewer.Filter {

    //=========================================================================================
    // クラス：フィルタコンポーネントのメタ情報
    //=========================================================================================
    public class FilterMetaInfo {
        public static readonly FilterMetaInfo BlurFilter        = new FilterMetaInfo(Resources.GVFilter_FilterNameBlur,      new ParameterInfo[] { 
                                                                    new ParameterInfo(Resources.GVFilter_ParamNameBlur,     Resources.GVFilter_ParamLevelLow,       Resources.GVFilter_ParamLevelHigh,       'L', ParameterValueType.FloatPercent, 0.0f, 1.0f, 0.0f) });
        public static readonly FilterMetaInfo BrightFilter      = new FilterMetaInfo(Resources.GVFilter_FilterNameBright,    new ParameterInfo[] {
                                                                    new ParameterInfo(Resources.GVFilter_ParamNameBright,   Resources.GVFilter_ParamLevelBrightLow, Resources.GVFilter_ParamLevelBrightHigh, 'I', ParameterValueType.FloatSignedPercent, -1.0f, 1.0f, 0.0f),
                                                                    new ParameterInfo(Resources.GVFilter_ParamNameContrast, Resources.GVFilter_ParamLevelLow,       Resources.GVFilter_ParamLevelHigh,       'C', ParameterValueType.FloatSignedPercent, -1.0f, 1.0f, 0.0f),
                                                                    new ParameterInfo(Resources.GVFilter_ParamNameGamma,    Resources.GVFilter_ParamLevelGamma0,    Resources.GVFilter_ParamLevelGamma2,     'G', ParameterValueType.FloatValue2, 0.0f, 2.0f, 1.0f) } );
        public static readonly FilterMetaInfo HsvModifyFilter   = new FilterMetaInfo(Resources.GVFilter_FilterNameHsvModify, new ParameterInfo[] {
                                                                    new ParameterInfo(Resources.GVFilter_ParamNameHsvH,     Resources.GVFilter_ParamLevelPlus,      Resources.GVFilter_ParamLevelMinus,      'H', ParameterValueType.FloatValue0, -180.0f, 180.0f, 0.0f),
                                                                    new ParameterInfo(Resources.GVFilter_ParamNameHsvS,     Resources.GVFilter_ParamLevelThin,      Resources.GVFilter_ParamLevelDark,       'S', ParameterValueType.FloatSignedPercent, -1.0f, 1.0f, 0.0f),
                                                                    new ParameterInfo(Resources.GVFilter_ParamNameHsvV,     Resources.GVFilter_ParamLevelBrightLow, Resources.GVFilter_ParamLevelBrightHigh, 'V', ParameterValueType.FloatSignedPercent, -1.0f, 1.0f, 0.0f) });
        public static readonly FilterMetaInfo MonochromeFilter  = new FilterMetaInfo(Resources.GVFilter_FilterNameMonochrome,new ParameterInfo[0]);
        public static readonly FilterMetaInfo NegativeFilter    = new FilterMetaInfo(Resources.GVFilter_FilterNameNegative,  new ParameterInfo[0]);
        public static readonly FilterMetaInfo ReliefFilter        = new FilterMetaInfo(Resources.GVFilter_FilterNameReleif,  new ParameterInfo[] { 
                                                                    new ParameterInfo(Resources.GVFilter_ParamNameReleif,   Resources.GVFilter_ParamLevelShallow,   Resources.GVFilter_ParamLevelDeep,       'L', ParameterValueType.FloatPercent, 0.0f, 1.0f, 0.0f) });
        public static readonly FilterMetaInfo SepiaFilter       = new FilterMetaInfo(Resources.GVFilter_FilterNameSepia,     new ParameterInfo[0]);
        public static readonly FilterMetaInfo SharpFilter       = new FilterMetaInfo(Resources.GVFilter_FilterNameSharp,     new ParameterInfo[] {
                                                                    new ParameterInfo(Resources.GVFilter_ParamNameSharp,    Resources.GVFilter_ParamLevelLow,       Resources.GVFilter_ParamLevelHigh,       'L', ParameterValueType.FloatPercent, 0.0f, 1.0f, 0.0f) });
        

        // フィルターの表示名
        private string m_displayName;

        // パラメータの情報
        private ParameterInfo[] m_parameterList;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]displayInfo    フィルターの表示名
        // 　　　　[in]parmeterList   パラメータの情報
        // 戻り値：なし
        //=========================================================================================
        private FilterMetaInfo(string displayInfo, ParameterInfo[] parameterList) {
            m_displayName = displayInfo;
            m_parameterList = parameterList;
        }

        //=========================================================================================
        // プロパティ：フィルターの表示名
        //=========================================================================================
        public string DisplayName {
            get {
                return m_displayName;
            }
        }

        //=========================================================================================
        // プロパティ：パラメータの情報
        //=========================================================================================
        public ParameterInfo[] ParameterList {
            get {
                return m_parameterList;
            }
        }

        //=========================================================================================
        // クラス：フィルターへのパラメータの定義
        //=========================================================================================
        public class ParameterInfo {
            // パラメータの表示名
            private string m_displayName;

            // レベル小での表示名
            private string m_lowLevelDisplayName;

            // レベル大での表示名
            private string m_highLevelDisplayName;

            // パラメータUIのショートカットキー
            private char m_paramShortcut;

            // パラメータの型
            private ParameterValueType m_valueType;

            // パラメータの最小値
            private object m_minValue;

            // パラメータの最大値
            private object m_maxValue;

            // パラメータのデフォルト値
            private object m_defaultValue;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]displayName    パラメータの表示名
            // 　　　　[in]lowDispName    レベル小での表示名
            // 　　　　[in]highDispName   レベル大での表示名
            // 　　　　[in]paramShortcut  パラメータUIのショートカットキー
            // 　　　　[in]valueType      パラメータの型
            // 　　　　[in]minValue       パラメータの最小値
            // 　　　　[in]maxValue       パラメータの最大値
            // 　　　　[in]defaultValue   パラメータのデフォルト値
            // 戻り値：なし
            //=========================================================================================
            public ParameterInfo(string displayName, string lowDispName, string highDispName, char paramShortcut, ParameterValueType valueType, object minValue, object maxValue, object defaultValue) {
                m_displayName = displayName;
                m_lowLevelDisplayName = lowDispName;
                m_highLevelDisplayName = highDispName;
                m_paramShortcut = paramShortcut;
                m_valueType = valueType;
                m_minValue = minValue;
                m_maxValue = maxValue;
                m_defaultValue = defaultValue;
            }

            //=========================================================================================
            // プロパティ：パラメータの表示名
            //=========================================================================================
            public string DisplayName {
                get {
                    return m_displayName;
                }
            }

            //=========================================================================================
            // レベル小での表示名
            //=========================================================================================
            public string LowLevelDisplayName {
                get {
                    return m_lowLevelDisplayName;
                }
            }

            //=========================================================================================
            // レベル大での表示名
            //=========================================================================================
            public string HighLevelDisplayName {
                get {
                    return m_highLevelDisplayName;
                }
            }

            //=========================================================================================
            // プロパティ：パラメータUIのショートカットキー
            //=========================================================================================
            public char ParameterShortcut {
                get {
                    return m_paramShortcut;
                }
            }

            //=========================================================================================
            // プロパティ：パラメータの型
            //=========================================================================================
            public ParameterValueType ParameterValueType {
                get {
                    return m_valueType;
                }
            }

            //=========================================================================================
            // パラメータの最小値
            //=========================================================================================
            public object MinValue {
                get {
                    return m_minValue;
                }
            }
            
            //=========================================================================================
            // パラメータの最大値
            //=========================================================================================
            public object MaxValue {
                get {
                    return m_maxValue;
                }
            }

            //=========================================================================================
            // パラメータのデフォルト値
            //=========================================================================================
            public object DefaultValue {
                get {
                    return m_defaultValue;
                }
            }
        }

        //=========================================================================================
        // 列挙子：パラメータの型
        //=========================================================================================
        public enum ParameterValueType {
            // float型 0～1で0%～100%
            FloatPercent,
            // float型 -1～1で-100%～100%
            FloatSignedPercent,
            // float型 小数点以下切り捨ての数値
            FloatValue0,
            // float型 少数点以下2桁の数値
            FloatValue2,
        }
    }
}
