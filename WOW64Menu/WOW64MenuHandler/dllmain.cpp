#include <windows.h>
#include <Guiddef.h>
#include "ClassFactory.h" // For the class factory.
#include "Reg.h"

// {722A9393-1D22-4B5A-800E-4BF260C2B626}
// When you write your own handler, you must create a new CLSID by using the 
// "Create GUID" tool in the Tools menu, and specify the CLSID value here.
const CLSID CLSID_ContextMenu = 
{ 0x722a9393, 0x1d22, 0x4b5a, { 0x80, 0xe, 0x4b, 0xf2, 0x60, 0xc2, 0xb6, 0x26 } };

HINSTANCE   g_hInst     = NULL;
long        g_cDllRef   = 0;

BOOL APIENTRY DllMain(HMODULE hModule, DWORD dwReason, LPVOID lpReserved)
{
	switch (dwReason)
	{
	case DLL_PROCESS_ATTACH:
		// Hold the instance of this DLL module, we will use it to get the 
		// path of the DLL to register the component.
		g_hInst = hModule;
		DisableThreadLibraryCalls(hModule);
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

//
//   FUNCTION: DllGetClassObject
//
//   PURPOSE: Create the class factory and query to the specific interface.
//
//   PARAMETERS:
//   * rclsid - The CLSID that will associate the correct data and code.
//   * riid - A reference to the identifier of the interface that the caller 
//     is to use to communicate with the class object.
//   * ppv - The address of a pointer variable that receives the interface 
//     pointer requested in riid. Upon successful return, *ppv contains the 
//     requested interface pointer. If an error occurs, the interface pointer 
//     is NULL. 
//
STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, void **ppv)
{
	HRESULT hr = CLASS_E_CLASSNOTAVAILABLE;

	if (IsEqualCLSID(CLSID_ContextMenu, rclsid))
	{
		hr = E_OUTOFMEMORY;

		ClassFactory *pClassFactory = new ClassFactory();
		if (pClassFactory)
		{
			hr = pClassFactory->QueryInterface(riid, ppv);
			pClassFactory->Release();
		}
	}

	return hr;
}

//
//   FUNCTION: DllCanUnloadNow
//
//   PURPOSE: Check if we can unload the component from the memory.
//
//   NOTE: The component can be unloaded from the memory when its reference 
//   count is zero (i.e. nobody is still using the component).
// 
STDAPI DllCanUnloadNow(void)
{
	return g_cDllRef > 0 ? S_FALSE : S_OK;
}

//
//   FUNCTION: DllRegisterServer
//
//   PURPOSE: Register the COM server and the context menu handler.
// 
STDAPI DllRegisterServer(void)
{
	HRESULT hr;

	wchar_t szModule[MAX_PATH];
	if (GetModuleFileName(g_hInst, szModule, ARRAYSIZE(szModule)) == 0)
	{
		hr = HRESULT_FROM_WIN32(GetLastError());
		return hr;
	}

	// Register the component.
	hr = RegisterInprocServer(szModule, CLSID_ContextMenu, 
		L"WOW64MenuHandler.ContextMenu Class", 
		L"Apartment");
	if (SUCCEEDED(hr))
	{
		hr = RegisterShellExtContextMenuHandler(L"LibraryLocation", 
			CLSID_ContextMenu, 
			L"WOW64MenuHandler.ContextMenu");
		hr = RegisterShellExtContextMenuHandler(L"LibraryFolder\\background", 
			CLSID_ContextMenu, 
			L"WOW64MenuHandler.ContextMenu");
		hr = RegisterShellExtContextMenuHandler(L"Directory", 
			CLSID_ContextMenu, 
			L"WOW64MenuHandler.ContextMenu");
		hr = RegisterShellExtContextMenuHandler(L"Directory\\Background", 
			CLSID_ContextMenu, 
			L"WOW64MenuHandler.ContextMenu");
		hr = RegisterShellExtContextMenuHandler(L"Folder", 
			CLSID_ContextMenu, 
			L"WOW64MenuHandler.ContextMenu");
		hr = RegisterShellExtContextMenuHandler(L"Drive", 
			CLSID_ContextMenu, 
			L"WOW64MenuHandler.ContextMenu");
		hr = RegisterShellExtContextMenuHandler(L"InternetShortcut", 
			CLSID_ContextMenu, 
			L"WOW64MenuHandler.ContextMenu");
		hr = RegisterShellExtContextMenuHandler(L"lnkfile", 
			CLSID_ContextMenu, 
			L"WOW64MenuHandler.ContextMenu");
		hr = RegisterShellExtContextMenuHandler(L"*", 
			CLSID_ContextMenu, 
			L"WOW64MenuHandler.ContextMenu");
	}

	return hr;
}

//
//   FUNCTION: DllUnregisterServer
//
//   PURPOSE: Unregister the COM server and the context menu handler.
// 
STDAPI DllUnregisterServer(void)
{
	HRESULT hr = S_OK;

	wchar_t szModule[MAX_PATH];
	if (GetModuleFileName(g_hInst, szModule, ARRAYSIZE(szModule)) == 0)
	{
		hr = HRESULT_FROM_WIN32(GetLastError());
		return hr;
	}

	// Unregister the component.
	hr = UnregisterInprocServer(CLSID_ContextMenu);
	if (SUCCEEDED(hr))
	{
		// Unregister the context menu handler.
		hr = UnregisterShellExtContextMenuHandler(L"LibraryLocation", 
			CLSID_ContextMenu);
		hr = UnregisterShellExtContextMenuHandler(L"LibraryFolder\\background", 
			CLSID_ContextMenu);
		hr = UnregisterShellExtContextMenuHandler(L"Directory", 
			CLSID_ContextMenu);
		hr = UnregisterShellExtContextMenuHandler(L"Directory\\Background", 
			CLSID_ContextMenu);
		hr = UnregisterShellExtContextMenuHandler(L"Folder", 
			CLSID_ContextMenu);
		hr = UnregisterShellExtContextMenuHandler(L"Drive", 
			CLSID_ContextMenu);
		hr = UnregisterShellExtContextMenuHandler(L"InternetShortcut", 
			CLSID_ContextMenu);
		hr = UnregisterShellExtContextMenuHandler(L"lnkfile", 
			CLSID_ContextMenu);
		hr = UnregisterShellExtContextMenuHandler(L"*", 
			CLSID_ContextMenu);
	}

	return hr;
}