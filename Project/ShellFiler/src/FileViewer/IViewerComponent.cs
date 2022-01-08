﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Command.FileViewer;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // インターフェース：ビューアのインターフェース
    //=========================================================================================
    public interface IViewerComponent {

        //=========================================================================================
        // 機　能：該当の行番号が描画対象となるかどうかを返す
        // 引　数：[in]minLine  最小行番号
        // 　　　　[in]maxLine  最大行番号
        // 　　　　[in]minByte  最小ファイル位置
        // 　　　　[in]maxByte  最大ファイル位置
        // 戻り値：描画対象のときtrue
        //=========================================================================================
        bool IsDisplay(int minLine, int maxLine, int minByte, int maxByte);

        //=========================================================================================
        // 機　能：サイズ変更時の処理を行う
        // 引　数：[in]rcClient     クライアント領域
        // 戻り値：なし
        //=========================================================================================
        void OnSizeChange(Rectangle rcClient);
        
        //=========================================================================================
        // 機　能：水平スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        void SetHorizontalScrollbar();
        
        //=========================================================================================
        // 機　能：垂直スクロールバーをセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        void SetVerticalScrollbar();
        
        //=========================================================================================
        // 機　能：画面を描画する
        // 引　数：[in]g         グラフィックス
        // 　　　　[in]fillBack  背景を塗りつぶすときtrue
        // 　　　　[in]startLine 開始行（-1で全体を描画）
        // 　　　　[in]endLine   終了行
        // 戻り値：なし
        //=========================================================================================
        void OnPaint(TextViewerGraphics g, bool fillBack, int startLine, int endLine);
        
        //=========================================================================================
        // 機　能：スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        void OnHScroll(ScrollEventArgs evt);

        //=========================================================================================
        // 機　能：スクロールイベントを処理する
        // 引　数：[in]evt  スクロールイベント
        // 戻り値：なし
        //=========================================================================================
        void OnVScroll(ScrollEventArgs evt);
        
        //=========================================================================================
        // 機　能：マウスホイールイベントを処理する
        // 引　数：[in]evt  マウスイベント
        // 戻り値：なし
        //=========================================================================================
        void OnMouseWheel(MouseEventArgs evt);

        //=========================================================================================
        // 機　能：表示位置を上下に移動する
        // 引　数：[in]lines    移動する行数（下方向が＋）
        // 戻り値：なし
        //=========================================================================================
        void MoveDisplayPosition(int lines);
        
        //=========================================================================================
        // 機　能：マウスのボタンがダブルクリックされたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void OnMouseDoubleClick(MouseEventArgs evt);

        //=========================================================================================
        // 機　能：マウスのボタンが押されたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void OnMouseDown(MouseEventArgs evt);

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]mouseX  マウスカーソルのX位置
        // 　　　　[in]mouseY  マウスカーソルのY位置
        // 戻り値：なし
        //=========================================================================================
        void OnMouseMove(int mouseX, int mouseY);

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void OnMouseUp(MouseEventArgs evt);

        //=========================================================================================
        // 機　能：右クリックによりコンテキストメニューの表示指示が行われたときの処理を行う
        // 引　数：[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void OnContextMenu(MouseEventArgs evt);

        //=========================================================================================
        // 機　能：選択をキャンセルする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        void CancelSelect();

        //=========================================================================================
        // 機　能：すべて選択する
        // 引　数：なし
        // 戻り値：選択に成功したときtrue
        //=========================================================================================
        bool SelectAll();

        //=========================================================================================
        // 機　能：選択位置をクリップボードにコピーする
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        bool CopyText();

        //=========================================================================================
        // 機　能：選択位置を形式を指定してクリップボードにコピーする
        // 引　数：なし
        // 戻り値：コピーに成功したときtrue
        //=========================================================================================
        bool CopyTextAs();

        //=========================================================================================
        // 機　能：タブ幅を変更する
        // 引　数：[in]tab  新しいタブ幅
        // 戻り値：タブ幅の変更に成功したときtrue
        //=========================================================================================
        bool SetTab(int tab);

        //=========================================================================================
        // 機　能：エンコードを変更する
        // 引　数：[in]encoding  新しいエンコード
        // 戻り値：エンコードの変更に成功したときtrue
        //=========================================================================================
        bool SetEncoding(EncodingType encoding);

        //=========================================================================================
        // 機　能：改行幅をセットする
        // 引　数：[in]setting  改行幅の設定
        // 戻り値：改行幅の変更に成功したときtrue
        //=========================================================================================
        bool SetLineWidth(TextViewerLineBreakSetting setting);

        //=========================================================================================
        // プロパティ：行番号表示領域の幅
        //=========================================================================================
        float LineNoAreaWidth {
            get;
        }

        //=========================================================================================
        // プロパティ：画面上に表示可能な行数行番号表示領域の幅
        //=========================================================================================
        int CompleteLineSize {
            get;
        }

        //=========================================================================================
        // プロパティ：先頭行に表示している表示上の行数
        //=========================================================================================
        int VisibleTopLine {
            get;
        }
    }
}