using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：ファイル時刻の条件指定方法
    //=========================================================================================
    public class DateTimeType {
        // 文字列表現からシンボルへのMap
        private static Dictionary<string, DateTimeType> m_nameToSymbol = new Dictionary<string, DateTimeType>();

        public static readonly DateTimeType None                   = new DateTimeType("None",                   TypeGroup.None);        // 指定しない
        public static readonly DateTimeType DateXxxStart           = new DateTimeType("DateXxxStart",           TypeGroup.Date);        // 指定日時以前で転送
        public static readonly DateTimeType DateEndXxx             = new DateTimeType("DateEndXxx",             TypeGroup.Date);        // 指定日時以降で転送
        public static readonly DateTimeType DateStartXxxEnd        = new DateTimeType("DateStartXxxEnd",        TypeGroup.Date);        // 指定日時の期間の間
        public static readonly DateTimeType DateXxxStartEndXxx     = new DateTimeType("DateXxxStartEndXxx",     TypeGroup.Date);        // 指定日時の期間以外
        public static readonly DateTimeType DateXxx                = new DateTimeType("DateXxx",                TypeGroup.Date);        // 指定日時
        public static readonly DateTimeType RelativeXxxStart       = new DateTimeType("RelativeXxxStart",       TypeGroup.Relative);    // 指定相対日時以前で転送
        public static readonly DateTimeType RelativeEndXxx         = new DateTimeType("RelativeEndXxx",         TypeGroup.Relative);    // 指定相対日時以降で転送
        public static readonly DateTimeType RelativeStartXxxEnd    = new DateTimeType("RelativeStartXxxEnd",    TypeGroup.Relative);    // 指定相対日時の期間の間
        public static readonly DateTimeType RelativeXxxStartEndXxx = new DateTimeType("RelativeXxxStartEndXxx", TypeGroup.Relative);    // 指定相対日時の期間以外
        public static readonly DateTimeType RelativeXxx            = new DateTimeType("RelativeXxx",            TypeGroup.Relative);    // 指定相対日時
        public static readonly DateTimeType TimeXxxStart           = new DateTimeType("TimeXxxStart",           TypeGroup.Time);        // 指定時刻以前で転送
        public static readonly DateTimeType TimeEndXxx             = new DateTimeType("TimeEndXxx",             TypeGroup.Time);        // 指定時刻以降で転送
        public static readonly DateTimeType TimeStartXxxEnd        = new DateTimeType("TimeStartXxxEnd",        TypeGroup.Time);        // 指定時刻の期間の間
        public static readonly DateTimeType TimeXxxStartEndXxx     = new DateTimeType("TimeXxxStartEndXxx",     TypeGroup.Time);        // 指定時刻の期間以外
        public static readonly DateTimeType TimeXxx                = new DateTimeType("TimeXxx",                TypeGroup.Time);        // 指定時刻

        // 文字列表現
        private string m_stringName;
        
        // 指定方法のグループ
        private TypeGroup m_group;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]stringName   文字列表現
        // 　　　　[in]group        指定方法のグループ
        // 戻り値：なし
        //=========================================================================================
        public DateTimeType(string stringName, TypeGroup group) {
            m_stringName = stringName;
            m_group = group;
            m_nameToSymbol.Add(stringName, this);
        }
        
        //=========================================================================================
        // 機　能：文字列をシンボルに変換する
        // 引　数：[in]stringName   文字列表現
        // 戻り値：シンボル（対応するシンボルがないときnull）
        //=========================================================================================
        public static DateTimeType FromString(string stringName) {
            if (m_nameToSymbol.ContainsKey(stringName)) {
                return m_nameToSymbol[stringName];
            } else {
                return null;
            }
        }

        //=========================================================================================
        // プロパティ：文字列表現
        //=========================================================================================
        public string StringName {
            get {
                return m_stringName;
            }
        }

        //=========================================================================================
        // プロパティ：指定方法のグループ
        //=========================================================================================
        public TypeGroup Group {
            get {
                return m_group;
            }
        }

        //=========================================================================================
        // 列挙子：指定方法のグループ
        //=========================================================================================
        public enum TypeGroup {
            None,               // 指定なし
            Date,               // 日付指定
            Relative,           // 相対日付指定
            Time,               // 時刻指定
        }
    }
}
