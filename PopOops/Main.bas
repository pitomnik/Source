Attribute VB_Name = "modMain"
' PopOops is pop-up blocking solution for Internet Explorer
' Copyright (C) 2002-2004 Shahin Gasanov
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

Option Explicit

Public Declare Function InstallHook Lib "ClickMon.dll" (ByVal hWnd As Long) As Long
Public Declare Function UninstallHook Lib "ClickMon.dll" () As Long

Public pntTray As POINTAPI
Public hObject As Long, hHook As Long, hClickWin As Long
Public iSenseInd As Integer

Public Function GetWindowClass(ByVal Window As Long) As String
    Dim lRet As Long
    Dim sClass As String
    
    GetWindowClass = ""
    
    sClass = String$(256, vbNullChar)
    lRet = GetClassName(Window, sClass, 255)
    If lRet > 0 Then GetWindowClass = Left$(sClass, lRet)
End Function

Public Function IsWindowClicked(Window As Long) As Boolean
    IsWindowClicked = (Window = hClickWin)
    If IsWindowClicked Then hClickWin = 0
End Function

Public Function IsKeyPressed(ByVal Key As Long) As Boolean
    GetAsyncKeyState Key
    IsKeyPressed = GetAsyncKeyState(Key) < 0
End Function

Public Sub CallEvent()
    ResolvePointer(hObject).FireEvent
End Sub

Private Function ResolvePointer(ByVal Pointer As Long) As IECollection
    Dim oIE As IECollection
    
    CopyMemory oIE, hObject, 4
    Set ResolvePointer = oIE
    CopyMemory oIE, 0&, 4
End Function

Public Sub SetMouseHook(ByVal Flag As Boolean)
    If Flag And (iSenseInd > 0) Then
        If hHook = 0 Then hHook = InstallHook(frmMain.picHook.hWnd)
    Else
        If hHook <> 0 Then hHook = UninstallHook
    End If
End Sub

Public Function ReadInternetFile(ByVal Agent As String, ByVal URL As String) As String
    Const MAX_CHUNK = 4096
    Dim hSession As Long, hLink As Long
    Dim lResult As Long, lBytes As Long
    Dim sChunk As String * MAX_CHUNK, sBuffer As String
    
    ReadInternetFile = ""
    
    hSession = InternetOpen(Agent, INTERNET_OPEN_TYPE_DIRECT, vbNullString, vbNullString, 0)
    hLink = InternetOpenUrl(hSession, URL, vbNullString, 0, INTERNET_FLAG_RELOAD, 0)

    Do
        sChunk = ""
        lResult = InternetReadFile(hLink, sChunk, MAX_CHUNK, lBytes)
        sBuffer = sBuffer & Left$(sChunk, lBytes)
    Loop While lResult And lBytes
    
    InternetCloseHandle hLink
    InternetCloseHandle hSession
    
    ReadInternetFile = sBuffer
End Function
