using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Command;
using ShellFiler.Util;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：ファンクションバー
    //=========================================================================================
    public partial class FunctionBar : Panel {
        // ファンクションキーの数
        private const int FUNCTION_COUNT = 12;

        // セパレータの最小幅
        private const int CX_SEPARATOR_MINIMUM = 6;

        // ファンクションバーの最小高
        private const int CY_FUNCTION_BAR = 19;

        // ファンクションキーの定義
        private static readonly Keys[] KEY_LIST = { Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11, Keys.F12};

        // 初期化が完了したときtrue
        private bool m_initialized = false;

        // キー入力の統合先（親フォーム）
        private IKeyEventIntegrator m_keyIntegrator;

        // コマンドの送信元の識別用
        private UICommandSender m_commandSender;
        
        // コマンドの送信先
        private IUICommandTarget m_commandTarget;

        // Shiftキーが押されているときtrue
        private bool m_isShiftDown = false;

        // Ctrlキーが押されているときtrue
        private bool m_isControlDown = false;
        
        // ALTキーが押されているときtrue
        private bool m_isAltDown = false;

        // 左ウィンドウにカーソルがあるときtrue
        private bool m_isLeft = true;

        // ボタンの状態（マークされている状態として更新されているときtrue）
        private bool m_uiStatusMarked = true;

        // ボタンの状態（直前のパスヒストリが有効として更新されているときtrue）
        private bool m_uiStatusPathHisPrev = true;

        // ボタンの状態（直後のパスヒストリが有効として更新されているときtrue）
        private bool m_uiStatusPathHisNext = true;

        // 現在押されている状態にあるボタンのインデックス（-1:押されていない）
        private int m_indexCurrentPush = -1;

        // キーの設定一覧
        private KeyItemSettingList m_keySettingList = null;

        // ボタンの領域
        private Rectangle[] m_rcButton = new Rectangle[FUNCTION_COUNT];

        // ボタンが有効なときtrue
        private ButtonItemInfo[][] m_buttonItemInfo;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public FunctionBar() {
            InitializeComponent();

            // ダブルバッファリング有効
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
        
        //=========================================================================================
        // 機　能：初期化する
        // 引　数：[in]keyIntergator  キー入力の統合先
        // 　　　　[in]commandSender  コマンドの送信元の識別用
        // 　　　　[in]commandTarget  コマンドの送信先
        // 　　　　[in]keySettingList キー設定
        // 　　　　[in]isMain         メイン画面のファンクションバーのときtrue
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(IKeyEventIntegrator keyIntergator, UICommandSender commandSender, IUICommandTarget commandTarget, KeyItemSettingList keySettingList, bool isMain) {
            m_keyIntegrator = keyIntergator;
            m_commandSender = commandSender;
            m_commandTarget = commandTarget;
            m_keySettingList = keySettingList;

            if (isMain) {
                Program.MainWindow.CursorLRChanged += new MainWindowForm.CursorLRChangedHandler(MainWindowForm_CursorLRChanged);
            }

            ResetButtonState();
            m_initialized = true;
            OnSizeChanged();
        }

        //=========================================================================================
        // 機　能：項目情報を初期化する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void ResetButtonState() {
            m_buttonItemInfo = new ButtonItemInfo[8][];    // Shift*Control*Alt
            for (int i = 0; i < 8; i++) {
                m_buttonItemInfo[i] = new ButtonItemInfo[FUNCTION_COUNT];
                for (int j = 0; j < FUNCTION_COUNT; j++) {
                    KeyState key = new KeyState(KEY_LIST[j], ((i & 0x4) != 0), ((i & 0x2) != 0), ((i & 0x1) != 0));     // GetButtonItemInfoに依存
                    KeyItemSetting setting = m_keySettingList.GetSettingFromKey(key);
                    UIResource uiResource = null;
                    bool enabled = false;
                    if (setting != null) {
                        ActionCommand command = setting.ActionCommandMoniker.CreateActionCommand();
                        uiResource = command.UIResource;
                        enabled = true;
                    }
                    m_buttonItemInfo[i][j] = new ButtonItemInfo(setting, uiResource, enabled);
                }
            }
            Invalidate();
        }

        //=========================================================================================
        // 機　能：m_buttonItemInfoから現在のシフト状態に基づいて指定インデックスの情報を返す
        // 引　数：[in]index  ファンクションキーのインデックス
        // 戻り値：なし
        //=========================================================================================
        private ButtonItemInfo GetButtonItemInfo(int index) {
            int modKey = 0;
            if (m_isShiftDown) {
                modKey |= 4;
            }
            if (m_isControlDown) {
                modKey |= 2;
            }
            if (m_isAltDown) {
                modKey |= 1;
            }
            return m_buttonItemInfo[modKey][index];
        }

        //=========================================================================================
        // 機　能：再描画が必要になったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FunctionBar_Paint(object sender, PaintEventArgs evt) {
            if (m_rcButton.Length == 0 || m_keySettingList == null) {
                return;
            }
            for (int i = 0; i < m_rcButton.Length; i++) {
                Bitmap bmp = CreateDoubleBuffer(i, false);
                evt.Graphics.DrawImage(bmp, m_rcButton[i].Left, m_rcButton[i].Top);
                bmp.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：ダブルバッファリング用に画面イメージを作成したビットマップを作成する
        // 引　数：[in]index   作成するボタンのインデックス
        // 　　　　[in]isPush  押された状態を作成するときtrue
        // 戻り値：なし
        //=========================================================================================
        private Bitmap CreateDoubleBuffer(int index, bool isPush) {
            Bitmap bmp = new Bitmap(m_rcButton[index].Width, m_rcButton[index].Height);
            Graphics gBmp = Graphics.FromImage(bmp);
            FunctionBarGraphics grpBmp = new FunctionBarGraphics(gBmp);
            try {
                KeyState key = GetKeyState(index);
                KeyItemSetting setting = m_keySettingList.GetSettingFromKey(key);
                Rectangle rc = new Rectangle(0, 0, m_rcButton[index].Width, m_rcButton[index].Height);
                DrawFunctionButton(grpBmp, rc, setting, index, isPush);
            } finally {
                grpBmp.Dispose();
                gBmp.Dispose();
            }
            return bmp;
        }

        //=========================================================================================
        // 機　能：ダブルバッファリング用に画面イメージを作成したビットマップを作成する
        // 引　数：[in]g       グラフィックス
        // 　　　　[in]rc      描画するボタンの領域
        // 　　　　[in]setting 描画するボタンのキー設定
        // 　　　　[in]index   作成するボタンのインデックス
        // 　　　　[in]isPush  押された状態を作成するときtrue
        // 戻り値：なし
        //=========================================================================================
        private void DrawFunctionButton(FunctionBarGraphics g, Rectangle rc, KeyItemSetting setting, int index, bool isPush) {
            // 背景を描画
            g.Graphics.FillRectangle(SystemBrushes.Control, rc);

            // 枠を描画
            if (!isPush) {
                g.Graphics.DrawLine(SystemPens.ControlLightLight, rc.Left, rc.Top, rc.Left, rc.Bottom - 2);
                g.Graphics.DrawLine(SystemPens.ControlLightLight, rc.Left, rc.Top, rc.Right - 1, rc.Top);
                g.Graphics.DrawLine(SystemPens.ControlDarkDark, rc.Left, rc.Bottom - 1, rc.Right - 1, rc.Bottom - 1);
                g.Graphics.DrawLine(SystemPens.ControlDarkDark, rc.Right - 1, rc.Top + 1, rc.Right - 1, rc.Bottom - 1);
                g.Graphics.DrawLine(SystemPens.ControlDark, rc.Left + 1, rc.Bottom - 2, rc.Right - 2, rc.Bottom - 2);
                g.Graphics.DrawLine(SystemPens.ControlDark, rc.Right - 2, rc.Top + 1, rc.Right - 2, rc.Bottom - 2);
            } else {
                g.Graphics.DrawLine(SystemPens.ControlDarkDark, rc.Left, rc.Top, rc.Left, rc.Bottom - 1);
                g.Graphics.DrawLine(SystemPens.ControlDarkDark, rc.Left, rc.Top, rc.Right - 1, rc.Top);
                g.Graphics.DrawLine(SystemPens.ControlDark, rc.Left + 1, rc.Top + 1, rc.Left + 1, rc.Bottom - 2);
                g.Graphics.DrawLine(SystemPens.ControlDark, rc.Left + 1, rc.Top + 1, rc.Right - 1, rc.Top + 1);
                g.Graphics.DrawLine(SystemPens.ControlLightLight, rc.Left, rc.Bottom - 1, rc.Right - 1, rc.Bottom - 1);
                g.Graphics.DrawLine(SystemPens.ControlLightLight, rc.Right - 1, rc.Top + 1, rc.Right - 1, rc.Bottom - 1);
            }

            int offset = (isPush) ? 1 : 0;

            // アイコンを用意
            ActionCommand command = null;
            IconImageListID icon = IconImageListID.None;
            if (setting != null) {
                command = setting.ActionCommandMoniker.CreateActionCommand();
                if (m_isLeft) {
                    icon = command.UIResource.IconIdLeft;
                } else {
                    icon = command.UIResource.IconIdRight;
                }
            }

            IconImageListID overrayIcon = IconImageListID.None;
            if (Configuration.Current.FunctionBarUseOverrayIcon) {
                switch (index) {
                    case 0:
                        overrayIcon = IconImageListID.Icon_Func01;
                        break;
                    case 1:
                        overrayIcon = IconImageListID.Icon_Func02;
                        break;
                    case 2:
                        overrayIcon = IconImageListID.Icon_Func03;
                        break;
                    case 3:
                        overrayIcon = IconImageListID.Icon_Func04;
                        break;
                    case 4:
                        overrayIcon = IconImageListID.Icon_Func05;
                        break;
                    case 5:
                        overrayIcon = IconImageListID.Icon_Func06;
                        break;
                    case 6:
                        overrayIcon = IconImageListID.Icon_Func07;
                        break;
                    case 7:
                        overrayIcon = IconImageListID.Icon_Func08;
                        break;
                    case 8:
                        overrayIcon = IconImageListID.Icon_Func09;
                        break;
                    case 9:
                        overrayIcon = IconImageListID.Icon_Func10;
                        break;
                    case 10:
                        overrayIcon = IconImageListID.Icon_Func11;
                        break;
                    case 11:
                        overrayIcon = IconImageListID.Icon_Func12;
                        break;
                }
            }

            // 描画
            string disp = "";
            if (setting != null && setting.DisplayName != null) {
                disp = setting.DisplayName;
            } else if (command != null) {
                disp = command.UIResource.HintShort;
            }
            int iconMargin = 0;
            if (Configuration.Current.FunctionBarUseOverrayIcon) {
                iconMargin = 5;
            }
            int dispMargin = iconMargin + 4;
            int iconX = rc.X + 2 + offset;
            int iconY = rc.Y + 1 + offset;
            int cx = UIIconManager.CxDefaultIcon;
            if (GetButtonItemInfo(index).Enabled) {
                if (icon != IconImageListID.None) {
                    UIIconManager.IconImageList.Draw(g.Graphics, iconX + iconMargin, iconY, (int)icon);
                }
                if (overrayIcon != IconImageListID.None) {
                    UIIconManager.IconImageList.Draw(g.Graphics, iconX, iconY, (int)overrayIcon);
                }
                if (setting != null) {
                    g.Graphics.DrawString(disp, g.FunctionBarFont, Brushes.Black, new Point(rc.Left + dispMargin + cx + offset, (rc.Height - g.FunctionBarFont.Height) / 2 + offset), g.StringFormat);
                }
            } else {
                if (icon != IconImageListID.None) {
                    GraphicsUtils.DrawDisabledImageList(g.Graphics, UIIconManager.IconImageList, (int)icon, iconX + iconMargin, iconY, SystemColors.Control);
                }
                if (overrayIcon != IconImageListID.None) {
                    GraphicsUtils.DrawDisabledImageList(g.Graphics, UIIconManager.IconImageList, (int)overrayIcon, iconX + iconMargin, iconY, SystemColors.Control);
                }
                if (setting != null) {
                    g.Graphics.DrawString(disp, g.FunctionBarFont, Brushes.Gray, new Point(rc.Left + dispMargin + cx + offset, (rc.Height - g.FunctionBarFont.Height) / 2 + offset), g.StringFormat);
                }
            }
        }

        //=========================================================================================
        // 機　能：F1～F12のキー情報を作成する
        // 引　数：[in]index   作成するキーのインデックス
        // 戻り値：なし
        //=========================================================================================
        private KeyState GetKeyState(int index) {
            KeyState key = new KeyState(KEY_LIST[index], m_isShiftDown, m_isControlDown, m_isAltDown);
            return key;
        }

        //=========================================================================================
        // 機　能：ウィンドウサイズが変更されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FunctionBar_SizeChanged(object sender, EventArgs evt) {
            OnSizeChanged();
        }

        //=========================================================================================
        // 機　能：ウィンドウサイズが変更されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnSizeChanged() {
            if (!m_initialized) {
                return;
            }
            int funcButton = Configuration.Current.FunctionBarSplitCount;
            int separatorCount = 0;
            if (funcButton != 0) {
                separatorCount = 2;
            }
            int cyFunctionBar = MainWindowForm.Y(CY_FUNCTION_BAR);
            int y1 = ClientRectangle.Height - cyFunctionBar;
            int cyButton = cyFunctionBar;
            float cxButtonF = (float)(ClientRectangle.Width - 1 - CX_SEPARATOR_MINIMUM * separatorCount) / FUNCTION_COUNT;
            int cxButton = (int)cxButtonF;
            float x = 0;
            if (funcButton == 4) {
                m_rcButton[0] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[1] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[2] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[3] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                x += CX_SEPARATOR_MINIMUM;
                m_rcButton[4] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[5] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[6] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[7] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                x += CX_SEPARATOR_MINIMUM;
                m_rcButton[8] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[9] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[10] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[11] = new Rectangle((int)x, y1, ClientRectangle.Width - 1 - (int)x, cyButton);
            } else if (funcButton == 5) {
                m_rcButton[0] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[1] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[2] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[3] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[4] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                x += CX_SEPARATOR_MINIMUM;
                m_rcButton[5] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[6] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[7] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[8] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[9] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                x += CX_SEPARATOR_MINIMUM;
                m_rcButton[10] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[11] = new Rectangle((int)x, y1, ClientRectangle.Width - 1 - (int)x, cyButton);
            } else {
                m_rcButton[0] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[1] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[2] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[3] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[4] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[5] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[6] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[7] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[8] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[9] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[10] = new Rectangle((int)x, y1, cxButton, cyButton);
                x += cxButtonF;
                m_rcButton[11] = new Rectangle((int)x, y1, ClientRectangle.Width - 1 - (int)x, cyButton);
            }
            Invalidate();
        }

        //=========================================================================================
        // 機　能：マウスボタンが押されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FunctionBar_MouseDown(object sender, MouseEventArgs evt) {
            Capture = true;
            FunctionBarGraphics g = new FunctionBarGraphics(this);
            try {
                for (int i = 0; i < m_rcButton.Length; i++) {
                    if (m_rcButton[i].Contains(evt.Location)) {
                        ButtonItemInfo info = GetButtonItemInfo(i);
                        if (info.Enabled) {
                            Bitmap bmp = CreateDoubleBuffer(i, true);
                            g.Graphics.DrawImage(bmp, m_rcButton[i].Left, m_rcButton[i].Top);
                            bmp.Dispose();
                            m_indexCurrentPush = i;
                            break;
                        }
                    }
                }
            } finally {
                g.Dispose();
            }
        }

        //=========================================================================================
        // 機　能：マウスボタンが離されたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FunctionBar_MouseUp(object sender, MouseEventArgs evt) {
            Capture = false;
            int indexExec = -1;
            for (int i = 0; i < m_rcButton.Length; i++) {
                if (m_rcButton[i].Contains(evt.Location)) {
                    ButtonItemInfo info = GetButtonItemInfo(i);
                    if (info.Enabled && m_indexCurrentPush == i) {
                        indexExec = i;
                    }
                    break;
                }
            }
            m_indexCurrentPush = -1;
            Invalidate();

            // コマンドを実行
            if (indexExec != -1) {
                KeyState key = GetKeyState(indexExec);
                KeyItemSetting setting = m_keySettingList.GetSettingFromKey(key);
                if (setting != null) {
                    UICommandItem item = new UICommandItem(setting.ActionCommandMoniker);
                    m_commandTarget.OnUICommand(m_commandSender, item);
                }
            }
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FunctionBar_MouseMove(object sender, MouseEventArgs evt) {
            if (!Capture) {
                return;
            }

            // 新しい位置を検索
            int indexNewPush = -1;
            for (int i = 0; i < m_rcButton.Length; i++) {
                ButtonItemInfo info = GetButtonItemInfo(i);
                if (info.Enabled && m_rcButton[i].Contains(evt.Location)) {
                    indexNewPush = i;
                    break;
                }
            }

            // 位置が変わっていたら描画
            if (indexNewPush != m_indexCurrentPush) {
                FunctionBarGraphics g = new FunctionBarGraphics(this);
                try {
                    if (m_indexCurrentPush != -1) {
                        Bitmap bmp = CreateDoubleBuffer(m_indexCurrentPush, false);
                        g.Graphics.DrawImage(bmp, m_rcButton[m_indexCurrentPush].Left, m_rcButton[m_indexCurrentPush].Top);
                        bmp.Dispose();
                    }
                    if (indexNewPush != -1) {
                        Bitmap bmp = CreateDoubleBuffer(indexNewPush, true);
                        g.Graphics.DrawImage(bmp, m_rcButton[indexNewPush].Left, m_rcButton[indexNewPush].Top);
                        bmp.Dispose();
                    }
                    m_indexCurrentPush = indexNewPush;
                } finally {
                    g.Dispose();
                }
            }
        }
        
        //=========================================================================================
        // 機　能：メインウィンドウのカーソルの左右に変化が生じたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MainWindowForm_CursorLRChanged(object sender, EventArgs evt) {
            m_isLeft = Program.Document.CurrentTabPage.IsCursorLeft;
            Invalidate();
        }

        //=========================================================================================
        // 機　能：キー入力の状態が変わったときの処理を行う
        // 引　数：[in]key  キーコマンド
        // 戻り値：なし
        //=========================================================================================
        public void OnKeyChange(KeyCommand key) {
            bool oldShift = m_isShiftDown;
            bool oldControl = m_isControlDown;
            bool oldAlt = m_isAltDown;
            m_isShiftDown = key.Shift;
            m_isControlDown = key.Control;
            m_isAltDown = key.Alt;
            if (oldShift != m_isShiftDown || oldControl != m_isControlDown || oldAlt != m_isAltDown) {
                Invalidate();
            }
        }

        //=========================================================================================
        // 機　能：ファンクションバーのステータス状態を更新する
        // 引　数：[in]context   ツールバーの更新状態のコンテキスト
        // 戻り値：なし
        //=========================================================================================
        public void RefreshButtonStatus(UIItemRefreshContext context) {
            if (m_buttonItemInfo == null) {
                return;
            }
            bool changed = false;

            // マーク中のみ有効
            if (m_uiStatusMarked != context.IsMarked) {                
                m_uiStatusMarked = context.IsMarked;
                for (int i = 0; i < FUNCTION_COUNT; i++) {
                    ButtonItemInfo itemInfo = GetButtonItemInfo(i);
                    if (itemInfo.UIResource != null && UIEnableCondition.CheckMark(itemInfo.UIResource.UIEnableCondition)) {
                        itemInfo.Enabled = m_uiStatusMarked;
                        changed = true;
                    }
                }
            }

            // 前のパスヒストリが有効
            if (m_uiStatusPathHisPrev != context.GetPathHistoryPrev(m_commandSender)) {
                m_uiStatusPathHisPrev = context.GetPathHistoryPrev(m_commandSender);
                for (int i = 0; i < FUNCTION_COUNT; i++) {
                    ButtonItemInfo itemInfo = GetButtonItemInfo(i);
                    if (itemInfo.UIResource != null && itemInfo.UIResource.UIEnableCondition == UIEnableCondition.PathHistP) {
                        itemInfo.Enabled = m_uiStatusPathHisPrev;
                        changed = true;
                    }
                }
            }

            // 次のパスヒストリが有効
            if (m_uiStatusPathHisNext != context.GetPathHistoryNext(m_commandSender)) {
                m_uiStatusPathHisNext = context.GetPathHistoryNext(m_commandSender);
                for (int i = 0; i < FUNCTION_COUNT; i++) {
                    ButtonItemInfo itemInfo = GetButtonItemInfo(i);
                    if (itemInfo.UIResource != null && itemInfo.UIResource.UIEnableCondition == UIEnableCondition.PathHistN) {
                        itemInfo.Enabled = m_uiStatusPathHisNext;
                        changed = true;
                    }
                }
            }

            if (changed) {
                Invalidate();
            }
        }

        //=========================================================================================
        // 機　能：Windowsからのキー入力を受け取ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FunctionBar_KeyDown(object sender, KeyEventArgs evt) {
            m_keyIntegrator.OnKeyDown(new KeyCommand(evt));
        }

         //=========================================================================================
        // 機　能：Windowsからのキー入力を受け取ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FunctionBar_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs evt) {
            m_keyIntegrator.OnKeyChar(evt.KeyChar);
        }

        //=========================================================================================
        // 機　能：Windowsからのキー入力を受け取ったときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void FunctionBar_PreviewKeyDown(object sender, PreviewKeyDownEventArgs evt) {
        }

        //=========================================================================================
        // 機　能：Windowsからのキー入力を受け取ったときの処理を行う
        // 引　数：[in]key  キーコマンド
        // 戻り値：なし
        //=========================================================================================
        private void FunctionBar_KeyUp(object sender, KeyEventArgs evt) {
            m_keyIntegrator.OnKeyUp(new KeyCommand(evt));
        }
    }

    //=========================================================================================
    // クラス：ボタンの情報
    //=========================================================================================
    class ButtonItemInfo {
        // キーの設定情報
        private KeyItemSetting m_setting;

        // UIのリソース情報
        private UIResource m_uiResource;

        // 項目が有効なときtrue
        private bool m_isEnabled;

        public ButtonItemInfo(KeyItemSetting setting, UIResource uiResource, bool enabled) {
            m_setting = setting;
            m_uiResource = uiResource;
            m_isEnabled = enabled;
        }
        public KeyItemSetting KeyItemSetting {
            get {
                return m_setting;
            }
        }
        public UIResource UIResource {
            get {
                return m_uiResource;
            }
        }
        public bool Enabled {
            get {
                return m_isEnabled;
            }
            set {
                m_isEnabled = value;
            }
        }
    }
}
