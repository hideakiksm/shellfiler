using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShellFiler.Api;
using ShellFiler.FileTask;
using ShellFiler.FileSystem;
using ShellFiler.Document;
using ShellFiler.Util;

namespace ShellFiler.UI.Dialog {

    //=========================================================================================
    // クラス：インテリセンスのUI
    // 　　　　呼び出し元ではインスタンスを常に固定しておき、Show()とHide()でON/OFFを切り替える
    //=========================================================================================
    public partial class IntelliSensePopup : Form {
        // アイコンを除く部分の表示開始Ｘ位置
        public const int CX_ICON_MARGIN = 16;

        // 補完対象のコンボボックス
        private ComboBox m_comboBox;
        
        // 項目一覧の固定部分
        private ItemList m_itemListFixed;
        
        // 項目一覧の使用分
        private ItemList m_itemListForUse;
        
        // 現在表示中のファイル一覧
        private UIFileList m_uiFileList;

        // Windows用のファイルシステム
        private IFileSystem m_windowsFileSystem;

        // 検索語句に対応したファイル一覧（null:取得していない）
        private List<IFile> m_searchWordFileList = null;

        // 検索語句に対応したファイル一覧を取得したディレクトリ（\補完済み、null:取得していない）
        private string m_searchWordFileListBaseDir = null;

        // 現在フォーカス中の項目
        private int m_focusedItemIndex = -1;

        // 検索語句がヒットした範囲の下限（-1～Count）
        private int m_searchRangeMin = 0;

        // 検索語句がヒットした範囲の上限（0～Count+1）、minとmaxが逆転している場合はNotFountを意味する。
        private int m_searchRangeMax = 0;

        //=========================================================================================
        // 機　能：コンストラクタ
        // 引　数：[in]comboBox      補完対象のコンボボックス
        // 　　　　[in]itemListFixed 項目一覧の固定部分
        // 　　　　[in]uiFileList    現在表示中のファイル一覧
        // 戻り値：なし
        //=========================================================================================
        public IntelliSensePopup(ComboBox comboBox, ItemList itemListFixed, UIFileList uiFileList) {
            InitializeComponent();
            m_comboBox = comboBox;
            m_itemListFixed = itemListFixed;
            m_itemListForUse = new ItemList();
            m_itemListForUse.Initialize(m_itemListFixed, new ItemList());
            m_uiFileList = uiFileList;
            m_windowsFileSystem = Program.Document.FileSystemFactory.CreateDefaultFileSystemForFileList();

            this.listViewWordList.VirtualListSize = m_itemListForUse.Count;

            ColumnHeader column = new ColumnHeader();
            column.Width = this.listViewWordList.Width - 20;
            this.listViewWordList.Columns.AddRange(new ColumnHeader[] {column});
        }

        //=========================================================================================
        // 機　能：検索語句をセットする
        // 引　数：[in]searchWord  検索語句
        // 戻り値：検索可能なときtrue、項目が1件もないときfalse
        //=========================================================================================
        public bool SetSearchWord(string searchWord) {
            string oldDir = m_searchWordFileListBaseDir;
            if (GenericFileStringUtils.IsWindowsAbsolutePath(searchWord)) {
                // 語句がWindows絶対パスの場合、ファイル一覧を作成
                string baseDir = m_windowsFileSystem.GetDirectoryName(searchWord);
                baseDir = m_windowsFileSystem.GetFullPath(baseDir);
                if (m_searchWordFileListBaseDir != baseDir) {
                    FileOperationRequestContext cache = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.Windows, FileSystemID.None, null, null, null);
                    FileOperationStatus status = m_windowsFileSystem.GetFileList(cache, baseDir, out m_searchWordFileList);
                    if (status.Succeeded) {
                        m_searchWordFileListBaseDir = baseDir;
                    } else {
                        m_searchWordFileList = null;
                        m_searchWordFileListBaseDir = null;
                    }
                }
            } else if (m_uiFileList.FileSystem.FileSystemId == FileSystemID.Windows) {
                // 語句が表示中Windowsディレクトリの相対パスの場合、ファイル一覧を作成
                IFileSystem uiFileSystem = m_uiFileList.FileSystem;
                string baseDir = m_uiFileList.DisplayDirectoryName + searchWord;
                baseDir = uiFileSystem.GetFullPath(baseDir);
                baseDir = uiFileSystem.GetDirectoryName(baseDir);
                baseDir = uiFileSystem.CompleteDirectoryName(baseDir);
                string uiDir = m_uiFileList.DisplayDirectoryName;
                if (baseDir.ToLower() == uiDir.ToLower()) {
                    // ファイル一覧に含まれるため不要
                    m_searchWordFileList = null;
                    m_searchWordFileListBaseDir = null;
                } else if (m_searchWordFileListBaseDir != baseDir) {
                    FileOperationRequestContext cache = Program.Document.FileSystemFactory.CreateFileOperationRequestContext(BackgroundTaskID.NullId, FileSystemID.Windows, FileSystemID.None, null, null, null);
                    FileOperationStatus status = m_windowsFileSystem.GetFileList(cache, baseDir, out m_searchWordFileList);
                    if (status.Succeeded) {
                        m_searchWordFileListBaseDir = baseDir;
                    } else {
                        m_searchWordFileList = null;
                        m_searchWordFileListBaseDir = null;
                    }
                }
            } else {
                m_searchWordFileList = null;
                m_searchWordFileListBaseDir = null;
            }

