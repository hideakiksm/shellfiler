using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.UI;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileList.MoveCursor;
using ShellFiler.Command.FileViewer;
using ShellFiler.Command.FileViewer.Edit;
using ShellFiler.Command.GraphicsViewer;
using ShellFiler.Command.GraphicsViewer.Edit;
using ShellFiler.Locale;

namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // プロパティ：キーの状態
    //=========================================================================================
    public class KeyState : ICloneable {
        // キー
        private Keys m_key;

        // シフトキーが押されている状態で実行されるときtrue
        private bool m_isShift;

        // コントロールキーが押されている状態で実行されるときtrue
        private bool m_isControl;

        // ALTキーが押されている状態で実行されるときtrue
        private bool m_isAlt;

        // 2ストロークキーの状態
        private TwoStrokeType m_twoStrokeKey;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]key       キー
        // 　　　　[in]isShift   シフトキーが押されている状態で実行されるときtrue
        // 　　　　[in]isControl コントロールキーが押されている状態で実行されるときtrue
        // 　　　　[in]isAlt     ALTキーが押されている状態で実行されるときtrue
        // 戻り値：なし
        //=========================================================================================
        public KeyState(Keys key, bool isShift, bool isControl, bool isAlt) {
            m_key = key;
            m_isShift = isShift;
            m_isControl = isControl;
            m_isAlt = isAlt;
            m_twoStrokeKey = TwoStrokeType.None;
        }

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]key       キー
        // 　　　　[in]twoStroke 2ストロークキーの状態
        // 戻り値：なし
        //=========================================================================================
        public KeyState(Keys key, TwoStrokeType twoStroke) {
            m_key = key;
            m_isShift = false;
            m_isControl = false;
            m_isAlt = false;
            m_twoStrokeKey = twoStroke;
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
        // 機　能：ハッシュコードを返す
        // 引　数：なし
        // 戻り値：ハッシュコード
        //=========================================================================================
        public override int GetHashCode() {
            int code = m_key.GetHashCode();
            if (m_isShift) {
                code ^= 0x1001;
            }
            if (m_isControl) {
                code ^= 0x202;
            }
            if (m_isAlt) {
                code ^= 0x44;
            }
            code ^= ((int)m_twoStrokeKey) << 3;
            return code;
        }

        //=========================================================================================
        // 機　能：比較する
        // 引　数：[in]other  比較対象
        // 戻り値：等しいときtrue
        //=========================================================================================
        public override bool Equals(object other) {
            KeyState b = (KeyState)other;
            return (this.m_key == b.m_key) && (this.m_isShift == b.m_isShift) && (this.m_isControl == b.m_isControl) && (this.m_isAlt == b.m_isAlt) && (this.m_twoStrokeKey == b.m_twoStrokeKey);
        }

        //=========================================================================================
        // 機　能：マウスのボタンであるときtrueを返す
        // 引　数：なし
        // 戻り値：マウスのボタンであるときtrue
        //=========================================================================================
        public bool IsMouse() {
            return (this.m_key == Keys.LButton || this.m_key == Keys.MButton || this.m_key == Keys.RButton);
        }

        //=========================================================================================
        // 機　能：キーの名前を返す
        // 引　数：[in]keyList    2ストロークキーの表示名を取得するキー一覧（「Meta」でよいときnull）
        // 戻り値：キーの表記（「Shift+Ctrl+A」形式）
        //=========================================================================================
        public string GetDisplayName(KeyItemSettingList keyList) {
            string name = "";
            if (m_twoStrokeKey == TwoStrokeType.None) {
                if (m_isShift) {
                    name += "Shift+";
                }
                if (m_isControl) {
                    name += "Ctrl+";
                }
                if (m_isAlt) {
                    name += "Alt+";
                }
            } else {
                string twoStrokeName = null;
                if (keyList != null) {
                    KeyState twoStrokeKey = keyList.GetTwoStrokeKey(m_twoStrokeKey);
                    if (twoStrokeKey != null) {
                        twoStrokeName = twoStrokeKey.GetDisplayName(null);
                    }
                }
                if (twoStrokeName == null) {
                    twoStrokeName = TwoStrokeKeyState.GetDisplayNameKey(m_twoStrokeKey);
                }
                name += twoStrokeName + ".";
            }
            string keyName = KeyNameUtils.GetDisplayName(m_key);
            name += keyName;
            return name;
        }

        //=========================================================================================
        // 機　能：キーの名前を返す
        // 引　数：[in]keyList    2ストロークキーの表示名を取得するキー一覧（「Meta」でよいときnull）
        // 戻り値：キーの表記（「A+Shift+Ctrl」形式）
        //=========================================================================================
        public string GetDisplayNameKey(KeyItemSettingList keyList) {
            string keyName = KeyNameUtils.GetDisplayName(m_key);
            if (m_twoStrokeKey == TwoStrokeType.None) {
                if (m_isShift) {
                    keyName += " + Shift";
                }
                if (m_isControl) {
                    keyName += " + Ctrl";
                }
                if (m_isAlt) {
                    keyName += " + Alt";
                }
            } else {
                string twoStrokeName = null;
                if (keyList != null) {
                    KeyState twoStrokeKey = keyList.GetTwoStrokeKey(m_twoStrokeKey);
                    if (twoStrokeKey != null) {
                        twoStrokeName = twoStrokeKey.GetDisplayName(null);
                    }
                }
                if (twoStrokeName == null) {
                    twoStrokeName = TwoStrokeKeyState.GetDisplayNameKey(m_twoStrokeKey);
                }
                keyName = twoStrokeName + "." + keyName;
            }
            return keyName;
        }

        //=========================================================================================
        // 機　能：指定されたモニかが2ストロークキーの開始コマンドの場合、対応する2ストロークキーの種類を返す
        // 引　数：[in]moniker  調べるモニカ
        // 戻り値：2ストロークキーの種類
        //=========================================================================================
        public static TwoStrokeType GetTwoStrokeKeyType(ActionCommandMoniker moniker) {
            TwoStrokeType result = TwoStrokeType.None;
            string monikerName = moniker.CommandType.Name;
            if (monikerName == typeof(TwoStrokeKey1Command).Name) {
                result = TwoStrokeType.Key1;
            } else if (monikerName == typeof(TwoStrokeKey2Command).Name) {
                result = TwoStrokeType.Key2;
            } else if (monikerName == typeof(TwoStrokeKey3Command).Name) {
                result = TwoStrokeType.Key3;
            } else if (monikerName == typeof(TwoStrokeKey4Command).Name) {
                result = TwoStrokeType.Key4;
            } else if (monikerName == typeof(V_TwoStrokeKey1Command).Name) {
                result = TwoStrokeType.Key1;
            } else if (monikerName == typeof(V_TwoStrokeKey2Command).Name) {
                result = TwoStrokeType.Key2;
            } else if (monikerName == typeof(V_TwoStrokeKey3Command).Name) {
                result = TwoStrokeType.Key3;
            } else if (monikerName == typeof(V_TwoStrokeKey4Command).Name) {
                result = TwoStrokeType.Key4;
            } else if (monikerName == typeof(G_TwoStrokeKey1Command).Name) {
                result = TwoStrokeType.Key1;
            } else if (monikerName == typeof(G_TwoStrokeKey2Command).Name) {
                result = TwoStrokeType.Key2;
            } else if (monikerName == typeof(G_TwoStrokeKey3Command).Name) {
                result = TwoStrokeType.Key3;
            } else if (monikerName == typeof(G_TwoStrokeKey4Command).Name) {
                result = TwoStrokeType.Key4;
            }
            return result;
        }

        //=========================================================================================
        // プロパティ：キー
        //=========================================================================================
        public Keys Key {
            get {
                return m_key;
            }
        }

        //=========================================================================================
        // プロパティ：シフトキーが押されている状態で実行されるときtrue
        //=========================================================================================
        public bool IsShift {
            get {
                return m_isShift;
            }
        }

        //=========================================================================================
        // プロパティ：コントロールキーが押されている状態で実行されるときtrue
        //=========================================================================================
        public bool IsControl {
            get {
                return m_isControl;
            }
        }

        //=========================================================================================
        // プロパティ：ALTキーが押されている状態で実行されるときtrue
        //=========================================================================================
        public bool IsAlt {
            get {
                return m_isAlt;
            }
        }
        
        //=========================================================================================
        // プロパティ：2ストロークキーの状態
        //=========================================================================================
        public TwoStrokeType TwoStrokeKey {
            get {
                return m_twoStrokeKey;
            }
        }
    }
}
