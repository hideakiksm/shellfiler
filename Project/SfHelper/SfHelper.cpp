#include "pch.h"
#include "ExplorerMenu.h"


// エラーが発生したときtrue
BOOL s_error = FALSE;

//=========================================================================================
// 機　能：エクスプローラメニューを初期化する
// 引　数：なし
// 戻り値：ライブラリハンドル
//=========================================================================================
void* __stdcall InitializeExplorerMenu()
{
	ExplorerMenu* pExpMenu = new ExplorerMenu();
	return pExpMenu;
}

//=========================================================================================
// 機　能：エクスプローラメニューを作成する
// 引　数：[in]hExpMenu   ライブラリハンドル
// 　　　　[in]hwnd       メニューを表示するウィンドウ
// 　　　　[in]path       表示するファイルのパス
// 戻り値：メニューのハンドル
//=========================================================================================
HMENU __stdcall ShowExplorerMenu(void* hExpMenu, HWND hwnd, const WCHAR* path)
{
	ExplorerMenu* pMenu = (ExplorerMenu*)hExpMenu;
	HMENU hPopup = pMenu->CreateExplorerMenu((HWND)hwnd, path);
	return hPopup;
}

//=========================================================================================
// 機　能：メニュー項目を実行する
// 引　数：[in]hExpMenu   ライブラリハンドル
// 　　　　[in]nCmd       実行するメニュー項目の項番
// 戻り値：実行に成功したときtrue
//=========================================================================================
BOOL __stdcall ExecuteExplorerMenuItem(void* hExpMenu, LONG nCmd) 
{
	ExplorerMenu* pMenu = (ExplorerMenu*)hExpMenu;
	return pMenu->ExecuteMenuItem(nCmd);
}

//=========================================================================================
// 機　能：エクスプローラメニューの後始末を行う
// 引　数：[in]hExpMenu   ライブラリハンドル
// 戻り値：なし
//=========================================================================================
void __stdcall DeleteExplorerMenu(void* hExpMenu)
{
	ExplorerMenu* pMenu = (ExplorerMenu*)hExpMenu;
	delete pMenu;
}

//=========================================================================================
// 機　能：メニュー表示中のメッセージを中継する
// 引　数：[in]hExpMenu   ライブラリハンドル
// 　　　　[in]hwnd       メニューを表示中のウィンドウのハンドル
// 　　　　[in]message    ウィンドウメッセージ
// 　　　　[in]wParam     ウィンドウメッセージのパラメータ
// 　　　　[in]lParam     ウィンドウメッセージのパラメータ
// 戻り値：メッセージを処理したときTRUE
//=========================================================================================
BOOL __stdcall HandleMenuMessage(void* hExpMenu, HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam) 
{
	ExplorerMenu* pMenu = (ExplorerMenu*)hExpMenu;
	return pMenu->HandleMenuMsg(hwnd, message, wParam, lParam);
}

//=========================================================================================
// 機　能：パスワードソルトを返す
// 引　数：なし
// 戻り値：パスワードのソルト
//=========================================================================================
int __stdcall GetPasswordSalt()
{
	int index[] = {
		 4, 24, 21, 16, 18, 11, 26, 19,  6, 20, 15, 30,  0,  3, 12,  9,
		14, 27, 29,  7, 22,  2, 10,  8, 13, 17,  1, 23, 31,  5, 25, 28
	};
	SYSTEMTIME systime;
	GetSystemTime(&systime);
	DWORD wMilliseconds = systime.wMilliseconds;

	DWORD org = 0;
	DWORD date = (systime.wYear - 2000) * 400 + (systime.wMonth - 1) * 32 + systime.wDay;
	DWORD pwkey = systime.wMilliseconds ^ 0x10305070;
	GetSystemTime(&systime);
	pwkey ^= 0x02040608;
	if (s_error) {
		date ^= 0x9182;
	}
	pwkey ^= wMilliseconds;
	org = (date << 16) ^ pwkey;

	DWORD result = 0;
	for (int i = 0; i < 32; i++) {
		result = result << 1;
		result |= (org >> index[i]) & 0x1;
	}

	return result;
}
