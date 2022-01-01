using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileViewer.Edit {

    //=========================================================================================
    // クラス：コマンドを実行する
    // パラメータで指定されたテキスト{1}で前方方向:{0}に向かって、{2}行目(0開始)からファイルを検索します。
    //   書式 　 V_SearchDirect(bool forward, string keyword, int startLine)
    //   引数  　forward:前方方向へ検索
    // 　　　　　forward-default:true
    // 　　　　　forward-range:
    // 　　　　　keyword:検索キーワード
    // 　　　　　keyword-default:
    // 　　　　　keyword-range:
    // 　　　　　startLine:検索開始行
    // 　　　　　startLine-default:0
    // 　　　　　startLine-range:0,999999
    //   戻り値　検索を開始したときtrue
    //   対応Ver 0.0.1
    //=========================================================================================
    class V_SearchDirectCommand : FileViewerActionCommand {
        // 前方方向へ検索するときtrue
        private bool m_forward;

        // 検索キーワード
        private string m_keyword;

        // 検索開始行
        private int m_startLine;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_SearchDirectCommand() {
        }
        
        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_forward = (bool)param[0];
            m_keyword = (string)param[1];
            m_startLine = (int)param[2];
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            if (!Abailable) {
                return false;
            }
            if (TextFileViewer.TextViewerComponent is TextViewerComponent) {
                TextViewerComponent viewer = (TextViewerComponent)(TextFileViewer.TextViewerComponent);
                TextSearchCondition condition = new TextSearchCondition();
                bool trimmed;
                condition.SearchString = TextSearchCondition.TrimBySearchLength(m_keyword, out trimmed);
                condition.SearchMode = TextSearchMode.NormalIgnoreCase;
                condition.SearchWord = false;
                condition.AutoSearchString = null;
                viewer.CancelSelect();
                viewer.SearchText(condition, m_forward, m_startLine);
            } else if (TextFileViewer.TextViewerComponent is DumpViewerComponent) {
                DumpViewerComponent viewer = (DumpViewerComponent)(TextFileViewer.TextViewerComponent);
                DumpSearchCondition condition = new DumpSearchCondition();
                condition.SearchBytes = DumpUtils.StringToBytes(m_keyword);
                condition.AutoSearchBytes = null;
                viewer.CancelSelect();
                viewer.SearchBytes(condition, m_forward, m_startLine);
            }

            return true;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_SearchDirectCommand;
            }
        }
    }
}