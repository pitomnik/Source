#include "ContextMenu.h"
#include "resource.h"
#include <strsafe.h>
#include <Shlwapi.h>
#pragma comment(lib, "shlwapi.lib")

extern HINSTANCE g_hInst;
extern long g_cDllRef;

#define IDM_DISPLAY 0 // The command's identifier offset.

ContextMenu::ContextMenu(void) : m_cRef(1), 
	m_pszMenuText(L"WOW&64 (32-bit) Menu"),
	m_pszVerb("WOW64Menu"),
	m_pwszVerb(L"WOW64Menu"),
	m_pszVerbCanonicalName("WOW64Menu"),
	m_pwszVerbCanonicalName(L"WOW64Menu"),
	m_pszVerbHelpText("WOW64 (32-bit) Menu"),
	m_pwszVerbHelpText(L"WOW64 (32-bit) Menu")
{
	InterlockedIncrement(&g_cDllRef);

	// Load the bitmap for the menu item. 
	// If you want the menu item bitmap to be transparent, the color depth of 
	// the bitmap must not be greater than 8bpp.
	m_hMenuBmp = (HBITMAP)LoadImage(g_hInst, MAKEINTRESOURCE(IDB_ICON), 
		IMAGE_BITMAP, 0, 0, LR_DEFAULTSIZE | LR_LOADTRANSPARENT);

	if (IsVistaOrLater())
	{
		m_hMenuBmp = MakeBitmapTransparent(m_hMenuBmp);
	}
}

ContextMenu::~ContextMenu(void)
{
	if (m_hMenuBmp)
	{
		DeleteObject(m_hMenuBmp);
		m_hMenuBmp = NULL;
	}

	InterlockedDecrement(&g_cDllRef);
}

void ContextMenu::OnVerbInvoke(HWND hWnd)
{
	try
	{
		TCHAR lpParameters[MAX_PATH * MAX_FILE_COUNT];

		lstrcpy(lpParameters, L"\"");
		lstrcat(lpParameters, m_szSelectedFiles);
		lstrcat(lpParameters, L"\"");

		ShellExecute(hWnd, NULL, L"WOW64Menu.exe", lpParameters, NULL, SW_SHOW);
	}
	catch (...)
	{
		// do nothing
	}
}

#pragma region IUnknown

// Query to the interface the component supported.
IFACEMETHODIMP ContextMenu::QueryInterface(REFIID riid, void **ppv)
{
	static const QITAB qit[] = 
	{
		QITABENT(ContextMenu, IContextMenu),
		QITABENT(ContextMenu, IShellExtInit), 
		{ 0 },
	};
	return QISearch(this, qit, riid, ppv);
}

// Increase the reference count for an interface on an object.
IFACEMETHODIMP_(ULONG) ContextMenu::AddRef()
{
	return InterlockedIncrement(&m_cRef);
}

// Decrease the reference count for an interface on an object.
IFACEMETHODIMP_(ULONG) ContextMenu::Release()
{
	ULONG cRef = InterlockedDecrement(&m_cRef);
	if (0 == cRef)
	{
		delete this;
	}

	return cRef;
}

#pragma endregion

#pragma region IShellExtInit

// Initialize the context menu handler.
IFACEMETHODIMP ContextMenu::Initialize(
	LPCITEMIDLIST pidlFolder, LPDATAOBJECT pDataObj, HKEY hKeyProgID)
{
	if (NULL == pDataObj)
	{
		return SHGetPathFromIDList(pidlFolder, m_szSelectedFiles) ? S_OK : E_INVALIDARG;
	}

	HRESULT hr;
	IShellItemArray *psia;

	m_szSelectedFiles[0] = 0;

	hr = SHCreateShellItemArrayFromDataObject(pDataObj, IID_IShellItemArray, (void**)&psia);

	if (SUCCEEDED(hr))
	{
		IEnumShellItems *pesi;

		hr = psia->EnumItems(&pesi);

		if (SUCCEEDED(hr))
		{
			IShellItem *psi;
			PWSTR pszFilePath = NULL;

			while (pesi->Next(1, &psi, NULL) == S_OK)
			{
				if (psi->GetDisplayName(SIGDN_FILESYSPATH, &pszFilePath) == S_OK)
				{
					if (m_szSelectedFiles[0] != 0)
					{
						lstrcat(m_szSelectedFiles, L"|");
					}
					lstrcat(m_szSelectedFiles, pszFilePath);

					CoTaskMemFree(pszFilePath);
				}
			}

			psi->Release();
		}

		psia->Release();
	}

	// No files selected.
	if (m_szSelectedFiles[0] == 0)
	{
		hr = E_FAIL;
	}

	// If any value other than S_OK is returned from the method, the context 
	// menu item is not displayed.
	return hr;
}

