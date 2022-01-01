using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Util;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：取得済みアイコン1個分の情報
    //=========================================================================================
    public class FileIcon {
        // アイコンのID
        private FileIconID m_fileIconId;

        // アイコンの画像
        private Bitmap m_iconImage;

        // ハッシュ値
        private long m_hashValue = 0;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]id   アイコンのID
        // 　　　　[in]bmp  アイコンのビットマップ表現（画像はアタッチする）
        // 戻り値：なし
        //=========================================================================================
        public FileIcon(FileIconID id, Bitmap bmp) {
            m_fileIconId = id;
            m_iconImage = bmp;
            m_hashValue = GraphicsUtils.GetImageDataHash(bmp);
        }
                
        //=========================================================================================
        // 機　能：アイコンを破棄する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Dispose() {
            m_iconImage.Dispose();
            m_iconImage = null;
        }

        //=========================================================================================
        // 機　能：同じデータかどうかを返す（アイコンのIDは無視）
        // 引　数：なし
        // 戻り値：同じデータのときtrue
        //=========================================================================================
        public bool EqualsIconImage(FileIcon other) {
            FileIcon otherIcon = (FileIcon)other;
            if (m_hashValue != otherIcon.m_hashValue) {
                return false;
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ハッシュ値を返す
        // 引　数：なし
        // 戻り値：ハッシュ値
        //=========================================================================================
        public override int GetHashCode() {
            return (int)m_hashValue;
        }

        //=========================================================================================
        // プロパティ：アイコンの画像
        //=========================================================================================
        public Bitmap IconImage {
            get {
                return m_iconImage;
            }
        }

        //=========================================================================================
        // プロパティ：ファイルアイコンのID
        //=========================================================================================
        public FileIconID FileIconId {
            get {
                return m_fileIconId;
            }
        }
    }
}
