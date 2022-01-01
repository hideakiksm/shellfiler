using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Terminal.TerminalSession.CommandEmulator;

namespace ShellFiler.Document.OSSpec {

    //=========================================================================================
    // クラス：実行結果の期待値のトークンの種類
    //=========================================================================================
    public class OSSpecTokenType {
        public delegate IOSSpecTokenParser CreateParserDelegate();
        public static readonly OSSpecTokenType SpaceRepeat      = new OSSpecTokenType("Sp+",           delegate() { return new OSSpecParserSpN(); });             // [Sp+]           1個以上のスペースまたはタブ（戻り=なし）
        public static readonly OSSpecTokenType Space            = new OSSpecTokenType("Sp",            delegate() { return new OSSpecParserSp(); });              // [Sp]            1個のスペースまたはタブ（戻り=なし）
        public static readonly OSSpecTokenType Str              = new OSSpecTokenType("Str",           delegate() { return new OSSpecParserString(); });          // [Str]           空白とタブ以外の文字列の並び
        public static readonly OSSpecTokenType StrAll           = new OSSpecTokenType("StrAll",        delegate() { return new OSSpecParserStringAll(); });       // [StrAll]        行末までの文字列の並び
        public static readonly OSSpecTokenType UInt             = new OSSpecTokenType("UInt",          delegate() { return new OSSpecParserUInt(); });            // [UInt]          数値0～9の並び（戻り=int:値）
        public static readonly OSSpecTokenType ULong            = new OSSpecTokenType("ULong",         delegate() { return new OSSpecParserULong(); });           // [ULong]         数値0～9の並び（戻り=int:値）
        public static readonly OSSpecTokenType Specify          = new OSSpecTokenType("",              delegate() { return new OSSpecParserSpecify(); });         // (その他)        文字列の定義（戻り=なし）
        public static readonly OSSpecTokenType LsAttr           = new OSSpecTokenType("LsAttr",        delegate() { return new OSSpecParserLsAttr(); });          // [LsAttr]        属性の出力 drwxrwxrwx+（戻り=FileAttributeLinux:属性）
        public static readonly OSSpecTokenType LsLink           = new OSSpecTokenType("LsLink",        delegate() { return new OSSpecParserUInt(); });            // [LsLink]        ハードリンクの数（戻り=int:値）
        public static readonly OSSpecTokenType LsOwner          = new OSSpecTokenType("LsOwner",       delegate() { return new OSSpecParserString(); });          // [LsOwner]       オーナーユーザー名（戻り=string:値）
        public static readonly OSSpecTokenType LsGroup          = new OSSpecTokenType("LsGroup",       delegate() { return new OSSpecParserString(); });          // [LsGroup]       オーナーのグループ名（戻り=string:値）
        public static readonly OSSpecTokenType LsSize           = new OSSpecTokenType("LsSize",        delegate() { return new OSSpecParserULong(); });           // [LsSize]        サイズ（戻り=long:値）
        public static readonly OSSpecTokenType LsUpdTimeFull    = new OSSpecTokenType("LsUpdTimeFull", delegate() { return new OSSpecParserLsTimeFullIso(); });   // [LsUpdTimeFull] full-iso形式での更新時刻     YYYY-MM-DD HH:MM:SS.XXXXXXXXX +9999
        public static readonly OSSpecTokenType LsAccTimeFull    = new OSSpecTokenType("LsAccTimeFull", delegate() { return new OSSpecParserLsTimeFullIso(); });   // [LsAccTimeFull] full-iso形式でのアクセス時刻 YYYY-MM-DD HH:MM:SS.XXXXXXXXX +9999
        public static readonly OSSpecTokenType LsCreTimeFull    = new OSSpecTokenType("LsCreTimeFull", delegate() { return new OSSpecParserLsTimeFullIso(); });   // [LsCreTimeFull] full-iso形式での作成時刻     YYYY-MM-DD HH:MM:SS.XXXXXXXXX +9999
        public static readonly OSSpecTokenType LsFileName       = new OSSpecTokenType("LsFileName",    delegate() { return new OSSpecParserQuotedPath(); });      // [LsFileName]    ファイル名
        public static readonly OSSpecTokenType LsLnPath         = new OSSpecTokenType("LsLnPath",      delegate() { return new OSSpecParserQuotedPath(); });      // [LsLnPath]      シンボリックリンクのファイル名
        public static readonly OSSpecTokenType StatNotFound     = new OSSpecTokenType("StatNotFound",  delegate() { return new OSSpecParserStringAll(); });       // [StatNotFound]  ファイル情報の取得対象がない
        public static readonly OSSpecTokenType DfUsed           = new OSSpecTokenType("DfUsed",        delegate() { return new OSSpecParserULong(); });           // [DfUsed]        使用量[KB]
        public static readonly OSSpecTokenType DfFree           = new OSSpecTokenType("DfFree",        delegate() { return new OSSpecParserULong(); });           // [DfFree]        残り使用可能容量[KB]
        public static readonly OSSpecTokenType DfUsedP          = new OSSpecTokenType("DfUsedP",       delegate() { return new OSSpecParserUIntPercent(); });     // [DfUsedP]       使用量%
        public static readonly OSSpecTokenType DuDirSize        = new OSSpecTokenType("DuDirSize",     delegate() { return new OSSpecParserULong(); });           // [DuDirSize]     フォルダ内ファイルサイズ[byte]
        public static readonly OSSpecTokenType DuDirPath        = new OSSpecTokenType("DuDirPath",     delegate() { return new OSSpecParserString(); });          // [DuDirPath]     フォルダパス
        public static readonly OSSpecTokenType CksumCRC         = new OSSpecTokenType("CksumCRC",      delegate() { return new OSSpecParserUInt32(); });          // [CksumCRC]      cksumコマンドのCRC32値（ファイル本体とファイルサイズのCRC32取得結果）
        public static readonly OSSpecTokenType CksumSize        = new OSSpecTokenType("CksumSize",     delegate() { return new OSSpecParserULong(); });           // [CksumSize]     ファイルサイズ
        public static readonly OSSpecTokenType PwdCurrent       = new OSSpecTokenType("PwdCurrent",    delegate() { return new OSSpecParserStringAll(); });       // [PwdCurrent]    カレントディレクトリ
        
        // 定義中でのトークン
        private string m_defFileToken;

        // 構文解析処理作成メソッドのdelegate
        private CreateParserDelegate m_createParserDelegate;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]defFileToken  定義中でのトークン
        // 　　　　[in]createParser  構文解析処理作成メソッドのdelegate
        // 戻り値：なし
        //=========================================================================================
        private OSSpecTokenType(string defFileToken, CreateParserDelegate createParser) {
            m_defFileToken = defFileToken;
            m_createParserDelegate = createParser;
        }

        //=========================================================================================
        // プロパティ：定義中でのトークン
        //=========================================================================================
        public string DefFileToken {
            get {
                return m_defFileToken;
            }
        }

        //=========================================================================================
        // プロパティ：構文解析処理作成メソッドのdelegate
        //=========================================================================================
        public CreateParserDelegate CreateParser {
            get {
                return m_createParserDelegate;
            }
        }
    }
}
