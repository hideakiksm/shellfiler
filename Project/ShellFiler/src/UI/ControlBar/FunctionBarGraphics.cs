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

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：ファンクションバーの描画用グラフィックス
    //=========================================================================================
    public class FunctionBarGraphics : HighDpiGraphics {
        // グラフィック（null:未初期化）
        private Graphics m_graphics;

        // 描画対象のコントロール（null:グラフィック指定）
        private Control m_control;

        // ファンクションバー 描画用のフォント
        private Font m_functionBarFont = null;

        // 描画時のフォーマッタ
        private StringFormat m_stringFormat = null;

        // アイコンのキャッシュ
        private Dictionary<FileIconID, Bitmap> m_cacheMapIconIdToBmp = new Dictionary<FileIconID,Bitmap>();

        //=========================================================================================
        // 機　能：コンストラクタ（グラフィックス指定）
        // 引　数：[in]graphics  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public FunctionBarGraphics(Graphics graphics) : base(graphics) {
            m_control = null;
            m_graphics = graphics;
        }

        //=========================================================================================
        // 機　能：コンストラクタ（コントロール指定で必要時に初期化）
        // 引　数：[in]control    描画対象のコントロール
        // 戻り値：なし
        //=========================================================================================
        public FunctionBarGraphics(Control control) : base(control) {
            m_control = control;
            m_graphics = null;
        }

        //=========================================================================================
        // 機　能：グラフィックスを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override void Dispose() {
            if (m_stringFormat != null) {
                m_stringFormat.Dispose();
                m_stringFormat = null;
            }
            foreach (Bitmap bmp in m_cacheMapIconIdToBmp.Values) {
                bmp.Dispose();
            }
            m_cacheMapIconIdToBmp.Clear();
            if (m_functionBarFont != null) {
                m_functionBarFont.Dispose();
                m_functionBarFont = null;
            }
        }
        
        //=========================================================================================
        // 機　能：アイコンをキャッシュから返す
        // 引　数：[in]id  アイコンのID
        // 戻り値：アイコンのビットマップ
        //=========================================================================================
        public Bitmap GetIconCache(FileIconID id) {
            if (m_cacheMapIconIdToBmp.ContainsKey(id)) {
                return m_cacheMapIconIdToBmp[id];
            } else {
                return null;
            }
        }

        //=========================================================================================
        // 機　能：アイコンをキャッシュに登録する
        // 引　数：[in]id  アイコンのID
        // 　　　　[in]bmp ビットマップ
        // 戻り値：アイコンのビットマップ
        //=========================================================================================
        public void AddIconCache(FileIconID id, Bitmap bmp) {
            m_cacheMapIconIdToBmp.Add(id, bmp);
        }

        //=========================================================================================
        // プロパティ：ファンクションバー 描画用のフォント
        //=========================================================================================
        public Font FunctionBarFont {
            get {
                if (m_functionBarFont == null) {
                    m_functionBarFont = new Font(Configuration.Current.FunctionBarFontName, Configuration.Current.FunctionBarFontSize);
                }
                return m_functionBarFont;
            }
        }

        //=========================================================================================
        // プロパティ：文字列のフォーマッタ
        //=========================================================================================
        public StringFormat StringFormat {
            get {
                if (m_stringFormat == null) {
                    m_stringFormat = new StringFormat();
                }
                return m_stringFormat;
            }
        }
    }
}