            // 使用する語句一覧を作成
            if (oldDir != m_searchWordFileListBaseDir) {
                ItemList variableList = new ItemList();
                if (m_searchWordFileList != null) {
                    foreach (IFile file in m_searchWordFileList) {
                        if (file.Attribute.IsDirectory) {
                            variableList.Add(ItemType.FileSystemDir, file.FileName);
                            variableList.Add(ItemType.FileSystemDir, m_searchWordFileListBaseDir + file.FileName);
                        } else {
                            variableList.Add(ItemType.FileSystemFile, file.FileName);
                            variableList.Add(ItemType.FileSystemFile, m_searchWordFileListBaseDir + file.FileName);
                        }
                    }
                }
                variableList.Sort();
                m_itemListForUse.Initialize(m_itemListFixed, variableList);
                if (m_itemListForUse.Count == 0) {
                    return false;
                }
                this.listViewWordList.VirtualListSize = m_itemListForUse.Count;
                this.listViewWordList.Invalidate();
            }

            // 検索を実行
            if (m_itemListForUse.Count > 0) {
                int oldMin = m_searchRangeMin;
                int oldMax = m_searchRangeMax;
                m_itemListForUse.SearchTargetRange(searchWord, out m_searchRangeMin, out m_searchRangeMax);
                m_focusedItemIndex = Math.Max(0, m_searchRangeMin);
                this.listViewWordList.TopItem = this.listViewWordList.Items[m_focusedItemIndex];
                if (oldMin != m_searchRangeMin || oldMax != m_searchRangeMax) {
                    this.listViewWordList.Invalidate();
                }
            }
            return true;
        }

        //=========================================================================================
        // 機　能：インテリセンスの対象エディットコントロールに対してキーワードを入力する
        // 引　数：[in]cursorMode     入力後のカーソル移動方法
        // 　　　　[in]retrieveAlways 常に結果を取得するときtrue
        // 　　　　[out]keyword       入力されたキーワードを返す変数への参照
        // 　　　　[out]existOther    他に入力候補があるときtrueを返す変数への参照
        // 戻り値：なし
        //=========================================================================================
        public void InputCurrentKeyword(CursorMode cursorMode, bool retrieveAlways, out string keyword, out bool existOther) {
            if (m_itemListForUse.Count == 0) {
                // 該当なし
                keyword = null;
                existOther = false;
                return;
            }
            // 取得可能
            keyword = m_itemListForUse[m_focusedItemIndex].DisplayString;
            if (m_searchRangeMin <= m_focusedItemIndex && m_focusedItemIndex <= m_searchRangeMax) {
                if (m_searchRangeMax - m_searchRangeMin >= 1) {
                    // 指定キーワード内で2件以上
                    int newIndex = m_focusedItemIndex;
                    existOther = true;
                    if (cursorMode == CursorMode.Forward) {
                        if (newIndex >= m_searchRangeMax) {
                            newIndex = m_searchRangeMin;
                        } else {
                            newIndex++;
                        }
                    } else if (cursorMode == CursorMode.Backword) {
                        if (newIndex <= m_searchRangeMin) {
                            newIndex = m_searchRangeMax;
                        } else {
                            newIndex--;
                        }
                    }
                    RedrawItem(m_focusedItemIndex);
                    m_focusedItemIndex = newIndex;
                    this.listViewWordList.TopItem = this.listViewWordList.Items[m_focusedItemIndex];
                    RedrawItem(m_focusedItemIndex);
                } else {
                    // 指定キーワード内で1件確定
                    keyword = m_itemListForUse[m_focusedItemIndex].DisplayString;
                    existOther = false;
                }
            } else {
                if (retrieveAlways) {
                    keyword = m_itemListForUse[m_focusedItemIndex].DisplayString;
                    existOther = false;
                } else {
                    keyword = null;
                    existOther = false;
                }
            }
        }

