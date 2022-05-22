using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.Document;
using ShellFiler.Document.Setting;
using ShellFiler.Properties;
using ShellFiler.Util;
using SP.Windows;

namespace ShellFiler.UI.FileList.DefaultList {

    //=========================================================================================
    // クラス：ファイル一覧コンポーネントのヘッダ
    //=========================================================================================
    class FilePanelHeader : Panel {
        // セクションの幅の最大値
        private const int MAX_SECTION_WIDTH = 1000;

        // 表示の親となるコンポーネント
        private FileListView m_parent;

        // 初期化済みのファイルシステム
        private FileSystemID m_currentFileSystemId = null;

        // 認識しているコントロールの幅（初期化前は-1）
        private int m_controlWidth = -1;

        // 項目のドラッグ中のスクリーン座標マウス位置
        private Point m_trackingMousePos = Point.Empty;

        // ヘッダの一覧
        private FileListHeaderItem[] m_fileListItem;

        // ヘッダコントロール
        private Header m_headerControl;

        // ヘッダの高さ
        private int m_cyHeader;

        // 現在ソート中の項目(-1:対象カラムがない)
        private int m_headerSortItem = -1;

        // 現在ソート中の方向
        private HeaderSortDirection m_headerSortDirection = HeaderSortDirection.NoSort;

        // 各項目の表示位置
        private Dictionary<FileListHeaderItem.FileListHeaderItemId, FilePanelHeaderPosition> m_itemToPosition = new Dictionary<FileListHeaderItem.FileListHeaderItemId, FilePanelHeaderPosition>();

        // ソートイベントのハンドラ
        public delegate void HeaderSortChangedEventHandler(object sender, HeaderSortChangedEventArgs evt); 

        // ソートイベント
        public event HeaderSortChangedEventHandler HeaderSortChanged; 

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]parent     表示の親となるビュー
        // 戻り値：なし
        //=========================================================================================
        public FilePanelHeader(FileListView parent) {
            m_parent = parent;

            // コントロールの初期化
            m_headerControl = new Header();
            m_headerControl.Location = new Point(0, 0);
            m_headerControl.SectionTracking += new HeaderSectionWidthConformableEventHandler(HeaderControl_SectionTracking);
            m_headerControl.SectionClick += new HeaderSectionEventHandler(HeaderControl_SectionClick);
            m_headerControl.DividerDblClick += new HeaderSectionEventHandler(HeaderControl_DividerDblClick);
            m_headerControl.Clickable = true;
            m_cyHeader = MainWindowForm.X(m_headerControl.Size.Height);
            this.Controls.Add(m_headerControl);
            m_parent.AutoScroll = false;
            this.AutoSize = false;
            this.Dock = DockStyle.None;
            this.Size = new Size(m_parent.ClientRectangle.Width, m_cyHeader);
            m_parent.Controls.Add(this);
        }
        
