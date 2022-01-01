using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileViewer;
using ShellFiler.Util;
using ShellFiler.Locale;
using ShellFiler.UI;
using ShellFiler.UI.Dialog;

namespace ShellFiler.Command.FileViewer.View {

    //=========================================================================================
    // クラス：コマンドを実行する
    // テキストのエンコード方式を{0}に設定します。
    //   書式 　 V_SetTextEncoding(string code)
    //   引数  　code:文字コード
    // 　　　　　code-default:SJIS
    // 　　　　　code-range:SJIS=SJIS,UTF-8=UTF-8,EUC-JP=EUC-JP,JIS=JIS,UNICODE=UNICODE
    //   戻り値　エンコードを変更したときtrue
    //   対応Ver 0.0.1
    //=========================================================================================
    class V_SetTextEncodingCommand : FileViewerActionCommand {
        // 文字コード
        private string m_code;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_SetTextEncodingCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            m_code = (string)param[0];
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
            EncodingType encoding = CodeParameterToEncoding(m_code);
            if (encoding == EncodingType.UNKNOWN) {
                return false;
            }
            return TextFileViewer.TextViewerComponent.SetEncoding(encoding);
        }

        //=========================================================================================
        // 機　能：数値のcodeパラメータをエンコーディングの定数に変換する
        // 引　数：[in]code  文字コードの文字列指定
        // 戻り値：エンコーディング
        //=========================================================================================
        public static EncodingType CodeParameterToEncoding(string code) {
            EncodingType encoding = EncodingType.UNKNOWN;
            if (code == "SJIS") {
                encoding = EncodingType.SJIS;
            } else if (code == "UTF-8") {
                encoding = EncodingType.UTF8;
            } else if (code == "EUC-JP") {
                encoding = EncodingType.EUC;
            } else if (code == "JIS") {
                encoding = EncodingType.JIS;
            } else if (code == "UNICODE") {
                encoding = EncodingType.UNICODE;
            }
            return encoding;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_SetTextEncodingCommand;
            }
        }
    }
}
