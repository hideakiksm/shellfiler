#include "pch.h"
#include "ExplorerMenu.h"

//=========================================================================================
// 機　能：コンストラクタ
// 引　数：なし
// 戻り値：なし
//=========================================================================================
ExplorerMenu::ExplorerMenu()
{
	m_hwnd = NULL;
	m_hMenu = NULL;
	m_wchPath = NULL;
	m_pMalloc = NULL;
	m_pShellFolder = NULL;
	m_pContextMenu = NULL;
	m_pContextMenu2 = NULL;
	m_pidlMain = NULL;
	m_pidlNextItem = NULL;
}

//=========================================================================================
// 機　能：デストラクタ
// 引　数：なし
// 戻り値：なし
//=========================================================================================
ExplorerMenu::~ExplorerMenu()
{
	if(m_hMenu != NULL) {
		DestroyMenu(m_hMenu);
		m_hMenu = NULL;
	}
	if(m_pContextMenu2 != NULL) {
		m_pContextMenu2->Release();
		m_pContextMenu2 = NULL;
	}
	if(m_pContextMenu != NULL) {
		m_pContextMenu->Release();
		m_pContextMenu = NULL;
	}
	if(m_pShellFolder != NULL) {
		m_pShellFolder->Release();
		m_pShellFolder = NULL;
	}
	if (m_wchPath != NULL) {
		delete[] m_wchPath;
		m_wchPath = NULL;
	}
	if(m_pMalloc != NULL) {
		if(m_pidlMain != NULL) {
			m_pMalloc->Free(m_pidlMain);
			m_pidlMain = NULL;
		}
		if(m_pidlNextItem != NULL) {
			m_pMalloc->Free(m_pidlNextItem);
			m_pidlNextItem = NULL;
		}
		m_pMalloc->Release();
		m_pMalloc = NULL;
	}
}

//=========================================================================================
// 機　能：エクスプローラのメニューでのメッセージを処理する
// 引　数：[in]hwnd     ウィンドウハンドル
// 　　　　[in]message  ウィンドウメッセージ
// 　　　　[in]wParam   WPARAM値
// 　　　　[in]lParam   LPARAM値
// 戻り値：メッセージを処理したときTRUE
//=========================================================================================
BOOL ExplorerMenu::HandleMenuMsg(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	// WM_INITMENUPOPUP
	// WM_DRAWITEM
	// WM_MEASUREITEM
	if (m_pContextMenu2 != NULL && hwnd == m_hwnd) {
		m_pContextMenu2->HandleMenuMsg(message, wParam, lParam);
		return TRUE;
	} else {
		return FALSE;
	}
}

//=========================================================================================
// 機　能：エクスプローラのメニューを作成する
// 引　数：[in]hwnd     表示するウィンドウのハンドル
// 　　　　[in]wchPath  対象ファイルのパス名
// 戻り値：メニューのハンドル（NULL:失敗）
//=========================================================================================
HMENU ExplorerMenu::CreateExplorerMenu(HWND hwnd, const WCHAR* wchPath)
{
	HRESULT hRes;
	m_hwnd = hwnd;

	m_wchPath = new WCHAR[wcslen(wchPath) + 1];
#pragma warning (disable:4996)
	wcscpy(m_wchPath, wchPath);

	// ShellFolderインターフェースを取得
	hRes = SHGetMalloc(&m_pMalloc);
	if(FAILED(hRes)) {
		return NULL;
	}
	hRes = SHGetDesktopFolder(&m_pShellFolder);
	if(FAILED(hRes)) {
		return NULL;
	}

	// 対象オブジェクトのPIDLを取得
	ULONG ulAttr = 0;
	hRes = m_pShellFolder->ParseDisplayName(hwnd, NULL, m_wchPath, NULL, &m_pidlMain, &ulAttr);
	if(FAILED(hRes) || (m_pidlMain == NULL)) {
		return NULL;
	}
	UINT nCount = getItemCount(m_pidlMain);
	if(nCount == 0) {
		return NULL;
	}
	LPITEMIDLIST pidlItem = m_pidlMain;
	while(--nCount) {
		m_pidlNextItem = duplicateItem(m_pMalloc, pidlItem);
		if(m_pidlNextItem == NULL) {
			return NULL;
		}
		LPSHELLFOLDER psfNextFolder;
		hRes = m_pShellFolder->BindToObject(m_pidlNextItem, NULL, IID_IShellFolder, (void**)(&psfNextFolder));
		if(FAILED(hRes)) {
			return NULL;
		}
		m_pShellFolder->Release();
		m_pShellFolder = psfNextFolder;
		m_pMalloc->Free(m_pidlNextItem);
		m_pidlNextItem = NULL;
		pidlItem = getNextItem(pidlItem);
	}

	// 対象オブジェクトを取得
	LPCITEMIDLIST* ppidl = (LPCITEMIDLIST*)&pidlItem;
	hRes = m_pShellFolder->GetUIObjectOf(hwnd, 1, ppidl, IID_IContextMenu, NULL, (void**)(&m_pContextMenu));
	if(FAILED(hRes)) {
		return NULL;
	}
	hRes = m_pContextMenu->QueryInterface(IID_IContextMenu2, (void**)(&m_pContextMenu2));
	if(FAILED(hRes)) {
		return NULL;
	}

	// メニューを作成
	m_hMenu = CreatePopupMenu();
	hRes = m_pContextMenu2->QueryContextMenu(m_hMenu, 0, 1, 0x7fff, CMF_EXPLORE);
	if(FAILED(hRes)) {
		return NULL;
	}

	return m_hMenu;
}

