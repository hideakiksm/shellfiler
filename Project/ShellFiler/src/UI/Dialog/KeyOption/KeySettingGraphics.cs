using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using System.Runtime.InteropServices;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：キー設定の描画用グラフィックス
    //=========================================================================================
    public class KeySettingGraphics {
        // グラフィック
        private Graphics m_graphics;

        // 無効状態で選択中のブラシ
        private Brush m_disabledSelectionBrush = null;

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public KeySettingGraphics(Graphics graphics) {
            m_graphics = graphics;
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_disabledSelectionBrush != null) {
                m_disabledSelectionBrush.Dispose();
                m_disabledSelectionBrush = null;
            }
        }
        
        //=========================================================================================
        // プロパティ：グラフィックス
        //=========================================================================================
        public Graphics Graphics {
            get {
                return m_graphics;
            }
        }

        //=========================================================================================
        // プロパティ：無効状態で選択中のブラシ
        //=========================================================================================
        public Brush DisabledSelectionBrush {
            get {
                if (m_disabledSelectionBrush == null) {
                    m_disabledSelectionBrush = new SolidBrush(Color.FromArgb(192, 192, 192));
                }
                return m_disabledSelectionBrush;
            }
        }
    }
}
