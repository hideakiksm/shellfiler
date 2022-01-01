using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace ShellFiler.Locale {

    //=========================================================================================
    // クラス：キーの表示名の変換クラス
    //=========================================================================================
    class KeyNameUtils {
        // キー名のリスト
        private static List<KeyNameCategory> s_keyNameList = null;

        // キーの順序
        private static Dictionary<Keys, int> s_keyOrderMap = null;

        //=========================================================================================
        // 機　能：キーの表示名を得る
        // 引　数：[in]key  キー
        // 戻り値：キーの表示名
        //=========================================================================================
        public static string GetDisplayName(Keys key) {
            string keyName = "";
            switch (key) {
                case Keys.Cancel:
                    keyName = "Cancel";
                    break;
                case Keys.Back:
                    keyName = "BackSpace";
                    break;
                case Keys.Tab:
                    keyName = "Tab";
                    break;
                case Keys.LineFeed:
                    keyName = "LF";
                    break;
                case Keys.Clear:
                    keyName = "Clear";
                    break;
                case Keys.Enter:
                    keyName = "Enter";
                    break;
                case Keys.ShiftKey:
                    keyName = "Shift";
                    break;
                case Keys.ControlKey:
                    keyName = "Ctrl";
                    break;
                case Keys.Menu:
                    keyName = "Alt";
                    break;
                case Keys.Pause:
                    keyName = "Pause";
                    break;
                case Keys.CapsLock:
                    keyName = "CapsLock";
                    break;
                case Keys.KanaMode:
                    keyName = "かなモード";
                    break;
                case Keys.JunjaMode:
                    keyName = "Junja";
                    break;
                case Keys.FinalMode:
                    keyName = "Final";
                    break;
                case Keys.KanjiMode:
                    keyName = "漢字";
                    break;
                case Keys.Escape:
                    keyName = "Esc";
                    break;
                case Keys.IMEConvert:
                    keyName = "変換";
                    break;
                case Keys.IMENonconvert:
                    keyName = "無変換";
                    break;
                case Keys.IMEAccept:
                    keyName = "Accept";
                    break;
                case Keys.IMEModeChange:
                    keyName = "モード";
                    break;
                case Keys.Space:
                    keyName = "Space";
                    break;
                case Keys.Prior:
                    keyName = "PageUp";
                    break;
                case Keys.Next:
                    keyName = "PageDown";
                    break;
                case Keys.End:
                    keyName = "End";
                    break;
                case Keys.Home:
                    keyName = "Home";
                    break;
                case Keys.Left:
                    keyName = "←";
                    break;
                case Keys.Up:
                    keyName = "↑";
                    break;
                case Keys.Right:
                    keyName = "→";
                    break;
                case Keys.Down:
                    keyName = "↓";
                    break;
                case Keys.Select:
                    keyName = "Select";
                    break;
                case Keys.Print:
                    keyName = "Print";
                    break;
                case Keys.Execute:
                    keyName = "Execute";
                    break;
                case Keys.PrintScreen:
                    keyName = "PrintScreen";
                    break;
                case Keys.Insert:
                    keyName = "Ins";
                    break;
                case Keys.Delete:
                    keyName = "Del";
                    break;
                case Keys.Help:
                    keyName = "Help";
                    break;
                case Keys.D0:
                    keyName = "0";
                    break;
                case Keys.D1:
                    keyName = "1";
                    break;
                case Keys.D2:
                    keyName = "2";
                    break;
                case Keys.D3:
                    keyName = "3";
                    break;
                case Keys.D4:
                    keyName = "4";
                    break;
                case Keys.D5:
                    keyName = "5";
                    break;
                case Keys.D6:
                    keyName = "6";
                    break;
                case Keys.D7:
                    keyName = "7";
                    break;
                case Keys.D8:
                    keyName = "8";
                    break;
                case Keys.D9:
                    keyName = "9";
                    break;
                case Keys.A:
                    keyName = "A";
                    break;
                case Keys.B:
                    keyName = "B";
                    break;
                case Keys.C:
                    keyName = "C";
                    break;
                case Keys.D:
                    keyName = "D";
                    break;
                case Keys.E:
                    keyName = "E";
                    break;
                case Keys.F:
                    keyName = "F";
                    break;
                case Keys.G:
                    keyName = "G";
                    break;
                case Keys.H:
                    keyName = "H";
                    break;
                case Keys.I:
                    keyName = "I";
                    break;
                case Keys.J:
                    keyName = "J";
                    break;
                case Keys.K:
                    keyName = "K";
                    break;
                case Keys.L:
                    keyName = "L";
                    break;
                case Keys.M:
                    keyName = "M";
                    break;
                case Keys.N:
                    keyName = "N";
                    break;
                case Keys.O:
                    keyName = "O";
                    break;
                case Keys.P:
                    keyName = "P";
                    break;
                case Keys.Q:
                    keyName = "Q";
                    break;
                case Keys.R:
                    keyName = "R";
                    break;
                case Keys.S:
                    keyName = "S";
                    break;
                case Keys.T:
                    keyName = "T";
                    break;
                case Keys.U:
                    keyName = "U";
                    break;
                case Keys.V:
                    keyName = "V";
                    break;
                case Keys.W:
                    keyName = "W";
                    break;
                case Keys.X:
                    keyName = "X";
                    break;
                case Keys.Y:
                    keyName = "Y";
                    break;
                case Keys.Z:
                    keyName = "Z";
                    break;
                case Keys.LWin:
                    keyName = "Windows";
                    break;
                case Keys.RWin:
                    keyName = "Windows";
                    break;
                case Keys.Apps:
                    keyName = "App";
                    break;
                case Keys.Sleep:
                    keyName = "Sleep";
                    break;
                case Keys.NumPad0:
                    keyName = "Num0";
                    break;
                case Keys.NumPad1:
                    keyName = "Num1";
                    break;
                case Keys.NumPad2:
                    keyName = "Num2";
                    break;
                case Keys.NumPad3:
                    keyName = "Num3";
                    break;
                case Keys.NumPad4:
                    keyName = "Num4";
                    break;
                case Keys.NumPad5:
                    keyName = "Num5";
                    break;
                case Keys.NumPad6:
                    keyName = "Num6";
                    break;
                case Keys.NumPad7:
                    keyName = "Num7";
                    break;
                case Keys.NumPad8:
                    keyName = "Num8";
                    break;
                case Keys.NumPad9:
                    keyName = "Num9";
                    break;
                case Keys.Multiply:
                    keyName = "Num*";
                    break;
                case Keys.Add:
                    keyName = "Num+";
                    break;
                case Keys.Separator:
                    keyName = "Separator";
                    break;
                case Keys.Subtract:
                    keyName = "Num-";
                    break;
                case Keys.Decimal:
                    keyName = "Num.";
                    break;
                case Keys.Divide:
                    keyName = "Num/";
                    break;
                case Keys.F1:
                    keyName = "F1";
                    break;
                case Keys.F2:
                    keyName = "F2";
                    break;
                case Keys.F3:
                    keyName = "F3";
                    break;
                case Keys.F4:
                    keyName = "F4";
                    break;
                case Keys.F5:
                    keyName = "F5";
                    break;
                case Keys.F6:
                    keyName = "F6";
                    break;
                case Keys.F7:
                    keyName = "F7";
                    break;
                case Keys.F8:
                    keyName = "F8";
                    break;
                case Keys.F9:
                    keyName = "F9";
                    break;
                case Keys.F10:
                    keyName = "F10";
                    break;
                case Keys.F11:
                    keyName = "F11";
                    break;
                case Keys.F12:
                    keyName = "F12";
                    break;
                case Keys.F13:
                    keyName = "F13";
                    break;
                case Keys.F14:
                    keyName = "F14";
                    break;
                case Keys.F15:
                    keyName = "F15";
                    break;
                case Keys.F16:
                    keyName = "F16";
                    break;
                case Keys.F17:
                    keyName = "F17";
                    break;
                case Keys.F18:
                    keyName = "F18";
                    break;
                case Keys.F19:
                    keyName = "F19";
                    break;
                case Keys.F20:
                    keyName = "F20";
                    break;
                case Keys.F21:
                    keyName = "F21";
                    break;
                case Keys.F22:
                    keyName = "F22";
                    break;
                case Keys.F23:
                    keyName = "F23";
                    break;
                case Keys.F24:
                    keyName = "F24";
                    break;
                case Keys.NumLock:
                    keyName = "NumLock";
                    break;
                case Keys.Scroll:
                    keyName = "ScrollLock";
                    break;
                case Keys.BrowserBack:
                    keyName = "Back";
                    break;
                case Keys.BrowserForward:
                    keyName = "Forward";
                    break;
                case Keys.BrowserRefresh:
                    keyName = "Refresh";
                    break;
                case Keys.BrowserStop:
                    keyName = "Stop";
                    break;
                case Keys.BrowserSearch:
                    keyName = "Search";
                    break;
                case Keys.BrowserFavorites:
                    keyName = "Favorites";
                    break;
                case Keys.BrowserHome:
                    keyName = "Home";
                    break;
                case Keys.VolumeMute:
                    keyName = "Mute";
                    break;
                case Keys.VolumeDown:
                    keyName = "VolumeDown";
                    break;
                case Keys.VolumeUp:
                    keyName = "VolumeUp";
                    break;
                case Keys.MediaNextTrack:
                    keyName = "NextTrack";
                    break;
                case Keys.MediaPreviousTrack:
                    keyName = "PreviousTrack";
                    break;
                case Keys.MediaStop:
                    keyName = "Stop";
                    break;
                case Keys.MediaPlayPause:
                    keyName = "PlayPause";
                    break;
                case Keys.LaunchMail:
                    keyName = "Mail";
                    break;
                case Keys.SelectMedia:
                    keyName = "Media";
                    break;
                case Keys.LaunchApplication1:
                    keyName = "Application1";
                    break;
                case Keys.LaunchApplication2:
                    keyName = "Application2";
                    break;
                case Keys.Oem1:
                    keyName = ":";
                    break;
                case Keys.Oemplus:
                    keyName = ";";
                    break;
                case Keys.Oemcomma:
                    keyName = ",";
                    break;
                case Keys.OemMinus:
                    keyName = "-";
                    break;
                case Keys.OemPeriod:
                    keyName = ".";
                    break;
                case Keys.OemQuestion:
                    keyName = "/";
                    break;
                case Keys.Oemtilde:
                    keyName = "@";
                    break;
                case Keys.Oem4:
                    keyName = "[";
                    break;
                case Keys.OemPipe:
                    keyName = "\\";
                    break;
                case Keys.Oem6:
                    keyName = "]";
                    break;
                case Keys.Oem7:
                    keyName = "^";
                    break;
                case Keys.Oem8:
                    keyName = "OEM 8";
                    break;
                case Keys.Oem102:
                    keyName = "_";
                    break;
                case Keys.ProcessKey:
                    keyName = "ProcessKey";
                    break;
                case Keys.Attn:
                    keyName = "Attn";
                    break;
                case Keys.Crsel:
                    keyName = "Crsel";
                    break;
                case Keys.Exsel:
                    keyName = "Exsel";
                    break;
                case Keys.EraseEof:
                    keyName = "EraseEof";
                    break;
                case Keys.Play:
                    keyName = "Play";
                    break;
                case Keys.Zoom:
                    keyName = "Zoom";
                    break;
                case Keys.Pa1:
                    keyName = "PA1";
                    break;
                case Keys.OemClear:
                    keyName = "Clear";
                    break;
                default:
                    keyName = key.ToString();
                    break;
            }
            return keyName;
        }

        //=========================================================================================
        // キー名のリスト
        //=========================================================================================
        public static List<KeyNameCategory> KeyCategoryList {
            get {
                if (s_keyNameList == null) {
                    List<KeyNameCategory> keyNameList = new List<KeyNameCategory>();
                    Dictionary<Keys, int> keyOrderMap = new Dictionary<Keys, int>();
                    KeyNameCategory keyCategoryNormal = new KeyNameCategory("英数字キー");
                    keyNameList.Add(keyCategoryNormal);
                    KeyNameCategory keyCategoryTen = new KeyNameCategory("テンキー");
                    keyNameList.Add(keyCategoryTen);
                    KeyNameCategory keyCategoryFunc = new KeyNameCategory("ファンクションキー");
                    keyNameList.Add(keyCategoryFunc);
                    KeyNameCategory keyCategorySpec = new KeyNameCategory("特殊キー");
                    keyNameList.Add(keyCategorySpec);
                    KeyNameCategory keyCategorySymbol = new KeyNameCategory("記号キー");
                    keyNameList.Add(keyCategorySymbol);

                    AddKeyItem(Keys.D0, "0", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.D1, "1", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.D2, "2", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.D3, "3", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.D4, "4", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.D5, "5", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.D6, "6", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.D7, "7", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.D8, "8", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.D9, "9", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.A,  "A", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.B,  "B", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.C,  "C", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.D,  "D", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.E,  "E", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.F,  "F", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.G,  "G", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.H,  "H", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.I,  "I", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.J,  "J", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.K,  "K", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.L,  "L", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.M,  "M", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.N,  "N", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.O,  "O", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.P,  "P", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.Q,  "Q", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.R,  "R", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.S,  "S", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.T,  "T", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.U,  "U", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.V,  "V", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.W,  "W", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.X,  "X", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.Y,  "Y", keyCategoryNormal, keyOrderMap);
                    AddKeyItem(Keys.Z,  "Z", keyCategoryNormal, keyOrderMap);

                    AddKeyItem(Keys.NumPad0,  "Num0", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.NumPad1,  "Num1", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.NumPad2,  "Num2", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.NumPad3,  "Num3", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.NumPad4,  "Num4", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.NumPad5,  "Num5", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.NumPad6,  "Num6", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.NumPad7,  "Num7", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.NumPad8,  "Num8", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.NumPad9,  "Num9", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.Decimal,  "Num.", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.Add,      "Num+", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.Subtract, "Num-", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.Multiply, "Num*", keyCategoryTen, keyOrderMap);
                    AddKeyItem(Keys.Divide,   "Num/", keyCategoryTen, keyOrderMap);

                    AddKeyItem(Keys.F1,  "F1",  keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F2,  "F2",  keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F3,  "F3",  keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F4,  "F4",  keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F5,  "F5",  keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F6,  "F6",  keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F7,  "F7",  keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F8,  "F8",  keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F9,  "F9",  keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F10, "F10", keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F11, "F11", keyCategoryFunc, keyOrderMap);
                    AddKeyItem(Keys.F12, "F12", keyCategoryFunc, keyOrderMap);

                    AddKeyItem(Keys.Space,  "Space",      keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Enter,  "Enter",      keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Left,   "←",         keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Up,     "↑",         keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Right,  "→",         keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Down,   "↓",         keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Prior,  "PageUp",     keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Next,   "PageDown",   keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.End,    "End",        keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Home,   "Home",       keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Insert, "Ins",        keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Delete, "Del",        keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Back,   "BackSpace",  keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Escape, "Esc",        keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Tab,    "Tab",        keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Pause,  "Pause",      keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.Scroll, "ScrollLock", keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.IMEConvert, "変換",   keyCategorySpec, keyOrderMap);
                    AddKeyItem(Keys.IMENonconvert, "無変換", keyCategorySpec, keyOrderMap);

                    AddKeyItem(Keys.OemMinus,    "-",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.Oem7,        "^",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.OemPipe,     "|",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.Oemtilde,    "@",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.Oem4,        "[",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.Oemplus,     ";",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.Oem1,        ":",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.Oem6,        "]",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.Oemcomma,    ",",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.OemPeriod,   ".",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.OemQuestion, "/",     keyCategorySymbol, keyOrderMap);
                    AddKeyItem(Keys.Oem102,      "_",     keyCategorySymbol, keyOrderMap);

                    s_keyNameList = keyNameList;
                    s_keyOrderMap = keyOrderMap;
                }
                return s_keyNameList;
            }
        }

        //=========================================================================================
        // 機　能：キーの定義を登録する
        // 引　数：[in]key      キー
        // 　　　　[in]display  キーの表示名
        // 　　　　[in]category カテゴリ情報の登録先
        // 　　　　[in]orderMap キーの順序の登録先
        // 戻り値：なし
        //=========================================================================================
        private static void AddKeyItem(Keys key, string display, KeyNameCategory category, Dictionary<Keys, int> orderMap) {
            category.Add(key, display);
            orderMap.Add(key, orderMap.Count);
        }

        //=========================================================================================
        // 機　能：キーの表示上のソート順序を取得する
        // 引　数：[in]key  キー
        // 戻り値：ソート順序
        // メ　モ：KeyCategoryListの呼び出し後に実行すること
        //=========================================================================================
        public static int GetKeyOrder(Keys key) {
            if (s_keyOrderMap.ContainsKey(key)) {
                return s_keyOrderMap[key];
            } else {
                return s_keyOrderMap.Count;
            }
        }

        //=========================================================================================
        // クラス：キー定義でのカテゴリ
        //=========================================================================================
        public class KeyNameCategory {
            // カテゴリ名
            private string m_categoryName;
            
            // キー名の一覧
            private List<KeyNameItem> m_keyNameItemList = new List<KeyNameItem>();

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]groupName  カテゴリ名
            // 戻り値：なし
            //=========================================================================================
            public KeyNameCategory(string categoryName) {
                m_categoryName = categoryName;
            }

            //=========================================================================================
            // 機　能：キー定義を追加する
            // 引　数：[in]keyCode  キーコード
            // 　　　　[in]dispName キーの表示名
            // 戻り値：なし
            //=========================================================================================
            public void Add(Keys keyCode, string dispName) {
                m_keyNameItemList.Add(new KeyNameItem(keyCode, dispName));
            }

            //=========================================================================================
            // プロパティ：カテゴリ名
            //=========================================================================================
            public string CategoryName {
                get {
                    return m_categoryName;
                }
            }

            //=========================================================================================
            // プロパティ：キー名の一覧
            //=========================================================================================
            public List<KeyNameItem> KeyNameItemList {
                get {
                    return m_keyNameItemList;
                }
            }
        }

        //=========================================================================================
        // クラス：キーの定義
        //=========================================================================================
        public class KeyNameItem {
            // キーコード
            private Keys m_keyCode;

            // キーの表示名
            private string m_displayName;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]keyCode      キーコード
            // 　　　　[in]displayName  キーコードに対応する表示名
            // 戻り値：なし
            //=========================================================================================
            public KeyNameItem(Keys keyCode, string displayName) {
                m_displayName = displayName;
                m_keyCode = keyCode;
            }

            //=========================================================================================
            // プロパティ：キーコード
            //=========================================================================================
            public Keys KeyCode {
                get {
                    return m_keyCode;
                }
            }

            //=========================================================================================
            // プロパティ：キーの表示名
            //=========================================================================================
            public string DisplayName {
                get {
                    return m_displayName;
                }
            }
        }
    }
}
