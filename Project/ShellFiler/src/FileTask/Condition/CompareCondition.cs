using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：ファイル操作の対象となる比較条件
    //=========================================================================================
    public class CompareCondition {
        // 使用する転送条件のリスト
        private List<FileConditionItem> m_conditionList;

        // 正規表現オブジェクトのキャッシュ
        private Dictionary<string, Regex> m_regexCache = new Dictionary<string, Regex>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]conditionList   使用する転送条件のリスト
        // 戻り値：なし
        //=========================================================================================
        public CompareCondition(List<FileConditionItem> conditionList) {
            m_conditionList = conditionList;
        }

        //=========================================================================================
        // 機　能：キャッシュから正規表現オブジェクトを取得する
        // 引　数：[in]regString   正規表現文字列
        // 　　　　[in]wildCard    正規表現がワイルドカード形式の時true
        // 戻り値：なし
        //=========================================================================================
        public Regex GetCachedRegularExpression(string regString, bool wildCard) {
            string cacheKey = (wildCard ? "w:" : "r:") + regString;
            if (m_regexCache.ContainsKey(cacheKey)) {
                return m_regexCache[cacheKey];
            } else {
                Regex regex;
                if (wildCard) {
                    regex = new Regex(WildCardConverter.ConvertWildCardToRegexString(regString));
                } else {
                    regex = new Regex(regString);
                }
                m_regexCache.Add(cacheKey, regex);
                return regex;
            }
        }

        //=========================================================================================
        // プロパティ：使用する転送条件のリスト
        //=========================================================================================
        public List<FileConditionItem> ConditionList {
            get {
                return m_conditionList;
            }
            set {
                m_conditionList = value;
            }
        }
    }
}
