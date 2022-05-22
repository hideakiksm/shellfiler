using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Util;

namespace ShellFiler.Util {

    //=========================================================================================
    // クラス：テキストレンダリング関連のユーティリティ
    //=========================================================================================
    public class TextRendererUtils {

        //=========================================================================================
        // 機　能：文字列の領域を計測する
        // 引　数：[in]g       グラフィックス
        // 　　　　[in]font    フォント
        // 　　　　[in]strLine 計測する文字列
        // 　　　　[in]range   計測する文字列の範囲の配列
        // 戻り値：文字列の幅の配列（開始0を除く、rangeに対応する座標、range.Lengthと同じ長さ）
        // メ　モ：範囲の配列は[開始,長さ]で[0,2], [2,0], [2,3]を計測する場合、[2, 2, 5]を指定、開始0は不要）
        //=========================================================================================
        public static float[] MeasureStringRegion(Graphics g, Font font, string strLine, params int[] range) {
            if (strLine.Length == 0) {
                float[] result = new float[range.Length];
                for (int i = 0; i < result.Length; i++) {
                    result[i] = 0;
                }
                return result;
            } else {
                if (strLine.IndexOf((char)0xfffd) != -1) {       // 不明な文字は誤動作するため「.」に置換
                    strLine = strLine.Replace((char)0xfffd, '.');
                }
                const int MAX_RANGE_COUNT = 30;
                List<float> result = new List<float>();
                int idxRange = 0;
                int lastRange = 0;
                float lastResult = 0;
                while (idxRange < range.Length) {
                    CharacterRange[] rangeList = new CharacterRange[Math.Min(MAX_RANGE_COUNT, range.Length - idxRange)];
                    for (int i = 0; i < rangeList.Length; i++) {
                        rangeList[i] = new CharacterRange(lastRange, range[idxRange] - lastRange);
                        lastRange = range[idxRange];
                        idxRange++;
                    }
                    StringFormat sf = new StringFormat();
                    sf.SetMeasurableCharacterRanges(rangeList);
                    Region[] charRegion = g.MeasureCharacterRanges(strLine + ".", font, new RectangleF(0, 0, GraphicsUtils.INFINITY_WIDTH, font.Height * 2), sf);
                    sf.Dispose();

                    for (int i = 0; i < charRegion.Length; i++) {
                        float pos = Math.Max(lastResult, charRegion[i].GetBounds(g).Right);
                        result.Add(pos);
                        lastResult = pos;
                    }
                }
                return result.ToArray();
            }
        }

        //=========================================================================================
        // 機　能：文字列の表示幅を計算する
        // 引　数：[in]g       グラフィックス
        // 　　　　[in]font    フォント
        // 　　　　[in]strLine 計測する文字列
        // 戻り値：文字列の表示幅
        //=========================================================================================
        public static float MeasureStringJust(Graphics g, Font font, string strLine) {
            if (strLine.IndexOf((char)0xfffd) != -1) {       // 不明な文字は誤動作するため「.」に置換
                strLine = strLine.Replace((char)0xfffd, '.');
            }
            CharacterRange[] rangeList = new CharacterRange[1];
            rangeList[0] = new CharacterRange(0, strLine.Length);
            StringFormat sf = new StringFormat();
            sf.SetMeasurableCharacterRanges(rangeList);
            Region[] charRegion = g.MeasureCharacterRanges(strLine + ".", font, new RectangleF(0, 0, GraphicsUtils.INFINITY_WIDTH, font.Height * 2), sf);
            sf.Dispose();
            float size = charRegion[0].GetBounds(g).Right;
            return size;
        }

        //=========================================================================================
        // 機　能：文字列の表示幅を計算してint型に変換して返す
        // 引　数：[in]g       グラフィックス
        // 　　　　[in]font    フォント
        // 　　　　[in]strLine 計測する文字列
        // 戻り値：文字列の表示幅
        //=========================================================================================
        public static int MeasureStringJustInt(HighDpiGraphics g, Font font, string strLine) {
            int size = (int) Math.Ceiling(MeasureStringJust(g.Graphics, font, strLine));
            return size;
        }
    }
}
