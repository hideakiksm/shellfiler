using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;
using Nomad.Archive.SevenZip;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zによりファイルの展開を行うクラス
    //=========================================================================================
    public class SevenZipUtils {
        // ロックオブジェクト
        private static object s_lockSevenZip = new object();

        //=========================================================================================
        // 機　能：アーカイブファイル名の拡張子から圧縮形式を特定する
        // 引　数：[in]arcName    アーカイブファイル名
        // 　　　　[out]format    認識した形式を返す変数
        // 　　　　[out]tarArchie 実行後にTarアーカイブの処理が必要なときtrue
        // 戻り値：形式を認識できたときtrue
        //=========================================================================================
        public static bool GetFormatTypeFromExtension(string arcName, out KnownSevenZipFormat format, out bool tarArchive) {
            tarArchive = false;
            format = KnownSevenZipFormat.SevenZip;
            
            // 拡張子による判定
            string name = arcName.ToLower();
            if (name.EndsWith(".7z")) {
                format = KnownSevenZipFormat.SevenZip;
                return true;
            } else if (name.EndsWith(".tgz")) {
                format = KnownSevenZipFormat.GZip;
                tarArchive = true;
                return true;
            } else if (name.EndsWith(".tar.gz")) {
                format = KnownSevenZipFormat.GZip;
                tarArchive = true;
                return true;
            } else if (name.EndsWith(".gz")) {
                format = KnownSevenZipFormat.GZip;
                return true;
            } else if (name.EndsWith(".tar.bz2")) {
                format = KnownSevenZipFormat.BZip2;
                tarArchive = true;
                return true;
            } else if (name.EndsWith(".bz2")) {
                format = KnownSevenZipFormat.BZip2;
                return true;
            } else if (name.EndsWith(".tar")) {
                format = KnownSevenZipFormat.Tar;
                return true;
            } else if (name.EndsWith(".zip") || name.EndsWith(".jar") || name.EndsWith(".war")) {
                format = KnownSevenZipFormat.Zip;
                return true;
            } else if (name.EndsWith(".arj")) {
                format = KnownSevenZipFormat.Arj;
                return true;
            } else if (name.EndsWith(".cab")) {
                format = KnownSevenZipFormat.Cab;
                return true;
            } else if (name.EndsWith(".iso")) {
                format = KnownSevenZipFormat.Iso;
                return true;
            } else if (name.EndsWith(".lzh")) {
                format = KnownSevenZipFormat.Lzh;
                return true;
            } else if (name.EndsWith(".rar")) {
                format = KnownSevenZipFormat.Rar;
                return true;
            }
            // 以下はいったん非対応
            //    Chm,
            //    Compound,
            //    Cpio,
            //    Deb,
            //    Lzma,
            //    Nsis,
            //    Rpm,
            //    Split,
            //    Wim,
            //    Z,

            // 内容判別実行
            try {
                int readSize;
                byte[] buffer = new byte[7];
                FileStream stream = File.OpenRead(arcName);
                try {
                    readSize = stream.Read(buffer, 0, buffer.Length);
                } finally {
                    stream.Dispose();
                }
                char[] chBuffer = new char[readSize];
                for (int i = 0; i < readSize; i++) {
                    chBuffer[i] = (char)(buffer[i]);
                }
                if (readSize > 2 && chBuffer[0] == 'P' && chBuffer[1] == 'K') {
                    // PK → ZIP
                    format = KnownSevenZipFormat.Zip;
                    return true;
                } else if (readSize >= 4 && chBuffer[0] == 'M' && chBuffer[1] == 'S' && chBuffer[2] == 'C' && chBuffer[3] == 'F') {
                    // MSCF → CAB
                    format = KnownSevenZipFormat.Cab;
                    return true;
                } else if (readSize >= 7 && chBuffer[2] == '-' && chBuffer[3] == 'l' && chBuffer[4] == 'd' && chBuffer[6] == '-') {
                    // ??-ld?- → LHA
                    format = KnownSevenZipFormat.Lzh;
                    return true;
                }
            } catch (Exception) {
                return false;
            }
            return false;
        }
        
        //=========================================================================================
        // 機　能：アーカイブの種類から圧縮形式を特定する
        // 引　数：[in]type          アーカイブの種類
        // 　　　　[out]firstFormat  1段階目の圧縮形式
        // 　　　　[out]secondFormat 2段階目の圧縮形式（使用しないときは1段階目と同じ）
        // 戻り値：なし
        //=========================================================================================
        public static void GetFormatTypeFromArchiveType(ArchiveType type, out KnownSevenZipFormat firstFormat, out KnownSevenZipFormat secondFormat) {
            if (type == ArchiveType.Zip) {
                firstFormat = KnownSevenZipFormat.Zip;
                secondFormat = KnownSevenZipFormat.Zip;
            } else if (type == ArchiveType.SevenZip) {
                firstFormat = KnownSevenZipFormat.SevenZip;
                secondFormat = KnownSevenZipFormat.SevenZip;
            } else if (type == ArchiveType.TarGz) {
                firstFormat = KnownSevenZipFormat.Tar;
                secondFormat = KnownSevenZipFormat.GZip;
            } else if (type == ArchiveType.TarBz2) {
                firstFormat = KnownSevenZipFormat.Tar;
                secondFormat = KnownSevenZipFormat.BZip2;
            } else {
                firstFormat = KnownSevenZipFormat.Tar;
                secondFormat = KnownSevenZipFormat.Tar;
            }
        }

        //=========================================================================================
        // 機　能：C#形式の時刻プロパティをVARIANTに変換する
        // 引　数：[in]time  設定する時刻
        // 　　　　[in]value Variantを書き込む変数
        // 戻り値：なし
        //=========================================================================================
        public static void CsTimeToVariantTime(DateTime time, IntPtr value) {
            Marshal.GetNativeVariantForObject(time.ToFileTime(), value);
            Marshal.WriteInt16(value, (short)VarEnum.VT_FILETIME);
        }

        //=========================================================================================
        // 機　能：圧縮フォーマットに対応したClassIdを取得する
        // 引　数：[in]format  圧縮フォーマット
        // 戻り値：ClassId
        //=========================================================================================
        public static Guid GetClassIdFromKnownFormat(KnownSevenZipFormat format) {
            // 複数スレッドから同時にアクセスがあったときに直接呼び出すと、Mapに重複エントリーができて例外となる
            lock (s_lockSevenZip) {
                return SevenZipFormat.GetClassIdFromKnownFormat(format);
            }
        }

        //=========================================================================================
        // プロパティ：サポートしている拡張子の種類
        //=========================================================================================
        public static string[] SupportedExtList {
            get {
                string[] extList = {
                    ".7z",
                    ".tar.gz",
                    ".tar.bz2",
                    ".tar",
                    ".zip",
                    ".jar",
                    ".war",
                    ".arj",
                    ".cab",
                    ".iso",
                    ".lzh",
                    ".rar",
                };
                return extList;
            }
        }
    }
}
