﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Properties;
using ShellFiler.FileViewer;

namespace ShellFiler.FileTask.DataObject {

    //=========================================================================================
    // インターフェース：取得データの格納先インターフェース
    //=========================================================================================
    public interface IRetrieveFileDataTarget {
        
        //=========================================================================================
        // 機　能：新しいデータを追加する
        // 引　数：[in]buffer  データの入ったバッファ
        // 　　　　[in]offset  buffer中のオフセット
        // 　　　　[in]length  データの長さ
        // 戻り値：なし
        // メ　モ：読み込みスレッドまたはその他外部の作業スレッドからの呼び出しを想定
        //=========================================================================================
        void AddData(byte[] buffer, int offset, int length);

        //=========================================================================================
        // 機　能：データの追加が終わったことを通知する
        // 引　数：[in]status    読み込み状況のステータス
        // 　　　　[in]errorInfo status=Failedのときエラー情報の文字列、それ以外はnull
        // 戻り値：なし
        // メ　モ：読み込みスレッドまたはその他外部の作業スレッドからの呼び出しを想定
        //=========================================================================================
        void AddCompleted(RetrieveDataLoadStatus status, string errorInfo); 
       
        //=========================================================================================
        // 機　能：受信したときのイベントを発行する
        // 引　数：[in]final   最後のイベント通知のときtrue
        // 戻り値：なし
        // メ　モ：読み込みスレッドからの呼び出しを想定
        //=========================================================================================
        void FireEvent(bool final);
    }
}
