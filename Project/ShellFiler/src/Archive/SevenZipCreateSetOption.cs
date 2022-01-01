using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Nomad.Archive.SevenZip;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Archive {

    //=========================================================================================
    // クラス：7zへの圧縮オプションを設定する処理をまとめるための実装クラス
    //=========================================================================================
    public class SevenZipCreateSetOption {

        //=========================================================================================
        // 機　能：圧縮について、オプションを設定する
        // 引　数：[in]dllPath         DLLのパス名
        // 　　　　[in]archiveSetting  作成するアーカイブの設定
        // 　　　　[in]arcFileName     作成するアーカイブファイル名
        // 戻り値：作成用インターフェース
        //=========================================================================================
        public static void SetArchiveOption(IOutArchive archive, ArchiveParameter parameter) {
            if (parameter.ArchiveType == ArchiveType.Zip) {
                // .zip
                // M:圧縮方式 COPY/DEFLATE/DEFLATE64/BZIP2/LZMA/PPMD
                // X:圧縮レベル 0～9
                // EM:暗号化方式  AES128/AES192/AES256/ZIPCRYPTO
                ISetProperties setProperties = (ISetProperties)archive;
                if (parameter.Local7zOption.EncryptionMethod != null) {
                    Win32VariantArray propValue = new Win32VariantArray(3);
                    try {
                        propValue.SetValue(0, (string)(parameter.Local7zOption.CompressionMethod));
                        propValue.SetValue(1, (uint)(parameter.Local7zOption.CompressionLevel));
                        propValue.SetValue(2, (string)(parameter.Local7zOption.EncryptionMethod));
                        string[] propName = { "M", "X", "EM" };
                        setProperties.SetProperties(propName, propValue.VariantTop, propName.Length);
                    } finally {
                        propValue.Dispose();
                    }
                } else {
                    Win32VariantArray propValue = new Win32VariantArray(2);
                    try {
                        uint level = (uint)(parameter.Local7zOption.CompressionLevel);
                        propValue.SetValue(0, (string)(parameter.Local7zOption.CompressionMethod));
                        propValue.SetValue(1, level);
                        string[] propName = { "M", "X" };
                        setProperties.SetProperties(propName, propValue.VariantTop, propName.Length);
                    } finally {
                        propValue.Dispose();
                    }
                }
            } else if (parameter.ArchiveType == ArchiveType.SevenZip) {
                // .7z
                // X:圧縮レベル 0～9
                ISetProperties setProperties = (ISetProperties)archive;
                Win32VariantArray propValue = new Win32VariantArray(1);
                try {
                    propValue.SetValue(0, (uint)parameter.Local7zOption.CompressionLevel);
                    string[] propName = { "X" };
                    setProperties.SetProperties(propName, propValue.VariantTop, propName.Length);
                } finally {
                    propValue.Dispose();
                }
            }
        }

        //=========================================================================================
        // 機　能：tar.gzとtar.bz2の外側の圧縮について、オプションを設定する
        // 引　数：[in]dllPath         DLLのパス名
        // 　　　　[in]archiveSetting  作成するアーカイブの設定
        // 　　　　[in]arcFileName     作成するアーカイブファイル名
        // 戻り値：作成用インターフェース
        //=========================================================================================
        public static void SetOuterArchiveOption(IOutArchive archive, ArchiveParameter parameter) {
            if (parameter.ArchiveType == ArchiveType.TarGz || parameter.ArchiveType == ArchiveType.TarBz2) {
                // .gz または .bz2
                // X:圧縮レベル 0～9
                ISetProperties setProperties = (ISetProperties)archive;
                Win32VariantArray propValue = new Win32VariantArray(1);
                try {
                    propValue.SetValue(0, (uint)parameter.Local7zOption.CompressionLevel);
                    string[] propName = { "X" };
                    setProperties.SetProperties(propName, propValue.VariantTop, propName.Length);
                } finally {
                    propValue.Dispose();
                }
            }
        }
    }
}
