using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.FileSystem;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Provider;
using ShellFiler.Command.FileList;
using ShellFiler.GraphicsViewer;
using ShellFiler.GraphicsViewer.Filter;
using ShellFiler.Util;
using ShellFiler.UI;
using ShellFiler.UI.Dialog.GraphicsViewer;

namespace ShellFiler.Command.GraphicsViewer.Filter {

    //=========================================================================================
    // クラス：コマンドを実行する
    // シャープフィルターをレベル{0}で適用します。
    //   書式 　 G_FilterSharp(int level)
    //   引数  　level:適用レベルの差分(-100%～100%)
    // 　　　　　level-default:0
    // 　　　　　level-range:-100,100
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_FilterSharpCommand : GraphicsViewerActionCommand {
        // 適用レベルの差分
        private float m_levelDelta;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_FilterSharpCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            int iLevelDelta = (int)param[0];
            m_levelDelta = (float)iLevelDelta / 100.0f;
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            object[] deltaList = new object[1];
            deltaList[0] = m_levelDelta;
            G_FilterBlurCommand.ModifyFilterParam(GraphicsViewerPanel, typeof(ComponentSharp), deltaList);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_FilterSharpCommand;
            }
        }
    }
}
