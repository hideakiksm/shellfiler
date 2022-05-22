using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.UI {

    //=========================================================================================
    // クラス：スプラッシュウィンドウ
    //=========================================================================================
    public partial class SplashWindow : Form {
        // 開始時刻
        private DateTime m_startTime;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SplashWindow() {
            InitializeComponent();
            this.Icon = Resources.ShellFilerMain;
            m_startTime = DateTime.Now;

            // ダブルバッファリング有効
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        //=========================================================================================
        // 機　能：描画イベントを処理する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SplashWindow_Paint(object sender, PaintEventArgs evt) {
            SplashWindowGraphics g = new SplashWindowGraphics(evt.Graphics);
            
            // タイトルロゴ
            g.Graphics.DrawRectangle(Pens.Black, 0, 0, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
            Rectangle rcSrc = new Rectangle(0, 0, UIIconManager.TitleLogo.Width, UIIconManager.TitleLogo.Height);
            Rectangle rcDest = new Rectangle(24, 28, rcSrc.Width, rcSrc.Height);
            g.Graphics.DrawImage(UIIconManager.TitleLogo, rcDest, rcSrc, GraphicsUnit.Pixel);

            // アイコン
            int iconSize = (int) (48 * evt.Graphics.DpiY / 96);
            Bitmap bmpIcon = UIIconManager.MainIcon48;
            Rectangle rcIconSrc = new Rectangle(0, 0, bmpIcon.Width, bmpIcon.Height);
            Rectangle rcIconDest = new Rectangle(this.ClientRectangle.Width - iconSize - rcDest.Left, rcDest.Bottom - iconSize, iconSize, iconSize);
            g.Graphics.DrawImage(bmpIcon, rcIconDest, rcIconSrc, GraphicsUnit.Pixel);

            // Version
            int yPos = 80;
            FileVersionInfo ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            string version = string.Format(Resources.SplashVersion, ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart);
            g.DrawString(version, g.EtcFont, Brushes.Black, 40, yPos);

            // Copyright
            yPos += 15;
            AssemblyCopyrightAttribute asmcpy = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute));
            string copyright = asmcpy.Copyright;
            g.DrawString(copyright, g.EtcFont, Brushes.Black, 40, yPos);
            yPos += 18;

            // ライブラリ
            yPos += 12;
            string[] libList = StringUtils.SeparateLine(Resources.SplashLibrary);
            for (int i = 0; i < libList.Length; i++) {
                g.DrawString(libList[i], g.LibraryFont, Brushes.Gray, 20, yPos);
                yPos += 10;
            }
        }

        //=========================================================================================
        // 機　能：描画状態で待機する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void WaitDisplay() {
            int millisec = (DateTime.Now - m_startTime).Milliseconds;
            int waitTime = Configuration.Current.SplashWindowWait - millisec;
            if (waitTime > 0) {
                Thread.Sleep(waitTime);
            }
        }
    }
}
