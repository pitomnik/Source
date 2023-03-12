// PopOops is pop-up blocking solution for Internet Explorer
// Copyright (C) 2002-2004 Shahin Gasanov
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

#include "Main.h"

#pragma data_seg(".shared")
	HWND	hookOwner = 0 ;
	HHOOK	hookHandle = 0 ;
#pragma data_seg()

HINSTANCE myInstance ;

LRESULT CALLBACK MouseProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	if (nCode < 0)
		return CallNextHookEx(hookHandle, nCode, wParam, lParam) ;

	if (nCode == HC_ACTION && (wParam == WM_LBUTTONUP || wParam == WM_NCLBUTTONUP))
	{
		HWND activeWindow = ((MOUSEHOOKSTRUCT*)lParam)->hwnd ;
		char className[255] ;

		GetClassName(activeWindow, className, 255) ;

		if(strcmp(className, "Internet Explorer_Server") == 0 || strcmp(className, "MacromediaFlashPlayerActiveX") == 0)
			PostMessage(hookOwner, WM_LBUTTONUP, 0, -1) ;
	}

	return CallNextHookEx(hookHandle, nCode, wParam, lParam) ;
}

HHOOK WINAPI InstallHook(HWND hWnd)
{
	hookOwner = hWnd ;

	if (hookHandle == 0)
		hookHandle = SetWindowsHookEx(WH_MOUSE, (HOOKPROC)MouseProc, myInstance, 0) ;

	return hookHandle ;
}

HHOOK WINAPI UninstallHook()
{
	hookOwner = 0 ;

	if (UnhookWindowsHookEx(hookHandle))
		hookHandle = 0 ;

	return hookHandle ;
}

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	if (fdwReason == DLL_PROCESS_ATTACH)
	{
		myInstance = (HINSTANCE)hinstDLL ;
		DisableThreadLibraryCalls(hinstDLL) ;
	}

	return TRUE ;
}

extern "C" BOOL __stdcall _DllMainCRTStartup(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
    return DllMain(hinstDLL, fdwReason, lpvReserved) ;
}
