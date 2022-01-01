using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Locale;
using ShellFiler.FileTask;
using ShellFiler.Properties;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：同名ファイルダイアログでの詳細情報コントロール
    //=========================================================================================
    public partial class SameNameFileInfoControl : UserControl {
        // コンテキスト情報
        private FileOperationRequestContext m_context;

        // 転送元のアイコン
        private Icon m_iconSrc;

        // 転送先のアイコン
        private Icon m_iconDest;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SameNameFileInfoControl() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：コントロールを初期化する
        // 引　数：[in]cacheContext   コンテキスト情報
        // 　　　　[in]fileDetail     同名ファイルの詳細情報
        // 戻り値：なし
        //=========================================================================================
        public void InitializeControl(FileOperationRequestContext context, SameNameTargetFileDetail fileDetail) {
            // 表示する内容を決定
            m_context = context;
            fileDetail.ExtractIcon();
            m_iconSrc = fileDetail.SrcIcon;
            m_iconDest = fileDetail.DestIcon;
            if (fileDetail.SuccessFullInfo) {
                string srcModTime = DateTimeFormatter.DateTimeToInformation(fileDetail.SrcLastWriteTime);
                string destModTime = DateTimeFormatter.DateTimeToInformation(fileDetail.DestLastWriteTime);
                string srcFileSize = formatSize(fileDetail.SrcFileSize);
                string destFileSize = formatSize(fileDetail.DestFileSize);

                // コントロールに設定
                this.textBoxSrcPath.Text = fileDetail.SrcFilePath;
                this.textBoxSrcSize.Text = srcFileSize;
                this.textBoxSrcTime.Text = srcModTime;
                this.textBoxDestPath.Text = fileDetail.DestFilePath;
                this.textBoxDestSize.Text = destFileSize;
                this.textBoxDestTime.Text = destModTime;

                // 大小比較
                int index = 0;
                int compDate = BackgroundTaskCommandUtil.CompareFileDate(fileDetail.SrcFileSystemId, fileDetail.DestFileSystemId, fileDetail.SrcLastWriteTime, fileDetail.DestLastWriteTime);
                if (compDate < 0) {
                    index = 0;
                } else if (compDate == 0) {
                    index = 3;
                } else {
                    index = 6;
                }
                if (fileDetail.SrcFileSize < fileDetail.DestFileSize) {
                    index += 0;
                } else if (fileDetail.SrcFileSize == fileDetail.DestFileSize) {
                    index += 1;
                } else {
                    index += 2;
                }
                switch (index) {
                    case 0:
                        this.labelSrcCompare.Text = Resources.DlgSameFile_OldSmall;
                        this.labelDestCompare.Text = Resources.DlgSameFile_NewBig;
                        break;
                    case 1:
                        this.labelSrcCompare.Text = Resources.DlgSameFile_OldEq;
                        this.labelDestCompare.Text = Resources.DlgSameFile_NewEq;
                        break;
                    case 2:
                        this.labelSrcCompare.Text = Resources.DlgSameFile_OldBig;
                        this.labelDestCompare.Text = Resources.DlgSameFile_NewSmall;
                        break;
                    case 3:
                        this.labelSrcCompare.Text = Resources.DlgSameFile_EqSmall;
                        this.labelDestCompare.Text = Resources.DlgSameFile_EqBig;
                        break;
                    case 4:
                        this.labelSrcCompare.Text = Resources.DlgSameFile_EqEq;
                        this.labelDestCompare.Text = Resources.DlgSameFile_EqEq;
                        break;
                    case 5:
                        this.labelSrcCompare.Text = Resources.DlgSameFile_EqBig;
                        this.labelDestCompare.Text = Resources.DlgSameFile_EqSmall;
                        break;
                    case 6:
                        this.labelSrcCompare.Text = Resources.DlgSameFile_NewSmall;
                        this.labelDestCompare.Text = Resources.DlgSameFile_OldBig;
                        break;
                    case 7:
                        this.labelSrcCompare.Text = Resources.DlgSameFile_NewEq;
                        this.labelDestCompare.Text = Resources.DlgSameFile_OldEq;
                        break;
                    case 8:
                        this.labelSrcCompare.Text = Resources.DlgSameFile_NewBig;
                        this.labelDestCompare.Text = Resources.DlgSameFile_OldSmall;
                        break;
                }
            } else {
                // コントロールに設定
                this.textBoxSrcPath.Text = fileDetail.SrcFilePath;
                this.textBoxSrcSize.Text = "?";
                this.textBoxSrcTime.Text = "?";
                this.textBoxDestPath.Text = fileDetail.DestFilePath;
                this.textBoxDestSize.Text = "?";
                this.textBoxDestTime.Text = "?";
                this.labelSrcCompare.Text = "";
                this.labelDestCompare.Text = "";
            }
        }

        //=========================================================================================
        // 機　能：サイズの値を文字列化する
        // 引　数：[in]size  サイズ値
        // 戻り値：サイズの文字列表現
        //=========================================================================================
        private string formatSize(long size) {
            const long TERA = 1024L * 1024L * 1024L * 1024L;
            const long GIGA = 1024L * 1024L * 1024L;
            const long MEGA = 1024L * 1024L;
            const long KIRO = 1024L;
            string strSize;
            if (size > TERA) {
                strSize = String.Format("{0}.{1,000}T ({2}byte)", size / TERA, ((size / GIGA) % 1024) * 1000 / 1024, size);
            } else if (size > GIGA) {
                strSize = String.Format("{0}.{1,000}G ({2}byte)", size / GIGA, ((size / MEGA) % 1024) * 1000 / 1024, size);
            } else if (size > MEGA) {
                strSize = String.Format("{0}.{1,000}M ({2}byte)", size / MEGA, ((size / KIRO) % 1024) * 1000 / 1024, size);
            } else {
                strSize = String.Format("{0}", size);
            }
            return strSize;
        }

        //=========================================================================================
        // 機　能：描画イベント受信時の処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void SameNameFileInfoControl_Paint(object sender, PaintEventArgs evt) {
            Graphics g = evt.Graphics;
            const int MARGIN_ICON_TO_LABEL = 4;
            int xPosSrc = this.labelSrcCompare.Location.X - UIIconManager.CX_LARGE_ICON - MARGIN_ICON_TO_LABEL;
            int yPosSrc = this.labelSrcCompare.Location.Y - (UIIconManager.CY_LARGE_ICON - this.labelSrcCompare.Height) / 2;
            int xPosDest = xPosSrc;
            int yPosDest = this.labelDestCompare.Location.Y - (UIIconManager.CY_LARGE_ICON - this.labelDestCompare.Height) / 2;
            if (m_iconSrc != null) {
                g.DrawIcon(m_iconSrc, xPosSrc, yPosSrc);
            }
            if (m_iconDest != null) {
                g.DrawIcon(m_iconDest, xPosDest, yPosDest);
            }
        }
    }
}