//=========================================================================================
// 機　能：エクスプローラのメニュー項目を実行する
// 引　数：[in]nCmd  コマンドの項目
// 戻り値：実行に成功したときTRUE
//=========================================================================================
BOOL ExplorerMenu::ExecuteMenuItem(int nCmd)
{
	// メニュー項目を実行
	HRESULT hRes;
	CMINVOKECOMMANDINFO ici;
	ici.cbSize			= sizeof(CMINVOKECOMMANDINFO);
	ici.fMask			= 0;
	ici.hwnd			= m_hwnd;
	ici.lpVerb			= (LPCSTR)MAKEINTRESOURCE(nCmd - 1);
	ici.lpParameters	= NULL;
	ici.lpDirectory		= NULL;
	ici.nShow			= SW_SHOWNORMAL;
	ici.dwHotKey		= 0;
	ici.hIcon			= NULL;
	hRes = m_pContextMenu->InvokeCommand(&ici);
	if(FAILED(hRes)) {
		return FALSE;
	}
	return TRUE;
}

//=========================================================================================
// 機　能：アイテムIDリストのIDの数を計算する
// 引　数：[in]pidl	アイテムIDリストへのポインタ
// 戻り値：アイテムIDの数
//=========================================================================================
UINT ExplorerMenu::getItemCount(LPITEMIDLIST pidl)
{
	USHORT nLen;
	UINT nCount;

	nCount = 0;
	while((nLen = pidl->mkid.cb) != 0) {
		pidl = getNextItem(pidl);
		nCount++;
	}
	return nCount;
}

//=========================================================================================
// 機　能：次のアイテムを得る
// 引　数：[in]pidl	アイテムIDリストへのポインタ
// 戻り値：次のアイテムへのポインタ
//=========================================================================================
LPITEMIDLIST ExplorerMenu::getNextItem(LPITEMIDLIST pidl)
{
	USHORT nLen;

	if((nLen = pidl->mkid.cb) == 0) {
		return NULL;
	}

	return (LPITEMIDLIST)(((LPBYTE)pidl) + nLen);
}

//=========================================================================================
// 機　能：アイテムIDリストの次のアイテムのコピーを作る
// 引　数：[in]pMalloc	IMallocインタフェースへのポインタ
// 　　　　[in]pidl     アイテムIDリストへのポインタ
// 戻り値：コピーされたアイテムIDを含むITEMIDLISTへのポインタ
//=========================================================================================
LPITEMIDLIST ExplorerMenu::duplicateItem(LPMALLOC pMalloc, LPITEMIDLIST pidl)
{
	USHORT nLen;
	LPITEMIDLIST pidlNew;

	nLen = pidl->mkid.cb;
	if(nLen == 0) {
		return NULL;
	}

	pidlNew = (LPITEMIDLIST)pMalloc->Alloc(nLen + sizeof(USHORT));
	if(pidlNew == NULL) {
		return NULL;
	}

	CopyMemory(pidlNew, pidl, nLen);
	*((USHORT*)(((LPBYTE)pidlNew) + nLen)) = 0;

	return pidlNew;
}
