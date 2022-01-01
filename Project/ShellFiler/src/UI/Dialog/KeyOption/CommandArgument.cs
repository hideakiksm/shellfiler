using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：引数の定義
    //=========================================================================================
    public class CommandArgument {
        // 仮引数名
        public string ArgumentName;

        // 引数の説明
        public string ArgumentComment;

        // 引数の型
        public ArgumentType Type;

        // 引数のデフォルト値
        public object DefaultValue;

        // 引数の範囲（int型最大値）
        public int ValueRangeIntMax;
        
        // 引数の範囲（int型最小値）
        public int ValueRangeIntMin;
        
        // 引数の範囲（string型の取り得る値、自由入力のときは空、string以外のときはnull）
        public List<CommandArgument.StringSelect> ValueRangeString;

        //=========================================================================================
        // クラス：引数の型
        //=========================================================================================
        public enum ArgumentType {
            TypeString,             // string型
            TypeInt,                // int型
            TypeBool,               // bool型
            TypeMenuItem,           // メニュー項目型
        }

        //=========================================================================================
        // クラス：文字列型の引数の選択肢
        //=========================================================================================
        public class StringSelect {
            // 表示名
            public string DisplayName;

            // 実際の値
            public string InnerValue;
        }
    }
}