#pragma endregion

#pragma region IContextMenu

//
//   FUNCTION: ContextMenu::QueryContextMenu
//
//   PURPOSE: The Shell calls IContextMenu::QueryContextMenu to allow the 
//            context menu handler to add its menu items to the menu. It 
//            passes in the HMENU handle in the hmenu parameter. The 
//            indexMenu parameter is set to the index to be used for the 
//            first menu item that is to be added.
//
IFACEMETHODIMP ContextMenu::QueryContextMenu(
	HMENU hMenu, UINT indexMenu, UINT idCmdFirst, UINT idCmdLast, UINT uFlags)
{
	// If uFlags include CMF_DEFAULTONLY then we should not do anything.
	if (CMF_DEFAULTONLY & uFlags)
	{
		return MAKE_HRESULT(SEVERITY_SUCCESS, 0, USHORT(0));
	}

	// Use either InsertMenu or InsertMenuItem to add menu items.
	// Learn how to add sub-menu from:
	// http://www.codeproject.com/KB/shell/ctxextsubmenu.aspx

	MENUITEMINFO mii = { sizeof(mii) };
	mii.fMask = MIIM_BITMAP | MIIM_STRING | MIIM_FTYPE | MIIM_ID | MIIM_STATE;
	mii.wID = idCmdFirst + IDM_DISPLAY;
	mii.fType = MFT_STRING;
	mii.dwTypeData = m_pszMenuText;
	mii.fState = MFS_ENABLED;
	mii.hbmpItem = static_cast<HBITMAP>(m_hMenuBmp);
	if (!InsertMenuItem(hMenu, indexMenu, TRUE, &mii))
	{
		return HRESULT_FROM_WIN32(GetLastError());
	}

	// Add a separator.
	MENUITEMINFO sep = { sizeof(sep) };
	sep.fMask = MIIM_TYPE;
	sep.fType = MFT_SEPARATOR;
	if (!InsertMenuItem(hMenu, indexMenu + 1, TRUE, &sep))
	{
		return HRESULT_FROM_WIN32(GetLastError());
	}

	// Return an HRESULT value with the severity set to SEVERITY_SUCCESS. 
	// Set the code value to the offset of the largest command identifier 
	// that was assigned, plus one (1).
	return MAKE_HRESULT(SEVERITY_SUCCESS, 0, USHORT(IDM_DISPLAY + 1));
}

