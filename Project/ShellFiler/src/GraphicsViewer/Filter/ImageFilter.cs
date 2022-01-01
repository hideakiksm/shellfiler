using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.GraphicsViewer;
using ShellFiler.Util;

namespace ShellFiler.GraphicsViewer.Filter {
    
    //=========================================================================================
    // クラス：イメージに適用するフィルター
    //=========================================================================================
    public class ImageFilter {
        // 使用するフィルタのリスト
        private IFilterComponent[] m_filterList = null;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public ImageFilter() {
        }
        
        //=========================================================================================
        // 機　能：フィルターを初期化する
        // 引　数：[in]filterSetting  フィルター設定
        // 　　　　[in]resetFilter  フィルターをリセットするときtrue
        // 　　　　[in]changeImage  画像が変更されたときtrue
        // 戻り値：なし
        //=========================================================================================
        public void InitializeFilter(GraphicsViewerFilterSetting filterSetting, bool resetFilter, bool changeImage) {
            GraphicsViewerFilterMode mode = Configuration.Current.GraphicsViewerFilterMode;
            if (resetFilter || mode == GraphicsViewerFilterMode.AllImages) {
                // すべての画像にフィルターを適用する
                if (filterSetting.UseFilter) {
                    IFilterComponent[] filterList = new IFilterComponent[filterSetting.FilterList.Count];
                    for (int i = 0; i < filterSetting.FilterList.Count; i++) {
                        GraphicsViewerFilterItem settingItem = filterSetting.FilterList[i];
                        filterList[i] = (IFilterComponent)(settingItem.FilterClass.InvokeMember(null, BindingFlags.CreateInstance, null, null, null));
                        filterList[i].SetParameter(settingItem.FilterParameter);
                    }
                    SetFilter(filterList);
                } else {
                    filterSetting.UseFilter = false;
                    SetFilter(new IFilterComponent[0]);
                }
            } else if (mode == GraphicsViewerFilterMode.CurrentImageOnly) {
                // 表示中の画像だけにフィルターを適用する
                if (m_filterList == null) {
                    filterSetting.UseFilter = false;
                    SetFilter(new IFilterComponent[0]);
                } else if (changeImage) {
                    filterSetting.UseFilter = false;
                    SetFilter(new IFilterComponent[0]);
                }
            } else if (mode == GraphicsViewerFilterMode.CurrentWindowImages) {
                // 設定したウィンドウの画像だけにフィルターを適用する
                if (m_filterList == null) {
                    filterSetting.UseFilter = false;
                    SetFilter(new IFilterComponent[0]);
                }
            }
        }

        //=========================================================================================
        // 機　能：フィルターを設定する
        // 引　数：[in]filterList  使用するフィルタのリスト
        // 戻り値：なし
        //=========================================================================================
        public void SetFilter(IFilterComponent[] filterList) {
            m_filterList = filterList;
        }

        //=========================================================================================
        // 機　能：フィルターを適用する
        // 引　数：[in]bitmap  適用するイメージ
        // 戻り値：なし
        //=========================================================================================
        public void ApplyFilter(Bitmap bitmap) {
            // 画像データを取得
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr bitmapPtr = bitmapData.Scan0;
            byte[] srcData = new byte[bitmapData.Stride * bitmap.Height];
            Marshal.Copy(bitmapPtr, srcData, 0, srcData.Length);

            // 準備
            byte[] destData = new byte[bitmapData.Stride * bitmap.Height];
            int cpuCount = OSUtils.GetCpuCoreCount();
            int width = bitmap.Width;
            int height = bitmap.Height;
            int stride = bitmapData.Stride;
            int[] startLineList = DivideImageLine(height, cpuCount);

            // 作業スレッドにリクエスト
            for (int i = 0; i < m_filterList.Length; i++) {
                m_filterList[i].Initialize(srcData, destData, width, height, stride);
                List<ManualResetEvent> waitEventList = new List<ManualResetEvent>();
                for (int j = 0; j < cpuCount; j++) {
                    if (j >= startLineList.Length - 1) {
                        break;
                    }
                    int startLine = startLineList[j];
                    int endLine = startLineList[j + 1] - 1;
                    ManualResetEvent endEvent = new ManualResetEvent(false);
                    FilterThread filterThread = Program.Document.GraphicsViewerFilterThread;
                    filterThread.Request(m_filterList[i], startLine, endLine, endEvent);
                    waitEventList.Add(endEvent);
                }
                foreach (ManualResetEvent endEventWait in waitEventList) {
                    endEventWait.WaitOne();
                }

                // 次のフィルターではsrcとdestを入れ替え
                byte[] temp = destData;
                destData = srcData;
                srcData = temp;
            }

            // 元の画像に反映
            Marshal.Copy(srcData, 0, bitmapPtr, srcData.Length);
            bitmap.UnlockBits(bitmapData);
        }

        //=========================================================================================
        // 機　能：画像の行をCPUに割り当てる
        // 引　数：[in]height    画像の高さ
        // 　　　　[in]cpuCount  CPUの数
        // 戻り値：6行を4CPUに割り当てる場合、{0,2,3,5,6}が返る（CPU数+1に全体数）
        //=========================================================================================
        private int[] DivideImageLine(int height, int cpuCount) {
            int[] result;
            if (height <= cpuCount) {
                // 行数がCPU数より少ない場合は必要分だけ処理
                result = new int[height + 1];
                for (int i = 0; i <= height; i++) {
                    result[i] = i;
                }
            } else {
                // 行数をCPU数で均等に割り当て
                result = new int[cpuCount + 1];
                float currentY = 0.0f;
                float lineStep = (float)height / (float)cpuCount;       // 必ず1以上
                for (int i = 0; i < cpuCount; i++) {
                    result[i] = (int)currentY;
                    currentY += lineStep;
                }
                result[cpuCount] = height;
            }
            return result;
        }

        //=========================================================================================
        // プロパティ：フィルターの一覧
        //=========================================================================================
        public static Type[] FilterTypeList {
            get {
                Type[] filterList = {
                    typeof(ComponentBright),
                    typeof(ComponentHsvModify),
                    typeof(ComponentMonochrome),
                    typeof(ComponentNegative),
                    typeof(ComponentSepia),
                    typeof(ComponentSharp),
                    typeof(ComponentBlur),
                    typeof(ComponentRelief),
                };
                return filterList;
            }
        }
    }
}
