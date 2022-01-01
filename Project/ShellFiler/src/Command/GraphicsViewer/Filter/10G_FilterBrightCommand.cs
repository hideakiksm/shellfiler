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
    // 明るさフィルターの明るさを明るさ{0}、コントラスト{1}、ガンマ{2}で適用します。
    //   書式 　 G_FilterBright(int bright, int contrast, int gamma)
    //   引数  　bright:明るさレベルの差分(-100%～100%)
    // 　　　　　bright-default:0
    // 　　　　　bright-range:-100,100
    // 　　　　　contrast:コントラストレベルの差分(-100%～100%)
    // 　　　　　contrast-default:0
    // 　　　　　contrast-range:-100,100
    // 　　　　　gamma:ガンマの100倍の差分(-100～100)
    // 　　　　　gamma-default:0
    // 　　　　　gamma-range:-100,100
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_FilterBrightCommand : GraphicsViewerActionCommand {
        // 明るさの差分
        private float m_brightDelta;

        // コントラストの差分
        private float m_contrastDelta;

        // ガンマの差分
        private float m_gammaDelta;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_FilterBrightCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            int iLevelDelta = (int)param[0];
            m_brightDelta = (float)iLevelDelta / 100.0f;
            int iContrastDelta = (int)param[1];
            m_contrastDelta = (float)iContrastDelta / 100.0f;
            int iGammaDelta = (int)param[2];
            m_gammaDelta = (float)iGammaDelta / 100.0f;
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            object[] deltaList = new object[3];
            deltaList[0] = m_brightDelta;
            deltaList[1] = m_contrastDelta;
            deltaList[2] = m_gammaDelta;
            G_FilterBlurCommand.ModifyFilterParam(GraphicsViewerPanel, typeof(ComponentBright), deltaList);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_FilterBrightCommand;
            }
        }
    }
}
