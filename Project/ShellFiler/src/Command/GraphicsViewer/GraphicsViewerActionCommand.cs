﻿using System;
using ShellFiler.Api;
using ShellFiler.Command;
using ShellFiler.GraphicsViewer;

namespace ShellFiler.Command.GraphicsViewer {
    
    //=========================================================================================
    // クラス：キー、ツールバーなどのイベントを受けて実行するテキストファイルビューア用のコマンド
    //=========================================================================================
    public abstract class GraphicsViewerActionCommand : ActionCommand {
        // ファイルビューア
        private GraphicsViewerPanel m_graphicsViewer;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerActionCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public virtual void SetParameter(params object[] param) {
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]graphicsViewer グラフィックファイルビューア
        // 　　　　[in]keyEvt         キー入力イベント（キー起因ではないときnull）
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(GraphicsViewerPanel graphicsViewer, KeyCommand keyEvt) {
            m_graphicsViewer = graphicsViewer;
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：[in]param パラメータ
        // 戻り値：実行結果
        //=========================================================================================
        public abstract object Execute();

        //=========================================================================================
        // プロパティ：グラフィックビューア
        //=========================================================================================
        public GraphicsViewerPanel GraphicsViewerPanel {
            get {
                return m_graphicsViewer;
            }
        }

        //=========================================================================================
        // プロパティ：イメージが利用できるときtrue
        //=========================================================================================
        public bool Abailable {
            get {
                return m_graphicsViewer.ImageAvailable;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public abstract UIResource UIResource {
            get;
        }
    }
}
