using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Properties;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.Virtual;

namespace ShellFiler.FileTask.ArgumentConverter {

    //=========================================================================================
    // クラス：プログラム引数の構成要素
    //=========================================================================================
    public class ArgumentElement {
        // 項目の種類
        private ArgumentElementType m_type;

        // 項目に対する修飾子
        private ArgumentElementDecorator m_decorator;

        // 種類が文字列のときの値（文字列以外のときはnull）
        private string m_stringValue;

        // 元の文字列
        private string m_originalPart;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]type          項目の種類
        // 　　　　[in]decorator     項目に対する修飾子
        // 　　　　[in]stringValue   種類が文字列のときの値（文字列以外のときはnull）
        // 　　　　[in]originalPart  元の文字列
        // 戻り値：なし
        //=========================================================================================
        public ArgumentElement(ArgumentElementType type, ArgumentElementDecorator decorator, string stringValue, string originalPart) {
            m_type = type;
            m_decorator = decorator;
            m_stringValue = stringValue;
            m_originalPart = originalPart;
        }

        //=========================================================================================
        // プロパティ：項目の種類
        //=========================================================================================
        public ArgumentElementType Type {
            get {
                return m_type;
            }
        }

        //=========================================================================================
        // プロパティ：項目に対する修飾子
        //=========================================================================================
        public ArgumentElementDecorator Decorator {
            get {
                return m_decorator;
            }
        }

        //=========================================================================================
        // プロパティ：種類が文字列のときの値（文字列以外のときはnull）
        //=========================================================================================
        public string StringValue {
            get {
                return m_stringValue;
            }
        }

        //=========================================================================================
        // プロパティ：元の文字列
        //=========================================================================================
        public string OriginalPart {
            get {
                return m_originalPart;
            }
        }
    }
}
