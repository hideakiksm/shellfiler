using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.Api {

    //=========================================================================================
    // 列挙子：コマンドの発生原因となった種類
    //=========================================================================================
    public enum UICommandSender {
        MainMenubar,                    // メメインニューバー
        MainToolbar,                    // メインツールバー
        AddressBarLeft,                 // 左側のアドレスバー
        AddressBarRight,                // 右側のアドレスバー
        FileListHeader,                 // ファイル一覧のヘッダ
        MainStatusBar,                  // ステータスバー
        MainFunctionBar,                // ファンクションバー
        FileViewerMenu,                 // ファイルビューアのメニュー
        FileViewerToolbar,              // ファイルビューアのツールバー
        FileViewerFunctionBar,          // ファイルビューアのファンクションバー
        GraphicsViewerMenu,             // グラフィックビューアのメニュー
        GraphicsViewerToolbar,          // グラフィックビューアのツールバー
        GraphicsViewerFunctionBar,      // グラフィックビューアのファンクションバー
        TerminalMenu,                   // ターミナルのメニュー
        TerminalToolBar,                // ターミナルのツールバー
        TerminalFunctionBar,            // ターミナルのファンクションバー
        MonitoringViewerMenu,           // モニタリングビューアのメニュー
        MonitoringViewerToolBar,        // モニタリングビューアのツールバー
        MonitoringViewerFunctionBar,    // モニタリングビューアのファンクションバー
        StateListPanel,                 // 状態一覧パネル
        DummyForTest,                   // メニューの動作確認用
    }
}
