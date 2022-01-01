using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileSystem.Windows;
using ShellFiler.FileSystem.SFTP;
using ShellFiler.UI.Dialog;
using ShellFiler.Util;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // クラス：連番指定時の情報
    //=========================================================================================
    public class RenameNumberingInfo : ICloneable {
        // フォーマット指定（「?」の箇所に連番を埋め込み）
        private string m_fileNameFormatter;

        // 桁数
        private int m_width;

        // 基数の種類
        private NumericRadix m_radix;
        
        // 開始番号
        private int m_startNumber;

        // 増分
        private int m_increaseNumber;
        
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public RenameNumberingInfo() {
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]formatter      フォーマット指定（「?」の箇所に連番を埋め込み）
        //         [in]width          桁数
        //         [in]radix          基数の種類
        //         [in]startNumber    開始番号
        //         [in]increaseNumber 増分
        // 戻り値：なし
        //=========================================================================================
        public RenameNumberingInfo(string formatter, int width, NumericRadix radix, int startNumber, int increaseNumber) {
            m_fileNameFormatter = formatter;
            m_width = width;
            m_radix = radix;
            m_startNumber = startNumber;
            m_increaseNumber = increaseNumber;
        }

        //=========================================================================================
        // 機　能：連番UIでのデフォルト設定を返す
        // 引　数：なし
        // 戻り値：連番UIでのデフォルト設定
        //=========================================================================================
        public static RenameNumberingInfo DefaultRenameSequenceUI() {
            return new RenameNumberingInfo("File?", 4, NumericRadix.Radix10, 1, 1);
        }

        //=========================================================================================
        // 機　能：分割UIでのデフォルト設定を返す
        // 引　数：なし
        // 戻り値：分割UIでのデフォルト設定
        //=========================================================================================
        public static RenameNumberingInfo DefaultSplitUI() {
            return new RenameNumberingInfo("_?", 4, NumericRadix.Radix10, 1, 1);
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            return MemberwiseClone();
        }

        //=========================================================================================
        // 機　能：連番部分の文字列を作成する
        // 引　数：[in]numberingInfo  変更ルール
        // 　　　　[in]modifyCtx      名前変更のコンテキスト情報
        // 戻り値：新しいファイル名
        //=========================================================================================
        public static string CreateSequenceString(RenameNumberingInfo numberingInfo, ModifyFileInfoContext modifyCtx) {
            // 新しい連番を発行
            long sequence = modifyCtx.SequenceNumber;
            if (sequence == -1) {
                sequence = numberingInfo.StartNumber;
            } else {
                sequence += numberingInfo.IncreaseNumber;
            }
            modifyCtx.SequenceNumber = sequence;

            // 連番を文字列化
            string strSequence = numberingInfo.Radix.ConvertLong(sequence);
            StringBuilder sbSequence = new StringBuilder();
            for (int i = strSequence.Length; i < numberingInfo.Width; i++) {
                sbSequence.Append('0');
            }
            sbSequence.Append(strSequence);

            // 連番を作成
            StringBuilder result = new StringBuilder();
            char[] chFormat = numberingInfo.FileNameFormatter.ToCharArray();
            for (int i = 0; i < chFormat.Length; i++) {
                if (chFormat[i] == '?') {
                    result.Append(sbSequence);
                } else {
                    result.Append(chFormat[i]);
                }
            }
            return result.ToString();
        }

        //=========================================================================================
        // 機　能：ファイル名主部に連番部分を連結する
        // 引　数：[in]orgFileName     元のファイル名
        // 　　　　[in]numberingInfo   変更ルール
        // 　　　　[in]modifyCtx       名前変更のコンテキスト情報
        // 戻り値：新しいファイル名
        //=========================================================================================
        public static string CreateSequenceFileName(string orgFileName, RenameNumberingInfo numberingInfo, ModifyFileInfoContext modifyCtx) {
            string seqPart = RenameNumberingInfo.CreateSequenceString(numberingInfo, modifyCtx);
            int extPos = GenericFileStringUtils.GetExtPosition(orgFileName);
            string fileName;
            if (extPos == -1) {
                fileName = orgFileName + seqPart;
            } else {
                fileName = orgFileName.Substring(0, extPos) + seqPart + orgFileName.Substring(extPos);
            }
            return fileName;
        }

        //=========================================================================================
        // プロパティ：フォーマット指定（「?」の箇所に連番を埋め込み）
        //=========================================================================================
        public string FileNameFormatter {
            get {
                return m_fileNameFormatter;
            }
            set {
                m_fileNameFormatter = value;
            }
        }

        //=========================================================================================
        // プロパティ：桁数
        //=========================================================================================
        public int Width {
            get {
                return m_width;
            }
            set {
                m_width = value;
            }
        }

        //=========================================================================================
        // プロパティ：基数の種類
        //=========================================================================================
        public NumericRadix Radix {
            get {
                return m_radix;
            }
            set {
                m_radix = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：開始番号
        //=========================================================================================
        public int StartNumber {
            get {
                return m_startNumber;
            }
            set {
                m_startNumber = value;
            }
        }

        //=========================================================================================
        // プロパティ：増分
        //=========================================================================================
        public int IncreaseNumber {
            get {
                return m_increaseNumber;
            }
            set {
                m_increaseNumber = value;
            }
        }
    }
}
