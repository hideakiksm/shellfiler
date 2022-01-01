using System;
using System.Collections.Generic;
using ShellFiler.Api;
using ShellFiler.FileViewer;
using ShellFiler.FileSystem.Virtual;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：グラフィックビューアでの画像読み込みのリクエスト
    //=========================================================================================
    public class GraphicsViewerImageLoadRequest {
        // 読み込み対象のファイルシステム
        private IFileSystem m_targetFileSystem;

        // 読み込み対象のファイル一覧のコンテキスト情報
        private IFileListContext m_targetFileListContext;

        // 読み込みのベースとなるパス
        private string m_basePath;

        // 読み込み対象の画像
        private ImageInfo m_loadingImage;

        // 読み込み方向が正方向のときtrue、逆方向のときfalse
        private bool m_forward;

        // 読み込み中のフォーム
        private GraphicsViewerForm m_graphicsViewerForm;

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：[in]basePath            読み込みのベースとなるパス
        // 　　　　[in]fileListContext     ファイル一覧のコンテキスト情報
        // 　　　　[in]loadingImage        読み込み対象の画像
        // 　　　　[in]graphicsViewerForm  読み込み中のフォーム
        // 戻り値：実行結果
        //=========================================================================================
        public GraphicsViewerImageLoadRequest(IFileSystem fileSystem, IFileListContext fileListContext, string basePath, ImageInfo loadingImage, bool forward, GraphicsViewerForm graphicsViewerForm) {
            m_targetFileSystem = fileSystem;
            m_targetFileListContext = fileListContext;
            m_basePath = basePath;
            m_loadingImage = loadingImage;
            m_forward = forward;
            m_graphicsViewerForm = graphicsViewerForm;
        }

        //=========================================================================================
        // プロパティ：読み込み対象のファイルシステム
        //=========================================================================================
        public IFileSystem TargetFileSystem {
            get {
                return m_targetFileSystem;
            }
        }

        //=========================================================================================
        // プロパティ：読み込み対象のファイル一覧のコンテキスト情報
        //=========================================================================================
        public IFileListContext TargetFileListContext {
            get {
                return m_targetFileListContext;
            }
        }

        //=========================================================================================
        // プロパティ：読み込みのベースとなるパス
        //=========================================================================================
        public string BasePath {
            get {
                return m_basePath;
            }
        }

        //=========================================================================================
        // プロパティ：スライドショーのベースとなるパス(スライドショー以外のときnull)
        //=========================================================================================
        public ImageInfo LoadingImage {
            get {
                return m_loadingImage;
            }
        }

        //=========================================================================================
        // プロパティ：読み込み方向が正方向のときtrue、逆方向のときfalse
        //=========================================================================================
        public bool Forward {
            get {
                return m_forward;
            }
        }

        //=========================================================================================
        // プロパティ：読み込み中のフォーム
        //=========================================================================================
        public GraphicsViewerForm GraphicsViewerForm {
            get {
                return m_graphicsViewerForm;
            }
        }
    }
}