//
//   FUNCTION: ContextMenu::InvokeCommand
//
//   PURPOSE: This method is called when a user clicks a menu item to tell 
//            the handler to run the associated command. The lpcmi parameter 
//            points to a structure that contains the needed information.
//
IFACEMETHODIMP ContextMenu::InvokeCommand(LPCMINVOKECOMMANDINFO pici)
{
	BOOL fUnicode = FALSE;

	// Determine which structure is being passed in, CMINVOKECOMMANDINFO or 
	// CMINVOKECOMMANDINFOEX based on the cbSize member of lpcmi. Although 
	// the lpcmi parameter is declared in Shlobj.h as a CMINVOKECOMMANDINFO 
	// structure, in practice it often points to a CMINVOKECOMMANDINFOEX 
	// structure. This struct is an extended version of CMINVOKECOMMANDINFO 
	// and has additional members that allow Unicode strings to be passed.
	if (pici->cbSize == sizeof(CMINVOKECOMMANDINFOEX))
	{
		if (pici->fMask & CMIC_MASK_UNICODE)
		{
			fUnicode = TRUE;
		}
	}

	// Determines whether the command is identified by its offset or verb.
	// There are two ways to identify commands:
	// 
	//   1) The command's verb string 
	//   2) The command's identifier offset
	// 
	// If the high-order word of lpcmi->lpVerb (for the ANSI case) or 
	// lpcmi->lpVerbW (for the Unicode case) is nonzero, lpVerb or lpVerbW 
	// holds a verb string. If the high-order word is zero, the command 
	// offset is in the low-order word of lpcmi->lpVerb.

	// For the ANSI case, if the high-order word is not zero, the command's 
	// verb string is in lpcmi->lpVerb. 
	if (!fUnicode && HIWORD(pici->lpVerb))
	{
		// Is the verb supported by this context menu extension?
		if (StrCmpIA(pici->lpVerb, m_pszVerb) == 0)
		{
			OnVerbInvoke(pici->hwnd);
		}
		else
		{
			// If the verb is not recognized by the context menu handler, it 
			// must return E_FAIL to allow it to be passed on to the other 
			// context menu handlers that might implement that verb.
			return E_FAIL;
		}
	}

	// For the Unicode case, if the high-order word is not zero, the 
	// command's verb string is in lpcmi->lpVerbW. 
	else if (fUnicode && HIWORD(((CMINVOKECOMMANDINFOEX*)pici)->lpVerbW))
	{
		// Is the verb supported by this context menu extension?
		if (StrCmpIW(((CMINVOKECOMMANDINFOEX*)pici)->lpVerbW, m_pwszVerb) == 0)
		{
			OnVerbInvoke(pici->hwnd);
		}
		else
		{
			// If the verb is not recognized by the context menu handler, it 
			// must return E_FAIL to allow it to be passed on to the other 
			// context menu handlers that might implement that verb.
			return E_FAIL;
		}
	}

	// If the command cannot be identified through the verb string, then 
	// check the identifier offset.
	else
	{
		// Is the command identifier offset supported by this context menu 
		// extension?
		if (LOWORD(pici->lpVerb) == IDM_DISPLAY)
		{
			OnVerbInvoke(pici->hwnd);
		}
		else
		{
			// If the verb is not recognized by the context menu handler, it 
			// must return E_FAIL to allow it to be passed on to the other 
			// context menu handlers that might implement that verb.
			return E_FAIL;
		}
	}

	return S_OK;
}

//
//   FUNCTION: ContextMenu::GetCommandString
//
//   PURPOSE: If a user highlights one of the items added by a context menu 
//            handler, the handler's IContextMenu::GetCommandString method is 
//            called to request a Help text string that will be displayed on 
//            the Windows Explorer status bar. This method can also be called 
//            to request the verb string that is assigned to a command. 
//            Either ANSI or Unicode verb strings can be requested. This 
//            example only implements support for the Unicode values of 
//            uFlags, because only those have been used in Windows Explorer 
//            since Windows 2000.
//
IFACEMETHODIMP ContextMenu::GetCommandString(UINT_PTR idCommand, 
	UINT uFlags, UINT *pwReserved, LPSTR pszName, UINT cchMax)
{
	HRESULT hr = E_INVALIDARG;

	if (idCommand == IDM_DISPLAY)
	{
		switch (uFlags)
		{
		case GCS_HELPTEXTW:
			// Only useful for pre-Vista versions of Windows that have a 
			// Status bar.
			hr = StringCchCopy(reinterpret_cast<PWSTR>(pszName), cchMax, 
				m_pwszVerbHelpText);
			break;

		case GCS_VERBW:
			// GCS_VERBW is an optional feature that enables a caller to 
			// discover the canonical name for the verb passed in through 
			// idCommand.
			hr = StringCchCopy(reinterpret_cast<PWSTR>(pszName), cchMax, 
				m_pwszVerbCanonicalName);
			break;

		default:
			hr = S_OK;
		}
	}

	// If the command (idCommand) is not supported by this context menu 
	// extension handler, return E_INVALIDARG.

	return hr;
}

#pragma endregion