using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;
using ShellFiler.Locale;
using ShellFiler.Util;

namespace ShellFiler.FileTask.Condition {

    //=========================================================================================
    // クラス：日時の条件
    //=========================================================================================
    public class DateTimeCondition {
        // 相対日付の最大値
        public const int MAX_RELATIVE_VALUE = 999;

        // 日付の指定条件
        private DateTimeType m_timeType = DateTimeType.None;

        // 開始日時（指定しないときMinValue）
        private DateTime m_dateStart = DateTime.MinValue;

        // 開始相対日付 n日前（指定しないとき-1）
        private int m_relativeStart = -1;

        // 開始時刻（指定しないときnull）
        private TimeInfo m_timeStart = null;

        // 開始時刻そのものを含むときtrue（指定しないときfalse）
        private bool m_includeStart = false;

        // 終了日時（指定しないときMinValue）
        private DateTime m_dateEnd = DateTime.MinValue;

        // 終了相対日付 n日前（指定しないとき-1）
        private int m_relativeEnd = -1;

        // 終了時刻（指定しないときnull）
        private TimeInfo m_timeEnd = null;

        // 終了時刻そのものを含むときtrue（指定しないときfalse）
        private bool m_includeEnd = false;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DateTimeCondition() {
        }
        
        //=========================================================================================
        // 機　能：UI設定用のデフォルト値を設定する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void SetUIDefaultValue() {
            // 日付を初期化
            if (m_dateStart == DateTime.MinValue && m_dateEnd == DateTime.MinValue) {
                m_dateStart = ObjectUtils.SetDateHHMMSS(DateTime.Now, 120000);
                m_dateEnd = m_dateStart;
            } else if (m_dateStart == DateTime.MinValue) {
                m_dateStart = m_dateEnd;
            } else if (m_dateEnd == DateTime.MinValue) {
                m_dateEnd = m_dateStart;
            }

            // 日付を初期化
            if (m_relativeStart == -1 && m_relativeEnd == -1) {
                m_relativeStart = 3;
                m_relativeEnd = m_relativeStart;
            } else if (m_relativeStart == -1) {
                m_relativeStart = m_relativeEnd;
            } else if (m_relativeEnd == -1) {
                m_relativeEnd = m_relativeStart;
            }

            // 時刻を初期化
            if (m_timeStart == null && m_timeEnd == null) {
                m_timeStart = new TimeInfo(0, 0, 0);
                m_timeEnd = new TimeInfo(0, 0, 0);
            } else if (m_timeStart == null) {
                m_timeStart = (TimeInfo)(m_timeEnd.Clone());
            } else if (m_timeEnd == null) {
                m_timeEnd = (TimeInfo)(m_timeStart.Clone());
            }
        }
        
        //=========================================================================================
        // 機　能：UI設定用の値から不要部分を削除する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void CleanupField() {
            bool useDateStart, useRelativeStart, useTimeStart, useIncludeStart;
            bool useDateEnd, useRelativeEnd, useTimeEnd, useIncludeEnd;
            CheckMandatoryField(m_timeType, out useDateStart, out useRelativeStart, out useTimeStart, out useIncludeStart, out useDateEnd, out useRelativeEnd, out useTimeEnd, out useIncludeEnd);

