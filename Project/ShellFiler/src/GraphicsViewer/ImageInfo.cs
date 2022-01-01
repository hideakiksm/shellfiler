using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.Properties;
using ShellFiler.Util;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：イメージライフサイクル全体を管理するクラス
    //=========================================================================================
    public class ImageInfo {
        // 読み込んだファイルのパス
        private string m_filePath;

        // イメージの状態
        private ImageState m_imageState = ImageState.Null;

        // エラーの場合、そのエラーメッセージ（null:エラーなし）
        private string m_errorMessage = null;

        // 読み込んだイメージ（読み込んでいないときはnull）
        private BufferedImage m_image;

        // フィルタ適用済みのイメージ（読み込んでいないときはnull）
        private BufferedImage m_filteredImage;

        // 1ピクセルあたりの画像ビット数（読み込んでいないとき、不明なときは-1）
        private int m_imageColorBits;

        // マーク状態（0:マークなし、1～9:マーク）
        private int m_markState = 0;

        //=========================================================================================
        // 機　能：失敗時の画像情報を返す
        // 引　数：[in]filePath  読み込んだファイルのパス
        // 　　　　[in]status    完了時のステータス
        // 戻り値：なし
        //=========================================================================================
        public static ImageInfo Fail(string filePath, FileOperationStatus status) {
            string errorMessage;
            if (status == FileOperationStatus.ErrorConvertImage) {
                errorMessage = Resources.GraphicsViewer_ErrorConvert;
            } else if (status == FileOperationStatus.ErrorOutOfMemory) {
                errorMessage = Resources.GraphicsViewer_ErrorOutOfMemory;
            } else {
                errorMessage = Resources.GraphicsViewer_ErrorLoad;
            }
            return new ImageInfo(filePath, ImageState.Completed, errorMessage, null, -1);
        }

        //=========================================================================================
        // 機　能：スライドショーの最後の画像情報を返す
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public static ImageInfo LastSlide() {
            return new ImageInfo("", ImageState.Completed, Resources.GraphicsViewer_ErrorLast, null, -1);
        }

        //=========================================================================================
        // 機　能：成功時の画像情報を返す
        // 引　数：[in]filePath  読み込んだファイルのパス
        // 　　　　[in]image     読み込んだイメージ（失敗したときnull）
        // 　　　　[in]colorBits 1ピクセルあたりの画像ビット数（読み込んでいないとき、不明なときは-1）
        // 戻り値：なし
        //=========================================================================================
        public static ImageInfo Success(string filePath, BufferedImage image, int colorBits) {
            return new ImageInfo(filePath, ImageState.Completed, null, image, colorBits);
        }

        //=========================================================================================
        // 機　能：初期状態の画像情報を返す
        // 引　数：[in]filePath  読み込んだファイルのパス
        // 　　　　[in]image     読み込んだイメージ（失敗したときnull）
        // 　　　　[in]colorBits 1ピクセルあたりの画像ビット数（読み込んでいないとき、不明なときは-1）
        // 戻り値：なし
        //=========================================================================================
        public static ImageInfo InitialState(string filePath) {
            return new ImageInfo(filePath, ImageState.Null, null, null, -1);
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]filePath    画像ファイルのパス
        // 　　　　[in]imageState  画像の状態
        // 　　　　[in]message     エラーメッセージ（エラー時以外はnull）
        // 　　　　[in]image       読み込んだイメージ（失敗したときnull）
        // 　　　　[in]imageBits   1ピクセルあたりの画像ビット数（読み込んでいないとき、不明なときは-1）
        // 戻り値：なし
        //=========================================================================================
        private ImageInfo(string filePath, ImageState imageState, string message, BufferedImage image, int imageBits) {
            m_filePath = filePath;
            m_imageState = imageState;
            m_errorMessage = message;
            m_image = image;
            m_filteredImage = null;
            m_imageColorBits = imageBits;
        }

        //=========================================================================================
        // 機　能：フィルタ適用済みのイメージをセットする
        // 引　数：[in]filteredImage   フィルタ適用済みイメージ（null:元のイメージを使用）
        // 戻り値：なし
        //=========================================================================================
        public void SetFilteredImage(BufferedImage filteredImage) {
            if (m_filteredImage != null && m_image != m_filteredImage) {
                m_filteredImage.Dispose();
                m_filteredImage = null;
            }
            if (filteredImage == null) {
                m_filteredImage = m_image;
            } else {
                m_filteredImage = filteredImage;
            }
        }

        //=========================================================================================
        // 機　能：イメージを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisposeImage() {
            if (m_filteredImage != null && m_image != m_filteredImage) {
                m_filteredImage.Dispose();
                m_filteredImage = null;
            }
            if (m_image != null) {
                m_image.Dispose();
                m_image  = null;
            }
            m_imageColorBits = -1;
        }

        //=========================================================================================
        // 機　能：画像の状態を更新する
        // 引　数：[in]src  新しい状態の情報取得源
        // 戻り値：なし
        //=========================================================================================
        public void Update(ImageInfo src) {
            m_imageState = src.m_imageState;
            m_errorMessage = src.m_errorMessage;
            m_image = src.m_image;
            m_imageColorBits = src.m_imageColorBits;
        }

        //=========================================================================================
        // プロパティ：読み込んだファイルのパス
        //=========================================================================================
        public string FilePath {
            get {
                return m_filePath;
            }
        }

        //=========================================================================================
        // プロパティ：イメージの状態
        //=========================================================================================
        public ImageState State {
            get {
                return m_imageState;
            }
            set {
                m_imageState = value;
            }
        }

        //=========================================================================================
        // プロパティ：エラーの場合、そのエラーメッセージ（null:エラーなし）
        //=========================================================================================
        public string ErrorMessage {
            get {
                return m_errorMessage;
            }
        }

        //=========================================================================================
        // プロパティ：読み込んだイメージそのもの（読み込んでいないときはnull）
        //=========================================================================================
        public BufferedImage OriginalImage {
            get {
                return m_image;
            }
        }

        //=========================================================================================
        // プロパティ：表示用のイメージ（読み込んでいないときはnull）
        //=========================================================================================
        public BufferedImage Image {
            get {
                return m_filteredImage;
            }
        }

        //=========================================================================================
        // プロパティ：1ピクセルあたりの画像ビット数（読み込んでいないとき、不明なときは-1）
        //=========================================================================================
        public int ImageColorBits {
            get {
                return m_imageColorBits;
            }
            set {
                m_imageColorBits = value;
            }
        }

        //=========================================================================================
        // プロパティ：マーク状態（0:マークなし、1～9:マーク）
        //=========================================================================================
        public int MarkState {
            get {
                return m_markState;
            }
            set {
                m_markState = value;
            }
        }
    }
}
