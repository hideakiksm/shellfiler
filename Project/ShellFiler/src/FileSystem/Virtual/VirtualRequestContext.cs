using System;
using System.Collections.Generic;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Archive;
using ShellFiler.FileSystem.Virtual;
using ShellFiler.FileTask;
using ShellFiler.Virtual;

namespace ShellFiler.FileSystem.Virtual {

    //=========================================================================================
    // クラス：仮想フォルダのリクエストのコンテキスト情報
    //=========================================================================================
    public class VirtualRequestContext {
        // 仮想フォルダの展開ダイアログのコールバック（展開時以外はnull）
        private BackgroundWaitCallback m_virtualExtractDialogCallback;

        // 仮想フォルダ内コンテンツをファイルコピー転送元にするとき、その展開用インターフェイス（展開時以外はnull）
        // m_fileCopySrcArchiveとm_fileCopySrcMasterは同時にnullになるか、null以外になるかのどちらか
        private IArchiveVirtualExtract m_archiveExtract = null;

        // 仮想フォルダ内コンテンツをファイルコピー転送元にするとき、アーカイブ内のファイル情報（展開時以外はnull）
        private VirtualFileCopySrcMaster m_fileCopySrcMaster = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public VirtualRequestContext() {
        }

        //=========================================================================================
        // 機　能：コンテキスト情報を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            if (m_archiveExtract != null) {
                m_archiveExtract.Dispose();
                m_archiveExtract = null;
            }
        }

        //=========================================================================================
        // 機　能：ファイル転送でのコンテキスト情報を返す
        // 引　数：[out]archiveExtract     展開用インターフェイス（まだアーカイブが開かれていないときはnull）
        // 　　　　[out]fileCopySrcMaster  アーカイブ内のファイル情報（まだアーカイブが開かれていないときはnull）
        // 戻り値：なし
        // メ　モ：片方だけnullになることはない
        //=========================================================================================
        public void GetVirtualFileCopySrcContext(out IArchiveVirtualExtract archiveExtract, out VirtualFileCopySrcMaster fileCopySrcMaster) {
            archiveExtract = m_archiveExtract;
            fileCopySrcMaster = m_fileCopySrcMaster;
        }

        //=========================================================================================
        // 機　能：ファイル転送でのコンテキスト情報を設定する
        // 引　数：[in]archiveExtract     展開用インターフェイス
        // 　　　　[in]fileCopySrcMaster  アーカイブ内のファイル情報
        // 戻り値：なし
        //=========================================================================================
        public void SetVirtualFileCopySrcContext(IArchiveVirtualExtract archiveExtract, VirtualFileCopySrcMaster fileCopySrcMaster) {
            m_archiveExtract = archiveExtract;
            m_fileCopySrcMaster = fileCopySrcMaster;
        }

        //=========================================================================================
        // プロパティ：仮想フォルダの展開ダイアログのコールバック（展開時以外はnull）
        //=========================================================================================
        public BackgroundWaitCallback VirtualExtractDialogCallback {
            get {
                return m_virtualExtractDialogCallback;
            }
            set {
                m_virtualExtractDialogCallback = value;
            }
        }

        //=========================================================================================
        // プロパティ：仮想フォルダ内コンテンツを展開するとき、その展開用インターフェイス（展開時以外はnull）
        //=========================================================================================
        public IArchiveVirtualExtract CurrentArchiveExtract {
            get {
                return m_archiveExtract;
            }
            set {
                m_archiveExtract = value;
            }
        }
    }
}
