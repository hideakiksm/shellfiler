using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileSystem;
using ShellFiler.Util;
using ShellFiler.UI;

namespace ShellFiler.Command.FileList.Mouse {

    //=========================================================================================
    // クラス：マウスによるマークコマンドの基底クラス
    //=========================================================================================
    public abstract class AbstractMouseMarkCommand : AbstractMouseActionCommand {
        // マウスの前回行位置
        private int m_lastMouseLine = 0;

        // マウスクリック時のマーク方法
        private MouseMarkAction m_markAction;

        // ドラッグ＆ドロップの状態
        private DragState m_dragState = DragState.None;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public AbstractMouseMarkCommand() {
        }

        //=========================================================================================
        // 機　能：機能を実行する
        // 引　数：なし
        // 戻り値：実行結果
        //=========================================================================================
        public override object Execute() {
            Point pos = Cursor.Position;
            pos = FileListViewTarget.PointToClient(pos);
            bool dragPos = FileListComponentTarget.CheckDragStartPosition(pos.X, pos.Y);
            if (dragPos) {
                m_dragState = DragState.Ready;
                m_markAction = MouseMarkAction.MarkSelect;
            }
            FileListComponentTarget.OnMouseDown(this);
            return null;
        }

        //=========================================================================================
        // 機　能：マウスが移動したときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override void OnMouseMove() {
            if (m_dragState == DragState.None) {
                FileListComponentTarget.OnMouseMove(this);
            } else if (m_dragState == DragState.Ready) {
                m_dragState = DragState.OnDragDrop;
                Point pos = Cursor.Position;
                pos = FileListViewTarget.PointToClient(pos);
                FileListComponentTarget.BeginDragDrop(pos.X, pos.Y);
            }
        }

        //=========================================================================================
        // 機　能：マウスのボタンが離されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public override void OnMouseUp() {
            if (m_dragState == DragState.None) {
                FileListComponentTarget.OnMouseUp(this);
            }
        }

        //=========================================================================================
        // プロパティ：マウスクリック時のマーク方法
        //=========================================================================================
        public MouseMarkAction MarkAction {
            get {
                return m_markAction;
            }
            set {
                m_markAction = value;
            }
        } 
               
        //=========================================================================================
        // プロパティ：１つのファイルに対するマーク指示
        //=========================================================================================
        public MarkOperation MarkOperation {
            get {
                switch (m_markAction) {
                    case MouseMarkAction.MarkFirstOnly:
                        return MarkOperation.Mark;
                    case MouseMarkAction.RevertSelect:
                        return MarkOperation.Revert;
                    case MouseMarkAction.ExplorerMark:
                        return MarkOperation.Mark;
                    case MouseMarkAction.MarkSelect:
                        return MarkOperation.Mark;
                    case MouseMarkAction.ClearSelect:
                        return MarkOperation.Clear;
                    default:
                        return MarkOperation.None;
                }
            }
        }
      
        //=========================================================================================
        // プロパティ：マウスの前回行位置
        //=========================================================================================
        public int LastMouseFilePos {
            get {
                return m_lastMouseLine;
            }
            set {
                m_lastMouseLine = value;
            }
        }

        //=========================================================================================
        // 列挙子：ドラッグ＆ドロップの状態
        //=========================================================================================
        private enum DragState {
            // ドラッグ＆ドロップなし
            None,
            // ドラッグ準備OK
            Ready,
            // ドロップ中
            OnDragDrop,
        }
    }
}
