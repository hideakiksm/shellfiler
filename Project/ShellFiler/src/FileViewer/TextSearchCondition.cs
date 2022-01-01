using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Util;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：テキストビューアでの検索条件
    //=========================================================================================
    public class TextSearchCondition {
        // 検索キーワードの最大文字数
        public const int MAX_SEARCH_STRING_LENGTH = 64;

        // 検索文字列（検索しないときnull）
        private string m_searchString;

        // すべて小文字にした検索文字列（検索しないときnull）
        private string m_searchStringLower;

        // コンパイル済みの正規表現文字列
        private Regex m_compiledSearchRegEx;

        // 検索方法
        private TextSearchMode m_searchMode;
        
        // 語単位で検索するときtrue
        private bool m_searchWord;
        
        // 自動検索の検索文字列（検索しないときnull）
        private string m_autoSearchString;

        // すべて小文字にした自動検索の検索文字列（検索しないときnull）
        private string m_autoSearchStringLower;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TextSearchCondition() {
            m_searchString = null;
            m_searchStringLower = null;
            m_compiledSearchRegEx = null;
            m_searchMode = TextSearchMode.NormalIgnoreCase;
            m_searchWord = false;
            m_autoSearchString = null;
            m_autoSearchStringLower = null;
        }
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]src  コピー元
        // 戻り値：なし
        //=========================================================================================
        public TextSearchCondition(TextSearchCondition src) {
            m_searchString = src.m_searchString;
            m_searchStringLower = src.m_searchStringLower;
            m_compiledSearchRegEx = null;
            m_searchMode = src.m_searchMode;
            m_searchWord = src.m_searchWord;
            m_autoSearchString = src.m_autoSearchString;
            m_autoSearchStringLower = src.m_autoSearchStringLower;
        }

        //=========================================================================================
        // 機　能：検索語句を最大長でトリミングする
        // 引　数：[in]str       元の文字列
        // 　　　　[out]trimmed  トリミングしたときtrueを返す変数
        // 戻り値：なし
        //=========================================================================================
        public static string TrimBySearchLength(string str, out bool trimmed) {
            if (str == null) {
                trimmed = false;
                return str;
            } else if (str.Length > MAX_SEARCH_STRING_LENGTH) {
                trimmed = true;
                return str.Substring(0, MAX_SEARCH_STRING_LENGTH);
            } else {
                trimmed = false;
                return str;
            }
        }

        //=========================================================================================
        // 機　能：ワイルドカード用に正規表現のマッチ条件を評価する
        // 引　数：[in]m  マッチ条件
        // 戻り値：新しい条件
        //=========================================================================================
        private static string WildCardMatchEvaluator(Match m) {
            string s = m.Value;
            if (s.Equals("?")) {
                return ".";
            } else if (s.Equals("*")) {
                return ".*";
            } else {
                return Regex.Escape(s);
            }
        }

        //=========================================================================================
        // プロパティ：検索文字列（検索しないときnull）
        //=========================================================================================
        public string SearchString {
            get {
                return m_searchString;
            }
            set {
                m_searchString = value;
                if (m_searchString == null) {
                    m_searchStringLower = null;
                } else {
                    m_searchStringLower = StringCategory.ToLower(m_searchString);
                }
                m_compiledSearchRegEx = null;
            }
        }

        //=========================================================================================
        // プロパティ：すべて小文字にした検索文字列（検索しないときnull）
        //=========================================================================================
        public string SearchStringLower {
            get {
                return m_searchStringLower;
            }
        }

        //=========================================================================================
        // プロパティ：正規表現のコンパイル済みオブジェクト
        //=========================================================================================
        public Regex CompiledSearchRegEx {
            get {
                if (m_compiledSearchRegEx == null) {
                    if (m_searchMode == TextSearchMode.RegularExpression) {
                        // 正規表現そのもの
                        m_compiledSearchRegEx = new Regex(m_searchString);
                    } else if (m_searchMode == TextSearchMode.WildCardCaseSensitive) {
                        // ワイルドカード大文字小文字区別
                        string rPattern = Regex.Replace(m_searchString, ".", new MatchEvaluator(WildCardMatchEvaluator)); 
                        m_compiledSearchRegEx = new Regex(rPattern);
                    } else if (m_searchMode == TextSearchMode.WildCardIgnoreCase) {
                        // ワイルドカード大文字小文字同一視
                        string lower = StringCategory.ToLower(m_searchString);
                        string rPattern = Regex.Replace(lower, ".", new MatchEvaluator(WildCardMatchEvaluator)); 
                        m_compiledSearchRegEx = new Regex(rPattern);
                    }
                }
                return m_compiledSearchRegEx;
            }
        }

        //=========================================================================================
        // プロパティ：検索方法
        //=========================================================================================
        public TextSearchMode SearchMode {
            get {
                return m_searchMode;
            }
            set {
                m_searchMode = value;
                m_compiledSearchRegEx = null;
            }
        }
        
        //=========================================================================================
        // プロパティ：語単位で検索するときtrue
        //=========================================================================================
        public bool SearchWord {
            get {
                return m_searchWord;
            }
            set {
                m_searchWord = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：自動検索の検索文字列（検索しないときnull）
        //=========================================================================================
        public string AutoSearchString {
            get {
                return m_autoSearchString;
            }
            set {
                m_autoSearchString = value;
                if (m_autoSearchString == null) {
                    m_autoSearchStringLower = null;
                } else {
                    m_autoSearchStringLower = StringCategory.ToLower(m_autoSearchString);
                }
            }
        }

        //=========================================================================================
        // プロパティ：すべて小文字にした自動検索の検索文字列（検索しないときnull）
        //=========================================================================================
        public string AutoSearchStringLower {
            get {
                return m_autoSearchStringLower;
            }
        }
    }
}
