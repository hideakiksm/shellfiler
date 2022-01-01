
namespace ShellFiler.Document.Setting {

    //=========================================================================================
    // クラス：設定のタグ種別
    //=========================================================================================
    public enum SettingTagType {
        None,                                   // 値なし（エラー、不明など）
        EndOfFile,                              // EOF
        BeginObject,                            // オブジェクト開始
        EndObject,                              // オブジェクト終了
        StringValue,                            // 文字列の値
        PasswordValue,                          // パスワードの値
        ColorValue,                             // 色の値
        RectangleValue,                         // 領域の値
        IntValue,                               // 数値の値
        FloatValue,                             // floatの値
        BoolValue,                              // boolの値
        LongValue,                              // longの値
    }
}