        //=========================================================================================
        // 機　能：キー入力の処理を行う
        // 引　数：[in]evt  入力されたキーのキーイベント
        // 戻り値：なし
        //=========================================================================================
        public void OnComboboxKeyDown(KeyEventArgs evt) {
            if (m_itemListForUse.Count == 0) {
                return;
            }
            if (evt.KeyCode == Keys.Down) {
                CursorMove(1);
            } else if (evt.KeyCode == Keys.Up) {
                CursorMove(-1);
            } else if (evt.KeyCode == Keys.Prior) {
                int pageSize = this.listViewWordList.Height / this.listViewWordList.Items[0].Bounds.Height - 1;
                CursorMove(-pageSize);
            } else if (evt.KeyCode == Keys.Next) {
                int pageSize = this.listViewWordList.Height / this.listViewWordList.Items[0].Bounds.Height - 1;
                CursorMove(pageSize);
            }
        }

        //=========================================================================================
        // 機　能：カーソルを移動する
        // 引　数：[in]lines  移動量
        // 戻り値：なし
        //=========================================================================================
        private void CursorMove(int lines) {
            int newIndex = Math.Max(0, Math.Min(m_itemListForUse.Count - 1, m_focusedItemIndex + lines));
            if (m_focusedItemIndex != newIndex) {
                RedrawItem(m_focusedItemIndex);
                this.listViewWordList.TopItem = this.listViewWordList.Items[newIndex];
                m_focusedItemIndex = newIndex;
                RedrawItem(m_focusedItemIndex);
            }
        }

        //=========================================================================================
        // 機　能：指定された行をオーナードローによって再描画する
        // 引　数：[in]index  再描画する行
        // 戻り値：なし
        //=========================================================================================
        private void RedrawItem(int index) {
            ListViewItem item = this.listViewWordList.Items[index];
            item.Selected = !item.Selected;             // 選択に意味はないが再描画のために変更
        }

        //=========================================================================================
        // 機　能：オーナードローにより行を描画する
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewWordList_DrawItem(object sender, DrawListViewItemEventArgs evt) {
            if (m_itemListForUse.Count == 0) {
                return;
            }
            // 準備
            evt.DrawDefault = false;
            Graphics g = evt.Graphics;
            int index = evt.ItemIndex;
            Rectangle rcBounds = new Rectangle(evt.Bounds.Left + CX_ICON_MARGIN, evt.Bounds.Top, evt.Bounds.Width - CX_ICON_MARGIN, evt.Bounds.Height);
           
            // 描画モードを決定
            Brush backBrush = SystemBrushes.Window;
            Brush textBrush;
            bool drawFocus = false;
            if (m_searchRangeMin <= index && index <= m_searchRangeMax) {
                if (m_focusedItemIndex == index) {
                    backBrush = SystemBrushes.Highlight;
                    textBrush = SystemBrushes.HighlightText;
                    drawFocus = true;
                } else {
                    textBrush = SystemBrushes.WindowText;
                }
            } else {
                if (m_focusedItemIndex == index) {
                    drawFocus = true;
                }
                textBrush = SystemBrushes.GrayText;
            }

            // 背景を描画
            if (drawFocus) {
                Rectangle shrinkBounds = FormUtils.ShrinkRectangle(rcBounds, 1);
                g.FillRectangle(backBrush, shrinkBounds);
                ControlPaint.DrawFocusRectangle(g, rcBounds);
            } else {
                g.FillRectangle(backBrush, rcBounds);
            }

            // テキストを描画
            StringFormat drawFormat = new StringFormat();
            try {
                g.DrawString(m_itemListForUse[index].DisplayString, evt.Item.Font, textBrush, rcBounds, drawFormat);
            } finally {
                drawFormat.Dispose();
            }

            // アイコンを描画
            ItemType itemType = m_itemListForUse[index].ItemType;
            Bitmap icon = null;
            if (itemType == ItemType.Dictionary) {
                icon = UIIconManager.DlgIntelliShell;
            } else if (itemType == ItemType.FileSystemDir) {
                icon = UIIconManager.DlgIntelliHddDir;
            } else if (itemType == ItemType.FileSystemFile) {
                icon = UIIconManager.DlgIntelliHddFile;
            } else if (itemType == ItemType.TargetPathFile) {
                icon = UIIconManager.DlgIntelliListFile;
            } else if (itemType == ItemType.TargetPathDir) {
                icon = UIIconManager.DlgIntelliListDir;
            }
            g.DrawImage(icon, evt.Bounds.Left + 2, evt.Bounds.Top, UIIconManager.CX_LIST_ICON, UIIconManager.CY_LIST_ICON);
        }

