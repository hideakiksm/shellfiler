using System;
using System.Reflection;
using ShellFiler.Api;
using ShellFiler.FileViewer;

namespace ShellFiler.Command {
    
    //=========================================================================================
    // クラス：ActionCommand生成のためのクラス名とパラメータの組
    //=========================================================================================
    public class ActionCommandMoniker : ICloneable {
        // オプション
        private ActionCommandOption m_option;

        // コマンドのクラス名（パッケージ名あり）
        private Type m_type;

        // パラメータ
        private object[] m_param;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]option  オプション
        //         [in]type    コマンドのクラスのType
        // 　　　　[in]param   パラメータ
        // 戻り値：なし
        //=========================================================================================
        public ActionCommandMoniker(ActionCommandOption option, Type type, params object[] param) {
            m_option = option;
            m_type = type;
            m_param = param;
        }
        
        //=========================================================================================
        // 機　能：ActionCommandを作成する
        // 引　数：なし
        // 戻り値：作成したActionCommand
        //=========================================================================================
        public ActionCommand CreateActionCommand() {
            ActionCommand command = (ActionCommand)(m_type.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
            command.SetParameter(m_param);
            return command;
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
            int code = m_type.GetHashCode();
            if (m_param != null) {
                code ^= m_param.Length;
                foreach (object value in m_param) {
                    if (value != null) {
                        code ^= value.GetHashCode();
                    }
                }
            }
            return code;
        }

        //=========================================================================================
        // 機　能：比較する
        // 引　数：[in]other  比較対象
        // 戻り値：等しいときtrue
        //=========================================================================================
        public override bool Equals(object other) {
            if (!(other is ActionCommandMoniker)) {
                return false;
            }
            ActionCommandMoniker a = this;
            ActionCommandMoniker b = (ActionCommandMoniker)other;
            if (a.m_type.FullName != b.m_type.FullName) {
                return false;
            }
            if (a.m_param != null && b.m_param == null) {
                return false;
            } else if (a.m_param == null && b.m_param != null) {
                return false;
            } else if (a.m_param == null && b.m_param == null) {
                return true;
            } else {
                if (a.m_param.Length != b.m_param.Length) {
                    return false;
                }
                for (int i = 0; i < a.m_param.Length; i++) {
                    if (a.m_param[i] == null && b.m_param[i] == null) {
                        ;
                    } else if (a.m_param[i] == null && b.m_param[i] != null) {
                        return false;
                    } else if (a.m_param[i] != null && b.m_param[i] == null) {
                        return false;
                    } else if (!a.m_param[i].Equals(b.m_param[i])) {
                        return false;
                    }
                }
                return true;
            }
        }

        //=========================================================================================
        // 機　能：比較する
        // 引　数：[in]other  比較対象
        // 戻り値：等しいときtrue
        //=========================================================================================
        public static bool Equals(ActionCommandMoniker obj1, ActionCommandMoniker obj2) {
            if (obj1 == null && obj2 == null) {
                return true;
            }
            if (obj1 == null || obj2 == null) {
                return false;
            }
            return obj1.Equals(obj2);
        }

        //=========================================================================================
        // プロパティ：オプション
        //=========================================================================================
        public ActionCommandOption Option {
            get {
                return m_option;
            }
        }

        //=========================================================================================
        // プロパティ：コマンドのクラス
        //=========================================================================================
        public Type CommandType {
            get {
                return m_type;
            }
        }

        //=========================================================================================
        // プロパティ：パラメータ
        //=========================================================================================
        public object[] Parameter {
            get {
                return m_param;
            }
        }
    }
}
