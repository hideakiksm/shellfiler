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
    // インターフェース：画像に適用するフィルタ
    //=========================================================================================
    public interface IFilterComponent {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]srcImage    元のイメージ
        // 　　　　[in]destImage   結果を書き込むイメージ
        // 　　　　[in]cxImage     画像の幅
        // 　　　　[in]cyImage     画像の高さ
        // 　　　　[in]stride      ストライド（次のラインまでのバイト数）
        // 戻り値：なし
        //=========================================================================================
        void Initialize(byte[] srcImage, byte[] destImage, int cxImage, int cyImage, int stride);
        
        //=========================================================================================
        // 機　能：パラメータを設定する
        // 引　数：[in]paramList  パラメータ
        // 戻り値：なし
        //=========================================================================================
        void SetParameter(params object[] paramList);

        //=========================================================================================
        // 機　能：フィルター処理を実行する
        // 引　数：[in]startLine   開始行
        // 　　　　[in]endLine     終了行
        // 戻り値：なし
        //=========================================================================================
        void FilterExecute(int startLine, int endLine, BooleanFlag cancelEvent);

        //=========================================================================================
        // プロパティ：フィルターの持つ機能のメタ情報
        //=========================================================================================
        FilterMetaInfo MetaInfo {
            get;
        }
    }
}
