using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Document;
using ShellFiler.Api;
using ShellFiler.Properties;
using ShellFiler.Command;
using ShellFiler.Command.FileList;
using ShellFiler.Command.FileViewer;
using ShellFiler.Util;

namespace ShellFiler.UI.ControlBar {

    //=========================================================================================
    // クラス：ディスクドライブの項目に関するUIを作成するユーティリティ
    //=========================================================================================
    class DriveItem {

        //=========================================================================================
        // 機　能：ドライブアイコンを取得する
        // 引　数：[in]driveInfo  作成するアイコンの元になるドライブの情報
        // 戻り値：アイコンのビットマップ（破棄が必要）
        //=========================================================================================
        public static Bitmap GetDriveIcon(DriveInfo driveInfo) {
            // アイコンの基本部分を取得
            IconImageListID iconBase;
            switch (driveInfo.DriveType) {
                case DriveType.CDRom:
                    iconBase = IconImageListID.Icon_DriveCD;
                    break;
                case DriveType.Removable:
                    iconBase = IconImageListID.Icon_DriveRemovable;
                    break;
                case DriveType.Network:
                    iconBase = IconImageListID.Icon_DriveNet;
                    break;
                case DriveType.Ram:
                    iconBase = IconImageListID.Icon_DriveRam;
                    break;
                case DriveType.Fixed:
                    iconBase = IconImageListID.Icon_DriveHDD;
                    break;
                default:
                    iconBase = IconImageListID.Icon_DriveHDD;
                    break;
            }

            // ドライブ名をオーバーレイで作成
            IconImageListID iconOverray = IconImageListID.None;
            char driveChar = (driveInfo.Name.ToUpper())[0];
            switch (driveChar) {
                case 'A': iconOverray = IconImageListID.Icon_DriveNameA; break;
                case 'B': iconOverray = IconImageListID.Icon_DriveNameB; break;
                case 'C': iconOverray = IconImageListID.Icon_DriveNameC; break;
                case 'D': iconOverray = IconImageListID.Icon_DriveNameD; break;
                case 'E': iconOverray = IconImageListID.Icon_DriveNameE; break;
                case 'F': iconOverray = IconImageListID.Icon_DriveNameF; break;
                case 'G': iconOverray = IconImageListID.Icon_DriveNameG; break;
                case 'H': iconOverray = IconImageListID.Icon_DriveNameH; break;
                case 'I': iconOverray = IconImageListID.Icon_DriveNameI; break;
                case 'J': iconOverray = IconImageListID.Icon_DriveNameJ; break;
                case 'K': iconOverray = IconImageListID.Icon_DriveNameK; break;
                case 'L': iconOverray = IconImageListID.Icon_DriveNameL; break;
                case 'M': iconOverray = IconImageListID.Icon_DriveNameM; break;
                case 'N': iconOverray = IconImageListID.Icon_DriveNameN; break;
                case 'O': iconOverray = IconImageListID.Icon_DriveNameO; break;
                case 'P': iconOverray = IconImageListID.Icon_DriveNameP; break;
                case 'Q': iconOverray = IconImageListID.Icon_DriveNameQ; break;
                case 'R': iconOverray = IconImageListID.Icon_DriveNameR; break;
                case 'S': iconOverray = IconImageListID.Icon_DriveNameS; break;
                case 'T': iconOverray = IconImageListID.Icon_DriveNameT; break;
                case 'U': iconOverray = IconImageListID.Icon_DriveNameU; break;
                case 'V': iconOverray = IconImageListID.Icon_DriveNameV; break;
                case 'W': iconOverray = IconImageListID.Icon_DriveNameW; break;
                case 'X': iconOverray = IconImageListID.Icon_DriveNameX; break;
                case 'Y': iconOverray = IconImageListID.Icon_DriveNameY; break;
                case 'Z': iconOverray = IconImageListID.Icon_DriveNameZ; break;
            }
            Bitmap bmpIcon = CreateDriveOverrayIcon(iconBase, iconOverray);
            return bmpIcon;
        }

        //=========================================================================================
        // 機　能：オーバーレイアイコンを作成する
        // 引　数：[in]iconBase  ベースに表示するアイコン
        // 　　　　[in]iconOverray オーバーレイアイコン
        // 戻り値：作成したアイコンのビットマップ（Disposeが必要）
        //=========================================================================================
        public static Bitmap CreateDriveOverrayIcon(IconImageListID iconBase, IconImageListID iconOverray) {
            Bitmap icon = new Bitmap(UIIconManager.CX_DEFAULT_ICON, UIIconManager.CY_DEFAULT_ICON);
            Graphics g = Graphics.FromImage(icon);
            try {
                UIIconManager.IconImageList.Draw(g, 0, 0, (int)iconBase);
                UIIconManager.IconImageList.Draw(g, 0, 0, (int)iconOverray);
            } finally {
                g.Dispose();
            }
            return icon;
        }

        //=========================================================================================
        // 機　能：ドライブの情報から表示名を作成して返す
        // 引　数：[in]driveInfo  ドライブの情報
        // 戻り値：表示名
        //=========================================================================================
        public static string GetDisplayName(DriveInfo driveInfo) {
            // ドライブの情報を取得
            string driveType = "";
            string volumeName = "";
            switch (driveInfo.DriveType) {
                case DriveType.CDRom:
                    driveType = Resources.ToolbarItem_DriveTypeCDROM;
                    break;
                case DriveType.Removable:
                    driveType = Resources.ToolbarItem_DriveTypeRemovable;
                    break;
                case DriveType.Network:
                    driveType = Resources.ToolbarItem_DriveTypeNetwork;
                    break;
                case DriveType.Ram:
                    driveType = Resources.ToolbarItem_DriveTypeRam;
                    try {
                        volumeName = driveInfo.VolumeLabel;
                    } catch (Exception) {
                    }
                    break;
                case DriveType.Fixed:
                    driveType = Resources.ToolbarItem_DriveTypeFixed;
                    try {
                        volumeName = driveInfo.VolumeLabel;
                    } catch (Exception) {
                    }
                    break;
                default:
                    break;
            }

            // 表示名を作成
            string dispName = driveInfo.RootDirectory.Name[0] + ":";
            if (driveType != "") {
                dispName += " " + driveType;
                if (volumeName != null && volumeName != "") {
                    dispName += ":" + volumeName;
                }
            } else {
                dispName += " " + volumeName;
            }
            return dispName;
        }
    }
}
