#pragma once

#include <windows.h>
#include <shlobj.h> // For IShellExtInit and IContextMenu.

class ContextMenu : public IShellExtInit, public IContextMenu
{
public:
	// IUnknown
	IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv);
	IFACEMETHODIMP_(ULONG) AddRef();
	IFACEMETHODIMP_(ULONG) Release();

	// IShellExtInit
	IFACEMETHODIMP Initialize(LPCITEMIDLIST pidlFolder, LPDATAOBJECT pDataObj, HKEY hKeyProgID);

	// IContextMenu
	IFACEMETHODIMP QueryContextMenu(HMENU hMenu, UINT indexMenu, UINT idCmdFirst, UINT idCmdLast, UINT uFlags);
	IFACEMETHODIMP InvokeCommand(LPCMINVOKECOMMANDINFO pici);
	IFACEMETHODIMP GetCommandString(UINT_PTR idCommand, UINT uFlags, UINT *pwReserved, LPSTR pszName, UINT cchMax);

	ContextMenu(void);

protected:
	~ContextMenu(void);

private:
	// Reference count of component.
	long m_cRef;

	static const int MAX_FILE_COUNT = 100;

	// The list of the selected files.
	TCHAR m_szSelectedFiles[MAX_PATH * MAX_FILE_COUNT];

	// The method that handles the verb.
	void OnVerbInvoke(HWND hWnd);

	PWSTR m_pszMenuText;
	HBITMAP m_hMenuBmp;
	PCSTR m_pszVerb;
	PCWSTR m_pwszVerb;
	PCSTR m_pszVerbCanonicalName;
	PCWSTR m_pwszVerbCanonicalName;
	PCSTR m_pszVerbHelpText;
	PCWSTR m_pwszVerbHelpText;

	inline BOOL IsVistaOrLater()
	{
		OSVERSIONINFO osvi;

		ZeroMemory(&osvi, sizeof(OSVERSIONINFO));

		osvi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);

		GetVersionEx(&osvi);

		return (osvi.dwMajorVersion > 6) ||
			(osvi.dwMajorVersion == 6) && (osvi.dwMinorVersion >= 1);
	}

	inline HBITMAP MakeBitmapTransparent(HBITMAP hbmSrc)
	{
		HDC hdcSrc, hdcDst;
		HBITMAP hbmOld, hbmNew;
		BITMAP bm;
		COLORREF clrTP, clrBK;

		if ((hdcSrc = CreateCompatibleDC(NULL)) != NULL)
		{
			if ((hdcDst = CreateCompatibleDC(NULL)) != NULL)
			{
				int nRow, nCol;

				GetObject(hbmSrc, sizeof(bm), &bm);

				hbmOld = (HBITMAP)SelectObject(hdcSrc, hbmSrc);

				hbmNew = CreateBitmap(bm.bmWidth, bm.bmHeight, bm.bmPlanes, bm.bmBitsPixel, NULL);

				SelectObject(hdcDst, hbmNew);

				BitBlt(hdcDst, 0, 0, bm.bmWidth, bm.bmHeight, hdcSrc, 0, 0, SRCCOPY);

				clrTP = GetPixel(hdcDst, 0, 0); // get color of first pixel at 0, 0

				clrBK = GetSysColor(COLOR_MENU); // get the current background color of the menu

				for (nRow = 0; nRow < bm.bmHeight; nRow++) // work our way through all the pixels changing their color
				{
					for (nCol = 0; nCol < bm.bmWidth; nCol++) // when we hit our set transparency color
					{
						if (GetPixel(hdcDst, nCol, nRow) == clrTP)
						{
							SetPixel(hdcDst, nCol, nRow, clrBK);
						}
					}
				}

				DeleteDC(hdcDst);
			}

			DeleteDC(hdcSrc);
		}

		return hbmNew;
	}
};