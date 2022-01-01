using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document.Setting;
using ShellFiler.UI;
using ShellFiler.UI.FileList;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.FileViewer;
using ShellFiler.GraphicsViewer;
using ShellFiler.Properties;
using ShellFiler.FileTask;
using ShellFiler.FileTask.Management;
using ShellFiler.FileSystem;

namespace ShellFiler.Document {

    //=========================================================================================
    // クラス：２ストロークキーの状態
    //=========================================================================================
    public class TwoStrokeKeyState {
        // 入力シーン
        private CommandUsingSceneType m_scene;

        // 入力中のウィンドウ（入力がないときnull）
        private ITwoStrokeKeyForm m_targetControl = null;

        // 2ストロークキーの種類（入力がないときNone）
        private TwoStrokeType m_keyType = TwoStrokeType.None;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public TwoStrokeKeyState() {
        }

        //=========================================================================================
        // 機　能：キーの状態をセットする
        // 引　数：[in]scene     入力シーン
        // 　　　　[in]target    入力中のウィンドウ
        // 　　　　[in]keyType   2ストロークキーの種類
        // 戻り値：なし
        //=========================================================================================
        public void SetKeyState(CommandUsingSceneType scene, ITwoStrokeKeyForm target, TwoStrokeType keyType) {
            if (m_targetControl != null) {
                m_targetControl.TwoStrokeKeyStateChanged(TwoStrokeType.None);
            }
            m_scene = scene;
            m_targetControl = target;
            m_keyType = keyType;
            m_targetControl.TwoStrokeKeyStateChanged(m_keyType);
        }

        //=========================================================================================
        // 機　能：キーの状態をリセットする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetKeyState() {
            if (m_targetControl != null) {
                m_targetControl.TwoStrokeKeyStateChanged(TwoStrokeType.None);
            }
            m_targetControl = null;
            m_keyType = TwoStrokeType.None;
        }

        //=========================================================================================
        // 機　能：2ストロークキーの状態を取得する
        // 引　数：[in]scene   入力シーン
        // 　　　　[in]target  入力中のウィンドウ
        // 戻り値：なし
        //=========================================================================================
        public TwoStrokeType GetTwoStrokeState(CommandUsingSceneType scene, ITwoStrokeKeyForm target) {
            if (scene == m_scene && target == m_targetControl) {
                return m_keyType;
            } else {
                return TwoStrokeType.None;
            }
        }

        //=========================================================================================
        // 機　能：2ストロークキーの名前を取得する
        // 引　数：[in]keyType  キーの名前
        // 戻り値：なし
        //=========================================================================================
        public static string GetDisplayNameKey(TwoStrokeType keyType) {
            string dispName = "";
            switch (keyType) {
                case TwoStrokeType.Key1:
                    dispName = Resources.KeyName_TwoStroke1;
                    break;
                case TwoStrokeType.Key2:
                    dispName = Resources.KeyName_TwoStroke2;
                    break;
                case TwoStrokeType.Key3:
                    dispName = Resources.KeyName_TwoStroke3;
                    break;
                case TwoStrokeType.Key4:
                    dispName = Resources.KeyName_TwoStroke4;
                    break;
            }
            return dispName;
        }
    }
}
