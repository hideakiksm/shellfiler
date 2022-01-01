using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using ShellFiler.Api;
using ShellFiler.UI.Log;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.Locale;

namespace ShellFiler.FileViewer {

    //=========================================================================================
    // クラス：ダンプ検索の検索コア
    //=========================================================================================
    public class DumpSearchCore {

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public DumpSearchCore() {
        }

        //=========================================================================================
        // 機　能：1行の中からバイト列を前方検索する
        // 引　数：[in]searchBytes  検索バイト列
        // 　　　　[in]targetBuffer 検索対象のバイト列
        // 　　　　[in]start        検索開始位置
        // 　　　　[in]fileSize     検索するファイルのサイズ
        // 　　　　[in]searchEnd    検索終了アドレス
        // 戻り値：検索にヒットしたアドレス（-1:これ以上ヒットなし）
        //=========================================================================================
        public int ForwardSearchHitAddress(byte[] searchBytes, byte[] targetBuffer, int start, int fileSize, int searchEnd) {
            int startLoop = Math.Max(0, start - searchBytes.Length + 1);
            int endLoop = Math.Min(fileSize - searchBytes.Length, searchEnd);
            int keywordLen = searchBytes.Length;
            for (int i = startLoop; i <= endLoop; i++) {
                bool match = true;
                for (int j = 0; j < keywordLen; j++) {
                    if (searchBytes[j] != targetBuffer[i + j]) {
                        match = false;
                        break;
                    }
                }
                if (match) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：1行の中からバイト列を後方検索する
        // 引　数：[in]searchBytes  検索バイト列
        // 　　　　[in]targetBuffer 検索対象のバイト列
        // 　　　　[in]start        検索開始位置（末端アドレス）
        // 　　　　[in]fileSize     検索するファイルのサイズ
        // 戻り値：検索にヒットしたアドレス（末端位置、-1:これ以上ヒットなし）
        //=========================================================================================
        public int ReverseSearchHitAddress(byte[] searchBytes, byte[] targetBuffer, int start, int fileSize) {
            int startLoop = Math.Min(fileSize - 1, start + searchBytes.Length - 1);
            int endLoop = searchBytes.Length - 1;
            int keywordLen = searchBytes.Length;
            for (int i = startLoop; i >= endLoop; i--) {
                bool match = true;
                for (int j = 0; j < keywordLen; j++) {
                    if (searchBytes[j] != targetBuffer[i + j - keywordLen + 1]) {
                        match = false;
                        break;
                    }
                }
                if (match) {
                    return i;
                }
            }
            return -1;
        }

        //=========================================================================================
        // 機　能：バイト単位でのヒット位置をセットする
        // 引　数：[in]condition   検索条件
        // 　　　　[in]buffer      検索対象のバッファ
        // 　　　　[in]hitPosition ヒット位置の格納対象
        // 戻り値：なし
        //=========================================================================================
        public void SetDumpByteHitPosition(DumpSearchCondition condition, byte[] buffer, DumpSearchHitPosition hitPosition) {
            // 検索バイト列で検索
            if (condition.SearchBytes != null && condition.SearchBytes.Length > 0) {
                int address = hitPosition.StartAddress;
                int endAddress = hitPosition.EndAddress;
                while (address <= endAddress) {
                    int hitAddress = ForwardSearchHitAddress(condition.SearchBytes, buffer, address, hitPosition.FileSize, hitPosition.EndAddress);
                    if (hitAddress == -1) {
                        break;
                    }
                    hitPosition.FillHitState(hitAddress, condition.SearchBytes.Length, true);
                    address = Math.Max(address + 1, hitAddress + 1);
                }
            }

            // 自動検索バイト列で検索
            if (condition.AutoSearchBytes != null && condition.AutoSearchBytes.Length > 0) {
                int address = hitPosition.StartAddress;
                int endAddress = hitPosition.EndAddress;
                while (address <= endAddress) {
                    int hitAddress = ForwardSearchHitAddress(condition.AutoSearchBytes, buffer, address, hitPosition.FileSize, hitPosition.EndAddress);
                    if (hitAddress == -1) {
                        break;
                    }
                    hitPosition.FillHitState(hitAddress, condition.AutoSearchBytes.Length, false);
                    address = Math.Max(address + 1, hitAddress + 1);
                }
            }
        }
    }
}