        //=========================================================================================
        // 機　能：オーナーデータにより行情報を返す
        // 引　数：[in]sender   イベントの送信元
        // 　　　　[in]evt      送信イベント
        // 戻り値：なし
        //=========================================================================================
        private void listViewWordList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs evt) {
            // 行情報はオーナードローで管理されるため、ダミーの行情報を返す
            evt.Item = new ListViewItem();
        }

        //=========================================================================================
        // クラス：出力する項目の一覧
        //=========================================================================================
        public class ItemList : IComparer<ItemInfo> {
            // 項目一覧
            private List<ItemInfo> m_itemList = new List<ItemInfo>();
            
            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public ItemList() {
            }

            //=========================================================================================
            // 機　能：2つの項目リストのマージから出力項目を初期化する
            // 引　数：[in]list1  項目リスト1
            // 　　　　[in]list2  項目リスト2
            // 戻り値：なし
            //=========================================================================================
            public void Initialize(ItemList list1, ItemList list2) {
                m_itemList.Clear();
                if (list2.Count == 0) {
                    m_itemList.Capacity = list1.Count;
                    foreach (ItemInfo item in list1.m_itemList) {
                        m_itemList.Add(item);
                    }
                } else {
                    m_itemList.Capacity = list1.Count + list2.Count;
                    int idx1 = 0;
                    int idx2 = 0;
                    while (true) {
                        if (idx1 == list1.Count || idx2 == list2.Count) {
                            break;
                        }
                        int cmp = Compare(list1.m_itemList[idx1], list2.m_itemList[idx2]);
                        if (cmp == -1) {
                            m_itemList.Add(list1.m_itemList[idx1]);
                            idx1++;
                        } else if (cmp == 1) {
                            m_itemList.Add(list2.m_itemList[idx2]);
                            idx2++;
                        } else {
                            m_itemList.Add(list1.m_itemList[idx1]);
                            idx1++;
                            idx2++;
                        }
                    }
                    if (idx1 < list1.Count) {
                        for (int i = idx1; i < list1.Count; i++) {
                            m_itemList.Add(list1.m_itemList[i]);
                        }
                    } else {
                        for (int i = idx2; i < list2.Count; i++) {
                            m_itemList.Add(list2.m_itemList[i]);
                        }
                    }
                }
            }

            //=========================================================================================
            // 機　能：バイナリサーチで表示範囲を検索する
            // 引　数：[in]word      検索語句
            // 　　　　[in]indexMin  最小範囲（-1～Count）
            // 　　　　[in]indexMax  最大範囲（0～Count+1）
            // 戻り値：なし
            // メ　モ：検索結果は範囲として取得される。minとmaxが逆転している状態はNotFoundを意味する。
            //=========================================================================================
            public void SearchTargetRange(string word, out int indexMin, out int indexMax) {
                if (m_itemList.Count == 0) {
                    indexMin = 0;
                    indexMax = 0;
                    return;
                }
                ItemInfo wordItem = new ItemInfo(ItemType.Dummy, word);

                // 下限を検索
                int left = 0;
                int right = m_itemList.Count - 1;
                while (left <= right) {
                    int middle = (left + right) / 2;
                    if (CompareForSearch(m_itemList[middle], wordItem) == -1) {
                        left = middle + 1;
                    } else {
                        right = middle - 1;
                    }
                }
                indexMin = left;

                // 上限を検索
                left = 0;
                right = m_itemList.Count - 1;
                while (left <= right) {
                    int middle = (left + right) / 2;
                    if (CompareForSearch(m_itemList[middle], wordItem) == 1) {
                        right = middle - 1;
                    } else {
                        left = middle + 1;
                    }
                }
                indexMax = right;
            }

            //=========================================================================================
            // 機　能：項目を追加する
            // 引　数：[in]type    項目の種類
            // 　　　　[in]dispStr 表示名
            // 　　　　[in]hintStr ヒント
            // 戻り値：なし
            //=========================================================================================
            public void Add(ItemType type, string dispStr) {
                m_itemList.Add(new ItemInfo(type, dispStr));
            }

            //=========================================================================================
            // 機　能：項目をソートする
            // 引　数：なし
            // 戻り値：なし
            //=========================================================================================
            public void Sort() {
                m_itemList.Sort(0, m_itemList.Count, this);
            }

            //=========================================================================================
            // 機　能：2つのファイルを比較する
            // 引　数：[in]item1  比較対象の項目１
            // 　　　　[in]item2  比較対象の項目２
            // 戻り値：比較結果
            //=========================================================================================
            public int Compare(ItemInfo item1, ItemInfo item2) {
                return string.Compare(item1.SearchKey, item2.SearchKey);
            }
            //=========================================================================================
            // 機　能：2つのファイルを比較する
            // 引　数：[in]target  比較対象
            // 　　　　[in]word    検索キー
            // 戻り値：比較結果
            //=========================================================================================
            public int CompareForSearch(ItemInfo target, ItemInfo word) {
                int len = word.SearchKey.Length;
                return string.Compare(target.SearchKey, 0, word.SearchKey, 0, len);
            }

            //=========================================================================================
            // プロパティ：項目数
            //=========================================================================================
            public int Count {
                get {
                    return m_itemList.Count;
                }
            }

            //=========================================================================================
            // インデクサ
            //=========================================================================================
            public ItemInfo this[int index] {
                get {
                    return m_itemList[index];
                }
            }
        }

        //=========================================================================================
        // クラス：出力する項目の一覧
        //=========================================================================================
        public class ItemInfo {
            // 項目の種類
            private ItemType m_itemType;
            
            // 表示名
            private string m_displayString;

            // 検索キー
            private string m_searchKey;

            //=========================================================================================
            // 機　能：コンストラクタ
            // 引　数：[in]type    項目の種類
            // 　　　　[in]dispStr 表示名
            // 　　　　[in]hintStr ヒント
            // 戻り値：なし
            //=========================================================================================
            public ItemInfo(ItemType type, string dispStr) {
                m_itemType = type;
                m_displayString = dispStr;
                m_searchKey = dispStr.ToLower();
            }

            //=========================================================================================
            // プロパティ：項目の種類
            //=========================================================================================
            public ItemType ItemType {
                get {
                    return m_itemType;
                }
            }

            //=========================================================================================
            // プロパティ：表示名
            //=========================================================================================
            public string DisplayString {
                get {
                    return m_displayString;
                }
            }

            //=========================================================================================
            // プロパティ：検索キー
            //=========================================================================================
            public string SearchKey {
                get {
                    return m_searchKey;
                }
            }
        }

        //=========================================================================================
        // 列挙子：項目の種類
        //=========================================================================================
        public enum ItemType {
            Dummy,                  // ダミー（検索語句用）
            Dictionary,             // コマンド名の辞書
            TargetPathDir,          // 対象パスに表示中のファイル一覧のディレクトリ
            TargetPathFile,         // 対象パスに表示中のファイル一覧のファイル
            FileSystemDir,          // ファイルシステム中のディレクトリ
            FileSystemFile,         // ファイルシステム中のファイル
        }

        //=========================================================================================
        // 列挙子：カーソルの移動方法
        //=========================================================================================
        public enum CursorMode {
            Forward,                // 前方（下方向）に移動
            Backword,               // 後方（上方向）に移動
            Stay,                   // 移動しない
        }
    }
}
