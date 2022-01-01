using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI.FileList;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：タブページの情報
    //=========================================================================================
    public class TabPageInfo : ICloneable {
        // 左ウィンドウのファイル一覧
        private UIFileList m_leftFileList;

        // 右ウィンドウのファイル一覧
        private UIFileList m_rightFileList;

        // 左ウィンドウのウィンドウ保存情報
        private IFileListViewState m_leftViewState;

        // 右ウィンドウのウィンドウ保存情報
        private IFileListViewState m_rightViewState;

        // カーソルが左にあるときtrue
        private bool m_isCursorLeft;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]leftFileList    左ウィンドウのファイル一覧
        // 　　　　[in]rightFileList   右ウィンドウのファイル一覧
        // 　　　　[in]leftViewState   左ウィンドウのウィンドウ保存情報
        // 　　　　[in]rightViewState  右ウィンドウのウィンドウ保存情報
        // 　　　　[in]isCursorLeft    カーソルが左にあるときtrue
        // 戻り値：なし
        //=========================================================================================
        public TabPageInfo(UIFileList leftFileList, UIFileList rightFileList, IFileListViewState leftViewState, IFileListViewState rightViewState, bool isCursorLeft) {
            m_leftFileList = leftFileList;
            m_rightFileList = rightFileList;
            m_leftViewState = leftViewState;
            m_rightViewState = rightViewState;
            m_isCursorLeft = isCursorLeft;
        }

        //=========================================================================================
        // 機　能：後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            m_leftFileList.Dispose();
            m_rightFileList.Dispose();
        }

        //=========================================================================================
        // 機　能：ファイル一覧を返す
        // 引　数：[in]isLeft  左ウィンドウの一覧を返すときtrue
        // 戻り値：ファイル一覧
        //=========================================================================================
        public UIFileList GetFileList(bool isLeft) {
            if (isLeft) {
                return m_leftFileList;
            } else {
                return m_rightFileList;
            }
        }

        //=========================================================================================
        // 機　能：ファイル一覧のUI状態を返す
        // 引　数：[in]isLeft  左ウィンドウの状態を返すときtrue
        // 戻り値：ファイル一覧のUI状態
        //=========================================================================================
        public IFileListViewState GetFileListViewState(bool isLeft) {
            if (isLeft) {
                return m_leftViewState;
            } else {
                return m_rightViewState;
            }
        }

        //=========================================================================================
        // 機　能：ファイル一覧のUI状態を設定する
        // 引　数：[in]isLeft  左ウィンドウの状態を返すときtrue
        // 　　　　[in]statte  ファイル一覧のUI状態
        // 戻り値：なし
        //=========================================================================================
        public void SetFileListViewState(bool isLeft, IFileListViewState state) {
            if (isLeft) {
                m_leftViewState = state;
            } else {
                m_rightViewState = state;
            }
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            UIFileList leftFileList = m_leftFileList.CloneForTabCopy();
            UIFileList rightFileList = m_rightFileList.CloneForTabCopy();
            leftFileList.OppositeFileList = rightFileList;
            rightFileList.OppositeFileList = leftFileList;
            IFileListViewState leftViewState = (IFileListViewState)m_leftViewState.Clone();
            IFileListViewState rightViewState = (IFileListViewState)m_rightViewState.Clone();

            TabPageInfo tabPageInfo = new TabPageInfo(leftFileList, rightFileList, leftViewState, rightViewState, m_isCursorLeft);
            return tabPageInfo;
        }

        //=========================================================================================
        // プロパティ：左ウィンドウのファイル一覧
        //=========================================================================================
        public UIFileList LeftFileList {
            get {
                return m_leftFileList;
            }
        }

        //=========================================================================================
        // プロパティ：右ウィンドウのファイル一覧
        //=========================================================================================
        public UIFileList RightFileList {
            get {
                return m_rightFileList;
            }
        }

        //=========================================================================================
        // プロパティ：左ウィンドウのウィンドウ保存情報
        //=========================================================================================
        public IFileListViewState LeftUIState {
            get {
                return m_leftViewState;
            }
        }

        //=========================================================================================
        // プロパティ：右ウィンドウのウィンドウ保存情報
        //=========================================================================================
        public IFileListViewState RightUIState {
            get {
                return m_rightViewState;
            }
        }

        //=========================================================================================
        // プロパティ：カーソルが左にあるときtrue
        //=========================================================================================
        public bool IsCursorLeft {
            get {
                return m_isCursorLeft;
            }
            set {
                m_isCursorLeft = value;
            }
        }
    }
}
