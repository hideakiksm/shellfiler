using System;
using System.Collections.Generic;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.FileTask;
using ShellFiler.FileSystem.Virtual;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：グラフィックビューアの起動パラメータ
    //=========================================================================================
    public class GraphicsViewerParameter {
        // グラフィックビューアの起動モード
        private GraphicsViewerMode m_graphicsViewerMode;

        // グラフィックビューア／スライドショーの起動パラメータ
        private GraphicsViewerModeParameter m_forGraphicsViewer;

        // クリップボードビューアの起動パラメータ
        private ClipboardViewer m_forClipBoardViewer;

        //=========================================================================================
        // 機　能：グラフィックビューア用の起動パラメータを作成する
        // 引　数：[in]fileNameList  スライドショーの対象となるファイル一覧
        // 　　　　[in]initialFile   はじめの表示対象となるファイル一覧中のインデックス
        // 　　　　[in]fileSystem    対象となるファイルシステム
        // 　　　　[in]fileListCtx   対象となるファイル一覧のコンテキスト情報
        // 　　　　[in]basePath      スライドショーの実行パス（最後はセパレータ）
        // 　　　　[in]task          バックグラウンドタスク
        // 戻り値：起動パラメータ
        //=========================================================================================
        public static GraphicsViewerParameter CreateForGraphicsViewer(List<string> fileNameList, int initialFile, IFileSystem fileSystem, IFileListContext fileListCtx, string basePath) {
            GraphicsViewerParameter param = new GraphicsViewerParameter();
            param.m_graphicsViewerMode = GraphicsViewerMode.GraphicsViewer;
            param.m_forGraphicsViewer = new GraphicsViewerModeParameter(fileNameList, initialFile, fileSystem, fileListCtx, basePath);
            param.m_forClipBoardViewer = null;
            return param;
        }

        //=========================================================================================
        // 機　能：スライドショー用の起動パラメータを作成する
        // 引　数：[in]fileNameList  スライドショーの対象となるファイル一覧
        // 　　　　[in]initialFile   はじめの表示対象となるファイル一覧中のインデックス
        // 　　　　[in]fileSystem    対象となるファイルシステム
        // 　　　　[in]fileListCtx   対象となるファイル一覧のコンテキスト情報
        // 　　　　[in]basePath      スライドショーの実行パス（最後はセパレータ）
        // 　　　　[in]task          バックグラウンドタスク
        // 戻り値：起動パラメータ
        //=========================================================================================
        public static GraphicsViewerParameter CreateForSlideShow(List<string> fileNameList, int initialFile, IFileSystem fileSystem, IFileListContext fileListCtx, string basePath) {
            GraphicsViewerParameter param = new GraphicsViewerParameter();
            param.m_graphicsViewerMode = GraphicsViewerMode.SlideShow;
            param.m_forGraphicsViewer = new GraphicsViewerModeParameter(fileNameList, initialFile, fileSystem, fileListCtx, basePath);
            param.m_forClipBoardViewer = null;
            return param;
        }

        //=========================================================================================
        // 機　能：クリップボードビューア用の起動パラメータを作成する
        // 引　数：[in]targetImage  表示する画像
        // 戻り値：起動パラメータ
        //=========================================================================================
        public static GraphicsViewerParameter CreateForClipboardViewer(ImageInfo targetImage) {
            GraphicsViewerParameter param = new GraphicsViewerParameter();
            param.m_graphicsViewerMode = GraphicsViewerMode.ClipboardViewer;
            param.m_forGraphicsViewer = null;
            param.m_forClipBoardViewer = new ClipboardViewer(targetImage);
            return param;
        }
        
        //=========================================================================================
        // プロパティ：スライドショーとして起動するときtrue
        //=========================================================================================
        public GraphicsViewerMode GraphicsViewerMode {
            get {
                return m_graphicsViewerMode;
            }
        }

        //=========================================================================================
        // グラフィックビューア／スライドショーの起動パラメータ
        //=========================================================================================
        public GraphicsViewerModeParameter ForGraphicsViewer {
            get {
                return m_forGraphicsViewer;
            }
        }

        //=========================================================================================
        // クリップボードビューアの起動パラメータ
        //=========================================================================================
        public ClipboardViewer ForClipBoardViewer {
            get {
                return m_forClipBoardViewer;
            }
        }
    }

    //=========================================================================================
    // クラス：グラフィックビューア／スライドショーの起動パラメータ
    //=========================================================================================
    public class GraphicsViewerModeParameter {
        // スライドショーの対象となるファイル一覧
        private List<string> m_fileNameList;

        // はじめの表示対象となるファイル一覧中のインデックス
        private int m_initialFileIndex;

        // 対象となるファイルシステム
        private IFileSystem m_fileSystem;
        
        // ファイル一覧のコンテキスト情報
        private IFileListContext m_fileListContext;

        // スライドショーの実行パス（最後はセパレータ）
        private string m_basePath;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]fileNameList  スライドショーの対象となるファイル一覧
        // 　　　　[in]initialFile   はじめの表示対象となるファイル一覧中のインデックス
        // 　　　　[in]fileSystem    対象となるファイルシステム
        // 　　　　[in]fileListCtx   ファイル一覧のコンテキスト情報
        // 　　　　[in]basePath      スライドショーの実行パス（最後はセパレータ）
        // 戻り値：なし
        //=========================================================================================
        public GraphicsViewerModeParameter(List<string> fileNameList, int initialFile, IFileSystem fileSystem, IFileListContext fileListCtx, string basePath) {
            m_fileNameList = fileNameList;
            m_initialFileIndex = initialFile;
            m_fileSystem = fileSystem;
            m_fileListContext = fileListCtx;
            m_basePath = basePath;
        }

        //=========================================================================================
        // プロパティ：対象となるファイルシステム
        //=========================================================================================
        public IFileSystem FileSystem {
            get {
                return m_fileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：ファイル一覧のコンテキスト情報
        //=========================================================================================
        public IFileListContext FileListContext {
            get {
                return m_fileListContext;
            }
        }

        //=========================================================================================
        // プロパティ：スライドショーの対象となるファイル一覧
        //=========================================================================================
        public List<string> FileNameList {
            get {
                return m_fileNameList;
            }
        }

        //=========================================================================================
        // プロパティ：スライドショーの実行パス（最後はセパレータ）
        //=========================================================================================
        public string BasePath {
            get {
                return m_basePath;
            }
        }

        //=========================================================================================
        // プロパティ：はじめの表示対象となるファイル一覧中のインデックス
        //=========================================================================================
        public int InitialFileIndex {
            get {
                return m_initialFileIndex;
            }
        }
    }

    //=========================================================================================
    // クラス：クリップボードビューアの起動パラメータ
    //=========================================================================================
    public class ClipboardViewer {
        // 表示する画像
        private ImageInfo m_targetImage;

        //=========================================================================================
        // 機　能：クリップボードビューア用の起動パラメータを作成する
        // 引　数：[in]targetImage  表示する画像
        // 戻り値：起動パラメータ
        //=========================================================================================
        public ClipboardViewer(ImageInfo targetImage) {
            m_targetImage = targetImage;
        }

        //=========================================================================================
        // プロパティ：表示する画像
        //=========================================================================================
        public ImageInfo TargetImage {
            get {
                return m_targetImage;
            }
        }
    }
}
