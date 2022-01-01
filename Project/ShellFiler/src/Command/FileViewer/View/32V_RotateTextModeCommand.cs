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
    // テキストの文字コードを順番に変更します。
    //   書式 　 V_RotateTextMode()
    //   引数  　なし
    //   戻り値　変更に成功したときtrue
    //   対応Ver 0.0.1
    //=========================================================================================
    class V_RotateTextModeCommand : FileViewerActionCommand {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public V_RotateTextModeCommand() {
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
            EncodingType encoding = TextFileViewer.TextBufferLineInfo.TextEncodingType;
            EncodingType newEncoding = EncodingType.UNKNOWN;
            if (encoding == EncodingType.SJIS) {
                newEncoding = EncodingType.UTF8;
            } else if (encoding == EncodingType.UTF8) {
                newEncoding = EncodingType.EUC;
            } else if (encoding == EncodingType.EUC) {
                newEncoding = EncodingType.JIS;
            } else if (encoding == EncodingType.JIS) {
                newEncoding = EncodingType.UNICODE;
            } else if (encoding == EncodingType.UNICODE) {
                newEncoding = EncodingType.SJIS;
            } else {
                return false;
            }
            return TextFileViewer.TextViewerComponent.SetEncoding(newEncoding);
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.V_RotateTextModeCommand;
            }
        }
    }
}
