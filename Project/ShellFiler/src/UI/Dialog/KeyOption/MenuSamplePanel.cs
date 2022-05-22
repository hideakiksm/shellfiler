using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Document.Setting;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog.KeyOption {

    //=========================================================================================
    // クラス：メニューのサンプルを表示するパネル
    //=========================================================================================
    public partial class MenuSamplePanel : UserControl {
        // 通常項目の左側にあるアイコン描画領域の幅
        public const int CX_LEFT_AREA = 24;

        // ルートメニューの高さ
        public const int CY_TOP_AREA = 20;

        // ルートメニューのマージンの幅
        public const int CX_TOP_MARGIN = 8;

        // 各メニュー項目のマージン
        public const int CX_MENU_MARGIN = 8;

        // 各メニュー項目の高さ
        public const int CY_MENU_ITEM = 20;

        // 表示対象となっているメニュー項目
        private MenuItemSetting m_currentMenu;

        // ルートメニューも表示するときtrue（ポップアップメニューとしてしか表示しない場合はfalse）
        private bool m_rootMenu;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public MenuSamplePanel() {
            InitializeComponent();
        }

        //=========================================================================================
        // 機　能：表示対象を設定して初期化する
        // 引　数：[in]menuSetting  表示対象となっているメニュー項目
        // 　　　　[in]rootMenu     ルートメニューも表示するときtrue
        // 戻り値：なし
        //=========================================================================================
        public void Initialize(MenuItemSetting menuSetting, bool rootMenu) {
            m_currentMenu = menuSetting;
            m_rootMenu = rootMenu;
            this.Invalidate();
        }

        //=========================================================================================
        // 機　能：描画イベントを処理する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void MenuSamplePanel_Paint(object sender, PaintEventArgs evt) {
            DoubleBuffer doubleBuffer = new DoubleBuffer(evt.Graphics, this.Width, this.Height);
            doubleBuffer.SetDrawOrigin(0, 0);
            MenuGraphics g = new MenuGraphics(doubleBuffer.DrawingGraphics);
            try {
                if (m_currentMenu == null) {
                    g.Graphics.FillRectangle(SystemBrushes.Control, this.ClientRectangle);
                    return;
                }
                g.Graphics.FillRectangle(g.MenuBackBrush, this.ClientRectangle);
                DrawMenu(g);
            } finally {
                g.Dispose();
            }

            doubleBuffer.FlushScreen(0, 0);
        }

        //=========================================================================================
        // 機　能：メニューを描画する
        // 引　数：[in]g   グラフィックス
        // 戻り値：なし
        //=========================================================================================
        private void DrawMenu(MenuGraphics g) {
            // ルート
            string topMenuName = m_currentMenu.ItemName.Replace("&", "");
            int cxTop = (int)(GraphicsUtils.MeasureString(g.Graphics, this.Font, topMenuName)) + CX_TOP_MARGIN;
            int cyTop;
            if (m_rootMenu) {
                cyTop = CY_TOP_AREA;
            } else {
                cyTop = 0;
            }

            // 幅の最大
            int cxItem = Math.Max(cxTop + CX_TOP_MARGIN, this.Width * 2 / 3);
            int cyItem = 0;
            List<MenuItemSetting> menuList = m_currentMenu.SubMenuList;
            for (int i = 0; i < menuList.Count; i++) {
                if (menuList[i].Type == MenuItemSetting.ItemType.Separator) {
                    cyItem += CY_MENU_ITEM / 2;
                } else {
                    string itemName = menuList[i].ItemName.Replace("&", "");
                    cxItem = Math.Max(cxItem, CX_LEFT_AREA + CX_MENU_MARGIN * 3 + (int)GraphicsUtils.MeasureString(g.Graphics, this.Font, itemName));
                    cyItem += CY_MENU_ITEM;
                }
            }
            if (cyItem == 0) {
                cyItem = 2;
            }
            Rectangle rcMenuTop = new Rectangle(1, 1, cxTop + CX_TOP_MARGIN * 2, cyTop - 2);
            Rectangle rcLeftArea = new Rectangle(1, cyTop, CX_LEFT_AREA, cyItem);

            // 枠を描画
            if (m_rootMenu) {
                g.Graphics.FillRectangle(g.MenuTopBrush, rcMenuTop);                                                // 上背景
            }
            g.Graphics.FillRectangle(g.LeftAreaBrush(rcLeftArea), rcLeftArea);                                      // 左背景
            g.Graphics.DrawLine(g.BorderPen, 0, 0, cxTop + CX_TOP_MARGIN * 2, 0);                                   // 上
            g.Graphics.DrawLine(g.BorderPen, cxTop + CX_TOP_MARGIN * 2, 0, cxTop + CX_TOP_MARGIN * 2, cyTop);
            g.Graphics.DrawLine(g.BorderPen, cxTop + CX_TOP_MARGIN * 2, cyTop, cxItem, cyTop);
            g.Graphics.DrawLine(g.BorderPen, 0, 0, 0, cyTop + cyItem);                                              // 左右
            g.Graphics.DrawLine(g.BorderPen, cxItem, cyTop, cxItem, cyTop + cyItem);
            g.Graphics.DrawLine(g.BorderPen, 0, cyTop + cyItem, cxItem, cyTop + cyItem);

            // 項目を描画
            if (m_rootMenu) {
                int yPosTop = (CY_MENU_ITEM - this.Font.Height) / 2;
                g.Graphics.DrawString(topMenuName, this.Font, SystemBrushes.ControlText, new Point(CX_TOP_MARGIN, yPosTop + 2));
            }
            int yPos = cyTop + 2;
            for (int i = 0; i < menuList.Count; i++) {
                if (menuList[i].Type == MenuItemSetting.ItemType.Separator) {
                    int yPosBar = yPos + CY_MENU_ITEM / 4;
                    g.Graphics.DrawLine(g.SeparatorLightPen, CX_LEFT_AREA + CX_MENU_MARGIN, yPosBar, cxItem - 2, yPosBar);
                    g.Graphics.DrawLine(g.SeparatorDarkPen, CX_LEFT_AREA + CX_MENU_MARGIN, yPosBar + 1, cxItem - 2, yPosBar + 1);
                    yPos += CY_MENU_ITEM / 2;
                } else {
                    int yFont = (CY_MENU_ITEM - this.Font.Height) / 2;
                    string itemName = menuList[i].ItemName.Replace("&", "");
                    g.Graphics.DrawString(itemName, this.Font, SystemBrushes.ControlText, new Point(CX_LEFT_AREA + CX_MENU_MARGIN, yPos + yFont));

                    if (menuList[i].UIResource != null) {
                        int xPosIcon = (CX_LEFT_AREA - UIIconManager.CxDefaultIcon) / 2;
                        int yPosIcon = (CY_MENU_ITEM - UIIconManager.CyDefaultIcon) / 2 + yPos;
                        int imageId = (int)(menuList[i].UIResource.IconIdLeft);
                        if (imageId != 0) {
                            UIIconManager.IconImageList.Draw(g.Graphics, new Point(xPosIcon, yPosIcon), imageId);
                        }
                    }
                    yPos += CY_MENU_ITEM;
                }
            }
        }
    }
}