        //=========================================================================================
        // 機　能：コントロールの後始末を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void DisposeControl() {
            m_parent.Controls.Remove(this);
        }

        //=========================================================================================
        // 機　能：水平スクロールバーの位置が変更されたときの処理を行う
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public void OnHScroll() {
            m_headerControl.Left = -m_parent.HorzScrollPosition;
            m_headerControl.Size = new Size(m_parent.HorzScrollPosition + m_parent.Width, m_cyHeader);
        }

        //=========================================================================================
        // 機　能：ファイルパネルヘッダを新しい一覧に合わせてリフレッシュする
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        public bool RefreshFilePanelHeader() {
            // 一覧項目に変化がないなら処理をスキップ
            if (m_currentFileSystemId == m_parent.FileList.FileSystem.FileSystemId) {
                // ソート状態のみ更新
                UpdateSortState();
                return false;
            }

            m_currentFileSystemId = m_parent.FileList.FileSystem.FileSystemId;
            m_fileListItem = m_parent.FileList.FileSystem.FileListHeaderItemList;

            // UIのカラム数を調整
            int sectionCount = m_headerControl.Sections.Count;
            int itemCount = m_fileListItem.Length;
            if (sectionCount < itemCount) {
                // UIのほうが少ない
                for (int i = 0; i < itemCount - sectionCount; i++) {
        			m_headerControl.Sections.Add(new HeaderSection("", 0));
                }
            } else {
                // UIのほうが多い
                for (int i = 0; i < sectionCount - itemCount; i++) {
                    m_headerControl.Sections.RemoveAt(m_headerControl.Sections.Count - 1);
                }
            }

            // ヘッダの項目を更新
            for (int i = 0; i < m_fileListItem.Length; i++) {
                FileListHeaderItem item = m_fileListItem[i];
                m_headerControl.Sections[i].Text = item.DisplayName;
            }

            // ソート状態を更新
            UpdateSortState();

            // 最小幅(ウィンドウサイズ非依存の固定値)をあらかじめ計算
            m_fileListItem = m_parent.FileList.FileSystem.FileListHeaderItemList;
            FileListGraphics g = new FileListGraphics(m_parent, 0, 0);
            try {
                m_itemToPosition.Clear();
                foreach (FileListHeaderItem item in m_fileListItem) {
                    FilePanelHeaderPosition pos = new FilePanelHeaderPosition();
                    const int MARGIN = 10;
                    int minWidth = (int)(GraphicsUtils.MeasureString(g.Graphics, g.FileListFont, item.WidthSample) + 1);
                    pos.MinWidth = minWidth + MARGIN + FileLineRenderer.ITEM_MARGIN * 2;
                    m_itemToPosition.Add(item.ItemID, pos);
                }
            } finally {
                g.Dispose();
            }

            // ソートマークを更新
            ResizeHeader();
            m_parent.FileListViewComponent.OnSizeChange();
            return true;
        }

        //=========================================================================================
        // 機　能：ファイル一覧更新の際にソート状態をUIに反映する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void UpdateSortState() {
            // コンフィグからメンバにソート情報を反映
            FileListSortMode sortMode = m_parent.FileList.SortMode;
            SortMethodToHeaderItem(sortMode.SortOrder1, sortMode.SortDirection1, out m_headerSortItem, out m_headerSortDirection);

            // メンバからヘッダに反映
            for (int i = 0; i < m_fileListItem.Length; i++) {
                if (m_headerSortItem == i) {
                    m_headerControl.Sections[i].SortMark = HeaderSortDirection.GetUISortMark(m_headerSortDirection, m_fileListItem[m_headerSortItem].ItemID);
                    bool useSub = (m_fileListItem[i].ItemID == FileListHeaderItem.FileListHeaderItemId.FileName);
                    if (useSub) {
                        if (m_headerSortDirection == HeaderSortDirection.NormalSub || m_headerSortDirection == HeaderSortDirection.ReverseSub) {
                            m_headerControl.Sections[i].Text = m_fileListItem[i].DisplayName + " (拡張子順)";
                        } else {
                            m_headerControl.Sections[i].Text = m_fileListItem[i].DisplayName;
                        }
                    }
                } else{
                    m_headerControl.Sections[i].SortMark = HeaderSectionSortMarks.Non;
                }
            }
        }

        //=========================================================================================
        // 機　能：ウィンドウの幅が更新されたときの処理を行う
        // 引　数：[in]resetHeaderPos  ウィンドウ幅変更のためヘッダ位置をリセットするときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnSizeChange(bool resetHeaderPos) {
            if (m_itemToPosition.Count == 0) {
                return;
            }
            if (m_controlWidth == -1 || resetHeaderPos) {
                ResizeHeader();
            }
            if (m_controlWidth != m_parent.ClientRectangle.Width) {
                m_controlWidth = m_parent.ClientRectangle.Width;
                this.Size = new Size(m_controlWidth, m_cyHeader);
            }
            if (resetHeaderPos) {
                m_headerControl.Left = 0;
            }

            // UIに反映
            m_headerControl.Left = -m_parent.HorzScrollPosition;
            m_headerControl.Size = new Size(m_parent.HorzScrollPosition + m_controlWidth, m_cyHeader);
            for (int i = 0; i < m_fileListItem.Length; i++) {
                int width = m_itemToPosition[m_fileListItem[i].ItemID].CurrentWidth;
                m_headerControl.Sections[i].Width = width;
            }
        }

        //=========================================================================================
        // 機　能：ヘッダのサイズを再計算する
        // 引　数：なし
        // 戻り値：なし
        //=========================================================================================
        private void ResizeHeader() {
            if (m_itemToPosition.Count == 0) {
                return;
            }

            // 最小幅を計算
            int cxWindow = m_parent.Width - SystemInformation.VerticalScrollBarWidth - 6;
            int cxRequiredAll = 0;                  // 最小幅
            int cxFixed = 0;                        // 固定領域の幅の合計
            int cxVariableItem = 0;                 // 可変領域の幅
            foreach (FileListHeaderItem fileListItem in m_fileListItem) {
                int minWidth = m_itemToPosition[fileListItem.ItemID].MinWidth;
                cxRequiredAll += minWidth;
                if (!fileListItem.Variable) {
                    cxFixed += minWidth;
                } else {
                    cxVariableItem = minWidth;
                }
            }

            // 位置を決定
            int cxVariable = Math.Max(cxVariableItem, cxWindow - cxFixed);       // 可変領域の幅
            int xPos = 0;                                                        // 現在のＸ位置
            foreach (FileListHeaderItem fileListItem in m_fileListItem) {
                FilePanelHeaderPosition position = m_itemToPosition[fileListItem.ItemID];
                position.XPos = xPos;
                if (fileListItem.Variable) {
                    position.CurrentWidth = cxVariable;
                    xPos += cxVariable;
                } else {
                    position.CurrentWidth = position.MinWidth;
                    xPos += position.MinWidth;
                }
            }
        }

        //=========================================================================================
        // 機　能：表示項目に対するＸ位置を返す
        // 引　数：[in]item     調査する項目
        // 戻り値：Ｘ位置
        //=========================================================================================
        public int GetItemXPos(FileListHeaderItem.FileListHeaderItemId item) {
            return m_itemToPosition[item].XPos - m_parent.HorzScrollPosition;
        }
        
        //=========================================================================================
        // 機　能：表示項目に対する幅を返す
        // 引　数：[in]item     調査する項目
        // 戻り値：表示幅
        //=========================================================================================
        public int GetItemWidth(FileListHeaderItem.FileListHeaderItemId item) {
            return m_itemToPosition[item].CurrentWidth;
        }

        //=========================================================================================
        // 機　能：ヘッダがクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void HeaderControl_SectionClick(object sender, HeaderSectionEventArgs evt) {
            // こちらにカーソルを移動
            if (!m_parent.HasCursor) {
                m_parent.ToggleCursorLeftRight();
            }

            int index = evt.Item.Index;
            bool useSub = (m_fileListItem[index].ItemID == FileListHeaderItem.FileListHeaderItemId.FileName);
            if (m_headerSortItem == index) {
                // 次の状態へ
                m_headerSortDirection = HeaderSortDirection.GetNextDirection(m_headerSortDirection, useSub);
            } else {
                // 新しい項目
                if (m_headerSortItem != -1) {
                    m_headerControl.Sections[m_headerSortItem].ImageIndex = -1;
                }
                m_headerSortItem = index;
                m_headerSortDirection = HeaderSortDirection.Normal;
            }
            m_headerControl.Sections[m_headerSortItem].SortMark = HeaderSortDirection.GetUISortMark(m_headerSortDirection, m_fileListItem[m_headerSortItem].ItemID);
            if (useSub) {
                if (m_headerSortDirection == HeaderSortDirection.NormalSub || m_headerSortDirection == HeaderSortDirection.ReverseSub) {
                    m_headerControl.Sections[m_headerSortItem].Text = m_fileListItem[index].DisplayName + " (拡張子順)";
                } else {
                    m_headerControl.Sections[m_headerSortItem].Text = m_fileListItem[index].DisplayName;
                }
            }

            if (m_headerSortDirection == HeaderSortDirection.NoSort) {
                m_headerSortItem = -1;
            }

            FileListSortMode.Method method;
            FileListSortMode.Direction direction;
            HeaderItemToSortMethod(m_headerSortItem, m_headerSortDirection, out method, out direction);
            HeaderSortChanged(this, new HeaderSortChangedEventArgs(method, direction));
        }

        //=========================================================================================
        // 機　能：Documentのソート方法をヘッダのソート方法に変換する
        // 引　数：[in]method         Documentのソート方法
        // 　　　　[in]direction      Documentのソートの向き
        // 　　　　[out]itemIndex     ソート中のヘッダ項目のインデックスを返す変数
        // 　　　　[out]itemDirection ヘッダのソートの向きを返す変数
        // 戻り値：なし
        //=========================================================================================
        private void SortMethodToHeaderItem(FileListSortMode.Method method, FileListSortMode.Direction direction, out int itemIndex, out HeaderSortDirection itemDirection) {
            for (int i = 0; i < m_fileListItem.Length; i++) {
                switch (m_fileListItem[i].ItemID) {
                    case FileListHeaderItem.FileListHeaderItemId.FileName:
                        if (method == FileListSortMode.Method.FileName) {
                            itemIndex = i;
                            itemDirection = HeaderSortDirection.SortModeToHeaderInfo(direction, false);
                            return;
                        } else if (method == FileListSortMode.Method.Extension) {
                            itemIndex = i;
                            itemDirection = HeaderSortDirection.SortModeToHeaderInfo(direction, true);
                            return;
                        }
                        break;
                    case FileListHeaderItem.FileListHeaderItemId.FileSize:
                        if (method == FileListSortMode.Method.FileSize) {
                            itemIndex = i;
                            itemDirection = HeaderSortDirection.SortModeToHeaderInfo(direction, false);
                            return;
                        }
                        break;
                    case FileListHeaderItem.FileListHeaderItemId.ModifiedTime:
                        if (method == FileListSortMode.Method.DateTime) {
                            itemIndex = i;
                            itemDirection = HeaderSortDirection.SortModeToHeaderInfo(direction, false);
                            return;
                        }
                        break;
                    case FileListHeaderItem.FileListHeaderItemId.Attribute:
                        if (method == FileListSortMode.Method.Attribute) {
                            itemIndex = i;
                            itemDirection = HeaderSortDirection.SortModeToHeaderInfo(direction, false);
                            return;
                        }
                        break;
                }
            }
            itemIndex = -1;
            itemDirection = HeaderSortDirection.NoSort;
        }

        //=========================================================================================
        // 機　能：ヘッダのソート方法をDocumentのソート方法に変換する
        // 引　数：[in]index          ソート中のヘッダ項目のインデックス
        // 　　　　[in]itemDirection  ヘッダのソートの向き
        // 　　　　[out]method        Documentのソート方法を返す変数
        // 　　　　[out]direction     Documentのソートの向きを返す変数
        // 戻り値：なし
        //=========================================================================================
        private void HeaderItemToSortMethod(int index, HeaderSortDirection headerDirec, out FileListSortMode.Method method, out FileListSortMode.Direction direction) {
            if (index == -1) {
                method = FileListSortMode.Method.NoSort;
                direction = FileListSortMode.Direction.Normal;
            } else {
                if (m_fileListItem[index].ItemID == FileListHeaderItem.FileListHeaderItemId.FileName) {
                    if (headerDirec == HeaderSortDirection.Normal || headerDirec == HeaderSortDirection.Reverse) {
                        method = FileListSortMode.Method.FileName;
                        direction = HeaderSortDirection.HeaderInfoToSortMode(headerDirec);
                    } else {
                        method = FileListSortMode.Method.Extension;
                        direction = HeaderSortDirection.HeaderInfoToSortMode(headerDirec);
                    }
                } else if (m_fileListItem[index].ItemID == FileListHeaderItem.FileListHeaderItemId.FileSize) {
                    method = FileListSortMode.Method.FileSize;
                    direction = HeaderSortDirection.HeaderInfoToSortMode(headerDirec);
                } else if (m_fileListItem[index].ItemID == FileListHeaderItem.FileListHeaderItemId.ModifiedTime) {
                    method = FileListSortMode.Method.DateTime;
                    direction = HeaderSortDirection.HeaderInfoToSortMode(headerDirec);
                } else if (m_fileListItem[index].ItemID == FileListHeaderItem.FileListHeaderItemId.Attribute) {
                    method = FileListSortMode.Method.Attribute;
                    direction = HeaderSortDirection.HeaderInfoToSortMode(headerDirec);
                } else {
                    method = FileListSortMode.Method.NoSort;
                    direction = FileListSortMode.Direction.Normal;
                }
            }
        }

        //=========================================================================================
        // 機　能：ヘッダのセパレータがダブルクリックされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        void HeaderControl_DividerDblClick(object sender, HeaderSectionEventArgs evt) {
            // こちらにカーソルを移動
            if (!m_parent.HasCursor) {
                m_parent.ToggleCursorLeftRight();
            }

            // ダブルクリックされたカラムを最小幅にする
            int index = evt.Item.Index;
            FileListHeaderItem headerItem = m_fileListItem[index];
            FilePanelHeaderPosition pos = m_itemToPosition[headerItem.ItemID];
            pos.CurrentWidth = pos.MinWidth;
            m_headerControl.Sections[index].Width = pos.CurrentWidth;

            // 残りのカラム位置を再計算する
            int xPos = pos.XPos + pos.CurrentWidth;
            for (int i = index + 1; i < m_fileListItem.Length; i++) {
                FileListHeaderItem itemNext = m_fileListItem[i];
                FilePanelHeaderPosition posNext = m_itemToPosition[itemNext.ItemID];
                posNext.XPos = xPos;
                xPos += posNext.CurrentWidth;
            }
            m_parent.FileListViewComponent.OnSizeChange();
            m_parent.Invalidate();
        }

        //=========================================================================================
        // 機　能：ヘッダのセパレータがドラッグされたときの処理を行う
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void HeaderControl_SectionTracking(object sender, HeaderSectionWidthConformableEventArgs evt) {
            // こちらにカーソルを移動
            if (!m_parent.HasCursor) {
                m_parent.ToggleCursorLeftRight();
            }
           
            // 実行条件を確認
            int index = evt.Item.Index;
            FileListHeaderItem headerItem = m_fileListItem[index];
            FilePanelHeaderPosition pos = m_itemToPosition[headerItem.ItemID];
            Point mousePosition = Cursor.Position;
            if (mousePosition.Equals(m_trackingMousePos)) {
                return;
            }
            if (evt.Width > MainWindowForm.X(MAX_SECTION_WIDTH)) {
                evt.Accepted = false;
                return;
            }
            m_trackingMousePos = mousePosition;
            // 水平スクロールバーが少し右にある状態で、ヘッダの長さを短くドラッグすると、
            // ヘッダのサイズ変更→要求サイズ縮小→水平スクロールバー左スクロール→
            // ヘッダの表示位置変更→相対位置からのドラッグ発生→ヘッダのサイズ変更→…
            // が発生する。防止のため、マウス位置が移動していない場合は処理しないようにする。
            
            // ヘッダの位置を調整する
            pos.CurrentWidth = evt.Width;
            m_headerControl.Sections[index].Width = pos.CurrentWidth;

            // 残りのカラム位置を再計算する
            int xPos = pos.XPos + pos.CurrentWidth;
            for (int i = index + 1; i < m_fileListItem.Length; i++) {
                FileListHeaderItem itemNext = m_fileListItem[i];
                FilePanelHeaderPosition posNext = m_itemToPosition[itemNext.ItemID];
                posNext.XPos = xPos;
                xPos += posNext.CurrentWidth;
            }
            m_parent.Invalidate();
            m_parent.FileListViewComponent.OnSizeChange();
        }

        //=========================================================================================
        // 機　能：状態一覧パネルのアクティブ状態を設定する
        // 引　数：[in]isActive  状態一覧パネルがアクティブになったときtrue
        // 戻り値：なし
        //=========================================================================================
        public void OnActivateStateListPanel(bool isActive) {
            if (!isActive) {
                // ヘッダがアクティブになった
                UpdateSortState();
            } else {
                // ヘッダが無効化された
                for (int i = 0; i < m_headerControl.Sections.Count; i++) {
                    m_headerControl.Sections[i].SortMark = HeaderSectionSortMarks.Non;
                }
            }
        }

        //=========================================================================================
        // プロパティ：表示項目の一覧
        //=========================================================================================
        public FileListHeaderItem[] FileListHeaderItemList {
            get {
                return m_fileListItem;
            }
        }

        //=========================================================================================
        // プロパティ：ウィンドウの最小幅
        //=========================================================================================
        public int MinimumWidth {
            get {
                int width = 0;
                foreach (FilePanelHeaderPosition position in m_itemToPosition.Values) {
                    width += position.MinWidth;
                }
                return width;
            }
        }

        //=========================================================================================
        // プロパティ：ヘッダで要求された画面の幅
        //=========================================================================================
        public int RequiredWidth {
            get {
                int width = 0;
                foreach (FilePanelHeaderPosition pos in m_itemToPosition.Values) {
                    width += pos.CurrentWidth;
                }
                return width;
            }
        }

        //=========================================================================================
        // プロパティ：ヘッダの高さ
        //=========================================================================================
        public int CyHeader {
            get {
                return m_cyHeader;
            }
        }

        //=========================================================================================
        // クラス：ソートイベントの引数
        //=========================================================================================
        public class HeaderSortChangedEventArgs {
            // ソートする項目
            private FileListSortMode.Method m_sortMethod;
            
            // ソートの向き
            private FileListSortMode.Direction m_sortDirection;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]sortMethod     ソートする項目
            // 　　　　[in]sortDirection  ソートの向き
            // 戻り値：表示幅
            //=========================================================================================
            public HeaderSortChangedEventArgs(FileListSortMode.Method sortMethod, FileListSortMode.Direction sortDirection) {
                m_sortMethod = sortMethod;
                m_sortDirection = sortDirection;
            }

            //=========================================================================================
            // プロパティ：ソートする項目
            //=========================================================================================
            public FileListSortMode.Method SortMode {
                get {
                    return m_sortMethod;
                }
            }
            
            //=========================================================================================
            // プロパティ：ソートの向き
            //=========================================================================================
            public FileListSortMode.Direction SortDirection {
                get {
                    return m_sortDirection;
                }
            }
        }

        //=========================================================================================
        // クラス：ヘッダのソートの向き
        //=========================================================================================
        private class HeaderSortDirection {
            public static readonly HeaderSortDirection NoSort     = new HeaderSortDirection("NoSort");          // ソートなし
            public static readonly HeaderSortDirection Normal     = new HeaderSortDirection("Normal");          // 順方向
            public static readonly HeaderSortDirection Reverse    = new HeaderSortDirection("Reverse");         // 逆方向
            public static readonly HeaderSortDirection NormalSub  = new HeaderSortDirection("NormalSub");       // サブカラムで順方向
            public static readonly HeaderSortDirection ReverseSub = new HeaderSortDirection("ReverseSub");      // サブカラムで逆方向
            
            // 表示名（デバッグ用）
            private string m_name;

        
            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]name  表示名（デバッグ用）
            // 戻り値：なし
            //=========================================================================================
            public HeaderSortDirection(string name) {
                m_name = name;
            }

            //=========================================================================================
            // 機　能：次の項目に該当する向きを返す
            // 引　数：[in]current  現在の向き
            // 　　　　[in]useSub   サブカラム（拡張子によるソート）を含めた順番で取得するときtrue
            // 戻り値：次の向き
            //=========================================================================================
            public static HeaderSortDirection GetNextDirection(HeaderSortDirection current, bool useSub) {
                if (useSub) {
                    // サブカラムあり
                    if (current == HeaderSortDirection.NoSort) {
                        return HeaderSortDirection.Normal;
                    } else if (current == HeaderSortDirection.Normal) {
                        return HeaderSortDirection.Reverse;
                    } else if (current == HeaderSortDirection.Reverse) {
                        return HeaderSortDirection.NormalSub;
                    } else if (current == HeaderSortDirection.NormalSub) {
                        return HeaderSortDirection.ReverseSub;
                    } else {
                        return HeaderSortDirection.NoSort;
                    }
                } else {
                    // サブカラムなし
                    if (current == HeaderSortDirection.NoSort) {
                        return HeaderSortDirection.Normal;
                    } else if (current == HeaderSortDirection.Normal) {
                        return HeaderSortDirection.Reverse;
                    } else {
                        return HeaderSortDirection.NoSort;
                    }
                }
            }

            //=========================================================================================
            // 機　能：Documentのソートの向きをヘッダのソートの向きに変換する
            // 引　数：[in]direction   Documentのソートの向き
            // 　　　　[in]subColumn   サブカラムの向きを返すときtrue
            // 戻り値：ヘッダのソートの向き
            //=========================================================================================
            public static HeaderSortDirection SortModeToHeaderInfo(FileListSortMode.Direction direction, bool subColumn) {
                if (!subColumn) {
                    if (direction == FileListSortMode.Direction.Normal) {
                        return HeaderSortDirection.Normal;
                    } else {
                        return HeaderSortDirection.Reverse;
                    }
                } else {
                    if (direction == FileListSortMode.Direction.Normal) {
                        return HeaderSortDirection.NormalSub;
                    } else {
                        return HeaderSortDirection.ReverseSub;
                    }
                }
            }

            //=========================================================================================
            // 機　能：ヘッダのソートの向きをDocumentのソートの向きに変換する
            // 引　数：[in]headerDirec   ヘッダのソートの向き
            // 戻り値：Documentのソートの向き
            //=========================================================================================
            public static FileListSortMode.Direction HeaderInfoToSortMode(HeaderSortDirection headerDirec) {
                if (headerDirec == HeaderSortDirection.Reverse || headerDirec == HeaderSortDirection.ReverseSub) {
                    return FileListSortMode.Direction.Reverse;
                } else {
                    return FileListSortMode.Direction.Normal;
                }
            }

            //=========================================================================================
            // 機　能：ソートの向きからヘッダUIのイメージリストのインデックスを返す
            // 引　数：[in]direction  ソートの向き
            // 　　　　[in]itemId     項目の内容のID
            // 戻り値：イメージリストのインデックス（-1:解除）
            //=========================================================================================
            public static HeaderSectionSortMarks GetUISortMark(HeaderSortDirection direction, FileListHeaderItem.FileListHeaderItemId itemId) {
                if (itemId == FileListHeaderItem.FileListHeaderItemId.ModifiedTime) {
                    // 更新時刻は向きが逆、サブ項目もなし
                    if (direction == HeaderSortDirection.Normal) {
                        return HeaderSectionSortMarks.Down;
                    } else if (direction == HeaderSortDirection.Reverse) {
                        return HeaderSectionSortMarks.Up;
                    } else {
                        return HeaderSectionSortMarks.Non;
                    }
                } else {
                    // 通常の項目
                    if (direction == HeaderSortDirection.Normal) {
                        return HeaderSectionSortMarks.Up;
                    } else if (direction == HeaderSortDirection.NormalSub) {
                        return HeaderSectionSortMarks.Up;
                    } else if (direction == HeaderSortDirection.Reverse) {
                        return HeaderSectionSortMarks.Down;
                    } else if (direction == HeaderSortDirection.ReverseSub) {
                        return HeaderSectionSortMarks.Down;
                    } else {
                        return HeaderSectionSortMarks.Non;
                    }
                }
            }
        }
    }
}
