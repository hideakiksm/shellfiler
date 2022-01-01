using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileTask.Management;
using ShellFiler.Properties;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Locale;
using ShellFiler.UI.Dialog.KeyOption;
using ShellFiler.Util;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：2ストロークキーの状態表示ボタン
    //=========================================================================================
    class TwoStrokeButton {
        // 親のツールバー
        private ToolStrip m_parent;
        
        // コマンドの利用シーン
        private CommandUsingSceneType m_commandUsingScene;

        // ボタンの領域
        private Rectangle m_rcTwoStateButton;

        // 2ストロークキーの状態
        private TwoStrokeType m_twoStrokeType = TwoStrokeType.None;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent        親ツールバー
        // 　　　　[in]commandScene  コマンドの利用シーン
        // 戻り値：なし
        //=========================================================================================
        public TwoStrokeButton(ToolStrip parent, CommandUsingSceneType commandScene) {
            m_parent = parent;
            m_commandUsingScene = commandScene;
        }

        //=========================================================================================
        // 機　能：初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void Initialize() {
        }

        //=========================================================================================
        // 機　能：フォームのサイズが変更されたときの処理を行う
        // 引　数：[in]rcButton   ツールバー内でのボタンの領域
        // 戻り値：なし
        //=========================================================================================
        public void OnSizeChanged(Rectangle rcButton) {
            m_rcTwoStateButton = rcButton;
        }

        //=========================================================================================
        // 機　能：描画イベント受信時の処理を行う
        // 引　数：[in]grp  グラフィックス
        // 戻り値：なし
        //=========================================================================================
        public void DrawButton(Graphics grp) {
            if (m_rcTwoStateButton == null) {
                return;     // デザイナーでの表示
            }
            if (m_twoStrokeType == TwoStrokeType.None) {
                return;
            }

            KeyItemSettingList settingList = Program.Document.KeySetting.GetKeyList(m_commandUsingScene);
            KeyState twoStrokeAssign = settingList.GetTwoStrokeKey(m_twoStrokeType);
            if (twoStrokeAssign == null) {
                return;
            }
            DoubleBuffer doubleBuffer = new DoubleBuffer(grp, m_rcTwoStateButton.Width, m_rcTwoStateButton.Height);
            try {
                doubleBuffer.SetDrawOrigin(-m_rcTwoStateButton.Left, -m_rcTwoStateButton.Top);
                doubleBuffer.DrawingGraphics.FillRectangle(Brushes.White, m_rcTwoStateButton);
                TwoStrokeButtonGraphics grp2 = new TwoStrokeButtonGraphics(doubleBuffer.DrawingGraphics);
                try {
                    DrawButtonExec(grp2, twoStrokeAssign);
                } finally {
                    grp2.Dispose();
                }
            } finally {
                doubleBuffer.FlushScreen(m_rcTwoStateButton.Left, m_rcTwoStateButton.Top);
            }
        }

        //=========================================================================================
        // 機　能：描画を実行する
        // 引　数：[in]grp       グラフィックス
        // 　　　　[in]twoStroke 2ストロークキーに割り当てられたボタン
        // 戻り値：なし
        //=========================================================================================
        private void DrawButtonExec(TwoStrokeButtonGraphics grp, KeyState twoStroke) {
            Graphics g = grp.Graphics;
            int cx = m_rcTwoStateButton.Width / 2;
            if (twoStroke.IsShift && twoStroke.IsControl && twoStroke.IsAlt) {
                g.DrawImage(UIIconManager.TwoStrokeKeyShiftCtrlAlt, m_rcTwoStateButton);
            } else if (twoStroke.IsShift && twoStroke.IsControl && !twoStroke.IsAlt) {
                g.DrawImage(UIIconManager.TwoStrokeKeyShiftCtrl, m_rcTwoStateButton);
            } else if (twoStroke.IsShift && !twoStroke.IsControl && twoStroke.IsAlt) {
                g.DrawImage(UIIconManager.TwoStrokeKeyShiftAlt, m_rcTwoStateButton);
            } else if (twoStroke.IsShift && !twoStroke.IsControl && !twoStroke.IsAlt) {
                g.DrawImage(UIIconManager.TwoStrokeKeyShift, m_rcTwoStateButton);
            } else if (!twoStroke.IsShift && twoStroke.IsControl && twoStroke.IsAlt) {
                g.DrawImage(UIIconManager.TwoStrokeKeyCtrlAlt, m_rcTwoStateButton);
            } else if (!twoStroke.IsShift && twoStroke.IsControl && !twoStroke.IsAlt) {
                g.DrawImage(UIIconManager.TwoStrokeKeyCtrl, m_rcTwoStateButton);
            } else if (!twoStroke.IsShift && !twoStroke.IsControl && twoStroke.IsAlt) {
                g.DrawImage(UIIconManager.TwoStrokeKeyAlt, m_rcTwoStateButton);
            } else if (!twoStroke.IsShift && !twoStroke.IsControl && !twoStroke.IsAlt) {
                g.DrawImage(UIIconManager.TwoStrokeKeyNormal, m_rcTwoStateButton);
                cx = m_rcTwoStateButton.Width;
            }

            // ボタン名
            string keyName = KeyNameUtils.GetDisplayName(twoStroke.Key);
            int size = (int)(GraphicsUtils.MeasureString(g, grp.NormalFont, keyName));
            Font font;
            if (size <= cx) {
                font = grp.NormalFont;
            } else {
                font = grp.SmallFont;
            }
            Point pos = new Point(m_rcTwoStateButton.Left + m_rcTwoStateButton.Width - cx + Math.Max(0, (cx - size) / 2) - 1,
                                  m_rcTwoStateButton.Top + (m_rcTwoStateButton.Height - font.Height) / 2 - 1);
            Point pos2 = new Point(pos.X + 1, pos.Y);
            g.SmoothingMode = SmoothingMode.None;
            g.DrawString(keyName, font, grp.HighlightBrush, pos2);
            g.DrawString(keyName, font, grp.FontBrush, pos);
        }

        //=========================================================================================
        // 機　能：2ストロークキーの状態が変わったことを通知する
        // 引　数：[in]newState  新しい状態
        // 戻り値：なし
        //=========================================================================================
        public void TwoStrokeKeyStateChanged(TwoStrokeType newState) {
            m_twoStrokeType = newState;
            m_parent.Invalidate();
        }
    }
}
