#pragma once

//=========================================================================================
//	クラス：エクスプローラ風メニュー
//=========================================================================================
class ExplorerMenu {
	// 親となるウィンドウハンドル
	HWND m_hwnd;

	// 作成したメニュー
	HMENU m_hMenu;

	// 一時作業用の対象ファイルパス名
	WCHAR* m_wchPath;

	// メニューのIMallocインターフェース
	LPMALLOC m_pMalloc;

	// メニューのIShellFolderインターフェース
	LPSHELLFOLDER m_pShellFolder;

	// メニューのIContextMenuインターフェース
	LPCONTEXTMENU m_pContextMenu;

	// メニューのIContextMenu2インターフェース
	LPCONTEXTMENU2 m_pContextMenu2;

	// IDLの作業用
	LPITEMIDLIST m_pidlMain;

	// IDLの作業用
	LPITEMIDLIST m_pidlNextItem;

public:
	ExplorerMenu();
	~ExplorerMenu();
	BOOL HandleMenuMsg(HWND, UINT, WPARAM, LPARAM);
	HMENU CreateExplorerMenu(HWND hwnd, const WCHAR* wchPath);
	BOOL ExecuteMenuItem(int nCmd);
private:
	UINT getItemCount(LPITEMIDLIST);
	LPITEMIDLIST getNextItem(LPITEMIDLIST);
	LPITEMIDLIST duplicateItem(LPMALLOC, LPITEMIDLIST);
};
