using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ShellFiler.FileSystem;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：アイコンサイズ
    //=========================================================================================
    public class IconSize {
        public static readonly IconSize Small16 = new IconSize(0, 16, 16, 0x1);
        public static readonly IconSize Large32 = new IconSize(1, 32, 32, 0x0);
        public static readonly IconSize ExtraLarge48 = new IconSize(2, 48, 48, 0x2);
        public static readonly IconSize Jumbo256 = new IconSize(3, 256, 256, 0x4);

        // 順序づけ
        private int m_order;

        // アイコンの幅
        private int m_cxIconSize;

        // アイコンの高さ
        private int m_cyIconSize;

        // APIへのサイズ指定
        private int m_apiSize;
    
        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]order       順序づけ
        // 　　　　[in]cxIconSize  アイコンの幅
        // 　　　　[in]cyIconSize  アイコンの高さ
        // 　　　　[in]apiSize     APIへのサイズ指定
        // 戻り値：なし
        //=========================================================================================
        private IconSize(int order, int cxIconSize, int cyIconSize, int apiSize) {
            m_order = order;
            m_cxIconSize = cxIconSize;
            m_cyIconSize = cyIconSize;
            m_apiSize = apiSize;
        }

        //=========================================================================================
        // 機　能：内部に表示できるアイコンサイズを取得する
        // 引　数：[in]imageSize  画像のサイズ
        // 戻り値：なし
        //=========================================================================================
        public static IconSize GetInsideIconSize(Size imageSize) {
            int size = Math.Min(imageSize.Width, imageSize.Height);
            if (size < IconSize.Large32.CxIconSize) {
                return IconSize.Small16;
            } else if (size < IconSize.ExtraLarge48.CxIconSize) {
                return IconSize.Large32;
            } else if (size < IconSize.Jumbo256.CxIconSize) {
                return IconSize.ExtraLarge48;
            } else {
                return IconSize.Jumbo256;
            }
        }

        //=========================================================================================
        // プロパティ：すべてのサイズの列挙
        //=========================================================================================
        public static IconSize[] AllSize {
            get {
                IconSize[] allSize = new IconSize[] {
                    Small16,
                    Large32,
                    ExtraLarge48,
                    Jumbo256,
                };
                return allSize;
            }
        }

        //=========================================================================================
        // プロパティ：順序づけ
        //=========================================================================================
        public int Order {
            get {
                return m_order;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンの幅
        //=========================================================================================
        public int CxIconSize {
            get {
                return m_cxIconSize;
            }
        }

        //=========================================================================================
        // プロパティ：アイコンの高さ
        //=========================================================================================
        public int CyIconSize {
            get {
                return m_cyIconSize;
            }
        }

        //=========================================================================================
        // プロパティ：APIへのサイズ指定
        //=========================================================================================
        public int ApiSize {
            get {
                return m_apiSize;
            }
        }
    }
}
