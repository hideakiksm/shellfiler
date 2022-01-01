using System;
using System.Collections.Generic;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document;

namespace ShellFiler.Archive {

    //=========================================================================================
    // 列挙子：圧縮ファイルのフォーマット形式
    //=========================================================================================
    public class ArchiveType {
        public static readonly ArchiveType Zip      = new ArchiveType(".zip");          // .zip
        public static readonly ArchiveType SevenZip = new ArchiveType(".7z");           // .7z
        public static readonly ArchiveType TarGz    = new ArchiveType(".tar.gz");       // .tar.gz
        public static readonly ArchiveType TarBz2   = new ArchiveType(".tar.bz2");      // .tar.bz2
        public static readonly ArchiveType Tar      = new ArchiveType(".tar");          // .tar

        // 「.」から始まる拡張子
        private string m_extension;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]extension 「.」から始まる拡張子
        // 戻り値：なし
        //=========================================================================================
        public ArchiveType(string extension) {
            m_extension = extension;
        }

        //=========================================================================================
        // 機　能：圧縮ファイルのフォーマット形式をサポートされている形式にあわせて修正する
        // 引　数：[in]type      フォーマット形式
        // 　　　　[in]supported サポートされている圧縮形式（nullのとき全種類をサポート）
        // 戻り値：修正済みのフォーマット形式
        //=========================================================================================
        public static ArchiveType ModifyArchiveType(ArchiveType type, List<ArchiveType> supported) {
            if (supported == null) {
                return type;
            } else {
                if (supported.Contains(type)) {
                    return type;
                } else {
                    return supported[0];
                }
            }
        }

        //=========================================================================================
        // プロパティ：「.」から始まる拡張子
        //=========================================================================================
        public string Extension {
            get {
                return m_extension;
            }
        }
    }
}
