using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Util;
using ShellFiler.Properties;
using ShellFiler.FileSystem;
using ShellFiler.Locale;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：設定の読み込み用クラス
    //=========================================================================================
    public class SettingLoader {
        // ファイル名
        protected string m_fileName;

        // 読み込み内容
        protected string[] m_settingData;

        // 読み込み中の行
        protected int m_currentLine;

        // 読み込み中の行の値
        protected object m_currentValue;

        // 読み込んだファイルのバージョン（ロードしていないとき-1）
        protected int m_fileVersion = -1;

        // 読み込み済みファイルの書き込み日時
        private DateTime m_lastFileWriteTime = DateTime.MinValue;

        // 発生したエラーの詳細メッセージ（エラーが発生していないときnull）
        protected Error m_errorDetail = null;

        // 発生した警告メッセージのリスト
        private List<Warning> m_warningList = new List<Warning>();

        // 新しく登録する警告の種類
        private WarningGroup m_currentGroup;

        // 暗号化ユーティリティ（初期化前はnull）
        protected EncryptUtils m_encrypt = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public SettingLoader(string fileName) {
            m_fileName = fileName;
        }

        //=========================================================================================
        // 機　能：設定を読み込む
        // 引　数：[in]encrypt   暗号化するときtrue
        // 戻り値：入力に成功したときtrue
        //=========================================================================================
        public bool LoadSetting(bool encrypt) {
            // 全体を読み込み
            byte[] bytesData;
            try {
                bytesData = File.ReadAllBytes(m_fileName);
                FileInfo fileInfo = new FileInfo(m_fileName);
                m_lastFileWriteTime = fileInfo.LastWriteTime;
            } catch (Exception) {
                return SetError(Error.LoadFile);
            }

            // 暗号化を解除
            if (encrypt) {
                EncryptUtils encryptUtils = new EncryptUtils(0);
                bytesData = encryptUtils.DecryptBytes(bytesData);
            }

            // 文字列化
            string strAll = Encoding.UTF8.GetString(bytesData);
            m_settingData = StringUtils.SeparateLine(strAll);

            // ヘッダを読み込む
            if (m_settingData.Length == 0 || !m_settingData[0].StartsWith(SettingTag.Header.TagName + ":")) {
                return SetError(Error.HeaderFormat);
            }
            string version = m_settingData[0].Substring(SettingTag.Header.TagName.Length + 2);
            if (!int.TryParse(version, out m_fileVersion)) {
                return SetError(Error.HeaderVersion);
            }
            m_currentLine = 0;

            return true;
        }

        //=========================================================================================
        // 機　能：読み込み位置をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetReadPosition() {
            m_currentLine = 0;
        }

        //=========================================================================================
        // 機　能：エラー情報をセットする
        // 引　数：[in]error  エラー情報
        // 戻り値：常にfalse
        //=========================================================================================
        private bool SetError(Error error) {
            m_errorDetail = error;
            return false;
        }

        //=========================================================================================
        // 機　能：警告情報をセットする
        // 引　数：[in]template   メッセージ
        // 　　　　[in]paramList  パラメータ
        // 戻り値：なし
        //=========================================================================================
        public bool SetWarning(string template, params object[] paramList) {
            string message = string.Format(template, paramList);
            m_warningList.Add(new Warning(m_currentGroup, message));
            return false;
        }

        //=========================================================================================
        // 機　能：新しく登録する警告の種類をセットする
        // 引　数：[in]group  警告の種類
        // 戻り値：なし
        //=========================================================================================
        public void SetCurrentWarningGroup(WarningGroup group) {
            m_currentGroup = group;
        }

        //=========================================================================================
        // 機　能：次のタグを読み込む
        // 引　数：[out]tagName  タグ名を返す変数
        // 　　　　[out]tagType  タグの種類を返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public bool GetTag(out SettingTag tag, out SettingTagType tagType) {
            bool success;
            tag = null;
            tagType = SettingTagType.None;
            m_currentValue = null;

            // 終了をチェック
            m_currentLine++;
            if (m_currentLine >= m_settingData.Length) {
                return SetError(Error.UnexpectedEof);
            }

            // タグを分解して解析
            string line = m_settingData[m_currentLine].TrimStart();
            string[] keyValue = line.Split(new char[] {'>'}, 2);
            if (keyValue.Length != 2 || keyValue[0].Length < 2 || keyValue[0][0] != '<') {
                return SetError(Error.LineFormat);
            }

            // タグを解析
            string tagName;
            string[] typeTag = keyValue[0].Split(new char[] {':'}, 2);
            if (typeTag.Length == 1) {
                tagName = typeTag[0];
                if (tagName.Length < 2) {
                    return SetError(Error.LineFormat);
                }
                if (tagName[1] == '/') {
                    tagType = SettingTagType.EndObject;
                    tagName = tagName.Substring(2);
                } else {
                    tagType = SettingTagType.BeginObject;
                    tagName = tagName.Substring(1);
                }
            } else {
                string typeName = typeTag[0];
                if (typeName.Length != 2) {
                    return SetError(Error.LineFormat);
                }
                string value = keyValue[1];
                char typeChar = typeName[1];
                tagName = typeTag[1];
                if (typeChar == 's') {
                    // 文字列
                    tagType = SettingTagType.StringValue;
                    m_currentValue = DecodeString(value);
                } else if (typeChar == 'p') {
                    // パスワード
                    tagType = SettingTagType.PasswordValue;
                    success = ParsePassword(value, out m_currentValue);
                    if (!success) {
                        return success;
                    }
                } else if (typeChar == 'c') {
                    // 色
                    tagType = SettingTagType.ColorValue;
                    success = ParseColor(value, out m_currentValue);
                    if (!success) {
                        return success;
                    }
                } else if (typeChar == 'r') {
                    // 領域
                    tagType = SettingTagType.RectangleValue;
                    success = ParseRectangle(value, out m_currentValue);
                    if (!success) {
                        return success;
                    }
                } else if (typeChar == 'i') {
                    // int
                    tagType = SettingTagType.IntValue;
                    success = ParseInt(value, out m_currentValue);
                    if (!success) {
                        return success;
                    }
                } else if (typeChar == 'f') {
                    // float
                    tagType = SettingTagType.FloatValue;
                    success = ParseFloat(value, out m_currentValue);
                    if (!success) {
                        return success;
                    }
                } else if (typeChar == 'b') {
                    // bool
                    tagType = SettingTagType.BoolValue;
                    success = ParseBool(value, out m_currentValue);
                    if (!success) {
                        return success;
                    }
                } else if (typeChar == 'l') {
                    // long
                    tagType = SettingTagType.LongValue;
                    success = ParseLong(value, out m_currentValue);
                    if (!success) {
                        return success;
                    }
                } else {
                    tagType = SettingTagType.None;
                }
            }
            tag = SettingTag.FromTagName(tagName);

            return true;
        }

        //=========================================================================================
        // 機　能：パスワードの文字列を解析する
        // 引　数：[in]value    文字列表現での値
        // 　　　　[out]result  読み込んだ結果を返す変数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParsePassword(string value, out object result) {
            result = null;
            if (m_encrypt == null) {
                int saltCode = SfHelperDecoder.GetPasswordSalt();
                m_encrypt = new EncryptUtils(saltCode);
            }
            string password = m_encrypt.Decrypt(value);
            if (password == null) {
                return SetError(Error.PasswordFormat);
            }
            result = password;
            return true;
        }

        //=========================================================================================
        // 機　能：色情報の文字列を解析する
        // 引　数：[in]value    文字列表現での値
        // 　　　　[out]result  読み込んだ結果を返す変数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParseColor(string value, out object result) {
            result = Color.Empty;
            if (value == null || value == "") {
                return true;
            } else {
                if (!value.StartsWith("RGB(")) {
                    return false;
                }
                if (!value.EndsWith(")")) {
                    return false;
                }
                string[] rgb = value.Substring(4, value.Length - 5).Split(',');
                if (rgb.Length != 3) {
                    return false;
                }
                int r,g,b;
                if (!int.TryParse(rgb[0], out r)) {
                    return false;
                }
                if (!int.TryParse(rgb[1], out g)) {
                    return false;
                }
                if (!int.TryParse(rgb[2], out b)) {
                    return false;
                }
                if (0 <= r && r <= 255 && 0 <= g && g <= 255 && 0 <= b && b <= 255) {
                    result = Color.FromArgb(r, g, b);
                    return true;
                } else {
                    return false;
                }
            }
        }

        //=========================================================================================
        // 機　能：領域の文字列を解析する
        // 引　数：[in]value    文字列表現での値
        // 　　　　[out]result  読み込んだ結果を返す変数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParseRectangle(string value, out object result) {
            result = Rectangle.Empty;
            if (value == null || value == "") {
                return true;
            } else {
                if (!value.StartsWith("Rectangle(")) {
                    return false;
                }
                if (!value.EndsWith(")")) {
                    return false;
                }
                string[] strRect = value.Substring(10, value.Length - 11).Split(',');
                if (strRect.Length != 4) {
                    return false;
                }
                int left, top, width, height;
                if (!int.TryParse(strRect[0], out left)) {
                    return false;
                }
                if (!int.TryParse(strRect[1], out top)) {
                    return false;
                }
                if (!int.TryParse(strRect[2], out width)) {
                    return false;
                }
                if (!int.TryParse(strRect[3], out height)) {
                    return false;
                }
                if (width < 0 || height < 0) {
                    return false;
                }
                result = new Rectangle(left, top, width, height);
                return true;
            }
        }

        //=========================================================================================
        // 機　能：数値の文字列を解析する
        // 引　数：[in]value    文字列表現での値
        // 　　　　[out]result  読み込んだ結果を返す変数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParseInt(string value, out object result) {
            result = 0;
            int intValue;
            if (!int.TryParse(value, out intValue)) {
                return SetError(Error.IntFormat);
            }
            result = intValue;
            return true;
        }

        //=========================================================================================
        // 機　能：実数の値の文字列を解析する
        // 引　数：[in]value    文字列表現での値
        // 　　　　[out]result  読み込んだ結果を返す変数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParseFloat(string value, out object result) {
            result = 0;
            float floatValue;
            if (!float.TryParse(value, out floatValue)) {
                return SetError(Error.FloatFormat);
            }
            result = floatValue;
            return true;
        }

        //=========================================================================================
        // 機　能：boolの文字列を解析する
        // 引　数：[in]value    文字列表現での値
        // 　　　　[out]result  読み込んだ結果を返す変数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParseBool(string value, out object result) {
            result = false;
            bool boolValue;
            if (!bool.TryParse(value, out boolValue)) {
                return SetError(Error.BoolFormat);
            }
            result = boolValue;
            return true;
        }

        //=========================================================================================
        // 機　能：longの文字列を解析する
        // 引　数：[in]value    文字列表現での値
        // 　　　　[out]result  読み込んだ結果を返す変数
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        private bool ParseLong(string value, out object result) {
            result = 0;
            long longValue;
            if (!long.TryParse(value, out longValue)) {
                return SetError(Error.LongFormat);
            }
            result = longValue;
            return true;
        }

        //=========================================================================================
        // 機　能：次のタグが特定の内容であることを期待して読み込む
        // 引　数：[in]tagName  タグ名の期待値
        // 　　　　[in]tagType  タグの種類の期待値
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public bool ExpectTag(SettingTag tagName, SettingTagType tagType) {
            SettingTag actualTagName;
            SettingTagType actualTagType;
            bool success = GetTag(out actualTagName, out actualTagType);
            if (!success) {
                return success;
            }
            if (tagType != actualTagType) {
                return SetError(Error.UnexpectedData);
            }
            if (tagName != null && tagType != actualTagType) {
                return SetError(Error.UnexpectedData);
            }
            return true;
        }

        //=========================================================================================
        // 機　能：次のタグが特定の内容であることを期待して読み込む
        // 引　数：[in]tagName  タグ名の期待値
        // 　　　　[in]tagType  タグの種類の期待値
        // 　　　　[out]fit     期待値通りのときtrueを返す変数
        // 戻り値：読み込みに成功したときtrue
        //=========================================================================================
        public bool PeekTag(SettingTag tagName, SettingTagType tagType, out bool fit) {
            fit = false;
            SettingTag actualTagName;
            SettingTagType actualTagType;
            bool success = GetTag(out actualTagName, out actualTagType);
            if (!success) {
                return success;
            }
            m_currentLine--;
            if (tagType != actualTagType) {
                fit = false;
                return true;
            }
            if (tagName != null && tagType != actualTagType) {
                fit = false;
                return true;
            }
            fit = true;
            return true;
        }

        //=========================================================================================
        // 機　能：次のタグに進める
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void NextTag() {
            m_currentLine++;
        }

        //=========================================================================================
        // 機　能：文字列を出力形式からデコードする
        // 引　数：[in]org  デコード前の文字列
        // 戻り値：デコード後の文字列
        //=========================================================================================
        private string DecodeString(string org) {
            StringBuilder result = new StringBuilder();
            char[] chArray = org.ToCharArray();
            bool escape = false;
            for (int i = 0; i < chArray.Length; i++) {
                char ch = chArray[i];
                if (escape) {
                    // \の次の文字
                    escape = false;
                    if (ch == 'n') {
                        result.Append('\n');
                    } else if (ch == 'r') {
                        result.Append('\r');
                    } else if (ch == 'a') {
                        result.Append('\x00');
                    } else {
                        result.Append(ch);
                    }
                } else if (ch == '\\') {
                    escape = true;
                } else {
                    // 通常の文字
                    result.Append(ch);
                }
            }
            return result.ToString();
        }

        //=========================================================================================
        // プロパティ：ファイル名
        //=========================================================================================
        public string FileName {
            get {
                return m_fileName;
            }
        }

        //=========================================================================================
        // プロパティ：エラーの詳細情報
        //=========================================================================================
        public string ErrorDetail {
            get {
                string message;
                if (m_errorDetail == null) {
                    // エラーなしで呼ばれた場合はタグ不正と解釈
                    message = Path.GetFileName(m_fileName) + ":" + m_currentLine + " " + Error.UnexpectedData.ErrorMessage;
                } else if (m_errorDetail == Error.LoadFile) {
                    message = m_errorDetail.ErrorMessage;
                } else {
                    message = Path.GetFileName(m_fileName) + ":" + m_currentLine + " " + m_errorDetail.ErrorMessage;
                }
                return message;
            }
        }

        //=========================================================================================
        // プロパティ：発生した警告メッセージのリスト
        //=========================================================================================
        public List<Warning> WarningList {
            get {
                return m_warningList;
            }
        }

        //=========================================================================================
        // プロパティ：値の文字列での取得
        //=========================================================================================
        public string StringValue {
            get {
                return (string)m_currentValue;
            }
        }

        //=========================================================================================
        // プロパティ：値のパスワード文字列での取得
        //=========================================================================================
        public string PasswordValue {
            get {
                return (string)m_currentValue;
            }
        }

        //=========================================================================================
        // プロパティ：値の数値での取得
        //=========================================================================================
        public int IntValue {
            get {
                return (int)m_currentValue;
            }
        }

        //=========================================================================================
        // プロパティ：値のfloatでの取得
        //=========================================================================================
        public float FloatValue {
            get {
                return (float)m_currentValue;
            }
        }

        //=========================================================================================
        // プロパティ：値の色での取得
        //=========================================================================================
        public Color ColorValue {
            get {
                return (Color)m_currentValue;
            }
        }

        //=========================================================================================
        // プロパティ：値の領域での取得
        //=========================================================================================
        public Rectangle RectangleValue {
            get {
                return (Rectangle)m_currentValue;
            }
        }

        //=========================================================================================
        // プロパティ：値のboolでの取得
        //=========================================================================================
        public bool BoolValue {
            get {
                return (bool)m_currentValue;
            }
        }

        //=========================================================================================
        // プロパティ：値のlongでの取得
        //=========================================================================================
        public long LongValue {
            get {
                return (long)m_currentValue;
            }
        }

        //=========================================================================================
        // プロパティ：読み込んだファイルのバージョン（ロードしていないとき-1）
        //=========================================================================================
        public int FileVersion {
            get {
                return m_fileVersion;
            }
        }

        //=========================================================================================
        // プロパティ：読み込み済みファイルの書き込み日時
        //=========================================================================================
        public DateTime LastFileWriteTime {
            get {
                return m_lastFileWriteTime;
            }
        }

        //=========================================================================================
        // クラス：エラー情報
        //=========================================================================================
        public class Error {
            public static readonly Error LoadFile        = new Error(Resources.SettingLoader_ErrorLoadFile);
            public static readonly Error HeaderFormat    = new Error(Resources.SettingLoader_ErrorHeaderFormat);
            public static readonly Error HeaderVersion   = new Error(Resources.SettingLoader_ErrorHeaderVersion);
            public static readonly Error LineFormat      = new Error(Resources.SettingLoader_ErrorLineFormat);
            public static readonly Error UnknownTagValue = new Error(Resources.SettingLoader_ErrorUnknownTagValue);
            public static readonly Error PasswordFormat  = new Error(Resources.SettingLoader_ErrorPasswordFormat);
            public static readonly Error ColorFormat     = new Error(Resources.SettingLoader_ErrorColorFormat);
            public static readonly Error IntFormat       = new Error(Resources.SettingLoader_ErrorIntFormat);
            public static readonly Error FloatFormat     = new Error(Resources.SettingLoader_ErrorFloatFormat);
            public static readonly Error BoolFormat      = new Error(Resources.SettingLoader_ErrorBoolFormat);
            public static readonly Error LongFormat      = new Error(Resources.SettingLoader_ErrorLongFormat);
            public static readonly Error UnexpectedData  = new Error(Resources.SettingLoader_ErrorUnexpectedData);
            public static readonly Error UnexpectedEof   = new Error(Resources.SettingLoader_ErrorUnexpectedEof);

            // エラーメッセージ
            private string m_errorMessage;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]errorMessage  エラーメッセージ
            // 戻り値：なし
            //=========================================================================================
            private Error(string errorMessage) {
                m_errorMessage = errorMessage;
            }

            //=========================================================================================
            // プロパティ：エラーメッセージ
            //=========================================================================================
            public string ErrorMessage {
                get {
                    return m_errorMessage;
                }
            }
        }

        //=========================================================================================
        // クラス：発生した警告の種類
        // キー定義では単一のファイル内に複数の種類が定義されており、解析時に切り替えながら
        // 警告の事象を登録していく。SetCurrentWarningGroup()で登録した警告の種類を使って
        // 警告メッセージを登録する。
        //=========================================================================================
        public class WarningGroup {
            public static readonly WarningGroup KeyFileList         = new WarningGroup(Resources.SettingLoader_GroupFileList);          // ファイル一覧のキー定義
            public static readonly WarningGroup KeyFileViewer       = new WarningGroup(Resources.SettingLoader_GroupFileViewer);        // ファイルビューアのキー定義
            public static readonly WarningGroup KeyGraphicsViewer   = new WarningGroup(Resources.SettingLoader_GroupGraphicsViewer);    // グラフィックビューアのキー定義
            public static readonly WarningGroup KeyMonitoringViewer = new WarningGroup(Resources.SettingLoader_GroupMonitoringViewer);  // モニタリングビューアのキー定義
            public static readonly WarningGroup KeyTerminal         = new WarningGroup(Resources.SettingLoader_GroupTerminal);          // SSHターミナルのキー定義
            public static readonly WarningGroup KeyAssociate        = new WarningGroup(Resources.SettingLoader_GroupAssociate);         // 関連付けの定義
            public static readonly WarningGroup MenuFileList        = new WarningGroup(Resources.SettingLoader_GroupMenuFileList);      // ファイル一覧のメニュー定義

            // 表示名
            private string m_displayName;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]displayName  表示名
            // 戻り値：なし
            //=========================================================================================
            private WarningGroup(string displayName) {
                m_displayName = displayName;
            }

            //=========================================================================================
            // プロパティ：表示名
            //=========================================================================================
            public string DisplayName {
                get {
                    return m_displayName;
                }
            }
        }

        //=========================================================================================
        // クラス：警告情報
        //=========================================================================================
        public class Warning {
            // 問題の発生箇所
            private WarningGroup m_group;
            
            // 警告メッセージ
            private string m_message;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]kind     問題の発生箇所
            // 　　　　[in]message  警告メッセージ
            // 戻り値：なし
            //=========================================================================================
            public Warning(WarningGroup group, string errorMessage) {
                m_group = group;
                m_message = errorMessage;
            }

            //=========================================================================================
            // プロパティ：問題の発生箇所
            //=========================================================================================
            public WarningGroup Group {
                get {
                    return m_group;
                }
            }

            //=========================================================================================
            // プロパティ：メッセージ
            //=========================================================================================
            public string Message {
                get {
                    return m_message;
                }
            }
        }
    }
}
