using System.IO;
using ShellFiler.Properties;

namespace ShellFiler.FileSystem.Windows {

    //=========================================================================================
    // クラス：ドライブ情報のユーティリティ
    //=========================================================================================
    class DriveTypeUtil {

        //=========================================================================================
        // 機　能：DriveTypeを表示用文字列に変換する
        // 引　数：[in]type  DriveType
        // 戻り値：表示用文字列
        //=========================================================================================
        public static string TypeToDisplayString(DriveType type) {
            string dispName = "";
            switch (type) {
                case DriveType.CDRom:
                    dispName = Resources.DriveTypeCDRom;
                    break;
                case DriveType.Fixed:
                    dispName = Resources.DriveTypeFixed;
                    break;
                case DriveType.Network:
                    dispName = Resources.DriveTypeNetwork;
                    break;
                case DriveType.Ram:
                    dispName = Resources.DriveTypeRam;
                    break;
                case DriveType.Removable:
                    dispName = Resources.DriveTypeRemovable;
                    break;
                default:
                    dispName = Resources.DriveTypeUnknown;
                    break;
            }
            return dispName;
        }
    }
}
