using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ShellFiler.Document;
using ShellFiler.FileTask.FileFilter;
using ShellFiler.Util;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：ミラーコピーの設定
    //=========================================================================================
    public class MirrorCopyOption : ICloneable {
        // 上書き条件
        private MirrorCopyTransferMode m_transferMode = MirrorCopyTransferMode.OverwriteIfNewer;
        
        // すべての属性を設定するときtrue
        private AttributeSetMode m_attributeSetMode;
        
        // 除外するファイル
        private List<string> m_exceptFileList = new List<string>();

        // 削除オプション
        private DeleteFileOption m_deleteFileOption = new DeleteFileOption();

        // ごみ箱を使って削除するときtrue（m_moveTargetFolder!=nullのとき値は無効）
        private bool m_useRecycleBin = true;

        // 削除せず移動するとき、その移動先のフォルダ（移動しないときnull）
        private string m_moveTargetFolder = null;

        // 除外するファイルの正規表現（未設定のときnull）
        private List<Regex> m_exceptFileRegex = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MirrorCopyOption() {
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            MirrorCopyOption obj = new MirrorCopyOption();
            obj.m_transferMode = m_transferMode;
            obj.m_attributeSetMode = m_attributeSetMode;
            obj.m_exceptFileList = new List<string>();
            obj.m_exceptFileList.AddRange(m_exceptFileList);
            obj.m_deleteFileOption = (DeleteFileOption)(m_deleteFileOption.Clone());
            obj.m_useRecycleBin = m_useRecycleBin;
            obj.m_moveTargetFolder = m_moveTargetFolder;
            return obj;
        }

        //=========================================================================================
        // 機　能：指定されたファイルが対象となるかどうかを返す
        // 引　数：[in]filePath  チェックするファイルのパス
        // 戻り値：対象となるときtrue
        //=========================================================================================
        public bool CheckTargetFile(string filePath) {
            // 未設定の場合は初期化
            if (m_exceptFileRegex == null) {
                m_exceptFileRegex = new List<Regex>();
                for (int i = 0; i < m_exceptFileList.Count; i++) {
                    if (m_exceptFileList[i] != "") {
                        Regex regex = StringUtils.ConvertWildcardToRegex(m_exceptFileList[i]);
                        m_exceptFileRegex.Add(regex);
                    }
                }
            }

            // 評価
            for (int i = 0; i < m_exceptFileRegex.Count; i++) {
                if (m_exceptFileRegex[i].IsMatch(filePath)) {
                    return false;
                }
            }
            return true;
        }

        //=========================================================================================
        // プロパティ：上書き条件
        //=========================================================================================
        public MirrorCopyTransferMode TransferMode {
            get {
                return m_transferMode;
            }
            set {
                m_transferMode = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：すべての属性を設定するときtrue
        //=========================================================================================
        public AttributeSetMode AttributeSetMode {
            get {
                return m_attributeSetMode;
            }
            set {
                m_attributeSetMode = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：除外するファイル
        //=========================================================================================
        public List<string> ExceptFileList {
            get {
                return m_exceptFileList;
            }
            set {
                m_exceptFileList = value;
                m_exceptFileRegex = null;
            }
        }
        
        //=========================================================================================
        // プロパティ：削除オプション
        //=========================================================================================
        public DeleteFileOption DeleteFileOption {
            get {
                return m_deleteFileOption;
            }
            set {
                m_deleteFileOption = value;
            }
        }
        
        //=========================================================================================
        // プロパティ：ごみ箱を使って削除するときtrue（m_moveTargetFolder!=nullのとき値は無効）
        //=========================================================================================
        public bool UseRecycleBin {
            get {
                return m_useRecycleBin;
            }
            set {
                m_useRecycleBin = value;
            }
        }

        //=========================================================================================
        // 列挙子：ファイルの転送モード
        //=========================================================================================
        public enum MirrorCopyTransferMode {
            // 強制的に上書き
            ForceOverwrite,
            // 自分が新しければ上書き
            OverwriteIfNewer,
            // 転送しない
            NotOverwrite,
            // サイズまたは日付が違うとき上書き
            DifferenceAttribute,
        }
    }
}