            if (!useDateStart) {
                m_dateStart = DateTime.MinValue;
            }
            if (!useRelativeStart) {
                m_relativeStart = -1;
            }
            if (!useTimeStart) {
                m_timeStart = null;
            }
            if (!useIncludeStart) {
                m_includeStart = false;
            }
            if (!useDateEnd) {
                m_dateEnd = DateTime.MinValue;
            }
            if (!useRelativeEnd) {
                m_relativeEnd = -1;
            }
            if (!useTimeEnd) {
                m_timeEnd = null;
            }
            if (!useIncludeEnd) {
                m_includeEnd = false;
            }
        }

        //=========================================================================================
        // 機　能：クローンを作成する
        // 引　数：なし
        // 戻り値：作成したクローン
        //=========================================================================================
        public object Clone() {
            DateTimeCondition clone = new DateTimeCondition();
            clone.m_timeType = m_timeType;
            clone.m_dateStart = m_dateStart;
            clone.m_relativeStart = m_relativeStart;
            if (m_timeStart == null) {
                clone.m_timeStart = null;
            } else {
                clone.m_timeStart = (TimeInfo)(m_timeStart.Clone());
            }
            clone.m_includeStart = m_includeStart;
            clone.m_dateEnd = m_dateEnd;
            clone.m_relativeEnd = m_relativeEnd;
            if (m_timeEnd == null) {
                clone.m_timeEnd = null;
            } else {
                clone.m_timeEnd = (TimeInfo)(m_timeEnd.Clone());
            }
            clone.m_includeEnd = m_includeEnd;

            return clone;
        }

        //=========================================================================================
        // 機　能：設定値が同じかどうかを返す
        // 引　数：[in]obj1   比較対象1
        // 　　　　[in]obj2   比較対象2
        // 戻り値：設定値が同じときtrue
        //=========================================================================================
        public static bool EqualsConfig(DateTimeCondition obj1, DateTimeCondition obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            } else if (obj1 == null || obj2 == null) {
                return false;
            }

            if (obj1.m_timeType != obj2.m_timeType) {
                return false;
            }

            if (obj1.m_dateStart != obj2.m_dateStart) {
                return false;
            }
            if (obj1.m_relativeStart != obj2.m_relativeStart) {
                return false;
            }
            if (!TimeInfo.EqualsValue(obj1.m_timeStart, obj2.m_timeStart)) {
                return false;
            }
            if (obj1.m_includeStart != obj2.m_includeStart) {
                return false;
            }

            if (obj1.m_dateEnd != obj2.m_dateEnd) {
                return false;
            }
            if (obj1.m_relativeEnd != obj2.m_relativeEnd) {
                return false;
            }
            if (!TimeInfo.EqualsValue(obj1.m_timeEnd, obj2.m_timeEnd)) {
                return false;
            }
            if (obj1.m_includeEnd != obj2.m_includeEnd) {
                return false;
            }
            return true;
        }

        
        //=========================================================================================
        // 機　能：実際の時刻と、このオブジェクトの条件を比較する
        // 引　数：[in]date  比較対象の時刻
        // 戻り値：条件に一致するファイルのときtrue
        //=========================================================================================
        public bool CompareFile(DateTime date) {
            if (m_timeType == DateTimeType.None) {
                return true;
            } else if (m_timeType.Group == DateTimeType.TypeGroup.Time) {
                // 時刻指定
                int time = date.Hour * 10000 + date.Minute + date.Second;
                if (m_timeType == DateTimeType.TimeXxxStart) {
                    if (m_includeStart) {
                        return (time <= m_timeStart.HHMMSS);
                    } else {
                        return (time < m_timeStart.HHMMSS);
                    }
                } else if (m_timeType == DateTimeType.TimeEndXxx) {
                    if (m_includeEnd) {
                        return (m_timeEnd.HHMMSS <= time);
                    } else {
                        return (m_timeEnd.HHMMSS < time);
                    }
                } else if (m_timeType == DateTimeType.TimeStartXxxEnd) {
                    if (m_includeStart && m_includeEnd) {
                        return (m_timeStart.HHMMSS <= time && time <= m_timeEnd.HHMMSS);
                    } else if (m_includeStart) {
                        return (m_timeStart.HHMMSS <= time && time < m_timeEnd.HHMMSS);
                    } else if (m_includeEnd) {
                        return (m_timeStart.HHMMSS < time && time <= m_timeEnd.HHMMSS);
                    } else {
                        return (m_timeStart.HHMMSS < time && time < m_timeEnd.HHMMSS);
                    }
                } else if (m_timeType == DateTimeType.TimeXxxStartEndXxx) {
                    if (m_includeStart && m_includeEnd) {
                        return (time <= m_timeStart.HHMMSS && m_timeEnd.HHMMSS <= time);
                    } else if (m_includeStart) {
                        return (time <= m_timeStart.HHMMSS && m_timeEnd.HHMMSS < time);
                    } else if (m_includeEnd) {
                        return (time < m_timeStart.HHMMSS && m_timeEnd.HHMMSS <= time);
                    } else {
                        return (time < m_timeStart.HHMMSS && m_timeEnd.HHMMSS < time);
                    }
                } else if (m_timeType == DateTimeType.TimeXxx) {
                    return (m_timeStart.HHMMSS == time);
                } else {
                    Program.Abort("timeTypeの値が想定外です。");
                    return false;
                }
            } else {
                DateTime dateStart;
                DateTime dateEnd;
                if (m_timeType.Group == DateTimeType.TypeGroup.Date) {
                    dateStart = m_dateStart;
                    dateEnd = m_dateEnd;
                } else {
                    DateTime current = DateTime.Now;
                    if (m_relativeStart != -1) {
                        DateTime startRelative = current.AddDays(-m_relativeStart);
                        dateStart = ObjectUtils.SetDateHHMMSS(startRelative, m_timeStart.HHMMSS);
                    } else {
                        dateStart = current;            // 未使用
                    }
                    if (m_relativeEnd != -1) {
                        DateTime endRelative = current.AddDays(-m_relativeEnd);
                        dateEnd = ObjectUtils.SetDateHHMMSS(endRelative, m_timeEnd.HHMMSS);
                    } else {
                        dateEnd = current;              // 未使用
                    }
                }

                if (m_timeType == DateTimeType.DateXxxStart || m_timeType == DateTimeType.RelativeXxxStart) {
                    if (m_includeStart) {
                        return (date <= dateStart);
                    } else {
                        return (date < dateStart);
                    }
                } else if (m_timeType == DateTimeType.DateEndXxx || m_timeType == DateTimeType.RelativeEndXxx) {
                    if (m_includeEnd) {
                        return (dateEnd <= date);
                    } else {
                        return (dateEnd < date);
                    }
                } else if (m_timeType == DateTimeType.DateStartXxxEnd || m_timeType == DateTimeType.RelativeStartXxxEnd) {
                    if (m_includeStart && m_includeEnd) {
                        return (dateStart <= date && date <= dateEnd);
                    } else if (m_includeStart) {
                        return (dateStart <= date && date < dateEnd);
                    } else if (m_includeEnd) {
                        return (dateStart < date && date <= dateEnd);
                    } else {
                        return (dateStart < date && date < dateEnd);
                    }
                } else if (m_timeType == DateTimeType.DateXxxStartEndXxx || m_timeType == DateTimeType.RelativeXxxStartEndXxx) {
                    if (m_includeStart && m_includeEnd) {
                        return (date <= dateStart && dateEnd <= date);
                    } else if (m_includeStart) {
                        return (date <= dateStart && dateEnd < date);
                    } else if (m_includeEnd) {
                        return (date < dateStart && dateEnd <= date);
                    } else {
                        return (date < dateStart && dateEnd < date);
                    }
                } else if (m_timeType == DateTimeType.DateXxx || m_timeType == DateTimeType.RelativeXxx) {
                    return (dateStart.Ticks == date.Ticks);
                } else {
                    Program.Abort("timeTypeの値が想定外です。");
                    return false;
                }
            }
        }
        
        //=========================================================================================
        // 機　能：内容に矛盾がないかどうかを検証する
        // 引　数：なし
        // 戻り値：内容が正しいときtrue
        //=========================================================================================
        public bool Validate() {
            if (m_timeType == null) {
                return false;
            }
            bool useDateStart, useRelativeStart, useTimeStart, useIncludeStart;
            bool useDateEnd, useRelativeEnd, useTimeEnd, useIncludeEnd;
            CheckMandatoryField(m_timeType, out useDateStart, out useRelativeStart, out useTimeStart, out useIncludeStart, out useDateEnd, out useRelativeEnd, out useTimeEnd, out useIncludeEnd);

            if (useDateStart) {
                if (m_dateStart == DateTime.MinValue) {
                    return false;
                }
            } else {
                if (m_dateStart != DateTime.MinValue) {
                    return false;
                }
            }

            if (useRelativeStart) {
                if (m_relativeStart == -1 || m_relativeStart > MAX_RELATIVE_VALUE) {
                    return false;
                }
            } else {
                if (m_relativeStart != -1) {
                    return false;
                }
            }

            if (useTimeStart) {
                if (m_timeStart == null) {
                    return false;
                }
            } else {
                if (m_timeStart != null) {
                    return false;
                }
            }

            if (useIncludeStart) {
                ;
            } else {
                if (m_includeStart != false) {
                    return false;
                }
            }

            if (useDateEnd) {
                if (m_dateEnd == DateTime.MinValue) {
                    return false;
                }
            } else {
                if (m_dateEnd != DateTime.MinValue) {
                    return false;
                }
            }

            if (useRelativeEnd) {
                if (m_relativeEnd == -1 || m_relativeEnd > MAX_RELATIVE_VALUE) {
                    return false;
                }
            } else {
                if (m_relativeEnd != -1) {
                    return false;
                }
            }

            if (useTimeEnd) {
                if (m_timeEnd == null) {
                    return false;
                }
            } else {
                if (m_timeEnd != null) {
                    return false;
                }
            }

            if (useIncludeEnd) {
                ;
            } else {
                if (m_includeEnd != false) {
                    return false;
                }
            }

            return true;
        }

        //=========================================================================================
        // 機　能：モードごとに必要なフィールドを返す
        // 引　数：[in]type             時刻指定のモード
        // 　　　　[in]useDateStart     m_dateStartが必要なときtrue
        // 　　　　[in]useRelativeStart m_relativeStartが必要なときtrue
        // 　　　　[in]useTimeStart     m_timeStartが必要なときtrue
        // 　　　　[in]useIncludeStart  m_includeStartが必要なときtrue
        // 　　　　[in]useDateEnd       m_dateEndが必要なときtrue
        // 　　　　[in]useRelativeEnd   m_relativeEndが必要なときtrue
        // 　　　　[in]useTimeEnd       m_timeEndが必要なときtrue
        // 　　　　[in]useIncludeEnd    m_includeEndが必要なときtrue
        // 戻り値：なし
        //=========================================================================================
        public void CheckMandatoryField(DateTimeType timeType, out bool useDateStart, out bool useRelativeStart, out bool useTimeStart, out bool useIncludeStart, out bool useDateEnd, out bool useRelativeEnd, out bool useTimeEnd, out bool useIncludeEnd) {
            useDateStart = true;
            useRelativeStart = true;
            useTimeStart = true;
            useIncludeStart = true;
            useDateEnd = true;
            useRelativeEnd = true;
            useTimeEnd = true;
            useIncludeEnd = true;
            if (timeType == DateTimeType.None) {
                useDateStart = false;
                useRelativeStart = false;
                useTimeStart = false;
                useIncludeStart = false;
                useDateEnd = false;
                useRelativeEnd = false;
                useTimeEnd = false;
                useIncludeEnd = false;
            } else if (timeType == DateTimeType.DateXxxStart) {
                useTimeStart = false;
                useRelativeStart = false;
                useDateEnd = false;
                useTimeEnd = false;
                useRelativeEnd = false;
                useIncludeEnd = false;
            } else if (timeType == DateTimeType.DateEndXxx) {
                useDateStart = false;
                useRelativeStart = false;
                useTimeStart = false;
                useIncludeStart = false;
                useRelativeEnd = false;
                useTimeEnd = false;
            } else if (timeType == DateTimeType.DateStartXxxEnd) {
                useRelativeStart = false;
                useTimeStart = false;
                useRelativeEnd = false;
                useTimeEnd = false;
            } else if (timeType == DateTimeType.DateXxxStartEndXxx) {
                useRelativeStart = false;
                useTimeStart = false;
                useRelativeEnd = false;
                useTimeEnd = false;
            } else if (timeType == DateTimeType.DateXxx) {
                useRelativeStart = false;
                useTimeStart = false;
                useIncludeStart = false;
                useDateEnd = false;
                useRelativeEnd = false;
                useTimeEnd = false;
                useIncludeEnd = false;
            } else if (timeType == DateTimeType.RelativeXxxStart) {
                useDateStart = false;
                useDateEnd = false;
                useTimeEnd = false;
                useRelativeEnd = false;
                useIncludeEnd = false;
            } else if (m_timeType == DateTimeType.RelativeEndXxx) {
                useDateStart = false;
                useRelativeStart = false;
                useTimeStart = false;
                useIncludeStart = false;
                useDateEnd = false;
            } else if (timeType == DateTimeType.RelativeStartXxxEnd) {
                useDateStart = false;
                useDateEnd = false;
            } else if (timeType == DateTimeType.RelativeXxxStartEndXxx) {
                useDateStart = false;
                useDateEnd = false;
            } else if (timeType == DateTimeType.RelativeXxx) {
                useDateStart = false;
                useIncludeStart = false;
                useDateEnd = false;
                useRelativeEnd = false;
                useTimeEnd = false;
                useIncludeEnd = false;
            } else if (timeType == DateTimeType.TimeXxxStart) {
                useDateStart = false;
                useRelativeStart = false;
                useDateEnd = false;
                useTimeEnd = false;
                useRelativeEnd = false;
                useIncludeEnd = false;
            } else if (m_timeType == DateTimeType.TimeEndXxx) {
                useDateStart = false;
                useRelativeStart = false;
                useTimeStart = false;
                useIncludeStart = false;
                useDateEnd = false;
                useRelativeEnd = false;
            } else if (timeType == DateTimeType.TimeStartXxxEnd) {
                useDateStart = false;
                useRelativeStart = false;
                useDateEnd = false;
                useRelativeEnd = false;
            } else if (timeType == DateTimeType.TimeXxxStartEndXxx) {
                useDateStart = false;
                useRelativeStart = false;
                useDateEnd = false;
                useRelativeEnd = false;
            } else if (timeType == DateTimeType.TimeXxx) {
                useDateStart = false;
                useRelativeStart = false;
                useIncludeStart = false;
                useDateEnd = false;
                useRelativeEnd = false;
                useTimeEnd = false;
                useIncludeEnd = false;
            } else {
                Program.Abort("timeTypeの値が想定外です。");
                return;
            }
        }

        //=========================================================================================
        // 機　能：ファイルから読み込む
        // 引　数：[in]loader  読み込み用クラス
        // 　　　　[in]endTag  期待される終了タグ
        // 　　　　[out]obj    読み込み対象のオブジェクト
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public static bool LoadSetting(SettingLoader loader, SettingTag endTag, out DateTimeCondition obj) {
            bool success;
            obj = new DateTimeCondition();

            while (true) {
                SettingTag tagName;
                SettingTagType tagType;
                success = loader.GetTag(out tagName, out tagType);
                if (!success) {
                    return false;
                }
                if (tagType == SettingTagType.EndObject && tagName == endTag) {
                    break;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileCondition_DateTimeType) {
                    obj.m_timeType = DateTimeType.FromString(loader.StringValue);                       // NGのときnullとしてValidate()
                } else if (tagType == SettingTagType.LongValue && tagName == SettingTag.FileCondition_DateTimeDateStart) {
                    if (loader.LongValue == 0) {
                        obj.m_dateStart = DateTime.MinValue;
                    } else {
                        obj.m_dateStart = new DateTime(loader.LongValue);
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.FileCondition_DateTimeRelativeStart) {
                    obj.m_relativeStart = loader.IntValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileCondition_DateTimeTimeStart) {
                    obj.m_timeStart = TimeInfo.ParseTimeInfo(loader.StringValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_DateTimeIncludeStart) {
                    obj.m_includeStart = loader.BoolValue;
                } else if (tagType == SettingTagType.LongValue && tagName == SettingTag.FileCondition_DateTimeDateEnd) {
                    if (loader.LongValue == 0) {
                        obj.m_dateEnd = DateTime.MinValue;
                    } else {
                        obj.m_dateEnd = new DateTime(loader.LongValue);
                    }
                } else if (tagType == SettingTagType.IntValue && tagName == SettingTag.FileCondition_DateTimeRelativeEnd) {
                    obj.m_relativeEnd = loader.IntValue;
                } else if (tagType == SettingTagType.StringValue && tagName == SettingTag.FileCondition_DateTimeTimeEnd) {
                    obj.m_timeEnd = TimeInfo.ParseTimeInfo(loader.StringValue);
                } else if (tagType == SettingTagType.BoolValue && tagName == SettingTag.FileCondition_DateTimeIncludeEnd) {
                    obj.m_includeEnd = loader.BoolValue;
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：ファイルに項目1件を保存する
        // 引　数：[in]saver  保存用クラス
        // 　　　　[in]obj    保存するオブジェクト
        // 戻り値：保存に成功したときtrue
        //=========================================================================================
        public static bool SaveSetting(SettingSaver saver, DateTimeCondition obj) {
            saver.AddString(SettingTag.FileCondition_DateTimeType, obj.m_timeType.StringName);

            if (obj.m_dateStart == DateTime.MinValue) {
                saver.AddLong(SettingTag.FileCondition_DateTimeDateStart, 0);
            } else {
                saver.AddLong(SettingTag.FileCondition_DateTimeDateStart, obj.m_dateStart.Ticks);
            }
            saver.AddInt(SettingTag.FileCondition_DateTimeRelativeStart, obj.m_relativeStart);
            if (obj.m_timeStart != null) {
                saver.AddString(SettingTag.FileCondition_DateTimeTimeStart, obj.m_timeStart.ToString());
            }
            saver.AddBool(SettingTag.FileCondition_DateTimeIncludeStart, obj.m_includeStart);

            if (obj.m_dateEnd == DateTime.MinValue) {
                saver.AddLong(SettingTag.FileCondition_DateTimeDateEnd, 0);
            } else {
                saver.AddLong(SettingTag.FileCondition_DateTimeDateEnd, obj.m_dateEnd.Ticks);
            }
            saver.AddInt(SettingTag.FileCondition_DateTimeRelativeEnd, obj.m_relativeEnd);
            if (obj.m_timeEnd != null) {
                saver.AddString(SettingTag.FileCondition_DateTimeTimeEnd, obj.m_timeEnd.ToString());
            }
            saver.AddBool(SettingTag.FileCondition_DateTimeIncludeEnd, obj.m_includeEnd);

            return true;
        }
        
        //=========================================================================================
        // プロパティ：日付の指定条件
        //=========================================================================================
        public DateTimeType TimeType {
            get {
                return m_timeType;
            }
            set {
                m_timeType = value;
            }
        }

        //=========================================================================================
        // プロパティ：開始日時（指定しないときMinValue）
        //=========================================================================================
        public DateTime DateStart {
            get {
                return m_dateStart;
            }
            set {
                m_dateStart = value;
            }
        }

        //=========================================================================================
        // プロパティ：開始相対日付 n日前（指定しないとき-1）
        //=========================================================================================
        public int RelativeStart {
            get {
                return m_relativeStart;
            }
            set {
                m_relativeStart = value;
            }
        }

        //=========================================================================================
        // プロパティ：開始時刻（指定しないときnull）
        //=========================================================================================
        public TimeInfo TimeStart {
            get {
                return m_timeStart;
            }
            set {
                m_timeStart = value;
            }
        }

        //=========================================================================================
        // プロパティ：開始時刻そのものを含むときtrue（指定しないときfalse）
        //=========================================================================================
        public bool IncludeStart {
            get {
                return m_includeStart;
            }
            set {
                m_includeStart = value;
            }
        }

        //=========================================================================================
        // プロパティ：終了日時（指定しないときMinValue）
        //=========================================================================================
        public DateTime DateEnd {
            get {
                return m_dateEnd;
            }
            set {
                m_dateEnd = value;
            }
        }

        //=========================================================================================
        // プロパティ：終了相対日付 n日前（指定しないとき-1）
        //=========================================================================================
        public int RelativeEnd {
            get {
                return m_relativeEnd;
            }
            set {
                m_relativeEnd = value;
            }
        }

        //=========================================================================================
        // プロパティ：終了時刻（指定しないときnull）
        //=========================================================================================
        public TimeInfo TimeEnd {
            get {
                return m_timeEnd;
            }
            set {
                m_timeEnd = value;
            }
        }

        //=========================================================================================
        // プロパティ：終了時刻そのものを含むときtrue（指定しないときfalse）
        //=========================================================================================
        public bool IncludeEnd {
            get {
                return m_includeEnd;
            }
            set {
                m_includeEnd = value;
            }
        }
    }
}
