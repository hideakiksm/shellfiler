using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Command;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.Locale;
using System.Runtime.InteropServices;

namespace ShellFiler.UI.FileList.Crawler {

    //=========================================================================================
    // クラス：サムネイルの読み込みリクエストの追加パラメータ
    //=========================================================================================
    class FileCrawlerCreateThumbnailRequestParam {
        // 読み込み対象のファイル
        private List<UIFileWithIndex> m_targetFiles;

        // ファイル画像のサイズ
        private Size m_imageSize;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]targetFiles  読み込み対象のファイル
        // 　　　　[in]imageSize    ファイル画像のサイズ
        // 戻り値：なし
        //=========================================================================================
        public FileCrawlerCreateThumbnailRequestParam(List<UIFileWithIndex> targetFiles, Size imageSize) {
            m_targetFiles = targetFiles;
            m_imageSize = imageSize;
        }

        //=========================================================================================
        // プロパティ：読み込み対象のファイル
        //=========================================================================================
        public List<UIFileWithIndex> TargetFiles {
            get {
                return m_targetFiles;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル画像のサイズ
        //=========================================================================================
        public Size ImageSize {
            get {
                return m_imageSize;
            }
        }
    }
}
