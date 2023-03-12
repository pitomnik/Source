VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "IECollection"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
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

Public Event PopupBlocked()

Private WithEvents shwWindows As SHDocVw.ShellWindows
Attribute shwWindows.VB_VarHelpID = -1
Private colMembers As Collection

Private Sub Class_Initialize()
    hObject = ObjPtr(Me)
    Set shwWindows = New SHDocVw.ShellWindows
    Refresh
End Sub

Private Sub Class_Terminate()
    SetMouseHook False
    Set shwWindows = Nothing
    Set colMembers = Nothing
End Sub

Public Property Get NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
    Set NewEnum = colMembers.[_NewEnum]
End Property

Public Sub Refresh()
    On Error Resume Next
    
    Dim shwWin As SHDocVw.InternetExplorer
    
    Set colMembers = Nothing
    Set colMembers = New Collection
    
    For Each shwWin In shwWindows
        Add shwWin
    Next
    
    SetMouseHook (colMembers.Count > 0)
    
    Set shwWin = Nothing
End Sub

Private Sub Add(ByVal IEInstance As SHDocVw.InternetExplorer)
    Dim objMember As New IEMember
    
    If GetWindowClass(IEInstance.hWnd) = "IEFrame" Then
        Set objMember.Instance = IEInstance
        colMembers.Add objMember
    End If
    
    Set objMember = Nothing
End Sub

Private Sub shwWindows_WindowRegistered(ByVal lCookie As Long)
    Refresh
End Sub

Private Sub shwWindows_WindowRevoked(ByVal lCookie As Long)
    Refresh
End Sub

Friend Sub FireEvent()
    RaiseEvent PopupBlocked
End Sub
