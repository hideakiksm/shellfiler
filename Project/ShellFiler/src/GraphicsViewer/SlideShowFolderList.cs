using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.Properties;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：フォルダ内のスライドショー画像一覧
    //=========================================================================================
    public class SlideShowFolderList : ISlideShowList {
        // 画像のキャッシュ枚数
        private const int MAX_IMAGE_LOAD = 3;

        // 画像一覧（先頭と末尾の終了画面を含む）
        private ImageInfo[] m_imageList;

        // 画像一覧に対するインデックス
        private Dictionary<string, int> m_filePathToImageListIndex = new Dictionary<string, int>();

        // 最初と最後のメッセージ
        private ImageInfo m_firstLastImageInfo;

        // 現在表示中の画像のインデックス
        private int m_currentImageIndex;

        // スライドショーモードのときtrue
        private bool m_slideShowMode = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]gvParam       グラフィックビューアの起動パラメータ
        // 戻り値：なし
        //=========================================================================================
        public SlideShowFolderList(GraphicsViewerParameter gvParam) {
            m_firstLastImageInfo = ImageInfo.LastSlide();

            List<string> fileNameList = gvParam.ForGraphicsViewer.FileNameList;
            int fileCount = fileNameList.Count;
            m_imageList = new ImageInfo[fileCount + 2];
            m_imageList[0] = m_firstLastImageInfo;
            for (int i = 0; i < fileCount; i++) {
                string filePath = gvParam.ForGraphicsViewer.BasePath + fileNameList[i];
                m_imageList[i + 1] = ImageInfo.InitialState(filePath);
                m_filePathToImageListIndex.Add(filePath, i + 1);
            }
            m_imageList[m_imageList.Length - 1] = m_firstLastImageInfo;
            m_currentImageIndex = gvParam.ForGraphicsViewer.InitialFileIndex + 1;
        }

        //=========================================================================================
        // 機　能：すべての読み込み済みの画像の情報を破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisposeAllImage() {
            foreach (ImageInfo imageInfo in m_imageList) {
                imageInfo.DisposeImage();
            }
            m_imageList = null;
            m_currentImageIndex = -1;
        }

        //=========================================================================================
        // 機　能：ファイルの読み込みの結果を通知する
        // 引　数：[in]imageInfo  読み込んだ画像の情報
        // 戻り値：なし
        //=========================================================================================
        public void NotifyFileLoad(ImageInfo imageInfo) {
            int index = m_filePathToImageListIndex[imageInfo.FilePath];
            m_imageList[index].Update(imageInfo);
        }

        //=========================================================================================
        // 機　能：スライドショーで次の画像の切り替えをリクエストする
        // 引　数：[in]forward            順方向に切り替えるときtrue
        // 　　　　[out]showImage         表示する画像（表示するものがないときnull）
        // 　　　　[out]nextLoadImageList 次の表示のためにバックグラウンド読み込みする画像
        // 戻り値：次の画像に変更できるときtrue
        //=========================================================================================
        public bool GetNextSlide(bool forward, out ImageInfo showImage, out List<ImageInfo> nextLoadImageList) {
            showImage = null;
            nextLoadImageList = new List<ImageInfo>();
            if (forward && m_currentImageIndex >= m_imageList.Length - 1 || !forward && m_currentImageIndex <= 0) {
                // 最後を超えようとした場合、終わり
                return false;
            }

            int startIndex;
            int endIndex;
            int stepIndex;
            if (forward) {
                startIndex = Math.Max(1, m_currentImageIndex - 4);
                endIndex = Math.Min(m_currentImageIndex + 4, m_imageList.Length - 2);
                stepIndex = 1;
            } else {
                startIndex = Math.Min(m_currentImageIndex + 4, m_imageList.Length - 2);
                endIndex = Math.Max(1, m_currentImageIndex - 4);
                stepIndex = -1;
            }

            ImageState currentState = m_imageList[m_currentImageIndex].State;
            ImageState nextState = m_imageList[m_currentImageIndex + stepIndex].State;
            if (currentState == ImageState.Loading || currentState == ImageState.Null || nextState == ImageState.Loading) {
                return false;
            }
            showImage = m_imageList[m_currentImageIndex + stepIndex];

            int loadingCount = 0;
            bool futureImage = false;
            for (int i = startIndex; (stepIndex > 0) ? (i <= endIndex) : (i >= endIndex); i += stepIndex) {
                if (i == m_currentImageIndex) {
                    // 現在位置
                    futureImage = true;
                } else if (i == m_currentImageIndex + stepIndex) {
                    // 次の位置
                    // Loading→N/A
                    // Completed→Completed
                    // Null→Loading
                    if (m_imageList[i].State == ImageState.Null) {
                        m_imageList[i].State = ImageState.Loading;
                        nextLoadImageList.Add(m_imageList[i]);
                        loadingCount++;
                    }
                } else if (futureImage) {
                    // 今後表示される画像
                    // Loading→Loading
                    // Completed→Completed
                    // Null→Loading
                    if (m_imageList[i].State == ImageState.Loading) {
                        loadingCount++;
                    } else if (m_imageList[i].State == ImageState.Completed) {
                        ;
                    } else if (m_imageList[i].State == ImageState.Null) {
                        if (loadingCount < MAX_IMAGE_LOAD) {
                            m_imageList[i].State = ImageState.Loading;
                            nextLoadImageList.Add(m_imageList[i]);
                            loadingCount++;
                        }
                    }
                } else {
                    // 過去に表示された画像
                    // Loading → Null（処理は自動でキャンセル）
                    // Completed → Null
                    // Null → Null
                    if (m_imageList[i].State == ImageState.Loading) {
                        m_imageList[i].State = ImageState.Null;
                    } else if (m_imageList[i].State == ImageState.Completed) {
                        m_imageList[i].State = ImageState.Null;
                        m_imageList[i].DisposeImage();
                    }
                }
            }
            m_currentImageIndex += stepIndex;

            return true;
        }

        //=========================================================================================
        // 機　能：スライドショーモードであることを設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetSlideShowMode() {
            m_slideShowMode = true;
        }

        //=========================================================================================
        // プロパティ：スライドショーモードのときtrue
        //=========================================================================================
        public bool IsSlideShowMode {
            get {
                return m_slideShowMode;
            }
        }

        //=========================================================================================
        // プロパティ：すべての画像の枚数（最初と最後のメッセージを含む）
        //=========================================================================================
        public int AllImageCount {
            get {
                return m_imageList.Length;
            }
        }

        //=========================================================================================
        // プロパティ：画像一覧（最初と最後のメッセージを含む、クリップボードビューアのときnull）
        //=========================================================================================
        public ImageInfo[] AllImages {
            get {
                return m_imageList;
            }
        }

        //=========================================================================================
        // プロパティ：現在表示中の画像のインデックス
        //=========================================================================================
        public int CurrentIndex {
            get {
                return m_currentImageIndex;
            }
        }

        //=========================================================================================
        // プロパティ：現在表示中の画像
        //=========================================================================================
        public ImageInfo CurrentImage {
            get {
                return m_imageList[m_currentImageIndex];
            }
        }
    }
}
