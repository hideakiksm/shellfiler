using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShellFiler.Api;
using ShellFiler.Document.OSSpec;
using ShellFiler.Util;

namespace ShellFiler.Terminal.TerminalSession.CommandEmulator {

    //=========================================================================================
    // クラス：トークンの解析を行う:属性の出力 drwxrwxrwx+（戻り=FileAttributeLinux:属性）
    //=========================================================================================
    public class OSSpecParserLsAttr : IOSSpecTokenParser {

        //=========================================================================================
        // 機　能：構文解析を行う
        // 引　数：[in]line          コマンドの実行結果の1行分
        // 　　　　[in]expect        期待値の設定
        // 　　　　[in,out]parsePos  解析開始位置（次の解析位置を返す）
        // 　　　　[out]value        解析の結果取得した値
        // 戻り値：解析に成功したときtrue
        //=========================================================================================
        public bool ParseToken(string line, OSSpecColumnExpect expect, ref int parsePos, out object value) {
            // drwxrwxrwx+  "+"を含めて11文字
            value = null;
            if (line.Length - parsePos < 10) {
                return false; 
            }
            int startPos = parsePos;
            bool isExec = false;
            bool isLink = false;
            int sortOrder = 0;
            FileAttributes attr = FileAttributes.Normal;
            int permissions = 0;

            // ディレクトリ/シンボリックリンク
            if (line[parsePos] == 'd') {
                attr |= FileAttributes.Directory;
                sortOrder += 1;
            } else if (line[parsePos] == 's' || line[parsePos] == 'l') {
                isLink = true;
                sortOrder += 2;
            } else if (line[parsePos] != '-') {
                return false;
            }
            parsePos++;

            // オーナー
            int permissionDigit = 0x40;
            sortOrder *= 10;
            if (line[parsePos] == 'r') {
                sortOrder += 1;
                permissions += 4 * permissionDigit;
            } else if (line[parsePos] != '-') {
                return false;
            }
            parsePos++;

            sortOrder *= 10;
            if (line[parsePos] == 'w') {
                sortOrder += 1;
                permissions += 2 * permissionDigit;
            } else if (line[parsePos] == '-') {
                attr |= FileAttributes.ReadOnly;
            } else {
                return false;
            }
            parsePos++;

            sortOrder *= 10;
            if (line[parsePos] == 'x') {
                isExec = true;
                sortOrder += 1;
                permissions += 1 * permissionDigit;
            } else if (line[parsePos] == 't') {
                ;
            } else if (line[parsePos] != '-') {
                return false;
            }
            parsePos++;

            // グループ/他人
            for (int i = 0; i < 2; i++) {
                permissionDigit = (permissionDigit >> 3);
                sortOrder *= 10;
                if (line[parsePos] == 'r') {
                    sortOrder += 1;
                    permissions += 4 * permissionDigit;
                } else if (line[parsePos] != '-') {
                    return false;
                }
                parsePos++;

                sortOrder *= 10;
                if (line[parsePos] == 'w') {
                    sortOrder += 1;
                    permissions += 2 * permissionDigit;
                } else if (line[parsePos] != '-') {
                    return false;
                }
                parsePos++;

                sortOrder *= 10;
                if (line[parsePos] == 'x') {
                    sortOrder += 1;
                    permissions += 1 * permissionDigit;
                } else if (line[parsePos] == 't') {
                    ;
                } else if (line[parsePos] != '-') {
                    return false;
                }
                parsePos++;
            }

            // 拡張
            if (parsePos < line.Length && (line[parsePos] == '+' || line[parsePos] == '-')) {
                parsePos++;
            }
            string strAttr = line.Substring(startPos, parsePos - startPos);
            FileAttribute objAttr = FileAttribute.FromLinuxStringPermissions(strAttr, attr, isExec, isLink, sortOrder);
            value = new FileAttributeLinux(objAttr, permissions);

            return true;
        }
    }
}
