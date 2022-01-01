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
    // 色の調整フィルターを色相{0}、彩度{1}、明度{2}で適用します。
    //   書式 　 G_FilterHsvModify(int hvalue, int svalue, int vvalue)
    //   引数  　hvalue:色相の差分(-180～180)
    // 　　　　　hvalue-default:0
    // 　　　　　hvalue-range:-180,180
    //   　　  　svalue:彩度の差分(-100%～100%)
    // 　　　　　svalue-default:0
    // 　　　　　svalue-range:-100,100
    //   　　  　vvalue:明度の差分(-100～100%)
    // 　　　　　vvalue-default:0
    // 　　　　　vvalue-range:-100,100
    //   戻り値　なし
    //   対応Ver 0.0.1
    //=========================================================================================
    class G_FilterHsvModifyCommand : GraphicsViewerActionCommand {
        // 色相の差分
        private float m_hDelta;

        // 彩度の差分
        private float m_sDelta;

        // 明度の差分
        private float m_vDelta;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public G_FilterHsvModifyCommand() {
        }

        //=========================================================================================
        // 機　能：パラメータをセットする
        // 引　数：[in]param  セットするパラメータ
        // 戻り値：なし
        //=========================================================================================
        public override void SetParameter(params object[] param) {
            int iHDelta = (int)param[0];
            m_hDelta = (float)iHDelta;
            int iSDelta = (int)param[1];
            m_sDelta = (float)iSDelta / 100.0f;
            int iVDelta = (int)param[2];
            m_vDelta = (float)iVDelta / 100.0f;
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            object[] deltaList = new object[3];
            deltaList[0] = m_hDelta;
            deltaList[1] = m_sDelta;
            deltaList[2] = m_vDelta;
            G_FilterBlurCommand.ModifyFilterParam(GraphicsViewerPanel, typeof(ComponentHsvModify), deltaList);
            return null;
        }

        //=========================================================================================
        // プロパティ：コマンドのUI表現
        //=========================================================================================
        public override UIResource UIResource {
            get {
                return UIResource.G_FilterHsvModifyCommand;
            }
        }
    }
}
