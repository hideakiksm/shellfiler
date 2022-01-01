using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using ShellFiler.Api;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：テキスト検索の検索コア
    //=========================================================================================
    public class TextSearchCore {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TextSearchCore() {
        }

        //=========================================================================================
        // 機　能：1行の中から文字列を検索する
        // 引　数：[in]strLine          検索対象の文字列
        // 　　　　[in]condition        検索条件
        // 　　　　[out]hitPosSearch    検索結果の位置を返す変数（検索していないときnull）
        // 　　　　[out]hitLengthSearch 検索結果の長さを返す変数（検索していないときnull）
        // 　　　　[out]hitPosAuto      自動検索結果の位置を返す変数（検索していないときnull）
        // 　　　　[out]hitLengthAuto   自動検索結果の長さを返す変数（検索していないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void SearchLine(string strLine, TextSearchCondition condition, out List<int> hitPosSearch, out List<int> hitLengthSearch, out List<int> hitPosAuto, out List<int> hitLengthAuto) {
            // 検索文字列
            if (condition.SearchString != null && condition.SearchString != "") {
                string strLineLower;
                switch (condition.SearchMode) {
                    case TextSearchMode.NormalIgnoreCase:
                        strLineLower = StringCategory.ToLower(strLine);
                        SearchStringNormal(strLineLower, condition.SearchStringLower, condition.SearchWord, out hitPosSearch, out hitLengthSearch);
                        break;
                    case TextSearchMode.NormalCaseSensitive:
                        SearchStringNormal(strLine, condition.SearchStringLower, condition.SearchWord, out hitPosSearch, out hitLengthSearch);
                        break;
                    case TextSearchMode.WildCardIgnoreCase:
                        strLineLower = StringCategory.ToLower(strLine);
                        SearchStringRegExp(strLineLower, condition.CompiledSearchRegEx, condition.SearchWord, out hitPosSearch, out hitLengthSearch);
                        break;
                    case TextSearchMode.WildCardCaseSensitive:
                        SearchStringRegExp(strLine, condition.CompiledSearchRegEx, condition.SearchWord, out hitPosSearch, out hitLengthSearch);
                        break;
                    case TextSearchMode.RegularExpression:
                        SearchStringRegExp(strLine, condition.CompiledSearchRegEx, condition.SearchWord, out hitPosSearch, out hitLengthSearch);
                        break;
                    default:
                        hitPosSearch = new List<int>();
                        hitLengthSearch = new List<int>();
                        break;
                }
            } else {
                hitPosSearch = null;
                hitLengthSearch = null;
            }

            // 自動検索文字列
            if (condition.AutoSearchString != null) {
                string strLineLower = StringCategory.ToLower(strLine);
                SearchStringNormal(strLineLower, condition.AutoSearchStringLower, false, out hitPosAuto, out hitLengthAuto);
            } else {
                hitPosAuto = null;
                hitLengthAuto = null;
            }
        }

        //=========================================================================================
        // 機　能：文字列中の特定キーワードを通常検索（大文字小文字区別）で検索する
        // 引　数：[in]strLine    検索対象の文字列
        // 　　　　[in]keyword    検索キーワード
        // 　　　　[in]word       語句単位検索をするときtrue
        // 　　　　[out]hitPos    文字列中、ヒットしたインデックスのリストを返す変数（検索していないときnull）
        // 　　　　[out]hitLength 文字列中、ヒットした文字列長のリストを返す変数（検索していないときnull）
        // 戻り値：なし
        //=========================================================================================
        private void SearchStringNormal(string strLine, string keyword, bool word, out List<int> hitPos, out List<int> hitLength) {
            hitPos = new List<int>();
            hitLength = new List<int>();

            // 検索ヒット位置を探す
            int startPos = 0;
            while (startPos < strLine.Length) {
                int hitIndex = strLine.IndexOf(keyword, startPos);
                if (hitIndex == -1) {
                    break;
                }

                // 単語単位に絞り込む
                if (word) {
                    bool startOk = true;
                    if (hitIndex != 0) {
                        ViewerCharType prevStart = StringCategory.GetViewerCharType(strLine[hitIndex - 1]);
                        ViewerCharType start = StringCategory.GetViewerCharType(strLine[hitIndex]);
                        startOk = (prevStart != start);
                    }
                    bool endOk = true;
                    if (hitIndex + keyword.Length < strLine.Length) {
                        ViewerCharType end = StringCategory.GetViewerCharType(strLine[hitIndex + keyword.Length - 1]);
                        ViewerCharType nextEnd = StringCategory.GetViewerCharType(strLine[hitIndex + keyword.Length]);
                        endOk = (end != nextEnd);
                    }
                    if (startOk && endOk) {
                        hitPos.Add(hitIndex);
                        hitLength.Add(keyword.Length);
                    }
                } else {
                    hitPos.Add(hitIndex);
                    hitLength.Add(keyword.Length);
                }

                startPos = hitIndex + keyword.Length;
            }
        }
        
        //=========================================================================================
        // 機　能：文字列を正規表現で検索する
        // 引　数：[in]strLine    検索対象の文字列
        // 　　　　[in]regex      正規表現
        // 　　　　[in]word       語句単位検索をするときtrue
        // 　　　　[out]hitPos    文字列中、ヒットしたインデックスのリストを返す変数（検索していないときnull）
        // 　　　　[out]hitLength 文字列中、ヒットした文字列長のリストを返す変数（検索していないときnull）
        // 戻り値：なし
        //=========================================================================================
        private void SearchStringRegExp(string strLine, Regex regex, bool word, out List<int> hitPos, out List<int> hitLength) {
            hitPos = new List<int>();
            hitLength = new List<int>();
            MatchCollection matchCol = regex.Matches(strLine);
            foreach (Match match in matchCol) {
                if (word) {
                    bool startOk = true;
                    if (match.Index != 0) {
                        ViewerCharType prevStart = StringCategory.GetViewerCharType(strLine[match.Index - 1]);
                        ViewerCharType start = StringCategory.GetViewerCharType(strLine[match.Index]);
                        startOk = (prevStart != start);
                    }
                    bool endOk = true;
                    if (match.Index + match.Length < strLine.Length) {
                        ViewerCharType end = StringCategory.GetViewerCharType(strLine[match.Index + match.Length - 1]);
                        ViewerCharType nextEnd = StringCategory.GetViewerCharType(strLine[match.Index + match.Length]);
                        endOk = (end != nextEnd);
                    }
                    if (startOk && endOk) {
                        hitPos.Add(match.Index);
                        hitLength.Add(match.Length);
                    }
                } else {
                    hitPos.Add(match.Index);
                    hitLength.Add(match.Length);
                }
            }
        }

        //=========================================================================================
        // 機　能：通常検索の結果から、ヒット位置の強調位置の情報を作成する
        // 引　数：[in]hitPos        通常検索でヒットした位置の先頭（物理行の先頭からの情報）
        // 　　　　[in]keywordLength 検索キーワードの長さ
        // 　　　　[in]strStart      本文文字列で、該当する論理行の開始位置のインデックス
        // 　　　　[in]strLength     本文文字列で、該当する論理行の長さ
        // 戻り値：検索結果（文字列中、ヒットしたインデックスのリスト）
        // メ　モ：連続したヒット領域の結合、対象となる論理行の範囲内でのクリッピングを行う
        //         0 1 2 3 4 5 6 7 8
        //         □■■■□■■■□  （■StrLine中のヒットした文字）
        //               |→clipのとき
        //         入力はhitPos={1,5}、strStart=3、hitLength={3,3}
        //         出力は{3,4}, {5,8}
        //=========================================================================================
        public List<HitDiffPoint> GetHitDiffPointSearch(List<int> hitPos, List<int> hitLength, int strStart, int strLength) {
            // 変化点を登録
            List<HitDiffPoint> diffPoint = new List<HitDiffPoint>();    // 必ず偶数個
            bool connect = false;
            for (int i = 0; i < hitPos.Count; i++) {
                // 開始点：前回が継続状態でないなら登録
                if (connect) {
                    connect = false;
                } else {
                    diffPoint.Add(new HitDiffPoint(hitPos[i], true, false));
                }
                // 終了点：次と重なるなら継続
                if (i < hitPos.Count - 1 && hitPos[i] + hitLength[i] == hitPos[i + 1]) {
                    connect = true;
                } else {
                    diffPoint.Add(new HitDiffPoint(hitPos[i] + hitLength[i], false, false));
                }
            }

            // 開始点でクリッピング
            List<HitDiffPoint> diffPointClip = new List<HitDiffPoint>();    // 必ず偶数個
            int index = 0;
            while (index < diffPoint.Count - 1) {
                if (diffPoint[index + 1].Position <= strStart) {
                    // 次へ空送り
                    index += 2;
                } else if (diffPoint[index].Position < strStart && strStart < diffPoint[index + 1].Position) {
                    // 開始点をまたぐ
                    diffPointClip.Add(new HitDiffPoint(strStart, true, false));
                    index++;
                    break;
                } else {
                    // 開始点以降で開始
                    if (diffPoint[index].Position >= strStart + strLength) {         // 範囲内になし
                        return diffPointClip;
                    }
                    diffPointClip.Add(diffPoint[index]);
                    index++;
                    break;
                }
            }

            // 終了点でクリッピング（この位置でindexは奇数番目(ヒット終了)か、全処理終了）
            while (index < diffPoint.Count) {
                if (diffPoint[index].Position <= strStart + strLength) {
                    // まだ終了点に達していない
                    diffPointClip.Add(diffPoint[index++]);          // ヒット終了
                    if (index < diffPoint.Count) {                  // ヒット開始
                        if (diffPoint[index].Position < strStart + strLength) {
                            diffPointClip.Add(diffPoint[index++]);
                        } else {
                            break;
                        }
                    }
                } else {
                    // 終了点をまたぐ
                    diffPointClip.Add(new HitDiffPoint(strStart + strLength, false, false));
                    break;
                }
            }
            return diffPointClip;
        }

        //=========================================================================================
        // 機　能：通常検索と自動検索両方の結果から、ヒット位置の強調位置の情報を作成する
        // 引　数：[in]hitPosSearch  通常検索でヒットした位置の先頭（物理行の先頭からの情報）
        // 　　　　[in]hitPosAuto    自動検索でヒットした位置の先頭（物理行の先頭からの情報）
        // 　　　　[in]searchKeyLen  検索キーワードの長さ
        // 　　　　[in]autoKeyLen    自動検索キーワードの長さ
        // 　　　　[in]strStart      本文文字列で、該当する論理行の開始位置のインデックス
        // 　　　　[in]strLength     本文文字列で、該当する論理行の長さ
        // 戻り値：検索結果（文字列中、ヒットしたインデックスのリスト）
        // メ　モ：連続したヒット領域の結合、対象となる論理行の範囲内でのクリッピングを行う
        //         0 1 2 3 4 5 6 7 8 9
        //         □■■■□■■■□□  （検索：■StrLine中のヒットした文字）
        //         □□■■□□■■■■  （自動：■StrLine中のヒットした文字）
        //               |→clipのとき
        //         入力はhitPos={1,5}、autoPos={2,6,8}、hitLength={3,3}、autoLength={2,2}、strLength=4
        //         出力は{3,4,t,t}, {5,6,t,f}, {6,6,t,t}
        //=========================================================================================
        public List<HitDiffPoint> GetHitDiffPointAutoAndSearch(List<int> hitPosSearch, List<int> hitLengthSearch, List<int> hitPosAuto, List<int> hitLengthAuto, int strStart, int strLength) {
            List<HitDiffPoint> result = new List<HitDiffPoint>();
            
            // 検索キーワード、自動検索キーワードの変化点を検出
            List<HitDiffPoint> diffPointSearch;
            if (hitPosSearch == null || hitPosSearch.Count == 0) {
                diffPointSearch = new List<HitDiffPoint>();
            } else {
                diffPointSearch = GetHitDiffPointSearch(hitPosSearch, hitLengthSearch, strStart, strLength);
            }
            List<HitDiffPoint> diffPointAuto;           // HitAutoSearchではなく、HitSearchでフラグ表現
            if (hitPosAuto == null || hitPosAuto.Count == 0) {
                diffPointAuto = new List<HitDiffPoint>();
            } else {
                diffPointAuto = GetHitDiffPointSearch(hitPosAuto, hitLengthAuto, strStart, strLength);
            }

            // 合成
            bool prevHitSearch = false;
            bool prevHitAuto = false;
            int indexDiffSearch = 0;
            int indexDiffAuto = 0;
            while (true) {
                int searchValue = int.MaxValue;
                int autoValue = int.MaxValue;
                if (indexDiffSearch < diffPointSearch.Count) {
                    searchValue = diffPointSearch[indexDiffSearch].Position;
                }
                if (indexDiffAuto < diffPointAuto.Count) {
                    autoValue = diffPointAuto[indexDiffAuto].Position;
                }

                if (searchValue == int.MaxValue && autoValue == int.MaxValue) {
                    break;
                } else if (searchValue == autoValue) {
                    result.Add(new HitDiffPoint(searchValue, diffPointSearch[indexDiffSearch].HitSearch, diffPointAuto[indexDiffAuto].HitSearch));
                    prevHitSearch = diffPointSearch[indexDiffSearch].HitSearch;
                    prevHitAuto = diffPointAuto[indexDiffAuto].HitSearch;
                    indexDiffSearch++;
                    indexDiffAuto++;
                } else if (searchValue < autoValue) {
                    result.Add(new HitDiffPoint(searchValue, diffPointSearch[indexDiffSearch].HitSearch, prevHitAuto));
                    prevHitSearch = diffPointSearch[indexDiffSearch].HitSearch;
                    indexDiffSearch++;
                } else if (autoValue < searchValue) {
                    result.Add(new HitDiffPoint(autoValue, prevHitSearch, diffPointAuto[indexDiffAuto].HitSearch));
                    prevHitAuto = diffPointAuto[indexDiffAuto].HitSearch;
                    indexDiffAuto++;
                }
            }
            return result;
        }
    }
}
