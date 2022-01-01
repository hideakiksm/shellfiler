using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using ShellFiler.Api;
using ShellFiler.FileViewer;

namespace ShellFiler.GraphicsViewer {
    
    //=========================================================================================
    // クラス：イメージの拡大と縮小の倍率を変換するクラス
    // 　　　　MinZoomKey～0～MaxZoomKeyを指定すると、それに対応する倍率を取得する
    //=========================================================================================
    class ZoomLevel {
        // ズームの最大倍率
        public double ZOOM_RATIO_MAX = 16.0;

        // ズーム指定キーの範囲(0～ZOOM_KEY_RANGE)
        private int ZOOM_KEY_RANGE = 2000;

        // ズーム値の計算済みテーブル（中央がキー0に対応、計算済みでないときnull）
        private static float[] s_zoomTable = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ZoomLevel() {
            if (s_zoomTable == null) {
                float[] zoomTable = new float[ZOOM_KEY_RANGE + 1];
                double halfRange = ZOOM_KEY_RANGE / 2.0;
                for (int i = 0; i < zoomTable.Length; i++) {
                    zoomTable[i] = (float)(Math.Pow(ZOOM_RATIO_MAX, i / halfRange) / ZOOM_RATIO_MAX);
                }
                s_zoomTable = zoomTable;
            }
        }

        //=========================================================================================
        // 機　能：ズーム倍率を取得する
        // 引　数：[in]zoomKey  ズームのキーとなる値
        // 戻り値：ズーム倍率
        //=========================================================================================
        public float GetZoomRatio(int zoomKey) {
            int key = Math.Min(ZOOM_KEY_RANGE, Math.Max(0, zoomKey - MinZoomKey));
            return s_zoomTable[key];
        }

        //=========================================================================================
        // 機　能：ズーム倍率からズームキーを取得する
        // 引　数：[in]zoomRatio  ズーム倍率
        // 　　　　[in]delta      近似値の計算方法(-1または1)
        // 戻り値：ズームキー
        //=========================================================================================
        public int ZoomRatioToZoomKey(float zoomRatio, int delta) {
            if (zoomRatio == 1.0f) {
                return 0;
            } else if (zoomRatio == s_zoomTable[0]) {
                return MinZoomKey;
            } else if (zoomRatio == s_zoomTable[s_zoomTable.Length - 1]) {
                return MaxZoomKey;
            }

            if (delta == 1) {
                for (int i = 0; i < s_zoomTable.Length - 1; i++) {
                    if (s_zoomTable[i] > zoomRatio) {
                        return i - ZOOM_KEY_RANGE / 2 - 1;
                    }
                }
                return MaxZoomKey;
            } else {
                for (int i = s_zoomTable.Length - 1; i >= 0; i--) {
                    if (s_zoomTable[i] < zoomRatio) {
                        return i - ZOOM_KEY_RANGE / 2 + 1;
                    }
                }
                return MinZoomKey;
            }
        }

        //=========================================================================================
        // プロパティ：ズーム指定の最小値
        //=========================================================================================
        public int MinZoomKey {
            get {
                return -ZOOM_KEY_RANGE / 2;
            }
        }

        //=========================================================================================
        // プロパティ：ズーム指定の最大値
        //=========================================================================================
        public int MaxZoomKey {
            get {
                return ZOOM_KEY_RANGE / 2 + 1;
            }
        }

        //=========================================================================================
        // プロパティ：ズームの最小値
        //=========================================================================================
        public float MinZoomValue {
            get {
                return s_zoomTable[0];
            }
        }

        //=========================================================================================
        // プロパティ：ズームの最大値
        //=========================================================================================
        public float MaxZoomValue {
            get {
                return s_zoomTable[s_zoomTable.Length - 1];
            }
        }
    }
}
