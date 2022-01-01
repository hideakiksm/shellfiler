using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：スライドショーでの画像一覧のマーク結果
    //=========================================================================================
    public class SlideShowMarkResult {
        // 有効なマーク数
        public const int MARK_STATE_COUNT = 9;

        // フォルダ名
        private string m_folder;

        // 画像一覧
        private List<ImageFileInfo> m_fileList = new List<ImageFileInfo>();

        // マークごとの画像の分類
        private Dictionary<int, List<ImageFileInfo>> m_markIndex = new Dictionary<int, List<ImageFileInfo>>();

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SlideShowMarkResult() {
            for (int i = 0; i <= MARK_STATE_COUNT; i++) {
                m_markIndex.Add(i, new List<ImageFileInfo>());
            }
        }

        //=========================================================================================
        // 機　能：画像を追加する
        // 引　数：[in]filePath   追加する画像のファイルパス
        // 　　　　[in]markState  マーク状態（0:マーククリア、1～9:マーク）
        // 戻り値：なし
        //=========================================================================================
        public void AddImage(string filePath, int markState) {
            m_folder = GenericFileStringUtils.GetDirectoryName(filePath);
            string file = GenericFileStringUtils.GetFileName(filePath);
            ImageFileInfo imageFileInfo = new ImageFileInfo(file, markState);
            m_fileList.Add(imageFileInfo);
            m_markIndex[markState].Add(imageFileInfo);
        }

        //=========================================================================================
        // 機　能：マークされた画像ファイル名の一覧を取得する
        // 引　数：[in]markState  マーク状態（1～9:マーク）
        // 戻り値：画像のファイル名
        //=========================================================================================
        public List<string> GetMarkImage(int markState) {
            List<ImageFileInfo> markImages = m_markIndex[markState];
            List<string> result = new List<string>(markImages.Count);
            foreach (ImageFileInfo fileInfo in markImages) {
                result.Add(fileInfo.FileName);
            }
            return result;
        }

        //=========================================================================================
        // 機　能：マークされた画像ファイル数を取得する
        // 引　数：[in]markState  マーク状態（1～9:マーク）
        // 戻り値：画像ファイル数
        //=========================================================================================
        public int GetMarkImageCount(int markState) {
            return m_markIndex[markState].Count;
        }

        //=========================================================================================
        // プロパティ：フォルダ名
        //=========================================================================================
        public string Folder {
            get {
                return m_folder;
            }
        }

        //=========================================================================================
        // プロパティ：画像一覧
        //=========================================================================================
        public List<ImageFileInfo> FileList {
            get {
                return m_fileList;
            }
        }

        //=========================================================================================
        // プロパティ：有効な画像があるときtrue
        //=========================================================================================
        public bool Available {
            get {
                int markCount = 0;
                for (int i = 1; i <= MARK_STATE_COUNT; i++) {
                    markCount += m_markIndex[i].Count;
                }
                return (markCount > 0);
            }
        }

        //=========================================================================================
        // クラス：画像ファイルの情報
        //=========================================================================================
        public class ImageFileInfo {
            // ファイル名
            private string m_fileName;

            // マーク状態（0:マーククリア、1～9:マーク）
            private int m_markState;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]fileName   ファイル名
            // 　　　　[in]markState  マーク状態（0:マーククリア、1～9:マーク）
            // 戻り値：なし
            //=========================================================================================
            public ImageFileInfo(string fileName, int markState) {
                m_fileName = fileName;
                m_markState = markState;
            }
            //=========================================================================================
            // プロパティ：ファイル名
            //=========================================================================================
            public string FileName {
                get {
                    return m_fileName;
                }
            }

            //=========================================================================================
            // プロパティ：マーク状態（0:マーククリア、1～9:マーク）
            //=========================================================================================
            public int MarkState {
                get {
                    return m_markState;
                }
            }
        }
    }
}
